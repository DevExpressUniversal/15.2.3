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
	public abstract class EF7DescendantBuilderProviderBase : DescendantBuilderProvider {
		readonly DataAccessEntityFrameworkModel model;
		protected IDXAssemblyInfo serviceAssemblyInfo;
		protected abstract string ServiceAssemblyName { get; }
		protected DataAccessEntityFrameworkModel Model { get { return this.model; } }
		protected EF7DescendantBuilderProviderBase() {
		}
		protected EF7DescendantBuilderProviderBase(DataAccessEntityFrameworkModel model) {
			this.model = model;
		}
		public override bool Available(Type dbContext, IDXTypeInfo dbDescendant, ISolutionTypesProvider typesProvider) {
			if(!EntityFrameworkModelBase.IsAtLeastEF7(dbContext))
				return false;
			if(dbDescendant.ResolveType().Assembly.GetReferencedAssemblies().All(n => n.Name != ServiceAssemblyName))
				return false;
			this.serviceAssemblyInfo = typesProvider.GetAssembly(ServiceAssemblyName);
			return this.serviceAssemblyInfo != null;
		}
	}
	public abstract class EF7DescendantBuilderBase : RuntimeDescendantBuilder {
		public override string ProviderName { get { return string.Empty; } }
		public override string SqlProviderServicesTypeName { get { return string.Empty; } }
		public override string ProviderManifestToken { get { return string.Empty; } }
		protected abstract string DbContextOptionsExtensionsTypeName { get; }
		protected abstract string UseServerExtensionName { get; }
		protected EF7DescendantBuilderBase(DataAccessEntityFrameworkModel model, TypesCollector typesCollector, IDXAssemblyInfo dxAssemblyInfo)
			: base(model, typesCollector, dxAssemblyInfo) {
		}
		public override object Build() {
			IDXTypeInfo typeInfo = TypesCollector.DbDescendantInfo;
			if(typeInfo == null)
				return null;
			DescendantInstance = null;
			try {
				Type contextType = typeInfo.ResolveType();
				if(string.IsNullOrWhiteSpace(Model.ConnectionString)) {
					DescendantInstance = CreateContextInstance(contextType);
				} else {
					ModuleBuilder mb = CreateDynamicAssembly();
					TypeBuilder newContextTypeBuilder = mb.DefineType("NewContextType", TypeAttributes.Public, contextType);
					Type dbContextOptionsBuilderType = TypesCollector.DbContextOptionsBuilder;
					MethodBuilder onConfiguringNew = newContextTypeBuilder.DefineMethod("OnConfiguring",
						MethodAttributes.HideBySig
						| MethodAttributes.NewSlot
						| MethodAttributes.Virtual
						| MethodAttributes.Final,
						CallingConventions.HasThis,
						typeof(void),
						new Type[] {dbContextOptionsBuilderType});
					IDXTypeInfo dxTypeInfo = this.servicesAssembly.TypesInfo.FirstOrDefault(t => t.FullName == DbContextOptionsExtensionsTypeName);
					if(dxTypeInfo == null)
						return null;
					Type optionsType = dxTypeInfo.ResolveType();
					MethodInfo onConfiguring = TypesCollector.DbContext.ResolveType().GetMethod("OnConfiguring", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {dbContextOptionsBuilderType}, null);
					MethodInfo useSqlServer = optionsType.GetMethod(UseServerExtensionName, BindingFlags.Public | BindingFlags.Static, null, new[] {dbContextOptionsBuilderType, typeof(string)}, null);
					ILGenerator il = onConfiguringNew.GetILGenerator();
					il.Emit(OpCodes.Nop);
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Call, onConfiguring);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldstr, Model.ConnectionString);
					il.Emit(OpCodes.Call, useSqlServer);
					il.Emit(OpCodes.Pop);
					il.Emit(OpCodes.Ret);
					newContextTypeBuilder.DefineMethodOverride(onConfiguringNew, onConfiguring);
					if(contextType.GetConstructors().All(c => c.GetParameters().Length != 0))
						CreateDefaultConstructor(contextType, newContextTypeBuilder);
					Type resultType = newContextTypeBuilder.CreateType();
					DescendantInstance = Activator.CreateInstance(resultType);
				}
			} catch(TargetInvocationException tiex) {
				if(SupressExceptions)
					return null;
				if(tiex.InnerException != null)
					throw tiex.InnerException;
				throw;
			} catch(Exception) {
				if(SupressExceptions)
					return null;
				throw;
			}
			Model.ContextInstance = DescendantInstance;
			Model.IsEntityFramework7 = true;
			return DescendantInstance;
		}
		object CreateContextInstance(Type contextType) {
			ConstructorInfo[] constructors = contextType.GetConstructors();
			ConstructorInfo info = constructors.FirstOrDefault(c => c.GetParameters().Length == 0);
			if(info != null)
				return info.Invoke(new object[] {});
			ModuleBuilder mb = CreateDynamicAssembly();
			TypeBuilder tb = mb.DefineType(contextType.FullName, TypeAttributes.Public, contextType);
			CreateDefaultConstructor(contextType, tb);
			Type type = tb.CreateType();
			return Activator.CreateInstance(type);
		}
		void CreateDefaultConstructor(Type contextType, TypeBuilder newContextTypeBuilder) {
			Type dbContextOptionsType = TypesCollector.DbContextOptionsT.MakeGenericType(contextType);
			ConstructorInfo dbContextOptionsConstructor = dbContextOptionsType.GetConstructor(new Type[0]);
			ConstructorInfo dbContextDefaultConstructor = TypesCollector.DbContextType.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == TypesCollector.DbContextOptions);
			ConstructorBuilder cb = newContextTypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[0]);
			ILGenerator ilG = cb.GetILGenerator();
			ilG.Emit(OpCodes.Ldarg_0);
			ilG.Emit(OpCodes.Newobj, dbContextOptionsConstructor);
			ilG.Emit(OpCodes.Call, dbContextDefaultConstructor);
			ilG.Emit(OpCodes.Nop);
			ilG.Emit(OpCodes.Nop);
			ilG.Emit(OpCodes.Ret);
		}
	}
}
