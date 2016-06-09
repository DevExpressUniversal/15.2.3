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
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	class QueryHelper {
		internal static IEnumerable<T> GetValues<T>(object dataSource, string valueMember) {
			if(dataSource == null)
				return Enumerable.Empty<T>();
			if(string.IsNullOrEmpty(valueMember))
				return Materialize(dataSource, dataSource is Array) as IEnumerable<T>;
			return Materialize(Select(dataSource, valueMember, typeof(T)), dataSource is Array) as IEnumerable<T>;
		}
		internal static object TakeDistinct<T>(object dataSource, int? count) {
			if(dataSource == null)
				return null;
			bool isArray = dataSource is Array;
			dataSource = Distinct(dataSource, typeof(T));
			if(!count.HasValue || count.Value <= 0)
				return Materialize(dataSource, isArray);
			return Materialize(Take(dataSource, count.Value), isArray);
		}
		internal static int CountDistinct<T>(object dataSource) {
			return (dataSource != null) ? Count(Distinct(dataSource, typeof(T))) : 0;
		}
		static IDictionary<Type, Func<object, object>> ofTypeCache = new Dictionary<Type, Func<object, object>>();
		static MethodInfo enumerableOfTypeInfo = typeof(Enumerable)
			.GetMember("OfType", BindingFlags.Static | BindingFlags.Public).FirstOrDefault() as MethodInfo;
		static MethodInfo queryableOfTypeInfo = typeof(Queryable)
			.GetMember("OfType", BindingFlags.Static | BindingFlags.Public).FirstOrDefault() as MethodInfo;
		static IDictionary<Type, Func<object, int, object>> takeCache = new Dictionary<Type, Func<object, int, object>>();
		static MethodInfo enumerableTakeInfo = typeof(Enumerable)
			.GetMember("Take", BindingFlags.Static | BindingFlags.Public).First() as MethodInfo;
		static MethodInfo queryableTakeInfo = typeof(Queryable)
			.GetMember("Take", BindingFlags.Static | BindingFlags.Public).First() as MethodInfo;
		static object Take(object dataSource, int? count) {
			if(!count.HasValue)
				return dataSource;
			if(dataSource != null) {
				var dataSorceType = dataSource.GetType();
				Func<object, int, object> take;
				if(!takeCache.TryGetValue(dataSorceType, out take)) {
					Type elementType = GenericTypeHelper.GetElementType(dataSorceType);
					var eType = typeof(IEnumerable<>).MakeGenericType(elementType);
					var qType = typeof(IQueryable<>).MakeGenericType(elementType);
					bool isQueryable = qType.IsAssignableFrom(dataSorceType);
					MethodInfo mInfoTake = (isQueryable ? queryableTakeInfo : enumerableTakeInfo)
						.MakeGenericMethod(elementType);
					var pDataSource = Expression.Parameter(typeof(object), "dataSource");
					var pCount = Expression.Parameter(typeof(int), "count");
					var takeCall = Expression.Call(mInfoTake,
									Expression.Convert(pDataSource, isQueryable ? qType : eType), pCount);
					take = Expression.Lambda<Func<object, int, object>>(
						takeCall, pDataSource, pCount).Compile();
					takeCache.Add(dataSorceType, take);
				}
				return take(dataSource, count.Value);
			}
			return dataSource;
		}
		static IDictionary<Type, Func<object, int>> countCache = new Dictionary<Type, Func<object, int>>();
		static MethodInfo enumerableCountInfo = typeof(Enumerable)
			.GetMember("Count", BindingFlags.Static | BindingFlags.Public).First() as MethodInfo;
		static MethodInfo queryableCountInfo = typeof(Queryable)
			.GetMember("Count", BindingFlags.Static | BindingFlags.Public).First() as MethodInfo;
		static int Count(object dataSource) {
			if(dataSource != null) {
				var dataSorceType = dataSource.GetType();
				Func<object, int> count;
				if(!countCache.TryGetValue(dataSorceType, out count)) {
					Type elementType = GenericTypeHelper.GetElementType(dataSorceType);
					var eType = typeof(IEnumerable<>).MakeGenericType(elementType);
					var qType = typeof(IQueryable<>).MakeGenericType(elementType);
					bool isQueryable = qType.IsAssignableFrom(dataSorceType);
					MethodInfo mInfoCount = (isQueryable ? queryableCountInfo : enumerableCountInfo)
						.MakeGenericMethod(elementType);
					var pDataSource = Expression.Parameter(typeof(object), "dataSource");
					var countCall = Expression.Call(mInfoCount,
									Expression.Convert(pDataSource, isQueryable ? qType : eType));
					count = Expression.Lambda<Func<object, int>>(
						countCall, pDataSource).Compile();
					countCache.Add(dataSorceType, count);
				}
				return count(dataSource);
			}
			return 0;
		}
		static IDictionary<Type, Func<object, object>> distinctCache = new Dictionary<Type, Func<object, object>>();
		static MethodInfo enumerableDistinctInfo = typeof(Enumerable)
			.GetMember("Distinct", BindingFlags.Static | BindingFlags.Public).First() as MethodInfo;
		static MethodInfo queryableDistinctInfo = typeof(Queryable)
			.GetMember("Distinct", BindingFlags.Static | BindingFlags.Public).First() as MethodInfo;
		static object Distinct(object dataSource, Type valueType) {
			if(dataSource != null) {
				var dataSorceType = dataSource.GetType();
				Func<object, object> distinct;
				if(!distinctCache.TryGetValue(dataSorceType, out distinct)) {
					Type elementType = GenericTypeHelper.GetElementType(dataSorceType);
					var eType = typeof(IEnumerable<>).MakeGenericType(elementType);
					var qType = typeof(IQueryable<>).MakeGenericType(elementType);
					bool isQueryable = qType.IsAssignableFrom(dataSorceType);
					MethodInfo mInfoDistinct = (isQueryable ? queryableDistinctInfo : enumerableDistinctInfo)
						.MakeGenericMethod(elementType);
					MethodInfo mInfoOfType = (isQueryable ? queryableOfTypeInfo : enumerableOfTypeInfo)
						.MakeGenericMethod(elementType);
					var pDataSource = Expression.Parameter(typeof(object), "dataSource");
					var convert = Expression.Convert(pDataSource, isQueryable ? qType : eType);
					var distinctCall = TypeHelper.AllowNull(valueType) ?
						Expression.Call(mInfoDistinct, convert) :
						Expression.Call(mInfoDistinct, Expression.Call(mInfoOfType, convert));
					distinct = Expression.Lambda<Func<object, object>>(
						distinctCall, pDataSource).Compile();
					distinctCache.Add(dataSorceType, distinct);
				}
				return distinct(dataSource);
			}
			return dataSource;
		}
		static IDictionary<Type, Func<object, object>> selectCache = new Dictionary<Type, Func<object, object>>();
		static MethodInfo enumerableSelectInfo = typeof(Enumerable)
			.GetMember("Select", BindingFlags.Static | BindingFlags.Public).FirstOrDefault() as MethodInfo;
		static MethodInfo queryableSelectInfo = typeof(Queryable)
			.GetMember("Select", BindingFlags.Static | BindingFlags.Public).FirstOrDefault() as MethodInfo;
		static object Select(object dataSource, string valueMember, Type valueType) {
			if(dataSource != null) {
				var dataSorceType = dataSource.GetType();
				Func<object, object> select;
				if(!selectCache.TryGetValue(dataSorceType, out select)) {
					Type elementType = GenericTypeHelper.GetElementType(dataSorceType);
					var eType = typeof(IEnumerable<>).MakeGenericType(elementType);
					var qType = typeof(IQueryable<>).MakeGenericType(elementType);
					bool isQueryable = qType.IsAssignableFrom(dataSorceType);
					MethodInfo mInfoSelect = (isQueryable ? queryableSelectInfo : enumerableSelectInfo)
						.MakeGenericMethod(elementType, valueType);
					var pDataSource = Expression.Parameter(typeof(object), "dataSource");
					var pInfo = elementType.GetProperty(valueMember);
					var x = System.Linq.Expressions.Expression.Parameter(elementType, "x");
					Expression selector = Expression.Lambda(x, x);
					if(pInfo != null) {
						selector = Expression.Lambda(
							Expression.MakeMemberAccess(x, pInfo), x);
					}
					MethodInfo mInfoOfType = (isQueryable ? queryableOfTypeInfo : enumerableOfTypeInfo)
						.MakeGenericMethod(elementType);
					var convert = Expression.Convert(pDataSource, isQueryable ? qType : eType);
					var selectCall = TypeHelper.AllowNull(valueType) ?
						Expression.Call(mInfoSelect, convert, selector) :
						Expression.Call(mInfoSelect, Expression.Call(mInfoOfType, convert), selector);
					select = Expression.Lambda<Func<object, object>>(
						selectCall, pDataSource).Compile();
					selectCache.Add(dataSorceType, select);
				}
				return select(dataSource);
			}
			return dataSource;
		}
		static IDictionary<Type, Func<object, object>> materializationCache = new Dictionary<Type, Func<object, object>>();
		static MethodInfo enumerableToListInfo = typeof(Enumerable)
			.GetMember("ToList", BindingFlags.Static | BindingFlags.Public).First() as MethodInfo;
		static MethodInfo enumerableToArrayInfo = typeof(Enumerable)
			.GetMember("ToArray", BindingFlags.Static | BindingFlags.Public).First() as MethodInfo;
		static object Materialize(object dataSource, bool forceToArray = false) {
			if(dataSource != null) {
				var dataSorceType = dataSource.GetType();
				Func<object, object> materialize;
				if(!materializationCache.TryGetValue(dataSorceType, out materialize)) {
					Type elementType = GenericTypeHelper.GetElementType(dataSorceType);
					var eType = typeof(IEnumerable<>).MakeGenericType(elementType);
					MethodInfo mInfoMaterialize = (forceToArray || dataSource is Array) ?
						enumerableToArrayInfo : enumerableToListInfo;
					mInfoMaterialize = mInfoMaterialize.MakeGenericMethod(elementType);
					var pDataSource = Expression.Parameter(typeof(object), "dataSource");
					var materializeCall = Expression.Call(mInfoMaterialize,
									Expression.Convert(pDataSource, eType));
					materialize = Expression.Lambda<Func<object, object>>(
						materializeCall, pDataSource).Compile();
					materializationCache.Add(dataSorceType, materialize);
				}
				return materialize(dataSource);
			}
			return dataSource;
		}
	}
}
