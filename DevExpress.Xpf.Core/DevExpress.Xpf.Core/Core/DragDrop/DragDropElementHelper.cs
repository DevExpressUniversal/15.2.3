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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Threading;
using DevExpress.Xpf.Utils;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections.Generic;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using RoutedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler;
using Visual = System.Windows.FrameworkElement;
#endif
namespace DevExpress.Xpf.Core {
	public abstract class IndependentMouseEventArgs : RoutedEventArgs {
		readonly object originalSource;
		public IndependentMouseEventArgs(object originalSource) {
			this.originalSource = originalSource;
		}
		public new object OriginalSource { get { return originalSource; } }
		public abstract Point GetPosition(UIElement relativeTo);
		public abstract void CaptureMouse(UIElement element);
		public abstract void ReleaseMouseCapture(UIElement element);
#if !SL
		public abstract MouseButtonState LeftButton { get; }
#endif
	}
	public abstract class IndependentMouseButtonEventArgs : IndependentMouseEventArgs {
		public IndependentMouseButtonEventArgs(object originalSource) 
			: base(originalSource) {
		}
	}
	public delegate void IndependentMouseEventHandler(object sender, IndependentMouseEventArgs e);
	public delegate void IndependentMouseButtonEventHandler(object sender, IndependentMouseButtonEventArgs e);
	public class PlatformMouseEventArgs : IndependentMouseEventArgs {
		internal static void CaptureMouseCore(UIElement element) {
#if !SILVERLIGHT
			Mouse.Capture(element);
#else
			element.CaptureMouse();
#endif
		}
		internal static void ReleaseMouseCaptureCore(UIElement element) {
#if !SILVERLIGHT
			Mouse.Capture(null);
#else
			element.ReleaseMouseCapture();
#endif
		}
		protected readonly MouseEventArgs e;
		public PlatformMouseEventArgs(MouseEventArgs e) 
			: base(e.OriginalSource) {
			this.e = e;
		}
#if !SL
		public override MouseButtonState LeftButton { get { return Mouse.LeftButton; } }
#endif
		public override Point GetPosition(UIElement relativeTo) {
			return e.GetPosition(relativeTo);
		}
		public override void CaptureMouse(UIElement element) {
			CaptureMouseCore(element);
		}
		public override void ReleaseMouseCapture(UIElement element) {
			ReleaseMouseCaptureCore(element);
		}
	}
	public class PlatformMouseButtonEventArgs : IndependentMouseButtonEventArgs {
		protected readonly MouseButtonEventArgs e;
		internal MouseButtonEventArgs OrigninalEventArgs { get { return e; } }
		public PlatformMouseButtonEventArgs(MouseButtonEventArgs e)
			: base(e.OriginalSource) {
			this.e = e;
		}
#if !SL
		public override MouseButtonState LeftButton { get { return Mouse.LeftButton; } }
#endif
		public override Point GetPosition(UIElement relativeTo) {
			return e.GetPosition(relativeTo);
		}
		public override void CaptureMouse(UIElement element) {
			PlatformMouseEventArgs.CaptureMouseCore(element);
		}
		public override void ReleaseMouseCapture(UIElement element) {
			PlatformMouseEventArgs.ReleaseMouseCaptureCore(element);
		}
	}
	public class InputEventsHelper {
		public static readonly DependencyProperty RaiseButtonClickOnLeftMouseLeftButtonUpProperty;
		public static readonly RoutedEvent MouseLeftButtonDownEvent;
		public static readonly RoutedEvent MouseLeftButtonUpEvent;
		public static readonly RoutedEvent MouseMoveEvent;
		static InputEventsHelper() {
			Type ownerType = typeof(InputEventsHelper);
			MouseLeftButtonDownEvent = EventManager.RegisterRoutedEvent("MouseLeftButtonDown", RoutingStrategy.Direct, typeof(IndependentMouseButtonEventHandler), ownerType);
			MouseLeftButtonUpEvent = EventManager.RegisterRoutedEvent("MouseLeftButtonUp", RoutingStrategy.Direct, typeof(IndependentMouseButtonEventHandler), ownerType);
			MouseMoveEvent = EventManager.RegisterRoutedEvent("MouseMove", RoutingStrategy.Direct, typeof(IndependentMouseEventHandler), ownerType);
			RaiseButtonClickOnLeftMouseLeftButtonUpProperty = DependencyProperty.RegisterAttached("RaiseButtonClickOnLeftMouseLeftButtonUp", typeof(bool), ownerType, new PropertyMetadata(false, OnRaiseButtonClickOnLeftMouseLeftButtonUpChanged));
		}
		public static void AddMouseLeftButtonDownHandler(DependencyObject dObj, IndependentMouseButtonEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(MouseLeftButtonDownEvent, handler);
		}
		public static void RemoveMouseLeftButtonDownHandler(DependencyObject dObj, IndependentMouseButtonEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(MouseLeftButtonDownEvent, handler);
		}
		public static void AddMouseLeftButtonUpHandler(DependencyObject dObj, IndependentMouseButtonEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(MouseLeftButtonUpEvent, handler);
		}
		public static void RemoveMouseLeftButtonUpHandler(DependencyObject dObj, IndependentMouseButtonEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(MouseLeftButtonUpEvent, handler);
		}
		public static void AddMouseMoveHandler(DependencyObject dObj, IndependentMouseEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.AddHandler(MouseMoveEvent, handler);
		}
		public static void RemoveMouseMoveHandler(DependencyObject dObj, IndependentMouseEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) uiElement.RemoveHandler(MouseMoveEvent, handler);
		}
		public static bool GetRaiseButtonClickOnLeftMouseLeftButtonUp(Button obj) {
			return (bool)obj.GetValue(RaiseButtonClickOnLeftMouseLeftButtonUpProperty);
		}
		public static void SetRaiseButtonClickOnLeftMouseLeftButtonUp(Button obj, bool value) {
			obj.SetValue(RaiseButtonClickOnLeftMouseLeftButtonUpProperty, value);
		}
		static void OnRaiseButtonClickOnLeftMouseLeftButtonUpChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Button button = d as Button;
			bool newValue = (bool)e.NewValue;
			if(button == null)
				return;
			if(newValue)
				AddMouseLeftButtonUpHandler(button, OnButtonMouseLeftButtonUp);
			else
				RemoveMouseLeftButtonUpHandler(button, OnButtonMouseLeftButtonUp);
		}
		static void OnButtonMouseLeftButtonUp(object sender, IndependentMouseButtonEventArgs e) {
			Button button = (Button)sender;
			ButtonBaseAutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(button) as ButtonBaseAutomationPeer;
			IInvokeProvider invoker = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
			invoker.Invoke();
		}
	}
}
namespace DevExpress.Xpf.Core.Native {
	class DummyDropTargetElement : FrameworkElement {
		static DummyDropTargetElement instance;
		public static DummyDropTargetElement Instance {
			get {
				if(instance == null)
					instance = new DummyDropTargetElement();
				return instance;
			}
		}
	}
	public class DragDropElementHelper {
#if DEBUGTEST
		public static EventLog eventLog = new EventLog();
#endif
		public static IDropTarget CreateDropTarget(UIElement dropTargetElement) {
			return dropTargetElement != null
				? CreateNotEmptyDropTarget(dropTargetElement, null)
				: EmptyDropTarget.Instance;
		}
		static IDropTarget CreateNotEmptyDropTarget(UIElement dropTargetElement, UIElement sourceElement) {
			IDropTargetFactory factory = DragManager.GetDropTargetFactory(dropTargetElement);
			if(sourceElement != null && factory is IDropTargetFactoryEx)
				return ((IDropTargetFactoryEx)factory).CreateDropTarget(dropTargetElement, sourceElement);
			return factory.CreateDropTarget(dropTargetElement);
		}
		BaseLocationStrategy locationStrategy;
		protected bool IsMouseDown;
		bool isDragging;
		bool isRelative;
		Point mouseDownPosition;
		Point mouseDownPositionCorrection;
		Point mousePositionToDragElement;
		readonly BaseDragDropStrategy strategy;
		UIElement keyboardEventSource;
		public DragDropElementHelper(ISupportDragDrop supportDragDrop, bool isRelativeMode = true) {
			SupportDragDrop = supportDragDrop;
			this.strategy = CreateDragDropStrategy();
			SourceElement = this.strategy.GetSourceElement();
			if(SourceElement == null) return;
			CurrentDropTarget = EmptyDropTarget.Instance;
			SubscribeEvents();
			CurrentDropTargetElement = DummyDropTargetElement.Instance;
			this.isRelative = isRelativeMode;
		}
		public IDropTarget CurrentDropTarget { get; set; }
		protected UIElement CurrentDropTargetElement { get; set; }
		protected UIElement CurrentRelativeElement { get; set; }
		public bool IsDragging { get { return isDragging; } }
		public IDragElement DragElement { get; set; }
		protected ISupportDragDrop SupportDragDrop { get; set; }
		public FrameworkElement SourceElement { get; set; }
		protected DispatcherTimer Timer { get; set; }
		protected ModifierKeys ModifierKeys { get; set; }
		protected Point DragElementLocation { get { return lastPos; } }
		bool IsTouchEnabled { get; set; }
		protected internal FrameworkElement TopVisual { get { return this.topVisual; } }
		protected Point MouseDownPositionCorrection { get { return this.mouseDownPositionCorrection; } }
		protected virtual BaseDragDropStrategy CreateDragDropStrategy() {
			return new BaseDragDropStrategy(SupportDragDrop, this);
		}
		protected virtual void SubscribeEvents() {
#if DEBUGTEST
			eventLog.AddEvent("Subscribe");
#endif
			InputEventsHelper.AddMouseLeftButtonDownHandler(SourceElement, new IndependentMouseButtonEventHandler(OnMouseLeftButtonDownIndependent));
			InputEventsHelper.AddMouseLeftButtonUpHandler(SourceElement, new IndependentMouseButtonEventHandler(OnMouseLeftButtonUpIndependent));
			InputEventsHelper.AddMouseMoveHandler(SourceElement, new IndependentMouseEventHandler(OnMouseMoveIndependent));
			SourceElement.LostMouseCapture += new MouseEventHandler(OnLostMouseCapture);
#if SILVERLIGHT
			this.strategy.SubscribedElement.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown), true);
			SourceElement.MouseMove += new MouseEventHandler(OnPreviewMouseMove);
			SourceElement.AddHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnPreviewMouseLeftButtonUp), true);
#else
			this.strategy.SubscribedElement.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown);
			SourceElement.PreviewMouseMove += new MouseEventHandler(OnPreviewMouseMove);
			SourceElement.PreviewMouseRightButtonUp += new MouseButtonEventHandler(OnPreviewMouseRightButtonUp);
			SourceElement.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OnPreviewMouseLeftButtonUp);
#endif
			Timer = new DispatcherTimer();
			Timer.Interval = TimeSpan.FromMilliseconds(153);
			Timer.Tick += new EventHandler(OnTimer);
			ModifierKeys = Keyboard.Modifiers;
		}
		protected virtual void OnLostMouseCapture(object sender, MouseEventArgs e) {
			if(e.OriginalSource == SourceElement && e.LeftButton == MouseButtonState.Pressed)
				CancelDragging();
			if(!IsDragging && Mouse.Captured == null)
				IsMouseDown = false;
		}
		public virtual void Destroy() {
			UnsubscribeEvents();
		}
		protected virtual void UnsubscribeEvents() {
			if(SourceElement == null)
				return;
#if DEBUGTEST
			eventLog.AddEvent("Unsubscribe");
#endif
			InputEventsHelper.RemoveMouseLeftButtonDownHandler(SourceElement, new IndependentMouseButtonEventHandler(OnMouseLeftButtonDownIndependent));
			InputEventsHelper.RemoveMouseLeftButtonUpHandler(SourceElement, new IndependentMouseButtonEventHandler(OnMouseLeftButtonUpIndependent));
			InputEventsHelper.RemoveMouseMoveHandler(SourceElement, new IndependentMouseEventHandler(OnMouseMoveIndependent));
			SourceElement.LostMouseCapture -= new MouseEventHandler(OnLostMouseCapture);
#if SILVERLIGHT
			if(this.strategy.SubscribedElement != null)
				this.strategy.SubscribedElement.RemoveHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown));
			SourceElement.MouseMove -= new MouseEventHandler(OnPreviewMouseMove);
			SourceElement.RemoveHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnPreviewMouseLeftButtonUp));
#else
			if(this.strategy.SubscribedElement != null)
				this.strategy.SubscribedElement.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown);
			SourceElement.PreviewMouseMove -= new MouseEventHandler(OnPreviewMouseMove);
			SourceElement.PreviewMouseRightButtonUp -= new MouseButtonEventHandler(OnPreviewMouseRightButtonUp);
			SourceElement.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(OnPreviewMouseLeftButtonUp);
#endif
		}
		protected virtual void OnTimer(object sender, EventArgs e) {
			if(Keyboard.Modifiers != ModifierKeys) {
				ModifierKeys = Keyboard.Modifiers;
				if(CurrentDropTarget != null)
					OnDragOver();
			}
		}
		internal FrameworkElement topVisual;
		IndependentMouseEventArgs startDragEventArgs;
		protected virtual internal void StartDragging(Point offset, IndependentMouseEventArgs e) {
			this.IsMouseDown = false;
			this.isDragging = true;
			topVisual = this.strategy.GetTopVisual(SupportDragDrop.SourceElement);
			if(topVisual != null) {
				DragManager.SetIsDragging(topVisual, true);
				this.locationStrategy = this.strategy.CreateLocationStrategy();
			}
#if !SILVERLIGHT
			if(Application.Current != null)
				Application.Current.Deactivated += new EventHandler(OnApplicationDeactivated);
#endif
			keyboardEventSource = LayoutHelper.GetTopLevelVisual((DependencyObject)SupportDragDrop.SourceElement) as UIElement;
			if(keyboardEventSource is Popup)
				keyboardEventSource = ((Popup)keyboardEventSource).Child;
			if(keyboardEventSource != null) {
#if !SILVERLIGHT
				keyboardEventSource.AddHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown), true);
#else
				keyboardEventSource.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(OnPreviewKeyDown), true);
#endif
			}
			startDragEventArgs = e;			
			DragElement = SupportDragDrop.CreateDragElement(offset);
			e.CaptureMouse(SourceElement); 
			sourceElementLocation = this.locationStrategy.GetPosition(SourceElement, topVisual);
			Timer.Start();
		}
		void OnApplicationDeactivated(object sender, EventArgs e) {
			CancelDragging();
		}
		protected virtual void EndDragging(IndependentMouseButtonEventArgs e) {
			Timer.Stop();
			IDropTarget dropTarget = CurrentDropTarget;
			CancelDragging();
			if(dropTarget != null) {
				Drop(dropTarget, mouseLocationRelative);
			}
		}
		protected void CancelDragging() {
			Timer.Stop();
#if !SILVERLIGHT
			if(Application.Current != null)
				Application.Current.Deactivated -= new EventHandler(OnApplicationDeactivated);
#endif
			if(keyboardEventSource != null) {
#if !SILVERLIGHT
				keyboardEventSource.RemoveHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown));
#else
				keyboardEventSource.RemoveHandler(UIElement.KeyDownEvent, new KeyEventHandler(OnPreviewKeyDown));
#endif
				keyboardEventSource = null;
			}
			if(startDragEventArgs != null) {
				startDragEventArgs.ReleaseMouseCapture(SourceElement);
				startDragEventArgs = null;
			}
			if(topVisual != null) {
				topVisual.ClearValue(DragManager.IsDraggingProperty);
				topVisual = null;
			}
			this.isDragging = false;
			this.IsMouseDown = false;
			RemoveDragElement();
			UpdateCurrentDropTarget(null);
		}
		public void OnPreviewKeyDown(object sender, KeyEventArgs e) {
			if(e.Key == Key.Escape)
				CancelDragging();
		}
		public void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			OnMouseLeftButtonDownIndependent(sender, new PlatformMouseButtonEventArgs(e));
		}
		Point startSourceElementLocation;
		public void OnMouseLeftButtonDownIndependent(object sender, IndependentMouseButtonEventArgs e) {
			if(e.OriginalSource is DependencyObject && !DragManager.GetIsStartDragPlace((DependencyObject)e.OriginalSource))
				return;
			if(SupportDragDrop != null && !GetCanStartDrag(sender, e))
				return;
			this.IsMouseDown = true;
			startSourceElementLocation = SourceElement.GetPosition(null);
			this.mouseDownPosition = e.GetPosition(SourceElement);
#if !SL
			ChangeTouchEnabled(LayoutHelper.GetTopLevelVisual(sender as DependencyObject).Parent is Popup);
			this.startDragPoint = PointFromScreen(SourceElement, UIElementExtensions.MapPoint(SourceElement, this.mouseDownPosition, null));
#else
			this.startDragPoint = UIElementExtensions.MapPoint(SourceElement, this.mouseDownPosition, AppHelper.RootVisual);
#endif
			mousePositionToDragElement = GetPosition(e, this.strategy.GetDragElement());
#if !SL
			if(SourceElement.FlowDirection == FlowDirection.RightToLeft)
				this.mouseDownPosition.X = SourceElement.ActualWidth - this.mouseDownPosition.X;			
#endif
			this.startDragPoint.X -= mousePositionToDragElement.X;
			this.startDragPoint.Y -= mousePositionToDragElement.Y;			
		}
#if !SL
		void ChangeTouchEnabled(bool isPopupContainer) {
			IsTouchEnabled = isPopupContainer && SystemParameters.MenuDropAlignment;			
		}
#endif
		private bool GetCanStartDrag(object sender, IndependentMouseButtonEventArgs e) {
			if(SupportDragDrop is ISupportDragDropPlatformIndependent)
				return ((ISupportDragDropPlatformIndependent)SupportDragDrop).CanStartDrag(sender, e);
			if(e is PlatformMouseButtonEventArgs)
				return SupportDragDrop.CanStartDrag(sender, ((PlatformMouseButtonEventArgs)e).OrigninalEventArgs);
			throw new NotImplementedException();
		}
		bool isProcessingOnPreviewMouseMove = false;
		public void OnPreviewMouseMove(object sender, MouseEventArgs e) {			
			OnMouseMoveIndependent(sender, new PlatformMouseEventArgs(e));
		}
		Point mouseLocationRelative;
		public void OnMouseMoveIndependent(object sender, IndependentMouseEventArgs e) {
			OnMouseMoveIndependentCore(sender, e, false);
		}
#if SL
		protected bool IsReleaseBeforeStartDragging = false;
#endif
		void OnMouseMoveIndependentCore(object sender, IndependentMouseEventArgs e, bool isLeave = false) {
			if(isProcessingOnPreviewMouseMove) return;
			else isProcessingOnPreviewMouseMove = true;
			try {
#if !SILVERLIGHT
				if(e.LeftButton == MouseButtonState.Released) {
					return;
				}
#endif
				if(this.IsMouseDown && IsElementVisible(SourceElement) && (IsDragGesture(GetPosition(e, SourceElement)) || isLeave)) {
#if SL
					SupportDragDrop.SourceElement.ReleaseMouseCapture();
					IsReleaseBeforeStartDragging = SupportDragDrop.SourceElement != SourceElement;
#endif
					StartDragging(GetStartDraggingOffset(e, SupportDragDrop.SourceElement), e);
				}
				if(IsDragging) {
					DependencyObject hitTestFactoryElement = GetHitTestFactoryElement(GetPositionProvider(e));
					UpdateCurrentDropTarget(hitTestFactoryElement as UIElement);
					Point mouseLocation = this.locationStrategy.GetMousePosition(e, topVisual);
					mouseLocationRelative = (CurrentRelativeElement != null) ? e.GetPosition(CurrentRelativeElement) : e.GetPosition(topVisual);
					lastPos = new Point(mouseLocation.X - sourceElementLocation.X, mouseLocation.Y - sourceElementLocation.Y);
					OnDragOver();
					UpdateDragElementLocation(lastPos);
					this.strategy.UpdateLocation(e);
				}
			} finally { isProcessingOnPreviewMouseMove = false; }
		}
		protected virtual Point GetStartDraggingOffset(IndependentMouseEventArgs e, FrameworkElement sourceElement) {
			return mousePositionToDragElement;
		}
		protected Point GetPosition(IndependentMouseEventArgs e, FrameworkElement relativeTo) {
			Point mousePosition = e.GetPosition(relativeTo);
#if !SL
			if(relativeTo != null && relativeTo.FlowDirection == FlowDirection.RightToLeft)
				mousePosition.X = relativeTo.ActualWidth - mousePosition.X;
#endif
			return mousePosition;
		}
		Point PointFromScreen(Visual visual, Point point) {
#if !SL
			if(!BrowserInteropHelper.IsBrowserHosted) {
				PresentationSource source = PresentationSource.FromVisual(visual);
				return source != null ? source.CompositionTarget.TransformFromDevice.Transform(point) : point;
			}
			else
#endif
				return point;
		}
		Point PointToScreen(Visual visual, Point point) {
#if !SL
			if(!BrowserInteropHelper.IsBrowserHosted && !IsTouchEnabled) {
				PresentationSource source = PresentationSource.FromVisual(visual);
				return source != null ? source.CompositionTarget.TransformToDevice.Transform(point) : point;
			}
			else
#endif
				return point;
		}
		protected virtual Func<UIElement, Point> GetPositionProvider(IndependentMouseEventArgs e) {
			return element => e.GetPosition(element);
		}
		protected virtual UIElement GetTopLevelDropContainer(UIElement relativeTo) {
			return relativeTo;
		}
		protected virtual bool IsElementVisible(UIElement elem) {
			return UIElementHelper.IsVisibleInTree(elem);
		}
		bool IsSkipHitTestVisibleCheck() {
			return SupportDragDrop is ISupportDragDropColumnHeader ? ((ISupportDragDropColumnHeader)SupportDragDrop).SkipHitTestVisibleChecking : false;
		}
		[SecuritySafeCritical]
		public DependencyObject GetHitTestFactoryElement(Func<UIElement, Point> positionProvider) {
			DependencyObject hitTestFactoryElement = null;
			bool skipHitTestVisibleCheck = IsSkipHitTestVisibleCheck();
			HitTestResultCallback callback = delegate(HitTestResult result) {
				if(result.VisualHit is UIElement && !((UIElement)result.VisualHit).IsVisible)
					return HitTestResultBehavior.Continue;
				hitTestFactoryElement = GetDropTargetByHitElement(result.VisualHit);
				if(hitTestFactoryElement != null)
					return HitTestResultBehavior.Stop;
				return HitTestResultBehavior.Continue;
			};
			HitTestFilterCallback filterCallback = delegate(DependencyObject element) {
				UIElement el = element as UIElement;
				if(el != null && !el.IsHitTestVisible && !skipHitTestVisibleCheck)
					return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
				return HitTestFilterBehavior.Continue;
			};
#if !SL
			List<IntPtr> windowHandlesToIgnore = new List<IntPtr>();
			CollectIgnoredWindowsPointers(windowHandlesToIgnore);
#endif
			foreach(UIElement topLevelDropContainer in SupportDragDrop.GetTopLevelDropContainers()) {
				if(topLevelDropContainer == null 
#if !SL
					|| topLevelDropContainer.Dispatcher != Dispatcher.CurrentDispatcher
#endif
					) 
					continue;
#if SILVERLIGHT
				Point point = positionProvider(Application.Current.RootVisual);
				if(((FrameworkElement)Application.Current.RootVisual).FlowDirection == FlowDirection.RightToLeft)
					point.X = AppHelper.HostWidth - point.X;
				HitTestHelper.HitTest(GetTopLevelDropContainer(topLevelDropContainer), filterCallback, callback, new PointHitTestParameters(point), true);
#else
				Point point = positionProvider(topLevelDropContainer);
				if(topLevelDropContainer is Window) {
					if(PresentationSource.FromVisual(topLevelDropContainer) == null)
						continue;
					Point screenPoint = topLevelDropContainer.PointToScreen(point);
					IntPtr handle = NativeMethods.WindowFromPoint(new NativeMethods.POINT() { x = (int)screenPoint.X, y = (int)screenPoint.Y });
					if(!windowHandlesToIgnore.Contains(handle) && handle != new WindowInteropHelper((Window)topLevelDropContainer).Handle)
						continue;
				}
				VisualTreeHelper.HitTest(GetTopLevelDropContainer(topLevelDropContainer), filterCallback,
					callback, new PointHitTestParameters(point));
#endif
				if(hitTestFactoryElement != null) {
					break;
				}
			}
				return hitTestFactoryElement;
		}
#if !SL
		void CollectIgnoredWindowsPointers(List<IntPtr> pointers) {
			if(Application.Current == null) return;
			if(Application.Current.CheckAccess()) {
				foreach(Window item in Application.Current.Windows) {
					if(!item.IsHitTestVisible)
						pointers.Add(new WindowInteropHelper(item).Handle);
				}
			}
			else {
				Application.Current.Dispatcher.Invoke(new Action(() => {
					foreach(Window item in Application.Current.Windows) {
						if(!item.IsHitTestVisible)
							pointers.Add(new WindowInteropHelper(item).Handle);
					}
				}));
			}
		}
#endif
		public void OnDragOver(IDropTarget dropTarget, Point position) {
			dropTarget.OnDragOver(SupportDragDrop.SourceElement, position);
		}
		public void Drop(IDropTarget dropTarget, Point position) {
			dropTarget.Drop(SupportDragDrop.SourceElement, position);
		}
		public void OnDragLeave(IDropTarget dropTarget) {
			dropTarget.OnDragLeave();
		}
		void OnDragOver() {
			OnDragOver(CurrentDropTarget, mouseLocationRelative);
		}
		void UpdateCurrentDropTarget(UIElement dropTargetElement) {
			if(CurrentDropTargetElement != dropTargetElement) {
				OnDragLeave(CurrentDropTarget);
				CurrentDropTarget = GetDropTarget(dropTargetElement);
				CurrentDropTargetElement = dropTargetElement;
				CurrentRelativeElement = GetRelativeElement(dropTargetElement);
			}
		}
		IDropTarget GetDropTarget(UIElement dropTargetElement) {
			IDropTarget dropTarget = null;
			if(dropTargetElement != null || SupportDragDrop == null) {
				dropTarget = CreateDropTargetCore(dropTargetElement);
			}
			if(dropTarget == null) {
				dropTarget = SupportDragDrop.CreateEmptyDropTarget();
				if(dropTarget == null)
					dropTarget = EmptyDropTarget.Instance;
			}
			return dropTarget;
		}
		UIElement GetRelativeElement(UIElement dropTargetElement) {
			if(dropTargetElement == null)
				return null;
			IDropTargetFactory factory = DragManager.GetDropTargetFactory(dropTargetElement);
			if(SupportDragDrop != null && !SupportDragDrop.IsCompatibleDropTargetFactory(factory, dropTargetElement))
				return null;
			return dropTargetElement;
		}
		IDropTarget CreateDropTargetCore(UIElement dropTargetElement) {
			if(dropTargetElement != null) {
				IDropTargetFactory factory = DragManager.GetDropTargetFactory(dropTargetElement);
				if(SupportDragDrop != null && !SupportDragDrop.IsCompatibleDropTargetFactory(factory, dropTargetElement))
					return null;
				return CreateNotEmptyDropTarget(dropTargetElement, SupportDragDrop.SourceElement);
			}
			return EmptyDropTarget.Instance;
		}
		public UIElement GetDropTargetByHitElement(DependencyObject element) {
			DependencyObject current = element;
			while(current != null) {
				if(current is UIElement) {
					if(DragManager.GetDropTargetFactory(current) != null)
						return (UIElement)current;
				}
				current = VisualTreeHelper.GetParent(current);
			}
			return null;
		}
		public void OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e) {
			if(IsDragging)
				e.Handled = true;
		}
		public void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			OnMouseLeftButtonUpIndependent(sender, new PlatformMouseButtonEventArgs(e));
		}
		public void OnMouseLeftButtonUpIndependent(object sender, IndependentMouseButtonEventArgs e) {
			this.IsMouseDown = false;
			if(IsDragging)
				EndDragging(e);
		}
		protected virtual void RemoveDragElement() {
			if(DragElement == null)
				return;
			DragElement.Destroy();
			DragElement = null;
		}
		Point lastPos = new Point(0, 0);
		Point sourceElementLocation;
		Point startDragPoint;
		protected virtual Point CorrectDragElementLocation(Point newPos) {
			if(isRelative) {
				newPos.X -= this.mouseDownPosition.X;
				newPos.Y -= this.mouseDownPosition.Y;
			}
#if !SL
			if(!BrowserInteropHelper.IsBrowserHosted) {
				newPos.X += this.startDragPoint.X;
				newPos.Y += this.startDragPoint.Y;
			}
#endif
			return newPos;
		}
		protected virtual void UpdateDragElementLocation(Point newPos) {
			if(DragElement == null) return;
			newPos = CorrectDragElementLocation(newPos);
			DragElement.UpdateLocation(PointToScreen(SourceElement, newPos));
		}
		bool IsDragGesture(Point point) {
			mouseDownPositionCorrection = new Point(point.X - this.mouseDownPosition.X, point.Y - this.mouseDownPosition.Y);
			double minHorzDistance = SystemParameters.MinimumHorizontalDragDistance;
			double minVertDistance = SystemParameters.MinimumVerticalDragDistance;
			Point newSourceElementLocation = SourceElement.GetPosition(null);
			bool allowDrag = newSourceElementLocation.X - startSourceElementLocation.X == 0 &&
				newSourceElementLocation.Y - startSourceElementLocation.Y == 0;
			if(!allowDrag)
				IsMouseDown = false;
			return Math.Abs(mouseDownPositionCorrection.X) > minHorzDistance ||
				Math.Abs(mouseDownPositionCorrection.Y) > minVertDistance && allowDrag;
		}				
	}
	public class BaseDragDropStrategy {
		readonly DragDropElementHelper helper;
		readonly ISupportDragDrop supportDragDrop;
		protected DragDropElementHelper Helper { get { return this.helper; } }
		protected ISupportDragDrop SupportDragDrop { get { return this.supportDragDrop; } }
		protected FrameworkElement TopVisual { get { return Helper.TopVisual; } }
		public virtual FrameworkElement SubscribedElement { get { return Helper.SourceElement; } }
		public BaseDragDropStrategy(ISupportDragDrop supportDragDrop, DragDropElementHelper helper) {
			this.supportDragDrop = supportDragDrop;
			this.helper = helper;
		}
		public virtual FrameworkElement GetSourceElement() {
			return SupportDragDrop.SourceElement;
		}
		public virtual FrameworkElement GetTopVisual(FrameworkElement node) {
#if !SL
			return LayoutHelper.GetTopLevelVisual(node);
#else
			return (FrameworkElement)Application.Current.RootVisual;
#endif
		}
		public virtual void UpdateLocation(IndependentMouseEventArgs e) { }
		public virtual FrameworkElement GetDragElement() {
			return SupportDragDrop.SourceElement;
		}
		public virtual BaseLocationStrategy CreateLocationStrategy() {
#if !SL
			if(Helper.TopVisual.Parent is Popup && Helper.SourceElement.FlowDirection == FlowDirection.RightToLeft)
				return new RTL_LocationStrategy();
#endif
			return new BaseLocationStrategy();
		}
	}
	public class DragDropElementHelperBounded : DragDropElementHelper {
		public DragDropElementHelperBounded(ISupportDragDrop supportDragDrop, bool isRelativeMode = true) : base(supportDragDrop, isRelativeMode) { }
		public DragDropElementHelperBounded(ISupportDragDropColumnHeader supportDragDrop, bool isRelativeMode = true) : base(supportDragDrop, isRelativeMode) { }
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			SourceElement.MouseLeave += OnMouseLeave;
		}
		protected override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			SourceElement.MouseLeave -= OnMouseLeave;
		}
		void OnMouseLeave(object sender, MouseEventArgs e) {
			if(!IsDragging)
				CancelDragging();
		}
	}
	public class BaseLocationStrategy {
		public virtual Point GetPosition(FrameworkElement element, FrameworkElement relativeTo) {
			return element.GetPosition(relativeTo);
		}
		public virtual Point GetMousePosition(IndependentMouseEventArgs e, UIElement relativeTo) {
			return e.GetPosition(relativeTo);
		}
	}
	public class RTL_MouseLocationStrategy : BaseLocationStrategy {
		public override Point GetMousePosition(IndependentMouseEventArgs e, UIElement relativeTo) {
			Point pos = e.GetPosition(relativeTo);
			return new Point(relativeTo.RenderSize.Width - pos.X, pos.Y);
		}
	}
	public class RTL_LocationStrategy : RTL_MouseLocationStrategy {
		public override Point GetPosition(FrameworkElement element, FrameworkElement relativeTo) {
			Point pos = element.GetPosition(relativeTo);
			return new Point(relativeTo.ActualWidth - element.ActualWidth - pos.X, pos.Y);
		}
	}
}
