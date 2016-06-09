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
using System.Reflection.Emit;
using DevExpress.Entity.Model;
using DevExpress.Entity.Model.DescendantBuilding;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Native.EntityFramework {
	public class EF6MySqlDescendantBuilderProvider : DataAccessDescendantBuilderProvider {
		protected override string ExpectedProviderName { get { return "MySql.Data.MySqlClient"; } }		
		DbDescendantBuilder builder;
		readonly DataAccessEntityFrameworkModel model;
		public EF6MySqlDescendantBuilderProvider() {
		}
		public EF6MySqlDescendantBuilderProvider(DataAccessEntityFrameworkModel model) {
			this.model = model;
		}
		public override DbDescendantBuilder GetBuilder(TypesCollector typesCollector, ISolutionTypesProvider typesProvider) {
			IDXAssemblyInfo assemblyInfo = typesProvider.GetAssembly(Constants.EntityFrameworkMySqlClientAssemblyName);
			if(assemblyInfo == null) {
				this.model.Exceptions.Add(new ApplicationException(string.Format("Cannot load the specified assembly: \"{0}\".", Constants.EntityFrameworkSqlClientAssemblyName)));
			}
			this.builder = new EF6MySqlDbDescendantBuilder(this.model, typesCollector, assemblyInfo);
			return this.builder;
		}
	}
	public class EF6MySqlDbDescendantBuilder : RuntimeDescendantBuilder {
		public override string ProviderName {
			get { return "MySql.Data.MySqlClient"; }
		}
		public override string SqlProviderServicesTypeName {
			get {
				return "MySql.Data.MySqlClient.MySqlProviderServices";
			}
		}
		public override string ProviderManifestToken {
			get { return "2008"; } 
		}
		public EF6MySqlDbDescendantBuilder(DataAccessEntityFrameworkModel model, TypesCollector typesCollector, IDXAssemblyInfo dxAssemblyInfo)
			: base(model, typesCollector, dxAssemblyInfo) {
		}
		protected override Type EmitDbConfigurationType(ModuleBuilder moduleBuilder) {
			Type dbConfigurationType = GetDbConfigurationType();
			TypeBuilder typeBuilder = moduleBuilder.DefineType(this.GetType().FullName + "." + "DbConfiguration", TypeAttributes.Public, dbConfigurationType, null);
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[0]);
			ILGenerator ilGenerator = constructorBuilder.GetILGenerator();
			Type sqlProviderServicesType = GetSqlProviderServicesType();
			Type singletonDependencyResolverType = GetActivatedSingletonDependencyResolverType();
			ilGenerator.DeclareLocal(sqlProviderServicesType);
			EmitCallToBaseTypeCtor(dbConfigurationType, ilGenerator);
			ConstructorInfo ctor = sqlProviderServicesType.GetConstructor(new Type[0]);
			ilGenerator.Emit(OpCodes.Newobj, ctor);
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
	}
}
