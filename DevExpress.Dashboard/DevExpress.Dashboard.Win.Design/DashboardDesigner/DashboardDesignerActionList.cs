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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Design;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Design {
	public class DashboardDesignerActionList : DashboardActionList {
		readonly DashboardDesigner control;
		readonly Control parentControl;
		readonly IContainer container;
		readonly IDesignerHost designerHost;
		readonly IComponentChangeService changeService;
		public DashboardDesignerActionList(DashboardDesignerControlDesigner designer) : base(designer.Control) {
			control = designer.Control as DashboardDesigner;
			if (control != null) {
				container = control.Container;
				parentControl = DevExpress.XtraBars.Design.DesignHelpers.GetContainerControl(container);
				ISite site = control.Site;
				if (site != null) {
					designerHost = site.GetService(typeof(IDesignerHost)) as IDesignerHost;
					changeService = site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				}
			}
		}
		T FindComponent<T>() where T : class {
			foreach (IComponent component in container.Components) {
				T comp = component as T;
				if (comp != null)
					return comp;
			}
			return null;
		}
		void CreateBarInternal(Type componentType) {
			DashboardDesignTimeBarsGenerator generator = new DashboardDesignTimeBarsGenerator(designerHost, control, componentType);
			generator.RemoveExistingBars(componentType == typeof(RibbonControl), FindComponent<RibbonControl>(), FindComponent<BarManager>(), FindComponent<DashboardBarController>());
			generator.AddNewBars(DashboardBarsUtils.GetBarCreators(componentType == typeof(RibbonControl)), String.Empty, BarInsertMode.Add);
		}
		void CreatePopupMenu(RibbonControl ribbon, BarManager manager) {
			DashboardPopupMenu popupMenu = FindComponent<DashboardPopupMenu>();
			if(popupMenu == null) {
				changeService.OnComponentChanging(control, null);
				popupMenu = (DashboardPopupMenu)designerHost.CreateComponent(typeof(DashboardPopupMenu));
				changeService.OnComponentChanged(control, null, null, null);
				changeService.OnComponentChanging(popupMenu, null);
				popupMenu.Manager = manager;
				changeService.OnComponentChanged(popupMenu, null, null, null);
				changeService.OnComponentChanging(popupMenu, null);
				popupMenu.Ribbon = ribbon;
				changeService.OnComponentChanged(popupMenu, null, null, null);
			}
			else {
				changeService.OnComponentChanging(popupMenu, null);
				string barItemsNamespace = DashboardBarsUtils.DashboardSaveBarItemType.Namespace;
				for(int i = popupMenu.ItemLinks.Count - 1; i >= 0; i--) {
					BarItem item = popupMenu.ItemLinks[i].Item;
					if(item != null && item.GetType().Namespace == barItemsNamespace)
						popupMenu.ItemLinks.RemoveAt(i);
				}
				changeService.OnComponentChanged(popupMenu, null, null, null);
			}
			changeService.OnComponentChanging(control, null);
			control.PopupMenu = popupMenu;
			changeService.OnComponentChanged(control, null, null, null);
			DashboardBarsUtils.SetupDesignerPopupMenu(popupMenu);
		}
		public void CreateRibbon() {
			string transactionName = DashboardWinLocalizer.GetString(DashboardWinStringId.DesignerActionListCreateRibbonTransaction);
			DesignerTransaction transaction = designerHost.CreateTransaction(transactionName);
			try {
				RibbonControl ribbon = FindComponent<RibbonControl>();
				if(ribbon == null) {
					changeService.OnComponentChanging(control, null);
					ribbon = (RibbonControl)designerHost.CreateComponent(typeof(RibbonControl));
					changeService.OnComponentChanged(control, null, null, null);
					changeService.OnComponentChanging(control, null);
					DashboardBackstageViewControl viewControl = (DashboardBackstageViewControl)designerHost.CreateComponent(typeof(DashboardBackstageViewControl));
					changeService.OnComponentChanged(control, null, null, null);
					BarAndDockingController controller = FindComponent<BarAndDockingController>();
					if(controller == null) {
						changeService.OnComponentChanging(control, null);
						controller = (DashboardBarAndDockingController)designerHost.CreateComponent(typeof(DashboardBarAndDockingController));
						changeService.OnComponentChanged(control, null, null, null);
					}
					ISupportLookAndFeel lookAndFeelControl = parentControl as ISupportLookAndFeel;
					if(lookAndFeelControl != null) {
						changeService.OnComponentChanging(controller, null);
						controller.LookAndFeel.ParentLookAndFeel = lookAndFeelControl.LookAndFeel;
						changeService.OnComponentChanged(controller, null, null, null);
					}
					changeService.OnComponentChanging(viewControl, null);
					viewControl.Controller = controller;
					changeService.OnComponentChanged(viewControl, null, null, null);
					changeService.OnComponentChanging(ribbon, null);
					ribbon.Controller = controller;
					changeService.OnComponentChanged(ribbon, null, null, null);
					DashboardBarsUtils.SetupRibbon(ribbon, viewControl, control);
					Control.ControlCollection controls = parentControl.Controls;
					changeService.OnComponentChanging(parentControl, null);
					controls.Add(ribbon);
					changeService.OnComponentChanged(parentControl, null, null, null);
					foreach(BackstageViewItem item in viewControl.Items) {
						changeService.OnComponentChanging(control, null);
						designerHost.Container.Add(item);
						changeService.OnComponentChanged(control, null, null, null);
					}
					DashboardBackstageRecentTab recentTab = viewControl.DashboardRecentTab;
					changeService.OnComponentChanging(control, null);
					container.Add(recentTab);
					changeService.OnComponentChanged(control, null, null, null);
					changeService.OnComponentChanging(control, null);
					container.Add(recentTab.ContentControl);
					changeService.OnComponentChanged(control, null, null, null);
					changeService.OnComponentChanging(control, null);
					container.Add(recentTab.RecentDashboardsControl);
					changeService.OnComponentChanged(control, null, null, null);
					changeService.OnComponentChanging(control, null);
					controls.Add(viewControl);
					changeService.OnComponentChanged(control, null, null, null);
				}
				RibbonForm ribbonForm = parentControl as RibbonForm;
				if(ribbonForm != null) {
					changeService.OnComponentChanging(ribbonForm, null);
					ribbonForm.Ribbon = ribbon;
					changeService.OnComponentChanged(ribbonForm, null, null, null);
				}
				changeService.OnComponentChanging(control, null);
				control.MenuManager = ribbon;
				changeService.OnComponentChanged(control, null, null, null);
				changeService.OnComponentChanging(ribbon, null);
				CreateBarInternal(typeof(RibbonControl));
				changeService.OnComponentChanged(ribbon, null, null, null);
				CreatePopupMenu(ribbon, null);
				RibbonPage page = DashboardBarsUtils.FindPage(ribbon, typeof(HomeRibbonPage));
				if(page != null) {
					changeService.OnComponentChanging(ribbon, null);
					ribbon.SelectedPage = page;
					changeService.OnComponentChanged(ribbon, null, null, null);
				}
			}
			finally {
				transaction.Commit();
				EditorContextHelperEx.RefreshSmartPanel(control);
			}
		}
		public void CreateBars() {
			string transactionName = DashboardWinLocalizer.GetString(DashboardWinStringId.DesignerActionListCreateBarsTransaction);
			using (DesignerTransaction transaction = designerHost.CreateTransaction(transactionName)) {
				BarManager barManager = FindComponent<BarManager>();
				if(barManager == null) {
					changeService.OnComponentChanging(container, null);
					barManager = (BarManager)designerHost.CreateComponent(typeof(BarManager));
					changeService.OnComponentChanged(container, null, null, null);
				}
				changeService.OnComponentChanging(barManager, null);
				barManager.Form = parentControl;
				changeService.OnComponentChanged(barManager, null, null, null);
				changeService.OnComponentChanging(control, null);
				control.MenuManager = barManager;
				changeService.OnComponentChanged(control, null, null, null);
				CreateBarInternal(typeof(BarManager));
				CreatePopupMenu(null, barManager);
				transaction.Commit();
			}
		}
		public void RefreshRibbon() {
			using(DesignerTransaction transaction = designerHost.CreateTransaction(DashboardWinLocalizer.GetString(DashboardWinStringId.DesignerActionListUpdateRibbonTransaction))) {
				CreateBarInternal(typeof(RibbonControl));
				CreatePopupMenu(FindComponent<RibbonControl>(), null);
				transaction.Commit();
			}
			EditorContextHelperEx.RefreshSmartPanel(control);
		}
		public void RefreshBars() {
			using(DesignerTransaction transaction = designerHost.CreateTransaction(DashboardWinLocalizer.GetString(DashboardWinStringId.DesignerActionListUpdateBarsTransaction))) {
				CreateBarInternal(typeof(BarManager));
				CreatePopupMenu(null, FindComponent<BarManager>());
				transaction.Commit();
			}
			EditorContextHelperEx.RefreshSmartPanel(control);
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = new DesignerActionItemCollection();
			if (control != null && parentControl != null && container != null && designerHost != null && changeService != null) {
				if (FindComponent<RibbonControl>() == null)
					collection.Add(new DesignerActionMethodItem(this, "CreateRibbon", DashboardWinLocalizer.GetString(DashboardWinStringId.DesignerActionMethodCreateRibbonItem)));
				else
					collection.Add(new DesignerActionMethodItem(this, "RefreshRibbon", DashboardWinLocalizer.GetString(DashboardWinStringId.DesignerActionMethodUpdateRibbonItem)));
				if (FindComponent<BarManager>() == null)
					collection.Add(new DesignerActionMethodItem(this, "CreateBars", DashboardWinLocalizer.GetString(DashboardWinStringId.DesignerActionMethodCreateBarsItem)));
				else
					collection.Add(new DesignerActionMethodItem(this, "RefreshBars", DashboardWinLocalizer.GetString(DashboardWinStringId.DesignerActionMethodUpdateBarsItem)));
			}
			foreach (DesignerActionItem item in base.GetSortedActionItems())
				collection.Add(item);
			return collection;
		}
	}
}
