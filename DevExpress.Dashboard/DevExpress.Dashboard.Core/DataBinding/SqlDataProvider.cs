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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using DevExpress.DashboardCommon.DB;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.DataAccess;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.PivotGrid.ServerMode;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	 [Obsolete("The SqlDataProvider class is obsolete now. Use the DashboardSqlDataSource class instead.")]
	public class SqlDataProvider : SqlDataProviderBase, IPivotQueryExecutor {
		static string GetSubSelectString(IDataStore store, string query) {
			return ServerModeQueryExecutor.GetSubSelectString(store, query);
		}
		const string subSelectAlias = "DashboardSubSelectRe";
		static SqlDataProvider() {
			CriteriaOperator.RegisterCustomFunctions(new ICustomFunctionOperator[] {
				new DashboardDistinctCountFunction(),
				new FunctionOperatorAddFunction(FunctionOperatorTypeAdd.GetQuarter),
				new FunctionOperatorAddFunction(FunctionOperatorTypeAdd.GetDateMonthYear),
				new FunctionOperatorAddFunction(FunctionOperatorTypeAdd.GetDateQuarterYear),
				new FunctionOperatorAddFunction(FunctionOperatorTypeAdd.GetDateHour),
				new FunctionOperatorAddFunction(FunctionOperatorTypeAdd.GetDateHourMinute),
				new FunctionOperatorAddFunction(FunctionOperatorTypeAdd.GetDateHourMinuteSecond),
				new FunctionOperatorAddFunction(FunctionOperatorTypeAdd.GetWeekOfYear)
			});
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
		static object ReformatReadValue(object value, Type targetType) {
			if(value == null || value.GetType() == targetType || targetType == typeof(object))
				return value;
			if(targetType == typeof(Guid)) {
				byte[] val = value as byte[];
				return val == null ? new Guid(value.ToString()) : new Guid(val);
			} else
				if(targetType == typeof(TimeSpan)) {
					double seconds = Convert.ToDouble(value);
					if(seconds > TimeSpan.MaxValue.TotalSeconds - 0.0005 && seconds < TimeSpan.MaxValue.TotalSeconds + 0.0005)
						return TimeSpan.MaxValue;
					if(seconds < TimeSpan.MinValue.TotalSeconds + 0.0005 && seconds > TimeSpan.MinValue.TotalSeconds - 0.0005)
						return TimeSpan.MinValue;
					return TimeSpan.FromSeconds(seconds);
				} else
					if(targetType == typeof(char)) {
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
		internal bool IsServerModeSupported { get { return string.IsNullOrEmpty(StoredProcedureName) && !invalidForServerModeCustomSQL.Contains(SqlQuery) && IsSqlDataProvider; } }
		internal ServerModeDataSource PivotServerModeDataSource { get { return IsServerModeSupported ? pivotServerModeDataSource : null; } }
		ServerModeDataSource pivotServerModeDataSource;
		readonly List<string> invalidForServerModeCustomSQL = new List<string>();
		CriteriaOperator customFilter;
		internal override object DataSchema {
			get { return base.DataSchema; }
			set {
				pivotServerModeDataSource = new ServerModeDataSource(this, false);
				base.DataSchema = value;
			}
		}
		public SqlDataProvider(string connectionName, DataConnectionParametersBase connectionParameters, string sqlQuery)
			: base(new DataConnection(connectionName, connectionParameters), sqlQuery) {
			pivotServerModeDataSource = new ServerModeDataSource(this, false);
		}
		public SqlDataProvider(DataConnection dataConnection, string sqlQuery)
			: base(dataConnection, sqlQuery) {
			pivotServerModeDataSource = new ServerModeDataSource(this, false);
		}
		public SqlDataProvider(DataConnection dataConnection)
			: base(dataConnection) {
			pivotServerModeDataSource = new ServerModeDataSource(this, false);
		}
		public SqlDataProvider() {
			pivotServerModeDataSource = new ServerModeDataSource(this, false);
		}
#if DEBUGTEST
		internal SqlDataProvider(object data, object dataPreview)
			: base(data, dataPreview) {
				pivotServerModeDataSource = new ServerModeDataSource(this, false);
		}
#endif
		internal void ExecuteSchemaWithoutData(List<AliasChangeInfo> aliasChangesList) {
			if(IsCustomSql)
				GetServerCustomSqlSchema();
			else {
				DataSchema = CreateDataSchema();
				RaiseTableOrColumnAliasChanged(aliasChangesList);
			}
		}
		void GetServerCustomSqlSchema() {
			if(DataConnection != null && DataConnection.CommandChannel != null) {
				SubSelectStatement subSelect = new SubSelectStatement(GetSubSelectString(DataConnection.DataStore, SqlQuery));
				subSelect.TopSelectedRecords = 1;
				subSelect.Operands.Add(new QuerySubQueryContainer());
				subSelect.Condition = new BinaryOperator(new ConstantValue(0), new ConstantValue(1), BinaryOperatorType.Equal);
				SelectedData data = ExecuteSelect(CreateAutoSql(subSelect).Replace("(*) as F0", " * "));
				DataSchema = CreateSqlView(data);
			}
		}
		protected override object CreateDataSchema() {
			if((IsCustomSql && IsSqlDataProvider) || DataSelection.IsEmpty)
				return null;
			DataSchema dataSchema = new DataSchema();
			foreach(DataTable dataTable in DataSelection)
				if(dataTable.SelectedColumns.Count() > 0)
					PrepareDataSchema(dataTable, dataSchema);
			return dataSchema;
		}
		void PrepareDataSchema(DataTable dataTable, DataSchema dataSchema) {
			SchemaTableTypeDescriptor schemaTable = dataSchema.AddTable(dataTable.UniqueName, dataTable.Alias);
			foreach(DataColumn dataColumn in dataTable.SelectedColumns) {
				DBColumn column = dataColumn.Column;
				if(column != null)
					schemaTable.AddColumn(column.Name, DataTable.GetColumnType(column), dataColumn.ActualName, IncludeTableNamesInSchema);
			}
		}
		protected override void LoadDataObjects(List<DataLoaderError> errors) {
			try {
				LoadDataObjectsCore();
			}
			catch(InvalidOperationException ex) {
				if(errors != null)
					errors.Add(new DataLoaderError(DataLoaderErrorType.Query, ex.Message));
				else
					throw;
			}
		}
		protected override void SchemaChanged() {
			return;
		}
		protected override CriteriaOperator RaiseCustomFilter(string tableName) {
			customFilter = base.RaiseCustomFilter(tableName);
			return customFilter;
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
					DataConnection != null && DataConnection.IsConnected &&
					IsCustomSql ?
						DataSchema is DataView :
						DataSelection != null && !DataSelection.IsEmpty &&
						DBSchema != null && (Tables != null && Tables.Length > 0 || Views != null && Views.Length > 0);
			}
		}
		IEnumerable<ServerModeColumnModel> IPivotQueryExecutor.GetColumns() {
			if(IsCustomSql) {
				DataView view = DataSchema as DataView;
				if(view != null)
					foreach(PropertyDescriptor column in view.ColumnDescriptors)
						yield return new ServerModeColumnModel() {
							Name = column.Name,
							Alias = column.Name,
							TableName = subSelectAlias,
							TableAlias = subSelectAlias,
							DataType = column.PropertyType,
						};
			} else {
				if(DataSelection != null)
					foreach(DataColumn column in DataSelection.SelectedColumns)
						if(column.Column != null && column.DataTable.Table != null)
							yield return new ServerModeColumnModel() {
								Name = column.Column.Name,
								Alias = column.Alias,
								TableName = column.DataTable.Table.Name,
								TableAlias = column.DataTable.Alias,
								DataType = DBColumn.GetType(column.Column.ColumnType),
							};
			}
		}
		IPivotQueryResult IPivotQueryExecutor.GetQueryResult(ServerModeQueryCriteria query, CancellationToken cancellationToken) {
			return GetSqlResult(query, false);
		}
		DataReaderWrapper GetSqlResult(ServerModeQueryCriteria query, bool includeMetadata) {
			try {
				if(IsCustomSql) {
					if(string.IsNullOrEmpty(SqlQuery))
						return null;
					SubSelectStatement selectStatement = new SubSelectStatement(GetSubSelectString(DataConnection.DataStore, SqlQuery));
					SetSelectStatementProperties(query, selectStatement, null);
					return CreateDataReader(selectStatement, GetAggregateTypes(query), true, includeMetadata);
				} else {
					CriteriaOperator outerFilter = null;
					string tableGroupName = DataSelection.GetFilterTableName(DataSelection[0].Alias);
					Dictionary<string, IEnumerable<DataTable>> dataTablesList = DataSelection.GetTableGroups();
					SelectStatement selectStatement = null;
					if(dataTablesList != null && dataTablesList.Keys.Count > 0 && tableGroupName != null && dataTablesList.ContainsKey(tableGroupName)) {
						IEnumerable<DataTable> tables = dataTablesList[tableGroupName];
						if(tableGroupName != null && tables.Count() > 0) {
							DataTable table = tables.First();
							selectStatement = new SelectStatement(table.Table, table.Alias);
							foreach(DataTable dataTable in tables)
								dataTable.PrepareSelectStatement(selectStatement);
						}
						if(!ReferenceEquals(customFilter, null)) {
							outerFilter = customFilter;
						} else {
							FilterInfo filter = DataSelection.Filters[tableGroupName];
							if(filter != null) {
								outerFilter = FilterHelper.GetFilter(filter.FilterString, Parameters);
							}
						}
						SetSelectStatementProperties(query, selectStatement, outerFilter);
						return CreateDataReader(selectStatement, GetAggregateTypes(query), false, includeMetadata);
					} else
						return null;
				}
			}
			finally {
			}
		}
		Type[] GetAggregateTypes(ServerModeQueryCriteria query) {
			if(query.AggregatesStart < 0)
				return null;
			return query.Operands.Skip(query.AggregatesStart).Select((a) => ColumnTypeResolver.ResolveType(a, this.DataConnection.DataStore as ConnectionProviderSql)).ToArray();
		}
		Type IPivotQueryExecutor.ValidateCriteria(CriteriaOperator criteria, bool? exactType) {
			if(exactType == false)
				return ColumnTypeResolver.ResolveType(criteria, this.DataConnection.DataStore as ConnectionProviderSql);
			ServerModeQueryCriteria query = new ServerModeQueryCriteria();
			query.Filter = new BinaryOperator(new ConstantValue(0), new ConstantValue(1), BinaryOperatorType.Equal);
			query.Operands.Add(criteria);
			DataReaderWrapper result = GetSqlResult(query, exactType == true);
			return exactType == true ? result.Metadata[0].DataType : result.HasData ? typeof(object) : null;
		}
		void SetSelectStatementProperties(ServerModeQueryCriteria query, SelectStatement selectStatement, CriteriaOperator outerFilter) {
			selectStatement.Condition = CriteriaOperator.And(query.Filter, outerFilter);
			selectStatement.GroupProperties.AddRange(query.Grouping);
			selectStatement.Operands.AddRange(query.Operands);
			if(query.Sort.Count > 0) {
				ISupportOrderByExpressionAlias supportOrderByExpressionAlias = DataConnection.DataStore as ISupportOrderByExpressionAlias;
				if(supportOrderByExpressionAlias != null) {
					int i = 0;
					foreach(SortingColumn sort in query.Sort) {
						QuerySubQueryContainer aggregate = sort.Property as QuerySubQueryContainer;
						if(!ReferenceEquals(aggregate, null)) {
							uint ihash = (uint)selectStatement.Table.Name.GetHashCode();
							string shash = ihash.ToString("X8", CultureInfo.InvariantCulture);
							string alias = "F" + i++ + shash;
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
			selectStatement.TopSelectedRecords = query.TopSelectedRecords;
			selectStatement.SkipSelectedRecords = query.SkipSelectedRecords;
		}
		DataReaderWrapper CreateDataReader(SelectStatement selectStatement, Type[] aggragateTypes, bool customSql, bool includeMetadata) {
			return new DataReaderWrapper(
				customSql || includeMetadata ?
					SelectDataCommandChannel(selectStatement, customSql, includeMetadata) :
					DataConnection.DataStore.SelectData(selectStatement),
				 aggragateTypes,
				includeMetadata,
				customSql || includeMetadata ? selectStatement.TopSelectedRecords : 0
			);
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
				ReformatValues(data, selectStatement.Operands.Select((criteria) => ColumnTypeResolver.ResolveType(criteria, DataConnection.DataStore as ConnectionProviderSql)).ToArray());
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
		class DataReaderWrapper : IEnumerable<object[]>, IEnumerator<object[]>, IPivotQueryResult {
			readonly Type[] aggregatesSchema;
			readonly SelectStatementResultRow[] data;
			object[] values = null;
			int counter = -1;
			readonly int count;
			readonly ServerModeColumnModelBase[] metadata;
			public bool HasData { get { return data != null; } }
			public DataReaderWrapper(SelectedData data, Type[] aggregatesSchema, bool metadataIncluded, int maxCount) {
				SelectStatementResult dataResult = metadataIncluded ? data.ResultSet[1] : data.ResultSet[0];
				this.data = dataResult.Rows;
				this.aggregatesSchema = aggregatesSchema; 
				this.count = maxCount > 0 && maxCount < this.data.Length ? maxCount : this.data.Length;
				this.metadata = metadataIncluded ? BuildMetadata(data.ResultSet[0]) : null;
			}
			public ServerModeColumnModelBase[] Metadata { get { return metadata; } }
			ServerModeColumnModelBase[] BuildMetadata(SelectStatementResult metadataResult) {
				ServerModeColumnModelBase[] result = new ServerModeColumnModelBase[metadataResult.Rows.Length];
				int i = 0;
				foreach(SelectStatementResultRow schemaRow in metadataResult.Rows) {
					string name = (string)schemaRow.Values[0];
					DBColumnType type = (DBColumnType)Enum.Parse(typeof(DBColumnType), (string)schemaRow.Values[2]);
					result[i++] = new ServerModeColumnModelBase() { Name = name, DataType = DBColumn.GetType(type) };
				}
				return result;
			}
			IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator() {
				return this;
			}
			IEnumerator IEnumerable.GetEnumerator() {
				return this;
			}
			object[] IEnumerator<object[]>.Current {
				get { return values; }
			}
			void IDisposable.Dispose() {
			}
			object IEnumerator.Current {
				get { return values; }
			}
			bool IEnumerator.MoveNext() {
				bool result = ++counter < count;
				if(result)
					values = data[counter].Values;
				return result;
			}
			void IEnumerator.Reset() {
				counter = -1;
			}
			Type[] IPivotQueryResult.AggregatesSchema {
				get { return aggregatesSchema; }
			}
			IEnumerable<object[]> IPivotQueryResult.Data {
				get { return this; }
			}
		}
		class ColumnTypeResolver: CriteriaTypeResolverBase, IQueryCriteriaVisitor<CriteriaTypeResolverResult> {
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
		#endregion
	}
}
