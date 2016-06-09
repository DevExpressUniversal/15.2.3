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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraBars.Helpers.Docking;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Handler;
namespace DevExpress.XtraBars.Accessible {
	public class AccessibleItemUserInfoHelper : IDXAccessibleUserInfo {
		BarItem item;
		public AccessibleItemUserInfoHelper(BarItem item) {
			this.item = item;
		}
		string IDXAccessibleUserInfo.DefaultAction { get { return null; } }
		string IDXAccessibleUserInfo.AccessibleName { get { return item.AccessibleName; } }
		string IDXAccessibleUserInfo.AccessibleDescription { get { return item.AccessibleDescription; } }
		AccessibleRole IDXAccessibleUserInfo.AccessibleRole { get { return AccessibleRole.Default; } }
	}
	public class DockControlAccessible : BaseAccessible {
		BarDockControl dockControl;
		public DockControlAccessible(BarDockControl dockControl) {
			this.dockControl = dockControl;
		}
		public BarDockControl DockControl { get { return dockControl; } }
		public override Control GetOwnerControl() { return DockControl; }
		public override bool IsTopControl { get { return true; } }
		protected override string GetName() {
			switch(DockControl.DockStyle) {
				case BarDockStyle.Top : return GetString(AccStringId.BarDockControlTop);
				case BarDockStyle.Left : return GetString(AccStringId.BarDockControlLeft);
				case BarDockStyle.Bottom : return GetString(AccStringId.BarDockControlBottom);
				case BarDockStyle.Right : return GetString(AccStringId.BarDockControlRight);
			}
			return base.GetName();
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.Grouping; }
		protected override ChildrenInfo GetChildrenInfo() {
			return new ChildrenInfo(ChildType.Toolbar, DockControl.Rows.GetObjectCount());
		}
		protected override void OnChildrenCountChanged() {
			foreach(DockRow row in DockControl.Rows) {
				foreach(DockRowObject rObj in row) {
					if(rObj.Dockable.BarControl == null) AddChild(new BaseAccessible());
					else AddChild(rObj.Dockable.BarControl.DXAccessible);
				}
			}
		}
	}
	public class CustomLinksContainerAccessible : BaseAccessible {
		CustomLinksControl control;
		public CustomLinksContainerAccessible(CustomLinksControl control) {
			this.control = control;
		}
		protected CustomLinksControl Control { get { return control; } }
		public override Control GetOwnerControl() { return control; }
		protected override ChildrenInfo GetChildrenInfo() {
			return new ChildrenInfo(ChildType.Item, Control.VisibleLinks.Count);
		}
		protected override void OnChildrenCountChanged() {
			foreach(BarItemLink link in Control.VisibleLinks) {
				AddChild(link.DXAccessible);
			}
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.List; } 
	}
	public class ToolbarAccessible : BaseAccessible {
		Bar bar;
		public ToolbarAccessible(Bar bar) {
			this.bar = bar;
			if(this.bar.DockControl != null) this.ParentCore = this.bar.DockControl.DXAccessible;
		}
		public Bar Bar { get { return bar; } }
		public bool IsReady { get { return Bar.BarControl != null && Bar.ViewInfo != null; } }
		public override Control GetOwnerControl() { return Bar.BarControl; }
		protected override ChildrenInfo GetChildrenInfo() {
			if(!IsReady) return null;
			return new ChildrenInfo(ChildType.Item, Bar.BarControl.VisibleLinks.Count);
		}
		protected override void OnChildrenCountChanged() {
			if(!IsReady) return;
			foreach(BarItemLink link in Bar.BarControl.VisibleLinks) {
				AddLink(link);
			}
		}
		protected virtual void AddLink(BarItemLink link) {
			AddChild(link.DXAccessible);
			link.DXAccessible.ParentCore = this;
		}
		protected override AccessibleRole GetRole() { 
			if(Bar.IsMainMenu) return AccessibleRole.MenuBar;
			if(Bar.IsStatusBar) return AccessibleRole.StatusBar;
			return AccessibleRole.ToolBar;
		}
		protected override string GetName() { return Bar.Text; }
		public override string Value {
			get {
				if(!Bar.IsStatusBar) return null;
				return GetStaticText();
			}
		}
		protected string GetStaticText() {
			string res = string.Empty;
			foreach(BarItemLink link in Bar.VisibleLinks) {
				BarStaticItemLink st = link as BarStaticItemLink;
				if(st == null) return null;
				if(res.Length == 0) res += link.Caption;
				else res += " | " + link.Caption;
			}
			return res;
		}
		internal void ResetChildrenAccessible() {
			ResetCollection();
			OnChildrenCountChanged();
		}
		public override AccessibleObject GetChild(int index) {
			if(GetChildCount() >= Children.Count) {
				ResetChildrenAccessible();
			}
			return base.GetChild(index);
		}
	}
	public class EditLinkAccessible : BaseLinkAccessible {
		public EditLinkAccessible(BarEditItemLink link) : base(link) { }
		public new BarEditItemLink Link { get { return (BarEditItemLink)base.Link; } }
		public new BarEditItem Item { get { return (BarEditItem)base.Item; } }
		protected override string GetDefaultAction() {
			if(!CanEdit) return null;
			return GetString(AccStringId.BarLinkEdit);
		}
		bool CanEdit { get { return IsReady && Link.Enabled && Item.Edit != null && Link.CanOpenEdit; } }
		protected override void DoDefaultAction() {
			if(!CanEdit) return;
			Link.ShowEditor();
		}
		protected override ChildrenInfo GetChildrenInfo() {
			if(!Link.EditorActive) return null;
			return new ChildrenInfo(ChildType.Item, 1);
		}
		protected override void OnChildrenCountChanged() {
			if(!Link.EditorActive) return;
			AddChild(Link.ActiveEditor.GetAccessible());
		}
	}
	public class BaseLinkAccessible : BaseAccessible {
		BarItemLink link;
		public BaseLinkAccessible(BarItemLink link) {
			this.link = link;
			if(link.BarControl != null) this.ParentCore = link.BarControl.DXAccessible;
			if(Item != null)
				CustomInfo = new AccessibleItemUserInfoHelper(Item);
		}
		public BarLinkViewInfo ViewInfo { get { return Link.LinkViewInfo; } }
		public BarItemLink Link { get { return link; } }
		public BarItem Item { get { return Link.Item; } }
		public bool IsReady { get { return Item != null; } }
		protected override Rectangle RectangleToScreen(Rectangle bounds) {
			if(Link.BarControl == null)
				return base.RectangleToScreen(bounds);
			return Link.BarControl.RectangleToScreen(bounds);
			}
		protected override Point PointToClient(Point point) {
			if(Link.BarControl == null)
				return base.PointToClient(point);
			return Link.BarControl.PointToClient(point);
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.PushButton; }
		protected override string GetName() { return Link.Caption == "" ? GetString(AccStringId.BarLinkCaption) : RemoveMnemonic(Link.Caption); }
		protected override string GetDescription() { return !IsReady || Item.Hint == "" ? null : Item.Hint; }
		protected override string GetKeyboardShortcut() {
			if(!Link.Enabled || !IsReady || !Link.Item.ItemShortcut.IsExist) return null;
			return Link.Item.ItemShortcut.ToString();
		}
		protected override string GetDefaultAction() {
			if(!IsReady || !Link.Enabled) return null;
			return GetString(AccStringId.BarLinkClick);
		}
		protected override void DoDefaultAction() {
			if(!IsReady || !Link.Enabled) return;
			Link.OnLinkActionCore(BarLinkAction.KeyClick, null);
		}
		public override Rectangle ClientBounds {
			get {
				if(ViewInfo == null) return Rectangle.Empty;
				return ViewInfo.Bounds;
			}
		}
		protected bool InRibbon { get { return Ribbon != null; } }
		protected RibbonControl Ribbon { get { return Link.Item != null? Link.Item.Ribbon : null; } }
		protected virtual NavigationObjectRibbonItem GetActiveKeyboardItem() {
				if(Ribbon == null) return null;
			NavigationObject navObj = null;
			if(Ribbon.PopupGroupForm != null && Ribbon.PopupGroupForm.Visible) navObj = Ribbon.PopupGroupForm.Control.ViewInfo.KeyboardActiveObject;
			if(navObj == null && Ribbon.MinimizedRibbonPopupForm != null && Ribbon.MinimizedRibbonPopupForm.Visible) {
				if(Ribbon.MinimizedRibbonPopupForm.Control.PopupGroupForm != null && Ribbon.MinimizedRibbonPopupForm.Control.PopupGroupForm.Visible)
					navObj = Ribbon.MinimizedRibbonPopupForm.Control.PopupGroupForm.Control.ViewInfo.KeyboardActiveObject;
				else
					navObj = Ribbon.MinimizedRibbonPopupForm.Control.ViewInfo.KeyboardActiveObject;
			} 
			if(navObj == null) navObj = Ribbon.ViewInfo.KeyboardActiveObject;
			return navObj as NavigationObjectRibbonItem;
		}
		protected override AccessibleStates GetState() {
			AccessibleStates state = AccessibleStates.Focusable;
			NavigationObjectRibbonItem item = GetActiveKeyboardItem();
			if(InRibbon && item != null && item.ItemLink == Link) state |= AccessibleStates.Focused;
			if(Link.Manager == null)
				return state;
			if(Link.Manager.HighlightedLink == Link) state = AccessibleStates.HotTracked | AccessibleStates.Focused;
			if(!Link.Enabled) return AccessibleStates.Unavailable;
			if(!Link.Visible) return state |= AccessibleStates.Invisible;
			return state;
		}
		public virtual int ID { 
			get {
				if(ParentCore == null || !ParentCore.HasChildren) return -1;
				return ParentCore.Children.IndexOf(this); 
			} 
		}
		protected RibbonPageGroup OwnerPageGroup {
			get { 
				RibbonPageGroupItemLinkCollection coll = Link.Holder as RibbonPageGroupItemLinkCollection;
				return coll == null ? null : coll.PageGroup;
			}
		}
		protected RibbonQuickAccessToolbar OwnerQuickAccessToolbar {
			get {
				RibbonQuickToolbarItemLinkCollection coll = Link.Holder as RibbonQuickToolbarItemLinkCollection;
				return coll == null ? null : coll.Toolbar;
			}
		}
		public override bool IsVisible {
			get {
				if(!Link.Visible)
					return false;
				if(Link.Ribbon != null) {
					if(OwnerPageGroup != null) {
						if(!OwnerPageGroup.Visible || OwnerPageGroup.GroupInfo == null || OwnerPageGroup.GroupInfo.Minimized || !OwnerPageGroup.Page.Visible || OwnerPageGroup.Ribbon.Minimized || !OwnerPageGroup.Ribbon.Visible)
							return false;
					}
					RibbonQuickAccessToolbar qt = OwnerQuickAccessToolbar;
					if(qt != null) {
						if(qt.Ribbon.ToolbarLocation == RibbonQuickAccessToolbarLocation.Hidden)
							return false;
					}
				}
				return ViewInfo != null && ViewInfo.Bounds != Rectangle.Empty;
			}
		}
	}
	public class StaticLinkAccessible : BaseLinkAccessible {
		public StaticLinkAccessible(BarStaticItemLink link) : base(link) { }
		protected override AccessibleRole GetRole() { return AccessibleRole.StaticText; }
		protected override string GetName() { return GetString(AccStringId.BarLinkStatic); }
		public override string Value { get { return Link.Caption; } }
		protected override AccessibleStates GetState() {
			return AccessibleStates.ReadOnly;
		}
	}
	public class BaseButtonLinkAccessible : BaseLinkAccessible {
		public BaseButtonLinkAccessible(BarBaseButtonItemLink link) : base(link) {	}
		public new BarBaseButtonItemLink Link { get { return (BarBaseButtonItemLink)base.Link; } }
		public new BarBaseButtonItem Item { get { return (BarBaseButtonItem)base.Item; } }
		protected override AccessibleStates GetState() {
			AccessibleStates res = base.GetState();
			res |= AccessibleStates.Focusable;
			if(Link == Link.Manager.SelectionInfo.HighlightedLink) res |= AccessibleStates.Focused; 
			if(Item != null && Item.Down) res |= AccessibleStates.Pressed;
			return res;
		}
	}
	public class ButtonLinkAccessible : BaseButtonLinkAccessible {
		public ButtonLinkAccessible(BarButtonItemLink link) : base(link) {	}
		public new BarButtonItemLink Link { get { return (BarButtonItemLink)base.Link; } }
		public new BarButtonItem Item { get { return (BarButtonItem)base.Item; } }
		protected CustomControl GetPopup() {
			if(GetLinks() == null) return null;
			PopupMenu menu = Item.DropDownControl as PopupMenu;
			if(menu == null) return null;
			return menu.SubControl;
		}
		protected BarItemLinkCollection GetLinks() {
			if(Item != null && Item.IsDropDownButtonStyle) {
				PopupMenu menu = Item.DropDownControl as PopupMenu;
				if(menu != null) return menu.ItemLinks;
			}
			return null;
		}
		protected override ChildrenInfo GetChildrenInfo() {
			if(GetLinks() == null) return null;
			ChildrenInfo res = new ChildrenInfo(ChildType.Item, GetLinks().Count);
			if(Link.Opened && GetPopup() != null) res[ChildType.Toolbar] = 1;
			return res;
		}
		protected override void OnChildrenCountChanged() {
			if(GetLinks() == null) return;
			foreach(BarItemLink link in GetLinks()) {
				AddLink(link);
			}
			if(Link.Opened && GetPopup() != null) AddChild(GetPopup().DXAccessible);
		}
		protected virtual void AddLink(BarItemLink link) {
			AddChild(link.DXAccessible);
			link.DXAccessible.ParentCore = this;
		}
	}
	public class BaseMenuLinkAccessible : BaseLinkAccessible {
		public BaseMenuLinkAccessible(BarCustomContainerItemLink link) : base(link) {
		}
		public new BarCustomContainerItemLink Link { get { return (BarCustomContainerItemLink)base.Link; } }
		public new BarCustomContainerItem Item { get { return (BarCustomContainerItem)base.Item; } }
		protected override AccessibleRole GetRole() { return AccessibleRole.MenuItem; }
		protected BarItemLinkCollection GetLinks() {
			return Item == null ? null : Item.ItemLinks;
		}
		protected override ChildrenInfo GetChildrenInfo() {
			if(GetLinks() == null) return null;
			ChildrenInfo res = new ChildrenInfo(ChildType.Item, GetLinks().Count);
			if(Link.Opened && Link.SubControl != null) res[ChildType.Toolbar] = 1;
			return res;
		}
		protected override void OnChildrenCountChanged() {
			if(GetLinks() == null) return;
			foreach(BarItemLink link in GetLinks()) {
				AddLink(link);
			}
			if(Link.Opened && Link.SubControl != null) AddChild(Link.SubControl.DXAccessible);
		}
		protected virtual void AddLink(BarItemLink link) {
			AddChild(link.DXAccessible);
			link.DXAccessible.ParentCore = this;
		}
		protected override string GetDefaultAction() {
			if(!IsReady || !Link.Enabled || Link.BarControl == null) return null;
			return !Link.Opened ? GetString(AccStringId.BarLinkMenuOpen) : GetString(AccStringId.BarLinkMenuClose);
		}
		protected override void DoDefaultAction() {
			if(!IsReady || !Link.Enabled) return;
			if(!Link.Opened) 
				Link.OpenMenu();
			else
				Link.CloseMenu();
		}
	}
}
