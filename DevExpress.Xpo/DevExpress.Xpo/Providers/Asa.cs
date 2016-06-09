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
	using System.Reflection;
	public class AsaConnectionProvider : ConnectionProviderSql, ISqlGeneratorFormatterEx {
		ReflectConnectionHelper helper;
		ReflectConnectionHelper ConnectionHelper {
			get {
				if(helper == null)
					helper = new ReflectConnectionHelper(Connection, "iAnywhere.Data.SQLAnywhere.SAException");
				return helper;
			}
		}
		public const string XpoProviderTypeString = "Asa";
		public static string GetConnectionString(string databaseFile, string userid, string password) {
			return String.Format("{3}={4};uid={0};pwd={1};dbf={2};persist security info=true;", userid, password, databaseFile, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString);
		}
		public static string GetConnectionString(string server, string database, string userid, string password) {
			return String.Format("{4}={5};eng={0};uid={1};pwd={2};dbn={3};persist security info=true;",
				server, userid, password, database, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString);
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] { connection };
			return CreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new AsaConnectionProvider(connection, autoCreateOption);
		}
		static AsaConnectionProvider() {
			RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
			RegisterDataStoreProvider("iAnywhere.Data.SQLAnywhere.SAConnection", new DataStoreCreationFromConnectionDelegate(CreateProviderFromConnection));
			RegisterFactory(new AsaProviderFactory());
		}
		public static void Register() { }
		public AsaConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
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
			return "char(1)";
		}
		protected override string GetSqlCreateColumnTypeForDecimal(DBTable table, DBColumn column) {
			return "money";
		}
		protected override string GetSqlCreateColumnTypeForDouble(DBTable table, DBColumn column) {
			return "double precision";
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
			return "datetime";
		}
		protected override string GetSqlCreateColumnTypeForGuid(DBTable table, DBColumn column) {
			return "uniqueidentifier";
		}
		protected override string GetSqlCreateColumnTypeForByteArray(DBTable table, DBColumn column) {
			return "image";
		}
		public override string GetSqlCreateColumnFullAttributes(DBTable table, DBColumn column) {
			string result = GetSqlCreateColumnType(table, column);
			if(column.IsKey)
				result += " NOT NULL";
			else
				result += " NULL";
			if(column.IsKey && column.IsIdentity && (column.ColumnType == DBColumnType.Int32 || column.ColumnType == DBColumnType.Int64) && IsSingleColumnPKColumn(table, column)) {
				result += " IDENTITY";
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
			return ReflectConnectionHelper.GetConnection("iAnywhere.Data.SQLAnywhere", "iAnywhere.Data.SQLAnywhere.SAConnection", connectionString);
		}
		protected override Exception WrapException(Exception e, IDbCommand query) {
			object o;
			if(ConnectionHelper.TryGetExceptionProperty(e, "Errors", out o) && ((ICollection)o).Count > 0) {
				object error = ReflectConnectionHelper.GetCollectionFirstItem((ICollection)o);
				int code = (int)ReflectConnectionHelper.GetPropertyValue(error, "NativeError");
				;
				if(code == -143 || code == -141)
					return new SchemaCorrectionNeededException(e);
				if(code == -198 || code == -193 || code == -196)
					return new ConstraintViolationException(query.CommandText, GetParametersString(query), e);
			}
			return base.WrapException(e, query);
		}
		protected override void CreateDataBase() {
			const int CannotOpenDatabaseError = -83;
			try {
				Connection.Open();
			} catch(Exception e) {
				object o;
				if(ConnectionHelper.TryGetExceptionProperty(e, "Errors", out o) && ((ICollection)o).Count > 0) {
					object error = ReflectConnectionHelper.GetCollectionFirstItem((ICollection)o);
					int code = (int)ReflectConnectionHelper.GetPropertyValue(error, "NativeError");
					;
					if(code == CannotOpenDatabaseError) {
						throw new UnableToOpenDatabaseException(XpoDefault.ConnectionStringRemovePassword(ConnectionString), e);
					}
				}
				throw;
			}
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
		Type GetTypeFromString(string typeName, int length) {
			return DBColumn.GetType(GetDbTypeFromString(typeName, length));
		}
		DBColumnType GetDbTypeFromString(string typeStr, int length) {
			switch(typeStr) {
				case "int":
				case "integer":
					return DBColumnType.Int32;
				case "image":
				case "binary":
				case "long binary":
				case "varbinary":
					return DBColumnType.ByteArray;
				case "varchar":
				case "long varchar":
				case "nvarchar":
				case "long nvarchar":
				case "ntext":
				case "text":
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
				case "money":
				case "decimal":
					return DBColumnType.Decimal;
				case "nchar":
				case "char":
					return length == 1 ? DBColumnType.Char : DBColumnType.String;
				case "float":
				case "double":
				case "real":
					return DBColumnType.Double;
				case "uniqueidentifier":
					return DBColumnType.Guid;
				case "date":
				case "datetime":
					return DBColumnType.DateTime;
			}
			return DBColumnType.Unknown;
		}
		DBColumnType GetTypeFromNumber(byte type, byte prec, int length, short userType, byte charsize, out int len) {
			len = 0;
			switch(type) {
				case 52:
					return DBColumnType.Int16;
				case 56:
					return DBColumnType.Int32;
				case 38:
					return DBColumnType.Int64;
				case 48:
					return DBColumnType.Byte;
				case 34:
					return DBColumnType.ByteArray;
				case 45:
					return DBColumnType.Guid;
				case 60:
				case 63:
					return DBColumnType.Decimal;
				case 59:
					return DBColumnType.Single;
				case 35:
				case 39:
					len = userType == 2 ? length : length / charsize;
					return DBColumnType.String;
				case 47:
					if(length == 1){
						return DBColumnType.Char;
					}
					return DBColumnType.String;
				case 50:
					return DBColumnType.Boolean;
				case 61:
					return DBColumnType.DateTime;
				case 62:
					return DBColumnType.Double;
			}
			return DBColumnType.Unknown;
		}
		void GetColumns(DBTable table) {
			foreach(SelectStatementResultRow row in SelectData(new Query("select c.name, c.type, c.prec, c.length, c.usertype, @@ncharsize from dbo.syscolumns c left join dbo.sysobjects t on c.id = t.id where t.name = ?", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" })).Rows) {
				int len;
				DBColumnType type = GetTypeFromNumber(Convert.ToByte(row.Values[1]), ((row.Values[2] is DBNull) || (Convert.ToInt32(row.Values[2]) > byte.MaxValue)) ? (byte)0 : Convert.ToByte(row.Values[2]), Convert.ToInt32(row.Values[3]), Convert.ToInt16(row.Values[4]), Convert.ToByte(row.Values[5]), out len);
				table.AddColumn(new DBColumn((string)row.Values[0], false, String.Empty, len, type));
			}
		}
		void GetPrimaryKey(DBTable table) {
			SelectStatementResult data = SelectData(new Query("select c.column_name, c.[default] from syscolumn c join systable t on c.table_id = t.table_id where t.table_name = ? and c.pkey='Y'", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" }));
			if(data.Rows.Length > 0) {
				StringCollection cols = new StringCollection();
				foreach(SelectStatementResultRow row in data.Rows) {
					string columnName = (string)row.Values[0];
					DBColumn column = table.GetColumn(columnName);
					if(column != null)
						column.IsKey = true;
					cols.Add(columnName);
					if(string.Equals(row.Values[1] as string, "autoincrement")) {
						for(int i = 0; i < table.Columns.Count; i++) {
							if(table.Columns[i].Name == columnName) {
								table.Columns[i].IsIdentity = true;
							}
						}
					}
				}
				table.PrimaryKey = new DBPrimaryKey(cols);
			}
		}
		void GetIndexes(DBTable table) {
			SelectStatementResult data = SelectData(new Query(
@"select i.index_id, cols.column_name, ind.""unique""
  from sysixcol i
  join systable tbl on tbl.table_id = i.table_id
  join syscolumn cols on i.column_id = cols.column_id and i.table_id = cols.table_id
  join sysidx ind on ind.index_id = i.index_id and ind.table_id = i.table_id
  where tbl.table_name = ?
order by i.index_id, i.sequence", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" }));
			Hashtable idxs = new Hashtable();
			foreach(SelectStatementResultRow row in data.Rows) {
				DBIndex index = (DBIndex)idxs[row.Values[0]];
				if(index == null) {
					StringCollection list = new StringCollection();
					list.Add((string)row.Values[1]);
					index = new DBIndex(null, list, ((byte)row.Values[2] & 3) != 0 ? true : false);
					table.Indexes.Add(index);
					idxs[row.Values[0]] = index;
				} else
					index.Columns.Add((string)row.Values[1]);
			}
		}
		void GetForeignKeys(DBTable table) {
			SelectStatementResult data = SelectData(new Query(
@"select f.foreign_key_id, pc.column_name, fc.column_name, pt.table_name
  from sysfkcol fkdata
  join sysforeignkey f on f.foreign_key_id = fkdata.foreign_key_id and f.foreign_table_id = fkdata.foreign_table_id
  join systable ft on ft.table_id = f.foreign_table_id
  join systable pt on pt.table_id = f.primary_table_id
  join syscolumn fc on fkdata.foreign_column_id = fc.column_id and f.foreign_table_id = fc.table_id
  join syscolumn pc on fkdata.primary_column_id = pc.column_id and f.primary_table_id = pc.table_id
where ft.table_name = ?
order by fkdata.foreign_key_id", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" }));
			Hashtable fks = new Hashtable();
			foreach(SelectStatementResultRow row in data.Rows) {
				DBForeignKey fk = (DBForeignKey)fks[row.Values[0]];
				if(fk == null) {
					StringCollection pkc = new StringCollection();
					StringCollection fkc = new StringCollection();
					pkc.Add((string)row.Values[1]);
					fkc.Add((string)row.Values[2]);
					fk = new DBForeignKey(fkc, (string)row.Values[3], pkc);
					table.ForeignKeys.Add(fk);
					fks[row.Values[0]] = fk;
				} else {
					fk.Columns.Add((string)row.Values[2]);
					fk.PrimaryKeyTableKeyColumns.Add((string)row.Values[1]);
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
		ExecMethodDelegate commandBuilderDeriveParametersHandler;
		protected override void CommandBuilderDeriveParameters(IDbCommand command) {
			if(commandBuilderDeriveParametersHandler == null) {
				commandBuilderDeriveParametersHandler = ReflectConnectionHelper.GetCommandBuilderDeriveParametersDelegate("iAnywhere.Data.SQLAnywhere", "iAnywhere.Data.SQLAnywhere.SACommandBuilder");
			}
			commandBuilderDeriveParametersHandler(command);
		}
		public override string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.PadLeft:
					switch(operands.Length) {
						case 2:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "InsertStr(0,{0},Space(({1} - Length({0}))))", operands[0], operands[1]);
						case 3:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "(space((Length({0})*0)) + space(({1}*0)) + Repeat({2},{1} - Length({0})) + {0})", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.PadRight:
					switch(operands.Length) {
						case 2:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "InsertStr(Length({0}),{0},Space({1} - Length({0})))", operands[0], operands[1]);
						case 3:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "({0} + space(({1}*0)) + Repeat({2},{1} - Length({0})))", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.EndsWith:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(Right({0}, Len({1})) = ({1}))", operands[0], operands[1]);
				case FunctionOperatorType.StartsWith:
					object secondOperand = operands[1];
					if(secondOperand is OperandValue && ((OperandValue)secondOperand).Value is string) {
						string operandString = (string)((OperandValue)secondOperand).Value;
						int likeIndex = operandString.IndexOfAny(achtungChars);
						if(likeIndex < 0) {
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "({0} like {1})", operands[0], new ConstantValue(operandString + "%"));
						} else if(likeIndex > 0) {
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "(({0} like {2}) And (CharIndex({1}, {0}) = 1))", operands[0], secondOperand, new ConstantValue(operandString.Substring(0, likeIndex) + "%"));
						}
					}
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(CharIndex({1}, {0}) = 1)", operands[0], operands[1]);
				default:
					return base.FormatFunction(processParameter, operatorType, operands);
			}
		}
		public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.BigMul:
					return string.Format(CultureInfo.InvariantCulture, "(Cast({0} as BigInt) * Cast({1} as BigInt))", operands[0], operands[1]);
				case FunctionOperatorType.Abs:
					return string.Format(CultureInfo.InvariantCulture, "ABS({0})", operands[0]);
				case FunctionOperatorType.Sqr:
					return string.Format(CultureInfo.InvariantCulture, "sqrt({0})", operands[0]);
				case FunctionOperatorType.Rnd:
					return "rand()";
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
				case FunctionOperatorType.Sinh:
					return string.Format(CultureInfo.InvariantCulture, "( (Exp({0}) - Exp(({0} * (-1) ))) / 2 )", operands[0]);
				case FunctionOperatorType.Cosh:
					return string.Format(CultureInfo.InvariantCulture, "( (Exp({0}) + Exp(({0} * (-1) ))) / 2 )", operands[0]);
				case FunctionOperatorType.Tanh:
					return string.Format(CultureInfo.InvariantCulture, "( (Exp({0}) - Exp(({0} * (-1) ))) / (Exp({0}) + Exp(({0} * (-1) ))) )", operands[0]);
				case FunctionOperatorType.Sin:
					return string.Format(CultureInfo.InvariantCulture, "Sin({0})", operands[0]);
				case FunctionOperatorType.Asin:
					return string.Format(CultureInfo.InvariantCulture, "Asin({0})", operands[0]);
				case FunctionOperatorType.Tan:
					return string.Format(CultureInfo.InvariantCulture, "Tan({0})", operands[0]);
				case FunctionOperatorType.Atn:
					return string.Format(CultureInfo.InvariantCulture, "Atan({0})", operands[0]);
				case FunctionOperatorType.Atn2:
					return string.Format(CultureInfo.InvariantCulture, "(case when {0} = 0 then (case when {1} >= 0 then 0 else Atan(1) * 4 end) else 2 * Atan({0} / (SQRT({1} * {1} + {0} * {0}) + {1})) end)", operands[0], operands[1]);
				case FunctionOperatorType.Cos:
					return string.Format(CultureInfo.InvariantCulture, "Cos({0})", operands[0]);
				case FunctionOperatorType.Acos:
					return string.Format(CultureInfo.InvariantCulture, "Acos({0})", operands[0]);
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
				case FunctionOperatorType.Max:
					return string.Format(CultureInfo.InvariantCulture, "(case when {0} > {1} then {0} else {1} end)", operands[0], operands[1]);
				case FunctionOperatorType.Min:
					return string.Format(CultureInfo.InvariantCulture, "(case when {0} < {1} then {0} else {1} end)", operands[0], operands[1]);
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
				case FunctionOperatorType.Ascii:
					return string.Format(CultureInfo.InvariantCulture, "Ascii({0})", operands[0]);
				case FunctionOperatorType.Char:
					return string.Format(CultureInfo.InvariantCulture, "Char({0})", operands[0]);
				case FunctionOperatorType.ToInt:
					return string.Format(CultureInfo.InvariantCulture, "Cast({0} as int)", operands[0]);
				case FunctionOperatorType.ToLong:
					return string.Format(CultureInfo.InvariantCulture, "Cast({0} as bigint)", operands[0]);
				case FunctionOperatorType.ToFloat:
					return string.Format(CultureInfo.InvariantCulture, "Cast({0} as float)", operands[0]);
				case FunctionOperatorType.ToDouble:
					return string.Format(CultureInfo.InvariantCulture, "Cast({0} as double)", operands[0]);
				case FunctionOperatorType.ToDecimal:
					return string.Format(CultureInfo.InvariantCulture, "Cast({0} as decimal)", operands[0]);
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, "Cast({0} as nvarchar)", operands[0]);
				case FunctionOperatorType.GetMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(ms, {0})", operands[0]);
				case FunctionOperatorType.GetSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(ss, {0})", operands[0]);
				case FunctionOperatorType.GetMinute:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(mi, {0})", operands[0]);
				case FunctionOperatorType.GetHour:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(hh, {0})", operands[0]);
				case FunctionOperatorType.GetDay:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(dd, {0})", operands[0]);
				case FunctionOperatorType.GetMonth:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(mm, {0})", operands[0]);
				case FunctionOperatorType.GetYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(yy, {0})", operands[0]);
				case FunctionOperatorType.GetTimeOfDay:
					return string.Format(CultureInfo.InvariantCulture, "((Hour({0}) * cast(36000000000 as BigInt)) + (Minute({0}) * cast(600000000 as BigInt)) + (Second({0}) * 10000000) +  (DATEPART(ms, {0}) * 10000))", operands[0]);
				case FunctionOperatorType.GetDayOfWeek:
					return string.Format(CultureInfo.InvariantCulture, "Mod((DatePart(cdw, {0}) - DatePart(cdw, '1900-01-01')  + 8) , 7)", operands[0]);
				case FunctionOperatorType.GetDayOfYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(dy, {0})", operands[0]);
				case FunctionOperatorType.GetDate:
					return string.Format(CultureInfo.InvariantCulture, "Date({0})", operands[0]);
				case FunctionOperatorType.AddTicks:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(ms, ({1} / 10000) % 86400000, DATEADD(dd, {1} / 864000000000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddMilliSeconds:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(ms, {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.AddTimeSpan:
				case FunctionOperatorType.AddSeconds:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(ms, (CONVERT(double,({1})) * 1000) % 86400000, DATEADD(dd, (CONVERT(double,({1})) * 1000) / 86400000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddMinutes:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(ms, (CONVERT(double,({1})) * 60000) % 86400000, DATEADD(dd, (CONVERT(double,({1})) * 60000) / 86400000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddHours:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(ms, (CONVERT(double,({1})) * 3600000) % 86400000, DATEADD(dd, (CONVERT(double,({1})) * 3600000) / 86400000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddDays:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(ms, (CONVERT(double,({1})) * 86400000) % 86400000, DATEADD(dd, (CONVERT(double,({1})) * 86400000) / 86400000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddMonths:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(mm, {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.AddYears:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(yy, {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(year, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMonth:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(month, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffDay:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(day, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffHour:
					return string.Format(CultureInfo.InvariantCulture, "((DATEDIFF(day, {0}, {1}) * 24) + DATEPART(hh, {1}) - DATEPART(hh, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMinute:
					return string.Format(CultureInfo.InvariantCulture, "((((DATEDIFF(day, {0}, {1}) * 24) + DATEPART(hh, {1}) - DATEPART(hh, {0})) * 60) + DATEPART(mi, {1}) - DATEPART(mi, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(second, DATEADD(ms, -DATEPART(ms, {0}), {0}), DATEADD(ms, -DATEPART(ms, {1}), {1}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(millisecond, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffTick:
					return string.Format(CultureInfo.InvariantCulture, "(DATEDIFF(millisecond, {0}, {1}) * 10000)", operands[0], operands[1]);
				case FunctionOperatorType.Now:
					return "NOW()";
				case FunctionOperatorType.UtcNow:
					return "Cast((current utc timestamp) as timestamp)";
				case FunctionOperatorType.Today:
					return "TODAY()";
				case FunctionOperatorType.Concat:
					return FnConcat(operands);
				case FunctionOperatorType.Len:
					return string.Format(CultureInfo.InvariantCulture, "Length({0})", operands[0]);
				case FunctionOperatorType.Replace:
					return string.Format(CultureInfo.InvariantCulture, "Replace({0}, {1}, {2})", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Reverse:
					return string.Format(CultureInfo.InvariantCulture, "Reverse({0})", operands[0]);
				case FunctionOperatorType.Insert:
					return string.Format(CultureInfo.InvariantCulture, "Stuff({0}, ({1})+1, 0, {2})", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Remove:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "Stuff({0}, {1}+1, Length({0}) - ({1}), '')", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "Stuff({0}, {1}+1, {2}, '')", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.Substring:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "SubString({0}, {1} + 1)", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "SubString({0}, {1} + 1,{2})", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.CharIndex:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(Charindex({0}, {1}) - 1)", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "(case when (Charindex({0},(Right({1},Length({1}) - {2})))) = 0 then (-1) else (Charindex({0},(Right({1},Length({1}) - {2}))) + {2} - 1) end)", operands[0], operands[1], operands[2]);
						case 4:
							return string.Format(CultureInfo.InvariantCulture, "(case when (CharIndex({0},(substring({1},{2} + 1,{3})))) = 0  then (-1) else (CharIndex({0},(substring({1},{2} + 1,{3}))) + {2} - 2) end)", operands[0], operands[1], operands[2], operands[3]);
					}
					goto default;
				case FunctionOperatorType.Contains:
					return string.Format(CultureInfo.InvariantCulture, "(CharIndex({1}, {0}) > 0)", operands[0], operands[1]);
				default:
					return base.FormatFunction(operatorType, operands);
			}
		}
		readonly static char[] achtungChars = new char[] { '_', '%', '[', ']' };
		string FnConcat(string[] operands) {
			string args = "(";
			foreach(string arg in operands) {
				if(args.Length > 1)
					args += " || ";
				args += arg;
			}
			return args + ")";
		}
		public override ICollection CollectTablesToCreate(ICollection tables) {
			Hashtable dbTables = new Hashtable();
			foreach(SelectStatementResultRow row in GetDataForTables(tables, null, "select TABLE_NAME, TABLE_TYPE from SYSTABLE where TABLE_NAME in ({0}) and TABLE_TYPE in ('BASE', 'VIEW')").Rows)
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
		protected override int GetSafeNameTableMaxLength() {
			return 128;
		}
		protected override bool NeedsIndexForForeignKey { get { return false; } }
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
		public override string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int skipSelectedRecords, int topSelectedRecords) {
			base.FormatSelect(selectedPropertiesSql, fromSql, whereSql, orderBySql, groupBySql, havingSql, skipSelectedRecords, topSelectedRecords);
			string modificatorsSql = string.Empty;
			if(skipSelectedRecords != 0) {
				int topSelectedRecordsValue = topSelectedRecords == 0 ? int.MaxValue - 1 - skipSelectedRecords : topSelectedRecords;
				modificatorsSql = string.Format(CultureInfo.InvariantCulture, "top {0} start at {1} ", topSelectedRecordsValue, skipSelectedRecords + 1);
			} else {
				modificatorsSql = string.Format(CultureInfo.InvariantCulture, (topSelectedRecords != 0) ? "top {0} " : string.Empty, topSelectedRecords);
			}
			string expandedWhereSql = whereSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}where {1}", Environment.NewLine, whereSql) : string.Empty;
			string expandedOrderBySql = orderBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}order by {1}", Environment.NewLine, orderBySql) : string.Empty;
			string expandedHavingSql = havingSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}having {1}", Environment.NewLine, havingSql) : string.Empty;
			string expandedGroupBySql = groupBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}group by {1}", Environment.NewLine, groupBySql) : string.Empty;
			return string.Format(CultureInfo.InvariantCulture, "select {0}{1} from {2}{3}{4}{5}{6}", modificatorsSql, selectedPropertiesSql, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedOrderBySql);
		}
		public override bool NativeSkipTakeSupported { get { return true; } }
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
				default:
					return base.FormatBinary(operatorType, leftOperand, rightOperand);
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
						return "N'" + ((string)value).Replace("'", "''") + "'";
				}
			}
			createParameter = true;
			return string.Format(CultureInfo.InvariantCulture, ":p{0}", index);
		}
		public override bool SupportNamedParameters { get { return true; } }
		public override bool NativeOuterApplySupported { get { return true; } }
		protected override IDataParameter CreateParameter(IDbCommand command, object value, string name) {
			IDataParameter param = CreateParameter(command);
			param.Value = value;
			param.ParameterName = (string.IsNullOrEmpty(name) || name[0] != ':') ? name : name.Remove(0, 1);
			if(value is byte[])
				param.DbType = DbType.Binary;
			if(value is string)
				param.DbType = DbType.String;
			return param;
		}
		public override string FormatConstraint(string constraintName) {
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", constraintName);
		}
		void ClearDatabase(IDbCommand command) {
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
			SelectStatementResult tables = SelectData(new Query(string.Format("select t.table_name from SYSTABLE t join SYSUSERPERM u on t.creator=u.user_id where (t.TABLE_TYPE = 'BASE' {0}) and u.user_name not in ('sys', 'dbo') and u.user_name NOT LIKE 'rs_sys%'", includeViews ? " or t.TABLE_TYPE = 'VIEW'" : string.Empty)));
			ArrayList result = new ArrayList(tables.Rows.Length);
			foreach(SelectStatementResultRow row in tables.Rows) {
				result.Add(row.Values[0]);
			}
			return (string[])result.ToArray(typeof(string));
		}
		class AsaSpInsertSqlGenerator : InsertSqlGenerator {
			public AsaSpInsertSqlGenerator(ISqlGeneratorFormatter formatter, TaggedParametersHolder identities, Dictionary<OperandValue, string> parameters) : base(formatter, identities, parameters) { }
			string identParamName;
			public Query GenerateSql(InsertStatement ins, string identParamName) {
				this.identParamName = identParamName;
				return GenerateSql(ins);
			}
			protected override string InternalGenerateSql() {
				InsertStatement ins = Root as InsertStatement;
				if(ReferenceEquals(ins, null)) { throw new InvalidOperationException("Root is not InsertStatement"); }
				StringBuilder result = new StringBuilder();
				result.Append("IF VAREXISTS('r') = 0 THEN ");
				result.Append("CREATE VARIABLE r INT;");
				result.Append("END IF;");
				string spName = formatter.ComposeSafeTableName(string.Format("sp_{0}_insert", ins.Table.Name));
				result.AppendFormat(CultureInfo.InvariantCulture, "CALL [{0}] (@{1}={2}", spName, ins.IdentityColumn, "r");
				for(int i = 0; i < Root.Operands.Count; i++) {
					string name = Process(Root.Operands[i]);
					string val = GetNextParameterName(((InsertStatement)Root).Parameters[i]);
					result.AppendFormat(CultureInfo.InvariantCulture, ", @{0} = {1}", ((QueryOperand)Root.Operands[i]).ColumnName, val);
				}
				result.Append(");SELECT r FROM DUMMY;");
				return result.ToString();
			}
		}
		protected override ModificationResult ProcessModifyData(params ModificationStatement[] dmlStatements) {
			BeginTransaction();
			try {
				List<ParameterValue> result = new List<ParameterValue>();
				TaggedParametersHolder identities = new TaggedParametersHolder();
				foreach(ModificationStatement root in dmlStatements) {
					InsertStatement ins = root as InsertStatement;
					if(ins != null) {
						if(!ReferenceEquals(ins.IdentityParameter, null) && (ins.IdentityColumn == IdentityColumnMagicName)) {
							identities.ConsolidateIdentity(ins.IdentityParameter);
							Query query = new AsaSpInsertSqlGenerator(this, identities, new Dictionary<OperandValue, string>()).GenerateSql(ins, IdentityColumnMagicName);
							ins.IdentityParameter.Value = GetScalar(query);
							if(!object.ReferenceEquals(null, ins.IdentityParameter)) {
								result.Add(ins.IdentityParameter);
							}
							continue;
						}
					}
					ParameterValue res = UpdateRecord(root, identities);
					if(!object.ReferenceEquals(null, res)) {
						result.Add(res);
					}
				}
				CommitTransaction();
				return new ModificationResult(result);
			} catch {
				RollbackTransaction();
				throw;
			}
		}
		bool hasIdentityes;
		public override string GenerateStoredProcedures(DBTable table, out string dropLines) {
			StringBuilder result = new StringBuilder();
			hasIdentityes = false;
			GenerateView(table, result);
			GenerateInsertSP(table, result);
			GenerateUpdateSP(table, result);
			GenerateDeleteSP(table, result);
			GenerateInsteadOfInsertTrigger(table, result);
			GenerateInsteadOfUpdateTrigger(table, result);
			GenerateInsteadOfDeleteTrigger(table, result);
			dropLines = string.Empty;
			return result.ToString();
		}
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
				string dbType = GetSqlCreateColumnType(table, table.Columns[i]);
				bool isFK = false;
				for(int j = 0; j < table.ForeignKeys.Count; j++) {
					if(table.ForeignKeys[j].Columns.Contains(table.Columns[i].Name)) {
						isFK = true;
						break;
					}
				}
				string formatStr;
				string name;
				if(table.Columns[i].IsIdentity) {
					name = IdentityColumnMagicName;
					formatStr = "\t@{0} {1}{3} OUT{2}";
				} else {
					name = table.Columns[i].Name;
					formatStr = "\t@{0} {1}{3}{2}";
				}
				string comma = i < table.Columns.Count - 1 ? "," : string.Empty;
				string defaultVal = isFK ? " = null" : string.Empty;
				StringBuilderAppendLine(result, string.Format(formatStr, name, dbType, comma, defaultVal));
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
			if(hasIdentityes) {
				StringBuilderAppendLine(result, string.Format("\tSET @{0} = @@Identity", IdentityColumnMagicName));
			}
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
				string dbType = GetSqlCreateColumnType(table, table.Columns[i]);
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
				string dbType = GetSqlCreateColumnType(table, table.Columns[i]);
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
			StringBuilderAppendLine(result, "INSTEAD OF INSERT");
			string viewName = ComposeSafeTableName(string.Format("{0}_xpoView", table.Name));
			StringBuilderAppendLine(result, string.Format("ON [{0}]", viewName));
			StringBuilderAppendLine(result, "REFERENCING NEW AS inserted");
			StringBuilderAppendLine(result, "FOR EACH ROW");
			StringBuilderAppendLine(result, "BEGIN");
			string spName = ComposeSafeTableName(string.Format("sp_{0}_xpoView_insert", table.Name));
			if(hasIdentityes) {
				StringBuilderAppendLine(result, string.Format("\tRAISERROR 25002 'Use {0} instead';", spName));
				StringBuilderAppendLine(result, "END");
				StringBuilderAppendLine(result, "GO");
				return;
			}
			StringBuilderAppendLine(result, string.Format("\tCALL [{0}](", spName));
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				result.Append(string.Format("\t\tinserted.[{0}]", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result, "\t);");
			StringBuilderAppendLine(result, "END");
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateInsteadOfUpdateTrigger(DBTable table, StringBuilder result) {
			string triggerName = ComposeSafeTableName(string.Format("t_{0}_xpoView_update", table.Name));
			StringBuilderAppendLine(result, string.Format("CREATE TRIGGER [{0}]", triggerName));
			StringBuilderAppendLine(result, "INSTEAD OF UPDATE");
			string viewName = ComposeSafeTableName(string.Format("{0}_xpoView", table.Name));
			StringBuilderAppendLine(result, string.Format("ON [{0}]", viewName));
			StringBuilderAppendLine(result, "REFERENCING");
			StringBuilderAppendLine(result, "\tNEW AS inserted");
			StringBuilderAppendLine(result, "\tOLD AS deleted");
			StringBuilderAppendLine(result, "FOR EACH ROW");
			StringBuilderAppendLine(result, "BEGIN");
			string spName = ComposeSafeTableName(string.Format("sp_{0}_xpoView_update", table.Name));
			StringBuilderAppendLine(result, string.Format("\tCALL [{0}](", spName));
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\tdeleted.[{0}]", columnName));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				StringBuilderAppendLine(result, ",");
				StringBuilderAppendLine(result, string.Format("\t\tdeleted.[{0}],", table.Columns[i].Name));
				result.Append(string.Format("\t\tinserted.[{0}]", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result, "\t);");
			StringBuilderAppendLine(result, "END");
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateInsteadOfDeleteTrigger(DBTable table, StringBuilder result) {
			string triggerName = ComposeSafeTableName(string.Format("t_{0}_xpoView_delete", table.Name));
			StringBuilderAppendLine(result, string.Format("CREATE TRIGGER [{0}]", triggerName));
			StringBuilderAppendLine(result, "INSTEAD OF DELETE");
			string viewName = ComposeSafeTableName(string.Format("{0}_xpoView", table.Name));
			StringBuilderAppendLine(result, string.Format("ON [{0}]", viewName));
			StringBuilderAppendLine(result, "REFERENCING OLD AS deleted");
			StringBuilderAppendLine(result, "FOR EACH ROW");
			StringBuilderAppendLine(result, "BEGIN");
			string spName = ComposeSafeTableName(string.Format("sp_{0}_xpoView_delete", table.Name));
			StringBuilderAppendLine(result, string.Format("\tCALL [{0}](", spName));
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\tdeleted.[{0}]", columnName));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				StringBuilderAppendLine(result, ",");
				result.Append(string.Format("\t\tdeleted.[{0}]", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result, "\t);");
			StringBuilderAppendLine(result, "END");
			StringBuilderAppendLine(result, "GO");
		}
		void AppendKeys(DBTable table, StringBuilder result) {
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				DBColumn keyColumn = GetDbColumnByName(table, table.PrimaryKey.Columns[i]);
				string dbType = GetSqlCreateColumnType(table, keyColumn);
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
				result.Append(string.Format("\t\t\t[{0}]", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result);
		}
		void InitTriggerCore(DBTable table, StringBuilder result) {
			StringBuilderAppendLine(result, "BEGIN");
			StringBuilderAppendLine(result, "\tDECLARE @cur CURSOR");
			StringBuilderAppendLine(result, "\tSET @cur = CURSOR FOR");
			StringBuilderAppendLine(result, "\t\tSELECT");
		}
		void FinalizeTrigger(DBTable table, StringBuilder result) {
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\t\tFETCH NEXT FROM @cur INTO");
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				result.Append(string.Format("\t\t\t@{0}", table.Columns[i].Name));
			}
			FinalizeTriggerCore(result);
		}
		void FinalizeTriggerCore(StringBuilder result) {
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\tEND");
			StringBuilderAppendLine(result, "\tCLOSE @cur");
			StringBuilderAppendLine(result, "\tDEALLOCATE @cur");
			StringBuilderAppendLine(result, "END");
			StringBuilderAppendLine(result, "GO");
		}
		public override DBStoredProcedure[] GetStoredProcedures() {
			List<DBStoredProcedure> result = new List<DBStoredProcedure>();
			string query = @"select
    p.procname,
    p.parmname,
    p.parmmode,
    isnull(p.user_type, p.parmdomain, p.user_type) as parmtype,
    p.length
from sys.sysprocparms p
where p.procname in(
    select distinct proc_name
    from sys.sysprocedure
    where creator = 1
)
order by p.procname, p.parm_id";
			IDbCommand cmd = CreateCommand(new Query(query));
			using(IDataReader rdr = cmd.ExecuteReader()) {
				DBStoredProcedure curProcedure = null;
				while(rdr.Read()) {
					string procName = (string)rdr[0];
					string parmName = (string)rdr[1];
					string parmMode = ((string)rdr[2]).ToUpper();
					string parmType = (string)rdr[3];
					int paramLength = Convert.ToInt32(rdr[4]);
					if(curProcedure == null || curProcedure.Name != procName) {
						if(curProcedure != null) {
							result.Add(curProcedure);
						}
						curProcedure = new DBStoredProcedure();
						curProcedure.Name = procName;
					}
					DBStoredProcedureArgument arg = new DBStoredProcedureArgument();
					arg.Name = parmName;
					arg.Type = GetDbTypeFromString(parmType, paramLength);
					switch(parmMode) {
						case "IN":
							arg.Direction = DBStoredProcedureArgumentDirection.In;
							break;
						case "OUT":
							arg.Direction = DBStoredProcedureArgumentDirection.Out;
							break;
						case "INOUT":
							arg.Direction = DBStoredProcedureArgumentDirection.InOut;
							break;
						default:
							throw new InvalidOperationException();
					}
					curProcedure.Arguments.Add(arg);
				}
				if(curProcedure != null) {
					result.Add(curProcedure);
				}
			}
			return result.ToArray();
		}
	}
	public class AsaProviderFactory : ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return AsaConnectionProvider.CreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return AsaConnectionProvider.CreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(ServerParamID) || !parameters.ContainsKey(DatabaseParamID) ||
				!parameters.ContainsKey(UserIDParamID) || !parameters.ContainsKey(PasswordParamID)) {
				return null;
			}
			return AsaConnectionProvider.GetConnectionString(parameters[ServerParamID], parameters[DatabaseParamID], parameters[UserIDParamID], parameters[PasswordParamID]);
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
		public override bool IsFilebased { get { return true; } }
		public override string ProviderKey { get { return AsaConnectionProvider.XpoProviderTypeString; } }
		public override string[] GetDatabases(string server, string userid, string password) {
			return new string[0];
		}
		public override string FileFilter { get { return null; } }
		public override bool MeanSchemaGeneration { get { return false; } }
		public override bool SupportStoredProcedures { get { return true; } }
	}
}
