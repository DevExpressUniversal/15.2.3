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
using System.Text;
using DevExpress.XtraEditors;
using System.ComponentModel.Design;
using DevExpress.LookAndFeel;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design {
	class ScrollablePanel : XtraScrollableControl {
		#region inner classes
		delegate void XRKeyEventHandler(IKeyTarget keyTarget, Control control, KeyPressEventArgs e);
		class KeyEventHelper {
			public static void HandleKeyPress(IKeyTarget keyTarget, Control control, KeyPressEventArgs args) {
				if(keyTarget != null)
					keyTarget.HandleKeyPress(control, args);
			}
			IDesignerHost host;
			ISelectionService selectionService;
			Control control;
			public KeyEventHelper(IDesignerHost host, Control control) {
				this.host = host;
				this.control = control;
				selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			}
			public void HandleKeyPressEvent(XRKeyEventHandler keyHandler, KeyPressEventArgs e) {
				IKeyTarget target = CreateKeyTarget();
				if(target != null && keyHandler != null)
					keyHandler(target, control, e);
			}
			IKeyTarget CreateKeyTarget() {
				XRControl ctrl = selectionService.PrimarySelection as XRControl;
				if(ctrl == null)
					return null;
				return host.GetDesigner(ctrl) as IKeyTarget;
			}
		}
		#endregion
		static XtraScrollEventArgs MouseWheelEmptyScrollEventArgs = new XtraScrollEventArgs(ScrollEventType.EndScroll, 0, 0, DevExpress.XtraEditors.ScrollOrientation.VerticalScroll);
		static object MouseWheelZoomEvent = new object();
		private KeyEventHelper keyEventHelper;
		bool forceUpdateEnabled = true;
		public event MouseWheelZoomEventHandler MouseWheelZoom {
			add { Events.AddHandler(MouseWheelZoomEvent, value); }
			remove { Events.RemoveHandler(MouseWheelZoomEvent, value); }
		}
		public ScrollablePanel(IDesignerHost host) {
			AlwaysScrollActiveControlIntoView = false;
			AutoScroll = true;
			AutoScrollMargin = new Size(10, 10);
			SetStyle(ControlStyles.Selectable, true);
			keyEventHelper = new KeyEventHelper(host, this);
		}
		public void ApplyLookAndFeel(UserLookAndFeel lookAndFeel) {
			ReportPaintStyle paintStyle = ReportPaintStyles.GetPaintStyle(lookAndFeel);
			BackColor = paintStyle.GetReportBackgroundColor(lookAndFeel);
		}
		public void SetOffsetWithoutUpdate(int deltaX, int deltaY) {
			forceUpdateEnabled = false;
			try {
				base.SetOffset(deltaX, deltaY);
			} finally {
				forceUpdateEnabled = true;
			}
		}
		const int WM_SETREDRAW = 11;
		public void SuppressRedraw() {
			Win32.SendMessage(new System.Runtime.InteropServices.HandleRef(this, this.Handle), WM_SETREDRAW, 0, IntPtr.Zero);
		}
		public void ResumeRedraw() {
			Win32.SendMessage(new System.Runtime.InteropServices.HandleRef(this, this.Handle), WM_SETREDRAW, -1, IntPtr.Zero);
		}
		public void SetScroll(bool enable) {
			HScroll = VScroll = enable;
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			keyEventHelper.HandleKeyPressEvent(new XRKeyEventHandler(KeyEventHelper.HandleKeyPress), e);
		}
		void OnMouseWheelZoom(MouseWheelZoomEventArgs e) {
			MouseWheelZoomEventHandler handler = Events[MouseWheelZoomEvent] as MouseWheelZoomEventHandler;
			if(handler != null) handler(this, e);
		}
		protected override void ForceUpdate() {
			if(forceUpdateEnabled)
				base.ForceUpdate();
		}
		protected override void OnMouseWheelCore(MouseEventArgs ev) {
			if(Control.ModifierKeys == Keys.Control)
				OnMouseWheelZoom(new MouseWheelZoomEventArgs(GetZoomOffsetLocation(ev.Location), ev.Delta));
			else
				base.OnMouseWheelCore(ev);
		}
		PointF GetZoomOffsetLocation(Point cursorLocation) {
			if(ClientRectangle.Contains(cursorLocation)) {
				return new PointF(cursorLocation.X - ClientSize.Width / 2f, cursorLocation.Y - ClientSize.Height / 2f);
			} else
				return Point.Empty;
		}
	}
	public class MouseWheelZoomEventArgs : EventArgs {
		PointF offset;
		int delta;
		public PointF Offset { get { return offset; } }
		public int Delta { get { return delta; } }
		public MouseWheelZoomEventArgs(PointF offset, int delta) {
			this.offset = offset;
			this.delta = delta;
		}
	}
	public delegate void MouseWheelZoomEventHandler(object sender, MouseWheelZoomEventArgs e);
}
