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
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Interop;
using POINT = DevExpress.Xpf.Core.NativeMethods.POINT;
using RECT = DevExpress.Xpf.Core.NativeMethods.RECT;
namespace DevExpress.Xpf.Docking.Platform.Win32 {
	[Flags]
	enum WS : uint {
		WS_BORDER = 0x00800000,
		WS_CAPTION = 0x00C00000,
		WS_CHILD = 0x40000000,
		WS_CHILDWINDOW = 0x40000000,
		WS_CLIPCHILDREN = 0x02000000,
		WS_CLIPSIBLINGS = 0x04000000,
		WS_DISABLED = 0x08000000,
		WS_DLGFRAME = 0x00400000,
		WS_GROUP = 0x00020000,
		WS_HSCROLL = 0x00100000,
		WS_ICONIC = 0x20000000,
		WS_MAXIMIZE = 0x01000000,
		WS_MAXIMIZEBOX = 0x00010000,
		WS_MINIMIZE = 0x20000000,
		WS_MINIMIZEBOX = 0x00020000,
		WS_OVERLAPPED = 0x00000000,
		WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
		WS_POPUP = 0x80000000,
		WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
		WS_SIZEBOX = 0x00040000,
		WS_SYSMENU = 0x00080000,
		WS_TABSTOP = 0x00010000,
		WS_THICKFRAME = 0x00040000,
		WS_TILED = 0x00000000,
		WS_TILEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
		WS_VISIBLE = 0x10000000,
		WS_VSCROLL = 0x00200000,
	}
	class SC {
		internal const int SC_SIZE = 0xF000;
		internal const int SC_MAXIMIZE = 0xF030;
		internal const int SC_CLOSE = 0xF060;
		internal const int SC_RESTORE = 0xF120;
		internal const int SC_MOVE = 0xF010;
		internal const int SC_MINIMIZE = 0xF020;
	}
	enum WM {
		WM_MOVE = 0x0003,
		WM_SIZE = 0x0005,
		WM_ACTIVATE = 0x0006,
		WM_SETFOCUS = 0x0007,
		WM_KILLFOCUS = 0x0008,
		WM_SHOWWINDOW = 0x0018,
		WM_GETMINMAXINFO = 0x0024,
		WM_WINDOWPOSCHANGING = 0x0046,
		WM_WINDOWPOSCHANGED = 0x0047,
		WM_NCCALCSIZE = 0x0083,
		WM_NCHITTEST = 0x0084,
		WM_NCACTIVATE = 0x0086,
		WM_NCMOUSEMOVE = 0x00A0,
		WM_KEYDOWN = 0x0100,
		WM_KEYUP = 0x0101,
		WM_SYSCOMMAND = 0x0112,
		WM_INITMENUPOPUP = 0x0117,
		WM_LBUTTONUP = 0x0202,
		WM_SIZING = 0x0214,
		WM_CAPTURECHANGED = 0x0215,
		WM_MOVING = 0x0216,
		WM_ENTERSIZEMOVE = 0x0231,
		WM_EXITSIZEMOVE = 0x0232,
		WM_DWMCOMPOSITIONCHANGED = 0x031E,
	}
	internal enum WVR {
		ALIGNTOP = 0x0010,
		ALIGNLEFT = 0x0020,
		ALIGNBOTTOM = 0x0040,
		ALIGNRIGHT = 0x0080,
		HREDRAW = 0x0100,
		VREDRAW = 0x0200,
		VALIDRECTS = 0x0400,
		REDRAW = HREDRAW | VREDRAW,
	}
	[StructLayout(LayoutKind.Sequential)]
	internal struct WINDOWPOS {
		public IntPtr hwnd;
		public IntPtr hwndInsertAfter;
		public int x;
		public int y;
		public int cx;
		public int cy;
		public int flags;
	}
	internal enum SW {
		HIDE = 0,
		SHOWNORMAL = 1,
		NORMAL = 1,
		SHOWMINIMIZED = 2,
		SHOWMAXIMIZED = 3,
		MAXIMIZE = 3,
		SHOWNOACTIVATE = 4,
		SHOW = 5,
		MINIMIZE = 6,
		SHOWMINNOACTIVE = 7,
		SHOWNA = 8,
		RESTORE = 9,
		SHOWDEFAULT = 10,
		FORCEMINIMIZE = 11,
	}
	internal enum SWP {
		ASYNCWINDOWPOS = 0x4000,
		DEFERERASE = 0x2000,
		DRAWFRAME = 0x0020,
		FRAMECHANGED = 0x0020,
		HIDEWINDOW = 0x0080,
		NOACTIVATE = 0x0010,
		NOCOPYBITS = 0x0100,
		NOMOVE = 0x0002,
		NOOWNERZORDER = 0x0200,
		NOREDRAW = 0x0008,
		NOREPOSITION = 0x0200,
		NOSENDCHANGING = 0x0400,
		NOSIZE = 0x0001,
		NOZORDER = 0x0004,
		SHOWWINDOW = 0x0040,
	}
	internal enum GWL {
		WNDPROC = -4,
		HINSTANCE = -6,
		HWNDPARENT = -8,
		STYLE = -16,
		EXSTYLE = -20,
		USERDATA = -21,
		ID = -12
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct MINMAXINFO {
		public DevExpress.Xpf.Core.NativeMethods.POINT ptReserved;
		public DevExpress.Xpf.Core.NativeMethods.POINT ptMaxSize;
		public DevExpress.Xpf.Core.NativeMethods.POINT ptMaxPosition;
		public DevExpress.Xpf.Core.NativeMethods.POINT ptMinTrackSize;
		public DevExpress.Xpf.Core.NativeMethods.POINT ptMaxTrackSize;
	};
	[StructLayout(LayoutKind.Sequential)]
	internal class WINDOWPLACEMENT {
		public int length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
		public int flags;
		public SW showCmd;
		public POINT ptMinPosition;
		public POINT ptMaxPosition;
		public RECT rcNormalPosition;
	}
	[StructLayout(LayoutKind.Sequential)]
	public class MONITORINFO {
		public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
		public DevExpress.Xpf.Core.NativeMethods.RECT rcMonitor = new DevExpress.Xpf.Core.NativeMethods.RECT();
		public DevExpress.Xpf.Core.NativeMethods.RECT rcWork = new DevExpress.Xpf.Core.NativeMethods.RECT();
		public int dwFlags = 0;
	}
	static class NativeHelper {
		[SecuritySafeCritical]
		public static IEnumerable<Window> SortWindowsTopToBottom(IEnumerable<Window> unsorted) {
			var byHandle = unsorted.Where(x => PresentationSource.FromVisual(x) != null).
				ToDictionary(win =>
				((HwndSource)PresentationSource.FromVisual(win)).Handle);
			for(IntPtr hWnd = GetTopWindowSafe(IntPtr.Zero); hWnd != IntPtr.Zero; hWnd = GetNextWindowSafe(hWnd, GW_HWNDNEXT)) {
				if(byHandle.ContainsKey(hWnd))
					yield return byHandle[hWnd];
			}
		}
		[SecuritySafeCritical]
		static IntPtr GetTopWindowSafe(IntPtr hWnd) {
			return GetTopWindow(hWnd);
		}
		[SecuritySafeCritical]
		static IntPtr GetNextWindowSafe(IntPtr hWnd, uint wCmd) {
			return GetWindow(hWnd, wCmd);
		}
		const uint GW_HWNDNEXT = 2;
		[DllImport("User32")]
		static extern IntPtr GetTopWindow(IntPtr hWnd);
		[DllImport("User32")]
		static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);
		internal const int SW_PARENTCLOSING = 1;
		internal const int SW_PARENTOPENING = 3;
		internal const int GWL_STYLE = -16;
		internal const uint MF_BYCOMMAND = 0x00000000;
		internal const uint MF_GRAYED = 0x00000001;
		internal const uint MF_ENABLED = 0x00000000;
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		static extern bool SetForegroundWindow(HandleRef hWnd);
		[SecuritySafeCritical]
		internal static bool SetForegroundWindowSafe(HandleRef hWnd) {
			return SetForegroundWindow(hWnd);
		}
		[Flags]
		internal enum SetWindowPosFlags : uint {
			SynchronousWindowPosition = 0x4000,
			DeferErase = 0x2000,
			DrawFrame = 0x0020,
			FrameChanged = 0x0020,
			HideWindow = 0x0080,
			DoNotActivate = 0x0010,
			DoNotCopyBits = 0x0100,
			IgnoreMove = 0x0002,
			DoNotChangeOwnerZOrder = 0x0200,
			DoNotRedraw = 0x0008,
			DoNotReposition = 0x0200,
			DoNotSendChangingEvent = 0x0400,
			IgnoreResize = 0x0001,
			IgnoreZOrder = 0x0004,
			ShowWindow = 0x0040,
		}
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);
		[SecuritySafeCritical]
		internal static bool SetWindowPosSafe(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags) {
			return SetWindowPos(hWnd, hWndInsertAfter, X, Y, cx, cy, (SetWindowPosFlags)uFlags);
		}
		[DllImport("user32.dll")]
		internal static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
		[SecuritySafeCritical]
		internal static int SendMessageSafe(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam) {
			return SendMessage(hWnd, Msg, wParam, lParam);
		}
		[DllImport("user32.dll")]
		static extern int PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll")]
		static extern bool GetCursorPos(ref Win32Point pt);
		[StructLayout(LayoutKind.Sequential)]
		internal struct Win32Point {
			public Int32 X;
			public Int32 Y;
		};
		[SecuritySafeCritical]
		internal static Point GetMousePositionSafe() {
			Win32Point w32Mouse = new Win32Point();
			GetCursorPos(ref w32Mouse);
			return new Point(w32Mouse.X, w32Mouse.Y);
		}
		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
		[SecuritySafeCritical]
		internal static int SetWindowLongSafe(IntPtr hWnd, int nIndex, int dwNewLong) {
			return SetWindowLong(hWnd, nIndex, dwNewLong);
		}
		[DllImport("user32.dll", SetLastError = true)]
		static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		[SecuritySafeCritical]
		internal static int GetWindowLongSafe(IntPtr hWnd, int nIndex) {
			return GetWindowLong(hWnd, nIndex);
		}
		[DllImport("user32.dll")]
		static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
		[SecuritySafeCritical]
		internal static IntPtr GetSystemMenuSafe(IntPtr hWnd, bool bRevert) {
			return GetSystemMenu(hWnd, bRevert);
		}
		[DllImport("user32.dll")]
		static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
		[SecuritySafeCritical]
		internal static bool EnableMenuItemSafe(IntPtr hMenu, uint uIDEnableItem, uint uEnable) {
			return EnableMenuItem(hMenu, uIDEnableItem, uEnable);
		}
		[DllImport("user32.dll")]
		static extern bool SetCursorPos(int x, int y);
		[DllImport("user32.dll")]
		public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
		public const int MOUSEEVENTF_LEFTDOWN = 0x02;
		public const int MOUSEEVENTF_LEFTUP = 0x04;
		[SecuritySafeCritical]
		static void LeftMouseClick(int xpos, int ypos) {
			SetCursorPos(xpos, ypos);
			mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
			mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
		}
		[SecuritySafeCritical]
		public static void MouseDoubleClick(int x, int y) {
			LeftMouseClick(x, y);
			LeftMouseClick(x, y);
		}
		[DllImport("user32")]
		internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
		[DllImport("User32")]
		internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
		[SecuritySafeCritical]
		internal static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam) {
			MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
			int MONITOR_DEFAULTTONEAREST = 0x00000002;
			System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
			if(monitor != System.IntPtr.Zero) {
				MONITORINFO monitorInfo = new MONITORINFO();
				GetMonitorInfo(monitor, monitorInfo);
				DevExpress.Xpf.Core.NativeMethods.RECT rcWorkArea = monitorInfo.rcWork;
				DevExpress.Xpf.Core.NativeMethods.RECT rcMonitorArea = monitorInfo.rcMonitor;
				mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
				mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
				mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
				mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
				mmi.ptMaxTrackSize.x = mmi.ptMaxSize.x;
				mmi.ptMaxTrackSize.y = mmi.ptMaxSize.y;
			}
			Marshal.StructureToPtr(mmi, lParam, true);
		}
		public static int GET_X_LPARAM(IntPtr lParam) {
			return LOWORD(lParam.ToInt32());
		}
		public static int GET_Y_LPARAM(IntPtr lParam) {
			return HIWORD(lParam.ToInt32());
		}
		public static int HIWORD(int i) {
			return (short)(i >> 16);
		}
		public static int LOWORD(int i) {
			return (short)(i & 0xFFFF);
		}
		public static bool IsFlagSet(int value, int mask) {
			return 0 != (value & mask);
		}
		[SecurityCritical]
		public static void SafeDeleteObject(ref IntPtr gdiObject) {
			IntPtr p = gdiObject;
			gdiObject = IntPtr.Zero;
			if(IntPtr.Zero != p) {
				DeleteObject(p);
			}
		}
		[SecurityCritical]
		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject(IntPtr hObject);
		[SecurityCritical]
		[DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn", SetLastError = true)]
		private static extern IntPtr _CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
		[SecurityCritical]
		public static IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse) {
			IntPtr ret = _CreateRoundRectRgn(nLeftRect, nTopRect, nRightRect, nBottomRect, nWidthEllipse, nHeightEllipse);
			if(IntPtr.Zero == ret) {
				throw new Win32Exception();
			}
			return ret;
		}
		[SecurityCritical]
		[DllImport("gdi32.dll", EntryPoint = "CreateRectRgn", SetLastError = true)]
		private static extern IntPtr _CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
		[SecurityCritical]
		public static IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect) {
			IntPtr ret = _CreateRectRgn(nLeftRect, nTopRect, nRightRect, nBottomRect);
			if(IntPtr.Zero == ret) {
				throw new Win32Exception();
			}
			return ret;
		}
		[SecurityCritical]
		[DllImport("gdi32.dll", EntryPoint = "CreateRectRgnIndirect", SetLastError = true)]
		private static extern IntPtr _CreateRectRgnIndirect([In] ref RECT lprc);
		[SecurityCritical]
		public static IntPtr CreateRectRgnIndirect(RECT lprc) {
			IntPtr ret = _CreateRectRgnIndirect(ref lprc);
			if(IntPtr.Zero == ret) {
				throw new Win32Exception();
			}
			return ret;
		}
		[SecurityCritical]
		[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "DefWindowProcW")]
		public static extern IntPtr DefWindowProc(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);
		[SecurityCritical]
		public static MONITORINFO GetMonitorInfo(IntPtr hMonitor) {
			var mi = new MONITORINFO();
			if(!GetMonitorInfo(hMonitor, mi)) {
				throw new Win32Exception();
			}
			return mi;
		}
		[SecurityCritical]
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetWindowPlacement(IntPtr hwnd, WINDOWPLACEMENT lpwndpl);
		[SecurityCritical]
		public static WINDOWPLACEMENT GetWindowPlacement(IntPtr hwnd) {
			WINDOWPLACEMENT wndpl = new WINDOWPLACEMENT();
			if(GetWindowPlacement(hwnd, wndpl)) {
				return wndpl;
			}
			throw new Win32Exception();
		}
		[SecurityCritical]
		[DllImport("user32.dll", EntryPoint = "GetWindowRect", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool _GetWindowRect(IntPtr hWnd, out RECT lpRect);
		[SecurityCritical]
		public static RECT GetWindowRect(IntPtr hwnd) {
			RECT rc;
			if(!_GetWindowRect(hwnd, out rc)) {
				throw new Win32Exception();
			}
			return rc;
		}
		[SecurityCritical]
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowVisible(IntPtr hwnd);
		[SecurityCritical]
		[DllImport("user32.dll", EntryPoint = "SetWindowRgn", SetLastError = true)]
		private static extern int _SetWindowRgn(IntPtr hWnd, IntPtr hRgn, [MarshalAs(UnmanagedType.Bool)] bool bRedraw);
		[SecurityCritical]
		public static void SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw) {
			int err = _SetWindowRgn(hWnd, hRgn, bRedraw);
			if(0 == err) {
				throw new Win32Exception();
			}
		}
	}
}
