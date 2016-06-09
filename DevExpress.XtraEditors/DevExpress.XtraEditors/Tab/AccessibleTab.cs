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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraTab;
using DevExpress.XtraTab.Buttons;
using DevExpress.XtraTab.ViewInfo;
namespace DevExpress.Accessibility.Tab {
	public class TabPageAccessible : XtraScrollableControlAccessible {
		IXtraTab tab;
		public TabPageAccessible(XtraTabPage page)
			: base(page) {
			tab = page.TabControl;
		}
		protected virtual IXtraTabPage Page {
			get { return Control as IXtraTabPage; }
		}
		protected override AccessibleRole GetRole() {
			return AccessibleRole.Client;
		}
		protected override string GetName() {
			return Page.Text;
		}
		public override Rectangle ClientBounds {
			get {
				if(tab == null || tab.ViewInfo == null)
					return Rectangle.Empty;
				return tab.ViewInfo.PageClientBounds;
			}
		}
		public override Control GetOwnerControl() {
			return (tab != null) ? tab.OwnerControl : null;
		}
		protected override AccessibleStates GetState() {
			return AccessibleStates.Focusable;
		}
	}
	public class TabControlAccessible : BaseAccessible {
		IXtraTab tabControl;
		public TabControlAccessible(IXtraTab tabControl) {
			this.tabControl = tabControl;
			if(tabControl is XtraTabControl)
				CustomInfo = new AccessibleUserInfoHelper(TabControl as Control);
		}
		protected IXtraTab TabControl { get { return tabControl; } }
		protected BaseTabControlViewInfo ViewInfo { get { return TabControl.ViewInfo; } }
		public override Control GetOwnerControl() { return TabControl.OwnerControl; }
		protected override AccessibleRole GetRole() { return AccessibleRole.PageTabList; }
		protected virtual bool HasButtons {
			get { return ViewInfo != null && !ViewInfo.HeaderInfo.ButtonsBounds.IsEmpty; }
		}
		protected override ChildrenInfo GetChildrenInfo() {
			if(ViewInfo == null) return null;
			ChildrenInfo info = new ChildrenInfo("Tab", TabControl.PageCount);
			if(ViewInfo != null) {
				if(HasButtons) {
					info["Buttons"] = ViewInfo.HeaderInfo.Buttons.Buttons.Count;
				}
				if(GetSelectedPage() != null) {
					info[GetSelectedPage()] = 1;
				}
			}
			return info;
		}
		protected IXtraTabPage GetSelectedPage() {
			return ViewInfo == null ? null : ViewInfo.SelectedTabPage;
		}
		protected override void OnChildrenCountChanged() {
			if(ViewInfo == null) return;
			for(int n = 0; n < ViewInfo.HeaderInfo.Buttons.Buttons.Count; n++) {
				AddTabHeaderButton(n);
			}
			for(int n = 0; n < TabControl.PageCount; n++) {
				AddTabPageHeader(n, TabControl.GetTabPage(n));
			}
			if(GetSelectedPage() != null) {
				XtraTabPage page = GetSelectedPage() as XtraTabPage;
				if(page != null)
					AddChild(new TabPageClientAccessible(TabControl, page));
				else
					AddChild(new TabClientAccessible(TabControl));
			}
		}
		protected void AddTabPageHeader(int index, IXtraTabPage page) {
			AddChild(new TabPageHeaderAccessible(TabControl, page));
		}
		protected void AddTabHeaderButton(int index) {
			AddChild(new TabHeaderButtonAccessible(TabControl, index));
		}
		protected class TabPageClientAccessible : BaseAccessible {
			IXtraTab tabControl;
			XtraTabPage page;
			public TabPageClientAccessible(IXtraTab tabControl, XtraTabPage page) {
				this.page = page;
				this.tabControl = tabControl;
			}
			protected override string GetName() { return page.Text; }
			protected override AccessibleRole GetRole() { return AccessibleRole.Window; }
			public override Control GetOwnerControl() { return tabControl.OwnerControl; }
			public override Rectangle ClientBounds {
				get {
					if(tabControl.ViewInfo == null) return Rectangle.Empty;
					return tabControl.ViewInfo.PageClientBounds;
				}
			}
			protected override AccessibleStates GetState() {
				AccessibleStates states = AccessibleStates.Focusable;
				if(page.ContainsFocus)
					states |= AccessibleStates.Focused;
				return states;
			}
			protected override ChildrenInfo GetChildrenInfo() {
				return new ChildrenInfo(page, 1);
			}
			protected override void OnChildrenCountChanged() {
				AddChild(page.DXAccessibleInternal);
			}
		}
		protected class TabClientAccessible : BaseAccessible {
			IXtraTab tabControl;
			public TabClientAccessible(IXtraTab tabControl) {
				this.tabControl = tabControl;
			}
			protected IXtraTabPage GetSelectedPage() {
				return tabControl.ViewInfo == null ? null : tabControl.ViewInfo.SelectedTabPage;
			}
			protected override string GetName() { return GetSelectedPage() == null ? null : GetSelectedPage().Text; }
			protected override AccessibleRole GetRole() { return AccessibleRole.Client; }
			public override Control GetOwnerControl() { return tabControl.OwnerControl; }
			public override Rectangle ClientBounds {
				get {
					if(tabControl.ViewInfo == null) return Rectangle.Empty;
					return tabControl.ViewInfo.PageBounds;
				}
			}
		}
		protected class TabHeaderButtonAccessible : ButtonAccessible {
			IXtraTab tabControl;
			int button;
			public TabHeaderButtonAccessible(IXtraTab tabControl, int button) {
				this.tabControl = tabControl;
				this.button = button;
			}
			protected TabButtonInfo GetButton() {
				if(tabControl.ViewInfo == null || button >= tabControl.ViewInfo.HeaderInfo.Buttons.Buttons.Count) return null;
				return tabControl.ViewInfo.HeaderInfo.Buttons.Buttons[button];
			}
			protected bool CanClick { get { return GetButton() != null && GetButton().State != ObjectState.Disabled; } }
			protected override string GetDefaultAction() {
				if(!CanClick) return null;
				if(GetName() != null) return GetName();
				return base.GetDefaultAction();
			}
			protected override AccessibleStates GetState() {
				if(!CanClick) return AccessibleStates.Unavailable;
				return AccessibleStates.Default;
			}
			protected override void DoDefaultAction() {
				if(CanClick && GetButton() != null) {
					tabControl.ViewInfo.HeaderInfo.Buttons.OnClickButton(GetButton());
				}
			}
			protected override string GetName() {
				if(GetButton() == null) return null;
				return tabControl.ViewInfo.HeaderInfo.Buttons.GetButtonTooltip(GetButton());
			}
			public override Rectangle ClientBounds { get { return GetButton() != null ? GetButton().Bounds : Rectangle.Empty; } }
		}
	}
	public class TabPageHeaderAccessible : BaseAccessible {
		IXtraTab tabControl;
		IXtraTabPage page;
		public TabPageHeaderAccessible(IXtraTab tabControl, IXtraTabPage page) {
			this.tabControl = tabControl;
			this.page = page;
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.PageTab; }
		public override Control GetOwnerControl() { return tabControl.OwnerControl; }
		protected override string GetName() { return page.Text; }
		protected override AccessibleStates GetState() {
			AccessibleStates state = AccessibleStates.None;
			if(!page.PageEnabled) state |= AccessibleStates.Unavailable;
			if(!page.PageVisible) state |= AccessibleStates.Invisible;
			if(page.PageEnabled && page.PageVisible) {
				state |= AccessibleStates.Selectable;
				state |= AccessibleStates.Focusable;
			}
			if(tabControl.ViewInfo != null && tabControl.ViewInfo.SelectedTabPage == page)
				state |= AccessibleStates.Selected;
			Control pageControl = page as Control;
			if(pageControl != null && pageControl.CanFocus)
				state |= AccessibleStates.Focusable;
			if(pageControl != null && pageControl.ContainsFocus)
				state |= AccessibleStates.Focused;
			return state;
		}
		protected override string GetDefaultAction() {
			return GetString(AccStringId.TabSwitch);
		}
		protected override void DoDefaultAction() {
			SelectPage();
		}
		protected override void Select(AccessibleSelection flags) {
			if((flags & (AccessibleSelection.TakeFocus | AccessibleSelection.TakeSelection | AccessibleSelection.AddSelection)) != 0)
				SelectPage();
			if((flags & AccessibleSelection.TakeFocus) != 0) {
				Control pageControl = page as Control;
				if(pageControl != null)
					pageControl.SelectNextControl(pageControl, true, true, true, false);
			}
		}
		void SelectPage() {
			if(tabControl.ViewInfo != null) tabControl.ViewInfo.SelectedTabPage = page;
		}
		public override Rectangle ClientBounds {
			get {
				if(tabControl.ViewInfo != null) {
					BaseTabPageViewInfo pi = tabControl.ViewInfo.HeaderInfo.AllPages[page];
					if(pi != null) return pi.Bounds;
				}
				return Rectangle.Empty;
			}
		}
		protected override ChildrenInfo GetChildrenInfo() {
			if(page == null) return null;
			ChildrenInfo info = new ChildrenInfo("CloseButton", 1);
			BaseTabPageViewInfo tabPageViewInfo = page.TabControl.ViewInfo.HeaderInfo.AllPages[page];
			BaseTabHeaderViewInfo headerInfo = page.TabControl.ViewInfo.HeaderInfo;
			bool canShowClose = tabPageViewInfo != null && headerInfo.CanShowCloseButtonForPage(tabPageViewInfo) && headerInfo.CanShowPageCloseButtons();
			if(canShowClose) {
				info["CloseButton"] = 1;
				return info;
			}
			return null;
		}
		protected override void OnChildrenCountChanged() {
			AddChild(new TabPageButtonAccessible(page));
		}
		protected class TabPageButtonAccessible : ButtonAccessible {
			IXtraTabPage tabPageCore;
			BaseTabPageViewInfo tabPageViewInfoCore;
			public TabPageButtonAccessible(IXtraTabPage tabPage) {
				tabPageCore = tabPage;
			}
			protected bool CanClick {
				get { return CloseButton != null && CloseButton.Properties.Visible && CloseButton.Properties.Enabled; }
			}
			protected override string GetDefaultAction() {
				if(!CanClick) { return null; }
				if(GetName() != null) return GetName();
				return base.GetDefaultAction();
			}
			protected override AccessibleStates GetState() {
				BaseTabPageViewInfo tabPageViewInfo = TabPage.TabControl.ViewInfo.HeaderInfo.AllPages[TabPage];
				bool CanShowClose = TabPage.TabControl.ViewInfo.HeaderInfo.CanShowCloseButtonForPage(tabPageViewInfo) && TabPage.TabControl.ViewInfo.HeaderInfo.CanShowPageCloseButtons();
				if(!CanShowClose) return AccessibleStates.Unavailable;
				return AccessibleStates.Default;
			}
			protected IXtraTabPage TabPage {
				get { return tabPageCore; }
			}
			protected BaseTabPageViewInfo TabPageViewInfo {
				get {
					if(tabPageViewInfoCore == null)
						tabPageViewInfoCore = TabPage.TabControl.ViewInfo.HeaderInfo.AllPages[TabPage];
					return tabPageViewInfoCore;
				}
			}
			protected override void DoDefaultAction() {
				if(CanClick) {
					if(TabPageViewInfo != null && TabPage.PageEnabled)
						TabPage.TabControl.ViewInfo.OnPageCloseButtonClick(new ClosePageButtonEventArgs(TabPage, TabPage));
				}
			}
			protected override string GetName() {
				if(CloseButton != null)
					return CloseButton.Properties.ToolTip;
				return null;
			}
			public override Rectangle ClientBounds { get { return GetCloseButtonRect(); } }
			protected Rectangle GetCloseButtonRect() {
				BaseTabPageViewInfo tabPageViewInfo = TabPage.TabControl.ViewInfo.HeaderInfo.AllPages[TabPage];
				if(tabPageViewInfo.ButtonsPanel != null && tabPageViewInfo.ButtonsPanel.ViewInfo != null && tabPageViewInfo.ButtonsPanel.ViewInfo.Buttons != null) {
					foreach(BaseButtonInfo item in tabPageViewInfo.ButtonsPanel.ViewInfo.Buttons) {
						if(item.Button is BaseCloseButton)
							return item.Bounds;
					}
				}
				return Rectangle.Empty;
			}
			IBaseButton closeButtonCore;
			protected IBaseButton CloseButton {
				get {
					if(closeButtonCore == null)
						closeButtonCore = GetCloseButton();
					return closeButtonCore;
				}
			}
			protected IBaseButton GetCloseButton() {
				if(TabPageViewInfo != null) {
					for(int i = 0; i < TabPageViewInfo.ButtonsPanel.Buttons.Count; i++) {
						if(TabPageViewInfo.ButtonsPanel.Buttons[i] is BaseCloseButton) {
							return TabPageViewInfo.ButtonsPanel.Buttons[i];
						}
					}
				}
				return null;
			}
		}
	}
}
