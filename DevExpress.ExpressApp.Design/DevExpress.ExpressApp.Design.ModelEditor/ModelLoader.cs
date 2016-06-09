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
using System.IO;
using System.ComponentModel.Design;
using System.Collections;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.ExpressApp.Core;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Design.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System.Configuration;
using System.Windows.Forms.Design;
using System.Windows.Forms;
namespace DevExpress.ExpressApp.Design.ModelEditor {
	public class ModelLoader {
		public const string UnusableNodesFoundMessage = "Your model differences were found to contain nodes that are no longer usable, possibly due to a version upgrade of XAF or an incompatible module list. These unusable nodes have been saved in files called UnusableNodes*.xafml in your user model data location.";
		private ProjectWrapper projectWrapper;
		private ITypesInfo typesInfo;
		private IServiceProvider serviceProvider;
		private XafApplication GetXafApplicationByDesigner(Project project) {
			ProjectItem applicationProjectItem = FindXafEntityProjectItem(project.ProjectItems, "Application");
			if(applicationProjectItem == null) {
				throw new ArgumentException("Cannot find Application item in '" + project.FileName + "' project");
			}
			return GetComponenFromDesigner<XafApplication>(applicationProjectItem, typesInfo);
		}
		private string GetConfigFileNameByDesigner(Project project) {
			ProjectItem applicationProjectItem = FindXafEntityProjectItem(project.ProjectItems, null, ".config");
			if(applicationProjectItem == null) {
				throw new ArgumentException("Cannot find configuration item in '" + project.FileName + "' project");
			}
			return applicationProjectItem.Properties.Item("FullPath").Value.ToString();
		}
		private ModuleBase GetXafModuleByDesigner(Project project) {
			ProjectItem applicationProjectItem = FindXafEntityProjectItem(project.ProjectItems, "Module");
			if(applicationProjectItem == null) {
				throw new ArgumentException("Cannot find Module item in '" + project.FileName + "' project");
			}
			return GetComponenFromDesigner<ModuleBase>(applicationProjectItem, typesInfo);
		}
		private static ComponentType GetComponenFromDesigner<ComponentType>(ProjectItem projectItem, ITypesInfo typesInfo) {
			if(projectItem == null) {
				throw new ArgumentNullException("projectItem");
			}
			string fileName = projectItem.get_FileNames(0);
			EnvDTE.Window window = null;
			IDesignerHost designerHost = null;
			if(!projectItem.get_IsOpen(EnvDTE.Constants.vsViewKindDesigner)) {
				window = projectItem.Open(EnvDTE.Constants.vsViewKindDesigner);
				window.SetFocus();
				designerHost = (IDesignerHost)window.Object;
			}
			else {
				foreach(Document document in projectItem.DTE.Documents) {
					if(document.ProjectItem == projectItem) {
						foreach(EnvDTE.Window currentWindow in document.Windows) {
							designerHost = currentWindow.Object as IDesignerHost;
							if(designerHost != null) {
								break;
							}
						}
					}
				}
			}
			if(designerHost == null) {
				throw new ArgumentNullException("designerHost");
			}
			if(designerHost.RootComponent == null) {
				throw new ArgumentNullException("designerHost.RootComponent");
			}
			Type rootType = designerHost.GetType(designerHost.RootComponentClassName);
			if(rootType == null) {
				throw new ArgumentException("The '" + designerHost.RootComponentClassName + "' type was not found. Be sure the project was built.");
			}
			ITypeInfo componentTypeInfo = typesInfo.FindTypeInfo(typeof(ComponentType));
			ITypeInfo rootTypeInfo = typesInfo.FindTypeInfo(rootType);
			if(!ReflectionHelper.IsTypeAssignableFrom(componentTypeInfo, rootTypeInfo)) {
				ReflectionHelper.ThrowInvalidCastException(typeof(ComponentType), rootType);
			}
			XafTypesInfo.Reset();
			return (ComponentType)Activator.CreateInstance(rootType);
		}
		private ProjectItem FindXafEntityProjectItem(ProjectItems projectItems, string entityName) {
			return FindXafEntityProjectItem(projectItems, entityName, null);
		}
		private ProjectItem FindXafEntityProjectItem(ProjectItems projectItems, string entityName, string extention) {
			if(projectItems == null) {
				return null;
			}
			foreach(ProjectItem projectItem in projectItems) {
				string fileName = projectItem.get_FileNames(0);
				string fileNameWithoutExtention = Path.GetFileNameWithoutExtension(fileName);
				if(extention != null && Path.GetExtension(fileName) != extention) {
					continue;
				}
				try {
					Property subTypeProperty = projectItem.Properties.Item("SubType");
					if(projectItem.FileCodeModel != null && subTypeProperty != null && (string)subTypeProperty.Value == "Component") {
						if((entityName == null || fileNameWithoutExtention.Contains(entityName)) && !fileName.Contains("Designer")) {
							return projectItem;
						}
					}
				}
				catch { }
				ProjectItem nestedItem = FindXafEntityProjectItem(projectItem.ProjectItems, entityName);
				if(nestedItem != null) {
					return nestedItem;
				}
			}
			return null;
		}
		private Type FindDiscoveredType(ITypeDiscoveryService typeDiscoveryService, Type baseType, string assemblyName) {
			Guard.ArgumentNotNull(typeDiscoveryService, "typeDiscoveryService");
			ICollection discoveredTypes = typeDiscoveryService.GetTypes(baseType, true);
			return GetLeafType(discoveredTypes, assemblyName, baseType);
		}
		private Type GetLeafType(ICollection types, string assemblyName, Type baseType) {
			List<Type> items = new List<Type>();
			foreach(object item in types) {
				if(item is Type) {
					items.Add((Type)item);
				}
			}
			DesignerModelFactory dmf = new DesignerModelFactory();
			return dmf.GetLeafType(items, assemblyName, baseType);
		}
		private bool CheckLoadedAssembliesConflict(ITypeResolutionService typeResolutionService, XafApplication application) {
			foreach(ModuleBase module in application.Modules) {
				if(typeResolutionService.GetType(module.GetType().FullName) != module.GetType()) {
					return true;
				}
			}
			return false;
		}
		private static bool IsApplicationProject(ProjectWrapper projectWrapper) {
			return projectWrapper is WebApplicationWrapper || projectWrapper is WinApplicationWrapper || projectWrapper is WebApplicationProjectWrapper;
		}
		class DesignTimeFileModelStore : FileModelStore, IModelEditorDisposable {
			IServiceProvider serviceProvider;
			ProjectWrapper projectWrapper;
			List<string> diffsFilePaths = new List<string>();
			public DesignTimeFileModelStore(string storePath, string fileNameTemplate, IServiceProvider serviceProvider, ProjectWrapper projectWrapper)
				: base(storePath, fileNameTemplate) {
				this.serviceProvider = serviceProvider;
				this.projectWrapper = projectWrapper;
				diffsFilePaths.Add(Path.Combine(storePath, GetFileNameForAspect(null, fileNameTemplate)));
				foreach(string aspect in GetAspects()) {
					diffsFilePaths.Add(Path.Combine(storePath, GetFileNameForAspect(aspect, fileNameTemplate)));
				}
			}
			public override bool CheckFileExist(string fileName) {
				foreach(ProjectItem projectItem in projectWrapper.GetAllProjectItems()) {
					string diffFileName = projectItem.get_FileNames(0);
					if(diffFileName == fileName) {
						return true;
					}
				}
				return false;
			}
			public override bool ReadOnly {
				get {
					return !ProjectWrapper.CanEditFile(diffsFilePaths.ToArray(), serviceProvider);
				}
			}
			void IModelEditorDisposable.Dispose() { 
				projectWrapper = null;
				serviceProvider = null;
				diffsFilePaths = null;
			}
		}
		private List<ModuleDiffStoreInfo> PatchModulesDiffsStore(ProjectWrapper currentProjectWrapper, IEnumerable<ModuleBase> modules) {
			List<ModuleDiffStoreInfo> result = new List<ModuleDiffStoreInfo>();
			foreach(ModuleBase moduleBase in modules) {
				System.Reflection.Assembly moduleAssembly = moduleBase.GetType().Assembly;
				string assemblyName = moduleAssembly.GetName().Name;
				if(currentProjectWrapper.AssemblyName != assemblyName) {
					SolutionWrapper solutionWrapper = new SolutionWrapper(currentProjectWrapper.Project.DTE.Solution);
					ProjectWrapper projectWrapper = FindProjectWithModelDiffs(solutionWrapper, assemblyName);
					if(projectWrapper != null) {
						ProjectItem diffsFileProjectItem = projectWrapper.GetInvariantDiffsFile();
						if(diffsFileProjectItem != null) {
							Property buildActionProperty = diffsFileProjectItem.Properties.Item("BuildAction");
							if(buildActionProperty != null) {
								string diffsFileName = projectWrapper.GetInvariantDiffsFileName();
								if(3 == (int)buildActionProperty.Value) {
									if(!string.IsNullOrEmpty(diffsFileName)) {
										DesignTimeFileModelStore modelStore = new DesignTimeFileModelStore(Path.GetDirectoryName(diffsFileName), Path.GetFileNameWithoutExtension(diffsFileName), serviceProvider, projectWrapper);
										moduleBase.DiffsStore = modelStore;
										result.Add(new ModuleDiffStoreInfo(moduleBase.GetType(), modelStore, projectWrapper.Project.Name));
									}
								}
								else {
									IUIService uiService = (IUIService)serviceProvider.GetService(typeof(IUIService));
									uiService.ShowMessage(string.Format("A model differences file which is not an embedded resource is detected. Settings from such files are ignored at run time. To fix this issue, set the 'Build Action' property of the following file to 'Embedded Resource':\r\n\r\n'{0}'", diffsFileName), "", MessageBoxButtons.OK);
								}
							}
						}
					}
				}
			}
			return result;
		}
		private static ProjectWrapper FindProjectWithModelDiffs(SolutionWrapper solutionWrapper, string assemblyName) {
			foreach(ProjectWrapper xafProjectWrapper in solutionWrapper.XafProjects) {
				if(xafProjectWrapper.ContainsModelDiffs && xafProjectWrapper.AssemblyName == assemblyName) {
					return xafProjectWrapper;
				}
			}
			return null;
		}
		public ModelLoader(ProjectWrapper projectWrapper, ITypesInfo typesInfo, IServiceProvider serviceProvider) {
			this.typesInfo = typesInfo;
			this.projectWrapper = projectWrapper;
			this.serviceProvider = serviceProvider;
		}
		private bool IsDeviceSpecificModel(string targetDiffFileName) {
			return targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultMobileName + ModelDifferenceStore.ModelFileExtension) ||
				targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultTabletName + ModelDifferenceStore.ModelFileExtension) ||
				targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultDesktopName + ModelDifferenceStore.ModelFileExtension);
		}
		public IModelEditorController LoadModel(ITypeDiscoveryService typeDiscoveryService, ITypeResolutionService typeResolutionService, string targetDiffFileName, out IDisposable obj) {
			Project project = this.projectWrapper.Project;
			DesignerModelFactory dmf = new DesignerModelFactory();
			ProjectWrapper projectWrapper = ProjectWrapper.Create(project);
			string projectDiffFileName = projectWrapper.GetInvariantDiffsFileName();
			FileModelStore targetDiffStore = null;
			FileModelStore projectDiffStore = null;
			if(IsDeviceSpecificModel(targetDiffFileName)) {
				targetDiffStore = new FileModelStore(Path.GetDirectoryName(targetDiffFileName), Path.GetFileNameWithoutExtension(targetDiffFileName));
				projectDiffStore = new DesignTimeFileModelStore(Path.GetDirectoryName(projectDiffFileName), Path.GetFileNameWithoutExtension(projectDiffFileName), serviceProvider, projectWrapper);
			}
			else {
				bool isExternalTargetDiffStore = Path.GetDirectoryName(projectDiffFileName) != Path.GetDirectoryName(targetDiffFileName);
				if(isExternalTargetDiffStore) {
					targetDiffStore = new FileModelStore(Path.GetDirectoryName(targetDiffFileName), Path.GetFileNameWithoutExtension(targetDiffFileName));
					projectDiffStore = new DesignTimeFileModelStore(Path.GetDirectoryName(projectDiffFileName), Path.GetFileNameWithoutExtension(projectDiffFileName), serviceProvider, projectWrapper);
				}
				else {
					targetDiffStore = new DesignTimeFileModelStore(Path.GetDirectoryName(targetDiffFileName), Path.GetFileNameWithoutExtension(targetDiffFileName), serviceProvider, projectWrapper);
				}
			}
			IModelApplication modelApplication = null;
			ImageLoader.Reset();
			List<ModuleDiffStoreInfo> moduleDiffStoreInfos = null;
			if(IsApplicationProject(projectWrapper)) {
				XafApplication application = null;
				if(projectWrapper is WebApplicationWrapper) {
					application = GetXafApplicationByDesigner(project);
				}
				else if(projectWrapper is WinApplicationWrapper || projectWrapper is WebApplicationProjectWrapper) {
					if(typeDiscoveryService != null) {
						Type applicationType = FindDiscoveredType(typeDiscoveryService, typeof(XafApplication), project.Properties.Item("AssemblyName").Value.ToString());
						if(applicationType != null) {
							application = (XafApplication)ReflectionHelper.CreateObject(applicationType);
						}
					}
					if(application == null || (typeResolutionService != null && CheckLoadedAssembliesConflict(typeResolutionService, application))) {
						application = GetXafApplicationByDesigner(project);
					}
				}
				else {
					throw new ArgumentException(projectWrapper.GetType().FullName, "projectWrapper");
				}
				obj = application;
				ApplicationModulesManager modulesManager = dmf.CreateModulesManager(application, null, projectWrapper.BinariesDir);
				moduleDiffStoreInfos = PatchModulesDiffsStore(projectWrapper, modulesManager.Modules);
				if(projectDiffStore != null) {
					moduleDiffStoreInfos.Add(new ModuleDiffStoreInfo(null, projectDiffStore, projectWrapper.Project.Name));
				}
				modelApplication = dmf.CreateApplicationModel(application, modulesManager, null, projectDiffStore, targetDiffStore);
			}
			else if(projectWrapper is ModuleWrapper) {
				ModuleBase module = null;
				if(typeDiscoveryService != null) {
					Type moduleType = FindDiscoveredType(typeDiscoveryService, typeof(ModuleBase), project.Properties.Item("AssemblyName").Value.ToString());
					if(moduleType != null) {
						module = (ModuleBase)ReflectionHelper.CreateObject(moduleType);
					}
				}
				if(module == null) {
					module = GetXafModuleByDesigner(project);
				}
				ApplicationModulesManager modulesManager = dmf.CreateModulesManager(module, projectWrapper.BinariesDir);
				moduleDiffStoreInfos = PatchModulesDiffsStore(projectWrapper, modulesManager.Modules);
				obj = module;
				modelApplication = dmf.CreateApplicationModel(module, modulesManager, targetDiffStore);
			}
			else {
				throw new ArgumentException("Unknown project type", projectWrapper.GetType().FullName);
			}
			CaptionHelper.Setup(modelApplication);
			ModelEditorViewController modelEditorViewController = new ModelEditorViewController(modelApplication, targetDiffStore);
			modelEditorViewController.SetModuleDiffStore(moduleDiffStoreInfos);
			return modelEditorViewController;
		}
	}
}
