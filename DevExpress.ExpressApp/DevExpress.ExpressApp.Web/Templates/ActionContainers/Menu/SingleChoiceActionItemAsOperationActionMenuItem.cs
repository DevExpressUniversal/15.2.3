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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Web.UnitTesting;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu {
	public class SingleChoiceActionItemAsOperationActionMenuItem : ClickableMenuActionItem
#if DebugTest
, DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable
#endif
 {
		private SingleChoiceActionHelper actionHelper;
		private MenuChoiceActionHelper helper;
		protected MenuChoiceActionHelper MenuChoiceActionHelper { get { return helper; } }
		protected SingleChoiceActionHelper ActionHelper {
			get {
				if(actionHelper == null) {
					actionHelper = new SingleChoiceActionHelper(Action);
				}
				return actionHelper;
			}
		}
		public SingleChoiceActionItemAsOperationActionMenuItem(SingleChoiceAction action)
			: base(action) {
			Action.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(Action_ItemsChanged);
		}
		public new SingleChoiceAction Action {
			get { return (SingleChoiceAction)base.Action; }
		}
		private void OnMenuItemChanged(MenuItemSubItemsChangedEventArgs args) {
			if(MenuItemSubItemsChanged != null) {
				MenuItemSubItemsChanged(this, args);
			}
		}
		protected override XafMenuItem CreateMenuItem() {
			XafMenuItem rootMenuItem = new XafMenuItem(this, this);
			if(!Action.ShowItemsOnClick) {
				rootMenuItem.DropDownMode = true;
			}
			return rootMenuItem;
		}
		public override string GetScript(XafCallbackManager callbackManager, string controlID, string indexPath) {
			string result = "";
			if(!Action.ShowItemsOnClick || helper.HasSingleItem(Action)) {
				result = base.GetScript(callbackManager, controlID, indexPath);
			}
			return result;
		}
		protected virtual void SetToolTip(string toolTip, MenuItem menuItem) {
			if(menuItem != null) {
				menuItem.ToolTip = toolTip;
			}
		}
		protected virtual string GetToolTip() {
			return Action.GetTotalToolTip();
		}
		protected override void SetToolTip(string toolTip) {
			SetToolTip(toolTip, MenuItem);
		}
		protected override void SynchronizeWithActionCore() {
			RebuildContent();
			base.SynchronizeWithActionCore();
		}
		public override void ProcessAction() {
			Action.DoExecute(ActionHelper.FindDefaultItem());
		}
		private void Action_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			SynchronizeWithAction();
		}
		private List<MenuItem> GetMenuItemItems(MenuItem menuItem) {
			List<MenuItem> result = new List<MenuItem>();
			foreach(MenuItem item in menuItem.Items) {
				result.Add(item);
				if(item.Items.Count > 0) {
					result.AddRange(GetMenuItemItems(item));
				}
			}
			return result;
		}
		protected virtual void RebuildContent() {
			if(helper == null && Action != null) {
				helper = new MenuChoiceActionHelper(Action);
			}
			if(helper != null) {
				helper.ClearMap();
				if(Action != null) {
					List<MenuItem> oldItems = GetMenuItemItems(MenuItem);
					helper.RebuildMenu(MenuItem.Items);
					if(Action.Items.Count > 0) {
						List<MenuItem> newItems = GetMenuItemItems(MenuItem);
						OnMenuItemChanged(new MenuItemSubItemsChangedEventArgs(oldItems, newItems, MenuItem));
					}
				}
			}
		}
		public event EventHandler<MenuItemSubItemsChangedEventArgs> MenuItemSubItemsChanged;
		public override void Dispose() {
			if(helper != null) {
				helper.ClearMap();
				helper = null;
			}
			if(Action != null) {
				Action.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(Action_ItemsChanged);
			}
			base.Dispose();
		}
#if DebugTest
		#region ISingleChoiceActionItemUnitTestable Members
		private MenuItem FindItem(string path) {
			return ASPxMenuTestHelper.FindItem(path, MenuItem.Items);
		}
		public bool ItemVisible(string itemPath) {
			return FindItem(itemPath).ClientVisible;
		}
		public bool ItemEnabled(string itemPath) {
			return FindItem(itemPath).ClientEnabled;
		}
		public bool ItemBeginsGroup(string itemPath) {
			return FindItem(itemPath).BeginGroup;
		}
		public bool ItemImageVisible(string itemPath) {
			return !FindItem(itemPath).Image.IsEmpty;
		}
		public bool ItemSelected(string itemPath) {
			return FindItem(itemPath).Selected;
		}
		#endregion
#endif
	}
}
