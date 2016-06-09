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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking.Platform;
using DevExpress.Xpf.Layout.Core;
using LK = DevExpress.Xpf.Docking.Platform.FloatingWindowLock.LockerKey;
namespace DevExpress.Xpf.Docking.VisualElements {
	public abstract class FloatPanePresenter : BaseFloatingContainer, IUIElement, IDisposable {
		#region static
		public static readonly DependencyProperty BorderStyleProperty;
		public static readonly DependencyProperty IsMaximizedProperty;
		public static readonly DependencyProperty FormBorderMarginProperty;
		public static readonly DependencyProperty SingleBorderMarginProperty;
		public static readonly DependencyProperty MaximizedBorderMarginProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualContentMarginProperty;
		public static readonly DependencyProperty ShadowMarginProperty;
		public static readonly DependencyProperty BorderMarginProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualShadowMarginProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualBorderMarginProperty;
		static FloatPanePresenter() {
			var dProp = new DependencyPropertyRegistrator<FloatPanePresenter>();
			dProp.OverrideFrameworkMetadata(IsOpenProperty, true);
			dProp.Register("BorderStyle", ref BorderStyleProperty, FloatGroupBorderStyle.Empty,
				(dObj, e) => ((FloatPanePresenter)dObj).OnBorderStyleChanged((FloatGroupBorderStyle)e.NewValue));
			dProp.Register("IsMaximized", ref IsMaximizedProperty, false,
				(dObj, e) => ((FloatPanePresenter)dObj).OnIsMaximizedChanged((bool)e.NewValue));
			dProp.Register("ShadowMargin", ref ShadowMarginProperty, new Thickness(double.NaN),
				(dObj, e) => ((FloatPanePresenter)dObj).OnBorderMarginChanged());
			dProp.Register("BorderMargin", ref BorderMarginProperty, new Thickness(double.NaN),
				(dObj, e) => ((FloatPanePresenter)dObj).OnBorderMarginChanged());
			dProp.Register("ActualBorderMargin", ref ActualBorderMarginProperty, new Thickness(0), null,
				(dObj, value) => ((FloatPanePresenter)dObj).CoerceActualBorderMargin());
			dProp.Register("ActualShadowMargin", ref ActualShadowMarginProperty, new Thickness(0), null,
				(dObj, value) => ((FloatPanePresenter)dObj).CoerceActualShadowMargin());
			dProp.Register("SingleBorderMargin", ref SingleBorderMarginProperty, new Thickness(double.NaN),
				(dObj, e) => ((FloatPanePresenter)dObj).OnContentMarginChanged());
			dProp.Register("FormBorderMargin", ref FormBorderMarginProperty, new Thickness(double.NaN),
				(dObj, e) => ((FloatPanePresenter)dObj).OnContentMarginChanged());
			dProp.Register("MaximizedBorderMargin", ref MaximizedBorderMarginProperty, new Thickness(double.NaN),
				(dObj, e) => ((FloatPanePresenter)dObj).OnContentMarginChanged());
			dProp.Register("ActualContentMargin", ref ActualContentMarginProperty, new Thickness(0), null,
				(dObj, value) => ((FloatPanePresenter)dObj).CoerceActualContentMargin());
		}
		#endregion static
		bool isDisposing = false;
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				DeactivateContentHolder();
				OnDisposing();
				Owner = null;
				if(Presenter != null)
					Presenter.Content = null;
				if(Decorator != null)
					Decorator.Child = null;
				DockLayoutManager.Release(Presenter);
				DockLayoutManager.Release(this);
			}
			GC.SuppressFinalize(this);
		}
		#region Properties
		public FloatGroupBorderStyle BorderStyle {
			get { return (FloatGroupBorderStyle)GetValue(BorderStyleProperty); }
			set { SetValue(BorderStyleProperty, value); }
		}
		public bool IsMaximized {
			get { return (bool)GetValue(IsMaximizedProperty); }
			set { SetValue(IsMaximizedProperty, value); }
		}
		public Thickness ShadowMargin {
			get { return (Thickness)GetValue(ShadowMarginProperty); }
			set { SetValue(ShadowMarginProperty, value); }
		}
		public Thickness BorderMargin {
			get { return (Thickness)GetValue(BorderMarginProperty); }
			set { SetValue(BorderMarginProperty, value); }
		}
		public Thickness FormBorderMargin {
			get { return (Thickness)GetValue(FormBorderMarginProperty); }
			set { SetValue(FormBorderMarginProperty, value); }
		}
		public Thickness SingleBorderMargin {
			get { return (Thickness)GetValue(SingleBorderMarginProperty); }
			set { SetValue(SingleBorderMarginProperty, value); }
		}
		public Thickness MaximizedBorderMargin {
			get { return (Thickness)GetValue(MaximizedBorderMarginProperty); }
			set { SetValue(MaximizedBorderMarginProperty, value); }
		}
		public Thickness ActualShadowMargin {
			get { return (Thickness)GetValue(ActualShadowMarginProperty); }
		}
		public Thickness ActualBorderMargin {
			get { return (Thickness)GetValue(ActualBorderMarginProperty); }
		}
		public Thickness ActualContentMargin {
			get { return (Thickness)GetValue(ActualContentMarginProperty); }
		}
		#endregion Properties
		protected virtual void OnBorderStyleChanged(FloatGroupBorderStyle borderStyle) {
			CoerceValue(ActualContentMarginProperty);
			if(Presenter != null)
				((FloatingContentPresenter)Presenter).UpdateBorderStyle(borderStyle);
		}
		protected virtual void OnContentMarginChanged() {
			CoerceValue(ActualContentMarginProperty);
		}
		protected virtual void OnBorderMarginChanged() {
			CoerceValue(ActualBorderMarginProperty);
			CoerceValue(ActualShadowMarginProperty);
		}
		protected virtual Thickness CoerceActualContentMargin() {
			Thickness margin = MaximizedBorderMargin;
			if(!IsMaximized)
				margin = (BorderStyle == FloatGroupBorderStyle.Single) ? SingleBorderMargin : FormBorderMargin;
			return !MathHelper.AreEqual(margin, new Thickness(double.NaN)) ? margin : new Thickness(0);
		}
		protected virtual Thickness CoerceActualShadowMargin() {
			Thickness margin = !IsMaximized ? ShadowMargin : new Thickness(0);
			return !MathHelper.AreEqual(margin, new Thickness(double.NaN)) ? margin : new Thickness(0);
		}
		protected virtual Thickness CoerceActualBorderMargin() {
			Thickness margin = !IsMaximized ? BorderMargin : new Thickness(0);
			return !MathHelper.AreEqual(margin, new Thickness(double.NaN)) ? margin : new Thickness(0);
		}
		protected virtual void OnIsMaximizedChanged(bool maximized) {
			CoerceValue(ActualContentMarginProperty);
			CoerceValue(ActualBorderMarginProperty);
			CoerceValue(ActualShadowMarginProperty);
			if(Presenter != null)
				((FloatingContentPresenter)Presenter).UpdateResizeBordersVisibility(maximized);
		}
		public UIElement Element { get { return Presenter as UIElement; } }
		#region IUIElement
		IUIElement IUIElement.Scope {
			get { return DockLayoutManager.GetDockLayoutManager(this); }
		}
		UIChildren uiChildren = new UIChildren();
		UIChildren IUIElement.Children {
			get {
				if(uiChildren == null) uiChildren = new UIChildren();
				return uiChildren;
			}
		}
		#endregion IUIElement
		protected DockLayoutManager Container { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Container = DockLayoutManager.Ensure(this);
		}
		public void Activate(DockLayoutManager container) {
			this.BringToFront(container);
			ActivateContentHolder();
		}
		public void Deactivate() {
			OnIsOpenChanged(false);
			DeactivateContentHolder();
		}
		protected override void OnContentChanged(object content) {
			Container = DockLayoutManager.Ensure(this);
			if(content == null && Container == null)
				Deactivate();
			Owner = Container;
			base.OnContentChanged(content);
		}
		protected override BaseFloatingContainer.ManagedContentPresenter CreateContentPresenter() {
			return new FloatingContentPresenter(this);
		}
		protected override void OnHierarchyCreated() {
			base.OnHierarchyCreated();
			this.Forward(Presenter, FloatingContentPresenter.ActualContentMarginProperty, "ActualContentMargin");
			this.Forward(Presenter, FloatingContentPresenter.ActualBorderMarginProperty, "ActualBorderMargin");
			this.Forward(Presenter, FloatingContentPresenter.ActualShadowMarginProperty, "ActualShadowMargin");
		}
		protected override void OnContentUpdated(object content, FrameworkElement owner) { }
		protected abstract void OnDisposing();
		protected abstract void ActivateContentHolder();
		protected abstract void DeactivateContentHolder();
		#region Internal Classes
		[TemplatePart(Name = "PART_Content", Type = typeof(UIElement))]
		[TemplatePart(Name = "PART_SingleBorder", Type = typeof(Grid))]
		[TemplatePart(Name = "PART_FormBorder", Type = typeof(Grid))]
		[TemplatePart(Name = "PART_ResizeBorders", Type = typeof(Grid))]
		public class FloatingContentPresenter : ManagedContentPresenter {
			#region static
			public static readonly DependencyProperty ActualContentMarginProperty;
			public static readonly DependencyProperty ActualShadowMarginProperty;
			public static readonly DependencyProperty ActualBorderMarginProperty;
			static FloatingContentPresenter() {
				var dProp = new DependencyPropertyRegistrator<FloatingContentPresenter>();
				dProp.Register("ActualContentMargin", ref ActualContentMarginProperty, new Thickness(0), null);
				dProp.Register("ActualShadowMargin", ref ActualShadowMarginProperty, new Thickness(0), null);
				dProp.Register("ActualBorderMargin", ref ActualBorderMarginProperty, new Thickness(0), null);
			}
			#endregion static
			public FloatingContentPresenter(FloatPanePresenter container)
				: base(container) {
			}
			public UIElement PartContent { get; private set; }
			public FrameworkElement PartSingleBorder { get; private set; }
			public FrameworkElement PartFormBorder { get; private set; }
			public FrameworkElement PartResizeBorders { get; private set; }
			public DockLayoutManager Manager { get; private set; }
			public FormBorderControl PartBorderControl { get; private set; }
			public override void OnApplyTemplate() {
				base.OnApplyTemplate();
				Manager = DockLayoutManager.Ensure(this);
				if(Manager != null) {
					FloatingView view = Manager.ViewAdapter.GetView(Container) as FloatingView;
					if(view != null)
						view.Initialize((IUIElement)Container);
				}
				PartContent = GetTemplateChild("PART_Content") as UIElement;
				PartSingleBorder = GetTemplateChild("PART_SingleBorder") as FrameworkElement;
				PartFormBorder = GetTemplateChild("PART_FormBorder") as FrameworkElement;
				PartResizeBorders = GetTemplateChild("PART_ResizeBorders") as FrameworkElement;
				PartBorderControl = GetTemplateChild("PART_FormBorderControl") as FormBorderControl;
				UpdateBorderStyle(((FloatPanePresenter)Container).BorderStyle);
				UpdateResizeBordersVisibility(((FloatPanePresenter)Container).IsMaximized);
				SetupBindings();
			}
			void SetupBindings() {
				if(PartBorderControl == null) return;
				var presenter = ((FloatPanePresenter)Container);
				BindingHelper.SetBinding(PartBorderControl, FormBorderControl.ActualBorderMarginProperty, presenter, FloatPanePresenter.ActualBorderMarginProperty);
				BindingHelper.SetBinding(PartBorderControl, FormBorderControl.ActualContentMarginProperty, presenter, FloatPanePresenter.ActualContentMarginProperty);
				BindingHelper.SetBinding(PartBorderControl, FormBorderControl.ActualShadowMarginProperty, presenter, FloatPanePresenter.ActualShadowMarginProperty);
				BindingHelper.SetBinding(PartBorderControl, FormBorderControl.BorderStyleProperty, presenter, FloatPanePresenter.BorderStyleProperty);
				BindingHelper.SetBinding(PartBorderControl, FormBorderControl.IsMaximizedProperty, presenter, FloatPanePresenter.IsMaximizedProperty);
			}
			internal void UpdateBorderStyle(FloatGroupBorderStyle borderStyle) {
				UpdateSingleBorderVisibility(borderStyle);
				UpdateFormBorderVisibility(borderStyle);
			}
			internal void UpdateResizeBordersVisibility(bool maximized) {
				if(PartResizeBorders == null) return;
				PartResizeBorders.Visibility = maximized ? Visibility.Collapsed : Visibility.Visible;
			}
			void UpdateSingleBorderVisibility(FloatGroupBorderStyle borderStyle) {
				if(PartSingleBorder == null) return;
				PartSingleBorder.Visibility = (borderStyle == FloatGroupBorderStyle.Single) ? Visibility.Visible : Visibility.Collapsed;
			}
			void UpdateFormBorderVisibility(FloatGroupBorderStyle borderStyle) {
				if(PartFormBorder == null) return;
				PartFormBorder.Visibility = (borderStyle == FloatGroupBorderStyle.Form) ? Visibility.Visible : Visibility.Collapsed;
			}
			public Thickness GetFloatingMargin() {
				return new Thickness(6);
			}
			public Thickness ActualShadowMargin {
				get { return (Thickness)GetValue(ActualShadowMarginProperty); }
				set { SetValue(ActualShadowMarginProperty, value); }
			}
			public Thickness ActualBorderMargin {
				get { return (Thickness)GetValue(ActualBorderMarginProperty); }
				set { SetValue(ActualBorderMarginProperty, value); }
			}
			public Thickness ActualContentMargin {
				get { return (Thickness)GetValue(ActualContentMarginProperty); }
				set { SetValue(ActualContentMarginProperty, value); }
			}
		}
		#endregion
		public static void Invalidate(DependencyObject dObj) {
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get { return new Bars.SingleLogicalChildEnumerator(Presenter); }
		}
	}
	public class FloatingWindowPresenter : FloatPanePresenter {
		bool Win32Compatible { get { return Container.IsTransparencyDisabled && EnvironmentHelper.IsWinXP; } }
		FloatingPaneWindow windowCore;
		AdornerWindow adornerWindowCore;
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			UnlockUpdateIfNeeded();
			EnsureAdornerWindow();
			isFloatingWindowLocked = false;
		}
		protected override void OnUnloaded(object sender, RoutedEventArgs e) {
			DisposeAdornerWindow();
			base.OnUnloaded(sender, e);
		}
		bool ShouldPostponeUpdate() {
			var dockManager = Container ?? DockLayoutManager.GetDockLayoutManager(this);
			bool isManagerReady = dockManager != null && dockManager.IsTransparencyDisabled && !dockManager.IsFloating;
			return isManagerReady && ContainerTemplate == null && Content != null;
		}
		protected override void OnContentChanged(object content) {
			LockUpdateIfNeeded();
			base.OnContentChanged(content);
		}
		System.Windows.Threading.DispatcherOperation updateOperation;
		void LockUpdateIfNeeded() {
			if(updateOperation == null && ShouldPostponeUpdate()) {
				BeginUpdate();
				updateOperation = Dispatcher.BeginInvoke(new Action(() => {
					updateOperation = null;
					EndUpdate();
				}));
			}
		}
		void UnlockUpdateIfNeeded() {
			if(updateOperation != null) {
				updateOperation.Abort();
				if(IsUpdateLocked) EndUpdate();
			}
		}
		public AdornerWindow AdornerWindow { get { return EnsureAdornerWindow(); } }
		protected override void OnDisposing() {
			if(updateOperation != null) updateOperation.Abort();
			DisposeAdornerWindow(true);
			Ref.Dispose(ref windowCore);
		}
		void DisposeAdornerWindow(bool force = false) {
			if(!Win32Compatible || force) Ref.Dispose(ref adornerWindowCore);
		}
		protected virtual AdornerWindow GetWin32AdornerWindow(FloatingPaneWindow window) {
			return Container.Win32AdornerWindowProvider.GetWindow(window, Container);
		}
		protected virtual AdornerWindow CreateAdornerWindow(FloatingPaneWindow window) {
			return new AdornerWindow(window, Container);
		}
		protected override UIElement CreateContentContainer() {
			this.windowCore = CreateWindow();
			return windowCore;
		}
		protected virtual FloatingPaneWindow CreateWindow() {
			return new FloatingPaneWindow(this);
		}
		protected internal FloatingPaneWindow Window {
			get { return windowCore; }
		}
		protected override bool IsAlive {
			get { return windowCore != null; }
		}
		bool isFloatingWindowLocked;
		protected override void OnIsOpenChanged(bool isOpen) {
			if(Window != null) {
				if(!isOpen) {
					isFloatingWindowLocked = true;
					Window.Owner = null;
				}
				else
					Window.UpdateOwnerWindow();
			}
			base.OnIsOpenChanged(isOpen);
		}
		bool isTemplateAssigned;
		protected override void UpdatePresenterContentTemplate() {
			base.UpdatePresenterContentTemplate();
			isTemplateAssigned = (ContainerTemplate != null);
			if(isTemplateAssigned && isHiddenInitialization()) {
				CheckBoundsInContainer();
				Window.hiddenInitialization = false;
			}
		}
		bool isHiddenInitialization() {
			return IsAlive && Window.hiddenInitialization;
		}
		protected override void UpdateFloatingBoundsCore(Rect bounds) {
			if(isFloatingWindowLocked) return;
			if(!isTemplateAssigned || Container.IsFloating) return;
			if(Window.FlowDirection == System.Windows.FlowDirection.RightToLeft)
				bounds.X = bounds.Left + bounds.Width;
			Window.SetFloatingBounds(Container, bounds);
		}
		bool lockShow = false;
		protected override void UpdateIsOpenCore(bool isOpen) {
			if(lockShow) return;
			if(isOpen) {
				if(!Window.IsVisible) {
					if(Container.IsTransparencyDisabled && Container.IsFloating)
						Window.Height = Window.Width = 0;
					bool allowsTransparency = Window.AllowsTransparency;
					bool showActivated = Window.ShowActivated = Container.IsFloating || Window.WindowState == WindowState.Maximized;
					if(allowsTransparency)
						Window.Opacity = 0;
					Window.Focusable = false;
					Window.UpdateRenderOptions();
					lockShow = true;
					Window.LockHelper.Lock(LK.FloatingBounds);
					Window.Show();
					Window.LockHelper.Unlock(LK.FloatingBounds);
					lockShow = false;
					if(showActivated)
						WindowHelper.BringToFront(Window);
					Window.Focusable = true;
					if(allowsTransparency) {
						var showAnimation = new DoubleAnimation(1.0, new Duration(TimeSpan.FromMilliseconds(100)));
						showAnimation.Freeze();
						Window.BeginAnimation(OpacityProperty, showAnimation);
					}
				}
			}
			else Window.Hide();
		}
		protected override void AddDecoratorToContentContainer(NonLogicalDecorator decorator) {
			Window.Content = decorator;
		}
		protected override void ActivateContentHolder() {
			if(!Window.IsActive) Window.Activate();
		}
		protected override void DeactivateContentHolder() {
			Window owner = (Window != null) ? Window.Owner : null;
			Ref.Dispose(ref windowCore);
			DisposeAdornerWindow();
			if(owner != null && Container.FloatGroups.Where(x => x.IsActuallyVisible).Count() == 0)
				owner.Activate();
		}
		internal AdornerWindow EnsureAdornerWindow() {
			if(!IsAlive) return null;
			if(Win32Compatible) return GetWin32AdornerWindow(Window);
			if(adornerWindowCore == null || adornerWindowCore.IsDisposing)
				this.adornerWindowCore = CreateAdornerWindow(Window);
			return adornerWindowCore;
		}
	}
	public class FloatingAdornerPresenter : FloatPanePresenter {
		#region static
		public static readonly DependencyProperty SizeToContentProperty =
			DependencyProperty.Register("SizeToContent", typeof(SizeToContent), typeof(FloatingAdornerPresenter), new PropertyMetadata(SizeToContent.Manual, OnSizeToContentChanged));
		private static void OnSizeToContentChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			FloatingAdornerPresenter floatingAdornerPresenter = dObj as FloatingAdornerPresenter;
			if(floatingAdornerPresenter != null)
				floatingAdornerPresenter.OnSizeToContentChanged((SizeToContent)e.OldValue, (SizeToContent)e.NewValue);
		}
		static bool CanUpdateFloatingBounds(DockLayoutManager container) {
			UIElement adornerSource = DevExpress.Xpf.Core.Native.LayoutHelper.GetTopContainerWithAdornerLayer(container);
			return adornerSource != null && adornerSource.IsArrangeValid && container.IsArrangeValid;
		}
		#endregion
		FloatingPaneAdornerElement adornerElementCore;
		protected override void OnDisposing() {
			adornerElementCore = null;
		}
		public SizeToContent SizeToContent {
			get { return (SizeToContent)GetValue(SizeToContentProperty); }
			set { SetValue(SizeToContentProperty, value); }
		}
		protected internal FloatingPaneAdornerElement AdornerElement {
			get { return adornerElementCore; }
		}
		protected override bool IsAlive {
			get { return adornerElementCore != null; }
		}
		protected virtual void OnSizeToContentChanged(SizeToContent oldValue, SizeToContent newValue) {
			UpdateFloatingBoundsCore(new Rect(FloatLocation, FloatSize));
		}
		protected override void AddDecoratorToContentContainer(NonLogicalDecorator decorator) {
			AdornerElement.Child = decorator;
			Container.DragAdorner.Register(AdornerElement);
			Container.DragAdorner.SetVisible(AdornerElement, true);
		}
		protected override UIElement CreateContentContainer() {
			Container.DragAdorner.Activate();
			this.adornerElementCore = CreateAdornerElement(Container);
			return adornerElementCore;
		}
		protected override void OnHierarchyCreated() {
			base.OnHierarchyCreated();
			adornerElementCore.EnsureFlowDirection();
		}
		protected virtual FloatingPaneAdornerElement CreateAdornerElement(DockLayoutManager container) {
			return new FloatingPaneAdornerElement(this);
		}
		protected override void UpdateFloatingBoundsCore(Rect bounds) {
			if(CanUpdateFloatingBounds(Container)) InvokeFloatingBoundsUpdate(bounds);
			else {
				if(Container.IsDisposing || AdornerElement == null) return;
				Dispatcher.BeginInvoke(new Action(() =>
				{
					if(Container.IsDisposing || AdornerElement == null) return;
					InvokeFloatingBoundsUpdate(bounds);
				}));
			}
		}
		Thickness GetActualTreshold() {
			bool isAeroMode = WindowHelper.IsAeroMode(Container);
			return isAeroMode ? new Thickness(3, 3, 14, 14) : new Thickness(15);
		}
		void InvokeFloatingBoundsUpdate(Rect bounds) {
			bounds = GeometryHelper.CorrectBounds(bounds, CoordinateHelper.GetAvailableAdornerRect(Container, bounds), GetActualTreshold());
			if(MathHelper.IsEmpty(bounds.Size)) return;
			Container.DragAdorner.SetBoundsInContainer(AdornerElement, bounds);
		}
		protected override void UpdateIsOpenCore(bool isOpen) {
			Container.DragAdorner.SetVisible(AdornerElement, isOpen);
		}
		protected override void ActivateContentHolder() {
			Container.DragAdorner.BringToFront(AdornerElement);
		}
		protected override void DeactivateContentHolder() {
			Container.DragAdorner.Unregister(AdornerElement);
		}
	}
}
