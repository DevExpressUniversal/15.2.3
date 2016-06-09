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

using DevExpress.Xpf.Docking.Platform.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Assert = DevExpress.Xpf.Layout.Core.Base.AssertionException;
using RECT = DevExpress.Xpf.Core.NativeMethods.RECT;
namespace DevExpress.Xpf.Docking.Platform.Shell {
	using HANDLE_MESSAGE = System.Collections.Generic.KeyValuePair<WM, MessageHandler>;
	delegate IntPtr MessageHandler(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled);
	class WindowChromeWorker : DependencyObject {
		#region static
		[SecurityCritical]
		static WindowChromeWorker() { }
		public static readonly DependencyProperty WindowChromeWorkerProperty = DependencyProperty.RegisterAttached(
			"WindowChromeWorker", typeof(WindowChromeWorker), typeof(WindowChromeWorker),
			new PropertyMetadata(null, _OnChromeWorkerChanged));
		[SecuritySafeCritical]
		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		private static void _OnChromeWorkerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var w = (Window)d;
			var cw = (WindowChromeWorker)e.NewValue;
			Assert.IsNotNull(w);
			Assert.IsNotNull(cw);
			Assert.IsNull(cw._window);
			cw.SetWindow(w);
		}
		public static WindowChromeWorker GetWindowChromeWorker(Window window) {
			Assert.IsNotNull(window, "window");
			return (WindowChromeWorker)window.GetValue(WindowChromeWorkerProperty);
		}
		public static void SetWindowChromeWorker(Window window, WindowChromeWorker chrome) {
			Assert.IsNotNull(window, "window");
			window.SetValue(WindowChromeWorkerProperty, chrome);
		}
		#endregion
		const SWP _swpFlags = SWP.FRAMECHANGED | SWP.NOSIZE | SWP.NOMOVE | SWP.NOZORDER | SWP.NOOWNERZORDER | SWP.NOACTIVATE;
		readonly List<HANDLE_MESSAGE> _messageTable;
		Window _window;
		[SecurityCritical]
		IntPtr _hwnd;
		[SecurityCritical]
		HwndSource _hwndSource = null;
		bool _isHooked = false;
		WindowChrome _chrome;
		WindowState _lastRoundingState;
		[SecuritySafeCritical]
		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public WindowChromeWorker() {
			_messageTable = new List<HANDLE_MESSAGE> {
				new HANDLE_MESSAGE(WM.WM_NCACTIVATE,			OnNCActivate),
				new HANDLE_MESSAGE(WM.WM_NCCALCSIZE,			OnNCCalcSize),
				new HANDLE_MESSAGE(WM.WM_WINDOWPOSCHANGED,	  OnWindowPosChanged),
				new HANDLE_MESSAGE(WM.WM_DWMCOMPOSITIONCHANGED, OnDwmCompositionChanged),
			};
		}
		[SecuritySafeCritical]
		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public void SetWindowChrome(WindowChrome windowChrome) {
			VerifyAccess();
			Assert.IsNotNull(_window);
			if(windowChrome == _chrome) return;
			_chrome = windowChrome;
			ApplyWindowChrome();
		}
		[SecurityCritical]
		void SetWindow(Window window) {
			Assert.IsNull(_window);
			Assert.IsNotNull(window);
			UnsubscribeWindowEvents();
			_window = window;
			_hwnd = new WindowInteropHelper(_window).Handle;
			_window.Closed += UnsetWindow;
			if(IntPtr.Zero != _hwnd) {
				_hwndSource = HwndSource.FromHwnd(_hwnd);
				Assert.IsNotNull(_hwndSource);
				_window.ApplyTemplate();
				if(_chrome != null) {
					ApplyWindowChrome();
				}
			}
			else {
				_window.SourceInitialized += OnWindowSourceInitialized;
			}
		}
		[SecuritySafeCritical]
		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		void OnWindowSourceInitialized(object sender, EventArgs e) {
			_hwnd = new WindowInteropHelper(_window).Handle;
			Assert.IsNotDefault(_hwnd);
			_hwndSource = HwndSource.FromHwnd(_hwnd);
			Assert.IsNotNull(_hwndSource);
			if(_chrome != null) {
				ApplyWindowChrome();
			}
		}
		[SecurityCritical]
		void UnsubscribeWindowEvents() {
			if(_window != null) {
				_window.SourceInitialized -= OnWindowSourceInitialized;
			}
		}
		[SecuritySafeCritical]
		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		void UnsetWindow(object sender, EventArgs e) {
			UnsubscribeWindowEvents();
			RestoreStandardChromeState(true);
		}
		[SecurityCritical]
		void ApplyWindowChrome() {
			if(_hwnd == IntPtr.Zero || _hwndSource.IsDisposed) {
				return;
			}
			if(_chrome == null) {
				RestoreStandardChromeState(false);
				return;
			}
			if(!_isHooked) {
				_hwndSource.AddHook(_WndProc);
				_isHooked = true;
			}
			UpdateFrameState();
			NativeHelper.SetWindowPosSafe(_hwnd, IntPtr.Zero, 0, 0, 0, 0, (int)_swpFlags);
		}
		#region Window Message Handlers
		[SecurityCritical]
		IntPtr _WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			Assert.AreEqual(hwnd, _hwnd);
			var message = (WM)msg;
			foreach(var handlePair in _messageTable) {
				if(handlePair.Key == message) {
					return handlePair.Value(message, wParam, lParam, out handled);
				}
			}
			return IntPtr.Zero;
		}
		[SecurityCritical]
		IntPtr OnNCActivate(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled) {
			IntPtr lRet = NativeHelper.DefWindowProc(_hwnd, WM.WM_NCACTIVATE, wParam, new IntPtr(-1));
			handled = true;
			return lRet;
		}
		[SecurityCritical]
		IntPtr OnNCCalcSize(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled) {
			handled = true;
			return new IntPtr((int)(WVR.VALIDRECTS | WVR.REDRAW));
		}
		private WINDOWPOS _previousWP;
		[SecurityCritical]
		IntPtr OnWindowPosChanged(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled) {
			Assert.IsNotDefault(lParam);
			var wp = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
			if (!wp.Equals(_previousWP) && !NativeHelper.IsFlagSet(wp.flags, (int)SWP.NOSIZE)) {
				SetRoundingRegion(wp);
			}
			_previousWP = wp;
			handled = false;
			return IntPtr.Zero;
		}
		[SecurityCritical]
		IntPtr OnDwmCompositionChanged(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled) {
			handled = false;
			return IntPtr.Zero;
		}
		#endregion
		[SecurityCritical]
		bool _ModifyStyle(WS removeStyle, WS addStyle) {
			Assert.IsNotDefault(_hwnd);
			var dwStyle = (WS)NativeHelper.GetWindowLongSafe(_hwnd, NativeHelper.GWL_STYLE);  
			var dwNewStyle = (dwStyle & ~removeStyle) | addStyle;
			if(dwStyle == dwNewStyle) {
				return false;
			}
			NativeHelper.SetWindowLongSafe(_hwnd, (int)GWL.STYLE, (int)dwNewStyle);
			return true;
		}
		[SecurityCritical]
		WindowState GetHwndState() {
			var wpl = NativeHelper.GetWindowPlacement(_hwnd);
			switch(wpl.showCmd) {
				case SW.SHOWMINIMIZED: return WindowState.Minimized;
				case SW.SHOWMAXIMIZED: return WindowState.Maximized;
				default: break;
			}
			return WindowState.Normal;
		}
		[SecurityCritical]
		Rect GetWindowRect() {
			RECT windowPosition = NativeHelper.GetWindowRect(_hwnd);
			return new Rect(windowPosition.left, windowPosition.top, windowPosition.Width, windowPosition.Height);
		}
		[SecurityCritical]
		void UpdateFrameState() {
			if(IntPtr.Zero == _hwnd || _hwndSource.IsDisposed) {
				return;
			}
			SetRoundingRegion(null);
			NativeHelper.SetWindowPosSafe(_hwnd, IntPtr.Zero, 0, 0, 0, 0, (int)_swpFlags);
		}
		[SecurityCritical]
		void ClearRoundingRegion() {
			NativeHelper.SetWindowRgn(_hwnd, IntPtr.Zero, NativeHelper.IsWindowVisible(_hwnd));
		}
		[SecurityCritical]
		void SetRoundingRegion(WINDOWPOS? wp) {
			const int MONITOR_DEFAULTTONEAREST = 0x00000002;
			WINDOWPLACEMENT wpl = NativeHelper.GetWindowPlacement(_hwnd);
			if(wpl.showCmd == SW.SHOWMAXIMIZED) {
				int left;
				int top;
				if(wp.HasValue) {
					left = wp.Value.x;
					top = wp.Value.y;
				}
				else {
					Rect r = GetWindowRect();
					left = (int)r.Left;
					top = (int)r.Top;
				}
				IntPtr hMon = NativeHelper.MonitorFromWindow(_hwnd, MONITOR_DEFAULTTONEAREST);
				MONITORINFO mi = NativeHelper.GetMonitorInfo(hMon);
				RECT rcMax = mi.rcWork;
				rcMax.Offset(-left, -top);
				IntPtr hrgn = IntPtr.Zero;
				try {
					hrgn = NativeHelper.CreateRectRgnIndirect(rcMax);
					NativeHelper.SetWindowRgn(_hwnd, hrgn, NativeHelper.IsWindowVisible(_hwnd));
					hrgn = IntPtr.Zero;
				}
				finally {
					NativeHelper.SafeDeleteObject(ref hrgn);
				}
			}
			else {
				Size windowSize;
				if(null != wp && !NativeHelper.IsFlagSet(wp.Value.flags, (int)SWP.NOSIZE)) {
					windowSize = new Size((double)wp.Value.cx, (double)wp.Value.cy);
				}
				else if(null != wp && (_lastRoundingState == _window.WindowState)) {
					return;
				}
				else {
					windowSize = GetWindowRect().Size;
				}
				_lastRoundingState = _window.WindowState;
				IntPtr hrgn = IntPtr.Zero;
				try {
					double shortestDimension = Math.Min(windowSize.Width, windowSize.Height);
					double topLeftRadius = DpiHelper.LogicalPixelsToDevice(new Point(0, 0)).X;
					topLeftRadius = Math.Min(topLeftRadius, shortestDimension / 2);
					hrgn = CreateRoundRectRgn(new Rect(windowSize), topLeftRadius);
					NativeHelper.SetWindowRgn(_hwnd, hrgn, NativeHelper.IsWindowVisible(_hwnd));
					hrgn = IntPtr.Zero;
				}
				finally {
					NativeHelper.SafeDeleteObject(ref hrgn);
				}
			}
		}
		[SecurityCritical]
		static IntPtr CreateRoundRectRgn(Rect region, double radius) {
			return NativeHelper.CreateRectRgn(
				(int)Math.Floor(region.Left),
				(int)Math.Floor(region.Top),
				(int)Math.Ceiling(region.Right),
				(int)Math.Ceiling(region.Bottom));
		}
		[SecurityCritical]
		void RestoreStandardChromeState(bool isClosing) {
			VerifyAccess();
			UnhookCustomChrome();
			if(!isClosing && !_hwndSource.IsDisposed) {
				RestoreHrgn();
				_window.InvalidateMeasure();
			}
		}
		[SecurityCritical]
		void UnhookCustomChrome() {
			Assert.IsNotNull(_window);
			if(_isHooked) {
				_hwndSource.RemoveHook(_WndProc);
				_isHooked = false;
			}
		}
		[SecurityCritical]
		void RestoreHrgn() {
			ClearRoundingRegion();
			NativeHelper.SetWindowPosSafe(_hwnd, IntPtr.Zero, 0, 0, 0, 0, (int)_swpFlags);
		}
		static class DpiHelper {
			static Matrix _transformToDevice;
			[SecuritySafeCritical]
			static DpiHelper() {
				IntPtr hdc = DevExpress.Xpf.Core.NativeMethods.GetDC(IntPtr.Zero);
				int pixelsPerInchX = DevExpress.Xpf.Core.NativeMethods.GetDeviceCaps(hdc, DevExpress.Xpf.Core.NativeMethods.LOGPIXELSX);
				int pixelsPerInchY = DevExpress.Xpf.Core.NativeMethods.GetDeviceCaps(hdc, DevExpress.Xpf.Core.NativeMethods.LOGPIXELSY);
				_transformToDevice = Matrix.Identity;
				_transformToDevice.Scale((double)pixelsPerInchX / 96d, (double)pixelsPerInchY / 96d);
			}
			public static Point LogicalPixelsToDevice(Point logicalPoint) {
				return _transformToDevice.Transform(logicalPoint);
			}
		}
	}
}
