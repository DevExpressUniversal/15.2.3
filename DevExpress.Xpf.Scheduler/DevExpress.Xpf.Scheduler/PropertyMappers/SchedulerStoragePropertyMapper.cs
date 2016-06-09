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
using System.Windows;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
namespace DevExpress.Xpf.Scheduler.Native {
	#region SchedulerStoragePropertySyncManager
	public class SchedulerStoragePropertySyncManager : DependencyPropertySyncManager {
		SchedulerStorage storage;
		public SchedulerStoragePropertySyncManager(SchedulerStorage storage) {
			Guard.ArgumentNotNull(storage, "storage");
			this.storage = storage;
		}
		public SchedulerStorage Storage { get { return storage; } }
		public override void Register() {
			PropertyMapperTable.RegisterPropertyMapper(SchedulerStorage.RemindersCheckIntervalProperty,
				new RemindersCheckIntervalPropertyMapper(SchedulerStorage.RemindersCheckIntervalProperty, Storage));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerStorage.TimeZoneIdProperty, new TimeZoneIdPropertyMapper(SchedulerStorage.TimeZoneIdProperty, Storage));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerStorage.EnableTimeZonesProperty, new EnableTimeZonesPropertyMapper(SchedulerStorage.EnableTimeZonesProperty, Storage));
		}
	}
	#endregion
	#region PersistentObjectStoragePropertySyncManager
	public abstract class PersistentObjectStoragePropertySyncManager<T, U> : DependencyPropertySyncManager where T : IPersistentObjectStorage<U> where U : IPersistentObject {
		readonly PersistentObjectStorageBase<T, U> storage;
		protected PersistentObjectStoragePropertySyncManager(PersistentObjectStorageBase<T, U> storage) {
			Guard.ArgumentNotNull(storage, "storage");
			this.storage = storage;
		}
		public PersistentObjectStorageBase<T, U> Storage { get { return storage; } }
	}
	#endregion
	#region AppointmentStoragePropertySyncManager
	public class AppointmentStoragePropertySyncManager : PersistentObjectStoragePropertySyncManager<IAppointmentStorageBase, Appointment> {
		public AppointmentStoragePropertySyncManager(AppointmentStorage storage)
			: base(storage) {
		}
		public AppointmentStorage Appointments {
			get { return (AppointmentStorage)base.Storage; }
		}
		public override void Register() {
			PropertyMapperTable.RegisterPropertyMapper(AppointmentStorage.DataSourceProperty, new StorageDataSourcePropertyMapper<IAppointmentStorageBase, Appointment>(AppointmentStorage.DataSourceProperty, Storage));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentStorage.AutoReloadProperty, new StorageAutoReloadPropertyMapper<IAppointmentStorageBase, Appointment>(AppointmentStorage.AutoReloadProperty, Storage));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentStorage.ResourceSharingProperty, new AppointmentStorageResourceSharingPropertyMapper(AppointmentStorage.ResourceSharingProperty, Appointments));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentStorage.FilterProperty, new StorageFilterPropertyMapper<IAppointmentStorageBase, Appointment>(AppointmentStorage.FilterProperty, Appointments));
			PropertyMapperTable.RegisterPropertyMapper(AppointmentStorage.DataMemberProperty, new StorageDataMemberPropertyMapper<IAppointmentStorageBase, Appointment>(AppointmentStorage.DataMemberProperty, Appointments));			
			PropertyMapperTable.RegisterPropertyMapper(AppointmentStorage.CommitIdToDataSourceProperty, new AppointmentStorageCommitIdToDataSourcePropertyMapper(AppointmentStorage.CommitIdToDataSourceProperty, Appointments));
		}
	}
	#endregion
	#region ResourceStoragePropertySyncManager
	public class ResourceStoragePropertySyncManager : PersistentObjectStoragePropertySyncManager<IResourceStorageBase, Resource> {
		public ResourceStoragePropertySyncManager(ResourceStorage storage)
			: base(storage) {
		}
		public ResourceStorage Resources {
			get { return (ResourceStorage)base.Storage; }
		}
		public override void Register() {
			PropertyMapperTable.RegisterPropertyMapper(ResourceStorage.DataSourceProperty, new StorageDataSourcePropertyMapper<IResourceStorageBase, Resource>(ResourceStorage.DataSourceProperty, Storage));
			PropertyMapperTable.RegisterPropertyMapper(ResourceStorage.AutoReloadProperty, new StorageAutoReloadPropertyMapper<IResourceStorageBase, Resource>(ResourceStorage.AutoReloadProperty, Storage));
			PropertyMapperTable.RegisterPropertyMapper(ResourceStorage.FilterProperty, new StorageFilterPropertyMapper<IResourceStorageBase, Resource>(ResourceStorage.FilterProperty, Storage));
			PropertyMapperTable.RegisterPropertyMapper(ResourceStorage.DataMemberProperty, new StorageDataMemberPropertyMapper<IResourceStorageBase, Resource>(ResourceStorage.DataMemberProperty, Storage));
			PropertyMapperTable.RegisterPropertyMapper(ResourceStorage.ColorSavingProperty, new ResourceStorageColorSavingPropertyMapper(ResourceStorage.ColorSavingProperty, Resources));
		}
	}
	#endregion
	#region Storage mappers
	public abstract class SchedulerStoragePropertyMapperBase : DependencyPropertyMapperBase {
		readonly ISchedulerStorageBase innerStorage;
		protected SchedulerStoragePropertyMapperBase(DependencyProperty property, SchedulerStorage storage)
			: base(property, storage) {
			Guard.ArgumentNotNull(storage, "storage");
			this.innerStorage = storage.InnerStorage;
		}
		public ISchedulerStorageBase InnerStorage { get { return innerStorage; } }
	}
	public class RemindersCheckIntervalPropertyMapper : SchedulerStoragePropertyMapperBase {
		public RemindersCheckIntervalPropertyMapper(DependencyProperty property, SchedulerStorage storage)
			: base(property, storage) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerStorage.RemindersCheckInterval = (int)newValue;
		}
		public override object GetInnerPropertyValue() {
			return InnerStorage.RemindersCheckInterval;
		}
	}
	public class TimeZoneIdPropertyMapper : SchedulerStoragePropertyMapperBase {
		public TimeZoneIdPropertyMapper(DependencyProperty property, SchedulerStorage storage)
			: base(property, storage) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerStorage.TimeZoneId = (string)newValue;
		}
		public override object GetInnerPropertyValue() {
			return InnerStorage.TimeZoneId;
		}
	}
	public class EnableTimeZonesPropertyMapper : SchedulerStoragePropertyMapperBase {
		public EnableTimeZonesPropertyMapper(DependencyProperty property, SchedulerStorage storage)
			: base(property, storage) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerStorage.EnableTimeZones = (bool)newValue;
		}
		public override object GetInnerPropertyValue() {
			return InnerStorage.EnableTimeZones;
		}
	}
	public abstract class PersistentObjectStorageBasePropertyMapper<T, U> : DependencyPropertyMapperBase where T : IPersistentObjectStorage<U> where U : IPersistentObject {
		readonly IPersistentObjectStorage<U> innerStorage;
		protected PersistentObjectStorageBasePropertyMapper(DependencyProperty property, PersistentObjectStorageBase<T, U> storage)
			: base(property, storage) {
			Guard.ArgumentNotNull(storage, "storage");
			this.innerStorage = storage.InnerStorage;
		}
		public IPersistentObjectStorage<U> InnerStorage { get { return innerStorage; } }
		public PersistentObjectStorageBase<T, U> Storage { get { return (PersistentObjectStorageBase<T, U>)Owner; } }
	}
	public class StorageDataSourcePropertyMapper<T, U> : PersistentObjectStorageBasePropertyMapper<T, U> where T : IPersistentObjectStorage<U> where U : IPersistentObject {
		public StorageDataSourcePropertyMapper(DependencyProperty property, PersistentObjectStorageBase<T, U> storage)
			: base(property, storage) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerStorage.DataSource = newValue;
		}
		public override object GetInnerPropertyValue() {
			return InnerStorage.DataSource;
		}
	}
	public class StorageDataMemberPropertyMapper<T, U> : PersistentObjectStorageBasePropertyMapper<T, U> where T : IPersistentObjectStorage<U> where U : IPersistentObject {
		public StorageDataMemberPropertyMapper(DependencyProperty property, PersistentObjectStorageBase<T, U> storage)
			: base(property, storage) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerStorage.DataMember = (string)newValue;
		}
		public override object GetInnerPropertyValue() {
			return InnerStorage.DataMember;
		}
	}
	public class StorageAutoReloadPropertyMapper<T, U> : PersistentObjectStorageBasePropertyMapper<T, U> where T : IPersistentObjectStorage<U> where U : IPersistentObject {
		public StorageAutoReloadPropertyMapper(DependencyProperty property, PersistentObjectStorageBase<T, U> storage)
			: base(property, storage) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerStorage.AutoReload = (bool)newValue;
		}
		public override object GetInnerPropertyValue() {
			return InnerStorage.AutoReload;
		}
	}
	public class StorageFilterPropertyMapper<T, U> : PersistentObjectStorageBasePropertyMapper<T, U> where T : IPersistentObjectStorage<U> where U : IPersistentObject {
		public StorageFilterPropertyMapper(DependencyProperty property, PersistentObjectStorageBase<T, U> storage)
			: base(property, storage) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerStorage.Filter = (string)newValue;
		}
		public override object GetInnerPropertyValue() {
			return InnerStorage.Filter;
		}
	}
	public abstract class AppointmentStoragePropertyMapperBase : PersistentObjectStorageBasePropertyMapper<IAppointmentStorageBase, Appointment> {
		protected AppointmentStoragePropertyMapperBase(DependencyProperty property, AppointmentStorage storage)
			: base(property, storage) {
		}
		protected IAppointmentStorageBase AppointmentStorage { get { return (IAppointmentStorageBase)InnerStorage; } }
	}
	public class AppointmentStorageResourceSharingPropertyMapper : AppointmentStoragePropertyMapperBase {
		public AppointmentStorageResourceSharingPropertyMapper(DependencyProperty property, AppointmentStorage storage)
			: base(property, storage) {
		}
		public override object GetInnerPropertyValue() {
			return AppointmentStorage.ResourceSharing;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			AppointmentStorage.ResourceSharing = (bool)newValue;
		}
	}   
	public class AppointmentStorageCommitIdToDataSourcePropertyMapper : AppointmentStoragePropertyMapperBase {
		public AppointmentStorageCommitIdToDataSourcePropertyMapper(DependencyProperty property, AppointmentStorage storage)
			: base(property, storage) {
		}
		public override object GetInnerPropertyValue() {
			return AppointmentStorage.Mappings.CommitIdToDataSource;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			AppointmentStorage.Mappings.CommitIdToDataSource = (bool)newValue;
		}
	}
	public abstract class ResourceStoragePropertyMapperBase : PersistentObjectStorageBasePropertyMapper<IResourceStorageBase, Resource> {
		protected ResourceStoragePropertyMapperBase(DependencyProperty property, ResourceStorage storage)
			: base(property, storage) {
		}
		protected IResourceStorageBase ResourceStorage { get { return (IResourceStorageBase)InnerStorage; } }
	}
	public class ResourceStorageColorSavingPropertyMapper : ResourceStoragePropertyMapperBase {
		public ResourceStorageColorSavingPropertyMapper(DependencyProperty property, ResourceStorage storage)
			: base(property, storage) {
		}
		public override object GetInnerPropertyValue() {
			return ResourceStorage.ColorSaving;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			ResourceStorage.ColorSaving = (ColorSavingType)newValue;
		}
	}
	#endregion
}
