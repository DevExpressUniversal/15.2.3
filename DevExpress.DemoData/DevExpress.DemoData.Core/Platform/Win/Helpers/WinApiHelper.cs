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
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Interop;
using System.Windows;
using Microsoft.Win32;
using System.Text;
using System.ComponentModel;
namespace DevExpress.DemoData.Helpers {
	public static class WinApiHelper {
		static Dictionary<IntPtr, WinProcHook> winProcs = new Dictionary<IntPtr, WinProcHook>();
		public class WinProcHook {
			[SecuritySafeCritical]
			public static WinProcHook Get(IntPtr hwnd) {
				WinProcHook hook;
				if(!winProcs.TryGetValue(hwnd, out hook)) {
					hook = new WinProcHook(hwnd);
					winProcs.Add(hwnd, hook);
				}
				return hook;
			}
			public bool ProcessWmCopyData { get; set; }
			public bool ProcessWmWindowPosChanged { get; set; }
			public event Action<byte[]> OnDataReceived;
			public event Action<WindowPosition> OnWindowPosChanged;
			private WinProcHook(IntPtr hwnd) {
				HwndSource hwndSource = HwndSource.FromHwnd(hwnd);
				hwndSource.AddHook(HookProc);
			}
			[SecuritySafeCritical]
			IntPtr HookProc(IntPtr handle, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
				if(ProcessWmWindowPosChanged && msg == (int)(uint)Import.WM.WM_WINDOWPOSCHANGED) {
					Import.WINDOWPOS windowPos = (Import.WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(Import.WINDOWPOS));
					WindowPosition position = new WindowPosition();
					position.Hwnd = windowPos.hwnd;
					position.HwndInsertAfter = windowPos.hwndInsertAfter;
					position.Left = windowPos.x;
					position.Top = windowPos.y;
					position.Width = windowPos.cx;
					position.Height = windowPos.cy;
					position.Flags = (SetWindowPosFlags)windowPos.flags;
					if(OnWindowPosChanged != null)
						OnWindowPosChanged(position);
				}
				if(ProcessWmCopyData && msg == (int)(uint)Import.WM.WM_COPYDATA) {
					Import.COPYDATASTRUCT copyData = (Import.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(Import.COPYDATASTRUCT));
					byte[] b = new byte[1];
					int length = copyData.cbData / Marshal.SizeOf(b[0]);
					b = new byte[length];
					Marshal.Copy(copyData.lpData, b, 0, b.Length);
					if(OnDataReceived != null)
						OnDataReceived(b);
					handled = true;
				}
				return IntPtr.Zero;
			}
		}
		public class WindowPosition {
			public IntPtr Hwnd { get; set; }
			public IntPtr HwndInsertAfter { get; set; }
			public int Left { get; set; }
			public int Top { get; set; }
			public int Width { get; set; }
			public int Height { get; set; }
			public SetWindowPosFlags Flags { get; set; }
		}
		[Flags]
		public enum SetWindowPosFlags {
			NoSize = (int)Import.SetWindowPosFlags.NOSIZE, NoMove = (int)Import.SetWindowPosFlags.NOMOVE, NoZOrder = (int)Import.SetWindowPosFlags.NOZORDER,
			NoRedraw = (int)Import.SetWindowPosFlags.NOREDRAW, NoActivate = (int)Import.SetWindowPosFlags.NOACTIVATE, DrawFrame = (int)Import.SetWindowPosFlags.DRAWFRAME,
			FrameChanged = (int)Import.SetWindowPosFlags.FRAMECHANGED, ShowWindow = (int)Import.SetWindowPosFlags.SHOWWINDOW, HideWindow = (int)Import.SetWindowPosFlags.HIDEWINDOW,
			NoCopyBits = (int)Import.SetWindowPosFlags.NOCOPYBITS, NoOwnerZOrder = (int)Import.SetWindowPosFlags.NOOWNERZORDER, NoReposition = (int)Import.SetWindowPosFlags.NOREPOSITION,
			NoSendChanging = (int)Import.SetWindowPosFlags.NOSENDCHANGING, DeferErase = (int)Import.SetWindowPosFlags.DEFERERASE, AsyncWindowPos = (int)Import.SetWindowPosFlags.ASYNCWINDOWPOS
		}
		public static readonly IntPtr HwndTopmost = Import.HWND_TOPMOST;
		public static readonly IntPtr HwndNoTopmost = Import.HWND_NOTOPMOST;
		public static readonly IntPtr HwndTop = Import.HWND_TOP;
		public static readonly IntPtr HwndBottom = Import.HWND_BOTTOM;
		[SecuritySafeCritical]
		public static IntPtr LoadLibrary(string filePath) {
			return Import.LoadLibrary(filePath);
		}
		[SecuritySafeCritical]
		public static void FreeLibrary(IntPtr hmodule) {
			Import.FreeLibrary(hmodule);
		}
		[SecuritySafeCritical]
		public static IntPtr GetProcAddress(IntPtr hmodule, string procName) {
			return Import.GetProcAddress(hmodule, procName);
		}
		[SecuritySafeCritical]
		public static void MakeWindowChild(IntPtr hwnd) {
			Import.WS style = (Import.WS)(uint)Import.GetWindowLong(hwnd, (int)Import.GWL.GWL_STYLE);
			style |= Import.WS.WS_CHILD;
			style &= ~Import.WS.WS_POPUP;
			Import.SetWindowLong(hwnd, (int)Import.GWL.GWL_STYLE, (int)(uint)style);
		}
		[SecuritySafeCritical]
		public static void SetWindowParent(IntPtr hwnd, IntPtr newParentHwnd) {
			Import.SetParent(hwnd, newParentHwnd);
		}
		[SecuritySafeCritical]
		public static void MoveWindow(IntPtr hwnd, int x, int y, int width, int height, bool repaint) {
			Import.MoveWindow(hwnd, x, y, width, height, repaint);
		}
		[SecuritySafeCritical]
		public static IntPtr GetPrevWindow(IntPtr hwnd) {
			return Import.GetWindow(hwnd, Import.GetWindowCmd.GW_HWNDPREV);
		}
		[SecuritySafeCritical]
		public static IntPtr GetNextWindow(IntPtr hwnd) {
			return Import.GetWindow(hwnd, Import.GetWindowCmd.GW_HWNDNEXT);
		}
		[SecuritySafeCritical]
		public static void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, SetWindowPosFlags flags) {
			Import.SetWindowPosFlags nativeFlags = (Import.SetWindowPosFlags)(uint)(int)flags;
			Import.SetWindowPos(hwnd, hwndInsertAfter, x, y, width, height, nativeFlags);
		}
		[SecuritySafeCritical]
		public static IntPtr SetFocus(IntPtr hwnd) {
			return Import.SetFocus(hwnd);
		}
		[SecuritySafeCritical]
		public static bool SetForegroundWindow(IntPtr hwnd) {
			return Import.SetForegroundWindow(hwnd);
		}
		[SecuritySafeCritical]
		public static bool RestoreWindowAsync(IntPtr hwnd) {
			return Import.ShowWindowAsync(hwnd, (int)Import.ShowWindowCommands.Restore);
		}
		[SecuritySafeCritical]
		public static void SendData(IntPtr hwnd, byte[] data) {
			int size;
			IntPtr dataPtr = IntPtrAlloc(data, out size);
			Import.COPYDATASTRUCT copyData = new Import.COPYDATASTRUCT();
			copyData.dwData = (IntPtr)0xC0b1;
			copyData.lpData = dataPtr;
			copyData.cbData = size;
			IntPtr copyDataBuff = IntPtrAlloc(copyData);
			Import.SendMessage(hwnd, (int)(uint)Import.WM.WM_COPYDATA, IntPtr.Zero, copyDataBuff);
			IntPtrFree(copyDataBuff); copyDataBuff = IntPtr.Zero;
			IntPtrFree(dataPtr); dataPtr = IntPtr.Zero;
		}
		[SecuritySafeCritical]
		public static IntPtr IntPtrAlloc<T>(T param) {
			IntPtr retval = Marshal.AllocHGlobal(Marshal.SizeOf(param));
			Marshal.StructureToPtr(param, retval, false);
			return (retval);
		}
		[SecuritySafeCritical]
		public static IntPtr IntPtrAlloc(byte[] data, out int size) {
			size = Marshal.SizeOf(data[0]) * data.Length;
			IntPtr retval = Marshal.AllocHGlobal(size);
			Marshal.Copy(data, 0, retval, data.Length);
			return retval;
		}
		[SecuritySafeCritical]
		public static void IntPtrFree(IntPtr preAllocated) {
			if(IntPtr.Zero == preAllocated) return;
			Marshal.FreeHGlobal(preAllocated); preAllocated = IntPtr.Zero;
		}
		[SecuritySafeCritical]
		public static IntPtr SetWndProcHook(IntPtr hwnd, IntPtr module, IntPtr hookPtr) {
			return Import.SetWindowsHookEx(Import.HookType.WH_CALLWNDPROC, hookPtr, module, (uint)GetWindowThreadProcessId(hwnd));
		}
		[SecuritySafeCritical]
		public static bool UnhookWindowsHookEx(IntPtr hhook) {
			return Import.UnhookWindowsHookEx(hhook);
		}
		[SecuritySafeCritical]
		public static int GetWindowThreadProcessId(IntPtr hwnd) {
			return (int)Import.GetWindowThreadProcessId(hwnd, IntPtr.Zero);
		}
		[SecuritySafeCritical]
		public static System.Drawing.Rectangle? GetWindowNormalPlacement(IntPtr hwnd) {
			Import.WINDOWPLACEMENT placement = new Import.WINDOWPLACEMENT();
			placement.length = Marshal.SizeOf(placement);
			if(!Import.GetWindowPlacement(hwnd, ref placement)) return null;
			return placement.rcNormalPosition;
		}
		[SecuritySafeCritical]
		public static bool CopyWindowPlacement(IntPtr source, IntPtr dest) {
			Import.WINDOWPLACEMENT placement = new Import.WINDOWPLACEMENT();
			placement.length = Marshal.SizeOf(placement);
			if(!Import.GetWindowPlacement(source, ref placement)) return false;
			if(!Import.SetWindowPlacement(dest, ref placement)) return false;
			return true;
		}
		[SecuritySafeCritical]
		public static bool SetWindowState(IntPtr hwnd, WindowState state) {
			Import.ShowWindowCommands command;
			switch(state) {
				case WindowState.Maximized: command = Import.ShowWindowCommands.ShowMaximized; break;
				case WindowState.Minimized: command = Import.ShowWindowCommands.ShowMinimized; break;
				default: command = Import.ShowWindowCommands.Normal; break;
			}
			return Import.ShowWindow(hwnd, command);
		}
		[SecuritySafeCritical]
		public static int SetWindowNoMaximize(IntPtr hwnd) {
			Import.WS style = (Import.WS)(uint)Import.GetWindowLong(hwnd, (int)Import.GWL.GWL_STYLE);
			style &= ~Import.WS.WS_MAXIMIZEBOX;
			return Import.SetWindowLong(hwnd, (int)Import.GWL.GWL_STYLE, (int)(uint)style);
		}
		[SecuritySafeCritical]
		public static void MakeWindowTransparent(IntPtr hwnd) {
			Import.WS style = (Import.WS)(uint)Import.GetWindowLong(hwnd, (int)Import.GWL.GWL_EXSTYLE);
			style |= Import.WS.WS_EX_LAYERED | Import.WS.WS_EX_TRANSPARENT;
			Import.SetWindowLong(hwnd, (int)Import.GWL.GWL_EXSTYLE, (int)(uint)style);
		}
		public enum SpecialFolderType {
			CommonDocuments = Import.SpecialFolderType.CommonDocuments
		}
		[SecuritySafeCritical]
		public static string GetFolderPath(SpecialFolderType folderType) {
			StringBuilder path = new StringBuilder(Import.MAX_PATH);
			int result = Import.SHGetFolderPath(IntPtr.Zero, (int)folderType, IntPtr.Zero, 0, path);
			if(result != 0)
				throw new Win32Exception(result);
			return path.ToString();
		}
		static class Import {
			public const int MAX_PATH = 260;
			[DllImport("shell32.dll")]
			public static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, uint dwFlags, [Out] StringBuilder pszPath);
			public enum SpecialFolderType {
				AdministrativeTools = 0x0030,
				CommonAdministrativeTools = 0x002f,
				ApplicationData = 0x001a,
				CommonAppData = 0x0023,
				CommonDocuments = 0x002e,
				Cookies = 0x0021,
				CreateFlag = 0x8000,
				History = 0x0022,
				InternetCache = 0x0020,
				LocalApplicationData = 0x001c,
				MyPictures = 0x0027,
				Personal = 0x0005,
				ProgramFiles = 0x0026,
				CommonProgramFiles = 0x002b,
				System = 0x0025,
				Windows = 0x0024,
				Fonts = 0x0014
			}
			[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW", SetLastError = true)]
			public static extern uint RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved, ref RType lpType, byte[] pvData, ref uint pcbData);
			public enum RFlags {
				Any = 65535,
				RegNone = 1,
				Noexpand = 268435456,
				RegBinary = 8,
				Dword = 24,
				RegDword = 16,
				Qword = 72,
				RegQword = 64,
				RegSz = 2,
				RegMultiSz = 32,
				RegExpandSz = 4,
				RrfZeroonfailure = 536870912
			}
			public enum RType {
				RegNone = 0,
				RegSz = 1,
				RegExpandSz = 2,
				RegMultiSz = 7,
				RegBinary = 3,
				RegDword = 4,
				RegQword = 11,
				RegQwordLittleEndian = 11,
				RegDwordLittleEndian = 4,
				RegDwordBigEndian = 5,
				RegLink = 6,
				RegResourceList = 8,
				RegFullResourceDescriptor = 9,
				RegResourceRequirementsList = 10
			}
			[DllImport("advapi32.dll", SetLastError = true)]
			public static extern int RegCloseKey(IntPtr hKey);
			[DllImport("advapi32.dll", CharSet = CharSet.Auto)]
			public static extern int RegOpenKeyEx(UIntPtr hKey, string subKey, int ulOptions, int samDesired, out IntPtr hkResult);
			public static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);
			public static UIntPtr HKEY_CURRENT_USER = new UIntPtr(0x80000001u);
			public const int KEY_QUERY_VALUE = 0x1;
			public const int KEY_SET_VALUE = 0x2;
			public const int KEY_CREATE_SUB_KEY = 0x4;
			public const int KEY_ENUMERATE_SUB_KEYS = 0x8;
			public const int KEY_NOTIFY = 0x10;
			public const int KEY_CREATE_LINK = 0x20;
			public const int KEY_WOW64_32KEY = 0x200;
			public const int KEY_WOW64_64KEY = 0x100;
			public const int KEY_WOW64_RES = 0x300;
			public const int KEY_READ = 0x20019;
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);
			[DllImport("user32.dll", SetLastError = true)]
			public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr onlyZero);
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool UnhookWindowsHookEx(IntPtr hhk);
			[DllImport("user32.dll", SetLastError = true)]
			public static extern IntPtr SetWindowsHookEx(HookType hookType, IntPtr lpfn, IntPtr hMod, uint dwThreadId);
			public enum HookType : int {
				WH_JOURNALRECORD = 0,
				WH_JOURNALPLAYBACK = 1,
				WH_KEYBOARD = 2,
				WH_GETMESSAGE = 3,
				WH_CALLWNDPROC = 4,
				WH_CBT = 5,
				WH_SYSMSGFILTER = 6,
				WH_MOUSE = 7,
				WH_HARDWARE = 8,
				WH_DEBUG = 9,
				WH_SHELL = 10,
				WH_FOREGROUNDIDLE = 11,
				WH_CALLWNDPROCRET = 12,
				WH_KEYBOARD_LL = 13,
				WH_MOUSE_LL = 14
			}
			[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
			public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
			[DllImport("kernel32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool FreeLibrary(IntPtr hModule);
			[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
			public static extern IntPtr LoadLibrary(string lpFileName);
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool SetForegroundWindow(IntPtr hWnd);
			[StructLayout(LayoutKind.Sequential)]
			public struct WINDOWPLACEMENT {
				public int length;
				public int flags;
				public ShowWindowCommands showCmd;
				public System.Drawing.Point ptMinPosition;
				public System.Drawing.Point ptMaxPosition;
				public System.Drawing.Rectangle rcNormalPosition;
			}
			public enum ShowWindowCommands : int {
				Hide = 0,
				Normal = 1,
				ShowMinimized = 2,
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
			[DllImport("user32.dll")]
			public static extern IntPtr SetFocus(IntPtr hWnd);
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);
			[DllImport("user32.dll", SetLastError = true)]
			public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
			[DllImport("user32.dll", SetLastError = true)]
			public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
			[DllImport("user32.dll", SetLastError = true)]
			public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
			[DllImport("user32.dll", SetLastError = true)]
			public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
			[DllImport("user32.dll", SetLastError = true)]
			public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);
			[DllImport("user32.dll")]
			public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
			[DllImport("user32.dll")]
			public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
			public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
			public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
			public static readonly IntPtr HWND_TOP = new IntPtr(0);
			public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
			public enum GetWindowCmd : uint {
				GW_HWNDFIRST = 0,
				GW_HWNDLAST = 1,
				GW_HWNDNEXT = 2,
				GW_HWNDPREV = 3,
				GW_OWNER = 4,
				GW_CHILD = 5,
				GW_ENABLEDPOPUP = 6
			}
			public enum SetWindowPosFlags : uint {
				NOSIZE = 0x0001,
				NOMOVE = 0x0002,
				NOZORDER = 0x0004,
				NOREDRAW = 0x0008,
				NOACTIVATE = 0x0010,
				DRAWFRAME = 0x0020,
				FRAMECHANGED = 0x0020,
				SHOWWINDOW = 0x0040,
				HIDEWINDOW = 0x0080,
				NOCOPYBITS = 0x0100,
				NOOWNERZORDER = 0x0200,
				NOREPOSITION = 0x0200,
				NOSENDCHANGING = 0x0400,
				DEFERERASE = 0x2000,
				ASYNCWINDOWPOS = 0x4000
			}
			[StructLayout(LayoutKind.Sequential)]
			public struct WINDOWPOS {
				public IntPtr hwnd;
				public IntPtr hwndInsertAfter;
				public int x;
				public int y;
				public int cx;
				public int cy;
				public int flags;
			}
			[StructLayout(LayoutKind.Sequential)]
			public struct COPYDATASTRUCT {
				public IntPtr dwData;
				public int cbData;
				public IntPtr lpData;
			}
			public enum WM : uint {
				WM_COPYDATA = 0x004A,
				WM_WINDOWPOSCHANGED = 0x0047,
				WM_WINDOWPOSCHANGING = 0x0046
			}
			public enum GWL : int {
				GWL_WNDPROC = (-4),
				GWL_HINSTANCE = (-6),
				GWL_HWNDPARENT = (-8),
				GWL_STYLE = (-16),
				GWL_EXSTYLE = (-20),
				GWL_USERDATA = (-21),
				GWL_ID = (-12)
			}
			[Flags]
			public enum WS : uint {
				WS_OVERLAPPED = 0x00000000,
				WS_POPUP = 0x80000000,
				WS_CHILD = 0x40000000,
				WS_MINIMIZE = 0x20000000,
				WS_VISIBLE = 0x10000000,
				WS_DISABLED = 0x08000000,
				WS_CLIPSIBLINGS = 0x04000000,
				WS_CLIPCHILDREN = 0x02000000,
				WS_MAXIMIZE = 0x01000000,
				WS_BORDER = 0x00800000,
				WS_DLGFRAME = 0x00400000,
				WS_VSCROLL = 0x00200000,
				WS_HSCROLL = 0x00100000,
				WS_SYSMENU = 0x00080000,
				WS_THICKFRAME = 0x00040000,
				WS_GROUP = 0x00020000,
				WS_TABSTOP = 0x00010000,
				WS_MINIMIZEBOX = 0x00020000,
				WS_MAXIMIZEBOX = 0x00010000,
				WS_CAPTION = WS_BORDER | WS_DLGFRAME,
				WS_TILED = WS_OVERLAPPED,
				WS_ICONIC = WS_MINIMIZE,
				WS_SIZEBOX = WS_THICKFRAME,
				WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
				WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
				WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
				WS_CHILDWINDOW = WS_CHILD,
				WS_EX_DLGMODALFRAME = 0x00000001,
				WS_EX_NOPARENTNOTIFY = 0x00000004,
				WS_EX_TOPMOST = 0x00000008,
				WS_EX_ACCEPTFILES = 0x00000010,
				WS_EX_TRANSPARENT = 0x00000020,
				WS_EX_MDICHILD = 0x00000040,
				WS_EX_TOOLWINDOW = 0x00000080,
				WS_EX_WINDOWEDGE = 0x00000100,
				WS_EX_CLIENTEDGE = 0x00000200,
				WS_EX_CONTEXTHELP = 0x00000400,
				WS_EX_RIGHT = 0x00001000,
				WS_EX_LEFT = 0x00000000,
				WS_EX_RTLREADING = 0x00002000,
				WS_EX_LTRREADING = 0x00000000,
				WS_EX_LEFTSCROLLBAR = 0x00004000,
				WS_EX_RIGHTSCROLLBAR = 0x00000000,
				WS_EX_CONTROLPARENT = 0x00010000,
				WS_EX_STATICEDGE = 0x00020000,
				WS_EX_APPWINDOW = 0x00040000,
				WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
				WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
				WS_EX_LAYERED = 0x00080000,
				WS_EX_NOINHERITLAYOUT = 0x00100000, 
				WS_EX_LAYOUTRTL = 0x00400000, 
				WS_EX_COMPOSITED = 0x02000000,
				WS_EX_NOACTIVATE = 0x08000000
			}
		}
	}
}
