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
using EnvDTE;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Interop;
using DevExpress.Utils.Design;
using System.Xml.Linq;
using DevExpress.Design.UI;
using DevExpress.Entity.ProjectModel;
using DevExpress.Data.Entity;
using DevExpress.Utils;
namespace DevExpress.Design.Entity{
	public class DTESolutionTypesProvider : SolutionTypesProviderBase {
		IServiceProvider serviceProvider;
		public DTESolutionTypesProvider()
			: base() { 
		}
		public override void AddReferenceFromFile(string assemblyPath) {
			ProjectHelper.AddReferenceFromFile(ActiveProject, assemblyPath);
		}
		public override void AddReference(string assemblyName) {
			ProjectHelper.AddReference(ActiveProject, assemblyName);
		}
		public override bool IsReferenceExists(string assemblyName) {
			return ProjectHelper.IsReferenceExists(ActiveProject, assemblyName);
		}
		protected override IEnumerable<Type> GetActiveProjectTypes() {
			return GetTypes(ActiveProject);
		}
		protected override IDXAssemblyInfo GetAssemblyCore(string assemblyName) {
			return ActiveProjectTypes.Assemblies.FirstOrDefault(x => x.Name == assemblyName);
		}
		protected virtual void LogException(Exception ex) {
		}
		protected IEnumerable<Type> GetTypesCore(Project project, bool excludeGlobalTypes) {
			Type baseType = typeof(object);
			ICollection types = null;
			try {
				types = Platform.GetDTETypes(DTEHelper.GetCurrentDTE(), project, typeof(object), excludeGlobalTypes);
			} catch(Exception ex) {
				LogException(ex);
			}
			if(types == null)
				yield break;
			foreach(Type type in types)
				if(TypesFilterHelper.ShouldIncludeTypesFromAssembly(type.Assembly.FullName))
					yield return type;
		}
		[CLSCompliant(false)]
		protected virtual IEnumerable<Type> GetTypes(Project project) {		  
			IEnumerable<Type> types = GetTypesCore(project, false);
			return FilterTypes(types, project, false);
		}
		Assembly GetCorrectAssembly(VSLangProj.Reference reference, Assembly referencedAssembly) {
			try {
				AssemblyName assemblyName = null;
				string file = reference.Path;
				if(string.IsNullOrEmpty(file) || !File.Exists(file)) {
					if(referencedAssembly != null) {
						assemblyName = referencedAssembly.GetName();
						file = referencedAssembly.Location;
					}
				}
				else
					assemblyName = AssemblyName.GetAssemblyName(file);
				if(assemblyName == null || !TypesFilterHelper.ShouldIncludeTypesFromAssembly(assemblyName.FullName))
					return null;
				IEnumerable<Assembly> loaded = AppDomain.CurrentDomain.GetAssemblies();
				string fullName = assemblyName.FullName;
				string baseDir = AppDomain.CurrentDomain.BaseDirectory;
				string fileName = Path.GetFileName(file);
				string candidate = Path.Combine(baseDir, fileName);
				Assembly loadedAssembly = loaded.Where(x => x.GetName().FullName == fullName)
					.FirstOrDefault(x => string.Compare(x.Location, candidate, StringComparison.OrdinalIgnoreCase) == 0);
				if(loadedAssembly != null)
					return loadedAssembly;
				if(File.Exists(candidate)) {
					AssemblyName candidateName = AssemblyName.GetAssemblyName(candidate);
					if(candidateName.FullName == fullName)
						file = candidate;
				}
				if(referencedAssembly != null && string.Compare(referencedAssembly.Location, file, StringComparison.OrdinalIgnoreCase) == 0)
					return null; 
				try {
					byte[] rawAssembly = File.ReadAllBytes(file);
					return Assembly.Load(rawAssembly);
				}
				catch { }
			}
			catch { }
			return null;
		}
		Assembly GetLoaded(IEnumerable<Assembly> assemblies, VSLangProj.Reference reference) {
			string name = AssemblyHelper.GetPartialName(reference.Name);
			return assemblies.FirstOrDefault(x => x.FullName == name || AssemblyHelper.GetPartialName(x.FullName) == name);
		}
		static IEnumerable<VSLangProj.Reference> GetProjectReferences(EnvDTE.Project project) {
			VSLangProj.VSProject vsp = project.Object as VSLangProj.VSProject;
			foreach(VSLangProj.Reference r in vsp.References)
				yield return r;
		}
		protected IEnumerable<Type> FilterTypes(IEnumerable<Type> types, Project project, bool excludeDevExpress) {
			Dictionary<Assembly, List<Type>> collectedAssemblies = GetCollectedAssemblies(types);
			IEnumerable<VSLangProj.Reference> references = GetProjectReferences(project);
			byte[] dxkey = GetType().Assembly.GetName().GetPublicKeyToken();
			foreach(VSLangProj.Reference reference in references) {
				Assembly referencedAssembly = GetLoaded(collectedAssemblies.Keys, reference);
				referencedAssembly = GetCorrectAssembly(reference, referencedAssembly);
				if(referencedAssembly != null && excludeDevExpress) {
					byte[] key = referencedAssembly.GetName().GetPublicKeyToken();
					if(dxkey.SequenceEqual<byte>(key)) continue;
				}
				IEnumerable<Type> projectReferenceTypes = GetExportedTypes(referencedAssembly);
				if(projectReferenceTypes != null && projectReferenceTypes.Any())
					collectedAssemblies[projectReferenceTypes.First().Assembly] = projectReferenceTypes.ToList();
			}
			List<Type> result = new List<Type>();
			foreach(List<Type> item in collectedAssemblies.Values)
				result.AddRange(item);
			return result;
		}
		Dictionary<Assembly, List<Type>> GetCollectedAssemblies(IEnumerable<Type> types) {
			Dictionary<Assembly, List<Type>> collectedAssemblies = new Dictionary<Assembly, List<Type>>();
			foreach(Type type in types) {
				List<Type> typeList;
				collectedAssemblies.TryGetValue(type.Assembly, out typeList);
				if(typeList == null) {
					typeList = new List<Type>();
					collectedAssemblies[type.Assembly] = typeList;
				}
				if(!typeList.Contains(type))
					typeList.Add(type);
			}
			return collectedAssemblies;
		}
		IEnumerable<Type> GetExportedTypes(Assembly assembly) {
			try {
				if(assembly != null)
					return assembly.GetExportedTypes();
			} catch { }
			return null; 
		}			   
		Assembly FindShellAssembly() {
			foreach(Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) 
				if(asm.GetName().Name == "Microsoft.VisualStudio.Shell.Design")
					return asm;			
			return null;
		}
		protected override string GetActiveProjectAssemblyFullName() {
			return GetProjectAssemblyFullName(ActiveProject);
		}
		protected override string[] GetProjectOutputs() {
			return GetProjectOutputs(ActiveProject);
		}
		string[] GetProjectOutputs(Project project) {
			if(project == null)
				return null;
			IVsHierarchy vsHierarchy = DTEHelper.GetVsHierarchy(project);
			if(vsHierarchy == null)
				return null;
			string[] projectOutputs = ProjectHelper.GetProjectOutputsFast(ServiceProvider, vsHierarchy);
			if(projectOutputs == null)
				projectOutputs = ProjectHelper.GetProjectOutputsSlow(project);
			return projectOutputs;
		}
		string GetProjectAssemblyFullName(Project project) {
			if(project == null)
				return null;
			try {
				string[] projectOutputs = GetProjectOutputs(project);
				if(projectOutputs == null)
					return null;
				foreach(string item in projectOutputs)
					if(!string.IsNullOrEmpty(item) && File.Exists(item)) {
						AssemblyName assemblyName = AssemblyName.GetAssemblyName(item);
						if(assemblyName == null)
							continue;
						return assemblyName.FullName;
					}
			}
			catch { }
			return null;
		}
		public override string GetAssemblyReferencePath(string projectAssemblyFullName, string referenceName) {
			if(string.IsNullOrEmpty(referenceName))
				return null;
			try {
				if(File.Exists(referenceName))
					return referenceName;
				Project project = GetProject(projectAssemblyFullName);
				if(project == null)
					return null;
				VSLangProj.VSProject vsProject = project.Object as VSLangProj.VSProject;
				if(vsProject == null)
					return null;
				VSLangProj.Reference result = vsProject.References.Find(referenceName);
				if(result == null)
					return null;
				return result.Path;
			}
			catch(Exception ex) {
				LogException(ex);
				return null;
			}
		}
		Project GetProject(string projectAssemblyFullName) { 
			if(string.IsNullOrEmpty(projectAssemblyFullName))
				return null;			
			DTE dte = DTEHelper.GetCurrentDTE();
			if(dte == null)
				return null;
			Project[] projects = DTEHelper.GetProjects(dte.Solution);
			if(projects.Length == 0)
				return null;
			foreach(Project project in projects) 
				if(GetProjectAssemblyFullName(project) == projectAssemblyFullName)
					return project;
			return null;
		}
		protected override IEnumerable<string> GetSolutionAssemblyFullNames() {
			DTE dte = DTEHelper.GetCurrentDTE();
			Project[] projects = DTEHelper.GetProjects(dte.Solution);
			if(projects.Length == 0)
				return new string[0];
			List<string> result = new List<string>();
			foreach(Project project in projects) {
				string name = GetProjectAssemblyFullName(project);
				if(!string.IsNullOrEmpty(name) && !result.Contains(name))
					result.Add(name);
			}
			return result;
		}
		protected override string GetOutputDir() {
			return GetOutputDir(ActiveProject);
		}
		string GetOutputDir(EnvDTE.Project project) {
			if(project == null || project.Properties == null)
				return string.Empty;
			try {
				EnvDTE.ConfigurationManager configurationManager = project.ConfigurationManager;
				if(configurationManager == null)
					return string.Empty;
				EnvDTE.Configuration activeConfiguration = configurationManager.ActiveConfiguration;
				if(activeConfiguration == null)
					return string.Empty;
				EnvDTE.Properties configurationProperties = activeConfiguration.Properties;
				if(configurationProperties == null)
					return string.Empty;
				string outPath = string.Empty;
				try {
					XDocument doc = XDocument.Load(project.FullName);
					XElement outDir = (from xml2 in doc.Descendants()
									   where xml2.Name.LocalName == "OutDir" && !string.IsNullOrEmpty(xml2.Value)
									   select xml2).FirstOrDefault();
					if(outDir != null)
						outPath = outDir.Value;
				}
				catch { }
				if(string.IsNullOrEmpty(outPath) || !Directory.Exists(outPath)) {
					EnvDTE.Property outputPath = configurationProperties.Item("OutputPath");
					if(outputPath == null)
						return string.Empty;
					outPath = (string)outputPath.Value;
				}
				string result = Path.Combine(Path.GetDirectoryName(project.FullName), outPath );
				return new FileInfo(result).FullName;
			}
			catch(Exception ex) {
				LogException(ex);
			}
			return string.Empty;
		}
		IServiceProvider ServiceProvider {
			get {
				if(serviceProvider == null)
					serviceProvider = Platform.CreateShellServiceProvider(DTEHelper.GetCurrentDTE() as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
				return serviceProvider;
			}
		}
		EnvDTE.Project ActiveProject {
			get {
				return DTEHelper.GetActiveProject();
			}
		}
	}
}
