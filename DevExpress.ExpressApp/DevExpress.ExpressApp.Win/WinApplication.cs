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
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionControls.Binding;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.ExpressApp.Win.EasyTest;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.ExpressApp.Win.Localization;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.ExpressApp.Win.Templates.ActionControls.Binding;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Win {
	public class CustomHandleExceptionEventArgs : HandledEventArgs {
		private Exception exception;
		public CustomHandleExceptionEventArgs(Exception e) {
			this.exception = e;
		}
		public Exception Exception {
			get { return exception; }
		}
	}
	public class CustomGetUserModelDifferencesPathEventArgs : EventArgs {
		private string path;
		public CustomGetUserModelDifferencesPathEventArgs(string path) {
			this.path = path;
		}
		public string Path {
			get { return path; }
			set { path = value; }
		}
	}
	public enum FileLocation { None, ApplicationFolder, CurrentUserApplicationDataFolder }
	[ToolboxItemFilter("Xaf.Platform.Win")]
	[ToolboxBitmap(typeof(WinApplication), "Resources.Toolbox_XAFApplication_Win.ico")]
	public class WinApplication : XafApplication, ISupportSplashScreen {
		public const string UserDiffsModelLocationKey = "UserModelDiffsLocation";
		public const string ModelDiffsPathArgName = "-ModelDiffsPath:";
		public const string IgnoreUserModelDiffsArgName = "-IgnoreUserModelDiffs";
		public const string NewVersionServerAppSettingsName = "NewVersionServer";
		private int logonAttemptCount = 0;
		private bool exiting;
		private string modelDiffsPath;
		private string userModelDiffsPath;
		protected bool ignoreUserModelDiffs;
		private Boolean isApplicationUpdate;
		private Boolean isDelayedDetailViewDataLoadingEnabled;
		private IFrameTemplateFactory frameTemplateFactory;
		private static Messaging messaging = Messaging.GetMessaging(null);
		private bool isLoggedOff = false;
		private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
			HandleException(e.Exception);
		}
		private void DoLogon() {
			do {
				isLoggedOff = false;
				if(SecuritySystem.Instance.NeedLogonParameters) {
					Tracing.Tracer.LogText("Logon With Parameters");
					PopupWindowShowAction showLogonAction = CreateLogonAction();
					PopupWindowShowActionHelper helper = new PopupWindowShowActionHelper(showLogonAction);
#pragma warning disable 0618
					using(WinWindow popupWindow = helper.CreatePopupWindow(false)) {
#pragma warning restore 0618
						ShowLogonWindow(popupWindow);
					}
				}
				else {
					Logon(null);
				}
			} while(isLoggedOff);
		}
		private string GetUserModelDifferencesPath() {
			CustomGetUserModelDifferencesPathEventArgs args = new CustomGetUserModelDifferencesPathEventArgs("");
			switch(GetFileLocation<FileLocation>(FileLocation.ApplicationFolder, UserDiffsModelLocationKey)) {
				case FileLocation.CurrentUserApplicationDataFolder:
					args.Path = Application.UserAppDataPath;
					break;
				case FileLocation.ApplicationFolder:
					args.Path = PathHelper.GetApplicationFolder();
					break;
				default:
					args.Path = "";
					break;
			}
			OnCustomGetUserModelDifferencesPath(args);
			return args.Path;
		}
		private void GuardRecursiveUpdate() {
			string[] arguments = Environment.GetCommandLineArgs();
			if(arguments != null && arguments.Length > 0) {
				foreach(string arg in arguments) {
					if(arg == "ApplicationUpdateComplete") {
						IApplicationUpdater appUpdater = CreateApplicationUpdater();
						if((appUpdater != null) && appUpdater.NeedAppUpdate()) {
							throw new UserFriendlyException(new SystemException(UserVisibleExceptionLocalizer.GetExceptionMessage(UserVisibleExceptionId.ApplicationUpdateError)));
						}
					}
				}
			}
		}
		private void RefreshShowViewStrategy() {
			if(ShowViewStrategy.UIType != ((IModelOptionsWin)Model.Options).UIType) {
				base.ShowViewStrategy = CreateShowViewStrategy();
			}
		}
		protected internal new TemplateContext CalculateContext(TemplateContext context, String viewID) {
			return base.CalculateContext(context, viewID);
		}
		protected virtual void OnCustomGetUserModelDifferencesPath(CustomGetUserModelDifferencesPathEventArgs args) {
			return;
		}
		protected override Window CreateWindowCore(TemplateContext context, ICollection<Controller> controllers, bool isMain, bool activateControllersImmediatelly) {
			Tracing.Tracer.LogVerboseValue("WinApplication.CreateWindowCore.activateControllersImmediatelly", activateControllersImmediatelly);
			return new WinWindow(this, context, controllers, isMain, activateControllersImmediatelly);
		}
		protected override Window CreatePopupWindowCore(TemplateContext context, ICollection<Controller> controllers) {
			return new WinWindow(this, context, controllers, false, true);
		}
		protected override ShowViewStrategyBase CreateShowViewStrategy() {
			ShowViewStrategyBase strategy = new ShowInMultipleWindowsStrategy(this);
			if(Model != null) {
				IModelOptionsWin modelOptions = Model.Options as IModelOptionsWin;
				if(modelOptions != null) {
					switch(modelOptions.UIType) {
						case UIType.SingleWindowSDI:
							strategy = new ShowInSingleWindowStrategy(this);
							break;
						case UIType.StandardMDI:
							strategy = new MdiShowViewStrategy(this, MdiMode.Standard);
							break;
						case UIType.TabbedMDI:
							strategy = new MdiShowViewStrategy(this, MdiMode.Tabbed);
							break;
						default:
						case UIType.MultipleWindowSDI:
							break;
					}
				}
			}
			return strategy;
		}
		protected override IFrameTemplate CreateDefaultTemplate(TemplateContext context) {
			if(Model != null && Model.Options is IModelOptionsWin && FrameTemplateFactory is DefaultFrameTemplateFactoryV2) {
				((DefaultFrameTemplateFactoryV2)FrameTemplateFactory).FormStyle = ((IModelOptionsWin)Model.Options).FormStyle;
			}
			return FrameTemplateFactory.CreateTemplate(context);
		}
		protected override void OnCustomizeTemplate(IFrameTemplate frameTemplate, string templateContextName) {
			ISupportClassicToRibbonTransform supportClassicToRibbonTransform = frameTemplate as ISupportClassicToRibbonTransform;
			if(supportClassicToRibbonTransform != null && Model != null && Model.Options is IModelOptionsWin) {
				supportClassicToRibbonTransform.FormStyle = ((IModelOptionsWin)Model.Options).FormStyle;
			}
			base.OnCustomizeTemplate(frameTemplate, templateContextName);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetLogonParametersFileName(string path) {
			return Path.Combine(path, ModelStoreBase.LogonParametesDefaultName);
		}
		protected override SettingsStorage CreateLogonParameterStoreCore() {
			SettingsStorageOnString result = new SettingsStorageOnString();
			if(!string.IsNullOrEmpty(userModelDiffsPath)) {
				string fileName = GetLogonParametersFileName(userModelDiffsPath);
				if(File.Exists(fileName)) {
					result.SetContentFromString(File.ReadAllText(fileName));
				}
			}
			return result;
		}
		protected override void WriteLastLogonParametersCore(SettingsStorage storage, DetailView view, object logonObject) {
			base.WriteLastLogonParametersCore(storage, view, logonObject);
			SettingsStorageOnString settingsStorageOnString = storage as SettingsStorageOnString;
			if(!string.IsNullOrEmpty(userModelDiffsPath) && (settingsStorageOnString != null)) {
				string serializedValues = settingsStorageOnString.GetContentAsString();
				File.WriteAllText(GetLogonParametersFileName(userModelDiffsPath), serializedValues);
			}
		}
		protected override void Logon(PopupWindowShowActionExecuteEventArgs logonWindowArgs) {
			try {
				StartSplash();
				base.Logon(logonWindowArgs);
				if(logonWindowArgs != null) {
					logonWindowArgs.CanCloseWindow = true;
				}
				logonAttemptCount = 0;
			}
			catch(Exception e) {
				StopSplash();
				if(logonWindowArgs != null) {
					logonAttemptCount++;
					if (logonAttemptCount < MaxLogonAttemptCount) {
						HandleException(e);
						logonWindowArgs.CanCloseWindow = false;
					}
					else {
						logonWindowArgs.CanCloseWindow = true;
						HandleException(new UserFriendlyException(UserVisibleExceptionId.LogonAttemptsAmountedToLimitWin, e));
					}
				}
				else {
					throw;
				}
			}
		}
		protected override void OnSetupComplete() {
			base.OnSetupComplete();
			InitializeLanguage();
			messaging = Messaging.GetMessaging(this);
		}
		protected override void OnSetupStarted() {
			StartSplash();
			UpdateStatus(ApplicationStatusMesssageId.ApplicationSetupStarted.ToString(), ApplicationName + ApplicationStatusMesssagesLocalizer.Active.GetLocalizedString(ApplicationStatusMesssageId.ApplicationSetupStarted), (String)null);
			base.OnSetupStarted();
			UpdateStatus(ApplicationStatusMesssageId.LoadApplicationModules.ToString(), "", ApplicationStatusMesssagesLocalizer.Active.GetLocalizedString(ApplicationStatusMesssageId.LoadApplicationModules));
		}
		protected override void OnLoggingOn(LogonEventArgs args) {
			UpdateStatus(ApplicationStatusMesssageId.LoadUserDifferences.ToString(), "", ApplicationStatusMesssagesLocalizer.Active.GetLocalizedString(ApplicationStatusMesssageId.LoadUserDifferences));
			base.OnLoggingOn(args);
		}
		protected override void OnDetailViewCreated(DetailView view) {
			view.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
			base.OnDetailViewCreated(view);
		}
		protected override void CheckCompatibilityCore() {
			isApplicationUpdate = false;
			try {
				IApplicationUpdater appUpdater = CreateApplicationUpdater();
				if(appUpdater != null) {
					isApplicationUpdate = appUpdater.CheckAndUpdate();
				}
			}
			catch(DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException e) {
				Tracing.Tracer.LogError(e);
			}
			if(isApplicationUpdate) {
				Application.Exit();
				Application.DoEvents();
			}
			else {
				base.CheckCompatibilityCore();
			}
		}
		protected override string GetTraceLogDirectory() {
			string outputDirectory = null;
			switch(GetFileLocation<FileLocation>(FileLocation.ApplicationFolder, TraceLogLocationKey)) {
				case FileLocation.ApplicationFolder:
					outputDirectory = PathHelper.GetApplicationFolder();
					break;
				case FileLocation.CurrentUserApplicationDataFolder:
					outputDirectory = Application.LocalUserAppDataPath;
					break;
				default:
					break;
			}
			return outputDirectory;
		}
		protected override ModelDifferenceStore CreateModelDifferenceStoreCore() {
			if(!string.IsNullOrEmpty(modelDiffsPath)) {
				return new FileModelStore(modelDiffsPath, GetDiffDefaultName(modelDiffsPath));
			}
			return base.CreateModelDifferenceStoreCore();
		}
		protected override ModelDifferenceStore CreateUserModelDifferenceStoreCore() {
			if(!string.IsNullOrEmpty(userModelDiffsPath)) {
				return new FileModelStore(userModelDiffsPath, ModelDifferenceStore.UserDiffDefaultName);
			}
			return base.CreateUserModelDifferenceStoreCore();
		}
		protected override LayoutManager CreateLayoutManagerCore(bool simple) {
			if(simple) {
				return new WinSimpleLayoutManager();
			}
			else {
				return new WinLayoutManager();
			}
		}
		protected override ApplicationModelManager CreateModelManager(IEnumerable<Type> boModelTypes) {
			UpdateStatus(ApplicationStatusMesssageId.GenerateApplicationModel.ToString(), "", ApplicationStatusMesssagesLocalizer.Active.GetLocalizedString(ApplicationStatusMesssageId.GenerateApplicationModel));
			return base.CreateModelManager(boModelTypes);
		}
		protected override ModuleTypeList GetDefaultModuleTypes() {
			List<Type> result = new List<Type>(base.GetDefaultModuleTypes());
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule));
			return new ModuleTypeList(result.ToArray());
		}
		protected override void Dispose(bool disposing) {
			RemoveSplash();
			Application.ThreadException -= new ThreadExceptionEventHandler(Application_ThreadException);
			base.Dispose(disposing);
		}
		protected override List<Controller> CreateLogonWindowControllers() {
			List<Controller> logonWindowControllers = base.CreateLogonWindowControllers();
			logonWindowControllers.Add(new HtmlFormattingController());
			logonWindowControllers.Add(new WindowControlFinderController());
			return logonWindowControllers;
		}
		protected internal virtual IApplicationUpdater CreateApplicationUpdater() {
			String newAppVersionServer = (String)ConfigurationManager.AppSettings[NewVersionServerAppSettingsName];
			if(GetCheckCompatibilityType(ObjectSpaceProvider) == CheckCompatibilityType.ModuleInfo && !String.IsNullOrEmpty(newAppVersionServer) && (ObjectSpaceProvider.ModuleInfoType != null)) {
				return new ApplicationUpdater(ObjectSpaceProvider.CreateUpdatingObjectSpace(false), Modules, ApplicationName, newAppVersionServer, ObjectSpaceProvider.ModuleInfoType);
			}
			if(GetCheckCompatibilityType(ObjectSpaceProvider) != CheckCompatibilityType.ModuleInfo && !String.IsNullOrEmpty(newAppVersionServer)) {
				throw new ArgumentException("Cannot update the application when the XafApplication.CheckCompatibilityType property is set to DatabaseAndSchema. Please set this property to ModuleInfo, or disable the application update using the configuration file's NewVersionServer option.");
			}
			return null;
		}
		protected virtual Form CreateModelEditorForm() {
			ModelEditorViewController controller = new ModelEditorViewController(Model, CreateUserModelDifferenceStore());
			ModelDifferenceStore modelDifferencesStore = this.CreateModelDifferenceStore();
			if(modelDifferencesStore != null) {
				List<ModuleDiffStoreInfo> modulesDiffStoreInfo = new List<ModuleDiffStoreInfo>();
				modulesDiffStoreInfo.Add(new ModuleDiffStoreInfo(null, modelDifferencesStore, "Model"));
				controller.SetModuleDiffStore(modulesDiffStoreInfo);
			}
			return new ModelEditorForm(controller, new SettingsStorageOnModel(((IModelApplicationModelEditor)Model).ModelEditorSettings));
		}
		protected virtual void DoApplicationRun() {
			Application.Run();
		}
		protected virtual void ShowLogonWindow(WinWindow popupWindow) {
			StopSplash();
			popupWindow.ShowDialog();
		}
		protected virtual void ProcessStartupActions() {
			List<PopupWindowShowAction> startUpActions = new List<PopupWindowShowAction>();
			foreach(ModuleBase module in Modules) {
				startUpActions.AddRange(module.GetStartupActions());
			}
			Tracing.Tracer.LogValue("startUpActions", startUpActions.Count);
			foreach(PopupWindowShowAction action in startUpActions) {
				Tracing.Tracer.LogVerboseText("startUpAction: {0}, Enabled: {1}, Active: {2}", action.Id, action.Enabled, action.Active);
				if(action.Enabled && action.Active) {
					StopSplash();
					Tracing.Tracer.LogValue("startUpAction executing", action.Id);
					action.Application = this;
					using(PopupWindowShowActionHelper helper = new PopupWindowShowActionHelper(action)) {
						helper.ShowPopupWindow();
					}
					Tracing.Tracer.LogText("startUpAction executed", action.Id);
				}
			}
		}
		protected virtual Exception PreprocessException(Exception e) {
			Tracing.Tracer.LogError(e);
			if(!System.Diagnostics.Debugger.IsAttached) {
				if((e is DevExpress.Xpo.DB.Exceptions.SqlExecutionErrorException ||
					e.InnerException is DevExpress.Xpo.DB.Exceptions.SqlExecutionErrorException)) {
					e = new UserFriendlyException(UserVisibleExceptionId.UserFriendlySqlException, e);
				}
				if(e is CompatibilityException) {
					CompatibilityException compatibilityException = (CompatibilityException)e;
					if(compatibilityException.Error is CompatibilityUnableToOpenDatabaseError) {
						e = new UserFriendlyException(UserVisibleExceptionId.UserFriendlyConnectionFailedException, e);
					}
					else if(compatibilityException.Error is CompatibilityCheckVersionsError) {
					}
					else {
						e = new UserFriendlyException(UserVisibleExceptionId.UserFriendlyConnectionFailedException, e);
					}
				}
			}
			return e;
		}
		protected virtual void HandleExceptionCore(Exception e) {
			messaging.Show(Title, e);
		}
		protected virtual IFrameTemplateFactory CreateFrameTemplateFactory() {
			if(UseOldTemplates) {
				return new DefaultFrameTemplateFactory();
			}
			else {
				return new DefaultFrameTemplateFactoryV2();
			}
		}
		protected virtual void ShowStartupWindow() {
			UpdateStatus(ApplicationStatusMesssageId.ShowStartupWindow.ToString(), "", ApplicationStatusMesssagesLocalizer.Active.GetLocalizedString(ApplicationStatusMesssageId.ShowStartupWindow));
			ShowViewStrategy.ShowStartupWindow();
		}
		protected override void ExitCore() {
			base.ExitCore();
			exiting = true;
			Application.Exit();
		}
#if !DebugTest
		protected override string GetDcAssemblyFilePath() {
			if(System.Diagnostics.Debugger.IsAttached) {
				return null;
			}
			if(!string.IsNullOrEmpty(userModelDiffsPath)) {
				return Path.Combine(userModelDiffsPath, DcAssemblyFileName);
			}
			return base.GetDcAssemblyFilePath();
		}
		protected override string GetModelAssemblyFilePath() {
			if(System.Diagnostics.Debugger.IsAttached) {
				return null;
			}
			if(!string.IsNullOrEmpty(userModelDiffsPath)) {
				return Path.Combine(userModelDiffsPath, ModelAssemblyFileName);
			}
			return base.GetModelAssemblyFilePath();
		}
		protected override string GetModulesVersionInfoFilePath() {
			if(System.Diagnostics.Debugger.IsAttached) {
				return null;
			}
			if(!string.IsNullOrEmpty(userModelDiffsPath)) {
				return Path.Combine(userModelDiffsPath, ModulesVersionInfoFileName);
			}
			return base.GetModulesVersionInfoFilePath();
		}
#endif
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetScrollBarDesktopMode() {
			DevExpress.XtraEditors.ScrollBarBase.UIMode = DevExpress.XtraEditors.ScrollUIMode.Desktop;
		}
		static WinApplication() {
			SimpleActionBinding.Register();
			SingleChoiceActionBinding.Register();
			ParametrizedActionBinding.Register();
			PopupWindowShowActionBinding.Register();
			SetScrollBarDesktopMode();
		}
		public WinApplication() {
			UseOldTemplates = true;
			DevExpress.XtraEditors.Controls.Localizer.Active = new XtraEditorsLocalizer();
			defaultTitleTemplate = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			Tracing.Tracer.LogSeparator("Initialize application");
			string[] args = Environment.GetCommandLineArgs();
			Tracing.Tracer.LogValue("Command line arguments", args);
			modelDiffsPath = PathHelper.GetApplicationFolder();
			ignoreUserModelDiffs = Array.IndexOf(args, IgnoreUserModelDiffsArgName) >= 0;
			userModelDiffsPath = null;
			if(ignoreUserModelDiffs) {
				Tracing.Tracer.LogText(String.Format("The {0} command line parameter was passed", IgnoreUserModelDiffsArgName));
			}
			else {
				userModelDiffsPath = GetUserModelDifferencesPath();
			}
			foreach(string arg in args) {
				if(arg.StartsWith(ModelDiffsPathArgName)) {
					modelDiffsPath = arg.Substring(ModelDiffsPathArgName.Length);
					break;
				}
			}
		}
		public override ConfirmationResult AskConfirmation(ConfirmationType confirmationType) {
			ConfirmationResult result = base.AskConfirmation(confirmationType);
			DialogResult userChoice = DialogResult.None;
			switch(confirmationType) {
				case ConfirmationType.NeedSaveChanges: {
						userChoice = GetUserChoice(CaptionHelper.GetLocalizedText(Confirmations, "Save"),
							MessageBoxButtons.YesNoCancel);
						break;
					}
				case ConfirmationType.CancelChanges: {
						userChoice = GetUserChoice(CaptionHelper.GetLocalizedText(Confirmations, "Cancel"),
							MessageBoxButtons.YesNo);
						break;
					}
			}
			if(userChoice == DialogResult.Yes) {
				result = ConfirmationResult.Yes;
			}
			if(userChoice == DialogResult.No) {
				result = ConfirmationResult.No;
			}
			if(userChoice == DialogResult.Cancel) {
				result = ConfirmationResult.Cancel;
			}
			Tracing.Tracer.LogValue("UserConfirmationResult", result);
			return result;
		}
		public DialogResult GetUserChoice(string message, MessageBoxButtons buttons) {
			return Messaging.GetUserChoice(message, Title, buttons);
		}
		public void EditModel() {
			if(!ShowViewStrategy.CloseAllWindows()) {
				return;
			}
			ModelDifferenceStore differenceStore = CreateUserModelDifferenceStore();
			if(differenceStore != null) {
				differenceStore.SaveDifference(((ModelApplicationBase)Model).LastLayer); 
			}
			ICurrentAspectProvider oldAspectProvider = ((ModelApplicationBase)Model).CurrentAspectProvider;
			try {
				((ModelApplicationBase)Model).CurrentAspectProvider = new CurrentAspectProvider(oldAspectProvider.CurrentAspect);
				using(Form modelEditorForm = CreateModelEditorForm()) {
					modelEditorForm.ShowDialog();
					if(modelEditorForm is IModelEditorSettings) {
						if(differenceStore != null) {
							differenceStore.SaveDifference(((ModelApplicationBase)Model).LastLayer);
						}
					}
				}
				ApplicationModelManager.AddCustomMembersFromModelToTypeInfo(Model);
			}
			finally {
				((ModelApplicationBase)Model).CurrentAspectProvider = oldAspectProvider;
			}
			try {
				RefreshShowViewStrategy();
				ShowStartupWindow();
			}
			catch(Exception e) {
				HandleException(e);
				Application.Exit();
			}
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]   
		public void Restart() {
			ShowViewStrategy.CloseAllWindows();
			Setup(new ExpressApplicationSetupParameters(ApplicationName, Security, ObjectSpaceProviders, ControllersManager,   Modules));
			ShowStartupWindow();
		}
		public void HandleException(Exception e) {
			if(e is ActionExecutionException) {
				e = e.InnerException;
			}
			CustomHandleExceptionEventArgs args = new CustomHandleExceptionEventArgs(PreprocessException(e));
			if(CustomHandleException != null) {
				CustomHandleException(this, args);
			}
			if(!args.Handled) {
				HandleExceptionCore(args.Exception);
			}
		}
		public void Start() {
			try {
				Tick.In("WinApplication.Start()");
				GuardRecursiveUpdate();
				DoLogon();
				if(!isApplicationUpdate && IsLoggedOn) {
					try {
						ProcessStartupActions();
						if(!exiting) {
							StartSplash();
							ShowStartupWindow();
						}
						else {
							return;
						}
					}
					finally {
						StopSplash();
					}
					Tracing.Tracer.LogSeparator("Application startup done");
					Tracing.Tracer.LogSeparator("Application running");
					Tick.Out("WinApplication.Start()");
					DoApplicationRun();
					if(!ignoreUserModelDiffs) {
						SaveModelChanges();
					}
				}
			}
			catch(Exception e) {
				HandleException(e);
			}
			finally {
				LogOffCore(false);
			}
			Tracing.Tracer.LogSeparator("Application stopping");
		}
		public override void SaveModelChanges() {
			do {
				try {
					base.SaveModelChanges();
				}
				catch(Exception e) {
					Tracing.Tracer.LogError(e);
					if(Messaging.GetMessaging(this).GetUserChoice(
						CaptionHelper.GetLocalizedText("Messages", "UnableToSaveCustomization", new object[] { e.Message }), Title,
						MessageBoxButtons.RetryCancel) == DialogResult.Retry) {
						continue;
					}
				}
				break;
			} while(true);
		}
		public override bool ShowDetailViewFrom(Frame sourceFrame) {
			if(sourceFrame != null) {
				View sourceView = sourceFrame.View;
				DetailView detailView = sourceView as DetailView;
				if(detailView != null) {
					WinWindow window = sourceFrame as WinWindow;
					if(window != null) {
						return !window.Form.Modal;
					}
				}
			}
			return base.ShowDetailViewFrom(sourceFrame);
		}
		public override void LogOff() {
			base.LogOff();
			if(LogOffCore(true)) {
				DoLogon();
				if(!isLoggedOn) {
					Application.Exit();
				}
				else {
					try {
						ProcessStartupActions();
						StartSplash();
						ShowStartupWindow();
					}
					finally {
						StopSplash();
					}
				}
			}
		}
		private bool LogOffCore(bool canCancel) {
			if(IsLoggedOn) {
				LoggingOffEventArgs loggingOffEventArgs = new LoggingOffEventArgs(SecuritySystem.LogonParameters, canCancel);
				OnLoggingOff(loggingOffEventArgs);
				if(!canCancel || !loggingOffEventArgs.Cancel) {
					bool isMainWindowShown = ShowViewStrategy.MainWindow != null;
					if(ShowViewStrategy.CloseAllWindows()) {
						if(!ignoreUserModelDiffs) {
							SaveModelChanges();
						}
						isLoggedOn = false; 
						SecuritySystem.Instance.Logoff();
						OnLoggedOff();
						isLoggedOff = true;
						ResetUserDifferences(); 
						return isMainWindowShown;
					}
				}
			}
			return false;
		}
		private void ResetUserDifferences() {
			DropModel();	
		}
		private ISplash splash = new DevExpress.ExpressApp.Win.Core.SplashScreen();
		protected ISupportUpdateSplash supportUpdateSplash;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ISplash SplashScreen {
			get { return splash; }
			set {
				if((value != splash) && (splash != null)) {
					StopSplash();
				}
				splash = value;
				supportUpdateSplash = splash as ISupportUpdateSplash;
			}
		}
		public virtual void StartSplash() {
			if(splash != null && !splash.IsStarted) {
				splash.Start();
				Application.DoEvents();
			}
		}
		public virtual void StopSplash() {
			if(splash != null) {
				splash.Stop();
			}
		}
		public virtual void RemoveSplash() {
			SplashScreen = null;
		}
		public virtual void UpdateSplash(String context, String caption, String description, params Object[] additionalParams) {
			if(supportUpdateSplash != null) {
				supportUpdateSplash.UpdateSplash(caption, description, additionalParams);
			}
		}
		public override void UpdateStatus(String context, String title, String message, params Object[] additionalParams) {
			base.UpdateStatus(context, title, message, additionalParams);
			UpdateSplash(context, title, message, additionalParams);
			Application.DoEvents();
		}
		public override Window MainWindow {
			get {
				if(ShowViewStrategy != null) {
					return ((WinShowViewStrategyBase)ShowViewStrategy).MainWindow;
				}
				return null;
			}
		}
		[Browsable(false)]
		public bool UseOldTemplates { get; set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new WinShowViewStrategyBase ShowViewStrategy {
			get { return base.ShowViewStrategy as WinShowViewStrategyBase; }
			set { base.ShowViewStrategy = value; }
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
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string UserModelDifferenceFilePath {
			get { return userModelDiffsPath; }
			set { userModelDiffsPath = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ModelDifferenceFilePath {
			get { return modelDiffsPath; }
			set { modelDiffsPath = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IgnoreUserModelDiffs {
			get { return ignoreUserModelDiffs; }
			set { ignoreUserModelDiffs = value; }
		}
		public static Messaging Messaging {
			get { return messaging; }
			set { messaging = value; }
		}
		[ Category("Behavior")]
		public override Boolean IsDelayedDetailViewDataLoadingEnabled {
			get { return isDelayedDetailViewDataLoadingEnabled; }
			set { isDelayedDetailViewDataLoadingEnabled = value; }
		}
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("WinApplicationCustomHandleException"),
#endif
 Category("Customization")]
		public event EventHandler<CustomHandleExceptionEventArgs> CustomHandleException;
	}
}
