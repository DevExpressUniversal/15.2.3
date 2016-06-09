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
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using DevExpress.Utils.Extensions;
namespace DevExpress.Utils.Win.Hook {
	public interface IHookController {
		IntPtr OwnerHandle { get; }
		bool InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam);
		bool InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam);
	}
	public interface IHookController2 : IHookController {
		void WndGetMessage(ref Message msg);
	}
	public interface IMessageRedirector {
		bool AllowRedirect(int msg);
		Control MessageTarget { get; }
	}
	public enum HookResult { Unknown, NotProcessed, Processed, ProcessedExit }
	public interface IHookControllerWithResult : IHookController {
		HookResult Result { get; set; }
	}
	public delegate int Hook(int ncode, IntPtr wParam, IntPtr lParam);
	public class HookInfo {
		List<IHookController> hookControllers;
		int threadId;
		public bool inHook, inMouseHook;
		internal HookManager.CWPSTRUCT hookStr = new HookManager.CWPSTRUCT();
		internal HookManager.MOUSEHOOKSTRUCTEX hookStrEx = new HookManager.MOUSEHOOKSTRUCTEX();
		public IntPtr wndHookHandle, getMessageHookHandle, mouseHookHandle;
		public Hook wndHookProc, mouseHookProc, getMessageHookProc;
		HookManager manager;
		public HookInfo(HookManager manager) {
			this.manager = manager;
			this.inMouseHook = false;
			this.inHook = false;
			wndHookHandle = getMessageHookHandle = mouseHookHandle = IntPtr.Zero;
			wndHookProc = mouseHookProc = getMessageHookProc = null;
			this.threadId = HookManager.GetCurrentThreadId();
			this.hookControllers = new List<IHookController>();
		}
		public List<IHookController> HookControllers { get { return hookControllers; } }
		public int ThreadId { get { return threadId; } }
		public Point GetPoint() {
			if(hookStrEx != null) {
				HookManager.POINT pt = hookStrEx.Mouse.Pt;
				return new Point(pt.X, pt.Y);
			}
			return Point.Empty;
		}
	}
	[SecuritySafeCritical]
	public class HookManager {
		Dictionary<int, HookInfo> hookHash;
		public List<IHookController> HookControllers;
		static HookManager defaultManager = new HookManager();
		public static HookManager DefaultManager { get { return defaultManager; } }
		readonly int startupThreadID;
		public HookManager() {
			startupThreadID = GetCurrentThreadId();
			Application.ApplicationExit += OnApplicationExit;
			Application.ThreadExit += OnThreadExit;
			hookHash = new Dictionary<int, HookInfo>();
			HookControllers = new List<IHookController>();
		}
		~HookManager() {
			RemoveHooks();
			Application.ApplicationExit -= OnApplicationExit;
			Application.ThreadExit -= OnThreadExit;
		}
		public Dictionary<int, HookInfo> HookHash { get { return hookHash; } }
		public void CheckController(IHookController ctrl) {
			HookInfo hInfo = GetInfoByThread();
			if(hInfo.HookControllers.Contains(ctrl)) return;
			AddController(ctrl);
		}
		public void AddController(IHookController ctrl) {
			HookInfo hInfo = GetInfoByThread();
			hInfo.HookControllers.Add(ctrl);
			if(hInfo.HookControllers.Count == 1) InstallHook(hInfo);
		}
		public void RemoveController(IHookController ctrl) {
			HookInfo hInfo = GetInfoByThread();
			hInfo.HookControllers.Remove(ctrl);
			if(hInfo.HookControllers.Count == 0) RemoveHook(hInfo, false);
		}
		protected virtual HookInfo GetInfoByThread() {
			int thId = CurrentThread;
			HookInfo hInfo;
			lock(HookHash) {
				if(!HookHash.TryGetValue(thId, out hInfo)) {
					hInfo = new HookInfo(this);
					HookHash.Add(thId, hInfo);
				}
			}
			return hInfo;
		}
		public static int CurrentThread { get { return GetCurrentThreadId(); } }
		internal void InstallHook(HookInfo hInfo) {
			if(hInfo.wndHookHandle != IntPtr.Zero) return;
			hInfo.mouseHookProc = new Hook(MouseHook);
			hInfo.wndHookProc = new Hook(WndHook);
			hInfo.getMessageHookProc = new Hook(GetMessageHook);
			hInfo.wndHookHandle = SetWindowsHookEx(4, hInfo.wndHookProc, 0, hInfo.ThreadId); 
			hInfo.mouseHookHandle = SetWindowsHookEx(7, hInfo.mouseHookProc, 0, hInfo.ThreadId); 
			hInfo.getMessageHookHandle = IntPtr.Zero;
		}
		internal void RemoveHook(HookInfo hInfo, bool disposing) {
			lock(HookHash) {
				if(hInfo != null && hInfo.wndHookHandle != IntPtr.Zero) {
					UnhookWindowsHookEx(hInfo.wndHookHandle);
					hInfo.wndHookHandle = IntPtr.Zero;
					hInfo.wndHookProc = null;
					hInfo.getMessageHookHandle = IntPtr.Zero;
					hInfo.getMessageHookProc = null;
					UnhookWindowsHookEx(hInfo.mouseHookHandle);
					hInfo.mouseHookHandle = IntPtr.Zero;
					hInfo.mouseHookProc = null;
					HookHash.Remove(hInfo.ThreadId);
				}
			}
		}
		void OnThreadExit(object sender, EventArgs e) {
			RemoveHook(GetInfoByThread(), true);
		}
		void OnApplicationExit(object sender, EventArgs e) {
			Application.ThreadExit -= OnThreadExit;
			Application.ApplicationExit -= OnApplicationExit;
			if(GetCurrentThreadId() == startupThreadID)
				RemoveHooks();
		}
		protected virtual void RemoveHooks() {
			lock(HookHash) {
				List<HookInfo> list = new List<HookInfo>(HookHash.Values);
				HookHash.Clear();
				for(int n = 0; n < list.Count; n++) {
					RemoveHook(list[n], true);
				}
			}
		}
		public static int GetCurrentThreadId() { return GetCurrentThreadIdSafe(); }
		[SecuritySafeCritical]
		static int GetCurrentThreadIdSafe() { return GetCurrentThreadIdCore(); }
		[DllImport("kernel32.dll", ExactSpelling=true, CharSet=CharSet.Auto, EntryPoint="GetCurrentThreadId")]
		static extern int GetCurrentThreadIdCore();
		[StructLayout(LayoutKind.Sequential)]
		internal sealed class CWPSTRUCT { 
			public IntPtr lParam; 
			public IntPtr wParam; 
			public int	message; 
			public IntPtr	hwnd; 
		}
		[StructLayout(LayoutKind.Sequential)]
		internal struct CWPRETSTRUCT { 
			public IntPtr lResult;
			public IntPtr lParam; 
			public IntPtr wParam; 
			public int	message; 
			public IntPtr	hwnd; 
		}
		[StructLayout(LayoutKind.Sequential)]
		internal struct POINT { 
			public int X;
			public int Y;
		}
		[StructLayout(LayoutKind.Sequential)]
		internal struct MOUSEHOOKSTRUCT { 
			public POINT	 Pt; 
			public IntPtr	 hwnd; 
			public uint		 wHitTestCode; 
			public IntPtr	 dwExtraInfo; 
		}
		[StructLayout(LayoutKind.Sequential)]
		internal class MOUSEHOOKSTRUCTEX {
			public MOUSEHOOKSTRUCT Mouse;
			public int mouseData;
		}
		[StructLayout(LayoutKind.Sequential)]
		internal struct API_MSG {
			public IntPtr   Hwnd; 
			public int   Msg; 
			public IntPtr WParam; 
			public IntPtr LParam; 
			public int  Time; 
			public POINT  Pt; 
			public Message ToMessage() {
				System.Windows.Forms.Message res = new System.Windows.Forms.Message();
				res.HWnd = this.Hwnd;
				res.Msg = this.Msg;
				res.WParam = this.WParam;
				res.LParam = this.LParam;
				return res;
			}
			public void FromMessage(ref Message msg) {
				this.Hwnd = msg.HWnd;
				this.Msg = msg.Msg;
				this.WParam = msg.WParam;
				this.LParam = msg.LParam;
			}
		}
		protected int WndHook(int ncode, IntPtr wParam, IntPtr lParam) {
			HookInfo hInfo = GetInfoByThread();
			int res = 0;
			if(!hInfo.inHook && lParam != IntPtr.Zero) {
				CWPSTRUCT hookStr = hInfo.hookStr;
				Marshal.PtrToStructure(lParam, hookStr);
				Control ctrl = null;
				try {
					try {
						ctrl =  DevExpress.XtraEditors.Controls.HookPopup.GetControlFromHandle(hookStr.hwnd);
						hInfo.inHook = true;
						res = InternalPreFilterMessage(hInfo, hookStr.message, ctrl, hookStr.hwnd, hookStr.wParam, hookStr.lParam) ? 1 : 0;
					} finally {
						hInfo.inHook = false;
					}
					return CallNextHookEx(hInfo.wndHookHandle, ncode, wParam, lParam);
				} finally {
					InternalPostFilterMessage(hInfo, hookStr.message, ctrl, hookStr.hwnd, hookStr.wParam, hookStr.lParam);
				}
			} else
				return CallNextHookEx(hInfo.wndHookHandle, ncode, wParam, lParam);
		}
		protected int GetMessageHook(int ncode, IntPtr wParam, IntPtr lParam) {
			HookInfo hInfo = GetInfoByThread();
			if(!hInfo.inHook && lParam != IntPtr.Zero) {
				try {
					hInfo.inHook = true;
					API_MSG hookStr = (API_MSG)Marshal.PtrToStructure(lParam, typeof(API_MSG));
					InternalGetMessage(ref hookStr);
				} finally {
					hInfo.inHook = false;
				}
			} 
			return CallNextHookEx(hInfo.wndHookHandle, ncode, wParam, lParam); 
		}
		protected int MouseHook(int ncode, IntPtr wParam, IntPtr lParam) {
			HookInfo hInfo = GetInfoByThread();
			int res = 0;
			bool allowFutureProcess = true;
			if(ncode == 0) {
				if(!hInfo.inMouseHook && lParam != IntPtr.Zero) {
					try {
						MOUSEHOOKSTRUCTEX hookStrEx = hInfo.hookStrEx;
						Marshal.PtrToStructure(lParam, hookStrEx);
						MOUSEHOOKSTRUCT hookStr = hookStrEx.Mouse;
						Control ctrl = DevExpress.XtraEditors.Controls.HookPopup.GetControlFromHandle(hookStr.hwnd);
						hInfo.inMouseHook = true;
						allowFutureProcess = !InternalPreFilterMessage(hInfo, (int)wParam.ToInt64(), ctrl, hookStr.hwnd, new IntPtr(hookStrEx.mouseData), hookStr.Pt.LParamFromPoint());
					} finally {
						hInfo.inMouseHook = false;
					}
				} else return CallNextHookEx(hInfo.mouseHookHandle, ncode, wParam, lParam);
			}
			res = CallNextHookEx(hInfo.mouseHookHandle, ncode, wParam, lParam); 
			if(!allowFutureProcess) res = -1;
			return res;
		}
		[DllImport("USER32.dll", CharSet=CharSet.Auto)]
		protected static extern IntPtr SetWindowsHookEx(int idHook, Hook lpfn, int hMod,int dwThreadId);
		[DllImport("USER32.dll", CharSet=CharSet.Auto)]
		protected static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam,  IntPtr lParam);
		[DllImport("USER32.dll", CharSet=CharSet.Auto)]
		protected static extern bool UnhookWindowsHookEx(IntPtr hhk);
		internal bool InternalPreFilterMessage(HookInfo hInfo, int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			bool result = false;
			for(int n = hInfo.HookControllers.Count - 1; n >= 0; n--) {
				if(n >= hInfo.HookControllers.Count) continue;
				IHookController ctrl = hInfo.HookControllers[n];
				result |= ctrl.InternalPreFilterMessage(Msg, wnd, HWnd, WParam, LParam);
				IHookControllerWithResult ctrl3 = ctrl as IHookControllerWithResult;
				if(ctrl3 != null && ctrl3.Result == HookResult.ProcessedExit)
					break;
			}
			return result;
		}
		internal bool InternalPostFilterMessage(HookInfo hInfo, int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			bool result = false;
			for(int n = hInfo.HookControllers.Count - 1; n >=0 ; n --) {
				IHookController ctrl = hInfo.HookControllers[n];
				result |= ctrl.InternalPostFilterMessage(Msg, wnd, HWnd, WParam, LParam);
				if(Msg == 0x2) { 
					if(ctrl.OwnerHandle == HWnd) {
						RemoveController(ctrl);
					}
				}
				IHookControllerWithResult ctrl3 = ctrl as IHookControllerWithResult;
				if(ctrl3 != null && ctrl3.Result == HookResult.ProcessedExit)
					break;
			}
			return result;
		}
		internal void InternalGetMessage(ref API_MSG msg) {
			HookInfo hInfo = GetInfoByThread();
			for(int n = 0; n < hInfo.HookControllers.Count; n ++) {
				IHookController2 ctrl = hInfo.HookControllers[n] as IHookController2;
				if(ctrl != null) {
					Message m = msg.ToMessage();
					ctrl.WndGetMessage(ref m);
					msg.FromMessage(ref m);
				}
			}
		}
	}
	public delegate void MsgEventHandler(object sender, ref Message msg);
	public class ControlWndHookInfo {
		ControlWndHook hook;
		int refCount;
		public ControlWndHookInfo(ControlWndHook hook, MsgEventHandler handler, MsgEventHandler afterHandler) {
			this.refCount = 0;
			this.hook = hook;
			AddRef(handler, afterHandler);
		}
		public int RefCount { get { return refCount; } }
		public void AddRef(MsgEventHandler handler, MsgEventHandler afterHandler) {
			this.refCount ++; 
			if(Hook != null) {
				if(handler != null) Hook.WndMessage += handler;
				if(afterHandler != null) Hook.AfterWndMessage += afterHandler;
			}
		}
		public void Release(MsgEventHandler handler, MsgEventHandler afterHandler) {
			if(Hook != null) {
				if(handler != null) Hook.WndMessage -= handler;
				if(afterHandler != null) Hook.AfterWndMessage -= afterHandler;
			}
			if(-- this.refCount == 0) {
				if(this.hook != null) ReleaseCore();
			}
		}
		public ControlWndHook Hook { get { return hook; } }
		protected void ReleaseCore() {
			this.hook.Control = null;
			this.hook = null;
		}
	}
	public interface IPopupForm { }
	[SecuritySafeCritical]
	public class ControlWndHook {
		#region static
		static IDictionary<Control, ControlWndHookInfo> hooks = new Dictionary<Control, ControlWndHookInfo>(17);
		readonly static object syncObj = new object();
		public static void AddHook(Control ctrl, MsgEventHandler handler, MsgEventHandler afterHandler) {
			lock(syncObj) {
				ControlWndHookInfo info;
				if(!hooks.TryGetValue(ctrl, out info)) {
					ControlWndHook hook = new ControlWndHook();
					hook.Control = ctrl;
					info = new ControlWndHookInfo(hook, handler, afterHandler);
					hooks.Add(ctrl, info);
				}
				else info.AddRef(handler, afterHandler);
			}
		}
		public static void RemoveHook(Control ctrl, MsgEventHandler handler, MsgEventHandler afterHandler) {
			lock(syncObj) {
				ControlWndHookInfo info;
				if(hooks.TryGetValue(ctrl, out info)) {
					info.Release(handler, afterHandler);
					if(info.RefCount == 0)
						hooks.Remove(ctrl);
				}
			}
		}
		#endregion static
		public event MsgEventHandler WndMessage;
		public event MsgEventHandler AfterWndMessage;
		Control controlCore;
		public Control Control {
			get { return controlCore; }
			set {
				if(Control == value) return;
				if(Control != null)
					UnHook();
				this.controlCore = value;
				if(Control != null)
					Hook();
			}
		}
		public virtual void UnHook() {
			UnHook(true);
		}
		protected virtual void UnHook(bool unsubscribeEvents) {
			UnHookCore();
			if(unsubscribeEvents)
				UnsubscribeControlEvents();
		}
		public virtual void Hook() {
			UnsubscribeControlEvents();
			UnHookCore();
			HookCore();
			SubscribeControlEvents();
		}
		Drawing.Helpers.IWin32Subclasser subclasser;
		Drawing.Helpers.Win32SubclasserFactory.WndProc wndProc;
		protected virtual void HookCore() {
			if(Control == null || !Control.IsHandleCreated) return;
			this.wndProc = new Drawing.Helpers.Win32SubclasserFactory.WndProc(HookProc);
			this.subclasser = Drawing.Helpers.Win32SubclasserFactory.Create(Control.Handle, wndProc);
		}
		protected virtual void UnHookCore() {
			Ref.Dispose(ref subclasser);
			this.wndProc = null;
		}
		void SubscribeControlEvents() {
			if(Control != null) {
				Control.HandleDestroyed += OnControl_HandleDestroyed;
				Control.HandleCreated += OnControl_HandleCreated;
			}
		}
		void UnsubscribeControlEvents() {
			if(Control != null) {
				Control.HandleDestroyed -= OnControl_HandleDestroyed;
				Control.HandleCreated -= OnControl_HandleCreated;
			}
		}
		protected virtual void OnControl_HandleDestroyed(object sender, EventArgs e) {
			UnHook(false);
		}
		protected virtual void OnControl_HandleCreated(object sender, EventArgs e) {
			Hook();
		}
		bool HookProc(ref System.Windows.Forms.Message m) {
			if(IsSCKeyMenu(m) && HasPopupForm(Control)) {
				m.Result = IntPtr.Zero;
				return true;
			}
			if(WndMessage != null)
				WndMessage(this, ref m);
			if(AfterWndMessage != null)
				AfterWndMessage(this, ref m);
			return false;
		}
		const int SC_KEYMENU = 0xF100;
		static bool IsSCKeyMenu(Message m) {
			return (m.Msg == Drawing.Helpers.MSG.WM_SYSCOMMAND) && (GetInt(m.WParam) & 0xFFF0) == SC_KEYMENU;
		}
		static int GetInt(IntPtr ptr) {
			return IntPtr.Size == 8 ? unchecked((int)ptr.ToInt64()) : ptr.ToInt32();
		}
		static bool HasPopupForm(Control ctrl) {
			Form frm = ctrl as Form;
			if(frm == null)
				frm = ctrl.FindForm();
			if(frm == null)
				return false;
			for(int i = 0; i < frm.OwnedForms.Length; i++) {
				if(frm.OwnedForms[i] is IPopupForm)
					return true;
			}
			return false;
		}
	}
	public class FormHook {
	}
}
