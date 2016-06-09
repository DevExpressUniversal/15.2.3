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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
namespace DevExpress.Utils.Design {
	[CLSCompliant(false)]
	public static class ProjectHelper {
		static readonly string[] projectKindsWin = new string[] { 
			"{F184B08F-C81C-45F6-A57F-5ABD9991F28F}",
			"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"};
		const string projectKindCpp = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
		const string projectKindWeb = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
		static readonly string[] projectKindsLS = new string[] {
			"{ECD6D718-D1CF-4119-97F3-97C25A0DFBF9}", 
			"{8D9C8A9D-5DED-4B64-BF65-B598BA549D93}", 
			"{8A4D642E-824A-4D58-BD56-648CC30C8F17}"};
		const string projectKindSolutionFolder = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";		
		static string webApplicationExtenderName = "WebApplication";
		const string STR_AssemblyName = "AssemblyName";
		const string STR_DefaultNamespace = "DefaultNamespace";
		public static bool IsManagedCppProject(EnvDTE.Project project) {
			if(project == null) return false;
			return string.Equals(project.Kind, projectKindCpp, StringComparison.OrdinalIgnoreCase);
		}
		public static bool IsWebApplication(IServiceProvider provider) {
			return IsWebApplication(GetActiveProject(provider));
		}
		public static bool IsWebApplication(Project project) {
			if(project != null && project.ExtenderNames is string[])
				return Array.IndexOf((string[])project.ExtenderNames, webApplicationExtenderName) >= 0;
			return false;
		}
		public static bool IsWebProject(IServiceProvider provider) {
			return IsWebProject(GetActiveProject(provider));
		}
		public static bool IsWebProject(Project project) {
			return project != null && project.Kind.ToUpper() == projectKindWeb;
		}
		public static bool IsWinProject(Project project) {
			return project != null && projectKindsWin.Contains<string>(project.Kind.ToUpper());
		}
		public static bool IsLightSwitchProject(IServiceProvider provider) {
			Project project = GetActiveProject(provider);
			return project != null && IsLightSwitchEnvironment(project.DTE);
		}
		public static bool IsLightSwitchEnvironment(EnvDTE.DTE dte) {
			if(dte != null && dte.Solution != null) {
				return IsLightSwitchEnvironmentRecursive(dte.Solution.Projects);
			}
			return false;
		}
		public static bool IsSolutionItemsProject(EnvDTE.Project project) {
			return project != null && project.Kind == EnvDTE.Constants.vsProjectKindMisc;
		}
		static bool IsLightSwitchEnvironmentRecursive(IEnumerable projects) {
			foreach (Project project in projects) {			   
				if (IsLightSwitchProjectCore(project) || (IsSolutionFolder(project) && IsLightSwitchEnvironmentRecursive(GetSubprojects(project))))
					return true;
			}
			return false;
		}
		static bool IsSolutionFolder(Project project) {
			return project != null && string.Equals(project.Kind, projectKindSolutionFolder, StringComparison.OrdinalIgnoreCase);
		}
		static IEnumerable<Project> GetSubprojects(Project project) {
			foreach (ProjectItem projectItem in project.ProjectItems) {
				yield return projectItem.SubProject;
			}
		}
		public static EnvDTE.Project GetActiveProject(IServiceProvider sp) {
			if(sp == null) return null;
			EnvDTE.Project project = sp.GetService(typeof(EnvDTE.Project)) as EnvDTE.Project;
			if(project != null) return project;
			EnvDTE.ProjectItem item = (EnvDTE.ProjectItem)sp.GetService(typeof(EnvDTE.ProjectItem));
			return item != null ? item.ContainingProject : null;
		}
		public static bool IsDebuggerLaunched(IServiceProvider sp) {
			EnvDTE.DTE dte = sp.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
			if(dte == null)
				return false;
			EnvDTE.Processes processes = dte.Debugger.DebuggedProcesses;
			return processes != null && processes.Count > 0;
		}
		public static EnvDTE.ProjectItem GetDesignFormProjectItem(EnvDTE.ProjectItem form) {
			foreach(EnvDTE.ProjectItem item in form.ProjectItems) {
				if(IsFormDesignProjectItem(form, item))
					return item;
			}
			return null;
		}
		static bool IsFormDesignProjectItem(EnvDTE.ProjectItem form, EnvDTE.ProjectItem designerForm) {
			return IsCsFormDesignProjectItem(form, designerForm) || IsVbFormDesignProjectItem(form, designerForm);
		}
		static bool IsCsFormDesignProjectItem(EnvDTE.ProjectItem form, EnvDTE.ProjectItem designerForm) {
			string sample = string.Concat(GetFormNameWithoutExtension(form), ".Designer.cs");
			return string.Equals(designerForm.Name, sample, StringComparison.Ordinal);
		}
		static bool IsVbFormDesignProjectItem(EnvDTE.ProjectItem form, EnvDTE.ProjectItem designerForm) {
			string sample = string.Concat(GetFormNameWithoutExtension(form), ".Designer.vb");
			return string.Equals(designerForm.Name, sample, StringComparison.Ordinal);
		}
		static string GetFormNameWithoutExtension(EnvDTE.ProjectItem form) {
			return Path.GetFileNameWithoutExtension(form.Name);
		}
		static bool IsLightSwitchProjectCore(Project project) {
			return project != null && projectKindsLS.Any<string>(item => {
				return string.Equals(project.Kind, item, StringComparison.OrdinalIgnoreCase);
			});
		}
		private static string filterTypeName = "";
		public static CodeType[] FindCodeElements(ProjectItems projectItems, string filterTypeName) {
			ProjectHelper.filterTypeName = filterTypeName;
			List<CodeType> elements = new List<CodeType>();
			PickCodeElements(projectItems, elements);
			return elements.ToArray();
		}
		private static void PickCodeElements(ProjectItems projectItems, List<CodeType> elements) {
			if(projectItems == null)
				return;
			for(int i = 1; i <= projectItems.Count; i++) {
				ProjectItem projectItem = projectItems.Item(i);
				PickCodeElements(projectItem, elements);
				PickCodeElements(projectItem.ProjectItems, elements);
			}
		}
		private static void PickCodeElements(ProjectItem projectItem, List<CodeType> elements) {
			try {
				if(projectItem != null && projectItem.FileCodeModel != null && projectItem.FileCodeModel.CodeElements != null)
					PickCodeTypeElements(projectItem.FileCodeModel.CodeElements, elements);
			} catch { }
		}
		private static void PickCodeTypeElements(CodeElements codeElements, List<CodeType> elements) {
			for(int i = 1; i <= codeElements.Count; i++) {
				CodeElement codeElement = codeElements.Item(i);
				if(MatchCodeElement(codeElement as CodeType))
					elements.Add((CodeType)codeElement);
				if(codeElement.Kind == vsCMElement.vsCMElementNamespace && codeElement.Children != null) {
					PickCodeTypeElements(codeElement.Children, elements);
				}
			}
		}
		private static bool MatchCodeElement(CodeType codeType) {
			return (codeType == null || codeType.Kind != vsCMElement.vsCMElementClass) ? false :
				(codeType.get_IsDerivedFrom(filterTypeName) || Object.Equals(codeType.FullName, filterTypeName));
		}
		public static Type TryLoadType(IServiceProvider sp, string typeName) {
			ITypeResolutionService svc = sp.GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
			if(svc == null) return null;
			return svc.GetType(typeName);
		}
		public static string GetAssemblyName(EnvDTE.Project project) {
			object val = GetPropertyValue(project, STR_AssemblyName);
			if(val != null)
				return val.ToString();
			return string.Empty;
		}
		public static string GetDefaultNamespace(EnvDTE.Project project) {
			object val = GetPropertyValue(project, STR_DefaultNamespace);
			if(val != null)
				return val.ToString();
			return string.Empty;
		}
		public static object GetPropertyValue(EnvDTE.Project project, string name) {
			if(project == null || project.Properties == null)
				return null;
			return GetPropertyValue(project.Properties, name);
		}
		public static object GetPropertyValue(EnvDTE.Properties properties, string name) {
			if(properties == null || string.IsNullOrEmpty(name))
				return null;
			foreach(EnvDTE.Property prop in properties)
				if(prop.Name == name) return prop.Value;
			return null;
		}
		public static CodeElement FindCodeElement(ProjectItem item, Func<CodeElement, bool> callback) {
			FileCodeModel codeModel = item.FileCodeModel;
			if(codeModel == null)
				return null;
			return FindCodeElementCore(codeModel.CodeElements, callback);
		}
		static CodeElement FindCodeElementCore(CodeElements elements, Func<CodeElement, bool> callback) {
			foreach(CodeElement element in elements) {
				CodeElement target = FindCodeElementCore(element.Children, callback);
				if(target != null)
					return target;
				if(callback(element))
					return element;
			}
			return null;
		}
		public static string[] GetProjectOutputsFast(IServiceProvider provider, IVsHierarchy hierarchy) {
			try {
				IVsSolutionBuildManager service = (IVsSolutionBuildManager)provider.GetService(typeof(IVsSolutionBuildManager));
				if(service == null) {
					return null;
				}
				IVsProjectCfg[] cfgArray = new IVsProjectCfg[1];
				if(service.FindActiveProjectCfg(IntPtr.Zero, IntPtr.Zero, hierarchy, cfgArray) != 0)
					return null;
				if((cfgArray[0] == null) || !(cfgArray[0] is IVsProjectCfg2))
					return null;
				IVsProjectCfg2 cfg = (IVsProjectCfg2)cfgArray[0];
				if(cfg != null) {
					IVsOutputGroup group;
					cfg.OpenOutputGroup("Built", out group);
					IVsOutputGroup2 group2 = group as IVsOutputGroup2;
					if(group2 != null) {
						IVsOutput2 output;
						if(group2.get_KeyOutputObject(out output) != 0)
							return null;
						if(output != null) {
							string str;
							if(output.get_DeploySourceURL(out str) == 0 && !string.IsNullOrEmpty(str))
								return new string[] { GetLocalPathUnescaped(str) };
						}
					}
				}
			}
			catch {
			}
			return null;
		}
		public static string GetLocalPathUnescaped(string url) {
			string str = "file:///";
			if(url.StartsWith(str, StringComparison.OrdinalIgnoreCase))
				return url.Substring(str.Length);
			return GetLocalPath(url);
		}
		public static string GetLocalPath(string fileName) {
			Uri uri = new Uri(fileName);
			return (uri.LocalPath + uri.Fragment);
		}
		public static string[] GetProjectOutputsSlow(Project project) {
			ArrayList list = new ArrayList(2);
			if(project != null) {
				ConfigurationManager configurationManager = null;
				Configuration activeConfiguration = null;
				try {
					configurationManager = project.ConfigurationManager;
				}
				catch {
				}
				if(configurationManager != null) {
					try {
						activeConfiguration = configurationManager.ActiveConfiguration;
					}
					catch {
					}
					if(activeConfiguration != null) {
						OutputGroups outputGroups = null;
						try {
							outputGroups = activeConfiguration.OutputGroups;
						}
						catch {
						}
						try {
							if(outputGroups != null) {
								int count = outputGroups.Count;
								for(int i = 0; i < count; i++) {
									OutputGroup group = outputGroups.Item(i + 1);
									string canonicalName = group.CanonicalName;
									if((group.FileCount > 0) && (string.Equals("Built", canonicalName, StringComparison.OrdinalIgnoreCase))) {
										Array fileURLs = group.FileURLs as Array;
										int length = fileURLs.Length;
										for(int j = 0; j < length; j++) {
											string url = fileURLs.GetValue(j) as string;
											url = GetLocalPathUnescaped(url);
											if((string.Equals(Path.GetExtension(url), ".exe", StringComparison.OrdinalIgnoreCase) || string.Equals(Path.GetExtension(url), ".dll", StringComparison.OrdinalIgnoreCase)) && !list.Contains(url)) {
												list.Add(url);
											}
										}
									}
								}
							}
						}
						catch {
						}
					}
				}
			}
			string[] strArray = new string[list.Count];
			list.CopyTo(strArray);
			return strArray;
		}
		public static void AddReference(IComponent component, string assembly) {
			if(component == null) return;
			AddReference(component.Site, assembly);
		}
		public static void AddReference(IServiceProvider provider, string assembly) {
			if(provider == null) return;
			var project = GetActiveProject(provider);
			if(project == null) return;
			AddReference(project, assembly);
		}
		public static void AddReference(EnvDTE.Project project, string assemblyName) {
			if(IsWebProject(project))
				AddReference(project.Object as VsWebSite.VSWebSite, assemblyName);
			else
				AddReference(project.Object as VSLangProj.VSProject, assemblyName);
		}
		static void AddReference(VSLangProj.VSProject vsproj, string assemblyName) {
			if(vsproj == null || IsReferenceExists(vsproj, GetShortName(assemblyName)))
				return;
			try {
				vsproj.References.Add(assemblyName);
			} catch {
			}
		}
		public static bool IsReferenceExists(EnvDTE.Project project, string assemblyName) {
			return IsWebProject(project) ? IsReferenceExists(project.Object as VsWebSite.VSWebSite, assemblyName) :
				IsReferenceExists(project.Object as VSLangProj.VSProject, assemblyName);
		}
		static bool IsReferenceExists(VSLangProj.VSProject vsproj, string assemblyName) {
			if(vsproj == null) return false;
			foreach(VSLangProj.Reference reference in vsproj.References) {
				if(string.Equals(reference.Name, assemblyName, StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}
		static void AddReference(VsWebSite.VSWebSite webSite, string assemblyName) {
			if(webSite == null || IsReferenceExists(webSite, GetShortName(assemblyName)))
				return;
			try {
				webSite.References.AddFromGAC(assemblyName);
			} catch {
			}
		}
		static bool IsReferenceExists(VsWebSite.VSWebSite webSite, string assemblyName) {
			if(webSite == null) return false;
			foreach(VsWebSite.AssemblyReference reference in webSite.References) {
				if(string.Equals(reference.Name, assemblyName, StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}
		static string GetShortName(string assemblyName) {
			AssemblyName asmName = new AssemblyName(assemblyName);
			return asmName.Name;
		}
		public static ProjectItems GetSubItems(ProjectItems items, string subItemName) {
			if(items == null) return null;
			foreach(ProjectItem item in items) {
				if(string.Equals(item.Name, subItemName, StringComparison.Ordinal))
					return item.ProjectItems;
			}
			return null;
		}
		public static void AddReferenceFromFile(EnvDTE.Project project, string path) {
			if(IsWebProject(project))
				AddReferenceFromFile(project.Object as VsWebSite.VSWebSite, path);
			else
				AddReferenceFromFile(project.Object as VSLangProj.VSProject, path);
		}
		static void AddReferenceFromFile(VsWebSite.VSWebSite webSite, string path) {
			if(webSite == null || IsReferenceExists(webSite, Path.GetFileNameWithoutExtension(path)))
				return;
			try {
				webSite.References.AddFromFile(path);
			} catch {
			}
		}
		static void AddReferenceFromFile(VSLangProj.VSProject vsproj, string path) {
			if(vsproj == null || IsReferenceExists(vsproj, Path.GetFileNameWithoutExtension(path)))
				return;
			try {
				vsproj.References.Add(path);
			} catch {
			}
		}
		public static bool IsAnotherDxVerionReferenced(EnvDTE.Project project, ref string referenceInfo) {
			if(project == null)
				return false;
			Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
			if(IsWebProject(project))
				return IsAnotherDxVerionReferenced(project.Object as VsWebSite.VSWebSite, currentVersion, ref referenceInfo);
			return IsAnotherDxVerionReferenced(project.Object as VSLangProj.VSProject, currentVersion, ref referenceInfo);
		}
		static bool IsAnotherDxVerionReferenced(VSLangProj.VSProject vsproj, Version currentVersion, ref string referenceInfo) {
			if(vsproj == null) 
				return false;
			foreach(VSLangProj.Reference reference in vsproj.References) {
				if(reference.Name.StartsWith("DevExpress", StringComparison.OrdinalIgnoreCase) && 
					reference.Name.Contains(string.Format("v{0}.{1}", reference.MajorVersion, reference.MinorVersion)) &&
					reference.MajorVersion != 0
					&& (reference.MajorVersion != currentVersion.Major || reference.MinorVersion != currentVersion.Minor || reference.BuildNumber != currentVersion.Build)
					) {
					referenceInfo = reference.Name + " " + reference.MajorVersion + "." + reference.MinorVersion + "." + reference.BuildNumber;
					return true;
				}
			}
			return false;
		}
		static bool IsAnotherDxVerionReferenced(VsWebSite.VSWebSite webSite, Version currentVersion, ref string referenceInfo) {
			if(webSite == null)
				return false;
			Regex r = new Regex(@"Version=(\d{1,2})\.(\d).(\d{1,2})", RegexOptions.IgnoreCase);
			foreach(VsWebSite.AssemblyReference reference in webSite.References) {
				if(reference.Name.StartsWith("DevExpress", StringComparison.OrdinalIgnoreCase)) {
					Match m = r.Match(reference.StrongName);
					if(m.Success
						&& (m.Groups[1].Value != currentVersion.Major.ToString() || m.Groups[2].Value != currentVersion.Minor.ToString() || m.Groups[3].Value != currentVersion.Build.ToString())
						) {
						referenceInfo = reference.Name + " " + m.Groups[1].Value + "." + m.Groups[2].Value + "." + m.Groups[3].Value;
						return true;
					}
				}
			}
			return false;
		}
		public static bool IsInClientProfile(EnvDTE.Project project) { 
			if(IsWebProject(project))
				return false;
			try {
				EnvDTE.Property property = project.Properties.Item("TargetFrameworkMoniker");
				return ((string)property.Value).Contains("Profile=Client");
			} catch {
				return false;
			}
		}
	}
	[CLSCompliant(false)]
	public abstract class ProjectResearcherBase : IDisposable {
		EnvDTE.Project project;
		public ProjectResearcherBase(EnvDTE.Project project) {
			this.project = project;
		}
		public virtual void Refresh(object data) {
			RefreshCore(this.project.ProjectItems, data);
			Completed(data);
		}
		public virtual void ProcessProjectItem(EnvDTE.ProjectItem item, object data) {
			EnvDTE.FileCodeModel codeModel = GetFileCodeModel(item);
			if(codeModel == null)
				return;
			ProcessProjectItemCore(codeModel.CodeElements, data);
			ProcessProjectItemCompleted(item, data);
		}
		protected EnvDTE.FileCodeModel GetFileCodeModel(EnvDTE.ProjectItem item) {
			EnvDTE.FileCodeModel codeModel = null;
			try {
				codeModel = item.FileCodeModel;
			}
			catch {
			}
			return codeModel;
		}
		protected virtual void RefreshCore(EnvDTE.ProjectItems items, object data) {
			foreach(EnvDTE.ProjectItem item in items) {
				RefreshCore(item.ProjectItems, data);
				if(AllowTraverse(item)) ProcessProjectItem(item, data);
			}
		}
		protected virtual bool AllowTraverse(ProjectItem item) {
			return true;
		}
		protected virtual void ProcessProjectItemCore(EnvDTE.CodeElements elements, object data) {
			foreach(EnvDTE.CodeElement element in elements) {
				ProcessProjectItemCore(element.Children, data);
				ProcessCodeElement(element, data);
			}
		}
		protected virtual void Completed(object data) { }
		protected virtual void ProcessProjectItemCompleted(EnvDTE.ProjectItem item, object data) { }
		public abstract void ProcessCodeElement(EnvDTE.CodeElement element, object data);
		#region IDisposable
		public virtual void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			this.project = null;
		}
	}
}
