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
using DevExpress.XtraScheduler.Internal.Implementations;
#if !SL
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using DevExpress.XtraScheduler.Internal;
#else
using System.Windows.Media;
using DevExpress.XtraScheduler;
#endif
namespace DevExpress.XtraScheduler.Native {
#if !SL
		[Editor("DevExpress.XtraScheduler.Design.AppointmentDependencyStorageTypeEditor," + AssemblyInfo.SRAssemblySchedulerDesign, typeof(UITypeEditor))]
#endif
	public class AppointmentDependencyStorageBase : PersistentObjectStorage<AppointmentDependency>, IAppointmentDependencyStorage, IInternalAppointmentDependencyStorage, IAppointmentDependencyLoaderProvider {
		#region Fields
		IAppointmentDependencyFactory dependencyFactory;
		#endregion
		public AppointmentDependencyStorageBase() {
			Initialize();
		}
		public AppointmentDependencyStorageBase(ISchedulerStorageBase storage)
			: base(storage) {
			Initialize();
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new AppointmentDependency this[int index] { get { return (AppointmentDependency)base[index]; } }
		[Browsable(false), Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new AppointmentDependencyCollection Items { get { return (AppointmentDependencyCollection)base.Items; } }
		[Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		protected internal AppointmentDependencyMappingInfo InnerMappings { get { return (AppointmentDependencyMappingInfo)base.Mappings; } set { base.Mappings = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new AppointmentDependencyDataManager DataManager { get { return (AppointmentDependencyDataManager)base.DataManager; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IAppointmentDependencyFactory DependencyFactory { get { return dependencyFactory; } }
		public new AppointmentDependencyMappingInfo Mappings { get { return InnerMappings; } }
		public new AppointmentDependencyCustomFieldMappingCollection CustomFieldMappings { get { return (AppointmentDependencyCustomFieldMappingCollection)base.CustomFieldMappings; } }
		#endregion
		#region Events
		#region UIChanged
		EventHandler onUIChanged;
		public event EventHandler UIChanged { add { onUIChanged += value; } remove { onUIChanged -= value; } }
		protected internal virtual void RaiseUIChanged() {
			if (onUIChanged != null)
				onUIChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		void Initialize() {
			this.dependencyFactory = new DefaultAppointmentDependencyFactory();
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
		protected override NotificationCollection<AppointmentDependency> CreateCollection() {
			return Storage != null ? new AppointmentDependencyCollection(Storage) : new AppointmentDependencyCollection();
		}
		protected internal override DataManager<AppointmentDependency> CreateDataManager() {
			return new AppointmentDependencyDataManager();
		}
		protected internal override MappingInfoBase<AppointmentDependency> CreateMappingInfo() {
			return new AppointmentDependencyMappingInfo();
		}
		protected internal override CustomFieldMappingCollectionBase<AppointmentDependency> CreateCustomMappingsCollection() {
			return new AppointmentDependencyCustomFieldMappingCollection();
		}
		protected internal virtual void SubscribeUserInterfaceObjectsEvents() {
		}
		protected internal virtual void UnsubscribeUserInterfaceObjectsEvents() {
		}
		protected override ISchedulerUnboundDataKeeper CreateUnboundDataKeeper() {
			return new UnboundDataKeeper();
		}
		protected internal override void UpdateKeyFieldName() {
		}
		#region AppointmentDependency collection methods
		public void AddRange(AppointmentDependency[] dependencies) {
			Items.AddRange(dependencies);
		}
		public int Add(AppointmentDependency dependency) {
			return Items.Add(dependency);
		}
		public void Remove(AppointmentDependency dependency) {
			Items.Remove(dependency);
		}
		public bool Contains(AppointmentDependency dependency) {
			return Items.Contains(dependency);
		}
		#endregion
		public AppointmentDependency CreateAppointmentDependency(object parentId, object dependentId) {
			AppointmentDependency dependency = DependencyFactory.CreateAppointmentDependency(parentId, dependentId);
			CreateCustomFields(dependency);
			return dependency;
		}
		public AppointmentDependency CreateAppointmentDependency(object parentId, object dependentId, AppointmentDependencyType type) {
			AppointmentDependency dependency = DependencyFactory.CreateAppointmentDependency(parentId, dependentId, type);
			CreateCustomFields(dependency);
			return dependency;
		}
		protected internal override AppointmentDependency CreateObject(object rowHandle) {
			return CreateAppointmentDependency(AppointmentDependencyInstance.Empty, AppointmentDependencyInstance.Empty);
		}
		protected internal override ObjectsNonPersistentInformation CreateNonPersistentInformation() {
			return null;
		}
		[Obsolete("Please, use SetAppointmentDependencyFactory(IAppointmentDependencyFactory factory) instead.", false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public void SetDependencyFactory(IDependencyFactory factory) {
		}
		public void SetAppointmentDependencyFactory(IAppointmentDependencyFactory factory) {
			if (factory == null)
				factory = new DefaultAppointmentDependencyFactory();
			if (dependencyFactory == factory)
				return;
			if (dependencyFactory.GetType() == typeof(DefaultAppointmentDependencyFactory) &&
				factory.GetType() == typeof(DefaultAppointmentDependencyFactory))
				return;
			this.dependencyFactory = factory;
			RaiseReload(true);
		}
		public void AttachStorage(ISchedulerStorageBase storage) {
			SetStorageCore(storage);
			Items.SetStorageCore(storage);
		}
		protected internal override void ApplyNonPersistentInformation(ObjectsNonPersistentInformation nonPersistentInfo) {
		}
		protected internal override void LoadObjectsFromDataManager() {
			AppointmentDependencyCollectionLoader loader = new AppointmentDependencyCollectionLoader(this, this);
			loader.LoadDependencies();
		}
		#region IAppointmentDependencyLoaderProvider Members
		AppointmentDependency IAppointmentDependencyLoaderProvider.LoadDependency(IAppointmentDependencyStorage storage, int objectIndex) {
			return LoadObjectFromDataManager(objectIndex);
		}
		int IAppointmentDependencyLoaderProvider.TotalObjectCount { get { return DataManager.SourceObjectCount; } }
		#endregion
		#region IDependencyStorage (members)
		AppointmentDependencyBaseCollection IInternalAppointmentDependencyStorage.GetAllDependenciesById(object id) {
			return Items.GetAllDependenciesById(id);
		}
		bool IInternalAppointmentDependencyStorage.Contains(object id1, object id2) {
			return Items.Contains(id1, id2);
		}
		#endregion
	}
}
