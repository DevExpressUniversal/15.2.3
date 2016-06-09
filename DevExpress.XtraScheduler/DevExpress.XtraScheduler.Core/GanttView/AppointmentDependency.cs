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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Utils;
using System.IO;
using DevExpress.XtraScheduler.Xml;
using DevExpress.XtraScheduler.Data;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler {
	#region DependencyType
	public enum AppointmentDependencyType {
		FinishToStart,
		StartToStart,
		FinishToFinish,
		StartToFinish
	}
	#endregion
	public interface AppointmentDependency : IPersistentObject {
		AppointmentDependencyType Type { get; set; }
		object ParentId { get; }
		object DependentId { get; }
	}	
}
namespace DevExpress.XtraScheduler.Xml {
	using System.Xml;
	using DevExpress.XtraScheduler.Native;
	using System.Collections;
	using DevExpress.XtraScheduler.Internal;
	using Internal.Implementations;
	#region AppointmentDependencyContextElement
	public class AppointmentDependencyContextElement : XmlContextItem {
		IAppointmentDependencyStorage storage;
		public AppointmentDependencyContextElement(AppointmentDependency dependency, IAppointmentDependencyStorage storage)
			: base(AppointmentDependencySR.XmlElementName, dependency, null) {
			this.storage = storage;
		}
		protected AppointmentDependency AppointmentDependency { get { return (AppointmentDependency)Value; } }
		protected IAppointmentDependencyStorage Storage { get { return storage; } }
		public override string ValueToString() {
			return new AppointmentDependencyXmlPersistenceHelper(AppointmentDependency, Storage).ToXml();
		}
	}
	#endregion
	#region AppointmentDependencyXmlPersistenceHelper
	public class AppointmentDependencyXmlPersistenceHelper : SchedulerXmlPersistenceHelper {
		IAppointmentDependencyStorage storage;
		AppointmentDependency dependency;
		public AppointmentDependencyXmlPersistenceHelper(AppointmentDependency dependency, IAppointmentDependencyStorage storage) {
			this.dependency = dependency;
			this.storage = storage;
		}
		protected AppointmentDependency Dependency { get { return dependency; } }
		public IAppointmentDependencyStorage Storage { get { return storage; } }
		protected override IXmlContext GetXmlContext() {
			XmlContext context = new XmlContext(AppointmentDependencySR.XmlElementName);
			context.Attributes.Add(new SchedulerObjectContextAttribute(AppointmentDependencySR.ParentId, Dependency.ParentId, null));
			context.Attributes.Add(new SchedulerObjectContextAttribute(AppointmentDependencySR.DependentId, Dependency.DependentId, null));
			context.Attributes.Add(new IntegerContextAttribute(AppointmentDependencySR.Type, (int)Dependency.Type, (int)AppointmentDependencyType.FinishToStart));
			if (Storage != null)
				AddCustomFieldsToContext(context, Dependency.CustomFields, ((IInternalPersistentObjectStorage<AppointmentDependency>)Storage).ActualMappings);
			return context;
		}
		public override ObjectXmlLoader CreateObjectXmlLoader(XmlNode root) {
			return new AppointmentDependencyXmlLoader(root, Storage);
		}
		public static AppointmentDependency ObjectFromXml(IAppointmentDependencyStorage storage, string xml) {
			return ObjectFromXml(storage, GetRootElement(xml));
		}
		public static AppointmentDependency ObjectFromXml(IAppointmentDependencyStorage storage, XmlNode root) {
			AppointmentDependencyXmlPersistenceHelper helper = new AppointmentDependencyXmlPersistenceHelper(null, storage);
			return (AppointmentDependency)helper.FromXmlNode(root);
		}
	}
	#endregion
	#region AppointmentDependencyXmlLoader
	public class AppointmentDependencyXmlLoader : PersistentObjectXmlLoader {
		IAppointmentDependencyStorage storage;
		public AppointmentDependencyXmlLoader(XmlNode root, IAppointmentDependencyStorage storage)
			: base(root) {
			Guard.ArgumentNotNull(storage, "storage");
			this.storage = storage;
		}
		public IAppointmentDependencyStorage Storage { get { return storage; } }
		public override object ObjectFromXml() {
			AppointmentDependency dep = CreateDependencyInstance();
			dep.BeginUpdate();
			try {
				IInternalAppointmentDependency internalDep = (IInternalAppointmentDependency)dep;
				internalDep.SetParentId(ReadAttributeAsObject(AppointmentDependencySR.ParentId, typeof(object), AppointmentDependencyInstance.Empty.ParentId));
				internalDep.SetDependentId(ReadAttributeAsObject(AppointmentDependencySR.DependentId, typeof(object), AppointmentDependencyInstance.Empty.DependentId));
				dep.Type = (AppointmentDependencyType)ReadAttributeAsInt(AppointmentDependencySR.Type, (int)AppointmentDependencyType.StartToFinish);
				CustomFieldsFromXml(dep, ((IInternalPersistentObjectStorage<AppointmentDependency>)Storage).ActualMappings);
			} finally {
				dep.EndUpdate();
			}
			return dep;
		}
		protected internal virtual AppointmentDependency CreateDependencyInstance() {
			return SchedulerUtils.CreateAppointmentDependencyInstance(Storage);
		}
	}
	#endregion
	#region AppointmentDependencyCollectionXmlPersistenceHelper
	public class AppointmentDependencyCollectionXmlPersistenceHelper : CollectionXmlPersistenceHelper {
		IAppointmentDependencyStorage storage;
		public AppointmentDependencyCollectionXmlPersistenceHelper(IAppointmentDependencyStorage storage)
			: base(storage.Items) {
			this.storage = storage;
		}
		protected override string XmlCollectionName { get { return AppointmentSR.XmlCollectionName; } }
		protected IAppointmentDependencyStorage Storage { get { return storage; } }
		protected AppointmentDependencyCollection Dependencies { get { return Storage.Items; } }
		public static AppointmentDependencyCollection ObjectFromXml(IAppointmentDependencyStorage storage, string xml) {
			return ObjectFromXml(storage, GetRootElement(xml));
		}
		public static AppointmentDependencyCollection ObjectFromXml(IAppointmentDependencyStorage storage, XmlNode root) {
			AppointmentDependencyCollectionXmlPersistenceHelper helper = new AppointmentDependencyCollectionXmlPersistenceHelper(storage);
			return (AppointmentDependencyCollection)helper.FromXmlNode(root);
		}
		protected override ObjectCollectionXmlLoader CreateObjectCollectionXmlLoader(XmlNode root) {
			return null;
		}
		protected override IXmlContext CreateXmlContext() {
			IXmlContext context = base.CreateXmlContext();
			((XmlContext)context).XmlDocumentHeader = true;
			return context;
		}
		protected override IXmlContextItem CreateXmlContextItem(object obj) {
			return new AppointmentDependencyContextElement((AppointmentDependency)obj, Storage);
		}
		public override object ParseXmlDocument(XmlNode root) {
			IAppointmentDependencyLoaderProvider provider = new AppointmentDependencyLoaderXmlProvider(XmlDocumentHelper.GetChildren(root));
			AppointmentDependencyCollectionLoader loader = new AppointmentDependencyCollectionLoader(provider, (IAppointmentDependencyStorage)Storage);
			loader.LoadDependencies();
			return Dependencies;
		}
	}
	#endregion
	#region AppointmentDependencyLoaderXmlProvider
	public class AppointmentDependencyLoaderXmlProvider : IAppointmentDependencyLoaderProvider {
		DXXmlNodeCollection items;
		public AppointmentDependencyLoaderXmlProvider(DXXmlNodeCollection items) {
			this.items = items;
		}
		protected DXXmlNodeCollection Items { get { return items; } }
		#region IAppointmentDependencyLoaderProvider
		public int TotalObjectCount { get { return Items.Count; } }
		public AppointmentDependency LoadDependency(IAppointmentDependencyStorage storage, int objectIndex) {
			XmlNode item = Items[objectIndex];
			if (item == null)
				return null;
			AppointmentDependency dep = AppointmentDependencyXmlPersistenceHelper.ObjectFromXml(storage, item);
			if (dep != null) dep.RowHandle = objectIndex;
			return dep;
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler {
	#region IAppointmentDependencyFactory (interface)
	public interface IAppointmentDependencyFactory {
		AppointmentDependency CreateAppointmentDependency(object parentId, object dependentId);
		AppointmentDependency CreateAppointmentDependency(object parentId, object dependentId, AppointmentDependencyType type);
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region AppointmentDependencySR
	public static class AppointmentDependencySR {
		public const string XmlCollectionName = "Items";
		public const string XmlElementName = "AppointmentDependency";
		public const string Type = "Type";
		public const string ParentId = "ParentId";
		public const string DependentId = "DependentId";
	}
	#endregion
	#region DefaultAppointmentDependencyFactory
	public class DefaultAppointmentDependencyFactory : IAppointmentDependencyFactory {
		public AppointmentDependency CreateAppointmentDependency(object parentId, object dependentId) {
			return new AppointmentDependencyInstance(parentId, dependentId);
		}
		public AppointmentDependency CreateAppointmentDependency(object parentId, object dependentId, AppointmentDependencyType type) {
			return new AppointmentDependencyInstance(parentId, dependentId, type);
		}
	}
	#endregion
	[Obsolete("Please, use DefaultAppointmentDependencyFactory instead.", false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	#region DefaultDependencyFactory
	public class DefaultDependencyFactory : IDependencyFactory {
		public AppointmentDependency CreateDependency(object parentId, object dependentId) {
			return new AppointmentDependencyInstance(parentId, dependentId);
		}
	}
	#endregion
	#region IDependencyFactory (interface)
	[Obsolete("Please, use IAppointmentDependencyFactory interface instead.", false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IDependencyFactory {
		AppointmentDependency CreateDependency(object parentId, object dependentId);
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Data {
	using DevExpress.XtraScheduler.Native;
	#region DependencyDataManager
	public class AppointmentDependencyDataManager : DataManager<AppointmentDependency> {
	}
	#endregion
	public interface IAppointmentDependencyLoaderProvider {
		AppointmentDependency LoadDependency(IAppointmentDependencyStorage storage, int objectIndex);
		int TotalObjectCount { get; }
	}
	#region AppointmentDependencyCollectionLoader
	public class AppointmentDependencyCollectionLoader {
		IAppointmentDependencyLoaderProvider provider;
		IAppointmentDependencyStorage storage;
		public AppointmentDependencyCollectionLoader(IAppointmentDependencyLoaderProvider provider, IAppointmentDependencyStorage storage) {
			if (provider == null)
				Exceptions.ThrowArgumentException("provider", provider);
			if (storage == null)
				Exceptions.ThrowArgumentException("storage", storage);
			this.provider = provider;
			this.storage = storage;
		}
		protected IAppointmentDependencyLoaderProvider Provider { get { return provider; } }
		protected IAppointmentDependencyStorage Storage { get { return storage; } }
		public void LoadDependencies() {
			AppointmentDependencyCollection target = Storage.Items;
			target.BeginUpdate();
			try {
				LoadAppointmentDependenciesCore(target);
			} finally {
				target.CancelUpdate();
			}
		}
		void LoadAppointmentDependenciesCore(AppointmentDependencyCollection target) {
			int count = Provider.TotalObjectCount;
			for (int i = 0; i < count; i++) {
				AppointmentDependency dep = Provider.LoadDependency(Storage, i);
				AddLoadedAppointmentDependencyToCollection(dep, target);
			}
		}
		internal void AddLoadedAppointmentDependencyToCollection(AppointmentDependency dep, AppointmentDependencyCollection target) {
			if (IsLoadedDependencyValid(dep))
				target.Add(dep);
			else {
				if (dep != null)
					dep.Dispose();
			}
		}
		internal bool IsLoadedDependencyValid(AppointmentDependency dep) {
			return dep.ParentId != null && dep.DependentId != null;
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Internal.Implementations {
	#region AppointmentDependency
	public class AppointmentDependencyInstance : PersistentObject, AppointmentDependency, IInternalAppointmentDependency, INotifyPropertyChanged {
		#region Fields
		static readonly AppointmentDependencyInstance empty = CreateEmptyAppointmentDependency();
		AppointmentDependencyType type;
		object parentId;
		object dependentId;
		#endregion
		public AppointmentDependencyInstance() {
		}
		protected internal AppointmentDependencyInstance(object parentId, object dependentId) {
			Guard.ArgumentNotNull(parentId, "parentId");
			Guard.ArgumentNotNull(dependentId, "dependentId");
			this.parentId = parentId;
			this.dependentId = dependentId;
		}
		protected internal AppointmentDependencyInstance(object parentId, object dependentId, AppointmentDependencyType type)
			: this(parentId, dependentId) {
			this.type = type;
		}
		#region Empty
		public static AppointmentDependencyInstance Empty { get { return empty; } }
		#endregion
		#region Type
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDependencyType")]
#endif
		public AppointmentDependencyType Type {
			get { return type; }
			set {
				if (Type == value)
					return;
				AppointmentDependencyType old = Type;
				type = value;
				if (OnContentChanging("Type", old, value)) {
					OnContentChanged();
					NotifyPropertyChanged("Type");
				} else
					type = old;
			}
		}
		#endregion
		#region ParentId
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDependencyParentId")]
#endif
		public object ParentId { get { return parentId; } }
		#endregion
		#region DependentId
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDependencyDependentId")]
#endif
		public object DependentId { get { return dependentId; } }
		#endregion
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		void NotifyPropertyChanged(String info) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(info));
		}
		#endregion
		static AppointmentDependencyInstance CreateEmptyAppointmentDependency() {
			int fakeIds = Int32.MinValue;
			AppointmentDependencyInstance empty = new AppointmentDependencyInstance(fakeIds, fakeIds);
			empty.SetDependentId(empty);
			empty.SetParentId(empty);
			return empty;
		}
		public override object GetRow(ISchedulerStorageBase storage) {
			return (storage != null) ? ((IAppointmentDependencyStorage)storage.AppointmentDependencies).GetObjectRow(this) : null;
		}
		public override object GetValue(ISchedulerStorageBase storage, string columnName) {
			return (storage != null) ? ((IAppointmentDependencyStorage)storage.AppointmentDependencies).GetObjectValue(this, columnName) : null;
		}
		public override void SetValue(ISchedulerStorageBase storage, string columnName, object val) {
			if (storage != null)
				((IAppointmentDependencyStorage)storage.AppointmentDependencies).SetObjectValue(this, columnName, val);
		}
		public void SetParentId(object id) {
			parentId = id;
		}
		public void SetDependentId(object id) {
			dependentId = id;
		}
		protected internal override XmlPersistenceHelper CreateXmlPersistenceHelper() {
			return new AppointmentDependencyXmlPersistenceHelper(this, null);
		}
		internal static bool IsEmptyParentId(object id) {
			return id == AppointmentDependencyInstance.Empty.ParentId;
		}
		internal static bool IsEmptyDependentId(object id) {
			return id == AppointmentDependencyInstance.Empty.DependentId;
		}
		internal static bool IsNullOrEmptyParentId(AppointmentDependency value) {
			return value.ParentId == null || Object.Equals(value.ParentId, AppointmentDependencyInstance.Empty.ParentId);
		}
		internal static bool IsNullOrEmptyDependentId(AppointmentDependency value) {
			return value.DependentId == null || Object.Equals(value.DependentId, AppointmentDependencyInstance.Empty.DependentId);
		}
		internal static bool IsNullOrEmptyAppointmentId(AppointmentDependency value) {
			return AppointmentDependencyInstance.IsNullOrEmptyParentId(value) || AppointmentDependencyInstance.IsNullOrEmptyDependentId(value);
		}
		internal static bool IsNullOrEmpty(AppointmentDependency value) {
			return value == null || Object.Equals(value, AppointmentDependencyInstance.Empty);
		}
		bool IInternalPersistentObject.RaiseStateChanging(PersistentObjectState state, string propertyName, object oldValue, object newValue) {
			return RaiseStateChanging(this, state, propertyName, oldValue, newValue);
		}
		void IInternalPersistentObject.RaiseStateChanged(IPersistentObject obj, PersistentObjectState state) {
			RaiseStateChanged(obj, state);
		}
	}
	#endregion
}
