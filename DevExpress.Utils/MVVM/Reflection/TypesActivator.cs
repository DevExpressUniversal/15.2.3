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

namespace DevExpress.Utils.MVVM {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using BF = System.Reflection.BindingFlags;
	class TypesActivator {
		static IDictionary<Type, Type> typesCache = new Dictionary<Type, Type>();
		internal Type CreateType(Type sourceType, Func<Type, Type> createType) {
			Type result;
			if(!typesCache.TryGetValue(sourceType, out result)) {
				result = createType(sourceType);
				typesCache.Add(sourceType, result);
			}
			return result;
		}
		IDictionary<Type, Delegate> createCache = new Dictionary<Type, Delegate>();
		internal object Create(Type type) {
			Delegate create = null;
			if(!createCache.TryGetValue(type, out create)) {
				var ctorInfo = FindConstructor(type);
				var ctorExpr = Expression.New(ctorInfo);
				create = Expression.Lambda(ctorExpr).Compile();
				createCache.Add(type, create);
			}
			return ((Func<object>)create)();
		}
		IDictionary<Type, Delegate> createCache0 = new Dictionary<Type, Delegate>();
		internal T Create<T>(Func<Type, Type> createType) {
			Delegate create = null;
			if(!createCache0.TryGetValue(typeof(T), out create)) {
				Type type = CreateType(typeof(T), createType);
				var ctorInfo = FindConstructor(type);
				var ctorExpr = Expression.New(ctorInfo);
				create = Expression.Lambda(ctorExpr).Compile();
				createCache0.Add(typeof(T), create);
			}
			return ((Func<T>)create)();
		}
		IDictionary<int, Delegate> createCache1 = new Dictionary<int, Delegate>();
		internal T Create<T, P1>(Func<Type, Type> createType, P1 parameter1) {
			int hash = Types.CalcHashCode(typeof(T), typeof(P1));
			Delegate create = null;
			if(!createCache1.TryGetValue(hash, out create)) {
				Type type = CreateType(typeof(T), createType);
				var ctorInfo = type.GetConstructor(new Type[] { typeof(P1) });
				ParameterExpression p1 = Expression.Parameter(typeof(P1));
				var ctorExpr = Expression.New(ctorInfo, p1);
				create = Expression.Lambda(ctorExpr, p1).Compile();
				createCache1.Add(hash, create);
			}
			return ((Func<P1, T>)create)(parameter1);
		}
		IDictionary<int, Delegate> createCache2 = new Dictionary<int, Delegate>();
		internal T Create<T, P1, P2>(Func<Type, Type> createType, P1 parameter1, P2 parameter2) {
			int hash = Types.CalcHashCode(typeof(T), typeof(P1), typeof(P2));
			Delegate create = null;
			if(!createCache2.TryGetValue(hash, out create)) {
				Type type = CreateType(typeof(T), createType);
				var ctorInfo = type.GetConstructor(new Type[] { typeof(P1), typeof(P2) });
				ParameterExpression p1 = Expression.Parameter(typeof(P1));
				ParameterExpression p2 = Expression.Parameter(typeof(P2));
				var ctorExpr = Expression.New(ctorInfo, p1, p2);
				create = Expression.Lambda(ctorExpr, p1, p2).Compile();
				createCache2.Add(hash, create);
			}
			return ((Func<P1, P2, T>)create)(parameter1, parameter2);
		}
		IDictionary<int, Delegate> createCache3 = new Dictionary<int, Delegate>();
		internal T Create<T, P1, P2, P3>(Func<Type, Type> createType, P1 parameter1, P2 parameter2, P3 parameter3) {
			int hash = Types.CalcHashCode(typeof(T), typeof(P1), typeof(P2), typeof(P3));
			Delegate create = null;
			if(!createCache3.TryGetValue(hash, out create)) {
				Type type = CreateType(typeof(T), createType);
				var ctorInfo = type.GetConstructor(new Type[] { typeof(P1), typeof(P2), typeof(P3) });
				ParameterExpression p1 = Expression.Parameter(typeof(P1));
				ParameterExpression p2 = Expression.Parameter(typeof(P2));
				ParameterExpression p3 = Expression.Parameter(typeof(P3));
				var ctorExpr = Expression.New(ctorInfo, p1, p2, p3);
				create = Expression.Lambda(ctorExpr, p1, p2, p3).Compile();
				createCache3.Add(hash, create);
			}
			return ((Func<P1, P2, P3, T>)create)(parameter1, parameter2, parameter3);
		}
		internal void ResetCache() {
			createCache.Clear();
		}
		internal void ResetCache(Type type) {
			if(type != null)
				createCache.Remove(type);
		}
		internal void ResetTypesCache(Type type) {
			if(type != null)
				typesCache.Remove(type);
		}
		internal void Reset() {
			ResetCache();
			createCache0.Clear();
			createCache1.Clear();
			createCache2.Clear();
			createCache3.Clear();
		}
		internal static ConstructorInfo FindConstructor(Type type) {
			return type.GetConstructor(Type.EmptyTypes) ??
				FindConstructorWithAllOptionalParameters(type);
		}
		internal static ConstructorInfo FindConstructorWithAllOptionalParameters(Type type) {
			return type.GetConstructors(BF.Public | BF.NonPublic | BF.Instance)
							.FirstOrDefault(x => x.IsPublicOrFamily() && x.AllOptionalParameters());
		}
		static class Types {
			internal static int CalcHashCode(params Type[] types) {
				int[] hashCodes = new int[types.Length];
				for(int i = 0; i < types.Length; i++)
					hashCodes[i] = types[i].GetHashCode();
				return HashCodeHelper.CalcHashCode2(hashCodes);
			}
		}
	}
}
