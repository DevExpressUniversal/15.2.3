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
	public class PervasiveSqlConnectionProvider : ConnectionProviderSql, ISqlGeneratorFormatterEx {
		public const int PervasiveErrorCode = -4999;
		public const string XpoProviderTypeString = "Pervasive";
		const int PervasiveDataTypeKeyColumn = 227;
		const int PervasiveDataTypeIndexColumn = 255;
		ReflectConnectionHelper helper;
		ReflectConnectionHelper ConnectionHelper {
			get {
				if(helper == null)
					helper = new ReflectConnectionHelper(Connection, "Pervasive.Data.SqlClient.PsqlException");
				return helper;
			}
		}
		public static string GetConnectionString(string server, string userid, string password, string database) {
			return String.Format("{4}={5};Server={0};UID={1};PWD={2};ServerDSN={3};",
				server, userid, password, database, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString);
		}
		public static string GetConnectionString(string server, string userid, string password, string database, string encoding) {
			return String.Format("{4}={5};Server={0};UID={1};PWD={2};ServerDSN={3};encoding={6};",
				server, userid, password, database, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString, encoding);
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] { connection };
			return CreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new PervasiveSqlConnectionProvider(connection, autoCreateOption);
		}
		static PervasiveSqlConnectionProvider() {
			RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
			RegisterDataStoreProvider("Pervasive.Data.SqlClient.PsqlConnection", new DataStoreCreationFromConnectionDelegate(CreateProviderFromConnection));
			RegisterFactory(new PervasiveProviderFactory());
		}
		public static void Register() { }
		public PervasiveSqlConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		protected override string GetSqlCreateColumnTypeForBoolean(DBTable table, DBColumn column) {
			return "bit";
		}
		protected override string GetSqlCreateColumnTypeForByte(DBTable table, DBColumn column) {
			return "utinyint";
		}
		protected override string GetSqlCreateColumnTypeForSByte(DBTable table, DBColumn column) {
			return "tinyint";
		}
		protected override string GetSqlCreateColumnTypeForChar(DBTable table, DBColumn column) {
			return "varchar(4)";
		}
		protected override string GetSqlCreateColumnTypeForDecimal(DBTable table, DBColumn column) {
			return "decimal(20,4)";
		}
		protected override string GetSqlCreateColumnTypeForDouble(DBTable table, DBColumn column) {
			return "double";
		}
		protected override string GetSqlCreateColumnTypeForSingle(DBTable table, DBColumn column) {
			return "real";
		}
		protected override string GetSqlCreateColumnTypeForInt32(DBTable table, DBColumn column) {
			return "integer";
		}
		protected override string GetSqlCreateColumnTypeForUInt32(DBTable table, DBColumn column) {
			return "uinteger";
		}
		protected override string GetSqlCreateColumnTypeForInt16(DBTable table, DBColumn column) {
			return "smallint";
		}
		protected override string GetSqlCreateColumnTypeForUInt16(DBTable table, DBColumn column) {
			return "usmallint";
		}
		protected override string GetSqlCreateColumnTypeForInt64(DBTable table, DBColumn column) {
			return "bigint";
		}
		protected override string GetSqlCreateColumnTypeForUInt64(DBTable table, DBColumn column) {
			return "ubigint";
		}
		public const int MaximumStringSize = 4000;
		protected override string GetSqlCreateColumnTypeForString(DBTable table, DBColumn column) {
			if(column.Size > 0 && column.Size <= MaximumStringSize)
				return "varchar(" + column.Size.ToString(CultureInfo.InvariantCulture) + ')';
			else
				return "longvarchar";
		}
		protected override string GetSqlCreateColumnTypeForDateTime(DBTable table, DBColumn column) {
			return "timestamp";
		}
		protected override string GetSqlCreateColumnTypeForGuid(DBTable table, DBColumn column) {
			return "char(36)";
		}
		protected override string GetSqlCreateColumnTypeForByteArray(DBTable table, DBColumn column) {
			return "longvarbinary";
		}
		public override string GetSqlCreateColumnFullAttributes(DBTable table, DBColumn column) {
			if(column.IsKey && column.IsIdentity && (column.ColumnType == DBColumnType.Int32 || column.ColumnType == DBColumnType.Int64) && IsSingleColumnPKColumn(table, column)) {
				if (column.ColumnType == DBColumnType.Int64) throw new NotSupportedException(Res.GetString(Res.ConnectionProvider_TheAutoIncrementedKeyWithX0TypeIsNotSuppor, column.ColumnType, this.GetType()));
				return "identity not null";
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
			return (value as IConvertible).ToInt64(CultureInfo.InvariantCulture);
		}
		protected override IDbConnection CreateConnection() {
			return ConnectionHelper.GetConnection(ConnectionString);
		}
		public static IDbConnection CreateConnection(string connectionString) {
			return ReflectConnectionHelper.GetConnection("Pervasive.Data.SqlClient", "Pervasive.Data.SqlClient.PsqlConnection", connectionString);
		}
		protected override void CreateDataBase() {
			try {
				Connection.Open();
			} catch(Exception e) {
				throw new UnableToOpenDatabaseException(XpoDefault.ConnectionStringRemovePassword(ConnectionString), e);
			}
		}
		ExecMethodDelegate commandBuilderDeriveParametersHandler;
		protected override void CommandBuilderDeriveParameters(IDbCommand command) {
			if(commandBuilderDeriveParametersHandler == null) {
				commandBuilderDeriveParametersHandler = ReflectConnectionHelper.GetCommandBuilderDeriveParametersDelegate("Pervasive.Data.SqlClient", "Pervasive.Data.SqlClient.PsqlCommandBuilder");
			}
			commandBuilderDeriveParametersHandler(command);
		}
		protected override SelectedData ExecuteSproc(string sprocName, params OperandValue[] parameters) {
			using(IDbCommand command = CreateCommand()) {
				List<string> paramsList = new List<string>();
				int counter = 0;
				foreach(OperandValue param in parameters) {
					bool createParam = true;
					string paramName = GetParameterName(param, counter++, ref createParam);
					paramsList.Add(paramName);
					if(createParam) {
						command.Parameters.Add(CreateParameter(command, param.Value, paramName));
					}
				}
				command.CommandText = string.Format("call {0}({1})", sprocName, string.Join(", ", paramsList.ToArray()));
				List<SelectStatementResult> selectStatementResults = GetSelectedStatmentResults(command);
				return new SelectedData(selectStatementResults.ToArray());
			}
		}
		public override DBStoredProcedure[] GetStoredProcedures() {
			throw new NotSupportedException();
		}
		delegate bool TablesFilter(DBTable table);
		SelectStatementResult GetDataForTables(ICollection tables, TablesFilter filter, string queryText) {
			QueryParameterCollection parameters = new QueryParameterCollection();
			StringCollection inList = new StringCollection();
			int i = 0;
			foreach(DBTable table in tables) {
				if(filter == null || filter(table)) {
					parameters.Add(new OperandValue(ComposeSafeTableName(table.Name)));
					inList.Add("?");
					++i;
				}
			}
			if(inList.Count == 0)
				return new SelectStatementResult();
			return SelectData(new Query(string.Format(CultureInfo.InvariantCulture, queryText, StringListHelper.DelimitedText(inList, ",")), parameters, inList));
		}
		Type GetTypeFromString(string typeName) {
			switch(typeName) {
				case "int":
					return typeof(int);
				case "image":
					return typeof(byte[]);
				case "varchar":
				case "longvarchar":
					return typeof(string);
				case "bit":
					return typeof(bool);
				case "tinyint":
					return typeof(byte);
				case "smallint":
					return typeof(short);
				case "bigint":
					return typeof(long);
				case "numeric":
					return typeof(Decimal);
				case "nchar":
					return typeof(char);
				case "char":
					return typeof(char);
				case "money":
					return typeof(Decimal);
				case "float":
					return typeof(float);
				case "uniqueidentifier":
					return typeof(Guid);
				case "nvarchar":
					return typeof(string);
				case "datetime":
					return typeof(DateTime);
				case "ntext":
					return typeof(string);
			}
			return typeof(object);
		}
		DBColumnType GetDBColumnTypeByDBInfo(byte dataType, UInt16 size, byte dec, UInt16 flags, out int columnSize) {
			columnSize = 0;
			switch(dataType) {
				case 0: 
					if((flags & 0x1000) != 0x1000) { 
						switch(size) {
							case 1:
								return DBColumnType.Char;
							case 36:
								return DBColumnType.Guid;
							default:
								columnSize = size;
								return DBColumnType.String;
						}
					}
					goto default;
				case 1: 
					switch(size) {
						case 1:
							return DBColumnType.Byte;
						case 2:
							return DBColumnType.Int16;
						case 4:
							return DBColumnType.Int32;
						case 8:
							return DBColumnType.Int64;
					}
					goto default;
				case 2:
					switch(size) {
						case 4:
							return DBColumnType.Single;
						case 8:
							return DBColumnType.Double;
					}
					goto default;
				case 3:
					return DBColumnType.DateTime;
				case 5: 
					return DBColumnType.Decimal;
				case 8: 
					switch(size) {
						case 3:
							return DBColumnType.SByte;
						case 5:
							return DBColumnType.UInt16;
						case 10:
							return DBColumnType.UInt32;
						case 20:
							return DBColumnType.UInt64;
					}
					goto default;
				case 11: 
					columnSize = size - 1;
					return DBColumnType.String;
				case 12: 
					columnSize = -1;
					return DBColumnType.String;
				case 14: 
					switch(size) {
						case 1:
							return DBColumnType.Byte;
						case 2:
							return DBColumnType.UInt16;
						case 4:
							return DBColumnType.UInt32;
						case 8:
							return DBColumnType.UInt64;
					}
					goto default;
				case 15: 
					switch(size) {
						case 4:
							return DBColumnType.Int32;
						case 2:
							return DBColumnType.Int16;
					}
					goto default;
				case 7:
				case 16: 
					return DBColumnType.Boolean;
				case 20: 
					return DBColumnType.DateTime;
				case 21: 
					if((flags & 0x1000) == 0x1000) {
						return DBColumnType.ByteArray;
					} else {
						return DBColumnType.String;
					}
				default:
					return DBColumnType.Unknown;
			}
		}
		void GetColumns(DBTable table) {
			foreach(SelectStatementResultRow row in SelectData(new Query("select c.Xe$Name, c.Xe$DataType, c.Xe$Size, c.Xe$Dec, c.Xe$Flags from X$Field c join X$File t on c.Xe$File = t.Xf$Id where t.Xf$Name = ? order by c.Xe$Id", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" })).Rows) {
				int columnSize;
				byte dataType = Convert.ToByte(row.Values[1]);
				if(dataType == PervasiveDataTypeKeyColumn || dataType == PervasiveDataTypeIndexColumn)
					continue;
				DBColumnType columnType = GetDBColumnTypeByDBInfo(dataType, Convert.ToUInt16(row.Values[2]), row.Values[3] is byte ? (byte)row.Values[3] : (byte)0, Convert.ToUInt16(row.Values[4]), out columnSize);
				table.AddColumn(new DBColumn(((string)row.Values[0]).TrimEnd(), false, String.Empty, columnSize, columnType));
			}
		}
		void GetPrimaryKey(DBTable table) {
			SelectStatementResult data = SelectData(new Query(@"select c.Xe$Name from X$Index i join X$File t on i.Xi$File = t.Xf$Id 
join X$Field c on i.Xi$File = c.Xe$File and i.Xi$Field = c.Xe$Id 
where i.Xi$Flags & 16384 <> 0 and t.Xf$Name = ?
order by i.Xi$Part", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" }));
			if(data.Rows.Length > 0) {
				StringCollection cols = new StringCollection();
				foreach(SelectStatementResultRow row in data.Rows) {
					string columnName = ((string)row.Values[0]).TrimEnd();
					DBColumn column = table.GetColumn(columnName);
					if(column != null)
						column.IsKey = true;
					cols.Add(columnName);
				}
				table.PrimaryKey = new DBPrimaryKey(cols);
			}
		}
		public override void CreateIndex(DBTable table, DBIndex index) {
			if(table.Name != "XPObjectType")
				base.CreateIndex(table, index);
		}
		void GetIndexes(DBTable table) {
			SelectStatementResult data = SelectData(new Query(
				@"select i.Xi$Number, c.Xe$Name, i.Xi$Flags & 1 from X$Index i
join X$File t on i.Xi$File = t.Xf$Id
join X$Field c on i.Xi$File = c.Xe$File and i.Xi$Field = c.Xe$Id
where t.Xf$Name = ? and i.Xi$Flags & 16384 = 0
order by i.Xi$Number, i.Xi$Part", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" }));
			Hashtable idxs = new Hashtable();
			foreach(SelectStatementResultRow row in data.Rows) {
				DBIndex index = (DBIndex)idxs[row.Values[0]];
				if(index == null) {
					StringCollection list = new StringCollection();
					list.Add(((string)row.Values[1]).TrimEnd());
					index = new DBIndex(String.Empty, list, (Convert.ToInt32(row.Values[2]) & 1) == 0 ? true : false);
					table.Indexes.Add(index);
					idxs[row.Values[0]] = index;
				} else
					index.Columns.Add(((string)row.Values[1]).TrimEnd());
			}
		}
		void GetForeignKeys(DBTable table) {
			SelectStatementResult data = SelectData(new Query(
				@"select r.Xr$Name, fc.Xe$Name, pc.Xe$Name, p.Xf$Name from X$Relate r
join X$File f on f.Xf$Id = r.Xr$FId
join X$File p on p.Xf$Id = r.Xr$PId
join X$Index fi on fi.Xi$Number = r.Xr$FIndex and fi.Xi$File = r.Xr$FId
join X$Index pi1 on pi1.Xi$Number = r.Xr$Index and pi1.Xi$File = r.Xr$PId and pi1.Xi$Part = fi.Xi$Part 
join X$Field fc on fc.Xe$Id = fi.Xi$Field
join X$Field pc on pc.Xe$Id = pi1.Xi$Field
where f.Xf$Name = ?
order by r.Xr$Name, fi.Xi$Part", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" }));
			Hashtable fks = new Hashtable();
			foreach(SelectStatementResultRow row in data.Rows) {
				DBForeignKey fk = (DBForeignKey)fks[row.Values[0]];
				if(fk == null) {
					StringCollection pkc = new StringCollection();
					StringCollection fkc = new StringCollection();
					pkc.Add(((string)row.Values[2]).TrimEnd());
					fkc.Add(((string)row.Values[1]).TrimEnd());
					fk = new DBForeignKey(fkc, ((string)row.Values[3]).TrimEnd(), pkc);
					table.ForeignKeys.Add(fk);
					fks[row.Values[0]] = fk;
				} else {
					fk.Columns.Add(((string)row.Values[1]).TrimEnd());
					fk.PrimaryKeyTableKeyColumns.Add(((string)row.Values[2]).TrimEnd());
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
			foreach(SelectStatementResultRow row in GetDataForTables(tables, null, "select Xf$Name from X$File where Xf$Name in ({0})").Rows)
				dbTables.Add(((string)row.Values[0]).TrimEnd(), false);
			foreach(SelectStatementResultRow row in GetDataForTables(tables, null, "select Xv$Name from X$View where Xv$Name in ({0})").Rows)
				dbTables.Add(((string)row.Values[0]).TrimEnd(), true);
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
			return 20;
		}
		public override string FormatTable(string schema, string tableName) {
			return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", tableName);
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "\"{0}\" {1}", tableName, tableAlias);
		}
		public override string FormatColumn(string columnName) {
			return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", columnName);
		}
		public override string FormatColumn(string columnName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "{1}.\"{0}\"", columnName, tableAlias);
		}
		public override string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int topSelectedRecords) {
			string modificatorsSql = string.Format(CultureInfo.InvariantCulture, (topSelectedRecords != 0) ? "top {0} " : string.Empty, topSelectedRecords);
			string expandedWhereSql = whereSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}where {1}", Environment.NewLine, whereSql) : string.Empty;
			string expandedOrderBySql = orderBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}order by {1}", Environment.NewLine, orderBySql) : string.Empty;
			string expandedHavingSql = havingSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}having {1}", Environment.NewLine, havingSql) : string.Empty;
			string expandedGroupBySql = groupBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}group by {1}", Environment.NewLine, groupBySql) : string.Empty;
			return string.Format(CultureInfo.InvariantCulture, "select {0}{1} from {2}{3}{4}{5}{6}", modificatorsSql, selectedPropertiesSql, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedOrderBySql);
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
		public override string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.Atn2:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(case when ({1}) = 0 then Sign({0}) * Atan(1) * 2 else Atan2({0},  {1}) end)", operands[0], operands[1]);
				case FunctionOperatorType.Cosh:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "((Exp({0}) + Exp(-({0}))) / 2.0)", operands[0]);
				case FunctionOperatorType.Sinh:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "((Exp({0}) - Exp(-({0}))) / 2.0)", operands[0]);
				case FunctionOperatorType.Tanh:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "((Exp({0}) - Exp(-({0}))) / (Exp({0}) + Exp(-({0}))))", operands[0]);
				case FunctionOperatorType.Substring:
					switch(operands.Length) {
						case 2:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "Substring({0}, ({1}) + 1, Length({0}) - ({1}))", operands[0], operands[1]);
						case 3:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "Substring({0}, ({1}) + 1, {2})", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.CharIndex:
					switch(operands.Length) {
						case 2:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "(LOCATE({0}, {1}) - 1)", operands[0], operands[1]);
						case 3:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "(LOCATE({0}, {1}, ({2}) + 1) - 1)", operands[0], operands[1], operands[2]);
						case 4:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "(LOCATE({0}, SUBSTRING({1}, 1, ({2}) + ({3})), ({2}) + 1) - 1)", operands[0], operands[1], operands[2], operands[3]);
					}
					goto default;
				case FunctionOperatorType.PadLeft:
					switch(operands.Length) {
						case 2:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "isnull(Concat(SPACE((({1}) - Length({0}))), ({0})), {0})", operands[0], operands[1]);
						case 3:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "isnull(Concat(REPLICATE({2}, (({1}) - Length({0}))), ({0})), {0})", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.PadRight:
					switch(operands.Length) {
						case 2:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "isnull(Concat(({0}), SPACE((({1}) - Length({0})))), {0})", operands[0], operands[1]);
						case 3:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "isnull(Concat(({0}), REPLICATE({2}, (({1}) - Length({0})))), {0})", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.AddTicks:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "TIMESTAMPADD(SQL_TSI_SECOND, MOD(CAST(CAST(({1}) as double) / 10000000 as bigint),86400), TIMESTAMPADD(SQL_TSI_DAY, CAST(CAST(({1}) as double) / 864000000000 as bigint), {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddMilliSeconds:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "TIMESTAMPADD(SQL_TSI_SECOND, CAST(({1}) / 1000 as bigint), {0})", operands[0], operands[1]);
				case FunctionOperatorType.AddTimeSpan:
				case FunctionOperatorType.AddSeconds:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "TIMESTAMPADD(SQL_TSI_SECOND, CAST(({1}) as bigint), {0})", operands[0], operands[1]);
				case FunctionOperatorType.AddMinutes:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "TIMESTAMPADD(SQL_TSI_SECOND, MOD(CAST(CAST(({1}) as double) * 60 as bigint),86400), TIMESTAMPADD(SQL_TSI_DAY, CAST((CAST(({1}) as double) * 60) / 86400 as bigint), {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddHours:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "TIMESTAMPADD(SQL_TSI_SECOND, MOD(CAST(CAST(({1}) as double) * 3600 as bigint),86400), TIMESTAMPADD(SQL_TSI_DAY, CAST((CAST(({1}) as double) * 3600) / 86400 as bigint), {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddDays:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "TIMESTAMPADD(SQL_TSI_SECOND, MOD(CAST(CAST(({1}) as double) * 86400 as bigint),86400), TIMESTAMPADD(SQL_TSI_DAY, CAST(CAST(({1}) as double) as bigint), {0}))", operands[0], operands[1]);
				case FunctionOperatorType.GetTimeOfDay:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "((CAST(HOUR({0}) as BIGINT) *  36000000000) + (CAST(MINUTE({0}) as BIGINT) * 600000000) + (CAST(SECOND({0}) as BIGINT) * 10000000))", operands[0]);
				case FunctionOperatorType.EndsWith:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(SUBSTRING({0}, LENGTH({0}) - LENGTH({1}) + 1, LENGTH({1})) = ({1}))", operands[0], operands[1]);
				case FunctionOperatorType.Contains:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(LOCATE({1}, {0}) >= 1)", operands[0], operands[1]);
				case FunctionOperatorType.StartsWith:
					object secondOperand = operands[1];
					if(secondOperand is OperandValue && ((OperandValue)secondOperand).Value is string) {
						string operandString = (string)((OperandValue)secondOperand).Value;
						int likeIndex = operandString.IndexOfAny(achtungChars);
						if(likeIndex < 0) {
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "({0} like {1})", operands[0], new ConstantValue(operandString + "%"));
						} else if(likeIndex > 0) {
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "(({0} like {2}) And (LOCATE({1}, {0}) = 1))", operands[0], secondOperand, new ConstantValue(operandString.Substring(0, likeIndex) + "%"));
						}
					}
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(LOCATE({1}, {0}) = 1)", operands[0], operands[1]);				
				default:
					return base.FormatFunction(processParameter, operatorType, operands);
			}
		}
		readonly static char[] achtungChars = new char[] { '_', '%' };
		public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.Abs:
					return string.Format(CultureInfo.InvariantCulture, "Abs({0})", operands[0]);
				case FunctionOperatorType.Sign:
					return string.Format(CultureInfo.InvariantCulture, "Sign({0})", operands[0]);
				case FunctionOperatorType.BigMul:
					return string.Format(CultureInfo.InvariantCulture, "CAST((({0}) * ({1})) as BIGINT)", operands[0], operands[1]);
				case FunctionOperatorType.Cos:
					return string.Format(CultureInfo.InvariantCulture, "Cos({0})", operands[0]);
				case FunctionOperatorType.Sin:
					return string.Format(CultureInfo.InvariantCulture, "Sin({0})", operands[0]);
				case FunctionOperatorType.Tan:
					return string.Format(CultureInfo.InvariantCulture, "Tan({0})", operands[0]);
				case FunctionOperatorType.Atn:
					return string.Format(CultureInfo.InvariantCulture, "Atan({0})", operands[0]);
				case FunctionOperatorType.Acos:
					return string.Format(CultureInfo.InvariantCulture, "Acos({0})", operands[0]);
				case FunctionOperatorType.Asin:
					return string.Format(CultureInfo.InvariantCulture, "Asin({0})", operands[0]);
				case FunctionOperatorType.Log:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "Log({0})", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(Log({0}) / Log({1}))", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Log10:
					return string.Format(CultureInfo.InvariantCulture, "Log10({0})", operands[0]);
				case FunctionOperatorType.Exp:
					return string.Format(CultureInfo.InvariantCulture, "Exp({0})", operands[0]);
				case FunctionOperatorType.Power:
					return string.Format(CultureInfo.InvariantCulture, "Power({0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Sqr:
					return string.Format(CultureInfo.InvariantCulture, "Sqrt({0})", operands[0]);
				case FunctionOperatorType.Max:
					return string.Format(CultureInfo.InvariantCulture, "if({0} > {1}, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Min:
					return string.Format(CultureInfo.InvariantCulture, "if({0} < {1}, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Floor:
					return string.Format(CultureInfo.InvariantCulture, "Floor({0})", operands[0]);
				case FunctionOperatorType.Ceiling:
					return string.Format(CultureInfo.InvariantCulture, "Ceiling({0})", operands[0]);
				case FunctionOperatorType.Round:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "Round({0}, 0)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "Round({0}, {1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Ascii:
					return string.Format(CultureInfo.InvariantCulture, "Ascii({0})", operands[0]);
				case FunctionOperatorType.Char:
					return string.Format(CultureInfo.InvariantCulture, "Char({0})", operands[0]);
				case FunctionOperatorType.ToInt:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS int)", operands[0]);
				case FunctionOperatorType.ToLong:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS bigint)", operands[0]);
				case FunctionOperatorType.ToFloat:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS real)", operands[0]);
				case FunctionOperatorType.ToDouble:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS double)", operands[0]);
				case FunctionOperatorType.ToDecimal:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS decimal(20,4))", operands[0]);
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, "CONVERT({0}, SQL_VARCHAR)", operands[0]);
				case FunctionOperatorType.Len:
					return string.Format(CultureInfo.InvariantCulture, "Length({0})", operands[0]);
				case FunctionOperatorType.Remove:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "LEFT({0}, {1})", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "Stuff({0}, ({1})+1, {2}, '')", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.Replace:
					return string.Format(CultureInfo.InvariantCulture, "(Replace({0}, {1}, {2}) + '')", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Insert:
					return string.Format(CultureInfo.InvariantCulture, "Stuff({0}, ({1})+1, 0, {2})", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.GetMilliSecond:
					throw new NotSupportedException();
				case FunctionOperatorType.Concat:
					string args = String.Empty;
					foreach(string arg in operands) {
						args = (args.Length > 0) ? string.Concat("Concat(", args, ", ", arg, ")") : arg;
					}
					return args;
				case FunctionOperatorType.AddMonths:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_MONTH, ({1}), ({0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddYears:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_YEAR, ({1}), ({0}))", operands[0], operands[1]);
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
				case FunctionOperatorType.GetDayOfYear:
					return string.Format(CultureInfo.InvariantCulture, "DAYOFYEAR({0})", operands[0]);
				case FunctionOperatorType.GetDayOfWeek:
					return string.Format(CultureInfo.InvariantCulture, "MOD(DAYOFWEEK({0}) - DAYOFWEEK('1900-01-01') + 8, 7)", operands[0]);
				case FunctionOperatorType.GetDate:
					return string.Format(CultureInfo.InvariantCulture, "Cast({0} as Date)", operands[0]);
				case FunctionOperatorType.DateDiffYear:
					return string.Format(CultureInfo.InvariantCulture, "CAST(TIMESTAMPDIFF(SQL_TSI_YEAR, ({0}), ({1})) as INT)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMonth:
					return string.Format(CultureInfo.InvariantCulture, "CAST(TIMESTAMPDIFF(SQL_TSI_MONTH, ({0}), ({1})) as INT)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffDay:
					return string.Format(CultureInfo.InvariantCulture, "CAST(TIMESTAMPDIFF(SQL_TSI_DAY, ({0}), ({1})) as INT)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffHour:
					return string.Format(CultureInfo.InvariantCulture, "CAST(TIMESTAMPDIFF(SQL_TSI_HOUR, ({0}), ({1})) as INT)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMinute:
					return string.Format(CultureInfo.InvariantCulture, "CAST(TIMESTAMPDIFF(SQL_TSI_MINUTE, ({0}), ({1})) as INT)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffSecond:
					return string.Format(CultureInfo.InvariantCulture, "CAST(TIMESTAMPDIFF(SQL_TSI_SECOND, ({0}), ({1})) as INT)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "(CAST(TIMESTAMPDIFF(SQL_TSI_SECOND, ({0}), ({1})) as INT) * 1000)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffTick:
					return string.Format(CultureInfo.InvariantCulture, "(CAST(TIMESTAMPDIFF(SQL_TSI_SECOND, ({0}), ({1})) as INT) * 10000000)", operands[0], operands[1]);
				case FunctionOperatorType.Now:
					return "Now()";
				case FunctionOperatorType.UtcNow:
					return "CURRENT_TIMESTAMP()";
				case FunctionOperatorType.Today:
					return "cast(now() as date)";
				case FunctionOperatorType.Rnd:
					return "rand()";
				case FunctionOperatorType.IsNull:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "(({0}) is null)", operands[0]);
					}
					goto default;
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(CultureInfo.InvariantCulture, "({0} is null or length({0}) = 0)", operands[0]);
				default:
					return base.FormatFunction(operatorType, operands);
			}
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
					return "?";
			}
		}
		public override bool SupportNamedParameters { get { return false; } }
		public override void CreateForeignKey(DBTable table, DBForeignKey fk) {
			if(fk.PrimaryKeyTable == table.Name)
				return;
			base.CreateForeignKey(table, fk);
		}
		protected override IDataParameter CreateParameter(IDbCommand command, object value, string name) {
			IDataParameter param = CreateParameter(command);
			param.Value = value;
			param.ParameterName = name;
			if(value is byte[])
				param.DbType = DbType.Binary;
			if(value is string)
				param.DbType = DbType.String;
			return param;
		}
		public override string FormatConstraint(string constraintName) {
			return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", constraintName);
		}
		void ClearDatabase(IDbCommand command) {
			string[] tables = GetStorageTablesList(false);
			if(tables.Length == 0)
				return;
			SelectStatementResult constraints = SelectData(new Query("select r.Xr$Name, f.Xf$Name from X$Relate r join X$File f on f.Xf$Id = r.Xr$FId"));
			foreach(SelectStatementResultRow row in constraints.Rows) {
				command.CommandText = "alter table \"" + (string)row.Values[1] + "\" drop constraint \"" + (string)row.Values[0] + "\"";
				command.ExecuteNonQuery();
			}
			foreach(string table in tables) {
				command.CommandText = "drop table \"" + table + "\"";
				command.ExecuteNonQuery();
			}
		}
		protected override Exception WrapException(Exception e, IDbCommand query) {
			object[] properiesValues;
			if(ConnectionHelper.TryGetExceptionProperties(e, new string[] { "Message", "Errors" }, out properiesValues)) {
				string message = (string)properiesValues[0];
				ICollection errors = (ICollection)properiesValues[1];
				if(errors.Count > 0) {
					foreach(object error in errors) {
						int number = (int)ReflectConnectionHelper.GetPropertyValue(error, "Number");
						if(number == PervasiveErrorCode + 71 || number == PervasiveErrorCode + 5) {
							return new ConstraintViolationException(query.CommandText, GetParametersString(query), e);
						}
						break;
					}
				}
				if(message.Contains("[LNA][Pervasive][ODBC Engine Interface][Data Record Manager]There is a violation of the RI definitions") || message.Contains("(Btrieve Error 71)") ||
					message.Contains("[LNA][Pervasive][ODBC Engine Interface][Data Record Manager]The record has a key field containing a duplicate value") || message.Contains("(Btrieve Error 5)"))
					return new ConstraintViolationException(query.CommandText, GetParametersString(query), e);
				if(message.Contains("[LNA][Pervasive][ODBC Engine Interface]Invalid column name:") ||
					message.Contains("[LNA][Pervasive][ODBC Engine Interface][Data Record Manager]No such table or object.") ||
					message.Contains("[LNA][Pervasive][ODBC Engine Interface]Error in expression:"))
					return new SchemaCorrectionNeededException(e);
			}
			return base.WrapException(e, query);
		}
		protected override void ProcessClearDatabase() {
			IDbCommand command = CreateCommand();
			ClearDatabase(command);
		}
		public override string[] GetStorageTablesList(bool includeViews) {
			SelectStatementResult tables = SelectData(new Query("select Xf$Name from X$File where Xf$Flags & 16 = 0"));
			SelectStatementResult views = includeViews ? SelectData(new Query("select Xv$Name from X$View")) : null;
			List<string> result = new List<string>(tables.Rows.Length + (views == null ? 0 : views.Rows.Length));
			foreach(SelectStatementResultRow row in tables.Rows) {
				result.Add(((string)row.Values[0]).TrimEnd(' '));
			}
			if(views != null) {
				foreach(SelectStatementResultRow row in views.Rows) {
					result.Add(((string)row.Values[0]).TrimEnd(' '));
				}
			}
			return result.ToArray();
		}
		bool inSchemaUpdate;
		protected override void BeginTransactionCore(object il) {
			if(!inSchemaUpdate)
				base.BeginTransactionCore(il);
		}
		protected override void CommitTransactionCore() {
			if(!inSchemaUpdate)
				base.CommitTransactionCore();
		}
		protected override void RollbackTransactionCore() {
			if(!inSchemaUpdate)
				base.RollbackTransactionCore();
		}
		protected override UpdateSchemaResult ProcessUpdateSchema(bool skipIfFirstTableNotExists, params DBTable[] tables) {
			inSchemaUpdate = true;
			try {
				return base.ProcessUpdateSchema(skipIfFirstTableNotExists, tables);
			} finally {
				inSchemaUpdate = false;
			}
		}
	}
	public class PervasiveProviderFactory : ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return PervasiveSqlConnectionProvider.CreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return PervasiveSqlConnectionProvider.CreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(ServerParamID) || !parameters.ContainsKey(DatabaseParamID) ||
				!parameters.ContainsKey(UserIDParamID) || !parameters.ContainsKey(PasswordParamID)) {
				return null;
			}
			return PervasiveSqlConnectionProvider.GetConnectionString(parameters[ServerParamID], parameters[UserIDParamID],
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
		public override string ProviderKey { get { return PervasiveSqlConnectionProvider.XpoProviderTypeString; } }
		public override string[] GetDatabases(string server, string userid, string password) {
			return new string[0];
		}
		public override string FileFilter { get { return null; } }
		public override bool MeanSchemaGeneration { get { return true; } }
	}
}
