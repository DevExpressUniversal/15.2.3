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
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.DataAccess.Native.Sql.ConnectionProviders {
	public class DataAccessMSSqlCEConnectionProvider : MSSqlCEConnectionProvider, IAliasFormatter, ISupportOrderByExpressionAlias, IDataStoreSchemaExplorerEx, IDataStoreEx {
		#region static
		static DBColumnType GetColumnType(string typeName, int length) {
			switch(typeName) {
				case "int":
					return DBColumnType.Int32;
				case "image":
				case "varbinary":
					return DBColumnType.ByteArray;
				case "nchar":
				case "char":
					if(length == 1)
						return DBColumnType.Char;
					return DBColumnType.String;
				case "varchar":
				case "nvarchar":
				case "xml":
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
				case "decimal":
					return DBColumnType.Decimal;
				case "money":
				case "smallmoney":
					return DBColumnType.Decimal;
				case "float":
					return DBColumnType.Double;
				case "real":
					return DBColumnType.Single;
				case "uniqueidentifier":
					return DBColumnType.Guid;
				case "datetime":
				case "datetime2":
				case "smalldatetime":
				case "date":
					return DBColumnType.DateTime;
			}
			return DBColumnType.Unknown;
		}
		#endregion
		const string aliasLead = "\"";
		const string aliasEnd = "\"";
		const bool singleQuotedString = true;
		[SuppressMessage("Microsoft.Design", "CA1021")]
		public static IDataStore DataAccessCreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = ReflectConnectionHelper.GetConnection("System.Data.SqlServerCe", "System.Data.SqlServerCe.SqlCeConnection", connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection};
			return DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore DataAccessCreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DataAccessMSSqlCEConnectionProvider(connection, autoCreateOption);
		}
		static DataAccessMSSqlCEConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterDataStoreProvider("System.Data.SqlServerCe.SqlCeConnection", DataAccessCreateProviderFromConnection);
			RegisterFactory(new DataAccessMSSqlCEProviderFactory());
		}
		public static void ProviderRegister() {
		}
		IDataStoreEx DataStoreEx { get { return this; } }
		public DataAccessMSSqlCEConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
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
				switch(Type.GetTypeCode(value.GetType())) {
					case TypeCode.Decimal:
						return ((decimal)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.DateTime:
						return string.Format("convert(datetime, '{0}', 120)", ((DateTime)value).ToString("yyyy-MM-dd HH':'mm':'ss", CultureInfo.InvariantCulture));
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
		string[] IDataStoreSchemaExplorerEx.GetStorageDataObjectsNames(DataObjectTypes types) {
			if((types & DataObjectTypes.Tables) == 0)
				return new string[0];
			List<string> result = new List<string>();
			const string getTablesListQuery = @"select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_TYPE = 'TABLE'";
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesListQuery), (reader, cancellationToken)=> {
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
			GetStorageTablesColumns(tables, true);
		}
		DBTable[] GetStorageDataObjects(bool includeColumns, bool isView, params string[] tablesList) {
			string[] tableNames = ((IDataStoreSchemaExplorerEx)this).GetStorageDataObjectsNames(isView ? DataObjectTypes.Views : DataObjectTypes.Tables);
			IEnumerable<string> filteredTables = tablesList.Length == 0 ? tableNames : tableNames.Where(n => tablesList.Any(f => f == n));
			DBTable[] tables = filteredTables.Select(p => ConnectionProviderHelper.CreateTable(p, isView)).ToArray();
			if(includeColumns)
				GetStorageTablesColumns(tables, tablesList.Length != 0);
			GetStorageTablesForeignKeys(tables, tablesList.Length != 0);
			return tables;
		}
		static string GetTablesFilter(ICollection<DBTable> tables, string prefix, bool useTablesFilter) {
			return (useTablesFilter && tables.Count != 0) ? string.Format("{0} in ({1})", prefix, string.Join(", ", tables.Select(n => string.Format("'{0}'", n.Name)))) : string.Empty;
		}
		void GetStorageTablesColumns(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.Columns.Clear();
			string getTablesColumnsQuery = string.Format(@"select TABLE_NAME, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
                                                           from INFORMATION_SCHEMA.COLUMNS {0}", GetTablesFilter(tables, "where TABLE_NAME", useTablesFilter));
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesColumnsQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string tableName = reader.GetString(0);
					if(table == null || !string.Equals(table.Name, tableName))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, tableName));
					if(table == null)
						continue;
					string columnName = reader.GetString(1);
					string typeName = reader.GetString(2);
					int size = !reader.IsDBNull(3) ? ((IConvertible)reader.GetValue(3)).ToInt32(CultureInfo.InvariantCulture) : 0;
					DBColumnType type = GetColumnType(typeName, size);
					table.AddColumn(ConnectionProviderHelper.CreateColumn(columnName, false, false, type == DBColumnType.String ? size : 0, type));
				}
			});
		}
		void GetStorageTablesForeignKeys(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.ForeignKeys.Clear();
			string getTablesForeignKeysQuery = string.Format(@"select c.TABLE_NAME, c.COLUMN_NAME, c.CONSTRAINT_NAME, cr.TABLE_NAME, cr.COLUMN_NAME
                                                               from INFORMATION_SCHEMA.TABLE_CONSTRAINTS fk
                                                                join INFORMATION_SCHEMA.KEY_COLUMN_USAGE c on c.CONSTRAINT_NAME = fk.CONSTRAINT_NAME
                                                                join INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc on rc.CONSTRAINT_NAME = fk.CONSTRAINT_NAME
                                                                join INFORMATION_SCHEMA.KEY_COLUMN_USAGE cr on cr.CONSTRAINT_NAME = rc.UNIQUE_CONSTRAINT_NAME and cr.ORDINAL_POSITION = c.ORDINAL_POSITION
                                                               where fk.CONSTRAINT_TYPE = 'FOREIGN KEY' {0}
                                                               order by c.CONSTRAINT_NAME, c.ORDINAL_POSITION", GetTablesFilter(tables, "and c.TABLE_NAME", useTablesFilter));
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesForeignKeysQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				DBForeignKey fk = null;
				while(reader.Read()) {
					string foreignTableName = reader.GetString(0);
					if(table == null || !string.Equals(table.Name, foreignTableName)) {
						table = tables.FirstOrDefault(t => string.Equals(t.Name, foreignTableName));
						fk = null;
					}
					if(table == null)
						continue;
					string primaryColumnName = reader.GetString(1);
					string keyName = reader.GetString(2);
					string primaryTableName = reader.GetString(3);
					string foreignColumnName = reader.GetString(4);
					if(fk == null || !string.Equals(fk.Name, keyName))
						fk = table.ForeignKeys.FirstOrDefault(k => string.Equals(k.Name, keyName));
					if(fk == null) {
						fk = new DBForeignKey {
							Name = keyName, PrimaryKeyTable = primaryTableName
						};
						table.ForeignKeys.Add(fk);
					}
					fk.Columns.Add(primaryColumnName);
					fk.PrimaryKeyTableKeyColumns.Add(foreignColumnName);
				}
			});
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
	public class DataAccessMSSqlCEProviderFactory : MSSqlCEProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessMSSqlCEConnectionProvider.DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessMSSqlCEConnectionProvider.DataAccessCreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
	}
}
