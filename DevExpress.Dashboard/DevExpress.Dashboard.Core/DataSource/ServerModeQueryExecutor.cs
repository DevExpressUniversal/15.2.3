#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.DataAccess.Sql;
using DevExpress.PivotGrid.ServerMode;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon.Native {
	public class OperandValueList : List<KeyValuePair<string, OperandValue>> {
	}
	public class ServerModeQueryExecutor : IPivotQueryExecutor {
		const string subSelectAlias = "DashboardSubSelectRe";
		public static string GetSubSelectString(IDataStore store, string query) {
			IAliasFormatter formatter = store as IAliasFormatter;
			string aliasLead, aliasEnd;
			if(formatter != null) {
				aliasLead = formatter.AliasLead;
				aliasEnd = formatter.AliasEnd;
			}
			else
				aliasLead = aliasEnd = "\"";
			return string.Format(" ({0}{1}) {2} ", query, Environment.NewLine, ConnectionProviderHelper.EncloseAlias(subSelectAlias, aliasLead, aliasEnd));
		}
		static Type[] GetOperandsTypes(SelectStatement statement, IDataStore dataStore) {
			return statement.Operands.Select(
				criteria => {
					Type targetType = ColumnTypeResolver.ResolveType(criteria, dataStore as ConnectionProviderSql);
					return Nullable.GetUnderlyingType(targetType) ?? targetType;
				}
			).ToArray();
		}
		static SelectedData AddConstantValuesToData(SelectedData data, Query query, bool includeMetadata, int targetsCount) {
			if(query.ConstantValues == null || query.OperandIndexes == null || query.ConstantValues.Count == 0)
				return data;
			SelectStatementResult queryResult = includeMetadata ? data.ResultSet[1] : data.ResultSet[0];
			int rowsCount = query.TopSelectedRecords > 0 && query.TopSelectedRecords < queryResult.Rows.Length ? query.TopSelectedRecords : queryResult.Rows.Length;
			SelectStatementResultRow[] rows = new SelectStatementResultRow[rowsCount];
			int[] operandIndexArray = new int[targetsCount];
			for(int i = 0; i < targetsCount; i++) {
				operandIndexArray[i] = -1;
			}
			foreach(KeyValuePair<int, int> pair in query.OperandIndexes) {
				operandIndexArray[pair.Key] = pair.Value;
			}
			object[] constantValuesArray = new object[targetsCount];
			foreach(KeyValuePair<int, OperandValue> pair in query.ConstantValues) {
				constantValuesArray[pair.Key] = pair.Value.Value;
			}
			for(int ri = 0; ri < rows.Length; ri++) {
				object[] values = new object[targetsCount];
				for(int i = 0; i < targetsCount; i++) {
					int j = operandIndexArray[i];
					values[i] = j != -1 ? queryResult.Rows[ri].Values[j] : constantValuesArray[i];
				}
				rows[ri] = new SelectStatementResultRow(values);
			}
			if(includeMetadata) {
				SelectStatementResult metadataResult = data.ResultSet[0];
				SelectStatementResultRow[] metadataRows = new SelectStatementResultRow[targetsCount];
				for(int i = 0; i < targetsCount; i++) {
					int j = operandIndexArray[i];
					if(j != -1) {
						metadataRows[i] = metadataResult.Rows[j];
					}
					else {
						object[] typeValues = new object[3];
						typeValues[2] = DBColumn.GetColumnType(constantValuesArray[i].GetType(), true).ToString();
						metadataRows[i] = new SelectStatementResultRow(typeValues);
					}
				}
				return new SelectedData(new SelectStatementResult(metadataRows), new SelectStatementResult(rows));
			}
			else
				return new SelectedData(new SelectStatementResult(rows));
		}
		static void ReformatValues(SelectedData data, Type[] types) {
			int count = types.Length;
			foreach(SelectStatementResultRow row in data.ResultSet[0].Rows)
				for(int i = 0; i < count; i++)
					row.Values[i] = ReformatReadValue(row.Values[i], types[i]);
		}
		static bool IsInteger(Type type) {
			TypeCode typeCode = Type.GetTypeCode(type);
			switch(typeCode) {
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.SByte:
				case TypeCode.Byte:
					return true;
			}
			return false;
		}
		static void ReformatValues(SelectedDataEx data, Type[] types) {
			for(int i = 0; i < data.Lists.Length; i++) {
				Type targetType = types[i];
				if(!IsInteger(targetType)) 
					continue;
				data.Schema[i].Type = targetType;
				IList columnValues = data.Lists[i];
				object[] reformattedColumnValues = new object[columnValues.Count];
				for(int j = 0; j < columnValues.Count; j++)
					reformattedColumnValues[j] = ReformatReadValue(columnValues[j], targetType);
				data.Lists[i] = reformattedColumnValues;
			}
		}
		static object ReformatReadValue(object value, Type targetType) {
			if(value == null || value.GetType() == targetType || targetType == typeof(object))
				return value;
			if(targetType == typeof(Guid)) {
				byte[] val = value as byte[];
				return val == null ? new Guid(value.ToString()) : new Guid(val);
			}
			else if(targetType == typeof(TimeSpan)) {
				double seconds = Convert.ToDouble(value);
				if(seconds > TimeSpan.MaxValue.TotalSeconds - 0.0005 && seconds < TimeSpan.MaxValue.TotalSeconds + 0.0005)
					return TimeSpan.MaxValue;
				if(seconds < TimeSpan.MinValue.TotalSeconds + 0.0005 && seconds > TimeSpan.MinValue.TotalSeconds - 0.0005)
					return TimeSpan.MinValue;
				return TimeSpan.FromSeconds(seconds);
			}
			else if(targetType == typeof(char)) {
				string charStr = value as string;
				if(charStr != null) {
					if(charStr.Length == 0)
						return ' ';
					else
						return Convert.ToChar(charStr);
				}
			}
			return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
		}
		public static SelectStatement GetSubSelect(IDataStore dataStore, string sql) {
			SubSelectStatement subSelect = new SubSelectStatement(GetSubSelectString(dataStore, sql));
			subSelect.TopSelectedRecords = 1;
			subSelect.Operands.Add(new QuerySubQueryContainer());
			subSelect.Condition = new BinaryOperator(new ConstantValue(0), new ConstantValue(1), BinaryOperatorType.Equal);
			return subSelect;
		}
		DBSchema dbSchema;		
		ServerModeDataSource pivotServerModeDataSource;
		public ResultTable ResultTable { get; set; }
		public IEnumerable<IParameter> Parameters { get; set; }
		internal ServerModeDataSource PivotServerModeDataSource { get { return pivotServerModeDataSource; } }
		public CriteriaOperator CustomFilter { get; set; }
		SqlDataConnection DataConnection { get; set; }
		public SqlQuery Query { get; set; }
		TableQuery TableQuery {
			get {
				if(Query == null)
					return null;
				return Query.GetTableQueryForServerMode();
			}
		}
		CustomSqlQuery CustomSqlQuery {
			get {
				if(Query == null || DataConnection == null)
					return null;
				return Query.GetCustomSqlQueryForServerMode(DBSchema, DataConnection.SqlGeneratorFormatter);
			}
		}
		DBSchema DBSchema {
			get {
				return dbSchema;
			}
			set {
				dbSchema = value;				
			}
		}
		public ServerModeQueryExecutor(SqlDataConnection dataConnection, DBSchema dbSchema) {
			this.DataConnection = dataConnection;
			this.DBSchema = dbSchema;
			pivotServerModeDataSource = new ServerModeDataSource(this, false);
		}
		OperandValueList GetCustomSqlCommandParameters() {
			OperandValueList result = new OperandValueList();
			ICommandParameterNameProvider commandParameterNameProvider = DataConnection.DataStore as ICommandParameterNameProvider;
			foreach(IParameter parameter in Parameters) {
				if(commandParameterNameProvider != null)
					foreach(string name in commandParameterNameProvider.GetCommandParameterNames(parameter.Name)) {
						result.Add(new KeyValuePair<string, OperandValue>(name, new OperandValue(parameter.Value)));
					}
				else {
					result.Add(new KeyValuePair<string, OperandValue>(parameter.Name, new OperandValue(parameter.Value)));
				}
			}
			return result;
		}
		#region IPivotQueryExecutor
		CriteriaSyntax IPivotQueryExecutor.CriteriaSyntax {
			get {
				return CriteriaSyntax.ServerCriteria | CriteriaSyntax.ForceInt16SummaryAsInt32;
			}
		}
		List<object> IPivotQueryExecutor.GetQueryResult(NestedSummaryServerModeQueryCriteria query) {
			return null;
		}
		bool IPivotQueryExecutor.Connected {
			get {
				return
					DataConnection != null && DataConnection.IsConnected && CustomSqlQuery != null ? ResultTable != null :
					TableQuery != null && (DBSchema != null && DBSchema.Tables != null && DBSchema.Tables.Length > 0 || DBSchema.Views != null && DBSchema.Views.Length > 0);
			}
		}
		IEnumerable<ServerModeColumnModel> IPivotQueryExecutor.GetColumns() {
			if(CustomSqlQuery != null && ResultTable != null) {
				return ResultTable.Columns.Select(
					column => new ServerModeColumnModel() {
						Name = column.Name,
						Alias = column.Name,
						TableName = subSelectAlias,
						TableAlias = subSelectAlias,
						DataType = column.PropertyType,
					}
				);
			}
			else if(TableQuery != null) {
				Dictionary<string, DBTable> dbTables = DBSchema.Tables.Union(DBSchema.Views).ToDictionary(t => t.Name);
				return TableQuery.Tables.SelectMany(
						t => t.SelectedColumns.Select(
							c =>
								new ServerModeColumnModel() {
									Name = c.Name,
									Alias = c.Alias,
									TableName = t.Name,
									TableAlias = t.ActualName,
									DataType = DBColumn.GetType(dbTables[t.Name].GetColumn(c.Name).ColumnType)
								}
						)
					);
			}
			return Enumerable.Empty<ServerModeColumnModel>();
		}
		IPivotQueryResult IPivotQueryExecutor.GetQueryResult(ServerModeQueryCriteria query, CancellationToken cancellationToken) {
			return GetSqlResult(query, false, CancellationToken.None);
		}
		DataReaderWrapperBase GetSqlResult(ServerModeQueryCriteria query, bool includeMetadata, CancellationToken cancellationToken) {
			CustomSqlQuery customSqlQuery = CustomSqlQuery;
			TableQuery tableQuery = TableQuery;
			if(customSqlQuery != null) {
				if(string.IsNullOrEmpty(customSqlQuery.Sql))
					return null;
				SubSelectStatement selectStatement = new SubSelectStatement(GetSubSelectString(DataConnection.DataStore, customSqlQuery.Sql));
				SetSelectStatementProperties(query, selectStatement, null);
				return CreateDataReader(selectStatement, query, includeMetadata, cancellationToken);
			}
			else if(tableQuery != null) {
				try {
					return CreateDataReaderTableQuery(tableQuery, query, includeMetadata, cancellationToken);
				}
				catch(InvalidOperationException) {
				}
			}
			return null;
		}
		Type IPivotQueryExecutor.ValidateCriteria(CriteriaOperator criteria, bool? exactType) {
			if(exactType == false)
				return ColumnTypeResolver.ResolveType(criteria, this.DataConnection.DataStore as ConnectionProviderSql);
			ServerModeQueryCriteria query = new ServerModeQueryCriteria();
			query.Filter = new BinaryOperator(new ConstantValue(0), new ConstantValue(1), BinaryOperatorType.Equal);
			query.Operands.Add(criteria);
			DataReaderWrapperBase result = GetSqlResult(query, exactType == true, CancellationToken.None);
			return exactType == true ? result.Metadata[0] : result.HasData ? typeof(object) : null;
		}
		void SetSelectStatementProperties(ServerModeQueryCriteria query, SelectStatement selectStatement, CriteriaOperator outerFilter) {
			selectStatement.Condition = CriteriaOperator.And(query.Filter, outerFilter);
			UniqueAliasGenerator aliasGenerator = new UniqueAliasGenerator();
			selectStatement.Operands.AddRange(query.Operands);
			SetSelectStatementSorting(query, selectStatement, aliasGenerator);
			SetSelectStatementGroupping(query, selectStatement, aliasGenerator);
			selectStatement.TopSelectedRecords = query.TopSelectedRecords;
			selectStatement.SkipSelectedRecords = query.SkipSelectedRecords;
		}
		#endregion
		void SetSelectStatementSorting(ServerModeQueryCriteria query, SelectStatement selectStatement, UniqueAliasGenerator aliasGenerator) {
			if(query.Sort.Count <= 0)
				return;
			ISupportOrderByExpressionAlias supportOrderByExpressionAlias = DataConnection.DataStore as ISupportOrderByExpressionAlias;
			if(supportOrderByExpressionAlias != null) {
				foreach(SortingColumn sort in query.Sort) {
					QuerySubQueryContainer aggregate = sort.Property as QuerySubQueryContainer;
					if(!ReferenceEquals(aggregate, null)) {
						string alias = aliasGenerator.Generate(selectStatement.Table.Name);
						AliasedQuerySubQueryContainer aliasedAggregate = new AliasedQuerySubQueryContainer(aggregate, alias);
						selectStatement.SortProperties.Add(new SortingColumn(new QueryOperand(aliasedAggregate.Alias, null), sort.Direction));
						selectStatement.Operands.Add(aliasedAggregate);
					}
					else {
						selectStatement.SortProperties.Add(sort);
					}
				}
			}
			else {
				selectStatement.SortProperties.AddRange(query.Sort);
			}
		}
		void SetSelectStatementGroupping(ServerModeQueryCriteria query, SelectStatement selectStatement, UniqueAliasGenerator aliasGenerator) {
			if(query.Grouping.Count <= 0)
				return;
			ISupportGroupByExpressionAlias supportGroupByExpressionAlias = DataConnection.DataStore as ISupportGroupByExpressionAlias;
			if(supportGroupByExpressionAlias != null) {
				foreach(CriteriaOperator group in query.Grouping) {
					CriteriaOperator aggregate = group as FunctionOperator ?? (CriteriaOperator)(group as BinaryOperator);
					if(!ReferenceEquals(aggregate, null)) {
						string alias = aliasGenerator.Generate(selectStatement.Table.Name);
						AliasedQuerySubQueryContainer aliasedAggregate = new AliasedQuerySubQueryContainer(new QuerySubQueryContainer { AggregateProperty = aggregate }, alias);
						selectStatement.GroupProperties.Add(new QueryOperand(aliasedAggregate.Alias, null));
						selectStatement.Operands.Remove(aggregate);
						selectStatement.Operands.Add(aliasedAggregate);
					}
					else {
						selectStatement.GroupProperties.Add(group);
					}
				}
			}
			else {
				selectStatement.GroupProperties.AddRange(query.Grouping);
			}
		}
		DataReaderWrapperBase CreateDataReaderTableQuery(TableQuery tableQuery, ServerModeQueryCriteria query, bool includeMetadata, CancellationToken cancellationToken) {
			SelectStatement selectStatement = tableQuery.BuildSelectStatement(DBSchema, DataConnection.SqlGeneratorFormatter, Parameters);
			selectStatement.Operands.Clear();
			CriteriaOperator outerFilter;
			if(!ReferenceEquals(CustomFilter, null)) {
				OperandPropertyReplacingCriteriaOperatorPatcher patcher = new OperandPropertyReplacingCriteriaOperatorPatcher(Parameters);
				outerFilter = patcher.Process(CustomFilter);
			}
			else
				outerFilter = FilterHelper.GetFilter(tableQuery.FilterString, Parameters);
			SetSelectStatementProperties(query, selectStatement, outerFilter);
			IDataStoreEx dataStoreEx = DataConnection.DataStore as IDataStoreEx;
			if(dataStoreEx != null) {
				Query query1 = new SelectSqlGenerator(DataConnection.SqlGeneratorFormatter).GenerateSql(selectStatement);
				if(includeMetadata)
					return new DataReaderWrapper(SelectDataCommandChannel(selectStatement, false, includeMetadata), true, GetAggregateTypes(query), selectStatement.TopSelectedRecords);
				else {
					SelectedDataEx data = dataStoreEx.SelectData(cancellationToken, query1, null);
					ReformatValues(data, GetOperandsTypes(selectStatement, DataConnection.DataStore));
					return new DataReaderWrapperEx(data, query.AggregatesStart, 0);
				}
			}
			SelectedData selectedData;
			if(includeMetadata)
				selectedData = SelectDataCommandChannel(selectStatement, false, includeMetadata);
			else {
				selectedData = DataConnection.DataStore.SelectData(selectStatement);
				ReformatValues(selectedData, GetOperandsTypes(selectStatement, DataConnection.DataStore));
			}
			return new DataReaderWrapper(
				selectedData,
				includeMetadata, GetAggregateTypes(query),
				includeMetadata ? selectStatement.TopSelectedRecords : 0
			);
		}
		Type[] GetAggregateTypes(ServerModeQueryCriteria query) {
			if(query.AggregatesStart < 0)
				return null;
			return query.Operands.Skip(query.AggregatesStart).Select((a) => ColumnTypeResolver.ResolveType(a, this.DataConnection.DataStore as ConnectionProviderSql)).ToArray();
		}
		DataReaderWrapperBase CreateDataReader(SelectStatement selectStatement, ServerModeQueryCriteria query, bool includeMetadata, CancellationToken cancellationToken) {
			IDataStoreEx dataStoreEx = DataConnection.DataStore as IDataStoreEx;
			if(dataStoreEx != null) {
				Query query1 = new SelectSqlGenerator(DataConnection.SqlGeneratorFormatter).GenerateSql(selectStatement);
				List<ColumnInfoEx> schema = new List<ColumnInfoEx>();
				if(includeMetadata)
					return new DataReaderWrapper(SelectDataCommandChannel(selectStatement, true, true), true, GetAggregateTypes(query), selectStatement.TopSelectedRecords);
				else {
					IList<IParameter> parametersList = Parameters as IList<IParameter> ?? Parameters.ToList();
					SelectedDataEx data = dataStoreEx.SelectData(cancellationToken, new Query(query1.Sql, new QueryParameterCollection(parametersList.Select(p => new OperandValue(p.Value)).ToArray()), parametersList.Select(p => p.Name).ToArray()), null);
					ReformatValues(data, GetOperandsTypes(selectStatement, DataConnection.DataStore));
					return new DataReaderWrapperEx(data, query.AggregatesStart, selectStatement.TopSelectedRecords);
				}
			}
			return new DataReaderWrapper(SelectDataCommandChannel(selectStatement, true, includeMetadata),
				includeMetadata, GetAggregateTypes(query), selectStatement.TopSelectedRecords);
		}
		SelectedData SelectDataCommandChannel(SelectStatement selectStatement, bool customSql, bool includeMetadata) {
			Query query = new SelectSqlGenerator(DataConnection.SqlGeneratorFormatter).GenerateSql(selectStatement);
			List<KeyValuePair<string, OperandValue>> parameters = GetUnitedParameters(query.Parameters, query.ParametersNames, customSql);
			SelectedData data = SelectDataCommandChannel(
				query.Sql,
				new QueryParameterCollection(parameters.Select(p => p.Value).ToArray()),
				parameters.Select(p => p.Key).ToArray(),
				includeMetadata
			);
			data = AddConstantValuesToData(data, query, includeMetadata, selectStatement.Operands.Count);
			if(!includeMetadata)
				ReformatValues(data, GetOperandsTypes(selectStatement, DataConnection.DataStore));
			return data;
		}
		List<KeyValuePair<string, OperandValue>> GetUnitedParameters(QueryParameterCollection queryParameterValues, IList queryParametersNames, bool customSql) {
			List<KeyValuePair<string, OperandValue>> customSqlParameters = GetCustomSqlCommandParameters();
			List<KeyValuePair<string, OperandValue>> queryParameters = new List<KeyValuePair<string, OperandValue>>();
			for(int i = 0; i < queryParameterValues.Count; i++)
				queryParameters.Add(new KeyValuePair<string, OperandValue>(queryParametersNames[i].ToString(), queryParameterValues[i]));
			if(!customSql || customSqlParameters.Count == 0)
				return queryParameters;
			if(queryParameters.Count == 0)
				return customSqlParameters;
			ConnectionProviderSql supportNamedParameters = DataConnection.DataStore as ConnectionProviderSql;
			if(supportNamedParameters != null && !supportNamedParameters.SupportNamedParameters)
				return queryParameters;
			Dictionary<string, OperandValue> pars = new Dictionary<string, OperandValue>();
			foreach(KeyValuePair<string, OperandValue> queryParameter in queryParameters)
				pars[queryParameter.Key] = queryParameter.Value;
			foreach(IParameter parameter in Parameters) {
				OperandParameter op = new OperandParameter(parameter.Name, parameter.Value);
				bool createParameter = true;
				string generatedParameterName = DataConnection.SqlGeneratorFormatter.GetParameterName(op, -1, ref createParameter);
				if(createParameter) {
					OperandValue value;
					if(pars.TryGetValue(generatedParameterName, out value)) {
						if(value.Value.Equals(parameter.Value))
							pars.Remove(generatedParameterName);
						else
							throw new ArgumentException();
					}
				}
			}
			return pars.Concat(customSqlParameters).ToList();
		}
		SelectedData SelectDataCommandChannel(string sql, QueryParameterCollection parameters, string[] parametersNames, bool includeMetadata) {
			SelectedData result;
			if(parameters.Count > 0)
				result = includeMetadata ?
					CommandChannelHelper.ExecuteQueryWithMetadataWithParams(DataConnection.CommandChannel, sql, parameters, parametersNames) :
					CommandChannelHelper.ExecuteQueryWithParams(DataConnection.CommandChannel, sql, parameters, parametersNames);
			else
				result = includeMetadata ?
					CommandChannelHelper.ExecuteQueryWithMetadata(DataConnection.CommandChannel, sql) :
					CommandChannelHelper.ExecuteQuery(DataConnection.CommandChannel, sql);
			return result;
		}
		class UniqueAliasGenerator {
			int index = 0;
			public string Generate(string tableName) {
				uint ihash = (uint)tableName.GetHashCode();
				string shash = ihash.ToString("X8", CultureInfo.InvariantCulture);
				return "F" + index++ + shash;
			}
		}
		class ColumnTypeResolver : CriteriaTypeResolverBase, IQueryCriteriaVisitor<CriteriaTypeResolverResult> {
			static readonly ColumnTypeResolver Instance = new ColumnTypeResolver();
			ColumnTypeResolver() { }
			CriteriaTypeResolverResult IQueryCriteriaVisitor<CriteriaTypeResolverResult>.Visit(QueryOperand theOperand) {
				return new CriteriaTypeResolverResult(DBColumn.GetType(theOperand.ColumnType));
			}
			CriteriaTypeResolverResult IQueryCriteriaVisitor<CriteriaTypeResolverResult>.Visit(QuerySubQueryContainer subQueryContainer) {
				switch(subQueryContainer.AggregateType) {
					case Aggregate.Exists:
						return new CriteriaTypeResolverResult(typeof(bool));
					case Aggregate.Count:
						return new CriteriaTypeResolverResult(typeof(int));
					default:
						return Process(subQueryContainer.AggregateProperty);
				}
			}
			ConnectionProviderSql provider;
			Type ResolveTypeInternal(CriteriaOperator criteria, ConnectionProviderSql provider) {
				this.provider = provider;
				return Process(criteria).Type;
			}
			static public Type ResolveType(CriteriaOperator criteria, ConnectionProviderSql provider) {
				return Instance.ResolveTypeInternal(criteria, provider);
			}
			protected override Type GetCustomFunctionType(string functionName, params Type[] operands) {
				ICustomFunctionOperator customFunction = null;
				if(provider != null) {
					customFunction = provider.GetCustomFunctionOperator(functionName);
				}
				if(customFunction == null) {
					return base.GetCustomFunctionType(functionName, operands);
				}
				return customFunction.ResultType(operands);
			}
		}
	}
	class AliasedQuerySubQueryContainer : QuerySubQueryContainer {
		string alias;
		internal string Alias { get { return alias; } set { alias = value; } }
		internal AliasedQuerySubQueryContainer(QuerySubQueryContainer criteria, string alias)
			: base(criteria.Node, criteria.AggregateProperty, criteria.AggregateType) {
			this.alias = alias;
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			var queryVisitor = (IQueryCriteriaVisitor<T>)visitor;
			var baseVisitResult = queryVisitor.Visit(this);
			if(!(visitor is BaseSqlGenerator) || string.IsNullOrEmpty(Alias))
				return baseVisitResult;
			return (T)(object)string.Format("{0} {1}", (string)(object)baseVisitResult, Alias);
		}
	}
}
