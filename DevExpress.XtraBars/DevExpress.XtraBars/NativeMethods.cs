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
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.XtraBars {
	[System.Security.SecuritySafeCritical]
	internal static class BarNativeMethods {
		internal const int TVIF_STATE = 0x8;
		internal const int TVIS_STATEIMAGEMASK = 0xF000;
		internal const int TV_FIRST = 0x1100;
		internal const int TVM_SETITEM = TV_FIRST + 63;
		internal const int TME_NONCLIENT = 0x00000010;
		internal const int TME_LEAVE = 0x00000002;
		internal const int GWL_STYLE = -16;
		internal const int WS_CHILD = 0x40000000;
		internal const int GWL_HWNDPARENT = -8;
		[StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
		internal struct TVITEM {
			public int mask;
			public IntPtr hItem;
			public int state;
			public int stateMask;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string lpszText;
			public int cchTextMax;
			public int iImage;
			public int iSelectedImage;
			public int cChildren;
			public IntPtr lParam;
		}
		[StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		internal struct CURSORINFO {
			public int cbSize;
			public int flags;
			public IntPtr hCursor;
			public Point ptScreenPos;
		}
		[StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		internal struct MENUITEMINFO {
			public uint cbSize;
			public uint fMask;
			public uint fType;
			public uint fState;
			public uint wID;
			public IntPtr hSubMenu;
			public IntPtr hbmpChecked;
			public IntPtr hbmpUnchecked;
			public IntPtr dwItemData;
			public IntPtr dwTypeData;
			public uint cch;
			public IntPtr hbmpItem;
		}
		[StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		internal struct WINDOWPLACEMENT {
			public int length;
			public int flags;
			public int showCmd;
			public int ptMinPosition_x;
			public int ptMinPosition_y;
			public int ptMaxPosition_x;
			public int ptMaxPosition_y;
			public int rcNormalPosition_left;
			public int rcNormalPosition_top;
			public int rcNormalPosition_right;
			public int rcNormalPosition_bottom;
		}
		internal static bool SystemParametersInfo(uint uiAction, uint uiParam, ref int bRetValue, uint fWinINI) {
			return BarUnsafeNativeMethods.SystemParametersInfo(uiAction, uiParam, ref bRetValue, fWinINI);
		}
		internal static bool GetCursorInfo(out CURSORINFO info) {
			return BarUnsafeNativeMethods.GetCursorInfo(out info);
		}
		internal static bool IsWindowEnabled(IntPtr hWnd) {
			return BarUnsafeNativeMethods.IsWindowEnabled(hWnd);
		}
		internal static int SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam) {
			return BarUnsafeNativeMethods.SendMessage(hWnd, Msg, wParam, lParam);
		}
		internal static bool EnableWindow(IntPtr hWnd, bool enable) {
			return BarUnsafeNativeMethods.EnableWindow(hWnd, enable);
		}
		internal static void OutputDebugString(string lpOutputString) {
			BarUnsafeNativeMethods.OutputDebugString(lpOutputString);
		}
		internal static bool GetCaretPos(ref NativeMethods.POINT point) {
			return BarUnsafeNativeMethods.GetCaretPos(ref point);
		}
		internal static int SendMessage(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam) {
			return BarUnsafeNativeMethods.SendMessage(hWnd, message, wParam, lParam);
		}
		internal static int PostMessage(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam) {
			return BarUnsafeNativeMethods.PostMessage(hWnd, message, wParam, lParam);
		}
		internal static int GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT placement) {
			return BarUnsafeNativeMethods.GetWindowPlacement(hWnd, ref placement);
		}
		internal static bool DestroyMenu(IntPtr menu) {
			return BarUnsafeNativeMethods.DestroyMenu(menu);
		}
		internal static bool IsZoomed(IntPtr hwnd) {
			return BarUnsafeNativeMethods.IsZoomed(hwnd);
		}
		internal static IntPtr GetFocus() {
			return BarUnsafeNativeMethods.GetFocus();
		}
		internal static short GetKeyState(System.Windows.Forms.Keys key) {
			return BarUnsafeNativeMethods.GetKeyState(key);
		}
		internal static IntPtr SetFocus(HandleRef hWnd) {
			return BarUnsafeNativeMethods.SetFocus(hWnd);
		}
		internal static IntPtr SetCapture(IntPtr hwnd) {
			return BarUnsafeNativeMethods.SetCapture(hwnd);
		}
		internal static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam) {
			return BarUnsafeNativeMethods.SendMessage(hWnd, msg, wParam, lParam);
		}
		internal static IntPtr WindowFromPoint(NativeMethods.POINT pt) {
			return BarUnsafeNativeMethods.WindowFromPoint(pt);
		}
		internal static bool IsIconic(IntPtr hWnd) {
			return BarUnsafeNativeMethods.IsIconic(hWnd);
		}
		internal static int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool fRedraw) {
			return BarUnsafeNativeMethods.SetWindowRgn(hWnd, hRgn, fRedraw);
		}
		internal static void PostQuitMessage(int exitCode) {
			BarUnsafeNativeMethods.PostQuitMessage(exitCode);
		}
		internal static IntPtr GetParent(IntPtr hWnd) {
			return BarUnsafeNativeMethods.GetParent(hWnd);
		}
		internal static IntPtr GetWindow(IntPtr hWnd, uint wCmd) {
			return BarUnsafeNativeMethods.GetWindow(hWnd, wCmd);
		}
		internal static IntPtr GetTopWindow(IntPtr hWnd) {
			return BarUnsafeNativeMethods.GetTopWindow(hWnd);
		}
		internal static IntPtr GetAncestor(IntPtr hWnd, uint wCmd) {
			return BarUnsafeNativeMethods.GetAncestor(hWnd, wCmd);
		}
		internal static short VkKeyScan(char key) {
			return BarUnsafeNativeMethods.VkKeyScan(key);
		}
		internal static IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert) {
			return BarUnsafeNativeMethods.GetSystemMenu(hWnd, bRevert);
		}
		internal static bool GetMenuItemInfo(IntPtr hMenu, uint uItem, bool fByPosition, ref BarNativeMethods.MENUITEMINFO lpmii) {
			return BarUnsafeNativeMethods.GetMenuItemInfo(hMenu, uItem, fByPosition, ref lpmii);
		}
		internal static int GetMenuItemCount(IntPtr hMenu) {
			return BarUnsafeNativeMethods.GetMenuItemCount(hMenu);
		}
		internal static bool DeleteMenu(IntPtr hMenu, int pos, int flags) {
			return BarUnsafeNativeMethods.DeleteMenu(hMenu, pos, flags);
		}
		internal static int GetMenuString(IntPtr hMenu, uint uIDItem, IntPtr lpString, int nMaxCount, int uFlag) {
			return BarUnsafeNativeMethods.GetMenuString(hMenu, uIDItem, lpString, nMaxCount, uFlag);
		}
		internal static IntPtr GetMenu(IntPtr handle) {
			return BarUnsafeNativeMethods.GetMenu(handle);
		}
		internal static IntPtr SetMenu(IntPtr handle, IntPtr menu) {
			return BarUnsafeNativeMethods.SetMenu(handle, menu);
		}
		internal static IntPtr CreateMenu() {
			return BarUnsafeNativeMethods.CreateMenu();
		}
		internal static bool RemoveMenu(IntPtr hMenu, int pos, int flags) {
			return BarUnsafeNativeMethods.RemoveMenu(hMenu, pos, flags);
		}
		internal static IntPtr GetWindowDC(IntPtr hWnd) {
			return BarUnsafeNativeMethods.GetWindowDC(hWnd);
		}
		internal static IntPtr CreateDC(string lpszDriver, string lpszDevice,string lpszOutput, IntPtr lpInitData) {
			return BarUnsafeNativeMethods.CreateDC(lpszDriver, lpszDevice, lpszOutput, lpInitData);
		}
		internal static int ReleaseDC(IntPtr hWnd, IntPtr hDC) {
			return BarUnsafeNativeMethods.ReleaseDC(hWnd, hDC);
		}
		internal static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags) {
			return BarUnsafeNativeMethods.SetWindowPos(hWnd, hWndInsertAfter, X, Y, cx, cy, uFlags);
		}
		internal static IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref BarNativeMethods.TVITEM lParam) {
			return BarUnsafeNativeMethods.SendMessage(hWnd, Msg, wParam, ref lParam);
		}
		internal static int LockWindowUpdate(IntPtr hWnd) {
			return BarUnsafeNativeMethods.LockWindowUpdate(hWnd);
		}
		internal static IntPtr GetForegroundWindow() {
			return BarUnsafeNativeMethods.GetForegroundWindow();
		}
		public static object PtrToStructure(IntPtr ptr, Type structureType) {
			return Marshal.PtrToStructure(ptr, structureType);
		}
		public static int SizeOf(Type t) {
			return Marshal.SizeOf(t);
		}
		public static int SizeOf(object structure) {
			return Marshal.SizeOf(structure);
		}
		public static int ReadInt32(IntPtr ptr, int ofs) {
			return Marshal.ReadInt32(ptr, ofs);
		}
		public static void WriteInt32(IntPtr ptr, int ofs, int val) {
			Marshal.WriteInt32(ptr, ofs, val);
		}
		public static IntPtr AllocCoTaskMem(int cb) {
			return Marshal.AllocCoTaskMem(cb);
		}
		public static string PtrToStringAnsi(IntPtr ptr) {
			return Marshal.PtrToStringAnsi(ptr);
		}
		public static void FreeCoTaskMem(IntPtr ptr) {
			Marshal.FreeCoTaskMem(ptr);
		}
		public static void StructureToPtr(object structure, IntPtr ptr, bool fDeleteOld) {
			Marshal.StructureToPtr(structure, ptr, fDeleteOld);
		}
		public static int GetDeviceCaps(IntPtr hdc, int index) {
			return BarUnsafeNativeMethods.GetDeviceCaps(hdc, index);
		}
		internal static bool HasNoMessageQueue() {
			int threadId = DevExpress.Utils.Win.Hook.HookManager.GetCurrentThreadId();
			if(!BarUnsafeNativeMethods.PostThreadMessage(threadId, 0x0000 , IntPtr.Zero, IntPtr.Zero)) {
				if(Marshal.GetLastWin32Error() == 0x5A4)
					return true;
			}
			return false;
		}
		static class BarUnsafeNativeMethods {
			[DllImport("user32.dll")]
			internal static extern bool GetCursorInfo(out CURSORINFO info);
			[DllImport("user32.dll")]
			internal static extern short GetKeyState(System.Windows.Forms.Keys key);
			[DllImport("user32.dll")]
			internal static extern int LockWindowUpdate(IntPtr hWnd);
			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref BarNativeMethods.TVITEM lParam);
			[DllImport("user32.dll")]
			internal static extern IntPtr GetWindowDC(IntPtr hWnd);
			[DllImport("gdi32.dll")]
			internal static extern IntPtr CreateDC(string lpszDriver, string lpszDevice,
			   string lpszOutput, IntPtr lpInitData);
			[DllImport("user32.dll")]
			internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
			[DllImport("user32.dll")]
			internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern IntPtr GetMenu(IntPtr handle);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern IntPtr SetMenu(IntPtr handle, IntPtr menu);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern IntPtr CreateMenu();
			[DllImport("USER32.dll")]
			internal static extern bool RemoveMenu(IntPtr hMenu, int pos, int flags);
			[DllImport("USER32.dll")]
			internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
			[DllImport("USER32.dll")]
			internal static extern bool GetMenuItemInfo(IntPtr hMenu, uint uItem, bool fByPosition, ref BarNativeMethods.MENUITEMINFO lpmii);
			[DllImport("USER32.dll")]
			internal static extern int GetMenuItemCount(IntPtr hMenu);
			[DllImport("USER32.dll")]
			internal static extern bool DeleteMenu(IntPtr hMenu, int pos, int flags);
			[DllImport("USER32.dll")]
			internal static extern int GetMenuString(IntPtr hMenu, uint uIDItem, IntPtr lpString, int nMaxCount, int uFlag);
			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			internal static extern short VkKeyScan(char key);
			[DllImport("User32.dll")]
			internal static extern IntPtr GetAncestor(IntPtr hWnd, uint wCmd);
			[DllImport("User32.dll")]
			internal static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);
			[DllImport("User32.dll")]
			internal static extern IntPtr GetTopWindow(IntPtr hWnd);
			[DllImport("user32.dll")]
			internal static extern IntPtr GetParent(IntPtr hWnd);
			[DllImport("user32.dll")]
			internal static extern void PostQuitMessage(int exitCode);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool fRedraw);
			[DllImport("USER32.dll")]
			internal static extern bool IsIconic(IntPtr hWnd);
			[DllImport("user32.dll")]
			internal static extern IntPtr WindowFromPoint(NativeMethods.POINT pt);
			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern IntPtr GetForegroundWindow();
			[DllImport("USER32.dll")]
			internal static extern IntPtr SetCapture(IntPtr hwnd);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern IntPtr GetFocus();
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern IntPtr SetFocus(HandleRef hWnd);
			[DllImport("USER32.dll")]
			internal static extern bool IsZoomed(IntPtr hwnd);
			[DllImport("USER32.dll")]
			internal static extern bool DestroyMenu(IntPtr menu);
			[DllImport("USER32.dll")]
			internal static extern int GetWindowPlacement(IntPtr hWnd, ref BarNativeMethods.WINDOWPLACEMENT placement);
			[DllImport("USER32.dll")]
			internal static extern bool GetCaretPos(ref NativeMethods.POINT point);
			[DllImport("USER32.dll")]
			internal static extern int SendMessage(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam);
			[DllImport("USER32.dll")]
			internal static extern int PostMessage(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam);
			[DllImport("USER32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool PostThreadMessage(int threadId, int message, IntPtr wParam, IntPtr lParam);
			[DllImport("KERNEL32.dll")]
			internal static extern void OutputDebugString(string lpOutputString);
			[DllImport("User32.dll")]
			internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int bRetValue, uint fWinINI);
			[DllImport("user32.dll")]
			internal static extern bool IsWindowEnabled(IntPtr hWnd);
			[DllImport("user32.dll")]
			internal static extern int SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
			[DllImport("user32.dll")]
			internal static extern bool EnableWindow(IntPtr hWnd, bool enable);
			[DllImport("gdi32.dll")]
			internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
		}
	}
	public static class DPISettings {
		const int LOGPIXELSX = 88;
		const int LOGPIXELSY = 90;
		const int DESKTOPVERTRES = 117;
		const int DESKTOPHORZRES = 118;
		static int? dpiX;
		public static int DpiX {
			get {
				if(!dpiX.HasValue)
					EnsureDPISettings();
				return dpiX.Value;
			}
		}
		static int? dpiY;
		public static int DpiY {
			get {
				if(!dpiY.HasValue)
					EnsureDPISettings();
				return dpiY.Value;
			}
		}
		static int? hRes;
		public static int HRes {
			get {
				if(!hRes.HasValue)
					EnsureDPISettings();
				return hRes.Value;
			}
		}
		static int? vRes;
		public static int VRes {
			get {
				if(!vRes.HasValue)
					EnsureDPISettings();
				return vRes.Value;
			}
		}
		public static void Reset() {
			dpiX = null;
			dpiY = null;
			hRes = null;
			vRes = null;
		}
		static void EnsureDPISettings() {
			IntPtr hdcDesktop = NativeMethods.GetDC(IntPtr.Zero);
			try {
				dpiX = BarNativeMethods.GetDeviceCaps(hdcDesktop, LOGPIXELSX);
				dpiY = BarNativeMethods.GetDeviceCaps(hdcDesktop, LOGPIXELSY);
				hRes = BarNativeMethods.GetDeviceCaps(hdcDesktop, DESKTOPHORZRES);
				vRes = BarNativeMethods.GetDeviceCaps(hdcDesktop, DESKTOPVERTRES);
			}
			finally { NativeMethods.ReleaseDC(IntPtr.Zero, hdcDesktop); }
		}
		internal static Rectangle CheckRelativeScreenBounds(System.Windows.Forms.Control ctrl, Rectangle bounds) {
			return ctrl.IsHandleCreated ? CheckRelativeScreenBounds(ctrl.Handle, bounds) : bounds;
		}
		internal static Rectangle CheckRelativeScreenBounds(IntPtr hWnd, Rectangle bounds) {
			IntPtr hdc = NativeMethods.GetDC(hWnd);
			try {
				int wndDpiX = BarNativeMethods.GetDeviceCaps(hdc, LOGPIXELSX);
				int wndDpiY = BarNativeMethods.GetDeviceCaps(hdc, LOGPIXELSY);
				var screen = System.Windows.Forms.Screen.FromHandle(hWnd);
				Rectangle screenRect = screen.Bounds;
				IntPtr displayDC = BarNativeMethods.CreateDC("DISPLAY", screen.DeviceName, null, IntPtr.Zero);
				try {
					int dpiX = BarNativeMethods.GetDeviceCaps(displayDC, LOGPIXELSX);
					int dpiY = BarNativeMethods.GetDeviceCaps(displayDC, LOGPIXELSY);
					int hRes = BarNativeMethods.GetDeviceCaps(displayDC, DESKTOPHORZRES);
					int vRes = BarNativeMethods.GetDeviceCaps(displayDC, DESKTOPVERTRES);
					double hResFactor = (double)hRes / (double)screenRect.Width;
					double vResFactor = (double)vRes / (double)screenRect.Height;
					double dpiXFactor = ((double)dpiX) / (double)wndDpiX;
					double dpiYFactor = ((double)dpiY) / (double)wndDpiY;
					double xScaling = hResFactor * dpiXFactor;
					double yScaling = vResFactor * dpiYFactor;
					return new Rectangle(
									Round((double)bounds.X * xScaling),
									Round((double)bounds.Y * yScaling),
									Round((double)bounds.Width * xScaling),
									Round((double)bounds.Height * yScaling)
								);
				}
				finally { NativeMethods.DeleteDC(displayDC); }
			}
			finally { NativeMethods.ReleaseDC(hWnd, hdc); }
		}
		static int Round(double value) {
			return (value >= 0) ? (int)(value + 0.5) : (int)(value - 0.5);
		}
	}
}
