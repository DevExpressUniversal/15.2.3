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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp {
	public enum ConfirmationResult { Yes, No, Cancel };
	public enum ConfirmationType { NeedSaveChanges, CancelChanges };
	public enum DatabaseUpdateMode { Never, UpdateDatabaseAlways, UpdateOldDatabase };
	public enum CheckCompatibilityType { DatabaseSchema, ModuleInfo };
	public class LogonController : DialogController {
		protected override SimpleAction CreateAcceptAction() {
			SimpleAction logonAction = new SimpleAction(this, "Logon", DialogController.DialogActionContainerName);
			logonAction.Caption = "Log On";
			logonAction.ActionMeaning = ActionMeaning.Accept;
			return logonAction;
		}
	}
	[Designer("DevExpress.ExpressApp.Design.XafApplicationRootDesigner, DevExpress.ExpressApp.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IRootDesigner))]
	[DesignerSerializer("DevExpress.ExpressApp.Design.XafApplicationSerializer, DevExpress.ExpressApp.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix,
	"System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	[ToolboxItem(false)]
	[ToolboxBitmap(typeof(DevExpress.ExpressApp.XafApplication), "Resources.Toolbox_XAFApplication.ico")]
	[DesignerCategory("Component")]
	public abstract class XafApplication : Component, IDisposable, INotifyPropertyChanged, ISupportInitialize, IApplicationModelManagerProvider {
		public const string Confirmations = "Confirmations";
		private readonly object lockObjectA = new object();
		private readonly object compatibilityCheckLockObject = new object();
		public const string CurrentVersion = XafAssemblyInfo.VersionSuffix;
		public const string TraceLogLocationKey = "TraceLogLocation";
		public const string TablePrefixesKey = "TablePrefixes";
		public const string OptionsNodeName = "Options";
		public const string XafApplicationLogonCatchExceptionKey = "XafApplicationLogonCatchException";
		protected const string AfterSetupLayerId = ModelApplicationLayerIds.AfterSetup;
		protected const string DcAssemblyFileName = "DcAssembly.dll";
		protected const string ModelAssemblyFileName = "ModelAssembly.dll";
		protected const string ModulesVersionInfoFileName = "ModulesVersionInfo";
		protected ApplicationModelManager modelManager;
		private ModelApplicationBase afterSetupLayer;
		private ModelApplicationBase userAppDiffLayer;
		private ITypesInfo typesInfo;
		private bool isConnectionOwner = true;
		private ViewShortcut currentViewShortcutForSetCulture = null;
		private string selectedFormattingCultureName = "";
		private string applicationName = "";
		private string title = "";
		private bool isCompatibilityChecked = false;
		private ModuleList modules;
		private ControllersManager controllersManager;
		protected List<IObjectSpaceProvider> objectSpaceProviders;
		protected Boolean isObjectSpaceProviderOwner;
		private ShowViewStrategyBase showViewStrategy;
		private IEditorsFactory editorFactory = new EditorsFactory();
		private IModelApplication model;
		private String tablePrefixes;
		private bool isDisposed = false;
		private List<IObjectSpace> securitySystemObjectSpaces;
		private ISecurityStrategyBase security;
		private String connectionString = "";
		private IDbConnection connection;
		private DatabaseUpdateMode databaseUpdateMode = DatabaseUpdateMode.UpdateOldDatabase;
		private int isInitializingCounter = 0;
		private List<Type> localizerTypes = new List<Type>();
		private ICurrentAspectProvider currentAspectProvider = new CurrentAspectProviderFromCulture();
		private Boolean delayedViewItemsInitialization;
		private Boolean isAsyncServerMode;
		private Boolean? isActualModulesVersionInfo;
		private Int32 maxLogonAttemptCount;
		private Boolean linkNewObjectToParentImmediately = true;
		private CheckCompatibilityType checkCompatibilityType = CheckCompatibilityType.ModuleInfo;
		private ModelApplicationBase[] ModelDifferenceLayers {
			get {
				List<ModelApplicationBase> result = new List<ModelApplicationBase>(2);
				if(userAppDiffLayer != null) {
					result.Add(userAppDiffLayer);
				}
				result.Add(afterSetupLayer);
				return result.ToArray();
			}
		}
		private CollectionSourceDataAccessMode GetDataAccessMode(String listViewId) {
			CollectionSourceDataAccessMode result = CollectionSourceDataAccessMode.Client;
			if((model != null) && !String.IsNullOrEmpty(listViewId)) {
				IModelListView modelListView = FindModelView(listViewId) as IModelListView;
				if(modelListView != null) {
					result = modelListView.DataAccessMode;
				}
			}
			return result;
		}
		private IEnumerable<String> GetApplicationAspects() {
			List<string> languages = new List<string>();
			string languagesValue = ConfigurationManager.AppSettings["Languages"];
			if(languagesValue != null) {
				languages.AddRange(languagesValue.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
			}
			OnCustomizeLanguages(languages);
			return languages;
		}
		private void showLogonAction_OnExecute(Object sender, PopupWindowShowActionExecuteEventArgs args) {
			LogonController controller = null;
			try {
				if(args != null && args.PopupWindow != null) {
					controller = args.PopupWindow.GetController<LogonController>();
					foreach(ActionBase action in controller.Actions) {
						action.Enabled["Executing"] = false;
					}
				}
				Logon(args);
			}
			finally {
				if(controller != null) {
					foreach(ActionBase action in controller.Actions) {
						action.Enabled.RemoveItem("Executing");
					}
				}
			}
		}
		private void showLogonAction_OnCustomizePopupWindowParams(Object sender, CustomizePopupWindowParamsEventArgs args) {
			DetailView dv = CreateDetailView(CreateLogonWindowObjectSpace(SecuritySystem.LogonParameters), SecuritySystem.LogonParameters, null);
			dv.LayoutManager.CustomizationEnabled = false;
			args.View = dv;
			args.DialogController = CreateLogonController();
			args.DialogController.Controllers.AddRange(CreateLogonWindowControllers());
			args.IsSizeable = false;
			ReadLastLogonParameters(SecuritySystem.LogonParameters);
		}
		private void modulesManager_CustomizeTypesInfo(Object sender, CustomizeTypesInfoEventArgs e) {
			CustomizeTypesInfo();
		}
		private void DatabaseUpdater_StatusUpdating(Object sender, StatusUpdatingEventArgs e) {
			UpdateStatus(e.Context, e.Title, e.Message, e.AdditionalParams);
		}
		protected virtual void CustomizeTypesInfo() {
			if(objectSpaceProviders.Count > 0) {
				ITableNameCustomizer tableNameCustomizer = objectSpaceProviders[0] as ITableNameCustomizer;
				if(tableNameCustomizer != null) {
					tableNameCustomizer.CustomizeTableName += CustomizeTableName;
					tableNameCustomizer.Customize(tablePrefixes);
				}
			}
		}
		private void ObjectSpace_Connected(Object sender, EventArgs e) {
			try {
				CheckCompatibility();
			}
			finally {
				IObjectSpace objectSpace = sender as IObjectSpace;
				if(objectSpace != null) {
					objectSpace.Connected -= new EventHandler(ObjectSpace_Connected);
				}
			}
		}
		private ListView CreateListView(String listViewId, IModelListView modelListView, CollectionSourceBase collectionSource, bool isRoot) {
			return CreateListView(listViewId, modelListView, collectionSource, isRoot, null);
		}
		private ListView CreateListView(String listViewId, IModelListView modelListView, CollectionSourceBase collectionSource, bool isRoot, ListEditor listEditor) {
			Tracing.Tracer.LogVerboseText("Creating the '{0}' list view", listViewId);
			Guard.ArgumentNotNull(collectionSource, "collectionSource");
			ListViewCreatingEventArgs args = new ListViewCreatingEventArgs(listViewId, collectionSource, isRoot);
			OnViewCreating(args);
			OnListViewCreating(args);
			ListView result = args.View;
			if(result == null) {
				if((modelListView == null) || (!String.IsNullOrEmpty(args.ViewID) && (listViewId != args.ViewID))) {
					IModelView modelView = FindModelView(args.ViewID);
					if(modelView == null) {
						throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.NodeWasNotFound, args.ViewID));
					}
					modelListView = modelView as IModelListView;
				}
				if(modelListView == null) {
					throw new ArgumentException(string.Format(
						"A '{0}' node was passed while a '{1}' node was expected. Node id: '{2}'",
						typeof(IModelDetailView).Name, typeof(IModelListView).Name, listViewId));
				}
				result = new ListView(collectionSource, listEditor, isRoot, this);
				result.SetModel(modelListView);
			}
			Tracing.Tracer.LogVerboseText("The '{0}' list view is created", listViewId);
			OnViewCreated(result);
			OnListViewCreated(result);
			return result;
		}
		private CultureInfo CreateSpecificCultureSafe(String cultureName, CultureInfo defaultCultureInfo) {
			try {
				if(defaultCultureInfo != null && defaultCultureInfo.Name == cultureName) {
					return defaultCultureInfo;
				}
				else {
					return CultureInfo.CreateSpecificCulture(cultureName);
				}
			}
			catch(Exception e) {
				Tracing.Tracer.LogError(new ArgumentException("Error occurs while creating a CultureInfo object by name: '" + cultureName + "'", e));
			}
			return defaultCultureInfo;
		}
		private ApplicationModulesManager CreateModuleManager(String[] moduleAssemblies) {
			Tracing.Tracer.LogSubSeparator("Initialize Modules Manager");
			ApplicationModulesManager modulesManager = CreateApplicationModulesManager(CreateControllersManager());
			if(modulesManager == null) {
				throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.TheCreateApplicationModulesManagerReturnsNull));
			}
			modulesManager.Modules = modules;
			foreach(Type moduleType in GetDefaultModuleTypes()) {
				modulesManager.AddModule(moduleType);
			}
			modulesManager.AddModuleFromAssemblies(moduleAssemblies);
			return modulesManager;
		}
		private IList<IObjectSpaceProvider> CreateObjectSpaceProviders(String connectionString) {
			CreateCustomObjectSpaceProviderEventArgs args = null;
			if(!String.IsNullOrEmpty(connectionString)) {
				args = new CreateCustomObjectSpaceProviderEventArgs(connectionString);
			}
			else if(Connection != null) {
				args = new CreateCustomObjectSpaceProviderEventArgs(Connection);
			}
			else {
				args = new CreateCustomObjectSpaceProviderEventArgs(this.connectionString);
			}
			args.IsObjectSpaceProviderOwner = true;
			OnCreateCustomObjectSpaceProvider(args);
			if(args.ObjectSpaceProviders.Count == 0) {
				CreateDefaultObjectSpaceProvider(args);
			}
			isObjectSpaceProviderOwner = args.IsObjectSpaceProviderOwner;
			return args.ObjectSpaceProviders;
		}
		private void OnBeforeSetCulture() {
			if(MainWindow != null && MainWindow.View != null) {
				currentViewShortcutForSetCulture = MainWindow.View.CreateShortcut();
				MainWindow.SetView(null);
			}
		}
		private void CheckDetailViewId(String detailViewId, Type objectType) {
			if(String.IsNullOrEmpty(detailViewId)) {
				throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToFindADetailViewForType, objectType.FullName));
			}
		}
		private void OnViewCreating(ViewCreatingEventArgs args) {
			if(ViewCreating != null) {
				ViewCreating(this, args);
			}
		}
		private void OnViewCreated(View view) {
			if(ViewCreated != null) {
				ViewCreated(this, new ViewCreatedEventArgs(view));
			}
		}
		private void CreateUserModelDifferenceStore(CreateCustomModelDifferenceStoreEventArgs args) {
			if(CreateCustomUserModelDifferenceStore != null) {
				CreateCustomUserModelDifferenceStore(this, args);
			}
			if(!args.Handled) {
				args.Store = CreateUserModelDifferenceStoreCore();
			}
		}
		private Boolean IsActualModulesVersionInfo() {
			if(isActualModulesVersionInfo.HasValue) {
				return isActualModulesVersionInfo.Value;
			}
			String modulesVersionInfoFile = GetModulesVersionInfoFilePath();
			if(File.Exists(modulesVersionInfoFile)) {
				SettingsStorageOnString storage = new SettingsStorageOnString();
				storage.SetContentFromString(File.ReadAllText(modulesVersionInfoFile));
				if(storage.Values.Keys.Count != Modules.Count) {
					isActualModulesVersionInfo = false;
				}
				else {
					foreach(ModuleBase module in Modules) {
						String version = storage.LoadOption("", module.Name);
						if(String.IsNullOrEmpty(version) || (version != module.Version.ToString())) {
							isActualModulesVersionInfo = false;
							break;
						}
					}
				}
				if(!isActualModulesVersionInfo.HasValue) {
					isActualModulesVersionInfo = true;
				}
			}
			else {
				isActualModulesVersionInfo = false;
			}
			return isActualModulesVersionInfo.Value;
		}
		private void UpdateModulesVersionInfo() {
			String modulesVersionInfoFilePath = GetModulesVersionInfoFilePath();
			if(!String.IsNullOrEmpty(modulesVersionInfoFilePath)) {
				SettingsStorageOnString storage = new SettingsStorageOnString();
				foreach(ModuleBase module in Modules) {
					storage.SaveOption("", module.Name, module.Version.ToString());
				}
				File.WriteAllText(modulesVersionInfoFilePath, storage.GetContentAsString());
			}
		}
		private void ResetGeneratedAssemblies() {
			String dcAssemblyFilePath = GetDcAssemblyFilePath();
			if(File.Exists(dcAssemblyFilePath)) {
				File.Delete(dcAssemblyFilePath);
			}
			String modelAssemblyFilePath = GetModelAssemblyFilePath();
			if(File.Exists(modelAssemblyFilePath)) {
				File.Delete(modelAssemblyFilePath);
			}
		}
		private void SetupModelApplication(IEnumerable<ModelApplicationBase> layers) {
			if(model != null) {
				modelManager.DropModel((ModelApplicationBase)model);
			}
			ModelApplicationBase result = modelManager.CreateModelApplication(layers);
			InitializeCurrentAspectProvider(result);
			ActivateResourceLocalizers((IModelApplication)result);
			InitializeCaptionHelper((IModelApplication)result);
			model = (IModelApplication)result;
			OnModelChanged();
		}
		private void Initialize(ITypesInfo typesInfo) {
#if DebugTest
			lock(DevExpress.ExpressApp.DC.TypesInfo.lockObject) {
				DevExpress.ExpressApp.Tests.BaseXafTest.DisposableObjects.Add(this);
			}
#endif
			Tick.In("new XafApplication()");
			Guard.ArgumentNotNull(typesInfo, "typesInfo");
			this.typesInfo = typesInfo;
			objectSpaceProviders = new List<IObjectSpaceProvider>();
			defaultCollectionSourceMode = CollectionSourceMode.Proxy;
			delayedViewItemsInitialization = true;
			isAsyncServerMode = false;
			maxLogonAttemptCount = 3;
			String traceLogDirectory = "";
			try {
				traceLogDirectory = GetTraceLogDirectory();
			}
			catch(SecurityException) { }
			Tracing.Initialize(traceLogDirectory);
			tablePrefixes = ConfigurationManager.AppSettings[TablePrefixesKey];
			modules = new ModuleList(this);
			securitySystemObjectSpaces = new List<IObjectSpace>();
			Tick.Out("new XafApplication()");
		}
		private void RegisterModuleInfoTypes() {
			lock(DevExpress.ExpressApp.DC.TypesInfo.lockObject) {
				foreach(IObjectSpaceProvider objectSpaceProvider in objectSpaceProviders) {
					if(GetCheckCompatibilityType(objectSpaceProvider) == ExpressApp.CheckCompatibilityType.ModuleInfo
						&& objectSpaceProvider.SchemaUpdateMode == SchemaUpdateMode.DatabaseAndSchema
						&& objectSpaceProvider.ModuleInfoType != null) {
						typesInfo.RegisterEntity(objectSpaceProvider.ModuleInfoType);
					}
				}
			}
		}
		private void AddEntityStoresIntoTypesInfo() {
			TypesInfo localTypesInfo = typesInfo as TypesInfo;
			if(localTypesInfo != null) {
				foreach(IObjectSpaceProvider objectSpaceProvider in objectSpaceProviders) {
					localTypesInfo.AddEntityStore(objectSpaceProvider.EntityStore);
				}
			}
		}
		private void AddObjectSpaceProviders(IList<IObjectSpaceProvider> objectSpaceProviders) {
			if(objectSpaceProviders != null) {
				foreach(IObjectSpaceProvider objectSpaceProvider in objectSpaceProviders) {
					if((objectSpaceProvider != null) && !this.objectSpaceProviders.Contains(objectSpaceProvider)) {
						this.objectSpaceProviders.Add(objectSpaceProvider);
					}
				}
			}
		}
		protected CollectionSourceMode defaultCollectionSourceMode;
		protected bool isLoggedOn = false; 
		protected String defaultTitleTemplate = "";
		protected IList<Type> ignoredExceptions = new List<Type>();
		protected bool IsCompatibilityChecked {
			get { return isCompatibilityChecked; }
		}
		protected bool IsDisposed {
			get { return isDisposed; }
		}
		protected bool IsLoggedOn {
			get { return isLoggedOn; }
		}
		protected ControllersManager ControllersManager {
			get { return controllersManager; }
		}
		protected internal virtual bool SupportMasterDetailMode {
			get { return true; }
		}
		protected virtual bool IsSharedModel {
			get { return false; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected Boolean IsAsyncServerMode {
			get { return isAsyncServerMode; }
			set { isAsyncServerMode = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected void DropModel() {
			SetupModelApplication(ModelDifferenceLayers);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected ExpressApp.CheckCompatibilityType GetCheckCompatibilityType(IObjectSpaceProvider objectSpaceProvider) {
			CheckCompatibilityType updaterType = CheckCompatibilityType;
			if(objectSpaceProvider.CheckCompatibilityType.HasValue) {
				updaterType = objectSpaceProvider.CheckCompatibilityType.Value;
			}
			return updaterType;
		}
		protected internal View CreateView(IModelView viewModel) {
			View view = null;
			if(viewModel is IModelListView) {
				IModelListView listViewModel = (IModelListView)viewModel;
				IObjectSpace objectSpace = CreateObjectSpace(listViewModel.ModelClass.TypeInfo.Type);
				CollectionSourceBase collectionSource = CreateCollectionSource(objectSpace, listViewModel.ModelClass.TypeInfo.Type, viewModel.Id);
				view = CreateListView(listViewModel, collectionSource, true);
			}
			else if(viewModel is IModelDetailView) {
				IModelDetailView detailViewModel = (IModelDetailView)viewModel;
				IObjectSpace objectSpace = CreateObjectSpace(detailViewModel.ModelClass.TypeInfo.Type);
				view = CreateDetailView(objectSpace, detailViewModel, true);
			}
			else if(viewModel is IModelDashboardView) {
				IObjectSpace objectSpace = CreateObjectSpace();
				view = CreateDashboardView(objectSpace, viewModel.Id, true);
			}
			return view;
		}
		protected TemplateContext CalculateContext(TemplateContext context, String viewID) {
			String result = "";
			if(!String.IsNullOrEmpty(context.Name)) {
				result = context;
			}
			else {
				result = TemplateContext.PopupWindow;
				if(FindModelView(viewID) is IModelListView) {
					result = TemplateContext.LookupWindow;
				}
			}
			return result;
		}
		protected virtual List<Controller> CreateControllersCore(Type baseType) {
			return controllersManager.CreateControllers(baseType, model);
		}
		protected virtual bool UseNestedObjectSpace(Frame sourceFrame, TargetWindow targetWindow, Type objectType) {
			return true;
		}
		protected internal virtual IList<Controller> CreateControllers(Type baseType, bool createAllControllers, ICollection<Controller> additional) {
			List<Controller> controllers = new List<Controller>();
			if(controllersManager != null) {
				if(createAllControllers) {
					controllers = CreateControllersCore(baseType);
					for(int i = controllers.Count - 1; i >= 0; i--) {
						if(typeof(DialogController).IsAssignableFrom(controllers[i].GetType())) {
							controllers.RemoveAt(i);
						}
					}
				}
				if(additional != null) {
					foreach(Controller additionalController in additional) {
						for(int i = controllers.Count - 1; i >= 0; i--) {
							if(controllers[i].GetType().IsAssignableFrom(additionalController.GetType())) {
								controllers.RemoveAt(i);
							}
						}
					}
				}
			}
			if(additional != null) {
				controllers.AddRange(additional);
			}
			foreach(Controller controller in controllers) {
				controller.Application = this;
			}
			return controllers;
		}
		protected virtual void OnAfterSetCulture() {
			if(MainWindow != null && currentViewShortcutForSetCulture != null) {
				MainWindow.SetView(ProcessShortcut(currentViewShortcutForSetCulture));
				currentViewShortcutForSetCulture = null;
			}
		}
		protected ViewParameters CreateViewParameters(ViewShortcut shortcut) {
			ViewParameters result = null;
			if(shortcut == null) {
				throw new ArgumentNullException("shortcut");
			}
			try {
				IModelView modelView = FindModelView(shortcut.ViewId);
				Type objectType = ReflectionHelper.FindType(shortcut.ObjectClassName);
				if(modelView is IModelObjectView) {
					if(objectType == null) {
						if(((IModelObjectView)modelView).ModelClass == null) {
							throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.TheClassNameAttributeIsEmpty,
								modelView.Id));
						}
						objectType = ((IModelObjectView)modelView).ModelClass.TypeInfo.Type;
						if(objectType == null) {
							throw new InvalidOperationException();
						}
					}
					else if(!String.IsNullOrEmpty(shortcut.ViewId)) {
						if(((IModelObjectView)modelView).ModelClass != null) {
							Type objectTypeFromNode = ((IModelObjectView)modelView).ModelClass.TypeInfo.Type;
							if(!objectTypeFromNode.IsAssignableFrom(objectType)) {
								throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.TheClassNameAttributeRefersToADifferentType, objectType.FullName, objectTypeFromNode.FullName));
							}
						}
					}
				}
				else if(modelView == null) {
					if(!String.IsNullOrEmpty(shortcut.ObjectKey)) {
						modelView = FindModelView(GetDetailViewId(objectType));
					}
					else {
						modelView = FindModelView(GetListViewId(objectType));
					}
				}
				IObjectSpace objectSpace = CreateObjectSpace(objectType);
				if(modelView is IModelDashboardView) {
					result = new DashboardViewParameters(modelView.Id, shortcut.ScrollPosition);
				}
				else if(modelView is IModelObjectView) {
					Object objectKey = null;
					if(!String.IsNullOrEmpty(shortcut.ObjectKey)) {
						objectKey = objectSpace.GetObjectKey(objectType, shortcut.ObjectKey);
					}
					if(modelView is IModelDetailView) {
						if(String.IsNullOrEmpty(shortcut.ObjectKey)) {
							Int32 count = objectSpace.GetObjectsCount(objectType, null);
							if(count == 1) {
								objectKey = objectSpace.GetKeyValue(objectSpace.FindObject(objectType, null));
							}
							else {
								throw new ArgumentException(String.Format(
									"The ObjectKey is empty while the shortcut refers to a DetailView and there are '{0}' objects to show", count));
							}
						}
						result = new DetailViewParameters(modelView.Id, objectType, objectKey, shortcut.ScrollPosition);
					}
					else if(modelView is IModelListView) {
						result = new ListViewParameters(modelView.Id, objectType, objectKey, shortcut.ScrollPosition);
					}
					else {
						result = new ViewParameters(modelView.Id, objectType, objectKey, shortcut.ScrollPosition);
					}
				}
				if(result == null) {
					throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnknownViewNodeName, modelView.GetType().FullName, typeof(IModelDetailView).Name, typeof(IModelListView).Name));
				}
				result.ObjectSpace = objectSpace;
			}
			catch(Exception e) {
				throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.AnErrorHasOccurredWhileProcessingAShortcut, shortcut.ToString()) + Environment.NewLine + e.Message, e);
			}
			return result;
		}
		protected virtual void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
			throw new NotImplementedException(@"Override the XafApplication.CreateDefaultObjectSpaceProvider method and create an ObjectSpaceProvider there. Use the following code to create an XPObjectSpaceProvider:
protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
   args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection);
}
For details, refer to the ""XafApplication.ObjectSpaceProvider Property"" article in XAF documentation.");
		}
		protected T GetFileLocation<T>(T defaultValue, string keyName) {
			T result = defaultValue;
			string value = ConfigurationManager.AppSettings[keyName];
			if(!string.IsNullOrEmpty(value)) {
				result = (T)Enum.Parse(typeof(T), value, true);
			}
			return result;
		}
		protected virtual string CalculateLanguageName(string userLanguageName, string preferredLanguageName) {
			string treceMessage = Tracing.Tracer.GetMessageByValue("preferredLanguageName", preferredLanguageName, false);
			treceMessage += Tracing.Tracer.GetMessageByValue("userLanguageName", userLanguageName, true);
			Tracing.Tracer.LogText(treceMessage);
			if(string.IsNullOrEmpty(userLanguageName)) {
				Tracing.Tracer.LogWarning("User's language is empty");
			}
			string languageName = "";
			try {
				if(preferredLanguageName == CaptionHelper.DefaultLanguage) {
					languageName = CultureInfo.InvariantCulture.TwoLetterISOLanguageName;
				}
				else if(preferredLanguageName == CaptionHelper.UserLanguage || string.IsNullOrEmpty(preferredLanguageName)) {
					languageName = userLanguageName;
				}
				else if(!string.IsNullOrEmpty(preferredLanguageName)) {
					languageName = preferredLanguageName;
				}
			}
			catch(Exception e) {
				Tracing.Tracer.LogError(e);
			}
			return languageName;
		}
		protected virtual string CalculateFormattingCultureName(string userFormattingCultureName, string userLanguageName, string preferredLanguageName) {
			string treceMessage = Tracing.Tracer.GetMessageByValue("preferredLanguageName", preferredLanguageName, false);
			treceMessage += Tracing.Tracer.GetMessageByValue("userFormattingCultureName", userLanguageName, true);
			treceMessage += Tracing.Tracer.GetMessageByValue("selectedFormattingCultureName", selectedFormattingCultureName, true);
			Tracing.Tracer.LogText(treceMessage);
			if(string.IsNullOrEmpty(userLanguageName)) {
				Tracing.Tracer.LogWarning("User's language is empty");
			}
			if(!string.IsNullOrEmpty(selectedFormattingCultureName)) {
				return selectedFormattingCultureName;
			}
			if(!string.IsNullOrEmpty(userFormattingCultureName)) {
				return userFormattingCultureName;
			}
			string formattingCultureName = "";
			try {
				if(preferredLanguageName == CaptionHelper.DefaultLanguage
					|| preferredLanguageName == CaptionHelper.UserLanguage
					|| string.IsNullOrEmpty(preferredLanguageName)) {
					formattingCultureName = userLanguageName;
				}
				else if(!string.IsNullOrEmpty(preferredLanguageName)) {
					formattingCultureName = preferredLanguageName;
				}
			}
			catch(Exception e) {
				Tracing.Tracer.LogError(e);
			}
			return formattingCultureName;
		}
		protected virtual void SetLanguageCore(string languageName) {
			CustomizeLanguageEventArgs customizeLanguageArgs = new CustomizeLanguageEventArgs(languageName, GetUserCultureName(), GetPreferredLanguage());
			OnCustomizeLanguage(customizeLanguageArgs);
			if(!string.IsNullOrEmpty(customizeLanguageArgs.LanguageName)) {
				CurrentAspectProvider.CurrentAspect = customizeLanguageArgs.LanguageName;
			}
			Tracing.Tracer.LogVerboseValue("Thread.CurrentThread.CurrentUICulture.DisplayName", Thread.CurrentThread.CurrentUICulture.Name);
		}
		protected virtual void SetFormattingCultureCore(string formattingCultureName) {
			CultureInfo culture = Thread.CurrentThread.CurrentCulture;
			if(!string.IsNullOrEmpty(formattingCultureName)) {
				if(formattingCultureName == CultureInfo.InvariantCulture.TwoLetterISOLanguageName) {
					culture = CultureInfo.InvariantCulture;
				}
				else {
					culture = CreateSpecificCultureSafe(formattingCultureName, culture);
				}
			}
			if(culture.IsReadOnly) {
				culture = (CultureInfo)culture.Clone();
			}
			CustomizeFormattingCultureEventArgs customizeFormattingCultureArgs = new CustomizeFormattingCultureEventArgs(culture, GetUserCultureName(), GetPreferredLanguage());
			OnCustomizeFormattingCulture(customizeFormattingCultureArgs);
			if(customizeFormattingCultureArgs.FormattingCulture != null) {
				Thread.CurrentThread.CurrentCulture = customizeFormattingCultureArgs.FormattingCulture;
			}
			Tracing.Tracer.LogVerboseValue("Thread.CurrentThread.CurrentCulture.DisplayName", Thread.CurrentThread.CurrentCulture.Name);
		}
		protected void InitializeLanguage() {
			string languageName = CalculateLanguageName(GetUserCultureName(), GetPreferredLanguage());
			SetLanguageCore(languageName);
			string formattingCultureName = CalculateFormattingCultureName(GetUserFormattingCultureName(), GetUserCultureName(), GetPreferredLanguage());
			SetFormattingCultureCore(formattingCultureName);
		}
		protected virtual string GetUserCultureName() {
			return Thread.CurrentThread.CurrentUICulture.Name;
		}
		protected virtual string GetUserFormattingCultureName() {
			return Thread.CurrentThread.CurrentCulture.Name;
		}
		protected string GetPreferredLanguage() {
			string result;
			if(model != null) {
				result = model.PreferredLanguage;
			}
			else {
				result = CaptionHelper.UserLanguage;
			}
			return result;
		}
		protected virtual void OnDatabaseVersionMismatch(DatabaseVersionMismatchEventArgs args) {
			if(DatabaseVersionMismatch != null) {
				DatabaseVersionMismatch(this, args);
			}
		}
		protected virtual void OnCustomCheckCompatibility(CustomCheckCompatibilityEventArgs args) {
			if(CustomCheckCompatibility != null) {
				Tracing.Tracer.LogVerboseText("->CustomCheckCompatibility");
				CustomCheckCompatibility(this, args);
				Tracing.Tracer.LogVerboseText("<-CustomCheckCompatibility");
			}
		}
		protected virtual void OnListViewCreating(ListViewCreatingEventArgs args) {
			if(ListViewCreating != null) {
				ListViewCreating(this, args);
			}
		}
		protected virtual void OnListViewCreated(ListView listView) {
			if(ListViewCreated != null) {
				ListViewCreated(this, new ListViewCreatedEventArgs(listView));
			}
		}
		protected virtual void OnDetailViewCreating(DetailViewCreatingEventArgs args) {
			if(DetailViewCreating != null) {
				DetailViewCreating(this, args);
			}
		}
		protected virtual void OnDetailViewCreated(DetailView view) {
			if(DetailViewCreated != null) {
				DetailViewCreated(this, new DetailViewCreatedEventArgs(view));
			}
		}
		protected virtual void OnDashboardViewCreating(DashboardViewCreatingEventArgs args) {
			if(DashboardViewCreating != null) {
				DashboardViewCreating(this, args);
			}
		}
		protected virtual void OnDashboardViewCreated(DashboardView view) {
			if(DashboardViewCreated != null) {
				DashboardViewCreated(this, new DashboardViewCreatedEventArgs(view));
			}
		}
		protected internal virtual void OnViewShowing(Frame targetFrame, View view, Frame sourceFrame) {
			if(ViewShowing != null) {
				ViewShowing(this, new ViewShowingEventArgs(targetFrame, view, sourceFrame));
			}
		}
		protected internal virtual void OnViewShown(Frame frame, Frame sourceFrame) {
			if(ViewShown != null) {
				ViewShown(this, new ViewShownEventArgs(frame, sourceFrame));
			}
		}
		protected virtual void OnLoggingOn(LogonEventArgs args) {
			try {
				if(LoggingOn != null) {
					LoggingOn(this, args);
				}
			}
			catch(Exception e) {
				Tracing.Tracer.LogSubSeparator("LoggingOn exception:");
				Tracing.Tracer.LogError(e);
				throw;
			}
		}
		protected virtual void OnLoggedOn(LogonEventArgs args) {
			Tracing.Tracer.LogSubSeparator("Logon completed");
			try {
				if(LoggedOn != null) {
					LoggedOn(this, args);
				}
			}
			catch(Exception e) {
				Tracing.Tracer.LogSubSeparator("LoggedOn exception:");
				Tracing.Tracer.LogError(e);
				throw;
			}
		}
		protected virtual bool OnLogonFailed(object logonParameters, Exception e) {
			LogonFailedEventArgs logonExceptionArgs = new LogonFailedEventArgs(logonParameters, e);
			logonExceptionArgs.Handled = false;
			try {
				if(LogonFailed != null) {
					LogonFailed(this, logonExceptionArgs);
				}
			}
			catch(Exception exception) {
				Tracing.Tracer.LogSubSeparator("LogonFailed exception:");
				Tracing.Tracer.LogError(exception);
				throw;
			}
			return logonExceptionArgs.Handled;
		}
		protected virtual void OnLoggingOff(LoggingOffEventArgs args) {
			try {
				if(LoggingOff != null) {
					LoggingOff(this, args);
				}
			}
			catch(Exception exception) {
				Tracing.Tracer.LogSubSeparator("LoggingOff exception:");
				Tracing.Tracer.LogError(exception);
				throw;
			}
		}
		protected virtual void OnLoggedOff() {
			Tracing.Tracer.LogSubSeparator("Logoff completed");
			try {
				if(LoggedOff != null) {
					LoggedOff(this, EventArgs.Empty);
				}
			}
			catch(Exception exception) {
				Tracing.Tracer.LogSubSeparator("LoggedOff exception:");
				Tracing.Tracer.LogError(exception);
				throw;
			}
		}
		protected virtual void OnSettingUp(ExpressApplicationSetupParameters parameters) {
			if(SettingUp != null) {
				SettingUp(this, new SetupEventArgs(parameters));
			}
		}
		protected virtual void OnSetupStarted() {
		}
		protected virtual void OnSetupComplete() {
			if(SetupComplete != null) {
				SetupComplete(this, EventArgs.Empty);
			}
		}
		protected virtual void OnCustomProcessShortcut(CustomProcessShortcutEventArgs args) {
			if(CustomProcessShortcut != null) {
				CustomProcessShortcut(this, args);
			}
		}
		protected virtual bool OnHandleShortcutProcessingException(ViewShortcut shortcut, Exception shortcutProcessingException) {
			HandleShortcutProcessingExceptionEventArgs args = new HandleShortcutProcessingExceptionEventArgs(shortcut, shortcutProcessingException);
			if(HandleShortcutProcessingException != null) {
				HandleShortcutProcessingException(this, args);
			}
			return args.Handled;
		}
		protected virtual void OnObjectSpaceCreated(IObjectSpace objectSpace) {
			if(ObjectSpaceCreated != null) {
				ObjectSpaceCreated(this, new ObjectSpaceCreatedEventArgs(objectSpace));
			}
		}
		protected virtual void OnPropertyChanged(string propertyName) {
			if(PropertyChanged != null) {
				PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
				PropertyChanged(this, args);
			}
		}
		protected virtual void OnCustomizeFormattingCulture(CustomizeFormattingCultureEventArgs args) {
			if(CustomizeFormattingCulture != null) {
				CustomizeFormattingCulture(this, args);
			}
		}
		protected virtual void OnCustomizeLanguage(CustomizeLanguageEventArgs args) {
			if(CustomizeLanguage != null) {
				CustomizeLanguage(this, args);
			}
		}
		protected virtual void OnModelChanged() {
			if(ModelChanged != null) {
				ModelChanged(this, EventArgs.Empty);
			}
		}
		protected virtual IFrameTemplate OnCreateCustomTemplate(string name) {
			CreateCustomTemplateEventArgs args = new CreateCustomTemplateEventArgs(this, name);
			if(CreateCustomTemplate != null) {
				CreateCustomTemplate(this, args);
			}
			return args.Template;
		}
		protected virtual void OnInitializing() {
			isInitializingCounter++;
		}
		protected virtual void OnInitialized() {
			if(isInitializingCounter > 0) {
				isInitializingCounter--;
			}
			if(modules.FindModule(typeof(DevExpress.ExpressApp.SystemModule.SystemModule)) == null) {
				modules.AddRequiredModule<DevExpress.ExpressApp.SystemModule.SystemModule>("The module is a system module");
			}
		}
		protected virtual void OnCustomizeTemplate(IFrameTemplate frameTemplate, String templateContextName) {
			if(CustomizeTemplate != null) {
				CustomizeTemplate(this, new CustomizeTemplateEventArgs(frameTemplate, templateContextName));
			}
		}
		protected virtual void OnCustomizeLanguages(IList<string> languages) {
			if(CustomizeLanguagesList != null) {
				CustomizeLanguagesList(this, new CustomizeLanguagesListEventArgs(languages));
			}
		}
		protected virtual ModuleTypeList GetDefaultModuleTypes() {
			return new ModuleTypeList(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
		}
		protected virtual void CheckCompatibilityCore() {
			if(DatabaseUpdateMode != DatabaseUpdateMode.Never) {
				lock(compatibilityCheckLockObject) {
					foreach(IObjectSpaceProvider objectSpaceProvider in objectSpaceProviders) {
						if(objectSpaceProvider.SchemaUpdateMode != SchemaUpdateMode.None) {
							if(CheckCompatibilityType == ExpressApp.CheckCompatibilityType.DatabaseSchema || objectSpaceProvider.ModuleInfoType != null) {
								DatabaseUpdaterBase databaseUpdater = CreateDatabaseUpdater(objectSpaceProvider);
								try {
									databaseUpdater.StatusUpdating += new EventHandler<StatusUpdatingEventArgs>(DatabaseUpdater_StatusUpdating);
									CompatibilityError compatibilityError = databaseUpdater.CheckCompatibility();
									if(compatibilityError != null) {
										if(compatibilityError is CompatibilityDatabaseIsOldError || compatibilityError is CompatibilityUnableToOpenDatabaseError) {
											DatabaseVersionMismatchEventArgs args = new DatabaseVersionMismatchEventArgs(databaseUpdater, compatibilityError);
											OnDatabaseVersionMismatch(args);
											if(args.Handled) {
												compatibilityError = databaseUpdater.CheckCompatibility();
											}
										}
										if(compatibilityError != null) {
											throw new CompatibilityException(compatibilityError);
										}
									}
									else if(DatabaseUpdateMode == DatabaseUpdateMode.UpdateDatabaseAlways) {
										DatabaseVersionMismatchEventArgs args = new DatabaseVersionMismatchEventArgs(databaseUpdater, null);
										databaseUpdater.ForceUpdateDatabase = true;
										OnDatabaseVersionMismatch(args);
									}
								}
								finally {
									databaseUpdater.Dispose();
								}
							}
						}
					}
				}
			}
		}
		protected virtual string GetTraceLogDirectory() {
			return PathHelper.GetApplicationFolder();
		}
		protected virtual ControllersManager CreateControllersManager() {
			return new ControllersManager();
		}
		protected virtual ApplicationModulesManager CreateApplicationModulesManager(ControllersManager controllersManager) {
			if(controllersManager == null) {
				throw new ArgumentNullException("controllersManager");
			}
			return new ApplicationModulesManager(controllersManager, "");
		}
		protected virtual ListEditor CreateListEditorCore(IModelListView modelListView, CollectionSourceBase collectionSource) {
			return EditorFactory.CreateListEditor(modelListView, this, collectionSource);
		}
		protected virtual IFrameTemplate CreateDefaultTemplate(TemplateContext context) { return null; }
		protected virtual ShowViewStrategyBase CreateShowViewStrategy() {
			return null;
		}
		protected virtual void OnShowViewStrategyChanged() {
		}
		protected virtual Window CreateWindowCore(TemplateContext context, ICollection<Controller> controllers, bool isMain, bool activateControllersImmediatelly) {
			Tracing.Tracer.LogVerboseValue("XafApplication.CreateWindowCore.activateControllersImmediatelly", activateControllersImmediatelly);
			return new Window(this, context, controllers, isMain, activateControllersImmediatelly);
		}
		protected virtual Window CreatePopupWindowCore(TemplateContext context, ICollection<Controller> controllers) {
			return new Window(this, context, controllers, false, true);
		}
		protected PopupWindowShowAction CreateLogonAction() {
			PopupWindowShowAction showLogonAction = new PopupWindowShowAction(null);
			showLogonAction.Application = this;
			showLogonAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(showLogonAction_OnCustomizePopupWindowParams);
			showLogonAction.Execute += new PopupWindowShowActionExecuteEventHandler(showLogonAction_OnExecute);
			return showLogonAction;
		}
		protected virtual LogonController CreateLogonController() {
			return CreateController<LogonController>();
		}
		protected virtual List<Controller> CreateLogonWindowControllers() {
			CreateCustomLogonWindowControllersEventArgs args = new CreateCustomLogonWindowControllersEventArgs();
			if(CreateCustomLogonWindowControllers != null) {
				CreateCustomLogonWindowControllers(this, args);
			}
			List<Controller> result = new List<Controller>(new Controller[] {
				CreateController<ActionControlsSiteController>(),
				CreateController<FillActionContainersController>()
			});
			result.AddRange(args.Controllers);
			return result;
		}
		protected virtual IObjectSpace CreateLogonWindowObjectSpace(object logonParameters) {
			CreateCustomLogonWindowObjectSpaceEventArgs args = new CreateCustomLogonWindowObjectSpaceEventArgs(logonParameters);
			if(CreateCustomLogonWindowObjectSpace != null) {
				CreateCustomLogonWindowObjectSpace(this, args);
			}
			if(args.ObjectSpace == null) {
				args.ObjectSpace = new NonPersistentObjectSpace(typesInfo);
			}
			return args.ObjectSpace;
		}
		protected virtual String GetDcAssemblyFilePath() {
			return null;
		}
		protected virtual String GetModelAssemblyFilePath() {
			return null;
		}
		protected virtual String GetModulesVersionInfoFilePath() {
			return null;
		}
		protected virtual ModelDifferenceStore CreateDeviceSpecificModelDifferenceStoreCore() {
			return null;
		}
		protected virtual ModelDifferenceStore CreateUserModelDifferenceStoreCore() {
			return null;
		}
		protected virtual SettingsStorage CreateLogonParameterStoreCore() {
			return null;
		}
		protected virtual ModelDifferenceStore CreateModelDifferenceStoreCore() {
			return null;
		}
		protected virtual CollectionSourceBase CreateCollectionSourceCore(
				IObjectSpace objectSpace, Type objectType, CollectionSourceDataAccessMode dataAccessMode, CollectionSourceMode mode) {
			return new CollectionSource(objectSpace, objectType, dataAccessMode, mode);
		}
		protected virtual PropertyCollectionSource CreatePropertyCollectionSourceCore(
				IObjectSpace objectSpace, Type masterObjectType, Object masterObject, IMemberInfo memberInfo, CollectionSourceDataAccessMode dataAccessMode, CollectionSourceMode mode) {
			return new PropertyCollectionSource(objectSpace, masterObjectType, masterObject, memberInfo, dataAccessMode, mode);
		}
		protected abstract LayoutManager CreateLayoutManagerCore(bool simple);
		protected virtual void ExitCore() { }
		protected SettingsStorage CreateLogonParameterStore() {
			CreateCustomLogonParameterStoreEventArgs args = new CreateCustomLogonParameterStoreEventArgs();
			if(CreateCustomLogonParameterStore != null) {
				CreateCustomLogonParameterStore(this, args);
			}
			if(args.Handled) {
				return args.Storage;
			}
			else {
				return CreateLogonParameterStoreCore();
			}
		}
		protected virtual void ReadLastLogonParametersCore(SettingsStorage storage, object logonObject) {
			if(storage == null) {
				throw new ArgumentNullException("storage");
			}
			LastLogonParametersReadingEventArgs argsReading = new LastLogonParametersReadingEventArgs(logonObject, storage);
			if(LastLogonParametersReading != null) {
				LastLogonParametersReading(this, argsReading);
			}
			if(!argsReading.Handled) {
				ObjectSerializer.ReadObjectPropertyValues(argsReading.SettingsStorage, logonObject);
			}
			if(LastLogonParametersRead != null) {
				LastLogonParametersReadEventArgs argsRead = new LastLogonParametersReadEventArgs(logonObject, storage);
				LastLogonParametersRead(this, argsRead);
			}
		}
		protected void ReadLastLogonParameters(object logonObject) {
			SettingsStorage storage = CreateLogonParameterStore();
			if(storage != null) {
				ReadLastLogonParametersCore(storage, logonObject);
			}
		}
		protected virtual void WriteLastLogonParametersCore(SettingsStorage storage, DetailView view, object logonObject) {
			if(storage == null) {
				throw new ArgumentNullException("storage");
			}
			LastLogonParametersWritingEventArgs argsWriting = new LastLogonParametersWritingEventArgs(view, logonObject, storage);
			if(LastLogonParametersWriting != null) {
				LastLogonParametersWriting(this, argsWriting);
			}
			if(!argsWriting.Handled) {
				ObjectSerializer.WriteObjectPropertyValues(view, argsWriting.SettingsStorage, logonObject);
			}
		}
		protected void WriteLastLogonParameters(DetailView view, object logonObject) {
			SettingsStorage storage = CreateLogonParameterStore();
			if(storage != null) {
				WriteLastLogonParametersCore(storage, view, logonObject);
			}
		}
		protected virtual void Logon(PopupWindowShowActionExecuteEventArgs logonWindowArgs) {
			Tracing.Tracer.LogSubSeparator("Start logon process");
			if(isLoggedOn) {
				Tracing.Tracer.LogText("The user already logged on");
			}
			LogonEventArgs args = new LogonEventArgs(SecuritySystem.LogonParameters);
			try {
				OnLoggingOn(args);
				CheckCompatibility();
				try {
					IObjectSpace logonObjectSpace = CreateObjectSpace(SecuritySystem.Instance.UserType);
					securitySystemObjectSpaces.Add(logonObjectSpace);
					SecuritySystem.Instance.Logon(logonObjectSpace);
					if(logonWindowArgs != null) {
						logonWindowArgs.CanCloseWindow = true;
						if(logonWindowArgs.PopupWindow != null && SecuritySystem.LogonParameters != null) {
							WriteLastLogonParameters((DetailView)logonWindowArgs.PopupWindow.View, SecuritySystem.LogonParameters);
						}
					}
					if((SecuritySystem.Instance is IObjectSpaceLinks) && (objectSpaceProviders.Count > 1)) {
						foreach(IObjectSpaceProvider objectSpaceProvider in objectSpaceProviders) {
							IObjectSpace securitySystemObjectSpace = objectSpaceProvider.CreateObjectSpace();
							securitySystemObjectSpaces.Add(securitySystemObjectSpace);
							((IObjectSpaceLinks)SecuritySystem.Instance).ObjectSpaces.Add(securitySystemObjectSpace);
						}
					}
					Tracing.Tracer.LogSubSeparator("Logon successful");
					isLoggedOn = true;
					string preferredLanguageBeforeUserDiffs = GetPreferredLanguage();
					LoadUserDifferences();
					EnsureShowViewStrategy();
					if(preferredLanguageBeforeUserDiffs != GetPreferredLanguage()) {
						InitializeLanguage();
					}
					OnLoggedOn(args);
				}
				finally {
					SecuritySystem.Instance.ClearSecuredLogonParameters();
				}
			}
			catch(ThreadAbortException) {
			}
			catch(Exception sourceException) {
				Tracing.Tracer.LogSubSeparator("Logon failed");
				Tracing.Tracer.LogError(sourceException);
				bool isLogonFailedHandled = false;
				try {
					isLoggedOn = false;
					if(SecuritySystem.Instance.IsAuthenticated) {
						SecuritySystem.Instance.Logoff();
						if(SecuritySystem.LogonParameters != null) {
							ReadLastLogonParameters(SecuritySystem.LogonParameters);
						}
						if((logonWindowArgs != null) && (logonWindowArgs.PopupWindow != null) && (logonWindowArgs.PopupWindow.View is ObjectView)) {
							((ObjectView)logonWindowArgs.PopupWindow.View).CurrentObject = SecuritySystem.LogonParameters;
						}
					}
					foreach(IObjectSpace securitySystemObjectSpace in securitySystemObjectSpaces) {
						securitySystemObjectSpace.Dispose();
					}
					securitySystemObjectSpaces.Clear();
					isLogonFailedHandled = OnLogonFailed(SecuritySystem.LogonParameters, sourceException);
				}
				catch(Exception catchSectionException) {
					catchSectionException.Data.Add(XafApplicationLogonCatchExceptionKey, sourceException);
					throw catchSectionException;
				}
				if(!isLogonFailedHandled) {
					throw;
				}
			}
		}
		protected virtual void LoadUserDifferences() {
			CreateCustomModelDifferenceStoreEventArgs args = new CreateCustomModelDifferenceStoreEventArgs();
			CreateUserModelDifferenceStore(args);
			List<IModelApplication> layers = new List<IModelApplication>();
			if(userAppDiffLayer != null) {
				layers.Add((IModelApplication)userAppDiffLayer);
			}
			layers.Add((IModelApplication)afterSetupLayer);
			foreach(KeyValuePair<string, ModelStoreBase> item in args.ExtraDiffStores) {
				if(item.Value != null) {
					layers.Add((IModelApplication)modelManager.CreateLayerByStore(item.Key, item.Value));
				}
			}
			if(args.Store != null) {
				layers.Add((IModelApplication)modelManager.CreateLayerByStore(ModelApplicationLayerIds.UserDiffs, args.Store));
			}
			CustomizeUserDifferences(layers);   
			CheckCustomizedUserDifferences(layers);
			SetupModelApplication(layers.Cast<ModelApplicationBase>());
			if(UserDifferencesLoaded != null) {
				UserDifferencesLoaded(this, EventArgs.Empty);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void CustomizeUserDifferences(IList<IModelApplication> layers) { }
		private void CheckCustomizedUserDifferences(IList<IModelApplication> layers) {  
			if(layers.Count == 0) {
				throw new InvalidOperationException("The collection is empty");
			}
			for(int i = 0; i < layers.Count; ++i) {
				if(layers[i] == null) {
					throw new InvalidOperationException(string.Format("At least one item in the collection is null (index: {0})", i));
				}
			}
			for(int i = 0; i < layers.Count; ++i) {
				IModelApplication layer1 = layers[i];
				for(int j = 0; j < i; ++j) {
					IModelApplication layer2 = layers[j];
					if(layer1 == layer2) {
						throw new InvalidOperationException(string.Format("At least one item is present in the collection twice (id: {0})", ((ModelApplicationBase)layer1).Id));
					}
				}
			}
		}
		protected virtual void DisposeCore() { }
		protected virtual void OnCreateCustomObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
			if(CreateCustomObjectSpaceProvider != null) {
				CreateCustomObjectSpaceProvider(this, args);
			}
		}
		protected virtual IObjectSpace CreateObjectSpaceCore(Type objectType) {
			IObjectSpace result = null;
			if(objectType != null) {
				foreach(IObjectSpaceProvider objectSpaceProvider in objectSpaceProviders) {
					Type originalObjectType = objectSpaceProvider.EntityStore.GetOriginalType(objectType);
					if((originalObjectType != null) && objectSpaceProvider.EntityStore.RegisteredEntities.Contains(originalObjectType)) {
						result = objectSpaceProvider.CreateObjectSpace();
						break;
					}
				}
			}
			if((result == null) && (objectSpaceProviders.Count > 0)) {
				result = objectSpaceProviders[0].CreateObjectSpace();
			}
			return result;
		}
		protected override void Dispose(Boolean disposing) {
			if(isDisposed) {
				return;
			}
			DashboardViewCreating = null;
			DashboardViewCreated = null;
			StatusUpdating = null;
			UserDifferencesLoaded = null;
			DatabaseUpdaterCreating = null;
			CustomizeLanguagesList = null;
			DatabaseVersionMismatch = null;
			CustomCheckCompatibility = null;
			CustomizeLanguage = null;
			CustomizeFormattingCulture = null;
			CreateCustomTemplate = null;
			CustomizeTemplate = null;
			CustomizeTableName = null;
			CreateCustomUserModelDifferenceStore = null;
			CreateCustomModelDifferenceStore = null;
			CreateCustomDeviceSpecificModelDifferenceStore = null;
			CustomProcessShortcut = null;
			HandleShortcutProcessingException = null;
			ViewCreating = null;
			ViewCreated = null;
			DetailViewCreating = null;
			DetailViewCreated = null;
			ListViewCreating = null;
			ListViewCreated = null;
			ViewShowing = null;
			ViewShown = null;
			ObjectSpaceCreated = null;
			CreateCustomObjectSpaceProvider = null;
			SettingUp = null;
			SetupComplete = null;
			CreateCustomPropertyCollectionSource = null;
			CreateCustomCollectionSource = null;
			LastLogonParametersReading = null;
			LastLogonParametersRead = null;
			LastLogonParametersWriting = null;
			CreateCustomLogonParameterStore = null;
			CreateCustomLogonWindowControllers = null;
			CreateCustomLogonWindowObjectSpace = null;
			LoggingOn = null;
			LoggedOn = null;
			LoggingOff = null;
			LoggedOff = null;
			LogonFailed = null;
			PropertyChanged = null;
			ModelChanged = null;
#if DebugTest
			DevExpress.ExpressApp.Tests.BaseXafTest.DisposableObjects.Remove(this);
#endif
			SafeExecutor executor = new SafeExecutor(this);
			isDisposed = true;
			if(disposing) {
				executor.Execute(delegate() {
					DisposeCore();
				});
				controllersManager = null;
				if(showViewStrategy != null) {
					executor.Dispose(showViewStrategy);
					showViewStrategy = null;
				}
				if(securitySystemObjectSpaces != null) {
					foreach(IObjectSpace securitySystemObjectSpace in securitySystemObjectSpaces) {
						executor.Dispose(securitySystemObjectSpace);
					}
					securitySystemObjectSpaces.Clear();
					securitySystemObjectSpaces = null;
				}
				if(modules != null) {
					foreach(ModuleBase module in modules) {
						executor.Dispose(module);
					}
					modules.Clear();
					modules = null;
				}
				if(Security != null) {
					if(Security is IDisposable) {
						((IDisposable)Security).Dispose();
					}
					Security = null;
				}
			}
			if(connection != null) {
				if(IsConnectionOwner && disposing) {
					executor.Dispose(connection);
				}
				connection = null;
			}
			if(objectSpaceProviders.Count > 0) {
				if(isObjectSpaceProviderOwner && disposing) {
					foreach(IObjectSpaceProvider objectSpaceProvider in objectSpaceProviders) {
						if(objectSpaceProvider is IDisposable) {
							executor.Dispose((IDisposable)objectSpaceProvider);
						}
					}
				}
				objectSpaceProviders.Clear();
			}
			modelManager = null;
			model = null;
			typesInfo = null;
			currentAspectProvider = null;
			userAppDiffLayer = null;
			afterSetupLayer = null;
			base.Dispose(disposing);
			executor.ThrowExceptionIfAny();
		}
		protected virtual void InitializeSecuritySystem(ExpressApplicationSetupParameters parameters) {
			lock(DevExpress.ExpressApp.DC.TypesInfo.lockObject) {
				SecuritySystem.SetInstance(parameters.Security);
			}
		}
		protected virtual void ActivateResourceLocalizers(IModelApplication model) { 
			foreach(IXafResourceLocalizer localizer in ((IModelSources)model).Localizers) {
				localizer.Setup(model);
			}
		}
		protected virtual void InitializeCurrentAspectProvider(ModelApplicationBase result) {
			result.CurrentAspectProvider = CurrentAspectProvider;
		}
		protected virtual void InitializeCaptionHelper(IModelApplication model) {
			CaptionHelper.Setup(model);
		}
		protected virtual bool CanLoadTypesInfo() {
			return true;
		}
		protected virtual ApplicationModelManager CreateModelManager(IEnumerable<Type> boModelTypes) {
			ApplicationModelManager modelManager = new ApplicationModelManager();
			modelManager.Setup(typesInfo, boModelTypes, Modules, ControllersManager.Controllers, localizerTypes, GetApplicationAspects(), CreateModelDifferenceStore(), GetModelAssemblyFilePath());
			return modelManager;
		}
		protected virtual void OnStatusUpdating(String context, String title, String message, params Object[] additionalParams) {
			if(StatusUpdating != null) {
				StatusUpdating(this, new StatusUpdatingEventArgs(context, title, message, additionalParams));
			}
		}
		protected virtual void OnShortcutProcessed(ViewShortcut shortcut, View view) {
		}
		protected ModelDifferenceStore CreateModelDifferenceStore() {
			if(CreateCustomModelDifferenceStore != null) {
				CreateCustomModelDifferenceStoreEventArgs args = new CreateCustomModelDifferenceStoreEventArgs();
				CreateCustomModelDifferenceStore(this, args);
				if(args.Handled) {
					return args.Store;
				}
			}
			return CreateModelDifferenceStoreCore();
		}
		protected ModelDifferenceStore CreateDeviceSpecificModelDifferenceStore() {
			if(CreateCustomDeviceSpecificModelDifferenceStore != null) {
				CreateCustomModelDifferenceStoreEventArgs args = new CreateCustomModelDifferenceStoreEventArgs();
				CreateCustomDeviceSpecificModelDifferenceStore(this, args);
				if(args.Handled) {
					return args.Store;
				}
			}
			return CreateDeviceSpecificModelDifferenceStoreCore();
		}
		protected ModelDifferenceStore CreateUserModelDifferenceStore() {
			CreateCustomModelDifferenceStoreEventArgs args = new CreateCustomModelDifferenceStoreEventArgs();
			CreateUserModelDifferenceStore(args);
			return args.Store;
		}
		protected void EnsureShowViewStrategy() {
			if(showViewStrategy == null) {
				showViewStrategy = CreateShowViewStrategy();
				OnShowViewStrategyChanged();
			}
		}
		public XafApplication(ITypesInfo typesInfo) {
			Initialize(typesInfo);
		}
		public XafApplication()
			: this(XafTypesInfo.Instance) {
		}
		public virtual String GetDiffDefaultName(String storePath) {
			return ModelDifferenceStore.AppDiffDefaultName;
		}
		public ViewShortcut GetCompletedViewShortcut(ViewShortcut shortcut) {
			string viewId = shortcut.ViewId;
			Type type = shortcut.ObjectClass;
			if(string.IsNullOrEmpty(viewId) && type != null) {
				viewId = FindDetailViewId(type);
			}
			if((type == null) && (viewId != null)) {
				IModelView modelView = FindModelView(viewId);
				if(modelView is IModelObjectView) {
					type = ((IModelObjectView)modelView).ModelClass.TypeInfo.Type;
				}
			}
			ViewShortcut result = new ViewShortcut(type, shortcut.ObjectKey, viewId);
			shortcut.CopyTo(result);
			result.ViewId = viewId;
			if(type != null) {
				result.ObjectClassName = type.FullName;
			}
			result.ObjectKey = shortcut.ObjectKey;
			return result;
		}
		public void CheckCompatibility() {
			CustomCheckCompatibilityEventArgs args = new CustomCheckCompatibilityEventArgs(isCompatibilityChecked, (objectSpaceProviders.Count > 0) ? objectSpaceProviders[0] : null, Modules, applicationName);
			OnCustomCheckCompatibility(args);
			if(!args.Handled && !isCompatibilityChecked) {
				Tracing.Tracer.LogSubSeparator("Checking Compatibility");
				foreach(IObjectSpaceProvider objectSpaceProvider in objectSpaceProviders) {
					Tracing.Tracer.LogValue("ConnectionString", objectSpaceProvider.ConnectionString);
				}
				CheckCompatibilityCore();
				Tracing.Tracer.LogVerboseText("Compatibility is checked");
			}
			isCompatibilityChecked = true;
		}
		public void Setup() {
			if(!DesignMode) {
				Setup("", "", new String[0], null);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void Setup(ExpressApplicationSetupParameters parameters) {
			OnSettingUp(parameters);
			ObjectAccessComparerBase.LockCurrentComparer();
			applicationName = parameters.ApplicationName;
			if(parameters.ObjectSpaceProviders.Count > 0) {
				AddObjectSpaceProviders(parameters.ObjectSpaceProviders);
			}
			else {
				if(!String.IsNullOrEmpty(parameters.ConnectionString)) {
					AddObjectSpaceProviders(CreateObjectSpaceProviders(parameters.ConnectionString));
				}
				else if(objectSpaceProviders.Count == 0) {
					throw new ArgumentNullException("ObjectSpaceProvider and ConnectionString");
				}
			}
			AddEntityStoresIntoTypesInfo();
			RegisterModuleInfoTypes();
			controllersManager = parameters.ControllersManager;
			Guard.ArgumentNotNull(parameters.Modules, "parameters.Modules");
			modules = parameters.Modules;
			InitializeSecuritySystem(parameters); 
			modelManager = CreateModelManager(parameters.DomainComponents); 
			ModelDifferenceStore userAppDiffStore = CreateDeviceSpecificModelDifferenceStore();
			if(userAppDiffStore != null) {
				userAppDiffLayer = modelManager.CreateLayerByStore(userAppDiffStore.Name, userAppDiffStore);
			}
			afterSetupLayer = modelManager.CreateLayer(ModelApplicationLayerIds.AfterSetup);
			SetupModelApplication(ModelDifferenceLayers);
			if(!IsActualModulesVersionInfo()) {
				UpdateModulesVersionInfo();
			}
			OnSetupComplete();
		}
		public void Setup(String applicationName, IObjectSpaceProvider objectSpaceProvider) {
			Setup(applicationName, objectSpaceProvider, new String[0], null);
		}
		public void Setup(String applicationName, String connectionString, String[] moduleAssemblies) {
			Setup(applicationName, connectionString, moduleAssemblies, null);
		}
		public void Setup(String applicationName, String connectionString, String[] moduleAssemblies, ISecurityStrategyBase security) {
			Setup(applicationName, CreateObjectSpaceProviders(connectionString), moduleAssemblies, security);
		}
		public void Setup(String applicationName, IObjectSpaceProvider objectSpaceProvider, String[] moduleAssemblies, ISecurityStrategyBase security) {
			Setup(applicationName, new IObjectSpaceProvider[] { objectSpaceProvider }, moduleAssemblies, security);
		}
		public void Setup(String applicationName, IObjectSpaceProvider objectSpaceProvider, ApplicationModulesManager modulesManager, ISecurityStrategyBase security) {
			Setup(applicationName, new IObjectSpaceProvider[] { objectSpaceProvider }, modulesManager, security);
		}
		public void Setup(String applicationName, IList<IObjectSpaceProvider> objectSpaceProviders, String[] moduleAssemblies, ISecurityStrategyBase security) {
			Setup(applicationName, objectSpaceProviders, CreateModuleManager(moduleAssemblies), security);
		}
		public void Setup(String applicationName, IList<IObjectSpaceProvider> objectSpaceProviders, ApplicationModulesManager modulesManager, ISecurityStrategyBase security) {
			try {
				Tick.In("XafApplication.Setup()");
				OnSetupStarted();
				if(string.IsNullOrEmpty(applicationName)) {
					applicationName = this.applicationName;
				}
				if(security != null) {
					this.Security = security;
				}
				if(this.Security == null) {
					this.Security = new SecurityDummy();
				}
				AddObjectSpaceProviders(objectSpaceProviders);
				AddEntityStoresIntoTypesInfo();
				RegisterModuleInfoTypes();
				modulesManager.Security = this.Security;
				modulesManager.CustomizeTypesInfo += new EventHandler<CustomizeTypesInfoEventArgs>(modulesManager_CustomizeTypesInfo);
				try {
					modulesManager.Load(typesInfo, CanLoadTypesInfo());
				}
				finally {
					modulesManager.CustomizeTypesInfo -= new EventHandler<CustomizeTypesInfoEventArgs>(modulesManager_CustomizeTypesInfo);
				}
				if(!IsActualModulesVersionInfo()) {
					ResetGeneratedAssemblies();
				}
				typesInfo.GenerateEntities(GetDcAssemblyFilePath());
				LocalizedClassInfoTypeConverter.ClassCaptionProvider = new CaptionHelperClassCaptionProvider();
				ExpressApplicationSetupParameters setupParameters =
					new ExpressApplicationSetupParameters(applicationName, modulesManager.Security, objectSpaceProviders, modulesManager.ControllersManager, modulesManager.Modules);
				setupParameters.DomainComponents = modulesManager.DomainComponents;
				Setup(setupParameters);
			}
			finally {
				Tick.Out("XafApplication.Setup()");
			}
		}
		public virtual DatabaseUpdaterBase CreateDatabaseUpdater(IObjectSpaceProvider objectSpaceProvider) {
			DatabaseUpdaterBase databaseUpdater = null;
			CheckCompatibilityType updaterType = GetCheckCompatibilityType(objectSpaceProvider);
			switch(updaterType) {
				case CheckCompatibilityType.DatabaseSchema:
					databaseUpdater = new DatabaseSchemaUpdater(objectSpaceProvider, Modules);
					break;
				default:
				case CheckCompatibilityType.ModuleInfo:
					if(objectSpaceProvider.ModuleInfoType != null) {
						databaseUpdater = new DatabaseUpdater(objectSpaceProvider, Modules, ApplicationName, objectSpaceProvider.ModuleInfoType);
					}
					break;
			}
			DatabaseUpdaterEventArgs args = new DatabaseUpdaterEventArgs(databaseUpdater);
			if(DatabaseUpdaterCreating != null) {
				DatabaseUpdaterCreating(this, args);
			}
			return args.Updater;
		}
		public CollectionSourceBase CreateCollectionSource(
			IObjectSpace objectSpace, Type objectType, String listViewId, CollectionSourceDataAccessMode dataAccessMode, CollectionSourceMode mode) {
			CollectionSourceBase result = null;
			if(CreateCustomCollectionSource != null) {
				CreateCustomCollectionSourceEventArgs args = new CreateCustomCollectionSourceEventArgs(objectSpace, objectType, listViewId, dataAccessMode, mode);
				CreateCustomCollectionSource(this, args);
				result = args.CollectionSource;
			}
			if(result == null) {
				if((dataAccessMode == CollectionSourceDataAccessMode.Server) && isAsyncServerMode) {
					result = new CollectionSource(objectSpace, objectType, dataAccessMode, isAsyncServerMode, mode);
				}
				else {
					result = CreateCollectionSourceCore(objectSpace, objectType, dataAccessMode, mode);
				}
			}
			return result;
		}
		public CollectionSourceBase CreateCollectionSource(
			IObjectSpace objectSpace, Type objectType, String listViewId, CollectionSourceMode mode) {
			return CreateCollectionSource(objectSpace, objectType, listViewId, GetDataAccessMode(listViewId), mode);
		}
		public CollectionSourceBase CreateCollectionSource(
			IObjectSpace objectSpace, Type objectType, String listViewId) {
			return CreateCollectionSource(objectSpace, objectType, listViewId, GetDataAccessMode(listViewId), defaultCollectionSourceMode);
		}
		public CollectionSourceBase CreateCollectionSource(
			IObjectSpace objectSpace, Type objectType, String listViewId, Boolean isServerMode, CollectionSourceMode mode) {
			return CreateCollectionSource(objectSpace, objectType, listViewId, isServerMode ? CollectionSourceDataAccessMode.Server : CollectionSourceDataAccessMode.Client, mode);
		}
		public PropertyCollectionSource CreatePropertyCollectionSource(
			IObjectSpace objectSpace, Type masterObjectType, Object masterObject, IMemberInfo memberInfo, String listViewId, CollectionSourceDataAccessMode dataAccessMode, CollectionSourceMode mode) {
			PropertyCollectionSource result = null;
			if(CreateCustomPropertyCollectionSource != null) {
				CreateCustomPropertyCollectionSourceEventArgs args = new CreateCustomPropertyCollectionSourceEventArgs(
					objectSpace, masterObjectType, masterObject, memberInfo, listViewId, dataAccessMode, mode);
				CreateCustomPropertyCollectionSource(this, args);
				result = args.PropertyCollectionSource;
			}
			if(result == null) {
				result = CreatePropertyCollectionSourceCore(objectSpace, masterObjectType, masterObject, memberInfo, dataAccessMode, mode);
			}
			return result;
		}
		public PropertyCollectionSource CreatePropertyCollectionSource(
			IObjectSpace objectSpace, Type masterObjectType, Object masterObject, IMemberInfo memberInfo, String listViewId, CollectionSourceMode mode) {
			return CreatePropertyCollectionSource(objectSpace, masterObjectType, masterObject, memberInfo, listViewId, GetDataAccessMode(listViewId), mode);
		}
		public PropertyCollectionSource CreatePropertyCollectionSource(
			IObjectSpace objectSpace, Type masterObjectType, Object masterObject, IMemberInfo memberInfo, String listViewId) {
			CollectionSourceMode collectionSourceMode = defaultCollectionSourceMode;
			CollectionSourceModeAttribute collectionSourceModeAttribute = memberInfo.FindAttribute<CollectionSourceModeAttribute>();
			if(collectionSourceModeAttribute != null) {
				collectionSourceMode = collectionSourceModeAttribute.Mode;
			}
			return CreatePropertyCollectionSource(objectSpace, masterObjectType, masterObject, memberInfo, listViewId, GetDataAccessMode(listViewId), collectionSourceMode);
		}
		public PropertyCollectionSource CreatePropertyCollectionSource(
			IObjectSpace objectSpace, Type masterObjectType, Object masterObject, IMemberInfo memberInfo, String listViewId, Boolean isServerMode, CollectionSourceMode mode) {
			return CreatePropertyCollectionSource(objectSpace, masterObjectType, masterObject, memberInfo, listViewId, isServerMode ? CollectionSourceDataAccessMode.Server : CollectionSourceDataAccessMode.Client, mode);
		}
		public void Exit() {
			ExitCore();
		}
		public virtual ConfirmationResult AskConfirmation(ConfirmationType confirmationType) {
			Tracing.Tracer.LogText("ConfirmationRequest: {0}", confirmationType.ToString());
			ConfirmationResult result = ConfirmationResult.Cancel;
			switch(confirmationType) {
				case ConfirmationType.CancelChanges: {
						result = ConfirmationResult.Yes;
						break;
					}
				case ConfirmationType.NeedSaveChanges: {
						result = ConfirmationResult.No;
						break;
					}
			}
			Tracing.Tracer.LogValue("DefaultConfirmationResult", result);
			return result;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Boolean ShowDetailViewFrom(Frame sourceFrame) {
			if(sourceFrame != null) {
				View sourceView = sourceFrame.View;
				DetailView detailView = sourceView as DetailView;
				ListView listView = sourceView as ListView;
				return (sourceView == null)
					|| (detailView != null)
					|| ((listView != null) && (listView.EditView == null))
					|| sourceView.IsRoot;
			}
			return true;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual IObjectSpace GetObjectSpaceToShowDetailViewFrom(Frame sourceFrame, Type objectType, TargetWindow targetWindow) {
			ListView listView = sourceFrame.View as ListView;
			DetailView detailView = sourceFrame.View as DetailView;
			if((listView != null)
					&& (listView.CollectionSource is PropertyCollectionSource)
					&& ((PropertyCollectionSource)listView.CollectionSource).MemberInfo.IsAggregated
					&& (listView.EditView == null)
					&& UseNestedObjectSpace(sourceFrame, targetWindow, objectType)) {
				return CreateNestedObjectSpace(listView.ObjectSpace);
			}
			if((detailView != null) && (detailView.ObjectSpace is INestedObjectSpace) && UseNestedObjectSpace(sourceFrame, targetWindow, objectType)) {
				return CreateNestedObjectSpace(((INestedObjectSpace)detailView.ObjectSpace).ParentObjectSpace);
			}
			return CreateObjectSpace(objectType);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual IObjectSpace GetObjectSpaceToShowDetailViewFrom(Frame sourceFrame, Type objectType) {
			return GetObjectSpaceToShowDetailViewFrom(sourceFrame, objectType, TargetWindow.Default);
		}
		public LayoutManager CreateLayoutManager(Boolean simple) {
			return CreateLayoutManagerCore(simple);
		}
		public View ProcessShortcut(ViewShortcut shortcut) {
			ViewShortcut shortcutCopy = new ViewShortcut();
			shortcutCopy.Clear();
			shortcut.CopyTo(shortcutCopy);
			try {
				CustomProcessShortcutEventArgs args = new CustomProcessShortcutEventArgs(shortcutCopy);
				OnCustomProcessShortcut(args);
				View result = null;
				if(args.Handled) {
					result = args.View;
				}
				else {
					ViewParameters viewParameters = CreateViewParameters(shortcutCopy);
					IObjectSpace objectSpace = viewParameters.ObjectSpace;
					if(viewParameters is DashboardViewParameters) {
						result = CreateDashboardView(objectSpace, shortcutCopy.ViewId, true);
					}
					else if(viewParameters is ListViewParameters) {
						result = CreateListView(shortcutCopy.ViewId, (IModelListView)null, CreateCollectionSource(objectSpace, viewParameters.ObjectType, shortcutCopy.ViewId), true);
						if(viewParameters.ObjectKey != null) {
							((ListView)result).CurrentObject = objectSpace.GetObjectByKey(viewParameters.ObjectType, viewParameters.ObjectKey);
						}
					}
					else if(viewParameters is DetailViewParameters) {
						Object obj = objectSpace.GetObjectByKey(viewParameters.ObjectType, viewParameters.ObjectKey);
						if(obj == null) {
							throw new UserFriendlyException(UserVisibleExceptionId.RequestedObjectIsNotFound);
						}
						result = CreateDetailView(objectSpace, viewParameters.ViewId, true, obj);
						String mode = "";
						if(shortcut.TryGetValue("mode", out mode) && (mode != null) && (mode.ToLower() == "edit")) {
							((DetailView)result).ViewEditMode = ViewEditMode.Edit;
						}
					}
					else {
						throw new InvalidOperationException();
					}
					result.ScrollPosition = viewParameters.ScrollPosition;
				}
				OnShortcutProcessed(shortcut, result);
				return result;
			}
			catch(Exception e) {
				if(!OnHandleShortcutProcessingException(shortcutCopy, e)) {
					throw;
				}
				return null;
			}
		}
		public ListEditor CreateListEditor(CollectionSourceBase collectionSource, IModelListView modelListView) {
			ListEditor result = CreateListEditorCore(modelListView, collectionSource);
			if(result == null) {
				throw new Exception("Cannot create ListEditor");
			}
			return result;
		}
		public ListView CreateListView(IModelListView modelListView, CollectionSourceBase collectionSource, Boolean isRoot) {
			return CreateListView(modelListView, collectionSource, isRoot, null);
		}
		public ListView CreateListView(IModelListView modelListView, CollectionSourceBase collectionSource, Boolean isRoot, ListEditor listEditor) {
			if(modelListView == null) {
				throw new ArgumentNullException("modelListView");
			}
			return CreateListView(modelListView.Id, modelListView, collectionSource, isRoot, listEditor);
		}
		public ListView CreateListView(String listViewId, CollectionSourceBase collectionSource, Boolean isRoot) {
			if(String.IsNullOrEmpty(listViewId)) {
				throw new ArgumentNullException("listViewId");
			}
			return CreateListView(listViewId, null, collectionSource, isRoot, null);
		}
		public ListView CreateListView(IObjectSpace objectSpace, Type objectType, Boolean isRoot) {
			String listViewId = FindListViewId(objectType);
			CollectionSourceBase collectionSource = CreateCollectionSource(objectSpace, objectType, listViewId);
			return CreateListView(listViewId, collectionSource, isRoot);
		}
		public DetailView CreateDetailView(IObjectSpace objectSpace, Object obj) {
			return CreateDetailView(objectSpace, obj, null);
		}
		public DetailView CreateDetailView(IObjectSpace objectSpace, Object obj, Boolean isRoot) {
			if(obj == null) {
				throw new ArgumentNullException("obj");
			}
			String detailViewId = GetDetailViewId(obj.GetType());
			return CreateDetailView(objectSpace, detailViewId, isRoot, obj);
		}
		public DetailView CreateDetailView(IObjectSpace objectSpace, Object obj, View sourceView) {
			if(obj == null) {
				throw new ArgumentNullException("obj");
			}
			bool isRoot = true;
			lock(lockObjectA) {
				if(sourceView != null) {
					isRoot = (sourceView.ObjectSpace != objectSpace);
				}
				String detailViewId = FindDetailViewId(obj, sourceView);
				return CreateDetailView(objectSpace, detailViewId, isRoot, obj);
			}
		}
		public DetailView CreateDetailView(IObjectSpace objectSpace, String detailViewID, Boolean isRoot) {
			return CreateDetailView(objectSpace, detailViewID, isRoot, null);
		}
		public DetailView CreateDetailView(IObjectSpace objectSpace, String detailViewID, Boolean isRoot, Object obj) {
			Guard.ArgumentNotNull(objectSpace, "objectSpace");
			DetailViewCreatingEventArgs args = new DetailViewCreatingEventArgs(detailViewID, objectSpace, obj, isRoot);
			OnViewCreating(args);
			OnDetailViewCreating(args);
			DetailView result = args.View;
			if(result == null) {
				if(obj != null) {
					CheckDetailViewId(args.ViewID, obj.GetType());
				}
				IModelView modelView = FindModelView(args.ViewID);
				if(!(modelView is IModelDetailView)) {
					throw new ArgumentException(String.Format(
						"A '{0}' node was passed while a '{1}' node was expected. Node id: '{2}'",
						null, typeof(IModelDetailView).Name, detailViewID));
				}
				result = new DetailView(objectSpace, obj, this, isRoot);
				result.DelayedItemsInitialization = delayedViewItemsInitialization;
				result.SetModel(modelView);
			}
			OnViewCreated(result);
			OnDetailViewCreated(result);
			return result;
		}
		public DetailView CreateDetailView(IObjectSpace objectSpace, IModelDetailView modelDetailView, Boolean isRoot) {
			return CreateDetailView(objectSpace, modelDetailView, isRoot, null);
		}
		public DetailView CreateDetailView(IObjectSpace objectSpace, IModelDetailView modelDetailView, Boolean isRoot, Object obj) {
			if(modelDetailView == null) {
				throw new ArgumentNullException("modelDetailView");
			}
			return CreateDetailView(objectSpace, modelDetailView.Id, isRoot, obj);
		}
		public DashboardView CreateDashboardView(IObjectSpace objectSpace, String dashboardViewId, Boolean isRoot) {
			DashboardViewCreatingEventArgs args = new DashboardViewCreatingEventArgs(dashboardViewId, objectSpace);
			OnViewCreating(args);
			OnDashboardViewCreating(args);
			DashboardView result = args.View;
			if(result == null) {
				IModelView modelView = FindModelView(args.ViewID);
				if(!(modelView is IModelDashboardView)) {
					throw new ArgumentException(String.Format(
						"A '{0}' node was passed while a '{1}' node was expected. Node id: '{2}'",
						null, typeof(IModelDashboardView).Name, dashboardViewId));
				}
				result = new DashboardView(objectSpace, this, isRoot);
				result.DelayedItemsInitialization = delayedViewItemsInitialization;
				result.SetModel(modelView);
			}
			OnViewCreated(result);
			OnDashboardViewCreated(result);
			return result;
		}
		public NestedFrame CreateNestedFrame(ViewItem detailViewItem, TemplateContext context) {
			return new NestedFrame(this, context, detailViewItem, CreateControllers(typeof(Controller), true, null));
		}
		public Frame CreateFrame(TemplateContext context) {
			return new Frame(this, context, CreateControllers(typeof(Controller), true, null));
		}
		public Window CreateWindow(TemplateContext context, ICollection<Controller> controllers, bool createAllControllers, bool isMain) {
			return CreateWindowCore(context, CreateControllers(typeof(Controller), createAllControllers, controllers), isMain, true);
		}
		public Window CreateWindow(TemplateContext context, ICollection<Controller> controllers, bool isMain) {
			return CreateWindow(context, controllers, true, isMain);
		}
		public Window CreatePopupWindow(TemplateContext context, string viewId, bool createAllControllers, params Controller[] controllers) {
			return CreatePopupWindowCore(CalculateContext(context, viewId), CreateControllers(typeof(Controller), createAllControllers, controllers));
		}
		public Window CreatePopupWindow(TemplateContext context, string viewId, params Controller[] controllers) {
			return CreatePopupWindow(context, viewId, true, controllers);
		}
		public IFrameTemplate CreateTemplate(String templateContextName) {
			IFrameTemplate result = OnCreateCustomTemplate(templateContextName);
			if(result == null) {
				result = CreateDefaultTemplate(templateContextName);
			}
			if(result == null) {
				throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotCreateTemplateByName, templateContextName));
			}
			OnCustomizeTemplate(result, templateContextName);
			return result;
		}
		public IObjectSpace CreateObjectSpace() {
			return CreateObjectSpace(null);
		}
		public IObjectSpace CreateObjectSpace(Type objectType) {
			CheckCompatibility();
			IObjectSpace objectSpace = CreateObjectSpaceCore(objectType);
			if(objectSpace == null) {
				objectSpace = CreateObjectSpaceCore(null);
			}
			if(!objectSpace.IsConnected) {
				objectSpace.Connected += new EventHandler(ObjectSpace_Connected);
			}
			OnObjectSpaceCreated(objectSpace);
			return objectSpace;
		}
		public IObjectSpace CreateNestedObjectSpace(IObjectSpace parentObjectSpace) {
			IObjectSpace objectSpace = null;
			if(parentObjectSpace != null) {
				objectSpace = parentObjectSpace.CreateNestedObjectSpace();
			}
			if(!objectSpace.IsConnected) {
				objectSpace.Connected += new EventHandler(ObjectSpace_Connected);
			}
			OnObjectSpaceCreated(objectSpace);
			return objectSpace;
		}
		public virtual ControllerType CreateController<ControllerType>() where ControllerType : Controller, new() {
			ControllerType result;
			if(controllersManager == null) {
				result = new ControllerType();
			}
			else {
				result = controllersManager.CreateController<ControllerType>(Model);
			}
			result.Application = this;
			return result;
		}
		public virtual void SaveModelChanges() {
			ModelApplicationBase lastLayer = ((ModelApplicationBase)Model).LastLayer;
			if(lastLayer != null && lastLayer.Id == ModelApplicationLayerIds.UserDiffs) {
				ModelDifferenceStore userStore = CreateUserModelDifferenceStore();
				if(userStore != null) {
					Tracing.Tracer.LogSeparator("Save model changes");
					userStore.SaveDifference(lastLayer);
				}
			}
		}
		public IModelClass FindModelClass(Type objectType) {
			IModelClass modelClass = null;
			if((objectType != null) && (model != null) && (model.BOModel != null)) {
				ITypeInfo typeInfo = typesInfo.FindTypeInfo(objectType);
				if(typeInfo != null) {
					modelClass = model.BOModel.GetClass(typeInfo.Type);
				}
				else {
					modelClass = model.BOModel.GetClass(objectType);
				}
				if(modelClass == null) {
					Type interfaceType = ModelAutoGeneratedRuleNodesHelper.GetAssociatedDomainComponentType(objectType);
					if(interfaceType != null) {
						modelClass = model.BOModel.GetClass(interfaceType);
					}
				}
			}
			return modelClass;
		}
		public String FindListViewId(Type objectType) {
			IModelClass modelClass = FindModelClass(objectType);
			if(modelClass == null) return string.Empty;
			IModelListView defaultListView = modelClass.DefaultListView;
			return defaultListView != null ? defaultListView.Id : string.Empty;
		}
		public String GetListViewId(Type objectType) {
			String result = FindListViewId(objectType);
			if(String.IsNullOrEmpty(result)) {
				throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToFindAListViewForType, objectType));
			}
			return result;
		}
		public String FindLookupListViewId(Type objectType) {
			IModelClass modelClass = FindModelClass(objectType);
			return ((modelClass != null) && (modelClass.DefaultLookupListView != null)) ? modelClass.DefaultLookupListView.Id : "";
		}
		public String FindDetailViewId(Type objectType) {
			IModelClass modelClass = FindModelClass(objectType);
			return ((modelClass != null) && (modelClass.DefaultDetailView != null)) ? modelClass.DefaultDetailView.Id : "";
		}
		public String FindDetailViewId(Object obj, View sourceView) {
			if(obj == null) {
				throw new ArgumentNullException("obj");
			}
			String result = "";
			Type objectType = obj.GetType();
			if((sourceView is ObjectView) && (((ObjectView)sourceView).ObjectSpace != null)) {
				objectType = ((ObjectView)sourceView).ObjectSpace.GetObjectType(obj);
			}
			else if(obj is XafDataViewRecord) {
				objectType = ((XafDataViewRecord)obj).ObjectType;
			}
			if(sourceView != null) {
				ITypeInfo typeInfo = typesInfo.FindTypeInfo(objectType);
				if((sourceView is ObjectView) && ((ObjectView)sourceView).ObjectTypeInfo == typeInfo) {
					if(sourceView is ListView) {
						result = ((ListView)sourceView).DetailViewId;
					}
					else if(sourceView is DetailView) {
						result = sourceView.Id;
					}
				}
			}
			if(String.IsNullOrEmpty(result)) {
				result = FindDetailViewId(objectType);
			}
			return result;
		}
		public String GetDetailViewId(Type objectType) {
			String result = FindDetailViewId(objectType);
			CheckDetailViewId(result, objectType);
			return result;
		}
		public virtual IModelView FindModelView(String viewId) {
			IModelViews views = (model != null) ? model.Views : null;
			return (views != null) ? views[viewId] : null;
		}
		public virtual IModelTemplate GetTemplateCustomizationModel(IFrameTemplate template) {
			if(template == null) {
				throw new ArgumentNullException("template");
			}
			string id = template.GetType().FullName;
			IModelTemplate result = Model.Templates[id];
			if(result == null) {
				result = Model.Templates.AddNode<IModelTemplate>(id);
			}
			return result;
		}
		public void BeginInit() {
			Modules.BeginInit();
			OnInitializing();
		}
		public void EndInit() {
			OnInitialized();
			Modules.EndInit();
		}
		void IDisposable.Dispose() {
			this.Dispose();
		}
		public new void Dispose() {
			base.Dispose();
			GC.ReRegisterForFinalize(this);
		}
		public void SetLanguage(string languageName) {
			OnBeforeSetCulture();
			SetLanguageCore(languageName);
			if(model != null) {
				model.PreferredLanguage = languageName;
			}
			OnAfterSetCulture();
		}
		public void SetFormattingCulture(string formattingCultureName) {
			OnBeforeSetCulture();
			SetFormattingCultureCore(formattingCultureName);
			selectedFormattingCultureName = formattingCultureName;
			OnAfterSetCulture();
		}
		public virtual void LogOff() { }
		public virtual void UpdateStatus(String context, String title, String message, params Object[] additionalParams) {
			OnStatusUpdating(context, title, message, additionalParams);
		}
		ApplicationModelManager IApplicationModelManagerProvider.GetModelManager() {
			return modelManager;
		}
#if DebugTest
		public void SetModelApplication(IModelApplication model) {
			this.model = model;
			EnsureShowViewStrategy();
			OnModelChanged();
		}
#endif
		[Browsable(false)]
		public ITypesInfo TypesInfo {
			get { return typesInfo; }
		}
		[Browsable(false)]
#if !SL
	[DevExpressExpressAppLocalizedDescription("XafApplicationModel")]
#endif
		public IModelApplication Model {
			get { return model; }
		}
		[Browsable(false)]
#if !SL
	[DevExpressExpressAppLocalizedDescription("XafApplicationCurrentAspectProvider")]
#endif
		public ICurrentAspectProvider CurrentAspectProvider {
			get { return currentAspectProvider; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("XafApplicationSite")]
#endif
		public override ISite Site {
			get { return base.Site; }
			set {
				if(value != null) {
					Tracing.Close(true); 
				}
				base.Site = value;
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationResourcesExportedToModel"),
#endif
 Editor("DevExpress.ExpressApp.Design.ResourceLocalizedTypesEditor, DevExpress.ExpressApp.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public List<Type> ResourcesExportedToModel {
			get { return localizerTypes; }
			set { localizerTypes = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("XafApplicationDefaultCollectionSourceMode")]
#endif
		public CollectionSourceMode DefaultCollectionSourceMode {
			get { return defaultCollectionSourceMode; }
			set { defaultCollectionSourceMode = value; }
		}
		[Browsable(false)]
		public Boolean DelayedViewItemsInitialization {
			get { return delayedViewItemsInitialization; }
			set { delayedViewItemsInitialization = value; }
		}
		[Browsable(false)]
		public IList<Type> IgnoredExceptions {
			get {
				return ignoredExceptions;
			}
		}
		[Browsable(false)]
		public DatabaseUpdateMode DatabaseUpdateMode {
			get { return databaseUpdateMode; }
			set { databaseUpdateMode = value; }
		}
		[Browsable(false)]
		public IObjectSpaceProvider ObjectSpaceProvider {
			get { return objectSpaceProviders.Count > 0 ? objectSpaceProviders[0] : null; }
		}
		[Browsable(false)]
		public IList<IObjectSpaceProvider> ObjectSpaceProviders {
			get { return objectSpaceProviders.AsReadOnly(); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ModuleList Modules {
			get { return modules; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public ISecurityStrategyBase Security {
			get { return security; }
			set {
				if((security != value) && !isLoggedOn) {
					security = value;
					if((security != null) && !IsLoading) {
						Type securityModuleType = security.GetModuleType();
						if(securityModuleType != null && modules.FindModule(securityModuleType) == null) {
							modules.Add((ModuleBase)TypeHelper.CreateInstance(securityModuleType));
						}
					}
					OnPropertyChanged("Security");
				}
			}
		}
		[Localizable(true)]
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationApplicationName"),
#endif
 Category("Data")]
		public string ApplicationName {
			get {
				return applicationName;
			}
			set {
				applicationName = value;
				OnPropertyChanged("ApplicationName");
			}
		}
		[Browsable(false)]
		public bool IsConnectionOwner {
			get { return isConnectionOwner; }
			set { isConnectionOwner = value; }
		}
		[Browsable(false)]
		public IDbConnection Connection {
			get {
				if(this.connection != null) {
					return this.connection;
				}
				return null;
			}
			set {
				if(objectSpaceProviders.Count > 0) {
					throw new InvalidOperationException("set_Connection");
				}
				if(value != null) {
					this.connectionString = "";
				}
				this.connection = value;
				OnPropertyChanged("Connection");
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationConnectionString"),
#endif
 Category("Data"), Editor("System.Web.UI.Design.ConnectionStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public String ConnectionString {
			get {
				if(objectSpaceProviders.Count > 0) {
					return objectSpaceProviders[0].ConnectionString;
				}
				else {
					return connectionString;
				}
			}
			set {
				if(isLoggedOn) {
					throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToChangeConnectionString));
				}
				if(objectSpaceProviders.Count > 0) {
					objectSpaceProviders[0].ConnectionString = value;
					isCompatibilityChecked = false;
				}
				else {
					connectionString = value;
				}
				if(value != "" && connection != null) {
					this.connection.Dispose();
					this.connection = null;
				}
			}
		}
		[Browsable(false)]
		public string Title {
			get {
				String titleTemplate = defaultTitleTemplate;
				if((model != null) && !String.IsNullOrEmpty(model.Title)) {
					titleTemplate = model.Title;
				}
				if(!String.IsNullOrEmpty(title)) {
					titleTemplate = title;
				}
				return String.Format(new ObjectFormatter(), titleTemplate, this);
			}
			set { title = value; }
		}
		[Browsable(false)]
		public bool IsLoading {
			get { return isInitializingCounter > 0; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("XafApplicationEditorFactory")]
#endif
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEditorsFactory EditorFactory {
			get { return editorFactory; }
			set { editorFactory = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationTablePrefixes"),
#endif
 Category("Data")]
		public string TablePrefixes {
			get { return tablePrefixes; }
			set { tablePrefixes = value; }
		}
		[Browsable(false)]
		public virtual Window MainWindow { get { return null; } }
		[Browsable(false)]
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationShowViewStrategy"),
#endif
 Category("Behavior")]
		public ShowViewStrategyBase ShowViewStrategy {
			get { return showViewStrategy; }
			set {
				if(value == null) {
					throw new ArgumentNullException("ShowViewStrategy");
				}
				showViewStrategy = value;
				OnShowViewStrategyChanged();
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationMaxLogonAttemptCount"),
#endif
 Category("Behavior")]
		public Int32 MaxLogonAttemptCount {
			get { return maxLogonAttemptCount; }
			set { maxLogonAttemptCount = value; }
		}
		[Browsable(false)]
		public virtual Boolean IsDelayedDetailViewDataLoadingEnabled {
			get { return false; }
			set { throw new NotImplementedException(); }
		}
		[Browsable(false)]
		public Boolean LinkNewObjectToParentImmediately {
			get { return linkNewObjectToParentImmediately; }
			set { linkNewObjectToParentImmediately = value; }
		}
		public CheckCompatibilityType CheckCompatibilityType {
			get { return checkCompatibilityType; }
			set { checkCompatibilityType = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationDatabaseVersionMismatch"),
#endif
 Category("Main")]
		public event EventHandler<DatabaseVersionMismatchEventArgs> DatabaseVersionMismatch;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationCustomCheckCompatibility"),
#endif
 Category("Main")]
		public event EventHandler<CustomCheckCompatibilityEventArgs> CustomCheckCompatibility;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationCustomizeLanguage"),
#endif
 Category("Globalization")]
		public event EventHandler<CustomizeLanguageEventArgs> CustomizeLanguage;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationCustomizeFormattingCulture"),
#endif
 Category("Globalization")]
		public event EventHandler<CustomizeFormattingCultureEventArgs> CustomizeFormattingCulture;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationCreateCustomTemplate"),
#endif
 Category("Customization")]
		public event EventHandler<CreateCustomTemplateEventArgs> CreateCustomTemplate;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationCustomizeTemplate"),
#endif
 Category("Customization")]
		public event EventHandler<CustomizeTemplateEventArgs> CustomizeTemplate;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationCustomizeTableName"),
#endif
 Category("Customization")]
		public event EventHandler<CustomizeTableNameEventArgs> CustomizeTableName;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationCreateCustomUserModelDifferenceStore"),
#endif
 Category("Events")]
		public event EventHandler<CreateCustomModelDifferenceStoreEventArgs> CreateCustomUserModelDifferenceStore;
		[ Category("Events")]
		public event EventHandler<CreateCustomModelDifferenceStoreEventArgs> CreateCustomDeviceSpecificModelDifferenceStore;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationCreateCustomModelDifferenceStore"),
#endif
 Category("Events")]
		public event EventHandler<CreateCustomModelDifferenceStoreEventArgs> CreateCustomModelDifferenceStore;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationCustomProcessShortcut"),
#endif
 Category("Events")]
		public event EventHandler<CustomProcessShortcutEventArgs> CustomProcessShortcut;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationHandleShortcutProcessingException"),
#endif
 Category("Events")]
		public event EventHandler<HandleShortcutProcessingExceptionEventArgs> HandleShortcutProcessingException;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationViewCreating"),
#endif
 Category("Events")]
		public event EventHandler<ViewCreatingEventArgs> ViewCreating;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationViewCreated"),
#endif
 Category("Events")]
		public event EventHandler<ViewCreatedEventArgs> ViewCreated;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationDetailViewCreating"),
#endif
 Category("Events")]
		public event EventHandler<DetailViewCreatingEventArgs> DetailViewCreating;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationDetailViewCreated"),
#endif
 Category("Events")]
		public event EventHandler<DetailViewCreatedEventArgs> DetailViewCreated;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationDashboardViewCreating"),
#endif
 Category("Events")]
		public event EventHandler<DashboardViewCreatingEventArgs> DashboardViewCreating;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationDashboardViewCreated"),
#endif
 Category("Events")]
		public event EventHandler<DashboardViewCreatedEventArgs> DashboardViewCreated;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationListViewCreating"),
#endif
 Category("Events")]
		public event EventHandler<ListViewCreatingEventArgs> ListViewCreating;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationListViewCreated"),
#endif
 Category("Events")]
		public event EventHandler<ListViewCreatedEventArgs> ListViewCreated;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationViewShowing"),
#endif
 Category("Events")]
		public event EventHandler<ViewShowingEventArgs> ViewShowing;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationViewShown"),
#endif
 Category("Events")]
		public event EventHandler<ViewShownEventArgs> ViewShown;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationObjectSpaceCreated"),
#endif
 Category("Events")]
		public event EventHandler<ObjectSpaceCreatedEventArgs> ObjectSpaceCreated;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationCreateCustomObjectSpaceProvider"),
#endif
 Category("Events")]
		public event EventHandler<CreateCustomObjectSpaceProviderEventArgs> CreateCustomObjectSpaceProvider;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationSettingUp"),
#endif
 Category("Events")]
		public event EventHandler<SetupEventArgs> SettingUp;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationSetupComplete"),
#endif
 Category("Events")]
		public event EventHandler<EventArgs> SetupComplete;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationCreateCustomPropertyCollectionSource"),
#endif
 Category("Events")]
		public event EventHandler<CreateCustomPropertyCollectionSourceEventArgs> CreateCustomPropertyCollectionSource;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationCreateCustomCollectionSource"),
#endif
 Category("Events")]
		public event EventHandler<CreateCustomCollectionSourceEventArgs> CreateCustomCollectionSource;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationLoggingOff"),
#endif
 Category("Events")]
		public event EventHandler<LoggingOffEventArgs> LoggingOff;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationLoggedOff"),
#endif
 Category("Events")]
		public event EventHandler<EventArgs> LoggedOff;
		[ Category("Events")]
		public event EventHandler<StatusUpdatingEventArgs> StatusUpdating;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationModelChanged"),
#endif
 Category("Events")]
		public event EventHandler ModelChanged;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationUserDifferencesLoaded"),
#endif
 Category("Events")]
		public event EventHandler<EventArgs> UserDifferencesLoaded;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationLastLogonParametersReading"),
#endif
 Category("Logon")]
		public event EventHandler<LastLogonParametersReadingEventArgs> LastLogonParametersReading;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationLastLogonParametersRead"),
#endif
 Category("Logon")]
		public event EventHandler<LastLogonParametersReadEventArgs> LastLogonParametersRead;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationLastLogonParametersWriting"),
#endif
 Category("Logon")]
		public event EventHandler<LastLogonParametersWritingEventArgs> LastLogonParametersWriting;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationCreateCustomLogonParameterStore"),
#endif
 Category("Logon")]
		public event EventHandler<CreateCustomLogonParameterStoreEventArgs> CreateCustomLogonParameterStore;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationCreateCustomLogonWindowControllers"),
#endif
 Category("Logon")]
		public event EventHandler<CreateCustomLogonWindowControllersEventArgs> CreateCustomLogonWindowControllers;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationCreateCustomLogonWindowObjectSpace"),
#endif
 Category("Logon")]
		public event EventHandler<CreateCustomLogonWindowObjectSpaceEventArgs> CreateCustomLogonWindowObjectSpace;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationLoggingOn"),
#endif
 Category("Logon")]
		public event EventHandler<LogonEventArgs> LoggingOn;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationLoggedOn"),
#endif
 Category("Logon")]
		public event EventHandler<LogonEventArgs> LoggedOn;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("XafApplicationLogonFailed"),
#endif
 Category("Logon")]
		public event EventHandler<LogonFailedEventArgs> LogonFailed;
		[Browsable(false)]
		public event PropertyChangedEventHandler PropertyChanged;
		[Browsable(false)]
		public event EventHandler<DatabaseUpdaterEventArgs> DatabaseUpdaterCreating;
		public event EventHandler<CustomizeLanguagesListEventArgs> CustomizeLanguagesList;
	}
}
