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
using System.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Utils.Serializing;
using DevExpress.Utils;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler {
	public interface ISchedulerStorage : ISchedulerStorageBase {
		new IAppointmentStorage Appointments { get; }
		void SetAppointmentId(Appointment apt, object id);
		Color GetLabelColor(object labelId);
		event PrepareFilterColumnEventHandler PrepareAppointmentFilterColumn;
		event PrepareFilterColumnEventHandler PrepareResourceFilterColumn;
	}
	#region SchedulerStorage
	[
	System.Runtime.InteropServices.ComVisible(false),
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerStorage), DevExpress.Utils.ControlConstants.BitmapPath + "SchedulerStorage.bmp"),
	Designer("DevExpress.XtraScheduler.Design.SchedulerStorageDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A component that holds data for the SchedulerControl.")
	]
	public class SchedulerStorage : SchedulerStorageBase, ISchedulerStorage, IInternalSchedulerStorage, IFilteredComponentsProvider {
		public SchedulerStorage() {
		}
		public SchedulerStorage(IContainer components)
			: base(components) {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerStorageAppointments"),
#endif
 Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppointmentStorage Appointments { get { return (AppointmentStorage)InnerAppointments; } }
		IAppointmentStorage ISchedulerStorage.Appointments {
			get { return Appointments; }
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerStorageResources"),
#endif
Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ResourceStorage Resources { get { return (ResourceStorage)InnerResources; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerStorageAppointmentDependencies"),
#endif
Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppointmentDependencyStorage AppointmentDependencies { get { return (AppointmentDependencyStorage)InnerAppointmentDependencies; } }
		#endregion
		#region Events
		#region PrepareAppointmentFilterColumn
		PrepareFilterColumnEventHandler onPrepareAppointmentFilterColumn;
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerStoragePrepareAppointmentFilterColumn")]
#endif
		public event PrepareFilterColumnEventHandler PrepareAppointmentFilterColumn { add { onPrepareAppointmentFilterColumn += value; } remove { onPrepareAppointmentFilterColumn -= value; } }
		bool IInternalSchedulerStorage.RaisePrepareAppointmentFilterColumn(FilterColumn filterColumn) { 
			if (onPrepareAppointmentFilterColumn != null) {
				PrepareFilterColumnEventArgs args = new PrepareFilterColumnEventArgs(filterColumn);
				onPrepareAppointmentFilterColumn(this, args);
				return args.Cancel;
			}
			return false;
		}
		#endregion
		#region PrepareResourceFilterColumn
		PrepareFilterColumnEventHandler onPrepareResourceFilterColumn;
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerStoragePrepareResourceFilterColumn")]
#endif
		public event PrepareFilterColumnEventHandler PrepareResourceFilterColumn { add { onPrepareResourceFilterColumn += value; } remove { onPrepareResourceFilterColumn -= value; } }
		bool IInternalSchedulerStorage.RaisePrepareResourceFilterColumn(FilterColumn filterColumn) { 
			if (onPrepareResourceFilterColumn != null) {
				PrepareFilterColumnEventArgs args = new PrepareFilterColumnEventArgs(filterColumn);
				onPrepareResourceFilterColumn(this, args);
				return args.Cancel;
			}
			return false;
		}
		#endregion
		#endregion
		#region CreateAppointmentStorage
		protected internal override AppointmentStorageBase CreateAppointmentStorage() {
			return new AppointmentStorage(this);
		}
		#endregion
		#region CreateResourceStorage
		protected internal override ResourceStorageBase CreateResourceStorage() {
			return new ResourceStorage(this);
		}
		#endregion
		protected internal override AppointmentDependencyStorageBase CreateAppointmentDependencyStorage() {
			return new AppointmentDependencyStorage(this);
		}
		#region IFilteredComponentsProvider Members
		public ICollection GetFilteredComponents() {
			List<IFilteredComponent> collection = new List<IFilteredComponent>();
			collection.Add(Appointments);
			collection.Add(Resources);
			return collection;
		}
		#endregion
		protected internal virtual IAppointmentStatus GetStatus(object statusId) {
			return base.GetInnerStatus(statusId);
		}
		public virtual void SetAppointmentId(Appointment apt, object id) {
			if (apt == null)
				Exceptions.ThrowArgumentNullException("apt");
			apt.SetId(id);
			Appointments.Items.UpdateIdHash(apt);
		}
		public Color GetLabelColor(object labelId) {
			return Appointments.Labels.GetById(labelId).Color;
		}
	}
	#endregion
	public interface IAppointmentStorage : IAppointmentStorageBase {
		bool CommitIdToDataSource { get; set; }
		new AppointmentLabelCollection Labels { get; }
		new AppointmentStatusCollection Statuses { get; }
		new AppointmentCustomFieldMappingCollection CustomFieldMappings { get; }
	}
	#region AppointmentStorage
	public class AppointmentStorage : AppointmentStorageBase, IAppointmentStorage, IFilteredComponent, IXtraSupportShouldSerialize, IXtraSupportDeserializeCollectionItem {
		PersistentObjectStorageFilteredComponentImplementation<Appointment> filteredControlImplementation;
		XtraSupportShouldSerializeHelper shouldSerializeHelper = new XtraSupportShouldSerializeHelper();
		public AppointmentStorage(SchedulerStorage storage)
			: base(storage) {
			this.filteredControlImplementation = CreateFilteredControlImplementation();
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("AppointmentStorageCommitIdToDataSource"),
#endif
		DefaultValue(true), Category(SRCategoryNames.Behavior), NotifyParentProperty(true), AutoFormatDisable()]
		public bool CommitIdToDataSource {
			get { return Mappings.CommitIdToDataSource; }
			set { Mappings.CommitIdToDataSource = value; }
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("AppointmentStorageCustomFieldMappings"),
#endif
Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatDisable()]
		public new AppointmentCustomFieldMappingCollection CustomFieldMappings { get { return (AppointmentCustomFieldMappingCollection)base.CustomFieldMappings; } }
		internal PersistentObjectStorageFilteredComponentImplementation<Appointment> FilteredControlImplementation { get { return filteredControlImplementation; } }
		#region Statuses
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("AppointmentStorageStatuses"),
#endif
 Category(SRCategoryNames.Appearance)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatDisable()]
		public AppointmentStatusCollection Statuses { get { return (AppointmentStatusCollection)InnerStatuses; } }
		internal bool ShouldSerializeStatuses() {
			return !InnerStatuses.HasDefaultContent();
		}
		internal void ResetStatuses() {
			InnerStatuses.LoadDefaults();
		}
		#endregion
		#region Labels
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("AppointmentStorageLabels"),
#endif
		Category(SRCategoryNames.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatDisable()]
		public AppointmentLabelCollection Labels { get { return (AppointmentLabelCollection)InnerLabels; } }
		internal bool ShouldSerializeLabels() {
			return !InnerLabels.HasDefaultContent();
		}
		internal void ResetLabels() {
			InnerLabels.LoadDefaults();
		}
		#endregion
		#endregion
		#region IXtraSupportShouldSerialize Members
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			return shouldSerializeHelper.ShouldSerialize(propertyName);
		}
		#endregion
		#region IXtraSupportDeserializeCollectionItem Members
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return null;
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
		}
		#endregion
		[Editor("DevExpress.XtraScheduler.Design.AppointmentFilterEditor, " + AssemblyInfo.SRAssemblySchedulerDesign, typeof(System.Drawing.Design.UITypeEditor))]
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("AppointmentStorageFilter"),
#endif
		XtraSerializableProperty(), DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		NotifyParentProperty(true), AutoFormatDisable()]
		public override string Filter {
			get {
				return base.Filter;
			}
			set {
				base.Filter = value;
			}
		}
		#region IFilteredComponent Members
		IBoundPropertyCollection IFilteredComponent.CreateFilterColumnCollection() {
			return FilteredControlImplementation.CreateFilterColumnCollection();
		}
		#region Events
		event EventHandler IFilteredComponentBase.PropertiesChanged { add { FilteredControlImplementation.PropertiesChanged += value; } remove { FilteredControlImplementation.PropertiesChanged -= value; } }
		event EventHandler IFilteredComponentBase.RowFilterChanged { add { FilteredControlImplementation.RowFilterChanged += value; } remove { FilteredControlImplementation.RowFilterChanged -= value; } }
		#endregion
		DevExpress.Data.Filtering.CriteriaOperator IFilteredComponentBase.RowCriteria { get { return FilteredControlImplementation.RowCriteria; } set { FilteredControlImplementation.RowCriteria = value; } }
		#endregion
		protected internal virtual PersistentObjectStorageFilteredComponentImplementation<Appointment> CreateFilteredControlImplementation() {
			return new AppointmentStorageFilteredComponentImplementation(this);
		}
		protected override IAppointmentStatusStorage CreateAppointmentStatusCollection() {
			return new AppointmentStatusCollection();
		}
		protected override IAppointmentLabelStorage CreateAppointmentLabelCollection() {
			return new AppointmentLabelCollection();
		}
	}
	#endregion
	public interface IResourceStorage : IResourceStorageBase {
		new ResourceCustomFieldMappingCollection CustomFieldMappings { get; }
	}
	#region ResourceStorage
	public class ResourceStorage : ResourceStorageBase, IResourceStorage, IFilteredComponent {
		PersistentObjectStorageFilteredComponentImplementation<Resource> filteredControlImplementation;
		public ResourceStorage(SchedulerStorage storage)
			: base(storage) {
			this.filteredControlImplementation = CreateFilteredControlImplementation();
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ResourceStorageMappings"),
#endif
		Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ResourceMappingInfo Mappings { get { return InnerMappings; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ResourceStorageCustomFieldMappings"),
#endif
		Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ResourceCustomFieldMappingCollection CustomFieldMappings { get { return (ResourceCustomFieldMappingCollection)base.CustomFieldMappings; } }
		internal PersistentObjectStorageFilteredComponentImplementation<Resource> FilteredControlImplementation { get { return filteredControlImplementation; } }
		#region Filter
		[Editor("DevExpress.XtraScheduler.Design.ResourceFilterEditor, " + AssemblyInfo.SRAssemblySchedulerDesign, typeof(System.Drawing.Design.UITypeEditor))]
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ResourceStorageFilter"),
#endif
		XtraSerializableProperty(), DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		NotifyParentProperty(true), AutoFormatDisable()]
		public override string Filter {
			get { return base.Filter; }
			set { base.Filter = value; }
		}
		#endregion
		#endregion
		#region IFilteredComponent Members
		IBoundPropertyCollection IFilteredComponent.CreateFilterColumnCollection() {
			return FilteredControlImplementation.CreateFilterColumnCollection();
		}
		#region Events
		event EventHandler IFilteredComponentBase.PropertiesChanged {
			add {
				FilteredControlImplementation.PropertiesChanged += value;
			}
			remove {
				FilteredControlImplementation.PropertiesChanged -= value;
			}
		}
		event EventHandler IFilteredComponentBase.RowFilterChanged {
			add {
				FilteredControlImplementation.RowFilterChanged += value;
			}
			remove {
				FilteredControlImplementation.RowFilterChanged -= value;
			}
		}
		#endregion
		CriteriaOperator IFilteredComponentBase.RowCriteria { get { return FilteredControlImplementation.RowCriteria; } set { FilteredControlImplementation.RowCriteria = value; } }
		#endregion
		protected override IResourceFactory CreateResourceFactory() {
			return new WinResourceFactory();
		}
		protected internal virtual PersistentObjectStorageFilteredComponentImplementation<Resource> CreateFilteredControlImplementation() {
			return new ResourceStorageFilteredComponentImplementation(this);
		}
	}
	#endregion
	#region AppointmentDependencyStorage
	public class AppointmentDependencyStorage : AppointmentDependencyStorageBase {
		public AppointmentDependencyStorage(SchedulerStorage storage)
			: base(storage) {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("AppointmentDependencyStorageMappings"),
#endif
		Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new AppointmentDependencyMappingInfo Mappings { get { return base.Mappings; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("AppointmentDependencyStorageCustomFieldMappings"),
#endif
		Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new AppointmentDependencyCustomFieldMappingCollection CustomFieldMappings { get { return base.CustomFieldMappings; } }
	}
	#endregion
	#endregion
}
namespace DevExpress.XtraScheduler.Internal {
	public class WinResourceFactory : IResourceFactory {
		public Resource CreateResource() {
			return new Internal.Implementations.ResourceInstance();
		}
	}
}
