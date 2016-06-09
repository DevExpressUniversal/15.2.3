#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.DC {
	public class BaseTypeInfoSource {
		protected const BindingFlags membersBindingFlags =
			BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance |
			BindingFlags.GetProperty | BindingFlags.GetField;
		private readonly Dictionary<Type, Object[]> ownAttributes;
		private readonly Dictionary<Type, Boolean> attributesInheritedInfoCache;
		protected readonly Dictionary<IMemberInfo, Object> nativeMemberInfoDictionary;
		private static Boolean IsIEnumerableImplementor(Type type) {
			if(typeof(IEnumerable).IsAssignableFrom(type)) {
				return true;
			}
			if(type == typeof(IEnumerable)) {
				return true;
			}
			foreach(Type implementedInterface in type.GetInterfaces()) {
				if(implementedInterface == typeof(IEnumerable)) {
					return true;
				}
			}
			return false;
		}
		private Boolean IsGenericIEnumerableImplementor(Type type, out Type genericArgument) {
			if(IsGenericIEnumerable(type)) {
				genericArgument = GetFirstGenericArgument(type);
				return true;
			}
			foreach(Type implementedInterface in type.GetInterfaces()) {
				if(IsGenericIEnumerable(implementedInterface)) {
					genericArgument = GetFirstGenericArgument(implementedInterface);
					return true;
				}
			}
			genericArgument = null;
			return false;
		}
		private Boolean IsGenericIEnumerable(Type type) {
			if(!type.IsGenericType || type.IsGenericTypeDefinition) return false;
			return type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
		}
		private Type GetFirstGenericArgument(Type type) {
			return type.GetGenericArguments()[0];
		}
		private Boolean IsIndexedMember(PropertyInfo propertyInfo) {
			ParameterInfo[] parms = propertyInfo.GetIndexParameters();
			return parms.Length > 0;
		}
		private Boolean CalcIsOverride(PropertyInfo propertyInfo) {
			MethodInfo getMethod = propertyInfo.GetGetMethod();
			MethodInfo setMethod = propertyInfo.GetSetMethod();
			return
				(getMethod != null) && getMethod.IsVirtual && (getMethod.GetBaseDefinition() != getMethod)
				||
				(setMethod != null) && setMethod.IsVirtual && (setMethod.GetBaseDefinition() != setMethod);
		}
		private Object[] GetOwnAttributes(Type type) {
			Object[] result;
			if(!ownAttributes.TryGetValue(type, out result)) {
				result = type.GetCustomAttributes(false);
				ownAttributes.Add(type, result);
			}
			return result;
		}
		private Boolean IsInheritedAttribute(Attribute attribute) {
			Boolean isInherited;
			Type attributeType = attribute.GetType();
			if(!attributesInheritedInfoCache.TryGetValue(attributeType, out isInherited)) {
				Object[] usageAttributes = attributeType.GetCustomAttributes(typeof(AttributeUsageAttribute), false);
				if(usageAttributes.Length == 0) {
					isInherited = true;
				}
				else {
					AttributeUsageAttribute usageAttribute = (AttributeUsageAttribute)usageAttributes[0];
					isInherited = usageAttribute.Inherited;
				}
				attributesInheritedInfoCache.Add(attributeType, isInherited);
			}
			return isInherited;
		}
		protected Boolean IsListType(Type type) {
			return IsListTypeCore(type);
		}
		internal static Boolean IsListTypeCore(Type type) {
			if(type == typeof(String)) return false;
			return IsIEnumerableImplementor(type);
		}
		protected void SetListInfo(XafMemberInfo memberInfo) {
			Type type = memberInfo.MemberType;
			if(IsListType(type)) {
				memberInfo.IsList = true;
				CollectionAttribute attribute = memberInfo.FindAttribute<CollectionAttribute>();
				if(attribute != null) {
					memberInfo.ListElementType = attribute.ElementType;
				}
				else {
					Type listElementType;
					if(IsGenericIEnumerableImplementor(type, out listElementType)) {
						memberInfo.ListElementType = listElementType;
					}
				}
			}
		}
		protected void HandleIsAggregatedAttribute(XafMemberInfo xafMemberInfo) {
			xafMemberInfo.IsAggregated = (xafMemberInfo.FindAttribute<AggregatedAttribute>(false) != null);
		}
		protected void HandleFieldSizeAttribute(XafMemberInfo xafMemberInfo) {
			FieldSizeAttribute fieldSizeAttribute = xafMemberInfo.FindAttribute<FieldSizeAttribute>(false);
			if(fieldSizeAttribute != null) {
				xafMemberInfo.ValueMaxLength = (fieldSizeAttribute.Size > 0) ? fieldSizeAttribute.Size : 0;
				if(xafMemberInfo.Size == 0) {
					xafMemberInfo.Size = fieldSizeAttribute.Size;
				}
			}
		}
		protected void HandleCalculatedAttribute(XafMemberInfo xafMemberInfo) {
			CalculatedAttribute calculatedAttribute = xafMemberInfo.FindAttribute<CalculatedAttribute>(false);
			if(calculatedAttribute != null) {
				xafMemberInfo.Expression = calculatedAttribute.Expression;
			}
		}
		protected Boolean CalcTypeVisibility(TypeInfo typeInfo) {
			Boolean result = typeInfo.Type.IsPublic;
			BrowsableAttribute browsableAttribute = typeInfo.FindAttribute<BrowsableAttribute>();
			if((browsableAttribute != null) && !browsableAttribute.Browsable) {
				result = false;
			}
			return result;
		}
		protected Boolean CalcMemberVisibility(XafMemberInfo xafMemberInfo) {
			BrowsableAttribute browsableAttribute = xafMemberInfo.FindAttribute<BrowsableAttribute>();
			return browsableAttribute == null || browsableAttribute.Browsable;
		}
		protected IEnumerable<PropertyInfo> GetOwnProperties(Type type) {
			List<PropertyInfo> result = new List<PropertyInfo>();
			foreach(PropertyInfo propertyInfo in type.GetProperties(membersBindingFlags)) {
				if((IsListType(type) || !IsIndexedMember(propertyInfo)) && !CalcIsOverride(propertyInfo)) {
					result.Add(propertyInfo);
				}
			}
			return result;
		}
		protected IEnumerable<FieldInfo> GetOwnFields(Type type) {
			return type.GetFields(membersBindingFlags);
		}
		protected IEnumerable<PropertyDescriptor> GetPropertyDescriptors(Type type) {
			if(TypeDescriptor.GetProvider(type) == null) {
				return new PropertyDescriptor[0];
			}
			else {
				List<PropertyDescriptor> result = new List<PropertyDescriptor>();
				foreach(PropertyDescriptor property in TypeDescriptor.GetProperties(type)) {
					result.Add(property);
				}
				return result;
			}
		}
		protected void InitMemberFromMemberInfo(XafMemberInfo xafMemberInfo, MemberInfo reflectionMemberInfo) {
			xafMemberInfo.AddExtender<MemberInfo>(reflectionMemberInfo);
			try {
				xafMemberInfo.SetAttributes(reflectionMemberInfo.GetCustomAttributes(false), reflectionMemberInfo.GetCustomAttributes(true));
			}
			catch {
			}
			HandleIsAggregatedAttribute(xafMemberInfo);
		}
		protected void InitMemberFromPropertyInfo(XafMemberInfo xafMemberInfo, PropertyInfo propertyInfo) {
			InitMemberFromMemberInfo(xafMemberInfo, propertyInfo);
			xafMemberInfo.IsProperty = true;
			MethodInfo getter = propertyInfo.GetGetMethod();
			xafMemberInfo.IsPublic = (getter != null && getter.IsPublic);
			xafMemberInfo.IsReadOnly = propertyInfo.GetSetMethod() == null;
			xafMemberInfo.MemberType = propertyInfo.PropertyType;
			SetListInfo(xafMemberInfo);
			nativeMemberInfoDictionary[xafMemberInfo] = propertyInfo;
		}
		protected void InitMemberFromFieldInfo(XafMemberInfo xafMemberInfo, FieldInfo fieldInfo) {
			InitMemberFromMemberInfo(xafMemberInfo, fieldInfo);
			xafMemberInfo.IsProperty = false;
			xafMemberInfo.IsPublic = fieldInfo.IsPublic;
			xafMemberInfo.IsReadOnly = fieldInfo.IsInitOnly;
			xafMemberInfo.MemberType = fieldInfo.FieldType;
			SetListInfo(xafMemberInfo);
			nativeMemberInfoDictionary[xafMemberInfo] = fieldInfo;
		}
		protected void InitMemberFromPropertyDescriptor(XafMemberInfo memberInfo, PropertyDescriptor propertyDescriptor) {
			memberInfo.MemberType = propertyDescriptor.PropertyType;
			memberInfo.DisplayName = propertyDescriptor.DisplayName;
			memberInfo.IsPublic = true;
			SetListInfo(memberInfo);
			nativeMemberInfoDictionary[memberInfo] = propertyDescriptor;
		}
		protected BaseTypeInfoSource() {
			ownAttributes = new Dictionary<Type, object[]>();
			attributesInheritedInfoCache = new Dictionary<Type, bool>();
			nativeMemberInfoDictionary = new Dictionary<IMemberInfo, Object>();
		}
		public virtual void InitAttributes(TypeInfo typeInfo) {
			Type type = typeInfo.Type;
			typeInfo.SetAttributes(GetOwnAttributes(type), type.GetCustomAttributes(true));
			if(type.IsInterface) {
				foreach(Type implementedInterfaceType in type.GetInterfaces()) {
					Object[] implementedInterfaceAttributes = GetOwnAttributes(implementedInterfaceType);
					List<Object> inheritedAttributes = new List<Object>();
					foreach(Attribute attribute in implementedInterfaceAttributes) {
						if(IsInheritedAttribute(attribute)) {
							inheritedAttributes.Add(attribute);
						}
					}
					typeInfo.SetAttributes(null, inheritedAttributes.ToArray());
				}
			}
		}
		public virtual Type GetOriginalType(Type type) {
			return type;
		}
		public virtual Object GetValue(IMemberInfo memberInfo, Object obj) {
			Object nativeMemberInfo = null;
			if(nativeMemberInfoDictionary.TryGetValue(memberInfo, out nativeMemberInfo)) {
				if(nativeMemberInfo is PropertyInfo) {
					return ((PropertyInfo)nativeMemberInfo).GetValue(obj, null);
				}
				else if(nativeMemberInfo is FieldInfo) {
					return ((FieldInfo)nativeMemberInfo).GetValue(obj);
				}
				else if(nativeMemberInfo is PropertyDescriptor) {
					return ((PropertyDescriptor)nativeMemberInfo).GetValue(obj);
				}
			}
			return null;
		}
		public virtual void SetValue(IMemberInfo memberInfo, Object obj, Object value) {
			Object nativeMemberInfo = null;
			if(nativeMemberInfoDictionary.TryGetValue(memberInfo, out nativeMemberInfo)) {
				PropertyInfo propertyInfo = nativeMemberInfo as PropertyInfo;
				if(propertyInfo != null) {
					if(propertyInfo.CanWrite) {
						propertyInfo.SetValue(obj, value, null);
					}
				}
				else if(nativeMemberInfo is FieldInfo) {
					if(!memberInfo.IsReadOnly) {
						((FieldInfo)nativeMemberInfo).SetValue(obj, value);
					}
				}
				else if(nativeMemberInfo is PropertyDescriptor) {
					((PropertyDescriptor)nativeMemberInfo).SetValue(obj, value);
				}
			}
		}
		public virtual void UpdateMember(XafMemberInfo memberInfo) {
		}
		protected static void RegisterUsedEnumerations(TypeInfo typeInfo) {
			foreach(IMemberInfo memberInfo in typeInfo.OwnMembers) {
				Type memberType = memberInfo.MemberTypeInfo.IsNullable ? memberInfo.MemberTypeInfo.UnderlyingTypeInfo.Type : memberInfo.MemberType;
				if(memberType.IsEnum) {
					EnumProcessingHelper.RegisterEnum(memberType);
				}
			}
		}
	}
}
