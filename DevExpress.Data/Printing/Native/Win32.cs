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
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.XtraPrinting.Native {
#endif
	public static class Win32 {
		public static int EM_CHARFROMPOS = 0x00D7;
		public static int EM_REPLACESEL = 0x00C2;
		public const int
			GWL_EXSTYLE = (-20),
			WS_EX_TOOLWINDOW = 0x00000080,
			WS_EX_CLIENTEDGE = 0x00000200,
			WS_EX_STATICEDGE = 0x00020000,
			WS_BORDER = 0x00800000,
			WM_DESTROY = 0x0002,
			WM_SETFOCUS = 0x0007,
			WM_KILLFOCUS = 0x0008,
			WM_CONTEXTMENU = 0x007B,
			WM_HSCROLL = 0x0114,
			WM_VSCROLL = 0x0115,
			WM_MOUSEACTIVATE = 0x0021,
			WM_MOUSEFIRST = 0x0200,
			WM_MOUSEMOVE = 0x0200,
			WM_LBUTTONDOWN = 0x0201,
			WM_LBUTTONUP = 0x0202,
			WM_LBUTTONDBLCLK = 0x0203,
			WM_RBUTTONDOWN = 0x0204,
			WM_RBUTTONUP = 0x0205,
			WM_RBUTTONDBLCLK = 0x0206,
			WM_MBUTTONDOWN = 0x0207,
			WM_MBUTTONUP = 0x0208,
			WM_MBUTTONDBLCLK = 0x0209,
			WM_XBUTTONDOWN = 0x020B,
			WM_XBUTTONUP = 0x020C,
			WM_XBUTTONDBLCLK = 0x020D,
			WM_MOUSEWHEEL = 0x020A,
			WM_MOUSELAST = 0x020A,
			WM_CAPTURECHANGED = 0x0215,
			WM_USER = 1024,
			EM_FORMATRANGE = WM_USER + 57,
			EM_SETZOOM = WM_USER + 225,
			MM_ISOTROPIC = 7,
			SC_SIZE = 0xF000,
			WM_SYSCOMMAND = 0x112,
			ROP_DSTINVERT = 0x00550009;
		[StructLayout(LayoutKind.Sequential)]
		public class SIZE {
			public int cx;
			public int cy;
			public SIZE() {
			}
			public SIZE(int cx, int cy) {
				this.cx = cx;
				this.cy = cy;
			}
		}
		public struct STRUCT_CHARRANGE {
			public int cpMin;
			public int cpMax;
		}
		public struct STRUCT_RECT {
			public int left;
			public int top;
			public int right;
			public int bottom;
		}
		public struct STRUCT_FORMATRANGE {
			public IntPtr hdc;
			public IntPtr hdcTarget;
			public STRUCT_RECT rc;
			public STRUCT_RECT rcPage;
			public STRUCT_CHARRANGE chrg;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct PARAFORMAT2 {
			public int cbSize;
			public int dwMask;
			public short wNumbering;
			public short wReserved;
			public int dxStartIndent;
			public int dxRightIndent;
			public int dxOffset;
			public short wAlignment;
			public short cTabCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public int[] rgxTabs;
			public int dySpaceBefore;
			public int dySpaceAfter;
			public int dyLineSpacing;
			public short sStyle;
			public byte bLineSpacingRule;
			public byte bOutlineLevel;
			public short wShadingWeight;
			public short wShadingStyle;
			public short wNumberingStart;
			public short wNumberingStyle;
			public short wNumberingTab;
			public short wBorderSpace;
			public short wBorderWidth;
			public short wBorders;
		}
		[StructLayout(LayoutKind.Sequential)]
		internal struct CharFormat2 {
			public int cbSize;
			public int dwMask;
			public int dwEffects;
			public int yHeight;
			public int yOffset;
			public int crTextColor;
			public byte bCharSet;
			public byte bPitchAndFamily;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
			public string szFaceName;
			public short wWeight;
			public short sSpacing;
			public int crBackColor;
			public int lcid;
			public int dwReserved;
			public short sStyle;
			public short wKerning;
			public byte bUnderlineType;
			public byte bAnimation;
			public byte bRevAuthor;
			public byte bReserved1;
		}
		public static IntPtr MakeLParam(int low, int high) {
			return (IntPtr)((high << 16) | (low & 0xffff));
		}
		public static int HiWord(IntPtr n) {
			return HiWord(unchecked((int)(long)n));
		}
		static int HiWord(int n) {
			return (n >> 16) & 0xffff;
		}
		public static int LoWord(IntPtr n) {
			return LoWord(unchecked((int)(long)n));
		}
		static int LoWord(int n) {
			return n & 0xffff;
		}
		[DllImport("user32.dll", EntryPoint = "MoveWindow")]
		[System.Security.SuppressUnmanagedCodeSecurity]
		static extern Boolean MoveWindowCore(IntPtr hWnd, Int32 x, Int32 y, Int32 width, Int32 height, Boolean needRepaint);
		[System.Security.SecuritySafeCritical]
		public static Boolean MoveWindow(IntPtr hWnd, Int32 x, Int32 y, Int32 width, Int32 height, Boolean needRepaint) { return MoveWindowCore(hWnd, x, y, width, height, needRepaint); }
		[DllImport("user32.dll", EntryPoint = "GetActiveWindow")]
		static extern IntPtr GetActiveWindowCore();
		[System.Security.SecuritySafeCritical]
		public static IntPtr GetActiveWindow() { return GetActiveWindowCore(); }
		[DllImport("user32.dll", EntryPoint = "SetActiveWindow")]
		static extern IntPtr SetActiveWindowCore(IntPtr hWnd);
		[System.Security.SecuritySafeCritical]
		public static IntPtr SetActiveWindow(IntPtr hWnd) { return SetActiveWindowCore(hWnd); }
		[DllImport("user32.dll", EntryPoint = "GetWindowLong")]
		static extern int GetWindowLongCore(IntPtr hWnd, int nIndex);
		[System.Security.SecuritySafeCritical]
		public static int GetWindowLong(IntPtr hWnd, int nIndex) { return GetWindowLongCore(hWnd, nIndex); }
		[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
		static extern IntPtr SetWindowLongCore(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
		[System.Security.SecuritySafeCritical]
		public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong) { return SetWindowLongCore(hWnd, nIndex, dwNewLong); }
		[DllImport("user32.dll", EntryPoint = "ShowScrollBar")]
		static extern bool ShowScrollBarCore(IntPtr hWnd, int wBar, bool bShow);
		[System.Security.SecuritySafeCritical]
		public static bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow) { return ShowScrollBarCore(hWnd, wBar, bShow); }
		[DllImport("user32.dll", EntryPoint = "SendMessage")]
		static extern int SendMessageCore(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
		[System.Security.SecuritySafeCritical]
		public static int SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam) { return SendMessageCore(hWnd, msg, wParam, lParam); }
		[DllImport("user32", EntryPoint = "SendMessage")]
		static extern IntPtr SendMessageCore(HandleRef hWnd, int msg, int wParam, IntPtr lParam);
		[System.Security.SecuritySafeCritical]
		public static IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, IntPtr lParam) { return SendMessageCore(hWnd, msg, wParam, lParam); }
		[DllImport("user32", EntryPoint = "SendMessage")]
		static extern IntPtr SendMessageCore(HandleRef hWnd, int msg, int wParam, string s);
		[System.Security.SecuritySafeCritical]
		public static IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, string s) { return SendMessageCore(hWnd, msg, wParam, s); }
		[DllImport("gdi32.dll", EntryPoint = "CreateDC")]
		static extern IntPtr CreateDCCore(string lpszDriver, string lpszDevice, IntPtr lpszOutput, IntPtr lpInitData);
		[System.Security.SecuritySafeCritical]
		public static IntPtr CreateDC(string lpszDriver, string lpszDevice, IntPtr lpszOutput, IntPtr lpInitData) { return CreateDCCore(lpszDriver, lpszDevice, lpszOutput, lpInitData); }
		[DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
		static extern bool DeleteDCCore(IntPtr hdc);
		[System.Security.SecuritySafeCritical]
		public static bool DeleteDC(IntPtr hdc) { return DeleteDCCore(hdc); }
		[DllImport("gdi32.dll", EntryPoint = "SetMapMode")]
		static extern int SetMapModeCore(HandleRef hDC, int nMapMode);
		[System.Security.SecuritySafeCritical]
		public static int SetMapMode(HandleRef hDC, int nMapMode) { return SetMapModeCore(hDC, nMapMode); }
		[DllImport("gdi32.dll", EntryPoint = "SetWindowExtEx")]
		static extern bool SetWindowExtExCore(HandleRef hDC, int x, int y, [In, Out] SIZE size);
		[System.Security.SecuritySafeCritical]
		public static bool SetWindowExtEx(HandleRef hDC, int x, int y, SIZE size) { return SetWindowExtExCore(hDC, x, y, size); }
		[DllImport("gdi32.dll", EntryPoint = "SetViewportExtEx")]
		static extern bool SetViewportExtExCore(HandleRef hDC, int x, int y, SIZE size);
		[System.Security.SecuritySafeCritical]
		public static bool SetViewportExtEx(HandleRef hDC, int x, int y, SIZE size) { return SetViewportExtExCore(hDC, x, y, size); }
		[DllImport("gdi32.dll", EntryPoint = "SetViewportOrgEx")]
		static extern bool SetViewportOrgExCore(IntPtr hDC, int x, int y, [In, Out] SIZE size);
		[System.Security.SecuritySafeCritical]
		public static bool SetViewportOrgEx(IntPtr hDC, int x, int y, [In, Out] SIZE size) { return SetViewportOrgExCore(hDC, x, y, size); }
		[DllImport("gdi32.dll", EntryPoint = "BitBlt")]
		static extern int BitBltCore(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
		[System.Security.SecuritySafeCritical]
		public static int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop) { return BitBltCore(hDC, x, y, nWidth, nHeight, hSrcDC, xSrc, ySrc, dwRop); }
		[DllImport("user32.dll", EntryPoint = "GetAsyncKeyState")]
		static extern short GetAsyncKeyStateCore(int keyCode);
		[System.Security.SecuritySafeCritical]
		public static short GetAsyncKeyState(int keyCode) { return GetAsyncKeyStateCore(keyCode); }
		[DllImport("user32.dll", EntryPoint = "WaitMessage")]
		static extern void WaitMessageCore();
		[System.Security.SecuritySafeCritical]
		public static void WaitMessage() { WaitMessageCore(); }
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true, EntryPoint = "GlobalAlloc")]
		static extern IntPtr GlobalAllocCore(int uFlags, IntPtr dwBytes);
		[System.Security.SecuritySafeCritical]
		public static IntPtr GlobalAlloc(int uFlags, IntPtr dwBytes) { return GlobalAllocCore(uFlags, dwBytes); }
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true, EntryPoint = "GlobalFree")]
		static extern IntPtr GlobalFreeCore(HandleRef handle);
		[System.Security.SecuritySafeCritical]
		public static IntPtr GlobalFree(HandleRef handle) { return GlobalFreeCore(handle); }
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true, EntryPoint = "GlobalLock")]
		static extern IntPtr GlobalLockCore(HandleRef handle);
		[System.Security.SecuritySafeCritical]
		public static IntPtr GlobalLock(HandleRef handle) { return GlobalLockCore(handle); }
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true, EntryPoint = "GlobalUnlock")]
		static extern bool GlobalUnlockCore(HandleRef handle);
		[System.Security.SecuritySafeCritical]
		public static bool GlobalUnlock(HandleRef handle) { return GlobalUnlockCore(handle); }
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, EntryPoint = "SetFocus")]
		static extern IntPtr SetFocusCore(HandleRef hWnd);
		[System.Security.SecuritySafeCritical]
		public static IntPtr SetFocus(HandleRef hWnd) { return SetFocusCore(hWnd); }
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, EntryPoint = "GetFocus")]
		static extern IntPtr GetFocusCore();
		[System.Security.SecuritySafeCritical]
		public static IntPtr GetFocus() { return GetFocusCore(); }
		public static MouseButtons GetMouseButtons() {
			MouseButtons buttons = MouseButtons.None;
			if(GetAsyncKeyState(1) < 0)
				buttons |= MouseButtons.Left;
			if(GetAsyncKeyState(2) < 0)
				buttons |= MouseButtons.Right;
			if(GetAsyncKeyState(4) < 0)
				buttons |= MouseButtons.Middle;
			if(GetAsyncKeyState(5) < 0)
				buttons |= MouseButtons.XButton1;
			if(GetAsyncKeyState(6) < 0)
				buttons |= MouseButtons.XButton2;
			return buttons;
		}
	}
}
