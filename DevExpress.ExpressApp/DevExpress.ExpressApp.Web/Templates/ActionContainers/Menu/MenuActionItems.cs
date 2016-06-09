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
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu {
	public enum SingleChoiceActionItemOrientation { Horizontal, Vertical };
	public class DropDownSelectedItemChangedEventArgs : EventArgs {
		private ChoiceActionItem item;
		public ChoiceActionItem Item {
			get {
				return item;
			}
		}
		public DropDownSelectedItemChangedEventArgs(ChoiceActionItem item) {
			this.item = item;
		}
	}	
	public class MenuItemSubItemsChangedEventArgs : EventArgs {
		private List<MenuItem> oldMenuItems;
		private List<MenuItem> newMenuItems;
		private MenuItem menuItem;
		public List<MenuItem> OldMenuItems {
			get { return oldMenuItems; }
		}
		public List<MenuItem> NewMenuItems {
			get { return newMenuItems; }
		}
		public MenuItem MenuItem {
			get { return menuItem; }
		}
		public MenuItemSubItemsChangedEventArgs(List<MenuItem> oldMenuItems, List<MenuItem> newMenuItems, MenuItem menuItem) {
			this.oldMenuItems = oldMenuItems;
			this.newMenuItems = newMenuItems;
			this.menuItem = menuItem;
		}
	}
	public class CreateCustomMenuActionItemEventArgs : EventArgs {
		private MenuActionItemBase actionItem;
		private ActionBase action;
		public CreateCustomMenuActionItemEventArgs(ActionBase action) {
			this.action = action;
		}
		public ActionBase Action { get { return action; } }
		public MenuActionItemBase ActionItem {
			get { return actionItem; }
			set { actionItem = value; }
		}
	}
	public class MenuActionItemCreatedEventArgs : EventArgs {
		private MenuActionItemBase actionItem;
		public MenuActionItemCreatedEventArgs(MenuActionItemBase actionItem) {
			this.actionItem = actionItem;
		}
		public MenuActionItemBase ActionItem {
			get { return actionItem; }
		}
	}
	public interface ISupportActionProcessing {
		void ProcessAction();
		ActionBase Action { get; }
	}
#if DebugTest
	public
#endif
	interface IClientClickScriptProvider {
		string GetScript(XafCallbackManager callbackManager, string controlID, string indexPath);
	}
}
