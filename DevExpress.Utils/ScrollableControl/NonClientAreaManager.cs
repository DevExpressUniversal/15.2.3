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
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using System.Security;
using DevExpress.LookAndFeel;
namespace DevExpress.Utils.NonclientArea {
	public class NonClientAreaManager {
		ArrayList viewInfos;
		XtraScrollableControl owner;
		public XtraScrollableControl Owner { get { return owner; } }
		public NonClientAreaManager(XtraScrollableControl owner, ArrayList viewInfos) {
			this.owner = owner;
			this.viewInfos = viewInfos;
		}
		public virtual bool ProcessMessage(ref Message m) {
			if(m.Msg == NCMessages.WM_NCPAINT) {
				OnNcPaint();
				return true;
			}
			if(m.Msg == NCMessages.WM_NCCALCSIZE) {
				OnNcCalcSize(ref m);
				return true;
			}
			if(m.Msg == NCMessages.WM_NCHITTEST) {
				return OnCalcNcHitTest(ref m);
			}
			if(m.Msg == MSG.WM_SETCURSOR) {
				Owner.OnScrollAction(ScrollNotifyAction.MouseMove);
			}
			if(m.Msg == NCMessages.WM_NCMOUSEMOVE) {
				TrackMouseLeaveMessage();
				if(Owner.SizeGrip.Bounds.Contains(Owner.PointToClient(Control.MousePosition))) {
					OnNcMouseLeave();
				}
			}
			if(m.Msg == NCMessages.WM_NCMOUSELEAVE) {
				isTracking = false;
				OnNcMouseLeave();
			}
			if((m.Msg >= 0x00A0) && (m.Msg <= 0x00A9) || (m.Msg >= 0x00AB) && (m.Msg <= 0x00AD)) {
				ProcessMouseMessage(ref m);
				return true;
			}
			return false;
		}
		[SecuritySafeCritical]
		protected virtual void DoDraw(IScrollView viewInfo, IntPtr dc) {
			if (viewInfo.Bounds == Rectangle.Empty) return;
			if(viewInfo.TouchMode && viewInfo.IsOverlap) return;
			try {
				using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(dc, viewInfo.Bounds)) {
					using(GraphicsCache tempCache = new GraphicsCache(bg.Graphics)) {
						if(viewInfo.TouchMode) {
							Rectangle bounds = viewInfo.Bounds;
							BackgroundPaintHelper.PaintTransparentBackground(Owner, new PaintEventArgs(tempCache.Graphics, bounds), bounds);
						}
						else 
							viewInfo.DoDraw(tempCache, viewInfo.Bounds);
					}
					bg.Render();
				}
				return;
			}
			catch(Exception e) {
				if(ControlPaintHelper.IsCriticalException(e)) throw;
			}
			using(Graphics g = Graphics.FromHdc(dc)) {
				using(GraphicsCache tempCache = new GraphicsCache(g)) {
					viewInfo.DoDraw(tempCache, viewInfo.Bounds);
				}
			}
		}
		[SecuritySafeCritical]
		protected virtual void OnNcPaint() {
			if(!Owner.IsHandleCreated) return;
			IntPtr dc = NativeMethods.GetWindowDC(Owner.Handle);
			foreach(IScrollView vi in viewInfos) {
				if(IsElementVisible(vi)) DoDraw(vi, dc);
			}
			NativeMethods.ReleaseDC(Owner.Handle, dc);
		}
		protected internal void ForceRepaintNcElement(IScrollView element) {
			if(!Owner.IsHandleCreated) return;
			if(!IsElementVisible(element)) return;
			IntPtr dc = NativeMethods.GetWindowDC(Owner.Handle);
			DoDraw(element, dc);
			NativeMethods.ReleaseDC(Owner.Handle, dc);
		}
		protected internal void OnNcMouseLeave() {
			Owner.HScrollBar.OnMouseLeave(EventArgs.Empty);
			Owner.VScrollBar.OnMouseLeave(EventArgs.Empty);
		}
		protected internal void PerformNcAreaLayout() {
			foreach(IScrollView viewInfo in viewInfos) {
				if(viewInfo != null && IsElementVisible(viewInfo)) viewInfo.PerformLayout();
			}
		}
		protected virtual void OnNcCalcSize(ref Message m) {
			Rectangle clientRect = Owner.ClientRectangle;
			NativeMethods.NCCALCSIZE_PARAMS NCparams = NativeMethods.NCCALCSIZE_PARAMS.GetFrom(m.LParam);
			if(Owner.VScrollVisible && !Owner.IsOverlapVScrollBar) NCparams.rgrc0.Right -= Owner.DefaultVScrollBarWidth;
			if(Owner.HScrollVisible && !Owner.IsOverlapHScrollBar) NCparams.rgrc0.Bottom -= Owner.DefaultHScrollBarHeight;
			NCparams.SetTo(m.LParam);
			m.Result = IntPtr.Zero;
		}
		bool isTracking = false;
		void TrackMouseLeaveMessage() {
			if(isTracking) return;
			NativeMethods.TRACKMOUSEEVENTStruct track = new NativeMethods.TRACKMOUSEEVENTStruct();
			track.dwFlags = 0x13;
			track.hwndTrack = Owner.Handle;
			if(!NativeMethods.TrackMouseEvent(track)) {
				return;
			}
			isTracking = true;
		}
		protected virtual bool OnCalcNcHitTest(ref Message m) {
			Point p = GetPointFromMessage(ref m);
			p = Owner.PointToClient(p);
			IScrollView control = GetElementByPoint(p);
			if(control != null) {
				if(control is HScrollBarViewInfoWithHandler) {
					m.Result = new IntPtr(6);
				}
				if(control is VScrollBarViewInfoWithHandler) {
					m.Result = new IntPtr(7);
				}
				if(control is SizeGripViewInfoWithHandler) {
					m.Result = new IntPtr(4);
				}
				return true;
			}
			return false;
		}
		protected Point GetPointFromMessage(ref Message m) {
			return new Point((short)((int)m.LParam), ((int)m.LParam) >> 0x10);
		}
		protected IScrollView GetElementByPoint(Point point) {
			foreach(IScrollView viewInfo in viewInfos) {
				if(viewInfo.Bounds.Contains(point)) return viewInfo;
			}
			return null;
		}
		protected bool IsElementVisible(IScrollView control) {
			return control.Bounds.Width > 0 && control.Bounds.Height > 0;
		}
		protected MouseEventArgs CalcMouseEventArgs(ref Message m, IScrollView control, Point point) {
			return new MouseEventArgs(MouseButtons.Left, (int)m.WParam, point.X - control.Bounds.X, point.Y - control.Bounds.Y, 0);
		}
		protected virtual void ProcessMouseMessage(ref Message m) {
			Owner.CapturedScrollBar = null;
			Point p = GetPointFromMessage(ref m);
			p = Owner.PointToClient(p);
			if(Owner.IsOverlapVScrollBar && Owner.IsOverlapHScrollBar) return;
			IScrollView element = GetElementByPoint(p);
			if(element != null) {
				MouseEventArgs mea = CalcMouseEventArgs(ref m, element, p);
				switch(m.Msg) {
					case NCMessages.WM_NCMOUSEMOVE:
						element.OnMouseMove(mea);
						break;
					case NCMessages.WM_NCLBUTTONDBLCLK:
						element.OnMouseDown(mea);
						break;
					case NCMessages.WM_NCLBUTTONDOWN:
						if(!Owner.Capture && element is ScrollBarViewInfoWithHandlerBase) {
							Owner.Capture = Owner.IsCaptured = true;
							Owner.CapturedScrollBar = element as ScrollBarViewInfoWithHandlerBase;
						}
						element.OnMouseDown(mea);
						break;
					case NCMessages.WM_NCLBUTTONUP:
						element.OnMouseUp(mea);
						break;
				}
			}
		}
	}
}
