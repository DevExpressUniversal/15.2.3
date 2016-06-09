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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DevExpress.XtraTabbedMdi;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraBars;
using DevExpress.Skins;
using DevExpress.XtraEditors;
namespace DevExpress.Utils.Mdi {
	public interface IMdiClientSubclasser : IDisposable {
		MdiClient ClientWindow { get; }
		void ProcessNC();
	}
	public interface IMdiClientSubclasserOwner {
		void OnSetNextMdiChild(SetNextMdiChildEventArgs e);
		void OnContextMenu();
		bool AllowMdiLayout { get;}
		bool AllowMdiSystemMenu { get;}
		Rectangle CalculateNC(Rectangle bounds);
		void InvalidateNC();
		void DrawNC(DXPaintEventArgs e);
		void Paint(Graphics g);
		void EraseBackground(Graphics g);
		void HandleCreated();
		void HandleDestroyed();
	}
	[ToolboxItem(false)]
	public class MdiClientSubclasser : NativeWindow, IMdiClientSubclasser {
		MdiClient clientWindowCore;
		IMdiClientSubclasserOwner ownerCore;
		public MdiClient ClientWindow {
			get { return clientWindowCore; }
		}
		public IMdiClientSubclasserOwner Owner {
			get { return ownerCore; }
		}
		public MdiClientSubclasser(MdiClient mdiClient, IMdiClientSubclasserOwner owner) {
			this.ownerCore = owner;
			this.clientWindowCore = mdiClient;
			ClientWindow.HandleCreated += OnHandleCreated;
			ClientWindow.HandleDestroyed += OnHandleDestroyed;
			if(ClientWindow.IsHandleCreated)
				this.AssignHandle(ClientWindow.Handle);
			RegisterInGlobalList();
		}
		public void Dispose() {
			UnregisterFromGlobalList();
			if(ClientWindow != null) {
				ClientWindow.HandleCreated -= OnHandleCreated;
				ClientWindow.HandleDestroyed -= OnHandleDestroyed;
				clientWindowCore = null;
			}
			if(Handle != IntPtr.Zero)
				ReleaseHandle();
			ownerCore = null;
		}
		protected virtual bool IsShowSystemMenuCore {
			get { return Owner.AllowMdiSystemMenu; }
		}
		void OnHandleCreated(object sender, EventArgs e) {
			AssignHandle(((MdiClient)sender).Handle);
			if(Owner != null)
				Owner.HandleCreated();
		}
		void OnHandleDestroyed(object sender, EventArgs e) {
			if(Owner != null) 
				Owner.HandleDestroyed();
			ReleaseHandle();
		}
		protected override void OnHandleChange() {
			base.OnHandleChange();
			ProcessNC();
		}
		public static void ProcessNC(IntPtr handle) {
			BarNativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0,
				WinAPI.SWP_FRAMECHANGED | WinAPI.SWP_NOACTIVATE | WinAPI.SWP_NOCOPYBITS | 
				WinAPI.SWP_NOMOVE | WinAPI.SWP_NOOWNERZORDER | WinAPI.SWP_NOSIZE | WinAPI.SWP_NOZORDER);
		}
		public virtual void ProcessNC() {
			ProcessNC(Handle);
		}
		#region WNDPROC
		internal int Ignore_WM_MDINEXT = 0;
		protected override void WndProc(ref Message m) {
			switch(m.Msg) {
				default:
					DoBaseWndProc(ref m);
					break;
				case WinAPI.WM_MDIRESTORE:
				case WinAPI.WM_MDIMAXIMIZE:
				case WinAPI.WM_MDITILE:
				case WinAPI.WM_MDICASCADE:
				case WinAPI.WM_MDIICONARRANGE:
					DoMdiLayout(ref m);
					break;
				case MSG.WM_CONTEXTMENU:
					DoContextMenu(ref m);
					break;
				case MSG.WM_NCCALCSIZE:
					DoNCCalcSize(ref m);
					break;
				case MSG.WM_NCHITTEST:
					DoNCHitTest(ref m);
					break;
				case MSG.WM_NCPAINT:
					DoNCPaint(ref m);
					break;
				case MSG.WM_NCACTIVATE:
					DoNCActivate(ref m);
					break;
				case WinAPI.WM_MDINEXT:
					DoMdiNext(ref m);
					break;
				case WinAPI.WM_PARENTNOTIFY:
					DoParentNotify(ref m);
					break;
				case MSG.WM_SIZE:
					DoResize(ref m);
					break;
				case MSG.WM_ERASEBKGND:
					DoEraseBackground(ref m);
					break;
				case MSG.WM_PAINT:
					DoPaint(ref m);
					break;
				case MSG.WM_PRINT:
					DoPrint(ref m);
					break;
				case MSG.WM_VSCROLL:
					DoScroll(ref m, true);
					break;
				case MSG.WM_HSCROLL:
					DoScroll(ref m, false);
					break;
			}
			XtraBars.CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		protected void DoBaseWndProc(ref Message m) {
			base.WndProc(ref m);
		}
		protected virtual void DoNCCalcSize(ref Message m) {
			if(m.WParam == IntPtr.Zero) {
				WinAPI.RECT nccsRect = (WinAPI.RECT)m.GetLParam(typeof(WinAPI.RECT));
				Rectangle patchedRectangle = Owner.CalculateNC(nccsRect.ToRectangle());
				nccsRect.RestoreFromRectangle(patchedRectangle);
				BarNativeMethods.StructureToPtr(nccsRect, m.LParam, false);
				m.Result = IntPtr.Zero;
			}
			else {
				WinAPI.NCCALCSIZE_PARAMS nccsParams = (WinAPI.NCCALCSIZE_PARAMS)m.GetLParam(typeof(WinAPI.NCCALCSIZE_PARAMS));
				Rectangle bounds = nccsParams.rgrcProposed.ToRectangle();
				Rectangle patchedRectangle = Owner.CalculateNC(bounds);
				if(IsRightToLeftLayout) {
					patchedRectangle.X = bounds.X + bounds.Right - patchedRectangle.Right;
				}
				nccsParams.rgrcProposed.RestoreFromRectangle(patchedRectangle);
				BarNativeMethods.StructureToPtr(nccsParams, m.LParam, false);
				m.Result = IntPtr.Zero;
			}
		}
		protected virtual void DoMdiLayout(ref Message m) {
			if(Owner.AllowMdiLayout)
				base.WndProc(ref m);
			else m.Result = IntPtr.Zero;
		}
		protected virtual void DoNCPaint(ref Message m) {
			Owner.InvalidateNC();
			m.Result = IntPtr.Zero;
		}
		protected virtual void DoNCHitTest(ref Message m) {
			m.Result = new IntPtr(1);
		}
		protected virtual void DoNCActivate(ref Message m) {
			m.Result = new IntPtr(1);
		}
		protected virtual void DoMdiNext(ref Message m) {
			SetNextMdiChildEventArgs e = new SetNextMdiChildEventArgs(m);
			if(Ignore_WM_MDINEXT == 0)
				Owner.OnSetNextMdiChild(e);
			if(e.Handled)
				m.Result = IntPtr.Zero;
			else base.WndProc(ref m);
		}
		protected virtual void DoContextMenu(ref Message m) {
			Owner.OnContextMenu();
		}
		protected virtual void DoResize(ref Message m) {
			base.WndProc(ref m);
		}
		protected virtual void DoEraseBackground(ref Message m) {
			base.WndProc(ref m);
		}
		protected virtual void DoPaint(ref Message m) {
			base.WndProc(ref m);
		}
		protected virtual void DoPrint(ref Message m) {
			base.WndProc(ref m);
		}
		protected virtual void DoParentNotify(ref Message m) {
			base.WndProc(ref m);
		}
		protected virtual void DoScroll(ref Message m, bool vScroll) {
			m.Result = IntPtr.Zero;
		}
		#endregion WNDPROC
		#region GloabalRegistration
		static readonly IDictionary GlobalList = new Hashtable();
		void RegisterInGlobalList() {
			MdiClientSubclasserService.Register(ClientWindow, this);
		}
		void UnregisterFromGlobalList() {
			MdiClientSubclasserService.Unregister(ClientWindow);
		}
		#endregion GloabalRegistration
		public static MdiClientSubclasser FromMdiClient(MdiClient client) {
			return MdiClientSubclasserService.FromMdiClient(client) as MdiClientSubclasser;
		}
		public static MdiClient GetMdiClient(Form mdiParent) {
			return MdiClientSubclasserService.GetMdiClient(mdiParent);
		}
		public static bool IsShowSystemMenu(Form mdiParent) {
			MdiClient client = GetMdiClient(mdiParent);
			if(client != null) {
				MdiClientSubclasser subclasser = FromMdiClient(client);
				if(subclasser != null)
					return subclasser.IsShowSystemMenuCore;
			}
			return true;
		}
		bool isInAsyncDraw = false;
		public void AsyncDrawNC(Rectangle bounds) {
			if(bounds.IsEmpty) return;
			isInAsyncDraw = true;
			ClientWindow.BeginInvoke(new Action<Rectangle>(DrawNCCore), bounds);
		}
		public void DrawNC(Rectangle bounds) {
			IntPtr hDC = BarNativeMethods.GetWindowDC(ClientWindow.Handle);
			try {
				using(Graphics g = Graphics.FromHdc(hDC)) {
					SkinElementPainter.CorrectByRTL = IsRightToLeftLayout;
					Owner.DrawNC(new DevExpress.Utils.Drawing.DXPaintEventArgs(g, bounds));
					SkinElementPainter.CorrectByRTL = false;
				}
			}
			finally { BarNativeMethods.ReleaseDC(ClientWindow.Handle, hDC); }
		}
		bool IsRightToLeftLayout { 
			get { 
				Form form = ClientWindow.FindForm();
				return form == null ? false : form.RightToLeftLayout;
			} 
		}
		void DrawNCCore(Rectangle bounds) {
			if(isInAsyncDraw) {
				DrawNC(bounds);
				isInAsyncDraw = false;
			}
		}
	}
	internal static class WinAPI {
		#region API
		internal const int WM_PARENTNOTIFY = 0x0210;
		internal const int WM_MDICREATE = 0x0220;
		internal const int WM_MDIDESTROY = 0x0221;
		internal const int WM_MDIACTIVATE = 0x0222;
		internal const int WM_MDIRESTORE = 0x0223;
		internal const int WM_MDINEXT = 0x0224;
		internal const int WM_MDIMAXIMIZE = 0x0225;
		internal const int WM_MDITILE = 0x0226;
		internal const int WM_MDICASCADE = 0x0227;
		internal const int WM_MDIICONARRANGE = 0x0228;
		internal const int WM_MDIGETACTIVE = 0x0229;
		[StructLayout(LayoutKind.Sequential)]
		internal struct RECT {
			public int left;
			public int top;
			public int right;
			public int bottom;
			public Rectangle ToRectangle() {
				return Rectangle.FromLTRB(left, top, right, bottom);
			}
			public void RestoreFromRectangle(Rectangle original) {
				this.left = original.Left;
				this.right = original.Right;
				this.top = original.Top;
				this.bottom = original.Bottom;
			}
		}
		[StructLayout(LayoutKind.Sequential)]
		internal struct WINDOWPOS {
			public IntPtr hwnd;
			public IntPtr hwndInsertAfter;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public int flags;
			public Rectangle ToRectangle() {
				return new Rectangle(x, y, cx, cy);
			}
			public void RestoreFromRectangle(Rectangle original) {
				this.x = original.X;
				this.y = original.Y;
				this.cx = original.Width;
				this.cy = original.Height;
			}
		}
		[StructLayout(LayoutKind.Sequential)]
		internal struct NCCALCSIZE_PARAMS {
			public RECT rgrcProposed;
			public RECT rgrcWindowBefore;
			public RECT rgrcClientBefore;
			public WINDOWPOS lppos;
		}
		internal const int SWP_NOSIZE = 0x0001;
		internal const int SWP_NOMOVE = 0x0002;
		internal const int SWP_NOZORDER = 0x0004;
		internal const int SWP_NOREDRAW = 0x0008;
		internal const int SWP_NOACTIVATE = 0x0010;
		internal const int SWP_FRAMECHANGED = 0x0020;  
		internal const int SWP_SHOWWINDOW = 0x0040;
		internal const int SWP_HIDEWINDOW = 0x0080;
		internal const int SWP_NOCOPYBITS = 0x0100;
		internal const int SWP_NOOWNERZORDER = 0x0200;  
		internal const int SWP_NOSENDCHANGING = 0x0400;  
		[DllImport("user32.dll")]
		internal static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, int flags);
		internal const int DCX_WINDOW = 0x00000001;
		internal const int DCX_CACHE = 0x00000002;
		internal const int DCX_NORESETATTRS = 0x00000004;
		internal const int DCX_CLIPCHILDREN = 0x00000008;
		internal const int DCX_CLIPSIBLINGS = 0x00000010;
		internal const int DCX_PARENTCLIP = 0x00000020;
		internal const int DCX_EXCLUDERGN = 0x00000040;
		internal const int DCX_INTERSECTRGN = 0x00000080;
		internal const int DCX_EXCLUDEUPDATE = 0x00000100;
		internal const int DCX_INTERSECTUPDATE = 0x00000200;
		internal const int DCX_LOCKWINDOWUPDATE = 0x00000400;
		internal const int DCX_VALIDATE = 0x00200000;
		internal const int WVR_ALIGNTOP = 0x0010;
		internal const int WVR_ALIGNLEFT = 0x0020;
		internal const int WVR_ALIGNBOTTOM = 0x0040;
		internal const int WVR_ALIGNRIGHT = 0x0080;
		internal const int WVR_HREDRAW = 0x0100;
		internal const int WVR_VREDRAW = 0x0200;
		internal const int WVR_REDRAW = WVR_HREDRAW | WVR_VREDRAW;
		internal const int WVR_VALIDRECTS = 0x0400;
		#endregion
	}
}
