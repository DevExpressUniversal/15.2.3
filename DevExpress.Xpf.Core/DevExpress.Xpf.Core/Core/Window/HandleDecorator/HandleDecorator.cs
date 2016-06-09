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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
#if DXWINDOW
using DevExpress.Internal.DXWindow.Core.HandleDecorator.Helpers;
using MSG = DevExpress.Internal.DXWindow.Core.HandleDecorator.Helpers.MSG;
using DecoratorNativeMethods = DevExpress.Internal.DXWindow.Core.HandleDecorator.Helpers.NativeMethods;
#else
using DevExpress.Utils;
using DevExpress.Xpf.Core.HandleDecorator.Helpers;
using MSG = DevExpress.Xpf.Core.HandleDecorator.Helpers.MSG;
using DecoratorNativeMethods = DevExpress.Xpf.Core.HandleDecorator.Helpers.NativeMethods;
using DevExpress.Xpf.Editors.Helpers;
#endif
using System.Reflection;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow.Core.HandleDecorator {
#else
namespace DevExpress.Xpf.Core.HandleDecorator {
#endif
	public struct StructDecoratorMargins {
		public Thickness LeftMargins;
		public Thickness RightMargins;
		public Thickness TopMargins;
		public Thickness BottomMargins;
	}
	public abstract class Decorator : IDisposable {
		const int firstAppearanceAnimationDurationMs = 220;
		const int restoreAnimationDurationMs = 300;
		SolidColorBrush activeColor, inactiveColor;
		string themeName;
		Thickness decoratorOffset;
		Thickness decoratorLeftMargins;
		Thickness decoratorRightMargins;
		Thickness decoratorTopMargins;
		Thickness decoratorBottomMargins;
		SolidColorBrush defaultActiveColor = new SolidColorBrush(Colors.Black);
		SolidColorBrush defaultInactiveColor = new SolidColorBrush(Colors.Black);
		protected internal string CurrentThemeName {
			get { return themeName; }
			set { themeName = value; }
		}
		protected internal SolidColorBrush ActiveColor {
			get { return activeColor; }
			set {
				if(value == null || value.Color.A == (byte)0) {
					activeColor = defaultActiveColor;
					return;
				}
				activeColor = value;
			}
		}
		public SolidColorBrush InactiveColor {
			get { return inactiveColor; }
			set {
				if(value == null || value.Color.A == (byte)0) {
					inactiveColor = defaultInactiveColor;
					return;
				}
				inactiveColor = value;
			}
		}
		protected internal Thickness DecoratorOffset {
			get { return decoratorOffset; }
		}
		protected internal Thickness DecoratorLeftMargins {
			get { return decoratorLeftMargins; }
		}
		protected internal Thickness DecoratorRightMargins {
			get { return decoratorRightMargins; }
		}
		protected internal Thickness DecoratorTopMargins {
			get { return decoratorTopMargins; }
		}
		protected internal Thickness DecoratorBottomMargins {
			get { return decoratorBottomMargins; }
		}
		Window controlCore;
		protected List<HandleDecoratorWindow> windowsCore;
		public Decorator(SolidColorBrush activeColor, SolidColorBrush inactiveColor, Thickness offset, StructDecoratorMargins structDecoratorMargins, bool startupActiveState) {
			windowsCore = new List<HandleDecoratorWindow>();
			ownerCore = IntPtr.Zero;
			this.stratupActive = startupActiveState;
			this.ActiveColor = activeColor;
			this.InactiveColor = inactiveColor;
			this.decoratorOffset = offset;
			this.decoratorLeftMargins = structDecoratorMargins.LeftMargins;
			this.decoratorRightMargins = structDecoratorMargins.RightMargins;
			this.decoratorTopMargins = structDecoratorMargins.TopMargins;
			this.decoratorBottomMargins = structDecoratorMargins.BottomMargins;
		}
		public Window Control {
			get { return controlCore; }
			set {
				Window oldValue = controlCore;
				controlCore = value;
#if !DXWINDOW
				themeName = GetThemeName(controlCore);
#endif
				if(oldValue != controlCore) OnControlChanged(oldValue, controlCore);
			}
		}
		internal List<HandleDecoratorWindow> Windows {
			get {
				return windowsCore;
			}
		}
		private IntPtr ownerCore;
		public void SetOwner(IntPtr owner) {
			if(Windows.Count == 0 || owner == IntPtr.Zero || ownerCore.Equals(owner)) return;
			ownerCore = owner;
			foreach(var window in Windows) DecoratorNativeMethods.SetWindowLong(window.Handle, NativeHelper.GWL_HWNDPARENT, owner);
		}
		protected void OnControlChanged(Window old, Window newControl) {
			UnSubscribeControlEvents(old);
			SubscribeControlEvents(newControl);
			if(old != null && NativeHelper.IsHandleCreated(old)) DestroyDecoratorWindowHandles();
			if(newControl != null && NativeHelper.IsHandleCreated(newControl)) InitDecoratorWindowHandles();
		}
		private void UnSubscribeControlEvents(Window control) {
			if(control != null) {
				control.SourceInitialized -= controlCore_HandleCreated;
				control.Unloaded -= controlCore_HandleDestroyed;
				control.Closed -= control_Closed;
			}
		}
#if !DXWINDOW
		string GetThemeName(DependencyObject obj) {
			Assembly themeAssembly = null;
			string themeName = ThemeHelper.GetRealThemeName(obj);
			if(themeName == null) return ThemeElementPainter.strLoadDefault;
			try { themeAssembly = AssemblyHelper.GetThemeAssembly(themeName); } finally {
				if(themeAssembly == null) themeName = ThemeElementPainter.strLoadDefault;
			}
			return themeName;
		}
#endif
		private void SubscribeControlEvents(Window control) {
			if(control != null) {
				control.SourceInitialized += controlCore_HandleCreated;
				control.Unloaded += controlCore_HandleDestroyed;
				control.Closed += control_Closed;
			}
		}
		void control_Closed(object sender, EventArgs e) {
			RemoveHook(addedHookHandle);
		}
		IntPtr addedHookHandle;
		WindowInteropHelper interopHelperCore;
		void AddHook(Window window) {
			interopHelperCore = new WindowInteropHelper(window);
			HwndSource.FromHwnd(interopHelperCore.Handle).AddHook(HwndSourceHookHandler);
			addedHookHandle = interopHelperCore.Handle;
		}
		protected virtual IntPtr HwndSourceHookHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			interopHelperCore = new WindowInteropHelper(controlCore);
			switch(msg) {
				case MSG.WM_WINDOWPOSCHANGED: {
						if(hwnd != interopHelperCore.Handle || firstAppearance || restoreAnimation) {
							UpdateDecoratorWindowPos(GetFormRectangle());
							return IntPtr.Zero;
						}
						WindowPosChanged();
						return IntPtr.Zero;
					}
				case NativeHelper.WM_DESTROY: {
						Hide();
						return IntPtr.Zero;
					}
				default:
					return IntPtr.Zero;
			}
		}
		internal protected void Hide() {
			if(windowsCore.Count == 0) return;
			if(windowsCore[0] != null) {
				windowsCore[0].HideWnd();
			}
		}
		void controlCore_HandleDestroyed(object sender, EventArgs e) {
			DestroyDecoratorWindowHandles();
		}
		void controlCore_HandleCreated(object sender, EventArgs e) {
			EnsureThemeProps();
			InitDecoratorWindowHandles();
		}
		void EnsureThemeProps() {
			if(Control == null) return;
#if !DXWINDOW
			this.themeName = GetThemeName(Control);
#endif
			var dxwindow = Control as DXWindow;
			if(dxwindow == null) return;
			this.decoratorTopMargins = dxwindow.BorderEffectTopMargins;
			this.decoratorBottomMargins = dxwindow.BorderEffectBottomMargins;
			this.decoratorLeftMargins = dxwindow.BorderEffectLeftMargins;
			this.decoratorRightMargins = dxwindow.BorderEffectRightMargins;
			this.decoratorOffset = dxwindow.BorderEffectOffset;
		}
		bool stratupActive = true;
		protected abstract HandleDecoratorWindow CreateHandleDecoratorWindow(HandleDecoratorWindowTypes hdt, bool startupActive);
		protected void InitDecoratorWindowHandles() {
			AddHook(Control);
			var decorWindow = CreateHandleDecoratorWindow(HandleDecoratorWindowTypes.Composite, stratupActive);
			windowsCore.Add(decorWindow);
			foreach(var window in windowsCore) window.EnsureHandle();
			UpdateDecoratorWindowsWithDelayIfNecessary();
		}
		void RemoveHook(Window window) {
			interopHelperCore = new WindowInteropHelper(window);
			if((int)interopHelperCore.Handle > 0)
				HwndSource.FromHwnd(interopHelperCore.Handle).RemoveHook(HwndSourceHookHandler);
		}
		void RemoveHook(IntPtr handle) {
			try {
				HwndSource.FromHwnd(handle).RemoveHook(HwndSourceHookHandler);
				DestroyAllDecoratorWindows();
			} catch { }
		}
		protected void DestroyDecoratorWindowHandles() {
			StopTimer();
			IsHandleDecoratorWindowVisible = false;
			RemoveHook(Control);
			DestroyAllDecoratorWindows();
		}
		void DestroyAllDecoratorWindows() {
			if(windowsCore == null || windowsCore.Count == 0)
				return;
			foreach(HandleDecoratorWindow decWindow in windowsCore) {
				decWindow.GetPainter().ClearImages();
				decWindow.Dispose();
			}
			windowsCore.Clear();
		}
		bool activeState = true;
		public bool ActiveState {
			get { return activeState; }
			set { activeState = value; }
		}
		internal void ActiveStateChanged(bool active) {
			if(windowsCore.Count == 0) return;
			activeState = active;
			foreach(var window in windowsCore)
				window.IsActive = active;
		}
		internal SolidColorBrush GetDecoratorWindowColor(bool active) {
			if(active) return activeColor;
			return inactiveColor;
		}
		public void SetDecoratorSizingMargins(Thickness leftMargins, Thickness rightMargins, Thickness topMargins, Thickness bottomMargins) {
			this.decoratorLeftMargins = leftMargins;
			this.decoratorRightMargins = rightMargins;
			this.decoratorTopMargins = topMargins;
			this.decoratorBottomMargins = bottomMargins;
		}
		public void SetDecoratorWindowOffset(Thickness decoratorOffset) {
			this.decoratorOffset = decoratorOffset;
		}
		public void UpdateDecoratorBitmaps(Window owner) {
			if(owner == null) return;
			if(Windows.Count > 0 && Windows[0] != null) Windows[0].GetPainter().ClearImages();
#if !DXWINDOW
			themeName = ThemeManager.GetThemeName(owner);
#endif
			RenderDecorator();
		}
		public void RenderDecorator() {
			if(windowsCore.Count == 0) return;
			foreach(var window in windowsCore) {
				if(window.bmpManager.CachedBitmap != null) window.bmpManager.CachedBitmap.Dispose();
				window.bmpManager.CachedBitmap = null;
				window.RenderDecoratorWindow(false);
			}
		}
		bool restoreAnimation = false;
		bool firstAppearance = true;
		Timer timerCore;
		private void UpdateDecoratorWindows(bool delayIfNecessary, bool delayWhileMove) {
			if(delayIfNecessary) {
				StartDelayTimer();
				return;
			}
			if(firstAppearance) {
				firstAppearance = false;
				return;
			}
			if(timerCore != null)
				StopTimer();
			if(windowsCore == null || windowsCore.Count == 0)
				return;
			UpdateHandleDecoratorWindowPositions(delayWhileMove);
		}
		void StartDelayTimer() {
			if(timerCore != null && !firstAppearance) {
				timerCore.Stop();
			}
			else {
				timerCore = new Timer();
				timerCore.Interval = firstAppearance ? firstAppearanceAnimationDurationMs : restoreAnimationDurationMs;
				timerCore.Tick += timerCore_Tick;
			}
			if(!firstAppearance)
				restoreAnimation = true;
			timerCore.Start();
		}
		Rectangle prevControlRect;
		private void UpdateDecoratorWindowsWithDelayIfNecessary() {
			var formRect = GetFormRectangle();
			UpdateDecoratorWindowPos(formRect);
			var shouldDelay = ShouldDelay(NativeHelper.GetHandle(Control));
			var delayWhileMove = ShouldDelayWhileMove(formRect, prevControlRect);
			UpdateDecoratorWindows(shouldDelay, delayWhileMove);
			prevControlRect = formRect;
		}
		Rectangle GetFormRectangle() {
			DecoratorNativeMethods.RECT rect = new DecoratorNativeMethods.RECT();
			DecoratorNativeMethods.GetWindowRect(NativeHelper.GetHandle(Control), ref rect);
			return rect.ToRectangle();
		}
		private void UpdateDecoratorWindowPos(Rectangle formRect) {
			foreach(var window in windowsCore) window.UpdateWindowPos(formRect);
		}
		private bool ShouldDelayWhileMove(Rectangle currentCtrlBounds, Rectangle previousCtrlBounds) {
			bool res = false;
			var decoratorWindow = GetHwndDecoratorWindowByType(HandleDecoratorWindowTypes.Composite);
			if(decoratorWindow == null) decoratorWindow = GetHwndDecoratorWindowByType(HandleDecoratorWindowTypes.Left);
			if(decoratorWindow == null) return true;
			var leftBorderBounds = HandleDecoratorWindowLayoutCalculator.Calculate(HandleDecoratorWindowTypes.Left, currentCtrlBounds, decoratorWindow.GetPainter());
			var bordersWidth = leftBorderBounds.Width;
			var bordersOffset = decoratorWindow.GetPainter().GetOffsetByWindowType(HandleDecoratorWindowTypes.Left);
			if(bordersOffset != 0 && (Math.Abs(previousCtrlBounds.Width - currentCtrlBounds.Width) > bordersWidth || Math.Abs(previousCtrlBounds.Height - currentCtrlBounds.Height) > bordersWidth)) res = true;
			return res;
		}
		private HandleDecoratorWindow GetHwndDecoratorWindowByType(HandleDecoratorWindowTypes type) {
			if(Windows.Count == 0) return null;
			foreach(var window in Windows) {
				if(window.WindowType == type)
					return window;
			}
			return null;
		}
		void timerCore_Tick(object sender, EventArgs e) {
			if(firstAppearance) firstAppearance = false;
			StopTimer();
			UpdateDecoratorWindows(false, false);
			restoreAnimation = false;
		}
		void StopTimer() {
			if(timerCore != null) {
				timerCore.Stop();
				timerCore.Tick -= timerCore_Tick;
				timerCore.Dispose();
				timerCore = null;
			}
		}
		internal void WindowPosChanged() {
			UpdateDecoratorWindowsWithDelayIfNecessary();
		}
		bool ShouldShowHandleDecoratorWindow {
			get {
				if(Control == null)
					return false;
				if(Control.Width == 0 || Control.Height == 0)
					return false;
				return DecoratorNativeMethods.IsWindowVisible(NativeHelper.GetHandle(Control)) && !DecoratorNativeMethods.IsIconic(NativeHelper.GetHandle(Control)) && !DecoratorNativeMethods.IsZoomed(NativeHelper.GetHandle(Control));
			}
		}
		bool IsHandleDecoratorWindowVisible { get; set; }
		private void UpdateHandleDecoratorWindowPositions(bool delayWhileMove) {
			if(windowsCore == null || windowsCore.Count == 0)
				return;
			SetHandleDecoratorWindowsVisibility(ShouldShowHandleDecoratorWindow);
			UpdateZOrder();
			foreach(var window in windowsCore) window.CommitChanges(restoreAnimation, delayWhileMove);
		}
		private void UpdateZOrder() {
			if(windowsCore == null || windowsCore.Count == 0)
				return;
			IntPtr handle = NativeHelper.GetHandle(controlCore);
			foreach(var window in windowsCore) {
				SetZOrderOfWindows(window.Handle, handle);
				handle = window.Handle;
			}
		}
		void SetZOrderOfWindows(IntPtr hWndFirst, IntPtr hWndSecond) {
			if(hWndFirst == IntPtr.Zero)
				return;
			if(DecoratorNativeMethods.GetWindow(hWndFirst, DecoratorNativeMethods.GW_HWNDPREV) != hWndSecond)
				DecoratorNativeMethods.SetWindowPos(hWndFirst, hWndSecond, 0, 0, 0, 0, DecoratorNativeMethods.SWP.SWP_NOMOVE | DecoratorNativeMethods.SWP.SWP_NOSIZE | DecoratorNativeMethods.SWP.SWP_NOACTIVATE);
		}
		DecoratorNativeMethods.ShowWindowCommands prevWindowPlacement;
		bool ShouldDelay(IntPtr hWnd) {
			DecoratorNativeMethods.WINDOWPLACEMENT windowPlacement;
			DecoratorNativeMethods.GetWindowPlacement(hWnd, out windowPlacement);
			DecoratorNativeMethods.ShowWindowCommands showCmd = windowPlacement.ShowCmd;
			bool res = false;
			if(showCmd == DecoratorNativeMethods.ShowWindowCommands.Normal
				&& (prevWindowPlacement == DecoratorNativeMethods.ShowWindowCommands.Maximize
					|| prevWindowPlacement == DecoratorNativeMethods.ShowWindowCommands.ShowMaximized
					|| prevWindowPlacement == DecoratorNativeMethods.ShowWindowCommands.Minimize
					|| prevWindowPlacement == DecoratorNativeMethods.ShowWindowCommands.ShowMinimized
					|| prevWindowPlacement == DecoratorNativeMethods.ShowWindowCommands.Hide)) {
				res = true;
			}
			if(showCmd == DecoratorNativeMethods.ShowWindowCommands.Hide)
				firstAppearance = true;
			prevWindowPlacement = showCmd;
			return res;
		}
		void SetHandleDecoratorWindowsVisibility(bool isVisible) {
			if(windowsCore == null || windowsCore.Count == 0)
				return;
			if(IsHandleDecoratorWindowVisible == isVisible)
				return;
			foreach(var window in windowsCore) {
				window.IsVisible = isVisible;
			}
			IsHandleDecoratorWindowVisible = isVisible;
		}
		#region IDisposable Members
		public void Dispose() {
			UnSubscribeControlEvents(Control);
			DestroyDecoratorWindowHandles();
		}
		#endregion
	}
	public class FormHandleDecorator : Decorator {
		public FormHandleDecorator(SolidColorBrush activeColor, SolidColorBrush inactiveColor, Thickness decoratorOffset, StructDecoratorMargins structDecoratorMargins, bool startupActiveState) : base(activeColor, inactiveColor, decoratorOffset, structDecoratorMargins, startupActiveState) { }
		protected override HandleDecoratorWindow CreateHandleDecoratorWindow(HandleDecoratorWindowTypes hdt, bool startupActive) {
			return new FormDecoratorWindow(this, hdt, startupActive);
		}
	}
}
