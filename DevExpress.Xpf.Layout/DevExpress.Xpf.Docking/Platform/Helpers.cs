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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Layout.Core.Platform;
using SW = System.Windows;
#if SILVERLIGHT
using SWC = DevExpress.Xpf.Layout.Core;
#else
using SWC = System.Windows.Controls;
#endif
namespace DevExpress.Xpf.Docking.Platform {
	public static class KeyHelper {
		public static bool IsCtrlPressed {
#if !SILVERLIGHT
			get { return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl); }
#else
			get { return (Keyboard.Modifiers & ModifierKeys.Control) != 0; }
#endif
		}
		public static bool IsShiftPressed {
#if !SILVERLIGHT
			get { return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift); }
#else
			get { return (Keyboard.Modifiers & ModifierKeys.Shift) != 0; }
#endif
		}
		public static IInputElement FocusedElement {
#if !SILVERLIGHT
			get { return Keyboard.FocusedElement; }
#else
			get { return null; }
#endif
		}
	}
	public static class WindowHelper {
#if !SILVERLIGHT
		public static bool IsAeroMode(DependencyObject dObj){
			DXWindow dxWindow = Window.GetWindow(dObj) as DXWindow;
			return dxWindow!= null && dxWindow.IsAeroMode;
		}
		internal static void DragSize(this Window window, SizingAction sizingAction) {
			var hwndSource = HwndSource.FromDependencyObject(window) as HwndSource;
			if(hwndSource != null) {
				var handle = hwndSource.Handle;
				Win32.NativeHelper.SendMessageSafe(handle, (int)Win32.WM.WM_SYSCOMMAND, (IntPtr)(Win32.SC.SC_SIZE + sizingAction), IntPtr.Zero);
				Win32.NativeHelper.SendMessageSafe(handle, 514, IntPtr.Zero, IntPtr.Zero);
			}
		}
		internal static Rect GetScreenBounds(this Window window) {
			return new Rect(window.Left, window.Top, window.Width, window.Height);
		}
#endif
		public static bool IsXBAP {
#if !SILVERLIGHT
			get { return BrowserInteropHelper.IsBrowserHosted; }
#else
			get { return true; }
#endif
		}
		public static Rect CheckScreenBounds(DockLayoutManager manager, Rect screenBounds) {
			if(manager == null)
				return screenBounds;
			Rect workingArea = GetWorkingArea(manager, screenBounds);
			Rect intersectBounds = screenBounds;
			if(manager.FlowDirection == FlowDirection.RightToLeft) {
				intersectBounds = new Rect(-intersectBounds.X, intersectBounds.Y, intersectBounds.Width, intersectBounds.Height);
			}
			if(!IsZero(workingArea) && !IntersectsWith(workingArea, intersectBounds)) {
				workingArea = GetWorkingArea(manager, new Rect(0, 0, 1, 1));
				screenBounds = new Rect(workingArea.Left + workingArea.Width - screenBounds.Width,
					Math.Min(Math.Max(screenBounds.Top, workingArea.Top), workingArea.Bottom - screenBounds.Height),
					screenBounds.Width, screenBounds.Height);
			}
			return screenBounds;
		}
		static bool IsZero(Rect r) {
			return r == new Rect(0, 0, 0, 0);
		}
		static bool IntersectsWith(Rect r, Rect rect) {
			if(r.IsEmpty || rect.IsEmpty) return false;
			return ((((rect.Left <= r.Right) && (rect.Right >= r.Left)) && (rect.Top <= r.Bottom)) && (rect.Bottom >= r.Top));
		}
		public static Rect GetMaximizeBounds(DockLayoutManager manager, Rect restoreBounds) {
			if(manager == null) return restoreBounds;
#if SILVERLIGHT
			return GetWorkingArea(manager, restoreBounds);
#else
			return GetWorkingArea(manager, restoreBounds, true);
#endif
		}
#if !SILVERLIGHT
		static Rect GetWorkingArea(DockLayoutManager manager, Rect screenBounds, bool checkAeroMode) {
			return checkAeroMode && manager.GetRealFloatingMode() == Core.FloatingMode.Adorner ?
				GetAdornerWorkingArea(manager, checkAeroMode) :
				GetWorkingArea(manager, screenBounds);
		}
		static Rect GetAdornerWorkingArea(DockLayoutManager manager, bool checkAeroMode) {
			if(!checkAeroMode) return GetAdornerWorkingArea(manager);
			UIElement adornerSource = LayoutHelper.GetTopContainerWithAdornerLayer(manager);
			if(adornerSource != null) {
				Point adornerOrigin = adornerSource.TranslatePoint(CoordinateHelper.ZeroPoint, manager);
				Rect adornerRect = new Rect(adornerOrigin, adornerSource.RenderSize);
				DXWindow window = Window.GetWindow(manager) as DXWindow;
				if(window != null && window.IsAeroMode) {
					var clientRect = window.ClientArea;
					adornerOrigin = adornerSource.TranslatePoint(clientRect.Location, manager);
					adornerRect = new Rect(adornerOrigin, clientRect.Size);
				}
				MatrixTransform transform = adornerSource.TransformToVisual(manager) as MatrixTransform;
				if(transform != null && !transform.Matrix.IsIdentity) {
					Matrix matrix = transform.Matrix;
					transform = new MatrixTransform(Math.Abs(matrix.M11), matrix.M12, matrix.M21, Math.Abs(matrix.M22), 0, 0);
					if(!transform.Matrix.IsIdentity) {
						adornerRect = transform.TransformBounds(adornerRect);
						adornerRect.Location = adornerOrigin;
					}
				}
				if(FrameworkElement.GetFlowDirection(adornerSource) != manager.FlowDirection)
					adornerRect.X -= adornerRect.Width;
				return adornerRect;
			}
			return new Rect(CoordinateHelper.ZeroPoint, manager.RenderSize);
		}
#endif
		static Rect GetWorkingArea(DockLayoutManager manager, Rect screenBounds) {
			return manager.GetRealFloatingMode() == Core.FloatingMode.Adorner ?
				GetAdornerWorkingArea(manager) :
				GetScrenWorkingArea(manager, screenBounds);
		}
		static Rect GetAdornerWorkingArea(DockLayoutManager manager) {
#if !SILVERLIGHT
			UIElement adornerSource = LayoutHelper.GetTopContainerWithAdornerLayer(manager);
#else
			UIElement adornerSource = manager;
#endif
			if(adornerSource != null) {
				Point origin = adornerSource.TranslatePoint(CoordinateHelper.ZeroPoint, manager);
#if !SILVERLIGHT
				if(FrameworkElement.GetFlowDirection(adornerSource) != manager.FlowDirection)
					origin.X -= adornerSource.RenderSize.Width;
#endif
				return new Rect(origin, adornerSource.RenderSize);
			}
			return new Rect(CoordinateHelper.ZeroPoint, manager.RenderSize);
		}
#if !SILVERLIGHT
		static Rect GetScrenWorkingArea(DockLayoutManager manager, Rect restoreBounds) {
			Point offset = manager.PointToScreen(CoordinateHelper.ZeroPoint);
			Rect screen = GetWorkingArea(Rect.Offset(restoreBounds, offset.X, offset.Y));
			Point screenPoint = manager.PointFromScreen(screen.Location);
			if(manager.FlowDirection == FlowDirection.RightToLeft)
				screenPoint = new Point(-screenPoint.X, screenPoint.Y);
			return new Rect(
					screenPoint,
					(Size)GetDisplayMathix(manager).Transform((Vector)screen.Size)
				);
		}
		[System.Security.SecuritySafeCritical]
		static Rect GetWorkingArea(Rect box) {
			NativeMethods.RECT rc = new NativeMethods.RECT(0, 0, 0, 0);
			NativeMethods.RECT temp = new NativeMethods.RECT((int)box.Left, (int)box.Top, (int)box.Right, (int)box.Bottom);
			IntPtr handle = NativeMethods.MonitorFromRect(ref temp, 2);
			if(handle != IntPtr.Zero) {
				NativeMethods.MONITORINFOEX info = new NativeMethods.MONITORINFOEX();
				NativeMethods.GetMonitorInfo(new System.Runtime.InteropServices.HandleRef(null, handle), info);
				rc = info.rcWork;
			}
			return new Rect(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
		}
		public static Point GetScreenLocation(UIElement element) {
			Point screenLocation = PointToScreen(element, CoordinateHelper.ZeroPoint);
			if(!IsXBAP)
				screenLocation = GetDisplayMathix(element).Transform(screenLocation);
			return screenLocation;
		}
		static Point PointToScreen(UIElement element, Point p) {
			if(!IsXBAP)
				return element.PointToScreen(p);
			else
				return element.MapPoint(p, element.GetRootVisual());
		}
		static SW.Media.Matrix GetDisplayMathix(UIElement element) {
			PresentationSource pSource = PresentationSource.FromVisual(element);
			if(pSource != null)
				return pSource.CompositionTarget.TransformFromDevice;
			return SW.Media.Matrix.Identity;
		}
#else
		static Rect GetScrenWorkingArea(DockLayoutManager manager, Rect restoreBounds) {
			return restoreBounds;
		}
		public static Point GetScreenLocation(UIElement element) {
			UIElement root = LayoutHelper.FindRoot(element) as UIElement;
			return element.TranslatePoint(CoordinateHelper.ZeroPoint, root);
		}
		internal static Point GetElementOffset(FrameworkElement element) {
			UIElement root = LayoutHelper.FindRoot(element) as UIElement;
			return (root != null) ? root.MapPoint(new Point(element.GetLeft(), element.GetTop()), element) : new Point();
		}
#endif
		internal const int MagicPosition = -32000;
		public static void BringToFront(DependencyObject window) {
#if !SILVERLIGHT
			var source = HwndSource.FromDependencyObject(window) as HwndSource;
			if(source != null) SetForegroundWindow(source);
#endif
		}
		public static void BindFlowDirection(FrameworkElement window, object source) {
			window.SetBinding(FrameworkElement.FlowDirectionProperty,
					new System.Windows.Data.Binding()
					{
						Path = new PropertyPath(FrameworkElement.FlowDirectionProperty),
						Source = source
					}
				);
		}
		public static void BindFlowDirectionIfNeeded(FrameworkElement window, object source) {
			if(!BindingHelper.IsDataBound(window, FrameworkElement.FlowDirectionProperty))
				BindFlowDirection(window, source);
		}
#if !SILVERLIGHT
		static void SetForegroundWindow(System.Windows.Interop.HwndSource hWndSource) {
			if(hWndSource != null)
				Win32.NativeHelper.SetForegroundWindowSafe(new System.Runtime.InteropServices.HandleRef(null, hWndSource.Handle));
		}
#endif
	}
	public static class CoordinateHelper {
		public static readonly Point ZeroPoint = new Point(0, 0);
		public static Point PointToScreen(DockLayoutManager container, UIElement element, Point elementPoint) {
			return element.TranslatePoint(elementPoint, container);
		}
		public static Point PointFromScreen(DockLayoutManager container, UIElement element, Point screenPoint) {
			return container.TranslatePoint(screenPoint, element);
		}
#if SILVERLIGHT
		public static Point PointToScreen(UIElement element, Point elementPoint) {
			return element.TranslatePoint(elementPoint, Application.Current.RootVisual);
		}
		public static Point PointFromScreen(UIElement element, Point screenPoint) {
			return element.MapPointFromScreen(screenPoint);
		}
#endif
		public static Rect GetAvailableAdornerRect(DockLayoutManager container, Rect bounds) {
#if !SILVERLIGHT
			UIElement adornerSource = LayoutHelper.GetTopContainerWithAdornerLayer(container);
#else
			UIElement adornerSource = container;
#endif
			if(adornerSource == null) return bounds;
			Size adornerSize = adornerSource.RenderSize;
			Point adornerOrigin = adornerSource.TranslatePoint(ZeroPoint, container);
			Rect adornerRect = new Rect(adornerOrigin, adornerSize);
#if !SILVERLIGHT
			DXWindow window = Window.GetWindow(container) as DXWindow;
			if(window != null && window.IsAeroMode) {
				var clientRect = window.ClientArea;
				adornerOrigin = adornerSource.TranslatePoint(clientRect.Location, container);
				adornerSize = clientRect.Size;
				adornerRect = new Rect(adornerOrigin, adornerSize);
			}
			MatrixTransform transform = adornerSource.TransformToVisual(container) as MatrixTransform;
			if(transform != null && !transform.Matrix.IsIdentity) {
				Matrix matrix = transform.Matrix;
				transform = new MatrixTransform(Math.Abs(matrix.M11), matrix.M12, matrix.M21, Math.Abs(matrix.M22), 0, 0);
				if(!transform.Matrix.IsIdentity) {
					adornerRect = transform.TransformBounds(adornerRect);
					adornerRect.Location = adornerOrigin;
				}
			}
#endif
			if(((FrameworkElement)adornerSource).FlowDirection != container.FlowDirection) {
				adornerRect.X -= adornerRect.Width;
			}
			return adornerRect;
		}
		public static Rect GetContainerRect(DockLayoutManager container) {
			return new Rect(new Point(0, 0), container.RenderSize);
		}
		public static Point GetCenter(Rect rect) {
			return new Point(rect.Left + rect.Width * 0.5, rect.Top + rect.Height * 0.5);
		}
	}
	public static class VisualOrderHelper {
		public static int GetVisualOrder(this UIElement element, DockLayoutManager container) {
			int counter = -1;
			using(var enumerator = new IUIElementEnumerator(container)) {
				while(enumerator.MoveNext()) {
					counter++;
					if(enumerator.Current == element) break;
				}
			}
			if(counter != -1) {
				if(element is AutoHideTray)
					counter += 100000;
				if(element is FloatPanePresenter)
					counter += 200000;
				if(element is CustomizationControl)
					counter += 300000;
			}
			return counter;
		}
		public static void BringToFront(this FloatPanePresenter pane, DockLayoutManager container) {
			if(pane == null || container == null) return;
			System.Windows.Controls.Panel floatingRoot = GetFloatingRoot(pane);
			if(floatingRoot != null) {
				UIElement topElement = GetFloatingRootChild(pane);
				UIElement[] elements = GetChildren(floatingRoot);
				UpdateVisualOrder(topElement, elements);
				((IUIElement)container).Children.MakeLast(pane);
			}
		}
		static void UpdateVisualOrder(UIElement topChild, UIElement[] children) {
			for(int i = 0; i < children.Length; i++) {
				if(topChild == children[i]) continue;
				psvPanel.SetZIndex(children[i], i);
			}
			psvPanel.SetZIndex(topChild, children.Length + 1);
		}
		static UIElement[] GetChildren(System.Windows.Controls.Panel floatingRoot) {
			UIElement[] elements = new UIElement[floatingRoot.Children.Count];
			floatingRoot.Children.CopyTo(elements, 0);
			return elements;
		}
		static System.Windows.Controls.Panel GetFloatingRoot(FloatPanePresenter element) {
			if(element == null) return null;
			UIElement root = element;
			while(root != null && !(root is GroupPanel)) {
				root = VisualTreeHelper.GetParent(root) as UIElement;
			}
			return root as System.Windows.Controls.Panel;
		}
		static UIElement GetFloatingRootChild(FloatPanePresenter element) {
			if(element == null) return null;
			UIElement root = element;
			UIElement child = null;
			while(root != null && !(root is GroupPanel)) {
				child = root;
				root = VisualTreeHelper.GetParent(root) as UIElement;
			}
			return child;
		}
	}
	public static class EventArgsHelper {
#if !SILVERLIGHT
		[System.Diagnostics.DebuggerStepThrough]
		public static Layout.Core.Platform.MouseEventArgs Convert(UIElement element, SW.Input.MouseButtonEventArgs e) {
			MouseButtons buttons = GetMouseButtons(e);
			MouseButtons changed = GetChangedButtons(e);
			return new Layout.Core.Platform.MouseEventArgs(e.GetPosition(element), buttons, changed);
		}
		[System.Diagnostics.DebuggerStepThrough]
		public static Layout.Core.Platform.MouseEventArgs Convert(UIElement element, SW.Input.MouseEventArgs e) {
			MouseButtons buttons = GetMouseButtons(e);
			return new Layout.Core.Platform.MouseEventArgs(e.GetPosition(element), buttons);
		}
		[System.Diagnostics.DebuggerStepThrough]
		static MouseButtons GetChangedButtons(SW.Input.MouseButtonEventArgs e) {
			MouseButtons buttons = MouseButtons.None;
			if(e.ChangedButton == MouseButton.Left) buttons |= MouseButtons.Left;
			if(e.ChangedButton == MouseButton.Middle) buttons |= MouseButtons.Middle;
			if(e.ChangedButton == MouseButton.Right) buttons |= MouseButtons.Right;
			return buttons;
		}
		[System.Diagnostics.DebuggerStepThrough]
		static MouseButtons GetMouseButtons(SW.Input.MouseEventArgs e) {
			MouseButtons buttons = MouseButtons.None;
			if(e.LeftButton == MouseButtonState.Pressed) buttons |= MouseButtons.Left;
			if(e.MiddleButton == MouseButtonState.Pressed) buttons |= MouseButtons.Middle;
			if(e.RightButton == MouseButtonState.Pressed) buttons |= MouseButtons.Right;
			if(e.XButton1 == MouseButtonState.Pressed) buttons |= MouseButtons.XButton1;
			if(e.XButton2 == MouseButtonState.Pressed) buttons |= MouseButtons.XButton2;
			return buttons;
		}
#else
		[System.Diagnostics.DebuggerStepThrough]
		public static Layout.Core.Platform.MouseEventArgs Convert(UIElement element, SW.Input.MouseEventArgs e, MouseButtons buttons, MouseButtons changed = MouseButtons.None) {
			return new Layout.Core.Platform.MouseEventArgs(e.GetPosition(element), buttons, changed);
		}
#endif
	}
	public static class HitTestHelper {
		public static bool IsDraggable(DependencyObject element) {
			if(element == null) return false;
			HitTestType hitTestType = DockPane.GetHitTestType(element);
			return hitTestType == HitTestType.Label || hitTestType == HitTestType.Header;
		}
		static bool TryGetControlBox(DependencyObject element, out BaseControlBoxControl result) {
			result = null;
			using(IEnumerator<DependencyObject> e = LayoutItemsHelper.GetEnumerator(element, null)) {
				while(e.MoveNext()) {
					if(e.Current is BaseControlBoxControl) {
						result = e.Current as BaseControlBoxControl;
						return true;
					}
				}
			}
			return false;
		}
		public static bool HitTestTypeEquals(object prevHitResult, object hitResult) {
			bool objectEquals = object.Equals(prevHitResult, hitResult);
			if(!objectEquals) {
				if(hitResult == null && object.Equals(HitTestType.Undefined, prevHitResult)) return true;
				if(prevHitResult == null && object.Equals(HitTestType.Undefined, hitResult)) return true;
			}
			return objectEquals;
		}
		static HitCache HitTestCache = new HitCache();
		public static void ResetCache() {
			HitTestCache.Reset();
		}
		public static HitTestResult GetHitResult(UIElement element, Point hitPoint) {
			return HitTestCache.GetHitResult(element, hitPoint);
		}
		public static HitTestResult HitTest(UIElement element, Point hitPoint, ref HitCache cache) {
			if(cache == null)
				cache = new HitCache();
			return cache.GetHitResult(element, hitPoint);
		}
#if !SILVERLIGHT
		public static HitTestType GetHitTestType(HitTestResult hitResult) {
			HitTestType resultType = DockPane.GetHitTestType(hitResult.VisualHit);
			if(resultType == HitTestType.Undefined) {
				resultType = CheckAdornerDecorator(hitResult);
				if(resultType == HitTestType.Undefined) {
					resultType = CheckNonLogicalTree(hitResult);
				}
			}
			return resultType;
		}
#else
		public static HitTestType GetHitTestType(HitTestResult hitResult) {
			DependencyObject child = hitResult.VisualHit;
			HitTestType result = HitTestType.Undefined;
			while(child != null) {
				result = (HitTestType)child.GetValue(DockPane.HitTestTypeProperty);
				if(result != HitTestType.Undefined)
					break;
				if(child is IUIElement)
					break;
				child = VisualTreeHelper.GetParent(child);
			}
			return result;
		}
#endif
		public static bool CheckVisualHitTest(IDockLayoutElement element, Point pt, Func<DependencyObject, DependencyObject, bool> isVisualChildDelegate) {
			UIElement hitTarget = element.View;
			UIElement hitElement = element.Element;
			if(element is FloatPanePresenterElement)
				hitElement = hitTarget;
			if(hitTarget is AutoHideTray && !(hitElement is AutoHideTray)) {
				IUIElement uiElement = hitElement as IUIElement;
				if(FindScope<AutoHidePane>(uiElement) != null) {
					AutoHidePane pane = ((AutoHideTray)hitTarget).Panel;
					pt = hitTarget.TranslatePoint(pt, pane);
					hitTarget = pane;
				}
			}
			HitTestResult result = GetHitResult(hitTarget, pt);
			DependencyObject visualHit = (result != null) ? result.VisualHit : null;
			return (visualHit != null) && isVisualChildDelegate(hitElement, visualHit);
		}
		public static bool CheckVisualHitTest(IDockLayoutElement element, Point pt) {
			return CheckVisualHitTest(element, pt, IsVisualChild);
		}
		static bool IsVisualChild(DependencyObject root, DependencyObject child) {
			DependencyObject parent = child;
			while(parent != null) {
				if(parent == root) return true;
				parent = VisualTreeHelper.GetParent(parent);
			}
			return false;
		}
		static IUIElement FindScope<T>(IUIElement element) {
			if(element == null) return null;
			if(element is T) return element;
			IUIElement parent = element.Scope;
			while(parent != null) {
				if(parent is T) return parent;
				parent = parent.Scope;
			}
			return null;
		}
		static HitTestType CheckAdornerDecorator(HitTestResult result) {
#if !SILVERLIGHT
			System.Windows.Documents.AdornerDecorator decorator =
				LayoutHelper.FindParentObject<System.Windows.Documents.AdornerDecorator>(result.VisualHit);
			if(decorator != null) {
				DependencyObject parent = LayoutHelper.GetParent(decorator);
				if(parent != null) return (HitTestType)parent.GetValue(DockPane.HitTestTypeProperty);
			}
#endif
			return HitTestType.Undefined;
		}
		static HitTestType CheckNonLogicalTree(HitTestResult hitElement) {
			HitTestType result = HitTestType.Undefined;
			DependencyObject element = hitElement.VisualHit;
			while(element != null) {
				result = DockPane.GetHitTestType(element);
				if(result != HitTestType.Undefined) break;
				element = LayoutHelper.GetParent(element);
			}
			return result;
		}
		public sealed class HitCache : IDisposable {
			class HitCacheHelper {
				class HitCacheResult {
					#region static
					public static HitCacheResult Empty;
					static HitCacheResult() {
						Empty = new HitCacheResult();
					}
					#endregion
					WeakReference lastElement;
					Point lastPoint;
					WeakReference resultReference;
					HitCacheResult() { }
					public HitCacheResult(UIElement element, Point point, HitTestResult result) {
						lastElement = new WeakReference(element);
						lastPoint = point;
						resultReference = new WeakReference(result);
					}
					bool CheckElement(UIElement element) {
						return lastElement == null || !object.ReferenceEquals(lastElement.Target, element);
					}
					public bool Accept(Point point, UIElement element) {
						return point != lastPoint || CheckElement(element);
					}
					public HitTestResult Result { get { return resultReference != null ? (HitTestResult)resultReference.Target : null; } }
				}
#if !SILVERLIGHT
				class HitCacheResultQueue : System.Collections.Concurrent.ConcurrentQueue<HitCacheResult> {
					public HitCacheResult Dequeue() {
						HitCacheResult result;
						TryDequeue(out result);
						return result;
					}
				}
#else
				class HitCacheResultQueue : Queue<HitCacheResult> { }
#endif
				const int hitCacheSize = 3;
				HitCacheResultQueue cacheInternal = new HitCacheResultQueue();
				public HitTestResult GetHitResult(UIElement element, Point point) {
					HitCacheResultQueue cache = cacheInternal;
					var potentialResult = cache.FirstOrDefault((hit) => { return !hit.Accept(point, element) && hit.Result != null; }, HitCacheResult.Empty).Result;
					if(potentialResult == null || !IsValid(potentialResult)) {
						potentialResult = new TopMostDockLayoutManagerHitResult(element, point).GetHitTest();
						cache.Enqueue(new HitCacheResult(element, point, potentialResult));
						if(cache.Count > hitCacheSize) cache.Dequeue();
					}
					return potentialResult;
				}
				public void Reset() {
#if !SILVERLIGHT
					var _cacheInternal = new HitCacheResultQueue();
					cacheInternal = _cacheInternal;
#else
					cacheInternal.Clear();
#endif
				}
			}
			HitCacheHelper hitCacheHelper;
			public HitCache() {
				hitCacheHelper = new HitCacheHelper();
			}
			public HitTestResult GetHitResult(UIElement element, Point point) {
				return hitCacheHelper.GetHitResult(element, point);
			}
			public void Dispose() {
				Reset();
				GC.SuppressFinalize(this);
			}
			static bool IsValid(HitTestResult result) {
				return (result != null) && LayoutHelper.GetParent(result.VisualHit) != null;
			}
			public void Reset() {
				hitCacheHelper.Reset();
			}
		}
		class TopMostDockLayoutManagerHitResult {
			HitTestResult _hitResult;
			public TopMostDockLayoutManagerHitResult(UIElement view, Point hitPoint) {
#if !SILVERLIGHT
				VisualTreeHelper.HitTest(view, NoNestedDockLayoutManager, HitTestResult, new PointHitTestParameters(hitPoint));
#else
				HitTest(view, NoNestedDockLayoutManager, HitTestResult, new PointHitTestParameters(hitPoint));
#endif
			}
			public HitTestResult GetHitTest() { return _hitResult; }
			HitTestResultBehavior HitTestResult(HitTestResult result) {
				this._hitResult = result;
				return HitTestResultBehavior.Stop;
			}
			HitTestFilterBehavior NoNestedDockLayoutManager(DependencyObject potentialHitTestTarget) {
				if(potentialHitTestTarget is UIElement && !isVisible((UIElement)potentialHitTestTarget))
					return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
				if(potentialHitTestTarget is DockLayoutManager)
					return HitTestFilterBehavior.ContinueSkipChildren;
				return HitTestFilterBehavior.Continue;
			}
#if SILVERLIGHT
			static void HitTest(UIElement reference, HitTestFilterCallback filterCallback, HitTestResultCallback resultCallback, PointHitTestParameters hitTestParameters) {
				if(resultCallback == null && filterCallback == null) return;
				HitTestFilterBehavior filterBehaviorRes = HitTestFilterBehavior.Continue;
				DependencyObject parentForSkipChildren = null;
				IEnumerable<UIElement> elements = GetHitElements(reference, hitTestParameters.HitPoint);
				foreach(UIElement element in elements) {
					filterBehaviorRes = FilterCallback(filterCallback, element, parentForSkipChildren == null);
					if(filterBehaviorRes == HitTestFilterBehavior.Stop)
						break;
					if(filterBehaviorRes == HitTestFilterBehavior.ContinueSkipSelf)
						continue;
					if(filterBehaviorRes == HitTestFilterBehavior.ContinueSkipChildren)
						parentForSkipChildren = element;
					else if(filterBehaviorRes == HitTestFilterBehavior.ContinueSkipSelfAndChildren) {
						parentForSkipChildren = element;
						continue;
					}
					if(VisualTreeHelper.GetParent(element) == parentForSkipChildren)
						continue;
					else parentForSkipChildren = null;
					if(ResultCallback(resultCallback, element) == HitTestResultBehavior.Stop)
						break;
				}
			}
			static IEnumerable<UIElement> GetHitElements(UIElement treeRoot, Point hitPoint) {
				UIElement appRoot = Application.Current.RootVisual;
				Point appHitPoint = treeRoot.TransformToVisual(appRoot).Transform(hitPoint);
				if(CalcFlowDirection(appRoot) != CalcFlowDirection(treeRoot))
					appHitPoint.X = appRoot.RenderSize.Width - appHitPoint.X;
				return VisualTreeHelper.FindElementsInHostCoordinates(appHitPoint, treeRoot);
			}
			static FlowDirection CalcFlowDirection(DependencyObject dObj) {
				object localValue = dObj.ReadLocalValue(FrameworkElement.FlowDirectionProperty);
				return object.Equals(localValue, DependencyProperty.UnsetValue) ?
					default(FlowDirection) : (FlowDirection)localValue;
			}
			static HitTestFilterBehavior FilterCallback(HitTestFilterCallback filterCallback,
				DependencyObject patentialHitTestTarget, bool raise) {
				if(filterCallback == null || !raise)
					return HitTestFilterBehavior.Continue;
				return filterCallback(patentialHitTestTarget);
			}
			static HitTestResultBehavior ResultCallback(HitTestResultCallback resultCallback,
				DependencyObject visualHit) {
				if(resultCallback == null)
					return HitTestResultBehavior.Continue;
				return resultCallback(new HitTestResult(visualHit));
			}
			bool isVisible(UIElement element) {
				return element.Visibility == Visibility.Visible;
			}
#else
			bool isVisible(UIElement element) {
				return element.IsVisible;
			}
#endif
		}
	}
	public static class CaptionLocationExtension {
		public static SWC.Dock ToDock(this CaptionLocation captionLocation) {
			switch(captionLocation) {
				case CaptionLocation.Right:
					return SWC.Dock.Right;
				case CaptionLocation.Bottom:
					return SWC.Dock.Bottom;
				case CaptionLocation.Left:
					return SWC.Dock.Left;
				default:
					return SWC.Dock.Top;
			}
		}
	}
	public static class AutoHideTypeExtension {
		public static SWC.Dock ToDock(this AutoHideType type) {
			switch(type) {
				case AutoHideType.Right:
					return SWC.Dock.Right;
				case AutoHideType.Top:
					return SWC.Dock.Top;
				case AutoHideType.Bottom:
					return SWC.Dock.Bottom;
				default:
					return SWC.Dock.Left;
			}
		}
		public static AutoHideType ToAutoHideType(this SWC.Dock dock) {
			switch(dock) {
				case SWC.Dock.Left:
					return AutoHideType.Left;
				case SWC.Dock.Right:
					return AutoHideType.Right;
				case SWC.Dock.Top:
					return AutoHideType.Top;
				case SWC.Dock.Bottom:
					return AutoHideType.Bottom;
				default:
					return AutoHideType.Default;
			}
		}
	}
}
