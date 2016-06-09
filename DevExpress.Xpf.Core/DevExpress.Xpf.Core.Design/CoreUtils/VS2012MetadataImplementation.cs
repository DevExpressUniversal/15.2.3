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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Design;
using DevExpress.Design.SmartTags;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Services;
using Microsoft.Windows.Design.Model;
using System.Reflection;
using Microsoft.Windows.Design.Metadata;
using System.Collections;
using DevExpress.Utils;
using Guard = Platform::DevExpress.Utils.Guard;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Xpf.Core.Design {
	public class VS2012TypeResolver : RuntimeBase<IVS2012TypeResolver, object>, IVS2012TypeResolver {
		public static IVS2012TypeResolver Get(object typeResolver) {
			return typeResolver == null ? null : new VS2012TypeResolver(typeResolver);
		}
		static Type iTypeResolver;
		static PropertyInfo projectAssemblyProperty;
		static PropertyInfo assemblyReferencesProperty;
		static MethodInfo ensureAssemblyReferencedMethod;
		static MethodInfo getTypesMethod;
		static MethodInfo getTypeMethod;
		protected VS2012TypeResolver(object typeResolver) : base(typeResolver) { }
		public IVS2012AssemblyMetadata ProjectAssembly { get { return Safe(() => VS2012AssemblyMetadata.Get(ProjectAssemblyProperty.GetValue(Value, null)), null); } }
		public IEnumerable<IVS2012AssemblyMetadata> AssemblyReferences { get { return EnumerateSafe(() => (IEnumerable)AssemblyReferencesProperty.GetValue(Value, null)).Select(a => VS2012AssemblyMetadata.Get(a)); } }
		public bool EnsureAssemblyReferenced(string simpleNameOrPath, bool includeDependencies) { return Safe(() => (bool)EnsureAssemblyReferencedMethod.Invoke(Value, new object[] { simpleNameOrPath, includeDependencies }), true); }
		public IVS2012TypeMetadata GetType(Type type) { return Safe(() => VS2012TypeMetadata.Get(GetTypeMethod.Invoke(Value, new object[] { type })), null); }
		public IEnumerable<IVS2012TypeMetadata> GetTypes(IVS2012AssemblyMetadata assembly) {
			VS2012AssemblyMetadata assemblyMetadata = Guard.ArgumentMatchType<VS2012AssemblyMetadata>(assembly, "assembly");
			if(!assemblyMetadata.IsLoaded) return new IVS2012TypeMetadata[] { };
			return EnumerateSafe(() => GetTypesCore(assemblyMetadata.Value)).Select(t => VS2012TypeMetadata.Get(t));
		}
		IEnumerable GetTypesCore(object assembly) {
			try {
				return (IEnumerable)GetTypesMethod.Invoke(Value, new object[] { assembly, (Action<Exception>)(e => { }) });
			} catch {
				return (IEnumerable)GetTypesMethod.Invoke(Value, new object[] { assembly });
			}
		}
		Type ITypeResolver {
			get {
				if(iTypeResolver == null)
					iTypeResolver = Value.GetType().GetInterface("ITypeResolver");
				return iTypeResolver;
			}
		}
		PropertyInfo ProjectAssemblyProperty {
			get {
				if(projectAssemblyProperty == null)
					projectAssemblyProperty = ITypeResolver.GetProperty("ProjectAssembly");
				return projectAssemblyProperty;
			}
		}
		PropertyInfo AssemblyReferencesProperty {
			get {
				if(assemblyReferencesProperty == null)
					assemblyReferencesProperty = ITypeResolver.GetProperty("AssemblyReferences");
				return assemblyReferencesProperty;
			}
		}
		MethodInfo EnsureAssemblyReferencedMethod {
			get {
				if(ensureAssemblyReferencedMethod == null)
					ensureAssemblyReferencedMethod = ITypeResolver.GetMethod("EnsureAssemblyReferenced");
				return ensureAssemblyReferencedMethod;
			}
		}
		MethodInfo GetTypesMethod {
			get {
				if(getTypesMethod == null)
					getTypesMethod = ITypeResolver.GetMethod("GetTypes");
				return getTypesMethod;
			}
		}
		MethodInfo GetTypeMethod {
			get {
				if(getTypeMethod == null)
					getTypeMethod = ITypeResolver.GetMethod("GetType", new Type[] { typeof(Type) });
				return getTypeMethod;
			}
		}
	}
	public abstract class VS2012MemberMetadata : RuntimeBase<IVS2012MemberMetadata, object>, IVS2012MemberMetadata {
		const int MemberAccessTypePublic = 8;
		static Type iMemberId;
		static Type iMember;
		static PropertyInfo accessProperty;
		static PropertyInfo nameProperty;
		static PropertyInfo fullNameProperty;
		public VS2012MemberMetadata(object memberMetadata) : base(memberMetadata) { }
		public bool IsPublic { get { return Safe(() => (Access & MemberAccessTypePublic) != 0, false); } }
		public string Name { get { return Safe(() => (string)NameProperty.GetValue(Value, null), string.Empty); } }
		public string FullName { get { return Safe(() => (string)FullNameProperty.GetValue(Value, null), string.Empty); } }
		int Access { get { return (int)AccessProperty.GetValue(Value, null); } }
		Type IMemberId {
			get {
				if(iMemberId == null)
					iMemberId = Value.GetType().GetInterface("IMemberId");
				return iMemberId;
			}
		}
		Type IMember {
			get {
				if(iMember == null)
					iMember = Value.GetType().GetInterface("IMember");
				return iMember;
			}
		}
		PropertyInfo AccessProperty {
			get {
				if(accessProperty == null)
					accessProperty = IMember.GetProperty("Access");
				return accessProperty;
			}
		}
		PropertyInfo NameProperty {
			get {
				if(nameProperty == null)
					nameProperty = IMemberId.GetProperty("Name");
				return nameProperty;
			}
		}
		PropertyInfo FullNameProperty {
			get {
				if(fullNameProperty == null)
					fullNameProperty = IMember.GetProperty("FullName");
				return fullNameProperty;
			}
		}
	}
	public class VS2012AssemblyMetadata : RuntimeBase<IVS2012AssemblyMetadata, object>, IVS2012AssemblyMetadata {
		public static IVS2012AssemblyMetadata Get(object assemblyMetadata) {
			return assemblyMetadata == null ? null : new VS2012AssemblyMetadata(assemblyMetadata);
		}
		static Type iAssemblyId;
		static Type iAssembly;
		static PropertyInfo nameProperty;
		static PropertyInfo fullNameProperty;
		static PropertyInfo isLoadedProperty;
		static PropertyInfo locationProperty;
		protected VS2012AssemblyMetadata(object assemblyMetadata) : base(assemblyMetadata) { }
		public string Name { get { return Safe(() => (string)NameProperty.GetValue(Value, null), string.Empty); } }
		public string FullName { get { return Safe(() => (string)FullNameProperty.GetValue(Value, null), string.Empty); } }
		public bool IsLoaded { get { return Safe(() => (bool)IsLoadedProperty.GetValue(Value, null), false); } }
		public string Location { get { return Safe(() => LocationProperty.GetValue(Value, null).ToString(), string.Empty); } }
		Type IAssemblyId {
			get {
				if(iAssemblyId == null)
					iAssemblyId = Value.GetType().GetInterface("IAssemblyId");
				return iAssemblyId;
			}
		}
		Type IAssembly {
			get {
				if(iAssembly == null)
					iAssembly = Value.GetType().GetInterface("IAssembly");
				return iAssembly;
			}
		}
		PropertyInfo NameProperty {
			get {
				if(nameProperty == null)
					nameProperty = IAssemblyId.GetProperty("Name");
				return nameProperty;
			}
		}
		PropertyInfo FullNameProperty {
			get {
				if(fullNameProperty == null)
					fullNameProperty = IAssemblyId.GetProperty("FullName");
				return fullNameProperty;
			}
		}
		PropertyInfo IsLoadedProperty {
			get {
				if(isLoadedProperty == null)
					isLoadedProperty = IAssembly.GetProperty("IsLoaded");
				return isLoadedProperty;
			}
		}
		PropertyInfo LocationProperty {
			get {
				if(locationProperty == null)
					locationProperty = IAssembly.GetProperty("Location");
				return locationProperty;
			}
		}
	}
	public class VS2012TypeMetadata : VS2012MemberMetadata, IVS2012TypeMetadata {
		public static IVS2012TypeMetadata Get(object typeMetadata) {
			return typeMetadata == null ? null : new VS2012TypeMetadata(typeMetadata);
		}
		static Type iTypeId;
		static Type iType;
		static PropertyInfo namespaceProperty;
		static PropertyInfo isAbstractProperty;
		static PropertyInfo isArrayProperty;
		static PropertyInfo isInterfaceProperty;
		static PropertyInfo runtimeTypeProperty;
		static PropertyInfo isGenericTypeProperty;
		static PropertyInfo runtimeAssemblyProperty;
		static PropertyInfo baseTypeProperty;
		static MethodInfo isAssignableFromMethod;
		static MethodInfo hasDefaultConstructorMethod;
		protected VS2012TypeMetadata(object typeMetadata) : base(typeMetadata) { }
		public string Namespace { get { return Safe(() => (string)NamespaceProperty.GetValue(Value, null) ?? string.Empty, string.Empty); } }
		public bool IsAbstract { get { return Safe(() => (bool)IsAbstractProperty.GetValue(Value, null), true); } }
		public bool IsArray { get { return Safe(() => (bool)IsArrayProperty.GetValue(Value, null), false); } }
		public bool IsInterface { get { return Safe(() => (bool)IsInterfaceProperty.GetValue(Value, null), false); } }
		public Type RuntimeType { get { return Safe(() => (Type)RuntimeTypeProperty.GetValue(Value, null), typeof(object)); } }
		public bool IsGenericType { get { return Safe(() => (bool)IsGenericTypeProperty.GetValue(Value, null), false); } }
		public IVS2012AssemblyMetadata RuntimeAssembly { get { return Safe(() => VS2012AssemblyMetadata.Get(RuntimeAssemblyProperty.GetValue(Value, null)), null); } }
		public IVS2012TypeMetadata BaseType { get { return Safe(() => VS2012TypeMetadata.Get(BaseTypeProperty.GetValue(Value, null)), null); } }
		public bool IsAssignableFrom(IVS2012TypeMetadata type) {
			VS2012TypeMetadata typeMetadata = Guard.ArgumentMatchType<VS2012TypeMetadata>(type, "type");
			return Safe(() => (bool)IsAssignableFromMethod.Invoke(Value, new object[] { typeMetadata.Value }), false);
		}
		public bool HasDefaultConstructor { get { return Safe(() => (bool)HasDefaultConstructorMethod.Invoke(Value, new object[] { true }), false); } }
		Type ITypeId {
			get {
				if(iTypeId == null)
					iTypeId = Value.GetType().GetInterface("ITypeId");
				return iTypeId;
			}
		}
		Type IType {
			get {
				if(iType == null)
					iType = Value.GetType().GetInterface("IType");
				return iType;
			}
		}
		PropertyInfo NamespaceProperty {
			get {
				if(namespaceProperty == null)
					namespaceProperty = IType.GetProperty("Namespace");
				return namespaceProperty;
			}
		}
		PropertyInfo IsAbstractProperty {
			get {
				if(isAbstractProperty == null)
					isAbstractProperty = IType.GetProperty("IsAbstract");
				return isAbstractProperty;
			}
		}
		PropertyInfo IsArrayProperty {
			get {
				if(isArrayProperty == null)
					isArrayProperty = IType.GetProperty("IsArray");
				return isArrayProperty;
			}
		}
		PropertyInfo IsInterfaceProperty {
			get {
				if(isInterfaceProperty == null)
					isInterfaceProperty = IType.GetProperty("IsInterface");
				return isInterfaceProperty;
			}
		}
		PropertyInfo RuntimeTypeProperty {
			get {
				if(runtimeTypeProperty == null)
					runtimeTypeProperty = IType.GetProperty("RuntimeType");
				return runtimeTypeProperty;
			}
		}
		PropertyInfo IsGenericTypeProperty {
			get {
				if(isGenericTypeProperty == null)
					isGenericTypeProperty = IType.GetProperty("IsGenericType");
				return isGenericTypeProperty;
			}
		}
		PropertyInfo RuntimeAssemblyProperty {
			get {
				if(runtimeAssemblyProperty == null)
					runtimeAssemblyProperty = IType.GetProperty("RuntimeAssembly");
				return runtimeAssemblyProperty;
			}
		}
		PropertyInfo BaseTypeProperty {
			get {
				if(baseTypeProperty == null)
					baseTypeProperty = IType.GetProperty("BaseType");
				return baseTypeProperty;
			}
		}
		MethodInfo IsAssignableFromMethod {
			get {
				if(isAssignableFromMethod == null)
					isAssignableFromMethod = ITypeId.GetMethod("IsAssignableFrom");
				return isAssignableFromMethod;
			}
		}
		MethodInfo HasDefaultConstructorMethod {
			get {
				if(hasDefaultConstructorMethod == null)
					hasDefaultConstructorMethod = IType.GetMethod("HasDefaultConstructor");
				return hasDefaultConstructorMethod;
			}
		}
	}
	public class VS2012PropertyMetadata : VS2012MemberMetadata, IVS2012PropertyMetadata {
		public static VS2012PropertyMetadata Get(object propertyMetadata) {
			return propertyMetadata == null ? null : new VS2012PropertyMetadata(propertyMetadata);
		}
		protected VS2012PropertyMetadata(object propertyMetadata) : base(propertyMetadata) { }
	}
}
