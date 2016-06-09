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
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using DevExpress.Web.Internal;
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.CommonFunctions.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.MoveFooter.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.TemplateScripts.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.1x1.gif", "image/gif")]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.ActionContainers.ArrowDn.gif", "image/gif")]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.JScripts.DefaultVerticalTemplate.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.JScripts.PopupControllersManager.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.JScripts.PopupFrameController.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.JScripts.PopupScrollController.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.JScripts.ASPxClientMenuController.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.JScripts.PopupTemplate.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.JScripts.ConfirmController.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.JScripts.ShowPopupController.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.JScripts.XafFooter.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.JScripts.XafNavigation.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.JScripts.ControllersManager.js", "text/javascript", PerformSubstitution = true)]
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.JScripts.ClientSideValidation.js", "text/javascript", PerformSubstitution = true)]
namespace DevExpress.ExpressApp.Web {
	public class WebWindow : Window {
		class ASPxRegisterScriptHelper : RenderUtils {
			class HackASPxPopupControl : ASPxPopupControl {
				public void RegisterScripts() {
					RegisterIncludeScripts();
				}
			}
			public static void RegisterJQueryScript(Page page) {
				ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.JQueryScriptResourceName, true);
			}
			public static void RegisterGlobalizeScript(Page page) {
				ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.GlobalizeScriptResourceName, true);
			}
			public static void RegisterKnockoutScript(Page page) {
				ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.KnockoutScriptResourceName, true);
			}
			public static void RegisterDevExtremeCoreScript(Page page) {
				ResourceManager.RegisterScriptResource(page, typeof(RenderUtils), RenderUtils.DevExtremeCoreScriptResourceName, true);
			}
			public static void RegisterASPxPopupControlScripts() {
				new HackASPxPopupControl().RegisterScripts();
			}
		}
		private readonly object lockObject = new object();
		private const string scrollerId = "Sc";
		private const string sessionKeepAliveControlId = "SKA";
		private bool isClosing;
		private static bool isRedirecting;
		private ScrollControl scrollControl;
		private LightDictionary<string, string> clientScripts = new LightDictionary<string, string>();
		private LightDictionary<string, string> startUpScripts = new LightDictionary<string, string>();
		private LightDictionary<string, string> clientIncludeScripts = new LightDictionary<string, string>();
		private LightDictionary<string, Type> clientScriptResources = new LightDictionary<string, Type>();
		internal protected LightDictionary<string, string> StartUpScripts {
			get { return startUpScripts; }
		}
		protected LightDictionary<string, string> ClientScripts {
			get { return clientScripts; }
		}
		private ICallbackManagerHolder GetCallbackManagerHolder(Page page) {
			Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), page.GetType(), "Page");
			return (ICallbackManagerHolder)page;
		}
		internal void RaisePagePreRender() {
			Tracing.Tracer.LogVerboseText("-> RaisePagePreRender");
			if(PagePreRender != null) {
				PagePreRender(this, EventArgs.Empty);
			}
			Tracing.Tracer.LogVerboseText("<- RaisePagePreRender");
		}
		private void page_PreRender(object sender, EventArgs e) {
			Page page = (Page)sender;
			string message = "page_PreRender: " + Environment.NewLine;
			message += Tracing.Tracer.GetMessageByValue("page", page, true);
			Tracing.Tracer.LogVerboseText(message);
			lock(lockObject) {
				Tracing.Tracer.LogVerboseValue("isLocked", false);
				if(page == null) {
					return;
				}
				page.PreRender -= new EventHandler(page_PreRender);
				HttpRequest request = page.Request;
				Tracing.Tracer.LogVerboseValue("request", request);
				if(request == null) {
					return;
				}
				HttpResponse response = page.Response;
				Tracing.Tracer.LogVerboseValue("response", response);
				if(response == null) {
					return;
				}
				Tracing.Tracer.LogVerboseValue("request.RawUrl", request.RawUrl);
				if(HasTemplate) {
					RaisePagePreRender();
				}
				CheckForRedirectCore(request, false);
				Tracing.Tracer.LogVerboseValue("page.ClientScript", page.ClientScript);
				foreach(string scriptKey in startUpScripts.GetKeys()) {
					page.ClientScript.RegisterStartupScript(GetType(), scriptKey, startUpScripts[scriptKey], true);
				}
				startUpScripts.Clear();
				Tracing.Tracer.LogVerboseText("startUpScripts were registered");
				foreach(string scriptKey in clientIncludeScripts.GetKeys()) {
					page.ClientScript.RegisterClientScriptInclude(GetType(), scriptKey, clientIncludeScripts[scriptKey]);
				}
				clientIncludeScripts.Clear();
				Tracing.Tracer.LogVerboseText("clientIncludeScripts were registered");
				foreach(string scriptKey in clientScriptResources.GetKeys()) {
					page.ClientScript.RegisterClientScriptResource(clientScriptResources[scriptKey], scriptKey);
				}
				Tracing.Tracer.LogVerboseText("clientScriptResources were registered");
				foreach(string scriptKey in clientScripts.GetKeys()) {
					page.ClientScript.RegisterClientScriptBlock(GetType(), scriptKey, clientScripts[scriptKey], true);
				}
				clientScripts.Clear();
				if(ProcessPreRenderCompleted != null) {
					ProcessPreRenderCompleted(this, EventArgs.Empty);
				}
				message = "clientScripts were registered" + Environment.NewLine + "page_PreRender is completed" + Environment.NewLine;
				Tracing.Tracer.LogVerboseText(message);
			}
			UpdateScrollPosition();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler ProcessPreRenderCompleted;
		private void CheckForRedirectCore(HttpRequest request, bool callbackOnly) {
			if(isRedirecting || callbackOnly && (CurrentRequestPage == null || !CurrentRequestPage.IsCallback)) {
				return;
			}
			if(request != null) {
				string redirectUrl = string.Empty;
				if(!HasTemplate) {
					Tracing.Tracer.LogVerboseText("Template is null, redirecting to: " + request.RawUrl);
					redirectUrl = request.RawUrl;
				}
				if(!string.IsNullOrEmpty(redirectUrl)) {
					bool useHttpLocationHeader = CurrentRequestPage != null && CurrentRequestPage.IsCallback;
					isRedirecting = true;
					WebApplication.Redirect(redirectUrl, !useHttpLocationHeader, useHttpLocationHeader);
				}
			}
		}
		private void page_Unload(object sender, EventArgs e) {
			Tracing.Tracer.LogVerboseText("page_Unload");
			BreakLinksToControls();
		}
		private void scrollControl_ScrollPositionChanged(object sender, ScrollPositionChangedEventArgs e) {
			if(View != null) {
				View.ScrollPosition = e.CurrentScrollPosition;
			}
		}
		private void SetScrollPosition(Point scrollPosition) {
			ScrollControl.CurrentScrollPosition = scrollPosition;
		}
		private bool IsStartPageRequest(Page page, ViewShortcut currentRequestShortcut) {
			if(page.IsPostBack ||
				HttpContext.Current.Session.IsNewSession ||
				currentRequestShortcut != ViewShortcut.Empty ||
				GetType() == typeof(PopupWindow) ||
				((View is ObjectView) && View.ObjectSpace.IsNewObject(((ObjectView)View).CurrentObject))) {
				return false;
			}
			return true;
		}
		private void CreateNotifyWindowCloseControl(Page page) {
			ASPxCallback callbackControl = new ASPxCallback();
			callbackControl.ID = "NotifyWindowCloseControl";
			callbackControl.JSProperties.Add("cpServerWindowID", PopupWindowManager.GetWindowId(this));
			callbackControl.ClientInstanceName = "NotifyWindowCloseControl";
			callbackControl.ClientSideEvents.EndCallback = "function(s) {if (s.cpStartupScripts) { setTimeout(s.cpStartupScripts, 0); } }";
			callbackControl.Callback += new CallbackEventHandler(delegate(object source, CallbackEventArgs e) {
				WebWindow window = ((WebApplication)Application).PopupWindowManager.GetWindowById(e.Parameter);
				if(window is PopupWindow && ((PopupWindow)window).RefreshParentWindowOnCloseButtonClick) {
					string script = string.Format("window.requestInProgress = false; {0}", ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager.GetScript());
					callbackControl.JSProperties["cpStartupScripts"] = script;
				}
				((WebApplication)Application).PopupWindowManager.DestroyWindowById(e.Parameter);
			});
			page.Form.Controls.Add(callbackControl);
		}
		private void UpdateScrollPosition() {
			if(View != null) {
				SetScrollPosition(View.ScrollPosition);
			}
		}
		private void CallbackManager_PreRender(object sender, EventArgs e) {
			XafCallbackManager callbackManager = (XafCallbackManager)sender;
			if(CurrentRequestPage != null && CurrentRequestPage.IsCallback) {
				string traceMessage = "CallbackManager_CallbackCompleted:";
				traceMessage += Tracing.Tracer.GetMessageByValue("callbackManager", callbackManager, true);
				Tracing.Tracer.LogVerboseText(traceMessage);
				lock(lockObject) {
					Tracing.Tracer.LogVerboseValue("isLocked", false);
					callbackManager.PreRender -= new EventHandler<EventArgs>(CallbackManager_PreRender);
					if(HasTemplate) {
						RaisePagePreRender();
					}
					foreach(string scriptKey in startUpScripts.GetKeys()) {
						callbackManager.RegisterClientScript(scriptKey, startUpScripts[scriptKey]);
					}
					startUpScripts.Clear();
					Tracing.Tracer.LogVerboseText("startUpScripts were registered");
					foreach(string scriptKey in clientScripts.GetKeys()) {
						callbackManager.RegisterClientScript(scriptKey, clientScripts[scriptKey]);
					}
					clientScripts.Clear();
					Tracing.Tracer.LogVerboseText("clientScripts were registered");
					foreach(string scriptKey in clientIncludeScripts.GetKeys()) {
						callbackManager.RegisterClientScriptInclude(scriptKey, clientIncludeScripts[scriptKey]);
					}
					clientIncludeScripts.Clear();
					Tracing.Tracer.LogVerboseText("clientIncludeScripts were registered");
					foreach(string scriptKey in clientScriptResources.GetKeys()) {
						callbackManager.RegisterClientScriptResource(clientScriptResources[scriptKey], scriptKey);
					}
					Tracing.Tracer.LogVerboseText("clientScriptResources were registered" + Environment.NewLine + "CallbackManager_CallbackCompleted is completed");
				}
				UpdateScrollPosition();
			}
		}
		private void OnCustomRegisterTemplateDependentScripts(CustomRegisterTemplateDependentScriptsEventArgs args) {
			if(CustomRegisterTemplateDependentScripts != null) {
				CustomRegisterTemplateDependentScripts(this, args);
			}
		}
		private void AddBrowserIsIncompattibleText() {
			ErrorHandling.Instance.SetPageError(new InvalidOperationException(CaptionHelper.GetLocalizedText("Messages", "BrowserIsIncompatible")));
		}
		protected virtual void CreateSystemControlsCore(Page page) {
			CreateScrollControl(page);
			CreateSessionKeepAliveControl(page);
			CreateNotifyWindowCloseControl(page);
		}
		public virtual void CreateControls(Page page, bool checkRequestParams) {
			string traceMessage = Tracing.Tracer.GetSeparator("->  WebWindow.CreateControls");
			if(IsDisposed) {
				try {
					traceMessage += Tracing.Tracer.GetMessageByValue("page", page, true);
					traceMessage += Tracing.Tracer.GetMessageByValue("RawUrl", page.Request.RawUrl, true);
					traceMessage += Tracing.Tracer.GetMessageByValue("CurrentShortcut", GetCurrentShortcut(), true);
					Tracing.Tracer.LogText(traceMessage);
				}
				catch(Exception) {
				}
				throw new ObjectDisposedException(GetType().FullName);
			}
			traceMessage += Tracing.EmptyHeader + "ControlsCreating" + Environment.NewLine;
			traceMessage += Tracing.Tracer.GetMessageByValue("page.IsPostBack", page.IsPostBack, true);
			traceMessage += Tracing.Tracer.GetMessageByValue("page.IsCallback", page.IsCallback, true);
			traceMessage += Tracing.Tracer.GetMessageByValue("Session is created with current request", HttpContext.Current.Session.IsNewSession, true);
			Tracing.Tracer.LogText(traceMessage);
			if(checkRequestParams && ControlsCreating != null) {
				ControlsCreating(page, EventArgs.Empty);
			}
			((WebApplication)Application).InitializeCulture();
			RegisterCommonScripts(page);
			Tracing.Tracer.LogVerboseValue("checkRequestParams", checkRequestParams);
			if(Template != null && Template != page) {
				ClearTemplate();
			}
			CreateSystemControlsCore(page);
			Tracing.Tracer.LogVerboseValue("currentShortcut", GetCurrentShortcut());
			if(checkRequestParams) {
				if(page.IsPostBack && HttpContext.Current.Session.IsNewSession) {
					WebApplication.Redirect(HttpContext.Current.Request.RawUrl);
				}
			}
			page.PreRender += new EventHandler(page_PreRender);
			GetCallbackManagerHolder(page).CallbackManager.PreRender += new EventHandler<EventArgs>(CallbackManager_PreRender);
			page.Unload += new EventHandler(page_Unload);
			WebWindow.SetCurrentRequestWindow(this);
			traceMessage = Tracing.Tracer.GetMessageByValue("page", page, false);
			traceMessage += Tracing.Tracer.GetMessageByValue("page is IFrameTemplate", page is IFrameTemplate, true);
			Tracing.Tracer.LogText(traceMessage);
			if(page is IFrameTemplate) {
				Tracing.Tracer.LogVerboseValue("Template", page.Request.RawUrl);
				((WebApplication)Application).RaiseCustomizeTemplate((IFrameTemplate)page, Context.Name);
				SetTemplate((IFrameTemplate)page);
			}
			else {
				CreateTemplate();
				Tracing.Tracer.LogVerboseValue("Template", Template);
				GetForm(page).Controls.Add((Control)Template);
			}
			page.ClientScript.GetPostBackEventReference(page, "");
			HttpContext.Current.Response.Cache.SetExpires(DateTime.Now);
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			HttpContext.Current.Response.Cache.SetNoStore();
			if(WebApplicationStyleManager.IsNewStyle) {
				BrowserInfo browserInfo = RenderUtils.Browser;
				if(browserInfo.IsIE && browserInfo.Version < 9) {
					AddBrowserIsIncompattibleText();
				}
			}
			Tracing.Tracer.LogVerboseText("<-  WebWindow.CreateControls");
		}
		protected ViewShortcut GetCurrentShortcut() {
			if(View != null) {
				return View.CreateShortcut();
			}
			else {
				return ViewShortcut.Empty;
			}
		}
		public override bool Close(bool isForceRefresh) {
			if(isClosing) {
				return false;
			}
			isClosing = true;
			try {
				return SetView(null);
			}
			finally {
				isClosing = false;
			}
		}
		public WebWindow(XafApplication application, TemplateContext context, ICollection<Controller> controllers, bool isMain, bool activateControllersImmediatelly)
			: base(application, context, controllers, isMain, activateControllersImmediatelly) {
		}
		public void CreateControls(Page page) {
			CreateControls(page, true);
		}
		public void BreakLinksToControls() {
			Page page = WebWindow.CurrentRequestPage;
			if(page != null) {
				page.PreRender -= new EventHandler(page_PreRender);
				page.Unload -= new EventHandler(page_Unload);
				GetCallbackManagerHolder(page).CallbackManager.PreRender -= new EventHandler<EventArgs>(CallbackManager_PreRender);
			}
			if(View != null) {
				View.BreakLinksToControls();
			}
			ClearTemplate();
			if(scrollControl != null) {
				scrollControl.ScrollPositionChanged -= new EventHandler<ScrollPositionChangedEventArgs>(scrollControl_ScrollPositionChanged);
				scrollControl = null;
			}
			WebWindow.SetCurrentRequestPage(null);
		}
		public void RegisterStartupScript(string scriptName, string script, bool overwrite) {
			if(overwrite && startUpScripts.ContainsKey(scriptName)) {
				startUpScripts.Remove(scriptName);
			}
			if(!startUpScripts.ContainsKey(scriptName)) {
				startUpScripts.Add(scriptName, script);
			}
		}
		public void RegisterStartupScript(string scriptName, string script) {
			RegisterStartupScript(scriptName, script, false);
		}
		public void RegisterClientScript(string scriptName, string script, bool overrideScript) {
			if(overrideScript) {
				clientScripts[scriptName] = script;
			}
			else {
				if(!clientScripts.ContainsKey(scriptName)) {
					clientScripts.Add(scriptName, script);
				}
			}
		}
		public void RegisterClientScript(string scriptName, string script) {
			RegisterClientScript(scriptName, script, false);
		}
		public void RegisterClientScriptInclude(string scriptName, string scriptFileName) {
			clientIncludeScripts[scriptName] = scriptFileName;
		}
		public void RegisterClientScriptResource(Type type, string resourceName) {
			if(!clientScriptResources.ContainsKey(resourceName)) {
				clientScriptResources.Add(resourceName, type);
			}
		}
		public void CreateScrollControl(Page page) {
			scrollControl = new ScrollControl(page);
			scrollControl.ID = scrollerId;
			scrollControl.ScrollPositionChanged += new EventHandler<ScrollPositionChangedEventArgs>(scrollControl_ScrollPositionChanged);
			page.Form.Controls.Add(scrollControl);
		}
		public void CreateSessionKeepAliveControl(Page page) {
			SessionKeepAliveControl sessionKeepAliveControl = new SessionKeepAliveControl();
			sessionKeepAliveControl.ID = sessionKeepAliveControlId;
			page.Form.Controls.Add(sessionKeepAliveControl);
		}
		public event EventHandler PagePreRender;
		public event EventHandler ControlsCreating;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetCurrentRequestWindow(WebWindow window) {
			if(HttpContext.Current != null) {
				HttpContext.Current.Session["XAF_CurrentRequestWindow"] = window;
			}
		}
		protected internal static void SetCurrentRequestPage(Page page) {
			if(HttpContext.Current != null) {
				HttpContext.Current.Session["XAF_CurrentRequestPage"] = page;
			}
			isRedirecting = false;
		}
		public static HtmlForm GetForm(Page page) {
			HtmlForm result = null;
			foreach(Control control in page.Controls) {
				if(control is HtmlForm) {
					result = (HtmlForm)control;
					break;
				}
			}
			if(result == null) {
				throw new ArgumentException("The passed Page should contain " +
					"the HtmlForm object within the Controls collection");
			}
			return result;
		}
		protected virtual void RegisterCommonScripts(Page page) {
			page.ClientScript.RegisterClientScriptResource(typeof(WebWindow), "DevExpress.ExpressApp.Web.Resources.CommonFunctions.js");
			page.ClientScript.RegisterClientScriptResource(typeof(WebWindow), "DevExpress.ExpressApp.Web.Resources.JScripts.ControllersManager.js");
			page.ClientScript.RegisterClientScriptResource(typeof(WebWindow), "DevExpress.ExpressApp.Web.Resources.JScripts.ClientSideValidation.js");
			CustomRegisterTemplateDependentScriptsEventArgs args = new CustomRegisterTemplateDependentScriptsEventArgs(page);
			OnCustomRegisterTemplateDependentScripts(args);
			if(!args.Handled) {
				ASPxRegisterScriptHelper.RegisterJQueryScript(page);
				ASPxRegisterScriptHelper.RegisterASPxPopupControlScripts();
				page.ClientScript.RegisterClientScriptResource(typeof(WebWindow), "DevExpress.ExpressApp.Web.Resources.JScripts.ConfirmController.js");
				if(WebApplicationStyleManager.IsNewStyle) {
					ASPxRegisterScriptHelper.RegisterGlobalizeScript(page);
					ASPxRegisterScriptHelper.RegisterKnockoutScript(page);
					ASPxRegisterScriptHelper.RegisterDevExtremeCoreScript(page);
					page.ClientScript.RegisterClientScriptResource(typeof(WebWindow), "DevExpress.ExpressApp.Web.Resources.JScripts.PopupControllersManager.js");
					page.ClientScript.RegisterClientScriptResource(typeof(WebWindow), "DevExpress.ExpressApp.Web.Resources.JScripts.PopupFrameController.js");
					page.ClientScript.RegisterClientScriptResource(typeof(WebWindow), "DevExpress.ExpressApp.Web.Resources.JScripts.PopupScrollController.js");
					page.ClientScript.RegisterClientScriptResource(typeof(WebWindow), "DevExpress.ExpressApp.Web.Resources.JScripts.ShowPopupController.js");
					page.ClientScript.RegisterClientScriptResource(typeof(WebWindow), "DevExpress.ExpressApp.Web.Resources.JScripts.ASPxClientMenuController.js");
				}
				else {
					page.ClientScript.RegisterClientScriptResource(typeof(WebWindow), "DevExpress.ExpressApp.Web.Resources.TemplateScripts.js");
					page.ClientScript.RegisterClientScriptResource(typeof(WebWindow), "DevExpress.ExpressApp.Web.Resources.MoveFooter.js");
				}
			}
		}
		public static void CheckForRedirect() {
			CheckForRedirect(false);
		}
		public static void CheckForRedirect(bool callbackOnly) {
			if(CurrentRequestWindow != null) {
				CurrentRequestWindow.CheckForRedirectCore(HttpContext.Current.Request, callbackOnly);
			}
		}
		public static Page CurrentRequestPage {
			get {
				if(HttpContext.Current != null && HttpContext.Current.Session != null) {
					return (Page)HttpContext.Current.Session["XAF_CurrentRequestPage"];
				}
				else {
					return null;
				}
			}
		}
		public static WebWindow CurrentRequestWindow {
			get {
				if(HttpContext.Current != null) {
					return (WebWindow)HttpContext.Current.Session["XAF_CurrentRequestWindow"];
				}
				else {
					return null;
				}
			}
		}
		public ScrollControl ScrollControl {
			get { return scrollControl; }
			set { scrollControl = value; }
		}
		public new WebApplication Application {
			get { return (WebApplication)base.Application; }
		}
#if DebugTest
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void DebugTest_SetCurrentRequestPage(Page page) {
			SetCurrentRequestPage(page);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void DebugTest_CreateControls(Page page, bool checkRequestParams) {
			CreateControls(page, checkRequestParams);
		}
		public LightDictionary<string, string> DebugTest_StartUpScripts {
			get { return StartUpScripts; }
		}
#endif
		public event EventHandler<CustomRegisterTemplateDependentScriptsEventArgs> CustomRegisterTemplateDependentScripts;
		#region Obsolete 15.1
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ViewShortcut GetCurrentRequestShortcut() {
			if(WebApplication.Instance != null) {
				return WebApplication.Instance.RequestManager.GetViewShortcut();
			}
			else {
				return ViewShortcut.Empty;
			}
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected bool UseCurrentShortcut { get; set; }
		#endregion
	}
	public class CustomRegisterTemplateDependentScriptsEventArgs : HandledEventArgs {
		private Page page;
		public CustomRegisterTemplateDependentScriptsEventArgs(Page page) {
			this.page = page;
		}
		public Page Page {
			get { return page; }
		}
		#region Obsolete 15.1
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool RequiredClientScriptsRegistration { get; set; }
		#endregion
	}
}
