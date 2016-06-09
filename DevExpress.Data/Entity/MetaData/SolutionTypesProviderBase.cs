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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
namespace DevExpress.Entity.ProjectModel {
	public abstract class SolutionTypesProviderBase : ISolutionTypesProvider {
		IProjectTypes activeProjectTypes;
		Dictionary<string, IProjectTypes> solutionProjectTypes;
		public IProjectTypes ActiveProjectTypes {
			get {
				if(activeProjectTypes != null)
					return activeProjectTypes;
				string name = GetActiveProjectAssemblyFullName();
				activeProjectTypes = new ProjectTypes(name, GetSolutionAssemblyFullNames(), GetActiveProjectTypes(), GetResourceOptionsForActiveProject());
				if(name != null)
					solutionProjectTypes[name] = activeProjectTypes;
				return activeProjectTypes;
			}
		}		
		protected abstract IEnumerable<Type> GetActiveProjectTypes();
		protected abstract string GetActiveProjectAssemblyFullName();
		protected abstract IEnumerable<string> GetSolutionAssemblyFullNames();
		protected abstract string[] GetProjectOutputs();
		protected abstract string GetOutputDir();
		protected abstract IDXAssemblyInfo GetAssemblyCore(string assemblyName);
		public virtual void AddReferenceFromFile(string assemblyPath) { }
		public virtual void AddReference(string assemblyName) { }
		public virtual bool IsReferenceExists(string assemblyName) { return true; }
		public SolutionTypesProviderBase() {
			solutionProjectTypes = new Dictionary<string, IProjectTypes>();
			MetaDataServices.Initialize(this);
		}
		ResourceOptions GetResourceOptionsForActiveProject() {
			return new ResourceOptions(true, GetOutputDir());
		}
		public IDXAssemblyInfo GetAssembly(string assemblyName) {
			return GetAssemblyCore(assemblyName);
		}
		public IEnumerable<IDXTypeInfo> GetTypes() {
			foreach(IDXAssemblyInfo info in ActiveProjectTypes)
				foreach(IDXTypeInfo type in info.TypesInfo)
					yield return type;
		}
		public virtual bool IsLocalType(IDXTypeInfo typeInfo) {
			return typeInfo.Assembly.IsProjectAssembly;
		}
		public void Add(IDXTypeInfo typeInfo) {
			if(ActiveProjectTypes.ProjectAssembly != null)
				ActiveProjectTypes.ProjectAssembly.Add(typeInfo);
		}
		public void ClearCache() {
			this.activeProjectTypes = null;
		}
		public bool Contains(IDXTypeInfo typeInfo) {
			return this.ActiveProjectTypes.Where(asm => typeInfo.Assembly.AssemblyFullName == asm.AssemblyFullName).
				 Select<IDXAssemblyInfo, IDXTypeInfo>(asm => asm.TypesInfo.Where(t => t.FullName == typeInfo.FullName).First()).Any();
		}
		public void Remove(IDXTypeInfo typeInfo) {
			if(typeInfo == null || typeInfo.Assembly == null || string.IsNullOrEmpty(typeInfo.Assembly.AssemblyFullName))
				return;
			IDXAssemblyInfo info = this.ActiveProjectTypes.FirstOrDefault(asm => typeInfo.Assembly.AssemblyFullName == asm.AssemblyFullName);
			if(info != null)
				info.Remove(typeInfo);
		}
		public IDXTypeInfo GetTypeInfo(string typeFullName) {
			foreach(IDXAssemblyInfo activeProjectType in this.ActiveProjectTypes)
				foreach(IDXTypeInfo dXTypeInfo in activeProjectType.TypesInfo)
					if(dXTypeInfo != null && dXTypeInfo.FullName == typeFullName)
						return dXTypeInfo;
			return null;
		}
		public IDXTypeInfo FindType(string fullName) {
			return FindType(delegate(IDXTypeInfo type) {
				return type != null && type.FullName == fullName;
			});
		}
		public IDXTypeInfo FindType(Predicate<IDXTypeInfo> predicate) {
			if(predicate == null)
				return null;
			foreach(IDXTypeInfo type in GetTypes())
				if(predicate(type))
					return type;
			return null;
		}
		public IEnumerable<IDXTypeInfo> FindTypes(IDXTypeInfo baseClass, Predicate<IDXTypeInfo> predicate) {
			Type baseType = baseClass != null ? baseClass.ResolveType() : null;
			foreach(IDXAssemblyInfo assemblyInfo in this.ActiveProjectTypes.Assemblies)
				foreach(IDXTypeInfo tyeInfo in assemblyInfo.TypesInfo) {
					if(baseType == null)
						yield return tyeInfo;
					Type type = tyeInfo.ResolveType();
					if(type != null && baseType.IsAssignableFrom(type))
						yield return tyeInfo;
				}
		}
		public IEnumerable<IDXTypeInfo> FindTypes(Predicate<IDXTypeInfo> predicate) {
			if(predicate == null)
				yield break;
			foreach(IDXTypeInfo type in GetTypes())
				if(predicate(type))
					yield return type;
		}
		public virtual string GetAssemblyReferencePath(string projectAssemblyFullName, string referenceName) {
			return null;
		}
		public IProjectTypes GetProjectTypes(string assemblyFullName) {
			if (string.IsNullOrEmpty(assemblyFullName))
				return null;
			IProjectTypes projectTypes;
			if (solutionProjectTypes.TryGetValue(assemblyFullName, out projectTypes))
				return projectTypes;
			projectTypes = GetProjectTypesCore(assemblyFullName);
			if (projectTypes == null)
				return null;
			solutionProjectTypes[assemblyFullName] = projectTypes;
			return projectTypes;
		}
		protected virtual IProjectTypes GetProjectTypesCore(string assemblyFullName) {
			return null;
		}
	}
}
