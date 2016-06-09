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
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System.Windows.Controls;
using System;
namespace DevExpress.Xpf.Ribbon.Design {
	class RibbonStatusBarControlContextMenuProvider : ContainerContextMenuProvider {
		MenuGroup AddBarItemToLeftItemLinks { get; set; }
		MenuGroup AddBarEditItemToLeftItemLinks { get; set; }
		MenuGroup AddBarItemToRightItemLinks { get; set; }
		MenuGroup AddBarEditItemToRightItemLinks { get; set; }
		MenuGroup AddBarItemLinkToLeft { get; set; }
		MenuGroup AddBarItemLinkToRight { get; set; }
		protected override void InitializeMenuGroups() {
			base.InitializeMenuGroups();
			AddBarItemToLeftItemLinks = new MenuGroup("Add to Left") { HasDropDown = true };
			AddBarEditItemToLeftItemLinks = new MenuGroup("BarEditItem") { HasDropDown = true };
			AddBarItemToRightItemLinks = new MenuGroup("Add to Right") { HasDropDown = true };
			AddBarEditItemToRightItemLinks = new MenuGroup("BarEditItem") { HasDropDown = true };
			AddBarItemLinkToLeft = new MenuGroup("Add BarItemLink") { HasDropDown = true };
			AddBarItemLinkToRight = new MenuGroup("Add BarItemLink") { HasDropDown = true };
		}
		protected override void AddGroups() {
			AddBarItemGroup.Items.Add(AddEditItemGroup);
		}
		protected override void OnAddBarItemExecute(object sender, MenuActionEventArgs e) {
			AddBarItemMenuAction action = sender as AddBarItemMenuAction;
			if(action == null || !(action.Tag is RibbonStatusBarItemLinkType)) return;
			ModelItem newBarItem = CreateNewBarItem(e.Context, action.IsBarEditItem, action.BarItemType);
			ModelItem statusBar = e.Selection.PrimarySelection;
			ModelItem barManager = RibbonDesignTimeHelper.FindBarManager(statusBar);
			if(barManager == null) return;
			ModelItem link = RibbonDesignTimeHelper.CreateBarItemLink(newBarItem);
			BarManagerDesignTimeHelper.AddBarItem(barManager, newBarItem);
			if((RibbonStatusBarItemLinkType)action.Tag == RibbonStatusBarItemLinkType.Left)
				BarManagerDesignTimeHelper.AddBarItemLink(statusBar.Properties["LeftItemLinks"], link);
			else if((RibbonStatusBarItemLinkType)action.Tag == RibbonStatusBarItemLinkType.Right)
				BarManagerDesignTimeHelper.AddBarItemLink(statusBar.Properties["RightItemLinks"], link);
		}
		protected override void AddItemsForGroup() {
			base.AddItemsForGroup();
			AddBarItemToLeftItemLinks.Items.Add(AddBarItemLinkToLeft);
			AddBarItemToRightItemLinks.Items.Add(AddBarItemLinkToRight);
			InitializeItemLinksGroup(AddEditItemGroup, AddBarEditItemToLeftItemLinks, RibbonStatusBarItemLinkType.Left);
			InitializeItemLinksGroup(AddEditItemGroup, AddBarEditItemToRightItemLinks, RibbonStatusBarItemLinkType.Right);
			InitializeItemLinksGroup(AddBarItemGroup, AddBarItemToLeftItemLinks, RibbonStatusBarItemLinkType.Left);
			InitializeItemLinksGroup(AddBarItemGroup, AddBarItemToRightItemLinks, RibbonStatusBarItemLinkType.Right);			
			Items.Remove(AddBarItemGroup);
			AddBarItemToLeftItemLinks.Items.Add(AddBarEditItemToLeftItemLinks);
			AddBarItemToRightItemLinks.Items.Add(AddBarEditItemToRightItemLinks);
			Items.Add(AddBarItemToLeftItemLinks);
			Items.Add(AddBarItemToRightItemLinks);
		}
		protected override void InitializeAddBarItemLinkGroup(ModelItem barManager) {
			InitializeAddBarItemLinkCollection(AddBarItemLinkToLeft, barManager, RibbonStatusBarItemLinkType.Left);
			InitializeAddBarItemLinkCollection(AddBarItemLinkToRight, barManager, RibbonStatusBarItemLinkType.Right);
		}
		protected override ModelProperty GetParentItemLinksCollection(MenuAction menuAction, ModelItem container) {
			AddBarItemLinkMenuAction sender = menuAction as AddBarItemLinkMenuAction;
			if(sender == null || sender.Tag == null || !container.IsItemOfType(typeof(RibbonStatusBarControl)))
				return base.GetParentItemLinksCollection(menuAction, container);
			switch((RibbonStatusBarItemLinkType)sender.Tag) {
				case RibbonStatusBarItemLinkType.Left:
					return container.Properties["LeftItemLinks"];
				case RibbonStatusBarItemLinkType.Right:
					return container.Properties["RightItemLinks"];
				default: return base.GetParentItemLinksCollection(menuAction, container);
			}
		}
		void InitializeAddBarItemLinkCollection(MenuGroup collection, ModelItem barManager, RibbonStatusBarItemLinkType type) {
			collection.Items.Clear();
			if(barManager == null) return;
			foreach(ModelItem item in BarManagerDesignTimeHelper.GetBarManagerItems(barManager)) {
				if(SkipAddingToAddBarItemLink(item)) continue;
				AddBarItemLinkMenuAction menuAction = new AddBarItemLinkMenuAction(item) { Tag = type };
				menuAction.Execute += new EventHandler<MenuActionEventArgs>(OnAddBarItemLinkMenuActionExecute);
				collection.Items.Add(menuAction);
			}
			collection.HasDropDown = collection.Items.Count > 0;
		}
		protected override bool SkipAddingToAddBarItemLink(ModelItem barItem) {
			return base.SkipAddingToAddBarItemLink(barItem) || barItem.IsItemOfType(typeof(BarButtonGroup)) || barItem.IsItemOfType(typeof(RibbonGalleryBarItem));
		}
		void InitializeItemLinksGroup(MenuGroup source, MenuGroup target, RibbonStatusBarItemLinkType type) {
			foreach(AddBarItemMenuAction item in source.Items) {
				AddBarItemMenuAction menuItem = (AddBarItemMenuAction)item.Clone();
				menuItem.Tag = type;
				menuItem.Execute += OnAddBarItemExecute;
				target.Items.Add(menuItem);
			}
		}
	}
	enum RibbonStatusBarItemLinkType {
		Left,
		Right
	}
}
