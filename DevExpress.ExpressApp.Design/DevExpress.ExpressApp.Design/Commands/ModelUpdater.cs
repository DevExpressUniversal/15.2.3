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
using System.IO;
using System.Windows.Forms;
using DevExpress.ExpressApp.Design.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using Microsoft.VisualStudio.Shell.Design;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.Reflection;
namespace DevExpress.ExpressApp.Design {
	public class CrossDomainBridge : MarshalByRefObject {
		private IServiceProvider serviceProvider;
		public CrossDomainBridge(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}
		public bool CanEditFile(string[] documents) {
			return ProjectWrapper.CanEditFile(documents, serviceProvider);
		}
		public IDictionary<string, string> FindEFContextTypes(string fullProjectName) {
			Dictionary<string, string> contextTypes = new Dictionary<string, string>();
			if(serviceProvider != null) {
				IVsHierarchy pvHier;
				IVsSolution solution = serviceProvider.GetService(typeof(IVsSolution)) as IVsSolution;
				solution.GetProjectOfUniqueName(fullProjectName, out pvHier);
				DynamicTypeService dynamicTypeService = (DynamicTypeService)serviceProvider.GetService(typeof(DynamicTypeService));
				ITypeDiscoveryService typeDiscoveryService = (ITypeDiscoveryService)dynamicTypeService.GetTypeDiscoveryService(pvHier);
				foreach(Type type in EFDesignTimeTypeInfoHelper.GetEntityContextTypes(typeDiscoveryService)) {
					if(!type.Assembly.FullName.StartsWith("EntityFramework"))
					contextTypes[type.FullName] = type.Assembly.FullName;
				}
			}
			return contextTypes;
		}
	}
	public class ModelUpdater : MarshalByRefObject {
		private CrossDomainBridge crossDomainBridge;
		private bool hasUnusableNodes;
		private string unusableNodesFileNameTemplate = string.Empty;
		private string GetUnusableNodeFileNameTemplate(string diffsDirectory) {
			bool useDefaultName = true;
			string[] unusableNodesFiles = Directory.GetFiles(diffsDirectory, ModelStoreBase.UnusableDiffDefaultName + "*.xml");
			foreach(string filename in unusableNodesFiles) {
				if(!crossDomainBridge.CanEditFile(new string[] { filename })) {
					MessageBox.Show("Unable to save the unuseable nodes to the file " + filename, "Update Model", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					useDefaultName = false;
				}
				else if(File.Exists(filename) && ((File.GetAttributes(filename) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)) {
					useDefaultName = false;
				}
			}
			return useDefaultName ? ModelStoreBase.UnusableDiffDefaultName : ModelStoreBase.UnusableDiffDefaultName + DateTime.Now.ToString().Replace("/", "").Replace(".", "").Replace(":", "").Replace(" ", "_");
		}
		private void InitEFTypesInfo(string fullProjectName, string modulesDir) {
			ExportedTypeHelpers.AddExportedTypeHelper(new EFDesignTimeExportedTypeHelper());
			List<Type> contextTypes = new List<Type>();
			IDictionary<string, string> assemblyTypes = crossDomainBridge.FindEFContextTypes(fullProjectName);
			DevExpress.Persistent.Base.ReflectionHelper.AddResolvePath(modulesDir);
			try {
				foreach(string typeName in assemblyTypes.Keys) {
					Assembly assembly = DevExpress.Persistent.Base.ReflectionHelper.GetAssembly(assemblyTypes[typeName], modulesDir);
					Type contextType = assembly.GetType(typeName);
					if(contextType != null) {
						contextTypes.Add(contextType);
					}
				}
			}
			finally {
				DevExpress.Persistent.Base.ReflectionHelper.RemoveResolvePath(modulesDir);
			}
			EFDesignTimeTypeInfoHelper.ForceInitialize(contextTypes);
		}
		private void UpdateModel(string targetFileName, string modulesDir, string diffsDirectory, string diffsNameTemplate, string assemblyName, string fullProjectName) {
			DesignerModelFactory dmf = new DesignerModelFactory();
			if(dmf.IsModule(targetFileName) || dmf.IsApplication(targetFileName)) {
				string[] targetFiles = Directory.GetFiles(diffsDirectory, diffsNameTemplate + "*.xafml");
				if(targetFiles.Length > 0) {
					if(!crossDomainBridge.CanEditFile(targetFiles)) {
						string message = "Cannot edit the Application Model because the following files are locked:" +
							Environment.NewLine + string.Join(Environment.NewLine, targetFiles);
						MessageBox.Show(message, "Update Model", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
					else {
						DevExpress.Persistent.Base.ReflectionHelper.Reset();
						XafTypesInfo.HardReset();
						DevExpress.ExpressApp.Xpo.XpoTypesInfoHelper.ForceInitialize();
						FileModelStore fileModelStore = null;
						ModelApplicationBase modelApplication = null;
						DevExpress.ExpressApp.Model.Core.DesignerOnlyCalculator.IsRunFromDesigner = true;
						XafTypesInfo.Reset();
						InitEFTypesInfo(fullProjectName, modulesDir);
						if(dmf.IsApplication(targetFileName)) {
							XafApplication application = dmf.CreateApplicationByConfigFile(targetFileName, assemblyName, ref modulesDir);
							fileModelStore = dmf.CreateApplicationModelStore(diffsDirectory);
							modelApplication = (ModelApplicationBase)dmf.CreateApplicationModel(application, dmf.CreateModulesManager(application, targetFileName, diffsDirectory), targetFileName, fileModelStore);
						}
						else {
							ModuleBase module = dmf.CreateModuleFromFile(targetFileName, modulesDir);
							fileModelStore = dmf.CreateModuleModelStore(diffsDirectory);
							modelApplication = (ModelApplicationBase)dmf.CreateApplicationModel(module, dmf.CreateModulesManager(module, diffsDirectory), fileModelStore);
						}
						if(modelApplication != null) {
							ModelApplicationBase unusedModel = ModelEditorHelper.GetFullUnusableModel(modelApplication);
							hasUnusableNodes = unusedModel != null && unusedModel.HasModification;
							unusableNodesFileNameTemplate = GetUnusableNodeFileNameTemplate(Path.GetDirectoryName(diffsDirectory));
							try {
								fileModelStore.SaveDifference(modelApplication.LastLayer);
							}
							catch(UnauthorizedAccessException e) {
								MessageBox.Show("Cannot to save the updated model to the file: " + e.Message, "Update Model", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							}
							catch {
								MessageBox.Show("Cannot to save the updated model to the file: " + fileModelStore.Name, "Update Model", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							}
						}
					}
				}
			}
		}
		public void UpdateModel(string targetFileName, string modulesDir, string diffsDirectory, string diffsNameTemplate, string[] deviceSpecificDiffsNameTemplates,  string assemblyName, string fullProjectName) {
			UpdateModel(targetFileName, modulesDir, diffsDirectory, diffsNameTemplate, assemblyName, fullProjectName);
			foreach(string deviceSpecificDiffsTemplateName in deviceSpecificDiffsNameTemplates){
				UpdateModel(targetFileName, modulesDir, diffsDirectory, deviceSpecificDiffsTemplateName, assemblyName, fullProjectName);
			}
		}
		public bool HasUnusableNodes {
			get { return hasUnusableNodes; }
		}
		public string UnusableNodesFileNameTemplate {
			get { return unusableNodesFileNameTemplate; }
		}
		public CrossDomainBridge CrossDomainBridge {
			get {
				return crossDomainBridge;
			}
			set {
				crossDomainBridge = value;
			}
		}
	}
}
