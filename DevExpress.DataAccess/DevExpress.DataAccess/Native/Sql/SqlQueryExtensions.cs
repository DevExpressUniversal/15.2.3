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
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.DataAccess.Native.Sql {
	public static class SqlQueryExtensions {
		#region inner classes
		class SelectSqlGeneratorWithAlias : SelectSqlGenerator {
			readonly TableQuery query;
			Dictionary<string, int> columnCounters;
			public SelectSqlGeneratorWithAlias(ISqlGeneratorFormatter formatter, TableQuery query)
				: base(formatter) {
				this.query = query;
			}
			#region Overrides of SelectSqlGenerator
			protected override string InternalGenerateSql() {
				this.columnCounters = this.query.Tables.ToDictionary(t => t.ActualName, _ => 0);
				return base.InternalGenerateSql();
			}
			public override string GetNextParameterName(OperandValue parameter) {
				OperandParameter operandParameter = parameter as OperandParameter;
				return base.GetNextParameterName(!ReferenceEquals(operandParameter, null) ? new OperandParameter(operandParameter.ParameterName, new object()) : parameter);
			}
			protected override string PatchProperty(CriteriaOperator propertyOperator, string propertyString) {
				QueryOperand operand = propertyOperator is QuerySubQueryContainer ? ((QuerySubQueryContainer)propertyOperator).AggregateProperty as QueryOperand : propertyOperator as QueryOperand;
				if(ReferenceEquals(operand, null))
					return propertyString;
				TableInfo table = this.query.Tables.FirstOrDefault(t => String.Equals(operand.NodeAlias, t.ActualName, StringComparison.Ordinal));
				if(table == null)
					return propertyString;
				int colIndex = this.columnCounters[operand.NodeAlias]++;
				ColumnInfo column = table.SelectedColumns[colIndex];
				if(column == null)
					return propertyString;
				return column.HasAlias ? String.Format("{0} as {1}", propertyString, this.formatter.FormatColumn(column.Alias)) : propertyString;
			}
			#endregion
		}
		class SelectStatementBuilderHelper {
			readonly Dictionary<string, DBTable> dbTables;
			readonly ISqlGeneratorFormatter sqlFormatter;
			public SelectStatementBuilderHelper(ISqlGeneratorFormatter sqlFormatter, DBSchema dbSchema) {
				Guard.ArgumentNotNull(dbSchema, "dbSchema");
				this.sqlFormatter = sqlFormatter;
				this.dbTables = dbSchema.Tables.Union(dbSchema.Views).ToDictionary(t => t.Name);
			}
			public string BuildSql(TableQuery query) {
				if(this.sqlFormatter == null)
					throw new InvalidOperationException();
				if(query.Tables.Count == 0)
					return "select 1";
				SelectStatement selectStatement = BuildSelectStatement(query, query.Parameters);
				return new SelectSqlGeneratorWithAlias(this.sqlFormatter, query).GenerateSql(selectStatement).Sql;
			}
			public SelectStatement BuildSelectStatement(TableQuery query, IEnumerable<IParameter> parameters) {
				SelectStatement result = BuildSelectStatementCore(query);
				IList<IParameter> parametersList = parameters == null
					? new IParameter[0]
					: (parameters as IList<IParameter> ?? parameters.ToList());
				if(!String.IsNullOrEmpty(query.FilterString))
					result.Condition = FilterHelper.GetFilter(query.FilterString, parametersList);
				if(query.Top > 0)
					result.TopSelectedRecords = query.Top;
				if(query.Skip > 0 && query.Sorting.Count != 0)
					result.SkipSelectedRecords = query.Skip;
				result.SortProperties.AddRange(
					query.Sorting.Select(
						info => new SortingColumn(info.Column, info.Table, (SortingDirection)info.Direction)));
				result.GroupProperties.AddRange(query.Grouping.Select(info => new QueryOperand(this.dbTables[info.Table].GetColumn(info.Column), info.Table)));
				if(!string.IsNullOrEmpty(query.GroupFilterString))
					result.GroupCondition = FilterHelper.GetFilter(query.GroupFilterString, parametersList).Accept(new HavingCriteriaPatcher(query.Tables));
				return result;
			}
			SelectStatement BuildSelectStatementCore(TableQuery tableQuery) {
				TableInfoList tables = tableQuery.Tables;
				if(!tables.Any())
					throw new NoTablesValidationException();
				RelationInfoList relations = tableQuery.Relations;
				if(!relations.Any()) {
					if(tables.Count == 1)
						return BuildSelectStatement(tables[0], tables);
					throw new TablesNotRelatedValidationException(tables.Select(t => t.ActualName).ToArray());
				}
				Dictionary<string, TableInfo> tablesInfos = tables.ToDictionary(t => t.ActualName);
				Dictionary<string, List<string>> depGraph = tables.ToDictionary(
					t => t.ActualName,
					t => relations.Where(r => String.Equals(r.NestedTable, t.ActualName, StringComparison.Ordinal)).Select(r => r.ParentTable).ToList());
				string rootAlias = depGraph.First(pair => pair.Value.Count == 0).Key;
				SelectStatement result = BuildSelectStatement(tablesInfos[rootAlias], tables);
				StrikeoutDagNode(rootAlias, depGraph);
				while(depGraph.Any()) {
					string node = depGraph.First(pair => pair.Value.Count == 0).Key;
					result.SubNodes.Add(BuildJoinNode(tablesInfos, tablesInfos[node], relations.Where(r => String.Equals(r.NestedTable, node, StringComparison.Ordinal))));
					StrikeoutDagNode(node, depGraph);
				}
				return result;
			}
			static void StrikeoutDagNode(string node, Dictionary<string, List<string>> graph) {
				graph.Remove(node);
				foreach(List<string> curves in graph.Values)
					curves.RemoveAll(s => s == node);
			}
			SelectStatement BuildSelectStatement(TableInfo rootTable, IEnumerable<TableInfo> tables) {
				DBTable rootDbTable = this.dbTables[rootTable.Name];
				SelectStatement res = new SelectStatement(rootDbTable, rootTable.ActualName);
				foreach(TableInfo table in tables)
					res.Operands.AddRange(BuildQueryOperands(this.dbTables[table.Name], table.ActualName, table.SelectedColumns));
				return res;
			}
			static IEnumerable<CriteriaOperator> BuildQueryOperands(DBTable dbTable, string tableActualName, IEnumerable<ColumnInfo> columns) {
				foreach(ColumnInfo column in columns) {
					QueryOperand queryOperand = new QueryOperand(column.Name, tableActualName, dbTable.GetColumn(column.Name).ColumnType);
					if(column.Aggregation == AggregationType.None)
						yield return queryOperand;
					else
						yield return new QuerySubQueryContainer(null, queryOperand, (Aggregate)column.Aggregation);
				}
			}
			JoinNode BuildJoinNode(Dictionary<string, TableInfo> tables, TableInfo table, IEnumerable<RelationInfo> relations) {
				IList<RelationInfo> rels = relations as IList<RelationInfo> ?? relations.ToList();
				JoinType joinType = rels.First().JoinType;
				if(rels.Any(r => r.JoinType != joinType))
					throw new InvalidOperationException(String.Format("Indeterminate join type for table '{0}'", table.ActualName));
				DBTable nestedDbTable = this.dbTables[table.Name];
				return new JoinNode(nestedDbTable, table.ActualName, joinType) {
					Condition = CriteriaOperator.And(rels.SelectMany(relation => {
						TableInfo parentTable = tables[relation.ParentTable];
						DBTable parentDbTable = this.dbTables[parentTable.Name];
						return relation.KeyColumns.Select(keyColumn => {
							string nestedColumnName = keyColumn.NestedKeyColumn;
							ColumnInfo nestedColumn = FindColumnInfo(table, nestedColumnName);
							if(nestedColumn != null)
								nestedColumnName = nestedColumn.Name;
							DBColumn nestedDbColumn = nestedDbTable.GetColumn(nestedColumnName);
							QueryOperand nestedOperand = new QueryOperand(nestedDbColumn, relation.NestedTable);
							string parentColumnName = keyColumn.ParentKeyColumn;
							ColumnInfo parentColumn = FindColumnInfo(parentTable, parentColumnName);
							if(parentColumn != null)
								parentColumnName = parentColumn.Name;
							DBColumn parentDbColumn = parentDbTable.GetColumn(parentColumnName);
							QueryOperand parentOperand = new QueryOperand(parentDbColumn, relation.ParentTable);
							return new BinaryOperator(nestedOperand, parentOperand, (BinaryOperatorType)keyColumn.ConditionOperator);
						});
					}))
				};
			}
			static ColumnInfo FindColumnInfo(TableInfo table, string actualName) {
				return table.SelectedColumns.FirstOrDefault(c => String.Equals(c.ActualName, actualName, StringComparison.Ordinal));
			}
		}
		#endregion
		public static SelectStatement BuildSelectStatement(this TableQuery tableQuery, DBSchema dbSchema, ISqlGeneratorFormatter sqlFormatter, IEnumerable<IParameter> parameters) {
			return new SelectStatementBuilderHelper(sqlFormatter, dbSchema).BuildSelectStatement(tableQuery, parameters);
		}
		public static string BuildSql(this TableQuery query, DBSchema dbSchema, ISqlGeneratorFormatter sqlFormatter) {
			return new SelectStatementBuilderHelper(sqlFormatter, dbSchema).BuildSql(query);
		}
		public static bool IsTopThousandApplicable(this SqlQuery query) {
			return query is TableQuery; 
		}
		public static object GetDataSchema(this TableQuery query, DBSchema dbSchema) {
			Dictionary<string, DBTable> dbTables = dbSchema.Tables.Union(dbSchema.Views).ToDictionary(t => t.Name);
			ResultTable result = new ResultTable(query.Name);
			foreach(TableInfo table in query.Tables)
				foreach(ColumnInfo column in table.SelectedColumns)
					result.AddColumn(column.ActualName, DBColumn.GetType(dbTables[table.Name].GetColumn(column.Name).ColumnType));
			return result;
		}
	}
}
