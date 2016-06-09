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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Runtime.InteropServices;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors.Controls;
using DevExpress.Skins;
using System.Security;
namespace DevExpress.XtraEditors {
	public partial class ProgressWindow : XtraForm {
		private static readonly object cancel = new object();
		int savedClientHeight = -1;
		MarqueeProgressBarControl marquee;
		bool cancelPending = false;
		public ProgressWindow() {
			InitializeComponent();
			this.ControlBox = false;
			this.savedClientHeight = ClientSize.Height;
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			this.marquee = new MarqueeProgressBarControl();
			this.marquee.Visible = false;
			this.marquee.Bounds = reProgress.Bounds;
			Controls.Add(this.marquee);
			btCancel.Text = Localizer.Active.GetLocalizedString(StringId.ProgressCancel);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CancelPending { get { return cancelPending; } }
		public event EventHandler Cancel {
			add { Events.AddHandler(cancel, value); }
			remove { Events.RemoveHandler(cancel, value); }
		}
		public void SetCaption(PrintingSystemActivity activity) {
			string caption = "";
			if((activity & PrintingSystemActivity.Exporting) != 0) {
				caption = Localizer.Active.GetLocalizedString(StringId.ProgressExport);
			}
			if((activity & PrintingSystemActivity.Printing) != 0) {
				caption = Localizer.Active.GetLocalizedString(StringId.ProgressPrinting);
			}
			if(activity == PrintingSystemActivity.Preparing) {
				caption = Localizer.Active.GetLocalizedString(StringId.ProgressCreateDocument);
			}
			if(caption != String.Empty) 
				Text = caption;
		}
		Form parent;
		public void ShowCenter(Form parent) {
			this.parent = parent;
			Point offset = Point.Empty;
			if(parent.IsMdiChild && parent.MdiParent != null) {
				offset = parent.MdiParent.Location;
			}
			Point location = RectangleHelper.GetCenterBounds(parent.Bounds, Size).Location;
			location.Offset(offset);
			Location = location;
			Show(parent);
			if(savedClientHeight > ClientSize.Height) ClientSize = new Size(ClientSize.Width, savedClientHeight);
			Focus();
			Refresh();
		}
		int savedProgressWidth = -1;
		public void EnableCancel() {
			btCancel.Visible = true;
			if(savedProgressWidth > 0) {
				this.reProgress.Width = savedProgressWidth;
				if(marquee != null) this.marquee.Width = savedProgressWidth;
			}
		}
		public void DisableCancel() {
			if(this.savedProgressWidth < 0) savedProgressWidth = this.reProgress.Width;
			btCancel.Visible = false;
			this.reProgress.Width = btCancel.Right - reProgress.Left;
			if(marquee != null) {
				this.marquee.Width = reProgress.Width;
			}
		}
		protected void RaiseCancel() {
			EventHandler handler = (EventHandler)Events[cancel];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected override bool ShowWithoutActivation { get { return true; } }
		public virtual void SetMarqueProgress(string text) {
			if(!this.marquee.Visible) {
				this.marquee.Visible = true;
				this.reProgress.Visible = false;
				if(!string.IsNullOrEmpty(text)) Text = text;
			}
			Refresh();
			ProcessEvents();
		}
		public virtual void SetProgress(int progress) {
			if(this.marquee.Visible) {
				this.marquee.Visible = false;
				this.reProgress.Visible = true;
			}
			this.reProgress.Position = progress;
			Refresh();
			ProcessEvents();
		}
		public void ProcessEvents() {
			for(int n = 0; n < 10; n++) { if(!DoParentEvents()) break; }
			for(int n = 0; n < 10; n++) { if(!DoEvents()) break; }
		}
		protected override void OnClosing(CancelEventArgs e) {
			e.Cancel = true;
			base.OnClosing(e);
		}
		void btCancel_Click(object sender, EventArgs e) {
			btCancel.Text = Localizer.Active.GetLocalizedString(StringId.ProgressCancelPending);
			btCancel.Enabled = false;
			this.cancelPending = true;
			RaiseCancel();
			btCancel.Refresh();
		}
		bool DoParentEvents() {
			if(parent == null || !parent.IsHandleCreated) return false;
			return DoEventsCore(parent, parent.Handle, true);
		}
		public bool DoEvents() {
			if(!IsHandleCreated) return false;
			return DoEventsCore(this, Handle, false);
		}
		int? threadCallbackMessage;
		int ThreadCallbackMessage {
			get {
				if(threadCallbackMessage.HasValue) return threadCallbackMessage.Value;
				System.Reflection.FieldInfo fi = typeof(Control).GetField("threadCallbackMessage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
				if(fi != null) threadCallbackMessage = (int)fi.GetValue(null);
				return threadCallbackMessage.Value;
			}
		}
		System.Reflection.MethodInfo wndProcMethod;
		void InvokeWndProc(Control target, ref Message msg) {
			if(wndProcMethod == null) {
				wndProcMethod = typeof(Control).GetMethod("WndProc", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			}
			if(wndProcMethod != null) wndProcMethod.Invoke(target, new object[] { msg });
		}
		protected bool DoEventsCore(Control control, IntPtr handle, bool filterMessages) {
			if(!IsHandleCreated) return false;
			MSG msg = new MSG();
			if(PeekMessage(ref msg, handle, 0, 0, PeekMessageOption.PM_REMOVE)) {
				if(msg.message == ThreadCallbackMessage) {
					Message m = new Message() { Msg = msg.message, HWnd = msg.hwnd, WParam = msg.wParam, LParam = msg.lParam };
					if(!InvokeRequired) InvokeWndProc(control, ref m);
					return true;
				}
				if(filterMessages && !IsAllowMessage(ref msg)) return true;
				TranslateMessage(ref msg);
				DispatchMessage(ref msg);
				return true;
			}
			return false;
		}
		bool IsAllowMessage(ref MSG msg) {
			switch(msg.message) {
				case DevExpress.Utils.Drawing.Helpers.MSG.WM_NCHITTEST: return true;
			}
			return false;
		}
		const int SC_MOVE = 61456,
				  WM_NCLBUTTONDOWN = 161,
				  WM_SYSCOMMAND = 274,
				  HTCAPTION = 2;
		protected override void WndProc(ref Message msg) {
			if(msg.Msg == WM_SYSCOMMAND && msg.WParam.ToInt32() == SC_MOVE) return;
			if(msg.Msg == WM_NCLBUTTONDOWN && msg.WParam.ToInt32() == HTCAPTION) return;
			base.WndProc(ref msg);
		}
		[StructLayout(LayoutKind.Sequential)]
		struct MSG {
			public IntPtr hwnd;
			public int message;
			public IntPtr wParam;
			public IntPtr lParam;
			public int time;
			public int pt_x;
			public int pt_y;
			public override string ToString() {
				return string.Format("{0}: {1}", hwnd, message);
			}
		}
		enum PeekMessageOption { PM_NOREMOVE = 0, PM_REMOVE }
		[SecuritySafeCritical]
		static bool PeekMessage(ref MSG lpMsg, IntPtr hwnd, Int32 wMsgFilterMin, Int32 wMsgFilterMax, PeekMessageOption wRemoveMsg) {
			return PeekMessageCore(ref lpMsg, hwnd, wMsgFilterMin, wMsgFilterMax, wRemoveMsg);
		}
		[SecuritySafeCritical]
		static bool TranslateMessage(ref MSG lpMsg) {
			return TranslateMessageCore(ref lpMsg);
		}
		[SecuritySafeCritical]
		static Int32 DispatchMessage(ref MSG lpMsg) {
			return DispatchMessageCore(ref lpMsg);
		}
		[DllImport("user32.dll", SetLastError = true, EntryPoint = "PeekMessage")]
		static extern bool PeekMessageCore(ref MSG lpMsg, IntPtr hwnd, Int32 wMsgFilterMin, Int32 wMsgFilterMax, PeekMessageOption wRemoveMsg);
		[DllImport("user32.dll", SetLastError = true, EntryPoint = "TranslateMessage")]
		static extern bool TranslateMessageCore(ref MSG lpMsg);
		[DllImport("user32.dll", SetLastError = true, EntryPoint = "DispatchMessage")]
		static extern Int32 DispatchMessageCore(ref MSG lpMsg);
	}
}
