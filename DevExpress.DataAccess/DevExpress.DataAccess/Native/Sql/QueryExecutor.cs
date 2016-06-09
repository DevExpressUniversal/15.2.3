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
using System.Threading;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.DataAccess.Native.Sql {
	public sealed class QueryExecutor {
		public static void ApplyCustomFilter(SqlDataSource sqlDataSource, TableQuery query, IEnumerable<IParameter> parameters, SelectStatement selectStatement, CustomizeFilterExpressionEventHandler customFilterExpression) {
			if (customFilterExpression == null)
				return;
			CriteriaOperator filterEx = FilterHelper.GetCriteriaOperator(query.FilterString, parameters);
			CustomizeFilterExpressionEventArgs args = new CustomizeFilterExpressionEventArgs(query.Name, filterEx);
			customFilterExpression(sqlDataSource, args);
			CriteriaOperator filterExpression = args.FilterExpression;
			filterExpression = new OperandPropertyReplacingCriteriaOperatorPatcher(parameters).Process(filterExpression);
			selectStatement.Condition = filterExpression;
		}
		readonly SqlDataConnection connection;
		readonly CustomizeFilterExpressionEventHandler customFilterExpression;
		readonly IEnumerable<IParameter> sourceParameters;
		readonly Dictionary<string, DBTable> dbTables;
		readonly DBSchema dbSchema;
		readonly SqlDataSource sqlDataSource;
		public IEnumerable<IParameter> SourceParameters {
			get {  return this.sourceParameters; }   
		}
		public QueryExecutor(SqlDataConnection connection, DBSchema dbSchema) : this(connection, dbSchema, null) { }
		public QueryExecutor(SqlDataConnection connection, DBSchema dbSchema, CustomizeFilterExpressionEventHandler customFilterExpression) : this(connection, dbSchema, customFilterExpression, null) { }
		public QueryExecutor(SqlDataConnection connection, DBSchema dbSchema, CustomizeFilterExpressionEventHandler customFilterExpression, IEnumerable<IParameter> sourceParameters) : this(null, connection, dbSchema, customFilterExpression, sourceParameters) { }
		public QueryExecutor(SqlDataSource sqlDataSource, SqlDataConnection connection, DBSchema dbSchema, CustomizeFilterExpressionEventHandler customFilterExpression, IEnumerable<IParameter> sourceParameters) {
			Guard.ArgumentNotNull(connection, "connection");
			this.sqlDataSource = sqlDataSource;
			this.dbSchema = dbSchema;
			this.connection = connection;
			this.customFilterExpression = customFilterExpression;
			this.sourceParameters = sourceParameters;
			this.dbTables = dbSchema == null ? null : dbSchema.Tables.Union(dbSchema.Views).ToDictionary(t => t.Name);
		}
		public SelectedDataEx Execute(SqlQuery query, CancellationToken cancellationToken) { return Execute(query, false, cancellationToken); }
		public SelectedDataEx Execute(SqlQuery query, bool topThousand, CancellationToken cancellationToken) {
			Guard.ArgumentNotNull(query, "query");
			IEnumerable<IParameter> evalParameters = ParametersEvaluator.EvaluateParameters(this.sourceParameters, query.Parameters);
			TableQuery tableQuery = query as TableQuery;
			if(tableQuery != null) {
				return Execute(tableQuery, evalParameters, cancellationToken, topThousand);
			}
			CustomSqlQuery sqlQuery = query as CustomSqlQuery;
			if(sqlQuery != null)
				return Execute(sqlQuery, evalParameters, cancellationToken);
			StoredProcQuery spQuery = query as StoredProcQuery;
			if(spQuery != null)
				return Execute(spQuery, evalParameters, cancellationToken);
			throw new NotSupportedException(string.Format("Unknown query type: {0}", query.GetType()));
		}
		#region TableQuery
		SelectedDataEx Execute(TableQuery query, IEnumerable<IParameter> parameters, CancellationToken cancellationToken, bool topThousand) {
			IEnumerable<IParameter> queryParameters = parameters as IList<IParameter> ?? parameters.ToList();
			SelectStatement selectStatement = query.BuildSelectStatement(this.dbSchema, this.connection.SqlGeneratorFormatter, queryParameters);
			if(topThousand && (selectStatement.TopSelectedRecords == 0 || selectStatement.TopSelectedRecords > 1000))
				selectStatement.TopSelectedRecords = 1000;
			ApplyCustomFilter(sqlDataSource, query, queryParameters, selectStatement, customFilterExpression);
			cancellationToken.ThrowIfCancellationRequested();
			IDataStoreEx dataStoreEx = this.connection.DataStore as IDataStoreEx;
			if(dataStoreEx != null) {
				string[] columns = query.Tables.SelectMany(table => table.SelectedColumns, (table, column) => column.ActualName).ToArray();
				Query xpoQuery = new SelectSqlGenerator(this.connection.SqlGeneratorFormatter).GenerateSql(selectStatement);
				return dataStoreEx.SelectData(cancellationToken, xpoQuery, columns.ToArray());
			}
			List<ColumnInfoEx> schema = new List<ColumnInfoEx>();
			foreach(TableInfo table in query.Tables) {
				DBTable dbTable = this.dbTables[table.Name];
				foreach(ColumnInfo column in table.SelectedColumns) {
					DBColumn dbColumn = dbTable.GetColumn(column.Name);
					Type type;
					switch(column.Aggregation) {
						case AggregationType.Count:
							type = typeof(int);
							break;
						default:
							type = DBColumn.GetType(dbColumn.ColumnType);
							break;
					}
					schema.Add(new ColumnInfoEx{ Name = column.ActualName, Type = type} );
				}
			}
			SelectedData selectedData = this.connection.DataStore.SelectData(selectStatement);
			return DataStoreExHelper.GetData(selectedData, schema.ToArray());
		}
		#endregion
		#region CustomSqlQuery
		SelectedDataEx Execute(CustomSqlQuery query, IEnumerable<IParameter> parameters, CancellationToken cancellationToken) {
			IDataStoreEx dataStoreEx = (IDataStoreEx)this.connection.CommandChannel;
			IList<IParameter> parametersList = parameters as IList<IParameter> ?? parameters.ToList();
			return dataStoreEx.SelectData(cancellationToken, new Query(query.Sql, new QueryParameterCollection(parametersList.Select(p => new OperandValue(p.Value)).ToArray()), parametersList.Select(p => p.Name).ToArray()), null);
		}
		#endregion
		#region StoredProcQuery
		SelectedDataEx Execute(StoredProcQuery query, IEnumerable<IParameter> parameters, CancellationToken cancellationToken) {
			return ((IDataStoreEx)this.connection.CommandChannel).ExecuteStoredProcedure(cancellationToken,
				query.StoredProcName, parameters.Select(p => new OperandValue(p.Value)).ToArray());
		}
		#endregion
	}
}
