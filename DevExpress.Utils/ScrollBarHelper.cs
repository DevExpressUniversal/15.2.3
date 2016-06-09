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
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.WXPaint;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraEditors.ScrollHelpers {
	using System.Runtime.InteropServices;
	using DevExpress.XtraEditors;
	using System.Security;
	[StructLayout(LayoutKind.Sequential)]
	internal struct SCROLLBARINFO {
		public const uint 
			STATE_SYSTEM_INVISIBLE = 0x00008000,
			STATE_SYSTEM_OFFSCREEN = 0x00010000,
			STATE_SYSTEM_UNAVAILABLE = 0x00000001,
			OBJID_VSCROLL = 0xFFFFFFFB,
			OBJID_HSCROLL = 0xFFFFFFFA;
		const int CCHILDREN_SCROLLBAR = 5;
		public Int32 cbSize;
		public GDIRect rcScrollBar;
		public Int32  dxyLineButton;
		public Int32   xyThumbTop;
		public Int32   xyThumbBottom;
		public Int32   reserved;
		public Int32 rgstate0,rgstate1,rgstate2,rgstate3,rgstate4,rgstate5;
		public void Init() {
			this.rcScrollBar = new GDIRect();
			this.cbSize = Marshal.SizeOf(this);
			this.dxyLineButton = this.xyThumbTop = this.xyThumbBottom = this.reserved = 0;
			this.rgstate0 = this.rgstate1 = this.rgstate2 = this.rgstate3 = this.rgstate4 = this.rgstate5 = 0;
		}
		[SecuritySafeCritical]
		internal static bool GetScrollBarInfo(IntPtr hwnd, uint idObject, ref SCROLLBARINFO psbi) {
			return GetScrollBarInfoCore(hwnd, idObject, ref psbi);
		}
		[System.Runtime.InteropServices.DllImport("USER32.dll", EntryPoint = "GetScrollBarInfo")]
		internal static extern bool GetScrollBarInfoCore(IntPtr hwnd, uint idObject, ref SCROLLBARINFO psbi);
	}
	[StructLayout(LayoutKind.Sequential)]
	internal struct SCROLLINFO { 
		public const int 
			SIF_RANGE = 0x0001,
			SIF_PAGE = 0x0002,
			SIF_POS = 0x0004,
			SIF_DISABLENOSCROLL = 0x0008,
			SIF_TRACKPOS = 0x0010,
			SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);
		public const int SB_HORZ = 0, SB_VERT = 1;
		public Int32 cbSize, fMask, nMin, nMax, nPage, nPos, nTrackPos;
		public void Init() {
			this.cbSize = Marshal.SizeOf(this);
			this.nMin = this.nMax = this.nPage = this.nPos = this.nTrackPos = 0;
			this.fMask = SIF_ALL;
		}
		[SecuritySafeCritical]
		internal static int SetScrollInfo(IntPtr handle, int fnBar, ref SCROLLINFO scrollInfo, bool redraw) {
			return SetScrollInfoCore(handle, fnBar, ref scrollInfo, redraw);
		}
		[System.Runtime.InteropServices.DllImport("USER32.dll", EntryPoint = "SetScrollInfo")]
		internal static extern int SetScrollInfoCore(IntPtr handle, int fnBar, ref SCROLLINFO scrollInfo, bool redraw);
		[SecuritySafeCritical]
		internal static bool GetScrollInfo(IntPtr handle, int fnBar, ref SCROLLINFO scrollInfo) {
			return GetScrollInfoCore(handle, fnBar, ref scrollInfo);
		}
		[System.Runtime.InteropServices.DllImport("USER32.dll", EntryPoint = "GetScrollInfo")]
		internal static extern bool GetScrollInfoCore(IntPtr handle, int fnBar, ref SCROLLINFO scrollInfo);
	}
	[StructLayout(LayoutKind.Sequential)]
	internal struct GDIRect {
		public int left, top, right, bottom;
		public GDIRect(int l, int t, int r, int b) {
			left = l; top = t; right = r; bottom = b;
		}
		public GDIRect(Rectangle r) {
			left = r.Left; top = r.Top; right = r.Right; bottom = r.Bottom;
		}
		public Rectangle ToRectangle() {
			return new Rectangle(left, top, right - left, bottom - top);
		}
	}
	public class ScrollBarAPIHelper : IDisposable {
		int lockUpdate = 0;
		HScrollBar hscroll;
		VScrollBar vscroll;
		Control sourceControl, parentControl, corner;
		UserLookAndFeel lookAndFeel;
		public event EventHandler ScrollMouseLeave;
		Control hScrollGhost, vScrollGhost;
		public ScrollBarAPIHelper() {
			this.lookAndFeel = null;
			this.corner = new Control() { TabStop = false };
			this.hscroll = CreateHScrollBarInstance();
			this.vscroll = CreateVScrollBarInstance();
			if(ScrollBarBase.GetUIMode(ScrollUIMode.Default) == ScrollUIMode.Touch) {
				hScrollGhost = new Control() { TabStop = false };
				vScrollGhost = new Control() { TabStop = false };
				ScrollBarBase.ApplyUIMode(hscroll);
				ScrollBarBase.ApplyUIMode(vscroll);
			}
			this.VScroll.Scroll += new ScrollEventHandler(OnVScroll_Scroll);
			this.HScroll.Scroll += new ScrollEventHandler(OnHScroll_Scroll);
			this.VScroll.MouseLeave += new EventHandler(OnScroll_Leave);
			this.HScroll.MouseLeave += new EventHandler(OnScroll_Leave);
			this.sourceControl = null;
			this.LookAndFeel = null;
			this.parentControl = null;
		}
		protected virtual HScrollBar CreateHScrollBarInstance() {
			return new DevExpress.XtraEditors.HScrollBar();
		}
		protected virtual VScrollBar CreateVScrollBarInstance() {
			return new DevExpress.XtraEditors.VScrollBar();
		}
		public virtual UserLookAndFeel LookAndFeel {
			get { return lookAndFeel; }
			set {
				if(LookAndFeel == value) return;
				if(lookAndFeel != null) lookAndFeel.StyleChanged -= new EventHandler(OnControl_LookAndFeelChanged);
				this.lookAndFeel = value;
				if(lookAndFeel != null) {
					lookAndFeel.StyleChanged += new EventHandler(OnControl_LookAndFeelChanged);
					OnControl_LookAndFeelChanged(this, EventArgs.Empty);
				}
			}
		}
		protected virtual void BeginUpdate() {
			lockUpdate ++;
		}
		protected virtual void EndUpdate() {
			-- lockUpdate;
		}
		protected virtual bool IsLockUpdate { get { return lockUpdate != 0; } }
		public virtual void Dispose() {
			if(HScroll != null) {
				this.hscroll.Dispose();
				this.hscroll = null;
				this.vscroll.Dispose();
				this.vscroll = null;
				this.corner.Dispose();
				this.corner = null;
			}
			if(SourceControl != null) {
				SourceControl.VisibleChanged -= new EventHandler(OnControl_VisibleChanged);
				SourceControl.HandleCreated -= new EventHandler(OnControl_HandleCreated);
				SourceControl.SizeChanged -= new EventHandler(OnControl_SizeChanged);
				if(IsTextBoxMaskBox(SourceControl) && this.parentControl != null) {
					this.parentControl.TextChanged -= new EventHandler(OnControl_TextChanged);
					this.parentControl.SizeChanged -= new EventHandler(OnControlParent_SizeChanged);
				} else
					SourceControl.TextChanged -= new EventHandler(OnControl_TextChanged);
				this.sourceControl = null;
			}
		}
		public Control Corner { get { return corner; } }
		public HScrollBar HScroll { get { return hscroll; } }
		public VScrollBar VScroll { get { return vscroll; } }
		protected virtual void OnScroll_Leave(object sender, EventArgs e) {
			if(ScrollMouseLeave != null) ScrollMouseLeave(this, e);
		}
		protected virtual void OnControl_LookAndFeelChanged(object sender, EventArgs e) {
			if(VScroll == null || LookAndFeel == null) return;
			VScroll.LookAndFeel.Assign(LookAndFeel);
			HScroll.LookAndFeel.Assign(LookAndFeel);
		}
		protected virtual bool IsTextBoxMaskBox(Control control) {
			 return false;
		}
		public virtual void Init(Control control, Control parent) {
			this.sourceControl = control;
			this.parentControl = parent;
			if(SourceControl == null) return;
			Corner.TabStop = HScroll.TabStop = VScroll.TabStop = false;
			Corner.Visible = VScroll.Visible = HScroll.Visible = false;
			parent.Controls.AddRange(new Control[] {HScroll, VScroll, Corner});
			if(hScrollGhost != null) parent.Controls.Add(hScrollGhost);
			if(vScrollGhost != null) parent.Controls.Add(vScrollGhost);
			SourceControl.HandleCreated += new EventHandler(OnControl_HandleCreated);
			if(IsTextBoxMaskBox(SourceControl) && parent != null) {
				parent.TextChanged += new EventHandler(OnControl_TextChanged);
				parent.SizeChanged += new EventHandler(OnControlParent_SizeChanged);
			} else
			SourceControl.TextChanged += new EventHandler(OnControl_TextChanged);
			SourceControl.VisibleChanged += new EventHandler(OnControl_VisibleChanged);
			SourceControl.SizeChanged += new EventHandler(OnControl_SizeChanged);
			SourceControl.MouseMove += OnControl_MouseMove;
			HScroll.BringToFront();
			VScroll.BringToFront();
			Corner.BringToFront();
			if(SourceControl.IsHandleCreated) UpdateScrollBars();
		}
		void OnControl_MouseMove(object sender, MouseEventArgs e) {
			VScroll.OnAction(ScrollNotifyAction.MouseMove);
			HScroll.OnAction(ScrollNotifyAction.MouseMove);
		}
		protected Control SourceControl { get { return sourceControl; } }
		protected virtual void OnControl_HandleCreated(object sender, EventArgs e) {
			HScroll.BringToFront();
			VScroll.BringToFront();
			if(hScrollGhost != null) hScrollGhost.BringToFront();
			if(vScrollGhost != null) vScrollGhost.BringToFront();
			Corner.BringToFront();
			UpdateScrollBars();
		}
		protected virtual void OnControl_VisibleChanged(object sender, EventArgs e) {
			if(SourceControl != null && SourceControl.Visible) 
				UpdateScrollBars();
		}
		protected virtual void OnControlParent_SizeChanged(object sender, EventArgs e) {
			if(SourceControl != null && !SourceControl.Visible) UpdateScrollBars();
		}
		protected virtual void OnControl_SizeChanged(object sender, EventArgs e) {
			UpdateScrollBars();
		}
		protected virtual void OnControl_TextChanged(object sender, EventArgs e) {
			UpdateScrollBars();
		}
		protected virtual void OnVScroll_Scroll(object sender, ScrollEventArgs e) {
			UpdateOriginalScroll(e, false);
		}
		protected virtual void OnHScroll_Scroll(object sender, ScrollEventArgs e) {
			UpdateOriginalScroll(e, true);
		}
		public virtual void UpdateScrollBars() {
			UpdateDXScrollBar(true);
			UpdateDXScrollBar(false);
			Rectangle corner = Rectangle.Empty;
			if(HScroll.ActualVisible && VScroll.ActualVisible) {
				corner = HScroll.Bounds;
				corner.X = VScroll.Bounds.X;
				corner.Width = VScroll.Bounds.Width;
				Corner.Bounds = corner;
				if(vScrollGhost != null) Corner.BackColor = SourceControl.BackColor;
				Corner.Visible = true;
			} else {
				Corner.Visible = false;
			}
		}
		protected virtual void UpdateOriginalScroll(ScrollEventArgs e, bool isHorz) {
			if(IsLockUpdate) return;
			if(SourceControl == null || !SourceControl.IsHandleCreated) return;
			BeginUpdate();
			try {
				ScrollBarBase dxScroll = isHorz ? (ScrollBarBase)HScroll : (ScrollBarBase)VScroll;
				SCROLLINFO sInfo = new SCROLLINFO();
				sInfo.Init();
				sInfo.fMask = SCROLLINFO.SIF_POS;
				sInfo.nPos = dxScroll.Value;
				SCROLLINFO.SetScrollInfo(SourceControl.Handle, isHorz ? SCROLLINFO.SB_HORZ : SCROLLINFO.SB_VERT, ref sInfo, true);			
				const int SB_LINEUP = 0, SB_LINEDOWN = 1, SB_PAGEUP = 2, SB_PAGEDOWN = 3, 
						  SB_THUMBPOSITION = 4, SB_THUMBTRACK = 5, SB_TOP = 6, SB_BOTTOM = 7, SB_ENDSCROLL = 8;
				int wparam = SB_THUMBPOSITION | ((dxScroll.Value & 0xffff) << 16);
				switch(e.Type) {
					case ScrollEventType.First: wparam = SB_TOP; break;
					case ScrollEventType.Last : wparam = SB_BOTTOM; break;
					case ScrollEventType.LargeDecrement : wparam = SB_PAGEUP; break;
					case ScrollEventType.LargeIncrement: wparam = SB_PAGEDOWN; break;
					case ScrollEventType.SmallIncrement : wparam = SB_LINEDOWN; break;
					case ScrollEventType.SmallDecrement : wparam = SB_LINEUP; break;
					case ScrollEventType.EndScroll : wparam = SB_ENDSCROLL; break;
					case ScrollEventType.ThumbTrack : wparam = SB_THUMBTRACK | ((dxScroll.Value & 0xffff) << 16); break;
				}
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(SourceControl.Handle, isHorz ? WM_HSCROLL : WM_VSCROLL, wparam, IntPtr.Zero);
			}
			finally {
				EndUpdate();
			}
			UpdateDXScrollBar(isHorz);
		}
		public bool IsNativeVisible(bool isHorz) {
			if(SourceControl == null || !SourceControl.IsHandleCreated || !SourceControl.Visible) return false;
			SCROLLBARINFO sbInfo = new SCROLLBARINFO();
			sbInfo.Init();
			SCROLLBARINFO.GetScrollBarInfo(SourceControl.Handle, isHorz ? SCROLLBARINFO.OBJID_HSCROLL : SCROLLBARINFO.OBJID_VSCROLL, ref sbInfo);
			Rectangle scrollBounds = sbInfo.rcScrollBar.ToRectangle();
			if(scrollBounds.IsEmpty || sbInfo.rgstate0 == SCROLLBARINFO.STATE_SYSTEM_INVISIBLE || sbInfo.rgstate0 == SCROLLBARINFO.STATE_SYSTEM_OFFSCREEN) {
				return false;
			}
			return true;
		}
		protected virtual void UpdateDXScrollBar(bool isHorz) {
			if(IsLockUpdate) return;
			ScrollTouchBase dxScroll = isHorz ? (ScrollTouchBase)HScroll : (ScrollTouchBase)VScroll;
			Control ghost = isHorz ? hScrollGhost : vScrollGhost;
			if(SourceControl == null) return;
			if(!SourceControl.IsHandleCreated || !dxScroll.Parent.IsHandleCreated) {
				dxScroll.SetVisibility(false);
				if(ghost != null) ghost.Visible = false;
				return;
			}
			BeginUpdate();
			try {
				SCROLLBARINFO sbInfo = new SCROLLBARINFO();
				sbInfo.Init();
				SCROLLBARINFO.GetScrollBarInfo(SourceControl.Handle, isHorz ? SCROLLBARINFO.OBJID_HSCROLL : SCROLLBARINFO.OBJID_VSCROLL, ref sbInfo);
				Rectangle scrollBounds = sbInfo.rcScrollBar.ToRectangle();
				if((SourceControl != null && !SourceControl.Visible) || scrollBounds.IsEmpty || sbInfo.rgstate0 == SCROLLBARINFO.STATE_SYSTEM_INVISIBLE || sbInfo.rgstate0 == SCROLLBARINFO.STATE_SYSTEM_OFFSCREEN) {
					dxScroll.SetVisibility(false);
					if(ghost != null) ghost.Visible = false;
					return;
				}
				scrollBounds = dxScroll.Parent.RectangleToClient(scrollBounds);
				dxScroll.Bounds = scrollBounds;
				if(ghost != null) ghost.Bounds = scrollBounds;
				ScrollArgs currentArgs = new ScrollArgs(dxScroll), args = new ScrollArgs();
				if(sbInfo.rgstate0 == SCROLLBARINFO.STATE_SYSTEM_UNAVAILABLE) {
					args.Maximum = args.Minimum = 0;
					args.Value = 0;
					args.Enabled = false;
				} else {
					SCROLLINFO sInfo = new SCROLLINFO();
					sInfo.Init();
					SCROLLINFO.GetScrollInfo(SourceControl.Handle, isHorz ? SCROLLINFO.SB_HORZ : SCROLLINFO.SB_VERT, ref sInfo);
					args.Enabled = true;
					args.Maximum = sInfo.nMax;
					args.Minimum = sInfo.nMin;
					args.LargeChange = sInfo.nPage;
					args.SmallChange = isHorz ? 8 : 1;
					args.Value = sInfo.nTrackPos;
				}
				dxScroll.SetVisibility(true);
				if(ghost != null) {
					ghost.BackColor = SourceControl.BackColor;
					ghost.Visible = true;
				}
				if(currentArgs.IsEquals(args)) return;
				args.AssignTo(dxScroll);
			}
			finally {
				EndUpdate();
			}
		}
		const int WM_HSCROLL = 0x0114, WM_VSCROLL = 0x0115;
		public virtual void WndProc(ref Message msg) {
			WndProcCore(ref msg);
		}
		[SecuritySafeCritical]
		void WndProcCore(ref Message msg) {
			if(IsLockUpdate) return;
			const int WM_SETTEXT = 0x000C, WM_ENABLE = 0x000A, WM_CHAR = 0x0102,
					  WM_NCCALCSIZE = 0x83, WM_WINDOWPOSCHANGED = 0x47, EM_SCROLLCARET = 0x00B7, WM_MOUSEWHEEL = 0x020A,
					  WM_SHOWWINDOW = 0x18, WM_WININICHANGE = 0x001A, WM_KEYDOWN = 0x100, WM_SYSKEYDOWN = 0x104;
			if(SourceControl == null || !SourceControl.IsHandleCreated) return;
			if(msg.HWnd != SourceControl.Handle) return;
			switch(msg.Msg) {
				case WM_SETTEXT:
				case WM_CHAR:
				case WM_ENABLE:
					break;
				case WM_SYSKEYDOWN:
				case WM_KEYDOWN:
				case WM_WININICHANGE:
				case WM_SHOWWINDOW:
				case WM_MOUSEWHEEL:
				case 0x118:
				case 0x2111: 
				case WM_NCCALCSIZE:
				case WM_WINDOWPOSCHANGED:
				case EM_SCROLLCARET:
				case WM_HSCROLL:
				case WM_VSCROLL:
					UpdateScrollBars();
					break;
			}
		}
	}
}
