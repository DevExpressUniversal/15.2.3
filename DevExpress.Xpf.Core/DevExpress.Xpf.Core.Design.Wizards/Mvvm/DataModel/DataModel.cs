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
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using DevExpress.Design.Mvvm;
using DevExpress.Entity.Model;
using DevExpress.Entity.ProjectModel;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.DataModel {
	public class DataModel : IDataModel {
		private DataModel(UnitOfWorkSourceInfo unitOfWorkSourceInfo, ExpressionHelperInfo commonUtilsInfo) {
			UnitOfWorkSource = unitOfWorkSourceInfo;
			ExpressionHelper = commonUtilsInfo;
			unitOfWorkSourceInfo.ParentDataModel = this;
		}
		public string Name {
			get {
				if(DbContainer == null)
					return "Empty";
				return string.Format("{0} Data Model", DbContainer.Name);
			}
		}
		public IDbContainerInfo DbContainer {
			get { return EntityRepositoriesCore.FirstOrDefault().With(x => x.DbContainerInfo); }
		}
		public IContainerInfo ContainerInfo { get { return DbContainer; } }
		public IEnumerable<IEntitySetInfo> Entities {
			get {
				if(this.UnitOfWorkFactory == null || this.UnitOfWorkFactory.EntitiesUnitOfWork == null)
					yield break;
				IEnumerable<EntityRepositoryInfo> entityRepositories = this.UnitOfWorkFactory.EntitiesUnitOfWork.EntityRepositories;
				if(entityRepositories == null)
					yield break;
				foreach(EntityRepositoryInfo entityRepository in entityRepositories)
					yield return entityRepository.EntitySet;
			}
		}
		public ExpressionHelperInfo ExpressionHelper { get; private set; }
		public UnitOfWorkSourceInfo UnitOfWorkSource { get; private set; }
		public bool IsValid {
			get { return UnitOfWorkSource != null && UnitOfWorkSource.IsValid && DbContainer != null; }
		}
		public UnitOfWorkFactoryInfo UnitOfWorkFactory {
			get {
				if(UnitOfWorkSource == null)
					return null;
				return UnitOfWorkSource.UnitOfWorkFactory;
			}
		}
		EntitiesUnitOfWorkInfo IDataModel.EntitiesUnitOfWork {
			get {
				if(this.UnitOfWorkFactory == null)
					return null;
				return this.UnitOfWorkFactory.EntitiesUnitOfWork;
			}
		}
		IEnumerable<EntityRepositoryInfo> IDataModel.EntityRepositories {
			get {
				return EntityRepositoriesCore;
			}
		}
		IEnumerable<EntityRepositoryInfo> EntityRepositoriesCore {
			get {
				if(this.UnitOfWorkFactory == null || this.UnitOfWorkFactory.EntitiesUnitOfWork == null)
					return null;
				return this.UnitOfWorkFactory.EntitiesUnitOfWork.EntityRepositories;
			}
		}
		public static DataModel Create(Type typeElement, Type expressionHelperType, IServiceContainer serviceContainer) {
			UnitOfWorkSourceInfo unitOfWorkSourceInfo = UnitOfWorkSourceInfo.Create(typeElement, serviceContainer);
			if(unitOfWorkSourceInfo == null || !unitOfWorkSourceInfo.IsValid)
				return null;
			ExpressionHelperInfo expressionHelperInfo = ExpressionHelperInfo.Create(expressionHelperType, serviceContainer);
			if(expressionHelperInfo == null || !expressionHelperInfo.IsValid)
				return null;
			DataModel result = new DataModel(unitOfWorkSourceInfo, expressionHelperInfo);
			if(result.IsValid)
				return result;
			return null;
		}
		public static IDataModel Create(IDbContainerInfo container, IEnumerable<IEntitySetInfo> selectedTables, string namespaceName, string commonNamespaceName, string utilsNamespace) {
			UnitOfWorkSourceInfo unitOfWorkSourceInfo = UnitOfWorkSourceInfo.Create(container, selectedTables, namespaceName, commonNamespaceName);
			if(unitOfWorkSourceInfo == null)
				return null;
			ExpressionHelperInfo expressionHelperInfo = ExpressionHelperInfo.Create(MetaDataServices.SolutionTypesProvider, utilsNamespace);
			if(expressionHelperInfo == null || !expressionHelperInfo.IsValid)
				return null;
			DataModel result = new DataModel(unitOfWorkSourceInfo, expressionHelperInfo);
			if(result.IsValid)
				return result;
			return null;
		}
		bool IDataModel.IsInSolution {
			get {
				return DbContainer.IsSolutionType;
			}
		}
	}
	public class UnitOfWorkSourceInfo : DXTypeInfo {
		public UnitOfWorkSourceInfo(Type type)
			: base(type) {
		}
		public UnitOfWorkSourceInfo(string name, string namespaceName)
			: base(name, namespaceName) {
		}
		public IDataModel ParentDataModel { get; internal set; }
		public IDXMethodInfo GetUnitOfWorkFactoryMethod { get; set; }
		public UnitOfWorkFactoryInfo UnitOfWorkFactory { get; set; }
		public virtual bool IsValid { get { return GetUnitOfWorkFactoryMethod != null && UnitOfWorkFactory != null && UnitOfWorkFactory.IsValid; } }
		public static UnitOfWorkSourceInfo Create(Type typeElement, IServiceContainer serviceContainer) {
			if(typeElement == null || !typeElement.IsClass || !typeElement.IsSealed ||
				(!typeElement.IsAbstract && !typeElement.IsSealed))
				return null;
			if(!typeElement.IsPublic) {
				IDXTypeInfo typeInfo = MetaDataServices.GetExistingOrCreateNew(typeElement);
				if(typeInfo == null || !typeInfo.Assembly.IsProjectAssembly)
					return null;
			}
			UnitOfWorkFactoryInfo unitOfWorkFactoryInfo = null;
			MethodInfo unitOfWorkFactoryInfoMethod = null;
			foreach(MethodInfo method in typeElement.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)) {
				unitOfWorkFactoryInfo = UnitOfWorkFactoryInfo.Create(method.ReturnType, serviceContainer);
				if(unitOfWorkFactoryInfo != null) {
					unitOfWorkFactoryInfoMethod = method;
					break;
				}
			}
			if(unitOfWorkFactoryInfo == null || unitOfWorkFactoryInfoMethod == null)
				return null;
			UnitOfWorkSourceInfo result = new UnitOfWorkSourceInfo(typeElement) {
				GetUnitOfWorkFactoryMethod = new DXMethodInfo(unitOfWorkFactoryInfoMethod),
				UnitOfWorkFactory = unitOfWorkFactoryInfo
			};
			if(result.IsValid)
				return result;
			return null;
		}
		public static UnitOfWorkSourceInfo Create(IDbContainerInfo container, IEnumerable<IEntitySetInfo> entities, string namespaceName, string commonNamespace) {
			UnitOfWorkSourceInfo result = new UnitOfWorkSourceInfo("UnitOfWorkSource", namespaceName);
			result.Assembly = MetaDataServices.SolutionTypesProvider.ActiveProjectTypes.ProjectAssembly;
			UnitOfWorkFactoryInfo unitOfWorkFactoryInfo = UnitOfWorkFactoryInfo.Create(container, entities, namespaceName, commonNamespace);
			if(unitOfWorkFactoryInfo == null || !unitOfWorkFactoryInfo.IsValid)
				return null;
			result.UnitOfWorkFactory = unitOfWorkFactoryInfo;
			result.GetUnitOfWorkFactoryMethod = new DXMethodInfo("GetUnitOfWorkFactory", unitOfWorkFactoryInfo.TypeInfo);
			if(result.IsValid)
				return result;
			return null;
		}
	}
	public class ExpressionHelperInfo : DXTypeInfo {
		public static ExpressionHelperInfo Create(ISolutionTypesProvider solutionTypesProvider, string utilsNamespace) {
			ExpressionHelperInfo expressionHelperInfo = new ExpressionHelperInfo("ExpressionHelper", utilsNamespace);
			expressionHelperInfo.Assembly = solutionTypesProvider.ActiveProjectTypes.ProjectAssembly;
			return expressionHelperInfo;
		}
		public static ExpressionHelperInfo Create(Type expressionHelperType, IServiceContainer serviceContainer) {
			return new ExpressionHelperInfo(expressionHelperType);
		}
		public ExpressionHelperInfo(Type type) : base(type) { }
		public ExpressionHelperInfo(string name, string namespaceName) : base(name, namespaceName) { }
		public IDXTypeInfo ExpressionHelperType { get; private set; }
		public bool IsValid { get { return true; } }
	}
	public class UnitOfWorkFactoryInfo {
		EntitiesUnitOfWorkInfo entitiesUnitOfWork;
		public UnitOfWorkFactoryInfo(IDXTypeInfo type, IDXMethodInfo method, EntitiesUnitOfWorkInfo entitiesUnitOfWork) {
			CreateUnitOfWorkMethod = method;
			TypeInfo = type;
			this.entitiesUnitOfWork = entitiesUnitOfWork;
		}
		public IDXMethodInfo CreateUnitOfWorkMethod { get; private set; }
		public IDXTypeInfo TypeInfo { get; private set; }
		public EntitiesUnitOfWorkInfo EntitiesUnitOfWork { get { return this.entitiesUnitOfWork; } }
		public bool IsValid {
			get { return TypeInfo != null && this.entitiesUnitOfWork != null && this.entitiesUnitOfWork.IsValid; }
		}
		public static UnitOfWorkFactoryInfo Create(Type typeElement, IContainerInfo containerInfo, IServiceContainer serviceContainer) {
			if(typeElement == null || !typeElement.IsInterface)
				return null;
			UnitOfWorkFactoryInfo result = null;
			foreach(MethodInfo method in ReflectionHelper.GetInterfaceMethods(typeElement)) {
				EntitiesUnitOfWorkInfo entitiesUnitOfWorkInfo = EntitiesUnitOfWorkInfo.Create(method.ReturnType, containerInfo, serviceContainer);
				if(entitiesUnitOfWorkInfo != null) {
					result = new UnitOfWorkFactoryInfo(new DXTypeInfo(typeElement), new DXMethodInfo(method), entitiesUnitOfWorkInfo);
					break;
				}
			}
			if(result != null && result.IsValid)
				return result;
			return null;
		}
		public static UnitOfWorkFactoryInfo Create(Type typeElement, IServiceContainer serviceContainer) {
			if(typeElement == null || !typeElement.IsInterface)
				return null;
			UnitOfWorkFactoryInfo result = null;
			foreach(MethodInfo method in ReflectionHelper.GetInterfaceMethods(typeElement)) {
				EntitiesUnitOfWorkInfo entitiesUnitOfWorkInfo = EntitiesUnitOfWorkInfo.Create(method.ReturnType, serviceContainer);
				if(entitiesUnitOfWorkInfo != null) {
					result = new UnitOfWorkFactoryInfo(new DXTypeInfo(typeElement), new DXMethodInfo(method), entitiesUnitOfWorkInfo);
					break;
				}
			}
			if(result != null && result.IsValid)
				return result;
			return null;
		}
		public static UnitOfWorkFactoryInfo Create(IDbContainerInfo container, IEnumerable<IEntitySetInfo> entities, string namespaceName, string commonNamespaceName) {
			if(container == null || entities == null || String.IsNullOrEmpty(namespaceName))
				return null;
			EntitiesUnitOfWorkInfo entitiesUnitOfWorkInfo = EntitiesUnitOfWorkInfo.Create(container, entities, namespaceName, commonNamespaceName);
			if(entitiesUnitOfWorkInfo == null)
				return null;
			DXTypeInfo typeInfo = new DXTypeInfo("IUnitOfWorkFactory", namespaceName);
			typeInfo.Assembly = MetaDataServices.SolutionTypesProvider.ActiveProjectTypes.ProjectAssembly;
			DXMethodInfo methodInfo = new DXMethodInfo("CreateUnitOfWork", entitiesUnitOfWorkInfo.TypeInfo);
			UnitOfWorkFactoryInfo result = new UnitOfWorkFactoryInfo(typeInfo, methodInfo, entitiesUnitOfWorkInfo);
			if(result.IsValid)
				return result;
			return null;
		}
	}
	public class EntitiesUnitOfWorkInfo {
		internal EntitiesUnitOfWorkInfo(IDXTypeInfo type, IDXTypeInfo baseType, IEnumerable<EntityRepositoryInfo> infos) {
			TypeInfo = type;
			BaseType = baseType;
			EntityRepositories = infos;
		}
		public IDXTypeInfo TypeInfo { get; private set; }
		public IEnumerable<EntityRepositoryInfo> EntityRepositories { get; private set; }
		public bool IsValid { get { return TypeInfo != null && EntityRepositories != null && EntityRepositories.FirstOrDefault(ri => !ri.IsValid) == null && BaseType != null; } }
		public IDXTypeInfo BaseType { get; private set; }
		public static EntitiesUnitOfWorkInfo Create(Type type, IContainerInfo containerInfo, IServiceContainer serviceContainer) {
			if(type == null || !type.IsInterface)
				return null;
			List<EntityRepositoryInfo> infos = new List<EntityRepositoryInfo>();
			foreach(PropertyInfo property in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)) {
				EntityRepositoryInfo info = EntityRepositoryInfo.Create(property, containerInfo, serviceContainer);
				if(info != null && info.IsValid && !infos.Contains(info))
					infos.Add(info);
			}
			Type[] interfaces = type.GetInterfaces();
			if(interfaces == null || interfaces.Length < 1)
				return null;
			ISolutionTypesProvider solutionTypesProvider = ((ISolutionTypesProvider)serviceContainer.GetService(typeof(ISolutionTypesProvider)));
			IDXTypeInfo baseType = solutionTypesProvider.ActiveProjectTypes.GetExistingOrCreateNew(interfaces[0]);
			IDXTypeInfo typeInfo = solutionTypesProvider.ActiveProjectTypes.GetExistingOrCreateNew(type);
			EntitiesUnitOfWorkInfo result = new EntitiesUnitOfWorkInfo(typeInfo, baseType, infos);
			if(result.IsValid)
				return result;
			return null;
		}
		public static EntitiesUnitOfWorkInfo Create(Type type, IServiceContainer serviceContainer) {
			if(type == null || !type.IsInterface)
				return null;
			List<EntityRepositoryInfo> infos = new List<EntityRepositoryInfo>();
			foreach(PropertyInfo property in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)) {
				EntityRepositoryInfo info = EntityRepositoryInfo.Create(property, serviceContainer);
				if(info != null && info.IsValid && !infos.Contains(info))
					infos.Add(info);
			}
			Type[] interfaces = type.GetInterfaces();
			if(interfaces == null || interfaces.Length < 1)
				return null;
			ISolutionTypesProvider solutionTypesProvider = ((ISolutionTypesProvider)serviceContainer.GetService(typeof(ISolutionTypesProvider)));
			IDXTypeInfo baseType = solutionTypesProvider.ActiveProjectTypes.GetExistingOrCreateNew(interfaces[0]);
			IDXTypeInfo typeInfo = solutionTypesProvider.ActiveProjectTypes.GetExistingOrCreateNew(type);
			EntitiesUnitOfWorkInfo result = new EntitiesUnitOfWorkInfo(typeInfo, baseType, infos);
			if(result.IsValid)
				return result;
			return null;
		}
		public static EntitiesUnitOfWorkInfo Create(IDbContainerInfo container, IEnumerable<IEntitySetInfo> entities, string namespaceName, string commonNamespace) {
			if(entities == null || container == null)
				return null;
			DXTypeInfo typeInfo = new DXTypeInfo(string.Format("I{0}UnitOfWork", container.Name), namespaceName);
			typeInfo.Assembly = MetaDataServices.SolutionTypesProvider.ActiveProjectTypes.ProjectAssembly;
			DXTypeInfo baseType = new DXTypeInfo(string.Format("IUnitOfWork", container.Name), commonNamespace);
			EntitiesUnitOfWorkInfo result = new EntitiesUnitOfWorkInfo(typeInfo, baseType, entities.Select<IEntitySetInfo, EntityRepositoryInfo>(es => EntityRepositoryInfo.Create(es, namespaceName, container)).Where(er => er != null).ToArray());
			if(result.IsValid)
				return result;
			return null;
		}
	}
	public class EntityRepositoryInfo {
		internal EntityRepositoryInfo(IDXTypeInfo type, IEntitySetInfo entitySet, IDXTypeInfo primaryKeyType, IDbContainerInfo dbContainerInfo) {
			EntitySet = entitySet;
			TypeInfo = type;
			PrimaryKeyType = primaryKeyType;
			DbContainerInfo = dbContainerInfo;
		}
		public IDbContainerInfo DbContainerInfo { get; private set; }
		public IDXTypeInfo TypeInfo { get; private set; }
		public bool IsValid { get { return TypeInfo != null && EntitySet != null; } }
		public IEntitySetInfo EntitySet { get; private set; }
		public bool IsReadOnly { get; set; }
		public string Name {
			get {
				if(EntitySet == null)
					return string.Empty;
				return EntitySet.Name;
			}
		}
		public IDXTypeInfo PrimaryKeyType { get; private set; }
		static Type[] GetTypeArguments(IServiceContainer serviceContainer, Type type) {
			if(type == null || serviceContainer == null || !type.IsInterface)
				return null;
			Type irepository = type;
			Type[] arguments = irepository.GetGenericArguments();
			if(arguments == null || arguments.Length < 1)
				return null;
			return arguments;
		}
		static EntityRepositoryInfo Create(Type type, Type[] arguments, Type entityType, IDbContainerInfo container) {
			IEntitySetInfo entitySet = container.EntitySets.FirstOrDefault(es => es.ElementType.Type.FullName == entityType.FullName && es.ElementType.Type.Assembly.FullName == entityType.Assembly.FullName); 
			if(entitySet == null)
				return null;
			if(arguments.Length > 1)
				return new EntityRepositoryInfo(new DXTypeInfo(type), entitySet, MetaDataServices.GetExistingOrCreateNew(arguments[1]), container);
			else
				return new EntityRepositoryInfo(new DXTypeInfo(type), entitySet, null, container) { IsReadOnly = true };
		}
		public static EntityRepositoryInfo Create(PropertyInfo property, IContainerInfo containerInfo, IServiceContainer serviceContainer) {
			if(property == null)
				return null;
			Type type = property.PropertyType;
			Type[] arguments = GetTypeArguments(serviceContainer, type);
			if(arguments == null)
				return null;
			Type entityType = arguments[0];
			if(entityType == null)
				return null;
			IEntityFrameworkModel efModel = (IEntityFrameworkModel)serviceContainer.GetService(typeof(IEntityFrameworkModel));
			if(efModel == null)
				return null;
			IDbContainerInfo container = efModel.GetContainer(containerInfo);
			if(container == null)
				return null;
			return Create(type, arguments, entityType, container);
		}
		public static EntityRepositoryInfo Create(PropertyInfo property, IServiceContainer serviceContainer) {
			if(property == null)
				return null;
			Type type = property.PropertyType;
			Type[] arguments = GetTypeArguments(serviceContainer, type);
			if(arguments == null)
				return null;
			Type entityType = arguments[0];
			if(entityType == null)
				return null;
			IEntityFrameworkModel efModel = (IEntityFrameworkModel)serviceContainer.GetService(typeof(IEntityFrameworkModel));
			if(efModel == null)
				return null;
			IEnumerable<IContainerInfo> containers = efModel.GetContainersInfo();
			foreach(IContainerInfo ci in containers) {
				IDbContainerInfo container = efModel.GetContainer(ci);
				if(container == null)
					continue;
				EntityRepositoryInfo result = Create(type, arguments, entityType, container);
				if(result != null)
					return result;
			}
			return null;
		}
		public static EntityRepositoryInfo Create(IEntitySetInfo entity, string namespaceName, IDbContainerInfo container) {
			DXTypeInfo typeInfo = new DXTypeInfo(string.Format("I{0}Repository", entity.Name), namespaceName);
			typeInfo.Assembly = MetaDataServices.SolutionTypesProvider.ActiveProjectTypes.ProjectAssembly;
			EntityRepositoryInfo result = null;
			if(entity.IsView || entity.ReadOnly || entity.AttachedInfo.UserSelectedIsReadOnly)
				result = new EntityRepositoryInfo(typeInfo, entity, null, container) { IsReadOnly = true };
			else {
				IEdmPropertyInfo keyMember = entity.ElementType.KeyMembers.First(km => km is IEdmPropertyInfo) as IEdmPropertyInfo;
				DXTypeInfo primaryKeyType = new DXTypeInfo(keyMember.GetUnderlyingClrType().Name, keyMember.GetUnderlyingClrType().Namespace);
				result = new EntityRepositoryInfo(typeInfo, entity, primaryKeyType, container);
			}
			if(result.IsValid)
				return result;
			return null;
		}
	}
}
