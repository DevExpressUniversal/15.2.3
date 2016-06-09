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
using DevExpress.XtraScheduler;
using DevExpress.Utils;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Xpf.Scheduler {
	public class SchedulerFrameworkElement : DXFrameworkElement
#if SL
		, ISupportInitialize
#endif
	{
#if SL
		void ISupportInitialize.BeginInit() {
			BeginInit();
		}
		void ISupportInitialize.EndInit() {
			EndInit();
		}
		public virtual void BeginInit() {
		}
		public virtual void EndInit() {
			OnInitialized(EventArgs.Empty);
		}
		protected virtual void OnInitialized(EventArgs e) {
		}
#endif
	}
	public abstract class PersistentObjectMapping<T, U> : SchedulerFrameworkElement
		where T : IPersistentObjectStorage<U>
		where U : IPersistentObject {
		readonly MappingInfoBase<U> innerMappingInfo;
		protected PersistentObjectMapping() {
			this.innerMappingInfo = CreateInnerMappingInfo();
			CreatePropertySyncManager();
		}
		protected PersistentObjectMapping(PersistentObjectStorageBase<T, U> dataStorage) {
			Guard.ArgumentNotNull(dataStorage, "dataStorage");
			this.innerMappingInfo = ObtainInnerMappingInfo(dataStorage);
			CreatePropertySyncManager();
		}
		protected abstract MappingInfoBase<U> CreateInnerMappingInfo();
		protected abstract MappingInfoBase<U> ObtainInnerMappingInfo(PersistentObjectStorageBase<T, U> dataStorage);
		protected internal MappingInfoBase<U> InnerMappingInfo { get { return innerMappingInfo; } }
		protected abstract void CreatePropertySyncManager();
	}
	public class AppointmentMapping : PersistentObjectMapping<IAppointmentStorageBase, Appointment> {
		public AppointmentMapping() {
		}
		public AppointmentMapping(AppointmentStorage dataStorage)
			: base(dataStorage) {
		}
		#region Start
		public string Start {
			get { return (string)GetValue(StartProperty); }
			set { SetValue(StartProperty, value); }
		}
		public static readonly DependencyProperty StartProperty = CreateStartProperty();
		static DependencyProperty CreateStartProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentMapping, string>("Start", string.Empty, (d, e) => d.OnStartChanged(e.OldValue, e.NewValue), null);
		}
		private void OnStartChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(StartProperty, null, newValue);
		}
		#endregion
		#region AppointmentId
		public string AppointmentId {
			get { return (string)GetValue(AppointmentIdProperty); }
			set { SetValue(AppointmentIdProperty, value); }
		}
		public static readonly DependencyProperty AppointmentIdProperty = CreateAppointmentIdProperty();
		static DependencyProperty CreateAppointmentIdProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentMapping, string>("AppointmentId", string.Empty, (d, e) => d.OnAppointmentIdChanged(e.OldValue, e.NewValue), null);
		}
		private void OnAppointmentIdChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(AppointmentIdProperty, null, newValue);
		}
		#endregion
		#region AllDay
		public string AllDay {
			get { return (string)GetValue(AllDayProperty); }
			set { SetValue(AllDayProperty, value); }
		}
		public static readonly DependencyProperty AllDayProperty = CreateAllDayProperty();
		static DependencyProperty CreateAllDayProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentMapping, string>("AllDay", string.Empty, (d, e) => d.OnAllDayChanged(e.OldValue, e.NewValue), null);
		}
		private void OnAllDayChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(AllDayProperty, null, newValue);
		}
		#endregion
		#region Description
		public string Description {
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		public static readonly DependencyProperty DescriptionProperty = CreateDescriptionProperty();
		static DependencyProperty CreateDescriptionProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentMapping, string>("Description", string.Empty, (d, e) => d.OnDescriptionChanged(e.OldValue, e.NewValue), null);
		}
		private void OnDescriptionChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(DescriptionProperty, null, newValue);
		}
		#endregion
		#region End
		public string End {
			get { return (string)GetValue(EndProperty); }
			set { SetValue(EndProperty, value); }
		}
		public static readonly DependencyProperty EndProperty = CreateEndProperty();
		static DependencyProperty CreateEndProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentMapping, string>("End", string.Empty, (d, e) => d.OnEndChanged(e.OldValue, e.NewValue), null);
		}
		private void OnEndChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(EndProperty, null, newValue);
		}
		#endregion
		#region Label
		public string Label {
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}
		public static readonly DependencyProperty LabelProperty = CreateLabelProperty();
		static DependencyProperty CreateLabelProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentMapping, string>("Label", string.Empty, (d, e) => d.OnLabelChanged(e.OldValue, e.NewValue), null);
		}
		private void OnLabelChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(LabelProperty, null, newValue);
		}
		#endregion
		#region Location
		public string Location {
			get { return (string)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		public static readonly DependencyProperty LocationProperty = CreateLocationProperty();
		static DependencyProperty CreateLocationProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentMapping, string>("Location", string.Empty, (d, e) => d.OnLocationChanged(e.OldValue, e.NewValue), null);
		}
		private void OnLocationChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(LocationProperty, null, newValue);
		}
		#endregion
		#region RecurrenceInfo
		public string RecurrenceInfo {
			get { return (string)GetValue(RecurrenceInfoProperty); }
			set { SetValue(RecurrenceInfoProperty, value); }
		}
		public static readonly DependencyProperty RecurrenceInfoProperty = CreateRecurrenceInfoProperty();
		static DependencyProperty CreateRecurrenceInfoProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentMapping, string>("RecurrenceInfo", string.Empty, (d, e) => d.OnRecurrenceInfoChanged(e.OldValue, e.NewValue), null);
		}
		private void OnRecurrenceInfoChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(RecurrenceInfoProperty, null, newValue);
		}
		#endregion
		#region ReminderInfo
		public string ReminderInfo {
			get { return (string)GetValue(ReminderInfoProperty); }
			set { SetValue(ReminderInfoProperty, value); }
		}
		public static readonly DependencyProperty ReminderInfoProperty = CreateReminderInfoProperty();
		static DependencyProperty CreateReminderInfoProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentMapping, string>("ReminderInfo", string.Empty, (d, e) => d.OnReminderInfoChanged(e.OldValue, e.NewValue), null);
		}
		private void OnReminderInfoChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(ReminderInfoProperty, null, newValue);
		}
		#endregion
		#region ResourceId
		public string ResourceId {
			get { return (string)GetValue(ResourceIdProperty); }
			set { SetValue(ResourceIdProperty, value); }
		}
		public static readonly DependencyProperty ResourceIdProperty = CreateResourceIdProperty();
		static DependencyProperty CreateResourceIdProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentMapping, string>("ResourceId", string.Empty, (d, e) => d.OnResourceIdChanged(e.OldValue, e.NewValue), null);
		}
		private void OnResourceIdChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(ResourceIdProperty, null, newValue);
		}
		#endregion
		#region Status
		public string Status {
			get { return (string)GetValue(StatusProperty); }
			set { SetValue(StatusProperty, value); }
		}
		public static readonly DependencyProperty StatusProperty = CreateStatusProperty();
		static DependencyProperty CreateStatusProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentMapping, string>("Status", string.Empty, (d, e) => d.OnStatusChanged(e.OldValue, e.NewValue), null);
		}
		private void OnStatusChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(StatusProperty, null, newValue);
		}
		#endregion
		#region Subject
		public string Subject {
			get { return (string)GetValue(SubjectProperty); }
			set { SetValue(SubjectProperty, value); }
		}
		public static readonly DependencyProperty SubjectProperty = CreateSubjectProperty();
		static DependencyProperty CreateSubjectProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentMapping, string>("Subject", string.Empty, (d, e) => d.OnSubjectChanged(e.OldValue, e.NewValue), null);
		}
		private void OnSubjectChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(SubjectProperty, null, newValue);
		}
		#endregion
		#region Type
		public string Type {
			get { return (string)GetValue(TypeProperty); }
			set { SetValue(TypeProperty, value); }
		}
		public static readonly DependencyProperty TypeProperty = CreateTypeProperty();
		static DependencyProperty CreateTypeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentMapping, string>("Type", string.Empty, (d, e) => d.OnTypeChanged(e.OldValue, e.NewValue), null);
		}
		private void OnTypeChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(TypeProperty, null, newValue);
		}
		#endregion
		#region TimeZoneId
		public string TimeZoneId {
			get { return (string)GetValue(TimeZoneIdProperty); }
			set { SetValue(TimeZoneIdProperty, value); }
		}
		public static readonly DependencyProperty TimeZoneIdProperty = CreateTimeZoneIdProperty();
		static DependencyProperty CreateTimeZoneIdProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentMapping, string>("TimeZoneId", string.Empty, (d, e) => d.OnTimeZoneIdChanged(e.OldValue, e.NewValue), null);
		}
		private void OnTimeZoneIdChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(TimeZoneIdProperty, null, newValue);
		}
		#endregion
		#region PropertySyncManager
		AppointmentMappingPropertySyncManager propertySyncManager;
		protected internal AppointmentMappingPropertySyncManager PropertySyncManager { get { return propertySyncManager; } }
		protected virtual AppointmentMappingPropertySyncManager CreateDependencyPropertySyncManager() {
			return new AppointmentMappingPropertySyncManager(this);
		}
		#endregion
		protected internal AppointmentMappingInfo InnerAppointmentMappings { get { return (AppointmentMappingInfo)base.InnerMappingInfo; } }
		protected override void CreatePropertySyncManager() {
			this.propertySyncManager = CreateDependencyPropertySyncManager();
			PropertySyncManager.Register();
		}
		protected override MappingInfoBase<Appointment> CreateInnerMappingInfo() {
			return new AppointmentMappingInfo();
		}
		protected override MappingInfoBase<Appointment> ObtainInnerMappingInfo(PersistentObjectStorageBase<IAppointmentStorageBase, Appointment> dataStorage) {
			XtraSchedulerDebug.Assert(dataStorage.InnerStorage != null);
			return dataStorage.InnerStorage.Mappings;
		}
	}
	public class ResourceMapping : PersistentObjectMapping<IResourceStorageBase, Resource> {
		public ResourceMapping() {
		}
		public ResourceMapping(ResourceStorage dataStorage)
			: base(dataStorage) {
		}
		#region Caption
		public string Caption {
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		public static readonly DependencyProperty CaptionProperty = CreateCaptionProperty();
		static DependencyProperty CreateCaptionProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceMapping, string>("Caption", string.Empty, (d, e) => d.OnCaptionChanged(e.OldValue, e.NewValue), null);
		}
		private void OnCaptionChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(CaptionProperty, null, newValue);
		}
		#endregion
		#region Color
		public string Color {
			get { return (string)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty = CreateColorProperty();
		static DependencyProperty CreateColorProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceMapping, string>("Color", string.Empty, (d, e) => d.OnColorChanged(e.OldValue, e.NewValue), null);
		}
		private void OnColorChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(ColorProperty, null, newValue);
		}
		#endregion
		#region Id
		public string Id {
			get { return (string)GetValue(IdProperty); }
			set { SetValue(IdProperty, value); }
		}
		public static readonly DependencyProperty IdProperty = CreateIdProperty();
		static DependencyProperty CreateIdProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceMapping, string>("Id", string.Empty, (d, e) => d.OnIdChanged(e.OldValue, e.NewValue), null);
		}
		private void OnIdChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(IdProperty, null, newValue);
		}
		#endregion
		#region Image
		public string Image {
			get { return (string)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}
		public static readonly DependencyProperty ImageProperty = CreateImageProperty();
		static DependencyProperty CreateImageProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceMapping, string>("Image", string.Empty, (d, e) => d.OnImageChanged(e.OldValue, e.NewValue), null);
		}
		private void OnImageChanged(string oldValue, string newValue) {
			PropertySyncManager.Update(ImageProperty, oldValue, newValue);
		}
		#endregion
		#region PropertySyncManager
		ResourceMappingPropertySyncManager propertySyncManager;
		protected internal ResourceMappingPropertySyncManager PropertySyncManager { get { return propertySyncManager; } }
		protected virtual ResourceMappingPropertySyncManager CreateDependencyPropertySyncManager() {
			return new ResourceMappingPropertySyncManager(this);
		}
		#endregion
		protected internal ResourceMappingInfo InnerResourceMappings { get { return (ResourceMappingInfo)base.InnerMappingInfo; } }
		protected override void CreatePropertySyncManager() {
			this.propertySyncManager = CreateDependencyPropertySyncManager();
			PropertySyncManager.Register();
		}
		protected override MappingInfoBase<Resource> CreateInnerMappingInfo() {
			return new ResourceMappingInfo();
		}
		protected override MappingInfoBase<Resource> ObtainInnerMappingInfo(PersistentObjectStorageBase<IResourceStorageBase, Resource> dataStorage) {
			XtraSchedulerDebug.Assert(dataStorage.InnerStorage != null);
			return dataStorage.InnerStorage.Mappings;
		}
	}
}
