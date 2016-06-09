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

using DevExpress.XtraScheduler;
using System.ComponentModel;
using DevExpress.XtraScheduler.Native;
using System.Collections;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using System.Windows.Media;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.Internal.Implementations;
using DevExpress.Xpf.Core;
#if SL
using PlatformIndependentColor = System.Windows.Media.Color;
using DevExpress.Xpf.Scheduler.WPFCompatibility;
#else
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal;
using System;
#endif
namespace DevExpress.Xpf.Scheduler {
	#region AppointmentStatus
	public class AppointmentStatus : UserInterfaceObjectWpf, IAppointmentStatus {
		private static AppointmentStatus empty = new AppointmentStatus();
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentStatusEmpty")]
#endif
		public static AppointmentStatus Empty {
			get { return empty; }
		}
		AppointmentStatusType type;
		Color color;
		protected internal AppointmentStatus(object id, Brush brush, AppointmentStatusType type, string displayName, string menuCaption)
			: base(id, displayName, menuCaption) {
			Type = type;
			Brush = brush;
		}
		protected internal AppointmentStatus(object id, Color color, AppointmentStatusType type, string displayName, string menuCaption)
			: base(id, displayName, menuCaption) {
			Type = type;
			Brush = new SolidColorBrush(color);
		}
		public AppointmentStatus()
			: base(null, String.Empty) {
			Brush = new SolidColorBrush(Colors.White);
			Type = AppointmentStatusType.Custom;
		}
		[Obsolete("Use AppointmentStatusCollection.CreateNewStatus instead", false)]
		public AppointmentStatus(AppointmentStatusType type, string displayName, string menuCaption)
			: base(null, displayName, menuCaption) {
			Type = type;
			Brush = new SolidColorBrush(Colors.White);
		}
		[Obsolete("Use AppointmentStatusCollection.CreateNewStatus instead", false)]
		public AppointmentStatus(AppointmentStatusType type, string displayName)
			: base(type, displayName) {
			Type = type;
			Brush = new SolidColorBrush(Colors.White);
		}
		[Obsolete("Use AppointmentStatusCollection.CreateNewStatus instead", false)]
		public AppointmentStatus(AppointmentStatusType type, Color color, string displayName)
			: base(type, displayName) {
			Type = type;
			Brush = new SolidColorBrush(color);
		}
		[Obsolete("Use AppointmentStatusCollection.CreateNewStatus instead", false)]
		public AppointmentStatus(AppointmentStatusType type, Color color, string displayName, string menuCaption)
			: base(null, displayName, menuCaption) {
			Type = type;
			Brush = new SolidColorBrush(color);
		}
		protected internal override void Assign(UserInterfaceObject source) {
			base.Assign(source);
			AppointmentStatus sourceStatus = source as AppointmentStatus;
			if (sourceStatus == null)
				return;
			this.Type = sourceStatus.Type;
		}
		[Browsable(false)]
		public AppointmentStatusType Type {
			get { return type; }
			set {
				if (type == value)
					return;
				AppointmentStatusType oldType = type;
				type = value;
				OnChanged("Type", oldType, value);
			}
		}
		public new Brush Brush {
			get { return base.Brush; }
			set {
				if (base.Brush == value)
					return;
				Brush oldBrush = base.Brush;
				base.Brush = value;
				OnChanged("Brush", oldBrush, value);
			}
		}
		[Obsolete("Use the Brush property instead")]
		System.Drawing.Color IAppointmentStatus.Color {
			get { return DXColorConverter.ToDrawingColor(Color); }
			set { Color = DXColorConverter.ToMediaColor(value); }
		}
		[Obsolete("Use the Brush property instead")]
		public Color Color {
			get { return this.color; }
			set {
				if (this.color == value)
					return;
				this.color = value;
				Brush = new SolidColorBrush(Color);
			}
		}
		public void Dispose() {
		}
	}
	#endregion
	#region AppointmentLabel
	public class AppointmentLabel : UserInterfaceObjectWpf, IAppointmentLabel {
		static AppointmentLabel empty = new AppointmentLabel();
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentLabelEmpty")]
#endif
		public static AppointmentLabel Empty { get { return empty; } }
		protected internal AppointmentLabel(object id, Color color, string displayName, string menuCaption)
			: base(id, displayName, menuCaption) {
			Brush = new SolidColorBrush(color);
		}
		protected internal AppointmentLabel(object id, Brush brush, string displayName, string menuCaption)
			: base(id, displayName, menuCaption) {
			Brush = brush;
		}
		public AppointmentLabel()
			: base(null, String.Empty) {
		}
		[Obsolete("Use CreateNewLabel instead.", false)]
		public AppointmentLabel(string displayName)
			: base(null, displayName) {
		}
		[Obsolete("Use CreateNewLabel instead.", false)]
		public AppointmentLabel(string displayName, string menuCaption)
			: base(null, displayName, menuCaption) {
		}
		[Obsolete("Use CreateNewLabel instead.", false)]
		public AppointmentLabel(Color color, string displayName)
			: base(null, displayName) {
			Color = color;
		}
		[Obsolete("Use CreateNewLabel instead.", false)]
		public AppointmentLabel(Color color, string displayName, string menuCaption)
			: base(null, displayName, menuCaption) {
			Color = color;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		int IAppointmentLabel.ColorValue { get; set; }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentLabelColor")]
#endif
		public Color Color {
			get { return ColorExtension.FromArgb(((IAppointmentLabel)this).ColorValue); }
			set {
				int newColorValue = value.ToArgb();
				if (((IAppointmentLabel)this).ColorValue == newColorValue)
					return;
				int oldColorValue = ((IAppointmentLabel)this).ColorValue;
				((IAppointmentLabel)this).ColorValue = newColorValue;
				OnChanged("Color", oldColorValue, newColorValue);
				Brush = new SolidColorBrush(value);
			}
		}
		public new object Id {
			get { return base.Id; }
			set {
				OnChanged("Id", Id, value);
				((IIdProvider)this).SetId(value);
			}
		}
		public void Dispose() {
		}
	}
	#endregion
	public static class AppointmentLabelExtension {
		public static Color GetColor(this IAppointmentLabel label) {
			AppointmentLabel labelObject = label as AppointmentLabel;
			if (labelObject != null)
				return labelObject.Color;
			return ColorExtension.FromArgb(label.ColorValue);
		}
		public static void SetColor(this IAppointmentLabel label, Color color) {
			AppointmentLabel labelObject = label as AppointmentLabel;
			if (labelObject != null)
				labelObject.Color = color;
			else
				label.ColorValue = color.ToArgb();
		}
	}
	public static class AppointmentStatusExtension {
		public static Brush GetBrush(this IAppointmentStatus status) {
			AppointmentStatus statusObject = status as AppointmentStatus;
			if (statusObject == null)
				return null;
			return statusObject.Brush;
		}
		public static void SetBrush(this IAppointmentStatus status, Brush brush) {
			AppointmentStatus statusObject = status as AppointmentStatus;
			if (statusObject != null)
				statusObject.Brush = brush;
		}
	}
	public interface ISchedulerStorage : ISchedulerStorageBase {
		new IAppointmentStorage Appointments { get; }
		Color GetLabelColor(object labelId);
	}
	public interface IAppointmentStorage : IAppointmentStorageBase {
		new AppointmentLabelCollection Labels { get; }
		new AppointmentStatusCollection Statuses { get; }
	}
}
namespace DevExpress.Xpf.Scheduler.Native {
	public abstract class UserInterfaceObjectWpf : UserInterfaceObject {
		protected UserInterfaceObjectWpf(object id, string displayName)
			: base(id, displayName) {
		}
		protected UserInterfaceObjectWpf(object id, string displayName, string menuCaption)
			: base(id, displayName, menuCaption) {
		}
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Brush Brush { get; set; }
		protected internal override void Assign(UserInterfaceObject source) {
			base.Assign(source);
			UserInterfaceObjectWpf sourceObject = source as UserInterfaceObjectWpf;
			if (sourceObject == null)
				return;
			this.Brush = sourceObject.Brush.Clone();
		}
	}
	public class SchedulerDataStorage : SchedulerStorageBase, ISchedulerStorage {
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SchedulerDataStorage() {
		}
		#region Properties
		[ Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public IAppointmentStorage Appointments {
			get { return (IAppointmentStorage)InnerAppointments; }
			set {
				if (InnerAppointments == value)
					return;
				IInternalAppointmentStorage internalInnerAppointments = (IInternalAppointmentStorage)InnerAppointments;
				if (internalInnerAppointments.IsLoading) {
					internalInnerAppointments.EndInit();
					((IInternalAppointmentStorage)value).BeginInit();
				}
				InnerAppointments = value;
				OnInnerAppointmentsChanged();
			}
		}
		[
Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public IResourceStorageBase Resources {
			get { return (IResourceStorageBase)InnerResources; }
			set {
				if (InnerResources == value)
					return;
				IInternalResourceStorage internalInnerResources = (IInternalResourceStorage)InnerResources;
				if (internalInnerResources.IsLoading) {
					internalInnerResources.EndInit();
					((IInternalResourceStorage)value).BeginInit();
				}
				InnerResources = value;
				OnInnerResourcesChanged();
			}
		}
		[
Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppointmentDependencyDataStorage AppointmentDependencies {
			get { return (AppointmentDependencyDataStorage)InnerAppointmentDependencies; }
			set {
				if (InnerAppointmentDependencies == value)
					return;
				InnerAppointmentDependencies = value;
				OnInnerAppointmentDependenciesChanged();
			}
		}
		#endregion
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void SetAppointmentStorage(IAppointmentStorageBase appointmentStorage) {
			Appointments = (IAppointmentStorage)appointmentStorage;
		}
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void SetResourceStorage(IResourceStorageBase resourceStorage) {
			Resources = resourceStorage;
		}
		public Color GetLabelColor(object labelId) {
			return Appointments.Labels.GetById(labelId).Color;
		}
		protected internal override AppointmentStorageBase CreateAppointmentStorage() {
			return new AppointmentDataStorage(this);
		}
		protected internal override ResourceStorageBase CreateResourceStorage() {
			return new ResourceDataStorage(this);
		}
		protected internal override AppointmentDependencyStorageBase CreateAppointmentDependencyStorage() {
			return new AppointmentDependencyDataStorage(this);
		}
		protected internal virtual void OnInnerAppointmentsChanged() {
			((IInternalAppointmentStorage)InnerAppointments).AttachStorage(this);
		}
		protected internal virtual void OnInnerResourcesChanged() {
			((IInternalResourceStorage)InnerResources).AttachStorage(this);
		}
		protected internal virtual void OnInnerAppointmentDependenciesChanged() {
			((IInternalAppointmentDependencyStorage)InnerAppointmentDependencies).AttachStorage(this);
		}
	}
	public class AppointmentDataStorage : AppointmentStorageBase, IAppointmentStorage, IXtraSupportShouldSerialize, IXtraSupportDeserializeCollectionItem {
		XtraSupportShouldSerializeHelper shouldSerializeHelper = new XtraSupportShouldSerializeHelper();
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AppointmentDataStorage() {
			Initialize();
		}
		public AppointmentDataStorage(SchedulerDataStorage storage)
			: base(storage) {
			Initialize();
		}
		private void Initialize() {
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("Statuses", XtraShouldSerializeStatuses);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("Labels", XtraShouldSerializeLabels);
		}
		#region Properties
		[ Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new AppointmentMappingInfo Mappings {
			get { return base.Mappings; }
			set { base.Mappings = value; }
		}
		[ Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatDisable()]
		public new AppointmentCustomFieldMappingCollection CustomFieldMappings {
			get { return (AppointmentCustomFieldMappingCollection)base.CustomFieldMappings; }
			set { base.CustomFieldMappings = value; }
		}
#if SL
		protected override bool IsFastPropertiesSupport { get { return !DesignerProperties.IsInDesignTool; } }
#endif
		#region Statuses
		[ Category(SRCategoryNames.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatDisable(),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public AppointmentStatusCollection Statuses {
			get { return (AppointmentStatusCollection)InnerStatuses; }
			set { InnerStatuses = value; }
		}
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
			return new AppointmentStatus();
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
Category(SRCategoryNames.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatDisable(),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public AppointmentLabelCollection Labels {
			get { return (AppointmentLabelCollection)InnerLabels; }
			set { InnerLabels = value; }
		}
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
			return new AppointmentLabel();
		}
		internal void XtraSetIndexLabelsItem(XtraSetItemIndexEventArgs e) {
			AppointmentLabel label = e.Item.Value as AppointmentLabel;
			if (label == null)
				return;
			InnerLabels.Add(label);
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
#if SL
		protected override XtraScheduler.Data.ISchedulerUnboundDataKeeper CreateUnboundDataKeeper() {
			return new AppointmentUnboundDataKeeper();
		}
		#region AppointmentsTransaction base support
		protected override bool SupportsAppointmentTransaction { get { return true; } }
		public virtual void StartAppointmentsTransaction() {
			InternalStartAppointmentsTransaction();
		}
		public virtual  void CommitAppointmentsTransaction(System.Collections.Generic.IList<Appointment> appointments) {
			InternalCommitAppointmentsTransaction(appointments, AppointmentsTransactionType.Unknown);
		}
		public virtual void CancelAppointmentsTransaction() {
			InternalCancelAppointmentsTransaction();
		}
		#endregion
		#region RIA Services Support
		AsyncLoadingObjectStorageHelper<Appointment> asyncLoadingObjectStorageHelper;
		protected AsyncLoadingObjectStorageHelper<Appointment> AsyncLoadingObjectStorageHelper {
			get {
				if (asyncLoadingObjectStorageHelper == null)
					asyncLoadingObjectStorageHelper = new AsyncLoadingObjectStorageHelper<Appointment>(this);
				return asyncLoadingObjectStorageHelper;
			}
		}
		protected internal override XtraScheduler.Data.DataManager<Appointment> CreateDataManager() {
			return new RIASupportedAppointmentDataManager();
		}
		protected override void InitializeDataManager() {
			base.InitializeDataManager();
			AsyncLoadingObjectStorageHelper.AttachAsyncObjectLoadingSupport(DataManager as IAsyncObjectLoadingSupportOwner);
		}
		#endregion
#endif
		protected override IAppointmentStatusStorage CreateAppointmentStatusCollection() {
			return new AppointmentStatusCollection();
		}
		protected override IAppointmentLabelStorage CreateAppointmentLabelCollection() {
			return new AppointmentLabelCollection();
		}
		protected override IList ObtainIListFromDataSource(object dataSource, string dataMember) {
			if (dataSource == null)
				return null;
			IList result = DataBindingHelper.ExtractDataSource(dataSource, new WrappedICollectionViewListExtractionAlgorithm());
			if (result != null) {
				return result;
			}
			return base.ObtainIListFromDataSource(dataSource, dataMember);
		}
	}
	public class ResourceDataStorage : ResourceStorageBase {
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ResourceDataStorage() {
		}
		public ResourceDataStorage(SchedulerDataStorage storage)
			: base(storage) {
		}
		#region Properties
		[ Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ResourceMappingInfo Mappings { get { return InnerMappings; } set { InnerMappings = value; } }
		[ Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ResourceCustomFieldMappingCollection CustomFieldMappings {
			get { return (ResourceCustomFieldMappingCollection)base.CustomFieldMappings; }
			set { base.CustomFieldMappings = value; }
		}
#if SL
		protected override bool IsFastPropertiesSupport { get { return !DesignerProperties.IsInDesignTool; } }
#endif
		#endregion
#if SL
		protected override XtraScheduler.Data.ISchedulerUnboundDataKeeper CreateUnboundDataKeeper() {
			return new ResourceUnboundDataKeeper();
		}
		#region RIA Services Support
		AsyncLoadingObjectStorageHelper<Resource> asyncLoadingObjectStorageHelper;
		protected AsyncLoadingObjectStorageHelper<Resource> AsyncLoadingObjectStorageHelper {
			get {
				if (asyncLoadingObjectStorageHelper == null)
					asyncLoadingObjectStorageHelper = new AsyncLoadingObjectStorageHelper<Resource>(this);
				return asyncLoadingObjectStorageHelper;
			}
		}
		protected internal override XtraScheduler.Data.DataManager<Resource> CreateDataManager() {
			return new RIASupportedResourceDataManager();
		}
		protected override void InitializeDataManager() {
			base.InitializeDataManager();
			AsyncLoadingObjectStorageHelper.AttachAsyncObjectLoadingSupport(DataManager as IAsyncObjectLoadingSupportOwner);
		}
		#endregion
#endif
		protected override IList ObtainIListFromDataSource(object dataSource, string dataMember) {
			IList result = DevExpress.Xpf.Core.Native.DataBindingHelper.ExtractDataSource(dataSource, new WrappedICollectionViewListExtractionAlgorithm());
			if (result != null)
				return result;
			return base.ObtainIListFromDataSource(dataSource, dataMember);
		}
		protected override IResourceFactory CreateResourceFactory() {
			return new WpfResourceFactory();
		}
	}
	public class WpfResourceFactory : IResourceFactory {
		public Resource CreateResource() {
			return new ResourceInstance();
		}
	}
	public class AppointmentDependencyDataStorage : AppointmentDependencyStorageBase {
		public AppointmentDependencyDataStorage()
			: base() {
		}
		public AppointmentDependencyDataStorage(SchedulerDataStorage storage)
			: base(storage) {
		}
		#region Properties
		[
Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new AppointmentDependencyMappingInfo Mappings { get { return InnerMappings; } set { InnerMappings = value; } }
		[
Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new AppointmentDependencyCustomFieldMappingCollection CustomFieldMappings {
			get { return (AppointmentDependencyCustomFieldMappingCollection)base.CustomFieldMappings; }
			set { CustomFieldMappings = value; }
		}
		#endregion
	}
}
