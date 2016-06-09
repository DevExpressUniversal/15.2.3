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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;
namespace DevExpress.Xpf.Internal.EntityFrameworkWrappers {
	class RuntimeTypesHelper {
		public static PropertyInfo GetProperty(Type type, string name) {
			return type.GetInterfaces()
				.SelectMany(i => i.GetProperties())
				.Concat(type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				.Where(p => p.Name == name)
				.First();
		}
		public static MethodInfo GetMethod(Type type, string name) {
			return type.GetInterfaces()
				.SelectMany(i => i.GetMethods())
				.Concat(type.GetMethods())
				.Where(m => !m.IsGenericMethod && m.Name == name)
				.First();
		}
	}
	interface IWrapper {
		object Object { get; }
	}
	static class Wrapper {
		public static object Wrap(Type type, object obj) {
			if(type == typeof(MetadataPropertyRuntimeWrapper))
				return MetadataPropertyRuntimeWrapper.Wrap(obj);
			if(type == typeof(EntitySetBaseRuntimeWrapper))
				return EntitySetBaseRuntimeWrapper.Wrap(obj);
			if(type == typeof(MetadataItemRuntimeWrapper))
				return MetadataItemRuntimeWrapper.Wrap(obj);
			if(type == typeof(EntityContainerRuntimeWrapper))
				return EntityContainerRuntimeWrapper.Wrap(obj);
			if(type == typeof(GlobalItemRuntimeWrapper))
				return GlobalItemRuntimeWrapper.Wrap(obj);
			if(type == typeof(ItemCollectionRuntimeWrapper))
				return ItemCollectionRuntimeWrapper.Wrap(obj);
			if(type == typeof(MetadataWorkspaceRuntimeWrapper))
				return MetadataWorkspaceRuntimeWrapper.Wrap(obj);
			if(type == typeof(ObjectContextRuntimeWrapper))
				return ObjectContextRuntimeWrapper.Wrap(obj);
			if(type == typeof(DbContextRuntimeWrapper))
				return DbContextRuntimeWrapper.Wrap(obj);
			if(type == typeof(EdmTypeRuntimeWrapper))
				return EdmTypeRuntimeWrapper.Wrap(obj);
			if(type == typeof(TypeUsageRuntimeWrapper))
				return TypeUsageRuntimeWrapper.Wrap(obj);
			if(type == typeof(EdmMemberRuntimeWrapper))
				return EdmMemberRuntimeWrapper.Wrap(obj);
			if(type == typeof(EntityTypeRuntimeWrapper))
				return EntityTypeRuntimeWrapper.Wrap(obj);
			if(type == typeof(RelationshipEndMemberRuntimeWrapper))
				return RelationshipEndMemberRuntimeWrapper.Wrap(obj);
			if(type == typeof(NavigationPropertyRuntimeWrapper))
				return NavigationPropertyRuntimeWrapper.Wrap(obj);
			if(type == typeof(StructuralTypeRuntimeWrapper))
				return StructuralTypeRuntimeWrapper.Wrap(obj);
			if(type == typeof(PrimitiveTypeRuntimeWrapper))
				return PrimitiveTypeRuntimeWrapper.Wrap(obj);
			if(type == typeof(EdmPropertyRuntimeWrapper))
				return EdmPropertyRuntimeWrapper.Wrap(obj);
			if(type == typeof(ReferentialConstraintRuntimeWrapper))
				return ReferentialConstraintRuntimeWrapper.Wrap(obj);
			if(type == typeof(AssociationEndMemberRuntimeWrapper))
				return AssociationEndMemberRuntimeWrapper.Wrap(obj);
			if(type == typeof(EntityTypeBaseRuntimeWrapper))
				return EntityTypeBaseRuntimeWrapper.Wrap(obj);
			if(type == typeof(RelationshipTypeRuntimeWrapper))
				return RelationshipTypeRuntimeWrapper.Wrap(obj);
			if(type == typeof(AssociationTypeRuntimeWrapper))
				return AssociationTypeRuntimeWrapper.Wrap(obj);
			return obj;
		}
	}
	internal class MetadataPropertyRuntimeWrapper : IWrapper {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected MetadataPropertyRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "MetadataProperty") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static MetadataPropertyRuntimeWrapper Wrap(object obj) {
			return new MetadataPropertyRuntimeWrapper(obj, null);
		}
		public String Name {
			get { return (String)RuntimeTypesHelper.GetProperty(type, "Name").GetValue(Object, null); }
		}
		public Object Value {
			get { return (Object)RuntimeTypesHelper.GetProperty(type, "Value").GetValue(Object, null); }
			set { RuntimeTypesHelper.GetProperty(type, "Value").SetValue(Object, value, null); }
		}
		public PropertyKindRuntimeWrapper PropertyKind {
			get { return (PropertyKindRuntimeWrapper)RuntimeTypesHelper.GetProperty(type, "PropertyKind").GetValue(Object, null); }
		}
	}
	internal class EntitySetBaseRuntimeWrapper : IWrapper {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected EntitySetBaseRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "EntitySetBase") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static EntitySetBaseRuntimeWrapper Wrap(object obj) {
			return new EntitySetBaseRuntimeWrapper(obj, null);
		}
		public String Table {
			get { return (String)RuntimeTypesHelper.GetProperty(type, "Table").GetValue(Object, null); }
			set { RuntimeTypesHelper.GetProperty(type, "Table").SetValue(Object, value, null); }
		}
		public String Schema {
			get { return (String)RuntimeTypesHelper.GetProperty(type, "Schema").GetValue(Object, null); }
			set { RuntimeTypesHelper.GetProperty(type, "Schema").SetValue(Object, value, null); }
		}
		public ReadOnlyMetadataCollectionRuntimeWrapper<MetadataPropertyRuntimeWrapper> MetadataProperties {
			get { return ReadOnlyMetadataCollectionRuntimeWrapper<MetadataPropertyRuntimeWrapper>.Wrap(RuntimeTypesHelper.GetProperty(type, "MetadataProperties").GetValue(Object, null)); }
		}
		public BuiltInTypeKindRuntimeWrapper BuiltInTypeKind {
			get { return (BuiltInTypeKindRuntimeWrapper)RuntimeTypesHelper.GetProperty(type, "BuiltInTypeKind").GetValue(Object, null); }
		}
		public String Name {
			get { return (String)RuntimeTypesHelper.GetProperty(type, "Name").GetValue(Object, null); }
			set { RuntimeTypesHelper.GetProperty(type, "Name").SetValue(Object, value, null); }
		}
		public EntityTypeBaseRuntimeWrapper ElementType {
			get { return EntityTypeBaseRuntimeWrapper.Wrap(RuntimeTypesHelper.GetProperty(type, "ElementType").GetValue(Object, null)); }
			set { RuntimeTypesHelper.GetProperty(type, "ElementType").SetValue(Object, ((IWrapper)value).Object, null); }
		}
	}
	internal class ReadOnlyMetadataCollectionRuntimeWrapper<T> : IWrapper, IEnumerable<T> {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected ReadOnlyMetadataCollectionRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "ReadOnlyMetadataCollection`1") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static ReadOnlyMetadataCollectionRuntimeWrapper<T> Wrap(object obj) {
			return new ReadOnlyMetadataCollectionRuntimeWrapper<T>(obj, null);
		}
		class E : IEnumerator<T> {
			IEnumerator e;
			public E(IEnumerator e) { this.e = e; }
			public T Current {
				get { return (T)(typeof(T).GetInterface("IWrapper") != null ? Wrapper.Wrap(typeof(T), e.Current) : e.Current); }
			}
			object IEnumerator.Current { get { return Current; } }
			public void Dispose() { }
			public bool MoveNext() { return e.MoveNext(); }
			public void Reset() { e.Reset(); }
		}
		public IEnumerator<T> GetEnumerator() {
			return new E(((IEnumerable)Object).GetEnumerator());
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
	internal class MetadataItemRuntimeWrapper : IWrapper {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected MetadataItemRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "MetadataItem") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static MetadataItemRuntimeWrapper Wrap(object obj) {
			return new MetadataItemRuntimeWrapper(obj, null);
		}
		public ReadOnlyMetadataCollectionRuntimeWrapper<MetadataPropertyRuntimeWrapper> MetadataProperties {
			get { return ReadOnlyMetadataCollectionRuntimeWrapper<MetadataPropertyRuntimeWrapper>.Wrap(RuntimeTypesHelper.GetProperty(type, "MetadataProperties").GetValue(Object, null)); }
		}
		public BuiltInTypeKindRuntimeWrapper BuiltInTypeKind {
			get { return (BuiltInTypeKindRuntimeWrapper)RuntimeTypesHelper.GetProperty(type, "BuiltInTypeKind").GetValue(Object, null); }
		}
	}
	internal class EntityContainerRuntimeWrapper : IWrapper {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected EntityContainerRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "EntityContainer") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static EntityContainerRuntimeWrapper Wrap(object obj) {
			return new EntityContainerRuntimeWrapper(obj, null);
		}
		public String Name {
			get { return (String)RuntimeTypesHelper.GetProperty(type, "Name").GetValue(Object, null); }
			set { RuntimeTypesHelper.GetProperty(type, "Name").SetValue(Object, value, null); }
		}
		public ReadOnlyMetadataCollectionRuntimeWrapper<EntitySetBaseRuntimeWrapper> BaseEntitySets {
			get { return ReadOnlyMetadataCollectionRuntimeWrapper<EntitySetBaseRuntimeWrapper>.Wrap(RuntimeTypesHelper.GetProperty(type, "BaseEntitySets").GetValue(Object, null)); }
		}
	}
	internal class GlobalItemRuntimeWrapper : MetadataItemRuntimeWrapper, IWrapper {
		protected GlobalItemRuntimeWrapper(object obj, Token token) : base(obj, token) { }
		public new static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "GlobalItem") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public new static GlobalItemRuntimeWrapper Wrap(object obj) {
			return new GlobalItemRuntimeWrapper(obj, null);
		}
	}
	internal class ItemCollectionRuntimeWrapper : IWrapper, IEnumerable<GlobalItemRuntimeWrapper> {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected ItemCollectionRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "ItemCollection") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static ItemCollectionRuntimeWrapper Wrap(object obj) {
			return new ItemCollectionRuntimeWrapper(obj, null);
		}
		class E : IEnumerator<GlobalItemRuntimeWrapper> {
			IEnumerator e;
			public E(IEnumerator e) { this.e = e; }
			public GlobalItemRuntimeWrapper Current {
				get { return GlobalItemRuntimeWrapper.Wrap(e.Current); }
			}
			object IEnumerator.Current { get { return Current; } }
			public void Dispose() { }
			public bool MoveNext() { return e.MoveNext(); }
			public void Reset() { e.Reset(); }
		}
		public IEnumerator<GlobalItemRuntimeWrapper> GetEnumerator() {
			return new E(((IEnumerable)Object).GetEnumerator());
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
	internal class MetadataWorkspaceRuntimeWrapper : IWrapper {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected MetadataWorkspaceRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "MetadataWorkspace") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static MetadataWorkspaceRuntimeWrapper Wrap(object obj) {
			return new MetadataWorkspaceRuntimeWrapper(obj, null);
		}
		public ItemCollectionRuntimeWrapper GetItemCollection(DataSpaceRuntimeWrapper dataSpace) {
			return
				ItemCollectionRuntimeWrapper.Wrap(
			RuntimeTypesHelper.GetMethod(type, "GetItemCollection").Invoke(Object, new object[] { 
					(int)dataSpace
		 })
			)
			;
		}
		public ReadOnlyCollectionRuntimeWrapper<GlobalItemRuntimeWrapper> GetItems(DataSpaceRuntimeWrapper dataSpace) {
			return
				ReadOnlyCollectionRuntimeWrapper<GlobalItemRuntimeWrapper>.Wrap(
			RuntimeTypesHelper.GetMethod(type, "GetItems").Invoke(Object, new object[] { 
					(int)dataSpace
		 })
			)
			;
		}
		public EntityContainerRuntimeWrapper GetEntityContainer(String name, DataSpaceRuntimeWrapper dataSpace) {
			return
				EntityContainerRuntimeWrapper.Wrap(
			RuntimeTypesHelper.GetMethod(type, "GetEntityContainer").Invoke(Object, new object[] { 
			((object)name is IWrapper) ? ((IWrapper)(object)name).Object : name
		, 
					(int)dataSpace
		 })
			)
			;
		}
	}
	internal class ObjectContextRuntimeWrapper : IWrapper {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected ObjectContextRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "ObjectContext") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static ObjectContextRuntimeWrapper Wrap(object obj) {
			return new ObjectContextRuntimeWrapper(obj, null);
		}
		public MetadataWorkspaceRuntimeWrapper MetadataWorkspace {
			get { return MetadataWorkspaceRuntimeWrapper.Wrap(RuntimeTypesHelper.GetProperty(type, "MetadataWorkspace").GetValue(Object, null)); }
		}
	}
	internal class DbContextRuntimeWrapper : IWrapper {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected DbContextRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "DbContext") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static DbContextRuntimeWrapper Wrap(object obj) {
			return new DbContextRuntimeWrapper(obj, null);
		}
		public ObjectContextRuntimeWrapper ObjectContext {
			get { return ObjectContextRuntimeWrapper.Wrap(RuntimeTypesHelper.GetProperty(type, "ObjectContext").GetValue(Object, null)); }
		}
	}
	internal class EdmTypeRuntimeWrapper : GlobalItemRuntimeWrapper, IWrapper {
		protected EdmTypeRuntimeWrapper(object obj, Token token) : base(obj, token) { }
		public new static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "EdmType") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public new static EdmTypeRuntimeWrapper Wrap(object obj) {
			return new EdmTypeRuntimeWrapper(obj, null);
		}
		public String Name {
			get { return (String)RuntimeTypesHelper.GetProperty(type, "Name").GetValue(Object, null); }
			set { RuntimeTypesHelper.GetProperty(type, "Name").SetValue(Object, value, null); }
		}
	}
	internal class TypeUsageRuntimeWrapper : IWrapper {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected TypeUsageRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "TypeUsage") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static TypeUsageRuntimeWrapper Wrap(object obj) {
			return new TypeUsageRuntimeWrapper(obj, null);
		}
		public EdmTypeRuntimeWrapper EdmType {
			get { return EdmTypeRuntimeWrapper.Wrap(RuntimeTypesHelper.GetProperty(type, "EdmType").GetValue(Object, null)); }
		}
	}
	internal class EdmMemberRuntimeWrapper : IWrapper {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected EdmMemberRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "EdmMember") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static EdmMemberRuntimeWrapper Wrap(object obj) {
			return new EdmMemberRuntimeWrapper(obj, null);
		}
		public String Name {
			get { return (String)RuntimeTypesHelper.GetProperty(type, "Name").GetValue(Object, null); }
			set { RuntimeTypesHelper.GetProperty(type, "Name").SetValue(Object, value, null); }
		}
		public TypeUsageRuntimeWrapper TypeUsage {
			get { return TypeUsageRuntimeWrapper.Wrap(RuntimeTypesHelper.GetProperty(type, "TypeUsage").GetValue(Object, null)); }
			set { RuntimeTypesHelper.GetProperty(type, "TypeUsage").SetValue(Object, ((IWrapper)value).Object, null); }
		}
	}
	internal class EntityTypeRuntimeWrapper : EntityTypeBaseRuntimeWrapper, IWrapper {
		protected EntityTypeRuntimeWrapper(object obj, Token token) : base(obj, token) { }
		public new static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "EntityType") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public new static EntityTypeRuntimeWrapper Wrap(object obj) {
			return new EntityTypeRuntimeWrapper(obj, null);
		}
		public ReadOnlyMetadataCollectionRuntimeWrapper<EdmMemberRuntimeWrapper> KeyMembers {
			get { return ReadOnlyMetadataCollectionRuntimeWrapper<EdmMemberRuntimeWrapper>.Wrap(RuntimeTypesHelper.GetProperty(type, "KeyMembers").GetValue(Object, null)); }
		}
		public ReadOnlyMetadataCollectionRuntimeWrapper<NavigationPropertyRuntimeWrapper> NavigationProperties {
			get { return ReadOnlyMetadataCollectionRuntimeWrapper<NavigationPropertyRuntimeWrapper>.Wrap(RuntimeTypesHelper.GetProperty(type, "NavigationProperties").GetValue(Object, null)); }
		}
		public ReadOnlyMetadataCollectionRuntimeWrapper<EdmPropertyRuntimeWrapper> Properties {
			get { return ReadOnlyMetadataCollectionRuntimeWrapper<EdmPropertyRuntimeWrapper>.Wrap(RuntimeTypesHelper.GetProperty(type, "Properties").GetValue(Object, null)); }
		}
		public Type ClrType {
			get { return (Type)RuntimeTypesHelper.GetProperty(type, "ClrType").GetValue(Object, null); }
		}
	}
	internal class RelationshipEndMemberRuntimeWrapper : EdmMemberRuntimeWrapper, IWrapper {
		protected RelationshipEndMemberRuntimeWrapper(object obj, Token token) : base(obj, token) { }
		public new static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "RelationshipEndMember") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public new static RelationshipEndMemberRuntimeWrapper Wrap(object obj) {
			return new RelationshipEndMemberRuntimeWrapper(obj, null);
		}
		public RelationshipMultiplicityRuntimeWrapper RelationshipMultiplicity {
			get { return (RelationshipMultiplicityRuntimeWrapper)RuntimeTypesHelper.GetProperty(type, "RelationshipMultiplicity").GetValue(Object, null); }
			set { RuntimeTypesHelper.GetProperty(type, "RelationshipMultiplicity").SetValue(Object, value, null); }
		}
		public EntityTypeRuntimeWrapper GetEntityType() {
			return
				EntityTypeRuntimeWrapper.Wrap(
			RuntimeTypesHelper.GetMethod(type, "GetEntityType").Invoke(Object, new object[] { })
			)
			;
		}
	}
	internal class IEnumerableRuntimeWrapper<T> : IWrapper, IEnumerable<T> {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected IEnumerableRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "IEnumerable`1") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static IEnumerableRuntimeWrapper<T> Wrap(object obj) {
			return new IEnumerableRuntimeWrapper<T>(obj, null);
		}
		class E : IEnumerator<T> {
			IEnumerator e;
			public E(IEnumerator e) { this.e = e; }
			public T Current {
				get { return (T)(typeof(T).GetInterface("IWrapper") != null ? Wrapper.Wrap(typeof(T), e.Current) : e.Current); }
			}
			object IEnumerator.Current { get { return Current; } }
			public void Dispose() { }
			public bool MoveNext() { return e.MoveNext(); }
			public void Reset() { e.Reset(); }
		}
		public IEnumerator<T> GetEnumerator() {
			return new E(((IEnumerable)Object).GetEnumerator());
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
	internal class NavigationPropertyRuntimeWrapper : EdmMemberRuntimeWrapper, IWrapper {
		protected NavigationPropertyRuntimeWrapper(object obj, Token token) : base(obj, token) { }
		public new static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "NavigationProperty") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public new static NavigationPropertyRuntimeWrapper Wrap(object obj) {
			return new NavigationPropertyRuntimeWrapper(obj, null);
		}
		public RelationshipEndMemberRuntimeWrapper FromEndMember {
			get { return RelationshipEndMemberRuntimeWrapper.Wrap(RuntimeTypesHelper.GetProperty(type, "FromEndMember").GetValue(Object, null)); }
			set { RuntimeTypesHelper.GetProperty(type, "FromEndMember").SetValue(Object, ((IWrapper)value).Object, null); }
		}
		public RelationshipEndMemberRuntimeWrapper ToEndMember {
			get { return RelationshipEndMemberRuntimeWrapper.Wrap(RuntimeTypesHelper.GetProperty(type, "ToEndMember").GetValue(Object, null)); }
			set { RuntimeTypesHelper.GetProperty(type, "ToEndMember").SetValue(Object, ((IWrapper)value).Object, null); }
		}
		public StructuralTypeRuntimeWrapper DeclaringType {
			get { return StructuralTypeRuntimeWrapper.Wrap(RuntimeTypesHelper.GetProperty(type, "DeclaringType").GetValue(Object, null)); }
		}
		public IEnumerableRuntimeWrapper<EdmPropertyRuntimeWrapper> GetDependentProperties() {
			return
				IEnumerableRuntimeWrapper<EdmPropertyRuntimeWrapper>.Wrap(
			RuntimeTypesHelper.GetMethod(type, "GetDependentProperties").Invoke(Object, new object[] { })
			)
			;
		}
	}
	internal class ReadOnlyCollectionRuntimeWrapper<T> : IWrapper, IEnumerable<T> {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected ReadOnlyCollectionRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "ReadOnlyCollection`1") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static ReadOnlyCollectionRuntimeWrapper<T> Wrap(object obj) {
			return new ReadOnlyCollectionRuntimeWrapper<T>(obj, null);
		}
		class E : IEnumerator<T> {
			IEnumerator e;
			public E(IEnumerator e) { this.e = e; }
			public T Current {
				get { return (T)(typeof(T).GetInterface("IWrapper") != null ? Wrapper.Wrap(typeof(T), e.Current) : e.Current); }
			}
			object IEnumerator.Current { get { return Current; } }
			public void Dispose() { }
			public bool MoveNext() { return e.MoveNext(); }
			public void Reset() { e.Reset(); }
		}
		public IEnumerator<T> GetEnumerator() {
			return new E(((IEnumerable)Object).GetEnumerator());
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
	internal class StructuralTypeRuntimeWrapper : EdmTypeRuntimeWrapper, IWrapper {
		protected StructuralTypeRuntimeWrapper(object obj, Token token) : base(obj, token) { }
		public new static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "StructuralType") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public new static StructuralTypeRuntimeWrapper Wrap(object obj) {
			return new StructuralTypeRuntimeWrapper(obj, null);
		}
		public String FullName {
			get { return (String)RuntimeTypesHelper.GetProperty(type, "FullName").GetValue(Object, null); }
		}
	}
	internal class PrimitiveTypeRuntimeWrapper : IWrapper {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected PrimitiveTypeRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "PrimitiveType") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static PrimitiveTypeRuntimeWrapper Wrap(object obj) {
			return new PrimitiveTypeRuntimeWrapper(obj, null);
		}
		public Type ClrEquivalentType {
			get { return (Type)RuntimeTypesHelper.GetProperty(type, "ClrEquivalentType").GetValue(Object, null); }
		}
	}
	internal class EdmPropertyRuntimeWrapper : IWrapper {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected EdmPropertyRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "EdmProperty") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static EdmPropertyRuntimeWrapper Wrap(object obj) {
			return new EdmPropertyRuntimeWrapper(obj, null);
		}
		public String Name {
			get { return (String)RuntimeTypesHelper.GetProperty(type, "Name").GetValue(Object, null); }
			set { RuntimeTypesHelper.GetProperty(type, "Name").SetValue(Object, value, null); }
		}
		public StructuralTypeRuntimeWrapper DeclaringType {
			get { return StructuralTypeRuntimeWrapper.Wrap(RuntimeTypesHelper.GetProperty(type, "DeclaringType").GetValue(Object, null)); }
		}
		public PrimitiveTypeRuntimeWrapper PrimitiveType {
			get { return PrimitiveTypeRuntimeWrapper.Wrap(RuntimeTypesHelper.GetProperty(type, "PrimitiveType").GetValue(Object, null)); }
			set { RuntimeTypesHelper.GetProperty(type, "PrimitiveType").SetValue(Object, ((IWrapper)value).Object, null); }
		}
	}
	internal class ReferentialConstraintRuntimeWrapper : IWrapper {
		protected Type type;
		public object Object { get; protected set; }
		protected class Token { }
		protected ReferentialConstraintRuntimeWrapper(object obj, Token token) {
			var asWrapper = obj as IWrapper;
			if(asWrapper != null)
				obj = asWrapper.Object;
			Object = obj;
			this.type = obj.GetType();
		}
		public static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "ReferentialConstraint") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public static ReferentialConstraintRuntimeWrapper Wrap(object obj) {
			return new ReferentialConstraintRuntimeWrapper(obj, null);
		}
		public ReadOnlyMetadataCollectionRuntimeWrapper<EdmPropertyRuntimeWrapper> FromProperties {
			get { return ReadOnlyMetadataCollectionRuntimeWrapper<EdmPropertyRuntimeWrapper>.Wrap(RuntimeTypesHelper.GetProperty(type, "FromProperties").GetValue(Object, null)); }
		}
		public ReadOnlyMetadataCollectionRuntimeWrapper<EdmPropertyRuntimeWrapper> ToProperties {
			get { return ReadOnlyMetadataCollectionRuntimeWrapper<EdmPropertyRuntimeWrapper>.Wrap(RuntimeTypesHelper.GetProperty(type, "ToProperties").GetValue(Object, null)); }
		}
		public RelationshipEndMemberRuntimeWrapper FromRole {
			get { return RelationshipEndMemberRuntimeWrapper.Wrap(RuntimeTypesHelper.GetProperty(type, "FromRole").GetValue(Object, null)); }
			set { RuntimeTypesHelper.GetProperty(type, "FromRole").SetValue(Object, ((IWrapper)value).Object, null); }
		}
		public RelationshipEndMemberRuntimeWrapper ToRole {
			get { return RelationshipEndMemberRuntimeWrapper.Wrap(RuntimeTypesHelper.GetProperty(type, "ToRole").GetValue(Object, null)); }
			set { RuntimeTypesHelper.GetProperty(type, "ToRole").SetValue(Object, ((IWrapper)value).Object, null); }
		}
	}
	internal class AssociationEndMemberRuntimeWrapper : RelationshipEndMemberRuntimeWrapper, IWrapper {
		protected AssociationEndMemberRuntimeWrapper(object obj, Token token) : base(obj, token) { }
		public new static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "AssociationEndMember") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public new static AssociationEndMemberRuntimeWrapper Wrap(object obj) {
			return new AssociationEndMemberRuntimeWrapper(obj, null);
		}
	}
	internal class EntityTypeBaseRuntimeWrapper : StructuralTypeRuntimeWrapper, IWrapper {
		protected EntityTypeBaseRuntimeWrapper(object obj, Token token) : base(obj, token) { }
		public new static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "EntityTypeBase") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public new static EntityTypeBaseRuntimeWrapper Wrap(object obj) {
			return new EntityTypeBaseRuntimeWrapper(obj, null);
		}
	}
	internal class RelationshipTypeRuntimeWrapper : EntityTypeBaseRuntimeWrapper, IWrapper {
		protected RelationshipTypeRuntimeWrapper(object obj, Token token) : base(obj, token) { }
		public new static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "RelationshipType") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public new static RelationshipTypeRuntimeWrapper Wrap(object obj) {
			return new RelationshipTypeRuntimeWrapper(obj, null);
		}
	}
	internal class AssociationTypeRuntimeWrapper : RelationshipTypeRuntimeWrapper, IWrapper {
		protected AssociationTypeRuntimeWrapper(object obj, Token token) : base(obj, token) { }
		public new static bool IsCompatible(Type type) {
			while(type != null) {
				if(type.Name == "AssociationType") {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
		public new static AssociationTypeRuntimeWrapper Wrap(object obj) {
			return new AssociationTypeRuntimeWrapper(obj, null);
		}
		public ReadOnlyMetadataCollectionRuntimeWrapper<ReferentialConstraintRuntimeWrapper> ReferentialConstraints {
			get { return ReadOnlyMetadataCollectionRuntimeWrapper<ReferentialConstraintRuntimeWrapper>.Wrap(RuntimeTypesHelper.GetProperty(type, "ReferentialConstraints").GetValue(Object, null)); }
		}
		public ReadOnlyMetadataCollectionRuntimeWrapper<AssociationEndMemberRuntimeWrapper> AssociationEndMembers {
			get { return ReadOnlyMetadataCollectionRuntimeWrapper<AssociationEndMemberRuntimeWrapper>.Wrap(RuntimeTypesHelper.GetProperty(type, "AssociationEndMembers").GetValue(Object, null)); }
		}
	}
	enum BuiltInTypeKindRuntimeWrapper {
		AssociationEndMember,
		AssociationSetEnd,
		AssociationSet,
		AssociationType,
		EntitySetBase,
		EntityTypeBase,
		CollectionType,
		CollectionKind,
		ComplexType,
		Documentation,
		OperationAction,
		EdmType,
		EntityContainer,
		EntitySet,
		EntityType,
		EnumType,
		EnumMember,
		Facet,
		EdmFunction,
		FunctionParameter,
		GlobalItem,
		MetadataProperty,
		NavigationProperty,
		MetadataItem,
		EdmMember,
		ParameterMode,
		PrimitiveType,
		PrimitiveTypeKind,
		EdmProperty,
		ProviderManifest,
		ReferentialConstraint,
		RefType,
		RelationshipEndMember,
		RelationshipMultiplicity,
		RelationshipSet,
		RelationshipType,
		RowType,
		SimpleType,
		StructuralType,
		TypeUsage
	}
	enum PropertyKindRuntimeWrapper {
		System,
		Extended
	}
	enum DataSpaceRuntimeWrapper {
		OSpace,
		CSpace,
		SSpace,
		OCSpace,
		CSSpace
	}
	enum RelationshipMultiplicityRuntimeWrapper {
		ZeroOrOne,
		One,
		Many
	}
}
