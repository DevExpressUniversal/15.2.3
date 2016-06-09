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
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
namespace DevExpress.PivotGrid.ServerMode.Queryable {
	static class LinqExpressionHelper {
		public static IQueryable MakeGroupBy(IQueryable q, IEnumerable<CriteriaOperator> subj, IPivotCriteriaToExpressionConverter converter) {
			ParameterExpression param = Expression.Parameter(q.ElementType, "");
			List<Expression> list = subj.Select((op) => converter.Convert(param, op)).ToList();
			NewExpression groupByClassInit = CreateComplexObject(list);
			return q.MakeGroupBy(groupByClassInit, param);
		}
		public static NewExpression CreateComplexObject(List<Expression> list) {
			Type[] types = list.Select(expr => expr.Type).ToArray();
			Type realType = AnonymousClasses.TypesArray[list.Count].GetGenericTypeDefinition().MakeGenericType(types);
			MemberInfo[] setters = new MemberInfo[list.Count];
			for(int i = 0; i < list.Count; i++)
				setters[i] = realType.GetMethod("get_P" + i);
			NewExpression groupByClassInit = Expression.New(
					realType.GetConstructor(types),
					list,
					setters);
			return groupByClassInit;
		}
		public static IQueryable MakeGroupBy(this IQueryable q, Expression groupByGetter, ParameterExpression param) {
			LambdaExpression lambda = Expression.Lambda(groupByGetter, param ?? Expression.Parameter(q.ElementType, ""));
			MethodCallExpression callSelect =
				Expression.Call(typeof(System.Linq.Queryable), "GroupBy",
					new Type[] { q.ElementType, lambda.ReturnType },
					q.Expression, lambda);
			return q.Provider.CreateQuery(callSelect);
		}
		public static IQueryable MakeOrderBy(this IQueryable q, LambdaExpression sortBy, bool asc) {
			return q.Provider.CreateQuery(Expression.Call(typeof(System.Linq.Queryable), asc ? "OrderBy" : "OrderByDescending",
				new Type[] {
					q.ElementType, sortBy.ReturnType },
				   q.Expression, sortBy));
		}
		public static IQueryable MakeSelect(this IQueryable q, ParameterExpression parameter, Expression selectList) {
			LambdaExpression lambda = Expression.Lambda(selectList, parameter);
			Expression callSelect = Expression.Call(typeof(System.Linq.Queryable), "Select",
						new Type[] { q.ElementType, lambda.Body.Type },
									q.Expression, Expression.Quote(lambda));
			return q.Provider.CreateQuery(callSelect);
		}
		public static IEnumerable<object[]> DoSelectSeveral(IQueryable q, ParameterExpression parameter, IList<Expression> selectList, bool useEntity) {
			if(selectList.Count > 0 && selectList.Count <= AnonymousClasses.TypesArray.Length) {
				try {
					return DoSelectOneMS362794(q, parameter, selectList);
				} catch {
					return DoSelectComplexMS362794(q, parameter, selectList, useEntity);
				}
			} else
				return DoSelectComplexMS362794(q, parameter, selectList, useEntity);
		}
		static IEnumerable<object[]> DoSelectComplexMS362794(IQueryable q, ParameterExpression parameter, IList<Expression> selectList, bool useEntity) {
			if(useEntity) {
				int allCount = Convert.ToInt32(Math.Ceiling(selectList.Count / 15d));
				int lastCount = selectList.Count - (allCount - 1) * 15;
				IQueryable query = q.MakeSelect(parameter, InitMember(
											Enumerable.Range(0, allCount).Select((i) => InitMember(selectList.Skip(i * 15).Take((i == allCount - 1) ? lastCount : 15).ToList())).ToList()
																							   ));
				List<object[]> data = new List<object[]>();
				foreach(MS362794 r in query) {
					object[] all = new object[selectList.Count];
					for(int i = 0; i < allCount; i++)
						Array.Copy(((MS362794)r.Container[i]).Container, 0, all, i * 15, (i == allCount - 1) ? lastCount : 15);
					data.Add(all);
				}
				return data;
			} else
				return (IQueryable<object[]>)q.MakeSelect(parameter,
												Expression.NewArrayInit(typeof(object), selectList.Select(expr => Expression.Convert(expr, typeof(object))).ToArray()));
		}
		static IEnumerable<object[]> DoSelectOneMS362794(IQueryable q, ParameterExpression parameter, IList<Expression> selectList) {
			foreach(MS362794 r in q.MakeSelect(parameter, InitMember(selectList)))
				yield return r.Container;
		}
		static Expression InitMember(IList<Expression> selectList) {
			Type genericType = SummaryWorkaroundForMS362794.TypesArray[selectList.Count];
			Type realType = genericType.MakeGenericType(selectList.Select(expr => expr.Type).ToArray());
			List<MemberBinding> bindings = new List<MemberBinding>(selectList.Count);
			for(int i = 0; i < selectList.Count; ++i)
				bindings.Add(Expression.Bind(realType.GetProperty("P" + i.ToString()), selectList[i]));
			return Expression.MemberInit(Expression.New(realType), bindings);
		}
	}
}
