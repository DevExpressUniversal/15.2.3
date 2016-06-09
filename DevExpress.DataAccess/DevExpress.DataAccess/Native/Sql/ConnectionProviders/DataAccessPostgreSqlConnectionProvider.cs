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
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.DataAccess.Native.Sql.ConnectionProviders {
	public class DataAccessPostgreSqlConnectionProviderBase : PostgreSqlConnectionProvider, IAliasFormatter, IDataStoreSchemaExplorerEx, IDataStoreEx {
		#region static
		static DBColumnType GetColumnType(string typeName, int length) {
			switch(typeName) {
				case "int4":
					return DBColumnType.Int32;
				case "bytea":
					return DBColumnType.ByteArray;
				case "bpchar":
					return length <= 1 ? DBColumnType.Char : DBColumnType.String;
				case "varchar":
				case "text":
					return DBColumnType.String;
				case "bool":
					return DBColumnType.Boolean;
				case "int2":
					return DBColumnType.Int16;
				case "int8":
					return DBColumnType.Int64;
				case "numeric":
				case "money":
					return DBColumnType.Decimal;
				case "float8":
					return DBColumnType.Double;
				case "float4":
					return DBColumnType.Single;
				case "date":
				case "timestamp":
					return DBColumnType.DateTime;
				case "uuid":
					return DBColumnType.Guid;
			}
			return DBColumnType.Unknown;
		}
		#endregion
		const string aliasLead = "\"";
		const string aliasEnd = "\"";
		const bool singleQuotedString = true;
		int serverMajorVersion = -1;
		int maxIndexKeys = -1;
		protected IDataStoreEx DataStoreEx { get { return this; } }
		int ServerMajorVersion {
			get {
				if(this.serverMajorVersion >= 0)
					return this.serverMajorVersion;
				try {
					DataStoreEx.ProcessQuery(CancellationToken.None, new Query("show SERVER_VERSION"), (reader,token) => {
						reader.Read();
						string versionString = reader.GetString(0);
						this.serverMajorVersion = int.Parse(versionString.Split('.')[0]);
					});
				} catch(SqlExecutionErrorException) {
					this.serverMajorVersion = 8;
				}
				return this.serverMajorVersion;
			}
		}
		int MaxIndexKeys {
			get {
				if(this.maxIndexKeys < 0) {
					try {
						DataStoreEx.ProcessQuery(CancellationToken.None, new Query("current_setting('max_index_keys')::int"), (reader, cancellationToken)=> {
							reader.Read();
							this.maxIndexKeys = reader.GetInt32(0);
						});
					} catch {
						this.maxIndexKeys = 32;
					}
				}
				return this.maxIndexKeys;
			}
		}
		public DataAccessPostgreSqlConnectionProviderBase(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		protected override string GetSafeNameRoot(string originalName) {
			return originalName;
		}
		public override string ComposeSafeColumnName(string columnName) {
			return columnName;
		}
		public override string FormatColumn(string columnName, string tableAlias) {
			return base.FormatColumn(columnName, ConnectionProviderHelper.EncloseAlias(tableAlias, aliasLead, aliasEnd));
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			if(tableName == SubSelectStatement.CustomSqlString)
				return tableAlias;
			string encloseAlias = ConnectionProviderHelper.EncloseAlias(tableAlias, aliasLead, aliasEnd);
			return string.IsNullOrEmpty(encloseAlias) ? base.FormatTable(schema, tableName) : base.FormatTable(schema, tableName, encloseAlias);
		}
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			bool shouldCreateParameter = createParameter;
			OperandParameter param = parameter as OperandParameter;
			if(!ReferenceEquals(param, null)) {
				createParameter = true;
				return string.Format("@{0}", param.ParameterName);
			}
			object value = parameter.Value;
			if(value != null) {
				createParameter = false;
				switch(Type.GetTypeCode(value.GetType())) {
					case TypeCode.Decimal:
						return ((decimal)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.DateTime:
						return string.Format("'{0}'", ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
				}
			}
			createParameter = shouldCreateParameter;
			return base.GetParameterName(new ConstantValue(parameter.Value), index, ref createParameter);
		}
		#region IAliasFormatter
		string IAliasFormatter.AliasLead {
			get {
				return aliasLead;
			}
		}
		string IAliasFormatter.AliasEnd {
			get {
				return aliasEnd;
			}
		}
		bool IAliasFormatter.SingleQuotedString {
			get {
				return singleQuotedString;
			}
		}
		int IAliasFormatter.MaxTableAliasLength {
			get {
				return GetSafeNameTableMaxLength();
			}
		}
		int IAliasFormatter.MaxColumnAliasLength {
			get {
				return GetSafeNameColumnMaxLength();
			}
		}
		#endregion
		#region IDataStoreSchemaExplorerEx Members
		string[] IDataStoreSchemaExplorerEx.GetStorageDataObjectsNames(DataObjectTypes types) {
			List<string> result = new List<string>();
			string typeCondition = ConnectionProviderHelper.GetDataObjectTypeCondition(types, "TABLE_TYPE = 'BASE TABLE'", "TABLE_TYPE = 'VIEW'");
			string getTablesListQuery = string.Format(@"select CASE WHEN TABLE_SCHEMA <> '{0}' then TABLE_SCHEMA || '.' ELSE '' END || TABLE_NAME
                                                           from INFORMATION_SCHEMA.TABLES
                                                           where ({1}) and TABLE_SCHEMA <> 'information_schema' and TABLE_SCHEMA <> 'pg_catalog'", this.ObjectsOwner, typeCondition);
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesListQuery), (reader, cancellationToken)=> {
				while(reader.Read()) {
					result.Add(reader.GetString(0).TrimEnd());
				}
			});
			return result.ToArray();
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageTables(params string[] tablesList) {
			return ((IDataStoreSchemaExplorerEx)this).GetStorageTables(false, tablesList);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageTables(bool includeColumns, params string[] tablesList) {
			return GetStorageDataObjects(includeColumns, false, tablesList);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageViews(params string[] tablesList) {
			return ((IDataStoreSchemaExplorerEx)this).GetStorageViews(false, tablesList);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageViews(bool includeColumns, params string[] tablesList) {
			return GetStorageDataObjects(includeColumns, true, tablesList);
		}
		void IDataStoreSchemaExplorerEx.GetColumns(params DBTable[] tables) {
			GetStorageTablesColumns(tables, true);
		}
		DBTable[] GetStorageDataObjects(bool includeColumns, bool isView, params string[] tablesList) {
			string[] tableNames = ((IDataStoreSchemaExplorerEx)this).GetStorageDataObjectsNames(isView ? DataObjectTypes.Views : DataObjectTypes.Tables);
			IEnumerable<string> filteredTables = tablesList.Length == 0 ? tableNames : tableNames.Where(n => tablesList.Any(f => f == n));
			DBTable[] tables = filteredTables.Select(p => ConnectionProviderHelper.CreateTable(p, isView)).ToArray();
			if(includeColumns)
				GetStorageTablesColumns(tables, tablesList.Length != 0);
			GetStorageTablesForeignKeys(tables, tablesList.Length != 0);
			return tables;
		}
		static string GetTablesFilter(ICollection<DBTable> tables, string prefix) {
			return tables.Count != 0 ? string.Format("{0} in ({1})", prefix, string.Join(", ", tables.Select(n => string.Format("'{0}'", n.Name.ToLowerInvariant())))) : string.Empty;
		}
		void GetStorageTablesColumns(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.Columns.Clear();
			string getTablesColumnsQuery = string.Format(@"select CASE WHEN n.nspname <> '{0}' then n.nspname || '.' ELSE '' END || c.relname as full_table_name,
                                                                  a.attname, t.typname, a.atttypmod, a.attnum
                                                           from pg_attribute a
                                                            join pg_class c on a.attrelid = c.oid
                                                            join pg_type t on a.atttypid = t.oid
                                                            join pg_namespace n on c.relnamespace = n.oid
                                                           where a.attnum > 0 AND NOT a.attisdropped AND (c.relkind = 'r' OR c.relkind = 'v')
                                                                 AND n.nspname <> 'information_schema' and n.nspname <> 'pg_catalog'", this.ObjectsOwner);
			getTablesColumnsQuery = useTablesFilter
				? string.Format("select * from ({0}) table_columns {1} order by full_table_name, attnum", getTablesColumnsQuery, GetTablesFilter(tables, "where lower(full_table_name)"))
				: string.Format("{0} order by n.nspname, c.relname, a.attnum", getTablesColumnsQuery);
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesColumnsQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string tableName = reader.GetString(0);
					string columnName = reader.GetString(1);
					if(table == null || !string.Equals(table.Name, tableName))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, tableName));
					if(table == null)
						continue;
					string typeName = reader.GetString(2);
					int size = reader.GetInt32(3) - 4;
					DBColumnType type = GetColumnType(typeName, size);
					DBColumn column = ConnectionProviderHelper.CreateColumn(columnName, false, false, type == DBColumnType.String ? size : 0, type);
					table.AddColumn(column);
				}
			});
		}
		void GetStorageTablesForeignKeys(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.ForeignKeys.Clear();
			string getTablesForeignKeysQuery = ServerMajorVersion < 9
				? GetForeingnKeysQueryForOldServerVersion(tables, useTablesFilter)
				: GetForeingnKeysQueryForNewServerVersion(tables, useTablesFilter);
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesForeignKeysQuery), (reader, cancellationToken)=> {
				while(reader.Read()) {
					string foreignTableName = reader.GetString(0);
					DBTable table = tables.FirstOrDefault(t => t.Name == foreignTableName);
					if(table == null)
						continue;
					string primaryTableName = reader.GetString(1);
					string foreignColumnName = reader.GetString(2);
					string primaryColumnName = reader.GetString(3);
					string keyName = reader.GetString(4);
					DBForeignKey fk = table.ForeignKeys.FirstOrDefault(k => k.Name == keyName);
					if(fk == null) {
						fk = new DBForeignKey {
							Name = keyName, PrimaryKeyTable = primaryTableName
						};
						table.ForeignKeys.Add(fk);
					}
					fk.Columns.Add(foreignColumnName);
					fk.PrimaryKeyTableKeyColumns.Add(primaryColumnName);
				}
			});
		}
		string GetForeingnKeysQueryForNewServerVersion(ICollection<DBTable> tables, bool useTablesFilter) {
			string getTablesForeignKeysQuery = string.Format(@"select CASE WHEN n.nspname <> '{0}' then n.nspname || '.' ELSE '' END || tc.relname as full_table_name,
                                                                      CASE WHEN nf.nspname <> '{0}' then nf.nspname || '.' ELSE '' END || tcf.relname as primary_table_name,
                                                                      n.nspname as table_owner, tc.relname as table_name, nf.nspname as foreign_owner, tcf.relname as foreign_name,
                                                                      conkey as table_columns, c.conname as key_name, confkey as primary_columns, generate_subscripts(conkey, 1) as column_index
                                                               from pg_constraint c
                                                                join pg_class tc on c.conrelid = tc.oid
                                                                join pg_namespace n on tc.relnamespace = n.oid
                                                                join pg_class tcf on c.confrelid = tcf.oid
                                                                join pg_namespace nf on tcf.relnamespace = nf.oid
                                                               where c.contype = 'f'", this.ObjectsOwner);
			getTablesForeignKeysQuery = string.Format(@"select full_table_name, primary_table_name, ap.attname as table_column, af.attname as primary_column, key_name, column_index
                                                        from ({0}) k
                                                         join pg_attribute ap on k.table_columns[k.column_index] = ap.attnum
                                                         join pg_class cp on ap.attrelid = cp.oid AND k.table_name = cp.relname
                                                         join pg_namespace np on cp.relnamespace = np.oid AND k.table_owner = np.nspname
                                                         join pg_attribute af on k.primary_columns[k.column_index] = af.attnum
                                                         join pg_class cf on af.attrelid = cf.oid AND k.foreign_name = cf.relname
                                                         join pg_namespace nf on cf.relnamespace = nf.oid AND k.foreign_owner = nf.nspname
                                                         where ap.attnum > 0 AND NOT ap.attisdropped AND (cp.relkind = 'r' OR cp.relkind = 'v')
                                                           AND af.attnum > 0 AND NOT af.attisdropped AND (cf.relkind = 'r' OR cf.relkind = 'v')", getTablesForeignKeysQuery);
			getTablesForeignKeysQuery = useTablesFilter
				? string.Format(@"{0} {1} order by key_name, column_index", getTablesForeignKeysQuery, GetTablesFilter(tables, "and lower(full_table_name)"))
				: string.Format(@"{0} order by key_name, column_index", getTablesForeignKeysQuery);
			return getTablesForeignKeysQuery;
		}
		string GetForeingnKeysQueryForOldServerVersion(ICollection<DBTable> tables, bool useTablesFilter) {
			string getTablesForeignKeysQuery = string.Format(@"select CASE WHEN n.nspname <> '{0}' then n.nspname || '.' ELSE '' END || tc.relname as full_table_name,
                                                                      CASE WHEN nf.nspname <> '{0}' then nf.nspname || '.' ELSE '' END || tcf.relname as primary_table_name,
                                                                      ap.attname as table_column, af.attname as primary_column, c.conname as key_name, pos.n as column_index
                                                               from generate_series(1, {1}, 1) pos(n), pg_constraint c
                                                                join pg_class tc on c.conrelid = tc.oid
                                                                join pg_namespace n on tc.relnamespace = n.oid
                                                                join pg_class tcf on c.confrelid = tcf.oid
                                                                join pg_namespace nf on tcf.relnamespace = nf.oid AND n.nspname = nf.nspname
                                                                join pg_class cp on tc.relname = cp.relname
                                                                join pg_attribute ap on ap.attrelid = cp.oid
                                                                join pg_namespace np on cp.relnamespace = np.oid AND n.nspname = np.nspname
                                                                join pg_class cf on tcf.relname = cf.relname
                                                                join pg_attribute af on af.attrelid = cf.oid
                                                               where c.contype = 'f' and c.conkey[pos.n] = ap.attnum and confkey[pos.n] = af.attnum
                                                                 AND ap.attnum > 0 AND NOT ap.attisdropped AND (cp.relkind = 'r' OR cp.relkind = 'v')
                                                                 AND af.attnum > 0 AND NOT af.attisdropped AND (cf.relkind = 'r' OR cf.relkind = 'v')", this.ObjectsOwner, MaxIndexKeys);
			getTablesForeignKeysQuery = useTablesFilter
				? string.Format(@"select * from ({0}) k {1} order by key_name, column_index", getTablesForeignKeysQuery, GetTablesFilter(tables, "where lower(full_table_name)"))
				: string.Format("{0} order by key_name, pos.n", getTablesForeignKeysQuery);
			return getTablesForeignKeysQuery;
		}
		#endregion
		#region IDataStoreEx
		void IDataStoreEx.ProcessQuery(CancellationToken cancellationToken, Query query, Action<IDataReader, CancellationToken> action) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					using(IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess)) {
						action(reader, cancellationToken);
					}
				}
			}
		}
		void IDataStoreEx.ProcessStoredProc(CancellationToken cancellationToken, string sprocName, Action<IDataReader, CancellationToken> action, params OperandValue[] parameters) {
			throw new NotSupportedException();
		}
		SelectedDataEx IDataStoreEx.SelectData(CancellationToken cancellationToken, Query query, string[] columns) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					return DataStoreExHelper.GetData(command, cancellationToken, columns, NativeSkipTakeSupported, query.SkipSelectedRecords);
				}
			}
		}
		ColumnInfoEx[] IDataStoreEx.SelectSchema(Query query) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					return DataStoreExHelper.GetSchema(command);
				}
			}
		}
		SelectedDataEx IDataStoreEx.ExecuteStoredProcedure(CancellationToken cancellationToken, string sprocName, params OperandValue[] parameters) {
			throw new NotSupportedException();
		}
		#endregion
	}
	public class DataAccessPostgreSqlConnectionProvider : DataAccessPostgreSqlConnectionProviderBase, ISupportStoredProc, IDataStoreEx {
		[SuppressMessage("Microsoft.Design", "CA1021")]
		public static IDataStore DataAccessCreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection};
			return DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore DataAccessCreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DataAccessPostgreSqlConnectionProvider(connection, autoCreateOption);
		}
		static DataAccessPostgreSqlConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterDataStoreProvider("NpgsqlConnection", DataAccessCreateProviderFromConnection);
			RegisterFactory(new DataAccessPostgreSqlProviderFactory());
		}
		public static void ProviderRegister() {
		}
		public DataAccessPostgreSqlConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		public override DBStoredProcedure[] GetStoredProcedures() {
			return ((ISupportStoredProc)this).GetStoredProcedures();
		}
		#region ISupportStoredProc Members
		DBStoredProcedure[] ISupportStoredProc.GetStoredProcedures(params string[] procedureNames) {
			List<DBStoredProcedure> procedures = new List<DBStoredProcedure>();
			if(procedureNames.Length == 0) {
				const string selectQuery = @"SELECT proname
                                             FROM pg_catalog.pg_namespace n
                                               JOIN pg_catalog.pg_proc p ON pronamespace = n.oid
                                             WHERE nspname = 'public' and prorettype <> '2279' and proisagg = 'f'";
				DataStoreEx.ProcessQuery(CancellationToken.None, new Query(selectQuery), (reader, cancellationToken)=> {
					while(reader.Read()) {
						string name = reader.GetString(0);
						procedures.Add(ConnectionProviderHelper.CreateDBStoredProcedure(name));
					}
				});
			} else
				procedures.AddRange(procedureNames.Select(name => ConnectionProviderHelper.CreateDBStoredProcedure(name)));
			IDbCommand command = Connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			foreach(DBStoredProcedure sproc in procedures) {
				IDataReader reader = null;
				try {
					command.CommandText = FormatColumn(sproc.Name);
					CommandBuilderDeriveParameters(command);
					List<string> fakeParams = new List<string>();
					List<DBStoredProcedureArgument> dbArguments = new List<DBStoredProcedureArgument>();
					foreach(IDataParameter parameter in command.Parameters) {
						DBStoredProcedureArgumentDirection direction = ConnectionProviderHelper.GetDBStoredProcedureArgumentDirection(parameter);
						DBColumnType columnType = GetColumnType(parameter.DbType, true);
						dbArguments.Add(new DBStoredProcedureArgument(parameter.ParameterName, columnType, direction));
						fakeParams.Add("null");
					}
					sproc.Arguments.AddRange(dbArguments);
					command.CommandType = CommandType.Text;
					command.CommandText = string.Format("select * from \"{0}\"({1}) where 1=0", sproc.Name, string.Join(", ", fakeParams.ToArray()));
					reader = command.ExecuteReader();
					DBStoredProcedureResultSet curResultSet = new DBStoredProcedureResultSet();
					List<DBNameTypePair> dbColumns = new List<DBNameTypePair>();
					for(int i = 0; i < reader.FieldCount; i++) {
						DBColumnType columnType = DBColumn.GetColumnType(reader.GetFieldType(i));
						dbColumns.Add(new DBNameTypePair(reader.GetName(i), columnType));
					}
					curResultSet.Columns.AddRange(dbColumns);
					reader.Close();
					sproc.ResultSets.Add(curResultSet);
				} catch {
					if(reader != null && !reader.IsClosed) {
						reader.Close();
					}
				}
			}
			return procedures.ToArray();
		}
		DBStoredProcedureResultSet ISupportStoredProc.GetStoredProcedureTableSchema(string procedureName) {
			using(IDbCommand command = Connection.CreateCommand()) {
				command.CommandType = CommandType.StoredProcedure;
				command.CommandText = FormatColumn(procedureName);
				CommandBuilderDeriveParameters(command);
				List<string> fakeParams = new List<string>();
				for(int i = 0; i < command.Parameters.Count; i++)
					fakeParams.Add("null");
				command.CommandType = CommandType.Text;
				command.CommandText = string.Format("select * from \"{0}\"({1}) where 1=0", procedureName, string.Join(", ", fakeParams.ToArray()));
				return ConnectionProviderHelper.GetResultSetFromCommand(command);
			}
		}
		#endregion
		#region IDataStoreEx
		void IDataStoreEx.ProcessQuery(CancellationToken cancellationToken, Query query, Action<IDataReader, CancellationToken> action) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					using(IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess)) {
						action(reader, cancellationToken);
					}
				}
			}
		}
		void IDataStoreEx.ProcessStoredProc(CancellationToken cancellationToken, string sprocName, Action<IDataReader, CancellationToken> action, params OperandValue[] parameters) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand()) {
					PrepareCommandForStoredProc(command, sprocName, parameters);
					using(IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess)) {
						action(reader, cancellationToken);
					}
				}
			}
		}
		SelectedDataEx IDataStoreEx.SelectData(CancellationToken cancellationToken, Query query, string[] columns) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					return DataStoreExHelper.GetData(command, cancellationToken, columns, NativeSkipTakeSupported, query.SkipSelectedRecords);
				}
			}
		}
		SelectedDataEx IDataStoreEx.ExecuteStoredProcedure(CancellationToken cancellationToken, string sprocName, params OperandValue[] parameters) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand()) {
					PrepareCommandForStoredProc(command, sprocName, parameters);
					return DataStoreExHelper.GetData(command, cancellationToken);
				}
			}
		}
		void PrepareCommandForStoredProc(IDbCommand command, string sprocName, OperandValue[] parameters) {
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = FormatTable(ComposeSafeSchemaName(sprocName), ComposeSafeTableName(sprocName));
			CommandBuilderDeriveParameters(command);
			IDataParameter returnParameter;
			List<IDataParameter> outParameters;
			PrepareParametersForExecuteSproc(parameters, command, out outParameters, out returnParameter);
		}
		#endregion
	}
	public class DataAccessPostgreSqlProviderFactory : PostgreSqlProviderFactory {
		public const string PortParamID = "Port";
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessPostgreSqlConnectionProvider.DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessPostgreSqlConnectionProvider.DataAccessCreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string[] GetDatabases(string server, string userid, string password) {
			return GetDatabases(server, string.Empty, userid, password);
		}
		public string[] GetDatabases(string server, string port, string userid, string password) {
			int portNumber;
			string connectionString = int.TryParse(port, out portNumber)
				? DataAccessPostgreSqlConnectionProvider.GetConnectionString(server, portNumber, userid, password, string.Empty)
				: DataAccessPostgreSqlConnectionProvider.GetConnectionString(server, userid, password, string.Empty);
			Xpo.DB.Helpers.ConnectionStringParser helper = new Xpo.DB.Helpers.ConnectionStringParser(connectionString);
			helper.RemovePartByName(DataStoreBase.XpoProviderTypeParameterName);
			helper.RemovePartByName("database");
			string parsedConnectionString = helper.GetConnectionString();
			using(IDbConnection connection = DataAccessPostgreSqlConnectionProvider.CreateConnection(parsedConnectionString)) {
				try {
					connection.Open();
					using(IDbCommand command = connection.CreateCommand()) {
						command.CommandText = "SELECT datname FROM pg_database " +
											  "WHERE datistemplate = false;";
						using(IDataReader reader = command.ExecuteReader()) {
							List<string> result = new List<string>();
							while(reader.Read()) {
								result.Add((string)reader.GetValue(0));
							}
							return result.ToArray();
						}
					}
				} catch(Exception) {
					return new string[0];
				}
			}
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(ServerParamID) || !parameters.ContainsKey(DatabaseParamID) || !parameters.ContainsKey(UserIDParamID) || !parameters.ContainsKey(PasswordParamID))
				return null;
			return parameters.ContainsKey(PortParamID)
				? PostgreSqlConnectionProvider.GetConnectionString(parameters[ServerParamID], int.Parse(parameters[PortParamID]), parameters[UserIDParamID], parameters[PasswordParamID], parameters[DatabaseParamID])
				: PostgreSqlConnectionProvider.GetConnectionString(parameters[ServerParamID], parameters[UserIDParamID], parameters[PasswordParamID], parameters[DatabaseParamID]);
		}
	}
}
