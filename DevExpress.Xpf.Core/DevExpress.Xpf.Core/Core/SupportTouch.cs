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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.UI.Interactivity;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Core.Native;
using System.Windows.Markup;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Core {
	public enum ScrollBarMode { Standard, TouchOverlap }
	public class ScrollBarExtensions : DependencyObject {
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyPropertyKey HorizontalMouseWheelListeningInitializedPropertyKey = DependencyProperty.RegisterAttachedReadOnly("HorizontalMouseWheelListeningInitialized",
			typeof(bool), typeof(ScrollBarExtensions), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, HorizontalMouseWheelListeningInitializedChanged));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty HorizontalMouseWheelListeningInitializedProperty = HorizontalMouseWheelListeningInitializedPropertyKey.DependencyProperty;
		public static readonly DependencyProperty HandlesDefaultMouseScrollingProperty = DependencyProperty.RegisterAttached("HandlesDefaultMouseScrolling", typeof(bool), typeof(ScrollBarExtensions),
			new PropertyMetadata(true));
		public static readonly DependencyProperty AllowMouseScrollingProperty = DependencyProperty.RegisterAttached("AllowMouseScrolling", typeof(bool), typeof(ScrollBarExtensions),
			new PropertyMetadata(false, AllowMouseScrollingChanged));
		public static readonly DependencyProperty ScrollBehaviorProperty = DependencyProperty.RegisterAttached("ScrollBehavior", typeof(ScrollBehaviorBase), typeof(ScrollBarExtensions),
			new PropertyMetadata(null));
		public static readonly DependencyProperty PreventParentScrollingProperty = DependencyProperty.RegisterAttached("PreventParentScrolling", typeof(bool), typeof(ScrollBarExtensions),
	new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
		public static readonly DependencyProperty ScrollBarModeProperty = DependencyProperty.RegisterAttached("ScrollBarMode", typeof(ScrollBarMode), typeof(ScrollBarExtensions),
			new FrameworkPropertyMetadata(ScrollBarMode.Standard, FrameworkPropertyMetadataOptions.Inherits));
		public static readonly DependencyProperty IsTouchScrollBarModeProperty = DependencyProperty.RegisterAttached("IsTouchScrollBarMode", typeof(bool), typeof(ScrollBarExtensions),
			new PropertyMetadata(false));
		public static readonly DependencyProperty ScrollViewerMouseMovedProperty = DependencyProperty.RegisterAttached("ScrollViewerMouseMoved", typeof(bool), typeof(ScrollBarExtensions),
			new PropertyMetadata(false));
		public static readonly DependencyProperty ScrollViewerSizeChangedProperty = DependencyProperty.RegisterAttached("ScrollViewerSizeChanged", typeof(bool), typeof(ScrollBarExtensions),
			new PropertyMetadata(false));
		public static readonly DependencyProperty CurrentHorizontalScrollMarginProperty = DependencyProperty.RegisterAttached("CurrentHorizontalScrollMargin", typeof(Thickness), typeof(ScrollBarExtensions),
			new PropertyMetadata(new Thickness()));
		public static readonly DependencyProperty ScrollViewerOrientationProperty = DependencyProperty.RegisterAttached("ScrollViewerOrientation", typeof(Orientation?), typeof(ScrollBarExtensions),
			new PropertyMetadata(null));
		public static readonly DependencyProperty ListeningScrollBarThumbDragDeltaProperty = DependencyProperty.RegisterAttached("ListeningScrollBarThumbDragDelta", typeof(bool), typeof(ScrollBarExtensions), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, ListeningScrollBarThumbDragDeltaChanged));
		public static readonly DependencyProperty IsScrollBarThumbDragDeltaListenerProperty = DependencyProperty.RegisterAttached("IsScrollBarThumbDragDeltaListener", typeof(bool), typeof(ScrollBarExtensions),
		  new FrameworkPropertyMetadata(false));
		public static bool GetIsScrollBarThumbDragDeltaListener(DependencyObject d) {
			return (bool)d.GetValue(IsScrollBarThumbDragDeltaListenerProperty);
		}
		public static void SetIsScrollBarThumbDragDeltaListener(DependencyObject d, bool value) {
			d.SetValue(IsScrollBarThumbDragDeltaListenerProperty, value);
		}
		static void ListeningScrollBarThumbDragDeltaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is Thumb) {
				var behaviors = Interaction.GetBehaviors(d);
				if ((bool)e.NewValue)
					behaviors.Add(new ScrollBarThumbDragDeltaBehavior());
				else {
					var behavior = behaviors.FirstOrDefault(x => x is ScrollBarThumbDragDeltaBehavior);
					if (behavior != null)
						behaviors.Remove(behavior);
				}
			}
		}
		public static bool GetListeningScrollBarThumbDragDelta(DependencyObject d) {
			return (bool)d.GetValue(ListeningScrollBarThumbDragDeltaProperty);
		}
		public static void SetListeningScrollBarThumbDragDelta(DependencyObject d, bool value) {
			d.SetValue(ListeningScrollBarThumbDragDeltaProperty, value);
		}
		public static bool GetHandlesDefaultMouseScrolling(DependencyObject d) {
			return (bool)d.GetValue(HandlesDefaultMouseScrollingProperty);
		}
		public static void SetHandlesDefaultMouseScrolling(DependencyObject d, bool value) {
			d.SetValue(HandlesDefaultMouseScrollingProperty, value);
		}
		static void AllowMouseScrollingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var behaviors = Interaction.GetBehaviors(d);
			if ((bool)e.NewValue)
				behaviors.Add(new MouseScrollBehavior());
			else {
				var behavior = behaviors.FirstOrDefault(x => x is MouseScrollBehavior);
				if (behavior != null)
					behaviors.Remove(behavior);
			}
		}
		static void HorizontalMouseWheelListeningInitializedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var behaviors = Interaction.GetBehaviors(d);
			if ((bool)e.NewValue)
				behaviors.Add(new HWndHostWMMouseHWheelBehavior());
			else {
				var behavior = behaviors.FirstOrDefault(x => x is HWndHostWMMouseHWheelBehavior);
				if (behavior != null)
					behaviors.Remove(behavior);
			}
		}
		public static bool GetHorizontalMouseWheelListeningInitializedProperty(DependencyObject d) {
			return (bool)d.GetValue(HorizontalMouseWheelListeningInitializedProperty);
		}
		internal static void SetHorizontalMouseWheelListeningInitializedProperty(DependencyObject d, bool value) {
			d.SetValue(HorizontalMouseWheelListeningInitializedPropertyKey, value);
		}
		public static bool GetAllowMouseScrolling(DependencyObject d) {
			return (bool)d.GetValue(AllowMouseScrollingProperty);
		}
		public static void SetAllowMouseScrolling(DependencyObject d, bool value) {
			d.SetValue(AllowMouseScrollingProperty, value);
		}
		public static ScrollBehaviorBase GetScrollBehavior(DependencyObject obj) {
			return (ScrollBehaviorBase)obj.GetValue(ScrollBehaviorProperty);
		}
		public static void SetScrollBehavior(DependencyObject obj, ScrollBehaviorBase value) {
			obj.SetValue(ScrollBehaviorProperty, value);
		}
		public static bool GetPreventParentScrolling(DependencyObject obj) {
			return (bool)obj.GetValue(PreventParentScrollingProperty);
		}
		public static void SetPreventParentScrolling(DependencyObject obj, bool value) {
			obj.SetValue(PreventParentScrollingProperty, value);
		}
		public static ScrollBarMode GetScrollBarMode(DependencyObject obj) {
			return (ScrollBarMode)obj.GetValue(ScrollBarModeProperty);
		}
		public static void SetScrollBarMode(DependencyObject obj, ScrollBarMode value) {
			obj.SetValue(ScrollBarModeProperty, value);
		}
		public static bool GetIsTouchScrollBarMode(DependencyObject obj) {
			return (bool)obj.GetValue(IsTouchScrollBarModeProperty);
		}
		public static void SetIsTouchScrollBarMode(DependencyObject obj, bool value) {
			obj.SetValue(IsTouchScrollBarModeProperty, value);
		}
		public static bool GetScrollViewerMouseMoved(DependencyObject obj) {
			return (bool)obj.GetValue(ScrollViewerMouseMovedProperty);
		}
		public static void SetScrollViewerMouseMoved(DependencyObject obj, bool value) {
			obj.SetValue(ScrollViewerMouseMovedProperty, value);
		}
		public static bool GetScrollViewerSizeChanged(DependencyObject obj) {
			return (bool)obj.GetValue(ScrollViewerSizeChangedProperty);
		}
		public static void SetScrollViewerSizeChanged(DependencyObject obj, bool value) {
			obj.SetValue(ScrollViewerSizeChangedProperty, value);
		}
		public static Thickness GetCurrentHorizontalScrollMargin(DependencyObject obj) {
			return (Thickness)obj.GetValue(CurrentHorizontalScrollMarginProperty);
		}
		public static void SetCurrentHorizontalScrollMargin(DependencyObject obj, Thickness value) {
			obj.SetValue(CurrentHorizontalScrollMarginProperty, value);
		}
		public static Orientation? GetScrollViewerOrientation(DependencyObject obj) {
			return (Orientation?)obj.GetValue(ScrollViewerOrientationProperty);
		}
		public static void SetScrollViewerOrientation(DependencyObject obj, Orientation? value) {
			obj.SetValue(ScrollViewerOrientationProperty, value);
		}
	}
	public abstract class ScrollBehaviorBase : MarkupExtension {
		public abstract bool CanScrollUp(DependencyObject source);
		public abstract bool CanScrollDown(DependencyObject source);
		public abstract bool CanScrollLeft(DependencyObject source);
		public abstract bool CanScrollRight(DependencyObject source);
		public abstract void ScrollToHorizontalOffset(DependencyObject source, double offset);
		public abstract void ScrollToVerticalOffset(DependencyObject source, double offset);
		public abstract void MouseWheelDown(DependencyObject source);
		public abstract void MouseWheelUp(DependencyObject source);
		public abstract void MouseWheelLeft(DependencyObject source);
		public abstract void MouseWheelRight(DependencyObject source);
		public abstract bool CheckHandlesMouseWheelScrolling(DependencyObject source);
		public abstract bool PreventMouseScrolling(DependencyObject source);
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
	public class NativeScrollBehavior : ScrollBehaviorBase {
		public override bool CanScrollUp(DependencyObject source) { return true; }
		public override bool CanScrollDown(DependencyObject source) { return true; }
		public override bool CanScrollLeft(DependencyObject source) { return true; }
		public override bool CanScrollRight(DependencyObject source) { return true; }
		public override void ScrollToHorizontalOffset(DependencyObject source, double offset) { }
		public override void ScrollToVerticalOffset(DependencyObject source, double offset) { }
		public override void MouseWheelDown(DependencyObject source) { }
		public override void MouseWheelUp(DependencyObject source) { }
		public override void MouseWheelLeft(DependencyObject source) { }
		public override void MouseWheelRight(DependencyObject source) { }
		public override bool CheckHandlesMouseWheelScrolling(DependencyObject source) { return true; }
		public override bool PreventMouseScrolling(DependencyObject source) { return true; }
	}
	public class BaseEditScrollBehavior : NativeScrollBehavior {
		public override bool CheckHandlesMouseWheelScrolling(DependencyObject source) {
			return ((DevExpress.Xpf.Editors.BaseEdit)source).EditMode != EditMode.InplaceInactive;
		}
	}
	public class ScrollViewerScrollBehavior : ScrollBehaviorBase {
		static readonly ReflectionHelper ReflectionHelper = new ReflectionHelper();
		ScrollViewer GetScrollViewer(DependencyObject source) { return (ScrollViewer)source; }
		IScrollInfo GetScrollInfo(DependencyObject source) {
			return ReflectionHelper.GetPropertyValue<IScrollInfo>(source, "ScrollInfo", BindingFlags.NonPublic | BindingFlags.Instance);
		}
		public override bool CanScrollDown(DependencyObject source) {
			ScrollViewer scrollViewer = GetScrollViewer(source);
			IScrollInfo scrollInfo = GetScrollInfo(source);
			if (scrollInfo != null && !scrollInfo.CanVerticallyScroll)
				return false;
			return (scrollViewer.VerticalOffset + scrollViewer.ViewportHeight) < scrollViewer.ExtentHeight;
		}
		public override bool CanScrollUp(DependencyObject source) {
			ScrollViewer scrollViewer = GetScrollViewer(source);
			IScrollInfo scrollInfo = GetScrollInfo(source);
			if (scrollInfo != null && !scrollInfo.CanVerticallyScroll)
				return false;
			return scrollViewer.VerticalOffset > 0;
		}
		public override bool CanScrollLeft(DependencyObject source) {
			ScrollViewer scrollViewer = GetScrollViewer(source);
			IScrollInfo scrollInfo = GetScrollInfo(source);
			if (scrollInfo != null && !scrollInfo.CanHorizontallyScroll)
				return false;
			return scrollViewer.HorizontalOffset > 0;
		}
		public override bool CanScrollRight(DependencyObject source) {
			ScrollViewer scrollViewer = GetScrollViewer(source);
			IScrollInfo scrollInfo = GetScrollInfo(source);
			if (scrollInfo != null && !scrollInfo.CanHorizontallyScroll)
				return false;
			return (scrollViewer.HorizontalOffset + scrollViewer.ViewportWidth) < scrollViewer.ExtentWidth;
		}
		public override void ScrollToHorizontalOffset(DependencyObject source, double offset) {
			ScrollViewer scrollViewer = GetScrollViewer(source);
			IScrollInfo scrollInfo = GetScrollInfo(source);
			if (scrollInfo != null)
				scrollInfo.SetHorizontalOffset(scrollInfo.HorizontalOffset + offset);
			else
				scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + offset);
		}
		public override void ScrollToVerticalOffset(DependencyObject source, double offset) {
			ScrollViewer scrollViewer = GetScrollViewer(source);
			IScrollInfo scrollInfo = GetScrollInfo(source);
			if (scrollInfo != null)
				scrollInfo.SetVerticalOffset(scrollInfo.VerticalOffset - offset);
			else
				scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offset);
		}
		public override void MouseWheelDown(DependencyObject source) {
			ScrollViewer scrollViewer = GetScrollViewer(source);
			IScrollInfo scrollInfo = GetScrollInfo(source);
			if (scrollInfo != null)
				scrollInfo.MouseWheelDown();
			else
				DoWheelScrollLines(scrollViewer.LineDown);
		}
		public override void MouseWheelUp(DependencyObject source) {
			ScrollViewer scrollViewer = GetScrollViewer(source);
			IScrollInfo scrollInfo = GetScrollInfo(source);
			if (scrollInfo != null)
				scrollInfo.MouseWheelUp();
			else
				DoWheelScrollLines(scrollViewer.LineUp);
		}
		public override void MouseWheelLeft(DependencyObject source) {
			ScrollViewer scrollViewer = GetScrollViewer(source);
			IScrollInfo scrollInfo = GetScrollInfo(source);
			if (scrollInfo != null)
				scrollInfo.MouseWheelLeft();
			else
				DoWheelScrollLines(scrollViewer.LineLeft);
		}
		public override void MouseWheelRight(DependencyObject source) {
			ScrollViewer scrollViewer = GetScrollViewer(source);
			IScrollInfo scrollInfo = GetScrollInfo(source);
			if (scrollInfo != null)
				scrollInfo.MouseWheelRight();
			else
				DoWheelScrollLines(scrollViewer.LineRight);
		}
		void DoWheelScrollLines(Action action) {
			for (int i = 0; i < SystemParameters.WheelScrollLines; i++)
				action();
		}
		public override bool CheckHandlesMouseWheelScrolling(DependencyObject source) {
			return ReflectionHelper.GetPropertyValue<bool>(source, "HandlesMouseWheelScrolling", BindingFlags.NonPublic | BindingFlags.Instance);
		}
		public override bool PreventMouseScrolling(DependencyObject source) { return false; }
	}
	public class MouseScrollBehavior : Behavior<FrameworkElement> {
		#region Inner Classes
		class ScrollBehaviorInfo : Tuple<DependencyObject, ScrollBehaviorBase> {
			public ScrollBehaviorInfo(DependencyObject source, ScrollBehaviorBase behavior) : base(source, behavior) { }
			public bool CanScrollUp() { return Item2.CanScrollUp(Item1); }
			public bool CanScrollDown() { return Item2.CanScrollDown(Item1); }
			public bool CanScrollLeft() { return Item2.CanScrollLeft(Item1); }
			public bool CanScrollRight() { return Item2.CanScrollRight(Item1); }
			public void ScrollToHorizontalOffset(double offset) { Item2.ScrollToHorizontalOffset(Item1, offset); }
			public void ScrollToVerticalOffset(double offset) { Item2.ScrollToVerticalOffset(Item1, offset); }
			public void MouseWheelDown() { Item2.MouseWheelDown(Item1); }
			public void MouseWheelUp() { Item2.MouseWheelUp(Item1); }
			public void MouseWheelLeft() { Item2.MouseWheelLeft(Item1); }
			public void MouseWheelRight() { Item2.MouseWheelRight(Item1); }
			public bool CheckHandlesMouseWheelScrolling() { return Item2.CheckHandlesMouseWheelScrolling(Item1); }
			public bool PreventMouseScrolling() { return Item2.PreventMouseScrolling(Item1); }
			public bool PreventParentScrolling() { return ScrollBarExtensions.GetPreventParentScrolling(Item1); }
		}
		#endregion
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.Loaded += AssociatedObjectOnLoaded;
			AssociatedObject.Unloaded += AssociatedObject_Unloaded;
			AssociatedObject.PreviewMouseWheel += AssociatedObjectPreviewMouseWheel;
			VerifyMouseHWheelListening();
		}
#if DEBUGTEST
		public
#endif
 void AssociatedObjectPreviewMouseWheel(object sender, MouseWheelEventArgs e) {
			var dxmea = e as MouseWheelEventArgsEx;
			if (dxmea == null)
				PerformVerticalScrolling(e);
			else
				PerformHorizontalScrolling(dxmea);
		}
		ScrollBehaviorInfo GetScrollBehaviorInfo(Func<ScrollBehaviorInfo, bool> canScroll, Point point) {
			List<ScrollBehaviorInfo> list = new List<ScrollBehaviorInfo>();
			VisualTreeHelper.HitTest(AssociatedObject, hitTest => HitTestFilterCallback(list, canScroll, hitTest), HitTestResultCallback, new PointHitTestParameters(point));
			foreach (ScrollBehaviorInfo el in list) {
				if (el.PreventMouseScrolling())
					return null;
			}
			return list.LastOrDefault() ?? FindScrollBehaviorInfo(AssociatedObject).If(info => info.CheckHandlesMouseWheelScrolling());
		}
		ScrollBehaviorInfo FindScrollBehaviorInfo(DependencyObject source) {
			return source.With(s => ScrollBarExtensions.GetScrollBehavior(s)).With(b => new ScrollBehaviorInfo(source, b));
		}
		HitTestFilterBehavior HitTestFilterCallback(List<ScrollBehaviorInfo> list, Func<ScrollBehaviorInfo, bool> canScroll, DependencyObject potentialHitTestTarget) {
			potentialHitTestTarget.With(x => FindScrollBehaviorInfo(x)).If(info => info.CheckHandlesMouseWheelScrolling()).If(canScroll).Do(list.Add);
			return HitTestFilterBehavior.Continue;
		}
		HitTestResultBehavior HitTestResultCallback(HitTestResult result) {
			return HitTestResultBehavior.Continue;
		}
		bool CanScrollHorizontal(ScrollBehaviorInfo scrollInfo, double delta) {
			if (scrollInfo.PreventParentScrolling()) return true;
			if (delta > 0)
				return scrollInfo.CanScrollRight();
			if (delta < 0)
				return scrollInfo.CanScrollLeft();
			return false;
		}
		bool CanScrollVertical(ScrollBehaviorInfo scrollInfo, double delta) {
			if (scrollInfo.PreventParentScrolling()) return true;
			if (delta > 0)
				return scrollInfo.CanScrollUp();
			if (delta < 0)
				return scrollInfo.CanScrollDown();
			return false;
		}
		void PerformHorizontalScrolling(MouseWheelEventArgsEx e) {
			if (e.DeltaX % 120 == 0)
				PerformHorizontalStandardScrolling(e);
			else
				PerformHorizontalPerPixelScrolling(e);
			if(e.DeltaY == 0) e.Handled = true;
		}
		void PerformHorizontalPerPixelScrolling(MouseWheelEventArgsEx e) {
			ScrollBehaviorInfo scrollInfo = GetScrollBehaviorInfo((info) => CanScrollHorizontal(info, e.DeltaX), e.GetPosition(AssociatedObject));
			if (scrollInfo == null)
				return;
			scrollInfo.ScrollToHorizontalOffset(e.DeltaX);
		}
		void PerformHorizontalStandardScrolling(MouseWheelEventArgsEx e) {
			ScrollBehaviorInfo scrollInfo = GetScrollBehaviorInfo((info) => CanScrollHorizontal(info, e.DeltaX), e.GetPosition(AssociatedObject));
			if (scrollInfo == null)
				return;
			int lineCount = e.DeltaX / 120;
			if (lineCount > 0)
				ScrollByLine(lineCount, scrollInfo.MouseWheelRight);
			else if (lineCount < 0)
				ScrollByLine(lineCount, scrollInfo.MouseWheelLeft);
		}
		void PerformVerticalScrolling(MouseWheelEventArgs e) {
			if (e.Delta % 120 == 0)
				PerformVerticalStandardScrolling(e);
			else
				PerformVerticalPerPixelScrolling(e);
		}
		void PerformVerticalStandardScrolling(MouseWheelEventArgs e) {
			ScrollBehaviorInfo scrollInfo = GetScrollBehaviorInfo((info) => CanScrollVertical(info, e.Delta), e.GetPosition(AssociatedObject));
			if (scrollInfo == null)
				return;
			if (!ScrollBarExtensions.GetHandlesDefaultMouseScrolling(scrollInfo.Item1))
				return;
			int lineCount = e.Delta / 120;
			if (lineCount > 0)
				ScrollByLine(lineCount, scrollInfo.MouseWheelUp);
			else if (lineCount < 0)
				ScrollByLine(lineCount, scrollInfo.MouseWheelDown);
			e.Handled = lineCount != 0;
		}
		void PerformVerticalPerPixelScrolling(MouseWheelEventArgs e) {
			ScrollBehaviorInfo scrollInfo = GetScrollBehaviorInfo((info) => CanScrollVertical(info, e.Delta), e.GetPosition(AssociatedObject));
			if (scrollInfo == null)
				return;
			scrollInfo.ScrollToVerticalOffset(e.Delta);
			e.Handled = true;
		}
		void ScrollByLine(int lineCount, Action scrollAction) {
			for (int i = 0; i < Math.Abs(lineCount); i++)
				scrollAction();
		}
		void AssociatedObject_Unloaded(object sender, RoutedEventArgs e) {
		}
		void AssociatedObjectOnLoaded(object sender, RoutedEventArgs e) {
			VerifyMouseHWheelListening();
		}
		protected override void OnDetaching() {
			AssociatedObject.Loaded -= AssociatedObjectOnLoaded;
			AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
			AssociatedObject.PreviewMouseWheel -= AssociatedObjectPreviewMouseWheel;
			base.OnDetaching();
		}
		void VerifyMouseHWheelListening() {
			if (!AssociatedObject.IsLoaded)
				return;
			var root = LayoutHelper.FindRoot(AssociatedObject);
			if (object.Equals(root, AssociatedObject))
				return;
			ScrollBarExtensions.SetHorizontalMouseWheelListeningInitializedProperty(root, true);
		}
	}
	public class MouseWheelEventArgsEx : MouseWheelEventArgs {
		public int DeltaX { get; private set; }
		public int DeltaY { get { return Delta; } }
		public MouseWheelEventArgsEx(MouseDevice mouse, int timestamp, int deltaX, int deltaY)
			: this(mouse, timestamp, deltaY) {
			DeltaX = deltaX;
		}
		public MouseWheelEventArgsEx(MouseDevice mouse, int timestamp, int delta)
			: base(mouse, timestamp, delta) {
		}
	}
	public class HWndHostWMMouseHWheelBehavior : Behavior<FrameworkElement> {
		public static readonly RoutedEvent PreviewInputReportEvent = EventManager.RegisterRoutedEvent("PreviewInputReport", RoutingStrategy.Tunnel, typeof(EventHandler), typeof(HWndHostWMMouseHWheelBehavior));
		HwndSource source;
		static HWndHostWMMouseHWheelBehavior() {
			InputManager.Current.PreProcessInput += CurrentPreProcessInput;
		}
		static void CurrentPreProcessInput(object sender, PreProcessInputEventArgs e) {
			if (e.StagingItem.Input.Device == null)
				return;
			if (!(e.StagingItem.Input.Device is MouseDevice))
				return;
			var item = e.StagingItem;
			var irea = e.StagingItem.Input as DXInputReportEventArgs;
			if (irea == null || irea.Handled)
				return;
			e.Cancel();
			var mwea = new MouseWheelEventArgsEx((MouseDevice)item.Input.Device, item.Input.Timestamp, irea.Report.Wheel, 0);
			mwea.RoutedEvent = Mouse.PreviewMouseWheelEvent;
			e.PushInput(mwea, e.StagingItem);
		}
		protected override void OnAttached() {
			base.OnAttached();
			source = (HwndSource)PresentationSource.FromVisual(AssociatedObject);
			source.Do(x => x.AddHook(Filter));
		}
		IntPtr Filter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			IntPtr result = IntPtr.Zero;
			if (msg != NativeMethods.WM_MOUSEHWHEEL)
				return result;
			int wheel = NativeMethods.SignedHIWORD(NativeMethods.IntPtrToInt32(wParam));
			NativeMethods.POINT pt = new NativeMethods.POINT(NativeMethods.SignedLOWORD(NativeMethods.IntPtrToInt32(lParam)), NativeMethods.SignedHIWORD(NativeMethods.IntPtrToInt32(lParam)));
			if (!ScreenToClient(hwnd, ref pt))
				throw new Win32Exception();
			int x = pt.x;
			int y = pt.y;
			int msgTime = GetMessageTime();
			handled = ReportInput(InputMode.Foreground, msgTime, x, y, wheel);
			return result;
		}
		[SecuritySafeCritical]
		int GetMessageTime() {
			return NativeMethods.GetMessageTime();
		}
		[SecuritySafeCritical]
		bool ScreenToClient(IntPtr hwnd, ref NativeMethods.POINT pt) {
			return NativeMethods.ScreenToClient(new HandleRef(this, hwnd).Handle, ref pt);
		}
		bool ReportInput(InputMode inputMode, int timeStamp, int x, int y, int wheel) {
			MouseDevice primaryMouseDevice = Mouse.PrimaryDevice;
			if (primaryMouseDevice == null || primaryMouseDevice.ActiveSource == null)
				return false;
			var inputSource = primaryMouseDevice.ActiveSource;
			if (inputSource == null || inputSource.IsDisposed)
				return false;
			var inputReport = new DXInputReport(inputSource, inputMode, timeStamp, x, y, wheel);
			InputEventArgs inputReportEventArgs = new DXInputReportEventArgs(primaryMouseDevice, inputReport);
			inputReportEventArgs.RoutedEvent = PreviewInputReportEvent;
			return InputManager.Current.ProcessInput(inputReportEventArgs);
		}
		protected override void OnDetaching() {
			source.Do(x => x.RemoveHook(Filter));
			source = null;
			base.OnDetaching();
		}
		public class DXInputReportEventArgs : InputEventArgs {
			public DXInputReport Report { get; private set; }
			public DXInputReportEventArgs(MouseDevice inputDevice, DXInputReport report)
				: base(inputDevice, report.Timestamp) {
				Report = report;
			}
		}
		public class DXInputReport {
			readonly int x;
			readonly int y;
			readonly int wheel;
			readonly PresentationSource inputSource;
			public int X {
				get {
					return x;
				}
			}
			public int Y {
				get {
					return y;
				}
			}
			public int Wheel {
				get {
					return wheel;
				}
			}
			readonly InputType type;
			readonly InputMode mode;
			readonly int timestamp;
			public Visual RootVisual { get { return inputSource.With(i => i.RootVisual); } }
			public InputType Type {
				get {
					return type;
				}
			}
			public InputMode Mode {
				get {
					return mode;
				}
			}
			public int Timestamp {
				get {
					return timestamp;
				}
			}
			public DXInputReport(PresentationSource inputSource, InputMode mode, int timestamp, int x, int y, int wheel) {
				this.mode = mode;
				this.timestamp = timestamp;
				this.x = x;
				this.y = y;
				this.type = InputType.Mouse;
				this.wheel = wheel;
				this.inputSource = inputSource;
			}
		}
	}
	public class ScrollViewerTouchBehavior : Behavior<FrameworkElement> {
		#region static
		public static readonly DependencyProperty IsEnabledProperty;
		public static readonly DependencyProperty OrientationProperty;
		static ScrollViewerTouchBehavior() {
			IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ScrollViewerTouchBehavior), new PropertyMetadata(false, IsEnabledChanged));
			OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation?), typeof(ScrollViewerTouchBehavior), new UIPropertyMetadata(null, new PropertyChangedCallback(OnOrientationChanged)));
		}
		static void IsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var behaviors = Interaction.GetBehaviors(d);
			if ((bool)e.NewValue)
				behaviors.Add(new ScrollViewerTouchBehavior());
			else {
				var behavior = behaviors.FirstOrDefault(x => x is ScrollViewerTouchBehavior);
				if (behavior != null)
					behaviors.Remove(behavior);
			}
		}
		public static bool GetIsEnabled(DependencyObject d) {
			return (bool)d.GetValue(IsEnabledProperty);
		}
		public static void SetIsEnabled(DependencyObject d, bool value) {
			d.SetValue(IsEnabledProperty, value);
		}
		private static void OnOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ScrollViewerTouchBehavior scrollViewerTouchBehavior = o as ScrollViewerTouchBehavior;
			if (scrollViewerTouchBehavior != null)
				scrollViewerTouchBehavior.OnOrientationChanged((Orientation?)e.OldValue, (Orientation?)e.NewValue);
		}
		#endregion
		ScrollableObjectBase ScrollViewer;
		public Orientation? Orientation {
			get { return (Orientation?)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		ScrollableObjectFactory _ScrollFactory;
		ScrollableObjectFactory ScrollFactory {
			get {
				if (_ScrollFactory == null) _ScrollFactory = new ScrollableObjectFactory();
				return _ScrollFactory;
			}
		}
		void UpdaterTimerTick(object sender, EventArgs e) {
			updaterTimer.Stop();
			if (AssociatedObject == null)
				return;
			ScrollBarExtensions.SetScrollViewerMouseMoved(AssociatedObject, false);
			ScrollBarExtensions.SetScrollViewerSizeChanged(AssociatedObject, false);
		}
		DispatcherTimer updaterTimer;
		void StartUpdateTimer() {
			if (updaterTimer == null) {
				updaterTimer = new DispatcherTimer();
				updaterTimer.Interval = TimeSpan.FromMilliseconds(100);
				updaterTimer.Tick += UpdaterTimerTick;
			}
			StopTimer();
			updaterTimer.Start();
		}
		void StopTimer() {
			if (updaterTimer != null && updaterTimer.IsEnabled)
				updaterTimer.Stop();
		}
		protected virtual void OnOrientationChanged(Orientation? oldValue, Orientation? newValue) {
			UpdateAssociatedObjectScrollViewerOrientation();
		}
		protected override void OnAttached() {
			base.OnAttached();
			if (AssociatedObject.IsLoaded)
				InitializeScrollViewer();
			else
				AssociatedObject.Loaded += AssociatedObject_Loaded;
		}
		protected override void OnDetaching() {
			StopTimer();
			if (ScrollViewer != null) {
				ScrollViewer.ScrollChanged -= AssociatedObject_ScrollChanged;
				ScrollViewer.Detach();
				ScrollViewer = null;
			}
			AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
			AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
			if (updaterTimer != null)
				updaterTimer.Tick -= UpdaterTimerTick;
			base.OnDetaching();
		}
		void AssociatedObject_Loaded(object sender, RoutedEventArgs e) {
			if (AssociatedObject == null) {
				FrameworkElement el = sender as FrameworkElement;
				el.Do(x => x.Loaded -= AssociatedObject_Loaded);
				return;
			}
			AssociatedObject.Loaded -= AssociatedObject_Loaded;
			InitializeScrollViewer();
		}
		void InitializeScrollViewer() {
			ScrollViewer = ScrollFactory.Resolve(AssociatedObject);
			if (ScrollViewer != null) {
				ScrollViewer.ScrollChanged += AssociatedObject_ScrollChanged;
				ScrollViewer.Attach(AssociatedObject);
			}
			AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
			AssociatedObject.MouseMove += AssociatedObject_MouseMove;
			UpdateAssociatedObjectScrollViewerOrientation();
		}
		void UpdateAssociatedObjectScrollViewerOrientation() {
			if (AssociatedObject != null && ScrollBarExtensions.GetScrollViewerOrientation(AssociatedObject) == null) {
				ScrollBarExtensions.SetScrollViewerOrientation(AssociatedObject, Orientation);
			}
		}
		void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
			if (ScrollBarExtensions.GetScrollBarMode(AssociatedObject) == ScrollBarMode.Standard)
				return;
			ScrollBarExtensions.SetScrollViewerMouseMoved(AssociatedObject, true);
			StartUpdateTimer();
		}
		void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e) {
			if (ScrollBarExtensions.GetScrollBarMode(AssociatedObject) == ScrollBarMode.Standard)
				return;
			ScrollBarExtensions.SetScrollViewerSizeChanged(AssociatedObject, true);
			StartUpdateTimer();
		}
#if DEBUGTEST
		public static int ScrollChangedCount = 0;
#endif
		void AssociatedObject_ScrollChanged(object sender, EventArgs e) {
			if (ScrollBarExtensions.GetScrollBarMode(AssociatedObject) == ScrollBarMode.Standard)
				return;
			ScrollBarExtensions.SetScrollViewerMouseMoved(AssociatedObject, true);
#if DEBUGTEST
			ScrollChangedCount++;
#endif
			StartUpdateTimer();
		}
		class ScrollableObjectBase {
			protected FrameworkElement AssociatedObject { get; private set; }
			public void Attach(FrameworkElement associatedObject) {
				AssociatedObject = associatedObject;
				OnAttach();
			}
			public void Detach() {
				OnDetach();
				AssociatedObject = null;
			}
			protected virtual void OnAttach() { }
			protected virtual void OnDetach() { }
			public event EventHandler ScrollChanged;
			protected void RaiseScrollChanged() {
				if (ScrollChanged != null) ScrollChanged(this, EventArgs.Empty);
			}
		}
		class ScrollBarObject : ScrollableObjectBase {
			IScrollBarUpdated sourceObject;
			protected override void OnAttach() {
				base.OnAttach();
				sourceObject = AssociatedObject as IScrollBarUpdated;
				if (sourceObject == null)
					return;
				sourceObject.ScrollBarUpdated += PanelBase_ScrollChanged;
			}
			protected override void OnDetach() {
				if (sourceObject == null)
					return;
				sourceObject.ScrollBarUpdated -= PanelBase_ScrollChanged;
				base.OnDetach();
			}
			void PanelBase_ScrollChanged(object sender, EventArgs e) {
				RaiseScrollChanged();
			}
		}
		class ScrollViewerScrollableObject : ScrollableObjectBase {
			ScrollViewer scrollViewer;
			protected override void OnAttach() {
				base.OnAttach();
				scrollViewer = AssociatedObject as ScrollViewer;
				if (scrollViewer != null) scrollViewer.ScrollChanged += OnScrollChanged;
			}
			protected override void OnDetach() {
				if (scrollViewer != null) scrollViewer.ScrollChanged -= OnScrollChanged;
				base.OnDetach();
			}
			void OnScrollChanged(object sender, ScrollChangedEventArgs e) {
				if (e.OriginalSource == AssociatedObject)
					RaiseScrollChanged();
			}
		}
		class ScrollableObjectFactory {
			delegate ScrollableObjectBase CreateInstance();
			IDictionary<Type, CreateInstance> initializers;
			IDictionary<Type, CreateInstance> Initializers {
				get { return initializers; }
			}
			public ScrollableObjectFactory() {
				initializers = new Dictionary<Type, CreateInstance>();
				InitializeFactory();
			}
			void InitializeFactory() {
				Initializers[typeof(ScrollViewer)] = () => new ScrollViewerScrollableObject();
				Initializers[typeof(IScrollBarUpdated)] = () => new ScrollBarObject();
			}
			Type GetInitializerType(IDictionary<Type, CreateInstance> initializers, Type elementType) {
				if (elementType.GetInterfaces().Contains(typeof(IScrollBarUpdated)))
					return typeof(IScrollBarUpdated);
				return initializers.Keys.FirstOrDefault(x => x.IsAssignableFrom(elementType));
			}
			public ScrollableObjectBase Resolve(object associatedObject) {
				if (associatedObject != null) {
					CreateInstance createInstance = null;
					Type type = GetInitializerType(initializers, associatedObject.GetType());
					if (type != null && initializers.TryGetValue(type, out createInstance))
						return createInstance();
				}
				return new ScrollableObjectBase();
			}
		}
	}
	public interface IScrollBarUpdated {
		event EventHandler ScrollBarUpdated;
	}
	public class VerticalScrollBarRowSpanConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (!(values[0] is ScrollBarMode) || !(values[1] is Orientation?))
				return 1;
			if ((ScrollBarMode)values[0] != ScrollBarMode.TouchOverlap || (Orientation?)values[1] == null)
				return 1;
			return ((Orientation?)values[1]).Value == Orientation.Vertical ? 2 : 1;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class HorizontalScrollBarRowSpanConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (!(values[0] is ScrollBarMode) || !(values[1] is Orientation?))
				return 1;
			if ((ScrollBarMode)values[0] != ScrollBarMode.TouchOverlap || (Orientation?)values[1] == null)
				return 1;
			return ((Orientation?)values[1]).Value == Orientation.Horizontal ? 2 : 1;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public interface IScrollInfoEx {
		void SetVerticalOffset(double offset);
		void SetHorizontalOffset(double offset);
	}
	public interface IScrollBarThumbDragDeltaListener {
		Orientation Orientation { get; }
		ScrollBar ScrollBar { get; set; }
		void OnScrollBarThumbDragDelta(DragDeltaEventArgs e);
		void OnScrollBarThumbMouseMove(MouseEventArgs e);
	}
	public class ScrollBarThumbDragDeltaBehavior : Behavior<FrameworkElement> {
		IScrollBarThumbDragDeltaListener Listener { get; set; }
		Thumb ScrollBarThumb { get { return AssociatedObject as Thumb; } }
		ScrollBar ScrollBar { get; set; }
		protected override void OnAttached() {
			AssociatedObject.Loaded += AssociatedObject_Loaded;
			if (ScrollBarThumb != null) {
				ScrollBarThumb.DragDelta += ScrollBarThumbDragDelta;
				ScrollBarThumb.MouseMove += ScrollBarThumbMouseMove;
			}
			base.OnAttached();
		}
		void ScrollBarThumbMouseMove(object sender, MouseEventArgs e) {
			if (Listener != null)
				Listener.OnScrollBarThumbMouseMove(e);
		}
		void AssociatedObject_Loaded(object sender, RoutedEventArgs e) {
			ScrollBar = FindScrollBar();
			CreateListener();
		}
		ScrollBar FindScrollBar() {
			return LayoutHelper.FindLayoutOrVisualParentObject<ScrollBar>(ScrollBarThumb);
		}
		void ScrollBarThumbDragDelta(object sender, DragDeltaEventArgs e) {
			if (Listener != null && Listener.Orientation == ScrollBar.Orientation)
				Listener.OnScrollBarThumbDragDelta(e);
		}
		private void CreateListener() {
			if (ScrollBar != null) {
				var listener =
					LayoutHelper.FindLayoutOrVisualParentObject(ScrollBar, x => (bool)x.GetValue(ScrollBarExtensions.IsScrollBarThumbDragDeltaListenerProperty) == true);
				Listener = listener as IScrollBarThumbDragDeltaListener;
				if (Listener != null && ScrollBar.Orientation == Listener.Orientation)
					Listener.ScrollBar = ScrollBar;
			}
		}
		protected override void OnDetaching() {
			AssociatedObject.Loaded -= AssociatedObject_Loaded;
			if (ScrollBarThumb != null) {
				ScrollBarThumb.DragDelta -= ScrollBarThumbDragDelta;
				ScrollBarThumb.MouseMove -= ScrollBarThumbMouseMove;
			}
			base.OnDetaching();
		}
	}
}
