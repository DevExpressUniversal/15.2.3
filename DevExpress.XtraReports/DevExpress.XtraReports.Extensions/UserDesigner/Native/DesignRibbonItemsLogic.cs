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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraEditors.Repository;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using System.Windows.Forms;
using System.Linq;
namespace DevExpress.XtraReports.UserDesigner.Native {
	public class XRDesignRibbonItemsLogic : XRDesignItemsLogicBase {
		#region static
		static void AddItemToMenu(PopupMenu menu, BarItem subItem) {
			AddItemToMenu(menu, subItem, false);
		}
		static void AddItemToMenu(PopupMenu menu, BarItem subItem, bool beginGroup) {
			if(subItem != null)
				menu.ItemLinks.Add(subItem, beginGroup);
		}
		#endregion
		ToolboxRibbonrLogic toolboxLogic;
		RibbonControl Ribbon { get { return ((RibbonBarManager)Manager).Ribbon.GetActualRibbon(); } }
		RibbonPage GetDesignRibbonPage() { return Ribbon.GetPage(item => item is XRDesignRibbonPage); }
		RibbonPage GetToolboxRibbonPage() { return Ribbon.GetPage(item => item is XRToolboxRibbonPage); }
		public XRDesignRibbonItemsLogic(RibbonBarManager manager, IServiceProvider serviceProvider)
			: base(manager, serviceProvider) {
				Ribbon.SelectedPageChanged += Ribbon_SelectedPageChanged;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			Ribbon.SelectedPageChanged -= Ribbon_SelectedPageChanged;
			UnsubscribeActualToolboxBarManagerEvents();
		}
		protected override void EndInitCore() {
			base.EndInitCore();
			InitSubButtonsItems();
		}
		protected override void InitZoomItem() {
			BarButtonItem subMenuButtonItem = GetButtonByStyle(ReportCommand.Zoom, BarButtonStyle.DropDown);
			if(subMenuButtonItem != null) {
				if(subMenuButtonItem.DropDownControl == null) {
					subMenuButtonItem.ActAsDropDown = true;
					CreateZoomItems(subMenuButtonItem);
				}
				foreach(BarItemLink itemLink in ((PopupMenu)subMenuButtonItem.DropDownControl).ItemLinks) {
					if(itemLink.Item is ZoomRuntimeCommandBarItem)
						((ZoomRuntimeCommandBarItem)itemLink.Item).SetZoomService(ZoomService);
					else if(itemLink.Item is XRZoomBarEditItem)
						((XRZoomBarEditItem)itemLink.Item).SetZoomService(ZoomService);
				}
			}
		}
		protected override void InitFontControls() {
			BarEditItem editItem = XRDesignRibbonControllerConfigurator.GetFontNameEditor(Manager);
			if(editItem != null) {
				this.RecentlyItemsBox = (DevExpress.XtraEditors.IRecentlyUsedItems)editItem.Edit;
				((RepositoryItemComboBox)editItem.Edit).DropDownRows = 12;
				FontNameBox = (RepositoryItemComboBox)editItem.Edit;
				FontNameEdit = editItem;
			}
			editItem = XRDesignRibbonControllerConfigurator.GetFontSizeEditor(Manager);
			if(editItem != null) {
				FontSizeBox = (RepositoryItemComboBox)editItem.Edit;
				FontSizeEdit = editItem;
			}
			base.InitFontControls();
		}
		void InitSubButtonsItems() {
			CreateItemSubmenu(ReportCommand.SaveFile, ReportCommand.SaveFileAs);
			CreateItemSubmenu(ReportCommand.NewReport, ReportCommand.NewReportWizard);
		}
		private void CreateItemSubmenu(ReportCommand primaryCommand, ReportCommand secondaryCommand) {
			BarButtonItem subMenuButtonItem = GetButtonByStyle(primaryCommand, BarButtonStyle.DropDown);
			if(subMenuButtonItem != null) {
				PopupMenu menu = new PopupMenu();
				AddItemToMenu(menu, GetButtonByStyle(primaryCommand, BarButtonStyle.Default));
				AddItemToMenu(menu, GetBarItemsByReportCommand(secondaryCommand).Length > 0 ? GetBarItemsByReportCommand(secondaryCommand)[0] : null);
				subMenuButtonItem.DropDownControl = menu;
			}
		}
		BarButtonItem GetButtonByStyle(ReportCommand command, BarButtonStyle style) {
			foreach(BarItem item in GetBarItemsByReportCommand(command)) {
				BarButtonItem buttonItem = item as BarButtonItem;
				if(buttonItem != null && buttonItem.ButtonStyle == style)
					return buttonItem;
			}
			return null;
		}
		void CreateZoomItems(BarButtonItem subMenuButtonItem) {
			PopupMenu menu = new PopupMenu();
			foreach(int zoomPercent in ZoomService.PredefinedZoomFactorsInPercents) {
				BarItem zoomBarItem = new ZoomRuntimeCommandBarItem(zoomPercent);
				if(zoomPercent == 100)
					zoomBarItem.ItemShortcut = new BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D0);
				AddItem(menu, zoomBarItem, false);
			}
			XRZoomBarEditItem exactItem = new XRZoomBarEditItem();
			exactItem.Caption = ReportLocalizer.GetString(ReportStringId.RibbonXRDesign_ZoomExact_Caption);
			DevExpress.XtraPrinting.Preview.Native.PrintBarManagerConfigurator.AssignTextEditToZoomBarEditItem(Manager, exactItem, new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit());
			AddItem(menu, exactItem, true);
			subMenuButtonItem.DropDownControl = menu;
		}
		void AddItem(PopupMenu menu, BarItem item, bool beginGroup) {
			Manager.Items.Add(item);
			AddItemToMenu(menu, item, beginGroup);
		}
		protected override void OnDesignerDeactivated(object sender, EventArgs e) {
			base.OnDesignerDeactivated(sender, e);
			if(Ribbon.SelectedPage is XRToolboxRibbonPage) Ribbon.SelectedPage = GetDesignRibbonPage();
			ClearToolboxBarItems();
			UpdateToolboxVisibility();
		}
		void ClearToolboxBarItems() {
			RibbonPage page = GetToolboxRibbonPage();
			if(page == null) return;
			page.Groups.Clear();
		}
		protected override void XRDesignPanel_Activated(object sender, EventArgs e) {
			base.XRDesignPanel_Activated(sender, e);
			SubscribeActualToolboxBarManagerEvents();
			ClearToolboxBarItems();
			CreateToolboxBarItems();
			if(toolboxLogic == null)
				toolboxLogic = new ToolboxRibbonrLogic(Ribbon);
			UpdateToolboxVisibility();
		}
		void SubscribeActualToolboxBarManagerEvents() {
			UnsubscribeActualToolboxBarManagerEvents();
			Ribbon.Manager.PressedLinkChanged += Manager_PressedLinkChanged;
			Ribbon.Manager.ItemDoubleClick += Manager_ItemDoubleClick;
		}
		void UnsubscribeActualToolboxBarManagerEvents() {
			Ribbon.Manager.PressedLinkChanged -= Manager_PressedLinkChanged;
			Ribbon.Manager.ItemDoubleClick -= Manager_ItemDoubleClick;
		}
		private void Ribbon_SelectedPageChanged(object sender, EventArgs e) {
			RibbonPage page = GetToolboxRibbonPage();
			if(page != null && page.Visible != false && Ribbon.SelectedPage != page) {
				PressCursorBarItem();
				if(ToolboxService != null) 
					ToolboxService.SetSelectedToolboxItem(null);
				if(Manager != null) Manager.SelectLink(null);
			}
		}
		protected override void Manager_PressedLinkChanged(object sender, HighlightedLinkChangedEventArgs e) {
			if(e.Link == null || !(e.Link.Item is BarCheckItem))
				return;
			RibbonPage page = GetRibbonPageByLink(e.Link);
			if(page == null) return;
			if(!ReferenceEquals(page, GetToolboxRibbonPage())) return;
			BarCheckItem barButtonItem = (BarCheckItem)e.Link.Item;
			if(ToolboxService != null)
				ToolboxService.SetSelectedToolboxItem(barButtonItem.Tag as ToolboxItem);
			if(toolboxLogic != null)
				toolboxLogic.PrepareDragData(e.Link);
		}
		static RibbonPage GetRibbonPageByLink(BarItemLink barItemLink) {
			RibbonPageGroupItemLinkCollection linkedObject = barItemLink.LinkedObject as RibbonPageGroupItemLinkCollection;
			if(linkedObject == null) 
				return null;
			return linkedObject.PageGroup.Page;
		}
		protected override void PressCursorBarItem() {
			BarCheckItem item = GetCurrentCursorItem();
			if(item != null)
				item.Checked = true;
		}
		BarCheckItem GetCurrentCursorItem() {
			ToolboxItem item = ToolboxService != null ? ToolboxService.GetSelectedToolboxItem() : null;
			if(item == null) return null;
			RibbonPage page = GetToolboxRibbonPage();
			if(page == null) return null;
			RibbonPageGroup group = FindGroup(item, page);
			if(group != null) {
				foreach(BarItemLink link in group.ItemLinks) {
					if(link.Item.Tag == null)
						return link.Item as BarCheckItem;
				}
			}
			return null;
		}
		private static RibbonPageGroup FindGroup(ToolboxItem item, RibbonPage page) {
			foreach(RibbonPageGroup group in page.Groups) {
				foreach(BarItemLink link in group.ItemLinks) {
					if(ReferenceEquals(link.Item.Tag, item))
						return group;
				}
			}
			return null;
		}
		public void UpdateToolboxVisibility() {
			RibbonPage page = GetToolboxRibbonPage();
			if(page == null) return;
			if(page.Groups.Count > 0) {
				page.Category.Visible = (Ribbon.SelectedPage is XRDesignRibbonPage) || (Ribbon.SelectedPage is XRToolboxRibbonPage);
				page.Visible = page.Category.Visible;
			} else { page.Visible = page.Category.Visible = false; }
		}
		void CreateToolboxBarItems() {
			RibbonPage page = GetToolboxRibbonPage();
			if(page == null) return;
			XRToolboxService toolboxService = this.XRDesignPanel.GetService(typeof(IToolboxService)) as XRToolboxService;
			if(toolboxService == null) return;
			IDesignerHost host = XRDesignPanel.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return;
			const int toolboxGroup = -1000;
			foreach(string category in toolboxService.CategoryNames) {
				RibbonPageGroup group = new RibbonPageGroup(category);
				group.ShowCaptionButton = false;
				page.Groups.Add(group);
				BarCheckItem cursorItem = new BarCheckItem()
				{
					Caption = ReportLocalizer.GetString(ReportStringId.UD_XtraReportsPointerItemCaption),
					Glyph = toolboxService.GetSmallImage(typeof(object)),
					LargeGlyph = toolboxService.GetLargeImage(typeof(object)),
					AllowAllUp = true,
					Checked = page.Groups.IndexOf(group) == 0,
					Tag = null,
				};
				cursorItem.CheckedChanged += BarItem_CheckedChanged;
				cursorItem.GroupIndex = toolboxGroup;
				Ribbon.Items.Add(cursorItem);
				group.ItemLinks.Add(cursorItem);
				ToolboxItem[][] subcategorizedItems = XRToolboxService.GroupItemsBySubCategory(toolboxService.GetToolboxItems(category), host);
				for(int i = 0; i < subcategorizedItems.Length; i++) {
					ToolboxItem[] subcategory = subcategorizedItems[i];
					for(int j = 0; j < subcategory.Length; j++) {
						ToolboxItem item = subcategory[j];
						BarCheckItem barItem = new BarCheckItem()
						{
							Caption = item is LocalizableToolboxItem ? ((LocalizableToolboxItem)item).DisplayName : item.DisplayName,
							Glyph = toolboxService.GetSmallImage(item),
							LargeGlyph = toolboxService.GetLargeImage(item),
							AllowAllUp = true,
							Checked = false,
							Tag = item
						};
						Ribbon.Items.Add(barItem);
						barItem.GroupIndex = toolboxGroup;
						barItem.CheckedChanged += BarItem_CheckedChanged;
						BarItemLink link = group.ItemLinks.Add(barItem, i != 0 && j == 0);
					}
				}
			}
		}
		private void BarItem_CheckedChanged(object sender, ItemClickEventArgs e) {
			BarCheckItem item = e.Item as BarCheckItem;
			if(item != null && item.Checked == false) {
				item.Checked = true;
			}
		}
	} 
}
