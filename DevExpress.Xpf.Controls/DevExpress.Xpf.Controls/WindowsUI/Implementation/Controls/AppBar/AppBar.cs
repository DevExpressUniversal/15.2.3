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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using DevExpress.Xpf.WindowsUI.Navigation;
using DevExpress.Xpf.WindowsUI.UIAutomation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
#if SILVERLIGHT
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.WindowsUI {
	interface IAppBarElement {
		bool IsCompact { get; set; }
	}
	public enum AppBarAlignment {
		Bottom,
		Top
	}
	public enum AppBarHideMode {
		Default,
		Sticky,
		AlwaysVisible
	}
	public delegate void AppBarOpeningEventHandler(object sender, AppBarEventArgs args);
	public delegate void AppBarClosingEventHandler(object sender, AppBarEventArgs args);
	public class AppBarEventArgs : CancelEventArgs { }
#if !SILVERLIGHT
	 [DevExpress.Xpf.Core.DXToolboxBrowsable]
	public class AppBar : veItemsControl, IFlyoutServiceProvider, IWeakEventListener {
#else
	public class AppBar : veItemsControl {
#endif
		#region static
		private FlyoutService _FlyoutService;
		public static readonly DependencyProperty IsOpenProperty;
		public static readonly DependencyProperty IsStickyProperty;
		public static readonly DependencyProperty IsCompactProperty;
		public static readonly DependencyProperty AlignmentProperty;
		public static readonly DependencyProperty ItemSpacingProperty;
		public static readonly DependencyProperty BackCommandProperty;
		public static readonly DependencyProperty ExitCommandProperty;
		public static readonly DependencyProperty IsBackButtonEnabledProperty;
		public static readonly DependencyProperty IsExitButtonEnabledProperty;
		public static readonly DependencyProperty HideModeProperty;
		bool VerifyIsOpen(bool value) {
			if(HideMode == AppBarHideMode.AlwaysVisible && !value)
				return true;
			if(value) {
				if(Opening != null) {
					AppBarEventArgs args = new AppBarEventArgs();
					Opening(this, args);
					if(args.Cancel) return !value;
				}
			} else {
				if(Closing != null) {
					AppBarEventArgs args = new AppBarEventArgs();
					Closing(this, args);
					if(args.Cancel) return !value;
				}
			}
			return value;
		}
		static AppBar() {
			var dProp = new DependencyPropertyRegistrator<AppBar>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("IsOpen", ref IsOpenProperty, false,
				(d, e) => ((AppBar)d).OnIsOpenChanged((bool)e.OldValue, (bool)e.NewValue), (d, e) => {
					return ((AppBar)d).VerifyIsOpen((bool)e);
				});
			dProp.Register("IsSticky", ref IsStickyProperty, false);
			dProp.Register("IsCompact", ref IsCompactProperty, false,
				(d, e) => ((AppBar)d).OnIsCompactChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.Register("Alignment", ref AlignmentProperty, AppBarAlignment.Bottom, FrameworkPropertyMetadataOptions.None,
				(d, e) => ((AppBar)d).OnAlignmentChanged((AppBarAlignment)e.OldValue, (AppBarAlignment)e.NewValue));
			dProp.Register("ItemSpacing", ref ItemSpacingProperty, 0.0);
			dProp.Register("IsBackButtonEnabled", ref IsBackButtonEnabledProperty, false, (d, e) => {
				((AppBar)d).OnIsBackButtonEnabledChanged((bool)(e.OldValue), (bool)(e.NewValue));
			});
			dProp.Register("IsExitButtonEnabled", ref IsExitButtonEnabledProperty, false, (d, e) => {
				((AppBar)d).OnIsExitButtonEnabledChanged((bool)(e.OldValue), (bool)(e.NewValue));
			});
			dProp.Register("BackCommand", ref BackCommandProperty, (ICommand)null);
			dProp.Register("ExitCommand", ref ExitCommandProperty, (ICommand)null);
			dProp.Register("HideMode", ref HideModeProperty, AppBarHideMode.Default, (d, e) => ((AppBar)d).OnHideModeChanged((AppBarHideMode)e.OldValue, (AppBarHideMode)e.NewValue));
		}
		#endregion
		public AppBar() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(AppBar);
			SizeChanged += AppBar_SizeChanged;
			Clip = new RectangleGeometry();
#endif
			ThemeChangedEventManager.AddListener(this, this);
		}
		internal AppBarExitButton PartExitButton { get; set; }
		internal AppBarBackButton PartBackButton { get; set; }
		void OnIsBackButtonEnabledChanged(bool oldValue, bool newValue) {
			if(PartBackButton == null) return;
			if(newValue && PartBackButton.Visibility == Visibility.Collapsed && NavigationHelper.CanGoBack(this)) {
				PartBackButton.Visibility = Visibility.Visible;
			} else {
				PartBackButton.Visibility = Visibility.Collapsed;
			}
		}
		void OnIsExitButtonEnabledChanged(bool oldValue, bool newValue) {
			if(PartExitButton == null) return;
			Window window = LayoutHelper.FindRoot(this) as Window;
			if(window == null || window.WindowStyle != WindowStyle.None
#if SILVERLIGHT
				&& window.WindowStyle != WindowStyle.BorderlessRoundCornersWindow
#endif
) {
				return;
			}
			PartExitButton.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
		}
		void OnHideModeChanged(AppBarHideMode oldValue, AppBarHideMode newValue) {
			if(newValue == AppBarHideMode.AlwaysVisible)
				IsOpen = true;
		}
		void UpdateParts() {
			AppBarButton back = GetTemplateChild("PART_BackButton") as AppBarButton;
			if(back != null) {
				back.Visibility = IsBackButtonEnabled && NavigationHelper.CanGoBack(this) ? Visibility.Visible : Visibility.Collapsed;
			}
			AppBarButton exit = GetTemplateChild("PART_ExitButton") as AppBarButton;
			if(exit != null) {
				exit.Visibility = IsExitButtonEnabled ? Visibility.Visible : Visibility.Collapsed;
			}
			PartAnimatedPanel.Do(x => {
				x.EffectsLayer = GetTemplateChild("PART_EffectsLayer") as Panel;
				x.AttachToVisualTree(this);
			});
		}
		internal void Invalidate() {
			AnimatedPanel panel = LayoutHelper.FindElementByType(this, typeof(AnimatedPanel)) as AnimatedPanel;
			if(panel != null) panel.InvalidateMeasure();
		}
		WeakReference parentRef;
		protected override void OnLoaded() {
			base.OnLoaded();
#if SILVERLIGHT
			FrameworkElement root = LayoutHelper.FindRoot(this) as FrameworkElement;
#else
			FrameworkElement root = Window.GetWindow(this);
			if(root == null) {
				root = LayoutHelper.FindLayoutOrVisualParentObject<NavigationPage>(this, true);
			}
			if(root == null) {
				root = LayoutHelper.FindLayoutOrVisualParentObject<UserControl>(this, true);
			}
#endif
			if(root != null) {
				rootReference = new WeakReference(root);
				root.KeyDown += OnRootKeyDown;
				var parent = LayoutHelper.FindLayoutOrVisualParentObject(this, typeof(NavigationPage)) as UIElement ?? VisualTreeHelper.GetParent(this) as UIElement;
				UIEventService.Subscribe(this, parent);
				parentRef = new WeakReference(parent);
			}
			CompositionTarget.Rendering -= CompositionTarget_Rendering;
			CompositionTarget.Rendering += CompositionTarget_Rendering;
		}
		void CompositionTarget_Rendering(object sender, EventArgs e) {
			AnimatedPanel panel = LayoutHelper.FindElementByType(this, typeof(AnimatedPanel)) as AnimatedPanel;
			if(panel != null) {
				panel.CompositionTarget_Rendering(this, e);
			}
			if(direction == SlideDirection.None) {
				return;
			}
			Func<double, double> ease = (t) => Math.Sin(t * Math.PI * 0.5);
			double step = 0.04;
			if(direction == SlideDirection.Opening)
				shiftParameter += step;
			else
				shiftParameter -= step;
			bool onOpen = false;
			bool onClose = false;
			if(IsOpen && shiftParameter >= 1) {
				shiftParameter = 1;
				onOpen = true;
			}
			if(!IsOpen && shiftParameter <= 0) {
				shiftParameter = 0;
				onClose = true;
			}
			if(PartBorder != null) {
				double easedParameter = ease(shiftParameter);
				Func<double, double> mapY;
				if(IsAtBottom)
					mapY = (t) => PartBorder.ActualHeight * (1.0 - t);
				else
					mapY = (t) => PartBorder.ActualHeight * (-1.0 + t);
				double shiftY = mapY(easedParameter);
				PartBorder.RenderTransform = new TranslateTransform() { X = 0, Y = shiftY };
			}
			if(onClose) {
				direction = SlideDirection.None;
				if(Closed != null) {
					Closed(this, new EventArgs());
				}
				Visibility = Visibility.Collapsed;
			}
			if(onOpen) {
				direction = SlideDirection.None;
				if(Opened != null) {
					Opened(this, new EventArgs());
				}
			}
		}
#if SILVERLIGHT
		void AppBar_SizeChanged(object sender, SizeChangedEventArgs e) {
			RectangleGeometry g = (RectangleGeometry)Clip;
			g.Rect = new Rect() { Width = e.NewSize.Width, Height = e.NewSize.Height };
		}
#endif
		WeakReference rootReference;
		protected override void OnUnloaded() {
			CompositionTarget.Rendering -= CompositionTarget_Rendering;
			if(rootReference != null) {
				FrameworkElement root = rootReference.Target as FrameworkElement;
				if(root != null) {
					root.KeyDown -= OnRootKeyDown;
				}
			}
			if(parentRef != null) {
				UIElement parent = parentRef.Target as UIElement;
				UIEventService.Unsubscribe(this, parent);
				parentRef = null;
			}
			base.OnUnloaded();
		}
		void OnRootKeyDown(object sender, KeyEventArgs e) {
			if(e.Key == Key.Escape && IsOpen) {
				IsOpen = false;
			}
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new AppBarContentPresenter();
		}
		protected override void ClearContainer(DependencyObject element, object item) {
			var presenter = element as AppBarContentPresenter;
			if(presenter != null) presenter.ClearValue(AppBarContentPresenter.HorizontalAlignmentProperty);
			base.ClearContainer(element, item);
		}
		void window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			DependencyObject source = e.OriginalSource as DependencyObject;
#if !SILVERLIGHT
			if(source != null) {
				if(!(source is Visual)) source = LogicalTreeHelper.GetParent(source);
			}
#endif
			if(source == null) return;
			AppBar bar = LayoutHelper.FindParentObject<AppBar>(source);
			if(bar != null) return;
#if !SILVERLIGHT
			if(DevExpress.Xpf.Editors.Flyout.FlyoutControl.GetFlyout(e.OriginalSource as DependencyObject) != null) return;
#endif
#pragma warning disable 612, 618 //Property IsSticky is Obsolete
			if(HideMode == AppBarHideMode.Default && !IsSticky) {
				IsOpen = false;
			}
#pragma warning restore 612, 618
		}
		protected virtual void OnIsCompactChanged(bool oldValue, bool newValue) {
			foreach(var item in Items) {
				IAppBarElement container = ItemContainerGenerator.ContainerFromItem(item) as IAppBarElement;
				if(container != null) container.IsCompact = newValue;
			}
		}
		void OnAlignmentChanged(AppBarAlignment oldValue, AppBarAlignment newValue) {
			VerticalAlignment = newValue == AppBarAlignment.Top ? VerticalAlignment.Top : VerticalAlignment.Bottom;
		}
#if !SILVERLIGHT
		DevExpress.Xpf.Editors.Flyout.FlyoutControl PartFlyoutControl;
#endif
		FrameworkElement PartPresenter;
		Border PartBorder;
		protected override void GetTemplateChildren() {
			base.GetTemplateChildren();
#if !SILVERLIGHT
			PartFlyoutControl = GetTemplateChild("PART_Flyout") as DevExpress.Xpf.Editors.Flyout.FlyoutControl;
#endif
			PartPresenter = GetTemplateChild("PART_ItemsPresenter") as FrameworkElement;
			PartBorder = GetTemplateChild("PART_Border") as Border;
			PartExitButton = GetTemplateChild("PART_ExitButton") as AppBarExitButton;
			PartBackButton = GetTemplateChild("PART_BackButton") as AppBarBackButton;
		}
		override protected void OnApplyTemplateComplete() {
			base.OnApplyTemplateComplete();
			controlInfo.Clear();
			if(PartFlyoutControl != null) {
				PartFlyoutControl.StaysOpen = true;
				PartFlyoutControl.AlwaysOnTop = true;
			}
		}
		AnimatedPanel PartAnimatedPanel { get { return PartItemsPanel as AnimatedPanel; } }
		protected override void ClearTemplateChildren() {
			PartAnimatedPanel.Do(x => x.DetachFromVisualTree(this));
			base.ClearTemplateChildren();
		}
		override protected void EnsureItemsPanelCore(Panel itemsPanel) {
			base.EnsureItemsPanelCore(itemsPanel);
			UpdateParts();
		}
		enum SlideDirection { Opening, Closing, None }
		SlideDirection direction = SlideDirection.None;
		double shiftParameter = 0;
		void OnOpened() {
			Visibility = Visibility.Visible;
			direction = SlideDirection.Opening;
		}
		void OnHide() {
			direction = SlideDirection.Closing;
		}
		protected virtual void OnIsOpenChanged(bool oldValue, bool newValue) {
			if(oldValue == newValue) return;
			if(!IsLoaded && newValue == true) {
				shiftParameter = 1.0;
			}
			if(newValue) OnOpened();
			else OnHide();
		}
		internal void ToggleIsOpen() {
			if(!(HideMode == AppBarHideMode.AlwaysVisible && IsOpen))
				IsOpen = !IsOpen;
		}
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new AppBarAutomationPeer(this);
		}
		#endregion
		bool IsAtBottom {
			get { return Alignment == AppBarAlignment.Bottom; }
		}
		public bool IsOpen {
			get { return (bool)GetValue(IsOpenProperty); }
			set { SetValue(IsOpenProperty, value); }
		}
		[Obsolete("Use AppBar.HideMode property instead")]
		public bool IsSticky {
			get { return (bool)GetValue(IsStickyProperty); }
			set { SetValue(IsStickyProperty, value); }
		}
		public bool IsCompact {
			get { return (bool)GetValue(IsCompactProperty); }
			set { SetValue(IsCompactProperty, value); }
		}
		public AppBarAlignment Alignment {
			get { return (AppBarAlignment)GetValue(AlignmentProperty); }
			set { SetValue(AlignmentProperty, value); }
		}
		public double ItemSpacing {
			get { return (double)GetValue(ItemSpacingProperty); }
			set { SetValue(ItemSpacingProperty, value); }
		}
		public ICommand BackCommand {
			get { return (ICommand)GetValue(BackCommandProperty); }
			set { SetValue(BackCommandProperty, value); }
		}
		public ICommand ExitCommand {
			get { return (ICommand)GetValue(ExitCommandProperty); }
			set { SetValue(ExitCommandProperty, value); }
		}
		public AppBarHideMode HideMode {
			get { return (AppBarHideMode)GetValue(HideModeProperty); }
			set { SetValue(HideModeProperty, value); }
		}
		public event EventHandler Opened;
		public event EventHandler Closed;
		public event AppBarClosingEventHandler Closing;
		public event AppBarOpeningEventHandler Opening;
		public bool IsBackButtonEnabled {
			get { return (bool)GetValue(IsBackButtonEnabledProperty); }
			set { SetValue(IsBackButtonEnabledProperty, value); }
		}
		public bool IsExitButtonEnabled {
			get { return (bool)GetValue(IsExitButtonEnabledProperty); }
			set { SetValue(IsExitButtonEnabledProperty, value); }
		}
		internal List<Generation> controlInfo = new List<Generation>();
		internal int itemsGeneration = 0;
		class UIEventService : DependencyObject {
			#region static
			readonly static Dictionary<UIElement, UIEventService> ownerMap = new Dictionary<UIElement, UIEventService>();
			public static void Subscribe(AppBar listener, UIElement owner) {
				if(owner == null) return;
				UIEventService helper;
				if(!ownerMap.TryGetValue(owner, out helper)) {
					helper = new UIEventService(owner);
					ownerMap[owner] = helper;
				}
				helper.AddListener(listener);
			}
			public static void Unsubscribe(AppBar listener, UIElement owner) {
				if(owner == null) return;
				UIEventService helper;
				if(ownerMap.TryGetValue(owner, out helper)) {
					helper.RemoveListener(listener);
					if(!helper.HasListeners) {
						helper.Unsubscribe();
						ownerMap.Remove(owner);
					}
				}
			}
			#endregion
			readonly int SwipeGestureThreshold = 18;
			readonly int SwipeGestureTop = 3;
			readonly int SwipeGestureBottom = 18;
			public bool HasListeners { get { return listeners.Count > 0; } }
			List<AppBar> listeners = new List<AppBar>();
			UIElement Owner;
			WeakReference rootRef;
			bool isTop;
			bool isBottom;
			bool isInTouch;
			TouchPoint touchStart;
			public UIEventService(UIElement owner) {
				Owner = owner;
				UIElement root = LayoutHelper.FindRoot(owner) as UIElement;
				if(root == null) return;
				rootRef = new WeakReference(root);
				root.MouseLeftButtonUp += OnMouseLeftButtonUp;
				root.StylusSystemGesture += OnStylusSystemGesture;
				root.PreviewTouchUp += OnRootPreviewTouchUp;
#if SILVERLIGHT
				root.MouseRightButtonDown += OnMouseRightButtonDown;
				root.MouseRightButtonUp += OnMouseRightButtonUp;
#else
				FrameworkElement fe = root as FrameworkElement;
				if(fe != null)
					fe.AddHandler(FrameworkElement.ContextMenuOpeningEvent, new RoutedEventHandler(OnContextMenuOpening), true);
#endif
				Touch.FrameReported += OnTouchFrameReported;
			}
			public void AddListener(AppBar listener) {
				if(!listeners.Contains(listener)) {
					listeners.Add(listener);
				}
			}
			public void RemoveListener(AppBar listener) {
				listeners.Remove(listener);
			}
			public void Unsubscribe() {
				if(rootRef == null) return;
				var root = rootRef.Target as UIElement;
				if(root == null) return;
				root.MouseLeftButtonUp -= OnMouseLeftButtonUp;
#if SILVERLIGHT
				root.MouseRightButtonDown -= OnMouseRightButtonDown;
				root.MouseRightButtonUp -= OnMouseRightButtonUp;
#else
				root.StylusSystemGesture -= OnStylusSystemGesture;
				root.PreviewTouchUp -= OnRootPreviewTouchUp;
				FrameworkElement fe = root as FrameworkElement;
				if(fe != null)
					fe.RemoveHandler(FrameworkElement.ContextMenuOpeningEvent, new RoutedEventHandler(OnContextMenuOpening));
#endif
				Touch.FrameReported -= OnTouchFrameReported;
				rootRef = null;
			}
			void OnTouchFrameReportedCore(TouchFrameEventArgs e) {
				TouchPoint tp = e.GetPrimaryTouchPoint(Owner);
				if(tp == null) return;
				switch(tp.Action) {
					case TouchAction.Down:
						var touch = tp;
						isTop = isBottom = false;
						touchStart = null;
						if(touch.Position.Y > 0 && touch.Position.Y < SwipeGestureBottom) {
							touchStart = touch;
							isTop = true;
							e.SuspendMousePromotionUntilTouchUp();
						}
						if(touch.Position.Y > Owner.RenderSize.Height - SwipeGestureBottom && touch.Position.Y < Owner.RenderSize.Height - SwipeGestureTop) {
							touchStart = touch;
							isBottom = true;
							e.SuspendMousePromotionUntilTouchUp();
						}
						break;
					case TouchAction.Move:
						if(!isTop && !isBottom) return;
						if(!HasListeners) return;
						var Touch = tp;
						if(isTop) {
							if(touchStart != null && Touch.Position.Y > (touchStart.Position.Y + SwipeGestureThreshold)) {
								foreach(AppBar bar in listeners) {
									bar.IsOpen = !bar.IsOpen;
								}
								isTop = false;
								isInTouch = true;
							}
						}
						if(isBottom) {
							if(touchStart != null && Touch.Position.Y < (touchStart.Position.Y - SwipeGestureThreshold)) {
								foreach(AppBar bar in listeners) {
									bar.IsOpen = !bar.IsOpen;
								}
								isBottom = false;
								isInTouch = true;
							}
						}
						break;
					case TouchAction.Up:
						break;
				}
			}
			void OnTouchFrameReported(object sender, TouchFrameEventArgs e) {
				var action = new Action<TouchFrameEventArgs>(OnTouchFrameReportedCore);
				if(CheckAccess()) action(e);
				else Dispatcher.Invoke(action, e);
			}
			void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
				if(!isInTouch) {
					foreach(AppBar bar in listeners) {
						bar.window_MouseLeftButtonUp(sender, e);
					}
				}
				isInTouch = false;
			}
#if SILVERLIGHT
			void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
				e.Handled = true;
			}
			void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e) {
				if(!HasListeners) return;
				foreach(AppBar bar in listeners) {
					bar.IsOpen = !bar.IsOpen;
				}
				e.Handled = true;
			}
#else
			void OnRootPreviewTouchUp(object sender, TouchEventArgs e) {
				Dispatcher.BeginInvoke(new Action(() => {
					if(systemGestureLocker)
						NotifyAffectedControls();
					systemGestureLocker.Unlock();
				}));
			}
			Locker systemGestureLocker = new Locker();
			void OnStylusSystemGesture(object sender, StylusSystemGestureEventArgs e) {
				if(e.SystemGesture == SystemGesture.HoldEnter)
					systemGestureLocker.Lock();
			}
			void OnContextMenuOpening(object sender, RoutedEventArgs e) {
				systemGestureLocker.Unlock();
				if(!e.Handled) NotifyAffectedControls();
			}
			void NotifyAffectedControls() {
				if(!HasListeners) return;
				foreach(AppBar bar in listeners) {
					bar.ToggleIsOpen();
				}
			}
#endif
		}
#if !SILVERLIGHT
		#region IFlyoutProvider Members
		Editors.Flyout.FlyoutControl IFlyoutProvider.FlyoutControl {
			get { return PartFlyoutControl; }
		}
		Editors.Flyout.FlyoutPlacement IFlyoutProvider.Placement {
			get { return Alignment == AppBarAlignment.Top ? Editors.Flyout.FlyoutPlacement.Bottom : Editors.Flyout.FlyoutPlacement.Top; }
		}
		IFlyoutEventListener IFlyoutProvider.FlyoutEventListener {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		#endregion
#endif
		#region IFlyoutServiceProvider Members
		internal FlyoutService FlyoutService {
			get {
				if(_FlyoutService == null) _FlyoutService = new FlyoutService(this);
				return _FlyoutService;
			}
		}
		FlyoutService IFlyoutServiceProvider.FlyoutService {
			get { return FlyoutService; }
		}
		#endregion
		protected internal virtual void OnThemeChanged() {
			InvalidateMeasure(); 
		}
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(managerType == typeof(ThemeChangedEventManager)) {
				OnThemeChanged();
				return true;
			}
			return false;
		}
		protected override void OnDispose() {
			ThemeChangedEventManager.RemoveListener(this, this);
			base.OnDispose();
		}
	}
	class Generation : Dictionary<UIElement, ControlInfo> {
	}
	class ControlInfo : DependencyObject {
		public int Frames = 0;
		int StabilizationThreshold = 2;
		public bool IsNew {
			get {
				return Frames <= StabilizationThreshold;
			}
		}
		public bool Stabilized {
			get {
				return Frames == StabilizationThreshold;
			}
		}
		public double? Opacity;
		public double TargetX;
		public double TargetY;
		public bool FadingIn {
			get { return fadingIn; }
			set { fadingIn = value; if(value) fadingOut = false; }
		}
		public bool FadingOut {
			get { return fadingOut; }
			set { fadingOut = value; if(value) fadingIn = false; }
		}
		public bool AllowOpactityChange { get; set; }
		bool fadingIn, fadingOut;
	}
}
namespace DevExpress.Xpf.WindowsUI.Internal {
	class AppBarContentPresenter : ContentPresenter {
		internal AppBarContentPresenter() {
		}
	}
	public class AppBarPanel : Panel {
		public AppBarPanel() {
		}
		protected override Size MeasureOverride(Size availableSize) {
			double width = 0;
			double height = 0;
			foreach(FrameworkElement element in Children) {
				element.Measure(availableSize);
				width += element.DesiredSize.Width;
				height = Math.Max(height, element.DesiredSize.Height);
			}
			return new Size(width, height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Size backbuttonSize = Children[0].DesiredSize;
			Size effectsLayerSize = Children[1].DesiredSize;
			Size exitButtonSize = Children[2].DesiredSize;
			Children[0].Arrange(new Rect(new Point(), backbuttonSize));
			Children[1].Arrange(new Rect(new Point(Children[0].DesiredSize.Width, 0),
				new Size(finalSize.Width - backbuttonSize.Width - exitButtonSize.Width, effectsLayerSize.Height)));
			Children[2].Arrange(new Rect(finalSize.Width - exitButtonSize.Width, 0, exitButtonSize.Width, exitButtonSize.Height));
			return finalSize;
		}
	}
	class ThemeChangedEventManager : WeakEventManager {
		#region static
		protected static ThemeChangedEventManager GetManager() {
			Type managerType = typeof(ThemeChangedEventManager);
			var manager = (ThemeChangedEventManager)WeakEventManager.GetCurrentManager(managerType);
			if(manager == null) {
				manager = new ThemeChangedEventManager();
				WeakEventManager.SetCurrentManager(managerType, manager);
			}
			return manager;
		}
		public static void AddListener(UIElement source, IWeakEventListener listener) {
			GetManager().ProtectedAddListener(source, listener);
		}
		public static void RemoveListener(UIElement source, IWeakEventListener listener) {
			GetManager().ProtectedRemoveListener(source, listener);
		}
		#endregion static
		protected override void StartListening(object source) {
			if(source is DependencyObject)
				Core.ThemeManager.AddThemeChangedHandler((DependencyObject)source, DeliverEvent);
		}
		protected override void StopListening(object source) {
			if(source is DependencyObject)
				Core.ThemeManager.RemoveThemeChangedHandler((DependencyObject)source, DeliverEvent);
		}
	}
}
