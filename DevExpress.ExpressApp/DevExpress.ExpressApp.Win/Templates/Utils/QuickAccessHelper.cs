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

using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Templates.Ribbon;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.Templates.Utils {
	public class QuickAccessHelper {
		private XafRibbonControlV2 ribbonControl;
		private IActionControlsSite site;
		private ActionCollection allActions;
		private List<string> CollectQuickAccessActionIds() {
			List<string> quickAccessActionId = new List<string>();
			foreach(ActionBase action in allActions) {
				if(action.QuickAccess) {
					quickAccessActionId.Add(action.Id);
				}
			}
			return quickAccessActionId;
		}
		private List<IActionControl> GetAllActionControls() {
			List<IActionControl> result = new List<IActionControl>();
			foreach(IActionControlContainer actionContainer in site.ActionContainers) {
				result.AddRange(actionContainer.GetActionControls());
			}
			result.AddRange(site.ActionControls);
			return result;
		}
		private IActionControl FindActionControl(string actionId) {
			List<IActionControl> allActionControls = GetAllActionControls();
			foreach(IActionControl actionControl in allActionControls) {
				if(actionControl.ActionId == actionId) {
					return actionControl;
				}
			}
			return null;
		}
		private IActionControl FindActionControl(BarItem barItem) {
			List<IActionControl> allActionControls = GetAllActionControls();
			foreach(IActionControl actionControl in allActionControls) {
				if(actionControl.NativeControl == barItem) {
					return actionControl;
				}
			}
			return null;
		}
		private ActionBase FindActionByBarItem(BarItem barItem) {
			IActionControl actionControl = FindActionControl(barItem);
			return actionControl != null ? allActions.Find(actionControl.ActionId) : null;
		}
		private BarItem GetBarItem(string actionId) {
			IActionControl actionControl = FindActionControl(actionId);
			if(actionControl != null && actionControl.NativeControl != null && actionControl.NativeControl is BarItem) {
				return (BarItem)actionControl.NativeControl;
			}
			return null;
		}
		private List<BarItem> CollectActionsBarItems(List<string> actionIds) {
			List<BarItem> barItems = new List<BarItem>();
			foreach(string id in actionIds) {
				BarItem barItem = GetBarItem(id);
				if(barItem != null) {
					barItems.Add(barItem);
				}
			}
			return barItems;
		}
		private void ribbonControl_ToolbarCustomized(object sender, Controls.ToolbarCustomizedEventArgs e) {
			ActionBase action = FindActionByBarItem(e.Link.Item);
			if(action != null) {
				action.QuickAccess = e.Action == CollectionChangeAction.Add;
			}
		}
		public QuickAccessHelper(XafRibbonControlV2 ribbonControl, IActionControlsSite site, ActionCollection allActions) {
			Guard.ArgumentNotNull(ribbonControl, "ribbonControl");
			Guard.ArgumentNotNull(site, "site");
			Guard.ArgumentNotNull(allActions, "allActions");
			this.allActions = allActions;
			this.ribbonControl = ribbonControl;
			this.site = site;
			this.ribbonControl.ToolbarCustomized += ribbonControl_ToolbarCustomized;
		}
		public void FillToolbar() {
			List<string> quickAccessActionIds = CollectQuickAccessActionIds();
			List<BarItem> barItems = CollectActionsBarItems(quickAccessActionIds);
			ribbonControl.Toolbar.ItemLinks.AddRange(barItems.ToArray());
		}
		public void SetRibbonGroupLinkHints() {
			foreach(RibbonPage page in ribbonControl.TotalPageCategory.Pages) {
				foreach(RibbonPageGroup group in page.Groups) {
					group.ToolbarContentButtonLink.Item.Hint = group.Text;
				}
			}
		}
	}
}
