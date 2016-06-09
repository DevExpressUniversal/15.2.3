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
using System.Linq;
using System.Text;
using DevExpress.XtraBars.Design;
using System.Windows.Forms;
using DevExpress.XtraBars.InternalItems;
namespace DevExpress.XtraBars.Ribbon {
	public class RadialMenuWindowHandler {
		public RadialMenuWindowHandler(RadialMenuWindow window) {
			Window = window;
		}
		public RadialMenuWindow Window { get; private set; }
		public RadialMenuViewInfo ViewInfo { get { return Window.ViewInfo; } }
		public RadialMenu Menu { get { return Window.Menu; } }
		Cursor defaultCursor;
		public virtual void OnMouseLeave() {
			ViewInfo.HoverInfo = null;
			if(defaultCursor != null) {
				Cursor.Current = defaultCursor;
				defaultCursor = null;
			}
		}
		public virtual void OnMouseEnter() {
			if(defaultCursor == null) {
				defaultCursor = Cursor.Current;
				Cursor.Current = Cursors.Default;
			}
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			RadialMenuHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			if(ShouldProcessHoverInfo(hitInfo)) {
				ViewInfo.HoverInfo = hitInfo;
			}
		}
		bool ShouldProcessHoverInfo(RadialMenuHitInfo hitInfo) {
			return !ViewInfo.HoverInfo.Equals(hitInfo);
		}
		public virtual void OnMouseDown(MouseEventArgs e) {
			ViewInfo.PressedInfo = ViewInfo.CalcHitInfo(e.Location);
			ProcessPress();
		}
		protected virtual void ProcessPress() {
			if(ViewInfo.PressedInfo.HitTest == RadialMenuHitTest.Link) {
				ProcessLinkPress();
			}
		}
		protected virtual void ProcessLinkPress() {
			if(!(ViewInfo.PressedInfo.LinkInfo.Link.Item is BarLinksHolder))
				ViewInfo.PressedInfo.LinkInfo.Link.OnLinkActionCore(BarLinkAction.Press, null);
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			RadialMenuHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			if(ViewInfo.PressedInfo.Equals(hitInfo)) {
				ProcessClick();
			}
			ViewInfo.PressedInfo = null;
			ViewInfo.HoverInfo = hitInfo;
		}
		protected virtual void ProcessClick() {
			if(ViewInfo.PressedInfo.HitTest == RadialMenuHitTest.Glyph) {
				ProcessGlyphClick();
			} else if(ViewInfo.PressedInfo.HitTest == RadialMenuHitTest.Link) {
				ProcessLinkClick();
			} else if(ViewInfo.PressedInfo.HitTest == RadialMenuHitTest.LinkArrow) {
				ProcessLinkArrowClick();
			}
		}
		protected virtual void ProcessLinkArrowClick() {
			BarItemLink link = ViewInfo.PressedInfo.LinkInfo.Link;
			if(!CanProcessLinkArrowClick(link)) return;
			if(link.Item is BarLinksHolder) {
				ViewInfo.Menu.ActualLinksHolder = (BarLinksHolder)link.Item;
				if(!Menu.Manager.IsDesignMode)
					link.OnLinkActionCore(BarLinkAction.MouseClick, null);
			}
		}
		protected bool CanProcessLinkArrowClick(BarItemLink link) {
			if(Menu.Manager.IsDesignMode)
				return true;
			return link.Item.Enabled;
		}
		void SelectLinkInDesignTime(BarItemLink link) {
			if(link.Item is BarDesignTimeItem) return; 
			BarLinkInfoProvider.SetLinkInfo(link.Item, link);
			Menu.Manager.Helper.CustomizationManager.SelectObject(link.Item);
		}
		protected virtual void ProcessLinkClick() {
			BarItemLink link = ViewInfo.PressedInfo.LinkInfo.Link;
			if(Menu.Manager.IsDesignMode) {
				SelectLinkInDesignTime(ViewInfo.PressedInfo.LinkInfo.Link);
			} else {
				if(!CanProcessLinkClick(link)) return;
				if(link.Item is BarLinksHolder)
					ViewInfo.Menu.ActualLinksHolder = (BarLinksHolder)link.Item;
				if(ShouldPerformPostponedClick(link)) {
					this.postponedClickedLink = link;
					ViewInfo.Menu.CloseUp += OnRadialMenuCloseUpCore;
					ViewInfo.Menu.MakeCollapsed(true, true);
					return;
				}
				link.OnLinkActionCore(BarLinkAction.MouseClick, null);
			}
		}
		protected virtual bool ShouldPerformPostponedClick(BarItemLink link) {
			if(link == null) return false;
			BarButtonItem item = link.Item as BarButtonItem;
			if(item == null) return false;
			return item.CloseRadialMenuOnItemClick;
		}
		BarItemLink postponedClickedLink = null;
		protected virtual void OnRadialMenuCloseUpCore(object sender, EventArgs e) {
			ViewInfo.Menu.CloseUp -= OnRadialMenuCloseUpCore;
			if(postponedClickedLink == null || ViewInfo.Window == null) return;
			Control topControl = ViewInfo.Window.GetTopControl();
			if(topControl == null) return;
			topControl.BeginInvoke(new MethodInvoker(OnPostponedItemLinkClickCore));
		}
		protected virtual void OnPostponedItemLinkClickCore() {
			postponedClickedLink.OnLinkActionCore(BarLinkAction.MouseClick, null);
			postponedClickedLink = null;
		}
		protected bool CanProcessLinkClick(BarItemLink link) {
			return link.Item.Enabled;
		}
		protected virtual void ProcessGlyphClick() {
			if(Menu.State == RadialMenuState.Collapsed) {
				Menu.RaiseCenterButtonClick(Menu.ActualLinksHolder);
				Menu.MakeExpanded();
			}
			else {
				if(Menu.ActualLinksHolder == Menu) {
					Menu.RaiseCenterButtonClick(null);
					Menu.MakeCollapsed(true, true);
				}
				else {
					if(Menu.LinksHolderList.Count > 0) {
						BarLinksHolder next = Menu.LinksHolderList.Pop();
						Menu.RaiseCenterButtonClick(next);
						Menu.SetPrevHolder(next);
					}
				}
			}
		}
	}
}
