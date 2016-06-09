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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public interface IFlyoutAdorner : INavigationAdorner, IDisposable {
		FlyoutStyle Style { get; }
		AdornerHitTest HitTest(Point screenPoint, bool isOwnControl);
		DialogResult? Result { get; }
	}
	internal class FlyoutAdornerBootStrapper : BaseAdornerBootStrapper {
		WindowsUIView View;
		int[] mouseHit = new int[] { 0x201, 0x202, 0x200, 0x2a3, 0x204 };
		int[] mouseIgnore = new int[] { 0x205, 0xa0, 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 0xaa, 0xab };
		int[] msgCancel = new int[] { 0x207, 520, 0x20b, 0x20c, 0xab, 0xac };
		AdornerHitTest prevHitTest;
		public FlyoutAdornerBootStrapper(WindowsUIView view, IFlyoutAdorner flyoutAdorner)
			: base(flyoutAdorner) {
			View = view;
		}
		IFlyoutAdorner FlyoutAdorner {
			get { return Adorner as IFlyoutAdorner; }
		}
		public override void Cancel() {
			if((FlyoutAdorner != null) && isShown)
				View.HideFlyout();
		}
		static DXMouseEventArgs GetArgs(Point pt, Control target) {
			return GetArgs(Control.MouseButtons, target.PointToClient(pt));
		}
		static DXMouseEventArgs GetArgs(MouseButtons buttons, Point pt) {
			return new DXMouseEventArgs(buttons, 0, pt.X, pt.Y, 0);
		}
		public override void Hide() {
			if((FlyoutAdorner != null) && IsShown)
				View.HideFlyout();
		}
		public void HideAdorner() {
			if(IsShown)
				base.Hide();
		}
		protected override bool ShowCore() {
			return FlyoutAdorner.Show();
		}
		protected override bool CanFilterMessage {
			get { return base.CanFilterMessage && IsShown; }
		}
		protected override bool PreFilterMessage(ref Message m) {
			Control ctrl = WinAPIHelper.FindControl(m.HWnd);
			if(ctrl != null) {
				var ownerCtrl = Views.DocumentsHostContext.GetForm(View.Manager);
				if(!Views.DocumentsHostContext.IsChild(ctrl, ownerCtrl)) {
					var form = Views.DocumentsHostContext.CheckForm(ctrl, ctrl as Form);
					if(form != ownerCtrl || (ownerCtrl != null && ownerCtrl.MdiParent != null && ownerCtrl.MdiParent != ctrl))
						return false;
				}
			}
			else return false;
			if(Array.IndexOf<int>(msgCancel, m.Msg) != -1) {
				Cancel();
			}
			if(Array.IndexOf<int>(mouseHit, m.Msg) != -1 || Array.IndexOf<int>(mouseIgnore, m.Msg) != -1) {
				if(FlyoutAdorner == null)
					return false;
				Control ownerEdit = GetOwnerEdit(ctrl);
				bool isOwnControl = FlyoutAdorner.IsOwnControl(ownerEdit ?? ctrl);
				Point screenPoint = WinAPIHelper.Translate(m.HWnd, IntPtr.Zero, WinAPIHelper.GetPoint(m.LParam));
				bool ignoreMessage = Array.IndexOf<int>(mouseIgnore, m.Msg) != -1;
				if(ignoreMessage && m.Msg != MSG.WM_RBUTTONUP)
					screenPoint = WinAPIHelper.GetPoint(m.LParam);
				AdornerHitTest hitTest = FlyoutAdorner.HitTest(screenPoint, isOwnControl);
				if(ignoreMessage) {
					FlyoutAdorner.OnMouseLeave();
					return (hitTest != AdornerHitTest.Control);
				}
				if(m.Msg == MSG.WM_LBUTTONDOWN) {
					switch(hitTest) {
						case AdornerHitTest.Adorner:
							FlyoutAdorner.OnMouseDown(screenPoint);
							break;
						case AdornerHitTest.None:
							if(FlyoutAdorner.Style == FlyoutStyle.Popup)
								Hide();
							break;
					}
					return (hitTest != AdornerHitTest.Control);
				}
				if(m.Msg == MSG.WM_LBUTTONUP) {
					if(hitTest == AdornerHitTest.Adorner)
						FlyoutAdorner.OnMouseUp(screenPoint);
					return (hitTest != AdornerHitTest.Control);
				}
				if(m.Msg == MSG.WM_MOUSEMOVE) {
					if(hitTest == AdornerHitTest.Adorner) {
						FlyoutAdorner.OnMouseMove(screenPoint);
					}
					else {
						if(prevHitTest != hitTest)
							FlyoutAdorner.OnMouseLeave();
					}
					prevHitTest = hitTest;
					return (hitTest != AdornerHitTest.Control);
				}
				if(m.Msg == MSG.WM_MOUSELEAVE) {
					if(prevHitTest != hitTest)
						FlyoutAdorner.OnMouseLeave();
					prevHitTest = hitTest;
				}
			}
			if((m.Msg == MSG.WM_KEYDOWN) && (WinAPIHelper.GetInt(m.WParam) == (int)Keys.Escape)) {
				Cancel();
				return true;
			}
			return base.PreFilterMessage(ref m);
		}
		Control GetOwnerEdit(Control ctrl) {
			if(ctrl == null) return null;
			if(ctrl is DevExpress.XtraEditors.Popup.PopupBaseForm)
				return (ctrl as DevExpress.XtraEditors.Popup.PopupBaseForm).OwnerEdit;
			if(ctrl.Parent != null)
				return GetOwnerEdit(ctrl.Parent);
			return null;
		}
		public void Update() {
			if(FlyoutAdorner != null)
				FlyoutAdorner.Update();
		}
		protected internal DialogResult GetDialogResult() {
			return (FlyoutAdorner != null) ? FlyoutAdorner.Result.GetValueOrDefault(DialogResult.None) : DialogResult.None;
		}
		protected internal void SetDialogResult(DialogResult result) {
			FlyoutAdornerElementInfoArgs args = FlyoutAdorner as FlyoutAdornerElementInfoArgs;
			if(args != null) args.Result = result;
		}
	}
}
