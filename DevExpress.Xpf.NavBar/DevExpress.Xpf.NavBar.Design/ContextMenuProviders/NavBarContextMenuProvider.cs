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

extern alias Platform;
using System;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
#if SL
using Platform::DevExpress.Xpf.NavBar;
#endif
namespace DevExpress.Xpf.NavBar.Design {
	class NavBarContextMenuProvider : PrimarySelectionContextMenuProvider {
		MenuAction AddNavBarGroupAction { get; set; }
		NavBarViewMenuAction NavPaneAction { get; set; }
		NavBarViewMenuAction ExplorerBarAction { get; set; }
		NavBarViewMenuAction SideBarAction { get; set; }
		MenuGroup SetNavBarView { get; set; }
		public NavBarContextMenuProvider() {
			AddNavBarGroupAction = new MenuAction("Add NavBarGroup");
			AddNavBarGroupAction.Execute += OnAddNavBarGroupActionExecute;
			NavPaneAction = new NavBarViewMenuAction("Navigation Pane", typeof(NavigationPaneView)) { Checkable = true };
			NavPaneAction.Execute += OnSetViewActionExecute;
			ExplorerBarAction = new NavBarViewMenuAction("Explorer Bar", typeof(ExplorerBarView)) { Checkable = true };
			ExplorerBarAction.Execute += OnSetViewActionExecute;
			SideBarAction = new NavBarViewMenuAction("Side Bar", typeof(SideBarView)) { Checkable = true };
			SideBarAction.Execute += OnSetViewActionExecute;
			SetNavBarView = new MenuGroup("Set View") { HasDropDown = true };
			SetNavBarView.Items.Add(ExplorerBarAction);
			SetNavBarView.Items.Add(NavPaneAction);
			SetNavBarView.Items.Add(SideBarAction);
			Items.Add(SetNavBarView);
			Items.Add(AddNavBarGroupAction);
			UpdateItemStatus += OnUpdateItemStatus;
		}
		void OnSetViewActionExecute(object sender, MenuActionEventArgs e) {
			NavBarViewMenuAction action = sender as NavBarViewMenuAction;
			ModelItem navBar = e.Selection.PrimarySelection;
			if(action == null || navBar == null) return;
			using(ModelEditingScope scope = navBar.BeginEdit("Set View")) {
				navBar.Properties["View"].SetValue(ModelFactory.CreateItem(navBar.Context, action.ViewType));
				scope.Complete();
			}
		}
		void OnUpdateItemStatus(object sender, MenuActionEventArgs e) {
			ModelItem navBar = e.Selection.PrimarySelection;
			if(navBar == null) return;
			foreach(NavBarViewMenuAction action in SetNavBarView.Items) {
				action.Checked = action.ViewType == navBar.Properties["View"].Value.ItemType;
			}
		}
		void OnAddNavBarGroupActionExecute(object sender, MenuActionEventArgs e) {
			ModelItem navBar = e.Selection.PrimarySelection;
			if(navBar == null) return;
			NavBarDesignTimeHelper.AddNewNavBarGroup(navBar);
		}
	}
	class NavBarViewMenuAction : MenuAction {
		public Type ViewType { get; private set; }
		public NavBarViewMenuAction(string displayName, Type viewType)
			: base(displayName) {
			if(!viewType.IsSubclassOf(typeof(NavBarViewBase))) throw new ArgumentException("viewType has incorrect value");
			ViewType = viewType;
		}
	}
}
