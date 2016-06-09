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
using DevExpress.Xpf.Docking.Platform.Win32;
using DevExpress.Xpf.Docking.UIAutomation;
using DevExpress.Xpf.Docking.VisualElements;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using LK = DevExpress.Xpf.Docking.Platform.FloatingWindowLock.LockerKey;
namespace DevExpress.Xpf.Docking.Platform {
	public class FloatingPaneWindow : WindowContentHolder, IDisposable, IAdornerWindowClient, IFloatingPane, IDraggableWindow {
		#region static
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty CanMaximizeProperty =
			DependencyProperty.Register("CanMaximize", typeof(bool), typeof(FloatingPaneWindow), new PropertyMetadata(true, OnCanMaximizeChanged));
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty FloatingMaxSizeProperty =
			DependencyProperty.Register("FloatingMaxSize", typeof(Size), typeof(FloatingPaneWindow), new PropertyMetadata(new Size(), new PropertyChangedCallback(OnFloatingMaxSizeChanged)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty FloatingMinSizeProperty =
			DependencyProperty.Register("FloatingMinSize", typeof(Size), typeof(FloatingPaneWindow), new PropertyMetadata(new Size(), new PropertyChangedCallback(OnFloatingMinSizeChanged)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty AllowAeroSnapProperty =
			DependencyProperty.Register("AllowAeroSnap", typeof(bool), typeof(FloatingPaneWindow), new PropertyMetadata(false, OnAllowAeroSnapChanged));
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty IsMaximizedProperty =
			DependencyProperty.Register("IsMaximized", typeof(bool), typeof(FloatingPaneWindow), new PropertyMetadata(false, new PropertyChangedCallback(OnIsMaximizedChanged)));
		static FloatingPaneWindow() {
			Window.WindowStateProperty.AddOwner(typeof(FloatingPaneWindow), new FrameworkPropertyMetadata(WindowState.Normal, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, FloatingPaneWindow.OnWindowStateChanged));
		}
		private static void OnCanMaximizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			UpdateSystemCommands((FloatingPaneWindow)d);
		}
		private static void OnFloatingMaxSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FloatingPaneWindow floatingPaneWindow = d as FloatingPaneWindow;
			if(floatingPaneWindow != null)
				floatingPaneWindow.OnFloatingMaxSizeChanged((Size)e.OldValue, (Size)e.NewValue);
		}
		private static void OnFloatingMinSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FloatingPaneWindow floatingPaneWindow = d as FloatingPaneWindow;
			if(floatingPaneWindow != null)
				floatingPaneWindow.OnFloatingMinSizeChanged((Size)e.OldValue, (Size)e.NewValue);
		}
		private static void OnAllowAeroSnapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FloatingPaneWindow floatingPaneWindow = d as FloatingPaneWindow;
			if(floatingPaneWindow != null)
				floatingPaneWindow.OnAllowAeroSnapChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		private static void OnIsMaximizedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			FloatingPaneWindow floatingPaneWindow = o as FloatingPaneWindow;
			if(floatingPaneWindow != null)
				floatingPaneWindow.OnIsMaximizedChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		private static void OnWindowStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FloatingPaneWindow floatingPaneWindow = d as FloatingPaneWindow;
			if(floatingPaneWindow != null)
				floatingPaneWindow.OnWindowStateChanged((WindowState)e.OldValue, (WindowState)e.NewValue);
		}
		static void UpdateSystemCommands(FloatingPaneWindow window) {
			IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
			var currentStyle = NativeHelper.GetWindowLongSafe(hwnd, NativeHelper.GWL_STYLE);
			if(window.CanMaximize)
				currentStyle |= (int)WS.WS_MAXIMIZEBOX;
			else currentStyle &= ~(int)WS.WS_MAXIMIZEBOX;
			currentStyle &= ~(int)WS.WS_MINIMIZEBOX;
			NativeHelper.SetWindowLongSafe(hwnd, NativeHelper.GWL_STYLE, currentStyle);
		}
		static bool CheckClosedKeyGesture() {
			return (Keyboard.IsKeyDown(Key.F4) && Keyboard2.IsAltPressed);
		}
		static bool CheckEscapeKeyGesture() {
			return (Keyboard.IsKeyDown(Key.Escape));
		}
		static bool CheckCommand(int command, int match) {
			return command == match;
		}
		static int GetInt(IntPtr ptr) {
			return (IntPtr.Size == 8) ? (int)ptr.ToInt64() : ptr.ToInt32();
		}
		#endregion
		#region properties
		bool CanMaximize {
			get { return (bool)GetValue(CanMaximizeProperty); }
			set { SetValue(CanMaximizeProperty, value); }
		}
		internal bool AllowAeroSnap {
			get { return (bool)GetValue(AllowAeroSnapProperty); }
			set { SetValue(AllowAeroSnapProperty, value); }
		}
		bool IsMaximized {
			get { return (bool)GetValue(IsMaximizedProperty); }
			set { SetValue(IsMaximizedProperty, value); }
		}
		bool AllowNativeDragging {
			get { return Manager.EnableNativeDragging; }
		}
		bool AllowNativeSizing {
			get { return AllowNativeDragging && Manager.RedrawContentWhenResizing; }
		}
		protected internal Rect WindowRect {
			get { return GetWindowRect(); }
		}
		FloatingWindowLock floatingWindowState;
		internal FloatingWindowLock LockHelper {
			get {
				if(floatingWindowState == null) floatingWindowState = new FloatingWindowLock();
				return floatingWindowState;
			}
		}
		protected internal FloatGroup FloatGroup { get; private set; }
		private Win32DragService Win32DragService {
			get {
				return Manager.Win32DragService;
			}
		}
		public Rect Bounds {
			get { return WindowState == System.Windows.WindowState.Maximized ? lastFloatingBounds : new Rect(Left, Top, Width, Height); }
		}
		bool IsAutoSize { get { return SizeToContent != SizeToContent.Manual; } }
		public DockLayoutManager Manager { get; private set; }
		#endregion
		bool isDisposingCore;
		internal bool hiddenInitialization;
		MatrixTransform transform;
		public FloatingPaneWindow(BaseFloatingContainer container)
			: base(container) {
			hiddenInitialization = true;
			WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
			FloatGroup = DockLayoutManager.GetLayoutItem(container) as FloatGroup;
			Manager = DockLayoutManager.GetDockLayoutManager(container);
			Manager.SizeChanged += OnManagerSizeChanged;
			WindowHelper.BindFlowDirection(this, Manager);
			AllowsTransparency = !Manager.IsTransparencyDisabled;
			Loaded += OnLoaded;
			Activated += OnActivated;
			LocationChanged += OnLocationChanged;
			SizeChanged += OnSizeChanged;
			PreviewGotKeyboardFocus += OnPreviewGotKeyboardFocus;
			SetBinding(AllowAeroSnapProperty, new System.Windows.Data.Binding("AllowAeroSnap") { Source = Manager });
			SetBinding(FloatingMaxSizeProperty, new System.Windows.Data.Binding("ActualMaxSize") { Source = FloatGroup });
			SetBinding(FloatingMinSizeProperty, new System.Windows.Data.Binding("ActualMinSize") { Source = FloatGroup });
			SetBinding(CanMaximizeProperty, new System.Windows.Data.Binding("CanMaximize") { Source = FloatGroup });
			SetBinding(IsMaximizedProperty, new System.Windows.Data.Binding("IsMaximized") { Source = FloatGroup });
			FloatGroup.LayoutChanged += OnFloatGroupLayoutChanged;
			DevExpress.Xpf.Docking.Platform.Shell.WindowChrome.SetWindowChrome(this, new DevExpress.Xpf.Docking.Platform.Shell.WindowChrome());
		}
		void OnPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
			if(e.OriginalSource == sender) {
				var focused = FocusManager.GetFocusedElement(FloatGroup);
				if(focused != null && focused != sender && focused.Focusable) {
					IInputElement oldFocused = Keyboard.FocusedElement;
					Visual focusedAsVisual = focused as Visual;
					if(focusedAsVisual != null && !focusedAsVisual.IsDescendantOf(this)) {
						FocusManager.SetFocusedElement(FloatGroup, null);
						return;
					}
					focused.Focus();
					if(Keyboard.FocusedElement == focused || Keyboard.FocusedElement != oldFocused) {
						e.Handled = true;
						return;
					}
				}
			}
		}
		void OnFloatGroupLayoutChanged(object sender, EventArgs e) {
			LayoutUpdated -= OnFloatGroupLayoutChangedLayoutUpdated;
			LayoutUpdated += OnFloatGroupLayoutChangedLayoutUpdated;
		}
		void OnWindowStateChanged(WindowState oldValue, WindowState newValue) {
			this._previousWindowState = newValue;
			bool isMaximized = newValue == WindowState.Maximized;
			if(isMaximized) {
				if(!FloatGroup.IsMaximized) {
					try {
						if(Win32DragService.IsDragging) LockHelper.Lock(LK.Maximize);
						MaximizeFloatGroup();
						Rect restoreBounds = TransformToRelativeBounds(RestoreBounds);
						DocumentPanel.SetRestoreBounds(FloatGroup, restoreBounds);
					}
					finally {
						LockHelper.Unlock(LK.Maximize);
					}
				}
				else {
					if(!Win32DragService.IsDragging) {
						var restoreBounds = TransformFromRelativeBounds(DocumentPanel.GetRestoreBounds(FloatGroup));
						if(restoreBounds.Width == 0 || restoreBounds.Height == 0) return;
						Left = restoreBounds.Left; Top = restoreBounds.Top; Width = restoreBounds.Width; Height = restoreBounds.Height;
					}
				}
			}
			else {
				if(FloatGroup.IsMaximized && !Win32DragService.IsInEvent) {
					RestoreFloatGroup();
				}
			}
			if(LockHelper.IsLocked(LK.ParentOpening)) {
				Dispatcher.BeginInvoke(new Action(() => {
					LockHelper.Unlock(LK.ParentOpening);
					WindowState = WindowState.Maximized;
				}));
			}
		}
		void CorrectRestoreBounds() {
			Rect restoreBounds = lastRestoreBounds;
			if(LockHelper.IsLocked(LK.RestoreBounds) || restoreBounds == Rect.Empty) return;
			LockHelper.Lock(LK.RestoreBounds);
			Left = restoreBounds.Left; Top = restoreBounds.Top; Width = restoreBounds.Width; Height = restoreBounds.Height;
			lastRestoreBounds = Rect.Empty;
			LockHelper.Unlock(LK.RestoreBounds);
		}
		private void UpdateMinMax() {
			var window = this;
			window.ClearValue(MinHeightProperty);
			window.ClearValue(MinWidthProperty);
			window.ClearValue(MaxHeightProperty);
			window.ClearValue(MaxWidthProperty);
			if(window.AllowAeroSnap) {
				Size min = LayoutItemsHelper.GetResizingMinSize(window.FloatGroup);
				Size max = LayoutItemsHelper.GetResizingMaxSize(window.FloatGroup);
				if(!double.IsNaN(min.Height))
					window.MinHeight = min.Height;
				if(!double.IsNaN(min.Width))
					window.MinWidth = min.Width;
				if(!double.IsNaN(max.Height))
					window.MaxHeight = max.Height;
				if(!double.IsNaN(max.Width))
					window.MaxWidth = max.Width;
			}
		}
		protected virtual void OnFloatingMaxSizeChanged(Size oldValue, Size newValue) {
			UpdateMinMax();
		}
		protected virtual void OnFloatingMinSizeChanged(Size oldValue, Size newValue) {
			UpdateMinMax();
		}
		private void UpdateResizingMode() {
			ResizeMode = AllowAeroSnap && AllowNativeDragging ? ResizeMode.CanResize : ResizeMode.NoResize;
		}
		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			if(!AllowsTransparency)
				drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Black, 1), new Rect(0, 0, Width, Height));
		}
		Rect lastRestoreBounds = Rect.Empty;
		Locker changeWindowStateLocker = new Locker();
		private void ChangeWindowState(bool isMaximized) {
			changeWindowStateLocker.Unlock();
			if(changeWindowStateLocker) return;
			ChangeWindowStateCore(isMaximized);
		}
		private void ChangeWindowStateCore(bool isMaximized) {
			if(isMaximized && WindowState != WindowState.Maximized) {
				WindowState = System.Windows.WindowState.Maximized;
			}
			if(!isMaximized && WindowState == WindowState.Maximized) {
				lastRestoreBounds = lastFloatingBounds;
				WindowState = System.Windows.WindowState.Normal;
			}
		}
		internal bool MaximizeOrRestore(bool maximize) {
			bool canChangeState = maximize ? CanMaximize : true;
			if(canChangeState) {
				DisableSizeToContent();
				WindowState = maximize ? WindowState.Maximized : WindowState.Normal;
			}
			return canChangeState;
		}
		System.Windows.Threading.DispatcherOperation changeWindowStateOperation;
		private void EnsureWindowState(bool isMaximized) {
			changeWindowStateLocker.Lock();
			Action<bool> changeStateAction = new Action<bool>(ChangeWindowState);
			if(OwnerWindow is DXWindow) changeWindowStateOperation = Dispatcher.BeginInvoke(changeStateAction, isMaximized);
			else changeStateAction(isMaximized);
		}
		protected virtual void OnIsMaximizedChanged(bool oldValue, bool newValue) {
			if(!IsLoaded) return;
			EnsureWindowState(newValue);
		}
		protected virtual void OnAllowAeroSnapChanged(bool oldValue, bool newValue) {
			UpdateResizingMode();
			UpdateMinMax();
		}
		void OnFloatGroupLayoutChangedLayoutUpdated(object sender, EventArgs e) {
			LayoutUpdated -= OnFloatGroupLayoutChangedLayoutUpdated;
			UnsubcribeDragWidget();
			SubscribeDragWidget();
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			bool canChangeSize = !Manager.IsThemeChangedLocked && (Win32DragService.IsResizing || (!correctBoundsRequested && !Win32DragService.IsInEvent && !LockHelper.IsLocked(LK.NativeBounds) && (!LockHelper.IsLocked(LK.FloatingBounds)) || IsAutoSize));
			if(canChangeSize) {
				if(IsAutoSize) Dispatcher.BeginInvoke(new Action(EnsureRelativeSize));
				else EnsureRelativeSize();
			}
		}
		protected virtual void OnLocationChanged(object sender, EventArgs e) {
			bool canChangeLocation = fBoundsChangeRequested || (!FloatGroup.IsMaximized && !Win32DragService.IsInEvent && !LockHelper.IsLocked(LK.FloatingBounds) && !LockHelper.IsLocked(LK.NativeBounds));
			if(canChangeLocation) {
				EnusreRelativeLocation();
			}
		}
		bool fBoundsChangeRequested;
		internal void UpdateFloatGroupBounds() {
			fBoundsChangeRequested = true;
		}
		private void EnusreRelativeLocation() {
			Rect rect = TransformToRelativeBounds(WindowRect);
			LockHelper.Lock(LK.FloatingBounds);
			if(Win32DragService.IsDragging)
				Manager.RaiseDockItemDraggingEvent(FloatGroup, rect.Location);
			FloatGroup.FloatLocation = rect.Location;
			LockHelper.Unlock(LK.FloatingBounds);
		}
		private void EnsureRelativeSize() {
			Rect windowRect = WindowRect;
			PresentationSource pSource = PresentationSource.FromDependencyObject(Container.Owner);
			if(pSource != null) windowRect.Transform(pSource.CompositionTarget.TransformFromDevice);
			Rect rect = TransformToRelativeBounds(windowRect);
			LockHelper.Lock(LK.FloatingBounds);
			FloatGroup.FloatSize = rect.Size;
			LockHelper.Unlock(LK.FloatingBounds);
		}
		void OnActivated(object sender, EventArgs e) {
			Activated -= OnActivated;
			if(!Manager.IsFloating || !AllowNativeDragging) return;
			Dispatcher.BeginInvoke(new Action(() => {
				TryStartDragging(true);
			}));
		}
		WeakReference dragWidgetRef;
		private void SubscribeDragWidget() {
			FrameworkElement dragWidget = LayoutHelper.FindElement(this, (x => x is FloatingDragWidget && x.IsVisible));
			if(dragWidget == null) dragWidget = LayoutHelper.FindElement(this, (x) => DockPane.GetHitTestType(x) == Base.HitTestType.Header);
			if(dragWidget != null) {
				dragWidget.PreviewMouseDown += OnDragWidgetPreviewMouseDown;
				dragWidgetRef = new WeakReference(dragWidget);
			}
		}
		protected override void OnPreviewMouseMove(MouseEventArgs e) {
			base.OnPreviewMouseMove(e);
			if(AllowNativeSizing)
				TryStartSizing();
		}
		private void UnsubcribeDragWidget() {
			if(dragWidgetRef != null) {
				var dragWidget = dragWidgetRef.Target as FrameworkElement;
				if(dragWidget != null) dragWidget.PreviewMouseDown -= OnDragWidgetPreviewMouseDown;
			}
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			Dispatcher.BeginInvoke(new Action(() => {
				UnsubcribeDragWidget();
				SubscribeDragWidget();
			}));
			EnsureWindowState(IsMaximized);
		}
		void OnDragWidgetPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if(!AllowNativeDragging) return;
			if(e.ChangedButton == MouseButton.Left && (HitTestHelper.IsDraggable(e.OriginalSource as DependencyObject)))
				if(TryStartDragging()) e.Handled = true;
		}
		private bool TryStartDragging(bool isFloating = false) {
			return Win32DragService.TryStartDragging(this, isFloating);
		}
		private void TryStartSizing() {
			if(Win32DragService.TryStartSizing(this))
				EnsureRelativeSize();
		}
		private void DoMoving() {
			Win32DragService.DoDragging();
		}
		private void DoSizing() {
			if(!Win32DragService.IsResizing) return;
			Win32DragService.DoSizing();
			EnsureRelativeSize();
		}
		private void FinishDragging() {
			if(Win32DragService.IsResizing) {
				EnsureRelativeSize();
			}
			Win32DragService.FinishDragging();
		}
		Rect TransformToRelativeBounds(Rect rect) {
			FrameworkElement owner = Container.Owner;
			if(owner != null) {
				if(DevExpress.Xpf.Core.Native.ScreenHelper.IsAttachedToPresentationSource(owner)) {
					Point relativeLocation = owner.PointFromScreen(rect.Location);
					rect.Location = relativeLocation;
				}
			}
			return rect;
		}
		Rect TransformFromRelativeBounds(Rect rect) {
			FrameworkElement owner = Container.Owner;
			if(owner != null) {
				if(DevExpress.Xpf.Core.Native.ScreenHelper.IsAttachedToPresentationSource(owner)) {
					Point screenLocation = owner.PointToScreen(rect.Location);
					rect.Location = screenLocation;
				}
			}
			return rect;
		}
		Window OwnerWindow = null;
		protected override void TrySetOwnerCore(Window containerWindow) {
			if(Manager.OwnsFloatWindows)
				base.TrySetOwnerCore(containerWindow);
			if(OwnerWindow == null) OwnerWindow = containerWindow;
		}
		internal void UpdateOwnerWindow() {
			Window containerWindow = DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<Window>(Manager);
			if(OwnerWindow != null)
				UnSubscribe(OwnerWindow);
			OwnerWindow = containerWindow;
			if(Manager.OwnsFloatWindows)
				Owner = OwnerWindow;
			if(OwnerWindow != null)
				Subscribe(OwnerWindow);
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			OnPreviewKey(this, e);
		}
		protected override void OnPreviewKeyUp(KeyEventArgs e) {
			base.OnPreviewKeyUp(e);
			OnPreviewKey(this, e);
		}
		bool EnqueueUpdateBounds(Rect bounds) {
			LockHelper.LockHelperDelegate lockerDelegate = new LockHelper.LockHelperDelegate(() => {
				UpdateBoundsNative(bounds);
			});
			LockHelper themeChangingLocker = LockHelper.GetLocker(LK.ThemeChanging);
			if(themeChangingLocker) themeChangingLocker.Reset();
			if(Manager.IsThemeChangedLocked) {
				themeChangingLocker.Lock();
				themeChangingLocker.DoWhenUnlocked(lockerDelegate);
				Dispatcher.BeginInvoke(new Action(() => themeChangingLocker.Unlock()), System.Windows.Threading.DispatcherPriority.Render);
				return true;
			}
			LockHelper checkBoundsLocker = LockHelper.GetLocker(LK.CheckFloatBounds);
			if(checkBoundsLocker) checkBoundsLocker.Reset();
			if(checkBoundsLocker) {
				checkBoundsLocker.DoWhenUnlocked(lockerDelegate);
				return true;
			}
			return false;
		}
		Rect lastFloatingBounds;
		public void UpdateBoundsNative(Rect bounds) {
			if(EnqueueUpdateBounds(bounds)) return;
			lastFloatingBounds = bounds;
			if(Manager.IsThemeChangedLocked || Manager.SerializationController.IsDeserializing) return;
			if(LockHelper.IsLocked(LK.WindowState) || (!LockHelper.IsLocked(LK.Maximize) && Win32DragService.IsInEvent) || LockHelper.IsLocked(LK.FloatingBounds)) return;
			if(IsAutoSize) return;
			if(!AllowsTransparency && FloatGroup.SizeToContent != SizeToContent.Manual) {
				Size newSize = SizeToContentHelper.FitSizeToContent(FloatGroup.SizeToContent, new Size(RenderSize.Width, RenderSize.Height), bounds.Size);
				bounds.Size = newSize;
			}
			try {
				LockHelper.Lock(LK.NativeBounds);
				const int flags = 0X0004 | 0x0040 | 0x0010;
				var source = System.Windows.Interop.HwndSource.FromVisual(this) as System.Windows.Interop.HwndSource;
				if(source != null) {
					bounds.Transform(PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice);
					NativeHelper.SetWindowPosSafe(source.Handle, IntPtr.Zero, (int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height, flags);
				}
			}
			finally {
				LockHelper.Unlock(LK.NativeBounds);
			}
		}
		protected override void SetBounds(Rect bounds) {
			if(firstCheck) {
				CheckTransform();
				BindingHelper.SetBinding(this, ShowInTaskbarProperty, Manager, "ShowFloatWindowsInTaskbar");
				Dispatcher.BeginInvoke(new Action(() => {
					SetBinding(SizeToContentProperty, new System.Windows.Data.Binding("SizeToContent") { Source = FloatGroup, Mode = System.Windows.Data.BindingMode.TwoWay });
				}));
				firstCheck = false;
			}
			bounds = CheckBoundsWithTransform(bounds);
			if(DevExpress.Xpf.Core.Native.ScreenHelper.IsAttachedToPresentationSource(this)) UpdateBoundsNative(bounds);
			else base.SetBounds(bounds);
		}
		Rect CheckBoundsWithTransform(Rect bounds) {
			if(transform != null && !transform.Matrix.IsIdentity) {
				Point sz = transform.Transform(new Point(bounds.Width, bounds.Height));
				bounds.Width = sz.X;
				bounds.Height = sz.Y;
			}
			return bounds;
		}
		bool firstCheck = true;
		void CheckTransform() {
			FrameworkElement owner = OwnerWindow;
			if(owner == null || !Manager.IsDescendantOf(owner)) {
				owner = DevExpress.Xpf.Core.Native.LayoutHelper.GetTopLevelVisual(Manager);
			}
			if(owner != null) {
				transform = Manager.TransformToVisual(owner) as MatrixTransform;
				if(transform != null && !transform.Matrix.IsIdentity) {
					Matrix matrix = transform.Matrix;
					transform = new MatrixTransform(Math.Abs(matrix.M11), matrix.M12, matrix.M21, Math.Abs(matrix.M22), 0, 0);
					if(!transform.Matrix.IsIdentity)
						((FrameworkElement)Content).LayoutTransform = transform;
				}
			}
		}
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			base.OnClosing(e);
			if(!e.Cancel && CheckClosedKeyGesture() && !isDisposingCore) {
				if(OwnerWindow != null) {
					Dispatcher.BeginInvoke(new System.Action(OwnerWindow.Close));
				}
				e.Cancel = true;
			}
		}
		void OnPreviewKey(object sender, System.Windows.Input.KeyEventArgs e) {
			if(Manager != null) {
				Manager.RaiseEvent(e);
			}
		}
		LockHelper sizeChangedLockHelper = new LockHelper();
		void OnManagerSizeChanged(object sender, SizeChangedEventArgs e) {
			if(sizeChangedLockHelper.IsLocked) sizeChangedLockHelper.Reset();
			if(Manager.IsThemeChangedLocked) {
				sizeChangedLockHelper.Lock();
				sizeChangedLockHelper.DoWhenUnlocked(new LockHelper.LockHelperDelegate(() => {
					TryCheckRelativeLocationAsync(sender);
				}));
				Dispatcher.BeginInvoke(new Action(() => sizeChangedLockHelper.Unlock()), System.Windows.Threading.DispatcherPriority.Render);
				return;
			}
			if((Owner is DXWindow || Owner is FloatingPaneWindow))
				TryCheckRelativeLocationAsyncLocker.LockOnce();
			TryCheckRelativeLocationAsync(sender);
		}
		void IDisposable.Dispose() {
			if(!isDisposingCore) {
				isDisposingCore = true;
				OnDisposing();
			}
			GC.SuppressFinalize(this);
		}
		protected void OnDisposing() {
			checkRelativeLocationOnLayoutUpdatedOperation.Do(x => x.Abort());
			changeWindowStateOperation.Do(x => x.Abort());
			tryCorrectBoundsAsyncOperation.Do(x => x.Abort());
			tryCheckRelativeLocationAsyncOperation.Do(x => x.Abort());
			sizeChangedLockHelper.Reset();
			UnsubcribeDragWidget();
			if(OwnerWindow != null) {
				UnSubscribe(OwnerWindow);
				OwnerWindow = null;
			}
			LockHelper.GetLocker(LK.ThemeChanging).Reset();
			LockHelper.GetLocker(LK.CheckFloatBounds).Reset();
			Close();
			LayoutUpdated -= OnFloatGroupLayoutChangedLayoutUpdated;
			FloatGroup.LayoutChanged -= OnFloatGroupLayoutChanged;
			Manager.SizeChanged -= OnManagerSizeChanged;
			ClearValue(AllowAeroSnapProperty);
			ClearValue(FloatingMaxSizeProperty);
			ClearValue(FloatingMinSizeProperty);
			ClearValue(CanMaximizeProperty);
		}
		protected override Point GetRelativeLocation(FrameworkElement owner) {
			PresentationSource pSource = PresentationSource.FromDependencyObject(owner);
			PresentationSource thisSource = PresentationSource.FromDependencyObject(this);
			if(pSource != null) {
				Point windowLocation = WindowState == System.Windows.WindowState.Maximized && thisSource != null ? this.PointToScreen(new Point()) : new Point(Left, Top);
				Point location = pSource.CompositionTarget.TransformToDevice.Transform(windowLocation);
				if(double.IsNaN(location.X) || double.IsNaN(location.Y))
					return PointHelper.Empty;
				Point screenLocation = owner.PointFromScreen(location);
				if(FlowDirection == FlowDirection.RightToLeft)
					screenLocation.X -= ActualWidth;
				return screenLocation;
			}
			return base.GetRelativeLocation(owner);
		}
		protected override void EnsureRelativeLocationCore(Point floatLocation) {
			if(floatLocation == PointHelper.Empty) return;
			FloatGroup.EnsureFloatLocation(floatLocation);
		}
		System.Windows.Threading.DispatcherOperation tryCheckRelativeLocationAsyncOperation;
		Locker TryCheckRelativeLocationAsyncLocker = new Locker();
		protected override void TryCheckRelativeLocationAsync(object sender) {
			if(Owner is DXWindow || Owner is FloatingPaneWindow) {
				tryCheckRelativeLocationAsyncOperation.Do(x => x.Abort());
				tryCheckRelativeLocationAsyncOperation = Dispatcher.BeginInvoke(
					new System.Action(() => {
						OwnerWindow.LayoutUpdated -= CheckRelativeLocationOnLayoutUpdated;
						OwnerWindow.InvalidateVisual();
						OwnerWindow.LayoutUpdated += CheckRelativeLocationOnLayoutUpdated;
					}),
					System.Windows.Threading.DispatcherPriority.Render);
			}
			else base.TryCheckRelativeLocationAsync(sender);
		}
		System.Windows.Threading.DispatcherOperation checkRelativeLocationOnLayoutUpdatedOperation;
		void CheckRelativeLocationOnLayoutUpdated(object sender, System.EventArgs e) {
			if(OwnerWindow == null) return;
			OwnerWindow.LayoutUpdated -= CheckRelativeLocationOnLayoutUpdated;
			checkRelativeLocationOnLayoutUpdatedOperation = Dispatcher.BeginInvoke(
				new System.Action(() => {
					CheckRelativeLocation();
					TryCheckRelativeLocationAsyncLocker.Unlock();
				}), System.Windows.Threading.DispatcherPriority.Render);
		}
		System.Windows.Threading.DispatcherOperation tryCorrectBoundsAsyncOperation;
		protected override void TryCorrectBoundsAsync(FrameworkElement owner, Rect bounds) {
			if(TryCheckRelativeLocationAsyncLocker.IsLocked && Owner is FloatingPaneWindow)
				return;
			if(OwnerWindow is DXWindow || Owner is FloatingPaneWindow) {
				correctBoundsRequested = true;
				if(fFirstRun) {
					fFirstRun = !OwnerWindow.IsArrangeValid;
					savedOwner = owner;
					savedBounds = bounds;
					if(!OwnerWindow.IsArrangeValid || !Manager.IsMeasureValid) {
						tryCorrectBoundsAsyncOperation = Dispatcher.BeginInvoke(
							new System.Action(() => {
								OwnerWindow.LayoutUpdated -= CorrectBoundsOnLayoutUpdated;
								if(!correctBoundsRequested) return;
								OwnerWindow.InvalidateVisual();
								OwnerWindow.LayoutUpdated += CorrectBoundsOnLayoutUpdated;
							}));
						return;
					}
				}
			}
			correctBoundsRequested = false;
			base.TryCorrectBoundsAsync(owner, bounds);
		}
		bool correctBoundsRequested;
		int lockCorrectBounds;
		void CorrectBoundsAction(FrameworkElement owner, Rect bounds) {
			if(lockCorrectBounds > 0 || !correctBoundsRequested) return;
			try {
				lockCorrectBounds++;
				correctBoundsRequested = false;
				CorrectBoundsCore(owner, bounds);
			}
			finally {
				lockCorrectBounds--;
			}
		}
		FrameworkElement savedOwner;
		Rect savedBounds;
		bool fFirstRun = true;
		void CorrectBoundsOnLayoutUpdated(object sender, System.EventArgs e) {
			OwnerWindow.LayoutUpdated -= CorrectBoundsOnLayoutUpdated;
			Dispatcher.BeginInvoke(
				new System.Action<FrameworkElement, Rect>(CorrectBoundsAction),
				new object[] { savedOwner, savedBounds });
		}
		protected override void Subscribe(Window ownerWindow) {
			if(ownerWindow == null) return;
			base.Subscribe(ownerWindow);
			InputBindings.AddRange(ownerWindow.InputBindings);
			CommandBindings.AddRange(ownerWindow.CommandBindings);
		}
		protected override void UnSubscribe(Window ownerWindow) {
			if(ownerWindow == null) ownerWindow = this.OwnerWindow;
			if(ownerWindow != null) {
				ownerWindow.LayoutUpdated -= CheckRelativeLocationOnLayoutUpdated;
				ownerWindow.LayoutUpdated -= CorrectBoundsOnLayoutUpdated;
			}
			base.UnSubscribe(ownerWindow);
			InputBindings.Clear();
			CommandBindings.Clear();
		}
		protected override void OnSourceInitialized(EventArgs e) {
			base.OnSourceInitialized(e);
			AddHwndSourceHooks();
			UpdateSystemCommands(this);
		}
		protected override void OnClosed(EventArgs e) {
			RemoveHwndSourceHooks();
			base.OnClosed(e);
		}
		internal void UpdateRenderOptions() {
			Dispatcher.BeginInvoke(new Action(() => {
				DevExpress.Xpf.Core.Native.VisualEffectsInheritanceHelper.SetTextAndRenderOptions(this, Manager);
			}), System.Windows.Threading.DispatcherPriority.Background);
		}
		private void MaximizeFloatGroup() {
			Manager.MDIController.Maximize(FloatGroup.IsMaximizable ? FloatGroup : FloatGroup[0]);
		}
		private void RestoreFloatGroup() {
			Manager.MDIController.Restore(FloatGroup.IsMaximizable ? FloatGroup : FloatGroup[0]);
		}
		#region WndProc
		System.Windows.Interop.HwndSourceHook sysCommandHook;
		System.Windows.Interop.HwndSourceHook nativeDraggingHook;
		void AddHwndSourceHooks() {
			var source = System.Windows.Interop.HwndSource.FromVisual(this) as System.Windows.Interop.HwndSource;
			if(source != null) {
				if(sysCommandHook == null) {
					sysCommandHook = new System.Windows.Interop.HwndSourceHook(WndProcSystemCommand);
					source.AddHook(sysCommandHook);
				}
				if(nativeDraggingHook == null) {
					nativeDraggingHook = new HwndSourceHook(WndProcNativeDragging);
					source.AddHook(nativeDraggingHook);
				}
			}
		}
		void RemoveHwndSourceHooks() {
			var source = System.Windows.Interop.HwndSource.FromVisual(this) as System.Windows.Interop.HwndSource;
			if(source != null) {
				if(sysCommandHook != null)
					source.RemoveHook(sysCommandHook);
				if(nativeDraggingHook != null)
					source.RemoveHook(nativeDraggingHook);
			}
			sysCommandHook = null;
			nativeDraggingHook = null;
		}
		IntPtr WndProcSystemCommand(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled) {
			switch((WM)message) {
				case WM.WM_SYSCOMMAND:
					int command = GetInt(wParam) & 0xFFF0;
					if(CheckCommand(command, SC.SC_CLOSE) && !CheckClosedKeyGesture()) {
						if(Manager.DockController.Close(FloatGroup))
							RemoveHwndSourceHooks();
						handled = true;
					}
					if(CheckCommand(command, SC.SC_RESTORE) && FloatGroup.IsMaximized) {
						RestoreFloatGroup();
						handled = true;
					}
					if(CheckCommand(command, SC.SC_MAXIMIZE) && !FloatGroup.IsMaximized) {
						MaximizeFloatGroup();
						handled = true;
					}
					break;
				case WM.WM_INITMENUPOPUP: {
						IntPtr hMenu = NativeHelper.GetSystemMenuSafe(hwnd, false);
						if(hMenu != IntPtr.Zero) {
							NativeHelper.EnableMenuItemSafe(hMenu, SC.SC_MINIMIZE, NativeHelper.MF_BYCOMMAND | NativeHelper.MF_GRAYED);
							bool isMaxmized = FloatGroup.IsMaximized;
							uint mfMaximize = !isMaxmized && CanMaximize ? NativeHelper.MF_ENABLED : NativeHelper.MF_GRAYED;
							uint mfRestore = isMaxmized ? NativeHelper.MF_ENABLED : NativeHelper.MF_GRAYED;
							uint mfSizeMove = AllowNativeDragging && !isMaxmized ? NativeHelper.MF_ENABLED : NativeHelper.MF_GRAYED;
							NativeHelper.EnableMenuItemSafe(hMenu, SC.SC_MAXIMIZE, NativeHelper.MF_BYCOMMAND | mfMaximize);
							NativeHelper.EnableMenuItemSafe(hMenu, SC.SC_RESTORE, NativeHelper.MF_BYCOMMAND | mfRestore);
							NativeHelper.EnableMenuItemSafe(hMenu, SC.SC_MOVE, NativeHelper.MF_BYCOMMAND | mfSizeMove);
							NativeHelper.EnableMenuItemSafe(hMenu, SC.SC_SIZE, NativeHelper.MF_BYCOMMAND | mfSizeMove);
						}
					}
					break;
			}
			return IntPtr.Zero;
		}
		[SecuritySafeCritical]
		IntPtr WndProcNativeDragging(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled) {
			switch((WM)message) {
				case WM.WM_CAPTURECHANGED:
					if(Win32DragService.IsInEvent && CheckEscapeKeyGesture()) {
						FloatingView view = Manager.GetView(FloatGroup) as FloatingView;
						view.OnKeyDown(Key.Escape);
					}
					break;
				case WM.WM_LBUTTONUP:
					if(Win32DragService.IsInEvent) {
						FinishDragging();
						handled = true;
					}
					break;
				case WM.WM_EXITSIZEMOVE: {
						FinishDragging();
						InvalidateVisual();
					}
					break;
				case WM.WM_SIZE:
					WmSizeChanged(wParam);
					break;
				case WM.WM_SIZING:
					DisableSizeToContent();
					if(Win32DragService.IsResizing) {
						DoSizing();
						handled = true;
					}
					break;
				case WM.WM_MOVING: {
						DoMoving();
					}
					break;
				case WM.WM_GETMINMAXINFO:
					if(!LockHelper.IsLocked(LK.NativeBounds)) {
						if(!Win32DragService.IsResizing) {
							NativeHelper.WmGetMinMaxInfo(hwnd, lParam);
							handled = true;
						}
					}
					break;
				case WM.WM_SHOWWINDOW:
					handled = WmShowWindow(wParam, lParam);
					break;
			}
			return IntPtr.Zero;
		}
		private bool WmShowWindow(IntPtr wParam, IntPtr lParam) {
			switch(NativeMethods.IntPtrToInt32(lParam)) {
				case NativeHelper.SW_PARENTCLOSING:
					break;
				case NativeHelper.SW_PARENTOPENING:
					ShowActivated = WindowState == System.Windows.WindowState.Maximized;
					LockHelper.Lock(LK.ParentOpening);
					break;
				default:
					break;
			}
			return false;
		}
		private System.Windows.WindowState _previousWindowState;
		private bool WmSizeChanged(IntPtr wParam) {
			switch(((int)wParam)) {
				case 0:
					if(this._previousWindowState != System.Windows.WindowState.Normal) {
						CorrectRestoreBounds();
						this._previousWindowState = System.Windows.WindowState.Normal;
					}
					break;
				case 1:
					if(this._previousWindowState != System.Windows.WindowState.Minimized) {
						this._previousWindowState = System.Windows.WindowState.Minimized;
					}
					break;
				case 2:
					if(this._previousWindowState != System.Windows.WindowState.Maximized) {
						this._previousWindowState = System.Windows.WindowState.Maximized;
					}
					break;
			}
			return false;
		}
		[SecuritySafeCritical]
		private Rect GetWindowRect() {
			var rect = new NativeMethods.RECT();
			if(interopHelperCore != null)
				NativeMethods.GetWindowRect(new HandleRef(this, interopHelperCore.Handle), ref rect);
			Rect windowRect = new Rect(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
			if(FlowDirection == FlowDirection.RightToLeft)
				windowRect.X += rect.Width;
			return windowRect;
		}
		internal void DisableSizeToContent() {
			if(IsAutoSize) SizeToContent = SizeToContent.Manual;
		}
		#endregion WndProc
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new FloatingPaneWindowAutomationPeer(this);
		}
		#endregion
		#region IFloatingPane Members
		DockLayoutManager IFloatingPane.Manager {
			get { return this.Manager; }
		}
		FloatGroup IFloatingPane.FloatGroup {
			get { return this.FloatGroup; }
		}
		#endregion
		#region IDraggableWindow Members
		DockLayoutManager IDraggableWindow.Manager {
			get { return Manager; }
		}
		FloatGroup IDraggableWindow.FloatGroup {
			get { return FloatGroup; }
		}
		Window IDraggableWindow.Window {
			get { return this; }
		}
		#endregion
	}
	class FloatingWindowLock {
		public enum LockerKey { WindowState, FloatingBounds, Maximize, NativeBounds, RestoreBounds, ParentOpening, CheckFloatBounds, ThemeChanging }
		Dictionary<LK, LockHelper> lockers = new Dictionary<LK, LockHelper>();
		public void Lock(LK key) {
			GetLocker(key).Lock();
		}
		public void Unlock(LK key) {
			GetLocker(key).Unlock();
		}
		public bool IsLocked(LK key) {
			return GetLocker(key);
		}
		public LockHelper GetLocker(LK key) {
			LockHelper locker = null;
			if(!lockers.TryGetValue(key, out locker)) {
				locker = new LockHelper();
				lockers[key] = locker;
			}
			return locker;
		}
	}
	public class FloatingPaneAdornerElement : Decorator, IFloatingPane, ISupportAutoSize {
		#region static
		static Size GetConstraintSize(SizeToContent sizeToContent, Size bounds) {
			Size patchedBounds = bounds;
			switch(sizeToContent) {
				case SizeToContent.Height:
					patchedBounds.Height = double.PositiveInfinity;
					break;
				case SizeToContent.Width:
					patchedBounds.Width = double.PositiveInfinity;
					break;
				case SizeToContent.WidthAndHeight:
					patchedBounds = SizeHelper.Infinite;
					break;
				case SizeToContent.Manual:
					break;
			}
			return patchedBounds;
		}
		static Size GetAutoSize(SizeToContent sizeToContent, Size bounds, Size desiredSize) {
			Size patchedBounds = bounds;
			switch(sizeToContent) {
				case SizeToContent.Height:
					patchedBounds.Height = desiredSize.Height;
					break;
				case SizeToContent.Width:
					patchedBounds.Width = desiredSize.Width;
					break;
				case SizeToContent.WidthAndHeight:
					patchedBounds = desiredSize;
					break;
				case SizeToContent.Manual:
					break;
			}
			return patchedBounds;
		}
		#endregion
		public FloatingPaneAdornerElement(BaseFloatingContainer container) {
			FloatGroup = DockLayoutManager.GetLayoutItem(container) as FloatGroup;
			Manager = DockLayoutManager.GetDockLayoutManager(container);
			WindowHelper.BindFlowDirection(this, Manager);
			UseLayoutRounding = true;
		}
		public DockLayoutManager Manager { get; private set; }
		public FloatGroup FloatGroup { get; private set; }
		public SizeToContent SizeToContent {
			get { return FloatGroup.Return(x => x.SizeToContent, () => SizeToContent.Manual); }
		}
		public void EnsureFlowDirection() {
			WindowHelper.BindFlowDirectionIfNeeded(this, Manager);
		}
		Size MeasureAutoSize(Size availableSize) {
			Size constraint = GetConstraintSize(SizeToContent, availableSize);
			Measure(constraint);
			var placementSize = GetAutoSize(SizeToContent, availableSize, DesiredSize);
			FloatGroup.UpdateAutoFloatingSize(placementSize);
			return placementSize;
		}
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new DevExpress.Xpf.Docking.UIAutomation.FloatingPaneAdornerElementAutomationPeer(this);
		}
		#endregion
		#region IFloatingPane Members
		DockLayoutManager IFloatingPane.Manager {
			get { return this.Manager; }
		}
		FloatGroup IFloatingPane.FloatGroup {
			get { return this.FloatGroup; }
		}
		#endregion
		#region ISupportAutoSize
		bool ISupportAutoSize.IsAutoSize {
			get { return SizeToContent != SizeToContent.Manual; }
		}
		Size ISupportAutoSize.FitToContent(Size availableSize) {
			return MeasureAutoSize(availableSize);
		}
		#endregion
	}
}
