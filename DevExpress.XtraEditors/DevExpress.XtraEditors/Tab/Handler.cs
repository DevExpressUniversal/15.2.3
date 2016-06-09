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
using DevExpress.Utils.Controls;
namespace DevExpress.XtraTab.ViewInfo {
	public class BaseTabHandler : BaseHandler {
		IXtraTab tabControl;
		BaseTabHitInfo downHitInfo;
		public BaseTabHandler(IXtraTab tabControl) {
			this.tabControl = tabControl;
			this.downHitInfo = ViewInfo.CreateHitInfo();
		}
		protected override Rectangle ClientBounds { get { return TabControl.Bounds; } }
		public IXtraTab TabControl { get { return tabControl; } }
		public BaseTabControlViewInfo ViewInfo { get { return TabControl.ViewInfo; } }
		public BaseTabHitInfo DownHitInfo { get { return downHitInfo; } set { downHitInfo = value; } }
		protected override void OnResize(Rectangle clientRect) {
			if(ViewInfo != null) ViewInfo.Resize();
			base.OnResize(clientRect);
		}
		protected override void OnLostCapture() {
			if(ViewInfo != null) ViewInfo.HeaderInfo.Buttons.ProcessEvent(new ProcessEventEventArgs(EventType.LostCapture, null));
		}
		protected override bool OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseDown(ee);
			if(ee.Handled) return true;
			if(ViewInfo == null) return false;
			this.DownHitInfo = ViewInfo.CalcHitInfo(e.Location);
			this.DownHitInfo.MakePageReferenceWeak();
			if(DownHitInfo.HitTest == XtraTabHitTest.PageHeader) {
				if(!DownHitInfo.InPageControlBox) {
					IXtraTabPage page = DownHitInfo.Page;
					ProcessTabMouseDown(e, DownHitInfo, TabMiddleClickFiringMode.MouseDown);
					if(page.TabControl == null)
						return true;
					else ViewInfo.SelectedTabPage = page;
				}
				else {
					BaseTabPageViewInfo pInfo = null;
					if(TryGetTabPageViewInfo(e.Location, out pInfo))
						pInfo.ButtonsPanel.Handler.OnMouseDown(ee);
				}
			}
			if(DownHitInfo.HitTest == XtraTabHitTest.PageHeaderButtons) {
				ViewInfo.HeaderInfo.Buttons.ProcessEvent(new ProcessEventEventArgs(EventType.MouseDown, ee));
				return true;
			}
			return false;
		}
		public override bool RequireMouse(MouseEventArgs e) {
			Point p = new Point(e.X, e.Y);
			if(TabControl.ViewInfo.HeaderInfo.Bounds.Contains(p)) return true;
			return base.RequireMouse(e); ;
		}
		protected override bool OnMouseMove(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseMove(e);
			if(ee.Handled) return true;
			UpdateHotTrackedPage(e.Location);
			BaseTabPageViewInfo pInfo = null;
			if(TryGetTabPageViewInfo(e.Location, out pInfo))
				pInfo.ButtonsPanel.Handler.OnMouseMove(ee);
			if(ViewInfo != null) ViewInfo.HeaderInfo.Buttons.ProcessEvent(new ProcessEventEventArgs(EventType.MouseMove, ee));
			return false;
		}
		protected override bool OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			if(ViewInfo == null) return false;
			BaseTabHitInfo mouseUpHitInfo = ViewInfo.CalcHitInfo(e.Location);
			ViewInfo.HeaderInfo.Buttons.ProcessEvent(new ProcessEventEventArgs(EventType.MouseUp, ee));
			if(mouseUpHitInfo.HitTest == XtraTabHitTest.PageHeader && !mouseUpHitInfo.InPageControlBox) {
				IXtraTabPage page = mouseUpHitInfo.Page;
				ProcessTabMouseUp(e, mouseUpHitInfo, TabMiddleClickFiringMode.MouseUp);
				if(page.TabControl == null)
					return true;
			}
			BaseTabPageViewInfo pInfo = null;
			if(TryGetTabPageViewInfo(e.Location, out pInfo))
				pInfo.ButtonsPanel.Handler.OnMouseUp(ee);
			return base.OnMouseUp(ee);
		}
		protected bool TryGetTabPageViewInfo(Point pt, out BaseTabPageViewInfo pInfo) {
			if(ViewInfo == null) { pInfo = null; return false; }
			pInfo = ViewInfo.HeaderInfo.FindPage(pt);
			return pInfo != null;
		}
		Point tabMiddleClickPoint;
		protected void ProcessTabMouseDown(MouseEventArgs e, BaseTabHitInfo hi, TabMiddleClickFiringMode mode) {
			if(ViewInfo == null) return;
			if(e.Button == MouseButtons.Middle) {
				if(ViewInfo.FireTabMiddleClickOnMouseDown && (hi.Page != null) && hi.Page.PageEnabled)
					ViewInfo.OnTabMiddleClick(new PageEventArgs(hi.Page));
				else tabMiddleClickPoint = e.Location;
			}
		}
		protected void ProcessTabMouseUp(MouseEventArgs e, BaseTabHitInfo hi, TabMiddleClickFiringMode mode) {
			if(ViewInfo == null) return;
			if(e.Button == MouseButtons.Middle) {
				if(ViewInfo.FireTabMiddleClickOnMouseUp && CheckMiddleClickLocation(e.Location)) {
					if((hi.Page != null) && hi.Page.PageEnabled)
						ViewInfo.OnTabMiddleClick(new PageEventArgs(hi.Page));
				} tabMiddleClickPoint = Point.Empty;
			}
		}
		bool CheckMiddleClickLocation(Point point) {
			return Math.Abs(tabMiddleClickPoint.X - point.X) < 2 && Math.Abs(tabMiddleClickPoint.Y - point.Y) < 2;
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			DXMouseEventArgs args = DXMouseEventArgs.GetMouseArgs(TabControl.OwnerControl, e);
			UpdateHotTrackedPage(args.Location);
			if(ViewInfo != null) ViewInfo.HeaderInfo.Buttons.ProcessEvent(new ProcessEventEventArgs(EventType.MouseEnter, args));
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.KeyCode == Keys.Tab && (e.KeyData & Keys.Control) != 0) {
				bool forward = (e.KeyData & Keys.Shift) == 0;
				TabControl.ViewInfo.SelectNextPage(forward ? 1 : -1);
				e.Handled = true;
			}
			if(e.KeyData == Keys.Left || e.KeyData == Keys.Right) {
				TabControl.ViewInfo.SelectNextPage(e.KeyData == Keys.Right ? 1 : -1);
				e.Handled = true;
			}
			if(e.KeyData == Keys.Home) {
				TabControl.ViewInfo.SelectFirstPage();
				e.Handled = true;
			}
			if(e.KeyData == Keys.End) {
				TabControl.ViewInfo.SelectLastPage();
				e.Handled = true;
			}
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			UpdateHotTrackedPage(new Point(-10000, -10000));
			if(ViewInfo != null) ViewInfo.HeaderInfo.Buttons.ProcessEvent(new ProcessEventEventArgs(EventType.MouseLeave, EventArgs.Empty));
			foreach(BaseTabPageViewInfo item in ViewInfo.HeaderInfo.AllPages) {
				item.ButtonsPanel.Handler.Reset();
			}
		}
		protected virtual void UpdateHotTrackedPage(Point pt) {
			if(ViewInfo == null) return;
			BaseTabHitInfo hitInfo = ViewInfo.CalcHitInfo(pt);
			ViewInfo.HotTrackedTabPage = hitInfo.Page;
		}
	}
}
