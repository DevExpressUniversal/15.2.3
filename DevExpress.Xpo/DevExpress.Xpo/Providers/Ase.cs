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
	public class AseConnectionProvider : ConnectionProviderSql {
		public const string XpoProviderTypeString = "Ase";
		const string MulticolumnIndexesAreNotSupported = "Multicolumn indexes are not supported.";
		ReflectConnectionHelper helper;
		ReflectConnectionHelper ConnectionHelper {
			get {
				if(helper == null)
					helper = new ReflectConnectionHelper(Connection, "Sybase.Data.AseClient.AseException");
				return helper;
			}
		}
		public static string GetConnectionString(string server, string database, string userid, string password) {
			return GetConnectionString(server, 5000, database, userid, password);
		}
		public static string GetConnectionString(string server, int port, string database, string userid, string password) {
			return String.Format("{5}={6};Port={4};Data Source={0};User ID={1};Password={2};Initial Catalog={3};persist security info=true",
				server, userid, password, database, port, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString);
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] { connection };
			return CreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new AseConnectionProvider(connection, autoCreateOption);
		}
		static AseConnectionProvider() {
			RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
			RegisterDataStoreProvider("AseConnection", new DataStoreCreationFromConnectionDelegate(CreateProviderFromConnection));
			RegisterFactory(new AseProviderFactory());
		}
		public static void Register() { }
		public AseConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
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
			return "unichar(1)";
		}
		protected override string GetSqlCreateColumnTypeForDecimal(DBTable table, DBColumn column) {
			return "money";
		}
		protected override string GetSqlCreateColumnTypeForDouble(DBTable table, DBColumn column) {
			return "double precision";
		}
		protected override string GetSqlCreateColumnTypeForSingle(DBTable table, DBColumn column) {
			return "real";
		}
		protected override string GetSqlCreateColumnTypeForInt32(DBTable table, DBColumn column) {
			return "integer";
		}
		protected override string GetSqlCreateColumnTypeForUInt32(DBTable table, DBColumn column) {
			return "unsigned integer";
		}
		protected override string GetSqlCreateColumnTypeForInt16(DBTable table, DBColumn column) {
			return "smallint";
		}
		protected override string GetSqlCreateColumnTypeForUInt16(DBTable table, DBColumn column) {
			return "unsigned smallint";
		}
		protected override string GetSqlCreateColumnTypeForInt64(DBTable table, DBColumn column) {
			return "bigint";
		}
		protected override string GetSqlCreateColumnTypeForUInt64(DBTable table, DBColumn column) {
			return "unsigned bigint";
		}
		public const int MaximumStringSize = 800;
		protected override string GetSqlCreateColumnTypeForString(DBTable table, DBColumn column) {
			if(column.Size > 0 && column.Size <= MaximumStringSize)
				return "univarchar(" + column.Size.ToString(CultureInfo.InvariantCulture) + ')';
			else
				return "unitext";
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
		const int MaxVarLength = 16384;
		string GetSqlCreateColumnTypeSp(DBTable table, DBColumn column) {
			string type = GetSqlCreateColumnType(table, column);
			switch(type) {
				case "image":
					return string.Format("varbinary({0})", MaxVarLength);
				case "unitext":
					return string.Format("varchar({0})", MaxVarLength);
			}
			return type;
		}
		public override string GetSqlCreateColumnFullAttributes(DBTable table, DBColumn column) {
			string result = GetSqlCreateColumnType(table, column);
			if(column.ColumnType == DBColumnType.Boolean)
				return result;
			if(column.IsKey) {
				if(column.IsIdentity && (column.ColumnType == DBColumnType.Int32 || column.ColumnType == DBColumnType.Int64) && IsSingleColumnPKColumn(table, column)) {
					result += " IDENTITY";
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
		protected override Int64 GetIdentity(Query sql) {
			object value = GetScalar(new Query(sql.Sql + "\nselect @@Identity", sql.Parameters, sql.ParametersNames));
			return (value as IConvertible).ToInt64(CultureInfo.InvariantCulture);
		}
		protected override IDbConnection CreateConnection() {
			return ConnectionHelper.GetConnection(ConnectionString);
		}
		readonly static string[] assemblyNames = new string[]{
			"Sybase.AdoNet4.AseClient",
			"Sybase.AdoNet35.AseClient",
			"Sybase.AdoNet2.AseClient",
			"Sybase.Data.AseClient"
		};
		public static IDbConnection CreateConnection(string connectionString) {
			IDbConnection connection = null;
			string typeName = "Sybase.Data.AseClient.AseConnection";
			foreach(string assemblyName in assemblyNames){
				connection = ReflectConnectionHelper.GetConnection(assemblyName, typeName, false);
				if(connection != null)
					break;
			}
			if(connection == null) {
				throw new TypeLoadException("Could not load type " + typeName);
			}
			connection.ConnectionString = connectionString;
			return connection;
		}
		protected override void CreateDataBase() {
			const int CannotOpenDatabaseError = 911;
			try {
				ConnectionStringParser parser = new ConnectionStringParser(ConnectionString);
				parser.RemovePartByName("Pooling");
				string connectString = parser.GetConnectionString() + ";Pooling=false";
				using(IDbConnection conn = ConnectionHelper.GetConnection(connectString)) {
					conn.Open();
				}
			} catch(Exception e) {
				object o;
				if(ConnectionHelper.TryGetExceptionProperty(e, "Errors", out o)
					&& ((ICollection)o).Count > 0
					&& ((int)ReflectConnectionHelper.GetPropertyValue(ReflectConnectionHelper.GetCollectionFirstItem(((ICollection)o)), "MessageNumber")) == CannotOpenDatabaseError
					&& CanCreateDatabase) {
					ConnectionStringParser helper = new ConnectionStringParser(ConnectionString);
					string dbName = helper.GetPartByName("initial catalog");
					helper.RemovePartByName("initial catalog");
					string connectToServer = helper.GetConnectionString();
					using(IDbConnection conn = ConnectionHelper.GetConnection(connectToServer)) {
						conn.Open();
						using(IDbCommand c = conn.CreateCommand()) {
							c.CommandText = "Create Database " + dbName;
							c.ExecuteNonQuery();
							c.CommandText = "exec master.dbo.sp_dboption " + dbName + ", 'ddl in tran', true";
							c.ExecuteNonQuery();
						}
					}
				} else
					throw new UnableToOpenDatabaseException(XpoDefault.ConnectionStringRemovePassword(ConnectionString), e);
			}
		}
		protected override bool IsConnectionBroken(Exception e) {
			object o;
			if(ConnectionHelper.TryGetExceptionProperty(e, "Errors", out o)
				&& ((ICollection)o).Count > 0) {
				object error = ReflectConnectionHelper.GetCollectionFirstItem((ICollection)o);
				int messageNumber = (int)ReflectConnectionHelper.GetPropertyValue(error, "MessageNumber");
				if(messageNumber == 30046) {
					Connection.Close();
					return true;
				}
			}
			return base.IsConnectionBroken(e);
		}
		protected override Exception WrapException(Exception e, IDbCommand query) {
			object o;
			if(ConnectionHelper.TryGetExceptionProperty(e, "Errors", out o)
				&& ((ICollection)o).Count > 0) {
				object error = ReflectConnectionHelper.GetCollectionFirstItem((ICollection)o);
				int messageNumber = (int)ReflectConnectionHelper.GetPropertyValue(error, "MessageNumber");
				if(messageNumber == 208 || messageNumber == 207)
					return new SchemaCorrectionNeededException((string)ReflectConnectionHelper.GetPropertyValue(error, "Message"), e);
				if(messageNumber == 2601 || messageNumber == 547)
					return new ConstraintViolationException(query.CommandText, GetParametersString(query), e);
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
		DBColumnType GetTypeFromNumber(byte type, byte prec, int length, short userType, byte charsize, out int len) {
			len = 0;
			switch(type) {
				case 38:
					switch(length) {
						case 1:
							return DBColumnType.Byte;
						case 2:
							return DBColumnType.Int16;
						case 4:
							return DBColumnType.Int32;
						case 8:
							return DBColumnType.Int64;
						default:
							return DBColumnType.Unknown;
					}
				case 68:
					switch(length) {
						case 1:
							return DBColumnType.Byte;
						case 2:
							return DBColumnType.UInt16;
						case 4:
							return DBColumnType.UInt32;
						case 8:
							return DBColumnType.UInt64;
						default:
							return DBColumnType.Unknown;
					}
				case 56:
					return DBColumnType.Int32;
				case 66:
					return DBColumnType.UInt32;
				case 52:
					return DBColumnType.Int16;
				case 65:
					return DBColumnType.UInt16;
				case 50:
					return DBColumnType.Boolean;
				case 39:
				case 35:
					if(userType == 2)
						len = length;
					else
						len = length / charsize;
					return DBColumnType.String;
				case 174:
				case 155:
					len = length / 2;
					return DBColumnType.String;
				case 34:
				case 45:
					return DBColumnType.ByteArray;
				case 111:
				case 61:
					return DBColumnType.DateTime;
				case 109:
					return DBColumnType.Double;
				case 110:
				case 60:
					return DBColumnType.Decimal;
				case 108:
				case 63:
					if(prec <= 3)
						return DBColumnType.SByte;
					if(prec <= 5)
						return DBColumnType.Int16;
					if(prec <= 10)
						return DBColumnType.Int32;
					return DBColumnType.Int64;
			}
			return DBColumnType.Unknown;
		}
		void GetColumns(DBTable table) {
			foreach(SelectStatementResultRow row in SelectData(new Query("select c.name, c.type, c.prec, c.length, c.usertype, @@ncharsize, c.status from syscolumns c left join sysobjects t on c.id = t.id where t.name = @p1", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" })).Rows) {
				int len;
				DBColumnType type = GetTypeFromNumber((byte)row.Values[1], row.Values[2] is DBNull ? (byte)0 : (byte)row.Values[2], (int)row.Values[3], (short)row.Values[4], (byte)row.Values[5], out len);
				DBColumn column = new DBColumn((string)row.Values[0], false, String.Empty, len, type);
				column.IsIdentity = (Convert.ToInt32(row.Values[6]) & 128) == 128;
				table.AddColumn(column);
			}
		}
		void GetPrimaryKey(DBTable table) {
			SelectStatementResult data = SelectData(new Query("select index_col(o.name, i.indid, 1), i.keycnt from sysindexes i join sysobjects o on o.id = i.id where i.status & 2048 <> 0 and o.name = @p1", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" }));
			if(data.Rows.Length > 0) {
				if((short)data.Rows[0].Values[1] != 1)
					throw new NotImplementedException(MulticolumnIndexesAreNotSupported); 
				StringCollection cols = new StringCollection();
				cols.Add((string)data.Rows[0].Values[0]);
				foreach(string columnName in cols) {
					DBColumn column = table.GetColumn(columnName);
					if(column != null)
						column.IsKey = true;
				}
				table.PrimaryKey = new DBPrimaryKey(cols);
			}
		}
		public override void CreateIndex(DBTable table, DBIndex index) {
			if(table.Name != "XPObjectType")
				base.CreateIndex(table, index);
		}
		void GetIndexes(DBTable table) {
			SelectStatementResult data = SelectData(new Query("select index_col(o.name, i.indid, 1), i.keycnt, (i.status & 2) from sysindexes i join sysobjects o on o.id = i.id where o.name = @p1 and i.keycnt > 1 and i.status & 2048 = 0", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" }));
			foreach(SelectStatementResultRow row in data.Rows) {
				if((short)row.Values[1] != 2)
					throw new NotImplementedException(MulticolumnIndexesAreNotSupported); 
				StringCollection cols = new StringCollection();
				cols.Add((string)row.Values[0]);
				table.Indexes.Add(new DBIndex(cols, Convert.ToInt32(row.Values[2]) == 2));
			}
		}
		void GetForeignKeys(DBTable table) {
			SelectStatementResult data = SelectData(new Query(
@"select f.keycnt, fc.name, pc.name, r.name from sysreferences f
join sysobjects o on o.id = f.tableid
join sysobjects r on r.id = f.reftabid
join syscolumns fc on f.fokey1 = fc.colid and fc.id = o.id
join syscolumns pc on f.refkey1 = pc.colid and pc.id = r.id
where o.name = @p1", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" }));
			foreach(SelectStatementResultRow row in data.Rows) {
				if((short)row.Values[0] != 1)
					throw new NotImplementedException(MulticolumnIndexesAreNotSupported); 
				StringCollection pkc = new StringCollection();
				StringCollection fkc = new StringCollection();
				pkc.Add((string)row.Values[1]);
				fkc.Add((string)row.Values[2]);
				table.ForeignKeys.Add(new DBForeignKey(pkc, (string)row.Values[3], fkc));
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
			foreach(SelectStatementResultRow row in GetDataForTables(tables, null, "select name,type from sysobjects where name in ({0}) and type in ('U', 'V')").Rows)
				dbTables.Add(row.Values[0], ((string)row.Values[1]).Trim() == "V");
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
			return 28;
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
		public override string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int topSelectedRecords) {
			string modificatorsSql = string.Format(CultureInfo.InvariantCulture, (topSelectedRecords != 0) ? "top {0} " : string.Empty, topSelectedRecords);
			string expandedWhereSql = whereSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}where {1}", Environment.NewLine, whereSql) : string.Empty;
			string expandedOrderBySql = orderBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}order by {1}", Environment.NewLine, orderBySql) : string.Empty;
			string expandedHavingSql = havingSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}having {1}", Environment.NewLine, havingSql) : string.Empty;
			string expandedGroupBySql = groupBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}group by {1}", Environment.NewLine, groupBySql) : string.Empty;
			if(topSelectedRecords == 0)
				return string.Format(CultureInfo.InvariantCulture, "select {0}{1} from {2}{3}{4}{5}{6}", modificatorsSql, selectedPropertiesSql, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedOrderBySql);
			else
				return string.Format(CultureInfo.InvariantCulture, "set rowcount {0} select {1} from {2}{3}{4}{5}{6} set rowcount 0", topSelectedRecords, selectedPropertiesSql, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedOrderBySql);
		}
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
		public override string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			switch(operatorType) {
				case BinaryOperatorType.Modulo:
					return string.Format(CultureInfo.InvariantCulture, "{0} % {1}", leftOperand, rightOperand);
				case BinaryOperatorType.BitwiseAnd:
					return string.Format(CultureInfo.InvariantCulture, "({0} & {1})", leftOperand, rightOperand);
				case BinaryOperatorType.BitwiseOr:
					return string.Format(CultureInfo.InvariantCulture, "({0} | {1})", leftOperand, rightOperand);
				case BinaryOperatorType.BitwiseXor:
					return string.Format(CultureInfo.InvariantCulture, "({0} ^ {1})", leftOperand, rightOperand);
				default:
					return base.FormatBinary(operatorType, leftOperand, rightOperand);
			}
		}
		public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.Acos:
					return string.Format(CultureInfo.InvariantCulture, "acos({0})", operands[0]);
				case FunctionOperatorType.Asin:
					return string.Format(CultureInfo.InvariantCulture, "asin({0})", operands[0]);
				case FunctionOperatorType.Atn:
					return string.Format(CultureInfo.InvariantCulture, "atan({0})", operands[0]);
				case FunctionOperatorType.Atn2:
					return string.Format(CultureInfo.InvariantCulture, "(case when {0} = 0 then (case when {1} >= 0 then 0 else atan(1) * 4 end) else 2 * atan({0} / (sqrt({1} * {1} + {0} * {0}) + {1})) end)", operands[0], operands[1]);
				case FunctionOperatorType.Cosh:
					return string.Format(CultureInfo.InvariantCulture, "((exp({0}) + exp({0} * -1)) / 2)", operands[0]);
				case FunctionOperatorType.Sinh:
					return string.Format(CultureInfo.InvariantCulture, "((exp({0}) - exp({0} * -1)) / 2)", operands[0]);
				case FunctionOperatorType.Tanh:
					return string.Format(CultureInfo.InvariantCulture, "((exp({0} * 2) - 1) / (exp({0} * 2) + 1))", operands[0]);
				case FunctionOperatorType.Log:
					return FnLog(operands);
				case FunctionOperatorType.Log10:
					return string.Format(CultureInfo.InvariantCulture, "log10({0})", operands[0]);
				case FunctionOperatorType.Round:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "round({0}, 0)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "round({0}, {1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Sqr:
					return string.Format(CultureInfo.InvariantCulture, "sqrt({0})", operands[0]);
				case FunctionOperatorType.ToInt:
					return string.Format(CultureInfo.InvariantCulture, "cast({0} AS integer)", operands[0]);
				case FunctionOperatorType.ToLong:
					return string.Format(CultureInfo.InvariantCulture, "cast({0} AS bigint)", operands[0]);
				case FunctionOperatorType.ToFloat:
					return string.Format(CultureInfo.InvariantCulture, "cast({0} AS real)", operands[0]);
				case FunctionOperatorType.ToDouble:
					return string.Format(CultureInfo.InvariantCulture, "cast({0} AS double precision)", operands[0]);
				case FunctionOperatorType.ToDecimal:
					return string.Format(CultureInfo.InvariantCulture, "cast({0} AS money)", operands[0]);
				case FunctionOperatorType.BigMul:
					return string.Format(CultureInfo.InvariantCulture, "cast({0} * {1} as bigint)", operands[0], operands[1]);
				case FunctionOperatorType.Max:
					return string.Format(CultureInfo.InvariantCulture, "(case when {0} > {1} then {0} else {1} end)", operands[0], operands[1]);
				case FunctionOperatorType.Min:
					return string.Format(CultureInfo.InvariantCulture, "(case when {0} < {1} then {0} else {1} end)", operands[0], operands[1]);
				case FunctionOperatorType.Rnd:
					return "Rand()";
				case FunctionOperatorType.CharIndex:
					return FnCharIndex(operands);
				case FunctionOperatorType.PadLeft:
					return FnLpad(operands);
				case FunctionOperatorType.PadRight:
					return FnRpad(operands);
				case FunctionOperatorType.Remove:
					return FnRemove(operands);
				case FunctionOperatorType.GetMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "datepart(ms, {0})", operands[0]);
				case FunctionOperatorType.AddTicks:
					return string.Format(CultureInfo.InvariantCulture, "dateadd(ms, (cast({1} as bigint) / 10000) % 86400000, dateadd(day, (cast({1} as bigint) / 10000) / 86400000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddMilliSeconds:
					return string.Format(CultureInfo.InvariantCulture, "dateadd(ms, {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.AddTimeSpan:
				case FunctionOperatorType.AddSeconds:
					return string.Format(CultureInfo.InvariantCulture, "dateadd(ms, cast((cast({1} as numeric(38,19)) * 1000) as bigint) % 86400000, dateadd(day, cast((cast({1} as numeric(38,19)) * 1000) / 86400000 as bigint), {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddMinutes:
					return string.Format(CultureInfo.InvariantCulture, "dateadd(ms, cast((cast({1} as numeric(38,19)) * 60000) as bigint) % 86400000, dateadd(day, cast((cast({1} as numeric(38,19)) * 60000) / 86400000 as bigint), {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddHours:
					return string.Format(CultureInfo.InvariantCulture, "dateadd(ms, cast((cast({1} as numeric(38,19)) * 3600000) as bigint) % 86400000, dateadd(day, cast((cast({1} as numeric(38,19)) * 3600000) / 86400000 as bigint), {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddDays:
					return string.Format(CultureInfo.InvariantCulture, "dateadd(ms, cast((cast({1} as numeric(38,19)) * 86400000) as bigint) % 86400000, dateadd(day, cast((cast({1} as numeric(38,19)) * 86400000) / 86400000 as bigint), {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddMonths:
					return string.Format(CultureInfo.InvariantCulture, "dateadd(month, {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.AddYears:
					return string.Format(CultureInfo.InvariantCulture, "dateadd(year, {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffYear:
					return string.Format(CultureInfo.InvariantCulture, "datediff(yy, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMonth:
					return string.Format(CultureInfo.InvariantCulture, "datediff(mm, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffDay:
					return string.Format(CultureInfo.InvariantCulture, "datediff(dd, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffHour:
					return string.Format(CultureInfo.InvariantCulture, "datediff(hh, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMinute:
					return string.Format(CultureInfo.InvariantCulture, "datediff(mi, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffSecond:
					return string.Format(CultureInfo.InvariantCulture, "datediff(ss, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "datediff(ms, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffTick:
					return string.Format(CultureInfo.InvariantCulture, "(datediff(ms, {0}, {1}) * 10000)", operands[0], operands[1]);
				case FunctionOperatorType.Now:
					return "getdate()";
				case FunctionOperatorType.UtcNow:
					return "getutcdate()";
				case FunctionOperatorType.Today:
					return "cast(getdate() as date)";
				case FunctionOperatorType.IsNull:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "(({0}) is null)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "isnull({0}, {1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) is null or ({0}) = '')", operands[0]);
				case FunctionOperatorType.Contains:
					return string.Format(CultureInfo.InvariantCulture, "(CharIndex({1}, {0}) > 0)", operands[0], operands[1]);
				case FunctionOperatorType.EndsWith:
					return string.Format(CultureInfo.InvariantCulture, "(Right({0}, Len({1})) = ({1}))", operands[0], operands[1]);
				default:
					return base.FormatFunction(operatorType, operands);
			}
		}
		readonly static char[] achtungChars = new char[] { '_', '%', '[', ']' };
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
							return string.Format(CultureInfo.InvariantCulture, "(({0} like {2}) And (CharIndex({1}, {0}) = 1))", processParameter(operands[0]), processParameter(secondOperand), processParameter(new ConstantValue(operandString.Substring(0, likeIndex) + "%")));
						}
					}
					return string.Format(CultureInfo.InvariantCulture, "(CharIndex({1}, {0}) = 1)", processParameter(operands[0]), processParameter(operands[1]));
				default:
					return base.FormatFunction(processParameter, operatorType, operands);
			}
		}
		string FnRemove(string[] operands) {
			switch(operands.Length) {
				case 2:
					return string.Format(CultureInfo.InvariantCulture, "substring({0}, 1, {1})", operands[0], operands[1]);
				case 3:
					return string.Format(CultureInfo.InvariantCulture, "stuff({0}, {1} + 1, {2}, null)", operands[0], operands[1], operands[2]);
				default:
					throw new NotSupportedException();
			}
		}
		string FnRpad(string[] operands) {
			switch(operands.Length) {
				case 2:
					return string.Format(CultureInfo.InvariantCulture, "({0} + replicate(' ', {1} - char_length({0})))", operands[0], operands[1]);
				case 3:
					return string.Format(CultureInfo.InvariantCulture, "({0} + replicate({2}, {1} - char_length({0})))", operands[0], operands[1], operands[2]);
				default:
					throw new NotSupportedException();
			}
		}
		string FnLpad(string[] operands) {
			switch(operands.Length) {
				case 2:
					return string.Format(CultureInfo.InvariantCulture, "(replicate(' ', {1} - char_length({0})) + {0})", operands[0], operands[1]);
				case 3:
					return string.Format(CultureInfo.InvariantCulture, "(replicate({2}, {1} - char_length({0})) + {0})", operands[0], operands[1], operands[2]);
				default:
					throw new NotSupportedException();
			}
		}
		string FnLog(string[] operands) {
			switch(operands.Length) {
				case 1:
					return string.Format(CultureInfo.InvariantCulture, "log({0})", operands[0]);
				case 2:
					return string.Format(CultureInfo.InvariantCulture, "(log({0}) / log({1}))", operands[0], operands[1]);
				default:
					throw new NotSupportedException();
			}
		}
		string FnCharIndex(string[] operands) {
			switch(operands.Length) {
				case 2:
					return string.Format(CultureInfo.InvariantCulture, "(charindex({0}, {1}) - 1)", operands[0], operands[1]);
				case 3:
					return string.Format(CultureInfo.InvariantCulture, "(case when charindex({0}, substring({1}, {2} + 1, char_length({1}) - {2})) > 0 then charindex({0}, substring({1}, {2} + 1, char_length({1}) - {2})) + {2} - 1 else -1 end)", operands[0], operands[1], operands[2]);
				case 4:
					return string.Format(CultureInfo.InvariantCulture, "(case when charindex({0}, substring({1}, {2} + 1, {3} - {2})) > 0 then charindex({0}, substring({1}, {2} + 1, {3} - {2})) + {2} - 1 else -1 end)", operands[0], operands[1], operands[2], operands[3]);
				default:
					throw new NotSupportedException();
			}
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
						return "'" + ((string)value).Replace("'", "''") + "'";
				}
			}
			createParameter = true;
			return "@p" + index.ToString(CultureInfo.InvariantCulture);
		}
		object aseDbTypeImage;
		object aseDbTypeUnitext;
		object aseDbTypeUniVarChar;
		SetPropertyValueDelegate setAseDbParameterDbType;
		protected override IDataParameter CreateParameter(IDbCommand command, object value, string name) {
			IDataParameter param = CreateParameter(command);
			param.Value = value;
			param.ParameterName = name;
			if(value is byte[]) {
				if(setAseDbParameterDbType == null)
					InitAseDbTypeVars();
				setAseDbParameterDbType(param, aseDbTypeImage);
			}
			if(value is string) {
				if(setAseDbParameterDbType == null)
					InitAseDbTypeVars();
				if(((string)value).Length > MaximumStringSize)
					setAseDbParameterDbType(param, aseDbTypeUnitext);
				else
					setAseDbParameterDbType(param, aseDbTypeUniVarChar);
			}
			return param;
		}
		void InitAseDbTypeVars() {
			Type aseDbParameterType = ConnectionHelper.GetType("Sybase.Data.AseClient.AseParameter");
			Type aseDbType = ConnectionHelper.GetType("Sybase.Data.AseClient.AseDbType");
			aseDbTypeImage = Enum.Parse(aseDbType, "Image", false);
			aseDbTypeUnitext = Enum.Parse(aseDbType, "Unitext", false);
			aseDbTypeUniVarChar = Enum.Parse(aseDbType, "UniVarChar", false);
			setAseDbParameterDbType = ReflectConnectionHelper.CreateSetPropertyDelegate(aseDbParameterType, "AseDbType");
		}
		public override string FormatConstraint(string constraintName) {
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", constraintName);
		}
		void ClearDatabase(IDbCommand command) {
			SelectStatementResult constraints = SelectData(new Query("select o.name, t.name  from sysreferences f join sysobjects o on f.constrid = o.id join sysobjects t on f.tableid = t.id"));
			foreach(SelectStatementResultRow row in constraints.Rows) {
				command.CommandText = "alter table [" + (string)row.Values[1] + "] drop constraint [" + (string)row.Values[0] + "]";
				command.ExecuteNonQuery();
			}
			string[] tables = GetStorageTablesList(false);
			foreach(string table in tables) {
				command.CommandText = "drop table [" + table + "]";
				command.ExecuteNonQuery();
			}
		}
		protected override void ProcessClearDatabase() {
			IDbCommand command = CreateCommand();
			ClearDatabase(command);
		}
		public override string[] GetStorageTablesList(bool includeViews) {
			SelectStatementResult tables = SelectData(new Query(string.Format("select name from sysobjects where type in ('U'{0})", includeViews ? ", 'V'" : string.Empty)));
			ArrayList result = new ArrayList(tables.Rows.Length);
			foreach(SelectStatementResultRow row in tables.Rows) {
				result.Add(row.Values[0]);
			}
			return (string[])result.ToArray(typeof(string));
		}
		bool hasIdentityes;
		void GenerateView(DBTable table, StringBuilder result) {
			string objName = ComposeSafeTableName(string.Format("{0}_xpoView", table.Name));
			StringBuilderAppendLine(result, string.Format("CREATE VIEW [{0}] AS", objName));
			StringBuilderAppendLine(result, "\tSELECT");
			for(int i = 0; i < table.Columns.Count; i++) {
				if(!hasIdentityes) {
					hasIdentityes = table.Columns[i].IsIdentity;
				}
				string identityMagicAlias = table.Columns[i].IsIdentity ? " AS " + IdentityColumnMagicName : string.Empty;
				StringBuilderAppendLine(result, string.Format("\t\t[{0}]{2}{1}", table.Columns[i].Name, i < table.Columns.Count - 1 ? "," : string.Empty, identityMagicAlias));
			}
			StringBuilderAppendLine(result, string.Format("\tFROM [{0}]", table.Name));
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateInsertSP(DBTable table, StringBuilder result) {
			string objName = ComposeSafeTableName(string.Format("sp_{0}_xpoView_insert", table.Name));
			StringBuilderAppendLine(result, string.Format("CREATE PROCEDURE [{0}]", objName));
			for(int i = 0; i < table.Columns.Count; i++) {
				string dbType = GetSqlCreateColumnTypeSp(table, table.Columns[i]);
				bool isFK = false;
				string name;
				string formatStr;
				for(int j = 0; j < table.ForeignKeys.Count; j++) {
					if(table.ForeignKeys[j].Columns.Contains(table.Columns[i].Name)) {
						isFK = true;
						break;
					}
				}
				if(table.Columns[i].IsIdentity) {
					name = IdentityColumnMagicName;
					formatStr = "\t@{0} {1}{3} OUTPUT{2}";
				} else {
					name = table.Columns[i].Name;
					formatStr = "\t@{0} {1}{3}{2}";
				}
				StringBuilderAppendLine(result, string.Format(formatStr, name, dbType, i < table.Columns.Count - 1 ? "," : string.Empty, isFK ? " = null" : string.Empty));
			}
			StringBuilderAppendLine(result, "AS");
			StringBuilderAppendLine(result, "BEGIN");
			StringBuilderAppendLine(result, string.Format("\tINSERT INTO [{0}](", table.Name));
			for(int i = 0; i < table.Columns.Count; i++) {
				if(table.Columns[i].IsIdentity) { continue; }
				StringBuilderAppendLine(result, string.Format("\t\t[{0}]{1}", table.Columns[i].Name, i < table.Columns.Count - 1 ? "," : string.Empty));
			}
			StringBuilderAppendLine(result, "\t)");
			StringBuilderAppendLine(result, "\tVALUES(");
			for(int i = 0; i < table.Columns.Count; i++) {
				if(table.Columns[i].IsIdentity) { continue; }
				StringBuilderAppendLine(result, string.Format("\t\t@{0}{1}", table.Columns[i].Name, i < table.Columns.Count - 1 ? "," : string.Empty));
			}
			StringBuilderAppendLine(result, "\t)");
			StringBuilderAppendLine(result, string.Format("\t\tSELECT @{0} = @@Identity", IdentityColumnMagicName));
			StringBuilderAppendLine(result, "END");
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateUpdateSP(DBTable table, StringBuilder result) {
			string objName = ComposeSafeTableName(string.Format("sp_{0}_xpoView_update", table.Name));
			StringBuilderAppendLine(result, string.Format("CREATE PROCEDURE [{0}]", objName));
			AppendKeys(table, result);
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				if(i != 0 || table.PrimaryKey.Columns.Count > 0) { StringBuilderAppendLine(result, ","); }
				string dbType = GetSqlCreateColumnTypeSp(table, table.Columns[i]);
				StringBuilderAppendLine(result, string.Format("\t@old_{0} {1},", table.Columns[i].Name, dbType));
				result.Append(string.Format("\t@{0} {1}", table.Columns[i].Name, dbType));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "AS");
			bool hasColumns = false;
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				hasColumns = true;
			}
			if(!hasColumns) {
				StringBuilderAppendLine(result, "BEGIN");
				StringBuilderAppendLine(result, string.Format("\tRAISERROR('There are no columns to update in {0}_xpoView', 16, 1, null);", table.Name));
				StringBuilderAppendLine(result, "END");
			} else {
				StringBuilderAppendLine(result, string.Format("\tUPDATE [{0}] SET", table.Name));
				bool first = true;
				for(int i = 0; i < table.Columns.Count; i++) {
					if(IsKey(table, table.Columns[i].Name)) { continue; }
					if(first) { first = false; } else { StringBuilderAppendLine(result, ","); }
					result.Append(string.Format("\t\t[{0}]=@{0}", table.Columns[i].Name));
				}
				StringBuilderAppendLine(result);
				StringBuilderAppendLine(result, "\tWHERE");
				AppendWhere(table, result);
			}
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateDeleteSP(DBTable table, StringBuilder result) {
			string objName = ComposeSafeTableName(string.Format("sp_{0}_xpoView_delete", table.Name));
			StringBuilderAppendLine(result, string.Format("CREATE PROCEDURE [{0}]", objName));
			AppendKeys(table, result);
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				if(i != 0 || table.PrimaryKey.Columns.Count > 0) { StringBuilderAppendLine(result, ","); }
				string dbType = GetSqlCreateColumnTypeSp(table, table.Columns[i]);
				result.Append(string.Format("\t@old_{0} {1}", table.Columns[i].Name, dbType));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "AS");
			StringBuilderAppendLine(result, string.Format("\tDELETE FROM [{0}] WHERE", table.Name));
			AppendWhere(table, result);
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateInsteadOfInsertTrigger(DBTable table, StringBuilder result) {
			string triggerName = ComposeSafeTableName(string.Format("t_{0}_xpoView_insert", table.Name));
			StringBuilderAppendLine(result, string.Format("CREATE TRIGGER [{0}]", triggerName));
			string viewName = ComposeSafeTableName(string.Format("{0}_xpoView", table.Name));
			string spName = ComposeSafeTableName(string.Format("sp_{0}_xpoView_insert", table.Name));
			StringBuilderAppendLine(result, string.Format("ON [{0}]", viewName));
			StringBuilderAppendLine(result, "INSTEAD OF INSERT AS");
			if(hasIdentityes) {
				StringBuilderAppendLine(result, "BEGIN");
				StringBuilderAppendLine(result, string.Format("\tRAISERROR 25002 'Use {0} instead'", spName));
				StringBuilderAppendLine(result, "END");
				StringBuilderAppendLine(result, "GO");
				return;
			}
			InitTriggerCore(table, result);
			bool first = true;
			for(int i = 0; i < table.Columns.Count; i++) {
				if(table.Columns[i].IsIdentity) { continue; }
				if(first) { first = false; } else { StringBuilderAppendLine(result, ","); }
				result.Append(string.Format("\t\t\t[{0}]", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\t\tFROM inserted");
			StringBuilderAppendLine(result, "\tOPEN cur");
			for(int i = 0; i < table.Columns.Count; i++) {
				if(table.Columns[i].IsIdentity) { continue; }
				StringBuilderAppendLine(result, string.Format("\tDECLARE @{0} {1}", table.Columns[i].Name, GetSqlCreateColumnTypeSp(table, table.Columns[i])));
			}
			StringBuilderAppendLine(result, "\tFETCH NEXT FROM cur INTO");
			first = true;
			for(int i = 0; i < table.Columns.Count; i++) {
				if(table.Columns[i].IsIdentity) { continue; }
				if(first) { first = false; } else { StringBuilderAppendLine(result, ","); }
				result.Append(string.Format("\t\t@{0}", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\tWHILE(@@fetch_status <> -1)");
			StringBuilderAppendLine(result, "\tBEGIN");
			StringBuilderAppendLine(result, string.Format("\t\tEXEC [{0}]", spName));
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				if(table.Columns[i].IsIdentity) {
					result.Append("\t\t\t0");
					continue;
				}
				result.Append(string.Format("\t\t\t@{0}", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\t\tFETCH NEXT FROM cur INTO");
			first = true;
			for(int i = 0; i < table.Columns.Count; i++) {
				if(table.Columns[i].IsIdentity) { continue; }
				if(first) { first = false; } else { StringBuilderAppendLine(result, ","); }
				result.Append(string.Format("\t\t\t@{0}", table.Columns[i].Name));
			}
			FinalizeTriggerCore(result);
		}
		void GenerateInsteadOfUpdateTrigger(DBTable table, StringBuilder result) {
			string triggerName = ComposeSafeTableName(string.Format("t_{0}_xpoView_update", table.Name));
			StringBuilderAppendLine(result, string.Format("CREATE TRIGGER [{0}]", triggerName));
			string viewName = ComposeSafeTableName(string.Format("{0}_xpoView", table.Name));
			StringBuilderAppendLine(result, string.Format("ON [{0}]", viewName));
			StringBuilderAppendLine(result, "INSTEAD OF UPDATE AS");
			InitTriggerCore(table, result);
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\t\ti.[{0}]", columnName));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				StringBuilderAppendLine(result, ",");
				StringBuilderAppendLine(result, string.Format("\t\t\td.[{0}] as [old_{0}],", table.Columns[i].Name));
				result.Append(string.Format("\t\t\ti.[{0}]", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\t\tFROM");
			StringBuilderAppendLine(result, "\t\t\tinserted i");
			StringBuilderAppendLine(result, "\t\t\tINNER JOIN");
			StringBuilderAppendLine(result, "\t\t\tdeleted d");
			StringBuilderAppendLine(result, "\t\t\tON");
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, " AND"); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\t\t\ti.[{0}] = d.[{0}]", columnName));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\tOPEN cur");
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				string type = GetSqlCreateColumnTypeSp(table, GetDbColumnByName(table, table.PrimaryKey.Columns[i]));
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				StringBuilderAppendLine(result, string.Format("\tDECLARE @{0} {1}", columnName, type));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				string type = GetSqlCreateColumnTypeSp(table, table.Columns[i]);
				StringBuilderAppendLine(result, string.Format("\tDECLARE @old_{0} {1}", table.Columns[i].Name, type));
				StringBuilderAppendLine(result, string.Format("\tDECLARE @{0} {1}", table.Columns[i].Name, type));
			}
			StringBuilderAppendLine(result, "\tFETCH NEXT FROM cur INTO");
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\t@{0}", columnName));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				StringBuilderAppendLine(result, ",");
				StringBuilderAppendLine(result, string.Format("\t\t@old_{0},", table.Columns[i].Name));
				result.Append(string.Format("\t\t@{0}", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\tWHILE(@@fetch_status <> -1)");
			StringBuilderAppendLine(result, "\tBEGIN");
			string spName = ComposeSafeTableName(string.Format("sp_{0}_xpoView_update", table.Name));
			StringBuilderAppendLine(result, string.Format("\t\tEXEC [{0}]", spName));
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\t\t@{0}", columnName));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				StringBuilderAppendLine(result, ",");
				StringBuilderAppendLine(result, string.Format("\t\t\t@old_{0},", table.Columns[i].Name));
				result.Append(string.Format("\t\t\t@{0}", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\t\tFETCH NEXT FROM cur INTO");
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\t\t@{0}", columnName));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				StringBuilderAppendLine(result, ",");
				StringBuilderAppendLine(result, string.Format("\t\t\t@old_{0},", table.Columns[i].Name));
				result.Append(string.Format("\t\t\t@{0}", table.Columns[i].Name));
			}
			FinalizeTriggerCore(result);
		}
		void GenerateInsteadOfDeleteTrigger(DBTable table, StringBuilder result) {
			string triggerName = ComposeSafeTableName(string.Format("t_{0}_xpoView_delete", table.Name));
			StringBuilderAppendLine(result, string.Format("CREATE TRIGGER [{0}]", triggerName));
			string viewName = ComposeSafeTableName(string.Format("{0}_xpoView", table.Name));
			StringBuilderAppendLine(result, string.Format("ON [{0}]", viewName));
			StringBuilderAppendLine(result, "INSTEAD OF DELETE AS");
			InitTrigger(table, result);
			StringBuilderAppendLine(result, "\t\tFROM deleted");
			StringBuilderAppendLine(result, "\tOPEN cur");
			for(int i = 0; i < table.Columns.Count; i++) {
				string columnName = table.Columns[i].IsIdentity ? IdentityColumnMagicName : table.Columns[i].Name;
				StringBuilderAppendLine(result, string.Format("\tDECLARE @{0} {1}", columnName, GetSqlCreateColumnTypeSp(table, table.Columns[i])));
			}
			StringBuilderAppendLine(result, "\tFETCH NEXT FROM cur INTO");
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = table.Columns[i].IsIdentity ? IdentityColumnMagicName : table.Columns[i].Name;
				result.Append(string.Format("\t\t@{0}", columnName));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\tWHILE(@@fetch_status <> -1)");
			StringBuilderAppendLine(result, "\tBEGIN");
			string spName = ComposeSafeTableName(string.Format("sp_{0}_xpoView_delete", table.Name));
			StringBuilderAppendLine(result, string.Format("\t\tEXEC [{0}]", spName));
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\t\t@{0}", columnName));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				StringBuilderAppendLine(result, ",");
				result.Append(string.Format("\t\t\t@{0}", table.Columns[i].Name));
			}
			FinalizeTrigger(table, result);
		}
		void AppendKeys(DBTable table, StringBuilder result) {
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				DBColumn keyColumn = GetDbColumnByName(table, table.PrimaryKey.Columns[i]);
				string dbType = GetSqlCreateColumnTypeSp(table, keyColumn);
				result.Append(string.Format("\t@{0} {1}", keyColumn.Name, dbType));
			}
		}
		void AppendWhere(DBTable table, StringBuilder result) {
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, " AND"); }
				result.Append(string.Format("\t\t[{0}] = @{0}", table.PrimaryKey.Columns[i]));
			}
			StringBuilderAppendLine(result);
		}
		void InitTrigger(DBTable table, StringBuilder result) {
			InitTriggerCore(table, result);
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = table.Columns[i].IsIdentity ? IdentityColumnMagicName : table.Columns[i].Name;
				result.Append(string.Format("\t\t\t[{0}]", columnName));
			}
			StringBuilderAppendLine(result);
		}
		void InitTriggerCore(DBTable table, StringBuilder result) {
			StringBuilderAppendLine(result, "BEGIN");
			StringBuilderAppendLine(result, "\tDECLARE cur CURSOR FOR");
			StringBuilderAppendLine(result, "\t\tSELECT");
		}
		void FinalizeTrigger(DBTable table, StringBuilder result) {
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\t\tFETCH NEXT FROM cur INTO");
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = table.Columns[i].IsIdentity ? IdentityColumnMagicName : table.Columns[i].Name;
				result.Append(string.Format("\t\t\t@{0}", columnName));
			}
			FinalizeTriggerCore(result);
		}
		void FinalizeTriggerCore(StringBuilder result) {
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\tEND");
			StringBuilderAppendLine(result, "\tCLOSE cur");
			StringBuilderAppendLine(result, "\tDEALLOCATE cur");
			StringBuilderAppendLine(result, "END");
			StringBuilderAppendLine(result, "GO");
		}
		ExecMethodDelegate commandBuilderDeriveParametersHandler;
		protected override void CommandBuilderDeriveParameters(IDbCommand command) {
			if(commandBuilderDeriveParametersHandler == null) {
				commandBuilderDeriveParametersHandler = ReflectConnectionHelper.GetCommandBuilderDeriveParametersDelegate(Connection.GetType().Assembly.FullName, "Sybase.Data.AseClient.AseCommandBuilder");
			}
			commandBuilderDeriveParametersHandler(command);
		}
		public override DBStoredProcedure[] GetStoredProcedures() {
			IDbCommand command = Connection.CreateCommand();
			command.CommandText = "select * from sysobjects where type = 'P'";
			IDataReader reader = command.ExecuteReader();
			List<DBStoredProcedure> result = new List<DBStoredProcedure>();
			while(reader.Read()) {
				DBStoredProcedure curSproc = new DBStoredProcedure();
				curSproc.Name = reader.GetString(0);
				result.Add(curSproc);
			}
			reader.Close();
			foreach(DBStoredProcedure curSproc in result) {
				try {
					command.CommandType = CommandType.StoredProcedure;
					command.CommandText = curSproc.Name;
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
					curSproc.Arguments.AddRange(dbArguments);
					command.CommandType = CommandType.Text;
					command.CommandText = string.Format(
@"set showplan on
set fmtonly on
exec [{0}] {1}
set fmtonly off
set showplan off", curSproc.Name, string.Join(", ", fakeParams.ToArray()));
					reader = command.ExecuteReader();
					DBStoredProcedureResultSet curResultSet = new DBStoredProcedureResultSet();
					List<DBNameTypePair> dbColumns = new List<DBNameTypePair>();
					for(int i = 0; i < reader.FieldCount; i++) {
						dbColumns.Add(new DBNameTypePair(reader.GetName(i), DBColumn.GetColumnType(reader.GetFieldType(i))));
					}
					curResultSet.Columns.AddRange(dbColumns);
					reader.Close();
					curSproc.ResultSets.Add(curResultSet);
				} catch {
					if(!reader.IsClosed) {
						reader.Close();
					}
				}
			}
			return result.ToArray();
		}
	}
	public class AseProviderFactory : ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return AseConnectionProvider.CreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return AseConnectionProvider.CreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(ServerParamID) || !parameters.ContainsKey(DatabaseParamID) ||
				!parameters.ContainsKey(UserIDParamID) || !parameters.ContainsKey(PasswordParamID)) {
				return null;
			}
			return AseConnectionProvider.GetConnectionString(parameters[ServerParamID], parameters[DatabaseParamID],
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
		public override string ProviderKey { get { return AseConnectionProvider.XpoProviderTypeString; } }
		public override string[] GetDatabases(string server, string userid, string password) {
			return new string[0];
		}
		public override string FileFilter { get { return null; } }
		public override bool MeanSchemaGeneration { get { return true; } }
		public override bool SupportStoredProcedures { get { return false; } }
	}
}
