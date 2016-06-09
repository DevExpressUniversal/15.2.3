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
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	static class TypeHelper {
		internal static bool IsExpandableProperty(PropertyDescriptor pd) {
			return IsExpandableType(pd.PropertyType);
		}
		internal static bool IsExpandableType(Type type) {
			return !(type.IsValueType || type.IsEnum || Equals(type, typeof(string)) || Equals(type, typeof(object)));
		}
		internal static bool IsNullable(Type type) {
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
		internal static bool AllowNull(Type type) {
			return !type.IsValueType || IsNullable(type);
		}
	}
	static class GenericTypeHelper {
		internal static Type GetElementType(Type dataSorceType) {
			if(typeof(Array).IsAssignableFrom(dataSorceType))
				return dataSorceType.GetElementType();
			Type[] genericArguments = FindGenericArguments(dataSorceType,
				t => typeof(IQueryable<>) == t || typeof(IEnumerable<>) == t);
			return (genericArguments != null) ? genericArguments[0] : typeof(object);
		}
		readonly static ConcurrentDictionary<Type, bool> genericTypes = new ConcurrentDictionary<Type, bool>();
		static Type FindTypeDefinition(Type type, Predicate<Type> match) {
			if(IsGenericType(type) && match(type.GetGenericTypeDefinition()))
				return type;
			foreach(Type item in type.GetInterfaces()) {
				if(IsGenericType(item) && match(item.GetGenericTypeDefinition())) {
					return item;
				}
			}
			return null;
		}
		static Type[] FindGenericArguments(Type type, Predicate<Type> match) {
			Type typeDefinition = FindTypeDefinition(type, match);
			return (typeDefinition != null) ? typeDefinition.GetGenericArguments() : null;
		}
		static bool IsGenericType(Type type) {
			return type.IsGenericType() && genericTypes.GetOrAdd(type, 
				t => t.GetGenericArguments().Length == 1);
		}
	}
	static class EnumHelper {
		internal static bool IsFlags(Type enumType) {
			return enumType.IsDefined(typeof(FlagsAttribute), false);
		}
		internal static object GetDefaultValue(Type enumType) {
			return Activator.CreateInstance(enumType);
		}
		internal static object[] GetValues(Type enumType) {
			var values = Enum.GetValues(enumType);
			object[] res = new object[values.Length];
			Array.Copy(values, res, res.Length);
			return res;
		}
	}
}
