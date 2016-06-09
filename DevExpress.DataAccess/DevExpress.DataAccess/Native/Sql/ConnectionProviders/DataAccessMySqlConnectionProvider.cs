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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.DataAccess.Native.Sql.ConnectionProviders {
	public class DataAccessMySqlConnectionProvider : MySqlConnectionProvider, IAliasFormatter, IDataStoreSchemaExplorerEx, ISupportStoredProc, IDataStoreEx {
		#region static
		static DBColumnType GetColumnType(string typeName) {
			if(string.IsNullOrEmpty(typeName))
				return DBColumnType.Unknown;
			switch(typeName) {
				case "char(1)":
					return DBColumnType.Char;
				case "tinyint(1)":
					return DBColumnType.Boolean;
			}
			string typeWithoutBrackets = RemoveBrackets(typeName);
			switch(typeWithoutBrackets.ToLowerInvariant()) {
				case "int":
				case "mediumint":
					return DBColumnType.Int32;
				case "int unsigned":
				case "mediumint unsigned":
					return DBColumnType.UInt32;
				case "longblob":
				case "tinyblob":
				case "mediumblob":
				case "blob":
					return DBColumnType.ByteArray;
				case "text":
				case "mediumtext":
				case "longtext":
					return DBColumnType.String;
				case "bit":
				case "tinyint unsigned":
					return DBColumnType.Byte;
				case "tinyint":
					return DBColumnType.SByte;
				case "smallint":
					return DBColumnType.Int16;
				case "smallint unsigned":
					return DBColumnType.UInt16;
				case "bigint":
					return DBColumnType.Int64;
				case "bigint unsigned":
					return DBColumnType.UInt64;
				case "double":
					return DBColumnType.Double;
				case "datetime":
				case "date":
				case "timestamp":
					return DBColumnType.DateTime;
				case "decimal":
					return DBColumnType.Decimal;
			}
			if(typeName.StartsWith("char("))
				return DBColumnType.String;
			if(typeName.StartsWith("varchar("))
				return DBColumnType.String;
			if(typeName.StartsWith("decimal("))
				return DBColumnType.Decimal;
			return DBColumnType.Unknown;
		}
		static int GetColumnSize(string typeName) {
			if(string.IsNullOrEmpty(typeName))
				return 0;
			if(typeName.StartsWith("char("))
				return int.Parse(typeName.Substring(5, typeName.Length - 6));
			if(typeName.StartsWith("varchar("))
				return int.Parse(typeName.Substring(8, typeName.Length - 9));
			return 0;
		}
		static string RemoveBrackets(string typeName) {
			string typeWithoutBrackets = typeName;
			int bracketOpen = typeName.IndexOf('(');
			if(bracketOpen >= 0) {
				int bracketClose = typeName.IndexOf(')', bracketOpen);
				if(bracketClose >= 0) {
					typeWithoutBrackets = typeName.Remove(bracketOpen, bracketClose - bracketOpen + 1);
				}
			}
			return typeWithoutBrackets;
		}
		static string ReadStringValue(IDataReader reader, int index) {
			object value = reader.GetValue(index);
			byte[] bytes = value as byte[];
			return bytes != null ? Encoding.Default.GetString(bytes) : (string)value;
		}
		static StringCollection SplitColumns(string refs) {
			StringCollection cols = new StringCollection();
			Regex regColumns = new Regex(@"`(?<column>[^`]+)`");
			MatchCollection matches = regColumns.Matches(refs);
			foreach(Match match in matches) {
				if(match.Success)
					cols.Add(match.Groups["column"].Value);
			}
			return cols;
		}
		#endregion
		const string aliasLead = "`";
		const string aliasEnd = "`";
		const bool singleQuotedString = false;
		object[] mySqlByteArrayTypes;
		GetPropertyValueDelegate getMySqlDbType;
		object[] MySqlByteArrayTypes {
			get {
				if(this.mySqlByteArrayTypes == null)
					InitializeMySqlDbTypeType();
				return this.mySqlByteArrayTypes;
			}
		}
		GetPropertyValueDelegate GetMySqlDbType {
			get {
				if(this.getMySqlDbType == null)
					InitializeGetMySqlDbType();
				return this.getMySqlDbType;
			}
		}
		[SuppressMessage("Microsoft.Design", "CA1021")]
		public static IDataStore DataAccessCreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection};
			return DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore DataAccessCreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DataAccessMySqlConnectionProvider(connection, autoCreateOption);
		}
		public static string GetConnectionString(string server, string userid, string password, string database, string port) {
			if(string.IsNullOrWhiteSpace(port))
				return GetConnectionString(server, userid, password, database);
			return string.Format("{4}={5};server={0};port={6};user id={1}; password={2}; database={3};persist security info=true;CharSet=utf8;", server, userid, password, database, XpoProviderTypeParameterName, XpoProviderTypeString, port);
		}
		static DataAccessMySqlConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterDataStoreProvider("MySql.Data.MySqlClient.MySqlConnection", DataAccessCreateProviderFromConnection);
			RegisterFactory(new DataAccessMySqlProviderFactory());
		}
		public static void ProviderRegister() {
		}
		IDataStoreEx DataStoreEx { get { return this; } }
		public DataAccessMySqlConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			bool shouldCreateParameter = createParameter;
			OperandParameter param = parameter as OperandParameter;
			if(!ReferenceEquals(param, null)) {
				createParameter = true;
				return string.Format("?{0}", param.ParameterName);
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
		public override string FormatTable(string schema, string tableName) {
			return !string.IsNullOrEmpty(schema) 
				? string.Format(CultureInfo.InvariantCulture, "`{0}`.`{1}`", schema, tableName) 
				: base.FormatTable(schema, tableName);
		}
		public override string FormatColumn(string columnName, string tableAlias) {
			return base.FormatColumn(columnName, ConnectionProviderHelper.EncloseAlias(tableAlias, aliasLead, aliasEnd));
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			if(tableName == SubSelectStatement.CustomSqlString)
				return tableAlias;
			string encloseAlias = ConnectionProviderHelper.EncloseAlias(tableAlias, aliasLead, aliasEnd);
			if(!string.IsNullOrEmpty(schema))
				return string.IsNullOrEmpty(encloseAlias) 
				? FormatTable(schema, tableName) 
				: string.Format(CultureInfo.InvariantCulture, "`{0}`.`{1}` {2}", schema, tableName, encloseAlias);
			return string.IsNullOrEmpty(encloseAlias)
				? base.FormatTable(schema, tableName)
				: base.FormatTable(schema, tableName, encloseAlias);
		}
		public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.AddDays:
					return string.Format(CultureInfo.InvariantCulture, "AddDate({0}, interval {1} day)", operands[0], operands[1]);
				default:
					return base.FormatFunction(operatorType, operands);
			}
		}
		protected override void PrepareDelegates() {
		}
		void InitializeMySqlDbTypeType() {
			Type mySqlDbTypeType = ConnectionHelper.GetType("MySql.Data.MySqlClient.MySqlDbType");
			this.mySqlByteArrayTypes = new object[] {
				Enum.Parse(mySqlDbTypeType, "TinyBlob"),
				Enum.Parse(mySqlDbTypeType, "MediumBlob"),
				Enum.Parse(mySqlDbTypeType, "LongBlob"),
				Enum.Parse(mySqlDbTypeType, "Blob")
			};
		}
		void InitializeGetMySqlDbType() {
			Type mySqlParameterType = ConnectionHelper.GetType("MySql.Data.MySqlClient.MySqlParameter");
			SetPropertyValueDelegate setMySqlDbType;
			ReflectConnectionHelper.CreatePropertyDelegates(mySqlParameterType, "MySqlDbType", out setMySqlDbType, out this.getMySqlDbType);
		}
		ReflectConnectionHelper helper;
		ReflectConnectionHelper ConnectionHelper { get { return this.helper ?? (this.helper = new ReflectConnectionHelper(Connection, "MySql.Data.MySqlClient.MySqlException")); } }
		MethodInfo miServerVersion;
		bool Is5 {
			get {
				if(this.miServerVersion == null) {
					Type connType = Connection.GetType();
					if(ConnectionHelper.ConnectionType.IsInstanceOfType(Connection)) {
						PropertyInfo pi = connType.GetProperty("ServerVersion", BindingFlags.Instance | BindingFlags.Public);
						if(pi != null && pi.CanRead) {
							this.miServerVersion = pi.GetGetMethod();
						}
					}
				}
				if(this.miServerVersion == null)
					throw new InvalidOperationException("MySql server version not found.");
				string serverVersion = (string)this.miServerVersion.Invoke(Connection, new object[0]);
				return !serverVersion.StartsWith("4.");
			}
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
			if(Is5) {
				string getTablesListQuery = string.Format(@"select TABLE_NAME
                                  from   INFORMATION_SCHEMA.TABLES
                                  where  table_schema = '{0}' and ({1})",
					Connection.Database, ConnectionProviderHelper.GetDataObjectTypeCondition(types, "TABLE_TYPE = 'BASE TABLE'", "TABLE_TYPE = 'VIEW'"));
				GetObjectNames(getTablesListQuery, result);
			} else {
				if((types & DataObjectTypes.Tables) == DataObjectTypes.Tables) {
					string getTablesListQuery = string.Format(@"show tables from {0}", Connection.Database);
					GetObjectNames(getTablesListQuery, result);
				}
			}
			return result.ToArray();
		}
		void GetObjectNames(string getTablesListQuery, List<string> result) {
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesListQuery), (reader, cancellationToken)=> {
				while(reader.Read()) {
					result.Add(reader.GetString(0));
				}
			});
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
			IEnumerable<string> filteredTables = tablesList.Length == 0 ? tableNames : tableNames.Where(n => tablesList.Any(f => string.Equals(f, n, StringComparison.InvariantCulture)));
			DBTable[] tables = filteredTables.Select(p => ConnectionProviderHelper.CreateTable(p, isView)).ToArray();
			if(includeColumns)
				GetStorageTablesColumns(tables, tablesList.Length != 0);
			GetStorageTablesForeignKeys(tables, tablesList.Length != 0);
			return tables;
		}
		static string GetTablesFilter(ICollection<DBTable> tables, string prefix, bool useTablesFilter) {
			return (useTablesFilter && tables.Count != 0) ? string.Format("{0} in ({1})", prefix, string.Join(", ", tables.Select(n => string.Format("'{0}'", n.Name)))) : string.Empty;
		}
		void GetStorageTablesColumns(ICollection<DBTable> tables, bool useTablesFilter) {
			if(Is5) {
				GetStorageTablesColumnsMySql5(tables, useTablesFilter);
			} else {
				GetStorageTablesColumnsMySql4(tables);
			}
		}
		void GetStorageTablesColumnsMySql4(ICollection<DBTable> tables) {
			foreach(DBTable table in tables) {
				DBTable dbTable = table;
				dbTable.Columns.Clear();
				string getTablesColumnsQuery = string.Format(@"show columns from `{0}`", table.Name);
				DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesColumnsQuery), (reader, cancellationToken)=> {
					while(reader.Read()) {
						string rowColumnName = ReadStringValue(reader, 0);
						string rowType = ReadStringValue(reader, 1);
						string rowExtra = ReadStringValue(reader, 5);
						int size = GetColumnSize(rowType);
						DBColumnType type = GetColumnType(rowType);
						bool isIdentity = !string.IsNullOrEmpty(rowExtra) && rowExtra.Contains("auto_increment");
						dbTable.AddColumn(ConnectionProviderHelper.CreateColumn(rowColumnName, false, isIdentity, size, type));
					}
				});
			}
		}
		void GetStorageTablesColumnsMySql5(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.Columns.Clear();
			string getTablesColumnsQuery = string.Format(@"select TABLE_NAME, COLUMN_NAME, COLUMN_TYPE, EXTRA like '%auto_increment%' as IsIdentity
                                                               from   INFORMATION_SCHEMA.COLUMNS
                                                               where  TABLE_SCHEMA = '{0}' {1}",
				Connection.Database, GetTablesFilter(tables, "and TABLE_NAME", useTablesFilter));
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesColumnsQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string tableName = reader.GetString(0);
					string columnName = reader.GetString(1);
					if(table == null || !string.Equals(table.Name, tableName, StringComparison.InvariantCulture))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, tableName, StringComparison.InvariantCulture));
					if(table == null)
						continue;
					string typeName = reader.GetString(2);
					string identity = reader.GetString(3);
					int size = GetColumnSize(typeName);
					DBColumnType type = GetColumnType(typeName);
					table.AddColumn(ConnectionProviderHelper.CreateColumn(columnName, false, string.Equals(identity, "1"), size, type));
				}
			});
		}
		void GetStorageTablesForeignKeys(ICollection<DBTable> tables, bool useTablesFilter) {
			if(Is5) {
				GetStorageTablesForeignKeysMySql5(tables, useTablesFilter);
			} else
				GetStorageTablesForeignKeysMySql4(tables);
		}
		void GetStorageTablesForeignKeysMySql4(IEnumerable<DBTable> tables) {
			foreach(DBTable table in tables) {
				table.ForeignKeys.Clear();
				string getTablesForeignKeysQuery = string.Format(@"show create table `{0}`", table.Name);
				string createTableScript = null;
				DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesForeignKeysQuery), (reader, cancellationToken)=> {
					try {
						reader.Read();
						object val = reader.GetValue(1);
						createTableScript = val as string ?? Encoding.Default.GetString((byte[])val);
					} catch {
					}
				});
				if(string.IsNullOrWhiteSpace(createTableScript))
					continue;
				Regex regConstraint = new Regex(@"CONSTRAINT `(?<name>[^`]+)` FOREIGN KEY \((?<columns>.+)\) REFERENCES `(?<table>[^`]+)` \((?<refcolumns>.+)\)");
				MatchCollection matches = regConstraint.Matches(createTableScript);
				foreach(Match match in matches) {
					if(match.Success) {
						string keyName = match.Groups["name"].Value;
						string refTable = match.Groups["table"].Value;
						StringCollection cols = SplitColumns(match.Groups["columns"].Value);
						StringCollection fcols = SplitColumns(match.Groups["refcolumns"].Value);
						table.AddForeignKey(new DBForeignKey(cols, refTable, fcols) {
							Name = keyName
						});
					}
				}
			}
		}
		void GetStorageTablesForeignKeysMySql5(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.ForeignKeys.Clear();
			string getTablesForeignKeysQuery = string.Format(@"select TABLE_NAME, COLUMN_NAME, CONSTRAINT_NAME, REFERENCED_TABLE_NAME, REFERENCED_COLUMN_NAME
                                                                   from   INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                                                                   where  TABLE_SCHEMA = '{0}' and REFERENCED_TABLE_NAME is not null {1}
                                                                   order by CONSTRAINT_NAME, ORDINAL_POSITION",
				Connection.Database, GetTablesFilter(tables, "and TABLE_NAME", useTablesFilter));
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesForeignKeysQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string tableName = reader.GetString(0);
					string columnName = reader.GetString(1);
					if(table == null || !string.Equals(table.Name, tableName, StringComparison.InvariantCulture))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, tableName, StringComparison.InvariantCulture));
					if(table == null)
						continue;
					string keyName = reader.GetString(2);
					DBForeignKey fk = table.ForeignKeys.FirstOrDefault(k => string.Equals(k.Name, keyName, StringComparison.InvariantCulture));
					if(fk == null) {
						fk = new DBForeignKey {
							Name = keyName, PrimaryKeyTable = reader.GetString(3)
						};
						table.ForeignKeys.Add(fk);
					}
					fk.Columns.Add(columnName);
					fk.PrimaryKeyTableKeyColumns.Add(reader.GetString(4));
				}
			});
		}
		#endregion
		#region ISupportStoredProc Members
		DBStoredProcedure[] ISupportStoredProc.GetStoredProcedures(params string[] procedureNames) {
			if(!Is5)
				return new DBStoredProcedure[0];
			List<DBStoredProcedure> result = new List<DBStoredProcedure>();
			if(procedureNames.Length == 0) {
				const string getProcedureNamesQuery = "select routine_schema, routine_name from information_schema.ROUTINES where routine_schema = database()";
				DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getProcedureNamesQuery), (reader, cancellationToken)=> {
					while(reader.Read()) {
						result.Add(ConnectionProviderHelper.CreateDBStoredProcedure(string.Format("{0}.{1}", reader.GetString(0), reader.GetString(1))));
					}
				});
			} else
				result.AddRange(procedureNames.Select(name => ConnectionProviderHelper.CreateDBStoredProcedure(name)));
			IDbCommand command = Connection.CreateCommand();
			foreach(DBStoredProcedure procedure in result) {
				PrepareCommandForGetStoredProcParameters(command, procedure.Name);
				List<DBStoredProcedureArgument> dbArguments = new List<DBStoredProcedureArgument>();
				foreach(IDataParameter parameter in command.Parameters) {
					if(parameter.Direction == ParameterDirection.ReturnValue)
						continue;
					DBStoredProcedureArgumentDirection direction = ConnectionProviderHelper.GetDBStoredProcedureArgumentDirection(parameter);
					DBColumnType columnType = ConnectionProviderSql.GetColumnType(parameter.DbType, true);
					if(columnType == DBColumnType.Unknown && MySqlByteArrayTypes != null) {
						object mySqlDbType = GetMySqlDbType(parameter);
						foreach(object mySqlByteArrayType in MySqlByteArrayTypes) {
							if(object.Equals(mySqlByteArrayType, mySqlDbType)) {
								columnType = DBColumnType.ByteArray;
							}
						}
					}
					dbArguments.Add(new DBStoredProcedureArgument(parameter.ParameterName.TrimEnd(), columnType, direction));
				}
				procedure.Arguments.AddRange(dbArguments);
			}
			return result.ToArray();
		}
		DBStoredProcedureResultSet ISupportStoredProc.GetStoredProcedureTableSchema(string procedureName) {
			using(IDbCommand command = CreateCommand()) {
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
	public class DataAccessMySqlProviderFactory : MySqlProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessMySqlConnectionProvider.DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessMySqlConnectionProvider.DataAccessCreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(DataAccessConnectionParameter.PortParamID))
				return base.GetConnectionString(parameters);
			if(!parameters.ContainsKey(ServerParamID) || !parameters.ContainsKey(DatabaseParamID))
				return null;
			if(!parameters.ContainsKey(UserIDParamID) || !parameters.ContainsKey(PasswordParamID))
				return null;
			string server = parameters[ServerParamID];
			string userid = parameters[UserIDParamID];
			string password = parameters[PasswordParamID];
			string database = parameters[DatabaseParamID];
			string port = parameters[DataAccessConnectionParameter.PortParamID];
			return DataAccessMySqlConnectionProvider.GetConnectionString(server, userid, password, database, port);
		}
		public override string[] GetDatabases(string server, string userid, string password) {
			return GetDatabases(server, string.Empty, userid, password);
		}
		public string[] GetDatabases(string server, string port, string userid, string password) {
			string connectionString = DataAccessMySqlConnectionProvider.GetConnectionString(server, userid, password, string.Empty, port);
			Xpo.DB.Helpers.ConnectionStringParser helper = new Xpo.DB.Helpers.ConnectionStringParser(connectionString);
			helper.RemovePartByName("database");
			helper.RemovePartByName(DataStoreBase.XpoProviderTypeParameterName);
			string connectToServer = helper.GetConnectionString();
			using(IDbConnection conn = MySqlConnectionProvider.CreateConnection(connectToServer)) {
				try {
					conn.Open();
					using(IDbCommand comm = conn.CreateCommand()) {
						comm.CommandText = "show databases";
						IDataReader reader = comm.ExecuteReader();
						List<string> result = new List<string>();
						while(reader.Read())
							result.Add((string)reader.GetValue(0));
						return result.ToArray();
					}
				} catch {
					return new string[0];
				}
			}
		}
	}
}
