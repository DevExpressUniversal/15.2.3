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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraEditors;
using DevExpress.Utils.Drawing;
using DevExpress.Accessibility;
using DevExpress.XtraNavBar;
using DevExpress.XtraNavBar.ViewInfo;
namespace DevExpress.XtraNavBar.Accessibility {
	public class BaseNavBarAccessible : BaseControlAccessible {
		NavBarViewInfo viewInfo;
		NavBarControl navBar;
		public BaseNavBarAccessible(NavBarControl navBar, NavBarViewInfo viewInfo) : base(navBar) {
			this.viewInfo = viewInfo;
			this.navBar = navBar;
		}
		public override bool IsTopControl { get { return true; } }
		protected NavBarViewInfo ViewInfo { get { return viewInfo; } }
		protected NavBarControl NavBar { get { return navBar; } }
		protected override AccessibleRole GetRole() { return AccessibleRole.Client; }
		protected override ChildrenInfo GetChildrenInfo() { 
			ChildrenInfo res = new ChildrenInfo(ChildType.Item, NavBar.Groups.Count); 
			if(!ViewInfo.UpButtonBounds.IsEmpty) res["Up"] = 1;
			if(!ViewInfo.DownButtonBounds.IsEmpty) res["Down"] = 1;
			return res;
		}
		protected override void OnChildrenCountChanged() {
			if(!ViewInfo.UpButtonBounds.IsEmpty) AddChild(new NavBarScrollButton(NavBar, true));
			foreach(NavBarGroup group in NavBar.Groups) {
				AddGroup(group);
			}
			if(!ViewInfo.DownButtonBounds.IsEmpty) AddChild(new NavBarScrollButton(NavBar, false));
		}
		protected virtual void AddGroup(NavBarGroup group) {
			AddChild(new NavBarGroupAccessible(NavBar, group));
		}
	}
	public class NavBarScrollButton : BaseAccessible {
		NavBarControl navBar;
		bool isUpButton;
		public NavBarScrollButton(NavBarControl navBar, bool isUpButton) {
			this.isUpButton = isUpButton;
			this.navBar = navBar;
		}
		public bool IsUpButton { get { return isUpButton; } }
		public NavBarControl NavBar { get { return navBar; } }
		public NavBarViewInfo ViewInfo { get { return NavBar.ViewInfo; } }
		public override Rectangle ClientBounds {
			get {
				return IsUpButton ? ViewInfo.UpButtonBounds : ViewInfo.DownButtonBounds;
			}
		}
		protected override AccessibleStates GetState() {
			return (IsUpButton ? ViewInfo.UpButtonState : ViewInfo.DownButtonState) == ObjectState.Disabled ? AccessibleStates.Unavailable : AccessibleStates.Default;
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.ScrollBar; }
		protected override string GetName() { return IsUpButton ? GetString(AccStringId.NavBarScrollUp) : GetString(AccStringId.NavBarScrollDown); }
		protected override string GetDefaultAction() { 
			if((GetState() & AccessibleStates.Unavailable) != 0) return null;
			return GetName(); 
		}
		protected override void DoDefaultAction() {
			if(ViewInfo == null || ClientBounds.IsEmpty) return;
			if((GetState() & AccessibleStates.Unavailable) != 0) return;
			NavBarHitInfo hitInfo = ViewInfo.CreateHitInfo();
			hitInfo.SetGroup(NavBar.ActiveGroup);
			NavBar.ViewInfo.OnButtonPress(hitInfo, IsUpButton);
		}
	}
	public class ExplorerNavBarAccessible : BaseNavBarAccessible {
		public ExplorerNavBarAccessible(NavBarControl navBar, ExplorerBarNavBarViewInfo viewInfo) : base(navBar, viewInfo) { }
		protected new ExplorerBarNavBarViewInfo ViewInfo { get { return (ExplorerBarNavBarViewInfo)base.ViewInfo; } }
		protected override ChildrenInfo GetChildrenInfo() { 
			ChildrenInfo res = base.GetChildrenInfo();
			if(ViewInfo.ScrollBarVisible && ViewInfo.ScrollBar != null) res[ChildType.VScroll] = 1;
			return res;
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			if(ViewInfo.ScrollBarVisible && ViewInfo.ScrollBar != null) 
				AddChild(ViewInfo.ScrollBar.GetAccessible());
		}
	}
	public class NavigationPaneAccessible : BaseNavBarAccessible {
		public NavigationPaneAccessible(NavBarControl navBar, NavBarViewInfo viewInfo) : base(navBar, viewInfo) { }
		protected new NavigationPaneViewInfo ViewInfo { get { return (NavigationPaneViewInfo)base.ViewInfo; } }
		protected override ChildrenInfo GetChildrenInfo() { 
			if(NavBar.Groups.Count == 0) return null;
			ChildrenInfo res = new ChildrenInfo(ChildType.Item, NavBar.Groups.Count - 1); 
			if(NavBar.ActiveGroup != null && ViewInfo.HeaderInfo.Bounds.IsEmpty) return res;
			res[NavBar.ActiveGroup] = 1;
			return res;
		}
		protected override void OnChildrenCountChanged() {
			if(NavBar.ActiveGroup != null && !ViewInfo.HeaderBounds.IsEmpty) {
				AddChild(new NavigationClientAccessible(NavBar, ViewInfo));
			}
			foreach(NavBarGroup group in NavBar.Groups) {
				AddGroup(group);
			}
		}
		protected class NavigationClientAccessible : NavBarGroupAccessible {
			NavigationPaneViewInfo viewInfo;
			public NavigationClientAccessible(NavBarControl navBar, NavigationPaneViewInfo viewInfo) : base(navBar, navBar.ActiveGroup) {
				this.viewInfo = viewInfo;
			}
			protected override NavBarGroup Group { get { return NavBar.ActiveGroup; } }
			public new NavigationPaneViewInfo ViewInfo { get { return viewInfo; } }
			protected override AccessibleRole GetRole() { return AccessibleRole.Grouping; }
			protected override string GetName() { return ViewInfo == null ? null : ViewInfo.HeaderInfo.Caption; }
			public override Rectangle ClientBounds { 
				get { 
					if(ViewInfo == null) return Rectangle.Empty;
					Rectangle res = ViewInfo.HeaderBounds;
					NavGroupInfoArgs info = ViewInfo.GetGroupInfo(Group);
					if(info == null || info.ClientInfoBounds.IsEmpty) return res;
					Rectangle client = info.ClientInfoBounds;
					if(res.IsEmpty) return client;
					res.Height = client.Bottom - res.Top;
					return res;
				}
			}
		}
		protected override void AddGroup(NavBarGroup group) {
			if(group == NavBar.ActiveGroup) return;
			AddChild(new NavigationPaneGroupAccessible(NavBar, group));
		}
		protected class NavigationPaneGroupAccessible : NavBarGroupAccessible {
			public NavigationPaneGroupAccessible(NavBarControl navBar, NavBarGroup group) : base(navBar, group) {
			}
			public new NavigationPaneViewInfo ViewInfo { get { return base.ViewInfo as NavigationPaneViewInfo; } }
			protected override ChildrenInfo GetChildrenInfo() { return null; }
			protected override void OnChildrenCountChanged() { }
			protected override AccessibleRole GetRole() { return AccessibleRole.PushButton; }
			protected override string GetDefaultAction() { return GetString(AccStringId.NavBarItemClick); }
			protected override void DoDefaultAction() { ViewInfo.DoGroupClick(Group); }
			public override Rectangle ClientBounds {
				get {
					if(ViewInfo == null) return Rectangle.Empty;
					NavGroupInfoArgs info = ViewInfo.GetGroupInfo(Group);
					if(info != null && !info.Bounds.IsEmpty) return info.Bounds;
					NavigationPaneOverflowPanelObjectInfo bi = ViewInfo.OverflowInfo.GetButtonInfo(Group);
					if(bi != null) return bi.Bounds;
					return Rectangle.Empty;
				}
			}
		}
	}
	public class NavBarGroupAccessible : BaseAccessible {
		NavBarGroup group;
		NavBarControl navBar;
		public NavBarGroupAccessible(NavBarControl navBar, NavBarGroup group) {
			this.group = group;
			this.navBar = navBar;
		}
		protected NavBarViewInfo ViewInfo { get { return NavBar.ViewInfo; } }
		protected NavBarControl NavBar { get { return navBar; } }
		protected virtual NavBarGroup Group { get { return group; } }
		protected override ChildrenInfo GetChildrenInfo() {
			if(Group.GroupStyle == NavBarGroupStyle.ControlContainer) {
				if(Group.ControlContainer == null) return null;
				return new ChildrenInfo(ChildType.Item, 1);
			}
			return new ChildrenInfo(ChildType.Item, Group.ItemLinks.Count);
		}
		protected override void OnChildrenCountChanged() {
			if(Group.GroupStyle == NavBarGroupStyle.ControlContainer) {
				if(Group.ControlContainer != null) AddContainer(Group.ControlContainer);
				return;
			}
			foreach(NavBarItemLink link in Group.ItemLinks) {
				AddLink(link);
			}
		}
		protected override string GetName() { return Group.Caption; }
		protected virtual void AddLink(NavBarItemLink link) {
			AddChild(new NavBarLinkAccessible(NavBar, link));
		}
		protected virtual void AddContainer(NavBarGroupControlContainer container) {
			AddChild(container.DXAccessible);
		}
		public override Rectangle ClientBounds {
			get {
				NavGroupInfoArgs info = ViewInfo.GetGroupInfo(Group);
				if(info == null) return Rectangle.Empty;
				Rectangle res = info.Bounds, client = Rectangle.Empty;
				if(!info.ClientInfoBounds.IsEmpty) client = info.ClientInfoBounds;
				if(res.IsEmpty) return client;
				if(client.IsEmpty) return res;
				if(res.Bottom < client.Bottom) res.Height = client.Bottom - res.Top;
				return res;
			}
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.Client; }
		protected override string GetDefaultAction() {
			if(ViewInfo == null || !Group.IsVisible) return null;
			if(ViewInfo.AllowExpandCollapse) return Group.Expanded ? GetString(AccStringId.NavBarGroupCollapse) : GetString(AccStringId.NavBarGroupExpand);
			return GetString(AccStringId.NavBarItemClick);
		}
		protected override void DoDefaultAction() {
			if(ViewInfo == null || !Group.IsVisible) return;
			if(ViewInfo.AllowExpandCollapse) {
				Group.Expanded = !Group.Expanded;
				return;
			}
			ViewInfo.DoGroupClick(Group);
		}
		protected override AccessibleStates GetState() {
			AccessibleStates res = AccessibleStates.None;
			if(!Group.IsVisible) res |= AccessibleStates.Invisible;
			if(ViewInfo != null && ViewInfo.AllowExpandCollapse) res |= (Group.Expanded ? AccessibleStates.Expanded : AccessibleStates.Collapsed);
			return res;
		}
	}
	public class NavBarLinkAccessible : BaseAccessible {
		NavBarControl navBar;
		NavBarItemLink link;
		public NavBarLinkAccessible(NavBarControl navBar, NavBarItemLink link) {
			this.navBar = navBar;
			this.link = link;
		}
		protected override string GetName() { return Link.Caption; }
		protected NavBarViewInfo ViewInfo { get { return NavBar.ViewInfo; } }
		public NavBarControl NavBar { get { return navBar; } }
		public NavBarItemLink Link { get { return link; } }
		protected override AccessibleRole GetRole() { return AccessibleRole.Link; }
		protected override AccessibleStates GetState() {
			AccessibleStates res = AccessibleStates.None;
			if(!Link.Visible || ClientBounds.IsEmpty) res |= AccessibleStates.Invisible;
			if(!Link.Enabled) res |= AccessibleStates.Unavailable;
			if(NavBar.ViewInfo.PressedInfo.InLink && NavBar.ViewInfo.PressedInfo.Link == Link) res |= AccessibleStates.Pressed | AccessibleStates.Selected | AccessibleStates.Focused | AccessibleStates.Focusable | AccessibleStates.Selectable;
			return res;
		}
		protected override string GetDefaultAction() { return Link.Enabled ? GetString(AccStringId.NavBarItemClick) : null; }
		protected override void DoDefaultAction() { 
			if(Link.Enabled) NavBar.RaiseLinkClicked(Link);
		}
		public override Rectangle ClientBounds {
			get {
				if(ViewInfo == null) return Rectangle.Empty;
				NavLinkInfoArgs info = ViewInfo.GetLinkInfo(Link);
				return info == null ? Rectangle.Empty : info.Bounds;
			}
		}
	}
}
