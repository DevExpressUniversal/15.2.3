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
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Entity.ProjectModel;
using System.Data.Common;
using DevExpress.Utils;
namespace DevExpress.Entity.Model.DescendantBuilding {
	public class SqlCeDescendantBuilder : EF60DbDescendantBuilderBase {
		protected override bool SupressExceptions { get { return true; } }
		public SqlCeDescendantBuilder(TypesCollector typesCollector, IDXAssemblyInfo servicesAssembly)
			: base(typesCollector, servicesAssembly) {
		}
		public override string ProviderName {
			get { return "System.Data.SqlServerCe.4.0"; }
		}
		public override string SqlProviderServicesTypeName {
			get { return "System.Data.Entity.SqlServerCompact.SqlCeProviderServices"; }
		}
		protected override Type EmitDbConfigurationType(ModuleBuilder moduleBuilder) {
			Type dbConfigurationType = GetDbConfigurationType();
			TypeBuilder typeBuilder = moduleBuilder.DefineType(this.GetType().FullName + "." + "DbConfiguration", TypeAttributes.Public, dbConfigurationType, null);
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[0]);
			ILGenerator ilGenerator = constructorBuilder.GetILGenerator();
			EmitCallToBaseTypeCtor(dbConfigurationType, ilGenerator);
			ilGenerator.Emit(OpCodes.Ldarg_0);
			Type sqlCeProviderServicesType = GetSqlProviderServicesType();
			FieldInfo instanceField = sqlCeProviderServicesType.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
			ilGenerator.Emit(OpCodes.Ldsfld, instanceField);
			ilGenerator.Emit(OpCodes.Ldstr, ProviderName);
			Type singletonDependencyResolverType = this.dbContext.GetAssembly().GetTypes().FirstOrDefault(x => x.FullName.StartsWith("System.Data.Entity.Infrastructure.DependencyResolution.SingletonDependencyResolver"));
			singletonDependencyResolverType = singletonDependencyResolverType.MakeGenericType(typeof(DbProviderServices));
			ConstructorInfo singletonConstructor = singletonDependencyResolverType.GetConstructors().FirstOrDefault(x => {
				ParameterInfo[] parameters = x.GetParameters();
				return parameters != null && parameters.Length == 2 && parameters[1].ParameterType.FullName == typeof(object).FullName;
			});
			ilGenerator.Emit(OpCodes.Newobj, singletonConstructor);
			MethodInfo addDependencyResolverMethod = dbConfigurationType.GetMethod("AddDependencyResolver", BindingFlags.Instance | BindingFlags.NonPublic);
			ilGenerator.Emit(OpCodes.Call, addDependencyResolverMethod);
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldsfld, instanceField);
			EmitCallToAddDefaultResolver(dbConfigurationType, ilGenerator);
			ilGenerator.Emit(OpCodes.Ret);
			return typeBuilder.CreateType();
		}	  
		protected override object CreateDefaultDbConnection(Type dbContextType, TypesCollector typesCollector) {
			Type sqlCeConnectionFactoryType = EntityFrameworkAssembly.GetType("System.Data.Entity.Infrastructure.SqlCeConnectionFactory");
			ConstructorInfo ctor = sqlCeConnectionFactoryType.GetConstructor(new Type[] { typeof(string) });
			object factory = ctor.Invoke(new object[] { ProviderName });
			string dbFilePath = Path.Combine(TempFolder, Constants.DatabaseFileName);
			MethodInfo createConnection = sqlCeConnectionFactoryType.GetMethod("CreateConnection", BindingFlags.Public | BindingFlags.Instance);
			return createConnection.Invoke(factory, new object[] { GetConnectionString(dbFilePath) });
		}
		protected override string GetConnectionString(string dbFilePath) {
			return String.Format("Data Source={0}", dbFilePath);
		}
		public override string ProviderManifestToken {
			get { return "4.0"; }
		}
		protected override void SetDbConfiguration(object configuration) {
			base.SetDbConfiguration(configuration);
			Type internalConfigurationType = EntityFrameworkAssembly.GetType("System.Data.Entity.Infrastructure.DependencyResolution.InternalConfiguration");
			PropertyInfo instanceProperty = internalConfigurationType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
			object instance = instanceProperty.GetValue(null, new object[0]);
			object resolvers = instance.GetType().GetField("_resolvers", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
			object firstResolver = resolvers.GetType().GetField("_firstResolver", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(resolvers);
			object appConfigResolver = GetAppConfigResolver(firstResolver);
			IDictionary providerFactories = appConfigResolver.GetType().GetField("_providerFactories", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(appConfigResolver) as IDictionary;
			Type sqlCeProviderServicesType = GetSqlProviderServicesType();
			FieldInfo instanceField = sqlCeProviderServicesType.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
			if (!providerFactories.Contains(ProviderName))
				providerFactories.Add(ProviderName, instanceField.GetValue(null));
		}
		object GetAppConfigResolver(object firstResolver) {
			object resolvers = firstResolver.GetType().GetProperty("Resolvers", BindingFlags.Instance | BindingFlags.Public).GetValue(firstResolver, new object[0]);
			IEnumerable items = resolvers as IEnumerable;
			return items.Cast<object>().FirstOrDefault(x => x.GetType().FullName == "System.Data.Entity.Infrastructure.DependencyResolution.AppConfigDependencyResolver");
		}
	}
}
