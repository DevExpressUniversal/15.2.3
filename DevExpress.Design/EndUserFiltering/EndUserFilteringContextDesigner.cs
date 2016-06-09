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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Utils.Filtering;
using DevExpress.Utils.MVVM;
using DevExpress.Utils.MVVM.Design;
using DevExpress.XtraBars.Navigation;
namespace DevExpress.Utils.Design {
	public class FilteringUIContextDesigner : BaseComponentDesigner {
		public FilteringUIContext Context {
			get { return Component as FilteringUIContext; }
		}
		internal EnvDTE.Project GetProject() {
			var item = GetService<EnvDTE.ProjectItem>();
			return (item != null) ? item.ContainingProject : null;
		}
		static void Save(EnvDTE.DTE dte, bool build = false) {
			try {
				dte.ActiveDocument.Save();
				if(build)
					dte.Solution.SolutionBuild.Build(true);
			}
			catch { }
		}
		protected T GetService<T>() where T : class {
			return ServiceProvider.GetService(typeof(T)) as T;
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if(savedViewModelType != null)
				Context.BaseViewModelType = savedViewModelType;
			if(savedViewModelType != null)
				Save(GetService<EnvDTE.DTE>());
			savedViewModelType = null;
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			useBaseViewModelType = BaseViewModelType != null;
			list.Add(new EndUserFilteringContextDesignerActionList(Component, this));
			base.RegisterActionLists(list);
		}
		protected ContainerControl RootContainerControl {
			get { return DesignerHost.RootComponent as ContainerControl; }
		}
		IDesignerHost designerHostCore = null;
		protected IDesignerHost DesignerHost {
			get { return designerHostCore ?? (designerHostCore = GetService<IDesignerHost>()); }
		}
		protected IServiceProvider ServiceProvider {
			get { return Component.Site; }
		}
		public Type ModelType {
			get { return Context.ModelType; }
			set {
				if(Context.ModelType == value) return;
				EditorContextHelper.SetPropertyValue(this, Component, "ModelType", value);
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		public Type BaseViewModelType {
			get { return Context.BaseViewModelType; }
			set {
				if(Context.BaseViewModelType == value) return;
				EditorContextHelper.SetPropertyValue(this, Component, "BaseViewModelType", value);
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		public IFilteringUIProvider UIProvider {
			get { return Context.Control; }
			set {
				if(Context.Control == value) return;
				EditorContextHelper.SetPropertyValue(this, Component, "Control", value);
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		public IViewModelProvider ParentViewModelProvider {
			get { return Context.ParentViewModelProvider; }
			set {
				if(Context.ParentViewModelProvider == value) return;
				EditorContextHelper.SetPropertyValue(this, Component, "ParentViewModelProvider", value);
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		public bool ShowGeneratedCode { get; set; }
		bool useBaseViewModelType;
		public bool UseBaseViewModelType {
			get { return useBaseViewModelType; }
			set {
				if(useBaseViewModelType == value) return;
				useBaseViewModelType = value;
				EditorContextHelper.SetPropertyValue(this, Component, "UseBaseViewModelType", value);
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		protected internal void GoToBaseViewModel() {
			var dte = GetService<EnvDTE.DTE>();
			if(dte != null && dte.Solution.IsOpen) {
				var currentProject = DTEHelper.GetCurrentProject(dte);
				if(currentProject != null) {
					var codeClass = MVVMContextDesigner.GetCodeClass(currentProject.ProjectItems, BaseViewModelType.FullName);
					if(codeClass != null) {
						EditorContextHelperEx.HideSmartPanel(Component);
						try {
							bool isOpenAsDesigner = codeClass.ProjectItem.get_IsOpen(EnvDTE.Constants.vsViewKindDesigner);
							if(isOpenAsDesigner) {
								var es = GetService<IEventBindingService>();
								es.ShowCode(codeClass.GetStartPoint(EnvDTE.vsCMPart.vsCMPartHeader).Line);
								return;
							}
							bool isOpenAsCode = codeClass.ProjectItem.get_IsOpen(EnvDTE.Constants.vsViewKindCode);
							if(!isOpenAsCode)
								codeClass.ProjectItem.Open(EnvDTE.Constants.vsViewKindCode);
							codeClass.ProjectItem.Document.Activate();
							var selection = codeClass.ProjectItem.Document.Selection as EnvDTE.TextSelection;
							if(selection != null)
								selection.MoveToPoint(codeClass.GetStartPoint(EnvDTE.vsCMPart.vsCMPartHeader));
						}
						catch { }
					}
				}
			}
		}
		protected internal bool HasBaseViewModel() {
			string rootClassName = DesignerHost.RootComponentClassName;
			string viewModelClasName = rootClassName + "FilteringViewModel";
			var dte = GetService<EnvDTE.DTE>();
			return ((dte != null && dte.Solution.IsOpen)) &&
				MVVMContextDesigner.GetCodeClass(dte.ActiveDocument.ProjectItem.FileCodeModel, viewModelClasName) != null;
		}
		protected internal void AddBaseViewModel() {
			string rootClassName = DesignerHost.RootComponentClassName;
			string viewModelClasName = rootClassName + "FilteringViewModel";
			var dte = GetService<EnvDTE.DTE>();
			if(dte != null && dte.Solution.IsOpen) {
				var projectItem = dte.ActiveDocument.ProjectItem;
				EnvDTE.FileCodeModel fileCodeModel = projectItem.FileCodeModel;
				var rootClass = MVVMContextDesigner.GetCodeClass(fileCodeModel, rootClassName);
				if(rootClass != null) {
					int? codeLine = null;
					var viewModelClass = MVVMContextDesigner.GetCodeClass(fileCodeModel, viewModelClasName);
					if(viewModelClass == null) {
						try {
							Save(dte);
							viewModelClass = MVVMContextDesigner.AddClass(rootClass, "FilteringViewModel");
							codeLine = viewModelClass.GetStartPoint(EnvDTE.vsCMPart.vsCMPartHeader).Line;
							Save(dte, true);
						}
						catch { }
					}
					if(viewModelClass != null) {
						var viewModelType = ResolveType(projectItem, viewModelClass);
						if(ShowGeneratedCode && codeLine.HasValue)
							ShowCode(codeLine.Value);
						BaseViewModelType = viewModelType;
						if(viewModelType == null)
							ShowMessage(messageAdd);
						else
							savedViewModelType = viewModelType;
					}
				}
			}
		}
		static void ShowMessage(string text) {
			DevExpress.Design.UI.MessageWindow.Show(new FilteringUIContextMessageWindowViewModel(text));
		}
		static Type savedViewModelType;
		Type ResolveType(EnvDTE.ProjectItem projectItem, EnvDTE.CodeClass viewModelClass) {
			Type viewModelType = DesignerHost.GetType(viewModelClass.FullName);
			if(viewModelType == null) {
				var sln = GetService<Microsoft.VisualStudio.Shell.Interop.IVsSolution>();
				if(sln != null) {
					Microsoft.VisualStudio.Shell.Interop.IVsHierarchy hier;
					if(sln.GetProjectOfUniqueName(projectItem.ContainingProject.UniqueName, out hier) == 0) {
						var dts = GetService<Microsoft.VisualStudio.Shell.Design.DynamicTypeService>();
						if(dts != null) {
							var trs = dts.GetTypeResolutionService(hier);
							if(trs != null)
								viewModelType = trs.GetType(viewModelClass.FullName);
						}
					}
				}
			}
			return viewModelType;
		}
		void ShowCode(int line) {
			var eventService = GetService<IEventBindingService>();
			if(eventService != null) {
				EditorContextHelperEx.HideSmartPanel(Component);
				eventService.ShowCode(line);
			}
		}
		const string messageAdd = "A new  filtering ViewModel's class has been created successfully. Rebuild the current solution to continue configuring the EndUserFilteringContext.";
		protected internal void RemoveBaseViewModel() {
			BaseViewModelType = null;
			string rootClassName = DesignerHost.RootComponentClassName;
			string viewModelClasName = rootClassName + "FilteringViewModel";
			var dte = GetService<EnvDTE.DTE>();
			if((dte != null && dte.Solution.IsOpen)) {
				var projectItem = dte.ActiveDocument.ProjectItem;
				EnvDTE.FileCodeModel fileCodeModel = projectItem.FileCodeModel;
				var viewModelClass = MVVMContextDesigner.GetCodeClass(fileCodeModel, viewModelClasName);
				if(viewModelClass != null) {
					try {
						Save(dte);
						fileCodeModel.Remove(viewModelClass);
						Save(dte, true);
					}
					catch { }
				}
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		protected internal void RunWizard() {
			if(DevExpress.Design.Filtering.UI.FilteringModelWizard.Run(DesignerHost, Component) != null)
				EditorContextHelperEx.HideSmartPanel(Component);
		}
		protected internal void CreateProvider(ProviderType providerType) {
			switch(providerType) {
				case ProviderType.AccordionControl:
					CreateAccordionControlProvider();
					break;
				case ProviderType.LayoutControl:
					CreateLayoutControlProvider();
					break;
				default:
					return;
			}
		}
		protected void CreateProvider(Type providerType) {
			IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			IComponentChangeService componentChangeService = GetService<IComponentChangeService>();
			if(host == null || componentChangeService == null) return;
			Control rootContainer = host.RootComponent as Control;
			if(rootContainer == null) return;
			bool flag = true;
			DesignerTransaction transaction = null;
			try {
				try {
					transaction = host.CreateTransaction();
				}
				catch(CheckoutException ex) {
					if(ex == CheckoutException.Canceled)
						return;
					throw ex;
				}
				IComponent providerComponent = host.CreateComponent(providerType);
				Control providerControl = providerComponent as Control;
				IFilteringUIProvider uiProvider = providerControl as IFilteringUIProvider;
				if(providerControl == null || uiProvider == null) {
					if(providerComponent != null)
						providerComponent.Dispose();
					return;
				}
				componentChangeService.OnComponentChanging(rootContainer, null);
				rootContainer.Controls.Add(providerControl);
				providerControl.Dock = DockStyle.Right;
				this.UIProvider = uiProvider;
				componentChangeService.OnComponentChanged(rootContainer, null, null, null);
			}
			catch {
				flag = false;
			}
			finally {
				if(flag)
					transaction.Commit();
				else
					transaction.Cancel();
			}
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		protected void CreateProviderAndAddReferences(Type providerType, string[] referencedAssemblies) {
			CheckDevExpressReferences(providerType, referencedAssemblies);
			CreateProvider(providerType);
		}
		protected void CheckDevExpressReferences(Type type, string[] referencedAssemblies) {
			EnvDTE.Project project = GetProject();
			if(project == null || referencedAssemblies == null) return;
			try {
				foreach(var reference in referencedAssemblies)
					AddReference(project, reference);
			}
			catch { }
		}
		protected void AddReference(EnvDTE.Project project, string assemblyName) {
			if(!ProjectHelper.IsReferenceExists(project, assemblyName))
				ProjectHelper.AddReference(project, assemblyName + AssemblyInfo.FullAssemblyVersionExtension);
		}
		protected void CreateAccordionControlProvider() {
			Type type = typeof(AccordionControl);
			CreateProviderAndAddReferences(type, new string[] { type.Assembly.GetName().Name, AssemblyInfo.SRAssemblyEditors });
		}
		protected void CreateLayoutControlProvider() {
			string assemblyName = AssemblyInfo.SRAssemblyLayoutControl;
			var layoutAssembly = AssemblyHelper.GetLoadedAssembly(assemblyName) ?? AssemblyHelper.LoadDXAssembly(assemblyName);
			if(layoutAssembly == null) return;
			var layoutControlType = layoutAssembly.GetType("DevExpress.XtraLayout.LayoutControl");
			if(layoutControlType == null) return;
			CreateProviderAndAddReferences(layoutControlType, new string[] { assemblyName, AssemblyInfo.SRAssemblyEditors });
		}
		protected internal enum ProviderType { AccordionControl, LayoutControl }
	}
	public class EndUserFilteringContextDesignerActionList : DesignerActionList {
		FilteringUIContextDesigner designerCore;
		public EndUserFilteringContextDesignerActionList(IComponent component, FilteringUIContextDesigner designer)
			: base(component) {
			designerCore = designer;
		}
		protected FilteringUIContext Context {
			get { return Component as FilteringUIContext; }
		}
		protected FilteringUIContextDesigner Designer {
			get { return designerCore; }
		}
		protected internal bool HasUIProvider {
			get { return Context != null && Context.Control != null; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionPropertyItem("ModelType", "Model Type", "FilteringModel"));
			res.Add(new DesignerActionHeaderItem("UI Provider", "UIProvider"));
			res.Add(new DesignerActionPropertyItem("UIProvider", "Control", "UIProvider"));
			if(!HasUIProvider) {
				res.Add(new DesignerActionMethodItem(this, "AddAccordionControlUIProvider", "Add Accordion Control", "UIProvider"));
				res.Add(new DesignerActionMethodItem(this, "AddLayoutControlUIProvider", "Add Layout Control", "UIProvider"));
			}
			else {
				res.Add(new DesignerActionMethodItem(this, "RetrieveFields", "Retrieve Fields", "UIProvider"));
			}
			res.Add(new DesignerActionHeaderItem("Filtering ViewModel", "FilteringViewModel"));
			res.Add(new DesignerActionPropertyItem("UseBaseViewModelType", "Inherit from custom ViewModel", "FilteringViewModel"));
			if(UseBaseViewModelType) {
				bool hasViewModel = Designer.HasBaseViewModel();
				if(BaseViewModelType == null) {
					res.Add(new DesignerActionMethodItem(this, "AddViewModel", hasViewModel ? "Assign Filtering ViewModel" : "Add Filtering ViewModel", "FilteringViewModel"));
					if(!hasViewModel)
						res.Add(new DesignerActionPropertyItem("ShowGeneratedCode", "Show generated code-behind", "FilteringViewModel"));
				}
				if(hasViewModel)
					res.Add(new DesignerActionMethodItem(this, "RemoveViewModel", "Remove Filtering ViewModel", "FilteringViewModel"));
				res.Add(new DesignerActionPropertyItem("BaseViewModelType", "View Model", "FilteringViewModel"));
				res.Add(new DesignerActionPropertyItem("ShowAllTypes", "Show All Types", "FilteringViewModel"));
				res.Add(new DesignerActionPropertyItem("HideNamespaces", "Hide Namespaces", "FilteringViewModel"));
				if(BaseViewModelType != null)
					res.Add(new DesignerActionMethodItem(this, "GoToViewModel", "Go To Filtering ViewModel", "FilteringViewModel"));
			}
			res.Add(new DesignerActionHeaderItem("Parent ViewModel", "ParentViewModelProvider"));
			res.Add(new DesignerActionPropertyItem("ParentViewModelProvider", "Provider", "ParentViewModelProvider"));
			return res;
		}
		[TypeConverter(typeof(ModelObjectTypeConverter))]
		public Type ModelType {
			get { return Designer.ModelType; }
			set { Designer.ModelType = value; }
		}
		[TypeConverter(typeof(ViewModelSourceObjectTypeConverter))]
		public Type BaseViewModelType {
			get { return Designer.BaseViewModelType; }
			set { Designer.BaseViewModelType = value; }
		}
		public IFilteringUIProvider UIProvider {
			get { return Designer.UIProvider; }
			set { Designer.UIProvider = value; }
		}
		public IViewModelProvider ParentViewModelProvider {
			get { return Designer.ParentViewModelProvider; }
			set { Designer.ParentViewModelProvider = value; }
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public void GoToViewModel() {
			Designer.GoToBaseViewModel();
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public bool ShowAllTypes {
			get { return ViewModelSourceObjectTypeConverter.ShowAllTypes; }
			set { ViewModelSourceObjectTypeConverter.ShowAllTypes = value; }
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public bool HideNamespaces {
			get { return ViewModelSourceObjectTypeConverter.HideNamespaces; }
			set { ViewModelSourceObjectTypeConverter.HideNamespaces = value; }
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public void AddViewModel() {
			Designer.AddBaseViewModel();
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public void RetrieveFields() {
			Context.RetrieveFields();
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public void RemoveViewModel() {
			Designer.RemoveBaseViewModel();
		}
		public bool ShowGeneratedCode {
			get { return Designer.ShowGeneratedCode; }
			set { Designer.ShowGeneratedCode = value; }
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public bool UseBaseViewModelType {
			get { return Designer.UseBaseViewModelType; }
			set { Designer.UseBaseViewModelType = value; }
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public void RunWizard() {
			Designer.RunWizard();
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public void AddAccordionControlUIProvider() {
			Designer.CreateProvider(FilteringUIContextDesigner.ProviderType.AccordionControl);
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public void AddLayoutControlUIProvider() {
			Designer.CreateProvider(FilteringUIContextDesigner.ProviderType.LayoutControl);
		}
	}
	class FilteringUIContextMessageWindowViewModel : DevExpress.Design.UI.MessageWindowViewModel {
		string message;
		public FilteringUIContextMessageWindowViewModel(string message)
			: base(null) {
			this.message = message;
		}
		public override string Title { get { return "FilteringUIContext"; } }
		public override string Message { get { return message; } }
		public override string ButtonText { get { return "Ok"; } }
		public override bool? OwnerWindowResult { get { return false; } }
	}
}
