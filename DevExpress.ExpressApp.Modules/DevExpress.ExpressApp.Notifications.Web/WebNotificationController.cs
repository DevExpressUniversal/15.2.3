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
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates;
[assembly: WebResource(DevExpress.ExpressApp.Notifications.Web.WebNotificationsController.ScriptResourceName, "application/x-javascript")]
namespace DevExpress.ExpressApp.Notifications.Web {
	public class WebNotificationsController : NotificationsController, IXafCallbackHandler {
		private string timerUrl;
		private IBinaryDataRequestProcessor timerProcessor;
		internal const string ScriptResourceName = "DevExpress.ExpressApp.Notifications.Web.showNotificationWindow.js";
		void IXafCallbackHandler.ProcessAction(string parameter) {
			if(parameter.Contains("ShowNotificationView")) {
				if(!isViewShowing) {
					service.Refresh();
				}
			}
		}
		private static string GetHandlerUrl(string url) {
			return "DXX.axd?handlerName=BinaryDataHttpHandler&d=" + HttpUtility.UrlEncode(url);
		}
		private void RegisterUpdateTimerScript(string url) {
			if(notificationsModule != null) {
				int refreshInterval = (int)notificationsModule.NotificationsRefreshInterval.TotalMilliseconds;
				int startDelay = (int)notificationsModule.NotificationsStartDelay.TotalMilliseconds;
				string updateTimerScript = String.Format("window.xafFramework.RegisterNotificationCallback({0}, {1}, '{2}');", refreshInterval, startDelay, url);
				((WebWindow)Frame).RegisterStartupScript("InitReminder", updateTimerScript, true);
			}
		}
		private void RegisterClearTimerScript() {
			string clearTimersScript = String.Format("window.xafFramework.ClearTimersScript();");
			((WebWindow)Frame).RegisterStartupScript("ClearTimersScript", clearTimersScript, true);
		}
		private void Frame_TemplateChanged(object sender, EventArgs e) {
			if(Frame.Template != null) {
				((Page)Frame.Template).LoadComplete += (s, args) => {
					if(!((Page)Frame.Template).IsPostBack) {
						isViewShowing = false;
						RegisterUpdateTimerScript(timerUrl);
					}
					ICallbackManagerHolder holder = (ICallbackManagerHolder)Frame.Template;
					holder.CallbackManager.RegisterHandler("NotificationController", this);
				};
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			((WebWindow)Window).RegisterClientScriptResource(typeof(WebNotificationsController), ScriptResourceName);
			#region BinaryDataRequestProcessor
			ImageProcessorsHelper.RegisterBinaryDataProcessor(timerProcessor);
			NameValueCollection parameters = new NameValueCollection();
			parameters[BinaryDataRequestProcessorFromDatabase.BinaryDataProcessorKey] = timerProcessor.Id;
			parameters["TimeStamp"] = DateTime.Now.GetHashCode().ToString();
			timerUrl = UrlHelper.BuildQueryString(parameters);
			timerUrl = GetHandlerUrl(timerUrl);
			#endregion
			Frame.TemplateChanged += Frame_TemplateChanged;
			RegisterUpdateTimerScript(timerUrl);
		}
		protected override void OnDeactivated() {
			ImageProcessorsHelper.RemoveBinaryDataProcessor(timerProcessor);
			if(Frame != null) {
				Frame.TemplateChanged -= Frame_TemplateChanged;
				RegisterClearTimerScript();
			}
			base.OnDeactivated();
		}
		protected override void ShowNotificationViewCore(ShowViewParameters showViewParameters) {
			Window window = Application.CreatePopupWindow(TemplateContext.PopupWindow, showViewParameters.CreatedView.Id, showViewParameters.CreateAllControllers, showViewParameters.Controllers.ToArray());
			window.SetView(showViewParameters.CreatedView, Frame);
			((WebApplication)Application).PopupWindowManager.ShowPopup(window as WebWindow, WebWindow.CurrentRequestWindow, true);
		}
		public WebNotificationsController() {
			timerProcessor = new NotificationsBinaryDataRequestProcessor(this);
		}
		public bool IsViewShowing {
			get { return isViewShowing; }
		}
	}
}
