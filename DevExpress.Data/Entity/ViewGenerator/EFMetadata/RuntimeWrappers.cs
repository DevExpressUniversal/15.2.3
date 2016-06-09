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
using System.Collections.Generic;
using System.Collections;
using DevExpress.Utils;
namespace DevExpress.Entity.Model.Metadata {
	public class RuntimeBase {
		readonly object value;
		public object Value { get { return value; } }
		protected RuntimeBase(object value) {
			this.value = value;
		}
		#region Equality
		public override int GetHashCode() {
			return value.GetHashCode();
		}
		public static bool operator ==(RuntimeBase r1, RuntimeBase r2) {
			bool r1IsNull = (object)r1 == null;
			bool r2IsNull = (object)r2 == null;
			if(r1IsNull && r2IsNull) return true;
			if(r1IsNull || r2IsNull) return false;
			return object.Equals(r1.value, r2.value);
		}
		public static bool operator !=(RuntimeBase r1, RuntimeBase r2) {
			return !(r1 == r2);
		}
		public override bool Equals(object obj) {
			return this == obj as RuntimeBase;
		}
		#endregion
		public override string ToString() { return value.ToString(); }
	}
	public abstract class RuntimeWrapper : RuntimeBase {
		string expectedTypeName;
		protected RuntimeWrapper(string expectedTypeName, object value)
			: base(value) {
			this.expectedTypeName = expectedTypeName;
		}
		public static TTargetType ConvertEnum<TTargetType>(object source) {
			return (TTargetType)Enum.Parse(typeof(TTargetType), source.ToString());
		}
		void CheckTypeName() {
			if(CheckOnlyTypeName)
				IsTypeNamesMatch(Type, expectedTypeName, true);
			else
				IsTypeMatch(Type, expectedTypeName, true);
		}
		public static bool IsTypeNamesMatch(Type targetType, string expectedTypeName, bool throwOnError = false) {
			if(string.IsNullOrEmpty(expectedTypeName) || string.Compare(targetType.Name, expectedTypeName, true) == 0)
				return true;
			Type baseType = targetType.GetBaseType();
			while(baseType != null) {
				if(string.Compare(baseType.Name, expectedTypeName, true) == 0)
					return true;
				baseType = baseType.GetBaseType();
			}
			if(throwOnError)
				throw new ArgumentException(string.Format("Expected Type of the Value is \"{0}\", but was \"{1}\"", expectedTypeName, targetType.Name));
			return false;
		}
		public static bool IsTypeMatch(Type targetType, string expectedTypeName, bool throwOnError = false) {
			if(string.IsNullOrEmpty(expectedTypeName) || string.Compare(targetType.FullName, expectedTypeName, true) == 0)
				return true;
			Type baseType = targetType.GetBaseType();
			while(baseType != null) {
				if(string.Compare(baseType.FullName, expectedTypeName, true) == 0)
					return true;
				baseType = baseType.GetBaseType();
			}
			if(throwOnError)
				throw new ArgumentException(string.Format("Expected Type of the Value is \"{0}\", but was \"{1}\"", expectedTypeName, targetType.FullName));
			return false;
		}
		protected Type Type {
			get { return Value.GetType(); }
		}
		protected virtual bool CheckOnlyTypeName { get { return false; } }
		Dictionary<string, PropertyAccessor> properties = new Dictionary<string, PropertyAccessor>();
		protected PropertyAccessor GetPropertyAccessor(string name) {
			if(string.IsNullOrEmpty(name))
				throw new ArgumentException("name");
			PropertyAccessor accessor;
			if(properties.TryGetValue(name, out accessor))
				return accessor;
			if(PropertyAccessor.IsComplexPropertyName(name))
				accessor = new NestedPropertyAccessor(Value, name);
			else
				accessor = new PropertyAccessor(Value, name);
			properties[name] = accessor;
			return accessor;
		}
		Dictionary<string, MethodAccessor> methodAccessors = new Dictionary<string, MethodAccessor>();
		protected MethodAccessor GetMethodAccessor(string name) {
			if(string.IsNullOrEmpty(name))
				throw new ArgumentException("name");
			MethodAccessor accessor;
			if(methodAccessors.TryGetValue(name, out accessor))
				return accessor;
			accessor = new MethodAccessor(Value, name);
			methodAccessors[name] = accessor;
			return accessor;
		}
	}
	public class EdmFunctionInfo : RuntimeWrapper {
		public EdmFunctionInfo(object source)
			: base(typeof(System.Data.Metadata.Edm.EdmFunction).FullName, source) {
		}
		public string Name { get { 
			string functionName = GetPropertyAccessor("FunctionName").Value as string;
			return functionName != null ? functionName : GetPropertyAccessor("Name").Value as string;
			}
		}
		public FunctionParameterInfo[] Parameters {
			get {
				IEnumerable<object> edmParameters = GetPropertyAccessor("Parameters").Value as IEnumerable<object>;
				IEnumerable<FunctionParameterInfo> functionParameters = edmParameters.Select(p => new FunctionParameterInfo(p));
				return functionParameters.ToArray();
			}
		}
		public EdmComplexTypePropertyInfo[] ResultTypeProperties {
			get {
				object returnObject = GetPropertyAccessor("ReturnParameter").Value;
				if(returnObject == null)
					return null;
				FunctionParameterInfo returnParameter = new FunctionParameterInfo(returnObject);
				return returnParameter.ResultTypeProperties;
			}
		}
	}
	public class FunctionParameterInfo : RuntimeWrapper {
		public FunctionParameterInfo(object source)
			: base(typeof(System.Data.Metadata.Edm.FunctionParameter).FullName, source) {
		}
		public string Name { get { return GetPropertyAccessor("Name").Value as string; } }
		public BuiltInTypeKind BuiltInTypeKind {
			get {
				return ConvertEnum<BuiltInTypeKind>(GetPropertyAccessor("BuiltInTypeKind").Value);
			}
		}
		public string TypeName { get { return GetPropertyAccessor("TypeName").Value as string; } }
		public EdmTypeInfo EdmType {
			get {
				TypeUsageInfo typeUsageInfo = new TypeUsageInfo(GetPropertyAccessor("TypeUsage").Value);
				return typeUsageInfo.EdmType;
			}
		}
		public Type ClrType {
			get {
				TypeUsageInfo typeUsageInfo = new TypeUsageInfo(GetPropertyAccessor("TypeUsage").Value);
				return typeUsageInfo.ClrType;
			}
		}
		internal EdmComplexTypePropertyInfo[] ResultTypeProperties {
			get {
				TypeUsageInfo typeUsageInfo = new TypeUsageInfo(GetPropertyAccessor("TypeUsage").Value);
				EdmComplexTypeInfo edmComplexTypeInfo = new EdmComplexTypeInfo(typeUsageInfo.CollectionElementType.Value);
				return edmComplexTypeInfo.Properties;
			}
		}
	}
	public class TypeUsageInfo : RuntimeWrapper {
		public TypeUsageInfo(object source)
			: base(typeof(System.Data.Metadata.Edm.TypeUsage).FullName, source) {
		}
		EdmTypeInfo edmType;
		public EdmTypeInfo EdmType {
			get {
				if(edmType == null)
					edmType = new EdmTypeInfo(GetPropertyAccessor("EdmType").Value);
				return edmType;
			}
		}
		public EdmTypeInfo CollectionElementType {
			get {
				return EdmType.CollectionElementType;
			}
		}
		public string Name {
			get {
				return EdmType.Name;
			}
		}
		public Type ClrType {
			get {
				return EdmType.ClrType;
			}
		}
	}
	public class EdmTypeInfo : RuntimeWrapper {
		public EdmTypeInfo(object value)
			: base(typeof(System.Data.Metadata.Edm.EdmType).FullName, value) {
		}
		public BuiltInTypeKind BuiltInTypeKind {
			get {
				return ConvertEnum<BuiltInTypeKind>(GetPropertyAccessor("BuiltInTypeKind").Value);
			}
		}
		public Type ClrType {
			get {
				return GetPropertyAccessor("ClrType").Value as Type;
			}
		}
		public string Name {
			get {
				return GetPropertyAccessor("Name").Value as string;
			}
		}
		TypeUsageInfo typeUsageInfo;
		public EdmTypeInfo CollectionElementType {
			get {
				if(typeUsageInfo != null)
					return typeUsageInfo.EdmType;
				MethodAccessor getCollectionType = GetMethodAccessor("GetCollectionType");
				object source = getCollectionType.Invoke();
				object result = PropertyAccessor.GetValue(PropertyAccessor.GetValue(PropertyAccessor.GetValue(source, "TypeUsage"), "EdmType"), "TypeUsage");
				typeUsageInfo = new TypeUsageInfo(result);
				return typeUsageInfo.EdmType;
			}
		}
	}
	public class EdmComplexTypeInfo : RuntimeWrapper {
		public EdmComplexTypeInfo(object value)
			: base(typeof(System.Data.Metadata.Edm.ComplexType).FullName, value) {
		}
		public EdmComplexTypePropertyInfo[] Properties {
			get {
				IEnumerable<object> edmParameters = GetPropertyAccessor("Properties").Value as IEnumerable<object>;
				if(edmParameters != null) {
					IEnumerable<EdmComplexTypePropertyInfo> properties = edmParameters.Select(p => new EdmComplexTypePropertyInfo(p));
					return properties.ToArray();
				} else
					return new EdmComplexTypePropertyInfo[] { };
			}
		}
	}
	public class EdmComplexTypePropertyInfo : RuntimeWrapper {
		public EdmComplexTypePropertyInfo(object value)
			: base(typeof(System.Data.Metadata.Edm.EdmProperty).FullName, value) {
			Name = PropertyAccessor.GetValue(value, "Name") as string;
			ClrType = PropertyAccessor.GetValue(PropertyAccessor.GetValue(value, "PrimitiveType"), "ClrEquivalentType") as Type;
			if(ClrType == null) {
				TypeUsageInfo info = new TypeUsageInfo(PropertyAccessor.GetValue(value, "TypeUsage"));
				ClrType = info.ClrType;
			}
		}
		public string Name {
			get;
			private set;
		}
		public Type ClrType {
			get;
			private set;
		}
	}
	public class EntitySetBaseInfo : RuntimeWrapper {
		public EntitySetBaseInfo(object source)
			: base(typeof(System.Data.Metadata.Edm.EntitySetBase).FullName, source) {
		}
		EntityTypeBaseInfo elementType;
		public EntityTypeBaseInfo ElementType {
			get {
				if(elementType == null)
					elementType = new EntityTypeBaseInfo(GetPropertyAccessor("ElementType").Value);
				return elementType;
			}
		}
		public string Name { get { return GetPropertyAccessor("Name").Value as string; } }
		public BuiltInTypeKind BuiltInTypeKind {
			get { return ConvertEnum<BuiltInTypeKind>(GetPropertyAccessor("BuiltInTypeKind").Value); }
		}
	}
	public class EntityTypeBaseInfo : RuntimeWrapper {
		public EntityTypeBaseInfo(object source) :
			base(typeof(System.Data.Metadata.Edm.EntityTypeBase).FullName, source) {
		}
		public object BaseType {
			get { return GetPropertyAccessor("BaseType").Value; }
		}
		public string FullName { get { return GetPropertyAccessor("FullName").Value as string; } }
		public string Name { get { return GetPropertyAccessor("Name").Value as string; } }
		public bool Abstract { get { return (bool)(GetPropertyAccessor("Abstract").Value); } }
		public string BaseTypeFullName { get { return GetPropertyAccessor("BaseType.FullName").Value as string; } }
		public BuiltInTypeKind BuiltInTypeKind {
			get { return ConvertEnum<BuiltInTypeKind>(GetPropertyAccessor("BuiltInTypeKind").Value); }
		}
		IEnumerable<EdmMemberInfo> properties;
		public IEnumerable<EdmMemberInfo> Properties {
			get {
				if(properties != null)
					return properties;
				IEnumerable objects = GetPropertyAccessor("Properties").Value as IEnumerable;
				properties = objects.Cast<object>().Select<object, EdmMemberInfo>(x => new EdmMemberInfo(x));
				return properties;
			}
		}
		IEnumerable<EdmMemberInfo> navigationProperties;
		public IEnumerable<EdmMemberInfo> NavigationProperties {
			get {
				if(navigationProperties != null)
					return navigationProperties;
				IEnumerable objects = GetPropertyAccessor("NavigationProperties").Value as IEnumerable;
				navigationProperties = objects.Cast<object>().Select<object, EdmMemberInfo>(x => new EdmMemberInfo(x));
				return navigationProperties;
			}
		}
		IEnumerable<EdmMemberInfo> keyMembers;
		public IEnumerable<EdmMemberInfo> KeyMembers {
			get {
				if(keyMembers != null)
					return keyMembers;
				IEnumerable objects = GetPropertyAccessor("KeyMembers").Value as IEnumerable;
				keyMembers = objects.Cast<object>().Select<object, EdmMemberInfo>(x => new EdmMemberInfo(x));
				return keyMembers;
			}
		}
	}
	public class AssociationTypeInfo : EntityTypeBaseInfo {
		public AssociationTypeInfo(object source)
			: base(source) {
		}
		public IEnumerable<EdmMemberInfo> GetDependentProperties(EdmMemberInfo navProperty) {
			try {
				if(!navProperty.IsNavigationProperty)
					return null;
				IEnumerable referentialConstraints = GetPropertyAccessor("ReferentialConstraints").Value as IEnumerable;
				object count = GetPropertyAccessor("ReferentialConstraints.Count").Value;
				if(referentialConstraints == null || count == null || (int)count <= 0)
					return null;
				object firstRc = referentialConstraints.OfType<object>().FirstOrDefault();
				if(firstRc == null)
					return null;
				PropertyAccessor constraintFromRole = new PropertyAccessor(firstRc, "FromRole");
				PropertyAccessor firstRcToRole = new PropertyAccessor(firstRc, "ToRole");
				if(!EdmEquals(navProperty.FromEndMember, firstRcToRole.Value))
					return null;
				MethodAccessor constraintFromRoleGetEntityType = new MethodAccessor(constraintFromRole.Value, "GetEntityType");
				object fromRoleEntityType = constraintFromRoleGetEntityType.Invoke();
				if(fromRoleEntityType == null)
					return null;
				IEnumerable keyMembers = PropertyAccessor.GetValue(fromRoleEntityType, "KeyMembers") as IEnumerable;
				if(keyMembers == null)
					return null;
				List<EdmMemberInfo> dependantProperties = new List<EdmMemberInfo>();
				IEnumerable constraintFromProperties = PropertyAccessor.GetValue(firstRc, "FromProperties") as IEnumerable;
				IEnumerable constraintToProperties = PropertyAccessor.GetValue(firstRc, "ToProperties") as IEnumerable;
				if(constraintFromProperties == null || constraintToProperties == null)
					return null;
				List<object> constraintFromPropertiesArray = constraintFromProperties.OfType<object>().ToList();
				List<object> constraintToPropertiesArray = constraintToProperties.OfType<object>().ToList();
				foreach(object key in keyMembers) {
					int index = constraintFromPropertiesArray.IndexOf(key);
					if(index < 0 || constraintToPropertiesArray.Count <= index)
						continue;
					object toProperty = constraintToPropertiesArray[index];
					if(toProperty == null)
						continue;
					dependantProperties.Add(new EdmMemberInfo(toProperty));
				}
				return dependantProperties;
			}
			catch {
				return null;
			}
		}
		bool EdmEquals(object firstItem, object secondItem) {
			if(firstItem == null || secondItem == null)
				return false;
			if(firstItem == secondItem)
				return true;
			PropertyAccessor firstItemName = new PropertyAccessor(firstItem, "Name");
			PropertyAccessor secondItemName = new PropertyAccessor(secondItem, "Name");
			if(firstItemName.Value != secondItemName.Value)
				return false;
			PropertyAccessor firstItemBuiltInTypeKind = new PropertyAccessor(firstItem, "BuiltInTypeKind");
			PropertyAccessor secondItemBuiltInTypeKind = new PropertyAccessor(secondItem, "BuiltInTypeKind");
			return (ConvertEnum<BuiltInTypeKind>(firstItemBuiltInTypeKind.Value) == ConvertEnum<BuiltInTypeKind>(secondItemBuiltInTypeKind.Value));
		}
		public AssociationTypeInfo GetCSpaceAssociationType(IEntityTypeInfo declaringType) {
			if(declaringType == null)
				return null;
			EntityTypeInfo typeBase = declaringType as EntityTypeInfo;
			if(typeBase == null || typeBase.AssociationTypeSource == null)
				return null;
			return typeBase.AssociationTypeSource.GetAssociationTypeFromCSpace(this.FullName);
		}
		public bool IsForeignKey {
			get {
				object value = GetPropertyAccessor("IsForeignKey").Value;
				return value == null ? false : (bool)value;
			}
		}
		public IEnumerable<EdmMemberInfo> GetToEndPropertyNames(EdmMemberInfo navProperty, EntityTypeBaseInfo toEndEntityTypeInfo) {
			if(!navProperty.IsNavigationProperty)
				return null;
			IEnumerable referentialConstraints = GetPropertyAccessor("ReferentialConstraints").Value as IEnumerable;
			object count = GetPropertyAccessor("ReferentialConstraints.Count").Value;
			if(referentialConstraints == null || count == null || (int)count <= 0)
				return null;
			int itemsCount = (int)count;
			for(int i = 0; i < itemsCount; i++) {
				object rc = referentialConstraints.OfType<object>().ElementAt(i);
				if(rc == null)
					continue;
				IEnumerable toProperties = PropertyAccessor.GetValue(rc, "ToProperties") as IEnumerable;
				if(toProperties == null)
					continue;
				count = PropertyAccessor.GetValue(toProperties, "Count");
				if(count == null || (int)count <= 0)
					continue;
				var properties = toProperties.Cast<object>();
				if(!properties.Any())
					continue;
				var typedProperties =
					(from p in properties
					let typed = new EdmMemberInfo(p)
					where typed.DeclaringType != null
					let type = new EntityTypeBaseInfo(typed.DeclaringType)
					where type.Name == toEndEntityTypeInfo.Name
					select typed).ToList();
				if(typedProperties.Any())
					return typedProperties;
			}
			return null;
		}
	}
	public class PrimitiveType : RuntimeWrapper {
		public PrimitiveType(object value) : base("EdmPrimitiveType", value) { }
		public Type ClrEquivalentType {
			get {
				return (Type)GetPropertyAccessor("ClrEquivalentType").Value;
			}
		}
	}
	public class EdmMemberInfo : RuntimeWrapper {
		public EdmMemberInfo(object member)
			: base(typeof(System.Data.Metadata.Edm.EdmMember).FullName, member) {
		}
		public string Name { get { return GetPropertyAccessor("Name").Value as string; } }
		public bool IsProperty { get { return BuiltInTypeKind == BuiltInTypeKind.EdmProperty || BuiltInTypeKind == BuiltInTypeKind.NavigationProperty; } }
		public bool IsNavigationProperty { get { return BuiltInTypeKind == BuiltInTypeKind.NavigationProperty; } }
		public bool IsCollectionProperty {
			get {
				object value = GetPropertyAccessor("TypeUsage.EdmType.BuiltInTypeKind").Value;
				return value != null && ConvertEnum<BuiltInTypeKind>(value) == BuiltInTypeKind.CollectionType;
			}
		}
		public BuiltInTypeKind BuiltInTypeKind {
			get { return ConvertEnum<BuiltInTypeKind>(GetPropertyAccessor("BuiltInTypeKind").Value); }
		}
		public AssociationTypeInfo GetAssociationType() {
			if(!IsNavigationProperty)
				return null;
			return new AssociationTypeInfo(GetPropertyAccessor("RelationshipType").Value);
		}
		public object FromEndMember {
			get {
				return GetPropertyAccessor("FromEndMember").Value;
			}
		}
		public bool IsKeyMember {
			get {
				object value = GetPropertyAccessor("IsKeyMember").Value;
				if(value == null)
					return false;
				return (bool)value;
			}
		}
		internal object ToEndMember {
			get {
				return GetPropertyAccessor("ToEndMember").Value;
			}
		}
		internal object DeclaringType {
			get {
				return GetPropertyAccessor("DeclaringType").Value;
			}
		}
		public PrimitiveType PrimitiveType {
			get {
				return new PrimitiveType(GetPropertyAccessor("PrimitiveType").Value);
			}
		}
	}
}
