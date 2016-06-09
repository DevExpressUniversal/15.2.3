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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp {
	public class CustomProcessShortcutEventArgs : HandledEventArgs {
		private View view;
		public CustomProcessShortcutEventArgs(ViewShortcut shortcut) {
			this.Shortcut = shortcut;
		}
		public ViewShortcut Shortcut;
		public View View {
			get { return view; }
			set { view = value; }
		}
	}
	public class HandleShortcutProcessingExceptionEventArgs : HandledEventArgs {
		private ViewShortcut shortcut;
		private Exception shortcutProcessingException;
		public HandleShortcutProcessingExceptionEventArgs(ViewShortcut shortcut, Exception shortcutProcessingException) {
			this.Handled = false;
			this.shortcut = shortcut;
			this.shortcutProcessingException = shortcutProcessingException;
		}
		public ViewShortcut Shortcut {
			get { return shortcut; }
		}
		public Exception ShortcutProcessingException {
			get { return shortcutProcessingException; }
		}
	}
	public class ViewCreatingEventArgs : EventArgs {
		private string viewID;
		private IObjectSpace objectSpace;
		private bool isRoot;
		private View view;
		public ViewCreatingEventArgs(string viewID, IObjectSpace objectSpace, bool isRoot) {
			this.viewID = viewID;
			this.objectSpace = objectSpace;
			this.isRoot = isRoot;
		}
		public string ViewID {
			get { return viewID; }
			set { viewID = value; }
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
		public bool IsRoot {
			get { return isRoot; }
		}
		public View View {
			get { return view; }
			set { view = value; }
		}
	}
	public class ListViewCreatingEventArgs : ViewCreatingEventArgs {
		private CollectionSourceBase collectionSource;
		public ListViewCreatingEventArgs(string viewID, CollectionSourceBase collectionSource, bool isRoot)
			: base(viewID, collectionSource.ObjectSpace, isRoot) {
			this.collectionSource = collectionSource;
		}
		public CollectionSourceBase CollectionSource {
			get { return collectionSource; }
		}
		public new ListView View {
			get { return (ListView)base.View; }
			set { base.View = value; }
		}
	}
	public class ViewCreatedEventArgs : EventArgs {
		private View view;
		public ViewCreatedEventArgs(View view) {
			this.view = view;
		}
		public View View {
			get { return view; }
			set { view = value; } 
		}
	}
	public class ListViewCreatedEventArgs : ViewCreatedEventArgs {
		public ListViewCreatedEventArgs(ListView listView) : base(listView) { }
		public ListView ListView {
			get { return (ListView)base.View; }
			set { base.View = value; } 
		}
	}
	public class DetailViewCreatingEventArgs : ViewCreatingEventArgs {
		private Object obj;
		public DetailViewCreatingEventArgs(string viewID, IObjectSpace objectSpace, Object obj, bool isRoot)
			: base(viewID, objectSpace, isRoot) {
			this.obj = obj;
		}
		public Object Obj {
			get { return obj; }
		}
		public new DetailView View {
			get { return (DetailView)base.View; }
			set { base.View = value; }
		}
	}
	public class DetailViewCreatedEventArgs : ViewCreatedEventArgs {
		public DetailViewCreatedEventArgs(DetailView view) : base(view) { }
		public new DetailView View {
			get { return (DetailView)base.View; }
			set { base.View = value; }
		}
	}
	public class DashboardViewCreatingEventArgs : ViewCreatingEventArgs {
		public DashboardViewCreatingEventArgs(String viewId, IObjectSpace objectSpace) : base(viewId, objectSpace, true) { }
		public new DashboardView View {
			get { return (DashboardView)base.View; }
			set { base.View = value; }
		}
	}
	public class DashboardViewCreatedEventArgs : ViewCreatedEventArgs {
		public DashboardViewCreatedEventArgs(DashboardView view) : base(view) { }
		public new DashboardView View {
			get { return (DashboardView)base.View; }
		}
	}
	public class ViewShowingEventArgs : EventArgs {
		private Frame sourceFrame;
		private Frame targetFrame;
		private View view;
		public ViewShowingEventArgs(Frame targetFrame, View view, Frame sourceFrame) {
			this.targetFrame = targetFrame;
			this.sourceFrame = sourceFrame;
			this.view = view;
		}
		public Frame SourceFrame {
			get { return sourceFrame; }
		}
		public Frame TargetFrame {
			get { return targetFrame; }
		}
		public View View {
			get { return view; }
		}
	}
	public class ViewShownEventArgs : EventArgs {
		private Frame sourceFrame;
		private Frame targetFrame;
		public ViewShownEventArgs(Frame targetFrame, Frame sourceFrame) {
			this.targetFrame = targetFrame;
			this.sourceFrame = sourceFrame;
		}
		public Frame TargetFrame {
			get { return targetFrame; }
		}
		public Frame SourceFrame {
			get { return sourceFrame; }
		}
	}
	public class CustomizeTemplateEventArgs : EventArgs {
		private TemplateContext context;
		private IFrameTemplate template;
		public CustomizeTemplateEventArgs(IFrameTemplate template, TemplateContext context) {
			this.template = template;
			this.context = context;
		}
		public IFrameTemplate Template {
			get { return template; }
		}
		public TemplateContext Context {
			get { return context; }
		}
	}
	public class CreateCustomTemplateEventArgs : EventArgs {
		private IFrameTemplate template;
		private TemplateContext context;
		private XafApplication application;
		public CreateCustomTemplateEventArgs(XafApplication application, TemplateContext context) {
			this.context = context;
			this.application = application;
		}
		public IFrameTemplate Template {
			get { return template; }
			set { template = value; }
		}
		public TemplateContext Context {
			get { return context; }
		}
		public XafApplication Application {
			get { return application; }
		}
	}
	public class CustomCheckCompatibilityEventArgs : HandledEventArgs {
		private IObjectSpaceProvider objectSpaceProvider;
		private IList<ModuleBase> modules;
		private string applicationName;
		private bool isCompatibilityChecked;
		public CustomCheckCompatibilityEventArgs(bool isCompatibilityChecked, IObjectSpaceProvider objectSpaceProvider, IList<ModuleBase> modules, string applicationName) {
			this.isCompatibilityChecked = isCompatibilityChecked;
			this.objectSpaceProvider = objectSpaceProvider;
			this.modules = modules;
			this.applicationName = applicationName;
		}
		public IObjectSpaceProvider ObjectSpaceProvider {
			get { return objectSpaceProvider; }
		}
		public IList<ModuleBase> Modules {
			get { return modules; }
		}
		public string ApplicationName {
			get { return applicationName; }
		}
		public bool IsCompatibilityChecked {
			get { return isCompatibilityChecked; }
		}
	}
	public class DatabaseVersionMismatchEventArgs : HandledEventArgs {
		private DatabaseUpdaterBase updater;
		private CompatibilityError compatibilityError;
		public DatabaseVersionMismatchEventArgs(DatabaseUpdaterBase updater, CompatibilityError compatibilityError) {
			this.updater = updater;
			this.compatibilityError = compatibilityError;
		}
		public DatabaseUpdaterBase Updater {
			get { return updater; }
			set { updater = value; }
		}
		public CompatibilityError CompatibilityError {
			get { return compatibilityError; }
			set { compatibilityError = value; }
		}
	}
	public class LastLogonParametersReadingEventArgs : HandledEventArgs {
		private object logonObject;
		private SettingsStorage settingsStorage;
		public LastLogonParametersReadingEventArgs(object logonObject, SettingsStorage settingsStorage) {
			this.logonObject = logonObject;
			this.settingsStorage = settingsStorage;
		}
		public object LogonObject {
			get { return logonObject; }
		}
		public SettingsStorage SettingsStorage {
			get { return settingsStorage; }
		}
	}
	public class LastLogonParametersReadEventArgs : EventArgs {
		private object logonObject;
		private SettingsStorage settingsStorage;
		public LastLogonParametersReadEventArgs(object logonObject, SettingsStorage settingsStorage) {
			this.logonObject = logonObject;
			this.settingsStorage = settingsStorage;
		}
		public object LogonObject {
			get { return logonObject; }
		}
		public SettingsStorage SettingsStorage {
			get { return settingsStorage; }
		}
	}
	public class LastLogonParametersWritingEventArgs : HandledEventArgs {
		private DetailView detailView;
		private object logonObject;
		private SettingsStorage settingsStorage;
		public LastLogonParametersWritingEventArgs(DetailView detailView, object logonObject, SettingsStorage settingsStorage) {
			this.detailView = detailView;
			this.logonObject = logonObject;
			this.settingsStorage = settingsStorage;
		}
		public DetailView DetailView {
			get { return detailView; }
		}
		public object LogonObject {
			get { return logonObject; }
		}
		public SettingsStorage SettingsStorage {
			get { return settingsStorage; }
		}
	}
	public class CreateCustomCollectionSourceEventArgs : EventArgs {
		private IObjectSpace objectSpace;
		private Type objectType;
		private String listViewID;
		private CollectionSourceDataAccessMode dataAccessMode;
		private CollectionSourceMode mode;
		private CollectionSourceBase collectionSource;
		public CreateCustomCollectionSourceEventArgs(IObjectSpace objectSpace, Type objectType, String listViewID, CollectionSourceDataAccessMode dataAccessMode, CollectionSourceMode mode) {
			this.objectSpace = objectSpace;
			this.objectType = objectType;
			this.listViewID = listViewID;
			this.dataAccessMode = dataAccessMode;
			this.mode = mode;
		}
		public CreateCustomCollectionSourceEventArgs(IObjectSpace objectSpace, Type objectType, String listViewID, CollectionSourceMode mode)
			: this(objectSpace, objectType, listViewID, CollectionSourceDataAccessMode.Client, mode) {
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
		public Type ObjectType {
			get { return objectType; }
		}
		public string ListViewID {
			get { return listViewID; }
		}
		public CollectionSourceDataAccessMode DataAccessMode {
			get { return dataAccessMode; }
		}
		public CollectionSourceMode Mode {
			get { return mode; }
		}
		public CollectionSourceBase CollectionSource {
			get { return collectionSource; }
			set { collectionSource = value; }
		}
	}
	public class CreateCustomPropertyCollectionSourceEventArgs : EventArgs {
		private IObjectSpace objectSpace;
		private Object masterObject;
		private IMemberInfo memberInfo;
		private String listViewID;
		private Type masterObjectType;
		private CollectionSourceDataAccessMode dataAccessMode;
		private CollectionSourceMode mode;
		private PropertyCollectionSource propertyCollectionSource;
		public CreateCustomPropertyCollectionSourceEventArgs(IObjectSpace objectSpace, Type masterObjectType, Object masterObject, IMemberInfo memberInfo, String listViewID, CollectionSourceDataAccessMode dataAccessMode, CollectionSourceMode mode) {
			this.objectSpace = objectSpace;
			this.masterObject = masterObject;
			this.memberInfo = memberInfo;
			this.listViewID = listViewID;
			this.masterObjectType = masterObjectType;
			this.dataAccessMode = dataAccessMode;
			this.mode = mode;
		}
		public CreateCustomPropertyCollectionSourceEventArgs(IObjectSpace objectSpace, Type masterObjectType, Object masterObject, IMemberInfo memberInfo, String listViewID, CollectionSourceMode mode)
			: this(objectSpace, masterObjectType, masterObject, memberInfo, listViewID, CollectionSourceDataAccessMode.Client, mode) {
		}
		public Type MasterObjectType {
			get { return masterObjectType; }
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
		public object MasterObject {
			get { return masterObject; }
		}
		public IMemberInfo MemberInfo {
			get { return memberInfo; }
		}
		public string ListViewID {
			get { return listViewID; }
		}
		public CollectionSourceDataAccessMode DataAccessMode {
			get { return dataAccessMode; }
		}
		public CollectionSourceMode Mode {
			get { return mode; }
		}
		public PropertyCollectionSource PropertyCollectionSource {
			get { return propertyCollectionSource; }
			set { propertyCollectionSource = value; }
		}
	}
	public class ObjectsGettingEventArgs : EventArgs {
		private Type objectType;
		private CriteriaOperator criteria;
		private IList<SortProperty> sorting;
		private Boolean inTransaction;
		private IList objects;
		private Boolean handled;
		public ObjectsGettingEventArgs(Type objectType, CriteriaOperator criteria, IList<SortProperty> sorting, Boolean inTransaction) {
			this.objectType = objectType;
			this.criteria = criteria;
			this.sorting = sorting;
			this.inTransaction = inTransaction;
			handled = true;
		}
		public Type ObjectType {
			get { return objectType; }
		}
		public CriteriaOperator Criteria {
			get { return criteria; }
		}
		public IList<SortProperty> Sorting {
			get { return sorting; }
		}
		public Boolean InTransaction {
			get { return inTransaction; }
		}
		public IList Objects {
			get { return objects; }
			set { objects = value; }
		}
		#region Obsolete 15.2
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Boolean Handled {
			get { return handled; }
			set { handled = value; }
		}
		#endregion
	}
	public class ObjectGettingEventArgs : EventArgs {
		private Object sourceObject;
		private Object targetObject;
		public ObjectGettingEventArgs(Object sourceObject, Object targetObject) {
			this.sourceObject = sourceObject;
			this.targetObject = targetObject;
		}
		public Object SourceObject {
			get { return sourceObject; }
		}
		public Object TargetObject {
			get { return targetObject; }
			set { targetObject = value; }
		}
	}
	public class CreateCustomLogonParameterStoreEventArgs : HandledEventArgs {
		private SettingsStorage settingsStorage;
		public SettingsStorage Storage {
			get { return settingsStorage; }
			set { settingsStorage = value; }
		}
	}
	public class CreateCustomLogonWindowControllersEventArgs : EventArgs {
		private List<Controller> controllers = new List<Controller>();
		public List<Controller> Controllers {
			get { return controllers; }
			set { controllers = value; }
		}
	}
	public class CreateCustomLogonWindowObjectSpaceEventArgs : EventArgs {
		private IObjectSpace objectSpace;
		private object logonParameters;
		public CreateCustomLogonWindowObjectSpaceEventArgs(object logonParameters) {
			this.logonParameters = logonParameters;
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
			set { objectSpace = value; }
		}
		public object LogonParameters {
			get { return logonParameters; }
		}
	}
	public class LogonEventArgs : EventArgs {
		private object logonParameters;
		public LogonEventArgs(object logonParameters) {
			this.logonParameters = logonParameters;
		}
		public object LogonParameters {
			get { return logonParameters; }
		}
	}
	public class LoggingOffEventArgs : CancelEventArgs {
		private object logonParameters;
		private bool canCancel;
		public LoggingOffEventArgs(object logonParameters, bool canCancel) {
			this.logonParameters = logonParameters;
			this.canCancel = canCancel;
		}
		public object LogonParameters {
			get { return logonParameters; }
		}
		public bool CanCancel {
			get { return canCancel; }
		}
	}
	public class SetupEventArgs : EventArgs {
		private ExpressApplicationSetupParameters setupParameters;
		public SetupEventArgs(ExpressApplicationSetupParameters setupParameters) {
			this.setupParameters = setupParameters;
		}
		public ExpressApplicationSetupParameters SetupParameters {
			get { return setupParameters; }
		}
	}
	public class LogonFailedEventArgs : HandledEventArgs {
		private object logonParameters;
		private Exception exception;
		public LogonFailedEventArgs(object logonParameters, Exception exception) {
			this.logonParameters = logonParameters;
			this.exception = exception;
		}
		public object LogonParameters {
			get { return logonParameters; }
		}
		public Exception Exception {
			get { return exception; }
		}
	}
	public class ObjectSpaceCreatedEventArgs : EventArgs {
		private IObjectSpace objectSpace;
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
		public ObjectSpaceCreatedEventArgs(IObjectSpace objectSpace) {
			this.objectSpace = objectSpace;
		}
	}
	public class CreateCustomModelDifferenceStoreEventArgs : HandledEventArgs {
		private ModelDifferenceStore store;
		internal List<KeyValuePair<string, ModelStoreBase>> ExtraDiffStores = new List<KeyValuePair<string, ModelStoreBase>>();
		public ModelDifferenceStore Store {
			get { return store; }
			set { store = value; }
		}
		public void AddExtraDiffStore(string id, ModelStoreBase store) {
			ExtraDiffStores.Add(new KeyValuePair<string, ModelStoreBase>(id, store));
		}
	}
	public class CustomizeLanguageEventArgs : EventArgs {
		private string languageName;
		private string userLanguageName;
		private string preferredLanguageName;
		public CustomizeLanguageEventArgs(string languageName, string userLanguageName, string preferredLanguageName) {
			this.languageName = languageName;
			this.userLanguageName = userLanguageName;
			this.preferredLanguageName = preferredLanguageName;
		}
		public string LanguageName {
			get { return languageName; }
			set { languageName = value; }
		}
		public string UserLanguageName {
			get { return userLanguageName; }
		}
		public string PreferredLanguageName {
			get { return preferredLanguageName; }
		}
	}
	public class CustomizeFormattingCultureEventArgs : EventArgs {
		private CultureInfo formattingCulture;
		private string userLanguageName;
		private string preferredLanguageName;
		public CustomizeFormattingCultureEventArgs(CultureInfo formattingCulture, string userLanguageName, string preferredLanguageName) {
			this.formattingCulture = formattingCulture;
			this.userLanguageName = userLanguageName;
			this.preferredLanguageName = preferredLanguageName;
		}
		public CultureInfo FormattingCulture {
			get { return formattingCulture; }
			set { formattingCulture = value; }
		}
		public string UserLanguageName {
			get { return userLanguageName; }
		}
		public string PreferredLanguageName {
			get { return preferredLanguageName; }
		}
	}
	public class CreateCustomObjectSpaceProviderEventArgs : EventArgs {
		private String connectionString;
		private IDbConnection connection;
		private List<IObjectSpaceProvider> objectSpaceProviders;
		private Boolean isObjectSpaceProviderOwner;
		protected CreateCustomObjectSpaceProviderEventArgs() {
			objectSpaceProviders = new List<IObjectSpaceProvider>();
		}
		public CreateCustomObjectSpaceProviderEventArgs(IDbConnection connection)
			: this() {
			this.connection = connection;
		}
		public CreateCustomObjectSpaceProviderEventArgs(String connectionString)
			: this() {
			this.connectionString = connectionString;
		}
		public IDbConnection Connection {
			get { return connection; }
		}
		public String ConnectionString {
			get { return connectionString; }
		}
		public Boolean IsObjectSpaceProviderOwner {
			get { return isObjectSpaceProviderOwner; }
			set { isObjectSpaceProviderOwner = value; }
		}
		public IObjectSpaceProvider ObjectSpaceProvider {
			get {
				if(objectSpaceProviders.Count > 0) {
					return objectSpaceProviders[0];
				}
				else {
					return null;
				}
			}
			set {
				if(objectSpaceProviders.Count == 1) {
					objectSpaceProviders.Clear();
					objectSpaceProviders.Add(value);
				}
				else {
					objectSpaceProviders.Remove(value);
					objectSpaceProviders.Insert(0, value);
				}
			}
		}
		public List<IObjectSpaceProvider> ObjectSpaceProviders {
			get { return objectSpaceProviders; }
		}
	}
	public class ObjectManipulatingEventArgs : EventArgs {
		private Object obj;
		public ObjectManipulatingEventArgs(Object obj) {
			this.obj = obj;
		}
		public Object Object {
			get { return obj; }
		}
	}
	public class ObjectsManipulatingEventArgs : EventArgs {
		private ICollection objects;
		public ObjectsManipulatingEventArgs(Object obj) {
			objects = new Object[] { obj };
		}
		public ObjectsManipulatingEventArgs(ICollection objects) {
			this.objects = objects;
		}
		public ICollection Objects {
			get { return objects; }
		}
	}
	public class DeletingEventArgs : EventArgs {
		private IList objects;
		public DeletingEventArgs(IList objects) {
			this.objects = objects;
		}
		public IList Objects {
			get { return objects; }
		}
	}
	public class ObjectChangedEventArgs : ObjectManipulatingEventArgs {
		private IMemberInfo memberInfo;
		private String propertyName;
		private Object newValue;
		private Object oldValue;
		public ObjectChangedEventArgs(Object obj, String propertyName, Object oldValue, Object newValue)
			: base(obj) {
			this.propertyName = propertyName;
			this.newValue = newValue;
			this.oldValue = oldValue;
		}
		public ObjectChangedEventArgs(Object obj, IMemberInfo memberInfo, Object oldValue, Object newValue)
			: base(obj) {
			this.newValue = newValue;
			this.oldValue = oldValue;
			this.memberInfo = memberInfo;
		}
		public String PropertyName {
			get {
				if(memberInfo != null) {
					return memberInfo.Name;
				}
				else {
					return propertyName;
				}
			}
		}
		public IMemberInfo MemberInfo {
			get { return memberInfo; }
		}
		public Object OldValue {
			get { return oldValue; }
		}
		public Object NewValue {
			get { return newValue; }
		}
	}
	public class ValueStoringEventArgs : EventArgs {
		private Object oldValue;
		private Object newValue;
		public ValueStoringEventArgs(Object oldValue, Object newValue) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		public Object OldValue {
			get { return oldValue; }
		}
		public Object NewValue {
			get { return newValue; }
		}
	}
	public class ValidateObjectEventArgs : ObjectManipulatingEventArgs {
		private Boolean valid;
		private String errorText = "";
		public ValidateObjectEventArgs(Object obj, Boolean valid)
			: base(obj) {
			this.valid = valid;
		}
		public Boolean Valid {
			get { return valid; }
			set { valid = value; }
		}
		public String ErrorText {
			get { return errorText; }
			set { errorText = value; }
		}
	}
	public class ClientValidateObjectEventArgs : ValidateObjectEventArgs {
		public ClientValidateObjectEventArgs(Object obj, Boolean valid, IObjectSpace objectSpace)
			: base(obj, valid) {
			ObjectSpace = objectSpace;
			PropertyErrors = new Dictionary<string, string>();
		}
		public IObjectSpace ObjectSpace { get; private set; }
		public Dictionary<string, string> PropertyErrors { get; private set; }
	}
	public class ConfirmationEventArgs : EventArgs {
		private ConfirmationType confirmationType;
		private ConfirmationResult confirmationResult;
		public ConfirmationEventArgs(ConfirmationType confirmationType, ConfirmationResult confirmationResult) {
			this.confirmationType = confirmationType;
			this.confirmationResult = confirmationResult;
		}
		public ConfirmationType ConfirmationType {
			get { return confirmationType; }
		}
		public ConfirmationResult ConfirmationResult {
			get { return confirmationResult; }
			set { confirmationResult = value; }
		}
	}
	public class CompletedEventArgs : HandledEventArgs {
		private bool isCompleted = true;
		public bool IsCompleted {
			get { return isCompleted; }
			set { isCompleted = value; }
		}
	}
	public class CustomDeleteObjectsEventArgs : HandledEventArgs {
		private ICollection objects;
		public CustomDeleteObjectsEventArgs(ICollection objects) {
			this.objects = objects;
		}
		public ICollection Objects {
			get { return objects; }
		}
	}
	public class ModuleRegisteredEventArgs : EventArgs {
		private ModuleBase module;
		public ModuleRegisteredEventArgs(ModuleBase module) {
			this.module = module;
		}
		public ModuleBase Module {
			get { return module; }
		}
	}
	public class CustomizeTypesInfoEventArgs : EventArgs {
		private ITypesInfo typesInfo;
		public CustomizeTypesInfoEventArgs(ITypesInfo typesInfo) {
			this.typesInfo = typesInfo;
		}
		public ITypesInfo TypesInfo {
			get { return typesInfo; }
		}
	}
	public class RemovingTypeWithDependenciesEventArgs : EventArgs {
		private Type type;
		private ICollection<Type> dependentTypes;
		private ICollection<Type> requiredTypes;
		public RemovingTypeWithDependenciesEventArgs(Type type, ICollection<Type> dependentTypes, ICollection<Type> requiredTypes) {
			this.type = type;
			this.dependentTypes = dependentTypes;
			this.requiredTypes = requiredTypes;
		}
		public Type Type {
			get {
				return type;
			}
		}
		public ICollection<Type> DependentTypes {
			get {
				return dependentTypes;
			}
		}
		public ICollection<Type> RequiredTypes {
			get {
				return requiredTypes;
			}
		}
	}
	public class UsingTypeEventArgs : EventArgs {
		private Type type;
		public UsingTypeEventArgs(Type type) {
			this.type = type;
		}
		public Type Type {
			get {
				return type;
			}
		}
	}
	public class UsingAssemblyEventArgs : EventArgs {
		private Assembly assembly;
		public UsingAssemblyEventArgs(Assembly assembly) {
			this.assembly = assembly;
		}
		public Assembly Assembly {
			get {
				return assembly;
			}
		}
	}
	public delegate void UsingTypeEventHandler(object sender, UsingTypeEventArgs e);
	public delegate bool RemovingTypeWithDependenciesEventHandler(object sender, RemovingTypeWithDependenciesEventArgs e);
	public delegate void UsingAssemblyEventHandler(object sender, UsingAssemblyEventArgs e);
	public class ViewChangingEventArgs : EventArgs {
		private Frame sourceFrame;
		private View view;
		private bool disposeOldView;
		public ViewChangingEventArgs(View view, Frame sourceFrame, bool disposeOldView) {
			this.sourceFrame = sourceFrame;
			this.view = view;
			this.disposeOldView = disposeOldView;
		}
		public Frame SourceFrame {
			get { return sourceFrame; }
		}
		public View View {
			get { return view; }
		}
		public bool DisposeOldView {
			get { return disposeOldView; }
			set { disposeOldView = value; }
		}
	}
	public class ViewChangedEventArgs : EventArgs {
		private Frame sourceFrame;
		public ViewChangedEventArgs(Frame sourceFrame) {
			this.sourceFrame = sourceFrame;
		}
		public Frame SourceFrame {
			get { return sourceFrame; }
		}
	}
	public class ProcessActionContainerEventArgs : EventArgs {
		private IActionContainer actionContainer;
		public ProcessActionContainerEventArgs(IActionContainer actionContainer) {
			this.actionContainer = actionContainer;
		}
		public IActionContainer ActionContainer {
			get { return actionContainer; }
		}
	}
	public class DatabaseUpdaterEventArgs : EventArgs {
		public DatabaseUpdaterEventArgs(DatabaseUpdaterBase updater) {
			Updater = updater;
		}
		public DatabaseUpdaterBase Updater { get; set; }
	}
	public class DatabaseUpdaterManagerEventArgs : EventArgs {
		public DatabaseUpdaterManagerEventArgs(DatabaseUpdaterManager manager) {
			Manager = manager;
		}
		public DatabaseUpdaterManager Manager { get; set; }
	}
	public class CustomizeLanguagesListEventArgs : EventArgs {
		private IList<string> languages;
		public CustomizeLanguagesListEventArgs(IList<string> languages) {
			this.languages = languages;
		}
		public IList<string> Languages {
			get { return languages; }
		}
	}
	public class CustomHandleServiceExceptionEventArgs : HandledEventArgs {
		public CustomHandleServiceExceptionEventArgs(object service, Exception e) {
			this.Exception = e;
			this.Service = service;
		}
		public Exception Exception { get; private set; }
		public object Service { get; private set; }
	}
}
