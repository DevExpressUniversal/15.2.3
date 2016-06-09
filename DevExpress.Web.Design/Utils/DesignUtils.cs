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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml;
using DevExpress.Web.Internal;
using EnvDTE;
using Microsoft.Win32;
namespace DevExpress.Web.Design {
	public class ProjectItemState {
		public object PrevVisibleItem;
		public bool IsOpened;
	}
	delegate string PreprocessFunction(string input);
	delegate void ProcessFileBeforeAddToProject(string fileName, string webApplication);
	public delegate void CreateRibbonTabsFunction(bool clearExistingRibbonTabs);
	[CLSCompliant(false)]
	public class DesignUtils {
		static string ConvertToWebApplicationCmdGuid = "CB26E292-901A-419C-B79D-49BD45C43929";
		static int ConvertToWebApplicationID = 104;
		static float NormalFontHeight = 155; 
		static string WebApplicationExtenderName = "WebApplication";
		public static System.Windows.Forms.DialogResult ShowDialogOnInvokeTransactedChange(Form form, IComponent component) {
			System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.None;
			ISelectionService selectionService = (ISelectionService)component.Site.GetService(typeof(ISelectionService));
			if(selectionService.SelectionCount == 0 && component is IComponent)
				selectionService.SetSelectedComponents(new object[] { component });
			System.Web.UI.Design.ControlDesigner.InvokeTransactedChange(component, delegate(object arg) {
				result = ShowDialog(component.Site, form);
				return true;
			}, null, "Dialog Action");
			return result;
		}
		public static bool ShowDialog(IServiceProvider serviceProvider, System.Windows.Window window) {
			EnvDTE.DTE dte = serviceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
			if(dte == null) return false;
			var wih = new System.Windows.Interop.WindowInteropHelper(window);
			wih.Owner = new IntPtr(dte.MainWindow.HWnd);
			return window.ShowDialog() == true;
		}
		public static System.Windows.Forms.DialogResult ShowDialog(IServiceProvider serviceProvider, System.Windows.Forms.Form form) {
			System.Windows.Forms.Design.IUIService service =
				(System.Windows.Forms.Design.IUIService)serviceProvider.GetService(typeof(System.Windows.Forms.Design.IUIService));
			return (service != null) ? service.ShowDialog(form) : System.Windows.Forms.DialogResult.Cancel;
		}
		public static System.Windows.Forms.DialogResult ShowDialog(IServiceProvider serviceProvider, System.Windows.Forms.CommonDialog dialog) {
			System.Windows.Forms.Design.IUIService service =
				(System.Windows.Forms.Design.IUIService)serviceProvider.GetService(typeof(System.Windows.Forms.Design.IUIService));
			return (service != null) ? dialog.ShowDialog(service.GetDialogOwnerWindow()) : System.Windows.Forms.DialogResult.Cancel;
		}
		public static Size CorrectSize(Size size, float scaleFactor) {
			int newWidth = (int)(scaleFactor * ((float)size.Width));
			int newHeigth = (int)(scaleFactor * ((float)size.Height));
			return new Size(newWidth, newHeigth);
		}
		public static void CheckLargeFontSize(Control control) {
			float scaleFactor = DesignUtils.GetScaleFactor();
			if (scaleFactor != 1)
				control.Size = DesignUtils.CorrectSize(control.Size, scaleFactor);
		}
		public static Font GetDialogFont(IServiceProvider serviceProvider) {
			IUIService uiService = (IUIService)serviceProvider.GetService(typeof(IUIService));
			if (uiService != null) {
				IDictionary dictionary = uiService.Styles;
				if (dictionary != null) {
					return (Font)dictionary["DialogFont"];
				}
			}
			return null;
		}
		public static float GetScaleFactor() { 
			Label lblDummy = new System.Windows.Forms.Label();
			lblDummy.Font = new System.Drawing.Font("Tahoma", 96F);
			return ((float)lblDummy.Font.Height) / NormalFontHeight;
		}
		public static ToolStripRenderer GetToolStripRenderer(IServiceProvider serviceProvider) {
			IUIService uiService = (IUIService)serviceProvider.GetService(typeof(IUIService));
			if (uiService != null) {
				IDictionary dictionary = uiService.Styles;
				if (dictionary != null) {
					return (ToolStripRenderer)dictionary["VsRenderer"];
				}
			}
			return null;
		}
		public static DesignerActionItemCollection CreateDataSourceDesignerActions(ASPxDataWebControlDesignerBase controlDesigner, IComponent relatedComponent) {
			DesignerActionItemCollection collection = new DesignerActionItemCollection();
			DesignerActionItem item = CreateDesignerPropertyItem(controlDesigner, "DataSourceID", relatedComponent, StringResources.DataControl_ConfigureDataVerb);
			if (item != null)
				collection.Add(item);
			return collection;
		}
		public static DesignerActionPropertyItem CreateDesignerPropertyItem(ASPxDataWebControlDesignerBase controlDesigner, string memberName, IComponent relatedComponent, string configureVerbText) {
			DesignerActionPropertyItem result = null;
			PropertyDescriptor descriptor = TypeDescriptor.GetProperties(controlDesigner.Component)[memberName];
			if (descriptor != null && descriptor.IsBrowsable) {
				result = new DesignerActionPropertyItem(memberName, configureVerbText, StringResources.DataControl_DataActionGroup, StringResources.DataControl_ConfigureDataVerbDesc);
				if (relatedComponent != null)
					result.RelatedComponent = relatedComponent;
			}
			return result;
		}
		public static void ExecuteMenuCommand(Guid menuGroup, int commandID, IServiceProvider provider) {
			IMenuCommandService menuCommandService = provider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			menuCommandService.GlobalInvoke(new CommandID(menuGroup, commandID));
		}
		public static bool IsWebApplication(IServiceProvider provider) {
			EnvDTE.Project project = GetEnvDTEProject(provider);
			if (project.ExtenderNames is string[]) {
				List<string> extenderNames = new List<string>(project.ExtenderNames as string[]);
				return extenderNames.Contains(WebApplicationExtenderName);
			}
			return false;
		}
		public static void SelectItemInSolutionExplorer(string appRelativePath, IServiceProvider provider) {
			string pathEx = appRelativePath.Replace("/", "\\").Replace("~", "");
			EnvDTE.Project project = ((EnvDTE.ProjectItem)provider.GetService(typeof(EnvDTE.ProjectItem))).ContainingProject;
			IWebApplication webApplication = (IWebApplication)provider.GetService(typeof(IWebApplication));
			string slnName = Path.GetFileNameWithoutExtension(project.DTE.Solution.FullName);
			string fullItemPath = slnName + "\\" + webApplication.RootProjectItem.Name + pathEx;
			EnvDTE.Window solutionExplWindow = project.DTE.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer);
			solutionExplWindow.Activate();
			EnvDTE.UIHierarchy uiHierarchy = solutionExplWindow.Object as EnvDTE.UIHierarchy;
			EnvDTE.UIHierarchyItem uiHierarchyItem = null;
			uiHierarchyItem = GetProjectItem(uiHierarchy, fullItemPath);
			if (uiHierarchyItem == null)
				uiHierarchyItem = GetProjectItem(uiHierarchy, webApplication.RootProjectItem.Name + pathEx);
			if(uiHierarchyItem == null) {
				string vs2010fullItemPath = String.Format("{0}\\{1}{2}", slnName, project.Name, pathEx);
				uiHierarchyItem = GetProjectItem(uiHierarchy, vs2010fullItemPath);
			}
			if (uiHierarchyItem != null)
				uiHierarchyItem.Select(EnvDTE.vsUISelectionType.vsUISelectionTypeSelect);
		}
		public static EnvDTE.UIHierarchyItem GetProjectItem(EnvDTE.UIHierarchy uiHierarchy, string fullItemPath) {
			EnvDTE.UIHierarchyItem uiHierarchyItem = null;
			try {
				uiHierarchyItem = uiHierarchy.GetItem(fullItemPath);
			}
			catch(ArgumentException) {
				return null;
			}
			return uiHierarchyItem;
		}
		public static string GetProjectName(IServiceProvider provider) {
			EnvDTE.ProjectItem projectItem = ((EnvDTE.ProjectItem)provider.GetService(typeof(EnvDTE.ProjectItem)));
			return projectItem.ContainingProject.FullName;
		}
		public static string GetProjectLanguage(ProjectItem projectItem) {
			if (projectItem == null)
				return CodeModelLanguageConstants.vsCMLanguageCSharp;
			if (projectItem.ContainingProject.CodeModel != null)
				return projectItem.ContainingProject.CodeModel.Language;
			if (projectItem.FileCodeModel != null)
				return projectItem.FileCodeModel.Language;
			return CodeModelLanguageConstants.vsCMLanguageCSharp;
		}
		public static string[] CopyUserControlsToWebSite(string[] userControlNames,
			string dialogFormsResourcePathPrefix, string dialogFormsAppRelativeDirectoryPath,
			Type type, IServiceProvider provider) {
			return CopyUserControlsToWebSite(userControlNames, dialogFormsResourcePathPrefix,
				dialogFormsAppRelativeDirectoryPath, type, provider, false);
		}
		public static string[] CopyUserControlsToWebSite(string[] userControlNames,
			string dialogFormsResourcePathPrefix, string dialogFormsAppRelativeDirectoryPath,
			Type type, IServiceProvider provider, bool copyDesignerFileFromResource) {
			List<string> newFiles = new List<string>();
			for (int i = 0; i < userControlNames.Length; i++)
				CopyUserControlToWebSite(userControlNames[i], dialogFormsResourcePathPrefix,
										 dialogFormsAppRelativeDirectoryPath, type, copyDesignerFileFromResource,
										 provider, newFiles);
			if (!copyDesignerFileFromResource && IsWebApplication(provider) && newFiles.Count > 0)
				ConvertUserControlsToWebApplication(dialogFormsAppRelativeDirectoryPath, provider);
			return newFiles.ToArray();
		}
		public static void CopyUserControlToWebSite(string userControlName, string dialogFormsResourcePathPrefix,
			string dialogFormsAppRelativeDirectoryPath, Type type,
			bool copyDesignerFileFromResource, IServiceProvider provider, List<string> newFiles) {
			IWebApplication webApplication = (IWebApplication)provider.GetService(typeof(IWebApplication));
			string ascxFileName = userControlName + RenderUtils.DefaultUserControlFileExtension;
			string ascxCodeBehindFileName = userControlName + GetCodeBehindFileExtension(provider);
			string ascxDesignerFileName = userControlName + GetDesginerFileExtension(provider);
			string ascxFileResourceName = dialogFormsResourcePathPrefix + ascxFileName;
			string ascxCodeBehindResourceName = dialogFormsResourcePathPrefix + ascxCodeBehindFileName;
			string ascxDesignerFileResourceName = dialogFormsResourcePathPrefix +
				RenderUtils.DefaultUserControlDesignersRelativeNamespace +
				ascxDesignerFileName;
			if (IsNeedCopyDefaultDialogFile(userControlName, ascxFileResourceName, ascxCodeBehindResourceName, type, webApplication, provider)) {
				bool isNeedCopyDesignerFileFromResource = IsWebApplication(provider) && copyDesignerFileFromResource;
				newFiles.Add(dialogFormsAppRelativeDirectoryPath + ascxFileName);
				newFiles.Add(dialogFormsAppRelativeDirectoryPath + ascxCodeBehindFileName);
				List<ProcessFileBeforeAddToProject> beforeAddToProjectFunctions = new List<ProcessFileBeforeAddToProject>();
				beforeAddToProjectFunctions.Add(PatchDxControlVersionAndToken);
				if (isNeedCopyDesignerFileFromResource)
					beforeAddToProjectFunctions.Add(ConvertUserControlToWebAppFormat);
				ProjectItem ascxProjectItem = CopyFileFromResourceToWebSite(
					null, ascxFileResourceName, ascxFileName, dialogFormsAppRelativeDirectoryPath,
					provider, type.Assembly, beforeAddToProjectFunctions.ToArray()
				);
				CopyFileFromResourceToWebSite(
					ascxProjectItem, ascxCodeBehindResourceName, ascxCodeBehindFileName,
					dialogFormsAppRelativeDirectoryPath, provider, type.Assembly,
					new ProcessFileBeforeAddToProject[0] { }
				);
				if (isNeedCopyDesignerFileFromResource) {
					CopyFileFromResourceToWebSite(
						ascxProjectItem, ascxDesignerFileResourceName, ascxDesignerFileName,
						dialogFormsAppRelativeDirectoryPath, provider, type.Assembly,
						new ProcessFileBeforeAddToProject[1] { ConvertUserControlDesignerToWebAppFormat }
					);
				}
			}
		}
		protected static string GetWebApplicationRootNamespace(IServiceProvider provider) {
			EnvDTE.Project project = GetEnvDTEProject(provider);
			Property prop = project.Properties.Item("RootNamespace");
			return prop != null ? prop.Value.ToString() : "";
		}
		public static string GetCodeBehindFileExtension(IServiceProvider provider) {
			EnvDTE.ProjectItem projectItem = ((EnvDTE.ProjectItem)provider.GetService(typeof(EnvDTE.ProjectItem)));
			return SelectCodeBehindFileExtension(GetProjectLanguage(projectItem));
		}
		public static string GetDesginerFileExtension(IServiceProvider provider) {
			EnvDTE.ProjectItem projectItem = ((EnvDTE.ProjectItem)provider.GetService(typeof(EnvDTE.ProjectItem)));
			return SelectDesignerFileExtension(GetProjectLanguage(projectItem));
		}
		public static bool IsNeedCopyDefaultDialogFile(string userControlName, string dialogFormsResourcePathPrefix,
			string dialogFormsAppRelativeDirectoryPath, Type type, IServiceProvider provider) {
			EnvDTE.ProjectItem projectItem = ((EnvDTE.ProjectItem)provider.GetService(typeof(EnvDTE.ProjectItem)));
			EnvDTE.Project project = projectItem.ContainingProject;
			IWebApplication webApplication = (IWebApplication)provider.GetService(typeof(IWebApplication));
			string ascxFileName = userControlName + RenderUtils.DefaultUserControlFileExtension;
			string ascxCodeBehindFileName = userControlName + GetCodeBehindFileExtension(provider);
			string ascxDesignerFileName = userControlName + GetDesginerFileExtension(provider);
			string ascxFileResourceName = dialogFormsResourcePathPrefix + ascxFileName;
			string ascxCodeBehindResourceName = dialogFormsResourcePathPrefix + ascxCodeBehindFileName;
			return IsNeedCopyDefaultDialogFile(userControlName, ascxFileResourceName, ascxCodeBehindResourceName,
											type, webApplication, provider);
		}
		private static string SelectCodeBehindFileExtension(string language) {
			if (language == CodeModelLanguageConstants.vsCMLanguageCSharp)
				return RenderUtils.CSDefaultUserControlCodeBehindFileExtension;
			else
				return RenderUtils.VBDefaultUserControlCodeBehindFileExtension;
		}
		private static string SelectDesignerFileExtension(string language) {
			if (language == CodeModelLanguageConstants.vsCMLanguageCSharp)
				return RenderUtils.CSDefaultUserControlDesignerFileExtension;
			else
				return RenderUtils.VBDefaultUserControlDesignerFileExtension;
		}
		public static string MapPath(IServiceProvider serviceProvider, string virtualPath) {
			if(UrlUtils.IsAppRelativePath(virtualPath)) {
				if(serviceProvider != null) {
					IWebApplication webApplication = (IWebApplication)serviceProvider.GetService(typeof(IWebApplication));
					if(webApplication != null) {
						IProjectItem projectItem = webApplication.GetProjectItemFromUrl(virtualPath);
						if(projectItem != null)
							return projectItem.PhysicalPath;
						else {
							string relativePath = Path.GetDirectoryName(virtualPath).Replace("~", "");
							string res = webApplication.RootProjectItem.PhysicalPath + relativePath +
								Path.DirectorySeparatorChar + Path.GetFileName(virtualPath);
							return res.Replace("\\\\", "\\");
						}
					}
				}
			}
			return virtualPath;
		}
		internal static ProjectItem CopyFileFromResourceToWebSite(ProjectItem parentItem, string resourceName,
			string fileNameWithExtension, string dialogFormsAppRelativeDirectoryPath, IServiceProvider provider,
			Assembly assembly, ProcessFileBeforeAddToProject[] funcs) {
			Project project = GetEnvDTEProject(provider);
			IWebApplication webApplication = (IWebApplication)provider.GetService(typeof(IWebApplication));
			ProjectItem rootProjectItem = (ProjectItem)provider.GetService(typeof(ProjectItem));
			string relativeFileNameInWebSite = dialogFormsAppRelativeDirectoryPath + fileNameWithExtension;
			string physicalFileNameInWebSite = MapPath(provider, relativeFileNameInWebSite);
			if (parentItem == null || !IsWebApplication(provider)) 
				parentItem = (ProjectItem)EnvDTEHelper.CreateDirectoryProjectItemByPath(relativeFileNameInWebSite, project, webApplication);
			if (parentItem == null) 
				return null;
			ProjectItems projectItems = parentItem != null ? parentItem.ProjectItems : project.ProjectItems;
			ProjectItem projectItem = null;
			if(EnvDTEHelper.IsExistProjectItem(relativeFileNameInWebSite, webApplication)) {
				DTE dte = provider.GetService(typeof(DTE)) as DTE;
				bool isUnderSCC = dte.SourceControl.IsItemUnderSCC(physicalFileNameInWebSite);
				bool isChekedOut = dte.SourceControl.IsItemCheckedOut(physicalFileNameInWebSite);
				if(isUnderSCC && !isChekedOut)
					isChekedOut = dte.SourceControl.CheckOutItem(physicalFileNameInWebSite);
				if(!isUnderSCC && (File.GetAttributes(physicalFileNameInWebSite) == FileAttributes.ReadOnly))
					File.SetAttributes(physicalFileNameInWebSite, FileAttributes.Normal);
				if(isChekedOut || !isUnderSCC) {
					string fileText = FileUtils.GetResourceFileText(assembly, resourceName);
					FileUtils.SetFileText(physicalFileNameInWebSite, fileText);
					projectItem = (ProjectItem)EnvDTEHelper.GetProjectItem(fileNameWithExtension, projectItems);
				}
			}
			else {
				FileUtils.CopyFileFromResourceToFile(assembly, resourceName, physicalFileNameInWebSite);
				projectItem = projectItems.AddFromFileCopy(physicalFileNameInWebSite);
			}
			if (funcs.Length > 0) {
				bool isVB = GetProjectLanguage(rootProjectItem) == CodeModelLanguageConstants.vsCMLanguageVB;
				for (int i = 0; i < funcs.Length; i++)
					funcs[i](physicalFileNameInWebSite, IsWebApplication(provider) && isVB ? GetWebApplicationRootNamespace(provider) : "");
			}
			return projectItem;
		}
		public static void ConvertUserControlsToWebApplication(string appFolderPath, IServiceProvider provider) {
			EnvDTE.Project project = ((EnvDTE.ProjectItem)provider.GetService(typeof(EnvDTE.ProjectItem))).ContainingProject;
			IWebApplication webApplication = (IWebApplication)provider.GetService(typeof(IWebApplication));
			Object directoryObject = EnvDTEHelper.CreateDirectoryProjectItemByPath(appFolderPath,
				project, webApplication);
			if (directoryObject != null) {
				SelectItemInSolutionExplorer(appFolderPath, provider);
				ExecuteMenuCommand(new Guid(ConvertToWebApplicationCmdGuid), ConvertToWebApplicationID, provider);
			}
		}
		public static void IncludeFoldersInProject(ArrayList appFolderPaths, IServiceProvider provider) {
			IncludeFoldersInProject(appFolderPaths, new string[] { }, provider);
		}
		public static void IncludeFoldersInProject(ArrayList appFolderPaths,
			string[] excludedFileExtensions, IServiceProvider provider) {
			List<string> excludedFileExtensionList = new List<string>(excludedFileExtensions);
			foreach (string appFolderPath in appFolderPaths)
				IncludeFolderInProject(new DirectoryInfo(appFolderPath), provider, excludedFileExtensionList);
		}
		public static void IncludeFileInProject(string appFilePath, IServiceProvider provider) {
			EnvDTE.Project project = ((EnvDTE.ProjectItem)provider.GetService(typeof(EnvDTE.ProjectItem))).ContainingProject;
			project.ProjectItems.AddFromFile(appFilePath);
		}
		private static void IncludeFolderInProject(DirectoryInfo source, IServiceProvider provider,
			List<string> excludedFileExtensionList) {
			foreach (FileInfo fi in source.GetFiles()) {
				if (!excludedFileExtensionList.Contains(Path.GetExtension(fi.FullName)))
					IncludeFileInProject(fi.FullName, provider);
			}
			foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
				IncludeFolderInProject(diSourceSubDir, provider, excludedFileExtensionList);
		}
		private static bool IsNeedCopyDefaultDialogFile(string formName, string formResourceName, string formCodeBehindResourceName,
														Type type, IWebApplication webApplication,
														IServiceProvider provider) {
			string defaultFormUrl = CommonUtils.GetDefaultFormUrl(formName, type);
			string defaultFormCodeBehindUrl = CommonUtils.GetDefaultFormCodeBehindUrl(formName, GetCodeBehindFileExtension(provider), type);
			return !EnvDTEHelper.IsExistProjectItem(defaultFormUrl, webApplication) ||
				   !EnvDTEHelper.IsExistProjectItem(defaultFormCodeBehindUrl, webApplication) ||
				   !AreEmbeddedResourceAndPhysicalFileEqual(type.Assembly, formResourceName,
													EnvDTEHelper.GetPhysicalPathByUrl(defaultFormUrl,
																					  webApplication), RemoveDeclarationTags) ||
				   !AreEmbeddedResourceAndPhysicalFileEqual(type.Assembly, formCodeBehindResourceName,
													EnvDTEHelper.GetPhysicalPathByUrl(defaultFormCodeBehindUrl,
																					  webApplication), delegate(string input) { return input; });
		}
		private static bool AreEmbeddedResourceAndPhysicalFileEqual(Assembly resourceContainingAssembly,
			string resourceName, string physicalFilePath, PreprocessFunction processContent) {
			string resourceFileText = FileUtils.GetResourceFileText(resourceContainingAssembly, resourceName);
			string physicalFileText = FileUtils.GetFileText(physicalFilePath);
			string processedResourceFileText = ProcessFileContentBeforeComparison(resourceFileText, processContent);
			string processedPhysicalFileText = ProcessFileContentBeforeComparison(physicalFileText, processContent);
			return string.Equals(processedResourceFileText, processedPhysicalFileText);
		}
		private static string ProcessFileContentBeforeComparison(string fileContent, PreprocessFunction processContent) {
			if (fileContent != null) {
				fileContent = processContent(fileContent);
				if (fileContent != null)
					fileContent = fileContent.Trim(); 
			}
			return fileContent;
		}
		private static string RemoveDeclarationTags(string text) {
			text = Regex.Replace(text, RegExConst.RegExInheritsASPX, "", RegexOptions.IgnoreCase);
			text = Regex.Replace(text, RegExConst.RegExCodeBehindASPX, "", RegexOptions.IgnoreCase);
			text = Regex.Replace(text, RegExConst.RegExRegisterASPX, "", RegexOptions.IgnoreCase);
			return Regex.Replace(text, RegExConst.RegExAssemblyASPX, "", RegexOptions.IgnoreCase);
		}
		public static void PatchDxControlVersionAndToken(string fileName, string webApp) {
			string fileText = FileUtils.GetFileText(fileName);
			fileText = AspxCodeUtils.GetPatchedDxControlVersionAndTokenInContent(fileText);
			FileUtils.SetFileText(fileName, fileText);
		}
		private static void ConvertUserControlToWebAppFormat(string fileName, string webApplicationName) {
			string fileText = FileUtils.GetFileText(fileName);
			Regex codeFileRegEx = new Regex(RegExConst.RegExCodeFileASPX);
			MatchCollection matchCollection = codeFileRegEx.Matches(fileText);
			if (matchCollection.Count > 0)
				fileText = fileText.Replace(matchCollection[0].Value, matchCollection[0].Value.Replace("CodeFile=", "Codebehind="));
			if (!string.IsNullOrEmpty(webApplicationName)) {
				Regex inheritsRegEx = new Regex(RegExConst.RegExInheritsASPX,
					RegexOptions.Multiline | RegexOptions.IgnoreCase);
				if (inheritsRegEx.IsMatch(fileText))
					fileText = inheritsRegEx.Replace(fileText, "$1$2" + webApplicationName + ".$3");
			}
			FileUtils.SetFileText(fileName, fileText);
		}
		private static void ConvertUserControlDesignerToWebAppFormat(string fileName, string webApplicationName) {
			string fileText = FileUtils.GetFileText(fileName);
			FileUtils.SetFileText(fileName, fileText.Replace("{0}", webApplicationName));
		}
		private static EnvDTE.Project GetEnvDTEProject(IServiceProvider provider) {
			return ((EnvDTE.ProjectItem)provider.GetService(typeof(EnvDTE.ProjectItem))).ContainingProject;
		}
		public static string GetCurrentProjectPath(IServiceProvider provider) {
			Project project = DesignUtils.GetEnvDTEProject(provider);
			if(project != null) {
				Property property = project.Properties.Item("FullPath");
				if(property != null) {
					object projectPath = property.Value;
					if(projectPath != null && !string.IsNullOrEmpty(projectPath.ToString()))
						return projectPath.ToString();
				}
			}
			return string.Empty;
		}
		public static IDesignerHost GetDesignerHost(ITypeDescriptorContext context) {
			return (IDesignerHost)context.GetService(typeof(IDesignerHost));
		}
		public static string GetDataSourceID(object dataSourceOwner) {
			string dataSourceID = string.Empty;
			PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(dataSourceOwner)["DataSourceID"];
			if (propDescriptor != null)
				dataSourceID = propDescriptor.GetValue(dataSourceOwner) as string;
			return dataSourceID;
		}
		public static IComponent GetDataSource(ITypeDescriptorContext context, string dataSourceID) {
			IDesignerHost designerHost = GetDesignerHost(context);
			if (designerHost != null && designerHost.RootComponent != null && designerHost.Container != null)
				return designerHost.Container.Components[dataSourceID];
			return null;
		}
		public static IDataSourceDesigner GetDataSourceDesigner(ITypeDescriptorContext context, IComponent dataSource) {
			IDataSourceDesigner dataSourceDesigner = null;
			if (dataSource != null) {
				IDesignerHost designerHost = GetDesignerHost(context);
				dataSourceDesigner = designerHost.GetDesigner(dataSource) as IDataSourceDesigner;
			}
			return dataSourceDesigner;
		}
		public static DesignerDataSourceView GetDesignerDataSourceView(ITypeDescriptorContext context, IComponent dataSource) {
			DesignerDataSourceView view = null;
			IDataSourceDesigner dataSourceDesigner = GetDataSourceDesigner(context, dataSource);
			if (dataSourceDesigner != null)
				view = dataSourceDesigner.GetView(null);
			return view;
		}
		public static DesignerDataSourceView GetDesignerDataSourceView(ITypeDescriptorContext context, string dataSourceID) {
			IComponent dataSource = GetDataSource(context, dataSourceID);
			return GetDesignerDataSourceView(context, dataSource);
		}
		public static void ShowHelpFromUrl(string url, IHelpService helpService) {
			if(helpService == null || url.StartsWith("http://"))
				ExecuteProcess(url, string.Empty);
			else
				helpService.ShowHelpFromUrl(url);
		}
		public static void ExecuteProcess(string name, string arguments) {
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = name;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.Verb = "Open";
			process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
			process.Start();
		}
		public static string GetHash(string input) {
			MD5 md5Hasher = MD5.Create();
			byte[] inputBytes = Encoding.UTF8.GetBytes(input);
			return Convert.ToBase64String(md5Hasher.ComputeHash(inputBytes));
		}
		public static DialogResult ShowMessage(IServiceProvider serviceProvider, string message, string caption, MessageBoxButtons buttons) {
			if(serviceProvider != null) {
				IUIService service = (IUIService)serviceProvider.GetService(typeof(IUIService));
				if(service != null)
					return service.ShowMessage(message, caption, buttons);
			}
			return MessageBox.Show(message, caption, buttons);
		}
		public static void ShowError(IServiceProvider serviceProvider, string message) {
			if(serviceProvider != null) {
				IUIService service = (IUIService)serviceProvider.GetService(typeof(IUIService));
				if(service != null)
					service.ShowError(message);
			}
			else
				MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
	public class EnvDTEHelper {
		public static object CreateDirectoryProjectItemByPath(string path, object curProjectObj,
			IWebApplication webApplication) {
			EnvDTE.Project curProject = curProjectObj as EnvDTE.Project;
			string[] dirs = GetDirectories(path);
			string curPath = "~/";
			EnvDTE.ProjectItem ret = null;
			foreach (string dir in dirs) {
				curPath += dir + "/";
				if (IsExistProjectItem(curPath, webApplication)) {
					ret = GetProjectItem(dir, ret != null ? ret.ProjectItems : curProject.ProjectItems) as EnvDTE.ProjectItem;
					if (ret == null) {
						curProject.ProjectItems.AddFolder(dir, "");
					}
				} else
					ret = ret == null ? curProject.ProjectItems.AddFolder(dir, "") :
						ret.ProjectItems.AddFolder(dir, "");
			}
			return ret;
		}
		public static string GetPhysicalPathByUrl(string url, IWebApplication webApplication) {
			return webApplication.GetProjectItemFromUrl(url).PhysicalPath;
		}
		public static ProjectItemState GetProjectItemState(object projectItemObj) {
			EnvDTE.ProjectItem item = projectItemObj as EnvDTE.ProjectItem;
			ProjectItemState state = new ProjectItemState();
			state.IsOpened = item.get_IsOpen(Constants.vsViewKindPrimary);
			state.PrevVisibleItem = item.DTE.ActiveDocument != null ? item.DTE.ActiveDocument.ProjectItem : null;
			return state;
		}
		public static void SetStateForProjectItem(object projectItemObj, ProjectItemState state) {
			EnvDTE.ProjectItem item = projectItemObj as EnvDTE.ProjectItem;
			if(state.IsOpened && state.PrevVisibleItem != null)
				(state.PrevVisibleItem as ProjectItem).Open(Constants.vsViewKindDesigner).Activate();
			else
				item.DTE.ActiveDocument.Close(vsSaveChanges.vsSaveChangesNo);
		}
		public static object GetProjectItem(string name, object projectItemsObj) {
			EnvDTE.ProjectItems projectItems = (EnvDTE.ProjectItems)projectItemsObj;
			foreach (EnvDTE.ProjectItem curItem in projectItems)
				if (curItem.Name == name)
					return curItem;
			return null;
		}
		public static object GetProjectItemByPath(string appRelativePath, IServiceProvider provider) {
			EnvDTE.ProjectItem ret = null;
			DTE dte = (DTE)provider.GetService(typeof(DTE));
			EnvDTE.ProjectItem curProjectItem = (EnvDTE.ProjectItem)provider.GetService(typeof(EnvDTE.ProjectItem));
			IWebApplication webApplication = (IWebApplication)provider.GetService(typeof(IWebApplication));
			IProjectItem iProjectItem = webApplication.GetProjectItemFromUrl(appRelativePath);
			if (iProjectItem != null)
				ret = dte.Solution.FindProjectItem(iProjectItem.PhysicalPath);
			return ret;
		}
		public static bool IsExistProjectItem(string url, IWebApplication webApplication) {
			return webApplication.GetProjectItemFromUrl(url) != null;
		}
		public static void InsertTextToDocument(object textDocumentObj, string insertingText) {
			EnvDTE.TextDocument textDocument = textDocumentObj as EnvDTE.TextDocument;
			EnvDTE.EditPoint startPoint = textDocument.StartPoint.CreateEditPoint();
			textDocument.Selection.SelectAll();
			textDocument.Selection.Delete(1);
			startPoint.Insert(insertingText);
			startPoint.StartOfDocument();
		}
		private static string[] GetDirectories(string path) {
			List<string> dirs = new List<string>();
			string curPath = Path.GetDirectoryName(path);
			while (Path.GetDirectoryName(curPath) != "") {
				string curDir = curPath.Replace(Path.GetDirectoryName(curPath), "");
				dirs.Add(curDir.Replace("\\", ""));
				curPath = Path.GetDirectoryName(curPath);
			}
			dirs.Reverse();
			return dirs.ToArray();
		}
	}
	public class PropertyStore {
		private string fPath;
		public string Path {
			get { return fPath; }
		}
		public RegistryKey RootKey {
			get { return Registry.CurrentUser; }
		}
		public PropertyStore(string path) {
			fPath = path;
		}
		public string LoadString(string propertyName, string defaultValue) {
			RegistryKey key = RootKey.OpenSubKey(Path + "\\" + propertyName);
			return (key != null) ? (string)key.GetValue(propertyName) : defaultValue;
		}
		public void SaveString(string propertyName, string propertyValue) {
			string objectPath = Path + "\\" + propertyName;
			RegistryKey key = RootKey.OpenSubKey(objectPath, true);
			if (key == null)
				key = RootKey.CreateSubKey(objectPath);
			if (key != null) {
				key.SetValue(propertyName, propertyValue);
				key.Close();
			}
		}
		public Rectangle LoadRectangle(string objectName) {
			RegistryKey key = RootKey.OpenSubKey(Path + "\\" + objectName);
			Rectangle ret = new Rectangle();
			if (key != null) {
				ret.X = (int)key.GetValue("X");
				ret.Y = (int)key.GetValue("Y");
				ret.Height = (int)key.GetValue("Height");
				ret.Width = (int)key.GetValue("Width");
			}
			return ret;
		}
		public void SaveRectangle(string objectName, Rectangle rectangle) {
			string objectPath = Path + "\\" + objectName;
			RegistryKey key = RootKey.OpenSubKey(objectPath, true);
			if (key == null)
				key = RootKey.CreateSubKey(objectPath);
			if (key != null) {
				key.SetValue("X", rectangle.X);
				key.SetValue("Y", rectangle.Y);
				key.SetValue("Height", rectangle.Height);
				key.SetValue("Width", rectangle.Width);
				key.Close();
			}
		}
		public Color LoadColor(string objectName, Color defaultValue) {
			RegistryKey key = RootKey.OpenSubKey(Path + "\\" + objectName);
			Color color = defaultValue;
			if (key != null) {
				color = Color.FromArgb(byte.Parse((string)key.GetValue("A")), byte.Parse((string)key.GetValue("R")),
					byte.Parse((string)key.GetValue("G")), byte.Parse((string)key.GetValue("B")));
			}
			return color;
		}
		public void SaveColor(string objectName, Color color) {
			string objectPath = Path + "\\" + objectName;
			RegistryKey key = RootKey.OpenSubKey(objectPath, true);
			if (key == null)
				key = RootKey.CreateSubKey(objectPath);
			if (key != null) {
				key.SetValue("A", color.A);
				key.SetValue("R", color.R);
				key.SetValue("G", color.G);
				key.SetValue("B", color.B);
				key.Close();
			}
		}
		public int LoadInt(string objectName, int defaultValue) {
			RegistryKey key = RootKey.OpenSubKey(Path + "\\" + objectName);
			return (key != null) ? (int)key.GetValue("PX") : defaultValue;
		}
		public void SaveInt(string objectName, int pos) {
			string objectPath = Path + "\\" + objectName;
			RegistryKey key = RootKey.OpenSubKey(objectPath, true);
			if (key == null)
				key = RootKey.CreateSubKey(objectPath);
			if (key != null) {
				key.SetValue("PX", pos);
				key.Close();
			}
		}
	}
	public delegate XmlNode ProcessWebConfigXml(XmlDocument doc, string[] sectionNames, NameValueCollection parameters);
	public static class TagPrefixHelper {
		static object GetRegisterDirectiveManagerInstance(WebFormsReferenceManager refManager) {
			PropertyInfo pi = refManager.GetType().GetProperty("RegisterDirectiveService", BindingFlags.Instance | BindingFlags.NonPublic);
			if (pi != null)
				return pi.GetValue(refManager, new object[] { });
			pi = refManager.GetType().GetProperty("TheRegisterDirectiveManager", BindingFlags.Instance | BindingFlags.NonPublic);
			if (pi != null)
				return pi.GetValue(refManager, new object[] { });
			FieldInfo fi = refManager.GetType().GetField("_rgm", BindingFlags.Instance | BindingFlags.NonPublic);
			if (fi != null)
				return fi.GetValue(refManager);
			return null;
		}
		static void RegisterTagPrefix(object registerDirectiveManager, Type type) {
			MethodInfo mi = registerDirectiveManager.GetType().GetMethod("EnsureCustomControlRegisterDirective");
			if (mi == null)
				mi = registerDirectiveManager.GetType().Assembly.GetType("Microsoft.VisualStudio.Web.WebForms.IRegisterDirectiveService").GetMethod("EnsureCustomControlRegisterDirective");
			if (mi != null)
				mi.Invoke(registerDirectiveManager, new object[] { type });
		}
		public static void RegisterTagPrefix(WebFormsRootDesigner rootDesigner, Type type) {
			if (rootDesigner != null && rootDesigner.GetType().Name != "DummyRootDesigner") {
				var dte = (DTE)rootDesigner.Component.Site.GetService(typeof(DTE));
				if(dte.ActiveWindow.Object is HTMLWindow)
					return;
				WebFormsReferenceManager refManager = rootDesigner.ReferenceManager;
				object registerDirectiveManager = GetRegisterDirectiveManagerInstance(refManager);
				if (registerDirectiveManager != null)
					RegisterTagPrefix(registerDirectiveManager, type);
			}
		}
	}
}
