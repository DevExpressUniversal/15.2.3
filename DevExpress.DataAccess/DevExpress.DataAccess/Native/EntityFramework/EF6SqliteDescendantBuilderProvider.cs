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
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DevExpress.Entity.Model;
using DevExpress.Entity.Model.DescendantBuilding;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Native.EntityFramework {
	public class EF6SqliteDescendantBuilderProvider : DataAccessDescendantBuilderProvider {
		protected override string ExpectedProviderName { get { return "System.Data.SQLite.EF6"; } }
		DbDescendantBuilder builder;
		readonly DataAccessEntityFrameworkModel model;
		public EF6SqliteDescendantBuilderProvider() {
		}
		public EF6SqliteDescendantBuilderProvider(DataAccessEntityFrameworkModel model) {
			this.model = model;
		}
		public override DbDescendantBuilder GetBuilder(TypesCollector typesCollector, ISolutionTypesProvider typesProvider) {
			IDXAssemblyInfo sqliteEFAssemblyInfo = typesProvider.GetAssembly(Constants.EntityFrameworkSqliteAssemblyName);
			if(sqliteEFAssemblyInfo == null) {
				this.model.Exceptions.Add(new ApplicationException(string.Format("Cannot load the specified assembly: \"{0}\".", Constants.EntityFrameworkSqliteAssemblyName)));
			}
			IDXAssemblyInfo sqliteAssemblyInfo = typesProvider.GetAssembly(Constants.SqliteAssemblyName);
			if(sqliteAssemblyInfo == null) {
				this.model.Exceptions.Add(new ApplicationException(string.Format("Cannot load the specified assembly: \"{0}\".", Constants.SqliteAssemblyName)));
			}
			this.builder = new RuntimeEF6SqliteDescendantBuilder(this.model, typesCollector, sqliteEFAssemblyInfo, sqliteAssemblyInfo);
			return this.builder;
		}
	}
	public class RuntimeEF6SqliteDescendantBuilder : RuntimeDescendantBuilder {
		IDXAssemblyInfo sqliteAssembly;
		public override string ProviderName { get { return "System.Data.SQLite.EF6"; } }
		public override string SqlProviderServicesTypeName { get { return "System.Data.SQLite.EF6.SQLiteProviderServices"; } }
		public override string ProviderManifestToken
		{
			get { return "2008"; } 
		}
		public RuntimeEF6SqliteDescendantBuilder(DataAccessEntityFrameworkModel model, TypesCollector typesCollector, IDXAssemblyInfo servicesAssembly, IDXAssemblyInfo sqliteAssembly)
			: base(model, typesCollector, servicesAssembly) {
			this.sqliteAssembly = sqliteAssembly;
		}
		protected override Type EmitDbConfigurationType(ModuleBuilder moduleBuilder) {
			Type dbConfigurationType = GetDbConfigurationType();
			Type dependencyResolver = GetDependencyResolverType(moduleBuilder);
			TypeBuilder typeBuilder = moduleBuilder.DefineType(GetType().FullName + "." + "DbConfiguration", TypeAttributes.Public, dbConfigurationType, null);
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[0]);
			ILGenerator ilGenerator = constructorBuilder.GetILGenerator();
			EmitCallToBaseTypeCtor(dbConfigurationType, ilGenerator);
			ilGenerator.Emit(OpCodes.Nop);
			ilGenerator.Emit(OpCodes.Nop);
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Newobj, dependencyResolver.GetConstructor(new Type[] {}));
			ilGenerator.Emit(OpCodes.Call, dbConfigurationType.GetMethod("AddDependencyResolver", BindingFlags.Instance | BindingFlags.NonPublic));
			ilGenerator.Emit(OpCodes.Nop);
			ilGenerator.Emit(OpCodes.Ret);
			return typeBuilder.CreateType();
		}
		Type GetSQLiteFactoryType() {
			return this.sqliteAssembly.GetTypeInfo("System.Data.SQLite.SQLiteFactory").ResolveType();
		}
		Type GetSqliteProviderServicesType() {
			IDXTypeInfo type = this.servicesAssembly.TypesInfo.FirstOrDefault();
			if(type == null)
				return Type.GetType("System.Data.SQLite.EF6.SQLiteProviderServices, System.Data.SQLite.EF6");
			return type.ResolveType().Assembly.GetType("System.Data.SQLite.EF6.SQLiteProviderServices");
		}
		Type GetSQLiteProviderFactoryType() {
			return this.servicesAssembly.GetTypeInfo("System.Data.SQLite.EF6.SQLiteProviderFactory").ResolveType();
		}
		Type GetDependencyResolverType(ModuleBuilder moduleBuilder) {
			Type dependencyResolverType = GetIDbDependencyResolverType();
			TypeBuilder typeBuilder = moduleBuilder.DefineType(GetType().FullName + ".DependencyResolver", TypeAttributes.Public, typeof(object), null);
			typeBuilder.AddInterfaceImplementation(dependencyResolverType);
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[0]);
			ILGenerator ilGenerator = constructorBuilder.GetILGenerator();
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ConstructorInfo baseTypeCtor = typeof(object).GetConstructors(BindingFlags.Instance | BindingFlags.Public).First();
			ilGenerator.Emit(OpCodes.Call, baseTypeCtor);
			ilGenerator.Emit(OpCodes.Nop);
			ilGenerator.Emit(OpCodes.Ret);
			Type sqlProviderInvariantTypeName = BuildSqlProviderInvariantTypeNameType(moduleBuilder);
			MethodInfo getServiceMethod = BuildMethodGetService(typeBuilder, sqlProviderInvariantTypeName);
			BuildMethodGetServices(typeBuilder, getServiceMethod);
			return typeBuilder.CreateType();
		}
		Type BuildSqlProviderInvariantTypeNameType(ModuleBuilder moduleBuilder) {
			Type iProviderInvariantNameType = GetIProviderInvariantNameType();
			TypeBuilder type = moduleBuilder.DefineType(GetType().FullName + ".SqliteProviderInvariantName", TypeAttributes.Public, typeof(object), new Type[] {iProviderInvariantNameType});
			ConstructorBuilder constructorBuilder = type.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard, new Type[] {});
			ILGenerator gen = constructorBuilder.GetILGenerator();
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Call, typeof(object).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] {}, null));
			gen.Emit(OpCodes.Nop);
			gen.Emit(OpCodes.Ret);
			MethodBuilder method = type.DefineMethod("get_Name", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot);
			method.SetReturnType(typeof(string));
			gen = method.GetILGenerator();
			LocalBuilder str = gen.DeclareLocal(typeof(string));
			Label label9 = gen.DefineLabel();
			gen.Emit(OpCodes.Nop);
			gen.Emit(OpCodes.Ldstr, ProviderName);
			gen.Emit(OpCodes.Stloc_0);
			gen.Emit(OpCodes.Br_S, label9);
			gen.MarkLabel(label9);
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Ret);
			return type.CreateType();
		}
		MethodBuilder BuildMethodGetService(TypeBuilder typeBuilder, Type sqlProviderInvariantNameType) {
			Type sqliteFactoryType = GetSQLiteFactoryType();
			Type sqliteProviderFactoryType = GetSQLiteProviderFactoryType();
			Type iProviderInvariantNameType = GetIProviderInvariantNameType();
			Type dbProviderServicesType = FindEFType("System.Data.Entity.Core.Common.DbProviderServices");
			Type sqliteProviderServicesType = GetSqliteProviderServicesType();
			MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot;
			MethodBuilder method = typeBuilder.DefineMethod("GetService", methodAttributes);
			MethodInfo getTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] {typeof(RuntimeTypeHandle)}, null);
			MethodInfo opEquality = typeof(Type).GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] {typeof(Type), typeof(Type)}, null);
			MethodInfo getTypeMethod = typeof(Type).GetMethod("GetType", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] {typeof(String)}, null);
			MethodInfo getFieldMethod = typeof(Type).GetMethod("GetField", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] {typeof(String), typeof(BindingFlags)}, null);
			MethodInfo getValueMethod = typeof(FieldInfo).GetMethod("GetValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] {typeof(Object)}, null);
			ConstructorInfo ctor6 = sqliteProviderFactoryType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] {}, null);
			ConstructorInfo ctor7 = sqlProviderInvariantNameType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] {}, null);
			method.SetReturnType(typeof(Object));
			method.SetParameters(typeof(Type), typeof(Object));
			ParameterBuilder type = method.DefineParameter(1, ParameterAttributes.None, "type");
			ParameterBuilder key = method.DefineParameter(2, ParameterAttributes.None, "key");
			ILGenerator gen = method.GetILGenerator();
			LocalBuilder flag = gen.DeclareLocal(typeof(Boolean));
			LocalBuilder type2 = gen.DeclareLocal(typeof(Type));
			LocalBuilder info = gen.DeclareLocal(typeof(FieldInfo));
			LocalBuilder obj2 = gen.DeclareLocal(typeof(Object));
			Label label1 = gen.DefineLabel();
			Label label2 = gen.DefineLabel();
			Label label3 = gen.DefineLabel();
			Label label4 = gen.DefineLabel();
			Label label5 = gen.DefineLabel();
			Label label6 = gen.DefineLabel();
			gen.Emit(OpCodes.Nop);
			gen.Emit(OpCodes.Ldarg_1);
			gen.Emit(OpCodes.Ldtoken, dbProviderServicesType);
			gen.Emit(OpCodes.Call, getTypeFromHandleMethod);
			gen.Emit(OpCodes.Call, opEquality);
			gen.Emit(OpCodes.Ldc_I4_0);
			gen.Emit(OpCodes.Ceq);
			gen.Emit(OpCodes.Stloc_0);
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Brtrue_S, label1);
			gen.Emit(OpCodes.Nop);
			gen.Emit(OpCodes.Ldtoken, sqliteProviderServicesType);
			gen.Emit(OpCodes.Call, getTypeFromHandleMethod);
			gen.Emit(OpCodes.Stloc_1);
			gen.Emit(OpCodes.Ldloc_1);
			gen.Emit(OpCodes.Ldstr, "Instance");
			gen.Emit(OpCodes.Ldc_I4_S, 40);
			gen.Emit(OpCodes.Callvirt, getFieldMethod);
			gen.Emit(OpCodes.Stloc_2);
			gen.Emit(OpCodes.Ldloc_2);
			gen.Emit(OpCodes.Ldnull);
			gen.Emit(OpCodes.Callvirt, getValueMethod);
			gen.Emit(OpCodes.Castclass, dbProviderServicesType);
			gen.Emit(OpCodes.Stloc_3);
			gen.Emit(OpCodes.Br_S, label2);
			gen.MarkLabel(label1);
			gen.Emit(OpCodes.Ldarg_1);
			gen.Emit(OpCodes.Ldtoken, typeof(DbProviderFactory));
			gen.Emit(OpCodes.Call, getTypeFromHandleMethod);
			gen.Emit(OpCodes.Call, opEquality);
			gen.Emit(OpCodes.Ldc_I4_0);
			gen.Emit(OpCodes.Ceq);
			gen.Emit(OpCodes.Stloc_0);
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Brtrue_S, label3);
			gen.Emit(OpCodes.Newobj, ctor6);
			gen.Emit(OpCodes.Stloc_3);
			gen.Emit(OpCodes.Br_S, label2);
			gen.MarkLabel(label3);
			gen.Emit(OpCodes.Ldarg_1);
			gen.Emit(OpCodes.Ldtoken, iProviderInvariantNameType);
			gen.Emit(OpCodes.Call, getTypeFromHandleMethod);
			gen.Emit(OpCodes.Call, opEquality);
			gen.Emit(OpCodes.Ldc_I4_0);
			gen.Emit(OpCodes.Ceq);
			gen.Emit(OpCodes.Stloc_0);
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Brtrue_S, label6);
			gen.Emit(OpCodes.Newobj, ctor7);
			gen.Emit(OpCodes.Stloc_3);
			gen.Emit(OpCodes.Br_S, label2);
			gen.MarkLabel(label6);
			gen.Emit(OpCodes.Ldnull);
			gen.Emit(OpCodes.Stloc_3);
			gen.Emit(OpCodes.Br_S, label2);
			gen.MarkLabel(label2);
			gen.Emit(OpCodes.Ldloc_3);
			gen.Emit(OpCodes.Ret);
			return method;
		}
		public MethodBuilder BuildMethodGetServices(TypeBuilder typeBuilder, MethodInfo getServiceMethod) {
			MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot;
			MethodBuilder method = typeBuilder.DefineMethod("GetServices", methodAttributes);
			method.SetReturnType(typeof(IEnumerable<>).MakeGenericType(typeof(object)));
			method.SetParameters(typeof(Type), typeof(object));
			ParameterBuilder type = method.DefineParameter(1, ParameterAttributes.None, "type");
			ParameterBuilder key = method.DefineParameter(2, ParameterAttributes.None, "key");
			ILGenerator gen = method.GetILGenerator();
			gen.DeclareLocal(typeof(object));
			gen.DeclareLocal(typeof(bool));
			LocalBuilder enumerable = gen.DeclareLocal(typeof(IEnumerable<>).MakeGenericType(typeof(object)));
			Label label1 = gen.DefineLabel();
			Label label2 = gen.DefineLabel();
			gen.Emit(OpCodes.Nop);
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldarg_1);
			gen.Emit(OpCodes.Ldarg_2);
			gen.Emit(OpCodes.Call, getServiceMethod);
			gen.Emit(OpCodes.Stloc_0);
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Ldnull);
			gen.Emit(OpCodes.Ceq);
			gen.Emit(OpCodes.Stloc_1);
			gen.Emit(OpCodes.Ldloc_1);
			gen.Emit(OpCodes.Brtrue_S, label1);
			gen.Emit(OpCodes.Nop);
			gen.Emit(OpCodes.Ldc_I4_1);
			gen.Emit(OpCodes.Newarr, typeof(object));
			gen.Emit(OpCodes.Dup);
			gen.Emit(OpCodes.Ldc_I4_0);
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Stelem_Ref);
			gen.Emit(OpCodes.Stloc_2);
			gen.Emit(OpCodes.Br_S, label2);
			gen.MarkLabel(label1);
			gen.Emit(OpCodes.Call, typeof(Enumerable).GetMethod("Empty", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] {}, null).MakeGenericMethod(new Type[] {typeof(object)}));
			gen.Emit(OpCodes.Stloc_2);
			gen.Emit(OpCodes.Br_S, label2);
			gen.MarkLabel(label2);
			gen.Emit(OpCodes.Ldloc_2);
			gen.Emit(OpCodes.Ret);
			return method;
		}
	}
}
