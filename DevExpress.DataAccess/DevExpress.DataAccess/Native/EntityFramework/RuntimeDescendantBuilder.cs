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
	public abstract class RuntimeDescendantBuilder : EF60DbDescendantBuilderBase {
		internal DataAccessEntityFrameworkModel Model { get; set; }
		protected override bool SupressExceptions { get { return false; } }
		protected RuntimeDescendantBuilder(DataAccessEntityFrameworkModel model, TypesCollector typesCollector, IDXAssemblyInfo servicesAssembly)
			: base(typesCollector, servicesAssembly) {
			Model = model;
		}
		protected override object CreateDefaultDbConnection(Type dbContextType, TypesCollector typesCollector) {
			Type sqlConnectionFactoryType = EntityFrameworkAssembly.GetType("System.Data.Entity.Infrastructure.SqlConnectionFactory");
			ConstructorInfo ctor = sqlConnectionFactoryType.GetConstructor(new Type[0]);
			object factory = ctor.Invoke(new object[0]);
			MethodInfo createConnection = sqlConnectionFactoryType.GetMethod("CreateConnection", BindingFlags.Public | BindingFlags.Instance);
			return createConnection.Invoke(factory, new object[] {GetConnectionString(null)});
		}
		public override object Build() {
			IDXTypeInfo typeInfo = TypesCollector.DbDescendantInfo;
			if(typeInfo == null)
				return null;
			if(!CheckDefaultConstructors(typeInfo))
				base.Build();
			Model.ContextInstance = DescendantInstance;
			return DescendantInstance;
		}
		protected bool CheckDefaultConstructors(IDXTypeInfo typeInfo) {
			Type type = typeInfo.ResolveType();
			ConstructorInfo[] constructors = type.GetConstructors();
			try {
				if(string.IsNullOrEmpty(Model.ConnectionString)) {
					ConstructorInfo info = constructors.FirstOrDefault(c => c.GetParameters().Length == 0);
					if(info != null) {
						DescendantInstance = info.Invoke(new object[] {});
						return true;
					}
				} else {
					ConstructorInfo info = constructors.FirstOrDefault(c => {
						ParameterInfo[] parameters = c.GetParameters();
						return parameters.Length == 1 && parameters[0].ParameterType == typeof(string);
					});
					if(info != null) {
						DescendantInstance = info.Invoke(new object[] {Model.ConnectionString});
						return true;
					}
				}
			} catch(TypeInitializationException ex) {
				throw ex.InnerException;
			}
			return false;
		}
		protected override string GetConnectionString(string dbFilePath) {
			return Model.ConnectionString;
		}
		protected override object CreateModelFirstDbConnection(TypesCollector typesCollector) {
			string connectionString = GetConnectionString(null);
			Type entityBuilderType = EntityFrameworkAssembly.GetType("System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder");
			string metadataString = null;
			object entityBuilder = null;
			PropertyInfo metadata = entityBuilderType.GetProperty("Metadata", BindingFlags.Public | BindingFlags.Instance);
			try {
				entityBuilder = Activator.CreateInstance(entityBuilderType, new object[] {connectionString});
				metadataString = (string)metadata.GetValue(entityBuilder, new object[] {});
			} catch {
			}
			if(string.IsNullOrEmpty(metadataString)) {
				entityBuilder = Activator.CreateInstance(entityBuilderType);
				PropertyInfo provider = entityBuilderType.GetProperty("Provider", BindingFlags.Public | BindingFlags.Instance);
				provider.SetValue(entityBuilder, ProviderName, null);
				PropertyInfo providerConnectionString = entityBuilderType.GetProperty("ProviderConnectionString", BindingFlags.Public | BindingFlags.Instance);
				providerConnectionString.SetValue(entityBuilder, connectionString, null);
				metadata.SetValue(entityBuilder, TempFolder, null);
			}
			Type entityConnectionType = EntityFrameworkAssembly.GetType("System.Data.Entity.Core.EntityClient.EntityConnection");
			ConstructorInfo constructor = entityConnectionType.GetConstructor(new Type[] {typeof(string)});
			return constructor.Invoke(new object[] {entityBuilder.ToString()});
		}
		protected override void Clear() {
			DeleteTempFolder();
		}
		protected override Type EmitDbDescendant(TypesCollector typesCollector, Tuple<ConstructorInfo, Type[]> ctorTuple, ModuleBuilder mb, bool isModelFirst, Type dbConfigurationType) {
			TypeBuilder tb = mb.DefineType(this.descendant.FullName, TypeAttributes.Public, this.descendant);
			ConstructorBuilder cb = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctorTuple.Item2);
			ILGenerator ilG = cb.GetILGenerator();
			ilG.Emit(OpCodes.Ldarg_0);
			ilG.Emit(OpCodes.Ldarg_1);
			if(ctorTuple.Item1.GetParameters().Length == 2)
				ilG.Emit(OpCodes.Ldarg_2);
			ilG.Emit(OpCodes.Call, ctorTuple.Item1);
			ilG.Emit(OpCodes.Ret);
			Type resultType = tb.CreateType();
			return resultType;
		}
	}
}
