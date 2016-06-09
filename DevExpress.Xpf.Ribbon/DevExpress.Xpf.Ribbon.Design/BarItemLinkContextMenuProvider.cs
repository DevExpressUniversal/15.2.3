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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Ribbon;
using Platform::DevExpress.Xpf.Core.Native;
using System.Windows;
using Microsoft.Windows.Design.Services;
using DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.Ribbon.Design {
	class BarItemContextMenuProvider : PrimarySelectionContextMenuProvider {
		MenuAction AddToToolbar { get; set; }
		MenuAction AddToPageHeader { get; set; }
		MenuAction AddToApplicationMenu { get; set; }
		public BarItemContextMenuProvider() {
			AddToToolbar = new MenuAction("Show in Quick Access Toolbar");
			AddToToolbar.Checkable = true;
			AddToToolbar.Execute += OnAddToRibbonExecute;
			AddToPageHeader = new MenuAction("Show in Page Header");
			AddToPageHeader.Checkable = true;
			AddToPageHeader.Execute += OnAddToRibbonExecute;
			AddToApplicationMenu = new MenuAction("Show in Application Menu");
			AddToApplicationMenu.Checkable = true;
			AddToApplicationMenu.Execute += OnAddToRibbonExecute;
			Items.Add(AddToToolbar);
			Items.Add(AddToPageHeader);
			Items.Add(AddToApplicationMenu);
			UpdateItemStatus += OnUpdateItemStatus;
		}
		void OnAddToRibbonExecute(object sender, MenuActionEventArgs e) {
			ModelItem barItem = e.Selection.PrimarySelection;
			ModelItem ribbonControl = null;
			foreach(ModelItem link in BarManagerDesignTimeHelper.GetBarItemLinks(barItem)) {
				ribbonControl = RibbonDesignTimeHelper.GetRibbonByBarItemLink(link);
				if(ribbonControl != null) break;
			}
			if(ribbonControl == null) {
				ribbonControl = RibbonDesignTimeHelper.GetRibbonByCommonBarItem(barItem);
			}
			var menuItem = sender as MenuAction;
			if(ribbonControl == null || menuItem == null) return;
			ModelProperty targetProperty1;
			ModelProperty targetProperty2;
			if(menuItem.Equals(AddToToolbar)) {
				targetProperty1 = ribbonControl.Properties["ToolbarItems"];
				targetProperty2 = ribbonControl.Properties["ToolbarItemLinks"];
			} else if(menuItem.Equals(AddToPageHeader)) {
				targetProperty1 = ribbonControl.Properties["PageHeaderItems"];
				targetProperty2 = ribbonControl.Properties["PageHeaderItemLinks"];
			} else if(menuItem.Equals(AddToApplicationMenu)) {
				ModelItem appMenu = ribbonControl.Properties["ApplicationMenu"].IsSet ?
					ribbonControl.Properties["ApplicationMenu"].Value : CreateApplicationMenu(ribbonControl);
				targetProperty1 = appMenu.Properties["Items"];
				targetProperty2 = appMenu.Properties["ItemLinks"];
			} else
				return;
			using(ModelEditingScope scope = ribbonControl.BeginEdit(menuItem.DisplayName)) {
				ModelItem barItemLink = GetBarItemLink(barItem, targetProperty1);
				if(barItemLink == null)
					barItemLink = GetBarItemLink(barItem, targetProperty2);
				if(barItemLink == null) {
					barItemLink = RibbonDesignTimeHelper.CreateBarItemLink(barItem);
					BarManagerDesignTimeHelper.AddBarItemLink(targetProperty1, barItemLink);
				} else
					BarManagerDesignTimeHelper.RemoveBarItemLink(barItemLink);
				scope.Complete();
			}
		}
		ModelItem CreateApplicationMenu(ModelItem ribbonControl) {
			ModelItem appMenu = null;
			using(ModelEditingScope scope = ribbonControl.BeginEdit("Create ApplicationMenu")) {
				appMenu = RibbonDesignTimeHelper.AddRibbonApplicationMenu(ribbonControl);
				scope.Complete();
			}
			return appMenu;
		}
		ModelItem GetBarItemLink(ModelItem barItem, ModelProperty property) {
			if(!property.IsCollection) return null;
			foreach(ModelItem link in property.Collection) {
				if(link.Properties["BarItemName"].IsSet && link.Properties["BarItemName"].ComputedValue.ToString() == barItem.Name)
					return link;
			}
			return null;
		}
		bool ContainsBarItem(ModelItem barItem, ModelProperty property) {
			if(property == null || barItem == null)
				return false;
			return property.Collection.Contains(barItem);
		}
		void OnUpdateItemStatus(object sender, MenuActionEventArgs e) {
			ModelItem barItem = e.Selection.PrimarySelection;
			ModelItem ribbonControl = null;
			foreach(ModelItem link in BarManagerDesignTimeHelper.GetBarItemLinks(barItem)) {
				ribbonControl = RibbonDesignTimeHelper.GetRibbonByBarItemLink(link);
				if(ribbonControl != null) break;
			}
			if(ribbonControl == null) {
				ribbonControl = RibbonDesignTimeHelper.GetRibbonByCommonBarItem(barItem);
			}
			if(ribbonControl == null || !IsValidType(barItem)) {
				SetVisibleForMenuItems(false);
				return;
			}
			SetVisibleForMenuItems(true);
			ModelProperty appMenuProperty = ribbonControl.Properties["ApplicationMenu"];
			if(appMenuProperty != null)
				AddToApplicationMenu.Visible = appMenuProperty.IsSet && appMenuProperty.Value.IsItemOfType(typeof(ApplicationMenu));
			AddToToolbar.Visible = !ContainsBarItem(barItem, ribbonControl.Properties["ToolbarItems"]);
			UpdateCheckedState(barItem, ribbonControl);
		}
		void UpdateCheckedState(ModelItem barItem, ModelItem ribbonControl) {
			UpdateShowInToolbarCheckedState(barItem, ribbonControl);
			UpdateShowInPageHeaderCheckedState(barItem, ribbonControl);
			if (AddToApplicationMenu.Visible)
				UpdateShowInAppMenuCheckedState(barItem, ribbonControl);
		}
		void UpdateShowInAppMenuCheckedState(ModelItem barItem, ModelItem ribbonControl) {
			var appMenuProperty = ribbonControl.Properties["ApplicationMenu"];
			if(!appMenuProperty.IsSet)
				return;
			ModelProperty property = appMenuProperty.Value.Properties["ItemLinks"];
			AddToApplicationMenu.Checked = ContainsLink(barItem, property);
		}
		void UpdateShowInPageHeaderCheckedState(ModelItem barItem, ModelItem ribbonControl) {
			AddToPageHeader.Checked = ContainsLink(barItem, ribbonControl.Properties["PageHeaderItemLinks"]) || ContainsLink(barItem, ribbonControl.Properties["PageHeaderItems"]);
		}
		void UpdateShowInToolbarCheckedState(ModelItem barItem, ModelItem ribbonControl) {
			AddToToolbar.Checked = ContainsLink(barItem, ribbonControl.Properties["ToolbarItemLinks"]) || ContainsLink(barItem, ribbonControl.Properties["ToolbarItems"]);
		}
		bool ContainsLink(ModelItem barItem, ModelProperty property) {
			if(property == null)
				return false;
			return GetBarItemLink(barItem, property) != null;
		}
		bool IsValidType(ModelItem barItem) {
			return !barItem.IsItemOfType(typeof(BarEditItem)) && !barItem.IsItemOfType(typeof(BarButtonGroup)) && !barItem.IsItemOfType(typeof(RibbonGalleryBarItem));
		}
		void SetVisibleForMenuItems(bool isVisible) {
			foreach(MenuAction item in Items) {
				item.Visible = isVisible;
			}
		}
	}
}
