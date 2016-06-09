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
using DevExpress.Utils.Controls;
using DevExpress.Entity.ProjectModel;
using DevExpress.Entity.Model.Metadata;
using DevExpress.Utils;
namespace DevExpress.Entity.Model.DescendantBuilding {
	public abstract class DbDescendantBuilder : DisposableObject {
		protected Type dbContext;
		protected Type descendant;
		string tempFolder;
		TypesCollector typesCollector;
		protected abstract bool SupressExceptions { get;}
		protected DbDescendantBuilder(TypesCollector typesCollector) {			
			this.descendant = typesCollector.DbDescendantType;
			this.dbContext = typesCollector.DbContextType;
			this.typesCollector = typesCollector;
		}
		public string TempFolder {
			get {
				if(this.IsDisposed)
					throw new ObjectDisposedException(typeof(DbDescendantBuilder).Name);
				if(string.IsNullOrEmpty(tempFolder))
					tempFolder = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())).FullName;
				return tempFolder;
			}
		}
		public TypesCollector TypesCollector { get { return typesCollector; } }
		public object DescendantInstance { get; protected set; }
		protected void CreateProperty(TypeBuilder tb, PropertyInfo pi, TypesCollector typesCollector) {
			if (tb == null || pi == null || typesCollector == null || typesCollector.DbSet == null)
				return;
			Type propertyType = pi.PropertyType;
			if (propertyType == null)
				return;
			Type[] genericDbSetTypes = propertyType.GetGenericArguments();
			if (genericDbSetTypes == null || genericDbSetTypes.Length == 0)
				return;
			Type dbSetGenericArgumentType = genericDbSetTypes[0];
			if (dbSetGenericArgumentType == null)
				return;
			Type dbSetType = typesCollector.DbSet.ResolveType();
			if (dbSetType == null)
				return;
			propertyType = dbSetType.MakeGenericType(dbSetGenericArgumentType);
			FieldBuilder fieldBuilder = tb.DefineField("f_" + pi.Name, propertyType, FieldAttributes.Private);
			MethodAttributes gsAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
			MethodBuilder getBuilder = tb.DefineMethod("get_" + pi.Name, gsAttr, propertyType, Type.EmptyTypes);
			ILGenerator getIlGen = getBuilder.GetILGenerator();
			getIlGen.Emit(OpCodes.Ldarg_0);
			getIlGen.Emit(OpCodes.Ldfld, fieldBuilder);
			getIlGen.Emit(OpCodes.Ret);
			MethodBuilder setBuilder = tb.DefineMethod("set_" + pi.Name, gsAttr, null, new Type[] { propertyType });
			ILGenerator setIlGen = setBuilder.GetILGenerator();
			setIlGen.Emit(OpCodes.Ldarg_0);
			setIlGen.Emit(OpCodes.Ldarg_1);
			setIlGen.Emit(OpCodes.Stfld, fieldBuilder);
			setIlGen.Emit(OpCodes.Ret);
			PropertyBuilder pb = tb.DefineProperty(pi.Name, PropertyAttributes.HasDefault, propertyType, null);
			pb.SetGetMethod(getBuilder);
			pb.SetSetMethod(setBuilder);
		}
		protected Type FindEFType(string typeName) {
			return this.dbContext.GetAssembly().GetTypes().FirstOrDefault(x => x.FullName.StartsWith(typeName));
		}
		protected IEnumerable<PropertyInfo> GetDbSetProperties(Type type) {
			if (type == null)
				return null;
			PropertyInfo[] properties = type.GetProperties();
			if (properties == null)
				return null;
			return properties.Where(p => {
				if (!p.PropertyType.IsGenericType() || !p.PropertyType.FullName.StartsWith(Constants.DbSetTypeName))
					return false;
				Type[] genericArguments = p.PropertyType.GetGenericArguments();
				return genericArguments != null && genericArguments.Length == 1;
			});
		}
		protected Tuple<ConstructorInfo, Type[]> GetDbContextConstructor(object connection, Type dbContextType) {
			if (connection is string)
				return new Tuple<ConstructorInfo, Type[]>(dbContextType.GetConstructors().Where(a => a.GetParameters().Length == 1
				  && a.GetParameters()[0].ParameterType == typeof(string)).First<ConstructorInfo>(), new Type[] { typeof(string) });
			if (connection == null || connection is DbConnection)
				return new Tuple<ConstructorInfo, Type[]>(dbContextType.GetConstructors().Where(a => a.GetParameters().Length == 2
				  && a.GetParameters()[0].ParameterType == typeof(DbConnection)
				  && a.GetParameters()[1].ParameterType == typeof(bool)).First<ConstructorInfo>(), new Type[] { typeof(DbConnection), typeof(bool) });
			return null;
		}
		protected virtual object CreateDbConnection(Type dbContextType, bool isModelFirst, TypesCollector typesCollector) {
			if (isModelFirst)
				return CreateModelFirstDbConnection(typesCollector);
			else
				return CreateDefaultDbConnection(dbContextType, typesCollector);
		}
		protected abstract object CreateDefaultDbConnection(Type dbContextType, TypesCollector typesCollector);
		protected abstract object CreateModelFirstDbConnection(TypesCollector typesCollector);
		protected virtual void PrepareEdmx(EdmxResource edmxResource) {
			if (edmxResource == null)
				return;
			edmxResource.WriteResources(TempFolder);
		}
		protected virtual Type EmitDbDescendant(TypesCollector typesCollector, Tuple<ConstructorInfo, Type[]> ctorTuple, ModuleBuilder mb, bool isModelFirst) {
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
		protected ModuleBuilder CreateDynamicAssembly() {
			AssemblyName an = GetDynamicAssemblyName();
			AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave, TempFolder);
			string moduleName = an.Name + ".dll";
			return ab.DefineDynamicModule(an.Name, moduleName);
		}
		protected AssemblyName GetDynamicAssemblyName() {
			return new AssemblyName("Assembly" + descendant.Name);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing)
				Clear();
		}
		protected virtual void Clear() {
			DeleteDatabase(this.DescendantInstance);
			DeleteTempFolder();
			typesCollector = null;
		}
		protected void DeleteTempFolder() {
			if (string.IsNullOrEmpty(tempFolder) || !Directory.Exists(tempFolder))
				return;
			try {
				string[] files = Directory.GetFiles(tempFolder);
				foreach (string file in files)
					File.Delete(file);
				Directory.Delete(tempFolder, true);
			}
			catch { }
			finally {
				tempFolder = null;
			}
		}
		protected void DeleteDatabase(object dbContextInstance) {
			if (dbContextInstance == null)
				return;
			try {
				object database = PropertyAccessor.GetValue(dbContextInstance, "Database");
				if (database == null)
					return;
				MethodInfo delete = database.GetType().GetMethod("Delete", new Type[0]);
				if (delete != null)
					delete.Invoke(database, null);
			}
			catch { }
		}
		public virtual object Build() {
			DescendantInstance = null;
			IDXTypeInfo typeInfo = TypesCollector.DbDescendantInfo;
			if (typeInfo == null)
				return null;
			object connection = null;
			EdmxResource edmxResource = EdmxResource.GetEdmxResource(typeInfo);
			bool isModelFirst = edmxResource != null;
			connection = CreateDbConnection(dbContext, isModelFirst, typesCollector);
			if (connection == null)
				return null;
			try {
				Tuple<ConstructorInfo, Type[]> ctrTuple = GetDbContextConstructor(connection, dbContext);
				if(ctrTuple == null)
					return null;
				ModuleBuilder mb = CreateDynamicAssembly();
				Type resultType = EmitDbDescendant(typesCollector, ctrTuple, mb, isModelFirst);
				if(isModelFirst)
					PrepareEdmx(edmxResource);
				if(ctrTuple.Item1.GetParameters().Length == 1)
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
	}
}
