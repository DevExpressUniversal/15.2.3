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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraEditors.Native;
namespace DevExpress.XtraEditors.Senders {
	public class FrenchKeyboardDetector {
		public static bool IsFrenchKeyboard {
			get {
				switch(InputLanguage.CurrentInputLanguage.Culture.LCID) {
					case 1036:
					case 2060:
					case 5132:
					case 6156:
						return true;
					default:
						return false;
				}
			}
		}		
	}
	public class SpanishKeyboardDetector {
		public static bool IsSpanishKeyboard {
			get {
				switch(InputLanguage.CurrentInputLanguage.Culture.LCID) {
					case 3082: 
					case 1034:
					case 11274:
					case 16394:
					case 13322:
					case 9226:
					case 5130:
					case 7178:
					case 12298:
					case 17418:
					case 4106:
					case 18442:
					case 22538:
					case 2058:
					case 19466:
					case 15370:
					case 10250:
					case 20490:
					case 21514:
					case 14346:
					case 8202:
						return true;
					default:
						return false;
				}
			}
		}
	}
	public class BaseSender {
		public virtual bool SendMouseDown(Control control, MouseEventArgs e) {
			return false;
		}
		static Hashtable specialChars;
		static BaseSender() {
			specialChars = new Hashtable();
			specialChars['^'] = "{^}";
			specialChars['+'] = "{+}";
			specialChars['%'] = "{%}";
			specialChars['~'] = "{~}";
			specialChars['{'] = "{{}";
			specialChars['}'] = "{}}";
			specialChars[')'] = "{)}";
			specialChars['('] = "{(}";
		}
		bool IsCapsLock { get { return (EditorsNativeMethods.GetKeyState(0x14 ) & 1) != 0; } }
		bool IsFrenchKeyboard {
			get {
				return FrenchKeyboardDetector.IsFrenchKeyboard;
			}
		}
		public static bool UseSendKeysToProcessMessages = false;
		protected virtual bool CanSendCharMessage(Control destination, object message) {
			if(UseSendKeysToProcessMessages) return false;
			return message is Message;
		}
		public virtual bool SendKeyChar(Control destination, KeyPressEventArgs e, object message) {
			if(CanSendCharMessage(destination, message)) {
				return SendKeyCharEx(destination, e, message);
			}
			string s = e.KeyChar.ToString();
			if(specialChars.ContainsKey(e.KeyChar)) {
				s = specialChars[e.KeyChar].ToString();
			} else if(IsCapsLock) {
				if(Char.IsLetter(e.KeyChar)) {
					if(Char.IsUpper(e.KeyChar))
						s = Char.ToLower(e.KeyChar).ToString();
				} else if(Char.IsNumber(e.KeyChar) && IsFrenchKeyboard) {
					s = "{CAPSLOCK}" + s + "{CAPSLOCK}";
				}
			}
			if(!Application.MessageLoop)  
				SendKeys.SendWait(s);
			else
				SendKeys.Send(s);
			return true;
		}
		bool SendKeyCharEx(Control destination, KeyPressEventArgs e, object message) {
			Message msg = (Message)message;
			NativeMethods.PostMessage(GetDestinationHandle(destination), msg.Msg, msg.WParam, msg.LParam);
			return true;
		}
		IntPtr GetDestinationHandle(Control destination) {
			BaseEdit be = destination as BaseEdit;
			if(be == null) return destination.Handle;
			if(be.InnerControl != null && be.InnerControl.Visible) return be.InnerControl.Handle;
			return be.Handle;
		}
		public virtual bool SendKeyDown(KeyEventArgs e) {
			return false;
		}
		public static object SaveMessage(ref Message msg, object currentMessage) {
			if(msg.Msg == MSG.WM_KEYDOWN || msg.Msg == MSG.WM_SYSKEYDOWN)
				return msg;
			return currentMessage;
		}
		public static bool RequireShowEditor(ref Message msg) {
			if(UseSendKeysToProcessMessages) return false;
			if(msg.Msg == MSG.WM_DEADCHAR) {
				return true;
			}
			return false;
		}
	}
	public class NativeSender : BaseSender {
		const uint 
			MOUSEEVENTF_LEFTDOWN = 0x0002,
			MOUSEEVENTF_RIGHTDOWN = 0x0008,
			MOUSEEVENTF_MIDDLEDOWN = 0x0020,
			MOUSEEVENTF_ABSOLUTE = 0x8000;
		const int
			WM_LBUTTONDOWN = 0x0201,
			WM_RBUTTONDOWN = 0x0204,
			WM_MBUTTONDOWN = 0x0207;
		[System.Security.SecuritySafeCritical]
		static int SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam) { return SendMessageSafe(hWnd, Msg, wParam, lParam); }
		[System.Security.SecuritySafeCritical]
		static void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo) { mouse_eventSafe(dwFlags, dx, dy, dwData, dwExtraInfo); }
		[System.Security.SecuritySafeCritical]
		static uint SendMouseInput(int nInputs, [MarshalAs(UnmanagedType.LPArray)] MouseInputArgs[] pInputs, int cbSize) { return SendMouseInputSafe(nInputs, pInputs, cbSize); }
		[System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SendMessage")]
		static extern int SendMessageSafe(IntPtr hWnd, int Msg, uint wParam, uint lParam);
		[DllImport("user32.dll", EntryPoint = "mouse_event")]
		extern static void mouse_eventSafe(uint dwFlags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo);
		[DllImport("user32.dll", EntryPoint = "SendInput")]
		extern static uint SendMouseInputSafe(int nInputs, [MarshalAs(UnmanagedType.LPArray)] MouseInputArgs[] pInputs, int cbSize);
		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		struct MouseInputArgs {
			public int Type;
			public long dx;
			public long dy;
			public uint mouseData;
			public uint dwFlags;
			public uint time;
			public IntPtr extraInfo;
			public MouseInputArgs(long dx, long dy, uint dwFlags) {
				this.Type = 0;
				this.dx = dx;
				this.dy = dy;
				this.mouseData = 0;
				this.dwFlags = dwFlags;
				this.time = 0;
				this.extraInfo = IntPtr.Zero;
			}
			public MouseInputArgs(long dx, long dy, uint dwFlags, uint mouseData)
				: this(dx, dy, dwFlags) {
				this.mouseData = mouseData;
			}
		}
		void SendMouseDown(Point screenPoint, MouseButtons button) {
			uint flags = MOUSEEVENTF_ABSOLUTE;
			switch(button) {
				case MouseButtons.Left: flags |= MOUSEEVENTF_LEFTDOWN; break;
				case MouseButtons.Right: flags |= MOUSEEVENTF_RIGHTDOWN; break;
				case MouseButtons.Middle: flags |= MOUSEEVENTF_MIDDLEDOWN; break;
			}
			mouse_event(flags, Convert.ToUInt32(screenPoint.X), Convert.ToUInt32(screenPoint.Y), 0, IntPtr.Zero);
		}
		void SendMouseDown2(Control control, Point point, MouseButtons button) {
			int msg = WM_LBUTTONDOWN;
			switch(button) {
				case MouseButtons.Right: msg = WM_RBUTTONDOWN; break;
				case MouseButtons.Middle: msg = WM_MBUTTONDOWN; break;
			}
			BaseEdit baseEdit = control as BaseEdit;
			IntPtr handle = control.Handle;
			if(baseEdit != null) {
				Control innerControl = baseEdit.InnerControl;
				if (innerControl != null && innerControl.Visible && innerControl.Bounds.Contains(point)) {
					if(innerControl.HasChildren) {
						var cc = innerControl.GetChildAtPoint(point);
						if(cc != null && cc.Visible) innerControl = cc;
					}
					handle = innerControl.Handle;
					point = innerControl.PointToClient(control.PointToScreen(point));
				}
			}
			SendMessage(handle, msg, 0, (uint)(point.X | (point.Y << 16)));
		}
		public override bool SendMouseDown(Control control, MouseEventArgs e) {
			SendMouseDown2(control, new Point(e.X, e.Y), e.Button);
			return true;
		}
	}
}
