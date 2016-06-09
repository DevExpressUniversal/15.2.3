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
using System.Configuration;
using System.IO;
using System.Reflection;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Utils {
	public class DesignerModelFactory {
		private string ReadModulesFromConfig(string configFileName) {
			if(!File.Exists(configFileName)) {
				throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ConfigFileDoesNotExists, configFileName));
			}
			ExeConfigurationFileMap exeConfigurationFileMap = new System.Configuration.ExeConfigurationFileMap();
			exeConfigurationFileMap.ExeConfigFilename = configFileName;
			Configuration configuration = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, System.Configuration.ConfigurationUserLevel.None);
			if(configuration.AppSettings.Settings["Modules"] != null) {
				return configuration.AppSettings.Settings["Modules"].Value;
			}
			return string.Empty;
		}
		private string FindFileContainingApplicationType(string configFileName, string assembliesPath, bool isWebApplicationModel, string applicationFileName, ref string exceptions) {
			string targetFileEndsWith = isWebApplicationModel ? "*.dll" : "*.exe";
			string[] result = null;
			if(Directory.Exists(assembliesPath)) {
				if(isWebApplicationModel) {
					result = Directory.GetFiles(assembliesPath, "App_Code.dll");
					if(result.Length == 1) { return result[0]; }
				} else {
					if(Path.GetFileName(configFileName).ToLowerInvariant() != "app.config") {
						string exefile = configFileName.Substring(0, configFileName.LastIndexOf('.'));
						if(File.Exists(exefile) || File.Exists(Path.Combine(Environment.CurrentDirectory, exefile))) {
							return exefile.Substring(exefile.LastIndexOf('\\') + 1);
						}
					}
				}
				List<string> fileNames = new List<string>();
				foreach(string fileName in Directory.GetFiles(assembliesPath, targetFileEndsWith)) {
					if(Path.GetFileName(fileName).Contains("DevExpress.")) { continue; }
					if(IsFileContainingApplicationType(fileName, assembliesPath, ref exceptions)) {
						fileNames.Add(System.IO.Path.GetFullPath(fileName).ToLowerInvariant());
					}
				}
				if(fileNames.Count > 0) {
					if(fileNames.Count == 1) {
						return fileNames[0];
					}
					else {
						if(!string.IsNullOrEmpty(applicationFileName) && fileNames.Contains(Path.GetFullPath(applicationFileName).ToLowerInvariant())) {
							return Path.GetFullPath(applicationFileName).ToLowerInvariant();
						}
						else {
							return FindFileContainingApplicationType(fileNames, assembliesPath, applicationFileName);
						}
					}
				}
			}
			return null;
		}
		private string FindFileContainingApplicationType(List<string> fileNames, string assembliesPath, string assemblyName) {
			ReflectionHelper.AddResolvePath(assembliesPath);
			try {
				List<string> filesContainsXafApplicationDescendant = new List<string>();
				foreach(string fileName in fileNames) {
					Boolean containsApplicationType = (OnCustomApplicationTypeByAssembly(fileName, assembliesPath) != null);
					if(!containsApplicationType) {
						containsApplicationType = (GetApplicationType(fileName, assembliesPath) != null);
					}
					if(containsApplicationType) {
						filesContainsXafApplicationDescendant.Add(fileName);
					}
				}
				if(filesContainsXafApplicationDescendant.Count > 0) {
					if(filesContainsXafApplicationDescendant.Count == 1) {
						return filesContainsXafApplicationDescendant[0];
					}
					else {
						if(!string.IsNullOrEmpty(assemblyName)) {
							string targetAssemblyName = assemblyName.ToLowerInvariant();
							foreach(string fileFullName in filesContainsXafApplicationDescendant) {
								if(fileFullName.ToLowerInvariant() == targetAssemblyName || Path.GetFileNameWithoutExtension(fileFullName).ToLowerInvariant() == targetAssemblyName) {
									return fileFullName;
								}
							}
						}
						string message = string.Format("Multiple XafApplication descendants are found in the following assemblies:{0}{1}.", Environment.NewLine, string.Join("," + Environment.NewLine, filesContainsXafApplicationDescendant.ToArray()));
						throw new DesignerModelFactory_FindApplicationAssemblyException(message);
					}
				}
			}
			finally {
				ReflectionHelper.RemoveResolvePath(assembliesPath);
			}
			return null;
		}
		private IEnumerable<string> GetAspects(string configFileName) {
			if(!string.IsNullOrEmpty(configFileName) && configFileName.ToLowerInvariant().EndsWith(".config")) {
				System.Configuration.ConfigXmlDocument doc = new ConfigXmlDocument();
				ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap();
				exeConfigurationFileMap.ExeConfigFilename = configFileName;
				Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
				KeyValueConfigurationElement languagesElement = configuration.AppSettings.Settings["Languages"];
				if(languagesElement != null) {
					string languages = languagesElement.Value;
					return languages.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				}
			}
			return null;
		}
		private IEnumerable<string> GetAspects(string configFileName, ModelStoreBase modelDifferencesStore) {
			List<string> result = null;
			if(modelDifferencesStore != null) {
				IEnumerable<string> aspects = modelDifferencesStore.GetAspects();
				if(aspects != null) {
					result = new List<string>(aspects);
				}
			}
			if(!string.IsNullOrEmpty(configFileName)) {
				IEnumerable<string> aspects = GetAspects(configFileName);
				if(aspects != null) {
					if(result == null) {
						result = new List<string>(aspects);
					} else {
						foreach(string aspect in aspects) {
							if(!result.Contains(aspect)) {
								result.Add(aspect);
							}
						}
					}
				}
			}
			return result;
		}
		private Type GetApplicationType(string fileName, string assembliesPath) {
			Assembly assembly = ReflectionHelper.GetAssembly(Path.GetFileNameWithoutExtension(fileName), assembliesPath);
			Type[] types = AssemblyHelper.GetTypes(assembly, type => typeof(XafApplication).IsAssignableFrom(type) && !TypeHelper.IsObsolete(type));
			return GetLeafType(types, AssemblyHelper.GetName(assembly), typeof(XafApplication));
		}
		public Type GetLeafType(IList<Type> types, string assemblyName, Type baseType) {
			IList<Type> result = new List<Type>();
			result = GetAllLeafTypes(types, assemblyName);
			if(result.Count == 1) {
				return result[0];
			} else {
				if(result.Count > 1) {
					List<Type> findCreationTypes = new List<Type>();
					foreach(Type item in result) {
						if(!item.IsAbstract && !item.ContainsGenericParameters && item.IsPublic) {
							findCreationTypes.Add(item);
						}
					}
					if(findCreationTypes.Count > 1) {
						string typesName = string.Join(", ", Enumerator.ToArray<string>(Enumerator.Convert<Type, string>(findCreationTypes, delegate(Type source) { return source.Name; })));
						string message = string.Format("Cannot choose a descendant of the '{0}' base class to instantiate. Multiple descendant classes detected: {1}", baseType, typesName);
						throw new Exception(message);
					} else {
						if(findCreationTypes.Count > 0) {
							return findCreationTypes[0];
						}
					}
				}
			}
			return null;
		}
		private static bool IsTypeBaseForTypes(IList<Type> types, Type baseType) {
			foreach(Type type in types) {
				Type currentType = type.BaseType;
				while(currentType != null) {
					if(currentType == baseType) {
						return true;
					}
					currentType = currentType.BaseType;
				}
			}
			return false;
		}
		private List<Type> GetAllLeafTypes(IList<Type> types, string assemblyName) {
			List<Type> result = new List<Type>();
			foreach(Type type in types) {
				if(assemblyName == null || !type.ContainsGenericParameters && type.Assembly.GetName().Name == assemblyName) {
					if(!IsTypeBaseForTypes(types, type)) {
						result.Add(type);
					}
				}
			}
			return result;
		}
		private ITypeInfo OnCustomApplicationTypeByAssembly(string assemblyFileName, string assembliesPath) {
			if(CustomApplicationTypeByAssembly != null) {
				CustomApplicationTypeByAssebblyEventArgs args = new CustomApplicationTypeByAssebblyEventArgs(assemblyFileName, assembliesPath);
				CustomApplicationTypeByAssembly(this, args);
				return args.AplicationType;
			}
			return null;
		}
		protected virtual bool IsFileContainingApplicationType(string fileName, string assembliesPath, ref string exceptions) {
			Assembly assembly = null;
			try {
				assembly = ReflectionHelper.GetAssembly(Path.GetFileNameWithoutExtension(fileName), assembliesPath);
			}
			catch(BadImageFormatException e) {
				Tracing.Tracer.LogError(e);
				exceptions += e.Message;
				return false;
			}
			foreach(Type type in AssemblyHelper.GetTypes(assembly)) {
				if(typeof(XafApplication).IsAssignableFrom(type) && !TypeHelper.IsObsolete(type)) {
					return true;
				}
			}
			return false;
		}
		public string GetFileContainingApplicationType(string configFileName, string applicationFileName, ref string assembliesPath) {
			string exceptions = string.Empty;
			return GetFileContainingApplicationType(configFileName, applicationFileName, ref assembliesPath, ref exceptions);
		}
		public string GetFileContainingApplicationType(string configFileName, string applicationFileName, ref string assembliesPath, ref string exceptions) {
			string result = null;
			bool isWebApplicationModel = string.Compare(Path.GetFileNameWithoutExtension(configFileName), "web", true) == 0;
			if(string.IsNullOrEmpty(assembliesPath)) {
				assembliesPath = Path.GetDirectoryName(configFileName);
				if(string.IsNullOrEmpty(assembliesPath)) {
					assembliesPath = Environment.CurrentDirectory;
				}
			}
			result = FindFileContainingApplicationType(configFileName, assembliesPath, isWebApplicationModel, applicationFileName, ref exceptions);
			if(string.IsNullOrEmpty(result)) {
				string assembliesPathTemp = isWebApplicationModel ? Path.Combine(assembliesPath, "Bin") : Path.Combine(assembliesPath, @"Bin\Debug");
				result = FindFileContainingApplicationType(configFileName, assembliesPathTemp, isWebApplicationModel, applicationFileName, ref exceptions);
				if(!string.IsNullOrEmpty(result)) {
					assembliesPath = assembliesPathTemp;
				}
			}
			return result;
		}
		public string GetFileContainingApplicationType(string configFileName, ref string assembliesPath) {
			return GetFileContainingApplicationType(configFileName, null, ref assembliesPath);
		}
		public XafApplication CreateApplicationByConfigFile(string configFileName, string applicationFileName, ref string assembliesPath) {
			string exceptions = string.Empty;
			string targetFileName = GetFileContainingApplicationType(configFileName, applicationFileName, ref assembliesPath, ref exceptions);
			if(targetFileName != null) {
				ReflectionHelper.AddResolvePath(assembliesPath);
				try {
					Type applicationType = GetApplicationType(targetFileName, assembliesPath);
					if(applicationType != null) {
						return (XafApplication)TypeHelper.CreateInstance(applicationType);
					}
				} finally {
					ReflectionHelper.RemoveResolvePath(assembliesPath);
				}
			}
			if(!string.IsNullOrEmpty(exceptions)) {
				throw new Exception(exceptions);
			}
			throw new Exception(string.Format(DevExpress.ExpressApp.Localization.SystemExceptions.CannotFindDescendantOfXafApplicationClass, assembliesPath));
		}
		public XafApplication CreateApplicationByConfigFile(string configFileName, ref string assembliesPath) {
			return CreateApplicationByConfigFile(configFileName, null, ref assembliesPath);
		}
		public bool IsModule(string targetPath) {
			string lowerFileName = targetPath.ToLowerInvariant();
			bool result = false;
			if(Path.GetExtension(lowerFileName) == ".dll") {
				Assembly assembly = ReflectionHelper.GetAssembly(Path.GetFileNameWithoutExtension(lowerFileName), Path.GetDirectoryName(lowerFileName));
				foreach(Type type in AssemblyHelper.GetTypes(assembly)) {
					if(typeof(ModuleBase).IsAssignableFrom(type)) {
						result = true;
						break;
					}
				}
			}
			return result;
		}
		public bool IsApplication(string targetPath) {
			string lowerFileName = targetPath.ToLowerInvariant();
			return Path.GetExtension(lowerFileName) == ".config";
		}
		public ModuleBase CreateModuleFromFile(string fileName, string assembliesPath) {
			ReflectionHelper.AddResolvePath(assembliesPath);
			try {
				IList<Type> moduleTypes = ApplicationModulesManager.GetModuleTypes(Path.GetFileNameWithoutExtension(fileName), assembliesPath);
				if(moduleTypes.Count == 0) {
					throw new ArgumentException("The '" + Path.GetFileNameWithoutExtension(fileName) + "' assembly doesn't contain a ModuleBase descendants");
				}
				Type moduleType = GetLeafType(moduleTypes, null, typeof(ModuleBase));
				return ModuleFactory.WithEmptyDiffs.CreateModule(moduleType);
			} finally {
				ReflectionHelper.RemoveResolvePath(assembliesPath);
			}
		}
		public FileModelStore CreateModuleModelStore(string moduleDiffsPath) {
			return CreateApplicationModelStore(moduleDiffsPath, ModelDifferenceStore.ModelDiffDefaultName);
		}
		public FileModelStore CreateApplicationModelStore(string diffsPath) {
			return CreateApplicationModelStore(diffsPath, ModelDifferenceStore.AppDiffDefaultName);
		}
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public FileModelStore CreateApplicationModelStore(string diffsPath, string fileNameTemplate) {
			return new FileModelStore(diffsPath, fileNameTemplate);
		}
		private IModelApplication CreateApplicationModelCore(ApplicationModulesManager modulesManager, IEnumerable<Type> localizerTypes, IEnumerable<string> aspects, ModelStoreBase modelDifferencesStore, ModelStoreBase userDifferencesStore) {
			ApplicationModelManager modelManager = new ApplicationModelManager();
			modelManager.Setup(XafTypesInfo.Instance, modulesManager.DomainComponents, modulesManager.Modules, modulesManager.ControllersManager.Controllers, localizerTypes, aspects, modelDifferencesStore, null);
			ModelApplicationBase lastLayer = modelManager.CreateLayerByStore("Edited model", userDifferencesStore != null ? userDifferencesStore : ModelStoreBase.Empty);
			ModelApplicationBase model = modelManager.CreateModelApplication(new ModelApplicationBase[] { lastLayer });
			return (IModelApplication)model;
		}
		public IModelApplication CreateApplicationModel(XafApplication application, ApplicationModulesManager modulesManager, string configFileName, ModelStoreBase modelDifferencesStore) {
			return CreateApplicationModelCore(modulesManager, application.ResourcesExportedToModel, GetAspects(configFileName, modelDifferencesStore), null, modelDifferencesStore);
		}
		public IModelApplication CreateApplicationModel(XafApplication application, ApplicationModulesManager modulesManager, string configFileName, ModelStoreBase modelDifferencesStore, ModelStoreBase userModelDifferencesStore) {
			return CreateApplicationModelCore(modulesManager, application.ResourcesExportedToModel, GetAspects(configFileName, userModelDifferencesStore), modelDifferencesStore, userModelDifferencesStore);
		}
		public IModelApplication CreateApplicationModel(ModuleBase module, ApplicationModulesManager modulesManager, ModelStoreBase modelDifferencesStore) {
			module.DiffsStore = ModelStoreBase.Empty;
			IEnumerable<string> aspects = modelDifferencesStore != null ? modelDifferencesStore.GetAspects() : new string[0];
			return CreateApplicationModelCore(modulesManager, Type.EmptyTypes, aspects, null, modelDifferencesStore);
		}
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public ApplicationModelManager CreateApplicationModelManager(ModuleBase module, ApplicationModulesManager modulesManager) {
			ApplicationModelManager modelManager = new ApplicationModelManager();
			modelManager.Setup(XafTypesInfo.Instance, modulesManager.DomainComponents, modulesManager.Modules, modulesManager.ControllersManager.Controllers, Type.EmptyTypes, new string[0], null, null);
			return modelManager;
		}
		public ApplicationModulesManager CreateModulesManager(ModuleBase module, string assembliesPath) {
			ReflectionHelper.AddResolvePath(assembliesPath);
			try {
				ApplicationModulesManager result = new ApplicationModulesManager(new ControllersManager(), assembliesPath);
				foreach(Type moduleType in module.RequiredModuleTypes) {
					result.AddModule(moduleType);
				}
				result.AddModule(module);
				result.Load();
				return result;
			} finally {
				ReflectionHelper.RemoveResolvePath(assembliesPath);
			}
		}
		public ApplicationModulesManager CreateModulesManager(XafApplication application, string configFileName, string assembliesPath) {
			Guard.ArgumentNotNull(application, "Application");
			ReflectionHelper.AddResolvePath(assembliesPath);
			try {
				ApplicationModulesManager result = new ApplicationModulesManager(new ControllersManager(), assembliesPath);
				foreach(ModuleBase module in application.Modules) {
					result.AddModule(module);
				}
				result.Security = application.Security;
				if(!string.IsNullOrEmpty(configFileName)) {
					result.AddModuleFromAssemblies(ReadModulesFromConfig(configFileName).Split(';'));
				}
				result.Load();
				return result;
			} finally {
				ReflectionHelper.RemoveResolvePath(assembliesPath);
			}
		}
		public event EventHandler<CustomApplicationTypeByAssebblyEventArgs> CustomApplicationTypeByAssembly;
	}
	public class CustomApplicationTypeByAssebblyEventArgs : EventArgs {
		string assemblyFileName = null;
		string assembliesPath = null;
		ITypeInfo aplicationType = null;
		public CustomApplicationTypeByAssebblyEventArgs(string assemblyFileName, string assembliesPath) {
			this.assemblyFileName = assemblyFileName;
			this.assembliesPath = assembliesPath;
		}
		public string AssemblyFileName {
			get {
				return assemblyFileName;
			}
		}
		public string AssembliesPath {
			get {
				return assembliesPath;
			}
		}
		public ITypeInfo AplicationType {
			get {
				return aplicationType;
			}
			set {
				aplicationType = value;
			}
		}
	}
	[Serializable]
	public class DesignerModelFactory_FindApplicationAssemblyException : Exception {
		protected DesignerModelFactory_FindApplicationAssemblyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		public DesignerModelFactory_FindApplicationAssemblyException(string message) : base(message) { }
	}
}
