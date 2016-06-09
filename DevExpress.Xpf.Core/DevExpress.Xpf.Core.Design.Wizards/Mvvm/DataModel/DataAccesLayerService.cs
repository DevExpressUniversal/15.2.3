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
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.MetaData;
using DevExpress.Design.Mvvm;
using DevExpress.Design.Mvvm.EntityFramework;
using System.Reflection;
using System.ComponentModel.Design;
using DevExpress.Entity.ProjectModel;
using DevExpress.Entity.Model;
using System.Runtime.CompilerServices;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.DataModel {
	public class DataModelProxy : IDataModel {
		Dictionary<string, Type> repositories;
		readonly IServiceContainer serviceContainer;
		List<EntityRepositoryInfo> entityRepositoryInfoCache;
		Type iUnitOfWorkRuntime;
		MethodInfo getUnitOfWorkFactoryMethod;
		IDXTypeInfo unitOfWorkSource;
		IDXTypeInfo expressionHelper;
		Type unitOfWorkFactory;
		public DataModelProxy(IContainerInfo containerInfo, IServiceContainer serviceContainer) {
			ContainerInfo = containerInfo;
			this.serviceContainer = serviceContainer;
			Init(SolutionTypesProvider.GetTypes());
		}
		ISolutionTypesProvider SolutionTypesProvider { get { return (ISolutionTypesProvider)serviceContainer.GetService(typeof(ISolutionTypesProvider)); } }
		public IContainerInfo ContainerInfo { get; private set; }
		void AddRepository(Type type, Type keyType) {
			if (HasEntitySet(type))
				return;
			if (repositories == null)
				repositories = new Dictionary<string, Type>();
			repositories.Add(type.FullName, keyType);
		}
		public bool HasEntitySet(Type entity) {
			if (repositories == null)
				return false;
			return repositories.ContainsKey(entity.FullName);
		}
		Type GetFirstInterface(Type type) {
			if (type == null)
				return null;
			Type[] interfaces = type.GetInterfaces();
			if (interfaces == null)
				return null;
			Type[] result = interfaces.Where(x => !interfaces.Any(y => y.GetInterfaces().Contains(x))).ToArray();
			if (result.Length == 0)
				return null;
			return result[0];
		}
		void Init(IEnumerable<IDXTypeInfo> solutionTypes) {
			try {
				foreach (IDXTypeInfo typeInfo in solutionTypes) {
					Type dbUnitOfWorkRuntime = typeInfo.ResolveType();
					if (dbUnitOfWorkRuntime == null)
						continue;
					Type baseType = dbUnitOfWorkRuntime.BaseType;
					if (baseType == null || !baseType.IsGenericType)
						continue;
					Type[] dbUnitOfWorkTypeArgs = baseType.GetGenericArguments();
					if (dbUnitOfWorkTypeArgs == null || dbUnitOfWorkTypeArgs.Length < 1 || !ReflectionHelper.TypesAreEqual(ContainerInfo.ResolveType(), dbUnitOfWorkTypeArgs[0]))
						continue;
					iUnitOfWorkRuntime = GetFirstInterface(dbUnitOfWorkRuntime);
					if (iUnitOfWorkRuntime == null)
						continue;
					repositories = null;
					PropertyInfo[] pinfos = iUnitOfWorkRuntime.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
					if (pinfos != null) { 
						foreach (PropertyInfo pi in pinfos) {
							Type iRepository = pi.PropertyType;
							if (!iRepository.IsGenericType)
								continue;
							Type[] iRepositoryArgs = iRepository.GetGenericArguments();
							if (iRepositoryArgs == null || iRepositoryArgs.Length < 1)
								continue;
							Type keyType = null;
							if (iRepositoryArgs.Length > 1)
								keyType = iRepositoryArgs[1];
							AddRepository(iRepositoryArgs[0], keyType);
						}
					}
					if (!HasRepositories)
						continue;
					InitUnitOfWorkSource();
					InitExpressionHelper(solutionTypes);
					if (IsValid)
						break;
				}
			}
			catch {
				return;
			}
		}
		bool IsDataModelValid {
			get { return HasRepositories && iUnitOfWorkRuntime != null; }
		}
		void InitUnitOfWorkSource() {
			if (!IsDataModelValid)
				return;
			IDXTypeInfo iUnitOfWorkRuntimeInfo = SolutionTypesProvider.ActiveProjectTypes.GetExistingOrCreateNew(iUnitOfWorkRuntime);
			IEnumerable<IDXTypeInfo> solutionTypes = iUnitOfWorkRuntimeInfo.Assembly.TypesInfo;
			foreach (IDXTypeInfo typeInfo in solutionTypes) {
				Type type = typeInfo.ResolveType();
				MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
				if (methods == null)
					continue;
				foreach (MethodInfo mi in methods) {
					Type iUnitOfWorkFactory = mi.ReturnType;
					if (iUnitOfWorkFactory == null || !iUnitOfWorkFactory.IsInterface)
						continue;
					MethodInfo[] unitOfWorkFactoryInfos = ReflectionHelper.GetInterfaceMethods(iUnitOfWorkFactory);
					foreach (MethodInfo methodInfo in unitOfWorkFactoryInfos) {
						if (methodInfo.ReturnType != iUnitOfWorkRuntime)
							continue;
						getUnitOfWorkFactoryMethod = methodInfo;
						unitOfWorkFactory = iUnitOfWorkFactory;
						unitOfWorkSource = typeInfo;
						return;
					}
				}
			}
		}
		void InitExpressionHelper(IEnumerable<IDXTypeInfo> solutionTypes) {
			if (!IsDataModelValid) return;
			expressionHelper = solutionTypes.FirstOrDefault(t => t.Name == "ExpressionHelper");
		}
		bool IDataModel.IsInSolution {
			get {
				return ContainerInfo.IsSolutionType;
			}
		}
		public IDbContainerInfo DbContainer {
			get {
				IEntityFrameworkModel efModel = (IEntityFrameworkModel)serviceContainer.GetService(typeof(IEntityFrameworkModel));
				if (efModel == null)
					return null;
				return efModel.GetContainer(ContainerInfo);
			}
		}
		public IEnumerable<IEntitySetInfo> Entities {
			get { return DbContainer.EntitySets; }
		}
		public IEnumerable<EntityRepositoryInfo> EntityRepositories {
			get {
				if (entityRepositoryInfoCache != null)
					return entityRepositoryInfoCache;
				IEnumerable<IEntitySetInfo> entitySets = DbContainer.EntitySets;
				if (entitySets == null)
					return null;
				foreach (IEntitySetInfo entitySet in entitySets) {
					Type entityType = entitySet.ElementType.Type;
					if (entityType == null || !repositories.ContainsKey(entityType.FullName))
						continue;
					Type keyType = repositories[entityType.FullName];
					EntityRepositoryInfo rep = new EntityRepositoryInfo(MetaDataServices.GetExistingOrCreateNew(entityType), entitySet,
							MetaDataServices.GetExistingOrCreateNew(keyType), DbContainer);
					if (keyType == null)
						rep.IsReadOnly = true;
					if (entityRepositoryInfoCache == null)
						entityRepositoryInfoCache = new List<EntityRepositoryInfo>();
					entityRepositoryInfoCache.Add(rep);
				}
				return entityRepositoryInfoCache;
			}
		}
		public EntitiesUnitOfWorkInfo EntitiesUnitOfWork {
			get {
				return EntitiesUnitOfWorkInfo.Create(iUnitOfWorkRuntime, this.ContainerInfo, serviceContainer);
			}
		}
		public UnitOfWorkFactoryInfo UnitOfWorkFactory {
			get { return UnitOfWorkFactoryInfo.Create(unitOfWorkFactory, this.ContainerInfo, serviceContainer); }
		}
		public ExpressionHelperInfo ExpressionHelper {
			get { return new ExpressionHelperInfo(expressionHelper.ResolveType()); }
		}
		public UnitOfWorkSourceInfo UnitOfWorkSource {
			get {
				return new UnitOfWorkSourceInfo(unitOfWorkSource.ResolveType()) {
					GetUnitOfWorkFactoryMethod = new DXMethodInfo(getUnitOfWorkFactoryMethod),
					UnitOfWorkFactory = UnitOfWorkFactory
				};
			}
		}
		public bool IsValid {
			get {
				if (!HasRepositories) return false;
				if (iUnitOfWorkRuntime == null) return false;
				if (getUnitOfWorkFactoryMethod == null) return false;
				if (unitOfWorkSource == null) return false;
				if (unitOfWorkFactory == null) return false;
				if (expressionHelper == null) return false;
				return true;
			}
		}
		bool HasRepositories {
			get {
				return repositories != null;
			}
		}
		public string Name {
			get {
				if (ContainerInfo == null)
					return "Empty";
				return string.Format("{0} Data Model", ContainerInfo.Name);
			}
		}
	}
	public class DataAccesLayerService : HasTypesCacheBase, IDataAccessLayerService {
		readonly IServiceContainer serviceContainer;
		public DataAccesLayerService(IServiceContainer serviceContainer) {
			this.serviceContainer = serviceContainer;
		}
		ISolutionTypesProvider SolutionTypesProvider { get { return (ISolutionTypesProvider)serviceContainer.GetService(typeof(ISolutionTypesProvider)); } }
		Dictionary<IDXTypeInfo, IDataModel> models;
		void BuildDataModelCache() {
			IEnumerable<IDXTypeInfo> types = SolutionTypesProvider.GetTypes();
			IEntityFrameworkModel efModel = (IEntityFrameworkModel)serviceContainer.GetService(typeof(IEntityFrameworkModel));
			if (efModel == null)
				return;
			IEnumerable<IContainerInfo> containersInfo = efModel.GetContainersInfo();
			foreach (IContainerInfo containerInfo in containersInfo) {
				DataModelProxy dmp = new DataModelProxy(containerInfo, serviceContainer);
				if (!dmp.IsValid)
					continue;
				this.Add(containerInfo);
				if (models == null)
					models = new Dictionary<IDXTypeInfo, IDataModel>();
				models.Add(containerInfo, dmp);
			}
		}
		public IEnumerable<IDataModel> GetAvailableDataModels() {
			if (IsCacheEmpty)
				BuildDataModelCache();
			return this.Cache.Values.Select<IDXTypeInfo, IDataModel>(ti => {
				if (models == null || !models.ContainsKey(ti))
					return null;
				return models[ti];
			}).Where(dm => dm != null);
		}
		public void Register(IDataModel dataModel) {
			Add(dataModel as IDXTypeInfo);
		}
		public IEntitySetInfo FindEntitySetInAvailableDataModels(IDXTypeInfo entityTypeInfo, out IDataModel model) {
			model = null;
			try {
				Type entityType = entityTypeInfo.ResolveType();
				IEnumerable<IDataModel> datamodels = this.GetAvailableDataModels();				
				foreach (IDataModel dataModel in datamodels) {
					Type dbContainer = dataModel.ContainerInfo.ResolveType();
					IEnumerable<PropertyInfo> properties = dbContainer.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
					string dbSetTypeName = "DataServiceQuery`1";
					if (dataModel.ContainerInfo.ContainerType == DbContainerType.EntityFramework)
						dbSetTypeName = "DbSet`1";
					PropertyInfo info = properties.FirstOrDefault(x => x.PropertyType.Name == dbSetTypeName && x.PropertyType.GetGenericArguments().Contains(entityType));
					if (info != null) {
						IEntitySetInfo result = dataModel.Entities.FirstOrDefault(x => x.ElementType.Type == entityType);
						if (result != null) {
							model = dataModel;
							return result;
						}
					}
				}
				foreach (IDataModel dataModel in datamodels) {
					IDbContainerInfo dbContainer = dataModel.DbContainer;
					if (dbContainer == null)
						continue;
					IEntitySetInfo result = dataModel.Entities.FirstOrDefault(x => x.ElementType.Type == entityType);
					if (result != null) {
						model = dataModel;
						return result;
					}
				}
				return null;
			}
			catch {
				return null;
			}
		}
	}
	public static class ReflectionHelper {
		public static MethodInfo[] GetInterfaceMethods(Type type) {
			return type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
				.Concat(type.GetInterfaces().SelectMany(i => i.GetMethods(BindingFlags.Instance | BindingFlags.Public)))
				.ToArray();
		}
		public static bool TypesAreEqual(Type type1, Type type2) {
			if(type1 == type2) return true;
			if(type1 == null || type2 == null) return false;
			if(!string.Equals(type1.FullName, type2.FullName, StringComparison.Ordinal)) return false;
			return string.Equals(type1.Assembly.FullName, type2.Assembly.FullName, StringComparison.Ordinal);
		}
	}
}
