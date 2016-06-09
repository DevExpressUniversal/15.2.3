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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Reflection.Emit;
	using BF = System.Reflection.BindingFlags;
	using MA = System.Reflection.MethodAttributes;
	using TA = System.Reflection.TypeAttributes;
	public interface IEndUserFilteringViewModelTypeBuilder {
		Type Create(Type baseType, IEndUserFilteringViewModelProperties properties, IEndUserFilteringViewModelPropertyValues values);
	}
	sealed class DefaultEndUserFilteringViewModelTypeBuilder : IEndUserFilteringViewModelTypeBuilder {
		internal static readonly IEndUserFilteringViewModelTypeBuilder Instance = new DefaultEndUserFilteringViewModelTypeBuilder();
		DefaultEndUserFilteringViewModelTypeBuilder() { }
		static readonly string dynamicSuffix = ".Dynamic." + Guid.NewGuid().ToString();
		internal const string FilteringViewModel = "FilteringViewModel";
		static class NestedPropertiesHelper {
			internal static bool HasRootPath(string path) {
				if(string.IsNullOrEmpty(path)) return false;
				return path.IndexOf('.') > 0;
			}
			internal static string GetRootPath(ref string path) {
				if(string.IsNullOrEmpty(path)) return null;
				int pathSeparatorPos = path.IndexOf('.');
				if(pathSeparatorPos > 0) {
					string rootPath = path.Substring(0, pathSeparatorPos);
					path = path.Substring(pathSeparatorPos + 1);
					return rootPath;
				}
				return null;
			}
		}
		Type IEndUserFilteringViewModelTypeBuilder.Create(Type baseType, IEndUserFilteringViewModelProperties properties, IEndUserFilteringViewModelPropertyValues values) {
			return GetTypeOrCache(GetHash(baseType, properties), hash =>
			{
				TypeBuilder typeBuilder = GetTypeBuilder(baseType, hash);
				baseType = baseType ?? typeof(object);
				var valuesField = DefineValuesField(typeBuilder, baseType);
				BuildProperties(properties, values, valuesField, typeBuilder, baseType);
				ImplementIEndUserFilteringViewModel(typeBuilder, valuesField);
				BuildConstructors(baseType, typeBuilder);
				return typeBuilder.CreateType();
			});
		}
		Assembly OnTypeResolve(object sender, ResolveEventArgs args) {
			return args.RequestingAssembly;
		}
		IDictionary<int, Type> typesCache = new Dictionary<int, Type>();
		Type GetTypeOrCache(int hash, Func<int, Type> createType) {
			Type type;
			if(!typesCache.TryGetValue(hash, out type)) {
				type = createType(hash);
				typesCache.Add(hash, type);
			}
			return type;
		}
#if DEBUGTEST
		internal void Reset() {
			typesCache.Clear();
		}
		internal static string GetDynamicTypeName(string typeName, IEnumerable<KeyValuePair<string, Type>> properties) {
			return GetDynamicTypeName(typeName, GetHash(null, properties));
		}
		internal static string GetDynamicTypeName(Type baseType, IEnumerable<KeyValuePair<string, Type>> properties) {
			return GetDynamicTypeName(baseType, GetHash(baseType, properties));
		}
#endif
		static string GetDynamicTypeName(string typeName, int hash) {
			return MVVM.DynamicTypesHelper.GetDynamicTypeName(typeName, string.Format("{0:X8}", hash));
		}
		static string GetDynamicTypeName(Type baseType, int hash) {
			return MVVM.DynamicTypesHelper.GetDynamicTypeName(baseType, string.Format("{0:X8}", hash));
		}
		internal static int GetHash(Type baseType, IEnumerable<KeyValuePair<string, Type>> properties) {
			List<int> hashes = new List<int>();
			if(baseType != null)
				hashes.Add(baseType.GetHashCode());
			foreach(var p in properties) {
				hashes.Add(p.Key.GetHashCode());
				hashes.Add(p.Value.GetHashCode());
			}
			return HashCodeHelper.CalcHashCode2(hashes.ToArray());
		}
		static TypeBuilder GetTypeBuilder(Type baseType, int hash) {
			var assembly = (baseType != null) ? baseType.Assembly : typeof(IEndUserFilteringViewModelTypeBuilder).Assembly;
			var moduleBuilder = MVVM.DynamicTypesHelper.GetModuleBuilder(assembly);
			return (baseType != null) ?
				moduleBuilder.DefineType(GetDynamicTypeName(baseType, hash), TA.Public, baseType) :
				moduleBuilder.DefineType(GetDynamicTypeName(FilteringViewModel, hash), TA.Public);
		}
		#region Constructors
		void BuildConstructors(Type baseType, TypeBuilder typeBuilder, params FieldInfo[] fields) {
			var cInfos = baseType.GetConstructors(BF.Instance | BF.NonPublic | BF.Public);
			for(int i = 0; i < cInfos.Length; i++)
				CreateConstructor(cInfos[i], typeBuilder, fields);
		}
		void CreateConstructor(ConstructorInfo cInfo, TypeBuilder typeBuilder, FieldInfo[] fields = null) {
			var parameterTypes = GetParameterTypes(cInfo, fields);
			var fieldTypes = (fields != null) ? fields.Select(f => f.FieldType).ToArray() : Type.EmptyTypes;
			var ctorBuilder = typeBuilder.DefineConstructor(MA.Public, CallingConventions.Standard, parameterTypes);
			RegisterBackingFieldConstructor(typeBuilder, ctorBuilder);
			var ctorGenerator = ctorBuilder.GetILGenerator();
			ctorGenerator.Emit(OpCodes.Ldarg_0);
			EmitLdargs(parameterTypes, ctorGenerator, fieldTypes.Length);
			ctorGenerator.Emit(OpCodes.Call, cInfo);
			if(fieldTypes.Length > 0) {
				EmitLdargsAndStfld(fields, ctorGenerator);
			}
			BuildBackingFieldsInitializationForNestedType(typeBuilder, ctorGenerator);
			ctorGenerator.Emit(OpCodes.Ret);
		}
		static Type[] GetParameterTypes(MethodBase method, FieldInfo[] fields) {
			var parameterTypes = method.GetParameters().Select(p => p.ParameterType);
			if(fields != null)
				parameterTypes = parameterTypes.Concat(fields.Select(f => f.FieldType));
			return parameterTypes.ToArray();
		}
		static OpCode[] args = new OpCode[] { OpCodes.Ldarg_1, OpCodes.Ldarg_2, OpCodes.Ldarg_3 };
		static void EmitLdargs(Array parameters, ILGenerator generator, int start = 0) {
			for(int i = start; i < parameters.Length; i++) {
				if(i < 3)
					generator.Emit(args[i]);
				else
					generator.Emit(OpCodes.Ldarg_S, i + 1);
			}
		}
		static void EmitLdargsAndStfld(FieldInfo[] fields, ILGenerator generator) {
			for(int i = 0; i < fields.Length; i++) {
				generator.Emit(OpCodes.Ldarg_0);
				if(i < 3)
					generator.Emit(args[i]);
				else
					generator.Emit(OpCodes.Ldarg_S, i + 1);
				generator.Emit(OpCodes.Stfld, fields[i]);
			}
		}
		#endregion Constructors
		#region IEndUserFilteringViewModel
		void ImplementIEndUserFilteringViewModel(TypeBuilder typeBuilder, FieldInfo valuesField) {
			typeBuilder.AddInterfaceImplementation(typeof(IEndUserFilteringViewModel));
			var method = BuildInitializeMethod(typeBuilder, valuesField);
			typeBuilder.DefineMethodOverride(method,
				typeof(IEndUserFilteringViewModel).GetMethod("Initialize"));
		}
		static FieldInfo DefineValuesField(TypeBuilder typeBuilder, Type baseType) {
			FieldInfo valuesField;
			if(typeof(IEndUserFilteringViewModel).IsAssignableFrom(baseType)) {
				valuesField = baseType.GetFields(BF.Public | BF.NonPublic | BF.Instance)
					.Where(f => f.FieldType == typeof(IEndUserFilteringViewModelPropertyValues)).FirstOrDefault();
				if(valuesField == null)
					Throw(Error_ValuesFieldNotFound, baseType);
			}
			return typeBuilder.DefineField("__values",
				typeof(IEndUserFilteringViewModelPropertyValues), FieldAttributes.Private);
		}
		IDictionary<Type, IList<FieldInfo>> backingFields = new Dictionary<Type, IList<FieldInfo>>();
		IDictionary<Type, IList<ConstructorInfo>> backingFieldConstructors = new Dictionary<Type, IList<ConstructorInfo>>();
		MethodBuilder BuildInitializeMethod(TypeBuilder typeBuilder, FieldInfo valuesField) {
			MethodBuilder method = typeBuilder.DefineMethod(
				typeof(IEndUserFilteringViewModel).FullName + ".Initialize", MA.Private | MA.Virtual | MA.Final | MA.HideBySig | MA.NewSlot);
			method.SetReturnType(typeof(void));
			method.SetParameters(typeof(IEndUserFilteringViewModelPropertyValues));
			method.DefineParameter(1, ParameterAttributes.None, "values");
			var generator = method.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Stfld, valuesField);
			BuildBackingFieldsInitialization(typeBuilder, generator);
			generator.Emit(OpCodes.Ret);
			return method;
		}
		#endregion
		#region Properties
		void BuildProperties(IEndUserFilteringViewModelProperties properties, IEndUserFilteringViewModelPropertyValues values,
			FieldInfo valuesField, TypeBuilder typebuilder, Type baseType, string rootPath = null) {
			var baseProperties = baseType.GetProperties(BF.Public | BF.NonPublic | BF.Instance);
			var baseNestedTypes = baseType.GetNestedTypes(BF.Public | BF.NonPublic);
			foreach(var property in properties) {
				string propertyPath = (rootPath == null) ?
					property.Key : property.Key.Substring(rootPath.Length);
				Type propertyType = property.Value;
				if(!NestedPropertiesHelper.HasRootPath(propertyPath)) {
					var baseProperty = baseProperties.Where(p => p.Name == propertyPath).FirstOrDefault();
					if(baseProperty != null) {
						if(baseProperty.PropertyType != propertyType)
							Throw(Error_PropertyShouldMatchPropertyType, baseType.Name, propertyPath, propertyType.FullName);
						continue;
					}
					var propertyBuilder = BuildProperty(valuesField, typebuilder, propertyPath, propertyType, property.Key);
					BuildPropertyAttributes(propertyBuilder, values[property.Key].Metric);
				}
				else {
					string propertyName = NestedPropertiesHelper.GetRootPath(ref propertyPath);
					var baseNestedType = baseNestedTypes.Where(t => t.Name == propertyName).FirstOrDefault();
					var baseProperty = baseProperties.Where(p => p.Name == propertyName).FirstOrDefault();
					if(baseProperty != null) {
						if(baseProperty.PropertyType != baseNestedType)
							Throw(Error_NestedPropertyShouldMatchNestedType, baseType.Name, propertyPath, baseNestedType.FullName);
					}
					rootPath = (rootPath != null) ? rootPath + propertyName + "." : propertyName + ".";
					var nestedProperties = properties.GetNestedProperties(rootPath);
					GetTypeOrCache(GetHash(baseNestedType, nestedProperties), hash =>
					{
						TypeBuilder nestedTypeBuilder = typebuilder.DefineNestedType(
							GetDynamicTypeName(propertyName, hash), TA.NestedPublic, baseNestedType);
						baseNestedType = baseNestedType ?? typeof(object);
						var nestedValuesField = DefineValuesField(nestedTypeBuilder, baseNestedType);
						BuildProperties(nestedProperties, values.GetNestedValues(rootPath), nestedValuesField, nestedTypeBuilder, baseNestedType, rootPath);
						var backingField = BuildBackingField(typebuilder, propertyName, nestedTypeBuilder);
						var nestedPropertyBuilder = BuildNestedProperty(backingField, typebuilder, propertyName, nestedTypeBuilder);
						BuildNestedPropertyAttributes(nestedPropertyBuilder, values[property.Key].Metric);
						BuildConstructors(baseNestedType, nestedTypeBuilder, nestedValuesField);
						return nestedTypeBuilder.CreateType();
					});
				}
			}
		}
		FieldBuilder BuildBackingField(TypeBuilder typebuilder, string propertyName, Type propertyType) {
			var backingField = typebuilder.DefineField("__BackingField_For_" + propertyName, propertyType, FieldAttributes.Private);
			RegisterBackingField(typebuilder, backingField);
			return backingField;
		}
		void RegisterBackingField(TypeBuilder typebuilder, FieldBuilder backingField) {
			IList<FieldInfo> fields;
			if(!backingFields.TryGetValue(typebuilder, out fields)) {
				fields = new List<FieldInfo>();
				backingFields.Add(typebuilder, fields);
			}
			fields.Add(backingField);
		}
		void RegisterBackingFieldConstructor(TypeBuilder typeBuilder, ConstructorBuilder ctorBuilder) {
			TypeBuilder parentBuilder = typeBuilder.DeclaringType as TypeBuilder;
			if(parentBuilder != null) {
				IList<ConstructorInfo> constructors;
				if(!backingFieldConstructors.TryGetValue(parentBuilder, out constructors)) {
					constructors = new List<ConstructorInfo>();
					backingFieldConstructors.Add(parentBuilder, constructors);
				}
				constructors.Add(ctorBuilder);
			}
		}
		void BuildBackingFieldsInitializationForNestedType(TypeBuilder typeBuilder, ILGenerator generator) {
			TypeBuilder parentBuilder = typeBuilder.DeclaringType as TypeBuilder;
			if(parentBuilder != null) 
				BuildBackingFieldsInitialization(typeBuilder, generator);
		}
		void BuildBackingFieldsInitialization(TypeBuilder typeBuilder, ILGenerator generator) {
			IList<FieldInfo> fields;
			if(backingFields.TryGetValue(typeBuilder, out fields)) {
				foreach(FieldInfo backingField in fields) {
					IList<ConstructorInfo> constructors;
					if(backingFieldConstructors.TryGetValue(typeBuilder, out constructors)) {
						generator.Emit(OpCodes.Ldarg_0);
						generator.Emit(OpCodes.Ldarg_1);
						generator.Emit(OpCodes.Newobj, constructors.First());
						generator.Emit(OpCodes.Stfld, backingField);
					}
				}
			}
		}
		static PropertyBuilder BuildProperty(FieldInfo valuesField, TypeBuilder typebuilder, string propertyName, Type propertyType, string valuePath) {
			var propertyBuilder = typebuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, Type.EmptyTypes);
			var getter = BuildPropertyGetter(valuesField, typebuilder, propertyName, propertyType, valuePath);
			propertyBuilder.SetGetMethod(getter);
			return propertyBuilder;
		}
		static PropertyBuilder BuildNestedProperty(FieldInfo backingField, TypeBuilder typebuilder, string propertyName, Type propertyType) {
			var propertyBuilder = typebuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, Type.EmptyTypes);
			var getter = BuildNestedPropertyGetter(backingField, typebuilder, propertyName, propertyType);
			propertyBuilder.SetGetMethod(getter);
			return propertyBuilder;
		}
		static MethodInfo getItemMethodInfo = typeof(IEndUserFilteringViewModelPropertyValues).GetMethod("get_Item", new Type[] { typeof(string) });
		static MethodInfo getValueMethodInfo = typeof(IEndUserFilteringMetricViewModel).GetMethod("get_Value", Type.EmptyTypes);
		static MethodBuilder BuildPropertyGetter(FieldInfo valuesField, TypeBuilder typeBuilder, string propertyName, Type propertyType, string valuePath) {
			MethodBuilder method = typeBuilder.DefineMethod("get_" + propertyName, MA.Public | MA.HideBySig | MA.SpecialName);
			method.SetReturnType(propertyType);
			ILGenerator gen = method.GetILGenerator();
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, valuesField);
			gen.Emit(OpCodes.Ldstr, valuePath);
			gen.Emit(OpCodes.Callvirt, getItemMethodInfo);
			gen.Emit(OpCodes.Callvirt, getValueMethodInfo);
			gen.Emit(OpCodes.Ret);
			return method;
		}
		static MethodBuilder BuildNestedPropertyGetter(FieldInfo backingField, TypeBuilder typeBuilder, string propertyName, Type propertyType) {
			MethodBuilder method = typeBuilder.DefineMethod("get_" + propertyName, MA.Public | MA.HideBySig | MA.SpecialName);
			method.SetReturnType(propertyType);
			ILGenerator gen = method.GetILGenerator();
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, backingField);
			gen.Emit(OpCodes.Ret);
			return method;
		}
		static void BuildPropertyAttributes(PropertyBuilder propertyBuilder, IEndUserFilteringMetric metric) {
			DisplayAttributeBuilder.Build(metric, true)
				.@Do(a => propertyBuilder.SetCustomAttribute(a));
			FilterEditorAttributeBuilder.Build(metric)
				.@Do(a => propertyBuilder.SetCustomAttribute(a));
			DisplayFormatAttributeBuilder.Build(metric)
				.@Do(a => propertyBuilder.SetCustomAttribute(a));
			DataTypeAttributeBuilder.Build(metric)
				.@Do(a => propertyBuilder.SetCustomAttribute(a));
			FilterPropertyAttributeBuilder.Build(metric)
				.@Do(a => propertyBuilder.SetCustomAttribute(a));
		}
		static void BuildNestedPropertyAttributes(PropertyBuilder propertyBuilder, IEndUserFilteringMetric metric) {
			FilterPropertyAttributeBuilder.Build()
				.@Do(a => propertyBuilder.SetCustomAttribute(a));
		}
		#endregion
		#region Exceptions
		const string Error_ValuesFieldNotFound = "Class already supports IEndUserFilteringViewModel, but field for values is not found: {0}.";
		const string Error_PropertyShouldMatchPropertyType = "Property {0}.{1} should match property type: {2}.";
		const string Error_NestedPropertyShouldMatchNestedType = "Property {0}.{1} should match nested type: {2}.";
		static bool Throw(string format, Type type) {
			throw new EndUserFilteringViewModelTypeBuilderException(string.Format(format, type.Name));
		}
		static bool Throw(string format, params object[] parameters) {
			throw new EndUserFilteringViewModelTypeBuilderException(string.Format(format, parameters));
		}
		class EndUserFilteringViewModelTypeBuilderException : NotSupportedException {
			public EndUserFilteringViewModelTypeBuilderException(string message) : base(message) { }
		}
		#endregion
	}
}
