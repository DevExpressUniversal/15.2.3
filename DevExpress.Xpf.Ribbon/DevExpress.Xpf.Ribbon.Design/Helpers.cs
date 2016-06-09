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
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Core.Native;
using Platform::DevExpress.Xpf.Ribbon;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Ribbon.Design {
	internal static class RibbonDesignTimeHelper {
		public static ModelItem GetModelItem(ModelItem root, object source) {
			return BarManagerDesignTimeHelper.GetModelItem(root, source);
		}
		public static ModelItem AddRibbonPageCategory(ModelItem ribbon) {
			ModelItem pageCategory = CreateRibbonPageCategory(ribbon.Context, !IsRibbonContainDefaultCategory(ribbon));
			if(!(ribbon.GetCurrentValue() is RibbonControl))
				throw new InvalidOperationException("RibbonPageCategory can be added only for RibbonControl!");
			AddItemForCollection(ribbon.Properties["Categories"], pageCategory, "Add a new RibbonPageCategory");
			ModelItem page = CreateRibbonPage(ribbon.Context);
			pageCategory.Properties["Pages"].Collection.Add(page);
			return pageCategory;
		}
		public static ModelItem AddRibbonPage(ModelItem pageCategory) {
			ModelItem page = CreateRibbonPage(pageCategory.Context);
			if(!(pageCategory.GetCurrentValue() is RibbonPageCategoryBase))
				throw new InvalidOperationException("RibbonPage can be added only for RibbonPageCategory!");
			AddItemForCollection(pageCategory.Properties["Pages"], page, "Add a new RibbonPage");
			return page;
		}
		public static ModelItem AddRibbonPageGroup(ModelItem page) {
			ModelItem pageGroup = CreateRibbonPageGroup(page.Context);
			if(!(page.GetCurrentValue() is RibbonPage))
				throw new InvalidOperationException("RibbonPageGroup can be added only for RibbonPage!");
			AddItemForCollection(page.Properties["Groups"], pageGroup, "Add a new RibbonPageGroup");
			return pageGroup;
		}
		public static ModelItem AddRibbonApplicationMenu(ModelItem ribbon) {
			ModelItem appMenu = ModelFactory.CreateItem(ribbon.Context, typeof(ApplicationMenu), CreateOptions.None);
			ribbon.Properties["ApplicationMenu"].SetValue(appMenu);
			return appMenu;
		}
		public static ModelItem CreateBarItemLink(ModelItem barItem) {
			return BarManagerDesignTimeHelper.CreateBarItemLink(barItem);
		}
		public static void CreateBarManager(ModelItem ribbon) {
			if(ribbon == null || !ribbon.IsItemOfType(typeof(RibbonControl))) return;
			using(ModelEditingScope scope = ribbon.BeginEdit("Creating a BarManager")) {
				ModelItem oldParent = ribbon.Parent;
				PropertyIdentifier dockProperty = new PropertyIdentifier(typeof(DockPanel), "Dock");
				ribbon.Properties[dockProperty].SetValue(Dock.Top);
				ModelItem ribbonStatusBar = ModelFactory.CreateItem(ribbon.Context, typeof(RibbonStatusBarControl), CreateOptions.InitializeDefaults);
				ribbonStatusBar.Properties[dockProperty].SetValue(Dock.Bottom);
				ModelItem grid = ModelFactory.CreateItem(ribbon.Context, typeof(Platform::System.Windows.Controls.Grid), CreateOptions.InitializeDefaults);
				grid.Properties["Background"].SetValue(Brushes.Transparent);
				ModelItem dockPanel = ModelFactory.CreateItem(ribbon.Context, typeof(DockPanel), CreateOptions.InitializeDefaults);
				int targetIndex = -1;
				if(oldParent.Content.IsCollection) {
					targetIndex = oldParent.Content.Collection.IndexOf(ribbon);
					oldParent.Content.Collection.Remove(ribbon);
				} else oldParent.Content.ClearValue();
				dockPanel.Properties["Children"].Collection.Add(ribbon);
				dockPanel.Properties["Children"].Collection.Add(ribbonStatusBar);
				dockPanel.Properties["Children"].Collection.Add(grid);
				ModelItem barManager = ModelFactory.CreateItem(ribbon.Context, typeof(BarManager), CreateOptions.InitializeDefaults);
				barManager.Properties["Bars"].Collection.Clear();
				barManager.Properties["CreateStandardLayout"].SetValue(false);
				if(oldParent.Content.IsCollection) {
					if(targetIndex >= 0)
						oldParent.Content.Collection.Insert(targetIndex, barManager);
					else oldParent.Content.Collection.Add(barManager);
				} else oldParent.Content.SetValue(barManager);
				barManager.Properties["Child"].SetValue(dockPanel);
				barManager.ResetLayout();
				dockPanel.ResetLayout();
				ribbon.ResetLayout();
				ribbonStatusBar.ResetLayout();
				grid.ResetLayout();
				scope.Complete();
			}
		}
		public static ModelItem CreateRibbonGalleryItem(EditingContext editingContext) {
			ModelItem ribbonGalleryItem = ModelFactory.CreateItem(editingContext, typeof(RibbonGalleryBarItem), CreateOptions.InitializeDefaults);
			ModelItem gallery = ModelFactory.CreateItem(editingContext, typeof(Gallery), CreateOptions.InitializeDefaults);
			ribbonGalleryItem.Properties["Gallery"].SetValue(gallery);
			gallery.ResetLayout();
			ribbonGalleryItem.ResetLayout();
			return ribbonGalleryItem;
		}
		public static bool IsRibbonContainDefaultCategory(ModelItem ribbonControl) {
			return GetDefaultPageCategory(ribbonControl) != null;
		}
		public static ModelItem FindParent<T>(ModelItem from) {
			return BarManagerDesignTimeHelper.FindParentByType<T>(from);
		}
		public static ModelItem FindBarManager(ModelItem from) {
			return BarManagerDesignTimeHelper.FindBarManagerInParent(from);
		}
		public static ModelItem FindRibbonCotnrol(ModelItem from) {
			return FindParent<RibbonControl>(from);
		}
		public static ModelItem FindRibbonPageCategory(ModelItem from) {
			return FindParent<RibbonPageCategoryBase>(from);
		}
		public static ModelItem FindRibbonPage(ModelItem from) {
			return FindParent<RibbonPage>(from);
		}
		public static ModelItem FindRibbonPageGroup(ModelItem from) {
			return FindParent<RibbonPageGroup>(from);
		}
		public static ModelItem GetBarItemFromLink(ModelItem barItemLink) {
			return BarManagerDesignTimeHelper.GetBarItemFromLink(barItemLink);
		}
		public static ModelItem GetDefaultPageCategory(ModelItem ribbon) {
			return ribbon.Properties[RibbonControl.CategoriesProperty.Name].Collection.FirstOrDefault(cat => cat.IsItemOfType(typeof(RibbonDefaultPageCategory)));
		}
		public static ModelItem GetRibbonByBarItemLink(ModelItem barItemLink) {
			return BarManagerDesignTimeHelper.GetModelItemFromBarItemLink<RibbonControl>(barItemLink);
		}
		public static ModelItem GetRibbonByCommonBarItem(ModelItem barItem) {
			return BarManagerDesignTimeHelper.GetModelItemFromCommonBarItem<RibbonControl>(barItem);
		}
		public static ModelItem GetRibbonStatusBarByBarItemLink(ModelItem barItemLink) {
			return BarManagerDesignTimeHelper.GetModelItemFromBarItemLink<RibbonStatusBarControl>(barItemLink);
		}
		public static ModelItem GetRibbonPageGroupByBarItemLink(ModelItem barItemLink) {
			RibbonPageGroupControl group = null;
			foreach(BarItemLinkControl control in BarManagerDesignTimeHelper.GetBarItemLinkControls(barItemLink)) {
				group = LayoutHelper.FindParentObject<RibbonPageGroupControl>(control);
				if(group != null) break;
			}	
			return group != null ? RibbonDesignTimeHelper.GetModelItem(barItemLink, group.PageGroup) : null;
		}		
		public static void RemoveRibbonPageCategory(ModelItem pageCategory) {
			ModelProperty propForChange = pageCategory.Parent.Properties["Categories"];
			using(var edit = pageCategory.BeginEdit("Remove the RibbonPageCategory")) {
				propForChange.Collection.Remove(pageCategory);
				edit.Complete();
			}
		}
		public static void RemoveRibbonPage(ModelItem ribbonPage) {
			ModelProperty propForChange = ribbonPage.Parent.Properties["Pages"];
			using(var edit = ribbonPage.BeginEdit("Remove the RibbonPage")) {
				propForChange.Collection.Remove(ribbonPage);
				edit.Complete();
			}
		}
		public static void RemoveRibbonPageGroup(ModelItem ribbonGroup) {
			ModelItem ribbonPage = ribbonGroup.Parent;
			ModelProperty propForChange = ribbonGroup.Parent.Properties["Groups"];
			using(var edit = ribbonPage.BeginEdit("Remove the RibbonPageGroup")) {
				propForChange.Collection.Remove(ribbonGroup);
				edit.Complete();
			}
		}
		public static void SetSelectedPage(ModelItem ribbon, ModelItem ribbonPage) {
			if(ribbon == null || ribbonPage == null || ribbonPage.Parent == null ||
				!ribbon.Properties["Categories"].Collection.Contains(ribbonPage.Parent)) return;
			((RibbonControl)ribbon.GetCurrentValue()).SelectedPage = (RibbonPage)ribbonPage.GetCurrentValue();
		}
		static void AddItemForCollection(ModelProperty property, ModelItem child, string msg) {
			if(property == null || child == null || !property.IsCollection) return;
			using(var scope = property.Parent.BeginEdit(msg)) {
				property.Collection.Add(child);
				child.ResetLayout();
				scope.Complete();
			}
		}
		static ModelItem CreateRibbonPageCategory(EditingContext context, bool isDefaultCategory) {
			Type categoryType = isDefaultCategory ? typeof(RibbonDefaultPageCategory) : typeof(RibbonPageCategory);
			ModelItem newCategory = ModelFactory.CreateItem(context, categoryType, CreateOptions.None);
			newCategory.Properties["Caption"].SetValue(isDefaultCategory ? "Default Category" : "Ribbon Category");
			newCategory.ResetLayout();
			return newCategory;
		}
		static ModelItem CreateRibbonPage(EditingContext context) {
			ModelItem page = ModelFactory.CreateItem(context, typeof(RibbonPage), CreateOptions.InitializeDefaults);
			page.ResetLayout();
			return page;
		}
		static ModelItem CreateRibbonPageGroup(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(RibbonPageGroup), CreateOptions.InitializeDefaults);
		}
	}
}
