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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Scheduler.Web;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using DevExpress.Web.ASPxScheduler;
namespace DevExpress.ExpressApp.Scheduler.Web {
	public class DateNavigatorController  : ProcessActionContainerHolderController {
		private const string ActionId = "ShowHideDateNavigator";
		private SimpleAction showHideDateNavigatorAction;
		public DateNavigatorController() {
			TargetViewType = ViewType.ListView;
			showHideDateNavigatorAction = new SimpleAction(this, ActionId, DevExpress.Persistent.Base.PredefinedCategory.Search);
			showHideDateNavigatorAction.ImageName = "Action_ShowHideDateNavigator";
			showHideDateNavigatorAction.PaintStyle = ExpressApp.Templates.ActionItemPaintStyle.Image;
		}
		protected override void OnViewChanged() {
			base.OnViewChanged();
			showHideDateNavigatorAction.Active.SetItemValue("ASPxSchedulerListEdittor", View is ListView && ((ListView)View).Editor is ASPxSchedulerListEditor && WebApplicationStyleManager.IsNewStyle);
		}
		protected override bool NeedSubscribeToHolderMenuItemsCreated(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer webActionContainer) {
			return webActionContainer.ContainerId == "Search";
		}
		protected override void OnActionContainerHolderActionItemCreated(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionBaseItem item) {
			base.OnActionContainerHolderActionItemCreated(item);
			ASPxSchedulerListEditor editor = ((ListView)View).Editor as ASPxSchedulerListEditor;
			if(editor != null) {
				ASPxDateNavigator dateNavigator = (ASPxDateNavigator)editor.DateNavigator;
				if(item.Action.Id == ActionId) {
					ClickableMenuActionItem clickableMenuActionItem = item as ClickableMenuActionItem;
					if(clickableMenuActionItem != null) {
						clickableMenuActionItem.ClientClickScript = ShowHideDateNavigatorSchedulerScriptService.GetShowHideDateNavigatorScript(dateNavigator.ClientID);
						clickableMenuActionItem.ProcessOnServer = false;
					}
				}
			}
		}
	}
}
