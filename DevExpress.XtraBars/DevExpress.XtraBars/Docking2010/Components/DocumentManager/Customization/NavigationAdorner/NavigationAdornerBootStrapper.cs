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
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public enum AdornerHitTest { None, Adorner, Control }
	public interface INavigationAdorner : IBaseAdorner {		
		void Update();				
		void OnMouseDown(Point screenPoint);
		void OnMouseMove(Point screenPoint);
		void OnMouseUp(Point screenPoint);
		void OnMouseLeave();
		bool IsOwnControl(Control control);
	}
	class NavigationAdornerBootStrapper : BaseAdornerBootStrapper {
		WindowsUIView View;
		bool prevHitTest;
		public NavigationAdornerBootStrapper(WindowsUIView view, INavigationAdorner navigationAdorner) : base(navigationAdorner) {
			View = view;			
		}
		INavigationAdorner NavigationAdorner { get { return Adorner as INavigationAdorner; } }
		int[] msgCancel = new int[] { 
			0x0207,0x0208, 
			0x020B,0x020C, 
			0x00A4,0x00A5, 
			0x00A7,0x00A8, 
			0x00AB,0x00AC, 
			MSG.WM_ACTIVATEAPP,
			0xa1, 0xa2, 0xa3, 0xa6, 0xa9, 0xad
		};
		int[] mouseIgnore = new int[] { 
			MSG.WM_LBUTTONDBLCLK,
			MSG.WM_RBUTTONDBLCLK,			
			0x0205 
		};
		int[] mouseHit = new int[] { 
			MSG.WM_LBUTTONDOWN,
			MSG.WM_LBUTTONUP,
			MSG.WM_MOUSEMOVE,
			0x02A3, 
		};		
		protected override bool PreFilterMessage(ref Message m){			
			 if(Array.IndexOf(msgCancel, m.Msg) != -1) {
				if(IsShown) Cancel();
			}
			if(Array.IndexOf(mouseIgnore, m.Msg) != -1) {
				if(IsShown) {
					Control ctrl = WinAPIHelper.FindControl(m.HWnd);
					if(IsFlyoutPanelChild(ctrl)) 
						return false;
					return true;
				}
			}
			if(m.Msg == 0x0204 || m.Msg == 0x0206) { 
				Control ctrl = WinAPIHelper.FindControl(m.HWnd);
				Point screenPoint = WinAPIHelper.Translate(m.HWnd, IntPtr.Zero, WinAPIHelper.GetPoint(m.LParam));
				if(!IsShown) {
					if(NavigationAdorner.IsOwnControl(ctrl)) {
						var ea = GetArgs(screenPoint, ctrl);
						bool handled = false;
						View.navigationAdornerCounter++;
						try {
							bool canActivate = View.CanActivateNavigationAdornerOnRightClick(screenPoint, ctrl, ea, out handled);
							View.ForceShowNavigationAdorner = false;
							if(View.RaiseNavigationBarsShowing(ctrl, ea, !canActivate)) {
								View.ShowNavigationAdorner();
								return true;
							}
							return handled;
						}
						finally { View.navigationAdornerCounter--; }
					}
				}
				else {
					if(NavigationAdorner.IsOwnControl(ctrl)) {
						var ea = GetArgs(screenPoint, ctrl);
						if(!View.CanCancelNavigationAdornerOnRightClick(screenPoint, ctrl, ea)) {
							View.ShowNavigationAdorner();
							View.ForceShowNavigationAdorner = false;
							return true;
						}
					}
					else {
						if(IsFlyoutPanelChild(ctrl))
							return false;
					}
					Cancel();
				}
			}
			if(IsShown) {
				if(Array.IndexOf(mouseHit, m.Msg) != -1) {
					Control ctrl = WinAPIHelper.FindControl(m.HWnd);
					Point screenPoint = WinAPIHelper.Translate(m.HWnd, IntPtr.Zero, WinAPIHelper.GetPoint(m.LParam));
					AdornerHitTest adornerHitTest = NavigationAdorner.HitTest(screenPoint);
					if(adornerHitTest == AdornerHitTest.Control) {						
						View.SetCursor(null);						
						return false;
					}
					bool hitTest = adornerHitTest != AdornerHitTest.None;
					if(m.Msg == MSG.WM_LBUTTONDOWN) {
						if(hitTest)
							NavigationAdorner.OnMouseDown(screenPoint);
						else Cancel();
						return true;
					}
					if(m.Msg == MSG.WM_LBUTTONUP) {
						if(hitTest)
							NavigationAdorner.OnMouseUp(screenPoint);							
						else Cancel();
						return true;
					}
					if(m.Msg == MSG.WM_MOUSEMOVE) {
						if(hitTest)
							NavigationAdorner.OnMouseMove(screenPoint);
						else {
							if(prevHitTest != hitTest)
								NavigationAdorner.OnMouseLeave();
							prevHitTest = hitTest;
						}
						return true;
					}
					if(m.Msg == 0x02A3) {
						if(prevHitTest != hitTest)
							NavigationAdorner.OnMouseLeave();
						prevHitTest = hitTest;
					}
				}
			}
			if(m.Msg == MSG.WM_KEYDOWN) {
				Keys key = (Keys)WinAPIHelper.GetInt(m.WParam);
				if(key == Keys.Escape) {
					Control ctrl = WinAPIHelper.FindControl(m.HWnd);
					if(NavigationAdorner.IsOwnControl(ctrl)) {
						if(!IsShown) {
							bool canActivate = View.CanActivateNavigationAdornerOnKeyDown(ctrl);
							EventArgs ea = new KeyEventArgs(key);
							if(View.RaiseNavigationBarsShowing(ctrl, ea, !canActivate)) 
								Show();
						}
						else Cancel();
					}
				}
			}
			return base.PreFilterMessage(ref m);
		}
		static bool IsFlyoutPanelChild(Control ctrl) {
			return (ctrl != null) && Views.DocumentsHostContext.CheckForm(ctrl, ctrl as Form) is DevExpress.Utils.FlyoutPanelBeakForm;
		}
		protected override bool ShowCore() {
			return NavigationAdorner.Show();
		}
		public void Update() {
			if(NavigationAdorner != null)
				NavigationAdorner.Update();			
		}
		public override void Hide() {
			base.Hide();
			prevHitTest = false;			
		}
		public override void Cancel() {
			base.Cancel();
			prevHitTest = false;			
		}
		static DevExpress.Utils.DXMouseEventArgs GetArgs(Point pt, Control target) {
			return GetArgs(Control.MouseButtons, target.PointToClient(pt));
		}
		static DevExpress.Utils.DXMouseEventArgs GetArgs(MouseButtons buttons, Point pt) {
			return new DevExpress.Utils.DXMouseEventArgs(buttons, 0, pt.X, pt.Y, 0);
		}
	}
}
