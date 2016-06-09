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

using DevExpress.Utils;
namespace DevExpress.Xpo.DB {
	using System;
	using System.Data;
	using System.Text;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Diagnostics;
	using System.Globalization;
	using DevExpress.Xpo.DB;
	using System.Text.RegularExpressions;
	using DevExpress.Data.Filtering;
	using DevExpress.Xpo.DB.Exceptions;
	using DevExpress.Xpo.DB.Helpers;
	using DevExpress.Xpo;
	using System.IO;
	using System.Collections.Generic;
	using DevExpress.Xpo.Helpers;
	using DevExpress.Compatibility.System.Collections.Specialized;
#if DXRESTRICTED
	using IDbTransaction = System.Data.Common.DbTransaction;
	using IDataReader = System.Data.Common.DbDataReader;
	using IDbConnection = System.Data.Common.DbConnection;
	using IDbCommand = System.Data.Common.DbCommand;
	using IDataParameter = System.Data.Common.DbParameter;
	using IDbDataParameter = System.Data.Common.DbParameter;
#endif
	public class SQLiteConnectionProvider : ConnectionProviderSql {
		public const string XpoProviderTypeString = "SQLite";
		ReflectConnectionHelper helper;
		ReflectConnectionHelper ConnectionHelper {
			get {
				if(helper == null)
#if !DXPORTABLE
					helper = new ReflectConnectionHelper(Connection, "System.Data.SQLite.SQLiteException");
#else
					helper = new ReflectConnectionHelper(Connection, "Microsoft.Data.Sqlite.SqliteException");
#endif
				return helper;
			}
		}
		public static string GetConnectionString(string database) {
			return String.Format("{1}={2};Data Source={0}",
				database, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString);
		}
		public static string GetConnectionString(string database, string password) {
			return String.Format("{2}={3};Data Source={0};Password={1}",
				database, password, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString);
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] { connection };
			return CreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new SQLiteConnectionProvider(connection, autoCreateOption);
		}
		static SQLiteConnectionProvider() {
			RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
			RegisterDataStoreProvider("System.Data.SQLite.SQLiteConnection", new DataStoreCreationFromConnectionDelegate(CreateProviderFromConnection));
			RegisterFactory(new SQLiteProviderFactory());
		}
		public static void Register() { }
		public SQLiteConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		protected override string GetSqlCreateColumnTypeForBoolean(DBTable table, DBColumn column) {
			return "bit";
		}
		protected override string GetSqlCreateColumnTypeForByte(DBTable table, DBColumn column) {
			return "tinyint";
		}
		protected override string GetSqlCreateColumnTypeForSByte(DBTable table, DBColumn column) {
			return "numeric(3,0)";
		}
		protected override string GetSqlCreateColumnTypeForChar(DBTable table, DBColumn column) {
			return "nchar(1)";
		}
		protected override string GetSqlCreateColumnTypeForDecimal(DBTable table, DBColumn column) {
			return "money";
		}
		protected override string GetSqlCreateColumnTypeForDouble(DBTable table, DBColumn column) {
			return "double";
		}
		protected override string GetSqlCreateColumnTypeForSingle(DBTable table, DBColumn column) {
			return "float";
		}
		protected override string GetSqlCreateColumnTypeForInt32(DBTable table, DBColumn column) {
			return "int";
		}
		protected override string GetSqlCreateColumnTypeForUInt32(DBTable table, DBColumn column) {
			return "numeric(10,0)";
		}
		protected override string GetSqlCreateColumnTypeForInt16(DBTable table, DBColumn column) {
			return "smallint";
		}
		protected override string GetSqlCreateColumnTypeForUInt16(DBTable table, DBColumn column) {
			return "numeric(5,0)";
		}
		protected override string GetSqlCreateColumnTypeForInt64(DBTable table, DBColumn column) {
			return "numeric(20,0)";
		}
		protected override string GetSqlCreateColumnTypeForUInt64(DBTable table, DBColumn column) {
			return "numeric(20,0)";
		}
		public const int MaximumStringSize = 800;
		protected override string GetSqlCreateColumnTypeForString(DBTable table, DBColumn column) {
			if(column.Size > 0 && column.Size <= MaximumStringSize)
				return "nvarchar(" + column.Size.ToString(CultureInfo.InvariantCulture) + ')';
			else
				return "text";
		}
		protected override string GetSqlCreateColumnTypeForDateTime(DBTable table, DBColumn column) {
			return "datetime";
		}
		protected override string GetSqlCreateColumnTypeForGuid(DBTable table, DBColumn column) {
			return "char(36)";
		}
		protected override string GetSqlCreateColumnTypeForByteArray(DBTable table, DBColumn column) {
			return "image";
		}
		public override string GetSqlCreateColumnFullAttributes(DBTable table, DBColumn column) {
			string result = GetSqlCreateColumnType(table, column);
			if(column.ColumnType == DBColumnType.Boolean)
				return result;
			if(column.IsKey) {
				if(column.IsIdentity && (column.ColumnType == DBColumnType.Int32 || column.ColumnType == DBColumnType.Int64) && IsSingleColumnPKColumn(table, column)) {
					result = "INTEGER PRIMARY KEY AUTOINCREMENT";
				} else {
					result += " NOT NULL";
				}
			} else {
				result += " NULL";
			}
			return result;
		}
		protected override object ConvertToDbParameter(object clientValue, TypeCode clientValueTypeCode) {
			switch(clientValueTypeCode) {
				case TypeCode.Object:
					if(clientValue is Guid) {
						return clientValue.ToString();
					}
					break;
				case TypeCode.SByte:
					return (Int16)(SByte)clientValue;
				case TypeCode.UInt16:
					return (Int32)(UInt16)clientValue;
				case TypeCode.UInt32:
					return (Int64)(UInt32)clientValue;
				case TypeCode.UInt64:
					return (Decimal)(UInt64)clientValue;
				case TypeCode.Int64:
					return (Decimal)(Int64)clientValue;
			}
			return base.ConvertToDbParameter(clientValue, clientValueTypeCode);
		}
		protected override IDataParameter CreateParameter(IDbCommand command, object value, string name) {
			var parameter = base.CreateParameter(command, value, name);
			if(parameter.DbType == DbType.Decimal) {
				parameter.DbType = DbType.Double;
			}
			return parameter;
		}
		protected override bool IsConnectionBroken(Exception e) {
			object o;
			if(ConnectionHelper.TryGetExceptionProperty(e, "ErrorCode", true, out o) && (o != null)) {
				Type enumType = ConnectionHelper.GetType("System.Data.SQLite.SQLiteErrorCode");
				object sqliteErrorCodeIOErr = Enum.Parse(enumType, "IOErr", true);
				object sqliteErrorCodeFull = Enum.Parse(enumType, "Full", true);
				string[] names = Enum.GetNames(enumType);
				object sqliteErrorCodeNotADatabase;
				if(Array.IndexOf(names, "NotADb") >= 0) {
					sqliteErrorCodeNotADatabase = Enum.Parse(enumType, "NotADb", false);
				} else {
					sqliteErrorCodeNotADatabase = Enum.Parse(enumType, "NotADatabase", false);
				}
				bool broken = Connection.State != ConnectionState.Open
					 || object.Equals(Convert.ToInt32(o), Convert.ToInt32(sqliteErrorCodeIOErr))
					 || object.Equals(Convert.ToInt32(o), Convert.ToInt32(sqliteErrorCodeFull))
					 || object.Equals(Convert.ToInt32(o), Convert.ToInt32(sqliteErrorCodeNotADatabase));
				if(broken)
					Connection.Close();
				return broken;
			}
			return false;
		}
		protected override bool SupportCommandPrepare { get { return true; } }
		protected override Int64 GetIdentity(Query sql) {
			ExecSql(sql);
			object value = GetScalar(new Query("select last_insert_rowid()"));
			return (value as IConvertible).ToInt64(CultureInfo.InvariantCulture);
		}
		protected override IDbConnection CreateConnection() {
			return ConnectionHelper.GetConnection(ConnectionString);
		}
		public static IDbConnection CreateConnection(string connectionString) {
#if !DXPORTABLE
			return ReflectConnectionHelper.GetConnection("System.Data.SQLite", "System.Data.SQLite.SQLiteConnection", connectionString);
#else
			return ReflectConnectionHelper.GetConnection("Microsoft.Data.Sqlite", "Microsoft.Data.Sqlite.SqliteConnection", connectionString);
#endif
		}
		protected override void CreateDataBase() {
#if !DXPORTABLE
			try {
				Connection.Open();
			} catch(Exception e) {
				throw new UnableToOpenDatabaseException(XpoDefault.ConnectionStringRemovePassword(ConnectionString), e);
			}
#endif
		}
		protected override Exception WrapException(Exception e, IDbCommand query) {
			object[] values;
			if(ConnectionHelper.TryGetExceptionProperties(e, new string[] {"Message", "ErrorCode"}, new bool[] { false, true }, out values)) {
				string message = (string)values[0];
				if(message.IndexOf("no such column") > 0 ||
					message.IndexOf("no column named") > 0 ||
					message.IndexOf("no such table") > 0 ||
					message.IndexOf("no table named") > 0)
					return new SchemaCorrectionNeededException(e);
				if(values[1] != null) {
					Type enumType = ConnectionHelper.GetType("System.Data.SQLite.SQLiteErrorCode");
					object sqliteErrorCodeConstraint = Enum.Parse(enumType, "Constraint", false);
					if(object.Equals(values[1], sqliteErrorCodeConstraint))
						return new ConstraintViolationException(query.CommandText, GetParametersString(query), e);
					if(object.Equals(Convert.ToInt32(values[1]), Convert.ToInt32(sqliteErrorCodeConstraint)))
						return new ConstraintViolationException(query.CommandText, GetParametersString(query), e);
				}
			}
			return base.WrapException(e, query);
		}
		delegate bool TablesFilter(DBTable table);
		SelectStatementResult GetDataForTables(ICollection tables, TablesFilter filter, string queryText) {
			QueryParameterCollection parameters = new QueryParameterCollection();
			StringCollection inList = new StringCollection();
			int i = 0;
			foreach(DBTable table in tables) {
				if(filter == null || filter(table)) {
					parameters.Add(new OperandValue(ComposeSafeTableName(table.Name)));
					inList.Add("@p" + i.ToString(CultureInfo.InvariantCulture));
					++i;
				}
			}
			if(inList.Count == 0)
				return new SelectStatementResult();
			return SelectData(new Query(string.Format(CultureInfo.InvariantCulture, queryText, StringListHelper.DelimitedText(inList, ",")), parameters, inList));
		}
		DBColumnType GetTypeFromString(string typeName, out int size) {
			size = 0;
			typeName = typeName.ToLower(CultureInfo.InvariantCulture);
			switch (typeName) {
				case "bigint":
				case "integer":
				case "int":
					return DBColumnType.Int64;
				case "boolean":
				case "bit":
					return DBColumnType.Boolean;
				case "tinyint":
					return DBColumnType.Byte;
				case "text":
					return DBColumnType.String;
				case "date":
				case "time":
				case "datetime":
					return DBColumnType.DateTime;
				case "money":
					return DBColumnType.Decimal;
				case "real":
				case "double precision":
				case "double":
					return DBColumnType.Double;
				case "float":
					return DBColumnType.Single;
				case "smallint":
					return DBColumnType.Int16;
				case "blob":
					return DBColumnType.ByteArray;
			}
			if (typeName.StartsWith("nvarchar") || typeName.StartsWith("varchar") || typeName.StartsWith("text")) {
				int pos = typeName.IndexOf('(') + 1;
				if(pos > 0) {
					size = Int32.Parse(typeName.Substring(pos, typeName.IndexOf(')') - pos));
				}
				return DBColumnType.String;
			}
			if(typeName.StartsWith("numeric"))
				return DBColumnType.Decimal;
			if(typeName.StartsWith("char") || typeName.StartsWith("nchar")) {
				int pos = typeName.IndexOf('(') + 1;
				if(pos == 0)
					return DBColumnType.Char;
				size = Int32.Parse(typeName.Substring(pos, typeName.IndexOf(')') - pos));
				if(size == 1)
					return DBColumnType.Char;
				return DBColumnType.String;
			}
			if(typeName.StartsWith("image")) {
				int pos = typeName.IndexOf('(') + 1;
				if(pos == 0)
					return DBColumnType.ByteArray;
				size = Int32.Parse(typeName.Substring(pos, typeName.IndexOf(')') - pos));
				return DBColumnType.ByteArray;
			}
			return DBColumnType.Unknown;
		}
		void GetColumns(DBTable table) {
			foreach(SelectStatementResultRow row in SelectData(new Query("PRAGMA table_info('" + ComposeSafeTableName(table.Name) + "')")).Rows) {
				int size;
				DBColumnType type = GetTypeFromString((string)row.Values[2], out size);
				table.AddColumn(new DBColumn((string)row.Values[1], false, String.Empty, size, type));
			}
		}
		public override void CreatePrimaryKey(DBTable table) {
		}
		void GetPrimaryKey(DBTable table) {
			StringCollection cols = new StringCollection();
			foreach (SelectStatementResultRow row in SelectData(new Query("PRAGMA table_info('" + ComposeSafeTableName(table.Name) + "')")).Rows) {
				if ((Int64)row.Values[5] == 1) {
					string columnName = (string)row.Values[1];
					DBColumn column = table.GetColumn(columnName);
					if(column != null)
						column.IsKey = true;
					cols.Add(columnName);
				}
			}
			table.PrimaryKey = cols.Count > 0 ? new DBPrimaryKey(cols) : null;
		}
		void GetIndexes(DBTable table) {
			SelectStatementResult data = SelectData(new Query("pragma index_list('" + ComposeSafeTableName(table.Name) + "')"));
			foreach(SelectStatementResultRow row in data.Rows) {
				SelectStatementResult indexData = SelectData(new Query("pragma index_info('" + (string)row.Values[1] + "')"));
				StringCollection cols = new StringCollection();
				foreach(SelectStatementResultRow index in indexData.Rows) {
					cols.Add((string)index.Values[2]);
				}
				table.Indexes.Add(new DBIndex((string)row.Values[1], cols, (long)row.Values[2] == 1));
			}
		}
		public override void CreateForeignKey(DBTable table, DBForeignKey fk) {
		}
		void GetForeignKeys(DBTable table) {
			SelectStatementResult data = SelectData(new Query("pragma foreign_key_list('" + ComposeSafeTableName(table.Name) + "')"));
			foreach (SelectStatementResultRow row in data.Rows) {
				StringCollection pkc = new StringCollection();
				StringCollection fkc = new StringCollection();
				pkc.Add((string)row.Values[3]);
				fkc.Add((string)row.Values[4]);
				table.ForeignKeys.Add(new DBForeignKey(pkc, ((string)row.Values[2]).Trim('\"'), fkc));
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
			Dictionary<string, bool> dbTables = new Dictionary<string, bool>();
			foreach(SelectStatementResultRow row in GetDataForTables(tables, null, "SELECT [name] FROM [sqlite_master] WHERE [type] LIKE 'table' and [name] in ({0})").Rows)
				dbTables.Add((string)row.Values[0], false);
			foreach (SelectStatementResultRow row in GetDataForTables(tables, null, "SELECT [name] FROM [sqlite_master] WHERE [type] LIKE 'view' and [name] in ({0})").Rows)
				dbTables.Add((string)row.Values[0], true);
			ArrayList list = new ArrayList();
			foreach (DBTable table in tables) {
				bool isView;
				if (dbTables.TryGetValue(ComposeSafeTableName(table.Name), out isView))
					table.IsView = isView;
				else
					list.Add(table);
			}
			return list;
		}
		public override void CreateTable(DBTable table) {
			string columns = "";
			foreach(DBColumn col in table.Columns) {
				if(columns.Length > 0)
					columns += ", ";
				columns += (FormatColumnSafe(col.Name) + ' ' + GetSqlCreateColumnFullAttributes(table, col));
			}
			if(table.PrimaryKey != null && !table.GetColumn(table.PrimaryKey.Columns[0]).IsIdentity) {
				StringCollection formattedColumns = new StringCollection();
				for(Int32 i = 0; i < table.PrimaryKey.Columns.Count; ++i)
					formattedColumns.Add(FormatColumnSafe(table.PrimaryKey.Columns[i]));
				ExecuteSqlSchemaUpdate("Table", table.Name, string.Empty, String.Format(CultureInfo.InvariantCulture,
					"create table {0} ({1}, primary key ({2}))",
					FormatTableSafe(table), columns, StringListHelper.DelimitedText(formattedColumns, ",")));
			} else {
				ExecuteSqlSchemaUpdate("Table", table.Name, string.Empty, String.Format(CultureInfo.InvariantCulture,
					"create table {0} ({1})",
					FormatTableSafe(table), columns));
			}
		}
		protected override int GetSafeNameTableMaxLength() {
			return 1024;
		}
		public override string FormatTable(string schema, string tableName) {
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", tableName);
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "[{0}] {1}", tableName, tableAlias);
		}
		public override string FormatColumn(string columnName) {
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", columnName);
		}
		public override string FormatColumn(string columnName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "{1}.[{0}]", columnName, tableAlias);
		}
		public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch (operatorType) {
				case FunctionOperatorType.Abs:
					return string.Format("Abs({0})", operands[0]);
				case FunctionOperatorType.Sign:
					return string.Format("Sign({0})", operands[0]);
				case FunctionOperatorType.BigMul:
					return string.Format("CAST(({0}) * ({1}) as BIGINT)", operands[0], operands[1]);
				case FunctionOperatorType.Round:
					switch(operands.Length) {
						case 1:
							return string.Format("Round({0})", operands[0]);
						case 2:
							return string.Format("Round({0},{1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Floor:
					return string.Format("Floor({0})", operands[0]);
				case FunctionOperatorType.Ceiling:
					return string.Format("Ceiling({0})", operands[0]);
				case FunctionOperatorType.Cos:
					return string.Format("Cos({0})", operands[0]);
				case FunctionOperatorType.Sin:
					return string.Format("Sin({0})", operands[0]);
				case FunctionOperatorType.Acos:
					return string.Format("ACos({0})", operands[0]);
				case FunctionOperatorType.Asin:
					return string.Format("ASin({0})", operands[0]);
				case FunctionOperatorType.Cosh:
					return string.Format("Cosh({0})", operands[0]);
				case FunctionOperatorType.Sinh:
					return string.Format("Sinh({0})", operands[0]);
				case FunctionOperatorType.Exp:
					return string.Format("Exp({0})", operands[0]);
				case FunctionOperatorType.Log:
					switch (operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "Log({0})", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(Log({0}) / Log({1}))", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Log10:
					return string.Format("Log10({0})", operands[0]);
				case FunctionOperatorType.Atn:
					return string.Format("Atan({0})", operands[0]);
				case FunctionOperatorType.Atn2:
					return string.Format(CultureInfo.InvariantCulture, "Atan2({0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Tan:
					return string.Format("Tan({0})", operands[0]);
				case FunctionOperatorType.Tanh:
					return string.Format("Tanh({0})", operands[0]);
				case FunctionOperatorType.Power:
					return string.Format("Power({0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Sqr:
					return string.Format("Sqrt({0})", operands[0]);
				case FunctionOperatorType.Rnd:
					return "((random() / 18446744073709551614) + 0.5)";
				case FunctionOperatorType.Now:
					return "datetime('now','localtime')";
				case FunctionOperatorType.UtcNow:
					return "datetime('now')";
				case FunctionOperatorType.Today:
					return "datetime(date('now','localtime'))";
				case FunctionOperatorType.Replace:
					return string.Format("Replace({0}, {1}, {2})", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Reverse:
					return string.Format("Reverse({0})", operands[0]);
				case FunctionOperatorType.Insert:
					return string.Format(CultureInfo.InvariantCulture, "(SUBSTR({0}, 1, {1}) || ({2}) || SUBSTR({0}, ({1}) + 1))", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Remove:
					switch (operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "SUBSTR({0}, 1, {1})", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "(SUBSTR({0}, 1, {1}) || SUBSTR({0}, ({1}) + ({2}) + 1))", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.AddTicks:
					return string.Format(CultureInfo.InvariantCulture, "strftime('%Y-%m-%d %H:%M:%f', {0} , CAST((({1}) / 10000000) as text) || ' second')", operands[0], operands[1]);
				case FunctionOperatorType.AddMilliSeconds:
					return string.Format(CultureInfo.InvariantCulture, "strftime('%Y-%m-%d %H:%M:%f', {0} , CAST((({1}) / 1000) as text) || ' second')", operands[0], operands[1]);
				case FunctionOperatorType.AddTimeSpan:
				case FunctionOperatorType.AddSeconds:
					return string.Format(CultureInfo.InvariantCulture, "strftime('%Y-%m-%d %H:%M:%f', {0} , CAST({1} as text) || ' second')", operands[0], operands[1]);
				case FunctionOperatorType.AddMinutes:
					return string.Format(CultureInfo.InvariantCulture, "strftime('%Y-%m-%d %H:%M:%f', {0} , CAST({1} as text) || ' minute')", operands[0], operands[1]);
				case FunctionOperatorType.AddHours:
					return string.Format(CultureInfo.InvariantCulture, "strftime('%Y-%m-%d %H:%M:%f', {0} , CAST({1} as text) || ' hour')", operands[0], operands[1]);
				case FunctionOperatorType.AddDays:
					return string.Format(CultureInfo.InvariantCulture, "strftime('%Y-%m-%d %H:%M:%f', {0} , CAST({1} as text) || ' day')", operands[0], operands[1]);
				case FunctionOperatorType.AddMonths:
					return string.Format(CultureInfo.InvariantCulture, "strftime('%Y-%m-%d %H:%M:%f', {0} , CAST({1} as text) || ' month')", operands[0], operands[1]);
				case FunctionOperatorType.AddYears:
					return string.Format(CultureInfo.InvariantCulture, "strftime('%Y-%m-%d %H:%M:%f', {0} , CAST({1} as text) || ' year')", operands[0], operands[1]);
				case FunctionOperatorType.GetMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "(CAST((strftime('%f', {0}) * 1000) as integer) % 1000)", operands[0]);
				case FunctionOperatorType.GetSecond:
					return string.Format(CultureInfo.InvariantCulture, "CAST(strftime('%S', {0}) as integer)", operands[0]);
				case FunctionOperatorType.GetMinute:
					return string.Format(CultureInfo.InvariantCulture, "CAST(strftime('%M', {0}) as integer)", operands[0]);
				case FunctionOperatorType.GetHour:
					return string.Format(CultureInfo.InvariantCulture, "CAST(strftime('%H', {0}) as integer)", operands[0]);
				case FunctionOperatorType.GetDay:
					return string.Format(CultureInfo.InvariantCulture, "CAST(strftime('%d', {0}) as integer)", operands[0]);
				case FunctionOperatorType.GetMonth:
					return string.Format(CultureInfo.InvariantCulture, "CAST(strftime('%m', {0}) as integer)", operands[0]);
				case FunctionOperatorType.GetYear:
					return string.Format(CultureInfo.InvariantCulture, "CAST(strftime('%Y', {0}) as integer)", operands[0]);
				case FunctionOperatorType.GetDayOfWeek:
					return string.Format(CultureInfo.InvariantCulture, "CAST(strftime('%w', {0}) as integer)", operands[0]);
				case FunctionOperatorType.GetDayOfYear:
					return string.Format(CultureInfo.InvariantCulture, "CAST(strftime('%j', {0}) as integer)", operands[0]);
				case FunctionOperatorType.GetTimeOfDay:
					return string.Format(CultureInfo.InvariantCulture, "(CAST((strftime('%H', {0}) * 36000000000) as integer) + CAST((strftime('%M', {0}) * 600000000) as integer) + CAST((strftime('%f', {0}) * 10000000) as integer))", operands[0]);
				case FunctionOperatorType.GetDate:
					return string.Format(CultureInfo.InvariantCulture, "datetime(date({0}))", operands[0]);
				case FunctionOperatorType.DateDiffYear:
					return string.Format(CultureInfo.InvariantCulture, "(strftime('%Y', {1}) - strftime('%Y', {0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMonth:
					return string.Format(CultureInfo.InvariantCulture, "(((strftime('%Y', {1}) - strftime('%Y', {0})) * 12) + strftime('%m', {1}) - strftime('%m', {0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffDay:
					return string.Format(CultureInfo.InvariantCulture, "(julianday(date({1})) - julianday(date({0})))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffHour:
					return string.Format(CultureInfo.InvariantCulture, "(((julianday(date({1})) - julianday(date({0}))) * 24) + (strftime('%H', {1}) - strftime('%H', {0})))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMinute:
					return string.Format(CultureInfo.InvariantCulture, "((((((julianday(date({1})) - julianday(date({0}))) * 24) + (strftime('%H', {1}) - strftime('%H', {0})))) * 60)  + (strftime('%M', {1}) - strftime('%M', {0})))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffSecond:
					return string.Format(CultureInfo.InvariantCulture, "(strftime('%s', {1}) - strftime('%s', {0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "((strftime('%s', {1}) - strftime('%s', {0})) * 1000)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffTick:
					return string.Format(CultureInfo.InvariantCulture, "((strftime('%s', {1}) - strftime('%s', {0}))) * 10000000", operands[0], operands[1]);
				case FunctionOperatorType.Len: {
						string args = string.Empty;
						foreach (string arg in operands) {
							if (args.Length > 0)
								args += ", ";
							args += arg;
						}
						return "Length(" + args + ")";
					}
				case FunctionOperatorType.PadLeft:
					switch (operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "PadL({0}, {1})", operands[0], operands[1]);
						default:
							throw new NotSupportedException();
					}
				case FunctionOperatorType.PadRight:
					switch (operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "PadR({0}, {1})", operands[0], operands[1]);
						default:
							throw new NotSupportedException();
					}
				case FunctionOperatorType.CharIndex:
					switch (operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(Charindex({0}, {1}) - 1)", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "(Charindex({0}, {1}, ({2}) + 1) - 1)", operands[0], operands[1], operands[2]);
						case 4:
							return string.Format(CultureInfo.InvariantCulture, "(Charindex({0}, Substr({1}, 1, ({2}) + ({3})), ({2}) + 1) - 1)", operands[0], operands[1], operands[2], operands[3]);
					}
					goto default;
				case FunctionOperatorType.Substring: {
						switch (operands.Length) {
							case 2:
								return string.Format(CultureInfo.InvariantCulture, "substr({0}, ({1}) + 1)", operands[0], operands[1]);
							case 3:
								return string.Format(CultureInfo.InvariantCulture, "substr({0}, ({1}) + 1, {2})", operands[0], operands[1], operands[2]);
						}
						goto default;
					}
				case FunctionOperatorType.ToInt:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} as int)", operands[0]);
				case FunctionOperatorType.ToLong:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} as bigint)", operands[0]);
				case FunctionOperatorType.ToFloat:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} as real)", operands[0]);
				case FunctionOperatorType.ToDouble:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} as double precision)", operands[0]);
				case FunctionOperatorType.ToDecimal:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} as money)", operands[0]);				
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} as text)", operands[0]);
				case FunctionOperatorType.Ascii:
				case FunctionOperatorType.Char:
				case FunctionOperatorType.Max:
				case FunctionOperatorType.Min:
					throw new NotSupportedException();
				case FunctionOperatorType.IsNull:
					switch (operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "(({0}) is null)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "COALESCE({0}, {1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) is null or length({0}) = 0)", operands[0]);
				case FunctionOperatorType.EndsWith:
					return string.Format(CultureInfo.InvariantCulture, "(Substr({0}, Length({0}) - Length({1}) + 1) = ({1}))", operands[0], operands[1]);
				case FunctionOperatorType.Contains:
					return string.Format(CultureInfo.InvariantCulture, "(CharIndex({1}, {0}) > 0)", operands[0], operands[1]);
				default:
					return base.FormatFunction(operatorType, operands);
			}
		}
		readonly static char[] achtungChars = new char[] { '_', '%' };
		public override string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			switch (operatorType) {
				case FunctionOperatorType.StartsWith:
					object secondOperand = operands[1];
					if (secondOperand is OperandValue && ((OperandValue)secondOperand).Value is string) {
						string operandString = (string)((OperandValue)secondOperand).Value;
						int likeIndex = operandString.IndexOfAny(achtungChars);
						if (likeIndex < 0) {
							return string.Format(CultureInfo.InvariantCulture, "({0} like {1})", processParameter(operands[0]), processParameter(new ConstantValue(operandString + "%")));
						} else if (likeIndex > 0) {
							return string.Format(CultureInfo.InvariantCulture, "(({0} like {2}) And (Substr({0}, 1, Length({1})) = ({1})))", processParameter(operands[0]), processParameter(secondOperand), processParameter(new ConstantValue(operandString.Substring(0, likeIndex) + "%")));
						}
					}
					return string.Format(CultureInfo.InvariantCulture, "(Substr({0}, 1, Length({1})) = ({1}))", processParameter(operands[0]), processParameter(operands[1]));				
			}
			return base.FormatFunction(processParameter, operatorType, operands);
		}		
		public override string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int skipSelectedRecords, int topSelectedRecords) {
			base.FormatSelect(selectedPropertiesSql, fromSql, whereSql, orderBySql, groupBySql, havingSql, skipSelectedRecords, topSelectedRecords);
			string expandedWhereSql = whereSql != null ? string.Format(CultureInfo.InvariantCulture, "\nwhere {0}", whereSql) : string.Empty;
			string expandedOrderBySql = orderBySql != null ? string.Format(CultureInfo.InvariantCulture, "\norder by {0}", orderBySql) : string.Empty;
			string expandedHavingSql = havingSql != null ? string.Format(CultureInfo.InvariantCulture, "\nhaving {0}", havingSql) : string.Empty;
			string expandedGroupBySql = groupBySql != null ? string.Format(CultureInfo.InvariantCulture, "\ngroup by {0}", groupBySql) : string.Empty;
			string modificatorsSql = string.Empty;
			if(skipSelectedRecords != 0 || topSelectedRecords != 0) {
				modificatorsSql = string.Format(CultureInfo.InvariantCulture, " LIMIT {0} OFFSET {1} ", topSelectedRecords == 0 ? Int32.MaxValue : topSelectedRecords, skipSelectedRecords);
			}
			return string.Format(CultureInfo.InvariantCulture, "select {0} from {1}{2}{3}{4}{5}{6}", selectedPropertiesSql, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedOrderBySql, modificatorsSql);
		}
		public override bool NativeSkipTakeSupported { get { return true; } }
		public override string FormatInsertDefaultValues(string tableName) {
			return string.Format(CultureInfo.InvariantCulture, "insert into {0} values(null)", tableName);
		}
		public override string FormatInsert(string tableName, string fields, string values) {
			return string.Format(CultureInfo.InvariantCulture, "insert into {0}({1})values({2})",
				tableName, fields, values);
		}
		public override string FormatUpdate(string tableName, string sets, string whereClause) {
			return string.Format(CultureInfo.InvariantCulture, "update {0} set {1} where {2}",
				tableName, sets, whereClause);
		}
		public override bool BraceJoin { get { return false; } }
		public override string FormatDelete(string tableName, string whereClause) {
			return string.Format(CultureInfo.InvariantCulture, "delete from {0} where {1}", tableName, whereClause);
		}
		public override string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			switch(operatorType) {
				case BinaryOperatorType.BitwiseAnd:
					return string.Format(CultureInfo.InvariantCulture, "{0} & {1}", leftOperand, rightOperand);
				case BinaryOperatorType.BitwiseOr:
					return string.Format(CultureInfo.InvariantCulture, "{0} | {1}", leftOperand, rightOperand);
				case BinaryOperatorType.BitwiseXor:
					return string.Format(CultureInfo.InvariantCulture, "(~({0}&{1}) & ({0}|{1}))", leftOperand, rightOperand);
				case BinaryOperatorType.Modulo:
					return string.Format(CultureInfo.InvariantCulture, "{0} % {1}", leftOperand, rightOperand);
				default:
					return base.FormatBinary(operatorType, leftOperand, rightOperand);
			}
		}
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			object value = parameter.Value;
			createParameter = false;
			if(parameter is ConstantValue && value != null) {
				switch(DXTypeExtensions.GetTypeCode(value.GetType())) {
					case TypeCode.Int32:
						return ((int)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.Boolean:
						return (bool)value ? "1" : "0";
					case TypeCode.String:
						return "'" + ((string)value).Replace("'", "''") + "'";
				}
			}
			createParameter = true;
			return "@p" + index.ToString(CultureInfo.InvariantCulture);
		}
		public override string FormatConstraint(string constraintName) {
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", constraintName);
		}
		protected override void ProcessClearDatabase() {
			Connection.Close();
			ConnectionStringParser helper = new ConnectionStringParser(ConnectionString);
			string file = helper.GetPartByName("Data Source");
			if(File.Exists(file))
				File.Delete(file);
		}
		public override string[] GetStorageTablesList(bool includeViews) {
			SelectStatementResult tables = SelectData(new Query(String.Format("SELECT [name] FROM [sqlite_master] WHERE [type] LIKE 'table'{0} and [name] not like 'SQLITE_%'", (includeViews ? " or [type] LIKE 'view'" : ""))));
			ArrayList result = new ArrayList(tables.Rows.Length);
			foreach(SelectStatementResultRow row in tables.Rows) {
				result.Add(row.Values[0]);
			}
			return (string[])result.ToArray(typeof(string));
		}
		protected override SelectedData ExecuteSproc(string sprocName, params OperandValue[] parameters) {
			throw new NotSupportedException();
		}
		protected override void CommandBuilderDeriveParameters(IDbCommand command) {
			throw new NotSupportedException();
		}
		public override DBStoredProcedure[] GetStoredProcedures() {
			throw new NotSupportedException();
		}
	}
	public class SQLiteProviderFactory : ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return SQLiteConnectionProvider.CreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return SQLiteConnectionProvider.CreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(DatabaseParamID)) { return null; }
			string connectionString;
			string password;
			if(parameters.TryGetValue(PasswordParamID, out password) && !string.IsNullOrEmpty(password)) {
				connectionString = SQLiteConnectionProvider.GetConnectionString(parameters[DatabaseParamID], parameters[PasswordParamID]);
			}
			else {
				connectionString = SQLiteConnectionProvider.GetConnectionString(parameters[DatabaseParamID]);
			}
			return connectionString;
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
		public override bool HasUserName { get { return false; } }
		public override bool HasPassword { get { return true; } }
		public override bool HasIntegratedSecurity { get { return false; } }
		public override bool HasMultipleDatabases { get { return false; } }
		public override bool IsServerbased { get { return false; } }
		public override bool IsFilebased { get { return true; } }
		public override string ProviderKey { get { return SQLiteConnectionProvider.XpoProviderTypeString; } }
		public override string[] GetDatabases(string server, string userid, string password) {
			return new string[1] { server };
		}
		public override string FileFilter { get { return "SQLite databases|*.db"; } }
		public override bool MeanSchemaGeneration { get { return true; } }
	}
}
