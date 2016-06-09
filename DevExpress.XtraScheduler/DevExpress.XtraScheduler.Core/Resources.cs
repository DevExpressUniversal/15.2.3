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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraScheduler.Data;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Xml;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
#if !SL
using System.Drawing.Design;
using System.Globalization;
#else
using System.Windows.Media;
using System.Windows.Controls;
#endif
namespace DevExpress.XtraScheduler {
	[Serializable]
	public static class ResourceEmpty {
		static EmptyResource emptyRes = null;
		public static EmptyResourceId Id {
			get { return EmptyResourceId.Id; }
		}
		public static Resource Resource {
			get { return emptyRes ?? (emptyRes = new EmptyResource()); }
		}
	}
	[Serializable]
	public sealed class EmptyResourceId {
		static EmptyResourceId id;
		EmptyResourceId() { }
		public static EmptyResourceId Id {
			get { return id ?? (id = new EmptyResourceId()); }
		}
	}
	public interface Resource : IPersistentObject {
		object ParentId { get; set; }
		string Caption { get; set; }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		string ColorValue { get; set; }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		byte[] ImageBytes { get; set; }
		bool Visible { get; set; }
		[Obsolete("Use the GetImage and SetImage extension methods instead.")]
		Image Image { get; set; }
		[Obsolete("Use the GetColor and SetColor extension methods instead.")]
		Color Color { get; set; }
	}
	public static class IResourceExtension {
		public static bool MatchIds(this Resource resource1, Resource resource2) {
			return ResourceBase.MatchIds(resource1, resource2);
		}
	}
	#region ResourceCollection
	public class ResourceCollection : ResourceBaseCollection {
		ISchedulerStorageBase storage;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ResourceCollection() {
			this.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
		}
		public ResourceCollection(ISchedulerStorageBase storage) {
			if (storage == null)
				Exceptions.ThrowArgumentException("storage", storage);
			this.storage = storage;
			this.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
		}
		public override void AddRange(ICollection collection) {
			AddRangeCore(collection);
		}
		public int Add(object id, string caption) {
			Resource resource = this.storage.CreateResource(id);
			resource.Caption = caption;
			return Add(resource);
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("ResourceCollectionStorage")]
#endif
		public ISchedulerStorageBase Storage { get { return storage; } }
		public virtual void ReadXml(string fileName) {
			CreateXmlConverter().ReadXml(fileName);
			CommitItems();
		}
		public virtual void ReadXml(Stream stream) {
			CreateXmlConverter().ReadXml(stream);
			CommitItems();
		}
		void CommitItems() {
			if (!storage.Resources.UnboundMode)
				return;
			for (int i = 0; i < Count; i++) {
				this[i].RowHandle = -1;
				((IInternalResourceStorage)storage.Resources).CommitNewObject(this[i]);
			}
		}
		public virtual void WriteXml(string fileName) {
			CreateXmlConverter().WriteXml(fileName);
		}
		public virtual void WriteXml(Stream stream) {
			CreateXmlConverter().WriteXml(stream);
		}
		protected internal StorageXmlConverter<Resource> CreateXmlConverter() {
			IResourceStorageBase objectStorage = storage.Resources;
			return new StorageXmlConverter<Resource>(objectStorage, new ResourceCollectionXmlPersistenceHelper(objectStorage));
		}
		protected internal void SetStorageCore(ISchedulerStorageBase storage) {
			this.storage = storage;
		}
	}
	#endregion
	public interface IResourceStorageBase : IPersistentObjectStorage<Resource> {
		ColorSavingType ColorSaving { get; set; }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new ResourceCollection Items { get; }
		new ResourceMappingInfo Mappings { get; }
		Resource GetResourceById(object resourceId);
		Resource CreateResource(object id);
		Resource CreateResource(object id, string caption);
		int Add(Resource resource);
		void AddRange(Resource[] resources);
		void Remove(Resource resource);
		void SetResourceFactory(IResourceFactory factory);
		bool Contains(Resource resource);
		void SaveToXml(string fileName);
		void SaveToXml(Stream stream);
		void LoadFromXml(string fileName);
		void LoadFromXml(Stream stream);
	}
	#region ResourceStorageBase
#if !SL
	[Editor("DevExpress.XtraScheduler.Design.ResourceStorageTypeEditor," + AssemblyInfo.SRAssemblySchedulerDesign, typeof(UITypeEditor))]
#endif
	public class ResourceStorageBase : PersistentObjectStorage<Resource>, IResourceStorageBase, IInternalResourceStorage {
		IResourceFactory resourceFactory;
		string resourceTreeFilter;
		ResourcesTreeColumnInfos resourcesTreeSortedColumns;
		NotificationCollectionChangedListener<ResourcesTreeColumnInfo> sortedColumnsListener;
		public ResourceStorageBase() {
			Initialize();
		}
		public ResourceStorageBase(ISchedulerStorageBase storage)
			: base(storage) {
			Initialize();
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("ResourceStorageBaseColorSaving"),
#endif
 DefaultValue(ColorSavingType.OleColor), Category(SRCategoryNames.Behavior), NotifyParentProperty(true), AutoFormatDisable()]
		public ColorSavingType ColorSaving { get { return InnerMappings.ColorSaving; } set { InnerMappings.ColorSaving = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Resource this[int index] { get { return (Resource)base[index]; } }
		[Browsable(false), Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ResourceCollection Items { get { return (ResourceCollection)base.Items; } }
		[NotifyParentProperty(true), Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ResourceMappingInfo Mappings { get { return InnerMappings; } }
		[Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		protected internal ResourceMappingInfo InnerMappings { get { return (ResourceMappingInfo)base.Mappings; } set { base.Mappings = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ResourceDataManager DataManager { get { return (ResourceDataManager)base.DataManager; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IResourceFactory ResourceFactory { get { return resourceFactory; } }
		ResourcesTreeColumnInfos IInternalResourceStorage.ResourcesTreeSortedColumns { get { return resourcesTreeSortedColumns; } }
		#region Filter
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("ResourceStorageBaseFilter"),
#endif
		XtraSerializableProperty(), DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		NotifyParentProperty(true), AutoFormatDisable()]
		public override string Filter { get { return base.Filter; } set { base.Filter = value; } }
		protected internal string ResourcesTreeFilter {
			get { return resourceTreeFilter; }
			set {
				string newValue = value ?? String.Empty;
				if (resourceTreeFilter == newValue)
					return;
				if (IsFilterValid(newValue)) {
					this.resourceTreeFilter = newValue;
					RaiseFilterChanged();
				} else
					Exceptions.ThrowArgumentException("Filter", newValue);
			}
		}
		string IInternalResourceStorage.ResourcesTreeFilter {
			get { return ResourcesTreeFilter; }
			set { ResourcesTreeFilter = value; }
		}
		NotificationCollectionChangedListener<ResourcesTreeColumnInfo> SortedColumnsListener { get { return sortedColumnsListener; } }
		#endregion
		#endregion
		#region Events
		#region ResourceVisibilityChanged
		PersistentObjectEventHandler onResourceVisibilityChanged;
		public event PersistentObjectEventHandler ResourceVisibilityChanged { add { onResourceVisibilityChanged += value; } remove { onResourceVisibilityChanged -= value; } }
		protected internal virtual void RaiseResourceVisibilityChanged(Resource resource) {
			if (onResourceVisibilityChanged != null) {
				PersistentObjectEventArgs args = new PersistentObjectEventArgs(resource);
				onResourceVisibilityChanged(this, args);
			}
		}
		#endregion
		#region ResourceParentIdChanged
		PersistentObjectEventHandler onResourceParentIdChanged;
		internal event PersistentObjectEventHandler ResourceParentIdChanged { add { onResourceParentIdChanged += value; } remove { onResourceParentIdChanged -= value; } }
		protected internal virtual void RaiseResourceParentIdChanged(Resource resource) {
			if (onResourceParentIdChanged != null) {
				PersistentObjectEventArgs args = new PersistentObjectEventArgs(resource);
				onResourceParentIdChanged(this, args);
			}
		}
		#endregion
		#region FilterChanged
		EventHandler onSortedColumnsChanged;
		public event EventHandler SortedColumnsChanged { add { onSortedColumnsChanged += value; } remove { onSortedColumnsChanged -= value; } }
		protected internal virtual void RaiseSortedColumnsChanged() {
			if (onSortedColumnsChanged != null)
				onSortedColumnsChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		void Initialize() {
			this.resourceFactory = CreateResourceFactory();
			this.resourcesTreeSortedColumns = new ResourcesTreeColumnInfos();
			this.sortedColumnsListener = new NotificationCollectionChangedListener<ResourcesTreeColumnInfo>(resourcesTreeSortedColumns);
			SubscribeSortedColumnsListenerEvents();
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (sortedColumnsListener != null) {
						UnsubscribeSortedColumnsListenerEvents();
						sortedColumnsListener.Dispose();
						sortedColumnsListener = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		private void SubscribeSortedColumnsListenerEvents() {
			SortedColumnsListener.Changed += new EventHandler(SortedColumnsListenerChanged);
		}
		private void UnsubscribeSortedColumnsListenerEvents() {
			SortedColumnsListener.Changed -= new EventHandler(SortedColumnsListenerChanged);
		}
		void SortedColumnsListenerChanged(object sender, EventArgs e) {
			RaiseSortedColumnsChanged();
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
		}
		protected virtual IResourceFactory CreateResourceFactory() {
			return new DefaultResourceFactory();
		}
		protected override NotificationCollection<Resource> CreateCollection() {
			return Storage != null ? new ResourceCollection(Storage) : new ResourceCollection();
		}
		protected internal override DataManager<Resource> CreateDataManager() {
			return new ResourceDataManager();
		}
		protected internal override MappingInfoBase<Resource> CreateMappingInfo() {
			return new ResourceMappingInfo();
		}
		protected internal override CustomFieldMappingCollectionBase<Resource> CreateCustomMappingsCollection() {
			return new ResourceCustomFieldMappingCollection();
		}
		protected internal override void SubscribeObjectEvents(Resource resource) {
			base.SubscribeObjectEvents(resource);
			SubscribeResourceVisibilityEvent(resource);
		}
		protected internal override void OnItemsChanged(object sender, CollectionChangedEventArgs<Resource> e) {
			base.OnItemsChanged(sender, e);
		}
		protected internal override void UnsubscribeObjectEvents(Resource resource) {
			base.UnsubscribeObjectEvents(resource);
			UnsubscribeResourceVisibilityEvent(resource);
		}
		protected internal virtual void SubscribeResourceVisibilityEvent(Resource resource) {
			((IInternalResource)resource).VisibleChanged += new EventHandler(OnResourceVisibleChanged);
		}
		protected internal virtual void UnsubscribeResourceVisibilityEvent(Resource resource) {
			((IInternalResource)resource).VisibleChanged -= new EventHandler(OnResourceVisibleChanged);
		}
		protected internal override ObjectsNonPersistentInformation CreateNonPersistentInformation() {
			ResourcesNonPersistentInformation result = new ResourcesNonPersistentInformation();
			ResourceIdCollection nonVisibleResourcesIdCollection = result.NonVisibleResourcesIdCollection;
			ResourceIdCollection collapsedResourcesIdCollection = result.CollapsedResourcesIdCollection;
			int count = this.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = Items[i];
				if (!resource.Visible)
					nonVisibleResourcesIdCollection.Add(resource.Id);
				if (!((IInternalResource)resource).IsExpanded)
					collapsedResourcesIdCollection.Add(resource.Id);
			}
			return result;
		}
		protected internal override void ApplyNonPersistentInformation(ObjectsNonPersistentInformation nonPersistentInfo) {
			ResourcesNonPersistentInformation info = (ResourcesNonPersistentInformation)nonPersistentInfo;
			ApplyNonPersistentInformation(info.NonVisibleResourcesIdCollection, ResourceBase.SetVisibleToFalse);
			ApplyNonPersistentInformation(info.CollapsedResourcesIdCollection, ResourceBase.SetIsExpandedToFalse);
		}
		void ApplyNonPersistentInformation(ResourceIdCollection idCollection, Action<Resource> restorePropertyDelegate) {
			int count = idCollection.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = Items.GetResourceById(idCollection[i]);
				if (resource != ResourceBase.Empty)
					restorePropertyDelegate(resource);
			}
		}
		protected internal virtual void OnResourceVisibleChanged(object sender, EventArgs e) {
			Resource resource = (Resource)sender;
			UnsubscribeResourceVisibilityEvent(resource);
			try {
				RaiseResourceVisibilityChanged(resource);
			} finally {
				SubscribeResourceVisibilityEvent(resource);
			}
		}
		protected internal override void UpdateKeyFieldName() {
			this.DataManager.KeyFieldName = InnerMappings.Id;
		}
		public override void ValidateDataSource() {
		}
		#region Resource collection methods
		public void AddRange(Resource[] resources) {
			Items.AddRange(resources);
		}
		public int Add(Resource resource) {
			return Items.Add(resource);
		}
		public void Remove(Resource resource) {
			Items.Remove(resource);
		}
		public bool Contains(Resource resource) {
			return Items.Contains(resource);
		}
		#endregion
		public Resource CreateResource(object id) {
			Resource resource = ResourceFactory.CreateResource();
			resource.SetId(id);
			CreateCustomFields(resource);
			return resource;
		}
		public Resource CreateResource(object id, string caption) {
			Resource resource = ResourceFactory.CreateResource();
			resource.SetId(id);
			CreateCustomFields(resource);
			resource.Caption = caption;
			return resource;
		}
		public void SetResourceFactory(IResourceFactory factory) {
			if (factory == null)
				factory = new DefaultResourceFactory();
			if (resourceFactory == factory)
				return;
			if (resourceFactory.GetType() == typeof(DefaultResourceFactory) &&
				factory.GetType() == typeof(DefaultResourceFactory))
				return;
			this.resourceFactory = factory;
			RaiseReload(true);
		}
		protected internal override bool IsLoadedObjectValid(Resource resource) {
			if (!base.IsLoadedObjectValid(resource))
				return false;
			return resource.Id != null;
		}
		protected internal override Resource CreateObject(object rowHandle) {
			return CreateResource(EmptyResourceId.Id);
		}
		public Resource GetResourceById(object resourceId) {
			return Items.GetResourceById(resourceId);
		}
		public virtual void AttachStorage(ISchedulerStorageBase storage) {
			SetStorageCore(storage);
			Items.SetStorageCore(storage);
		}
		protected override ISchedulerUnboundDataKeeper CreateUnboundDataKeeper() {
			return new UnboundDataKeeper();
		}
		protected internal override void Assign(PersistentObjectStorage<Resource> source) {
			base.Assign(source);
			ResourceStorageBase storage = source as ResourceStorageBase;
			if (storage != null) {
				BeginUpdateInternal();
				try {
					Filter = storage.Filter;
					ColorSaving = storage.ColorSaving;
					ResourcesTreeFilter = storage.ResourcesTreeFilter;
				} finally {
					BeginUpdateInternal();
				}
			}
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Data {
	#region ResourceDataManager
	public class ResourceDataManager : DataManager<Resource> {
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Xml {
	using System.Xml;
	public class ByteArrayContextAttribute : XmlContextItem {
		public ByteArrayContextAttribute(string name, byte[] val, byte[] defaultValue)
			: base(name, val, defaultValue) {
		}
		public override string ValueToString() {
			return ObjectConverter.ObjectToString(Value as byte[]);
		}
	}
	public class ResourceXmlPersistenceHelper : SchedulerXmlPersistenceHelper {
		Resource resource;
		IResourceStorageBase storage;
		public ResourceXmlPersistenceHelper(Resource resource, IResourceStorageBase storage) {
			this.resource = resource;
			this.storage = storage;
		}
		protected Resource Resource { get { return resource; } }
		public IResourceStorageBase Storage { get { return storage; } }
		protected override IXmlContext GetXmlContext() {
			XmlContext context = new XmlContext(ResourceSR.XmlElementName);
			context.Attributes.Add(new StringContextAttribute(ResourceSR.Caption, Resource.Caption, string.Empty));
			context.Attributes.Add(new StringContextAttribute(ResourceSR.Color, Resource.ColorValue, null));
			context.Attributes.Add(new ByteArrayContextAttribute(ResourceSR.Image, Resource.ImageBytes, null));
			context.Attributes.Add(new SchedulerObjectContextAttribute(ResourceSR.Id, Resource.Id, null));
			context.Attributes.Add(new SchedulerObjectContextAttribute(ResourceSR.ParentId, Resource.ParentId, null));
			if (Storage != null)
				AddCustomFieldsToContext(context, Resource.CustomFields, ((IInternalResourceStorage)Storage).ActualMappings);
			return context;
		}
		public override ObjectXmlLoader CreateObjectXmlLoader(XmlNode root) {
			return new ResourceXmlLoader(root, Storage);
		}
		public static Resource ObjectFromXml(IResourceStorageBase storage, string xml) {
			return ObjectFromXml(storage, GetRootElement(xml));
		}
		public static Resource ObjectFromXml(IResourceStorageBase storage, XmlNode root) {
			ResourceXmlPersistenceHelper helper = new ResourceXmlPersistenceHelper(null, storage);
			return (Resource)helper.FromXmlNode(root);
		}
	}
	public class ResourceXmlLoader : PersistentObjectXmlLoader {
		IResourceStorageBase storage;
		public ResourceXmlLoader(XmlNode root, IResourceStorageBase storage)
			: base(root) {
			Guard.ArgumentNotNull(storage, "storage");
			this.storage = storage;
		}
		public IResourceStorageBase Storage { get { return storage; } }
		public override object ObjectFromXml() {
			Resource r = CreateResourceInstance();
			r.BeginUpdate();
			try {
				r.SetId(ReadAttributeAsObject(ResourceSR.Id, typeof(object), EmptyResourceId.Id));
				r.Caption = ReadAttributeAsString(ResourceSR.Caption, String.Empty);
				r.ColorValue = ReadAttributeAsString(ResourceSR.Color, null);
				r.ImageBytes = (byte[])ReadAttributeAsObject(ResourceSR.Image, typeof(byte[]), null);
				r.ParentId = ReadAttributeAsObject(ResourceSR.ParentId, typeof(object), ResourceBase.Empty.ParentId);
				CustomFieldsFromXml(r, ((IInternalResourceStorage)Storage).ActualMappings);
			} finally {
				r.EndUpdate();
			}
			return r;
		}
		protected internal virtual Resource CreateResourceInstance() {
			return SchedulerUtils.CreateResourceInstance(Storage);
		}
	}
	public class ResourceContextElement : XmlContextItem {
		IResourceStorageBase storage;
		public ResourceContextElement(Resource resource, IResourceStorageBase storage)
			: base(ResourceSR.XmlElementName, resource, null) {
			this.storage = storage;
		}
		protected internal Resource Resource { get { return (Resource)Value; } }
		public IResourceStorageBase Storage { get { return storage; } }
		public override string ValueToString() {
			return new ResourceXmlPersistenceHelper(Resource, Storage).ToXml();
		}
	}
	public class ResourceCollectionContextElement : XmlContextItem {
		ResourceStorageBase storage;
		public ResourceCollectionContextElement(ResourceStorageBase storage)
			: base(ResourceSR.XmlCollectionName, storage.Items, null) {
			this.storage = storage;
		}
		protected internal ResourceCollection Resources { get { return (ResourceCollection)Value; } }
		protected internal ResourceStorageBase Storage { get { return storage; } }
		public override string ValueToString() {
			return new ResourceCollectionXmlPersistenceHelper(Storage).ToXml();
		}
	}
	public class ResourceCollectionXmlPersistenceHelper : CollectionXmlPersistenceHelper {
		IResourceStorageBase storage;
		public ResourceCollectionXmlPersistenceHelper(IResourceStorageBase storage)
			: base(storage.Items) {
			this.storage = storage;
		}
		protected override string XmlCollectionName { get { return ResourceSR.XmlCollectionName; } }
		protected ResourceCollection Resources { get { return Storage.Items; } }
		protected IResourceStorageBase Storage { get { return storage; } }
		public static ResourceCollection ObjectFromXml(ResourceStorageBase storage, string xml) {
			return ObjectFromXml(storage, GetRootElement(xml));
		}
		public static ResourceCollection ObjectFromXml(ResourceStorageBase storage, XmlNode root) {
			ResourceCollectionXmlPersistenceHelper helper = new ResourceCollectionXmlPersistenceHelper(storage);
			return (ResourceCollection)helper.FromXmlNode(root);
		}
		protected override ObjectCollectionXmlLoader CreateObjectCollectionXmlLoader(XmlNode root) {
			return new ResourceCollectionXmlLoader(root, Storage);
		}
		protected override IXmlContext CreateXmlContext() {
			IXmlContext context = base.CreateXmlContext();
			((XmlContext)context).XmlDocumentHeader = true;
			return context;
		}
		protected override IXmlContextItem CreateXmlContextItem(object obj) {
			return new ResourceContextElement((Resource)obj, Storage);
		}
	}
	public class ResourceCollectionXmlLoader : ObjectCollectionXmlLoader {
		IResourceStorageBase storage;
		public ResourceCollectionXmlLoader(XmlNode root, IResourceStorageBase storage)
			: base(root) {
			Guard.ArgumentNotNull(storage, "storage");
			this.storage = storage;
		}
		protected override ICollection Collection { get { return Resources; } }
		protected IResourceStorageBase Storage { get { return storage; } }
		protected ResourceBaseCollection Resources { get { return storage.Items; } }
		protected override string XmlCollectionName { get { return ResourceSR.XmlCollectionName; } }
		protected override object LoadObject(XmlNode root) {
			return ResourceXmlPersistenceHelper.ObjectFromXml(Storage, root);
		}
		protected override void AddObjectToCollection(object obj) {
			Resources.Add((Resource)obj);
		}
		protected override void ClearCollectionObjects() {
			Resources.Clear();
		}
	}
	public class AppointmentResourceIdContextElement : XmlContextItem {
		public AppointmentResourceIdContextElement(object resourceId)
			: base(AppointmentSR.ResourceId, resourceId, null) {
		}
		public override string ValueToString() {
			return new AppointmentResourceIdXmlPersistenceHelper(Value).ToXml();
		}
	}
	public class AppointmentResourceIdXmlLoader : ObjectXmlLoader {
		public AppointmentResourceIdXmlLoader(XmlNode root)
			: base(root) {
		}
		public override object ObjectFromXml() {
			string typeName = ReadAttributeAsString(AppointmentSR.ResourceIdType, String.Empty);
			if (String.IsNullOrEmpty(typeName))
				return ReadAttributeAsObject(AppointmentSR.ResourceIdValue, typeof(object), null);
			if (typeName == "DevExpress.XtraScheduler.Native.EmptyResourceId")
				return EmptyResourceId.Id;
			Type type = Type.GetType(typeName);
			string resourceIdString = ReadAttributeAsString(AppointmentSR.ResourceIdValue, String.Empty);
			return ObjectConverter.StringToObject(resourceIdString, type);
		}
	}
	public class AppointmentResourceIdXmlPersistenceHelper : XmlPersistenceHelper {
		object resourceId;
		public AppointmentResourceIdXmlPersistenceHelper(object resourceId) {
			this.resourceId = resourceId;
		}
		protected override IXmlContext GetXmlContext() {
			Type resourceType = resourceId.GetType();
			XmlContext context = new XmlContext(AppointmentSR.ResourceId);
			if (!SchedulerCompatibility.Base64XmlObjectSerialization) {
				context.Attributes.Add(new StringContextAttribute(AppointmentSR.ResourceIdType, resourceType.FullName, String.Empty));
				context.Attributes.Add(new ObjectContextAttribute(AppointmentSR.ResourceIdValue, resourceId, null));
			} else
				context.Attributes.Add(new SchedulerObjectContextAttribute(AppointmentSR.ResourceIdValue, resourceId, null));
			return context;
		}
		public override ObjectXmlLoader CreateObjectXmlLoader(XmlNode root) {
			return new AppointmentResourceIdXmlLoader(root);
		}
		public static object ObjectFromXml(string xml) {
			return ObjectFromXml(GetRootElement(xml));
		}
		public static object ObjectFromXml(XmlNode root) {
			AppointmentResourceIdXmlPersistenceHelper helper = new AppointmentResourceIdXmlPersistenceHelper(null);
			return helper.FromXmlNode(root);
		}
	}
	public class AppointmentResourceIdCollectionContextElement : XmlContextItem {
		public AppointmentResourceIdCollectionContextElement(ResourceIdCollection resourceIds)
			: base(AppointmentSR.XmlCollectionResourceIdsName, resourceIds, null) {
		}
		protected ResourceIdCollection ResourceIds { get { return (ResourceIdCollection)Value; } }
		public override string ValueToString() {
			return new AppointmentResourceIdCollectionXmlPersistenceHelper(ResourceIds).ToXml();
		}
	}
	public class AppointmentResourceIdCollectionXmlPersistenceHelper : CollectionXmlPersistenceHelper {
		ResourceIdCollection resourceIds;
		public AppointmentResourceIdCollectionXmlPersistenceHelper(ResourceIdCollection resourceIds)
			: base(resourceIds) {
			this.resourceIds = resourceIds;
		}
		protected override string XmlCollectionName { get { return AppointmentSR.XmlCollectionResourceIdsName; } }
		public static ResourceIdCollection ObjectFromXml(ResourceIdCollection resourceIds, string xml) {
			return ObjectFromXml(resourceIds, GetRootElement(xml));
		}
		public static ResourceIdCollection ObjectFromXml(ResourceIdCollection resourceIds, XmlNode root) {
			AppointmentResourceIdCollectionXmlPersistenceHelper helper = new AppointmentResourceIdCollectionXmlPersistenceHelper(resourceIds);
			return (ResourceIdCollection)helper.FromXmlNode(root);
		}
		protected override ObjectCollectionXmlLoader CreateObjectCollectionXmlLoader(XmlNode root) {
			return new AppointmentResourceIdCollectionXmlLoader(root, resourceIds);
		}
		protected override IXmlContextItem CreateXmlContextItem(object obj) {
			return new AppointmentResourceIdContextElement(obj);
		}
	}
	public class AppointmentResourceIdCollectionXmlLoader : ObjectCollectionXmlLoader {
		ResourceIdCollection resourceIds;
		public AppointmentResourceIdCollectionXmlLoader(XmlNode root, ResourceIdCollection resourceIds)
			: base(root) {
			Guard.ArgumentNotNull(resourceIds, "resourceIds");
			this.resourceIds = resourceIds;
		}
		protected override ICollection Collection { get { return resourceIds; } }
		protected override string XmlCollectionName { get { return AppointmentSR.XmlCollectionResourceIdsName; } }
		protected override object LoadObject(XmlNode root) {
			return AppointmentResourceIdXmlPersistenceHelper.ObjectFromXml(root);
		}
		protected override void AddObjectToCollection(object obj) {
			resourceIds.Add(obj);
		}
		protected override void ClearCollectionObjects() {
			resourceIds.Clear();
		}
	}
}
namespace DevExpress.XtraScheduler.Internal {
	public interface IInternalResourceStorage : IInternalPersistentObjectStorage<Resource> {
		ResourcesTreeColumnInfos ResourcesTreeSortedColumns { get; }
		string ResourcesTreeFilter { get; set; }
		IResourceFactory ResourceFactory { get; }
		void AttachStorage(ISchedulerStorageBase storage);
		event PersistentObjectEventHandler ResourceVisibilityChanged;
		event EventHandler SortedColumnsChanged;
	}
	public static class ResourceSR {
		public const string XmlCollectionName = "Items";
		public const string XmlElementName = "Resource";
		public const string Caption = "Caption";
		public const string Color = "Color";
		public const string Id = "Id";
		public const string Image = "Image";
		public const string ParentId = "ParentId";
	}
	#region ResourcesNonPersistentInformation
	public class ResourcesNonPersistentInformation : ObjectsNonPersistentInformation {
		ResourceIdCollection nonVisibleResourcesIdCollection = new ResourceIdCollection();
		ResourceIdCollection collapsedResourcesIdCollection = new ResourceIdCollection();
		public ResourceIdCollection NonVisibleResourcesIdCollection { get { return nonVisibleResourcesIdCollection; } }
		public ResourceIdCollection CollapsedResourcesIdCollection { get { return collapsedResourcesIdCollection; } }
	}
	#endregion
	#region DefaultResourceFactory
	public class DefaultResourceFactory : IResourceFactory {
		public virtual Resource CreateResource() {
			return new ResourceBase();
		}
	}
	#endregion
	public static class WinColorToStringSerializer {
		public static string ColorToString(Color color) {
			if (color.IsEmpty)
				return "Empty";
			return color.IsKnownColor ? color.Name : String.Format("0x{0:X8}", color.ToArgb());
		}
		public static Color StringToColor(string colorName) {
			if (string.IsNullOrEmpty(colorName))
				return Color.Empty;
			if (colorName == "Empty")
				return Color.Empty;
			if (colorName.StartsWith("0x")) {
				string colorHexValue = colorName.Substring(2, 8);
				int argbColor = int.Parse(colorHexValue, NumberStyles.HexNumber);
				return Color.FromArgb(argbColor);
			}
			Color color = Color.FromName(colorName);
			return color.IsKnownColor ? color : Color.Empty;
		}
	}
}
namespace DevExpress.XtraScheduler.Internal {
	public interface IInternalResource : IInternalPersistentObject {
		ResourceBaseCollection ChildResources { get; set; }
		int AllChildResourcesCount { get; set; }
		bool IsExpanded { get; set; }
		void SetParentId(object val);
		void RaiseVisibleChanged();
		event EventHandler VisibleChanged;
	}
}
namespace DevExpress.XtraScheduler.Internal.Implementations {
	public class ResourceBase : PersistentObject, Resource, IInternalResource, INotifyPropertyChanged {
		#region Fields
		static ResourceBase empty = CreateEmptyResource();
		bool isExpanded = true;
		string caption = String.Empty;
		object parentId;
		bool visible = true;
		ResourceBaseCollection childResources = new ResourceBaseCollection();
		int allChildResourcesCount = 0;
		Image image;
		#endregion
		public ResourceBase() {
		}
		protected internal ResourceBase(object id, string caption) {
			SetId(id);
			this.caption = caption;
		}
		protected internal ResourceBase(object id, string caption, object parentId)
			: this(id, caption) {
			this.parentId = parentId;
		}
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			ImageBytes = null;
			if (this.image != null) {
				this.image.Dispose();
				this.image = null;
			}
			base.Dispose(disposing);
		}
		#endregion
		#region Properties
		public static ResourceBase Empty { get { return empty; } }
		public virtual string Caption {
			get { return caption; }
			set {
				if (caption == value)
					return;
				string oldCaption = caption;
				caption = value;
				if (OnContentChanging("Caption", oldCaption, value)) {
					OnContentChanged();
					NotifyPropertyChanged("Caption");
				} else
					caption = oldCaption;
			}
		}
		public string ColorValue { get; set; }
		public virtual Color Color {
			get { return WinColorToStringSerializer.StringToColor(ColorValue); }
			set {
				string newColorValue = WinColorToStringSerializer.ColorToString(value);
				if (newColorValue == ColorValue)
					return;
				Color oldColor = Color;
				if (OnContentChanging("Color", oldColor, value)) {
					ColorValue = newColorValue;
					OnContentChanged();
					NotifyPropertyChanged("Color");
				}
			}
		}
		public byte[] ImageBytes { get; set; }
		public virtual Image Image {
			get {
				if (image == null)
					image = SchedulerImageHelper.CreateImageFromBytes(ImageBytes);
				return image;
			}
			set {
				if (Object.Equals(image, value))
					return;
				if (OnContentChanging("Image", this.image, value)) {
					AssignImage(value);
					OnContentChanged();
				}
			}
		}
		public override object Id {
			get { return base.Id; }
		}
		public object ParentId {
			get { return parentId; }
			set {
				if (IsParentIdValueEquals(value))
					return;
				object oldParentId = parentId;
				parentId = value;
				if (OnContentChanging("ParentId", oldParentId, value)) {
					OnContentChanged();
					NotifyPropertyChanged("ParentId");
				} else
					parentId = oldParentId;
			}
		}
		bool IsParentIdValueEquals(object newValue) {
			if (parentId == null)
				return newValue == null;
			else
				return parentId.Equals(newValue);
		}
		#region Visible
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("ResourceVisible")]
#endif
		public bool Visible {
			get { return visible; }
			set {
				if (visible == value)
					return;
				visible = value;
				OnVisibleChanged();
			}
		}
		internal static void SetVisibleToFalse(Resource resource) {
			((ResourceBase)resource).visible = false;
		}
		#endregion
		#region ChildResources
		public ResourceBaseCollection ChildResources { get { return childResources; } set { childResources = value; } }
		public int AllChildResourcesCount { get { return allChildResourcesCount; } set { allChildResourcesCount = value; } }
		#endregion
		#region IsExpanded
		public bool IsExpanded { get { return isExpanded; } set { isExpanded = value; } }
		internal static void SetIsExpandedToFalse(Resource resource) {
			((IInternalResource)resource).IsExpanded = false;
		}
		#endregion
		#endregion
		#region Events
		#region VisibleChanged
		EventHandler onVisibleChanged;
		public event EventHandler VisibleChanged { add { onVisibleChanged += value; } remove { onVisibleChanged -= value; } }
		public virtual void RaiseVisibleChanged() {
			if (onVisibleChanged != null)
				onVisibleChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal virtual void OnVisibleChanged() {
			if (!IsUpdateLocked)
				RaiseVisibleChanged();
		}
		static ResourceBase CreateEmptyResource() {
			ResourceBase emptyResource = new EmptyResource();
			((IIdProvider)emptyResource).SetId(EmptyResourceId.Id);
			return emptyResource;
		}
		public void SetParentId(object val) {
			this.parentId = val;
		}
		internal static bool IsEmptyResourceId(object id) {
			return id is EmptyResourceId;
		}
		internal static bool InternalMatchIds(object id1, object id2) {
			return IsEmptyResourceId(id1) || IsEmptyResourceId(id2) || Object.Equals(id1, id2);
		}
		internal static bool InternalMatchIdToResourceIdCollection(ResourceIdCollection resourceIds, object id) {
			int count = resourceIds.Count;
			for (int i = 0; i < count; i++)
				if (InternalMatchIds(resourceIds[i], id))
					return true;
			return false;
		}
		internal static bool InternalAreResourceIdsCollectionsIntersect(ResourceIdCollection resourceIds1, ResourceIdCollection resourceIds2) {
			int count = resourceIds1.Count;
			for (int i = 0; i < count; i++)
				if (InternalMatchIdToResourceIdCollection(resourceIds2, resourceIds1[i]))
					return true;
			return false;
		}
		internal static bool InternalAreResourceIdsCollectionsIntersect(ResourceIdCollection resourceIds, ResourceBaseCollection resources) {
			int count = resources.Count;
			for (int i = 0; i < count; i++)
				if (InternalMatchIdToResourceIdCollection(resourceIds, resources[i].Id))
					return true;
			return false;
		}
		internal static bool InternalAreResourceIdsCollectionsSame(ResourceIdCollection resourceIds1, ResourceIdCollection resourceIds2) {
			int count = resourceIds1.Count;
			if (resourceIds2.Count != count)
				return false;
			for (int i = 0; i < count; i++)
				if (!resourceIds2.Contains(resourceIds1[i]))
					return false;
			return true;
		}
		internal static bool InternalAreResourceCollectionsSame(ResourceBaseCollection resources1, ResourceBaseCollection resources2) {
			ResourceIdCollection resourceIds1 = InternalToResourceIdCollection(resources1);
			ResourceIdCollection resourceIds2 = InternalToResourceIdCollection(resources2);
			return InternalAreResourceIdsCollectionsSame(resourceIds1, resourceIds2);
		}
		internal static ResourceIdCollection InternalToResourceIdCollection(ResourceBaseCollection resources) {
			ResourceIdCollection result = new ResourceIdCollection();
			int count = resources.Count;
			for (int i = 0; i < count; i++) {
				object id = resources[i].Id;
				if (id != null)
					result.Add(id);
			}
			return result;
		}
		public static bool MatchIds(Resource res1, Resource res2) {
			if (res1 == null || res2 == null)
				return false;
			return InternalMatchIds(res1.Id, res2.Id);
		}
		internal void ReplaceImage(Image newImage) {
			if (image != null)
				image.Dispose();
			if (newImage != null) {
				image = newImage;
				ImageBytes = SchedulerImageHelper.GetImageBytes(image);
			} else {
				image = null;
				ImageBytes = null;
			}
		}
		protected internal override XmlPersistenceHelper CreateXmlPersistenceHelper() {
			return new ResourceXmlPersistenceHelper(this, null);
		}
		public override object GetRow(ISchedulerStorageBase storage) {
			return (storage != null) ? storage.Resources.GetObjectRow(this) : null;
		}
		public override object GetValue(ISchedulerStorageBase storage, string columnName) {
			return (storage != null) ? storage.Resources.GetObjectValue(this, columnName) : null;
		}
		public override void SetValue(ISchedulerStorageBase storage, string columnName, object val) {
			if (storage != null)
				storage.Resources.SetObjectValue(this, columnName, val);
		}
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected void NotifyPropertyChanged(String info) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(info));
		}
		#endregion
		bool IInternalPersistentObject.RaiseStateChanging(PersistentObjectState state, string propertyName, object oldValue, object newValue) {
			return RaiseStateChanging(this, state, propertyName, oldValue, newValue);
		}
		void IInternalPersistentObject.RaiseStateChanged(IPersistentObject obj, PersistentObjectState state) {
			RaiseStateChanged(obj, state);
		}
		void AssignImage(Image val) {
			if (image != null)
				image.Dispose();
			if (val != null) {
				image = (Image)val.Clone();
				ImageBytes = SchedulerImageHelper.GetImageBytes(image);
			} else {
				image = null;
				ImageBytes = null;
			}
		}
	}
	internal class EmptyResource : ResourceBase {
		public override string Caption {
			get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_EmptyResource); }
			set { }
		}
	}
}
