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
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Linq.Helpers;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.Xpo.DB;
using DevExpress.Utils;
using System.Threading;
namespace DevExpress.PivotGrid.ServerMode.Queryable {
	public class QueryableQueryExecutor : IPivotQueryExecutor {
		static readonly string[] entityClasses = new string[] { "ObjectSet`1", "DbSet`1", "DbQuery`1" };
		static readonly string linq2sqlAttribute = "System.Data.Linq.Mapping.TableAttribute";
		readonly IPivotCriteriaToExpressionConverter converter;
		readonly IQueryable queryable;
		readonly bool isEntity;
		readonly bool isLinq2Sql;
		static QueryableQueryExecutor() {
		}
		public QueryableQueryExecutor(IQueryable queryAble) {
			this.queryable = queryAble;
			if(queryAble == null)
				return;
			Type queryableBaseType = queryable.GetType().GetGenericTypeDefinition();
			this.isEntity = entityClasses.Contains(queryableBaseType.Name);
			this.isLinq2Sql = queryable.ElementType.GetCustomAttributes(true).Any((att) => att.GetType().FullName.StartsWith(linq2sqlAttribute));
			converter = new CriteriaToExpressionConverter(isEntity, isLinq2Sql, queryableBaseType.GetAssembly());
		}
		CriteriaSyntax IPivotQueryExecutor.CriteriaSyntax {
			get { return CriteriaSyntax.ClientCriteria | CriteriaSyntax.SupportsNestedSummaries | CriteriaSyntax.SupportsTopNPercentage; }
		}
		bool IPivotQueryExecutor.Connected {
			get { return queryable != null; }
		}
		IEnumerable<ServerModeColumnModel> IPivotQueryExecutor.GetColumns() {
			List<ServerModeColumnModel> models = new List<ServerModeColumnModel>();
			return AddInfo(models, new PickManager(queryable).ConstructTree(), string.Empty, string.Empty);
		}
		IEnumerable<ServerModeColumnModel> AddInfo(List<ServerModeColumnModel> models, DevExpress.Data.Browsing.Design.INode cur, string folder, string dataMember) {
			foreach(PickManagerNode node in cur.ChildNodes) {
				Type dataType = Nullable.GetUnderlyingType(node.DataType);
				if(dataType == null)
					dataType = node.DataType;
				DBColumnType type = DBColumn.GetColumnType(dataType, true);
				string caption = string.IsNullOrEmpty(node.DisplayName) ? node.DataMember : node.DisplayName;
				if(type != DBColumnType.Unknown && !node.IsList)
					models.Add(new ServerModeColumnModel() { DataType = dataType, Name = node.DataMember, Caption = caption, DisplayFolder = folder});
				else
					AddInfo(models, node, string.IsNullOrEmpty(folder) ? caption : folder + "\\" + caption, dataMember + node.DataMember + ".");
			}
			return models;
		}
		Type IPivotQueryExecutor.ValidateCriteria(CriteriaOperator criteria, bool? exactType) {
			return CriteriaToExpressionConverterBase.ValidateCriteria(criteria, HasAggregateCriteriaChecker.Check(criteria) ? queryable.GetType() : queryable.ElementType, converter);
		}
		List<object> IPivotQueryExecutor.GetQueryResult(NestedSummaryServerModeQueryCriteria query) {
			if(queryable == null)
				return null;
			ParameterExpression elementParameter = Expression.Parameter(queryable.ElementType, "elem");
			IQueryable q = queryable;
			if(isEntity)
				converter.EnsureServerConstants(query, queryable);
			q = AppendFilter(query.Filter, q);
			IQueryable group = LinqExpressionHelper.MakeGroupBy(q, query.Grouping, converter);
			ParameterExpression aggregate0Parameter = Expression.Parameter(group.ElementType, "");
			Expression aggregate1 = converter.Convert(aggregate0Parameter, query.Operand);
			IQueryable select1 = group.MakeSelect(aggregate0Parameter, LinqExpressionHelper.CreateComplexObject(new List<Expression>() { aggregate1 }));
			Type realType = AnonymousClasses.TypesArray[1].GetGenericTypeDefinition().MakeGenericType(aggregate1.Type);
			ParameterExpression sel2par = Expression.Parameter(realType, "");
			IQueryable groupedbyconstant = select1.MakeGroupBy(Expression.Constant("0g", typeof(string)), null);
			ParameterExpression paramagg2 = Expression.Parameter(groupedbyconstant.ElementType, "elem");
			List<Expression> aggregates = new List<Expression>();
			foreach(AggregationItem ex in query.SummaryTypes) {
				aggregates.Add(CriteriaToExpressionConverterBase.GetSummaryExpression(paramagg2,
																							 Expression.Lambda(Expression.Property(sel2par, "P0"), sel2par),
																							 ex.ToAggregate(),
																							 realType, converter.EntityCriteriaHelper, converter.UseLinq2Sql));
			}
			return new List<object>(LinqExpressionHelper.DoSelectSeveral(groupedbyconstant, paramagg2, aggregates, isEntity).First());
		}
		IPivotQueryResult IPivotQueryExecutor.GetQueryResult(ServerModeQueryCriteria query, CancellationToken cancellationToken) {
			if(queryable == null)
				return null;
			ParameterExpression elementParameter = Expression.Parameter(queryable.ElementType, "elem");
			IQueryable q = queryable;
			if(isEntity)
				converter.EnsureServerConstants(query, queryable);
			q = AppendFilter(query.Filter, q);
			if(query.Grouping.Count > 0)
				q = LinqExpressionHelper.MakeGroupBy(q, query.Grouping, converter);
			else
				if(HasAggregateCriteriaChecker.Check(query.Operands.Skip(query.Grouping.Count)))
					q = q.MakeGroupBy(converter, new OperandValue("0g"));
			ParameterExpression selectParameter = Expression.Parameter(q.ElementType, "");
			if(query.Sort.Count > 0) {
				CriteriaOperator criteria = query.Sort[0].Property;
				AggregateOperand aggregate = criteria as AggregateOperand;
				Expression sortBy;
				if(ReferenceEquals(aggregate, null)) {
					sortBy = Expression.Property(Expression.Property(selectParameter, "Key"), "P" + (query.Grouping.FindIndex((op) => op.ToString() == criteria.ToString())).ToString());
				} else {
					sortBy = CriteriaToExpressionConverterBase.GetSummaryExpression(selectParameter,
																					Expression.Lambda(
																						converter.Convert(elementParameter, aggregate.AggregatedExpression),
																						elementParameter),
																					aggregate.AggregateType,
																					queryable.ElementType, converter.EntityCriteriaHelper, converter.UseLinq2Sql);
				}
				Expression orderedExpression = Expression.Call(typeof(System.Linq.Queryable), query.Sort[0].Direction == SortingDirection.Ascending ? "OrderBy" : "OrderByDescending",
					new Type[] { q.ElementType, sortBy.Type },
					q.Expression, Expression.Quote(Expression.Lambda(sortBy, selectParameter)));
				q = q.Provider.CreateQuery(orderedExpression);
			}
			if(query.SkipSelectedRecords > 0)
				q = q.Skip(query.SkipSelectedRecords);
			if(query.TopSelectedRecords > 0) {
				if(query.TopValuePercent) {
					int count = q.Count();
					int countToSelect = count * query.TopSelectedRecords / 100;
					if(countToSelect * 100 / count < query.TopSelectedRecords)
						countToSelect++;
					q = q.Take(countToSelect);
				} else
					q = q.Take(query.TopSelectedRecords);
			}
			List<string> groupingstring = query.Grouping.Select(g => g.ToString()).ToList();
			List<Expression> aggregates = query.Operands.Select((a) => {
				int index = groupingstring.IndexOf(a.ToString());
				if(index >= 0)
					return Expression.Property(Expression.Property(selectParameter, "Key"), "P" + index);
				else
					return converter.Convert(selectParameter, a);
			}).ToList();
			return new PivotQueryResult(LinqExpressionHelper.DoSelectSeveral(q, selectParameter, aggregates, isEntity), query.AggregatesStart >= 0 ? aggregates.Skip(query.AggregatesStart).Select((f) => f.Type).ToArray() : null);
		}
		IQueryable AppendFilter(CriteriaOperator filter, IQueryable q) {
			if(!object.ReferenceEquals(null, filter))
				return q.AppendWhere(converter, CriteriaToExpressionConverterBase.FixFilter(filter, queryable.ElementType, converter));
			return q;
		}
	}
}
