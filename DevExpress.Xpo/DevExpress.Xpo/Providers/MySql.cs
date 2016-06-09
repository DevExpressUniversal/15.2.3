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

namespace DevExpress.Xpo.DB {
	using System;
	using System.Data;
	using System.Text;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Diagnostics;
	using System.Globalization;
	using DevExpress.Xpo.DB;
	using DevExpress.Xpo.DB.Helpers;
	using DevExpress.Data.Helpers;
	using System.Text.RegularExpressions;
	using DevExpress.Data.Filtering;
	using DevExpress.Xpo.DB.Exceptions;
	using DevExpress.Xpo;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Data.Common;
	public class MySqlConnectionProvider : ConnectionProviderSql {
		public const string XpoProviderTypeString = "MySql";
		ReflectConnectionHelper helper;
		ReflectConnectionHelper ConnectionHelper {
			get {
				if(helper == null)
					helper = new ReflectConnectionHelper(Connection, "MySql.Data.MySqlClient.MySqlException");
				return helper;
			}
		}
		public static string GetConnectionString(string server, string userid, string password, string database) {
			return String.Format("{4}={5};server={0};user id={1}; password={2}; database={3};persist security info=true;CharSet=utf8;", server, userid, password, database, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString);
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] { connection };
			return CreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new MySqlConnectionProvider(connection, autoCreateOption);
		}
		static MySqlConnectionProvider() {
			RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
			RegisterDataStoreProvider("MySql.Data.MySqlClient.MySqlConnection", new DataStoreCreationFromConnectionDelegate(CreateProviderFromConnection));
			RegisterFactory(new MySqlProviderFactory());
		}
		public static void Register() { }
		public MySqlConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
			PrepareDelegates();
		}
		protected override string GetSqlCreateColumnTypeForBoolean(DBTable table, DBColumn column) {
			return "bit";
		}
		protected override string GetSqlCreateColumnTypeForByte(DBTable table, DBColumn column) {
			return "tinyint unsigned";
		}
		protected override string GetSqlCreateColumnTypeForSByte(DBTable table, DBColumn column) {
			return "tinyint";
		}
		protected override string GetSqlCreateColumnTypeForChar(DBTable table, DBColumn column) {
			return "char";
		}
		protected override string GetSqlCreateColumnTypeForDecimal(DBTable table, DBColumn column) {
			return "double";
		}
		protected override string GetSqlCreateColumnTypeForDouble(DBTable table, DBColumn column) {
			return "double";
		}
		protected override string GetSqlCreateColumnTypeForSingle(DBTable table, DBColumn column) {
			return "real";
		}
		protected override string GetSqlCreateColumnTypeForInt32(DBTable table, DBColumn column) {
			return "int";
		}
		protected override string GetSqlCreateColumnTypeForUInt32(DBTable table, DBColumn column) {
			return "int unsigned";
		}
		protected override string GetSqlCreateColumnTypeForInt16(DBTable table, DBColumn column) {
			return "smallint";
		}
		protected override string GetSqlCreateColumnTypeForUInt16(DBTable table, DBColumn column) {
			return "smallint unsigned";
		}
		protected override string GetSqlCreateColumnTypeForInt64(DBTable table, DBColumn column) {
			return "bigint";
		}
		protected override string GetSqlCreateColumnTypeForUInt64(DBTable table, DBColumn column) {
			return "bigint unsigned";
		}
		public const int MaximumStringSize = 255;
		protected override string GetSqlCreateColumnTypeForString(DBTable table, DBColumn column) {
			if(column.Size < 0 || column.Size > 16777215)
				return "LONGTEXT";
			if(column.Size > 65535)
				return "MEDIUMTEXT";
			if(column.Size > 255)
				return "TEXT";
			return "varchar(" + column.Size.ToString(CultureInfo.InvariantCulture) + ')';
		}
		protected override string GetSqlCreateColumnTypeForDateTime(DBTable table, DBColumn column) {
			return "datetime";
		}
		protected override string GetSqlCreateColumnTypeForGuid(DBTable table, DBColumn column) {
			return "char(38)";
		}
		protected override string GetSqlCreateColumnTypeForByteArray(DBTable table, DBColumn column) {
			if(column.Size <= 0 || column.Size > 16777215)
				return "LONGBLOB";
			if(column.Size > 65535)
				return "MEDIUMBLOB";
			if(column.Size > 127)
				return "BLOB";
			return "TINYBLOB";
		}
		public override string GetSqlCreateColumnFullAttributes(DBTable table, DBColumn column) {
			string result = GetSqlCreateColumnType(table, column);
			if(column.ColumnType == DBColumnType.Boolean)
				return result;
			if(column.IsKey)
				result += " NOT NULL";
			else
				result += " NULL";
			if(column.IsKey && column.IsIdentity && (column.ColumnType == DBColumnType.Int32 || column.ColumnType == DBColumnType.Int64) && IsSingleColumnPKColumn(table, column))
				result += " AUTO_INCREMENT PRIMARY KEY";
			return result;
		}
		protected override Int64 GetIdentity(Query sql) {
			object value = GetScalar(new Query(sql.Sql + ";\nselect last_insert_id()", sql.Parameters, sql.ParametersNames));
			return (value as IConvertible).ToInt64(CultureInfo.InvariantCulture);
		}
		MethodInfo miServerVersion;
		string GetMySqlServerVersion(IDbConnection conn) {
			if(miServerVersion == null) {
				Type connType = conn.GetType();
				if(ConnectionHelper.ConnectionType.IsInstanceOfType(conn)) {
					PropertyInfo pi = connType.GetProperty("ServerVersion", BindingFlags.Instance | BindingFlags.Public);
					if(pi != null && pi.CanRead) {
						miServerVersion = pi.GetGetMethod();
					}
				}
			}
			if(miServerVersion == null)
				throw new InvalidOperationException("MySql server version not found.");
			return (string)miServerVersion.Invoke(conn, new object[0]);
		}
		protected override Exception WrapException(Exception e, IDbCommand query) {
			object o;
			if(ConnectionHelper.TryGetExceptionProperty(e, "Number", out o)) {
				int number = (int)o;
				if(number == 0x41e || number == 0x47a)
					return new SchemaCorrectionNeededException(e);
				if(number == 0x5ab || number == 0x426)
					return new ConstraintViolationException(query.CommandText, GetParametersString(query), e);
			}
			return base.WrapException(e, query);
		}
		protected override IDbConnection CreateConnection() {
			return ConnectionHelper.GetConnection(ConnectionString);
		}
		public static IDbConnection CreateConnection(string connectionString) {
			return ReflectConnectionHelper.GetConnection("MySql.Data", "MySql.Data.MySqlClient.MySqlConnection", connectionString);
		}
		bool is5;
		protected override void CreateDataBase() {
			try {
				Connection.Open();
			} catch(Exception e) {
				object o;
				bool gotDatabaseNotFoundException = false; 
				Exception currentException = e;
				while(currentException != null){
					if(ConnectionHelper.TryGetExceptionProperty(currentException, "Number", out o) && ((int)o) == 1049){
						gotDatabaseNotFoundException = true;
						break;
					}
					currentException = currentException.InnerException;
				}
				ConnectionStringParser helper = new ConnectionStringParser(ConnectionString);
				if(gotDatabaseNotFoundException && CanCreateDatabase) {
					string dbName = helper.GetPartByName("database");
					helper.RemovePartByName("database");
					string connectToServer = helper.GetConnectionString();
					using(IDbConnection conn = CreateConnection(connectToServer)) {
						conn.Open();
						using(IDbCommand c = conn.CreateCommand()) {
							c.CommandText = "Create Database " + dbName;
							c.ExecuteNonQuery();
						}
					}
					Connection.Open();
				}else{
					throw new UnableToOpenDatabaseException(XpoDefault.ConnectionStringRemovePassword(helper), e);
				}
			}
			is5 = !GetMySqlServerVersion(Connection).StartsWith("4.");
		}
		delegate bool TablesFilter(DBTable table);
		SelectStatementResult GetDataForTables(ICollection tables, TablesFilter filter, string queryText) {
			QueryParameterCollection parameters = new QueryParameterCollection();
			StringCollection inList = new StringCollection();
			int i = 0;
			foreach(DBTable table in tables) {
				if(filter == null || filter(table)) {
					parameters.Add(new OperandValue(ComposeSafeTableName(table.Name)));
					inList.Add("@table" + i.ToString(CultureInfo.InvariantCulture));
					++i;
				}
			}
			if(inList.Count == 0)
				return new SelectStatementResult();
			return SelectData(new Query(string.Format(CultureInfo.InvariantCulture, queryText, StringListHelper.DelimitedText(inList, ",")), parameters, inList));
		}
		DBColumnType GetTypeFromString(string typeName, out int size) {
			size = 0;
			if(string.IsNullOrEmpty(typeName))
				return DBColumnType.Unknown;
			switch(typeName) {
				case "char(1)":
					return DBColumnType.Char;
				case "tinyint(1)":
					return DBColumnType.Boolean;
				default:
					break;
			}
			string typeWithoutBrackets = RemoveBrackets(typeName);
			switch(typeWithoutBrackets.ToLowerInvariant()) {
				case "int":
					return DBColumnType.Int32;
				case "int unsigned":
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
				case "float":
					return DBColumnType.Single;
				case "datetime":
				case "date":
					return DBColumnType.DateTime;
				case "decimal":
					return DBColumnType.Decimal;
				default:
					break;
			}
			if(typeName.StartsWith("char(")) {
				size = Int32.Parse(typeName.Substring(5, typeName.Length - 6));
				return DBColumnType.String;
			}
			if(typeName.StartsWith("varchar(")) {
				size = Int32.Parse(typeName.Substring(8, typeName.Length - 9));
				return DBColumnType.String;
			}
			return DBColumnType.Unknown;
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
		void GetColumns(DBTable table) {
			foreach(SelectStatementResultRow row in SelectData(new Query(string.Format(CultureInfo.InvariantCulture, "show columns from `{0}`", ComposeSafeTableName(table.Name)))).Rows) {
				int size;
				string rowValue1, rowValue5, rowValue0 = string.Empty;
				if(row.Values[1].GetType() == typeof(System.Byte[])) {
					rowValue1 = System.Text.Encoding.Default.GetString((byte[])row.Values[1]);
					rowValue5 = System.Text.Encoding.Default.GetString((byte[])row.Values[5]);
					rowValue0 = System.Text.Encoding.Default.GetString((byte[])row.Values[0]);
				} else {
					rowValue1 = (string)row.Values[1];
					rowValue5 = (string)row.Values[5];
					rowValue0 = (string)row.Values[0];
				}
				DBColumnType type = GetTypeFromString(rowValue1, out size);
				bool isAutoIncrement = false;
				string extraValue = rowValue5;
				if(!string.IsNullOrEmpty(extraValue) && extraValue.Contains("auto_increment"))
					isAutoIncrement = true;
				DBColumn column = new DBColumn(rowValue0, false, String.Empty, type == DBColumnType.String ? size : 0, type);
				column.IsIdentity = isAutoIncrement;
				table.AddColumn(column);
			}
		}
		void GetPrimaryKey(DBTable table) {
			SelectStatementResult data = SelectData(new Query(string.Format(CultureInfo.InvariantCulture, "show index from `{0}`", ComposeSafeTableName(table.Name))));
			if(data.Rows.Length > 0) {
				StringCollection cols = new StringCollection();
				for(int i = 0; i < data.Rows.Length; i++) {
					object[] topRow = data.Rows[i].Values;
					string topRow2, topRow4 = string.Empty;
					if(topRow[2].GetType() == typeof(System.Byte[])) {
						topRow2 = System.Text.Encoding.Default.GetString((byte[])topRow[2]);
						topRow4 = System.Text.Encoding.Default.GetString((byte[])topRow[4]);
					} else {
						topRow2 = (string)topRow[2];
						topRow4 = (string)topRow[4];
					}
					if(topRow2 == "PRIMARY") {
						DBColumn column = table.GetColumn(topRow4);
						if(column != null)
							column.IsKey = true;
						cols.Add(topRow4);
					}
				}
				if(cols.Count > 0) {
					table.PrimaryKey = new DBPrimaryKey(cols);
				}
			}
		}
		void GetIndexes(DBTable table) {
			SelectStatementResult data = SelectData(new Query(string.Format(CultureInfo.InvariantCulture, "show index from `{0}`", ComposeSafeTableName(table.Name))));
			DBIndex index = null;
			foreach(SelectStatementResultRow row in data.Rows) {
				string rowValues2, rowValues4 = string.Empty;
				int nonUnique = Convert.ToInt32(row.Values[1]);
				if(row.Values[2].GetType() == typeof(System.Byte[])) {
					rowValues2 = System.Text.Encoding.Default.GetString((byte[])row.Values[2]);
					rowValues4 = System.Text.Encoding.Default.GetString((byte[])row.Values[4]);
				} else {
					rowValues2 = (string)row.Values[2];
					rowValues4 = (string)row.Values[4];
				}
				if(index == null || index.Name != rowValues2) {
					StringCollection list = new StringCollection();
					list.Add(rowValues4);
					index = new DBIndex(rowValues2, list, nonUnique == 0);
					table.Indexes.Add(index);
				} else
					index.Columns.Add(rowValues4);
			}
		}
		void GetForeignKeys(DBTable table) {
			SelectStatementResult data = SelectData(new Query(string.Format(CultureInfo.InvariantCulture, "show create table `{0}`", ComposeSafeTableName(table.Name))));
			if(data.Rows.Length > 0) {
				object val = data.Rows[0].Values[1];
				string s = val as string;
				if(s == null)
					s = System.Text.Encoding.Default.GetString((byte[])val);
				int pos = 0;
				do {
					pos = s.IndexOf("CONSTRAINT", pos + 1);
					if(pos == -1)
						break;
					int colsIndex = s.IndexOf("FOREIGN KEY", pos);
					int refsIndex = s.IndexOf("REFERENCES", pos);
					if(colsIndex < 0 || refsIndex < 0)
						break;
					int primesIndex = s.IndexOf("(", refsIndex);
					int primesEndIndex = s.IndexOf(")", primesIndex);
					if(primesIndex < 0 || primesEndIndex < 0)
						break;
					string refTable = s.Substring(refsIndex + 12, primesIndex - 12 - refsIndex).Trim('`', ' ');
					string refs = s.Substring(colsIndex + 12, refsIndex - 12 - colsIndex).Trim('`', ' ', '(', ')');
					StringCollection cols = new StringCollection();
					foreach(string col in refs.Split(','))
						cols.Add(col.Trim('`', ' '));
					refs = s.Substring(primesIndex, primesEndIndex - primesIndex).Trim(' ', '(', ')');
					StringCollection fcols = new StringCollection();
					foreach(string col in refs.Split(','))
						fcols.Add(col.Trim('`', ' '));
					table.AddForeignKey(new DBForeignKey(cols, refTable, fcols));
				} while(true);
			}
		}
		public override void GetTableSchema(DBTable table, bool checkIndexes, bool checkForeignKeys) {
			GetColumns(table);
			GetPrimaryKey(table);
			if(checkIndexes)
				GetIndexes(table);
			if(checkForeignKeys)
				GetForeignKeys(table);
		}
		public override ICollection CollectTablesToCreate(ICollection tables) {
			if(is5) {
				Hashtable dbTables = new Hashtable();
				foreach(SelectStatementResultRow row in SelectData(new Query(String.Format(CultureInfo.InvariantCulture, "select TABLE_NAME, TABLE_TYPE from INFORMATION_SCHEMA.TABLES where table_schema = '{0}' and TABLE_TYPE in ('BASE TABLE', 'VIEW')", Connection.Database))).Rows) {
					string rowValues0, rowValues1 = string.Empty;
					if(row.Values[0].GetType() == typeof(System.Byte[])) {
						rowValues0 = System.Text.Encoding.Default.GetString((byte[])row.Values[0]);
						rowValues1 = System.Text.Encoding.Default.GetString((byte[])row.Values[1]);
					} else {
						rowValues0 = (string)row.Values[0];
						rowValues1 = (string)row.Values[1];
					}
					dbTables.Add(rowValues0.ToLower(), rowValues1 == "VIEW");
				}
				ArrayList list = new ArrayList();
				foreach(DBTable table in tables) {
					object o = dbTables[ComposeSafeTableName(table.Name).ToLower()];
					if(o == null)
						list.Add(table);
					else
						table.IsView = (bool)o;
				}
				return list;
			} else {
				Hashtable dbTables = new Hashtable();
				foreach(SelectStatementResultRow row in SelectData(new Query(String.Format(CultureInfo.InvariantCulture, "show tables from `{0}`", Connection.Database))).Rows) {
					string rowValues0, rowValues1 = string.Empty;
					if(row.Values[0].GetType() == typeof(System.Byte[])) {
						rowValues0 = System.Text.Encoding.Default.GetString((byte[])row.Values[0]);
					} else {
						rowValues0 = (string)row.Values[0];
					}
					dbTables.Add(rowValues0.ToLower(), null);
				}
				ArrayList list = new ArrayList();
				foreach(DBTable table in tables)
					if(!dbTables.Contains(ComposeSafeTableName(table.Name).ToLower()))
						list.Add(table);
				return list;
			}
		}
		ExecMethodDelegate commandBuilderDeriveParametersHandler;
		protected override void CommandBuilderDeriveParameters(IDbCommand command) {
			if(commandBuilderDeriveParametersHandler == null) {
				commandBuilderDeriveParametersHandler = ReflectConnectionHelper.GetCommandBuilderDeriveParametersDelegate("MySql.Data", "MySql.Data.MySqlClient.MySqlCommandBuilder");
			}
			commandBuilderDeriveParametersHandler(command);
		}
		object[] mySqlByteArrayTypes;
		SetPropertyValueDelegate setMySqlDbType;
		GetPropertyValueDelegate getMySqlDbType;
		protected virtual void PrepareDelegates() {
			Type mySqlParameterType = ConnectionHelper.GetType("MySql.Data.MySqlClient.MySqlParameter");
			Type mySqlDbTypeType = ConnectionHelper.GetType("MySql.Data.MySqlClient.MySqlDbType");
			mySqlByteArrayTypes = new object[] {
				Enum.Parse(mySqlDbTypeType, "TinyBlob"),
				Enum.Parse(mySqlDbTypeType, "MediumBlob"),
				Enum.Parse(mySqlDbTypeType, "LongBlob"),
				Enum.Parse(mySqlDbTypeType, "Blob")
			};
			ReflectConnectionHelper.CreatePropertyDelegates(mySqlParameterType, "MySqlDbType", out setMySqlDbType, out getMySqlDbType);
		}
		public override DBStoredProcedure[] GetStoredProcedures() {
			List<DBStoredProcedure> result = new List<DBStoredProcedure>();
			IDbCommand command = Connection.CreateCommand();
			command.CommandText = @"select name from mysql.proc where db = database()";
			IDataReader reader = command.ExecuteReader();
			while(reader.Read()) {
				DBStoredProcedure curProc = new DBStoredProcedure();
				curProc.Name = reader.GetString(0);
				result.Add(curProc);
			}
			reader.Close();
			command.CommandType = CommandType.StoredProcedure;
			foreach(DBStoredProcedure sproc in result) {
				try {
					command.CommandText = sproc.Name;
					CommandBuilderDeriveParameters(command);
					List<string> fakeParams = new List<string>();
					List<DBStoredProcedureArgument> dbArguments = new List<DBStoredProcedureArgument>();
					foreach(IDataParameter parameter in command.Parameters) {
						DBStoredProcedureArgumentDirection direction = DBStoredProcedureArgumentDirection.In;
						if(parameter.Direction == ParameterDirection.InputOutput) {
							direction = DBStoredProcedureArgumentDirection.InOut;
						}
						if(parameter.Direction == ParameterDirection.Output) {
							direction = DBStoredProcedureArgumentDirection.Out;
						}
						DBColumnType columnType = GetColumnType(parameter.DbType, true);
						if(columnType == DBColumnType.Unknown && mySqlByteArrayTypes != null) {
							object mySqlDbType = getMySqlDbType(parameter);
							foreach(object mySqlByteArrayType in mySqlByteArrayTypes) {
								if(object.Equals(mySqlByteArrayType, mySqlDbType)) {
									columnType = DBColumnType.ByteArray;
								}
							}
						}
						dbArguments.Add(new DBStoredProcedureArgument(parameter.ParameterName, columnType, direction));
						fakeParams.Add("null");
					}
					sproc.Arguments.AddRange(dbArguments);
				} catch {
					if(!reader.IsClosed) {
						reader.Close();
					}
				}
			}
			return result.ToArray();
		}
		protected override int GetSafeNameTableMaxLength() {
			return 64;
		}
		public override string FormatTable(string schema, string tableName) {
			return string.Format(CultureInfo.InvariantCulture, "`{0}`", tableName);
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "`{0}` {1}", tableName, tableAlias);
		}
		public override string FormatColumn(string columnName) {
			return string.Format(CultureInfo.InvariantCulture, "`{0}`", columnName);
		}
		public override string FormatColumn(string columnName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "{1}.`{0}`", columnName, tableAlias);
		}
		public override string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int skipSelectedRecords, int topSelectedRecords) {
			base.FormatSelect(selectedPropertiesSql, fromSql, whereSql, orderBySql, groupBySql, havingSql, skipSelectedRecords, topSelectedRecords);
			string modificatorsSql = string.Empty;
			if(skipSelectedRecords == 0) {
				modificatorsSql = string.Format(CultureInfo.InvariantCulture, (topSelectedRecords != 0) ? "limit {0} " : string.Empty, topSelectedRecords);
			} else {
				int topSelectedRecordsValue = topSelectedRecords == 0 ? int.MaxValue : topSelectedRecords;
				modificatorsSql = string.Format(CultureInfo.InvariantCulture, "limit {0}, {1} ", skipSelectedRecords, topSelectedRecordsValue);
			}
			string expandedWhereSql = whereSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}where {1}", Environment.NewLine, whereSql) : string.Empty;
			string expandedOrderBySql = orderBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}order by {1}", Environment.NewLine, orderBySql) : string.Empty;
			string expandedHavingSql = havingSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}having {1}", Environment.NewLine, havingSql) : string.Empty;
			string expandedGroupBySql = groupBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}group by {1}", Environment.NewLine, groupBySql) : string.Empty;
			return string.Format(CultureInfo.InvariantCulture, "select {1} from {2}{3}{4}{5}{6} {0}", modificatorsSql, selectedPropertiesSql, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedOrderBySql);
		}
		public override bool NativeSkipTakeSupported { get { return true; } }
		public override string FormatInsertDefaultValues(string tableName) {
			return string.Format(CultureInfo.InvariantCulture, "insert into {0} values()", tableName);
		}
		public override string FormatInsert(string tableName, string fields, string values) {
			return string.Format(CultureInfo.InvariantCulture, "insert into {0}({1})values({2})",
				tableName, fields, values);
		}
		public override string FormatUpdate(string tableName, string sets, string whereClause) {
			return string.Format(CultureInfo.InvariantCulture, "update {0} set {1} where {2}",
				tableName, sets, whereClause);
		}
		public override string FormatDelete(string tableName, string whereClause) {
			return string.Format(CultureInfo.InvariantCulture, "delete from {0} where {1}", tableName, whereClause);
		}
		static string FormatMod(string arg, int multiplier, int divider) {
			return string.Format("(Truncate(Cast({0} as decimal(65,30)) * {1}, 0) % {2})", arg, multiplier, divider);
		}
		static string FormatGetInt(string arg, int multiplier, int divider) {
			return string.Format("(Cast({0} as decimal(65,30)) * {1} Div {2})", arg, multiplier, divider);
		}
		static string FnAddDateTime(string datetimeOperand, string dayPart, string secondPart) {
			return string.Format(CultureInfo.InvariantCulture, "cast(AddDate(AddDate({0}, interval {1} day), interval {2} second) as datetime)", datetimeOperand, dayPart, secondPart);
		}
		public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.Concat:
					string args = String.Empty;
					for(int i = 0; i < operands.Length; i++) {
						if(operands[i].Length > 0)
							args += i == operands.Length - 1 ? string.Format(CultureInfo.InvariantCulture, "{0}", operands[i]) : string.Format(CultureInfo.InvariantCulture, "{0}, ", operands[i]);
					}
					return string.Format(CultureInfo.InvariantCulture, "CONCAT({0})", args);
				case FunctionOperatorType.Len:
					return string.Format(CultureInfo.InvariantCulture, "LENGTH({0})", operands[0]);
				case FunctionOperatorType.Substring:
					return operands.Length < 3 ? string.Format(CultureInfo.InvariantCulture, "SUBSTR({0},{1} + 1)", operands[0], operands[1]) : string.Format(CultureInfo.InvariantCulture, "SUBSTR({0}, {1} + 1, {2})", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Trim:
					return string.Format(CultureInfo.InvariantCulture, "Trim({0})", operands[0]);
				case FunctionOperatorType.Upper:
					return string.Format(CultureInfo.InvariantCulture, "Upper({0})", operands[0]);
				case FunctionOperatorType.Lower:
					return string.Format(CultureInfo.InvariantCulture, "Lower({0})", operands[0]);
				case FunctionOperatorType.Ascii:
					return string.Format(CultureInfo.InvariantCulture, "Ascii({0})", operands[0]);
				case FunctionOperatorType.Char:
					return string.Format(CultureInfo.InvariantCulture, "cast(Char({0}) as char(1))", operands[0]);
				case FunctionOperatorType.ToInt:
					return string.Format(CultureInfo.InvariantCulture, "Cast({0} as signed)", operands[0]);
				case FunctionOperatorType.ToLong:
					return string.Format(CultureInfo.InvariantCulture, "Cast({0} as decimal(20, 0))", operands[0]);
				case FunctionOperatorType.ToFloat:
					throw new NotSupportedException();
				case FunctionOperatorType.ToDouble:
					throw new NotSupportedException();
				case FunctionOperatorType.ToDecimal:
					return string.Format(CultureInfo.InvariantCulture, "Cast({0} as decimal(65,30))", operands[0]);
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, "Cast({0} as char)", operands[0]);
				case FunctionOperatorType.Replace:
					return string.Format(CultureInfo.InvariantCulture, "Replace({0},{1},{2})", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.PadLeft:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "If(Length({0}) >= {1}, {0}, LPad({0}, {1}, ' '))", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "If(Length({0}) >= {1}, {0}, LPad({0}, {1}, {2}))", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.PadRight:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "If(Length({0}) >= {1}, {0},RPad({0}, {1}, ' '))", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "If(Length({0}) >= {1}, {0},RPad({0}, {1}, {2}))", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.Reverse:
					return string.Format(CultureInfo.InvariantCulture, "Reverse({0})", operands[0]);
				case FunctionOperatorType.Insert:
					return string.Format(CultureInfo.InvariantCulture, "Concat(Left({0},{1}),{2},Right({0},Length({0})-({1})))", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.CharIndex:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(Instr({1},{0})-1)", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "( if( Instr(Right({1},(Length({1}) - {2})),{0}) = 0, -1 , Instr(Right({1},(Length({1}) - {2})),{0}) -1 + {2}) )", operands[0], operands[1], operands[2]);
						case 4:
							return string.Format(CultureInfo.InvariantCulture, "(if( instr(Left( Right({1},(Length({1}) - {2} ) ) ,{3}),{0}) = 0, -1, instr(Left( Right({1},(Length({1}) - {2} ) ) ,{3}),{0}) - 1 + {2}))", operands[0], operands[1], operands[2], operands[3]);
					}
					goto default;
				case FunctionOperatorType.Remove:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "Left({0},{1})", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "Concat(Left({0},{1}),Right({0},Length({0})-{1}-{2}))", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.Abs:
					return string.Format(CultureInfo.InvariantCulture, "Abs({0})", operands[0]);
				case FunctionOperatorType.BigMul:
					return string.Format(CultureInfo.InvariantCulture, "(cast({0} as signed int) * cast({1} as signed int))", operands[0], operands[1]);
				case FunctionOperatorType.Sqr:
					return string.Format(CultureInfo.InvariantCulture, "Sqrt({0})", operands[0]);
				case FunctionOperatorType.Sinh:
					return string.Format(CultureInfo.InvariantCulture, "( (Exp({0}) - Exp(({0} * (-1) ))) / 2 )", operands[0]);
				case FunctionOperatorType.Cosh:
					return string.Format(CultureInfo.InvariantCulture, "( (Exp({0}) + Exp(({0} * (-1) ))) / 2 )", operands[0]);
				case FunctionOperatorType.Tanh:
					return string.Format(CultureInfo.InvariantCulture, "( (Exp({0}) - Exp(({0} * (-1) ))) / (Exp({0}) + Exp(({0} * (-1) ))) )", operands[0]);
				case FunctionOperatorType.Rnd:
					return "Rand()";
				case FunctionOperatorType.Log:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "Log({0})", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "Log({1},{0})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Log10:
					return string.Format(CultureInfo.InvariantCulture, "Log10({0})", operands[0]);
				case FunctionOperatorType.Sin:
					return string.Format(CultureInfo.InvariantCulture, "Sin({0})", operands[0]);
				case FunctionOperatorType.Asin:
					return string.Format(CultureInfo.InvariantCulture, "Asin({0})", operands[0]);
				case FunctionOperatorType.Tan:
					return string.Format(CultureInfo.InvariantCulture, "Tan({0})", operands[0]);
				case FunctionOperatorType.Atn:
					return string.Format(CultureInfo.InvariantCulture, "Atan({0})", operands[0]);
				case FunctionOperatorType.Atn2:
					return string.Format(CultureInfo.InvariantCulture, "Atan2({0},{1})", operands[0], operands[1]);
				case FunctionOperatorType.Cos:
					return string.Format(CultureInfo.InvariantCulture, "Cos({0})", operands[0]);
				case FunctionOperatorType.Acos:
					return string.Format(CultureInfo.InvariantCulture, "Acos({0})", operands[0]);
				case FunctionOperatorType.Exp:
					return string.Format(CultureInfo.InvariantCulture, "Exp({0})", operands[0]);
				case FunctionOperatorType.Power:
					return string.Format(CultureInfo.InvariantCulture, "Power({0},{1})", operands[0], operands[1]);
				case FunctionOperatorType.Iif: {
						if(operands.Length < 3 || (operands.Length % 2) == 0)
							throw new ArgumentException(Res.GetString(Res.Filtering_TheIifFunctionOperatorRequiresThree));
						if(operands.Length == 3)
							return string.Format(CultureInfo.InvariantCulture, "If({0}, {1}, {2})", operands[0], operands[1], operands[2]);
						StringBuilder sb = new StringBuilder();
						int index = -2;
						int counter = 0;
						do {
							index += 2;
							sb.AppendFormat("If({0}, {1}, ", operands[index], operands[index + 1]);
							counter++;
						} while((index + 3) < operands.Length);
						sb.AppendFormat("{0}", operands[index + 2]);
						sb.Append(new string(')', counter));
						return sb.ToString();
					}
				case FunctionOperatorType.Max:
					return string.Format(CultureInfo.InvariantCulture, "if({0} > {1}, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Min:
					return string.Format(CultureInfo.InvariantCulture, "if({0} < {1}, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Round:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "Round({0},0)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "Round({0},{1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Sign:
					return string.Format(CultureInfo.InvariantCulture, "Sign({0})", operands[0]);
				case FunctionOperatorType.Floor:
					return string.Format(CultureInfo.InvariantCulture, "Floor({0})", operands[0]);
				case FunctionOperatorType.Ceiling:
					return string.Format(CultureInfo.InvariantCulture, "Ceiling({0})", operands[0]);
				case FunctionOperatorType.IsNull:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "({0} is null)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "Coalesce({0},{1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) is null or length({0}) = 0)", operands[0]);
				case FunctionOperatorType.EndsWith:
					return string.Format(CultureInfo.InvariantCulture, "(Right({0}, Length({1})) = ({1}))", operands[0], operands[1]);
				case FunctionOperatorType.Contains:
					return string.Format(CultureInfo.InvariantCulture, "(Instr({0}, {1}) > 0)", operands[0], operands[1]);
				case FunctionOperatorType.GetMilliSecond:
					throw new NotSupportedException();
				case FunctionOperatorType.GetSecond:
					return string.Format(CultureInfo.InvariantCulture, "Second({0})", operands[0]);
				case FunctionOperatorType.GetMinute:
					return string.Format(CultureInfo.InvariantCulture, "Minute({0})", operands[0]);
				case FunctionOperatorType.GetHour:
					return string.Format(CultureInfo.InvariantCulture, "Hour({0})", operands[0]);
				case FunctionOperatorType.GetDay:
					return string.Format(CultureInfo.InvariantCulture, "Day({0})", operands[0]);
				case FunctionOperatorType.GetMonth:
					return string.Format(CultureInfo.InvariantCulture, "Month({0})", operands[0]);
				case FunctionOperatorType.GetYear:
					return string.Format(CultureInfo.InvariantCulture, "Year({0})", operands[0]);
				case FunctionOperatorType.GetTimeOfDay:
					return string.Format(CultureInfo.InvariantCulture, "((Hour({0})) * 36000000000) + ((Minute({0})) * 600000000) + (Second({0}) * 10000000)", operands[0]);
				case FunctionOperatorType.GetDayOfWeek:
					return string.Format(CultureInfo.InvariantCulture, "((DayOfWeek({0})- DayOfWeek('1900-01-01')  + 8) % 7)", operands[0]);
				case FunctionOperatorType.GetDayOfYear:
					return string.Format(CultureInfo.InvariantCulture, "DayOfYear({0})", operands[0]);
				case FunctionOperatorType.GetDate:
					return string.Format(CultureInfo.InvariantCulture, "Date({0})", operands[0]);
				case FunctionOperatorType.AddTicks:
					return string.Format(CultureInfo.InvariantCulture, "cast(Adddate({0}, interval ({1} div 10000000 ) second )  as datetime)", operands[0], operands[1]);
				case FunctionOperatorType.AddMilliSeconds:
					return string.Format(CultureInfo.InvariantCulture, "cast(Adddate({0}, interval ( {1} div 1000) second )  as datetime)", operands[0], operands[1]);
				case FunctionOperatorType.AddTimeSpan:
				case FunctionOperatorType.AddSeconds:
					return FnAddDateTime(operands[0], FormatGetInt(operands[1], 1, 86400), FormatMod(operands[1], 1, 86400));
				case FunctionOperatorType.AddMinutes:
					return FnAddDateTime(operands[0], FormatGetInt(operands[1], 60, 86400), FormatMod(operands[1], 60, 86400));
				case FunctionOperatorType.AddHours:
					return FnAddDateTime(operands[0], FormatGetInt(operands[1], 3600, 86400), FormatMod(operands[1], 3600, 86400));
				case FunctionOperatorType.AddDays:
					return FnAddDateTime(operands[0], FormatGetInt(operands[1], 86400, 86400), FormatMod(operands[1], 86400, 86400));
				case FunctionOperatorType.AddMonths:
					return string.Format(CultureInfo.InvariantCulture, "cast(Adddate({0}, interval {1} month )  as datetime)", operands[0], operands[1]);
				case FunctionOperatorType.AddYears:
					return string.Format(CultureInfo.InvariantCulture, "cast(Adddate({0}, interval {1} year )  as datetime)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffYear:
					return string.Format(CultureInfo.InvariantCulture, "(Year({1}) - Year({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMonth:
					return string.Format(CultureInfo.InvariantCulture, "(((Year({1}) - Year({0})) * 12) + Month({1}) - Month({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffDay:
					return string.Format(CultureInfo.InvariantCulture, is5 ? "DATEDIFF({1}, {0})" : "(TO_DAYS({1}) - TO_DAYS({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffHour:
					return string.Format(CultureInfo.InvariantCulture, "((" + (is5 ? "DATEDIFF({1}, {0})" : "(TO_DAYS({1}) - TO_DAYS({0}))") + " * 24) + Hour({1}) - Hour({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMinute:
					return string.Format(CultureInfo.InvariantCulture, "((((" + (is5 ? "DATEDIFF({1}, {0})" : "(TO_DAYS({1}) - TO_DAYS({0}))") + " * 24) + Hour({1}) - Hour({0})) * 60) + Minute({1}) - Minute({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffSecond:
					return string.Format(CultureInfo.InvariantCulture, "(UNIX_TIMESTAMP({1}) - UNIX_TIMESTAMP({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "((UNIX_TIMESTAMP({1}) - UNIX_TIMESTAMP({0})) * 1000)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffTick:
					return string.Format(CultureInfo.InvariantCulture, "((UNIX_TIMESTAMP({1}) - UNIX_TIMESTAMP({0}))) * 10000000", operands[0], operands[1]);
				case FunctionOperatorType.Now:
					return "Now()";
				case FunctionOperatorType.UtcNow:
					return "UTC_TIMESTAMP()";
				case FunctionOperatorType.Today:
					return "CurDate()";
				default:
					return base.FormatFunction(operatorType, operands);
			}
		}
		readonly static char[] achtungChars = new char[] { '_', '%' };
		public override string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.StartsWith:
					object secondOperand = operands[1];
					if(secondOperand is OperandValue && ((OperandValue)secondOperand).Value is string) {
						string operandString = (string)((OperandValue)secondOperand).Value;
						int likeIndex = operandString.IndexOfAny(achtungChars);
						if(likeIndex < 0) {
							return string.Format(CultureInfo.InvariantCulture, "({0} like {1})", processParameter(operands[0]), processParameter(new ConstantValue(operandString + "%")));
						} else if(likeIndex > 0) {
							return string.Format(CultureInfo.InvariantCulture, "(({0} like {2}) And (Left({0}, Length({1})) = ({1})))", processParameter(operands[0]), processParameter(secondOperand), processParameter(new ConstantValue(operandString.Substring(0, likeIndex) + "%")));
						}
					}
					return string.Format(CultureInfo.InvariantCulture, "(Left({0}, Length({1})) = ({1}))", processParameter(operands[0]), processParameter(operands[1]));
				default:
					return base.FormatFunction(processParameter, operatorType, operands);
			}
		}
		public override string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			switch(operatorType) {
				case BinaryOperatorType.Modulo:
					return string.Format(CultureInfo.InvariantCulture, "{0} % {1}", leftOperand, rightOperand);
				default:
					return base.FormatBinary(operatorType, leftOperand, rightOperand);
			}
		}
		protected override object ReformatReadValue(object value, ConnectionProviderSql.ReformatReadValueArgs args) {
			if(value != null && args.DbTypeCode == TypeCode.Object && args.TargetTypeCode == TypeCode.DateTime && args.DbType == typeof(byte[])) {
				DateTime result;
				if(DateTime.TryParse(Encoding.ASCII.GetString((byte[])value), CultureInfo.InvariantCulture, DateTimeStyles.None, out result)){
					return result;
				}
			}
			return base.ReformatReadValue(value, args);
		}
		protected override object ConvertToDbParameter(object clientValue, TypeCode clientValueTypeCode) {
			if(clientValueTypeCode == TypeCode.Object && (clientValue is Guid)) {
				return clientValue.ToString();
			}
			return base.ConvertToDbParameter(clientValue, clientValueTypeCode);
		}
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			object value = parameter.Value;
			createParameter = false;
			if(parameter is ConstantValue && value != null) {
				switch(Type.GetTypeCode(value.GetType())) {
					case TypeCode.Int32:
						return ((int)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.Boolean:
						return (bool)value ? "1" : "0";
					case TypeCode.String:
						return "'" + ((string)value).Replace("'", "''").Replace(@"\", @"\\") + "'";
				}
			}
			createParameter = true;
			return "?p" + index.ToString(CultureInfo.InvariantCulture);
		}
		public override string FormatConstraint(string constraintName) {
			return string.Format(CultureInfo.InvariantCulture, "`{0}`", constraintName);
		}
		protected override string CreateForeignKeyTemplate { get { return base.CreateForeignKeyTemplate; } }
		void ClearDatabase(IDbCommand command) {
			command.CommandText = "SET FOREIGN_KEY_CHECKS = 0";
			command.ExecuteNonQuery();
			string[] tables = GetStorageTablesList(false);
			foreach(string table in tables) {
				command.CommandText = "drop table `" + table + "`";
				command.ExecuteNonQuery();
			}
			command.CommandText = "SET FOREIGN_KEY_CHECKS = 1";
			command.ExecuteNonQuery();
		}
		protected override void ProcessClearDatabase() {
			IDbCommand command = CreateCommand();
			ClearDatabase(command);
		}
		public override string[] GetStorageTablesList(bool includeViews) {
			SelectStatementResult tables = SelectData(new Query(String.Format(CultureInfo.InvariantCulture,
				is5 ?
				"select TABLE_NAME from INFORMATION_SCHEMA.TABLES where table_schema = '{0}' and (TABLE_TYPE = 'BASE TABLE' " + (includeViews ? " or TABLE_TYPE='VIEW')" : ")") :
				"show tables from {0}"
				, Connection.Database)));
			string[] result = new string[tables.Rows.Length];
			for(int i = 0; i < tables.Rows.Length; i++) {
				result[i] = (string)tables.Rows[i].Values[0];
			}
			return result;
		}
	}
	public class MySqlProviderFactory : ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return MySqlConnectionProvider.CreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return MySqlConnectionProvider.CreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(ServerParamID) || !parameters.ContainsKey(DatabaseParamID) ||
				!parameters.ContainsKey(UserIDParamID) || !parameters.ContainsKey(PasswordParamID)) {
				return null;
			}
			return MySqlConnectionProvider.GetConnectionString(parameters[ServerParamID], parameters[UserIDParamID], parameters[PasswordParamID], parameters[DatabaseParamID]);
		}
		public override IDataStore CreateProvider(Dictionary<string, string> parameters, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			string connectionString = GetConnectionString(parameters);
			if(connectionString == null) {
				objectsToDisposeOnDisconnect = new IDisposable[0];
				return null;
			}
			ConnectionStringParser helper = new ConnectionStringParser(connectionString);
			helper.RemovePartByName(DataStoreBase.XpoProviderTypeParameterName);
			return CreateProviderFromString(helper.GetConnectionString(), autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override bool HasUserName { get { return true; } }
		public override bool HasPassword { get { return true; } }
		public override bool HasIntegratedSecurity { get { return false; } }
		public override bool HasMultipleDatabases { get { return true; } }
		public override bool IsServerbased { get { return true; } }
		public override bool IsFilebased { get { return false; } }
		public override string ProviderKey { get { return MySqlConnectionProvider.XpoProviderTypeString; } }
		public override string[] GetDatabases(string server, string userid, string password) {
			string connectionString = MySqlConnectionProvider.GetConnectionString(server, userid, password, "");
			ConnectionStringParser helper = new ConnectionStringParser(connectionString);
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
		public override string FileFilter { get { return null; } }
		public override bool MeanSchemaGeneration { get { return true; } }
	}
}
