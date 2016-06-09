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
using System.Text;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Data;
using DevExpress.XtraScheduler.Exchange;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using System.Runtime.InteropServices;
using DevExpress.Data.Helpers;
using System.Security;
using System.Diagnostics.Contracts;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal;
using DevExpress.XtraScheduler.Commands;
#if !SL
#else
using System.Windows.Media;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.XtraScheduler {
	#region ColorSavingType
	public enum ColorSavingType {
		OleColor = 0,
		ArgbColor = 1,
		Color = 2
	}
	#endregion
	public interface IDisposeState {
		bool IsDisposed { get; }
	}
	#region SchedulerStorageBase (abstract class)
	public abstract class SchedulerStorageBase : Component, ISchedulerStorageBase, IInternalSchedulerStorageBase, ISupportInitialize, IBatchUpdateHandler {
		#region Fields
		BatchUpdateHelper batchUpdateHelper;
		IResourceStorageBase resources;
		IAppointmentStorageBase appointments;
		IAppointmentDependencyStorage dependencies;
		ReminderEngine remindersEngine;
		TimeZoneEngine timeZoneEngine;
		bool isDisposed;
		SchedulerStorageDeferredChanges deferredChanges;
		AppointmentMultiClientCache appointmentCache;
		bool appointmentCacheEnabled = true;
		List<object> activeCallers = new List<object>();
		TimeInterval lastFetchInterval;
		SchedulerBlocker resourceStorageEventsSubscribtionLocker = new SchedulerBlocker();
		AppointmentTree appointmentTree;
		AppointmentTreeController appointmentTreeController;
		SchedulerBlocker fetchAppointmentLocker = new SchedulerBlocker();
		#endregion
		protected SchedulerStorageBase() {
			TimeZoneEngine = new TimeZoneEngine();
			InitializeTimeZoneEngine();
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.appointments = CreateAppointmentStorage();
			this.resources = CreateResourceStorage();
			this.dependencies = CreateAppointmentDependencyStorage();
			this.remindersEngine = CreateRemindersEngine();
			this.remindersEngine.Enabled = true;
			this.appointmentCache = CreateAppointmentCache();
			this.appointmentCache.CreateCache(this);
			this.appointmentTree = CreateAppointmentTree();
			this.appointmentTreeController = CreateAppointmentTreeController();
			SubscribeAppointmentStorageEvents();
			SubscribeResourceStorageEvents();
			SubscribeDependencyStorageEvents();
			SubscribeRemindersEngineEvents();
		}
		protected virtual void InitializeTimeZoneEngine() {
		}
		protected SchedulerStorageBase(IContainer components)
			: this() {
#if !SL
			if (components != null)
				components.Add(this);
#endif
		}
		#region Properties
		#region TimeZoneEngine
		TimeZoneInfo ISupportTimeZoneEngine.DefaultAppointmentTimeZone {
			get { return TimeZoneEngine.DefaultAppointmentTimeZone; }
			set { TimeZoneEngine.DefaultAppointmentTimeZone = value; }
		}
		TimeZoneEngine IInternalSchedulerStorageBase.TimeZoneEngine {
			get { return timeZoneEngine; }
			set { timeZoneEngine = value; }
		}
		internal TimeZoneEngine TimeZoneEngine {
			get { return ((IInternalSchedulerStorageBase)this).TimeZoneEngine; }
			set { ((IInternalSchedulerStorageBase)this).TimeZoneEngine = value; }
		}
		#endregion
		#region IsDisposed
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool IsDisposed { get { return isDisposed; } }
		bool IDisposeState.IsDisposed { get { return IsDisposed; } }
		#endregion
		[Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		protected internal IAppointmentStorageBase InnerAppointments {
			get { return appointments; }
			set {
				if (appointments == value)
					return;
				SetInnerAppointmentsCore(value);
			}
		}
		[Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		protected internal IResourceStorageBase InnerResources {
			get { return resources; }
			set {
				if (resources == value)
					return;
				SetInnerResourcesCore(value);
			}
		}
		[Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		protected internal IAppointmentDependencyStorage InnerAppointmentDependencies {
			get { return dependencies; }
			set {
				if (dependencies == value)
					return;
				SetInnerDependenciesCore(value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SupportsRecurrence { get { return InnerAppointments.SupportsRecurrence; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SupportsReminders { get { return InnerAppointments.SupportsReminders; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ResourceSharing { get { return InnerAppointments.ResourceSharing; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UnboundMode { get { return InnerAppointments.UnboundMode; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RemindersEnabled { get { return EnableReminders && SupportsReminders; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal SchedulerStorageDeferredChanges DeferredChanges { get { return deferredChanges; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AppointmentCacheEnabled { get { return appointmentCacheEnabled; } set { appointmentCacheEnabled = value; } }
		protected ReminderEngine RemindersEngine { get { return this.remindersEngine; } }
		protected AppointmentMultiClientCache AppointmentCache { get { return appointmentCache; } }
		ReminderEngine IInternalSchedulerStorageBase.RemindersEngine { get { return RemindersEngine; } }
		AppointmentMultiClientCache IInternalSchedulerStorageBase.AppointmentCache { get { return AppointmentCache; } }
		protected internal AppointmentTree AppointmentTree { get { return appointmentTree; } }
		protected internal AppointmentTreeController AppointmentTreeController { get { return appointmentTreeController; } }
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseEnableReminders"),
#endif
 DefaultValue(true), Category(SRCategoryNames.Behavior), NotifyParentProperty(true), AutoFormatDisable()]
		public bool EnableReminders {
			get { return remindersEngine.Enabled; }
			set {
				if (!IsUpdateLocked) {
					if (EnableReminders == value)
						return;
					if (value)
						SubscribeRemindersEngineEvents();
					else
						UnsubscribeRemindersEngineEvents();
					remindersEngine.Enabled = value;
					RaiseUpdateUI();
				} else
					deferredChanges.EnableReminders = value;
			}
		}
		bool enableTimeZones = true;
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseEnableTimeZones"),
#endif
		DefaultValue(true), Category(SRCategoryNames.Behavior), NotifyParentProperty(true), AutoFormatDisable()]
		public bool EnableTimeZones {
			get { return enableTimeZones; }
			set {
				if (EnableTimeZones == value)
					return;
				enableTimeZones = value;
				RaiseUpdateUI();
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseRemindersCheckInterval"),
#endif
		DefaultValue(ReminderEngineBase.DefaultCheckInterval), Category(SRCategoryNames.Behavior), NotifyParentProperty(true), AutoFormatDisable()]
		public int RemindersCheckInterval {
			get { return remindersEngine.CheckInterval; }
			set { remindersEngine.CheckInterval = value; }
		}
		bool IInternalSchedulerStorageBase.DesignModeProtected { get { return DesignMode; } }
		#region TimeZoneId
		string timeZoneId;
		[TypeConverter(typeof(DevExpress.XtraScheduler.Design.TimeZoneIdStringTypeConverter)),
		XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public string TimeZoneId {
			get { return TimeZoneEngine.OperationTimeZone.Id; }
			set {
				if (value == timeZoneId)
					return;
				TimeZoneInfo newTimeZone = null;
				try {
					newTimeZone = TimeZoneInfo.FindSystemTimeZoneById(value);
				} catch {
					value = null;
					newTimeZone = TimeZoneEngine.Local;
				}
				this.timeZoneId = value;
				TimeZoneEngine.OperationTimeZone = newTimeZone;
				RefreshData();
			}
		}
		protected internal virtual bool ShouldSerializeTimeZoneId() {
			return !String.IsNullOrEmpty(this.timeZoneId);
		}
		protected internal virtual void ResetTimeZoneId() {
			TimeZoneId = null;
		}
		#endregion
		#endregion
		#region Events
		#region InternalAppointmentCollectionLoaded
		EventHandler onInternalAppointmentCollectionLoaded;
		protected internal event EventHandler InternalAppointmentCollectionLoaded { add { onInternalAppointmentCollectionLoaded += value; } remove { onInternalAppointmentCollectionLoaded -= value; } }
		protected internal virtual void RaiseInternalAppointmentCollectionLoaded() {
			if (onInternalAppointmentCollectionLoaded != null)
				onInternalAppointmentCollectionLoaded(this, EventArgs.Empty);
		}
		#endregion
		#region InternalAppointmentCollectionCleared
		EventHandler onInternalAppointmentCollectionCleared;
		protected internal event EventHandler InternalAppointmentCollectionCleared { add { onInternalAppointmentCollectionCleared += value; } remove { onInternalAppointmentCollectionCleared -= value; } }
		protected internal virtual void RaiseInternalAppointmentCollectionCleared() {
			if (onInternalAppointmentCollectionCleared != null)
				onInternalAppointmentCollectionCleared(this, EventArgs.Empty);
		}
		#endregion
		#region InternalAppointmentsInserted
		PersistentObjectsEventHandler onInternalAppointmentsInserted;
		protected internal event PersistentObjectsEventHandler InternalAppointmentsInserted { add { onInternalAppointmentsInserted += value; } remove { onInternalAppointmentsInserted -= value; } }
		protected internal virtual void RaiseInternalAppointmentsInserted(AppointmentBaseCollection appointments) {
			if (onInternalAppointmentsInserted != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(appointments);
				onInternalAppointmentsInserted(this, args);
			}
		}
		#endregion
		#region InternalAppointmentsChanged
		PersistentObjectsEventHandler onInternalAppointmentsChanged;
		protected internal event PersistentObjectsEventHandler InternalAppointmentsChanged { add { onInternalAppointmentsChanged += value; } remove { onInternalAppointmentsChanged -= value; } }
		protected internal virtual void RaiseInternalAppointmentsChanged(AppointmentBaseCollection appointments) {
			if (onInternalAppointmentsChanged != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(appointments);
				onInternalAppointmentsChanged(this, args);
			}
		}
		#endregion
		#region InternalAppointmentsDeleted
		PersistentObjectsEventHandler onInternalAppointmentsDeleted;
		protected internal event PersistentObjectsEventHandler InternalAppointmentsDeleted { add { onInternalAppointmentsDeleted += value; } remove { onInternalAppointmentsDeleted -= value; } }
		protected internal virtual void RaiseInternalAppointmentsDeleted(AppointmentBaseCollection appointments) {
			if (onInternalAppointmentsDeleted != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(appointments);
				onInternalAppointmentsDeleted(this, args);
			}
		}
		#endregion
		#region InternalAppointmentMappingsChanged
		EventHandler onInternalAppointmentMappingsChanged;
		protected internal event EventHandler InternalAppointmentMappingsChanged { add { onInternalAppointmentMappingsChanged += value; } remove { onInternalAppointmentMappingsChanged -= value; } }
		protected internal virtual void RaiseInternalAppointmentMappingsChanged() {
			if (onInternalAppointmentMappingsChanged != null)
				onInternalAppointmentMappingsChanged(this, EventArgs.Empty);
		}
		#endregion
		#region InternalAppointmentUIObjectsChanged
		EventHandler onInternalAppointmentUIObjectsChanged;
		protected internal event EventHandler InternalAppointmentUIObjectsChanged { add { onInternalAppointmentUIObjectsChanged += value; } remove { onInternalAppointmentUIObjectsChanged -= value; } }
		protected internal virtual void RaiseInternalAppointmentUIObjectsChanged() {
			if (onInternalAppointmentUIObjectsChanged != null)
				onInternalAppointmentUIObjectsChanged(this, EventArgs.Empty);
		}
		#endregion
		#region InternalResourceCollectionLoaded
		EventHandler onInternalResourceCollectionLoaded;
		protected internal event EventHandler InternalResourceCollectionLoaded { add { onInternalResourceCollectionLoaded += value; } remove { onInternalResourceCollectionLoaded -= value; } }
		protected internal virtual void RaiseInternalResourceCollectionLoaded() {
			if (onInternalResourceCollectionLoaded != null)
				onInternalResourceCollectionLoaded(this, EventArgs.Empty);
		}
		#endregion
		#region InternalResourceCollectionCleared
		EventHandler onInternalResourceCollectionCleared;
		protected internal event EventHandler InternalResourceCollectionCleared { add { onInternalResourceCollectionCleared += value; } remove { onInternalResourceCollectionCleared -= value; } }
		protected internal virtual void RaiseInternalResourceCollectionCleared() {
			if (onInternalResourceCollectionCleared != null)
				onInternalResourceCollectionCleared(this, EventArgs.Empty);
		}
		#endregion
		#region InternalResourcesInserted
		PersistentObjectsEventHandler onInternalResourcesInserted;
		protected internal event PersistentObjectsEventHandler InternalResourcesInserted { add { onInternalResourcesInserted += value; } remove { onInternalResourcesInserted -= value; } }
		protected internal virtual void RaiseInternalResourcesInserted(ResourceBaseCollection resources) {
			if (onInternalResourcesInserted != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(resources);
				onInternalResourcesInserted(this, args);
			}
		}
		#endregion
		#region InternalResourcesChanged
		PersistentObjectsEventHandler onInternalResourcesChanged;
		protected internal event PersistentObjectsEventHandler InternalResourcesChanged { add { onInternalResourcesChanged += value; } remove { onInternalResourcesChanged -= value; } }
		protected internal virtual void RaiseInternalResourcesChanged(ResourceBaseCollection resources) {
			if (onInternalResourcesChanged != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(resources);
				onInternalResourcesChanged(this, args);
			}
		}
		#endregion
		#region InternalResourcesDeleted
		PersistentObjectsEventHandler onInternalResourcesDeleted;
		protected internal event PersistentObjectsEventHandler InternalResourcesDeleted { add { onInternalResourcesDeleted += value; } remove { onInternalResourcesDeleted -= value; } }
		protected internal virtual void RaiseInternalResourcesDeleted(ResourceBaseCollection resources) {
			if (onInternalResourcesDeleted != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(resources);
				onInternalResourcesDeleted(this, args);
			}
		}
		#endregion
		#region InternalResourceMappingsChanged
		EventHandler onInternalResourceMappoingsChanged;
		protected internal event EventHandler InternalResourceMappingsChanged { add { onInternalResourceMappoingsChanged += value; } remove { onInternalResourceMappoingsChanged -= value; } }
		protected internal virtual void RaiseInternalResourceMappingsChanged() {
			if (onInternalResourceMappoingsChanged != null) {
				onInternalResourceMappoingsChanged(this, EventArgs.Empty);
			}
		}
		#endregion
		#region InternalResourceVisibilityChanged
		EventHandler onInternalResourceVisibilityChanged;
		protected internal event EventHandler InternalResourceVisibilityChanged { add { onInternalResourceVisibilityChanged += value; } remove { onInternalResourceVisibilityChanged -= value; } }
		protected internal virtual void RaiseInternalResourceVisibilityChanged() {
			if (onInternalResourceVisibilityChanged != null)
				onInternalResourceVisibilityChanged(this, EventArgs.Empty);
		}
		#endregion
		#region InternalResourceSortedColumnsChange
		EventHandler onInternalResourceSortedColumnsChange;
		protected internal event EventHandler InternalResourceSortedColumnsChange { add { onInternalResourceSortedColumnsChange += value; } remove { onInternalResourceSortedColumnsChange -= value; } }
		protected internal virtual void RaiseInternalResourceSortedColumnsChange() {
			if (onInternalResourceSortedColumnsChange != null)
				onInternalResourceSortedColumnsChange(this, EventArgs.Empty);
		}
		#endregion
		#region InternalDependencyCollectionLoaded
		EventHandler onInternalDependencyCollectionLoaded;
		protected internal event EventHandler InternalDependencyCollectionLoaded { add { onInternalDependencyCollectionLoaded += value; } remove { onInternalDependencyCollectionLoaded -= value; } }
		protected internal virtual void RaiseInternalDependencyCollectionLoaded() {
			if (onInternalDependencyCollectionLoaded != null)
				onInternalDependencyCollectionLoaded(this, EventArgs.Empty);
		}
		#endregion
		#region InternalDependencyCollectionCleared
		EventHandler onInternalDependencyCollectionCleared;
		protected internal event EventHandler InternalDependencyCollectionCleared { add { onInternalDependencyCollectionCleared += value; } remove { onInternalDependencyCollectionCleared -= value; } }
		protected internal virtual void RaiseInternalDependencyCollectionCleared() {
			if (onInternalDependencyCollectionCleared != null)
				onInternalDependencyCollectionCleared(this, EventArgs.Empty);
		}
		#endregion
		#region InternalDependenciesInserted
		PersistentObjectsEventHandler onInternalDependenciesInserted;
		protected internal event PersistentObjectsEventHandler InternalDependenciesInserted { add { onInternalDependenciesInserted += value; } remove { onInternalDependenciesInserted -= value; } }
		protected internal virtual void RaiseInternalDependenciesInserted(AppointmentDependencyBaseCollection dependencies) {
			if (onInternalDependenciesInserted != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(dependencies);
				onInternalDependenciesInserted(this, args);
			}
		}
		#endregion
		#region InternalDependenciesChanged
		PersistentObjectsEventHandler onInternalDependenciesChanged;
		protected internal event PersistentObjectsEventHandler InternalDependenciesChanged { add { onInternalDependenciesChanged += value; } remove { onInternalDependenciesChanged -= value; } }
		protected internal virtual void RaiseInternalDependenciesChanged(AppointmentDependencyBaseCollection dependencies) {
			if (onInternalDependenciesChanged != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(dependencies);
				onInternalDependenciesChanged(this, args);
			}
		}
		#endregion
		#region InternalDependenciesDeleted
		PersistentObjectsEventHandler onInternalDependenciesDeleted;
		protected internal event PersistentObjectsEventHandler InternalDependenciesDeleted { add { onInternalDependenciesDeleted += value; } remove { onInternalDependenciesDeleted -= value; } }
		protected internal virtual void RaiseInternalDependenciesDeleted(AppointmentDependencyBaseCollection dependencies) {
			if (onInternalDependenciesDeleted != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(dependencies);
				onInternalDependenciesDeleted(this, args);
			}
		}
		#endregion
		#region InternalDependencyMappingsChanged
		EventHandler onInternalDependencyMappingsChanged;
		protected internal event EventHandler InternalDependencyMappingsChanged { add { onInternalDependencyMappingsChanged += value; } remove { onInternalDependencyMappingsChanged -= value; } }
		protected internal virtual void RaiseInternalDependencyMappingsChanged() {
			if (onInternalDependencyMappingsChanged != null)
				onInternalDependencyMappingsChanged(this, EventArgs.Empty);
		}
		#endregion
		#region InternalDependencyUIObjectsChanged
		EventHandler onInternalDependencyUIObjectsChanged;
		protected internal event EventHandler InternalDependencyUIObjectsChanged { add { onInternalDependencyUIObjectsChanged += value; } remove { onInternalDependencyUIObjectsChanged -= value; } }
		protected internal virtual void RaiseInternalDependencyUIObjectsChanged() {
			if (onInternalDependencyUIObjectsChanged != null)
				onInternalDependencyUIObjectsChanged(this, EventArgs.Empty);
		}
		#endregion
		#region InternalDeferredNotifications
		EventHandler onInternalDeferredNotifications;
		protected internal event EventHandler InternalDeferredNotifications { add { onInternalDeferredNotifications += value; } remove { onInternalDeferredNotifications -= value; } }
		protected internal virtual void RaiseInternalDeferredNotifications() {
			if (onInternalDeferredNotifications != null)
				onInternalDeferredNotifications(this, EventArgs.Empty);
		}
		#endregion
		#region AppointmentCollectionLoaded
		EventHandler onAppointmentCollectionLoaded;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentCollectionLoaded")]
#endif
		public event EventHandler AppointmentCollectionLoaded { add { onAppointmentCollectionLoaded += value; } remove { onAppointmentCollectionLoaded -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseAppointmentCollectionLoaded() {
			if (onAppointmentCollectionLoaded != null)
				onAppointmentCollectionLoaded(this, EventArgs.Empty);
		}
		#endregion
		#region AppointmentCollectionCleared
		EventHandler onAppointmentCollectionCleared;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentCollectionCleared")]
#endif
		public event EventHandler AppointmentCollectionCleared { add { onAppointmentCollectionCleared += value; } remove { onAppointmentCollectionCleared -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseAppointmentCollectionCleared() {
			if (onAppointmentCollectionCleared != null)
				onAppointmentCollectionCleared(this, EventArgs.Empty);
		}
		#endregion
		#region AppointmentCollectionModified
		EventHandler onAppointmentCollectionModified;
		internal event EventHandler AppointmentCollectionModified { add { onAppointmentCollectionModified += value; } remove { onAppointmentCollectionModified -= value; } }
		protected internal virtual void RaiseAppointmentCollectionModified() {
			if (onAppointmentCollectionModified != null)
				onAppointmentCollectionModified(this, EventArgs.Empty);
		}
		#endregion
		#region AppointmentCollectionAutoReloading
		CancelListChangedEventHandler onAppointmentCollectionAutoReloading;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentCollectionAutoReloading")]
#endif
		public event CancelListChangedEventHandler AppointmentCollectionAutoReloading { add { onAppointmentCollectionAutoReloading += value; } remove { onAppointmentCollectionAutoReloading -= value; } }
		public virtual void OnAppointmentsAutoReloading(object sender, CancelListChangedEventArgs e) {
			if (onAppointmentCollectionAutoReloading != null)
				onAppointmentCollectionAutoReloading(this, e);
		}
		#endregion
		#region AppointmentInserting
		PersistentObjectCancelEventHandler onAppointmentInserting;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentInserting")]
#endif
		public event PersistentObjectCancelEventHandler AppointmentInserting { add { onAppointmentInserting += value; } remove { onAppointmentInserting -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void OnAppointmentInserting(object sender, PersistentObjectCancelEventArgs e) {
			if (onAppointmentInserting != null) {
				UnsubscribeAutoReloadEvents();
				try {
					onAppointmentInserting(this, e);
				} finally {
					SubscribeAutoReloadEvents();
				}
			}
		}
		#endregion
		#region AppointmentsInserted
		PersistentObjectsEventHandler onAppointmentsInserted;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentsInserted")]
#endif
		public event PersistentObjectsEventHandler AppointmentsInserted { add { onAppointmentsInserted += value; } remove { onAppointmentsInserted -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseAppointmentsInserted(AppointmentBaseCollection appointments) {
			if (onAppointmentsInserted != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(appointments);
				onAppointmentsInserted(this, args);
			}
		}
		#endregion
		#region AppointmentChanging
		PersistentObjectCancelEventHandler onAppointmentChanging;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentChanging")]
#endif
		public event PersistentObjectCancelEventHandler AppointmentChanging { add { onAppointmentChanging += value; } remove { onAppointmentChanging -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void OnAppointmentChanging(object sender, PersistentObjectCancelEventArgs e) {
			if (onAppointmentChanging != null) {
				UnsubscribeAutoReloadEvents();
				try {
					onAppointmentChanging(this, e);
				} finally {
					SubscribeAutoReloadEvents();
				}
			}
		}
		#endregion
		#region AppointmentsChanged
		PersistentObjectsEventHandler onAppointmentsChanged;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentsChanged")]
#endif
		public event PersistentObjectsEventHandler AppointmentsChanged { add { onAppointmentsChanged += value; } remove { onAppointmentsChanged -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseAppointmentsChanged(AppointmentBaseCollection appointments) {
			if (onAppointmentsChanged != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(appointments);
				onAppointmentsChanged(this, args);
			}
		}
		#endregion
		#region AppointmentDeleting
		PersistentObjectCancelEventHandler onAppointmentDeleting;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentDeleting")]
#endif
		public event PersistentObjectCancelEventHandler AppointmentDeleting { add { onAppointmentDeleting += value; } remove { onAppointmentDeleting -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void OnAppointmentDeleting(object sender, PersistentObjectCancelEventArgs e) {
			UnsubscribeAutoReloadEvents();
			if (onAppointmentDeleting != null) {
				try {
					onAppointmentDeleting(this, e);
				} finally {
					SubscribeAutoReloadEvents();
				}
			}
		}
		#endregion
		#region AppointmentsDeleted
		PersistentObjectsEventHandler onAppointmentsDeleted;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentsDeleted")]
#endif
		public event PersistentObjectsEventHandler AppointmentsDeleted { add { onAppointmentsDeleted += value; } remove { onAppointmentsDeleted -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseAppointmentsDeleted(AppointmentBaseCollection appointments) {
			if (onAppointmentsDeleted != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(appointments);
				onAppointmentsDeleted(this, args);
			}
		}
		#endregion
		#region FilterAppointment
		PersistentObjectCancelEventHandler onFilterAppointment;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseFilterAppointment")]
#endif
		public event PersistentObjectCancelEventHandler FilterAppointment { add { onFilterAppointment += value; } remove { onFilterAppointment -= value; } }
		protected virtual bool RaiseFilterAppointment(Appointment apt) {
			if (onFilterAppointment != null) {
				PersistentObjectCancelEventArgs args = new PersistentObjectCancelEventArgs(apt);
				onFilterAppointment(this, args);
				return !args.Cancel;
			} else
				return true;
		}
		bool IInternalSchedulerStorageBase.RaiseFilterAppointment(Appointment apt) {
			return RaiseFilterAppointment(apt);
		}
		#endregion
		event EventHandler<ReminderCancelEventArgs> onFilterReminderAlert;
		public event EventHandler<ReminderCancelEventArgs> FilterReminderAlert { add { onFilterReminderAlert += value; } remove { onFilterReminderAlert -= value; } }
		protected virtual void RaiseFilterReminderAlert(ReminderCancelEventArgs ea) {
			if (onFilterReminderAlert == null)
				return;
			onFilterReminderAlert(this, ea);
		}
		#region ResourceCollectionLoaded
		EventHandler onResourceCollectionLoaded;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseResourceCollectionLoaded")]
#endif
		public event EventHandler ResourceCollectionLoaded { add { onResourceCollectionLoaded += value; } remove { onResourceCollectionLoaded -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseResourceCollectionLoaded() {
			if (onResourceCollectionLoaded != null)
				onResourceCollectionLoaded(this, EventArgs.Empty);
		}
		#endregion
		#region ResourceCollectionCleared
		EventHandler onResourceCollectionCleared;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseResourceCollectionCleared")]
#endif
		public event EventHandler ResourceCollectionCleared { add { onResourceCollectionCleared += value; } remove { onResourceCollectionCleared -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseResourceCollectionCleared() {
			if (onResourceCollectionCleared != null)
				onResourceCollectionCleared(this, EventArgs.Empty);
		}
		#endregion
		#region ResourceCollectionAutoReloading
		CancelListChangedEventHandler onResourceCollectionAutoReloading;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseResourceCollectionAutoReloading")]
#endif
		public event CancelListChangedEventHandler ResourceCollectionAutoReloading { add { onResourceCollectionAutoReloading += value; } remove { onResourceCollectionAutoReloading -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void OnResourcesAutoReloading(object sender, CancelListChangedEventArgs e) {
			if (onResourceCollectionAutoReloading != null)
				onResourceCollectionAutoReloading(this, e);
		}
		#endregion
		#region ResourceInserting
		PersistentObjectCancelEventHandler onResourceInserting;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseResourceInserting")]
#endif
		public event PersistentObjectCancelEventHandler ResourceInserting { add { onResourceInserting += value; } remove { onResourceInserting -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void OnResourceInserting(object sender, PersistentObjectCancelEventArgs e) {
			if (onResourceInserting != null) {
				UnsubscribeAutoReloadEvents();
				try {
					onResourceInserting(this, e);
				} finally {
					SubscribeAutoReloadEvents();
				}
			}
		}
		#endregion
		#region ResourcesInserted
		PersistentObjectsEventHandler onResourcesInserted;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseResourcesInserted")]
#endif
		public event PersistentObjectsEventHandler ResourcesInserted { add { onResourcesInserted += value; } remove { onResourcesInserted -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseResourcesInserted(ResourceBaseCollection resources) {
			if (onResourcesInserted != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(resources);
				onResourcesInserted(this, args);
			}
		}
		#endregion
		#region ResourceChanging
		PersistentObjectCancelEventHandler onResourceChanging;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseResourceChanging")]
#endif
		public event PersistentObjectCancelEventHandler ResourceChanging { add { onResourceChanging += value; } remove { onResourceChanging -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void OnResourceChanging(object sender, PersistentObjectCancelEventArgs e) {
			if (onResourceChanging != null) {
				UnsubscribeAutoReloadEvents();
				try {
					onResourceChanging(this, e);
				} finally {
					SubscribeAutoReloadEvents();
				}
			}
		}
		#endregion
		#region ResourcesChanged
		PersistentObjectsEventHandler onResourcesChanged;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseResourcesChanged")]
#endif
		public event PersistentObjectsEventHandler ResourcesChanged { add { onResourcesChanged += value; } remove { onResourcesChanged -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseResourcesChanged(ResourceBaseCollection resources) {
			if (onResourcesChanged != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(resources);
				onResourcesChanged(this, args);
			}
		}
		#endregion
		#region ResourceDeleting
		PersistentObjectCancelEventHandler onResourceDeleting;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseResourceDeleting")]
#endif
		public event PersistentObjectCancelEventHandler ResourceDeleting { add { onResourceDeleting += value; } remove { onResourceDeleting -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void OnResourceDeleting(object sender, PersistentObjectCancelEventArgs e) {
			if (onResourceDeleting != null) {
				UnsubscribeAutoReloadEvents();
				try {
					onResourceDeleting(this, e);
				} finally {
					SubscribeAutoReloadEvents();
				}
			}
		}
		#endregion
		#region ResourcesDeleted
		PersistentObjectsEventHandler onResourcesDeleted;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseResourcesDeleted")]
#endif
		public event PersistentObjectsEventHandler ResourcesDeleted { add { onResourcesDeleted += value; } remove { onResourcesDeleted -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RaiseResourcesDeleted(ResourceBaseCollection resources) {
			if (onResourcesDeleted != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(resources);
				onResourcesDeleted(this, args);
			}
		}
		#endregion
		#region FilterResource
		PersistentObjectCancelEventHandler onFilterResource;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseFilterResource")]
#endif
		public event PersistentObjectCancelEventHandler FilterResource { add { onFilterResource += value; } remove { onFilterResource -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool RaiseFilterResource(Resource resource) {
			if (onFilterResource != null) {
				PersistentObjectCancelEventArgs args = new PersistentObjectCancelEventArgs(resource);
				onFilterResource(this, args);
				return !args.Cancel;
			} else
				return true;
		}
		#endregion
		#region AppointmentDependencyCollectionLoaded
		EventHandler onAppointmentDependencyCollectionLoaded;
		public event EventHandler AppointmentDependencyCollectionLoaded { add { onAppointmentDependencyCollectionLoaded += value; } remove { onAppointmentDependencyCollectionLoaded -= value; } }
		protected internal virtual void RaiseAppointmentDependencyCollectionLoaded() {
			if (onAppointmentDependencyCollectionLoaded != null)
				onAppointmentDependencyCollectionLoaded(this, EventArgs.Empty);
		}
		#endregion
		#region AppointmentDependencyCollectionCleared
		EventHandler onAppointmentDependencyCollectionCleared;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentDependencyCollectionCleared")]
#endif
		public event EventHandler AppointmentDependencyCollectionCleared { add { onAppointmentDependencyCollectionCleared += value; } remove { onAppointmentDependencyCollectionCleared -= value; } }
		protected internal virtual void RaiseAppointmentDependencyCollectionCleared() {
			if (onAppointmentDependencyCollectionCleared != null)
				onAppointmentDependencyCollectionCleared(this, EventArgs.Empty);
		}
		#endregion
		#region AppointmentDependencyCollectionAutoReloading
		CancelListChangedEventHandler onAppointmentDependencyCollectionAutoReloading;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentDependencyCollectionAutoReloading")]
#endif
		public event CancelListChangedEventHandler AppointmentDependencyCollectionAutoReloading { add { onAppointmentDependencyCollectionAutoReloading += value; } remove { onAppointmentDependencyCollectionAutoReloading -= value; } }
		protected internal virtual void OnAppointmentDependenciesAutoReloading(object sender, CancelListChangedEventArgs e) {
			if (onAppointmentDependencyCollectionAutoReloading != null)
				onAppointmentDependencyCollectionAutoReloading(this, e);
		}
		#endregion
		#region AppointmentDependencyInserting
		PersistentObjectCancelEventHandler onAppointmentDependencyInserting;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentDependencyInserting")]
#endif
		public event PersistentObjectCancelEventHandler AppointmentDependencyInserting { add { onAppointmentDependencyInserting += value; } remove { onAppointmentDependencyInserting -= value; } }
		protected internal virtual void OnAppointmentDependencyInserting(object sender, PersistentObjectCancelEventArgs e) {
			if (onAppointmentDependencyInserting != null) {
				UnsubscribeAutoReloadEvents();
				try {
					onAppointmentDependencyInserting(this, e);
				} finally {
					SubscribeAutoReloadEvents();
				}
			}
		}
		#endregion
		#region AppointmentDependenciesInserted
		PersistentObjectsEventHandler onAppointmentDependenciesInserted;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentDependenciesInserted")]
#endif
		public event PersistentObjectsEventHandler AppointmentDependenciesInserted { add { onAppointmentDependenciesInserted += value; } remove { onAppointmentDependenciesInserted -= value; } }
		protected internal virtual void RaiseAppointmentDependenciesInserted(AppointmentDependencyBaseCollection dependencies) {
			if (onAppointmentDependenciesInserted != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(dependencies);
				onAppointmentDependenciesInserted(this, args);
			}
		}
		#endregion
		#region AppointmentDependencyChanging
		PersistentObjectCancelEventHandler onAppointmentDependencyChanging;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentDependencyChanging")]
#endif
		public event PersistentObjectCancelEventHandler AppointmentDependencyChanging { add { onAppointmentDependencyChanging += value; } remove { onAppointmentDependencyChanging -= value; } }
		protected internal virtual void OnAppointmentDependencyChanging(object sender, PersistentObjectCancelEventArgs e) {
			if (onAppointmentDependencyChanging != null) {
				UnsubscribeAutoReloadEvents();
				try {
					onAppointmentDependencyChanging(sender, e);
				} finally {
					SubscribeAutoReloadEvents();
				}
			}
		}
		#endregion
		#region AppointmentDependenciesChanged
		PersistentObjectsEventHandler onAppointmentDependeciesChanged;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentDependenciesChanged")]
#endif
		public event PersistentObjectsEventHandler AppointmentDependenciesChanged { add { onAppointmentDependeciesChanged += value; } remove { onAppointmentDependeciesChanged -= value; } }
		protected internal virtual void RaiseAppointmentDependenciesChanged(AppointmentDependencyBaseCollection dependencies) {
			if (onAppointmentDependeciesChanged != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(dependencies);
				onAppointmentDependeciesChanged(this, args);
			}
		}
		#endregion
		#region AppointmentDependencyDeleting
		PersistentObjectCancelEventHandler onAppointmentDependencyDeleting;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentDependencyDeleting")]
#endif
		public event PersistentObjectCancelEventHandler AppointmentDependencyDeleting { add { onAppointmentDependencyDeleting += value; } remove { onAppointmentDependencyDeleting -= value; } }
		protected internal virtual void OnAppointmentDependencyDeleting(object sender, PersistentObjectCancelEventArgs e) {
			if (onAppointmentDependencyDeleting != null) {
				UnsubscribeAutoReloadEvents();
				try {
					onAppointmentDependencyDeleting(this, e);
				} finally {
					SubscribeAutoReloadEvents();
				}
			}
		}
		#endregion
		#region AppointmentDependenciesDeleted
		PersistentObjectsEventHandler onAppointmentAppointmentDependenciesDeleted;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseAppointmentDependenciesDeleted")]
#endif
		public event PersistentObjectsEventHandler AppointmentDependenciesDeleted { add { onAppointmentAppointmentDependenciesDeleted += value; } remove { onAppointmentAppointmentDependenciesDeleted -= value; } }
		protected internal virtual void RaiseDependenciesDeleted(AppointmentDependencyBaseCollection dependencies) {
			if (onAppointmentAppointmentDependenciesDeleted != null) {
				PersistentObjectsEventArgs args = new PersistentObjectsEventArgs(dependencies);
				onAppointmentAppointmentDependenciesDeleted(this, args);
			}
		}
		#endregion
		#region FilterDependency
		PersistentObjectCancelEventHandler onFilterDependency;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseFilterDependency")]
#endif
		public event PersistentObjectCancelEventHandler FilterDependency { add { onFilterDependency += value; } remove { onFilterDependency -= value; } }
		public virtual bool RaiseFilterDependency(AppointmentDependency dependency) {
			if (onFilterDependency != null) {
				PersistentObjectCancelEventArgs args = new PersistentObjectCancelEventArgs(dependency);
				onFilterDependency(this, args);
				return !args.Cancel;
			} else
				return true;
		}
		#endregion
		#region ReminderAlert
		ReminderEventHandler onReminderAlert;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseReminderAlert")]
#endif
		public event ReminderEventHandler ReminderAlert { add { onReminderAlert += value; } remove { onReminderAlert -= value; } }
		protected virtual void OnRemindersAlerted(object sender, ReminderEventArgs e) {
			RaiseReminderAlert(e);
			RaiseNotIgnoredInnerReminderAlert(e);
		}
		void IInternalSchedulerStorageBase.OnRemindersAlerted(object sender, ReminderEventArgs e) {
			OnRemindersAlerted(sender, e);
		}
		#endregion
		#region InternalReminderAlert
		ReminderEventHandler onInternalReminderAlert;
		protected internal event ReminderEventHandler InternalReminderAlert { add { onInternalReminderAlert += value; } remove { onInternalReminderAlert -= value; } }
		#endregion
		#region RaiseReminderAlert
		void RaiseReminderAlert(ReminderEventArgs e) {
			if (onReminderAlert != null)
				onReminderAlert(this, e);
		}
		#endregion
		#region RaiseNotIgnoredInnerReminderAlert
		protected internal virtual void RaiseNotIgnoredInnerReminderAlert(ReminderEventArgs e) {
			int count = e.AlertNotifications.Count;
			for (int i = count - 1; i >= 0; i--) {
				ReminderAlertNotification notification = e.AlertNotifications[i];
				if (CanRemoveAlertNotification(notification))
					e.AlertNotifications.RemoveAt(i);
			}
			if (e.AlertNotifications.Count > 0 && onInternalReminderAlert != null)
				onInternalReminderAlert(this, e);
		}
		#endregion
		#region CanRemoveAlertNotification
		protected internal virtual bool CanRemoveAlertNotification(ReminderAlertNotification notification) {
			return notification.Handled && notification.Ignore;
		}
		#endregion
		public void TriggerAlerts() {
			RemindersEngine.TriggerAlerts();
		}
		#region RaiseFetchAppointments
		FetchAppointmentsEventHandler onFetchAppointments;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerStorageBaseFetchAppointments")]
#endif
		public event FetchAppointmentsEventHandler FetchAppointments { add { onFetchAppointments += value; } remove { onFetchAppointments -= value; } }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool RaiseFetchAppointments(TimeInterval interval) {
			if (onFetchAppointments != null) {
				FetchAppointmentsEventArgs args = new FetchAppointmentsEventArgs(interval);
				onFetchAppointments(this, args);
				return args.ForceReloadAppointments;
			} else
				return false;
		}
		#endregion
		#region RaiseAfterFetchAppointments
		EventHandler onAfterFetchAppointments;
		event EventHandler IInternalSchedulerStorageBase.AfterFetchAppointments { add { onAfterFetchAppointments += value; } remove { onAfterFetchAppointments -= value; } }
		void IInternalSchedulerStorageBase.RaiseAfterFetchAppointments() {
			RaiseAfterFetchAppointments();
		}
		protected virtual void RaiseAfterFetchAppointments() {
			if (onAfterFetchAppointments != null)
				onAfterFetchAppointments(this, EventArgs.Empty);
		}
		#endregion
		#region BeforeDispose
		EventHandler onBeforeDispose;
		internal event EventHandler BeforeDispose { add { onBeforeDispose += value; } remove { onBeforeDispose -= value; } }
		protected internal virtual void RaiseBeforeDispose() {
			if (onBeforeDispose != null)
				onBeforeDispose(this, EventArgs.Empty);
		}
		#endregion
		#region UpdateUI
		EventHandler onUpdateUI;
		internal event EventHandler UpdateUI {
			add { onUpdateUI += value; }
			remove { onUpdateUI -= value; }
		}
		protected internal void RaiseUpdateUI() {
			if (this.onUpdateUI != null)
				this.onUpdateUI(this, EventArgs.Empty);
		}
		#endregion
		#region Obsolete events
		[Obsolete("You should use the 'ResourcesInserted' instead.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event PersistentObjectEventHandler ResourceInserted { add { } remove { } }
		[Obsolete("You should use the 'ResourcesChanged' instead.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event PersistentObjectEventHandler ResourceChanged { add { } remove { } }
		[Obsolete("You should use the 'ResourcesDeleted' instead.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event PersistentObjectEventHandler ResourceDeleted { add { } remove { } }
		#endregion
		#endregion
		#region IDisposable implementation
#if !SL
		protected override
#else
		protected virtual
#endif
 void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (!isDisposed)
						RaiseBeforeDispose();
					if (appointments != null) {
						UnsubscribeAppointmentStorageEvents();
						IDisposable disposableAppointments = appointments as IDisposable;
						if (disposableAppointments != null)
							disposableAppointments.Dispose();
						appointments = null;
					}
					if (resources != null) {
						UnsubscribeResourceStorageEvents();
						IDisposable disposableResources = resources as IDisposable;
						if (disposableResources != null)
							disposableResources.Dispose();
						resources = null;
					}
					if (dependencies != null) {
						UnsubscribeDependencyStorageEvents();
						dependencies.Dispose();
						dependencies = null;
					}
					if (remindersEngine != null) {
						UnsubscribeRemindersEngineEvents();
						remindersEngine.Dispose();
						remindersEngine = null;
					}
					if (appointmentCache != null) {
						appointmentCache.DeleteCache(this);
						appointmentCache = null;
					}
					if (appointmentTreeController != null) {
						appointmentTreeController.Dispose();
						appointmentTreeController = null;
					}
					if (appointmentTree != null) {
						appointmentTree = null;
					}
					batchUpdateHelper = null;
				}
			} finally {
#if !SL
				base.Dispose(disposing);
#endif
				isDisposed = true;
			}
		}
#if SL
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SchedulerStorageBase() {
			Dispose(false);
		}
#endif
		#endregion
		#region ISupportInitialize implementation
		public void BeginInit() {
			BeginUpdate();
			((IInternalAppointmentStorage)InnerAppointments).BeginInit();
		}
		public void EndInit() {
			((IInternalAppointmentStorage)InnerAppointments).EndInit();
			EndUpdate();
		}
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
			this.deferredChanges = new SchedulerStorageDeferredChanges();
			this.deferredChanges.EnableReminders = EnableReminders;
			UnsubscribeRemindersEngineEvents();
			remindersEngine.Enabled = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
			appointments.BeginUpdate();
			resources.BeginUpdate();
			dependencies.BeginUpdate();
		}
		void IBatchUpdateHandler.OnEndUpdate() {
			appointments.EndUpdate();
			resources.EndUpdate();
			dependencies.EndUpdate();
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			UnsubscribeDependencyStorageEvents();
			try {
				UnsubscribeResourceStorageEvents();
				try {
					UnsubscribeAppointmentStorageEvents();
					try {
						PerformDeferredLoading();
						SendRemindersEngineDeferredNotifications();
						SendAppointmentCacheDeferredNotifications();
						SendAppointmentTreeDeferredNotifications();
						if (!DeferredChanges.SuppressDeferredNotifications) {
							RaiseDeferredNotifications();
							RaiseInternalDeferredNotifications();
						}
						PerformDeferredDisposing();
					} finally {
						SubscribeAppointmentStorageEvents();
					}
				} finally {
					SubscribeResourceStorageEvents();
				}
			} finally {
				SubscribeDependencyStorageEvents();
			}
			this.EnableReminders = deferredChanges.EnableReminders;
			this.deferredChanges = null;
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
			appointments.CancelUpdate();
			resources.CancelUpdate();
			dependencies.CancelUpdate();
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			UnsubscribeDependencyStorageEvents();
			try {
				UnsubscribeResourceStorageEvents();
				try {
					UnsubscribeAppointmentStorageEvents();
					try {
						PerformDeferredLoading();
						SendRemindersEngineDeferredNotifications();
						SendAppointmentCacheDeferredNotifications();
						SendAppointmentTreeDeferredNotifications();
						if (!DeferredChanges.SuppressDeferredNotifications)
							RaiseInternalDeferredNotifications();
						PerformDeferredDisposing();
					} finally {
						SubscribeAppointmentStorageEvents();
					}
				} finally {
					SubscribeResourceStorageEvents();
				}
			} finally {
				SubscribeDependencyStorageEvents();
			}
			EnableReminders = deferredChanges.EnableReminders;
			this.deferredChanges = null;
		}
		#endregion
		#region SubscribeRemindersEngineEvents
		protected internal virtual void SubscribeRemindersEngineEvents() {
			RemindersEngine.ReminderAlert += new ReminderEventHandler(OnRemindersAlerted);
			RemindersEngine.FilterReminder += OnFilterReminder;
		}
		#endregion
		#region UnsubscribeRemindersEngineEvents
		protected internal virtual void UnsubscribeRemindersEngineEvents() {
			RemindersEngine.ReminderAlert -= new ReminderEventHandler(OnRemindersAlerted);
			RemindersEngine.FilterReminder -= OnFilterReminder;
		}
		#endregion
		#region SubscribeAppointmentStorageEvents
		protected internal virtual void SubscribeAppointmentStorageEvents() {
			IInternalAppointmentStorage internalInnerAppointments = (IInternalAppointmentStorage)InnerAppointments;
			internalInnerAppointments.AutoReloading += new CancelListChangedEventHandler(OnAppointmentsAutoReloading);
			internalInnerAppointments.Reload += new PersistentObjectStorageReloadEventHandler(OnAppointmentsReload);
			internalInnerAppointments.ObjectCollectionCleared += new EventHandler(OnAppointmentCollectionCleared);
			internalInnerAppointments.ObjectCollectionLoaded += new EventHandler(OnAppointmentCollectionLoaded);
			internalInnerAppointments.ObjectInserting += new PersistentObjectCancelEventHandler(OnAppointmentInserting);
			internalInnerAppointments.ObjectDeleting += new PersistentObjectCancelEventHandler(OnAppointmentDeleting);
			internalInnerAppointments.InternalObjectChanging += OnInternalAppointmentChanging;
			internalInnerAppointments.ObjectChanging += new PersistentObjectCancelEventHandler(OnAppointmentChanging);
			internalInnerAppointments.ObjectInserted += new PersistentObjectEventHandler(OnAppointmentInserted);
			internalInnerAppointments.ObjectDeleted += new PersistentObjectEventHandler(OnAppointmentDeleted);
			internalInnerAppointments.ObjectChanged += new PersistentObjectEventHandler(OnAppointmentChanged);
			internalInnerAppointments.MappingsChanged += new EventHandler(OnAppointmentMappingsChanged);
			internalInnerAppointments.UIChanged += new EventHandler(OnAppointmentUIObjectsChanged);
			internalInnerAppointments.FilterChanged += new EventHandler(OnAppointmentFilterChanged);
#if SL
			SubscribeAppointmentsTransactionCompleted();
#endif
		}
		#endregion
		internal void SubscribeAppointmentsTransactionCompleted() {
			((IInternalAppointmentStorage)InnerAppointments).AppointmentsTransactionCompleted += new AppointmentsTransactionEventHandler(OnAppointmentsTransactionCompleted);
		}
		#region UnsubscribeAppointmentStorageEvents
		protected internal virtual void UnsubscribeAppointmentStorageEvents() {
			IInternalAppointmentStorage internalInnerAppointments = (IInternalAppointmentStorage)InnerAppointments;
			internalInnerAppointments.AutoReloading -= new CancelListChangedEventHandler(OnAppointmentsAutoReloading);
			internalInnerAppointments.Reload -= new PersistentObjectStorageReloadEventHandler(OnAppointmentsReload);
			internalInnerAppointments.ObjectCollectionCleared -= new EventHandler(OnAppointmentCollectionCleared);
			internalInnerAppointments.ObjectCollectionLoaded -= new EventHandler(OnAppointmentCollectionLoaded);
			internalInnerAppointments.ObjectInserting -= new PersistentObjectCancelEventHandler(OnAppointmentInserting);
			internalInnerAppointments.ObjectDeleting -= new PersistentObjectCancelEventHandler(OnAppointmentDeleting);
			internalInnerAppointments.InternalObjectChanging -= OnInternalAppointmentChanging;
			internalInnerAppointments.ObjectChanging -= new PersistentObjectCancelEventHandler(OnAppointmentChanging);
			internalInnerAppointments.ObjectInserted -= new PersistentObjectEventHandler(OnAppointmentInserted);
			internalInnerAppointments.ObjectDeleted -= new PersistentObjectEventHandler(OnAppointmentDeleted);
			internalInnerAppointments.ObjectChanged -= new PersistentObjectEventHandler(OnAppointmentChanged);
			internalInnerAppointments.MappingsChanged -= new EventHandler(OnAppointmentMappingsChanged);
			internalInnerAppointments.UIChanged -= new EventHandler(OnAppointmentUIObjectsChanged);
			internalInnerAppointments.FilterChanged -= new EventHandler(OnAppointmentFilterChanged);
#if SL
			UnsubscribeAppointmentsTransactionCompleted();
#endif
		}
		#endregion
		internal void UnsubscribeAppointmentsTransactionCompleted() {
			((IInternalAppointmentStorage)InnerAppointments).AppointmentsTransactionCompleted -= new AppointmentsTransactionEventHandler(OnAppointmentsTransactionCompleted);
		}
		#region SubscribeResourceStorageEvents
		protected internal virtual void SubscribeResourceStorageEvents() {
			if (this.resourceStorageEventsSubscribtionLocker.IsLocked)
				return;
			IInternalResourceStorage internalInnerResources = (IInternalResourceStorage)InnerResources;
			internalInnerResources.AutoReloading += new CancelListChangedEventHandler(OnResourcesAutoReloading);
			internalInnerResources.Reload += new PersistentObjectStorageReloadEventHandler(OnResourcesReload);
			internalInnerResources.ObjectCollectionCleared += new EventHandler(OnResourceCollectionCleared);
			internalInnerResources.ObjectCollectionLoaded += new EventHandler(OnResourceCollectionLoaded);
			internalInnerResources.ObjectInserting += new PersistentObjectCancelEventHandler(OnResourceInserting);
			internalInnerResources.ObjectDeleting += new PersistentObjectCancelEventHandler(OnResourceDeleting);
			internalInnerResources.ObjectChanging += new PersistentObjectCancelEventHandler(OnResourceChanging);
			internalInnerResources.ObjectInserted += new PersistentObjectEventHandler(OnResourceInserted);
			internalInnerResources.ObjectDeleted += new PersistentObjectEventHandler(OnResourceDeleted);
			internalInnerResources.ObjectChanged += new PersistentObjectEventHandler(OnResourceChanged);
			internalInnerResources.ResourceVisibilityChanged += new PersistentObjectEventHandler(OnResourceVisibilityChanged);
			internalInnerResources.MappingsChanged += new EventHandler(OnResourceMappingsChanged);
			internalInnerResources.FilterChanged += new EventHandler(OnResourceFilterChanged);
			internalInnerResources.SortedColumnsChanged += new EventHandler(OnResourceSortedColumnsChange);
		}
		#endregion
		#region UnsubscribeResourceStorageEvents
		protected internal virtual void UnsubscribeResourceStorageEvents() {
			if (this.resourceStorageEventsSubscribtionLocker.IsLocked)
				return;
			IInternalResourceStorage internalInnerResources = (IInternalResourceStorage)InnerResources;
			internalInnerResources.AutoReloading -= new CancelListChangedEventHandler(OnResourcesAutoReloading);
			internalInnerResources.Reload -= new PersistentObjectStorageReloadEventHandler(OnResourcesReload);
			internalInnerResources.ObjectCollectionCleared -= new EventHandler(OnResourceCollectionCleared);
			internalInnerResources.ObjectCollectionLoaded -= new EventHandler(OnResourceCollectionLoaded);
			internalInnerResources.ObjectInserting -= new PersistentObjectCancelEventHandler(OnResourceInserting);
			internalInnerResources.ObjectDeleting -= new PersistentObjectCancelEventHandler(OnResourceDeleting);
			internalInnerResources.ObjectChanging -= new PersistentObjectCancelEventHandler(OnResourceChanging);
			internalInnerResources.ObjectInserted -= new PersistentObjectEventHandler(OnResourceInserted);
			internalInnerResources.ObjectDeleted -= new PersistentObjectEventHandler(OnResourceDeleted);
			internalInnerResources.ObjectChanged -= new PersistentObjectEventHandler(OnResourceChanged);
			internalInnerResources.ResourceVisibilityChanged -= new PersistentObjectEventHandler(OnResourceVisibilityChanged);
			internalInnerResources.MappingsChanged -= new EventHandler(OnResourceMappingsChanged);
			internalInnerResources.FilterChanged -= new EventHandler(OnResourceFilterChanged);
			internalInnerResources.SortedColumnsChanged -= new EventHandler(OnResourceSortedColumnsChange);
		}
		#endregion
		#region SubscribeDependencyStorageEvents
		protected internal virtual void SubscribeDependencyStorageEvents() {
			IInternalAppointmentDependencyStorage internalInnerAppointmentDependencies = (IInternalAppointmentDependencyStorage)InnerAppointmentDependencies;
			internalInnerAppointmentDependencies.AutoReloading += new CancelListChangedEventHandler(OnAppointmentDependenciesAutoReloading);
			internalInnerAppointmentDependencies.Reload += new PersistentObjectStorageReloadEventHandler(OnDependenciesReload);
			internalInnerAppointmentDependencies.ObjectCollectionCleared += new EventHandler(OnDependencyCollectionCleared);
			internalInnerAppointmentDependencies.ObjectCollectionLoaded += new EventHandler(OnDependencyCollectionLoaded);
			internalInnerAppointmentDependencies.ObjectInserting += new PersistentObjectCancelEventHandler(OnAppointmentDependencyInserting);
			internalInnerAppointmentDependencies.ObjectDeleting += new PersistentObjectCancelEventHandler(OnAppointmentDependencyDeleting);
			internalInnerAppointmentDependencies.ObjectChanging += new PersistentObjectCancelEventHandler(OnAppointmentDependencyChanging);
			internalInnerAppointmentDependencies.ObjectInserted += new PersistentObjectEventHandler(OnDependencyInserted);
			internalInnerAppointmentDependencies.ObjectDeleted += new PersistentObjectEventHandler(OnDependencyDeleted);
			internalInnerAppointmentDependencies.ObjectChanged += new PersistentObjectEventHandler(OnDependencyChanged);
			internalInnerAppointmentDependencies.MappingsChanged += new EventHandler(OnDependencyMappingsChanged);
			internalInnerAppointmentDependencies.UIChanged += new EventHandler(OnDependencyUIObjectsChanged);
			internalInnerAppointmentDependencies.FilterChanged += new EventHandler(OnDependencyFilterChanged);
		}
		#endregion
		#region UnsubscribeDependencyStorageEvents
		protected internal virtual void UnsubscribeDependencyStorageEvents() {
			IInternalAppointmentDependencyStorage internalInnerAppointmentDependencies = (IInternalAppointmentDependencyStorage)InnerAppointmentDependencies;
			internalInnerAppointmentDependencies.AutoReloading -= new CancelListChangedEventHandler(OnAppointmentDependenciesAutoReloading);
			internalInnerAppointmentDependencies.Reload -= new PersistentObjectStorageReloadEventHandler(OnDependenciesReload);
			internalInnerAppointmentDependencies.ObjectCollectionCleared -= new EventHandler(OnDependencyCollectionCleared);
			internalInnerAppointmentDependencies.ObjectCollectionLoaded -= new EventHandler(OnDependencyCollectionLoaded);
			internalInnerAppointmentDependencies.ObjectInserting -= new PersistentObjectCancelEventHandler(OnAppointmentDependencyInserting);
			internalInnerAppointmentDependencies.ObjectDeleting -= new PersistentObjectCancelEventHandler(OnAppointmentDependencyDeleting);
			internalInnerAppointmentDependencies.ObjectChanging -= new PersistentObjectCancelEventHandler(OnAppointmentDependencyChanging);
			internalInnerAppointmentDependencies.ObjectInserted -= new PersistentObjectEventHandler(OnDependencyInserted);
			internalInnerAppointmentDependencies.ObjectDeleted -= new PersistentObjectEventHandler(OnDependencyDeleted);
			internalInnerAppointmentDependencies.ObjectChanged -= new PersistentObjectEventHandler(OnDependencyChanged);
			internalInnerAppointmentDependencies.MappingsChanged -= new EventHandler(OnDependencyMappingsChanged);
			internalInnerAppointmentDependencies.UIChanged -= new EventHandler(OnDependencyUIObjectsChanged);
			internalInnerAppointmentDependencies.FilterChanged -= new EventHandler(OnDependencyFilterChanged);
		}
		#endregion
		#region SubscribeAutoReloadEvents
		protected internal virtual void SubscribeAutoReloadEvents() {
			IInternalResourceStorage internalInnerResources = (IInternalResourceStorage)InnerResources;
			internalInnerResources.AutoReloading += new CancelListChangedEventHandler(OnResourcesAutoReloading);
			internalInnerResources.Reload += new PersistentObjectStorageReloadEventHandler(OnResourcesReload);
			IInternalAppointmentStorage internalInnerAppointments = (IInternalAppointmentStorage)InnerAppointments;
			internalInnerAppointments.AutoReloading += new CancelListChangedEventHandler(OnAppointmentsAutoReloading);
			internalInnerAppointments.Reload += new PersistentObjectStorageReloadEventHandler(OnAppointmentsReload);
			IInternalAppointmentDependencyStorage internalInnerAppointmentDependencies = (IInternalAppointmentDependencyStorage)InnerAppointmentDependencies;
			internalInnerAppointmentDependencies.AutoReloading += new CancelListChangedEventHandler(OnAppointmentDependenciesAutoReloading);
			internalInnerAppointmentDependencies.Reload += new PersistentObjectStorageReloadEventHandler(OnDependenciesReload);
		}
		#endregion
		#region UnsubscribeAutoReloadEvents
		protected internal virtual void UnsubscribeAutoReloadEvents() {
			IInternalResourceStorage internalInnerResources = (IInternalResourceStorage)InnerResources;
			internalInnerResources.AutoReloading -= new CancelListChangedEventHandler(OnResourcesAutoReloading);
			internalInnerResources.Reload -= new PersistentObjectStorageReloadEventHandler(OnResourcesReload);
			IInternalAppointmentStorage internalInnerAppointments = (IInternalAppointmentStorage)InnerAppointments;
			internalInnerAppointments.AutoReloading -= new CancelListChangedEventHandler(OnAppointmentsAutoReloading);
			internalInnerAppointments.Reload -= new PersistentObjectStorageReloadEventHandler(OnAppointmentsReload);
			IInternalAppointmentDependencyStorage internalInnerAppointmentDependencies = (IInternalAppointmentDependencyStorage)InnerAppointmentDependencies;
			internalInnerAppointmentDependencies.AutoReloading -= new CancelListChangedEventHandler(OnAppointmentDependenciesAutoReloading);
			internalInnerAppointmentDependencies.Reload -= new PersistentObjectStorageReloadEventHandler(OnDependenciesReload);
		}
		#endregion
		protected internal virtual ReminderEngine CreateRemindersEngine() {
			return new ReminderEngine();
		}
		protected virtual AppointmentMultiClientCache CreateAppointmentCache() {
			return new AppointmentMultiClientCache();
		}
		protected virtual AppointmentTree CreateAppointmentTree() {
			return new AppointmentTree();
		}
		protected virtual AppointmentTreeController CreateAppointmentTreeController() {
			return new AppointmentTreeController(AppointmentTree);
		}
		protected virtual void OnFilterReminder(object sender, ReminderCancelEventArgs e) {
			RaiseFilterReminderAlert(e);
		}
		protected internal abstract AppointmentStorageBase CreateAppointmentStorage();
		protected internal abstract ResourceStorageBase CreateResourceStorage();
		protected internal abstract AppointmentDependencyStorageBase CreateAppointmentDependencyStorage();
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void SetAppointmentStorage(IAppointmentStorageBase appointmentStorage) {
			InnerAppointments = appointmentStorage;
		}
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void SetResourceStorage(IResourceStorageBase resourceStorage) {
			InnerResources = resourceStorage;
		}
#if !SL
		#region Import/Export/Synchronization
		public virtual void ImportFromOutlook() {
			AppointmentImporter importer = CreateOutlookImporter();
			if (importer != null) {
				using (MemoryStream stream = new MemoryStream()) {
					importer.Import(stream);
				}
			}
		}
		public virtual void ExportToOutlook() {
			AppointmentExporter exporter = CreateOutlookExporter();
			if (exporter != null) {
				using (MemoryStream stream = new MemoryStream()) {
					exporter.Export(stream);
				}
			}
		}
		public virtual void SynchronizeStorageWithOutlook(string outlookEntryIdFieldName) {
			AppointmentImportSynchronizer synchronizer = CreateOutlookImportSynchronizer();
			if (synchronizer != null) {
				synchronizer.ForeignIdFieldName = outlookEntryIdFieldName;
				synchronizer.Synchronize();
			}
		}
		public virtual void SynchronizeOutlookWithStorage(string outlookEntryIdFieldName) {
			AppointmentExportSynchronizer synchronizer = CreateOutlookExportSynchronizer();
			if (synchronizer != null) {
				synchronizer.ForeignIdFieldName = outlookEntryIdFieldName;
				synchronizer.Synchronize();
			}
		}
		public virtual AppointmentImporter CreateOutlookImporter() {
			return ExchangeHelper.CreateImporter(this, ExchangeHelper.GetOutlookAssemblyName(), "DevExpress.XtraScheduler.Outlook.OutlookImport");
		}
		public virtual AppointmentExporter CreateOutlookExporter() {
			return ExchangeHelper.CreateExporter(this, ExchangeHelper.GetOutlookAssemblyName(), "DevExpress.XtraScheduler.Outlook.OutlookExport");
		}
		public virtual AppointmentImportSynchronizer CreateOutlookImportSynchronizer() {
			return ExchangeHelper.CreateImportSynchronizer(this, ExchangeHelper.GetOutlookAssemblyName(), "DevExpress.XtraScheduler.Outlook.OutlookImportSynchronizer");
		}
		public virtual AppointmentExportSynchronizer CreateOutlookExportSynchronizer() {
			return ExchangeHelper.CreateExportSynchronizer(this, ExchangeHelper.GetOutlookAssemblyName(), "DevExpress.XtraScheduler.Outlook.OutlookExportSynchronizer");
		}
		public void ImportFromVCalendar(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				ImportFromVCalendar(fs);
			}
		}
		public void ImportFromVCalendar(Stream stream) {
			AppointmentImporter importer = ExchangeHelper.CreateImporter(this, ExchangeHelper.GetVCalendarAssemblyName(), "DevExpress.XtraScheduler.VCalendar.VCalendarImporter");
			if (importer != null)
				importer.Import(stream);
		}
		public void ExportToVCalendar(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				ExportToVCalendar(fs);
			}
		}
		public void ExportToVCalendar(Stream stream) {
			AppointmentExporter exporter = ExchangeHelper.CreateExporter(this, ExchangeHelper.GetVCalendarAssemblyName(), "DevExpress.XtraScheduler.VCalendar.VCalendarExporter");
			if (exporter != null)
				exporter.Export(stream);
		}
		public void ImportFromICalendar(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				ImportFromICalendar(fs);
			}
		}
		public void ImportFromICalendar(Stream stream) {
			AppointmentImporter importer = ExchangeHelper.CreateImporter(this, ExchangeHelper.GetICalendarAssemblyName(), "DevExpress.XtraScheduler.iCalendar.iCalendarImporter");
			if (importer != null)
				importer.Import(stream);
		}
		public void ExportToICalendar(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				ExportToICalendar(fs);
			}
		}
		public void ExportToICalendar(Stream stream) {
			AppointmentExporter exporter = ExchangeHelper.CreateExporter(this, ExchangeHelper.GetICalendarAssemblyName(), "DevExpress.XtraScheduler.iCalendar.iCalendarExporter");
			if (exporter != null)
				exporter.Export(stream);
		}
		#endregion
#endif
		#region SendRemindersEngineDeferredNotifications
		protected internal virtual void SendRemindersEngineDeferredNotifications() {
			if (deferredChanges.ClearAppointments)
				remindersEngine.OnAppointmentCollectionCleared();
			if (deferredChanges.RaiseAppointmentsLoaded)
				remindersEngine.OnAppointmentCollectionLoaded(InnerAppointments.Items);
			if (deferredChanges.DeletedAppointments.Count > 0)
				remindersEngine.OnAppointmentsDeferredDeleted(deferredChanges.DeletedAppointments);
			if (deferredChanges.InsertedAppointments.Count > 0)
				remindersEngine.OnAppointmentsInserted(deferredChanges.InsertedAppointments);
			if (deferredChanges.ChangedAppointments.Count > 0)
				remindersEngine.OnAppointmentsChanged(deferredChanges.ChangedAppointments);
		}
		#endregion
		#region SendAppointmentCacheDeferredNotifications
		protected internal virtual void SendAppointmentCacheDeferredNotifications() {
			appointmentCache.BeginUpdate();
			try {
				if (deferredChanges.ClearAppointments)
					appointmentCache.OnAppointmentCollectionCleared();
				if (deferredChanges.RaiseAppointmentsLoaded)
					appointmentCache.OnAppointmentCollectionLoaded();
				if (deferredChanges.DeletedAppointments.Count > 0)
					appointmentCache.OnAppointmentsDeleted(deferredChanges.DeletedAppointments);
				if (deferredChanges.InsertedAppointments.Count > 0)
					appointmentCache.OnAppointmentsInserted(deferredChanges.InsertedAppointments);
				if (deferredChanges.ChangedAppointments.Count > 0)
					appointmentCache.OnAppointmentsChanged(deferredChanges.ChangedAppointments);
			} finally {
				appointmentCache.EndUpdate();
			}
		}
		#endregion
		#region SendAppointmentTreeDeferredNotifications
		protected internal virtual void SendAppointmentTreeDeferredNotifications() {
			if (deferredChanges.ClearAppointments)
				AppointmentTreeController.OnAppointmentCollectionCleared();
			if (deferredChanges.RaiseAppointmentsLoaded)
				appointmentTreeController.OnAppointmentCollectionLoaded(InnerAppointments.Items);
			if (deferredChanges.DeletedAppointments.Count > 0)
				appointmentTreeController.OnAppointmentsDeleted(deferredChanges.DeletedAppointments);
			if (deferredChanges.InsertedAppointments.Count > 0)
				AppointmentTreeController.OnAppointmentsInserted(deferredChanges.InsertedAppointments);
			if (deferredChanges.ChangedAppointments.Count > 0)
				AppointmentTreeController.OnAppointmentsChanged(deferredChanges.ChangedAppointments);
		}
		#endregion
		#region RaiseDeferredNotifications
		protected internal virtual void RaiseDeferredNotifications() {
			if (deferredChanges.ClearResources)
				RaiseResourceCollectionCleared();
			if (deferredChanges.RaiseResourcesLoaded)
				RaiseResourceCollectionLoaded();
			if (deferredChanges.DeletedResources.Count > 0)
				RaiseResourcesDeleted(deferredChanges.DeletedResources);
			if (deferredChanges.InsertedResources.Count > 0)
				RaiseResourcesInserted(deferredChanges.InsertedResources);
			if (deferredChanges.ChangedResources.Count > 0)
				RaiseResourcesChanged(deferredChanges.ChangedResources);
			if (deferredChanges.ClearAppointments)
				RaiseAppointmentCollectionCleared();
			if (deferredChanges.RaiseAppointmentsLoaded)
				RaiseAppointmentCollectionLoaded();
			if (deferredChanges.DeletedAppointments.Count > 0)
				RaiseAppointmentsDeleted(deferredChanges.DeletedAppointments);
			if (deferredChanges.InsertedAppointments.Count > 0)
				RaiseAppointmentsInserted(deferredChanges.InsertedAppointments);
			if (deferredChanges.ChangedAppointments.Count > 0)
				RaiseAppointmentsChanged(deferredChanges.ChangedAppointments);
			if (deferredChanges.RaiseAppointmentsModified)
				RaiseAppointmentCollectionModified();
			if (deferredChanges.ClearDependencies)
				RaiseAppointmentDependencyCollectionCleared();
			if (deferredChanges.RaiseDependenciesLoaded)
				RaiseAppointmentDependencyCollectionLoaded();
			if (deferredChanges.DeletedDependencies.Count > 0)
				RaiseDependenciesDeleted(deferredChanges.DeletedDependencies);
			if (deferredChanges.InsertedDependencies.Count > 0)
				RaiseAppointmentDependenciesInserted(deferredChanges.InsertedDependencies);
			if (deferredChanges.ChangedDependencies.Count > 0)
				RaiseAppointmentDependenciesChanged(deferredChanges.ChangedDependencies);
		}
		#endregion
		protected internal virtual void PerformDeferredLoading() {
			if (deferredChanges.LoadResources) {
				ValidateResourcesDataSource();
				LoadResourcesCore(this.deferredChanges.KeepResourcesNonPersistentInformation);
			}
			if (deferredChanges.LoadAppointments) {
				ValidateAppointmentsDataSource();
				ClearAppointmentDeferredChanges(this.deferredChanges);
				LoadAppointmentsCore(this.deferredChanges.KeepAppointmentsNonPersistentInformation);
			}
			if (deferredChanges.LoadDependencies) {
				ValidateDependenciesDataSource();
				LoadDependenciesCore(this.deferredChanges.KeepDependenciesNonPersistentInformation);
			}
		}
		void ClearAppointmentDeferredChanges(SchedulerStorageDeferredChanges deferredChanges) {
			deferredChanges.ChangedAppointments.Clear();
			deferredChanges.InsertedAppointments.Clear();
			deferredChanges.DeletedAppointments.Clear();
		}
		protected internal virtual void PerformDeferredDisposing() {
			deferredChanges.DeletedAppointments.ForEach(DisposeObject);
			deferredChanges.DeletedResources.ForEach(DisposeObject);
			deferredChanges.DeletedDependencies.ForEach(DisposeObject);
		}
		protected internal virtual void DisposeObject(IDisposable obj) {
			obj.Dispose();
		}
		protected internal virtual void OnAppointmentsReload(object sender, PersistentObjectStorageReloadEventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterLoadAppointments(e.KeepNonPersistentInformation);
			else {
				ValidateAppointmentsDataSource();
				LoadAppointments(e.KeepNonPersistentInformation);
			}
		}
		protected internal virtual void ValidateAppointmentsDataSource() {
			InnerAppointments.ValidateDataSource();
		}
		protected internal virtual void OnResourcesReload(object sender, PersistentObjectStorageReloadEventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterLoadResources(e.KeepNonPersistentInformation);
			else {
				ValidateResourcesDataSource();
				LoadResources(e.KeepNonPersistentInformation);
			}
			OnAppointmentsReload(sender, e);
		}
		protected internal virtual void ValidateResourcesDataSource() {
			InnerResources.ValidateDataSource();
		}
		protected internal virtual void OnDependenciesReload(object sender, PersistentObjectStorageReloadEventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterLoadDependencies(e.KeepNonPersistentInformation);
			else {
				ValidateDependenciesDataSource();
				LoadDependencies(e.KeepNonPersistentInformation);
			}
		}
		protected internal virtual void ValidateDependenciesDataSource() {
			InnerAppointmentDependencies.ValidateDataSource();
		}
		protected internal virtual void OnAppointmentInserted(object sender, PersistentObjectEventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterInsertedAppointment((Appointment)e.Object);
			else {
				AppointmentBaseCollection appointments = new AppointmentBaseCollection();
				appointments.Add((Appointment)e.Object);
				appointmentCache.OnAppointmentsInserted(appointments);
				appointmentTreeController.OnAppointmentsInserted(appointments);
				remindersEngine.OnAppointmentsInserted(appointments);
				UnsubscribeAutoReloadEvents();
				try {
					RaiseAppointmentsInserted(appointments);
				} finally {
					SubscribeAutoReloadEvents();
				}
				RaiseInternalAppointmentsInserted(appointments);
			}
		}
		protected internal virtual void OnAppointmentDeleted(object sender, PersistentObjectEventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterDeletedAppointment((Appointment)e.Object);
			else {
				AppointmentBaseCollection appointments = new AppointmentBaseCollection();
				appointments.Add((Appointment)e.Object);
				appointmentCache.OnAppointmentsDeleted(appointments);
				appointmentTreeController.OnAppointmentsDeleted(appointments);
				remindersEngine.OnAppointmentsDeleted(appointments);
				UnsubscribeAutoReloadEvents();
				try {
					RaiseAppointmentsDeleted(appointments);
				} finally {
					SubscribeAutoReloadEvents();
				}
				RaiseInternalAppointmentsDeleted(appointments);
				e.Object.Dispose(); 
			}
		}
		protected internal virtual void OnInternalAppointmentChanging(object sender, PersistentObjectStateChangingEventArgs e) {
			Appointment apt = (Appointment)e.Object;
			XtraSchedulerDebug.Assert(apt.Type == AppointmentType.Normal);
			AppointmentChangeStateData data = ((IInternalAppointment)apt).GetChangeStateData();
			XtraSchedulerDebug.Assert(data != null);
			AppointmentTreeController.OnAppointmentsChanging(apt, data);
		}
		protected internal virtual void OnAppointmentChanged(object sender, PersistentObjectEventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterChangedAppointment((Appointment)e.Object);
			else {
				AppointmentBaseCollection appointments = new AppointmentBaseCollection();
				appointments.Add((Appointment)e.Object);
				appointmentCache.OnAppointmentsChanged(appointments);
				AppointmentTreeController.OnAppointmentsChanged(appointments);
				remindersEngine.OnAppointmentsChanged(appointments);
				UnsubscribeAutoReloadEvents();
				try {
					RaiseAppointmentsChanged(appointments);
				} finally {
					SubscribeAutoReloadEvents();
				}
				RaiseInternalAppointmentsChanged(appointments);
			}
		}
		protected internal virtual void OnAppointmentCollectionCleared(object sender, EventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterClearAppointments();
			else {
				appointmentCache.OnAppointmentCollectionCleared();
				appointmentTreeController.OnAppointmentCollectionCleared();
				remindersEngine.OnAppointmentCollectionCleared();
				UnsubscribeAutoReloadEvents();
				try {
					RaiseAppointmentCollectionCleared();
				} finally {
					SubscribeAutoReloadEvents();
				}
				RaiseInternalAppointmentCollectionCleared();
			}
		}
		protected internal virtual void OnAppointmentCollectionLoaded(object sender, EventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterRaiseAppointmentCollectionLoaded();
			else {
				appointmentCache.OnAppointmentCollectionLoaded();
				appointmentTreeController.OnAppointmentCollectionLoaded(InnerAppointments.Items);
				remindersEngine.OnAppointmentCollectionLoaded(InnerAppointments.Items);
				UnsubscribeAutoReloadEvents();
				try {
					RaiseAppointmentCollectionLoaded();
				} finally {
					SubscribeAutoReloadEvents();
				}
				RaiseInternalAppointmentCollectionLoaded();
			}
		}
		protected internal virtual void OnAppointmentMappingsChanged(object sender, EventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterAppointmentMappingsChanged();
			else
				RaiseInternalAppointmentMappingsChanged();
		}
		protected internal virtual void OnAppointmentUIObjectsChanged(object sender, EventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterAppointmentUIObjectsChanged();
			else
				RaiseInternalAppointmentUIObjectsChanged();
		}
		protected internal virtual void OnResourceInserted(object sender, PersistentObjectEventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterInsertedResource((Resource)e.Object);
			else {
				ResourceBaseCollection resources = new ResourceBaseCollection();
				resources.Add((Resource)e.Object);
				UnsubscribeAutoReloadEvents();
				try {
					RaiseResourcesInserted(resources);
				} finally {
					SubscribeAutoReloadEvents();
				}
				RaiseInternalResourcesInserted(resources);
			}
		}
		protected internal virtual void OnResourceDeleted(object sender, PersistentObjectEventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterDeletedResource((Resource)e.Object);
			else {
				ResourceBaseCollection resources = new ResourceBaseCollection();
				resources.Add((Resource)e.Object);
				UnsubscribeAutoReloadEvents();
				try {
					RaiseResourcesDeleted(resources);
				} finally {
					SubscribeAutoReloadEvents();
				}
				RaiseInternalResourcesDeleted(resources);
				e.Object.Dispose(); 
			}
		}
		protected internal virtual void OnResourceChanged(object sender, PersistentObjectEventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterChangedResource((Resource)e.Object);
			else {
				ResourceBaseCollection resources = new ResourceBaseCollection();
				resources.Add((Resource)e.Object);
				UnsubscribeAutoReloadEvents();
				try {
					RaiseResourcesChanged(resources);
				} finally {
					SubscribeAutoReloadEvents();
				}
				RaiseInternalResourcesChanged(resources);
			}
		}
		protected internal virtual void OnResourceCollectionCleared(object sender, EventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterClearResources();
			else {
				UnsubscribeAutoReloadEvents();
				try {
					RaiseResourceCollectionCleared();
				} finally {
					SubscribeAutoReloadEvents();
				}
				RaiseInternalResourceCollectionCleared();
			}
		}
		protected internal virtual void OnResourceCollectionLoaded(object sender, EventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterRaiseResourceCollectionLoaded();
			else {
				UnsubscribeAutoReloadEvents();
				try {
					RaiseResourceCollectionLoaded();
				} finally {
					SubscribeAutoReloadEvents();
				}
				RaiseInternalResourceCollectionLoaded();
			}
		}
		protected internal virtual void OnResourceVisibilityChanged(object sender, PersistentObjectEventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterResourceVisibilityChange();
			else
				RaiseInternalResourceVisibilityChanged();
		}
		protected internal virtual void OnResourceSortedColumnsChange(object sender, EventArgs e) {
			if (!IsUpdateLocked)
				RaiseInternalResourceSortedColumnsChange();
		}
		protected internal virtual void OnResourceMappingsChanged(object sender, EventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterResourceMappingsChanged();
			else
				RaiseInternalResourceMappingsChanged();
		}
		protected internal virtual void OnAppointmentFilterChanged(object sender, EventArgs e) {
			if (!IsUpdateLocked)
				RaiseInternalAppointmentCollectionLoaded();
		}
		protected internal virtual void OnAppointmentsTransactionCompleted(object sender, AppointmentsTransactionEventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterRaiseAppointmentCollectionModified();
			else {
				RaiseAppointmentCollectionModified();
			}
		}
		protected internal virtual void OnResourceFilterChanged(object sender, EventArgs e) {
			if (!IsUpdateLocked)
				RaiseInternalResourceCollectionLoaded();
		}
		protected internal virtual void OnDependencyFilterChanged(object sender, EventArgs e) {
			if (!IsUpdateLocked)
				RaiseInternalDependencyCollectionLoaded();
		}
		protected internal virtual void OnDependencyInserted(object sender, PersistentObjectEventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterInsertedDependency((AppointmentDependency)e.Object);
			else {
				AppointmentDependencyBaseCollection dependencies = new AppointmentDependencyBaseCollection();
				dependencies.Add((AppointmentDependency)e.Object);
				UnsubscribeAutoReloadEvents();
				try {
					RaiseAppointmentDependenciesInserted(dependencies);
				} finally {
					SubscribeAutoReloadEvents();
				}
				RaiseInternalDependenciesInserted(dependencies);
			}
		}
		protected internal virtual void OnDependencyDeleted(object sender, PersistentObjectEventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterDeletedDependency((AppointmentDependency)e.Object);
			else {
				AppointmentDependencyBaseCollection dependencies = new AppointmentDependencyBaseCollection();
				dependencies.Add((AppointmentDependency)e.Object);
				UnsubscribeAutoReloadEvents();
				try {
					RaiseDependenciesDeleted(dependencies);
				} finally {
					SubscribeAutoReloadEvents();
				}
				RaiseInternalDependenciesDeleted(dependencies);
				e.Object.Dispose();
			}
		}
		protected internal virtual void OnDependencyChanged(object sender, PersistentObjectEventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterChangedDependency((AppointmentDependency)e.Object);
			else {
				AppointmentDependencyBaseCollection dependencies = new AppointmentDependencyBaseCollection();
				dependencies.Add((AppointmentDependency)e.Object);
				UnsubscribeAutoReloadEvents();
				try {
					RaiseAppointmentDependenciesChanged(dependencies);
				} finally {
					SubscribeAutoReloadEvents();
				}
				RaiseInternalDependenciesChanged(dependencies);
			}
		}
		protected internal virtual void OnDependencyCollectionCleared(object sender, EventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterClearDependencies();
			else {
				UnsubscribeAutoReloadEvents();
				try {
					RaiseAppointmentDependencyCollectionCleared();
				} finally {
					SubscribeAutoReloadEvents();
				}
				RaiseInternalDependencyCollectionCleared();
			}
		}
		protected internal virtual void OnDependencyCollectionLoaded(object sender, EventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterRaiseDependencyCollectionLoaded();
			else {
				UnsubscribeAutoReloadEvents();
				try {
					RaiseAppointmentDependencyCollectionLoaded();
				} finally {
					SubscribeAutoReloadEvents();
				}
				RaiseInternalDependencyCollectionLoaded();
			}
		}
		protected internal virtual void OnDependencyMappingsChanged(object sender, EventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterDependencyMappingsChanged();
			else
				RaiseInternalDependencyMappingsChanged();
		}
		protected internal virtual void OnDependencyUIObjectsChanged(object sender, EventArgs e) {
			if (IsUpdateLocked)
				deferredChanges.RegisterDependencyUIObjectsChanged();
			else
				RaiseInternalDependencyUIObjectsChanged();
		}
		protected internal virtual void LoadAppointments(bool keepNonPersistentInformation) {
			UnsubscribeAppointmentStorageEvents();
			try {
				LoadAppointmentsCore(keepNonPersistentInformation);
			} finally {
				SubscribeAppointmentStorageEvents();
			}
			if (IsUpdateLocked)
				deferredChanges.RegisterLoadAppointments(keepNonPersistentInformation);
			else {
				appointmentCache.OnAppointmentCollectionLoaded();
				appointmentTreeController.OnAppointmentCollectionLoaded(InnerAppointments.Items);
				remindersEngine.OnAppointmentCollectionLoaded(InnerAppointments.Items);
				RaiseAppointmentCollectionLoaded();
				RaiseInternalAppointmentCollectionLoaded();
			}
		}
		protected internal virtual void LoadResources(bool keepNonPersistentInformation) {
			UnsubscribeResourceStorageEvents();
			this.resourceStorageEventsSubscribtionLocker.Lock();
			try {
				LoadResourcesCore(keepNonPersistentInformation);
				if (IsUpdateLocked)
					deferredChanges.RegisterLoadResources(keepNonPersistentInformation);
				else {
					RaiseResourceCollectionLoaded();
					RaiseInternalResourceCollectionLoaded();
				}
			} finally {
				this.resourceStorageEventsSubscribtionLocker.Unlock();
				SubscribeResourceStorageEvents();
			}
		}
		protected internal virtual void LoadDependencies(bool keepNonPersistentInformation) {
			UnsubscribeDependencyStorageEvents();
			try {
				LoadDependenciesCore(keepNonPersistentInformation);
				if (IsUpdateLocked)
					deferredChanges.RegisterLoadDependencies(keepNonPersistentInformation);
				else {
					RaiseAppointmentDependencyCollectionLoaded();
					RaiseInternalDependencyCollectionLoaded();
				}
			} finally {
				SubscribeDependencyStorageEvents();
			}
		}
		protected internal virtual void LoadAppointmentsCore(bool keepNonPersistentInformation) {
			((IInternalAppointmentStorage)appointments).LoadObjects(keepNonPersistentInformation);
			this.AppointmentTreeController.LoadAppointments(InnerAppointments.Items);
		}
		protected internal virtual void LoadResourcesCore(bool keepNonPersistentInformation) {
			((IInternalResourceStorage)resources).LoadObjects(keepNonPersistentInformation);
		}
		protected internal virtual void LoadDependenciesCore(bool keepNonPersistentInformation) {
			lock (dependencies)
				((IInternalAppointmentDependencyStorage)dependencies).LoadObjects(keepNonPersistentInformation);
		}
		#region Direct Data Access Methods
		public object GetObjectRow(IPersistentObject obj) {
			Appointment apt = obj as Appointment;
			if (apt != null)
				return InnerAppointments.GetObjectRow(apt);
			Resource resource = obj as Resource;
			if (resource != null)
				return InnerResources.GetObjectRow(resource);
			else
				return InnerAppointmentDependencies.GetObjectRow((AppointmentDependency)obj);
		}
		public object GetObjectValue(IPersistentObject obj, string columnName) {
			Appointment apt = obj as Appointment;
			if (apt != null)
				return InnerAppointments.GetObjectValue(apt, columnName);
			Resource resource = obj as Resource;
			if (resource != null)
				return InnerResources.GetObjectValue(resource, columnName);
			else
				return InnerAppointmentDependencies.GetObjectValue((AppointmentDependency)obj, columnName);
		}
		public void SetObjectValue(IPersistentObject obj, string columnName, object val) {
			Appointment apt = obj as Appointment;
			if (apt != null) {
				InnerAppointments.SetObjectValue(apt, columnName, val);
				return;
			}
			Resource resource = obj as Resource;
			if (resource != null)
				InnerResources.SetObjectValue(resource, columnName, val);
			else
				InnerAppointmentDependencies.SetObjectValue((AppointmentDependency)obj, columnName, val);
		}
		#endregion
		public void SetAppointmentFactory(IAppointmentFactory appointmentFactory) {
			InnerAppointments.SetAppointmentFactory(appointmentFactory);
		}
		public void SetResourceFactory(IResourceFactory resourceFactory) {
			InnerResources.SetResourceFactory(resourceFactory);
		}
		public void SetAppointmentDependencyFactory(IAppointmentDependencyFactory dependencyFactory) {
			InnerAppointmentDependencies.SetAppointmentDependencyFactory(dependencyFactory);
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
		public Resource CreateResource(object resourceId) {
			return InnerResources.CreateResource(resourceId);
		}
		public Resource CreateResource(object resourceId, string resourceCaption) {
			return InnerResources.CreateResource(resourceId, resourceCaption);
		}
		public AppointmentDependency CreateAppointmentDependency(object parentId, object dependentId) {
			return InnerAppointmentDependencies.CreateAppointmentDependency(parentId, dependentId);
		}
		public AppointmentDependency CreateAppointmentDependency(object parentId, object dependentId, AppointmentDependencyType type) {
			return InnerAppointmentDependencies.CreateAppointmentDependency(parentId, dependentId, type);
		}
		public Reminder CreateReminder(Appointment appointment) {
			return new Reminder(appointment);
		}
		#region GetAppointments overloads
		public virtual AppointmentBaseCollection GetAppointments(TimeInterval interval) {
			if (interval == null)
				Exceptions.ThrowArgumentException("interval", interval);
			return GetAppointmentsCore(interval, this);
		}
		public AppointmentBaseCollection GetAppointments(TimeIntervalCollection intervals) {
			if (intervals == null)
				Exceptions.ThrowArgumentException("intervals", intervals);
			return GetAppointmentsCore(intervals.Interval, this);
		}
		public AppointmentBaseCollection GetAppointments(DateTime start, DateTime end) {
			return GetAppointmentsCore(new TimeInterval(start, end), this);
		}
		public AppointmentBaseCollection GetAppointments(DateTime start, DateTime end, bool useCache) {
			return GetAppointmentsCore(new TimeInterval(start, end), this, useCache);
		}
		protected internal virtual AppointmentBaseCollection GetNonRecurringAppointments() {
			AppointmentBaseCollection nonFilteredResult = RemoveRecurrence(InnerAppointments.Items);
			return FilterAppointments(nonFilteredResult);
		}
		protected internal virtual AppointmentBaseCollection RemoveRecurrence(AppointmentCollection items) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			int itemCount = items.Count;
			for (int i = 0; i < itemCount; i++) {
				Appointment currentItem = items[i];
				if (currentItem.Type == AppointmentType.Normal)
					result.Add(currentItem);
			}
			return result;
		}
		protected internal virtual AppointmentBaseCollection GetAppointmentsCore(TimeInterval interval, object callerObject) {
			return GetAppointmentsCore(interval, callerObject, true);
		}
		protected internal virtual AppointmentBaseCollection GetAppointmentsCore(TimeInterval interval, object callerObject, bool useCache) {
			AppointmentBaseCollection nonFilteredResult = GetNonFilteredAppointments(interval, callerObject);
			return FilterAppointments(nonFilteredResult);
		}
		protected internal virtual AppointmentDependencyBaseCollection GetFilteredDependencies(TimeInterval interval, object callerObject) {
			AppointmentDependencyBaseCollection nonFilteredResult = GetNonFilteredDependencies(interval, callerObject);
			return FilterDependencies(nonFilteredResult);
		}
		protected internal virtual AppointmentDependencyBaseCollection GetNonFilteredDependencies(TimeInterval interval, object callerObject) {
			AppointmentDependencyBaseCollection result = new AppointmentDependencyCollection();
			int count = InnerAppointmentDependencies.Items.Count;
			for (int i = 0; i < count; i++) {
				AppointmentDependency dependency = InnerAppointmentDependencies.Items[i];
				if (IsDependencyIntersectsInterval(dependency, interval) && IsDependencyValid(dependency))
					result.Add(dependency);
			}
			return result;
		}
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool IsDependencyValid(AppointmentDependency dependency) {
			return (dependency.ParentId != null) && (dependency.DependentId != null);
		}
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool IsDependencyIntersectsInterval(AppointmentDependency dependency, TimeInterval interval) {
			Appointment parent = InnerAppointments.Items.GetAppointmentById(dependency.ParentId);
			Appointment dependent = InnerAppointments.Items.GetAppointmentById(dependency.DependentId);
			if (!CheckDependencyAppointmentsVisibility(parent, dependent))
				return false;
			TimeInterval dependencyInterval = GetDependencyTimeInterval(parent, dependent, dependency.Type);
			return dependencyInterval.IntersectsWithExcludingBounds(interval);
		}
		protected internal virtual TimeInterval GetDependencyTimeInterval(Appointment parent, Appointment dependent, AppointmentDependencyType type) {
			DateTime start = Algorithms.Min<DateTime>(parent.Start, dependent.Start);
			DateTime end = Algorithms.Max<DateTime>(parent.End, dependent.End);
			return new TimeInterval(start, end);
		}
		protected internal bool CheckDependencyAppointmentsVisibility(Appointment parent, Appointment dependent) {
			if (parent == null || dependent == null)
				return false;
			AppointmentBaseCollection collection = new AppointmentBaseCollection();
			collection.Add(parent);
			collection.Add(dependent);
			collection = FilterAppointments(collection);
			return collection.Count == 2;
		}
		protected internal virtual Dictionary<TimeInterval, AppointmentBaseCollection> GetFilteredAppointmentByIntervals(TimeIntervalCollection searchIntervals, AppointmentResourcesMatchFilter filter, object callerObject) {
			InternalRaiseFetchAppointmentsCore(searchIntervals.Interval, callerObject);
			return AppointmentTreeController.FindAppointmentsByIntervals(searchIntervals, filter);
		}
		protected internal virtual IList<DateTime> GetAppointmentDates(TimeInterval interval, object callerObject, AppointmentResourcesMatchFilter filter) {
			XtraSchedulerDebug.Assert(callerObject is IDateNavigatorControllerOwner);
			InternalRaiseFetchAppointmentsCore(interval, callerObject);
			DateCollection result = AppointmentTreeController.FindAppointmentDates(interval, filter);
			DateCollection recurringDates = AppointmentTreeController.FindRecurringAppointmentDates(interval, filter);
			result.AddRange(recurringDates);
			return result;
		}
		public virtual AppointmentBaseCollection GetNonFilteredAppointments(TimeInterval interval, object callerObject) {
			return GetNonFilteredAppointmentsCore(interval, callerObject, true);
		}
		protected AppointmentBaseCollection GetNonFilteredAppointmentsCore(TimeInterval interval, object callerObject, bool useCache) {
			IInnerSchedulerCommandTarget schedulerCommandTarget = callerObject as IInnerSchedulerCommandTarget;
			if (schedulerCommandTarget == null || !schedulerCommandTarget.InnerSchedulerControl.NestedQuery)
				InternalRaiseFetchAppointmentsCore(interval, callerObject);
			return InternalGetNonFilteredAppointments(interval, callerObject, AppointmentResourcesMatchFilter.Empty, useCache);
		}
		protected AppointmentBaseCollection InternalGetNonFilteredAppointments(TimeInterval interval, object callerObject, AppointmentResourcesMatchFilter filter) {
			return InternalGetNonFilteredAppointments(interval, callerObject, filter, true);
		}
		protected AppointmentBaseCollection InternalGetNonFilteredAppointments(TimeInterval interval, object callerObject, AppointmentResourcesMatchFilter filter, bool useCache) {
			AppointmentBaseCollection result = new AppointmentBaseCollection(DXCollectionUniquenessProviderType.None);
			if (useCache && AppointmentCacheEnabled && appointmentCache.CacheHit(callerObject, interval))
				result = appointmentCache.GetAppointments(callerObject, interval);
			else {
				if (!useCache || ShouldGetAppointmentsFromStorage(interval)) {
					result = ((IInternalAppointmentStorage)InnerAppointments).GetAppointmentsExpandingPatterns(interval);
				} else {
					AppointmentBaseCollection recurringAppointments = FindRecurringAppointments(result, interval);
					result.AddRange(recurringAppointments);
					AppointmentBaseCollection treeAppointments = AppointmentTreeController.FindAppointments(interval);
					result.AddRange(treeAppointments);
				}
				if (useCache && AppointmentCacheEnabled)
					appointmentCache.SetContent(callerObject, result, interval);
			}
			AppointmentBaseComparer comparer = AppointmentComparerProvider.CreateDefaultAppointmentComparer(); 
			result.Sort(comparer);
			return result;
		}
		protected bool ShouldGetAppointmentsFromStorage(TimeInterval interval) {
			if (InnerAppointments.Count == 0)
				return true;
			return interval.Duration.Days > AppointmentTreeController.MaxSearchDayCount;
		}
		protected void InternalRaiseFetchAppointmentsCore(TimeInterval interval, object callerObject) {
			if (onFetchAppointments == null || this.fetchAppointmentLocker.IsLocked)
				return;
			if (!CheckNewInterval(interval, callerObject))
				return;
			if (FetchAppointmentsCore(interval) && !IsUpdateLocked) {
				System.Diagnostics.Debug.Assert(!activeCallers.Contains(callerObject));
				activeCallers.Add(callerObject);
				try {
					this.fetchAppointmentLocker.Lock();
					RaiseInternalAppointmentCollectionLoaded();
				} finally {
					this.fetchAppointmentLocker.Unlock();
					activeCallers.Remove(callerObject);
				}
			}
		}
		protected virtual bool CheckNewInterval(TimeInterval interval, object callerObject) {
			IInnerSchedulerCommandTarget commandTarget = callerObject as IInnerSchedulerCommandTarget;
			if (commandTarget == null || !commandTarget.InnerSchedulerControl.OptionsBehavior.EnableSmartFetch)
				return true;
			if (lastFetchInterval == null) {
				lastFetchInterval = interval.Clone();
				return true;
			}
			if (!lastFetchInterval.IntersectsWith(interval) || !lastFetchInterval.Contains(interval)) {
				lastFetchInterval = interval.Clone();
				return true;
			}
			return false;
		}
		protected virtual AppointmentBaseCollection FindRecurringAppointments(AppointmentBaseCollection result, TimeInterval interval) {
			if (!SupportsRecurrence)
				return new AppointmentBaseCollection();
			return AppointmentTreeController.FindAppointmentsExpandingPatterns(interval, TimeZoneEngine);
		}
		protected internal virtual AppointmentBaseCollection FilterAppointments(AppointmentBaseCollection appointments) {
			if (CanUseAppointmentExternalFilter())
				return FilterAppointmentsCore(appointments);
			return appointments;
		}
		protected internal virtual bool CanUseAppointmentExternalFilter() {
			if (String.IsNullOrEmpty(InnerAppointments.Filter) && onFilterAppointment == null)
				return false;
			return true;
		}
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual AppointmentDependencyBaseCollection FilterDependencies(AppointmentDependencyBaseCollection dependencies) {
			if (CanUseDependencyExternalFilter())
				return FilterDependenciesCore(dependencies);
			return dependencies;
		}
		protected internal virtual bool CanUseDependencyExternalFilter() {
			if (String.IsNullOrEmpty(InnerAppointmentDependencies.Filter) && onFilterDependency == null)
				return false;
			return true;
		}
		protected internal virtual AppointmentDependencyBaseCollection FilterDependenciesCore(AppointmentDependencyBaseCollection dependencies) {
			if (dependencies.Count == 0)
				return dependencies;
			AppointmentDependencyFilter filter = new AppointmentDependencyFilter(CreateAppointmentDependencyExternalFilter());
			filter.Process(dependencies);
			return (AppointmentDependencyCollection)filter.DestinationCollection;
		}
		protected internal virtual IPredicate<AppointmentDependency> CreateAppointmentDependencyExternalFilter() {
			CompositeAppointmentDependencyPredicate filter = new CompositeAppointmentDependencyPredicate();
			if (!String.IsNullOrEmpty(InnerAppointmentDependencies.Filter))
				filter.Items.Add(new FilterAppointmentDependencyPredicate(InnerAppointmentDependencies));
			if (onFilterDependency != null)
				filter.Items.Add(new FilterAppointmentDependencyViaStorageEventPredicate(this));
			if (filter.Items.Count == 0)
				return new EmptyAppointmentDependencyFilterPredicate();
			if (filter.Items.Count == 1)
				return filter.Items[0];
			return filter;
		}
		protected internal virtual bool FetchAppointmentsCore(TimeInterval interval) {
			System.Diagnostics.Debug.WriteLine(String.Format("->FetchAppointmentsCore: {0}", interval.ToString()));
			BeginUpdate();
			deferredChanges.SuppressDeferredNotifications = true;
			try {
				if (!batchUpdateHelper.OverlappedTransaction)
					UnsubscribeAppointmentStorageEvents();
				try {
					SchedulerStorageReloadListener listener = new SchedulerStorageReloadListener();
					((IInternalAppointmentStorage)this.InnerAppointments).Reload += new PersistentObjectStorageReloadEventHandler(listener.OnReload);
					try {
						if (RaiseFetchAppointments(interval)) {
							RaiseAfterFetchAppointments();
							listener.ShouldReload = true;
						}
					} finally {
						((IInternalAppointmentStorage)this.InnerAppointments).Reload -= new PersistentObjectStorageReloadEventHandler(listener.OnReload);
					}
					if (listener.ShouldReload) {
						LoadAppointmentsCore(true);
						deferredChanges.RegisterLoadAppointments(true);
						return true;
					} else
						return false;
				} finally {
					if (!batchUpdateHelper.OverlappedTransaction)
						SubscribeAppointmentStorageEvents();
				}
			} finally {
				CancelUpdate();
				if (IsUpdateLocked)
					deferredChanges.SuppressDeferredNotifications = false;
			}
		}
		protected internal virtual AppointmentBaseCollection FilterAppointmentsCore(AppointmentBaseCollection appointments) {
			AppointmentsFilter filter = new AppointmentsFilter(CreateAppointmentExternalFilter());
			filter.Process(appointments);
			return (AppointmentBaseCollection)filter.DestinationCollection;
		}
		protected internal virtual IPredicate<Appointment> CreateAppointmentExternalFilterPredicate() {
			return CanUseAppointmentExternalFilter() ? CreateAppointmentExternalFilter() : null;
		}
		protected internal virtual IPredicate<Appointment> CreateAppointmentExternalFilter() {
			CompositeAppointmentPredicate filter = new CompositeAppointmentPredicate();
			if (!String.IsNullOrEmpty(InnerAppointments.Filter))
				filter.Items.Add(new FilterAppointmentPredicate(InnerAppointments));
			if (onFilterAppointment != null)
				filter.Items.Add(new FilterAppointmentViaStorageEventPredicate(this));
			if (filter.Items.Count == 0)
				return new EmptyAppointmentPredicate();
			if (filter.Items.Count == 1)
				return filter.Items[0];
			return filter;
		}
		protected internal virtual IPredicate<OccurrenceInfo> CreateOccurrenceInfoExternalFilter() {
			IPredicate<Appointment> filter = CreateAppointmentExternalFilter();
			if (filter is SchedulerEmptyPredicate<Appointment>)
				return new EmptyOccurrenceInfoPredicate();
			return new FilterOccurrenceInfoPredicate(filter);
		}
		protected internal virtual IPredicate<Resource> CreateResourceExternalFilter(bool useResourcesTreeFilter) {
			CompositeResourcePredicate filter = new CompositeResourcePredicate();
			if (!String.IsNullOrEmpty(InnerResources.Filter))
				filter.Items.Add(new FilterResourcePredicate(InnerResources, InnerResources.Filter));
			if (onFilterResource != null)
				filter.Items.Add(new FilterResourceViaStorageEventPredicate(this));
			if (useResourcesTreeFilter && !String.IsNullOrEmpty(((IInternalResourceStorage)InnerResources).ResourcesTreeFilter))
				filter.Items.Add(new FilterResourcePredicate(InnerResources, ((IInternalResourceStorage)InnerResources).ResourcesTreeFilter, false));
			if (filter.Items.Count == 0)
				return new EmptyResourceFilterPredicate();
			if (filter.Items.Count == 1)
				return filter.Items[0];
			return filter;
		}
		#endregion
		public virtual void RefreshData() {
			BeginUpdate();
			try {
				LoadResources(true);
				LoadAppointments(true);
				LoadDependencies(true);
			} finally {
				EndUpdate();
			}
		}
		protected internal virtual IAppointmentStatus GetInnerStatus(object statusId) {
			return InnerAppointments.Statuses.GetById(statusId);
		}
		protected internal virtual ResourceBaseCollection GetFilteredResources(bool useResourcesTreeFilter) {
			ResourceBaseCollection nonFilteredResult = GetNonFilteredResourcesCore();
			return FilterResources(nonFilteredResult, useResourcesTreeFilter);
		}
		protected internal virtual ResourceBaseCollection GetResourcesTree(bool useResourcesTreeFilter) {
			ResourceBaseCollection filteredResources = GetVisibleResources(useResourcesTreeFilter);
			ResourceHierarchyHelper helper = new ResourceHierarchyHelper();
			return helper.GetSortedResourcesTree(filteredResources, this);
		}
		protected internal virtual ResourceBaseCollection FilterResources(ResourceBaseCollection resources, bool useResourcesTreeFilter) {
			if (CanUseResourcesExternalFilter(useResourcesTreeFilter))
				return FilterResourcesCore(resources, useResourcesTreeFilter);
			else
				return resources;
		}
		protected internal virtual bool CanUseResourcesExternalFilter(bool useResourcesTreeFilter) {
			if (String.IsNullOrEmpty(InnerResources.Filter) && onFilterResource == null)
				return useResourcesTreeFilter && !String.IsNullOrEmpty(((IInternalResourceStorage)InnerResources).ResourcesTreeFilter);
			return true;
		}
		protected internal virtual ResourceBaseCollection FilterResourcesCore(ResourceBaseCollection resources, bool useResourcesTreeFilter) {
			ResourceFilter filter = new ResourceFilter(CreateResourceExternalFilter(useResourcesTreeFilter));
			filter.Process(resources);
			return (ResourceBaseCollection)filter.DestinationCollection;
		}
		ResourceBaseCollection IInternalSchedulerStorageBase.GetNonFilteredResourcesCore() {
			return GetNonFilteredResourcesCore();
		}
		protected virtual ResourceBaseCollection GetNonFilteredResourcesCore() {
			ResourceBaseCollection result = new ResourceBaseCollection(DXCollectionUniquenessProviderType.MaximizePerformance);
			result.AddRange(InnerResources.Items);
			return result;
		}
		protected internal virtual ResourceBaseCollection GetVisibleResources(bool useResourcesTreeFilter) {
			ResourceBaseCollection result = GetFilteredResources(useResourcesTreeFilter);
			int count = result.Count;
			for (int i = count - 1; i >= 0; i--)
				if (!result[i].Visible)
					result.RemoveAt(i);
			return result;
		}
		public virtual int CalcTotalAppointmentCountForExchange() {
			AppointmentBaseCollection appointments = InnerAppointments.Items;
			int count = appointments.Count;
			int result = count;
			for (int i = 0; i < count; i++) {
				Appointment apt = appointments[i];
				if (apt.HasExceptions)
					result += ((IInternalAppointment)apt).PatternExceptions.Count;
			}
			return result;
		}
		protected internal virtual void SetInnerAppointmentsCore(IAppointmentStorageBase value) {
			UnsubscribeAppointmentStorageEvents();
			this.appointmentCache.DeleteCache(this);
			if (appointments != null) {
				AssignInnerAppointmentsProperties(value);
				IDisposable disposableAppointments = appointments as IDisposable;
				if (disposableAppointments != null)
					disposableAppointments.Dispose();
			}
			appointments = value;
			this.appointmentCache.CreateCache(this);
			SubscribeAppointmentStorageEvents();
		}
		protected internal virtual void SetInnerResourcesCore(IResourceStorageBase value) {
			UnsubscribeResourceStorageEvents();
			if (resources != null) {
				AssignInnerResourcesProperties(value);
				IDisposable disposableResources = resources as IDisposable;
				if (disposableResources != null)
					disposableResources.Dispose();
			}
			this.resources = value;
			SubscribeResourceStorageEvents();
		}
		protected internal virtual void SetInnerDependenciesCore(IAppointmentDependencyStorage value) {
			UnsubscribeDependencyStorageEvents();
			if (dependencies != null) {
				AssignInnerDependenciesProperties(value);
				dependencies.Dispose();
			}
			this.dependencies = value;
			SubscribeDependencyStorageEvents();
		}
		protected internal virtual void AssignInnerAppointmentsProperties(IAppointmentStorageBase newAppointments) {
			newAppointments.SetAppointmentFactory(((IInternalAppointmentStorage)appointments).AppointmentFactory);
		}
		protected internal virtual void AssignInnerResourcesProperties(IResourceStorageBase newResources) {
			newResources.SetResourceFactory(((IInternalResourceStorage)resources).ResourceFactory);
		}
		protected internal virtual void AssignInnerDependenciesProperties(IAppointmentDependencyStorage newDependencies) {
			newDependencies.SetAppointmentDependencyFactory(((IInternalAppointmentDependencyStorage)dependencies).DependencyFactory);
		}
		protected internal virtual void Assign(ISchedulerStorageBase source) {
			if (source == null)
				return;
			BeginUpdate();
			try {
#if !SL
				Component srcComponent = source as Component;
				if (srcComponent != null)
					Site = srcComponent.Site;
#endif
				EnableTimeZones = source.EnableTimeZones;
				TimeZoneId = source.TimeZoneId;
				EnableReminders = source.EnableReminders;
				RemindersCheckInterval = source.RemindersCheckInterval;
			} finally {
				EndUpdate();
			}
		}
		Dictionary<object, Appointment> IInternalSchedulerStorageBase.FindNearestAppointmentInterval(TimeInterval searchInterval, AppointmentResourcesMatchFilter filter, bool findPrevApt) {
			ResourceBaseCollection resources = filter.Resources;
			if (AppointmentTreeController == null || resources == null || resources.Count == 0)
				return null;
			Dictionary<object, Appointment> apts = new Dictionary<object, Appointment>();
			Dictionary<object, Appointment> occurrenceApts = new Dictionary<object, Appointment>();
			if (findPrevApt) {
				apts = AppointmentTreeController.FindPrevAppointment(searchInterval, filter);
				occurrenceApts = AppointmentTreeController.FindPrevOccurrenceAppointment(searchInterval, filter);
			} else {
				apts = AppointmentTreeController.FindNextAppointment(searchInterval, filter);
				occurrenceApts = AppointmentTreeController.FindNextOccurrenceAppointment(searchInterval, filter);
			}
			return FindNearestAppointmentInDictionaries(findPrevApt, resources, apts, occurrenceApts);
		}
		Dictionary<object, Appointment> FindNearestAppointmentInDictionaries(bool findPrevApt, ResourceBaseCollection resources, Dictionary<object, Appointment> apts, Dictionary<object, Appointment> occurrenceApts) {
			for (int i = 0; i < resources.Count; i++) {
				object resourceId = resources[i].Id;
				if (resourceId != null) {
					if (occurrenceApts[resourceId] == null)
						continue;
					if (apts[resourceId] == null) {
						apts[resourceId] = occurrenceApts[resourceId];
						continue;
					}
					Appointment apt = apts[resourceId];
					Appointment occurrenceApt = occurrenceApts[resourceId];
					TimeInterval aptInterval = apt != null ? ((IInternalAppointment)apt).GetInterval() : TimeInterval.Empty;
					TimeInterval occurrenceAptInterval = occurrenceApt != null ? ((IInternalAppointment)occurrenceApt).GetInterval() : TimeInterval.Empty;
					bool isOccurrenceAptsNearest = findPrevApt ? aptInterval.End < occurrenceAptInterval.End : aptInterval.Start > occurrenceAptInterval.Start;
					if (occurrenceApts[resourceId] != null && isOccurrenceAptsNearest)
						apts[resourceId] = occurrenceApts[resourceId];
				}
			}
			return apts;
		}
		#region ISupportReminders implementation
		void ISupportReminders.TriggerAlerts(DateTime currentTime) {
			if (this.remindersEngine == null)
				return;
			this.remindersEngine.TriggerAlerts(currentTime);
		}
		#endregion
		#region ISchedulerStorage
		IAppointmentStorageBase ISchedulerStorageBase.Appointments { get { return InnerAppointments; } }
		IAppointmentDependencyStorage ISchedulerStorageBase.AppointmentDependencies { get { return InnerAppointmentDependencies; } }
		IResourceStorageBase ISchedulerStorageBase.Resources { get { return InnerResources; } }
		AppointmentBaseCollection IInternalSchedulerStorageBase.GetNonRecurringAppointments() {
			return GetNonRecurringAppointments();
		}
		IPredicate<Appointment> IInternalSchedulerStorageBase.CreateAppointmentExternalFilterPredicate() {
			return CreateAppointmentExternalFilterPredicate();
		}
		Dictionary<TimeInterval, AppointmentBaseCollection> IInternalSchedulerStorageBase.GetFilteredAppointmentByIntervals(TimeIntervalCollection searchIntervals, AppointmentResourcesMatchFilter filter, object callerObject) {
			return GetFilteredAppointmentByIntervals(searchIntervals, filter, callerObject);
		}
		IList<DateTime> IInternalSchedulerStorageBase.GetAppointmentDates(TimeInterval interval, object callerObject, AppointmentResourcesMatchFilter filter) {
			return GetAppointmentDates(interval, callerObject, filter);
		}
		AppointmentBaseCollection ISchedulerStorageBase.GetAppointmentsCore(TimeInterval interval, object callerObject) {
			return GetAppointmentsCore(interval, callerObject);
		}
		AppointmentDependencyBaseCollection IInternalSchedulerStorageBase.GetFilteredDependencies(TimeInterval interval, object callerObject) {
			return GetFilteredDependencies(interval, callerObject);
		}
		Resource ISchedulerStorageBase.GetResourceById(object resourceId) {
			return InnerResources.GetResourceById(resourceId);
		}
		ResourceBaseCollection IInternalSchedulerStorageBase.GetFilteredResources(bool useResourcesTreeFilter) {
			return GetFilteredResources(useResourcesTreeFilter);
		}
		ResourceBaseCollection IInternalSchedulerStorageBase.GetVisibleResources(bool useResourcesTreeFilter) {
			return GetVisibleResources(useResourcesTreeFilter);
		}
		ResourceBaseCollection IInternalSchedulerStorageBase.GetResourcesTree(bool useResourcesTreeFilter) {
			return GetResourcesTree(useResourcesTreeFilter);
		}
		IPredicate<Appointment> IInternalSchedulerStorageBase.CreateAppointmentExternalFilter() {
			return CreateAppointmentExternalFilter();
		}
		IPredicate<OccurrenceInfo> IInternalSchedulerStorageBase.CreateOccurrenceInfoExternalFilter() {
			return CreateOccurrenceInfoExternalFilter();
		}
		void ISchedulerStorageBase.RegisterClient(object caller) {
			AppointmentMultiClientCache cache = AppointmentCache;
			if (cache != null)
				cache.CreateCache(caller);
		}
		void ISchedulerStorageBase.UnregisterClient(object caller) {
			AppointmentMultiClientCache cache = AppointmentCache;
			if (cache != null)
				cache.DeleteCache(caller);
		}
		#endregion
		#region IInternalSchedulerStorage
		SchedulerStorageDeferredChanges IInternalSchedulerStorageBase.DeferredChanges { get { return DeferredChanges; } }
		event EventHandler IInternalSchedulerStorageBase.InternalAppointmentCollectionCleared { add { InternalAppointmentCollectionCleared += value; } remove { InternalAppointmentCollectionCleared -= value; } }
		event EventHandler IInternalSchedulerStorageBase.InternalAppointmentCollectionLoaded { add { InternalAppointmentCollectionLoaded += value; } remove { InternalAppointmentCollectionLoaded -= value; } }
		event PersistentObjectsEventHandler IInternalSchedulerStorageBase.InternalAppointmentsInserted { add { InternalAppointmentsInserted += value; } remove { InternalAppointmentsInserted -= value; } }
		event PersistentObjectsEventHandler IInternalSchedulerStorageBase.InternalAppointmentsChanged { add { InternalAppointmentsChanged += value; } remove { InternalAppointmentsChanged -= value; } }
		event PersistentObjectsEventHandler IInternalSchedulerStorageBase.InternalAppointmentsDeleted { add { InternalAppointmentsDeleted += value; } remove { InternalAppointmentsDeleted -= value; } }
		event EventHandler IInternalSchedulerStorageBase.InternalAppointmentMappingsChanged { add { InternalAppointmentMappingsChanged += value; } remove { InternalAppointmentMappingsChanged -= value; } }
		event EventHandler IInternalSchedulerStorageBase.InternalAppointmentUIObjectsChanged { add { InternalAppointmentUIObjectsChanged += value; } remove { InternalAppointmentUIObjectsChanged -= value; } }
		event EventHandler IInternalSchedulerStorageBase.InternalResourceCollectionLoaded { add { InternalResourceCollectionLoaded += value; } remove { InternalResourceCollectionLoaded -= value; } }
		event EventHandler IInternalSchedulerStorageBase.InternalResourceCollectionCleared { add { InternalResourceCollectionCleared += value; } remove { InternalResourceCollectionCleared -= value; } }
		event PersistentObjectsEventHandler IInternalSchedulerStorageBase.InternalResourcesInserted { add { InternalResourcesInserted += value; } remove { InternalResourcesInserted -= value; } }
		event PersistentObjectsEventHandler IInternalSchedulerStorageBase.InternalResourcesChanged { add { InternalResourcesChanged += value; } remove { InternalResourcesChanged -= value; } }
		event PersistentObjectsEventHandler IInternalSchedulerStorageBase.InternalResourcesDeleted { add { InternalResourcesDeleted += value; } remove { InternalResourcesDeleted -= value; } }
		event EventHandler IInternalSchedulerStorageBase.InternalResourceMappingsChanged { add { InternalResourceMappingsChanged += value; } remove { InternalResourceMappingsChanged -= value; } }
		event EventHandler IInternalSchedulerStorageBase.InternalResourceVisibilityChanged { add { InternalResourceVisibilityChanged += value; } remove { InternalResourceVisibilityChanged -= value; } }
		event EventHandler IInternalSchedulerStorageBase.InternalResourceSortedColumnsChange { add { InternalResourceSortedColumnsChange += value; } remove { InternalResourceSortedColumnsChange -= value; } }
		event EventHandler IInternalSchedulerStorageBase.InternalDependencyCollectionLoaded { add { InternalDependencyCollectionLoaded += value; } remove { InternalDependencyCollectionLoaded -= value; } }
		event EventHandler IInternalSchedulerStorageBase.InternalDependencyCollectionCleared { add { InternalDependencyCollectionCleared += value; } remove { InternalDependencyCollectionCleared -= value; } }
		event PersistentObjectsEventHandler IInternalSchedulerStorageBase.InternalDependenciesInserted { add { InternalDependenciesInserted += value; } remove { InternalDependenciesInserted -= value; } }
		event PersistentObjectsEventHandler IInternalSchedulerStorageBase.InternalDependenciesChanged { add { InternalDependenciesChanged += value; } remove { InternalDependenciesChanged -= value; } }
		event PersistentObjectsEventHandler IInternalSchedulerStorageBase.InternalDependenciesDeleted { add { InternalDependenciesDeleted += value; } remove { InternalDependenciesDeleted -= value; } }
		event EventHandler IInternalSchedulerStorageBase.InternalDependencyMappingsChanged { add { InternalDependencyMappingsChanged += value; } remove { InternalDependencyMappingsChanged -= value; } }
		event EventHandler IInternalSchedulerStorageBase.InternalDependencyUIObjectsChanged { add { InternalDependencyUIObjectsChanged += value; } remove { InternalDependencyUIObjectsChanged -= value; } }
		event EventHandler IInternalSchedulerStorageBase.InternalDeferredNotifications { add { InternalDeferredNotifications += value; } remove { InternalDeferredNotifications -= value; } }
		event EventHandler IInternalSchedulerStorageBase.BeforeDispose { add { BeforeDispose += value; } remove { BeforeDispose -= value; } }
		event EventHandler IInternalSchedulerStorageBase.UpdateUI { add { UpdateUI += value; } remove { UpdateUI -= value; } }
		event ReminderEventHandler IInternalSchedulerStorageBase.InternalReminderAlert { add { InternalReminderAlert += value; } remove { InternalReminderAlert -= value; } }
		void IInternalSchedulerStorageBase.RaiseInternalResourceVisibilityChanged() {
			RaiseInternalResourceVisibilityChanged();
		}
		#endregion
	}
	#endregion
	public interface IPersistentObjectStorage<T> : IBatchUpdateable, IDisposable where T : IPersistentObject {
		MappingInfoBase<T> Mappings { get; set; }
		CustomFieldMappingCollectionBase<T> CustomFieldMappings { get; set; }
		NotificationCollection<T> Items { get; }
		object DataSource { get; set; }
		string DataMember { get; set; }
		[Browsable(false), DXBrowsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		bool UnboundMode { get; }
		bool AutoReload { get; set; }
		[Browsable(false), DXBrowsable(false)]
		int Count { get; }
		[Browsable(false), DXBrowsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		T this[int index] { get; }
		string Filter { get; set; }
		ISchedulerStorageBase Storage { get; }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new bool IsUpdateLocked { get; }
		object GetObjectRow(T obj);
		object GetObjectValue(T obj, string columnName);
		void SetObjectValue(T obj, string columnName, object val);
		void AppendBaseMappings(MappingCollection target);
		void Clear();
		StringCollection GetColumnNames();
		void ValidateDataSource();
		void CreateCustomFields(T obj);
	}
	#region PersistentObjectStorage<T>
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	public abstract class PersistentObjectStorage<T> : IPersistentObjectStorage<T>, IInternalPersistentObjectStorage<T>, IBatchUpdateable, IBatchUpdateHandler, IDataControllerData2 where T : IPersistentObject {
		#region Fields
		ISchedulerStorageBase storage;
		NotificationCollection<T> items;
		ISchedulerUnboundDataKeeper unboundDataKeeper;
		DataManager<T> dataManager;
		MappingInfoBase<T> mappings;
		MappingCollection actualMappings;
		CustomFieldMappingCollectionBase<T> customFieldMappings;
		bool isDisposed;
		BatchUpdateHelper batchUpdateHelper;
		bool autoReload = true;
		object dataSource;
		string dataMember = String.Empty;
		string filter;
		bool isObjectCollectionLoading;
		int initializing;
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected PersistentObjectStorage() {
			Initialize();
		}
		protected PersistentObjectStorage(ISchedulerStorageBase storage) {
			CheckForExistenceStorage(storage);
			this.storage = storage;
			Initialize();
		}
		protected virtual void CheckForExistenceStorage(ISchedulerStorageBase storage) {
			if (storage == null)
				Exceptions.ThrowArgumentException("storage", storage);
		}
		void Initialize() {
			ShouldUpdateAfterInsert = false;
			this.initializing = 0;
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.items = CreateCollection();
			this.mappings = CreateMappingInfo();
			PrepareMappings();
			this.actualMappings = new MappingCollection();
			this.customFieldMappings = CreateCustomMappingsCollection();
			this.dataManager = CreateDataManager();
			InitializeDataManager();
			RecreateMappings();
			this.filter = String.Empty;
			this.unboundDataKeeper = CreateUnboundDataKeeper();
			RecreateUnboundDataKeeperList();
			UpdateDataManagerListSource(UnboundDataKeeper.List);
			SubscribeCollectionEvents();
			SubscribeMappingsEvents();
			SubscribeAutoReloadEvent();
		}
		protected virtual void PrepareMappings() {
		}
		protected virtual void InitializeDataManager() {
			this.dataManager.DataController.DataClient = this;
		}
		#region Properties
		[Browsable(false)]
		public virtual bool IsLoading { get { return this.initializing != 0; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ISchedulerUnboundDataKeeper UnboundDataKeeper { get { return unboundDataKeeper; } }
		public ISchedulerStorageBase Storage { get { return storage; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsDisposed { get { return isDisposed; } }
		protected internal bool IsObjectCollectionLoading { get { return isObjectCollectionLoading; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataManager<T> DataManager { get { return dataManager; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MappingInfoBase<T> Mappings {
			get { return mappings; }
			set {
				if (mappings == value)
					return;
				SetInnerMappingsCore(value);
			}
		}
		[NotifyParentProperty(true)]
		protected internal MappingCollection ActualMappings { get { return actualMappings; } }
		MappingCollection IInternalPersistentObjectStorage<T>.ActualMappings { get { return ActualMappings; } }
		[Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CustomFieldMappingCollectionBase<T> CustomFieldMappings {
			get { return customFieldMappings; }
			set {
				if (customFieldMappings == value)
					return;
				SetInnerCustomFieldMappingsCore(value);
			}
		}
		[Browsable(false), Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public NotificationCollection<T> Items { get { return items; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public T this[int index] { get { return Items[index]; } }
		[Browsable(false)]
		public int Count { get { return Items.Count; } }
		[Browsable(false)]
		protected virtual bool IsFastPropertiesSupport { get { return true; } }
		#region AutoReload
		[DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable()]
		public bool AutoReload {
			get { return autoReload; }
			set {
				if (AutoReload == value)
					return;
				UnsubscribeAutoReloadEvent();
				autoReload = value;
				SubscribeAutoReloadEvent();
			}
		}
		#endregion
		protected virtual IList ObtainIListFromDataSource(object dataSource, string dataMember) {
			return DataHelper.GetList(dataSource, dataMember);
		}
		protected internal virtual void SetStorageCore(ISchedulerStorageBase storage) {
			this.storage = storage;
		}
		protected internal virtual void SetInnerMappingsCore(MappingInfoBase<T> value) {
			UnsubscribeMappingInfoEvents();
			this.mappings = value;
			SubscribeMappingInfoEvents();
			RecreateMappings();
		}
		protected internal virtual void SetInnerCustomFieldMappingsCore(CustomFieldMappingCollectionBase<T> value) {
			UnsubscribeCustomMappingsEvents();
			this.customFieldMappings = value;
			SubscribeCustomMappingsEvents();
			RecreateMappings();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool UnboundMode { get { return ObtainIListFromDataSource(this.dataSource, this.dataMember) == null; } }
		#region DataSource
		[
		DefaultValue(null),
		Category(SRCategoryNames.Data)
		]
#if !SL
		[AttributeProvider(typeof(IListSource))]
#endif
		[NotifyParentProperty(true), AutoFormatDisable()]
		public virtual object DataSource {
			get { return dataSource; }
			set {
				if (dataSource == value)
					return;
				dataSource = value;
				ActivateDataSource();
			}
		}
		#endregion
		#region DataMember
#if !SL
		[Editor(ControlConstants.DataMemberEditor, typeof(System.Drawing.Design.UITypeEditor))]
#endif
		[
		DefaultValue(""), Category(SRCategoryNames.Data),
		NotifyParentProperty(true), AutoFormatDisable()]
		public virtual string DataMember {
			get { return dataMember; }
			set {
				if (value == null)
					value = String.Empty;
				if (dataMember == value)
					return;
#if !SL
				ValidateDataSource(DataSource, value);
#endif
				dataMember = value;
				ActivateDataSource();
			}
		}
		#endregion
		#region Filter
		[
		XtraSerializableProperty(), DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		NotifyParentProperty(true), AutoFormatDisable()]
		public virtual string Filter {
			get { return filter; }
			set {
				string newValue = value ?? String.Empty;
				if (filter == newValue)
					return;
				if (IsFilterValid(newValue)) {
					filter = newValue;
					RaiseFilterChanged();
				} else
					Exceptions.ThrowArgumentException("Filter", newValue);
			}
		}
		#endregion
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false)]
		public virtual bool ShouldUpdateAfterInsert { get; set; }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.dataManager != null) {
					UnsubscribeAutoReloadEvent();
					this.dataManager.Dispose();
					this.dataManager = null;
				}
				if (this.unboundDataKeeper != null) {
					if (unboundDataKeeper is IDisposable)
						((IDisposable)this.unboundDataKeeper).Dispose();
					this.unboundDataKeeper = null;
				}
				if (items != null)
					UnsubscribeCollectionEvents();
				if (mappings != null && customFieldMappings != null)
					UnsubscribeMappingsEvents();
				this.mappings = null;
				this.actualMappings = null;
				this.customFieldMappings = null;
				this.batchUpdateHelper = null;
				this.items = null;
				this.dataSource = null;
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~PersistentObjectStorage() {
			Dispose(false);
		}
		#endregion
		#region IBatchUpdateable implementation
		[Obsolete("Use the SchedulerStorage.BeginUpdate method instead.")]
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		[Obsolete("Use the SchedulerStorage.EndUpdate method instead.")]
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		[Obsolete("Use the SchedulerStorage.CancelUpdate method instead.")]
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper {
			get { return batchUpdateHelper; }
		}
		void IBatchUpdateable.BeginUpdate() {
			BeginUpdateInternal();
		}
		void IBatchUpdateable.EndUpdate() {
			EndUpdateInternal();
		}
		void IBatchUpdateable.CancelUpdate() {
			CancelUpdateInternal();
		}
		protected internal void BeginUpdateInternal() {
			batchUpdateHelper.BeginUpdate();
		}
		protected internal void EndUpdateInternal() {
			batchUpdateHelper.EndUpdate();
		}
		protected internal void CancelUpdateInternal() {
			batchUpdateHelper.CancelUpdate();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
		#region Events
		#region Reload
		PersistentObjectStorageReloadEventHandler onReload;
		event PersistentObjectStorageReloadEventHandler IInternalPersistentObjectStorage<T>.Reload {
			add {
				onReload = (PersistentObjectStorageReloadEventHandler)DelegateHelper.RemoveAll(onReload, value);
				onReload += value;
			}
			remove { onReload -= value; }
		}
		public virtual void RaiseReload(bool keepNonPersistentInformation) {
			if (onReload != null) {
				PersistentObjectStorageReloadEventArgs args = new PersistentObjectStorageReloadEventArgs(keepNonPersistentInformation);
				onReload(this, args);
			}
		}
		#endregion
		#region MappingsChanged
		EventHandler onMappingsChanged;
		public event EventHandler MappingsChanged { add { onMappingsChanged += value; } remove { onMappingsChanged -= value; } }
		protected internal virtual void RaiseMappingsChanged() {
			if (onMappingsChanged != null)
				onMappingsChanged(this, EventArgs.Empty);
		}
		#endregion
		#region AutoReloading
		CancelListChangedEventHandler onAutoReloading;
		public event CancelListChangedEventHandler AutoReloading {
			add {
				onAutoReloading = (CancelListChangedEventHandler)DelegateHelper.RemoveAll(onAutoReloading, value);
				onAutoReloading += value;
			}
			remove { onAutoReloading -= value; }
		}
		protected internal virtual bool RaiseAutoReloading(ListChangedEventArgs e) {
			if (onAutoReloading != null) {
				CancelListChangedEventArgs args = new CancelListChangedEventArgs(e.ListChangedType, e.NewIndex, e.OldIndex);
				onAutoReloading(this, args);
				return !args.Cancel;
			} else
				return true;
		}
		#endregion
		#region ObjectInserting
		PersistentObjectCancelEventHandler onObjectInserting;
		public event PersistentObjectCancelEventHandler ObjectInserting { add { onObjectInserting += value; } remove { onObjectInserting -= value; } }
		protected internal virtual bool RaiseObjectInserting(T obj) {
			if (ShouldRecreateCustomFields(obj))
				CreateCustomFields(obj);
			if (onObjectInserting != null) {
				PersistentObjectCancelEventArgs args = new PersistentObjectCancelEventArgs(obj);
				onObjectInserting(this, args);
				return !args.Cancel;
			} else
				return true;
		}
		#endregion
		#region ObjectDeleting
		PersistentObjectCancelEventHandler onObjectDeleting;
		public event PersistentObjectCancelEventHandler ObjectDeleting { add { onObjectDeleting += value; } remove { onObjectDeleting -= value; } }
		protected internal virtual bool RaiseObjectDeleting(T obj) {
			if (onObjectDeleting != null) {
				PersistentObjectCancelEventArgs args = new PersistentObjectCancelEventArgs(obj);
				onObjectDeleting(this, args);
				return !args.Cancel;
			} else
				return true;
		}
		#endregion
		#region InternalObjectChanging
		event PersistentObjectStateChangingEventHandler InternalObjectChanging;
		event PersistentObjectStateChangingEventHandler IInternalPersistentObjectStorage<T>.InternalObjectChanging { add { InternalObjectChanging += value; } remove { InternalObjectChanging -= value; } }
		protected internal virtual void RaiseInternalObjectChanging(PersistentObjectStateChangingEventArgs args) {
			if (InternalObjectChanging != null)
				InternalObjectChanging(this, args);
		}
		#endregion
		#region ObjectChanging
		PersistentObjectCancelEventHandler onObjectChanging;
		public event PersistentObjectCancelEventHandler ObjectChanging { add { onObjectChanging += value; } remove { onObjectChanging -= value; } }
		protected internal virtual bool RaiseObjectChanging(T obj) {
			if (onObjectChanging != null) {
				PersistentObjectCancelEventArgs args = new PersistentObjectCancelEventArgs(obj);
				onObjectChanging(this, args);
				return !args.Cancel;
			} else
				return true;
		}
		#endregion
		#region ObjectInserted
		PersistentObjectEventHandler onObjectInserted;
		public event PersistentObjectEventHandler ObjectInserted { add { onObjectInserted += value; } remove { onObjectInserted -= value; } }
		protected internal virtual void RaiseObjectInserted(T obj) {
			if (onObjectInserted != null) {
				PersistentObjectEventArgs args = new PersistentObjectEventArgs(obj);
				onObjectInserted(this, args);
			}
		}
		#endregion
		#region ObjectDeleted
		PersistentObjectEventHandler onObjectDeleted;
		public event PersistentObjectEventHandler ObjectDeleted { add { onObjectDeleted += value; } remove { onObjectDeleted -= value; } }
		protected internal virtual bool RaiseObjectDeleted(T obj) {
			if (onObjectDeleted != null) {
				PersistentObjectEventArgs args = new PersistentObjectEventArgs(obj);
				onObjectDeleted(this, args);
				return true;
			} else
				return false;
		}
		#endregion
		#region ObjectChanged
		PersistentObjectEventHandler onObjectChanged;
		public event PersistentObjectEventHandler ObjectChanged { add { onObjectChanged += value; } remove { onObjectChanged -= value; } }
		protected internal virtual void RaiseObjectChanged(T obj) {
			if (onObjectChanged != null) {
				PersistentObjectEventArgs args = new PersistentObjectEventArgs(obj);
				onObjectChanged(this, args);
			}
		}
		#endregion
		#region ObjectCollectionCleared
		EventHandler onObjectCollectionCleared;
		public event EventHandler ObjectCollectionCleared { add { onObjectCollectionCleared += value; } remove { onObjectCollectionCleared -= value; } }
		protected internal virtual void RaiseObjectCollectionCleared() {
			if (onObjectCollectionCleared != null)
				onObjectCollectionCleared(this, EventArgs.Empty);
		}
		#endregion
		#region ObjectCollectionLoaded
		EventHandler onObjectCollectionLoaded;
		public event EventHandler ObjectCollectionLoaded { add { onObjectCollectionLoaded += value; } remove { onObjectCollectionLoaded -= value; } }
		protected internal virtual void RaiseObjectCollectionLoaded() {
			if (onObjectCollectionLoaded != null)
				onObjectCollectionLoaded(this, EventArgs.Empty);
		}
		#endregion
		#region FilterChanged
		EventHandler onFilterChanged;
		public event EventHandler FilterChanged { add { onFilterChanged += value; } remove { onFilterChanged -= value; } }
		protected internal virtual void RaiseFilterChanged() {
			if (onFilterChanged != null)
				onFilterChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		#region Pseudo ISupportInitialize implementation
		public virtual void BeginInit() {
			this.initializing++;
		}
		public virtual void EndInit() {
			this.initializing--;
			ActivateDataSource();
		}
		#endregion
#if !SL
		#region Design time
		void ValidateDataSource(object dataSource, string dataMember) {
			if (IsDesignTime() && !IsLoading)
				DataHelper.ValidateBindingContext(dataSource, dataMember);
		}
		bool IsDesignTime() {
#if SL
			return false;
#else
			if (Storage == null)
				return false;
			return ((IInternalSchedulerStorageBase)storage).DesignModeProtected;
#endif
		}
		#endregion
#endif
		protected internal void ActivateDataSource() {
			if (IsLoading)
				return;
			RecreateMappings();
			OnDataSourceChanged();
		}
		protected internal virtual void SubscribeCollectionEvents() {
			items.CollectionChanging += new CollectionChangingEventHandler<T>(OnItemsChanging);
			items.CollectionChanged += new CollectionChangedEventHandler<T>(OnItemsChanged);
		}
		protected internal virtual void UnsubscribeCollectionEvents() {
			items.CollectionChanging -= new CollectionChangingEventHandler<T>(OnItemsChanging);
			items.CollectionChanged -= new CollectionChangedEventHandler<T>(OnItemsChanged);
		}
		protected internal virtual void SubscribeObjectEvents(T obj) {
			IInternalPersistentObject implementor = (IInternalPersistentObject)obj;
			implementor.StateChanging += new PersistentObjectStateChangingEventHandler(OnObjectStateChanging);
			implementor.StateChanged += new PersistentObjectStateChangedEventHandler(OnObjectStateChanged);
		}
		protected internal virtual void UnsubscribeObjectEvents(T obj) {
			IInternalPersistentObject implementor = (IInternalPersistentObject)obj;
			implementor.StateChanging -= new PersistentObjectStateChangingEventHandler(OnObjectStateChanging);
			implementor.StateChanged -= new PersistentObjectStateChangedEventHandler(OnObjectStateChanged);
		}
		protected internal virtual void SubscribeMappingsEvents() {
			SubscribeMappingInfoEvents();
			SubscribeCustomMappingsEvents();
		}
		protected void SubscribeMappingInfoEvents() {
			if (mappings == null)
				return;
			mappings.Changed += new EventHandler(OnMappingsChanged);
			mappings.QueryPersistentObjectStorage += new QueryPersistentObjectStorageEventHandler<T>(OnQueryPersistentObjectStorage);
		}
		protected void SubscribeCustomMappingsEvents() {
			if (customFieldMappings == null)
				return;
			customFieldMappings.CollectionChanged += new CollectionChangedEventHandler<MappingBase>(OnCustomMappingsChanged);
			customFieldMappings.QueryPersistentObjectStorage += new QueryPersistentObjectStorageEventHandler<T>(OnQueryPersistentObjectStorage);
		}
		protected internal virtual void UnsubscribeMappingsEvents() {
			UnsubscribeMappingInfoEvents();
			UnsubscribeCustomMappingsEvents();
		}
		protected internal virtual void UnsubscribeMappingInfoEvents() {
			if (mappings == null)
				return;
			mappings.Changed -= new EventHandler(OnMappingsChanged);
			mappings.QueryPersistentObjectStorage -= new QueryPersistentObjectStorageEventHandler<T>(OnQueryPersistentObjectStorage);
		}
		protected internal virtual void UnsubscribeCustomMappingsEvents() {
			if (customFieldMappings == null)
				return;
			customFieldMappings.CollectionChanged -= new CollectionChangedEventHandler<MappingBase>(OnCustomMappingsChanged);
			customFieldMappings.QueryPersistentObjectStorage -= new QueryPersistentObjectStorageEventHandler<T>(OnQueryPersistentObjectStorage);
		}
		protected internal virtual void SubscribeAutoReloadEvent() {
			if (!AutoReload)
				return;
			dataManager.AutoReload += new ListChangedEventHandler(OnAutoReload);
		}
		protected internal virtual void UnsubscribeAutoReloadEvent() {
			dataManager.AutoReload -= new ListChangedEventHandler(OnAutoReload);
		}
		protected internal virtual void UnsubscribeAndDisposeAllObjects() {
			int count = Items.Count;
			for (int i = 0; i < count; i++) {
				T item = Items[i];
				UnsubscribeObjectEvents(item);
				item.Dispose();
			}
		}
		protected internal virtual bool ShouldRecreateCustomFields(T obj) {
			if (obj.IsDisposed)
				return false;
			if (CustomFieldMappings == null)
				return false;
			return obj.CustomFields.Count != CustomFieldMappings.Count;
		}
		#region CommitNewObject
		public virtual void CommitNewObject(T obj) {
			UnsubscribeAutoReloadEvent();
			try {
				dataManager.CommitNewObject(obj, ActualMappings);
				if (!UnboundMode && ShouldUpdateAfterInsert)
					dataManager.UpdateAfterInsert(obj, ActualMappings);
			} finally {
				SubscribeAutoReloadEvent();
			}
		}
		#endregion
		#region CommitExistingObject
		public virtual void CommitExistingObject(T obj) {
			UnsubscribeAutoReloadEvent();
			try {
				dataManager.CommitExistingObject(obj, ActualMappings);
			} finally {
				SubscribeAutoReloadEvent();
			}
		}
		#endregion
		#region RollbackExistingObject
		public virtual void RollbackExistingObject(T obj) {
			dataManager.RollbackExistingObject(obj, ActualMappings);
		}
		#endregion
		#region DeleteExistingObject
		protected internal virtual void DeleteExistingObject(T obj) {
			UnsubscribeAutoReloadEvent();
			try {
				dataManager.DeleteExistingObject(obj, items, ActualMappings);
			} finally {
				SubscribeAutoReloadEvent();
			}
		}
		#endregion
		#region ClearAllObjects
		protected internal virtual void ClearAllObjects() {
			UnsubscribeAutoReloadEvent();
			try {
				dataManager.ClearAllObjects(ActualMappings);
			} finally {
				SubscribeAutoReloadEvent();
			}
		}
		#endregion
		#region OnItemsChanging
		protected internal virtual void OnItemsChanging(object sender, CollectionChangingEventArgs<T> e) {
			switch (e.Action) {
				case CollectionChangedAction.Changed:
					break;
				case CollectionChangedAction.EndBatchUpdate:
					break;
				case CollectionChangedAction.Clear:
					UnsubscribeAndDisposeAllObjects();
					ClearAllObjects();
					break;
				case CollectionChangedAction.Add:
					e.Cancel = !OnItemInserting(e.Element);
					break;
				case CollectionChangedAction.Remove:
					e.Cancel = !((IInternalPersistentObject)e.Element).CanDelete();
					break;
			}
		}
		protected internal virtual bool OnItemInserting(T obj) {
			return RaiseObjectInserting(obj);
		}
		#endregion
		#region OnItemsChanged
		protected internal virtual void OnItemsChanged(object sender, CollectionChangedEventArgs<T> e) {
			switch (e.Action) {
				case CollectionChangedAction.Add:
					OnItemInserted(e.Element);
					break;
				case CollectionChangedAction.Remove:
					((IInternalPersistentObject)e.Element).DeleteCore();
					break;
				case CollectionChangedAction.Clear:
					RaiseObjectCollectionCleared();
					break;
				case CollectionChangedAction.Changed:
					break;
				case CollectionChangedAction.EndBatchUpdate:
					break;
			}
		}
		protected internal virtual void OnItemInserted(T obj) {
			SubscribeObjectEvents(obj);
			CommitNewObject(obj);
			SafeRaiseObjectPostEvent(obj, obj, RaiseObjectInserted);
		}
		#endregion
		#region OnObjectStateChanging
		protected internal virtual void OnObjectStateChanging(object sender, PersistentObjectStateChangingEventArgs e) {
			XtraSchedulerDebug.Assert(e.Object != null);
			switch (e.State) {
				case PersistentObjectState.ChildCreated:
					e.Cancel = !OnObjectChildCreating((T)sender, (T)e.Object);
					break;
				case PersistentObjectState.ChildDeleted:
					e.Cancel = !OnObjectChildDeleting((T)sender, (T)e.Object);
					break;
				case PersistentObjectState.Changed:
					{
						HandleInternalObjectChanging(e);
						e.Cancel = !OnObjectChanging((T)sender, (T)e.Object);
						break;
					}
				case PersistentObjectState.Deleted:
					e.Cancel = !OnObjectDeleting((T)sender, (T)e.Object);
					break;
				case PersistentObjectState.RollbackState:
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		protected virtual void HandleInternalObjectChanging(PersistentObjectStateChangingEventArgs e) {
		}
		bool OnObjectChildCreating(T sender, T obj) {
			UnsubscribeObjectEvents(sender);
			try {
				return RaiseObjectInserting(obj);
			} finally {
				SubscribeObjectEvents(sender);
			}
		}
		bool OnObjectChildDeleting(T sender, T obj) {
			return SafeRaiseObjectPreEvent(sender, obj, RaiseObjectDeleting);
		}
		bool OnObjectChanging(T sender, T obj) {
			return SafeRaiseObjectPreEvent(sender, obj, RaiseObjectChanging);
		}
		bool OnObjectDeleting(T sender, T obj) {
			return SafeRaiseObjectPreEvent(sender, obj, RaiseObjectDeleting);
		}
		#endregion
		#region OnObjectStateChanged
		protected internal virtual void OnObjectStateChanged(object sender, PersistentObjectStateChangedEventArgs e) {
			XtraSchedulerDebug.Assert(e.Object != null);
			switch (e.State) {
				case PersistentObjectState.ChildCreated:
					OnObjectChildCreated((T)sender, (T)e.Object);
					break;
				case PersistentObjectState.ChildDeleted:
					OnObjectChildDeleted((T)sender, (T)e.Object);
					break;
				case PersistentObjectState.Deleted:
					OnObjectDeleted((T)sender, (T)e.Object);
					break;
				case PersistentObjectState.Changed:
					OnObjectChanged((T)sender, (T)e.Object);
					break;
				case PersistentObjectState.RollbackState:
					OnRollbackObjectState((T)sender, (T)e.Object);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		protected internal virtual void OnObjectChildCreated(T sender, T obj) {
			CommitNewObject(obj);
			SafeRaiseObjectPostEvent(sender, obj, RaiseObjectInserted);
		}
		protected internal delegate void RaisePersistentObjectPostEventHandler(T obj);
		protected internal delegate bool RaisePersistentObjectPreEventHandler(T obj);
		protected internal virtual bool SafeRaiseObjectPreEvent(T sender, T obj, RaisePersistentObjectPreEventHandler handler) {
			UnsubscribeObjectEvents(sender);
			try {
				return handler(obj);
			} finally {
				SubscribeObjectEvents(sender);
			}
		}
		protected internal virtual void SafeRaiseObjectPostEvent(T sender, T obj, RaisePersistentObjectPostEventHandler handler) {
			PersistentObjectUserModificationsRegistrator<T> registrator = new PersistentObjectUserModificationsRegistrator<T>(this);
			registrator.SubscribeObjectEvents(sender);
			UnsubscribeObjectEvents(sender);
			try {
				handler(obj);
			} finally {
				SubscribeObjectEvents(sender);
				registrator.UnsubscribeObjectEvents(sender);
			}
		}
		protected internal virtual void OnObjectChildDeleted(T sender, T obj) {
			DeleteExistingObject(obj);
			UnsubscribeObjectEvents(obj);
			if (!RaiseObjectDeleted(obj))
				obj.Dispose();
		}
		protected internal virtual void OnObjectChanged(T sender, T obj) {
			CommitExistingObject(obj);
			SafeRaiseObjectPostEvent(sender, obj, RaiseObjectChanged);
		}
		protected internal virtual void OnObjectDeleted(T sender, T obj) {
			int index = Items.IndexOf(obj);
			if (index >= 0) {
				UnsubscribeCollectionEvents();
				try {
					Items.RemoveAt(index);
				} finally {
					SubscribeCollectionEvents();
				}
			}
			DeleteExistingObject(obj);
			UnsubscribeObjectEvents(obj);
			if (!RaiseObjectDeleted(obj))
				obj.Dispose();
		}
		protected internal virtual void OnRollbackObjectState(T sender, T obj) {
			UnsubscribeObjectEvents(sender);
			try {
				RollbackExistingObject(obj);
			} finally {
				SubscribeObjectEvents(sender);
			}
		}
		#endregion
		protected internal virtual void RecreateMappings() {
			AppendBaseMappings(ActualMappings);
			AppendCustomMappings();
			UpdateKeyFieldName();
		}
		protected internal virtual void RecreateUnboundDataKeeperList() {
			UnboundDataKeeper.Activate(ActualMappings);
		}
		protected internal virtual void UpdateDataManagerListSource(IList list) {
			DataManager.ListSource = list;
		}
		public virtual StringCollection GetColumnNames() {
			List<String> names = new List<String>();
			try {
#if !SL
				PropertyDescriptorCollection properties = DataHelper.GetItemProperties(DataManager.ListSource, "");
				if (properties != null) {
					int count = properties.Count;
					for (int i = 0; i < count; i++)
						names.Add(properties[i].Name);
				}
#endif
			} catch {
			}
			names.Sort();
			StringCollection result = new StringCollection();
			result.AddRange(names.ToArray());
			return result;
		}
		public virtual void AppendBaseMappings(MappingCollection target) {
			target.Clear();
			if (UnboundMode)
				AppendDefaultMappings(target);
			else
				AppendMappings(target);
		}
		public virtual void AppendDefaultMappings(MappingCollection target) {
			mappings.AddDefaultMappings(target, true);
		}
		public virtual void AppendMappings(MappingCollection target) {
			mappings.AddMappings(target, true);
		}
		public virtual void AppendCustomMappings() {
			ActualMappings.AddRange(CustomFieldMappings);
		}
		protected internal virtual void OnAutoReload(object sender, ListChangedEventArgs e) {
			if (RaiseAutoReloading(e))
				RaiseReload(true);
		}
		protected internal virtual void OnCustomMappingsChanged(object sender, CollectionChangedEventArgs<MappingBase> e) {
			RecreateMappings();
			RaiseMappingsChanged();
			if (UnboundMode) {
				RecreateUnboundDataKeeperList();
				UpdateDataManagerListSource(UnboundDataKeeper.List);
			}
			RaiseReload(true);
		}
		protected internal virtual void OnMappingsChanged(object sender, EventArgs e) {
			if (IsLoading)
				return;
			RecreateMappings();
			RaiseMappingsChanged();
			bool keepNonPersistentInformation = !OnDataSourceChangedCore();
			RaiseReload(keepNonPersistentInformation);
		}
		protected internal virtual void OnDataSourceChanged() {
			if (OnDataSourceChangedCore()) {
				RaiseReload(false);
			}
		}
		protected internal virtual bool OnDataSourceChangedCore() {
			IList list = CalculateCurrentListSource();
			if (Object.Equals(dataManager.ListSource, list))
				return false;
			if (UnboundMode) {
				RecreateUnboundDataKeeperList();
				list = UnboundDataKeeper.List;
			}
			UnsubscribeAutoReloadEvent();
			try {
				UpdateDataManagerListSource(list);
			} finally {
				SubscribeAutoReloadEvent();
			}
			return true;
		}
		protected internal virtual void OnQueryPersistentObjectStorage(object sender, QueryPersistentObjectStorageEventArgs<T> e) {
			e.ObjectStorage = this;
		}
		internal IList CalculateCurrentListSource() {
			IList result;
			if (UnboundMode)
				result = unboundDataKeeper.List;
			else {
				result = ObtainIListFromDataSource(DataSource, DataMember);
				if (result == null)
					result = unboundDataKeeper.List;
			}
			return result;
		}
		public virtual void LoadObjects(bool keepNonPersistentInformation) {
			LoadObjectsCoreHandler loadPersistentObjects = delegate () { this.LoadObjectsFromDataManager(); };
			LoadObjectsCore(keepNonPersistentInformation, loadPersistentObjects);
		}
		void IInternalPersistentObjectStorage<T>.LoadObjectsCore(bool keepNonPersistentInformation, LoadObjectsCoreHandler handler) {
			LoadObjectsCore(keepNonPersistentInformation, handler);
		}
		protected virtual void LoadObjectsCore(bool keepNonPersistentInformation, LoadObjectsCoreHandler handler) {
			ObjectsNonPersistentInformation nonPersistentInfo;
			if (keepNonPersistentInformation)
				nonPersistentInfo = CreateNonPersistentInformation();
			else
				nonPersistentInfo = null;
			UnsubscribeAndDisposeAllObjects();
			UnsubscribeCollectionEvents();
			isObjectCollectionLoading = true;
			try {
				Items.Clear();
				RaiseObjectCollectionCleared();
				handler();
				if (keepNonPersistentInformation)
					ApplyNonPersistentInformation(nonPersistentInfo);
				SubscribeAllObjectsEvents();
			} finally {
				SubscribeCollectionEvents();
				isObjectCollectionLoading = false;
			}
			RaiseObjectCollectionLoaded();
		}
		protected internal virtual void AddLoadedObjectToCollection(T obj) {
			if (IsLoadedObjectValid(obj))
				items.Add(obj);
			else {
				if (obj != null)
					obj.Dispose();
			}
		}
		protected internal virtual bool IsLoadedObjectValid(T obj) {
			return obj != null;
		}
		protected internal virtual void LoadObjectsFromDataManager() {
			items.Capacity = dataManager.SourceObjectCount;
			items.BeginUpdate();
			try {
				LoadObjectsFromDataManagerCore();
			} finally {
				items.CancelUpdate();
			}
		}
		protected virtual void LoadObjectsFromDataManagerCore() {
			int count = dataManager.SourceObjectCount;
			if (ActualMappings.Count <= 0)
				return;
			for (int i = 0; i < count; i++) {
				T obj = LoadObjectFromDataManager(DataManager.GetSourceObjectHandle(i));
				AddLoadedObjectToCollection(obj);
			}
		}
		protected internal virtual T LoadObjectFromDataManager(object rowHandle) {
			T obj = CreateObject(rowHandle);
			if (obj != null) {
				obj.RowHandle = rowHandle;
				dataManager.LoadExistingObjectProperties(obj, ActualMappings);
			}
			return obj;
		}
		protected internal virtual void SubscribeAllObjectsEvents() {
			int count = Items.Count;
			for (int i = 0; i < count; i++)
				SubscribeObjectEvents(Items[i]);
		}
		public virtual void CreateCustomFields(T obj) {
			obj.CustomFields.Clear();
			int count = customFieldMappings.Count;
			for (int i = 0; i < count; i++) {
				CustomField field = new CustomField();
				field.Name = customFieldMappings[i].Name;
				obj.CustomFields.Add(field);
			}
		}
		public void Clear() {
			Items.Clear();
		}
		#region Direct Data Access Methods
		public object GetObjectRow(T obj) {
			return dataManager.GetObjectRow(obj);
		}
		public object GetObjectValue(T obj, string columnName) {
			return dataManager.GetObjectValue(obj, columnName);
		}
		public virtual void SetObjectValue(T obj, string columnName, object val) {
			dataManager.SetObjectValue(obj, columnName, val);
			SynchronizeCustomFieldValue(obj, columnName, val);
		}
		#endregion
		protected internal virtual void SynchronizeCustomFieldValue(T obj, string columnName, object val) {
			string name = null;
			int count = customFieldMappings.Count;
			for (int i = 0; i < count; i++) {
				if (customFieldMappings[i].Member == columnName) {
					name = customFieldMappings[i].Name;
					break;
				}
			}
			if (!String.IsNullOrEmpty(name)) {
				CustomField field = obj.CustomFields.GetFieldByName(name);
				field.Value = val;
			}
		}
		public virtual void ValidateDataSource() {
			this.dataManager.TryRePopulateColumns();
			StringCollection errors = new StringCollection();
			DataFieldInfoCollection fields = DataManager.GetFieldInfos();
			ValidateDataSourceCore(errors, fields, ActualMappings);
		}
		void IInternalPersistentObjectStorage<T>.ValidateDataSourceCore(StringCollection errors, DataFieldInfoCollection fields, MappingCollection mappings) {
			ValidateDataSourceCore(errors, fields, mappings);
		}
		protected virtual void ValidateDataSourceCore(StringCollection errors, DataFieldInfoCollection fields, MappingCollection mappings) {
			CheckRequiredMappings(mappings, errors);
			ValidateMappingMembers(mappings, errors, fields);
			CheckDuplicateMappingMembers(mappings, errors, fields);
		}
		protected internal virtual void CheckRequiredMappings(MappingCollection mappings, StringCollection errors) {
			string[] requiredMappings = Mappings.GetRequiredMappingNames();
			int count = requiredMappings.Length;
			for (int i = 0; i < count; i++) {
				MappingBase mapping = mappings[requiredMappings[i]];
				if (mapping == null)
					errors.Add(GetRequiredMappingErrorText(requiredMappings[i]));
			}
		}
		protected internal virtual string GetRequiredMappingErrorText(string mappingName) {
			return String.Format(SchedulerLocalizer.GetString(SchedulerStringId.Msg_MissingRequiredMapping), mappingName);
		}
		protected internal virtual void ValidateMappingMembers(MappingCollection mappings, StringCollection errors, DataFieldInfoCollection fields) {
			int count = mappings.Count;
			for (int i = 0; i < count; i++)
				ValidateMappingMember(errors, mappings[i], fields);
		}
		protected internal virtual void ValidateMappingMember(StringCollection errors, MappingBase mapping, DataFieldInfoCollection fields) {
			Predicate<DataFieldInfo> predicate = delegate (DataFieldInfo fld) { return fld.Name == mapping.Member; };
			DataFieldInfo field = fields.Find(predicate);
			if (field == null)
				errors.Add(GetMissingMemberErrorText(mapping));
		}
		protected internal virtual string GetMissingMemberErrorText(MappingBase mapping) {
			return String.Format(SchedulerLocalizer.GetString(SchedulerStringId.Msg_MissingMappingMember), mapping.Name, mapping.Member);
		}
		protected internal virtual void CheckDuplicateMappingMembers(MappingCollection mappings, StringCollection errors, DataFieldInfoCollection fields) {
			Dictionary<DataFieldInfo, MappingCollection> table = CreateEmptyMappingMemberTable(fields);
			PopulateMappingMemberTable(mappings, fields, table);
			foreach (DataFieldInfo field in table.Keys) {
				MappingCollection fieldMappings = table[field];
				if (fieldMappings.Count > 1)
					errors.Add(GetDuplicateMappingMemberErrorText(fieldMappings));
			}
		}
		protected internal virtual Dictionary<DataFieldInfo, MappingCollection> CreateEmptyMappingMemberTable(DataFieldInfoCollection fields) {
			Dictionary<DataFieldInfo, MappingCollection> table = new Dictionary<DataFieldInfo, MappingCollection>();
			int count = fields.Count;
			for (int i = 0; i < count; i++)
				table.Add(fields[i], new MappingCollection());
			return table;
		}
		protected internal virtual void PopulateMappingMemberTable(MappingCollection mappings, DataFieldInfoCollection fields, Dictionary<DataFieldInfo, MappingCollection> table) {
			int count = mappings.Count;
			for (int i = 0; i < count; i++) {
				MappingBase mapping = mappings[i];
				Predicate<DataFieldInfo> predicate = delegate (DataFieldInfo fld) { return fld.Name == mapping.Member; };
				DataFieldInfo field = fields.Find(predicate);
				if (field != null)
					table[field].Add(mapping);
			}
		}
		protected internal virtual string GetDuplicateMappingMemberErrorText(MappingCollection mappings) {
			int count = mappings.Count;
			if (count <= 0)
				return String.Empty;
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(SchedulerLocalizer.GetString(SchedulerStringId.Msg_DuplicateMappingMember), mappings[0].Member);
			sb.AppendFormat(mappings[0].Name);
			for (int i = 1; i < count; i++)
				sb.AppendFormat(", {0}", mappings[i].Name);
			return sb.ToString();
		}
		#region IsFilterValid
		protected bool IsFilterValid(string filter) {
			if (String.IsNullOrEmpty(filter))
				return true;
			Object op = CriteriaOperator.TryParse(filter);
			return (op != null);
		}
		#endregion
		protected abstract NotificationCollection<T> CreateCollection();
		protected internal abstract DataManager<T> CreateDataManager();
		protected internal abstract MappingInfoBase<T> CreateMappingInfo();
		protected internal abstract CustomFieldMappingCollectionBase<T> CreateCustomMappingsCollection();
		protected internal abstract ObjectsNonPersistentInformation CreateNonPersistentInformation();
		protected internal abstract void ApplyNonPersistentInformation(ObjectsNonPersistentInformation nonPersistentInfo);
		protected internal abstract void UpdateKeyFieldName();
		protected internal abstract T CreateObject(object rowHandle);
		protected abstract ISchedulerUnboundDataKeeper CreateUnboundDataKeeper();
		protected internal virtual void Assign(PersistentObjectStorage<T> storage) {
			if (storage == null)
				return;
			BeginUpdateInternal();
			try {
				Items.Clear();
				for (int i = 0; i < storage.Items.Count; i++) {
					Items.Add(CloneItem(storage.Items[0]));
				}
				AutoReload = storage.AutoReload;
				Filter = storage.Filter;
				ShouldUpdateAfterInsert = storage.ShouldUpdateAfterInsert;
			} finally {
				BeginUpdateInternal();
			}
		}
		protected T CloneItem(T item) {
			T result = (T)Activator.CreateInstance(item.GetType());
			result.Assign(item);
			return result;
		}
		#region IDataControllerData Members
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() {
			return new UnboundColumnInfoCollection();
		}
		object IDataControllerData.GetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
			return null;
		}
		void IDataControllerData.SetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
		}
		#endregion
		#region IDataControllerData2 Members
		bool IDataControllerData2.CanUseFastProperties { get { return IsFastPropertiesSupport; } }
		void IDataControllerData2.SubstituteFilter(SubstituteFilterEventArgs args) { }
		bool IDataControllerData2.HasUserFilter { get { return false; } }
		bool? IDataControllerData2.IsRowFit(int listSourceRow, bool fit) {
			return null;
		}
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			ComplexColumnInfoCollection result = new ComplexColumnInfoCollection();
			int count = ActualMappings.Count;
			for (int i = 0; i < count; i++) {
				string member = ActualMappings[i].Member;
				if (member.Contains(".") && DataManager.DataController.Columns[member] == null)
					result.Add(member);
			}
			return result;
		}
		PropertyDescriptorCollection IDataControllerData2.PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			int count = ActualMappings.Count;
			for (int i = 0; i < count; i++) {
				string member = ActualMappings[i].Member;
				if (collection.Find(member, false) == null)
					collection.Find(member, true);
			}
			return collection;
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region LoadObjectsCoreHandler
	public delegate void LoadObjectsCoreHandler();
	#endregion
	#region SchedulerStorageDeferredChanges
	public class SchedulerStorageDeferredChanges {
		#region Fields
		bool enableReminders;
		bool clearAppointments;
		bool clearResources;
		bool clearDependencies;
		bool loadAppointments;
		bool raiseAppointmentsLoaded;
		bool raiseAppointmentsModified;
		bool keepAppointmentsNonPersistentInformation = true;
		bool loadResources;
		bool raiseResourcesLoaded;
		bool keepResourcesNonPersistentInformation = true;
		bool resourceVisibilityChanged;
		bool loadDependencies;
		bool raiseDependenciesLoaded;
		bool keepDependenciesNonPersistentInformation = true;
		bool suppressDeferredNotifications;
		bool appointmentMappingsChanged;
		bool resourceMappingsChanged;
		bool dependencyMappingsChanged;
		bool appointmentUIObjectsChanged;
		bool dependencyUIObjectsChanged;
		AppointmentBaseCollection insertedAppointments;
		AppointmentBaseCollection changedAppointments;
		AppointmentBaseCollection deletedAppointments;
		ResourceBaseCollection insertedResources;
		ResourceBaseCollection changedResources;
		ResourceBaseCollection deletedResources;
		AppointmentDependencyBaseCollection insertedDependencies;
		AppointmentDependencyBaseCollection changedDependencies;
		AppointmentDependencyBaseCollection deletedDependencies;
		#endregion
		#region Properties
		public bool EnableReminders { get { return enableReminders; } set { enableReminders = value; } }
		public bool ClearAppointments { get { return clearAppointments; } }
		public bool ClearResources { get { return clearResources; } }
		public bool ClearDependencies { get { return clearDependencies; } }
		public bool LoadAppointments { get { return loadAppointments; } }
		public bool RaiseAppointmentsLoaded { get { return raiseAppointmentsLoaded; } }
		public bool RaiseAppointmentsModified { get { return raiseAppointmentsModified; } }
		public bool KeepAppointmentsNonPersistentInformation { get { return keepAppointmentsNonPersistentInformation; } }
		public bool LoadResources { get { return loadResources; } }
		public bool RaiseResourcesLoaded { get { return raiseResourcesLoaded; } }
		public bool KeepResourcesNonPersistentInformation { get { return keepResourcesNonPersistentInformation; } }
		public bool LoadDependencies { get { return loadDependencies; } }
		public bool RaiseDependenciesLoaded { get { return raiseDependenciesLoaded; } }
		public bool KeepDependenciesNonPersistentInformation { get { return keepDependenciesNonPersistentInformation; } }
		public AppointmentBaseCollection InsertedAppointments { get { return insertedAppointments; } }
		public AppointmentBaseCollection ChangedAppointments { get { return changedAppointments; } }
		public AppointmentBaseCollection DeletedAppointments { get { return deletedAppointments; } }
		public ResourceBaseCollection InsertedResources { get { return insertedResources; } }
		public ResourceBaseCollection ChangedResources { get { return changedResources; } }
		public ResourceBaseCollection DeletedResources { get { return deletedResources; } }
		public AppointmentDependencyBaseCollection InsertedDependencies { get { return insertedDependencies; } }
		public AppointmentDependencyBaseCollection ChangedDependencies { get { return changedDependencies; } }
		public AppointmentDependencyBaseCollection DeletedDependencies { get { return deletedDependencies; } }
		public bool ResourceVisibilityChanged { get { return resourceVisibilityChanged; } }
		public bool SuppressDeferredNotifications { get { return suppressDeferredNotifications; } set { suppressDeferredNotifications = value; } }
		public bool AppointmentMappingsChanged { get { return appointmentMappingsChanged; } }
		public bool ResourceMappingsChanged { get { return resourceMappingsChanged; } }
		public bool DependencyMappingsChanged { get { return dependencyMappingsChanged; } }
		public bool AppointmentUIObjectsChanged { get { return appointmentUIObjectsChanged; } }
		public bool DependencyUIObjectsChanged { get { return dependencyUIObjectsChanged; } }
		#endregion
		public SchedulerStorageDeferredChanges() {
			this.insertedAppointments = new AppointmentBaseCollection();
			this.insertedAppointments.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
			this.changedAppointments = new AppointmentBaseCollection();
			this.changedAppointments.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
			this.deletedAppointments = new AppointmentBaseCollection();
			this.deletedAppointments.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
			this.insertedResources = new ResourceBaseCollection();
			this.insertedResources.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
			this.changedResources = new ResourceBaseCollection();
			this.changedResources.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
			this.deletedResources = new ResourceBaseCollection();
			this.deletedResources.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
			this.insertedDependencies = new AppointmentDependencyBaseCollection();
			this.insertedDependencies.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
			this.changedDependencies = new AppointmentDependencyBaseCollection();
			this.changedDependencies.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
			this.deletedDependencies = new AppointmentDependencyBaseCollection();
			this.deletedDependencies.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
		}
		#region Register(*)Appointment
		static void ValidateAppointment(Appointment apt) {
			if (apt == null)
				Exceptions.ThrowArgumentException("apt", apt);
		}
		public void RegisterInsertedAppointment(Appointment apt) {
			ValidateAppointment(apt);
			if (changedAppointments.Contains(apt) || deletedAppointments.Contains(apt))
				Exceptions.ThrowInternalException();
			insertedAppointments.Add(apt);
		}
		public void RegisterChangedAppointment(Appointment apt) {
			ValidateAppointment(apt);
			if (deletedAppointments.Contains(apt))
				Exceptions.ThrowInternalException();
			changedAppointments.Add(apt);
		}
		public void RegisterDeletedAppointment(Appointment apt) {
			ValidateAppointment(apt);
			int index = insertedAppointments.IndexOf(apt);
			if (index >= 0)
				insertedAppointments.RemoveAt(index);
			index = changedAppointments.IndexOf(apt);
			if (index >= 0)
				changedAppointments.RemoveAt(index);
			deletedAppointments.Add(apt);
		}
		public void RegisterClearAppointments() {
			insertedAppointments.Clear();
			changedAppointments.Clear();
			deletedAppointments.Clear();
			clearAppointments = true;
			loadAppointments = false;
			raiseAppointmentsLoaded = false;
			keepAppointmentsNonPersistentInformation = false;
		}
		public void RegisterLoadAppointments(bool keepNonPersistentInformation) {
			insertedAppointments.Clear();
			changedAppointments.Clear();
			deletedAppointments.Clear();
			loadAppointments = true;
			raiseAppointmentsLoaded = true;
			clearAppointments = false;
			if (keepAppointmentsNonPersistentInformation)
				keepAppointmentsNonPersistentInformation = keepNonPersistentInformation;
		}
		public void RegisterRaiseAppointmentCollectionLoaded() {
			raiseAppointmentsLoaded = true;
		}
		public void RegisterRaiseAppointmentCollectionModified() {
			this.raiseAppointmentsModified = true;
		}
		public void RegisterAppointmentMappingsChanged() {
			this.appointmentMappingsChanged = true;
		}
		public void RegisterAppointmentUIObjectsChanged() {
			this.appointmentUIObjectsChanged = true;
		}
		#endregion
		#region Register(*)Resource
		static void ValidateResource(Resource resource) {
			if (resource == null)
				Exceptions.ThrowArgumentException("resource", resource);
		}
		public void RegisterInsertedResource(Resource resource) {
			ValidateResource(resource);
			if (changedResources.Contains(resource) || deletedResources.Contains(resource))
				Exceptions.ThrowInternalException();
			insertedResources.Add(resource);
			resourceVisibilityChanged = true;
		}
		public void RegisterChangedResource(Resource resource) {
			ValidateResource(resource);
			if (deletedResources.Contains(resource))
				Exceptions.ThrowInternalException();
			changedResources.Add(resource);
			resourceVisibilityChanged = true;
		}
		public void RegisterDeletedResource(Resource resource) {
			ValidateResource(resource);
			int index = insertedResources.IndexOf(resource);
			if (index >= 0)
				insertedResources.RemoveAt(index);
			index = changedResources.IndexOf(resource);
			if (index >= 0)
				changedResources.RemoveAt(index);
			deletedResources.Add(resource);
			resourceVisibilityChanged = true;
		}
		public void RegisterResourceVisibilityChange() {
			resourceVisibilityChanged = true;
		}
		public void RegisterClearResources() {
			insertedResources.Clear();
			changedResources.Clear();
			deletedResources.Clear();
			clearResources = true;
			loadResources = false;
			raiseResourcesLoaded = false;
			resourceVisibilityChanged = true;
			keepResourcesNonPersistentInformation = false;
		}
		public void RegisterLoadResources(bool keepNonPersistentInformation) {
			insertedResources.Clear();
			changedResources.Clear();
			deletedResources.Clear();
			loadResources = true;
			raiseResourcesLoaded = true;
			clearResources = false;
			resourceVisibilityChanged = true;
			if (keepResourcesNonPersistentInformation)
				keepResourcesNonPersistentInformation = keepNonPersistentInformation;
		}
		public void RegisterRaiseResourceCollectionLoaded() {
			raiseResourcesLoaded = true;
		}
		public void RegisterResourceMappingsChanged() {
			this.resourceMappingsChanged = true;
		}
		#endregion
		#region Register(*)Dependency
		static void ValidateDependency(AppointmentDependency aptDependency) {
			if (aptDependency == null)
				Exceptions.ThrowArgumentException("aptDependency", aptDependency);
		}
		public void RegisterInsertedDependency(AppointmentDependency aptDependency) {
			ValidateDependency(aptDependency);
			if (changedDependencies.Contains(aptDependency) || deletedDependencies.Contains(aptDependency))
				Exceptions.ThrowInternalException();
			insertedDependencies.Add(aptDependency);
		}
		public void RegisterChangedDependency(AppointmentDependency aptDependency) {
			ValidateDependency(aptDependency);
			if (deletedDependencies.Contains(aptDependency))
				Exceptions.ThrowInternalException();
			changedDependencies.Add(aptDependency);
		}
		public void RegisterDeletedDependency(AppointmentDependency aptDependency) {
			ValidateDependency(aptDependency);
			int index = insertedDependencies.IndexOf(aptDependency);
			if (index >= 0)
				insertedDependencies.RemoveAt(index);
			index = changedDependencies.IndexOf(aptDependency);
			if (index >= 0)
				changedDependencies.RemoveAt(index);
			deletedDependencies.Add(aptDependency);
		}
		public void RegisterClearDependencies() {
			insertedDependencies.Clear();
			changedDependencies.Clear();
			deletedDependencies.Clear();
			clearDependencies = true;
			loadDependencies = false;
			raiseDependenciesLoaded = false;
			keepDependenciesNonPersistentInformation = false;
		}
		public void RegisterLoadDependencies(bool keepNonPersistentInformation) {
			insertedDependencies.Clear();
			changedDependencies.Clear();
			deletedDependencies.Clear();
			loadDependencies = true;
			raiseDependenciesLoaded = true;
			clearDependencies = false;
			if (keepDependenciesNonPersistentInformation)
				keepDependenciesNonPersistentInformation = keepNonPersistentInformation;
		}
		public void RegisterRaiseDependencyCollectionLoaded() {
			raiseDependenciesLoaded = true;
		}
		public void RegisterDependencyMappingsChanged() {
			this.dependencyMappingsChanged = true;
		}
		public void RegisterDependencyUIObjectsChanged() {
			this.dependencyUIObjectsChanged = true;
		}
		#endregion
	}
	#endregion
	#region PersistentObjectUserModificationsRegistrator<T>
	public class PersistentObjectUserModificationsRegistrator<T> where T : IPersistentObject {
		IPersistentObjectStorage<T> storage;
		public PersistentObjectUserModificationsRegistrator(IPersistentObjectStorage<T> storage) {
			if (storage == null)
				Exceptions.ThrowArgumentException("storage", storage);
			this.storage = storage;
		}
		protected internal IPersistentObjectStorage<T> Storage { get { return storage; } }
		public virtual void SubscribeObjectEvents(T obj) {
			((IInternalPersistentObject)obj).StateChanged += new PersistentObjectStateChangedEventHandler(OnObjectStateChanged);
		}
		public virtual void UnsubscribeObjectEvents(T obj) {
			((IInternalPersistentObject)obj).StateChanged -= new PersistentObjectStateChangedEventHandler(OnObjectStateChanged);
		}
		protected internal virtual void OnObjectStateChanged(object sender, PersistentObjectStateChangedEventArgs e) {
			((IInternalPersistentObjectStorage<T>)storage).CommitExistingObject((T)e.Object);
		}
	}
	#endregion
	#region PersistentObjectUserModificationsDelayedRegistrator<T>
	public class PersistentObjectUserModificationsDelayedRegistrator<T> : PersistentObjectUserModificationsRegistrator<T> where T : IPersistentObject {
		bool objectWasChanged;
		public PersistentObjectUserModificationsDelayedRegistrator(IPersistentObjectStorage<T> storage)
			: base(storage) {
		}
		internal bool ObjectWasChanged { get { return objectWasChanged; } }
		protected internal override void OnObjectStateChanged(object sender, PersistentObjectStateChangedEventArgs e) {
			this.objectWasChanged = true;
		}
	}
	#endregion
	#region AppointmentMultiClientCacheItem (abstract class)
	public abstract class AppointmentMultiClientCacheItem : IDisposable {
		readonly AppointmentCache cache;
		protected AppointmentMultiClientCacheItem(AppointmentCache cache) {
			this.cache = cache;
		}
		public AppointmentCache Cache { get { return cache; } }
		public bool IsAlive { get { return KeyObject != null; } }
		public abstract object KeyObject { get; }
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~AppointmentMultiClientCacheItem() {
			Dispose(false);
		}
		protected internal abstract void Dispose(bool disposing);
	}
	#endregion
	#region FullTrustAppointmentMultiClientCacheItem
	public class FullTrustAppointmentMultiClientCacheItem : AppointmentMultiClientCacheItem {
		static GCHandle zeroHandle;
		static FullTrustAppointmentMultiClientCacheItem() {
			StaticInitialize();
		}
		[SecuritySafeCritical]
		static GCHandle CreateHandler() {
			return GCHandle.Alloc(new Object());
		}
		[SecuritySafeCritical]
		static GCHandle CreateHandler(object obj) {
			return GCHandle.Alloc(obj, GCHandleType.Weak);
		}
		[SecuritySafeCritical]
		static void StaticInitialize() {
			GCHandle h = CreateHandler();
			h.Free();
			zeroHandle = h;
		}
		GCHandle handle;
		public FullTrustAppointmentMultiClientCacheItem(object obj, AppointmentCache cache)
			: base(cache) {
			this.handle = CreateHandler(obj);
		}
		public override object KeyObject {
			get {
				return GetKeyObject();
			}
		}
		[SecuritySafeCritical]
		object GetKeyObject() {
			GCHandle h = handle;
			if (!h.IsAllocated)
				return null;
			object res = h.Target;
			return handle.IsAllocated ? res : null;
		}
		protected internal override void Dispose(bool disposing) {
			SafeDispose();
		}
		[SecuritySafeCritical]
		void SafeDispose() {
			GCHandle h = handle;
			handle = zeroHandle;
			if (h.IsAllocated)
				h.Free();
		}
	}
	#endregion
	#region PartialTrustAppointmentMultiClientCacheItem
	public class PartialTrustAppointmentMultiClientCacheItem : AppointmentMultiClientCacheItem {
		object key;
		public PartialTrustAppointmentMultiClientCacheItem(object key, AppointmentCache cache)
			: base(cache) {
			this.key = key;
		}
		public override object KeyObject { get { return key; } }
		protected internal override void Dispose(bool disposing) {
			this.key = null;
		}
	}
	#endregion
	public class AppointmentMultiClientCache {
		readonly List<AppointmentMultiClientCacheItem> cacheTable = new List<AppointmentMultiClientCacheItem>();
		internal List<AppointmentMultiClientCacheItem> CacheTable { get { return cacheTable; } }
		public virtual void CreateCache(object callerObject) {
			XtraSchedulerDebug.Assert(!(callerObject is ValueType));
			AppointmentCache cache = CreateNewCache();
			AppointmentMultiClientCacheItem item = CreateCacheItem(callerObject, cache);
			cacheTable.Add(item);
		}
		protected internal virtual AppointmentMultiClientCacheItem CreateCacheItem(object key, AppointmentCache cache) {
#if (!SL)
			if (SecurityHelper.IsPartialTrust)
				return new PartialTrustAppointmentMultiClientCacheItem(key, cache);
			else
				return new FullTrustAppointmentMultiClientCacheItem(key, cache);
#else
			return new PartialTrustAppointmentMultiClientCacheItem(key, cache);
#endif
		}
		public virtual void DeleteCache(object callerObject) {
			int index = GetActualAppointmentCacheIndex(callerObject);
			if (index >= 0) {
				AppointmentMultiClientCacheItem item = cacheTable[index];
				item.Dispose();
				cacheTable.RemoveAt(index);
			}
		}
		protected internal virtual AppointmentCache GetActualAppointmentCache(object callerObject) {
			return GetActualAppointmentCacheCore(callerObject);
		}
		protected internal virtual AppointmentCache GetActualAppointmentCacheCore(object callerObject) {
			int index = GetActualAppointmentCacheIndex(callerObject);
			if (index >= 0)
				return cacheTable[index].Cache;
			else
				return null;
		}
		protected internal virtual int GetActualAppointmentCacheIndex(object callerObject) {
			XtraSchedulerDebug.Assert(!(callerObject is ValueType));
			int count = cacheTable.Count;
			for (int i = count - 1; i >= 0; i--) {
				AppointmentMultiClientCacheItem item = cacheTable[i];
				if (!item.IsAlive) {
					item.Dispose();
					cacheTable.RemoveAt(i);
				}
				if (Object.ReferenceEquals(item.KeyObject, callerObject))
					return i;
			}
			return -1;
		}
		protected internal virtual void CleanDeadCacheItems() {
			int count = cacheTable.Count;
			for (int i = count - 1; i >= 0; i--) {
				AppointmentMultiClientCacheItem item = cacheTable[i];
				if (!item.IsAlive) {
					cacheTable.RemoveAt(i);
					item.Dispose();
				}
			}
		}
		protected internal virtual AppointmentCache CreateNewCache() {
			return new AppointmentCache();
		}
		protected internal virtual void BeginUpdate() {
			CleanDeadCacheItems();
			foreach (AppointmentMultiClientCacheItem item in cacheTable)
				item.Cache.BeginUpdate();
		}
		protected internal virtual void EndUpdate() {
			CleanDeadCacheItems();
			foreach (AppointmentMultiClientCacheItem item in cacheTable)
				item.Cache.EndUpdate();
		}
		public virtual void OnAppointmentCollectionCleared() {
			CleanDeadCacheItems();
			foreach (AppointmentMultiClientCacheItem item in cacheTable)
				item.Cache.OnAppointmentCollectionCleared();
		}
		public virtual void OnAppointmentCollectionLoaded() {
			CleanDeadCacheItems();
			foreach (AppointmentMultiClientCacheItem item in cacheTable)
				item.Cache.OnAppointmentCollectionLoaded();
		}
		public virtual void OnAppointmentsInserted(AppointmentBaseCollection appointments) {
			CleanDeadCacheItems();
			foreach (AppointmentMultiClientCacheItem item in cacheTable)
				item.Cache.OnAppointmentsInserted(appointments);
		}
		public virtual void OnAppointmentsChanged(AppointmentBaseCollection appointments) {
			CleanDeadCacheItems();
			foreach (AppointmentMultiClientCacheItem item in cacheTable)
				item.Cache.OnAppointmentsChanged(appointments);
		}
		public virtual void OnAppointmentsDeleted(AppointmentBaseCollection appointments) {
			CleanDeadCacheItems();
			foreach (AppointmentMultiClientCacheItem item in cacheTable)
				item.Cache.OnAppointmentsDeleted(appointments);
		}
		public virtual bool CacheHit(object callerObject, TimeInterval interval) {
			AppointmentCache cache = GetActualAppointmentCache(callerObject);
			if (cache == null)
				return false;
			else
				return cache.CacheHit(interval);
		}
		public virtual AppointmentBaseCollection GetAppointments(object callerObject, TimeInterval interval) {
			AppointmentCache cache = GetActualAppointmentCache(callerObject);
			if (cache == null)
				return new AppointmentBaseCollection();
			else
				return cache.GetAppointments(interval);
		}
		public virtual void SetContent(object callerObject, AppointmentBaseCollection appointments, TimeInterval interval) {
			AppointmentCache cache = GetActualAppointmentCache(callerObject);
			if (cache != null)
				cache.SetContent(appointments, interval);
		}
	}
	#region AppointmentCache
	public class AppointmentCache : IBatchUpdateable, IBatchUpdateHandler {
		#region Fields
		BatchUpdateHelper batchUpdateHelper;
		bool isCacheValid;
		AppointmentBaseCollection appointments;
		TimeInterval interval = TimeInterval.Empty;
		bool deferredSort;
		#endregion
		public AppointmentCache() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.appointments = new AppointmentBaseCollection();
			this.appointments.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
		}
		#region Properties
		internal bool IsCacheValid { get { return isCacheValid; } set { isCacheValid = value; } }
		internal AppointmentBaseCollection Appointments { get { return appointments; } }
		internal TimeInterval Interval { get { return interval; } set { interval = value; } }
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
			deferredSort = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (deferredSort)
				SortContent();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			if (deferredSort)
				SortContent();
		}
		#endregion
		public virtual bool CacheHit(TimeInterval interval) {
			return isCacheValid && this.interval.Contains(interval);
		}
		public virtual AppointmentBaseCollection GetAppointments(TimeInterval interval) {
			if (this.interval.Equals(interval)) {
				AppointmentBaseCollection result = new AppointmentBaseCollection();
				result.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
				result.AddRange(Appointments);
				return result;
			} else
				return AppointmentBaseCollection.GetAppointmentsFromSortedCollection(this.appointments, interval);
		}
		public virtual void SetContent(AppointmentBaseCollection appointments, TimeInterval interval) {
			Reset();
			SetContentCore(appointments, interval);
		}
		protected internal virtual void SetContentCore(AppointmentBaseCollection appointments, TimeInterval interval) {
			this.interval = interval.Clone();
			this.appointments.AddRange(appointments);
			SortContent();
			this.isCacheValid = true;
		}
		protected internal virtual void SortContent() {
			if (IsUpdateLocked)
				deferredSort = true;
			else {
				AppointmentStartDateComparer comparer = new AppointmentStartDateComparer();
				this.appointments.Sort(comparer);
			}
		}
		protected internal virtual void Reset() {
			this.interval = TimeInterval.Empty;
			this.appointments.Clear();
			this.isCacheValid = false;
		}
		public virtual void OnAppointmentCollectionCleared() {
			Reset();
		}
		public virtual void OnAppointmentCollectionLoaded() {
			Reset();
		}
		protected bool MatchCacheInterval(Appointment apt) {
			return DevExpress.XtraScheduler.Internal.Implementations.AppointmentInstance.IsAppointmentIntersectsInterval(apt, this.interval);
		}
		public virtual void OnAppointmentsInserted(AppointmentBaseCollection appointments) {
			if (!IsCacheValid)
				return;
			appointments.ForEach(OnAppointmentInserted);
			SortContent();
		}
		public virtual void OnAppointmentsDeleted(AppointmentBaseCollection appointments) {
			if (!IsCacheValid)
				return;
			appointments.ForEach(OnAppointmentDeleted);
			SortContent();
		}
		public virtual void OnAppointmentsChanged(AppointmentBaseCollection appointments) {
			if (!IsCacheValid)
				return;
			appointments.ForEach(OnAppointmentChanged);
			SortContent();
		}
		protected internal virtual void OnAppointmentInserted(Appointment apt) {
			if (apt.Type == AppointmentType.Pattern)
				OnAppointmentPatternInserted(apt);
			else
				OnAppointmentInsertedCore(apt);
		}
		protected internal virtual void OnAppointmentDeleted(Appointment apt) {
			if (apt.Type == AppointmentType.Pattern)
				OnAppointmentPatternDeleted(apt);
			else
				OnAppointmentDeletedCore(apt);
		}
		protected internal virtual void OnAppointmentChanged(Appointment apt) {
			if (apt.Type == AppointmentType.Pattern)
				OnAppointmentPatternChanged(apt);
			else
				OnAppointmentChangedCore(apt);
		}
		protected internal virtual void OnAppointmentPatternInserted(Appointment apt) {
			AppendPatternOccurrencesOnly(apt);
		}
		protected internal virtual void OnAppointmentPatternDeleted(Appointment apt) {
			DeletePatternOccurrences(apt);
		}
		protected internal virtual void OnAppointmentPatternChanged(Appointment apt) {
			appointments.Remove(apt);
			DeletePatternOccurrences(apt);
			AppendPatternOccurrencesOnly(apt);
		}
		protected internal virtual void DeletePatternOccurrences(Appointment pattern) {
			int count = appointments.Count;
			for (int i = count - 1; i >= 0; i--) {
				Appointment apt = appointments[i];
				if (apt.Type == AppointmentType.Occurrence &&
					Object.ReferenceEquals(pattern, apt.RecurrencePattern))
					appointments.RemoveAt(i);
			}
		}
		protected internal virtual void AppendPatternOccurrencesOnly(Appointment pattern) {
			AppointmentPatternExpander expander = new AppointmentPatternExpander(pattern);
			this.appointments.AddRange(expander.ExpandSkipExceptions(this.interval));
		}
		protected internal virtual void OnAppointmentInsertedCore(Appointment apt) {
			if (apt.IsException) {
				DeletePatternOccurrences(apt.RecurrencePattern);
				appointments.Remove(apt);
				AppendPatternOccurrencesOnly(apt.RecurrencePattern);
				if (apt.Type == AppointmentType.DeletedOccurrence) {
					return;
				}
			}
			if (MatchCacheInterval(apt))
				appointments.Add(apt);
		}
		protected internal virtual void OnAppointmentDeletedCore(Appointment apt) {
			appointments.Remove(apt);
			if (apt.Type == AppointmentType.DeletedOccurrence) {
				Appointment pattern = apt.RecurrencePattern;
				if (pattern != null) {
					DeletePatternOccurrences(pattern);
					if (pattern.Type == AppointmentType.Pattern)
						AppendPatternOccurrencesOnly(pattern);
				}
				appointments.Remove(apt);
			}
		}
		protected internal virtual void OnAppointmentChangedCore(Appointment apt) {
			if (apt.Type == AppointmentType.DeletedOccurrence) {
				Appointment pattern = apt.RecurrencePattern;
				if (pattern != null) {
					DeletePatternOccurrences(pattern);
					AppendPatternOccurrencesOnly(pattern);
				}
				appointments.Remove(apt);
			} else {
				DeletePatternOccurrences(apt);
				int index = appointments.IndexOf(apt);
				if (index >= 0) {
					if (!MatchCacheInterval(apt))
						Appointments.RemoveAt(index);
				} else {
					if (MatchCacheInterval(apt))
						appointments.Add(apt);
				}
			}
		}
	}
	#endregion
	#region ObjectsNonPersistentInformation (abstract class)
	public abstract class ObjectsNonPersistentInformation {
	}
	#endregion
	public class SchedulerStorageReloadListener {
		bool shouldReload;
		public void OnReload(object sender, EventArgs e) {
			this.shouldReload = true;
		}
		public bool ShouldReload { get { return shouldReload; } set { shouldReload = value; } }
	}
}
namespace DevExpress.XtraScheduler.Xml {
	using System.Xml;
	public class StorageXmlConverter<T> where T : IPersistentObject {
		IPersistentObjectStorage<T> storage;
		CollectionXmlPersistenceHelper helper;
		public StorageXmlConverter(IPersistentObjectStorage<T> storage, CollectionXmlPersistenceHelper helper) {
			this.storage = storage;
			this.helper = helper;
		}
		public IPersistentObjectStorage<T> Storage { get { return storage; } }
		public CollectionXmlPersistenceHelper Helper { get { return helper; } }
		public void ReadXml(string fileName) {
			if (String.IsNullOrEmpty(fileName))
				return;
			if (!File.Exists(fileName))
				return;
			using (FileStream fs = File.OpenRead(fileName)) {
				ReadXml(fs);
			}
		}
		public void ReadXml(Stream stream) {
			if (!Storage.UnboundMode)
				throw new Exception(SchedulerLocalizer.GetString(SchedulerStringId.Msg_LoadCollectionFromXml));
			LoadObjectsCoreHandler loadPersistentObjects = delegate () {
				XmlDocument doc;
				doc = XmlDocumentHelper.LoadFromStream(stream);
				;
				ParseXmlDocument(XmlDocumentHelper.GetDocumentElement(doc));
			};
			((IInternalPersistentObjectStorage<T>)Storage).LoadObjectsCore(false, loadPersistentObjects);
		}
		protected virtual void ParseXmlDocument(XmlNode root) {
			helper.ParseXmlDocument(root);
		}
		public void WriteXml(string fileName) {
			using (FileStream fs = File.Create(fileName)) {
				WriteXml(fs);
			}
		}
		public void WriteXml(Stream stream) {
			helper.WriteXml(stream);
		}
	}
}
