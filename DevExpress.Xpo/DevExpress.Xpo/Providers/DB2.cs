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
	public class DB2ConnectionProvider : ConnectionProviderSql {
		public const string XpoProviderTypeString = "DB2";
		public static string DefaultObjectsOwner = "";
		public string ObjectsOwner = DefaultObjectsOwner;
		string currentUser;
		ReflectConnectionHelper helper;
		ReflectConnectionHelper ConnectionHelper {
			get {
				if(helper == null)
					helper = new ReflectConnectionHelper(Connection, "IBM.Data.DB2.DB2Exception");
				return helper;
			}
		}
		public static string GetConnectionString(string server, string database, string userid, string password) {
			return String.Format("{4}={5};server={0};user id={1};password={2};database={3};persist security info=true",
				server, userid, password, database, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString);
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] { connection };
			return CreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DB2ConnectionProvider(connection, autoCreateOption);
		}
		static DB2ConnectionProvider() {
			RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
			RegisterDataStoreProvider("DB2Connection", new DataStoreCreationFromConnectionDelegate(CreateProviderFromConnection));
			RegisterFactory(new DB2ProviderFactory());
		}
		public static void Register() { }
		public DB2ConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
			currentUser = GetCurrentUser();
		}
		public override string ComposeSafeSchemaName(string tableName) {
			string schemaName = base.ComposeSafeSchemaName(tableName);
			return CorrectSchemaName(schemaName);
		}
		string CorrectSchemaName(string schemaName) {
			return !string.IsNullOrEmpty(schemaName) ? schemaName : (string.IsNullOrEmpty(ObjectsOwner) ? currentUser : ObjectsOwner);
		}
		protected override string GetSqlCreateColumnTypeForBoolean(DBTable table, DBColumn column) {
			return "char(1)";
		}
		protected override string GetSqlCreateColumnTypeForByte(DBTable table, DBColumn column) {
			return "smallint";
		}
		protected override string GetSqlCreateColumnTypeForSByte(DBTable table, DBColumn column) {
			return "numeric(3,0)";
		}
		protected override string GetSqlCreateColumnTypeForChar(DBTable table, DBColumn column) {
			return "char(1)";
		}
		protected override string GetSqlCreateColumnTypeForDecimal(DBTable table, DBColumn column) {
			return "decimal(28,4)";
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
				return "clob";
		}
		protected override string GetSqlCreateColumnTypeForDateTime(DBTable table, DBColumn column) {
			return "timestamp";
		}
		protected override string GetSqlCreateColumnTypeForGuid(DBTable table, DBColumn column) {
			return "char(36)";
		}
		protected override string GetSqlCreateColumnTypeForByteArray(DBTable table, DBColumn column) {
			return "blob";
		}
		public override string GetSqlCreateColumnFullAttributes(DBTable table, DBColumn column) {
			string result = GetSqlCreateColumnType(table, column);
			if(column.IsKey)
				result += " NOT NULL";
			if(column.IsKey && column.IsIdentity && (column.ColumnType == DBColumnType.Int32 || column.ColumnType == DBColumnType.Int64) && IsSingleColumnPKColumn(table, column)) {
				result += " GENERATED BY DEFAULT AS IDENTITY";
			}
			return result;
		}
		protected override object ReformatReadValue(object value, ConnectionProviderSql.ReformatReadValueArgs args) {
			if(args.TargetTypeCode == TypeCode.Boolean && args.DbTypeCode == TypeCode.String)
				return ((string)value) != "0";
			return base.ReformatReadValue(value, args);
		}
		protected override Exception WrapException(Exception e, IDbCommand query) {
			object o;
			if(ConnectionHelper.TryGetExceptionProperty(e, "Errors", out o) && ((ICollection)o).Count > 0) {
				foreach(object error in (IEnumerable)o) {
					int nativeError = (int)ReflectConnectionHelper.GetPropertyValue(error, "NativeError");
					if(nativeError == -206 || nativeError == -204)
						return new SchemaCorrectionNeededException((string)ReflectConnectionHelper.GetPropertyValue(error, "Message"), e);
					if(nativeError == -803 || nativeError == -532)
						return new ConstraintViolationException(query.CommandText, GetParametersString(query), e);
				}
			}
			return base.WrapException(e, query);
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
				case TypeCode.Boolean:
					return ((Boolean)clientValue) ? "1" : "0";
			}
			return base.ConvertToDbParameter(clientValue, clientValueTypeCode);
		}
		protected override Int64 GetIdentity(Query sql) {
			object value = GetScalar(new Query(sql.Sql + ";select identity_val_local() from sysibm.sysdummy1", sql.Parameters, sql.ParametersNames));
			return (value as IConvertible).ToInt64(CultureInfo.InvariantCulture);
		}
		protected override IDbConnection CreateConnection() {
			return ConnectionHelper.GetConnection(ConnectionString);
		}
		static public IDbConnection CreateConnection(string connectionString) {
			return ReflectConnectionHelper.GetConnection("IBM.Data.DB2", "IBM.Data.DB2.DB2Connection", connectionString);
		}
		protected override void CreateDataBase() {
			try {
				Connection.Open();
			} catch(Exception e) {
				throw new UnableToOpenDatabaseException(XpoDefault.ConnectionStringRemovePassword(ConnectionString), e);
			}
		}		
		delegate bool TablesFilter(DBTable table);
		SelectStatementResult GetDataForTables(ICollection tables, TablesFilter filter, string queryText) {
			QueryParameterCollection parameters = new QueryParameterCollection();
			StringCollection inList = new StringCollection();
			int i = 0;
			foreach(DBTable table in tables) {
				if(filter == null || filter(table)) {
					parameters.Add(new OperandValue(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", ComposeSafeSchemaName(table.Name), ComposeSafeTableName(table.Name))));					
					inList.Add("@table" + i.ToString(CultureInfo.InvariantCulture));
					++i;
				}
			}
			if(inList.Count == 0)
				return new SelectStatementResult();
			return SelectData(new Query(string.Format(CultureInfo.InvariantCulture, queryText, StringListHelper.DelimitedText(inList, ",")), parameters, inList));
		}
		DBColumnType GetTypeFromString(string typeName, int size) {
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
				case "TIMESTAMP":
					return DBColumnType.DateTime;
				case "CLOB":
					return DBColumnType.String;
			}
			return DBColumnType.Unknown;
		}
		void GetColumns(DBTable table) {
			string safeSchemaName = ComposeSafeSchemaName(table.Name);			
			string safeTableName = ComposeSafeTableName(table.Name);
			foreach(SelectStatementResultRow row in SelectData(new Query("select COLNAME, TYPENAME, LENGTH, IDENTITY from SYSCAT.COLUMNS where TABSCHEMA = ? and TABNAME = ? ORDER BY COLNO", new QueryParameterCollection(new OperandValue(safeSchemaName), new OperandValue(safeTableName)), new string[] { "@p1", "@p2" })).Rows) {
				DBColumnType type = GetTypeFromString((string)row.Values[1], (int)row.Values[2]);
				DBColumn column = new DBColumn((string)row.Values[0], false, String.Empty, type == DBColumnType.String ? (int)row.Values[2] : 0, type);
				column.IsIdentity = row.Values[3].ToString() == "Y";
				table.AddColumn(column);
			}
		}
		void GetPrimaryKey(DBTable table) {
			string safeSchemaName = ComposeSafeSchemaName(table.Name);			
			string safeTableName = ComposeSafeTableName(table.Name);
			SelectStatementResult data = SelectData(new Query("select COLNAMES from SYSCAT.INDEXES where TABSCHEMA = ? and TABNAME = ? and UNIQUERULE = 'P'", new QueryParameterCollection(new OperandValue(safeSchemaName), new OperandValue(safeTableName)), new string[] { "@p1", "@p2" }));
			if(data.Rows.Length > 0) {
				StringCollection cols = new StringCollection();
				string[] colNames = ((string)data.Rows[0].Values[0]).Split('+');
				for(int i = 1; i < colNames.Length; i++) {
					cols.Add(colNames[i]);
					DBColumn column = table.GetColumn(colNames[i]);
					if(column != null)
						column.IsKey = true;
				}
				table.PrimaryKey = new DBPrimaryKey(cols);
			}
		}
		void GetIndexes(DBTable table) {
			string safeSchemaName = ComposeSafeSchemaName(table.Name);
			string safeTableName = ComposeSafeTableName(table.Name);
			SelectStatementResult data = SelectData(new Query("select COLNAMES, UNIQUERULE from SYSCAT.INDEXES where TABSCHEMA = ? and TABNAME = ?", new QueryParameterCollection(new OperandValue(safeSchemaName), new OperandValue(safeTableName)), new string[] { "@p1", "@p2" }));
			foreach(SelectStatementResultRow row in data.Rows) {
				StringCollection cols = new StringCollection();
				string[] colNames = ((string)row.Values[0]).Split('+');
				for(int i = 1; i < colNames.Length; i++)
					cols.Add(colNames[i]);
				string unique = row.Values[1] as string;
				bool isUnique = false;
				switch(unique){
					case "P":
					case "U":
						isUnique = true;
						break;
				}
				table.Indexes.Add(new DBIndex(cols,  isUnique));
			}
		}
		void GetForeignKeys(DBTable table) {
			string safeSchemaName = ComposeSafeSchemaName(table.Name);
			string safeTableName = ComposeSafeTableName(table.Name);
			SelectStatementResult data = SelectData(new Query("select REFTABNAME, FK_COLNAMES, PK_COLNAMES, REFTABSCHEMA from SYSCAT.REFERENCES where TABSCHEMA = ? and TABNAME = ?", new QueryParameterCollection(new OperandValue(safeSchemaName), new OperandValue(safeTableName)), new string[] { "@p1", "@p2" }));
			foreach(SelectStatementResultRow row in data.Rows) {
				StringCollection pkc = new StringCollection();
				StringCollection fkc = new StringCollection();
				string[] colNames = ((string)row.Values[1]).Split(' ');
				for(int i = 1; i < colNames.Length; i++)
					if(colNames[i] != String.Empty)
						fkc.Add(colNames[i]);
				colNames = ((string)row.Values[2]).Split(' ');
				for(int i = 1; i < colNames.Length; i++)
					if(colNames[i] != String.Empty)
						pkc.Add(colNames[i]);
				string rtable = (string)row.Values[0];
				string rschema = (string)row.Values[3];
				if(ObjectsOwner != rschema && !String.IsNullOrEmpty(rschema))
					rtable = rschema + "." + rtable;
				table.ForeignKeys.Add(new DBForeignKey(fkc, rtable, pkc));
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
			Hashtable dbTables = new Hashtable();						
			foreach(SelectStatementResultRow row in GetDataForTables(tables, null, "select TABNAME, TYPE from SYSCAT.TABLES where TRIM(TABSCHEMA) CONCAT '.' CONCAT TRIM(TABNAME) in ({0}) and TYPE in ('T', 'V')").Rows)
				dbTables.Add(row.Values[0], ((string)row.Values[1]) == "V");
			ArrayList list = new ArrayList();
			foreach(DBTable table in tables) {
				object o = dbTables[ComposeSafeTableName(table.Name)];
				if(o == null)
					list.Add(table);
				else
					table.IsView = (bool)o;
			}
			return list;
		}
		protected override int GetSafeNameTableMaxLength() {
			return 30;
		}
		protected override int GetSafeNameColumnMaxLength() {
			return 30;
		}
		protected override int GetSafeNameConstraintMaxLength() {
			return 18;
		}
		string FormatOwnedDBObject(string schema, string objectName) {
			return string.Concat("\"", CorrectSchemaName(schema), "\".\"", objectName, "\"");
		}
		public override string FormatTable(string schema, string tableName) {
			return FormatOwnedDBObject(schema, tableName);
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			return FormatOwnedDBObject(schema, tableName) + ' ' + tableAlias;
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
			string expandedWhereSql = whereSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}where {1}", Environment.NewLine, whereSql) : string.Empty;
			string expandedOrderBySql = orderBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}order by {1}", Environment.NewLine, orderBySql) : string.Empty;
			string expandedHavingSql = havingSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}having {1}", Environment.NewLine, havingSql) : string.Empty;
			string expandedGroupBySql = groupBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}group by {1}", Environment.NewLine, groupBySql) : string.Empty;
			string modificatorsSql = string.Empty;
			if(skipSelectedRecords == 0) {
				modificatorsSql = string.Format(CultureInfo.InvariantCulture, (topSelectedRecords != 0) ? " fetch first {0} rows only" : string.Empty, topSelectedRecords);
				return string.Format(CultureInfo.InvariantCulture, "select {1} from {2}{3}{4}{5}{6} {0}", modificatorsSql, selectedPropertiesSql, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedOrderBySql);
			}
			string[] fields = SimpleSqlParser.GetColumns(selectedPropertiesSql);
			StringBuilder expandedSelectedProperties = SimpleSqlParser.GetExpandedProperties(fields, "resultSet");
			selectedPropertiesSql = string.Join(", ", fields);
			string baseFormat = "select {8} from(select {0}, row_number() over({1}) as rowNumber from {4}{5}{6}{7})resultSet where resultSet.rowNumber > {2}";
			if(topSelectedRecords != 0) {
				baseFormat += " and resultSet.rowNumber <= {2} + {3}";
			}
			return string.Format(CultureInfo.InvariantCulture, baseFormat,
				selectedPropertiesSql, expandedOrderBySql, skipSelectedRecords, topSelectedRecords, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedSelectedProperties);
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
					return string.Format(CultureInfo.InvariantCulture, "MOD({0}, {1})", leftOperand, rightOperand);
				default:
					return base.FormatBinary(operatorType, leftOperand, rightOperand);
			}
		}
		public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.Concat:
					return FnConcat(operands);
				case FunctionOperatorType.Sqr:
					return string.Format(CultureInfo.InvariantCulture, "Sqrt({0})", operands[0]);
				case FunctionOperatorType.Log:
					return FnLog(operands);
				case FunctionOperatorType.Log10:
					return string.Format(CultureInfo.InvariantCulture, "log10({0})", operands[0]);
				case FunctionOperatorType.Acos:
					return string.Format(CultureInfo.InvariantCulture, "acos({0})", operands[0]);
				case FunctionOperatorType.Asin:
					return string.Format(CultureInfo.InvariantCulture, "asin({0})", operands[0]);
				case FunctionOperatorType.Atn:
					return string.Format(CultureInfo.InvariantCulture, "atan({0})", operands[0]);
				case FunctionOperatorType.Atn2:
					return string.Format(CultureInfo.InvariantCulture, "(case when ({0}) = 0 then (case when ({1}) >= 0 then 0 else atan(1) * 4 end) else 2 * atan(({0}) / (sqrt(({1}) * ({1}) + ({0}) * ({0})) + ({1}))) end)", operands[0], operands[1]);
				case FunctionOperatorType.Cosh:
					return string.Format(CultureInfo.InvariantCulture, "cosh({0})", operands[0]);
				case FunctionOperatorType.Sinh:
					return string.Format(CultureInfo.InvariantCulture, "sinh({0})", operands[0]);
				case FunctionOperatorType.Tanh:
					return string.Format(CultureInfo.InvariantCulture, "tanh({0})", operands[0]);
				case FunctionOperatorType.Round:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "trunc({0}, 0)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "trunc({0}, {1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Max:
					return string.Format(CultureInfo.InvariantCulture, "(case when {0} > {1} then {0} else {1} end)", operands[0], operands[1]);
				case FunctionOperatorType.Min:
					return string.Format(CultureInfo.InvariantCulture, "(case when {0} < {1} then {0} else {1} end)", operands[0], operands[1]);
				case FunctionOperatorType.Rnd:
					return "Rand()";
				case FunctionOperatorType.BigMul:
					return string.Format(CultureInfo.InvariantCulture, "(cast({0} as bigint) * cast({1} as bigint))", operands[0], operands[1]);
				case FunctionOperatorType.GetMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "(cast((microsecond({0}) / 1000) as int))", operands[0]);
				case FunctionOperatorType.GetSecond:
					return string.Format(CultureInfo.InvariantCulture, "second({0})", operands[0]);
				case FunctionOperatorType.GetMinute:
					return string.Format(CultureInfo.InvariantCulture, "minute({0})", operands[0]);
				case FunctionOperatorType.GetHour:
					return string.Format(CultureInfo.InvariantCulture, "hour({0})", operands[0]);
				case FunctionOperatorType.GetDay:
					return string.Format(CultureInfo.InvariantCulture, "day({0})", operands[0]);
				case FunctionOperatorType.GetMonth:
					return string.Format(CultureInfo.InvariantCulture, "month({0})", operands[0]);
				case FunctionOperatorType.GetYear:
					return string.Format(CultureInfo.InvariantCulture, "year({0})", operands[0]);
				case FunctionOperatorType.GetTimeOfDay:
					return string.Format(CultureInfo.InvariantCulture, "(cast(hour({0}) as bigint) * 36000000000 + cast(minute({0}) as bigint) * 600000000 + cast(second({0}) as bigint) * 10000000 + cast(microsecond({0}) as bigint) * 10)", operands[0]);
				case FunctionOperatorType.GetDayOfWeek:
					return string.Format(CultureInfo.InvariantCulture, "(dayofweek({0}) - 1)", operands[0]);
				case FunctionOperatorType.GetDayOfYear:
					return string.Format(CultureInfo.InvariantCulture, "dayofyear({0})", operands[0]);
				case FunctionOperatorType.GetDate:
					return string.Format(CultureInfo.InvariantCulture, "date({0})", operands[0]);
				case FunctionOperatorType.Ascii:
					return string.Format(CultureInfo.InvariantCulture, "ascii({0})", operands[0]);
				case FunctionOperatorType.Char:
					return string.Format(CultureInfo.InvariantCulture, "chr({0})", operands[0]);
				case FunctionOperatorType.IsNull:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "(({0}) is null)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "COALESCE({0}, {1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) is null or length({0}) = 0)", operands[0]);
				case FunctionOperatorType.ToInt:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS int)", operands[0]);
				case FunctionOperatorType.ToLong:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS bigint)", operands[0]);
				case FunctionOperatorType.ToFloat:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS real)", operands[0]);
				case FunctionOperatorType.ToDouble:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS double precision)", operands[0]);
				case FunctionOperatorType.ToDecimal:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS decimal(28,4))", operands[0]);
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, "char({0})", operands[0]);
				case FunctionOperatorType.Remove:
					return FnRemove(operands);
				case FunctionOperatorType.Insert:
					return string.Format(CultureInfo.InvariantCulture, "(substr({0}, 1, {1}) || ({2}) || substr({0}, ({1}) + 1, length({0}) - ({1})))", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Len:
					return string.Format(CultureInfo.InvariantCulture, "length({0})", operands[0]);
				case FunctionOperatorType.CharIndex:
					return FnCharIndex(operands);
				case FunctionOperatorType.Substring:
					string len = operands.Length < 3 ? "length(" + operands[0] + ")" + " - " + operands[1] : operands[2];
					return string.Format(CultureInfo.InvariantCulture, "substr({0}, ({1}) + 1, {2})", operands[0], operands[1], len);
				case FunctionOperatorType.PadLeft:
					return FnPadLeft(operands);
				case FunctionOperatorType.PadRight:
					return FnPadRight(operands);
				case FunctionOperatorType.Trim:
					return string.Format(CultureInfo.InvariantCulture, "trim({0})", operands[0]);
				case FunctionOperatorType.AddTicks:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + (cast(({1}) as bigint) / 10) microseconds)", operands[0], operands[1]);
				case FunctionOperatorType.AddMilliSeconds:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + (cast(({1}) as double precision) * 1000) microseconds)", operands[0], operands[1]);
				case FunctionOperatorType.AddTimeSpan:
				case FunctionOperatorType.AddSeconds:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + (cast(({1}) as double precision) * 1000 / 86400000) days + MOD((cast(({1}) as double precision) * 1000000), 86400000000) microseconds)", operands[0], operands[1]);
				case FunctionOperatorType.AddMinutes:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + (cast(({1}) as double precision) * 60000 / 86400000) days + MOD((cast(({1}) as double precision) * 60000000), 86400000000) microseconds)", operands[0], operands[1]);
				case FunctionOperatorType.AddHours:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + (cast(({1}) as double precision) * 3600000 / 86400000) days + MOD((cast(({1}) as double precision) * 3600000000), 86400000000) microseconds)", operands[0], operands[1]);
				case FunctionOperatorType.AddDays:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + cast(({1}) as double precision) days + MOD((cast(({1}) as double precision) * 86400000000), 86400000000) microseconds)", operands[0], operands[1]);
				case FunctionOperatorType.AddMonths:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + ({1}) months)", operands[0], operands[1]);
				case FunctionOperatorType.AddYears:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + ({1}) years)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffYear:
					return string.Format(CultureInfo.InvariantCulture, "(year({1}) - year({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMonth:
					return string.Format(CultureInfo.InvariantCulture, "(((year({1}) - year({0})) * 12) + month({1}) - month({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffDay:
					return string.Format(CultureInfo.InvariantCulture, "(DAYS({1}) - DAYS({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffHour:
					return string.Format(CultureInfo.InvariantCulture, "(((DAYS({1}) - DAYS({0})) * 24) + hour({1}) - hour({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMinute:
					return string.Format(CultureInfo.InvariantCulture, "((((DAYS({1}) - DAYS({0})) * 24) + hour({1}) - hour({0})) * 60 + minute({1}) - minute({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffSecond:
					return string.Format(CultureInfo.InvariantCulture, "((DAYS({1}) - DAYS({0})) * 86400 + (MIDNIGHT_SECONDS({1}) - MIDNIGHT_SECONDS({0})))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "(((DAYS({1}) - DAYS({0})) * 86400 + (MIDNIGHT_SECONDS({1}) - MIDNIGHT_SECONDS({0}))) * 1000 + cast(((microsecond({1}) - microsecond({0})) / 1000) as int))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffTick:
					return string.Format(CultureInfo.InvariantCulture, "(((DAYS({1}) - DAYS({0})) * 86400 + (MIDNIGHT_SECONDS({1}) - MIDNIGHT_SECONDS({0}))) * 10000000 + ((microsecond({1}) - microsecond({0})) * 10))", operands[0], operands[1]);
				case FunctionOperatorType.Now:
					return "(current timestamp)";
				case FunctionOperatorType.Today:
					return "(current date)";
				case FunctionOperatorType.UtcNow:
					throw new NotSupportedException("UtcNow");
				case FunctionOperatorType.Contains:
					return string.Format(CultureInfo.InvariantCulture, "(Position({1}, {0}, CODEUNITS32) > 0)", operands[0], operands[1]);
				case FunctionOperatorType.EndsWith:
					return string.Format(CultureInfo.InvariantCulture, "(Right({0}, Length({1})) = ({1}))", operands[0], operands[1]);
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
							return string.Format(CultureInfo.InvariantCulture, "(({0} like {2}) And (Position({1}, {0}, CODEUNITS32) = 1))", processParameter(operands[0]), processParameter(secondOperand), processParameter(new ConstantValue(operandString.Substring(0, likeIndex) + "%")));
						}
					}
					return string.Format(CultureInfo.InvariantCulture, "(Position({1}, {0}, CODEUNITS32) = 1)", processParameter(operands[0]), processParameter(operands[1]));
				default:
					return base.FormatFunction(processParameter, operatorType, operands);
			}
		}
		string FnPadLeft(string[] operands) {
			if(operands.Length == 2) {
				return string.Format(CultureInfo.InvariantCulture, "case when length({0}) > ({1}) then {0} else repeat(' ', ({1}) - length({0})) || ({0}) end", operands[0], operands[1]);
			}
			throw new NotSupportedException();
		}
		string FnPadRight(string[] operands) {
			if(operands.Length == 2) {
				return string.Format(CultureInfo.InvariantCulture, "case when length({0}) > ({1}) then {0} else ({0}) || repeat(' ', ({1}) - length({0})) end", operands[0], operands[1]);
			}
			throw new NotSupportedException();
		}
		string FnCharIndex(string[] operands) {
			if(operands.Length == 2) {
				return string.Format(CultureInfo.InvariantCulture, "(position({0}, {1}, CODEUNITS32) - 1)", operands[0], operands[1]);
			}
			if(operands.Length == 3) {
				return string.Format(CultureInfo.InvariantCulture, "(case when position({0}, substr({1}, ({2}) + 1), CODEUNITS32) > 0 then position({0}, substr({1}, ({2}) + 1), CODEUNITS32) + ({2}) - 1 else -1 end)", operands[0], operands[1], operands[2]);
			}
			if(operands.Length == 4) {
				return string.Format(CultureInfo.InvariantCulture, "(case when position({0}, substr({1}, ({2}) + 1, {3}), CODEUNITS32) > 0 then position({0}, substr({1}, ({2}) + 1, {3}), CODEUNITS32) + ({2}) - 1 else -1 end)", operands[0], operands[1], operands[2], operands[3]);
			}
			throw new NotSupportedException();
		}
		string FnLog(string[] operands) {
			if(operands.Length == 1) {
				return string.Format(CultureInfo.InvariantCulture, "Ln({0})", operands[0]);
			}
			if(operands.Length == 2) {
				return string.Format(CultureInfo.InvariantCulture, "(Ln({0}) / Ln({1}))", operands[0], operands[1]);
			}
			throw new NotSupportedException();
		}
		string FnRemove(string[] operands) {
			if(operands.Length == 3) {
				return string.Format(CultureInfo.InvariantCulture, "(substr({0}, 1, {1}) || substr({0}, ({1}) + 1 + ({2})))", operands[0], operands[1], operands[2]);
			}
			if(operands.Length == 2) {
				return string.Format(CultureInfo.InvariantCulture, "substr({0}, 1, {1})", operands[0], operands[1]);
			}
			throw new NotSupportedException();
		}
		string FnConcat(string[] operands) {
			string args = String.Empty;
			foreach(string arg in operands) {
				if(args.Length > 0)
					args += " || ";
				args += arg;
			}
			return args;
		}
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			if(parameter.Value == null) { return null; }
			createParameter = false;
			object value = parameter.Value;
			TypeCode valueTypeCode = Type.GetTypeCode(parameter.Value.GetType());
			switch(valueTypeCode) {
				case TypeCode.Byte:
					return ((byte)parameter.Value).ToString(CultureInfo.InvariantCulture);
				case TypeCode.UInt16:
					return ((UInt16)parameter.Value).ToString(CultureInfo.InvariantCulture);
				case TypeCode.UInt32:
					return ((UInt32)parameter.Value).ToString(CultureInfo.InvariantCulture);
				case TypeCode.UInt64:
					return ((UInt64)parameter.Value).ToString(CultureInfo.InvariantCulture);
				case TypeCode.Int16:
					return ((Int16)parameter.Value).ToString(CultureInfo.InvariantCulture);
				case TypeCode.Int32:
					return ((Int32)parameter.Value).ToString(CultureInfo.InvariantCulture);
				case TypeCode.Int64:
					return ((Int64)parameter.Value).ToString(CultureInfo.InvariantCulture);
				case TypeCode.SByte:
					return ((SByte)parameter.Value).ToString(CultureInfo.InvariantCulture);
				default:
					if(parameter is ConstantValue && value != null) {
						switch(valueTypeCode) {
							case TypeCode.Boolean:
								return (bool)value ? "1" : "0";
							case TypeCode.String:
								return "'" + ((string)value).Replace("'", "''") + "'";
						}
					}
					createParameter = true;
					return "@p" + index.ToString(CultureInfo.InvariantCulture);
			}
		}
		public override string FormatConstraint(string constraintName) {
			return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", constraintName);
		}
		public void ClearDatabase(IDbCommand command) {
			string[] tables = GetStorageTablesList(false);
			foreach(string table in tables) {				
				string safeTableName = ComposeSafeTableName(table);
				string safeSchemaName = ComposeSafeSchemaName(table);								
				command.CommandText = string.Format(CultureInfo.InvariantCulture, "drop table \"{0}\".\"{1}\"", string.IsNullOrEmpty(safeSchemaName) ? currentUser : safeSchemaName, safeTableName); 
				command.ExecuteNonQuery();
			}
		}
		protected override void ProcessClearDatabase() {
			IDbCommand command = CreateCommand();
			ClearDatabase(command);
		}
		public override string[] GetStorageTablesList(bool includeViews) {
			SelectStatementResult tables = SelectData(new Query(string.Format("select TABSCHEMA, TABNAME from SYSCAT.TABLES where OWNERTYPE = 'U' and TYPE in ('T'{0})", includeViews ? ", 'V'" : string.Empty)));
			ArrayList result = new ArrayList(tables.Rows.Length);
			foreach(SelectStatementResultRow row in tables.Rows) {
				string schemaName = ((string)row.Values[0]).Trim();
				string tableName = ((string)row.Values[1]).Trim();
				if(string.Equals(schemaName, "SYSTOOLS")) continue;
				if(string.Equals(schemaName, string.IsNullOrEmpty(ObjectsOwner) ? currentUser : ObjectsOwner))
					result.Add(string.Format(CultureInfo.InvariantCulture, "{0}", tableName));
				else
					result.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", schemaName, tableName));
			}
			return (string[])result.ToArray(typeof(string));
		}
		public virtual string GetCurrentUser() {
			try {
				Query query = new Query("SELECT CURRENT USER FROM SYSIBM.SYSDUMMY1");
				SelectStatementResult result = SelectData(query);
				return result.Rows[0].Values[0].ToString();
			} catch(Exception) {
				return string.Empty;
			}
		}
		const string IdentityVarName = "IDENTITY_VAR";
		bool hasIdentityes;
		void GenerateView(DBTable table, StringBuilder result) {
			StringBuilderAppendLine(result, string.Format("CREATE VIEW <SCHEMA>.\"{0}_xpoView\" AS", table.Name));
			StringBuilderAppendLine(result, "\tSELECT");
			for(int i = 0; i < table.Columns.Count; i++) {
				if(!hasIdentityes) {
					hasIdentityes = table.Columns[i].IsIdentity;
				}
				string identityMagicAlias = table.Columns[i].IsIdentity ? " AS \"" + IdentityColumnMagicName + "\"" : string.Empty;
				StringBuilderAppendLine(result, string.Format("\t\t\"{0}\"{2}{1}", table.Columns[i].Name, i < table.Columns.Count - 1 ? "," : string.Empty, identityMagicAlias));
			}
			StringBuilderAppendLine(result, string.Format("\tFROM <SCHEMA>.\"{0}\"", table.Name));
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateInsertSP(DBTable table, StringBuilder result) {
			StringBuilderAppendLine(result, string.Format("CREATE PROCEDURE <SCHEMA>.\"sp_{0}_xpoView_insert\" (", table.Name));
			for(int i = 0; i < table.Columns.Count; i++) {
				string dbType = GetSqlCreateColumnType(table, table.Columns[i]);
				string name;
				string formatStr;
				bool isFK = false;
				if(table.Columns[i].IsIdentity) {
					name = IdentityColumnMagicName;
					formatStr = "\tOUT @{0} {1}{3}{2}";
				} else {
					name = table.Columns[i].Name;
					formatStr = "\tIN @{0} {1}{3}{2}";
				}
				StringBuilderAppendLine(result, string.Format(formatStr, name, dbType, i < table.Columns.Count - 1 ? "," : string.Empty, isFK ? " = null" : string.Empty));
			}
			StringBuilderAppendLine(result, ") LANGUAGE SQL");
			StringBuilderAppendLine(result, "BEGIN");
			StringBuilderAppendLine(result, string.Format("\t\tINSERT INTO <SCHEMA>.\"{0}\"(", table.Name));
			for(int i = 0; i < table.Columns.Count; i++) {
				if(table.Columns[i].IsIdentity) { continue; }
				StringBuilderAppendLine(result, string.Format("\t\t\t\"{0}\"{1}", table.Columns[i].Name, i < table.Columns.Count - 1 ? "," : string.Empty));
			}
			StringBuilderAppendLine(result, "\t\t)");
			StringBuilderAppendLine(result, "\t\tVALUES(");
			for(int i = 0; i < table.Columns.Count; i++) {
				if(table.Columns[i].IsIdentity) { continue; }
				StringBuilderAppendLine(result, string.Format("\t\t\t@{0}{1}", table.Columns[i].Name, i < table.Columns.Count - 1 ? "," : string.Empty));
			}
			StringBuilderAppendLine(result, "\t\t);");
			StringBuilderAppendLine(result, string.Format("\t\tselect identity_val_local() into @{0} from sysibm.sysdummy1;", IdentityColumnMagicName));
			StringBuilderAppendLine(result, "END");
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateUpdateSP(DBTable table, StringBuilder result) {
			StringBuilderAppendLine(result, string.Format("CREATE PROCEDURE <SCHEMA>.\"sp_{0}_xpoView_update\"(", table.Name));
			AppendKeys(table, result);
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				if(i != 0 || table.PrimaryKey.Columns.Count > 0) { StringBuilderAppendLine(result, ","); }
				string dbType = GetSqlCreateColumnType(table, table.Columns[i]);
				StringBuilderAppendLine(result, string.Format("\tIN @old_{0} {1},", table.Columns[i].Name, dbType));
				result.Append(string.Format("\tIN @{0} {1}", table.Columns[i].Name, dbType));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, ") LANGUAGE SQL");
			StringBuilderAppendLine(result, "BEGIN");
			StringBuilderAppendLine(result, string.Format("\tUPDATE <SCHEMA>.\"{0}\" SET", table.Name));
			bool first = true;
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				if(first) { first = false; } else { StringBuilderAppendLine(result, ","); }
				result.Append(string.Format("\t\t\"{0}\"=@{0}", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\tWHERE");
			AppendWhere(table, result);
			StringBuilderAppendLine(result, "END");
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateDeleteSP(DBTable table, StringBuilder result) {
			StringBuilderAppendLine(result, string.Format("CREATE PROCEDURE <SCHEMA>.\"sp_{0}_xpoView_delete\"(", table.Name));
			AppendKeys(table, result);
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				if(i != 0 || table.PrimaryKey.Columns.Count > 0) { StringBuilderAppendLine(result, ","); }
				string dbType = GetSqlCreateColumnType(table, table.Columns[i]);
				result.Append(string.Format("\tIN @old_{0} {1}", table.Columns[i].Name, dbType));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, ") LANGUAGE SQL");
			StringBuilderAppendLine(result, "BEGIN");
			StringBuilderAppendLine(result, string.Format("\tDELETE FROM <SCHEMA>.\"{0}\" WHERE", table.Name));
			AppendWhere(table, result);
			StringBuilderAppendLine(result, "END");
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateInsteadOfInsertTrigger(DBTable table, StringBuilder result) {
			string triggerName = string.Format("t_{0}_xpoView_insert", table.Name);
			StringBuilderAppendLine(result, string.Format("CREATE TRIGGER <SCHEMA>.\"{0}\"", triggerName));
			StringBuilderAppendLine(result, "INSTEAD OF INSERT");
			string viewName = string.Format("{0}_xpoView", table.Name);
			StringBuilderAppendLine(result, string.Format("ON <SCHEMA>.\"{0}\"", viewName));
			StringBuilderAppendLine(result, "REFERENCING NEW AS inserted");
			StringBuilderAppendLine(result, "FOR EACH ROW");
			StringBuilderAppendLine(result, "BEGIN ATOMIC");
			string spName = string.Format("sp_{0}_xpoView_insert", table.Name);
			if(hasIdentityes) {
				StringBuilderAppendLine(result, string.Format("\tSIGNAL SQLSTATE '38T00' SET MESSAGE_TEXT = 'Use {0} instead';", spName));
				StringBuilderAppendLine(result, "END");
				StringBuilderAppendLine(result, "GO");
				return;
			}
			StringBuilderAppendLine(result, string.Format("\tCALL <SCHEMA>.\"{0}\"(", spName));
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				result.Append(string.Format("\t\tinserted.\"{0}\"", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result, "\t);");
			StringBuilderAppendLine(result, "END");
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateInsteadOfUpdateTrigger(DBTable table, StringBuilder result) {
			string triggerName = string.Format("t_{0}_xpoView_update", table.Name);
			StringBuilderAppendLine(result, string.Format("CREATE TRIGGER <SCHEMA>.\"{0}\"", triggerName));
			StringBuilderAppendLine(result, "INSTEAD OF UPDATE");
			string viewName = string.Format("{0}_xpoView", table.Name);
			StringBuilderAppendLine(result, string.Format("ON <SCHEMA>.\"{0}\"", viewName));
			StringBuilderAppendLine(result, "REFERENCING");
			StringBuilderAppendLine(result, "\tOLD AS deleted");
			StringBuilderAppendLine(result, "\tNEW AS inserted");
			StringBuilderAppendLine(result, "FOR EACH ROW");
			StringBuilderAppendLine(result, "BEGIN ATOMIC");
			string spName = string.Format("sp_{0}_xpoView_update", table.Name);
			StringBuilderAppendLine(result, string.Format("\tCALL <SCHEMA>.\"{0}\"(", spName));
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\tdeleted.\"{0}\"", columnName));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				StringBuilderAppendLine(result, ",");
				StringBuilderAppendLine(result, string.Format("\t\tdeleted.\"{0}\",", table.Columns[i].Name));
				result.Append(string.Format("\t\tinserted.\"{0}\"", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result, "\t);");
			StringBuilderAppendLine(result, "END");
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateInsteadOfDeleteTrigger(DBTable table, StringBuilder result) {
			string triggerName = string.Format("t_{0}_xpoView_delete", table.Name);
			StringBuilderAppendLine(result, string.Format("CREATE TRIGGER <SCHEMA>.\"{0}\"", triggerName));
			StringBuilderAppendLine(result, "INSTEAD OF DELETE");
			string viewName = string.Format("{0}_xpoView", table.Name);
			StringBuilderAppendLine(result, string.Format("ON <SCHEMA>.\"{0}\"", viewName));
			StringBuilderAppendLine(result, "REFERENCING OLD AS deleted");
			StringBuilderAppendLine(result, "FOR EACH ROW");
			StringBuilderAppendLine(result, "BEGIN ATOMIC");
			string spName = string.Format("sp_{0}_xpoView_delete", table.Name);
			StringBuilderAppendLine(result, string.Format("\tCALL <SCHEMA>.\"{0}\"(", spName));
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\tdeleted.\"{0}\"", columnName));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				StringBuilderAppendLine(result, ",");
				result.Append(string.Format("\t\tdeleted.\"{0}\"", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result, "\t);");
			StringBuilderAppendLine(result, "END");
			StringBuilderAppendLine(result, "GO");
		}
		void AppendWhere(DBTable table, StringBuilder result) {
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, " AND"); }
				result.Append(string.Format("\t\t\"{0}\" = @{0}", table.PrimaryKey.Columns[i]));
			}
			result.Append(";");
			StringBuilderAppendLine(result);
		}
		void AppendKeys(DBTable table, StringBuilder result) {
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				DBColumn keyColumn = GetDbColumnByName(table, table.PrimaryKey.Columns[i]);
				string dbType = GetSqlCreateColumnType(table, keyColumn);
				result.Append(string.Format("\tIN @{0} {1}", keyColumn.Name, dbType));
			}
		}
		ExecMethodDelegate commandBuilderDeriveParametersHandler;
		protected override void CommandBuilderDeriveParameters(IDbCommand command) {
			if(commandBuilderDeriveParametersHandler == null) {
				commandBuilderDeriveParametersHandler = ReflectConnectionHelper.GetCommandBuilderDeriveParametersDelegate("IBM.Data.DB2", "IBM.Data.DB2.DB2CommandBuilder");
			}
			commandBuilderDeriveParametersHandler(command);
		}
		public override DBStoredProcedure[] GetStoredProcedures() {
			List<DBStoredProcedure> result = new List<DBStoredProcedure>();
			IDbCommand command = Connection.CreateCommand();
			command.CommandText = @"select * from syscat.procedures where definer not in ('SYSIBM')";
			IDataReader reader = command.ExecuteReader();
			while(reader.Read()) {
				DBStoredProcedure curProc = new DBStoredProcedure();
				curProc.Name = reader.GetString(1);
				result.Add(curProc);
			}
			reader.Close();
			command.CommandType = CommandType.StoredProcedure;
			foreach(DBStoredProcedure sproc in result) {
				command.CommandText = sproc.Name;
				try {
					CommandBuilderDeriveParameters(command);
					List<DBStoredProcedureArgument> dbArguments = new List<DBStoredProcedureArgument>();
					foreach(IDataParameter parameter in command.Parameters) {
						DBStoredProcedureArgumentDirection direction = DBStoredProcedureArgumentDirection.In;
						if(parameter.Direction == ParameterDirection.ReturnValue) {
							continue;
						}
						if(parameter.Direction == ParameterDirection.InputOutput) {
							direction = DBStoredProcedureArgumentDirection.InOut;
						}
						if(parameter.Direction == ParameterDirection.Output) {
							direction = DBStoredProcedureArgumentDirection.Out;
						}
						DBColumnType columnType = GetColumnType(parameter.DbType, true);
						dbArguments.Add(new DBStoredProcedureArgument(parameter.ParameterName, columnType, direction));
					}
					sproc.Arguments.AddRange(dbArguments);
				} catch {
					continue;
				}
			}
			return result.ToArray();
		}
	}
	public class DB2ProviderFactory : ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DB2ConnectionProvider.CreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DB2ConnectionProvider.CreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(ServerParamID) || !parameters.ContainsKey(DatabaseParamID) ||
				!parameters.ContainsKey(UserIDParamID) || !parameters.ContainsKey(PasswordParamID)) {
				return null;
			}
			return DB2ConnectionProvider.GetConnectionString(parameters[ServerParamID], parameters[DatabaseParamID],
				parameters[UserIDParamID], parameters[PasswordParamID]);
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
		public override string ProviderKey { get { return DB2ConnectionProvider.XpoProviderTypeString; } }
		public override string[] GetDatabases(string server, string userid, string password) {
			Type db2EnumeratorType = DevExpress.Xpo.Helpers.XPTypeActivator.GetType("IBM.Data.DB2", "IBM.Data.DB2.DB2Enumerator");
			Type db2EnumDatabaseType = DevExpress.Xpo.Helpers.XPTypeActivator.GetType("IBM.Data.DB2", "IBM.Data.DB2.DB2Enumerator+DB2EnumDatabase");
			Type db2EnumInstanceType = DevExpress.Xpo.Helpers.XPTypeActivator.GetType("IBM.Data.DB2", "IBM.Data.DB2.DB2Enumerator+DB2EnumInstance");
			if(db2EnumeratorType == null || db2EnumDatabaseType == null)
				return new string[0];
			ArrayList databases;
			if(db2EnumeratorType.GetMethod("EnumerateDBs", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, new Type[] { typeof(string) }, null) == null) {
				object db2EnumInstance = Activator.CreateInstance(db2EnumInstanceType, server, "50000");
				databases = (ArrayList)ReflectConnectionHelper.InvokeStaticMethod(db2EnumeratorType, "InternalOperation2", new object[] { db2EnumInstance, userid, password }, false);
			} else {
				databases = (ArrayList)ReflectConnectionHelper.InvokeStaticMethod(db2EnumeratorType, "EnumerateDBs", new object[] { server }, false);
			}
			string[] result = new string[databases.Count];
			for(int i = 0; i < databases.Count; i++) {
				if(db2EnumDatabaseType.IsInstanceOfType(databases[i])) {
					result[i] = (string)ReflectConnectionHelper.GetPropertyValue(databases[i], "Database");
				}
			}
			return result;
		}
		public override string FileFilter { get { return null; } }
		public override bool MeanSchemaGeneration { get { return true; } }
		public override bool SupportStoredProcedures { get { return false; } }
	}
}
