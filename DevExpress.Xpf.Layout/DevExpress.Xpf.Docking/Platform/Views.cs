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
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Layout.Core.Platform;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Docking.Platform {
	public class LayoutView : BaseView, IAdornerWindowClient {
		VisualizerAdornerHelper visualizerAdornerHelper;
		ResizingWindowHelper resizingWindowHelper;
		public IUIElement RootElement { get; private set; }
		public UIElement RootUIElement { get; private set; }
		public DockLayoutManager Container { get; private set; }
		public LayoutGroup RootGroup { get; private set; }
		public override bool IsActiveAndCanProcessEvent {
			get { return base.IsActiveAndCanProcessEvent && ZOrder != -1; }
		}
		Rect IAdornerWindowClient.Bounds {
			get { return CalcBounds(); }
		}
		Rect CalcBounds() {
			Point screenLocation = WindowHelper.GetScreenLocation(RootUIElement);
			Size screenSize = RootUIElement.RenderSize;
			if(Container.FlowDirection == FlowDirection.RightToLeft) {
				screenLocation.X = screenLocation.X - screenSize.Width;
			}
			return new Rect(screenLocation, screenSize);
		}
		public bool IsAdornerHelperInitialized {
			get { return visualizerAdornerHelper != null; }
		}
		public VisualizerAdornerHelper AdornerHelper {
			get { return visualizerAdornerHelper; }
		}
		public ResizingWindowHelper ResizingWindowHelper {
			get { return resizingWindowHelper; }
		}
		public LayoutView(IUIElement viewUIElement)
			: base(null, null) {
			RootElement = viewUIElement;
			Initialize(viewUIElement);
			RegisterListeners();
		}
		protected override bool CanUseCustomServiceListener(object key) {
			return Adapter.DragService.DragItem is MDIDocumentElement &&
				(object.Equals(key, OperationType.Reordering) || object.Equals(key, OperationType.Resizing));
		}
		protected override ServiceListener GetCustomUIServiceListener<ServiceListener>(object key) {
			if(object.Equals(key, OperationType.Reordering))
				return new LayoutViewMDIReorderingListener() as ServiceListener;
			if(object.Equals(key, OperationType.Resizing))
				return new LayoutViewMDIResizingListener() as ServiceListener;
			return null;
		}
		protected override ILayoutElementFactory ResolveDefaultFactory() {
			return new LayoutElementFactory();
		}
		protected override ILayoutElementBehavior ResolveDefaultBehavior() {
			return new EmptyBehavior();
		}
		protected internal virtual void Initialize(IUIElement viewUIElement) {
			RootUIElement = CheckRootUIElement(viewUIElement);
			if(RootUIElement != null) {
				Container = DockLayoutManager.GetDockLayoutManager(RootUIElement);
				RootGroup = DockLayoutManager.GetLayoutItem(RootUIElement) as LayoutGroup;
				if(visualizerAdornerHelper != null) visualizerAdornerHelper.Dispose();
				visualizerAdornerHelper = new VisualizerAdornerHelper(this);
				InvalidateZOrder();
			}
			resizingWindowHelper = new ResizingWindowHelper(this);
			SubscribeMouseEvents(RootUIElement);
		}
		protected override bool CheckReordering(Point point) {
			var hit = AdornerHelper.GetHitInfo(point);
			return hit == null || !hit.InHint;
		}
		protected override bool CanSuspendDocking(ILayoutElement dragItem) {
			return RaiseStartDocking(dragItem);
		}
		protected override bool CanSuspendFloating(ILayoutElement dragItem) {
			return RaiseStartDocking(dragItem);
		}
		protected override bool CanSuspendClientDragging(ILayoutElement dragItem) {
			if(LayoutItemsHelper.IsLayoutItem(((IDockLayoutElement)dragItem).Item))
				return false;
			return RaiseStartDocking(dragItem);
		}
		protected override bool CanSuspendBehindDragging(ILayoutElement dragItem) {
			return !Container.EnableNativeDragging && RaiseStartDocking(dragItem);
		}
		protected override bool CanSuspendResizing(ILayoutElement dragItem) {
			BaseLayoutItem item = ((IDockLayoutElement)dragItem).CheckDragElement().Item;
			return Container.RaiseDockOperationStartingEvent(DockOperation.Resize, item);
		}
		protected override bool CanSuspendReordering(ILayoutElement dragItem) {
			if(dragItem is MDIDocumentElement) {
				BaseLayoutItem item = ((IDockLayoutElement)dragItem).CheckDragElement().Item;
				return Container.RaiseDockOperationStartingEvent(DockOperation.Move, item);
			}
			return false;
		}
		bool RaiseStartDocking(ILayoutElement dragItem) {
			BaseLayoutItem item = ((IDockLayoutElement)dragItem).Item;
			return Container.RaiseItemCancelEvent(item, DockLayoutManager.DockItemStartDockingEvent);
		}
		protected virtual UIElement CheckRootUIElement(IUIElement viewUIElement) {
			return viewUIElement as UIElement;
		}
		protected override void OnDispose() {
			UnSubscribeMouseEvents(RootUIElement);
			ReleaseCaptureCore();
			Ref.Dispose(ref visualizerAdornerHelper);
			Ref.Dispose(ref resizingWindowHelper);
			Container = null;
			RootUIElement = null;
			RootElement = null;
			base.OnDispose();
		}
		protected virtual void RegisterListeners() {
			RegisterUIServiceListener(new LayoutViewRegularDragListener());
			RegisterUIServiceListener(new LayoutViewFloatingDragListener());
			RegisterUIServiceListener(new LayoutViewReorderingListener());
			RegisterUIServiceListener(new LayoutViewClientDraggingListener());
			RegisterUIServiceListener(new LayoutViewNonClientDraggingListener());
			RegisterUIServiceListener(new LayoutViewUIInteractionListener());
			RegisterUIServiceListener(new LayoutViewSelectionListener());
			RegisterUIServiceListener(new LayoutViewActionListener());
			RegisterUIServiceListener(new LayoutViewContextActionServiceListener());
		}
		public override HostType Type {
			get { return HostType.Layout; }
		}
		public override object RootKey {
			get { return RootElement; }
		}
		protected override ILayoutElement GetDragItemCore(ILayoutElement element) {
			return ((IDockLayoutElement)element).GetDragItem();
		}
		protected override ILayoutElementBehavior GetElementBehaviorCore(ILayoutElement element) {
			return element is IDockLayoutElement ? ((IDockLayoutElement)element).GetBehavior() : null;
		}
		IDictionary<object, IDisposable> subscriptions;
		MouseEventSubscriber mouseEventSubscriber;
		protected virtual void SubscribeMouseEvents(UIElement element) {
			mouseEventSubscriber = new MouseEventSubscriber(element, this) { Root = RootUIElement };
			SubscribeCore(element, mouseEventSubscriber);
		}
#if SILVERLIGHT
		public virtual void OnMouseButtonEvent(object sender, EventArgs ea) {
			if(ea is MouseButtonEventArgs)
				mouseEventSubscriber.ProcessLeftMouseDown(sender, (MouseButtonEventArgs)ea, false);
		}
#endif
		protected virtual void UnSubscribeMouseEvents(UIElement element) {
			UnSubscribeCore(element);
		}
		void SubscribeCore(object element, IDisposable subscriber) {
			if(element == null) return;
			if(subscriptions == null)
				subscriptions = new Dictionary<object, IDisposable>();
			IDisposable existingSubscriber;
			if(subscriptions.TryGetValue(element, out existingSubscriber)) {
				Ref.Dispose(ref existingSubscriber);
				subscriptions[element] = subscriber;
			}
			else subscriptions.Add(element, subscriber);
		}
		void UnSubscribeCore(object element) {
			if(element == null || subscriptions == null) return;
			IDisposable subscriber;
			if(subscriptions.TryGetValue(element, out subscriber)) {
				Ref.Dispose(ref subscriber);
				subscriptions.Remove(element);
			}
		}
		protected IInputElement KeyboardFocusHolder { get; private set; }
		protected void UnsubscribeKeyboardEvent() {
			if(KeyboardFocusHolder != null) {
				UnSubscribeCore(KeyboardFocusHolder);
				KeyboardFocusHolder = null;
			}
		}
		protected void SubscribeKeyboardEvent() {
			if(KeyboardFocusHolder == KeyHelper.FocusedElement) return;
			if(KeyboardFocusHolder != null)
				UnSubscribeCore(KeyboardFocusHolder);
			KeyboardFocusHolder = KeyHelper.FocusedElement;
			if(KeyboardFocusHolder != null)
				SubscribeCore(KeyboardFocusHolder, new KeyboardEventSubscriber(KeyboardFocusHolder, this));
		}
		internal void RootUIElementKeyDown(object sender, KeyEventArgs e) {
#if !SILVERLIGHT
			if(e.IsDown)
				OnKeyDown(e.Key);
			if(e.IsUp)
				OnKeyUp(e.Key);
#endif
			if(Adapter.DragService.OperationType != OperationType.Regular)
				if(e.Key == Key.Tab) e.Handled = true;
		}
		public override Point ClientToScreen(Point clientPoint) {
			return CoordinateHelper.PointToScreen(Container, RootUIElement, clientPoint);
		}
		public override Point ScreenToClient(Point screenPoint) {
			return CoordinateHelper.PointFromScreen(Container, RootUIElement, screenPoint);
		}
		protected override void SetCaptureCore() {
			RootUIElement.CaptureMouse();
			SubscribeKeyboardEvent();
		}
		protected override void ReleaseCaptureCore() {
			UnsubscribeKeyboardEvent();
			if(RootUIElement != null)
				RootUIElement.ReleaseMouseCapture();
		}
		protected override int CalcZOrder() {
			return ((UIElement)RootElement).GetVisualOrder(Container);
		}
		protected internal virtual DockingHintAdorner CreateDockingHintAdorner(UIElement adornedElement) {
			return new DockingHintAdorner(adornedElement) { HostType = Type };
		}
		protected internal virtual TabHeadersAdorner CreateTabHeadersAdorner(UIElement adornedElement) {
			return new TabHeadersAdorner(adornedElement);
		}
		protected internal virtual ShadowResizeAdorner CreateShadowResizeAdorner(UIElement adornedElement) {
			return new ShadowResizeAdorner(adornedElement);
		}
#if !SILVERLIGHT
		protected internal virtual AdornerWindow GetAdornerWindow() {
			return new AdornerWindow(this, Container);
		}
		internal void OnDesignTimeEvent(object sender, RoutedEventArgs e) {
			if(IsDisposing) return;
			mouseEventSubscriber.OnDesignTimeEvent(sender, e);
		}
#endif
	}
	public class FloatingView : LayoutView {
		public FloatingView(IUIElement viewUIElement)
			: base(viewUIElement) {
		}
		protected override ILayoutElementFactory ResolveDefaultFactory() {
			return new FloatLayoutElementFactory();
		}
		public override HostType Type {
			get { return HostType.Floating; }
		}
		protected override UIElement CheckRootUIElement(IUIElement viewUIElement) {
			return ((FloatPanePresenter)viewUIElement).Element;
		}
		public FloatGroup FloatGroup {
			get { return RootGroup as FloatGroup; }
		}
		protected override void RegisterListeners() {
			base.RegisterListeners();
			RegisterUIServiceListener(new FloatingViewFloatingMovingListener());
			RegisterUIServiceListener(new FloatingViewFloatingResizingListener());
			RegisterUIServiceListener(new FloatingViewUIInteractionListener());
		}
		protected internal void EnterReordering() {
			RootUIElement.Opacity = 0.9; 
		}
		protected internal void LeaveReordering() {
			RootUIElement.ClearValue(FrameworkElement.OpacityProperty);
		}
		protected internal void SetFloatLocation(Point screenLocation) {
			if(!Container.RaiseDockItemDraggingEvent(FloatGroup, screenLocation))
				FloatGroup.FloatLocation = screenLocation;
		}
		protected internal void SetFloatingBounds(Rect screenRect) {
			FloatGroup.FloatLocation = screenRect.Location();
			FloatGroup.FitSizeToContent(screenRect.Size());
			RootUIElement.UpdateLayout();
		}
		public override Point ScreenToClient(Point screenPoint) {
			if(IsDisposing) return new Point(); 
			return base.ScreenToClient(screenPoint);
		}
#if !SILVERLIGHT
		protected internal override AdornerWindow GetAdornerWindow() {
			FloatingWindowPresenter presenter = RootKey as FloatingWindowPresenter;
			return presenter != null ? presenter.EnsureAdornerWindow() : null;
		}
		protected override void ReleaseCaptureCore() {
			base.ReleaseCaptureCore();
			ResizingWindowHelper.Reset();
		}
		protected override bool CanUseCustomServiceListener(object key) {
			bool supportNativeDragging = (object.Equals(key, typeof(IUIInteractionServiceListener)) || object.Equals(key, OperationType.FloatingMoving));
			return base.CanUseCustomServiceListener(key) || (supportNativeDragging && Container.EnableNativeDragging);
		}
		protected override ServiceListener GetCustomUIServiceListener<ServiceListener>(object key) {
			if(object.Equals(key, typeof(IUIInteractionServiceListener)))
				return new FloatingViewNativeInteractionListener() as ServiceListener;
			if(object.Equals(key, OperationType.FloatingMoving))
				return new FloatingViewNativeFloatingMovingListener() as ServiceListener;
			return base.GetCustomUIServiceListener<ServiceListener>(key);
		}
		protected override bool CanSuspendFloatingMoving(ILayoutElement dragItem) {
			if(Container.EnableNativeDragging) return false;
			BaseLayoutItem item = ((IDockLayoutElement)dragItem).CheckDragElement().Item;
			return Container.RaiseDockOperationStartingEvent(DockOperation.Move, item);
		}
#endif
	}
	public class AutoHideView : LayoutView {
		public AutoHideView(IUIElement viewUIElement)
			: base(viewUIElement) {
		}
		protected override void OnDispose() {
			UnSubscribeMouseEvents(Tray);
			UnSubscribeMouseEvents(Tray.Panel);
			base.OnDispose();
		}
		protected override ILayoutElementFactory ResolveDefaultFactory() {
			return new AutoHideLayoutElementFactory();
		}
		public override HostType Type {
			get { return HostType.AutoHide; }
		}
		public AutoHideTray Tray {
			get { return RootKey as AutoHideTray; }
		}
		protected override void RegisterListeners() {
			base.RegisterListeners();
			RegisterUIServiceListener(new AutoHideViewRegularDragListener());
			RegisterUIServiceListener(new AutoHideViewFloatingDragListener());
			RegisterUIServiceListener(new AutoHideViewUIInteractionListener());
			RegisterUIServiceListener(new AutoHideViewResizingListener());
			RegisterUIServiceListener(new AutoHideViewReorderingListener());
			RegisterUIServiceListener(new AutoHideViewActionListener());
		}
		protected internal override void Initialize(IUIElement viewUIElement) {
			base.Initialize(viewUIElement);
			SubscribeMouseEvents(Tray.Panel);
		}
		protected internal void SetPanelBounds(Rect bounds) {
			bool fHorz = AutoHideTray.GetOrientation(Tray) == Orientation.Vertical;
			Tray.DoResizePanel(fHorz ? bounds.Width : bounds.Height);
		}
		protected override void SetCaptureCore() {
			Tray.LockAutoHide();
			base.SetCaptureCore();
		}
		protected override void ReleaseCaptureCore() {
			base.ReleaseCaptureCore();
			Tray.UnlockAutoHide();
#if !SILVERLIGHT
			ResizingWindowHelper.Reset();
#endif
		}
		protected internal override DockingHintAdorner CreateDockingHintAdorner(UIElement adornedElement) {
			return new AutoHideDockHintAdorner(adornedElement);
		}
		protected override ILayoutElementBehavior GetElementBehaviorCore(ILayoutElement element) {
			return Tray.IsAnimated ? null : base.GetElementBehaviorCore(element);
		}
		protected override bool CanUseCustomServiceListener(object key) {
			return false;
		}
	}
}
