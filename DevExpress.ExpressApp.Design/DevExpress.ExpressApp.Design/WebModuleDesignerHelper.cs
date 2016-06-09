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
using System.Text;
using System.Reflection;
using System.ComponentModel.Design;
using EnvDTE;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp.Design.Core;
using System.Web.Configuration;
using System.Xml;
namespace DevExpress.ExpressApp.Design {
	public class WebDesignerHelper {
		private const string ConvertToWebApplicationCmdGuid = "CB26E292-901A-419C-B79D-49BD45C43929";
		private string[] extensions = { ".css", ".gif", ".jpg", ".skin", ".png", ".cs", ".ascx.cs", ".ascx.vb", ".ascx", ".ascx.designer.cs", ".ascx.designer.vb" };
		private List<string> contentExtensions = new List<string>();
		private const int ConvertToWebApplicationID = 104;
		private ComponentDesigner componentDesigner;
		public ComponentDesigner ComponentDesigner {
			get { return componentDesigner; }
		}
		public WebDesignerHelper(ComponentDesigner componentDesigner) {
			this.componentDesigner = componentDesigner;
			this.contentExtensions.AddRange(new string[] { ".css", ".gif", ".jpg", ".skin", ".png" });
		}
		public EnvDTE.Project GetCurrentProject() {
			EnvDTE.ProjectItem projectItem = ((EnvDTE.ProjectItem)DesignerHost.GetService(typeof(EnvDTE.ProjectItem)));
			EnvDTE.Project project = projectItem.ContainingProject;
			return project;
		}
		public List<string> AddFiles(Assembly assembly, EnvDTE.Project project, string targetPath, params string[] templatesPaths) {
			List<string> result = new List<string>();
			List<string> templatPaths = new List<string>(templatesPaths);
			templatPaths.Sort();
			templatPaths.Reverse();
			foreach(string resourceName in assembly.GetManifestResourceNames()) {
				foreach(string path in templatPaths) {
					if(resourceName.Contains(path)) {
						string fullName = "\\" + resourceName.Replace(path, targetPath);
						string ex = string.Empty;
						foreach(string extension in extensions) {
							if(fullName.LastIndexOf(extension) > 0) {
								if(ex.Length < extension.Length) {
									ex = extension;
								}
							}
						}
						fullName = fullName.Substring(0, fullName.LastIndexOf(ex)).Replace(".", "\\") + ex;
						string addFile = CopyFileFromResourceToWebSite(resourceName, fullName, project, assembly);
						if(addFile != null) {
							result.Add(addFile); 
						}
						break;
					}
				}
			}
			return result;
		}
		public string GetCurrentLanguage(EnvDTE.Project project) {
			if(project.CodeModel.Language == CodeModelLanguageConstants.vsCMLanguageCSharp) {
				return "CS";
			}
			if(project.CodeModel.Language == CodeModelLanguageConstants.vsCMLanguageVB) {
				return "VB";
			}
			throw new NotSupportedException("Current language is not supported");
		}
		public string GetModuleAssemblyName(string assemblyName, IComponent component) {
			AssemblyName[] referencedAssemblies = component.GetType().Assembly.GetReferencedAssemblies();
			foreach(AssemblyName assemblyName_ in referencedAssemblies) {
				if(assemblyName_.Name.Contains(assemblyName)) {
					return assemblyName_.FullName;
				}
			}
			return null;
		}
		public bool IsWebSite(Project project) {
			return project.Kind == VsWebSite.PrjKind.prjKindVenusProject;
		}
		public IDesignerHost DesignerHost {
			get {
				return componentDesigner.Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			}
		}
		public object GetProjectItem(string name, object projectItemsObj) {
			EnvDTE.ProjectItems projectItems = (EnvDTE.ProjectItems)projectItemsObj;
			object result = null;
			foreach(EnvDTE.ProjectItem curItem in projectItems) {
				if(curItem.Name.ToLower() == name.ToLower()) return curItem;
				if(curItem.ProjectItems.Count > 0) { result = GetProjectItem(name, curItem.ProjectItems); }
				if(result != null) return result;
			}
			return result;
		}
		public virtual string WebConfigFullPath {
			get {
				Project currentProgect = GetCurrentProject();
				ProjectWrapper wrapper = ProjectWrapper.Create(currentProgect);
				if(wrapper is WebApplicationProjectWrapper) {
					string webConfigPath = wrapper.ConfigFilePath;
					if(ProjectWrapper.CanEditFile(new string[] { webConfigPath }, DesignerHost)) {
						return webConfigPath;
					}
				}
				return null;
			}
		}
		public void AddAssemblyInWebConfig(Type itemType) {
			String webConfigPath = WebConfigFullPath;
			if(!string.IsNullOrEmpty(webConfigPath)) {
				foreach(AssemblyName assemblyName in itemType.Assembly.GetReferencedAssemblies()) {
					System.Web.Configuration.AssemblyInfo referencedAssemblyInfo = new System.Web.Configuration.AssemblyInfo(assemblyName.FullName);
					AddAssemblyInWebConfig(referencedAssemblyInfo, webConfigPath);
				}
				System.Web.Configuration.AssemblyInfo assemblyInfo = new System.Web.Configuration.AssemblyInfo(itemType.Assembly.FullName);
				AddAssemblyInWebConfig(assemblyInfo, webConfigPath);
			}
		}
		public void AddHttpHandlerInWebConfig(string name, string path, string verb, Type type) {
			if(WebConfigFullPath == null) {
				EnvDTE.DTE dte = ((EnvDTE.DTE)DesignerHost.GetService(typeof(EnvDTE.DTE)));
				Project currentProject = GetCurrentProject();
				SolutionWrapper solutionWrapper = new SolutionWrapper(dte.Solution);
				foreach(IProjectWrapper projectWrapper in solutionWrapper.XafProjects) {
					foreach(Project project in projectWrapper.ContainsProjects()) {
						if(currentProject.FullName == project.FullName && (projectWrapper is WebApplicationWrapper || projectWrapper is WebApplicationProjectWrapper)) {
							AddHttpHandlerInWebConfig(projectWrapper.TargetPath, name, path, verb, type);
						}
					}
				}
			} else {
				AddHttpHandlerInWebConfig(WebConfigFullPath, name, path, verb, type);
			}
		}
		private void AddHttpHandlerInWebConfig(string webConfigFullPath, string name, string path, string verb, Type type) {
			XmlDocument document = LoadWebConfig(webConfigFullPath);
			XmlAttribute pathAttribute = CreateXmlAttribute(document, "path", path);
			XmlAttribute verbAttribute = CreateXmlAttribute(document, "verb", verb);
			XmlAttribute typeAttribute = CreateXmlAttribute(document, "type", type.AssemblyQualifiedName);
			XmlNodeList handlersNodeList = document.GetElementsByTagName("httpHandlers");
			XmlDocument documentHttpHandlers = document;
			string webConfigHttpHandlersFullPath = webConfigFullPath;
			if(handlersNodeList != null && handlersNodeList.Count > 0) {
				foreach(XmlAttribute atr in handlersNodeList[0].Attributes) {
					if(atr.Name == "configSource") {
						webConfigHttpHandlersFullPath = Path.Combine(Path.GetDirectoryName(webConfigFullPath), atr.Value);
						documentHttpHandlers = LoadWebConfig(webConfigHttpHandlersFullPath);
						break;
					}
				}
			}
			if(AddHttpHandler(documentHttpHandlers, "httpHandlers", type, new XmlAttribute[] { verbAttribute, pathAttribute, typeAttribute })) {
				SaveWebConfig(documentHttpHandlers, webConfigHttpHandlersFullPath);
			}
			XmlAttribute nameAttribute = CreateXmlAttribute(document, "name", name);
			XmlAttribute preConditionAttribute = CreateXmlAttribute(document, "preCondition", "integratedMode");
			XmlAttribute[] attributes = new XmlAttribute[] { nameAttribute, (XmlAttribute)verbAttribute.Clone(), (XmlAttribute)pathAttribute.Clone(), (XmlAttribute)typeAttribute.Clone(), preConditionAttribute };
			if(AddHttpHandler(document, "handlers", type, attributes)) {
				SaveWebConfig(document, webConfigFullPath);
			}
		}
		private XmlAttribute CreateXmlAttribute(XmlDocument document, string name, string value) {
			XmlAttribute attribute = document.CreateAttribute(name);
			attribute.Value = value;
			return attribute;
		}
		private bool AddHttpHandler(XmlDocument document, string sectionNamme, Type handlerType, XmlAttribute[] attributes) {
			XmlNodeList handlersNodeList = document.GetElementsByTagName(sectionNamme);
			XmlNode handlersNode = handlersNodeList.Count > 0 ? handlersNodeList[0] : null;
			if(handlersNode != null) {
				if(!IsExistHandler(handlersNode, handlerType)) {
					XmlElement handler = document.CreateElement("add");
					foreach(XmlAttribute attribute in attributes) {
						handler.SetAttributeNode(attribute);
					}
					handlersNode.AppendChild(handler);
					return true;
				}
			}
			return false;
		}
		private bool IsExistHandler(XmlNode handlersNode, Type handlerType) {
			foreach(XmlNode node in handlersNode.ChildNodes) {
				if(node.Attributes != null && node.Attributes["type"] != null && String.Equals(node.Attributes["type"].Value,handlerType.AssemblyQualifiedName, StringComparison.InvariantCultureIgnoreCase)) {
					return true;
				}
			}
			return false;
		}
		private void ExecuteMenuCommand(Guid menuGroup, int commandID, IServiceProvider provider) {
			IMenuCommandService menuCommandService = provider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			menuCommandService.GlobalInvoke(new CommandID(menuGroup, commandID));
		}
		private void SelectItemInSolutionExplorer(string appRelativePath, IServiceProvider provider) {
			EnvDTE.Project project = ((EnvDTE.ProjectItem)provider.GetService(typeof(EnvDTE.ProjectItem))).ContainingProject;
			string slnName = Path.GetFileNameWithoutExtension(project.DTE.Solution.FullName);
			string fullItemPath = slnName + "\\" + project.Name + "\\" + appRelativePath;
			EnvDTE.Window solutionExplWindow = project.DTE.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer);
			solutionExplWindow.Activate();
			EnvDTE.UIHierarchy uiHierarchy = solutionExplWindow.Object as EnvDTE.UIHierarchy;
			EnvDTE.UIHierarchyItem uiHierarchyItem = uiHierarchy.GetItem(fullItemPath);
			uiHierarchyItem.Select(EnvDTE.vsUISelectionType.vsUISelectionTypeSelect);
		}
		private string CopyFileFromResourceToWebSite(string resourceName, string targetFileNameInWebSite, EnvDTE.Project project, Assembly assembly) {
			string result = null;
			string tempFileName = Path.Combine(Path.GetDirectoryName(project.FileName), GenerateTempFileName());
			try {
				EnvDTE.ProjectItem directoryObject = CreateDirectoryProjectItemByPath(targetFileNameInWebSite,
					project);
				EnvDTE.ProjectItems projectItems = directoryObject != null ?
					directoryObject.ProjectItems : project.ProjectItems;
				if(!IsExistProjectItem(Path.GetFileName(targetFileNameInWebSite), projectItems)) {
					if(File.Exists(Path.GetDirectoryName(project.FileName) + targetFileNameInWebSite)) {
						return null;
					}
					CopyFileFromResourceToFile(assembly, resourceName, tempFileName);
					EnvDTE.ProjectItem newProjectItem;
					try {
						newProjectItem = projectItems.AddFromFileCopy(tempFileName);
					}
					catch {
						return null;
					}
					newProjectItem.Name = Path.GetFileName(targetFileNameInWebSite);
					if(contentExtensions.Contains(Path.GetExtension(targetFileNameInWebSite).ToLower())) {
						newProjectItem.Properties.Item("BuildAction").Value = VSLangProj.prjBuildAction.prjBuildActionContent;
					}
					result = targetFileNameInWebSite;
				}
			}
			finally {
				File.Delete(tempFileName);
			}
			return result;
		}
		private string GenerateTempFileName() {
			Guid guid = Guid.NewGuid();
			return guid.ToString() + ".tmp";
		}
		private void CopyFileFromResourceToFile(Assembly assembly, string resourceName, string targetFileName) {
			using(Stream fromStream = assembly.GetManifestResourceStream(resourceName)) {
				BinaryReader br = new BinaryReader(fromStream);
				using(Stream toStream = File.Create(targetFileName)) {
					BinaryWriter bw = new BinaryWriter(toStream);
					bw.Write(br.ReadBytes((int)fromStream.Length));
				}
			}
		}
		private bool IsExistProjectItem(string nameItem, ProjectItems projectItems) {
			return GetProjectItem(nameItem, projectItems) != null;
		}
		private EnvDTE.ProjectItem CreateDirectoryProjectItemByPath(string path, object curProjectObj) {
			EnvDTE.Project curProject = curProjectObj as EnvDTE.Project;
			string[] dirs = GetDirectories(path);
			EnvDTE.ProjectItem ret = null;
			foreach(string dir in dirs) {
				ProjectItems projectItems = ret != null ? ret.ProjectItems : curProject.ProjectItems;
				if(IsExistProjectItem(dir, projectItems))
					ret = GetProjectItem(dir, projectItems) as EnvDTE.ProjectItem;
				else
					ret = ret == null ? curProject.ProjectItems.AddFolder(dir, "") :
						ret.ProjectItems.AddFolder(dir, "");
			}
			return ret;
		}
		private string[] GetDirectories(string path) {
			List<string> dirs = new List<string>();
			string curPath = Path.GetDirectoryName(path);
			while(Path.GetDirectoryName(curPath) != "" && Path.GetDirectoryName(curPath) != null) {
				string curDir = curPath.Replace(Path.GetDirectoryName(curPath), "");
				dirs.Add(curDir.Replace("\\", ""));
				curPath = Path.GetDirectoryName(curPath);
			}
			dirs.Reverse();
			return dirs.ToArray();
		}
		private void ConvertUserControlsToWebApplication(EnvDTE.Project project, string targetPath, List<string> addedFiles) {
			if(!IsWebSite(project) && addedFiles.Count > 0) {
				string appFolderPath = targetPath.Replace('.', '\\');
				EnvDTE.ProjectItem directoryObject = CreateDirectoryProjectItemByPath(appFolderPath, project);
				if(directoryObject != null) {
					SelectItemInSolutionExplorer(appFolderPath, DesignerHost);
					ExecuteMenuCommand(new Guid(ConvertToWebApplicationCmdGuid), ConvertToWebApplicationID, DesignerHost);
					Application.DoEvents();
				}
			}
		}
		private bool IsReferencedAssemblyExist(System.Web.Configuration.AssemblyInfo existAssembly, System.Web.Configuration.AssemblyInfo newAssembly) {
			return existAssembly.Assembly.ToLower() == newAssembly.Assembly.ToLower() ||
				newAssembly.Assembly.StartsWith("mscorlib");
		}
		private void SaveWebConfig(XmlDocument document, string webConfigFullPath) {
			if(File.Exists(webConfigFullPath)) {
				using(FileStream stream = new FileStream(webConfigFullPath, FileMode.Open)) {
					XmlWriterSettings settings = new XmlWriterSettings();
					settings.Encoding = Encoding.Default;
					settings.CloseOutput = false;
					settings.Indent = true;
					settings.NewLineHandling = NewLineHandling.Entitize;
					settings.OmitXmlDeclaration = true;
					using(XmlWriter writer = XmlWriter.Create(stream, settings)) {
						document.Save(writer);
					}
				}
			}
		}
		protected internal virtual XmlDocument LoadWebConfig(string webConfigFullPath) {
			XmlDocument document = new XmlDocument();
			document.Load(webConfigFullPath);
			return document;
		}
		protected void AddAssemblyInWebConfig(System.Web.Configuration.AssemblyInfo newAssembly, string webConfigPath) {
			bool isReferencedAssemblyExist = false;
			CompilationSection compilationSection = (CompilationSection)WebConfigurationManager.OpenWebConfiguration("").GetSection("system.web/compilation");
			foreach(System.Web.Configuration.AssemblyInfo existAssembly in compilationSection.Assemblies) {
				isReferencedAssemblyExist = IsReferencedAssemblyExist(existAssembly, newAssembly);
				if(isReferencedAssemblyExist) {
					break;
				}
			}
			if(isReferencedAssemblyExist) {
				return;
			}
			System.Configuration.ExeConfigurationFileMap exeConfigurationFileMap = new System.Configuration.ExeConfigurationFileMap();
			exeConfigurationFileMap.ExeConfigFilename = webConfigPath;
			System.Configuration.Configuration configuration = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, System.Configuration.ConfigurationUserLevel.None);
			compilationSection = (CompilationSection)configuration.GetSection("system.web/compilation");
			foreach(System.Web.Configuration.AssemblyInfo existAssembly in compilationSection.Assemblies) {
				isReferencedAssemblyExist = IsReferencedAssemblyExist(existAssembly, newAssembly); ;
				if(isReferencedAssemblyExist) {
					break;
				}
			}
			if(!isReferencedAssemblyExist) {
				compilationSection.Assemblies.Add(newAssembly);
				configuration.Save();
			}
		}
		#region Obsolete 12.2
		[Obsolete("Use the AddFiles method instead."), Browsable(false)]
		public void GetAddedFiles(Assembly assembly, EnvDTE.Project project, string targetPath, List<string> addedFiles, params string[] templatesPaths) {
			addedFiles.AddRange(AddFiles(assembly, project, targetPath, templatesPaths));
		}
		#endregion
	}
}
