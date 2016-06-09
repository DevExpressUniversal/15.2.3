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
	public class AdvantageConnectionProvider : ConnectionProviderSql {
		bool isADS10;
		ReflectConnectionHelper helper;
		ReflectConnectionHelper ConnectionHelper {
			get {
				if(helper == null)
					helper = new ReflectConnectionHelper(Connection, "Advantage.Data.Provider.AdsException");
				return helper;
			}
		}
		public const string XpoProviderTypeString = "Advantage";
		public static string GetConnectionString(string database) {
			return String.Format("{1}={2};Data source={0};servertype=local;user id=ADSSYS;TrimTrailingSpaces=true", database, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString);
		}
		public static string GetConnectionString(string database, string password) {
			return String.Format("{1}={2};Data source={0};servertype=local;user id=ADSSYS;Password={3};TrimTrailingSpaces=true", database, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString, password);
		}
		public static string GetConnectionString(string database, string username, string password) {
			return String.Format("{1}={2};Data source={0};servertype=local;user id={3};Password={4};TrimTrailingSpaces=true", database, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString, username, password);
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] { connection };
			return CreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new AdvantageConnectionProvider(connection, autoCreateOption);
		}
		static AdvantageConnectionProvider() {
			RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
			RegisterDataStoreProvider("Advantage.Data.Provider.AdsConnection", new DataStoreCreationFromConnectionDelegate(CreateProviderFromConnection));
			RegisterFactory(new AdvantageProviderFactory());
		}
		public static void Register() { }
		public AdvantageConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
			CheckVersion(connection);
		}
		private void CheckVersion(IDbConnection connection) {
			using(IDbCommand command = connection.CreateCommand()) {
				command.CommandText = @"execute procedure sp_mgGetInstallInfo()";
				IDataReader reader = command.ExecuteReader();
				if(reader.Read()) {
					string versionString = null;
					for(int i = 0; i < reader.FieldCount; i++) {
						if(reader.GetName(i) == "Version") {
							versionString = reader.GetString(i);
							break;
						}
					}
					if(!string.IsNullOrEmpty(versionString)) {
						string[] parts = versionString.Split('.');
						int version;
						if(parts.Length > 0 && Int32.TryParse(parts[0], out version)) {
							isADS10 = version >= 10;
						}
					}
				}
			}
		}
		protected override string GetSqlCreateColumnTypeForBoolean(DBTable table, DBColumn column) {
			return "logical";
		}
		protected override string GetSqlCreateColumnTypeForByte(DBTable table, DBColumn column) {
			return "short";
		}
		protected override string GetSqlCreateColumnTypeForSByte(DBTable table, DBColumn column) {
			return "short";
		}
		protected override string GetSqlCreateColumnTypeForChar(DBTable table, DBColumn column) {
			return isADS10 ? "nchar(1)" : "char(1)";
		}
		protected override string GetSqlCreateColumnTypeForDecimal(DBTable table, DBColumn column) {
			return "money";
		}
		protected override string GetSqlCreateColumnTypeForDouble(DBTable table, DBColumn column) {
			return "double";
		}
		protected override string GetSqlCreateColumnTypeForSingle(DBTable table, DBColumn column) {
			return "double";
		}
		protected override string GetSqlCreateColumnTypeForInt32(DBTable table, DBColumn column) {
			return "integer";
		}
		protected override string GetSqlCreateColumnTypeForUInt32(DBTable table, DBColumn column) {
			return "money";
		}
		protected override string GetSqlCreateColumnTypeForInt16(DBTable table, DBColumn column) {
			return "short";
		}
		protected override string GetSqlCreateColumnTypeForUInt16(DBTable table, DBColumn column) {
			return "integer";
		}
		protected override string GetSqlCreateColumnTypeForInt64(DBTable table, DBColumn column) {
			return "numeric(20,0)";
		}
		protected override string GetSqlCreateColumnTypeForUInt64(DBTable table, DBColumn column) {
			return "numeric(21,0)";
		}
		public const int MaximumStringSize = 4000;
		protected override string GetSqlCreateColumnTypeForString(DBTable table, DBColumn column) {
			if(column.Size > 0 && column.Size <= MaximumStringSize)
				return (isADS10 ? "nchar(" : "char(") + column.Size.ToString(CultureInfo.InvariantCulture) + ')';
			else
				return isADS10 ? "nmemo" : "memo";
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
			if(column.IsKey && column.IsIdentity && (column.ColumnType == DBColumnType.Int32 || column.ColumnType == DBColumnType.Int64) && IsSingleColumnPKColumn(table, column)) {
				if(column.ColumnType == DBColumnType.Int64)
					throw new NotSupportedException(Res.GetString(Res.ConnectionProvider_TheAutoIncrementedKeyWithX0TypeIsNotSuppor, column.ColumnType, this.GetType()));
				result = "AutoInc";
			}
			return result;
		}
		protected override object ReformatReadValue(object value, ConnectionProviderSql.ReformatReadValueArgs args) {
			if(args.DbTypeCode == TypeCode.Decimal) {
				switch(args.TargetTypeCode) {
					case TypeCode.UInt32:
					case TypeCode.UInt64:
					case TypeCode.Int64:
						return Convert.ChangeType(((decimal)value), args.TargetTypeCode, CultureInfo.InvariantCulture);
					case TypeCode.Decimal:
						return ((decimal)value);
				}
			}
			return base.ReformatReadValue(value, args);
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
					return (Decimal)(UInt32)clientValue;
				case TypeCode.UInt64:
					return (Decimal)(UInt64)clientValue;
				case TypeCode.Int64:
					return (Decimal)(Int64)clientValue;
				case TypeCode.Single:
					return (double)(Single)clientValue;
				case TypeCode.Decimal:
					return ((Decimal)clientValue);
			}
			return base.ConvertToDbParameter(clientValue, clientValueTypeCode);
		}
		GetPropertyValueDelegate getAdsCommandLastAutoinc;
		GetPropertyValueDelegate GetAdsCommandLastAutoinc {
			get {
				if(getAdsCommandLastAutoinc == null)
					InitDelegates();
				return getAdsCommandLastAutoinc;
			}
		}
		protected override Int64 GetIdentity(Query sql) {
			using(IDbCommand cmd = CreateCommand(sql)) {
				try {
					cmd.ExecuteNonQuery();
					return (int)GetAdsCommandLastAutoinc(cmd);
				} catch(Exception e) {
					throw WrapException(e, cmd);
				}
			}
		}
		void InitDelegates() {
			Type adsCommandType = ConnectionHelper.GetType("Advantage.Data.Provider.AdsCommand");
			getAdsCommandLastAutoinc = ReflectConnectionHelper.CreateGetPropertyDelegate(adsCommandType, "LastAutoinc");
		}
		protected override IDbConnection CreateConnection() {
			return ConnectionHelper.GetConnection(ConnectionString);
		}
		public static IDbConnection CreateConnection(string connectionString) {
			return ReflectConnectionHelper.GetConnection("Advantage.Data.Provider", "Advantage.Data.Provider.AdsConnection", connectionString);
		}
		protected override void CreateDataBase() {
			const int CannotOpenDatabaseError = 5004;
			ConnectionStringParser str = new ConnectionStringParser(ConnectionString);
			str.RemovePartByName("Pooling");
			using(IDbConnection conn = ConnectionHelper.GetConnection(str.GetConnectionString() + ";pooling=false;")) {
				try {
					conn.Open();
				} catch(Exception e) {
					object o;
					if(ConnectionHelper.TryGetExceptionProperty(e, "Number", out o)) {
						int number = (int)o;
						if(number == CannotOpenDatabaseError && CanCreateDatabase) {
							ConnectionStringParser helper = new ConnectionStringParser(ConnectionString);
							string baseFullPath = helper.GetPartByName("Data Source");
							helper.RemovePartByName("Data Source");
							string baseName = System.IO.Path.GetFileNameWithoutExtension(baseFullPath);
							string basePath = System.IO.Path.GetDirectoryName(baseFullPath);
							conn.ConnectionString = helper.GetConnectionString() + ";Data Source=" + basePath + ";tabletype=ads_adt";
							conn.Open();
							using(IDbCommand c = conn.CreateCommand()) {
								c.CommandText = "Create Database " + baseName;
								c.ExecuteNonQuery();
							}
						} else {
							throw new UnableToOpenDatabaseException(XpoDefault.ConnectionStringRemovePassword(ConnectionString), e);
						}
					} else {
						throw new UnableToOpenDatabaseException(XpoDefault.ConnectionStringRemovePassword(ConnectionString), e);
					}
				}
			}
		}
		protected override Exception WrapException(Exception e, IDbCommand query) {
			object o;
			if(ConnectionHelper.TryGetExceptionProperty(e, "Number", out o)) {
				int number = (int)o;
				if(number == 2121 || number == 7041)
					return new SchemaCorrectionNeededException(e);
				if(number == 5141 || number == 7057)
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
					inList.Add(":p" + i.ToString(CultureInfo.InvariantCulture));
					++i;
				}
			}
			if(inList.Count == 0)
				return new SelectStatementResult();
			return SelectData(new Query(string.Format(CultureInfo.InvariantCulture, queryText, StringListHelper.DelimitedText(inList, ",")), parameters, inList));
		}
		DBColumnType GetTypeFromCode(int code, out bool isIdentity, ref short size) {
			isIdentity = false;
			switch(code) {
				case 1:
					return DBColumnType.Boolean;
				case 2:
					return DBColumnType.Int64;
				case 3:
					return DBColumnType.DateTime;
				case 4:
					return DBColumnType.String;
				case 5:
					return DBColumnType.String;
				case 6:
					return DBColumnType.ByteArray;
				case 7:
					return DBColumnType.ByteArray;
				case 8:
					return DBColumnType.String;
				case 9:
					return DBColumnType.DateTime;
				case 10:
					return DBColumnType.Double;
				case 11:
					return DBColumnType.Int32;
				case 12:
					return DBColumnType.Int16;
				case 13:
					return DBColumnType.DateTime;
				case 14:
					return DBColumnType.DateTime;
				case 15:
					isIdentity = true;
					return DBColumnType.Int32;
				case 16:
					return DBColumnType.ByteArray;
				case 17:
					return DBColumnType.Decimal;
				case 18:
					return DBColumnType.Decimal;
				case 19:
					return DBColumnType.Int64;
				case 20:
				case 26: 
				case 27: 
					return DBColumnType.String;
				case 28: 
					size = -1;
					return DBColumnType.String;
				case 21:
					return DBColumnType.Int32;
				case 22:
					return DBColumnType.DateTime;
			}
			return DBColumnType.Unknown;
		}
		void GetColumns(DBTable table) {
			var tableNameParameter = new string[] { ":p" };
			var tableNameParameterValue = new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name)));
			if(!table.IsView) {				
				foreach(SelectStatementResultRow row in SelectData(new Query("select Name, Field_Type, Field_Length from system.columns where parent = :p", tableNameParameterValue, tableNameParameter)).Rows) {
					bool isIdentity;
					short size = Convert.ToInt16(row.Values[2]);
					DBColumnType getTypeFromCode = GetTypeFromCode(Convert.ToInt32(row.Values[1]), out isIdentity, ref size);
					string name = row.Values[0] as string;
					table.AddColumn(new DBColumn(name == null ? string.Empty : name.TrimEnd(), false, String.Empty, size, getTypeFromCode) {
						IsIdentity = isIdentity
					});
				}
			}
			if(table.Columns.Count == 0) {
				string viewStatement = GetScalar(new Query("select View_Stmt from system.views where name = :p", tableNameParameterValue, tableNameParameter)) as string;
				if(!string.IsNullOrEmpty(viewStatement)) {
					table.IsView = true;
					var result = SelectData(new Query(string.Format("select top 0 * from ({0}) t", viewStatement)), true);
					if(result != null && result.Rows != null && result.Rows.Length > 0) {
						foreach(var metadataColumn in result.Rows) {
							if(metadataColumn.Values.Length == 3) {
								string name = metadataColumn.Values[0] as string;
								string dbTypeName = metadataColumn.Values[1] as string;
								string dbColumnTypeString = metadataColumn.Values[2] as string;
								DBColumnType dbColumnType;
								if(!Enum.TryParse<DBColumnType>(dbColumnTypeString, out dbColumnType)) {
									dbColumnType = DBColumnType.Unknown;
								}
								table.AddColumn(new DBColumn(name == null ? string.Empty : name.TrimEnd(), false, dbTypeName ?? string.Empty, -1, dbColumnType));
							}
						}
					}
				}
			}
		}
		public override void CreatePrimaryKey(DBTable table) {
			base.CreateIndex(table, table.PrimaryKey);
			using(IDbCommand cmd = CreateCommand()) {
				try {
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "sp_ModifyTableProperty";
					AddParameter(cmd, "TableName", ComposeSafeTableName(table.Name));
					AddParameter(cmd, "Property", "TABLE_PRIMARY_KEY");
					AddParameter(cmd, "Value", ComposeSafeConstraintName(GetIndexName(table.PrimaryKey, table)));
					AddParameter(cmd, "ValidationOption", DBNull.Value);
					AddParameter(cmd, "FailTable", DBNull.Value);
					cmd.ExecuteNonQuery();
				} catch(Exception e) {
					throw WrapException(e, cmd);
				}
			}
		}
		void AddParameter(IDbCommand cmd, string name, object value) {
			IDbDataParameter p = cmd.CreateParameter();
			p.ParameterName = name;
			p.Value = value;
			cmd.Parameters.Add(p);
		}
		void GetPrimaryKey(DBTable table) {
			SelectStatementResult data = SelectData(new Query(@"select i.Index_Expression from system.indexes i inner join system.tables t on t.Table_Primary_Key = i.Name where t.Name = :p",
				new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { ":p" }));
			if(data.Rows.Length > 0) {
				StringCollection cols = new StringCollection();
				if(data.Rows[0].Values[0] is DBNull) return;
				string columns = (string)data.Rows[0].Values[0];
				cols.AddRange(columns.TrimEnd().Split(';'));
				foreach(string columnName in cols) {
					DBColumn column = table.GetColumn(columnName);
					if(column != null)
						column.IsKey = true;
				}
				table.PrimaryKey = new DBPrimaryKey(cols);
			}
		}
		bool IsColumnsEqual(StringCollection first, StringCollection second) {
			if(first.Count != second.Count)
				return false;
			for(int i = 0; i < first.Count; i++)
				if(String.Compare(ComposeSafeColumnName(first[i]), ComposeSafeColumnName(second[i]), true) != 0)
					return false;
			return true;
		}
		public override void CreateIndex(DBTable table, DBIndex index) {
			if(table.Name != "XPObjectType" && (table.PrimaryKey == null || !IsColumnsEqual(table.PrimaryKey.Columns, index.Columns))) {
				base.CreateIndex(table, index);
			}
		}
		void GetIndexes(DBTable table) {
			SelectStatementResult data = SelectData(new Query(
				@"select Name, Index_Expression, Index_Options from system.indexes where Index_FTS_Delimiters is null and Parent = :p", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "p" }));
			foreach(SelectStatementResultRow row in data.Rows) {
				StringCollection list = new StringCollection();
				if(row.Values[1] is DBNull) continue;
				list.AddRange(((string)row.Values[1]).TrimEnd().Split(';'));
				DBIndex index = new DBIndex(((string)row.Values[0]).TrimEnd(), list, (Convert.ToInt32(row.Values[2]) & 1) != 0 ? true : false);
				table.Indexes.Add(index);
			}
		}
		void GetForeignKeys(DBTable table) {
			SelectStatementResult data = SelectData(new Query(
				@"select r.RI_Primary_Table, i.Index_Expression, ip.Index_Expression from system.relations r
				inner join system.indexes i on r.RI_Foreign_Index = i.Name
				inner join system.indexes ip on r.RI_Primary_Index = ip.Name
				where r.RI_Foreign_Table = :p", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "p" }));
			Hashtable fks = new Hashtable();
			foreach(SelectStatementResultRow row in data.Rows) {
				StringCollection fkc = new StringCollection();
				if(row.Values[1] is DBNull || row.Values[2] is DBNull) continue;
				fkc.AddRange(((string)row.Values[1]).TrimEnd().Split(';'));
				StringCollection pkc = new StringCollection();
				pkc.AddRange(((string)row.Values[2]).TrimEnd().Split(';'));
				DBForeignKey fk = new DBForeignKey(fkc, (string)row.Values[0], pkc);
				table.ForeignKeys.Add(fk);
			}
		}
		public override void CreateForeignKey(DBTable table, DBForeignKey fk) {
			using(IDbCommand cmd = CreateCommand()) {
				try {
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "sp_CreateReferentialIntegrity";
					AddParameter(cmd, "Name", ComposeSafeConstraintName(GetForeignKeyName(fk, table)));
					AddParameter(cmd, "PrimaryTable", ComposeSafeTableName(fk.PrimaryKeyTable));
					AddParameter(cmd, "ForeignTable", ComposeSafeTableName(table.Name));
					AddParameter(cmd, "ForeignKey", ComposeSafeConstraintName(GetIndexName(new DBIndex(fk.Columns, false), table)));
					AddParameter(cmd, "UpdateRule", 2);
					AddParameter(cmd, "DeleteRule", 2);
					AddParameter(cmd, "FailTable", DBNull.Value);
					AddParameter(cmd, "PrimaryKeyError", DBNull.Value);
					AddParameter(cmd, "CascadeError", DBNull.Value);
					cmd.ExecuteNonQuery();
				} catch(Exception e) {
					throw WrapException(e, cmd);
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
			foreach(SelectStatementResultRow row in GetDataForTables(tables, null, "select name from system.tables where name in ({0})").Rows)
				dbTables.Add(((string)row.Values[0]).TrimEnd(), false);
			foreach(SelectStatementResultRow row in GetDataForTables(tables, null, "select name from system.views where name in ({0})").Rows)
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
			return 60;
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
					return string.Format(CultureInfo.InvariantCulture, "{0} % {1}", leftOperand, rightOperand);
				case BinaryOperatorType.BitwiseOr:
					return string.Format(CultureInfo.InvariantCulture, "({0} Or {1})", leftOperand, rightOperand);
				case BinaryOperatorType.BitwiseAnd:
					return string.Format(CultureInfo.InvariantCulture, "({0} And {1})", leftOperand, rightOperand);
				default:
					return base.FormatBinary(operatorType, leftOperand, rightOperand);
			}
		}
		ExecMethodDelegate commandBuilderDeriveParametersHandler;
		protected override void CommandBuilderDeriveParameters(IDbCommand command) {
			if(commandBuilderDeriveParametersHandler == null) {
				commandBuilderDeriveParametersHandler = ReflectConnectionHelper.GetCommandBuilderDeriveParametersDelegate("Advantage.Data.Provider", "Advantage.Data.Provider.AdsCommandBuilder");
			}
			commandBuilderDeriveParametersHandler(command);
		}
		protected override SelectedData ExecuteSproc(string sprocName, params OperandValue[] parameters) {
			using(IDbCommand command = CreateCommand()) {
				command.CommandText = "select count(*) from system.storedprocedures where name = :p0";
				command.Parameters.Add(CreateParameter(command, sprocName, ":p0"));
				if(((int)command.ExecuteScalar()) > 0) {
					command.Parameters.Clear();
					command.CommandType = CommandType.StoredProcedure;
					command.CommandText = ComposeSafeTableName(sprocName);
					CommandBuilderDeriveParameters(command);
					IDataParameter returnParameter;
					List<IDataParameter> outParameters;
					PrepareParametersForExecuteSproc(parameters, command, out outParameters, out returnParameter);
					return ExecuteSprocInternal(command, returnParameter, outParameters);
				}
				string[] parametersList = new string[parameters.Length];
				command.Parameters.Clear();
				for(int i = 0; i < parameters.Length; i++) {
					bool createParameter = true;
					parametersList[i] = GetParameterName(parameters[0], i, ref createParameter);
					if(createParameter) {
						command.Parameters.Add(CreateParameter(command, parameters[i].Value, parametersList[i]));
					}
				}
				command.CommandText = string.Format("select {0}({1}) from system.iota", sprocName, string.Join(", ", parametersList));
				List<SelectStatementResult> selectStatementResults = GetSelectedStatmentResults(command);
				return new SelectedData(selectStatementResults.ToArray());
			}
		}
		public override DBStoredProcedure[] GetStoredProcedures() {
			IDbCommand command = Connection.CreateCommand();
			command.CommandText = "select name from system.storedprocedures";
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
				} catch {
					if(!reader.IsClosed) {
						reader.Close();
					}
				}
			}
			return result.ToArray();
		}
		static string FormatMod(string arg, int multiplier, int divider) {
			return string.Format("Cast(Truncate(Cast({0} as SQL_DOUBLE) * {1}, 0) % {2} as sql_integer)", arg, multiplier, divider);
		}
		static string FormatGetInt(string arg, int multiplier, int divider) {
			return string.Format("Cast(Truncate(Cast({0} as SQL_DOUBLE) * {1} / {2}, 0) as sql_integer)", arg, multiplier, divider);
		}
		static string FnAddDateTime(string datetimeOperand, string dayPart, string secondPart) {
			return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_Second, {2},TIMESTAMPADD(SQL_TSI_DAY, {1},(cast({0} as sql_timestamp))))", datetimeOperand, dayPart, secondPart);
		}
		public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.Abs:
					return string.Format(CultureInfo.InvariantCulture, "(abs(cast({0} as SQL_NUMERIC(20,10))))", operands[0]);
				case FunctionOperatorType.BigMul:
					return string.Format(CultureInfo.InvariantCulture, "(cast({0} as sql_numeric(38,0)) * cast({1} as sql_numeric(38,0) ))", operands[0], operands[1]);
				case FunctionOperatorType.Sqr:
					return string.Format(CultureInfo.InvariantCulture, "sqrt({0})", operands[0]);
				case FunctionOperatorType.Rnd:
					return "rand()";
				case FunctionOperatorType.Log:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "Log({0})", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(Log({0})/Log({1}))", operands[0], operands[1]);
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
					return string.Format(CultureInfo.InvariantCulture, "Atan2({0},{1})", operands[0], operands[1]);
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
					return string.Format(CultureInfo.InvariantCulture, "iif({0} > {1}, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Min:
					return string.Format(CultureInfo.InvariantCulture, "iif({0} < {1}, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Floor:
					return string.Format(CultureInfo.InvariantCulture, "Floor({0})", operands[0]);
				case FunctionOperatorType.Ceiling:
					return string.Format(CultureInfo.InvariantCulture, "Ceiling( (cast({0} as SQL_DOUBLE)) )", operands[0]);
				case FunctionOperatorType.IsNull:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "({0} is null)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "Coalesce({0}, {1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) is null or length({0}) = 0)", operands[0]);
				case FunctionOperatorType.GetMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "Frac_Second((cast({0} as sql_timestamp)))", operands[0]);
				case FunctionOperatorType.GetSecond:
					return string.Format(CultureInfo.InvariantCulture, "Second({0})", operands[0]);
				case FunctionOperatorType.GetMinute:
					return string.Format(CultureInfo.InvariantCulture, "Minute({0})", operands[0]);
				case FunctionOperatorType.GetHour:
					return string.Format(CultureInfo.InvariantCulture, "Hour({0})", operands[0]);
				case FunctionOperatorType.GetDay:
					return string.Format(CultureInfo.InvariantCulture, "DayOfMonth({0})", operands[0]);
				case FunctionOperatorType.GetMonth:
					return string.Format(CultureInfo.InvariantCulture, "Month({0})", operands[0]);
				case FunctionOperatorType.GetYear:
					return string.Format(CultureInfo.InvariantCulture, "Year({0})", operands[0]);
				case FunctionOperatorType.GetTimeOfDay:
					return string.Format(CultureInfo.InvariantCulture, @"(cast(((cast((cast(Hour(cast({0} as sql_timestamp)) as sql_numeric(19,0)) * 36000000000) as sql_numeric(19,0))) + 
                                                                         (cast((cast(Minute(cast({0} as sql_timestamp)) as sql_numeric(19,0)) * 600000000) as sql_numeric(19,0))) + 
                                                                         (cast((cast(Second(cast({0} as sql_timestamp)) as sql_numeric(19,0)) * 10000000) as sql_numeric(19,0)))) as sql_numeric(19,0)))", operands[0]);
				case FunctionOperatorType.GetDayOfWeek:
					return string.Format(CultureInfo.InvariantCulture, "(Mod(((DayOfWeek(cast({0} as sql_timestamp))- DayOfWeek('1900-01-01'))  + 8) , 7))", operands[0]);
				case FunctionOperatorType.GetDayOfYear:
					return string.Format(CultureInfo.InvariantCulture, "DayOfYear(cast({0} as sql_date))", operands[0]);
				case FunctionOperatorType.GetDate:
					return string.Format(CultureInfo.InvariantCulture, "(cast({0} as sql_date))", operands[0]);
				case FunctionOperatorType.AddTicks:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_SECOND, (cast(({1} / 10000000) as sql_integer)),(cast({0} as sql_timestamp)))", operands[0], operands[1]);
				case FunctionOperatorType.AddMilliSeconds:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_SECOND, (cast(({1} / 1000) as sql_integer)),(cast({0} as sql_timestamp)))", operands[0], operands[1]);
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
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_MONTH, cast({1} as sql_integer),(cast({0} as sql_timestamp)))", operands[0], operands[1]);
				case FunctionOperatorType.AddYears:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_YEAR, cast({1} as sql_integer),(cast({0} as sql_timestamp)))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffYear:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPDIFF(SQL_TSI_YEAR,(cast({0} as sql_timestamp)),(cast({1} as sql_timestamp)))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMonth:
					return string.Format(CultureInfo.InvariantCulture, "(((Year({1}) - Year({0})) * 12) + Month({1}) - Month({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffDay:
					return string.Format(CultureInfo.InvariantCulture, "(TIMESTAMPDIFF(SQL_TSI_Day, (cast({0} as sql_date)), (cast({1} as sql_date))))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffHour:
					return string.Format(CultureInfo.InvariantCulture, "(TIMESTAMPDIFF(SQL_TSI_Day,(cast({0} as sql_date)),(cast({1} as sql_date)))*24 + Hour({1}) - Hour({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMinute:
					return string.Format(CultureInfo.InvariantCulture, "((TIMESTAMPDIFF(SQL_TSI_Day,(cast({0} as sql_date)),(cast({1} as sql_date)))*24 + Hour({1}) - Hour({0})) * 60 + Minute({1}) - Minute({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffSecond:
					return string.Format(CultureInfo.InvariantCulture, "(TIMESTAMPDIFF(SQL_TSI_Second,(cast({0} as sql_timestamp)),(cast({1} as sql_timestamp))) + iif((Frac_Second((cast({1} as sql_timestamp))) - Frac_Second((cast({0} as sql_timestamp)))) < 1, 1, 0))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "((TIMESTAMPDIFF(SQL_TSI_Second,(cast({0} as sql_timestamp)),(cast({1} as sql_timestamp)))) * 1000 + Frac_Second((cast({1} as sql_timestamp))) - Frac_Second((cast({0} as sql_timestamp))))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffTick:
					return string.Format(CultureInfo.InvariantCulture, "(((TIMESTAMPDIFF(SQL_TSI_Second,(cast({0} as sql_timestamp)),(cast({1} as sql_timestamp)))) * 1000 + Frac_Second((cast({1} as sql_timestamp))) - Frac_Second((cast({0} as sql_timestamp)))) * 10000)", operands[0], operands[1]);
				case FunctionOperatorType.Now:
					return "Now()";
				case FunctionOperatorType.Today:
					return "( cast((Now()) as sql_date) )";
				case FunctionOperatorType.Ascii:
					return string.Format(CultureInfo.InvariantCulture, "Ascii({0})", operands[0]);
				case FunctionOperatorType.Char:
					return string.Format(CultureInfo.InvariantCulture, "Char({0})", operands[0]);
				case FunctionOperatorType.ToInt:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS SQL_INTEGER)", operands[0]);
				case FunctionOperatorType.ToLong:
					throw new NotSupportedException();
				case FunctionOperatorType.ToFloat:
					throw new NotSupportedException();
				case FunctionOperatorType.ToDouble:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS SQL_DOUBLE)", operands[0]);
				case FunctionOperatorType.ToDecimal:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} as SQL_MONEY)", operands[0]);
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, "cast({0} as SQL_VARCHAR(32000))", operands[0]);
				case FunctionOperatorType.Concat:
					return FnConcat(operands);
				case FunctionOperatorType.Replace:
					return string.Format(CultureInfo.InvariantCulture, "Replace({0},(cast({1} as sql_varchar(64))), (cast({2} as sql_varchar(32000))))", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Reverse:
					throw new NotSupportedException();
				case FunctionOperatorType.PadLeft:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(iif(({1} > (Length({0}))) , space({1} - (Length({0}))), '') + {0} )", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "(iif(({1} > (Length({0}))) , repeat(cast({2} as sql_varchar(1)), ({1} - (Length({0})))) , '') + {0} )", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.PadRight:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(cast({0} as sql_varchar(32000)) + iif(({1} > (Length({0}))) , space({1} - (Length({0}))), '') )", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "(cast({0} as sql_varchar(32000)) + iif(({1} > (Length({0}))) , repeat(cast({2} as sql_varchar(1)), ({1} - (Length({0})))), ''))", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.Substring:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "Substring({0}, {1} + 1, (Length({0}) - {1} ))", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "Substring({0}, {1} + 1, {2})", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.Insert:
					return string.Format(CultureInfo.InvariantCulture, "insert({0}, {1} + 1, 0, cast({2} as sql_varchar(32000)))", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Remove:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "insert({0}, {1} + 1,(Length({0}) - {1}), '')", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "insert({0}, {1} + 1, {2}, '')", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.Len:
					return string.Format(CultureInfo.InvariantCulture, "Length({0})", operands[0]);
				case FunctionOperatorType.CharIndex:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(Locate( (cast({0} as sql_varchar(1))) , {1}) - 1)", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "(Locate((cast({0} as sql_varchar(1))), {1}, {2} + 1) - 1)", operands[0], operands[1], operands[2]);
						case 4:
							return string.Format(CultureInfo.InvariantCulture, "(iif (Locate((cast({0} as sql_varchar(1))), Left({1}, ({2} - 1 + {3})), {2} + 1)=0,-1,Locate((cast({0} as sql_varchar(1))), Left({1}, ({2} - 1 + {3})), {2} + 1) - 1 + {2} ))", operands[0], operands[1], operands[2], operands[3]);
					}
					goto default;
				case FunctionOperatorType.EndsWith:
					return string.Format(CultureInfo.InvariantCulture, "(Right(Substring({0}, 1, Length({0})), Length({1})) = ({1}))", operands[0], operands[1]);
				case FunctionOperatorType.Contains:
					return string.Format(CultureInfo.InvariantCulture, "(Locate({1}, {0}) > 0)", operands[0], operands[1]);
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
							return string.Format(CultureInfo.InvariantCulture, "(({0} like {2}) And (Locate({1}, {0}) = 1))", processParameter(operands[0]), processParameter(secondOperand), new ConstantValue(operandString.Substring(0, likeIndex) + "%"));
						}
					}
					return string.Format(CultureInfo.InvariantCulture, "(Locate({1}, {0}) = 1)", processParameter(operands[0]), processParameter(operands[1]));
				default:
					return base.FormatFunction(processParameter, operatorType, operands);
			}
		}
		string FnConcat(string[] operands) {
			string args = "(";
			foreach(string arg in operands) {
				if(args.Length > 1)
					args += " + ";
				args += string.Format("cast({0} as sql_varchar(32000))", arg);
			}
			return args + ")";
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
			return ":p" + index.ToString(CultureInfo.InvariantCulture);
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
			SelectStatementResult constraints = SelectData(new Query("select Name from system.relations"));
			command.CommandText = "sp_DropReferentialIntegrity";
			command.CommandType = CommandType.StoredProcedure;
			ReflectConnectionHelper.InvokeMethod(command.Parameters, "Add", new object[] { "Name", DbType.AnsiString }, true);
			foreach(SelectStatementResultRow row in constraints.Rows) {
				ReflectConnectionHelper.SetPropertyValue(command.Parameters[0], "Value", ((string)row.Values[0]).TrimEnd());
				command.ExecuteNonQuery();
			}
			command.CommandType = CommandType.Text;
			command.Parameters.Clear();
			string[] tables = GetStorageTablesList(false);
			foreach(string table in tables) {
				command.CommandText = "drop table \"" + table + "\"";
				command.ExecuteNonQuery();
			}
		}
		protected override void ProcessClearDatabase() {
			using(IDbCommand command = CreateCommand())
				ClearDatabase(command);
		}
		public override string[] GetStorageTablesList(bool includeViews) {
			SelectStatementResult tables = SelectData(new Query("select name from system.tables"));
			ArrayList result = new ArrayList(tables.Rows.Length);
			foreach(SelectStatementResultRow row in tables.Rows) {
				result.Add(((string)row.Values[0]).TrimEnd());
			}
			if(includeViews) {
				SelectStatementResult views = SelectData(new Query("select name from system.views"));
				foreach(SelectStatementResultRow row in views.Rows) {
					result.Add(((string)row.Values[0]).TrimEnd());
				}
			}
			return (string[])result.ToArray(typeof(string));
		}
	}
	public class AdvantageProviderFactory : ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return AdvantageConnectionProvider.CreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return AdvantageConnectionProvider.CreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(DatabaseParamID)) { return null; }
			if(!parameters.ContainsKey(UserIDParamID)) {
				if(!parameters.ContainsKey(PasswordParamID)) {
					return AdvantageConnectionProvider.GetConnectionString(parameters[DatabaseParamID]);
				}
				return AdvantageConnectionProvider.GetConnectionString(parameters[DatabaseParamID], parameters[PasswordParamID]);
			}
			return AdvantageConnectionProvider.GetConnectionString(parameters[DatabaseParamID], parameters[UserIDParamID], parameters[PasswordParamID]);
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
		public override string ProviderKey { get { return AdvantageConnectionProvider.XpoProviderTypeString; } }
		public override string[] GetDatabases(string server, string userid, string password) {
			return new string[1] { server };
		}
		public override string FileFilter { get { return "Advantage databases|*.add"; } }
		public override bool MeanSchemaGeneration { get { return true; } }
	}
}
