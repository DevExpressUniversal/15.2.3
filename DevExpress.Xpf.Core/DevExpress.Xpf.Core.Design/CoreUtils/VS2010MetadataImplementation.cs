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

extern alias Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Design;
using Guard = Platform::DevExpress.Utils.Guard;
namespace DevExpress.Xpf.Core.Design {
	public sealed class VS2010MetadataReflectionExtensions : RuntimeBaseCore {
		static VS2010MetadataReflectionExtensions instance;
		public static VS2010MetadataReflectionExtensions Get(Type anyMetadata) {
			if(instance == null)
				instance = new VS2010MetadataReflectionExtensions(anyMetadata.Assembly);
			return instance;
		}
		readonly Assembly metadataAssembly;
		Type reflectionExtensionsType;
		MethodInfo getRuntimeTypeMethod;
		VS2010MetadataReflectionExtensions(Assembly metadataAssembly) {
			this.metadataAssembly = metadataAssembly;
		}
		public Type GetRuntimeType(VS2010TypeMetadata type) {
			return Safe(() => (Type)GetRuntimeTypeMethod.Invoke(null, new object[] { type.Value }), null);
		}
		Type ReflectionExtensionsType {
			get {
				if(reflectionExtensionsType == null)
					reflectionExtensionsType = metadataAssembly.GetType("Microsoft.Windows.Design.Metadata.Reflection.ReflectionExtensions");
				return reflectionExtensionsType;
			}
		}
		MethodInfo GetRuntimeTypeMethod {
			get {
				if(getRuntimeTypeMethod == null)
					getRuntimeTypeMethod = ReflectionExtensionsType.GetMethod("GetRuntimeType", BindingFlags.Static | BindingFlags.Public);
				return getRuntimeTypeMethod;
			}
		}
	}
	public class VS2010MetadataContext : RuntimeBase<IVS2010MetadataContext, object>, IVS2010MetadataContext {
		public static IVS2010MetadataContext Get(object metadataContext) {
			return metadataContext == null ? null : new VS2010MetadataContext(metadataContext);
		}
		static Type iMetadataContext;
		static PropertyInfo localAssemblyProperty;
		static PropertyInfo assembliesProperty;
		protected VS2010MetadataContext(object metadataContext) : base(metadataContext) { }
		public IVS2010AssemblyMetadata LocalAssembly { get { return Safe(() => VS2010AssemblyMetadata.Get(LocalAssemblyProperty.GetValue(Value, null)), null); } }
		public IEnumerable<IVS2010AssemblyMetadata> Assemblies { get { return EnumerateSafe(() => (IEnumerable)AssembliesProperty.GetValue(Value, null)).Select(a => VS2010AssemblyMetadata.Get(a)); } }
		Type IMetadataContext {
			get {
				if(iMetadataContext == null)
					iMetadataContext = Value.GetType().GetInterface("IMetadataContext");
				return iMetadataContext;
			}
		}
		PropertyInfo LocalAssemblyProperty {
			get {
				if(localAssemblyProperty == null)
					localAssemblyProperty = IMetadataContext.GetProperty("LocalAssembly");
				return localAssemblyProperty;
			}
		}
		PropertyInfo AssembliesProperty {
			get {
				if(assembliesProperty == null)
					assembliesProperty = IMetadataContext.GetProperty("Assemblies");
				return assembliesProperty;
			}
		}
	}
	public abstract class VS2010MemberMetadata : RuntimeBase<IVS2010MemberMetadata, object>, IVS2010MemberMetadata {
		static Type iMemberMetadata;
		static PropertyInfo isVisibleProperty;
		static PropertyInfo nameProperty;
		protected VS2010MemberMetadata(object memberMetadata) : base(memberMetadata) { }
		public bool IsVisible { get { return Safe(() => (bool)IsVisibleProperty.GetValue(Value, null), false); } }
		public string Name { get { return Safe(() => (string)NameProperty.GetValue(Value, null), string.Empty); } }
		Type IMemberMetadata {
			get {
				if(iMemberMetadata == null)
					iMemberMetadata = Value.GetType().GetInterface("IMemberMetadata");
				return iMemberMetadata;
			}
		}
		PropertyInfo IsVisibleProperty {
			get {
				if(isVisibleProperty == null)
					isVisibleProperty = IMemberMetadata.GetProperty("IsVisible");
				return isVisibleProperty;
			}
		}
		PropertyInfo NameProperty {
			get {
				if(nameProperty == null)
					nameProperty = IMemberMetadata.GetProperty("Name");
				return nameProperty;
			}
		}
	}
	public class VS2010AssemblyMetadata : RuntimeBase<IVS2010AssemblyMetadata, object>, IVS2010AssemblyMetadata {
		public static IVS2010AssemblyMetadata Get(object assemblyMetadata) {
			return assemblyMetadata == null ? null : new VS2010AssemblyMetadata(assemblyMetadata);
		}
		static Type iAssemblyMetadata;
		static PropertyInfo nameProperty;
		static PropertyInfo fullNameProperty;
		static PropertyInfo typesProperty;
		protected VS2010AssemblyMetadata(object assemblyMetadata) : base(assemblyMetadata) { }
		public string Name { get { return Safe(() => (string)NameProperty.GetValue(Value, null), string.Empty); } }
		public string FullName { get { return Safe(() => (string)FullNameProperty.GetValue(Value, null), string.Empty); } }
		public IEnumerable<IVS2010TypeMetadata> Types { get { return EnumerateSafe(() => (IEnumerable)TypesProperty.GetValue(Value, null)).Select(a => VS2010TypeMetadata.Get(a)); } }
		Type IAssemblyMetadata {
			get {
				if(iAssemblyMetadata == null)
					iAssemblyMetadata = Value.GetType().GetInterface("IAssemblyMetadata");
				return iAssemblyMetadata;
			}
		}
		PropertyInfo NameProperty {
			get {
				if(nameProperty == null)
					nameProperty = IAssemblyMetadata.GetProperty("Name");
				return nameProperty;
			}
		}
		PropertyInfo FullNameProperty {
			get {
				if(fullNameProperty == null)
					fullNameProperty = IAssemblyMetadata.GetProperty("FullName");
				return fullNameProperty;
			}
		}
		PropertyInfo TypesProperty {
			get {
				if(typesProperty == null)
					typesProperty = IAssemblyMetadata.GetProperty("Types");
				return typesProperty;
			}
		}
	}
	public class VS2010TypeMetadata : VS2010MemberMetadata, IVS2010TypeMetadata {
		public static IVS2010TypeMetadata Get(object typeMetadata) {
			return typeMetadata == null ? null : new VS2010TypeMetadata(typeMetadata);
		}
		static Type iTypeMetadata;
		static MethodInfo getConstructorMethod;
		static Array emptyTypesList;
		static PropertyInfo fullNameProperty;
		static PropertyInfo namespaceNameProperty;
		static PropertyInfo isAbstractProperty;
		static PropertyInfo isArrayProperty;
		static PropertyInfo isEnumProperty;
		static PropertyInfo isInterfaceProperty;
		static PropertyInfo isValueTypeProperty;
		static PropertyInfo isPointerProperty;
		static PropertyInfo isGenericTypeProperty;
		static PropertyInfo assemblyProperty;
		static PropertyInfo baseTypeProperty;
		static PropertyInfo interfacesProperty;
		protected VS2010TypeMetadata(object typeMetadata) : base(typeMetadata) { }
		public string FullName { get { return Safe(() => (string)FullNameProperty.GetValue(Value, null), string.Empty); } }
		public string NamespaceName { get { return Safe(() => (string)NamespaceNameProperty.GetValue(Value, null) ?? string.Empty, string.Empty); } }
		public bool IsAbstract { get { return Safe(() => (bool)IsAbstractProperty.GetValue(Value, null), true); } }
		public bool IsArray { get { return Safe(() => (bool)IsArrayProperty.GetValue(Value, null), false); } }
		public bool IsEnum { get { return Safe(() => (bool)IsEnumProperty.GetValue(Value, null), false); } }
		public bool IsInterface { get { return Safe(() => (bool)IsInterfaceProperty.GetValue(Value, null), false); } }
		public bool IsValueType { get { return Safe(() => (bool)IsValueTypeProperty.GetValue(Value, null), false); } }
		public bool IsPointer { get { return Safe(() => (bool)IsPointerProperty.GetValue(Value, null), false); } }
		public bool IsGenericType { get { return Safe(() => (bool)IsGenericTypeProperty.GetValue(Value, null), false); } }
		public IVS2010AssemblyMetadata Assembly { get { return Safe(() => VS2010AssemblyMetadata.Get(AssemblyProperty.GetValue(Value, null)), null); } }
		public IVS2010TypeMetadata BaseType { get { return Safe(() => VS2010TypeMetadata.Get(BaseTypeProperty.GetValue(Value, null)), null); } }
		public IVS2010ConstructorMetadata GetConstructor() { return Safe(() => VS2010ConstructorMetadata.Get(GetConstructorMethod.Invoke(Value, new object[] { EmptyTypesList })), null); }
		public IEnumerable<IVS2010TypeMetadata> Interfaces { get { return EnumerateSafe(() => (IEnumerable)InterfacesProperty.GetValue(Value, null)).Select(a => VS2010TypeMetadata.Get(a)); } }
		public Type GetRuntimeType() { return VS2010MetadataReflectionExtensions.Get(ITypeMetadata).GetRuntimeType(this); }
		Array EmptyTypesList {
			get {
				if(emptyTypesList == null)
					emptyTypesList = Array.CreateInstance(GetConstructorMethod.GetParameters()[0].ParameterType.GetElementType(), 0);
				return emptyTypesList;
			}
		}
		MethodInfo GetConstructorMethod {
			get {
				if(getConstructorMethod == null)
					getConstructorMethod = ITypeMetadata.GetMethod("GetConstructor");
				return getConstructorMethod;
			}
		}
		Type ITypeMetadata {
			get {
				if(iTypeMetadata == null)
					iTypeMetadata = Value.GetType().GetInterface("ITypeMetadata");
				return iTypeMetadata;
			}
		}
		PropertyInfo FullNameProperty {
			get {
				if(fullNameProperty == null)
					fullNameProperty = ITypeMetadata.GetProperty("FullName");
				return fullNameProperty;
			}
		}
		PropertyInfo NamespaceNameProperty {
			get {
				if(namespaceNameProperty == null)
					namespaceNameProperty = ITypeMetadata.GetProperty("NamespaceName");
				return namespaceNameProperty;
			}
		}
		PropertyInfo IsAbstractProperty {
			get {
				if(isAbstractProperty == null)
					isAbstractProperty = ITypeMetadata.GetProperty("IsAbstract");
				return isAbstractProperty;
			}
		}
		PropertyInfo IsArrayProperty {
			get {
				if(isArrayProperty == null)
					isArrayProperty = ITypeMetadata.GetProperty("IsArray");
				return isArrayProperty;
			}
		}
		PropertyInfo IsEnumProperty {
			get {
				if(isEnumProperty == null)
					isEnumProperty = ITypeMetadata.GetProperty("IsEnum");
				return isEnumProperty;
			}
		}
		PropertyInfo IsInterfaceProperty {
			get {
				if(isInterfaceProperty == null)
					isInterfaceProperty = ITypeMetadata.GetProperty("IsInterface");
				return isInterfaceProperty;
			}
		}
		PropertyInfo IsValueTypeProperty {
			get {
				if(isValueTypeProperty == null)
					isValueTypeProperty = ITypeMetadata.GetProperty("IsValueType");
				return isValueTypeProperty;
			}
		}
		PropertyInfo IsPointerProperty {
			get {
				if(isPointerProperty == null)
					isPointerProperty = ITypeMetadata.GetProperty("IsPointer");
				return isPointerProperty;
			}
		}
		PropertyInfo IsGenericTypeProperty {
			get {
				if(isGenericTypeProperty == null)
					isGenericTypeProperty = ITypeMetadata.GetProperty("IsGenericType");
				return isGenericTypeProperty;
			}
		}
		PropertyInfo AssemblyProperty {
			get {
				if(assemblyProperty == null)
					assemblyProperty = ITypeMetadata.GetProperty("Assembly");
				return assemblyProperty;
			}
		}
		PropertyInfo BaseTypeProperty {
			get {
				if(baseTypeProperty == null)
					baseTypeProperty = ITypeMetadata.GetProperty("BaseType");
				return baseTypeProperty;
			}
		}
		PropertyInfo InterfacesProperty {
			get {
				if(interfacesProperty == null)
					interfacesProperty = ITypeMetadata.GetProperty("Interfaces");
				return interfacesProperty;
			}
		}
	}
	public class VS2010ConstructorMetadata : VS2010MemberMetadata, IVS2010ConstructorMetadata {
		public static IVS2010ConstructorMetadata Get(object constructorMetadata) {
			return constructorMetadata == null ? null : new VS2010ConstructorMetadata(constructorMetadata);
		}
		static Type iConstructorMetadata;
		protected VS2010ConstructorMetadata(object constructorMetadata) : base(constructorMetadata) { }
		Type IConstructorMetadata {
			get {
				if(iConstructorMetadata == null)
					iConstructorMetadata = Value.GetType().GetInterface("IConstructorMetadata");
				return iConstructorMetadata;
			}
		}
	}
}
