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
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.UI {
	#region IResourceFilterControl
	public interface IResourceFilterControl {
		void ResetResourcesItems(ResourceBaseCollection resources);
		void ResourceVisibleChanged(ResourceBaseCollection resources);
	}
	#endregion
	#region ResourceFilterController
	public class ResourceFilterController : IDisposable, IBatchUpdateable, IBatchUpdateHandler {
		#region Fields
		BatchUpdateHelper batchUpdateHelper;
		bool changed;
		bool isDisposed;
		IInnerSchedulerControlOwner innerControlOwner;
		ISchedulerStorageBase innerStorage;
		IResourceFilterControl control;
		ResourceBaseCollection availableResources;
		#endregion
		public ResourceFilterController(IResourceFilterControl control) {
			if (control == null)
				Exceptions.ThrowArgumentException("control", control);
			this.control = control;
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			ReloadAvailableResources();
		}
		#region Properties
		protected internal IResourceFilterControl Control { get { return control; } }
		protected internal ResourceBaseCollection AvailableResources { get { return availableResources; } }
		protected internal ISchedulerStorageBase InnerStorage { get { return innerStorage; } }
		IInternalSchedulerStorageBase InternalInnerStorage { get { return (IInternalSchedulerStorageBase)innerStorage; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool IsDisposed { get { return isDisposed; } }
		protected internal IInnerSchedulerControlOwner InnerControlOwner {
			get { return innerControlOwner; }
			set {
				if (innerControlOwner == value)
					return;
				UnsubscribeControlEvents();
				UnsubscribeStorageEvents();
				this.innerControlOwner = value;
				this.innerStorage = GetInnerSchedulerStorage();
				SubscribeControlEvents();
				SubscribeStorageEvents();
				ReloadAvailableResources();
			}
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
			this.changed = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (changed)
				UpdateRelatedControls();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				UnsubscribeControlEvents();
				UnsubscribeStorageEvents();
				this.innerControlOwner = null;
				this.innerStorage = null;
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ResourceFilterController() {
			Dispose(false);
		}
		#endregion
		#region SubscribeControlEvents
		protected internal virtual void SubscribeControlEvents() {
			if (InnerControlOwner == null)
				return;
			InnerControlOwner.StorageChanged += new EventHandler(OnStorageChanged);
			InnerControlOwner.BeforeDispose += new EventHandler(OnBeforeSchedulerControlDispose);
		}
		#endregion
		#region UnsubscribeControlEvents
		protected internal virtual void UnsubscribeControlEvents() {
			if (InnerControlOwner == null)
				return;
			InnerControlOwner.StorageChanged -= new EventHandler(OnStorageChanged);
			InnerControlOwner.BeforeDispose -= new EventHandler(OnBeforeSchedulerControlDispose);
		}
		#endregion
		#region SubscribeStorageEvents
		protected internal virtual void SubscribeStorageEvents() {
			IInternalSchedulerStorageBase internalStorage = InnerStorage as IInternalSchedulerStorageBase;
			if (internalStorage == null)
				return;
			internalStorage.InternalResourcesChanged += new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesDeleted += new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesInserted += new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourceCollectionCleared += new EventHandler(OnResourceCollectionChanged);
			internalStorage.InternalResourceCollectionLoaded += new EventHandler(OnResourceCollectionChanged);
			internalStorage.InternalResourceVisibilityChanged += new EventHandler(OnResourceVisibilityChanged);
			internalStorage.InternalDeferredNotifications += new EventHandler(OnDeferredNotifications);
		}
		#endregion
		#region UnsubscribeStorageEvents
		protected internal virtual void UnsubscribeStorageEvents() {
			IInternalSchedulerStorageBase internalStorage = InnerStorage as IInternalSchedulerStorageBase;
			if (internalStorage == null)
				return;
			internalStorage.InternalResourcesChanged -= new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesDeleted -= new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesInserted -= new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourceCollectionCleared -= new EventHandler(OnResourceCollectionChanged);
			internalStorage.InternalResourceCollectionLoaded -= new EventHandler(OnResourceCollectionChanged);
			internalStorage.InternalResourceVisibilityChanged -= new EventHandler(OnResourceVisibilityChanged);
			internalStorage.InternalDeferredNotifications -= new EventHandler(OnDeferredNotifications);
		}
		#endregion
		protected internal virtual void OnDeferredNotifications(object sender, EventArgs e) {
			IInternalSchedulerStorageBase internalStorage = innerStorage as IInternalSchedulerStorageBase;
			SchedulerStorageDeferredChanges changes = internalStorage.DeferredChanges;
			if (changes == null)
				return;
			if (changes.ResourceVisibilityChanged) {
				if (ShouldResetResourcesItems(changes))
					ReloadAvailableResources();
				else
					Control.ResourceVisibleChanged(availableResources);
			}
		}
		protected internal virtual bool ShouldResetResourcesItems(SchedulerStorageDeferredChanges changes) {
			bool result = false;
			if (changes.ChangedResources.Count > 0)
				result = true;
			if (changes.DeletedResources.Count > 0)
				result = true;
			if (changes.InsertedResources.Count > 0)
				result = true;
			if (changes.ClearResources)
				result = true;
			if (changes.LoadResources)
				result = true;
			return result;
		}
		protected internal virtual void OnResourceVisibilityChanged(object sender, EventArgs e) {
			Control.ResourceVisibleChanged(availableResources);
		}
		protected internal virtual void OnResourcesChanged(object sender, PersistentObjectsEventArgs e) {
			ReloadAvailableResources();
		}
		protected internal virtual void OnResourceCollectionChanged(object sender, EventArgs e) {
			ReloadAvailableResources();
		}
		protected internal virtual void OnStorageChanged(object sender, EventArgs e) {
			UnsubscribeStorageEvents();
			this.innerStorage = GetInnerSchedulerStorage();
			SubscribeStorageEvents();
			ReloadAvailableResources();
		}
		protected internal virtual void OnBeforeSchedulerControlDispose(object sender, EventArgs e) {
			this.InnerControlOwner = null;
		}
		protected internal virtual ISchedulerStorageBase GetInnerSchedulerStorage() {
			return innerControlOwner != null ? innerControlOwner.Storage : null;
		}
		protected internal virtual void ReloadAvailableResources() {
			this.availableResources = GetFilteredResources();
			Control.ResetResourcesItems(availableResources);
		}
		protected internal virtual ResourceBaseCollection GetFilteredResources() {
			if (InnerStorage != null)
				return InternalInnerStorage.GetFilteredResources(true);
			else
				return new ResourceBaseCollection();
		}
		protected internal virtual bool GetResourceVisible(Resource resource) {
			return resource.Visible;
		}
		public virtual void SetResourceVisible(Resource resource, bool visible) {
			if (resource == null)
				Exceptions.ThrowArgumentException("resource", resource);
			if (GetResourceVisible(resource) == visible)
				return;
			SetResourceVisibleCore(resource, visible);
			UpdateRelatedControls();
		}
		protected internal virtual void SetResourceVisibleCore(Resource resource, bool visible) {
			resource.BeginUpdate();
			try {
				resource.Visible = visible;
			} finally {
				resource.CancelUpdate();
			}
		}
		protected internal virtual void UpdateRelatedControls() {
			if (IsUpdateLocked) {
				this.changed = true;
				return;
			}
			UpdateRelatedControlsCore();
		}
		protected internal virtual void UpdateRelatedControlsCore() {
			if (InnerStorage == null)
				return;
			UnsubscribeStorageEvents();
			try {
				IInternalSchedulerStorageBase internalStorage = InnerStorage as IInternalSchedulerStorageBase;
				if (internalStorage != null)
					internalStorage.RaiseInternalResourceVisibilityChanged();
			} finally {
				SubscribeStorageEvents();
			}
		}
		public virtual void SetAllResourcesVisible(bool visible) {
			BeginUpdate();
			try {
				SetAllResourcesVisibleCore(visible);
			} finally {
				EndUpdate();
			}
		}
		protected internal virtual void SetAllResourcesVisibleCore(bool visible) {
			int count = availableResources.Count;
			for (int i = 0; i < count; i++)
				SetResourceVisible(availableResources[i], visible);
		}
	}
	#endregion
	#region AppointmentResourcesEditResourceFilterController
	public class AppointmentResourcesEditResourceFilterController : ResourceFilterController {
		AppointmentResourceIdCollection resourceIds;
		public AppointmentResourcesEditResourceFilterController(IResourceFilterControl control)
			: base(control) {
		}
		internal AppointmentResourceIdCollection ResourceIds { get { return resourceIds; } set { resourceIds = value; } }
		#region Events
		#region BeginSetResourceVisibility
		EventHandler onBeginSetResourceVisibility;
		protected internal event EventHandler BeginSetResourceVisibility { add { onBeginSetResourceVisibility += value; } remove { onBeginSetResourceVisibility -= value; } }
		protected internal virtual void RaiseBeginSetResourceVisibility() {
			if (onBeginSetResourceVisibility != null)
				onBeginSetResourceVisibility(this, EventArgs.Empty);
		}
		#endregion
		#region EndSetResourceVisibility
		EventHandler onEndSetResourceVisibility;
		protected internal event EventHandler EndSetResourceVisibility { add { onEndSetResourceVisibility += value; } remove { onEndSetResourceVisibility -= value; } }
		protected internal virtual void RaiseEndSetResourceVisibility() {
			if (onEndSetResourceVisibility != null)
				onEndSetResourceVisibility(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal override bool GetResourceVisible(Resource resource) {
			if (resourceIds != null)
				return resourceIds.Contains(resource.Id);
			else
				return false;
		}
		protected internal override void SetResourceVisibleCore(Resource resource, bool visible) {
			if (resourceIds == null)
				return;
			RaiseBeginSetResourceVisibility();
			try {
				resourceIds.BeginUpdate();
				try {
					if (visible)
						resourceIds.Add(resource.Id);
					else
						resourceIds.Remove(resource.Id);
				} finally {
					resourceIds.EndUpdate();
				}
			} finally {
				RaiseEndSetResourceVisibility();
			}
		}
	}
	#endregion
}
