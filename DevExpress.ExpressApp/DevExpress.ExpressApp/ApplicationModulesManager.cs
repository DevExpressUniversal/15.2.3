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
using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp {
	public sealed class ModuleTypeList : TypeCollection {
		protected override void OnInsert(Type item) {
			base.OnInsert(item);
			Guard.TypeArgumentIs(typeof(ModuleBase), item, "item");
		}
		public ModuleTypeList() { }
		public ModuleTypeList(params Type[] moduleTypes) {
			AddRange(moduleTypes);
		}
	}
	public class ApplicationModulesManager {
		private ControllersManager controllersManager;
		private ModuleList modules;
		private ISecurityStrategyBase security;
		private Boolean isLoaded;
		private String assembliesPath;
		private static Object lockObject = new Object();
		protected ExportedTypeCollection allDomainComponents = new ExportedTypeCollection();
		private static void AddModuleIntoList(ModuleBase module, IList<ModuleBase> moduleList, List<ModuleBase> requiredModules) {
			foreach(Type referencedModuleType in module.RequiredModuleTypes) {
				EnsureModuleInList(referencedModuleType, moduleList, requiredModules);
			}
			if(!moduleList.Contains(module)) {
				moduleList.Add(module);
			}
		}
		private static void EnsureModuleInList(Type moduleType, IList<ModuleBase> moduleList, List<ModuleBase> requiredModules) {
			ModuleBase module = null;
			Type typeToCreate = ModuleBase.GetRealModuleType(moduleType);
			foreach(ModuleBase existingModule in moduleList) {
				if(existingModule.GetType() == typeToCreate) {
					module = existingModule;
					break;
				}
			}
			if(module == null) {
				module = ModuleFactory.WithResourcesDiffs.CreateModule(typeToCreate);
			}
			AddModuleIntoList(module, moduleList, requiredModules);
			if(!requiredModules.Contains(module)) {
				requiredModules.Add(module);
			}
		}
		private static bool CheckAssemblyVersionCompatibility(Version currentAssemblyVersion, Version coreAssemblyVersion) {
			return (
				(currentAssemblyVersion.Major == coreAssemblyVersion.Major) &&
				(currentAssemblyVersion.Minor == coreAssemblyVersion.Minor) &&
				(currentAssemblyVersion.Build <= coreAssemblyVersion.Build)
				);
		}
		public static ModuleTypeList GetModuleTypes(String assemblyName, String assembliesPath) {
			Assembly assembly = ReflectionHelper.GetAssembly(assemblyName.Trim(), assembliesPath);
			Boolean isCoreAssemblyFound = false;
			AssemblyName coreAssemblyName = typeof(ModuleBase).Assembly.GetName();
			if(!ReflectionHelper.CompareAssemblyName(assembly, coreAssemblyName.Name)) {
				foreach(AssemblyName currentAssemblyName in assembly.GetReferencedAssemblies()) {
					if(currentAssemblyName.Name == coreAssemblyName.Name) {
						isCoreAssemblyFound = true;
						if(!CheckAssemblyVersionCompatibility(currentAssemblyName.Version, coreAssemblyName.Version)) {
							throw new Exception(string.Format(
								"The assembly '{0}' refers assembly '{1}' version {2} instead of {3}",
								assemblyName, currentAssemblyName.Name, currentAssemblyName.Version.ToString(), coreAssemblyName.Version.ToString()));
						}
					}
				}
				if(!isCoreAssemblyFound) {
					throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.TheAssemblyDoesntReferAssembly, assemblyName, coreAssemblyName.Name, coreAssemblyName.Version.ToString()));
				}
			}
			Type[] moduleTypes = AssemblyHelper.GetTypes(assembly, type => typeof(ModuleBase).IsAssignableFrom(type) && !TypeHelper.IsObsolete(type));
			return new ModuleTypeList(moduleTypes);
		}
		public static List<ModuleBase> AddModuleIntoList(ModuleBase module, IList<ModuleBase> moduleList) {
			List<ModuleBase> requiredModules = new List<ModuleBase>();
			AddModuleIntoList(module, moduleList, requiredModules);
			return requiredModules;
		}
		private void CheckIsLoaded() {
			if(isLoaded) {
				throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ModulesHasAlreadyBeenLoaded));
			}
		}
		private Boolean ContainsModuleFromAssembly(String assemblyName) {
			foreach(ModuleBase module in modules) {
				if(String.Compare(module.AssemblyName, assemblyName, true) == 0) {
					return true;
				}
			}
			return false;
		}
		private void LoadModulesFromAssembly(String assemblyName) {
			if(!ContainsModuleFromAssembly(assemblyName)) {
				ModuleTypeList moduleTypes = GetModuleTypes(assemblyName, assembliesPath);
				foreach(Type type in moduleTypes) {
					AddModule(type, true);
				}
			}
		}
		private void Init(ControllersManager controllersManager, String assembliesPath) {
			Tracing.Tracer.LogVerboseText("ApplicationModulesManager.Init");
			this.assembliesPath = assembliesPath;
			this.modules = new ModuleList();
			this.controllersManager = controllersManager;
		}
		private void SetupModules() {
			Tracing.Tracer.LogText("SetupModules");
			foreach(ModuleBase module in modules) {
				try {
					module.Setup(this);
				}
				catch(Exception e) {
					throw new InvalidOperationException(String.Format("Exception occurs while initializing the '{0}' module: {1}", module.GetType().FullName, e.Message), e);
				}
			}
			foreach(Controller controller in controllersManager.Controllers) {
				ISupportSetup supportSetupItem = controller as ISupportSetup;
				if(supportSetupItem != null) {
					try {
						supportSetupItem.Setup(this);
					}
					catch(Exception e) {
						throw new InvalidOperationException(String.Format("Exception occurs while initializing the '{0}' controller: {1}", controller.GetType().FullName, e.Message), e);
					}
				}
			}
		}
		private void DoCustomizeTypesInfo(ITypesInfo typesInfo) {
			Tracing.Tracer.LogText("Customize TypesInfo");
			lock(lockObject) {
				foreach(ModuleBase module in modules) {
					try {
						module.CustomizeTypesInfo(typesInfo);
					}
					catch(Exception e) {
						throw new InvalidOperationException(String.Format("Exception occurs while the '{0}' customizes TypesInfo: {1}", module.GetType().FullName, e.Message), e);
					}
				}
				foreach(Controller controller in controllersManager.Controllers) {
					try {
						controller.CustomizeTypesInfo(typesInfo);
					}
					catch(Exception e) {
						throw new InvalidOperationException(String.Format("Exception occurs while the '{0}' customizes TypesInfo: {1}", controller.GetType().FullName, e.Message), e);
					}
				}
				OnCustomizeTypesInfo(typesInfo);
			}
		}
		private void LoadRegularTypesToTypesInfo(ITypesInfo typesInfo) {
			foreach(ModuleBase module in modules) {
				IEnumerable<Type> regularTypes = module.GetRegularTypes();
				if(regularTypes != null) {
					foreach(Type regularType in regularTypes) {
						typesInfo.FindTypeInfo(regularType);
					}
				}
				((DcAssemblyInfo)XafTypesInfo.Instance.FindAssemblyInfo(module.GetType().Assembly)).AllTypesLoaded = true;
			}
		}
		private void RegisterControllerTypes() {
			foreach(ModuleBase module in modules) {
				List<Type> controllerTypes = new List<Type>(module.GetControllerTypes());
				controllersManager.RegisterControllerTypes(controllerTypes.ToArray());
			}
		}
		protected virtual void CollectDomainComponents() {
			foreach(ModuleBase module in modules) {
				allDomainComponents.AddRange(module.GetExportedTypes());
			}
			if(security != null) {
#pragma warning disable 0618
				IEnumerable<Type> securityTypes = ModuleHelper.CollectRequiredExportedTypes(security.GetBusinessClasses(), ExportedTypeHelpers.IsExportedType);
#pragma warning restore 0618
				allDomainComponents.AddRange(securityTypes);
			}
		}
		protected virtual void LoadTypesInfo(IList<ModuleBase> modules, ITypesInfo typesInfo) {
			foreach(Type type in allDomainComponents) {
				typesInfo.RegisterEntity(type);
			}
		}
		protected virtual void OnCustomizeTypesInfo(ITypesInfo typesInfo) {
			if(CustomizeTypesInfo != null) {
				CustomizeTypesInfo(this, new CustomizeTypesInfoEventArgs(typesInfo));
			}
		}
		protected virtual void OnModuleRegistered(ModuleBase module) {
			if(ModuleRegistered != null) {
				ModuleRegistered(this, new ModuleRegisteredEventArgs(module));
			}
		}
		public ApplicationModulesManager(ControllersManager controllersManager, string assembliesPath) {
			Init(controllersManager, assembliesPath);
			Tracing.Tracer.LogSubSeparator("");
		}
		public ApplicationModulesManager() : this(new ControllersManager(), "") { }
		public void Clear() {
			Init(controllersManager, assembliesPath);
			isLoaded = false;
		}
		public void Load(ITypesInfo typesInfo, Boolean loadTypesInfo) {
			Tracing.Tracer.LogVerboseText("-> ApplicationModulesManager.Load");
			CheckIsLoaded();
			Tracing.Tracer.LogLoadedAssemblies();
			Tracing.Tracer.LockFlush();
			try {
				Tick.In("Load");
				Tick.In("Load.1");
				Tick.In("LoadRegularTypesToTypesInfo");
				LoadRegularTypesToTypesInfo(typesInfo);
				Tick.Out("LoadRegularTypesToTypesInfo");
				RegisterControllerTypes();
				Tick.Out("Load.1");
				Tick.In("Load.2");
				SetupModules();
				Tick.Out("Load.2");
				Tick.In("Load.3");
				CollectDomainComponents();
				Tick.Out("Load.3");
				if(loadTypesInfo) {
					Tick.In("Load.4");
					LoadTypesInfo(modules, typesInfo);
					DoCustomizeTypesInfo(typesInfo);
					Tick.Out("Load.4");
				}
			}
			finally {
				Tracing.Tracer.ResumeFlush();
				Tick.Out("Load");
			}
			isLoaded = true;
			Tracing.Tracer.LogVerboseText("<- ApplicationModulesManager.Load");
		}
		public void Load(ITypesInfo typesInfo) {
			Load(typesInfo, true);
		}
		public void AddModule(ModuleBase module) {
			CheckIsLoaded();
			Guard.ArgumentNotNull(module, "module");
			if(modules.FindModule(module.GetType()) == null) {
				try {
					ReflectionHelper.AddResolvePath(assembliesPath);
					try {
						modules.Add(module);
					}
					finally {
						ReflectionHelper.RemoveResolvePath(assembliesPath);
					}
					OnModuleRegistered(module);
				}
				catch(Exception e) {
					throw new InvalidOperationException(String.Format("Exception occurs while registering {0}\r\n{1}", module.GetType().FullName, e.Message), e);
				}
			}
			else {
				Tracing.Tracer.LogWarning("The {0} module is registered more than once.", module.GetType().Name);
			}
		}
		public virtual ModuleBase AddModule(Type moduleType, Boolean loadModuleDiffs) {
			if(modules.FindModule(moduleType) == null) {
				ModuleBase result;
				ReflectionHelper.AddResolvePath(assembliesPath);
				try {
					if(loadModuleDiffs) {
						result = ModuleFactory.WithResourcesDiffs.CreateModule(moduleType);
					}
					else {
						result = ModuleFactory.WithEmptyDiffs.CreateModule(moduleType);
					}
				}
				finally {
					ReflectionHelper.RemoveResolvePath(assembliesPath);
				}
				AddModule(result);
				return result;
			}
			return null;
		}
		public Boolean AddModule(Type moduleType) {
			return (AddModule(moduleType, true) != null);
		}
		public void AddModuleFromAssemblies(params String[] moduleAssemblyNames) {
			CheckIsLoaded();
			if(moduleAssemblyNames != null) {
				foreach(String assemblyName in moduleAssemblyNames) {
					if(!String.IsNullOrEmpty(assemblyName)) {
						String trimmedAssemblyName = assemblyName.Trim();
						if(!String.IsNullOrEmpty(trimmedAssemblyName)) {
							LoadModulesFromAssembly(trimmedAssemblyName);
						}
					}
				}
			}
		}
		public IEnumerable<Type> DomainComponents {
			get { return allDomainComponents; }
		}
		public ModuleList Modules {
			get { return modules; }
			set { modules = value; }
		}
		public ISecurityStrategyBase Security {
			get { return security; }
			set { security = value; }
		}
		public ControllersManager ControllersManager {
			get { return controllersManager; }
		}
		public string AssembliesPath {
			get { return assembliesPath; }
		}
		public event EventHandler<ModuleRegisteredEventArgs> ModuleRegistered;
		public event EventHandler<CustomizeTypesInfoEventArgs> CustomizeTypesInfo;
		public void Load() {
			Load(XafTypesInfo.Instance, true);
		}
	}
}
