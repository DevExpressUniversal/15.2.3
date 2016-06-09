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
using System.Threading;
using System.Windows.Forms;
#if SLTESTSERVICE
using DevExpress.Xpf.SilverlightTestService;
#endif
namespace DevExpress.XtraTests {
	public enum MouseAction { LDown, LUp, LClick, Move, WheelForward, WheelBackward, RDown, RUp }
	public delegate void MouseInputAction(double screenX, double screenY);
	public delegate void SendActionsDelegate();
	[CLSCompliant(false)]
	public abstract class WindowsTesterBase {
		public static bool EnableLogging { get; set; }
#if SLTESTSERVICE
		public static LogForm LogForm { get; set; }
		public static void InitLogForm() {
			LogForm = new LogForm();
		}
		public static void ShowLogForm() {
			if(LogForm == null)
				InitLogForm();
			LogForm.Show();
		}
		public static void AddLog(string log) {
			if(LogForm == null || LogForm.IsDisposed)
				LogForm = new LogForm();
			LogForm.AddLog(log);
		}
#endif
		int mouseMoveDelay;
		int keyboardDelay;
		IInputHelper inputHelper;
		System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
		SendActionsDelegate toModalWindowActions;
		IntPtr lastFocusedElement;
		const int DefaultMouseMoveDelay = 0;
		const int DefaultKeyboardDelay = 0;
		static public void Delay(int millisecs) { Thread.Sleep(millisecs); }
		public WindowsTesterBase() {
			this.inputHelper = new InputHelper();
			MouseMoveDelay = DefaultMouseMoveDelay;
			KeyboardDelay = DefaultKeyboardDelay;
			InputHelper.BeforeAction = InputHelperBeforeAction;
			InputHelper.AfterAction = InputHelperAfterAction;
		}
		public abstract void DoEvents();
		protected abstract double GetScreenWidth();
		protected abstract double GetScreenHeight();
		protected abstract object GetActiveControl();
		protected abstract IntPtr GetHandle();
		protected abstract bool HasActiveControl { get; }
		IntPtr LastFocusedElement { get { return lastFocusedElement; } set { lastFocusedElement = value; } }
		public int MouseMoveDelay { get { return mouseMoveDelay; } set { mouseMoveDelay = value; } }
		public int KeyboardDelay { get { return keyboardDelay; } set { keyboardDelay = value; } }
		protected IInputHelper InputHelper { get { return inputHelper; } }
		void UpdateInputLanguage() {
			if(InputLanguage.CurrentInputLanguage != InputLanguage.FromCulture(System.Windows.Forms.Application.CurrentCulture))
				InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(System.Windows.Forms.Application.CurrentCulture);
		}
		public void SendString(string keys) {
			UpdateInputLanguage();
			List<KeyAction> keyActions = new KeysStringParser(keys).Parse();
			foreach (KeyAction action in keyActions) {
				if (action.isSealedKey) {
					if (action.sealedKeyState) {
						InputHelper.SendKeyDown(action.vCode, KeyCodeType.VirtualCode);
					} else {
						InputHelper.SendKeyUp(action.vCode, KeyCodeType.VirtualCode);
					}
				} else {
					InputHelper.SendKeyDown(action.vCode, KeyCodeType.VirtualCode);
					InputHelper.SendKeyUp(action.vCode, KeyCodeType.VirtualCode);
				}
				Delay(KeyboardDelay);
			}
		}
		public void SendSystemKey(ushort[] keys, bool[] isKeyUp) {
			SendInputHelper.SendKey(keys, isKeyUp);
		}
		public void SendSystemKeyDown(uint key, uint flags) {
			SendInputHelper.SendKey((ushort)key, false);
		}
		public void SendSystemKeyUp(uint key, uint flags) {
			SendInputHelper.SendKey((ushort)key, true);
		}
 	[System.Security.SecuritySafeCritical]
		public void SendKeyDown(uint key, uint flags) {
			TestImports.SendMessage(GetHandle(), TestImports.WM_KEYDOWN, key, flags);
		}
 	[System.Security.SecuritySafeCritical]
		public void SendKeyUp(uint key, uint flags) {
			TestImports.SendMessage(GetHandle(), TestImports.WM_KEYUP, key, flags);
		}
		public void SendKeyDown(string keys) {
			List<KeyAction> keyActions = new KeysStringParser(keys).Parse();
			InputHelper.SendKeyDown(keyActions[0].vCode, KeyCodeType.VirtualCode);
		}
		public void SendKeyUp(string keys) {
			List<KeyAction> keyActions = new KeysStringParser(keys).Parse();
			InputHelper.SendKeyUp(keyActions[0].vCode, KeyCodeType.VirtualCode);
		}
 	[System.Security.SecuritySafeCritical]
		public void SendKey(char key) {
			uint virtualCode = TestImports.VkKeyScan(key) & 255;
			TestImports.SendMessage(GetHandle(), TestImports.WM_KEYDOWN, virtualCode, 0);
			TestImports.SendMessage(GetHandle(), TestImports.WM_CHAR, key, 0);
			TestImports.SendMessage(GetHandle(), TestImports.WM_KEYUP, virtualCode, 0);
		}
		public virtual void SendLMouseDown(double screenX, double screenY) {
			SetCursorPos(screenX, screenY);
			InputHelper.SendMouseLDown(Convert.ToInt32(screenX), Convert.ToInt32(screenY));
		}
		public virtual void SendRMouseDown(double screenX, double screenY) {
			SetCursorPos(screenX, screenY);
			InputHelper.SendMouseRDown(Convert.ToInt32(screenX), Convert.ToInt32(screenY));
		}
		public virtual void SendLMouseUp(double screenX, double screenY) {
			SetCursorPos(screenX, screenY);
			InputHelper.SendMouseLUp(Convert.ToInt32(screenX), Convert.ToInt32(screenY));
		}
		public virtual void SendRMouseUp(double screenX, double screenY) {
			SetCursorPos(screenX, screenY);
			InputHelper.SendMouseRUp(Convert.ToInt32(screenX), Convert.ToInt32(screenY));
		}
		public virtual void SendMouseMove(double screenX, double screenY) {
			SetCursorPos(screenX, screenY);
#if SLTESTSERVICE
			if(WindowsTesterBase.EnableLogging)
				WindowsTesterBase.AddLog(DateTime.Now.ToString() + ": Mouse: Move" + " pt=" + screenX + "," + screenY);
#endif
			InputHelper.SendMouseMove(Convert.ToInt32(screenX), Convert.ToInt32(screenY), 0);
		}
		public virtual void SendMouseWheel(double screenX, double screenY, bool forward) {
#if SLTESTSERVICE
			if(WindowsTesterBase.EnableLogging)
				WindowsTesterBase.AddLog(DateTime.Now.ToString() + ": Mouse: Wheel" + (forward? "Forward": "Back") + " pt=" + screenX + "," + screenY);
#endif
			InputHelper.SendMouseWheel(Convert.ToInt32(screenX), Convert.ToInt32(screenY), forward);
		}
		public virtual void SendLMouseClick(double screenX, double screenY) {
			SetCursorPos(screenX, screenY);
			InputHelper.SendMouseLDown(Convert.ToInt32(screenX), Convert.ToInt32(screenY));
			InputHelper.SendMouseLUp(Convert.ToInt32(screenX), Convert.ToInt32(screenY));
		}
		public virtual void SendRMouseClick(double screenX, double screenY) {
			SetCursorPos(screenX, screenY);
			InputHelper.SendMouseRDown(Convert.ToInt32(screenX), Convert.ToInt32(screenY));
			InputHelper.SendMouseRUp(Convert.ToInt32(screenX), Convert.ToInt32(screenY));
		}
 	[System.Security.SecuritySafeCritical]
		public void MoveMousePointTo(int ptX, int ptY) {
			TestImports.Win32Point curPos = new TestImports.Win32Point();
			TestImports.GetCursorPos(out curPos);
			while ((ptX != curPos.X) || (ptY != curPos.Y)) {
				int dX = curPos.X - ptX,
					dY = curPos.Y - ptY;
				if (curPos.X == ptX) {
					dX = 0;
				} else {
					dX = curPos.X < ptX ? 1 : -1;
				}
				if (curPos.Y == ptY) {
					dY = 0;
				} else {
					dY = curPos.Y < ptY ? 1 : -1;
				}
				curPos.X += dX;
				curPos.Y += dY;
				InputHelper.SendMouseMove(curPos.X, curPos.Y, 0);
				Delay(MouseMoveDelay);
			}
		}
		public void SendToModalWindow(SendActionsDelegate actions, int delay) {
			this.toModalWindowActions = actions;
			this.timer.Tick += new EventHandler(timer_Elapsed);
			this.timer.Interval = delay;
			this.timer.Start();
		}
		void timer_Elapsed(object sender, EventArgs e) {
			this.timer.Stop();
			this.timer.Tick -= new EventHandler(timer_Elapsed);
			InputHelper.BeforeAction = PrepareToActionInModalWindow;
			InputHelper.AfterAction = null;
			this.toModalWindowActions();
			InputHelper.BeforeAction = InputHelperBeforeAction;
			InputHelper.AfterAction = InputHelperAfterAction;
		}
 	[System.Security.SecuritySafeCritical]
		protected virtual void SetCursorPos(double X, double Y) {
			TestImports.SetCursorPos(Convert.ToInt32(X), Convert.ToInt32(Y));
		}
		TestImports.Win32Point GetCursorPos() {
			TestImports.Win32Point result;
			TestImports.GetCursorPos(out result);
			return result;
		}
 	[System.Security.SecuritySafeCritical]
		void InputHelperBeforeAction() {
			if (LastFocusedElement == IntPtr.Zero) {
				LastFocusedElement = GetHandle();
			}
			TestImports.SetForegroundWindow(LastFocusedElement);
		}
 	[System.Security.SecuritySafeCritical]
		void InputHelperAfterAction() {
			DoEvents();
			LastFocusedElement = TestImports.GetFocus();
		}
 	[System.Security.SecuritySafeCritical]
		void PrepareToActionInModalWindow() {
			TestImports.SetForegroundWindow(GetTopWindowHandle());
		}
 	[System.Security.SecuritySafeCritical]
		public IntPtr GetTopWindowHandle() {
			IntPtr desktopWndHandle = TestImports.GetDesktopWindow();
			IntPtr windowHandle = TestImports.GetTopWindow(desktopWndHandle);
			int processId = System.Diagnostics.Process.GetCurrentProcess().Id;
			while (!IsWindowBelongToProcess(windowHandle, processId) || windowHandle == IntPtr.Zero) {
				windowHandle = TestImports.GetWindow(windowHandle, (uint)GetWindowCmd.GW_HWNDNEXT);
			}
			return windowHandle;
		}
 	[System.Security.SecuritySafeCritical]
		protected bool IsWindowBelongToProcess(IntPtr hWnd, int expectedProcessId) {
			uint windowProcessId;
			TestImports.GetWindowThreadProcessId(hWnd, out windowProcessId);
			if (!TestImports.IsWindowVisible(hWnd)) return false;
			return expectedProcessId == windowProcessId;
		}
		void SendMouseWheelForward(double screenX, double screenY) {
			SendMouseWheel(screenX, screenY, true);
		}
		void SendMouseWheelBackward(double screenX, double screenY) {
			SendMouseWheel(screenX, screenY, false);
		}
		public MouseInputAction GetMouseInputAction(MouseAction action) {
			switch(action) {
				case MouseAction.LDown:
					return SendLMouseDown;
				case MouseAction.LUp:
					return SendLMouseUp;
				case MouseAction.LClick:
					return SendLMouseClick;
				case MouseAction.Move:
					return SendMouseMove;
				case MouseAction.RDown:
					return SendRMouseDown;
				case MouseAction.RUp:
					return SendRMouseUp;
				case MouseAction.WheelForward:
					return SendMouseWheelForward;
				case MouseAction.WheelBackward:
					return SendMouseWheelBackward;
				default:
					throw new NotImplementedException();
			}
		}
	}
	[CLSCompliant(false)]
	public static class SendInputHelper {
		[StructLayout(LayoutKind.Sequential)]
		public struct INPUT {
			public int type;
			public INPUTUNION inputUnion;
		}
		[StructLayout(LayoutKind.Explicit)]
		public struct INPUTUNION {
			[FieldOffset(0)]
			public HARDWAREINPUT hi;
			[FieldOffset(0)]
			public KEYBDINPUT ki;
			[FieldOffset(0)]
			public MOUSEINPUT mi;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct HARDWAREINPUT {
			public int uMsg;
			public ushort wParamL;
			public ushort wParamH;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct MOUSEINPUT {
			public int dx;
			public int dy;
			public int mouseData;
			public int dwFlags;
			public int time;
			public IntPtr dwExtraInfo;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct KEYBDINPUT {
			public ushort wVk;
			public ushort wScan;
			public uint dwFlags;
			public int time;
			public IntPtr dwExtraInfo;
		}
		const int INPUT_MOUSE = 0;
		const int INPUT_KEYBOARD = 1;
		const int INPUT_HARDWARE = 2;
		const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
		const uint KEYEVENTF_KEYUP = 0x0002;
		const uint KEYEVENTF_UNICODE = 0x0004;
		const uint KEYEVENTF_SCANCODE = 0x0008;
		const uint XBUTTON1 = 0x0001;
		const uint XBUTTON2 = 0x0002;
		const uint MOUSEEVENTF_MOVE = 0x0001;
		const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
		const uint MOUSEEVENTF_LEFTUP = 0x0004;
		const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
		const uint MOUSEEVENTF_RIGHTUP = 0x0010;
		const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
		const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
		const uint MOUSEEVENTF_XDOWN = 0x0080;
		const uint MOUSEEVENTF_XUP = 0x0100;
		const uint MOUSEEVENTF_WHEEL = 0x0800;
		const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
		const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
		[DllImport("user32.dll", SetLastError = true)]
		static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
		[DllImport("user32.dll")]
		static extern bool BlockInput(bool fBlockIt);
		public static void SendKey(ushort virtualKey, bool isKeyUp) {
			INPUT input = new INPUT();
			input.type = INPUT_KEYBOARD;
			input.inputUnion.ki.wVk = virtualKey;
			input.inputUnion.ki.wScan = 0;
			input.inputUnion.ki.dwFlags = isKeyUp ? KEYEVENTF_KEYUP : 0;
			input.inputUnion.ki.time = 0;
			input.inputUnion.ki.dwExtraInfo = IntPtr.Zero;
			uint res = SendInput(1, new INPUT[] { input }, Marshal.SizeOf(input));
			if(res != 1) {
				int error = Marshal.GetLastWin32Error();
				throw new Exception(String.Format("SendKey returns 0x{0:X8}", error));
			}
		}
 	[System.Security.SecuritySafeCritical]
		public static void SendKey(ushort[] virtualKeys, bool[] isKeyUp) {
			INPUT[] input = new INPUT[virtualKeys.Length];
			for(int i = 0; i < input.Length; i++) {
				input[i].type = INPUT_KEYBOARD;
				input[i].inputUnion.ki.wVk = virtualKeys[i];
				input[i].inputUnion.ki.wScan = 0;
				input[i].inputUnion.ki.dwFlags = isKeyUp[i] ? KEYEVENTF_KEYUP : 0;
				input[i].inputUnion.ki.time = 0;
				input[i].inputUnion.ki.dwExtraInfo = IntPtr.Zero;
			}
			uint res = SendInput((uint)input.Length, input, Marshal.SizeOf(typeof(INPUT)));
			if(res != input.Length) {
				int error = Marshal.GetLastWin32Error();
				throw new Exception(String.Format("SendKey returns 0x{0:X8}", error));
			}
		}
	}
}
