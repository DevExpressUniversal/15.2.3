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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraScheduler.Data;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Xml;
using DevExpress.XtraScheduler.UI;
using System.Linq;
using DevExpress.XtraScheduler.Internal.Implementations;
#if !SL
using System.Drawing.Design;
using System.Xml;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal;
#else
using System.Windows.Media;
using DevExpress.XtraScheduler;
#endif
namespace DevExpress.XtraScheduler {
	#region AppointmentType
	public enum AppointmentType {
		Normal = 0,
		Pattern = 1,
		Occurrence = 2,
		ChangedOccurrence = 3,
		DeletedOccurrence = 4
	};
	#endregion
	public interface Appointment : IPersistentObject {
		DateTime Start { get; set; }
		DateTime End { get; set; }
		TimeSpan Duration { get; set; }
		bool AllDay { get; set; }
		string Subject { get; set; }
		string Description { get; set; }
		string Location { get; set; }
		object LabelKey { get; set; }
		object StatusKey { get; set; }
		[Obsolete("Use the LabelKey property instead.", false)]
		int LabelId { get; set; }
		[Obsolete("Use the StatusKey property instead.", false)]
		int StatusId { get; set; }
		string TimeZoneId { get; set; }
		object ResourceId { get; set; }
		AppointmentResourceIdCollection ResourceIds { get; }
		bool LongerThanADay { get; }
		AppointmentType Type { get; }
		int PercentComplete { get; set; }
		bool SameDay { get; }
		Appointment RecurrencePattern { get; }
		IRecurrenceInfo RecurrenceInfo { get; }
		bool IsBase { get; }
		bool HasExceptions { get; }
		bool IsRecurring { get; }
		int RecurrenceIndex { get; }
		bool IsOccurrence { get; }
		bool IsException { get; }
		bool HasReminder { get; set; }
		Reminder Reminder { get; }
		ReminderCollection Reminders { get; }
		void Assign(Appointment src);
		Appointment Copy();
		AppointmentBaseCollection GetExceptions();
		Appointment CreateException(AppointmentType type, int recurrenceIndex);
		Appointment FindException(int recurrenceIndex);
		void DeleteExceptions();
		Appointment GetOccurrence(int recurrenceIndex);
		void RestoreOccurrence();
		Reminder CreateNewReminder();
	}
	#region AppointmentStartDateComparer
	public class AppointmentStartDateComparer : IComparer<Appointment>, IComparer {
		int IComparer.Compare(object x, object y) {
			Appointment aptX = (Appointment)x;
			Appointment aptY = (Appointment)y;
			return CompareCore(aptX, aptY);
		}
		public int Compare(Appointment x, Appointment y) {
			return CompareCore(x, y);
		}
		protected internal virtual int CompareCore(Appointment aptX, Appointment aptY) {
			return aptX.Start.CompareTo(aptY.Start);
		}
	}
	#endregion
	#region AppointmentComparer
	public abstract class AppointmentBaseComparer : IComparer<Appointment>, IComparer {
		protected internal abstract bool ApplyLongerThanDayComparison { get; }
		int IComparer.Compare(object x, object y) {
			Appointment aptX = (Appointment)x;
			Appointment aptY = (Appointment)y;
			return CompareCore(aptX, aptY);
		}
		public int Compare(Appointment x, Appointment y) {
			return CompareCore(x, y);
		}
		protected internal virtual int IsSeveralDaysAppointment(Appointment apt) {
			return apt.LongerThanADay ? 1 : 0;
		}
		protected internal virtual int CompareCore(Appointment aptX, Appointment aptY) {
			int result;
			if (ApplyLongerThanDayComparison) {
				result = IsSeveralDaysAppointment(aptY) - IsSeveralDaysAppointment(aptX);
				if (result != 0)
					return result;
			}
			result = aptX.Start.CompareTo(aptY.Start);
			if (result != 0)
				return result;
			result = -aptX.End.CompareTo(aptY.End);
			if (result != 0)
				return result;
			result = CompareAppointmentHandles(aptX, aptY);
			if (result != 0)
				return result;
			aptX = aptX.RecurrencePattern;
			aptY = aptY.RecurrencePattern;
			if (aptX == null)
				return (aptY == null) ? 0 : 1;
			else {
				if (aptY == null)
					return -1;
				else
					return CompareAppointmentHandles(aptX, aptY);
			}
		}
		protected internal virtual int CompareAppointmentHandles(Appointment aptX, Appointment aptY) {
			if ((aptX.RowHandle is int) && (aptY.RowHandle is int)) {
				int x = (int)aptX.RowHandle;
				int y = (int)aptY.RowHandle;
				return CompareHandlersAsInt(x, y);
			}
			IComparable comparableX = aptX.RowHandle as IComparable;
			IComparable comparableY = aptY.RowHandle as IComparable;
			if (comparableX != null && comparableY != null)
				return comparableX.CompareTo(comparableY);
			else
				return 0;
		}
		int CompareHandlersAsInt(int x, int y) {
			if (x == y)
				return 0;
			if (x < 0 && y >= 0)
				return 1;
			if (x >= 0 && y < 0)
				return -1;
			return x.CompareTo(y);
		}
	}
	public class AppointmentComparer : AppointmentBaseComparer {
		protected internal override bool ApplyLongerThanDayComparison { get { return true; } }
		public AppointmentComparer() {
			XtraSchedulerDebug.Assert(false);
		}
	}
	#endregion
	public class WeekViewAppointmentComparer : AppointmentBaseComparer {
		protected internal override bool ApplyLongerThanDayComparison { get { return true; } }
	}
	public class AppointmentTimeIntervalComparer : AppointmentBaseComparer {
		protected internal override bool ApplyLongerThanDayComparison { get { return false; } }
	}
	public class TimeLineAppointmentComparer : AppointmentBaseComparer {
		protected internal override bool ApplyLongerThanDayComparison { get { return true; } }
		protected internal override int IsSeveralDaysAppointment(Appointment apt) {
			return base.IsSeveralDaysAppointment(apt) * -1;
		}
	}
	#region AppointmentCollection
	public class AppointmentCollection : AppointmentBaseCollection {
		#region Fields
		Dictionary<object, Appointment> idHash = new Dictionary<object, Appointment>();
		ISchedulerStorageBase storage;
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AppointmentCollection() {
			this.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
		}
		public AppointmentCollection(ISchedulerStorageBase storage) {
			if (storage == null)
				Exceptions.ThrowArgumentException("storage", storage);
			this.storage = storage;
			this.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
		}
		public override void AddRange(ICollection collection) {
			AddRangeCore(collection);
		}
		#region Properties
		internal Dictionary<object, Appointment> IdHash { get { return idHash; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentCollectionStorage")]
#endif
		public ISchedulerStorageBase Storage { get { return storage; } }
		#endregion
		protected internal void SetStorageCore(ISchedulerStorageBase storage) {
			this.storage = storage;
		}
		public virtual void ReadXml(string fileName) {
			CreateXmlConverter().ReadXml(fileName);
			CommitItems();
		}
		public virtual void ReadXml(Stream stream) {
			CreateXmlConverter().ReadXml(stream);
			CommitItems();
		}
		void CommitItems() {
			if (!storage.Appointments.UnboundMode)
				return;
			CommitItems(storage.Appointments.Items);
		}
		void CommitItems(AppointmentBaseCollection appointments) {
			for (int i = 0; i < appointments.Count; i++) {
				Appointment apt = appointments[i];
				if (apt.Type == AppointmentType.DeletedOccurrence || apt.Type == AppointmentType.Occurrence)
					continue;
				apt.RowHandle = -1;
				((IInternalPersistentObjectStorage<Appointment>)storage.Appointments).CommitNewObject(apt);
				if (apt.Type == AppointmentType.Pattern) {
					CommitItems(apt.GetExceptions());
				}
			}
		}
		public virtual void WriteXml(string fileName) {
			CreateXmlConverter().WriteXml(fileName);
		}
		public virtual void WriteXml(Stream stream) {
			CreateXmlConverter().WriteXml(stream);
		}
		protected internal StorageXmlConverter<Appointment> CreateXmlConverter() {
			IAppointmentStorageBase objectStorage = storage.Appointments;
			return new StorageXmlConverter<Appointment>(objectStorage, new AppointmentCollectionXmlPersistenceHelper(objectStorage));
		}
		public override int Add(Appointment apt) {
			if (apt == null || !apt.IsBase)
				Exceptions.ThrowArgumentException("apt", apt);
			return base.Add(apt);
		}
		public bool IsNewAppointment(Appointment apt) {
			if (Contains(apt))
				return false;
			Appointment pattern = apt.RecurrencePattern;
			if (pattern != null) {
				if (apt.Type == AppointmentType.Occurrence)
					return false;
				return IsNewAppointment(pattern);
			}
			return true;
		}
		public static TimeInterval CalcPatternInterval(Appointment pattern) {
			DateTime start = DateTime.MaxValue;
			DateTime end = DateTime.MinValue;
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo, ((IInternalAppointment)pattern).TimeZoneEngine);
			int startIndex = 0;
			int endIndex = -1;
			if (pattern.RecurrenceInfo.Range == RecurrenceRange.OccurrenceCount) {
				DateTime startRange = pattern.RecurrenceInfo.Start;
				DateTime endRange = calc.CalcOccurrenceStartTime(pattern.RecurrenceInfo.OccurrenceCount - 1);
				TimeInterval interval = new TimeInterval(startRange, endRange + DateTimeHelper.DaySpan);
				AppointmentBaseCollection apts = calc.CalcOccurrences(interval, pattern);
				endIndex = apts.Count - 1;
			}
			if (pattern.RecurrenceInfo.Range == RecurrenceRange.EndByDate) {
				TimeInterval interval = new TimeInterval(pattern.RecurrenceInfo.Start, pattern.RecurrenceInfo.End.Date + DateTimeHelper.DaySpan);
				AppointmentBaseCollection apts = calc.CalcOccurrences(interval, pattern);
				endIndex = apts.Count - 1;
			}
			int aptsCount = endIndex + 1;
			AppointmentBaseCollection exceptions = pattern.GetExceptions();
			int count = exceptions.Count;
			if (count != 0) {
				int[] exceptionsIndex = new int[count];
				for (int i = 0; i < count; i++) {
					Appointment exception = exceptions[i];
					if (exception.Start < start)
						start = exception.Start;
					if (exception.End > end)
						end = exception.End;
					exceptionsIndex[i] = exception.RecurrenceIndex;
				}
				Array.Sort(exceptionsIndex);
				while (startIndex < count && exceptionsIndex[startIndex] == startIndex)
					startIndex++;
				int j = count - 1;
				while (j >= 0 && exceptionsIndex[j] == endIndex) {
					j--;
					endIndex--;
				}
			}
			if (startIndex < aptsCount || aptsCount == 0) {
				DateTime startPattern = calc.CalcOccurrenceStartTime(startIndex);
				if (startPattern < start)
					start = startPattern;
			}
			if (endIndex != -1) {
				DateTime endPattern = calc.CalcOccurrenceStartTime(endIndex).Add(pattern.Duration);
				if (endPattern > end)
					end = endPattern;
			}
			return new TimeInterval(start, end == DateTime.MinValue ? DateTime.MaxValue : end);
		}
		[Obsolete("You should use the 'AppointmentConflictsCalculator.CalculateConflicts' instead.", true), EditorBrowsable(EditorBrowsableState.Never)]
		public AppointmentBaseCollection GetConflicts(Appointment appointment, TimeInterval interval) {
			return null;
		}
		public static bool AreIntersecting(Appointment apt1, Appointment apt2) {
			AppointmentConflictsCalculator calculator = new AppointmentConflictsCalculator(new AppointmentBaseCollection());
			return calculator.AreIntersecting(apt1, apt2);
		}
		[Obsolete("You should use the 'AppointmentConflictsCalculator.IsIntersecting' instead", true), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsIntersecting(TimeInterval interval, Resource resource) {
			return false;
		}
		protected override void OnInsertComplete(int index, Appointment value) {
			base.OnInsertComplete(index, value);
			if (value.Id != null)
				AddAppointmentToHash(value);
		}
		private void AddAppointmentToHash(Appointment value) {
			if (!IdHash.ContainsKey(value.Id))
				IdHash.Add(value.Id, value);
			else
				IdHash[value.Id] = value;
		}
		internal void UpdateIdHash(Appointment apt) {
			if (apt.Id == null)
				return;
			RemoveAppointmentFromHash(apt);
			AddAppointmentToHash(apt);
		}
		internal virtual void RemoveAppointmentFromHash(Appointment apt) {
			List<object> keysToRemove = new List<object>();
			Appointment keyValue;
			foreach (object key in IdHash.Keys) {
				if (!IdHash.TryGetValue(key, out keyValue))
					continue;
				if (Object.ReferenceEquals(keyValue, apt))
					keysToRemove.Add(key);
			}
			foreach (object key in keysToRemove)
				IdHash.Remove(key);
		}
		protected override void OnRemoveComplete(int index, Appointment value) {
			base.OnRemoveComplete(index, value);
			object id = value.Id;
			if (id != null && IdHash.ContainsKey(id))
				IdHash.Remove(id);
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			IdHash.Clear();
		}
		public Appointment GetAppointmentById(object id) {
			return AppointmentExists(id) ? idHash[id] : null;
		}
		protected internal bool AppointmentExists(object id) {
			return id != null ? idHash.ContainsKey(id) : false;
		}
	}
	#endregion
	#region IAppointmentFactory
	public interface IAppointmentFactory {
		Appointment CreateAppointment(AppointmentType type);
	}
	#endregion
	#region IResourceFactory
	public interface IResourceFactory {
		Resource CreateResource();
	}
	#endregion    
	public interface IAppointmentStorageBase : IPersistentObjectStorage<Appointment> {
		IAppointmentLabelStorage Labels { get; }
		IAppointmentStatusStorage Statuses { get; }
		new AppointmentMappingInfo Mappings { get; }
		bool ResourceSharing { get; set; }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new AppointmentCollection Items { get; }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		bool SupportsRecurrence { get; }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		bool SupportsReminders { get; }
		void SetAppointmentFactory(IAppointmentFactory factory);
		bool IsNewAppointment(Appointment appointment);
		int Add(Appointment appointment);
		void AddRange(Appointment[] items);
		void Remove(Appointment apt);
		bool Contains(Appointment apt);
		Appointment GetAppointmentById(object id);
		Appointment CreateAppointment(AppointmentType type);
		Appointment CreateAppointment(AppointmentType type, DateTime start, DateTime end);
		Appointment CreateAppointment(AppointmentType type, DateTime start, TimeSpan duration);
		Appointment CreateAppointment(AppointmentType type, DateTime start, DateTime end, string subject);
		Appointment CreateAppointment(AppointmentType type, DateTime start, TimeSpan duration, string subject);
		void LoadFromXml(Stream stream);
		void LoadFromXml(string fileName);
		void SaveToXml(Stream stream);
		void SaveToXml(string fileName);
		event SchedulerUnhandledExceptionEventHandler LoadException;
	}
	public interface IStorageCollection<T> : IEnumerable<T> where T : IUserInterfaceObject {
		int Count { get; }
		int IndexOf(T item);
		T GetById(object id);
		T GetByIndex(int index);
		int Add(T item);
		int Add(string displayName);
		int Add(string displayName, string menuCaption);
		void AddRange(ICollection collection);
		bool Remove(T item);
		void LoadDefaults();
		void Clear();
		bool HasDefaultContent();
	}
	public interface IAppointmentStatusStorage : IStorageCollection<IAppointmentStatus>, IDisposable {
		IAppointmentStatus GetByType(AppointmentStatusType type);
		IAppointmentStatus CreateNewStatus(string displayName);
		IAppointmentStatus CreateNewStatus(object id, string displayName);
		IAppointmentStatus CreateNewStatus(object id, string displayName, string menuCaption);
	}
	public interface IAppointmentLabelStorage : IStorageCollection<IAppointmentLabel>, IDisposable {
		IAppointmentLabel CreateNewLabel(string displayName);
		IAppointmentLabel CreateNewLabel(object id, string displayName);
		IAppointmentLabel CreateNewLabel(object id, string displayName, string menuCaption);
	}
	#region AppointmentStorageBase
#if !SL
	[Editor("DevExpress.XtraScheduler.Design.AppointmentStorageTypeEditor," + AssemblyInfo.SRAssemblySchedulerDesign, typeof(UITypeEditor))]
#endif
	public abstract class AppointmentStorageBase : PersistentObjectStorage<Appointment>, IAppointmentStorageBase, IInternalAppointmentStorage, IAppointmentLoaderProvider, IAppointmentMappingsProvider, ISupportsAppointmentTransaction {
		#region Fields
		IAppointmentFactory appointmentFactory;
		IAppointmentLabelStorage labels;
		IAppointmentStatusStorage statuses;
		NotificationCollectionChangedListener<IAppointmentLabel> labelsListener;
		NotificationCollectionChangedListener<IAppointmentStatus> statusesListener;
		bool inAppointmentsTransaction;
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected AppointmentStorageBase() {
			Initialize();
		}
		protected AppointmentStorageBase(ISchedulerStorageBase storage)
			: base(storage) {
			Initialize();
		}
		void Initialize() {
			this.appointmentFactory = new DefaultAppointmentFactory(Storage);
			this.labels = CreateAppointmentLabelCollection();
			this.statuses = CreateAppointmentStatusCollection();
			this.labelsListener = CreateLabelsListener();
			this.statusesListener = CreateStatusesListener();
			labels.LoadDefaults();
			statuses.LoadDefaults();
			SubscribeUserInterfaceObjectsEvents();
		}
		protected abstract IAppointmentLabelStorage CreateAppointmentLabelCollection();
		protected abstract IAppointmentStatusStorage CreateAppointmentStatusCollection();
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentStorageBaseResourceSharing"),
#endif
		DefaultValue(false), Category(SRCategoryNames.Behavior), NotifyParentProperty(true), AutoFormatDisable()]
		public bool ResourceSharing { get { return Mappings.ResourceSharing; } set { Mappings.ResourceSharing = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SupportsRecurrence { get { return ActualMappings[AppointmentSR.Type] != null && ActualMappings[AppointmentSR.RecurrenceInfo] != null; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SupportsReminders { get { return ActualMappings[AppointmentSR.ReminderInfo] != null; } }
		[Browsable(false), Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Appointment this[int index] { get { return (Appointment)base[index]; } }
		[Browsable(false), Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new AppointmentCollection Items { get { return (AppointmentCollection)base.Items; } }
		[ Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new AppointmentMappingInfo Mappings { get { return (AppointmentMappingInfo)base.Mappings; } set { base.Mappings = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new AppointmentDataManager DataManager { get { return (AppointmentDataManager)base.DataManager; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IAppointmentFactory AppointmentFactory { get { return appointmentFactory; } }
		protected virtual bool SupportsAppointmentTransaction { get { return false; } }
		protected bool InAppointmentsTransaction { get { return inAppointmentsTransaction; } }
		#region Filter
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentStorageBaseFilter"),
#endif
		XtraSerializableProperty(), DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		NotifyParentProperty(true), AutoFormatDisable()]
		public override string Filter { get { return base.Filter; } set { base.Filter = value; } }
		#endregion
		protected internal IAppointmentStatusStorage InnerStatuses {
			get { return statuses; }
			set {
				if (statuses == value)
					return;
				SetInnerStatusesCore(value);
			}
		}
		protected internal IAppointmentLabelStorage InnerLabels {
			get { return labels; }
			set {
				if (labels == value)
					return;
				SetInnerLabelsCore(value);
			}
		}
		protected internal NotificationCollectionChangedListener<IAppointmentLabel> LabelsListener { get { return labelsListener; } }
		protected internal NotificationCollectionChangedListener<IAppointmentStatus> StatusesListener { get { return statusesListener; } }
		#endregion
		#region Events
		#region LoadException
		SchedulerUnhandledExceptionEventHandler onLoadException;
		public event SchedulerUnhandledExceptionEventHandler LoadException { add { onLoadException += value; } remove { onLoadException -= value; } }
		public virtual bool RaiseLoadException(Exception exception) {
			if (onLoadException == null)
				return false;
			SchedulerUnhandledExceptionEventArgs ea = new SchedulerUnhandledExceptionEventArgs(exception);
			onLoadException(this, ea);
			return ea.Handled;
		}
		#endregion
		#region UIChanged
		EventHandler onUIChanged;
		public event EventHandler UIChanged { add { onUIChanged += value; } remove { onUIChanged -= value; } }
		protected internal virtual void RaiseUIChanged() {
			if (onUIChanged != null)
				onUIChanged(this, EventArgs.Empty);
		}
		#endregion
		#region AppointmentsTransactionCompleted
		AppointmentsTransactionEventHandler onAppointmentsTransactionCompleted;
		event AppointmentsTransactionEventHandler IInternalAppointmentStorage.AppointmentsTransactionCompleted { add { onAppointmentsTransactionCompleted += value; } remove { onAppointmentsTransactionCompleted -= value; } }
		protected internal virtual void RaiseAppointmentsTransactionCompleted(AppointmentsTransactionEventArgs e) {
			if (onAppointmentsTransactionCompleted != null)
				onAppointmentsTransactionCompleted(this, e);
		}
		#endregion
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (labelsListener != null && statusesListener != null) {
						UnsubscribeUserInterfaceObjectsEvents();
						labelsListener = null;
						statusesListener = null;
					}
					if (labels != null) {
						labels.Dispose();
						labels = null;
					}
					if (statuses != null) {
						statuses.Dispose();
						statuses = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		#region Pseudo ISupportInitialize implementation
		public override void BeginInit() {
			base.BeginInit();
			UnsubscribeUserInterfaceObjectsEvents();
			InnerLabels.Clear();
			InnerStatuses.Clear();
		}
		public override void EndInit() {
			if (InnerLabels.Count <= 0)
				InnerLabels.LoadDefaults();
			if (InnerStatuses.Count <= 0)
				InnerStatuses.LoadDefaults();
			SubscribeUserInterfaceObjectsEvents();
			base.EndInit();
		}
		#endregion
		protected internal virtual void SubscribeUserInterfaceObjectsEvents() {
			SubscribeLabelsEvents();
			SubscribeStatusesEvents();
		}
		protected internal virtual void UnsubscribeUserInterfaceObjectsEvents() {
			UnsubscribeLabelsEvents();
			UnsubscribeStatusesEvents();
		}
		protected internal virtual void SubscribeLabelsEvents() {
			labelsListener.Changed += new EventHandler(OnUserInterfaceObjectsChanged);
		}
		protected internal virtual void UnsubscribeLabelsEvents() {
			labelsListener.Changed -= new EventHandler(OnUserInterfaceObjectsChanged);
		}
		protected internal virtual void SubscribeStatusesEvents() {
			statusesListener.Changed += new EventHandler(OnUserInterfaceObjectsChanged);
		}
		protected internal virtual void UnsubscribeStatusesEvents() {
			statusesListener.Changed -= new EventHandler(OnUserInterfaceObjectsChanged);
		}
		protected internal virtual void OnUserInterfaceObjectsChanged(object sender, EventArgs e) {
			RaiseUIChanged();
		}
		public Appointment CreateAppointment(AppointmentType type) {
			Appointment apt = AppointmentFactory.CreateAppointment(type);
			((IInternalAppointment)apt).TimeZoneEngine = ((IInternalSchedulerStorageBase)Storage).TimeZoneEngine;
			CreateCustomFields(apt);
			return apt;
		}
		public Appointment CreateAppointment(AppointmentType type, DateTime start, DateTime end) {
			Appointment result = CreateAppointment(type);
			result.Start = start;
			result.End = end;
			return result;
		}
		public Appointment CreateAppointment(AppointmentType type, DateTime start, TimeSpan duration) {
			Appointment result = CreateAppointment(type);
			result.Start = start;
			result.Duration = duration;
			return result;
		}
		public Appointment CreateAppointment(AppointmentType type, DateTime start, DateTime end, string subject) {
			Appointment result = CreateAppointment(type);
			result.Start = start;
			result.End = end;
			result.Subject = subject;
			return result;
		}
		public Appointment CreateAppointment(AppointmentType type, DateTime start, TimeSpan duration, string subject) {
			Appointment result = CreateAppointment(type);
			result.Start = start;
			result.Duration = duration;
			result.Subject = subject;
			return result;
		}
		public virtual void SaveToXml(string fileName) {
			Items.WriteXml(fileName);
		}
		public virtual void SaveToXml(Stream stream) {
			Items.WriteXml(stream);
		}
		public virtual void LoadFromXml(string fileName) {
			Items.ReadXml(fileName);
		}
		public virtual void LoadFromXml(Stream stream) {
			Items.ReadXml(stream);
			for (int i = 0; i < Items.Count; i++) {
				CommitExistingObject(Items[0]);
			}
		}
		public void SetAppointmentFactory(IAppointmentFactory factory) {
			if (factory == null)
				factory = new DefaultAppointmentFactory(Storage);
			if (Object.Equals(appointmentFactory, factory))
				return;
			this.appointmentFactory = factory;
			RaiseReload(true);
		}
		protected override NotificationCollection<Appointment> CreateCollection() {
			return Storage != null ? new AppointmentCollection(Storage) : new AppointmentCollection();
		}
		protected internal override DataManager<Appointment> CreateDataManager() {
			return new AppointmentDataManager();
		}
		protected internal override MappingInfoBase<Appointment> CreateMappingInfo() {
			return new AppointmentMappingInfo();
		}
		protected internal override CustomFieldMappingCollectionBase<Appointment> CreateCustomMappingsCollection() {
			return new AppointmentCustomFieldMappingCollection();
		}
		protected override ISchedulerUnboundDataKeeper CreateUnboundDataKeeper() {
			return new UnboundDataKeeper();
		}
		protected internal override ObjectsNonPersistentInformation CreateNonPersistentInformation() {
			return null;
		}
		protected internal virtual NotificationCollectionChangedListener<IAppointmentStatus> CreateStatusesListener() {
			return new NotificationCollectionChangedListener<IAppointmentStatus>((UserInterfaceObjectCollection<IAppointmentStatus>)statuses);
		}
		protected internal virtual NotificationCollectionChangedListener<IAppointmentLabel> CreateLabelsListener() {
			return new NotificationCollectionChangedListener<IAppointmentLabel>((UserInterfaceObjectCollection<IAppointmentLabel>)labels);
		}
		protected internal override void ApplyNonPersistentInformation(ObjectsNonPersistentInformation nonPersistentInfo) {
		}
		public virtual AppointmentBaseCollection GetAppointmentsExpandingPatterns(TimeInterval interval) {
			return GetAppointmentsExpandingPatterns(interval, false);
		}
		public virtual AppointmentBaseCollection GetAppointmentsExpandingPatterns(TimeInterval interval, bool recurringOnly) {
			AppointmentBaseCollection result = new AppointmentBaseCollection(DXCollectionUniquenessProviderType.None);
			AppointmentCollection appointments = Items;
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = appointments[i];
				if (apt.Type == AppointmentType.Pattern) {
					AppointmentPatternExpander expander = new AppointmentPatternExpander(apt);
					result.AddRange(expander.Expand(interval));
				} else {
					if (!recurringOnly && AppointmentInstance.IsAppointmentIntersectsInterval(apt, interval))
						result.Add(apt);
				}
			}
			return result;
		}
		protected internal override bool OnItemInserting(Appointment apt) {
			if (!RaiseObjectInserting(apt))
				return false;
			if (apt.HasExceptions)
				CanInsertExceptions(apt);
			return true;
		}
		protected internal virtual bool CanInsertExceptions(Appointment pattern) {
			IInternalAppointment internalAppointment = (IInternalAppointment)pattern;
			int count = internalAppointment.PatternExceptions.Count;
			for (int i = 0; i < count; i++)
				if (!RaiseObjectInserting(internalAppointment.PatternExceptions[i]))
					return false;
			return true;
		}
		protected internal override void OnItemInserted(Appointment apt) {
			base.OnItemInserted(apt);
			if (apt.HasExceptions) {
				IInternalAppointment internalAppointment = (IInternalAppointment)apt;
				int count = internalAppointment.PatternExceptions.Count;
				for (int i = 0; i < count; i++) {
					Appointment occurrence = internalAppointment.PatternExceptions[i];
					OnObjectChildCreated(apt, occurrence);
				}
			}
			if (SupportsAppointmentTransaction && !IsObjectCollectionLoading)
				CompleteAppointmentsTransaction(apt, AppointmentsTransactionType.Insert);
		}
		#region AppointmentsTransaction base support
		protected virtual internal void InternalStartAppointmentsTransaction() {
			if (!SupportsAppointmentTransaction)
				return;
			this.inAppointmentsTransaction = true;
		}
		protected virtual internal void InternalCommitAppointmentsTransaction(IList<Appointment> appointments, AppointmentsTransactionType type) {
			if (!SupportsAppointmentTransaction)
				return;
			this.inAppointmentsTransaction = false;
			AppointmentsTransactionEventArgs args = CreateAppointmentsTransactionEventArgs(appointments, type);
			RaiseAppointmentsTransactionCompleted(args);
		}
		protected virtual internal void InternalCancelAppointmentsTransaction() {
			if (!SupportsAppointmentTransaction)
				return;
			this.inAppointmentsTransaction = false;
		}
		protected internal AppointmentsTransactionEventArgs CreateAppointmentsTransactionEventArgs(IList<Appointment> appointments, AppointmentsTransactionType type) {
			return new AppointmentsTransactionEventArgs(appointments, type);
		}
		protected internal override void OnObjectStateChanged(object sender, PersistentObjectStateChangedEventArgs e) {
			base.OnObjectStateChanged(sender, e);
			if (!SupportsAppointmentTransaction)
				return;
			CompleteAppointmentsTransaction((Appointment)e.Object, GetAppointmentsTransactionType(e.State));
		}
		protected virtual AppointmentsTransactionType GetAppointmentsTransactionType(PersistentObjectState state) {
			switch (state) {
				case PersistentObjectState.Changed:
					return AppointmentsTransactionType.Update;
				case PersistentObjectState.ChildCreated:
					return AppointmentsTransactionType.ChildCreate;
				case PersistentObjectState.ChildDeleted:
					return AppointmentsTransactionType.ChildDelete;
				case PersistentObjectState.Deleted:
					return AppointmentsTransactionType.Delete;
				case PersistentObjectState.RollbackState:
					return AppointmentsTransactionType.Rollback;
				default:
					return AppointmentsTransactionType.Unknown;
			}
		}
		protected virtual void CompleteAppointmentsTransaction(Appointment apt, AppointmentsTransactionType type) {
			if (InAppointmentsTransaction)
				return;
			AppointmentsTransactionEventArgs args = CreateAppointmentsTransactionEventArgs(new Appointment[] { apt }, type);
			RaiseAppointmentsTransactionCompleted(args);
		}
		#endregion
		#region ISupportsAppointmentTransaction
		void ISupportsAppointmentTransaction.InternalStartAppointmentsTransaction() {
			InternalStartAppointmentsTransaction();
		}
		void ISupportsAppointmentTransaction.InternalCommitAppointmentsTransaction(IList<Appointment> appointments, AppointmentsTransactionType type) {
			InternalCommitAppointmentsTransaction(appointments, type);
		}
		void ISupportsAppointmentTransaction.InternalCancelAppointmentsTransaction() {
			InternalCancelAppointmentsTransaction();
		}
		#endregion
		protected internal override void UpdateKeyFieldName() {
		}
		protected override void ValidateDataSourceCore(StringCollection errors, DataFieldInfoCollection fields, MappingCollection mappings) {
			base.ValidateDataSourceCore(errors, fields, mappings);
			if (mappings[AppointmentSR.RecurrenceInfo] != null) {
				if (mappings[AppointmentSR.Type] == null)
					errors.Add(GetInvalidRecurrenceMappingErrorText());
			}
		}
		protected internal virtual string GetInvalidRecurrenceMappingErrorText() {
			return SchedulerLocalizer.GetString(SchedulerStringId.Msg_InconsistentRecurrenceInfoMapping);
		}
		#region Appointment collection methods
		public bool IsNewAppointment(Appointment apt) {
			return Items.IsNewAppointment(apt);
		}
		public void AddRange(Appointment[] items) {
			Items.AddRange(items);
		}
		public int Add(Appointment apt) {
			return Items.Add(apt);
		}
		public void Remove(Appointment apt) {
			if (Items.Contains(apt))
				Items.Remove(apt);
			else
				apt.Delete();
		}
		public bool Contains(Appointment apt) {
			return Items.Contains(apt);
		}
		#endregion
		protected internal override void LoadObjectsFromDataManager() {
			AppointmentCollectionLoader loader = CreateAppointmentCollectionLoader();
			loader.OnException += OnLoaderException;
			try {
				loader.LoadAppointments();
			} finally {
				loader.OnException -= OnLoaderException;
			}
		}
		void OnLoaderException(object sended, SchedulerUnhandledExceptionEventArgs e) {
			e.Handled = RaiseLoadException(e.Exception);
		}
		protected internal virtual AppointmentCollectionLoader CreateAppointmentCollectionLoader() {
			if (SupportsRecurrence)
				return new AppointmentCollectionLoaderRecurrence(this, this);
			else
				return new AppointmentCollectionLoaderNoRecurrence(this, this);
		}
		#region IAppointmentLoaderProvider implementation
		int IAppointmentLoaderProvider.TotalObjectCount { get { return DataManager.SourceObjectCount; } }
		Appointment IAppointmentLoaderProvider.LoadAppointment(IAppointmentStorageBase storage, int objectIndex) {
			return LoadObjectFromDataManager(objectIndex);
		}
		object IAppointmentLoaderProvider.GetPatternId(IAppointmentStorageBase storage, Appointment apt) {
			if (!apt.IsException)
				return Guid.Empty;
			MappingBase mapping = ((IInternalAppointmentStorage)storage).ActualMappings[AppointmentSR.RecurrenceInfo];
			object val = DataManager.GetRowValue(apt.RowHandle, mapping.Member);
			string patternReferenceString = StringPropertyMapping.ValueToString(val);
			PatternReference reference = PatternReferenceXmlPersistenceHelper.ObjectFromXml(patternReferenceString);
			return (reference != null) ? reference.Id : Guid.Empty;
		}
		#endregion
		protected internal override Appointment CreateObject(object rowHandle) {
			AppointmentType type = LoadAppointmentType(rowHandle);
			Appointment apt = CreateAppointment(type);
			return apt;
		}
		internal AppointmentType LoadAppointmentType(object rowHandle) {
			try {
				MappingBase mapping = ActualMappings[AppointmentSR.Type];
				if (mapping == null)
					return AppointmentType.Normal;
				else {
					object val = DataManager.GetRowValue(rowHandle, mapping.Member);
#if SL
					if (val == null) 
						return AppointmentType.Normal;
#endif
					return val is DBNull ? AppointmentType.Normal : (AppointmentType)val;
				}
			} catch {
				return AppointmentType.Normal;
			}
		}
		protected virtual void SetInnerLabelsCore(IAppointmentLabelStorage value) {
			UnsubscribeLabelsEvents();
			this.labels = value;
			this.labelsListener = CreateLabelsListener();
			SubscribeLabelsEvents();
		}
		protected virtual void SetInnerStatusesCore(IAppointmentStatusStorage value) {
			UnsubscribeStatusesEvents();
			this.statuses = value;
			this.statusesListener = CreateStatusesListener();
			SubscribeStatusesEvents();
		}
		void IInternalAppointmentStorage.AttachStorage(ISchedulerStorageBase storage) {
			SetStorageCore(storage);
			Items.SetStorageCore(storage);
		}
		public Appointment GetAppointmentById(object id) {
			return Items.GetAppointmentById(id);
		}
		protected internal override void Assign(PersistentObjectStorage<Appointment> source) {
			base.Assign(source);
			AppointmentStorageBase storage = source as AppointmentStorageBase;
			if (storage != null) {
				BeginUpdateInternal();
				try {
					ResourceSharing = storage.ResourceSharing;
				} finally {
					BeginUpdateInternal();
				}
			}
		}
		protected override void HandleInternalObjectChanging(PersistentObjectStateChangingEventArgs e) {
			Appointment apt = (Appointment)e.Object;
			if (((IInternalAppointment)apt).ShouldUpdateChangeStateData(e.PropertyName, e.OldValue, e.NewValue))
				RaiseInternalObjectChanging(e);
		}
		#region IAppointmentMappingsProvider implementation
		AppointmentMappingInfo IAppointmentMappingsProvider.Mappings {
			get { return Mappings; }
		}
		#endregion
		#region IAppointmentStorage
		IAppointmentLabelStorage IAppointmentStorageBase.Labels { get { return InnerLabels; } }
		IAppointmentStatusStorage IAppointmentStorageBase.Statuses { get { return InnerStatuses; } }
		#endregion
	}
	#endregion
	public interface IAppointmentLabel : IUserInterfaceObject, IDisposable {
		int ColorValue { get; set; }
	}
	#region AppointmentLabelBase
	#endregion
	#region AppointmentStatusType
	public enum AppointmentStatusType {
		Free = 0,
		Tentative = 1,
		Busy = 2,
		OutOfOffice = 3,
		WorkingElsewhere = 4,
		Custom = 5
	};
	#endregion
	internal interface IAppointmentStatusFactory {
		IAppointmentStatus CreateInstance(AppointmentStatusType type, string text, string menuCaption);
		IAppointmentStatus CreateInstanceByType(AppointmentStatusType type);
	}
	public interface IAppointmentStatus : IUserInterfaceObject, IDisposable {
		[Browsable(false), XtraSerializableProperty()]
		AppointmentStatusType Type { get; set; }
		[Obsolete("Use the GetBrush and SetBrush extension methods instead."), Browsable(false), DXBrowsable(false)]
		Color Color { get; set; }
	}
	#region AppointmentStatusBase (abstract class)
	#endregion
}
namespace DevExpress.XtraScheduler.Data {
	#region IAppointmentLoaderProvider
	public interface IAppointmentLoaderProvider {
		int TotalObjectCount { get; }
		Appointment LoadAppointment(IAppointmentStorageBase storage, int objectIndex);
		object GetPatternId(IAppointmentStorageBase storage, Appointment apt);
	}
	#endregion
	#region AppointmentCollectionLoader
	public abstract class AppointmentCollectionLoader {
		IAppointmentLoaderProvider provider;
		IAppointmentStorageBase storage;
		protected AppointmentCollectionLoader(IAppointmentLoaderProvider provider, IAppointmentStorageBase storage) {
			if (provider == null)
				Exceptions.ThrowArgumentException("provider", provider);
			if (storage == null)
				Exceptions.ThrowArgumentException("storage", storage);
			this.provider = provider;
			this.storage = storage;
		}
		protected IAppointmentLoaderProvider Provider { get { return provider; } }
		protected IAppointmentStorageBase Storage { get { return storage; } }
		public event SchedulerUnhandledExceptionEventHandler OnException;
		protected bool RaiseOnException(Exception exception) {
			if (OnException == null)
				return false;
			SchedulerUnhandledExceptionEventArgs ea = new SchedulerUnhandledExceptionEventArgs(exception);
			OnException(this, ea);
			return ea.Handled;
		}
		public abstract void LoadAppointments();
	}
	#endregion
	#region AppointmentCollectionLoaderNoRecurrence
	public class AppointmentCollectionLoaderNoRecurrence : AppointmentCollectionLoader {
		public AppointmentCollectionLoaderNoRecurrence(IAppointmentLoaderProvider provider, AppointmentStorageBase storage)
			: base(provider, storage) {
		}
		public override void LoadAppointments() {
			AppointmentCollection target = Storage.Items;
			target.BeginUpdate();
			try {
				LoadAppointmentsCore(target);
			} finally {
				target.CancelUpdate();
			}
		}
		void LoadAppointmentsCore(AppointmentCollection target) {
			int count = Provider.TotalObjectCount;
			for (int i = 0; i < count; i++) {
				Appointment apt = Provider.LoadAppointment(Storage, i);
				AddLoadedAppointmentToCollection(apt, target);
			}
		}
		internal void AddLoadedAppointmentToCollection(Appointment apt, AppointmentCollection target) {
			if (IsLoadedAppointmentValid(apt))
				target.Add(apt);
			else {
				if (apt != null)
					apt.Dispose();
			}
		}
		internal bool IsLoadedAppointmentValid(Appointment apt) {
			return apt != null && apt.Type == AppointmentType.Normal;
		}
	}
	#endregion
	#region AppointmentCollectionLoaderRecurrence
	public class AppointmentCollectionLoaderRecurrence : AppointmentCollectionLoader {
		public AppointmentCollectionLoaderRecurrence(IAppointmentLoaderProvider provider, IAppointmentStorageBase storage)
			: base(provider, storage) {
		}
		public override void LoadAppointments() {
			AppointmentCollection appointments = Storage.Items;
			appointments.BeginUpdate();
			try {
				AppointmentBaseCollection patterns = new AppointmentBaseCollection();
				patterns.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
				AppointmentBaseCollection exceptions = new AppointmentBaseCollection();
				exceptions.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
				LoadAndSortOutAppointments(appointments, patterns, exceptions);
				List<Appointment> wrongPatterns = new List<Appointment>();
				Dictionary<object, Appointment> patternRecurrenceIdTable = CreatePatternRecurrenceIdTable(patterns, out wrongPatterns);
				LinkExceptionsToPatterns(exceptions, patternRecurrenceIdTable);
				ValidatePatterns(patterns, wrongPatterns);
				appointments.AddRange(patterns);
			} finally {
				appointments.CancelUpdate();
			}
		}
		void ValidatePatterns(AppointmentBaseCollection patterns, List<Appointment> wrongPatternConllection) {
			int count = wrongPatternConllection.Count;
			for (int i = 0; i < count; i++)
				patterns.Remove(wrongPatternConllection[i]);
		}
		internal void LoadAndSortOutAppointments(AppointmentBaseCollection normalAppointments, AppointmentBaseCollection patternAppointments, AppointmentBaseCollection exceptionAppointments) {
			int count = Provider.TotalObjectCount;
			for (int i = 0; i < count; i++) {
				Appointment apt = Provider.LoadAppointment(Storage, i);
				if (apt != null)
					SortOutAppointment(apt, normalAppointments, patternAppointments, exceptionAppointments);
			}
		}
		internal void SortOutAppointment(Appointment apt, AppointmentBaseCollection normalAppointments, AppointmentBaseCollection patternAppointments, AppointmentBaseCollection exceptionAppointments) {
			switch (apt.Type) {
				case AppointmentType.Normal:
					normalAppointments.Add(apt);
					break;
				case AppointmentType.Pattern:
					patternAppointments.Add(apt);
					break;
				case AppointmentType.ChangedOccurrence: 
				case AppointmentType.DeletedOccurrence:
					exceptionAppointments.Add(apt);
					break;
				default:
					apt.Dispose();
					break;
			}
		}
		internal Dictionary<object, Appointment> CreatePatternRecurrenceIdTable(AppointmentBaseCollection patternAppointments, out List<Appointment> wrongPatternConllection) {
			wrongPatternConllection = new List<Appointment>();
			Dictionary<object, Appointment> result = new Dictionary<object, Appointment>();
			int count = patternAppointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment pattern = patternAppointments[i];
				try {
					result.Add(pattern.RecurrenceInfo.Id, pattern);
				} catch (Exception e) {
					if (!RaiseOnException(e))
						throw e;
					else
						wrongPatternConllection.Add(pattern);
				}
			}
			return result;
		}
		internal void LinkExceptionsToPatterns(AppointmentBaseCollection exceptionAppointments, Dictionary<object, Appointment> patternTable) {
			int count = exceptionAppointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment exception = exceptionAppointments[i];
				object id = Provider.GetPatternId(Storage, exception);
				Appointment pattern;
				if (patternTable.TryGetValue(id, out pattern))
					((IInternalAppointment)pattern).RecurrenceChain.RegisterException(exception);
				else
					exception.Dispose();
			}
		}
	}
	#endregion
	#region AppointmentDataManager
	public class AppointmentDataManager : DataManager<Appointment> {
		protected internal override void AdjustRowHandle(Appointment apt, object deletedRowHandle) {
			base.AdjustRowHandle(apt, deletedRowHandle);
			AdjustPatternExceptionsRowHandles(apt, deletedRowHandle);
		}
		protected internal virtual void AdjustPatternExceptionsRowHandles(Appointment apt, object deletedRowHandle) {
			if (apt.HasExceptions) {
				AppointmentBaseCollection exceptions = ((IInternalAppointment)apt).PatternExceptions;
				int count = exceptions.Count;
				for (int i = 0; i < count; i++)
					base.AdjustRowHandle(exceptions[i], deletedRowHandle);
			}
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Xml {
	#region AppointmentContextElement
	public class AppointmentContextElement : XmlContextItem {
		IAppointmentStorageBase storage;
		public AppointmentContextElement(Appointment appointment, IAppointmentStorageBase storage)
			: base(AppointmentSR.XmlElementName, appointment, null) {
			this.storage = storage;
		}
		protected Appointment Appointment { get { return (Appointment)Value; } }
		protected IAppointmentStorageBase Storage { get { return storage; } }
		public override string ValueToString() {
			return new AppointmentXmlPersistenceHelper(Appointment, Storage).ToXml();
		}
	}
	#endregion
	#region AppointmentXmlPersistenceHelper
	public class AppointmentXmlPersistenceHelper : SchedulerXmlPersistenceHelper {
		IAppointmentStorageBase storage;
		Appointment appointment;
		public AppointmentXmlPersistenceHelper(Appointment appointment, IAppointmentStorageBase storage) {
			this.appointment = appointment;
			this.storage = storage;
		}
		protected Appointment Appointment { get { return appointment; } }
		public IAppointmentStorageBase Storage { get { return storage; } }
		protected override IXmlContext GetXmlContext() {
			XmlContext context = new XmlContext(AppointmentSR.XmlElementName);
			context.Attributes.Add(new SchedulerObjectContextAttribute(AppointmentSR.Id, Appointment.Id, null));
			context.Attributes.Add(new IntegerContextAttribute(AppointmentSR.Type, (int)Appointment.Type, (int)AppointmentType.Normal));
			IInternalAppointment internalAppointment = (IInternalAppointment)Appointment;
			if (internalAppointment.TimeZoneEngine == null) {
				context.Attributes.Add(new DateTimeContextAttribute(AppointmentSR.Start, Appointment.Start, DateTime.MinValue));
				context.Attributes.Add(new DateTimeContextAttribute(AppointmentSR.End, Appointment.End, DateTime.MinValue + TimeInterval.DefaultDuration));
			} else {
				context.Attributes.Add(new DateTimeContextAttribute(AppointmentSR.Start, internalAppointment.TimeZoneEngine.FromOperationTime(Appointment.Start, Appointment.TimeZoneId), DateTime.MinValue));
				context.Attributes.Add(new DateTimeContextAttribute(AppointmentSR.End, internalAppointment.TimeZoneEngine.FromOperationTime(Appointment.End, Appointment.TimeZoneId), DateTime.MinValue + TimeInterval.DefaultDuration));
			}
			context.Attributes.Add(new StringContextAttribute(AppointmentSR.TimeZoneId, Appointment.TimeZoneId, String.Empty));
			context.Attributes.Add(new BooleanContextAttribute(AppointmentSR.AllDay, Appointment.AllDay, false));
			context.Attributes.Add(new StringContextAttribute(AppointmentSR.Description, Appointment.Description, String.Empty));
			context.Attributes.Add(new SchedulerObjectContextAttribute(AppointmentSR.Label, Appointment.LabelKey, null));
			context.Attributes.Add(new StringContextAttribute(AppointmentSR.Location, Appointment.Location, String.Empty));
#if !SL && !WPF
			context.Attributes.Add(new IntegerContextAttribute(AppointmentSR.PercentComplete, Appointment.PercentComplete, 0));
#endif
			if (Appointment.IsRecurring) {
				if ((Appointment.Type == AppointmentType.Pattern))
					context.Elements.Add(CreateRecurrenceInfoContextElement());
				else
					context.Elements.Add(CreatePatternReferenceContextElement());
			}
			if (Appointment.HasReminder) {
				context.Elements.Add(CreateReminderCollectionContextElement());
			}
			if (Appointment.IsBase) {
				if (Appointment.ResourceIds.Count > 1)
					context.Elements.Add(CreateAppointmentResourceIdCollectionContextElement());
				else
					context.Attributes.Add(new SchedulerObjectContextAttribute(AppointmentSR.ResourceId, Appointment.ResourceId, EmptyResourceId.Id));
			}
			context.Attributes.Add(new SchedulerObjectContextAttribute(AppointmentSR.Status, Appointment.StatusKey, null));
			context.Attributes.Add(new StringContextAttribute(AppointmentSR.Subject, Appointment.Subject, String.Empty));
			if (Storage != null)
				AddCustomFieldsToContext(context, Appointment.CustomFields, ((IInternalAppointmentStorage)Storage).ActualMappings);
			return context;
		}
		public override ObjectXmlLoader CreateObjectXmlLoader(XmlNode root) {
			return new AppointmentXmlLoader(root, Storage);
		}
		protected virtual IXmlContextItem CreateReminderCollectionContextElement() {
			return new ReminderCollectionContextElement(Appointment);
		}
		protected virtual IXmlContextItem CreateAppointmentResourceIdCollectionContextElement() {
			return new AppointmentResourceIdCollectionContextElement(Appointment.ResourceIds);
		}
		protected virtual IXmlContextItem CreateRecurrenceInfoContextElement() {
			return new RecurrenceInfoContextElement(Appointment);
		}
		protected virtual IXmlContextItem CreatePatternReferenceContextElement() {
			return new PatternReferenceContextElement(new PatternReference(Appointment.RecurrenceInfo.Id, Appointment.RecurrenceIndex));
		}
		public static Appointment ObjectFromXml(IAppointmentStorageBase storage, string xml) {
			return ObjectFromXml(storage, GetRootElement(xml));
		}
		public static Appointment ObjectFromXml(IAppointmentStorageBase storage, XmlNode root) {
			AppointmentXmlPersistenceHelper helper = new AppointmentXmlPersistenceHelper(null, storage);
			return (Appointment)helper.FromXmlNode(root);
		}
	}
	#endregion
	#region AppointmentXmlLoader
	public class AppointmentXmlLoader : PersistentObjectXmlLoader {
		IAppointmentStorageBase storage;
		public AppointmentXmlLoader(XmlNode root, IAppointmentStorageBase storage)
			: base(root) {
			this.storage = storage;
		}
		protected IAppointmentStorageBase Storage { get { return storage; } }
		public override object ObjectFromXml() {
			AppointmentType type = (AppointmentType)ReadAttributeAsInt(AppointmentSR.Type, (int)AppointmentType.Normal);
			Appointment apt = CreateAppointmentInstance(type);
			IInternalAppointment internalApt = (IInternalAppointment)apt;
			((IBatchUpdateable)apt).BeginUpdate();
			try {
				apt.TimeZoneId = ReadAttributeAsString(AppointmentSR.TimeZoneId, String.Empty);
				apt.Start = internalApt.TimeZoneEngine.FromOperationTime(ReadAttributeAsDateTime(AppointmentSR.Start, DateTime.MinValue), apt.TimeZoneId);
				apt.End = internalApt.TimeZoneEngine.FromOperationTime(ReadAttributeAsDateTime(AppointmentSR.End, DateTime.MinValue), apt.TimeZoneId);
				apt.SetId(ReadAttributeAsObject(AppointmentSR.Id, typeof(object), null));
				apt.AllDay = ReadAttributeAsBoolean(AppointmentSR.AllDay, false);
				apt.Description = ReadAttributeAsString(AppointmentSR.Description, String.Empty);
				apt.Subject = ReadAttributeAsString(AppointmentSR.Subject, String.Empty);
				apt.Location = ReadAttributeAsString(AppointmentSR.Location, String.Empty);
				apt.LabelKey = ReadAttributeAsObject(AppointmentSR.Label, typeof(object), null);
				apt.StatusKey = ReadAttributeAsObject(AppointmentSR.Status, typeof(object), null);
#if !SL && !WPF
				apt.PercentComplete = ReadAttributeAsInt(AppointmentSR.PercentComplete, 0);
#endif
				if (apt.IsBase) {
					apt.ResourceId = ReadAttributeAsObject(AppointmentSR.ResourceId, typeof(object), EmptyResourceId.Id);
				}
				if (apt.Type == AppointmentType.Pattern) {
					internalApt.SetRecurrenceInfo(RecurrenceInfoFromXml());
				} else if (apt.IsException) {
					internalApt.SetRecurrenceIndex(RecurrenceIndexFromXml());
				}
				ResourceIdsFromXml(apt.ResourceIds);
				ReminderFromXml(apt);
				if (Storage != null)
					CustomFieldsFromXml(apt, ((IInternalAppointmentStorage)Storage).ActualMappings);
			} finally {
				((IBatchUpdateable)apt).EndUpdate();
			}
			return apt;
		}
		protected virtual void ResourceIdsFromXml(ResourceIdCollection resourceIds) {
			DXXmlNodeCollection nodes = GetChildNodes(AppointmentSR.XmlCollectionResourceIdsName);
			if (nodes.Count > 0)
				AppointmentResourceIdCollectionXmlPersistenceHelper.ObjectFromXml(resourceIds, nodes[0]);
		}
		protected virtual void ReminderFromXml(Appointment apt) {
			DXXmlNodeCollection nodes = GetChildNodes(ReminderSR.XmlCollectionName);
			if (nodes.Count > 0)
				ReminderCollectionXmlPersistenceHelper.ObjectFromXml(apt, nodes[0]);
			else {
				nodes = GetChildNodes(ReminderSR.XmlElementName);
				if (nodes.Count > 0) {
					Reminder r = apt.CreateNewReminder();
					ReminderXmlPersistenceHelper.ObjectFromXml(r, nodes[0]);
					apt.Reminders.Add(r);
				}
			}
		}
		protected virtual IRecurrenceInfo RecurrenceInfoFromXml() {
			DXXmlNodeCollection nodes = GetChildNodes(AppointmentSR.RecurrenceInfo);
			if (nodes.Count > 0)
				return RecurrenceInfoXmlPersistenceHelper.ObjectFromXml(nodes[0]);
			else
				return null;
		}
		protected virtual int RecurrenceIndexFromXml() {
			DXXmlNodeCollection nodes = GetChildNodes(AppointmentSR.RecurrenceInfo);
			if (nodes.Count <= 0)
				return 0;
			PatternReference patternRef = PatternReferenceXmlPersistenceHelper.ObjectFromXml(nodes[0]);
			return patternRef.RecurrenceIndex;
		}
		protected internal virtual Appointment CreateAppointmentInstance(AppointmentType type) {
			return SchedulerUtils.CreateAppointmentInstance(Storage, type);
		}
	}
	#endregion
	#region AppointmentCollectionContextElement
	public class AppointmentCollectionContextElement : XmlContextItem {
		AppointmentStorageBase storage;
		public AppointmentCollectionContextElement(AppointmentStorageBase storage)
			: base(AppointmentSR.XmlCollectionName, storage.Items, null) {
			this.storage = storage;
		}
		public override string ValueToString() {
			return new AppointmentCollectionXmlPersistenceHelper(storage).ToXml();
		}
	}
	#endregion
	#region AppointmentCollectionXmlPersistenceHelper
	public class AppointmentCollectionXmlPersistenceHelper : CollectionXmlPersistenceHelper {
		IAppointmentStorageBase storage;
		public AppointmentCollectionXmlPersistenceHelper(IAppointmentStorageBase storage)
			: base(storage.Items) {
			this.storage = storage;
		}
		protected override string XmlCollectionName { get { return AppointmentSR.XmlCollectionName; } }
		protected IAppointmentStorageBase Storage { get { return storage; } }
		protected AppointmentBaseCollection Appointments { get { return Storage.Items; } }
		public static AppointmentBaseCollection ObjectFromXml(IAppointmentStorageBase storage, string xml) {
			return ObjectFromXml(storage, GetRootElement(xml));
		}
		public static AppointmentBaseCollection ObjectFromXml(IAppointmentStorageBase storage, XmlNode root) {
			AppointmentCollectionXmlPersistenceHelper helper = new AppointmentCollectionXmlPersistenceHelper(storage);
			return (AppointmentBaseCollection)helper.FromXmlNode(root);
		}
		protected override ObjectCollectionXmlLoader CreateObjectCollectionXmlLoader(XmlNode root) {
			return null;
		}
		protected override IXmlContext CreateXmlContext() {
			IXmlContext context = base.CreateXmlContext();
			((XmlContext)context).XmlDocumentHeader = true;
			return context;
		}
		protected override void AddItemToContext(object item, IXmlContext context) {
			base.AddItemToContext(item, context);
			Appointment apt = (Appointment)item;
			if (apt.Type == AppointmentType.Pattern && apt.HasExceptions) {
				ICollection exceptions = ((IInternalAppointment)apt).PatternExceptions;
				foreach (Appointment excpt in exceptions) {
					base.AddItemToContext(excpt, context);
				}
			}
		}
		protected override IXmlContextItem CreateXmlContextItem(object obj) {
			return new AppointmentContextElement((Appointment)obj, Storage);
		}
		public override object ParseXmlDocument(XmlNode root) {
			IAppointmentLoaderProvider provider = new AppointmentLoaderXmlProvider(XmlDocumentHelper.GetChildren(root));
			AppointmentCollectionLoader loader = new AppointmentCollectionLoaderRecurrence(provider, (AppointmentStorageBase)Storage);
			loader.OnException += OnLoaderException;
			try {
				loader.LoadAppointments();
			} finally {
				loader.OnException -= OnLoaderException;
			}
			return Appointments;
		}
		void OnLoaderException(object sended, SchedulerUnhandledExceptionEventArgs e) {
			if (Storage == null)
				return;
			e.Handled = ((IInternalAppointmentStorage)Storage).RaiseLoadException(e.Exception);
		}
	}
	#endregion
	#region AppointmentLoaderXmlProvider
	public class AppointmentLoaderXmlProvider : IAppointmentLoaderProvider {
		DXXmlNodeCollection items;
		public AppointmentLoaderXmlProvider(DXXmlNodeCollection items) {
			this.items = items;
		}
		protected DXXmlNodeCollection Items { get { return items; } }
		#region IAppointmentLoaderProvider implementation
		public int TotalObjectCount { get { return Items.Count; } }
		public Appointment LoadAppointment(IAppointmentStorageBase storage, int objectIndex) {
			XmlNode item = Items[objectIndex];
			if (item == null)
				return null;
			Appointment apt = AppointmentXmlPersistenceHelper.ObjectFromXml(storage, item);
			if (apt != null)
				apt.RowHandle = objectIndex;
			return apt;
		}
		public object GetPatternId(IAppointmentStorageBase storage, Appointment apt) {
			if (apt.IsException) {
				XmlNode aptNode = Items[(int)apt.RowHandle];
				if (aptNode != null)
					return LoadPatternId(GetChildNode(aptNode.ChildNodes, RecurrenceInfoSR.XmlElementName));
			}
			return Guid.Empty;
		}
		protected object LoadPatternId(XmlNode patternReferenceNode) {
			if (patternReferenceNode != null) {
				PatternReference patternRef = PatternReferenceXmlPersistenceHelper.ObjectFromXml(patternReferenceNode);
				if (patternRef != null)
					return patternRef.Id;
			}
			return Guid.Empty;
		}
		XmlNode GetChildNode(XmlNodeList nodes, string name) {
			for (int i = 0; i < nodes.Count; i++)
				if (String.Compare(nodes[i].Name, name, StringComparison.CurrentCultureIgnoreCase) == 0)
					return nodes[i];
			return null;
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	public interface ISupportsAppointmentTransaction {
		void InternalStartAppointmentsTransaction();
		void InternalCommitAppointmentsTransaction(IList<Appointment> appointments, AppointmentsTransactionType type);
		void InternalCancelAppointmentsTransaction();
	}
	public interface IInternalAppointmentStorage : IInternalPersistentObjectStorage<Appointment> {
		IAppointmentFactory AppointmentFactory { get; }
		bool RaiseLoadException(Exception exception);
		AppointmentBaseCollection GetAppointmentsExpandingPatterns(TimeInterval interval);
		AppointmentBaseCollection GetAppointmentsExpandingPatterns(TimeInterval interval, bool recurringOnly);
		void AttachStorage(ISchedulerStorageBase storage);
		event EventHandler UIChanged;
		event AppointmentsTransactionEventHandler AppointmentsTransactionCompleted;
	}	
	#region IAppointmentMappingsProvider
	public interface IAppointmentMappingsProvider {
		AppointmentMappingInfo Mappings { get; }
	}
	#endregion
	#region AppointmentSR
	public static class AppointmentSR {
		public const string XmlCollectionName = "Items";
		public const string XmlElementName = "Appointment";
		public const string AllDay = "AllDay";
		public const string Description = "Description";
		public const string End = "End";
		public const string Location = "Location";
		public const string Label = "Label";
		public const string PercentComplete = "PercentComplete";
		public const string ResourceId = "ResourceId";
		public const string XmlCollectionResourceIdsName = "ResourceIds";
		public const string ResourceIdType = "Type";
		public const string ResourceIdValue = "Value";
		public const string Start = "Start";
		public const string Status = "Status";
		public const string Subject = "Subject";
		public const string Type = "Type";
		public const string Id = "Id";
		public const string IdMappingName = "AppointmentId";
		public const string RecurrenceInfo = "RecurrenceInfo";
		public const string ReminderInfo = "ReminderInfo";
		public const string TimeZoneId = "TimeZoneId";
	}
	#endregion
	#region DefaultAppointmentFactory
	public class DefaultAppointmentFactory : IAppointmentFactory {
		public DefaultAppointmentFactory(ISchedulerStorageBase storage) {
			SchedulerStorage = storage;
		}
		public ISchedulerStorageBase SchedulerStorage { get; set; }
		public Appointment CreateAppointment(AppointmentType type) {
			AppointmentInstance newAppointment = new AppointmentInstance(type);
			newAppointment.TimeZoneEngine = ((IInternalSchedulerStorageBase)SchedulerStorage).TimeZoneEngine;
			IAppointmentLabel label0 = SchedulerStorage.Appointments.Labels.GetByIndex(0);
			IAppointmentStatus status0 = SchedulerStorage.Appointments.Statuses.GetByIndex(0);
			newAppointment.LabelKey = label0.Id;
			newAppointment.StatusKey = status0.Id;
			return newAppointment;
		}
	}
	#endregion
	#region InnerAppointment (abstract class)
	public abstract class InnerAppointment : IDisposable, INotifyPropertyChanged {
		#region Fields
		readonly Appointment owner;
		IAppointmentBase baseAppointment;
		IAppointmentResources appointmentResources;
		IAppointmentReminders appointmentReminders;
		IAppointmentProcess appointmentProcess;
		#endregion
		protected InnerAppointment(Appointment owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			this.baseAppointment = new DefaultIAppointment();
		}
		#region Properties
		public virtual Appointment Owner { get { return owner; } }
		public virtual Appointment RecurrencePattern { get { return null; } protected internal set { } }
		public virtual int RecurrenceIndex { get { return 0; } protected internal set { } }
		public virtual IRecurrenceChain RecurrenceChain { get { return null; } }
		public abstract AppointmentType Type { get; }
		protected internal virtual AppointmentResourceIdCollection InnerResourceIds { get { return AppointmentResources != null ? AppointmentResources.ResourceIds : null; } }
		public virtual AppointmentResourceIdCollection ResourceIds { get { return AppointmentResources.ResourceIds; } }
		protected internal IAppointmentResources AppointmentResources { get { return appointmentResources; } set { appointmentResources = value; } }
		public IAppointmentReminders AppointmentReminders {
			get {
				if (appointmentReminders == null)
					CreateRemindersFromCollection(new ReminderCollection(), false);
				return appointmentReminders;
			}
		}
		internal IAppointmentReminders InnerAppointmentReminders { get { return appointmentReminders; } }
		public bool HasReminder { get { return InnerAppointmentReminders != null ? InnerAppointmentReminders.Reminders.Count > 0 : false; } }
		protected internal IAppointmentBase BaseAppointment { get { return baseAppointment; } set { baseAppointment = value; } }
		protected internal virtual AppointmentBaseCollection PatternExceptions { get { return null; } }
		protected internal virtual IRecurrenceInfo InnerRecurrenceInfo { get { return null; } }
		protected internal virtual IRecurrenceInfo RecurrenceInfo { get { return null; } }
		protected internal virtual ReminderCollection Reminders { get { return AppointmentReminders.Reminders; } }
		protected internal virtual ReminderCollection InnerReminders { get { return InnerAppointmentReminders != null ? InnerAppointmentReminders.Reminders : null; } }
		protected internal virtual IAppointmentProcess InnerAppointmentProcess { get { return appointmentProcess; } }
		protected internal virtual IAppointmentProcess AppointmentProcess {
			get {
				if (appointmentProcess == null)
					appointmentProcess = CreateAppointmentProcess();
				return appointmentProcess;
			}
		}
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
				if (appointmentResources != null) {
					UnsubscribeResourceIdsEvents();
					appointmentResources.Dispose();
					appointmentResources = null;
				}
				if (appointmentProcess != null)
					appointmentProcess = null;
				DisposeReminders();
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		public abstract bool CanChangeType(AppointmentType newType);
		protected internal abstract bool CanDelete();
		protected internal abstract void Delete();
		public virtual void Initialize() {
			CreateResources();
			SubscribeResourceIdsEvents();
		}
		protected internal virtual void DetachInnerObjects() {
			UnsubscribeResourceIdsEvents();
			appointmentResources = null;
			baseAppointment = null;
		}
		public virtual void CreateResources() {
			DefaultAppointmentResources result = new DefaultAppointmentResources();
			result.Initialize();
			AppointmentResources = result;
		}
		protected internal virtual IAppointmentProcess CreateAppointmentProcess() {
			return new EmptyAppointmentProcess();
		}
		public virtual void SetRecurrenceInfo(IRecurrenceInfo recurrenceInfo) {
			Exceptions.ThrowArgumentException("recurrenceInfo", recurrenceInfo);
		}
		public virtual void SetPattern(Appointment pattern) {
			Exceptions.ThrowArgumentException("pattern", pattern);
		}
		public virtual void BeforeChangeAppointmentType() {
		}
		public virtual void AfterChangeAppointmentType(InnerAppointment previousInnerAppointment) {
			this.baseAppointment = previousInnerAppointment.BaseAppointment;
			UnsubscribeResourceIdsEvents();
			this.AppointmentResources.Dispose();
			this.AppointmentResources = previousInnerAppointment.AppointmentResources;
			SubscribeResourceIdsEvents();
			this.RecurrencePattern = previousInnerAppointment.RecurrencePattern; 
			this.RecurrenceIndex = previousInnerAppointment.RecurrenceIndex; 
		}
		protected internal virtual void SubscribeResourceIdsEvents() {
			AppointmentResources.PropertyChanging += OnResourceIdsChanging;
			AppointmentResources.PropertyChanged += OnResourceIdsChanged;
		}
		protected internal virtual void UnsubscribeResourceIdsEvents() {
			AppointmentResources.PropertyChanging -= OnResourceIdsChanging;
			AppointmentResources.PropertyChanged -= OnResourceIdsChanged;
		}
		protected internal abstract void OnResourceIdsChanging(object sender, CancellablePropertyChangingEventArgs e);
		protected internal virtual void OnResourceIdsChanged(object sender, PropertyChangedEventArgs e) {
			RaisePropertyChanged("Resources");
		}
		protected internal virtual void CreateSingleReminder() {
			DisposeReminders();
			this.appointmentReminders = CreateReminders(true);
			SubscribeRemindersEvents();
		}
		protected internal virtual void CreateRemindersFromCollection(ReminderCollection source, bool createSourceCopy) {
			DisposeReminders();
			this.appointmentReminders = CreateReminders(false);
			if (createSourceCopy)
				source = appointmentReminders.CreateRemindersCopy(source);
			this.appointmentReminders.ReplaceReminders(source);
			SubscribeRemindersEvents();
		}
		protected internal virtual void DisposeReminders() {
			if (appointmentReminders != null) {
				UnsubscribeRemindersEvents();
				appointmentReminders.Dispose();
				appointmentReminders = null;
			}
		}
		public virtual IAppointmentReminders CreateReminders(bool appendSingleReminderByDefault) {
			return new DefaultAppointmentReminders(Owner, appendSingleReminderByDefault);
		}
		protected internal virtual void SubscribeRemindersEvents() {
			if (this.appointmentReminders == null)
				return;
			AppointmentReminders.PropertyChanging += OnRemindersChanging;
			AppointmentReminders.PropertyChanged += OnRemindersChanged;
		}
		protected internal virtual void UnsubscribeRemindersEvents() {
			if (this.appointmentReminders == null)
				return;
			AppointmentReminders.PropertyChanging -= OnRemindersChanging;
			AppointmentReminders.PropertyChanged -= OnRemindersChanged;
		}
		protected internal virtual void OnRemindersChanging(object sender, CancellablePropertyChangingEventArgs e) {
			e.Cancel = !RaisePropertyChanging(e.PropertyName, e.OldValue, e.NewValue);
		}
		protected internal virtual void OnRemindersChanged(object sender, PropertyChangedEventArgs e) {
			RaisePropertyChanged(e.PropertyName);
		}
	}
	#endregion
	#region InnerNormalAppointment
	public class InnerNormalAppointment : InnerAppointment {
		public InnerNormalAppointment(Appointment owner)
			: base(owner) {
			XtraSchedulerDebug.Assert(owner.Type == AppointmentType.Normal);
		}
		public override AppointmentType Type { get { return AppointmentType.Normal; } }
		public override bool CanChangeType(AppointmentType newType) {
			return (newType == AppointmentType.Pattern);
		}
		protected internal override IAppointmentProcess CreateAppointmentProcess() {
			return new DefaultAppointmentProcess();
		}
		protected internal override void OnResourceIdsChanging(object sender, CancellablePropertyChangingEventArgs e) {
			e.Cancel = !RaisePropertyChanging("Resources", e.OldValue, e.NewValue);
		}
		public override void AfterChangeAppointmentType(InnerAppointment previousInnerAppointment) {
			base.AfterChangeAppointmentType(previousInnerAppointment);
			CreateRemindersFromCollection(previousInnerAppointment.AppointmentReminders.Reminders, true);
		}
		protected internal override bool CanDelete() {
			return ((IInternalPersistentObject)Owner).RaiseDeleting();
		}
		protected internal override void Delete() {
			((IInternalPersistentObject)Owner).RaiseStateChanged(PersistentObjectState.Deleted);
		}
	}
	#endregion
	#region InnerPatternAppointment
	public class InnerPatternAppointment : InnerAppointment {
		IRecurrenceChain recurrenceChain;
		public InnerPatternAppointment(Appointment owner)
			: base(owner) {
			XtraSchedulerDebug.Assert(owner.Type == AppointmentType.Pattern);
		}
		#region Properties
		public override AppointmentType Type { get { return AppointmentType.Pattern; } }
		public override IRecurrenceChain RecurrenceChain { get { return recurrenceChain; } }
		protected internal override AppointmentBaseCollection PatternExceptions { get { return RecurrenceChain.RecurrenceExceptions; } }
		protected internal override IRecurrenceInfo InnerRecurrenceInfo { get { return RecurrenceChain.RecurrenceInfo; } }
		protected internal override IRecurrenceInfo RecurrenceInfo { get { return InnerRecurrenceInfo; } }
		#endregion
		public override bool CanChangeType(AppointmentType newType) {
			return (newType == AppointmentType.Normal);
		}
		public override void Initialize() {
			base.Initialize();
			this.recurrenceChain = CreateRecurrenceChain();
			SubscribeRecurrenceChainEvents();
		}
		protected override void Dispose(bool disposing) {
			try {
				if (recurrenceChain != null) {
					UnsubscribeRecurrenceChainEvents();
					recurrenceChain.Dispose();
					recurrenceChain = null;
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		protected internal virtual IRecurrenceChain CreateRecurrenceChain() {
			return new DefaultRecurrenceChain(Owner);
		}
		protected internal virtual void SubscribeRecurrenceChainEvents() {
			RecurrenceChain.PropertyChanging += OnRecurrenceChainChanging;
			RecurrenceChain.PropertyChanged += OnRecurrenceChainChanged;
		}
		protected internal virtual void UnsubscribeRecurrenceChainEvents() {
			RecurrenceChain.PropertyChanging -= OnRecurrenceChainChanging;
			RecurrenceChain.PropertyChanged -= OnRecurrenceChainChanged;
		}
		protected internal virtual void OnRecurrenceChainChanging(object sender, CancellablePropertyChangingEventArgs e) {
			e.Cancel = !RaisePropertyChanging("RecurrenceInfo", e.OldValue, e.NewValue);
		}
		protected internal virtual void OnRecurrenceChainChanged(object sender, EventArgs e) {
			RaisePropertyChanged("RecurrenceInfo");
			((IInternalAppointment)Owner).UpdateReminders(); 
		}
		protected internal override void OnResourceIdsChanging(object sender, CancellablePropertyChangingEventArgs e) {
			e.Cancel = !RaisePropertyChanging("Resources", e.OldValue, e.NewValue);
		}
		public override void BeforeChangeAppointmentType() {
			RecurrenceChain.DeleteExceptions();
		}
		public override void SetRecurrenceInfo(IRecurrenceInfo recurrenceInfo) {
			Guard.ArgumentNotNull(recurrenceInfo, "recurrenceInfo");
			UnsubscribeRecurrenceChainEvents();
			try {
				RecurrenceChain.RecurrenceInfo = recurrenceInfo;
			} finally {
				SubscribeRecurrenceChainEvents();
			}
		}
		public override IAppointmentReminders CreateReminders(bool appendSingleReminderByDefault) {
			return new DefaultPatternAppointmentReminders(Owner, appendSingleReminderByDefault);
		}
		public override void AfterChangeAppointmentType(InnerAppointment previousInnerAppointment) {
			base.AfterChangeAppointmentType(previousInnerAppointment);
			CreateRemindersFromCollection(previousInnerAppointment.AppointmentReminders.Reminders, true);
		}
		protected internal override bool CanDelete() {
			if (!((IInternalPersistentObject)Owner).RaiseDeleting())
				return false;
			return RecurrenceChain.CanDeleteExceptions();
		}
		protected internal override void Delete() {
			RecurrenceChain.DeleteExceptions();
			((IInternalPersistentObject)Owner).RaiseStateChanged(PersistentObjectState.Deleted);
		}
	}
	#endregion
	#region InnerOccurrenceBaseAppointment (abstract class)
	public abstract class InnerOccurrenceBaseAppointment : InnerAppointment {
		Appointment pattern;
		int recurrenceIndex;
		protected InnerOccurrenceBaseAppointment(Appointment owner)
			: base(owner) {
		}
		public override Appointment RecurrencePattern {
			get { return pattern; }
			protected internal set {
				pattern = value;
			}
		}
		public override int RecurrenceIndex {
			get { return recurrenceIndex; }
			protected internal set {
				Guard.ArgumentNonNegative(value, "RecurrenceIndex");
				recurrenceIndex = value;
			}
		}
		protected internal override IRecurrenceInfo RecurrenceInfo {
			get {
				if (RecurrencePattern != null)
					return RecurrencePattern.RecurrenceInfo;
				else
					return null;
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					pattern = null;
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		public override void CreateResources() {
			DefaultAppointmentResources result;
			if (RecurrencePattern != null)
				result = new DefaultOccurrenceResources(RecurrencePattern.ResourceIds);
			else
				result = new DefaultAppointmentResources();
			result.Initialize();
			AppointmentResources = result;
		}
		public override void SetPattern(Appointment pattern) {
			Guard.ArgumentNotNull(pattern, "pattern");
			if (pattern.Type != AppointmentType.Pattern)
				Exceptions.ThrowArgumentException("pattern", pattern);
			UnsubscribeResourceIdsEvents();
			AppointmentResources.Dispose();
			RecurrencePattern = pattern;
			CreateResources();
			SubscribeResourceIdsEvents();
		}
		protected internal override void OnResourceIdsChanging(object sender, CancellablePropertyChangingEventArgs e) {
			e.Cancel = true;
		}
		protected internal override bool CanDelete() {
			return ((IInternalAppointment)RecurrencePattern).RecurrenceChain.CanDeleteOccurrence(Owner);
		}
		protected internal override void Delete() {
			((IInternalAppointment)RecurrencePattern).RecurrenceChain.DeleteOccurrence(Owner);
		}
	}
	#endregion
	#region InnerOccurrenceAppointment
	public class InnerOccurrenceAppointment : InnerOccurrenceBaseAppointment {
		public InnerOccurrenceAppointment(Appointment owner)
			: base(owner) {
			XtraSchedulerDebug.Assert(owner.Type == AppointmentType.Occurrence);
		}
		public override AppointmentType Type { get { return AppointmentType.Occurrence; } }
		public override bool CanChangeType(AppointmentType newType) {
			return newType != AppointmentType.Normal && newType != AppointmentType.Pattern;
		}
		public override void SetPattern(Appointment pattern) {
			base.SetPattern(pattern);
			CreateRemindersFromCollection(new ReminderCollection(), false);
		}
		public override IAppointmentReminders CreateReminders(bool appendSingleReminderByDefault) {
			if (RecurrencePattern != null)
				return new DefaultOccurrenceAppointmentReminders(Owner);
			else
				return new DefaultAppointmentReminders(Owner, appendSingleReminderByDefault);
		}
		public override void AfterChangeAppointmentType(InnerAppointment previousInnerAppointment) {
			base.AfterChangeAppointmentType(previousInnerAppointment);
			CreateRemindersFromCollection(new ReminderCollection(), true);
		}
	}
	#endregion
	#region InnerChangedOccurrenceAppointment
	public class InnerChangedOccurrenceAppointment : InnerOccurrenceBaseAppointment {
		public InnerChangedOccurrenceAppointment(Appointment owner)
			: base(owner) {
			XtraSchedulerDebug.Assert(owner.Type == AppointmentType.ChangedOccurrence);
		}
		public override AppointmentType Type { get { return AppointmentType.ChangedOccurrence; } }
		public override bool CanChangeType(AppointmentType newType) {
			return (newType == AppointmentType.DeletedOccurrence);
		}
		public override void AfterChangeAppointmentType(InnerAppointment previousInnerAppointment) {
			base.AfterChangeAppointmentType(previousInnerAppointment);
			CreateRemindersFromCollection(previousInnerAppointment.AppointmentReminders.Reminders, true);
		}
	}
	#endregion
	#region InnerDeletedOccurrenceAppointment
	public class InnerDeletedOccurrenceAppointment : InnerOccurrenceBaseAppointment {
		public InnerDeletedOccurrenceAppointment(Appointment owner)
			: base(owner) {
			XtraSchedulerDebug.Assert(owner.Type == AppointmentType.DeletedOccurrence);
		}
		public override AppointmentType Type { get { return AppointmentType.DeletedOccurrence; } }
		public override bool CanChangeType(AppointmentType newType) {
			return false;
		}
		public override void SetPattern(Appointment pattern) {
			base.SetPattern(pattern);
			CreateRemindersFromCollection(new ReminderCollection(), false);
		}
		public override void AfterChangeAppointmentType(InnerAppointment previousInnerAppointment) {
			base.AfterChangeAppointmentType(previousInnerAppointment);
			CreateRemindersFromCollection(previousInnerAppointment.AppointmentReminders.Reminders, true);
		}
	}
	#endregion
	#region InnerDisposedAppointment
	public class InnerDisposedAppointment : InnerAppointment {
		public InnerDisposedAppointment(Appointment owner)
			: base(owner) {
		}
		#region Properties
		public override Appointment Owner { get { return null; } }
		public override Appointment RecurrencePattern { get { return null; } protected internal set { } }
		public override int RecurrenceIndex { get { return 0; } protected internal set { } }
		public override AppointmentType Type { get { return AppointmentType.Normal; } }
		public override AppointmentResourceIdCollection ResourceIds { get { return null; } }
		protected internal override ReminderCollection Reminders { get { return null; } }
		protected internal override ReminderCollection InnerReminders { get { return null; } }
		#endregion
		public override bool CanChangeType(AppointmentType newType) {
			return false;
		}
		public override void Initialize() {
			throw new ObjectDisposedException("Appointment");
		}
		public override void CreateResources() {
			throw new ObjectDisposedException("Appointment");
		}
		public override void SetPattern(Appointment pattern) {
			throw new ObjectDisposedException("Appointment");
		}
		public override void AfterChangeAppointmentType(InnerAppointment previousInnerAppointment) {
			throw new ObjectDisposedException("Appointment");
		}
		protected internal override void SubscribeResourceIdsEvents() {
		}
		protected internal override void UnsubscribeResourceIdsEvents() {
		}
		protected internal override void OnResourceIdsChanging(object sender, CancellablePropertyChangingEventArgs e) {
		}
		protected internal override void RaisePropertyChanged(string propertyName) {
		}
		protected internal override bool CanDelete() {
			return false;
		}
		protected internal override void Delete() {
		}
	}
	#endregion
	#region AppointmentLabelColor
	public static class AppointmentLabelColor {
		public static readonly Color None = DXSystemColors.Window;
		public static readonly Color Important = DXColor.FromArgb(0xFF, 0xC2, 0xBE); 
		public static readonly Color Business = DXColor.FromArgb(0xA8, 0xD5, 0xFF); 
		public static readonly Color Personal = DXColor.FromArgb(0xC1, 0xF4, 0x9C); 
		public static readonly Color Vacation = DXColor.FromArgb(0xF3, 0xE4, 0xC7); 
		public static readonly Color MustAttend = DXColor.FromArgb(0xF4, 0xCE, 0x93); 
		public static readonly Color TravelRequired = DXColor.FromArgb(0xC7, 0xF4, 0xFF); 
		public static readonly Color NeedsPreparation = DXColor.FromArgb(0xCF, 0xDB, 0x98); 
		public static readonly Color Birthday = DXColor.FromArgb(0xE0, 0xCF, 0xE9); 
		public static readonly Color Anniversary = DXColor.FromArgb(0x8D, 0xE9, 0xDF); 
		public static readonly Color PhoneCall = DXColor.FromArgb(0xFF, 0xF7, 0xA5); 
	}
	#endregion
	#region AppointmentPatternExpander
	public class AppointmentPatternExpander {
		delegate Appointment FindOccurrenceDelegate(TimeInterval interval);
		Appointment pattern;
		public AppointmentPatternExpander(Appointment pattern) {
			if (pattern == null || pattern.Type != AppointmentType.Pattern)
				Exceptions.ThrowArgumentException("pattern", pattern);
			this.pattern = pattern;
		}
		public AppointmentBaseCollection Expand(TimeInterval interval) {
			if (interval == null)
				return new AppointmentBaseCollection();
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(this.pattern.RecurrenceInfo, ((IInternalAppointment)this.pattern).TimeZoneEngine);
			AppointmentBaseCollection result = calc.CalcOccurrences(interval, this.pattern);
			MergeSeriesWithException(interval, result);
			return result;
		}
		public AppointmentBaseCollection ExpandSkipExceptions(TimeInterval interval) {
			if (interval == null)
				return new AppointmentBaseCollection();
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(this.pattern.RecurrenceInfo, ((IInternalAppointment)this.pattern).TimeZoneEngine);
			AppointmentBaseCollection result = calc.CalcOccurrences(interval, this.pattern);
			if (this.pattern.HasExceptions) {
				AppointmentBaseCollection exceptions = ((IInternalAppointment)this.pattern).PatternExceptions;
				int count = exceptions.Count;
				for (int i = 0; i < count; i++)
					RemoveOccurrenceWithRecurrenceIndex(result, exceptions[i].RecurrenceIndex);
			}
			return result;
		}
		internal static void RemoveOccurrenceWithRecurrenceIndex(AppointmentBaseCollection series, int recurrenceIndex) {
			int count = series.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = series[i];
				if (apt.RecurrenceIndex == recurrenceIndex) {
					series.Remove(apt);
					break;
				}
			}
		}
		public Appointment FindLastOccurrence(TimeInterval interval) {
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo, ((IInternalAppointment)this.pattern).TimeZoneEngine);
			int index = calc.FindLastOccurrenceIndex(interval, pattern);
			if (index < 0)
				return null;
			return calc.CalcOccurrenceByIndex(index, pattern);
		}
		public Appointment FindFirstOccurrence(TimeInterval interval) {
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo, ((IInternalAppointment)this.pattern).TimeZoneEngine);
			int index = calc.FindFirstOccurrenceIndex(interval, pattern);
			if (index < 0)
				return null;
			return calc.CalcOccurrenceByIndex(index, pattern);
		}
		internal AppointmentBaseCollection ExpandLastOccurrences(TimeInterval interval) {
			return ExpandByRuleCore(interval, FindLastOccurrence);
		}
		internal AppointmentBaseCollection ExpandFirstOccurrences(TimeInterval interval) {
			return ExpandByRuleCore(interval, FindFirstOccurrence);
		}
		AppointmentBaseCollection ExpandByRuleCore(TimeInterval interval, FindOccurrenceDelegate findOccurrenceAction) {
			if (interval == null)
				return new AppointmentBaseCollection();
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			Appointment occurrence = findOccurrenceAction(interval);
			if (occurrence != null)
				result.Add(occurrence);
			MergeSeriesWithException(interval, result);
			return result;
		}
		void MergeSeriesWithException(TimeInterval interval, AppointmentBaseCollection result) {
			if (pattern.HasExceptions) {
				AppointmentBaseCollection exceptions = ((IInternalAppointment)pattern).PatternExceptions;
				int count = exceptions.Count;
				for (int i = 0; i < count; i++)
					MergeSeriesWithException(result, exceptions[i], interval);
			}
		}
		internal static void MergeSeriesWithException(AppointmentBaseCollection series, Appointment exception, TimeInterval interval) {
			RemoveOccurrenceWithRecurrenceIndex(series, exception.RecurrenceIndex);
			if (exception.Type == AppointmentType.ChangedOccurrence && AppointmentInstance.IsAppointmentIntersectsInterval(exception, interval))
				series.Add(exception);
		}
	}
	#endregion
	#region AppointmentCopyHelper
	public class AppointmentCopyHelper {
		public virtual void Assign(Appointment src, Appointment target) {
			((IBatchUpdateable)target).BeginUpdate();
			try {
				AssignCore(src, target);
			} finally {
				((IBatchUpdateable)target).CancelUpdate();
			}
		}
		protected internal virtual void AssignCore(Appointment src, Appointment target) {
			IInternalAppointment app = (IInternalAppointment)target;
			app.SetTypeCore(src.Type);
			app.TimeZoneEngine = ((IInternalAppointment)src).TimeZoneEngine;
			AssignSimpleProperties(src, target);
			AssignRecurrenceProperties(src, target);
			AssignCollectionProperties(src, target);
		}
		protected internal virtual void AssignSimpleProperties(Appointment src, Appointment target) {
			target.TimeZoneId = src.TimeZoneId;
			TimeInterval interval = ((IInternalAppointment)src).GetInterval();
			target.AllDay = false;
			target.Start = interval.InnerStart;
			target.Duration = interval.InnerDuration;
			target.AllDay = src.AllDay;
			target.Subject = src.Subject;
			target.Location = src.Location;
			target.Description = src.Description;
			target.StatusKey = src.StatusKey;
			target.LabelKey = src.LabelKey;
#if !SL && !WPF
			target.PercentComplete = src.PercentComplete;
#endif
		}
		protected internal virtual void AssignCollectionProperties(Appointment src, Appointment target) {
			src.CustomFields.CloneTo(target.CustomFields);
			AssignReminders(src, target);
			if (!(target.ResourceIds is AppointmentResourceIdReadOnlyCollection))
				((IInternalAppointment)target).CopyResources(src);
		}
		void AssignReminders(Appointment src, Appointment target) {
			IInternalAppointment app = ((IInternalAppointment)target);
			if (src.Reminders.Count == 0 && target.Reminders.Count != 0 && target.Type == AppointmentType.Occurrence) {
				app.OnContentChanged();
			}
			if (!(target.Reminders is ReminderReadOnlyCollection)) {
				app.CreateRemindersFromCollection(src.Reminders, true);
			}
		}
		protected internal virtual void AssignRecurrenceProperties(Appointment src, Appointment target) {
			if (src.Type == AppointmentType.Pattern)
				target.RecurrenceInfo.Assign(src.RecurrenceInfo);
			((IInternalAppointment)target).SetRecurrenceIndex(src.RecurrenceIndex);
		}
	}
	#endregion
	#region ExceptionPropertiesMerger
	public class ExceptionPropertiesMerger {
		#region Fields
		Appointment oldPattern;
		Appointment newPattern;
		AppointmentFormControllerBase controller;
		#endregion
		public ExceptionPropertiesMerger(Appointment oldPattern, Appointment newPattern, AppointmentFormControllerBase controller) {
			Guard.ArgumentNotNull(oldPattern, "oldPattern");
			Guard.ArgumentNotNull(newPattern, "newPattern");
			Guard.ArgumentNotNull(controller, "controller");
			this.oldPattern = oldPattern;
			this.newPattern = newPattern;
			this.controller = controller;
		}
		#region Properties
		public Appointment OldPattern { get { return oldPattern; } }
		public Appointment NewPattern { get { return newPattern; } }
		public AppointmentFormControllerBase Controller { get { return controller; } }
		#endregion
		public void Apply(Appointment exception) {
			AssignSimpleProperties(OldPattern, NewPattern, exception);
			AssignCollectionProperties(OldPattern, NewPattern, exception);
		}
		protected internal void AssignSimpleProperties(Appointment oldPattern, Appointment newPattern, Appointment exception) {
			XtraSchedulerDebug.Assert(oldPattern.AllDay == newPattern.AllDay);
			if (exception.Subject == oldPattern.Subject)
				exception.Subject = newPattern.Subject;
			if (exception.Location == oldPattern.Location)
				exception.Location = newPattern.Location;
			if (exception.Description == oldPattern.Description)
				exception.Description = newPattern.Description;
			if (Object.Equals(exception.StatusKey, oldPattern.StatusKey))
				exception.StatusKey = newPattern.StatusKey;
			if (Object.Equals(exception.LabelKey, oldPattern.LabelKey))
				exception.LabelKey = newPattern.LabelKey;
		}
		public void AssignCollectionProperties(Appointment oldPattern, Appointment newPattern, Appointment exception) {
			AssignCustomFields(oldPattern, NewPattern, exception);
			if (!(exception.Reminders is ReminderReadOnlyCollection))
				if (AreReminderCollectionsIdentical(oldPattern.Reminders, exception.Reminders))
					((IInternalAppointment)exception).CreateRemindersFromCollection(newPattern.Reminders, true);
		}
		protected internal virtual bool AreReminderCollectionsIdentical(ReminderCollection reminders1, ReminderCollection reminders2) {
			int count = reminders1.Count;
			if (reminders2.Count != count)
				return false;
			ReminderCollection copyReminders1 = new ReminderCollection(DXCollectionUniquenessProviderType.None);
			copyReminders1.AddRange(reminders1);
			ReminderCollection copyReminders2 = new ReminderCollection(DXCollectionUniquenessProviderType.None);
			copyReminders2.AddRange(reminders2);
			TimeBeforeStartReminderComparer comparer = new TimeBeforeStartReminderComparer();
			copyReminders1.Sort(comparer);
			copyReminders2.Sort(comparer);
			for (int i = 0; i < count; i++)
				if (copyReminders1[i].TimeBeforeStart != copyReminders2[i].TimeBeforeStart)
					return false;
			return true;
		}
		protected internal virtual void AssignCustomFields(Appointment oldPattern, Appointment newPattern, Appointment exception) {
			CustomFieldMappingCollectionBase<Appointment> mappings = Controller.InnerAppointments.CustomFieldMappings;
			int count = mappings.Count;
			for (int i = 0; i < count; i++) {
				CustomFieldMappingBase<Appointment> mapping = mappings[i];
				if (Controller.IsCustomFieldsEqual(mapping.Name, oldPattern, exception))
					Controller.AssignCustomField(mapping.Name, exception, newPattern);
			}
		}
		#region TimeBeforeStartReminderComparer
		class TimeBeforeStartReminderComparer : IComparer<Reminder> {
			#region IComparer<Reminder> Members
			public int Compare(Reminder x, Reminder y) {
				if (x.TimeBeforeStart < y.TimeBeforeStart)
					return -1;
				if (x.TimeBeforeStart == y.TimeBeforeStart)
					return 0;
				return 1;
			}
			#endregion
		}
		#endregion
	}
	#endregion
	public interface IAppointmentComparerProvider {
		AppointmentBaseComparer CreateAppointmentComparer();
		AppointmentBaseComparer CreateWeekViewAppointmentComparer();
	}
	#region AppointmentComparerProvider
	public class AppointmentComparerProvider : IAppointmentComparerProvider {
		IServiceProvider serviceProvider;
		public AppointmentComparerProvider() {
		}
		public AppointmentComparerProvider(IServiceProvider provider) {
			this.serviceProvider = provider;
		}
		public static AppointmentBaseComparer CreateDefaultAppointmentComparer() {
			return new AppointmentTimeIntervalComparer();
		}
		protected IServiceProvider ServiceProvider { get { return serviceProvider; } }
		protected virtual IComparer<Appointment> GetExternalAppointmentComparer() {
			if (ServiceProvider == null)
				return null;
			IExternalAppointmentCompareService result = ServiceProvider.GetService(typeof(IExternalAppointmentCompareService)) as IExternalAppointmentCompareService;
			return result != null ? result.Comparer : null;
		}
		public virtual AppointmentBaseComparer CreateAppointmentComparer() {
			IComparer<Appointment> extComparer = GetExternalAppointmentComparer();
			if (extComparer != null)
				return new ExternalAppointmentTimeIntervalComparer(extComparer);
			return new AppointmentTimeIntervalComparer();
		}
		public virtual AppointmentBaseComparer CreateWeekViewAppointmentComparer() {
			IComparer<Appointment> extComparer = GetExternalAppointmentComparer();
			if (extComparer != null)
				return new ExternalWeekViewAppointmentComparer(extComparer);
			return new WeekViewAppointmentComparer();
		}
	}
	#endregion
	#region ExternalAppointmentComparerBase
	public abstract class ExternalAppointmentComparerBase : AppointmentBaseComparer {
		IComparer<Appointment> comparer;
		protected ExternalAppointmentComparerBase(IComparer<Appointment> comparer) {
			Guard.ArgumentNotNull(comparer, "comparer");
			this.comparer = comparer;
		}
		public IComparer<Appointment> Comparer { get { return comparer; } }
		protected internal override int CompareCore(Appointment aptX, Appointment aptY) {
			int result = base.CompareCore(aptX, aptY);
			if (result == 0)
				return ExternalCompare(aptX, aptY);
			return result;
		}
		protected internal override int CompareAppointmentHandles(Appointment aptX, Appointment aptY) {
			return 0;
		}
		protected virtual int ExternalCompare(Appointment aptX, Appointment aptY) {
			return Comparer.Compare(aptX, aptY);
		}
	}
	#endregion
	#region ExternalAppointmentTimeIntervalComparer
	public class ExternalAppointmentTimeIntervalComparer : ExternalAppointmentComparerBase {
		public ExternalAppointmentTimeIntervalComparer(IComparer<Appointment> comparer)
			: base(comparer) {
		}
		protected internal override bool ApplyLongerThanDayComparison { get { return false; } }
	}
	#endregion
	#region ExternalWeekViewAppointmentComparer
	public class ExternalWeekViewAppointmentComparer : ExternalAppointmentComparerBase {
		public ExternalWeekViewAppointmentComparer(IComparer<Appointment> comparer)
			: base(comparer) {
		}
		protected internal override bool ApplyLongerThanDayComparison { get { return true; } }
	}
	#endregion
	public class AppointmentBatchUpdateHelper : BatchUpdateHelper {
		AppointmentChangeStateData changeStateData;
		public AppointmentBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		public void Reset() {
			this.changeStateData = null;
		}
		public bool IsEmptyChangeStateData { get { return changeStateData == null; } }
		public AppointmentChangeStateData ChangeStateData {
			get {
				if (changeStateData == null)
					changeStateData = new AppointmentChangeStateData();
				return changeStateData;
			}
		}
	}
	public class AppointmentChangeStateData {
		public TimeInterval Interval { get; set; }
	}
	public interface IInternalAppointment : IInternalPersistentObject {
		AppointmentBaseCollection PatternExceptions { get; }
		IRecurrenceChain RecurrenceChain { get; }
		IAppointmentReminders AppointmentReminders { get; }
		TimeZoneEngine TimeZoneEngine { get; set; }
		TimeInterval CreateInterval();
		TimeInterval GetInterval();
		void SetPattern(Appointment pattern);
		void SetTypeCore(AppointmentType type);
		void SetRecurrenceInfo(IRecurrenceInfo recurrenceInfo);
		void SetRecurrenceIndex(int recurrenceIndex);
		bool ShouldUpdateChangeStateData(string propertyName, object oldValue, object newValue);
		void CopyResources(Appointment src);
		void SetResourceId(object id);
		AppointmentChangeStateData GetChangeStateData();
		Appointment CopyCore();
		int BeginDismissReminder(Reminder reminder);
		void CancelDismissReminder(Reminder reminder, int index);
		void EndDismissReminder(Reminder reminder);
		void UpdateReminders();
		void CreateRemindersFromCollection(ReminderCollection source, bool createSourceCopy);
	}
	public static class InternalAppointmentPropertyAccessor {
		public static TimeZoneEngine GetTimeZoneEngine(Appointment apt) {
			IInternalAppointment iiApt = apt as IInternalAppointment;
			if (iiApt == null)
				return null;
			return iiApt.TimeZoneEngine;
		}
	}
}
namespace DevExpress.XtraScheduler.Internal.Implementations {
	public class AppointmentInstance : PersistentObject, Appointment, IInternalAppointment, INotifyPropertyChanged {
		#region Fields
		internal const int DefaultStatus = (int)AppointmentStatusType.Free;
		object id;
		InnerAppointment innerAppointment;
		AppointmentType type = AppointmentType.Normal;
		#endregion
		#region Constructors
		public AppointmentInstance() {
			this.innerAppointment = CreateInnerAppointment(AppointmentType.Normal);
			this.innerAppointment.Initialize();
			SubscribeInnerAppointmentEvents();
			SubscribeBaseAppointmentEvents();
		}
		protected internal AppointmentInstance(AppointmentType type) {
			this.type = type;
			this.innerAppointment = CreateInnerAppointment(type);
			this.innerAppointment.Initialize();
			SubscribeInnerAppointmentEvents();
			SubscribeBaseAppointmentEvents();
		}
		protected internal AppointmentInstance(DateTime start, TimeSpan duration, string subject)
			: this(AppointmentType.Normal, start, duration, subject) {
		}
		protected internal AppointmentInstance(AppointmentType type, DateTime start, TimeSpan duration, string subject) {
			this.type = type;
			this.innerAppointment = CreateInnerAppointment(type);
			BaseAppointment.Start = start;
			BaseAppointment.Duration = duration;
			BaseAppointment.Subject = subject;
			this.innerAppointment.Initialize();
			SubscribeInnerAppointmentEvents();
			SubscribeBaseAppointmentEvents();
		}
		protected internal AppointmentInstance(AppointmentType type, DateTime start, TimeSpan duration, string subject, object id)
			: this(type, start, duration, subject) {
			this.id = id;
		}
		protected internal AppointmentInstance(AppointmentType type, DateTime start, DateTime end, string subject)
			: this(type, start, end - start, subject) {
		}
		protected internal AppointmentInstance(AppointmentType type, DateTime start, TimeSpan duration)
			: this(type, start, duration, String.Empty) {
		}
		protected internal AppointmentInstance(AppointmentType type, DateTime start, DateTime end)
			: this(type, start, end, String.Empty) {
		}
		protected internal AppointmentInstance(DateTime start, DateTime end, string subject)
			: this(start, end - start, subject) {
		}
		protected internal AppointmentInstance(DateTime start, DateTime end)
			: this(start, end, String.Empty) {
		}
		protected internal AppointmentInstance(DateTime start, TimeSpan duration)
			: this(start, duration, String.Empty) {
		}
		#endregion
		#region Properties
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentStart")]
#endif
		public DateTime Start { get { return BaseAppointment.Start; } set { BaseAppointment.Start = value; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentEnd")]
#endif
		public DateTime End { get { return BaseAppointment.End; } set { BaseAppointment.End = value; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDuration")]
#endif
		public TimeSpan Duration {
			get { return BaseAppointment.Duration; }
			set {
				BaseAppointment.Duration = value;
				NotifyPropertyChanged("Duration");
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentAllDay")]
#endif
		public bool AllDay { get { return BaseAppointment.AllDay; } set { BaseAppointment.AllDay = value; } }
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentSubject"),
#endif
 DefaultValue("")]
		public string Subject { get { return BaseAppointment.Subject; } set { BaseAppointment.Subject = value; } }
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentLocation"),
#endif
 DefaultValue("")]
		public string Location { get { return BaseAppointment.Location; } set { BaseAppointment.Location = value; } }
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDescription"),
#endif
 DefaultValue("")]
		public string Description { get { return BaseAppointment.Description; } set { BaseAppointment.Description = value; } }
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentLabelKey"),
#endif
 DefaultValue(0)]
		public object LabelKey { get { return BaseAppointment.LabelId; } set { BaseAppointment.LabelId = value; } }
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentStatusKey"),
#endif
 DefaultValue(DefaultStatus)]
		public object StatusKey { get { return BaseAppointment.StatusId; } set { BaseAppointment.StatusId = value; } }
		int Appointment.StatusId {
			get {
				int statusValue = 0;
				try {
					statusValue = Convert.ToInt32(StatusKey);
				} catch { }
				return statusValue;
			}
			set {
				StatusKey = value;
			}
		}
		int Appointment.LabelId {
			get {
				int labelValue = 0;
				try {
					labelValue = Convert.ToInt32(LabelKey);
				} catch { }
				return labelValue;
			}
			set {
				LabelKey = value;
			}
		}
		#region TimeZoneId
		public string TimeZoneId {
			get { return BaseAppointment.TimeZoneId; }
			set {
				BaseAppointment.TimeZoneId = value;
				if (RecurrenceInfo != null && Type == AppointmentType.Pattern)
					((IInternalRecurrenceInfo)RecurrenceInfo).SetTimeZoneId(value);
			}
		}
		#endregion
		TimeZoneEngine IInternalAppointment.TimeZoneEngine { get; set; }
		internal TimeZoneEngine TimeZoneEngine {
			get { return ((IInternalAppointment)this).TimeZoneEngine; }
			set { ((IInternalAppointment)this).TimeZoneEngine = value; }
		}
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentType"),
#endif
 DefaultValue(AppointmentType.Normal)]
		public AppointmentType Type { get { return type; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentRecurrenceInfo")]
#endif
		public IRecurrenceInfo RecurrenceInfo { get { return InnerAppointment.RecurrenceInfo as IRecurrenceInfo; } }
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentRecurrencePattern"),
#endif
 DefaultValue(null)]
		public Appointment RecurrencePattern { get { return InnerAppointment.RecurrencePattern; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentRecurrenceIndex")]
#endif
		public int RecurrenceIndex { get { return InnerAppointment.RecurrenceIndex; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentReminders")]
#endif
		public ReminderCollection Reminders {
			get {
				if (IsDisposed)
					Exceptions.ThrowObjectDisposedException("Appointment", "Reminders");
				return InnerAppointment.Reminders;
			}
		}
		internal ReminderCollection InnerReminders { get { return InnerAppointment.InnerReminders; } }
		internal IAppointmentBase BaseAppointment { get { return InnerAppointment.BaseAppointment; } }
		public IAppointmentReminders AppointmentReminders { get { return InnerAppointment.AppointmentReminders; } }
		internal IAppointmentReminders InnerAppointmentReminders { get { return InnerAppointment.InnerAppointmentReminders; } }
		internal IAppointmentResources AppointmentResources { get { return InnerAppointment.AppointmentResources; } }
		public IRecurrenceChain RecurrenceChain { get { return InnerAppointment.RecurrenceChain; } }
#if !WPF
		internal IAppointmentProcess AppointmentProcess { get { return InnerAppointment.AppointmentProcess; } }
#endif
		internal InnerAppointment InnerAppointment { get { return innerAppointment; } }
		internal IRecurrenceInfo InnerRecurrenceInfo { get { return InnerAppointment.InnerRecurrenceInfo; } }
		AppointmentBaseCollection IInternalAppointment.PatternExceptions { get { return InnerAppointment.PatternExceptions; } }
#if !WPF
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentPercentComplete")]
#endif
		public int PercentComplete {
			get {
				if (InnerAppointment.InnerAppointmentProcess == null)
					return 0;
				return AppointmentProcess.PercentComplete;
			}
			set {
				if (PercentComplete == value)
					return;
				AppointmentProcess.PercentComplete = value;
			}
		}
#endif
		#region HasReminder
		void CreateRemindersByHasReminderValue(bool hasReminder) {
			if (hasReminder)
				InnerAppointment.CreateSingleReminder();
			else
				InnerAppointment.DisposeReminders();
		}
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentHasReminder"),
#endif
 DefaultValue(false)]
		public bool HasReminder {
			get { return InnerAppointment.HasReminder; }
			set {
				if (Type != AppointmentType.Occurrence) {
					if (HasReminder == value)
						return;
				}
				bool oldHasReminder = HasReminder;
				ReminderCollection oldReminderCollection = HasReminder ? this.Reminders : null;
				if (Type == AppointmentType.Occurrence) {
					Appointment currentRecurrencePattern = InnerAppointment.RecurrencePattern;
					InnerAppointment.RecurrencePattern = null;
					CreateRemindersByHasReminderValue(value);
					InnerAppointment.RecurrencePattern = currentRecurrencePattern;
				} else
					CreateRemindersByHasReminderValue(value);
				if (OnContentChanging("HasReminder", oldHasReminder, value)) {
					UpdateReminders();
					OnContentChanged();
				} else {
					if (oldReminderCollection != null)
						InnerAppointment.CreateRemindersFromCollection(oldReminderCollection, false);
					else
						InnerAppointment.DisposeReminders();
				}
			}
		}
		#endregion
		#region ResourceId
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentResourceId")]
#endif
		public object ResourceId {
			get { return InnerResourceIds != null ? InnerResourceIds[0] : EmptyResourceId.Id; }
			set {
				if (!IsBase)
					Exceptions.ThrowArgumentException("ResourceId", value);
				SetResourceId(value);
			}
		}
		#endregion
		protected internal AppointmentResourceIdCollection InnerResourceIds { get { return InnerAppointment.InnerResourceIds; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentResourceIds")]
#endif
		public AppointmentResourceIdCollection ResourceIds {
			get {
				if (IsDisposed)
					Exceptions.ThrowObjectDisposedException("Appointment", "ResourceIds");
				return InnerAppointment.ResourceIds;
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentSameDay")]
#endif
		public bool SameDay { get { return ((IInternalAppointment)this).GetInterval().SameDay; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentLongerThanADay")]
#endif
		public bool LongerThanADay { get { return ((IInternalAppointment)this).GetInterval().LongerThanADay; } }
		internal bool SeveralDays { get { return LongerThanADay && !AllDay; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentIsRecurring")]
#endif
		public bool IsRecurring { get { return Type != AppointmentType.Normal; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentIsException")]
#endif
		public bool IsException { get { return SchedulerUtils.IsExceptionType(Type); } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentIsOccurrence")]
#endif
		public bool IsOccurrence { get { return SchedulerUtils.IsOccurrenceType(Type); } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentIsBase")]
#endif
		public bool IsBase { get { return SchedulerUtils.IsBaseType(Type); } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentHasExceptions")]
#endif
		public bool HasExceptions { get { return Type == AppointmentType.Pattern && ((IInternalAppointment)this).PatternExceptions != null && ((IInternalAppointment)this).PatternExceptions.Count > 0; } }
		#region Reminder
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentReminder")]
#endif
		public Reminder Reminder { get { return InnerReminders != null && InnerReminders.Count > 0 ? InnerReminders[0] : null; } }
		#endregion
		[Obsolete("You should handle the 'SchedulerControl.AppointmentViewInfoCustomizing' event instead.", true), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public object Appearance { get { return null; } }
		public override bool DeferChangingToEndUpdate { get { return Type != AppointmentType.Occurrence; } }
		public override object Id { get { return id; } }
		protected internal AppointmentBatchUpdateHelper AppointmentBatchUpdateHelper {
			get {
				IBatchUpdateable batchUpdateable = this;
				return (AppointmentBatchUpdateHelper)batchUpdateable.BatchUpdateHelper;
			}
		}
		#endregion
		#region IBatchUpdateHandler implementation
		protected override void OnBeginUpdate() {
			if (InnerRecurrenceInfo != null)
				InnerRecurrenceInfo.BeginUpdate();
			base.OnBeginUpdate();
		}
		protected override void OnEndUpdate() {
			if (InnerRecurrenceInfo != null)
				InnerRecurrenceInfo.EndUpdate();
			base.OnEndUpdate();
		}
		protected override void OnCancelUpdate() {
			if (InnerRecurrenceInfo != null)
				InnerRecurrenceInfo.CancelUpdate();
			base.OnCancelUpdate();
		}
		#endregion
		#region Events
		[Obsolete("You should use one of the following events instead: SchedulerStorage.AppointmentChanging, SchedulerStorage.AppointmentsChanged, SchedulerStorage.AppointmentInserting, SchedulerStorage.AppointmentsInserted, SchedulerStorage.AppointmentDeleting, SchedulerStorage.AppointmentsDeleted.", true), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public event ChangeEventHandler TypeChanged { add { } remove { } }
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing)
					DisposeInnerAppointment();
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected override void InitBatchUpdateHelper() {
		}
		protected override bool CanReleaseBatchUpdateHelper() {
			return true;
		}
		protected override bool ShouldRecreateBatchUpdateHelper() {
			return AppointmentBatchUpdateHelper == null;
		}
		protected override BatchUpdateHelper CreateBatchUpdateHelper() {
			return new AppointmentBatchUpdateHelper(this);
		}
		protected internal virtual InnerAppointment CreateInnerAppointment(AppointmentType appointmentType) {
			switch (appointmentType) {
				default:
				case AppointmentType.Normal:
					return CreateInnerNormalAppointment();
				case AppointmentType.Pattern:
					return CreateInnerPatternAppointment();
				case AppointmentType.Occurrence:
					return CreateInnerOccurrenceAppointment();
				case AppointmentType.ChangedOccurrence:
					return CreateInnerChangedOccurrenceAppointment();
				case AppointmentType.DeletedOccurrence:
					return CreateInnerDeletedOccurrenceAppointment();
			}
		}
		protected internal virtual InnerNormalAppointment CreateInnerNormalAppointment() {
			return new InnerNormalAppointment(this);
		}
		protected internal virtual InnerPatternAppointment CreateInnerPatternAppointment() {
			return new InnerPatternAppointment(this);
		}
		protected internal virtual InnerOccurrenceAppointment CreateInnerOccurrenceAppointment() {
			return new InnerOccurrenceAppointment(this);
		}
		protected internal virtual InnerChangedOccurrenceAppointment CreateInnerChangedOccurrenceAppointment() {
			return new InnerChangedOccurrenceAppointment(this);
		}
		protected internal virtual InnerDeletedOccurrenceAppointment CreateInnerDeletedOccurrenceAppointment() {
			return new InnerDeletedOccurrenceAppointment(this);
		}
		public TimeInterval CreateInterval() {
			TimeInterval interval = ((IInternalAppointmentBase)BaseAppointment).Interval;
			TimeInterval result = new TimeInterval(interval.InnerStart, interval.InnerDuration);
			result.AllDay = interval.AllDay;
			return result;
		}
		public TimeInterval GetInterval() {
			return ((IInternalAppointmentBase)BaseAppointment).Interval;
		}
		protected internal virtual void DisposeInnerAppointment() {
			if (innerAppointment == null || (innerAppointment is InnerDisposedAppointment))
				return;
			InnerDisposedAppointment disposedInnerAppointment = new InnerDisposedAppointment(this);
			UnsubscribeInnerAppointmentEvents();
			innerAppointment.Dispose();
			innerAppointment = disposedInnerAppointment;
		}
		protected internal virtual void SubscribeInnerAppointmentEvents() {
			InnerAppointment.PropertyChanging += OnInnerAppointmentChanging;
			InnerAppointment.PropertyChanged += OnInnerAppointmentChanged;
		}
		protected internal virtual void UnsubscribeInnerAppointmentEvents() {
			InnerAppointment.PropertyChanging -= OnInnerAppointmentChanging;
			InnerAppointment.PropertyChanged -= OnInnerAppointmentChanged;
		}
		protected internal virtual void OnInnerAppointmentChanging(object sender, CancellablePropertyChangingEventArgs e) {
			e.Cancel = !OnContentChanging(e.PropertyName, e.OldValue, e.NewValue);
		}
		protected internal virtual void OnInnerAppointmentChanged(object sender, PropertyChangedEventArgs e) {
			OnContentChanged();
			RaisePropertyChanged(e.PropertyName);
		}
		#region Recurrence
		#region Pattern Exceptions
		public void DeleteExceptions() {
			if (this.Type != AppointmentType.Pattern)
				Exceptions.ThrowArgumentException("Pattern", this);
			if (((IInternalAppointment)this).RecurrenceChain.CanDeleteExceptions())
				((IInternalAppointment)this).RecurrenceChain.DeleteExceptions();
		}
		#endregion
		public AppointmentBaseCollection GetExceptions() {
			if (this.Type != AppointmentType.Pattern)
				Exceptions.ThrowArgumentException("Pattern", this);
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			if (HasExceptions)
				result.AddRange(((IInternalAppointment)this).PatternExceptions);
			return result;
		}
		public Appointment FindException(int recurrenceIndex) {
			if (!HasExceptions)
				return null;
			return ((IInternalAppointment)this).RecurrenceChain.FindException(recurrenceIndex);
		}
		public Appointment GetOccurrence(int recurrenceIndex) {
			if (this.Type != AppointmentType.Pattern)
				Exceptions.ThrowArgumentException("Pattern", this);
			return ((IInternalAppointment)this).RecurrenceChain.GetOccurrence(recurrenceIndex);
		}
		public void RestoreOccurrence() {
			if (!IsOccurrence)
				Exceptions.ThrowArgumentException("Occurrence", this);
			if (this.Type == AppointmentType.Occurrence)
				return;
			if (this.Type == AppointmentType.ChangedOccurrence)
				this.Delete(); 
			this.Delete(); 
		}
		public Appointment CreateException(AppointmentType type, int recurrenceIndex) {
			if (this.Type != AppointmentType.Pattern)
				Exceptions.ThrowArgumentException("Pattern", this);
			return ((IInternalAppointment)this).RecurrenceChain.CreateException(type, recurrenceIndex);
		}
		public virtual void SetPattern(Appointment pattern) {
			InnerAppointment.SetPattern(pattern);
		}
		public void SetRecurrenceInfo(IRecurrenceInfo recurrenceInfo) {
			InnerAppointment.SetRecurrenceInfo(recurrenceInfo);
		}
		public void SetRecurrenceIndex(int recurrenceIndex) {
			InnerAppointment.RecurrenceIndex = recurrenceIndex;
		}
		#endregion
		#region Reminders
		public int BeginDismissReminder(Reminder reminder) {
			return AppointmentReminders.BeginDismissReminder(reminder);
		}
		public void CancelDismissReminder(Reminder reminder, int index) {
			AppointmentReminders.CancelDismissReminder(reminder, index);
		}
		public void EndDismissReminder(Reminder reminder) {
			AppointmentReminders.EndDismissReminder(reminder);
			OnContentChanged();
		}
		public virtual void UpdateReminders() {
			if (InnerAppointmentReminders != null)
				InnerAppointmentReminders.UpdateReminders();
		}
		public Reminder CreateNewReminder() {
			return AppointmentReminders.CreateNewReminder();
		}
		public void CreateRemindersFromCollection(ReminderCollection source, bool createSourceCopy) {
			InnerAppointment.CreateRemindersFromCollection(source, createSourceCopy);
		}
		#endregion
		#region Resources
		public void SetResourceId(object id) {
			if (id == null)
				id = EmptyResourceId.Id;
			if (ResourceIds == null)
				return;
			if (ResourceIds.Count == 1 && id == ResourceIds[0])
				return;
			ResourceIds.BeginUpdate();
			try {
				ResourceIds.Clear();
				ResourceIds.Add(id);
			} finally {
				if (ResourceIds.Contains(id))
					ResourceIds.EndUpdate();
				else
					ResourceIds.CancelUpdate();
			}
		}
		public virtual void CopyResources(Appointment src) {
			InnerAppointment.UnsubscribeResourceIdsEvents();
			try {
				this.ResourceIds.BeginUpdate();
				try {
					this.ResourceIds.Clear();
					this.ResourceIds.AddRange(src.ResourceIds);
				} finally {
					this.ResourceIds.CancelUpdate();
				}
			} finally {
				InnerAppointment.SubscribeResourceIdsEvents();
			}
		}
		#endregion
		public Appointment Copy() {
			Appointment apt = CopyCore();
			if (apt.IsOccurrence)
				((IInternalAppointment)apt).SetTypeCore(AppointmentType.Normal);
			return apt;
		}
		public Appointment CopyCore() {
			Appointment apt = (Appointment)Activator.CreateInstance(GetType());
			apt.Assign(this);
			return apt;
		}
		public virtual void Assign(Appointment src) {
			AppointmentCopyHelper helper = new AppointmentCopyHelper();
			helper.Assign(src, this);
		}
		public virtual void SetTypeCore(AppointmentType newType) {
			if (this.Type == newType)
				return;
			InnerAppointment prevInnerAppointment = InnerAppointment;
			this.innerAppointment.BeforeChangeAppointmentType();
			UnsubscribeInnerAppointmentEvents();
			this.type = newType;
			DateTime startCopy = Start;
			TimeSpan durationCopy = Duration;
			this.innerAppointment = CreateInnerAppointment(newType);
			BaseAppointment.Start = startCopy; 
			BaseAppointment.Duration = durationCopy; 
			this.innerAppointment.Initialize();
			this.innerAppointment.AfterChangeAppointmentType(prevInnerAppointment);
			prevInnerAppointment.DetachInnerObjects();
			prevInnerAppointment.Dispose();
			SubscribeInnerAppointmentEvents();
		}
		protected internal void SubscribeBaseAppointmentEvents() {
			BaseAppointment.PropertyChanging += OnBaseAppointmentChanging;
			BaseAppointment.PropertyChanged += OnBaseAppointmentChanged;
		}
		protected internal void UnsubscribeBaseAppointmentEvents() {
			BaseAppointment.PropertyChanging -= OnBaseAppointmentChanging;
			BaseAppointment.PropertyChanged -= OnBaseAppointmentChanged;
		}
		protected internal virtual void OnBaseAppointmentChanging(object sender, CancellablePropertyChangingEventArgs e) {
			if (((IInternalAppointment)this).ShouldUpdateChangeStateData(e.PropertyName, e.OldValue, e.NewValue)) {
				UpdateChangeStateData(e);
			}
			e.Cancel = !OnContentChanging(e.PropertyName, e.OldValue, e.NewValue);
		}
		protected internal virtual void OnBaseAppointmentChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "Start") {
				if (Type != AppointmentType.Occurrence)
					UpdateReminders();
			} else if (e.PropertyName == "End" || e.PropertyName == "Duration" || e.PropertyName == "AllDay")
				UpdateReminders();
			OnContentChanged();
		}
		public override void Delete() {
			base.Delete();
		}
		public override object GetRow(ISchedulerStorageBase storage) {
			return (storage != null) ? storage.Appointments.GetObjectRow(this) : null;
		}
		public override object GetValue(ISchedulerStorageBase storage, string columnName) {
			return (storage != null) ? storage.Appointments.GetObjectValue(this, columnName) : null;
		}
		public override void SetValue(ISchedulerStorageBase storage, string columnName, object val) {
			if (storage != null)
				storage.Appointments.SetObjectValue(this, columnName, val);
		}
		void IIdProvider.SetId(object id) {
			this.id = id;
		}
		internal static bool IsAppointmentIntersectsInterval(Appointment apt, TimeInterval interval) {
			TimeInterval aptInterval = ((IInternalAppointment)apt).GetInterval();
			if (interval.Duration == TimeSpan.Zero)
				return interval.IntersectsWith(aptInterval);
			bool result = interval.IntersectsWithExcludingBounds(aptInterval);
			if (apt.Duration == TimeSpan.Zero)
				return result || (interval.Start == apt.Start);
			else
				return result;
		}
		protected internal override XmlPersistenceHelper CreateXmlPersistenceHelper() {
			return new AppointmentXmlPersistenceHelper(this, null);
		}
		public override bool CanDelete() {
			return InnerAppointment.CanDelete();
		}
		public override void DeleteCore() {
			InnerAppointment.Delete();
		}
		public override void RollbackObjectState(IPersistentObject objectToRollback) {
			AppointmentInstance apt = (AppointmentInstance)objectToRollback;
			apt.UnsubscribeBaseAppointmentEvents();
			try {
				apt.AllDay = false; 
			} finally {
				apt.SubscribeBaseAppointmentEvents();
			}
			if (IsException && this.RecurrencePattern != null)
				((IInternalAppointment)this.RecurrencePattern).RollbackObjectState(objectToRollback);
			else
				base.RollbackObjectState(objectToRollback);
		}
		public override bool OnContentChanging(string propertyName, object oldValue, object newValue) {
			if (Type == AppointmentType.Occurrence) {
				bool onContentChangingWasRaised;
				if (IsUpdateLocked)
					onContentChangingWasRaised = this.OnContentChangingWasRaised;
				else
					onContentChangingWasRaised = false;
				bool result = base.OnContentChanging(propertyName, oldValue, newValue);
				if (!onContentChangingWasRaised && this.RecurrencePattern != null) {
					result = result && ((IInternalPersistentObject)RecurrencePattern).RaiseStateChanging(this, PersistentObjectState.Changed, propertyName, oldValue, newValue);
					((IInternalPersistentObject)this).ChangeCancelled = !result;
				}
				return result;
			} else {
				if (this.RecurrencePattern != null) {
					if (IsUpdateLocked)
						return base.OnContentChanging(propertyName, oldValue, newValue);
					else
						return ((IInternalPersistentObject)RecurrencePattern).RaiseStateChanging(this, PersistentObjectState.Changed, propertyName, oldValue, newValue);
				} else
					return base.OnContentChanging(propertyName, oldValue, newValue);
			}
		}
		public override void OnContentChanged() {
			if (this.Type == AppointmentType.Occurrence && this.RecurrencePattern != null) {
				((IInternalAppointment)this).SetTypeCore(AppointmentType.ChangedOccurrence);
				((IInternalAppointment)RecurrencePattern).RecurrenceChain.RegisterException(this);
			} else if (SchedulerUtils.IsExceptionType(this.Type)) {
				base.OnContentChanged();
				if (this.RecurrencePattern != null && !IsUpdateLocked)
					((IInternalPersistentObject)RecurrencePattern).RaiseStateChanged(this, PersistentObjectState.Changed);
			} else
				base.OnContentChanged();
		}
		#region INotifyPropertyChanged Members
#if SILVERLIGHT
		public event PropertyChangedEventHandler PropertyChanged;
#endif
		void NotifyPropertyChanged(String info) {
#if SILVERLIGHT
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
#endif
		}
		#endregion
		bool IInternalAppointment.ShouldUpdateChangeStateData(string propertyName, object oldValue, object newValue) {
			if (Type != AppointmentType.Normal)
				return false;
			if (RowHandle == null)
				return false;
			if (RowHandle is int && (int)RowHandle == -1)
				return false;
			bool batchUpdatable = string.IsNullOrEmpty(propertyName);
			if (batchUpdatable)
				return true;
			if (Object.Equals(newValue, oldValue))
				return false;
			return IsChangeStateDataProperty(propertyName);
		}
		protected internal void UpdateChangeStateData(CancellablePropertyChangingEventArgs e) {
			TimeInterval currentInterval = null;
			if (e.PropertyName == "Start") {
				currentInterval = new TimeInterval(Convert.ToDateTime(e.OldValue), Duration);
			}
			if (e.PropertyName == "End") {
				currentInterval = new TimeInterval(Start, Convert.ToDateTime(e.NewValue));
			}
			if (e.PropertyName == "Duration") {
				currentInterval = new TimeInterval(Start, (TimeSpan)e.OldValue);
			}
			if (e.PropertyName == "AllDay") {
				currentInterval = ((IInternalAppointment)this).CreateInterval();
			}
			if (ShouldRecreateBatchUpdateHelper())
				RecreateBatchUpdateHelper();
			SaveChangeStateDataInterval(currentInterval);
		}
		protected internal bool IsChangeStateDataProperty(string propertyName) {
			List<string> propertyNames = new List<string>(new string[] { "Start", "End", "Duration", "AllDay" });
			return propertyNames.Contains(propertyName);
		}
		protected override void SaveObjectStateToBatchUpdateHelper() {
			if (AppointmentBatchUpdateHelper.IsEmptyChangeStateData)
				SaveChangeStateDataInterval(((IInternalAppointment)this).CreateInterval());
		}
		protected void SaveChangeStateDataInterval(TimeInterval interval) {
			if (AppointmentBatchUpdateHelper.ChangeStateData.Interval == null)
				AppointmentBatchUpdateHelper.ChangeStateData.Interval = interval;
		}
		public AppointmentChangeStateData GetChangeStateData() {
			return AppointmentBatchUpdateHelper.IsEmptyChangeStateData ? null : AppointmentBatchUpdateHelper.ChangeStateData;
		}
		#region INormalAppointment
		string Appointment.TimeZoneId {
			get { return TimeZoneId; }
			set { TimeZoneId = value; }
		}
		Appointment Appointment.RecurrencePattern { get { return RecurrencePattern; } }
		#region PropertyChanged
		event PropertyChangedEventHandler PropertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { PropertyChanged += value; }
			remove { PropertyChanged -= value; }
		}
		void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged == null)
				return;
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
#endregion
#endregion
