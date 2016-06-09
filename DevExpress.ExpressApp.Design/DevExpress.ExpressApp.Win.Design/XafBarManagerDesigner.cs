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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.ExpressApp.Win.Templates.Controls;
using DevExpress.Utils.Design;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Design;
namespace DevExpress.ExpressApp.Win.Design {
	public class XafBarManagerDesigner : BarManagerDesigner {
		public XafBarManagerDesigner() : base() {
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XafBarManagerActionList(this));
			base.RegisterActionLists(list);
		}
		protected override void CreateDefaultBars() {
			XafBar mainMenuBar = new XafBar();
			mainMenuBar.BarName = "Main Menu";
			mainMenuBar.DockStyle = BarDockStyle.Top;
			mainMenuBar.DockRow = 0;
			mainMenuBar.ApplyDockRowCol();
			XafBar statusBar = new XafBar();
			statusBar.BarName = "StatusBar";
			statusBar.DockStyle = BarDockStyle.Bottom;
			statusBar.ApplyDockRowCol();
			XafBar toolsBar = new XafBar();
			toolsBar.BarName = "Main Toolbar";
			toolsBar.DockStyle = BarDockStyle.Top;
			toolsBar.DockRow = 1;
			toolsBar.ApplyDockRowCol();
			Manager.Bars.Add(mainMenuBar);
			Manager.MainMenu = mainMenuBar;
			Manager.Bars.Add(statusBar);
			Manager.StatusBar = statusBar;
			Manager.Bars.Add(toolsBar);
			EditorContextHelperEx.RefreshSmartPanel(Component);
			ComponentChangeService.OnComponentChanged(Manager, null, null, null);
		}
		IComponentChangeService componentChangeService;
		IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null) {
					componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
				}
				return componentChangeService;
			}
		}
	}
	public class XafBarManagerActionList : BarManagerActionList {
		private IComponentChangeService componentChangeService;
		private IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null) {
					componentChangeService = (IComponentChangeService)Designer.Component.Site.GetService(typeof(IComponentChangeService));
				}
				return componentChangeService;
			}
		}
		public XafBarManagerActionList(XafBarManagerDesigner designer) : base(designer) { }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			result.Add(new DesignerActionMethodItem(this, "CreateXafBar", "Create Xaf Bar", "Menu"));
			result.Add(new DesignerActionMethodItem(this, "AddDefaultContainers", "Add Default Action Containers", "Menu"));
			result.Add(new DesignerActionMethodItem(this, "ClearBars", "Clear Bars", "Menu"));
			return result;
		}
		private void AddItemToManager(BarItems items) {
			foreach(BarItem barItem in items) {
				 CloneBarItem(barItem);
			}
			foreach(BarItem barItem in items) {
				if(barItem is BarLinkContainerItem) {
					foreach(BarItemLink link in ((BarLinkContainerItem)barItem).ItemLinks) {
						((BarLinkContainerItem)FindItem(barItem)).AddItem(FindItem(link.Item));
					}
				}
			}
		}
		private void CloneBarItem(BarItem barItem) {
			Type barItemType = barItem.GetType();
			BarItem result = (BarItem)Activator.CreateInstance(barItemType);
			Type iActionContainerInterface = barItemType.GetInterface("IActionContainer");
			if(iActionContainerInterface != null) {
				((IActionContainer)result).ContainerId = ((IActionContainer)barItem).ContainerId;
			}
			result.Name = barItem.Name;
			result.Caption = barItem.Caption;
			result.Id = barItem.Id;
			result.MergeOrder = barItem.MergeOrder;
			result.MergeType = barItem.MergeType;
			Type iTargetRibbonElementInterface = barItemType.GetInterface("ITargetRibbonElement");
			if(iTargetRibbonElementInterface != null) {
				((ITargetRibbonElement)result).TargetPageCaption = ((ITargetRibbonElement)barItem).TargetPageCaption;
				((ITargetRibbonElement)result).TargetPageCategoryCaption = ((ITargetRibbonElement)barItem).TargetPageCategoryCaption;
			}
			ClonePropertyValue<bool>("VisibleInRibbon", barItem, result);
			ClonePropertyValue<bool>("ApplicationMenuItem", barItem, result);
			ClonePropertyValue<int>("ApplicationMenuIndex", barItem, result);
			Manager.Container.Add(result);
			Manager.Items.Add(result);
		}
		private void ClonePropertyValue<T>(string propertyName, BarItem sourceItem, BarItem targetItem) {
			PropertyInfo property = sourceItem.GetType().GetProperty(propertyName);
			if(property != null) {
				T propertyValue = (T)property.GetValue(sourceItem, null);
				property.SetValue(targetItem, propertyValue, null);
			}
		}
		private void CopyBarItems(Bar sourceBar, Bar targetBar) {
			foreach(BarItemLink itemLink in sourceBar.ItemLinks) {
				targetBar.AddItem(FindItem(itemLink.Item));
			}
		}
		private BarItem FindItem(BarItem barItem) {
			foreach(BarItem item in Manager.Items) {
				if(item.Caption == barItem.Caption && item.Id == barItem.Id) {
					return item; 
				}
			}
			return null;
		}
		[RefreshProperties(RefreshProperties.All)]
		public void AddDefaultContainers() {
			MainForm mainForm = new MainForm();
			mainForm.BarManager.ForceInitialize();
			mainForm.BarManager.ForceLinkCreate();
			AddItemToManager(mainForm.BarManager.Items);
			if(Manager.MainMenu != null) {
				CopyBarItems(mainForm.BarManager.Bars["Main Menu"], Manager.MainMenu);
			}
			foreach(Bar bar in Manager.Bars) {
				if(bar != Manager.MainMenu) {
					CopyBarItems(mainForm.BarManager.Bars["Main Toolbar"], bar);
					break;
				}
			}
			ComponentChangeService.OnComponentChanged(Manager, null, null, null);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void ClearBars() {
			if(MessageBox.Show("You are about to delete all Bar Items. Are you sure?", "", MessageBoxButtons.YesNo) == DialogResult.Yes) {
				foreach(Bar bar in Manager.Bars) {
					bar.ItemLinks.Clear();
				}
				Manager.Items.Clear();
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateXafBar() {
			XafBar bar = new XafBar();
			bar.DockStyle = BarDockStyle.Top;
			Manager.Bars.Add(bar);
			bar.ApplyDockRowCol();
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
	}
}
