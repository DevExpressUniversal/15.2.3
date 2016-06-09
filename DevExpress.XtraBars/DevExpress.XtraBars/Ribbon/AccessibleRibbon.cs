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
using System.Collections;
using System.Windows.Forms;
using DevExpress.Accessibility;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Accessible;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.ViewInfo;
namespace DevExpress.XtraBars.Ribbon.Accessible {
	public class RibbonStatusBarAccessible : BaseControlAccessible { 
		RibbonStatusBar statusBar;
		public RibbonStatusBarAccessible(RibbonStatusBar statusBar) : base(statusBar) {
			this.statusBar = statusBar;
			CreateCollection();
		}
		public override AccessibleObject Parent { get { return StatusBar.Parent.AccessibilityObject; } }
		public RibbonStatusBar StatusBar { get { return statusBar; } set { statusBar = value; } }
		public override Control GetOwnerControl() { return StatusBar; }
		public override Rectangle ClientBounds { get { return StatusBar.Bounds; } }
		protected override string GetName() { return "Status Bar"; }
		protected override AccessibleRole GetRole() { return AccessibleRole.PropertyPage; }
		protected override AccessibleStates GetState() { return AccessibleStates.None; }
		public override string Value { get { return GetName(); } }  
		protected override ChildrenInfo GetChildrenInfo() {
			return new ChildrenInfo(ChildType.Item, StatusBar.ItemLinks.Count);
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			foreach(BarItemLink link in StatusBar.ItemLinks)
				AccessibleLinkHelper.Current.AddLink(Children, link, this);
		}
	}
	public class GalleryDropDownBarControlAccessible : CustomLinksContainerAccessible {
		public GalleryDropDownBarControlAccessible(GalleryDropDownBarControl dropdown) : base(dropdown) { }
		public GalleryDropDownBarControl DropDownBar { get { return Control as GalleryDropDownBarControl; } }
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo info = base.GetChildrenInfo();
			info = new ChildrenInfo(ChildType.Item, info.Count + 1);
			return info;
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			if(Children == null) CreateCollection();
			AccessibleRibbon ribbon = DropDownBar.Gallery.Ribbon == null ? null : DropDownBar.Gallery.Ribbon.AccessibleRibbon;
			Children.Insert(0, new InDropDownGalleryAccessible(ribbon, DropDownBar.Gallery));
		}
	}
	public class AccessibleLinkHelper {
		static AccessibleLinkHelper current;
		static AccessibleLinkHelper() { 
			current = new AccessibleLinkHelper();
		}
		public static AccessibleLinkHelper Current { get { return current; } }
		public void AddLink(BaseAccessibleCollection coll, BarItemLink link, BaseAccessible parentAcc) {
			if(link is BarButtonGroupLink) {
				RibbonButtonGroupItemViewInfo grInfo = link.RibbonItemInfo as RibbonButtonGroupItemViewInfo;
				if(grInfo == null) return;
				foreach(RibbonItemViewInfo itemInfo in grInfo.Items) {
					BarItemLink btnLink = itemInfo.Item as BarItemLink;
					AddLink(coll, btnLink, parentAcc);
					btnLink.DXAccessible.ParentCore = parentAcc;
				}
				return;
			}
			coll.Add(link.DXAccessible);
			link.DXAccessible.ParentCore = parentAcc;	
		}
	}
	public class BaseRibbonAccessible : BaseAccessible { 
		AccessibleRibbon ribbon;
		public BaseRibbonAccessible(AccessibleRibbon ribbon) : base() {
			this.ribbon = ribbon;
		}
		public AccessibleRibbon Ribbon { get { return ribbon; } set { ribbon = value; } }
		public override AccessibleObject Parent { get { return Ribbon.Accessible; } }
		public void AddLink(BarItemLink link) {
			AccessibleLinkHelper.Current.AddLink(Children, link, this); 
		}
		public int GetLinksCount(BarItemLinkCollection links) { 
			int linkCount = 0;
			for(int i = 0; i < links.Count; i++) {
				if(links[i] is BarButtonGroupLink) linkCount += (links[i].Item as BarButtonGroup).ItemLinks.Count;
				else linkCount++;
			}
			return linkCount;
		}
		public NavigationObject KeyboardActiveObject { get { return RibbonViewInfo.KeyboardActiveObject; } }
		public RibbonViewInfo RibbonViewInfo { get { return Ribbon.Ribbon.ViewInfo; } }
		public override Control GetOwnerControl() { return Ribbon.Ribbon; }
	}
	public class AccessiblePageGroupButton : BaseRibbonAccessible { 
		AccessiblePageGroup pageGroup;
		public AccessiblePageGroupButton(AccessibleRibbon ribbon, AccessiblePageGroup pageGroup) : base(ribbon) {
			this.pageGroup = pageGroup;
		}
		public AccessiblePageGroup PageGroup { get { return pageGroup; } }
		public RibbonPageGroupViewInfo PageGroupInfo { get { return PageGroup.PageGroupInfo; } }
		public override AccessibleObject Parent { get { return PageGroup.Accessible; } }
		protected virtual bool IsEqualGroups(NavigationObject obj) {
			NavigationObjectPageGroupButton btn = obj as NavigationObjectPageGroupButton;
			if(btn != null && btn.PageGroup == PageGroup.PageGroup) return true;
			NavigationObjectRibbonPageGroupItem item = obj as NavigationObjectRibbonPageGroupItem;
			if(item != null && item.PageGroup == PageGroup.PageGroup) return true;
			return false;
		}
	}
	public class AccessiblePageGroupCaptionButton : AccessiblePageGroupButton {
		public AccessiblePageGroupCaptionButton(AccessibleRibbon ribbon, AccessiblePageGroup pageGroup) : base(ribbon, pageGroup) { }
		public override Rectangle ClientBounds {
			get {
				RibbonPageGroupViewInfo pg = PageGroupInfo;
				if(pg != null) return pg.CaptionBounds;
				return Rectangle.Empty;
			}
		}
		protected override string GetName() { return PageGroup.PageGroup.Text + "..."; }
		protected override AccessibleRole GetRole() { return AccessibleRole.ButtonDropDown; }
		protected override AccessibleStates GetState() {
			AccessibleStates states = AccessibleStates.Focusable;
			if(PageGroupInfo.Minimized) return AccessibleStates.Invisible | AccessibleStates.Unavailable;
			if(KeyboardActiveObject != null && IsEqualGroups(KeyboardActiveObject)) states |= AccessibleStates.Focused;
			return states;
		}
		protected override string GetDefaultAction() { return "Press"; }
		protected virtual RibbonHandler RibbonHandler { get { return Ribbon.Ribbon.Handler as RibbonHandler; } }
		protected override void DoDefaultAction() {
			if(RibbonHandler == null) return;
			RibbonHandler.OnClickPageGroupCaptionButton(PageGroup.PageGroup);
		}
	}
	public class AccessibleRibbonPageGroupContentButton : AccessiblePageGroupCaptionButton {
		public AccessibleRibbonPageGroupContentButton(AccessibleRibbon ribbon, AccessiblePageGroup pageGroup) : base(ribbon, pageGroup) { }
		public override Rectangle ClientBounds {
			get {
				RibbonPageGroupViewInfo pg = PageGroupInfo;
				if(pg != null) return pg.ContentButtonViewInfo.Bounds;
				return Rectangle.Empty;
			}
		}
		protected override string GetName() { return PageGroup.PageGroup.Text; }
		protected override AccessibleRole GetRole() { return AccessibleRole.ButtonDropDown; }
		protected override AccessibleStates GetState() {
			AccessibleStates states = AccessibleStates.Focusable;
			if(!PageGroupInfo.Minimized) return AccessibleStates.Invisible | AccessibleStates.Unavailable;
			if(KeyboardActiveObject != null && IsEqualGroups(KeyboardActiveObject)) states |= AccessibleStates.Focused;
			return states;
		}
		protected override string GetDefaultAction() { return "Press"; }
		protected override void DoDefaultAction() {
			if(PageGroupInfo == null || PageGroupInfo.Items.Count <= 0) return;
			BarItemLink link = PageGroupInfo.Items[0].Item as BarItemLink;
			if(link != null) link.OnLinkActionCore(BarLinkAction.MouseClick, null);
		}
	}
	public class AccessiblePageGroup : BaseRibbonAccessible {
		AccessibleSelectedPageContent pageContent;
		RibbonPageGroup pageGroup;
		RibbonPageGroupViewInfo pageGroupInfo;
		public AccessiblePageGroup(AccessibleSelectedPageContent pageContent, RibbonPageGroup pageGroup) : base(pageContent.Ribbon) {
			this.pageContent = pageContent;
			this.pageGroup = pageGroup;
			this.pageGroupInfo = GetPageGroupInfo();
			CreateCollection();
		}
		public AccessibleSelectedPageContent PageContent { get { return pageContent; } }
		public RibbonPageGroup PageGroup { get { return pageGroup; } }
		public RibbonPageGroupViewInfo PageGroupInfo { get { return pageGroupInfo; } }
		public RibbonPageGroupViewInfo GetPageGroupInfo() {
			for(int i = 0; i < Groups.Count; i++)
				if(PageGroup == Groups[i].PageGroup) return Groups[i];
			return null;
		}
		public override AccessibleObject Parent { get { return PageContent.Accessible; } }
		protected override string GetName() { return PageGroup.Text; }
		public RibbonPageGroupViewInfoCollection Groups { get { return Ribbon.Ribbon.ViewInfo.Panel.Groups; } }
		public override Rectangle ClientBounds {
			get {
				RibbonPageGroupViewInfo pg = PageGroupInfo;
				if(pg != null)return pg.Bounds;
				return Rectangle.Empty;
			}
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.ToolBar; }
		protected override AccessibleStates GetState() { return AccessibleStates.Focusable; }
		protected virtual bool ShouldAddCaptionButton { get { return PageGroup.ShowCaptionButton; } }
		protected override ChildrenInfo GetChildrenInfo() {
			int itemsCount = GetLinksCount(PageGroup.ItemLinks) + 1;
			if(ShouldAddCaptionButton) itemsCount++;
			return new ChildrenInfo(ChildType.Item, itemsCount);
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			foreach(BarItemLink link in PageGroup.ItemLinks) {
				AddLink(link);
			}
			if(ShouldAddCaptionButton) Children.Add(new AccessiblePageGroupCaptionButton(Ribbon, this));
			Children.Add(new AccessibleRibbonPageGroupContentButton(Ribbon, this));
		}
		protected bool IsLinksEquals(BarItemLink link1, BarItemLink link2) {
			if(link1 == link2) return true;
			if(link1.ClonedFromLink == link2) return true;
			if(link2.ClonedFromLink == link1) return true;
			return false;
		}
		public virtual int GetIndexByLink(BarItemLink link) {
			int index = 0;
			foreach(BaseAccessible item in Children) {
				BaseLinkAccessible accLink = item as BaseLinkAccessible;
				if(accLink != null && IsLinksEquals(accLink.Link, link)) return index;
				AccessibleRibbonPageGroupContentButton btn = item as AccessibleRibbonPageGroupContentButton;
				if(btn != null && btn.PageGroupInfo.Items.Count > 0 && IsLinksEquals(btn.PageGroupInfo.Items[0].Item as BarItemLink, link)) return index;
				RibbonInplaceGalleryAccessible accGallery = item as RibbonInplaceGalleryAccessible;
				if(accGallery != null && IsLinksEquals(accGallery.GalleryLink, link)) return index;
				index++;
			}
			return -1;	
		} 
	}
	public class AccessibleSelectedPageContent : BaseRibbonAccessible {
		AccessibleLowerRibbon lowerRibbon;
		public AccessibleSelectedPageContent(AccessibleLowerRibbon lower) : base(lower.Ribbon) {
			this.lowerRibbon = lower;
			CreateCollection();
		}
		public AccessibleLowerRibbon LowerRibbon { get { return lowerRibbon; } set { lowerRibbon = value; } }
		public override Rectangle ClientBounds { get { return LowerRibbon.ClientBounds; } }
		public RibbonPageGroupCollection Content { get { return SelectedPage.Groups; } }
		public RibbonPageGroupCollection MergedContent { get { return SelectedPage.MergedGroups; } }
		public RibbonPage SelectedPage { get { return Ribbon.Ribbon.SelectedPage; } }
		public override AccessibleObject Parent { get { return LowerRibbon.Accessible; } }
		protected override string GetName() {
			if(SelectedPage == null) return null;
			return SelectedPage.Text;
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.Pane; }
		protected override AccessibleStates GetState() { return AccessibleStates.None; }
		protected override ChildrenInfo GetChildrenInfo() {
			if(SelectedPage == null) return null;
			return new ChildrenInfo(ChildType.Item, Content.Count + MergedContent.Count);
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			if(SelectedPage == null) return;
			for(int i = 0; i < Content.Count; i++) {
				Children.Add(new AccessiblePageGroup(this, Content[i]));
			}
			for(int i = 0; i < MergedContent.Count; i++) {
				Children.Add(new AccessiblePageGroup(this, MergedContent[i]));
			}
		}
	}
	public class AccessibleLowerRibbon : BaseRibbonAccessible {
		public AccessibleLowerRibbon(AccessibleRibbon ribbon) : base(ribbon) { 
			CreateCollection();
		}
		public override Rectangle ClientBounds { get { return Ribbon.Ribbon.ViewInfo.Panel.Bounds; } }
		protected override string GetName() { return "Lower Ribbon"; }
		protected override AccessibleRole GetRole() { return AccessibleRole.Pane; }
		protected override AccessibleStates GetState() {
			if(Ribbon.Ribbon.Minimized && (Ribbon.Ribbon.MinimizedRibbonPopupForm == null || !Ribbon.Ribbon.MinimizedRibbonPopupForm.Visible))return AccessibleStates.Invisible;
			return AccessibleStates.None; 
		}
		public override string Value { get { return GetName(); } }
		protected override ChildrenInfo GetChildrenInfo() {
			return new ChildrenInfo(ChildType.Item, 1);
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			Children.Add(new AccessibleSelectedPageContent(this));
		}
	}
	public class AccessibleApplicationButton : BaseRibbonAccessible {
		public AccessibleApplicationButton(AccessibleRibbon ribbon) : base(ribbon) {
			CreateCollection();
		}
		public RibbonApplicationButtonInfo ApplicationButtonInfo { get { return RibbonViewInfo.ApplicationButton; } }
		public PopupControl ApplicationMenu { get { return Ribbon.Ribbon.ApplicationButtonPopupControl; } }
		public Control ApplicationButtonControl { get { return Ribbon.Ribbon.ApplicationButtonControl; } }
		public CustomControl CustomControl { get { return ApplicationMenu.IPopup.CustomControl; } }
		public override Rectangle ClientBounds { get { return ApplicationButtonInfo.Bounds; } }
		protected override string GetName() { return Ribbon.Ribbon.ApplicationButtonAccessibleName; }
		protected override string GetDescription() { return Ribbon.Ribbon.ApplicationButtonAccessibleDescription; }
		protected override AccessibleRole GetRole() { return AccessibleRole.ButtonDropDownGrid; }
		protected override AccessibleStates GetState() {
			AccessibleStates states = AccessibleStates.Focusable;
			if(ApplicationMenu != null) states |= AccessibleStates.HasPopup;
			if(KeyboardActiveObject != null && KeyboardActiveObject.Object == ApplicationButtonInfo) states |= AccessibleStates.Focused;
			return states;
		}
		protected virtual bool ShouldAddApplicationMenu {
			get {
				if(ApplicationMenu == null || ApplicationMenu.IPopup == null || ApplicationMenu.IPopup.CustomControl == null) return false;
				return true;
			}
		}
		protected override ChildrenInfo GetChildrenInfo() {
			if(!ShouldAddApplicationMenu) return null;
			return new ChildrenInfo(ChildType.Item, 1);
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			if(ShouldAddApplicationMenu) Children.Add(CustomControl.DXAccessible);
		}
		bool IsApplicationDropDownControlVisible { 
			get { 
				if(ApplicationMenu != null) return ApplicationMenu.Visible;
				if(ApplicationButtonControl != null) return ApplicationButtonControl.Visible;
				return false;
			} 
		}
		protected override string GetDefaultAction() {
			if(Ribbon.Ribbon.ApplicationButtonDropDownControl == null) return string.Empty;
			if(!IsApplicationDropDownControlVisible) return "Open";
			return "Close";
		}
		protected override void DoDefaultAction() {
			if(Ribbon.Ribbon.ApplicationButtonDropDownControl == null) return;
			if(ApplicationMenu != null) {
				if(ApplicationMenu.Visible == false) {
					(Ribbon.Ribbon.Handler as RibbonHandler).ShowApplicationButtonPopup();
					(Ribbon.Ribbon.Handler as RibbonHandler).OnApplicationButtonClickCore();
				}
				else 
					ApplicationMenu.HidePopup();
			}
			else { 
			}
		}
	}
	public class AccessibleRibbonPage : BaseAccessible {
		RibbonPage page;
		RibbonPageViewInfo pageInfo; 
		AccessibleRibbonPageList pageList;
		public AccessibleRibbonPage(AccessibleRibbonPageList pageList, RibbonPage page) : base() {
			this.page = page;
			this.pageList = pageList;
			this.pageInfo = GetPageInfo();
		}
		public AccessibleRibbonPageList PageList { get { return pageList; } set { pageList = value; } }
		public RibbonPage Page { get { return page; } set { page = value; } }
		public RibbonPageViewInfo PageInfo { get { return pageInfo; } }
		public RibbonPageViewInfo GetPageInfo() {
			for(int i = 0; i < Ribbon.ViewInfo.Header.Pages.Count; i++) {
				if(Page == Ribbon.ViewInfo.Header.Pages[i].Page) return Ribbon.ViewInfo.Header.Pages[i];
			}						
			return null;
		} 
		public RibbonControl Ribbon { get { return Page.Ribbon; } }
		public override Rectangle ClientBounds { 
			get {
				if(PageInfo != null) return pageInfo.Bounds;
				return Rectangle.Empty;
			} 
		}
		protected override string GetName() { return Page.Text; }
		public override AccessibleObject Parent { get { return PageList.Accessible; } }
		protected override AccessibleRole GetRole() { return AccessibleRole.PageTab; }
		protected virtual bool IsEqualPages(NavigationObject obj) { 
			NavigationObjectPage navPage = obj as NavigationObjectPage;
			if(navPage == null) return false;
			return navPage.Page == Page;
		}
		protected override AccessibleStates GetState() {
			if(Page.Visible == false) return AccessibleStates.Invisible;
			AccessibleStates states = AccessibleStates.Selectable | AccessibleStates.Focusable;
			if(Ribbon.SelectedPage == Page) states |= AccessibleStates.Selected | AccessibleStates.Focused;
			if(Ribbon.ViewInfo.KeyboardActiveObject != null && IsEqualPages(Ribbon.ViewInfo.KeyboardActiveObject)) states |= AccessibleStates.Focused;
			return states;
		}
		protected override string GetDefaultAction() { return "Switch"; }
		protected override void DoDefaultAction() { DefaultAction(); }
		protected override void Select(AccessibleSelection flags) { DefaultAction(); }
		void DefaultAction() { if(Page.Visible) Ribbon.SelectedPage = Page; }
	}
	public class AccessibleRibbonPageList : BaseRibbonAccessible {
		public AccessibleRibbonPageList(AccessibleRibbon ribbon) : base(ribbon) {
			CreateCollection();
		}
		public override Rectangle ClientBounds { get { return Ribbon.Ribbon.ViewInfo.Header.Bounds; } }
		protected override string GetName() { return "Ribbon Tabs"; }
		protected override AccessibleRole GetRole() { return AccessibleRole.PageTabList; }
		protected override AccessibleStates GetState() {
			if(Ribbon.Ribbon.ShowPageHeadersMode == ShowPageHeadersMode.Hide ||
				(Ribbon.Ribbon.ShowPageHeadersMode == ShowPageHeadersMode.ShowOnMultiplePages && Ribbon.Ribbon.Pages.VisiblePages.Count == 1)) return AccessibleStates.Invisible;
			return AccessibleStates.None; 
		}
		public override string Value { get { return "Ribbon Tab List"; } }
		protected override ChildrenInfo GetChildrenInfo() {
			return new ChildrenInfo(ChildType.Item, Ribbon.Ribbon.TotalPageCategory.Pages.Count);
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			for(int i = 0; i < Ribbon.Ribbon.TotalPageCategory.Pages.Count; i++) {
				Children.Add(new AccessibleRibbonPage(this, Ribbon.Ribbon.TotalPageCategory.Pages[i]));
			}
		}
	}
	public class AccessibleQuickToolbarCustomizeItemLink : BaseMenuLinkAccessible {
		public AccessibleQuickToolbarCustomizeItemLink(BarCustomContainerItemLink link) : base(link) { }
		protected override string GetDefaultAction() { return "Press"; }
		protected override void DoDefaultAction() { Link.OnLinkActionCore(BarLinkAction.Press, null);}
		protected override AccessibleStates GetState() {
			AccessibleStates states = base.GetState();
#if DXWhidbey
			states |= AccessibleStates.HasPopup;
#endif            
			if(Link.Opened) states |= AccessibleStates.Pressed;
			return states;
		}
	}
	public class AccessibleQuickAccessToolbar : BaseRibbonAccessible {
		RibbonQuickAccessToolbar quickAccessToolbar;
		public AccessibleQuickAccessToolbar(AccessibleRibbon ribbon, RibbonQuickAccessToolbar quickAccessToolbar) : base(ribbon) {
			this.quickAccessToolbar = quickAccessToolbar;
			this.quickAccessToolbar.SetAccessible(this);
			CreateCollection();
		}
		internal void UpdateCollection() {
			if(QuickAccessToolbar.IsDestroying)
				return;
			ResetCollection();
			RequestChildren();
		}
		public RibbonQuickAccessToolbar QuickAccessToolbar { get { return quickAccessToolbar; } set { quickAccessToolbar = value; } }
		public RibbonQuickAccessToolbarViewInfo ToolbarViewInfo { get { return QuickAccessToolbar.Ribbon.ViewInfo.Toolbar; } }
		public override Rectangle ClientBounds { get { return QuickAccessToolbar.Ribbon.ViewInfo.Toolbar.Bounds; } }
		protected override string GetName() { return "Quick Access Toolbar"; }
		protected override AccessibleRole GetRole() { return AccessibleRole.ToolBar; }
		protected override AccessibleStates GetState() {
			if(QuickAccessToolbar.Ribbon.ToolbarLocation == RibbonQuickAccessToolbarLocation.Hidden) return AccessibleStates.Invisible;
			return AccessibleStates.None; 
		}
		public override string Value { get { return GetName(); } }
		protected override ChildrenInfo GetChildrenInfo() {
			int itemCount = QuickAccessToolbar.ItemLinks.Count;
			if(ToolbarViewInfo.IsToolbarCustomizationItem(ToolbarViewInfo.VisibleButtonCount - 1))
				itemCount++;
			 return new ChildrenInfo(ChildType.Item, itemCount);
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			foreach(BarItemLink link in QuickAccessToolbar.ItemLinks) {
				AddLink(link);
			}
			if(ToolbarViewInfo.VisibleButtonCount == 0 || ToolbarViewInfo.Items.Count < ToolbarViewInfo.VisibleButtonCount)
				return;
			BarItemLink linkCustomization = ToolbarViewInfo.Items[ToolbarViewInfo.VisibleButtonCount - 1].Item as BarItemLink;
			if(linkCustomization != null)
				AddLink(linkCustomization);
		}
	}
	public class RibbonInplaceGalleryButtonAccessible : BaseRibbonAccessible {
		RibbonGalleryBarItemLink link;
		public RibbonInplaceGalleryButtonAccessible(AccessibleRibbon ribbon, RibbonGalleryBarItemLink link)
			: base(ribbon) {
			this.link = link;
		}
		public RibbonGalleryBarItemLink GalleryLink { get { return link; } }
		public bool GalleryDropDownOpened { 
			get {
				if(GalleryLink == null || GalleryLink.Item == null || GalleryLink.Item.GalleryDropDown == null) return false;
				return GalleryLink.Item.GalleryDropDown.Opened; 
			} 
		}
		protected override string GetDefaultAction() { return "Press"; }
		protected virtual void PressUpDownButtons(RibbonHitTest hitTest) {
			RibbonHitInfo hitInfo = new RibbonHitInfo();
			hitInfo.SetHitTest(hitTest);
			hitInfo.GalleryInfo = GalleryLink.ViewInfo.GalleryInfo;
			GalleryLink.OnPressScrollButtonCore(hitInfo);
		}
	}
	public class RibbonInplaceGalleryUpButtonAccessible : RibbonInplaceGalleryButtonAccessible {
		public RibbonInplaceGalleryUpButtonAccessible(AccessibleRibbon ribbon, RibbonGalleryBarItemLink link) : base(ribbon, link) { }
		protected override string GetDescription() { return "Scrolls the gallery up one row"; }
		protected override void DoDefaultAction() { PressUpDownButtons(RibbonHitTest.GalleryUpButton); }
		public override Rectangle ClientBounds { get { return GalleryLink.ViewInfo.GalleryInfo.ButtonUpBounds; } }
		protected override string GetName() { return "Row up"; }
		protected override AccessibleRole GetRole() { return AccessibleRole.PushButton; }
		protected override AccessibleStates GetState() {
			if(GalleryLink.ScrollYPosition == 0) return AccessibleStates.Unavailable;
			return AccessibleStates.None; 
		}
	}
	public class RibbonInplaceGalleryDownButtonAccessible : RibbonInplaceGalleryButtonAccessible {
		public RibbonInplaceGalleryDownButtonAccessible(AccessibleRibbon ribbon, RibbonGalleryBarItemLink link) : base(ribbon, link) { }
		protected override string GetDescription() { return "Scrolls the gallery down one row"; }
		protected override void DoDefaultAction() { PressUpDownButtons(RibbonHitTest.GalleryDownButton); }
		public override Rectangle ClientBounds { get { return GalleryLink.ViewInfo.GalleryInfo.ButtonDownBounds; } }
		protected override string GetName() { return "Row down"; }
		protected override AccessibleRole GetRole() { return AccessibleRole.PushButton; }
		protected override AccessibleStates GetState() {
			if(GalleryLink.ScrollYPosition >= GalleryLink.ViewInfo.GalleryInfo.MaxScrollYPosition) return AccessibleStates.Unavailable;
			return AccessibleStates.None; 
		}
	}
	public class RibbonInplaceGalleryDropDownButtonAccessible : RibbonInplaceGalleryButtonAccessible {
		public RibbonInplaceGalleryDropDownButtonAccessible(AccessibleRibbon ribbon, RibbonGalleryBarItemLink link) : base(ribbon, link) { }
		public override Rectangle ClientBounds { get { return GalleryLink.ViewInfo.GalleryInfo.ButtonCommandBounds; } }
		protected override string GetName() { return "Expand Gallery Button"; }
		protected override AccessibleRole GetRole() { return AccessibleRole.ButtonDropDownGrid; }
		protected override AccessibleStates GetState() { 
			AccessibleStates states = AccessibleStates.Focusable;
#if DXWhidbey
			states |= AccessibleStates.HasPopup;
#endif
			if(KeyboardActiveObject != null && KeyboardActiveObject.Object == GalleryLink.ViewInfo) states |= AccessibleStates.Focused;
			return states;
		}
		protected override void DoDefaultAction() { GalleryLink.ShowPopup(); }
	}
	public class RibbonInplaceGalleryBaseAccessible : BaseRibbonAccessible {
		RibbonGalleryBarItemLink link;
		public RibbonInplaceGalleryBaseAccessible(AccessibleRibbon ribbon, RibbonGalleryBarItemLink link) : base(ribbon) {
			this.link = link;
		}
		public RibbonGalleryBarItemLink GalleryLink { get { return link as RibbonGalleryBarItemLink; } }
		public override Rectangle ClientBounds { get { return GalleryLink.ViewInfo.GalleryInfo.Bounds; } }
		protected override string GetName() { return GalleryLink.Caption; }
	}
	public class RibbonInplaceGalleryAccessible : RibbonInplaceGalleryBaseAccessible {
		public RibbonInplaceGalleryAccessible(AccessibleRibbon ribbon, RibbonGalleryBarItemLink link) : base(ribbon, link) {
			CreateCollection();
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.ComboBox; }
		protected override AccessibleStates GetState() { return AccessibleStates.None; }
		protected override ChildrenInfo GetChildrenInfo() {
			return new ChildrenInfo(ChildType.Item, GalleryLink.Item.Gallery.Groups.Count + 3);
		}
		protected override void OnChildrenCountChanged() {
			for(int i = 0; i < GalleryLink.Item.Gallery.Groups.Count; i++) {
				Children.Add(new RibbonInplaceGalleryGroupAccessible(Ribbon, this, GalleryLink.Item.Gallery.Groups[i] as GalleryItemGroup));
			}
			Children.Add(new RibbonInplaceGalleryUpButtonAccessible(Ribbon, GalleryLink));
			Children.Add(new RibbonInplaceGalleryDownButtonAccessible(Ribbon, GalleryLink));
			Children.Add(new RibbonInplaceGalleryDropDownButtonAccessible(Ribbon, GalleryLink));
		}
	}
	public class InDropDownGalleryFilterAccessible : BaseRibbonAccessible {
		InDropDownGalleryAccessible gallery;
		public InDropDownGalleryFilterAccessible(AccessibleRibbon ribbon, InDropDownGalleryAccessible gallery) : base(ribbon) {
			this.gallery = gallery;
		}
		public InDropDownGalleryAccessible Gallery { get { return gallery; } }
		RibbonHitInfo HitInfo { get { return Gallery.Gallery.ViewInfo.HitInfo; } }
		public override Rectangle ClientBounds { get { return Gallery.Gallery.ViewInfo.FilterAreaBounds; } }
		protected override AccessibleRole GetRole() { return AccessibleRole.Link; }
		protected override AccessibleStates GetState() {
			AccessibleStates states = AccessibleStates.Focusable | AccessibleStates.Selectable;
			if(!Gallery.Gallery.AllowFilter) states |= AccessibleStates.Invisible;
			if(HitInfo.InGalleryFilter) states |= AccessibleStates.Focused | AccessibleStates.Selected;
			return states;
		}
		protected override string GetName() { return Gallery.Gallery.FilterCaption; }		
	} 
	public class InDropDownGalleryAccessible : BaseRibbonAccessible { 
		InDropDownGallery gallery;
		public InDropDownGalleryAccessible(AccessibleRibbon ribbon, InDropDownGallery gallery) : base(ribbon) {
			this.gallery = gallery;
			CreateCollection();
		}
		public InDropDownGallery Gallery { get { return gallery; } }
		protected override AccessibleRole GetRole() { return AccessibleRole.List; }
		protected override AccessibleStates GetState() { return AccessibleStates.None; }
		protected override ChildrenInfo GetChildrenInfo() {
			int childCount = Gallery.Groups.Count + 1;
			if(Gallery.AllowFilter) childCount++;
			return new ChildrenInfo(ChildType.Item, childCount);
		}
		protected override void OnChildrenCountChanged() {
			if(Gallery.AllowFilter) Children.Add(new InDropDownGalleryFilterAccessible(Ribbon, this));
			for(int i = 0; i < Gallery.Groups.Count; i++) {
				Children.Add(new InDropDownGalleryGroupAccessible(Ribbon, this, Gallery.Groups[i] as GalleryItemGroup));
			}
			Children.Add(new ScrollBarAccessible(Gallery.ScrollBar));
		}
		public override Control GetOwnerControl() {
			if(Ribbon == null)
				return Gallery.BarControl;
			return base.GetOwnerControl();
		}
	}
	public class RibbonGalleryItemAccessible : BaseRibbonAccessible { 
		GalleryItem item;
		BaseAccessible gallery;
		GalleryItemViewInfo itemInfo;
		public RibbonGalleryItemAccessible(AccessibleRibbon ribbon, BaseAccessible gallery, GalleryItem item)
			: base(ribbon) {
			this.item = item;
			this.gallery = gallery;
			this.itemInfo = GetItemInfo();
		}
		public GalleryItem Item { get { return item; } }
		public GalleryItemViewInfo ItemInfo { get { return itemInfo; } }
		public virtual BaseAccessible Gallery { get { return gallery; } }
		protected override string GetDefaultAction() { return "Click"; }
		protected virtual GalleryItemViewInfo GetItemInfo() { return null; }
		public override Rectangle ClientBounds { get { return ItemInfo.Bounds; } }
		protected override string GetName() { return Item.Caption; }
		protected override AccessibleRole GetRole() { return AccessibleRole.ListItem; }
		protected override AccessibleStates GetState() {
			if(ItemInfo == null) return AccessibleStates.None;
			AccessibleStates states = AccessibleStates.Selectable | AccessibleStates.Focusable;
			if(ItemInfo.IsInvisible || ItemInfo.IsPartiallyVisible) states |= AccessibleStates.Offscreen;
			if(Item.Checked) states |= AccessibleStates.Selected;
			if(IsHotObject) states |= AccessibleStates.Focused;
			return states;
		}	
		public virtual bool IsHotObject { get { return false; } }
	}
	public class InRibbonGalleryItemAccessible : RibbonGalleryItemAccessible {
		public InRibbonGalleryItemAccessible(AccessibleRibbon ribbon, BaseAccessible gallery, GalleryItem item)
			: base(ribbon, gallery, item) {
		}
		public new RibbonInplaceGalleryBaseAccessible Gallery { get { return base.Gallery as RibbonInplaceGalleryBaseAccessible; } }
		protected override void DoDefaultAction() {
			Item.Gallery.OnItemClick(Gallery.GalleryLink, Gallery.GalleryLink.Item.Gallery, Item);
		}
		protected override GalleryItemViewInfo GetItemInfo() {
			if(Gallery == null) return null;
			foreach(GalleryItemGroupViewInfo group in Gallery.GalleryLink.ViewInfo.GalleryInfo.Groups) {
				foreach(GalleryItemViewInfo itemInfo in group.Items) {
					if(itemInfo.Item == Item) return itemInfo;
				}
			}
			return null;
		}
		public override bool IsHotObject {
			get {
				return Ribbon.Ribbon.ViewInfo.HotObject.InGalleryItem && Ribbon.Ribbon.ViewInfo.HotObject.GalleryItem == Item;
			}
		}
	}
	public class InDropDownGalleryItemAccessible : RibbonGalleryItemAccessible {
		public InDropDownGalleryItemAccessible(AccessibleRibbon ribbon, BaseAccessible gallery, GalleryItem item) : base(ribbon, gallery, item) { }
		protected override void DoDefaultAction() {
			Gallery.Gallery.OnItemClick(null, Gallery.Gallery, Item);
		}
		public new InDropDownGalleryAccessible Gallery { get { return base.Gallery as InDropDownGalleryAccessible; } }
		protected override GalleryItemViewInfo GetItemInfo() {
			if(Gallery == null) return null;
			foreach(GalleryItemGroupViewInfo group in Gallery.Gallery.ViewInfo.Groups) {
				foreach(GalleryItemViewInfo itemInfo in group.Items) {
					if(itemInfo.Item == Item) return itemInfo;
				}
			}
			return null;
		}
		RibbonHitInfo HitInfo { get { return Gallery.Gallery.ViewInfo.HitInfo; } }
		public override bool IsHotObject {  
			get { 
				if(HitInfo.InGalleryItem && HitInfo.GalleryItem == Item) return true;
				if(Gallery.Gallery.ViewInfo.KeyboardSelectedItem != null && Gallery.Gallery.ViewInfo.KeyboardSelectedItem.Item == Item) return true;
				return false;
			}
		}
		public override Control GetOwnerControl() {
			if(Ribbon == null)
				return Gallery.Gallery.BarControl;
			return base.GetOwnerControl();
		}
		public override AccessibleObject Parent {
			get {
				if(Ribbon == null)
					return Gallery.Accessible;
				return base.Parent;
			}
		}
	}
	public class RibbonGalleryGroupAccessible : BaseRibbonAccessible {
		BaseAccessible gallery;
		GalleryItemGroup group;
		public RibbonGalleryGroupAccessible(AccessibleRibbon ribbon, BaseAccessible gallery, GalleryItemGroup group) : base(ribbon) {
			this.gallery = gallery;
			this.group = group;
			CreateCollection();
		}
		public GalleryItemGroup Group { get { return group; } }
		public virtual BaseAccessible Gallery { get { return gallery; } }
		protected override AccessibleRole GetRole() { return AccessibleRole.List; }
		protected override ChildrenInfo GetChildrenInfo() {
			return new ChildrenInfo(ChildType.Item, Group.Items.Count);
		}
		protected override void OnChildrenCountChanged() {
			for(int i = 0; i < Group.Items.Count; i++) {
				Children.Add(CreateItemAccessible(Group.Items[i]));
			}
		}
		protected virtual RibbonGalleryItemAccessible CreateItemAccessible(GalleryItem item) {
			return new RibbonGalleryItemAccessible(Ribbon, Gallery, item);
		}
	}
	public class RibbonInplaceGalleryGroupAccessible : RibbonGalleryGroupAccessible {
		public RibbonInplaceGalleryGroupAccessible(AccessibleRibbon ribbon, RibbonInplaceGalleryAccessible gallery, GalleryItemGroup group) : base(ribbon, gallery, group) { }
		public new RibbonInplaceGalleryAccessible Gallery { get { return base.Gallery as RibbonInplaceGalleryAccessible; } }
		public override Rectangle ClientBounds { get { return Gallery.GalleryLink.ViewInfo.GalleryInfo.ContentBounds; } }
		protected override string GetName() { return Gallery.GalleryLink.Caption; }
		protected override RibbonGalleryItemAccessible CreateItemAccessible(GalleryItem item) { return new InRibbonGalleryItemAccessible(Ribbon, Gallery, item); }
	}
	public class InDropDownGalleryGroupAccessible : RibbonGalleryGroupAccessible {
		public InDropDownGalleryGroupAccessible(AccessibleRibbon ribbon, BaseAccessible gallery, GalleryItemGroup group)
			: base(ribbon, gallery, group) {
			CreateCollection();
		}
		public new InDropDownGalleryAccessible Gallery { get { return base.Gallery as InDropDownGalleryAccessible; } }
		public override Rectangle ClientBounds { get { return Gallery.Gallery.ViewInfo.Bounds; } }
		protected override string GetName() { return Gallery.Gallery.GalleryDropDown.Name; }
		protected override RibbonGalleryItemAccessible CreateItemAccessible(GalleryItem item) { return new InDropDownGalleryItemAccessible(Ribbon, Gallery, item); }
		public override Control GetOwnerControl() {
			if(Ribbon == null)
				return Gallery.Gallery.BarControl;
			return base.GetOwnerControl();
		}
	}
	public class AccessibleRibbon : BaseControlAccessible {
		public AccessibleRibbon(RibbonControl ribbon) : base(ribbon) {
			CreateCollection();
		}
		public RibbonControl Ribbon { get { return Control as RibbonControl; } }
		public override Rectangle ClientBounds { get { return Ribbon.Bounds; } }
		protected override string GetName() { return "The Ribbon"; }
		protected override AccessibleRole GetRole() { return AccessibleRole.PropertyPage; }
		protected override AccessibleStates GetState() { return AccessibleStates.None; }
		public override string Value { get { return GetName(); } }
		public override AccessibleObject Parent { 
			get {
				if (Ribbon.Parent == null) return null;
				return Ribbon.Parent.AccessibilityObject; 
			} 
		}
		protected override ChildrenInfo GetChildrenInfo() {
			return new ChildrenInfo(ChildType.Item, 4);
		}
		public override Control GetOwnerControl() { return Ribbon; }
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			Children.Add(new AccessibleApplicationButton(this));
			Children.Add(new AccessibleQuickAccessToolbar(this, Ribbon.Toolbar));
			Children.Add(new AccessibleRibbonPageList(this));
			Children.Add(new AccessibleLowerRibbon(this));	   
		}
		public override bool RaiseQueryAccessibilityHelp(QueryAccessibilityHelpEventArgs e) {
			return base.RaiseQueryAccessibilityHelp(e);
		}
		public virtual void Recreate() {
			Children.Clear();
			Children.Add(new AccessibleApplicationButton(this));
			Children.Add(new AccessibleQuickAccessToolbar(this, Ribbon.Toolbar));
			Children.Add(new AccessibleRibbonPageList(this));
			Children.Add(new AccessibleLowerRibbon(this));	   
		}
	}
}
