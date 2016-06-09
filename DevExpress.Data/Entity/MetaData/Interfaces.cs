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
using System.ComponentModel;
using System.Collections.Generic;
namespace DevExpress.Entity.ProjectModel {
	public interface IHasTypesCache {
		void ClearCache();
		bool Contains(IDXTypeInfo typeInfo);
		void Add(IDXTypeInfo typeInfo);
		void Remove(IDXTypeInfo typeInfo);
		IDXTypeInfo GetTypeInfo(string fullName);
	}
	public interface IResourceOptions {
	}
	public interface IDXAssemblyInfo : IHasTypesCache, IHasName {
		string AssemblyFullName { get; }
		IEnumerable<IDXTypeInfo> TypesInfo { get; }
		bool IsProjectAssembly { get; }
		IResourceOptions ResourceOptions { get; }
		bool IsSolutionAssembly { get; }
	}
	public interface IHasName {
		string Name { get; }
	}
	public interface IDXTypeInfo : IHasName {
		string NamespaceName { get; }
		string FullName { get; }
		Type ResolveType();
		IDXAssemblyInfo Assembly { get; set; }
		bool IsSolutionType { get; }
	}
	public interface IDXMemberInfo {
		string Name { get; }
	}
	public interface IDXMethodInfo : IDXMemberInfo {
		IDXTypeInfo ReturnType { get; }
	}
	public interface IDXPropertyInfo : IDXMemberInfo {
		IDXTypeInfo PropertyType { get; }
	}
	public interface IDXFieldInfo : IDXMemberInfo {
		IDXTypeInfo FieldType { get; }
	}
	public interface IProjectTypes : IEnumerable<IDXAssemblyInfo> {
		IEnumerable<IDXAssemblyInfo> Assemblies { get; }
		string ProjectAssemblyName { get; }
		IDXAssemblyInfo ProjectAssembly { get; }
		IEnumerable<IDXAssemblyInfo> GetTypesPerAssembly(Func<IDXTypeInfo, bool> filter);
		IEnumerable<IDXTypeInfo> GetTypes(Func<IDXTypeInfo, bool> filter);
		IDXTypeInfo GetExistingOrCreateNew(Type type);
	}
	public interface ISolutionTypesProvider : IHasTypesCache {
		IProjectTypes ActiveProjectTypes { get; }
		IEnumerable<IDXTypeInfo> GetTypes();
		IDXTypeInfo FindType(string fullName);
		IDXTypeInfo FindType(Predicate<IDXTypeInfo> predicate);
		IEnumerable<IDXTypeInfo> FindTypes(Predicate<IDXTypeInfo> predicate);
		IEnumerable<IDXTypeInfo> FindTypes(IDXTypeInfo baseClass, Predicate<IDXTypeInfo> predicate);
		string GetAssemblyReferencePath(string projectAssemblyFullName, string referenceName);
		IProjectTypes GetProjectTypes(string assemblyFullName);
		IDXAssemblyInfo GetAssembly(string assemblyName);
		void AddReferenceFromFile(string assemblyPath);
		void AddReference(string assemblyName);
		bool IsReferenceExists(string assemblyName);
	}
}
