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
	using System.Data.SqlTypes;
	using System.Collections.Generic;
	using DevExpress.Xpo.Helpers;
	public class MSSqlCEConnectionProvider : ConnectionProviderSql, ISqlGeneratorFormatterEx {
		public const string XpoProviderTypeString = "MSSqlServerCE";
		ReflectConnectionHelper helper;
		ReflectConnectionHelper ConnectionHelper {
			get {
				if(helper == null)
					helper = new ReflectConnectionHelper(Connection, "System.Data.SqlServerCe.SqlCeException");
				return helper;
			}
		}
		public static string GetConnectionString(string database, string password) {
			return String.Format("{2}={3};data source={0};password={1}", database, password, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString);
		}
		public static string GetConnectionString(string database) {
			return GetConnectionString(database, String.Empty);
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = ReflectConnectionHelper.GetConnection("System.Data.SqlServerCe", "System.Data.SqlServerCe.SqlCeConnection", connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] { connection };
			return CreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new MSSqlCEConnectionProvider(connection, autoCreateOption);
		}
		static MSSqlCEConnectionProvider() {
			RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
			RegisterDataStoreProvider("System.Data.SqlServerCe.SqlCeConnection", new DataStoreCreationFromConnectionDelegate(CreateProviderFromConnection));
			RegisterFactory(new MSSqlCEProviderFactory());
		}
		public static void Register() { }
		public MSSqlCEConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
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
			return "numeric(19,4)";
		}
		protected override string GetSqlCreateColumnTypeForDouble(DBTable table, DBColumn column) {
			return "float";
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
				return "nvarchar(" + column.Size.ToString(CultureInfo.InvariantCulture) + ')';
			else
				return "ntext";
		}
		protected override string GetSqlCreateColumnTypeForDateTime(DBTable table, DBColumn column) {
			return "datetime";
		}
		protected override string GetSqlCreateColumnTypeForGuid(DBTable table, DBColumn column) {
			return "uniqueidentifier";
		}
		public const int MaximumVarbinarySize = 8000;
		protected override string GetSqlCreateColumnTypeForByteArray(DBTable table, DBColumn column) {
			if (column.Size > 0 && column.Size <= MaximumVarbinarySize)
				return "varbinary(" + column.Size.ToString(CultureInfo.InvariantCulture) + ')';
			else
				return "image";
		}
		public override string GetSqlCreateColumnFullAttributes(DBTable table, DBColumn column) {
			string result = GetSqlCreateColumnType(table, column);
			if(column.IsKey)
				result += " NOT NULL";
			else
				result += " NULL";
			if(column.IsKey) {
				if(column.IsIdentity && (column.ColumnType == DBColumnType.Int32 || column.ColumnType == DBColumnType.Int64) && IsSingleColumnPKColumn(table, column))
					result += " IDENTITY";
			}
			return result;
		}
		protected override object ConvertToDbParameter(object clientValue, TypeCode clientValueTypeCode) {
			switch(clientValueTypeCode) {
				case TypeCode.SByte:
					return (Int16)(SByte)clientValue;
				case TypeCode.UInt16:
					return (Int32)(UInt16)clientValue;
				case TypeCode.UInt32:
					return (Int64)(UInt32)clientValue;
				case TypeCode.UInt64:
					return (Decimal)(UInt64)clientValue;
			}
			return base.ConvertToDbParameter(clientValue, clientValueTypeCode);
		}
		protected override Int64 GetIdentity(Query sql) {
			ExecSql(sql);
			object value = GetScalar(new Query("select @@Identity"));
			return value is SqlDecimal ? (long)(SqlDecimal)value : ((IConvertible)value).ToInt64(CultureInfo.InvariantCulture);
		}
		protected override Exception WrapException(Exception e, IDbCommand query) {
			object o;
			if(ConnectionHelper.TryGetExceptionProperty(e, "Errors", out o) && ((ICollection)o).Count > 0) {
				foreach(object error in (ICollection)o) {
					int nativeError = (int)ReflectConnectionHelper.GetPropertyValue(error, "NativeError");
					int hResult = (int)ReflectConnectionHelper.GetPropertyValue(error, "HResult");
					if(nativeError == 25503 || hResult == -2147217865) {
						return new SchemaCorrectionNeededException((string)ReflectConnectionHelper.GetPropertyValue(error, "Message"), e);
					}
					if(nativeError == 25016 || nativeError == 25025)
						return new ConstraintViolationException(query.CommandText, string.Empty, e);
					break;
				}
			}
			return base.WrapException(e, query);
		}
		protected override IDbConnection CreateConnection() {
			return ConnectionHelper.GetConnection(ConnectionString);
		}
		public override IDbCommand CreateCommand() {
			IDbCommand command = Connection.CreateCommand();
			OpenConnection();
			command.Connection = Connection;
			if(Transaction != null)
				command.Transaction = Transaction;
			return command;
		}
		protected override void CreateDataBase() {
			const int CannotOpenDatabaseError = 25046;
				try {
					Connection.Open();
				} catch(Exception e) {
					object o;
					if(ConnectionHelper.TryGetExceptionProperty(e, "NativeError", out o) && ((int)o) == CannotOpenDatabaseError
						&& CanCreateDatabase) {
						CreateDatabaseThroughSqlCeEngine();
					} else {
						throw new UnableToOpenDatabaseException(XpoDefault.ConnectionStringRemovePassword(ConnectionString), e);
					}
				}
		}
		void CreateDatabaseThroughSqlCeEngine() {
			Type sqlCeEngineType = ConnectionHelper.GetType("System.Data.SqlServerCe.SqlCeEngine");
			object sqlCeEngine = ReflectConnectionHelper.CreateInstance(sqlCeEngineType, ConnectionString);
			ReflectConnectionHelper.InvokeMethod(sqlCeEngine, sqlCeEngineType, "CreateDatabase", new object[0], false);
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
				case "int":
					return DBColumnType.Int32;
				case "varbinary":
				case "image":
					return DBColumnType.ByteArray;
				case "varchar":
					return DBColumnType.String;
				case "bit":
					return DBColumnType.Boolean;
				case "tinyint":
					return DBColumnType.Byte;
				case "smallint":
					return DBColumnType.Int16;
				case "bigint":
					return DBColumnType.Int64;
				case "numeric":
					return DBColumnType.Decimal;
				case "nchar":
				case "char":
					if(length == 1)
						return DBColumnType.Char;
					return DBColumnType.String;
				case "money":
					return DBColumnType.Decimal;
				case "real":
					return DBColumnType.Single;
				case "float":
					return DBColumnType.Double;
				case "uniqueidentifier":
					return DBColumnType.Guid;
				case "nvarchar":
					return DBColumnType.String;
				case "datetime":
					return DBColumnType.DateTime;
				case "ntext":
					return DBColumnType.String;
			}
			return DBColumnType.Unknown;
		}
		void GetColumns(DBTable table) {
			foreach(SelectStatementResultRow row in SelectData(new Query("select COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @p1", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" })).Rows) {
				int size = row.Values[2] != DBNull.Value ? ((IConvertible)row.Values[2]).ToInt32(CultureInfo.InvariantCulture) : 0;
				DBColumnType type = GetTypeFromString((string)row.Values[1], size);
				table.AddColumn(new DBColumn((string)row.Values[0], false, String.Empty, type == DBColumnType.String ? size : 0, type));
			}
		}
		void GetPrimaryKey(DBTable table) {
			SelectStatementResult data = SelectData(new Query("select c.COLUMN_NAME, tc.AUTOINC_INCREMENT from INFORMATION_SCHEMA.KEY_COLUMN_USAGE c join INFORMATION_SCHEMA.TABLE_CONSTRAINTS p on p.CONSTRAINT_NAME = c.CONSTRAINT_NAME join INFORMATION_SCHEMA.COLUMNS tc on tc.TABLE_NAME = c.TABLE_NAME and tc.COLUMN_NAME = c.COLUMN_NAME where c.TABLE_NAME = @p1 and p.CONSTRAINT_TYPE = 'PRIMARY KEY'", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" }));
			if(data.Rows.Length > 0) {
				StringCollection cols = new StringCollection();
				for(int i = 0; i < data.Rows.Length; i++) {
					cols.Add((string)data.Rows[i].Values[0]);
				}
				table.PrimaryKey = new DBPrimaryKey(cols);
				foreach(string columnName in cols) {
					DBColumn column = table.GetColumn(columnName);
					if(column != null)
						column.IsKey = true;
				}
				if(cols.Count == 1 && !(data.Rows[0].Values[1] is DBNull))
					table.GetColumn(cols[0]).IsIdentity = true;
			}
		}
		void GetIndexes(DBTable table) {
			SelectStatementResult data = SelectData(new Query(
				@"select i.INDEX_NAME, i.COLUMN_NAME, i.""UNIQUE"" from INFORMATION_SCHEMA.INDEXES i where i.TABLE_NAME = @p1 order by i.INDEX_NAME, i.ORDINAL_POSITION"
			, new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" }));
			DBIndex index = null;
			foreach(SelectStatementResultRow row in data.Rows) {
				if(index == null || index.Name != (string)row.Values[0]) {
					StringCollection list = new StringCollection();
					list.Add((string)row.Values[1]);
					index = new DBIndex((string)row.Values[0], list, (bool)row.Values[2]);
					table.Indexes.Add(index);
				} else
					index.Columns.Add((string)row.Values[1]);
			}
		}
		void GetForeignKeys(DBTable table) {
			SelectStatementResult data = SelectData(new Query(
@"select c.CONSTRAINT_NAME, c.COLUMN_NAME, cr.COLUMN_NAME, cr.TABLE_NAME
from INFORMATION_SCHEMA.TABLE_CONSTRAINTS fk 
join INFORMATION_SCHEMA.KEY_COLUMN_USAGE c on c.CONSTRAINT_NAME = fk.CONSTRAINT_NAME
join INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc on rc.CONSTRAINT_NAME = fk.CONSTRAINT_NAME
join INFORMATION_SCHEMA.KEY_COLUMN_USAGE cr on cr.CONSTRAINT_NAME = rc.UNIQUE_CONSTRAINT_NAME 
	and cr.ORDINAL_POSITION = c.ORDINAL_POSITION
where fk.CONSTRAINT_TYPE = 'FOREIGN KEY' and c.TABLE_NAME = @p1
order by c.CONSTRAINT_NAME, c.ORDINAL_POSITION"
			, new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" }));
			Hashtable fks = new Hashtable();
			foreach(SelectStatementResultRow row in data.Rows) {
				DBForeignKey fk = (DBForeignKey)fks[row.Values[0]];
				if(fk == null) {
					StringCollection pkc = new StringCollection();
					StringCollection fkc = new StringCollection();
					pkc.Add((string)row.Values[1]);
					fkc.Add((string)row.Values[2]);
					fk = new DBForeignKey(pkc, (string)row.Values[3], fkc);
					table.ForeignKeys.Add(fk);
					fks[row.Values[0]] = fk;
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
			Hashtable dbTables = new Hashtable();
			foreach(SelectStatementResultRow row in GetDataForTables(tables, null, "select TABLE_NAME, TABLE_TYPE from INFORMATION_SCHEMA.TABLES where TABLE_NAME in ({0}) and TABLE_TYPE in ('TABLE', 'VIEW')").Rows)
				dbTables.Add(row.Values[0], (string)row.Values[1] == "VIEW");
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
		string FormatDBObject(string objectName) {
			return '"' + objectName + '"';
		}
		public override string FormatTable(string schema, string tableName) {
			return FormatDBObject(tableName);
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			return FormatDBObject(tableName) + ' ' + tableAlias;
		}
		public override string FormatColumn(string columnName) {
			return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", columnName);
		}
		public override string FormatColumn(string columnName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "{1}.\"{0}\"", columnName, tableAlias);
		}
		public override string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int topSelectedRecords) {
			string expandedWhereSql = whereSql != null ? string.Format(CultureInfo.InvariantCulture, "\nwhere {0}", whereSql) : string.Empty;
			string expandedOrderBySql = orderBySql != null ? string.Format(CultureInfo.InvariantCulture, "\norder by {0}", orderBySql) : string.Empty;
			string expandedHavingSql = havingSql != null ? string.Format(CultureInfo.InvariantCulture, "\nhaving {0}", havingSql) : string.Empty;
			string expandedGroupBySql = groupBySql != null ? string.Format(CultureInfo.InvariantCulture, "\ngroup by {0}", groupBySql) : string.Empty;
			return string.Format(CultureInfo.InvariantCulture, "select {0} from {1}{2}{3}{4}{5}", selectedPropertiesSql, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedOrderBySql);
		}
		public override string FormatInsertDefaultValues(string tableName) {
			return string.Format(CultureInfo.InvariantCulture, "insert into {0}", tableName);
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
		readonly static char[] achtungChars = new char[] { '_', '%', '[', ']' };
		public override string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.StartsWith: {
						object secondOperand = operands[1];
						if(secondOperand is OperandValue && ((OperandValue)secondOperand).Value is string) {
							string operandString = (string)((OperandValue)secondOperand).Value;
							int likeIndex = operandString.IndexOfAny(achtungChars);
							if(likeIndex < 0) {
								return string.Format(CultureInfo.InvariantCulture, "({0} like {1})", processParameter(operands[0]), processParameter(new ConstantValue(operandString + "%")));
							} else if(likeIndex > 0) {
								return string.Format(CultureInfo.InvariantCulture, "(({0} like {2}) And (CharIndex({1}, {0}) = 1))", processParameter(operands[0]), processParameter(secondOperand), processParameter(new ConstantValue(operandString.Substring(0, likeIndex) + "%")));
							}
						}
						return string.Format(CultureInfo.InvariantCulture, "(CharIndex({1}, {0}) = 1)", processParameter(operands[0]), processParameter(secondOperand));
					}
				case FunctionOperatorType.EndsWith:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(Substring({0}, Len({0}) - Len({1}) + 1, Len({1})) = ({1}))", operands[0], operands[1]);
				case FunctionOperatorType.AddTimeSpan:
				case FunctionOperatorType.AddSeconds:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "DATEADD(ms, CONVERT(bigint, (CONVERT(numeric(38, 19),({1})) * 1000)) % 86400000, DATEADD(day, (CONVERT(numeric(38, 19),({1})) * 1000) / 86400000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddMinutes:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "DATEADD(ms, CONVERT(bigint, (CONVERT(numeric(38, 19),({1})) * 60000)) % 86400000, DATEADD(day, (CONVERT(numeric(38, 19),({1})) * 60000) / 86400000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddHours:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "DATEADD(ms, CONVERT(bigint, (CONVERT(numeric(38, 19),({1})) * 3600000)) % 86400000, DATEADD(day, (CONVERT(numeric(38, 19),({1})) * 3600000) / 86400000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddDays:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "DATEADD(ms, CONVERT(bigint, (CONVERT(numeric(38, 19),({1})) * 86400000)) % 86400000, DATEADD(day, (CONVERT(numeric(38, 19),({1})) * 86400000) / 86400000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.Atn2:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(case when ({1}) = 0 then Sign({0}) * Atan(1) * 2 else Atn2({0},  {1}) end)", operands[0], operands[1]);
				case FunctionOperatorType.Cosh:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "((Exp({0}) + Exp(-({0}))) / 2)", operands[0]);
				case FunctionOperatorType.Sinh:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "((Exp({0}) - Exp(-({0}))) / 2)", operands[0]);
				case FunctionOperatorType.Tanh:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "((Exp({0}) - Exp(-({0}))) / (Exp({0}) + Exp(-({0}))))", operands[0]);
				case FunctionOperatorType.Max:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(case when {0} > {1} then {0} else {1} end)", operands[0], operands[1]);
				case FunctionOperatorType.Min:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(case when {0} < {1} then {0} else {1} end)", operands[0], operands[1]);
				case FunctionOperatorType.GetTimeOfDay:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(CONVERT(BigInt,((CONVERT(BigInt,DATEPART(HOUR, {0}))) * 36000000000) + ((CONVERT(BigInt,DATEPART(MINUTE, {0}))) * 600000000) + ((CONVERT(BigInt,DATEPART(SECOND, {0}))) * 10000000) + ((CONVERT(BigInt,DATEPART(MILLISECOND, {0}))) * 10000)))", operands[0]);
				case FunctionOperatorType.GetDate:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "DATEADD(HOUR, -DATEPART(HOUR, {0}), DATEADD(MINUTE, -DATEPART(MINUTE, {0}), DATEADD(SECOND, -DATEPART(SECOND, {0}), DATEADD(MILLISECOND, -DATEPART(MILLISECOND, {0}), {0}))))", operands[0]);
				case FunctionOperatorType.AddTicks:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "DATEADD(ms, CONVERT(BigInt, ({1}) / 10000) % 86400000, DATEADD(day, ({1}) / 864000000000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.PadLeft:
					switch(operands.Length) {
						case 2:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "(case when LEN({0}) < {1} then REPLICATE(' ', (({1}) - LEN({0}))) + ({0}) else {0} end)", operands[0], operands[1]);
						case 3:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "(case when LEN({0}) < {1} then REPLICATE({2}, (({1}) - LEN({0}))) + ({0}) else {0} end)", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.PadRight:
					switch(operands.Length) {
						case 2:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "(case when LEN({0}) < {1} then {0} + REPLICATE(' ', (({1}) - LEN({0}))) else {0} end)", operands[0], operands[1]);
						case 3:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "(case when LEN({0}) < {1} then {0} + REPLICATE({2}, (({1}) - LEN({0}))) else {0} end)", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "({0} is null or len({0}) = 0)", operands[0]);
				case FunctionOperatorType.Substring:
					string len = operands.Length < 3 ? "Len(" + processParameter(operands[0]) + ")" + " - CONVERT(Int, " + processParameter(operands[1]) + ")" : processParameter(operands[2]);
					return string.Format(CultureInfo.InvariantCulture, "Substring({0}, CONVERT(Int, {1}) + 1, {2})", processParameter(operands[0]), processParameter(operands[1]), len);
				case FunctionOperatorType.UtcNow:
					return FnUtcNow(processParameter);				
				default:
					return base.FormatFunction(processParameter, operatorType, operands);
			}
		}
		public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch (operatorType) {
				case FunctionOperatorType.Abs:
					return string.Format(CultureInfo.InvariantCulture, "ABS({0})", operands[0]);
				case FunctionOperatorType.Sqr:
					return string.Format(CultureInfo.InvariantCulture, "sqrt({0})", operands[0]);
				case FunctionOperatorType.Rnd:
					return "rand()";
				case FunctionOperatorType.BigMul:
					return string.Format(CultureInfo.InvariantCulture, "(Convert(bigint, {0}) * CONVERT(bigint,  {1}))", operands[0], operands[1]);
				case FunctionOperatorType.Log:
					switch (operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "Log({0})", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(Log({0}) / Log({1}))", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Log10:
					return string.Format(CultureInfo.InvariantCulture, "Log10({0})", operands[0]);
				case FunctionOperatorType.Sin:
					return string.Format(CultureInfo.InvariantCulture, "Sin({0})", operands[0]);
				case FunctionOperatorType.Tan:
					return string.Format(CultureInfo.InvariantCulture, "Tan({0})", operands[0]);
				case FunctionOperatorType.Atn:
					return string.Format(CultureInfo.InvariantCulture, "Atan({0})", operands[0]);
				case FunctionOperatorType.Cos:
					return string.Format(CultureInfo.InvariantCulture, "Cos({0})", operands[0]);
				case FunctionOperatorType.Acos:
					return string.Format(CultureInfo.InvariantCulture, "Acos({0})", operands[0]);
				case FunctionOperatorType.Asin:
					return string.Format(CultureInfo.InvariantCulture, "Asin({0})", operands[0]);
				case FunctionOperatorType.Exp:
					return string.Format(CultureInfo.InvariantCulture, "Exp({0})", operands[0]);
				case FunctionOperatorType.Power:
					return string.Format(CultureInfo.InvariantCulture, "Power({0},{1})", operands[0], operands[1]);
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
				case FunctionOperatorType.Ascii:
					return string.Format(CultureInfo.InvariantCulture, "UNICODE({0})", operands[0]);
				case FunctionOperatorType.Char:
					return string.Format(CultureInfo.InvariantCulture, "NChar({0})", operands[0]);
				case FunctionOperatorType.ToInt:
					return string.Format(CultureInfo.InvariantCulture, "Convert(int, ({0}))", operands[0]);
				case FunctionOperatorType.ToLong:
					return string.Format(CultureInfo.InvariantCulture, "Convert(bigint, ({0}))", operands[0]);
				case FunctionOperatorType.ToFloat:
					return string.Format(CultureInfo.InvariantCulture, "Convert(real, ({0}))", operands[0]);
				case FunctionOperatorType.ToDouble:
					return string.Format(CultureInfo.InvariantCulture, "Convert(float, ({0}))", operands[0]);
				case FunctionOperatorType.ToDecimal:
					return string.Format(CultureInfo.InvariantCulture, "Convert(numeric(38,19), ({0}))", operands[0]);
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, "Convert(nvarchar(4000), ({0}))", operands[0]);
				case FunctionOperatorType.GetMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Millisecond, {0})", operands[0]);
				case FunctionOperatorType.GetSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Second, {0})", operands[0]);
				case FunctionOperatorType.GetMinute:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Minute, {0})", operands[0]);
				case FunctionOperatorType.GetHour:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Hour, {0})", operands[0]);
				case FunctionOperatorType.GetDay:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Day, {0})", operands[0]);
				case FunctionOperatorType.GetMonth:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Month, {0})", operands[0]);
				case FunctionOperatorType.GetYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Year, {0})", operands[0]);
				case FunctionOperatorType.GetDayOfWeek:
					return string.Format(CultureInfo.InvariantCulture, "CONVERT(Int, (DATEPART(dw, {0}) - DATEPART(dw, '1900.01.01') + 8) % 7)", operands[0]);
				case FunctionOperatorType.GetDayOfYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(DayOfYear, {0})", operands[0]);
				case FunctionOperatorType.AddMilliSeconds:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(ms, {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.AddMonths:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(MONTH, {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.AddYears:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(YEAR, {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffDay:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(day, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffHour:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(hour, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(millisecond, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMinute:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(minute, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMonth:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(month, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(second, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffTick:
					return string.Format(CultureInfo.InvariantCulture, "((DATEDIFF(millisecond, {0}, {1})) * 10000)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(year, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Now:
					return "GETDATE()";
				case FunctionOperatorType.Today:
					return "DATEADD(day, DATEDIFF(day, '00:00:00', getdate()), '00:00:00')";
				case FunctionOperatorType.Replace:
					return string.Format(CultureInfo.InvariantCulture, "Replace({0}, {1}, {2})", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Reverse:
					throw new NotSupportedException();
				case FunctionOperatorType.Insert:
					return string.Format(CultureInfo.InvariantCulture, "Stuff({0}, ({1})+1, 0, {2})", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Remove:
					switch (operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "Substring({0}, 1, {1})", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "Stuff({0}, ({1})+1, {2}, '')", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.CharIndex:
					switch (operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(Charindex({0}, {1}) - 1)", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "(Charindex({0}, {1}, CONVERT(Int, {2}) + 1) - 1)", operands[0], operands[1], operands[2]);
						case 4:
							return string.Format(CultureInfo.InvariantCulture, "(Charindex({0}, Substring({1}, 1, CONVERT(Int, {3})), CONVERT(Int, {2}) + 1) - 1)", operands[0], operands[1], operands[2], operands[3]);
					}
					goto default;
				case FunctionOperatorType.Concat:
					string args = String.Empty;
					foreach (string arg in operands) {
						if (args.Length > 0)
							args += " + ";
						args += "convert(nvarchar, " + arg + ")";
					}
					return args;
				case FunctionOperatorType.IsNull:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "({0} is null)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(case when ({0}) is null then {1} else {0} end)", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Contains:
					return string.Format(CultureInfo.InvariantCulture, "(CharIndex({1}, {0}) >= 1)", operands[0], operands[1]);
				default:
					return base.FormatFunction(operatorType, operands);
			}
		}
		public override string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			switch(operatorType) {
				case BinaryOperatorType.Modulo:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) % ({1}))", leftOperand, rightOperand);
				default:
					return base.FormatBinary(operatorType, leftOperand, rightOperand);
			}
		}
		string FnUtcNow(ProcessParameter processParameter) {
			DateTime now = DateTime.Now;
			DateTime utcNow = now.ToUniversalTime();
			int diffHour = (int)((TimeSpan)(utcNow - now)).TotalHours;
			return string.Format(new ProcessParameterInvariantCulture(processParameter), "DATEADD(HOUR, {0}, GETDATE())", new OperandValue(diffHour));
		}
#if !CF
		SetPropertyValueDelegate setSqlCeParameterSqlDbType;
#endif
		protected override IDataParameter CreateParameter(IDbCommand command, object value, string name) {
			System.Data.Common.DbParameter param = (System.Data.Common.DbParameter)CreateParameter(command);
			param.Value = value;
			param.ParameterName = name;
			if(param.DbType == DbType.AnsiString)
				param.DbType = DbType.String;
			if(param.DbType == DbType.String && value is string) {
				if(((string)value).Length > 4000) {
#if CF
					ReflectConnectionHelper.SetPropertyValue(param, "SqlDbType", SqlDbType.NText);
#else
					if(setSqlCeParameterSqlDbType == null) InitSqlCeParameterSetDelegate();
					setSqlCeParameterSqlDbType(param, SqlDbType.NText);
#endif
				}
			}
			if(param.DbType == DbType.Binary && value is byte[]) {
				if(((byte[])value).Length > 4000) {
#if CF
					ReflectConnectionHelper.SetPropertyValue(param, "SqlDbType", SqlDbType.Image);
#else
					if(setSqlCeParameterSqlDbType == null) InitSqlCeParameterSetDelegate();
					setSqlCeParameterSqlDbType(param, SqlDbType.Image);
#endif
				}
			}
			return param;
		}
#if !CF            
		void InitSqlCeParameterSetDelegate() {
			Type sqlCeParameterType = ConnectionHelper.GetType("System.Data.SqlServerCe.SqlCeParameter");
			setSqlCeParameterSqlDbType = ReflectConnectionHelper.CreateSetPropertyDelegate(sqlCeParameterType, "SqlDbType");
		}
#endif
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
						return "N'" + ((string)value).Replace("'", "''") + "'";
				}
			}
			createParameter = true;
			return "@p" + index.ToString(CultureInfo.InvariantCulture);
		}
		public override bool SupportNamedParameters { get { return false; } }
		public override string FormatConstraint(string constraintName) {
			return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", constraintName);
		}
		protected override int GetSafeNameTableMaxLength() {
			return 127;
		}
		protected override void ProcessClearDatabase() {
			Connection.Close();
			System.IO.File.Delete((string)ReflectConnectionHelper.GetPropertyValue(Connection, "DataSource"));
			CreateDatabaseThroughSqlCeEngine();
		}
		public override string[] GetStorageTablesList(bool includeViews) {
			SelectStatementResult tables = SelectData(new Query("select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_TYPE = 'TABLE'"));
			string[] result = new string[tables.Rows.Length];
			for(int i = 0; i < tables.Rows.Length; ++i) {
				result[i] = (string)tables.Rows[i].Values[0];
			}
			return result;
		}
		public string ObjectsOwner = "dbo";
		protected override string GetSafeNameRoot(string originalName) {
			return GetSafeNameMsSql(originalName);
		}
		protected override bool SupportCommandPrepare { get { return true; } }
		public override bool NativeOuterApplySupported { get { return true; } }
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
	public class MSSqlCEProviderFactory : ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return MSSqlCEConnectionProvider.CreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return MSSqlCEConnectionProvider.CreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			string connectionString;
			if(!parameters.ContainsKey(DatabaseParamID)) { return null; }
			if(parameters.ContainsKey(PasswordParamID)) {
				connectionString = MSSqlCEConnectionProvider.GetConnectionString(parameters[DatabaseParamID], parameters[PasswordParamID]);
			}
			else {
				connectionString = MSSqlCEConnectionProvider.GetConnectionString(parameters[DatabaseParamID]);
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
		public override string ProviderKey { get { return MSSqlCEConnectionProvider.XpoProviderTypeString; } }
		public override string[] GetDatabases(string server, string userid, string password) {
			return new string[1] { server };
		}
		public override string FileFilter { get { return "MSSQL Compact Edition databases|*.sdf"; } }
		public override bool MeanSchemaGeneration { get { return true; } }
	}
}
