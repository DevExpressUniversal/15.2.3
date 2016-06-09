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

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Ribbon.BackstageView;
namespace DevExpress.XtraBars {
	public class RibbonPageGroupDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			RibbonPageGroup group = (RibbonPageGroup)component;
			return group.GroupInfo == null ? Rectangle.Empty : group.GroupInfo.Bounds;
		}
		public Control GetOwnerControl(IComponent component) {
			RibbonPageGroup group = (RibbonPageGroup)component;
			return group.Ribbon;
		}
	}
	public class RibbonPageDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			RibbonPage page = (RibbonPage)component;
			return page.PageInfo == null ? Rectangle.Empty : page.PageInfo.Bounds;
		}
		public Control GetOwnerControl(IComponent component) {
			RibbonPage page = (RibbonPage)component;
			return page.Ribbon;
		}
	}
	public class RibbonPageCategoryDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			RibbonPageCategory category = (RibbonPageCategory)component;
			return category.CategoryInfo == null ? Rectangle.Empty : category.CategoryInfo.Bounds;
		}
		public Control GetOwnerControl(IComponent component) {
			RibbonPageCategory category = (RibbonPageCategory)component;
			return category.Ribbon;
		}
	}
	public class BarItemDesignTimeBoundsProvider : ISmartTagClientBoundsProviderEx {
		public Rectangle GetBounds(IComponent component) {
			RibbonStatusBar statusBar;
			BarItemLink link = GetBarItemLink(component);
			if(link == null || link.LinkViewInfo == null)
				return Rectangle.Empty;
			Rectangle bounds = link.Bounds;
			if(IsStatusBarOwner(link, out statusBar)) {
				bounds = statusBar.RectangleToClient(link.ScreenBounds);
			}
			return bounds;
		}
		public Control GetOwnerControl(IComponent component) {
			RibbonStatusBar statusBar;
			BarItemLink link = GetBarItemLink(component);
			if(link == null)
				return null;
			if(link.BarControl != null)
				return link.BarControl;
			if(IsStatusBarOwner(link, out statusBar)) {
				return statusBar;
			}
			return link.Ribbon;
		}
		protected virtual bool IsStatusBarOwner(BarItemLink link, out RibbonStatusBar statusBar) {
			statusBar = null;
			RibbonControl ribbon = link.Ribbon;
			if(ribbon != null && ribbon.StatusBar != null && link.Holder is RibbonStatusBarItemLinkCollection) {
				statusBar = ribbon.StatusBar;
				return true;
			}
			return false;
		}
		protected virtual BarItemLink GetBarItemLink(object obj) {
			BarItem item = (BarItem)obj;
			if(item.LinkProvider != null) {
				return item.LinkProvider.Link;
			}
			if(item.Links.Count > 0) {
				return item.Links[0];
			}
			return null;
		}
		public ISmartTagGlyphObserver GetObserver(IComponent component, out Control relatedControl) {
			relatedControl = null;
			Control owner = GetOwnerControl(component);
			if(owner == null) return null;
			if(owner is DockedBarControl) {
				BarDockControl bdc = ((DockedBarControl)owner).Parent as BarDockControl;
				if(bdc != null || bdc.Site == null) {
					IDesignerHost host = bdc.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
					if(host != null) {
						relatedControl = bdc;
						return host.GetDesigner(bdc) as ISmartTagGlyphObserver;
					}
				}
			}
			return null;
		}
	}
	public class RibbonPageDesignTimeActionsProvider {
		public void AddPageGroup(object component) {
			RibbonDesignTimeManager manager = GetDesignTimeManager(component);
			if(manager == null)
				return;
			manager.OnAddPageGroup(component, manager.Ribbon.DefaultRegistrationInfo);
		}
		public void AddPage(object component) {
			RibbonDesignTimeManager manager = GetDesignTimeManager(component);
			if(manager != null) manager.OnAddPage(component, manager.Ribbon.DefaultRegistrationInfo);
		}
		public void AddPageCategory(object component) {
			RibbonDesignTimeManager manager = GetDesignTimeManager(component);
			if(manager != null) manager.OnAddPageCategory(component, manager.Ribbon.DefaultRegistrationInfo);
		}
		protected RibbonDesignTimeManager GetDesignTimeManager(object component) {
			RibbonPage page = (RibbonPage)component;
			if(page.Ribbon == null)
				return null;
			return (RibbonDesignTimeManager)page.Ribbon.GetDesignTimeManager();
		}
	}
	public class RibbonPageCategoryDesignTimeActionsProvider {
		public void AddPage(object component) {
			RibbonPageCategory category = (RibbonPageCategory)component;
			RibbonDesignTimeManager manager = GetRibbonDesignTimeManager(component);
			if(manager == null)
				return;
			RibbonPage page = manager.CreateRibbonPage(manager.Ribbon.DefaultRegistrationInfo);
			if(page == null) return;
			category.Pages.Add(page);
			category.Ribbon.SelectedPage = page;
			manager.SelectComponentDelayed(page);
			manager.FireChanged(category.Ribbon);
		}
		public void AddPageCategory(object component) {
			RibbonDesignTimeManager manager = GetRibbonDesignTimeManager(component);
			if(manager != null) manager.OnAddPageCategory(component, manager.Ribbon.DefaultRegistrationInfo);
		}
		protected RibbonDesignTimeManager GetRibbonDesignTimeManager(object component) {
			RibbonPageCategory category = (RibbonPageCategory)component;
			if(category.Ribbon == null)
				return null;
			return (RibbonDesignTimeManager)category.Ribbon.GetDesignTimeManager();
		}
	}
	public class BackstageViewTabItemDesignTimeActionsProvider : BackstageViewItemDesignTimeActionsProvider {
		public void AddChildBackstageView(IComponent component) {
			BackstageViewDesignTimeManager manager = GetDesignTimeManager(component);
			if(manager != null) manager.AddChildBackstageView((BackstageViewTabItem)component);
		}
		public void AddRecentItemControl(IComponent component) {
			BackstageViewDesignTimeManager manager = GetDesignTimeManager(component);
			if(manager != null) manager.AddRecentItemControl((BackstageViewTabItem)component);
		}
	}
	public class BackstageViewTabItemDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			BackstageViewTabItem tabItem = (BackstageViewTabItem)component;
			BackstageViewControl backstageView = (BackstageViewControl)GetOwnerControl(component);
			if(backstageView == null)
				return Rectangle.Empty;
			BackstageViewItemBaseViewInfo itemViewInfo = backstageView.ViewInfo.GetItemInfo(tabItem);
			return itemViewInfo.Bounds;
		}
		public Control GetOwnerControl(IComponent component) {
			BackstageViewTabItem tabItem = (BackstageViewTabItem)component;
			return tabItem.Control;
		}
	}
	public class BackstageViewButtonItemDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			BackstageViewButtonItem buttonItem = (BackstageViewButtonItem)component;
			BackstageViewControl backstageView = (BackstageViewControl)GetOwnerControl(component);
			if(backstageView == null)
				return Rectangle.Empty;
			BackstageViewItemBaseViewInfo itemViewInfo = backstageView.ViewInfo.GetItemInfo(buttonItem);
			return itemViewInfo.Bounds;
		}
		public Control GetOwnerControl(IComponent component) {
			BackstageViewButtonItem buttonItem = (BackstageViewButtonItem)component;
			return buttonItem.Control;
		}
	}
	public class RecentControlPanelDesignTimeActionsProvider {
		public void AddLabelItem(IComponent component) {
			RecentControlDesignTimeManager manager = GetDesignTimeManager(component);
			RecentPanelBase panel = (RecentPanelBase)component;
			if(manager != null) manager.OnAddLabelItem(panel);
		}
		public void AddButtonItem(IComponent component) {
			RecentControlDesignTimeManager manager = GetDesignTimeManager(component);
			RecentPanelBase panel = (RecentPanelBase)component;
			if(manager != null) manager.OnAddButtonItem(panel);
		}
		public void AddSeparatorItem(IComponent component) {
			RecentControlDesignTimeManager manager = GetDesignTimeManager(component);
			RecentPanelBase panel = (RecentPanelBase)component;
			if(manager != null) manager.OnAddSeparatorItem(panel);
		}
		public void AddRecentItem(IComponent component) {
			RecentControlDesignTimeManager manager = GetDesignTimeManager(component);
			RecentPanelBase panel = (RecentPanelBase)component;
			if(manager != null) manager.OnAddRecentItem(panel);
		}
		public void AddTabItem(IComponent component) {
			RecentControlDesignTimeManager manager = GetDesignTimeManager(component);
			RecentPanelBase panel = (RecentPanelBase)component;
			if(manager != null) manager.OnAddTabItem(panel);
		}
		public void AddContainerItem(IComponent component) {
			RecentControlDesignTimeManager manager = GetDesignTimeManager(component);
			RecentPanelBase panel = (RecentPanelBase)component;
			if(manager != null) manager.OnAddContainerItem(panel);
		}
		public void AddHyperLinkItem(IComponent component) {
			RecentControlDesignTimeManager manager = GetDesignTimeManager(component);
			RecentPanelBase panel = (RecentPanelBase)component;
			if(manager != null) manager.OnAddHyperLinkItem(panel);
		}
		protected RecentControlDesignTimeManager GetDesignTimeManager(IComponent component) {
			RecentPanelBase panel = (RecentPanelBase)component;
			if(panel.RecentControl == null)
				return null;
			return panel.RecentControl.GetDesignTimeManager();
		}
	}
	public class RecentControlPanelDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			RecentPanelBase recentPanel = (RecentPanelBase)component;
			RecentItemControl recentControl = (RecentItemControl)GetOwnerControl(component);
			if(recentControl == null)
				return Rectangle.Empty;
			RecentPanelViewInfoBase viewInfo = recentPanel.ViewInfo;
			return viewInfo.Bounds;
		}
		public Control GetOwnerControl(IComponent component) {
			RecentPanelBase panel = (RecentPanelBase)component;
			return panel.RecentControl;
		}
	}
	public class RecentControlTextGlyphItemDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			RecentTextGlyphItemBase item = (RecentTextGlyphItemBase)component;
			RecentItemControl recentControl = (RecentItemControl)GetOwnerControl(component);
			if(recentControl == null)
				return Rectangle.Empty;
			RecentItemViewInfoBase viewInfo = item.ViewInfo;
			return viewInfo.Bounds;
		}
		public Control GetOwnerControl(IComponent component) {
			RecentTextGlyphItemBase item = (RecentTextGlyphItemBase)component;
			return item.RecentControl;
		}
	}
	public class GalleryItemGroupDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			GalleryObjectDescriptor desc = (GalleryObjectDescriptor)component;
			Rectangle rect = Rectangle.Empty;
			if(desc == null)
				return rect;
			if(desc.GalleryLink != null) {
				if(desc.GalleryLink.ViewInfo != null && desc.GalleryLink.ViewInfo.GalleryInfo != null) {
					if(desc.GalleryGroup != null)
						rect = desc.GalleryLink.ViewInfo.GalleryInfo.GetGroupInfo(desc.GalleryGroup).Bounds;
					if(desc.GalleryItem != null)
						rect = desc.GalleryLink.ViewInfo.GalleryInfo.GetItemInfo(desc.GalleryItem).Bounds;
				}
				return rect;
			}
			if(desc.StandaloneGallery != null) {
				if(desc.GalleryGroup != null)
					rect = desc.StandaloneGallery.GetGroupInfo(desc.GalleryGroup).Bounds;
				if(desc.GalleryItem != null)
					rect = desc.StandaloneGallery.GetItemInfo(desc.GalleryItem).Bounds;
				return rect;
			}
			return Rectangle.Empty;
		}
		public Control GetOwnerControl(IComponent component) {
			GalleryObjectDescriptor desc = (GalleryObjectDescriptor)component;
			if(desc.GalleryLink != null)
				return desc.GalleryLink.Ribbon;
			if(desc.StandaloneGallery != null)
				return desc.StandaloneGallery.OwnerControl;
			return null;
		}
	}
	public abstract class GalleryDesignTimeActionsProviderBase {
		protected RibbonDesignTimeManager GetDesignTimeManager(IComponent component) {
			RibbonDesignTimeManager designTimeManger = null;
			GalleryObjectDescriptor desc = (GalleryObjectDescriptor)component;
			if(desc.StandaloneGallery != null && desc.StandaloneGallery.OwnerControl != null) {
				GalleryControl gallery = desc.StandaloneGallery.OwnerControl as GalleryControl;
				if(gallery != null) designTimeManger = gallery.DesignTimeManager;
			}
			if(desc.Gallery is InRibbonGallery) {
				designTimeManger = ((InRibbonGallery)desc.Gallery).GetDesignTimeManager() as RibbonDesignTimeManager;
			}
			return designTimeManger;
		}
		protected GalleryControlGallery GetGalleryControlGallery(IComponent component) {
			GalleryObjectDescriptor desc = (GalleryObjectDescriptor)component;
			return desc.StandaloneGallery as GalleryControlGallery;
		}
		protected GalleryItemGroup GetGroup(IComponent component) {
			IDXObjectWrapper obj = (IDXObjectWrapper)component;
			GalleryItemGroup group = obj.SourceObject as GalleryItemGroup;
			if(group != null)
				return group;
			GalleryItem item = obj.SourceObject as GalleryItem;
			if(item == null) return null;
			return item.GalleryGroup;
		}
		protected RibbonGalleryBarItem GetRibbonGalleryBarItem(IComponent component) {
			GalleryObjectDescriptor desc = (GalleryObjectDescriptor)component;
			return desc.GalleryLink == null ? null : desc.GalleryLink.Item;
		}
		protected void DoSelectOwnerComponent(RibbonDesignTimeManager designTimeManager, IComponent component) {
			IChildVisualElement child = component as IChildVisualElement;
			if(child != null && child.Owner != null) {
				designTimeManager.SelectComponent(child.Owner);
			}
		}
	}
	public class GalleryItemDesignTimeActionsProvider : GalleryDesignTimeActionsProviderBase {
		public void AddGroup(IComponent component) {
			new GalleryItemGroupDesignTimeActionsProvider().AddGroup(component);
		}
		public void AddItem(IComponent component) {
			new GalleryItemGroupDesignTimeActionsProvider().AddItem(component);
		}
		public void RemoveItem(IComponent component) {
			RibbonDesignTimeManager manager = GetDesignTimeManager(component);
			if(manager == null)
				return;
			GalleryItem item = GetItem(component);
			if(item == null)
				return;
			GalleryItemGroup group = GetGroup(component);
			if(group == null)
				return;
			manager.RemoveGalleryItemCore(GetRibbonGalleryBarItem(component), null, GetGalleryControlGallery(component), group, item);
			DoSelectOwnerComponent(manager, component);
		}
		protected GalleryItem GetItem(IComponent component) {
			IDXObjectWrapper obj = (IDXObjectWrapper)component;
			return obj.SourceObject as GalleryItem;
		}
	}
	public class GalleryItemGroupDesignTimeActionsProvider : GalleryDesignTimeActionsProviderBase {
		public void AddGroup(IComponent component) {
			RibbonDesignTimeManager manager = GetDesignTimeManager(component);
			if(manager == null)
				return;
			manager.OnAddGalleryGroupCore(GetRibbonGalleryBarItem(component), null, GetGalleryControlGallery(component));
		}
		public void AddItem(IComponent component) {
			GalleryItemGroup group = GetGroup(component);
			if(group == null) return;
			RibbonDesignTimeManager manager = GetDesignTimeManager(component);
			if(manager != null) manager.OnAddGalleryItemCore(group);
		}
		public void RemoveGroup(IComponent component) {
			RibbonDesignTimeManager manager = GetDesignTimeManager(component);
			if(manager == null)
				return;
			GalleryItemGroup group = GetGroup(component);
			if(group == null)
				return;
			manager.RemoveGalleryGroupCore(GetRibbonGalleryBarItem(component), null, GetGalleryControlGallery(component), group);
			DoSelectOwnerComponent(manager, component);
		}
	}
	public class BackstageViewItemDesignTimeActionsProvider {
		public void AddCommand(IComponent component) {
			BackstageViewDesignTimeManager manager = GetDesignTimeManager(component);
			if(manager != null) manager.OnAddCommand();
		}
		public void AddTab(IComponent component) {
			BackstageViewDesignTimeManager manager = GetDesignTimeManager(component);
			if(manager != null) manager.OnAddTab();
		}
		public void AddSeparator(IComponent component) {
			BackstageViewDesignTimeManager manager = GetDesignTimeManager(component);
			if(manager != null) manager.OnAddSeparator();
		}
		protected BackstageViewDesignTimeManager GetDesignTimeManager(IComponent component) {
			BackstageViewItem item = (BackstageViewItem)component;
			if(item.Control == null)
				return null;
			return item.Control.GetDesignTimeManager();
		}
	}
	public class DockPanelDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			DevExpress.XtraBars.Docking.DockPanel panel = component as DevExpress.XtraBars.Docking.DockPanel;
			if(panel != null)
				return new Rectangle(new Point(0, 0), panel.Bounds.Size);
			return Rectangle.Empty;
		}
		public Control GetOwnerControl(IComponent component) {
			DevExpress.XtraBars.Docking.DockPanel panel = component as DevExpress.XtraBars.Docking.DockPanel;
			return panel;
		}
	}
	public class DockPanelDesignTimeActionsProvider {
		void Dock(IComponent component, Docking.DockingStyle style) {
			Docking.DockPanel panel = component as Docking.DockPanel;
			if(panel == null) return;
			if(panel.DockedAsTabbedDocument) Float(panel);				
			panel.DockTo(style);
		}
		public void DockToLeft(IComponent component) { Dock(component, Docking.DockingStyle.Left); }
		public void DockToRight(IComponent component) { Dock(component, Docking.DockingStyle.Right); }
		public void DockToTop(IComponent component) { Dock(component, Docking.DockingStyle.Top); }
		public void DockToBottom(IComponent component) { Dock(component, Docking.DockingStyle.Bottom); }		
		public void Float(IComponent component) {
			Docking.DockPanel panel = component as Docking.DockPanel;
			if(panel == null) return;
			if(panel.DockedAsTabbedDocument) panel.DockedAsTabbedDocument = false;
			else panel.DockTo(Docking.DockingStyle.Float);
		}
		public void DockedAsTabbedDocument(IComponent component) {
			Docking.DockPanel panel = component as Docking.DockPanel;
			if(panel == null) return;
			panel.DockedAsTabbedDocument = true;
		}
		public void AddCustomHeaderButtons(IComponent component) {
			Docking.DockPanel panel = component as Docking.DockPanel;
			if(panel == null) return;
			System.ComponentModel.Design.IDesigner designer = EditorContextHelper.GetDesigner(panel) as System.ComponentModel.Design.IDesigner;
			EditorContextHelper.EditValue(designer, panel, "CustomHeaderButtons");
		}		
	}
	public class TileBoundsProvider : DevExpress.Utils.Design.ISmartTagClientBoundsProvider {
		#region ISmartTagClientBoundsProvider Members
		public Rectangle GetBounds(IComponent component) {
			ITileItem item = component as ITileItem;
			if(item.Control == null) return Rectangle.Empty;
			Point point = item.Control.PointToClient(System.Windows.Forms.Cursor.Position);
			TileControlHitInfo info = item.Control.CalcHitInfo(point);
			if(info == null || info.ItemInfo == null) return Rectangle.Empty;
			Rectangle bounds = info.ItemInfo.Bounds;
			return new Rectangle(item.Control.PointToScreen(bounds.Location), bounds.Size);
		}
		public System.Windows.Forms.Control GetOwnerControl(IComponent component) {
			return null;
		}
		#endregion
	}
	public class TileActionsProvider : DevExpress.Utils.ComponentActions {
		void ShowCollectionDesigner(IComponent component, string propertyName) {
			System.ComponentModel.Design.IDesigner designer = GetDesigner(component);
			if(designer != null)
				EditorContextHelper.EditValue(designer, component, propertyName);
		}
		public void Elements(IComponent component) {
			ShowCollectionDesigner(component, "Elements");
		}
		public void Frames(IComponent component) {
			ShowCollectionDesigner(component, "Frames");
		}
		public void BackgroundImage(IComponent component) {
			ShowCollectionDesigner(component, "BackgroundImage");
		}
	}
	public class TileContainerBoundsProvider : ISmartTagClientBoundsProvider {
		#region ISmartTagClientBoundsProvider Members
		public Rectangle GetBounds(IComponent component) {
			DevExpress.XtraBars.Docking2010.Views.WindowsUI.TileContainer item = component as DevExpress.XtraBars.Docking2010.Views.WindowsUI.TileContainer;
			Point point = item.Info.TileControl.PointToClient(System.Windows.Forms.Cursor.Position);
			TileControlHitInfo info = item.Info.TileControl.CalcHitInfo(point);
			if(info == null || info.ViewInfo == null) return Rectangle.Empty;
			Rectangle bounds = item.Info.TileControl.CalcHitInfo(point).ViewInfo.Bounds;
			return new Rectangle(item.Info.TileControl.PointToScreen(bounds.Location), bounds.Size);
		}
		public System.Windows.Forms.Control GetOwnerControl(IComponent component) {
			return null;
		}
		#endregion
	}
	public class TileContainerActionsProvider : DevExpress.Utils.ComponentActions {
		void ShowCollectionDesigner(IComponent component, string propertyName) {
			System.ComponentModel.Design.IDesigner designer = GetDesigner(component);
			if(designer != null)
				EditorContextHelper.EditValue(designer, component, propertyName);
		}
		public void BackgroundImage(IComponent component) {
			ShowCollectionDesigner(component, "BackgroundImage");
		}
		public void Image(IComponent component) {
			ShowCollectionDesigner(component, "Image");
		}
		public void Buttons(IComponent component) {
			ShowCollectionDesigner(component, "Buttons");
		}
	}
	public class BarButtonItemFilter : ISmartTagFilter {
		BarButtonItem barButtonItem;
		public bool FilterMethod(string MethodName, object actionMethodItem) {
			if(barButtonItem == null || barButtonItem.DropDownControl == null) return true;
			if(MethodName == "AddDropDownMenu" || MethodName == "AddDropDownPopupControlContainer" || MethodName == "AddGalleryDropDown") return false;
			return true;
		}
		public bool FilterProperty(MemberDescriptor descriptor) { return true; }
		public void SetComponent(IComponent component) {
			barButtonItem = component as BarButtonItem;
		}
	}
	public class BarButtonItemActionsProvider : DevExpress.Utils.ComponentActions {
		Form GetForm(BarButtonItem buttonItem) {
			if(buttonItem == null) return null;
			if(buttonItem.Manager != null) return buttonItem.Manager.GetForm();
			if(buttonItem.Ribbon != null) return buttonItem.Ribbon.FindForm();
			return null;
		}
		public void AddDropDownMenu(IComponent component) {
			BarButtonItem buttonItem = component as BarButtonItem;
			Form form = GetForm(buttonItem);
			if(form == null) return;
			PopupMenu menu = new PopupMenu(form.Container);
			menu.Ribbon = buttonItem.Ribbon;
			menu.Manager = buttonItem.Manager;
			buttonItem.DropDownControl = menu;
			buttonItem.ActAsDropDown = true;
			buttonItem.ButtonStyle = BarButtonStyle.DropDown;
		}
		public void AddDropDownPopupControlContainer(IComponent component) {
			BarButtonItem buttonItem = component as BarButtonItem;
			Form form = GetForm(buttonItem);
			if(form == null) return;
			PopupControlContainer menu = new PopupControlContainer(form.Container);
			form.Controls.Add(menu);
			menu.Ribbon = buttonItem.Ribbon;
			menu.Manager = buttonItem.Manager;
			buttonItem.DropDownControl = menu;
			buttonItem.ActAsDropDown = true;
			buttonItem.ButtonStyle = BarButtonStyle.DropDown;
		}
		public void AddGalleryDropDown(IComponent component) {
			BarButtonItem buttonItem = component as BarButtonItem;
			Form form = GetForm(buttonItem);
			if(form == null) return;
			GalleryDropDown menu = new GalleryDropDown(form.Container);
			menu.Ribbon = buttonItem.Ribbon;
			menu.Manager = buttonItem.Manager;
			buttonItem.DropDownControl = menu;
			buttonItem.ActAsDropDown = true;
			buttonItem.ButtonStyle = BarButtonStyle.DropDown;
		}
	}
}
