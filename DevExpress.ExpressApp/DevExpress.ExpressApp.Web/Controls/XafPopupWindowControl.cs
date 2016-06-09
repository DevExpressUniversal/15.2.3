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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Web.Utils;
using DevExpress.Utils;
using DevExpress.Web;
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.TransparentImage.png", "image/png")]
namespace DevExpress.ExpressApp.Web.Controls {
	[ToolboxItem(false)]
	public class XafPopupWindowControl : Control {
		internal const string PopupWindowIDPrefix = "PopupWindow";
		internal const string PopupWindowCallbackControlID = "PopupWindowCallback";
		internal const string PopupControlWrapperID = "PopupControlWrapper";
		private bool isAutoHeightEnabled = true;
		private ASPxCallback callbackControl;
		private Panel wrapperPanel;
		private static string ResolveContentUrl(string url) {
			if(!string.IsNullOrEmpty(url) && !url.StartsWith("~") && !url.StartsWith("/")) {
				return "~/" + url;
			}
			return url;
		}
		private void OnCustomizePopupWindowSize(CustomizePopupWindowSizeEventArgs args) {
			if(CustomizePopupWindowSize != null) {
				CustomizePopupWindowSize(this, args);
			}
		}
		private void OnCustomizePopupControl(ASPxPopupControl popupControl) {
			if(CustomizePopupControl != null) {
				CustomizePopupControl(this, new CustomizePopupControlEventArgs(popupControl));
			}
		}
		private ASPxPopupControl CreatePopupControl(string url, bool isSizeable, bool showInFindPopup, string closeScript) {
			ASPxPopupControl popupControl = new ASPxPopupControl();
			popupControl.AutoUpdatePosition = true; 
			popupControl.ID = PopupWindowIDPrefix + popupControl.GetHashCode().ToString();
			popupControl.PopupHorizontalAlign = PopupHorizontalAlign.WindowCenter; 
			popupControl.PopupVerticalAlign = PopupVerticalAlign.WindowCenter; 
			popupControl.AllowDragging = true; 
			popupControl.AllowResize = isSizeable; 
			popupControl.ShowOnPageLoad = true;
			popupControl.ShowMaximizeButton = true;
			popupControl.Modal = true;
			popupControl.ContentStyle.Paddings.Padding = Unit.Pixel(0);
			popupControl.ShowSizeGrip = ShowSizeGrip.Auto; 
			popupControl.FooterText = String.Empty;
			popupControl.HeaderText = String.Empty;
			popupControl.SettingsLoadingPanel.Enabled = true; 
			popupControl.ShowFooter = true; 
			popupControl.CloseAction = CloseAction.CloseButton;
			popupControl.ContentUrl = ResolveContentUrl(url);
			if(!WebApplicationStyleManager.IsNewStyle) {
				popupControl.HeaderImage.Url = Page.ClientScript.GetWebResourceUrl(GetType(), "DevExpress.ExpressApp.Web.Resources.TransparentImage.png");
				popupControl.CssClass = "PopupWindow";
				popupControl.Width = PopupWindow.WindowWidth;
			}
			bool isMobileStyle = DeviceDetector.Instance.GetDeviceCategory() != DeviceCategory.Desktop;
			SetupForNew(popupControl, isMobileStyle, showInFindPopup);
			CustomizePopupWindowSizeEventArgs args = new CustomizePopupWindowSizeEventArgs(url);
			OnCustomizePopupWindowSize(args);
			if(args.Handled) {
				popupControl.Width = args.Size.Width;
				popupControl.Height = args.Size.Height;
			}
			bool canManageSize = (!WebApplicationStyleManager.IsNewStyle || showInFindPopup) && !args.Handled;
			popupControl.ClientSideEvents.PopUp = string.Format("function(s, e) {{ onPopupControlPopUp(s, {0}, {1}, {2}, {3}, {4}, {5}); }}", ClientSideEventsHelper.ToJSBoolean(canManageSize), HeightCore, ClientSideEventsHelper.ToJSBoolean(IsAutoHeightEnabled), ClientSideEventsHelper.ToJSBoolean(WebApplicationStyleManager.IsNewStyle), ClientSideEventsHelper.ToJSBoolean(isMobileStyle), ClientSideEventsHelper.ToJSBoolean(showInFindPopup));
			popupControl.ClientSideEvents.Shown = "function(s, e) { onPopupControlShown(s); }";
			popupControl.ClientSideEvents.CloseUp = string.Format("function(s, e) {{ onPopupControlCloseUp(s, {0}, {1}); {2}}}", ClientSideEventsHelper.ToJSBoolean(WebApplicationStyleManager.IsNewStyle), ClientSideEventsHelper.ToJSBoolean(isMobileStyle), closeScript);
			popupControl.ClientSideEvents.CloseButtonClick = @"function(s, e) { 
                                                                    var contentWindow = s.GetContentIFrame().contentWindow;
                                                                    if(!contentWindow.xaf.ConfirmUnsavedChangedController || contentWindow.xaf.ConfirmUnsavedChangedController.CanClosePopup(s, e)) {  
                                                                        if(contentWindow.xaf){
                                                                            contentWindow.xaf.ConfirmUnsavedChangedController.SetActive(false);
                                                                        }
                                                                        onPopupControlCloseButtonClick(s);
                                                                    }
                                                               }";
			OnCustomizePopupControl(popupControl);
			return popupControl;
		}
		private int HeightCore {
			get {
				if(WebApplicationStyleManager.IsNewStyle) {
					return WebApplication.Instance.ClientInfo.GetValue<int>("ClientHeight");
				}
				else {
					return PopupWindow.WindowHeight;
				}
			}
		}
		private void SetupForNew(ASPxPopupControl popupControl, bool isMobileStyle, bool showInFindPopup) {
			if(WebApplicationStyleManager.IsNewStyle) {
				popupControl.PopupAnimationType = AnimationType.None;
				popupControl.ShowFooter = false;
				popupControl.AllowDragging = false;
				popupControl.AllowResize = false;
				popupControl.ShowHeader = false;
				popupControl.ShowSizeGrip = showInFindPopup ? ShowSizeGrip.Auto : ShowSizeGrip.False;
				popupControl.ScrollBars = showInFindPopup ? ScrollBars.Both : ScrollBars.None;
				int top = 0;
				if(!showInFindPopup) {
					if(!isMobileStyle) {
						top = WebApplication.Instance.ClientInfo.GetValue<int>("popupControlTop");
						if(top < 0) {
							int scrollPosition = WebApplication.Instance.ClientInfo.GetValue<int>("ScrollPosition");
							top = scrollPosition;
						}
					}
				}
				else {
					int clientHeight = WebApplication.Instance.ClientInfo.GetValue<int>("ClientHeight");
					int findPopupHeight = 497;
					top = (clientHeight - findPopupHeight) / 2;
					int scrollPosition = WebApplication.Instance.ClientInfo.GetValue<int>("ScrollPosition");
					if(scrollPosition > 0) {
						top += scrollPosition;
					}
				}
				popupControl.Top = top;
				popupControl.PopupAlignCorrection = PopupAlignCorrection.Disabled;
				popupControl.PopupHorizontalAlign = PopupHorizontalAlign.NotSet;
				popupControl.PopupVerticalAlign = PopupVerticalAlign.NotSet;
			}
		}
		public void CreatePopupControlMarkup(string url, bool isSizeable, bool showInFindPopup) {
			CreatePopupControlMarkup(url, isSizeable, showInFindPopup, "");
		}
		public void CreatePopupControlMarkup(string url, bool isSizeable, bool showInFindPopup, string closeScript) {
			string markup = string.Empty;
			ASPxPopupControl popupControl = CreatePopupControl(url, isSizeable, showInFindPopup, closeScript);
			WrapperPanel.Controls.Add(popupControl);
			if (WebWindow.CurrentRequestPage.IsCallback) {
				markup = DevExpress.Web.Internal.RenderUtils.GetRenderResult(popupControl);
			}
			callbackControl.JSProperties.Add("cpControlID", popupControl.UniqueID);
			callbackControl.JSProperties.Add("cpMarkup", markup);
			if (WebWindow.CurrentRequestPage.IsCallback) {
				Controls.Remove(WrapperPanel); 
			}
		}
		public XafPopupWindowControl() {
			wrapperPanel = new Panel() { ID = PopupControlWrapperID };
			Controls.Add(WrapperPanel);
			callbackControl = new ASPxCallback();
			callbackControl.ID = PopupWindowCallbackControlID;
			callbackControl.ClientInstanceName = PopupWindowCallbackControlID;
			Controls.Add(callbackControl);
		}
		public bool IsAutoHeightEnabled {
			get { return isAutoHeightEnabled; }
			set { isAutoHeightEnabled = value; }
		}
		public Panel WrapperPanel {
			get { return wrapperPanel; }
		}
		public event EventHandler<CustomizePopupControlEventArgs> CustomizePopupControl;
		public event EventHandler<CustomizePopupWindowSizeEventArgs> CustomizePopupWindowSize;
#if DebugTest
		public ASPxCallback DebugTest_CallbackControl {
			get { return callbackControl; }
		}
#endif
	}
	public class CustomizePopupWindowSizeEventArgs : HandledEventArgs {
		public CustomizePopupWindowSizeEventArgs(string contentUrl) {
			this.ContentUrl = contentUrl;
		}
		public Size Size { get; set; }
		public string ContentUrl { get; private set; }
		public PopupWindow FindPopupWindow(WebApplication application) {
			Guard.ArgumentNotNull(application, "application");
			Guard.ArgumentNotNull(application.PopupWindowManager, "application.PopupWindowManager");
			return application.PopupWindowManager.FindPopupWindowByContentUrl(ContentUrl);
		}
	}
	public class CustomizePopupControlEventArgs : EventArgs {
		private ASPxPopupControl popupControl;
		public CustomizePopupControlEventArgs(ASPxPopupControl popupControl) {
			this.popupControl = popupControl;
		}
		public ASPxPopupControl PopupControl {
			get { return popupControl; }
		}
	}
}
