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
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.ExpressApp.Web.Localization;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security;
namespace DevExpress.ExpressApp.Web {
	[ToolboxItemFilter("Xaf.Platform.Web")]
	[ToolboxBitmap(typeof(WebApplication), "Resources.Toolbox_XAFApplication_Web.ico")]
	public class WebApplication : XafApplication {
		private ClientInfo clientInfo = null;
		public ClientInfo ClientInfo {
			get {
				if(clientInfo == null) {
					clientInfo = new ClientInfo();
				}
				return clientInfo;
			}
		}
		public void SwitchToNewStyle() {
			WebApplicationStyleManager.SwitchToNewStyle(true);
		}
		public void SwitchToOldStyle() {
			WebApplicationStyleManager.SwitchToNewStyle(false);
		}
		public const string SessionApplicationVariable = "SessionApplicationVariable";
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string PopupWindowTemplatePage = "Default.aspx?Dialog=true";
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string DialogKeyName = "dialog";
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string FindPopupWindowTemplatePage = "Default.aspx?FindPopup=true";
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string FindDialogKeyName = "findpopup";
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string DefaultPage = "Default.aspx";
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string LogonPage = "Login.aspx";
		private static readonly object sharedAppLocker = new object();
		static WebApplication() {
			ModelMultipleMasterStore.Instance = new WebModelMultipleMasterStore();
			Tracing.NeedContextInformation += new EventHandler<NeedContextInformationEventArgs>(Tracing_NeedContextInformation);
			CaptionHelper.RemoveAcceleratorSymbol = true;
			ObjectKeyHelper.SetIntance(new ObjectKeyHelperWeb());
			DefaultResourceReader.Active = new WebDefaultResourceReader();
			DevExpress.Web.Internal.RenderUtils.RaiseChangedOnLoadPostData = true;
			ImageResourceHttpHandler.AddImageLoaderHandler();
			XafHttpHandler.RegisterHandler(new SessionKeepAliveReconnectHttpHandler());
			XafHttpHandler.RegisterHandler(new TestScriptsManager());
			XafHttpHandler.RegisterHandler(new ImageResourceHttpHandler());
			XafHttpHandler.RegisterHandler(new BinaryDataHttpHandler());
		}
		private const string SessionManagerVariable = "SessionManagerVariable";
		private TemplateContentSettings settings = new TemplateContentSettings();
		private int logonAttemptCount = 0;
		private ApplicationStatisticObject applicationStatisticObject;
		private WebLogonController logonController;
		private WebWindow mainWindow;
		private IHttpRequestManager httpRequestManager;
		private void StartupAction_Executed(object sender, ActionBaseEventArgs e) {
			startUpActions.Remove((PopupWindowShowAction)sender);
			PopupWindow popupWindow = PopupWindowManager.ActivePopupWindows[(PopupWindowShowAction)sender];
			if(popupWindow != null) {
				popupWindow.Close();
			}
			if(startUpActions.Count == 0) {
				Redirect(DefaultPage);
			}
			else {
				Redirect(PopupWindowTemplatePage);
			}
		}
		private List<PopupWindowShowAction> startUpActions;
		private WebWindow GetRequestedWindow(Page page) {
			if(startUpActions.Count > 0) {
				return GetStartupActionWindow();
			}
			if(mainWindow == null) {
				ShowStartupWindow(page);
				return mainWindow;
			}
			WebWindow popupWindow = PopupWindowManager.FindRequestedWindow();
			if(popupWindow != null) {
				return popupWindow;
			}
			return mainWindow;
		}
		private WebWindow logonWindow;
		private PopupWindowManager popupWindowManager;
		private IFrameTemplateFactory frameTemplateFactory;
		private bool shouldGoToFormsRedirectURL = true;
		private bool canAutomaticallyLogonWithStoredLogonParameters = false;
		private bool canCreatePersistentCookie = false;
		private ViewEditMode? collectionsEditMode;
		private static void Tracing_NeedContextInformation(object sender, NeedContextInformationEventArgs e) {
			if(System.Web.HttpContext.Current != null) {
				if(System.Web.HttpContext.Current.Session != null) {
					e.ContextInformation = System.Web.HttpContext.Current.Session.SessionID;
				}
			}
			if(string.IsNullOrEmpty(e.ContextInformation)) {
				e.ContextInformation = sessionIdOnDisposing;
			}
			if(string.IsNullOrEmpty(e.ContextInformation)) {
				e.ContextInformation = "no context";
			}
		}
		private string GetRequestCultureName() {
			string cultureName = "";
			if((HttpContext.Current != null)
				&& (HttpContext.Current.Request != null)
				&& (HttpContext.Current.Request.UserLanguages != null)
				&& (HttpContext.Current.Request.UserLanguages.Length > 0)) {
				cultureName = HttpContext.Current.Request.UserLanguages[0].Replace('_', '-');
			}
			return cultureName;
		}
		private PopupWindow GetStartupActionWindow() {
			PopupWindow result = null;
			if(startUpActions != null && startUpActions.Count > 0) {
				result = PopupWindowManager.GetPopupWindow(startUpActions[0]);
				if(result.GetController<DialogController>() != null) {
					result.GetController<DialogController>().CanCloseWindow = false;
				}
			}
			return result;
		}
		protected internal bool isInitialized = false;
		protected override bool SupportMasterDetailMode {
			get { return false; }
		}
		internal ModelStoreBase UserDiffsStore { get { return base.CreateUserModelDifferenceStore(); } }  
		protected static bool GetIsCallback() {
			return RequestHelper.IsCallback;
		}
		protected internal void RaiseCustomizeTemplate(IFrameTemplate frameTemplate, String templateContextName) {
			OnCustomizeTemplate(frameTemplate, templateContextName);
		}
		protected void ShowStartupWindow(Page page) {
			WebWindow.SetCurrentRequestPage(page);
			CreateMainWindow();
			if(StartupWindowCreated != null) {
				StartupWindowCreated(this, EventArgs.Empty);
			}
		}
		private void CreateMainWindow() {
			mainWindow = (WebWindow)CreateWindow(TemplateContext.ApplicationWindow, null, true, true);
			PopupWindowManager.RegisterWindow(mainWindow);
		}
		protected override string GetTraceLogDirectory() {
			string outputDirectory = null;
			switch(GetFileLocation<FileLocation>(FileLocation.ApplicationFolder, TraceLogLocationKey)) {
				case FileLocation.ApplicationFolder:
					outputDirectory = PathHelper.GetApplicationFolder();
					break;
				default:
					break;
			}
			return outputDirectory;
		}
		protected override void OnLoggedOn(LogonEventArgs args) {
			logonAttemptCount = 0;
			WriteSecuredLogonParameters();
			Tracing.Tracer.LogValue("CurrentUserName", SecuritySystem.CurrentUserName);
			applicationStatisticObject.UserName = SecuritySystem.CurrentUserName;
			base.OnLoggedOn(args);
			ProcessFormsAuthenticationRedirectUrl(false);
			WebApplicationStyleManager.LockSwitchNewStyle = true;
		}
		protected void ProcessFormsAuthenticationRedirectUrl(bool endResponse) {
			if(shouldGoToFormsRedirectURL && HttpContext.Current != null &&
				!string.IsNullOrEmpty(FormsAuthentication.GetRedirectUrl(SecuritySystem.CurrentUserName, false)) &&
				string.Compare(FormsAuthentication.LoginUrl, HttpContext.Current.Request.Path, true) == 0) {
				Redirect(FormsAuthentication.GetRedirectUrl(SecuritySystem.CurrentUserName, false), endResponse);
			}
		}
		protected override bool OnLogonFailed(object logonParameters, Exception e) {
			DropModel();
			logonAttemptCount++;
			bool result = base.OnLogonFailed(logonParameters, e);
			if(!result) {
				if(logonAttemptCount >= MaxLogonAttemptCount) {
					if(logonWindow != null) {
						WebLogonController controller = logonWindow.GetController<WebLogonController>();
						if(controller != null) {
							controller.IsLogonEnabled = false;
							controller.CanCloseWindow = false;
							logonWindow.SetView(CreateDetailView(new NonPersistentObjectSpace(TypesInfo), ModelNodeIdHelper.GetDetailViewId(typeof(LogonAttemptsAmountedToLimit)), true));
							Redirect(HttpContext.Current.Request.RawUrl, true);
							return true;
						}
					}
				}
				if(!SecuritySystem.Instance.NeedLogonParameters) {
					if(logonWindow == null) {
						logonWindow = new WebWindow(this, "", new Controller[] { new ParameterlessLogonFailedInfoViewController() }, false, true);
					}
					if(logonWindow.View == null || logonWindow.View.Id != ParameterlessLogonFailedInfo.DetailViewId) {
						DetailView logonFailedView = CreateDetailView(new NonPersistentObjectSpace(TypesInfo), ParameterlessLogonFailedInfo.DetailViewId, true);
						logonFailedView.CurrentObject = new ParameterlessLogonFailedInfo(e.Message);
						logonWindow.SetView(logonFailedView);
					}
					return true;
				}
			}
			return result;
		}
		protected override void OnSetupComplete() {
			base.OnSetupComplete();
			isInitialized = true;
			InitializeCulture();
		}
		protected override bool OnHandleShortcutProcessingException(ViewShortcut shortcut, Exception shortcutProcessingException) {
			if(base.OnHandleShortcutProcessingException(shortcut, shortcutProcessingException)) {
				return true;
			}
			if(shortcutProcessingException != null && shortcutProcessingException is UserFriendlyException) {
				ErrorHandling.Instance.SetPageError(shortcutProcessingException);
				return true;
			}
			return false;
		}
		protected override LogonController CreateLogonController() {
			if(logonController == null) {
				logonController = CreateController<WebLogonController>();
			}
			return logonController;
		}
		protected override Window CreateWindowCore(TemplateContext context, ICollection<Controller> controllers, bool isMain, bool activateControllersImmediatelly) {
			return new WebWindow(this, context, controllers, isMain, activateControllersImmediatelly);
		}
		protected override Window CreatePopupWindowCore(TemplateContext context, ICollection<Controller> controllers) {
			return new PopupWindow(this, context, controllers);
		}
		protected override IFrameTemplate CreateDefaultTemplate(TemplateContext context) {
			IFrameTemplate template = FrameTemplateFactory.CreateTemplate(context);
			return template;
		}
		protected override ShowViewStrategyBase CreateShowViewStrategy() {
			return new ShowViewStrategy(this);
		}
		protected override List<Controller> CreateLogonWindowControllers() {
			List<Controller> controllerList = base.CreateLogonWindowControllers();
			if(TestScriptsManager.EasyTestEnabled) {
				controllerList.Add(new TestScriptsController());
			}
			controllerList.Add(new FocusController());
			controllerList.Add(new WebIdAssignationController());
			controllerList.Add(new ChooseThemeController());
			controllerList.Add(new WindowTemplateController());
			controllerList.Add(new RegisterThemeAssemblyController());
			controllerList.Add(new ActionHandleExceptionController());
			controllerList.Add(new WebPropertyEditorImmediatePostDataController());
			return controllerList;
		}
		protected override LayoutManager CreateLayoutManagerCore(bool simple) {
			return new WebLayoutManager(simple, DelayedViewItemsInitialization, WebApplicationStyleManager.IsNewStyle);
		}
		protected override SettingsStorage CreateLogonParameterStoreCore() {
			return new SettingsStorageOnCookies(ApplicationName);
		}
		protected virtual PopupWindowManager CreateWebPopupWindowManager() {
			return new PopupWindowManager(this);
		}
		protected virtual IFrameTemplateFactory CreateFrameTemplateFactory() {
			return new DefaultFrameTemplateFactory();
		}
		protected override ModelDifferenceStore CreateModelDifferenceStoreCore() {
			if(HttpContext.Current != null && HttpContext.Current.Server != null) {
				return new FileModelStore(HttpContext.Current.Server.MapPath(""), ModelDifferenceStore.AppDiffDefaultName);
			}
			return base.CreateModelDifferenceStoreCore();
		}
		protected override ModelDifferenceStore CreateDeviceSpecificModelDifferenceStoreCore() {
			if(HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Server != null) {
				string storePath = HttpContext.Current.Server.MapPath("");
				string deviceTemplateName = GetDiffDeviceTemplateName(storePath);
				if(!string.IsNullOrEmpty(deviceTemplateName)) {
					return new FileModelStore(storePath, deviceTemplateName);
				}
			}
			return base.CreateDeviceSpecificModelDifferenceStoreCore();
		}
		protected override ModelDifferenceStore CreateUserModelDifferenceStoreCore() {
			return new SessionModelDifferenceStore();
		}
		protected virtual bool CanReadSecuredLogonParameters() {
			return HttpContext.Current != null
				&& HttpContext.Current.User != null
				&& HttpContext.Current.User.Identity is FormsIdentity
				&& HttpContext.Current.User.Identity.IsAuthenticated
				&& ((FormsIdentity)HttpContext.Current.User.Identity).Ticket != null;
		}
		protected virtual void ReadSecuredLogonParameters() {
			if(SecuritySystem.LogonParameters != null) {
				FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
				FormsAuthenticationTicket ticket = id.Ticket;
#pragma warning disable 0618
				if(SecuritySystem.LogonParameters is ISupportStringSerialization) {
					((ISupportStringSerialization)SecuritySystem.LogonParameters).SetValuesFromString(ticket.UserData);
				}
#pragma warning restore 0618
				else {
					SettingsStorageOnString storage = new SettingsStorageOnString();
					storage.SetContentFromString(ticket.UserData);
					ObjectSerializer.ReadObjectPropertyValues(storage, SecuritySystem.LogonParameters);
				}
			}
		}
		protected virtual void WriteSecuredLogonParameters() {
			if(HttpContext.Current == null) {
				Tracing.Tracer.LogWarning("Cannot add a Forms cookie to the Respose.Cookies collection: the HttpContext.Current property is null");
				return;
			}
			if(HttpContext.Current.Response == null) {
				Tracing.Tracer.LogWarning("Cannot add a Forms cookie to the Respose.Cookies collection: the HttpContext.Current.Response property is null");
				return;
			}
			string logonParametersAsString = "";
			if(SecuritySystem.LogonParameters != null) {
#pragma warning disable 0618
				if(SecuritySystem.LogonParameters is ISupportStringSerialization) {
					logonParametersAsString = ((ISupportStringSerialization)SecuritySystem.LogonParameters).GetValuesAsString();
				}
#pragma warning restore 0618
				else {
					SettingsStorageOnString storage = new SettingsStorageOnString();
					ObjectSerializer.WriteObjectPropertyValues(null, storage, SecuritySystem.LogonParameters);
					logonParametersAsString = storage.GetContentAsString();
				}
			}
			HttpCookie cookie = FormsAuthentication.GetAuthCookie("", CanCreatePersistentCookie);
			FormsAuthenticationTicket formsTicket = FormsAuthentication.Decrypt(cookie.Value);
			DateTime ticketExpiration = formsTicket.Expiration;
			if(HttpContext.Current != null && HttpContext.Current.Session != null) {
				TimeSpan ticketTimeout = formsTicket.Expiration - formsTicket.IssueDate;
				if(HttpContext.Current.Session.Timeout > ticketTimeout.TotalMinutes) {
					Tracing.Tracer.LogWarning("The FormsAuthentication timeout is less than the ASP.NET Session timeout: '{0}' and '{1}'. The ASP.NET Session timeout is used for FormsAuthentication.", ticketTimeout.Minutes, HttpContext.Current.Session.Timeout);
					ticketExpiration = formsTicket.IssueDate.AddMinutes(HttpContext.Current.Session.Timeout + 1);
				}
			}
			FormsAuthenticationTicket xafTicket = new FormsAuthenticationTicket(
				formsTicket.Version, formsTicket.Name, formsTicket.IssueDate, ticketExpiration,
				formsTicket.IsPersistent, logonParametersAsString, formsTicket.CookiePath);
			string encryptedXafTicket = FormsAuthentication.Encrypt(xafTicket);
			if(encryptedXafTicket.Length < CookieContainer.DefaultCookieLengthLimit) {
				cookie.Value = encryptedXafTicket;
			}
			else {
				Tracing.Tracer.LogWarning("Cannot cache a login information into a FormsAuthentication cookie: " +
					"the result length is '" + encryptedXafTicket.Length + "' bytes and it exceeds the maximum cookie length '" + CookieContainer.DefaultCookieLengthLimit + "' (see the 'System.Net.CookieContainer.DefaultCookieLengthLimit' property)");
			}
			HttpContext.Current.Response.Cookies.Add(cookie);
		}
		protected virtual IHttpRequestManager CreateHttpRequestManager() {
			return new DefaultHttpRequestManager();
		}
		protected override void DisposeCore() {
			SafeExecutor executor = new SafeExecutor(this);
			if(mainWindow != null) {
				executor.Dispose(mainWindow);
				mainWindow = null;
			}
			if(logonWindow != null) {
				executor.Dispose(logonWindow);
				logonWindow = null;
			}
			logonController = null;
			if(startUpActions != null) {
				startUpActions.Clear();
				startUpActions = null;
			}
			if(popupWindowManager != null) {
				executor.Dispose(popupWindowManager);
				popupWindowManager = null;
			}
			applicationStatisticObject.SetDisposed();
			base.DisposeCore();
			executor.ThrowExceptionIfAny();
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			Modules.AddRequiredModule<DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule>("The module is a system module");
		}
		protected override string GetUserFormattingCultureName() {
			return GetRequestCultureName();
		}
		protected override string GetUserCultureName() {
			return GetRequestCultureName();
		}
		protected override void OnAfterSetCulture() {
			base.OnAfterSetCulture();
			if(HttpContext.Current != null && HttpContext.Current.Session != null) {
				Redirect(HttpContext.Current.Request.RawUrl, false, true);
			}
		}
		[Browsable(false)]
		public TemplateContentSettings Settings {
			get { return settings; }
		}
		public WebApplication() {
			Initialize();
		}
		private void Initialize() {
			applicationStatisticObject = new ApplicationStatisticObject();
			if(HttpContext.Current != null) {
				ValueManager.ValueManagerType = typeof(ASPSessionValueManager<>).GetGenericTypeDefinition();
			}
			lock(WebApplicationStatistic.Applications) {
				WebApplicationStatistic.Applications.Add(applicationStatisticObject);
			}
			popupWindowManager = CreateWebPopupWindowManager();
			if(ApplicationCreated != null) {
				ApplicationCreated(this, EventArgs.Empty);
			}
		}
		~WebApplication() {
			if(applicationStatisticObject != null) {
				applicationStatisticObject.SetFinalized();
				applicationStatisticObject = null;
			}
		}
		public override void LogOff() {
			base.LogOff();
			LogOff(true);
		}
		public void LogOff(bool canCancel) {
			LoggingOffEventArgs loggingOffEventArgs = new LoggingOffEventArgs(SecuritySystem.LogonParameters, canCancel);
			OnLoggingOff(loggingOffEventArgs);
			if(!canCancel || !loggingOffEventArgs.Cancel) {
				isLoggedOn = false; 
				if(HttpContext.Current != null) {
					SecuritySystem.Instance.Logoff();
					if(WebWindow.CurrentRequestPage != null) {
						((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager.NeedEndResponse = true;
					}
					HttpContext.Current.Session.Abandon();
					FormsAuthentication.SignOut();
					WebApplication.Redirect(FormsAuthentication.DefaultUrl);
				}
				OnLoggedOff();
			}
		}
		public static void LogOff(HttpSessionState session) {
			WebApplication instance = session[SessionApplicationVariable] as WebApplication;
			if(instance != null) {
				instance.LogOff(false);
			}
		}
		public void Start() {
			if(IsDisposed) {
				throw new ObjectDisposedException(GetType().FullName);
			}
			EnsureShowViewStrategy();
			Tracing.Tracer.LogVerboseValue("IsAuthenticated", SecuritySystem.IsAuthenticated);
			if(!SecuritySystem.IsAuthenticated && CanReadSecuredLogonParameters()) {
				if(!CanAutomaticallyLogonWithStoredLogonParameters) {
					LogOff(false);
				}
				else {
					try {
						ReadSecuredLogonParameters();
						try {
							shouldGoToFormsRedirectURL = false;
							Logon(null);
						}
						finally {
							shouldGoToFormsRedirectURL = true;
						}
					}
					catch(ThreadAbortException) {
						throw;
					}
					catch(Exception e) {
						Tracing.Tracer.LogSubSeparator("Exception occurs while a silent user authentication");
						Tracing.Tracer.LogError(e);
						LogOff(false);
					}
				}
			}
			else {
				if(SecuritySystem.Instance.NeedLogonParameters) {
					PopupWindowShowAction logonAction = CreateLogonAction();
					CustomizePopupWindowParamsEventArgs args = logonAction.GetPopupWindowParams();
					List<Controller> controllers = new List<Controller>();
					controllers.Add(args.DialogController);
					controllers.AddRange(args.DialogController.Controllers);
					logonWindow = new WebWindow(this, "", controllers, false, true);
					DetailView view = (DetailView)args.View;
					view.ViewEditMode = ViewEditMode.Edit;
					logonWindow.SetView(args.View, true, null);
				}
				else {
					shouldGoToFormsRedirectURL = true;
					Logon(null);
					if(!IsLoggedOn) {
						Redirect(LogonPage);
					}
				}
			}
		}
		private static string sessionIdOnDisposing;
		public static void DisposeInstance(HttpSessionState session) {
			try {
				if(session == null) {
					Tracing.Tracer.LogSeparator("Cannot dispose application: session is null");
				}
				else {
					lock(ASPSessionValueManagerBase.LockObject) {
						ASPSessionValueManagerBase.ClearSessionValues(session.SessionID);
						WebModelMultipleMasterStore.RemoveMasterStore(session);
						ASPSessionValueManagerBase.DisposedSessionSession = session;
						sessionIdOnDisposing = session.SessionID;
						try {
							Tracing.Tracer.LogSeparator("Dispose application");
							if(session[SessionApplicationVariable] != null) {
								WebApplication webApplication = (WebApplication)session[SessionApplicationVariable];
								if(webApplication.IsLoggedOn) {
									webApplication.LogOff(false);
								}
								(webApplication).Dispose();
								Tracing.Tracer.LogSeparator("Application is disposed");
								session[SessionApplicationVariable] = null;
							}
						}
						finally {
							ASPSessionValueManagerBase.DisposedSessionSession = null;
							sessionIdOnDisposing = "";
						}
					}
				}
			}
			catch(Exception ex) {
				Tracing.Tracer.LogError(ex);
			}
		}
		public static void SetInstance(HttpSessionState session, WebApplication application) {
			if(session == null) {
				throw new ArgumentNullException("session");
			}
			if(application == null) {
				throw new ArgumentNullException("application");
			}
			session[SessionApplicationVariable] = application;
			Tracing.Tracer.LogValue("session.Timeout", session.Timeout);
			Tracing.Tracer.LogValue("SessionID", session.SessionID);
			application.applicationStatisticObject.SetSessionId(session.SessionID);
		}
		public static void Redirect(string url, bool endResponse, bool useHttpLocationHeader) {
			Tracing.Tracer.LogVerboseValue("url", url);
			Tracing.Tracer.LogVerboseValue("endResponse", endResponse);
			Tracing.Tracer.LogVerboseValue("useHttpLocationHeader", useHttpLocationHeader);
			if(HttpContext.Current != null) {
				if(useHttpLocationHeader) {
					if(WebWindow.CurrentRequestPage != null) {
						Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), WebWindow.CurrentRequestPage.GetType(), "Page");
						((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager.Clear();
					}
					HttpContext.Current.Response.RedirectLocation = url;
					if(endResponse) {
						HttpContext.Current.ApplicationInstance.CompleteRequest();
					}
				}
				else {
					HttpContext.Current.Response.Redirect(url, endResponse);
				}
			}
		}
		public static void Redirect(string url) {
			Redirect(url, true, GetIsCallback());
		}
		public static void Redirect(string url, bool endResponse) {
			Redirect(url, endResponse, GetIsCallback());
		}
		public void InitializeCulture() {
			if(sharedApplication != this) {
				InitializeLanguage();
			}
		}
		private void CreateControlsCore(Page page) {
			if(IsDisposed) {
				throw new ObjectDisposedException(GetType().FullName);
			}
			Tracing.Tracer.LogVerboseSubSeparator("WebApplication.CreateControls");
			Tracing.Tracer.LogVerboseValue("page", page);
			Tracing.Tracer.LogVerboseValue("HttpContext.Current", HttpContext.Current);
			if(!IsLoggedOn) {
				Redirect(LogonPage);
			}
			Guard.ArgumentNotNull(page, "page");
			Guard.ArgumentNotNull(page.Request, "page.Request");
			Guard.ArgumentNotNull(page.Session, "page.Session");
			applicationStatisticObject.UpdateAccessedOn();
			WebApplicationStatistic.WriteTraceInfoIfNeed();
			InitializeCulture();
			if(ControlsCreating != null) {
				ControlsCreating(this, new ControlsCreatingEventArgs(page));
			}
			if(startUpActions == null) {
				startUpActions = new List<PopupWindowShowAction>();
				foreach(ModuleBase module in Modules) {
					foreach(PopupWindowShowAction action in module.GetStartupActions()) {
						if(action.Enabled && action.Active) {
							action.Application = this;
							action.Executed += new EventHandler<ActionBaseEventArgs>(StartupAction_Executed);
							startUpActions.Add(action);
						}
					}
				}
			}
			if(startUpActions.Count > 0) {
				if(!RequestManager.IsPopupWindow()) {
					Redirect(PopupWindowTemplatePage);
				}
			}
			GetRequestedWindow(page).CreateControls(page);
		}
		private void SetCollectionEditMode(ViewEditMode viewEditMode) {
			if(this.Model != null && this.Model.Options is IModelOptionsWeb) {
				((IModelOptionsWeb)this.Model.Options).CollectionsEditMode = viewEditMode;
			}
			else {
				collectionsEditMode = viewEditMode;
			}
		}
		private void CreateLogonControls(Page logonPage) {
			Tracing.Tracer.LogVerboseText("CreateLogonControls");
			ISecurityStrategyBase securitySystemInstance = SecuritySystem.Instance;
			bool needLogonParameters = securitySystemInstance.NeedLogonParameters;
			if(IsDisposed) {
				throw new ObjectDisposedException(GetType().FullName);
			}
			Tracing.Tracer.LogVerboseText("applicationStatisticObject == null: " + (applicationStatisticObject == null).ToString());
			applicationStatisticObject.UpdateAccessedOn();
			WebApplicationStatistic.WriteTraceInfoIfNeed();
			if(IsLoggedOn) {
				if(!CanReadSecuredLogonParameters() && needLogonParameters) {
					Tracing.Tracer.LogWarning("Cannot read FormsAuthentication ticket. Redirecting to the Login page to reenter credentials.");
					LogOff();
				}
				else {
					Tracing.Tracer.LogText("The user already logged on, redirecting to the default page");
					Redirect(DefaultPage);
				}
			}
			else {
				if(needLogonParameters) {
					if(logonWindow == null) {
						throw new InvalidOperationException("Application isn't started");
					}
					logonWindow.CreateControls(logonPage, false);
				}
				else {
					Logon(null);
					if(!IsLoggedOn) {
						Tracing.Tracer.LogText("logonWindow == null:" + (logonWindow == null).ToString());
						logonWindow.CreateControls(logonPage, false);
					}
				}
			}
		}
		private const string CurrentRequestTemplateTypeKey = "WebApplication_CurrentRequestTemplateType";
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static TemplateType CurrentRequestTemplateType {
			get {
				if(HttpContext.Current != null && HttpContext.Current.Session != null) {
					TemplateType? result = ValueManager.GetValueManager<TemplateType?>(CurrentRequestTemplateTypeKey).Value;
					if(result.HasValue) {
						return result.Value;
					}
				}
				return TemplateType.Vertical;
			}
			set {
				if(HttpContext.Current != null && HttpContext.Current.Session != null) {
					ValueManager.GetValueManager<TemplateType?>(CurrentRequestTemplateTypeKey).Value = value;
				}
			}
		}
		private const string PreferredApplicationWindowTemplateTypeKey = "WebApplication_PreferredApplicationWindowTemplateType";
		public static TemplateType PreferredApplicationWindowTemplateType {
			get {
				if(HttpContext.Current != null && HttpContext.Current.Session != null) {
					TemplateType? result = ValueManager.GetValueManager<TemplateType?>(PreferredApplicationWindowTemplateTypeKey).Value;
					if(result.HasValue) {
						return result.Value;
					}
				}
				return TemplateType.Vertical;
			}
			set {
				if(HttpContext.Current != null && HttpContext.Current.Session != null) {
					ValueManager.GetValueManager<TemplateType?>(PreferredApplicationWindowTemplateTypeKey).Value = value;
				}
			}
		}
		public void CreateControls(Page page) {
			if(CurrentRequestTemplateType != TemplateType.Logon) {
				CreateControlsCore(page);
			}
			else {
				CreateLogonControls(page);
			}
		}
#if !SL
	[DevExpressExpressAppWebLocalizedDescription("WebApplicationRequestManager")]
#endif
		[Browsable(false)]
		public IHttpRequestManager RequestManager {
			get {
				if(httpRequestManager == null) {
					httpRequestManager = CreateHttpRequestManager();
				}
				return httpRequestManager;
			}
		}
		public override bool ShowDetailViewFrom(Frame sourceFrame) {
			return true;
		}
		public static WebApplication Instance {
			get {
				if(HttpContext.Current != null && HttpContext.Current.Session != null) {
					return (WebApplication)HttpContext.Current.Session[SessionApplicationVariable];
				}
				return null;
			}
		}
		public string GetDiffDeviceTemplateName(string storePath) {
			string fileNameTemplate = "";
			switch(DeviceDetector.Instance.GetDeviceCategory()) {
				case DeviceCategory.Mobile:
					fileNameTemplate = ModelDifferenceStore.AppDiffDefaultMobileName;
					break;
				case DeviceCategory.Tablet:
					fileNameTemplate = ModelDifferenceStore.AppDiffDefaultTabletName;
					break;
				case DeviceCategory.Desktop:
					fileNameTemplate = ModelDifferenceStore.AppDiffDefaultDesktopName;
					break;
				default:
					break;
			}
			if(File.Exists(Path.Combine(storePath, fileNameTemplate + ModelStoreBase.ModelFileExtension))) {
				return fileNameTemplate;
			}
			else {
				return string.Empty;
			}
		}
		internal static ApplicationModelManager sharedModelManager;
		internal static XafApplication sharedApplication;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetShared(WebApplication application) {
			sharedApplication = application;
			sharedApplication.Setup(); 
		}
		protected override void ActivateResourceLocalizers(IModelApplication model) {
			if(sharedApplication != this) {
				base.ActivateResourceLocalizers(model);
			}
		}
		protected override void InitializeCaptionHelper(IModelApplication model) {
			if(sharedApplication != this) {
				base.InitializeCaptionHelper(model);
			}
		}
		protected override void InitializeSecuritySystem(ExpressApplicationSetupParameters parameters) {
			if(sharedApplication != this) {
				base.InitializeSecuritySystem(parameters);
			}
		}
		protected override bool IsSharedModel { get { return true; } }
		protected override bool CanLoadTypesInfo() {
			return base.CanLoadTypesInfo() && (!IsSharedModel || (sharedModelManager == null)) && ((sharedApplication == null || sharedApplication == this));
		}
		protected override void OnModelChanged() {
			base.OnModelChanged();
			if(collectionsEditMode.HasValue) {
				SetCollectionEditMode(collectionsEditMode.Value);
			}
		}
		protected override bool UseNestedObjectSpace(Frame sourceFrame, TargetWindow targetWindow, Type objectType) {
			if(targetWindow == TargetWindow.NewModalWindow || targetWindow == TargetWindow.NewWindow) {
				return true;
			}
			if(sourceFrame != null && sourceFrame.View != null) {
				if((sourceFrame.Context == TemplateContext.LookupControl || sourceFrame.Context == TemplateContext.LookupWindow || !sourceFrame.View.IsRoot)
					&& (((IModelDetailViewWeb)this.FindModelClass(objectType).DefaultDetailView).CollectionsEditMode == ViewEditMode.Edit)) {
					return true;
				}
			}
			return false;
		}
		protected override ApplicationModelManager CreateModelManager(IEnumerable<Type> boModelTypes) {
			if(sharedApplication != null) {
				if(sharedApplication == this) {
					ApplicationModelManager sharedModelManager = base.CreateModelManager(boModelTypes);
					((IUnchangeableModelProvider)sharedModelManager).GetUnchangeableModel().HasMultipleMasters = true;
					return sharedModelManager;
				}
				return ((IApplicationModelManagerProvider)sharedApplication).GetModelManager();
			}
			else {
				if(IsSharedModel) {
					lock(sharedAppLocker) {
						if(sharedModelManager == null) {
							sharedModelManager = base.CreateModelManager(boModelTypes);
							((IUnchangeableModelProvider)sharedModelManager).GetUnchangeableModel().HasMultipleMasters = true;
						}
						return sharedModelManager;
					}
				}
				else {
					return base.CreateModelManager(boModelTypes);
				}
			}
		}
		[Browsable(false)]
		public new bool IsLoggedOn {
			get { return base.isLoggedOn; }
		}
		public override Window MainWindow { get { return mainWindow; } }
		[Browsable(false), DefaultValue(false)]
		public bool CanAutomaticallyLogonWithStoredLogonParameters {
			get { return canAutomaticallyLogonWithStoredLogonParameters; }
			set { canAutomaticallyLogonWithStoredLogonParameters = value; }
		}
		[Browsable(false), DefaultValue(false)]
		public bool CanCreatePersistentCookie {
			get { return canCreatePersistentCookie; }
			set { canCreatePersistentCookie = value; }
		}
		[Browsable(false)]
		public WebWindow LogonWindow { get { return logonWindow; } }
		[Browsable(false)]
		public PopupWindowManager PopupWindowManager {
			get { return popupWindowManager; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IFrameTemplateFactory FrameTemplateFactory {
			get {
				if(frameTemplateFactory == null) {
					frameTemplateFactory = CreateFrameTemplateFactory();
				}
				return frameTemplateFactory;
			}
			set {
				Guard.ArgumentNotNull(value, "FrameTemplateFactory");
				frameTemplateFactory = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("In ASP.NET applications, use 'IModelOptionsWeb.CollectionsEditMode' instead.")]
		public ViewEditMode CollectionsEditMode {
			get {
				if(this.Model != null && this.Model.Options is IModelOptionsWeb) {
					return ((IModelOptionsWeb)this.Model.Options).CollectionsEditMode;
				}
				return collectionsEditMode.HasValue ? collectionsEditMode.Value : ViewEditMode.View;
			}
			set {
				SetCollectionEditMode(value);
			}
		}
		public static event EventHandler ApplicationCreated;
		[
#if !SL
	DevExpressExpressAppWebLocalizedDescription("WebApplicationControlsCreating"),
#endif
 Category("Events")]
		public event EventHandler<ControlsCreatingEventArgs> ControlsCreating;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler StartupWindowCreated;
		#region Obsolete 12.2
		[Obsolete("Use the WebApplication.Settings.NestedFrameControlPathName property instead.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string NestedFrameTemplateControl = "NestedFrameControl.ascx";
		#endregion
		#region Obsolete 15.1
		[Obsolete("Use the MainWindow.SetView method instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected void ShowViewInMainWindow(View view, Frame sourceFrame) {
			if(MainWindow == null) {
				throw new InvalidOperationException("MainWindow == null");
			}
			MainWindow.SetView(view, sourceFrame);
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public View GetRequestedView() {
			ViewShortcut viewShourtcut = RequestManager.GetViewShortcut();
			return ProcessShortcut(viewShourtcut);
		}
		[Obsolete("Use the WebWindow.CurrentRequestWindow property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public WebWindow GetRequestedWindow() {
			WebWindow result = mainWindow;
#pragma warning disable 0618
			TemplateContext templateContext = RequestManager.GetTemplateContext();
#pragma warning restore 0618
			if(templateContext == TemplateContext.LogonWindow && LogonWindow != null) {
				return LogonWindow;
			}
			else if(templateContext == TemplateContext.ApplicationWindow || mainWindow == null) {
				if(mainWindow == null) {
					CreateMainWindow();
				}
				result = mainWindow;
			}
			else {
				WebWindow popupWindow = PopupWindowManager.FindRequestedWindow();
				if(popupWindow != null) {
					result = popupWindow;
				}
				else {
					PopupWindow startupActionWindow = GetStartupActionWindow();
					return startupActionWindow == null ? result : startupActionWindow;
				}
			}
			return result;
		}
		#endregion
#if DebugTest
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void DebugTest_ClearSharedModelManager() {
			sharedModelManager = null;
		}
#endif
	}
	public enum FileLocation { None, ApplicationFolder }
	[DomainComponent]
	public class LogonAttemptsAmountedToLimit { }
	[DomainComponent]
	public class ParameterlessLogonFailedInfo {
		public const string DetailViewId = "ParameterlessLogonFailedInfo_DefaultDetailView";
		private string message;
		public ParameterlessLogonFailedInfo(string message) {
			this.message = message;
		}
		public string Message {
			get { return message; }
		}
	}
	public class ControlsCreatingEventArgs : EventArgs {
		private Page page;
		public ControlsCreatingEventArgs(Page page) {
			this.page = page;
		}
		public Page Page {
			get { return page; }
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ClientInfo {
		private IDictionary<string, object> propertiesMap = null;
		public void SetInfo(IDictionary<string, object> propertiesMap) {
			this.propertiesMap = propertiesMap;
		}
		public object GetValue(string propertyName) {
			if(propertiesMap != null && !string.IsNullOrEmpty(propertyName)) {
				object result;
				propertiesMap.TryGetValue(propertyName, out result);
				return result;
			}
			else {
				return null;
			}
		}
		public ValueType GetValue<ValueType>(string propertyName) {
			object innerValue = GetValue(propertyName);
			if(innerValue != null) {
				TypeConverter conv = TypeDescriptor.GetConverter(typeof(ValueType));
				if(conv.CanConvertFrom(innerValue.GetType())) {
					return (ValueType)conv.ConvertFrom(innerValue);
				}
				else {
					if(IsNumericType(innerValue.GetType()) || innerValue is ValueType) {
						return (ValueType)Convert.ChangeType(innerValue, typeof(ValueType));
					}
					else {
						return GetDefaultValue<ValueType>();
					}
				}
			}
			else {
				return GetDefaultValue<ValueType>();
			}
		}
		private ValueType GetDefaultValue<ValueType>() {
			Type valueType = typeof(ValueType);
			if(IsNumericType(valueType)) {
				return (ValueType)Convert.ChangeType(-1, valueType);
			}
			else {
				if(Type.GetTypeCode(valueType) == TypeCode.String) {
					return (ValueType)Convert.ChangeType(string.Empty, valueType);
				}
				else {
					return default(ValueType);
				}
			}
		}
		private static bool IsNumericType(Type valueType) {
			switch(Type.GetTypeCode(valueType)) {
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
					return true;
				default:
					return false;
			}
		}
	}
}
