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
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
#if DXRESTRICTED
using IDbTransaction = System.Data.Common.DbTransaction;
using IDataReader = System.Data.Common.DbDataReader;
using IDbConnection = System.Data.Common.DbConnection;
using IDbCommand = System.Data.Common.DbCommand;
using IDataParameter = System.Data.Common.DbParameter;
using IDbDataParameter = System.Data.Common.DbParameter;
#endif
namespace DevExpress.DataAccess.Native.Sql.ConnectionProviders {
	public class DataAccessSQLiteConnectionProvider : SQLiteConnectionProvider, IAliasFormatter, IDataStoreSchemaExplorerEx, IDataStoreEx {
		#region static
		static DBColumnType GetColumnType(string typeName) {
			typeName = typeName.ToLower(CultureInfo.InvariantCulture);
			switch(typeName) {
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
				case "timestamp":
					return DBColumnType.DateTime;
				case "money":
					return DBColumnType.Decimal;
				case "real":
				case "double precision":
				case "double":
				case "float":
					return DBColumnType.Double;
				case "smallint":
					return DBColumnType.Int16;
			}
			if(typeName.StartsWith("nvarchar") || typeName.StartsWith("varchar") || typeName.StartsWith("text"))
				return DBColumnType.String;
			if(typeName.StartsWith("numeric"))
				return DBColumnType.Decimal;
			if(typeName.StartsWith("char") || typeName.StartsWith("nchar")) {
				int pos = typeName.IndexOf('(') + 1;
				if(pos == 0)
					return DBColumnType.Char;
				int size = Int32.Parse(typeName.Substring(pos, typeName.IndexOf(')') - pos));
				if(size == 1)
					return DBColumnType.Char;
				return DBColumnType.String;
			}
			if(typeName.StartsWith("image"))
				return DBColumnType.ByteArray;
			if(typeName.StartsWith("blob"))
				return DBColumnType.ByteArray;
			return DBColumnType.Unknown;
		}
		static int GetColumnLength(string typeName) {
			typeName = typeName.ToLower(CultureInfo.InvariantCulture);
			if(typeName.StartsWith("nvarchar") || typeName.StartsWith("varchar") || typeName.StartsWith("text")) {
				int pos = typeName.IndexOf('(') + 1;
				if(pos > 0)
					return Int32.Parse(typeName.Substring(pos, typeName.IndexOf(')') - pos));
			}
			if(typeName.StartsWith("char") || typeName.StartsWith("nchar")) {
				int pos = typeName.IndexOf('(') + 1;
				if(pos != 0) {
					int size = Int32.Parse(typeName.Substring(pos, typeName.IndexOf(')') - pos));
					if(size == 1)
						return size;
				}
			}
			if(typeName.StartsWith("image")) {
				int pos = typeName.IndexOf('(') + 1;
				if(pos != 0)
					return Int32.Parse(typeName.Substring(pos, typeName.IndexOf(')') - pos));
			}
			return 0;
		}
		#endregion
		const string aliasLead = "[";
		const string aliasEnd = "]";
		const bool singleQuotedString = true;
		[SuppressMessage("Microsoft.Design", "CA1021")]
		public static IDataStore DataAccessCreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection};
			return DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore DataAccessCreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DataAccessSQLiteConnectionProvider(connection, autoCreateOption);
		}
		static DataAccessSQLiteConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterDataStoreProvider("System.Data.SQLite.SQLiteConnection", DataAccessCreateProviderFromConnection);
			RegisterFactory(new DataAccessSQLiteProviderFactory());
		}
		IDataStoreEx DataStoreEx { get { return this; } }
		public static void ProviderRegister() {
		}
		public DataAccessSQLiteConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			bool shouldCreateParameter = createParameter;
			OperandParameter param = parameter as OperandParameter;
			if(!ReferenceEquals(param, null)) {
				createParameter = true;
				return string.Format("@{0}", param.ParameterName);
			}
			object value = parameter.Value;
			if(value != null) {
				createParameter = false;
				switch(DXTypeExtensions.GetTypeCode(value.GetType())) {
					case TypeCode.Int64:
						return ((long)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.Decimal:
						return ((decimal)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.DateTime:
						return string.Format("'{0}'", ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
				}
			}
			createParameter = shouldCreateParameter;
			return base.GetParameterName(new ConstantValue(parameter.Value), index, ref createParameter);
		}
		protected override string GetSafeNameRoot(string originalName) {
			return originalName;
		}
		public override string ComposeSafeColumnName(string columnName) {
			return columnName;
		}
		public override string FormatColumn(string columnName, string tableAlias) {
			return base.FormatColumn(columnName, ConnectionProviderHelper.EncloseAlias(tableAlias, aliasLead, aliasEnd));
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			if(tableName == SubSelectStatement.CustomSqlString)
				return tableAlias;
			string encloseAlias = ConnectionProviderHelper.EncloseAlias(tableAlias, aliasLead, aliasEnd);
			return string.IsNullOrEmpty(encloseAlias) ? base.FormatTable(schema, tableName) : base.FormatTable(schema, tableName, encloseAlias);
		}
		#region IAliasFormatter
		string IAliasFormatter.AliasLead { get { return aliasLead; } }
		string IAliasFormatter.AliasEnd { get { return aliasEnd; } }
		bool IAliasFormatter.SingleQuotedString { get { return singleQuotedString; } }
		int IAliasFormatter.MaxTableAliasLength { get { return GetSafeNameTableMaxLength(); } }
		int IAliasFormatter.MaxColumnAliasLength { get { return GetSafeNameColumnMaxLength(); } }
		#endregion
		#region IDataStoreSchemaExplorerEx Members
		public string[] GetStorageDataObjectsNames(DataObjectTypes types) {
			List<string> result = new List<string>();
			Query getTablesListQuery = new Query(string.Format(@"SELECT [name]
                                                               FROM   [sqlite_master]
                                                               WHERE  ({0}) and [name] not like 'SQLITE_%'",
				ConnectionProviderHelper.GetDataObjectTypeCondition(types, "[type] LIKE 'table'", "[type] LIKE 'view'")));
			DataStoreEx.ProcessQuery(CancellationToken.None, getTablesListQuery, (reader, cancellationToken)=> {
				while(reader.Read()) {
					result.Add(reader.GetString(0));
				}
			});
			return result.ToArray();
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageTables(params string[] tablesList) {
			return ((IDataStoreSchemaExplorerEx)this).GetStorageTables(false, tablesList);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageTables(bool includeColumns, params string[] tablesList) {
			return GetStorageDataObjects(includeColumns, false, tablesList);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageViews(params string[] tablesList) {
			return ((IDataStoreSchemaExplorerEx)this).GetStorageViews(false, tablesList);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageViews(bool includeColumns, params string[] tablesList) {
			return GetStorageDataObjects(includeColumns, true, tablesList);
		}
		void IDataStoreSchemaExplorerEx.GetColumns(params DBTable[] tables) {
			GetStorageTablesColumns(tables);
		}
		DBTable[] GetStorageDataObjects(bool includeColumns, bool isView, params string[] tablesList) {
			string[] tableNames = ((IDataStoreSchemaExplorerEx)this).GetStorageDataObjectsNames(isView ? DataObjectTypes.Views : DataObjectTypes.Tables);
			IEnumerable<string> filteredTables = tablesList.Length == 0 ? tableNames : tableNames.Where(n => tablesList.Any(f => f == n));
			DBTable[] tables = filteredTables.Select(p => ConnectionProviderHelper.CreateTable(p, isView)).ToArray();
			if(includeColumns)
				GetStorageTablesColumns(tables);
			GetStorageTablesForeignKeys(tables);
			return tables;
		}
		void GetStorageTablesColumns(IEnumerable<DBTable> tables) {
			foreach(DBTable table in tables) {
				table.Columns.Clear();
				DataStoreEx.ProcessQuery(CancellationToken.None, new Query(string.Format("PRAGMA table_info('{0}')", table.Name)), (reader, cancellationToken)=> {
					while(reader.Read()) {
						string columnName = reader.GetString(1);
						string typeName = reader.GetString(2);
						int length = GetColumnLength(typeName);
						DBColumnType type = GetColumnType(typeName);
						DBColumn column = ConnectionProviderHelper.CreateColumn(columnName, false, false, length, type);
						table.AddColumn(column);
					}
				});
			}
		}
		void GetStorageTablesForeignKeys(IEnumerable<DBTable> tables) {
			foreach(DBTable table in tables) {
				table.ForeignKeys.Clear();
				Query getTablesForeignKeysQuery = new Query(string.Format("PRAGMA foreign_key_list('{0}')", table.Name));
					string foreignTableName = table.Name;
					IList<DBForeignKey> foreignKeys = table.ForeignKeys;
				DataStoreEx.ProcessQuery(CancellationToken.None, getTablesForeignKeysQuery, (reader, cancellationToken)=> {
					while(reader.Read()) {
						string primaryTableName = reader.GetString(2).Trim('\"');
						string keyName = string.Format("{0}_FK_{1}", foreignTableName, reader.GetInt64(0));
						DBForeignKey fk = foreignKeys.FirstOrDefault(k => k.Name == keyName);
						if(fk == null) {
							fk = new DBForeignKey {
								Name = keyName, PrimaryKeyTable = primaryTableName
							};
							foreignKeys.Add(fk);
						}
						string foreignColumnName = reader.IsDBNull(3) ? null : reader.GetString(3);
						string primaryColumnName = reader.IsDBNull(4) ? null : reader.GetString(4);
						if(primaryColumnName != null && foreignColumnName != null) {
							fk.Columns.Add(foreignColumnName);
							fk.PrimaryKeyTableKeyColumns.Add(primaryColumnName);
						}
					}
				});
			}
		}
		#endregion
		#region IDataStoreEx
		void IDataStoreEx.ProcessQuery(CancellationToken cancellationToken, Query query, Action<IDataReader, CancellationToken> action) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					using(IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess)) {
						action(reader, cancellationToken);
					}
				}
			}
		}
		void IDataStoreEx.ProcessStoredProc(CancellationToken cancellationToken, string sprocName, Action<IDataReader, CancellationToken> action, params OperandValue[] parameters) {
			throw new NotSupportedException();
		}
		SelectedDataEx IDataStoreEx.SelectData(CancellationToken cancellationToken, Query query, string[] columns) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					return DataStoreExHelper.GetData(command, cancellationToken, columns, NativeSkipTakeSupported, query.SkipSelectedRecords);
				}
			}
		}
		ColumnInfoEx[] IDataStoreEx.SelectSchema(Query query) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					return DataStoreExHelper.GetSchema(command);
				}
			}
		}
		SelectedDataEx IDataStoreEx.ExecuteStoredProcedure(CancellationToken cancellationToken, string sprocName, params OperandValue[] parameters) {
			throw new NotSupportedException();
		}
		#endregion
	}
	public class DataAccessSQLiteProviderFactory : SQLiteProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessSQLiteConnectionProvider.DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessSQLiteConnectionProvider.DataAccessCreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
	}
}
