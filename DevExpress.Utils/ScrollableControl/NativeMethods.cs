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
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.Utils.Drawing.Helpers {
	public static class StockIconHelper {
		public enum StockIconId : int {
			None = 0,
			Application = 2,
			Question = 23,
			Shield = 77,
			Warning = 78,
			Asterisk = 79,
			Error = 80,
			Exclamation = Warning,
			Hand = Error,
			Information = Asterisk,
			WinLogo = Application
		}
		public enum SHGSI : int {
			SHGSI_ICONLOCATION = 0,
			SHGSI_ICON = 0x000000100,
			SHGSI_SYSICONINDEX = 0x000004000,
			SHGSI_LINKOVERLAY = 0x000008000,
			SHGSI_SELECTED = 0x000010000,
			SHGSI_LARGEICON = 0x000000000,
			SHGSI_SMALLICON = 0x000000001,
			SHGSI_SHELLICONSIZE = 0x000000004
		}
		[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct SHSTOCKICONINFO {
			public Int32 cbSize;
			public IntPtr hIcon;
			public Int32 iSysIconIndex;
			public Int32 iIcon;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szPath;
		}
		public static Icon GetWindows8AssociatedIcon(Icon systemIcon) {
			StockIconId iconID = GetStockIconId(systemIcon);
			if(iconID == StockIconId.None) return null;
			return GetStockIcon(iconID);
		}
		public static StockIconId GetStockIconId(Icon systemIcon) {
			if(systemIcon == SystemIcons.Application) return StockIconId.Application;
			if(systemIcon == SystemIcons.Asterisk) return StockIconId.Asterisk;
			if(systemIcon == SystemIcons.Error) return StockIconId.Error;
			if(systemIcon == SystemIcons.Exclamation) return StockIconId.Exclamation;
			if(systemIcon == SystemIcons.Hand) return StockIconId.Hand;
			if(systemIcon == SystemIcons.Information) return StockIconId.Information;
			if(systemIcon == SystemIcons.Question) return StockIconId.Question;
			if(systemIcon == SystemIcons.Warning) return StockIconId.Warning;
			if(systemIcon == SystemIcons.WinLogo) return StockIconId.WinLogo;
			return StockIconId.None;
		}
		static Dictionary<StockIconId, Icon> iconCache;
		static Dictionary<StockIconId, Icon> IconCache {
			get {
				if(iconCache == null) {
					iconCache = new Dictionary<StockIconId, Icon>();
				}
				return iconCache;
			}
		}
		public static Icon GetStockIcon(StockIconId iconId) {
			Icon icon;
			if(!IconCache.TryGetValue(iconId, out icon)) {
				icon = LoadStockIcon(iconId);
				IconCache.Add(iconId, icon);
			}
			return icon;
		}
		public static void Reset() {
			foreach(var value in IconCache.Values) {
				value.Dispose();
			}
			IconCache.Clear();
		}
		[System.Security.SecuritySafeCritical]
		static Icon LoadStockIcon(StockIconId iconId) {
			SHSTOCKICONINFO sii = new SHSTOCKICONINFO();
			sii.cbSize = (Int32)Marshal.SizeOf(typeof(SHSTOCKICONINFO));
			Marshal.ThrowExceptionForHR(SHGetStockIconInfo(iconId, SHGSI.SHGSI_ICON, ref sii));
			Icon icon = null;
			if(sii.hIcon != IntPtr.Zero) {
				icon = Icon.FromHandle(sii.hIcon).Clone() as Icon;
				NativeMethods.DestroyIcon(sii.hIcon);
			}
			return icon;
		}
		[DllImport("Shell32.dll", SetLastError = false)]
		public static extern Int32 SHGetStockIconInfo(StockIconId siid, SHGSI uFlags, ref SHSTOCKICONINFO psii);
	}
	public class MouseEventFlag {
		public const int
		TME_NONCLIENT = 0x00000010,
		TME_LEAVE = 0x00000002;
	}
	public class MSG {
		public const int
			WM_DWMCOMPOSITIONCHANGED = 0x031E,
			SIZE_RESTORED = 0,
			WM_DESTROY = 0x02,
			VK_LBUTTON = 0x01,
			VK_RBUTTON = 0x02,
			MK_LBUTTON = 0x0001, MK_RBUTTON = 0x0002,
			WA_INACTIVE = 0,
			WM_SETCURSOR = 0x0020,
			WM_MOUSEACTIVATE = 0x0021,
			WM_ACTIVATE = 0x0006,
			WM_SETFOCUS = 0x0007,
			WM_KILLFOCUS = 0x0008,
			WM_MDICREATE = 0x0220,
			WM_MDIDESTROY = 0x0221,
			WM_MDIACTIVATE = 0x0222,
			WM_MDIRESTORE = 0x0223,
			WM_MDINEXT = 0x0224,
			WM_MDIMAXIMIZE = 0x0225,
			WM_MDISETMENU = 0x0230,
			WM_CONTEXTMENU = 0x007B,
			WM_SYSCOLORCHANGE = 0x15,
			WM_EXITMENULOOP = 530,
			WM_MENUCHAR = 288,
			WM_SYSCHAR = 0x0106,
			WM_VSCROLL = 0x115,
			WM_HSCROLL = 0x114,
			WM_COMMAND = 273,
			WM_MENUSELECT = 0x011F,
			WM_CHILDACTIVATE = 0x0022,
			WM_NCCALCSIZE = 0x0083,
			WM_GETMINMAXINFO = 0x24,
			WM_ENABLE = 0x000A,
			WM_NCHITTEST = 0x0084,
			WM_NCRBUTTONDOWN = 0x00A4,
			WM_NCRBUTTONUP = 0x00A5,
			WM_NCRBUTTONDBLCLK = 0x00A6,
			WM_NCLBUTTONDOWN = 0x00A1,
			WM_NCLBUTTONDBLCLK = 0x00A3,
			WM_NCLBUTTONUP = 0x00A2,
			WM_NCMBUTTONDOWN = 0x00A7,
			WM_NCMBUTTONUP = 0x00A8,
			WM_NCMBUTTONDBLCLK = 0x00A9,
			WM_NCMOUSEMOVE = 0x00A0,
			WM_NCMOUSELEAVE = 0x02A2,
			WM_MOUSELEAVE = 675,
			WM_MOUSEHOVER = 673,
			WM_NCMOUSEHOVER = 0x02A0,
			WM_NCPAINT = 0x0085,
			WM_NCACTIVATE = 0x0086,
			WM_SETICON = 0x0080,
			WM_GETTEXT = 0x000D,
			WM_SETTEXT = 0x000C,
			WM_LBUTTONDOWN = 0x0201,
			WM_LBUTTONUP = 0x0202,
			WM_LBUTTONDBLCLK = 0x0203,
			WM_RBUTTONDOWN = 0x0204,
			WM_RBUTTONUP = 0x0205,
			WM_RBUTTONDBLCLK = 0x0206,
			WM_MBUTTONDOWN = 0x0207,
			WM_MBUTTONUP = 0x0208,
			WM_MBUTTONDBLCLK = 0x0209,
			WM_MOUSEHWHEEL = 0x20e,
			WM_MOUSEWHEEL = 0x020A,
			WM_MOUSEMOVE = 0x0200,
			WM_PRINTCLIENT = 0x0318,
			WM_IME_NOTIFY = 642,
			WM_DEADCHAR = 0x103,
			WM_SYSKEYDOWN = 0x104,
			WM_KEYUP = 257,
			WM_KEYDOWN = 256,
			WM_CAPTURECHANGED = 0x215,
			WM_SYSCOMMAND = 0x0112,
			WM_SYSKEYUP = 0x0105,
			WM_CHAR = 0x0102,
			WM_SIZE = 5,
			WM_SIZING = 0x0214,
			WM_ENTERSIZEMOVE = 0x0231,
			WM_EXITSIZEMOVE = 0x0232,
			WM_SYNCPAINT = 0x0088,
			WM_PAINT = 0x000F,
			WM_PRINT = 0x0317,
			WM_ERASEBKGND = 0x0014,
			WM_SHOWWINDOW = 0x18,
			WM_NCCREATE = 0x0081,
			WM_MOVE = 0x0003,
			WM_ACTIVATEAPP = 28,
			WM_APP = 0x8000,
			WM_CREATE = 0x0001,
			WM_WINDOWPOSCHANGING = 0x0046,
			WM_WINDOWPOSCHANGED = 0x0047,
			WM_USER = 0x0400,
			WM_NCUAHDRAWCAPTION = 0x00AE,
			WM_NCUAHDRAWFRAME = 0x00AF,
			WM_IME_STARTCOMPOSITION = 0x010D,
			WM_IME_ENDCOMPOSITION = 0x010E,
			WM_IME_COMPOSITION = 0x010F,
			WM_IME_KEYLAST = 0x010F,
			WM_XREDRAW = WM_USER + 100,
			WM_XREDRAWC = WM_USER + 101,
			WM_USER7441 = WM_USER + 7441,
			WM_QUERYOPEN = 0x0013;
	}
	[System.Security.SecuritySafeCritical]
	public class NativeVista {
		#region Structs&Enums
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct XPMARGINS {
			public Int32 cxLeftWidth;	  
			public Int32 cxRightWidth;	 
			public Int32 cyTopHeight;	  
			public Int32 cyBottomHeight;   
			public Rectangle ToRectangle() {
				return new Rectangle(cxLeftWidth, cyTopHeight, cxRightWidth, cyBottomHeight);
			}
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		struct OSVERSIONINFOEX {
			public int dwOSVersionInfoSize;
			public int dwMajorVersion;
			public int dwMinorVersion;
			public int dwBuildNumber;
			public int dwPlatformId;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szCSDVersion;
			public UInt16 wServicePackMajor;
			public UInt16 wServicePackMinor;
			public UInt16 wSuiteMask;
			public byte wProductType;
			public byte wReserved;
		}
		[StructLayout(LayoutKind.Sequential)]
		struct DTTOPTS {
			public int dwSize;
			public int dwFlags;
			public int crText;
			public int crBorder;
			public int crShadow;
			public int iTextShadowType;
			public NativeMethods.POINT ptShadowOffset;
			public int iBorderSize;
			public int iFontPropId;
			public int iColorPropId;
			public int iStateId;
			public bool fApplyOverlay;
			public int iGlowSize;
			public int pfnDrawTextCallback;
			public IntPtr lParam;
		}
		public enum DWMWINDOWATTRIBUTE {
			DWMWA_NCRENDERING_ENABLED = 1,
			DWMWA_NCRENDERING_POLICY,
			DWMWA_TRANSITIONS_FORCEDISABLED,
			DWMWA_ALLOW_NCPAINT,
			DWMWA_CAPTION_BUTTON_BOUNDS,
			DWMWA_NONCLIENT_RTL_LAYOUT,
			DWMWA_FORCE_ICONIC_REPRESENTATION,
			DWMWA_FLIP3D_POLICY,
			DWMWA_EXTENDED_FRAME_BOUNDS,
			DWMWA_HAS_ICONIC_BITMAP,
			DWMWA_DISALLOW_PEEK,
			DWMWA_EXCLUDED_FROM_PEEK,
			DWMWA_LAST
		}
		public enum DWMNCRenderingPolicy {
			DWMNCRP_USEWINDOWSTYLE, 
			DWMNCRP_DISABLED, 
			DWMNCRP_ENABLED, 
			DWMNCRP_LAST
		}
		public enum BP_BUFFERFORMAT { 
			BPBF_COMPATIBLEBITMAP, 
			BPBF_DIB, 
			BPBF_TOPDOWNDIB, 
			BPBF_TOPDOWNMONODIB 
		};
		[StructLayout(LayoutKind.Sequential)]
		public struct BP_PAINTPARAMS {
			public int cbSize;
			public int dwFlags;
			IntPtr prcExclude;
			IntPtr pBlendFunction;
		}
		#endregion Structs&Enums
		#region SecurityCritical
		static class UnsafeNativeVista {
			[DllImport("kernel32", SetLastError = true)]
			internal static extern bool GetVersionEx(ref OSVERSIONINFOEX osvi);
			[DllImport("UxTheme.dll")]
			internal static extern IntPtr BeginBufferedPaint(IntPtr pHdcTarget, IntPtr lpRect, IntPtr bufferFormat, IntPtr bpPaintParams, ref IntPtr pHdc);
			[DllImport("UxTheme.dll", CharSet = CharSet.Auto)]
			internal static extern IntPtr BeginBufferedPaint(IntPtr hdcTarget, ref NativeMethods.RECT prcTarget, BP_BUFFERFORMAT dwFormat, IntPtr pPaintParams, ref IntPtr hdc);
			[DllImport("UxTheme.dll")]
			internal static extern IntPtr BufferedPaintSetAlpha(IntPtr hBufferedPaint, IntPtr prc, byte alpha);
			[DllImport("UxTheme.dll")]
			internal static extern IntPtr EndBufferedPaint(IntPtr hBufferedPaint, IntPtr fUpdateTarget);
			[DllImport("UxTheme.dll", CharSet = CharSet.Auto)]
			internal static extern int EndBufferedPaint(IntPtr hBufferedPaint, bool fUpdateTarget);
			[DllImport("UxTheme.dll", CharSet = CharSet.Auto)]
			internal static extern void BufferedPaintInit();
			[DllImport("UxTheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
			internal extern static Int32 GetThemeTextExtent(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, String text, int textLength, int textFlags, ref NativeMethods.RECT boundingRect, out NativeMethods.RECT extentRect);
			[DllImport("UxTheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
			internal extern static Int32 GetThemeTextExtent(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, String text, int textLength, int textFlags, IntPtr boundsingRect, out NativeMethods.RECT extentRect);
			[DllImport("UxTheme.dll", CharSet = CharSet.Unicode)]
			internal static extern int DrawThemeTextEx(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref NativeMethods.RECT pRect, ref DTTOPTS pOptions);
			[DllImport("UxTheme.dll", CharSet = CharSet.Unicode)]
			internal static extern int GetCurrentThemeName([MarshalAs(UnmanagedType.LPWStr)]StringBuilder pszThemeFileName, Int32 dwMaxNameChars, IntPtr pszColorBuff, Int32 cchMaxColorChars, IntPtr pszSizeBuff, Int32 cchMaxSizeChars);
			[DllImport("UxTheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal extern static IntPtr OpenThemeData(IntPtr hwnd, [MarshalAs(UnmanagedType.LPTStr)]string pszClassList);
			[DllImport("UxTheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal extern static IntPtr CloseThemeData(IntPtr hTheme);
			[DllImport("UxTheme.dll")]
			internal extern static int GetThemeAppProperties();
			[DllImport("UxTheme.dll")]
			internal extern static bool IsAppThemed();
			[DllImport("uxtheme.dll")]
			internal extern static void SetThemeAppProperties(int dwFlags);
			[DllImport("uxtheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal extern static IntPtr DrawThemeParentBackground(IntPtr hwnd, IntPtr hdc, ref NativeMethods.RECT rect);
			[DllImport("uxtheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal extern static void DrawThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref NativeMethods.RECT rect, ref NativeMethods.RECT clipRect);
			[DllImport("uxtheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal extern static IntPtr DrawThemeText(IntPtr hTheme, IntPtr hdc, int iPartId,
				int iStateId, [MarshalAs(UnmanagedType.LPTStr)]string text, int iCharCount, int dwTextFlags,
				int dwTextFlags2, ref NativeMethods.RECT rect);
			[DllImport("uxtheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal extern static IntPtr GetThemeBackgroundContentRect(IntPtr hTheme, IntPtr hdc,
				int iPartId, int iStateId, ref NativeMethods.RECT boundingRect, ref NativeMethods.RECT contentRect);
			[DllImport("uxtheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal extern static IntPtr GetThemeBackgroundExtent(IntPtr hTheme, IntPtr hdc,
				int iPartId, int iStateId, ref NativeMethods.RECT contentRect, ref NativeMethods.RECT ExtentRect);
			[DllImport("uxtheme.dll")]
			internal extern static int GetThemeBackgroundRegion(IntPtr hTheme, IntPtr hdc,
				int iPartId, int iStateId, ref NativeMethods.RECT rect, ref IntPtr region);
			[DllImport("uxtheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal extern static IntPtr GetCurrentThemeName([MarshalAs(UnmanagedType.LPWStr)]string pszThemeFileName,
				int dwMaxNameChars,
				[MarshalAs(UnmanagedType.LPWStr)]string pszColorBuff,
				int cchMaxColorChars,
				[MarshalAs(UnmanagedType.LPWStr)]string pszSizeBuff,
				int cchMaxSizeChars);
			[DllImport("uxtheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal static extern int SetWindowTheme(IntPtr hwnd, String pszSubAppName, String pszSubIdList);
			[DllImport("uxtheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal extern static bool IsThemeActive();
			[DllImport("uxtheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal extern static IntPtr GetWindowTheme(IntPtr hwnd);
			[DllImport("uxtheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal extern static IntPtr GetThemePartSize(IntPtr hTheme, IntPtr hdc,
				int iPartId, int iStateId, IntPtr rect, int eSize, ref NativeMethods.SIZE size);
			[DllImport("uxtheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal extern static IntPtr GetThemeColor(IntPtr hTheme, int iPartId, int iStateId, int iPropId,
				out int pColor);
			[DllImport("uxtheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal extern static IntPtr DrawThemeBorder(IntPtr hTheme, IntPtr hdc,
				int iStateId, ref NativeMethods.RECT pRect);
			[DllImport("uxtheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal extern static IntPtr DrawThemeEdge(IntPtr hTheme, IntPtr hdc,
				int iPartId, int iStateId, ref NativeMethods.RECT pRect, int uEdge, int uFlags, ref NativeMethods.RECT pContentRect);
			[DllImport("uxtheme.dll", CallingConvention = CallingConvention.Winapi)]
			internal extern static IntPtr GetThemeMargins(IntPtr hTheme, IntPtr hdc, int iPartId,
				int iStateId, int iPropId, IntPtr pRect, ref XPMARGINS margins);
			[DllImport("user32.dll")]
			internal static extern bool SetProcessDPIAware();
			[DllImport("user32.dll")]
			internal static extern void DisableProcessWindowsGhosting();
			[DllImport("dwmapi.dll")]
			internal static extern void DwmIsCompositionEnabled(ref bool isEnabled);
			[DllImport("dwmapi.dll")]
			internal static extern void DwmExtendFrameIntoClientArea(System.IntPtr hWnd, ref NativeMethods.Margins pMargins);
			[DllImport("dwmapi.dll")]
			internal static extern int DwmDefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, out IntPtr result);
			[DllImport("dwmapi.dll", PreserveSig = false)]
			internal static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
		}
		#endregion SecurityCritical
		public static IntPtr BeginBufferedPaint(IntPtr pHdcTarget, IntPtr lpRect, IntPtr dwFormat, IntPtr pPaintParams, ref IntPtr pHdc) {
			return UnsafeNativeVista.BeginBufferedPaint(pHdcTarget, lpRect, dwFormat, pPaintParams, ref pHdc);
		}
		public static IntPtr BeginBufferedPaint(IntPtr hdcTarget, ref NativeMethods.RECT prcTarget, BP_BUFFERFORMAT dwFormat, IntPtr pPaintParams, ref IntPtr pHdc) {
			return UnsafeNativeVista.BeginBufferedPaint(hdcTarget, ref prcTarget, dwFormat, pPaintParams, ref pHdc);
		}
		public static IntPtr BufferedPaintSetAlpha(IntPtr hBufferedPaint, IntPtr prc, byte alpha) {
			return UnsafeNativeVista.BufferedPaintSetAlpha(hBufferedPaint, prc, alpha);
		}
		public static IntPtr EndBufferedPaint(IntPtr hBufferedPaint, IntPtr fUpdateTarget) {
			return UnsafeNativeVista.EndBufferedPaint(hBufferedPaint, fUpdateTarget);
		}
		public static int EndBufferedPaint(IntPtr hBufferedPaint, bool fUpdateTarget) {
			return UnsafeNativeVista.EndBufferedPaint(hBufferedPaint, fUpdateTarget);
		}
		public static void BufferedPaintInit() {
			UnsafeNativeVista.BufferedPaintInit();
		}
		public static Int32 GetThemeTextExtent(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, String text, int textLength, int textFlags, ref NativeMethods.RECT boundingRect, out NativeMethods.RECT extentRect) {
			return UnsafeNativeVista.GetThemeTextExtent(hTheme, hdc, iPartId, iStateId, text, textLength, textFlags, ref boundingRect, out extentRect);
		}
		public static Int32 GetThemeTextExtent(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, String text, int textLength, int textFlags, IntPtr boundingRect, out NativeMethods.RECT extentRect) {
			return UnsafeNativeVista.GetThemeTextExtent(hTheme, hdc, iPartId, iStateId, text, textLength, textFlags, boundingRect, out extentRect);
		}
		public static int GetCurrentThemeName([MarshalAs(UnmanagedType.LPWStr)]StringBuilder pszThemeFileName,
			Int32 dwMaxNameChars, IntPtr pszColorBuff, Int32 cchMaxColorChars, IntPtr pszSizeBuff, Int32 cchMaxSizeChars) {
			return UnsafeNativeVista.GetCurrentThemeName(pszThemeFileName, dwMaxNameChars, pszColorBuff, cchMaxColorChars, pszSizeBuff, cchMaxSizeChars);
		}
		public static void DwmExtendFrameIntoClientArea(System.IntPtr hWnd, ref NativeMethods.Margins pMargins) {
			UnsafeNativeVista.DwmExtendFrameIntoClientArea(hWnd, ref pMargins);
		}
		public static IntPtr OpenThemeData(IntPtr hWnd, [MarshalAs(UnmanagedType.LPTStr)]string pszClassList) {
			return UnsafeNativeVista.OpenThemeData(hWnd, pszClassList);
		}
		public static IntPtr CloseThemeData(IntPtr hTheme) {
			return UnsafeNativeVista.CloseThemeData(hTheme);
		}
		public static int GetThemeAppProperties() {
			return UnsafeNativeVista.GetThemeAppProperties();
		}
		public static bool IsAppThemed() {
			return UnsafeNativeVista.IsAppThemed();
		}
		public static void SetThemeAppProperties(int dwFlags) {
			UnsafeNativeVista.SetThemeAppProperties(dwFlags);
		}
		public static IntPtr DrawThemeParentBackground(IntPtr hwnd, IntPtr hdc, ref NativeMethods.RECT rect) {
			return UnsafeNativeVista.DrawThemeParentBackground(hwnd, hdc, ref rect);
		}
		public static void DrawThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref NativeMethods.RECT rect, ref NativeMethods.RECT clipRect) {
			UnsafeNativeVista.DrawThemeBackground(hTheme, hdc, iPartId, iStateId, ref rect, ref clipRect);
		}
		public static IntPtr DrawThemeText(IntPtr hTheme, IntPtr hdc, int iPartId,
			int iStateId, [MarshalAs(UnmanagedType.LPTStr)]string text, int iCharCount, int dwTextFlags,
			int dwTextFlags2, ref NativeMethods.RECT rect) {
			return UnsafeNativeVista.DrawThemeText(hTheme, hdc, iPartId, iStateId, text, iCharCount, dwTextFlags, dwTextFlags2, ref rect);
		}
		public static IntPtr GetThemeBackgroundContentRect(IntPtr hTheme, IntPtr hdc,
			int iPartId, int iStateId, ref NativeMethods.RECT boundingRect, ref NativeMethods.RECT contentRect) {
			return UnsafeNativeVista.GetThemeBackgroundContentRect(hTheme, hdc, iPartId, iStateId, ref boundingRect, ref contentRect);
		}
		public static IntPtr GetThemeBackgroundExtent(IntPtr hTheme, IntPtr hdc,
			int iPartId, int iStateId, ref NativeMethods.RECT contentRect, ref NativeMethods.RECT ExtentRect) {
			return UnsafeNativeVista.GetThemeBackgroundExtent(hTheme, hdc, iPartId, iStateId, ref contentRect, ref ExtentRect);
		}
		public static int GetThemeBackgroundRegion(IntPtr hTheme, IntPtr hdc,
			int iPartId, int iStateId, ref NativeMethods.RECT rect, ref IntPtr region) {
			return UnsafeNativeVista.GetThemeBackgroundRegion(hTheme, hdc, iPartId, iStateId, ref rect, ref region);
		}
		public static IntPtr GetCurrentThemeName([MarshalAs(UnmanagedType.LPWStr)]string pszThemeFileName,
			int dwMaxNameChars,
			[MarshalAs(UnmanagedType.LPWStr)]string pszColorBuff,
			int cchMaxColorChars,
			[MarshalAs(UnmanagedType.LPWStr)]string pszSizeBuff,
			int cchMaxSizeChars) {
			return UnsafeNativeVista.GetCurrentThemeName(pszThemeFileName, dwMaxNameChars, pszColorBuff, cchMaxColorChars, pszSizeBuff, cchMaxSizeChars);
		}
		public static int SetWindowTheme(IntPtr hwnd, String pszSubAppName, String pszSubIdList) {
			return UnsafeNativeVista.SetWindowTheme(hwnd, pszSubAppName, pszSubIdList);
		}
		public static bool IsThemeActive() {
			return UnsafeNativeVista.IsThemeActive();
		}
		public static IntPtr GetWindowTheme(IntPtr hwnd) {
			return UnsafeNativeVista.GetWindowTheme(hwnd);
		}
		public static IntPtr GetThemePartSize(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, IntPtr rect, int eSize, ref NativeMethods.SIZE size) {
			return UnsafeNativeVista.GetThemePartSize(hTheme, hdc, iPartId, iStateId, rect, eSize, ref size);
		}
		public static IntPtr GetThemeColor(IntPtr hTheme, int iPartId, int iStateId, int iPropId, out int pColor) {
			return UnsafeNativeVista.GetThemeColor(hTheme, iPartId, iStateId, iPropId, out pColor);
		}
		public static IntPtr DrawThemeBorder(IntPtr hTheme, IntPtr hdc, int iStateId, ref NativeMethods.RECT pRect) {
			return UnsafeNativeVista.DrawThemeBorder(hTheme, hdc, iStateId, ref pRect);
		}
		public static IntPtr DrawThemeEdge(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref NativeMethods.RECT pRect, int uEdge, int uFlags, ref NativeMethods.RECT pContentRect) {
			return UnsafeNativeVista.DrawThemeEdge(hTheme, hdc, iPartId, iStateId, ref pRect, uEdge, uFlags, ref pContentRect);
		}
		public static IntPtr GetThemeMargins(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, int iPropId, IntPtr pRect, ref XPMARGINS margins) {
			return UnsafeNativeVista.GetThemeMargins(hTheme, hdc, iPartId, iStateId, iPropId, pRect, ref margins);
		}
		public static void FillRect(IntPtr hdc, Rectangle rect) {
			NativeMethods.RECT r = new NativeMethods.RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
			IntPtr hb = NativeMethods.CreateSolidBrush(0x00000000);
			NativeMethods.FillRect(hdc, ref r, hb);
			NativeMethods.DeleteObject(hb);
		}
		public static void PaintControl(IntPtr hWnd, IntPtr hdc, Rectangle rect, bool bEraseBackground) {
			IntPtr hdcPaint = IntPtr.Zero;
			IntPtr rectPtr = Marshal.AllocHGlobal(Marshal.SizeOf(rect));
			Marshal.StructureToPtr(rect, rectPtr, false);
			IntPtr hBufferedPaint = UnsafeNativeVista.BeginBufferedPaint(hdc, rectPtr, new IntPtr(BPBF_TOPDOWNDIB), IntPtr.Zero, ref hdcPaint);
			if(hdcPaint != IntPtr.Zero) {
				if(bEraseBackground) 
					NativeMethods.SendMessage(hWnd, MSG.WM_ERASEBKGND, hdcPaint, hdcPaint);
				NativeMethods.SendMessage(hWnd, MSG.WM_PRINTCLIENT, hdcPaint, PRF_CLIENT);
				UnsafeNativeVista.BufferedPaintSetAlpha(hBufferedPaint, rectPtr, 255);
				UnsafeNativeVista.EndBufferedPaint(hBufferedPaint, new IntPtr(1));
			}
		}
		public static Size CalcTextSizeOnGlass(Graphics graphics, string text, Font font, Rectangle bounds, TextFormatFlags flags) {
			IntPtr primaryHdc = graphics.GetHdc();
			System.Windows.Forms.VisualStyles.VisualStyleRenderer renderer = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(System.Windows.Forms.VisualStyles.VisualStyleElement.Window.Caption.Active);
			IntPtr fontHandle = font.ToHfont();
			IntPtr prevFontHandle = NativeMethods.SelectObject(primaryHdc, fontHandle);
			DTTOPTS dttOpts = new DTTOPTS();
			dttOpts.dwSize = Marshal.SizeOf(typeof(DTTOPTS));
			dttOpts.dwFlags = DTT_COMPOSITED | DTT_CALCRECT | DTT_TEXTCOLOR;
			dttOpts.crText = ColorTranslator.ToWin32(Color.Black);
			dttOpts.iGlowSize = 10;
			NativeMethods.RECT textBounds = new NativeMethods.RECT(0, 0, bounds.Right - bounds.Left, bounds.Bottom - bounds.Top);
			UnsafeNativeVista.DrawThemeTextEx(renderer.Handle, primaryHdc, 0, 0, text, -1, ((int)flags) | DT_CALCRECT, ref textBounds, ref dttOpts);
			NativeMethods.DeleteObject(fontHandle);
			NativeMethods.SelectObject(primaryHdc, prevFontHandle);
			graphics.ReleaseHdc(primaryHdc);
			return new Size(textBounds.Right - textBounds.Left, textBounds.Bottom - textBounds.Top);
		}
		static IntPtr CachedDibSection = IntPtr.Zero;
		static void DrawTextOnGlass(Graphics graphics, string text, Font font, Rectangle bounds, Color color, int flags, int rflags) {
			if(!System.Windows.Forms.VisualStyles.VisualStyleRenderer.IsSupported) {
				using(SolidBrush brush = new SolidBrush(color)) {
					graphics.DrawString(text, font, brush, bounds);
				}
				return;
			}
			IntPtr primaryHdc = graphics.GetHdc();
			IntPtr memoryHdc = NativeMethods.CreateCompatibleDC(primaryHdc);
			if(CachedDibSection == IntPtr.Zero) {
				NativeMethods.BITMAPINFO_SMALL info = new NativeMethods.BITMAPINFO_SMALL();
				info.biSize = Marshal.SizeOf(info);
				info.biWidth = 1280;
				info.biHeight = -64;
				info.biPlanes = 1;
				info.biBitCount = 32;
				info.biCompression = 0;
				CachedDibSection = NativeMethods.CreateDIBSection(primaryHdc, ref info, 0, 0, IntPtr.Zero, 0);
			}
			NativeMethods.SelectObject(memoryHdc, CachedDibSection);
			IntPtr fontHandle = font.ToHfont();
			NativeMethods.SelectObject(memoryHdc, fontHandle);
			const int SRCCOPY = 0xCC0020;
			NativeMethods.BitBlt(memoryHdc, 0, 0, bounds.Width, bounds.Height, primaryHdc, bounds.X, bounds.Y, SRCCOPY);
			var renderer = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(
				System.Windows.Forms.VisualStyles.VisualStyleElement.Window.Caption.Active);
			DTTOPTS dttOpts = new DTTOPTS();
			dttOpts.dwSize = Marshal.SizeOf(typeof(DTTOPTS));
			dttOpts.dwFlags = DTT_COMPOSITED | rflags | DTT_TEXTCOLOR;
			dttOpts.crText = ColorTranslator.ToWin32(color);
			dttOpts.iGlowSize = 10;
			NativeMethods.RECT textBounds = new NativeMethods.RECT(0, 0, bounds.Right - bounds.Left, bounds.Bottom - bounds.Top);
			UnsafeNativeVista.DrawThemeTextEx(renderer.Handle, memoryHdc, 0, 0, text, -1, (int)flags, ref textBounds, ref dttOpts);
			NativeMethods.BitBlt(primaryHdc, bounds.Left, bounds.Top, bounds.Width, bounds.Height, memoryHdc, 0, 0, SRCCOPY);
			NativeMethods.DeleteObject(fontHandle);
			NativeMethods.DeleteDC(memoryHdc);
			graphics.ReleaseHdc(primaryHdc);
		}
		static void DrawTextOnGlass(Graphics graphics, string text, Font font, Rectangle bounds, Color color, TextFormatFlags flags, int rflags) {
			DrawTextOnGlass(graphics, text, font, bounds, color, (int)flags, rflags);
		}
		public static void DrawTextOnGlass(Graphics graphics, string text, Font font, Rectangle bounds, Color color, TextFormatFlags flags) {
			DrawTextOnGlass(graphics, text, font, bounds, color, flags, 0);
		}
		public static void DrawGlowingText(Graphics graphics, string text, Font font, Rectangle bounds, Color color, TextFormatFlags flags) {
			DrawTextOnGlass(graphics, text, font, bounds, color, flags, DTT_GLOWSIZE);
		}
		const int BPBF_TOPDOWNDIB = 2;
		const int PRF_CLIENT = 0x00000004;
		const int DTT_COMPOSITED = 8192;
		const int DTT_GLOWSIZE = 2048;
		const int DTT_TEXTCOLOR = 1;
		const int DTT_CALCRECT = 0x200;
		const int DT_CALCRECT = 1024;
		public static bool IsVistaOrLater { get { return Environment.OSVersion.Version.Major >= 6; } }
		public static bool IsVista {
			get { return Environment.OSVersion.Version.Major == 6; }
		}
		public static bool IsWindows7 {
			get { return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor > 0; }
		}
		public static bool IsWindows8 {
			get { return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor > 1; }
		}
		public static bool IsWindowsBlue {
			get { return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor > 2; }
		}
		public static bool IsWindows2012Server {
			get {
				OSVERSIONINFOEX osv = new OSVERSIONINFOEX();
				osv.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));
				const int VER_NT_WORKSTATION = 1;
				if(GetVersionEx(ref osv)) {
					if(((osv.dwMajorVersion == 6 && osv.dwMinorVersion >= 2) || osv.dwMajorVersion > 6) && osv.dwPlatformId != VER_NT_WORKSTATION)  { 
						return true;
					}
				}
				return false;
			}
		}
		public static bool IsCompositionEnabled() {
			if(!IsVista) return false;
			bool enabled = false;
			DwmIsCompositionEnabled(ref enabled);
			return enabled;
		}
		public static void CallDwmBase(ref Message m) {
			IntPtr res = m.Result;
			UnsafeNativeVista.DwmDefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam, out res);
			m.Result = res;
		}
		public static void DisableWindowGhosting() {
			if(!IsVista) return;
			DisableProcessWindowsGhosting();
		}
		public static void SetProcessDPIAware() {
			if(DevExpress.Utils.Drawing.Helpers.NativeVista.IsVistaOrLater) {
				UnsafeNativeVista.SetProcessDPIAware();
			}
		}
		public static void DisableProcessWindowsGhosting() {
			UnsafeNativeVista.DisableProcessWindowsGhosting(); 
		}
		public static void DwmIsCompositionEnabled(ref bool isEnabled) {
			UnsafeNativeVista.DwmIsCompositionEnabled(ref isEnabled); 
		}
		public static void SetNCRendering(IntPtr handle, bool enable) {
			int renderPolicy = (int)(enable ? DWMNCRenderingPolicy.DWMNCRP_ENABLED : DWMNCRenderingPolicy.DWMNCRP_DISABLED);
			UnsafeNativeVista.DwmSetWindowAttribute(handle, (int)DWMWINDOWATTRIBUTE.DWMWA_NCRENDERING_POLICY, ref renderPolicy, sizeof(int));
		}
		public static void SetNCPaint(IntPtr handle, bool enable) {
			int renderPolicy = (int)(enable ? 1 : 0);
			UnsafeNativeVista.DwmSetWindowAttribute(handle, (int)DWMWINDOWATTRIBUTE.DWMWA_ALLOW_NCPAINT, ref renderPolicy, sizeof(int));
		}
		static bool GetVersionEx(ref OSVERSIONINFOEX osvi) {
			return UnsafeNativeVista.GetVersionEx(ref osvi);
		}
	}
	[ComImport, Guid("a5cd92ff-29be-454c-8d04-d82879fb3f1b")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IVirtualDesktopManager {
		IntPtr IsWindowOnCurrentVirtualDesktop(IntPtr topLevelWindow, out Int32 onCurrentDesktop);
	}
	[ComImport, Guid("aa509086-5ca9-4c25-8f95-589d3c07b48a")]
	public class VirtualDesktopManager { }
	[System.Security.SecuritySafeCritical]
	public class NativeMethods {
		#region Structs&Enums
		[StructLayout(LayoutKind.Sequential)]
		internal struct APPBARDATA {
			public int cbSize;
			public IntPtr hWnd;
			public uint uCallbackMessage;
			public uint uEdge;
			public NativeMethods.RECT rc;
			public IntPtr lParam;
		}
		public struct WNDCLASS {
			public Int32 style;
			public Delegate lpfnWndProc;
			public int cbClsExtra;
			public int cbWndExtra;
			public IntPtr hInstance;
			public IntPtr hIcon;
			public IntPtr hCursor;
			public IntPtr hbrBackground;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpszMenuName;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpszClassName;
		}
		public enum ShellAddToRecentDocs {
			Pidl = 0x1,
			PathA = 0x2,
			PathW = 0x3,
			AppIdInfo = 0x4,
			AppIdInfoIdList = 0x5,
			Link = 0x6,
			AppIdInfoLink = 0x7,
			ShellItem = 0x8
		}
		public enum FLASHW {
			FLASHW_STOP = 0,
			FLASHW_CAPTION = 1,
			FLASHW_TRAY = 2,
			FLASHW_ALL = 3,
			FLASHW_TIMER = 4,
			FLASHW_TIMERNOFG = 12
		}
		[StructLayout(LayoutKind.Sequential)]
		internal struct FLASHWINFO {
			public UInt32 cbSize;
			public IntPtr hwnd;
			public UInt32 dwFlags;
			public UInt32 uCount;
			public UInt32 dwTimeout;
		}
		[StructLayout(LayoutKind.Explicit)]
		public class PropVariant : IDisposable {
			[FieldOffset(0)]
			ushort valueType;
			[FieldOffset(8)]
			IntPtr ptr;
			[FieldOffset(8)]
			Int32 int32;			
			public PropVariant(string value) {
				if(value == null) return;
				valueType = (ushort)VarEnum.VT_LPWSTR;
				ptr = Marshal.StringToCoTaskMemUni(value);
			}
			public PropVariant(bool value) {
				valueType = (ushort)VarEnum.VT_BOOL;
				int32 = (value) ? -1 : 0;
			}
			public void Dispose() {
				PropVariantClear(this);
				GC.SuppressFinalize(this);
			}
		}
		public enum ShowWindowCommands : int {
			Hide = 0,
			Normal = 1,
			ShowMinimized = 2,
			Maximize = 3,
			ShowMaximized = 3,
			ShowNoActivate = 4,
			Show = 5,
			Minimize = 6,
			ShowMinNoActive = 7,
			ShowNA = 8,
			Restore = 9,
			ShowDefault = 10,
			ForceMinimize = 11
		}
		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPLACEMENT {
			public int Length;
			public int Flags;
			public ShowWindowCommands ShowCmd;
			public DevExpress.Utils.Drawing.Helpers.NativeMethods.POINT MinPosition;
			public DevExpress.Utils.Drawing.Helpers.NativeMethods.POINT MaxPosition;
			public DevExpress.Utils.Drawing.Helpers.NativeMethods.RECT NormalPosition;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct BLENDFUNCTION {
			public byte BlendOp;
			public byte BlendFlags;
			public byte SourceConstantAlpha;
			public byte AlphaFormat;
		}
		public enum ScrollWindowExFlags {
			SW_SCROLLCHILDREN = 0x01, 
			SW_INVALIDATE = 0x02, 
			SW_ERASE = 0x04, 
			SW_SMOOTHSCROLL = 0x10
		};
		[StructLayout(LayoutKind.Sequential)]
		public struct Margins {
			public int Left, Right, Top, Bottom;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct BITMAPINFO_SMALL {
			public int biSize;
			public int biWidth;
			public int biHeight;
			public short biPlanes;
			public short biBitCount;
			public int biCompression;
			public int biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public int biClrUsed;
			public int biClrImportant;
			public byte bmiColors_rgbBlue;
			public byte bmiColors_rgbGreen;
			public byte bmiColors_rgbRed;
			public byte bmiColors_rgbReserved;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct BITMAPINFO_FLAT {
			public int biSize;
			public int biWidth;
			public int biHeight;
			public short biPlanes;
			public short biBitCount;
			public int biCompression;
			public int bmiHeader_biSizeImage;
			public int bmiHeader_biXPelsPerMeter;
			public int bmiHeader_biYPelsPerMeter;
			public int bmiHeader_biClrUsed;
			public int bmiHeader_biClrImportant;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x400)]
			public byte[] bmiColors;
		}
		[StructLayout(LayoutKind.Sequential)]
		public class BITMAPINFOHEADER {
			public int biSize;
			public int biWidth;
			public int biHeight;
			public short biPlanes;
			public short biBitCount;
			public int biCompression;
			public int biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public int biClrUsed;
			public int biClrImportant;
			public BITMAPINFOHEADER() {
				this.biSize = 40;
			}
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct HWND : IWin32Window {
			IntPtr _Handle;
			public static readonly HWND Empty = new HWND(IntPtr.Zero);
			public static HWND Desktop {
				get { return NativeMethods.GetDesktopWindow(); }
			}
			public HWND(IntPtr aValue) {
				_Handle = aValue;
			}
			#region Overrides
			public override bool Equals(object obj) {
				if(obj == null)
					return false;
				if(obj is HWND)
					return Equals((HWND)obj);
				if(obj is IntPtr)
					return Equals((IntPtr)obj);
				return false;
			}
			public bool Equals(IntPtr ptr) {
				if(!_Handle.ToInt32().Equals(ptr.ToInt32()))
					return false;
				return true;
			}
			public bool Equals(HWND hwnd) {
				return Equals(hwnd._Handle);
			}
			public bool Equals(IWin32Window window) {
				return Equals(window.Handle);
			}
			public override int GetHashCode() {
				return _Handle.GetHashCode();
			}
			public override string ToString() {
				return "{" + "Handle=0x" + _Handle.ToInt32().ToString("X8") + "}";
			}
			#endregion
			public bool IsEmpty {
				get { return _Handle == IntPtr.Zero; }
			}
			public bool IsVisible {
				get { return NativeMethods.IsWindowVisible(_Handle); }
			}
			public IntPtr Handle {
				get { return _Handle; }
			}
			#region Operators
			public static bool operator ==(HWND aHwnd1, HWND aHwnd2) {
				if((object)aHwnd1 == null)
					return ((object)aHwnd2 == null);
				return aHwnd1.Equals(aHwnd2);
			}
			public static bool operator ==(IntPtr aIntPtr, HWND aHwnd) {
				if((object)aIntPtr == null)
					return ((object)aHwnd == null);
				return aHwnd.Equals(aIntPtr);
			}
			public static bool operator ==(HWND aHwnd, IntPtr aIntPtr) {
				if((object)aHwnd == null)
					return ((object)aIntPtr == null);
				return aHwnd.Equals(aIntPtr);
			}
			public static bool operator !=(HWND aHwnd1, HWND aHwnd2) {
				return !(aHwnd1 == aHwnd2);
			}
			public static bool operator !=(IntPtr aIntPtr, HWND aHwnd) {
				return !(aIntPtr == aHwnd);
			}
			public static bool operator !=(HWND aHwnd, IntPtr aIntPtr) {
				return !(aHwnd == aIntPtr);
			}
			public static implicit operator IntPtr(HWND aHwnd) {
				return aHwnd.Handle;
			}
			public static implicit operator HWND(IntPtr aIntPtr) {
				return new HWND(aIntPtr);
			}
			#endregion
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct HDC {
			IntPtr _Handle;
			public static readonly HDC Empty = new HDC(0);
			public HDC(IntPtr aValue) {
				_Handle = aValue;
			}
			public HDC(int aValue) {
				_Handle = new IntPtr(aValue);
			}
			#region Overrides
			public override bool Equals(object aObj) {
				if(aObj == null)
					return false;
				if(aObj is HDC)
					return Equals((HDC)aObj);
				if(aObj is IntPtr)
					return Equals((IntPtr)aObj);
				return false;
			}
			public bool Equals(HDC aHDC) {
				if(!_Handle.Equals(aHDC._Handle))
					return false;
				return true;
			}
			public bool Equals(IntPtr aIntPtr) {
				if(!_Handle.Equals(aIntPtr))
					return false;
				return true;
			}
			public override int GetHashCode() {
				return _Handle.GetHashCode();
			}
			public override string ToString() {
				return "{" + "Handle=0x" + _Handle.ToInt32().ToString("X8") + "}";
			}
			#endregion
			public void Release(HWND window) {
				NativeMethods.ReleaseDC(window, this);
			}
			public IntPtr SelectObject(IntPtr aGDIObj) {
				return NativeMethods.SelectObject(this, aGDIObj);
			}
			public HDC CreateCompatible() {
				return NativeMethods.CreateCompatibleDC(_Handle);
			}
			public IntPtr CreateCompatibleBitmap(int width, int height) {
				return NativeMethods.CreateCompatibleBitmap(_Handle, width, height);
			}
			public IntPtr CreateCompatibleBitmap(Rectangle rectangle) {
				return CreateCompatibleBitmap(rectangle.Width, rectangle.Height);
			}
			public void Delete() {
				NativeMethods.DeleteDC(_Handle);
			}
			public IntPtr Handle {
				get {
					return _Handle;
				}
			}
			public bool IsEmpty {
				get {
					return _Handle == IntPtr.Zero;
				}
			}
			#region Operators
			public static bool operator ==(HDC aHdc1, HDC aHdc2) {
				if((object)aHdc1 == null)
					return ((object)aHdc2 == null);
				return aHdc1.Equals(aHdc2);
			}
			public static bool operator ==(IntPtr aIntPtr, HDC aHdc) {
				if((object)aIntPtr == null)
					return ((object)aHdc == null);
				return aHdc.Equals(aIntPtr);
			}
			public static bool operator ==(HDC aHdc, IntPtr aIntPtr) {
				if((object)aHdc == null)
					return ((object)aIntPtr == null);
				return aHdc.Equals(aIntPtr);
			}
			public static bool operator !=(HDC aHdc1, HDC aHdc2) {
				return !(aHdc1 == aHdc2);
			}
			public static bool operator !=(IntPtr aIntPtr, HDC aHdc) {
				return !(aIntPtr == aHdc);
			}
			public static bool operator !=(HDC aHdc, IntPtr aIntPtr) {
				return !(aHdc == aIntPtr);
			}
			public static implicit operator IntPtr(HDC aHdc) {
				return aHdc.Handle;
			}
			public static implicit operator HDC(IntPtr aIntPtr) {
				return new HDC(aIntPtr);
			}
			#endregion
		}
		[StructLayout(LayoutKind.Sequential), CLSCompliant(false)]
		public struct COLORREF {
			uint _ColorRef;
			public COLORREF(Color aValue) {
				int lRGB = aValue.ToArgb();
				int n0 = (lRGB & 0xff) << 16;
				lRGB = lRGB & 0xffff00;
				lRGB = (lRGB | (lRGB >> 16 & 0xff));
				lRGB = (lRGB & 0xffff);
				lRGB = (lRGB | n0);
				_ColorRef = (uint)lRGB;
			}
			public COLORREF(int lRGB) {
				_ColorRef = (uint)lRGB;
			}
			public Color ToColor() {
				int r = (int)_ColorRef & 0xff;
				int g = ((int)_ColorRef >> 8) & 0xff;
				int b = ((int)_ColorRef >> 16) & 0xff;
				return Color.FromArgb(r, g, b);
			}
			public static COLORREF FromColor(System.Drawing.Color aColor) {
				return new COLORREF(aColor);
			}
			public static System.Drawing.Color ToColor(COLORREF aColorRef) {
				return aColorRef.ToColor();
			}
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct HHOOK {
			IntPtr _Handle;
			public static readonly HHOOK Empty = new HHOOK(0);
			public HHOOK(IntPtr aValue) {
				_Handle = aValue;
			}
			public HHOOK(int aValue) {
				_Handle = new IntPtr(aValue);
			}
			#region Overrides
			public override bool Equals(object aObj) {
				if(aObj == null)
					return false;
				if(aObj is HHOOK)
					return Equals((HHOOK)aObj);
				if(aObj is IntPtr)
					return Equals((IntPtr)aObj);
				return false;
			}
			public bool Equals(HHOOK aHHOOK) {
				if(!_Handle.Equals(aHHOOK._Handle))
					return false;
				return true;
			}
			public bool Equals(IntPtr aIntPtr) {
				if(!_Handle.Equals(aIntPtr))
					return false;
				return true;
			}
			public override int GetHashCode() {
				return _Handle.GetHashCode();
			}
			public override string ToString() {
				return "{" + "Handle=0x" + _Handle.ToInt32().ToString("X8") + "}";
			}
			#endregion
			public IntPtr Handle {
				get { return _Handle; }
			}
			public bool IsEmpty {
				get { return _Handle == IntPtr.Zero; }
			}
			#region Operators
			public static bool operator ==(HHOOK aHHook1, HHOOK aHHook2) {
				if((object)aHHook1 == null)
					return ((object)aHHook2 == null);
				return aHHook1.Equals(aHHook2);
			}
			public static bool operator ==(IntPtr aIntPtr, HHOOK aHHook) {
				if((object)aIntPtr == null)
					return ((object)aHHook == null);
				return aHHook.Equals(aIntPtr);
			}
			public static bool operator ==(HHOOK aHHook, IntPtr aIntPtr) {
				if((object)aHHook == null)
					return ((object)aIntPtr == null);
				return aHHook.Equals(aIntPtr);
			}
			public static bool operator !=(HHOOK aHHook1, HHOOK aHHook2) {
				return !(aHHook1 == aHHook2);
			}
			public static bool operator !=(IntPtr aIntPtr, HHOOK aHHook) {
				return !(aIntPtr == aHHook);
			}
			public static bool operator !=(HHOOK aHHook, IntPtr aIntPtr) {
				return !(aHHook == aIntPtr);
			}
			public static implicit operator IntPtr(HHOOK aHHook) {
				return aHHook.Handle;
			}
			public static implicit operator HHOOK(IntPtr aIntPtr) {
				return new HHOOK(aIntPtr);
			}
			#endregion
		}
		public struct COPYDATASTRUCT: IDisposable {
			public IntPtr dwData;
			public int cbData;
			public IntPtr lpData;
			public void Dispose() {
				if(lpData != IntPtr.Zero) {
					NativeMethods.LocalFree(this.lpData);
					lpData = IntPtr.Zero;
				}
			}
		}
		public enum SystemCursors {
			OCR_NORMAL = 32512,
			OCR_IBEAM = 32513,
			OCR_WAIT = 32514,
			OCR_CROSS = 32515,
			OCR_UP = 32516,
			OCR_SIZE = 32640,
			OCR_ICON = 32641,
			OCR_SIZENWSE = 32642,
			OCR_SIZENESW = 32643,
			OCR_SIZEWE = 32644,
			OCR_SIZENS = 32645,
			OCR_SIZEALL = 32646,
			OCR_ICOCUR = 32647,
			OCR_NO = 32648,
			OCR_HAND = 32649,
			OCR_APPSTARTING = 32650
		}
		[Flags]
		public enum RasterOperations {
			SRCCOPY = 0x00CC0020,
			SRCPAINT = 0x00EE0086,
			SRCAND = 0x008800C6,
			SRCINVERT = 0x00660046,
			SRCERASE = 0x00440328,
			NOTSRCCOPY = 0x00330008,
			NOTSRCERASE = 0x001100A6,
			MERGECOPY = 0x00C000CA,
			MERGEPAINT = 0x00BB0226,
			PATCOPY = 0x00F00021,
			PATPAINT = 0x00FB0A09,
			PATINVERT = 0x005A0049,
			DSTINVERT = 0x00550009,
			BLACKNESS = 0x00000042,
			WHITENESS = 0x00FF0062
		}
		public enum MouseMessages {
			WM_LBUTTONDOWN = 0x0201,
			WM_LBUTTONUP = 0x0202,
			WM_MOUSEMOVE = 0x0200,
			WM_MOUSEWHEEL = 0x020A,
			WM_RBUTTONDOWN = 0x0204,
			WM_RBUTTONUP = 0x0205
		}
		[Flags]
		public enum PrintOptions {
			PRF_CHECKVISIBLE = 0x00000001,
			PRF_NONCLIENT = 0x00000002,
			PRF_CLIENT = 0x00000004,
			PRF_ERASEBKGND = 0x00000008,
			PRF_CHILDREN = 0x00000010,
			PRF_OWNED = 0x00000020
		};	  
		[StructLayout(LayoutKind.Sequential)]
		public class TRACKMOUSEEVENTStruct {
			public int cbSize;
			public int dwFlags;
			public IntPtr hwndTrack;
			public int dwHoverTime = 0;
			public TRACKMOUSEEVENTStruct() : this(0, IntPtr.Zero, 0) { }
			public TRACKMOUSEEVENTStruct(int dwFlags, IntPtr hwndTrack, int dwHoverTime) {
				this.cbSize = Marshal.SizeOf(typeof(TRACKMOUSEEVENTStruct));
				this.dwFlags = dwFlags;
				this.hwndTrack = hwndTrack;
				this.dwHoverTime = dwHoverTime;
			}
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct NCCALCSIZE_PARAMS {
			public RECT rgrc0, rgrc1, rgrc2;
			public IntPtr lppos;
			[System.Security.SecuritySafeCritical]
			public static NCCALCSIZE_PARAMS GetFrom(IntPtr lParam) {
				return (NCCALCSIZE_PARAMS)Marshal.PtrToStructure(lParam, typeof(NCCALCSIZE_PARAMS));
			}
			[System.Security.SecuritySafeCritical]
			public void SetTo(IntPtr lParam) {
				Marshal.StructureToPtr(this, lParam, false);
			}
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct PAINTSTRUCT {
			public IntPtr hdc;
			public bool fErase;
			public RECT rcPaint;
			public bool fRestore;
			public bool fIncUpdate;
			public int reserved1;
			public int reserved2;
			public int reserved3;
			public int reserved4;
			public int reserved5;
			public int reserved6;
			public int reserved7;
			public int reserved8;
		}
		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		public struct POINT {
			public int X, Y;
			public POINT(int x, int y) {
				this.X = x;
				this.Y = y;
			}
			public POINT(Point pt) {
				this.X = pt.X;
				this.Y = pt.Y;
			}
			public Point ToPoint() { return new Point(X, Y); }
		}
		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		public struct SIZE {
			public int Width, Height;
			public SIZE(int w, int h) {
				this.Width = w;
				this.Height = h;
			}
			public SIZE(Size size) {
				this.Width = size.Width;
				this.Height = size.Height;
			}
			public Size ToSize() { return new Size(Width, Height); }
		}
		public enum RegionDataHeaderTypes : int {
			Rectangles = 1
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct RGNDATAHEADER {
			public int dwSize;
			public int iType;
			public int nCount;
			public int nRgnSize;
			public RECT rcBound;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT {
			public int Left, Top, Right, Bottom;
			public RECT(int l, int t, int r, int b) {
				Left = l; Top = t; Right = r; Bottom = b;
			}
			public RECT(Rectangle r) {
				Left = r.Left; Top = r.Top; Right = r.Right; Bottom = r.Bottom;
			}
			public Rectangle ToRectangle() {
				return Rectangle.FromLTRB(Left, Top, Right, Bottom);
			}
			public void Inflate(int width, int height) {
				Left -= width;
				Top -= height;
				Right += width;
				Bottom += height;
			}
			public override string ToString() {
				return string.Format("x:{0},y:{1},width:{2},height:{3}", Left, Top, Right - Left, Bottom - Top);
			}
		}
		public enum GetClipBoxReturn : int {
			Error = 0,
			NullRegion = 1,
			SimpleRegion = 2,
			ComplexRegion = 3
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct MINMAXINFO {
			public POINT ptReserved,
			ptMaxSize, ptMaxPosition, ptMinTrackSize, ptMaxTrackSize;
			[System.Security.SecuritySafeCritical]
			public static MINMAXINFO GetFrom(IntPtr lParam) {
				return (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
			}
			[System.Security.SecuritySafeCritical]
			public void SetTo(IntPtr lParam) {
				Marshal.StructureToPtr(this, lParam, false);
			}
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct GESTUREINFO {
			public int cbSize;
			public int dwFlags;
			public int dwID;
			public IntPtr hwndTarget;
			[MarshalAs(UnmanagedType.Struct)]
			internal POINTS ptsLocation;
			public int dwInstanceID;
			public int dwSequenceID;
			public Int64 ullArguments;
			public int cbExtraArgs;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct GESTURECONFIG {
			public GESTURECONFIG(int dwID, int dwWant, int dwBlock) {
				this.dwID = dwID;
				this.dwWant = dwWant;
				this.dwBlock = dwBlock;
			}
			public int dwID;
			public int dwWant;
			public int dwBlock;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct POINTS {
			public short x;
			public short y;
			public Point ToPoint() {
				return new Point(x, y);
			}
		}
		[StructLayout(LayoutKind.Sequential)]
		public class GESTURENOTIFYSTRUCT {
			public int cbSize;
			public int dwFlags;
			public IntPtr hwndTarget;
			[MarshalAs(UnmanagedType.Struct)]
			public POINTS ptsLocation;
			public int dwInstanceID;
		}
		public class WMSZ {
			public const int
				WMSZ_LEFT = 1,
				WMSZ_RIGHT = 2,
				WMSZ_TOP = 3,
				WMSZ_TOPLEFT = 4,
				WMSZ_TOPRIGHT = 5,
				WMSZ_BOTTOM = 6,
				WMSZ_BOTTOMLEFT = 7,
				WMSZ_BOTTOMRIGHT = 8;
		}
		public class SWP {
			public const int
				SWP_NOSIZE = 0x0001,
				SWP_NOMOVE = 0x0002,
				SWP_NOZORDER = 0x0004,
				SWP_NOREDRAW = 0x0008,
				SWP_NOACTIVATE = 0x0010,
				SWP_FRAMECHANGED = 0x0020, 
				SWP_DRAWFRAME = SWP_FRAMECHANGED,
				SWP_SHOWWINDOW = 0x0040,
				SWP_HIDEWINDOW = 0x0080,
				SWP_NOCOPYBITS = 0x0100,
				SWP_NOOWNERZORDER = 0x0200, 
				SWP_NOREPOSITION = SWP_NOOWNERZORDER,
				SWP_NOSENDCHANGING = 0x0400; 
		}
		public class DC {
			public const int
				DCX_WINDOW = 0x00000001,
				DCX_CACHE = 0x00000002,
				DCX_NORESETATTRS = 0x00000004,
				DCX_CLIPCHILDREN = 0x00000008,
				DCX_CLIPSIBLINGS = 0x00000010,
				DCX_PARENTCLIP = 0x00000020,
				DCX_EXCLUDERGN = 0x00000040,
				DCX_INTERSECTRGN = 0x00000080,
				DCX_EXCLUDEUPDATE = 0x00000100,
				DCX_INTERSECTUPDATE = 0x00000200,
				DCX_LOCKWINDOWUPDATE = 0x00000400,
				DCX_VALIDATE = 0x00200000;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPOS {
			public IntPtr hWnd;
			public IntPtr hHndInsertAfter;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public int flags;
		}
		public class SC {
			public const int
				SC_SIZE = 0xf000,
				SC_MOVE = 0xf010,
				SC_MINIMIZE = 0xf020,
				SC_MAXIMIZE = 0xf030,
				SC_NEXTWINDOW = 0xf040,
				SC_PREVWINDOW = 0xf050,
				SC_CLOSE = 0xf060,
				SC_VSCROLL = 0xf070,
				SC_HSCROLL = 0xf080,
				SC_MOUSEMENU = 0xf090,
				SC_KEYMENU = 0xf100,
				SC_ARRANGE = 0xf110,
				SC_RESTORE = 0xf120,
				SC_TASKLIST = 0xf130,
				SC_SCREENSAVE = 0xf140,
				SC_HOTKEY = 0xf150,
				SC_CONTEXTHELP = 0xf180,
				SC_DRAGMOVE = 0xf012,
				SC_SYSMENU = 0xf093;
		}
		public class HT {
			public const int HTERROR = (-2);
			public const int HTTRANSPARENT = (-1);
			public const int HTNOWHERE = 0, HTCLIENT = 1, HTCAPTION = 2, HTSYSMENU = 3,
				HTGROWBOX = 4, HTSIZE = HTGROWBOX, HTMENU = 5, HTHSCROLL = 6, HTVSCROLL = 7, HTMINBUTTON = 8, HTMAXBUTTON = 9,
				HTLEFT = 10, HTRIGHT = 11, HTTOP = 12, HTTOPLEFT = 13, HTTOPRIGHT = 14, HTBOTTOM = 15, HTBOTTOMLEFT = 16,
				HTBOTTOMRIGHT = 17, HTBORDER = 18, HTREDUCE = HTMINBUTTON, HTZOOM = HTMAXBUTTON, HTSIZEFIRST = HTLEFT,
				HTSIZELAST = HTBOTTOMRIGHT, HTOBJECT = 19, HTCLOSE = 20, HTHELP = 21;
		}
		[StructLayout(LayoutKind.Sequential)]
		public class NONCLIENTMETRICS {
			public int cbSize = Marshal.SizeOf(typeof(NONCLIENTMETRICS));
			public int iBorderWidth = 0;
			public int iScrollWidth = 0;
			public int iScrollHeight = 0;
			public int iCaptionWidth = 0;
			public int iCaptionHeight = 0;
			[MarshalAs(UnmanagedType.Struct)]
			public LOGFONT lfCaptionFont = null;
			public int iSmCaptionWidth = 0;
			public int iSmCaptionHeight = 0;
			[MarshalAs(UnmanagedType.Struct)]
			public LOGFONT lfSmCaptionFont = null;
			public int iMenuWidth = 0;
			public int iMenuHeight = 0;
			[MarshalAs(UnmanagedType.Struct)]
			public LOGFONT lfMenuFont = null;
			[MarshalAs(UnmanagedType.Struct)]
			public LOGFONT lfStatusFont = null;
			[MarshalAs(UnmanagedType.Struct)]
			public LOGFONT lfMessageFont = null;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class LOGFONT {
			public int lfHeight = 0;
			public int lfWidth = 0;
			public int lfEscapement = 0;
			public int lfOrientation = 0;
			public int lfWeight = 0;
			public byte lfItalic = 0;
			public byte lfUnderline = 0;
			public byte lfStrikeOut = 0;
			public byte lfCharSet = 0;
			public byte lfOutPrecision = 0;
			public byte lfClipPrecision = 0;
			public byte lfQuality = 0;
			public byte lfPitchAndFamily = 0;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
			public string lfFaceName = null;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct MENUITEMINFO {
			public int cbSize;
			public int fMask;
			public int fType;
			public int fState;
			public int wID;
			public IntPtr hSubMenu;
			public IntPtr hbmpChecked;
			public IntPtr hbmpUnchecked;
			public IntPtr dwItemData;
			public string dwTypeData;
			public int cch;
			public IntPtr hbmpItem;
		}
		public class MenuFlags {
			public const int MF_INSERT = 0x00000000,
				MF_CHANGE = 0x00000080,
				MF_APPEND = 0x00000100,
				MF_DELETE = 0x00000200,
				MF_REMOVE = 0x00001000,
				MF_BYCOMMAND = 0x00000000,
				MF_BYPOSITION = 0x00000400,
				MF_SEPARATOR = 0x00000800,
				MF_ENABLED = 0x00000000,
				MF_GRAYED = 0x00000001,
				MF_DISABLED = 0x00000002,
				MF_UNCHECKED = 0x00000000,
				MF_CHECKED = 0x00000008,
				MF_USECHECKBITMAPS = 0x00000200,
				MF_STRING = 0x00000000,
				MF_BITMAP = 0x00000004,
				MF_OWNERDRAW = 0x00000100,
				MF_POPUP = 0x00000010,
				MF_MENUBARBREAK = 0x00000020,
				MF_MENUBREAK = 0x00000040,
				MF_UNHILITE = 0x00000000,
				MF_HILITE = 0x00000080,
				MF_DEFAULT = 0x00001000,
				MF_SYSMENU = 0x00002000,
				MF_HELP = 0x00004000,
				MF_RIGHTJUSTIFY = 0x00004000,
				MF_MOUSESELECT = 0x00008000,
				MIIM_BITMAP = 0x00000080,
				MIIM_CHECKMARKS = 0x00000008,
				MIIM_DATA = 0x00000020,
				MIIM_FTYPE = 0x00000100,
				MIIM_ID = 0x00000002,
				MIIM_STATE = 0x00000001,
				MIIM_STRING = 0x00000040,
				MIIM_SUBMENU = 0x00000004,
				MIIM_TYPE = 0x00000010;
		}
		public struct NativeMessage {
			public IntPtr handle;
			public int msg;
			public IntPtr wParam;
			public IntPtr lParam;
			public int time;
			public System.Drawing.Point p;
		}
		public enum ChangeWindowMessageFilterFlags {
			Add = 1,
			Remove = 2
		}
		public const int SPI_GETNONCLIENTMETRICS = 0x0029;
		#endregion Structs&Enums
		#region SecurityCritical
		static class UnsafeNativeMethods {
			[DllImport("version.dll", SetLastError = true)]
			internal static extern bool VerQueryValue(byte[] pBlock, string lpSubBlock, out IntPtr lplpBuffer, out int puLen);
			[DllImport("version.dll", SetLastError = true)]
			internal static extern int GetFileVersionInfoSize(string lptstrFilename, out int dwSize);
			[DllImport("version.dll", SetLastError = true)]
			internal static extern bool GetFileVersionInfo(string lptstrFilename, int dwHandleIgnored, int dwLen, byte[] lpData);
			[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
			internal static extern IntPtr GetFocus();
			[DllImport("ComCtl32.dll", CharSet = CharSet.Auto)]
			internal static extern int SetWindowSubclass(IntPtr hWnd, Win32SubClassProc newProc, IntPtr uIdSubclass, IntPtr dwRefData);
			[DllImport("ComCtl32.dll", CharSet = CharSet.Auto)]
			internal static extern int RemoveWindowSubclass(IntPtr hWnd, Win32SubClassProc newProc, IntPtr uIdSubclass);
			[DllImport("ComCtl32.dll", CharSet = CharSet.Auto)]
			internal static extern IntPtr DefSubclassProc(IntPtr hWnd, IntPtr Msg, IntPtr wParam, IntPtr lParam);
			[DllImport("gdi32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
			public static extern int GetTextMetricsA(HandleRef hDC, ref NativeMethods.TEXTMETRICA lptm);
			[DllImport("gdi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
			public static extern int GetTextMetricsW(HandleRef hDC, ref NativeMethods.TEXTMETRIC lptm);
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool FlashWindowEx(ref FLASHWINFO pwfi);
			[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
			internal static extern ushort GlobalAddAtom(string lpString);
			[DllImport("user32.dll")]
			public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
			[DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
			internal static extern int SHAppBarMessage(int dwMessage, ref APPBARDATA pData);
			[DllImport("user32.dll")]
			internal static extern bool DrawMenuBar(IntPtr hWnd);
			[DllImport("user32.dll")]
			internal static extern IntPtr LoadImage(IntPtr hinst, int iconId, uint uType, int cxDesired, int cyDesired, uint fuLoad);
			[DllImport("user32.dll")]
			internal static extern int DestroyIcon(IntPtr hIcon);
			[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern IntPtr CreateWindowEx(int dwExStyle, IntPtr classAtom, string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool DestroyWindow(IntPtr hwnd);
			[DllImport("user32.dll", CharSet = CharSet.Unicode)]
			internal static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
			[DllImport("user32.dll", CharSet = CharSet.Unicode)]
			internal static extern Int32 RegisterClass(ref WNDCLASS lpWndClass);
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool UnregisterClass(IntPtr classAtom, IntPtr hInstance);
			[DllImport("GDI32.dll")]
			internal static extern int RestoreDC(IntPtr hdc, int savedDC);
			[DllImport("GDI32.dll")]
			internal static extern int SaveDC(IntPtr hdc);
			[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
			internal static extern int BitBlt(HandleRef hDC, int x, int y, int nWidth, int nHeight, HandleRef hSrcDC, int xSrc, int ySrc, int dwRop);
			[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
			internal static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
			[DllImport("gdi32.dll", EntryPoint = "GdiAlphaBlend")]
			internal static extern bool AlphaBlend(IntPtr hdcDest, int nXOriginDest, int nYOriginDest,
				int nWidthDest, int nHeightDest,
				IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,
				BLENDFUNCTION blendFunction);
			[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
			internal static extern IntPtr SelectObject(HandleRef hdc, HandleRef obj);
			[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
			internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr obj);
			[DllImport("gdi32.dll")]
			internal extern static IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
			[DllImport("gdi32.dll")]
			internal static extern int GetDIBits(HandleRef hdc, HandleRef hbm, int arg1, int arg2, IntPtr arg3, ref NativeMethods.BITMAPINFO_FLAT bmi, int arg5);
			[DllImport("gdi32.dll")]
			internal static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int width, int height);
			[DllImport("gdi32.dll")]
			internal static extern IntPtr CreateCompatibleBitmap(HandleRef hDC, int width, int height);
			[DllImport("gdi32.dll")]
			internal static extern IntPtr CreateCompatibleDC(HandleRef hDC);
			[DllImport("gdi32.dll")]
			internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);
			[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
			internal static extern bool GetTextMetrics(HandleRef hdc, out DevExpress.Utils.Text.TEXTMETRIC lptm);
			[DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			public static extern int WideCharToMultiByte(int codePage, int flags, [MarshalAs(UnmanagedType.LPWStr)] string wideStr, int chars, [In] [Out] byte[] pOutBytes, int bufferBytes, IntPtr defaultChar, IntPtr pDefaultUsed);
			internal static int GetTextExtentPoint32(HandleRef hDC, string text, ref SIZE size) {
				int num = text.Length;
				int result;
				if(Marshal.SystemDefaultCharSize == 1) {
					num = WideCharToMultiByte(0, 0, text, text.Length, null, 0, IntPtr.Zero, IntPtr.Zero);
					byte[] array = new byte[num];
					WideCharToMultiByte(0, 0, text, text.Length, array, array.Length, IntPtr.Zero, IntPtr.Zero);
					num = Math.Min(text.Length, 8192);
					result = GetTextExtentPoint32A(hDC, array, num, ref size);
				}
				else {
					result = GetTextExtentPoint32W(hDC, text, text.Length, ref size);
				}
				return result;
			}
			[DllImport("gdi32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
			internal static extern int GetTextExtentPoint32A(HandleRef hDC, byte[] lpszString, int byteCount, ref SIZE size);
			[DllImport("gdi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
			internal static extern int GetTextExtentPoint32W(HandleRef hDC, [MarshalAs(UnmanagedType.LPWStr)] string text, int len, ref SIZE size);
			[DllImport("gdi32.dll")]
			public static extern bool DeleteDC(HandleRef hDC);
			[DllImport("gdi32.dll")]
			internal static extern bool DeleteDC(IntPtr hDC);
			[DllImport("gdi32.dll")]
			internal static extern bool DeleteObject(HandleRef hObject);
			[DllImport("gdi32.dll")]
			internal static extern bool DeleteObject(IntPtr hObject);
			[DllImport("gdi32.dll", SetLastError = true)]
			internal static extern IntPtr CreateDIBSection(HandleRef hdc, ref BITMAPINFO_FLAT bmi, int iUsage, ref IntPtr ppvBits, IntPtr hSection, int dwOffset);
			[DllImport("gdi32.dll", SetLastError = true)]
			internal static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO_SMALL bmi, int iUsage, int pvvBits, IntPtr hSection, int dwOffset);
			[DllImport("gdi32.dll")]
			internal static extern int GetPaletteEntries(IntPtr hPal, int startIndex, int entries, byte[] palette);
			[DllImport("comctl32.dll", ExactSpelling = true)]
			internal static extern bool _TrackMouseEvent(TRACKMOUSEEVENTStruct tme);
			[DllImport("user32.dll")]
			internal static extern IntPtr TrackPopupMenu(IntPtr menuHandle, int uFlags, int x, int y, int nReserved, IntPtr hwnd, IntPtr par);
			[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
			internal static extern bool GetViewportOrgEx(IntPtr hDC, ref POINT point);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern bool ScrollWindowEx(IntPtr hWnd, int nXAmount, int nYAmount, RECT rectScrollRegion, ref RECT rectClip, IntPtr hrgnUpdate, ref RECT prcUpdate, int flags);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern bool ScrollWindowEx(IntPtr hWnd, int nXAmount, int nYAmount, IntPtr rectScrollRegion, ref RECT rectClip, IntPtr hrgnUpdate, ref RECT prcUpdate, int flags);
			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			internal static extern int ScrollWindowEx(IntPtr hWnd, int dx, int dy, ref NativeMethods.RECT scrollRect, ref NativeMethods.RECT clipRect, IntPtr hrgnUpdate, IntPtr updateRect, int flags);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern bool ScrollWindow(IntPtr hWnd, int nXAmount, int nYAmount, ref NativeMethods.RECT rectScrollRegion, ref NativeMethods.RECT rectClip);
			[DllImport("USER32.dll")]
			internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
			[DllImport("USER32.dll")]
			internal static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, int flags);
			[DllImport("USER32.dll")]
			internal static extern IntPtr GetWindowDC(IntPtr hwnd);
			[DllImport("USER32.dll")]
			internal static extern int GetClassLong(IntPtr hwnd, int flags);
			[DllImport("USER32.dll")]
			internal static extern int GetWindowLong(IntPtr hwnd, int flags);
			[DllImport("USER32.dll")]
			internal static extern int SetWindowLong(IntPtr hwnd, int flags, int val);
			[DllImport("user32.dll")]
			internal static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
			[DllImport("USER32.dll")]
			internal static extern IntPtr GetDesktopWindow();
			[DllImport("USER32.dll")]
			internal static extern bool RedrawWindow(IntPtr hwnd, IntPtr rcUpdate, IntPtr hrgnUpdate, int flags);
			[DllImport("USER32.dll")]
			internal static extern short GetAsyncKeyState(int vKey);
			[DllImport("USER32.dll")]
			internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
				int X, int Y, int cx, int cy, int uFlags);
			[DllImport("USER32.dll")]
			internal static extern int SetCapture(IntPtr hWnd);
			[DllImport("USER32.dll")]
			internal static extern bool ReleaseCapture();
			[DllImport("USER32.dll")]
			internal static extern bool IsWindowVisible(IntPtr hWnd);
			[DllImport("USER32.dll", CharSet = CharSet.Auto)]
			internal static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
			[DllImport("USER32.dll", CharSet = CharSet.Auto)]
			internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, int lParam);
			[DllImport("USER32.dll", CharSet = CharSet.Auto)]
			internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
			[DllImport("USER32.dll")]
			internal static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
			[DllImport("user32.dll")]
			internal static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);
			[DllImport("USER32.dll")]
			internal static extern int PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
			[DllImport("USER32.dll")]
			internal static extern bool IsZoomed(IntPtr hwnd);
			[DllImport("USER32.dll")]
			internal static extern bool IsIconic(IntPtr hwnd);
			[DllImport("USER32.dll")]
			internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
			[DllImport("USER32.dll")]
			internal static extern bool ValidateRect(IntPtr hwnd, ref RECT lpRect);
			[DllImport("User32.dll")]
			internal static extern int GetUpdateRect(IntPtr hwnd, ref RECT rect, bool erase);
			[DllImport("USER32.dll")]
			internal static extern IntPtr BeginPaint(IntPtr hWnd, [In, Out] ref PAINTSTRUCT lpPaint);
			[DllImport("USER32.dll")]
			internal static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);
			[DllImport("USER32.dll")]
			internal static extern bool LockWindowUpdate(IntPtr hWndLock);
			[DllImport("USER32.dll")]
			internal static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool redraw);
			[DllImport("gdi32.dll")]
			internal static extern int GetClipBox(IntPtr hdc, out NativeMethods.RECT lprc);
			[DllImport("GDI32.dll")]
			internal static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int fnCombineMode);
			[DllImport("GDI32.dll")]
			internal static extern int ExcludeClipRect(IntPtr hdc, int left, int top, int right, int bottom);
			[DllImport("GDI32.dll")]
			internal static extern int GetClipRgn(IntPtr hdc, IntPtr hrgn);
			[DllImport("GDI32.dll")]
			internal static extern int SelectClipRgn(IntPtr hdc, IntPtr hrgn);
			[DllImport("GDI32.dll")]
			internal static extern int ExtSelectClipRgn(IntPtr hdc, IntPtr hrgn, int mode);
			[DllImport("gdi32.dll")]
			internal static extern bool LPtoDP(IntPtr hdc, [In, Out] POINT[] lpPoints, int nCount);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern bool GetUpdateRgn(IntPtr hwnd, IntPtr hrgn, bool fErase);
			[DllImport("gdi32.dll")]
			internal static extern int GetRegionData(IntPtr hRgn, int dwCount, IntPtr lpRgnData);
			[DllImport("gdi32.dll")]
			internal static extern int OffsetRgn(IntPtr hrgn, int nXOffset, int nYOffset);
			[DllImport("user32.dll", SetLastError = true)]
			internal static extern int MapWindowPoints(IntPtr hwndFrom, IntPtr hwndTo, ref POINT lpPoints, [MarshalAs(UnmanagedType.U4)] int cPoints);
			[DllImport("gdi32.dll")]
			internal static extern int GetRandomRgn(IntPtr hdc, IntPtr hrgn, int iNum);
			[DllImport("GDI32.dll")]
			internal static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
			[DllImport("GDI32.dll")]
			internal static extern bool RectVisible(IntPtr hdc, ref NativeMethods.RECT rect);
			[DllImport("User32.dll", CharSet = CharSet.Auto)]
			internal static extern bool DragDetect(IntPtr hwnd, POINT pt);
			[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
			internal static extern int GetObject(IntPtr hObject, int nSize, [In, Out] LOGFONT lf);
			[DllImport("gdi32.dll")]
			internal static extern IntPtr SelectPalette(IntPtr hdc, IntPtr hpal, bool bForceBackground);
			[DllImport("gdi32.dll")]
			internal static extern int RealizePalette(IntPtr hdc);
			[DllImport("User32.dll")]
			internal static extern HDC GetDC(HWND handle);
			[DllImport("Gdi32.dll")]
			internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
			[DllImport("User32.dll")]
			internal static extern IntPtr GetCursor();
			[DllImport("User32.dll")]
			internal static extern bool SetSystemCursor(IntPtr hCursor, int id);
			[DllImport("Gdi32.dll")]
			internal static extern IntPtr CreateSolidBrush(COLORREF aColorRef);
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool FillRect(HDC hdc, ref RECT rect, IntPtr hbrush);
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool FillRect(IntPtr hdc, ref RECT rect, IntPtr hbrush);
			[DllImport("GDI32.dll")]
			internal static extern bool FillRgn(IntPtr hdc, IntPtr hrgn, IntPtr brush);
			[DllImport("gdi32.dll")]
			internal static extern int GetPixel(IntPtr hdc, int nXPos, int nYPos);
			[DllImport("gdi32.dll")]
			internal static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, int dwRop);
			[DllImport("User32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool UnhookWindowsHookEx(HHOOK aHook);
			[DllImport("User32.dll")]
			internal static extern IntPtr CopyIcon(IntPtr hCursor);
			[DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			internal static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, int dwThreadId);
			[DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
			[DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			internal static extern IntPtr GetModuleHandle(string lpModuleName);
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
			public static extern bool UpdateLayeredWindow(IntPtr hwnd,
				IntPtr hdcDst, ref NativeMethods.POINT pptDst, ref NativeMethods.SIZE pSizeDst,
				IntPtr hdcSrc, ref NativeMethods.POINT pptSrc,
				int crKey, ref BLENDFUNCTION pBlend, int dwFlags);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern bool InvalidateRgn(IntPtr hWnd, IntPtr hrgn, bool erase);
			[DllImport("user32.dll")]
			internal static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, uint dwFlags);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern bool AdjustWindowRectEx(ref NativeMethods.RECT lpRect, int dwStyle, bool bMenu, int dwExStyle);
			[DllImport("user32.dll")]
			internal static extern IntPtr FindWindow(string className, string windowText);
			[DllImport("user32.dll")]
			internal static extern int ShowWindow(IntPtr hWnd, int command);
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);
			[DllImport("User32.dll")]
			internal static extern IntPtr WindowFromPoint(Point pt);
			[DllImport("User32.dll")]
			internal static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);
			[DllImport("User32.dll")]
			internal static extern IntPtr SetActiveWindow(IntPtr hWnd);
			[DllImport("User32.dll")]
			internal static extern bool SetForegroundWindow(IntPtr hWnd);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern bool EnableWindow(IntPtr hWnd, bool enable);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern bool IsWindowEnabled(IntPtr hWnd);
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			internal static extern bool SystemParametersInfo(int uiAction, int uiParam, [In, Out] NONCLIENTMETRICS pvParam, int fWinIni);
			[DllImport("user32")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool SetGestureConfig(IntPtr hWnd, int dwReserved, int cIDs, [In, Out] GESTURECONFIG[] pGestureConfig, int cbSize);
			[DllImport("UxTheme")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool BeginPanningFeedback(IntPtr hWnd);
			[DllImport("UxTheme")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool EndPanningFeedback(IntPtr hWnd, bool fAnimateBack);
			[DllImport("UxTheme")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool UpdatePanningFeedback(IntPtr hwnd, int lTotalOverpanOffsetX, int lTotalOverpanOffsetY, bool fInInertia);
			[DllImport("user32")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool CloseGestureInfoHandle(IntPtr hGestureInfo);
			[DllImport("user32")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool GetGestureInfo(IntPtr hGestureInfo, ref GESTUREINFO pGestureInfo);
			[DllImport("dwmapi.dll")]
			internal static extern int DwmSetIconicThumbnail(IntPtr hwnd, IntPtr hbitmap, uint flags);
			[DllImport("dwmapi.dll")]
			internal static extern int DwmSetIconicLivePreviewBitmap(IntPtr hwnd, IntPtr hbitmap, IntPtr ptClient, uint flags);
			[DllImport("dwmapi.dll")]
			internal static extern int DwmInvalidateIconicBitmaps(IntPtr hwnd);
			[DllImport("dwmapi.dll")]
			internal static extern int DwmSetWindowAttribute(IntPtr hwnd, uint dwAttributeToSet, IntPtr pvAttributeValue, uint cbAttribute);
			[DllImport("user32.dll")]
			internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool ClientToScreen(IntPtr hwnd, ref POINT point);
			[DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			internal static extern int ExtractIconEx(string fileName, int iconStartingIndex, IntPtr[] largeIcons, IntPtr[] smallIcons, int iconCount);
			[DllImport("shell32.dll")]
			internal static extern void SHAddToRecentDocs(ShellAddToRecentDocs flags, [MarshalAs(UnmanagedType.LPWStr)] string path);
			[DllImport("Ole32.dll", PreserveSig = false)]
			internal extern static void PropVariantClear([In, Out] PropVariant pvar);
			[DllImport("user32.dll", CharSet = CharSet.Unicode)]
			internal static extern uint RegisterWindowMessage(string lpProcName);
			[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
			internal static extern IntPtr RemoveProp(IntPtr hWnd, string lpString);
			[DllImport("user32.dll", SetLastError = true)]
			internal static extern bool SetProp(IntPtr hWnd, string lpString, IntPtr hData);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern IntPtr GetCapture();
			[DllImport("user32.dll")]
			internal static extern void PostQuitMessage(int exitCode);
			[DllImport("kernel32.dll", SetLastError = true)]
			internal static extern IntPtr LocalFree(IntPtr p);
			[DllImport("kernel32.dll", SetLastError = true)]
			internal static extern IntPtr LocalAlloc(int flag, int size);
			[DllImport("user32.dll")]
			internal static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
			[DllImport("shell32.dll", SetLastError = true)]
			internal static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);
			[DllImport("gdi32.dll")]
			internal static extern bool SetViewportOrgEx(IntPtr hdc, int X, int Y, out POINT lpPoint);
			[DllImport("user32")]
			internal static extern bool ChangeWindowMessageFilter(int msg, ChangeWindowMessageFilterFlags flags);
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			internal static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			internal static extern bool RemoveMenu(IntPtr hMenu, int uPosition, int uFlags);
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			internal static extern bool GetMenuItemInfo(IntPtr hMenu, int uItem, bool fByPosition, ref MENUITEMINFO lpmii);
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			internal static extern bool EnableMenuItem(IntPtr hMenu, int uIDEnableItem, int uEnable);
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			internal static extern int GetMenuItemCount(IntPtr hMenu);
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool PeekMessage(out NativeMessage lpMsg, IntPtr hWnd, int wMsgFilterMin, int wMsgFilterMax, int wRemoveMsg);
		}
		#endregion SecurityCritical
		public static bool IsWindowOnCurrentDesktop(IntPtr hWnd) {
			if(!IsWindow10)
				return true;
			VirtualDesktopManager manager = new VirtualDesktopManager();
			IVirtualDesktopManager imanager = (IVirtualDesktopManager)manager;
			Int32 isOnCurrentDesktop = 0;
			IntPtr res = imanager.IsWindowOnCurrentVirtualDesktop(hWnd, out isOnCurrentDesktop);
			return isOnCurrentDesktop > 0;
		}
		static bool? isWindow10;
		public static bool IsWindow10 {
			get {
				if(isWindow10.HasValue)
					return isWindow10.Value;
				string fileName = Environment.GetEnvironmentVariable("windir") + "\\System32\\kernel32.dll";
				int dwSize = 0;
				int size = GetFileVersionInfoSize(fileName, out dwSize);
				byte[] data = new byte[size];
				bool res = GetFileVersionInfo(fileName, 0, size, data);
				if(!res) {
					isWindow10 = false;
					return isWindow10.Value;
				}
				IntPtr addr = IntPtr.Zero;
				int puLen = 0;
				res = VerQueryValue(data, "\\VarFileInfo\\Translation", out addr, out puLen);
				LanguageCodePage str = (LanguageCodePage)Marshal.PtrToStructure(addr, typeof(LanguageCodePage));
				string paramName = string.Format("\\StringFileInfo\\{0:X4}{1:X4}\\ProductVersion", str.language, str.codePage);
				res = VerQueryValue(data, paramName, out addr, out puLen);
				string productVersion = Marshal.PtrToStringAnsi(addr);
				if(!res) {
					isWindow10 = false;
					return isWindow10.Value;
				}
				isWindow10 = productVersion.StartsWith("10.");
				return isWindow10.Value;
			}
		}
		internal struct LanguageCodePage {
			internal short language;
			internal short codePage;
			internal void SomeMethod() {
				language = 0;
				codePage = 0;
			}
		}
		public static bool VerQueryValue(byte[] pBlock, string lpSubBlock, out IntPtr lplpBuffer, out int puLen) {
			return UnsafeNativeMethods.VerQueryValue(pBlock, lpSubBlock, out lplpBuffer, out puLen);
		}
		public static int GetFileVersionInfoSize(string lptstrFilename, out int dwSize) {
			return UnsafeNativeMethods.GetFileVersionInfoSize(lptstrFilename, out dwSize);
		}
		public static bool GetFileVersionInfo(string lptstrFilename, int dwHandleIgnored, int dwLen, byte[] lpData) {
			return UnsafeNativeMethods.GetFileVersionInfo(lptstrFilename, dwHandleIgnored, dwLen, lpData);
		}
		public delegate IntPtr Win32SubClassProc(IntPtr hWnd, IntPtr Msg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, IntPtr dwRefData);
		public static int SetWindowSubclass(IntPtr hWnd, Win32SubClassProc newProc, IntPtr uIdSubclass, IntPtr dwRefData) {
			return UnsafeNativeMethods.SetWindowSubclass(hWnd, newProc, uIdSubclass, dwRefData);
		}
		public static IntPtr GetFocus() { return UnsafeNativeMethods.GetFocus(); }
		public static int RemoveWindowSubclass(IntPtr hWnd, Win32SubClassProc newProc, IntPtr uIdSubclass) {
			return UnsafeNativeMethods.RemoveWindowSubclass(hWnd, newProc, uIdSubclass);
		}
		public static IntPtr DefSubclassProc(IntPtr hWnd, IntPtr Msg, IntPtr wParam, IntPtr lParam) {
			return UnsafeNativeMethods.DefSubclassProc(hWnd, Msg, wParam, lParam);
		}
		public static bool FlashWindowEx(IntPtr hWnd, FLASHW flags, int count, int timeout) {
			FLASHWINFO fInfo = new FLASHWINFO();
			fInfo.cbSize = (uint)(Marshal.SizeOf(fInfo));
			fInfo.hwnd = hWnd;
			fInfo.dwFlags = (uint)flags;
			fInfo.uCount = (uint)count;
			fInfo.dwTimeout = (uint)timeout;
			return FlashWindowEx(fInfo);
		}
		internal static bool FlashWindowEx(FLASHWINFO info) {
			return UnsafeNativeMethods.FlashWindowEx(ref info);
		}
		internal static int SHAppBarMessage(int dwMessage, ref APPBARDATA pData) {
			return UnsafeNativeMethods.SHAppBarMessage(dwMessage, ref pData);
		}
		public static bool DrawMenuBar(IntPtr hWnd) {
			return UnsafeNativeMethods.DrawMenuBar(hWnd);
		}
		public static short GlobalAddAtom(string lpString) {
			return (short)UnsafeNativeMethods.GlobalAddAtom(lpString);
		}
		public static IntPtr RemoveProp(IntPtr hWnd, string lpString) {
			return UnsafeNativeMethods.RemoveProp(hWnd, lpString);
		}
		public static IntPtr GetCapture() {
			return UnsafeNativeMethods.GetCapture();
		}
		public static bool SetProp(IntPtr hWnd, string lpString, IntPtr hData) {
			return UnsafeNativeMethods.SetProp(hWnd, lpString, hData);
		}
		public static IntPtr LoadImage(IntPtr hinst, int iconId, int uType, int cxDesired, int cyDesired, int fuLoad) {
			return UnsafeNativeMethods.LoadImage(hinst, iconId, (uint)uType, cxDesired, cyDesired, (uint)fuLoad);
		}
		public static int DestroyIcon(IntPtr hIcon) {
			return UnsafeNativeMethods.DestroyIcon(hIcon);
		}
		public static IntPtr CreateWindowEx(int dwExStyle, IntPtr classAtom, string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam) {
			return UnsafeNativeMethods.CreateWindowEx(dwExStyle, classAtom, lpWindowName, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);
		}
		public static bool DestroyWindow(IntPtr hWnd) {
			return UnsafeNativeMethods.DestroyWindow(hWnd);
		}
		public static IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam) {
			return UnsafeNativeMethods.DefWindowProc(hWnd, msg, wParam, lParam);
		}
		public static Int32 RegisterClass(ref WNDCLASS lpWndClass) {
			return UnsafeNativeMethods.RegisterClass(ref lpWndClass);
		}
		public static bool UnregisterClass(IntPtr classAtom, IntPtr hInstance) {
			return UnsafeNativeMethods.UnregisterClass(classAtom, hInstance);
		}
		public static int RestoreDC(IntPtr hdc, int savedDC) {
			return UnsafeNativeMethods.RestoreDC(hdc, savedDC);
		}
		public static int SaveDC(IntPtr hdc) {
			return UnsafeNativeMethods.SaveDC(hdc);
		}
		public static int BitBlt(HandleRef hDC, int x, int y, int nWidth, int nHeight, HandleRef hSrcDC, int xSrc, int ySrc, int dwRop) {
			return UnsafeNativeMethods.BitBlt(hDC, x, y, nWidth, nHeight, hSrcDC, xSrc, ySrc, dwRop);
		}
		public static int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop) {
			return UnsafeNativeMethods.BitBlt(hDC, x, y, nWidth, nHeight, hSrcDC, xSrc, ySrc, dwRop);
		}
		public static bool AlphaBlend(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hdcSrc, int xSrc, int ySrc, int nWidthSrc, int nHeightSrc, BLENDFUNCTION blendFunction) {
			return UnsafeNativeMethods.AlphaBlend(hDC, x, y, nWidth, nHeight, hdcSrc, xSrc, ySrc, nWidthSrc, nHeightSrc, blendFunction);
		}
		public static IntPtr SelectObject(HandleRef hdc, HandleRef obj) {
			return UnsafeNativeMethods.SelectObject(hdc, obj);
		}
		public static bool GetClientRect(IntPtr hWnd, out NativeMethods.RECT rect) {
			return UnsafeNativeMethods.GetClientRect(hWnd, out rect);
		}
		public static IntPtr SelectObject(IntPtr hdc, IntPtr obj) {
			return UnsafeNativeMethods.SelectObject(hdc, obj);
		}
		public static IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse) {
			return UnsafeNativeMethods.CreateRoundRectRgn(nLeftRect, nTopRect, nRightRect, nBottomRect, nWidthEllipse, nHeightEllipse);
		}
		public static int GetDIBits(HandleRef hdc, HandleRef hbm, int arg1, int arg2, IntPtr arg3, ref NativeMethods.BITMAPINFO_FLAT bmi, int arg5) {
			return UnsafeNativeMethods.GetDIBits(hdc, hbm, arg1, arg2, arg3, ref bmi, arg5);
		}
		public static IntPtr CreateCompatibleBitmap(IntPtr hDC, int width, int height) {
			return UnsafeNativeMethods.CreateCompatibleBitmap(hDC, width, height);
		}
		public static IntPtr CreateCompatibleBitmap(HandleRef hDC, int width, int height) {
			return UnsafeNativeMethods.CreateCompatibleBitmap(hDC, width, height);
		}
		public static IntPtr CreateCompatibleDC(HandleRef hDC) {
			return UnsafeNativeMethods.CreateCompatibleDC(hDC);
		}
		public static bool GetTextMetrics(HandleRef hDC, out DevExpress.Utils.Text.TEXTMETRIC lptm) {
			return UnsafeNativeMethods.GetTextMetrics(hDC, out lptm);
		}
		public static IntPtr CreateCompatibleDC(IntPtr hDC) {
			return UnsafeNativeMethods.CreateCompatibleDC(hDC);
		}
		public static bool DeleteDC(HandleRef hDC) {
			return UnsafeNativeMethods.DeleteDC(hDC);
		}
		public static bool DeleteDC(IntPtr hDC) {
			return UnsafeNativeMethods.DeleteDC(hDC);
		}
		public static bool DeleteObject(HandleRef hObject) {
			return UnsafeNativeMethods.DeleteObject(hObject);
		}
		public static bool DeleteObject(IntPtr hObject) {
			return UnsafeNativeMethods.DeleteObject(hObject);
		}
		public static IntPtr CreateDIBSection(HandleRef hdc, ref BITMAPINFO_FLAT bmi, int iUsage, ref IntPtr ppvBits, IntPtr hSection, int dwOffset) {
			return UnsafeNativeMethods.CreateDIBSection(hdc, ref bmi, iUsage, ref ppvBits, hSection, dwOffset);
		}
		public static IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO_SMALL bmi, int iUsage, int pvvBits, IntPtr hSection, int dwOffset) {
			return UnsafeNativeMethods.CreateDIBSection(hdc, ref bmi, iUsage, pvvBits, hSection, dwOffset);
		}
		public static int GetPaletteEntries(IntPtr hPal, int startIndex, int entries, byte[] palette) {
			return UnsafeNativeMethods.GetPaletteEntries(hPal, startIndex, entries, palette);
		}
		public static bool TrackMouseEvent(TRACKMOUSEEVENTStruct tme) {
			return UnsafeNativeMethods._TrackMouseEvent(tme);
		}
		public static IntPtr TrackPopupMenu(IntPtr menuHandle, int uFlags, int x, int y, int nReserved, IntPtr hwnd, IntPtr par) {
			return UnsafeNativeMethods.TrackPopupMenu(menuHandle, uFlags, x, y, nReserved, hwnd, par);
		}
		public static bool GetViewportOrgEx(IntPtr hDC, ref POINT point) {
			return UnsafeNativeMethods.GetViewportOrgEx(hDC, ref point);
		}
		public static bool ScrollWindowEx(IntPtr hWnd, int nXAmount, int nYAmount, RECT rectScrollRegion, ref RECT rectClip, IntPtr hrgnUpdate, ref RECT prcUpdate, int flags) {
			return UnsafeNativeMethods.ScrollWindowEx(hWnd, nXAmount, nYAmount, rectScrollRegion, ref rectClip, hrgnUpdate, ref prcUpdate, flags);
		}
		public static bool ScrollWindowEx(IntPtr hWnd, int nXAmount, int nYAmount, IntPtr rectScrollRegion, ref RECT rectClip, IntPtr hrgnUpdate, ref RECT prcUpdate, int flags) {
			return UnsafeNativeMethods.ScrollWindowEx(hWnd, nXAmount, nYAmount, rectScrollRegion, ref rectClip, hrgnUpdate, ref prcUpdate, flags);
		}
		public static int ScrollWindowEx(IntPtr hWnd, int dx, int dy, ref NativeMethods.RECT scrollRect, ref NativeMethods.RECT clipRect, IntPtr hrgnUpdate, IntPtr updateRect, int flags) {
			return UnsafeNativeMethods.ScrollWindowEx(hWnd, dx, dy, ref scrollRect, ref clipRect, hrgnUpdate, updateRect, flags);
		}
		public static bool ScrollWindow(IntPtr hWnd, int nXAmount, int nYAmount, ref NativeMethods.RECT rectScrollRegion, ref NativeMethods.RECT rectClip) {
			return UnsafeNativeMethods.ScrollWindow(hWnd, nXAmount, nYAmount, ref rectScrollRegion, ref rectClip);
		}
		public static int ReleaseDC(IntPtr hWnd, IntPtr hDC) {
			return UnsafeNativeMethods.ReleaseDC(hWnd, hDC);
		}
		public static IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, int flags) {
			return UnsafeNativeMethods.GetDCEx(hWnd, hrgnClip, flags);
		}
		public static IntPtr GetWindowDC(IntPtr hWnd) {
			return UnsafeNativeMethods.GetWindowDC(hWnd);
		}
		public static int GetClassLong(IntPtr hWnd, int flags) {
			return UnsafeNativeMethods.GetClassLong(hWnd, flags);
		}
		public static int GetWindowLong(IntPtr hWnd, int flags) {
			return UnsafeNativeMethods.GetWindowLong(hWnd, flags);
		}
		public static int SetWindowLong(IntPtr hWnd, int flags, int val) {
			return UnsafeNativeMethods.SetWindowLong(hWnd, flags, val);
		}
		public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong) {
			return UnsafeNativeMethods.SetWindowLong(hWnd, nIndex, dwNewLong);
		}
		public static IntPtr GetDesktopWindow() {
			return UnsafeNativeMethods.GetDesktopWindow();
		}
		public static bool RedrawWindow(IntPtr hWnd, IntPtr rcUpdate, IntPtr hrgnUpdate, int flags) {
			return UnsafeNativeMethods.RedrawWindow(hWnd, rcUpdate, hrgnUpdate, flags);
		}
		public static short GetAsyncKeyState(int vKey) {
			return UnsafeNativeMethods.GetAsyncKeyState(vKey);
		}
		public static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags) {
			return UnsafeNativeMethods.SetWindowPos(hWnd, hWndInsertAfter, x, y, cx, cy, uFlags);
		}
		public static int SetCapture(IntPtr hWnd) {
			return UnsafeNativeMethods.SetCapture(hWnd);
		}
		public static bool ReleaseCapture() {
			return UnsafeNativeMethods.ReleaseCapture();
		}
		public static bool IsWindowVisible(IntPtr hWnd) {
			return UnsafeNativeMethods.IsWindowVisible(hWnd);
		}
		public static IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, int lParam) {
			return UnsafeNativeMethods.SendMessage(hWnd, Msg, wParam, lParam);
		}
		public static bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow) {
			return UnsafeNativeMethods.ShowScrollBar(hWnd, wBar, bShow);
		}
		public static IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam) {
			return UnsafeNativeMethods.SendMessage(hWnd, Msg, wParam, lParam);
		}
		public static int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam) {
			return UnsafeNativeMethods.SendMessage(hWnd, Msg, wParam, lParam);
		}
		public static int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT cds) {
			return UnsafeNativeMethods.SendMessage(hWnd, Msg, wParam, ref cds);
		}
		public static int PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam) {
			return UnsafeNativeMethods.PostMessage(hWnd, Msg, wParam, lParam);
		}
		public static bool IsZoomed(IntPtr hWnd) {
			return UnsafeNativeMethods.IsZoomed(hWnd);
		}
		public static bool IsIconic(IntPtr hWnd) {
			return UnsafeNativeMethods.IsIconic(hWnd);
		}
		public static bool GetWindowRect(IntPtr hWnd, ref RECT lpRect) {
			return UnsafeNativeMethods.GetWindowRect(hWnd, ref lpRect);
		}
		public static bool ValidateRect(IntPtr hWnd, ref RECT lpRect) {
			return UnsafeNativeMethods.ValidateRect(hWnd, ref lpRect);
		}
		public static IntPtr BeginPaint(IntPtr hWnd, [In, Out] ref PAINTSTRUCT lpPaint) {
			return UnsafeNativeMethods.BeginPaint(hWnd, ref lpPaint);
		}
		public static bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint) {
			return UnsafeNativeMethods.EndPaint(hWnd, ref lpPaint);
		}
		public static bool LockWindowUpdate(IntPtr hWnd) {
			return UnsafeNativeMethods.LockWindowUpdate(hWnd);
		}
		public static int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool redraw) {
			return UnsafeNativeMethods.SetWindowRgn(hWnd, hRgn, redraw);
		}
		public static int GetClipBox(IntPtr hdc, out RECT lprc) {
			return UnsafeNativeMethods.GetClipBox(hdc, out lprc);
		}
		public static int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int fnCombineMode) {
			return UnsafeNativeMethods.CombineRgn(hrgnDest, hrgnSrc1, hrgnSrc2, fnCombineMode);
		}
		public static int ExcludeClipRect(IntPtr hdc, int left, int top, int right, int bottom) {
			return UnsafeNativeMethods.ExcludeClipRect(hdc, left, top, right, bottom);
		}
		public static int GetClipRgn(IntPtr hdc, IntPtr hrgn) {
			return UnsafeNativeMethods.GetClipRgn(hdc, hrgn);
		}
		public static int SelectClipRgn(IntPtr hdc, IntPtr hrgn) {
			return UnsafeNativeMethods.SelectClipRgn(hdc, hrgn);
		}
		public static int ExtSelectClipRgn(IntPtr hdc, IntPtr hrgn, int mode) {
			return UnsafeNativeMethods.ExtSelectClipRgn(hdc, hrgn, mode);
		}
		public static bool LPtoDP(IntPtr hdc, [In, Out] POINT[] lpPoints, int nCount) {
			return UnsafeNativeMethods.LPtoDP(hdc, lpPoints, nCount);
		}
		public static int GetUpdateRect(IntPtr hWnd, ref RECT lpRect, bool erase) {
			return UnsafeNativeMethods.GetUpdateRect(hWnd, ref lpRect, erase);
		}
		public static bool GetUpdateRgn(IntPtr hWnd, IntPtr hrgn, bool erase) {
			return UnsafeNativeMethods.GetUpdateRgn(hWnd, hrgn, erase);
		}
		public static int GetRegionData(IntPtr hRgn, int dwCount, IntPtr lpRgnData) {
			return UnsafeNativeMethods.GetRegionData(hRgn, dwCount, lpRgnData);
		}
		public static int OffsetRgn(IntPtr hRgn, int nXOffset, int nYOffset) {
			return UnsafeNativeMethods.OffsetRgn(hRgn, nXOffset, nYOffset);
		}
		public static int MapWindowPoints(IntPtr hwndFrom, IntPtr hwndTo, ref POINT lpPoints, [MarshalAs(UnmanagedType.U4)] int cPoints) {
			return UnsafeNativeMethods.MapWindowPoints(hwndFrom, hwndTo, ref lpPoints, cPoints);
		}
		public static int GetRandomRgn(IntPtr hdc, IntPtr hrgn, int iNum) {
			return UnsafeNativeMethods.GetRandomRgn(hdc, hrgn, iNum);
		}
		public static IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect) {
			return UnsafeNativeMethods.CreateRectRgn(nLeftRect, nTopRect, nRightRect, nBottomRect);
		}
		public static bool RectVisible(IntPtr hdc, ref NativeMethods.RECT rect) {
			return UnsafeNativeMethods.RectVisible(hdc, ref rect);
		}
		public static bool DragDetect(IntPtr hWnd, POINT pt) {
			return UnsafeNativeMethods.DragDetect(hWnd, pt);
		}
		public static int GetObject(IntPtr hObject, int nSize, [In, Out] LOGFONT lf) {
			return UnsafeNativeMethods.GetObject(hObject, nSize, lf);
		}
		public static IntPtr SelectPalette(IntPtr hdc, IntPtr hpal, bool bForceBackground) {
			return UnsafeNativeMethods.SelectPalette(hdc, hpal, bForceBackground);
		}
		public static int RealizePalette(IntPtr hdc) {
			return UnsafeNativeMethods.RealizePalette(hdc);
		}
		public static HDC GetDC(HWND handle) {
			return UnsafeNativeMethods.GetDC(handle);
		}
		public static int GetDeviceCaps(HDC hdc, int nIndex) {
			return UnsafeNativeMethods.GetDeviceCaps(hdc, nIndex);
		}
		public static IntPtr GetCursor() {
			return UnsafeNativeMethods.GetCursor();
		}
		public static bool SetSystemCursor(IntPtr hCursor, int id) {
			return UnsafeNativeMethods.SetSystemCursor(hCursor, id);
		}
		internal static IntPtr CreateSolidBrush(COLORREF aColorRef) {
			return UnsafeNativeMethods.CreateSolidBrush(aColorRef);
		}
		public static bool FillRect(IntPtr hdc, ref RECT rect, IntPtr hbrush) {
			return UnsafeNativeMethods.FillRect(hdc, ref rect, hbrush);
		}
		public static bool FillRect(HDC hdc, ref RECT rect, IntPtr hbrush) {
			return UnsafeNativeMethods.FillRect(hdc, ref rect, hbrush);
		}
		public static bool FillRgn(IntPtr hdc, IntPtr hrgn, IntPtr hbrush) {
			return UnsafeNativeMethods.FillRgn(hdc, hrgn, hbrush);
		}
		public static int GetPixel(IntPtr hdc, int nXPos, int nYPos) {
			return UnsafeNativeMethods.GetPixel(hdc, nXPos, nYPos);
		}
		public static bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, int dwRop) {
			return UnsafeNativeMethods.StretchBlt(hdcDest, nXOriginDest, nYOriginDest, nWidthDest, nHeightDest, hdcSrc, nXOriginSrc, nYOriginSrc, nWidthSrc, nHeightSrc, dwRop);
		}
		public static bool UnhookWindowsHookEx(HHOOK aHook) {
			return UnsafeNativeMethods.UnhookWindowsHookEx(aHook);
		}
		public static IntPtr CopyIcon(IntPtr hCursor) {
			return UnsafeNativeMethods.CopyIcon(hCursor);
		}
		public static IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, int dwThreadId) {
			return UnsafeNativeMethods.SetWindowsHookEx(idHook, lpfn, hMod, dwThreadId);
		}
		public static IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam) {
			return UnsafeNativeMethods.CallNextHookEx(hhk, nCode, wParam, lParam);
		}
		public static IntPtr GetModuleHandle(string lpModuleName) {
			return UnsafeNativeMethods.GetModuleHandle(lpModuleName);
		}		
		public static bool SetViewportOrgEx(IntPtr hDC, int x, int y, out POINT point) {
			return UnsafeNativeMethods.SetViewportOrgEx(hDC, x, y, out point); 
		}
		public static bool ChangeWindowMessageFilter(int msg, ChangeWindowMessageFilterFlags flags) {
			return UnsafeNativeMethods.ChangeWindowMessageFilter(msg, flags);
		}
		internal static bool IsLayoutRTL(IntPtr hwnd) {
			return (GetWindowLong(hwnd, -20) & 0x400000 ) != 0;
		}
		public static int ReadInt32(IntPtr ptr, int ofs) {
			return Marshal.ReadInt32(ptr, ofs);
		}
		public static void WriteInt32(IntPtr ptr, int ofs, int val) {
			Marshal.WriteInt32(ptr, ofs, val);
		}
		public static bool IsKeyboardContextMenuMessage(Message msg) {
			if(msg.Msg != MSG.WM_CONTEXTMENU) return false;
			Point pt = new Point(msg.LParam.ToInt32());
			if(pt.X == -1 && pt.Y == -1) return true;
			return false;
		}
		public static void ExcludeClipRect(IntPtr hdc, Rectangle rect) {
			UnsafeNativeMethods.ExcludeClipRect(hdc, rect.X, rect.Y, rect.Right, rect.Bottom);
		}
		public static Rectangle[] GetClipRectsFromHDC(IntPtr hWnd, IntPtr hdc, bool offsetPoints) {
			IntPtr rgn = NativeMethods.CreateRectRgn(0, 0, 0, 0);
			try {
				if(UnsafeNativeMethods.GetRandomRgn(hdc, rgn, SYSRGN) != 1) return null; 
				if(offsetPoints) {
					POINT pt = new POINT();
					UnsafeNativeMethods.MapWindowPoints(IntPtr.Zero, hWnd, ref pt, 1);
					if(IsLayoutRTL(hWnd)) {
						var c = Control.FromHandle(hWnd);
						if(c != null) pt.X = (pt.X * -1) + c.Width;
					}
					UnsafeNativeMethods.OffsetRgn(rgn, pt.X, pt.Y);
				}
				RECT[] apirects = RectsFromRegion(rgn);
				if(apirects == null || apirects.Length == 0) return null;
				Rectangle[] res = new Rectangle[apirects.Length];
				for(int n = 0; n < apirects.Length; n++) {
					res[n] = apirects[n].ToRectangle();
				}
				return res;
			}
			finally {
				UnsafeNativeMethods.DeleteObject(rgn);
			}
		}
		public static int SignedLOWORD(IntPtr n) {
			return SignedLOWORD((int)((long)n));
		}
		public static int SignedLOWORD(int n) {
			return (int)((short)(n & 65535));
		}
		public static int SignedHIWORD(IntPtr n) {
			return SignedHIWORD((int)((long)n));
		}
		public static int SignedHIWORD(int n) {
			return (int)((short)(n >> 16 & 65535));
		}
		public static Point FromMouseLParam(ref Message m ) {
			return new Point(SignedLOWORD(m.LParam), SignedHIWORD(m.LParam));
		}
		public static NativeMethods.RECT[] RectsFromRegion(IntPtr hRgn) {
			NativeMethods.RECT[] rects = null;
			int dataSize = UnsafeNativeMethods.GetRegionData(hRgn, 0, IntPtr.Zero);
			if(dataSize != 0) {
				IntPtr bytes = IntPtr.Zero;
				bytes = Marshal.AllocCoTaskMem(dataSize);
				int retValue = UnsafeNativeMethods.GetRegionData(hRgn, dataSize, bytes);
				RGNDATAHEADER header = (RGNDATAHEADER)Marshal.PtrToStructure(bytes, typeof(RGNDATAHEADER));
				if(header.iType == (int)NativeMethods.RegionDataHeaderTypes.Rectangles) {
					rects = new NativeMethods.RECT[header.nCount];
					int rectOffset = header.dwSize;
					for(int i = 0; i < header.nCount; i++) {
						IntPtr offset = new IntPtr(bytes.ToInt64() + rectOffset + (Marshal.SizeOf(typeof(NativeMethods.RECT)) * i));
						rects[i] = (NativeMethods.RECT)Marshal.PtrToStructure(offset, typeof(NativeMethods.RECT));
					}
				}
				if(bytes != IntPtr.Zero) Marshal.FreeCoTaskMem(bytes);
			}
			return rects;
		}
		public static IntPtr CreateSolidBrush(Color aColor) {
			return UnsafeNativeMethods.CreateSolidBrush(new COLORREF(aColor));
		}
		public static IntPtr CreateSolidBrush(int argb) {
			return UnsafeNativeMethods.CreateSolidBrush(new COLORREF(argb));
		}
		public static Region CreateRoundRegion(Rectangle windowBounds, int ellipseSize) {
			IntPtr rgn = UnsafeNativeMethods.CreateRoundRectRgn(windowBounds.X, windowBounds.Y, windowBounds.Width + 1, windowBounds.Height + 1, ellipseSize, ellipseSize);
			Region res = Region.FromHrgn(rgn);
			DeleteObject(rgn);
			return res;
		}
		public static bool UpdateLayeredWindow(IntPtr hwnd,
			IntPtr hdcDst, ref NativeMethods.POINT pptDst, ref NativeMethods.SIZE pSizeDst,
			IntPtr hdcSrc, ref NativeMethods.POINT pptSrc,
			int crKey, ref BLENDFUNCTION pBlend, int dwFlags) {
			return UnsafeNativeMethods.UpdateLayeredWindow(hwnd, hdcDst, ref pptDst, ref pSizeDst, hdcSrc, ref pptSrc, crKey, ref pBlend, dwFlags);
		}
		public static bool InvalidateRgn(IntPtr hWnd, IntPtr hrgn, bool erase) {
			return UnsafeNativeMethods.InvalidateRgn(hWnd, hrgn, erase);
		}
		public static bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, int dwFlags) {
			return UnsafeNativeMethods.SetLayeredWindowAttributes(hwnd, crKey, bAlpha, (uint)dwFlags);
		}
		public static bool AdjustWindowRectEx(ref NativeMethods.RECT lpRect, int dwStyle, bool bMenu, int dwExStyle) {
			return UnsafeNativeMethods.AdjustWindowRectEx(ref lpRect, dwStyle, bMenu, dwExStyle);
		}
		public static IntPtr FindWindow(string className, string windowText) {
			return UnsafeNativeMethods.FindWindow(className, windowText);
		}
		public static int ShowWindow(IntPtr hWnd, int command) {
			return UnsafeNativeMethods.ShowWindow(hWnd, command);
		}
		public static bool ShowWindow(IntPtr hWnd, ShowWindowCommands command) {
			return UnsafeNativeMethods.ShowWindow(hWnd, command);
		}
		public static bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl) {
			return UnsafeNativeMethods.GetWindowPlacement(hWnd, out lpwndpl);
		}
		public static IntPtr WindowFromPoint(Point pt) {
			return UnsafeNativeMethods.WindowFromPoint(pt);
		}
		public static IntPtr GetWindow(IntPtr hWnd, int wCmd) {
			return UnsafeNativeMethods.GetWindow(hWnd, (uint)wCmd);
		}
		public static IntPtr SetActiveWindow(IntPtr hWnd) {
			return UnsafeNativeMethods.SetActiveWindow(hWnd);
		}
		public static bool SetForegroundWindow(IntPtr hWnd) {
			return UnsafeNativeMethods.SetForegroundWindow(hWnd);
		}
		public static bool EnableWindow(IntPtr hWnd, bool enable) {
			return UnsafeNativeMethods.EnableWindow(hWnd, enable);
		}
		public static bool IsWindowEnabled(IntPtr hWnd) {
			return UnsafeNativeMethods.IsWindowEnabled(hWnd);
		}
		public static bool SystemParametersInfo(int uiAction, int uiParam, NONCLIENTMETRICS pvParam, int fWinIni) {
			return UnsafeNativeMethods.SystemParametersInfo(uiAction, uiParam, pvParam, fWinIni);
		}
		public static bool SetGestureConfig(IntPtr hWnd, int dwReserved, int cIDs, [In, Out] GESTURECONFIG[] pGestureConfig, int cbSize) {
			return UnsafeNativeMethods.SetGestureConfig(hWnd, dwReserved, cIDs, pGestureConfig, cbSize);
		}
		public static bool BeginPanningFeedback(IntPtr hWnd) {
			return UnsafeNativeMethods.BeginPanningFeedback(hWnd);
		}
		public static bool EndPanningFeedback(IntPtr hWnd, bool fAnimateBack) {
			return UnsafeNativeMethods.EndPanningFeedback(hWnd, fAnimateBack);
		}
		public static bool UpdatePanningFeedback(IntPtr hwnd, int lTotalOverpanOffsetX, int lTotalOverpanOffsetY, bool fInInertia) {
			return UnsafeNativeMethods.UpdatePanningFeedback(hwnd, lTotalOverpanOffsetX, lTotalOverpanOffsetY, fInInertia);
		}
		public static bool CloseGestureInfoHandle(IntPtr hGestureInfo) {
			return UnsafeNativeMethods.CloseGestureInfoHandle(hGestureInfo);
		}
		public static bool GetGestureInfo(IntPtr hGestureInfo, ref GESTUREINFO pGestureInfo) {
			return UnsafeNativeMethods.GetGestureInfo(hGestureInfo, ref pGestureInfo);
		}		
		public static int DwmSetIconicThumbnail(IntPtr hwnd, IntPtr hbitmap, int flags) {
			return UnsafeNativeMethods.DwmSetIconicThumbnail(hwnd, hbitmap, (uint)flags);
		}		
		public static int DwmSetIconicLivePreviewBitmap(IntPtr hwnd, IntPtr hbitmap, IntPtr ptClient, int flags) {
			return UnsafeNativeMethods.DwmSetIconicLivePreviewBitmap(hwnd, hbitmap, ptClient, (uint)flags);
		}
		public static int DwmSetWindowAttribute(IntPtr hwnd, int dwAttributeToSet, IntPtr pvAttributeValue, int cbAttribute) {
			return UnsafeNativeMethods.DwmSetWindowAttribute(hwnd, (uint)dwAttributeToSet, pvAttributeValue, (uint)cbAttribute);
		}
		public static int DwmInvalidateIconicBitmaps(IntPtr hwnd) {
			return UnsafeNativeMethods.DwmInvalidateIconicBitmaps(hwnd);
		}
		public static int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount) {
			return UnsafeNativeMethods.GetWindowText(hWnd, lpString, nMaxCount);
		}   
		public static bool ClientToScreen(IntPtr hwnd, ref POINT point) {
			return UnsafeNativeMethods.ClientToScreen(hwnd, ref point);
		}
		public static int ExtractIconEx(string fileName, int iconStartingIndex, IntPtr[] largeIcons, IntPtr[] smallIcons, int iconCount) {
			return UnsafeNativeMethods.ExtractIconEx(fileName, iconStartingIndex, largeIcons, smallIcons, iconCount);
		}
		public static void SHAddToRecentDocs(ShellAddToRecentDocs flags, [MarshalAs(UnmanagedType.LPWStr)] string path) {
			UnsafeNativeMethods.SHAddToRecentDocs(flags, path);
		}
		public static void PropVariantClear([In, Out] PropVariant pvar) {
			UnsafeNativeMethods.PropVariantClear(pvar);
		}
		public static int RegisterWindowMessage(string lpProcName) {
			return (int)UnsafeNativeMethods.RegisterWindowMessage(lpProcName);
		}
		public static void PostQuitMessage(int exitCode) {
			UnsafeNativeMethods.PostQuitMessage(exitCode);
		}
		public static IntPtr LocalFree(IntPtr p) {
			return UnsafeNativeMethods.LocalFree(p);
		}
		public static IntPtr LocalAlloc(int flag, int size) {
			return UnsafeNativeMethods.LocalAlloc(flag, size);
		}
		public static bool ShowWindowAsync(IntPtr hWnd, int nCmdShow) {
			return UnsafeNativeMethods.ShowWindowAsync(hWnd, nCmdShow);
		}
		public static void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID) {
			UnsafeNativeMethods.SetCurrentProcessExplicitAppUserModelID(AppID);
		}
		public static IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert) {
			return UnsafeNativeMethods.GetSystemMenu(hWnd, bRevert);
		}
		public static bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem) {
			return UnsafeNativeMethods.AppendMenu(hMenu, uFlags, uIDNewItem, lpNewItem);
		}
		public static bool RemoveMenu(IntPtr hMenu, int uPosition, int uFlags) {
			return UnsafeNativeMethods.RemoveMenu(hMenu, uPosition, uFlags);
		}
		public static void GetMenuItemInfo(IntPtr hMenu, int uItem, bool fByPosition, ref MENUITEMINFO lpmii) {
			bool a = UnsafeNativeMethods.GetMenuItemInfo(hMenu, uItem, fByPosition, ref lpmii);
		}
		public static void EnableMenuItem(IntPtr hMenu, int uIDEnableItem, int uEnable) {
			UnsafeNativeMethods.EnableMenuItem(hMenu, uIDEnableItem, uEnable);
		}
		public static int GetMenuItemCount(IntPtr hMenu) {
			return UnsafeNativeMethods.GetMenuItemCount(hMenu);
		}
		public static bool PeekMessage(out NativeMessage lpMsg, IntPtr hWnd, int wMsgFilterMin, int wMsgFilterMax, int wRemoveMsg) {
			return UnsafeNativeMethods.PeekMessage(out lpMsg, hWnd, wMsgFilterMin, wMsgFilterMax, wRemoveMsg);
		}
		public const int PM_NOREMOVE = 0x0000;
		public const int PM_REMOVE = 0x0001;
		public const int EM_SETWORDBREAKPROC = 0x00D0;
		public const int EM_GETWORDBREAKPROC = 0x00D1;
		public const int WB_LEFT = 0;
		public const int WB_RIGHT = 1;
		public const int WB_ISDELIMITER = 2;
		public delegate int EditWordBreakProc(string lpch, int ichCurrent, int cch, int code);
		public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
		public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
		public static int SYSRGN = 4;
		public const int RGN_AND = 1, RGN_OR = 2, RGN_XOR = 3, RGN_DIFF = 4, RGN_COPY = 5;
		public const int MAX_PATH = 260;
		public const int GW_HWNDFIRST = 0;
		public const int GW_HWNDLAST = 1;
		public const int GW_HWNDNEXT = 2;
		public const int GW_HWNDPREV = 3;
		public const int GW_OWNER = 4;
		public const int GW_CHILD = 5;
		public static SizeF GetFontAutoScaleDimensions(Font font, Control control) {
			using(Graphics g = control.CreateGraphics()) {
				IntPtr dc = g.GetHdc();
				try {
					return GetFontAutoScaleDimensions(font, dc);
				}
				finally {
					g.ReleaseHdc(dc);
				}
			}
		}
		public static SizeF GetFontAutoScaleDimensions(Font font) { return GetFontAutoScaleDimensions(font, IntPtr.Zero); }
		public static SizeF GetFontAutoScaleDimensions(Font font, IntPtr sourceDC) {
			object wrapper = font;
			SizeF empty = SizeF.Empty;
			IntPtr intPtrDC = sourceDC;
			if(intPtrDC == IntPtr.Zero) intPtrDC = NativeMethods.CreateCompatibleDC(IntPtr.Zero);
			if(intPtrDC == IntPtr.Zero) {
				throw new Exception();
			}
			IntPtr hfont = font.ToHfont();
			HandleRef hDC = new HandleRef(wrapper, intPtrDC);
			try {
				HandleRef hObject = new HandleRef(wrapper, hfont);
				HandleRef hObject2 = new HandleRef(wrapper, NativeMethods.SelectObject(hDC, hObject));
				try {
					TEXTMETRIC tEXTMETRIC = new TEXTMETRIC();
					GetTextMetrics(hDC, ref tEXTMETRIC);
					empty.Height = (float)tEXTMETRIC.tmHeight;
					if((tEXTMETRIC.tmPitchAndFamily & 1) != 0) {
						DevExpress.Utils.Drawing.Helpers.NativeMethods.SIZE size = new NativeMethods.SIZE();
						UnsafeNativeMethods.GetTextExtentPoint32(hDC, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", ref size);
						empty.Width = (float)((int)Math.Round((double)((float)size.Width / (float)"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".Length)));
					}
					else {
						empty.Width = (float)tEXTMETRIC.tmAveCharWidth;
					}
				}
				finally {
					NativeMethods.SelectObject(hDC, hObject2);
				}
			}
			finally {
				if(sourceDC == IntPtr.Zero) NativeMethods.DeleteDC(hDC);
				NativeMethods.DeleteObject(hfont);
			}
			return empty;
		}
		internal static int GetTextMetrics(HandleRef hDC, ref TEXTMETRIC lptm) {
			if(Marshal.SystemDefaultCharSize == 1) {
				NativeMethods.TEXTMETRICA tEXTMETRICA = default(NativeMethods.TEXTMETRICA);
				int textMetricsA = UnsafeNativeMethods.GetTextMetricsA(hDC, ref tEXTMETRICA);
				lptm.tmHeight = tEXTMETRICA.tmHeight;
				lptm.tmAscent = tEXTMETRICA.tmAscent;
				lptm.tmDescent = tEXTMETRICA.tmDescent;
				lptm.tmInternalLeading = tEXTMETRICA.tmInternalLeading;
				lptm.tmExternalLeading = tEXTMETRICA.tmExternalLeading;
				lptm.tmAveCharWidth = tEXTMETRICA.tmAveCharWidth;
				lptm.tmMaxCharWidth = tEXTMETRICA.tmMaxCharWidth;
				lptm.tmWeight = tEXTMETRICA.tmWeight;
				lptm.tmOverhang = tEXTMETRICA.tmOverhang;
				lptm.tmDigitizedAspectX = tEXTMETRICA.tmDigitizedAspectX;
				lptm.tmDigitizedAspectY = tEXTMETRICA.tmDigitizedAspectY;
				lptm.tmFirstChar = (char)tEXTMETRICA.tmFirstChar;
				lptm.tmLastChar = (char)tEXTMETRICA.tmLastChar;
				lptm.tmDefaultChar = (char)tEXTMETRICA.tmDefaultChar;
				lptm.tmBreakChar = (char)tEXTMETRICA.tmBreakChar;
				lptm.tmItalic = tEXTMETRICA.tmItalic;
				lptm.tmUnderlined = tEXTMETRICA.tmUnderlined;
				lptm.tmStruckOut = tEXTMETRICA.tmStruckOut;
				lptm.tmPitchAndFamily = tEXTMETRICA.tmPitchAndFamily;
				lptm.tmCharSet = tEXTMETRICA.tmCharSet;
				return textMetricsA;
			}
			return UnsafeNativeMethods.GetTextMetricsW(hDC, ref lptm);
		}
		[StructLayout(LayoutKind.Sequential)]
		internal struct TEXTMETRICA {
			public int tmHeight;
			public int tmAscent;
			public int tmDescent;
			public int tmInternalLeading;
			public int tmExternalLeading;
			public int tmAveCharWidth;
			public int tmMaxCharWidth;
			public int tmWeight;
			public int tmOverhang;
			public int tmDigitizedAspectX;
			public int tmDigitizedAspectY;
			public byte tmFirstChar;
			public byte tmLastChar;
			public byte tmDefaultChar;
			public byte tmBreakChar;
			public byte tmItalic;
			public byte tmUnderlined;
			public byte tmStruckOut;
			public byte tmPitchAndFamily;
			public byte tmCharSet;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct TEXTMETRIC {
			public int tmHeight;
			public int tmAscent;
			public int tmDescent;
			public int tmInternalLeading;
			public int tmExternalLeading;
			public int tmAveCharWidth;
			public int tmMaxCharWidth;
			public int tmWeight;
			public int tmOverhang;
			public int tmDigitizedAspectX;
			public int tmDigitizedAspectY;
			public char tmFirstChar;
			public char tmLastChar;
			public char tmDefaultChar;
			public char tmBreakChar;
			public byte tmItalic;
			public byte tmUnderlined;
			public byte tmStruckOut;
			public byte tmPitchAndFamily;
			public byte tmCharSet;
		}
	}
	[ToolboxItem(false)]
	public class SmartDoubleBufferPainter : Control {
		bool initialized = false, isVista = false;
		[ThreadStatic]
		static SmartDoubleBufferPainter _default;
		public SmartDoubleBufferPainter() {
			this.isVista = false; 
		}
		public static SmartDoubleBufferPainter Default {
			get {
				if(_default == null) _default = new SmartDoubleBufferPainter();
				return _default;
			}
		}
		public bool ProcessMessage(Control control, ref Message m) {
			if(!isVista) return false;
			if(m.Msg == MSG.WM_PAINT) return WMPaint(control, ref m);
			if(m.Msg == MSG.WM_ERASEBKGND) return WMEraseBkgn(control, ref m);
			return false;
		}
		bool WMEraseBkgn(Control control, ref Message m) {
			DrawToDc(control, m.WParam, Rectangle.Empty);
			m.Result = IntPtr.Zero;
			return true;
		}
		bool WMPaint(Control control, ref Message m) {
			Init();
			NativeMethods.PAINTSTRUCT ps = new NativeMethods.PAINTSTRUCT();
			IntPtr dc = NativeMethods.BeginPaint(control.Handle, ref ps);
			DrawToDc(control, dc, ps.rcPaint.ToRectangle());
			NativeMethods.EndPaint(Handle, ref ps);
			m.Result = IntPtr.Zero;
			return true;
		}
		void DrawToDc(Control control, IntPtr dc, Rectangle rect) {
			NativeMethods.RECT target = new NativeMethods.RECT(rect);
			IntPtr resDC = IntPtr.Zero;
			IntPtr hpaint = NativeVista.BeginBufferedPaint(dc, ref target, NativeVista.BP_BUFFERFORMAT.BPBF_DIB, IntPtr.Zero, ref resDC);
			if(hpaint == IntPtr.Zero) {
				isVista = false;
				resDC = dc;
			}
			Graphics g = Graphics.FromHdc(resDC);
			try {
				InvokePaint(control, new PaintEventArgs(g, rect));
				g.Dispose();
			}
			catch { }
			if(hpaint != IntPtr.Zero) NativeVista.EndBufferedPaint(hpaint, true);
		}
		void Init() {
			if(initialized) return;
			this.initialized = true;
			NativeVista.BufferedPaintInit();
		}
	}
	public class WindowScroller {
		public static bool ScrollVertical(Control control, Rectangle maxBounds, int y, int distance) {
			if(distance == 0 || control == null || !control.IsHandleCreated) return false;
			Rectangle rows = maxBounds;
			y = Math.Max(rows.Y, y);
			if(distance < 0) {
				if(y + distance < rows.Y) distance = (rows.Y - y);
			}
			else {
				if(distance >= rows.Height) distance = rows.Height;
			}
			NativeMethods.RECT scrollRect = new NativeMethods.RECT();
			scrollRect.Left = rows.X;
			scrollRect.Right = rows.Right;
			scrollRect.Top = y;
			scrollRect.Bottom = rows.Bottom - (distance > 0 ? distance : 0); 
			NativeMethods.RECT clip = new NativeMethods.RECT();
			clip.Left = maxBounds.Left;
			clip.Right = maxBounds.Right;
			clip.Top = maxBounds.Top;
			clip.Bottom = maxBounds.Bottom;
			NativeMethods.ScrollWindowEx(control.Handle, 0, distance, ref scrollRect, ref clip, IntPtr.Zero, IntPtr.Zero,
				(int)(NativeMethods.ScrollWindowExFlags.SW_INVALIDATE | NativeMethods.ScrollWindowExFlags.SW_ERASE));
			Rectangle inv = rows;
			inv.Y = distance < 0 ? rows.Bottom + distance : y;
			inv.Height = Math.Abs(distance);
			control.Invalidate(inv);
			return true;
		}
		public static bool ScrollHorizontal(Control control, Rectangle maxBounds, int x, int distance) {
			if(distance == 0 || control == null || !control.IsHandleCreated) return false;
			Rectangle rows = maxBounds;
			x = Math.Max(rows.X, x);
			if(distance < 0) {
				if(x + distance < rows.X) distance = (rows.X - x);
			}
			else {
				if(distance >= rows.Width) distance = rows.Width;
			}
			NativeMethods.RECT scrollRect = new NativeMethods.RECT();
			scrollRect.Left = x;
			scrollRect.Right = rows.Right - (distance > 0 ? distance : 0); 
			scrollRect.Top = rows.Y;
			scrollRect.Bottom = rows.Bottom;
			NativeMethods.RECT clip = new NativeMethods.RECT();
			clip.Left = maxBounds.Left;
			clip.Right = maxBounds.Right;
			clip.Top = maxBounds.Top;
			clip.Bottom = maxBounds.Bottom;
			NativeMethods.ScrollWindowEx(control.Handle, distance, 0, ref scrollRect, ref clip, IntPtr.Zero, IntPtr.Zero,
				(int)(NativeMethods.ScrollWindowExFlags.SW_INVALIDATE | NativeMethods.ScrollWindowExFlags.SW_ERASE));
			Rectangle inv = rows;
			inv.X = distance < 0 ? rows.Right + distance : x;
			inv.Width = Math.Abs(distance);
			control.Invalidate(inv);
			return true;
		}
		public static void Repaint(Control control) {
			if(control == null || !control.IsHandleCreated) return;
		}
	}
	public static class LayoutTransactionHelper {
		static Type layoutTransactionType = null;
		static Type iArrangedElementType = null;
		static void InitLayoutTransactionType() {
			AssemblyName[] refAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
			AssemblyName systemWindowsForms = null;
			foreach(AssemblyName a in refAssemblies) {
				if(a.FullName.Contains("System.Windows.Forms")) {
					systemWindowsForms = a;
					break;
				}
			}
			Assembly asm = Assembly.Load(systemWindowsForms);
			layoutTransactionType = asm.GetType("System.Windows.Forms.Layout.LayoutTransaction");
			iArrangedElementType = asm.GetType("System.Windows.Forms.Layout.IArrangedElement");
		}
		static MethodInfo createTransactionIfMethod = null;
		public static IDisposable CreateLayoutTransactionIf(Control ctrl, bool condition, string propName) {
			if(layoutTransactionType == null)
				InitLayoutTransactionType();
			if(createTransactionIfMethod == null)
				createTransactionIfMethod = layoutTransactionType.GetMethod("CreateTransactionIf", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(bool), typeof(Control), iArrangedElementType, typeof(string) }, null);
			return (IDisposable)createTransactionIfMethod.Invoke(null, new object[] { condition, ctrl.Parent, ctrl, propName });
		}
	}
	public interface IWin32Subclasser : IDisposable {
		IntPtr Id { get; }
		IntPtr Handle { get; }
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class Win32SubclasserException : Exception {
		internal Win32SubclasserException(string message)
			: base(message) {
		}
		public static bool Allow = true;
		internal static void Throw(string message) {
			if(Allow) throw new Win32SubclasserException(message);
		}
	}
	public static class Win32SubclasserFactory {
		public delegate bool WndProc(ref System.Windows.Forms.Message m);
		public static IWin32Subclasser Create(IntPtr hWnd, WndProc wndProc) {
			return new Win32Subclasser(IntPtr.Zero, hWnd, GetWin32SubClassProc(wndProc));
		}
		public static void DefaultSubclassProc(ref System.Windows.Forms.Message m) {
			m.Result = NativeMethods.DefSubclassProc(m.HWnd, new IntPtr(m.Msg), m.WParam, m.LParam);
		}
		static int GetInt(IntPtr ptr) {
			return IntPtr.Size == 8 ? unchecked((int)ptr.ToInt64()) : ptr.ToInt32();
		}
		static NativeMethods.Win32SubClassProc GetWin32SubClassProc(WndProc wndProc) {
			return (hWnd, msg, wParam, lParam, uIdSubclass, dwRefData) =>
			{
				var m = System.Windows.Forms.Message.Create(hWnd, GetInt(msg), wParam, lParam);
				Win32SubclasserData.SetHandled(wndProc != null && wndProc(ref m), dwRefData);
				return m.Result;
			};
		}
		#region IWin32Subclasser
		[StructLayout(LayoutKind.Sequential)]
		[System.Security.SecuritySafeCritical]
		struct Win32SubclasserData {
			internal static IntPtr Alloc() {
				IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
				Marshal.WriteInt32(ptr, 0);
				return ptr;
			}
			internal static void Free(IntPtr ptr) {
				int data = Marshal.ReadInt32(ptr);
				if((data & REFCOUNT) > 0) {
					data |= FREE;
					Marshal.WriteInt32(ptr, data);
				}
				else Marshal.FreeHGlobal(ptr);
			}
			const int REFCOUNT = 0xFFFF;
			const int HANDLED = 0x800000;
			const int FREE = 0x400000;
			internal static bool IsHandled(IntPtr ptr) {
				return (Marshal.ReadInt32(ptr) & HANDLED) == HANDLED;
			}
			internal static void SetHandled(bool handled, IntPtr ptr) {
				int data = Marshal.ReadInt32(ptr);
				Marshal.WriteInt32(ptr, SetHandled(data, handled));
			}
			internal static void AddRef(IntPtr ptr) {
				int data = Marshal.ReadInt32(ptr);
				int refCount = data & REFCOUNT;
				Marshal.WriteInt32(ptr, SetRefCount(data, ++refCount));
			}
			internal static void Release(IntPtr ptr) {
				int data = Marshal.ReadInt32(ptr);
				int refCount = data & REFCOUNT;
				Marshal.WriteInt32(ptr, SetRefCount(data, --refCount));
				if(refCount == 0 && ((data & FREE) == FREE))
					Marshal.FreeHGlobal(ptr);
			}
			static int SetRefCount(int data, int refCount) {
				return (data & (~REFCOUNT)) | refCount;
			}
			static int SetHandled(int data, bool handled) {
				if(handled)
					return data | HANDLED;
				else
					return data & (~HANDLED);
			}
		}
		class Win32Subclasser : IWin32Subclasser {
			readonly IntPtr idCore;
			readonly IntPtr handleCore;
			readonly NativeMethods.Win32SubClassProc wndProcInner;
			readonly NativeMethods.Win32SubClassProc wndProcOuter;
			readonly IntPtr dataPtr;
			public Win32Subclasser(IntPtr subClsID, IntPtr hWnd, NativeMethods.Win32SubClassProc subClassProc) {
				this.idCore = subClsID;
				this.handleCore = hWnd;
				this.wndProcInner = SubClassProcInner;
				this.wndProcOuter = subClassProc;
				this.dataPtr = Win32SubclasserData.Alloc();
				AssignHandle();
			}
			IntPtr IWin32Subclasser.Id {
				get { return idCore; }
			}
			IntPtr IWin32Subclasser.Handle {
				get { return handleCore; }
			}
			bool isDisposing;
			void IDisposable.Dispose() {
				DisposeCore();
			}
			void DisposeCore() {
				if(!isDisposing) {
					isDisposing = true;
					OnDisposing();
				}
				GC.SuppressFinalize(this);
			}
			void OnDisposing() {
				ReleaseHandle();
				Win32SubclasserData.Free(dataPtr);
			}
			void AssignHandle() {
				if(NativeMethods.SetWindowSubclass(handleCore, wndProcInner, idCore, dataPtr) != 1)
					Win32SubclasserException.Throw("SetWindowSubClass Failed");
			}
			void ReleaseHandle() {
				if(NativeMethods.RemoveWindowSubclass(handleCore, wndProcInner, idCore) != 1)
					Win32SubclasserException.Throw("RemoveWindowSubclass Failed");
			}
			const int WM_NCDESTROY = 0x0082;
			IntPtr SubClassProcInner(IntPtr hWnd, IntPtr Msg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, IntPtr dwRefData) {
				Win32SubclasserData.AddRef(dwRefData);
				try {
					if(Msg == new IntPtr(WM_NCDESTROY))
						DisposeCore();
					else {
						IntPtr result;
						if(OnWndProc(hWnd, Msg, wParam, lParam, uIdSubclass, dwRefData, out result))
							return result;
					}
					return NativeMethods.DefSubclassProc(hWnd, Msg, wParam, lParam);
				}
				finally { Win32SubclasserData.Release(dwRefData); }
			}
			protected virtual bool OnWndProc(IntPtr hWnd, IntPtr Msg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, IntPtr dwRefData, out IntPtr result) {
				result = IntPtr.Zero;
				if(wndProcOuter == null)
					return false;
				result = wndProcOuter(hWnd, Msg, wParam, lParam, uIdSubclass, dwRefData);
				return Win32SubclasserData.IsHandled(dwRefData);
			}
		}
		#endregion IWin32Subclasser
	}
}
namespace DevExpress.Utils.NonclientArea {
	public class NCMessages {
		public const int WM_NCCALCSIZE = 0x0083;
		public const int WM_NCHITTEST = 0x0084;
		public const int WM_NCPAINT = 0x0085;
		public const int WM_NCMOUSEMOVE = 0x00A0;
		public const int WM_NCLBUTTONDOWN = 0x00A1;
		public const int WM_NCLBUTTONUP = 0x00A2;
		public const int WM_NCLBUTTONDBLCLK = 0x00A3;
		public const int WM_NCMOUSELEAVE = 0x02A2;
	}
	public class CMessages {
		public const int WM_MOUSEMOVE = 0x0200;
		public const int WM_LBUTTONDOWN = 0x0201;
		public const int WM_LBUTTONUP = 0x0202;
		public const int WM_LBUTTONDBLCLK = 0x0203;
		public const int WM_RBUTTONDOWN = 0x0204;
		public const int WM_RBUTTONUP = 0x0205;
		public const int WM_RBUTTONDBLCLK = 0x0206;
		public const int WM_MBUTTONDOWN = 0x0207;
		public const int WM_MBUTTONUP = 0x0208;
		public const int WM_MBUTTONDBLCLK = 0x0209;
		public const int WM_MOUSEHOVER = 0x02A1;
		public const int WM_MOUSELEAVE = 0x02A3;
		public static bool IsMouseMessage(int msg){
			switch(msg){
				case(CMessages.WM_MOUSEMOVE): 
				case(CMessages.WM_LBUTTONDOWN): 
				case(CMessages.WM_LBUTTONUP): 
				case(CMessages.WM_LBUTTONDBLCLK): 
				case(CMessages.WM_RBUTTONDOWN): 
				case(CMessages.WM_RBUTTONUP): 
				case(CMessages.WM_RBUTTONDBLCLK): 
				case(CMessages.WM_MBUTTONDOWN): 
				case(CMessages.WM_MBUTTONUP): 
				case(CMessages.WM_MBUTTONDBLCLK): 
				case (CMessages.WM_MOUSEHOVER): 
				case (CMessages.WM_MOUSELEAVE): 
					return true;
			}
			return false;
		} 
	}
}
