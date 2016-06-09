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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Data.Common;
using System.Reflection.Emit;
using System.Collections.Generic;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.Entity.Model.DescendantBuilding {
	public abstract class EF60DbDescendantBuilderBase : DbDescendantBuilder {
		protected readonly IDXAssemblyInfo servicesAssembly;
		static Type dbConfigurationType;
		static object dbConfigurationTypeLockObject = new object();
		public EF60DbDescendantBuilderBase(TypesCollector typesCollector, IDXAssemblyInfo servicesAssembly)
			: base(typesCollector) {
			this.servicesAssembly = servicesAssembly;
		}
		public abstract string ProviderName { get; }
		public abstract string ProviderManifestToken { get; }
		public abstract string SqlProviderServicesTypeName { get; }
		protected abstract string GetConnectionString(string dbFilePath);
		protected void EmitCallToBaseTypeCtor(Type dbConfigurationType, ILGenerator ilGenerator) {
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ConstructorInfo baseTypeCtor = dbConfigurationType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(x => {
				ParameterInfo[] p = x.GetParameters();
				return p != null && p.Length == 0;
			});
			ilGenerator.Emit(OpCodes.Call, baseTypeCtor);
		}
		protected Type GetActivatedSingletonDependencyResolverType() {
			Type singletonDependencyResolverType = FindEFType("System.Data.Entity.Infrastructure.DependencyResolution.SingletonDependencyResolver");
			Type dbProviderServicesType = FindEFType("System.Data.Entity.Core.Common.DbProviderServices");
			return singletonDependencyResolverType.MakeGenericType(dbProviderServicesType);
		}
		protected void EmitCallToAddDependencyResolver(Type dbConfigurationType, ILGenerator ilGenerator) {
			Type singletonDependencyResolverType = GetActivatedSingletonDependencyResolverType();
			ConstructorInfo singletonConstructor = singletonDependencyResolverType.GetConstructors().FirstOrDefault(x => {
				ParameterInfo[] parameters = x.GetParameters();
				return parameters != null && parameters.Length == 2 && parameters[1].ParameterType.FullName == typeof(object).FullName;
			});
			ilGenerator.Emit(OpCodes.Newobj, singletonConstructor);
			ilGenerator.Emit(OpCodes.Stloc_1);
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldloc_1);
			MethodInfo addDependencyResolverMethod = dbConfigurationType.GetMethod("AddDependencyResolver", BindingFlags.Instance | BindingFlags.NonPublic);
			ilGenerator.Emit(OpCodes.Call, addDependencyResolverMethod);
		}
		protected void EmitCallToAddDefaultResolver(Type dbConfigurationType, ILGenerator ilGenerator) {
			MethodInfo addDefaultResolverMethod = dbConfigurationType.GetMethod("AddDefaultResolver", BindingFlags.Instance | BindingFlags.NonPublic);
			ilGenerator.Emit(OpCodes.Call, addDefaultResolverMethod);					   
		}	   
		protected virtual Type EmitDbConfigurationType(ModuleBuilder moduleBuilder) {
			Type dbConfigurationType = GetDbConfigurationType();
			TypeBuilder typeBuilder = moduleBuilder.DefineType(this.GetType().FullName + "." + "DbConfiguration", TypeAttributes.Public, dbConfigurationType, null);
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[0]);
			ILGenerator ilGenerator = constructorBuilder.GetILGenerator();
			Type sqlProviderServicesType = GetSqlProviderServicesType();
			Type singletonDependencyResolverType = GetActivatedSingletonDependencyResolverType();
			ilGenerator.DeclareLocal(sqlProviderServicesType);
			EmitCallToBaseTypeCtor(dbConfigurationType, ilGenerator);			
			PropertyInfo instanceProperty = sqlProviderServicesType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
			MethodInfo instanceGetter = instanceProperty.GetGetMethod();
			ilGenerator.Emit(OpCodes.Call, instanceGetter);
			ilGenerator.Emit(OpCodes.Stloc_0);
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldloc_0);			
			ilGenerator.Emit(OpCodes.Ldstr, ProviderName);			
			ConstructorInfo singletonConstructor = singletonDependencyResolverType.GetConstructors().FirstOrDefault(x => {
				ParameterInfo[] parameters = x.GetParameters();
				return parameters != null && parameters.Length == 2 && parameters[1].ParameterType.FullName == typeof(object).FullName;
			});
			ilGenerator.Emit(OpCodes.Newobj, singletonConstructor);
			MethodInfo addDependencyResolverMethod = dbConfigurationType.GetMethod("AddDependencyResolver", BindingFlags.Instance | BindingFlags.NonPublic);
			ilGenerator.Emit(OpCodes.Call, addDependencyResolverMethod);
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldloc_0);
			EmitCallToAddDefaultResolver(dbConfigurationType, ilGenerator);
			ilGenerator.Emit(OpCodes.Ret);
			return typeBuilder.CreateType();
		}
		protected Type GetIProviderInvariantNameType() {
			return EntityFrameworkAssembly.GetType("System.Data.Entity.Infrastructure.IProviderInvariantName");
		}
		protected Type GetIDbDependencyResolverType() {
			return this.EntityFrameworkAssembly.GetType("System.Data.Entity.Infrastructure.DependencyResolution.IDbDependencyResolver");
		}
		protected Type GetDbConfigurationType() {
			return this.EntityFrameworkAssembly.GetType("System.Data.Entity.DbConfiguration");
		}
		protected Type GetDbConfigurationTypeAttributeType() {
			return this.EntityFrameworkAssembly.GetType("System.Data.Entity.DbConfigurationTypeAttribute");
		}
		protected Type GetSqlProviderServicesType() {
			IDXTypeInfo info = this.servicesAssembly.TypesInfo.FirstOrDefault(x => x.FullName == SqlProviderServicesTypeName);
			if(info != null)
				return info.ResolveType();
			if(!SupressExceptions)
				throw new ArgumentException(string.Format("Could not find type {0}", SqlProviderServicesTypeName));
			return null;
		}
		protected override void PrepareEdmx(EdmxResource edmxResource) {
			if (edmxResource == null)
				return;
			edmxResource.WriteResources(TempFolder, new EdmxResource.SchemaAttributeValues(ProviderName, ProviderManifestToken));
		}
		public override object Build() {
			DescendantInstance = null;
			DXTypeInfo typeInfo = TypesCollector.DbDescendantInfo as DXTypeInfo;
			if (typeInfo == null)
				return null;
			EdmxResource edmxResource = EdmxResource.GetEdmxResource(typeInfo);
			bool isModelFirst = edmxResource != null;
			try {
				ClearDBConfiguration();
				ClearMetadataCache();
				ModuleBuilder mb = CreateDynamicAssembly();
				lock(dbConfigurationTypeLockObject) {
					if(dbConfigurationType == null)
						dbConfigurationType = EmitDbConfigurationType(mb);
				}
				Tuple<ConstructorInfo, Type[]> ctorTuple = GetDbContextConstructor(null, dbContext);
				if (ctorTuple == null)
					return null;
				Type resultType = EmitDbDescendant(TypesCollector, ctorTuple, mb, isModelFirst, dbConfigurationType);
				SetDbConfiguration(Activator.CreateInstance(dbConfigurationType));
				object connection = CreateDbConnection(dbContext, isModelFirst, TypesCollector);
				if (connection == null)
					return null;
				if (isModelFirst)
					PrepareEdmx(edmxResource);
				if (ctorTuple.Item1.GetParameters().Length == 1)
					DescendantInstance = Activator.CreateInstance(resultType, connection);
				else
					DescendantInstance = Activator.CreateInstance(resultType, connection, true);				
				return DescendantInstance;
			}
			catch(TargetInvocationException tiex) {
				if(SupressExceptions)
					return null;
				if(tiex.InnerException != null)
					throw tiex.InnerException;
				throw;
			}
			catch(Exception) {
				if(SupressExceptions)
					return null;
				throw;
			}
		}
	   protected virtual Type EmitDbDescendant(TypesCollector typesCollector, Tuple<ConstructorInfo, Type[]> ctorTuple, ModuleBuilder mb, bool isModelFirst, Type dbConfigurationType) {
			TypeBuilder tb = mb.DefineType(descendant.FullName, TypeAttributes.Public, !isModelFirst ? descendant : dbContext);
			ConstructorBuilder cb = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctorTuple.Item2);
			ILGenerator ilG = cb.GetILGenerator();
			ilG.Emit(OpCodes.Ldarg_0);
			ilG.Emit(OpCodes.Ldarg_1);
			if (ctorTuple.Item1.GetParameters().Length == 2)
				ilG.Emit(OpCodes.Ldarg_2);
			ilG.Emit(OpCodes.Call, ctorTuple.Item1);
			ilG.Emit(OpCodes.Ret);
			if (isModelFirst) {
				IEnumerable<PropertyInfo> properties = GetDbSetProperties(descendant);
				foreach (PropertyInfo pi in properties)
					CreateProperty(tb, pi, typesCollector);
			}
			Type resultType = tb.CreateType();
			return resultType;
		}
		protected virtual void SetDbConfiguration(object configuration) {
			Type dbConfigurationType = GetDbConfigurationType();
			MethodInfo setConfiguration = dbConfigurationType.GetMethod("SetConfiguration", BindingFlags.Static | BindingFlags.Public);
			setConfiguration.Invoke(null, new object[] { configuration });
		}
		protected Assembly EntityFrameworkAssembly {
			get { return dbContext.Assembly; }
		}
		protected void ClearDBConfiguration() {
			Type dbConfigurationManagerType = EntityFrameworkAssembly.GetType("System.Data.Entity.Infrastructure.DependencyResolution.DbConfigurationManager");
			FieldInfo configManager = dbConfigurationManagerType.GetField("_configManager", BindingFlags.Static | BindingFlags.NonPublic);
			Type dbConfigurationLoaderType = EntityFrameworkAssembly.GetType("System.Data.Entity.Infrastructure.DependencyResolution.DbConfigurationLoader");
			object dbConfigurationLoader = dbConfigurationLoaderType.GetConstructor(new Type[0]).Invoke(new object[0]);
			Type dbConfigurationFinderType = EntityFrameworkAssembly.GetType("System.Data.Entity.Infrastructure.DependencyResolution.DbConfigurationFinder");
			object dbConfigurationFinder = dbConfigurationFinderType.GetConstructor(new Type[0]).Invoke(new object[0]);
			ConstructorInfo ccor = dbConfigurationManagerType.GetConstructor(new Type[] { dbConfigurationLoaderType, dbConfigurationFinderType });
			object newInstance = ccor.Invoke(new object[] { dbConfigurationLoader, dbConfigurationFinder });
			configManager.SetValue(null, newInstance);
		}
		void ClearMetadataCache() {
			Type metadataCacheType = EntityFrameworkAssembly.GetType("System.Data.Entity.Core.Metadata.Edm.MetadataCache");
			FieldInfo metadataInstanceField = metadataCacheType.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
			object metadataInstance = metadataInstanceField.GetValue(null);
			MethodInfo clearMetadataCacheMethod = metadataCacheType.GetMethod("Clear");
			clearMetadataCacheMethod.Invoke(metadataInstance, new object[] { });
		}
		protected override object CreateModelFirstDbConnection(TypesCollector typesCollector) {
			string dbFilePath = Path.Combine(TempFolder, Constants.DatabaseFileName);
			Type entityBuilderType = EntityFrameworkAssembly.GetType("System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder");
			object entityBuilder = Activator.CreateInstance(entityBuilderType);
			PropertyInfo provider = entityBuilderType.GetProperty("Provider", BindingFlags.Public | BindingFlags.Instance);
			provider.SetValue(entityBuilder, ProviderName, null);
			PropertyInfo providerConnectionString = entityBuilderType.GetProperty("ProviderConnectionString", BindingFlags.Public | BindingFlags.Instance);
			providerConnectionString.SetValue(entityBuilder, GetConnectionString(dbFilePath), null);
			PropertyInfo metadata = entityBuilderType.GetProperty("Metadata", BindingFlags.Public | BindingFlags.Instance);
			metadata.SetValue(entityBuilder, TempFolder, null);
			Type entityConnectionType = EntityFrameworkAssembly.GetType("System.Data.Entity.Core.EntityClient.EntityConnection");
			ConstructorInfo constructor = entityConnectionType.GetConstructor(new Type[] { typeof(string) });
			return constructor.Invoke(new object[] { entityBuilder.ToString() });
		}
	}
}
