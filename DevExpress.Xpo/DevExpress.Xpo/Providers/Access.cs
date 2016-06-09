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
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Helpers;
using System.Resources;
namespace DevExpress.Xpo.DB {
	using System.Data;
	using System.Data.OleDb;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Text.RegularExpressions;
	using System.Globalization;
	using DevExpress.Xpo.DB;
	using DevExpress.Xpo.DB.Helpers;
	using DevExpress.Data.Filtering;
	using DevExpress.Xpo.DB.Exceptions;
	using System.Collections.Generic;
	using System.Text;
	using System.Reflection;
	using DevExpress.Data.Db;
	using DevExpress.Xpo.Helpers;
	public abstract class OleDBConnectionProvider : ConnectionProviderSql {
		public static bool SortColumnsAlphabetically = true;
		public OleDBConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption) : base(connection, autoCreateOption) { }
		protected OleDbConnection OleDBConnection { get { return (OleDbConnection)Connection; } }
		public override ICollection CollectTablesToCreate(ICollection tables) {
			ArrayList list = new ArrayList();
			foreach(DBTable table in tables) {
				DataTable data = OleDBConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, ComposeSafeTableName(table.Name), null });
				DataTable proc = OleDBConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Procedures, new object[] { null, null, ComposeSafeTableName(table.Name), null });
				if (data.Rows.Count == 0 && proc.Rows.Count == 0) {
					list.Add(table);
				} else if (data.Rows.Count != 0)
					table.IsView = (string)data.Rows[0]["TABLE_TYPE"] == "VIEW";
				else
					table.IsView = !ProcedureContainsParameters((string)proc.Rows[0]["PROCEDURE_DEFINITION"]);				 
			}
			return list;
		}
		protected bool ProcedureContainsParameters(string sourceString) {
			return sourceString.StartsWith("PARA", StringComparison.InvariantCultureIgnoreCase) ||
				Regex.Match(sourceString, "Forms]?!", RegexOptions.IgnoreCase).Success;			
		}
		DBColumnType GetType(OleDbType type) {
			switch(type) {
				case OleDbType.Integer:
					return DBColumnType.Int32;
				case OleDbType.VarBinary:
				case OleDbType.Binary:
					return DBColumnType.ByteArray;
				case OleDbType.VarWChar:
				case OleDbType.LongVarWChar:
				case OleDbType.WChar:
				case OleDbType.Char:
					return DBColumnType.String;
				case OleDbType.Boolean:
					return DBColumnType.Boolean;
				case OleDbType.SmallInt:
				case OleDbType.UnsignedTinyInt:
					return DBColumnType.Int16;
				case OleDbType.Decimal:
				case OleDbType.Currency:
					return DBColumnType.Decimal;
				case OleDbType.Single:
					return DBColumnType.Single;
				case OleDbType.Double:
					return DBColumnType.Double;
				case OleDbType.Date:
					return DBColumnType.DateTime;
				case OleDbType.Guid:
					return DBColumnType.Guid;
				case OleDbType.Numeric:
					return DBColumnType.Int64;
			}
			return DBColumnType.Unknown;
		}
		void GetColumns(DBTable table) {
			DataTable t = OleDBConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, ComposeSafeTableName(table.Name), null });
			if (SortColumnsAlphabetically) {
				foreach (DataRow r in t.Rows) {
					DBColumnType type = GetType((OleDbType)r["DATA_TYPE"]);
					table.AddColumn(new DBColumn((string)r["COLUMN_NAME"], false, String.Empty, type == DBColumnType.String ? (int)(long)r["character_maximum_length"] : 0, type));
				}
			} else {
				SortedList cols = new SortedList();
				foreach (DataRow r in t.Rows) {
					DBColumnType type = GetType((OleDbType)r["DATA_TYPE"]);
					cols.Add(r["ORDINAL_POSITION"], new DBColumn((string)r["COLUMN_NAME"], false, String.Empty, type == DBColumnType.String ? (int)(long)r["character_maximum_length"] : 0, type));
				}
				foreach (DictionaryEntry de in cols) {
					table.AddColumn(de.Value as DBColumn);
				}
			}
		}
		void GetPrimaryKey(DBTable table) {
			DataTable data = OleDBConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, new object[] { null, null, ComposeSafeTableName(table.Name) });
			if(data.Rows.Count > 0) {
				ArrayList cols = new ArrayList();
				foreach(DataRow row in data.Rows) {
					DBColumn column = table.GetColumn((string)row["COLUMN_NAME"]);
					column.IsKey = true;
					cols.Add(column);
				}
				table.PrimaryKey = new DBPrimaryKey(cols);
			}
			if(table.PrimaryKey != null && table.PrimaryKey.Columns.Count == 1) {
				using(OleDbCommand cmd = (OleDbCommand)CreateCommand()) {
					cmd.CommandText = "select [" + table.PrimaryKey.Columns[0] + "] from [" + ComposeSafeTableName(table.Name) + "]";
					using(OleDbDataReader r = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo)) {
						DataTable t = r.GetSchemaTable();
						table.GetColumn(table.PrimaryKey.Columns[0]).IsIdentity = (bool)t.Rows[0]["IsAutoIncrement"] == true;
					}
				}
			}
		}
		void GetIndexes(DBTable table) {
			DataTable dt = OleDBConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Indexes, new object[] { null, null, null, null, ComposeSafeTableName(table.Name) });
			DBIndex index = null;
			foreach(DataRow row in dt.Rows) {
				if(index == null || index.Name != (string)row["INDEX_NAME"]) {
					StringCollection list = new StringCollection();
					list.Add((string)row["COLUMN_NAME"]);
					index = new DBIndex((string)row["INDEX_NAME"], list, (bool)row["UNIQUE"]);
					table.Indexes.Add(index);
				} else
					index.Columns.Add((string)row["COLUMN_NAME"]);
			}
		}
		void GetForeignKeys(DBTable table) {
			DataTable dt = OleDBConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, new object[] { null, null, null, null, null, ComposeSafeTableName(table.Name) });
			Hashtable fks = new Hashtable();
			foreach(DataRow row in dt.Rows) {
				DBForeignKey fk = (DBForeignKey)fks[row["FK_NAME"]];
				int ord = (int)(Int64)row["ORDINAL"];
				if(fk == null) {
					StringCollection pkc = new StringCollection();
					StringCollection fkc = new StringCollection();
					pkc.Add((string)row["FK_COLUMN_NAME"]);
					fkc.Add((string)row["PK_COLUMN_NAME"]);
					fk = new DBForeignKey(pkc, (string)row["PK_TABLE_NAME"], fkc);
					table.ForeignKeys.Add(fk);
					fks[row["FK_NAME"]] = fk;
				} else {
					fk.Columns.Add((string)row["FK_COLUMN_NAME"]);
					fk.PrimaryKeyTableKeyColumns.Add((string)row["PK_COLUMN_NAME"]);
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
	}
	public class Access97ProviderFactory : ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return AccessConnectionProvider.CreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return AccessConnectionProvider.CreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(DatabaseParamID) || !parameters.ContainsKey(UserIDParamID) || !parameters.ContainsKey(PasswordParamID)) {
				return null;
			}
			return AccessConnectionProvider.GetConnectionString(parameters[DatabaseParamID], parameters[UserIDParamID], parameters[PasswordParamID]);
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
		public override bool HasMultipleDatabases { get { return false; } }
		public override bool IsServerbased { get { return false; } }
		public override bool IsFilebased { get { return true; } }
		public override string ProviderKey { get { return "Access97"; } }
		public override string[] GetDatabases(string server, string userid, string password) {
			return new string[1] { server };
		}
		public override string FileFilter { get { return "Access 97 databases|*.mdb"; } }
		public override bool MeanSchemaGeneration { get { return true; } }
	}
	public class Access2007ProviderFactory : Access97ProviderFactory {
		public override IDataStore CreateProvider(Dictionary<string, string> parameters, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			if(!parameters.ContainsKey(DatabaseParamID) || !parameters.ContainsKey(PasswordParamID)) {
				objectsToDisposeOnDisconnect = new IDisposable[0];
				return null;
			}
			string connectionString = AccessConnectionProvider.GetConnectionStringACE(parameters[DatabaseParamID],
						 parameters[PasswordParamID]);
			ConnectionStringParser helper = new ConnectionStringParser(connectionString);
			helper.RemovePartByName(DataStoreBase.XpoProviderTypeParameterName);
			return CreateProviderFromString(helper.GetConnectionString(), autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if (!parameters.ContainsKey(DatabaseParamID) || !parameters.ContainsKey(PasswordParamID)) {
				return null;
			}
			return AccessConnectionProvider.GetConnectionStringACE(parameters[DatabaseParamID], parameters[PasswordParamID]);
		}
		public override bool HasUserName { get { return false; } }
		public override string ProviderKey { get { return "Access2007"; } }
		public override string FileFilter { get { return "Access 2007 databases|*.accdb"; } }
	}
	public class AccessConnectionProvider : OleDBConnectionProvider, ISqlGeneratorFormatterEx {
		public const string XpoProviderTypeString = "MSAccess";
		public static string GetConnectionString(string database, string userid, string password) {
			return String.Format("{3}={4};Provider=Microsoft.Jet.OLEDB.4.0;Mode=Share Deny None;data source={0};user id={1};{5}password={2};", database, userid, password, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString, string.IsNullOrEmpty(password) ? string.Empty : "Jet OLEDB:Database ");
		}
		public static string GetConnectionStringACE(string database, string password) {
			return String.Format("{2}={3};Provider=Microsoft.ACE.OLEDB.12.0;Mode=Share Deny None;data source={0};Jet OLEDB:Database Password={1};", database, password, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString);
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = new OleDbConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] { connection };
			return CreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			if(((System.Data.OleDb.OleDbConnection)connection).Provider.StartsWith("Microsoft.Jet.OLEDB", StringComparison.InvariantCultureIgnoreCase)
				|| ((System.Data.OleDb.OleDbConnection)connection).Provider.StartsWith("Microsoft.ACE.OLEDB", StringComparison.InvariantCultureIgnoreCase))
				return new AccessConnectionProvider(connection, autoCreateOption);
			else
				return null;
		}
		static AccessConnectionProvider() {
			RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
			RegisterDataStoreProvider("System.Data.OleDb.OleDbConnection", new DataStoreCreationFromConnectionDelegate(CreateProviderFromConnection));
			RegisterFactory(new Access97ProviderFactory());
			RegisterFactory(new Access2007ProviderFactory());
		}
		public static void Register() { }
		public AccessConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		protected override string GetSqlCreateColumnTypeForBoolean(DBTable table, DBColumn column) {
			return "bit";
		}
		protected override string GetSqlCreateColumnTypeForByte(DBTable table, DBColumn column) {
			return "byte";
		}
		protected override string GetSqlCreateColumnTypeForSByte(DBTable table, DBColumn column) {
			return "short";
		}
		protected override string GetSqlCreateColumnTypeForChar(DBTable table, DBColumn column) {
			return "char(1)";
		}
		protected override string GetSqlCreateColumnTypeForDecimal(DBTable table, DBColumn column) {
			return "currency";
		}
		protected override string GetSqlCreateColumnTypeForDouble(DBTable table, DBColumn column) {
			return "double";
		}
		protected override string GetSqlCreateColumnTypeForSingle(DBTable table, DBColumn column) {
			return "single";
		}
		protected override string GetSqlCreateColumnTypeForInt32(DBTable table, DBColumn column) {
			return "int";
		}
		protected override string GetSqlCreateColumnTypeForUInt32(DBTable table, DBColumn column) {
			return "decimal(10,0)";
		}
		protected override string GetSqlCreateColumnTypeForInt16(DBTable table, DBColumn column) {
			return "short";
		}
		protected override string GetSqlCreateColumnTypeForUInt16(DBTable table, DBColumn column) {
			return "int";
		}
		protected override string GetSqlCreateColumnTypeForInt64(DBTable table, DBColumn column) {
			return "decimal(20,0)";
		}
		protected override string GetSqlCreateColumnTypeForUInt64(DBTable table, DBColumn column) {
			return "decimal(20,0)";
		}
		public const int MaximumStringSize = 255;
		protected override string GetSqlCreateColumnTypeForString(DBTable table, DBColumn column) {
			if(column.Size > 0 && column.Size <= MaximumStringSize)
				return "varchar(" + column.Size.ToString(CultureInfo.InvariantCulture) + ')';
			else
				return "LONGTEXT";
		}
		protected override string GetSqlCreateColumnTypeForDateTime(DBTable table, DBColumn column) {
			return "datetime";
		}
		protected override string GetSqlCreateColumnTypeForGuid(DBTable table, DBColumn column) {
			return "guid";
		}
		protected override string GetSqlCreateColumnTypeForByteArray(DBTable table, DBColumn column) {
			return "longbinary";
		}
		public override string GetSqlCreateColumnFullAttributes(DBTable table, DBColumn column) {
			string result = GetSqlCreateColumnType(table, column);
			if(column.ColumnType == DBColumnType.Boolean)
				return result;
			if(column.IsKey)
				result += " NOT NULL";
			else
				result += " NULL";
			if(column.IsKey && column.IsIdentity && (column.ColumnType == DBColumnType.Int32 || column.ColumnType == DBColumnType.Int64) && IsSingleColumnPKColumn(table, column)) {
				if (column.ColumnType == DBColumnType.Int64) throw new NotSupportedException(Res.GetString(Res.ConnectionProvider_TheAutoIncrementedKeyWithX0TypeIsNotSuppor, column.ColumnType, this.GetType()));
				result += " IDENTITY";
			}
			return result;
		}
		protected override object ConvertToDbParameter(object clientValue, TypeCode clientValueTypeCode) {
			switch(clientValueTypeCode) {
				case TypeCode.DateTime:
					return ((DateTime)clientValue).ToOADate();
				case TypeCode.Decimal:
					return (Double)(Decimal)clientValue;
			}
			return base.ConvertToDbParameter(clientValue, clientValueTypeCode);
		}
		protected override Int64 GetIdentity(Query sql) {
			ExecSql(sql);
			return (Int32)GetScalar(new Query("select @@Identity"));
		}
		protected override Exception WrapException(Exception e, IDbCommand query) {
			const int noValueGivenColumnAbsentInSelect = -2147217904;
			const int unknownFieldName = -2147217900;
			const int couldNotFindOutputTable = -2147217865;
			const int eFail = -2147467259;
			OleDbException oleDbException = e as OleDbException;
			if(oleDbException != null) {
				switch(oleDbException.ErrorCode) {
					case noValueGivenColumnAbsentInSelect:
					case unknownFieldName:
					case couldNotFindOutputTable:
						return new SchemaCorrectionNeededException(oleDbException);
					case eFail:
						if(oleDbException.Errors.Count > 0 && oleDbException.Errors[0].NativeError == -534971980 || oleDbException.Errors[0].NativeError == -105121349)
							return new ConstraintViolationException(query.CommandText, GetParametersString(query), e);
						break;
				}
			}
			return base.WrapException(e, query);
		}
		bool ExtractMajorPartOfNativeErrorCode(OleDbException e, out int errorCode) {			
			if(e.Errors.Count == 0) {
				errorCode = 0;
				return false;
			}
			int n = e.Errors[0].NativeError;
			errorCode = (System.Math.Abs(n) & 0xFFFF) * (n < 0 ? -1 : 1);
			return true;
		}
		protected override IDbConnection CreateConnection() {
			return new OleDbConnection(ConnectionString);
		}
		[System.Security.SecuritySafeCritical]
		protected override void CreateDataBase() {
			if(Connection.State == ConnectionState.Open)
				return;
			const int FileNotFoundErrorCode = -1811;
			try {
				Connection.Open();
			} catch(Exception e) {
				int errorCode;
				if(e is OleDbException
					&& (ExtractMajorPartOfNativeErrorCode((OleDbException)e, out errorCode) && errorCode == FileNotFoundErrorCode)
					&& CanCreateDatabase) {
					Type objClassType = Type.GetTypeFromProgID("ADOX.Catalog");
					object cat = Activator.CreateInstance(objClassType);
					cat.GetType().InvokeMember("Create", BindingFlags.InvokeMethod, null, cat, new object[] { ConnectionString });
					object con = cat.GetType().InvokeMember("ActiveConnection", BindingFlags.GetProperty, null, cat, null);
					if(con != null)
						con.GetType().InvokeMember("Close", BindingFlags.InvokeMethod, null, con, null);
				} else
					throw new UnableToOpenDatabaseException(XpoDefault.ConnectionStringRemovePassword(ConnectionString), e);
			}
		}
		protected override int GetSafeNameTableMaxLength() {
			return 64;
		}
		protected override string GetSafeNameRoot(string originalName) {
			return GetSafeNameAccess(originalName);
		}
		public override void CreateForeignKey(DBTable table, DBForeignKey fk) {
			if(fk.PrimaryKeyTable == "XPObjectType")
				return;
			base.CreateForeignKey(table, fk);
		}
		public override string FormatTable(string schema, string tableName) {
			return AccessFormatterHelper.FormatTable(schema, tableName);
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			return AccessFormatterHelper.FormatTable(schema, tableName, tableAlias);
		}
		public override string FormatColumn(string columnName) {
			return AccessFormatterHelper.FormatColumn(columnName);
		}
		public override string FormatColumn(string columnName, string tableAlias) {
			return AccessFormatterHelper.FormatColumn(columnName, tableAlias);
		}
		public override string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int topSelectedRecords) {
			string modificatorsSql = string.Format(CultureInfo.InvariantCulture, (topSelectedRecords != 0) ? "top {0} " : string.Empty, topSelectedRecords);
			string expandedWhereSql = whereSql == null ? null : string.Format(CultureInfo.InvariantCulture, "\nwhere {0}", whereSql);
			string expandedGroupBySql = groupBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}group by {1}", Environment.NewLine, groupBySql) : string.Empty;
			string expandedHavingSql = havingSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}having {1}", Environment.NewLine, havingSql) : string.Empty;
			string expandedOrderBySql = orderBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}order by {1}", Environment.NewLine, orderBySql) : string.Empty;
			return string.Format(CultureInfo.InvariantCulture, "select {0}{1} from {2}{3}{4}{5}{6}", 
				modificatorsSql, selectedPropertiesSql, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedOrderBySql);
		}
		public override string FormatInsertDefaultValues(string tableName) {
			return AccessFormatterHelper.FormatInsertDefaultValues(tableName);
		}
		public override string FormatInsert(string tableName, string fields, string values) {
			return AccessFormatterHelper.FormatInsert(tableName, fields, values);
		}
		public override string FormatUpdate(string tableName, string sets, string whereClause) {
			return AccessFormatterHelper.FormatUpdate(tableName, sets, whereClause);
		}
		public override string FormatDelete(string tableName, string whereClause) {
			return AccessFormatterHelper.FormatDelete(tableName, whereClause);
		}
		public override string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			return AccessFormatterHelper.FormatBinary(operatorType, leftOperand, rightOperand);
		}
		public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			string format = AccessFormatterHelper.FormatFunction(operatorType, operands);
			return format == null ? base.FormatFunction(operatorType, operands) : format;
		}		
		public override string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			string format = AccessFormatterHelper.FormatFunction(processParameter, operatorType, operands);
			return format == null ? base.FormatFunction(processParameter, operatorType, operands) : format;
		}
		string GetParameterType(object value) {
			DBColumn c = new DBColumn();
			c.ColumnType = DBColumn.GetColumnType(value.GetType());
			return GetSqlCreateColumnType(null, c);
		}
		protected override IDbCommand CreateCommand(Query query) {
			StringBuilder res = new StringBuilder();
			for(int i = 0; i < query.Parameters.Count; i++) {
				if(res.Length > 0)
					res.Append(',');
				res.Append(query.ParametersNames[i]);
				res.Append(' ');
				res.Append(GetParameterType(query.Parameters[i].Value));
			}
			if(res.Length > 0)
				query = new Query("parameters " + res + ";" + query.Sql, query.Parameters, query.ParametersNames, query.SkipSelectedRecords, query.TopSelectedRecords);
			return base.CreateCommand(query);
		}
		static string[] parameterNameCache = new string[0];
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			int len = parameterNameCache.Length;
			if(len <= index) {
				string[] newCache = new string[len + 10];
				Array.Copy(parameterNameCache, newCache, len);
				for(int i = len; i < newCache.Length; i++)
					newCache[i] = string.Concat("[@p", i.ToString(CultureInfo.InvariantCulture), "]");
				parameterNameCache = newCache;
			}
			return parameterNameCache[index];
		}
		public override string FormatConstraint(string constraintName) {
			return AccessFormatterHelper.FormatConstraint(constraintName);
		}
		public static string GetConnectionString(string database) {
			return GetConnectionString(database, "Admin", String.Empty);
		}
		protected override void ProcessClearDatabase() {
			using(IDbCommand command = CreateCommand()) {
				DataTable fks = OleDBConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, new object[] { null, null, null, null, null, null });
				foreach(DataRow row in fks.Rows) {
					if(Convert.ToInt32(row["ORDINAL"]) == 1) {
						command.CommandText = "alter table [" + row["FK_TABLE_NAME"] + "] drop constraint [" + row["FK_NAME"] + "]";
						command.ExecuteNonQuery();
					}
				}
				DataTable tables = OleDBConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null,  null, "TABLE" });
				foreach(DataRow row in tables.Rows) {
					command.CommandText = "drop table [" + row["TABLE_NAME"] + "]";
					command.ExecuteNonQuery();
				}
			}
		}
		public override string[] GetStorageTablesList(bool includeViews) {
			List<string> result = new List<string>();
			DataTable tables = OleDBConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
			foreach(DataRow row in tables.Rows) {
				result.Add((string)row["TABLE_NAME"]);
			}
			if (includeViews) {
				DataTable views = OleDBConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "VIEW" });
				DataTable procedures = OleDBConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Procedures, new object[] { });
				foreach (DataRow row in views.Rows) {
					result.Add((string)row["TABLE_NAME"]);
				}
				foreach (DataRow row in procedures.Rows) {
					if (!ProcedureContainsParameters((string)row["PROCEDURE_DEFINITION"])) result.Add((string)row["PROCEDURE_NAME"]);
				}
			}			
			return result.ToArray();
		}
		protected override bool NeedsIndexForForeignKey { get { return false; } }
		protected override SelectedData ExecuteSproc(string sprocName, params OperandValue[] parameters) {
			using(IDbCommand command = CreateCommand()) {
				command.CommandType = CommandType.StoredProcedure;
				command.CommandText = sprocName;
				int counter = 0;
				foreach(OperandValue param in parameters) {
					bool createParam = true;
					string paramName = GetParameterName(param, counter++, ref createParam);
					if(createParam) {
						command.Parameters.Add(CreateParameter(command, param.Value, paramName));
					}
				}
				List<SelectStatementResult> selectStatmentResults = GetSelectedStatmentResults(command);
				return new SelectedData(selectStatmentResults.ToArray());
			}
		}
		public override DBStoredProcedure[] GetStoredProcedures() {			  
			throw new NotSupportedException();
		}		
		protected override void CommandBuilderDeriveParameters(IDbCommand command) {			
			throw new NotSupportedException();
		}
	}
	public class AccessConnectionProviderMultiUserThreadSafe : MarshalByRefObject, IDataStore, IDataStoreSchemaExplorer, IDataStoreForTests, ICommandChannel {
		public const string XpoProviderTypeString = "MSAccessSafe";
		public static string GetConnectionString(string database, string userid, string password) {
			return String.Format("{3}={4};Provider=Microsoft.Jet.OLEDB.4.0;Mode=Share Deny None;data source={0};user id={1};{5}password={2};", database, userid, password, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString, string.IsNullOrEmpty(password) ? string.Empty : "Jet OLEDB:Database ");
		}
		public static string GetConnectionString(string database) {
			return GetConnectionString(database, "Admin", String.Empty);
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			objectsToDisposeOnDisconnect = new IDisposable[0];
			return new AccessConnectionProviderMultiUserThreadSafe(connectionString, autoCreateOption);
		}
		static AccessConnectionProviderMultiUserThreadSafe() {
			DataStoreBase.RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
		}
		public static void Register() { }
		public readonly string ConnectionString;
		readonly AutoCreateOption _AutoCreateOption;
		protected IDbConnection GetConnection() {
			return new OleDbConnection(ConnectionString);
		}
		public AccessConnectionProviderMultiUserThreadSafe(string connectionString, AutoCreateOption autoCreateOption) {
			this.ConnectionString = connectionString;
			this._AutoCreateOption = autoCreateOption;
		}
		public AutoCreateOption AutoCreateOption {
			get { return this._AutoCreateOption; }
		}
		public SelectedData SelectData(params SelectStatement[] selects) {
			using(IDbConnection connection = GetConnection()) {
				return AccessConnectionProvider.CreateProviderFromConnection(connection, AutoCreateOption).SelectData(selects);
			}
		}
		public ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
			using(IDbConnection connection = GetConnection()) {
				return AccessConnectionProvider.CreateProviderFromConnection(connection, AutoCreateOption).ModifyData(dmlStatements);
			}
		}
		public UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
			using(IDbConnection connection = GetConnection()) {
				return AccessConnectionProvider.CreateProviderFromConnection(connection, AutoCreateOption).UpdateSchema(dontCreateIfFirstTableNotExist, tables);
			}
		}
		public DBTable[] GetStorageTables(params string[] tables) {
			using(IDbConnection connection = GetConnection()) {
				return ((IDataStoreSchemaExplorer)AccessConnectionProvider.CreateProviderFromConnection(connection, AutoCreateOption)).GetStorageTables(tables);
			}
		}
		public string[] GetStorageTablesList(bool includeViews) {
			using(IDbConnection connection = GetConnection()) {
				return ((IDataStoreSchemaExplorer)AccessConnectionProvider.CreateProviderFromConnection(connection, AutoCreateOption)).GetStorageTablesList(includeViews);
			}
		}
		void IDataStoreForTests.ClearDatabase() {
			using(IDbConnection connection = GetConnection()) {
				((IDataStoreForTests)AccessConnectionProvider.CreateProviderFromConnection(connection, AutoCreateOption)).ClearDatabase();
			}
		}
		object ICommandChannel.Do(string command, object args) {
			using(IDbConnection connection = GetConnection()) {
				IDataStore provider = AccessConnectionProvider.CreateProviderFromConnection(connection, AutoCreateOption);
				ICommandChannel signalReceptor = provider as ICommandChannel;
				switch(command) {
					case CommandChannelHelper.Command_ExecuteNonQuerySQL:
					case CommandChannelHelper.Command_ExecuteQuerySQL:
					case CommandChannelHelper.Command_ExecuteScalarSQL:
					case CommandChannelHelper.Command_ExecuteStoredProcedure:
						break;
					default:
						throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupportedEx, command, typeof(AccessConnectionProviderMultiUserThreadSafe).FullName));
				}
				if(signalReceptor == null) {
					if(provider == null) {
						throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupported, command));
					} else {
						throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupportedEx, command, provider.GetType().FullName));
					}
				}
				return signalReceptor.Do(command, args);
			}
		}
	}
}
