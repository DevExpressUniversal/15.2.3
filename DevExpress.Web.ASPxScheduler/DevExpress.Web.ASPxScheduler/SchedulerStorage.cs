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
using System.Web.UI;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Data;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Serializing;
using System.IO;
using System.IO.Compression;
using DevExpress.Web.ASPxScheduler.Internal;
using System.Drawing.Design;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.Web.ASPxScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
using System.Drawing;
namespace DevExpress.Web.ASPxScheduler {
	#region ASPxSchedulerStorage
	public interface IASPxSchedulerStorage : ISchedulerStorageBase {
		new IASPxAppointmentStorage Appointments { get; }
		Color GetLabelColor(object labelId);
	}
	[ToolboxItem(false)]
	public class ASPxSchedulerStorage : SchedulerStorageBase, IASPxSchedulerStorage {
		ASPxScheduler control;
		public ASPxSchedulerStorage() {
		}
		public ASPxSchedulerStorage(ASPxScheduler control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
		}
		#region Properties
		#region Appointments
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerStorageAppointments"),
#endif
		Category(SRCategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ASPxAppointmentStorage Appointments { get { return (ASPxAppointmentStorage)InnerAppointments; } }
		IASPxAppointmentStorage IASPxSchedulerStorage.Appointments {
			get { return Appointments; }
		}
		#endregion
		#region Resources
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerStorageResources"),
#endif
		Category(SRCategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ASPxResourceStorage Resources { get { return (ASPxResourceStorage)InnerResources; } }
		#endregion
		protected internal ASPxScheduler Control { get { return control; } }
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event ReminderEventHandler ReminderAlert { add { base.ReminderAlert += value; } remove { base.ReminderAlert -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectCancelEventHandler ResourceChanging { add { base.ResourceChanging += value; } remove { base.ResourceChanging -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event CancelListChangedEventHandler ResourceCollectionAutoReloading { add { base.ResourceCollectionAutoReloading += value; } remove { base.ResourceCollectionAutoReloading -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event EventHandler ResourceCollectionCleared { add { base.ResourceCollectionCleared += value; } remove { base.ResourceCollectionCleared -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event EventHandler ResourceCollectionLoaded { add { base.ResourceCollectionLoaded += value; } remove { base.ResourceCollectionLoaded -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectCancelEventHandler ResourceDeleting { add { base.ResourceDeleting += value; } remove { base.ResourceDeleting -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectCancelEventHandler ResourceInserting { add { base.ResourceInserting += value; } remove { base.ResourceInserting -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectsEventHandler ResourcesChanged { add { base.ResourcesChanged += value; } remove { base.ResourcesChanged -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectsEventHandler ResourcesDeleted { add { base.ResourcesDeleted += value; } remove { base.ResourcesDeleted -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectsEventHandler ResourcesInserted { add { base.ResourcesInserted += value; } remove { base.ResourcesInserted -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectCancelEventHandler AppointmentChanging { add { base.AppointmentChanging += value; } remove { base.AppointmentChanging -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event CancelListChangedEventHandler AppointmentCollectionAutoReloading { add { base.AppointmentCollectionAutoReloading += value; } remove { base.AppointmentCollectionAutoReloading -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event EventHandler AppointmentCollectionCleared { add { base.AppointmentCollectionCleared += value; } remove { base.AppointmentCollectionCleared -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event EventHandler AppointmentCollectionLoaded { add { base.AppointmentCollectionLoaded += value; } remove { base.AppointmentCollectionLoaded -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectCancelEventHandler AppointmentDeleting { add { base.AppointmentDeleting += value; } remove { base.AppointmentDeleting -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectsEventHandler AppointmentDependenciesChanged { add { base.AppointmentDependenciesChanged += value; } remove { base.AppointmentDependenciesChanged -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectsEventHandler AppointmentDependenciesDeleted { add { base.AppointmentDependenciesDeleted += value; } remove { base.AppointmentDependenciesDeleted -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectsEventHandler AppointmentDependenciesInserted { add { base.AppointmentDependenciesInserted += value; } remove { base.AppointmentDependenciesInserted -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectCancelEventHandler AppointmentDependencyChanging { add { base.AppointmentDependencyChanging += value; } remove { base.AppointmentDependencyChanging -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event CancelListChangedEventHandler AppointmentDependencyCollectionAutoReloading { add { base.AppointmentDependencyCollectionAutoReloading += value; } remove { base.AppointmentDependencyCollectionAutoReloading -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event EventHandler AppointmentDependencyCollectionCleared { add { base.AppointmentDependencyCollectionCleared += value; } remove { base.AppointmentDependencyCollectionCleared -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event EventHandler AppointmentDependencyCollectionLoaded { add { base.AppointmentDependencyCollectionLoaded += value; } remove { base.AppointmentDependencyCollectionLoaded -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectCancelEventHandler AppointmentDependencyDeleting { add { base.AppointmentDependencyDeleting += value; } remove { base.AppointmentDependencyDeleting -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectCancelEventHandler AppointmentDependencyInserting { add { base.AppointmentDependencyInserting += value; } remove { base.AppointmentDependencyInserting -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectCancelEventHandler AppointmentInserting { add { base.AppointmentInserting += value; } remove { base.AppointmentInserting -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectsEventHandler AppointmentsChanged { add { base.AppointmentsChanged += value; } remove { base.AppointmentsChanged -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectsEventHandler AppointmentsDeleted { add { base.AppointmentsDeleted += value; } remove { base.AppointmentsDeleted -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectsEventHandler AppointmentsInserted { add { base.AppointmentsInserted += value; } remove { base.AppointmentsInserted -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event FetchAppointmentsEventHandler FetchAppointments { add { base.FetchAppointments += value; } remove { base.FetchAppointments -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectCancelEventHandler FilterAppointment { add { base.FilterAppointment += value; } remove { base.FilterAppointment -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectCancelEventHandler FilterDependency { add { base.FilterDependency += value; } remove { base.FilterDependency -= value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event PersistentObjectCancelEventHandler FilterResource { add { base.FilterResource += value; } remove { base.FilterResource -= value; } }
		public Color GetLabelColor(object labelId) {
			return Appointments.Labels.GetById(labelId).Color;
		}
		protected internal override AppointmentStorageBase CreateAppointmentStorage() {
			return new ASPxAppointmentStorage(this);
		}
		protected internal override ResourceStorageBase CreateResourceStorage() {
			return new ASPxResourceStorage(this);
		}
		protected internal override ReminderEngine CreateRemindersEngine() {
			return new ASPxReminderEngine();
		}
		protected internal override AppointmentDependencyStorageBase CreateAppointmentDependencyStorage() {
			return new AppointmentDependencyStorageBase();
		}
		public virtual void SetAppointmentId(Appointment apt, object id) {
			AppointmentIdHelper.SetAppointmentId(apt, id);
		}
		public virtual object GetAppointmentId(Appointment apt) {
			return AppointmentIdHelper.GetAppointmentId(apt);
		}
		protected internal override bool CanRemoveAlertNotification(ReminderAlertNotification notification) {
			return false;
		}
		protected internal virtual AppointmentStatus GetStatus(int statusId) {
			return (AppointmentStatus)base.GetInnerStatus(statusId);
		}
		protected internal override void Assign(ISchedulerStorageBase source) {
			base.Assign(source);
			ASPxSchedulerStorage storage = source as ASPxSchedulerStorage;
			if (storage != null) {
				BeginUpdate();
				try {
					Appointments.Assign(storage.Appointments);
					Resources.Assign(storage.Resources);
				} finally {
					EndUpdate();
				}
			}
		}
		protected override void InitializeTimeZoneEngine() {
			string timeZoneId = System.Configuration.ConfigurationManager.AppSettings["ServerTimeZoneId"];
			if (String.IsNullOrEmpty(timeZoneId))
				return;
			try {
				TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
				if (timeZone == null)
					return;
				TimeZoneEngine.OperationTimeZone = timeZone;
			} finally { }
		}
	}
	#endregion
	#region ASPxAppointmentStorage
	public interface IASPxAppointmentStorage : IAppointmentStorageBase {
		bool AutoRetrieveId { get; set; }
	}
	[Editor("DevExpress.Web.ASPxScheduler.Design.ASPxAppointmentStorageTypeEditor," + AssemblyInfo.SRAssemblySchedulerWebDesignFull, typeof(UITypeEditor))]
	public class ASPxAppointmentStorage : AppointmentStorageBase, IASPxAppointmentStorage, IXtraSupportShouldSerialize, IXtraSupportDeserializeCollectionItem, ISchedulerMappingFieldChecker {
		XtraSupportShouldSerializeHelper shouldSerializeHelper = new XtraSupportShouldSerializeHelper();
		bool autoRetrieveId = false;
		public ASPxAppointmentStorage(ASPxSchedulerStorage storage)
			: base(storage) {
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("Statuses", XtraShouldSerializeStatuses);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("Labels", XtraShouldSerializeLabels);
		}
		#region Properties
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentStorageMappings"),
#endif
		Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public new ASPxAppointmentMappingInfo Mappings { get { return (ASPxAppointmentMappingInfo)base.Mappings; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override object DataSource { get { return base.DataSource; } set { base.DataSource = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string DataMember { get { return base.DataMember; } set { base.DataMember = value; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentStorageCustomFieldMappings"),
#endif
		Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true), AutoFormatDisable()]
		public new ASPxAppointmentCustomFieldMappingCollection CustomFieldMappings { get { return (ASPxAppointmentCustomFieldMappingCollection)base.CustomFieldMappings; } }
		#region Statuses
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentStorageStatuses"),
#endif
Category(SRCategoryNames.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatDisable(),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public AppointmentStatusCollection Statuses { get { return (AppointmentStatusCollection)InnerStatuses; } }
		internal bool ShouldSerializeStatuses() {
			return !InnerStatuses.HasDefaultContent();
		}
		internal void ResetStatuses() {
			InnerStatuses.LoadDefaults();
		}
		internal bool XtraShouldSerializeStatuses() {
			return ShouldSerializeStatuses();
		}
		internal object XtraCreateStatusesItem(XtraItemEventArgs e) {
			return new AppointmentStatus(null);
		}
		internal void XtraSetIndexStatusesItem(XtraSetItemIndexEventArgs e) {
			AppointmentStatus status = e.Item.Value as AppointmentStatus;
			if (status == null)
				return;
			InnerStatuses.Add(status);
		}
		#endregion
		#region Labels
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentStorageLabels"),
#endif
Category(SRCategoryNames.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatDisable(),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public AppointmentLabelCollection Labels { get { return (AppointmentLabelCollection)InnerLabels; } }
		internal bool ShouldSerializeLabels() {
			return !InnerLabels.HasDefaultContent();
		}
		internal void ResetLabels() {
			InnerLabels.LoadDefaults();
		}
		internal bool XtraShouldSerializeLabels() {
			return ShouldSerializeLabels();
		}
		internal object XtraCreateLabelsItem(XtraItemEventArgs e) {
			return new AppointmentLabel(null, Color.Empty, null, null);
		}
		internal void XtraSetIndexLabelsItem(XtraSetItemIndexEventArgs e) {
			AppointmentLabel label = e.Item.Value as AppointmentLabel;
			if (label == null)
				return;
			InnerLabels.Add(label);
		}
		#endregion
		#region CommitIdToDataSource
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentStorageCommitIdToDataSource"),
#endif
		DefaultValue(true), XtraSerializableProperty(), Category(SRCategoryNames.Behavior), NotifyParentProperty(true), AutoFormatDisable()]
		public bool CommitIdToDataSource {
			get { return Mappings.CommitIdToDataSource; }
			set { Mappings.CommitIdToDataSource = value; }
		}
		#endregion
		#region AutoRetrieveId
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentStorageAutoRetrieveId"),
#endif
		DefaultValue(false), XtraSerializableProperty(), Category(SRCategoryNames.Behavior), NotifyParentProperty(true), AutoFormatDisable()]
		public bool AutoRetrieveId { get { return autoRetrieveId; } set { autoRetrieveId = value; } }
		#endregion
		#endregion
		protected internal override MappingInfoBase<Appointment> CreateMappingInfo() {
			return new ASPxAppointmentMappingInfo();
		}
		protected internal override DataManager<Appointment> CreateDataManager() {
			return new ASPxAppointmentDataManager();
		}
		protected internal override void UpdateKeyFieldName() {
			this.DataManager.KeyFieldName = Mappings.AppointmentId;
		}
		protected internal override CustomFieldMappingCollectionBase<Appointment> CreateCustomMappingsCollection() {
			return new ASPxAppointmentCustomFieldMappingCollection();
		}
		#region IXtraSupportShouldSerialize Members
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			return shouldSerializeHelper.ShouldSerialize(propertyName);
		}
		#endregion
		#region IXtraSupportDeserializeCollectionItem Members
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			switch (propertyName) {
				case "Statuses":
					return XtraCreateStatusesItem(e);
				case "Labels":
					return XtraCreateLabelsItem(e);
				default:
					return null;
			}
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			switch (propertyName) {
				case "Statuses":
					XtraSetIndexStatusesItem(e);
					break;
				case "Labels":
					XtraSetIndexLabelsItem(e);
					break;
			}
		}
		#endregion
		protected override IAppointmentStatusStorage CreateAppointmentStatusCollection() {
			return new AppointmentStatusCollection();
		}
		protected override IAppointmentLabelStorage CreateAppointmentLabelCollection() {
			return new AppointmentLabelCollection();
		}
		protected internal override void Assign(PersistentObjectStorage<Appointment> source) {
			base.Assign(source);
			ASPxAppointmentStorage storage = source as ASPxAppointmentStorage;
			if (storage != null) {
				BeginUpdateInternal();
				try {
					CommitIdToDataSource = storage.CommitIdToDataSource;
					AutoRetrieveId = storage.AutoRetrieveId;
					Labels.Assign(storage.Labels);
					Mappings.Assign(storage.Mappings);
					CustomFieldMappings.Assign(storage.CustomFieldMappings);
					Statuses.Assign(storage.Statuses);
				} finally {
					EndUpdateInternal();
				}
			}
		}
		#region ISchedulerMappingFieldProvider
		bool ISchedulerMappingFieldChecker.HasFields() {
			foreach (KeyValuePair<string, string> keyValue in Mappings.MemberDictionary)
				if (!String.IsNullOrEmpty(keyValue.Value))
					return true;
			foreach (MappingBase mapping in CustomFieldMappings)
				if (!String.IsNullOrEmpty(mapping.Member))
					return true;
			IPrivateFieldOwner privateFieldOwner = Mappings as IPrivateFieldOwner;
			if (privateFieldOwner != null && privateFieldOwner.PrivateFields.Count > 0)
				return true;
			return false;
		}
		bool ISchedulerMappingFieldChecker.Contains(string fieldName) {
			if (Mappings.MemberDictionary.ContainsValue(fieldName))
				return true;
			IPrivateFieldOwner privateFieldOwner = Mappings as IPrivateFieldOwner;
			if (privateFieldOwner != null) {
				if (privateFieldOwner.PrivateFields.Contains(fieldName))
					return true;
			}
			foreach (MappingBase mapping in CustomFieldMappings)
				if (mapping.Member == fieldName)
					return true;
			return false;
		}
		#endregion
	}
	#endregion
	public class ASPxResourceFactory : IResourceFactory {
		public Resource CreateResource() {
			return new ResourceInstance();
		}
	}
	#region ASPxResourceStorage
	[Editor("DevExpress.Web.ASPxScheduler.Design.ASPxResourceStorageTypeEditor," + AssemblyInfo.SRAssemblySchedulerWebDesignFull, typeof(UITypeEditor))]
	public class ASPxResourceStorage : ResourceStorageBase, ISchedulerMappingFieldChecker {
		public ASPxResourceStorage(ASPxSchedulerStorage storage)
			: base(storage) {
		}
		#region Properties
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxResourceStorageMappings"),
#endif
		Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public new ASPxResourceMappingInfo Mappings { get { return (ASPxResourceMappingInfo)InnerMappings; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override object DataSource { get { return base.DataSource; } set { base.DataSource = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string DataMember { get { return base.DataMember; } set { base.DataMember = value; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxResourceStorageCustomFieldMappings"),
#endif
		Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true), AutoFormatDisable()]
		public new ASPxResourceCustomFieldMappingCollection CustomFieldMappings { get { return (ASPxResourceCustomFieldMappingCollection)base.CustomFieldMappings; } }
		#endregion
		protected internal override MappingInfoBase<XtraScheduler.Resource> CreateMappingInfo() {
			return new ASPxResourceMappingInfo();
		}
		protected internal override CustomFieldMappingCollectionBase<XtraScheduler.Resource> CreateCustomMappingsCollection() {
			return new ASPxResourceCustomFieldMappingCollection();
		}
		protected internal override void Assign(PersistentObjectStorage<XtraScheduler.Resource> source) {
			base.Assign(source);
			ASPxResourceStorage storage = source as ASPxResourceStorage;
			if (storage != null) {
				BeginUpdateInternal();
				try {
					Mappings.Assign(storage.Mappings);
					CustomFieldMappings.Assign(storage.CustomFieldMappings);
				} finally {
					EndUpdateInternal();
				}
			}
		}
		#region ISchedulerMappingFieldProvider
		bool ISchedulerMappingFieldChecker.HasFields() {
			foreach (KeyValuePair<string, string> keyValue in Mappings.MemberDictionary)
				if (!String.IsNullOrEmpty(keyValue.Value))
					return true;
			foreach (MappingBase mapping in CustomFieldMappings)
				if (!String.IsNullOrEmpty(mapping.Member))
					return true;
			IPrivateFieldOwner privateFieldOwner = Mappings as IPrivateFieldOwner;
			if (privateFieldOwner != null && privateFieldOwner.PrivateFields.Count > 0)
				return true;
			return false;
		}
		bool ISchedulerMappingFieldChecker.Contains(string fieldName) {
			if (Mappings.MemberDictionary.ContainsValue(fieldName))
				return true;
			foreach (MappingBase mapping in CustomFieldMappings)
				if (mapping.Member == fieldName)
					return true;
			return false;
		}
		#endregion
		protected override IResourceFactory CreateResourceFactory() {
			return new ASPxResourceFactory();
		}
	}
	#endregion
	#region ASPxAppointmentCustomFieldMappingCollection (compatibility class)
	public class ASPxAppointmentCustomFieldMappingCollection : AppointmentCustomFieldMappingCollection {
		public new ASPxAppointmentCustomFieldMapping this[int index] { get { return (ASPxAppointmentCustomFieldMapping)base[index]; } }
		public new ASPxAppointmentCustomFieldMapping this[string name] { get { return (ASPxAppointmentCustomFieldMapping)base[name]; } }
		public int Add(ASPxAppointmentCustomFieldMapping mapping) {
			return base.Add(mapping);
		}
	}
	#endregion
	#region ASPxResourceCustomFieldMappingCollection (compatibility class)
	public class ASPxResourceCustomFieldMappingCollection : ResourceCustomFieldMappingCollection {
		public new ASPxResourceCustomFieldMapping this[int index] { get { return (ASPxResourceCustomFieldMapping)base[index]; } }
		public new ASPxResourceCustomFieldMapping this[string name] { get { return (ASPxResourceCustomFieldMapping)base[name]; } }
		public int Add(ASPxResourceCustomFieldMapping mapping) {
			return base.Add(mapping);
		}
	}
	#endregion
	#region ASPxAppointmentCustomFieldMapping
	public class ASPxAppointmentCustomFieldMapping : AppointmentCustomFieldMapping {
		public ASPxAppointmentCustomFieldMapping() {
		}
		public ASPxAppointmentCustomFieldMapping(string name, string member)
			: base(name, member) {
		}
		protected ASPxAppointmentCustomFieldMapping(string name, string member, FieldValueType valueType)
			: base(name, member, valueType) {
		}
		#region Properties
		#region Member
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentCustomFieldMappingMember"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerAppointmentDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true)]
		public override string Member { get { return base.Member; } set { base.Member = value; } }
		#endregion
		#endregion
	}
	#endregion
	#region ASPxResourceCustomFieldMapping
	public class ASPxResourceCustomFieldMapping : ResourceCustomFieldMapping {
		public ASPxResourceCustomFieldMapping() {
		}
		public ASPxResourceCustomFieldMapping(string name, string member)
			: base(name, member) {
		}
		protected ASPxResourceCustomFieldMapping(string name, string member, FieldValueType valueType)
			: base(name, member, valueType) {
		}
		#region Properties
		#region Member
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxResourceCustomFieldMappingMember"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerResourceDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true)]
		public override string Member { get { return base.Member; } set { base.Member = value; } }
		#endregion
		#endregion
	}
	#endregion
	#region ASPxAppointmentMappingInfo
	[Editor("DevExpress.Web.ASPxScheduler.Design.ASPxAppointmentMappingInfoTypeEditor," + AssemblyInfo.SRAssemblySchedulerWebDesignFull, typeof(UITypeEditor))]
	public class ASPxAppointmentMappingInfo : AppointmentMappingInfo, IPrivateFieldOwner {
		#region Properties
		#region UsePercentComplete
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override bool UsePercentComplete { get { return false; } }
		#endregion
		#region AppointmentId
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentMappingInfoAppointmentId"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerAppointmentDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public override string AppointmentId { get { return base.AppointmentId; } set { base.AppointmentId = value; } }
		#endregion
		#region AllDay
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentMappingInfoAllDay"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerAppointmentDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public override string AllDay { get { return base.AllDay; } set { base.AllDay = value; } }
		#endregion
		#region Status
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentMappingInfoStatus"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerAppointmentDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public override string Status { get { return base.Status; } set { base.Status = value; } }
		#endregion
		#region Label
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentMappingInfoLabel"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerAppointmentDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public override string Label { get { return base.Label; } set { base.Label = value; } }
		#endregion
		#region Description
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentMappingInfoDescription"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerAppointmentDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public override string Description { get { return base.Description; } set { base.Description = value; } }
		#endregion
		#region End
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentMappingInfoEnd"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerAppointmentDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public override string End { get { return base.End; } set { base.End = value; } }
		#endregion
		#region Location
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentMappingInfoLocation"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerAppointmentDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public override string Location { get { return base.Location; } set { base.Location = value; } }
		#endregion
		#region RecurrenceInfo
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentMappingInfoRecurrenceInfo"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerAppointmentDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public override string RecurrenceInfo { get { return base.RecurrenceInfo; } set { base.RecurrenceInfo = value; } }
		#endregion
		#region ReminderInfo
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentMappingInfoReminderInfo"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerAppointmentDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public override string ReminderInfo { get { return base.ReminderInfo; } set { base.ReminderInfo = value; } }
		#endregion
		#region ResourceId
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentMappingInfoResourceId"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerAppointmentDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public override string ResourceId { get { return base.ResourceId; } set { base.ResourceId = value; } }
		#endregion
		#region Start
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentMappingInfoStart"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerAppointmentDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public override string Start { get { return base.Start; } set { base.Start = value; } }
		#endregion
		#region Subject
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentMappingInfoSubject"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerAppointmentDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public override string Subject { get { return base.Subject; } set { base.Subject = value; } }
		#endregion
		#region Type
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxAppointmentMappingInfoType"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerAppointmentDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public override string Type { get { return base.Type; } set { base.Type = value; } }
		#endregion
		#region PercentComplete
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string PercentComplete { get { return base.PercentComplete; } set { base.PercentComplete = value; } }
		#endregion
		#endregion
		public override string[] GetRequiredMappingNames() {
			List<string> result = new List<string>();
			result.AddRange(base.GetRequiredMappingNames());
			result.Add(AppointmentSR.Id);
			return result.ToArray();
		}
		protected internal override MappingsTokenInfos GetMappingsTokenInfos() {
			return new ASPxAppointmentMappingsTokenInfos();
		}
		#region IPrivateFieldOwner
		List<string> privateFields = new List<string>();
		List<string> IPrivateFieldOwner.PrivateFields { get { return privateFields; } }
		#endregion
	}
	#endregion
	#region ASPxResourceMappingInfo
	[Editor("DevExpress.Web.ASPxScheduler.Design.ASPxResourceMappingInfoTypeEditor," + AssemblyInfo.SRAssemblySchedulerWebDesignFull, typeof(UITypeEditor))]
	public class ASPxResourceMappingInfo : ResourceMappingInfo, IPrivateFieldOwner {
		#region Properties
		#region UseParentId
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override bool UseParentId { get { return false; } }
		#endregion
		#region SRId
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string SRId { get { return ASPxResourceSR.ResourceId; } }
		#endregion
		#region ResourceId
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxResourceMappingInfoResourceId"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerResourceDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public virtual string ResourceId { get { return Id; } set { Id = value; } }
		#endregion
		#region Id
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(""), TypeConverter("DevExpress.XtraScheduler.Design.ResourceColumnNameConverter," + AssemblyInfo.SRAssemblySchedulerCore), NotifyParentProperty(true), AutoFormatDisable, Localizable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string Id {
			get { return base.Id; }
			set { base.Id = value; }
		}
		#endregion
		#region Caption
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxResourceMappingInfoCaption"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerResourceDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public override string Caption { get { return base.Caption; } set { base.Caption = value; } }
		#endregion
		#region Color
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxResourceMappingInfoColor"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerResourceDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public override string Color { get { return base.Color; } set { base.Color = value; } }
		#endregion
		#region Image
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxResourceMappingInfoImage"),
#endif
DefaultValue(""),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerResourceDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull),
		NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public override string Image { get { return base.Image; } set { base.Image = value; } }
		#endregion
		#region ParentId
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(""), TypeConverter("DevExpress.XtraScheduler.Design.ResourceColumnNameConverter," + AssemblyInfo.SRAssemblySchedulerCore), NotifyParentProperty(true), AutoFormatDisable, Localizable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string ParentId { get { return base.ParentId; } set { base.ParentId = value; } }
		#endregion
		#endregion
		protected internal override ResourceIdMapping GetResourceIdMapping() {
			return new ASPxResourceIdMapping();
		}
		protected internal override MappingsTokenInfos GetMappingsTokenInfos() {
			return new ASPxResourceMappingsTokenInfos();
		}
		#region IPrivateFieldOwner
		List<string> privateFields = new List<string>();
		List<string> IPrivateFieldOwner.PrivateFields { get { return privateFields; } }
		#endregion
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region IPrivateFieldOwner
	public interface IPrivateFieldOwner {
		List<string> PrivateFields { get; }
	}
	#endregion
	#region ASPxResourseSR
	public static class ASPxResourceSR {
		public const string ResourceId = "ResourceId";
	}
	#endregion
	#region ASPxResourceIdMapping
	public class ASPxResourceIdMapping : ResourceIdMapping {
		public override string Name { get { return ASPxResourceSR.ResourceId; } set { } }
	}
	#endregion
	public static class AppointmentIdHelper {
		public static object GetAppointmentId(Appointment apt) {
			if (apt == null)
				Exceptions.ThrowArgumentNullException("apt");
			return apt.Id;
		}
		public static void SetAppointmentId(Appointment apt, object id) {
			if (apt == null)
				Exceptions.ThrowArgumentNullException("apt");
			apt.SetId(id);
		}
	}
	#region ASPxAppointmentDataManager
	public class ASPxAppointmentDataManager : AppointmentDataManager {
		public override void CommitNewObject(Appointment obj, MappingCollection mappings) {
			base.CommitNewObject(obj, mappings);
			ObtainAppointmentId(obj, mappings);
		} 
		protected internal virtual void ObtainAppointmentId(Appointment apt, MappingCollection mappings) {
			MappingBase idMapping = mappings[AppointmentSR.Id];
			if (idMapping == null)
				return;
			if (!idMapping.CommitToDataSource)
				LoadObjectProperty(apt, idMapping);
		}
		protected override SchedulerDataController CreateDataController() {
			return new WebSchedulerDataController(ASPxScheduler.ActiveControl.AppointmentDataProvider);
		}
	}
	#endregion
	#region ASPxReminderEngine
	public class ASPxReminderEngine : ReminderEngine {
		bool enabled;
		public override bool Enabled { get { return enabled; } set { enabled = value; } }
	}
	#endregion
	#region ASPxAppointmentMappingsTokenInfos
	public class ASPxAppointmentMappingsTokenInfos : AppointmentMappingsTokenInfos {
		protected internal override void AddPercentCompleteTokenInfoDictionary() {
		}
	}
	#endregion
	#region ASPxResourceMappingsTokenInfos
	public class ASPxResourceMappingsTokenInfos : ResourceMappingsTokenInfos {
		protected internal override void AddIdTokenInfoDictionary() {
			Add(ASPxResourceSR.ResourceId, CreateIdTokenInfoDictionary());
		}
		protected internal override TokenInfoDictionary CreateIdTokenInfoDictionary() {
			TokenInfoDictionary result = new TokenInfoDictionary();
			TokenInfo info = new TokenInfo();
			AppendResourceSynonymsAsAttendants(info);
			result.Add("resource", info);
			result.Add("resourceId", info);
			result.Add("id", info);
			return result;
		}
		protected internal override void AddParentIdTokenInfoDictionary() {
		}
	}
	#endregion
	public class AppointmentCompactSerializer {
		ASPxAppointmentStorage storage;
		public AppointmentCompactSerializer(ASPxAppointmentStorage storage) {
			if (storage == null)
				Exceptions.ThrowArgumentNullException("storage");
			this.storage = storage;
		}
		public string SerializeToString() {
			using (MemoryStream memoryStream = new MemoryStream()) {
				using (DeflateStream stream = new DeflateStream(memoryStream, CompressionMode.Compress, true)) {
					using (BinaryWriter writer = new BinaryWriter(stream)) {
						Serialize(writer);
						writer.Close();
					}
				}
				return Convert.ToBase64String(memoryStream.ToArray());
			}
		}
		protected internal virtual void Serialize(BinaryWriter writer) {
			AppointmentBaseCollection appointments = storage.Items;
			int count = appointments.Count;
			writer.Write(((IInternalSchedulerStorageBase)storage.Storage).CalcTotalAppointmentCountForExchange());
			for (int i = 0; i < count; i++)
				SerializeObject(writer, appointments[i]);
		}
		protected internal virtual void SerializeObject(BinaryWriter writer, Appointment apt) {
			writer.Write((byte)apt.Type);
			writer.Write(apt.Start.Ticks);
			writer.Write(apt.Duration.Ticks);
			writer.Write(apt.AllDay);
			byte[] labelIdBytes = ObjectToByteArrayConverter.ObjectToByteArray(apt.LabelKey);
			writer.Write(labelIdBytes.Length);
			writer.Write(labelIdBytes);
			byte[] statusIdBytes = ObjectToByteArrayConverter.ObjectToByteArray(apt.StatusKey);
			writer.Write(statusIdBytes.Length);
			writer.Write(statusIdBytes);
			writer.Write(apt.Subject);
			writer.Write(apt.Description);
			writer.Write(apt.Location);
			SerializeResources(writer, apt.ResourceIds);
			SerializeRecurrence(writer, apt);
			SerializeReminders(writer, apt.Reminders);
			SerializeCustomFields(writer, apt.CustomFields);
			TypedBinaryWriterEx typedWriter = new TypedBinaryWriterEx(writer);
			typedWriter.WriteObject(apt.Id);
			if (apt.Type == AppointmentType.Pattern && apt.HasExceptions) {
				IInternalAppointment internalApt = (IInternalAppointment)apt;
				int exceptionsCount = internalApt.PatternExceptions.Count;
				for (int i = 0; i < exceptionsCount; i++)
					SerializeObject(writer, internalApt.PatternExceptions[i]);
			}
		}
		protected internal virtual void SerializeResources(BinaryWriter writer, ResourceIdCollection resourceIds) {
			int count = resourceIds.Count;
			if (count == 1 && resourceIds[0] == ResourceBase.Empty.Id) {
				count = 0;
				writer.Write(count);
				return;
			}
			writer.Write(count);
			TypedBinaryWriterEx typedWriter = new TypedBinaryWriterEx(writer);
			for (int i = 0; i < count; i++)
				typedWriter.WriteObject(resourceIds[i]);
		}
		protected internal virtual void SerializeCustomFields(BinaryWriter writer, CustomFieldCollection fields) {
			int count = fields.Count;
			writer.Write(count);
			TypedBinaryWriterEx typedWriter = new TypedBinaryWriterEx(writer);
			for (int i = 0; i < count; i++)
				typedWriter.WriteObject(fields[i]);
		}
		protected internal virtual void SerializeReminders(BinaryWriter writer, ReminderCollection reminders) {
			int count = reminders.Count;
			writer.Write(count);
			for (int i = 0; i < count; i++) {
				Reminder reminder = reminders[i];
				writer.Write(reminder.TimeBeforeStart.Ticks);
				writer.Write(reminder.AlertTime.Ticks);
			}
		}
		protected internal virtual void SerializeRecurrence(BinaryWriter writer, Appointment apt) {
			if (apt.Type == AppointmentType.Pattern)
				SerializeRecurrenceInfo(writer, apt.RecurrenceInfo);
			else if (apt.IsException)
				SerializeOccurrenceInfo(writer, apt);
		}
		protected internal virtual void SerializeRecurrenceInfo(BinaryWriter writer, IRecurrenceInfo recurrenceInfo) {
			writer.Write((byte)recurrenceInfo.DayNumber);
			writer.Write((byte)recurrenceInfo.WeekOfMonth);
			writer.Write(recurrenceInfo.Periodicity);
			writer.Write((byte)recurrenceInfo.Month);
			writer.Write(recurrenceInfo.OccurrenceCount);
			writer.Write((byte)recurrenceInfo.Range);
			writer.Write((byte)recurrenceInfo.Type);
			writer.Write((byte)recurrenceInfo.WeekDays);
			writer.Write(recurrenceInfo.AllDay);
			writer.Write(recurrenceInfo.Start.Ticks);
			writer.Write(recurrenceInfo.Duration.Ticks);
			byte[] recurrenceInfoIdBytes = ObjectToByteArrayConverter.ObjectToByteArray(recurrenceInfo.Id);
			writer.Write(recurrenceInfoIdBytes.Length);
			writer.Write(recurrenceInfoIdBytes);
		}
		protected internal virtual void SerializeOccurrenceInfo(BinaryWriter writer, Appointment apt) {
			byte[] patternIdBytes = ObjectToByteArrayConverter.ObjectToByteArray(apt.RecurrenceInfo.Id);
			writer.Write(patternIdBytes.Length);
			writer.Write(patternIdBytes);
			writer.Write(apt.RecurrenceIndex);
		}
	}
	public class AppointmentCompactDeserializer : IAppointmentLoaderProvider {
		int appointmentCount;
		BinaryReader reader;
		Dictionary<Appointment, object> patternTable;
		public AppointmentCompactDeserializer(string base64String) {
			this.patternTable = new Dictionary<Appointment, object>();
			byte[] bytes = Convert.FromBase64String(base64String);
			MemoryStream memoryStream = new MemoryStream(bytes);
			DeflateStream stream = new DeflateStream(memoryStream, CompressionMode.Decompress, true);
			this.reader = new BinaryReader(stream);
			appointmentCount = reader.ReadInt32();
		}
		#region IAppointmentLoaderProvider Members
		object IAppointmentLoaderProvider.GetPatternId(IAppointmentStorageBase storage, Appointment apt) {
			return patternTable[apt];
		}
		Appointment IAppointmentLoaderProvider.LoadAppointment(IAppointmentStorageBase storage, int objectIndex) {
			AppointmentType type = (AppointmentType)reader.ReadByte();
			Appointment apt = storage.CreateAppointment(type);
			DeserializeObject(apt);
			return apt;
		}
		int IAppointmentLoaderProvider.TotalObjectCount {
			get { return appointmentCount; }
		}
		#endregion
		protected internal virtual void DeserializeObject(Appointment apt) {
			apt.Start = new DateTime(reader.ReadInt64());
			apt.Duration = TimeSpan.FromTicks(reader.ReadInt64());
			apt.AllDay = reader.ReadBoolean();
			int labelIdLength = reader.ReadInt32();
			apt.LabelKey = ObjectToByteArrayConverter.ByteArrayToObject(reader.ReadBytes(labelIdLength));
			int statusIdLength = reader.ReadInt32();
			apt.StatusKey = ObjectToByteArrayConverter.ByteArrayToObject(reader.ReadBytes(statusIdLength));
			apt.Subject = reader.ReadString();
			apt.Description = reader.ReadString();
			apt.Location = reader.ReadString();
			DeserializeResources(apt.ResourceIds);
			DeserializeRecurrence(apt);
			DeserializeReminders(apt);
			DeserializeCustomFields(apt.CustomFields);
			TypedBinaryReaderEx typedReader = new TypedBinaryReaderEx(reader);
			AppointmentIdHelper.SetAppointmentId(apt, typedReader.ReadObject());
		}
		protected internal virtual void DeserializeResources(ResourceIdCollection resourceIds) {
			int count = reader.ReadInt32();
			if (count <= 0)
				return;
			TypedBinaryReaderEx typedReader = new TypedBinaryReaderEx(reader);
			for (int i = 0; i < count; i++)
				resourceIds.Add(typedReader.ReadObject());
		}
		protected internal virtual void DeserializeCustomFields(CustomFieldCollection fields) {
			int count = reader.ReadInt32();
			if (count <= 0)
				return;
			XtraSchedulerDebug.Assert(count == fields.Count);
			TypedBinaryReaderEx typedReader = new TypedBinaryReaderEx(reader);
			for (int i = 0; i < count; i++)
				fields[i] = typedReader.ReadObject();
		}
		protected internal virtual void DeserializeReminders(Appointment apt) {
			int count = reader.ReadInt32();
			if (count <= 0)
				return;
			for (int i = 0; i < count; i++) {
				Reminder reminder = apt.CreateNewReminder();
				reminder.TimeBeforeStart = TimeSpan.FromTicks(reader.ReadInt64());
				reminder.AlertTime = new DateTime(reader.ReadInt64());
				apt.Reminders.Add(reminder);
			}
		}
		protected internal virtual void DeserializeRecurrence(Appointment apt) {
			if (apt.Type == AppointmentType.Pattern) {
				DeserializeRecurrenceInfo(apt.RecurrenceInfo);
				patternTable.Add(apt, apt.RecurrenceInfo.Id);
			} else if (apt.IsException) {
				object patternId = DeserializeOccurrenceInfo(apt);
				patternTable.Add(apt, patternId);
			} else
				patternTable.Add(apt, Guid.Empty);
		}
		protected internal virtual void DeserializeRecurrenceInfo(IRecurrenceInfo recurrenceInfo) {
			recurrenceInfo.BeginUpdate();
			try {
				recurrenceInfo.DayNumber = (int)reader.ReadByte();
				recurrenceInfo.WeekOfMonth = (WeekOfMonth)reader.ReadByte();
				recurrenceInfo.Periodicity = reader.ReadInt32();
				recurrenceInfo.Month = reader.ReadByte();
				recurrenceInfo.OccurrenceCount = reader.ReadInt32();
				recurrenceInfo.Range = (RecurrenceRange)reader.ReadByte();
				recurrenceInfo.Type = (RecurrenceType)reader.ReadByte();
				recurrenceInfo.WeekDays = (WeekDays)reader.ReadByte();
				recurrenceInfo.AllDay = reader.ReadBoolean();
				recurrenceInfo.Start = new DateTime(reader.ReadInt64());
				recurrenceInfo.Duration = TimeSpan.FromTicks(reader.ReadInt64());
				int recurrenceInfoIdLength = reader.ReadInt32();
				((IIdProvider)recurrenceInfo).SetId(ObjectToByteArrayConverter.ByteArrayToObject(reader.ReadBytes(recurrenceInfoIdLength)));
			} finally {
				recurrenceInfo.EndUpdate();
			}
		}
		protected internal virtual object DeserializeOccurrenceInfo(Appointment apt) {
			int patternIdLength = reader.ReadInt32();
			object patternId = ObjectToByteArrayConverter.ByteArrayToObject(reader.ReadBytes(patternIdLength));
			((IInternalAppointment)apt).SetRecurrenceIndex(reader.ReadInt32());
			return patternId;
		}
	}
	public interface ISchedulerMappingFieldChecker {
		bool Contains(string fieldName);
		bool HasFields();
	}
}
