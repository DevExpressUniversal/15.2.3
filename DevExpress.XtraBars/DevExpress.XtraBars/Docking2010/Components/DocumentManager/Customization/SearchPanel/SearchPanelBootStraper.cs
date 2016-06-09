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
namespace DevExpress.XtraBars.Docking2010.Customization {
	class SearchPanelBootStraper : BaseAdornerBootStrapper {
		bool isMouseButtonPressed;
		public SearchPanelBootStraper(IBaseAdorner adorner) : base(adorner) { }
		int[] msgHit = new int[] { MSG.WM_LBUTTONDOWN, MSG.WM_RBUTTONDOWN, MSG.WM_MBUTTONDOWN, MSG.WM_MOUSEMOVE, MSG.WM_MOUSEWHEEL, MSG.WM_LBUTTONUP, MSG.WM_RBUTTONUP, MSG.WM_MBUTTONUP };
		int[] msgIgnore = new int[] { MSG.WM_LBUTTONDBLCLK, MSG.WM_MBUTTONDBLCLK, MSG.WM_RBUTTONDBLCLK };
		int[] msgCancel = new int[] {			
			0x020B,0x020C, 
			0x00A4,0x00A5, 
			0x00A7,0x00A8, 
			0x00AB,0x00AC, 
			MSG.WM_ACTIVATEAPP,
			0xa1, 0xa2, 0xa3, 0xa6, 0xa9, 0xad
		};
		protected bool Enabled { get { return Adorner != null && Adorner.Enabled; } }
		protected new ISearchPanelAdorner Adorner { get { return base.Adorner as ISearchPanelAdorner; } }
		protected override bool PreFilterMessage(ref Message m) {
			if(!Enabled) return false;
			if(m.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_KEYDOWN) {
				Keys key = (Keys)WinAPIHelper.GetInt(m.WParam);
				if((key | Control.ModifierKeys) == Adorner.Shortcut) {
					if(!IsShown) {
						Control ctrl = WinAPIHelper.FindControl(m.HWnd);
						if(!Adorner.RaiseShowing(ctrl)) return false;
						Show();
						return true;
					}
					else Cancel();
				}
				if(key == Keys.Escape && IsShown) { Cancel(); return true; }
			}
			if(Array.IndexOf<int>(msgCancel, m.Msg) != -1)
				if(IsShown) Cancel();
			if(Array.IndexOf<int>(msgIgnore, m.Msg) != -1)
				if(IsShown) return true;
			if(Array.IndexOf<int>(msgHit, m.Msg) != -1) {
				Point screenPoint = Control.MousePosition;
				var hitTest = Adorner.HitTest(screenPoint);
				bool cancel = hitTest != AdornerHitTest.Control && IsShown && !isMouseButtonPressed;
				switch(m.Msg) {
					case MSG.WM_RBUTTONUP:
					case MSG.WM_LBUTTONUP:
					case MSG.WM_MBUTTONUP:
						isMouseButtonPressed = false;
						if(cancel) {
							Cancel();
							return true;
						}
						break;
					case MSG.WM_RBUTTONDOWN:
					case MSG.WM_LBUTTONDOWN:
					case MSG.WM_MBUTTONDOWN:
						if(cancel) return true;
						isMouseButtonPressed = true;
						break;
					case MSG.WM_MOUSEWHEEL:
						if(IsShown) {
							MouseEventArgs args = new MouseEventArgs(MouseButtons.None, 0, screenPoint.X, screenPoint.Y, NativeMethods.SignedHIWORD(m.WParam));
							Adorner.ProcessMouseWheel(args);
							return true;
						}
						break;
					default:
						if(cancel) return true;
						break;
				}
			}
			return base.PreFilterMessage(ref m);
		}
		protected override bool ShowCore() {
			return Adorner.Show();
		}
		public override void Cancel() {
			if(IsDisposing) return;
			base.Cancel();
		}
		public override void Hide() {
			if(IsDisposing) return;
			base.Hide();
		}
		public override void Show() {
			if(IsDisposing || !Enabled) return;
			base.Show();
		}
		public void Update() {
			if(IsDisposing) return;
			if(Adorner == null) return;
			Adorner.Update();
		}
	}
}
