#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Native;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Design.Forms;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.DashboardWin.Design {
	public class DashboardDesignTimeBarsGenerator : DesignTimeBarsGenerator<DashboardDesigner, DashboardCommandId> {
		const string CustomItemsText = "Custom items";
		const string OthersText = "Others";
		readonly Type componentType;
		readonly List<Type> obsoletePages = new List<Type>();
		readonly List<Type> obsoletePageGroups = new List<Type>();
		readonly List<Type> obsoleteBars = new List<Type>();
		readonly List<Type> obsoleteBarItems = new List<Type>();
		readonly List<Type> recreateBarItems = new List<Type>();
		public DashboardDesignTimeBarsGenerator(IDesignerHost host, IComponent component, Type componentType)
			: base(host, component) {
			this.componentType = componentType;
			obsoleteBarItems.Add(typeof(InsertGeoPointMapBarItem));
			recreateBarItems.Add(typeof(ConvertDashboardItemTypeBarItem));
			recreateBarItems.Add(typeof(AddCalculatedFieldBarItem));
			recreateBarItems.Add(typeof(DashboardParametersBarItem));
		}
		protected override BarGenerationManagerFactory<DashboardDesigner, DashboardCommandId> CreateBarGenerationManagerFactory() {
			return new DashboardBarGenerationManagerFactory();
		}
		protected override ControlCommandBarControllerBase<DashboardDesigner, DashboardCommandId> CreateBarController() {
			return new DashboardBarController();
		}
		protected override void EnsureReferences(IDesignerHost designerHost) {
		}
		protected override Component ChooseBarContainer(List<Component> supportedBarContainerCollection) {
			if(supportedBarContainerCollection.Count == 1)
				return supportedBarContainerCollection[0];
			List<Component> managers = new List<Component>();
			foreach(Component component in supportedBarContainerCollection)
				if(component.GetType() == componentType)
					managers.Add(component);
			if(managers.Count == 1)
				return managers[0];
			using(SelectBarManagerForm form = new SelectBarManagerForm()) {
				form.BarContainerCollection.AddRange(managers);
				form.ShowDialog();
				return form.SelectedContainer;
			}
		}
		internal void RemoveExistingBars(bool isRibbon, RibbonControl ribbon, BarManager barManager, DashboardBarController barController) {
			Type saveBarItemType = DashboardBarsUtils.DashboardSaveBarItemType;
			string saveBarItemNamespace = saveBarItemType.Namespace;
			foreach(Type type in Assembly.GetAssembly(saveBarItemType).GetTypes()) {
				if(type.IsClass && !type.IsAbstract && type.Namespace == saveBarItemNamespace && Attribute.GetCustomAttributes(type).FirstOrDefault(a => a is ObsoleteAttribute) != null) {
					if(typeof(ControlCommandBasedRibbonPage).IsAssignableFrom(type))
						obsoletePages.Add(type);
					else if(typeof(DashboardRibbonPageGroup).IsAssignableFrom(type))
						obsoletePageGroups.Add(type);
					else if(typeof(DashboardCommandBar).IsAssignableFrom(type))
						obsoleteBars.Add(type);
					else if(typeof(CommandBarCheckItem).IsAssignableFrom(type))
						obsoleteBarItems.Add(type);
					else if(typeof(DashboardBarButtonItem).IsAssignableFrom(type))
						obsoleteBarItems.Add(type);
				}
			}
			bool? saveCustomItems = null;
			Func<bool> func = () => {
				if(!saveCustomItems.HasValue)
					saveCustomItems = MessageBox.Show("Do you want to preserve your custom bar items?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification) == DialogResult.Yes;
				return saveCustomItems.Value;
			};
			if(isRibbon && ribbon != null)
				RemoveRibbonItems(ribbon, barController, saveBarItemNamespace, func);
			else if(barManager != null)
				RemoveBarItems(barManager, barController, saveBarItemNamespace, func);
		}
		void RecreateBarItems(BarItems items, BarItemsContainer container, string dashboardRibbonNamespace, IDesignerHost designerHost) {
			for(int i = items.Count - 1; i >= 0; i--)
				if(recreateBarItems.Contains(items[i].GetType())) {
					BarSubItem subItem = items[i] as BarSubItem;
					if(subItem != null) {
						for(int j = subItem.ItemLinks.Count - 1; j >= 0; j--) {
							BarItem innerItem = subItem.ItemLinks[j].Item;
							if(innerItem != null && innerItem.GetType().Namespace != dashboardRibbonNamespace) {
								container.BarItems.Add(innerItem);
								innerItem.Links.Clear();
								subItem.Manager.Items.Remove(innerItem);
							}
						}
					}
					designerHost.DestroyComponent(items[i]);
				}
		}
		void RemoveRibbonItems(RibbonControl ribbon, DashboardBarController barController, string dashboardRibbonNamespace, Func<bool> saveCustomItems) {
			IDesignerHost designerHost = ribbon.Site != null ? (IDesignerHost)ribbon.Site.GetService(typeof(IDesignerHost)) : null;
			if(designerHost == null)
				return;
			RibbonItemsContainer container = new RibbonItemsContainer();
			PrepareRibbonPages(ribbon, ribbon.Pages, designerHost, container, barController, dashboardRibbonNamespace, saveCustomItems);
			foreach(RibbonPageCategory category in ribbon.PageCategories)
				PrepareRibbonPages(ribbon, category.Pages, designerHost, container, barController, dashboardRibbonNamespace, saveCustomItems);
			RecreateBarItems(ribbon.Items, container, dashboardRibbonNamespace, designerHost);
			if(container.Groups.Count > 0 || container.BarItems.Count > 0) {
				RibbonPage page = (RibbonPage)designerHost.CreateComponent(typeof(RibbonPage));
				page.Text = OthersText;
				foreach(RibbonPageGroup group in container.Groups)
					page.Groups.Add(group);
				if(container.BarItems.Count > 0) {
					RibbonPageGroup group = (RibbonPageGroup)designerHost.CreateComponent(typeof(RibbonPageGroup));
					group.Text = CustomItemsText;
					foreach(BarItem item in container.BarItems) {
						group.ItemLinks.Add(item);
						ribbon.Items.Add(item);
						barController.BarItems.Add(item);
					}
					page.Groups.Add(group);
				}
				ribbon.Pages.Add(page);
			}
		}
		void RemoveBarItems(BarManager barManager, DashboardBarController barController, string dashboardRibbonNamespace, Func<bool> saveCustomItems) {
			IDesignerHost designerHost = barManager.Site != null ? (IDesignerHost)barManager.Site.GetService(typeof(IDesignerHost)) : null;
			if(designerHost == null)
				return;
			BarItemsContainer container = new BarItemsContainer();
			PrepareBars(barManager, barController, designerHost, container, dashboardRibbonNamespace, saveCustomItems);
			RecreateBarItems(barManager.Items, container, dashboardRibbonNamespace, designerHost);
			if(container.BarItems.Count > 0) {
				Bar bar = (Bar)designerHost.CreateComponent(typeof(Bar));
				bar.Text = CustomItemsText;
				bar.DockStyle = BarDockStyle.Top;
				foreach(BarItem item in container.BarItems) {
					bar.ItemLinks.Add(item);
					barController.BarItems.Add(item);
				}
				barManager.Bars.Add(bar);
			}
		}
		void PrepareRibbonPages(RibbonControl ribbon, RibbonPageCollection pages, IDesignerHost designerHost, RibbonItemsContainer container, DashboardBarController barController, string dashboardRibbonNamespace, Func<bool> saveCustomItems) {
			for(int i = pages.Count - 1; i >= 0; i--) {
				RibbonPage page = pages[i];
				Type pageType = page.GetType();
				if(pageType.Namespace != dashboardRibbonNamespace)
					continue;
				bool isObsoletePage = obsoletePages.Contains(pageType);
				for(int j = page.Groups.Count - 1; j >= 0; j--) {
					RibbonPageGroup group = page.Groups[j];
					Type groupType = group.GetType();
					if(groupType.Namespace != dashboardRibbonNamespace) {
						if(isObsoletePage) {
							if(saveCustomItems())
								container.Groups.Add(group);
							page.Groups.Remove(group);
						}
						continue;
					}
					bool isObsoletePageGroup = obsoletePageGroups.Contains(groupType);
					for(int k = group.ItemLinks.Count - 1; k >= 0; k--) {
						BarItem item = group.ItemLinks[k].Item;
						Type barItemType = item.GetType();
						if(barItemType.Namespace != dashboardRibbonNamespace) {
							if(isObsoletePage || isObsoletePageGroup) {
								if(saveCustomItems())
									container.BarItems.Add(item);
								group.ItemLinks.Remove(item);
								ribbon.Items.Remove(item);
								barController.BarItems.Remove(item);
							}
							continue;
						}
						if(isObsoletePage || isObsoletePageGroup || obsoleteBarItems.Contains(barItemType)) {
							designerHost.DestroyComponent(item);
						}
					}
					if(isObsoletePageGroup)
						designerHost.DestroyComponent(group);
				}
				if(isObsoletePage)
					designerHost.DestroyComponent(page);
			}
		}
		void PrepareBars(BarManager barManager, DashboardBarController barController, IDesignerHost designerHost, BarItemsContainer container, string dashboardRibbonNamespace, Func<bool> saveCustomItems) {
			for(int i = barManager.Bars.Count - 1; i >= 0; i--) {
				Bar bar = barManager.Bars[i];
				Type barType = bar.GetType();
				if(barType.Namespace != dashboardRibbonNamespace)
					continue;
				bool isBarObsolete = obsoleteBars.Contains(barType);
				for(int j = bar.ItemLinks.Count - 1; j >= 0; j--) {
					BarItemLink link = bar.ItemLinks[j];
					BarItem barItem = link.Item;
					if(link != null && barItem != null) {
						Type barItemType = barItem.GetType();
						if(barItemType.Namespace != dashboardRibbonNamespace) {
							if(isBarObsolete) {
								if(saveCustomItems())
									container.BarItems.Add(barItem);
								barManager.Items.Remove(barItem);
								bar.ItemLinks.Remove(link);
								barController.BarItems.Remove(barItem);
								continue;
							}
						}
						if(isBarObsolete || obsoleteBarItems.Contains(barItemType))
							designerHost.DestroyComponent(barItem);
					}
				}
				if(isBarObsolete)
					designerHost.DestroyComponent(bar);
			}
		}
	}
}
