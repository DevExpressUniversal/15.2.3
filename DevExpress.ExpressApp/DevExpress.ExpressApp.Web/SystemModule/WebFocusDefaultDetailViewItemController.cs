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
using System.Text;
using DevExpress.ExpressApp.SystemModule;
using System.Web.UI;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Templates;
using System.Collections.Specialized;
using System.Web;
using System.ComponentModel;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class WebFocusDefaultDetailViewItemController : FocusDefaultDetailViewItemController {
		private bool needFocusControlOnCurrentRequest;
		private void WebLayoutManager_ItemCreated(object sender, ItemCreatedEventArgs e) {
			if(needFocusControlOnCurrentRequest && e.ModelLayoutElement != null && e.ModelLayoutElement is IModelTabbedGroup && e.TemplateContainer is TabbedGroupTemplateContainer) {
				foreach(IModelLayoutGroup currentNode in tabPageNodesWithDefaultItemList) {
					if(GetFullPathNode((IModelViewLayoutElement)currentNode.Parent) == GetFullPathNode(e.ModelLayoutElement)) {
						((TabbedGroupTemplateContainer)e.TemplateContainer).ActiveTabName = currentNode.Id;
						break;
					}
				}
			}
		}
		private void WebFocusDefaultDetailViewItemController_PreRender(object sender, EventArgs e) {
			((Control)sender).PreRender -= new EventHandler(WebFocusDefaultDetailViewItemController_PreRender);
			FocusDefaultItemControl();
		}
		private void CurrentRequestPage_Unload(object sender, EventArgs e) {
			((Page)sender).Unload -= new EventHandler(CurrentRequestPage_Unload);
			if(sender is ICallbackManagerHolder) {
				((ICallbackManagerHolder)sender).CallbackManager.PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
			}
		}
		private bool IsScriptRegistered(string key) {
			if(WebWindow.CurrentRequestWindow != null) {
				Page page = WebWindow.CurrentRequestWindow.Template as Page;
				return WebWindow.CurrentRequestWindow.StartUpScripts.ContainsKey(key) || (page != null && page.ClientScript.IsStartupScriptRegistered(typeof(WebWindow), key));
			}
			return false;
		}
		private string GetFocusOnCallbackScript(string controlToFocusId) {
			string functionBody =
@"function(){
                         var defaultItemClientControl = document.getElementById('" + controlToFocusId + @"');
                         if (defaultItemClientControl) {
                            var innerInputs = defaultItemClientControl.getElementsByTagName('input');
                            if (innerInputs && innerInputs.length > 0) {
                                for (var inputsCounter = 0; inputsCounter < innerInputs.length; inputsCounter++) {
                                    if (innerInputs[inputsCounter].getAttribute('type') == 'text') {
                                        try {
                                             innerInputs[inputsCounter].focus();
                                        }
                                        catch (err) { }
                                    }
                                }
                            }
                        }
}";
			if(WebApplicationStyleManager.IsNewStyle) {
				return "WaitPageLoad(" + functionBody + ", true);";
			}
			else {
				return "window.setTimeout(" + functionBody + ", 1)";
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			DetailView view = sender as DetailView;
			if(WebLayoutManager != null && WebLayoutManager == view.LayoutManager) {
				WebLayoutManager.ItemCreated -= new EventHandler<ItemCreatedEventArgs>(WebLayoutManager_ItemCreated);
			}
			if(view.ViewEditMode == ViewEditMode.Edit) {
				view.ControlsCreated -= new EventHandler(View_ControlsCreated);
			}
			Page page = WebWindow.CurrentRequestPage;
			if(page != null) {
				page.Unload += new EventHandler(CurrentRequestPage_Unload);
				if(View.IsRoot && needFocusControlOnCurrentRequest) {
					if(!page.IsCallback) {
						((Control)View.Control).PreRender += new EventHandler(WebFocusDefaultDetailViewItemController_PreRender);
					}
					else if(page is ICallbackManagerHolder) {
						((ICallbackManagerHolder)page).CallbackManager.PreRenderInternal += new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
					}
				}
			}
			if(defaultItem != null && defaultItem.Control == null) {
				defaultItem.CreateControl();
			}
		}
		private void CallbackManager_PreRenderInternal(object sender, EventArgs e) {
			((XafCallbackManager)sender).PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
			FocusDefaultItemControl();
		}
		protected override void FocusDefaultItemControlCore() {
			if(!IsScriptRegistered(PopupWindowManager.ShowViewInNewWindowScriptKey) && !IsScriptRegistered(PopupWindowManager.ShowPopupWindowScriptKey)) {
				Page page = WebWindow.CurrentRequestPage;
				if(page != null && defaultItem is WebPropertyEditor) {
					Control defaultItemControl = ((WebPropertyEditor)defaultItem).Editor;
					if(defaultItemControl != null) {
						if(!page.IsCallback || defaultItemControl is ASPxWebControl) {
							if(defaultItemControl.Page != null) {
								RegisterFocusDefaultItemControlScript(page, defaultItemControl);
							}
							else {
								defaultItemControl.Load += delegate(object sender, EventArgs e) {
									RegisterFocusDefaultItemControlScript(page, defaultItemControl);
								};
							}
						}
						else if(page is ICallbackManagerHolder) {
							((ICallbackManagerHolder)page).CallbackManager.RegisterClientScript("FocusDefaultItemControl", GetFocusOnCallbackScript(defaultItemControl.ClientID));
						}
					}
				}
			}
		}
		private void RegisterFocusDefaultItemControlScript(Page page, Control defaultItemControl) {
			if(WebApplicationStyleManager.IsNewStyle) {
				if(page.IsCallback) {
					ICallbackManagerHolder holder = page as ICallbackManagerHolder;
					if(holder != null) {
						holder.CallbackManager.RegisterClientScript("FocusDefaultItemControl", GetFocusOnCallbackScript(defaultItemControl.ClientID));
					}
				}
				else {
					page.ClientScript.RegisterClientScriptBlock(GetType(), "FocusDefaultItemControl", GetFocusOnCallbackScript(defaultItemControl.ClientID), true);
				}
			}
			else {
				defaultItemControl.Focus();
			}
		}
		protected override void OnViewControlsCreating() {
			string callbackId = RequestHelper.CallbackControlId;
			needFocusControlOnCurrentRequest = string.IsNullOrEmpty(callbackId) || callbackId == XafCallbackManager.CallbackControlID;
			if(View != null && View.IsRoot && View.IsControlCreated) {
				((Control)View.Control).PreRender -= new EventHandler(WebFocusDefaultDetailViewItemController_PreRender);
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
		protected override void OnActivated() {
			base.OnActivated();
			if(NeedDisableLogic) {
				return;
			}
			if(WebLayoutManager != null && NeedActivateDefaultTab()) {
				WebLayoutManager.ItemCreated += new EventHandler<ItemCreatedEventArgs>(WebLayoutManager_ItemCreated);
			}
			View.ControlsCreated += new EventHandler(View_ControlsCreated);
		}
		protected override void OnDeactivated() {
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			if(View.IsControlCreated && View.IsRoot) {
				((Control)View.Control).PreRender -= new EventHandler(WebFocusDefaultDetailViewItemController_PreRender);
			}
			if(WebLayoutManager != null) {
				WebLayoutManager.ItemCreated -= new EventHandler<ItemCreatedEventArgs>(WebLayoutManager_ItemCreated);
			}
			base.OnDeactivated();
		}
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public WebLayoutManager WebLayoutManager {
			get {
				return View.LayoutManager as WebLayoutManager;
			}
		}
	}
}
