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
using System.Text;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class WebShowStartupNavigationItemController : WindowController {
		private void WebShowStartupNavigationItemController_StartupWindowCreated(object sender, EventArgs e) {
			ShowStartupNavigationItem();
		}
		protected override void OnActivated() {
			base.OnActivated();
			((WebApplication)Application).StartupWindowCreated += new EventHandler(WebShowStartupNavigationItemController_StartupWindowCreated);
		}
		protected override void OnDeactivated() {
			((WebApplication)Application).StartupWindowCreated -= new EventHandler(WebShowStartupNavigationItemController_StartupWindowCreated);
			base.OnDeactivated();
		}
		protected virtual void ShowStartupNavigationItem() {
			ShowNavigationItemController controller = Window.GetController<ShowNavigationItemController>();
			SingleChoiceAction showNavigationItemAction = controller != null ? controller.ShowNavigationItemAction : null;
			if(showNavigationItemAction != null && showNavigationItemAction.Active && showNavigationItemAction.Enabled) {
				ChoiceActionItem startupNavigationItem = controller.GetStartupNavigationItem();
				if(startupNavigationItem != null) {  
					showNavigationItemAction.DoExecute(startupNavigationItem);
				}
			}
		}
		public WebShowStartupNavigationItemController() {
			this.TargetWindowType = WindowType.Main;
		}
	}
	[ToolboxItem(false)]
	public class StartupNavigationItemCallbackHandler : ASPxPanel, IXafCallbackHandler {
		private const string navigationUrlPropertyName = @"cpCurrentView";
		public const string CallbackHandlerID = "StartupNavigationItemCallbackHandler";
		private void ShowStartupView(Window window) {
			ShowNavigationItemController controller = window.GetController<ShowNavigationItemController>();
			if(controller != null && controller.ShowNavigationItemAction.Active && controller.ShowNavigationItemAction.Enabled) {
				ChoiceActionItem startupNavigationItem = controller.GetStartupNavigationItem();
				if(startupNavigationItem != null) {  
					controller.ShowNavigationItemAction.DoExecute(startupNavigationItem);
				}
			}
		}
		private bool TryNavigateToPreviousViewInHistory(Window window) {
			bool result = false;
			WebViewNavigationController navigationController = window.GetController<WebViewNavigationController>();
			if(navigationController != null) {
				SingleChoiceAction navigateToAction = navigationController.NavigateToAction;
				result = navigateToAction.Active && navigateToAction.Enabled && navigateToAction.Items.Count > 1;
				if(result) {
					navigateToAction.DoExecute(navigateToAction.Items[navigateToAction.Items.Count - 2]);
				}
			}
			return result;
		}
		private string GetViewQueryString() {
			View view = WebWindow.CurrentRequestWindow != null ? WebWindow.CurrentRequestWindow.View : null;
			return view != null && WebApplication.Instance != null ? WebApplication.Instance.RequestManager.GetQueryString(view.CreateShortcut()) : String.Empty;
		}
		private void CallbackManager_PreRenderInternal(object sender, EventArgs e) {
			if(!(WebWindow.CurrentRequestWindow is PopupWindow)) {
				((XafCallbackManager)sender).CallbackControl.JSProperties[navigationUrlPropertyName] = GetViewQueryString();
			}
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			ICallbackManagerHolder callbackManagerHolder = Page as ICallbackManagerHolder;
			if(callbackManagerHolder != null) {
				callbackManagerHolder.CallbackManager.RegisterHandler(CallbackHandlerID, this);
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			ICallbackManagerHolder callbackManagerHolder = Page as ICallbackManagerHolder;
			if(callbackManagerHolder != null) {
				callbackManagerHolder.CallbackManager.PreRenderInternal += new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
			}
			ID = CallbackHandlerID;
			ClientVisible = false;
		}
		protected override void OnUnload(EventArgs e) {
			ICallbackManagerHolder callbackManagerHolder = Page as ICallbackManagerHolder;
			if(callbackManagerHolder != null) {
				callbackManagerHolder.CallbackManager.PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
			}
			base.OnUnload(e);
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			WebWindow window = WebWindow.CurrentRequestWindow;
			if(window != null && window.IsMain) {
				StringBuilder onInit = new StringBuilder();
				onInit.AppendLine("function(s, e) {");
				string viewQueryString = GetViewQueryString();
				if(!string.IsNullOrEmpty(viewQueryString)) {
					onInit.AppendFormat("    window.XAFCurrentQueryString = '{0}';", viewQueryString).AppendLine();
					onInit.AppendLine("    if(document.location.hash == '') {");
					onInit.AppendFormat("        document.location.hash = '{0}';", viewQueryString).AppendLine();
					onInit.AppendLine("    }");
				}
				onInit.AppendFormat("    StartCheckQueryString('{0}');", CallbackHandlerID).AppendLine();
				onInit.Append("}");
				ClientSideEvents.Init = onInit.ToString();
			}
		}
		#region IXafCallbackHandler Members
		public void ProcessAction(string parameter) {
			WebApplication application = WebApplication.Instance;
			Window window = WebWindow.CurrentRequestWindow;
			if(application != null && window != null) {
				ViewShortcut shortcut = application.RequestManager.GetViewShortcut(parameter);
				if(!string.IsNullOrEmpty(shortcut.ViewId)) {
					window.SetView(application.ProcessShortcut(shortcut));
				}
				else if(parameter == "startup_view" || !TryNavigateToPreviousViewInHistory(window)) {
					ShowStartupView(window);
				}
			}
		}
		#endregion
	}
}
