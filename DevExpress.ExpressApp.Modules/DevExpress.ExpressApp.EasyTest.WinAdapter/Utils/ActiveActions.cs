#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections;
using System.Threading;
using System.Security;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter.Utils {
	internal enum InputSealedKey { ALT = Keys.Alt, CTRL = Keys.Control, SHIFT = Keys.Shift }
	internal enum InputSealedKeyValue { ALTVALUE = 18, CTRLVALUE = 17, SHIFTVALUE = 0x10 }
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
		public const int WM_KEYUP = 0x0101;
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
		[DllImport("user32.dll", EntryPoint = "PeekMessage")]
		extern public static uint PeekMessage(Msg msg, System.IntPtr hWnd, uint firstMessage, uint lastMessage, uint options);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		internal static extern int SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		internal static extern int PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern short GetKeyState(int keyCode);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern bool SetKeyboardState([MarshalAs(UnmanagedType.LPArray)] byte[] bytes);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern bool GetKeyboardState(byte[] bytes);
		public static bool CapsIsPressed {
			get { return GetKeyPressed(Keys.CapsLock); }
			set { SetKeyPressed(Keys.CapsLock, value); }
		}
		public static bool GetKeyPressed(Keys key) {
			uint keyValue = GetKeyValue(key);
			if(keyValue > 255) return false;
			byte[] bytes = new byte[255];
			if(GetKeyboardState(bytes)) {
				return bytes[keyValue] != 0;
			}
			return false;
		}
		public static void SetKeyPressed(Keys key, bool value) {
			uint keyValue = GetKeyValue(key);
			if(keyValue > 255) return;
			byte[] bytes = new byte[255];
			if(GetKeyboardState(bytes)) {
				if(value) {
					bytes[keyValue] = 128;
				}
				else {
					bytes[keyValue] = 0;
				}
				SetKeyboardState(bytes);
			}
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
			for(i = 0; i < ar.Length; i++)
				if((Keys)ar.GetValue(i) == key) break;
			if(i < ar.Length) {
				ar = Enum.GetValues(typeof(InputSealedKeyValue));
				return (int)ar.GetValue(i);
			}
			return -1;
		}
	}
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
		public MouseInputArgs(long dx, long dy, uint dwFlags, uint mouseData)
			: this(dx, dy, dwFlags) {
			this.mouseData = mouseData;
		}
	}
	public enum KeyEventType { KeyDown, KeyUp }
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
	public class ActiveActionsImports {
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
		public const int WM_KEYUP = 0x0101;
		public const int WM_CHAR = 0x0102;
		public const int WM_ACTIVATEAPP = 0x001C;
		public const int WM_LBUTTONDOWN = 0x0201;
		public const int WM_LBUTTONUP = 0x0202;
		public const int WM_LBUTTONDBLCLK = 0x0203;
		public const int WM_RBUTTONDOWN = 0x0204;
		public const int WM_RBUTTONUP = 0x0205;
		public const int WM_RBUTTONDBLCLK = 0x0206;
		public const int WM_MBUTTONDOWN = 0x0207;
		public const int WM_MBUTTONUP = 0x0208;
		public const int WM_MBUTTONDBLCLK = 0x0209;
		[DllImport("user32.dll", EntryPoint = "SendInput")]
		extern public static uint SendMouseInput(int nInputs, [MarshalAs(UnmanagedType.LPArray)] MouseInputArgs[] pInputs, int cbSize);
		[DllImport("user32.dll", EntryPoint = "SendInput")]
		extern public static uint SendKeyInput(int nInputs, [MarshalAs(UnmanagedType.LPArray)] KeyInputArgs[] pInputs, int cbSize);
		[DllImport("kernel32.dll", EntryPoint = "GetLastError")]
		extern public static uint GetLastError();
		[DllImport("user32.dll", EntryPoint = "MapVirtualKey")]
		extern public static uint MapVirtualKey(uint uCode, uint uMapType);
		[DllImport("user32.dll")]
		extern public static void mouse_event(uint dwFlags, int dx, int dy, uint dwData, IntPtr dwExtraInfo);
		[DllImport("user32.dll", EntryPoint = "PeekMessage")]
		extern public static uint PeekMessage(Msg msg, System.IntPtr hWnd, uint firstMessage, uint lastMessage, uint options);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		internal static extern int SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		internal static extern int PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern short GetKeyState(int keyCode);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern bool SetKeyboardState([MarshalAs(UnmanagedType.LPArray)] byte[] bytes);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern bool GetKeyboardState(byte[] bytes);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern IntPtr GetActiveWindow();
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(Point point);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref int lpdwProcessId);
		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		public static extern uint GetCurrentThreadId();
		public static bool CapsIsPressed {
			get { return GetKeyPressed(Keys.CapsLock); }
			set { SetKeyPressed(Keys.CapsLock, value); }
		}
		public static bool GetKeyPressed(Keys key) {
			uint keyValue = GetKeyValue(key);
			if(keyValue > 255) return false;
			byte[] bytes = new byte[255];
			if(GetKeyboardState(bytes)) {
				return bytes[keyValue] != 0;
			}
			return false;
		}
		public static void SetKeyPressed(Keys key, bool value) {
			uint keyValue = GetKeyValue(key);
			if(keyValue > 255) return;
			byte[] bytes = new byte[255];
			if(GetKeyboardState(bytes)) {
				if(value) {
					bytes[keyValue] = 128;
				}
				else {
					bytes[keyValue] = 0;
				}
				SetKeyboardState(bytes);
			}
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
			for(i = 0; i < ar.Length; i++)
				if((Keys)ar.GetValue(i) == key) break;
			if(i < ar.Length) {
				ar = Enum.GetValues(typeof(InputSealedKeyValue));
				return (int)ar.GetValue(i);
			}
			return -1;
		}
	}
	public enum ActiveActionsCancelMode { None, ApplicationDeactivated, UserCancel, UnknownTopWindow };
	public class ActiveActions : object, IMessageFilter, IDisposable {
		public class WndForm : Form {
			ActiveActions ActiveActions;
			public WndForm(ActiveActions ActiveActions) {
				this.ActiveActions = ActiveActions;
				StartPosition = FormStartPosition.Manual;
				FormBorderStyle = FormBorderStyle.None;
				ShowInTaskbar = false;
				Width = 1;
				Height = 1;
				Left = -100;
				Top = -100;
			}
			protected override void WndProc(ref Message m) {
				base.WndProc(ref m);
				if((m.Msg == ActiveActionsImports.WM_ACTIVATEAPP) && !this.ActiveActions.Canceled) {
					this.ActiveActions.CancelMode = ActiveActionsCancelMode.ApplicationDeactivated;
				}
			}
		}
		const int DefaultMouseMoveDelay = 0;
		const int DefaultMouseMoveDelayPerPixels = 50;
		const int DefaultKeyboardDelay = 0;
		const uint MOUSEEVENTF_MOVE = 0x0001,
			MOUSEEVENTF_LEFTDOWN = 0x0002,
			MOUSEEVENTF_LEFTUP = 0x0004,
			MOUSEEVENTF_RIGHTDOWN = 0x0008,
			MOUSEEVENTF_RIGHTUP = 0x0010,
			MOUSEEVENTF_MIDDLEDOWN = 0x0020,
			MOUSEEVENTF_MIDDLEUP = 0x0040,
			MOUSEEVENTF_WHEEL = 0x0800,
			MOUSEEVENTF_ABSOLUTE = 0x8000;
		ActiveActionsCancelMode canceleMode;
		WndForm wndForm;
		bool keyActiveActionsProccessing;
		int mouseMoveDelay = DefaultMouseMoveDelay;
		int mouseMoveDelayPerPixels = DefaultMouseMoveDelayPerPixels;
		int keyboardDelay = DefaultKeyboardDelay;
		public ActiveActions() {
			this.canceleMode = ActiveActionsCancelMode.None;
			Application.AddMessageFilter(this);
			wndForm = new WndForm(this);
			wndForm.Show();
		}
		public virtual void Dispose() {
			Application.RemoveMessageFilter(this);
			wndForm.Hide();
			wndForm.Dispose();
			wndForm = null;
		}
		public int MouseMoveDelay { get { return mouseMoveDelay; } set { mouseMoveDelay = value; } }
		public int MouseMoveDelayPerPixels { get { return mouseMoveDelayPerPixels; } set { mouseMoveDelayPerPixels = value; } }
		public int KeyboardDelay { get { return keyboardDelay; } set { keyboardDelay = value; } }
		public bool Canceled { get { return CancelMode != ActiveActionsCancelMode.None; } }
		public ActiveActionsCancelMode CancelMode { get { return this.canceleMode; } set { this.canceleMode = value; } }
		private int eventsLockCount;
		public bool EventsLocked {
			get { return eventsLockCount > 0; }
		}
		public void LockEvents() {
			eventsLockCount++;
		}
		public void UnlockEvents() {
			eventsLockCount--;
			if(eventsLockCount == 0) {
				DoEvents();
			}
			if(eventsLockCount < 0) {
				throw new InvalidOperationException("The 'UnlockEvents' method should be called AFTER the 'LockEvents' method call.");
			}
		}
		public void DoEvents() {
			if(!EventsLocked) {
				SendKeys.Flush();
				Application.DoEvents();
			}
		}
		public void SendKey(Control control, char key) {
			if(Canceled) return;
			SendKeyCore(control != null ? control : ActiveControl, key);
			SendKeys.Flush();
		}
		public void SendString(Control control, string keys) {
			if(Canceled) return;
			SendStringCore(control != null ? control : ActiveControl, keys);
			SendKeys.Flush();
		}
		public void MoveMousePointTo(Control control, Point pt) {
			Point screenPoint = control == null ? pt : control.PointToScreen(pt);
			MoveMousePointTo(screenPoint);
		}
		public void MoveMousePointTo(Point pt) {
			if(Canceled) return;
			int count = 0, countX = 1, countY = 1;
			while(!Cursor.Position.Equals(pt)) {
				if(Canceled) return;
				Point cpt = Cursor.Position;
				int dX = cpt.X - pt.X, dY = cpt.Y - pt.Y;
				double dXY = dY != 0 ? Math.Abs((double)dX / (double)dY) : 1;
				double dYX = dX != 0 ? Math.Abs((double)dY / (double)dX) : 1;
				if((cpt.X == pt.X) || (dXY * (countX++) <= 1))
					dX = 0;
				else {
					dX = cpt.X < pt.X ? 1 : -1;
					countX = 1;
				}
				if((cpt.Y == pt.Y) || (dYX * (countY++) <= 1))
					dY = 0;
				else {
					dY = cpt.Y < pt.Y ? 1 : -1;
					countY = 1;
				}
				cpt.X += dX;
				cpt.Y += dY;
				Cursor.Position = cpt;
				int x = Convert.ToInt32((65536.0 * cpt.X / Screen.PrimaryScreen.Bounds.Width) + (65536.0 / Screen.PrimaryScreen.Bounds.Width / 2));
				int y = Convert.ToInt32((65536.0 * cpt.Y / Screen.PrimaryScreen.Bounds.Height) + (65536.0 / Screen.PrimaryScreen.Bounds.Height / 2));
				ActiveActionsImports.mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, x, y, 0, IntPtr.Zero);
				DoEvents();
				if(count++ == MouseMoveDelayPerPixels) {
					Delay(MouseMoveDelay);
					count = 0;
				}
			}
		}
		public void MouseClick() {
			MouseClick(MouseButtons.Left);
		}
		public void MouseClick(MouseButtons mouseButtons) {
			MouseClick(Cursor.Position, mouseButtons);
		}
		public void MouseClick(Control control, Point pt) {
			MouseClick(control, pt, MouseButtons.Left);
		}
		public void MouseClick(IntPtr handle, Point pt, MouseButtons mouseButtons) {
			if(Canceled) return;
			uint lParam = (uint)((pt.Y << 16) + pt.X);
			ActiveActionsImports.SendMessage(handle, GetMouseClickMessage(mouseButtons, true), 0, lParam);
			DoEvents();
			ActiveActionsImports.SendMessage(handle, GetMouseClickMessage(mouseButtons, false), 0, lParam);
			DoEvents();
		}
		public void MouseClick(Control control, Point pt, MouseButtons mouseButtons) {
			if(control != null) {
				MouseClick(control.Handle, pt, mouseButtons);
			}
			else {
				MouseClick(pt, mouseButtons);
			}
		}
		[SecuritySafeCritical]
		public void MouseClick(Point pt) {
			MouseClick(pt, MouseButtons.Left);
		}
		[SecuritySafeCritical]
		public void MouseClick(Point pt, MouseButtons mouseButtons) {
			if(Canceled) return;
			if(CheckMouse(pt)) {
				ActiveActionsImports.mouse_event(GetMouseFlagsByMouseButtons(mouseButtons, true), Convert.ToInt32(pt.X), Convert.ToInt32(pt.Y), 0, IntPtr.Zero);
				DoEvents();
				ActiveActionsImports.mouse_event(GetMouseFlagsByMouseButtons(mouseButtons, false), Convert.ToInt32(pt.X), Convert.ToInt32(pt.Y), 0, IntPtr.Zero);
				DoEvents();
			}
		}
		public void MouseDown() {
			MouseDown(MouseButtons.Left);
		}
		public void MouseDown(MouseButtons mouseButtons) {
			MouseDown(Cursor.Position, mouseButtons);
		}
		public void MouseDown(Control control, Point pt) {
			MouseDown(control, pt, MouseButtons.Left);
		}
		public void MouseDown(Control control, Point pt, MouseButtons mouseButtons) {
			Point screenPoint = control == null ? pt : control.PointToScreen(pt);
			MouseDown(screenPoint, mouseButtons);
		}
		public void MouseDown(Point pt) {
			MouseDown(pt, MouseButtons.Left);
		}
		[SecuritySafeCritical]
		public void MouseDown(Point pt, MouseButtons mouseButtons) {
			if(Canceled) return;
			this.MoveMousePointTo(pt);
			if(CheckMouse(pt)) {
				ActiveActionsImports.mouse_event(GetMouseFlagsByMouseButtons(mouseButtons, true), Convert.ToInt32(pt.X), Convert.ToInt32(pt.Y), 0, IntPtr.Zero);
				DoEvents();
			}
		}
		public void MouseUp() {
			MouseUp(MouseButtons.Left);
		}
		public void MouseUp(MouseButtons mouseButtons) {
			MouseUp(Cursor.Position, mouseButtons);
		}
		public void MouseUp(Control control, Point pt) {
			MouseUp(control, pt, MouseButtons.Left);
		}
		public void MouseUp(Control control, Point pt, MouseButtons mouseButtons) {
			Point screenPoint = control == null ? pt : control.PointToScreen(pt);
			MouseUp(screenPoint, mouseButtons);
		}
		public void MouseUp(Point pt) {
			MouseUp(pt, MouseButtons.Left);
		}
		[SecuritySafeCritical]
		public void MouseUp(Point pt, MouseButtons mouseButtons) {
			if(Canceled) return;
			this.MoveMousePointTo(pt);
			if(CheckMouse(pt)) {
				ActiveActionsImports.mouse_event(GetMouseFlagsByMouseButtons(mouseButtons, false), Convert.ToInt32(pt.X), Convert.ToInt32(pt.Y), 0, IntPtr.Zero);
				DoEvents();
			}
		}
		public void MouseDblClick() {
			MouseDblClick(MouseButtons.Left);
		}
		public void MouseDblClick(MouseButtons mouseButtons) {
			MouseDblClick(Cursor.Position, mouseButtons);
		}
		public void MouseDblClick(Control control, Point pt) {
			MouseDblClick(control, pt, MouseButtons.Left);
		}
		[SecuritySafeCritical]
		public void MouseDblClick(IntPtr handle, Point pt, MouseButtons mouseButtons) {
			if(Canceled) return;
			uint lParam = (uint)((pt.Y << 16) + pt.X);
			ActiveActionsImports.SendMessage(handle, GetMouseClickMessage(mouseButtons, true), 0, lParam);
			ActiveActionsImports.SendMessage(handle, GetMouseClickMessage(mouseButtons, false), 0, lParam);
			DoEvents();
			ActiveActionsImports.SendMessage(handle, GetMouseClickMessage(mouseButtons, true), 0, lParam);
			ActiveActionsImports.SendMessage(handle, GetMouseClickMessage(mouseButtons, false), 0, lParam);
			DoEvents();
		}
		public void MouseDblClick(Control control, Point pt, MouseButtons mouseButtons) {
			if(control != null) {
				MouseDblClick(control.Handle, pt, mouseButtons);
			}
			else {
				MouseDblClick(pt, mouseButtons);
			}
		}
		public void MouseDblClick(Point pt) {
			MouseDblClick(pt, MouseButtons.Left);
		}
		[SecuritySafeCritical]
		public void MouseDblClick(Point pt, MouseButtons mouseButtons) {
			if(Canceled) return;
			this.MoveMousePointTo(pt);
			if(CheckMouse(pt)) {
				ActiveActionsImports.mouse_event(GetMouseFlagsByMouseButtons(mouseButtons, true), Convert.ToInt32(pt.X), Convert.ToInt32(pt.Y), 0, IntPtr.Zero);
				ActiveActionsImports.mouse_event(GetMouseFlagsByMouseButtons(mouseButtons, false), Convert.ToInt32(pt.X), Convert.ToInt32(pt.Y), 0, IntPtr.Zero);
				DoEvents();
				ActiveActionsImports.mouse_event(GetMouseFlagsByMouseButtons(mouseButtons, true), Convert.ToInt32(pt.X), Convert.ToInt32(pt.Y), 0, IntPtr.Zero);
				ActiveActionsImports.mouse_event(GetMouseFlagsByMouseButtons(mouseButtons, false), Convert.ToInt32(pt.X), Convert.ToInt32(pt.Y), 0, IntPtr.Zero);
				DoEvents();
			}
		}
		[SecuritySafeCritical]
		bool CheckMouse(Point pt) {
			IntPtr handle = ActiveActionsImports.WindowFromPoint(pt);
			if(handle != IntPtr.Zero) {
				uint currentTaskId = ActiveActionsImports.GetCurrentThreadId();
				int dummyvalue = 1;
				uint windowTaskId = ActiveActionsImports.GetWindowThreadProcessId(handle, ref dummyvalue);
				if(currentTaskId != windowTaskId)
					CancelMode = ActiveActionsCancelMode.UnknownTopWindow;
			}
			return !Canceled;
		}
		uint GetMouseFlagsByMouseButtons(MouseButtons mouseButtons, bool down) {
			uint flags = MOUSEEVENTF_ABSOLUTE;
			switch(mouseButtons) {
				case MouseButtons.Left:
					flags = down ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_LEFTUP;
					break;
				case MouseButtons.Middle:
					flags = down ? MOUSEEVENTF_RIGHTDOWN : MOUSEEVENTF_RIGHTUP;
					break;
				case MouseButtons.Right:
					flags = down ? MOUSEEVENTF_MIDDLEDOWN : MOUSEEVENTF_MIDDLEUP;
					break;
			}
			return flags;
		}
		int GetMouseClickMessage(MouseButtons mouseButtons, bool down) {
			int message = 0;
			switch(mouseButtons) {
				case MouseButtons.Left:
					message = down ? ActiveActionsImports.WM_LBUTTONDOWN : ActiveActionsImports.WM_LBUTTONUP;
					break;
				case MouseButtons.Middle:
					message = down ? ActiveActionsImports.WM_MBUTTONDOWN : ActiveActionsImports.WM_MBUTTONUP;
					break;
				case MouseButtons.Right:
					message = down ? ActiveActionsImports.WM_RBUTTONDOWN : ActiveActionsImports.WM_RBUTTONUP;
					break;
			}
			return message;
		}
		bool IMessageFilter.PreFilterMessage(ref Message m) {
			if((m.Msg == ActiveActionsImports.WM_KEYDOWN) && !this.keyActiveActionsProccessing) {
				CancelMode = ActiveActionsCancelMode.UserCancel;
				return true;
			}
			return false;
		}
		void IDisposable.Dispose() {
			Dispose();
		}
		static Control ActiveControl {
			get {
				if(Form.ActiveForm != null)
					return Form.ActiveForm.ActiveControl;
				else return null;
			}
		}
		static public void Delay(int millisecs) {
			Thread.Sleep(millisecs);
		}
		[SecuritySafeCritical]
		void SendKeyCore(Control control, char key) {
			if(control == null) return;
			this.keyActiveActionsProccessing = true;
			try {
				string upstring = new string(key, 1);
				char upkey = upstring.ToUpper()[0];
				ActiveActionsImports.SendMessage(control.Handle, ActiveActionsImports.WM_KEYDOWN, (uint)upkey, 0);
				ActiveActionsImports.SendMessage(control.Handle, ActiveActionsImports.WM_CHAR, (uint)key, 0);
				ActiveActionsImports.SendMessage(control.Handle, ActiveActionsImports.WM_KEYUP, (uint)upkey, 0);
				DoEvents();
				Delay(KeyboardDelay);
			}
			finally {
				this.keyActiveActionsProccessing = false;
			}
		}
		[SecuritySafeCritical]
		void SendStringCore(Control control, string keys) {
			if(control == null) return;
			Keys key;
			this.keyActiveActionsProccessing = true;
			try {
				ArrayList sealedKeyPressed = new ArrayList();
				while(keys != string.Empty) {
					if(HasKey(ref keys, out key))
						InputKey(control, key, sealedKeyPressed);
					else {
						SendKeyCore(control, keys[0]);
						keys = keys.Remove(0, 1);
					}
					Delay(KeyboardDelay);
				}
				foreach(object k in sealedKeyPressed) {
					UnPressedSealedKey(control, (Keys)k);
				}
			}
			finally {
				this.keyActiveActionsProccessing = false;
			}
		}
		[SecuritySafeCritical]
		static void InputKey(Control control, Keys key, ArrayList sealedKeyPressed) {
			if(PressSealedKey(control, key, sealedKeyPressed)) return;
			uint v = ActiveActionsImports.GetKeyValue(key);
			if(Keys.Back == key) {
				ActiveActionsImports.PostMessage(control.Handle, ActiveActionsImports.WM_CHAR, v, 0);
			}
			else {
				if(ActiveControl != null) {
					Message msg = new Message();
					msg.HWnd = control.Handle;
					msg.LParam = (IntPtr)0;
					msg.WParam = (IntPtr)v;
					msg.Msg = ActiveActionsImports.WM_KEYDOWN;
					if(!control.PreProcessMessage(ref msg))
						ActiveActionsImports.SendMessage(control.Handle, ActiveActionsImports.WM_KEYDOWN, v, 0);
					msg.Msg = ActiveActionsImports.WM_KEYUP;
					if(!control.PreProcessMessage(ref msg))
						ActiveActionsImports.SendMessage(control.Handle, ActiveActionsImports.WM_KEYUP, v, 0);
				}
			}
		}
		[SecuritySafeCritical]
		static bool PressSealedKey(Control control, Keys key, ArrayList sealedKeyPressed) {
			if(ActiveActionsImports.IsSealedKey(key)) {
				if(sealedKeyPressed.IndexOf(key) < 0) {
					sealedKeyPressed.Add(key);
					uint v = ActiveActionsImports.GetKeyValue(key);
					ActiveActionsImports.SetKeyPressed(key, true);
					ActiveActionsImports.SendMessage(control.Handle, ActiveActionsImports.WM_KEYDOWN, v, 0);
				}
				return true;
			}
			return false;
		}
		[SecuritySafeCritical]
		static void UnPressedSealedKey(Control control, Keys key) {
			uint v = ActiveActionsImports.GetKeyValue(key);
			ActiveActionsImports.SetKeyPressed(key, false);
			ActiveActionsImports.SendMessage(control.Handle, ActiveActionsImports.WM_KEYUP, v, 0);
		}
		static bool HasKey(ref string keys, out Keys key) {
			key = Keys.A;
			if((keys == string.Empty) || (keys[0] != '[')) return false;
			int closedBracket = keys.IndexOf(']');
			int openBracket = keys.IndexOf('[', 1);
			if((closedBracket < 0) || ((openBracket > 0) && (closedBracket > openBracket)))
				return false;
			string tabKey = keys.Substring(1, closedBracket - 1);
			if(tabKey == string.Empty) return false;
			foreach(Keys i in Enum.GetValues(typeof(Keys))) {
				if(i.ToString().ToUpper() == tabKey.ToUpper()) {
					key = i;
					keys = keys.Remove(0, closedBracket + 1);
					return true;
				}
			}
			return false;
		}
	}
}
