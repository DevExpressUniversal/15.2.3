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
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.DataAccess.Native.Sql.ConnectionProviders {
	public class DataAccessAsaConnectionProvider : AsaConnectionProvider, IAliasFormatter, IDataStoreSchemaExplorerEx, ISupportStoredProc, IDataStoreEx {
		#region static
		static DBColumnType GetColumnType(string typeStr, int length) {
			switch(typeStr) {
				case "integer":
					return DBColumnType.Int32;
				case "image":
					return DBColumnType.ByteArray;
				case "char":
				case "nchar":
				case "text":
				case "ntext":
				case "varchar":
				case "nvarchar":
				case "long varchar":
				case "long nvarchar":
					if(length == 1)
						return DBColumnType.Char;
					return DBColumnType.String;
				case "bit":
					return DBColumnType.Boolean;
				case "tinyint":
					return DBColumnType.Byte;
				case "smallint":
					return DBColumnType.Int16;
				case "bigint":
					return DBColumnType.Int64;
				case "decimal":
				case "numeric":
				case "money":
					return DBColumnType.Decimal;
				case "float":
					return DBColumnType.Double;
				case "uniqueidentifier":
					return DBColumnType.Guid;
				case "date":
				case "datetime":
				case "timestamp":
					return DBColumnType.DateTime;
			}
			return DBColumnType.Unknown;
		}
		static int GetColumnLength(string typeStr, int length, int charsize) {
			switch(typeStr) {
				case "char":
				case "nchar":
				case "text":
				case "ntext":
				case "nvarchar":
				case "long nvarchar":
					return length;
				case "varchar":
				case "long varchar":
					return length/charsize;
			}
			return 0;
		}
		public static string GetConnectionString(string server, string hostname, string database, string userid, string password) {
			return string.Format("{5}={6};eng={0};uid={1};pwd={2};dbn={3};LINKS=tcpip{{HOST={4}}};persist security info=true;",
				server, userid, password, database, hostname, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString);
		}
		#endregion
		const string aliasLead = "\"";
		const string aliasEnd = "\"";
		const bool singleQuotedString = true;
		[SuppressMessage("Microsoft.Design", "CA1021")]
		public static IDataStore DataAccessCreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection};
			return DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore DataAccessCreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DataAccessAsaConnectionProvider(connection, autoCreateOption);
		}
		static DataAccessAsaConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterDataStoreProvider("iAnywhere.Data.SQLAnywhere.SAConnection", DataAccessCreateProviderFromConnection);
			RegisterFactory(new DataAccessAsaProviderFactory());
		}
		public static void ProviderRegister() {
		}
		IDataStoreEx DataStoreEx { get { return this; } }
		public DataAccessAsaConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			bool shouldCreateParameter = createParameter;
			OperandParameter param = parameter as OperandParameter;
			if(!ReferenceEquals(param, null)) {
				createParameter = true;
				return string.Format(":{0}", param.ParameterName);
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
			return string.IsNullOrEmpty(encloseAlias) 
				? FormatTable(schema, tableName) 
				: string.Format("{0} {1}", FormatTable(schema, tableName), encloseAlias);
		}
		public override string FormatTable(string schema, string tableName) {
			return string.IsNullOrEmpty(schema) 
				? string.Format(CultureInfo.InvariantCulture, "\"{0}\"", tableName) 
				: string.Format(CultureInfo.InvariantCulture, "\"{0}\".\"{1}\"", schema, tableName);
		}
		#region IAliasFormatter
		string IAliasFormatter.AliasLead { get { return aliasLead; } }
		string IAliasFormatter.AliasEnd { get { return aliasEnd; } }
		bool IAliasFormatter.SingleQuotedString { get { return singleQuotedString; } }
		int IAliasFormatter.MaxTableAliasLength { get { return GetSafeNameTableMaxLength(); } }
		int IAliasFormatter.MaxColumnAliasLength { get { return GetSafeNameColumnMaxLength(); } }
		#endregion
		#region IDataStoreSchemaExplorerEx
		string[] IDataStoreSchemaExplorerEx.GetStorageDataObjectsNames(DataObjectTypes types) {
			List<string> result = new List<string>();
			string getTablesListQuery = string.Format(@"select case when u.user_name <> user_name() then u.user_name || '.' || t.table_name else t.table_name end as table_name
                                               from systable t
                                                 join sysuserperm u on t.creator = u.user_id
                                                 where ({0}) and u.user_name not in ('sys', 'dbo', 'SYS', 'DBO')
                                                             and u.user_name NOT LIKE 'rs_sys%' and u.user_name NOT LIKE 'RS_SYS%'",
				ConnectionProviderHelper.GetDataObjectTypeCondition(types, "t.TABLE_TYPE = 'BASE'", "t.TABLE_TYPE = 'VIEW'"));
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
			string getTablesColumnsQuery = @"select case when u.user_name <> user_name() then u.user_name || '.' || t.table_name else t.table_name end as table_name
                                                  , cname, coltype, length, in_primary_key, default_value, @@ncharsize, colno
                                             from systable t
                                                 join sysuserperm u on t.creator = u.user_id
                                                 join sys.syscolumns c on c.tname = t.table_name and c.creator = u.user_name
                                             where u.user_name not in ('sys', 'dbo', 'SYS', 'DBO') and u.user_name NOT LIKE 'rs_sys%' and u.user_name NOT LIKE 'RS_SYS%'";
			getTablesColumnsQuery = useTablesFilter
				? string.Format("select * from ({0}) table_columns {1} order by table_name, colno", getTablesColumnsQuery, GetTablesFilter(tables, "where table_name")) 
				: string.Format("{0} order by tname, colno", getTablesColumnsQuery);
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesColumnsQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string tableName = reader.GetString(0);
					if(table == null || !string.Equals(table.Name, tableName))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, tableName));
					if(table == null)
						continue;
					string columnName = reader.GetString(1);
					string typeStr = reader.GetString(2);
					int len = Convert.ToInt32(reader.GetValue(3));
					DBColumnType type = GetColumnType(typeStr, len);
					bool isKey = !reader.IsDBNull(4) && reader.GetString(4) == "Y";
					bool isIdentity = !reader.IsDBNull(5) && reader.GetString(5) == "autoincrement";
					int length = GetColumnLength(typeStr, len, Convert.ToInt32(reader.GetValue(6)));
					DBColumn column = ConnectionProviderHelper.CreateColumn(columnName, isKey, isIdentity, length, type);
					table.AddColumn(column);
				}
			});
		}
		void GetStorageTablesForeignKeys(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.ForeignKeys.Clear();
			string getTablesForeignKeysQuery = @"select case when fu.user_name <> user_name() then fu.user_name || '.' || ft.table_name else ft.table_name end as foreign_table_name,
                                                        fc.column_name as foreign_column_name, sc.constraint_name,
                                                        case when pu.user_name <> user_name() then pu.user_name || '.' || pt.table_name else pt.table_name end as primary_table_name,
                                                        pc.column_name as primary_column_name
                                                 from sysfkcol fkdata
                                                   join sysforeignkey f on f.foreign_key_id = fkdata.foreign_key_id and f.foreign_table_id = fkdata.foreign_table_id
                                                   join systable pt on pt.table_id = f.primary_table_id
                                                   join systable ft on ft.table_id = f.foreign_table_id
                                                   join sysuserperm pu on pt.creator = pu.user_id
                                                   join sysuserperm fu on ft.creator = fu.user_id
                                                   join syscolumn pc on fkdata.primary_column_id = pc.column_id and f.primary_table_id = pc.table_id
                                                   join syscolumn fc on fkdata.foreign_column_id = fc.column_id and f.foreign_table_id = fc.table_id
                                                   join sysconstraint sc on sc.table_object_id = ft.object_id and sc.ref_object_id = f.object_id
                                                 where fu.user_name not in ('sys', 'dbo', 'SYS', 'DBO') and fu.user_name NOT LIKE 'rs_sys%' and fu.user_name NOT LIKE 'RS_SYS%'
                                                       and pu.user_name not in ('sys', 'dbo', 'SYS', 'DBO') and pu.user_name NOT LIKE 'rs_sys%' and pu.user_name NOT LIKE 'RS_SYS%'
                                                               ";
			getTablesForeignKeysQuery = useTablesFilter 
				? string.Format("select * from ({0}) table_columns {1} order by constraint_name", getTablesForeignKeysQuery, GetTablesFilter(tables, "where foreign_table_name")) 
				: string.Format("{0} order by sc.constraint_name", getTablesForeignKeysQuery);
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesForeignKeysQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				DBForeignKey fk = null;
				while(reader.Read()) {
					string foreignTableName = reader.GetString(0);
					if(table == null || !string.Equals(table.Name, foreignTableName)) {
						table = tables.FirstOrDefault(t => string.Equals(t.Name, foreignTableName));
						fk = null;
					}
					if(table == null)
						continue;
					string primaryColumnName = reader.GetString(1);
					string keyName = reader.GetString(2);
					string primaryTableName = reader.GetString(3);
					string foreignColumnName = reader.GetString(4);
					if(fk == null || !string.Equals(fk.Name, keyName))
						fk = table.ForeignKeys.FirstOrDefault(k => string.Equals(k.Name, keyName));
					if(fk == null) {
						fk = new DBForeignKey {
							Name = keyName, PrimaryKeyTable = primaryTableName
						};
						table.ForeignKeys.Add(fk);
					}
					fk.Columns.Add(primaryColumnName);
					fk.PrimaryKeyTableKeyColumns.Add(foreignColumnName);
				}
			});
		}
		#endregion
		#region ISupportStoredProc
		DBStoredProcedure[] ISupportStoredProc.GetStoredProcedures(params string[] procedureNames) {
			List<DBStoredProcedure> procedures = new List<DBStoredProcedure>();
			if(procedureNames.Length == 0) {
				const string getProcedureNamesQuery = "select distinct proc_name from sys.sysprocedure where creator = 1 order by proc_name";
				DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getProcedureNamesQuery), (reader, cancellationToken)=> {
					while(reader.Read()) {
						string name = reader.GetString(0);
						procedures.Add(ConnectionProviderHelper.CreateDBStoredProcedure(name));
					}
				});
			} else
				procedures.AddRange(procedureNames.Select(name => ConnectionProviderHelper.CreateDBStoredProcedure(name)));
			using(IDbCommand command = Connection.CreateCommand()) {
				foreach(DBStoredProcedure procedure in procedures) {
					try {
						PrepareCommandForGetStoredProcParameters(command, procedure.Name);
						procedure.Arguments.AddRange(ConnectionProviderHelper.GetStoredProcedureArgumentsFromCommand(command));
					} catch {
					}
				}
			}
			return procedures.ToArray();
		}
		DBStoredProcedureResultSet ISupportStoredProc.GetStoredProcedureTableSchema(string procedureName) {
			using(IDbCommand command = Connection.CreateCommand()) {
				PrepareCommandForGetStoredProcParameters(command, procedureName);
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
					PrepareCommandForExecuteStoredProc(command, sprocName, parameters);
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
					PrepareCommandForExecuteStoredProc(command, sprocName, parameters);
					return DataStoreExHelper.GetData(command, cancellationToken);
				}
			}
		}
		void PrepareCommandForExecuteStoredProc(IDbCommand command, string sprocName, OperandValue[] parameters) {
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
	public class DataAccessAsaProviderFactory : AsaProviderFactory {
		public override string FileFilter { get { return "SAP SQL Anywhere databases|*.db"; } }
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessAsaConnectionProvider.DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessAsaConnectionProvider.DataAccessCreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(DatabaseParamID) || !parameters.ContainsKey(UserIDParamID) || !parameters.ContainsKey(PasswordParamID))
				return null;
			string database = parameters[DatabaseParamID];
			string userid = parameters[UserIDParamID];
			string password = parameters[PasswordParamID];
			if(parameters.ContainsKey(ServerParamID)) {
				string server = parameters[ServerParamID];
				if(parameters.ContainsKey(DataAccessConnectionParameter.HostnameParamID))
					return DataAccessAsaConnectionProvider.GetConnectionString(server, parameters[DataAccessConnectionParameter.HostnameParamID], database, userid, password);
				return AsaConnectionProvider.GetConnectionString(server, database, userid, password);
			}
			return AsaConnectionProvider.GetConnectionString(database, userid, password);
		}
	}
}
