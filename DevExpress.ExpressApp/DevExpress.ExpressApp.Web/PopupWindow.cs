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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Web {
	[Serializable]
	public sealed class PopupWindowLostException : ArgumentException {
		private PopupWindowLostException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public PopupWindowLostException(string message) : base(message) { }
	}
	public class PopupWindowManager : IDisposable, IXafCallbackHandler {
		public const string ShowPopupWindowScriptKey = "ShowPopupWindow";
		public const string ShowViewInNewWindowScriptKey = "ShowViewInNewWindow";
		public const string PopupWindowHandlerId = "PopupWindowHandler";
		private WebApplication application;
		private LightDictionary<string, WebWindow> windows = new LightDictionary<string, WebWindow>();
		private LightDictionary<PopupWindowShowAction, PopupWindow> activePopupWindows = new LightDictionary<PopupWindowShowAction, PopupWindow>();
		private LightDictionary<string, PopupWindowShowAction> activePopupWindowActions = new LightDictionary<string, PopupWindowShowAction>();
		private void RemoveWindow(WebWindow window) {
			window.Disposed -= new EventHandler(window_Disposed);
			windows.Remove(GetWindowId(window));
		}
		private void window_Disposed(object sender, EventArgs e) {
			RemoveWindow((WebWindow)sender);
		}
		private void RegisterPopupWindowAction(PopupWindowShowAction action) {
			Guard.ArgumentNotNull(action, "action");
			activePopupWindowActions[GetPopupWindowActionKey(action)] = action;
			action.Disposed -= new EventHandler(action_Disposed);
			action.Disposed += new EventHandler(action_Disposed);
		}
		private void RemoveActivePopupWindowAction(PopupWindowShowAction action) {
			Guard.ArgumentNotNull(action, "action");
			action.Disposed -= new EventHandler(action_Disposed);
			PopupWindow window = activePopupWindows[action];
			if(window != null) {
				RemoveActivePopupWindow(window);
				window.Dispose();
			}
			activePopupWindowActions.Remove(GetPopupWindowActionKey(action));
		}
		private void action_Disposed(object sender, EventArgs e) {
			RemoveActivePopupWindowAction((PopupWindowShowAction)sender);
		}
		private void AddActivePopupWindow(PopupWindowShowAction action, PopupWindow popupWindow) {
			popupWindow.Closed += new EventHandler<EventArgs>(popupWindow_Closed);
			popupWindow.Disposed += new EventHandler(popupWindow_Disposed);
			activePopupWindows.Add(action, popupWindow);
		}
		private void RemoveActivePopupWindow(PopupWindow popupWindow) {
			Guard.ArgumentNotNull(popupWindow, "popupWindow");
			popupWindow.Closed -= new EventHandler<EventArgs>(popupWindow_Closed);
			popupWindow.Disposed -= new EventHandler(popupWindow_Disposed);
			PopupWindowShowAction action = activePopupWindows.FindKeyByValue(popupWindow);
			if(action != null) {
				activePopupWindows.Remove(action);
			}
		}
		private void popupWindow_Closed(object sender, EventArgs e) {
			RemoveActivePopupWindow((PopupWindow)sender); 
		}
		private void popupWindow_Disposed(object sender, EventArgs e) {
			RemoveActivePopupWindow((PopupWindow)sender);
		}
		public PopupWindow FindRequestedWindowByActionId(string actionId) {
			PopupWindowShowAction action = activePopupWindowActions[actionId];
			if(action == null) {
				throw new PopupWindowLostException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotProcessTheRequestedUrlActionItemWasNotFound, actionId));
			}
			return GetPopupWindow(action);
		}
		private void RegisterCallbackHandler() {
			if(WebWindow.CurrentRequestPage is ICallbackManagerHolder) {
				((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager.RegisterHandler(PopupWindowHandlerId, this);
			}
		}
		protected string GetPopupWindowActionKey(PopupWindowShowAction action) {
			return action.GetHashCode().ToString();
		}
		public PopupWindow FindPopupWindowByContentUrl(string contentUrl) {
			if(contentUrl.Contains(DefaultHttpRequestManager.ActionIDKeyName + "=")) {
				string[] items = contentUrl.Split(new string[] { DefaultHttpRequestManager.ActionIDKeyName + "=" }, StringSplitOptions.None);
				if(items.Length == 2) {
					return FindRequestedWindowByActionId(items[1]);
				}
			}
			throw new ArgumentException(contentUrl, "contentUrl");
		}
		static PopupWindowManager() {
			ForceShowPopupWindowScriptValidation = true;
		}
		public PopupWindowManager(WebApplication application) {
			Guard.ArgumentNotNull(application, "application");
			this.application = application;
		}
		public void Dispose() {
			foreach(PopupWindow window in activePopupWindows.GetValues()) {
				RemoveActivePopupWindow(window);
			}
			foreach(PopupWindowShowAction action in activePopupWindowActions.GetValues()) {
				RemoveActivePopupWindowAction(action);
			}
		}
		public WebWindow FindRequestedWindow() {
			if(application == null) {
				throw new ObjectDisposedException(GetType().FullName);
			}
			if(application.MainWindow == null) {
				throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotProcessTheRequestedUrl));
			}
			string windowID = application.RequestManager.GetPopupWindowId();
			if(!string.IsNullOrEmpty(windowID)) {
				WebWindow result = windows[windowID];
				if(result == null) {
					throw new PopupWindowLostException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.WindowWasNotFound, windowID, string.Join(", ", windows.GetKeys().ToArray())));
				}
				return result;
			}
			string actionKey = application.RequestManager.GetPopupWindowShowActionId();
			if(!string.IsNullOrEmpty(actionKey)) {
				return FindRequestedWindowByActionId(actionKey);
			}
			return null;
		}
		public PopupWindow GetPopupWindow(PopupWindowShowAction action) {
			Guard.ArgumentNotNull(action, "action");
			if(action.IsDisposed) {
				throw new ObjectDisposedException(action.GetType().FullName);
			}
			PopupWindow window = activePopupWindows[action];
			if(window == null) {
				Tracing.Tracer.LogSubSeparator("Creating a popup window");
				if(HttpContext.Current != null) {
					string objectId = HttpContext.Current.Request.Params[PopupWindow.ObjectIdParamName];
					if(!String.IsNullOrEmpty(objectId)) {
						ObjectView view = action.SelectionContext as ObjectView;
						if(view != null) {
							IObjectSpace objectSpace = view.ObjectSpace;
							Type objectType = view.ObjectTypeInfo.Type;
							Object objectKey = objectSpace.GetObjectKey(objectType, objectId);
							view.CurrentObject = objectSpace.GetObjectByKey(objectType, objectKey);
						}
					}
				}
				CustomizePopupWindowParamsEventArgs args = action.GetPopupWindowParams();
				if(args.View == null) {
					throw new ArgumentNullException("CustomizePopupWindowParamsEventArgs.View", SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CustomizePopupWindowParamsEventArgsViewIsNull));
				}
				Tracing.Tracer.LogValue("args.ContextName", args.Context);
				Tracing.Tracer.LogValue("args.View.ID", args.View.Id);
				window = (PopupWindow)application.CreatePopupWindow(args.Context, args.View.Id, args.DialogController);
				RegisterWindow(window);
				AddActivePopupWindow(action, window);
				window.isSizeable = args.IsSizeable;
				window.SetView(args.View, null);
			}
			return window;
		}
		public void RegisterWindow(WebWindow window) {
			windows[GetWindowId(window)] = window;
			window.Disposed -= new EventHandler(window_Disposed);
			window.Disposed += new EventHandler(window_Disposed);
		}
		public void DestroyWindowById(string windowId) {
			WebWindow window = windows[windowId];
			if(window != null) {
				window.Close();
				window.Dispose();
			}
		}
		public WebWindow GetWindowById(string windowId) {
			return windows[windowId];
		}
		public static string GetWindowId(WebWindow window) {
			return window.GetHashCode().ToString();
		}
		[DefaultValue(true)]
		public static bool ForceShowPopupWindowScriptValidation { get; set; }
		public LightDictionary<PopupWindowShowAction, PopupWindow> ActivePopupWindows {
			get { return activePopupWindows; }
		}
		public LightDictionary<string, PopupWindowShowAction> ActivePopupWindowActions {
			get { return activePopupWindowActions; }
		}
		#region Processes request initiated by the script returned by the GetForceShowPopupWindowScript method
		void IXafCallbackHandler.ProcessAction(string parameter) {
			string[] parameters = parameter.Split('|');
			string callBackFuncName = parameters[0];
			bool forcePostBack = StringHelper.ParseToBool(parameters[1], false);
			string targetControlId = parameters[2];
			string url = parameters[3];
			bool isSizeable = StringHelper.ParseToBool(parameters[4], true);
			string actionId = parameters[5];
			bool showInFindPopup = StringHelper.ParseToBool(parameters[6], true);
			if(activePopupWindowActions[actionId] != null) {
				GetPopupWindow(activePopupWindowActions[actionId]);
			}
			ShowPopup(url, isSizeable, WebWindow.CurrentRequestWindow, callBackFuncName, forcePostBack, targetControlId, showInFindPopup);
		}
		#endregion
		#region Returns script that can be used to initiate request to show a popup window
		public string GetShowPopupWindowScript(PopupWindowShowAction action, string callBackFuncName, string targetControlId, bool forcePostBack, bool isSizeable) {
			return GetShowPopupWindowScript(action, callBackFuncName, targetControlId, forcePostBack, isSizeable, false, false);
		}
		public string GetShowPopupWindowScript(PopupWindowShowAction action, string callBackFuncName, string targetControlId, bool forcePostBack, bool isSizeable, bool useFindTemplate, bool showInFindPopup) {
			return GetShowPopupWindowScript(action, callBackFuncName, targetControlId, forcePostBack, isSizeable, useFindTemplate, showInFindPopup, "");
		}
		public string GetShowPopupWindowScript(PopupWindowShowAction action, string callBackFuncName, string targetControlId, bool forcePostBack, bool isSizeable, bool useFindTemplate, bool showInFindPopup, string endCallbackHandler) {
			RegisterPopupWindowAction(action);
			return GetShowPopupWindowScript(GetActionUrl(action, useFindTemplate), callBackFuncName, targetControlId, forcePostBack, isSizeable, GetPopupWindowActionKey(action), showInFindPopup, endCallbackHandler);
		}
		public string GetShowPopupWindowScript(string url, string callBackFuncName, string targetControlId, bool forcePostBack, bool isSizeable) {
			return GetShowPopupWindowScript(url, callBackFuncName, targetControlId, forcePostBack, isSizeable, "");
		}
		public string GetShowPopupWindowScript(string url, string callBackFuncName, string targetControlId, bool forcePostBack, bool isSizeable, string actionId) {
			return GetShowPopupWindowScript(url, callBackFuncName, targetControlId, forcePostBack, isSizeable, actionId, false, "");
		}
		public string GetShowPopupWindowScript(string url, string callBackFuncName, string targetControlId, bool forcePostBack, bool isSizeable, string actionId, bool showInFindPopup, string endCallbackHandler) {
			RegisterCallbackHandler();
			string parameter = string.Format("'{0}|{1}|{2}|{3}|{4}|{5}|{6}'", callBackFuncName, ClientSideEventsHelper.ToJSBoolean(forcePostBack), targetControlId, url, ClientSideEventsHelper.ToJSBoolean(isSizeable), actionId, ClientSideEventsHelper.ToJSBoolean(showInFindPopup));
			return ScriptGenerator.GetScript(PopupWindowHandlerId, parameter, ScriptGenerator.JSEmptyString, bool.FalseString.ToLower(), endCallbackHandler);
		}
		#endregion
		#region Shows popup right now
		public void ShowPopup(WebWindow windowToShow, WebWindow currentRequestWindow) {
			ShowPopup(windowToShow, true, currentRequestWindow, "", false, "");
		}
		public void ShowPopup(WebWindow windowToShow, WebWindow currentRequestWindow, bool showInFindPopup) {
			ShowPopup(windowToShow, currentRequestWindow, showInFindPopup, "");
		}
		public void ShowPopup(WebWindow windowToShow, WebWindow currentRequestWindow, bool showInFindPopup, string closeScript) {
			RegisterWindow(windowToShow);
			ShowPopup(GetWindowUrl(windowToShow, false), true, currentRequestWindow, "", false, "", showInFindPopup, closeScript);
		}
		public void ShowPopup(WebWindow windowToShow, bool isSizeable, WebWindow currentRequestWindow, string callBackFuncName, bool forcePostBack, string targetControlId) {
			RegisterWindow(windowToShow);
			ShowPopup(GetWindowUrl(windowToShow, false), isSizeable, currentRequestWindow, callBackFuncName, forcePostBack, targetControlId, false);
		}
		public void ShowPopup(PopupWindowShowAction action, string targetControlId) {
			ShowPopup(action, action.IsSizeable, WebWindow.CurrentRequestWindow, "", false, targetControlId);
		}
		public void ShowPopup(PopupWindowShowAction action, bool isSizeable, WebWindow webWindow, string callBackFuncName, bool forcePostBack, string targetControlId) {
			ShowPopup(action, isSizeable, webWindow, callBackFuncName, forcePostBack, targetControlId, false, false);
		}
		public void ShowPopup(PopupWindowShowAction action, bool isSizeable, WebWindow webWindow, string callBackFuncName, bool forcePostBack, string targetControlId, bool useFindTemplate, bool showInFindPopup) {
			RegisterPopupWindowAction(action);
			ShowPopup(GetActionUrl(action, useFindTemplate), isSizeable, webWindow, callBackFuncName, forcePostBack, targetControlId, showInFindPopup);
		}
		protected virtual void ShowPopup(string url, bool isSizeable, WebWindow webWindow, string callBackFuncName, bool forcePostBack, string targetControlId, bool showInFindPopup) {
			ShowPopup(url, isSizeable, webWindow, callBackFuncName, forcePostBack, targetControlId, showInFindPopup, "");
		}
		private void ShowPopup(string url, bool isSizeable, WebWindow webWindow, string callBackFuncName, bool forcePostBack, string targetControlId, bool showInFindPopup, string closeScript) {
			PreparePopupWindowControl(url, isSizeable, showInFindPopup, closeScript);
			RegisterPopupScript(webWindow, ShowPopupWindowScriptKey, callBackFuncName, forcePostBack, targetControlId);
		}
		private void PreparePopupWindowControl(string url, bool isSizeable, bool showInFindPopup, string closeScript) {
			if(WebWindow.CurrentRequestPage is IXafPopupWindowControlContainer) {
				XafPopupWindowControl xafPopupWindowControl = ((IXafPopupWindowControlContainer)WebWindow.CurrentRequestPage).XafPopupWindowControl;
				if(xafPopupWindowControl != null) {
					xafPopupWindowControl.CreatePopupControlMarkup(url, isSizeable, showInFindPopup, closeScript); 
				}
			}
		}
		private void RegisterPopupScript(WebWindow window, string scriptKey, string callBackFuncName, bool forcePostBack, string targetControlId) {
			if(WebWindow.CurrentRequestPage is IXafPopupWindowControlContainer) {
				XafPopupWindowControl xafPopupWindowControl = ((IXafPopupWindowControlContainer)WebWindow.CurrentRequestPage).XafPopupWindowControl;
				if(xafPopupWindowControl != null) {
					if(!string.IsNullOrEmpty(callBackFuncName)) {
						callBackFuncName = callBackFuncName.Replace("'", @"\'");
					}
					string script = string.Format("initializePopupWindow('{0}','{1}','{2}',{3},'{4}','{5}');", XafPopupWindowControl.PopupWindowCallbackControlID, xafPopupWindowControl.Parent.ClientID, callBackFuncName, forcePostBack.ToString().ToLower(), targetControlId, xafPopupWindowControl.WrapperPanel.ClientID);
					window.RegisterStartupScript(scriptKey, script);
				}
			}
		}
		#endregion
		private string GetWindowUrl(WebWindow window, bool useFindTemplate) {
			ViewShortcut shortcut = new ViewShortcut();
			if(window.View != null) {
				shortcut = window.View.CreateShortcut();
			}
			string result = GetUrlPrefix(useFindTemplate);
			return result + "&" + window.Application.RequestManager.GetPopupWindowQueryString(shortcut, GetWindowId(window));
		}
		private string GetActionUrl(PopupWindowShowAction action, bool useFindTemplate) {
			NameValueCollection parameters = new NameValueCollection();
			parameters.Add(DefaultHttpRequestManager.ActionIDKeyName, GetPopupWindowActionKey(action));
			string result = GetUrlPrefix(useFindTemplate);
			return result + "&" + UrlHelper.BuildQueryString(parameters);
		}
		private string GetUrlPrefix(bool useFindTemplate) {
			if(WebApplicationStyleManager.IsNewStyle && useFindTemplate && DeviceDetector.Instance.GetDeviceCategory() == DeviceCategory.Desktop) {
				return WebApplication.FindPopupWindowTemplatePage;
				}
				else {
				return WebApplication.PopupWindowTemplatePage;
			}
		}
		#region DebugTest
		public string DebugTest_GetWindowUrl(WebWindow window, bool useFindTemplate) {
			return GetWindowUrl(window, useFindTemplate);
		}
		public string DebugTest_GetActionUrl(PopupWindowShowAction action, bool useFindTemplate) {
			return GetActionUrl(action, useFindTemplate);
		}
		public string DebugTest_GetUrlPrefix(bool useFindTemplate) {
			return GetUrlPrefix(useFindTemplate);
		}
		#endregion
		#region Obsolete 15.1
		[Obsolete("Use the FindPopupWindowByContentUrl method instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PopupWindow FindPopupWindowByContenUrl(string contentUrl) {
			return FindPopupWindowByContentUrl(contentUrl);
		}
		[Obsolete("Use the GetShowPopupWindowScript method instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string GenerateOpeningScript(Control control, String url) {
			return GetShowPopupWindowScript(url, "", control.ClientID, false, false);
		}
		[Obsolete("Use the GetShowPopupWindowScript method instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public string GenerateOpeningScript(WebControl control, PopupWindowShowAction action, bool forcePostBack, NameValueCollection requestParams) {
			return GetShowPopupWindowScript(action, "", control.ClientID, forcePostBack, action.IsSizeable);
		}
		[Obsolete("Use the GetShowPopupWindowScript method instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string GenerateOpeningScript(WebControl control, PopupWindowShowAction action) {
			return GetShowPopupWindowScript(action, "", control.ClientID, false, action.IsSizeable);
		}
		[Obsolete("Use the GetShowPopupWindowScript method instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public string GenerateModalOpeningScript(WebControl control, PopupWindowShowAction action, int windowWidth, int windowHeight, bool forcePostBack, string callBackFuncName) {
			return GetShowPopupWindowScript(action, callBackFuncName, control.ClientID, forcePostBack, action.IsSizeable);
		}
		[Obsolete("Use the GetShowPopupWindowScript method instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public string GenerateModalOpeningScript(WebControl control, PopupWindowShowAction action, int windowWidth, int windowHeight, bool forcePostBack, string callBackFuncName, bool useFindTemplate, bool showInFindPopup) {
			return GetShowPopupWindowScript(action, callBackFuncName, control.ClientID, forcePostBack, action.IsSizeable, useFindTemplate, showInFindPopup);
		}
		[Obsolete("Use the GetShowPopupWindowScript method instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string GetOpeningScript(string callBackFuncName, bool forcePostBack, string targetControlId, string url, bool isSizeable) {
			return GetShowPopupWindowScript(url, callBackFuncName, targetControlId, forcePostBack, isSizeable);
		}
		[Obsolete("Use the ShowPopup method instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public void RegisterStartupWindowOpeningScript(WebWindow currentWindow, WebWindow targetPopupWindow, bool isModal) {
			ShowPopup(targetPopupWindow, currentWindow);
		}
		[Obsolete("Use the ShowPopup method instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public void RegisterStartupWindowOpeningScript(Page page, WebWindow currentWindow, WebWindow targetPopupWindow, bool isModal) {
			ShowPopup(targetPopupWindow, currentWindow);
		}
		[Obsolete("Use the ShowPopup method instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RegisterStartupPopupWindowShowActionScript(WebControl control, PopupWindowShowAction action) {
			ShowPopup(action, control.ClientID);
		}
		#endregion
	}
	public class PopupWindow : WebWindow {
		public const string DefaultClosingScript = "closeActiveXafPopupWindow();";
		private const string ClosingScriptTemplate = "if(xaf.ConfirmUnsavedChangedController){{xaf.ConfirmUnsavedChangedController.SetActive(false);}} closeXafPopupWindow('{0}');";
		private const string RefreshWindowParameter = "XafParentWindowRefresh";
		private bool isClosing;
		private string closureScript;
		internal bool isSizeable;
		private static int windowWidth = 800;
		private static int windowHeight = 300;
		private bool refreshParentWindowOnCloseButtonClick;
		private XafCallbackManager CallbackManager {
			get {
				ICallbackManagerHolder callbackManagerHolder = WebWindow.CurrentRequestPage as ICallbackManagerHolder;
				if(callbackManagerHolder != null) {
					return callbackManagerHolder.CallbackManager;
				}
				return null;
			}
		}
		internal static bool IsRefreshOnCallback {
			get {
				bool result = false;
				Page page = WebWindow.CurrentRequestPage;
				if(page != null && page.IsCallback) {
					result = page.Request.Form["__CALLBACKPARAM"].Contains(RefreshWindowParameter);
				}
				return result;
			}
		}
		private string GetRefreshWindowScript() {
			string result = "";
			if(CallbackManager != null) {
				if(WebApplicationStyleManager.IsNewStyle) {
					result = string.Format(@"window.dialogOpener.setTimeout(""WaitAnimateComplete(function(){{{0}}})"", 1);", CallbackManager.GetScript(string.Empty, "'" + RefreshWindowParameter + "'"));
				}
				else {
					result = string.Format(@"window.dialogOpener.setTimeout(""{0}"", 1);", CallbackManager.GetScript(string.Empty, "'" + RefreshWindowParameter + "'"));
				}
			}
			return result;
		}
		private string GetModalDialogClosureScriptBlock(bool isForceRefresh) {
			StringBuilder result = new StringBuilder();
			if(isForceRefresh) {
				if(!string.IsNullOrEmpty(ClosureScript)) {
					result.Append(ClosureScript);
				}
				result.Append(string.Format(@"
                try {{
                    if(window.dialogOpener != null) {{                        
                        if(window.dialogOpener.dialog.callbackFunc == null || window.dialogOpener.dialog.callbackFunc == '') {{
                            {0}    
                        }} else {{
                            if(window.dialogOpener.DoCallback != null) {{
                                var callbackCommand = ""DoCallback(\"""" + window.dialogOpener.dialog.callbackFunc + ""\"", "" + (window.dialogOpener.dialog.forcePostBack ? ""true"" : ""false"") + "", '"" + window.dialogOpener.dialog.targetControlId + ""')"";
                                window.dialogOpener.setTimeout(callbackCommand, 1);
                            }}
                        }}
                    }}
                }}
                catch(e) {{}}
                ", GetRefreshWindowScript()));
			}
			result.Append(GetCloseScript());
			return result.ToString();
		}
		private void WriteCommonScripts() {
			Stream stream = typeof(WebWindow).Assembly.GetManifestResourceStream("DevExpress.ExpressApp.Web.Resources.CommonFunctions.js");
			StreamReader reader = new StreamReader(stream, Encoding.UTF8);
			HttpContext.Current.Response.Output.Write("<script type=\"text/javascript\">");
			HttpContext.Current.Response.Output.Write(reader.ReadToEnd());
			HttpContext.Current.Response.Output.Write("</script>");
		}
		private void WriteClientScripts(bool isForceParentRefresh) {
			for(int i = ClientScripts.Count - 1; i >= 0; i--) {
				ClosureScript += ClientScripts[i];
				StartUpScripts.Remove(ClientScripts.GetKey(i));
			}
			ClosureScript += "\n";
			for(int i = StartUpScripts.Count - 1; i >= 0; i--) {
				ClosureScript += StartUpScripts[i];
				StartUpScripts.Remove(StartUpScripts.GetKey(i));
			}
			ClosureScript += "\n";
			string closureScript = "<script>" + GetModalDialogClosureScriptBlock(isForceParentRefresh) + "</script>";
			HttpContext.Current.Response.Output.Write(closureScript);
		}
		private void CloseOnPostBack(bool isForceParentRefresh) {
			HttpContext.Current.Response.ClearContent();
			WriteCommonScripts();
			WriteClientScripts(isForceParentRefresh);
			HttpContext.Current.Response.End();
		}
		private void CloseOnCallback(bool isForceParentRefresh) {
			if(!StartUpScripts.Keys.Contains("PopupWindowClosureScript")) {
				StartUpScripts.Add("PopupWindowClosureScript", GetModalDialogClosureScriptBlock(isForceParentRefresh));
			}
		}
		private string GetCloseScript() {
			if(WebApplicationStyleManager.IsNewStyle) {
				ICallbackManagerHolder callbackManagerHolder = WebWindow.CurrentRequestPage as ICallbackManagerHolder;
				if(callbackManagerHolder != null) {
					callbackManagerHolder.CallbackManager.CallbackControl.JSProperties["cpElementsToUpdate"] = "ScriptContainer;UPPopupWindowControl";
				}
			}
			return string.Format(ClosingScriptTemplate, this.GetHashCode());
		}
		private void RegisterPopupWindowId() {
			if(WebWindow.CurrentRequestWindow == this && !WebWindow.CurrentRequestPage.IsPostBack) {
				string script = string.Format("window.top.activePopupWindowID = '{0}';", this.GetHashCode());
				WebWindow.CurrentRequestPage.ClientScript.RegisterStartupScript(this.GetType(), "RegisterPopupWindowId", script, true);
			}
		}
		protected override void OnTemplateChanged() {
			if(Template != null) {
				Template.IsSizeable = isSizeable;
			}
			base.OnTemplateChanged();
			RegisterPopupWindowId();
		}
		protected virtual void OnClose() {
			if(Closed != null) {
				Closed(this, EventArgs.Empty);
			}
		}
		public PopupWindow(XafApplication application, TemplateContext context, ICollection<Controller> controllers)
			: base(application, context, controllers, false, true) {
		}
		public override bool Close(bool isForceParentRefresh) {
			if(!isClosing) {
				isClosing = true;
				try {
					base.Close(isForceParentRefresh);
					if(WebWindow.CurrentRequestPage != null && WebWindow.CurrentRequestPage.IsCallback) {
						CloseOnCallback(isForceParentRefresh);
					}
					else {
						CloseOnPostBack(isForceParentRefresh);
					}
				}
				finally {
					isClosing = false;
				}
				WebWindow.CurrentRequestPage.Unload += delegate(object sender, EventArgs e) {
					Dispose();
				};
				OnClose();
			}
			return true;
		}
		public static bool GetIsPostBackForced(Page page) {
			return page.Request["__EVENTARGUMENT"] == "forcePostBack";
		}
		public string ClosureScript {
			get { return closureScript; }
			set { closureScript = value; }
		}
		public static int WindowWidth {
			get { return windowWidth; }
			set { windowWidth = value; }
		}
		public static int WindowHeight {
			get { return windowHeight; }
			set { windowHeight = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool RefreshParentWindowOnCloseButtonClick {
			get { return refreshParentWindowOnCloseButtonClick; }
			set { refreshParentWindowOnCloseButtonClick = value; }
		}
		public const string ObjectIdParamName = "obj";
		public event EventHandler<EventArgs> Closed;
#if DebugTest
		public const string DebugTest_RefreshWindowParameter = RefreshWindowParameter;
#endif
	}
}
