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
using DevExpress.XtraScheduler;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Controls;
using System.Collections.Generic;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Xpf.Core;
using DevExpress.XtraScheduler.Design;
#if SL
using DXFrameworkContentElement = DevExpress.Xpf.Scheduler.WPFCompatibility.SLFrameworkContentElement;
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Scheduler {
	#region SchedulerElement
#if !SL
	public class SchedulerElement : DXFrameworkContentElement {
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object Tag {
			get { return base.Tag; }
			set { base.Tag = value; }
		}
#else
	public class SchedulerElement : Control {
#endif
	}
	#endregion
	#region SchedulerOptionsBase<T> (abstract class)
#if !SL
	public abstract class SchedulerOptionsBase<T> : Freezable where T : BaseOptions {
#else
	public abstract class SchedulerOptionsBase<T> : DependencyObject where T : BaseOptions {
#endif
		DependencyPropertySyncManager propertySyncManager;
		T innerObject;
		protected internal T InnerObject { get { return innerObject; } }
		protected SchedulerOptionsBase() {
			CreatePropertySyncManager();
		}
		protected virtual bool CanSyncInnerObject() {
			return InnerObject != null;
		}
		protected virtual void CreatePropertySyncManager() {
			this.propertySyncManager = CreateDependencyPropertySyncManager();
			PropertySyncManager.Register();
		}
		#region PropertyManager
		protected internal DependencyPropertySyncManager PropertySyncManager { get { return propertySyncManager; } }
		#endregion
		protected abstract DependencyPropertySyncManager CreateDependencyPropertySyncManager();
		protected internal virtual void UpdateInnerObjectPropertyValue(DependencyProperty property, object oldValue, object newValue) {
			if (!CanSyncInnerObject())
				PropertySyncManager.StartDeferredChanges();
			PropertySyncManager.Update(property, oldValue, newValue);
		}
		protected internal virtual void SynchronizeInnerObjectProperties() {
			PropertySyncManager.Deactivate();
			PropertySyncManager.Activate();
			if (PropertySyncManager.IsDeferredChanges)
				PropertySyncManager.CommitDeferredChanges();
		}
		protected internal virtual void SetupInnerObjectPropertyBinding(DependencyProperty property) {
			Binding binding = InnerBindingHelper.CreateTwoWayPropertyBinding(InnerObject, GetPropertyName(property));
			BindingOperations.SetBinding(this, property, binding);
		}
		protected internal virtual T DetachExistingInnerObject() {
			T result = InnerObject;
			PropertySyncManager.Deactivate();
			this.innerObject = null;
			return result;
		}
		protected internal virtual void AttachExistingInnerObject(T innerObject) {
			Guard.ArgumentNotNull(innerObject, "innerObject");
			this.innerObject = innerObject;
			SynchronizeInnerObjectProperties();
		}
#if !SL
		protected override bool FreezeCore(bool isChecking) {
			if (isChecking)
				return false;
			return base.FreezeCore(isChecking);
		}
		protected override void CloneCore(Freezable sourceFreezable) {
			if (sourceFreezable.GetType() != this.GetType())
				return;
			base.CloneCore(sourceFreezable);
		}
#endif
		string GetPropertyName(DependencyProperty property) {
#if (SL)
			return property.GetName();
#else
			return property.Name;
#endif
		}
	}
	#endregion
	#region OptionsCustomization
	public class OptionsCustomization : SchedulerOptionsBase<SchedulerOptionsCustomization> {
		static OptionsCustomization() {
		}
		#region Properties
		#region AllowAppointmentCopy
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsCustomizationAllowAppointmentCopy")]
#endif
		public UsedAppointmentType AllowAppointmentCopy {
			get { return (UsedAppointmentType)GetValue(AllowAppointmentCopyProperty); }
			set { SetValue(AllowAppointmentCopyProperty, value); }
		}
		public static readonly DependencyProperty AllowAppointmentCopyProperty = CreateAllowAppointmentCopyProperty();
		static DependencyProperty CreateAllowAppointmentCopyProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsCustomization, UsedAppointmentType>("AllowAppointmentCopy", UsedAppointmentType.All, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnAllowAppointmentCopyChanged(e.OldValue, e.NewValue));
		}
		private void OnAllowAppointmentCopyChanged(UsedAppointmentType oldValue, UsedAppointmentType newValue) {
			UpdateInnerObjectPropertyValue(AllowAppointmentCopyProperty, oldValue, newValue);
		}
		#endregion
		#region AllowAppointmentDrag
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsCustomizationAllowAppointmentDrag")]
#endif
		public UsedAppointmentType AllowAppointmentDrag {
			get { return (UsedAppointmentType)GetValue(AllowAppointmentDragProperty); }
			set { SetValue(AllowAppointmentDragProperty, value); }
		}
		public static readonly DependencyProperty AllowAppointmentDragProperty = CreateAllowAppointmentDragProperty();
		static DependencyProperty CreateAllowAppointmentDragProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsCustomization, UsedAppointmentType>("AllowAppointmentDrag", UsedAppointmentType.All, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnAllowAppointmentDragChanged(e.OldValue, e.NewValue));
		}
		private void OnAllowAppointmentDragChanged(UsedAppointmentType oldValue, UsedAppointmentType newValue) {
			UpdateInnerObjectPropertyValue(AllowAppointmentDragProperty, oldValue, newValue);
		}
		#endregion
		#region AllowAppointmentResize
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsCustomizationAllowAppointmentResize")]
#endif
		public UsedAppointmentType AllowAppointmentResize {
			get { return (UsedAppointmentType)GetValue(AllowAppointmentResizeProperty); }
			set { SetValue(AllowAppointmentResizeProperty, value); }
		}
		public static readonly DependencyProperty AllowAppointmentResizeProperty = CreateAllowAppointmentResizeProperty();
		static DependencyProperty CreateAllowAppointmentResizeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsCustomization, UsedAppointmentType>("AllowAppointmentResize", UsedAppointmentType.All, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnAllowAppointmentResizeChanged(e.OldValue, e.NewValue));
		}
		private void OnAllowAppointmentResizeChanged(UsedAppointmentType oldValue, UsedAppointmentType newValue) {
			UpdateInnerObjectPropertyValue(AllowAppointmentResizeProperty, oldValue, newValue);
		}
		#endregion
		#region AllowAppointmentDelete
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsCustomizationAllowAppointmentDelete")]
#endif
		public UsedAppointmentType AllowAppointmentDelete {
			get { return (UsedAppointmentType)GetValue(AllowAppointmentDeleteProperty); }
			set { SetValue(AllowAppointmentDeleteProperty, value); }
		}
		public static readonly DependencyProperty AllowAppointmentDeleteProperty = CreateAllowAppointmentDeleteProperty();
		static DependencyProperty CreateAllowAppointmentDeleteProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsCustomization, UsedAppointmentType>("AllowAppointmentDelete", UsedAppointmentType.All, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnAllowAppointmentDeleteChanged(e.OldValue, e.NewValue));
		}
		private void OnAllowAppointmentDeleteChanged(UsedAppointmentType oldValue, UsedAppointmentType newValue) {
			UpdateInnerObjectPropertyValue(AllowAppointmentDeleteProperty, oldValue, newValue);
		}
		#endregion
		#region AllowAppointmentCreate
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsCustomizationAllowAppointmentCreate")]
#endif
		public UsedAppointmentType AllowAppointmentCreate {
			get { return (UsedAppointmentType)GetValue(AllowAppointmentCreateProperty); }
			set { SetValue(AllowAppointmentCreateProperty, value); }
		}
		public static readonly DependencyProperty AllowAppointmentCreateProperty = CreateAllowAppointmentCreateProperty();
		static DependencyProperty CreateAllowAppointmentCreateProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsCustomization, UsedAppointmentType>("AllowAppointmentCreate", UsedAppointmentType.All, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnAllowAppointmentCreateChanged(e.OldValue, e.NewValue));
		}
		private void OnAllowAppointmentCreateChanged(UsedAppointmentType oldValue, UsedAppointmentType newValue) {
			UpdateInnerObjectPropertyValue(AllowAppointmentCreateProperty, oldValue, newValue);
		}
		#endregion
		#region AllowAppointmentEdit
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsCustomizationAllowAppointmentEdit")]
#endif
		public UsedAppointmentType AllowAppointmentEdit {
			get { return (UsedAppointmentType)GetValue(AllowAppointmentEditProperty); }
			set { SetValue(AllowAppointmentEditProperty, value); }
		}
		public static readonly DependencyProperty AllowAppointmentEditProperty = CreateAllowAppointmentEditProperty();
		static DependencyProperty CreateAllowAppointmentEditProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsCustomization, UsedAppointmentType>("AllowAppointmentEdit", UsedAppointmentType.All, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnAllowAppointmentEditChanged(e.OldValue, e.NewValue));
		}
		private void OnAllowAppointmentEditChanged(UsedAppointmentType oldValue, UsedAppointmentType newValue) {
			UpdateInnerObjectPropertyValue(AllowAppointmentEditProperty, oldValue, newValue);
		}
		#endregion
		#region AllowAppointmentDragBetweenResources
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsCustomizationAllowAppointmentDragBetweenResources")]
#endif
		public UsedAppointmentType AllowAppointmentDragBetweenResources {
			get { return (UsedAppointmentType)GetValue(AllowAppointmentDragBetweenResourcesProperty); }
			set { SetValue(AllowAppointmentDragBetweenResourcesProperty, value); }
		}
		public static readonly DependencyProperty AllowAppointmentDragBetweenResourcesProperty = CreateAllowAppointmentDragBetweenResourcesProperty();
		static DependencyProperty CreateAllowAppointmentDragBetweenResourcesProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsCustomization, UsedAppointmentType>("AllowAppointmentDragBetweenResources", UsedAppointmentType.All, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnAllowAppointmentDragBetweenResourcesChanged(e.OldValue, e.NewValue));
		}
		private void OnAllowAppointmentDragBetweenResourcesChanged(UsedAppointmentType oldValue, UsedAppointmentType newValue) {
			UpdateInnerObjectPropertyValue(AllowAppointmentDragBetweenResourcesProperty, oldValue, newValue);
		}
		#endregion
		#region AllowInplaceEditor
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsCustomizationAllowInplaceEditor")]
#endif
		public UsedAppointmentType AllowInplaceEditor {
			get { return (UsedAppointmentType)GetValue(AllowInplaceEditorProperty); }
			set { SetValue(AllowInplaceEditorProperty, value); }
		}
		public static readonly DependencyProperty AllowInplaceEditorProperty = CreateAllowInplaceEditorProperty();
		static DependencyProperty CreateAllowInplaceEditorProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsCustomization, UsedAppointmentType>("AllowInplaceEditor", UsedAppointmentType.All, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnAllowInplaceEditorChanged(e.OldValue, e.NewValue));
		}
		private void OnAllowInplaceEditorChanged(UsedAppointmentType oldValue, UsedAppointmentType newValue) {
			UpdateInnerObjectPropertyValue(AllowInplaceEditorProperty, oldValue, newValue);
		}
		#endregion
		#region AllowAppointmentMultiSelect
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsCustomizationAllowAppointmentMultiSelect")]
#endif
		public bool AllowAppointmentMultiSelect {
			get { return (bool)GetValue(AllowAppointmentMultiSelectProperty); }
			set { SetValue(AllowAppointmentMultiSelectProperty, value); }
		}
		public static readonly DependencyProperty AllowAppointmentMultiSelectProperty = CreateAllowAppointmentMultiSelectProperty();
		static DependencyProperty CreateAllowAppointmentMultiSelectProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsCustomization, bool>("AllowAppointmentMultiSelect", true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnAllowAppointmentMultiSelectChanged(e.OldValue, e.NewValue));
		}
		private void OnAllowAppointmentMultiSelectChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(AllowAppointmentMultiSelectProperty, oldValue, newValue);
		}
		#endregion
		#region AllowAppointmentConflicts
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsCustomizationAllowAppointmentConflicts")]
#endif
		public AppointmentConflictsMode AllowAppointmentConflicts {
			get { return (AppointmentConflictsMode)GetValue(AllowAppointmentConflictsProperty); }
			set { SetValue(AllowAppointmentConflictsProperty, value); }
		}
		public static readonly DependencyProperty AllowAppointmentConflictsProperty = CreateAllowAppointmentConflictsProperty();
		static DependencyProperty CreateAllowAppointmentConflictsProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsCustomization, AppointmentConflictsMode>("AllowAppointmentConflicts", AppointmentConflictsMode.Allowed, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnAllowAppointmentConflictsChanged(e.OldValue, e.NewValue));
		}
		private void OnAllowAppointmentConflictsChanged(AppointmentConflictsMode oldValue, AppointmentConflictsMode newValue) {
			UpdateInnerObjectPropertyValue(AllowAppointmentConflictsProperty, oldValue, newValue);
		}
		#endregion
		#region AllowDisplayAppointmentForm
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsCustomizationAllowDisplayAppointmentForm")]
#endif
		public AllowDisplayAppointmentForm AllowDisplayAppointmentForm {
			get { return (AllowDisplayAppointmentForm)GetValue(AllowDisplayAppointmentFormProperty); }
			set { SetValue(AllowDisplayAppointmentFormProperty, value); }
		}
		public static readonly DependencyProperty AllowDisplayAppointmentFormProperty = CreateAllowDisplayAppointmentFormProperty();
		static DependencyProperty CreateAllowDisplayAppointmentFormProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsCustomization, AllowDisplayAppointmentForm>("AllowDisplayAppointmentForm", AllowDisplayAppointmentForm.Auto, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnAllowDisplayAppointmentFormChanged(e.OldValue, e.NewValue));
		}
		private void OnAllowDisplayAppointmentFormChanged(AllowDisplayAppointmentForm oldValue, AllowDisplayAppointmentForm newValue) {
			UpdateInnerObjectPropertyValue(AllowDisplayAppointmentFormProperty, oldValue, newValue);
		}
		#endregion
		#endregion
		protected override DependencyPropertySyncManager CreateDependencyPropertySyncManager() {
			return new OptionsCustomizationPropertySyncManager(this);
		}
#if !SL
		protected override Freezable CreateInstanceCore() {
			return new OptionsCustomization();
		}
#endif
	}
	#endregion
	#region OptionsView
	public class OptionsView : SchedulerOptionsBase<SchedulerOptionsViewBase> {
		static OptionsView() {
			NavigationButtonOptionsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsView, NavigationButtonOptions>("NavigationButtonOptions", null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnNavigationButtonOptionsChanged(e.OldValue, e.NewValue), null);
		}
		#region Properties
		#region ShowOnlyResourceAppointments
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsViewShowOnlyResourceAppointments")]
#endif
		public bool ShowOnlyResourceAppointments {
			get { return (bool)GetValue(ShowOnlyResourceAppointmentsProperty); }
			set { SetValue(ShowOnlyResourceAppointmentsProperty, value); }
		}
		public static readonly DependencyProperty ShowOnlyResourceAppointmentsProperty = CreateShowOnlyResourceAppointmentsProperty();
		static DependencyProperty CreateShowOnlyResourceAppointmentsProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsView, bool>("ShowOnlyResourceAppointments", false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnShowOnlyResourceAppointmentsChanged(e.OldValue, e.NewValue));
		}
		private void OnShowOnlyResourceAppointmentsChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(ShowOnlyResourceAppointmentsProperty, oldValue, newValue);
		}
		#endregion
		#region FirstDayOfWeek
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsViewFirstDayOfWeek")]
#endif
		public FirstDayOfWeek FirstDayOfWeek {
			get { return (FirstDayOfWeek)GetValue(FirstDayOfWeekProperty); }
			set { SetValue(FirstDayOfWeekProperty, value); }
		}
		public static readonly DependencyProperty FirstDayOfWeekProperty = CreateFirstDayOfWeekProperty();
		static DependencyProperty CreateFirstDayOfWeekProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsView, FirstDayOfWeek>("FirstDayOfWeek", FirstDayOfWeek.System, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnFirstDayOfWeekChanged(e.OldValue, e.NewValue));
		}
		private void OnFirstDayOfWeekChanged(FirstDayOfWeek oldValue, FirstDayOfWeek newValue) {
			UpdateInnerObjectPropertyValue(FirstDayOfWeekProperty, oldValue, newValue);
		}
		#endregion
		#region NavigationButtonOptions
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsViewNavigationButtonOptions")]
#endif
		public NavigationButtonOptions NavigationButtonOptions {
			get { return (NavigationButtonOptions)GetValue(NavigationButtonOptionsProperty); }
			set { SetValue(NavigationButtonOptionsProperty, value); }
		}
		public static readonly DependencyProperty NavigationButtonOptionsProperty;
		static DependencyProperty CreateNavigationButtonOptionsProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsView, NavigationButtonOptions>("NavigationButtonOptions", null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnNavigationButtonOptionsChanged(e.OldValue, e.NewValue), null);
		}
		private void OnNavigationButtonOptionsChanged(NavigationButtonOptions oldValue, NavigationButtonOptions newValue) {
			if (InnerObject != null)
				OnOptionsAssigned(oldValue, newValue, InnerObject.NavigationButtons);
		}
		#endregion
		#endregion
		protected override DependencyPropertySyncManager CreateDependencyPropertySyncManager() {
			return new OptionsViewPropertySyncManager(this);
		}
#if !SL
		protected override Freezable CreateInstanceCore() {
			return new OptionsView();
		}
#endif
		protected internal virtual void OnOptionsAssigned<T>(SchedulerOptionsBase<T> oldValue, SchedulerOptionsBase<T> newValue, T innerOptionsObject) where T : BaseOptions {
			if (oldValue == newValue)
				return;
			if (oldValue != null) {
				oldValue.DetachExistingInnerObject();
			}
			if (newValue != null) {
				newValue.AttachExistingInnerObject(innerOptionsObject);
			}
		}
		protected internal override void AttachExistingInnerObject(SchedulerOptionsViewBase innerObject) {
			Guard.ArgumentNotNull(innerObject, "innerObject");
			OnOptionsAssigned(null, NavigationButtonOptions, innerObject.NavigationButtons);
			base.AttachExistingInnerObject(innerObject);
		}
	}
	#endregion
	public class NavigationButtonOptions : SchedulerOptionsBase<SchedulerNavigationButtonOptions> {
		internal static readonly TimeSpan defaultAppointmentSearchInterval = TimeSpan.FromDays(2 * 365);
		#region Properties
		#region Visibility
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("NavigationButtonOptionsVisibility")]
#endif
		public NavigationButtonVisibility Visibility {
			get { return (NavigationButtonVisibility)GetValue(VisibilityProperty); }
			set { SetValue(VisibilityProperty, value); }
		}
		public static readonly DependencyProperty VisibilityProperty = CreateVisibilityProperty();
		static DependencyProperty CreateVisibilityProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<NavigationButtonOptions, NavigationButtonVisibility>("Visibility", NavigationButtonVisibility.Auto, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnVisibilityChanged(e.OldValue, e.NewValue));
		}
		private void OnVisibilityChanged(NavigationButtonVisibility oldValue, NavigationButtonVisibility newValue) {
			UpdateInnerObjectPropertyValue(VisibilityProperty, oldValue, newValue);
		}
		#endregion
		#region AppointmentSearchInterval
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("NavigationButtonOptionsAppointmentSearchInterval")]
#endif
		public TimeSpan AppointmentSearchInterval {
			get { return (TimeSpan)GetValue(AppointmentSearchIntervalProperty); }
			set { SetValue(AppointmentSearchIntervalProperty, value); }
		}
		public static readonly DependencyProperty AppointmentSearchIntervalProperty = CreateAppointmentSearchIntervalProperty();
		static DependencyProperty CreateAppointmentSearchIntervalProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<NavigationButtonOptions, TimeSpan>("AppointmentSearchInterval", defaultAppointmentSearchInterval, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnAppointmentSearchIntervalChanged(e.OldValue, e.NewValue));
		}
		private void OnAppointmentSearchIntervalChanged(TimeSpan oldValue, TimeSpan newValue) {
			UpdateInnerObjectPropertyValue(AppointmentSearchIntervalProperty, oldValue, newValue);
		}
		#endregion
		#endregion
		protected override DependencyPropertySyncManager CreateDependencyPropertySyncManager() {
			return new NavigationButtonOptionsPropertySyncManager(this);
		}
#if !SL
		protected override Freezable CreateInstanceCore() {
			return new NavigationButtonOptions();
		}
#endif
	}
	public class SchedulerSelectionBarOptions : SchedulerOptionsBase<SelectionBarOptions> {
		#region Visible
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerSelectionBarOptionsVisible")]
#endif
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		public static readonly DependencyProperty VisibleProperty = CreateVisibleProperty();
		static DependencyProperty CreateVisibleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerSelectionBarOptions, bool>("Visible", true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnVisibleChanged(e.OldValue, e.NewValue));
		}
		private void OnVisibleChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(VisibleProperty, oldValue, newValue);
		}
		#endregion
		#region Height
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerSelectionBarOptionsHeight")]
#endif
		public int Height {
			get { return (int)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		public static readonly DependencyProperty HeightProperty = CreateHeightProperty();
		static DependencyProperty CreateHeightProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerSelectionBarOptions, int>("Height", 0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnHeightChanged(e.OldValue, e.NewValue));
		}
		private void OnHeightChanged(int oldValue, int newValue) {
			UpdateInnerObjectPropertyValue(HeightProperty, oldValue, newValue);
		}
		#endregion
		protected override DependencyPropertySyncManager CreateDependencyPropertySyncManager() {
			return new SchedulerSelectionBarOptionsPropertySyncManager(this);
		}
#if !SL
		protected override Freezable CreateInstanceCore() {
			return new SchedulerSelectionBarOptions();
		}
#endif
	}
	public class SchedulerAppointmentDisplayOptions : SchedulerOptionsBase<AppointmentDisplayOptions> {
		#region Properties
		#region StatusDisplayType
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerAppointmentDisplayOptionsStatusDisplayType")]
#endif
		public AppointmentStatusDisplayType StatusDisplayType {
			get { return (AppointmentStatusDisplayType)GetValue(StatusDisplayTypeProperty); }
			set { SetValue(StatusDisplayTypeProperty, value); }
		}
		public static readonly DependencyProperty StatusDisplayTypeProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerAppointmentDisplayOptions, AppointmentStatusDisplayType>("StatusDisplayType", AppointmentStatusDisplayType.Never, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnStatusDisplayTypeChanged(e.OldValue, e.NewValue), null);
		protected virtual void OnStatusDisplayTypeChanged(AppointmentStatusDisplayType oldValue, AppointmentStatusDisplayType newValue) {
			UpdateInnerObjectPropertyValue(StatusDisplayTypeProperty, oldValue, newValue);
		}
		#endregion
		#region StartTimeVisibility
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerAppointmentDisplayOptionsStartTimeVisibility")]
#endif
		public AppointmentTimeVisibility StartTimeVisibility {
			get { return (AppointmentTimeVisibility)GetValue(StartTimeVisibilityProperty); }
			set { SetValue(StartTimeVisibilityProperty, value); }
		}
		public static readonly DependencyProperty StartTimeVisibilityProperty = CreateStartTimeVisibilityProperty();
		static DependencyProperty CreateStartTimeVisibilityProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerAppointmentDisplayOptions, AppointmentTimeVisibility>("StartTimeVisibility", AppointmentTimeVisibility.Auto, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnStartTimeVisibilityChanged(e.OldValue, e.NewValue));
		}
		private void OnStartTimeVisibilityChanged(AppointmentTimeVisibility oldValue, AppointmentTimeVisibility newValue) {
			UpdateInnerObjectPropertyValue(StartTimeVisibilityProperty, oldValue, newValue);
		}
		#endregion
		#region EndTimeVisibility
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerAppointmentDisplayOptionsEndTimeVisibility")]
#endif
		public AppointmentTimeVisibility EndTimeVisibility {
			get { return (AppointmentTimeVisibility)GetValue(EndTimeVisibilityProperty); }
			set { SetValue(EndTimeVisibilityProperty, value); }
		}
		public static readonly DependencyProperty EndTimeVisibilityProperty = CreateEndTimeVisibilityProperty();
		static DependencyProperty CreateEndTimeVisibilityProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerAppointmentDisplayOptions, AppointmentTimeVisibility>("EndTimeVisibility", AppointmentTimeVisibility.Auto, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnEndTimeVisibilityChanged(e.OldValue, e.NewValue));
		}
		private void OnEndTimeVisibilityChanged(AppointmentTimeVisibility oldValue, AppointmentTimeVisibility newValue) {
			UpdateInnerObjectPropertyValue(EndTimeVisibilityProperty, oldValue, newValue);
		}
		#endregion
		#region TimeDisplayType
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerAppointmentDisplayOptionsTimeDisplayType")]
#endif
		public AppointmentTimeDisplayType TimeDisplayType {
			get { return (AppointmentTimeDisplayType)GetValue(TimeDisplayTypeProperty); }
			set { SetValue(TimeDisplayTypeProperty, value); }
		}
		public static readonly DependencyProperty TimeDisplayTypeProperty = CreateTimeDisplayTypeProperty();
		static DependencyProperty CreateTimeDisplayTypeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerAppointmentDisplayOptions, AppointmentTimeDisplayType>("TimeDisplayType", AppointmentTimeDisplayType.Auto, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnTimeDisplayTypeChanged(e.OldValue, e.NewValue));
		}
		private void OnTimeDisplayTypeChanged(AppointmentTimeDisplayType oldValue, AppointmentTimeDisplayType newValue) {
			UpdateInnerObjectPropertyValue(TimeDisplayTypeProperty, oldValue, newValue);
		}
		#endregion
		#region ShowReminder
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerAppointmentDisplayOptionsShowReminder")]
#endif
		public bool ShowReminder {
			get { return (bool)GetValue(ShowReminderProperty); }
			set { SetValue(ShowReminderProperty, value); }
		}
		public static readonly DependencyProperty ShowReminderProperty = CreateShowReminderProperty();
		static DependencyProperty CreateShowReminderProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerAppointmentDisplayOptions, bool>("ShowReminder", true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnShowReminderChanged(e.OldValue, e.NewValue));
		}
		private void OnShowReminderChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(ShowReminderProperty, oldValue, newValue);
		}
		#endregion
		#region ShowRecurrence
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerAppointmentDisplayOptionsShowRecurrence")]
#endif
		public bool ShowRecurrence {
			get { return (bool)GetValue(ShowRecurrenceProperty); }
			set { SetValue(ShowRecurrenceProperty, value); }
		}
		public static readonly DependencyProperty ShowRecurrenceProperty = CreateShowRecurrenceProperty();
		static DependencyProperty CreateShowRecurrenceProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerAppointmentDisplayOptions, bool>("ShowRecurrence", true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnShowRecurrenceChanged(e.OldValue, e.NewValue));
		}
		private void OnShowRecurrenceChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(ShowRecurrenceProperty, oldValue, newValue);
		}
		#endregion
		#endregion
		protected override DependencyPropertySyncManager CreateDependencyPropertySyncManager() {
			return new SchedulerAppointmentDisplayOptionsPropertySyncManager(this);
		}
#if !SL
		protected override Freezable CreateInstanceCore() {
			return new SchedulerAppointmentDisplayOptions();
		}
#endif
	}
	public class SchedulerDayViewAppointmentDisplayOptions : SchedulerViewAppointmentDisplayOptions {
		public SchedulerDayViewAppointmentDisplayOptions() {
		}
		static SchedulerDayViewAppointmentDisplayOptions() {
#if (!SL)
			StatusDisplayTypeProperty.OverrideMetadata(typeof(SchedulerDayViewAppointmentDisplayOptions), new FrameworkPropertyMetadata(AppointmentStatusDisplayType.Time));
#endif
		}
		#region AllDayAppointmentsStatusDisplayType
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerDayViewAppointmentDisplayOptionsAllDayAppointmentsStatusDisplayType")]
#endif
		public AppointmentStatusDisplayType AllDayAppointmentsStatusDisplayType {
			get { return (AppointmentStatusDisplayType)GetValue(AllDayAppointmentsStatusDisplayTypeProperty); }
			set { SetValue(AllDayAppointmentsStatusDisplayTypeProperty, value); }
		}
		public static readonly DependencyProperty AllDayAppointmentsStatusDisplayTypeProperty = CreateAllDayAppointmentsStatusDisplayTypeProperty();
		static DependencyProperty CreateAllDayAppointmentsStatusDisplayTypeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerDayViewAppointmentDisplayOptions, AppointmentStatusDisplayType>("AllDayAppointmentsStatusDisplayType", AppointmentStatusDisplayType.Never, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnAllDayAppointmentsStatusDisplayTypeChanged(e.OldValue, e.NewValue));
		}
		private void OnAllDayAppointmentsStatusDisplayTypeChanged(AppointmentStatusDisplayType oldValue, AppointmentStatusDisplayType newValue) {
			UpdateInnerObjectPropertyValue(AllDayAppointmentsStatusDisplayTypeProperty, oldValue, newValue);
		}
		#endregion
		#region InnerObject
		public DayViewAppointmentDisplayOptions DayViewInnerObject { get { return (DayViewAppointmentDisplayOptions)InnerObject; } }
		#endregion
		protected override DependencyPropertySyncManager CreateDependencyPropertySyncManager() {
			return new SchedulerDayViewAppointmentDisplayOptionsPropertySyncManager(this);
		}
#if !SL
		protected override Freezable CreateInstanceCore() {
			return new SchedulerDayViewAppointmentDisplayOptions();
		}
#endif
	}
	public class SchedulerWeekViewAppointmentDisplayOptions : SchedulerAppointmentDisplayOptions {
		#region Properties
		#region AppointmentAutoHeight
		public bool AppointmentAutoHeight {
			get { return (bool)GetValue(AppointmentAutoHeightProperty); }
			set { SetValue(AppointmentAutoHeightProperty, value); }
		}
		public static readonly DependencyProperty AppointmentAutoHeightProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerWeekViewAppointmentDisplayOptions, bool>("AppointmentAutoHeight", false, (d, e) => d.OnAppointmentAutoHeightChanged(e.OldValue, e.NewValue));
		void OnAppointmentAutoHeightChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(AppointmentAutoHeightProperty, oldValue, newValue);
		}
		#endregion
		#endregion
		public SchedulerWeekViewAppointmentDisplayOptions() {
		}
		protected override DependencyPropertySyncManager CreateDependencyPropertySyncManager() {
			return new SchedulerWeekViewAppointmentDisplayOptionsPropertySyncManager(this);
		}
#if !SL
		protected override Freezable CreateInstanceCore() {
			return new SchedulerWeekViewAppointmentDisplayOptions();
		}
#endif
	}
	public class SchedulerMonthViewAppointmentDisplayOptions : SchedulerAppointmentDisplayOptions {
		#region Properties
		#region AppointmentAutoHeight
		public bool AppointmentAutoHeight {
			get { return (bool)GetValue(AppointmentAutoHeightProperty); }
			set { SetValue(AppointmentAutoHeightProperty, value); }
		}
		public static readonly DependencyProperty AppointmentAutoHeightProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerMonthViewAppointmentDisplayOptions, bool>("AppointmentAutoHeight", false, (d, e) => d.OnAppointmentAutoHeightChanged(e.OldValue, e.NewValue));
		void OnAppointmentAutoHeightChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(AppointmentAutoHeightProperty, oldValue, newValue);
		}
		#endregion
		#endregion
		static SchedulerMonthViewAppointmentDisplayOptions() {
#if (!SL)
			ShowReminderProperty.OverrideMetadata(typeof(SchedulerMonthViewAppointmentDisplayOptions), new FrameworkPropertyMetadata(false));
			ShowRecurrenceProperty.OverrideMetadata(typeof(SchedulerMonthViewAppointmentDisplayOptions), new FrameworkPropertyMetadata(false));
#endif
		}
		public SchedulerMonthViewAppointmentDisplayOptions() {
		}
		protected override DependencyPropertySyncManager CreateDependencyPropertySyncManager() {
			return new SchedulerMonthViewAppointmentDisplayOptionsPropertySyncManager(this);
		}
		protected override void OnStatusDisplayTypeChanged(AppointmentStatusDisplayType oldValue, AppointmentStatusDisplayType newValue) {
			base.OnStatusDisplayTypeChanged(oldValue, newValue);
			UpdateInnerObjectPropertyValue(StatusDisplayTypeProperty, oldValue, newValue);
		}
#if !SL
		protected override Freezable CreateInstanceCore() {
			return new SchedulerMonthViewAppointmentDisplayOptions();
		}
#endif
	}
	public class SchedulerViewAppointmentDisplayOptions : SchedulerAppointmentDisplayOptions {
		#region Properties
		#region SnapToCellsMode
		public AppointmentSnapToCellsMode SnapToCellsMode {
			get { return (AppointmentSnapToCellsMode)GetValue(SnapToCellsModeProperty); }
			set { SetValue(SnapToCellsModeProperty, value); }
		}
		public static readonly DependencyProperty SnapToCellsModeProperty = CreateSnapToCellsModeProperty();
		static DependencyProperty CreateSnapToCellsModeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewAppointmentDisplayOptions, AppointmentSnapToCellsMode>("SnapToCellsMode", AppointmentSnapToCellsMode.Always, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnSnapToCellsModeChanged(e.OldValue, e.NewValue));
		}
		private void OnSnapToCellsModeChanged(AppointmentSnapToCellsMode oldValue, AppointmentSnapToCellsMode newValue) {
			UpdateInnerObjectPropertyValue(SnapToCellsModeProperty, oldValue, newValue);
		}
		#endregion
		#endregion
	}
	public class SchedulerTimelineViewAppointmentDisplayOptions : SchedulerViewAppointmentDisplayOptions {
		public SchedulerTimelineViewAppointmentDisplayOptions() {
		}
		#region Properties
		#region AppointmentAutoHeight
		public bool AppointmentAutoHeight {
			get { return (bool)GetValue(AppointmentAutoHeightProperty); }
			set { SetValue(AppointmentAutoHeightProperty, value); }
		}
		public static readonly DependencyProperty AppointmentAutoHeightProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerTimelineViewAppointmentDisplayOptions, bool>("AppointmentAutoHeight", false, (d, e) => d.OnAppointmentAutoHeightChanged(e.OldValue, e.NewValue));
		void OnAppointmentAutoHeightChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(AppointmentAutoHeightProperty, oldValue, newValue);
		}
		#endregion
		#region InnerObject
		internal TimelineViewAppointmentDisplayOptions TimelineViewInnerObject { get { return (TimelineViewAppointmentDisplayOptions)InnerObject; } }
		#endregion
		#endregion
		protected override DependencyPropertySyncManager CreateDependencyPropertySyncManager() {
			return new SchedulerTimelineViewAppointmentDisplayOptionsPropertySyncManager(this);
		}
#if !SL
		protected override Freezable CreateInstanceCore() {
			return new SchedulerTimelineViewAppointmentDisplayOptions();
		}
#endif
	}
	#region SchedulerOptionsView
	public class SchedulerOptionsView : SchedulerOptionsViewBase {
	}
	#endregion
	public class ResourceNavigatorOptions : DependencyObject {
		public ResourceNavigatorOptions() {
			ButtonFirst = new ResourceNavigatorButtonOptions();
			ButtonPrevPage = new ResourceNavigatorButtonOptions();
			ButtonPrev = new ResourceNavigatorButtonOptions();
			ButtonNext = new ResourceNavigatorButtonOptions();
			ButtonNextPage = new ResourceNavigatorButtonOptions();
			ButtonLast = new ResourceNavigatorButtonOptions();
			ButtonIncCount = new ResourceNavigatorButtonOptions();
			ButtonDecCount = new ResourceNavigatorButtonOptions();
		}
		#region Visibility
		public ResourceNavigatorVisibility Visibility {
			get { return (ResourceNavigatorVisibility)GetValue(VisibilityProperty); }
			set { SetValue(VisibilityProperty, value); }
		}
		public static readonly DependencyProperty VisibilityProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorOptions, ResourceNavigatorVisibility>("Visibility", ResourceNavigatorVisibility.Always);
		#endregion
		#region ButtonFirst
		public ResourceNavigatorButtonOptions ButtonFirst {
			get { return (ResourceNavigatorButtonOptions)GetValue(ButtonFirstProperty); }
			set { SetValue(ButtonFirstProperty, value); }
		}
		public static readonly DependencyProperty ButtonFirstProperty = CreateButtonFirstProperty();
		static DependencyProperty CreateButtonFirstProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorOptions, ResourceNavigatorButtonOptions>("ButtonFirst", null);
		}
		#endregion
		#region ButtonPrevPage
		public ResourceNavigatorButtonOptions ButtonPrevPage {
			get { return (ResourceNavigatorButtonOptions)GetValue(ButtonPrevPageProperty); }
			set { SetValue(ButtonPrevPageProperty, value); }
		}
		public static readonly DependencyProperty ButtonPrevPageProperty = CreateButtonPrevPageProperty();
		static DependencyProperty CreateButtonPrevPageProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorOptions, ResourceNavigatorButtonOptions>("ButtonPrevPage", null);
		}
		#endregion
		#region ButtonPrev
		public ResourceNavigatorButtonOptions ButtonPrev {
			get { return (ResourceNavigatorButtonOptions)GetValue(ButtonPrevProperty); }
			set { SetValue(ButtonPrevProperty, value); }
		}
		public static readonly DependencyProperty ButtonPrevProperty = CreateButtonPrevProperty();
		static DependencyProperty CreateButtonPrevProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorOptions, ResourceNavigatorButtonOptions>("ButtonPrev", null);
		}
		#endregion
		#region ButtonNext
		public ResourceNavigatorButtonOptions ButtonNext {
			get { return (ResourceNavigatorButtonOptions)GetValue(ButtonNextProperty); }
			set { SetValue(ButtonNextProperty, value); }
		}
		public static readonly DependencyProperty ButtonNextProperty = CreateButtonNextProperty();
		static DependencyProperty CreateButtonNextProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorOptions, ResourceNavigatorButtonOptions>("ButtonNext", null);
		}
		#endregion
		#region ButtonNextPage
		public ResourceNavigatorButtonOptions ButtonNextPage {
			get { return (ResourceNavigatorButtonOptions)GetValue(ButtonNextPageProperty); }
			set { SetValue(ButtonNextPageProperty, value); }
		}
		public static readonly DependencyProperty ButtonNextPageProperty = CreateButtonNextPageProperty();
		static DependencyProperty CreateButtonNextPageProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorOptions, ResourceNavigatorButtonOptions>("ButtonNextPage", null);
		}
		#endregion
		#region ButtonLast
		public ResourceNavigatorButtonOptions ButtonLast {
			get { return (ResourceNavigatorButtonOptions)GetValue(ButtonLastProperty); }
			set { SetValue(ButtonLastProperty, value); }
		}
		public static readonly DependencyProperty ButtonLastProperty = CreateButtonLastProperty();
		static DependencyProperty CreateButtonLastProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorOptions, ResourceNavigatorButtonOptions>("ButtonLast", null);
		}
		#endregion
		#region ButtonIncCount
		public ResourceNavigatorButtonOptions ButtonIncCount {
			get { return (ResourceNavigatorButtonOptions)GetValue(ButtonIncCountProperty); }
			set { SetValue(ButtonIncCountProperty, value); }
		}
		public static readonly DependencyProperty ButtonIncCountProperty = CreateButtonIncCountProperty();
		static DependencyProperty CreateButtonIncCountProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorOptions, ResourceNavigatorButtonOptions>("ButtonIncCount", null);
		}
		#endregion
		#region ButtonDecCount
		public ResourceNavigatorButtonOptions ButtonDecCount {
			get { return (ResourceNavigatorButtonOptions)GetValue(ButtonDecCountProperty); }
			set { SetValue(ButtonDecCountProperty, value); }
		}
		public static readonly DependencyProperty ButtonDecCountProperty = CreateButtonDecCountProperty();
		static DependencyProperty CreateButtonDecCountProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorOptions, ResourceNavigatorButtonOptions>("ButtonDecCount", null);
		}
		#endregion
	}
	public class ResourceNavigatorButtonOptions : DependencyObject {
		#region Visible
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		public static readonly DependencyProperty VisibleProperty = CreateVisibleProperty();
		static DependencyProperty CreateVisibleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorButtonOptions, bool>("Visible", true);
		}
		#endregion
	}
	public class SchedulerTimeIndicatorDisplayOptions : SchedulerOptionsBase<TimeIndicatorDisplayOptionsBase> {
		#region Visibility
		public TimeIndicatorVisibility Visibility {
			get { return (TimeIndicatorVisibility)GetValue(VisibilityProperty); }
			set { SetValue(VisibilityProperty, value); }
		}
		public static readonly DependencyProperty VisibilityProperty = CreateVisibilityProperty();
		static DependencyProperty CreateVisibilityProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerTimeIndicatorDisplayOptions, TimeIndicatorVisibility>("Visibility", TimeIndicatorVisibility.TodayView, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnVisibilityChanged(e.OldValue, e.NewValue));
		}
		void OnVisibilityChanged(TimeIndicatorVisibility oldValue, TimeIndicatorVisibility newValue) {
		}
		#endregion
		protected override DependencyPropertySyncManager CreateDependencyPropertySyncManager() {
			return new TimeIndicatorDisplayOptionsPropertySyncManager(this);
		}
#if !SL
		protected override Freezable CreateInstanceCore() {
			return new SchedulerTimeIndicatorDisplayOptions();
		}
#endif
	}
}
