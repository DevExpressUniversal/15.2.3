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
using System.Collections;
using System.ComponentModel;
using System.Security;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interop;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Commands;
using DevExpress.Services;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Scheduler;
using DevExpress.Xpf.Scheduler.Menu;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.Xpf.Scheduler.UI;
using DevExpress.Mvvm;
using System.Collections.ObjectModel;
using AssignableDocumentServiceHelper = DevExpress.Mvvm.UI.Native.AssignableServiceHelper2<DevExpress.Xpf.Scheduler.SchedulerControl, DevExpress.Mvvm.IDocumentManagerService>;
using AssignableDialogServiceHelper = DevExpress.Mvvm.UI.Native.AssignableServiceHelper2<DevExpress.Xpf.Scheduler.SchedulerControl, DevExpress.Mvvm.IDialogService>;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
#if WPF
using PlatformIndepententKeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;
using PlatformIndependentPropertyChangedCallback = System.Windows.PropertyChangedCallback;
using PlatformIndependentDependencyPropertyChangedEventArgs = System.Windows.DependencyPropertyChangedEventArgs;
using Keys = System.Windows.Forms.Keys;
using DevExpress.Xpf.Core.Core.Native;
using System.Collections.Generic;
using DevExpress.XtraPrinting.XamlExport;
#else
using System.Text;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.ComponentModel;
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using PlatformIndependentKeyEventArgs = DevExpress.Data.KeyEventArgs;
using PlatformIndepententKeyPressEventArgs = DevExpress.Data.KeyPressEventArgs;
using PlatformIndependentPropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using PlatformIndependentDependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using Keys = DevExpress.Data.Keys;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using DateTimeTypeConverter = DevExpress.Xpf.Core.DateTimeTypeConverter;
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DevExpress.Xpf.Core.Core.Native;
using System.Collections.Generic;
#endif
namespace DevExpress.Xpf.Scheduler {
	#region SchedulerControl
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
	[TemplatePart(Name = DateTimeScrollBarName, Type = typeof(ScrollBar))]
	public partial class SchedulerControl : Control, IInnerSchedulerControlOwner, ISupportAppointmentEdit, ISupportAppointmentDependencyEdit, System.ComponentModel.Design.IServiceContainer, IServiceProvider, ILogicalOwner, ISchedulerCommandTarget, IInnerSchedulerCommandTarget, IMouseKeyStateSupport, ICommandAwareControl<SchedulerCommandId> {
		const string DateTimeScrollBarName = "PART_DateTimeScrollBar";
		const string ResourceNavigatorScrollBarName = "PART_ResNavScrollBar";
		#region Fields
		Lazy<BarManagerMenuController> defaultMenuController;
		Lazy<BarManagerMenuController> appointmentMenuController;
		Lazy<BarManagerMenuController> timeRulerMenuController;
		InnerSchedulerControl innerControl;
		DateTimeScrollBar dateTimeScrollBarObject;
		ViewDateTimeScrollController dateTimeScrollController;
		SchedulerPopupMenu menu;
		FormManager formManager;
		XPFContentControl themeLoader;
		bool isActualDragLeave = false;
		bool deferredOnActiveViewChanged = false;
		Locker loadingLocker;
		SchedulerControlAccessor accessor;
		List<Action> propertyDeferredSetUpActions = new List<Action>();
		#endregion
		static SchedulerControl() {
			FormCustomizationUsingMVVM = DefaultBoolean.True;		  
#if DEBUGTEST
			DebugConfig.Init();
#endif
#if !SL
			BrickXamlExporterFactory.RegisterExporter(typeof(DevExpress.XtraScheduler.Printing.BrushBrick), typeof(BrushBrickXamlExporter));
			if (!BrowserInteropHelper.IsBrowserHosted)
				SetResourceDictionarySourceSwitchLevel();
#else
			SubscribeThemeManager();
#endif
		}
#if !SL
		[SecuritySafeCritical]
		static void SetResourceDictionarySourceSwitchLevel() {
			try {
				System.Diagnostics.PresentationTraceSources.ResourceDictionarySource.Switch.Level = System.Diagnostics.SourceLevels.Error;
			} catch {
			}
		}
#endif
		public SchedulerControl() {
			InitializeMouse();
#if !SL
			InitializeTouch();
#endif
			SelectionAppointmentsSynchronizer = new SelectionAppointmentsSynchronizer(this);
			FormCustomizationUsingMVVMLocal = DefaultBoolean.Default;
			VisualizatoinStatistics = new Scheduler.VisualizatoinStatistics();
			PreInitialization();
			DefaultStyleKey = typeof(SchedulerControl);
			this.accessor = new SchedulerControlAccessor(this);
			this.commandManager = new SchedulerControlBarCommandManager(this);
			FocusVisualStyle = null;
			SelectedAppointmentsBindable = new ObservableCollection<Appointment>();
#if SL
			WeakReferenceRepository.AddObject(this);
#endif
			Initialize();
			SubscribeToEvents();
		}
#if SL
		~SchedulerControl() {
			WeakReferenceRepository.RemoveObject(this);
		}
		protected override Size MeasureOverride(Size availableSize) {
			formManager.CloseInplaceForm();
			Size result = base.MeasureOverride(availableSize);
			return result;
		}
#endif
		#region Properties
		public static DefaultBoolean FormCustomizationUsingMVVM { get; set; }
		public DefaultBoolean FormCustomizationUsingMVVMLocal { get; set; }
		internal readonly bool defaultMVVMValue = true;
		internal protected bool ActualFormCustomizationUsingMVVM {
			get {
				if (FormCustomizationUsingMVVM == DefaultBoolean.Default) {
					return FormCustomizationUsingMVVMLocal.ToBoolean(defaultMVVMValue);
				} else {
					if (FormCustomizationUsingMVVMLocal == DefaultBoolean.Default) {
						return FormCustomizationUsingMVVM.ToBoolean(defaultMVVMValue);
					}
				}
				return FormCustomizationUsingMVVMLocal.ToBoolean(defaultMVVMValue);
			}
		}
		private HitTestManager hitTestManager;
#if SL
		protected internal HitTestManager HitTestManager { get { return hitTestManager; } }
		protected internal bool IsThemeApplying { get; set; }
		protected internal bool IsLoadedInternal { get; set; }
#endif
		internal BarManagerMenuController DefaultMenuController { get { return this.defaultMenuController.Value; } }
		internal BarManagerMenuController AppointmentMenuController { get { return this.appointmentMenuController.Value; } }
		internal BarManagerMenuController TimeRulerMenuController { get { return this.timeRulerMenuController.Value; } }
		internal SelectionAppointmentsSynchronizer SelectionAppointmentsSynchronizer { get; set; }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerControlDefaultMenuCustomizations")]
#endif
		public BarManagerActionCollection DefaultMenuCustomizations { get { return DefaultMenuController.ActionContainer.Actions; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerControlAppointmentMenuCustomizations")]
#endif
		public BarManagerActionCollection AppointmentMenuCustomizations { get { return AppointmentMenuController.ActionContainer.Actions; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerControlTimeRulerMenuCustomizations")]
#endif
		public BarManagerActionCollection TimeRulerMenuCustomizations { get { return TimeRulerMenuController.ActionContainer.Actions; } }
		internal ViewDateTimeScrollController DateTimeScrollController { get { return dateTimeScrollController; } }
		protected bool LeftButtonPressed { get; private set; }
		protected bool RightButtonPressed { get; private set; }
		IDocumentManagerService appointmentFormService = null;
		IDialogService appointmentRecurrenceDialogService = null;
		IDialogService manageRecurrentAppointmentDialogService = null;
		IDialogService gotoDateDialogService = null;
		IDocumentManagerService remindersFormService = null;
		IDialogService timeRulerDialogService = null;
#if DEBUGTEST
		public WindowedDocumentUIService AppointmentFormServiceForTest { get { return appointmentFormService as WindowedDocumentUIService; } }
		public DialogService AppointmentRecurrenceDialogServiceForTest { get { return appointmentRecurrenceDialogService as DialogService; } }
		public DialogService ManageRecurrentAppointmentDialogServiceForTest { get { return manageRecurrentAppointmentDialogService as DialogService; } }
		public DialogService GotoDateDialogServiceForTest { get { return gotoDateDialogService as DialogService; } }
		public WindowedDocumentUIService RemindersFormServiceForTest { get { return remindersFormService as WindowedDocumentUIService; } }
		public DialogService TimeRulerDialogServiceForTest { get { return timeRulerDialogService as DialogService; } }
		public void SetRecurrenceDialogService(IDialogService newDialogService) {
			if(appointmentRecurrenceDialogService != null)
				appointmentRecurrenceDialogService = newDialogService;
		}
		public void ReturnDefaultRecurrenceDialogService() {
			appointmentRecurrenceDialogService = TemplateHelper.LoadFromTemplate<IDialogService>(AppointmentRecurrenceDialogServiceTemplate);
		}
#endif
		#region ShowBorder
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerControlShowBorder")]
#endif
		public static readonly DependencyProperty ShowBorderProperty = CreateShowBorderProperty();
		static DependencyProperty CreateShowBorderProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, bool>("ShowBorder", true);
		}
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		#endregion
		#region DialogServices
		void OnDocumentServiceTemplatePropertyChanged(DataTemplate template, ref IDocumentManagerService property) {
			property = TemplateHelper.LoadFromTemplate<IDocumentManagerService>(template);
		}
		void OnDialogServiceTemplatePropertyChanged(DataTemplate template, ref IDialogService property) {
			property = TemplateHelper.LoadFromTemplate<IDialogService>(template);
		}
		#region AppointmentFormServiceTemplate
		public DataTemplate AppointmentFormServiceTemplate {
			get { return (DataTemplate)GetValue(AppointmentFormServiceTemplateProperty); }
			set { SetValue(AppointmentFormServiceTemplateProperty, value); }
		}
		public static readonly DependencyProperty AppointmentFormServiceTemplateProperty = CreateAppointmentFormServiceTemplateProperty();
		static DependencyProperty CreateAppointmentFormServiceTemplateProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, DataTemplate>("AppointmentFormServiceTemplate", null, (d, e) => d.OnAppointmentFormServiceTemplateChanged(e.OldValue, e.NewValue), null);
		}
		void OnAppointmentFormServiceTemplateChanged(DataTemplate oldValue, DataTemplate newValue) {
			OnDocumentServiceTemplatePropertyChanged(newValue, ref appointmentFormService);
		}
		#endregion
		#region AppointmentRecurrenceDialogServiceTemplate
		public DataTemplate AppointmentRecurrenceDialogServiceTemplate {
			get { return (DataTemplate)GetValue(AppointmentRecurrenceDialogServiceTemplateProperty); }
			set { SetValue(AppointmentRecurrenceDialogServiceTemplateProperty, value); }
		}
		public static readonly DependencyProperty AppointmentRecurrenceDialogServiceTemplateProperty = CreateRecurrenceDialogServiceTemplateProperty();
		static DependencyProperty CreateRecurrenceDialogServiceTemplateProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, DataTemplate>("AppointmentRecurrenceDialogServiceTemplate", null, (d, e) => d.OnAppointmentRecurrenceDialogServiceTemplateChanged(e.OldValue, e.NewValue), null);
		}
		void OnAppointmentRecurrenceDialogServiceTemplateChanged(DataTemplate oldValue, DataTemplate newValue) {
			OnDialogServiceTemplatePropertyChanged(newValue, ref this.appointmentRecurrenceDialogService);
		}
		#endregion
		#region ManageRecurrentAppointmentDialogServiceTemplate
		public DataTemplate ManageRecurrentAppointmentDialogServiceTemplate {
			get { return (DataTemplate)GetValue(ManageRecurrentAppointmentDialogServiceTemplateProperty); }
			set { SetValue(ManageRecurrentAppointmentDialogServiceTemplateProperty, value); }
		}
		public static readonly DependencyProperty ManageRecurrentAppointmentDialogServiceTemplateProperty = CreateManageRecurrentAppointmentDialogServiceTemplateProperty();
		static DependencyProperty CreateManageRecurrentAppointmentDialogServiceTemplateProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, DataTemplate>("ManageRecurrentAppointmentDialogServiceTemplate", null, (d, e) => d.OnManageRecurrentAppointmentDialogServiceTemplateChanged(e.OldValue, e.NewValue), null);
		}
		void OnManageRecurrentAppointmentDialogServiceTemplateChanged(DataTemplate oldValue, DataTemplate newValue) {
			OnDialogServiceTemplatePropertyChanged(newValue, ref this.manageRecurrentAppointmentDialogService);
		}
		#endregion       
		#region GotoDateDialogServiceTemplate
		public DataTemplate GotoDateDialogServiceTemplate {
			get { return (DataTemplate)GetValue(GotoDateDialogServiceTemplateProperty); }
			set { SetValue(GotoDateDialogServiceTemplateProperty, value); }
		}
		public static readonly DependencyProperty GotoDateDialogServiceTemplateProperty = CreateGotoDateDialogServiceTemplateProperty();
		static DependencyProperty CreateGotoDateDialogServiceTemplateProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, DataTemplate>("GotoDateDialogServiceTemplate", null, (d, e) => d.OnGotoDateDialogServiceTemplateChanged(e.OldValue, e.NewValue), null);
		}
		void OnGotoDateDialogServiceTemplateChanged(DataTemplate oldValue, DataTemplate newValue) {
			OnDialogServiceTemplatePropertyChanged(newValue, ref this.gotoDateDialogService);
		}
		#endregion
		#region RemindersFormServiceTemplate
		public DataTemplate RemindersFormServiceTemplate {
			get { return (DataTemplate)GetValue(RemindersFormServiceTemplateProperty); }
			set { SetValue(RemindersFormServiceTemplateProperty, value); }
		}
		public static readonly DependencyProperty RemindersFormServiceTemplateProperty = CreateRemindersFormServiceTemplateProperty();
		static DependencyProperty CreateRemindersFormServiceTemplateProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, DataTemplate>("RemindersFormServiceTemplate", null, (d, e) => d.OnRemindersFormServiceTemplateChanged(e.OldValue, e.NewValue), null);
		}
		void OnRemindersFormServiceTemplateChanged(DataTemplate oldValue, DataTemplate newValue) {
			OnDocumentServiceTemplatePropertyChanged(newValue, ref this.remindersFormService);
		}
		#endregion
		#region TimeRulerDialogServiceTemplate
		public DataTemplate TimeRulerDialogServiceTemplate {
			get { return (DataTemplate)GetValue(TimeRulerDialogServiceTemplateProperty); }
			set { SetValue(TimeRulerDialogServiceTemplateProperty, value); }
		}
		public static readonly DependencyProperty TimeRulerDialogServiceTemplateProperty = CreateTimeRulerDialogServiceTemplateProperty();
		static DependencyProperty CreateTimeRulerDialogServiceTemplateProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, DataTemplate>("TimeRulerDialogServiceTemplate", null, (d, e) => d.OnTimeRulerDialogServiceTemplateChanged(e.OldValue, e.NewValue), null);
		}
		void OnTimeRulerDialogServiceTemplateChanged(DataTemplate oldValue, DataTemplate newValue) {
			OnDialogServiceTemplatePropertyChanged(newValue, ref this.timeRulerDialogService);
		}
		#endregion
		#endregion
		#region HitTestType
		public static readonly DependencyProperty HitTestTypeProperty =
			DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedProperty<SchedulerControl, SchedulerHitTest>("HitTestType", SchedulerHitTest.None, FrameworkPropertyMetadataOptions.Inherits, null);
		public static SchedulerHitTest GetHitTestType(DependencyObject element) {
			return (SchedulerHitTest)element.GetValue(HitTestTypeProperty);
		}
		public static void SetHitTestType(DependencyObject element, SchedulerHitTest value) {
			element.SetValue(HitTestTypeProperty, value);
		}
		#endregion
		#region SelectableIntervalViewInfo
		public static readonly DependencyProperty SelectableIntervalViewInfoProperty =
			DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedProperty<SchedulerControl, ISelectableIntervalViewInfo>("SelectableIntervalViewInfo", null, FrameworkPropertyMetadataOptions.Inherits, null);
		public static ISelectableIntervalViewInfo GetSelectableIntervalViewInfo(DependencyObject element) {
			return (ISelectableIntervalViewInfo)element.GetValue(SelectableIntervalViewInfoProperty);
		}
		public static void SetSelectableIntervalViewInfo(DependencyObject element, ISelectableIntervalViewInfo value) {
			element.SetValue(SelectableIntervalViewInfoProperty, value);
		}
		#endregion
		#region InplaceEditTemplate
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerControlInplaceEditTemplate")]
#endif
		public DataTemplate InplaceEditTemplate {
			get { return (DataTemplate)GetValue(InplaceEditTemplateProperty); }
			set { SetValue(InplaceEditTemplateProperty, value); }
		}
		public static readonly DependencyProperty InplaceEditTemplateProperty = CreateInplaceEditTemplateProperty();
		static DependencyProperty CreateInplaceEditTemplateProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, DataTemplate>("InplaceEditTemplate", null);
		}
		#endregion
		#region ResourceNavigatorHorizontalStyle
		public static readonly DependencyProperty ResourceNavigatorHorizontalStyleProperty = CreateResourceNavigatorHorizontalStyleProperty();
		static DependencyProperty CreateResourceNavigatorHorizontalStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, Style>("ResourceNavigatorHorizontalStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerControlResourceNavigatorHorizontalStyle")]
#endif
		public Style ResourceNavigatorHorizontalStyle {
			get { return (Style)GetValue(ResourceNavigatorHorizontalStyleProperty); }
			set { SetValue(ResourceNavigatorHorizontalStyleProperty, value); }
		}
		#endregion
		#region ResourceNavigatorVerticalStyle
		public static readonly DependencyProperty ResourceNavigatorVerticalStyleProperty = CreateResourceNavigatorVerticalStyleProperty();
		static DependencyProperty CreateResourceNavigatorVerticalStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, Style>("ResourceNavigatorVerticalStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerControlResourceNavigatorVerticalStyle")]
#endif
		public Style ResourceNavigatorVerticalStyle {
			get { return (Style)GetValue(ResourceNavigatorVerticalStyleProperty); }
			set { SetValue(ResourceNavigatorVerticalStyleProperty, value); }
		}
		#endregion
		#region DragDropOptions
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerControlDragDropOptions")]
#endif
		public DragDropOptions DragDropOptions {
			get { return (DragDropOptions)GetValue(DragDropOptionsProperty); }
			set { SetValue(DragDropOptionsProperty, value); }
		}
		public static readonly DependencyProperty DragDropOptionsProperty = CreateDragDropOptionsProperty();
		static DependencyProperty CreateDragDropOptionsProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, DragDropOptions>("DragDropOptions", null, (d, e) => d.OnDragDropOptionsChanged(e.OldValue, e.NewValue));
		}
		protected virtual void OnDragDropOptionsChanged(DragDropOptions oldOptions, DragDropOptions newOptions) {
			UnsubscribeDragDropOptionsEvents(oldOptions);
			SubscribeDragDropOptionsEvents(newOptions);
			OnMovementTypeChanged(this, EventArgs.Empty);
			OnDragDropModeChanged(this, EventArgs.Empty);
		}
		protected virtual void SubscribeDragDropOptionsEvents(DragDropOptions options) {
			if (options == null)
				return;
			options.DragDropModeChanged += OnDragDropModeChanged;
			options.MovementTypeChanged += OnMovementTypeChanged;
		}
		protected virtual void UnsubscribeDragDropOptionsEvents(DragDropOptions options) {
			if (options == null)
				return;
			options.DragDropModeChanged -= OnDragDropModeChanged;
			options.MovementTypeChanged -= OnMovementTypeChanged;
		}
		void OnMovementTypeChanged(object sender, EventArgs e) {
			if (this.innerControl != null)
				this.InnerControl.RecreateAppointmentChangeHelper();
		}
		void OnDragDropModeChanged(object sender, EventArgs e) {
		}
		#endregion
		#region ActiveView
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public SchedulerViewBase ActiveView { get { return (SchedulerViewBase)GetValue(ActiveViewProperty); } }
		static readonly DependencyPropertyKey ActiveViewPropertyKey = CreateActiveViewProperty();
		public static readonly DependencyProperty ActiveViewProperty = ActiveViewPropertyKey.DependencyProperty;
		static DependencyPropertyKey CreateActiveViewProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<SchedulerControl, SchedulerViewBase>("ActiveView", null);
		}
		#endregion
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerControlActiveViewType"),
#endif
DefaultValue(SchedulerViewType.Day), Category(SRCategoryNames.View), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		#region ActiveViewType
		public SchedulerViewType ActiveViewType {
			get { return (SchedulerViewType)GetValue(ActiveViewTypeProperty); }
			set { SetValue(ActiveViewTypeProperty, value); }
		}
		public static readonly DependencyProperty ActiveViewTypeProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, SchedulerViewType>("ActiveViewType", SchedulerViewType.Day, (d, e) => d.OnActiveViewTypeChanged(e.OldValue, e.NewValue), (d, e) => d.CoerceActiveViewType(e));
		void OnActiveViewTypeChanged(SchedulerViewType oldValue, SchedulerViewType newValue) {
			if (!CanDeferPropertySet())
				PropertySyncManager.Update(ActiveViewTypeProperty, null, newValue);
			else
				this.propertyDeferredSetUpActions.Add(() => {
					PropertySyncManager.Update(ActiveViewTypeProperty, null, newValue);
				});
		}
		SchedulerViewType CoerceActiveViewType(SchedulerViewType baseValue) {
			if (baseValue == SchedulerViewType.Gantt)
				return SchedulerViewType.Timeline;
			else
				return baseValue;
		}
		#endregion
		#region Start
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerControlStart"),
#endif
		Category(SRCategoryNames.View), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
#if SL
		[TypeConverter(typeof(DateTimeTypeConverter))]
#endif
		public DateTime Start {
			get { return (DateTime)GetValue(StartProperty); }
			set { SetValue(StartProperty, value); }
		}
		public static readonly DependencyProperty StartProperty = CreateStartProperty();
		static DependencyProperty CreateStartProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, DateTime>("Start", NotificationTimeInterval.MinDateTime, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnStartChanged(e.OldValue, e.NewValue));
		}
		protected virtual void OnStartChanged(DateTime oldValue, DateTime newValue) {
			PropertySyncManager.Update(StartProperty, oldValue, newValue);
		}
		#endregion
		#region GroupType
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerControlGroupType"),
#endif
		Category(SRCategoryNames.View), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public SchedulerGroupType GroupType {
			get { return (SchedulerGroupType)GetValue(GroupTypeProperty); }
			set { SetValue(GroupTypeProperty, value); }
		}
		public static readonly DependencyProperty GroupTypeProperty = CreateGroupTypeProperty();
		static DependencyProperty CreateGroupTypeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, SchedulerGroupType>("GroupType", SchedulerGroupType.None, (d, e) => d.OnGroupTypeChanged(e.OldValue, e.NewValue));
		}
		protected internal virtual void OnGroupTypeChanged(SchedulerGroupType oldValue, SchedulerGroupType newValue) {
			if (!CanDeferPropertySet())
				PropertySyncManager.Update(GroupTypeProperty, null, newValue);
			else {
				this.propertyDeferredSetUpActions.Add(() => {
					PropertySyncManager.Update(GroupTypeProperty, null, newValue);
				});
			}
		}
		#endregion
		#region DayView
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerControlDayView"),
#endif
		Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DayView DayView {
			get { return (DayView)GetValue(DayViewProperty); }
			set { SetValue(DayViewProperty, value); }
		}
		public static readonly DependencyProperty DayViewProperty = CreateDayViewProperty();
		static DependencyProperty CreateDayViewProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, DayView>("DayView", null, new FrameworkPropertyMetadataOptions(), OnDayViewPropertyChanged);
		}		
		static void OnDayViewPropertyChanged(SchedulerControl control, DependencyPropertyChangedEventArgs<DayView> e) {
			control.OnDayViewChanged(e.OldValue, e.NewValue);
		}
		protected internal virtual void OnDayViewChanged(DayView oldValue, DayView newValue) {
			if (newValue == null)
				newValue = new DayView();
			OnViewAssigned(newValue);
		}
		#endregion
		#region WorkWeekView
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerControlWorkWeekView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public WorkWeekView WorkWeekView {
			get { return (WorkWeekView)GetValue(WorkWeekViewProperty); }
			set { SetValue(WorkWeekViewProperty, value); }
		}
		public static readonly DependencyProperty WorkWeekViewProperty = CreateWorkWeekViewProperty();
		static DependencyProperty CreateWorkWeekViewProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, WorkWeekView>("WorkWeekView", null, new FrameworkPropertyMetadataOptions(), OnWorkWeekViewPropertyChanged);
		}
		static void OnWorkWeekViewPropertyChanged(SchedulerControl control, DependencyPropertyChangedEventArgs<WorkWeekView> e) {
			control.OnWorkWeekViewChanged(e.OldValue, e.NewValue);
		}
		protected internal virtual void OnWorkWeekViewChanged(WorkWeekView oldValue, WorkWeekView newValue) {
			if (newValue == null)
				newValue = new WorkWeekView();
			OnViewAssigned(newValue);
		}
		#endregion
		#region WeekView
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerControlWeekView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public WeekView WeekView {
			get { return (WeekView)GetValue(WeekViewProperty); }
			set { SetValue(WeekViewProperty, value); }
		}
		public static readonly DependencyProperty WeekViewProperty = CreateWeekViewProperty();
		static DependencyProperty CreateWeekViewProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, WeekView>("WeekView", null, new FrameworkPropertyMetadataOptions(), OnWeekViewPropertyChanged);
		}
		static void OnWeekViewPropertyChanged(SchedulerControl control, DependencyPropertyChangedEventArgs<WeekView> e) {
			control.OnWeekViewChanged(e.OldValue, e.NewValue);
		}
		protected internal virtual void OnWeekViewChanged(WeekView oldValue, WeekView newValue) {
			if (newValue == null)
				newValue = new WeekView();
			OnViewAssigned(newValue);
		}
		#endregion
		#region FullWeekView
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerControlFullWeekView"),
#endif
		Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FullWeekView FullWeekView {
			get { return (FullWeekView)GetValue(FullWeekViewProperty); }
			set { SetValue(FullWeekViewProperty, value); }
		}
		public static readonly DependencyProperty FullWeekViewProperty = CreateFullWeekViewProperty();
		static DependencyProperty CreateFullWeekViewProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, FullWeekView>("FullWeekView", null, new FrameworkPropertyMetadataOptions(), OnFullWeekViewPropertyChanged);
		}
		static void OnFullWeekViewPropertyChanged(SchedulerControl control, DependencyPropertyChangedEventArgs<FullWeekView> e) {
			control.OnFullWeekViewChanged(e.OldValue, e.NewValue);
		}
		protected internal virtual void OnFullWeekViewChanged(FullWeekView oldValue, FullWeekView newValue) {
			if (newValue == null)
				newValue = new FullWeekView();
			OnViewAssigned(newValue);
		}
		#endregion    
		#region MonthView
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerControlMonthView"),
#endif
		Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MonthView MonthView {
			get { return (MonthView)GetValue(MonthViewProperty); }
			set { SetValue(MonthViewProperty, value); }
		}
		public static readonly DependencyProperty MonthViewProperty = CreateMonthViewProperty();
		static DependencyProperty CreateMonthViewProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, MonthView>("MonthView", null, new FrameworkPropertyMetadataOptions(), OnMonthViewPropertyChanged);
		}
		static void OnMonthViewPropertyChanged(SchedulerControl control, DependencyPropertyChangedEventArgs<MonthView> e) {
			control.OnMonthViewChanged(e.OldValue, e.NewValue);
		}
		protected internal virtual void OnMonthViewChanged(MonthView oldValue, MonthView newValue) {
			if (newValue == null)
				newValue = new MonthView();
			OnViewAssigned(newValue);
		}
		#endregion
		#region TimelineView
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerControlTimelineView"),
#endif
		Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TimelineView TimelineView {
			get { return (TimelineView)GetValue(TimelineViewProperty); }
			set { SetValue(TimelineViewProperty, value); }
		}
		public static readonly DependencyProperty TimelineViewProperty = CreateTimelineViewProperty();
		static DependencyProperty CreateTimelineViewProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, TimelineView>("TimelineView", null, new FrameworkPropertyMetadataOptions(), OnTimelineViewPropertyChanged);
		}
		static void OnTimelineViewPropertyChanged(SchedulerControl control, DependencyPropertyChangedEventArgs<TimelineView> e) {
			control.OnTimelineViewChanged(e.OldValue, e.NewValue);
		}
		protected internal virtual void OnTimelineViewChanged(TimelineView oldValue, TimelineView newValue) {
			if (newValue == null)
				newValue = new TimelineView();
			OnViewAssigned(newValue);
		}
		#endregion
		#region LogicalChildren
		protected override IEnumerator LogicalChildren {
			get {
				if (InnerControl != null && Views != null) {
					IEnumerator views = Views.GetViews().GetEnumerator();
					IEnumerator forms = FormManager.GetActiveFormList();
					List<object> res = new List<object>();
					while (views.MoveNext())
						res.Add(views.Current);
					while (forms.MoveNext())
						res.Add(forms.Current);
					return new MergedEnumerator(res.GetEnumerator(), this.logicalChildren.GetEnumerator());
				} else
					return null;
			}
		}
		#endregion
		#region SelectedAppointments
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppointmentBaseCollection SelectedAppointments { get { return InnerControl != null ? InnerControl.SelectedAppointments : new AppointmentBaseCollection(); } }
		public static readonly DependencyProperty SelectedAppointmentsBindableProperty = CreateSelectedAppointmentsBindableProperty();
		static DependencyProperty CreateSelectedAppointmentsBindableProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, ObservableCollection<Appointment>>("SelectedAppointmentsBindable", null, (d, e) => d.SelectedAppointmentsBindableChanged(e.NewValue, e.OldValue), (d, e) => d.SelectedAppointmentsBindableChangedCoerce(e));
		}
		public ObservableCollection<Appointment> SelectedAppointmentsBindable {
			get { return (ObservableCollection<Appointment>)GetValue(SelectedAppointmentsBindableProperty); }
			set { SetValue(SelectedAppointmentsBindableProperty, value); }
		}
		void SelectedAppointmentsBindableChanged(ObservableCollection<Appointment> newValue, ObservableCollection<Appointment> oldValue) {
			if (oldValue != null) {
				SelectionAppointmentsSynchronizer.UnsubscribeOnObservableCollectionChanged(oldValue);
			}
			SelectionAppointmentsSynchronizer.SubscribeOnObservableCollectionChanged(newValue);
			SelectionAppointmentsSynchronizer.SyhcnronizeSelectedAppointmentsBindable();
		}
		ObservableCollection<Appointment> SelectedAppointmentsBindableChangedCoerce(ObservableCollection<Appointment> value) {
			if (value == null)
				return new ObservableCollection<Appointment>();
			return value;
		}
		#endregion
		#region AppointmentChangeHelper
		internal AppointmentChangeHelper AppointmentChangeHelper { get { return InnerControl == null ? null : InnerControl.AppointmentChangeHelper; } }
		#endregion
		#region LimitInterval
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerControlLimitInterval"),
#endif
Category(SRCategoryNames.Scheduler), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public TimeInterval LimitInterval {
			get { return (TimeInterval)GetValue(LimitIntervalProperty); }
			set { SetValue(LimitIntervalProperty, value); }
		}
		public static readonly DependencyProperty LimitIntervalProperty = CreateLimitIntervalProperty();
		static DependencyProperty CreateLimitIntervalProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, TimeInterval>("LimitInterval", NotificationTimeInterval.FullRange,
				(d, e) => d.OnLimitIntervalChanged(e.OldValue, e.NewValue), (d, e) => d.OnLimitIntervalCoerce(e));
		}
		void OnLimitIntervalChanged(TimeInterval oldValue, TimeInterval newValue) {
			if (InnerControl != null)
#if SL
				InnerControl.LimitInterval = LimitIntervalCoerceCore(newValue);
#else
				InnerControl.LimitInterval = (NotificationTimeInterval)newValue;
#endif
		}
		TimeInterval OnLimitIntervalCoerce(TimeInterval value) {
#if !SL
			return LimitIntervalCoerceCore(value);
#else
			return value;
#endif
		}
		NotificationTimeInterval LimitIntervalCoerceCore(TimeInterval value) {
			return (value == null) ? NotificationTimeInterval.FullRange : new NotificationTimeInterval(value.Start, value.Duration);
		}
		#endregion
		#region WorkDays
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WorkDaysCollection WorkDays { get { return InnerControl.WorkDays; } }
		#endregion
		#region DefaultResourceColorSchemas
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public SchedulerColorSchemaCollection DefaultResourceColorSchemas {
			get { return (SchedulerColorSchemaCollection)GetValue(DefaultResourceColorSchemasProperty); }
			set { SetValue(DefaultResourceColorSchemasProperty, value); }
		}
		public static readonly DependencyProperty DefaultResourceColorSchemasProperty = CreateDefaultResourceColorSchemasProperty();
		static DependencyProperty CreateDefaultResourceColorSchemasProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, SchedulerColorSchemaCollection>("DefaultResourceColorSchemas", new SchedulerColorSchemaCollection(), (d, e) => d.OnDefaultResourceColorSchemasChanged(e.OldValue, e.NewValue));
		}
		protected void OnDefaultResourceColorSchemasChanged(SchedulerColorSchemaCollection oldValue, SchedulerColorSchemaCollection newValue) {
			SynchronizeToInnerResourceColorSchemas();
		}
		#endregion        
		#region ResourceColorSchemas
		[ Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SchedulerColorSchemaCollection ResourceColorSchemas {
			get { return (SchedulerColorSchemaCollection)GetValue(ResourceColorSchemasProperty); }
		}
		internal static readonly DependencyPropertyKey ResourceColorSchemasPropertyKey = DependencyPropertyManager.RegisterReadOnly("ResourceColorSchemas", typeof(SchedulerColorSchemaCollection), typeof(SchedulerControl), new PropertyMetadata(null));
		public static readonly DependencyProperty ResourceColorSchemasProperty = ResourceColorSchemasPropertyKey.DependencyProperty;
		#endregion
		#region ResourceNavigator
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerControlResourceNavigator")]
#endif
		public ResourceNavigatorOptions ResourceNavigator {
			get { return (ResourceNavigatorOptions)GetValue(ResourceNavigatorProperty); }
			set { SetValue(ResourceNavigatorProperty, value); }
		}
		public static readonly DependencyProperty ResourceNavigatorProperty = CreateResourceNavigatorProperty();
		static DependencyProperty CreateResourceNavigatorProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, ResourceNavigatorOptions>("ResourceNavigator", null);
		}
		#endregion
		#region OptionsBehavior
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerControlOptionsBehavior"),
#endif
		Category(SRCategoryNames.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsBehavior OptionsBehavior {
			get { return (OptionsBehavior)GetValue(OptionsBehaviorProperty); }
			set { SetValue(OptionsBehaviorProperty, value); }
		}
		public static readonly DependencyProperty OptionsBehaviorProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, OptionsBehavior>("OptionsBehavior", null, (d, e) => d.UpdateInnerObjectPropertyValue(OptionsBehaviorProperty, e.OldValue, e.NewValue), null);
		#endregion
		#region SchedulerOptionsCustomization
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerControlOptionsCustomization"),
#endif
		Category(SRCategoryNames.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsCustomization OptionsCustomization {
			get { return (OptionsCustomization)GetValue(OptionsCustomizationProperty); }
			set { SetValue(OptionsCustomizationProperty, value); }
		}
		public static readonly DependencyProperty OptionsCustomizationProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, OptionsCustomization>("OptionsCustomization", null, (d, e) => d.UpdateInnerObjectPropertyValue(OptionsCustomizationProperty, e.OldValue, e.NewValue), null);
		#endregion
		#region OptionsView
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerControlOptionsView"),
#endif
		Category(SRCategoryNames.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsView OptionsView {
			get { return (OptionsView)GetValue(OptionsViewProperty); }
			set { SetValue(OptionsViewProperty, value); }
		}
		public static readonly DependencyProperty OptionsViewProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, OptionsView>("OptionsView", null, (d, e) => d.UpdateInnerObjectPropertyValue(OptionsViewProperty, e.OldValue, e.NewValue), null);
		#endregion
		#region OptionsRangeControl
#if !SL
		public SchedulerOptionsRangeControl OptionsRangeControl {
			get { return (SchedulerOptionsRangeControl)GetValue(OptionsRangeControlProperty); }
			set { SetValue(OptionsRangeControlProperty, value); }
		}
		public static readonly DependencyProperty OptionsRangeControlProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, SchedulerOptionsRangeControl>("OptionsRangeControl", null, (d, e) => d.OnOptionsRangeControlChanged(e.OldValue, e.NewValue), null);
		void OnOptionsRangeControlChanged(SchedulerOptionsRangeControl oldValue, SchedulerOptionsRangeControl newValue) {
		}
#endif
		#endregion
		#region Selection
		internal SchedulerViewSelection Selection {
			get {
				if (innerControl != null)
					return innerControl.Selection;
				else
					return null;
			}
			set {
				if (innerControl != null)
					innerControl.Selection = value;
			}
		}
		#endregion
		[Browsable(false)]
		public bool SupportsRecurrence { get { return Storage != null ? Storage.SupportsRecurrence : false; } }
		[Browsable(false)]
		public bool SupportsReminders { get { return Storage != null ? Storage.SupportsReminders : false; } }
		[Browsable(false)]
		public bool ResourceSharing { get { return Storage != null ? Storage.ResourceSharing : false; } }
		[Browsable(false)]
		public bool RemindersEnabled { get { return Storage != null ? Storage.RemindersEnabled : false; } }
		[Browsable(false)]
		public bool UnboundMode { get { return Storage != null ? Storage.UnboundMode : true; } }
		#region Storage
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerControlStorage"),
#endif
 Category(SRCategoryNames.Data), DefaultValue(null)]
		public SchedulerStorage Storage {
			get { return (SchedulerStorage)GetValue(StorageProperty); }
			set { SetValue(StorageProperty, value); }
		}
		public static readonly DependencyProperty StorageProperty = CreateStorageProperty();
		static DependencyProperty CreateStorageProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, SchedulerStorage>("Storage", null, (d, e) => d.OnStorageChanged(e.OldValue, e.NewValue), null);
		}
		private void OnStorageChanged(SchedulerStorage oldValue, SchedulerStorage newValue) {
			if (IsLoading) {
				CancelStorageUpdate(oldValue);
				LockStorageUpdate(newValue);
			}
			PropertySyncManager.Update(StorageProperty, null, newValue);
			SetupInnerDataContextBinding();
		}
		#endregion
		internal XpfSchedulerMouseHandler MouseHandler { get { return InnerControl == null ? null : (XpfSchedulerMouseHandler)InnerControl.MouseHandler; } }
		internal FormManager FormManager { get { return formManager; } }
		#region AppointmentSelectionController
		internal AppointmentSelectionController AppointmentSelectionController {
			get {
				if (innerControl != null)
					return innerControl.AppointmentSelectionController;
				else
					return null;
			}
		}
		#endregion
		internal SchedulerInplaceEditControllerEx InplaceEditController { get { return InnerControl == null ? null : (SchedulerInplaceEditControllerEx)InnerControl.InplaceEditController; } }
		protected internal InnerSchedulerControl InnerControl { get { return innerControl; } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public SchedulerViewRepository Views { get { return (SchedulerViewRepository)InnerControl.Views; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeInterval SelectedInterval { get { return Selection != null ? Selection.Interval.Clone() : TimeInterval.Empty; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Resource SelectedResource { get { return Selection != null ? Selection.Resource : ResourceBase.Empty; } }
		protected internal DateTimeScrollBar DateTimeScrollBarObject { get { return dateTimeScrollBarObject; } }
		#region DateTimeScrollBar
		protected internal ScrollBarWrapper DateTimeScrollBar { get { return (ScrollBarWrapper)GetValue(DateTimeScrollBarProperty); } }
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty DateTimeScrollBarProperty = CreateDateTimeScrollBarProperty();
		static DependencyProperty CreateDateTimeScrollBarProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, ScrollBarWrapper>("DateTimeScrollBar", null, (d, e) => d.OnDateTimeScrollBarChanged(e.OldValue, e.NewValue), null);
		}
		protected void OnDateTimeScrollBarChanged(ScrollBarWrapper oldValue, ScrollBarWrapper newValaue) {
			if (oldValue != null) {
				UnsubscribeDateTimeScrollBarEvents();
				if (dateTimeScrollBarObject != null) {
					this.dateTimeScrollBarObject.Dispose();
					this.dateTimeScrollBarObject = null;
				}
				DestroyDateTimeScrollController();
			}
			if (newValaue != null) {
				CreateDateTimeScrollBar();
				CreateDateTimeScrollController();
				SubscribeDateTimeScrollBarEvents();
				UpdateDateTimeScrollBarValue();
			}
		}
		#endregion
		protected SchedulerPopupMenu Menu {
			get {
				if (menu == null)
					menu = new SchedulerPopupMenu(this);
				return menu;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsLoading { get { return loadingLocker.IsLocked; } }
		internal VisualizatoinStatistics VisualizatoinStatistics { get; private set; }
		public TimeZoneHelper TimeZoneHelper { get { return InnerControl.TimeZoneHelper; } }
		#endregion
		#region property synchronization logic
		DependencyPropertySyncManager propertySyncManager;
		protected internal DependencyPropertySyncManager PropertySyncManager { get { return propertySyncManager; } }
		protected virtual DependencyPropertySyncManager CreatePropertySyncManager() {
			return new SchedulerControlPropertySyncManager(this);
		}
		protected virtual bool CanSyncInnerObject() {
			return InnerControl != null;
		}
		protected internal virtual void UpdateInnerObjectPropertyValue(DependencyProperty property, object oldValue, object newValue) {
			if (!CanSyncInnerObject())
				PropertySyncManager.StartDeferredChanges();
			PropertySyncManager.Update(property, oldValue, newValue);
		}
		#endregion
		protected internal virtual void PreInitialization() {
			this.loadingLocker = new Locker();
			this.propertySyncManager = CreatePropertySyncManager();
			PropertySyncManager.Register();
		}
		protected internal virtual void Initialize() {
			this.hitTestManager = new HitTestManager(this);
			ResourceNavigator = CreateResourceNavigator();
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			DefaultStyleKey = typeof(SchedulerControl);
			this.innerControl = CreateInnerControl();
			this.innerControl.Initialize();
			this.SetValue(ResourceColorSchemasPropertyKey, innerControl.ResourceColorSchemas);
			if (PropertySyncManager.IsDeferredChanges)
				PropertySyncManager.CommitDeferredChanges();
			AddServices();
			this.formManager = new FormManager(this);
			this.defaultMenuController = CreateMenuController();
			this.appointmentMenuController = CreateMenuController();
			this.timeRulerMenuController = CreateTimeRulerMenuController();
#if SL
			AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseButtonDownHandler), true);
			AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseButtonUpHandler), true);
#endif
		}
		public ISchedulerStorage GetCoreStorage() {
			return Storage.InnerStorage;
		}
		protected internal virtual void SetupInnerDataContextBinding() {
			InnerBindingHelper.SetupDataContextBinding(this, Storage);
		}
		protected internal virtual InnerSchedulerControl CreateInnerControl() {
			return new XpfInnerSchedulerControl(this);
		}
		protected virtual ResourceNavigatorOptions CreateResourceNavigator() {
			return new ResourceNavigatorOptions();
		}
		protected internal virtual void SubscribeInnerControlEvents() {
			innerControl.AppointmentsSelectionChanged += OnAppointmentsSelectionChanged;
			innerControl.StorageChanged += OnStorageChanged;
			innerControl.ReminderAlert += OnReminderAlert;
			innerControl.ActiveViewChanging += OnActiveViewChanging;
			innerControl.BeforeActiveViewChange += OnBeforeActiveViewChange;
			innerControl.AfterActiveViewChange += OnAfterActiveViewChange;
			InnerControl.ViewUIChanged += new SchedulerViewUIChangedEventHandler(OnViewUIChanged);
			InnerControl.ActiveViewChanged += new EventHandler(OnActiveViewChanged);
			InnerControl.GroupTypeChanged += new EventHandler(OnGroupTypeChagend);
#if !SL
			InnerControl.OptionsBehavior.Changed += OnOptionsBehaviorChanged;
#endif
			SelectionAppointmentsSynchronizer.SubscribeOnInnerCollectionChange();
			((ICommandAwareControl<SchedulerCommandId>)InnerControl).UpdateUI += OnInnerSchedulerControlUpdateUI;
		}
		protected internal virtual void UnsubscribeInnerControlEvents() {
			innerControl.AppointmentsSelectionChanged -= OnAppointmentsSelectionChanged;
			innerControl.StorageChanged -= OnStorageChanged;
			innerControl.ReminderAlert -= OnReminderAlert;
			innerControl.ActiveViewChanging -= OnActiveViewChanging;
			innerControl.BeforeActiveViewChange -= OnBeforeActiveViewChange;
			innerControl.AfterActiveViewChange -= OnAfterActiveViewChange;
			InnerControl.ViewUIChanged -= new SchedulerViewUIChangedEventHandler(OnViewUIChanged);
			InnerControl.ActiveViewChanged -= new EventHandler(OnActiveViewChanged);
			InnerControl.GroupTypeChanged -= new EventHandler(OnGroupTypeChagend);
#if !SL
			InnerControl.OptionsBehavior.Changed -= OnOptionsBehaviorChanged;
#endif
			((ICommandAwareControl<SchedulerCommandId>)InnerControl).UpdateUI -= OnInnerSchedulerControlUpdateUI;
			SelectionAppointmentsSynchronizer.UnsubscribeOnInnerCollectionChange();
		}
		void OnInnerSchedulerControlUpdateUI(object sender, EventArgs e) {
			OnUpdateUI(sender, e);
		}	
		internal void OnReminderAlert(object sender, ReminderEventArgs e) {
			if (!IsLoaded)
				return;
			if (OptionsBehavior.ShowRemindersForm) {
				if (ActualFormCustomizationUsingMVVM) {
					if(remindersFormService == null)
						return;
					RemindersFormEventArgs args = new RemindersFormEventArgs(e.AlertNotifications);
					RaiseRemindersFormShowing(args);
					if (!args.Cancel) {
						if(!RemindersFormViewModel.CurrentInstances.ContainsKey(this)) {
							RemindersFormViewModel viewModel = args.ViewModel != null ? (args.ViewModel as RemindersFormViewModel) : RemindersFormViewModel.Create(this);
							if(viewModel != null) {
								RemindersFormViewModel.CurrentInstances[this] = viewModel;
								viewModel.InsertRemindersItems(e.AlertNotifications);
								AssignableDocumentServiceHelper.DoServiceAction(this, remindersFormService, service => {
									IDocument document = service.CreateDocument(viewModel);
									viewModel.ActiveDocument = document;
									document.DestroyOnClose = true;
									document.Show();
								});
							}
						} else {
							RemindersFormViewModel.CurrentInstances[this].AddReminderAlert(args.AlertNotifications);
						}
					} else {
						if(RemindersFormViewModel.CurrentInstances.ContainsKey(this)) {
							RemindersFormViewModel.CurrentInstances[this].CloseForm();
						}
					}
				} else {
					FormManager.ShowReminderForm(e.AlertNotifications);
				}
			}
		}
		protected internal virtual void InitializeViewsReadonlyProperties() {
			OverrideDefaultView(DayViewProperty, Views.DayView);
			OverrideDefaultView(WorkWeekViewProperty, Views.WorkWeekView);
			OverrideDefaultView(FullWeekViewProperty, Views.FullWeekView);
			OverrideDefaultView(WeekViewProperty, Views.WeekView);
			OverrideDefaultView(MonthViewProperty, Views.MonthView);
			OverrideDefaultView(TimelineViewProperty, Views.TimelineView);
			InnerControl.UpdateTimeMarkerVisibilityFromOptionBehavior();
			InnerControl.ActiveViewType = ActiveViewType;
			InnerControl.GroupType = GroupType;
		}
		protected internal virtual void OverrideDefaultView(DependencyProperty viewProperty, SchedulerViewBase defaultView) {
			SchedulerViewBase assignedView = (SchedulerViewBase)GetValue(viewProperty);
			if (assignedView != null) {
#if !SL
				assignedView.InitializeProperties();
#endif
				ReplaceView(assignedView);
			} else {
#if !SL
				defaultView.InitializeProperties();
#endif
				SetValue(viewProperty, defaultView);
				defaultView.PropertySyncManager.Activate();
				defaultView.PropertySyncManager.CommitDeferredChanges();
			}
		}
		protected internal virtual void SetupActiveViewTypePropertyBinding() {
			SetBinding(ActiveViewTypeProperty, InnerBindingHelper.CreateTwoWayPropertyBinding(InnerControl, "ActiveViewType"));
		}
		protected internal virtual void SetupGroupTypePropertyBinding() {
			Binding binding = InnerBindingHelper.CreateTwoWayPropertyBinding(InnerControl, "GroupType");
			SetBinding(GroupTypeProperty, binding);
		}
		protected internal virtual void OnActiveViewChanging(object sender, InnerActiveViewChangingEventArgs e) {
			SchedulerViewBase newView = Views[e.NewView.Type];
			e.Cancel = !RaiseActiveViewChanging(newView);
		}
		protected internal virtual void OnBeforeActiveViewChange(object sender, EventArgs e) {
			DestroyDateTimeScrollController();
		}
		protected internal virtual void OnAfterActiveViewChange(object sender, EventArgs e) {
			OnAfterActiveViewChangeCore();
		}
		internal void OnAfterActiveViewChangeCore() {
			UpdateActiveViewPropertyValue();
			CreateDateTimeScrollController();
		}
		protected internal virtual void UpdateActiveViewPropertyValue() {
			SchedulerViewBase newActiveView = Views[InnerControl.ActiveViewType];
			if (this.IsKeyboardFocusWithin)
				Focus();
			this.SetValue(ActiveViewPropertyKey, newActiveView);
		}
		protected internal virtual AppointmentSelectionController CreateAppointmentSelectionController() {
			return new WpfAppointmentSelectionController(this, true);
		}
		protected internal virtual void OnViewAssigned(SchedulerViewBase newViewValue) {
			if (!IsInitialized)
				return;
			ReplaceView(newViewValue);
			((IInnerSchedulerControlOwner)this).RecalcPreliminaryLayout();
		}
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
		protected internal virtual void ReplaceView(SchedulerViewBase newViewValue) {
			SchedulerViewBase oldView = Views.ReplaceView(this, newViewValue);
			if (oldView != null)
				RemoveLogicalChild(oldView);
			if (newViewValue != null)
				AddLogicalChild(newViewValue);
		}
		public override void BeginInit() {
			base.BeginInit();
			loadingLocker.Lock();
			InnerControl.BeginUpdate();
			LockStorageUpdate(Storage);
		}
		public override void EndInit() {
			PropertySyncManager.Activate();
			base.EndInit();			
			loadingLocker.Unlock();
			UnlockStorageUpdate(Storage);
			InnerControl.EndUpdate();
		}
		protected internal virtual void LockStorageUpdate(SchedulerStorage storage) {
			if (storage != null && !storage.IsUpdateLocked)
				storage.BeginUpdate();
		}
		protected internal virtual void UnlockStorageUpdate(SchedulerStorage storage) {
			if (storage != null && storage.IsUpdateLocked)
				storage.EndUpdate();
		}
		protected internal virtual void CancelStorageUpdate(SchedulerStorage storage) {
			if (storage != null && storage.IsUpdateLocked)
				storage.CancelUpdate();
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			CompleteInitialization(null);
		}
		protected internal virtual void CompleteInitialization(object storage) {
			XtraSchedulerDebug.Assert(IsInitialized);
			if (Storage == null) {
				Storage = new SchedulerStorage();
				Storage.InitializeProperties();
			}
			SetupInnerDataContextBinding();
			XtraSchedulerDebug.CheckNotNull("Storage", Storage);
			if (OptionsCustomization == null)
				OptionsCustomization = new Scheduler.OptionsCustomization();
			if (OptionsBehavior == null)
				OptionsBehavior = new Scheduler.OptionsBehavior();
			if (OptionsView == null)
				OptionsView = new Scheduler.OptionsView();
			if (DragDropOptions == null)
				DragDropOptions = new DragDropOptions();
#if !SL
			if (OptionsRangeControl == null)
				OptionsRangeControl = new SchedulerOptionsRangeControl();
#endif
			InitializeViewsReadonlyProperties();
			SynchronizeToInnerResourceColorSchemas();
			UpdateActiveViewPropertyValue();
		}
		protected internal virtual void ApplyChangesCore(SchedulerControlChangeType changeType, ChangeActions actions) {
			innerControl.ApplyChangesCore(changeType, actions);
		}
		#region IInnerSchedulerControlOwner implementation
#if SL
		bool IInnerSchedulerControlOwner.LeftButtonPressed { get { return LeftButtonPressed; } }
#endif
		ISupportAppointmentEdit IInnerSchedulerControlOwner.SupportAppointmentEdit { get { return this; } }
		ISupportAppointmentDependencyEdit IInnerSchedulerControlOwner.SupportAppointmentDependencyEdit { get { return this; } }
		SchedulerMouseHandler IInnerSchedulerControlOwner.CreateMouseHandler(InnerSchedulerControl control) {
			return this.CreateMouseHandler();
		}
		AppointmentChangeHelper IInnerSchedulerControlOwner.CreateAppointmentChangeHelper(InnerSchedulerControl innerSchedulerControl) {
			return this.CreateAppointmentChangeHelper();
		}
		NormalKeyboardHandlerBase IInnerSchedulerControlOwner.CreateKeyboardHandler() {
			return new XpfNormalKeyboardHandler();
		}
		SchedulerOptionsBehaviorBase IInnerSchedulerControlOwner.CreateOptionsBehavior() {
			return new SchedulerOptionsBehavior();
		}
		SchedulerOptionsViewBase IInnerSchedulerControlOwner.CreateOptionsView() {
			return new SchedulerOptionsView();
		}
		SchedulerViewRepositoryBase IInnerSchedulerControlOwner.CreateViewRepository() {
			return new SchedulerViewRepository();
		}
		ISchedulerInplaceEditController IInnerSchedulerControlOwner.CreateInplaceEditController() {
			return this.CreateInplaceEditController();
		}
		AppointmentSelectionController IInnerSchedulerControlOwner.CreateAppointmentSelectionController() {
			return CreateAppointmentSelectionController();
		}
		void IInnerSchedulerControlOwner.UpdatePaintStyle() {
		}
		bool IInnerSchedulerControlOwner.IsDateTimeScrollbarVisibilityDependsOnClientSize() {
			return false;
		}
		void IInnerSchedulerControlOwner.RecalcClientBounds() {
		}
		bool IInnerSchedulerControlOwner.ChangeResourceScrollBarOrientationIfNeeded() {
			return false;
		}
		bool IInnerSchedulerControlOwner.ChangeDateTimeScrollBarOrientationIfNeeded() {
			return false;
		}
		void IInnerSchedulerControlOwner.EnsureCalculationsAreFinished() {
		}
		bool IInnerSchedulerControlOwner.ChangeResourceScrollBarVisibilityIfNeeded() {
			return false;
		}
		bool IInnerSchedulerControlOwner.ChangeDateTimeScrollBarVisibilityIfNeeded() {
			return false;
		}
		void IInnerSchedulerControlOwner.RecalcViewBounds() {
		}
		void IInnerSchedulerControlOwner.RecalcScrollBarVisibility() {
			if (ActiveView != null)
				ActiveView.RecalcScrollBarVisibility();
		}
		void IInnerSchedulerControlOwner.RecalcFinalLayout() {
			CommandManager.UpdateBarItemsState();
		}
		void IInnerSchedulerControlOwner.RecalcPreliminaryLayout() {
			if (ActiveView != null)
				ActiveView.RecalcPreliminaryLayout();
		}
		void IInnerSchedulerControlOwner.ClearPreliminaryAppointmentsAndCellContainers() {
		}
		void IInnerSchedulerControlOwner.RecreateViewInfo() { 
		}
		ChangeActions IInnerSchedulerControlOwner.PrepareChangeActions() {
			return ChangeActions.RecalcPreliminaryLayout;
		}
		void IInnerSchedulerControlOwner.RecalcAppointmentsLayout() {
			if (ActiveView != null)
				ActiveView.RecalcAppointmentsLayout();
		}
		void IInnerSchedulerControlOwner.RecalcDraggingAppointmentPosition() {
			if (ActiveView != null)
				ActiveView.RecalcDraggingAppointmentPosition();
		}
		bool IInnerSchedulerControlOwner.ObtainDateTimeScrollBarVisibility() {
			return false;
		}
		void IInnerSchedulerControlOwner.UpdateScrollBarsPosition() {
		}
		void IInnerSchedulerControlOwner.UpdateDateTimeScrollBarValue() {
			this.UpdateDateTimeScrollBarValue();
		}
		void IInnerSchedulerControlOwner.UpdateResourceScrollBarValue() {
			RaiseResourceScrollBarValueChanged();
		}
		void IInnerSchedulerControlOwner.RepaintView() {
		}
		void IInnerSchedulerControlOwner.UpdateScrollMoreButtonsVisibility() {
		}
		ISchedulerStorageBase IInnerSchedulerControlOwner.Storage {
			get { return Storage != null ? Storage.InnerStorage : null; }
		}
		event EventHandler ICommandAwareControl<SchedulerCommandId>.BeforeDispose { add {; } remove {; } }
		void IInnerSchedulerControlOwner.ShowPrintPreview() {
		}
		#endregion
		protected virtual SchedulerMouseHandler CreateMouseHandler() {
			return new XpfSchedulerMouseHandler(this);
		}
		protected virtual AppointmentChangeHelper CreateAppointmentChangeHelper() {
			AppointmentChangeHelper result;
			if (DragDropOptions == null || DragDropOptions.MovementType == MovementType.SnapToCells)
				result = new AppointmentChangeHelper(InnerControl);
			else
				result = new XpfAppointmentChangeHelper(this);
			return result;
		}
		RequestVisualAppointmentInfoEventHandler onRequestVisualAppointmentInfo;
		protected internal event RequestVisualAppointmentInfoEventHandler RequestVisualAppointmentInfo {
			add {
				onRequestVisualAppointmentInfo += value;
			}
			remove {
				onRequestVisualAppointmentInfo -= value;
			}
		}
		protected internal virtual void RaiseRequestVisualAppointmentInfo(RequestVisualAppointmentInfoEventArgs e) {
			if (onRequestVisualAppointmentInfo != null)
				onRequestVisualAppointmentInfo(this, e);
		}
		void OnRequestVisualAppointmentInfo(object sender, RequestVisualAppointmentInfoEventArgs e) {
			RaiseRequestVisualAppointmentInfo(e);
		}
		protected virtual SchedulerInplaceEditControllerEx CreateInplaceEditController() {
			return new SchedulerInplaceEditControllerEx(InnerControl);
		}
		public SchedulerColorSchemaCollection GetResourceColorSchemasCopy() {
			SchedulerColorSchemaCollection result = new SchedulerColorSchemaCollection();
			result.Assign(InnerControl.ResourceColorSchemas);
			return result;
		}
		protected virtual void SynchronizeToInnerResourceColorSchemas() {
			if (InnerControl == null)
				return;
			ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> innerColorSchemas = InnerControl.ResourceColorSchemas;
			if (innerColorSchemas == null || innerColorSchemas.Count != 0)
				return;
			IBatchUpdateable schemaCollection = (IBatchUpdateable)innerColorSchemas;
			schemaCollection.BeginUpdate();
			try {
				innerColorSchemas.Clear();
				foreach (SchedulerColorSchema schema in DefaultResourceColorSchemas)
					innerColorSchemas.Add(schema.Clone());
			} finally {
				schemaCollection.EndUpdate();
			}
		}
		#region ISupportAppointmentEdit Members
		void ISupportAppointmentEdit.BeginEditNewAppointment(Appointment apt) {
			AppointmentChangeHelper.BeginEditNewAppointment(apt);
		}
		void ISupportAppointmentEdit.RaiseInitNewAppointmentEvent(Appointment apt) {
			AppointmentEventArgs args = new AppointmentEventArgs(apt);
			RaiseInitNewAppointment(args);
		}
		void ISupportAppointmentEdit.SelectNewAppointment(Appointment apt) {
			ActiveView.SelectAppointment(apt);
		}
		void ISupportAppointmentEdit.ShowAppointmentForm(Appointment apt, bool openRecurrenceForm, bool readOnly, CommandSourceType commandSourceType) {
			ShowEditAppointmentForm(apt, readOnly, commandSourceType, openRecurrenceForm);
		}
		RecurrentAppointmentAction IInnerSchedulerControlOwner.ShowDeleteRecurrentAppointmentForm(Appointment apt) {
			return RecurrentAppointmentAction.Cancel;
		}
		public virtual void ShowEditRecurrentAppointmentForm(Appointment appointment, bool readOnly) {
			ShowEditRecurrentAppointmentForm(appointment, readOnly, CommandSourceType.Unknown);
		}
		void ShowEditRecurrentAppointmentForm(Appointment appointment, bool readOnly, CommandSourceType commandSourceType) {
			if (ActualFormCustomizationUsingMVVM) {
				if(appointment == null || appointment.IsBase)
					XtraScheduler.Native.Exceptions.ThrowArgumentException("appointment", appointment);
				if(this.manageRecurrentAppointmentDialogService == null)
					return;
				EditRecurrentAppointmentFormEventArgs args = new EditRecurrentAppointmentFormEventArgs(appointment, readOnly, commandSourceType);
				RaiseEditRecurrentAppointmentFormShowing(args);
				if(args.Cancel)
					return;
				EditRecurrentAppointmentDialogViewModel viewModel = args.ViewModel != null ? (args.ViewModel as EditRecurrentAppointmentDialogViewModel) : EditRecurrentAppointmentDialogViewModel.Create(this, appointment, readOnly);
				if(viewModel != null) {
					AssignableDialogServiceHelper.DoServiceAction(this, manageRecurrentAppointmentDialogService, service => {
						service.ShowDialog(viewModel.CommandsCollection, null, viewModel);
					});
				}
			} else {
				FormManager.ShowEditRecurrentAppointmentForm(appointment, readOnly, commandSourceType);
			}
		}
		RecurrentAppointmentAction IInnerSchedulerControlOwner.ShowDeleteRecurrentAppointmentsForm(AppointmentBaseCollection apts) {
			if (ActualFormCustomizationUsingMVVM) {
				ShowDeleteRecurrentAppointmentsDialog(apts);
			} else {
				FormManager.ShowDeleteRecurrentAppointmentForm(apts);
			}
			return RecurrentAppointmentAction.Cancel;
		}
		RecurrentAppointmentAction IInnerSchedulerControlOwner.ShowEditRecurrentAppointmentForm(Appointment apt, bool readOnly, CommandSourceType commandSourceType) {
			if (ActualFormCustomizationUsingMVVM) {
				ShowEditRecurrentAppointmentForm(apt, readOnly, commandSourceType);
			} else {
				FormManager.ShowEditRecurrentAppointmentForm(apt, readOnly, commandSourceType);
			}
			return RecurrentAppointmentAction.Cancel;
		}
		bool IInnerSchedulerControlOwner.QueryDeleteForEachRecurringAppointment { get { return false; } }
		#endregion
		internal void ShowDeleteRecurrentAppointmentsDialog(AppointmentBaseCollection apts) {
			if(manageRecurrentAppointmentDialogService == null)
				return;
			DeleteRecurrentAppointmentFormEventArgs args = new DeleteRecurrentAppointmentFormEventArgs(apts);
			RaiseDeleteRecurrentAppointmentFormShowing(args);
			if(args.Cancel)
				return;
			DeleteRecurrentAppointmentDialogViewModel viewModel = args.ViewModel != null ? (args.ViewModel as DeleteRecurrentAppointmentDialogViewModel) : DeleteRecurrentAppointmentDialogViewModel.Create(this.Storage, apts);
			if(viewModel != null) {
				AssignableDialogServiceHelper.DoServiceAction(this, manageRecurrentAppointmentDialogService, service => {
					service.ShowDialog(viewModel.CommandsCollection, null, viewModel);
				});
			}
		}
		public virtual void ShowEditAppointmentForm(Appointment apt) {
			ShowEditAppointmentForm(apt, false);
		}
		public virtual void ShowEditAppointmentForm(Appointment apt, bool readOnly) {
			ShowEditAppointmentForm(apt, readOnly, CommandSourceType.Unknown, false);
		}
		internal virtual void ShowEditAppointmentForm(Appointment apt, bool readOnly, CommandSourceType commandSourceType, bool openRecurrenceDialog = false) {
			Guard.ArgumentNotNull(apt, "apt");
			ISchedulerStateService stateService = this.GetService<ISchedulerStateService>();
			if (stateService.AreAppointmentsDragged || stateService.IsAppointmentResized)
				return;
			if (ActualFormCustomizationUsingMVVM) {
				if(appointmentFormService == null)
					return;
				EditAppointmentFormEventArgs args = new EditAppointmentFormEventArgs(apt, readOnly, openRecurrenceDialog, commandSourceType);
				RaiseEditAppointmentFormShowing(args);
				if(args.Cancel)
					return;
				AppointmentFormViewModel viewModel = args.ViewModel != null ? (args.ViewModel as AppointmentFormViewModel) : AppointmentFormViewModel.Create(this, apt, readOnly, openRecurrenceDialog);
				if(viewModel != null) {
					AssignableDocumentServiceHelper.DoServiceAction(this, appointmentFormService, service => {
						IDocument document = service.CreateDocument(viewModel);
						viewModel.ActiveDocument = document;						
						document.DestroyOnClose = true;
						ISetSchedulerStateService setStateService = GetService<ISetSchedulerStateService>();
						if(setStateService != null)
							setStateService.IsModalFormOpened = true;
						document.Show();
					});
				}
			} else {
				FormManager.ShowEditAppointmentForm(apt, readOnly, commandSourceType, openRecurrenceDialog);
			}
			MouseHandler.SetCurrentCursor(CursorTypes.Default);
		}
		public virtual void ShowRecurrenceForm(UserControl parentForm, bool readOnly) {
			if (!ActualFormCustomizationUsingMVVM)
				FormManager.ShowRecurrenceForm(parentForm, readOnly);
		}
		public virtual void ShowRecurrenceForm(AppointmentFormViewModel parentFormViewModel, bool readOnly) {
			if(appointmentRecurrenceDialogService == null)
				return;
			RecurrenceFormEventArgs args = new RecurrenceFormEventArgs(parentFormViewModel, readOnly);
			RaiseRecurrenceFormShowing(args);
			if(args.Cancel)
				return;
			Window aptWindow = null;
#if !DEBUGTEST
			foreach (Window window in Application.Current.Windows) {
				if (window is DXWindow) {
					ContentPresenter presenter = window.Content as ContentPresenter;
					if (presenter != null) {
						AppointmentFormViewModel aptViewModel = presenter.Content as AppointmentFormViewModel;
						if (aptViewModel == parentFormViewModel)
							aptWindow = window;
					}
				}
			}
#endif
			RecurrenceDialogViewModel viewModel = args.ViewModel != null ? (args.ViewModel as RecurrenceDialogViewModel) : RecurrenceDialogViewModel.Create(parentFormViewModel, readOnly);
			if(viewModel != null) {
				AssignableDialogServiceHelper.DoServiceAction(aptWindow == null ? (DependencyObject)this : aptWindow, appointmentRecurrenceDialogService, service => {
					service.ShowDialog(viewModel.CommandsCollection, null, viewModel);
				});
			}
		}
		public virtual void ShowGotoDateForm(DateTime date) {
			if (ActualFormCustomizationUsingMVVM) {
				if(gotoDateDialogService == null)
					return;
				GotoDateFormEventArgs args = new GotoDateFormEventArgs(Views, date, ActiveViewType);
				RaiseGotoDateFormShowing(args);
				if(args.Cancel)
					return;
				GotoDateViewModel viewModel = args.ViewModel != null ? (args.ViewModel as GotoDateViewModel) : GotoDateViewModel.Create(this, Views, date, ActiveViewType);
				if(viewModel != null) {
					AssignableDialogServiceHelper.DoServiceAction(this, gotoDateDialogService, service => {
						service.ShowDialog(viewModel.CommandsCollection, null, viewModel);
					});
				}
			} else {
				FormManager.ShowGotoDateForm(date);
			}
		}
		public virtual void ShowCustomizeTimeRulerForm(TimeRuler ruler) {
			if (ActualFormCustomizationUsingMVVM) {
				if(timeRulerDialogService == null)
					return;
				CustomizeTimeRulerFormEventArgs args = new CustomizeTimeRulerFormEventArgs(ruler);
				args.AllowResize = false;
				RaiseCustomizeTimeRulerFormShowing(args);
				if (args.Cancel)
					return;
				TimeRulerViewModel viewModel = args.ViewModel != null ? (args.ViewModel as TimeRulerViewModel) : TimeRulerViewModel.Create(args.TimeRuler);
				if(viewModel != null) {
					AssignableDialogServiceHelper.DoServiceAction(this, timeRulerDialogService, service => {
						service.ShowDialog(viewModel.CommandsCollection, null, viewModel);
					});
				}
			} else {
				FormManager.ShowCustomizeTimeRulerForm(ruler);
			}
		}
		void ISupportAppointmentDependencyEdit.ShowAppointmentDependencyForm(AppointmentDependency dep, bool readOnly, CommandSourceType commandSourceType) {
		}		
#region Keyboard Support
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			if (FormManager.IsFormExceptReminderOpen)
				return;
			IKeyboardHandlerService service = GetService<IKeyboardHandlerService>();
			if (service != null && ShouldHandlePreviewKeyDown(e) && !Menu.IsOpen) {
				var args = e.ToPlatformIndependent();
				service.OnKeyDown(args);
				e.Handled = args.Handled;
			}
			base.OnPreviewKeyDown(e);
		}
		bool ShouldHandlePreviewKeyDown(KeyEventArgs e) {
			if (!IsBrowserHosted())
				return true;
			DependencyObject element = e.OriginalSource as DependencyObject;
			if (element != null)
				return !SchedulerFormBehavior.IsFormElement(element);
			return true;
		}
		bool IsBrowserHosted() {
#if WPF
			return BrowserInteropHelper.IsBrowserHosted;
#else
			return true;
#endif
		}
		protected override void OnPreviewKeyUp(KeyEventArgs e) {
			if (FormManager.IsFormExceptReminderOpen)
				return;
			IKeyboardHandlerService service = GetService<IKeyboardHandlerService>();
			if (service != null)
				service.OnKeyUp(e.ToPlatformIndependent());
			base.OnPreviewKeyUp(e);
		}
		protected override void OnTextInput(TextCompositionEventArgs e) {
			if (FormManager.IsFormExceptReminderOpen)
				return;
			IKeyboardHandlerService service = GetService<IKeyboardHandlerService>();
			if (service != null && e.Text != null && e.Text.Length == 1) {
				service.OnKeyPress(new PlatformIndepententKeyPressEventArgs(e.Text[0]));
			}
#if !SL
			base.OnPreviewTextInput(e);
#else
			base.OnPreviewTextInput(new DevExpress.Xpf.Editors.WPFCompatibility.SLTextCompositionEventArgs(e));
#endif
		}
#endregion
		protected virtual Lazy<BarManagerMenuController> CreateMenuController() {
			return new Lazy<BarManagerMenuController>(() => { return GridMenuInfoBase.CreateMenuController(Menu); });
		}
		protected virtual Lazy<BarManagerMenuController> CreateTimeRulerMenuController() {
			return new Lazy<BarManagerMenuController>(() => { return GridMenuInfoBase.CreateMenuController(Menu); });
		}
#if !SL
		protected override void OnContextMenuOpening(ContextMenuEventArgs e) {
			Visual source = e.OriginalSource as Visual;
			if (source == null)
				return;
			SchedulerLogger.Trace(XpfLoggerTraceLevel.Mouse, "->OnContextMenuOpening");
			ISchedulerStateService service = this.GetService<ISchedulerStateService>();
			if (!service.IsDataRefreshAllowed)
				return;
			DependencyObject commonAncestor = source.FindCommonVisualAncestor(this);
			if (commonAncestor == null)
				return;
			if (FormManager.IsFormExceptReminderOpen)
				return;
			if (Menu.IsOpen)
				Menu.ClosePopup();
			System.Windows.Point point = ((Visual)e.OriginalSource).TransformToVisual(this).Transform(new Point(e.CursorLeft, e.CursorTop));
			OnPopupMenu(XpfTypeConverter.FromPlatformPoint(point));
			e.Handled = true;
			base.OnContextMenuOpening(e);
		}
#else
#endif
		protected internal virtual void OnPopupMenu(System.Drawing.Point point) {
			ISchedulerStateService state = GetService<ISchedulerStateService>();
			if (state != null && state.IsAppointmentResized)
				return;
			MouseHandler.OnPopupMenu(point);
#if SL
			Menu.ClearItems();
#endif
			PopupMenu menu = CreatePopupMenu(point);
			menu.IsHitTestVisible = true;
			menu.FlowDirection = FlowDirection;
#if !SL
			menu.Placement = PlacementMode.MousePoint;
#else
			Menu.PlacementTarget = this;
			menu.Placement = PlacementMode.Top;
			Menu.Placement2 = PlacementMode2.Relative;
			Point relativePoint = this.MapPointFromScreen(XpfTypeConverter.ToPlatformPoint(point));
			Menu.HorizontalOffset = relativePoint.X;
			Menu.VerticalOffset = relativePoint.Y;
#endif
			menu.ShowPopup(this);
			System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
		}
		protected internal virtual PopupMenu CreatePopupMenu(System.Drawing.Point point) {
			ISchedulerHitInfo hitInfo = SchedulerHitInfo.CreateSchedulerHitInfo(this, XpfTypeConverter.ToPlatformPoint(point));
			SchedulerPopupMenuBuilder menuBuilder = CreatePopupMenuBuilder(hitInfo);
			SchedulerMenuBuilderInfo menuBuilderInfo = (SchedulerMenuBuilderInfo)menuBuilder.CreatePopupMenu();
			Menu.MenuBuilderInfo = menuBuilderInfo;
			return Menu;
		}
		protected internal virtual SchedulerPopupMenuBuilder CreatePopupMenuBuilder(ISchedulerHitInfo hitInfo) {
			return MouseHandler.CreatePopupMenuBuilder(hitInfo);
		}
		protected internal virtual void RaiseShowMenu(SchedulerMenuEventArgs e) {
			RaiseEvent(e);
		}
		protected internal virtual void OnAppointmentsSelectionChanged(object sender, EventArgs e) {
			ApplyChangesCore(SchedulerControlChangeType.SelectionChanged, ChangeActions.RecalcAppointmentsLayout);
			OnUpdateUI(this, EventArgs.Empty);
		}
		protected internal virtual void OnStorageChanged(object sender, EventArgs e) {
			RaiseStorageChanged();
		}
		protected virtual void CreateDateTimeScrollBar() {
			this.dateTimeScrollBarObject = new DateTimeScrollBar(this);
		}
		private void CreateDateTimeScrollController() {
			if (ActiveView != null && dateTimeScrollBarObject != null)
				this.dateTimeScrollController = ActiveView.CreateDateTimeScrollController();
		}
		protected virtual ScrollBar GetDateTimeScrollBar() {
			return GetTemplateChild(DateTimeScrollBarName) as ScrollBar;
		}
		protected internal virtual void OnDateTimeScroll(object sender, DateTimeScrollEventArgs e) {
			UnsubscribeDateTimeScrollBarEvents();
			try {
			} finally {
				SubscribeDateTimeScrollBarEvents();
			}
		}
		protected internal virtual void UpdateDateTimeScrollBarValue() {
			if (dateTimeScrollBarObject == null || dateTimeScrollController == null)
				return;
			UnsubscribeDateTimeScrollBarEvents();
			try {
				dateTimeScrollController.UpdateScrollBarPosition();
			} finally {
				SubscribeDateTimeScrollBarEvents();
			}
		}
		private void DestroyDateTimeScrollController() {
			if (this.dateTimeScrollController != null) {
				this.dateTimeScrollController.Dispose();
				this.dateTimeScrollController = null;
			}
		}
#region SubscribeDateTimeScrollBarEvents
		protected internal virtual void SubscribeDateTimeScrollBarEvents() {
			if (dateTimeScrollBarObject != null)
				dateTimeScrollBarObject.Scroll += OnDateTimeScroll;
		}
#endregion
#region UnsubscribeDateTimeScrollBarEvents
		protected internal virtual void UnsubscribeDateTimeScrollBarEvents() {
			if (dateTimeScrollBarObject != null)
				dateTimeScrollBarObject.Scroll -= OnDateTimeScroll;
		}
#endregion
		public override void OnApplyTemplate() {
			if (themeLoader != null) {
				themeLoader.Content = null;
			}
			themeLoader = GetTemplateChild("PART_ThemesLoader") as XPFContentControl;
#if !SL
#endif
			base.OnApplyTemplate();
			OnUpdateUI(this, EventArgs.Empty);
		}
		void OnActiveViewChanged(object sender, EventArgs e) {
			OnUpdateUI(this, EventArgs.Empty);
			this.deferredOnActiveViewChanged = true;
			RaiseActiveViewChanged();
		}
		void OnViewUIChanged(object sender, SchedulerViewUIChangedEventArgs e) {
			OnUpdateUI(this, EventArgs.Empty);
		}
		void OnGroupTypeChagend(object sender, EventArgs e) {
			OnUpdateUI(this, EventArgs.Empty);
		}
#if !SL
		void OnOptionsBehaviorChanged(object sender, BaseOptionChangedEventArgs e) {
			if (e.Name == "TouchAllowed")
				OnTouchAllowedChanged((bool)e.NewValue);
		}
#endif
		protected internal virtual IAppointmentStatus GetStatus(object statusId) {
			return Storage != null ? Storage.GetStatus(statusId) : AppointmentStatus.Empty;
		}
		protected internal virtual Color GetLabelColor(object labelId) {
			return Storage != null ? Storage.GetLabelColor(labelId) : Colors.White;
		}
#region ILogicalOwner Members
		List<object> logicalChildren = new List<object>();
		void ILogicalOwner.AddChild(object child) {
			this.logicalChildren.Add(child);
			AddLogicalChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			this.logicalChildren.Remove(child);
			RemoveLogicalChild(child);
		}
#endregion
		protected internal virtual void OnUnloaded(object sender, RoutedEventArgs e) {
#if SL
			if (IsThemeApplying || !IsLoadedInternal)
				return;
			IsLoadedInternal = false;
#endif
			if (Storage != null && Storage.RemindersEngine != null)
				Storage.RemindersEngine.Suspend();
			Menu.Reset();
			UnsubscribeFromEvents();
		}
		protected internal virtual void OnLoaded(object sender, RoutedEventArgs e) {
#if SL
			if (IsThemeApplying || IsLoadedInternal)
				return;
			IsLoadedInternal = true;
#endif
			if (Storage != null && Storage.RemindersEngine != null)
				Storage.RemindersEngine.Resume();
			Menu.Init();
			OnUpdateUI(this, EventArgs.Empty);
			UnsubscribeFromEvents();
			SubscribeToEvents();
#if !SL
			InternalUpdateBarManager();
#endif
#if SL && !DEBUG
#endif
			PropertyDeferredSetUp();
		}
		void PropertyDeferredSetUp() {
			if (this.propertyDeferredSetUpActions == null)
				return;
			foreach (var setPropertyAction in this.propertyDeferredSetUpActions)
				setPropertyAction();
			this.propertyDeferredSetUpActions.Clear();
		}
		bool CanDeferPropertySet() {
			return IsInitialized && !IsLoaded;
		}
		protected virtual void SubscribeToEvents() {
			SubscribeInnerControlEvents();
			SubscribeResourceColorSchemas();
		}
		protected virtual void UnsubscribeFromEvents() {
			UnsubscribeResourceColorSchemas();
			UnsubscribeInnerControlEvents();
		}
#if SL
		static void SubscribeThemeManager() {
			ThemeManager.ActualApplicationThemeChanged += OnApplicationThemeChanged;
			ThemeManager.ActualApplicationThemeChanging += OnApplicationThemeChanging;
		}
		void OnThemeChanging(DependencyObject sender, ThemeChangingRoutedEventArgs e) {
			IsThemeApplying = true;
			Unload();
			BeginInit();
		}
		void OnThemeChanged(DependencyObject sender, ThemeChangedRoutedEventArgs e) {
			EndInit();
			Dispatcher.BeginInvoke(new Action(() => IsThemeApplying = false));
		}
		static void OnApplicationThemeChanging(DependencyObject sender, ThemeChangingRoutedEventArgs e) {
			WeakReferenceRepository.DoAction<SchedulerControl>((scheduler) => scheduler.OnThemeChanging(sender, e));
		}
		static void OnApplicationThemeChanged(DependencyObject sender, ThemeChangedRoutedEventArgs e) {
			WeakReferenceRepository.DoAction<SchedulerControl>((scheduler) => scheduler.OnThemeChanged(sender, e));
		}
		protected internal virtual void Unload() {
			Views.Views.ForEach((view) => view.Unload());
		}
#endif
		void UnsubscribeResourceColorSchemas() {
			this.InnerControl.UnsubscribeResourceColorSchemasListenerEvents();
		}
		void SubscribeResourceColorSchemas() {
			this.InnerControl.UnsubscribeResourceColorSchemasListenerEvents();
			this.InnerControl.SubscribeResourceColorSchemasListenerEvents();
		}
#region IInnerSchedulerCommandTarget Members
		InnerSchedulerControl IInnerSchedulerCommandTarget.InnerSchedulerControl { get { return InnerControl; } }
#endregion
#region DragDrop support
		protected override void OnDragEnter(System.Windows.DragEventArgs e) {
			base.OnDragEnter(e);
			if (!this.isActualDragLeave)
				MouseHandler.OnDragEnter(e);
			else
				MouseHandler.OnDragOver(e);
			this.isActualDragLeave = false;
		}
		protected override void OnDragLeave(System.Windows.DragEventArgs e) {
			base.OnDragLeave(e);
			this.isActualDragLeave = true;
			Dispatcher.BeginInvoke(new DragLeaveDelegate(OnDragLeaveCore), e);
		}
		void OnDragLeaveCore(System.Windows.DragEventArgs e) {
			if (!this.isActualDragLeave) {
				return;
			}
			MouseHandler.OnDragLeave(e);
			this.isActualDragLeave = false;
		}
		protected override void OnDragOver(System.Windows.DragEventArgs e) {
			base.OnDragOver(e);
			MouseHandler.OnDragOver(e);
			e.Handled = true;
		}
		protected override void OnDrop(System.Windows.DragEventArgs e) {
			base.OnDrop(e);
			MouseHandler.OnDragDrop(e);
		}
#endregion
		protected internal void RegisterDraggedContent(UIElement content) {
			content.DragEnter += new DragEventHandler(content_DragEnter);
			content.DragLeave += new DragEventHandler(content_DragLeave);
			content.DragOver += new DragEventHandler(content_DragOver);
			content.Drop += new DragEventHandler(content_Drop);
		}
		protected internal void UnregisterDraggedContent(UIElement content) {
			content.DragEnter -= new DragEventHandler(content_DragEnter);
			content.DragLeave -= new DragEventHandler(content_DragLeave);
			content.DragOver -= new DragEventHandler(content_DragOver);
			content.Drop -= new DragEventHandler(content_Drop);
		}
		void content_Drop(object sender, System.Windows.DragEventArgs e) {
			OnDrop(e);
		}
		void content_DragOver(object sender, System.Windows.DragEventArgs e) {
			OnDragOver(e);
		}
		void content_DragLeave(object sender, System.Windows.DragEventArgs e) {
			OnDragLeave(e);
		}
		void content_DragEnter(object sender, System.Windows.DragEventArgs e) {
			OnDragEnter(e);
		}
#if !SL
		internal void InternalUpdateBarManager() {
			ForceElementNameBinding(this, BarManagerProperty);
			if (BarManager == null)
				return;
			foreach (BarItem item in BarManager.Items)
				ForceBarItemElementNameBinding(item);
			CommandManager.UpdateBarItemsState();
		}
		void ForceBarItemElementNameBinding(BarItem item) {
			ForceElementNameBinding(item, BarItem.CommandProperty);
			ForceElementNameBinding(item, BarItem.CommandParameterProperty);
			ForceElementNameBinding(item, BarItem.CommandTargetProperty);
		}
		internal void ForceElementNameBinding(DependencyObject o, DependencyProperty p) {
			BindingExpression bindingExpression = BindingOperations.GetBindingExpression(o, p);
			if (bindingExpression == null || bindingExpression.Status != BindingStatus.PathError)
				return;
			Binding binding = bindingExpression.ParentBinding;
			BindingOperations.ClearBinding(o, p);
			BindingOperations.SetBinding(o, p, binding);
		}
#endif
#region IMouseKeyStateSupport Members
		bool IMouseKeyStateSupport.LeftButtonPressed { get { return LeftButtonPressed; } }
		bool IMouseKeyStateSupport.RightButtonPressed { get { return RightButtonPressed; } }
#endregion
		protected internal virtual void OnRangeControlAutoAdjusting(object sender, RangeControlAdjustEventArgs e) {
			RaiseRangeControlAutoAdjusting(e);
		}
		internal void NotifyRangeControlAutoAdjusting(RangeControlAdjustEventArgs e) {
			OnRangeControlAutoAdjusting(this, e);
		}
		internal void SetFocus() {
		}
	}
#endregion
	public class VisualizatoinStatistics {
		public VisualizatoinStatistics() {
			Reset();
		}
		public int MaxCellHeight { get; set; }
		public int MinAppointmentHeight { get; set; }
		public void Reset() {
			MaxCellHeight = 100;
			MinAppointmentHeight = 0;
		}
		public bool IsActual() {
			return MaxCellHeight > 0 && MinAppointmentHeight > 0;
		}
		public void UpdateAppointmentRect(Rect rect) {
			int height = (int)rect.Height;
			if (MinAppointmentHeight <= 0)
				MinAppointmentHeight = height;
			else
				MinAppointmentHeight = Math.Min(height, MinAppointmentHeight);
		}
		public void UpdateCellRect(Rect rect) {
			int height = (int)rect.Height;
			if (MaxCellHeight <= 0)
				MaxCellHeight = height;
			else
				MaxCellHeight = Math.Max(height, MaxCellHeight);
		}
	}
	public class ViewRepositoryItemsControl : ItemsControl {
		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
		}
		protected override Size MeasureOverride(Size availableSize) {
			return new Size(1, 1);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			return base.ArrangeOverride(finalSize);
		}
	}
	public interface IMouseKeyStateSupport {
		bool LeftButtonPressed { get; }
		bool RightButtonPressed { get; }
	}
}
namespace DevExpress.XtraScheduler.Native {
	using DevExpress.Xpf.Scheduler.Services;
	public class XpfNormalKeyboardHandler : NormalKeyboardHandlerBase {
		public SchedulerViewBase View { get { return (SchedulerViewBase)InnerView.Owner; } }
		protected internal SchedulerControl Control { get { return View.Control; } }
		protected internal override InnerSchedulerControl InnerControl { get { return Control.InnerControl; } }
		protected internal override ISchedulerCommandTarget ISchedulerCommandTarget { get { return Control; } }
		public override Command GetKeyHandler(Keys keyData) {
			if (Control.FlowDirection == FlowDirection.RightToLeft)
				keyData = SwapKeysForRightToLeft(keyData);
			return base.GetKeyHandler(keyData);
		}
		protected virtual Keys SwapKeysForRightToLeft(Keys keyData) {
			int key = (int)keyData;
			int keyRight = (int)Keys.Right;
			int keyLeft = (int)Keys.Left;
			if ((key & keyRight) == keyRight)
				return (Keys)((key ^ keyRight) | keyLeft);
			if ((key & keyLeft) == keyLeft)
				return (Keys)((key ^ keyLeft) | keyRight);
			return keyData;
		}
	}
	public class XpfInnerSchedulerControl : InnerSchedulerControl {
		public XpfInnerSchedulerControl(IInnerSchedulerControlOwner owner)
			: base(owner) {
		}
		[Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SchedulerColorSchemaCollection ResourceColorSchemas { get { return (SchedulerColorSchemaCollection)base.ResourceColorSchemas; } }
		protected internal override void InitializeKeyboardHandlerDefaults(NormalKeyboardHandlerBase keyboardHandler) {
			base.InitializeKeyboardHandlerDefaults(keyboardHandler);
			SchedulerViewRepositoryBase views = this.Views;
			keyboardHandler.RegisterKeyHandlerForAllViewsAndGroupTypes(views, Keys.Tab, Keys.None, SchedulerCommandId.SelectNextAppointment);
			keyboardHandler.RegisterKeyHandlerForAllViewsAndGroupTypes(views, Keys.Tab, Keys.Control, SchedulerCommandId.MoveFocusNext); 
			keyboardHandler.RegisterKeyHandlerForAllViewsAndGroupTypes(views, Keys.Tab, Keys.Control | Keys.Shift, SchedulerCommandId.MoveFocusPrev); 
			keyboardHandler.RegisterKeyHandlerForAllViewsAndGroupTypes(views, Keys.Tab, Keys.Shift, SchedulerCommandId.SelectPrevAppointment);
		}
		protected override object CreateSchedulerCommandFactoryService() {
			return new XpfSchedulerCommandFactoryService(this);
		}
		protected internal override void UpdateRulersClientTimeZone() {
			base.UpdateRulersClientTimeZone();
			ApplyChanges(SchedulerControlChangeType.PerformViewLayoutChanged);
		}
		protected internal override object XtraCreateResourceColorSchemasItem(XtraItemEventArgs e) {
			return new SchedulerColorSchema();
		}
		protected override ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> CreateResourceColorSchemaCollection() {
			return new SchedulerColorSchemaCollection();
		}
		protected override ICollectionChangedListener CreateResourceColorSchemaChangedListener() {
			return new ResourceColorSchemasChangedListener(ResourceColorSchemas);
		}
#pragma warning disable 618
		protected internal override void UpdateTimeMarkerVisibilityFromOptionBehavior() {
			base.UpdateTimeMarkerVisibilityFromOptionBehavior();
			SchedulerControl control = Owner as SchedulerControl;
			if (control == null)
				return;
			UpdateTimeIndicatorVisibility(control.DayView);
			UpdateTimeIndicatorVisibility(control.WorkWeekView);
			UpdateTimeIndicatorVisibility(control.FullWeekView);
			UpdateTimeIndicatorVisibility(control.TimelineView);
		}		
		void UpdateTimeIndicatorVisibility(SchedulerViewBase view) {
			if (view == null)
				return;
			TimeIndicatorVisibility timeIndicatorVisibility = ConvertToTimeIndicatorVisibility(OptionsBehavior.ShowCurrentTime, view);
			if (OptionsBehavior.ShowCurrentTime == CurrentTimeVisibility.Auto && GetShowCurrentTimeInAllColumnsOption(view))
				return;
			DayView dayView = view as DayView;
			if (dayView != null && dayView.TimeIndicatorDisplayOptions.Visibility != timeIndicatorVisibility) {
				dayView.TimeIndicatorDisplayOptions.SetCurrentValue(SchedulerTimeIndicatorDisplayOptions.VisibilityProperty, timeIndicatorVisibility);
				return;
			}
			TimelineView timelineView = view as TimelineView;
			if (timelineView != null && timelineView.TimeIndicatorDisplayOptions.Visibility != timeIndicatorVisibility)
				timelineView.TimeIndicatorDisplayOptions.SetCurrentValue(SchedulerTimeIndicatorDisplayOptions.VisibilityProperty, timeIndicatorVisibility);
		}
		TimeIndicatorVisibility ConvertToTimeIndicatorVisibility(CurrentTimeVisibility visibility, SchedulerViewBase viewBase) {
			bool showCurrentTimeInAllColumns = GetShowCurrentTimeInAllColumnsOption(viewBase);
			switch (visibility) {
				case CurrentTimeVisibility.Always:
					return TimeIndicatorVisibility.Always;
				case CurrentTimeVisibility.Never:
					return TimeIndicatorVisibility.Never;
				default:
					if (!showCurrentTimeInAllColumns)
						return TimeIndicatorVisibility.CurrentDate;
					return TimeIndicatorVisibility.TodayView;					
			}
		}
		bool GetShowCurrentTimeInAllColumnsOption(SchedulerViewBase viewBase) {
			bool showCurrentTimeInAllColumns = true;
			DayView dayView = viewBase as DayView;
			if (dayView != null)
				showCurrentTimeInAllColumns = dayView.ShowCurrentTimeInAllColumns;
			return showCurrentTimeInAllColumns;
		}
#pragma warning restore 618
	}
	public class ResourceColorSchemasChangedListener : NotificationCollectionChangedListener<SchedulerColorSchema>, ICollectionChangedListener {
		public ResourceColorSchemasChangedListener(NotificationCollection<SchedulerColorSchema> collection)
			: base(collection) {
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Drawing {
#region DynamicBorder
	public class DynamicBorder : XPFContentControl {
#region VisibleBorderTemplate
		public static readonly DependencyProperty VisibleBorderTemplateProperty = DependencyPropertyManager.Register("VisibleBorderTemplate", typeof(ControlTemplate), typeof(DynamicBorder), new UIPropertyMetadata(null, new PlatformIndependentPropertyChangedCallback(OnVisibleBorderTemplateChanged)));
		public ControlTemplate VisibleBorderTemplate {
			get { return (ControlTemplate)GetValue(VisibleBorderTemplateProperty); }
			set { SetValue(VisibleBorderTemplateProperty, value); }
		}
		static void OnVisibleBorderTemplateChanged(DependencyObject d, PlatformIndependentDependencyPropertyChangedEventArgs e) {
			DynamicBorder instance = (DynamicBorder)d;
			instance.OnVisibleBorderTemplateChanged((ControlTemplate)e.OldValue, (ControlTemplate)e.NewValue);
		}
#endregion
#region InvisibleBorderTemplate
		public static readonly DependencyProperty InvisibleBorderTemplateProperty = DependencyPropertyManager.Register("InvisibleBorderTemplate", typeof(ControlTemplate), typeof(DynamicBorder), new UIPropertyMetadata(null, new PlatformIndependentPropertyChangedCallback(OnInvisibleBorderTemplateChanged)));
		public ControlTemplate InvisibleBorderTemplate {
			get { return (ControlTemplate)GetValue(InvisibleBorderTemplateProperty); }
			set { SetValue(InvisibleBorderTemplateProperty, value); }
		}
		static void OnInvisibleBorderTemplateChanged(DependencyObject d, PlatformIndependentDependencyPropertyChangedEventArgs e) {
			DynamicBorder instance = (DynamicBorder)d;
			instance.OnInvisibleBorderTemplateChanged((ControlTemplate)e.OldValue, (ControlTemplate)e.NewValue);
		}
#endregion
#region ShowBorder
		public static readonly DependencyProperty ShowBorderProperty = DependencyPropertyManager.Register("ShowBorder", typeof(bool), typeof(DynamicBorder), new UIPropertyMetadata(true, new PlatformIndependentPropertyChangedCallback(OnShowBorderChanged)));
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		static void OnShowBorderChanged(DependencyObject d, PlatformIndependentDependencyPropertyChangedEventArgs e) {
			DynamicBorder instance = (DynamicBorder)d;
			instance.OnShowBorderChanged((bool)e.OldValue, (bool)e.NewValue);
		}
#endregion
		protected internal virtual void OnVisibleBorderTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			SelectTemplate();
		}
		protected internal virtual void OnInvisibleBorderTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			SelectTemplate();
		}
		protected internal virtual void OnShowBorderChanged(bool oldValue, bool newValue) {
			SelectTemplate();
		}
		protected internal virtual void SelectTemplate() {
			ControlTemplate template = ShowBorder ? VisibleBorderTemplate : InvisibleBorderTemplate;
			if (!Object.ReferenceEquals(Template, template))
				Template = template;
		}
	}
#endregion
}
