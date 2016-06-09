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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Ribbon.Automation;
using DevExpress.Xpf.Utils;
using System.Linq;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Helpers;
using System.Collections;
using System.ComponentModel;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Bars.Native;
using System.Diagnostics.CodeAnalysis;
using System.Collections.ObjectModel;
namespace DevExpress.Xpf.Ribbon {
	[DXToolboxBrowsable]
	[ToolboxTabName(AssemblyInfo.DXTabWpfNavigation)]
	public class RibbonStatusBarControl : Control, IRibbonStatusBarControl, ILogicalChildrenContainer, IMultipleElementRegistratorSupport, IMergingSupport, IHierarchicalMergingSupport<RibbonStatusBarControl> {
		#region static
		const string statusBarControlMergingID = "A1FF3C6E-23B8-4F1F-BDFA-A29B20E14DCF";
		public static readonly DependencyProperty RibbonStatusBarProperty;
		public static readonly DependencyProperty IsSizeGripVisibleProperty;
		public static readonly DependencyProperty LeftItemLinksSourceProperty;
		public static readonly DependencyProperty LeftItemTemplateProperty;
		public static readonly DependencyProperty LeftItemTemplateSelectorProperty;
		public static readonly DependencyProperty LeftItemStyleProperty;
		public static readonly DependencyProperty RightItemLinksSourceProperty;
		public static readonly DependencyProperty RightItemTemplateProperty;
		public static readonly DependencyProperty RightItemTemplateSelectorProperty;
		public static readonly DependencyProperty RightItemStyleProperty;
		public static readonly DependencyProperty ItemLinksSourceElementGeneratesUniqueBarItemProperty;		
		public static readonly DependencyProperty ActualIsSizeGripVisibleProperty;
		protected static readonly DependencyPropertyKey ActualIsSizeGripVisiblePropertyKey;
		[IgnoreDependencyPropertiesConsistencyChecker]
		protected static readonly DependencyProperty IsWindowMaximizedProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty RightItemsAttachedBehaviorProperty; 
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty LeftItemsAttachedBehaviorProperty;
		public static readonly DependencyProperty MDIMergeStyleProperty;
		public static readonly DependencyProperty AsyncMergingEnabledProperty;
		static RibbonStatusBarControl() {
			AsyncMergingEnabledProperty = DependencyProperty.Register("AsyncMergingEnabled", typeof(bool), typeof(RibbonStatusBarControl), new PropertyMetadata(true));
			BarManager.BarManagerProperty.OverrideMetadata(typeof(RibbonStatusBarControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, (d, e) => ((RibbonStatusBarControl)d).OnManagerChanged((BarManager)e.OldValue, (BarManager)e.NewValue)));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonStatusBarControl), new FrameworkPropertyMetadata(typeof(RibbonStatusBarControl)));
			IsWindowMaximizedProperty = DependencyPropertyManager.Register("IsWindowMaximized", typeof(bool), typeof(RibbonStatusBarControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsWindowMaximizedPropertyChanged)));
			IsSizeGripVisibleProperty = DependencyProperty.Register("IsSizeGripVisible", typeof(bool), typeof(RibbonStatusBarControl), new System.Windows.PropertyMetadata(true, new System.Windows.PropertyChangedCallback(OnIsSizeGripVisiblePropertyChanged)));
			LeftItemLinksSourceProperty = DependencyProperty.Register("LeftItemLinksSource", typeof(IEnumerable), typeof(RibbonStatusBarControl), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnLeftItemLinksSourcePropertyChanged)));
			LeftItemTemplateProperty = DependencyProperty.Register("LeftItemTemplate", typeof(DataTemplate), typeof(RibbonStatusBarControl), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnLeftItemLinksTemplatePropertyChanged)));
			LeftItemTemplateSelectorProperty = DependencyProperty.Register("LeftItemTemplateSelector", typeof(DataTemplateSelector), typeof(RibbonStatusBarControl), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnLeftItemLinksTemplateSelectorPropertyChanged)));
			LeftItemStyleProperty = DependencyProperty.Register("LeftItemStyle", typeof(Style), typeof(RibbonStatusBarControl), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnLeftItemLinksTemplatePropertyChanged)));
			LeftItemsAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("LeftItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<RibbonStatusBarControl, BarItem>), typeof(RibbonStatusBarControl), new UIPropertyMetadata(null));
			RightItemLinksSourceProperty = DependencyProperty.Register("RightItemLinksSource", typeof(IEnumerable), typeof(RibbonStatusBarControl), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnRightItemLinksSourcePropertyChanged)));
			RightItemTemplateProperty = DependencyProperty.Register("RightItemTemplate", typeof(DataTemplate), typeof(RibbonStatusBarControl), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnRightItemLinksTemplatePropertyChanged)));
			RightItemTemplateSelectorProperty = DependencyProperty.Register("RightItemTemplateSelector", typeof(DataTemplateSelector), typeof(RibbonStatusBarControl), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnRightItemLinksTemplateSelectorPropertyChanged)));
			RightItemStyleProperty = DependencyProperty.Register("RightItemStyle", typeof(Style), typeof(RibbonStatusBarControl), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnRightItemLinksTemplatePropertyChanged)));
			RightItemsAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("RightItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<RibbonStatusBarControl, BarItem>), typeof(RibbonStatusBarControl), new UIPropertyMetadata(null));
			RibbonStatusBarProperty = DependencyPropertyManager.RegisterAttached("RibbonStatusBar", typeof(RibbonStatusBarControl), typeof(RibbonStatusBarControl), new FrameworkPropertyMetadata(null));
			ActualIsSizeGripVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualIsSizeGripVisible", typeof(bool), typeof(RibbonStatusBarControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ActualIsSizeGripVisibleProperty = ActualIsSizeGripVisiblePropertyKey.DependencyProperty;
			MDIMergeStyleProperty = DependencyProperty.Register("MDIMergeStyle", typeof(MDIMergeStyle), typeof(RibbonStatusBarControl), new System.Windows.PropertyMetadata(MDIMergeStyle.Always, new System.Windows.PropertyChangedCallback(OnMDIMergeStylePropertyChanged)));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(RibbonStatusBarControl), typeof(RibbonStatusBarAutomationPeer), owner => new RibbonStatusBarAutomationPeer((RibbonStatusBarControl)owner));
			ItemLinksSourceElementGeneratesUniqueBarItemProperty = DependencyProperty.Register("ItemLinksSourceElementGeneratesUniqueBarItem", typeof(bool), typeof(RibbonStatusBarControl), new System.Windows.PropertyMetadata(false, new System.Windows.PropertyChangedCallback((d, e) => ((RibbonStatusBarControl)d).OnItemLinksSourceElementGeneratesUniqueBarItemChanged((bool)e.OldValue))));
			VisibilityProperty.OverrideMetadata(typeof(RibbonStatusBarControl), new FrameworkPropertyMetadata(Visibility.Visible, null, new CoerceValueCallback((d, v) => ((RibbonStatusBarControl)d).CoerceVisibility(v))));
		}
		protected static void OnLeftItemLinksSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonStatusBarControl)d).OnLeftItemLinksSourceChanged(e);
		}
		protected static void OnLeftItemLinksTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonStatusBarControl)d).OnLeftItemLinksTemplateChanged(e);
		}
		protected static void OnLeftItemLinksTemplateSelectorPropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonStatusBarControl)d).OnLeftItemLinksTemplateSelectorChanged(e);
		}
		protected static void OnRightItemLinksSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonStatusBarControl)d).OnRightItemLinksSourceChanged(e);
		}
		protected static void OnRightItemLinksTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonStatusBarControl)d).OnRightItemLinksTemplateChanged(e);
		}
		protected static void OnRightItemLinksTemplateSelectorPropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonStatusBarControl)d).OnRightItemLinksTemplateSelectorChanged(e);
		}
		public static RibbonStatusBarControl GetRibbonStatusBar(DependencyObject obj) {
			return (RibbonStatusBarControl)obj.GetValue(RibbonStatusBarProperty);
		}
		public static void SetRibbonStatusBar(DependencyObject obj, RibbonStatusBarControl value) {
			obj.SetValue(RibbonStatusBarProperty, value);
		}
		protected static void OnIsSizeGripVisiblePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonStatusBarControl)d).OnIsSizeGripVisibleChanged((bool)e.OldValue);
		}
		protected static void OnMDIMergeStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonStatusBarControl)d).OnMDIMergeStyleChanged((MDIMergeStyle)e.OldValue);
		}		
		protected static void OnIsWindowMaximizedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonStatusBarControl)d).OnIsWindowMaximizedChanged((bool)e.OldValue);
		}
		#endregion
		public RibbonStatusBarControl() {
			addPartControlsAction = new PostponedAction(() => !IsInitialized);
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
			LeftPartControl = new RibbonStatusBarLeftPartControl(this);
			RightPartControl = new RibbonStatusBarRightPartControl(this);
			addPartControlsAction.PerformPostpone(() => {
				AddLogicalChild(LeftPartControl);
				AddLogicalChild(RightPartControl);
			});
			IsTabStop = false;
			SetBinding(IsWindowMaximizedProperty, new Binding() { Path  = new PropertyPath("(0)", FloatingContainer.IsMaximizedProperty), Source = this });
			KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.None);
			((IMergingSupport)this).IsAutomaticallyMerged = true;
		}
		#region dep props
		public bool ItemLinksSourceElementGeneratesUniqueBarItem {
			get { return (bool)GetValue(ItemLinksSourceElementGeneratesUniqueBarItemProperty); }
			set { SetValue(ItemLinksSourceElementGeneratesUniqueBarItemProperty, value); }
		}
		public IEnumerable LeftItemLinksSource {
			get { return (IEnumerable)GetValue(LeftItemLinksSourceProperty); }
			set { SetValue(LeftItemLinksSourceProperty, value); }
		}
		public DataTemplate LeftItemTemplate {
			get { return (DataTemplate)GetValue(LeftItemTemplateProperty); }
			set { SetValue(LeftItemTemplateProperty, value); }
		}
		public DataTemplateSelector LeftItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(LeftItemTemplateSelectorProperty); }
			set { SetValue(LeftItemTemplateSelectorProperty, value); }
		}
		public Style LeftItemStyle {
			get { return (Style)GetValue(LeftItemStyleProperty); }
			set { SetValue(LeftItemStyleProperty, value); }
		}
		public IEnumerable RightItemLinksSource {
			get { return (IEnumerable)GetValue(RightItemLinksSourceProperty); }
			set { SetValue(RightItemLinksSourceProperty, value); }
		}
		public DataTemplate RightItemTemplate {
			get { return (DataTemplate)GetValue(RightItemTemplateProperty); }
			set { SetValue(RightItemTemplateProperty, value); }
		}
		public DataTemplateSelector RightItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(RightItemTemplateSelectorProperty); }
			set { SetValue(RightItemTemplateSelectorProperty, value); }
		}
		public Style RightItemStyle {
			get { return (Style)GetValue(RightItemStyleProperty); }
			set { SetValue(RightItemStyleProperty, value); }
		}
		public bool ActualIsSizeGripVisible {
			get { return (bool)GetValue(ActualIsSizeGripVisibleProperty); }
			protected set { this.SetValue(ActualIsSizeGripVisiblePropertyKey, value); }
		}
		public MDIMergeStyle MDIMergeStyle {
			get { return (MDIMergeStyle)GetValue(MDIMergeStyleProperty); }
			set { SetValue(MDIMergeStyleProperty, value); }
		}
		protected bool IsWindowMaximized {
			get { return (bool)GetValue(IsWindowMaximizedProperty); }
			set { SetValue(IsWindowMaximizedProperty, value); }
		}
		public bool AsyncMergingEnabled {
			get { return (bool)GetValue(AsyncMergingEnabledProperty); }
			set { SetValue(AsyncMergingEnabledProperty, value); }
		}
		#endregion        
		Bars.IMDIChildHost mdiChildHost;
		readonly PostponedAction addPartControlsAction;
		void OnLoaded(object sender, RoutedEventArgs e) {
			UnsubscribeMDIChildHostEvents();
			mdiChildHost = LayoutHelper.FindParentObject<Bars.IMDIChildHost>(this);
			SubscribeMDIChildHostEvents();
			UnsubscribeTemplateEvents();
			SubscribeTemplateEvents();
			UpdateActualVisibility();
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			UnsubscribeTemplateEvents();
			SubscribeMDIChildHostEvents();
		}
		private void OnManagerChanged(BarManager oldValue, BarManager newValue) {
			if(oldValue != null && GetRibbonStatusBar(oldValue) == this) {
				SetRibbonStatusBar(oldValue, null);
			}
			if (newValue != null) {
				SetRibbonStatusBar(newValue, this);
				if (BarManagerHelper.GetChildRibbonStatusBar(newValue) == null)
					BarManagerHelper.SetChildRibbonStatusBar(newValue, this);
			}
		}
		protected internal RibbonStatusBarLeftPartControl LeftPartControl { get; private set; }
		protected internal RibbonStatusBarRightPartControl RightPartControl { get; private set; }
		protected internal Thumb SizeGrip { get; set; }
		public CommonBarItemCollection LeftItems { get { return LeftPartControl.CommonItems; } }
		public CommonBarItemCollection RightItems { get { return RightPartControl.CommonItems; } }
		public BarItemLinkCollection LeftItemLinks { get { return LeftPartControl.ItemLinks; } }
		public BarItemLinkCollection RightItemLinks { get { return RightPartControl.ItemLinks; } }
		protected ContentControl OriginItemContent { get; private set; }
		protected DXContentPresenter LeftContentControl { get; set; }
		protected DXContentPresenter RightContentControl { get; set; }
		public bool IsSizeGripVisible {
			get { return (bool)this.GetValue(IsSizeGripVisibleProperty); }
			set { SetValue(IsSizeGripVisibleProperty, value); }
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}		
		protected override void OnInitialized(EventArgs e) {
			if (InitialVisibility == null)
				InitialVisibility = Visibility;	
			base.OnInitialized(e);
			addPartControlsAction.Perform();
		}
		public override void OnApplyTemplate() {
			UnsubscribeTemplateEvents();
			LeftContentControl = (DXContentPresenter)this.GetTemplateChild("PART_LeftContentPresenter");
			RightContentControl = (DXContentPresenter)this.GetTemplateChild("PART_RightContentPresenter");
			SizeGrip = (Thumb)this.GetTemplateChild("PART_GripSizeThumb");
			OriginItemContent = (ContentControl)this.GetTemplateChild("PART_OriginItemContent");
			SubscribeTemplateEvents();
		}
		void UnsubscribeMDIChildHostEvents() {
			if(mdiChildHost == null)
				return;
			mdiChildHost.IsChildMenuVisibleChanged -= OnMdiChildHostIsChildMenuVisibleChanged;
		}
		void OnMdiChildHostIsChildMenuVisibleChanged(object sender, EventArgs e) {
			UpdateActualVisibility();
		}
		protected Visibility? InitialVisibility { get; set; }
		protected virtual void UpdateActualVisibility() {
			CoerceValue(VisibilityProperty);
		}
		void SubscribeMDIChildHostEvents() {
			if(mdiChildHost == null)
				return;
			mdiChildHost.IsChildMenuVisibleChanged += new EventHandler(OnMdiChildHostIsChildMenuVisibleChanged);
		}
		private void SubscribeTemplateEvents() {
			if(SizeGrip != null) {
				SizeGrip.DragDelta += new DragDeltaEventHandler(OnGripSizeThumbDragDelta);
				SizeGrip.MouseEnter += new System.Windows.Input.MouseEventHandler(OnSizeGripMouseEnter);
				SizeGrip.PreviewMouseDown += new MouseButtonEventHandler(OnSizeGripPreviewMouseDown);
			}
			if(LeftContentControl != null)
				LeftContentControl.Content = LeftPartControl;
			if(RightContentControl != null)
				RightContentControl.Content = RightPartControl;
		}
		void OnSizeGripPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			DXRibbonWindow ribbonWindow = GetParentWindow() as DXRibbonWindow;
			if(ribbonWindow != null) {
				if(FlowDirection == System.Windows.FlowDirection.LeftToRight)
					ribbonWindow.ResizeWindow(DXWindowActiveResizeParts.BottomRight);
				else
					ribbonWindow.ResizeWindow(DXWindowActiveResizeParts.BottomLeft);
			}
		}
		private void UnsubscribeTemplateEvents() {
			if(SizeGrip != null) {
				SizeGrip.DragDelta -= OnGripSizeThumbDragDelta;
				SizeGrip.MouseEnter -= OnSizeGripMouseEnter;
				SizeGrip.PreviewMouseDown -= OnSizeGripPreviewMouseDown;
			}
			if(LeftContentControl != null)
				LeftContentControl.Content = null;
			if(RightContentControl != null)
				RightContentControl.Content = null;
		}
		protected virtual void OnSizeGripMouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
			UpdateSizeGripCursor();
			if(SizeGrip == null)
				return;
			SizeGrip.Cursor = FlowDirection == FlowDirection.LeftToRight ? Cursors.SizeNWSE : Cursors.SizeNESW;
		}
		protected virtual void UpdateSizeGripCursor() {
		}
		protected virtual void OnItemLinksSourceElementGeneratesUniqueBarItemChanged(bool oldValue) {
			OnLeftItemLinksSourceChanged(new DependencyPropertyChangedEventArgs(LeftItemLinksSourceProperty, LeftItemLinksSource, LeftItemLinksSource));
			OnRightItemLinksSourceChanged(new DependencyPropertyChangedEventArgs(RightItemLinksSourceProperty, RightItemLinksSource, RightItemLinksSource));
		}
		private void OnLeftItemLinksTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<RibbonStatusBarControl, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				LeftItemsAttachedBehaviorProperty);
		}
		private void OnLeftItemLinksTemplateSelectorChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<RibbonStatusBarControl, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				LeftItemsAttachedBehaviorProperty);
		}
		protected virtual void OnLeftItemLinksSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			LeftItemGeneratorHelper.OnItemsSourceChanged(e);
		}
		BarItemGeneratorHelper<RibbonStatusBarControl> leftItemGeneratorHelper;
		protected BarItemGeneratorHelper<RibbonStatusBarControl> LeftItemGeneratorHelper {
			get {
				if(leftItemGeneratorHelper == null)
					leftItemGeneratorHelper = new BarItemGeneratorHelper<RibbonStatusBarControl>(this, LeftItemsAttachedBehaviorProperty, LeftItemStyleProperty, LeftItemTemplateProperty, LeftItems, LeftItemTemplateSelectorProperty, ItemLinksSourceElementGeneratesUniqueBarItem);
				return leftItemGeneratorHelper;
			}
		}
		private void OnRightItemLinksTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<RibbonStatusBarControl, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				RightItemsAttachedBehaviorProperty);
		}
		private void OnRightItemLinksTemplateSelectorChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<RibbonStatusBarControl, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				RightItemsAttachedBehaviorProperty);
		}
		protected virtual void OnRightItemLinksSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			RightItemGeneratorHelper.OnItemsSourceChanged(e);
		}
		BarItemGeneratorHelper<RibbonStatusBarControl> rightItemGeneratorHelper;
		protected BarItemGeneratorHelper<RibbonStatusBarControl> RightItemGeneratorHelper {
			get {
				if(rightItemGeneratorHelper == null)
					rightItemGeneratorHelper = new BarItemGeneratorHelper<RibbonStatusBarControl>(this, RightItemsAttachedBehaviorProperty, RightItemStyleProperty, RightItemTemplateProperty, RightItems, RightItemTemplateSelectorProperty, ItemLinksSourceElementGeneratesUniqueBarItem);
				return rightItemGeneratorHelper;
			}
		}		
		Window GetParentWindow() { return this.VisualParents().OfType<Window>().FirstOrDefault(); }
		double MinMax(double value, double minValue, double maxValue) {
			return Math.Max(minValue, Math.Min(value, maxValue));
		}
		void OnGripSizeThumbDragDelta(object sender, DragDeltaEventArgs e) {			
			Window wnd = GetParentWindow();
			if(wnd != null && wnd.Content is UIElement && wnd.WindowState != WindowState.Maximized) {
				UIElement content = wnd.Content as UIElement;
				if(wnd.FlowDirection == FlowDirection.LeftToRight) {					
					wnd.Width = Math.Max(wnd.Width + MinMax(wnd.Width + e.HorizontalChange, MinWidth, MaxWidth) - wnd.Width, wnd.MinWidth);
					wnd.Height = Math.Max(wnd.Height + MinMax(content.RenderSize.Height + e.VerticalChange, ActualHeight, wnd.MaxHeight) - content.RenderSize.Height, wnd.MinHeight);
				} else {
					double newWidth = Math.Max(wnd.Width + MinMax(wnd.Width + e.HorizontalChange, MinWidth, MaxWidth) - wnd.Width, wnd.MinWidth);
					double deltaWidth = wnd.Width - newWidth;					
					wnd.Width = newWidth;					
					wnd.Height = Math.Max(wnd.Height + MinMax(content.RenderSize.Height + e.VerticalChange, ActualHeight, wnd.MaxHeight) - content.RenderSize.Height, wnd.MinHeight);
					if(newWidth == wnd.ActualWidth)
						wnd.Left += deltaWidth;
				}
			}
		}
		protected virtual void OnIsSizeGripVisibleChanged(bool oldValue) {
			UpdateActualIsSizeGripVisible();
		}
		protected virtual void OnMDIMergeStyleChanged(MDIMergeStyle oldValue) {
			UpdateActualVisibility();
		}
		protected virtual void UpdateActualIsSizeGripVisible() {
			ActualIsSizeGripVisible = IsSizeGripVisible && !IsWindowMaximized;
		}
		protected virtual void OnIsWindowMaximizedChanged(bool oldValue) {
			UpdateActualIsSizeGripVisible();
		}		
		#region merging
		ObservableCollection<RibbonStatusBarControl> MergedChildren { get { return HierarchicalMergingHelper.ActualMergedChildren; } }
		RibbonStatusBarControl mergedParent;
		public RibbonStatusBarControl MergedParent {
			get { return mergedParent; }
			protected set {
				if (value == mergedParent) return;
				mergedParent = value;
				UpdateActualVisibility();
			}
		}
		public bool IsMergedChild(RibbonStatusBarControl item) {
			return HierarchicalMergingHelper.CompositeMergedChildren.Contains(item);
		}		
		public void Merge(RibbonStatusBarControl childStatusBar) {
			HierarchicalMergingHelper.Merge(childStatusBar);
		}
		void MergeCore(RibbonStatusBarControl childStatusBar) {
			if (childStatusBar == null)
				return;
			if (!IsMergedChild(childStatusBar) || childStatusBar.HierarchicalMergingHelper.ActualMergedParent != null || childStatusBar.HierarchicalMergingHelper.ActualMergedParent == this)
				return;
			childStatusBar.HierarchicalMergingHelper.ActualMergedParent = this;
			MergedChildren.Add(childStatusBar);
			((ILinksHolder)LeftPartControl).Merge(childStatusBar.LeftPartControl);
			((ILinksHolder)RightPartControl).Merge(childStatusBar.RightPartControl);
		}
		protected virtual object CoerceVisibility(object value) {
			if (MergingProperties.GetHideElements(this) && MDIMergeStyle != MDIMergeStyle.Never && HasMergingCandidates())
				return Visibility.Collapsed;
			var hideInMDIHost = mdiChildHost != null && !mdiChildHost.IsChildMenuVisible && MDIMergeStyle != Bars.MDIMergeStyle.Never;
			if (hideInMDIHost || MergedParent != null)
				return Visibility.Collapsed;
			return value;
		}
		bool HasMergingCandidates() {
			object mergingRegistratorName = ((IMultipleElementRegistratorSupport)this).GetName(typeof(IMergingSupport));
			return BarNameScope.GetService<IElementRegistratorService>(this).GetElements<IMergingSupport>(mergingRegistratorName, ScopeSearchSettings.Ancestors).Where(x => BarNameScope.GetService<IMergingService>(x).CanMerge(x, this)).Any();
		}
#pragma warning disable 3005
		public void UnMerge(RibbonStatusBarControl childStatusBar) {
			HierarchicalMergingHelper.UnMerge(childStatusBar);
		}
		void UnMergeCore(RibbonStatusBarControl childStatusBar) {
			if (childStatusBar == null || !MergedChildren.Contains(childStatusBar))
				return;
			MergedChildren.Remove(childStatusBar);
			childStatusBar.HierarchicalMergingHelper.ActualMergedParent = null;
			((ILinksHolder)LeftPartControl).UnMerge(childStatusBar.LeftPartControl);
			((ILinksHolder)RightPartControl).UnMerge(childStatusBar.RightPartControl);
		}
		public void UnMerge() {
			while (HierarchicalMergingHelper.MergedChildren.Count != 0) {
				UnMerge(HierarchicalMergingHelper.MergedChildren[HierarchicalMergingHelper.MergedChildren.Count - 1]);
			}
		}
#pragma warning restore 3005
		#endregion
		void IRibbonStatusBarControl.Merge(object child) {
			Merge((RibbonStatusBarControl)child);
		}
		void IRibbonStatusBarControl.UnMerge(object child) {
			UnMerge((RibbonStatusBarControl)child);
		}
		MDIMergeStyle IRibbonStatusBarControl.GetMDIMergeStyle() {
			return MDIMergeStyle;
		}
		public bool IsChild {
			get { return MergedParent != null; }
		}
		public bool IsMerged {
			get { return MergedChildren.Count != 0; }
		}
		protected override IEnumerator LogicalChildren {
			get { return new MergedEnumerator(logicalChildrenContainerItems.GetEnumerator(), new SingleObjectEnumerator(LeftPartControl), new SingleObjectEnumerator(RightPartControl)); }
		}
		#region ILogicalChildrenContainer
		readonly List<object> logicalChildrenContainerItems = new List<object>();
		void ILogicalChildrenContainer.AddLogicalChild(object child) {
			logicalChildrenContainerItems.Add(child);
			AddLogicalChild(child);
		}
		void ILogicalChildrenContainer.RemoveLogicalChild(object child) {
			logicalChildrenContainerItems.Remove(child);
			RemoveLogicalChild(child);
		}
		#endregion
		#region IMultipleElementRegistratorSupport Members
		object IMultipleElementRegistratorSupport.GetName(object registratorKey) {
			if (Equals(typeof(IMergingSupport), registratorKey)) {
				return MergingProperties.GetName(this).WithString(x => x) ?? statusBarControlMergingID;
			}
			if (Equals(typeof(IFrameworkInputElement), registratorKey)) {
				return Name;
			}
			throw new ArgumentException();
		}
		IEnumerable<object> IMultipleElementRegistratorSupport.RegistratorKeys {
			get { return new object[] { typeof(IFrameworkInputElement), typeof(IMergingSupport) }; }
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			MergingPropertiesHelper.OnPropertyChanged(this, e, statusBarControlMergingID);
			if (Equals(FrameworkElement.NameProperty, e.Property)) {
				BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(IFrameworkInputElement), e.OldValue, e.NewValue);
			}
		}
		#endregion
		#region IMergingSupport Members
		bool IMergingSupport.CanMerge(IMergingSupport second) { return MDIMergeStyle != MDIMergeStyle.Never && (second as RibbonStatusBarControl).Return(x => x.MDIMergeStyle != MDIMergeStyle.Never, () => true); }
		bool IMergingSupport.IsMerged { get { return MergedParent != null; } }
		bool IMergingSupport.IsAutomaticallyMerged { get; set; }
		bool IMergingSupport.IsMergedParent(IMergingSupport second) { return MergedParent == second; }
		object IMergingSupport.MergingKey { get { return typeof(RibbonStatusBarControl); } }		
		void IMergingSupport.Merge(IMergingSupport second) { HierarchicalMergingHelper.Merge((RibbonStatusBarControl)second); }
		void IMergingSupport.Unmerge(IMergingSupport second) { HierarchicalMergingHelper.UnMerge((RibbonStatusBarControl)second); }
		#endregion
		#region IHierarchicalMergingSupport
		HierarchicalMergingHelper<RibbonStatusBarControl> helper;
		HierarchicalMergingHelper<RibbonStatusBarControl> HierarchicalMergingHelper { get { return helper ?? (helper = new HierarchicalMergingHelper<RibbonStatusBarControl>(this)); } }
		HierarchicalMergingHelper<RibbonStatusBarControl> IHierarchicalMergingSupport<RibbonStatusBarControl>.Helper {
			get { return HierarchicalMergingHelper; }
		}
		RibbonStatusBarControl IHierarchicalMergingSupport<RibbonStatusBarControl>.MergedParent {
			get { return MergedParent; }
			set { MergedParent = value; }
		}		
		void IHierarchicalMergingSupport<RibbonStatusBarControl>.ReMerge() {
			var unmergedChildren = HierarchicalMergingHelper.ActualMergedChildren.AsEnumerable().Reverse().ToList();
			foreach (var child in unmergedChildren) {
				UnMergeCore(child);
			}
			foreach (var child in HierarchicalMergingHelper.CompositeMergedChildren) {
				MergeCore(child);
			}
		}
		#endregion
	}
}
