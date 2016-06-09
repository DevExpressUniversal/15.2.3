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
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace DevExpress.XtraTests {
	internal enum InputSealedKey {ALT = Keys.Alt, CTRL = Keys.Control, SHIFT = Keys.Shift }
	internal enum InputSealedKeyValue { ALTVALUE = 18, CTRLVALUE = 17, SHIFTVALUE = 0x10 }
	internal enum InputKeyTemplate {
		BACKSPACE = Keys.Back, 
		BS = Keys.Back, 
		BKSP = Keys.Back, 
		INS = Keys.Insert, 
		DEL = Keys.Delete,
		PGUP = Keys.PageUp,
		PGDN = Keys.PageDown,
		ESC = Keys.Escape,
		ENTER = Keys.Enter,
		APPS = Keys.Apps
	};
	enum GetWindowCmd {
		GW_HWNDFIRST = 0,
		GW_HWNDLAST = 1,
		GW_HWNDNEXT = 2,
		GW_HWNDPREV = 3,
		GW_OWNER = 4,
		GW_CHILD = 5,
		GW_ENABLEDPOPUP = 6
	}
	[CLSCompliant(false)]
	public class TestImports {
		public class Msg {
			public uint hWnd;
			public uint Message;
			public uint wParam;
			public uint lParam;
			public uint time;
			public System.IntPtr pt;
		}
		public const uint PM_NOREMOVE = 0;
		public const uint WM_MOUSEFIRST = 0x0200;
		public const uint WM_MOUSELAST = 0x020A;
		public const int WM_KEYDOWN = 0x0100;
		public const int WM_SYSKEYDOWN = 0x104;
		public const int WM_KEYUP = 0x0101;
		public const int WM_SYSKEYUP = 0x105;
		public const int WM_CHAR = 0x0102;
		[DllImport("user32.dll", EntryPoint = "SendInput")]
		extern public static uint SendMouseInput(int nInputs, [MarshalAs(UnmanagedType.LPArray)] MouseInputArgs[] pInputs, int cbSize);
		[DllImport("user32.dll", EntryPoint = "SendInput")]
		extern public static uint SendKeyInput(int nInputs, [MarshalAs(UnmanagedType.LPArray)] KeyInputArgs[] pInputs, int cbSize);
		[DllImport("kernel32.dll", EntryPoint = "GetLastError")]
		extern public static uint GetLastError();
		[DllImport("user32.dll", EntryPoint = "MapVirtualKey")]
		extern public static uint MapVirtualKey(uint uCode, uint uMapType);
		[DllImport("user32.dll")]
		extern public static void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo);
		[DllImport("user32.dll")]
		public static extern void keybd_event(uint vk, uint scan, uint flags, uint extrainfo);
		[DllImport("user32.dll", EntryPoint = "PeekMessage")]
		extern public static uint PeekMessage(Msg msg, System.IntPtr hWnd,uint firstMessage, uint lastMessage, uint options);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern int PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
		[System.Runtime.InteropServices.DllImport("user32.dll")] 
		public static extern short GetKeyState(int keyCode);
		[System.Runtime.InteropServices.DllImport("user32.dll")] 
		public static extern bool SetKeyboardState([MarshalAs(UnmanagedType.LPArray)] byte[] bytes);
		[System.Runtime.InteropServices.DllImport("user32.dll")] 
		public static extern bool GetKeyboardState(byte[] bytes);
		public struct Win32Point { public int X; public int Y; };
		[DllImport("user32.dll")]
		public static extern bool ClientToScreen(IntPtr hwnd, ref Win32Point lpPoint);
		[DllImport("user32.dll")]
		public static extern bool SetCursorPos(int X, int Y);
		[DllImport("user32.dll")]
		public static extern int GetCursorPos(out Win32Point lpPoint);
		[DllImport("user32.dll")]
		public static extern uint VkKeyScan(char ch);
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern IntPtr SetActiveWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern IntPtr GetFocus();
		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll")]
		public static extern IntPtr GetTopWindow(IntPtr hWnd);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowVisible(IntPtr hWnd);
		public static bool CapsIsPressed {
			get { return GetKeyPressed(Keys.CapsLock); }
			set { SetKeyPressed(Keys.CapsLock, value);	}
		}
		public static bool GetKeyPressed(Keys key) {
			uint keyValue = GetKeyValue(key);
			if(keyValue > 255) return false;
			byte[] bytes = new byte[255];
			if(GetKeyboardState(bytes)){
				return bytes[keyValue] != 0;
			} 
			return false;
		}
		public static void SetKeyPressed(uint keyValue, bool value) {
			if (keyValue > 255) return;
			byte[] bytes = new byte[255];
			if (GetKeyboardState(bytes)) {
				if (value) {
					bytes[keyValue] = 128;
				} else {
					bytes[keyValue] = 0;
				}
				SetKeyboardState(bytes);
			} 
		}
		public static void SetKeyPressed(Keys key, bool value) {
			SetKeyPressed(GetKeyValue(key), value);
		}
		public static uint GetKeyValue(Keys key) {
			if(IsSealedKey(key))
				return (uint)GetSealedKeyValue(key);
			else return (uint)key & 0x0000FFFF;
		}
		public static bool IsSealedKey(Keys key) {
			return GetSealedKeyValue(key) > -1;
		}
		public static int GetSealedKeyValue(Keys key) {
			Array ar = Enum.GetValues(typeof(InputSealedKey));
			int i;
			for(i = 0; i < ar.Length; i ++)
				if((Keys)ar.GetValue(i) == key) break;
			if(i < ar.Length) {
				ar = Enum.GetValues(typeof(InputSealedKeyValue));
				return (int)ar.GetValue(i);
			}
			return -1;
		}
	}
	[CLSCompliant(false)]
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
	public struct MouseInputArgs { 
		public int Type;
		public long dx;
		public long dy;
		public uint mouseData;
		public uint dwFlags;
		public uint time;
		public IntPtr extaInfo;
		public MouseInputArgs(long dx, long dy, uint dwFlags) {
			this.Type = 0;
			this.dx = dx;
			this.dy = dy;
			this.mouseData = 0;
			this.dwFlags = dwFlags;
			this.time = 0;
			this.extaInfo = IntPtr.Zero;
		}
		public MouseInputArgs(long dx, long dy, uint dwFlags, uint mouseData) : this(dx, dy, dwFlags) {
			this.mouseData = mouseData;
		}
	}
	public enum KeyEventType { KeyDown, KeyUp }
	[CLSCompliant(false)]
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
	public struct KeyInputArgs { 
		public int Type;
		public ushort wVK;
		public ushort wScan;
		public UInt32 dwFlags;
		public UInt32 time;
		public IntPtr extraInfo;
		public KeyInputArgs(KeyEventType keyType, ushort vk) {
			this.Type = 1;
			this.wVK = vk;
			this.wScan = Convert.ToUInt16(TestImports.MapVirtualKey(vk, 0));
			this.dwFlags = Convert.ToUInt32((keyType == KeyEventType.KeyDown ? 0 : 0x0002));
			this.time = 0;
			this.extraInfo = IntPtr.Zero;
		}
	}
}
