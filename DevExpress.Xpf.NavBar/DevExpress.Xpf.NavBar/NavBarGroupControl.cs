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

using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Diagnostics;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System.Windows.Media;
using System;
using DevExpress.Xpf.NavBar.Automation;
using DevExpress.Xpf.Editors.Flyout;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using System.Windows.Threading;
using DevExpress.Xpf.Navigation;
using DevExpress.Xpf.WindowsUI;
namespace DevExpress.Xpf.NavBar {
	[TemplateVisualState(Name = "Vertical", GroupName = "OrientationStates")]
	[TemplateVisualState(Name = "Horizontal", GroupName = "OrientationStates")]
	public partial class NavBarGroupControl : ContentControl, IScrollMode, INavBarContainer {
		public static readonly DependencyProperty NavBarProperty;
		public static readonly DependencyProperty DisplayModeProperty;
		public static readonly DependencyProperty LayoutSettingsProperty;
		public static readonly DependencyProperty ImageSettingsProperty;
		static readonly DependencyPropertyKey ActualGroupContentTemplatePropertyKey;
		public static readonly DependencyProperty ActualGroupContentTemplateProperty;
		public static readonly DependencyProperty DisplaySourceProperty;
		public static readonly DependencyProperty GroupProperty;
		public static readonly DependencyProperty GroupContentPresenterTemplateProperty;
		public static readonly DependencyProperty GroupItemsControlTemplateProperty;
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ShowBorderProperty;						
		static NavBarGroupControl() {
			ShowBorderProperty = DependencyPropertyManager.Register("ShowBorder", typeof(bool), typeof(NavBarGroupControl), new FrameworkPropertyMetadata(true, (d, e) => ((NavBarGroupControl)d).OnShowBorderChanged((bool)e.OldValue)));		
			ActualGroupContentTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGroupContentTemplate", typeof(DataTemplate), typeof(NavBarGroupControl), new PropertyMetadata(null));
			ActualGroupContentTemplateProperty = ActualGroupContentTemplatePropertyKey.DependencyProperty;
			DisplaySourceProperty = DependencyPropertyManager.Register("DisplaySource", typeof(DisplaySource), typeof(NavBarGroupControl), new PropertyMetadata(DisplaySource.Items, (d, e) => ((NavBarGroupControl)d).OnDisplaySourceChanged()));
			GroupProperty = DependencyPropertyManager.Register("Group", typeof(NavBarGroup), typeof(NavBarGroupControl), new PropertyMetadata(null, new PropertyChangedCallback((d,e)=>((NavBarGroupControl)d).OnGroupChanged(e))));
			GroupContentPresenterTemplateProperty = DependencyPropertyManager.Register("GroupContentPresenterTemplate", typeof(DataTemplate), typeof(NavBarGroupControl), new PropertyMetadata(null, (d, e) => ((NavBarGroupControl)d).UpdateGroupContentTemplate()));
			GroupItemsControlTemplateProperty = DependencyPropertyManager.Register("GroupItemsControlTemplate", typeof(DataTemplate), typeof(NavBarGroupControl), new PropertyMetadata(null, (d, e) => ((NavBarGroupControl)d).UpdateGroupContentTemplate()));
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(NavBarGroupControl), new PropertyMetadata(Orientation.Vertical, (d, e) => ((NavBarGroupControl)d).OnOrientationPropertyChanged()));
			NavBarProperty = DependencyPropertyManager.Register("NavBar", typeof(NavBarControl), typeof(NavBarGroupControl), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarGroupControl)d).OnNavBarChanged((NavBarControl)e.OldValue)));
			DisplayModeProperty = DependencyPropertyManager.Register("DisplayMode", typeof(DisplayMode), typeof(NavBarGroupControl), new FrameworkPropertyMetadata(DisplayMode.Default, new PropertyChangedCallback(OnDisplayModePropertyChanged)));
			LayoutSettingsProperty = DependencyPropertyManager.Register("LayoutSettings", typeof(LayoutSettings), typeof(NavBarGroupControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnLayoutSettingsPropertyChanged)));
			ImageSettingsProperty = DependencyPropertyManager.Register("ImageSettings", typeof(ImageSettings), typeof(NavBarGroupControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnImageSettingsPropertyChanged)));		
			InitNavBarGroupControl();
			TextElementHelper<NavBarGroupControl>.OverrideMetadata(gControl => gControl.Group);		  
		}
		protected static void OnDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarGroupControl)d).OnDisplayModeChanged((DisplayMode)e.OldValue);
		}
		protected static void OnLayoutSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarGroupControl)d).OnLayoutSettingsChanged((LayoutSettings)e.OldValue);
		}
		protected static void OnImageSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarGroupControl)d).OnImageSettingsChanged((ImageSettings)e.OldValue);
		}
		protected virtual void OnNavBarChanged(NavBarControl oldValue) {
			this.OnNavBarChanged(oldValue, NavBar);
		}						
		public NavBarGroupControl() {
			this.SetDefaultStyleKey(typeof(NavBarGroupControl));
			requestContainerHandler = new RequestContainersWeakEventHandler(this, (container, sender, args) => args.AddContainer(container));
			BindingOperations.SetBinding(this, NavBarGroupControl.GroupProperty, new Binding() { Converter = new FrameworkElementInfoSLCompatibilityConverterExtension() { ConvertToInfo = false, IgnoreWrongValue = true} });
			DataContextChanged += OnDataContextChanged;
			IsFlyoutOpen = false;
		}
		#region Properties
		public ImageSettings ImageSettings {
			get { return (ImageSettings)GetValue(ImageSettingsProperty); }
			set { SetValue(ImageSettingsProperty, value); }
		}
		public LayoutSettings LayoutSettings {
			get { return (LayoutSettings)GetValue(LayoutSettingsProperty); }
			set { SetValue(LayoutSettingsProperty, value); }
		}
		public DisplayMode DisplayMode {
			get { return (DisplayMode)GetValue(DisplayModeProperty); }
			set { SetValue(DisplayModeProperty, value); }
		}
		public NavBarControl NavBar {
			get { return (NavBarControl)GetValue(NavBarProperty); }
			set { SetValue(NavBarProperty, value); }
		}
		public DataTemplate ActualGroupContentTemplate {
			get { return (DataTemplate)GetValue(ActualGroupContentTemplateProperty); }
			set { this.SetValue(ActualGroupContentTemplatePropertyKey, value); }
		}
		public DisplaySource DisplaySource {
			get { return (DisplaySource)GetValue(DisplaySourceProperty); }
			set { SetValue(DisplaySourceProperty, value); }
		}
		public NavBarGroup Group {
			get { return (NavBarGroup)GetValue(GroupProperty); }
			set { SetValue(GroupProperty, value); }
		}
		public DataTemplate GroupContentPresenterTemplate {
			get { return (DataTemplate)GetValue(GroupContentPresenterTemplateProperty); }
			set { SetValue(GroupContentPresenterTemplateProperty, value); }
		}
		public DataTemplate GroupItemsControlTemplate {
			get { return (DataTemplate)GetValue(GroupItemsControlTemplateProperty); }
			set { SetValue(GroupItemsControlTemplateProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		Button NavPaneGroupButton { get; set; }
		ScrollControl IScrollMode.ScrollControl {
			get { return (ScrollControl)GetTemplateChild("scrollControl"); }
		}
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}		
		internal Size MinDesiredSize { get; set; }
		#endregion
		void OnDisplaySourceChanged() {
			UpdateGroupContentTemplate();
		}
		private void OnGroupChanged(DependencyPropertyChangedEventArgs e) {
			if((e.OldValue as NavBarGroup != null) && ((e.OldValue as NavBarGroup).GroupControl == this))
				(e.OldValue as NavBarGroup).GroupControl = null;
			if(e.NewValue as NavBarGroup != null)
				(e.NewValue as NavBarGroup).GroupControl = this;
			SetBinding(IsEnabledProperty, new Binding() { Source = Group, Path = new PropertyPath("IsEnabled"), Mode = BindingMode.OneWay });
			SetBinding(FlowDirectionProperty, new Binding() { Source = Group, Path = new PropertyPath("FlowDirection") });
		}
		internal void AddLogicalChildCore(object obj) {
			AddLogicalChild(obj);
		}
		internal void RemoveLogicalChildCore(object obj) {
			RemoveLogicalChild(obj);
		}
		protected internal DXExpander Expander { get; private set; }
		protected internal NavPaneFlyoutControl Flyout { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			navigationPaneView = null;
			overflowPanel_ = null;
			collapsedPanel_ = null;
			SetFlyoutIsOpen(false);
			Flyout = GetTemplateChild("PART_NavPaneFlyout") as NavPaneFlyoutControl;
			UpdateAnimations();
			SetBindings();
			UpdateOrientationStates();
			UpdateGroupToolTip();
			UpdateScrollModeStates();			
			if (Expander != null) {
				Expander.ClearValue(DXExpander.IsHitTestVisibleProperty);
			}
			Expander = (DXExpander)GetTemplateChild("PART_DXExpander");
			if (Expander != null) {
				BindingOperations.SetBinding(Expander, DXExpander.IsHitTestVisibleProperty, new Binding() { Path = new PropertyPath(NavBarAnimationOptions.IsExpandedProperty), Source = Expander });
			}
		}		
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseEnter(e);
			ShowPeekOnMouseEnter();
		}
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseLeave(e);
			HidePeekFormOnMouseLeave();
		}		
		DispatcherTimer showPeekFormTimer;
		DispatcherTimer hidePeekFormTimer;
		NavPaneItemsControlPanel overflowPanel_;
		NavPaneGroupButtonPanel collapsedPanel_;
		NavigationPaneView navigationPaneView;
		NavigationPaneView NavigationPaneView { get { return navigationPaneView ?? (navigationPaneView = Group.With(x => x.NavBar.With(n => n.View as NavigationPaneView))); } }
		NavPaneItemsControlPanel OverflowPanel { get { return overflowPanel_ ?? (overflowPanel_ = (NavPaneItemsControlPanel)NavigationPaneView.With(x => LayoutHelper.FindElementByType(x, typeof(NavPaneItemsControlPanel)))); } }
		NavPaneGroupButtonPanel CollapsedPanel { get { return collapsedPanel_ ?? (collapsedPanel_ = (NavPaneGroupButtonPanel)NavigationPaneView.With(x => LayoutHelper.FindElementByType(x, typeof(NavPaneGroupButtonPanel)))); } }
		DispatcherTimer ShowPeekFormTimer { get { return showPeekFormTimer ?? (showPeekFormTimer = new DispatcherTimer().Do(x => x.Tick += OnShowPeekFormTimerTick)); } }
		DispatcherTimer HidePeekFormTimer { get { return hidePeekFormTimer ?? (hidePeekFormTimer = new DispatcherTimer().Do(x => x.Tick += OnHidePeekFormTimerTick)); } }
		DataTemplate ActualPeekFormTemplate { get { return Group.With(x => x.GetActualPeekFormTemplate()); } }
		bool HasPeekFormTemplate { get { return Group.Return(x => x.HasPeekFormTemplate, () => false); } }
		bool IsInOverflowPanel { get { return OverflowPanel != null && OverflowPanel.Children.Contains(this); } }
		bool IsInCollapsedPanel { get { return CollapsedPanel != null && CollapsedPanel.Children.Contains(this) && !NavigationPaneView.IsExpanded; } }
		bool CanShowPeekForm {
			get {
				return HasPeekFormTemplate && NavigationPaneView != null &&
				  (NavigationPaneView.PeekFormShowMode.HasFlag(PeekFormShowMode.Collapsed) && IsInCollapsedPanel ||
				   NavigationPaneView.PeekFormShowMode.HasFlag(PeekFormShowMode.OverflowPanel) && IsInOverflowPanel);
			}
		}
		bool IsFlyoutOpen { get; set; }
		internal void ShowPeekOnMouseEnter() {
			if (!CanShowPeekForm)
				return;
			ShowPeekFormTimer.Stop();
			HidePeekFormTimer.Stop();
			if (!IsFlyoutOpen) {
				int peekShowDelay = (Group.NavBar.View as NavigationPaneView).PeekFormShowDelay;
				ShowPeekFormTimer.Interval = TimeSpan.FromMilliseconds(peekShowDelay);
				ShowPeekFormTimer.Start();
			}
		}
		internal void HidePeekFormOnMouseLeave() {
			if (!CanShowPeekForm)
				return;
			ShowPeekFormTimer.Stop();
			HidePeekFormTimer.Stop();
			if (IsFlyoutOpen) {
				int peekHideDelay = (Group.NavBar.View as NavigationPaneView).PeekFormHideDelay;
				HidePeekFormTimer.Interval = TimeSpan.FromMilliseconds(peekHideDelay);
				HidePeekFormTimer.Start();
			}
		}
		private void OnHidePeekFormTimerTick(object sender, EventArgs e) {
			HidePeekFormTimer.Stop();
			SetFlyoutIsOpen(false);
		}
		void OnShowPeekFormTimerTick(object sender, EventArgs e) {
			ShowPeekFormTimer.Stop();			
			if (!CanShowPeekForm || Flyout == null || Flyout.IsOpen)
				return;
			FlyoutPlacement placement;
			if (IsInOverflowPanel && OverflowPanel.IsMouseOver) {
				placement = FlyoutPlacement.Top;
			} else if (IsInCollapsedPanel) {
				placement = FlyoutPlacement.Right;
			} else
				return;
			if (ActualPeekFormTemplate == null)
				return;
			Flyout flyout = ActualPeekFormTemplate.LoadContent() as Flyout;
			if (flyout != null)
				Flyout.Content = flyout;
			else
				Flyout.ContentTemplate = ActualPeekFormTemplate;
			Flyout.Settings.SetValue(FlyoutSettings.PlacementProperty, placement);
			if (flyout != null)
				Flyout.Settings.SetValue(FlyoutSettings.ShowIndicatorProperty, flyout.ShowIndicator);
			SetFlyoutIsOpen(true);
		}
		void SetFlyoutIsOpen(bool value) {
			if (value) {
				Flyout.DataContext = Group.With(x=>x.DataContext);
				Flyout.PlacementTarget = this;
				Flyout.IsOpen = true;
				IsFlyoutOpen = true;				
			} else if (Flyout != null) {
				Flyout.IsOpen = false;
				Flyout.Popup.IsOpen = false;
				IsFlyoutOpen = false;
			}
		}
		void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			UpdateGroupToolTip();
		}
		protected internal virtual void SetBindingOnAnimationProgress() {
			NavBarGroup group = DataContext as NavBarGroup;
			if (group == null)
				return;
			if (VisualTreeHelper.GetChildrenCount(this) == 0 || group.NavBar == null || group.NavBar.View == null)
				return;
			DXExpander expander = GetDXExpander(this, group.NavBar.View);
			if (expander == null)
				return;
			BindingOperations.SetBinding(group, NavBarAnimationOptions.AnimationProgressProperty, new Binding("AnimationProgress") { Source = expander });
		}
		DXExpander GetDXExpander(DependencyObject reference, NavBarViewBase view) {
			DXExpander expander = GetDXExpanderInChildrens(reference);
			if(expander == null)
				return GetDXExpanderInChildrens(view);
			return expander;
		}
		DXExpander GetDXExpanderInChildrens(DependencyObject reference) {
			int childrenCount = VisualTreeHelper.GetChildrenCount(reference);
			if(childrenCount == 0)
				return null;
			DependencyObject child = null;
			for(int i = 0; i < childrenCount; i++) {
				child = VisualTreeHelper.GetChild(reference, i);
				if(child is DXExpander)
					return (DXExpander)child;
				else {
					child = GetDXExpanderInChildrens(child);
					if(child != null)
						return (DXExpander)child;
				}
			}
			return (DXExpander)child;
		}
		void OnOrientationPropertyChanged() {
			UpdateOrientationStates();
		}
		protected virtual void OnImageSettingsChanged(ImageSettings oldValue) {
		}
		protected virtual void OnLayoutSettingsChanged(LayoutSettings oldValue) {
		}
		protected virtual void OnDisplayModeChanged(DisplayMode oldValue) {
		}	 
		protected virtual void OnShowBorderChanged(bool oldValue) {
			VisualStateManager.GoToState(this, ShowBorder ? "HasBorder" : "NoBorder", false);
		}
		protected internal virtual void UpdateOrientationStates() {
			VisualStateManager.GoToState(this, Orientation == Orientation.Horizontal ? "Horizontal" : "Vertical", false);
		}
		void SetBindings() {
			SetBinding(DisplaySourceProperty, new Binding("DisplaySource"));
			SetBinding(OrientationProperty, new Binding("NavBar.View.Orientation"));
			SetBinding(ShowBorderProperty, new Binding("NavBar.View.ShowBorder"));
			SetBinding(ScrollingSettings.ScrollModeProperty, new Binding("ActualScrollMode"));			
			SetBindingOnAnimationProgress();
			Dispatcher.BeginInvoke(new Action(() => {
				if(Group != null && Group.NavBar != null) {
					ClearValue(System.Windows.Input.KeyboardNavigation.TabNavigationProperty);
					if(Group.NavBar.View is ExplorerBarView)
						SetValue(System.Windows.Input.KeyboardNavigation.TabNavigationProperty, System.Windows.Input.KeyboardNavigationMode.Continue);
					else
						SetBinding(System.Windows.Input.KeyboardNavigation.TabNavigationProperty, new Binding("Group.IsActive") {
							RelativeSource = new RelativeSource(RelativeSourceMode.Self),
							Converter = new BoolToObjectConverter() { FalseValue = System.Windows.Input.KeyboardNavigationMode.None, TrueValue = System.Windows.Input.KeyboardNavigationMode.Contained }
						});
				}
			}));
		}
		void UpdateAnimations() {
			NavBarVisualStateHelper.UpdateStates(this, "OrientationStates");
		}
		void UpdateGroupContentTemplate() {
			if (GroupContentPresenterTemplate == null || GroupItemsControlTemplate == null)
				return;
			ActualGroupContentTemplate = DisplaySource == DisplaySource.Content ? GroupContentPresenterTemplate : GroupItemsControlTemplate;
		}
		void UpdateScrollModeStates() {
			ScrollingSettings.UpdateScrollModeStates(this);
		}
		#region INavBarContainer Members
		readonly RequestContainersWeakEventHandler requestContainerHandler;
		RequestContainersWeakEventHandler INavBarContainer.RequestContainerHandler {
			get { return requestContainerHandler; }
		}
		#endregion
	}
	public class NavPaneActiveGroupControl : NavBarGroupControl {
		public static readonly DependencyProperty IsExpandedProperty;
		public static readonly DependencyProperty ItemsContainerOpacityProperty;
		public Visibility VisibilityCore {
			get { return (Visibility)GetValue(VisibilityCoreProperty); }
			set { SetValue(VisibilityCoreProperty, value); }
		}
		public static readonly DependencyProperty VisibilityCoreProperty =
			DependencyPropertyManager.Register("VisibilityCore", typeof(Visibility), typeof(NavPaneActiveGroupControl), new FrameworkPropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnVisibilityCorePropertyChanged)));
		protected static void OnVisibilityCorePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavPaneActiveGroupControl)d).OnVisibilityCoreChanged((Visibility)e.OldValue);
		}
		protected virtual void OnVisibilityCoreChanged(Visibility oldValue) {
		}
		static NavPaneActiveGroupControl() {
			IsExpandedProperty = DependencyPropertyManager.Register("IsExpanded", typeof(bool), typeof(NavPaneActiveGroupControl), new PropertyMetadata(true, (d, e) => ((NavPaneActiveGroupControl)d).OnIsExpandedPropertyChanged()));
			ItemsContainerOpacityProperty = DependencyPropertyManager.Register("ItemsContainerOpacity", typeof(double), typeof(NavPaneActiveGroupControl), new PropertyMetadata(0.0, (d, e) => ((NavPaneActiveGroupControl)d).OnItemsContainerOpacityPropertyChanged()));			
		}
		public NavPaneActiveGroupControl() {
			this.SetDefaultStyleKey(typeof(NavPaneActiveGroupControl));
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			DataContextChanged += new System.Windows.DependencyPropertyChangedEventHandler(OnDataContextChanged);
		}
		void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			Size old = desiredInfiniteSize;
			InvalidateMeasure();			
			Dispatcher.BeginInvoke(new Action(() => {
				NavBarGroup group = DataContext as NavBarGroup;
				UpdateDesiredInfiniteSize(DesiredSize);												
				if(!old.Equals(desiredInfiniteSize)) {
					FrameworkElement element = this;
					while(!(element is NavigationPaneView) && !(element is NavBarControl) && element != null) {
						element.InvalidateMeasure();
						element = VisualTreeHelper.GetParent(element) as FrameworkElement;
					}
				}
			}));
		}
		void OnUnloaded(object sender, EventArgs e) {
			UnsubscribeTemplateEvents();
		}
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public double OpacityCore {
			get { return (double)GetValue(ItemsContainerOpacityProperty); }
			set { SetValue(ItemsContainerOpacityProperty, value); }
		}
		protected CollapsedActiveGroup CollapsedActiveGroup { get; private set; }
		protected internal CollapsedActiveGroupControl CollapsedActiveGroupControl { get; private set; }
		protected NavPanePopup NavPanePopup { get; private set; }
		protected void SubscribeTemplateEvents() {
			if(CollapsedActiveGroup != null) {
				CollapsedActiveGroup.Checked += new RoutedEventHandler(OnCollapsedActiveGroupChecked);
				CollapsedActiveGroup.Unchecked += new RoutedEventHandler(OnCollapsedActiveGroupUnchecked);
			}
		}
		void OnCollapsedActiveGroupUnchecked(object sender, EventArgs e) {
			UpdateIsPopupOpened();
		}
		void OnCollapsedActiveGroupChecked(object sender, EventArgs e) {
			UpdateIsPopupOpened();
		}
		void UpdateIsPopupOpened() {
			if(CollapsedActiveGroup != null && Group != null && Group.NavBar != null && Group.NavBar.View is NavigationPaneView)
				((NavigationPaneView)Group.NavBar.View).IsPopupOpened = CollapsedActiveGroup.IsChecked.HasValue ? CollapsedActiveGroup.IsChecked.Value : false;
		}
		protected void UnsubscribeTemplateEvents() {
			if(CollapsedActiveGroup != null) {
				CollapsedActiveGroup.Checked -= OnCollapsedActiveGroupChecked;
				CollapsedActiveGroup.Unchecked -= OnCollapsedActiveGroupUnchecked;
			}
		}
		public override void OnApplyTemplate() {
			UnsubscribeTemplateEvents();
			base.OnApplyTemplate();
			CollapsedActiveGroup = GetTemplateChild("collapsedActiveGroup") as CollapsedActiveGroup;
			CollapsedActiveGroupControl = GetTemplateChild("collapsedActiveGroupControl") as CollapsedActiveGroupControl;
			NavPanePopup = GetTemplateChild("popup") as NavPanePopup;
			SubscribeTemplateEvents();
			SetBindings();
			OnIsExpandedPropertyChanged();
			OnItemsContainerOpacityPropertyChanged();			
		}
		void OnLoaded(object sender, EventArgs e) {
			OnIsExpandedPropertyChanged();
			OnItemsContainerOpacityPropertyChanged();
			UnsubscribeTemplateEvents();
			SubscribeTemplateEvents();
		}
		void SetBindings() {
			SetBinding(IsExpandedProperty, new Binding("NavBar.View.IsExpanded"));
			SetBinding(ScrollingSettings.ScrollModeProperty, new Binding("ActualScrollMode"));
			SetBinding(ItemsContainerOpacityProperty, new Binding("(0)") { Path = new PropertyPath(NavBarAnimationOptions.AnimationProgressProperty) });
		}
		protected internal override void UpdateOrientationStates() {
			base.UpdateOrientationStates();
			if(Orientation == Orientation.Vertical) {
				ClearValue(NavBarGroupControl.MinWidthProperty);
				SetBinding(MinHeightProperty, new Binding("DataContext.View.ActiveGroupMinHeight") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
			} else {
				ClearValue(NavBarGroupControl.MinHeightProperty);
				SetBinding(MinWidthProperty, new Binding("DataContext.View.ActiveGroupMinHeight") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
			}
		}
		void OnIsExpandedPropertyChanged() {
			string state = IsExpanded ? "Expanded" : "Collapsed";
			VisualStateManager.GoToState(this, state, false);
			UpdateGroupToolTip();
		}
		void OnItemsContainerOpacityPropertyChanged() {
			string state = OpacityCore == 0.0 ? "ShowContent" : "ShowItems";
			VisualStateManager.GoToState(this, state, false);
		}
		internal Size desiredInfiniteSize = new Size(0,0);
		protected override Size MeasureOverride(Size constraint) {
			return base.MeasureOverride(constraint);
		}
		private void UpdateDesiredInfiniteSize(Size constraint) {
			Size sz = new Size(
				Orientation == Orientation.Vertical ? double.PositiveInfinity : constraint.Width, 
				Orientation == Orientation.Horizontal ? double.PositiveInfinity : constraint.Height);
			desiredInfiniteSize = base.MeasureOverride(sz);
		}
	}
	public class NavBarGroupContentPresenter : ContentPresenter {
		public static readonly DependencyProperty ItemsDataTemplateProperty =
			DependencyPropertyManager.Register("ItemsDataTemplate", typeof(DataTemplate), typeof(NavBarGroupContentPresenter),
			new PropertyMetadata((d, e) => ((NavBarGroupContentPresenter)d).UpdateTemplate()));
		public static readonly DependencyProperty ContentDataTemplateProperty =
			DependencyPropertyManager.Register("ContentDataTemplate", typeof(DataTemplate), typeof(NavBarGroupContentPresenter),
			new PropertyMetadata((d, e) => ((NavBarGroupContentPresenter)d).UpdateTemplate()));
		public static readonly DependencyProperty DisplaySourceProperty =
			DependencyPropertyManager.Register("DisplaySource", typeof(DisplaySource), typeof(NavBarGroupContentPresenter),
			new PropertyMetadata((d, e) => ((NavBarGroupContentPresenter)d).UpdateTemplate()));
		public static readonly DependencyProperty ShowContentProperty =
			DependencyPropertyManager.Register("ShowContent", typeof(bool), typeof(NavBarGroupContentPresenter), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowContentPropertyChanged)));
		public static readonly DependencyProperty ActualContentProperty =
			DependencyPropertyManager.Register("ActualContent", typeof(object), typeof(NavBarGroupContentPresenter), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnActualContentPropertyChanged)));
		protected static void OnActualContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarGroupContentPresenter)d).OnActualContentChanged((object)e.OldValue);
		}
		protected static void OnShowContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarGroupContentPresenter)d).OnShowContentChanged((bool)e.OldValue);
		}
		public object ActualContent {
			get { return (object)GetValue(ActualContentProperty); }
			set { SetValue(ActualContentProperty, value); }
		}
		public bool ShowContent {
			get { return (bool)GetValue(ShowContentProperty); }
			set { SetValue(ShowContentProperty, value); }
		}		
		public DataTemplate ItemsDataTemplate {
			get { return (DataTemplate)GetValue(ItemsDataTemplateProperty); }
			set { SetValue(ItemsDataTemplateProperty, value); }
		}
		public DataTemplate ContentDataTemplate {
			get { return (DataTemplate)GetValue(ContentDataTemplateProperty); }
			set { SetValue(ContentDataTemplateProperty, value); }
		}
		public DisplaySource DisplaySource {
			get { return (DisplaySource)GetValue(DisplaySourceProperty); }
			set { SetValue(DisplaySourceProperty, value); }
		}
		void UpdateTemplate() {
			ContentTemplate = null;
			ContentTemplate = DisplaySource == DisplaySource.Items ? ItemsDataTemplate : ContentDataTemplate;
		}
		protected internal virtual void OnShowContentChanged(bool oldValue) {			
			Content = ShowContent ? ActualContent : null;
		}
		protected internal virtual void OnActualContentChanged(object oldValue) {
			if(ShowContent)
				Content = ActualContent;
		}
	}
}
