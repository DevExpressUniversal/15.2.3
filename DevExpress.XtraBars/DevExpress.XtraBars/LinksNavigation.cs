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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraBars.Controls {
	public abstract class LinksNavigation {
		public BarSelectionInfo SelectionInfo { get { return Manager.SelectionInfo; } }
		public abstract BarManager Manager { get; }
		public virtual bool IsNeededKey(KeyEventArgs e, BarItemLink activeLink) { return false; }
		public virtual bool ProcessKeyDown(KeyEventArgs e, BarItemLink activeLink) {
			if(activeLink != null && activeLink.IsNeededKey(e)) {
				if(e.Alt && Manager.ProcessLinkAccelerator(e.KeyCode)) return true;
				activeLink.ProcessKey(e);
				return true;
			}
			switch(e.KeyData) {
				case Keys.Escape:
					Escape();
					return true;
			}
			return false;
		}
		public virtual bool ProcessKeyUp(KeyEventArgs e, BarItemLink activeLink) { 
			if(activeLink != null && activeLink.IsNeededKey(e)) {
				activeLink.ProcessKeyUp(e);
				return true;
			}
			return false; 
		}
		protected virtual RibbonKeyTipManager KeyTipManager {
			get { return null; }
		}
		ContainerKeyTipManager ContainerKeyTipManager {
			get {
				CustomLinksControl bc = SelectionInfo.ActiveBarControl as CustomLinksControl;
				if(bc == null) return null;
				return bc.KeyTipManager;
			}
		}
		protected virtual bool IsRibbonKeyTipInterceptKey(Keys keys) {
			if(keys == Keys.Escape && KeyTipManager.KeyTipMode == RibbonKeyTipMode.PagesKeyTips) return false;
			return IsNavigationKey(keys);
		}
		protected virtual bool IsContainerKeyTipInterceptKey(Keys keys) {
			if(keys == Keys.Escape) return true;
			return IsNavigationKey(keys);
		}
		public virtual bool IsInterceptKey(KeyEventArgs e, BarItemLink activeLink) { 
			if(SelectionInfo.OpenedPopups.LastPopup is PopupContainerBarControl) return false;
			if(e.KeyData == (Keys.Alt | Keys.F4)) return false;
			if(activeLink != null && !activeLink.IsInterceptKey(e)) return false;
			if(ContainerKeyTipManager != null && ContainerKeyTipManager.ShouldCheckInterceptKey) {
				return IsContainerKeyTipInterceptKey(e.KeyData);
			}
			if(KeyTipManager != null && KeyTipManager.Show) {
				return IsRibbonKeyTipInterceptKey(e.KeyData);
			}
			return true; 
		}
		protected virtual void Escape() { }
		protected virtual bool IsNavigationKey(Keys key) { return false; }
	}
	public class LinksControlNavigation : LinksNavigation {
		CustomLinksControl linksControl;
		public LinksControlNavigation(CustomLinksControl linksControl) {
			this.linksControl = linksControl;
		}
		public virtual BarItemLinkReadOnlyCollection ReallyVisibleLinks { get { return LinksControl.GetVisibleLinks(); } }
		public CustomLinksControl LinksControl { get { return linksControl; } }
		public override BarManager Manager { get { return LinksControl.Manager; } }
	}
	public class HorizontalLinksNavigation : LinksControlNavigation { 
		public HorizontalLinksNavigation(CustomLinksControl linksControl) : base(linksControl) { }
		Keys[] neededKeys = new Keys[] { Keys.Tab | Keys.Control, Keys.Tab | Keys.Shift, Keys.Tab, Keys.Down, Keys.Up, Keys.Left, Keys.Right, Keys.Escape, Keys.Enter, Keys.End, Keys.Home, Keys.Space};
		public override bool IsNeededKey(KeyEventArgs e, BarItemLink activeLink) { 
			if(activeLink != null && activeLink.IsNeededKey(e)) {
				return true;
			}
			BarItemLink link = LinksControl.FindAcceleratedLink(e);
			if(link != null) return true;
			return Array.IndexOf(neededKeys, e.KeyData) != -1;
		}
		public override bool ProcessKeyDown(KeyEventArgs e, BarItemLink activeLink) {
			if(base.ProcessKeyDown(e, activeLink)) return true;
			switch(e.KeyData) {
				case Keys.Tab | Keys.Control:
					Manager.SelectionInfo.MoveBarSelection();
					return true;
				case Keys.Tab | Keys.Shift:
				case Keys.Tab:
					MoveLinkSelectionHorizontal(e.KeyData == Keys.Tab ? BarLinkNavigation.Right : BarLinkNavigation.Left, Keys.Tab);
					return true;
				case Keys.Left:
				case Keys.Right:
					MoveLinkSelectionHorizontal(e.KeyData == Keys.Right ? BarLinkNavigation.Right : BarLinkNavigation.Left, e.KeyCode);
					return true;
				case Keys.Home:
				case Keys.End:
					MoveLinkSelectionHorizontal(e.KeyData == Keys.End ? BarLinkNavigation.Last : BarLinkNavigation.First, Keys.None);
					return true;
				default:
					BarItemLink link = LinksControl.FindAcceleratedLink(e);
					if(link != null) {
						link.OnLinkActionCore(BarLinkAction.KeyClick, null);
						return true;
					}
					break;
			}
			return false;
		}
		protected override void Escape() {
			Manager.SelectionInfo.Clear();
		}
		protected virtual void MoveLinkSelectionHorizontal(BarLinkNavigation nav, Keys key) {
			if(Manager.IsCustomizing) return;
			Manager.SelectionInfo.MoveLinkSelectionHorizontal(ReallyVisibleLinks, nav, key);
		}
	}
	public class QuickCustomizingNavigation : LinksControlNavigation { 
		LinksNavigation vertical, horizontal;
		public QuickCustomizingNavigation(CustomLinksControl linksControl) : base(linksControl) {
			this.vertical = new VerticalLinksNavigation(LinksControl);
			this.horizontal = new HorizontalLinksNavigation(LinksControl);
		}
		public LinksNavigation Vertical { get { return vertical; } }
		public LinksNavigation Horizontal { get { return horizontal; } }
		public virtual LinksNavigation Current(BarItemLink activeLink) {
			if(activeLink is BarQMenuAddRemoveButtonsItemLink) return Vertical;
			return Horizontal;
		}
		public override bool IsNeededKey(KeyEventArgs e, BarItemLink activeLink) { 
			return Current(activeLink).IsNeededKey(e, activeLink);
		}
		public override bool IsInterceptKey(KeyEventArgs e, BarItemLink activeLink) { 
			return Current(activeLink).IsInterceptKey(e, activeLink);
		}
		public override bool ProcessKeyDown(KeyEventArgs e, BarItemLink activeLink) {
			return Current(activeLink).ProcessKeyDown(e, activeLink);
		}
		public override bool ProcessKeyUp(KeyEventArgs e, BarItemLink activeLink) {
			return Current(activeLink).ProcessKeyUp(e, activeLink);
		}
	}
	public class VerticalLinksNavigation : LinksControlNavigation { 
		public VerticalLinksNavigation(CustomLinksControl linksControl) : base(linksControl) {
		}
		Keys[] neededKeys = new Keys[] { Keys.Tab | Keys.Control, Keys.Tab | Keys.Shift, Keys.Tab, Keys.Down, Keys.Up, Keys.Left, Keys.Right, Keys.Escape, Keys.Enter, Keys.End, Keys.Home, Keys.PageUp, Keys.PageDown};
		public override bool IsNeededKey(KeyEventArgs e, BarItemLink activeLink) { 
			if(activeLink != null && activeLink.IsNeededKey(e)) {
				return true;
			}
			if(LinksControl.KeyTipManager.Show) {
				return IsContainerKeyTipInterceptKey(e.KeyData);
			}
			BarItemLink link = LinksControl.FindAcceleratedLink(e);
			if(link != null) return true;
			return Array.IndexOf(neededKeys, e.KeyData) != -1 || e.KeyData == Keys.F4;
		}
		protected override bool IsNavigationKey(Keys key) {
			return Array.IndexOf(neededKeys, key) != -1;
		}
		public virtual IPopup Popup { get { return LinksControl as IPopup; } }
		protected virtual bool ShouldHideKeyTipsOnKeyDown(KeyEventArgs e, BarItemLink activeLink) {
			if (LinksControl == null || !LinksControl.KeyTipManager.Show) return false;
			return IsNeededKey(e, activeLink);
		}
		public override bool ProcessKeyDown(KeyEventArgs e, BarItemLink activeLink) {
			if(base.ProcessKeyDown(e, activeLink)) return true;
			if(ShouldHideKeyTipsOnKeyDown(e, activeLink)) {
				LinksControl.KeyTipManager.HideKeyTips();
			}
			switch(e.KeyData) {
				case Keys.Tab | Keys.Control:
					Manager.SelectionInfo.MoveBarSelection();
					return true;
				case Keys.Tab | Keys.Shift:
				case Keys.Tab:
					MoveLinkSelectionVertical(e.KeyData == Keys.Tab ? BarLinkNavigation.Down : BarLinkNavigation.Up);
					return true;
				case Keys.Left:
				case Keys.Right:
					MoveLinkSelectionHorizontal(e.KeyData == Keys.Right ? BarLinkNavigation.Right : BarLinkNavigation.Left);
					return true;
				case Keys.Home:
				case Keys.End:
					MoveLinkSelectionVertical(e.KeyData == Keys.End ? BarLinkNavigation.Last : BarLinkNavigation.First);
					return true;
				case Keys.Up:
				case Keys.Down:
					MoveLinkSelectionVertical(e.KeyData == Keys.Down ? BarLinkNavigation.Down : BarLinkNavigation.Up);
					return true;
				case Keys.PageUp:
					MoveLinkSelectionVertical(BarLinkNavigation.PageUp);
					return true;
				case Keys.PageDown:
					MoveLinkSelectionVertical(BarLinkNavigation.PageDown);
					return true;
				default:
					BarItemLink link = LinksControl.FindAcceleratedLink(e);
					if(link != null) {
						link.OnLinkActionCore(BarLinkAction.KeyClick, null);
						return true;
					}
					break;
			}
			return false;
		}
		protected override void Escape() {
			if(Popup == null) return;
			BarItemLink ownerLink = Popup.OwnerLink;
			BarSubItemLink subItemLink = ownerLink as BarSubItemLink;
			bool showKeyTip = LinksControl != null && LinksControl.KeyTipManager.Show;
			Manager.SelectionInfo.ClosePopup(Popup);
			if(Popup.RibbonToolbar != null)
				Popup.RibbonToolbar.Hide();
			if(Manager.SelectionInfo.ActiveBarControl != null) {
				Manager.SelectionInfo.KeyboardHighlightedLink = ownerLink;
			}
			if(showKeyTip) {
				LinksControl.KeyTipManager.HideKeyTips();
				LinksControl.KeyTipManager.ActivateParentKeyTips();
			}
		}
		protected virtual void MoveLinkSelectionHorizontal(BarLinkNavigation nav) {
			if(Manager.IsCustomizing || Popup == null) return;
			Manager.SelectionInfo.MoveLinkSubSelectionHorizontal(Popup, nav);
		}
		protected virtual void MoveLinkSelectionVertical(BarLinkNavigation nav) {
			if(Manager.IsCustomizing || ReallyVisibleLinks.Count == 0) return;
			Manager.SelectionInfo.MoveLinkSubSelectionVertical(ReallyVisibleLinks, nav);
		}
	}
}
