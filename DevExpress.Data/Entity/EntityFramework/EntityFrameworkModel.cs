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
using System.ComponentModel.Design;
using DevExpress.Entity.ProjectModel;
using DevExpress.Entity.Model.Metadata;
using DevExpress.Utils;
namespace DevExpress.Entity.Model {
	public interface IEntityFrameworkModel {
		IDbContainerInfo GetContainer(IContainerInfo info);
		IEnumerable<IContainerInfo> GetContainersInfo();
		[EditorBrowsable(EditorBrowsableState.Never)]
		IDbContainerInfo GetContainer(string nameOrFullName);
	}
	public enum DbContainerType {
		EntityFramework,
		WCF
	}
	public interface IContainerInfo : IDXTypeInfo {
		DbContainerType ContainerType { get; }
	}
	public interface IDbContainerInfo : IContainerInfo {
		IEntityContainerInfo EntityContainer { get; }
		IEnumerable<IEntitySetInfo> EntitySets { get; }
		IEnumerable<IEntityFunctionInfo> EntityFunctions { get; }
		object MetadataWorkspace { get; } 
		string SourceUrl { get; set; }
	}
	public interface IEntityFunctionInfo {
		string Name { get; }
		FunctionParameterInfo[] Parameters { get; }
		EdmComplexTypePropertyInfo[] ResultTypeProperties { get; }
	}
	public interface IEntityContainerInfo {
		string Name { get; }
		IEnumerable<IEntitySetInfo> EntitySets { get; }
		IEnumerable<IEntityFunctionInfo> EntityFunctions { get; }
	}
	public interface IEntitySetInfo {
		IEntityTypeInfo ElementType { get; }
		bool IsView { get; }
		bool ReadOnly { get; }
		string Name { get; }
		IEntityContainerInfo EntityContainerInfo { get; }
		IEntitySetAttachedInfo AttachedInfo { get; }
	}
	public interface IEntitySetAttachedInfo {
		bool UserSelectedIsReadOnly { get; set; }
	}
	public class ContainerInfo : DXTypeInfo, IContainerInfo {
		public DbContainerType ContainerType { get; private set; }
		public ContainerInfo(Type type, DbContainerType containerType)
			: base(type) {
			ContainerType = containerType;
		}
		public ContainerInfo(IDXTypeInfo type, DbContainerType containerType)
			: this(type.ResolveType(), containerType) {
			Assembly = type.Assembly;
		}
	}
	public abstract class EntityFrameworkModelBase : IEntityFrameworkModel {
		Dictionary<DbContainerType, ContainerBuilder> builders;
		Dictionary<Type, IDbContainerInfo> dbContainers;
		IEnumerable<IContainerInfo> allContainersInfo;
		static string[] assemblyFilters = { "EntityFramework" };
		ContainerBuilder GetBuider(DbContainerType type) {
			if (builders == null)
				builders = new Dictionary<DbContainerType, ContainerBuilder>();
			if (builders.ContainsKey(type))
				return builders[type];
			ContainerBuilder result = GetContainerBuilderCore(type);
			if (result != null)
				builders.Add(type, result);
			return result;
		}
		protected virtual ContainerBuilder GetContainerBuilderCore(DbContainerType dbContainerType) {
			if (dbContainerType == DbContainerType.EntityFramework)
				return new EFContainerBuilderBase();
			return null;
		}
		protected abstract ISolutionTypesProvider TypesProvider { get; }
		public IDbContainerInfo GetContainer(IContainerInfo info) {
			Type type = info.ResolveType();
			if (dbContainers == null)
				dbContainers = new Dictionary<Type, IDbContainerInfo>();
			else if (dbContainers.ContainsKey(type))
			 return dbContainers[type];
			ContainerBuilder buider = GetBuider(info.ContainerType);
			IDbContainerInfo result = buider.Build(info, TypesProvider);
			if (result != null)
				dbContainers.Add(type, result);
			return result;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IDbContainerInfo GetContainer(string nameOrFullName) {
			return GetContainer(nameOrFullName, true);
		}
		public IDbContainerInfo GetContainer(string nameOrFullName, bool returnNullOnError) {
			IEnumerable<IContainerInfo> containers = GetContainersInfo(returnNullOnError);
			if(containers != null)
				foreach(IContainerInfo container in containers)
					if(nameOrFullName.Equals(container.Name) || nameOrFullName.Equals(container.FullName))
						return GetContainer(container);
			return null;
		}
		static bool DbTypeFilter(IDXTypeInfo typeInfo, string baseClassName) {
			if(typeInfo == null)
				return false;
			foreach(string assemblyName in assemblyFilters)
				if(typeInfo.Assembly.AssemblyFullName.StartsWith(assemblyName))
					return false;
			Type type = typeInfo.ResolveType();
			if(type == null)
				return false;
			if(typeInfo.Assembly.IsProjectAssembly)
				return IsContextType(type, baseClassName);
			return type.IsPublic() && IsContextType(type, baseClassName);
		}
		static bool IsContextType(Type type, string baseContextName) {
			if(type == null)
				return false;
			return !type.IsAbstract() && !type.IsSealed() && !type.IsGenericType()
				&& type.IsClass() && !type.IsNested && InheritFromContext(type, baseContextName);
		}
		static bool InheritFromContext(Type type, string baseContextName) {
			return GetBaseContextType(type, baseContextName) != null;
		}
		public static Type GetBaseContextType(Type type, string baseContextName) {
			if(type == null || type.FullName == baseContextName)
				return null;
			while (type != null && !String.IsNullOrEmpty(type.Name) && type.FullName != baseContextName)
				type = type.GetBaseType();
			if(type != null && type.FullName == baseContextName)
				return type;
			return null;
		}
		public static bool IsAtLeastEF6(IContainerInfo containerInfo) {
			if(containerInfo == null || containerInfo.ContainerType != DbContainerType.EntityFramework)
				return false;
			Type dbContext = GetBaseContextType(containerInfo.ResolveType(), Constants.DbContextTypeName);
			return IsAtLeastEF6(dbContext);
		}
		public static bool IsAtLeastEF6(Type dbContextType) {
			if(dbContextType == null)
				return false;
			return dbContextType.GetAssembly().GetName().Version.Major >= 6;
		}
		public static bool IsAtLeastEF7(IContainerInfo containerInfo) {
			if(containerInfo == null || containerInfo.ContainerType != DbContainerType.EntityFramework)
				return false;
			Type dbContext = GetBaseContextType(containerInfo.ResolveType(), Constants.DbContextEF7TypeName);
			return IsAtLeastEF7(dbContext);
		}
		public static bool IsAtLeastEF7(Type dbContextType) {
			if(dbContextType == null)
				return false;
			return dbContextType.GetAssembly().GetName().Version.Major >= 7;
		}
		protected virtual IProjectTypes ProjectTypes { get { return TypesProvider.ActiveProjectTypes; } }
		public IEnumerable<IContainerInfo> GetContainersInfo() {
			return GetContainersInfo(true);
		}
		public IEnumerable<IContainerInfo> GetContainersInfo(bool returnNullOnError) {
			try {
				if(allContainersInfo == null) {
					IEnumerable<IContainerInfo> wcfContexts = ProjectTypes.GetTypes(x => DbTypeFilter(x, Constants.ServiceContextTypeName)).Select<IDXTypeInfo, IContainerInfo>(x => new ContainerInfo(x, DbContainerType.WCF));
					IEnumerable<IContainerInfo> efContexts = ProjectTypes.GetTypes(x => DbTypeFilter(x, Constants.DbContextTypeName)).Select<IDXTypeInfo, IContainerInfo>(x => new ContainerInfo(x, DbContainerType.EntityFramework));
					IEnumerable<IContainerInfo> ef7Contexts = ProjectTypes.GetTypes(x => DbTypeFilter(x, Constants.DbContextEF7TypeName)).Select<IDXTypeInfo, IContainerInfo>(x => new ContainerInfo(x, DbContainerType.EntityFramework));
					allContainersInfo = wcfContexts.Concat(efContexts).Concat(ef7Contexts);
				}
				return allContainersInfo;
			}
			catch {
				if(returnNullOnError)
					return null;
				throw;
			}
		}
	}
}
