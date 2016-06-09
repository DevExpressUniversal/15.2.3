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
	using System.Text.RegularExpressions;
	using DevExpress.Data.Filtering;
	using DevExpress.Xpo.DB.Exceptions;
	using DevExpress.Xpo.DB.Helpers;
	using DevExpress.Xpo;
	using System.Collections.Generic;
	using DevExpress.Xpo.Helpers;
	using System.Reflection;
	public class PostgreSqlConnectionProvider : ConnectionProviderSql {
		public const string XpoProviderTypeString = "Postgres";
		const string ConectionStringEncodingParameterName = "Encoding";
		ReflectConnectionHelper helper;
		ReflectConnectionHelper ConnectionHelper {
			get {
				if(helper == null)
					helper = new ReflectConnectionHelper(Connection, "Npgsql.NpgsqlException");
				return helper;
			}
		}
		public static string GetConnectionString(string server, string userid, string password, string database) {
			return String.Format("{4}={5};Server={0};User Id={1};Password={2};Database={3};{6}=UNICODE;",
				server, userid, password, database, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString, ConectionStringEncodingParameterName);
		}
		public static string GetConnectionString(string server, int port, string userid, string password, string database) {
			return String.Format("{5}={6};Server={0};User Id={1};Password={2};Database={3};{7}=UNICODE;Port={4}",
				server, userid, password, database, port, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString, ConectionStringEncodingParameterName);
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] { connection };
			return CreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new PostgreSqlConnectionProvider(connection, autoCreateOption);
		}
		static PostgreSqlConnectionProvider() {
			RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
			RegisterDataStoreProvider("NpgsqlConnection", new DataStoreCreationFromConnectionDelegate(CreateProviderFromConnection));
			RegisterFactory(new PostgreSqlProviderFactory());
		}
		public static void Register() { }
		public PostgreSqlConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
			ReadDbVersion(connection);
		}
		decimal? versionMajor;
		int versionMinor;
		void ReadDbVersion(IDbConnection conn) {
			try {
				using(IDbCommand c = CreateCommand(new Query("SHOW server_version"))) {
					object result = c.ExecuteScalar();
					if(result != null && result is string) {
						SetServerVersionInternal((string)result);
					}
				}
			} catch { }
		}
		bool SetServerVersionInternal(string versionString) {
			string[] versionParts = versionString.Split('.');
			decimal versionMajorLocal;
			if(versionParts.Length == 3 && decimal.TryParse(string.Concat(versionParts[0], ".", versionParts[1]), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out versionMajorLocal) && int.TryParse(versionParts[2], out versionMinor)) {
				versionMajor = versionMajorLocal;
				return true;
			}
			return false;
		}
		bool SupportVersion(decimal major, int minor) {
			if(!versionMajor.HasValue)
				return true;
			if(versionMajor.Value > major)
				return true;
			if(versionMajor.Value == major && versionMinor >= minor)
				return true;
			return false;
		}
		public void SetServerVersion(string versionString) {
			if(!SetServerVersionInternal(versionString)) {
				throw new ArgumentException("versionString");
			}
		}
		protected override string GetSqlCreateColumnTypeForBoolean(DBTable table, DBColumn column) {
			return "bool";
		}
		protected override string GetSqlCreateColumnTypeForByte(DBTable table, DBColumn column) {
			return "smallint";
		}
		protected override string GetSqlCreateColumnTypeForSByte(DBTable table, DBColumn column) {
			return "smallint";
		}
		protected override string GetSqlCreateColumnTypeForChar(DBTable table, DBColumn column) {
			return "char(1)";
		}
		protected override string GetSqlCreateColumnTypeForDecimal(DBTable table, DBColumn column) {
			return "decimal(28,8)";
		}
		protected override string GetSqlCreateColumnTypeForDouble(DBTable table, DBColumn column) {
			return "double precision";
		}
		protected override string GetSqlCreateColumnTypeForSingle(DBTable table, DBColumn column) {
			return "real";
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
			return "bigint";
		}
		protected override string GetSqlCreateColumnTypeForUInt64(DBTable table, DBColumn column) {
			return "numeric(20,0)";
		}
		public const int MaximumStringSize = 4000;
		protected override string GetSqlCreateColumnTypeForString(DBTable table, DBColumn column) {
			if(column.Size > 0 && column.Size <= MaximumStringSize)
				return "varchar(" + column.Size.ToString(CultureInfo.InvariantCulture) + ')';
			else
				return "text";
		}
		protected override string GetSqlCreateColumnTypeForDateTime(DBTable table, DBColumn column) {
			return "timestamp";
		}
		protected override string GetSqlCreateColumnTypeForGuid(DBTable table, DBColumn column) {
			return "char(36)";
		}
		protected override string GetSqlCreateColumnTypeForByteArray(DBTable table, DBColumn column) {
			return "bytea";
		}
		public override string GetSqlCreateColumnFullAttributes(DBTable table, DBColumn column) {
			if(column.IsKey && column.IsIdentity && IsSingleColumnPKColumn(table, column)) {
				switch(column.ColumnType){
					case DBColumnType.Int32:
						return "serial PRIMARY KEY";
					case DBColumnType.Int64:
						return "bigserial PRIMARY KEY";
				}
			}
			string result = GetSqlCreateColumnType(table, column);
			if(column.IsKey)
				result += " NOT NULL";
			else
				result += " NULL";
			return result;
		}
		protected override object ConvertToDbParameter(object clientValue, TypeCode clientValueTypeCode) {
			switch(clientValueTypeCode) {
				case TypeCode.Object:
					if(clientValue is Guid) {
						return clientValue.ToString();
					}
					break;
				case TypeCode.Byte:
					return (Int16)(Byte)clientValue;
				case TypeCode.SByte:
					return (Int16)(SByte)clientValue;
				case TypeCode.UInt16:
					return (Int32)(UInt16)clientValue;
				case TypeCode.UInt32:
					return (Int64)(UInt32)clientValue;
				case TypeCode.UInt64:
					return (Decimal)(UInt64)clientValue;
				case TypeCode.Single:
					return (Double)(Single)clientValue;
			}
			return base.ConvertToDbParameter(clientValue, clientValueTypeCode);
		}
		protected override Int64 GetIdentity(InsertStatement root, TaggedParametersHolder identitiesByTag) {
			string seq = GetSeqName(root.Table.Name, root.IdentityColumn);
			Query sql = new InsertSqlGenerator(this, identitiesByTag, new Dictionary<OperandValue, string>()).GenerateSql(root);
			sql.Parameters.Add(new OperandValue(seq));
			sql.ParametersNames.Add("@seq");
			object value = GetScalar(new Query(sql.Sql + ";select currval(@seq)", sql.Parameters, sql.ParametersNames));
			long id = ((IConvertible)value).ToInt64(CultureInfo.InvariantCulture);
			return id;
		}
		protected override IDbConnection CreateConnection() {
			return ConnectionHelper.GetConnection(ConnectionString);
		}
		static public IDbConnection CreateConnection(string connectionString) {
			var connection = ReflectConnectionHelper.GetConnection("Npgsql", "Npgsql.NpgsqlConnection", true);
			if(connection.GetType().Assembly.GetName().Version.Major > 2) {
				var helper = new ConnectionStringParser(connectionString);
				helper.RemovePartByName(ConectionStringEncodingParameterName);
				connection.ConnectionString = helper.GetConnectionString();
			} else {
				connection.ConnectionString = connectionString;
			}
			return connection;
		}
		protected override IDataParameter CreateParameter(IDbCommand command, object value, string name) {
			IDataParameter param = CreateParameter(command);
			param.Value = value;
			param.ParameterName = name;
			if(param.DbType == DbType.AnsiString)
				param.DbType = DbType.String;
			if(value is DateTime)
				param.DbType = DbType.DateTime;
			if(value is byte[])
				param.DbType = DbType.Binary;
			return param;
		}
		protected override Exception WrapException(Exception e, IDbCommand query) {
			object o;
			if(ConnectionHelper.TryGetExceptionProperty(e, "Code", out o)) {
				string code = (string)o;
				if(code == "42703" || code == "42P01")
					return new SchemaCorrectionNeededException(e);
				if(code == "23503" || code == "23505")
					return new ConstraintViolationException(query.CommandText, GetParametersString(query), e);
			}
			return base.WrapException(e, query);
		}
		public override void CreateTable(DBTable table) {
			string columns = string.Empty;
			foreach(DBColumn col in table.Columns) {
				if(columns.Length > 0)
					columns += ", ";
				columns += (FormatColumnSafe(col.Name) + ' ' + GetSqlCreateColumnFullAttributes(table, col));
			}
			ExecuteSqlSchemaUpdate("Table", table.Name, string.Empty, String.Format(CultureInfo.InvariantCulture,
				"create table {0} ({1}) without oids",
				FormatTableSafe(table), columns));
		}
		protected override void CreateDataBase() {
			const string CannotOpenDatabaseError = "3D000";
			try {
				Connection.Open();
			} catch(Exception e) {
				object[] propertiesValues;
				if(ConnectionHelper.TryGetExceptionProperties(e, new string[] { "Errors", "Code" }, out propertiesValues)
					&& ((IList)propertiesValues[0]) != null
					&& ((IList)propertiesValues[0]).Count > 0
					&& ((string)propertiesValues[1]) == CannotOpenDatabaseError
					&& CanCreateDatabase) {
					ConnectionStringParser helper = new ConnectionStringParser(ConnectionString);
					string dbName = helper.GetPartByName("Database");
					helper.RemovePartByName("Database");
					using(IDbConnection conn = ConnectionHelper.GetConnection(helper.GetConnectionString() + ";Database=template1")) {
						conn.Open();
						using(IDbCommand c = conn.CreateCommand()) {
							if(!dbName.StartsWith("\""))
								dbName = '"' + dbName + '"';
							c.CommandText = "Create Database " + dbName + " WITH ENCODING='UNICODE'";
							c.ExecuteNonQuery();
						}
					}
				} else {
					throw new UnableToOpenDatabaseException(XpoDefault.ConnectionStringRemovePassword(ConnectionString), e);
				}
			}
		}
		string GetSeqName(string tableName, string columnName) {
			string schema = ComposeSafeSchemaName(tableName);
			string table = ComposeSafeTableName(tableName);
			const string postfix = "_xpoView";
			string seqName = "\"" + (table.EndsWith(postfix) ? table.Substring(0, table.Length - postfix.Length) : table);
			string seqTail = string.Concat("_", columnName, "_seq\"");
			if(seqName.Length + seqTail.Length > 64)
				seqName = seqName.Substring(0, 65 - seqTail.Length);
			return string.IsNullOrEmpty(schema) ? seqName + seqTail : string.Concat("\"", schema, "\".", seqName, seqTail);
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
		DBColumnType GetTypeFromString(string typeName, int length) {
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
					return DBColumnType.Decimal;
				case "float8":
					return DBColumnType.Double;
				case "float4":
					return DBColumnType.Single;
				case "timestamp":
					return DBColumnType.DateTime;
				case "uuid":
					return DBColumnType.Guid;
			}
			return DBColumnType.Unknown;
		}
		void GetColumns(DBTable table) {
			string schema = ComposeSafeSchemaName(table.Name);
			if (schema == string.Empty) schema = ObjectsOwner;
			string safeTableName = ComposeSafeTableName(table.Name);
			Query query = new Query(@"select a.attname, t.typname, a.atttypmod from pg_attribute a
join pg_class c on a.attrelid = c.oid
join pg_type t on a.atttypid = t.oid
join pg_namespace n on c.relnamespace = n.oid
where c.relname = @p0 AND n.nspname = @p1 AND a.attnum > 0 AND NOT a.attisdropped and (c.relkind = 'r' OR c.relkind = 'v')",
				new QueryParameterCollection(new OperandValue(safeTableName), new OperandValue(schema)), new string[] { "@p0", "@p1" });
			foreach(SelectStatementResultRow row in SelectData(query).Rows) {
				int size = ((int)row.Values[2]) - 4;
				DBColumnType type = GetTypeFromString((string)row.Values[1], size);
				table.AddColumn(new DBColumn((string)row.Values[0], false, String.Empty, type == DBColumnType.String ? size : 0, type));
			}
		}
		void GetPrimaryKey(DBTable table) {
			string schema = ComposeSafeSchemaName(table.Name);
			if(schema == string.Empty) schema = ObjectsOwner;
			string safeTableName = ComposeSafeTableName(table.Name);
			Query query = new Query(@"select a.attname from pg_constraint c
join pg_attribute a on c.conrelid=a.attrelid and a.attnum = ANY (c.conkey)
join pg_class tc on c.conrelid=tc.oid
join pg_namespace n on tc.relnamespace = n.oid
where c.contype = 'p' and tc.relname = @p0 and n.nspname = @p1
order by a.attnum",
				new QueryParameterCollection(new OperandValue(safeTableName), new OperandValue(schema)), new string[] { "@p0", "@p1" });
			SelectStatementResult data = SelectData(query);
			if(data.Rows.Length > 0) {
				StringCollection cols = new StringCollection();
				for(int i = 0; i < data.Rows.Length; i++) {
					string columnName = (string)data.Rows[i].Values[0];
					DBColumn column = table.GetColumn(columnName);
					if(column != null)
						column.IsKey = true;
					cols.Add(columnName);
				}
				table.PrimaryKey = new DBPrimaryKey(table.Name, cols);
			}
		}
		void GetIndexes(DBTable table) {
			string schema = ComposeSafeSchemaName(table.Name);
			if(schema == string.Empty) schema = ObjectsOwner;
			string safeTableName = ComposeSafeTableName(table.Name);
			Query query = new Query(@"select ind.relname, col.attname, col.attnum, i.indisunique from pg_index i
join pg_class ind on i.indexrelid = ind.oid
join pg_class tbl on i.indrelid = tbl.oid
join pg_attribute col on ind.oid = col.attrelid
join pg_namespace n on tbl.relnamespace = n.oid
where tbl.relname = @p0 and n.nspname = @p1
order by ind.relname, col.attnum
",
				new QueryParameterCollection(new OperandValue(safeTableName), new OperandValue(schema)), new string[] { "@p0", "@p1" });
			SelectStatementResult data = SelectData(query);
			DBIndex index = null;
			foreach(SelectStatementResultRow row in data.Rows) {
				if(1 == (short)row.Values[2]) {
					StringCollection list = new StringCollection();
					list.Add((string)row.Values[1]);
					index = new DBIndex((string)row.Values[0], list, (bool)row.Values[3]);
					table.Indexes.Add(index);
				} else
					index.Columns.Add((string)row.Values[1]);
			}
		}
		void GetForeignKeys(DBTable table) {
			string schema = ComposeSafeSchemaName(table.Name);
			if(schema == string.Empty) schema = ObjectsOwner;
			string safeTableName = ComposeSafeTableName(table.Name);
			Query query = new Query(@"select pos.n, col.attname ,fc.attname, tcf.relname from generate_series(1,current_setting('max_index_keys')::int,1) pos(n), pg_constraint c
join pg_class tc on c.conrelid = tc.oid
join pg_class tcf on c.confrelid = tcf.oid
join pg_attribute col on c.conrelid = col.attrelid
join pg_attribute fc on c.confrelid = fc.attrelid
join pg_namespace n on tcf.relnamespace = n.oid
where c.contype='f' and tc.relname = @p0 and n.nspname = @p1 and fc.attnum = c.confkey[pos.n] and col.attnum = c.conkey[pos.n]
order by c.conname, pos.n",
				new QueryParameterCollection(new OperandValue(safeTableName), new OperandValue(schema)), new string[] { "@p0", "@p1" });
			SelectStatementResult data = SelectData(query);
			DBForeignKey fk = null;
			foreach(SelectStatementResultRow row in data.Rows) {
				if((int)row.Values[0] == 1) {
					StringCollection pkc = new StringCollection();
					StringCollection fkc = new StringCollection();
					pkc.Add((string)row.Values[2]);
					fkc.Add((string)row.Values[1]);
					fk = new DBForeignKey(fkc, (string)row.Values[3], pkc);
					table.ForeignKeys.Add(fk);
				} else {
					fk.Columns.Add((string)row.Values[1]);
					fk.PrimaryKeyTableKeyColumns.Add((string)row.Values[2]);
				}
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
			Dictionary<string, bool> dbSchemaTables = new Dictionary<string, bool>();
			string queryString = @"select c.relname, c.relkind, n.nspname from pg_class c 
join pg_namespace n on c.relnamespace = n.oid
where c.relname in({0}) and (c.relkind = 'r' OR c.relkind = 'v')";  
			foreach (SelectStatementResultRow row in GetDataForTables(tables, null, queryString).Rows) {
				if (row.Values[0] is DBNull) continue;
				string tableName = (string)row.Values[0];
				bool isView = Convert.ToString(row.Values[1]) == "v";
				string tableSchemaName = (string)row.Values[2];
				dbTables[tableName] = isView;
				dbSchemaTables.Add(string.Concat(tableSchemaName, ".", tableName), isView);				
			}
			ArrayList list = new ArrayList();
			foreach (DBTable table in tables) {
				string tableName = ComposeSafeTableName(table.Name);
				string tableSchemaName = ComposeSafeSchemaName(table.Name);
				bool isView = false;
				if (!dbSchemaTables.TryGetValue(string.Concat(tableSchemaName, ".", tableName), out isView) && !dbTables.TryGetValue(tableName, out isView))
					list.Add(table);
				else
					table.IsView = isView;
			}
			return list;
		}
		protected override int GetSafeNameTableMaxLength() {
			return 63;
		}
		public override string FormatTable(string schema, string tableName) {			
			if(string.IsNullOrEmpty(schema))
				return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", tableName);
			else
				return string.Format(CultureInfo.InvariantCulture, "\"{0}\".\"{1}\"", schema, tableName);
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			if (string.IsNullOrEmpty(schema))
				return string.Format(CultureInfo.InvariantCulture, "\"{0}\" {1}", tableName, tableAlias);
			else
				return string.Format(CultureInfo.InvariantCulture, "\"{0}\".\"{1}\" {2}", schema, tableName, tableAlias);
		}
		public override string FormatColumn(string columnName) {
			return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", columnName);
		}
		public override string FormatColumn(string columnName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "{1}.\"{0}\"", columnName, tableAlias);
		}
		public override bool NativeSkipTakeSupported { get { return true; } }
		public override string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int skipSelectedRecords, int topSelectedRecords) {
			base.FormatSelect(selectedPropertiesSql, fromSql, whereSql, orderBySql, groupBySql, havingSql, skipSelectedRecords, topSelectedRecords);
			string modificatorsSql = string.Empty;
			if(skipSelectedRecords != 0 || topSelectedRecords != 0) {
				string limitSql = string.Format(CultureInfo.InvariantCulture, "LIMIT {0} ", topSelectedRecords);
				modificatorsSql = string.Format(CultureInfo.InvariantCulture, " {0}OFFSET {1} ", topSelectedRecords == 0 ? string.Empty : limitSql, skipSelectedRecords);
			}
			string expandedWhereSql = whereSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}where {1}", Environment.NewLine, whereSql) : string.Empty;
			string expandedOrderBySql = orderBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}order by {1}", Environment.NewLine, orderBySql) : string.Empty;
			string expandedHavingSql = havingSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}having {1}", Environment.NewLine, havingSql) : string.Empty;
			string expandedGroupBySql = groupBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}group by {1}", Environment.NewLine, groupBySql) : string.Empty;
			return string.Format(CultureInfo.InvariantCulture, "select {1} from {2}{3}{4}{5}{6}{0}", modificatorsSql, selectedPropertiesSql, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedOrderBySql);
		}
		public override string FormatInsertDefaultValues(string tableName) {
			return string.Format(CultureInfo.InvariantCulture, "insert into {0} values(default)", tableName);
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
		public override string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			switch(operatorType) {
				case BinaryOperatorType.Modulo:
					return string.Format(CultureInfo.InvariantCulture, "{0} % {1}", leftOperand, rightOperand);
				case BinaryOperatorType.BitwiseXor:
					return string.Format(CultureInfo.InvariantCulture, "({0} # {1})", leftOperand, rightOperand);
				default:
					return base.FormatBinary(operatorType, leftOperand, rightOperand);
			}
		}
		static string FormatMod(string arg, int multiplier, int divider, bool useFloor = true) {
			return string.Format(useFloor ? "mod((trunc(({0})::numeric * {1}))::bigint, {2})" : "mod((({0})::numeric * {1})::bigint, {2})", arg, multiplier, divider);
		}
		static string FormatGetInt(string arg, int multiplier, int divider) {
			return string.Format("((trunc(({0})::numeric * {1}))::bigint / {2})", arg, multiplier, divider);
		}
		string FnAddDateTime(string datetimeOperand, string dayPart, string secondPart, string millisecondPart) {
			if(SupportVersion(8.3m, 0)) {
				return string.Format(CultureInfo.InvariantCulture, "(({0}) + ({1} || ' day')::interval + ({2} || ' second')::interval + ({3} || ' millisecond')::interval)", 
					datetimeOperand, dayPart, secondPart, millisecondPart);
			}
			return string.Format(CultureInfo.InvariantCulture, "(({0}) + ({1} || ' day')::interval + ({2} || ' second')::interval)", 
				datetimeOperand, dayPart, secondPart);
		}
		public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.Len:
					return string.Concat("length(", operands[0], ')');
				case FunctionOperatorType.Substring:
					string len = operands.Length < 3 ? string.Concat("length(", operands[0], ")", " - ", operands[1]) : operands[2];
					return string.Concat("substr(", operands[0], ", (", operands[1], ") + 1, ", len, ")");
				case FunctionOperatorType.Char:
					return string.Format(CultureInfo.InvariantCulture, "chr({0})", operands[0]);
				case FunctionOperatorType.CharIndex:
					return FnCharIndex(operands);
				case FunctionOperatorType.Log:
					return FnLog(operands);
				case FunctionOperatorType.Log10:
					return string.Format(CultureInfo.InvariantCulture, "log({0})", operands[0]);
				case FunctionOperatorType.Remove:
					return FnRemove(operands);
				case FunctionOperatorType.Rnd:
					return "random()";
				case FunctionOperatorType.Max:
					return string.Format(CultureInfo.InvariantCulture, "(case when {0} > {1} then {0} else {1} end)", operands[0], operands[1]);
				case FunctionOperatorType.Min:
					return string.Format(CultureInfo.InvariantCulture, "(case when {0} < {1} then {0} else {1} end)", operands[0], operands[1]);
				case FunctionOperatorType.Round:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "Round(cast({0} as numeric), 0)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "Round(cast({0} as numeric),{1})", operands[0], operands[1]);
					}
					break;
				case FunctionOperatorType.Sqr:
					return string.Format(CultureInfo.InvariantCulture, "sqrt({0})", operands[0]);
				case FunctionOperatorType.Acos:
					return string.Format(CultureInfo.InvariantCulture, "acos({0})", operands[0]);
				case FunctionOperatorType.Asin:
					return string.Format(CultureInfo.InvariantCulture, "asin({0})", operands[0]);
				case FunctionOperatorType.Atn:
					return string.Format(CultureInfo.InvariantCulture, "atan({0})", operands[0]);
				case FunctionOperatorType.Atn2:
					return string.Format(CultureInfo.InvariantCulture, "atan2({0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Cosh:
					return string.Format(CultureInfo.InvariantCulture, "((exp({0}::real) + exp(-({0})::real)) / 2)", operands[0]);
				case FunctionOperatorType.Sinh:
					return string.Format(CultureInfo.InvariantCulture, "((exp({0}::real) - exp(-({0})::real)) / 2)", operands[0]);
				case FunctionOperatorType.Tanh:
					return string.Format(CultureInfo.InvariantCulture, "((exp({0}::real) - exp(-({0})::real)) / (exp({0}::real) + exp(-({0})::real)))", operands[0]);
				case FunctionOperatorType.BigMul:
					return string.Format(CultureInfo.InvariantCulture, "(({0})::bigint * ({1})::bigint)", operands[0], operands[1]);
				case FunctionOperatorType.ToInt:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS int)", operands[0]);
				case FunctionOperatorType.ToLong:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS bigint)", operands[0]);
				case FunctionOperatorType.ToFloat:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS real)", operands[0]);
				case FunctionOperatorType.ToDouble:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS double precision)", operands[0]);
				case FunctionOperatorType.ToDecimal:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS decimal)", operands[0]);				
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) || '')", operands[0]);
				case FunctionOperatorType.Insert:
					return string.Format(CultureInfo.InvariantCulture, "(substr({0}, 0, ({1}) + 1) || {2} || substr({0}, ({1}) + 1))", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.PadLeft:
					return FnLpad(operands);
				case FunctionOperatorType.PadRight:
					return FnRpad(operands);
				case FunctionOperatorType.Today:
					return "current_date";
				case FunctionOperatorType.Now:
					return "now()";
				case FunctionOperatorType.UtcNow:
					return "(now() at time zone 'UTC')";
				case FunctionOperatorType.GetMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "round(floor((extract(millisecond from {0}) / 1000 - floor(extract(millisecond from {0}) / 1000))::numeric(6, 6) * 1000000) / 1000)", operands[0]);
				case FunctionOperatorType.GetSecond:
					return string.Format(CultureInfo.InvariantCulture, "floor(extract(second from {0}))", operands[0]);
				case FunctionOperatorType.GetMinute:
					return string.Format(CultureInfo.InvariantCulture, "extract(minute from {0})", operands[0]);
				case FunctionOperatorType.GetHour:
					return string.Format(CultureInfo.InvariantCulture, "extract(hour from {0})", operands[0]);
				case FunctionOperatorType.GetDay:
					return string.Format(CultureInfo.InvariantCulture, "extract(day from {0})", operands[0]);
				case FunctionOperatorType.GetMonth:
					return string.Format(CultureInfo.InvariantCulture, "extract(month from {0})", operands[0]);
				case FunctionOperatorType.GetYear:
					return string.Format(CultureInfo.InvariantCulture, "extract(year from {0})", operands[0]);
				case FunctionOperatorType.GetTimeOfDay:
					return string.Format(CultureInfo.InvariantCulture, "floor(extract(hour from {0}) * 36000000000 + extract(minute from {0}) * 600000000 + extract(second from {0}) * 10000000)", operands[0]);
				case FunctionOperatorType.GetDayOfWeek:
					return string.Format(CultureInfo.InvariantCulture, "extract(dow from {0})", operands[0]);
				case FunctionOperatorType.GetDayOfYear:
					return string.Format(CultureInfo.InvariantCulture, "extract(doy from {0})", operands[0]);
				case FunctionOperatorType.GetDate:
					return string.Format(CultureInfo.InvariantCulture, "({0})::date", operands[0]);
				case FunctionOperatorType.AddTicks:
					if(SupportVersion(8.3m, 0)) {
						return string.Format(CultureInfo.InvariantCulture, "(({0}) + ((({1}) / 10000) || ' millisecond')::interval)", operands[0], operands[1]);
					}
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + ((({1}) / 10000000) || ' second')::interval)", operands[0], operands[1]);
				case FunctionOperatorType.AddMilliSeconds:
					if(SupportVersion(8.3m, 0)) {
						return string.Format(CultureInfo.InvariantCulture, "(({0}) + (({1}) || ' millisecond')::interval)", operands[0], operands[1]);
					}
					throw new NotSupportedException();
				case FunctionOperatorType.AddTimeSpan:
				case FunctionOperatorType.AddSeconds:
					return FnAddDateTime(operands[0], FormatGetInt(operands[1], 1000, 86400000), FormatMod(operands[1], 1, 86400), FormatMod(operands[1], 1000, 1000, false));
				case FunctionOperatorType.AddMinutes:
					return FnAddDateTime(operands[0], FormatGetInt(operands[1], 60000, 86400000), FormatMod(operands[1], 60, 86400), FormatMod(operands[1], 60000, 1000, false));
				case FunctionOperatorType.AddHours:
					return FnAddDateTime(operands[0], FormatGetInt(operands[1], 3600000, 86400000), FormatMod(operands[1], 3600, 86400), FormatMod(operands[1], 3600000, 1000, false));
				case FunctionOperatorType.AddDays:
					return FnAddDateTime(operands[0], FormatGetInt(operands[1], 86400000, 86400000), FormatMod(operands[1], 86400, 86400), FormatMod(operands[1], 86400000, 1000, false));
				case FunctionOperatorType.AddMonths:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + (({1}) || ' month')::interval)", operands[0], operands[1]);
				case FunctionOperatorType.AddYears:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + (({1}) || ' year')::interval)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffYear:
					return string.Format(CultureInfo.InvariantCulture, "(extract(year from ({1})) - extract(year from ({0})))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMonth:
					return string.Format(CultureInfo.InvariantCulture, "(((extract(year from ({1})) - extract(year from ({0}))) * 12) + extract(month from ({1})) - extract(month from ({0})))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffDay:
					return string.Format(CultureInfo.InvariantCulture, "(extract(epoch from (date_trunc('day', ({1})) - date_trunc('day', ({0})))) / 86400)::int", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffHour:
					return string.Format(CultureInfo.InvariantCulture, "(extract(epoch from (date_trunc('hour', ({1})) - date_trunc('hour', ({0})))) / 3600)::int", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMinute:
					return string.Format(CultureInfo.InvariantCulture, "(extract(epoch from (date_trunc('minute', ({1})) - date_trunc('minute', ({0})))) / 60)::int", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffSecond:
					return string.Format(CultureInfo.InvariantCulture, "(extract(epoch from (date_trunc('second', ({1})) - date_trunc('second', ({0})))))::int", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "(extract(epoch from (date_trunc('millisecond', ({1})) - date_trunc('millisecond', ({0})))) * 1000)::int", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffTick:
					return string.Format(CultureInfo.InvariantCulture, "(extract(epoch from (date_trunc('millisecond', ({1})) - date_trunc('millisecond', ({0})))) * 10000000)::int", operands[0], operands[1]);
				case FunctionOperatorType.IsNull:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "(({0}) is null)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "COALESCE({0}, {1})", operands[0], operands[1]);
					}
					break;
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) is null or length({0}) = 0)", operands[0]);
				case FunctionOperatorType.EndsWith:
					return string.Format(CultureInfo.InvariantCulture, "(Substr({0}, Length({0}) - Length({1}) + 1) = ({1}))", operands[0], operands[1]);
				case FunctionOperatorType.Contains:
					return string.Format(CultureInfo.InvariantCulture, "(Strpos({0}, {1}) > 0)", operands[0], operands[1]);
			}
			return base.FormatFunction(operatorType, operands);
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
		string FnLpad(string[] operands) {
			if(operands.Length == 2) {
				return string.Format(CultureInfo.InvariantCulture, "case when length({0}) > {1} then {0} else lpad({0}, {1}, ' ') end", operands[0], operands[1]);
			}
			if(operands.Length == 3) {
				return string.Format(CultureInfo.InvariantCulture, "case when length({0}) > {1} then {0} else lpad({0}, {1}, {2}) end", operands[0], operands[1], operands[2]);
			}
			throw new NotSupportedException();
		}
		string FnRpad(string[] operands) {
			if(operands.Length == 2) {
				return string.Format(CultureInfo.InvariantCulture, "case when length({0}) > {1} then {0} else rpad({0}, {1}, ' ') end", operands[0], operands[1]);
			}
			if(operands.Length == 3) {
				return string.Format(CultureInfo.InvariantCulture, "case when length({0}) > {1} then {0} else rpad({0}, {1}, {2}) end", operands[0], operands[1], operands[2]);
			}
			throw new NotSupportedException();
		}
		string FnRemove(string[] operands) {
			if(operands.Length == 3) {
				return string.Format(CultureInfo.InvariantCulture, "(substr({0}, 0, {1} + 1) || substr({0}, {1} + {2} + 1))", operands[0], operands[1], operands[2]);
			}
			if(operands.Length == 2) {
				return string.Format(CultureInfo.InvariantCulture, "substr({0}, 0, {1} + 1)", operands[0], operands[1]);
			}
			throw new NotSupportedException();
		}
		string FnLog(string[] operands) {
			if(operands.Length == 1) {
				return string.Format(CultureInfo.InvariantCulture, "ln({0})", operands[0]);
			}
			if(operands.Length == 2) {
				return string.Format(CultureInfo.InvariantCulture, "(ln({0}) / ln({1}))", operands[0], operands[1]);
			}
			throw new NotSupportedException();
		}
		string FnCharIndex(string[] operands) {
			if(operands.Length == 2) {
				return string.Format(CultureInfo.InvariantCulture, "(position({0} in {1}) - 1)", operands[0], operands[1]);
			}
			if(operands.Length == 3) {
				return string.Format(CultureInfo.InvariantCulture, "(case position({0} in substring({1}, {2} + 1)) > 0 when true then position({0} in substring({1}, {2} + 1)) + {2} - 1 else -1 end)", operands[0], operands[1], operands[2]);
			}
			if(operands.Length == 4) {
				return string.Format(CultureInfo.InvariantCulture, "(case position({0} in substring({1}, {2} + 1, {3})) > 0 when true then position({0} in substring({1}, {2} + 1, {3})) + {2} - 1 else -1 end)", operands[0], operands[1], operands[2], operands[3]);
			}
			throw new NotSupportedException();
		}
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			object value = parameter.Value;
			createParameter = false;
			if(parameter is ConstantValue && value != null) {
				switch(Type.GetTypeCode(value.GetType())) {
					case TypeCode.Int32:
						return ((int)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.Boolean:
						return (bool)value ? "true" : "false";
					case TypeCode.String: {
						return string.Concat(SupportVersion(8.1m, 0) ? "E'" : "'", ((string)value).Replace("'", "''").Replace(@"\", @"\\"), "'");
					}
				}
			}
			createParameter = true;
			return "@p" + index.ToString(CultureInfo.InvariantCulture);
		}
		public override string FormatConstraint(string constraintName) {
			return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", ComposeSafeConstraintName(constraintName));
		}
		public override string FormatOrder(string sortProperty, SortingDirection direction) {
			if(SupportVersion(8.3m, 0)) {
				return string.Format(CultureInfo.InvariantCulture, "{0} {1}", sortProperty, 
					direction == SortingDirection.Ascending ? "asc nulls first" : "desc nulls last");
			} else {
				return string.Format(CultureInfo.InvariantCulture, "{0} {1}", sortProperty, 
					direction == SortingDirection.Ascending ? "asc" : "desc");
			}
		}
		public void ClearDatabase(IDbCommand command) {
			Query query = new Query("select CONSTRAINT_NAME, TABLE_NAME, TABLE_SCHEMA from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'FOREIGN KEY'");
			SelectStatementResult constraints = SelectData(query);
			foreach(SelectStatementResultRow row in constraints.Rows) {
				string constraintName = ((string)row.Values[0]).Trim();
				string tableName = ((string)row.Values[1]).Trim();
				string schema = ((string)row.Values[2]).Trim();
				command.CommandText = string.Format("alter table {0} drop constraint {1}", FormatTable(schema, tableName), FormatConstraint(constraintName));
				command.ExecuteNonQuery();
			}
			query = new Query("select TABLE_NAME, TABLE_SCHEMA from INFORMATION_SCHEMA.VIEWS where TABLE_SCHEMA <> 'information_schema' and TABLE_SCHEMA <> 'pg_catalog'");
			SelectStatementResult views = SelectData(query);
			foreach(SelectStatementResultRow row in views.Rows) {
				string tableName = ((string)row.Values[0]).Trim();
				string schema = ((string)row.Values[1]).Trim();
				command.CommandText = string.Format("drop view {0}", FormatTable(schema, tableName));
				command.ExecuteNonQuery();
			}
			string[] tables = GetStorageTablesList(false);
			foreach(string table in tables) {
				string schema = GetSchemaName(table);
				string tableName = GetTableName(table);
				command.CommandText = string.Format("drop table {0}", FormatTable(schema, tableName));
				command.ExecuteNonQuery();
			}
		} 
		string GetSchemaName(string table) {
			int dot = table.IndexOf('.');
			if (dot > 0) { return table.Substring(0, dot); }
			return String.Empty;
		}
		string GetTableName(string table) {
			int dot = table.IndexOf('.');
			if (dot > 0) return table.Remove(0, dot + 1);
			return table;
		}
		protected override void ProcessClearDatabase() {
			IDbCommand command = CreateCommand();
			ClearDatabase(command);
		}
		public override string[] GetStorageTablesList(bool includeViews) {						
			Query query = new Query(string.Format(@"select TABLE_NAME, TABLE_SCHEMA from INFORMATION_SCHEMA.TABLES 
where (TABLE_TYPE = 'BASE TABLE' {0}) and TABLE_SCHEMA <> 'information_schema' and TABLE_SCHEMA <> 'pg_catalog'", includeViews ? " or TABLE_TYPE = 'VIEW'" : ""));
			SelectStatementResult tables = SelectData(query);
			List<string> result = new List<string>(tables.Rows.Length);
			foreach(SelectStatementResultRow row in tables.Rows) {
				string objectName = (string)row.Values[0];
				string owner = (string)row.Values[1];
				if (ObjectsOwner != owner && owner != null)
					result.Add(string.Concat(owner, ".", objectName));
				else
					result.Add(objectName);				
			}
			return result.ToArray();
		}
		public string ObjectsOwner = "public";
		void GenerateView(DBTable table, StringBuilder result) {
			string objName = ComposeSafeTableName(string.Format("{0}_xpoView", table.Name));
			result.AppendLine(string.Format("CREATE VIEW \"{0}\"(", objName));
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { result.AppendLine(","); }
				result.Append(string.Format("\t\"{0}\"", table.Columns[i].Name));
			}
			result.AppendLine();
			result.AppendLine(")");
			result.AppendLine("AS");
			result.AppendLine("\tSELECT");
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { result.AppendLine(","); }
				result.Append(string.Format("\t\t\"{0}\"", table.Columns[i].Name));
			}
			result.AppendLine();
			result.AppendLine(string.Format("\tFROM \"{0}\";", table.Name));
		}
		void GenerateInsertSP(DBTable table, StringBuilder result) {
			string objName = ComposeSafeTableName(string.Format("sp_{0}_xpoView_insert", table.Name));
			result.AppendLine(string.Format("CREATE FUNCTION \"{0}\"(", objName));
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { result.AppendLine(","); }
				string dbType = GetRawType(GetSqlCreateColumnType(table, table.Columns[i]));
				result.Append(string.Format("\t{0}_ {1}", table.Columns[i].Name, dbType));
			}
			result.AppendLine();
			result.AppendLine(")");
			result.AppendLine("RETURNS VOID");
			result.AppendLine("AS $$");
			result.AppendLine(string.Format("\tINSERT INTO \"{0}\"(", table.Name));
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { result.AppendLine(","); }
				result.Append(string.Format("\t\t\"{0}\"", table.Columns[i].Name));
			}
			result.AppendLine();
			result.AppendLine("\t)");
			result.AppendLine("\tVALUES(");
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { result.AppendLine(","); }
				result.Append(string.Format("\t\t${0}", (i + 1).ToString(CultureInfo.InvariantCulture)));
			}
			result.AppendLine();
			result.AppendLine("\t);");
			result.AppendLine("$$ LANGUAGE SQL;");
		}
		void GenerateUpdateSP(DBTable table, StringBuilder result) {
			string objName = ComposeSafeTableName(string.Format("sp_{0}_xpoView_update", table.Name));
			result.AppendLine(string.Format("CREATE FUNCTION \"{0}\"(", objName));
			AppendKeys(table, result);
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				if(i != 0 || table.PrimaryKey.Columns.Count > 0) { result.AppendLine(","); }
				string dbType = GetRawType(GetSqlCreateColumnType(table, table.Columns[i]));
				result.AppendLine(string.Format("\told_{0} {1},", table.Columns[i].Name, dbType));
				result.Append(string.Format("\t{0}_ {1}", table.Columns[i].Name, dbType));
			}
			result.AppendLine();
			result.AppendLine(")");
			result.AppendLine("RETURNS VOID");
			result.AppendLine("AS $$");
			result.AppendLine(string.Format("\tUPDATE \"{0}\" SET", table.Name));
			bool first = true;
			int paramNum = 0;
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				if(first) { first = false; } else { result.AppendLine(","); }
				result.Append(string.Format("\t\t\"{0}\"=${1}", table.Columns[i].Name, ((paramNum++) * 2) + table.PrimaryKey.Columns.Count + 2));
			}
			result.AppendLine();
			result.AppendLine("\tWHERE");
			AppendWhere(table, result);
			result.AppendLine("$$ LANGUAGE SQL;");
		}
		void GenerateDeleteSP(DBTable table, StringBuilder result) {
			string objName = ComposeSafeTableName(string.Format("sp_{0}_xpoView_delete", table.Name));
			result.AppendLine(string.Format("CREATE FUNCTION \"{0}\"(", objName));
			AppendKeys(table, result);
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				if(i != 0 || table.PrimaryKey.Columns.Count > 0) { result.AppendLine(","); }
				string dbType = GetSqlCreateColumnType(table, table.Columns[i]);
				result.Append(string.Format("\told_{0} {1}", table.Columns[i].Name, GetRawType(dbType)));
			}
			result.AppendLine();
			result.AppendLine(")");
			result.AppendLine("RETURNS VOID");
			result.AppendLine("AS $$");
			result.AppendLine(string.Format("\tDELETE FROM \"{0}\" WHERE", table.Name));
			AppendWhere(table, result);
			result.AppendLine("$$ LANGUAGE SQL;");
		}
		void GenerateInsteadOfInsertRule(DBTable table, StringBuilder result) {
			string ruleName = ComposeSafeTableName(string.Format("r_{0}_xpoView_insert", table.Name));
			result.AppendLine(string.Format("CREATE RULE \"{0}\" AS", ruleName));
			string viewName = ComposeSafeTableName(string.Format("{0}_xpoView", table.Name));
			result.AppendLine(string.Format("ON INSERT TO \"{0}\" DO INSTEAD", viewName));
			string spName = ComposeSafeTableName(string.Format("sp_{0}_xpoView_insert", table.Name));
			result.AppendLine(string.Format("\tSELECT \"{0}\"(", spName));
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { result.AppendLine(","); }
				result.Append(string.Format("\t\tnew.\"{0}\"", table.Columns[i].Name));
			}
			result.AppendLine();
			result.AppendLine("\t);");
		}
		void GenerateInsteadOfUpdateRule(DBTable table, StringBuilder result) {
			string ruleName = ComposeSafeTableName(string.Format("r_{0}_xpoView_update", table.Name));
			result.AppendLine(string.Format("CREATE RULE \"{0}\" AS", ruleName));
			string viewName = ComposeSafeTableName(string.Format("{0}_xpoView", table.Name));
			result.AppendLine(string.Format("ON UPDATE TO \"{0}\" DO INSTEAD", viewName));
			string spName = ComposeSafeTableName(string.Format("sp_{0}_xpoView_update", table.Name));
			result.AppendLine(string.Format("\tSELECT \"{0}\"(", spName));
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { result.AppendLine(","); }
				result.Append(string.Format("\t\tnew.\"{0}\"", table.PrimaryKey.Columns[i]));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				if(i != 0 || table.PrimaryKey.Columns.Count > 0) { result.AppendLine(","); }
				result.AppendLine(string.Format("\t\told.\"{0}\",", table.Columns[i].Name));
				result.Append(string.Format("\t\tnew.\"{0}\"", table.Columns[i].Name));
			}
			result.AppendLine();
			result.AppendLine("\t);");
		}
		void GenerateInsteadOfDeleteRule(DBTable table, StringBuilder result) {
			string ruleName = ComposeSafeTableName(string.Format("t_{0}_xpoView_delete", table.Name));
			result.AppendLine(string.Format("CREATE RULE \"{0}\" AS", ruleName));
			string viewName = ComposeSafeTableName(string.Format("{0}_xpoView", table.Name));
			result.AppendLine(string.Format("ON DELETE TO \"{0}\" DO INSTEAD", viewName));
			string spName = ComposeSafeTableName(string.Format("sp_{0}_xpoView_delete", table.Name));
			result.AppendLine(string.Format("\tSELECT \"{0}\"(", spName));
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { result.AppendLine(","); }
				result.Append(string.Format("\t\told.\"{0}\"", table.PrimaryKey.Columns[i]));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				if(i != 0 || table.PrimaryKey.Columns.Count > 0) { result.AppendLine(","); }
				result.Append(string.Format("\t\told.\"{0}\"", table.Columns[i].Name));
			}
			result.AppendLine();
			result.AppendLine("\t);");
		}
		static string GetRawType(string type) {
			int braceId = type.IndexOf('(');
			if(braceId < 0) { return type; }
			return type.Substring(0, braceId);
		}
		void AppendKeys(DBTable table, StringBuilder result) {
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { result.AppendLine(","); }
				DBColumn keyColumn = GetDbColumnByName(table, table.PrimaryKey.Columns[i]);
				string dbType = GetSqlCreateColumnType(table, keyColumn);
				result.Append(string.Format("\t{0}_ {1}", keyColumn.Name, GetRawType(dbType)));
			}
		}
		void AppendWhere(DBTable table, StringBuilder result) {
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { result.AppendLine(" AND"); }
				result.Append(string.Format("\t\t\"{0}\" = ${1}", table.PrimaryKey.Columns[i], i + 1));
			}
			result.AppendLine();
			result.AppendLine("\t;");
		}
		ExecMethodDelegate commandBuilderDeriveParametersHandler;
		protected override void CommandBuilderDeriveParameters(IDbCommand command) {
			if(commandBuilderDeriveParametersHandler == null) {
				commandBuilderDeriveParametersHandler = ReflectConnectionHelper.GetCommandBuilderDeriveParametersDelegate("Npgsql", "Npgsql.NpgsqlCommandBuilder");
			}
			commandBuilderDeriveParametersHandler(command);
		}
		public override DBStoredProcedure[] GetStoredProcedures() {
			List<DBStoredProcedure> result = new List<DBStoredProcedure>();
Query query = new Query(@"SELECT  proname 
FROM    pg_catalog.pg_namespace n
JOIN    pg_catalog.pg_proc p
ON      pronamespace = n.oid
WHERE   nspname = @p0",
				new QueryParameterCollection(new OperandValue(ObjectsOwner)), new string[] { "@p0" });
			SelectStatementResult data = SelectData(query);
			foreach(SelectStatementResultRow row in data.Rows) {
				result.Add(new DBStoredProcedure() {
					Name = Convert.ToString(row.Values[0])
				});
			}
			using(var command = Connection.CreateCommand()) {
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
							dbArguments.Add(new DBStoredProcedureArgument(parameter.ParameterName, columnType, direction));
							fakeParams.Add("null");
						}
						sproc.Arguments.AddRange(dbArguments);
						command.CommandType = CommandType.Text;
						command.CommandText = string.Format("select * from {0}({1}) where 1=0", sproc.Name, string.Join(", ", fakeParams.ToArray()));
						DBStoredProcedureResultSet curResultSet = new DBStoredProcedureResultSet();
						using(var reader = command.ExecuteReader()) {
							List<DBNameTypePair> dbColumns = new List<DBNameTypePair>();
							for(int i = 0; i < reader.FieldCount; i++) {
								DBColumnType columnType = DBColumn.GetColumnType(reader.GetFieldType(i));
								dbColumns.Add(new DBNameTypePair(reader.GetName(i), columnType));
							}
							curResultSet.Columns.AddRange(dbColumns);
						}
						sproc.ResultSets.Add(curResultSet);
					} catch { }
				}
			}
			return result.ToArray();
		}
	}
	public class PostgreSqlProviderFactory : ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return PostgreSqlConnectionProvider.CreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return PostgreSqlConnectionProvider.CreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(ServerParamID) || !parameters.ContainsKey(DatabaseParamID) ||
				!parameters.ContainsKey(UserIDParamID) || !parameters.ContainsKey(PasswordParamID)) {
				return null;
			}
			return PostgreSqlConnectionProvider.GetConnectionString(parameters[ServerParamID], parameters[UserIDParamID],
				parameters[PasswordParamID], parameters[DatabaseParamID]);
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
		public override string ProviderKey { get { return PostgreSqlConnectionProvider.XpoProviderTypeString; } }
		public override string[] GetDatabases(string server, string userid, string password) {
			return new string[0];
		}
		public override string FileFilter { get { return null; } }
		public override bool MeanSchemaGeneration { get { return true; } }
		public override bool SupportStoredProcedures { get { return false; } }
	}
}
