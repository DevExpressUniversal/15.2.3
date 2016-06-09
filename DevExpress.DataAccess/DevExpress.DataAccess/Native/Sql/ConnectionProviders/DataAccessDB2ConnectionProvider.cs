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
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.DataAccess.Native.Sql.ConnectionProviders {
	public class DataAccessDB2ConnectionProvider : DB2ConnectionProvider, IAliasFormatter, IDataStoreSchemaExplorerEx, ISupportStoredProc, IDataStoreEx {
		#region static
		static DBColumnType GetColumnType(string typeName, int size) {
			switch(typeName) {
				case "INTEGER":
					return DBColumnType.Int32;
				case "BLOB":
					return DBColumnType.ByteArray;
				case "VARCHAR":
					return DBColumnType.String;
				case "SMALLINT":
					return DBColumnType.Int16;
				case "BIGINT":
					return DBColumnType.Int64;
				case "DOUBLE":
					return DBColumnType.Double;
				case "REAL":
					return DBColumnType.Single;
				case "CHARACTER":
					return size == 1 ? DBColumnType.Char : DBColumnType.String;
				case "DECIMAL":
					return DBColumnType.Decimal;
				case "DATE":
				case "TIMESTAMP":
					return DBColumnType.DateTime;
				case "CLOB":
					return DBColumnType.String;
			}
			return DBColumnType.Unknown;
		}
		#endregion
		const string aliasLead = "\"";
		const string aliasEnd = "\"";
		const bool singleQuotedString = true;
		string currentUser;
		string CurrentUser {
			get { return this.currentUser ?? (this.currentUser = GetCurrentUser()); }
		}
		[SuppressMessage("Microsoft.Design", "CA1021")]
		public static IDataStore DataAccessCreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection};
			return DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore DataAccessCreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DataAccessDB2ConnectionProvider(connection, autoCreateOption);
		}
		static DataAccessDB2ConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterDataStoreProvider("DB2Connection", DataAccessCreateProviderFromConnection);
			RegisterFactory(new DataAccessDB2ProviderFactory());
		}
		public static void ProviderRegister() {
		}
		IDataStoreEx DataStoreEx { get { return this; } }
		public DataAccessDB2ConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
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
						return string.Format("TIMESTAMP '{0}'", ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
				}
			}
			createParameter = shouldCreateParameter;
			return base.GetParameterName(new ConstantValue(parameter.Value), index, ref createParameter);
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
		#region IAliasFormatter
		string IAliasFormatter.AliasLead { get { return aliasLead; } }
		string IAliasFormatter.AliasEnd { get { return aliasEnd; } }
		bool IAliasFormatter.SingleQuotedString { get { return singleQuotedString; } }
		int IAliasFormatter.MaxTableAliasLength { get { return GetSafeNameTableMaxLength(); } }
		int IAliasFormatter.MaxColumnAliasLength { get { return GetSafeNameColumnMaxLength(); } }
		#endregion
		#region IDataStoreSchemaExplorerEx Members
		string[] IDataStoreSchemaExplorerEx.GetStorageDataObjectsNames(DataObjectTypes types) {
			List<string> result = new List<string>();
			string typeCondition = ConnectionProviderHelper.GetDataObjectTypeCondition(types, "TYPE = 'T'", "TYPE = 'V'");
			string getTablesListQuery = string.Format(@"select CASE WHEN rtrim(TABSCHEMA) <> 'DB2ADMIN' then rtrim(TABSCHEMA) || '.' ELSE '' END || rtrim(TABNAME) as FULLTABNAME
                                                        from SYSCAT.TABLES
                                                        where TABSCHEMA <> 'SYSTOOLS' and OWNERTYPE = 'U' and ({0})", typeCondition);
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesListQuery), (reader, cancellationToken)=> {
				while(reader.Read()) {
					result.Add(reader.GetString(0));
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
			return tables.Count != 0 ? string.Format("{0} in ({1})", prefix, string.Join(", ", tables.Select(n => string.Format("'{0}'", n.Name)))) : string.Empty;
		}
		void GetStorageTablesColumns(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.Columns.Clear();
			string getTablesColumnsQuery = string.Format(@"select CASE WHEN rtrim(t.TABSCHEMA) <> '{0}' then rtrim(t.TABSCHEMA) || '.' ELSE '' END || rtrim(t.TABNAME) as FULLTABNAME,
                                                                  rtrim(COLNAME), TYPENAME, LENGTH, IDENTITY, c.colno
                                                           from SYSCAT.COLUMNS c
                                                            join SYSCAT.TABLES t on t.TABSCHEMA = c.TABSCHEMA and t.TABNAME = c.TABNAME
                                                           where t.TABSCHEMA <> 'SYSTOOLS' and t.OWNERTYPE = 'U' and t.TYPE in ('T','V')",
				string.IsNullOrEmpty(this.ObjectsOwner) ? CurrentUser : this.ObjectsOwner);
			getTablesColumnsQuery = useTablesFilter
				? string.Format("select * from ({0}) table_columns {1} order by colno", getTablesColumnsQuery, GetTablesFilter(tables, "where FULLTABNAME"))
				: string.Format("{0} order by colno", getTablesColumnsQuery);
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesColumnsQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string tableName = reader.GetString(0);
					if(table == null || !string.Equals(table.Name, tableName))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, tableName));
					if(table == null)
						continue;
					string columnName = reader.GetString(1);
					string typeName = reader.GetString(2);
					int size = reader.GetInt32(3);
					DBColumnType type = GetColumnType(typeName, size);
					DBColumn column = new DBColumn(columnName, false, string.Empty, type == DBColumnType.String ? size : 0, type);
					column.IsIdentity = reader.GetString(4) == "Y";
					table.AddColumn(column);
				}
			});
		}
		void GetStorageTablesForeignKeys(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.ForeignKeys.Clear();
			string getTablesForeignKeysQuery = string.Format(@"select CASE WHEN rtrim(pt.TABSCHEMA) <> '{0}' then rtrim(pt.TABSCHEMA) || '.' ELSE '' END || rtrim(pt.TABNAME) as PK_TABNAME,
                                                                      CASE WHEN rtrim(ft.TABSCHEMA) <> '{0}' then rtrim(ft.TABSCHEMA) || '.' ELSE '' END || rtrim(ft.TABNAME) as FK_TABNAME,
                                                                      FK_COLNAMES, PK_COLNAMES, r.CONSTNAME
                                                               from SYSCAT.REFERENCES r
                                                                join SYSCAT.TABLES ft on ft.TABSCHEMA = r.TABSCHEMA and ft.TABNAME = r.TABNAME
                                                                join SYSCAT.TABLES pt on pt.TABSCHEMA = r.REFTABSCHEMA and pt.TABNAME = r.REFTABNAME",
				string.IsNullOrEmpty(this.ObjectsOwner) ? CurrentUser : this.ObjectsOwner);
			if(useTablesFilter)
				getTablesForeignKeysQuery = string.Format("select * from ({0}) table_foreign_keys {1}", getTablesForeignKeysQuery, GetTablesFilter(tables, "where FK_TABNAME"));
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesForeignKeysQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string primaryTableName = reader.GetString(0);
					string foreignTableName = reader.GetString(1);
					if(table == null || !string.Equals(table.Name, foreignTableName))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, foreignTableName));
					if(table == null)
						continue;
					string foreignColumns = reader.GetString(2);
					string primaryColumns = reader.GetString(3);
					string keyName = reader.GetString(4);
					if(primaryColumns == null || foreignColumns == null)
						continue;
					StringCollection fkc = new StringCollection();
					fkc.AddRange(foreignColumns.Split(' ').Where(n => !string.IsNullOrWhiteSpace(n)).ToArray());
					StringCollection pkc = new StringCollection();
					pkc.AddRange(primaryColumns.Split(' ').Where(n => !string.IsNullOrWhiteSpace(n)).ToArray());
					DBForeignKey fk = new DBForeignKey(fkc, primaryTableName, pkc) {
						Name = keyName
					};
					table.ForeignKeys.Add(fk);
				}
			});
		}
		#endregion
		#region ISupportStoredProc
		DBStoredProcedure[] ISupportStoredProc.GetStoredProcedures(params string[] procedureNames) {
			List<DBStoredProcedure> procedures = new List<DBStoredProcedure>();
			if(procedureNames.Length == 0) {
				const string getProcedureNamesQuery = "select * from syscat.procedures where definer not in ('SYSIBM')";
				DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getProcedureNamesQuery), (reader, cancellationToken)=> {
					while(reader.Read()) {
						string name = reader.GetString(1);
						procedures.Add(ConnectionProviderHelper.CreateDBStoredProcedure(name));
					}
				});
			} else
				procedures.AddRange(procedureNames.Select(name => ConnectionProviderHelper.CreateDBStoredProcedure(name)));
			IDbCommand command = Connection.CreateCommand();
			foreach(DBStoredProcedure sproc in procedures) {
				try {
					PrepareCommandForGetStoredProcParameters(command, FormatTable(ComposeSafeSchemaName(sproc.Name), ComposeSafeTableName(sproc.Name)));
					sproc.Arguments.AddRange(ConnectionProviderHelper.GetStoredProcedureArgumentsFromCommand(command));
				} catch {
				}
			}
			return procedures.ToArray();
		}
		DBStoredProcedureResultSet ISupportStoredProc.GetStoredProcedureTableSchema(string procedureName) {
			using(IDbCommand command = Connection.CreateCommand()) {
				command.CommandType = CommandType.StoredProcedure;
				command.CommandText = FormatTable(ComposeSafeSchemaName(procedureName), ComposeSafeTableName(procedureName));
				CommandBuilderDeriveParameters(command);
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
		ColumnInfoEx[] IDataStoreEx.SelectSchema(Query query) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					return DataStoreExHelper.GetSchema(command);
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
			PrepareCommandForGetStoredProcParameters(command, sprocName);
			IDataParameter returnParameter;
			List<IDataParameter> outParameters;
			PrepareParametersForExecuteSproc(parameters, command, out outParameters, out returnParameter);
		}
		void PrepareCommandForGetStoredProcParameters(IDbCommand command, string sprocName) {
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = FormatTable(ComposeSafeSchemaName(sprocName), ComposeSafeTableName(sprocName));
			CommandBuilderDeriveParameters(command);
		}
		#endregion
	}
	public class DataAccessDB2ProviderFactory : DB2ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessDB2ConnectionProvider.DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessDB2ConnectionProvider.DataAccessCreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
	}
}
