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
using System.Text;
using DevExpress.ExpressApp.Utils;
using DevExpress.Web;
using DevExpress.ExpressApp.Actions;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	public class MenuChoiceActionHelper {
		private SingleChoiceAction action;
		private Dictionary<MenuItem, MenuChoiceActionItem> menuItemToMenuChoiceActionItemMap;
		public SingleChoiceAction Action {
			get { return action; }
		}
		public MenuChoiceActionHelper(SingleChoiceAction action) {
			this.action = action;
			menuItemToMenuChoiceActionItemMap = new Dictionary<MenuItem, MenuChoiceActionItem>();
		}
		public void ClearMap() { 
			foreach(MenuChoiceActionItem item in menuItemToMenuChoiceActionItemMap.Values) {
				item.Dispose();
			}
			menuItemToMenuChoiceActionItemMap.Clear();
		}
		public List<MenuChoiceActionItem> RebuildMenu(MenuItemCollection items) {
			List<MenuChoiceActionItem> result = new List<MenuChoiceActionItem>();
			items.Clear(); 
			ClearMap();
			if(Action.Items.Count > 1 || !HasSingleItem(Action)) {
				foreach(ChoiceActionItem actionItem in Action.Items) {
					MenuChoiceActionItem menuItem = CreateMenuChoiceActionItem(actionItem, Action);
					result.Add(menuItem);
					items.Add(menuItem.MenuItem);
					result.AddRange(CreateSubItems(menuItem.MenuItem, actionItem.Items, Action));
				}
			}
			return result;
		}
		public bool HasSingleItem(SingleChoiceAction action) {
			return action.Items.Count == 1 && action.Items[0].Items.Count == 0;
		}
		private MenuChoiceActionItem CreateMenuChoiceActionItem(ChoiceActionItem actionItem, ChoiceActionBase action) {
			MenuChoiceActionItem result = new MenuChoiceActionItem(actionItem, action);
			menuItemToMenuChoiceActionItemMap[result.MenuItem] = result;
			return result;
		}
		private List<MenuChoiceActionItem> CreateSubItems(DevExpress.Web.MenuItem parentMenuItem, ChoiceActionItemCollection choiceItems, ChoiceActionBase action) {
			List<MenuChoiceActionItem> result = new List<MenuChoiceActionItem>();
			foreach(ChoiceActionItem actionItem in choiceItems) {
				MenuChoiceActionItem menuItem = CreateMenuChoiceActionItem(actionItem, action);
				result.Add(menuItem);
				parentMenuItem.Items.Add(menuItem.MenuItem);
				result.AddRange(CreateSubItems(menuItem.MenuItem, actionItem.Items, action));
			}
			return result;
		}
		public void UpdateSelectedItem(DevExpress.Web.MenuItemCollection menuItems) {
			foreach(DevExpress.Web.MenuItem menuItem in menuItems) {
				MenuChoiceActionItem actionMenuItem = menuItemToMenuChoiceActionItemMap[menuItem];
				if(actionMenuItem != null) {
					if(actionMenuItem.ActionItem == Action.SelectedItem) {
						actionMenuItem.MenuItem.Checked = true;
						actionMenuItem.MenuItem.Menu.SelectedItem = actionMenuItem.MenuItem;
					}
					else {
						actionMenuItem.MenuItem.Checked = false;
					}
				}
				UpdateSelectedItem(menuItem.Items);
			}
		}
		public ChoiceActionItem GetActionItemByMenuItem(MenuItem menuItem) {
			return menuItemToMenuChoiceActionItemMap.ContainsKey(menuItem) ? menuItemToMenuChoiceActionItemMap[menuItem].ActionItem : null;
		}
	}
}
