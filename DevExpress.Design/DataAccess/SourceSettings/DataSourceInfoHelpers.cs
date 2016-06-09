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

namespace DevExpress.Design.DataAccess {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	static class DataSourceElementInfoHelper {
		public static string[] GetFields(Type elementType) {
			PropertyInfo[] properties = elementType.GetProperties(
				BindingFlags.Instance | BindingFlags.Public);
			return Array.ConvertAll(properties, (p) => p.Name);
		}
		public static string[] GetDeclaredFields(Type tableType) {
			PropertyInfo[] properties = tableType.GetProperties(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
			return Array.ConvertAll(properties, (p) => p.Name);
		}
	}
	static class DataTableInfoHelper {
		public static Type GetTableType(Type sourceType, string tableName) {
			MemberInfo member = sourceType.GetMember(tableName)[0];
			return (member is PropertyInfo) ?
				((PropertyInfo)member).PropertyType :
				((MethodInfo)member).ReturnType;
		}
		public static Type GetRowType(Type tableType) {
			return tableType.IsGenericType ?
				tableType.GetGenericArguments()[0] :
				tableType.BaseType.GetGenericArguments()[0];
		}
		public static IEnumerable<string> GetKeyExpressions(Type tableType, string keyAttributeName, string keyPropertyName, Func<Type, PropertyInfo[]> getProperties = null) {
			return GetKeyExpressionsFromKeyAttribute(tableType, keyAttributeName, keyPropertyName) ??
				System.Linq.Enumerable.Concat(
					GetKeyExpressionsFromProperties(tableType, keyAttributeName, keyPropertyName, getProperties),
					GetKeyExpressionsFromProperties(tableType, "System.ComponentModel.DataAnnotations.KeyAttribute", null, getProperties));
		}
		static IEnumerable<string> GetKeyExpressionsFromKeyAttribute(Type tableType, string keyAttributeName, string keyPropertyName) {
			object[] attributes = tableType.GetCustomAttributes(false);
			for(int i = 0; i < attributes.Length; i++) {
				Type attributeType = attributes[i].GetType();
				if(attributeType.Name == keyAttributeName) {
					return (IEnumerable<string>)attributeType.GetProperty(keyPropertyName).GetValue(attributes[i], null);
				}
			}
			return null;
		}
		static IEnumerable<string> GetKeyExpressionsFromProperties(Type tableType, string keyAttributeName, string keyPropertyName, Func<Type, PropertyInfo[]> getProperties = null) {
			getProperties = getProperties ?? ((type) => type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance));
			PropertyInfo[] properties = getProperties(tableType);
			for(int i = 0; i < properties.Length; i++) {
				object[] propertyAttributes = properties[i].GetCustomAttributes(false);
				if(FindKeyAttribute(propertyAttributes, keyAttributeName, keyPropertyName))
					yield return properties[i].Name;
			}
		}
		static bool FindKeyAttribute(object[] propertyAttributes, string keyAttributeName, string keyPropertyName) {
			bool found = false;
			for(int i = 0; i < propertyAttributes.Length; i++) {
				Type propertyAttributeType = propertyAttributes[i].GetType();
				if(propertyAttributeType.FullName == keyAttributeName || propertyAttributeType.Name == keyAttributeName) {
					if(IsKey(propertyAttributes[i], propertyAttributeType, keyPropertyName)) {
						found = true;
						break;
					}
				}
			}
			return found;
		}
		static bool IsKey(object attribute, Type attributeType, string keyPropertyName) {
			if(string.IsNullOrEmpty(keyPropertyName)) return true;
			PropertyInfo pInfo = attributeType.GetProperty(keyPropertyName, BindingFlags.Public | BindingFlags.Instance);
			return (pInfo != null) && (bool)pInfo.GetValue(attribute, null);
		}
	}
	static class DataTypeInfoHelper {
		public static IEnumerable<Type> GetLocalDataTypes() {
			return Metadata.AvailableTypes.Unique(Metadata.AvailableTypes.Local(ValidDataType), (t) => t);
		}
		public static IEnumerable<Type> GetLocalDataTypes(Predicate<Type> match) {
			return Metadata.AvailableTypes.Unique(Metadata.AvailableTypes.Local((t) => ValidDataType(t) && match(t)), (t) => t);
		}
		public static IEnumerable<Type> GetDataTypes(IEnumerable<Type> types, Predicate<Type> match) {
			return Metadata.AvailableTypes.Unique(Metadata.AvailableTypes.All(types, (t) => ValidDataType(t) && match(t)), (t) => t);
		}
		internal static bool ValidDataType(Type type) {
			return type.IsClass && 
				!BaseDataAccessTechnologyTypesProvider.IsStatic(type) && 
				!BaseDataAccessTechnologyTypesProvider.IsRelatedToEvents(type) &&
				!BaseDataAccessTechnologyTypesProvider.IsRelatedToAttributes(type) &&
				!BaseDataAccessTechnologyTypesProvider.IsRelatedToUI(type);
		}
	}
	static class EnumTypeInfoHelper {
		public static IEnumerable<Type> GetLocalDataTypes() {
			return Metadata.AvailableTypes.Unique(Metadata.AvailableTypes.Local(ValidDataType), (t) => t);
		}
		internal static bool ValidDataType(Type type) { 
			return type.IsEnum && type.IsPublic; 
		}
	}
}
