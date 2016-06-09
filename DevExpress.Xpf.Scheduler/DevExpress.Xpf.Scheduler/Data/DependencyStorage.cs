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
using System.IO;
using System.Linq;
using System.Windows;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.XtraScheduler;
using System.Collections.Generic;
using System.Collections.Specialized;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Core;
#if !SL
using DevExpress.Utils.Design.DataAccess;
using System.Windows.Media;
using System.Collections;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal;
#endif
namespace DevExpress.Xpf.Scheduler {
	public class SchedulerStorage : SchedulerFrameworkElement
#if SL
, ISupportInitialize
#endif
 {
		readonly ISchedulerStorage innerStorage;
		public SchedulerStorage() {
			this.innerStorage = CreateInnerStorage();
			CreatePropertySyncManager();
			SetCurrentValue(AppointmentStorageProperty, CreateAppointmentStorage());
			AppointmentStorage.InitializeProperties();
			SetCurrentValue(ResourceStorageProperty, CreateResourceStorage());
			ResourceStorage.InitializeProperties();
		}
		#region Events
		#region AppointmentChanging
		public event PersistentObjectCancelEventHandler AppointmentChanging {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentChanging += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentChanging -= value;
			}
		}
		#endregion
		#region AppointmentCollectionAutoReloading
		public event CancelListChangedEventHandler AppointmentCollectionAutoReloading {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentCollectionAutoReloading += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentCollectionAutoReloading -= value;
			}
		}
		#endregion
		#region AppointmentCollectionCleared
		public event EventHandler AppointmentCollectionCleared {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentCollectionCleared += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentCollectionCleared -= value;
			}
		}
		#endregion
		#region AppointmentCollectionLoaded
		public event EventHandler AppointmentCollectionLoaded {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentCollectionLoaded += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentCollectionLoaded -= value;
			}
		}
		#endregion
#if SL
		#region AppointmentCollectionModified
		public event EventHandler AppointmentCollectionModified {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentCollectionModified += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentCollectionModified -= value;
			}
		}
		#endregion
#endif
		#region AppointmentDeleting
		public event PersistentObjectCancelEventHandler AppointmentDeleting {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentDeleting += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentDeleting -= value;
			}
		}
		#endregion
		#region AppointmentInserting
		public event PersistentObjectCancelEventHandler AppointmentInserting {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentInserting += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentInserting -= value;
			}
		}
		#endregion
		#region AppointmentsChanged
		public event PersistentObjectsEventHandler AppointmentsChanged {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentsChanged += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentsChanged -= value;
			}
		}
		#endregion
		#region AppointmentsDeleted
		public event PersistentObjectsEventHandler AppointmentsDeleted {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentsDeleted += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentsDeleted -= value;
			}
		}
		#endregion
		#region AppointmentsInserted
		public event PersistentObjectsEventHandler AppointmentsInserted {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentsInserted += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentsInserted -= value;
			}
		}
		#endregion
		#region FetchAppointments
		public event FetchAppointmentsEventHandler FetchAppointments {
			add {
				if (innerStorage != null)
					innerStorage.FetchAppointments += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.FetchAppointments -= value;
			}
		}
		#endregion
		#region FilterAppointment
		public event PersistentObjectCancelEventHandler FilterAppointment {
			add {
				if (innerStorage != null)
					innerStorage.FilterAppointment += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.FilterAppointment -= value;
			}
		}
		#endregion
		#region FilterResource
		public event PersistentObjectCancelEventHandler FilterResource {
			add {
				if (innerStorage != null)
					innerStorage.FilterResource += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.FilterResource -= value;
			}
		}
		#endregion
		public event PersistentObjectCancelEventHandler FilterDependency {
			add {
				if (innerStorage != null)
					innerStorage.FilterDependency += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.FilterDependency -= value;
			}
		}
		public event EventHandler<ReminderCancelEventArgs> FilterReminderAlert {
			add {
				if (innerStorage != null)
					innerStorage.FilterReminderAlert += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.FilterReminderAlert -= value;
			}
		}
		public event ReminderEventHandler ReminderAlert {
			add {
				if (innerStorage != null)
					innerStorage.ReminderAlert += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.ReminderAlert -= value;
			}
		}
		#region ResourceChanging
		public event PersistentObjectCancelEventHandler ResourceChanging {
			add {
				if (innerStorage != null)
					innerStorage.ResourceChanging += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.ResourceChanging -= value;
			}
		}
		#endregion
		#region ResourceCollectionAutoReloading
		public event CancelListChangedEventHandler ResourceCollectionAutoReloading {
			add {
				if (innerStorage != null)
					innerStorage.ResourceCollectionAutoReloading += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.ResourceCollectionAutoReloading -= value;
			}
		}
		#endregion
		#region ResourceCollectionCleared
		public event EventHandler ResourceCollectionCleared {
			add {
				if (innerStorage != null)
					innerStorage.ResourceCollectionCleared += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.ResourceCollectionCleared -= value;
			}
		}
		#endregion
		#region ResourceCollectionLoaded
		public event EventHandler ResourceCollectionLoaded {
			add {
				if (innerStorage != null)
					innerStorage.ResourceCollectionLoaded += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.ResourceCollectionLoaded -= value;
			}
		}
		#endregion
		#region ResourceDeleting
		public event PersistentObjectCancelEventHandler ResourceDeleting {
			add {
				if (innerStorage != null)
					innerStorage.ResourceDeleting += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.ResourceDeleting -= value;
			}
		}
		#endregion
		#region ResourceInserting
		public event PersistentObjectCancelEventHandler ResourceInserting {
			add {
				if (innerStorage != null)
					innerStorage.ResourceInserting += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.ResourceInserting -= value;
			}
		}
		#endregion
		#region ResourcesChanged
		public event PersistentObjectsEventHandler ResourcesChanged {
			add {
				if (innerStorage != null)
					innerStorage.ResourcesChanged += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.ResourcesChanged -= value;
			}
		}
		#endregion
		#region ResourcesDeleted
		public event PersistentObjectsEventHandler ResourcesDeleted {
			add {
				if (innerStorage != null)
					innerStorage.ResourcesDeleted += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.ResourcesDeleted -= value;
			}
		}
		#endregion
		#region ResourcesInserted
		public event PersistentObjectsEventHandler ResourcesInserted {
			add {
				if (innerStorage != null)
					innerStorage.ResourcesInserted += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.ResourcesInserted -= value;
			}
		}
		#endregion
		#region DependencyChanging
		public event PersistentObjectCancelEventHandler DependencyChanging {
			add {
				if (innerStorage != null)
					innerStorage.ResourceChanging += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.ResourceChanging -= value;
			}
		}
		#endregion
		#region AppointmentDependencyCollectionAutoReloading
		public event CancelListChangedEventHandler AppointmentDependencyCollectionAutoReloading {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentDependencyCollectionAutoReloading += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentDependencyCollectionAutoReloading -= value;
			}
		}
		#endregion
		#region AppointmentDependencyCollectionCleared
		public event EventHandler AppointmentDependencyCollectionCleared {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentDependencyCollectionCleared += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentDependencyCollectionCleared -= value;
			}
		}
		#endregion
		#region AppointmentDependencyCollectionLoaded
		public event EventHandler AppointmentDependencyCollectionLoaded {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentDependencyCollectionLoaded += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentDependencyCollectionLoaded -= value;
			}
		}
		#endregion
		#region AppointmentDependencyDeleting
		public event PersistentObjectCancelEventHandler AppointmentDependencyDeleting {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentDependencyDeleting += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentDependencyDeleting -= value;
			}
		}
		#endregion
		#region AppointmentDependencyInserting
		public event PersistentObjectCancelEventHandler AppointmentDependencyInserting {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentDependencyInserting += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentDependencyInserting -= value;
			}
		}
		#endregion
		#region AppointmentDependencyChanging
		public event PersistentObjectCancelEventHandler AppointmentDependencyChanging {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentDependencyChanging += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentDependencyChanging -= value;
			}
		}
		#endregion
		#region AppointmentDependenciesChanged
		public event PersistentObjectsEventHandler AppointmentDependenciesChanged {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentDependenciesChanged += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentDependenciesChanged -= value;
			}
		}
		#endregion
		#region AppointmentDependenciesDeleted
		public event PersistentObjectsEventHandler AppointmentDependenciesDeleted {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentDependenciesDeleted += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentDependenciesDeleted -= value;
			}
		}
		#endregion
		#region AppointmentDependenciesInserted
		public event PersistentObjectsEventHandler AppointmentDependenciesInserted {
			add {
				if (innerStorage != null)
					innerStorage.AppointmentDependenciesInserted += value;
			}
			remove {
				if (innerStorage != null)
					innerStorage.AppointmentDependenciesInserted -= value;
			}
		}
		#endregion
		#endregion
		#region AppointmentStorageProperty
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerStorageAppointmentStorage")]
#endif
		public AppointmentStorage AppointmentStorage {
			get { return (AppointmentStorage)GetValue(AppointmentStorageProperty); }
			set { SetValue(AppointmentStorageProperty, value); }
		}
		public static readonly DependencyProperty AppointmentStorageProperty = CreateAppointmentStorageProperty();
		static DependencyProperty CreateAppointmentStorageProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerStorage, AppointmentStorage>("AppointmentStorage", null, (d, e) => d.OnAppointmentStorageChanged(e.OldValue, e.NewValue), null);
		}
		private void OnAppointmentStorageChanged(AppointmentStorage oldValue, AppointmentStorage newValue) {
			if (newValue == null) {
				newValue = CreateAppointmentStorage();
			}
			ReplaceAppointmentDataStorage(newValue);
			SetupAppointmentsInnerDataContextBinding();
		}
		protected virtual void ReplaceAppointmentDataStorage(AppointmentStorage newValue) {
			IAppointmentStorageBase value = newValue != null ? newValue.InnerAppointments : null;
			((IInternalSchedulerStorageBase)InnerStorage).SetAppointmentStorage(value);
		}
		#endregion
		#region ResourceStorageProperty
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerStorageResourceStorage")]
#endif
		public ResourceStorage ResourceStorage {
			get { return (ResourceStorage)GetValue(ResourceStorageProperty); }
			set { SetValue(ResourceStorageProperty, value); }
		}
		public static readonly DependencyProperty ResourceStorageProperty = CreateResourceStorageProperty();
		static DependencyProperty CreateResourceStorageProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerStorage, ResourceStorage>("ResourceStorage", null, (d, e) => d.OnResourceStorageChanged(e.OldValue, e.NewValue), null);
		}
		private void OnResourceStorageChanged(ResourceStorage oldValue, ResourceStorage newValue) {
			if (newValue == null)
				newValue = CreateResourceStorage();
			ReplaceResourceDataStorage(newValue);
			SetupResourcesInnerDataContextBinding();
		}
		protected virtual void ReplaceResourceDataStorage(ResourceStorage newValue) {
			ResourceDataStorage value = newValue != null ? newValue.InnerResources : null;
			((IInternalSchedulerStorageBase)InnerStorage).SetResourceStorage(value);
		}
		#endregion
		#region TimeZoneId
		public static readonly DependencyProperty TimeZoneIdProperty = DependencyProperty.Register("TimeZoneId", typeof(string), typeof(SchedulerStorage), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTimeZoneIdChanged)));
		[TypeConverter(typeof(DevExpress.XtraScheduler.Design.TimeZoneIdStringTypeConverter))]
		public string TimeZoneId {
			get { return (string)GetValue(TimeZoneIdProperty); }
			set { SetValue(TimeZoneIdProperty, value); }
		}
		static void OnTimeZoneIdChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			SchedulerStorage schedulerStorage = o as SchedulerStorage;
			if (schedulerStorage != null)
				schedulerStorage.OnTimeZoneIdChanged((string)e.OldValue, (string)e.NewValue);
		}
		protected virtual void OnTimeZoneIdChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(TimeZoneIdProperty, null, newValue);
		}
		#endregion
		#region EnableTimeZones
		public static readonly DependencyProperty EnableTimeZonesProperty = DependencyProperty.Register("EnableTimeZones", typeof(bool), typeof(SchedulerStorage), new UIPropertyMetadata(true, new PropertyChangedCallback(OnEnableTimeZonesChanged)));
		public bool EnableTimeZones {
			get { return (bool)GetValue(EnableTimeZonesProperty); }
			set { SetValue(EnableTimeZonesProperty, value); }
		}
		static void OnEnableTimeZonesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			SchedulerStorage schedulerStorage = o as SchedulerStorage;
			if (schedulerStorage != null)
				schedulerStorage.OnEnableTimeZonesChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		protected virtual void OnEnableTimeZonesChanged(bool oldValue, bool newValue) {
			PropertySyncManager.Update(EnableTimeZonesProperty, null, newValue);
		}
		#endregion
		#region RemindersCheckInterval
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerStorageRemindersCheckInterval")]
#endif
		public int RemindersCheckInterval {
			get { return (int)GetValue(RemindersCheckIntervalProperty); }
			set { SetValue(RemindersCheckIntervalProperty, value); }
		}
		public static readonly DependencyProperty RemindersCheckIntervalProperty = CreateRemindersCheckIntervalProperty();
		static DependencyProperty CreateRemindersCheckIntervalProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerStorage, int>("RemindersCheckInterval", ReminderEngineBase.DefaultCheckInterval, (d, e) => d.OnRemindersCheckIntervalChanged(e.OldValue, e.NewValue), null);
		}
		private void OnRemindersCheckIntervalChanged(int oldValue, int newValue) {
			PropertySyncManager.Update(RemindersCheckIntervalProperty, null, newValue);
		}
		#endregion
		public ISchedulerStorage InnerStorage { get { return innerStorage; } }
		#region PropertyManager
		DependencyPropertySyncManager propertySyncManager;
		protected internal DependencyPropertySyncManager PropertySyncManager { get { return propertySyncManager; } }
		protected virtual DependencyPropertySyncManager CreateDependencyPropertySyncManager() {
			return new SchedulerStoragePropertySyncManager(this);
		}
		#endregion
		#region Inner Storage facade methods
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerStorageEnableReminders")]
#endif
		public bool EnableReminders { get { return InnerStorage.EnableReminders; } set { InnerStorage.EnableReminders = value; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerStorageSupportsRecurrence")]
#endif
		public bool SupportsRecurrence { get { return InnerStorage.SupportsRecurrence; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerStorageSupportsReminders")]
#endif
		public bool SupportsReminders { get { return InnerStorage.SupportsReminders; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerStorageRemindersEnabled")]
#endif
		public bool RemindersEnabled { get { return InnerStorage.RemindersEnabled; } }
		public void TriggerAlerts() {
			((IInternalSchedulerStorageBase)InnerStorage).RemindersEngine.TriggerAlerts();
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerStorageUnboundMode")]
#endif
		public bool UnboundMode { get { return InnerStorage.UnboundMode; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerStorageResourceSharing")]
#endif
		public bool ResourceSharing { get { return InnerStorage.ResourceSharing; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerStorageIsUpdateLocked")]
#endif
		public bool IsUpdateLocked { get { return InnerStorage.IsUpdateLocked; } }
		public ISchedulerStorage GetCoreStorage() {
			return InnerStorage;
		}
		public void BeginUpdate() {
			InnerStorage.BeginUpdate();
		}
		public void CancelUpdate() {
			InnerStorage.CancelUpdate();
		}
		public void EndUpdate() {
			InnerStorage.EndUpdate();
		}
		public void RefreshData() {
			InnerStorage.RefreshData();
		}
		public void SetAppointmentFactory(IAppointmentFactory appointmentFactory) {
			InnerStorage.SetAppointmentFactory(appointmentFactory);
		}
		public void SetResourceFactory(IResourceFactory resourceFactory) {
			InnerStorage.SetResourceFactory(resourceFactory);
		}
		public void SetAppointmentDependencyFactory(IAppointmentDependencyFactory appointmentDependencyFactory) {
			InnerStorage.SetAppointmentDependencyFactory(appointmentDependencyFactory);
		}
		public object GetObjectRow(IPersistentObject obj) {
			return InnerStorage.GetObjectRow(obj);
		}
		public object GetObjectValue(IPersistentObject obj, string columnName) {
			return InnerStorage.GetObjectValue(obj, columnName);
		}
		public void SetObjectValue(IPersistentObject obj, string columnName, object val) {
			InnerStorage.SetObjectValue(obj, columnName, val);
		}
		public virtual AppointmentBaseCollection GetAppointments(TimeInterval interval) {
			return InnerStorage.GetAppointments(interval);
		}
		public virtual AppointmentBaseCollection GetAppointments(TimeIntervalCollection intervals) {
			return InnerStorage.GetAppointments(intervals);
		}
		public virtual AppointmentBaseCollection GetAppointments(DateTime start, DateTime end) {
			return InnerStorage.GetAppointments(start, end);
		}
		public virtual Color GetLabelColor(object labelId) {
			if (AppointmentStorage == null)
				return ColorExtension.Empty;
			return InnerStorage.GetLabelColor(labelId);
		}
		public override void BeginInit() {
			((ISupportInitialize)InnerStorage).BeginInit();
			base.BeginInit();
		}
		public override void EndInit() {
			base.EndInit();
			((ISupportInitialize)InnerStorage).EndInit();
			if (AppointmentStorage != null)
				AppointmentStorage.OnOwnerEndInit();
			if (ResourceStorage != null)
				ResourceStorage.OnOwnerEndInit();
			if (AppointmentStorage != null)
				AppointmentStorage.ReplaceInnerLabelsAndStatuses();
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			InitializeProperties();
		}
		internal ReminderEngine RemindersEngine { get { return ((IInternalSchedulerStorageBase)InnerStorage).RemindersEngine; } }
		#endregion
		protected virtual void CreatePropertySyncManager() {
			this.propertySyncManager = CreateDependencyPropertySyncManager();
			PropertySyncManager.Register();
		}
		protected internal virtual ResourceBaseCollection GetVisibleResources() {
			return ((IInternalSchedulerStorageBase)InnerStorage).GetVisibleResources(true);
		}
		protected virtual SchedulerDataStorage CreateInnerStorage() {
			return new SchedulerDataStorage();
		}
		protected virtual AppointmentStorage CreateAppointmentStorage() {
			return new AppointmentStorage(this);
		}
		protected virtual ResourceStorage CreateResourceStorage() {
			return new ResourceStorage(this);
		}
		protected internal virtual void InitializeProperties() {
			if (ResourceStorage == null) {
				SetCurrentValue(ResourceStorageProperty, CreateResourceStorage());
				ResourceStorage.InitializeProperties();
			}
			if (AppointmentStorage == null) {
				SetCurrentValue(AppointmentStorageProperty, CreateAppointmentStorage());
				AppointmentStorage.InitializeProperties();
			}
		}
		protected internal virtual void SetupResourcesInnerDataContextBinding() {
			InnerBindingHelper.SetupDataContextBinding(this, ResourceStorage);
		}
		protected internal virtual void SetupAppointmentsInnerDataContextBinding() {
			InnerBindingHelper.SetupDataContextBinding(this, AppointmentStorage);
		}
#if !SL
		public void ImportFromICalendar(string path) {
			InnerStorage.ImportFromICalendar(path);
		}
		public void ImportFromICalendar(Stream stream) {
			InnerStorage.ImportFromICalendar(stream);
		}
		public void ExportToICalendar(string path) {
			InnerStorage.ExportToICalendar(path);
		}
		public void ExportToICalendar(Stream stream) {
			InnerStorage.ExportToICalendar(stream);
		}
#endif
		protected internal IAppointmentLabel GetLabel(object labelId) {
			return AppointmentStorage.Labels.GetById(labelId);
		}
		protected internal IAppointmentStatus GetStatus(object statusId) {
			return AppointmentStorage.Statuses.GetById(statusId);
		}
		public virtual void SetAppointmentId(Appointment apt, object id) {
			if (apt == null)
				Exceptions.ThrowArgumentNullException("apt");
			apt.SetId(id);
			AppointmentStorage.Items.UpdateIdHash(apt);
		}
		public Appointment CreateAppointment(AppointmentType type) {
			return InnerStorage.CreateAppointment(type);
		}
		public Appointment CreateAppointment(AppointmentType type, DateTime start, DateTime end) {
			return InnerStorage.CreateAppointment(type, start, end);
		}
		public Appointment CreateAppointment(AppointmentType type, DateTime start, TimeSpan duration) {
			return InnerStorage.CreateAppointment(type, start, duration);
		}
		public Appointment CreateAppointment(AppointmentType type, DateTime start, DateTime end, string subject) {
			return InnerStorage.CreateAppointment(type, start, end, subject);
		}
		public Appointment CreateAppointment(AppointmentType type, DateTime start, TimeSpan duration, string subject) {
			return InnerStorage.CreateAppointment(type, start, duration, subject);
		}
		public Resource CreateResource(object resourceId) {
			return InnerStorage.CreateResource(resourceId);
		}
		public Resource CreateResource(object resourceId, string resourceCaption) {
			return InnerStorage.CreateResource(resourceId, resourceCaption);
		}
		public AppointmentDependency CreateAppointmentDependency(object parentId, object dependentId) {
			return InnerStorage.CreateAppointmentDependency(parentId, dependentId);
		}
		public AppointmentDependency CreateAppointmentDependency(object parentId, object dependentId, AppointmentDependencyType type) {
			return InnerStorage.CreateAppointmentDependency(parentId, dependentId, type);
		}
		public Reminder CreateReminder(Appointment appointment) {
			return InnerStorage.CreateReminder(appointment);
		}
	}
	#region PersistentObjectStorageBase
	public abstract class PersistentObjectStorageBase<T, U> : SchedulerFrameworkElement, INotifyPropertyChanged
		where T : IPersistentObjectStorage<U>
		where U : IPersistentObject {
		readonly IPersistentObjectStorage<U> innerStorage;
		readonly Locker loadingLocker = new Locker();
		protected PersistentObjectStorageBase(SchedulerStorage schedulerStorage) {
			this.innerStorage = ObtainInnerStorage(schedulerStorage);
			PreInitialize();
			PreparePropertySyncManager();
		}
		protected PersistentObjectStorageBase() {
			this.innerStorage = CreateInnerStorage();
			PreInitialize();
			PreparePropertySyncManager();
		}
		protected Locker LoadingLocker { get { return loadingLocker; } }
		public bool IsLoading { get { return loadingLocker.IsLocked; } }
		#region CustomFieldMappings
		public SchedulerCustomFieldMappingCollection CustomFieldMappings {
			get { return (SchedulerCustomFieldMappingCollection)GetValue(CustomFieldMappingsProperty); }
			set { SetValue(CustomFieldMappingsProperty, value); }
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly DependencyProperty CustomFieldMappingsProperty = CreateCustomFieldMappingsProperty();
		static DependencyProperty CreateCustomFieldMappingsProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<PersistentObjectStorageBase<T, U>, SchedulerCustomFieldMappingCollection>("CustomFieldMappings", null, (d, e) => d.OnCustomFieldMappingsChanged(e.OldValue, e.NewValue), null);
		}
		#endregion;
		void PreparePropertySyncManager() {
			CreatePropertySyncManager();
			PropertySyncManager.Activate();
		}
		void PreInitialize() {
			CustomFieldMappings = CreateCustomFieldMappingCollection();
		}
		protected internal IPersistentObjectStorage<U> InnerStorage { get { return innerStorage; } }
		protected abstract IPersistentObjectStorage<U> CreateInnerStorage();
		protected abstract PersistentObjectMapping<T, U> CreatePersistentObjectMapping();
		protected abstract IPersistentObjectStorage<U> ObtainInnerStorage(SchedulerStorage schedulerStorage);
		protected internal virtual void InitializeProperties() {
		}
		protected internal virtual void SetupMappingsInnerDataContextBinding(PersistentObjectMapping<T, U> persistentObjectMapping) {
			InnerBindingHelper.SetupDataContextBinding(this, persistentObjectMapping);
		}
		protected void OnCustomFieldMappingsChanged(SchedulerCustomFieldMappingCollection oldValue, SchedulerCustomFieldMappingCollection newValue) {
			if (oldValue != null)
				UnsubscribeCustomFieldMappingsEvents(oldValue);
			if (newValue != null) {
				ReCreateInnerStorageCustomFieldMappings(newValue);
				SubscribeCustomFieldMappingsEvents(newValue);
			} else
				ClearInnerStorageCustomFieldMappings();
		}
		#region Inner Storage facade methods
		public bool IsUpdateLocked { get { return InnerStorage.IsUpdateLocked; } }
		public void BeginUpdate() { InnerStorage.BeginUpdate(); }
		public void CancelUpdate() { InnerStorage.CancelUpdate(); }
		public void EndUpdate() { InnerStorage.EndUpdate(); }
		public bool UnboundMode { get { return InnerStorage.UnboundMode; } }
		public U this[int index] { get { return InnerStorage[index]; } }
		public NotificationCollection<U> GetItems() { return InnerStorage.Items; }
		public int Count { get { return InnerStorage.Count; } }
		public void Clear() { InnerStorage.Clear(); }
		public abstract int Add(U obj);
		public abstract void AddRange(U[] obj);
		public abstract void Remove(U obj);
		public abstract bool Contains(U obj);
		public object GetObjectRow(U obj) { return InnerStorage.GetObjectRow(obj); }
		public object GetObjectValue(U obj, string columnName) { return InnerStorage.GetObjectValue(obj, columnName); }
		public void SetObjectValue(U obj, string columnName, object val) { InnerStorage.SetObjectValue(obj, columnName, val); }
		public virtual StringCollection GetColumnNames() { return InnerStorage.GetColumnNames(); }
		public virtual void CreateCustomFields(U obj) { InnerStorage.CreateCustomFields(obj); }
		#endregion
		protected virtual void ClearInnerStorageCustomFieldMappings() {
			InnerStorage.CustomFieldMappings.Clear();
		}
		protected virtual void ReCreateInnerStorageCustomFieldMappings(SchedulerCustomFieldMappingCollection container) {
			int count = container.Count;
			InnerStorage.BeginUpdate();
			CustomFieldMappingCollectionBase<U> collection = InnerStorage.CustomFieldMappings;
			try {
				collection.Clear();
				for (int i = 0; i < count; i++) {
					SchedulerCustomFieldMapping item = container[i] as SchedulerCustomFieldMapping;
					if (item != null) {
						CustomFieldMappingBase<U> mapping = CreateInnerCustomFieldMapping(item);
						collection.Add(mapping);
					}
				}
			} finally {
				InnerStorage.EndUpdate();
			}
		}
		protected abstract CustomFieldMappingBase<U> CreateInnerCustomFieldMapping(SchedulerCustomFieldMapping item);
		protected virtual SchedulerCustomFieldMappingCollection CreateCustomFieldMappingCollection() {
			return new SchedulerCustomFieldMappingCollection();
		}
		protected virtual void UnsubscribeCustomFieldMappingsEvents(SchedulerCustomFieldMappingCollection container) {
			if (container != null)
				((INotifyPropertyChanged)container).PropertyChanged -= new PropertyChangedEventHandler(OnCustomFieldMappingsPropertyChanged);
		}
		void OnCustomFieldMappingsPropertyChanged(object sender, PropertyChangedEventArgs e) {
			ReCreateInnerStorageCustomFieldMappings(CustomFieldMappings);
		}
		protected virtual void SubscribeCustomFieldMappingsEvents(SchedulerCustomFieldMappingCollection container) {
			if (container != null)
				((INotifyPropertyChanged)container).PropertyChanged += new PropertyChangedEventHandler(OnCustomFieldMappingsPropertyChanged);
		}
		protected virtual void CreatePropertySyncManager() {
			this.propertySyncManager = CreateDependencyPropertySyncManager();
			PropertySyncManager.Register();
		}
		#region PropertySyncManager
		DependencyPropertySyncManager propertySyncManager;
		protected internal DependencyPropertySyncManager PropertySyncManager { get { return propertySyncManager; } }
		protected abstract DependencyPropertySyncManager CreateDependencyPropertySyncManager();
		#endregion
		#region DataSource
		public object DataSource {
			get { return (object)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly DependencyProperty DataSourceProperty = CreateDataSourceProperty();
		static DependencyProperty CreateDataSourceProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<PersistentObjectStorageBase<T, U>, object>("DataSource", null, (d, e) => d.OnDataSourceChanged(e.OldValue, e.NewValue), null);
		}
		private void OnDataSourceChanged(object oldValue, object newValue) {
			UpdateInnerObjectPropertyValue(DataSourceProperty, null, newValue);
		}
		#endregion
		#region AutoReload
		public bool AutoReload {
			get { return (bool)GetValue(AutoReloadProperty); }
			set { SetValue(AutoReloadProperty, value); }
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly DependencyProperty AutoReloadProperty = CreateAutoReloadProperty();
		static DependencyProperty CreateAutoReloadProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<PersistentObjectStorageBase<T, U>, bool>("AutoReload", true, (d, e) => d.OnAutoReloadChanged(e.OldValue, e.NewValue), null);
		}
		private void OnAutoReloadChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(AutoReloadProperty, null, newValue);
		}
		#endregion
		#region Filter
		public string Filter {
			get { return (string)GetValue(FilterProperty); }
			set { SetValue(FilterProperty, value); }
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly DependencyProperty FilterProperty = CreateFilterProperty();
		static DependencyProperty CreateFilterProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<PersistentObjectStorageBase<T, U>, string>("Filter", String.Empty, (d, e) => d.OnFilterChanged(e.OldValue, e.NewValue), null);
		}
		private void OnFilterChanged(string oldValue, string newValue) {
			UpdateInnerObjectPropertyValue(FilterProperty, null, newValue);
		}
		#endregion
		#region DataMember
		public string DataMember {
			get { return (string)GetValue(DataMemberProperty); }
			set { SetValue(DataMemberProperty, value); }
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly DependencyProperty DataMemberProperty = CreateDataMemberProperty();
		static DependencyProperty CreateDataMemberProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<PersistentObjectStorageBase<T, U>, string>("DataMember", String.Empty, (d, e) => d.OnDataMemberChanged(e.OldValue, e.NewValue), null);
		}
		private void OnDataMemberChanged(string oldValue, string newValue) {
			UpdateInnerObjectPropertyValue(DataMemberProperty, null, newValue);
		}
		#endregion
		protected virtual bool CanSyncInnerObject() {
			return !IsLoading && InnerStorage != null && InnerStorage.Storage != null;
		}
		protected internal virtual void UpdateInnerObjectPropertyValue(DependencyProperty property, object oldValue, object newValue) {
			PropertySyncManager.Update(property, oldValue, newValue);
		}
		protected internal virtual void SynchronizeInnerObjectProperties() {
			if (PropertySyncManager.IsDeferredChanges)
				PropertySyncManager.CommitDeferredChanges();
		}
#if SL
		public override void BeginInit() {
			base.BeginInit();
			if (!CanSyncInnerObject()) {
				PropertySyncManager.StartDeferredChanges();
			}
		}
		public override void EndInit() {
			base.EndInit();
			if (CanSyncInnerObject())
				PropertySyncManager.CommitDeferredChanges();
		}
#endif
		protected internal virtual void OnOwnerEndInit() {
			SynchronizeInnerObjectProperties();
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			InitializeProperties();
		}
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		#region OnPropertyChanged
		public virtual void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		#endregion
	}
	#endregion
	public class AppointmentStorage : PersistentObjectStorageBase<IAppointmentStorageBase, Appointment> {
		IAppointmentLabelStorage defaultLabelsCollection;
		IAppointmentStatusStorage defaultStatusesCollection;
		public AppointmentStorage(SchedulerStorage schedulerStorage)
			: base(schedulerStorage) {
			InitializeLabelsAndStatuses();
		}
		public AppointmentStorage() {
			InitializeLabelsAndStatuses();
		}
		#region Properties
		#region InnerAppointments
		protected internal IAppointmentStorageBase InnerAppointments { get { return (IAppointmentStorageBase)InnerStorage; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentStorageMappings")]
#endif
		#endregion
		#region AppointmentMapping
		public AppointmentMapping Mappings {
			get { return (AppointmentMapping)GetValue(MappingsProperty); }
			set { SetValue(MappingsProperty, value); }
		}
		public static readonly DependencyProperty MappingsProperty = CreateAppointmentMappingProperty();
		static DependencyProperty CreateAppointmentMappingProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentStorage, AppointmentMapping>("Mappings", null, (d, e) => d.OnAppointmentMappingChanged(e.OldValue, e.NewValue), null);
		}
		protected virtual void OnAppointmentMappingChanged(AppointmentMapping oldValue, AppointmentMapping newValue) {
			MappingInfoBase<Appointment> mappingInfo = newValue != null ? newValue.InnerMappingInfo : null;
			if (mappingInfo == null)
				mappingInfo = new AppointmentMappingInfo();
			InnerStorage.Mappings = mappingInfo;
			SyncMappingResourceSharingProperty(mappingInfo);
			SetupMappingsInnerDataContextBinding(newValue);
		}
		protected void SyncMappingResourceSharingProperty(MappingInfoBase<Appointment> value) {
			AppointmentMappingInfo mappingInfo = value as AppointmentMappingInfo;
			if (mappingInfo != null)
				mappingInfo.ResourceSharing = ResourceSharing;
		}
		#endregion
		#region CommitIdToDataSource
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentStorageCommitIdToDataSource")]
#endif
		public bool CommitIdToDataSource {
			get { return (bool)GetValue(CommitIdToDataSourceProperty); }
			set { SetValue(CommitIdToDataSourceProperty, value); }
		}
		public static readonly DependencyProperty CommitIdToDataSourceProperty = CreateCommitIdToDataSourceProperty();
		static DependencyProperty CreateCommitIdToDataSourceProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentStorage, bool>("CommitIdToDataSource", false, (d, e) => d.OnCommitIdToDataSourceChanged(e.OldValue, e.NewValue), null);
		}
		private void OnCommitIdToDataSourceChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(CommitIdToDataSourceProperty, null, newValue);
		}
		#endregion
		#region Labels
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppointmentLabelCollection Labels {
			get { return (AppointmentLabelCollection)GetValue(LabelsProperty); }
			set { SetValue(LabelsProperty, value); }
		}
		public static readonly DependencyProperty LabelsProperty = CreateLabelsProperty();
		static DependencyProperty CreateLabelsProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentStorage, AppointmentLabelCollection>("Labels", null, (d, e) => d.OnLabelsChanged(e.OldValue, e.NewValue), null);
		}
		protected void OnLabelsChanged(AppointmentLabelCollection oldValue, AppointmentLabelCollection newValue) {
			INotifyCollectionChanged oldCollectionChanged = oldValue as INotifyCollectionChanged;
			if (oldCollectionChanged != null)
				oldCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnLabelCollectionChanged);
			INotifyCollectionChanged newCollectionChanged = newValue as INotifyCollectionChanged;
			if (newValue != null)
				newCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(OnLabelCollectionChanged);
			ReCreateInnerStorageLabels(newValue);
		}
		void OnLabelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			ReCreateInnerStorageLabels(Labels);
		}
		internal virtual void ReplaceInnerLabelsAndStatuses() {
			BeginUpdate();
			ReCreateInnerStorageLabels(Labels);
			ReCreateInnerStorageStatuses(Statuses);
			EndUpdate();
		}
		protected virtual void ReCreateInnerStorageLabels(IAppointmentLabelStorage container) {
			InnerStorage.BeginUpdate();
			IAppointmentLabelStorage collection = InnerAppointments.Labels;
			try {
				collection.Clear();
				if (container == null)
					return;
				collection.AddRange((ICollection)container);
			} finally {
				InnerStorage.EndUpdate();
				((IInternalPersistentObjectStorage<Appointment>)InnerStorage).RaiseReload(false);
			}
		}
		#endregion;
		#region Statuses
		public AppointmentStatusCollection Statuses {
			get { return (AppointmentStatusCollection)GetValue(StatusesProperty); }
			set { SetValue(StatusesProperty, value); }
		}
		public static readonly DependencyProperty StatusesProperty = CreateStatusesProperty();
		static DependencyProperty CreateStatusesProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentStorage, AppointmentStatusCollection>("Statuses", null, (d, e) => d.OnStatusesChanged(e.OldValue, e.NewValue), null);
		}
		protected void OnStatusesChanged(AppointmentStatusCollection oldValue, AppointmentStatusCollection newValue) {
			INotifyCollectionChanged oldCollectionChanged = oldValue as INotifyCollectionChanged;
			if (oldCollectionChanged != null)
				oldCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnStatusCollectionChanged);
			INotifyCollectionChanged newCollectionChanged = newValue as INotifyCollectionChanged;
			if (newCollectionChanged != null)
				newCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(OnStatusCollectionChanged);
			ReCreateInnerStorageStatuses(newValue);
		}
		void OnStatusCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			ReCreateInnerStorageStatuses(Statuses);
		}
		protected virtual void ReCreateInnerStorageStatuses(IAppointmentStatusStorage container) {
			InnerStorage.BeginUpdate();
			try {
				InnerAppointments.Statuses.Clear();
				if (container == null)
					return;
				InnerAppointments.Statuses.AddRange((ICollection)container);
			} finally {
				InnerStorage.EndUpdate();
			}
		}
		#endregion;
		#region ResourceSharing
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentStorageResourceSharing")]
#endif
		public bool ResourceSharing {
			get { return (bool)GetValue(ResourceSharingProperty); }
			set { SetValue(ResourceSharingProperty, value); }
		}
		public static readonly DependencyProperty ResourceSharingProperty = CreateResourceSharingProperty();
		static DependencyProperty CreateResourceSharingProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentStorage, bool>("ResourceSharing", false, (d, e) => d.OnResourceSharingChanged(e.OldValue, e.NewValue), null);
		}
		private void OnResourceSharingChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(ResourceSharingProperty, null, newValue);
		}
		#endregion        
		#endregion
		#region Events
		public event SchedulerUnhandledExceptionEventHandler LoadException {
			add {
				if (InnerStorage != null)
					InnerAppointments.LoadException += value;
			}
			remove {
				if (InnerStorage != null)
					InnerAppointments.LoadException -= value;
			}
		}
		#endregion
#if SL
		public override void BeginInit() {
			LoadingLocker.Lock();
			base.BeginInit();
		}
		public override void EndInit() {
			base.EndInit();
			LoadingLocker.Unlock();
		}
#endif
		protected internal override void InitializeProperties() {
			base.InitializeProperties();
			if (this.defaultLabelsCollection == Labels && Labels.Count < 1) {
				AppointmentLabelCollection newLabels = new AppointmentLabelCollection();
				InnerAppointments.Labels.LoadDefaults();
				SynchronizeLabels(newLabels);
				SetCurrentValue(LabelsProperty, newLabels);
			}
			if (this.defaultStatusesCollection == Statuses && Statuses.Count < 1) {
				AppointmentStatusCollection newStatuses = new AppointmentStatusCollection();
				InnerAppointments.Statuses.LoadDefaults();
				SynchronizeStatuses(newStatuses);
				SetCurrentValue(StatusesProperty, newStatuses);
			}
			if (Mappings == null)
				SetCurrentValue(MappingsProperty, (AppointmentMapping)CreatePersistentObjectMapping());
		}
		void InitializeLabelsAndStatuses() {
			this.defaultLabelsCollection = CreateLabelsContainer();
			this.defaultLabelsCollection.LoadDefaults();
			this.defaultStatusesCollection = CreateStatusesContainer();
			this.defaultStatusesCollection.LoadDefaults();
			SetCurrentValue(LabelsProperty, defaultLabelsCollection);
			SetCurrentValue(StatusesProperty, defaultStatusesCollection);
		}
		void SynchronizeLabels(IAppointmentLabelStorage newLabels) {
			newLabels.AddRange((ICollection)InnerAppointments.Labels);
		}
		void SynchronizeStatuses(IAppointmentStatusStorage newStatuses) {
			newStatuses.AddRange((ICollection)InnerAppointments.Statuses);
		}
		protected virtual IAppointmentLabelStorage CreateLabelsContainer() {
			return new AppointmentLabelCollection();
		}
		protected virtual IAppointmentStatusStorage CreateStatusesContainer() {
			return new AppointmentStatusCollection();
		}
		protected override IPersistentObjectStorage<Appointment> ObtainInnerStorage(SchedulerStorage schedulerStorage) {
			XtraSchedulerDebug.Assert(schedulerStorage.InnerStorage != null);
			return schedulerStorage.InnerStorage.Appointments;
		}
		protected override PersistentObjectMapping<IAppointmentStorageBase, Appointment> CreatePersistentObjectMapping() {
			return new AppointmentMapping(this);
		}
		protected override IPersistentObjectStorage<Appointment> CreateInnerStorage() {
			return new AppointmentDataStorage();
		}
		protected override DependencyPropertySyncManager CreateDependencyPropertySyncManager() {
			return new AppointmentStoragePropertySyncManager(this);
		}
		protected override CustomFieldMappingBase<Appointment> CreateInnerCustomFieldMapping(SchedulerCustomFieldMapping item) {
			return new AppointmentCustomFieldMapping(item.Name, item.Member, item.ValueType);
		}
		#region Inner Storage facade methods
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentStorageSupportsRecurrence")]
#endif
		public bool SupportsRecurrence { get { return InnerAppointments.SupportsRecurrence; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentStorageSupportsReminders")]
#endif
		public bool SupportsReminders { get { return InnerAppointments.SupportsReminders; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentStorageItems")]
#endif
		public AppointmentCollection Items { get { return InnerAppointments.Items; } }
		public bool IsNewAppointment(Appointment apt) { return InnerAppointments.IsNewAppointment(apt); }
		public void SetAppointmentFactory(IAppointmentFactory factory) { InnerAppointments.SetAppointmentFactory(factory); }
		public Appointment GetAppointmentById(object id) { return InnerAppointments.GetAppointmentById(id); }
		public override int Add(Appointment obj) {
			return InnerAppointments.Add(obj);
		}
		public override void AddRange(Appointment[] obj) {
			InnerAppointments.AddRange(obj);
		}
		public override void Remove(Appointment obj) {
			InnerAppointments.Remove(obj);
		}
		public override bool Contains(Appointment obj) {
			return InnerAppointments.Contains(obj);
		}
		public void LoadFromXml(Stream stream) {
			InnerAppointments.LoadFromXml(stream);
		}
		public void LoadFromXml(string fileName) {
			InnerAppointments.LoadFromXml(fileName);
		}
		public void SaveToXml(Stream stream) {
			InnerAppointments.SaveToXml(stream);
		}
		public void SaveToXml(string fileName) {
			InnerAppointments.SaveToXml(fileName);
		}
		public Appointment CreateAppointment(AppointmentType type) {
			return InnerAppointments.CreateAppointment(type);
		}
		public Appointment CreateAppointment(AppointmentType type, DateTime start, DateTime end) {
			return InnerAppointments.CreateAppointment(type, start, end);
		}
		public Appointment CreateAppointment(AppointmentType type, DateTime start, TimeSpan duration) {
			return InnerAppointments.CreateAppointment(type, start, duration);
		}
		public Appointment CreateAppointment(AppointmentType type, DateTime start, DateTime end, string subject) {
			return InnerAppointments.CreateAppointment(type, start, end, subject);
		}
		public Appointment CreateAppointment(AppointmentType type, DateTime start, TimeSpan duration, string subject) {
			return InnerAppointments.CreateAppointment(type, start, duration, subject);
		}
		#endregion
	}
	public class ResourceStorage : PersistentObjectStorageBase<IResourceStorageBase, Resource> {
		public ResourceStorage(SchedulerStorage schedulerStorage)
			: base(schedulerStorage) {
		}
		public ResourceStorage() {
		}
		protected internal override void InitializeProperties() {
			base.InitializeProperties();
			if (Mappings == null)
				SetCurrentValue(MappingsProperty, (ResourceMapping)CreatePersistentObjectMapping());
		}
#if SL
		public override void BeginInit() {
			LoadingLocker.Lock();
			base.BeginInit();
		}
		public override void EndInit() {
			base.EndInit();
			LoadingLocker.Unlock();
		}
#endif
		protected internal ResourceDataStorage InnerResources { get { return (ResourceDataStorage)InnerStorage; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceStorageMappings")]
#endif
		#region ResourceMapping
		public ResourceMapping Mappings {
			get { return (ResourceMapping)GetValue(MappingsProperty); }
			set { SetValue(MappingsProperty, value); }
		}
		public static readonly DependencyProperty MappingsProperty = CreateResourceMappingProperty();
		static DependencyProperty CreateResourceMappingProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceStorage, ResourceMapping>("Mappings", null, (d, e) => d.OnResourceMappingChanged(e.OldValue, e.NewValue), null);
		}
		protected virtual void OnResourceMappingChanged(ResourceMapping oldValue, ResourceMapping newValue) {
			MappingInfoBase<Resource> mappingInfo = newValue != null ? newValue.InnerMappingInfo : null;
			if (mappingInfo == null)
				mappingInfo = new ResourceMappingInfo();
			InnerStorage.Mappings = MergeMappingInfos((ResourceMappingInfo)InnerStorage.Mappings, (ResourceMappingInfo)mappingInfo);
			SetupMappingsInnerDataContextBinding(newValue);
		}
		#endregion
		#region ColorSaving
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceStorageColorSaving")]
#endif
		public ColorSavingType ColorSaving {
			get { return (ColorSavingType)GetValue(ColorSavingProperty); }
			set { SetValue(ColorSavingProperty, value); }
		}
		public static readonly DependencyProperty ColorSavingProperty = CreateColorSavingProperty();
		static DependencyProperty CreateColorSavingProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceStorage, ColorSavingType>("ColorSaving", ColorSavingType.OleColor, (d, e) => d.OnColorSavingChanged(e.OldValue, e.NewValue), null);
		}
		private void OnColorSavingChanged(ColorSavingType oldValue, ColorSavingType newValue) {
			UpdateInnerObjectPropertyValue(ColorSavingProperty, null, newValue);
		}
		#endregion
		#region Inner Storage facade methods
		public void SetResourceFactory(IResourceFactory factory) { InnerResources.SetResourceFactory(factory); }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourceStorageItems")]
#endif
		public ResourceCollection Items { get { return InnerResources.Items; } }
		public override int Add(Resource obj) {
			return InnerResources.Add(obj);
		}
		public override void AddRange(Resource[] obj) {
			InnerResources.AddRange(obj);
		}
		public override void Remove(Resource obj) {
			InnerResources.Remove(obj);
		}
		public override bool Contains(Resource obj) {
			return InnerResources.Contains(obj);
		}
		public void LoadFromXml(Stream stream) {
			InnerResources.LoadFromXml(stream);
		}
		public void LoadFromXml(string fileName) {
			InnerResources.LoadFromXml(fileName);
		}
		public void SaveToXml(Stream stream) {
			InnerResources.SaveToXml(stream);
		}
		public void SaveToXml(string fileName) {
			InnerResources.SaveToXml(fileName);
		}
		#endregion
		protected override IPersistentObjectStorage<Resource> ObtainInnerStorage(SchedulerStorage schedulerStorage) {
			XtraSchedulerDebug.Assert(schedulerStorage.InnerStorage != null);
			return schedulerStorage.InnerStorage.Resources;
		}
		protected override PersistentObjectMapping<IResourceStorageBase, Resource> CreatePersistentObjectMapping() {
			return new ResourceMapping(this);
		}
		protected override IPersistentObjectStorage<Resource> CreateInnerStorage() {
			return new ResourceDataStorage();
		}
		protected override DependencyPropertySyncManager CreateDependencyPropertySyncManager() {
			return new ResourceStoragePropertySyncManager(this);
		}
		protected override CustomFieldMappingBase<Resource> CreateInnerCustomFieldMapping(SchedulerCustomFieldMapping item) {
			return new ResourceCustomFieldMapping(item.Name, item.Member, item.ValueType);
		}
		public Resource GetResourceById(object resourceId) {
			return InnerResources.GetResourceById(resourceId);
		}
		MappingInfoBase<Resource> MergeMappingInfos(ResourceMappingInfo source, ResourceMappingInfo result) {
			result.ColorSaving = source.ColorSaving;
			return result;
		}
		public Resource CreateResource(object id) {
			return InnerResources.CreateResource(id);
		}
	}
}
