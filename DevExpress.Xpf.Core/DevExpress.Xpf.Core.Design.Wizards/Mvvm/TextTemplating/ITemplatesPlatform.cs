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
using EnvDTE;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using DevExpress.Design.Mvvm;
using DevExpress.Utils.Design;
using DevExpress.Design.Mvvm.Wizards;
using DevExpress.Design.Mvvm.Wizards.UI;
using DevExpress.Utils.Format;
using DevExpress.Xpf.Core.Design.Wizards.Utils;
using System.Xml;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TextManager.Interop;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
using System.Xml.Linq;
using DevExpress.MarkupUtils.Design;
using System.ComponentModel.Design;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm {
	public class MvvmProjectItem {
		EnvDTE.ProjectItem item;
		public MvvmProjectItem() {
		}
		internal MvvmProjectItem(EnvDTE.ProjectItem item) {
			this.item = item;
		}
		public void Save() {
			if(item == null)
				return;
			item.Save();
		}
	}
	public interface ITemplatesPlatform {
		string GetTargetFolder();
		MvvmProjectItem AddFromFile(string fileName, bool rebuildNeeded = false);
		string GetDefaultNamespace();
		string GetProjectNamespace();
		string GetProjectFolder();
		string GetEmbeddedResourcesPathOverride(string fileFolder);
		void AddAssemblyReference(string referenceName, bool copyLocal = false);
		bool AddFileAndDependencies(string fileName, bool rebuildNeeded, IDictionary<string, string> properties, params string[] dependencies);
		bool IsItemExist(string uniqueName);
		void AddLicXFile();
		string FormatXaml(string xaml);
		bool RewriteExistingFiles { get; }
		bool IsVisualBasic { get; }
		IWcfEdmInfoProvider WcfEdmInfoProvider { get; }
	}
	public static class TemplatesPlatformExtensions {
		public static void AddAssemblyReferences(this ITemplatesPlatform templatePlatform, params string[] referenceNames) {
			if(referenceNames == null) return;
			foreach(var item in referenceNames)
				templatePlatform.AddAssemblyReference(item);
		}
		public static string GetFileExtensionForActiveProject(this ITemplatesPlatform templatePlatform) {
			return templatePlatform.IsVisualBasic ? DTETemplatesPlatform.STR_VbFileExt : DTETemplatesPlatform.STR_CSharpFileExt;
		}
		public static string GetEmbeddedResourcesPath(this ITemplatesPlatform templatePlatform, string fileFolder) {
			return templatePlatform.IsVisualBasic ? string.Empty : templatePlatform.GetEmbeddedResourcesPathOverride(fileFolder) ?? templatePlatform.GetProjectNamespace() + "." + fileFolder;
		}
	}
	public class DTETemplatesPlatform : ITemplatesPlatform, IWcfEdmInfoProvider {
		const string STR_Rootnamespace = "$rootnamespace$";
		public const string STR_CSharpFileExt = "cs";
		public const string STR_VbFileExt = "vb";
		IServiceContainer serviceContainer;
		UIHierarchy solutionExplorer;
		Window solutionExplorerWindow;
		public DTETemplatesPlatform(IServiceContainer serviceContainer) {
			this.serviceContainer = serviceContainer;			
		}
		IWizardRunContext WizardRunContext { get { return (IWizardRunContext)serviceContainer.GetService(typeof(IWizardRunContext)); } }
		EnvDTE.Project TargetProject {
			get {
				EnvDTE.Project targetProject = DTEHelper.GetActiveProject();
				if(targetProject == null)
					Log.SendWarning("Could not locate target Project for template expansion result.");
				return targetProject;
			}
		}
		public bool IsVisualBasic {
			get {
				return DTEHelper.GetActiveLanguageId() == LanguageId.VB;
			}
		}
		UIHierarchy SolutionExplorer {
			get {
				if(solutionExplorer != null)
					return solutionExplorer;
				if(SolutionExplorerWindow == null)
					return null;
				solutionExplorer = SolutionExplorerWindow.Object as UIHierarchy;
				return solutionExplorer;
			}
		}
		Window SolutionExplorerWindow {
			get {
				if(solutionExplorerWindow != null)
					return solutionExplorerWindow;
				DTE dte = DTEHelper.GetCurrentDTE();
				if(dte == null)
					return null;
				solutionExplorerWindow = dte.Windows.Item("{3AE79031-E1BC-11D0-8F78-00A0C9110057}");
				return solutionExplorerWindow;
			}
		}
		void AddAssemblyReference(EnvDTE.Project project, string assemblyName, bool copyLocal) {
			if(project == null || string.IsNullOrEmpty(assemblyName))
				return;
			try {
				VSLangProj.VSProject vsProject = project.Object as VSLangProj.VSProject;
				if(vsProject == null)
					return;
				VSLangProj.Reference result = vsProject.References.Find(assemblyName);
				if(result != null)
					return;
				result = vsProject.References.Add(assemblyName);
				if(result != null)
					result.CopyLocal = copyLocal;
			}
			catch(Exception ex) {
				Log.SendException(ex);
			}
		}
		EnvDTE.ProjectItem AddFromFileInternal(string fileName) {
			try {
				if(!string.IsNullOrEmpty(fileName) && File.Exists(fileName)) {
					EnvDTE.Project project = TargetProject;
					if(project != null) {
						ProjectItem pi = project.ProjectItems.AddFromFile(fileName);
						if(pi != null) {
							VsFileFormatter.Format(DTEHelper.GetCurrentDTE(), fileName);
							pi.Save();
						}
						return pi;
					}
				}
			}
			catch(Exception ex) {
				Log.SendException(ex);
			}
			return null;
		}
		public string GetProjectFolder() {
			EnvDTE.Project project = TargetProject;
			if(project == null)
				return null;
			return Path.GetDirectoryName(project.FileName);
		}
		IEnumerable<ProjectItem> GetProjectItemsToSerchIn() {
			ProjectItem folder = GetSelectedFolder();
			if(folder != null && folder.ProjectItems != null)
				foreach(ProjectItem item in folder.ProjectItems)
					yield return item;
			folder = GetCommonFolder();
			if(folder != null && folder.ProjectItems != null)
				foreach(ProjectItem item in folder.ProjectItems) {
					yield return item;
					foreach(ProjectItem child in GetProjectItemsRecursively(item))
						yield return child;
				}
			EnvDTE.Project project = TargetProject;
			if(project != null && project.ProjectItems != null)
				foreach(ProjectItem item in project.ProjectItems)
					yield return item;
		}
		ProjectItem GetCommonFolder() {
			return GetFolderInRoot(TemplatesConstants.STR_Common);			
		}
		ProjectItem GetServiceReferencesFolder() {
			return GetFolderInRoot(TemplatesConstants.STR_ServiceReferences);
		}
		ProjectItem GetFolderInRoot(string folderName) {
			EnvDTE.Project project = TargetProject;
			if(project == null)
				return null;
			return GetFolder(project.ProjectItems, folderName);
		}
		static ProjectItem GetFolder(EnvDTE.ProjectItems items, string folderName) {
			foreach(EnvDTE.ProjectItem item in items)
				if(item != null && DTEHelper.IsPhysicalFolder(item.Kind) && item.Name == folderName)
					return item;
			return null;
		}
		IEnumerable<ProjectItem> GetProjectItemsRecursively(ProjectItem projectItem) {
			if(projectItem == null || projectItem.ProjectItems == null)
				yield break;
			ProjectItems items = projectItem.ProjectItems;
			foreach(EnvDTE.ProjectItem item in items)
				yield return item;
			foreach(EnvDTE.ProjectItem item in items)
				foreach(EnvDTE.ProjectItem child in GetProjectItemsRecursively(item))
					yield return child;
		}
		ProjectItem GetSelectedFolder() {
			UIHierarchy explorer = SolutionExplorer;
			if(explorer != null) {
				UIHierarchyItem[] selectedItems = explorer.SelectedItems as UIHierarchyItem[];
				if(selectedItems != null && selectedItems.Length == 1) {
					ProjectItem projectItem = selectedItems[0].Object as ProjectItem;
					if(projectItem != null && DTEHelper.IsPhysicalFolder(projectItem.Kind))
						return projectItem;
				}
			}
			return null;
		}
		bool IsProjectItemExist(IEnumerable<ProjectItem> items, string itemName) {			
			foreach(ProjectItem item in items)
				if(string.Equals(item.Name, itemName, StringComparison.Ordinal))
					return true;
			return false;
		}
		public void AddAssemblyReference(string referenceName, bool copyLocal = false) {
			AddAssemblyReference(TargetProject, referenceName, copyLocal);
		}
		public MvvmProjectItem AddFromFile(string fileName, bool rebuildNeeded = false) {
			EnvDTE.ProjectItem pi = AddFromFileInternal(fileName);
			if(pi != null) {
				if(rebuildNeeded)
					AddRebuildTask();
				return new MvvmProjectItem(pi);
			}
			return null;
		}
		public string GetDefaultNamespace() {
			if(WizardRunContext.ReplacementsDictionary.ContainsKey(STR_Rootnamespace))
				return WizardRunContext.ReplacementsDictionary[STR_Rootnamespace];
			return GetProjectNamespace();
		}
		public string GetProjectNamespace() {
			return ProjectHelper.GetDefaultNamespace(TargetProject);
		}
		public string GetEmbeddedResourcesPathOverride(string fileFolder) {
			return null;
		}
		public string GetTargetFolder() {
			ProjectItem projectItem = GetSelectedFolder();
			if(projectItem != null) {
				string path = ProjectHelper.GetPropertyValue(projectItem.Properties, "FullPath") as string;
				if(!string.IsNullOrEmpty(path) && Directory.Exists(path))
					return path;
			}
			return GetProjectFolder();			
		}
		List<ProjectItem> winListToOpenAndReSave = new List<ProjectItem>();
		bool AddFileAndDependenciesInternal(string fileName, IDictionary<string, string> properties, string[] dependencies) {
			EnvDTE.ProjectItem item = AddFromFileInternal(fileName);
			if(properties != null) {
				foreach(var property in properties) {
					item.Properties.Item(property.Key).Value = property.Value;
				}
			}
			if(item == null || dependencies == null || dependencies.Length == 0)
				return false;
			if(fileName.Contains(@"\Views\") && ((!fileName.EndsWith("designer.cs") && fileName.EndsWith(".cs")) || (!fileName.EndsWith("designer.vb") && fileName.EndsWith(".vb")))) {
				winListToOpenAndReSave.Add(item);
			}
			for(int i = 0; i < dependencies.Length; i++) {
				try {
					string file = dependencies[i];
					if(!string.IsNullOrEmpty(file) && File.Exists(file)) {
						ProjectItem result = item.ProjectItems.AddFromFile(file);
						if(result == null)
							return false;						
						VsFileFormatter.Format(DTEHelper.GetCurrentDTE(), file);
						result.Save();
					}
				}
				catch(Exception ex) {
					Log.SendException(ex);
					return false;
				}
			}
			return true;
		}
		public bool AddFileAndDependencies(string fileName, bool rebuildNeeded, IDictionary<string, string> properties, params string[] dependencies) {
			bool success = AddFileAndDependenciesInternal(fileName, properties, dependencies);
			if(success && rebuildNeeded)
				AddRebuildTask();
			return success;
		}
		void AddRebuildTask() {
			IWizardTaskManager taskManager = (IWizardTaskManager)this.serviceContainer.GetService(typeof(IWizardTaskManager));
			if(taskManager == null)
				return;
			List<ITask> toRemove = new List<ITask>();
			foreach(ITask task in taskManager.Tasks)
				if(task != null && task is RebuildTask && !toRemove.Contains(task))
					toRemove.Add(task);
			foreach(ITask item in toRemove)
				taskManager.Remove(item);
			taskManager.Add(new RebuildTask(serviceContainer, winListToOpenAndReSave));
		}
		public virtual bool IsItemExist(string itemName) {
			return IsProjectItemExist(GetProjectItemsToSerchIn(), itemName);
		}		
		public virtual void AddLicXFile() {
			LicXFileCreator.AddLicenseToProject(TargetProject, PropertiesFolderName);
		}
		protected virtual string PropertiesFolderName { get { return "Properties"; } }
		public string FormatXaml(string xaml) {
			string result = XamlMarkupHelper.FormatXaml(xaml);
			if(!string.IsNullOrEmpty(result))
				return result;
			return TemplatingUtils.FormatXaml(xaml);
		}
		public virtual bool RewriteExistingFiles {
			get { return false; }
		}
		ProjectItem GetServiceReferenceFolder(Type clrType) {
			if(clrType == null)
				return null;
			string[] nameSpaces = clrType.Namespace.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
			if(nameSpaces == null || nameSpaces.Length == 0)
				return null;
			string referenceName = nameSpaces[nameSpaces.Length - 1];
			ProjectItem folder = GetServiceReferencesFolder();
			if(folder == null)
				return null;
			return GetFolder(folder.ProjectItems, referenceName);
		}
		IWcfEdmInfoProvider ITemplatesPlatform.WcfEdmInfoProvider { get { return this; } }
		public Stream GetEdmItemCollectionStream(Type clrType) {
			ProjectItem folder = GetServiceReferenceFolder(clrType);
			if(folder == null)
				return null;
			foreach(ProjectItem item in GetProjectItemsRecursively(folder))
				if(item.Name == TemplatesConstants.STR_WcfServiceEdmx && item.FileCount == 1)
					return File.OpenRead(item.get_FileNames(0));			
			return null;
		}
		public string GetSourceUrl(Type clrType) {
			ProjectItem folder = GetServiceReferenceFolder(clrType);
			if(folder == null)
				return null;
			foreach(ProjectItem item in GetProjectItemsRecursively(folder))
				if(item.Name.EndsWith(TemplatesConstants.STR_WcfDatasvcmapExtension) && item.FileCount == 1)
					return WcfManifestResourceEdmInfoProvider.GetSourceUrl(File.OpenRead(item.get_FileNames(0)));
			return string.Empty;
		}		
	}
}
