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

namespace DevExpress.Design {
	public enum PlatformCodeName {
		Win,
		Wpf,
		Silverlight,
		ASP,
		Unknown
	}
}
namespace DevExpress.Design.UI {
	using System;
	using System.Collections.Generic;
	using DevExpress.Design.DataAccess;
	public interface IPlatformServiceContainer : IServiceContainer {
		void Register<Service, ServiceProvider>(PlatformCodeName id, Func<ServiceProvider> initializer)
			where ServiceProvider : Service;
		void Register<Service, ServiceProvider>(PlatformCodeName id)
			where ServiceProvider : Service, new();
		Service Resolve<Service>(PlatformCodeName id);
	}
	public sealed class Platform {
		public static IPlatformServiceContainer ServiceContainer {
			get { return PlatformServiceContainer.Instance; }
		}
		sealed class PlatformServiceContainer : ServiceContainer, IPlatformServiceContainer {
			internal static IPlatformServiceContainer Instance = new PlatformServiceContainer();
			static PlatformServiceContainer() { }
			PlatformServiceContainer() : base(null) { }
			static IDictionary<PlatformCodeName, IServiceContainer> platformContainers;
			public void Register<Service, ServiceProvider>(PlatformCodeName id, Func<ServiceProvider> initializer)
				where ServiceProvider : Service {
				GetPlatformContainer(id).Register<Service, ServiceProvider>(initializer);
			}
			public void Register<Service, ServiceProvider>(PlatformCodeName id)
				where ServiceProvider : Service, new() {
				GetPlatformContainer(id).Register<Service, ServiceProvider>();
			}
			static IServiceContainer GetPlatformContainer(PlatformCodeName id) {
				if(platformContainers == null)
					platformContainers = new Dictionary<PlatformCodeName, IServiceContainer>();
				IServiceContainer container;
				if(!platformContainers.TryGetValue(id, out container)) {
					container = new ServiceContainer(null);
					platformContainers[id] = container;
				}
				return container;
			}
			public Service Resolve<Service>(PlatformCodeName id) {
				if(platformContainers != null) {
					IServiceContainer container;
					if(platformContainers.TryGetValue(id, out container))
						return container.Resolve<Service>();
				}
				throw new ResolvePlatformServiceException<Service>(id);
			}
			class ResolvePlatformServiceException<Service> : Exception {
				public ResolvePlatformServiceException(PlatformCodeName codeName)
					: base(typeof(Service).ToString() + " service is not registered for platform " + codeName.ToString()) {
				}
			}
		}
		public static bool IsDesignMode {
			get {
				string currentProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
				return Array.IndexOf(designerProcessNames, currentProcessName) != -1;
			}
		}
		static string[] designerProcessNames = new string[] { "devenv", "VCSExpress", "VBExpress", "WDExpress", "XDesProc" };
		static Version dteVersion;
		static string[] dteProjectAssemblies;
		static Type[] dteTypes;
		static ResolveDTEInfo resolveInfo;
		static System.Threading.ManualResetEvent QuerySyncEvent = new System.Threading.ManualResetEvent(false);
		public static void QueryDTEInfo() {
			QuerySyncEvent.Reset();
			dteVersion = null;
			dteProjectAssemblies = new string[] { };
			dteTypes = new Type[] { };
			resolveInfo = new ResolveDTEInfo(GetDTEProcessID());
			System.Threading.ThreadPool.RegisterWaitForSingleObject(resolveInfo.DTEAvailableEvent, ResolveDTETypes, resolveInfo, -1, true);
			System.Threading.ThreadPool.RegisterWaitForSingleObject(resolveInfo.DTEAvailableEvent, ResolveDTEProjectAssemblies, resolveInfo, -1, true);
			System.Threading.ThreadPool.QueueUserWorkItem(WaitDTEInfoElementsAvailable, resolveInfo);
			System.Threading.ThreadPool.QueueUserWorkItem(ResolveDTE, resolveInfo);
			QuerySyncEvent.Set();
			resolveInfo.DTEInfoAvailable.WaitOne();
		}
		public static void ReleaseDTEInfo() {
			resolveInfo.DTEAvailableEvent.Dispose();
			resolveInfo.DTETypesAvailableEvent.Dispose();
			resolveInfo.DTEProjectAssembliesAvailableEvent.Dispose();
			resolveInfo.DTEInfoAvailable.Dispose();
			resolveInfo = null;
			dteTypes = null;
			dteProjectAssemblies = null;
			QuerySyncEvent.Reset();
		}
		public static bool IsVS2012OrAbove {
			get {
				QuerySyncEvent.WaitOne();
				resolveInfo.DTEAvailableEvent.WaitOne();
				return (dteVersion != null) && dteVersion >= Utils.Design.VSVersions.VS2012;
			}
		}
		public static bool IsVS2013OrAbove {
			get {
				QuerySyncEvent.WaitOne();
				resolveInfo.DTEAvailableEvent.WaitOne();
				return (dteVersion != null) && dteVersion >= Utils.Design.VSVersions.VS2013;
			}
		}
		public static bool IsProjectAssembly(System.Reflection.AssemblyName assemblyName) {
			return IsProjectAssembly(assemblyName.Name);
		}
		public static bool IsProjectAssembly(string assemblyName) {
			QuerySyncEvent.WaitOne();
			resolveInfo.DTEProjectAssembliesAvailableEvent.WaitOne();
			return Array.IndexOf(dteProjectAssemblies, assemblyName) != -1;
		}
#if DEBUGTEST
		static internal bool ForceNoSyncForTests;
#endif
		public static Type[] GetTypes() {
#if DEBUGTEST
			if(!ForceNoSyncForTests) {
#endif
				QuerySyncEvent.WaitOne();
				resolveInfo.DTETypesAvailableEvent.WaitOne();
#if DEBUGTEST
			}
#endif
			return dteTypes;
		}
		public static void Queue(Action<EnvDTE.DTE> action, int delay = 50) {
			resolveInfo.DTEAvailableEvent.WaitOne();
			System.Threading.ThreadPool.QueueUserWorkItem((state) =>
			{
				System.Threading.Thread.Sleep(delay);
				using(new DevExpress.Utils.Design.MessageFilter())
					action((EnvDTE.DTE)state);
			}, resolveInfo.dte);
		}
		class ResolveDTEInfo {
			public ResolveDTEInfo(int pid) {
				this.PID = pid;
				this.DTEAvailableEvent = new System.Threading.ManualResetEvent(false);
				this.DTETypesAvailableEvent = new System.Threading.ManualResetEvent(false);
				this.DTEProjectAssembliesAvailableEvent = new System.Threading.ManualResetEvent(false);
				this.DTEInfoAvailable = new System.Threading.AutoResetEvent(false);
			}
			public EnvDTE.DTE dte;
			public readonly int PID;
			public readonly System.Threading.ManualResetEvent DTEAvailableEvent;
			public readonly System.Threading.ManualResetEvent DTETypesAvailableEvent;
			public readonly System.Threading.ManualResetEvent DTEProjectAssembliesAvailableEvent;
			public readonly System.Threading.AutoResetEvent DTEInfoAvailable;
		}
		static void ResolveDTE(object state) {
			ResolveDTEInfo info = ((ResolveDTEInfo)state);
			try {
				var dte = GetDTE(info.PID);
				if(dte != null) {
					using(new DevExpress.Utils.Design.MessageFilter())
						dteVersion = new Version(dte.Version);
					info.dte = dte;
				}
			}
			finally { info.DTEAvailableEvent.Set(); }
		}
		static void ResolveDTETypes(object state, bool timeout) {
			var dte = ((ResolveDTEInfo)state).dte;
			try {
				List<Type> validTypes = new List<Type>();
				var allTypes = GetDTETypes(dte, typeof(object), true);
				if(allTypes != null) {
					foreach(var t in allTypes) {
						Type type = t as Type;
						if(t != null && type.IsClass &&
								!BaseDataAccessTechnologyTypesProvider.IsStatic(type) &&
								!BaseDataAccessTechnologyTypesProvider.IsRelatedToEvents(type)) {
							validTypes.Add(type);
						}
					}
				}
				dteTypes = validTypes.ToArray();
			}
			finally { ((ResolveDTEInfo)state).DTETypesAvailableEvent.Set(); }
		}
		static void ResolveDTEProjectAssemblies(object state, bool timeout) {
			var dte = ((ResolveDTEInfo)state).dte;
			try {
				EnvDTE.Project[] projects = Utils.Design.DTEHelper.GetProjects(dte);
				dteProjectAssemblies = new string[projects.Length];
				for(int i = 0; i < projects.Length; i++) {
					if(projects[i].Properties == null) continue;
					try {
						object objAssemblyName = projects[i].Properties.Item("AssemblyName").Value;
						if(objAssemblyName != null)
							dteProjectAssemblies[i] = objAssemblyName.ToString();
					}
					catch { }
				}
			}
			finally { ((ResolveDTEInfo)state).DTEProjectAssembliesAvailableEvent.Set(); }
		}
		static void WaitDTEInfoElementsAvailable(object state) {
			System.Threading.WaitHandle[] handles = new System.Threading.WaitHandle[] { 
				((ResolveDTEInfo)state).DTETypesAvailableEvent, 
				((ResolveDTEInfo)state).DTEProjectAssembliesAvailableEvent 
			};
			System.Threading.WaitHandle.WaitAll(handles);
			((ResolveDTEInfo)state).DTEInfoAvailable.Set();
		}
		static void OnDTEInfoAvailable(object state, bool timeout) {
			((ResolveDTEInfo)state).DTEAvailableEvent.Close();
			((ResolveDTEInfo)state).DTETypesAvailableEvent.Close();
			((ResolveDTEInfo)state).DTEProjectAssembliesAvailableEvent.Close();
			((ResolveDTEInfo)state).DTEInfoAvailable.Close();
		}
		static int GetDTEProcessID() {
			return DevExpress.Utils.Design.DTEHelper.GetCorrectProcessId();
		}
		static EnvDTE.DTE GetDTE(int processID) {
			return DevExpress.Utils.Design.DTEHelper.GetDTEFromROT(processID);
		}
		static T GetService<T>(System.IServiceProvider serviceProvider) where T : class {
			return serviceProvider.GetService(typeof(T)) as T;
		}
		static Microsoft.VisualStudio.OLE.Interop.IServiceProvider GetServiceProvider(EnvDTE.DTE dte) {
			return dte as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
		}
		static EnvDTE.Project GetCurrentProject(EnvDTE.DTE dte) {
			if(dte == null) return null;
			try {
				if(dte.ActiveDocument != null && dte.ActiveDocument.ProjectItem != null)
					return dte.ActiveDocument.ProjectItem.ContainingProject;
			}
			catch { }
			if(dte.ActiveSolutionProjects != null) {
				object[] activeSolutionProjects = dte.ActiveSolutionProjects as object[];
				if(activeSolutionProjects != null && activeSolutionProjects.Length > 0)
					return activeSolutionProjects[0] as EnvDTE.Project;
			}
			return null;
		}
		static Microsoft.VisualStudio.Shell.Interop.IVsHierarchy GetVsHierarchy(EnvDTE.DTE dte, EnvDTE.Project project) {
			return DevExpress.Utils.Design.DTEHelper.GetVsHierarchy(dte, project);
		}
		[CLSCompliant(false)]
		public static IServiceProvider CreateShellServiceProvider(Microsoft.VisualStudio.OLE.Interop.IServiceProvider provider) {
			var shellAssembly = GetShellAssembly();
			if(shellAssembly != null) {
				Type serviceProviderType = shellAssembly.GetType("Microsoft.VisualStudio.Shell.ServiceProvider");
				var constructorInfo = serviceProviderType.GetConstructor(new Type[] { typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider) });
				if(constructorInfo != null)
					return constructorInfo.Invoke(new object[] { provider }) as IServiceProvider;
			}
			return null;
		}
		static System.ComponentModel.Design.ITypeDiscoveryService GetTypeDiscoveryService(object dynamicTypesService, Microsoft.VisualStudio.Shell.Interop.IVsHierarchy vsHierarchy) {
			var mInfo = dynamicTypesService.GetType().GetMethod("GetTypeDiscoveryService",
				new Type[] { typeof(Microsoft.VisualStudio.Shell.Interop.IVsHierarchy) });
			if(mInfo != null)
				return mInfo.Invoke(dynamicTypesService, new object[] { vsHierarchy }) as System.ComponentModel.Design.ITypeDiscoveryService;
			return null;
		}
		static Type GetDynamicTypesServiceType() {
			var shellDesignAssembly = GetShellDesignAssembly();
			if(shellDesignAssembly != null)
				return shellDesignAssembly.GetType("Microsoft.VisualStudio.Shell.Design.DynamicTypeService");
			return null;
		}
		public static System.Reflection.Assembly GetShellAssembly() {
			return GetAssembly("Microsoft.VisualStudio.Shell");
		}
		public static System.Reflection.Assembly GetShellDesignAssembly() {
			return GetAssembly("Microsoft.VisualStudio.Shell.Design");
		}
		public static System.Reflection.Assembly GetAssembly(string assemblyName) {
			try {
				var assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach(var assembly in assemblies) {
					if(assembly.GetName().Name == assemblyName)
						return assembly;
				}
			}
			catch { }
			try { return AppDomain.CurrentDomain.Load(assemblyName); }
			catch { }
			return null;
		}
		[CLSCompliant(false)]
		public static System.Collections.ICollection GetDTETypes(EnvDTE.DTE dte, Type baseType, bool excludeGlobalTypes) {
			return GetDTETypes(dte, GetCurrentProject(dte), baseType, excludeGlobalTypes);
		}
		[CLSCompliant(false)]
		public static System.Collections.ICollection GetDTETypes(EnvDTE.DTE dte, EnvDTE.Project project, Type baseType, bool excludeGlobalTypes) {
			var shellServiceProvider = CreateShellServiceProvider(GetServiceProvider(dte));
			var dynamicTypesServiceType = GetDynamicTypesServiceType();
			object dynamicTypesService = (shellServiceProvider != null && dynamicTypesServiceType != null) ?
				shellServiceProvider.GetService(dynamicTypesServiceType) : null;
			if(dynamicTypesService != null) {
				var vsHierarchy = GetVsHierarchy(dte, project);
				var typeDiscoveryService = GetTypeDiscoveryService(dynamicTypesService, vsHierarchy);
				if(typeDiscoveryService != null) {
					try { return typeDiscoveryService.GetTypes(baseType, excludeGlobalTypes); }
					catch { }
				}
			}
			return new Type[] { };
		}
		public static System.Reflection.Assembly[] GetProjectAssemblies() {
#if DEBUGTEST
			if(!ForceNoSyncForTests) {
#endif
				QuerySyncEvent.WaitOne();
				resolveInfo.DTEProjectAssembliesAvailableEvent.WaitOne();
#if DEBUGTEST
			}
#endif
			return GetProjectAssembliesCore();
		}
		static System.Reflection.Assembly[] GetProjectAssembliesCore() {
			System.Reflection.Assembly[] projectAssemblies = new System.Reflection.Assembly[dteProjectAssemblies.Length];
			try {
				var assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach(var assembly in assemblies) {
					int index = Array.IndexOf(dteProjectAssemblies, assembly.GetName().Name);
					if(index == -1) continue;
					projectAssemblies[index] = assembly;
				}
			}
			catch { }
			return projectAssemblies;
		}
	}
}
