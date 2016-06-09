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
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Drawing;
using System.Runtime.InteropServices;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler {
	#region ChangeEventHandler
	public delegate void ChangeEventHandler(object sender, ChangeEventArgs e);
	#endregion
	#region ChangeEventArgs
	public class ChangeEventArgs : EventArgs {
		#region Fields
		string propertyName;
		object oldValue;
		object newValue;
		#endregion
		public ChangeEventArgs(string propertyName, object oldValue, object newValue) {
			this.propertyName = propertyName;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		#region Properties
		public object OldValue { get { return oldValue; } }
		public object NewValue { get { return newValue; } }
		public string PropertyName { get { return propertyName; } }
		#endregion
	}
	#endregion
	#region RemindersFormDefaultActionEventHandler
	public delegate void RemindersFormDefaultActionEventHandler(object sender, RemindersFormDefaultActionEventArgs e);
	#endregion
	#region RemindersFormDefaultActionEventArgs
	public class RemindersFormDefaultActionEventArgs : ReminderEventArgs {
		#region Fields
		bool handled;
		bool cancel;
		#endregion
		public RemindersFormDefaultActionEventArgs(ReminderAlertNotificationCollection alertNotifications)
			: base(alertNotifications) {
		}
		#region Properties
		public bool Handled { get { return handled; } set { handled = value; } }
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		#endregion
	}
	#endregion
	#region PersistentObjectEventHandler
	public delegate void PersistentObjectEventHandler(object sender, PersistentObjectEventArgs e);
	#endregion
	#region PersistentObjectEventArgs
	public class PersistentObjectEventArgs : EventArgs {
		IPersistentObject obj;
		public PersistentObjectEventArgs(IPersistentObject obj) {
			if (obj == null)
				Exceptions.ThrowArgumentException("obj", obj);
			this.obj = obj;
		}
		public IPersistentObject Object { get { return obj; } }
	}
	#endregion
	#region PersistentObjectCancelEventHandler
	public delegate void PersistentObjectCancelEventHandler(object sender, PersistentObjectCancelEventArgs e);
	#endregion
	#region PersistentObjectCancelEventArgs
	public class PersistentObjectCancelEventArgs : PersistentObjectEventArgs {
		bool cancel;
		public PersistentObjectCancelEventArgs(IPersistentObject obj)
			: base(obj) {
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	#endregion
	#region PersistentObjectsEventHandler
	public delegate void PersistentObjectsEventHandler(object sender, PersistentObjectsEventArgs e);
	#endregion
	#region PersistentObjectsEventArgs
	public class PersistentObjectsEventArgs : EventArgs {
		IList objects;
		public PersistentObjectsEventArgs(IList objects) {
			if (objects == null)
				Exceptions.ThrowArgumentException("objects", objects);
			this.objects = objects;
		}
		public IList Objects { get { return objects; } }
	}
	#endregion
	#region CancelListChangedEventHandler
	public delegate void CancelListChangedEventHandler(object sender, CancelListChangedEventArgs e);
	#endregion
	#region CancelListChangedEventArgs
	[System.Runtime.InteropServices.ComVisible(false)]
	public class CancelListChangedEventArgs : ListChangedEventArgs {
		bool cancel;
		public CancelListChangedEventArgs(ListChangedType listChangedType, int newIndex, int oldIndex)
			: base(listChangedType, newIndex, oldIndex) {
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	#endregion
	#region ReminderEventHandler
	public delegate void ReminderEventHandler(object sender, ReminderEventArgs e);
	#endregion
	#region ReminderBaseEventArgs
	public class ReminderBaseEventArgs : EventArgs {
		ReminderBaseAlertNotificationCollection alertNotifications;
		public ReminderBaseEventArgs(ReminderBaseAlertNotificationCollection alertNotifications) {
			if (alertNotifications == null)
				Exceptions.ThrowArgumentException("alertNotifications", alertNotifications);
			this.alertNotifications = alertNotifications;
		}
		public ReminderBaseAlertNotificationCollection AlertNotifications { get { return alertNotifications; } }
	}
	#endregion
	#region ReminderEventArgs
	public class ReminderEventArgs : ReminderBaseEventArgs {
		public ReminderEventArgs(ReminderAlertNotificationCollection alertNotifications)
			: base(alertNotifications) {
		}
		public new ReminderAlertNotificationCollection AlertNotifications { get { return (ReminderAlertNotificationCollection)base.AlertNotifications; } }
	}
	#endregion
	public class ReminderCancelEventArgs : EventArgs {
		public ReminderCancelEventArgs(Reminder reminder) {
			Reminder = reminder;
		}
		public Reminder Reminder { get; internal set; }
		public bool Cancel { get; set; }
	}
	#region TimeIntervalEventArgs
	public delegate void TimeIntervalEventHandler(object sender, TimeIntervalEventArgs e);
	public class TimeIntervalEventArgs : EventArgs {
		TimeInterval interval;
		public TimeIntervalEventArgs(TimeInterval interval) {
			if (interval == null)
				Exceptions.ThrowArgumentException("interval", interval);
			this.interval = interval;
		}
		public TimeInterval Interval { get { return interval; } }
	}
	#endregion
	#region FetchAppointmentsEventHandler
	public delegate void FetchAppointmentsEventHandler(object sender, FetchAppointmentsEventArgs e);
	#endregion
	#region FetchAppointmentsEventArgs
	public class FetchAppointmentsEventArgs : TimeIntervalEventArgs {
		bool forceReloadAppointments;
		public FetchAppointmentsEventArgs(TimeInterval interval)
			: base(interval) {
		}
		public bool ForceReloadAppointments { get { return forceReloadAppointments; } set { forceReloadAppointments = value; } }
	}
	#endregion
	#region QueryWorkTimeEventArgs
	public delegate void QueryWorkTimeEventHandler(object sender, QueryWorkTimeEventArgs e);
	public class QueryWorkTimeEventArgs : EventArgs {
		#region Fields
		readonly TimeInterval interval;
		readonly Resource resource;
		readonly TimeOfDayIntervalCollection workTimes;
		#endregion
		public QueryWorkTimeEventArgs(TimeInterval interval, Resource resource) {
			Guard.ArgumentNotNull(interval, "interval");
			Guard.ArgumentNotNull(resource, "resource");
			this.interval = interval;
			this.resource = resource;
			this.workTimes = new TimeOfDayIntervalCollection();
		}
		#region Properties
		public TimeInterval Interval { get { return interval; } }
		public Resource Resource { get { return resource; } }
		public TimeOfDayInterval WorkTime {
			get {
				if (workTimes.Count <= 0) {
					workTimes.Add(TimeOfDayInterval.CreateEmpty());
					return TimeOfDayInterval.Empty;
				} 
				return workTimes[0];
			}
			set {
				workTimes.Clear();
				if (value != null)
					workTimes.Add(value);
			}
		}
		public TimeOfDayIntervalCollection WorkTimes { get { return workTimes; } }
		#endregion
	}
	#endregion
	public delegate void QueryResourceColorSchemaEventHandler(object sender, QueryResourceColorSchemaEventArgs e);
	public class QueryResourceColorSchemaEventArgs : EventArgs {
		readonly Resource resource;
		readonly int resourceColorIndex;
		SchedulerColorSchemaBase resourceColorSchema;
		public QueryResourceColorSchemaEventArgs(Resource resource, int resourceColorIndex) {
			Guard.ArgumentNotNull(resource, "resource");
			this.resource = resource;
			this.resourceColorIndex = resourceColorIndex;
		}
		public Resource Resource { get { return resource; } }
		public int ResourceColorIndex { get { return resourceColorIndex; } }
		public SchedulerColorSchemaBase ResourceColorSchema { get { return resourceColorSchema; } set { resourceColorSchema = value; } }
	}
	#region MoreButtonClickedEventHandler
	public delegate void MoreButtonClickedEventHandler(object sender, MoreButtonClickedEventArgs e);
	public class MoreButtonClickedEventArgs : EventArgs {
		#region Fields
		TimeInterval interval;
		Resource resource;
		DateTime targetViewStart;
		bool handled;
		#endregion
		public MoreButtonClickedEventArgs(DateTime targetViewStart, TimeInterval interval, Resource resource) {
			if (interval == null)
				Exceptions.ThrowArgumentNullException("interval");
			if (resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			this.interval = interval;
			this.resource = resource;
			this.targetViewStart = targetViewStart;
		}
		#region Properties
		public TimeInterval Interval { get { return interval; } }
		public Resource Resource { get { return resource; } }
		public bool Handled { set { handled = value; } get { return handled; } }
		public DateTime TargetViewStart { get { return targetViewStart; } set { targetViewStart = value; } }
		#endregion
	}
	#endregion
	#region AppointmentEventHandler
	public delegate void AppointmentEventHandler(object sender, AppointmentEventArgs e);
	#endregion
	#region AppointmentEventArgs
	public class AppointmentEventArgs : EventArgs {
		Appointment apt;
		public AppointmentEventArgs(Appointment apt) {
			if (apt == null)
				Exceptions.ThrowArgumentException("apt", apt);
			this.apt = apt;
		}
		protected AppointmentEventArgs() {
		}
		public Appointment Appointment { get { return apt; } }
	}
	#endregion
	#region AppointmentCancelEventArgs
	public class AppointmentCancelEventArgs : AppointmentEventArgs {
		bool cancel;
		public AppointmentCancelEventArgs() {
		}
		public AppointmentCancelEventArgs(Appointment apt)
			: base(apt) {
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	#endregion
	#region AppointmentOperationEventHandler
	public delegate void AppointmentOperationEventHandler(object sender, AppointmentOperationEventArgs e);
	#endregion
	#region AppointmentOperationEventArgs
	public class AppointmentOperationEventArgs : AppointmentEventArgs {
		#region Fields
		bool allow = true;
		#endregion
		public AppointmentOperationEventArgs(Appointment apt)
			: base(apt) {
		}
		protected AppointmentOperationEventArgs() {
		}
		#region Properties
		public bool Allow { get { return allow; } set { allow = value; } }
		public virtual bool Recurring { get { return Appointment.IsRecurring; } }
		#endregion
	}
	#endregion
	#region AppointmentDragEventHandler
	public delegate void AppointmentDragEventHandler(object sender, AppointmentDragEventArgs e);
	#endregion
	#region AppointmentDragEventArgs
	public class AppointmentDragEventArgs : EventArgs {
		#region Fields
		bool allow = true;
		bool forceUpdateFromStorage = false;
		Appointment sourceAppointment;
		Appointment editedAppointment;
		TimeInterval hitInterval;
		Resource hitResource;
		ResourceIdCollection newAppointmentResourceIds;
		bool copyEffect;
		#endregion
		public AppointmentDragEventArgs(Appointment sourceAppointment, Appointment editedAppointment, TimeInterval hitInterval, Resource hitResource) {
			if (sourceAppointment == null)
				Exceptions.ThrowArgumentException("sourceAppointment", sourceAppointment);
			if (editedAppointment == null)
				Exceptions.ThrowArgumentException("editedAppointment", editedAppointment);
			if (hitInterval == null)
				Exceptions.ThrowArgumentException("hitInterval", hitInterval);
			if (hitResource == null)
				Exceptions.ThrowArgumentException("hitResource", hitResource);
			this.sourceAppointment = sourceAppointment;
			this.editedAppointment = editedAppointment;
			this.hitInterval = hitInterval;
			this.hitResource = hitResource;
		}
		#region Properties
		public bool Allow { get { return allow; } set { allow = value; } }
		public bool ForceUpdateFromStorage { get { return forceUpdateFromStorage; } set { forceUpdateFromStorage = value; } }
		public Appointment SourceAppointment { get { return sourceAppointment; } }
		public Appointment EditedAppointment { get { return editedAppointment; } }
		public TimeInterval HitInterval { get { return hitInterval; } }
		public Resource HitResource { get { return hitResource; } }
		public ResourceIdCollection NewAppointmentResourceIds { get { return newAppointmentResourceIds; } set { newAppointmentResourceIds = value; } }
		public bool CopyEffect { get { return this.copyEffect; } internal set { this.copyEffect = value; } }
		#endregion
	}
	#endregion
	#region AppointmentResizeEventHandler
	public delegate void AppointmentResizeEventHandler(object sender, AppointmentResizeEventArgs e);
	#endregion
	#region AppointmentDragEventArgs
	public enum ResizedSide {
		AtStartTime,
		AtEndTime
	}
	public class AppointmentResizeEventArgs : EventArgs {
		#region Fields
		ResizedSide resizedSide;
		bool handled;
		bool allow = true;
		Appointment sourceAppointment;
		Appointment editedAppointment;
		TimeInterval hitInterval;
		Resource hitResource;
		#endregion
		public AppointmentResizeEventArgs(Appointment sourceAppointment, Appointment editedAppointment, TimeInterval hitInterval, Resource hitResource, ResizedSide resizedSide) {
			if (sourceAppointment == null)
				Exceptions.ThrowArgumentException("sourceAppointment", sourceAppointment);
			if (editedAppointment == null)
				Exceptions.ThrowArgumentException("editedAppointment", editedAppointment);
			if (hitInterval == null)
				Exceptions.ThrowArgumentException("hitInterval", hitInterval);
			if (hitResource == null)
				Exceptions.ThrowArgumentException("hitResource", hitResource);
			this.sourceAppointment = sourceAppointment;
			this.editedAppointment = editedAppointment;
			this.hitInterval = hitInterval;
			this.hitResource = hitResource;
			this.resizedSide = resizedSide;
		}
		#region Properties
		public ResizedSide ResizedSide { get { return resizedSide; } }
		public bool Handled { get { return handled; } set { handled = value; } }
		public bool Allow { get { return allow; } set { allow = value; } }
		public Appointment SourceAppointment { get { return sourceAppointment; } }
		public Appointment EditedAppointment { get { return editedAppointment; } }
		public TimeInterval HitInterval { get { return hitInterval; } }
		public Resource HitResource { get { return hitResource; } }
		#endregion
		internal void ResetHandledState() {
			this.allow = true;
			this.handled = false;
		}
	}
	#endregion
	#region AppointmentSplittedEventHandler
	public delegate void AppointmentSplittedEventHandler(object sender, AppointmentSplittedEventArgs e);
	#endregion
	#region AppointmentSplittedEventArgs
	public class AppointmentSplittedEventArgs : EventArgs {
		#region Fields
		Appointment master;
		Appointment copy;
		#endregion
		public AppointmentSplittedEventArgs(Appointment master, Appointment copy) {
			if (master == null)
				Exceptions.ThrowArgumentException("master", master);
			if (copy == null)
				Exceptions.ThrowArgumentException("copy", copy);
			this.master = master;
			this.copy = copy;
		}
		#region Properties
		public Appointment Master { get { return master; } }
		public Appointment Copy { get { return copy; } }
		#endregion
	}
	#endregion
	#region AppointmentConflictEventHandler
	public delegate void AppointmentConflictEventHandler(object sender, AppointmentConflictEventArgs e);
	#endregion
	#region AppointmentConflictEventArgs
	public class AppointmentConflictEventArgs : AppointmentEventArgs {
		#region Fields
		Appointment clone;
		TimeInterval interval = TimeInterval.Empty;
		AppointmentBaseCollection conflicts;
		#endregion
		public AppointmentConflictEventArgs(Appointment apt, Appointment clone, AppointmentBaseCollection conflicts)
			: base(apt) {
			if (clone == null)
				Exceptions.ThrowArgumentException("clone", clone);
			if (conflicts == null)
				Exceptions.ThrowArgumentException("conflicts", conflicts);
			this.clone = clone;
			this.conflicts = conflicts;
		}
		#region Properties
		public Appointment AppointmentClone { get { return clone; } }
		public TimeInterval Interval { get { return interval; } }
		public AppointmentBaseCollection Conflicts { get { return conflicts; } }
		#endregion
		internal void SetInterval(TimeInterval val) {
			interval = val.Clone();
		}
		public void RemoveConflictsWithDifferentResource(AppointmentBaseCollection conflicts, object resourceId) {
			for (int i = conflicts.Count - 1; i >= 0; i--)
				if (!ResourceBase.InternalMatchIdToResourceIdCollection(conflicts[i].ResourceIds, resourceId))
					conflicts.RemoveAt(i);
		}
	}
	#endregion
	#region AppointmentDisplayTextEventHandler
	public delegate void AppointmentDisplayTextEventHandler(object sender, AppointmentDisplayTextEventArgs e);
	#endregion
	#region AppointmentDisplayTextEventArgs
	public class AppointmentDisplayTextEventArgs : EventArgs {
		#region Fields
		string text;
		string description;
		IAppointmentViewInfo viewInfo;
		#endregion
		public AppointmentDisplayTextEventArgs(IAppointmentViewInfo viewInfo, string text, string description)
			: base() {
			this.text = text;
			this.description = description;
			this.viewInfo = viewInfo;
			Guard.ArgumentNotNull(viewInfo, "viewInfo");
		}
		#region Properties
		public string Text { get { return text; } set { text = value; } }
		public string Description { get { return description; } set { description = value; } }
		public Appointment Appointment { get { return ViewInfo.Appointment; } }
		public IAppointmentViewInfo ViewInfo { get { return viewInfo; } }
		#endregion
	}
	#endregion
	#region ExchangeAppointmentEventHandler
	public delegate void ExchangeAppointmentEventHandler(object sender, ExchangeAppointmentEventArgs e);
	#endregion
	#region ExchangeAppointmentEventArgs
	public class ExchangeAppointmentEventArgs : AppointmentEventArgs {
		object id;
		public ExchangeAppointmentEventArgs(Appointment apt)
			: base(apt) {
		}
		public object Id { get { return id; } set { id = value; } }
	}
	#endregion
	#region ExchangeExceptionEventHandler
	public delegate void ExchangeExceptionEventHandler(object sender, ExchangeExceptionEventArgs e);
	#endregion
	#region ExchangeExceptionEventArgs
	public class ExchangeExceptionEventArgs : EventArgs {
		bool handled;
		Exception originalException;
		public ExchangeExceptionEventArgs(Exception originalException) {
			this.originalException = originalException;
		}
		public Exception OriginalException { get { return originalException; } }
		public bool Handled { get { return handled; } set { handled = value; } }
	}
	#endregion
	#region AppointmentImportingEventHandler
	public delegate void AppointmentImportingEventHandler(object sender, AppointmentImportingEventArgs e);
	#endregion
	#region AppointmentImportingEventArgs
	public class AppointmentImportingEventArgs : AppointmentCancelEventArgs {
		public AppointmentImportingEventArgs(Appointment apt)
			: base(apt) {
		}
	}
	#endregion
	#region AppointmentImportedEventHandler
	public delegate void AppointmentImportedEventHandler(object sender, AppointmentImportedEventArgs e);
	#endregion
	#region AppointmentImportedEventArgs
	public class AppointmentImportedEventArgs : AppointmentEventArgs {
		public AppointmentImportedEventArgs(Appointment apt)
			: base(apt) {
		}
	}
	#endregion
	#region AppointmentExportingEventHandler
	public delegate void AppointmentExportingEventHandler(object sender, AppointmentExportingEventArgs e);
	#endregion
	#region AppointmentExportingEventArgs
	public class AppointmentExportingEventArgs : AppointmentCancelEventArgs {
		public AppointmentExportingEventArgs(Appointment apt)
			: base(apt) {
		}
	}
	#endregion
	#region AppointmentExportedEventHandler
	public delegate void AppointmentExportedEventHandler(object sender, AppointmentExportedEventArgs e);
	#endregion
	#region AppointmentExportedEventArgs
	public class AppointmentExportedEventArgs : AppointmentEventArgs {
		public AppointmentExportedEventArgs(Appointment apt)
			: base(apt) {
		}
	}
	#endregion
	#region SynchronizeOperation
	public enum SynchronizeOperation { Create = 0, Replace = 1, Delete = 2 }
	#endregion
	#region AppointmentSynchronizingEventHandler
	public delegate void AppointmentSynchronizingEventHandler(object sender, AppointmentSynchronizingEventArgs e);
	#endregion
	#region AppointmentSynchronizingEventArgs
	public class AppointmentSynchronizingEventArgs : AppointmentCancelEventArgs {
		SynchronizeOperation operation = SynchronizeOperation.Create;
		public AppointmentSynchronizingEventArgs() {
		}
		public AppointmentSynchronizingEventArgs(Appointment apt)
			: base(apt) {
		}
		public SynchronizeOperation Operation { get { return operation; } set { operation = value; } }
	}
	#endregion
	#region AppointmentSynchronizedEventHandler
	public delegate void AppointmentSynchronizedEventHandler(object sender, AppointmentSynchronizedEventArgs e);
	#endregion
	#region AppointmentSynchronizedEventArgs
	public class AppointmentSynchronizedEventArgs : AppointmentEventArgs {
		public AppointmentSynchronizedEventArgs() {
		}
		public AppointmentSynchronizedEventArgs(Appointment apt)
			: base(apt) {
		}
	}
	#endregion
	#region DateNavigatorQueryActiveViewTypeHandler
	public delegate void DateNavigatorQueryActiveViewTypeHandler(object sender, DateNavigatorQueryActiveViewTypeEventArgs e);
	#endregion
	#region DateNavigatorQueryActiveViewTypeEventArgs
	public class DateNavigatorQueryActiveViewTypeEventArgs : EventArgs {
		SchedulerViewType oldViewType;
		SchedulerViewType newViewType;
		DayIntervalCollection selectedDays;
		public DateNavigatorQueryActiveViewTypeEventArgs(SchedulerViewType oldViewType, SchedulerViewType newViewType, DayIntervalCollection selectedDays) {
			this.oldViewType = oldViewType;
			this.newViewType = newViewType;
			this.selectedDays = selectedDays;
		}
		public SchedulerViewType OldViewType { get { return oldViewType; } }
		public SchedulerViewType NewViewType { get { return newViewType; } set { newViewType = value; } }
		public DayIntervalCollection SelectedDays { get { return selectedDays; } }
	}
	#endregion
	#region SchedulerUnhandledExceptionEventHandler
	[ComVisible(true)]
	public delegate void SchedulerUnhandledExceptionEventHandler(object sender, SchedulerUnhandledExceptionEventArgs e);
	#endregion
	#region SchedulerUnhandledExceptionEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SchedulerUnhandledExceptionEventArgs : EventArgs {
		#region Fields
		Exception exception;
		bool handled;
		#endregion
		public SchedulerUnhandledExceptionEventArgs(Exception e) {
			Guard.ArgumentNotNull(e, "e");
			this.exception = e;
		}
		#region Properties
		public Exception Exception { get { return exception; } }
		public bool Handled { get { return handled; } set { handled = value; } }
		#endregion
	}
	#endregion
	#region RangeControlAdjustEventArgs
	public class RangeControlAdjustEventArgs {
		public RangeControlAdjustEventArgs() {
			Scales = new TimeScaleCollection();
		}
		public DateTime RangeMinimum { get; set; }
		public DateTime RangeMaximum { get; set; }
		public TimeScaleCollection Scales { get; private set; }
	}
	#endregion
	#region RangeControlAdjustEventHandler
	public delegate void RangeControlAdjustEventHandler(object sender, RangeControlAdjustEventArgs e);
	#endregion
	#region VisibleResourcesChangedEventArgs
	public class VisibleResourcesChangedEventArgs : EventArgs {
		ResourceBaseCollection innerResourceCollection;
		public VisibleResourcesChangedEventArgs(ResourceBaseCollection innerResourceCollection) {
			this.innerResourceCollection = innerResourceCollection;
		}
		public int OldFirstVisibleResourceIndex { get; internal set; }
		public int OldResourcePerPage { get; internal set; }
		public int NewFirstVisibleResourceIndex { get; internal set; }
		public int NewResourcePerPage { get; internal set; }
		public ResourceBaseCollection GetVisibleResources() {
			ResourceBaseCollection result = new ResourceBaseCollection();
			result.AddRange(this.innerResourceCollection);
			return result;
		}
	}
	#endregion
	#region VisibleResourcesChangedEventHandler
	public delegate void VisibleResourcesChangedEventHandler(object sender, VisibleResourcesChangedEventArgs args);
	#endregion
	#region CancellablePropertyChangingEventHandler
	public delegate void CancellablePropertyChangingEventHandler(object sender, CancellablePropertyChangingEventArgs e);
	#endregion
	#region CancellablePropertyChangingEventArgs
	public class CancellablePropertyChangingEventArgs : CancelEventArgs {
		#region Fields
		readonly string propertyName;
		readonly object oldValue;
		readonly object newValue;
		#endregion
		public CancellablePropertyChangingEventArgs(string propertyName, object oldValue, object newValue) {
			this.propertyName = propertyName;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		#region Properties
		public string PropertyName { get { return propertyName; } }
		public object OldValue { get { return oldValue; } }
		public object NewValue { get { return newValue; } }
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	public enum AppointmentsTransactionType { Unknown, Insert, Delete, Update, ChildCreate, ChildDelete, RestoreOccurrence, Rollback }; 
	public delegate void AppointmentsTransactionEventHandler(object sender, AppointmentsTransactionEventArgs e);
	public class AppointmentsTransactionEventArgs : EventArgs {
		readonly AppointmentsTransactionType transactionType;
		readonly IList<Appointment> appointments;
		public AppointmentsTransactionEventArgs(IList<Appointment> appointments, AppointmentsTransactionType transactionType) {
			this.transactionType = transactionType;
			this.appointments = new List<Appointment>(appointments);
		}
		public IList<Appointment> Appointments { get { return appointments; } }
		public AppointmentsTransactionType TransactionType { get { return transactionType; } }
	}
	#region SchedulerViewUIChangedEventHandler
	public delegate void SchedulerViewUIChangedEventHandler(object sender, SchedulerViewUIChangedEventArgs e);
	#endregion
	#region SchedulerViewUIChangedEventArgs
	public class SchedulerViewUIChangedEventArgs : ChangeEventArgs {
		SchedulerViewType viewType;
		public SchedulerViewUIChangedEventArgs(SchedulerViewType viewType, string propertyName, object oldValue, object newValue)
			: base(propertyName, oldValue, newValue) {
			this.viewType = viewType;
		}
		public SchedulerViewType ViewType { get { return viewType; } }
	}
	#endregion
	#region PersistentObjectStorageReloadEventHandler
	public delegate void PersistentObjectStorageReloadEventHandler(object sender, PersistentObjectStorageReloadEventArgs e);
	#endregion
	#region PersistentObjectStorageReloadEventArgs
	public class PersistentObjectStorageReloadEventArgs : EventArgs {
		bool keepNonPersistentInformation;
		public PersistentObjectStorageReloadEventArgs(bool keepNonPersistentInformation) {
			this.keepNonPersistentInformation = keepNonPersistentInformation;
		}
		public bool KeepNonPersistentInformation { get { return keepNonPersistentInformation; } }
	}
	#endregion
	#region PersistentObjectStateChangingEventHandler
	public delegate void PersistentObjectStateChangingEventHandler(object sender, PersistentObjectStateChangingEventArgs e);
	#endregion
	#region PersistentObjectStateChangingEventArgs
	public class PersistentObjectStateChangingEventArgs : PersistentObjectStateChangedEventArgs {
		bool cancel;
		object newValue;
		object oldValue;
		string propertyName = String.Empty;
		public PersistentObjectStateChangingEventArgs(IPersistentObject obj, PersistentObjectState state)
			: base(obj, state) {
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		public object NewValue { get { return newValue; } set { newValue = value; } }
		public object OldValue { get { return oldValue; } set { oldValue = value; } }
		public string PropertyName { get { return propertyName; } set { propertyName = value; } }
	}
	#endregion
	#region PersistentObjectStateChangedEventHandler
	public delegate void PersistentObjectStateChangedEventHandler(object sender, PersistentObjectStateChangedEventArgs e);
	#endregion
	#region PersistentObjectStateChangedEventArgs
	public class PersistentObjectStateChangedEventArgs : PersistentObjectEventArgs {
		PersistentObjectState state;
		public PersistentObjectStateChangedEventArgs(IPersistentObject obj, PersistentObjectState state)
			: base(obj) {
			this.state = state;
		}
		public PersistentObjectState State { get { return state; } }
	}
	#endregion
	#region ReminderContentChangingEventHandler
	public delegate void ReminderContentChangingEventHandler(object sender, ReminderContentChangingEventArgs e);
	#endregion
	#region ReminderContentChangingEventArgs
	public class ReminderContentChangingEventArgs : EventArgs {
		bool cancel;
		object newValue;
		object oldValue;
		string propertyName = String.Empty;
		public ReminderContentChangingEventArgs() {
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		public object NewValue { get { return newValue; } set { newValue = value; } }
		public object OldValue { get { return oldValue; } set { oldValue = value; } }
		public string PropertyName { get { return propertyName; } set { propertyName = value; } }
	}
	#endregion
	#region ReminderContentChangedEventHandler
	public delegate void ReminderContentChangedEventHandler(object sender, EventArgs e);
	#endregion
	#region QueryPersistentObjectStorageEventHandler<T>
	public delegate void QueryPersistentObjectStorageEventHandler<T>(object sender, QueryPersistentObjectStorageEventArgs<T> e) where T : IPersistentObject;
	#endregion
	#region QueryPersistentObjectStorageEventArgs<T>
	public class QueryPersistentObjectStorageEventArgs<T> : EventArgs where T : IPersistentObject {
		IPersistentObjectStorage<T> objectStorage;
		public IPersistentObjectStorage<T> ObjectStorage { get { return objectStorage; } set { objectStorage = value; } }
	}
	#endregion
	#region SchedulerControlStateChangedEventHandler
	public delegate void SchedulerControlStateChangedEventHandler(object sender, SchedulerControlStateChangedEventArgs e);
	#endregion
	#region SchedulerControlStateChangedEventArgs
	public class SchedulerControlStateChangedEventArgs : EventArgs {
		SchedulerControlChangeType change;
		bool ignoreApplyChanges;
		public SchedulerControlStateChangedEventArgs(SchedulerControlChangeType change) {
			this.change = change;
		}
		public SchedulerControlStateChangedEventArgs(SchedulerControlChangeType change, object oldValue, object newValue) {
			this.change = change;
			OldValue = oldValue;
			NewValue = newValue;
		}
		public object OldValue { get; set; }
		public object NewValue { get; set; }
		public SchedulerControlChangeType Change { get { return change; } }
		public bool IgnoreApplyChanges { get { return ignoreApplyChanges; } set { ignoreApplyChanges = value; } }
	}
	#endregion
	#region SchedulerControlLowLevelStateChangedEventHandler
	public delegate void SchedulerControlLowLevelStateChangedEventHandler(object sender, SchedulerControlLowLevelStateChangedEventArgs e);
	#endregion
	#region SchedulerControlLowLevelStateChangedEventArgs
	public class SchedulerControlLowLevelStateChangedEventArgs : EventArgs {
		SchedulerControlChangeType changeType;
		ChangeActions changeActions;
		public SchedulerControlLowLevelStateChangedEventArgs(SchedulerControlChangeType changeType, ChangeActions actions) {
			this.changeType = changeType;
			this.changeActions = actions;
		}
		public ChangeActions Actions { get { return changeActions; } }
		public SchedulerControlChangeType ChangeType { get { return changeType; } }
	}
	#endregion
	#region NewAppointmentOperationEventArgs
	public class NewAppointmentOperationEventArgs : AppointmentOperationEventArgs {
		bool recurring;
		public NewAppointmentOperationEventArgs() {
		}
		public override bool Recurring { get { return recurring; } }
		public void SetRecurring(bool recurring) {
			this.recurring = recurring;
		}
	}
	#endregion
	#region InnerActiveViewChangingEventHandler
	public delegate void InnerActiveViewChangingEventHandler(object sender, InnerActiveViewChangingEventArgs e);
	#endregion
	#region InnerActiveViewChangingEventArgs
	public class InnerActiveViewChangingEventArgs : EventArgs {
		#region Fields
		bool cancel;
		InnerSchedulerViewBase oldView;
		InnerSchedulerViewBase newView;
		#endregion
		public InnerActiveViewChangingEventArgs(InnerSchedulerViewBase oldView, InnerSchedulerViewBase newView) {
			if (oldView == null)
				Exceptions.ThrowArgumentException("oldView", oldView);
			if (newView == null)
				Exceptions.ThrowArgumentException("newView", newView);
			this.oldView = oldView;
			this.newView = newView;
		}
		#region Properties
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		public InnerSchedulerViewBase OldView { get { return oldView; } }
		public InnerSchedulerViewBase NewView { get { return newView; } }
		#endregion
	}
	#endregion
	#region TimeSpanStringConvertEventHandler
	public delegate void TimeSpanStringConvertEventHandler(object sender, TimeSpanStringConvertEventArgs e);
	#endregion
	#region TimeSpanStringConvertEventArgs
	public class TimeSpanStringConvertEventArgs : EventArgs {
		string stringValue = String.Empty;
		TimeSpan timeSpanValue = TimeSpan.MinValue;
		bool handled;
		public string StringValue { get { return stringValue; } set { stringValue = value; } }
		public TimeSpan TimeSpanValue { get { return timeSpanValue; } set { timeSpanValue = value; } }
		public bool Handled { get { return handled; } set { handled = value; } }
	}
	#endregion
	#region AfterApplyChangesEventHandler
	public delegate void AfterApplyChangesEventHandler(object sender, AfterApplyChangesEventArgs e);
	#endregion
	#region AfterApplyChangesEventArgs
	public class AfterApplyChangesEventArgs : EventArgs {
		List<SchedulerControlChangeType> changeTypes;
		ChangeActions actions;
		public AfterApplyChangesEventArgs(List<SchedulerControlChangeType> changeTypes, ChangeActions actions) {
			if (changeTypes == null)
				Exceptions.ThrowArgumentNullException("changeTypes");
			this.changeTypes = changeTypes;
			this.actions = actions;
		}
		public ChangeActions Actions { get { return actions; } }
		public List<SchedulerControlChangeType> ChangeTypes { get { return changeTypes; } }
	}
	#endregion
	#region QueryClientTimeZoneIdEventHandler
	public delegate void QueryClientTimeZoneIdEventHandler(object sender, QueryClientTimeZoneIdEventArgs e);
	#endregion
	#region QueryClientTimeZoneIdEventArgs
	public class QueryClientTimeZoneIdEventArgs : EventArgs {
		string timeZoneId = XtraScheduler.TimeZoneId.Custom;
		public QueryClientTimeZoneIdEventArgs() {
		}
		public string TimeZoneId { get { return timeZoneId; } set { timeZoneId = value; } }
	}
	#endregion       
}
