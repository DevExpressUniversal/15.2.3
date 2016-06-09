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

using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using System;
using System.Collections.Generic;
using System.Web.UI;
using DevExpress.ExpressApp.Web.Layout;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public abstract class ProcessActionContainerHolderController : ViewController {
		private List<ActionContainerHolder> holdersToProcess = new List<ActionContainerHolder>();
		private void ClearHolders() {
			foreach(ActionContainerHolder holder in holdersToProcess) {
				holder.MenuActionItemCreated -= new EventHandler<MenuActionItemCreatedEventArgs>(Owner_MenuActionItemCreated);
			}
			holdersToProcess.Clear();
		}
		private void Frame_TemplateChanged(object sender, EventArgs e) {
			ClearHolders();
		}
		private void Frame_ProcessActionContainer(object sender, ProcessActionContainerEventArgs e) {
			WebActionContainer webActionContainer = e.ActionContainer as WebActionContainer;
			if(webActionContainer != null && NeedSubscribeToHolderMenuItemsCreated(webActionContainer)) {
				if(!holdersToProcess.Contains(webActionContainer.Owner)) {
					OnProcessActionContainer(webActionContainer);
					holdersToProcess.Add(webActionContainer.Owner);
					webActionContainer.Owner.MenuActionItemCreated += new EventHandler<MenuActionItemCreatedEventArgs>(Owner_MenuActionItemCreated);
				}
			}
		}
		protected virtual void OnProcessActionContainer(WebActionContainer webActionContainer) {
		}
		private void Owner_MenuActionItemCreated(object sender, MenuActionItemCreatedEventArgs e) {
			OnActionContainerHolderActionItemCreated(e.ActionItem);
		}
		protected virtual void OnActionContainerHolderActionItemCreated(WebActionBaseItem item) { }
		protected virtual bool NeedSubscribeToHolderMenuItemsCreated(WebActionContainer webActionContainer) {
			return true;
		}
		protected override void OnActivated() {
			base.OnActivated();
			Frame.ProcessActionContainer += new EventHandler<ProcessActionContainerEventArgs>(Frame_ProcessActionContainer);
			Frame.TemplateChanged += new EventHandler(Frame_TemplateChanged);
		}
		protected override void OnDeactivated() {
			Frame.ProcessActionContainer -= new EventHandler<ProcessActionContainerEventArgs>(Frame_ProcessActionContainer);
			Frame.TemplateChanged -= new EventHandler(Frame_TemplateChanged);
			ClearHolders();
			base.OnDeactivated();
		}
	}
	public class FocusController : WindowController {
		protected override void OnActivated() {
			base.OnActivated();
			if(!NeedDisableLogic) {
				Frame.TemplateChanged += Frame_TemplateChanged;
				Frame.TemplateChanging += Frame_TemplateChanging;
			}
		}
		protected virtual bool NeedDisableLogic {
			get {
				if(WebApplicationStyleManager.IsNewStyle) {
					return DeviceDetector.Instance.GetDeviceCategory() != DeviceCategory.Desktop;
				}
				else {
					return false;
				}
			}
		}
		void Frame_TemplateChanging(object sender, EventArgs e) {
			Page page = Frame.Template as Page;
			if(page != null) {
				page.LoadComplete -= page_LoadComplete;
			}
		}
		void Frame_TemplateChanged(object sender, EventArgs e) {
			if(!NeedDisableLogic) {
				Page page = Frame.Template as Page;
				if(page != null) {
					page.LoadComplete += page_LoadComplete;
				}
			}
		}
		private void page_LoadComplete(object sender, EventArgs e) {
			Focus(Frame.Template);
		}
		protected void Focus(IFrameTemplate template) {
			object controlToFocus = GetControlOrMenuActionItemToFocus(template);
			if(controlToFocus is TemplatedMenuActionItem) {
				FocusTemplatedMenuActionItem((TemplatedMenuActionItem)controlToFocus);
			}
			else {
				if(controlToFocus is MenuActionItemBase) {
					FocusMenuActionItem((MenuActionItemBase)controlToFocus);
				}
			}
		}
		protected virtual void FocusTemplatedMenuActionItem(TemplatedMenuActionItem templatedMenuActionItem) {
			templatedMenuActionItem.Focus();
		}
		protected override void OnDeactivated() {
			Frame.TemplateChanged -= Frame_TemplateChanged;
			Frame.TemplateChanging -= Frame_TemplateChanging;
			base.OnDeactivated();
		}
		protected virtual void FocusMenuActionItem(MenuActionItemBase menuActionItemBase) {
			if(!((System.Web.UI.Page)Frame.Template).ClientScript.IsClientScriptBlockRegistered("FocusAcceptButtonController")) {
				string focusItemByIndexPathFunctionBody = @"function(){" + menuActionItemBase.MenuItem.Menu.ClientID + @".FocusItemByIndexPath(" + menuActionItemBase.MenuItem.IndexPath + @");}";
				string focusItemByIndexPathFunction;
				if(WebApplicationStyleManager.IsNewStyle) {
					focusItemByIndexPathFunction = "attachWindowEvent('load', function(){ WaitAnimateComplete(" + focusItemByIndexPathFunctionBody + ")});";
				}
				else {
					focusItemByIndexPathFunction = "attachWindowEvent('load', " + focusItemByIndexPathFunctionBody + ");";
				}
				((System.Web.UI.Page)Frame.Template).ClientScript.RegisterClientScriptBlock(GetType(), "FocusAcceptButtonController", @"
window.xafProcessEnterKeyDownScript = 'xafProcessEnterKeyDownScriptFunction()';
" + focusItemByIndexPathFunction + @"
function xafProcessEnterKeyDownScriptFunction() {
    var menu = " + menuActionItemBase.MenuItem.Menu.ClientID + @"; 
    for(var i=0; i<menu.GetItemCount(); i++) {
        var menuItem = menu.GetItem(i);
        if(menuItem.name==""" + menuActionItemBase.Action.Id + @""") {
            if(menuItem.GetEnabled()) {
                menu.DoItemClick(menuItem.indexPath); 
            }
            break;
        }
    }
}
", true);
			}
		}
		protected virtual Object GetControlOrMenuActionItemToFocus(IFrameTemplate template) {
			Object result = null;
			ICollection<IActionContainer> containers = template.GetContainers();
			if(HasSearchAction()) {
				result = GetSearchActionControl(containers);
			}
			else {
				MenuActionItemBase okActionControl = GetOkActionControl(containers);
				if(okActionControl != null) {
					result = okActionControl;
				}
				else {
				}
			}
			return result;
		}
		protected virtual MenuActionItemBase GetOkActionControl(ICollection<IActionContainer> actionContainers) {
			MenuActionItemBase result = null;
			foreach(IActionContainer container in actionContainers) {
				foreach(ActionBase action in container.Actions) {
					if(action.ActionMeaning == ActionMeaning.Accept && action.Active) {
						WebActionContainer webActionContainer = container as WebActionContainer;
						if(webActionContainer != null) {
							webActionContainer.Owner.EnsureChildControls();
							result = webActionContainer.Owner.FindActionItem(action);
						}
					}
				}
			}
			return result;
		}
		protected virtual TemplatedMenuActionItem GetSearchActionControl(ICollection<IActionContainer> actionContainers) {
			TemplatedMenuActionItem result = null;
			foreach(IActionContainer container in actionContainers) {
				foreach(ActionBase action in container.Actions) {
					if(action.Id == FilterController.FullTextSearchActionId) {
						WebActionContainer webActionContainer = container as WebActionContainer;
						if(webActionContainer != null) {
							webActionContainer.Owner.EnsureChildControls();
							TemplatedMenuActionItem templatedMenuActionItem = webActionContainer.Owner.FindActionItem(action) as TemplatedMenuActionItem;
							if(templatedMenuActionItem != null) {
								result = templatedMenuActionItem;
								break;
							}
						}
					}
				}
				if(result != null) {
					break;
				}
			}
			return result;
		}
		protected virtual bool HasSearchAction() {
			return HasSearchActionCore(Frame.GetController<FilterController>(), Frame.Template, Frame is PopupWindow);
		}
		protected bool HasSearchActionCore(FilterController filterController, IFrameTemplate frameTemplate, bool frameIsPopupWindow) {
			bool result = false;
			if(frameIsPopupWindow) {
				if(filterController != null && filterController.FullTextFilterAction.Active) {
					if(frameTemplate != null) {
						ILookupPopupFrameTemplate lookupPopupFrameTemplate = frameTemplate as ILookupPopupFrameTemplate;
						if(lookupPopupFrameTemplate == null) {
							ITemplateContentHolder templateContentHolder = frameTemplate as ITemplateContentHolder;
							if(templateContentHolder != null) {
								lookupPopupFrameTemplate = templateContentHolder.TemplateContent as ILookupPopupFrameTemplate;
							}
						}
						if(lookupPopupFrameTemplate != null) {
							result = lookupPopupFrameTemplate.IsSearchEnabled;
						}
					}
				}
			}
			return result;
		}
	}
}
