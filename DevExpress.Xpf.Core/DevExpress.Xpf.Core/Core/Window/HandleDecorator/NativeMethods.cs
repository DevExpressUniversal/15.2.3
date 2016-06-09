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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow.Core.HandleDecorator.Helpers {
#else
namespace DevExpress.Xpf.Core.HandleDecorator.Helpers {
#endif
	public class NativeHelper {
		public const int WM_DISPLAYCHANGE = 126;
		public const int WS_EX_NOACTIVATE = 0x08000000;
		public const int WS_EX_LAYERED = 0x00080000;
		public const int WS_EX_TOOLWINDOW = 0x00000080;
		public const int WS_EX_TRANSPARENT = 0x00000020;
		public const int WS_VISIBLE = 0x10000000;
		public const int WS_MINIMIZE = 0x20000000;
		public const int WS_CHILDWINDOW = 0x40000000;
		public const int WS_CLIPCHILDREN = 0x02000000;
		public const int WS_DISABLED = 0x08000000;
		public const int WM_DESTROY = 0x0002;
		#region SetWindowLong
		public const int GWL_WNDPROC = -4;
		public const int GWL_HWNDPARENT = -8;
		#endregion
		public const int WA_ACTIVE = 1;
		public const int WA_CLICKACTIVE = 2;
		internal static int GetLowWord(int value) {
			return (int)((short)(value & 65535));
		}
		internal static bool IsHandleCreated(System.Windows.Window window) {
			bool result = GetHandle(window) != IntPtr.Zero ? true : false;
			return result;
		}
		internal static IntPtr GetHandle(System.Windows.Window window) {
			var interopHelper = new System.Windows.Interop.WindowInteropHelper(window);
			IntPtr handle = interopHelper.Handle;
			interopHelper = null;
			return handle;
		}
	}
	public class MSG {
		public const int
			WA_INACTIVE = 0,
			WM_ACTIVATE = 0x0006,
			WM_NCHITTEST = 0x0084,
			WM_WINDOWPOSCHANGING = 0x0046,
			WM_WINDOWPOSCHANGED = 0x0047;
	}
	[System.Security.SecuritySafeCritical]
	public class NativeMethods {
		internal static long GetIntFromPtr(IntPtr ptr) {
			if(IntPtr.Size == 4) return ptr.ToInt32();
			return ptr.ToInt64();
		}
		#region Structs&Enums
		public struct WNDCLASS {
			public int style;
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
			public NativeMethods.POINT MinPosition;
			public NativeMethods.POINT MaxPosition;
			public NativeMethods.RECT NormalPosition;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct BLENDFUNCTION {
			public byte BlendOp;
			public byte BlendFlags;
			public byte SourceConstantAlpha;
			public byte AlphaFormat;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct HWND : IWin32Window {
			IntPtr _Handle;
			public static readonly HWND Empty = new HWND(IntPtr.Zero);
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
				if(!GetIntFromPtr(_Handle).Equals(GetIntFromPtr(ptr)))
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
				return "{" + "Handle=0x" + GetIntFromPtr(_Handle).ToString("X8") + "}";
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
				return "{" + "Handle=0x" + GetIntFromPtr(_Handle).ToString("X8") + "}";
			}
			#endregion
			public IntPtr SelectObject(IntPtr aGDIObj) {
				return NativeMethods.SelectObject(this, aGDIObj);
			}
			public HDC CreateCompatible() {
				return NativeMethods.CreateCompatibleDC(_Handle);
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
		public class HT {
			public const int HTERROR = (-2);
			public const int HTTRANSPARENT = (-1);
			public const int HTNOWHERE = 0, HTCLIENT = 1, HTCAPTION = 2, HTSYSMENU = 3,
				HTGROWBOX = 4, HTSIZE = HTGROWBOX, HTMENU = 5, HTHSCROLL = 6, HTVSCROLL = 7, HTMINBUTTON = 8, HTMAXBUTTON = 9,
				HTLEFT = 10, HTRIGHT = 11, HTTOP = 12, HTTOPLEFT = 13, HTTOPRIGHT = 14, HTBOTTOM = 15, HTBOTTOMLEFT = 16,
				HTBOTTOMRIGHT = 17, HTBORDER = 18, HTREDUCE = HTMINBUTTON, HTZOOM = HTMAXBUTTON, HTSIZEFIRST = HTLEFT,
				HTSIZELAST = HTBOTTOMRIGHT, HTOBJECT = 19, HTCLOSE = 20, HTHELP = 21;
		}
		#endregion Structs&Enums
		static class UnsafeNativeMethods {
			[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern IntPtr CreateWindowEx(int dwExStyle, IntPtr classAtom, [MarshalAs(UnmanagedType.LPTStr)] string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool DestroyWindow(IntPtr hwnd);
			[DllImport("user32.dll", CharSet = CharSet.Unicode)]
			internal static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
			[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern int RegisterClass(ref WNDCLASS lpWndClass);
			[DllImport("user32.dll", SetLastError=true, CharSet= CharSet.Unicode)]
			internal static extern int GetClassInfo(IntPtr hInstance, [MarshalAs(UnmanagedType.LPTStr)] string lpClassName, ref WNDCLASS lpWndClass);
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool UnregisterClass(IntPtr classAtom, IntPtr hInstance);
			[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
			internal static extern IntPtr SelectObject(HandleRef hdc, HandleRef obj);
			[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
			internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr obj);
			[DllImport("gdi32.dll")]
			internal static extern IntPtr CreateCompatibleDC(HandleRef hDC);
			[DllImport("gdi32.dll")]
			internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);
			[DllImport("gdi32.dll")]
			internal static extern bool DeleteObject(HandleRef hObject);
			[DllImport("gdi32.dll")]
			internal static extern bool DeleteObject(IntPtr hObject);
			[DllImport("USER32.dll")]
			internal static extern int SetWindowLong(IntPtr hwnd, int flags, long val);
			[DllImport("user32.dll")]
			internal static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
			[DllImport("USER32.dll")]
			internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
				int X, int Y, int cx, int cy, int uFlags);
			[DllImport("USER32.dll")]
			internal static extern bool IsWindowVisible(IntPtr hWnd);
			[DllImport("USER32.dll")]
			internal static extern bool IsZoomed(IntPtr hwnd);
			[DllImport("USER32.dll")]
			internal static extern bool IsIconic(IntPtr hwnd);
			[DllImport("USER32.dll")]
			internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
			[DllImport("USER32.dll")]
			internal static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool redraw);
			[DllImport("User32.dll")]
			internal static extern HDC GetDC(HWND handle);
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
			internal static extern bool UpdateLayeredWindow(IntPtr hwnd,
				IntPtr hdcDst, ref NativeMethods.POINT pptDst, ref NativeMethods.SIZE pSizeDst,
				IntPtr hdcSrc, ref NativeMethods.POINT pptSrc,
				int crKey, ref BLENDFUNCTION pBlend, int dwFlags);
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);
			[DllImport("User32.dll")]
			internal static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);
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
		public static int GetClassInfo(IntPtr hInstance, string lpClassName, ref WNDCLASS lpWndClass) {
			return UnsafeNativeMethods.GetClassInfo(hInstance, lpClassName, ref lpWndClass);
		}
		public static int RegisterClass(ref WNDCLASS lpWndClass) {
			return UnsafeNativeMethods.RegisterClass(ref lpWndClass);
		}
		public static bool UnregisterClass(IntPtr classAtom, IntPtr hInstance) {
			return UnsafeNativeMethods.UnregisterClass(classAtom, hInstance);
		}
		public static IntPtr SelectObject(HandleRef hdc, HandleRef obj) {
			return UnsafeNativeMethods.SelectObject(hdc, obj);
		}
		public static IntPtr SelectObject(IntPtr hdc, IntPtr obj) {
			return UnsafeNativeMethods.SelectObject(hdc, obj);
		}
		public static IntPtr CreateCompatibleDC(HandleRef hDC) {
			return UnsafeNativeMethods.CreateCompatibleDC(hDC);
		}
		public static IntPtr CreateCompatibleDC(IntPtr hDC) {
			return UnsafeNativeMethods.CreateCompatibleDC(hDC);
		}
		public static bool DeleteObject(HandleRef hObject) {
			return UnsafeNativeMethods.DeleteObject(hObject);
		}
		public static bool DeleteObject(IntPtr hObject) {
			return UnsafeNativeMethods.DeleteObject(hObject);
		}
		public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong) {
			return UnsafeNativeMethods.SetWindowLong(hWnd, nIndex, dwNewLong);
		}
		public static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags) {
			return UnsafeNativeMethods.SetWindowPos(hWnd, hWndInsertAfter, x, y, cx, cy, uFlags);
		}
		public static bool IsWindowVisible(IntPtr hWnd) {
			return UnsafeNativeMethods.IsWindowVisible(hWnd);
		}
		public static bool IsZoomed(IntPtr hWnd) {
			return UnsafeNativeMethods.IsZoomed(hWnd);
		}
		public static bool IsIconic(IntPtr hWnd) {
			return UnsafeNativeMethods.IsZoomed(hWnd);
		}
		public static bool GetWindowRect(IntPtr hWnd, ref RECT lpRect) {
			return UnsafeNativeMethods.GetWindowRect(hWnd, ref lpRect);
		}
		public static int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool redraw) {
			return UnsafeNativeMethods.SetWindowRgn(hWnd, hRgn, redraw);
		}
		public static HDC GetDC(HWND handle) {
			return UnsafeNativeMethods.GetDC(handle);
		}
		public static bool UpdateLayeredWindow(IntPtr hwnd,
			IntPtr hdcDst, ref NativeMethods.POINT pptDst, ref NativeMethods.SIZE pSizeDst,
			IntPtr hdcSrc, ref NativeMethods.POINT pptSrc,
			int crKey, ref BLENDFUNCTION pBlend, int dwFlags) {
			return UnsafeNativeMethods.UpdateLayeredWindow(hwnd, hdcDst, ref pptDst, ref pSizeDst, hdcSrc, ref pptSrc, crKey, ref pBlend, dwFlags);
		}
		public static bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl) {
			return UnsafeNativeMethods.GetWindowPlacement(hWnd, out lpwndpl);
		}
		public static IntPtr GetWindow(IntPtr hWnd, int wCmd) {
			return UnsafeNativeMethods.GetWindow(hWnd, (uint)wCmd);
		}
		public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
		public const int GW_HWNDPREV = 3;
		public const int GW_OWNER = 4;
	}
}
