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
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Utils;
namespace DevExpress.Utils.MVVM.Design {
	public class MVVMContextDesigner : BaseComponentDesigner {
		public MVVMContext Context {
			get { return Component as MVVMContext; }
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			CheckDevExpressMvvmReference();
			if(savedViewModelType != null)
				Context.ViewModelType = savedViewModelType;
			Context.ContainerControl = RootContainerControl;
			if(savedViewModelType != null)
				Save(GetService<EnvDTE.DTE>());
			savedViewModelType = null;
		}
		protected void CheckDevExpressMvvmReference() {
			EnvDTE.Project project = GetProject();
			if(project == null) return;
			try {
				if(!ProjectHelper.IsReferenceExists(project, AssemblyInfo.SRAssemblyXpfMvvm))
					ProjectHelper.AddReference(project, AssemblyInfo.SRAssemblyXpfMvvm + AssemblyInfo.FullAssemblyVersionExtension);
			}
			catch { }
		}
		internal EnvDTE.Project GetProject() {
			var item = GetService<EnvDTE.ProjectItem>();
			return (item != null) ? item.ContainingProject : null;
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new MVVMContextDesignerActionList(Component, this));
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
		public Type ViewModelType {
			get { return Context.ViewModelType; }
			set {
				if(Context.ViewModelType == value) return;
				EditorContextHelper.SetPropertyValue(this, Component, "ViewModelType", value);
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		public bool ShowGeneratedCode { get; set; }
		protected T GetService<T>() where T : class {
			return ServiceProvider.GetService(typeof(T)) as T;
		}
		protected internal void GoToViewModel() {
			var dte = GetService<EnvDTE.DTE>();
			if(dte != null && dte.Solution.IsOpen) {
				var currentProject = DTEHelper.GetCurrentProject(dte);
				if(currentProject != null) {
					var codeClass = GetCodeClass(currentProject.ProjectItems, Context.ViewModelType.FullName);
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
		protected internal bool HasViewModel() {
			string rootClassName = DesignerHost.RootComponentClassName;
			string viewModelClasName = rootClassName + "ViewModel";
			var dte = GetService<EnvDTE.DTE>();
			return ((dte != null && dte.Solution.IsOpen)) &&
				GetCodeClass(dte.ActiveDocument.ProjectItem.FileCodeModel, viewModelClasName) != null;
		}
		protected internal void RemoveViewModel() {
			ViewModelType = null;
			string rootClassName = DesignerHost.RootComponentClassName;
			string viewModelClasName = rootClassName + "ViewModel";
			var dte = GetService<EnvDTE.DTE>();
			if((dte != null && dte.Solution.IsOpen)) {
				var projectItem = dte.ActiveDocument.ProjectItem;
				EnvDTE.FileCodeModel fileCodeModel = projectItem.FileCodeModel;
				var viewModelClass = GetCodeClass(fileCodeModel, viewModelClasName);
				if(viewModelClass != null) {
					try {
						Save(dte);
						fileCodeModel.Remove(viewModelClass);
						var rootClass = GetCodeClass(fileCodeModel, rootClassName);
						if(rootClass != null)
							rootClass.RemoveMember("ViewModel");
						Save(dte, true);
					}
					catch { }
				}
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		protected internal void AddViewModel() {
			string rootClassName = DesignerHost.RootComponentClassName;
			string viewModelClasName = rootClassName + "ViewModel";
			var dte = GetService<EnvDTE.DTE>();
			if(dte != null && dte.Solution.IsOpen) {
				var projectItem = dte.ActiveDocument.ProjectItem;
				EnvDTE.FileCodeModel fileCodeModel = projectItem.FileCodeModel;
				var rootClass = GetCodeClass(fileCodeModel, rootClassName);
				if(rootClass != null) {
					int? codeLine = null;
					var viewModelClass = GetCodeClass(fileCodeModel, viewModelClasName);
					if(viewModelClass == null) {
						try {
							Save(dte);
							viewModelClass = AddClass(rootClass);
							codeLine = AddAccessor(rootClass, viewModelClass, Context.Site.Name);
							Save(dte, true);
						}
						catch { }
					}
					if(viewModelClass != null) {
						var viewModelType = ResolveType(projectItem, viewModelClass);
						if(ShowGeneratedCode && codeLine.HasValue)
							ShowCode(codeLine.Value);
						ViewModelType = viewModelType;
						if(viewModelType == null)
							ShowMessage(messageAdd);
						else
							savedViewModelType = viewModelType;
					}
				}
			}
		}
		const string messageAdd = "A new  ViewModel's class has been created successfully. Rebuild the current solution to continue configuring the MVVMContext.";
		static void ShowMessage(string text) {
			DevExpress.Design.UI.MessageWindow.Show(new MVVMContextMessageWindowViewModel(text));
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
		internal static EnvDTE.CodeClass AddClass(EnvDTE.CodeClass rootClass, string suffix = "ViewModel") {
			return rootClass.Namespace.AddClass(rootClass.Name + suffix, -1, null, null, EnvDTE.vsCMAccess.vsCMAccessPublic);
		}
		static int AddAccessor(EnvDTE.CodeClass rootClass, EnvDTE.CodeClass viewModelClass, string componentName) {
			try {
				var accessor = rootClass.AddProperty("ViewModel", null, viewModelClass.FullName, -1, EnvDTE.vsCMAccess.vsCMAccessPublic, null);
				EnvDTE.TextPoint start = accessor.Getter.GetStartPoint(EnvDTE.vsCMPart.vsCMPartBody);
				EnvDTE.TextPoint finish = accessor.Getter.GetEndPoint(EnvDTE.vsCMPart.vsCMPartBody);
				string code = null;
				if(rootClass.Language == EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp)
					code = "return " + componentName + ".GetViewModel<" + viewModelClass.Name + ">();";
				if(rootClass.Language == EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB)
					code = "Return " + componentName + ".GetViewModel(Of" + viewModelClass.Name + ")()";
				var editPoint = start.CreateEditPoint();
				editPoint.ReplaceText(finish, code, (int)EnvDTE.vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
				editPoint.StartOfLine();
				editPoint.SmartFormat(finish);
				return editPoint.Line;
			}
			catch { return -1; }
		}
		static void Save(EnvDTE.DTE dte, bool build = false) {
			try {
				dte.ActiveDocument.Save();
				if(build)
					dte.Solution.SolutionBuild.Build(true);
			}
			catch { }
		}
		internal static EnvDTE.CodeClass GetCodeClass(EnvDTE.ProjectItems projectItems, string fullName) {
			if(projectItems == null)
				return null;
			try {
				foreach(EnvDTE.ProjectItem item in projectItems) {
					if(item.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile) {
						try {
							var codeClass = GetCodeClass(item.FileCodeModel, fullName);
							if(codeClass != null)
								return codeClass;
						}
						catch { continue; }
					}
					if(item.SubProject != null) {
						var codeClass = GetCodeClass(item.SubProject.ProjectItems, fullName);
						if(codeClass != null)
							return codeClass;
					}
					else {
						var codeClass = GetCodeClass(item.ProjectItems, fullName);
						if(codeClass != null)
							return codeClass;
					}
				}
				return null;
			}
			catch { return null; }
		}
		internal static EnvDTE.CodeClass GetCodeClass(EnvDTE.FileCodeModel fileCodeModel, string fullName) {
			if(fileCodeModel == null)
				return null;
			foreach(EnvDTE.CodeClass codeClass in GetClasses(fileCodeModel.CodeElements))
				if(codeClass.FullName == fullName) return codeClass;
			return null;
		}
		static IEnumerable<EnvDTE.CodeClass> GetClasses(EnvDTE.CodeElements codeElements) {
			foreach(EnvDTE.CodeElement element in codeElements) {
				if(element.Kind == EnvDTE.vsCMElement.vsCMElementClass)
					yield return (EnvDTE.CodeClass)element;
				else {
					var codeType = element as EnvDTE.CodeType;
					if(codeType != null) {
						foreach(EnvDTE.CodeClass childClass in GetClasses(codeType.Members))
							yield return childClass;
						break;
					}
					var codeNamespace = element as EnvDTE.CodeNamespace;
					if(codeNamespace != null) {
						foreach(EnvDTE.CodeClass childClass in GetClasses(codeNamespace.Members))
							yield return childClass;
						break;
					}
				}
			}
		}
		protected internal bool CanCreateCommandBindingExpressions {
			get {
				return (Context.ViewModelType != null) &&
					System.Linq.Enumerable.Any(FindComponents<Utils.Menu.IDXMenuManager>(Context.Container));
			}
		}
		protected internal bool HasCommandBindingExpressions {
			get { return System.Linq.Enumerable.Any(Context.BindingExpressions, e => e is CommandBindingExpression); }
		}
		protected internal Utils.Menu.IDXMenuManager DXMenuManager {
			get { return FindComponent<Utils.Menu.IDXMenuManager>(Context.Container); }
		}
		protected internal void EditBindingExpressions() {
			EditorContextHelper.EditValue(this, Context, "BindingExpressions");
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		protected internal void RetrieveCommandBindingExpressions() {
			var menuManager = DXMenuManager;
			if(menuManager != null) {
				using(var transaction = DesignerHost.CreateTransaction("Populating Binding Expressions")) {
					try {
						Context.PopulateCommandBindingExpressions();
						IBarCommandSupports barCommandSupport = DesignerHost.GetDesigner((IComponent)menuManager) as IBarCommandSupports;
						if(barCommandSupport != null) {
							foreach(BindingExpression be in Context.BindingExpressions) {
								var cbe = be as CommandBindingExpression;
								if(cbe != null)
									CreateBarButtonItem(barCommandSupport, cbe);
							}
							transaction.Commit();
						}
						else transaction.Cancel();
					}
					catch { transaction.Cancel(); }
				}
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		void CreateBarButtonItem(IBarCommandSupports barCommandSupport, CommandBindingExpression expression) {
			var barButtonItem = barCommandSupport.CommandContext.AddBarItem<BarButtonItem>(expression.GroupName, expression.DisplayName);
			barButtonItem.Description = expression.Description;
			barButtonItem.ImageUri = new DxImageUri(expression.ImageNameOrUri);
			if(barButtonItem.Site != null) {
				try { barButtonItem.Site.Name = "bbi" + expression.Name; }
				catch { }
			}
			expression.Target = barButtonItem;
		}
		protected internal bool CanCreateServiceRegistrationExpressions {
			get {
				return (Context.ViewModelType != null) &&
					System.Linq.Enumerable.Any(FindComponents<Utils.MVVM.Services.IDocumentAdapterFactory>(Context.Container));
			}
		}
		protected internal bool HasServiceRegistrationExpressions {
			get { return System.Linq.Enumerable.Any(Context.RegistrationExpressions, e => e is ServiceRegistrationExpression); }
		}
		protected internal void RetrieveServiceRegistrationExpressions() {
			using(var transaction = DesignerHost.CreateTransaction("Populating Service Registrations")) {
				try {
					var factories = FindComponents<Utils.MVVM.Services.IDocumentAdapterFactory>(Context.Container);
					foreach(var f in factories) {
						var re = RegistrationExpression.RegisterDocumentManagerService(null, false, f);
						Context.RegistrationExpressions.Add(re);
					}
					transaction.Commit();
				}
				catch { transaction.Cancel(); }
			}
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		protected internal void EditRegistrationExpressions() {
			EditorContextHelper.EditValue(this, Context, "RegistrationExpressions");
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		static T FindComponent<T>(IContainer container) where T : class {
			if(container == null) return null;
			foreach(object obj in container.Components) {
				if(typeof(T).IsAssignableFrom(obj.GetType()))
					return (T)obj;
			}
			return null;
		}
		static IEnumerable<T> FindComponents<T>(IContainer container) where T : class {
			if(container == null)
				yield break;
			foreach(object obj in container.Components) {
				if(typeof(T).IsAssignableFrom(obj.GetType()))
					yield return (T)obj;
			}
		}
	}
	public class MVVMContextDesignerActionList : DesignerActionList {
		MVVMContextDesigner designerCore;
		public MVVMContextDesignerActionList(IComponent component, MVVMContextDesigner designer)
			: base(component) {
			designerCore = designer;
		}
		protected MVVMContext Context {
			get { return Component as MVVMContext; }
		}
		protected MVVMContextDesigner Designer {
			get { return designerCore; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			bool hasViewModel = Designer.HasViewModel();
			if(Context.ViewModelType == null) {
				res.Add(new DesignerActionMethodItem(this, "AddViewModel", hasViewModel ? "Assign ViewModel" : "Add ViewModel"));
				if(!hasViewModel)
					res.Add(new DesignerActionPropertyItem("ShowGeneratedCode", "Show generated code-behind"));
			}
			if(hasViewModel)
				res.Add(new DesignerActionMethodItem(this, "RemoveViewModel", "Remove ViewModel"));
			res.Add(new DesignerActionPropertyItem("ViewModelType", "View Model"));
			res.Add(new DesignerActionPropertyItem("ShowAllTypes", "Show All Types"));
			res.Add(new DesignerActionPropertyItem("HideNamespaces", "Hide Namespaces"));
			if(Context.ViewModelType != null) {
				res.Add(new DesignerActionMethodItem(this, "GoToViewModel", "Go To ViewModel"));
				res.Add(new DesignerActionHeaderItem("Bindings && Commands", "BindingExpressions"));
				if(Designer.CanCreateCommandBindingExpressions) {
					if(!Designer.HasCommandBindingExpressions)
						res.Add(new DesignerActionMethodItem(this, "RetrieveCommandBindingExpressions", "Retrieve Commands", "BindingExpressions"));
				}
				res.Add(new DesignerActionMethodItem(this, "EditBindingExpressions", "Edit...", "BindingExpressions"));
				res.Add(new DesignerActionHeaderItem("Services, Behaviors && Messages", "RegistrationExpressions"));
				if(Designer.CanCreateServiceRegistrationExpressions) {
					if(!Designer.HasServiceRegistrationExpressions)
						res.Add(new DesignerActionMethodItem(this, "RetrieveServiceRegistrationExpressions", "Retrieve Services", "RegistrationExpressions"));
				}
				res.Add(new DesignerActionMethodItem(this, "EditRegistrationExpressions", "Edit...", "RegistrationExpressions"));
			}
			return res;
		}
		[TypeConverter(typeof(ViewModelSourceObjectTypeConverter))]
		public Type ViewModelType {
			get { return Designer.ViewModelType; }
			set { Designer.ViewModelType = value; }
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public void GoToViewModel() {
			Designer.GoToViewModel();
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
			Designer.AddViewModel();
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public void RemoveViewModel() {
			Designer.RemoveViewModel();
		}
		public bool ShowGeneratedCode {
			get { return Designer.ShowGeneratedCode; }
			set { Designer.ShowGeneratedCode = value; }
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public void RetrieveCommandBindingExpressions() {
			Designer.RetrieveCommandBindingExpressions();
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public void EditBindingExpressions() {
			Designer.EditBindingExpressions();
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public void RetrieveServiceRegistrationExpressions() {
			Designer.RetrieveServiceRegistrationExpressions();
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public void EditRegistrationExpressions() {
			Designer.EditRegistrationExpressions();
		}
	}
	class MVVMContextMessageWindowViewModel : DevExpress.Design.UI.MessageWindowViewModel {
		string message;
		public MVVMContextMessageWindowViewModel(string message)
			: base(null) {
			this.message = message;
		}
		public override string Title { get { return "MVVMContext"; } }
		public override string Message { get { return message; } }
		public override string ButtonText { get { return "Ok"; } }
		public override bool? OwnerWindowResult { get { return false; } }
	}
}
