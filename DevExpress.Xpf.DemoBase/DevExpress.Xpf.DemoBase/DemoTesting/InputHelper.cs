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
namespace DevExpress.XtraTests {
	public enum KeyCodeType { ScanCode, VirtualCode };
	public delegate void InputHelperActionsDelegate();
	[CLSCompliant(false)]
	public interface IInputHelper {
		InputHelperActionsDelegate BeforeAction { get; set; }
		InputHelperActionsDelegate AfterAction { get; set; }
		void SendKeyDown(uint uCode, KeyCodeType keyCodeType);
		void SendKeyUp(uint uCode, KeyCodeType keyCodeType);
		void SendMouseLDown(int screenX, int screenY);
		void SendMouseLUp(int screenX, int screenY);
		void SendMouseRDown(int screenX, int screenY);
		void SendMouseRUp(int screenX, int screenY);
		void SendMouseMove(int screenX, int screenY, uint flags);
		void SendMouseWheel(int screenX, int screenY, bool forward);
	}
	[CLSCompliant(false)]
	public class InputHelper : IInputHelper {
		const int
			INPUT_MOUSE = 0,
			INPUT_KEYBOARD = 1,
			INPUT_HARDWARE = 2;
		const uint
			KEYEVENTF_EXTENDEDKEY = 0x0001,
			KEYEVENTF_KEYUP = 0x0002,
			KEYEVENTF_UNICODE = 0x0004,
			KEYEVENTF_SCANCODE = 0x0008;
		const double AbsoluteScreenSize = 65536.0;
		public const uint
			MOUSEEVENTF_MOVE = 0x0001,
			MOUSEEVENTF_LEFTDOWN = 0x0002,
			MOUSEEVENTF_LEFTUP = 0x0004,
			MOUSEEVENTF_RIGHTDOWN = 0x0008,
			MOUSEEVENTF_RIGHTUP = 0x0010,
			MOUSEEVENTF_MIDDLEDOWN = 0x0020,
			MOUSEEVENTF_MIDDLEUP = 0x0040,
			MOUSEEVENTF_XDOWN = 0x0080,
			MOUSEEVENTF_XUP = 0x0100,
			MOUSEEVENTF_WHEEL = 0x0800,
			MOUSEEVENTF_VIRTUALDESK = 0x4000,
			MOUSEEVENTF_ABSOLUTE = 0x8000;
		[StructLayout(LayoutKind.Explicit)]
		struct INPUT {
			[FieldOffset(0)]
			public int type;
			[FieldOffset(sizeof(int))]
			public MOUSEINPUT mi;
			[FieldOffset(sizeof(int))]
			public KEYBDINPUT ki;
			[FieldOffset(sizeof(int))]
			public HARDWAREINPUT hi;
		}
		[StructLayout(LayoutKind.Sequential)]
		struct MOUSEINPUT {
			public int dx;
			public int dy;
			public int mouseData;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}
		[StructLayout(LayoutKind.Sequential)]
		struct KEYBDINPUT {
			public ushort wVk;
			public ushort wScan;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}
		[StructLayout(LayoutKind.Sequential)]
		struct HARDWAREINPUT {
			public uint uMsg;
			public ushort wParamL;
			public ushort wParamH;
		}
		InputHelperActionsDelegate beforeAction;
		InputHelperActionsDelegate afterAction;
		public InputHelperActionsDelegate BeforeAction { get { return beforeAction; } set { beforeAction = value; } }
		public InputHelperActionsDelegate AfterAction { get { return afterAction; } set { afterAction = value; } }
		[DllImport("user32.dll")]
		static extern uint SendInput(uint numberOfInputs, ref INPUT input, int structSize);
		[DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
		static extern int GetSystemMetricsCore(int nIndex);
		[System.Security.SecuritySafeCritical]
		static int GetSystemMetrics(int nIndex) {
			return GetSystemMetricsCore(nIndex);
		}
 	[System.Security.SecuritySafeCritical]
		void SendAction(ref INPUT input) {
			if (BeforeAction != null) {
				BeforeAction();
			}
			SendInput(1, ref input, Marshal.SizeOf(input));
			if (AfterAction != null) {
				AfterAction();
			}
		}
		void SendMouseAction(double x, double y, uint flags, int data) {
			INPUT input = new INPUT();
			input.type = INPUT_MOUSE;
			input.mi = new MOUSEINPUT();
			input.mi.dwFlags = flags;
			input.mi.mouseData = data;
			if ((flags & MOUSEEVENTF_ABSOLUTE) == 0) {
				input.mi.dx = (int)x;
				input.mi.dy = (int)y;
			} else {
				input.mi.dx = (int)((x + 0.5) * (AbsoluteScreenSize / GetSystemMetrics(0)));
				input.mi.dy = (int)((y + 0.5) * (AbsoluteScreenSize / GetSystemMetrics(1)));
			}
			SendAction(ref input);
		}
		public void SendMouseLDown(int screenX, int screenY) {
			SendMouseAction(screenX, screenY, MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN,0);
		}
		public void SendMouseLUp(int screenX, int screenY) {
			SendMouseAction(screenX, screenY, MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTUP, 0);
		}
		public void SendMouseRDown(int screenX, int screenY) {
			SendMouseAction(screenX, screenY, MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_RIGHTDOWN, 0);
		}
		public void SendMouseRUp(int screenX, int screenY) {
			SendMouseAction(screenX, screenY, MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_RIGHTUP, 0);
		}
		public void SendMouseMove(int screenX, int screenY, uint flags) {
			SendMouseAction(screenX, screenY, flags | MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, 0);
		}
		public void SendMouseWheel(int screenX, int screenY, bool forward) {
			SendMouseAction(screenX, screenY, MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_WHEEL, forward ? 120 : -120);
		}
		void SendKey(uint uCode, KeyCodeType keyCodeType, bool push) {
			INPUT input = new INPUT();
			input.type = INPUT_KEYBOARD;
			input.ki = new KEYBDINPUT();
			if (keyCodeType == KeyCodeType.VirtualCode) {
				input.ki.dwFlags = 0;
				input.ki.wVk = (ushort)uCode;
				input.ki.wScan = 0;
			} else {
				input.ki.dwFlags = KEYEVENTF_SCANCODE;
				input.ki.wVk = 0;
				if ((uCode & 0xFF00) == 0xE000) {
					input.ki.dwFlags |= KEYEVENTF_EXTENDEDKEY;
				}
				input.ki.wScan = (ushort)(uCode & 0xFF);
			}
			if (!push) {
				input.ki.dwFlags |= KEYEVENTF_KEYUP;
			}
			SendAction(ref input);
		}
		public void SendKeyDown(uint uCode, KeyCodeType keyCodeType) {
			SendKey(uCode, keyCodeType, true);
		}
		public void SendKeyUp(uint uCode, KeyCodeType keyCodeType) {
			SendKey(uCode, keyCodeType, false);
		}
	}
}
