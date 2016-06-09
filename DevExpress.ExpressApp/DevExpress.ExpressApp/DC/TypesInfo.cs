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
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using System.Globalization;
namespace DevExpress.ExpressApp.DC {
	public class TypesInfo : ITypesInfo, IDisposableExt {
		private Boolean isDisposed;
		private ITypeInfoSource reflectionTypeInfoSource;
		private readonly List<IEntityStore> entityStores;
		private readonly Dictionary<Type, TypeInfo> typeStore;
		private readonly Dictionary<String, TypeInfo> typeNameStore;
		private readonly Dictionary<Assembly, DcAssemblyInfo> assemblyStore;
		private readonly Dictionary<Type, Object> entityTypesCache;
		private IDCEntityStore dcEntityStore;
		private CustomLogics customLogicsForDC;
		private EntitiesToGenerateInfo entitiesToGenerateInfoForDC;
		private TypeHierarchyHelper typeHierarchyHelper;
		private ITypeInfo[] types__ {
			get { return Enumerator.ToArray<TypeInfo>(typeStore.Values); }
		}
		private ITypeInfo[] persistentTypes__ {
			get { return Enumerator.ToArray<ITypeInfo>(PersistentTypes); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly Object lockObject = new Object();
		private void InitializeDCEntityStore(IDCEntityStore dcEntityStore) {
			foreach(String entityName in entitiesToGenerateInfoForDC.GetEntityNames()) {
				Type entityInterface = entitiesToGenerateInfoForDC.GetEntityInterface(entityName);
				if(entitiesToGenerateInfoForDC.HasBaseClass(entityName)) {
					Type baseClass = entitiesToGenerateInfoForDC.GetBaseClass(entityName);
					RegisterEntity(entityName, entityInterface, baseClass);
				}
				else {
					RegisterEntity(entityName, entityInterface);
				}
			}
			foreach(Type sharedPart in entitiesToGenerateInfoForDC.GetSharedParts()) {
				dcEntityStore.RegisterSharedPart(sharedPart);
			}
			foreach(Type interfaceType in customLogicsForDC.registeredLogics.Keys) {
				foreach(Type registeredLogics in customLogicsForDC.registeredLogics[interfaceType]) {
					dcEntityStore.RegisterDomainLogic(interfaceType, registeredLogics);
				}
			}
			foreach(Type interfaceType in customLogicsForDC.unregisteredLogics.Keys) {
				foreach(Type unregisteredLogics in customLogicsForDC.unregisteredLogics[interfaceType]) {
					dcEntityStore.UnregisterDomainLogic(interfaceType, unregisteredLogics);
				}
			}
		}
		private void DisposeAssemblyInfo() {
			foreach(DcAssemblyInfo assemblyInfo in assemblyStore.Values) {
				assemblyInfo.Dispose();
			}
			assemblyStore.Clear();
		}
		private void DisposeTypeInfo() {
#if DebugTest
			foreach(TypeInfo typeInfo in typeStore.Values) {
				typeInfo.Dispose();
			}
#endif
			typeStore.Clear();
			typeNameStore.Clear();
		}
		private Boolean AssemblyIsKnown(Assembly assembly) {
			bool isDotNetAssembly = false;
			if (assembly != null) {
				try {
					string location = assembly.Location;
					if (!string.IsNullOrEmpty(location)) {
						System.Diagnostics.FileVersionInfo versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(location);
						if (versionInfo != null) {
							isDotNetAssembly = versionInfo.CompanyName.StartsWith("Microsoft") && versionInfo.ProductName.Contains(".NET Framework");
						}
					}
				}
				catch { }
			}
			return !isDotNetAssembly;
		}
		private void AddAssemblyInfo(Assembly assembly, DcAssemblyInfo info) {
			assemblyStore[assembly] = info;
		}
		private void InitAssemblyInfo(Assembly assembly, DcAssemblyInfo result) {
			try {
				result.SetAttributes(assembly.GetCustomAttributes(false), assembly.GetCustomAttributes(true));
			}
			catch(Exception e) {
				String message = String.Format("DcAssemblyInfo: {0}, Assembly: {1}, Message: {2}, StackTrace: {3}",
					result.FullName, assembly.FullName, e.Message, e.StackTrace);
				throw new Exception(message);
			}
		}
		private void AddTypesToAssemblyInfo(DcAssemblyInfo assemblyInfo, IEnumerable<Type> types) {
			foreach(Type type in types) {
				assemblyInfo.AddTypeInfo(FindTypeInfo(type));
			}
		}
		private Type GetOriginalType(Type type) {
			Type result = type;
			foreach(IEntityStore entityStore in entityStores) {
				Type originalType = entityStore.GetOriginalType(type);
				if((originalType != null) && (originalType != type)) {
					result = originalType;
					break;
				}
			}
			return result;
		}
		private TypeInfo GetTypeInfo(Type type) {
			TypeInfo result = null;
			type = GetOriginalType(type);
			if(!TryGetTypeInfo(type, out result)) {
				typeHierarchyHelper.Register(type);
				result = CreateTypeInfo(type);
				AddTypeInfo(result);
			}
			return result;
		}
		private Boolean TryGetTypeInfo(Type type, out TypeInfo info) {
			return typeStore.TryGetValue(type, out info);
		}
		private void AddTypeInfo(TypeInfo info) {
			Type type = info.Type;
			typeStore.Add(type, info);
			String typeFullName = type.FullName;
			if(!String.IsNullOrEmpty(typeFullName)) {
				typeNameStore[typeFullName] = info;
			}
		}
		private TypeInfo CreateTypeInfo(Type type) {
			TypeInfo result = new TypeInfo(type, this);
			RefreshAssemblyInfo(result);
			result.Source = reflectionTypeInfoSource;
			result.Refresh();
			return result;
		}
		private void RefreshAssemblyInfo(TypeInfo typeInfo) {
			DcAssemblyInfo assemblyInfo = FindAssemblyInfo(typeInfo.Type.Assembly);
			if(assemblyInfo != null) {
				typeInfo.AssemblyInfo = assemblyInfo;
				assemblyInfo.AddTypeInfo(typeInfo);
			}
		}
		private IEntityStore FindSuitableEntityStore(Type type) {
			IEntityStore result = null;
			foreach(IEntityStore entityStore in entityStores) {
				if(entityStore.CanRegister(type)) {
					result = entityStore;
				}
			}
			return result;
		}
		private void EnsureReferencesForEntity(Type entityType) {
			foreach(Type requiredType in GetRequiredTypes(entityType)) {
				RegisterEntity(requiredType);
			}
		}
		private IEnumerable<Type> GetRequiredTypes(Type type) {
			List<Type> result = new List<Type>();
			ITypeInfo typeInfo = GetTypeInfo(type);
			foreach(ITypeInfo info in typeInfo.GetRequiredTypes(IsRequiredType)) {
				result.Add(info.Type);
			}
			return result;
		}
		private Boolean IsRequiredType(ITypeInfo typeInfo) {
			Type type = typeInfo.Type;
			foreach(IEntityStore entityStore in entityStores) {
				if(entityStore.CanRegister(type)) {
					return true;
				}
			}
			return false;
		}
		protected internal void Reset() {
			foreach(IEntityStore entityStore in entityStores) {
				entityStore.Reset();
			}
			entityTypesCache.Clear();
			DevExpress.ExpressApp.SystemModule.ObjectMethodActionsViewController.Reset();
		}
		protected internal void HardReset() {
			Reset();
			DisposeAssemblyInfo();
			DisposeTypeInfo();
			reflectionTypeInfoSource = new ReflectionTypeInfoSource();
			typeHierarchyHelper = new TypeHierarchyHelper();
		}
		public TypesInfo() {
			reflectionTypeInfoSource = new ReflectionTypeInfoSource();
			assemblyStore = new Dictionary<Assembly, DcAssemblyInfo>();
			typeStore = new Dictionary<Type, TypeInfo>();
			typeNameStore = new Dictionary<String, TypeInfo>();
			entityStores = new List<IEntityStore>();
			entityTypesCache = new Dictionary<Type, Object>();
			customLogicsForDC = new CustomLogics();
			entitiesToGenerateInfoForDC = new EntitiesToGenerateInfo();
			typeHierarchyHelper = new TypeHierarchyHelper();
		}
		public void AddEntityStore(IEntityStore entityStore) {
			Guard.ArgumentNotNull(entityStore, "entityStore");
			if(!entityStores.Contains(entityStore)) {
				entityStores.Add(entityStore);
				if(dcEntityStore == null) {
					dcEntityStore = entityStore as IDCEntityStore;
					if(dcEntityStore != null) {
						InitializeDCEntityStore(dcEntityStore);
					}
				}
			}
		}
		public IEntityStore FindEntityStore(Type entityStoreType) {
			IEntityStore result = null;
			foreach(IEntityStore entityStore in entityStores) {
				if(entityStoreType.IsAssignableFrom(entityStore.GetType())) {
					result = entityStore;
					break;
				}
			}
			return result;
		}
		public IEntityStore FindEntityStore(String entityStoreTypeName) {
			IEntityStore result = null;
			foreach(IEntityStore entityStore in entityStores) {
				if(entityStore.GetType().Name == entityStoreTypeName) {
					result = entityStore;
					break;
				}
			}
			return result;
		}
		public void RegisterEntity(String name, Type interfaceType) {
			lock(lockObject) {
				if(dcEntityStore != null) {
					dcEntityStore.RegisterEntity(name, interfaceType);
					if(IsRegisteredEntity(interfaceType)) {
						RefreshInfo(interfaceType);
					}
					else {
						RegisterEntity(interfaceType);
					}
				}
				else {
					entitiesToGenerateInfoForDC.AddEntity(name, interfaceType);
				}
			}
		}
		public void RegisterEntity(String name, Type interfaceType, Type baseClass) {
			lock(lockObject) {
				if(dcEntityStore != null) {
					dcEntityStore.RegisterEntity(name, interfaceType, baseClass);
					if(IsRegisteredEntity(interfaceType)) {
						RefreshInfo(interfaceType);
					}
					else {
						RegisterEntity(interfaceType);
					}
					RegisterEntity(baseClass);
				}
				else {
					entitiesToGenerateInfoForDC.AddEntity(name, interfaceType);
					entitiesToGenerateInfoForDC.AddBaseClass(name, baseClass);
				}
			}
		}
		public void RegisterSharedPart(Type interfaceType) {
			lock(lockObject) {
				if(dcEntityStore != null) {
					dcEntityStore.RegisterSharedPart(interfaceType);
				}
				else {
					entitiesToGenerateInfoForDC.AddSharedPart(interfaceType);
				}
			}
		}
		public void RegisterDomainLogic(Type interfaceType, Type logicType) {
			lock(lockObject) {
				if(dcEntityStore != null) {
					dcEntityStore.RegisterDomainLogic(interfaceType, logicType);
				}
				else {
					customLogicsForDC.RegisterLogic(interfaceType, logicType);
				}
			}
		}
		public void UnregisterDomainLogic(Type interfaceType, Type logicType) {
			lock(lockObject) {
				if(dcEntityStore != null) {
					dcEntityStore.UnregisterDomainLogic(interfaceType, logicType);
				}
				else {
					customLogicsForDC.UnregisterLogic(interfaceType, logicType);
				}
			}
		}
		public void GenerateEntities() {
			GenerateEntities(null);
		}
		public void GenerateEntities(String generatedAssemblyFile) {
			lock(lockObject) {
				if(dcEntityStore != null) {
					dcEntityStore.GenerateEntities(generatedAssemblyFile);
				}
			}
		}
		public Type GetGeneratedEntityType(Type interfaceType) {
			if(dcEntityStore != null) {
				return dcEntityStore.GetGeneratedEntityType(interfaceType);
			}
			return null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RegisterEntity(Type entityType) {
			lock(lockObject) {
				if((entityType != null) && !entityTypesCache.ContainsKey(entityType)) {
					IEntityStore entityStore = FindSuitableEntityStore(entityType);
					if(entityStore != null) {
						entityTypesCache.Add(entityType, null);
						entityStore.RegisterEntity(entityType);
						EnsureReferencesForEntity(entityType);
					}
				}
			}
		}
		public void RegisterEntities(params Type[] types) {
			foreach(Type type in types) {
				RegisterEntity(type);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsRegisteredEntity(Type entityType) {
			return entityTypesCache.ContainsKey(entityType);
		}
		public TypeInfo FindTypeInfo(string typeName) {
			lock(lockObject) {
				if(String.IsNullOrEmpty(typeName))
					return null;
				TypeInfo result = null;
				if(!typeNameStore.TryGetValue(typeName, out result)) {
					return null;
				}
				return result;
			}
		}
		public TypeInfo FindTypeInfo(Type type) {
			if(type != null) {
				lock(lockObject) {
					return GetTypeInfo(type);
				}
			}
			else {
				return null;
			}
		}
		public bool CanInstantiate(Type type) {
			TypeInfo info = FindTypeInfo(type);
			if(info != null) {
				return info.CanInstantiate();
			}
			return false;
		}
		public void RefreshInfo(Type type) {
			RefreshInfo(FindTypeInfo(type));
		}
		public void RefreshInfo(ITypeInfo info) {
			lock(lockObject) {
				TypeInfo typeInfo = (TypeInfo)info;
				RefreshInfoCore(typeInfo);
				foreach(TypeInfo requiredTypeInfo in typeInfo.RequiredTypes) {
					RefreshInfoCore(requiredTypeInfo);
				}
			}
		}
		private void RefreshInfoCore(TypeInfo typeInfo) {
			typeInfo.Refresh();
			typeInfo.RefreshMembers();
		}
		public DcAssemblyInfo FindAssemblyInfo(Type ofType) {
			return FindAssemblyInfo(ofType.Assembly);
		}
		public DcAssemblyInfo FindAssemblyInfo(Assembly assembly) {
			lock(lockObject) {
				if(assembly == null) return null;
				DcAssemblyInfo result = null;
				if(!assemblyStore.TryGetValue(assembly, out result)) {
					if(AssemblyIsKnown(assembly)) {
						result = new DcAssemblyInfo(assembly, this);
						AddAssemblyInfo(assembly, result);
						InitAssemblyInfo(assembly, result);
						return result;
					}
				}
				return result;
			}
		}
		public void LoadTypes(DcAssemblyInfo assemblyInfo) {
			lock(lockObject) {
				Guard.ArgumentNotNull(assemblyInfo, "assemblyInfo");
				if(!assemblyInfo.AllTypesLoaded) {
					Assembly assembly = assemblyInfo.Assembly;
					AddTypesToAssemblyInfo(assemblyInfo, AssemblyHelper.GetTypesFromAssembly(assembly));
					assemblyInfo.AllTypesLoaded = true;
				}
			}
		}
		public IMemberInfo CreatePath(IMemberInfo first, IMemberInfo second) {
			if(first == null)
				return second;
			if(second == null)
				return first;
			MemberPathInfo result = new MemberPathInfo("");
			foreach(IMemberInfo member in first.GetPath()) {
				result.AddMember(member);
			}
			foreach(IMemberInfo member in second.GetPath()) {
				result.AddMember(member);
			}
			return result;
		}
		ITypeInfo ITypesInfo.FindTypeInfo(string typeName) {
			return FindTypeInfo(typeName);
		}
		ITypeInfo ITypesInfo.FindTypeInfo(Type type) {
			return FindTypeInfo(type);
		}
		IAssemblyInfo ITypesInfo.FindAssemblyInfo(Type ofType) {
			return FindAssemblyInfo(ofType);
		}
		IAssemblyInfo ITypesInfo.FindAssemblyInfo(Assembly assembly) {
			return FindAssemblyInfo(assembly);
		}
		public void LoadTypes(Assembly assembly) {
			DcAssemblyInfo assemblyInfo = FindAssemblyInfo(assembly);
			if(assemblyInfo != null) {
				LoadTypes(assemblyInfo);
			}
		}
		public Type[] GetAssemblyTypes(Assembly assembly) {
			return GetAssemblyTypes(assembly, null);
		}
		public Type[] GetAssemblyTypes(Assembly assembly, Predicate<Type> filter) {
			List<Type> result = new List<Type>();
			if(!assemblyStore.ContainsKey(assembly) || !assemblyStore[assembly].AllTypesLoaded) {
				LoadTypes(assembly);
			}
			if(assemblyStore.ContainsKey(assembly) && assemblyStore[assembly].AllTypesLoaded) {
				foreach(TypeInfo typeInfo in assemblyStore[assembly].Types) {
					if(filter == null || filter(typeInfo.Type)) {
						result.Add(typeInfo.Type);
					}
				}
			}
			return result.ToArray();
		}
		public IEnumerable<ITypeInfo> PersistentTypes {
			get {
				foreach(IEntityStore entityStore in entityStores) {
					foreach(Type type in entityStore.RegisteredEntities) {
						ITypeInfo info = FindTypeInfo(type);
						if(info != null) {
							yield return info;
						}
					}
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IList<IEntityStore> EntityStores {
			get { return entityStores.AsReadOnly(); }
		}
		public TypeHierarchyHelper TypeHierarchyHelper {
			get { return typeHierarchyHelper; }
		}
		#region IDisposableExt Members
		public void Dispose() {
			if(isDisposed) {
				return;
			}
			isDisposed = true;
			dcEntityStore = null;
			reflectionTypeInfoSource = null;
			foreach(IEntityStore entityStore in entityStores) {
				if(entityStore is IDisposable) {
					((IDisposable)entityStore).Dispose();
				}	
			}
			entityStores.Clear();
			entityTypesCache.Clear();
			DisposeAssemblyInfo();
			DisposeTypeInfo();
			customLogicsForDC = null;
			entitiesToGenerateInfoForDC = null;
			typeHierarchyHelper = null;
		}
		public bool IsDisposed {
			get { return isDisposed; }
		}
		#endregion
	}
}
