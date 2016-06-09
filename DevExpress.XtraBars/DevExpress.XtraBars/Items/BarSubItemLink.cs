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
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
using DevExpress.Utils.Drawing.Animation;
namespace DevExpress.XtraBars {
	public interface IPopup {
		IPopup ParentPopup { get; set; }
		void ClosePopup();
		void OpenPopup(LocationInfo locInfo, IPopup parentPopup);
		bool IsPopupOpened { get; }
		bool CanShowPopup { get; }
		Rectangle Bounds { get; }
		Rectangle PopupOwnerRectangle { get; }
		Rectangle PopupChildForceBounds { get; }
		object PopupCreator { get; }
		BarItemLink OwnerLink { get; }
		CustomControl CustomControl { get;}
		bool CanOpenAsChild(IPopup popup);
		bool ContainsLink(BarItemLink link);
		bool LockHighlight(BarItemLink newLink);
		bool CanHighlight(BarItemLink link);
		bool CanCloseByTimer { get; }
		Form PopupForm { get; }
		void OnMouseWheel(MouseEventArgs e);
		RibbonMiniToolbar RibbonToolbar { get; }
		bool IsControlContainer { get; }
	}
	public class BarCustomContainerItemLink : BarItemLink {
		int openTime = -1;
		bool allowClose = true;
		Timer fullMenuTimer;
		CustomPopupBarControl subControl;
		internal bool allowOpenEmpty, allowCloseTimer;
		protected BarCustomContainerItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject)
			: base(ALinks, AItem, ALinkedObject) {
		}
		protected virtual bool AllowMenuTimers {
			get { return true; }
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.XtraBars.Accessible.BaseMenuLinkAccessible(this);
		}
		protected int OpenTime { get { return openTime; } }
		protected bool AllowClose { get { return allowClose; } set { allowClose = value; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarCustomContainerItemLinkItem")]
#endif
		public new virtual BarCustomContainerItem Item { get { return base.Item as BarCustomContainerItem; } }
		protected override void InitVars() {
			fullMenuTimer = new Timer();
			fullMenuTimer.Interval = 2000;
			fullMenuTimer.Tick += new EventHandler(OnFullMenuOpen);
			allowCloseTimer = true;
			allowOpenEmpty = false;
			subControl = null;
			base.InitVars();
		}
		void OnFullMenuOpen(object sender, EventArgs e) {
			fullMenuTimer.Stop();
			if(Manager == null) return;
			if(Manager.SelectionInfo.HighlightedLink == this && Opened && !Manager.ShowFullMenus && Manager.ShowFullMenusAfterDelay && !Manager.SelectionInfo.ShowNonRecentItems) {
				bool needUpdate = false;
				foreach(BarItemLink link in VisibleLinks) {
					needUpdate |= IsNotRecentLink(link);
					if(needUpdate) break;
				}
				if(needUpdate) {
					Manager.SelectionInfo.ShowNonRecentItems = true;
					UpdateVisibleLinks();
				}
			}
		}
		protected override bool ActDoubleClickAsSingle { get { return false; } }
		protected internal virtual bool AllowOpenEmpty {
			get { return allowOpenEmpty; }
			set {
				if(value == AllowOpenEmpty) return;
				allowOpenEmpty = value;
				OnLinkChanged();
			}
		}
		internal CustomPopupBarControl SubControl {
			get { return subControl; }
			set {
				if(SubControl != value) {
					if(SubControl != null && !SubControl.Destroying) {
						this.openTime = -1;
						Control bc = SubControl;
						subControl = null;
						bc.Dispose();
						Item.OnCloseUp();
					}
					subControl = value;
					if(SubControl != null) {
						this.openTime = System.Environment.TickCount;
						SubControl.Init();
						Item.OnPopup();
					}
				}
			}
		}
		protected virtual bool OpenSubToLeft {
			get {
				if(Manager.GetPopupMenuAlignment() == LeftRightAlignment.Left) return true;
				IPopup popup = BarControl as IPopup;
				if(popup != null && popup.OwnerLink != null) {
					BarLinkViewInfo info = popup.OwnerLink.LinkViewInfo;
					Rectangle linkBounds = new Rectangle(popup.OwnerLink.LinkPointToScreen(info.Bounds.Location), info.Bounds.Size);
					if(popup.Bounds.Right <= linkBounds.Left) {
						return true;
					}
				}
				return false;
			}
		}
		protected LocationInfo CalcLocationInfo() {
			BarLinkViewInfo li = RequestLinkViewInfo();
			if(li == null) return null;
			Rectangle r = li.Bounds;
			r.Location = LinkPointToScreen(r.Location);
			if(IsLinkInMenu)
				return PopupOpenHelper.CalcInMenuLocationInfo(li, r, BarControl, SubControl);
			return CalcOnBarLocationInfo(li, r);
		}
		protected virtual LocationInfo CalcOnBarLocationInfo(BarLinkViewInfo li, Rectangle linkBounds) {
			int x = linkBounds.X;
			bool openXBack = false;
			if(Manager.GetPopupMenuAlignment() == LeftRightAlignment.Left) {
				x = linkBounds.Right;
				openXBack = true;
			}
			if(BarControl != null && BarControl.IsVertical) {
				return new LocationInfo(new Point(linkBounds.X + linkBounds.Width, linkBounds.Y), linkBounds.Location, false, false);
			}
			return new LocationInfo(new Point(x, linkBounds.Y + linkBounds.Height), new Point(x, linkBounds.Location.Y), true, openXBack);
		}
		IPopup GetPopup() {
			BarItemLink link = LinkedObject as BarItemLink;
			if(link != null) return link.BarControl as IPopup;
			return BarControl as IPopup;
		}
		protected virtual void OpenCore(bool setOpened) {
			if(RadialMenu != null) {
				RadialMenu.SetLinksHolder(Item as BarLinksHolder);
				return;
			}
			if(BarControl == null && RibbonItemInfo == null) return;
			if(setOpened == Opened) return;
			if(Enabled && setOpened) {
				Item.OnGetItemData();
			}
			if(!CanOpenMenu && setOpened) return;
			if(!setOpened) {
				Manager.SelectionInfo.ClosePopup(SubControl);
			}
			else {
				SubControl = CreateSubControl();
				if(CalcLocationInfo() == null) {
					SubControl = null;
					Invalidate();
					return;
				}
				if(Manager.SelectionInfo.CanOpenPopup(SubControl)) {
					OnCreateForm();
				}
				Manager.SelectionInfo.OpenPopup(GetPopup(), SubControl, CalcLocationInfo(), null);
				if(Opened) {
					if(Bar != null && Bar.AutoPopupMode != BarAutoPopupMode.None)
						Manager.SelectionInfo.AutoOpenMenuBar = Bar;
					if(AllowMenuTimers) fullMenuTimer.Start();
				}
			}
		}
		protected virtual CustomPopupBarControl CreateSubBarControl() {
			return new SubMenuBarControl(Manager, this);
		}
		protected virtual CustomPopupBarControl CreateSubControl() {
			CustomPopupBarControl sc = CreateSubBarControl();
			sc.Init();
			sc.UpdateVisibleLinks();
			if(sc.VisibleLinks.Count == 0 && (
				!Manager.IsCustomizing && !Manager.Helper.CustomizationManager.IsHotCustomizing)) {
				sc.Dispose();
				return null;
			}
			sc.PopupClose += new EventHandler(PopupCloseHandler);
			return sc;
		}
		protected virtual void OnCreateForm() {
			SubMenuControlForm form = null;
			if(SubControl == null) return;
			form = Manager.SelectionInfo.GetLastOpenedPopupForm();
			if(Manager.GetPopupShowMode(SubControl) == PopupShowMode.Inplace && form != null) {
				ShowInplaceMenuAnimated(form);
				return;
			}
			form = new SubMenuControlForm(Manager, SubControl, FormBehavior.SubMenu);
			SubControl.Form = form;
		}
		protected void ShowInplaceMenuAnimated(SubMenuControlForm form) {
			XtraAnimator.Current.AddAnimation(new SubMenuInplaceAnimationInfo(form, SubControl, false));
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarCustomContainerItemLinkOpened")]
#endif
		public virtual bool Opened {
			get { return SubControl != null; }
			set {
				if(value == Opened) return;
				OpenCore(value);
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarCustomContainerItemLinkCanOpenMenu")]
#endif
		public virtual bool CanOpenMenu {
			get { return Enabled && !IsLinkInMenu && (Item.ItemLinks.Count > 0 || AllowOpenEmpty) && (!Manager.IsCustomizing || AlwaysWorking); }
		}
		public void OpenMenu() {
			OpenCore(true);
		}
		public void CloseMenu() {
			OpenCore(false);
		}
		protected override bool CanStartTimerCore {
			get { return CanOpenMenu && IsLinkInMenu; }
		}
		protected override void OnTimerClose() {
			Opened = false;
		}
		protected override void OnTimerOpen() {
			Opened = true;
		}
		protected override bool TimerIsOpened { get { return Opened; } }
		protected override IPopup TimerPopup { get { return SubControl; } }
		protected virtual void PopupCloseHandler(object sender, EventArgs e) {
			SubControl = null;
		}
		protected internal override bool ContainsSubItemLink(BarItemLink link, int level) {
			if(VisibleLinks == null || level > 10) return false;
			if(link.ClonedFromLink != null) link = link.ClonedFromLink;
			BarItemLinkReadOnlyCollection links = VisibleLinks;
			if(links.Contains(link)) return true;
			for(int n = 0; n < links.Count; n++) {
				if(links[n] == link) return true; 
				if(links[n].ContainsSubItemLink(link, level + 1)) return true;
			}
			return false;
		}
		public override void Dispose() {
			fullMenuTimer.Tick -= new EventHandler(OnFullMenuOpen);
			fullMenuTimer.Dispose();
			Opened = false;
			base.Dispose();
		}
		protected internal override bool NeedMouseCapture { get { return false; } }
		protected override bool ShouldCloseAllPopups {
			get {
				if(!Opened && base.ShouldCloseAllPopups) return true;
				return false;
			}
		}
		protected override void OnLinkAction(BarLinkAction action, object actionArgs) {
			switch(action) {
				case BarLinkAction.MouseDoubleClick:
					if(!IsLinkInMenu) {
						if(Opened) {
							OnFullMenuOpen(this, EventArgs.Empty);
						}
						else {
							Manager.SelectionInfo.ShowNonRecentItems = true;
							Opened = true;
						}
					}
					break;
				case BarLinkAction.MouseClick:
					if(!IsLinkInMenu && Opened && AllowClose) {
						if(System.Environment.TickCount - OpenTime > SystemInformation.DoubleClickTime) {
							Opened = false;
							Manager.SelectionInfo.HighlightedLink = this;
						}
					}
					break;
				case BarLinkAction.PressArrow:
				case BarLinkAction.Press:
					if(!Opened) {
						Manager.SelectionInfo.AllowAnimation = (Manager.SelectionInfo.ActiveBarControl == null && Manager.SelectionInfo.AutoOpenMenuBar == null);
						Opened = true;
						if(Opened) {
							Manager.SelectionInfo.ActiveBarControl = SubControl;
							AllowClose = false;
						}
					}
					else {
						AllowClose = true;
						if(!IsLinkInMenu) {
							if(action == BarLinkAction.PressArrow) {
								Opened = false;
								Manager.SelectionInfo.HighlightedLink = this;
							}
						}
						else {
							if(Opened) Manager.SelectionInfo.ActiveBarControl = Manager.SelectionInfo.OpenedPopups.LastPopup.CustomControl;
						}
					}
					break;
				case BarLinkAction.UnHighlight:
					fullMenuTimer.Stop();
					break;
				case BarLinkAction.KeyboardHighlight:
					Manager.SelectionInfo.AllowAnimation = false;
					if(!Opened) {
						if(Bar != null && Manager.SelectionInfo.AutoOpenMenuBar == Bar) {
							Opened = true;
						}
					}
					if(Opened && SubControl != null && action == BarLinkAction.KeyboardHighlight) {
						Manager.SelectionInfo.MoveLinkSubSelectionVertical(SubControl.VisibleLinks, BarLinkNavigation.First);
					}
					break;
				case BarLinkAction.Highlight:
					if(!Opened) {
						if(Bar != null && Manager.SelectionInfo.AutoOpenMenuBar == Bar) {
							Opened = true;
						}
					}
					break;
				case BarLinkAction.StartDrag:
					Opened = false;
					break;
				case BarLinkAction.DropSelect:
					if(Manager.SelectionInfo.CustomizeSelectedLink != this) {
						if(Manager.SelectionInfo.SelectedItem != null && Manager.Helper.DragManager.CheckCanDropIntoItem(Item, Manager.SelectionInfo.SelectedItem)) {
							if(Manager.SelectionInfo.SelectedItem != Item && !Item.IsPrivateItem && CanSelectInCustomization)
								Opened = true;
						}
					}
					break;
				case BarLinkAction.KeyClick:
					Manager.SelectionInfo.AllowAnimation = false;
					Opened = true;
					if(Opened && SubControl != null) {
						Manager.SelectionInfo.MoveLinkSubSelectionVertical(SubControl.VisibleLinks, BarLinkNavigation.First);
					}
					break;
			}
			base.OnLinkAction(action, actionArgs);
		}
		protected internal override bool IsNeedMouseCapture { get { return false; } }
		protected internal override bool IsNeededKey(KeyEventArgs e) {
			if(Enabled && CanOpenMenu) {
				if(IsLinkInMenu && e.KeyData == Keys.Right) return true;
				if(!IsLinkInMenu && e.KeyCode == Keys.Down) return true;
			}
			return base.IsNeededKey(e);
		}
		protected internal override void ProcessKey(KeyEventArgs e) {
			if(IsLinkInMenu && (e.KeyData == Keys.Right || e.KeyData == Keys.F4)) {
				IHasRibbonKeyTipManager hasKeyTip = Holder as IHasRibbonKeyTipManager;
				if(hasKeyTip != null && hasKeyTip.KeyTipManager != null && hasKeyTip.KeyTipManager.Show == true)
					hasKeyTip.KeyTipManager.OnKeyTipSelected(this);
				else
					OnLinkAction(BarLinkAction.KeyClick, null);
				return;
			}
			if(!IsLinkInMenu && (e.KeyCode == Keys.Down || e.KeyData == Keys.F4)) {
				OnLinkAction(BarLinkAction.KeyClick, null);
				return;
			}
			base.ProcessKey(e);
		}
		protected internal virtual void UpdateVisibleLinks() {
			if(SubControl != null) SubControl.LayoutChanged();
		}
		protected internal override void LayoutChanged() {
			UpdateVisibleLinks();
			base.LayoutChanged();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarCustomContainerItemLinkDisplayCaption")]
#endif
		public override string DisplayCaption {
			get {
				string result = Caption;
				if(IsLinkInMenu) return "(" + result + ")";
				return result;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarCustomContainerItemLinkVisibleLinks")]
#endif
		public virtual BarItemLinkReadOnlyCollection VisibleLinks {
			get { return ((BarCustomContainerItem)Item).ItemLinks; }
		}
		protected internal virtual BarItemLinkReadOnlyCollection InplaceLinks { get { return null; } }
		protected internal virtual IList ReallyVisibleLinks {
			get { return VisibleLinks; }
			set {
			}
		}
		protected internal virtual bool IsNotRecentLink(BarItemLink link) {
			if(Manager.ShowFullMenus) return false;
			return VisibleLinks.Contains(link) && !ReallyVisibleLinks.Contains(link);
		}
		protected override bool RibbonIsDroppedDown { get { return Opened; } }
		protected override bool IsCommandItemCore { get { return false; } }
		protected internal override void KeyTipItemClick() {
			OnLinkAction(BarLinkAction.KeyClick, Keys.Space);
			IHasRibbonKeyTipManager parentKeyTipHolder = Holder as IHasRibbonKeyTipManager;
			if(SubControl == null) return;
			if(parentKeyTipHolder != null) {
				SubControl.KeyTipManager.Parent = parentKeyTipHolder.KeyTipManager;
				parentKeyTipHolder.KeyTipManager.HideKeyTips();
			}
			else SubControl.KeyTipManager.Parent = null;
			SubControl.KeyTipManager.Ribbon = Ribbon;
			SubControl.KeyTipManager.ActivateKeyTips();
		}
		public DefaultBoolean ShowNavigationHeader { get { return Item.ShowNavigationHeader; } }
	}
	public class BarListItemLink : BarCustomContainerItemLink {
		protected BarListItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {
		}
		protected override bool CanStartTimerCore { get { return false; } }
		protected internal override BarItemLinkReadOnlyCollection InplaceLinks {
			get { 
				if(IsLinkInMenu && (!Manager.IsCustomizing || AlwaysWorking) && VisibleLinks != null) {
					BarItemLinkReadOnlyCollection links = new BarItemLinkReadOnlyCollection();
					BarListItem owner = Item as BarListItem;
					foreach(BarItemLink link in VisibleLinks) {
						BarItemLink newLink = ((ICloneable)link).Clone() as BarItemLink;
						newLink.MaxLinkTextWidth = owner == null ? 0 : owner.MaxSubItemTextWidth;
						newLink.OwnerListItemLink = this;
						links.AddItem(newLink);
					}
					return links;
				}
				return null;
			}
		}
	}
	public class BarMdiChildrenListItemLink : BarListItemLink {
		protected BarMdiChildrenListItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarMdiChildrenListItemLinkCanOpenMenu")]
#endif
		public override bool CanOpenMenu {
			get { return Enabled && !IsLinkInMenu && !Manager.IsCustomizing && ((BarMdiChildrenListItem)Item).MdiChildren != null && ((BarMdiChildrenListItem)Item).MdiChildren.Length > 0; }
		}
	}
	public class BarDockingMenuItemLink : BarListItemLink {
		protected BarDockingMenuItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject)
			: base(ALinks, AItem, ALinkedObject) {
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarDockingMenuItemLinkCanOpenMenu")]
#endif
		public override bool CanOpenMenu {
			get {
				return Enabled && !IsLinkInMenu && !Manager.IsCustomizing && 
					((BarDockingMenuItem)Item).GetDocuments() != null;
			}
		}
	}
	public class BarWorkspaceMenuItemLink : BarListItemLink {
		protected BarWorkspaceMenuItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject)
			: base(ALinks, AItem, ALinkedObject) {
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarDockingMenuItemLinkCanOpenMenu")]
#endif
		public override bool CanOpenMenu {
			get { return Enabled && !IsLinkInMenu && !Manager.IsCustomizing; }
		}
	}
	public class BarLinkContainerItemLink : BarCustomContainerItemLink {
		internal BarLinkContainerItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) { }
		protected override bool CanStartTimerCore { get { return false; }	}
		protected internal override BarItemLinkReadOnlyCollection InplaceLinks {
			get { 
				if(GetAllowInplaceLinks()) {
					BarItemLinkReadOnlyCollection links = new BarItemLinkReadOnlyCollection();
					foreach(BarItemLink link in VisibleLinks) {
						BarItemLink cloned = (BarItemLink)((ICloneable)link).Clone();
						UpdateLinkedObject(cloned);
						links.AddItem(cloned);
					}
					if(Holder != null) {
						foreach(BarItemLink link in links) {
							link.EnabledLinkCore = Holder.Enabled;
						}
					}
					return links;
				}
				return null;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarLinkContainerItemLinkCanOpenMenu")]
#endif
		public override bool CanOpenMenu {
			get { return (Enabled && ((BarCustomContainerItem)Item).CanOpenMenu && ((BarCustomContainerItem)Item).ItemLinks.Count > 0 || AllowOpenEmpty) || Manager.IsCustomizing; }
		}
		protected virtual bool GetAllowInplaceLinks() {
			return (IsLinkInMenu && (!Manager.IsCustomizing || AlwaysWorking) && VisibleLinks != null);
		}
		protected virtual void UpdateLinkedObject(BarItemLink inplaceLink) { }
	}
	public class BarLinkContainerExItemLink : BarLinkContainerItemLink {
		protected BarLinkContainerExItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) { }
		protected override bool GetAllowInplaceLinks() {
			return !Manager.IsCustomizing && VisibleLinks != null;
		}
		protected override void UpdateLinkedObject(BarItemLink inplaceLink) { 
			if(Bar != null) inplaceLink.linkedObject = Bar;
		}
		public override bool CanOpenMenu {
			get { return Enabled && !IsLinkInMenu && (Item.ItemLinks.Count > 0 || AllowOpenEmpty) && (!Manager.IsCustomizing || AlwaysWorking); }
		}
	}
	public class BarToolbarsListItemLink : BarLinkContainerItemLink {
		protected BarToolbarsListItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {	}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarToolbarsListItemLinkCanOpenMenu")]
#endif
		public override bool CanOpenMenu {
			get { return Enabled && !IsLinkInMenu && (Item.ItemLinks.Count > 0 || AllowOpenEmpty) && (!Manager.IsCustomizing || AlwaysWorking); }
		}
	}
	#region BarSubItemLink
	public class BarSubItemLink : BarLinkContainerItemLink {
		IList recentLinks;
		protected BarSubItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {
			recentLinks = new ArrayList();
		}
		protected override bool CanStartTimerCore { 
			get { 
				return CanOpenMenu && (IsLinkInMenu || BarControl is QuickCustomizationBarControl); 
			} 
		}
		protected internal override ToolTipControlInfo GetToolTipInfo(Point point) {
			if(Opened)
				return new ToolTipControlInfo();
			return base.GetToolTipInfo(point);
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarSubItemLinkDisplayCaption")]
#endif
		public override string DisplayCaption {
			get { return this.Caption; }
		}
		protected internal override BarItemLinkReadOnlyCollection InplaceLinks {
			get { return null; }
		}
		protected internal override IList ReallyVisibleLinks {
			get { return recentLinks; }
			set {
				if(value == null) {
					recentLinks.Clear();
					return;
				}
				recentLinks = value;
			}
		}
		public override bool CanVisible {
			get {
				BarSubItem subItem = Item as BarSubItem;
				if(subItem != null && !subItem.InplaceVisible) return false;
				return base.CanVisible;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarSubItemLinkCanShowSuperTip")]
#endif
public override bool CanShowSuperTip { get { return !this.Opened; } }
	}
	#endregion BarSubItemLink
	#region BarButtonGroupLink
	public class BarButtonGroupLink : BarLinkContainerExItemLink {
		protected BarButtonGroupLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject)
			: base(ALinks, AItem, ALinkedObject) {
		}
		protected override IRibbonItem[] RibbonGetButtonGroupChildren() {
			if(VisibleLinks.Count == 0) return null;
			BarItemLinkReadOnlyCollection inplaceLinks = InplaceLinks;
			IRibbonItem[] items = new IRibbonItem[inplaceLinks.Count];
			inplaceLinks.CopyTo(items);
			if(Holder != null) {
				foreach(BarItemLink link in items) {
					link.EnabledLinkCore = Holder.Enabled;
				}
			}
			return items; 
		}
		protected override bool RibbonIsButtonGroup { get { return true; } }
		protected override bool GetAllowInplaceLinks() {
			return VisibleLinks != null;
		}
		[Browsable(false)]
		public override bool GetActAsButtonGroupDefault() { return true; }
	}
	#endregion BarButtonGroupLink
	#region SkinBarSubItemLink
	public class SkinBarSubItemLink : BarSubItemLink {
		public SkinBarSubItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject)
			: base(ALinks, AItem, ALinkedObject) {
		}
		public override bool CanOpenMenu {
			get {
				if(Manager.IsDesignMode)
					return false;
				return base.CanOpenMenu;
			}
		}
	}
	#endregion
}
