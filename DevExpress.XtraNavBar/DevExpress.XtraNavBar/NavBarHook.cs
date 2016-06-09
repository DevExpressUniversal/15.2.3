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
using DevExpress.Utils.Win.Hook;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraNavBar.Forms;
using DevExpress.Utils.Menu;
using DevExpress.XtraNavBar.ViewInfo;
namespace DevExpress.XtraNavBar {
	public class NavBarHook : IHookController {
		NavBarControl navBar;
		internal const int WM_MOUSEWHEEL = 0x20a;
		internal const int WM_NCLBUTTONDOWN = 0x00a1;
		internal const int WM_LBUTTONDOWN = 0x201;
		internal const int WM_ACTIVATEAPP = 28;
		internal const int WM_ACTIVATE = 6;
		public NavBarHook(NavBarControl navBar) {
			this.navBar = navBar;
			HookManager.DefaultManager.AddController(this);
		}
		public NavBarControl NavBar { get { return navBar; } }
		IntPtr IHookController.OwnerHandle {
			get {
				Form frm = NavBar.FindForm();
				if(frm == null || !frm.IsHandleCreated) return IntPtr.Zero;
				return frm.Handle;
			}
		}
		protected virtual void OnActivateApp(Control wnd, IntPtr wParam, IntPtr lParam) {
			if(NavBar.NavPaneForm == null || !NavBar.NavPaneForm.Visible) return;
			if(lParam.ToInt32() != 0)
				NavBar.HideNavPaneForm();
		}
		protected virtual bool IsNavPaneFormChild(Control wnd) {
			while(wnd != NavBar.NavPaneForm && wnd != null) {
				if(wnd.Parent == null) {
					Form frm = wnd as Form;
					if(frm == null)
						return false;
					if(frm is IDXPopupMenuForm)
						return true;
					wnd = frm.Owner;
				}
				else
					wnd = wnd.Parent;
			}
			return wnd == NavBar.NavPaneForm;
		}
		protected virtual void OnLeftButtonDown(Control wnd, IntPtr wParam, IntPtr lParam) {
			Point p = Control.MousePosition;
			if(NavBar.NavPaneForm == null || !NavBar.NavPaneForm.Visible) return;
			NavigationPaneViewInfo navPaneViewInfo = NavBar.ViewInfo as NavigationPaneViewInfo;
			if(navPaneViewInfo == null)
				return;
			if(!navPaneViewInfo.ContentButton.Contains(NavBar.PointToClient(p)) && !NavBar.NavPaneForm.Bounds.Contains(p) && !IsNavPaneFormChild(wnd)) {
				NavBar.HideNavPaneForm();
			}
		}
		protected virtual void OnMouseWheel(Control wnd, IntPtr wParam, IntPtr lParam) {
			if(NavBar.OptionsNavPane.NavPaneState == NavPaneState.Collapsed || NavBar.ScrollMode != NavBarScrollMode.ScrollAlways) return;
			short delta = (short)((((int)((long)wParam)) >> 16) & 0xffff);
			Point pt = Control.MousePosition;
			NavBar.ViewInfo.OnMouseWheel(new MouseEventArgs(MouseButtons.None, 0, pt.X, pt.Y, (int)delta));
		}
		protected virtual void OnNCLeftButtonDown(Control wnd, IntPtr wParam, IntPtr lParam) {
			if (NavBar.NavPaneForm == null || !NavBar.NavPaneForm.Visible) return;
			Point p = Control.MousePosition;
			if(!NavBar.NavPaneForm.Bounds.Contains(p) && !IsNavPaneFormChild(wnd)) {
			NavBar.HideNavPaneForm();
		}
		}
		protected virtual void OnActivate(Control wnd, IntPtr wParam, IntPtr lParam) {
			if(wParam.ToInt32() != 0 && (wParam.ToInt32() & 0x0000ffff) == 0) NavBar.HideNavPaneForm();
			if(NavBar.NavPaneForm != null && NavBar.NavPaneForm.IsHandleCreated && lParam == NavBar.NavPaneForm.Handle)
				NavBar.skipDeactivate = true;
		}
		bool IHookController.InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			return false;
		}
		bool IHookController.InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			switch(Msg) {
				case WM_LBUTTONDOWN:
					OnLeftButtonDown(wnd, WParam, LParam);
					break;
				case WM_MOUSEWHEEL:
					OnMouseWheel(wnd, WParam, LParam);
					break;
				case WM_NCLBUTTONDOWN:
					OnNCLeftButtonDown(wnd, WParam, LParam);
					break;
				case WM_ACTIVATEAPP:
					OnActivateApp(wnd, WParam, LParam);
					break;
				case WM_ACTIVATE:
					OnActivate(wnd, WParam, LParam);
					break;
			}
			return false;
		}
	}
}
