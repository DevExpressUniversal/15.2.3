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
using System.IO;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraSplashForm;
using DevExpress.XtraWaitForm;
namespace DevExpress.XtraSplashScreen.Design {
	class TemplateNotFoundException : Exception {
		public TemplateNotFoundException(string message, Exception innerException)
			: base(message, innerException) {
		}
	}
	abstract class DesignTimeHelperBase {
		public static DesignTimeHelperBase Create(Mode mode) {
			if(mode == Mode.SplashScreen) return new SplashScreenDesignTimeHelper();
			return new WaitFormDesignTimeHelper();
		}
		public void AddItemTemplate(IServiceProvider serviceProvider) {
			using(DTEProjectHelper dteProject = new DTEProjectHelper(serviceProvider)) {
				dteProject.TryAddProjectItem(DataType, StrongName);
			}
		}
		protected string StrongName {
			get { return DataType.Name; }
		}
		public abstract Type DataType { get; }
	}
	class SplashScreenDesignTimeHelper : DesignTimeHelperBase {
		public override Type DataType {
			get { return typeof(SplashScreen); }
		}
	}
	class WaitFormDesignTimeHelper : DesignTimeHelperBase {
		public override Type DataType {
			get { return typeof(WaitForm); }
		}
	}
	class VSServiceHelper {
		public static void RefreshSmartTag(IComponent component) {
			DesignerActionUIService service = (DesignerActionUIService)component.Site.GetService(typeof(DesignerActionUIService));
			service.Refresh(component);
		}
		public static EnvDTE.Project GetDTEProject(IComponent component) {
			return GetDTEProject(component.Site);
		}
		public static EnvDTE.Project GetDTEProject(IServiceProvider serviceProvider) {
			EnvDTE.ProjectItem item = (EnvDTE.ProjectItem)serviceProvider.GetService(typeof(EnvDTE.ProjectItem));
			return item == null ? null : item.ContainingProject;
		}
		public static IDesignerHost GetDesignerHost(IComponent component) {
			return (IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
		}
		public static EnvDTE.ProjectItem GetDTEProjectItem(IComponent component) {
			return (EnvDTE.ProjectItem)component.Site.GetService(typeof(EnvDTE.ProjectItem));
		}
		public static Control GetRootComponent(IComponent component) {
			IDesignerHost svc = component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if (svc == null)
				return null;
			return svc.RootComponent as Control;
		}
		public static bool IsUserControlRootComponent(IComponent component) {
			Control root = GetRootComponent(component);
			return root != null && root is UserControl;
		}
	}
	class DTEProjectHelper : IDisposable {
		EnvDTE.Project project;
		IServiceProvider serviceProvider;
		public DTEProjectHelper(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
			this.project = VSServiceHelper.GetDTEProject(serviceProvider);
		}
		public string CreateUniqueItemName(Type dataType) {
			int counter = 0;
			string result = string.Empty;
			while(true) {
				result = string.Format("{0}{1}.{2}", dataType.Name, ++counter, ItemExtension);
				if(!IsTargetFileExist(result) && !IsItemExist(result))
					break;
			}
			return result;
		}
		public EnvDTE.ProjectItem GetItem(string itemName) {
			return GetItemCore(this.project.ProjectItems, itemName);
		}
		public bool IsItemExist(string itemName) {
			return GetItem(itemName) != null;
		}
		public void TryAddProjectItem(Type dataType, string strongName) {
			try {
				DXGalleryServiceProvider.Service.AddItem(this.serviceProvider, CreateUniqueItemName(dataType), strongName);
			}
			catch(Exception e) {
				throw new TemplateNotFoundException(string.Empty, e);
			}
		}
		public string ItemExtension {
			get {
				if(!IsValidLanguage) throw new InvalidOperationException("Unknown language of project");
				return IsCsLanguage ? "cs" : "vb";
			}
		}
		public string LanguageCode { get { return ItemExtension.ToUpper(); } }
		bool IsCsLanguage {
			get {
				string languageId = this.project.CodeModel.Language;
				return string.Equals(languageId, EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp, StringComparison.Ordinal);
			}
		}
		bool IsVbLanguage {
			get {
				string languageId = this.project.CodeModel.Language;
				return string.Equals(languageId, EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB, StringComparison.Ordinal);
			}
		}
		bool IsValidLanguage { get { return IsCsLanguage || IsVbLanguage; } }
		public void Save() {
			this.project.Save(string.Empty);
		}
		EnvDTE.ProjectItem GetItemCore(EnvDTE.ProjectItems items, string itemName) {
			foreach(EnvDTE.ProjectItem item in items) {
				EnvDTE.ProjectItem result = GetItemCore(item.ProjectItems, itemName);
				if(result != null)
					return result;
				if(string.Equals(item.Name, itemName, StringComparison.Ordinal))
					return item;
			}
			return null;
		}
		bool IsTargetFileExist(string itemName) {
			string target = Path.Combine(Path.GetDirectoryName(this.project.FullName), itemName);
			return File.Exists(target);
		}
		public EnvDTE80.Solution2 ExtSolution { get { return (EnvDTE80.Solution2)this.project.DTE.Solution; } }
		#region IDisposable
		public void Dispose() {
			Save();
		}
		#endregion
	}
	class ProjectResearcher : ProjectResearcherBase {
		List<EnvDTE.CodeClass> classes;
		public ProjectResearcher(EnvDTE.Project project) : base(project) {
			this.classes = new List<EnvDTE.CodeClass>();
		}
		public override void Refresh(object data) {
			this.classes.Clear();
			base.Refresh(data);
		}
		public override void ProcessCodeElement(EnvDTE.CodeElement element, object data) {
			if(element.Kind != EnvDTE.vsCMElement.vsCMElementClass)
				return;
			Type dataType = (Type)data;
			EnvDTE.CodeClass elementClass = (EnvDTE.CodeClass)element;
			if(elementClass.get_IsDerivedFrom(dataType.FullName))
				classes.Add(elementClass);
		}
		protected override void ProcessProjectItemCore(EnvDTE.CodeElements elements, object data) {
			EnvDTE.CodeElement nsCodeElement = FindNamespaceCodeElement(elements);
			if(nsCodeElement == null && elements.Count > 0) {
				ProcessCodeElement(elements.Item(1), data);
				return;
			}
			if(nsCodeElement != null && nsCodeElement.Children.Count > 0)
				ProcessCodeElement(nsCodeElement.Children.Item(1), data);
		}
		protected virtual EnvDTE.CodeElement FindNamespaceCodeElement(EnvDTE.CodeElements elements) {
			foreach(EnvDTE.CodeElement element in elements) {
				if(element.Kind == EnvDTE.vsCMElement.vsCMElementNamespace)
					return element;
			}
			return null;
		}
		public EnvDTE.CodeClass[] Classes { get { return this.classes.ToArray(); } }
		public override void Dispose() {
			if(this.classes != null)
				this.classes.Clear();
			this.classes = null;
			base.Dispose();
		}
	}
	class SplashScreensInfoStorage : IDisposable {
		List<TypeInfo> itemListCore = null;
		ProjectResearcher projectResearcherCore = null;
		public SplashScreensInfoStorage(EnvDTE.Project project) {
			this.itemListCore = new List<TypeInfo>();
			this.projectResearcherCore = new ProjectResearcher(project);
		}
		List<TypeInfo> ItemList {
			get { return this.itemListCore; }
			set { this.itemListCore = value; }
		}
		ProjectResearcher Researcher {
			get { return this.projectResearcherCore; }
			set { this.projectResearcherCore = value; }
		}
		public int ItemsCount { get { return ItemList.Count; } }
		public void Refresh() {
			ItemList.Clear();
			Researcher.Refresh(typeof(SplashFormBase));
			foreach(EnvDTE.CodeClass codeClass in Researcher.Classes) {
				Mode mode = Mode.SplashScreen;
				if(codeClass.get_IsDerivedFrom(typeof(WaitForm).FullName))
					mode = Mode.WaitForm;
				TypeInfo info = new TypeInfo(codeClass.FullName, mode);
				if(!ItemList.Contains(info)) ItemList.Add(info);
			}
		}
		public TypeInfo Parse(string value) {
			return ItemList.Find(delegate(TypeInfo form) {
				return string.Equals(form.GetShortName(), value, StringComparison.Ordinal);
			});
		}
		public bool ContainsWithShortName(TypeInfo info) {
			return Parse(info.GetShortName()) != null;
		}
		public TypeInfo this[int index] {
			get {
				TypeInfo item = ItemList[index];
				return new TypeInfo(item.TypeName, item.Mode);
			}
		}
		public TypeInfo[] Items { get { return ItemList.ToArray(); } }
		#region IDisposable
		public void Dispose() {
			ItemList = null;
			Researcher.Dispose();
			Researcher = null;
		}
		#endregion
	}
	class ComponentFinder : ProjectResearcherBase {
		IComponent component;
		EnvDTE.CodeVariable res;
		public ComponentFinder(IComponent component) : base(null) {
			this.res = null;
			this.component = component;
		}
		List<EnvDTE.CodeVariable> vars = new List<EnvDTE.CodeVariable>();
		public override void ProcessCodeElement(EnvDTE.CodeElement element, object data) {
			if(element.Kind != EnvDTE.vsCMElement.vsCMElementVariable)
				return;
			EnvDTE.CodeVariable var = (EnvDTE.CodeVariable)element;
			if(IsVarValid(var)) vars.Add(var);
		}
		protected override void ProcessProjectItemCompleted(EnvDTE.ProjectItem item, object data) {
			base.ProcessProjectItemCompleted(item, data);
			if(vars.Count == 1) this.res = vars[0];
		}
		bool IsVarValid(EnvDTE.CodeVariable var) {
			return IsNameValid(var) && IsTypeValid(var);
		}
		bool IsNameValid(EnvDTE.CodeVariable var) {
			return string.Equals(var.Name, component.Site.Name, StringComparison.Ordinal);
		}
		bool IsTypeValid(EnvDTE.CodeVariable var) {
			return string.Equals(var.Type.AsFullName, component.GetType().FullName, StringComparison.Ordinal);
		}
		public EnvDTE.CodeVariable Variable { get { return this.res; } }
	}
	class ComponentCleaner {
		public static void RemoveField(IComponent component) {
			EnvDTE.ProjectItem form = VSServiceHelper.GetDTEProjectItem(component);
			if(form == null)
				return;
			EnvDTE.ProjectItem designForm = ProjectHelper.GetDesignFormProjectItem(form);
			if(designForm == null)
				return;
			ComponentFinder finder = new ComponentFinder(component);
			finder.ProcessProjectItem(designForm, null);
			if(finder.Variable == null)
				return;
			RemoveFieldCore(finder.Variable);
		}
		static void RemoveFieldCore(EnvDTE.CodeVariable var) {
			EnvDTE.CodeClass codeClass = var.Parent as EnvDTE.CodeClass;
			if(codeClass != null) codeClass.RemoveMember(var);
		}
	}
	delegate void ParameterLessDelegate();
}
