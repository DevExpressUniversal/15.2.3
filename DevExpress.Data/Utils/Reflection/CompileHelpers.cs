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
using System.Text;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading;
using DevExpress.Utils;
using System.Collections.Concurrent;
namespace DevExpress.Data.Helpers {
	public static class CompileHelper {
		public static bool CanDynamicMethodWithSkipVisibility() {
#if DXPORTABLE
			return true;
#else
			return !SecurityHelper.IsPartialTrust;
#endif
		}
		static bool IsPoisonousTypeBuilder(Type type) {
			try {
				type.TypeHandle.GetHashCode();
			} catch(NotSupportedException) {
				return true;
			}
			return false;
		}
		static readonly ConcurrentDictionary<Type, bool> _isPublicCache = new ConcurrentDictionary<Type, bool>();
		public static bool IsPublicExposable(Type t) {
			if(t == null)
				return false;
			return _isPublicCache.GetOrAdd(t, DecideIsPublicExposable);
		}
		static bool DecideIsPublicExposable(Type t) {
			if(t == null)
				return false;
			if(t.IsGenericType())
				return DecideIsPublicExposableGeneric(t, new Dictionary<Type, object>());
			else
				return DecideIsPublicExposableCore(t);
		}
		static bool DecideIsPublicExposableCore(Type t) {
			return t.IsVisible() && !IsPoisonousTypeBuilder(t);
		}
		static bool DecideIsPublicExposableGeneric(Type t, Dictionary<Type, object> visited) {
			if(visited.ContainsKey(t))
				return true;
			if(!DecideIsPublicExposableCore(t))
				return false;
			visited.Add(t, null);
			return t.GetGenericArguments().All(subType => subType.IsGenericType() ? DecideIsPublicExposableGeneric(subType, visited) : IsPublicExposable(subType));
		}
		static IEnumerable<Type> FromDescendatsToRoots(Type t) {
			if(t.IsInterface()) {
				yield return t;
				foreach(Type i in t.GetInterfaces())
					yield return i;
			} else {
				for(Type current = t; current != null; current = current.GetBaseType())
					yield return current;
			}
		}
		static IEnumerable<MemberInfo> GetMembers(Type t, string memberName, BindingFlags flags) {
#if DXPORTABLE
			MemberInfo p = null;
			if ((flags & BindingFlags.IgnoreCase) != 0) {
				foreach (PropertyInfo info in t.GetProperties(flags)) {
					if (String.Compare(info.Name, memberName, StringComparison.OrdinalIgnoreCase) == 0) {
						p = info;
						break;
					}
				}
			}
			else {
				foreach (PropertyInfo info in t.GetProperties(flags)) {
					if (info.Name == memberName) {
						p = info;
						break;
					}
				}
			}
#else
			MemberInfo p = t.GetProperty(memberName, flags, null, null, Type.EmptyTypes, null);
#endif
			if(p != null)
				yield return p;
			MemberInfo f = t.GetField(memberName, flags);
			if(f != null)
				yield return f;
		}
		public static MemberInfo FindPropertyOrField(Type typeToSearchFrom, string property, bool includeNonPublic, bool ignoreCase) {
			var enumerable = FromDescendatsToRoots(typeToSearchFrom).SelectMany(t => GetMembers(t, property, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public));
			if(includeNonPublic)
				enumerable = enumerable.Union(FromDescendatsToRoots(typeToSearchFrom).SelectMany(t => GetMembers(t, property, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic)));
			if(ignoreCase)
				enumerable = enumerable.Union(FromDescendatsToRoots(typeToSearchFrom).SelectMany(t => GetMembers(t, property, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)));
			if(includeNonPublic && ignoreCase)
				enumerable = enumerable.Union(FromDescendatsToRoots(typeToSearchFrom).SelectMany(t => GetMembers(t, property, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase)));
			var mi = enumerable.FirstOrDefault();
			return mi;
		}
   }
}
