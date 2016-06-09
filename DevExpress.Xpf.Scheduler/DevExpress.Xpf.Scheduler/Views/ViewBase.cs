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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Utils.Controls;
using DevExpress.Xpf.Scheduler.Drawing;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.XtraScheduler.Internal.Implementations;
#if SL
using DevExpress.Xpf.Scheduler.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#else
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#endif
namespace DevExpress.Xpf.Scheduler {
	#region SchedulerViewBase (abstract class)
	public abstract class SchedulerViewBase : SchedulerElement, IInnerSchedulerViewOwner, ISchedulerViewRepositoryItem, ISchedulerViewPropertiesBase {
		#region Fields
		static readonly TimeSpan defaultNavigationButtonAppointmentSearchInterval = TimeSpan.Zero;
		const NavigationButtonVisibility defaultNavigationButtonVisibility = NavigationButtonVisibility.Auto;
		SchedulerControl control;
		InnerSchedulerViewBase innerView;
		ViewFactoryHelper factoryHelper;
		DependencyPropertySyncManager propertySyncManager;
		#endregion
		protected SchedulerViewBase() {
			this.factoryHelper = CreateFactoryHelper();
			this.propertySyncManager = CreatePropertySyncManager();
			DefaultStyleKey = GetType();
		}
		private void SchedulerViewBase_Unloaded(object sender, RoutedEventArgs e) {
			throw new NotImplementedException();
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerControl Control { get { return control; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public abstract SchedulerViewType Type { get; }
		protected internal InnerSchedulerViewBase InnerView { get { return innerView; } }
		internal virtual double DraggedAppointmentHeightInternal { get { return 0; } }
		#region DisplayName
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseDisplayName")]
#endif
		public string DisplayName {
			get { return (string)GetValue(DisplayNameProperty); }
			set { SetValue(DisplayNameProperty, value); }
		}
		public static readonly DependencyProperty DisplayNameProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, string>("DisplayName", String.Empty, (d, e) => d.OnDisplayNameChanged(e.OldValue, e.NewValue), null);
		void OnDisplayNameChanged(string oldValue, string newValue) {
			UpdateInnerObjectPropertyValue(DisplayNameProperty, oldValue, newValue);
		}
		#endregion
		#region MenuCaption
		public String MenuCaption {
			get { return (String)GetValue(MenuCaptionProperty); }
			set { SetValue(MenuCaptionProperty, value); }
		}
		public static readonly DependencyProperty MenuCaptionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, String>("MenuCaption", String.Empty, (d, e) => d.OnMenuCaptionChanged(e.OldValue, e.NewValue), null);
		void OnMenuCaptionChanged(String oldValue, String newValue) {
			UpdateInnerObjectPropertyValue(MenuCaptionProperty, oldValue, newValue);
		}
		#endregion
		#region InnerVisibleIntervals
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal TimeIntervalCollection InnerVisibleIntervals {
			get {
				if (innerView != null)
					return innerView.InnerVisibleIntervals;
				else
					return null;
			}
		}
		#endregion
		#region LimitInterval
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal TimeInterval LimitInterval {
			get {
				if (innerView != null)
					return innerView.LimitInterval;
				else
					return null;
			}
			set {
				if (innerView != null)
					innerView.LimitInterval = value;
			}
		}
		#endregion
		#region Enabled
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseEnabled")]
#endif
		public bool Enabled {
			get { return (bool)GetValue(EnabledProperty); }
			set { SetValue(EnabledProperty, value); }
		}
		public static readonly DependencyProperty EnabledProperty = CreateEnabledProperty();
		static DependencyProperty CreateEnabledProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, bool>("Enabled", true, (d, e) => d.OnEnabledChanged(e.OldValue, e.NewValue));
		}
		void OnEnabledChanged(bool oldValue, bool newValue) {
			if (!RaisePropertyChanging("Enabled"))
				CoerceEnabledProperty(newValue);
			else
				RaisePropertyChanged<bool>("Enabled", oldValue, newValue);
		}
		protected void CoerceEnabledProperty(bool newValue) {
#if !SILVERLIGHT
			CoerceValue(EnabledProperty);  
#endif
		}
		#endregion
		#region ShowMoreButtons
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseShowMoreButtons")]
#endif
		public bool ShowMoreButtons {
			get { return (bool)GetValue(ShowMoreButtonsProperty); }
			set { SetValue(ShowMoreButtonsProperty, value); }
		}
		public static readonly DependencyProperty ShowMoreButtonsProperty = CreateShowMoreButtonsProperty();
		static DependencyProperty CreateShowMoreButtonsProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, bool>("ShowMoreButtons", true, (d, e) => d.OnShowMoreButtonsChanged(e.OldValue, e.NewValue));
		}
		void OnShowMoreButtonsChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(ShowMoreButtonsProperty, oldValue, newValue);
		}
		#endregion
		#region GroupType
		[Category(SRCategoryNames.View), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerGroupType GroupType {
			get { return (SchedulerGroupType)GetValue(GroupTypeProperty); }
			set { SetValue(GroupTypeProperty, value); }
		}
		public static readonly DependencyProperty GroupTypeProperty = CreateGroupTypeProperty();
		static DependencyProperty CreateGroupTypeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, SchedulerGroupType>("GroupType", SchedulerGroupType.None, (d, e) => d.OnGroupTypeChanged(e.OldValue, e.NewValue));
		}
		void OnGroupTypeChanged(SchedulerGroupType oldValue, SchedulerGroupType newValue) {
			RaisePropertyChanged<SchedulerGroupType>("GroupType", oldValue, newValue);
		}
		#endregion
		#region NavigationButtonVisibility
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseNavigationButtonVisibility"),
#endif
		DefaultValue(defaultNavigationButtonVisibility), XtraSerializableProperty(), NotifyParentProperty(true)]
		public NavigationButtonVisibility NavigationButtonVisibility {
			get { return (NavigationButtonVisibility)GetValue(NavigationButtonVisibilityProperty); }
			set { SetValue(NavigationButtonVisibilityProperty, value); }
		}
		public static readonly DependencyProperty NavigationButtonVisibilityProperty = CreateNavigationButtonVisibilityProperty();
		static DependencyProperty CreateNavigationButtonVisibilityProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, NavigationButtonVisibility>("NavigationButtonVisibility", defaultNavigationButtonVisibility, (d, e) => d.OnNavigationButtonVisibility(e.OldValue, e.NewValue));
		}
		void OnNavigationButtonVisibility(XtraScheduler.NavigationButtonVisibility oldValue, XtraScheduler.NavigationButtonVisibility newValue) {
			UpdateInnerObjectPropertyValue(NavigationButtonVisibilityProperty, oldValue, newValue);
		}
		#endregion
		#region NavigationButtonAppointmentSearchInterval
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseNavigationButtonAppointmentSearchInterval"),
#endif
XtraSerializableProperty(), NotifyParentProperty(true)]
		public TimeSpan NavigationButtonAppointmentSearchInterval {
			get { return (TimeSpan)GetValue(NavigationButtonAppointmentSearchIntervalProperty); }
			set { SetValue(NavigationButtonAppointmentSearchIntervalProperty, value); }
		}
		public static readonly DependencyProperty NavigationButtonAppointmentSearchIntervalProperty = CreateNavigationButtonAppointmentSearchIntervalProperty();
		static DependencyProperty CreateNavigationButtonAppointmentSearchIntervalProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, TimeSpan>("NavigationButtonAppointmentSearchInterval", defaultNavigationButtonAppointmentSearchInterval, (d, e) => d.OnNavigationButtonAppointmentSearchInterval(e.OldValue, e.NewValue));
		}
		void OnNavigationButtonAppointmentSearchInterval(TimeSpan oldValue, TimeSpan newValue) {
			UpdateInnerObjectPropertyValue(NavigationButtonAppointmentSearchIntervalProperty, oldValue, newValue);
		}
		#endregion
		#region ResourcesPerPage
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseResourcesPerPage")]
#endif
		public int ResourcesPerPage {
			get { return (int)GetValue(ResourcesPerPageProperty); }
			set { SetValue(ResourcesPerPageProperty, value); }
		}
		public static readonly DependencyProperty ResourcesPerPageProperty = CreateResourcesPerPageProperty();
		static DependencyProperty CreateResourcesPerPageProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, int>("ResourcesPerPage", 0, (d, e) => d.OnResourcesPerPagePropertyChanged(e.OldValue, e.NewValue));
		}
		private void OnResourcesPerPagePropertyChanged(int oldValue, int newValue) {
			UpdateInnerObjectPropertyValue(ResourcesPerPageProperty, oldValue, newValue);
		}
		#endregion
		#region NavigationButtonPrevStyle
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseNavigationButtonPrevStyle")]
#endif
		public Style NavigationButtonPrevStyle {
			get { return (Style)GetValue(NavigationButtonPrevStyleProperty); }
			set { SetValue(NavigationButtonPrevStyleProperty, value); }
		}
		public static readonly DependencyProperty NavigationButtonPrevStyleProperty = CreateNavigationButtonPrevStyleProperty();
		static DependencyProperty CreateNavigationButtonPrevStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, Style>("NavigationButtonPrevStyle", null);
		}
		#endregion
		#region NavigationButtonNextStyle
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseNavigationButtonNextStyle")]
#endif
		public Style NavigationButtonNextStyle {
			get { return (Style)GetValue(NavigationButtonNextStyleProperty); }
			set { SetValue(NavigationButtonNextStyleProperty, value); }
		}
		public static readonly DependencyProperty NavigationButtonNextStyleProperty = CreateNavigationButtonNextStyleProperty();
		static DependencyProperty CreateNavigationButtonNextStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, Style>("NavigationButtonNextStyle", null);
		}
		#endregion
		#region HorizontalResourceHeaderStyle
		public static readonly DependencyProperty HorizontalResourceHeaderStyleProperty = CreateHorizontalResourceHeaderStyleProperty();
		static DependencyProperty CreateHorizontalResourceHeaderStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, Style>("HorizontalResourceHeaderStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseHorizontalResourceHeaderStyle")]
#endif
		public Style HorizontalResourceHeaderStyle {
			get { return (Style)GetValue(HorizontalResourceHeaderStyleProperty); }
			set { SetValue(HorizontalResourceHeaderStyleProperty, value); }
		}
		#endregion
		#region VerticalResourceHeaderStyle
		internal const string VerticalResourceHeaderStylePropertyName = "VerticalResourceHeaderStyle";
		public static readonly DependencyProperty VerticalResourceHeaderStyleProperty = CreateVerticalResourceHeaderStyleProperty();
		static DependencyProperty CreateVerticalResourceHeaderStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, Style>("VerticalResourceHeaderStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseVerticalResourceHeaderStyle")]
#endif
		public Style VerticalResourceHeaderStyle {
			get { return (Style)GetValue(VerticalResourceHeaderStyleProperty); }
			set { SetValue(VerticalResourceHeaderStyleProperty, value); }
		}
		#endregion
		#region SelectionTemplate
		public static readonly DependencyProperty SelectionTemplateProperty = CreateSelectionTemplateProperty();
		static DependencyProperty CreateSelectionTemplateProperty() {
			return DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, ControlTemplate>("SelectionTemplate", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseSelectionTemplate")]
#endif
		public ControlTemplate SelectionTemplate {
			get { return (ControlTemplate)GetValue(SelectionTemplateProperty); }
			set { SetValue(SelectionTemplateProperty, value); }
		}
		#endregion
		#region AppointmentToolTipContentTemplate
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseAppointmentToolTipContentTemplate")]
#endif
		public DataTemplate AppointmentToolTipContentTemplate {
			get { return (DataTemplate)GetValue(AppointmentToolTipContentTemplateProperty); }
			set { SetValue(AppointmentToolTipContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty AppointmentToolTipContentTemplateProperty = CreateAppointmentToolTipContentTemplateProperty();
		static DependencyProperty CreateAppointmentToolTipContentTemplateProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, DataTemplate>("AppointmentToolTipContentTemplate", null);
		}
		#endregion
		#region AppointmentToolTipVisibility
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseAppointmentToolTipVisibility")]
#endif
		public ToolTipVisibility AppointmentToolTipVisibility {
			get { return (ToolTipVisibility)GetValue(AppointmentToolTipVisibilityProperty); }
			set { SetValue(AppointmentToolTipVisibilityProperty, value); }
		}
		public static readonly DependencyProperty AppointmentToolTipVisibilityProperty = CreateAppointmentToolTipVisibilityProperty();
		static DependencyProperty CreateAppointmentToolTipVisibilityProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, ToolTipVisibility>("AppointmentToolTipVisibility", ToolTipVisibility.Standard);
		}
		#endregion
		#region ContentStyleSelector
		public static readonly DependencyProperty ContentStyleSelectorProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, StyleSelector>("ContentStyleSelector", null, (d, e) => d.OnContentStyleSelectorPropertyChanged(e.OldValue, e.NewValue));
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseContentStyleSelector")]
#endif
		public StyleSelector ContentStyleSelector { get { return (StyleSelector)GetValue(ContentStyleSelectorProperty); } set { SetValue(ContentStyleSelectorProperty, value); } }
		protected virtual void OnContentStyleSelectorPropertyChanged(StyleSelector oldSelector, StyleSelector newSelector) {
			if (oldSelector == newSelector)
				return;
			if (VisualViewInfo == null)
				SetVisualViewInfo(ViewInfo);
			if (VisualViewInfo != null)
				VisualViewInfo.Style = newSelector.SelectStyle(VisualViewInfo, VisualViewInfo);
		}
		#endregion
		#region DragDropHoverTimeCellsStyle
		public static readonly DependencyProperty DragDropHoverTimeCellsStyleProperty = CreateDragDropHoverTimeCellsStyleProperty();
		static DependencyProperty CreateDragDropHoverTimeCellsStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, Style>("DragDropHoverTimeCellsStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseDragDropHoverTimeCellsStyle")]
#endif
		public Style DragDropHoverTimeCellsStyle {
			get { return (Style)GetValue(DragDropHoverTimeCellsStyleProperty); }
			set { SetValue(DragDropHoverTimeCellsStyleProperty, value); }
		}
		#endregion
		#region HorizontalAppointmentStyleSelector
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseHorizontalAppointmentStyleSelector")]
#endif
		public StyleSelector HorizontalAppointmentStyleSelector {
			get { return (StyleSelector)GetValue(HorizontalAppointmentStyleSelectorProperty); }
			set { SetValue(HorizontalAppointmentStyleSelectorProperty, value); }
		}
		public static readonly DependencyProperty HorizontalAppointmentStyleSelectorProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, StyleSelector>("HorizontalAppointmentStyleSelector", null, (d, e) => d.OnHorizontalAppointmentStyleSelectorChanged(e.OldValue, e.NewValue), null);
		void OnHorizontalAppointmentStyleSelectorChanged(StyleSelector oldValue, StyleSelector newValue) {
			if (oldValue == newValue)
				return;
		}
		#endregion
		#region HorizontalAppointmentContentTemplate
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseHorizontalAppointmentContentTemplate")]
#endif
		public DataTemplate HorizontalAppointmentContentTemplate {
			get { return (DataTemplate)GetValue(HorizontalAppointmentContentTemplateProperty); }
			set { SetValue(HorizontalAppointmentContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty HorizontalAppointmentContentTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, DataTemplate>("HorizontalAppointmentContentTemplate", null);
		#endregion
		#region HorizontalAppointmentContentTemplateSelector
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewBaseHorizontalAppointmentContentTemplateSelector")]
#endif
		public DataTemplateSelector HorizontalAppointmentContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(HorizontalAppointmentContentTemplateSelectorProperty); }
			set { SetValue(HorizontalAppointmentContentTemplateSelectorProperty, value); }
		}
		public static readonly DependencyProperty HorizontalAppointmentContentTemplateSelectorProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, DataTemplateSelector>("HorizontalAppointmentContentTemplateSelector", null);
		#endregion
		#region FactoryHelper
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal ViewFactoryHelper FactoryHelper { get { return factoryHelper; } }
		#endregion
		#region VisibleResources
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal ResourceBaseCollection VisibleResources {
			get {
				if (innerView != null)
					return innerView.VisibleResources;
				else
					return null;
			}
		}
		#endregion
		#region VisibleStart
		protected internal DateTime VisibleStart {
			get {
				if (innerView != null)
					return innerView.VisibleStart;
				else
					return DateTime.MinValue;
			}
		}
		#endregion
		#region ViewInfo
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty ViewInfoProperty =
			DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, ISchedulerViewInfoBase>("ViewInfo", null);
		protected internal ISchedulerViewInfoBase ViewInfo {
			get { return (ISchedulerViewInfoBase)GetValue(ViewInfoProperty); }
			set { SetValue(ViewInfoProperty, value); }
		}
		#endregion
		#region FilteredAppointments
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal AppointmentBaseCollection FilteredAppointments {
			get {
				if (innerView != null)
					return innerView.FilteredAppointments;
				else
					return null;
			}
		}
		#endregion
		#region FilteredResources
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal ResourceBaseCollection FilteredResources {
			get {
				if (innerView != null)
					return innerView.FilteredResources;
				else
					return null;
			}
		}
		#endregion
		#region SelectedInterval
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeInterval SelectedInterval {
			get {
				SchedulerViewSelection selection = Control.Selection;
				if (selection != null)
					return selection.Interval.Clone();
				else
					return TimeInterval.Empty;
			}
		}
		#endregion
		#region SelectedResource
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Resource SelectedResource {
			get {
				SchedulerViewSelection selection = Control.Selection;
				if (selection != null)
					return selection.Resource;
				else
					return ResourceBase.Empty;
			}
		}
		#endregion
		protected internal DependencyPropertySyncManager PropertySyncManager { get { return propertySyncManager; } }
		#region DeferredScrolling
		ISchedulerDeferredScrollingOption ISchedulerViewPropertiesBase.DeferredScrolling { get { return null; } set { } }
		#endregion
		#endregion
		protected abstract DependencyPropertySyncManager CreatePropertySyncManager();
		protected internal abstract InnerSchedulerViewBase CreateInnerView();
		protected abstract void InitializePropertiesWithDefaultValues();
#if !SL
		internal void InitializeProperties() {
			InitializePropertiesWithDefaultValues();
		}
#endif
		protected internal virtual void Initialize(SchedulerControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.innerView = CreateInnerView();
			this.innerView.Initialize(Control.InnerControl.Selection);
			SetupInnerViewBindings();
		}
		protected virtual bool CanSyncInnerObject() {
			return InnerView != null;
		}
		protected internal virtual void UpdateInnerObjectPropertyValue(DependencyProperty property, object oldValue, object newValue) {
			if (!CanSyncInnerObject())
				PropertySyncManager.StartDeferredChanges();
			if (!DesignerProperties.GetIsInDesignMode(this))
				PropertySyncManager.Update(property, oldValue, newValue);
		}
		#region VisualViewInfo
		public static readonly DependencyProperty VisualViewInfoProperty = CreateVisualViewInfoProperty();
		static DependencyProperty CreateVisualViewInfoProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerViewBase, VisualViewInfoBase>("VisualViewInfo", null);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public VisualViewInfoBase VisualViewInfo {
			get { return (VisualViewInfoBase)GetValue(VisualViewInfoProperty); }
			set { SetValue(VisualViewInfoProperty, value); }
		}
		#endregion
		void CreateAndPrepareViewInfo() {
			ISchedulerViewInfoBase viewInfo = FactoryHelper.SelectStrategy(InnerView).CreateViewInfo(this);
			viewInfo.Create();
			ViewInfo = viewInfo;
			CalculateAppointments();
			viewInfo.EndCreate();
			SetVisualViewInfo(viewInfo);
			if (Control.SelectedAppointments.Count <= 0 && Control.IsKeyboardFocusWithin)
				Control.Focus();
		}
		void CalculateAppointments() {
			AppointmentsLayoutCalculator calculator = CreateAppointmentsLayoutCalculator();
			calculator.LayoutAppointments();
			calculator.LayoutDraggedAppointments();
			ViewInfo.UpdateSelection(Control.Selection);
			ViewInfo.UpdateAppointmentsSelection();
		}
		protected virtual void CopyAppointmentsVisualViewInfo(ISchedulerViewInfoBase viewInfo) {
			VisualViewInfoBase destination = GetVisualViewInfo(viewInfo);
			if (destination == null)
				return;
			destination.CopyAppointmentsViewInfo(viewInfo);
		}
		protected virtual void SetVisualViewInfo(ISchedulerViewInfoBase viewInfo) {
			VisualViewInfoBase destination = GetVisualViewInfo(viewInfo);
			if (destination == null) {
				VisualViewInfo = null;
				return;
			}
			if (ContentStyleSelector == null) {
				VisualViewInfo = null;
				return;
			}
			PrepareVisualViewInfo(destination, viewInfo);
			if (!Object.ReferenceEquals(VisualViewInfo, destination)) {
				VisualViewInfo = destination;
				VisualViewInfo.Style = ContentStyleSelector.SelectStyle(VisualViewInfo, VisualViewInfo);				
			}			
		}
#if SL
		protected override Size MeasureOverride(Size availableSize) {
			return new Size(0, 0);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			return new Size(0, 0);			
		}
		protected internal virtual void Unload() {
			if (VisualViewInfo != null)
				VisualViewInfo.Unload();
			this.RemoveFromVisualTree();
		}
#endif
		void PrepareVisualViewInfo(VisualViewInfoBase visualViewInfo, ISchedulerViewInfoBase viewInfo) {
			((ISupportCopyFrom<ISchedulerViewInfoBase>)visualViewInfo).CopyFrom(viewInfo);
			if (VisualViewInfo == null)
				return;
			Control.RaiseCustomizeVisualViewInfo(VisualViewInfo);
		}
		protected virtual VisualViewInfoBase GetVisualViewInfo(ISchedulerViewInfoBase viewInfo) {
			if (viewInfo is DayViewGroupByNone) {
				if (VisualViewInfo is VisualDayViewGroupByNone)
					return VisualViewInfo;
				else
					return new VisualDayViewGroupByNone();
			}
			if (viewInfo is DayViewGroupByResource) {
				if (VisualViewInfo is VisualDayViewGroupByResource)
					return VisualViewInfo;
				else
					return new VisualDayViewGroupByResource();
			}
			if (viewInfo is DayViewGroupByDate) {
				if (VisualViewInfo is VisualDayViewGroupByDate)
					return VisualViewInfo;
				else
					return new VisualDayViewGroupByDate();
			}
			if (viewInfo is MonthViewGroupByNone) {
				if (VisualViewInfo is VisualMonthViewGroupByNone)
					return VisualViewInfo;
				else
					return new VisualMonthViewGroupByNone();
			}
			if (viewInfo is MonthViewGroupByResource) {
				if (VisualViewInfo is VisualMonthViewGroupByResource)
					return VisualViewInfo;
				else
					return new VisualMonthViewGroupByResource();
			}
			if (viewInfo is MonthViewGroupByDate) {
				if (VisualViewInfo is VisualMonthViewGroupByDate)
					return VisualViewInfo;
				else
					return new VisualMonthViewGroupByDate();
			}
			if (viewInfo is WeekViewGroupByNone) {
				if (VisualViewInfo is VisualWeekViewGroupByNone)
					return VisualViewInfo;
				else
					return new VisualWeekViewGroupByNone();
			}
			if (viewInfo is WeekViewGroupByResource) {
				if (VisualViewInfo is VisualWeekViewGroupByResource)
					return VisualViewInfo;
				else
					return new VisualWeekViewGroupByResource();
			}
			if (viewInfo is WeekViewGroupByDate) {
				if (VisualViewInfo is VisualWeekViewGroupByDate)
					return VisualViewInfo;
				else
					return new VisualWeekViewGroupByDate();
			}
			if (viewInfo is TimelineViewGroupByNone) {
				if (VisualViewInfo is VisualTimelineViewGroupByNone)
					return VisualViewInfo;
				else
					return new VisualTimelineViewGroupByNone();
			}
			if (viewInfo is TimelineViewGroupByResource) {
				if (VisualViewInfo is VisualTimelineViewGroupByDate)
					return VisualViewInfo;
				else
					return new VisualTimelineViewGroupByDate();
			}
			if (viewInfo is TimelineViewGroupByDate) {
				if (VisualViewInfo is VisualTimelineViewGroupByDate)
					return VisualViewInfo;
				else
					return new VisualTimelineViewGroupByDate();
			}
			if (viewInfo is GanttViewGroupByNone) {
				if (VisualViewInfo is VisualGanttViewGroupByNone)
					return VisualViewInfo;
				else
					return new VisualGanttViewGroupByNone();
			}
			if (viewInfo is GanttViewGroupByDate) {
				if (VisualViewInfo is VisualGanttViewGroupByDate)
					return VisualViewInfo;
				else
					return new VisualGanttViewGroupByDate();
			}
			return null;
		}
		protected internal virtual AppointmentsLayoutCalculator CreateAppointmentsLayoutCalculator() {
			return new AppointmentsLayoutCalculator(this);
		}
		protected internal virtual void RecalcScrollBarVisibility() {
		}
		protected internal virtual void RecalcPreliminaryLayout() {
			CreateAndPrepareViewInfo();
		}
		protected internal virtual void RecalcAppointmentsLayout() {
			CalculateAppointments();
			CopyAppointmentsVisualViewInfo(ViewInfo);
		}
		protected internal virtual void RecalcDraggingAppointmentPosition() {
			AppointmentsLayoutCalculator calculator = CreateAppointmentsLayoutCalculator();
			calculator.LayoutDraggedAppointments();
			CopyAppointmentsVisualViewInfo(ViewInfo);
		}
		protected internal virtual void OnContainerScrollBarVisibleChanged() {
			if (InnerView != null)
				InnerView.RaiseChanged(SchedulerControlChangeType.ScrollBarVisibilityChanged);
		}
		protected internal virtual void Reset() {
			innerView.Reset();
		}
		#region IDisposable Members
		void IDisposable.Dispose() {
		}
		#endregion
		protected internal virtual void SetupInnerViewBindings() {
			PropertySyncManager.Register();
		}
		protected internal virtual void CleanupInnerViewBindings() {
			PropertySyncManager.Unregister();
		}
		protected internal virtual void SetupNavigationButtonAppointmentSearchIntervalPropertyBinding() {
			SetBinding(NavigationButtonAppointmentSearchIntervalProperty, InnerBindingHelper.CreateTwoWayPropertyBinding(InnerView, "NavigationButtonAppointmentSearchInterval"));
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal virtual bool CanShowResources() {
			return FilteredResources.Count > 0;
		}
		protected internal virtual void AttachExistingInnerView(SchedulerControl control, InnerSchedulerViewBase view) {
			XtraSchedulerDebug.Assert(this.Control == null);
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(view, "view");
			this.control = control;
			this.innerView = view;
			InnerView.SetOwner(this);
			this.InitializeProperties();
			InnerView.SetProperties(this);
			SetupInnerViewBindings();
			SynchronizeInnerObject();
		}
		protected virtual void SynchronizeInnerObject() {
			if (PropertySyncManager.IsDeferredChanges)
				PropertySyncManager.CommitDeferredChanges();
			PropertySyncManager.Activate();
		}
		protected internal virtual InnerSchedulerViewBase DetachExistingInnerView() {
			CleanupInnerViewBindings();
			InnerSchedulerViewBase result = InnerView;
			this.innerView = null;
			PropertySyncManager.Deactivate();
			return result;
		}
		protected internal virtual void SynchronizeToInnerView(InnerSchedulerViewBase view) {
			Guard.ArgumentNotNull(view, "view");
			view.NavigationButtonAppointmentSearchInterval = NavigationButtonAppointmentSearchInterval;
		}
		protected internal virtual AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new AppointmentDisplayOptions();
		}
		#region IInnerSchedulerViewOwner implementation
		WorkDaysCollection IInnerSchedulerViewOwner.WorkDays { get { return Control.InnerControl.WorkDays; } }
		DayOfWeek IInnerSchedulerViewOwner.FirstDayOfWeek { get { return Control.InnerControl.FirstDayOfWeek; } }
		string IInnerSchedulerViewOwner.ClientTimeZoneId { get { return Control.InnerControl.OptionsBehavior.ClientTimeZoneId; } }
		ResourceBaseCollection IInnerSchedulerViewOwner.GetFilteredResources() {
			return Control.InnerControl.GetFilteredResources();
		}
		AppointmentBaseCollection IInnerSchedulerViewOwner.GetFilteredAppointments(TimeInterval interval, ResourceBaseCollection resources, out bool appointmentsAreReloaded) {
			return Control.InnerControl.GetFilteredAppointments(interval, resources, out appointmentsAreReloaded);
		}
		AppointmentBaseCollection IInnerSchedulerViewOwner.GetNonFilteredAppointments() {
			return Control.InnerControl.GetNonFilteredAppointments();
		}
		AppointmentDisplayOptions IInnerSchedulerViewOwner.CreateAppointmentDisplayOptions() {
			return CreateAppointmentDisplayOptionsCore();
		}
		ResourceBaseCollection IInnerSchedulerViewOwner.GetResourcesTree() {
			return Control.InnerControl.GetResourcesTree();
		}
		void IInnerSchedulerViewOwner.UpdateSelection(SchedulerViewSelection selection) {
			if (ViewInfo == null)
				return;
			ViewInfo.UpdateSelection(selection);
			ViewInfo.UpdateAppointmentsSelection();
			if (VisualViewInfo != null)
				PrepareVisualViewInfo(VisualViewInfo, ViewInfo);
		}
		#endregion
		#region ISchedulerViewRepositoryItem Members
		void ISchedulerViewRepositoryItem.Initialize(InnerSchedulerControl control) {
			SchedulerControl wpfControl = (SchedulerControl)control.Owner;
			Initialize(wpfControl);
		}
		InnerSchedulerViewBase ISchedulerViewRepositoryItem.InnerView { get { return innerView; } }
		void ISchedulerViewRepositoryItem.Reset() {
			Reset();
		}
		#endregion
		protected internal abstract ViewFactoryHelper CreateFactoryHelper();
		protected internal abstract ViewDateTimeScrollController CreateDateTimeScrollController();
		public TimeIntervalCollection GetVisibleIntervals() {
			return innerView.GetVisibleIntervals();
		}
		public virtual void SetVisibleIntervals(TimeIntervalCollection intervals) {
			if (intervals == null)
				Exceptions.ThrowArgumentException("intervals", intervals);
			innerView.SetVisibleIntervals(intervals, Control.Selection);
		}
		public void AddAppointmentSelection(Appointment apt) {
			AppointmentSelectionController controller = Control.AppointmentSelectionController;
			if (controller == null)
				return;
			controller.AddToSelection(apt);
		}
		public void ReverseAppointmentSelection(Appointment apt) {
			AppointmentSelectionController controller = Control.AppointmentSelectionController;
			if (controller == null)
				return;
			controller.ChangeSelection(apt);
		}
		public virtual void LayoutChanged() {
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
		}
		public virtual void GotoTimeInterval(TimeInterval interval) {
			Guard.ArgumentNotNull(interval, "interval");
			if (Control.Selection == null)
				return;
			SetSelectionCore(interval, Control.Selection.Resource);
		}
		public virtual void SetSelection(TimeInterval interval, Resource resource) {
			if (interval == null)
				Exceptions.ThrowArgumentException("interval", interval);
			if (resource == null)
				Exceptions.ThrowArgumentException("resource", resource);
			if (Control.Selection == null)
				return;
			SetSelectionCore(interval, resource);
		}
		protected internal virtual void SetSelectionCore(TimeInterval interval, Resource resource) {
			SetSelectionCommand command = CreateSetSelectionCommand(Control.InnerControl, interval, resource);
			command.Execute();
		}
		protected internal virtual SetSelectionCommand CreateSetSelectionCommand(InnerSchedulerControl control, TimeInterval interval, Resource resource) {
			return new SetSelectionCommand(control, interval, resource);
		}
		public virtual void SelectAppointment(Appointment apt) {
			if (apt == null)
				Exceptions.ThrowArgumentException("apt", apt);
			SelectAppointmentCore(apt, apt.ResourceId);
		}
		public virtual void SelectAppointment(Appointment apt, Resource resource) {
			if (apt == null)
				Exceptions.ThrowArgumentException("apt", apt);
			if (resource == null)
				Exceptions.ThrowArgumentException("resource", resource);
			SelectAppointmentCore(apt, resource.Id);
		}
		protected internal virtual void SelectAppointmentCore(Appointment apt, object resourceId) {
			if (Control.Selection == null)
				return;
			if (!ResourceBase.InternalMatchIdToResourceIdCollection(apt.ResourceIds, resourceId))
				resourceId = Control.Selection.Resource.Id;
			Resource resource = this.FilteredResources.GetResourceById(resourceId);
			if (resource == ResourceBase.Empty)
				resource = Control.Selection.Resource;
			Control.BeginUpdate();
			try {
				SetSelectionCore(((IInternalAppointment)apt).CreateInterval(), resource);
				ChangeAppointmentSelection(apt);
			}
			finally {
				Control.EndUpdate();
			}
			ViewInfo.UpdateAppointmentsSelection();
			if (VisualViewInfo != null)
				PrepareVisualViewInfo(VisualViewInfo, ViewInfo);
			if (Control != null)
				Control.UpdateLayout();
		}
		public virtual void ChangeAppointmentSelection(Appointment apt) {
			AppointmentSelectionController controller = Control.AppointmentSelectionController;
			if (controller == null)
				return;
			if (controller.SelectSingleAppointment(apt)) {
				SelectAppointment(apt);
				MakeAppointmentVisibleInScrollContainers(apt);
			}
		}
		protected virtual void MakeAppointmentVisibleInScrollContainers(Appointment apt) {
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
		public virtual ResourceBaseCollection GetResources() {
			if (innerView != null && Object.ReferenceEquals(Control.ActiveView, this))
				return innerView.GetResources();
			else
				return new ResourceBaseCollection();
		}
		public virtual AppointmentBaseCollection GetAppointments() {
			if (innerView != null && Object.ReferenceEquals(Control.ActiveView, this))
				return innerView.GetAppointments();
			else
				return new AppointmentBaseCollection();
		}
		public virtual void ZoomIn() {
			InnerView.ZoomIn();
		}
		public virtual void ZoomOut() {
			InnerView.ZoomOut();
		}
#if !SL
		public SchedulerControlHitInfo CalcHitInfo(Point point) {
			ISchedulerHitInfo hitInfo = SchedulerHitInfo.CreateSchedulerHitInfo(control, point);
			return SchedulerControlHitInfo.Create(hitInfo);
		}
		public SchedulerControlHitInfo CalcHitInfo(System.Windows.Input.MouseEventArgs e) {
			PlatformIndependentMouseEventArgs pea = Control.CreatePlatformIndependentMouseEventArgs(e);
			ISchedulerHitInfo hitInfo = SchedulerHitInfo.CreateSchedulerHitInfo(control, pea);
			return SchedulerControlHitInfo.Create(hitInfo);
		}
#endif
		protected abstract SchedulerAppointmentDisplayOptions GetAppointmentDisplayOptions();
		protected abstract bool ValidateAppointmentDisplayOptions(SchedulerAppointmentDisplayOptions appointmentDisplayOptions);
		#region INotifyPropertyChanged members
		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual bool RaisePropertyChanging(string propertyName) {
			if (PropertyChanging != null) {
				PropertyChangingEventArgsEx args = new PropertyChangingEventArgsEx(propertyName);
				PropertyChanging(this, args);
				return !args.Cancel;
			}
			return true;
		}
		protected internal virtual void RaisePropertyChanged<T>(string propertyName, T oldValue, T newValue) {
			if (PropertyChanged != null) {
				PropertyChangedEventArgsEx args = new PropertyChangedEventArgsEx(propertyName, oldValue, newValue);
				PropertyChanged(this, args);
			}
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class ViewContentStyleSelectorProperties : SchedulerSealableObject {
		#region GroupByNone
		public static readonly DependencyProperty GroupByNoneProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ViewContentStyleSelectorProperties, Object>("GroupByNone", null, (s, e) => s.OnGroupByNoneChanged(e.OldValue, e.NewValue));
		public Object GroupByNone { get { return GetValue(GroupByNoneProperty); } set { SetValue(GroupByNoneProperty, value); } }
		void OnGroupByNoneChanged(Object oldValue, Object newValue) {
			SealHelper.SealIfSealable(newValue);
		}
		#endregion
		#region GroupByDate
		public static readonly DependencyProperty GroupByDateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ViewContentStyleSelectorProperties, Object>("GroupByDate", null, (s, e) => s.OnGroupByDateChanged(e.OldValue, e.NewValue));
		public Object GroupByDate { get { return GetValue(GroupByDateProperty); } set { SetValue(GroupByDateProperty, value); } }
		void OnGroupByDateChanged(Object oldValue, Object newValue) {
			SealHelper.SealIfSealable(newValue);
		}
		#endregion
		#region GroupByResource
		public static readonly DependencyProperty GroupByResourceProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ViewContentStyleSelectorProperties, Object>("GroupByResource", null, (s, e) => s.OnGroupByResourceChanged(e.OldValue, e.NewValue));
		public Object GroupByResource { get { return GetValue(GroupByResourceProperty); } set { SetValue(GroupByResourceProperty, value); } }
		void OnGroupByResourceChanged(Object oldValue, Object newValue) {
			SealHelper.SealIfSealable(newValue);
		}
		#endregion
#if !SL
		protected override Freezable CreateInstanceCore() {
			return new ViewContentStyleSelectorProperties();
		}
#endif
	}
	#region ViewContentTemplateSelector (abstarct class)
	public abstract class ViewContentStyleSelector : StyleSelector {
		ViewContentStyleSelectorProperties properties;
		protected ViewContentStyleSelector() {
		}
		public virtual ViewContentStyleSelectorProperties Properties { 
			get { return properties; } 
			set {
				properties = value; 
				SealHelper.SealIfSealable(value);
			} }
		protected internal abstract Type GroupByNoneType { get; }
		protected internal abstract Type GroupByDateType { get; }
		protected internal abstract Type GroupByResourceType { get; }
		protected virtual ViewContentStyleSelectorProperties CreateProperties() {
			return new ViewContentStyleSelectorProperties();
		}
		public override Style SelectStyle(object item, DependencyObject container) {
			VisualViewInfoBase visualViewInfo = container as VisualViewInfoBase;
			if (item != null && Properties != null) {
				if (item.GetType() == GroupByNoneType)
					return ObtainStyle(visualViewInfo, Properties.GroupByNone);
				if (item.GetType() == GroupByDateType)
					return ObtainStyle(visualViewInfo, Properties.GroupByDate);
				if (item.GetType() == GroupByResourceType)
					return ObtainStyle(visualViewInfo, Properties.GroupByResource);
			}
			return base.SelectStyle(item, container);
		}
#if !SL
		Style ObtainStyle(VisualViewInfoBase visualViewInfo, object key) {
			return (Style)visualViewInfo.FindResource(key);
		}
#else
		Style ObtainStyle(VisualViewInfoBase visualViewInfo, object key) {
			return (Style)key;
		}
#endif
	}
	#endregion
}
