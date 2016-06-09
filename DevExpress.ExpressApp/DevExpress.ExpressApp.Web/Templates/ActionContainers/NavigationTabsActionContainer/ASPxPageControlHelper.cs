#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	public class ASPxPageControlHelper : IDisposable {
		private List<SingleChoiceActionMenuContainerControl> menus;
		private bool skipPageControlEvent = false;
		private ASPxPageControl pageControl;
		private GroupingStyle groupingStyle = GroupingStyle.GroupItemsIntoTabs;
		private bool showImages = true;
		private bool showTabsImages;
		private SingleChoiceAction navigationAction;
		private Dictionary<TabBase, TabPageChoiceActionItem> tabsToItemsMap = new Dictionary<TabBase, TabPageChoiceActionItem>();
		private LightDictionary<ChoiceActionItem, TabPage> itemsToTabsMap = new LightDictionary<ChoiceActionItem, TabPage>();
		private TabPageChoiceActionItem CreateTabPageChoiceActionItem(ChoiceActionItem actionItem, ChoiceActionBase action) {
			TabPageChoiceActionItem result = new TabPageChoiceActionItem(actionItem, action, showTabsImages);
			tabsToItemsMap[result.TabPage] = result;
			return result;
		}
		private void pageControl_ActiveTabChanged(object source, TabControlEventArgs e) {
			if(!skipPageControlEvent && tabsToItemsMap.ContainsKey(e.Tab)) {
				if(GroupingStyle == GroupingStyle.ShowItemsAsTabs) {
					navigationAction.DoExecute(tabsToItemsMap[e.Tab].ActionItem);
				}
			}
		}
		private void ClearDictionaries() {
			foreach(TabPageChoiceActionItem tabChoiceActioItem in tabsToItemsMap.Values) {
				tabChoiceActioItem.Dispose();
			}
			tabsToItemsMap.Clear();
			itemsToTabsMap.Clear();
		}
		private void BuildPagesWithItemsAsTabs() {
			pageControl.AutoPostBack = true;
			pageControl.ContentStyle.Font.Size = new FontUnit(0);
			foreach(ChoiceActionItem group in navigationAction.Items) {
				if(group.Active) {
					foreach(ChoiceActionItem item in group.Items) {
						if(item.Active) {
							TabPageChoiceActionItem tabPageItem = CreateTabPageChoiceActionItem(item, navigationAction);
							TabPage tabPage = tabPageItem.TabPage;
							pageControl.TabPages.Add(tabPage);
							itemsToTabsMap.Add(item, tabPage);
						}
					}
				}
			}
		}
		private void BuildPagesWithItemsIntoTabs() {
			foreach(ChoiceActionItem group in navigationAction.Items) {
				if(group.Active) {
					TabPageChoiceActionItem tabPageItem = CreateTabPageChoiceActionItem(group, navigationAction);
					TabPage tabPage = tabPageItem.TabPage;
					pageControl.TabPages.Add(tabPage);
					itemsToTabsMap.Add(group, tabPage);
					SingleChoiceActionMenuContainerControl menu = new SingleChoiceActionMenuContainerControl();
					menus.Add(menu);
					menu.ShowImages = ShowImages;
					menu.ID = "M" + tabPage.Index;
					tabPage.Controls.Add(menu);
					List<ChoiceActionItem> items = new List<ChoiceActionItem>();
					foreach(ChoiceActionItem navigationItem in group.Items) {
						if(navigationItem.Active) {
							items.Add(navigationItem);
							itemsToTabsMap.Add(navigationItem, tabPage);
						}
					}
					menu.Register(items, navigationAction);
					menu.ItemClick += new EventHandler<MenuItemEventArgs>(menu_ItemClick);
				}
			}
		}
		private void menu_ItemClick(object sender, MenuItemEventArgs e) {
			navigationAction.DoExecute((ChoiceActionItem)e.Item.DataItem);
		}
		public ASPxPageControlHelper(ASPxPageControl pageControl) {
			menus = new List<SingleChoiceActionMenuContainerControl>();
			this.pageControl = pageControl;
			pageControl.ActiveTabChanged += new TabControlEventHandler(pageControl_ActiveTabChanged);
			if(WebWindow.CurrentRequestPage != null) {
				Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), WebWindow.CurrentRequestPage.GetType(), "Page");
			}
		}
		public void RebuildPages() {
			ClearDictionaries();
			if(pageControl != null && navigationAction != null) {
				pageControl.TabPages.Clear();
				if(GroupingStyle == GroupingStyle.GroupItemsIntoTabs) {
					BuildPagesWithItemsIntoTabs();
				}
				else {
					BuildPagesWithItemsAsTabs();
				}
			}
			UpdateActiveTab();
		}
		private void UpdateActiveTab() {
			if(this.navigationAction !=null && this.navigationAction.SelectedItem != null) {
				pageControl.ActiveTabPage = itemsToTabsMap[this.navigationAction.SelectedItem];
			}
		}
		public SingleChoiceActionMenuContainerControl ActiveTabMenu {
			get {
				if(navigationAction.SelectedItem != null && pageControl.ActiveTabPage.Controls.Count != 0) {
					return (SingleChoiceActionMenuContainerControl)pageControl.ActiveTabPage.Controls[0];
				}
				return null;
			}
		}
		public SingleChoiceAction Navigation {
			get { return navigationAction; }
			set {
				if(navigationAction != value) {
					if(navigationAction != null) {
						navigationAction.SelectedItemChanged -= navigationAction_SelectedItemChanged;
					}
					navigationAction = value;
					if(navigationAction != null) {
						navigationAction.SelectedItemChanged += navigationAction_SelectedItemChanged;
						RebuildPages();
					}
				}
			}
		}
		void navigationAction_SelectedItemChanged(object sender, EventArgs e) {
			UpdateActiveTab();
		}
		public XafApplication Application {
			get {
				if(navigationAction != null) {
					return navigationAction.Application;
				}
				return null;
			}
		}
		public GroupingStyle GroupingStyle {
			get { return groupingStyle; }
			set {
				if(groupingStyle != value) {
					groupingStyle = value;
					RebuildPages();
				}
			}
		}
		public bool ShowImages {
			get { return showImages; }
			set {
				if(showImages != value) {
					showImages = value;
					RebuildPages();
				}
			}
		}
		[DefaultValue(false)]
		public bool ShowTabsImages {
			get { return showTabsImages; }
			set {
				if(showTabsImages != value) {
					showTabsImages = value;
					RebuildPages();
				}
			}
		}
		public void Dispose() {
			Navigation = null;
			if(pageControl != null) {
				pageControl.ActiveTabChanged -= new TabControlEventHandler(pageControl_ActiveTabChanged);
				pageControl.Dispose();
				pageControl = null;
			}
			foreach(SingleChoiceActionMenuContainerControl menu in menus) {
				menu.ItemClick -= new EventHandler<MenuItemEventArgs>(menu_ItemClick);
				menu.Dispose();
			}
			menus.Clear();
			menus = null;
			ClearDictionaries();
		}
		#region DebugTest
#if DebugTest
		public bool IsItemControlVisible(ChoiceActionItem item) {
			bool result = false;
			if(itemsToTabsMap[item] != null) {
				result = itemsToTabsMap[item].Visible;
			}
			return result;
		}
		public string GetGroupControlCaption(ChoiceActionItem item) {
			if(itemsToTabsMap[item] != null) {
				return itemsToTabsMap[item].Text;
			}
			throw new ArgumentOutOfRangeException();
		}
		public int GetGroupChildControlCount(ChoiceActionItem item) {
			if(itemsToTabsMap[item] != null) {
				return ((SingleChoiceActionMenuContainerControl)itemsToTabsMap[item].Controls[0]).Items.Count;
			}
			throw new ArgumentOutOfRangeException();
		}
		public string GetChildControlCaption(ChoiceActionItem item) {
			DevExpress.Web.MenuItem menuItem = ((SingleChoiceActionMenuContainerControl)itemsToTabsMap[item].Controls[0]).GetMenuItem(item);
			if(menuItem != null) {
				return menuItem.Text;
			}
			throw new ArgumentOutOfRangeException();
		}
		public bool GetChildControlEnabled(ChoiceActionItem item) {
			DevExpress.Web.MenuItem menuItem = ((SingleChoiceActionMenuContainerControl)itemsToTabsMap[item].Controls[0]).GetMenuItem(item);
			if(menuItem != null) {
				return menuItem.ClientEnabled;
			}
			throw new ArgumentOutOfRangeException();
		}
		public bool GetChildControlVisible(ChoiceActionItem item) {
			DevExpress.Web.MenuItem menuItem = ((SingleChoiceActionMenuContainerControl)itemsToTabsMap[item].Controls[0]).GetMenuItem(item);
			if(menuItem != null) {
				return menuItem.Visible;
			}
			throw new ArgumentOutOfRangeException();
		}
#endif
		#endregion
	}
}
