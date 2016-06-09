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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.VisualStudio.Shell.Interop;
namespace DevExpress.Utils.Design {
	public enum LanguageId {
		Unknown,
		CSharp,
		VB
	}
	public static class VSVersions {
		public static readonly Version VS2010 = new Version(10, 0);
		public static readonly Version VS2012 = new Version(11, 0);
		public static readonly Version VS2013 = new Version(12, 0);
		public static readonly Version VS2015 = new Version(14, 0);
		[CLSCompliant(false)]
		public static Version GetVersion(EnvDTE._DTE dte) {
			Guard.ArgumentNotNull(dte, "dte");
			return new Version(dte.Version);
		}
		public static Version GetVersion(IServiceProvider serviceProvider) {
			return GetVersion(serviceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE._DTE);
		}
	}
	[CLSCompliant(false)]
	public class DTEHelper {
		[DllImport("ole32.dll")]
		static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);
		static string[] vsProgIds = new string[] { 
			"VisualStudio.DTE.10.0", 
			"VisualStudio.DTE.11.0", 
			"VisualStudio.DTE.12.0",
			"VisualStudio.DTE.14.0",
			"WDExpress.DTE.10.0", 
			"WDExpress.DTE.11.0", 
			"WDExpress.DTE.12.0",
		};
		const string VSProcessName = "devenv";
		static string[] VSProcessNames = new string[] { VSProcessName, "WDExpress", "VCSExpress", "VBExpress" };
		static EnvDTE.DTE dte;
		static string GetFilePath(string fileName) {
			EnvDTE.ProjectItem prjItem = GetFileByName(fileName);
			if(prjItem == null || prjItem.Kind != EnvDTE.Constants.vsProjectItemKindPhysicalFile)
				return null;
			return prjItem.FileNames[0];
		}
		static EnvDTE.ProjectItem GetFileByName(string fileName) {
			try {
				EnvDTE.Project prj = GetActiveProject();
				foreach(EnvDTE.ProjectItem pi in prj.ProjectItems) {
					EnvDTE.ProjectItem result = GetFileByName(pi, fileName);
					if(result != null)
						return result;
				}
			}
			catch { }
			return null;
		}
		static EnvDTE.ProjectItem GetFileByName(EnvDTE.ProjectItem prjItem, string fileName) {
			try {
				if(prjItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile) {
					if(prjItem.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase))
						return prjItem;
					string fullPath = prjItem.FileNames[0];
					if(!String.IsNullOrEmpty(fullPath) && fullPath.Equals(fileName, StringComparison.OrdinalIgnoreCase))
						return prjItem;
				}
				EnvDTE.ProjectItems prjItems = prjItem.ProjectItems;
				if(prjItems == null)
					return null;
				foreach(EnvDTE.ProjectItem pItem in prjItems) {
					EnvDTE.ProjectItem result = GetFileByName(pItem, fileName);
					if(result != null)
						return result;
				}
			}
			catch { }
			return null;
		}
		public static string GetExistingFilePathForActiveProject(string fileName) {
			try {
				return GetFilePath(fileName);
			}
			catch { }
			return null;
		}
		public static string GetCombinedFilePathForActiveProject(string fileName) {
			try {
				EnvDTE.Project prj = GetActiveProject();
				if(prj == null)
					return null;
				return Path.Combine(Path.GetDirectoryName(prj.FullName), fileName);
			}
			catch { }
			return null;
		}
		public static string GetFilePathForActiveProject(string fileName) {
			try {
				string filePath = GetExistingFilePathForActiveProject(fileName);
				if(!String.IsNullOrEmpty(filePath))
					return filePath;
				return GetCombinedFilePathForActiveProject(fileName);
			}
			catch { }
			return null;
		}
		public static bool HasFile(string fileName) {
			return GetFileByName(fileName) != null;
		}
		public static bool AddFileToActiveProject(string filePath) {
			if(HasFile(filePath))
				return false;
			try {
				EnvDTE.Project project = GetActiveProject();
				if(project == null)
					return false;
				project.ProjectItems.AddFromFileCopy(filePath);
			}
			catch {
				return false;
			}
			return true;
		}
		public static bool IsVS2012 {
			get {
				Version dteVersion = new Version(GetCurrentDTE().Version);
				return dteVersion >= VSVersions.VS2012;
			}
		}
		public static LanguageId GetActiveLanguageId() {
			return GetLanguageId(GetActiveProject());
		}
		public static LanguageId GetLanguageId(EnvDTE.Project project) {
			LanguageId languageId = GetLanguageIdFromProjectKind(project);
			if(languageId != LanguageId.Unknown)
				return languageId;
			return GetLanguageIdFromCodeModelLanguage(project);
		}
		static LanguageId GetLanguageIdFromCodeModelLanguage(EnvDTE.Project project) {
			if(project == null || project.CodeModel == null)
				return LanguageId.Unknown;
			LanguageId languageId = LanguageId.Unknown;
			try {
				string codeModelLanguage = project.CodeModel.Language;
				switch(codeModelLanguage) {
					case EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp:
						languageId = LanguageId.CSharp;
						break;
					case EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB:
						languageId = LanguageId.VB;
						break;
				}
			}
			catch { languageId = LanguageId.Unknown; }
			return languageId;
		}
		static LanguageId GetLanguageIdFromProjectKind(EnvDTE.Project project) {
			if(project == null)
				return LanguageId.Unknown;
			LanguageId languageId = LanguageId.Unknown;
			try {
				string kind = project.Kind;
				if(CheckKind(kind, VSLangProj.PrjKind.prjKindCSharpProject))
					languageId = LanguageId.CSharp;
				else if(CheckKind(kind, VSLangProj.PrjKind.prjKindVBProject))
					languageId = LanguageId.VB;
			}
			catch { languageId = LanguageId.Unknown; }
			return languageId;
		}
		public static bool AddAssemblyToReferences(AssemblyName assemblyName) {
			return AddAssemblyToReferences(assemblyName.Name);
		}
		public static bool AddAssemblyToReferences(string assemblyName) {
			if(string.IsNullOrEmpty(assemblyName))
				return false;
			var currentProject = DTEHelper.GetCurrentProject();
			var vsProjectObject = currentProject.Object as VSLangProj.VSProject;
			if(vsProjectObject == null) return false;
			var reference = vsProjectObject.References.Find(assemblyName);
			if(reference == null) {
				try {
					reference = vsProjectObject.References.Add(assemblyName);
					if(reference != null)
						reference.CopyLocal = false;
				}
				catch(Exception) {
					return false;
				}
			}
			return true;
		}
		public static string GetPropertyValue(EnvDTE.Project project, string name) {
			return (project != null) ? GetPropertyValue(project.Properties, name) : string.Empty;
		}
		public static string GetPropertyValue(EnvDTE.ProjectItem projectItem, string name) {
			return (projectItem != null) ? GetPropertyValue(projectItem.Properties, name) : string.Empty;
		}
		static string GetPropertyValue(EnvDTE.Properties properties, string name) {
			try {
				return (string)properties.Item(name).Value;
			}
			catch { return string.Empty; }
		}
		public static EnvDTE.DTE GetCurrentDTE() {
			return GetDTEFromROT(GetCorrectProcessId());
		}
		public static EnvDTE.DTE GetDTE() {
			using(new MessageFilter())
				return GetCurrentDTE();
		}
		public static EnvDTE.Project GetCurrentProject() {
			using(new MessageFilter())
				return GetCurrentProject(GetCurrentDTE());
		}
		public static EnvDTE.Project GetCurrentProject(EnvDTE.DTE dte) {
			if(dte == null) return null;
			using(new MessageFilter()) {
				try {
					if(dte.ActiveDocument != null && dte.ActiveDocument.ProjectItem != null)
						return dte.ActiveDocument.ProjectItem.ContainingProject;
				}
				catch { }
				return GetActiveProject(dte);
			}
		}
		public static EnvDTE.Project GetActiveProject() {
			using(new MessageFilter())
				return GetActiveProject(GetCurrentDTE());
		}
		public static EnvDTE.Project GetActiveProject(EnvDTE.DTE dte) {
			using(new MessageFilter()) {
				if(dte.ActiveSolutionProjects != null) {
					object[] activeSolutionProjects = dte.ActiveSolutionProjects as object[];
					if(activeSolutionProjects != null && activeSolutionProjects.Length > 0)
						return activeSolutionProjects[0] as EnvDTE.Project;
				}
				return null;
			}
		}
		public static List<string> GetCurrentProjectReferencesNames() {
			return GetProjectReferencesNames(GetCurrentProject());
		}
		public static List<string> GetProjectReferencesNames(EnvDTE.Project project) {
			VSLangProj.VSProject vsp = project.Object as VSLangProj.VSProject;
			List<string> references = new List<string>();
			foreach(VSLangProj.Reference r in vsp.References) {
				references.Add(r.Name);
			}
			return references;
		}
		public static bool IsVisualStudioExpressVersion {
			get {
				Process process = Process.GetCurrentProcess();
				return !string.Equals(process.ProcessName, VSProcessName, StringComparison.OrdinalIgnoreCase);
			}
		}
		internal static int GetCorrectProcessId() {
			using(Process currentProc = Process.GetCurrentProcess()) {
				if(Array.Exists(VSProcessNames, currentProc.ProcessName.Contains))
					return currentProc.Id;
				using(Process parentProc = ProcessExtensions.GetParentProcess(currentProc)) {
					if(Array.Exists(VSProcessNames, parentProc.ProcessName.Contains))
						return parentProc.Id;
					return ProcessExtensions.GetParentProcess(parentProc).Id;
				}
			}
		}
		internal static EnvDTE.DTE GetDTEFromROT(int processId) {
			if(dte == null)
				dte = GetDTEByItemMoniker(processId);
			if(dte == null)
				dte = GetDTEBySolutionPathMoniker();
			return dte;
		}
		static EnvDTE.DTE GetDTEByItemMoniker(int processId) {
			string[] progIds = GetProgIdsByProcessId(processId);
			return (EnvDTE.DTE)GetRunningObjectFromROT((name, moniker) => IsVisualStudioItemMoniker(name, progIds, moniker));
		}
		static EnvDTE.DTE GetDTEBySolutionPathMoniker() {
			var runningObj = GetRunningObjectFromROT((name, moniker) => IsVisualStudioSolutionPathMoniker(name, moniker));
			try {
				EnvDTE.Solution solution = (EnvDTE.Solution)runningObj;
				if(solution != null)
					return solution.DTE;
			}
			catch { }
			return null;
		}
		static object GetRunningObjectFromROT(Func<string, IMoniker, bool> resolveRORoutine) {
			object runningObj = null;
			IBindCtx ctx = null;
			IRunningObjectTable runningObjectTable = null;
			IEnumMoniker monikers = null;
			try {
				CreateBindCtx(0, out ctx);
				ctx.GetRunningObjectTable(out runningObjectTable);
				runningObjectTable.EnumRunning(out monikers);
				IntPtr numberFetched = IntPtr.Zero;
				IMoniker[] monikersList = new IMoniker[1];
				while(monikers.Next(1, monikersList, numberFetched) == 0) {
					var runningObjectMoniker = monikersList[0];
					string name = null;
					try {
						if(runningObjectMoniker != null)
							runningObjectMoniker.GetDisplayName(ctx, null, out name);
					}
					catch(UnauthorizedAccessException) { }
					if(!string.IsNullOrEmpty(name) && resolveRORoutine(name, runningObjectMoniker)) {
						runningObjectTable.GetObject(runningObjectMoniker, out runningObj);
						break;
					}
				}
			}
			finally { ReleaseObjects(ctx, runningObjectTable, monikers); }
			return runningObj;
		}
		static MKSYS GetMonikerClass(IMoniker moniker) {
			int mkSYS = 0;
			if(moniker != null && moniker.IsSystemMoniker(out mkSYS) == 0)
				return (MKSYS)mkSYS;
			return MKSYS.MKSYS_NONE;
		}
		static bool IsVisualStudioSolutionPathMoniker(string name, IMoniker m) {
			return Path.GetExtension(name) == ".sln" && GetMonikerClass(m) == MKSYS.MKSYS_FILEMONIKER;
		}
		static bool IsVisualStudioItemMoniker(string name, string[] progIds, IMoniker m) {
			foreach(string progId in progIds) {
				if(string.Equals(name, progId, StringComparison.Ordinal))
					return GetMonikerClass(m) == MKSYS.MKSYS_ITEMMONIKER;
			}
			return false;
		}
		static string[] GetProgIdsByProcessId(int processId) {
			string[] result = new string[vsProgIds.Length];
			for(int i = 0; i < result.Length; i++)
				result[i] = String.Format("!{0}:{1}", vsProgIds[i], processId);
			return result;
		}
		static void ReleaseObjects(IBindCtx bindCtx, IRunningObjectTable rot, IEnumMoniker enumMonikers) {
			if(enumMonikers != null)
				Marshal.ReleaseComObject(enumMonikers);
			if(rot != null)
				Marshal.ReleaseComObject(rot);
			if(bindCtx != null)
				Marshal.ReleaseComObject(bindCtx);
		}
		public static Microsoft.VisualStudio.OLE.Interop.IServiceProvider GetServiceProvider(EnvDTE.DTE dte) {
			return dte as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
		}
		public static Microsoft.VisualStudio.OLE.Interop.IServiceProvider GetServiceProvider() {
			return GetServiceProvider(GetCurrentDTE());
		}
		public static object QueryService(Guid serviceId, Guid interfaceId) {
			return QueryService(GetCurrentDTE(), serviceId, interfaceId);
		}
		public static object QueryService(EnvDTE.DTE dte, Guid serviceId, Guid interfaceId) {
			Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider = GetServiceProvider(dte);
			if(serviceProvider == null)
				return null;
			IntPtr pvObject;
			if(serviceProvider.QueryService(ref serviceId, ref interfaceId, out pvObject) != 0 || pvObject == IntPtr.Zero)
				return null;
			try {
				return Marshal.GetObjectForIUnknown(pvObject);
			}
			finally { Marshal.Release(pvObject); }
		}
		public static TInterface Query<TService, TInterface>()
			where TService : class
			where TInterface : class {
			return QueryService(typeof(TService).GUID, typeof(TInterface).GUID) as TInterface;
		}
		public static TInterface Query<TInterface>(Guid serviceId) where TInterface : class {
			return QueryService(serviceId, typeof(TInterface).GUID) as TInterface;
		}
		public static object Query(Type TInterface, Guid serviceId) {
			return QueryService(serviceId, TInterface.GUID);
		}
		public static TInterface Query<TInterface>(EnvDTE.DTE dte) where TInterface : class {
			return Query<TInterface>(dte, typeof(TInterface).GUID);
		}
		public static TInterface Query<TInterface>(EnvDTE.DTE dte, Guid serviceId) where TInterface : class {
			return QueryService(dte, serviceId, typeof(TInterface).GUID) as TInterface;
		}
		static bool IsValidSubProject(EnvDTE.ProjectItem project) {
			if(project == null)
				return false;
			try {
				if(project.SubProject == null)
					return false;
				return IsValidProject(project.SubProject);
			}
			catch(AccessViolationException) { return false; }
		}
		static bool CheckKind(string kind, string expected) {
			if(String.IsNullOrEmpty(kind) || String.IsNullOrEmpty(expected))
				return false;
			return String.Compare(kind, expected, true) == 0;
		}
		static bool IsInValidProjectKind(string projectKind) {
			return
			  CheckKind(projectKind, EnvDTE.Constants.vsProjectKindSolutionItems) ||
			  CheckKind(projectKind, EnvDTE.Constants.vsProjectKindUnmodeled) ||
			  CheckKind(projectKind, EnvDTE.Constants.vsProjectsKindSolution) ||
			  CheckKind(projectKind, EnvDTE.Constants.vsProjectItemKindMisc) ||
			  CheckKind(projectKind, EnvDTE.Constants.vsProjectItemKindPhysicalFile) ||
			  CheckKind(projectKind, EnvDTE.Constants.vsProjectItemKindPhysicalFolder) ||
			  CheckKind(projectKind, EnvDTE.Constants.vsProjectItemKindSolutionItems) ||
			  CheckKind(projectKind, EnvDTE.Constants.vsProjectItemKindVirtualFolder) ||
			  CheckKind(projectKind, EnvDTE.Constants.vsProjectItemsKindMisc) ||
			  CheckKind(projectKind, EnvDTE.Constants.vsProjectItemsKindSolutionItems);
		}
		public static bool IsValidProject(string projectKind) {
			return !IsInValidProjectKind(projectKind);
		}
		public static bool IsValidProject(EnvDTE.Project project) {
			if(project == null)
				return false;
			return IsValidProject(project.Kind);
		}
		public static bool IsPhysicalFolder(string projectItemKind) {
			return CheckKind(projectItemKind, EnvDTE.Constants.vsProjectItemKindPhysicalFolder);
		}
		static void GetProjectsRecursiveFromProjects(List<EnvDTE.Project> result, EnvDTE.Projects projects) {
			if(projects == null || projects.Count == 0)
				return;
			int count = projects.Count;
			for(int i = 1; i <= count; i++) {
				EnvDTE.Project project = projects.Item(i);
				if(project == null)
					continue;
				if(IsValidProject(project))
					result.Add(project);
				else
					GetProjectsRecursiveFromProjectItems(result, project.ProjectItems);
			}
		}
		static EnvDTE.Project[] GetProjectsRecursiveFromSolution(EnvDTE.Solution solution) {
			if(solution == null)
				return new EnvDTE.Project[0];
			EnvDTE.Projects projects = solution.Projects;
			List<EnvDTE.Project> result = new List<EnvDTE.Project>();
			GetProjectsRecursiveFromProjects(result, projects);
			return result.ToArray();
		}
		public static EnvDTE.Project[] GetProjects(EnvDTE.DTE dte) {
			return (dte != null) ? GetProjects(dte.Solution) : new EnvDTE.Project[0];
		}
		public static EnvDTE.Project[] GetProjects(EnvDTE.Solution solution) {
			return GetProjectsRecursiveFromSolution(solution);
		}
		public static IVsHierarchy GetVsHierarchy(EnvDTE.DTE dte, EnvDTE.Project project) {
			IVsHierarchy hierarchy = null;
			IVsSolution solution = Query<IVsSolution>(dte, typeof(SVsSolution).GUID);
			if(solution != null)
				solution.GetProjectOfUniqueName(project.FullName, out hierarchy);
			return hierarchy;
		}
		public static IVsHierarchy GetVsHierarchy(EnvDTE.Project project) {
			return GetVsHierarchy(project.DTE, project);
		}
		public static IVsUIShell GetVsUIShell() {
			using(new MessageFilter()) {
				return Query<IVsUIShell>(typeof(SVsUIShell).GUID);
			}
		}
		static void GetProjectsRecursiveFromProjectItems(List<EnvDTE.Project> result, EnvDTE.ProjectItems projects) {
			if(projects == null || projects.Count == 0)
				return;
			int count = projects.Count;
			for(int i = 1; i <= count; i++) {
				EnvDTE.ProjectItem projectItem = projects.Item(i);
				if(projectItem == null)
					continue;
				if(IsValidSubProject(projectItem))
					result.Add(projectItem.SubProject);
				else {
					if(projectItem.ProjectItems == null || projectItem.ProjectItems.Count == 0)
						GetProjectsRecursiveFromProject(result, projectItem.SubProject);
					else
						GetProjectsRecursiveFromProjectItems(result, projectItem.ProjectItems);
				}
			}
		}
		static void GetProjectsRecursiveFromProject(List<EnvDTE.Project> result, EnvDTE.Project project) {
			if(project == null)
				return;
			GetProjectsRecursiveFromProjectItems(result, project.ProjectItems);
		}
		static class ProcessExtensions { 
			internal static Process GetParentProcess(Process process) {
				int id = GetParentProcessID(process.Handle);
				return Process.GetProcessById(id);
			}
			[StructLayout(LayoutKind.Sequential)]
			struct PROCESS_BASIC_INFORMATION {
				internal IntPtr Reserved1;
				internal IntPtr PebBaseAddress;
				internal IntPtr Reserved2_0;
				internal IntPtr Reserved2_1;
				internal IntPtr UniqueProcessId;
				internal IntPtr InheritedFromUniqueProcessId;
			}
			static class UnsafeNT {
				[DllImport("ntdll.dll")]
				internal static extern int NtQueryInformationProcess(
						[In]  IntPtr processHandle,
						[In]  int processInformationClass,
						[In, Out] ref PROCESS_BASIC_INFORMATION processInformation,
						[In]  int processInformationLength,
						[Out, Optional] out int returnLength
					);
			}
			[System.Security.SecuritySafeCritical]
			static int GetParentProcessID(IntPtr handle) {
				PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();
				int returnLength;
				int ntStatus = UnsafeNT.NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
				if(ntStatus != 0)
					throw new System.ComponentModel.Win32Exception(ntStatus);
				try { return unchecked(pbi.InheritedFromUniqueProcessId.ToInt32()); }
				catch { return 0; }
			}
		}
		enum MKSYS : int {
			MKSYS_NONE = 0,
			MKSYS_GENERICCOMPOSITE = 1,
			MKSYS_FILEMONIKER = 2,
			MKSYS_ANTIMONIKER = 3,
			MKSYS_ITEMMONIKER = 4,
			MKSYS_POINTERMONIKER = 5,
			MKSYS_CLASSMONIKER = 7,
			MKSYS_OBJREFMONIKER = 8,
			MKSYS_SESSIONMONIKER = 9,
			MKSYS_LUAMONIKER = 10
		}
	}
	#region Workaround for COMException: Call was rejected by callee in VS2012
	public class MessageFilter : IOleMessageFilter, IDisposable {
		IOleMessageFilter oldFilter = null;
		const int SERVERCALL_ISHANDLED = 0;
		const int PENDINGMSG_WAITNOPROCESS = 2;
		const int SERVERCALL_RETRYLATER = 2;
		const int SERVERCALL_REJECTED = -1;
		public MessageFilter() {
			int hr = CoRegisterMessageFilter(this, out oldFilter);
		}
		int IOleMessageFilter.HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo) {
			return SERVERCALL_ISHANDLED;
		}
		int IOleMessageFilter.RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType) {
			if(dwRejectType == SERVERCALL_RETRYLATER) {
				return 99;
			}
			return SERVERCALL_REJECTED;
		}
		int IOleMessageFilter.MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType) {
			return PENDINGMSG_WAITNOPROCESS;
		}
		public void Dispose() {
			IOleMessageFilter filter;
			int hr = CoRegisterMessageFilter(oldFilter, out filter);
		}
		[DllImport("Ole32.dll")]
		static extern int CoRegisterMessageFilter(IOleMessageFilter newFilter, out IOleMessageFilter oldFilter);
	}
	[ComImport(), Guid("00000016-0000-0000-C000-000000000046"),
	InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	interface IOleMessageFilter {
		[PreserveSig]
		int HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo);
		[PreserveSig]
		int RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType);
		[PreserveSig]
		int MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType);
	}
	#endregion
}
