#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.Design;
using System.Xml;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils.About;
using DevExpress.Web.Design.Forms;
using DevExpress.Web.Internal;
using EnvDTE;
namespace DevExpress.Web.Design {
	public class ASPxWebControlDesignerBase : ControlDesigner {
		static ASPxWebControlDesignerBase() {
		}
		public WebFormsRootDesigner DesignerRoot {
			get { return RootDesigner; }
		}
		public IDesignerHost DesignerHost {
			get { return GetService(typeof(IDesignerHost)) as IDesignerHost; }
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			RegisterLookAndFeel(component);
			CheckNew(this, component, RootDesigner);
			if(RootDesigner != null) {
				if(RootDesigner.IsLoading)
					base.RootDesigner.LoadComplete += new EventHandler(OnDesignerLoadComplete);
				else
					OnDesignerLoadComplete(null, EventArgs.Empty);
			}
			IComponentChangeService service = (IComponentChangeService)component.Site.GetService(typeof(IComponentChangeService));
			if(service != null) {
				service.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
				service.ComponentRemoving += new ComponentEventHandler(OnComponentRemoving);
				service.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
				service.ComponentChanged += new ComponentChangedEventHandler(OnAnyComponentChanged);
			}
		}
		void RegisterLookAndFeel(IComponent component) {
			if(component == null || component.Site == null)
				return;
			var service = component.Site.GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
			var host = component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(service == null && host != null) {
				service = new VSLookAndFeelService(host);
				host.AddService(typeof(ILookAndFeelService), service);
			}
		}
		internal static void CheckNew(ControlDesigner designer, IComponent component, WebFormsRootDesigner rootDesigner) {
			if(rootDesigner == null || component == null || component.Site == null)
				return;
			if(rootDesigner.IsLoading)
				return;
		}
		protected override void Dispose(bool disposing) {
			if(disposing && Component != null && Component.Site != null) {
				if(RootDesigner != null)
					RootDesigner.LoadComplete -= new EventHandler(OnDesignerLoadComplete);
				IComponentChangeService service = (IComponentChangeService)base.Component.Site.GetService(typeof(IComponentChangeService));
				if(service != null) {
					service.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
					service.ComponentRemoving -= new ComponentEventHandler(OnComponentRemoving);
					service.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
					service.ComponentChanged -= new ComponentChangedEventHandler(OnAnyComponentChanged);
				}
			}
			base.Dispose(disposing);
		}
		protected System.Windows.Forms.DialogResult ShowDialog(System.Windows.Forms.Form form) {
			return DesignUtils.ShowDialogOnInvokeTransactedChange(form, Component);
		}
		protected T GetService<T>() {
			return (T)GetService(typeof(T));
		}
		protected virtual void OnDesignerLoadComplete(object sender, EventArgs e) {
		}
		protected virtual void OnAnyComponentChanged(object sender, ComponentChangedEventArgs ce) {
		}
		protected virtual void OnComponentAdded(object sender, ComponentEventArgs e) {
		}
		protected virtual void OnComponentRemoved(object sender, ComponentEventArgs e) {
		}
		protected virtual void OnComponentRemoving(object sender, ComponentEventArgs e) {
		}
		protected internal void FireControlPropertyChanged(string propertyName) {
			System.Web.UI.Design.ControlDesigner.InvokeTransactedChange(Component, delegate(object arg) {
				TypeDescriptor.Refresh(Component);
				EditorContextHelper.FireChanged(this, Component, propertyName);
				EditorContextHelper.RefreshSmartPanel(Component);
				return true;
			}, null, string.Format("{0} changed", propertyName));
		}
	}
	public class ASPxWebControlDesigner : ASPxWebControlDesignerBase, IDataBindingSchemaProvider {
		public const string CompressedJsHelpFileName = "DevExpress.Web.dxhelp";
		private FormsManager formsManager = null;
		private ASPxWebControl fControl = null;
		private bool fIsComponentChanged = false;
		private Dictionary<string, string> fPropertyNameToCaptionMap = null;
		private DataBindingSchema fSchema = null;
		DTE dteService;
		public sealed override DesignerActionListCollection ActionLists {
			get { return CreateActionLists(); }
		}
		public ASPxWebControl Control {
			get { return fControl; }
		}
		protected virtual bool ReuseBaseActionList {
			get { return true; }
		}
		public FormsManager FormsManager {
			get {
				if(formsManager == null)
					formsManager = CreateFormsManager();
				return formsManager;
			}
		}
		public sealed override TemplateGroupCollection TemplateGroups {
			get {
				return CreateTemplateGroups();
			}
		}
		public Dictionary<string, string> PropertyNameToCaptionMap {
			get {
				if(fPropertyNameToCaptionMap == null) {
					fPropertyNameToCaptionMap = new Dictionary<string, string>();
					FillPropertyNameToCaptionMap(fPropertyNameToCaptionMap);
				}
				return fPropertyNameToCaptionMap;
			}
		}
		protected override bool UsePreviewControl {
			get { return !IsRootDesignerDummy(); }
		}
		DTE DteService {
			get {
				if(dteService == null)
					dteService = (DTE)GetService(typeof(DTE));
				return dteService;
			}
		}
		static ASPxWebControlDesigner() {
			BinaryStorage.Strategies[BinaryStorage.DesignModeStrategyName] = new DesignModeStorageStrategy();
		}
		public override void Initialize(IComponent component) {
			fControl = (ASPxWebControl)component;
			base.Initialize(component);
			EnsureControlReferences();
			if(NeedCopyFormsOnInitialize())
				CopyUserControlsToWebSite();
			WebConfigManager webConfigManager = new WebConfigManager(DesignerHost);
			bool webConfigUpdated = false;
			if(!(webConfigManager.IsConfigTrackOutdated() && webConfigManager.LoadWebConfigXmlDocument()))
				return;
			if(webConfigManager.IsHashChanged()) {
				webConfigUpdated = EnsureHttpHandlerRegistered(webConfigManager) || webConfigUpdated;
				webConfigUpdated = webConfigManager.XmlHelper.RegisterDxSections() || webConfigUpdated;
				webConfigUpdated = webConfigManager.XmlHelper.UpdateDxSections() || webConfigUpdated;
				webConfigUpdated = EnsureWebConfigReferences(webConfigManager) || webConfigUpdated;
			} 
			else if(IsControlRequireHttpHandlerRegistration())
				webConfigUpdated = EnsureHttpHandlerRegistered(webConfigManager) || webConfigUpdated;
			if(webConfigUpdated) {
				webConfigManager.SaveWebConfig();
				webConfigManager.UpdateConfigModificationsTrackingInfo();
			}
		}
		protected bool EnsureHttpHandlerRegistered(WebConfigManager manager) {
			bool changed = false;
			if(IsWebConfigRequireModification(manager)) {
				AddHttpHandler(false, false, manager);
				changed = true;
			}
			return changed;
		}
		protected virtual Design.FormsManager CreateFormsManager() {
			return new FormsManager(GetFormsInfoArray(), NeedConfirmDialog(), DesignerHost);
		}
		public virtual void EnsureControlReferences() {
			EnsureReferences(
				AssemblyInfo.SRAssemblyData,
				AssemblyInfo.SRAssemblyWeb,
				AssemblyInfo.SRAssemblyRichEditCore,
				AssemblyInfo.SRAssemblyPrintingCore,
				AssemblyInfo.SRAssemblyASPxThemes
			);
		}
		public virtual bool EnsureWebConfigReferences(WebConfigManager manager) {
			bool updated = false;
			updated = manager.XmlHelper.AddReference(AssemblyInfo.SRAssemblyASPxThemesFull) || updated;
			return updated;
		}
		public virtual string GetFormCaption(string propertyName) {
			if(!PropertyNameToCaptionMap.ContainsKey(propertyName))
				return "";
			return string.Format(
				StringResources.EditorFormBase_CaptionPattern,
				this.Component.GetType().Name,
				PropertyNameToCaptionMap[propertyName]);
		}
		protected virtual TemplateGroupCollection CreateTemplateGroups() {
			return base.TemplateGroups;
		}
		protected DesignerActionListCollection CreateActionLists() {
			DesignerActionListCollection list = new DesignerActionListCollection();
			if(ReuseBaseActionList)
				list.AddRange(base.ActionLists);
			RegisterActionLists(list);
			DevExpress.Utils.Design.DXSmartTagsHelper.CreateDefaultLinks(this, list);
			list.Add(CreateAboutActionList(false));
			return list;
		}
		protected virtual void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(CreateCommonActionList());
		}
		protected virtual ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ASPxWebControlDesignerActionList(this);
		}
		protected DesignerActionList CreateAboutActionList(bool isExpiredTrial) {
			return new AboutActionList(this, isExpiredTrial);
		}
		public void AddHttpHandler() {
			AddHttpHandler(true);
		}
		public void AddHttpHandler(bool prompt) {
			AddHttpHandler(prompt, true);
		}
		public virtual void AddHttpHandler(bool prompt, bool showErrorMessage) {
			AddHttpHandler(prompt, showErrorMessage, null);
		}
		public virtual void AddHttpHandler(bool prompt, bool showErrorMessage, WebConfigManager webConfigManager) {
			if(webConfigManager == null) {
				webConfigManager = new WebConfigManager(DesignerHost);
				if(!webConfigManager.LoadWebConfigXmlDocument()) {
					if(showErrorMessage)
						MessageBoxEx.Show(null, StringResources.ActionList_AddingHttpModuleToWebConfigErrorDialogText,
						StringResources.ActionList_InvalidFormattedConfigurationFileDialogCaption,
						MessageBoxButtonsEx.OK);
					return;
				}
			}
			if(IsWebConfigRequireModification(webConfigManager)) {
				bool addModule = !prompt || MessageBoxEx.Show(null, StringResources.ActionList_AddHttpHandlerMessageText,
					StringResources.ActionList_AddHttpHandlerMessageBoxCaption,
					MessageBoxButtonsEx.OKCancel) == DialogResultEx.OK;
				if(addModule) {
					this.ModifyWebConfig(webConfigManager);
					webConfigManager.SaveWebConfig();
				}
			} else if(prompt) {
				MessageBoxEx.Show(null, StringResources.ActionList_AddHttpHandlerExistMessageText,
					StringResources.ActionList_AddHttpHandlerMessageBoxCaption,
					MessageBoxButtonsEx.OK);
			}
		}
		protected virtual bool IsControlRequireHttpHandlerRegistration() {
			return false;
		}
		protected virtual bool IsWebConfigRequireModification(WebConfigManager manager) {
			return !manager.XmlHelper.IsHttpModuleExists() ||
					(IsControlRequireHttpHandlerRegistration() && !manager.XmlHelper.IsHttpHandlerExists());
		}
		protected virtual void ModifyWebConfig(WebConfigManager manager) {
			manager.XmlHelper.RegisterHttpModule(false);
			manager.XmlHelper.RegisterHttpModule(true);
			manager.XmlHelper.RegisterHttpModuleHandler(false);
			manager.XmlHelper.RegisterHttpModuleHandler(true);
			if(IsControlRequireHttpHandlerRegistration()) {
				manager.XmlHelper.RegisterHttpHandler(false);
				manager.XmlHelper.RegisterHttpHandler(true);
			}
		}
		public virtual string Theme {
			get {
				return Control.Theme;
			}
		}
		public virtual void ShowAbout() {
			AboutDialogHelper.ShowAbout(Component.Site);
		}
		public virtual bool IsThemableControl() {
			return true;
		}
		public virtual bool HasCommonDesigner() {
			return fControl is IControlDesigner;
		}
		public virtual bool HasClientSideEvents() {
			return !HasCommonDesigner();
		}
		public virtual bool HasCopyDefaultDialogFormsToTheProject() {
			return false;
		}
		public virtual bool HasAbout() {
			return true;
		}
		public virtual void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new CommonFormDesigner(Control, DesignerHost)));
		}
		public void RunDesigner(string windowCaption) {
			var designer = new CommonFormDesigner(Control, DesignerHost);
			ShowDialog(new WrapperEditorForm(designer, windowCaption));
		}
		protected void CopyUserControlsToWebSite() {
			if(!FormsManager.IsProjectSupported())
				return;
			string[] newFiles = FormsManager.CopyUserControlsToWebSite();
			if(newFiles.Length > 0)
				NotifyNewFilesAdded(newFiles);
		}
		public void CopyDefaultDialogFormsToTheProject() {
			FormsInfo[] copiedForms = FormsManager.GenerateDefaultUserControlsToWebSite();
			IComponentChangeService changeService = Control.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			changeService.OnComponentChanging(Control, null);
			foreach(FormsInfo formsInfo in copiedForms) {
				Object settingsForms = GetControlSettingsForms(formsInfo);
				if(settingsForms != null) {
					MethodInfo methodInfo = settingsForms.GetType().GetMethod("SetFormPath", BindingFlags.Instance | BindingFlags.NonPublic);
					if(methodInfo != null) {
						foreach(string formName in formsInfo.FormNames) {
							string path = CommonUtils.GetDefaultFormUrl(formName, formsInfo.Type);
							methodInfo.Invoke(settingsForms, new object[] { formName, path });
						}
					}
				}
			}
			changeService.OnComponentChanged(Control, null, null, null);
		}
		protected virtual Object GetControlSettingsForms(FormsInfo formsInfo) {
			return null;
		}
		protected virtual FormsInfo[] GetFormsInfoArray() {
			return new FormsInfo[0];
		}
		protected virtual bool NeedCopyFormsOnInitialize() {
			return true;
		}
		protected virtual bool NeedConfirmDialog() {
			return true;
		}
		protected internal virtual void NotifyNewFilesAdded(string[] newFiles) {
		}
		public sealed override string GetDesignTimeHtml() {
			return GetDesignTimeHtml(null);
		}
		public sealed override string GetDesignTimeHtml(DesignerRegionCollection regions) {
			try {
				CreateAndPrepareControlHierarchy();
				if(regions != null)
					AddDesignerRegions(regions);
				return GetDesignTimeHtmlInternal();
			} catch(Exception e) {
				return GetErrorDesignTimeHtml(e);
			}
		}
		protected sealed override string GetEmptyDesignTimeHtml() {
			return GetEmptyDesignTimeHtmlInternal();
		}
		protected virtual string GetDesignTimeHtmlInternal() {
			return GetDesignTimeHtmlWithStyle(base.GetDesignTimeHtml());
		}
		protected virtual string GetEmptyDesignTimeHtmlInternal() {
			string baseProperty = GetBaseProperty();
			if(baseProperty != "")
				return CreatePlaceHolderDesignTimeHtml("Use the <b>" + baseProperty + "</b> property to create the control's contents.");
			return base.GetEmptyDesignTimeHtml();
		}
		protected internal string GetDesignTimeHtmlWithStyle(string html) {
			ASPxWebControl control = (ViewControl as ASPxWebControl);
			if(control != null) {
				int pos = html.LastIndexOf("</td>");
				if(pos == -1)
					pos = html.LastIndexOf("</");
				if(pos != -1)
					html = html.Insert(pos, control.GetStyleSheetsDesignHtml());
			}
			return html;
		}
		private void CreateAndPrepareControlHierarchy() {
			ResourceManager.ClearRegisteredCssResources();
			OnBeforeControlHierarchyCompleted();
			CompleteControlHierarchy();
		}
		protected virtual void OnBeforeControlHierarchyCompleted() {
		}
		public virtual string GetEditableDesignerRegionContent(ControlCollection controls) {
			var builder = new StringBuilder();
			foreach(Control control in controls) {
				CallEndInitRecursive(control);
				builder.Append(ControlPersister.PersistControl(control, DesignerHost));
			}
			return builder.ToString();
		}
		public virtual void SetEditableDesignerRegionContent(ControlCollection controls, string content) {
			SetEditableDesignerRegionContent(controls, content, "");
		}
		public virtual void SetEditableDesignerRegionContent(ControlCollection controls, string content, string propertyToChange) {
			controls.Clear();
			content = PrepareEditableContent(content);
			if(!string.IsNullOrEmpty(content)) {
				var parsedControls = ControlParser.ParseControls(DesignerHost, content);
				if(parsedControls != null)
					foreach(var control in parsedControls)
						controls.Add(control);
			}
			PropertyChanged(propertyToChange);
		}
		string PrepareEditableContent(string content) {
			if(string.IsNullOrEmpty(content))
				return string.Empty;
			var result = content.Trim();
			if(string.IsNullOrEmpty(result))
				return string.Empty;
			return new Regex(">(.*?)</input>").Replace(new Regex("</input>(\\s+)<").Replace(result, "</input><"), "/>");
		}
		static void CallEndInitRecursive(Control control) {
			IEndInitAccessor accessor = control as IEndInitAccessor;
			if(accessor != null)
				accessor.EndInit();
			if(!(control is ASPxWebControl) || control is IEndInitAccessorContainer)
				foreach(Control childControl in control.Controls)
					CallEndInitRecursive(childControl);
		}
		public System.Windows.Forms.Form CreateEditorForm(TypeEditorBase editor, string propertyName, object value) {
			return editor.CreateEditorForm(Component, GetTypeDescriptorContext(propertyName), DesignerHost, value);
		}
		public System.Windows.Forms.Form CreateEditorForm(string propertyName, object propertyOwner, object value) {
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(propertyOwner)[propertyName];
			TypeEditorBase editor = (TypeEditorBase)propertyDescriptor.GetEditor(typeof(UITypeEditor));
			fIsComponentChanged = false;
			return editor.CreateEditorForm(Component, GetTypeDescriptorContext(propertyName), DesignerHost, value);
		}
		protected System.Windows.Forms.Form CreateSubPropertyEditorForm(string subPropertyName, object propertyValue) {
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(propertyValue)[subPropertyName];
			object nestedProperty = propertyDescriptor.GetValue(propertyValue);
			TypeDescriptorContext typeDescriptorContext = new TypeDescriptorContext(null, propertyValue, propertyDescriptor);
			TypeEditorBase editor = (TypeEditorBase)propertyDescriptor.GetEditor(typeof(UITypeEditor));
			return editor.CreateEditorForm(Component, typeDescriptorContext, DesignerHost, nestedProperty);
		}
		public System.Windows.Forms.Form CreateEditorForm(string propertyName, object value) {
			return CreateEditorForm(propertyName, Component, value);
		}
		protected internal TypeDescriptorContext GetTypeDescriptorContext(string propertyName) {
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(Component)[propertyName];
			return new TypeDescriptorContext(null, Component, propertyDescriptor);
		}
		protected virtual void AddDesignerRegions(DesignerRegionCollection regions) {
		}
		protected virtual void CompleteControlHierarchy() {
			(ViewControl as ASPxWebControl).CompleteControlHierarchy();
		}
		protected virtual void RecreateControlHierarchy() {
			(ViewControl as ASPxWebControl).RecreateControlHierarchy();
		}
		protected virtual void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
		}
		protected virtual string GetBaseProperty() {
			return "";
		}
		protected override Control CreateViewControl() {
			ASPxWebControl control = base.CreateViewControl() as ASPxWebControl;
			control.InitInternal();
			if(!string.IsNullOrEmpty(control.Theme))
				control.ApplyTheme(control.Theme, false);
			else {
				string webConfigCustomThemeAssemblies = WebConfigHelper.GetDesignTimeWebConfigCustomThemeAssemblies(this);
				ThemesProvider.LoadCustomThemeAssemblies(webConfigCustomThemeAssemblies);
				string webConfigStyleSheetTheme = WebConfigHelper.GetDesignTimeWebConfigStyleSheetTheme(this);
				string webConfigTheme = WebConfigHelper.GetDesignTimeWebConfigTheme(this);
				control.ApplyTheme(webConfigStyleSheetTheme, false);
				control.ApplyTheme(webConfigTheme, false);
			}
			return control;
		}
		protected internal void PropertyChanged(string propName) {
			if(!string.IsNullOrEmpty(propName))
				Control.PropertyChanged(propName);
		}
		IDataSourceViewSchema System.Web.UI.Design.IDataBindingSchemaProvider.Schema {
			get {
				if(IsItemSchema()) {
					if(fSchema == null)
						fSchema = new DataBindingSchema(this);
					return fSchema;
				} else
					return GetSchema();
			}
		}
		bool System.Web.UI.Design.IDataBindingSchemaProvider.CanRefreshSchema {
			get {
				if(IsItemSchema())
					return false;
				else
					return CanRefreshSchema();
			}
		}
		void System.Web.UI.Design.IDataBindingSchemaProvider.RefreshSchema(bool preferSilent) {
			if(IsItemSchema())
				fSchema = null;
			else
				RefreshSchema(preferSilent);
		}
		protected virtual bool IsItemSchema() {
			return GetDataBindingSchemaItemType() != null;
		}
		protected virtual IDataSourceViewSchema GetSchema() {
			if(fSchema == null)
				fSchema = new DataBindingSchema(this);
			return fSchema;
		}
		protected virtual bool CanRefreshSchema() {
			return false;
		}
		protected virtual void RefreshSchema(bool preferSilent) {
			fSchema = null;
		}
		protected internal void ComponentChanging() {
			if(Component == null)
				return;
			DesignServices.ComponentChanging(Component.Site, Component);
		}
		protected internal void ComponentChanged() {
			if(Component == null)
				return;
			DesignServices.ComponentChanged(Component.Site, Component);
			fIsComponentChanged = true;
		}
		protected internal bool IsComponentChanged() {
			return fIsComponentChanged;
		}
		protected internal virtual string[] GetDataBindingSchemaFields() {
			return new string[0]; 
		}
		protected internal virtual string GetDataBindingSchemaName() {
			if(GetDataBindingSchemaItemType() != null)
				return GetDataBindingSchemaItemType().Name;
			else
				return ""; 
		}
		protected internal virtual Type GetDataBindingSchemaItemType() {
			return null; 
		}
		protected virtual string GetItemText(object item) {
			return item.ToString();
		}
		protected void RegisterTagPrefix(Type type) {
			if(CanRegisterTagPrefix() && RootDesigner != null && !IsRootDesignerDummy())
				RootDesigner.ReferenceManager.RegisterTagPrefix(type);
		}
		bool CanRegisterTagPrefix() {
			var htmlWindow = DteService.ActiveWindow.Object as HTMLWindow;
			return htmlWindow == null || htmlWindow.CurrentTab == vsHTMLTabs.vsHTMLTabsDesign;
		}
		protected void EnsureReferences(params string[] assemblies) {
			if(DesignerHost != null)
				DevExpress.Utils.Design.ReferencesHelper.EnsureReferences(DesignerHost, assemblies);
		}
		protected bool IsRootDesignerDummy() {
			return (RootDesigner != null && RootDesigner.GetType().Name == "DummyRootDesigner");
		}
	}
	public class AboutActionList : DesignerActionList {
		private ASPxWebControlDesigner designer = null;
		private bool isExpiredTrial = false;
		public AboutActionList(ASPxWebControlDesigner designer)
			: this(designer, false) {
		}
		public AboutActionList(ASPxWebControlDesigner designer, bool isExpiredTrial)
			: base(designer.Component) {
			this.designer = designer;
			this.isExpiredTrial = isExpiredTrial;
		}
		public override bool AutoShow {
			get { return true; }
			set { }
		}
		protected ASPxWebControlDesigner Designer {
			get { return designer; }
		}
		protected bool IsExpiredTrial {
			get { return isExpiredTrial; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = new DesignerActionItemCollection();
			if(Designer.HasAbout())
				collection.Add(new DesignerActionMethodItem(this, "ShowAbout",
					StringResources.ActionList_About,
					StringResources.ActionList_AboutCategory, "", false));
			return collection;
		}
		protected void ShowAbout() {
			Designer.ShowAbout();
		}
	}
	public class ASPxWebControlDesignerActionList : DesignerActionList {
		private ASPxWebControlDesigner designer = null;
		public ASPxWebControlDesignerActionList(ASPxWebControlDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public override bool AutoShow {
			get { return true; }
			set { }
		}
		protected ASPxWebControlDesigner Designer {
			get { return designer; }
		}
		protected virtual string RunDesignerItemCaption {
			get { return StringResources.CommonDesigner_RunDesigner; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = new DesignerActionItemCollection();
			if(Designer.HasCommonDesigner() || Designer.HasClientSideEvents()) {
				var controlTypeName = Designer.Control.GetType().Name;
				collection.Insert(0, new DesignerActionMethodItem(this, "RunDesigner",
					RunDesignerItemCaption, RunDesignerItemCaption,
					string.Format(string.Format("Run Designer to customize {0} control", controlTypeName), controlTypeName), true));
			}
			if(Designer.HasCopyDefaultDialogFormsToTheProject())
				collection.Add(new DesignerActionMethodItem(this, "CopyDefaultDialogFormsToTheProject",
					StringResources.CopyDefaultDialogFormsToTheProject,
					StringResources.ActionList_MiscCategory,
					StringResources.CopyDefaultDialogFormsToTheProjectDescription, false));
			if(Designer.IsThemableControl())
				collection.Add(new DesignerActionPropertyItem("Theme",
					StringResources.ActionList_Theme,
					StringResources.ActionList_MiscCategory, string.Empty));
			return collection;
		}
		[Editor(typeof(ThemeUITypeEditor), typeof(UITypeEditor))]
		public string Theme {
			get {
				return Designer.Theme;
			}
		}
		protected virtual void RunDesigner() {
			Designer.RunDesigner();
		}
		protected void CopyDefaultDialogFormsToTheProject() {
			Designer.CopyDefaultDialogFormsToTheProject();
		}
		protected void ShowHelpFromUrl(string relativeHelpUrl) {
			IHelpService helpService = null;
			if(designer.Component != null && designer.Component.Site != null)
				helpService = designer.Component.Site.GetService(typeof(IHelpService)) as IHelpService;
			DesignUtils.ShowHelpFromUrl(string.Format("{0}{1}", AssemblyInfo.SRDocumentationLink, relativeHelpUrl), helpService);
		}
	}
	public class DataBindingSchema : IDataSourceViewSchema {
		private IDataSourceFieldSchema[] fSchema = null;
		private ASPxWebControlDesigner fDesigner = null;
		public DataBindingSchema(ASPxWebControlDesigner designer) {
			fDesigner = designer;
		}
		protected void FillSchema(string[] fieldNames) {
			fSchema = new IDataSourceFieldSchema[fieldNames.Length];
			PropertyDescriptorCollection collection = TypeDescriptor.GetProperties(fDesigner.GetDataBindingSchemaItemType());
			for(int i = 0; i < fieldNames.Length; i++) {
				fSchema[i] = new TypeFieldSchema(collection[fieldNames[i]]);
			}
		}
		string System.Web.UI.Design.IDataSourceViewSchema.Name {
			get { return fDesigner.GetDataBindingSchemaName(); }
		}
		IDataSourceFieldSchema[] System.Web.UI.Design.IDataSourceViewSchema.GetFields() {
			if(fSchema == null)
				FillSchema(fDesigner.GetDataBindingSchemaFields());
			return fSchema;
		}
		IDataSourceViewSchema[] System.Web.UI.Design.IDataSourceViewSchema.GetChildren() {
			return new IDataSourceViewSchema[0];
		}
	}
	public class TypeFieldSchema : IDataSourceFieldSchema {
		private PropertyDescriptor fFieldDescriptor;
		private bool fIsIdentity;
		private bool fIsNullable;
		private int fLength;
		private bool fPrimaryKey;
		private bool fRetrievedMetaData;
		public TypeFieldSchema(PropertyDescriptor fieldDescriptor) {
			fLength = -1;
			if(fieldDescriptor == null)
				throw new ArgumentNullException(StringResources.InvalidFieldDescriptor);
			fFieldDescriptor = fieldDescriptor;
		}
		private void EnsureMetaData() {
			if(!fRetrievedMetaData) {
				DataObjectFieldAttribute attribute = (DataObjectFieldAttribute)fFieldDescriptor.Attributes[typeof(DataObjectFieldAttribute)];
				if(attribute != null) {
					fPrimaryKey = attribute.PrimaryKey;
					fIsIdentity = attribute.IsIdentity;
					fIsNullable = attribute.IsNullable;
					fLength = attribute.Length;
				}
				fRetrievedMetaData = true;
			}
		}
		Type System.Web.UI.Design.IDataSourceFieldSchema.DataType {
			get {
				Type type = fFieldDescriptor.PropertyType;
				if(type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>))) {
					return type.GetGenericArguments()[0];
				}
				return type;
			}
		}
		bool System.Web.UI.Design.IDataSourceFieldSchema.Identity {
			get {
				EnsureMetaData();
				return fIsIdentity;
			}
		}
		bool System.Web.UI.Design.IDataSourceFieldSchema.IsReadOnly {
			get {
				return fFieldDescriptor.IsReadOnly;
			}
		}
		bool System.Web.UI.Design.IDataSourceFieldSchema.IsUnique {
			get {
				return false;
			}
		}
		int System.Web.UI.Design.IDataSourceFieldSchema.Length {
			get {
				EnsureMetaData();
				return fLength;
			}
		}
		string System.Web.UI.Design.IDataSourceFieldSchema.Name {
			get {
				return fFieldDescriptor.Name;
			}
		}
		bool System.Web.UI.Design.IDataSourceFieldSchema.Nullable {
			get {
				EnsureMetaData();
				Type type = fFieldDescriptor.PropertyType;
				if(!type.IsValueType || fIsNullable) {
					return true;
				}
				if(type.IsGenericType) {
					return (type.GetGenericTypeDefinition() == typeof(Nullable<>));
				}
				return false;
			}
		}
		int System.Web.UI.Design.IDataSourceFieldSchema.Precision {
			get {
				return -1;
			}
		}
		bool System.Web.UI.Design.IDataSourceFieldSchema.PrimaryKey {
			get {
				EnsureMetaData();
				return fPrimaryKey;
			}
		}
		int System.Web.UI.Design.IDataSourceFieldSchema.Scale {
			get {
				return -1;
			}
		}
	}
	internal static class WebConfigHelper {
		public static string GetDesignTimeWebConfigStyleSheetTheme(ControlDesigner designer) {
			return GetDesignTimeWebConfigThemesProperty(designer, ThemesConfigurationSection.StyleSheetThemeAttribute);
		}
		public static string GetDesignTimeWebConfigTheme(ControlDesigner designer) {
			return GetDesignTimeWebConfigThemesProperty(designer, ThemesConfigurationSection.ThemeAttribute);
		}
		public static string GetDesignTimeWebConfigCustomThemeAssemblies(ControlDesigner designer) {
			return GetDesignTimeWebConfigThemesProperty(designer, ThemesConfigurationSection.CustomThemeAssembliesAttribute);
		}
		private static string GetDesignTimeWebConfigThemesProperty(ControlDesigner designer, string propertyName) {
			string webConfigContent = GetDesignTimeWebConfigContent(designer);
			if(!string.IsNullOrEmpty(webConfigContent)) {
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(webConfigContent);
				XmlNode node = xmlDoc.SelectSingleNode("/configuration/devExpress/themes");
				return node != null ? node.Attributes[propertyName].Value : string.Empty;
			}
			return string.Empty;
		}
		private static string GetDesignTimeWebConfigContent(ControlDesigner designer) {
			if(designer.Component != null) {
				IWebApplication webApp = designer.Component.Site.GetService(typeof(IWebApplication)) as IWebApplication;
				if(webApp != null) {
					IProjectItem item = webApp.GetProjectItemFromUrl("~/web.config");
					if(item != null)
						return File.ReadAllText(item.PhysicalPath);
				}
			}
			return string.Empty;
		}
	}
}
