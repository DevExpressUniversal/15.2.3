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
using System.Data.EntityClient;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DevExpress.Entity.Model;
using DevExpress.Entity.Model.DescendantBuilding;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Native.EntityFramework {
	public class EF5MsSqlDescendantBuilderProvider : DefaultDescendantBuilderProvider {
		DbDescendantBuilder builder;
		readonly DataAccessEntityFrameworkModel model;
		public EF5MsSqlDescendantBuilderProvider() {
		}
		public EF5MsSqlDescendantBuilderProvider(DataAccessEntityFrameworkModel model) {
			this.model = model;
		}
		public override DbDescendantBuilder GetBuilder(TypesCollector typesCollector, ISolutionTypesProvider typesProvider) {
			this.builder = new RuntimeEF5DbDescendantBuilder(this.model, typesCollector);
			return this.builder;
		}
	}
	public class RuntimeEF5DbDescendantBuilder : DefaultDescendantBuilder {
		internal DataAccessEntityFrameworkModel Model { get; set; }
		protected Assembly EntityFrameworkAssembly { get { return this.dbContext.Assembly; } }
		protected override bool SupressExceptions { get { return false; } }
		public RuntimeEF5DbDescendantBuilder(DataAccessEntityFrameworkModel model, TypesCollector typesCollector)
			: base(typesCollector) {
			Model = model;
		}
		public override object Build() {
			IDXTypeInfo typeInfo = TypesCollector.DbDescendantInfo;
			if(typeInfo == null)
				return null;
			Type type = typeInfo.ResolveType();
			ConstructorInfo[] constructors = type.GetConstructors();
			if(string.IsNullOrEmpty(Model.ConnectionString)) {
				ConstructorInfo info = constructors.FirstOrDefault(c => c.GetParameters().Length == 0);
				if(info != null)
					DescendantInstance = info.Invoke(new object[] {});
			} else {
				ConstructorInfo info = constructors.FirstOrDefault(c => {
					ParameterInfo[] parameters = c.GetParameters();
					return parameters.Length == 1 && parameters[0].ParameterType == typeof(string);
				});
				if(info != null) {
					DescendantInstance = info.Invoke(new object[] {Model.ConnectionString});
				} else
					base.Build();
			}
			Model.ContextInstance = DescendantInstance;
			return DescendantInstance;
		}
		protected override object CreateDbConnection(Type dbContextType, bool isModelFirst, TypesCollector typesCollector) {
			return Model.ConnectionString;
		}
		protected override object CreateDefaultDbConnection(Type dbContextType, TypesCollector typesCollector) {
			Type sqlConnectionFactoryType = EntityFrameworkAssembly.GetType("System.Data.Entity.Infrastructure.SqlConnectionFactory");
			ConstructorInfo ctor = sqlConnectionFactoryType.GetConstructor(new Type[0]);
			object factory = ctor.Invoke(new object[0]);
			MethodInfo createConnection = sqlConnectionFactoryType.GetMethod("CreateConnection", BindingFlags.Public | BindingFlags.Instance);
			return createConnection.Invoke(factory, new object[] {Model.ConnectionString});
		}
		protected override object CreateModelFirstDbConnection(TypesCollector typesCollector) {
			string metadataString = null;
			EntityConnectionStringBuilder entityBuilder = null;
			PropertyInfo metadata = typeof(EntityConnectionStringBuilder).GetProperty("Metadata", BindingFlags.Public | BindingFlags.Instance);
			try {
				entityBuilder = new EntityConnectionStringBuilder(Model.ConnectionString);
				metadataString = (string)metadata.GetValue(entityBuilder, new object[] {});
			} catch {
			}
			if(string.IsNullOrWhiteSpace(metadataString)) {
				entityBuilder = new EntityConnectionStringBuilder();
				entityBuilder.ProviderConnectionString = Model.ConnectionString;
				entityBuilder.Provider = "System.Data.SqlClient";
				entityBuilder.Metadata = TempFolder;
			}
			return entityBuilder.ToString();
		}
		protected override void Clear() {
			DeleteTempFolder();
		}
		protected override Type EmitDbDescendant(TypesCollector typesCollector, Tuple<ConstructorInfo, Type[]> ctorTuple, ModuleBuilder mb, bool isModelFirst) {
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
