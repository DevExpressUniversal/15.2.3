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
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Native {
	public abstract class SchedulerStorageBaseClientDataProvider : IRangeControlClientDataProvider, ISupportObjectChanged {
		ISchedulerStorageBase storage;
		protected SchedulerStorageBaseClientDataProvider(ISchedulerStorageBase storage) {
			this.storage = storage;
			SubscribeStorageEvents();
		}
		protected internal ISchedulerStorageBase Storage { get { return storage; } }
		protected virtual IComparer<Appointment> CreateComparer() {
			return SortedAppointmentBaseCollection.DefaultComparer;
		}
		#region IRangeControlClientDataProvider members
		public IRangeControlClientSyncSupport SyncSupport { get; set; }
		public virtual DateTime SelectedRangeStart { get { return GetSelectedRangeStart(); } }
		public virtual DateTime SelectedRangeEnd { get { return GetSelectedRangeEnd(); } }
		void IRangeControlClientDataProvider.OnOptionsChanged(string name, object oldValue, object newValue) {
			OnOptionsChangedCore(name, oldValue, newValue);
		}
		void IRangeControlClientDataProvider.OnSelectedRangeChanged(DateTime rangeMinimum, DateTime rangeMaximum) {
			OnSelectedRangeChangedCore(rangeMinimum, rangeMaximum);
		}
		List<DataItemThumbnailList> IRangeControlClientDataProvider.CreateThumbnailData(TimeIntervalCollection intervals) {
			return CreateThumbnailDataCore(intervals);
		}
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (storage != null) {
					UnsubscribeStorageEvents();
					storage = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SchedulerStorageBaseClientDataProvider() {
			Dispose(false);
		}
		#endregion
		protected virtual void SyncRangeControl() {
			SyncRangeControl(true);
		}
		protected virtual void SyncRangeControl(bool forceRequestData) {
			if (SyncSupport == null)
				return;
			SyncSupport.RefreshRangeControl(forceRequestData);
		}
		protected internal void SetStorageInternal(ISchedulerStorageBase storage) {
			if (Storage == storage)
				return;
			UnsubscribeStorageEvents();
			this.storage = storage;
			SubscribeStorageEvents();
		}
		#region Events
		#region SubscribeStorageEvents
		protected internal virtual void SubscribeStorageEvents() {
			IInternalSchedulerStorageBase internalStorage = storage as IInternalSchedulerStorageBase;
			if (internalStorage == null)
				return;
			internalStorage.InternalAppointmentCollectionCleared += new EventHandler(OnAppointmentsCollectionChanged);
			internalStorage.InternalAppointmentCollectionLoaded += new EventHandler(OnAppointmentsCollectionChanged);
			internalStorage.InternalAppointmentsDeleted += new PersistentObjectsEventHandler(OnAppointmentsChanged);
			internalStorage.InternalAppointmentsInserted += new PersistentObjectsEventHandler(OnAppointmentsChanged);
			internalStorage.InternalAppointmentsChanged += new PersistentObjectsEventHandler(OnAppointmentsChanged);
			internalStorage.InternalResourcesChanged += new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesDeleted += new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesInserted += new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourceCollectionCleared += new EventHandler(OnResourcesCollectionChanged);
			internalStorage.InternalResourceCollectionLoaded += new EventHandler(OnResourcesCollectionChanged);
			internalStorage.InternalResourceVisibilityChanged += new EventHandler(OnResourceVisibilityChanged);
			internalStorage.InternalDeferredNotifications += new EventHandler(OnDeferredNotifications);
		}
		#endregion
		#region UnsubscribeStorageEvents
		protected internal virtual void UnsubscribeStorageEvents() {
			IInternalSchedulerStorageBase internalStorage = storage as IInternalSchedulerStorageBase;
			if (internalStorage == null)
				return;
			internalStorage.InternalAppointmentCollectionCleared -= new EventHandler(OnAppointmentsCollectionChanged);
			internalStorage.InternalAppointmentCollectionLoaded -= new EventHandler(OnAppointmentsCollectionChanged);
			internalStorage.InternalAppointmentsDeleted -= new PersistentObjectsEventHandler(OnAppointmentsChanged);
			internalStorage.InternalAppointmentsInserted -= new PersistentObjectsEventHandler(OnAppointmentsChanged);
			internalStorage.InternalAppointmentsChanged -= new PersistentObjectsEventHandler(OnAppointmentsChanged);
			internalStorage.InternalResourcesChanged -= new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesDeleted -= new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesInserted -= new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourceCollectionCleared -= new EventHandler(OnResourcesCollectionChanged);
			internalStorage.InternalResourceCollectionLoaded -= new EventHandler(OnResourcesCollectionChanged);
			internalStorage.InternalResourceVisibilityChanged -= new EventHandler(OnResourceVisibilityChanged);
			internalStorage.InternalDeferredNotifications -= new EventHandler(OnDeferredNotifications);
		}
		#endregion
		protected internal virtual void OnAppointmentsCollectionChanged(object sender, EventArgs e) {
			SyncRangeControl();
		}
		protected internal virtual void OnAppointmentsChanged(object sender, PersistentObjectsEventArgs e) {
			SyncRangeControl();
		}
		protected internal virtual void OnResourceVisibilityChanged(object sender, EventArgs e) {
			SyncRangeControl();
		}
		protected internal virtual void OnDeferredNotifications(object sender, EventArgs e) {
			SyncRangeControl();
		}
		protected internal virtual void OnResourcesCollectionChanged(object sender, EventArgs e) {
			SyncRangeControl();
		}
		protected internal virtual void OnResourcesChanged(object sender, PersistentObjectsEventArgs e) {
			SyncRangeControl();
		}
		#endregion
		IScaleBasedRangeControlClientOptions IRangeControlClientDataProvider.GetOptions() {
			return GetOptionsCore();
		}
		protected abstract IScaleBasedRangeControlClientOptions GetOptionsCore();
		protected abstract DateTime GetSelectedRangeStart();
		protected abstract DateTime GetSelectedRangeEnd();
		protected abstract void OnSelectedRangeChangedCore(DateTime rangeMinimum, DateTime rangeMaximum);
		protected abstract Dictionary<TimeInterval, AppointmentBaseCollection> GetFilteredAppointmentByIntervals(TimeIntervalCollection intervals);
		protected abstract IDataItemThumbnail CreateDataItemThumbnailItem(Appointment apt);
		protected virtual void OnOptionsChangedCore(string name, object oldValue, object newValue) { }
		protected virtual List<DataItemThumbnailList> CreateThumbnailDataCore(TimeIntervalCollection intervals) {
			List<DataItemThumbnailList> result = new List<DataItemThumbnailList>();
			if (Storage == null)
				return result;
			Dictionary<TimeInterval, AppointmentBaseCollection> dict = GetFilteredAppointmentByIntervals(intervals);
			int count = intervals.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = intervals[i];
				DataItemThumbnailList item = CreateThumbnailItem(interval);
				InitThumbnailItem(item, dict[interval]);
				result.Add(item);
			}
			return result;
		}
		protected virtual DataItemThumbnailList CreateThumbnailItem(TimeInterval interval) {
			return new DataItemThumbnailList(interval);
		}
		protected virtual void InitThumbnailItem(DataItemThumbnailList item, AppointmentBaseCollection appointments) {
			appointments.Sort(CreateComparer());
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				IDataItemThumbnail thumbnailItem = CreateDataItemThumbnailItem(appointments[i]);
				item.Thumbnails.Insert(0, thumbnailItem);
			}
		}		
		#region ISupportObjectChanged implementation
		EventHandler onChanged;
		event EventHandler ISupportObjectChanged.Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal virtual void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
	}
}
