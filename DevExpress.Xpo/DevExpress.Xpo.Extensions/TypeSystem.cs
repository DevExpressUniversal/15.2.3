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
using System.Collections.Generic;
using System.Linq;
using System.Data.Services.Providers;
namespace DevExpress.Xpo.Helpers {
	public static class TypeSystem {
		readonly static Dictionary<KeyValuePair<Type, Type>, bool> convertableType = new Dictionary<KeyValuePair<Type, Type>, bool>();
		internal static Type GetElementType(Type seqType) {
			Type ienum = FindIEnumerable(seqType);
			if(ienum == null) return seqType;
			return ienum.GetGenericArguments()[0];
		}
		private static Type FindIEnumerable(Type seqType) {
			if(seqType == null || seqType == typeof(string))
				return null;
			if(seqType.IsArray)
				return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
			if(seqType.IsGenericType) {
				foreach(Type arg in seqType.GetGenericArguments()) {
					Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
					if(ienum.IsAssignableFrom(seqType)) {
						return ienum;
					}
				}
			}
			Type[] ifaces = seqType.GetInterfaces();
			if(ifaces != null && ifaces.Length > 0) {
				foreach(Type iface in ifaces) {
					Type ienum = FindIEnumerable(iface);
					if(ienum != null) return ienum;
				}
			}
			if(seqType.BaseType != null && seqType.BaseType != typeof(object)) {
				return FindIEnumerable(seqType.BaseType);
			}
			return null;
		}
		public static string ProcessTypeName(string namespaceName, Type type) {
			return ProcessTypeName(namespaceName, type.FullName);
		}
		public static string ProcessTypeName(string namespaceName, string typeName) {
			string name;
			if(typeName.Length > namespaceName.Length && typeName.Substring(0, namespaceName.Length) == namespaceName)
				name = typeName.Substring(namespaceName.Length + 1).Replace(".", "_").Replace("+", "_");
			else
				name = typeName.Replace(".", "_").Replace("+", "_");
			return name;
		}
		public static bool AreConvertable(Type type1, Type type2) {
			if(type1 == null || type2 == null) return false;
			bool isExists;
			lock(convertableType) {
				if(convertableType.TryGetValue(new KeyValuePair<Type, Type>(type1, type2), out isExists)) return isExists;
				isExists = CheckConvertable(type1, type2);
				convertableType.Add(new KeyValuePair<Type, Type>(type1, type2), isExists);
			}
			return isExists;
		}
		static bool CheckConvertable(Type type1, Type type2) {
			if(type1 == type2) return true;
			Type tempType = type2;
			do {
				if(tempType.IsAssignableFrom(type1))
					return true;
				tempType = tempType.BaseType;
			} while(tempType != null && tempType != typeof(object));
			tempType = type1;
			do {
				if(tempType.IsAssignableFrom(type2))
					return true;
				tempType = tempType.BaseType;
			} while(tempType != null && tempType != typeof(object));
			return false;
		}
		internal static bool IsEntity(Type type, XpoMetadata metadata) {
			ResourceType resType;
			return metadata.TryResolveResourceTypeByType(type, out resType);
		}
		internal static bool IsStruct(Type type, XpoMetadata metadata) {
			return metadata.ContainsStructWrapper(type);
		}
		internal static bool IsQueryableType(Type collectionType) {
			if(!collectionType.IsGenericTypeDefinition) {
				if(!collectionType.IsGenericType) return false;
				collectionType = collectionType.GetGenericTypeDefinition();
			}
			return typeof(IQueryable<>).IsAssignableFrom(collectionType) || collectionType.GetInterfaces().Any(inf => inf.IsGenericType && typeof(IQueryable<>) == inf.GetGenericTypeDefinition());
		}
		internal static bool IsEnumerableType(Type collectionType) {
			if(!collectionType.IsGenericTypeDefinition) {
				if(!collectionType.IsGenericType) return false;
				collectionType = collectionType.GetGenericTypeDefinition();
			}
			return typeof(IEnumerable<>).IsAssignableFrom(collectionType) || collectionType.GetInterfaces().Any(inf => inf.IsGenericType && typeof(IEnumerable<>) == inf.GetGenericTypeDefinition());
		}
	}
}
