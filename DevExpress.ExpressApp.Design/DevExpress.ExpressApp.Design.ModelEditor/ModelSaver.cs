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
using DevExpress.ExpressApp.Design.Core;
using System.Configuration;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using EnvDTE;
using System.IO;
using DevExpress.ExpressApp.Core;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Design.ModelEditor {
	[System.Security.SecuritySafeCritical]
	public class ModelSaver {
		private IServiceProvider serviceProvider = null;
		private IProjectWrapper projectWrapper = null;
		private IModelEditorController modelEditorController = null;
		private System.Configuration.Configuration OpenConfiguration(string configFilePath) {
			System.Configuration.Configuration result = null;
			if(!string.IsNullOrEmpty(configFilePath)) {
				result = ObjectFactory.CreateInstance<System.Configuration.Configuration>(delegate() {
					ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap();
					exeConfigurationFileMap.ExeConfigFilename = configFilePath;
					try {
						return System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
					}
					catch(ConfigurationErrorsException) {
					}
					return null;
				});
			}
			return result;
		}
		private void SaveConfiguration(string configFilePath, System.Configuration.Configuration configuration) {
			if(serviceProvider != null) {
				if(ProjectWrapper.CanEditFile(new string[] { configFilePath }, serviceProvider)) {
					configuration.Save();
				}
			}
#if DEBUG
			else {
				configuration.Save();
			}
#endif
		}
		private void AddAspectsToConfigFile(string configFilePath, IEnumerable<string> aspects) {
			System.Configuration.Configuration configuration = OpenConfiguration(configFilePath);
			if(configuration != null) {
				List<string> currentLanguages = new List<string>(aspects);
				bool isModified = currentLanguages.Count > 0;
				if(isModified) {
					KeyValueConfigurationElement languagesElement = configuration.AppSettings.Settings["Languages"];
					if(languagesElement == null) {
						languagesElement = new KeyValueConfigurationElement("Languages", string.Empty);
						configuration.AppSettings.Settings.Add(languagesElement);
					} else {
						string[] languages = languagesElement.Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
						foreach(string language in languages) {
							if(currentLanguages.IndexOf(language) == -1) {
								currentLanguages.Add(language);
							}
						}
					}
					string currentValue = string.Join(";", currentLanguages.ToArray());
					if(languagesElement.Value != currentValue) {
						languagesElement.Value = currentValue;
						SaveConfiguration(configFilePath, configuration);
					}
				}
			}
		}
		private void AddNewFile(string fileName, ProjectItem invariantProjectItem) {
			ProjectItem pi = null;
			if(invariantProjectItem.ProjectItems != null) {
				pi = invariantProjectItem.ProjectItems.AddFromFile(fileName);
			}
			else if(invariantProjectItem.ContainingProject != null) {
				pi = invariantProjectItem.ContainingProject.ProjectItems.AddFromFile(Path.GetFullPath(fileName));
			}
			if(pi != null) {
				try {
					if(pi.Name.Contains(ModelStoreBase.UnusableDiffDefaultName)) {
						pi.Properties.Item("BuildAction").Value = VSLangProj.prjBuildAction.prjBuildActionNone;
					}
					else {
						pi.Properties.Item("BuildAction").Value = invariantProjectItem.Properties.Item("BuildAction").Value;
						pi.Properties.Item("CopyToOutputDirectory").Value = invariantProjectItem.Properties.Item("CopyToOutputDirectory").Value; 
					} 
					pi.Properties.Item("Subtype").Value = "Designer";
				}
				catch(ArgumentException) { }
			}
		}
		private void AddAspectsToDependentConfigFiles(string assemblyName, IEnumerable<string> addAspects) {
			ISolutionWrapper solutionWrapper =  ObjectFactory.CreateInstance<ISolutionWrapper>(
			delegate() { return new SolutionWrapper(projectWrapper.Project.DTE.Solution); });
			foreach(IProjectWrapper xafProjectWrapper in solutionWrapper.XafProjects) {
				string configFileName = xafProjectWrapper.ConfigFilePath;
				if(configFileName != null && xafProjectWrapper.ContainsReference(assemblyName)) {
					AddAspectsToConfigFile(configFileName, addAspects);	
				}
			}
		}
		public ModelSaver(IServiceProvider serviceProvider, IProjectWrapper projectWrapper, IModelEditorController modelEditorController) {
			this.serviceProvider = serviceProvider;
			this.projectWrapper = projectWrapper;
			this.modelEditorController = modelEditorController;
		}
		public bool Save() {
			bool result = true;
			if(modelEditorController.IsModified) {
				result = modelEditorController.Save();
				string currentProjectDiffsPath = Path.GetDirectoryName(projectWrapper.GetInvariantDiffsFileName());
				foreach(string fileName in modelEditorController.LastSavedFiles) {
					ProjectItem invariantProjectItem = null;
					string diffsPath = Path.GetDirectoryName(fileName);
					if(diffsPath != currentProjectDiffsPath) {
						SolutionWrapper solutionWrapper = new SolutionWrapper(projectWrapper.Project.DTE.Solution);
						foreach(IProjectWrapper currentProjectWrapper in solutionWrapper.XafProjects) {
							if(currentProjectWrapper.IsExpressAppProject && currentProjectWrapper.ContainsModelDiffs) {
								string currentInvariantDiffsFileName = currentProjectWrapper.GetInvariantDiffsFileName();
								if(!string.IsNullOrEmpty(currentInvariantDiffsFileName) && Path.GetDirectoryName(currentInvariantDiffsFileName) == diffsPath) {
									invariantProjectItem = currentProjectWrapper.GetDeviceSpecificInvariantDiffsFile(GetDeviceSpecificModelDiffDefaultName(fileName));
									break;
								}
							}
						}
					}
					else {
						invariantProjectItem = projectWrapper.GetDeviceSpecificInvariantDiffsFile(GetDeviceSpecificModelDiffDefaultName(fileName));
					}
					string invariantDiffsFileName = invariantProjectItem != null ? invariantProjectItem.get_FileNames(0).ToLower() : null;
					if(!string.IsNullOrEmpty(invariantDiffsFileName) && invariantDiffsFileName != Path.GetFullPath(fileName).ToLower()) {
						bool isFound = false;
						foreach(ProjectItem item in projectWrapper.GetDiffsFiles()) {
							if(item.get_FileNames(0).ToLower() == Path.GetFullPath(fileName).ToLower()) {
								isFound = true;
								break;
							}
						}
						if(!isFound) {
							AddNewFile(Path.GetFullPath(fileName), invariantProjectItem);
							string diffsNameTemplate = GetDeviceSpecificModelDiffDefaultName(fileName);
							if(string.IsNullOrEmpty(diffsNameTemplate)) {
								diffsNameTemplate = projectWrapper.DiffsNameTemplate;
							}
							FileModelStore fileModelStore = ObjectFactory.CreateInstance<FileModelStore>(delegate() {
								return new FileModelStore(diffsPath, projectWrapper.DiffsNameTemplate);
							});
							AddAspectsToDependentConfigFiles(projectWrapper.AssemblyName, fileModelStore.GetAspects());
						}
					}
				}
			}
			return result;
		}
		private string GetDeviceSpecificModelDiffDefaultName(string targetDiffFileName) {
			if(targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultMobileName)) {
				return ModelDifferenceStore.AppDiffDefaultMobileName;
			}
			else {
				if(targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultTabletName)) {
					return ModelDifferenceStore.AppDiffDefaultTabletName;
				}
				else {
					if(targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultDesktopName)) {
						return ModelDifferenceStore.AppDiffDefaultDesktopName;
					}
				}
			}
			return null;
		}
	}
}
