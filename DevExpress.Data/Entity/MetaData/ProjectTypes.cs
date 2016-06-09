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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Utils;
namespace DevExpress.Entity.ProjectModel {
	public class ProjectTypes : IProjectTypes {
		List<IDXAssemblyInfo> assemblies;
		string projectAssemblyName;
		IEnumerable<string> solutionAssemblyNames;
		public ProjectTypes(string projectAssemblyName, IEnumerable<string> solutionAssemblyNames, IEnumerable<Type> allTypes)
			: this(projectAssemblyName, solutionAssemblyNames, allTypes, null) {
		}
		public ProjectTypes(string projectAssemblyName, IEnumerable<string> solutionAssemblyNames, IEnumerable<Type> allTypes, ResourceOptions options) {
			ResourceOptions = options;
			this.projectAssemblyName = projectAssemblyName;
			if(solutionAssemblyNames != null)
				this.solutionAssemblyNames = solutionAssemblyNames;
			else
				this.solutionAssemblyNames = new string[0];
			Initialize(allTypes);
		}
		void Initialize(IEnumerable<Type> allTypes) {
			assemblies = GetAllAssemblies(allTypes);
		}
		public IEnumerable<IDXTypeInfo> GetTypes(Func<IDXTypeInfo, bool> filter) {
			if(Assemblies == null || filter == null)
				yield break;
			foreach(IDXAssemblyInfo aInfo in Assemblies)
				foreach(IDXTypeInfo typeInfo in aInfo.TypesInfo.Where(filter))
					yield return typeInfo;
		}
		public IEnumerable<IDXAssemblyInfo> GetTypesPerAssembly(Func<IDXTypeInfo, bool> filter) {
			if(Assemblies == null || filter == null)
				yield break;
			foreach(IDXAssemblyInfo aInfo in Assemblies) {
				DXAssemblyInfo ast = new DXAssemblyInfo(aInfo);
				ast.AddRange(aInfo.TypesInfo.Where(filter));
				yield return ast;
			}
		}
		public IDXTypeInfo GetExistingOrCreateNew(Type type) {
			if(type == null)
				return null;
			IDXAssemblyInfo assemblyInfo = Assemblies.FirstOrDefault(ai => string.Compare(ai.AssemblyFullName, type.Assembly.FullName, true) == 0);
			if(assemblyInfo == null)
				return CreateNew(null, type);
			IDXTypeInfo typeInfo = assemblyInfo.GetTypeInfo(type.FullName);
			if(typeInfo != null)
				return typeInfo;
			return CreateNew(assemblyInfo, type);
		}
		IDXTypeInfo CreateNew(IDXAssemblyInfo assemblyInfo, Type type) {
			if(type == null)
				return null;
			if(assemblyInfo == null)
				assemblyInfo = Assemblies.FirstOrDefault(ai => string.Compare(ai.AssemblyFullName, type.Assembly.FullName, true) == 0);
			if(assemblyInfo == null) {
				assemblyInfo = new DXAssemblyInfo(type.Assembly, false, false, ResourceOptions.DefaultOptions, type);
				if(this.assemblies == null)
					this.assemblies = new List<IDXAssemblyInfo>();
				this.assemblies.Add(assemblyInfo);
			}
			return assemblyInfo.GetTypeInfo(type.FullName);
		}
		bool IsProjectAssembly(string fullName) {
			if(String.IsNullOrEmpty(fullName))
				return false;
			return projectAssemblyName == fullName;
		}
		bool IsInSolution(string fullName) {
			return !String.IsNullOrEmpty(fullName) && this.solutionAssemblyNames != null && this.solutionAssemblyNames.Contains(fullName);
		}
		List<IDXAssemblyInfo> GetAllAssemblies(IEnumerable<Type> projectTypes) {
			if(projectTypes == null)
				return new List<IDXAssemblyInfo>();
			Dictionary<string, DXAssemblyInfo> asms = new Dictionary<string, DXAssemblyInfo>();
			DXAssemblyInfo entityFramework = null;
			DXAssemblyInfo entityFrameworkServer = null;
			foreach(Type type in projectTypes) {
				Assembly asm = type.Assembly;
				if(asm == null || (string.IsNullOrEmpty(asm.Location) && !IsEntityFrameworkServer(asm.GetName().Name)))
					continue;
				string asmFullName = asm.FullName;
				if(String.IsNullOrEmpty(asmFullName))
					continue;
				if(asms.ContainsKey(asmFullName)) {
					DXAssemblyInfo ats = asms[asmFullName];
					if(ats != null)
						ats.Add(new DXTypeInfo(type));
					continue;
				}
				bool isProjectAssembly = IsProjectAssembly(asmFullName);
				ResourceOptions options = null;
				if(isProjectAssembly)
					options = ResourceOptions;
				bool isSolutionAssembly = IsInSolution(asmFullName);
				DXAssemblyInfo dxAssemblyInfo = new DXAssemblyInfo(asm, isProjectAssembly, isSolutionAssembly, options, type);
				if(entityFramework == null && IsEntityFramework(dxAssemblyInfo.Name))
					entityFramework = dxAssemblyInfo;
				if(entityFrameworkServer == null && IsEntityFrameworkServer(dxAssemblyInfo.Name))
					entityFrameworkServer = dxAssemblyInfo;
				asms.Add(asmFullName, dxAssemblyInfo);
			}
			return asms.Values.ToList<IDXAssemblyInfo>();
		}
		static bool IsEntityFramework(string assemblyName) {
			return string.Equals(assemblyName, DevExpress.Entity.Model.Constants.EntityFrameworkAssemblyName, StringComparison.OrdinalIgnoreCase);
		}
		static bool IsEntityFrameworkServer(string assemblyName) {
			if(string.Equals(assemblyName, DevExpress.Entity.Model.Constants.SqliteAssemblyName, StringComparison.OrdinalIgnoreCase)) return true;		   
			if(string.Equals(assemblyName, DevExpress.Entity.Model.Constants.EntityFrameworkSqlCeAssemblyName, StringComparison.OrdinalIgnoreCase)) return true;
			if(string.Equals(assemblyName, DevExpress.Entity.Model.Constants.EntityFrameworkSqlClientAssemblyName, StringComparison.OrdinalIgnoreCase)) return true;
			return false;
		}
		public IEnumerable<IDXAssemblyInfo> Assemblies {
			get { return assemblies; }
		}
		public IDXAssemblyInfo ProjectAssembly {
			get {
				IDXAssemblyInfo project = Assemblies.FirstOrDefault(ai => ai.IsProjectAssembly);
				if(project == null && !string.IsNullOrEmpty(this.projectAssemblyName)) {
					DXAssemblyInfo ats = new DXAssemblyInfo(this.projectAssemblyName, true, true, ResourceOptions);
					if(this.assemblies == null)
						this.assemblies = new List<IDXAssemblyInfo>();
					this.assemblies.Add(ats);
					return ats;
				} else
					return project;
			}
		}
		public ResourceOptions ResourceOptions { private set; get; }
		public string ProjectAssemblyName {
			get { return projectAssemblyName; }
		}
		public IEnumerator<IDXAssemblyInfo> GetEnumerator() {
			return Assemblies.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return Assemblies.GetEnumerator();
		}
	}
}
