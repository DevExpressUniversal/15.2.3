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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Description.Controls.Windows;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.Description.Controls {
	public class GuideControlEx : GuideControl, IMessageFilter {
		bool hasMessageFilter = false;
		IGuideForm activeGuideForm;
		protected IGuideForm ActiveGuideForm {
			get { return activeGuideForm; }
			set {
				if(ActiveGuideForm == value) return;
				if(activeGuideForm != null) {
					activeGuideForm.Dispose();
					ActivateRootControl();
				}
				activeGuideForm = value;
			}
		}
		void ActivateRootControl() {
			if(Root == null) return;
			Form f = Root.FindForm();
			if(f == null) return;
			f.Activate();
		}
		protected override void OnShowing() {
			HookMouse();
		}
		private void HookMouse() {
			if(hasMessageFilter) return;
			this.hasMessageFilter = true;
			Application.AddMessageFilter(this);
		}
		protected override void OnHide() {
			base.OnHide();
			ActiveGuideForm = null;
			UnHookMouse();
		}
		void UnHookMouse() {
			if(!hasMessageFilter) return;
			this.trackMouse = false;
			this.hasMessageFilter = false;
			Application.RemoveMessageFilter(this);
		}
		#region IMessageFilter Members
		bool IMessageFilter.PreFilterMessage(ref Message m) {
			if(IsKeyboardMessage(ref m)) return ProcessKeyboard(ref m);
			if(CheckIsGuideFormMessage(ref m)) return false;
			if(IsMouseMessage(ref m)) {
				return ProcessMouse(ref m);
			}
			return false;
		}
		bool CheckIsGuideFormMessage(ref Message m) {
			if(ActiveGuideForm == null || !ActiveGuideForm.Visible) return false;
			if(ActiveGuideForm.IsHandle(m.HWnd)) return true;
			Control control = Control.FromHandle(m.HWnd);
			if(control == null) return false;
			if(control.FindForm() == ActiveGuideForm) return true;
			return false;
		}
		protected virtual bool ProcessKeyboard(ref Message m) {
			if(m.Msg == MSG.WM_KEYDOWN) {
				if(m.WParam.ToInt32() == (int)Keys.Escape) {
					if(ActiveGuideForm != null) {
						ActiveGuideForm = null;
						return true;
					}
					Hide();
				}
			}
			return true;
		}
		protected MouseEventArgs GetMouseArgs(ref Message msg) {
			int btns = msg.WParam.ToInt32();
			MouseButtons buttons = MouseButtons.None;
			if((btns & MSG.MK_LBUTTON) != 0) buttons |= MouseButtons.Left;
			if((btns & MSG.MK_RBUTTON) != 0) buttons |= MouseButtons.Right;
			Point pt = PointToFormBounds(msg.HWnd, msg.LParam);
			MouseEventArgs e = new MouseEventArgs(buttons, 1, pt.X, pt.Y, 0);
			return e;
		}
		protected virtual Point PointToFormBounds(IntPtr hwnd, Point pt) {
			NativeMethods.POINT p = new NativeMethods.POINT(pt);
			NativeMethods.ClientToScreen(hwnd, ref p);
			return ConvertPoint(Root.PointToClient(new Point(p.X, p.Y)));
		}
		public Point PointToFormBounds(IntPtr hwnd, IntPtr ptr) {
			Point p = Point.Empty;
			try {
				p = new Point((int)ptr);
			}
			catch(Exception) {
				p = Point.Empty;
			}
			return PointToFormBounds(hwnd, p);
		}
		protected virtual bool ProcessMouse(ref Message m) {
			switch(m.Msg) {
				case MSG.WM_NCMOUSEMOVE:
					GenerateMouseMove();
					return true;
				case MSG.WM_MOUSELEAVE:
					OnMouseLeave(ref m);
					return true;
				case MSG.WM_MOUSEMOVE:
					TrackMouseLeaveMessage(m.HWnd);
					return OnMouseMove(GetMouseArgs(ref m));
				case MSG.WM_LBUTTONDBLCLK:
				case MSG.WM_RBUTTONDBLCLK:
					return true;
				case MSG.WM_LBUTTONDOWN:
				case MSG.WM_RBUTTONDOWN:
					return OnMouseDown(GetMouseArgs(ref m));
				case MSG.WM_LBUTTONUP:
				case MSG.WM_RBUTTONUP:
					return OnMouseUp(GetMouseArgs(ref m));
				case MSG.WM_MOUSEWHEEL:
				case MSG.WM_MOUSEHWHEEL:
					return true;
			}
			return false;
		}
		void GenerateMouseMove() {
			Point p = ConvertPoint(Root.PointToClient(Control.MousePosition));
			OnMouseMove(new MouseEventArgs(MouseButtons.None, 0, p.X, p.Y, 0));
		}
		void OnMouseLeave(ref Message m) {
			this.trackMouse = false;
			GenerateMouseMove();
		}
		bool OnMouseUp(MouseEventArgs e) {
			return true;
		}
		bool OnMouseDown(MouseEventArgs e) {
			if(e.Button == MouseButtons.Right) {
				Hide();
				return true;
			}
			if(e.Button == MouseButtons.Left) {
				var guide = FromPoint(e.Location);
				if(guide != null && guide != ActiveControl) {
					ActiveGuideForm = null;
					ActiveControl = guide;
				}
				if(ActiveControl != null && ActiveGuideForm == null) {
					ActiveGuideForm = CreageGuideForm();
					ActiveGuideForm.Show(this, ActiveControl);
					ActiveGuideForm.FormClosed += OnActiveGuideForm_FormClosed;
				}
			}
			return true;
		}
		protected virtual IGuideForm CreageGuideForm() {
			return new GuideFormAlt();
		}
		void OnActiveGuideForm_FormClosed(object sender, EventArgs e) {
			ActiveGuideForm = null;
		}
		bool OnMouseMove(MouseEventArgs e) {
			var guide = FromPoint(e.Location);
			if(ActiveGuideForm == null) {
				Cursor.Current = guide == null ? Cursors.Arrow : Cursors.Help;
			}
			ActiveControl = guide;
			return true;
		}
		bool trackMouse = false;
		void TrackMouseLeaveMessage(IntPtr hwnd) {
			if(trackMouse) return;
			NativeMethods.TRACKMOUSEEVENTStruct track = new NativeMethods.TRACKMOUSEEVENTStruct();
			track.dwFlags = 3;
			track.hwndTrack = hwnd;
			if(!NativeMethods.TrackMouseEvent(track)) {
				return;
			}
			trackMouse = true;
		}
		bool IsKeyboardMessage(ref Message m) {
			switch(m.Msg) {
				case MSG.WM_KEYDOWN:
				case MSG.WM_SYSKEYDOWN:
				case MSG.WM_SYSKEYUP:
				case MSG.WM_KEYUP:
					return true;
			}
			return false;
		}
		bool IsMouseMessage(ref Message m) {
			switch(m.Msg) {
				case MSG.WM_MOUSEWHEEL:
				case MSG.WM_MOUSEHWHEEL:
				case MSG.WM_NCMOUSEMOVE:
				case MSG.WM_LBUTTONDOWN:
				case MSG.WM_LBUTTONUP:
				case MSG.WM_LBUTTONDBLCLK:
				case MSG.WM_RBUTTONDOWN:
				case MSG.WM_RBUTTONUP:
				case MSG.WM_RBUTTONDBLCLK:
				case MSG.WM_MBUTTONDOWN:
				case MSG.WM_MBUTTONUP:
				case MSG.WM_MBUTTONDBLCLK:
				case MSG.WM_MOUSEMOVE:
				case MSG.WM_MOUSEHOVER:
				case MSG.WM_MOUSELEAVE:
					return true;
			}
			return false;
		}
		#endregion
		GuideControlDescription activeControl;
		public GuideControlDescription ActiveControl {
			get { return activeControl; }
			set {
				if(ActiveControl == value) return;
				if(ActiveGuideForm != null) return;
				activeControl = value;
				OnActiveControlChanged();
			}
		}
		protected virtual void OnActiveControlChanged() {
			if(!IsVisible) return;
			Window.Invalidate();
		}
		protected override DXGuideLayeredWindow CreateWindow() { return new DXGuideLayeredWindowEx(this); }
		protected GuideControlDescription FromPoint(Point point) {
			List<GuideControlDescription> controls = null;
			GuideControlDescription result = null;
			foreach(var c in this.Descriptions) {
				if(!c.IsValidNow) continue;
				if(c.ControlBounds.Contains(point)) {
					if(result == null) result = c;
					else {
						if(controls == null) {
							controls = new List<GuideControlDescription>();
							controls.Add(result);
						}
						controls.Add(c);
					}
				}
			}
			if(controls == null) return result;
			return SelectControl(controls);
		}
		GuideControlDescription SelectControl(List<GuideControlDescription> controls) {
			controls.Sort(CompareControls);
			return controls.Last();
		}
		int CompareControls(GuideControlDescription x, GuideControlDescription y) {
			if(object.ReferenceEquals(x, y)) return 0;
			if(x.AssociatedControl != null && y.AssociatedControl != null) {
				if(x.AssociatedControl.Contains(y.AssociatedControl)) return -1;
				if(y.AssociatedControl.Contains(x.AssociatedControl)) return 1;
				if(x.ControlBounds.Contains(y.ControlBounds)) return -1;
				if(y.ControlBounds.Contains(x.ControlBounds)) return 1;
			}
			return 0;
		}
	}
}
