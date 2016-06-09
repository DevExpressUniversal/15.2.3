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
	public class DataAccessFirebirdConnectionProvider : FirebirdConnectionProvider, IAliasFormatter, IDataStoreSchemaExplorerEx, ISupportStoredProc, IDataStoreEx {
		#region static
		static DBColumnType GetColumnType(short type, short subType, short scale, short size) {
			switch(type) {
				case 7:
					if(subType == 2)
						return DBColumnType.Decimal;
					if(subType == 1)
						return DBColumnType.Decimal;
					return scale < 0 ? DBColumnType.Decimal : DBColumnType.Int16;
				case 8:
					if(subType == 2)
						return DBColumnType.Decimal;
					if(subType == 1)
						return DBColumnType.Decimal;
					return scale < 0 ? DBColumnType.Decimal : DBColumnType.Int32;
				case 9:
				case 0x10:
				case 0x2d:
					if(subType == 2)
						return DBColumnType.Decimal;
					if(subType == 1)
						return DBColumnType.Decimal;
					return scale < 0 ? DBColumnType.Decimal : DBColumnType.Int64;
				case 10:
					return DBColumnType.Single;
				case 11:
				case 0x1b:
					return DBColumnType.Double;
				case 14:
				case 15:
					return size == 1 ? DBColumnType.Char : DBColumnType.String;
				case 0x0c: 
				case 0x23: 
					return DBColumnType.DateTime;
				case 0x25:
				case 0x26:
					return DBColumnType.String;
				case 40:
				case 0x29:
					return DBColumnType.String;
				case 0x105:
					return subType == 1 ? DBColumnType.String : DBColumnType.ByteArray;
			}
			return DBColumnType.Unknown;
		}
		#endregion
		const string aliasLead = "\"";
		const string aliasEnd = "\"";
		const bool singleQuotedString = true;
		[SuppressMessage("Microsoft.Design", "CA1021")]
		public static IDataStore DataAccessCreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection};
			return DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore DataAccessCreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DataAccessFirebirdConnectionProvider(connection, autoCreateOption);
		}
		static DataAccessFirebirdConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterDataStoreProvider("FbConnection", DataAccessCreateProviderFromConnection);
			RegisterFactory(new DataAccessFirebirdProviderFactory());
		}
		public static void ProviderRegister() {
		}
		IDataStoreEx DataStoreEx { get { return this; } }
		public DataAccessFirebirdConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
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
						return string.Format("CAST('{0}' AS TIMESTAMP)", ((DateTime)value).ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
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
		public override string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			switch(operatorType) {
				case BinaryOperatorType.Modulo:
					return string.Format(CultureInfo.InvariantCulture, "mod({0}, {1})", leftOperand, rightOperand);
				default:
					return base.FormatBinary(operatorType, leftOperand, rightOperand);
			}
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
			List<string> result = new List<string>();
			const string getTablesListQuery = "select RDB$RELATION_NAME, RDB$VIEW_BLR from RDB$RELATIONS where RDB$SYSTEM_FLAG = 0";
			bool needTables = (types & DataObjectTypes.Tables) != 0;
			bool needViews = (types & DataObjectTypes.Views) != 0;
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesListQuery), (reader, cancellationToken)=> {
				while(reader.Read()) {
					string name = reader.GetString(0).TrimEnd();
					bool isView = !reader.IsDBNull(1) && reader.GetValue(1) != null;
					if((!isView && needTables) || (isView && needViews))
						result.Add(name);
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
		static string GetTablesFilter(ICollection<DBTable> tables, string prefix) {
			return tables.Count != 0 ? string.Format("{0} in ({1})", prefix, string.Join(", ", tables.Select(n => string.Format("'{0}'", n.Name.ToUpperInvariant())))) : string.Empty;
		}
		void GetStorageTablesColumns(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.Columns.Clear();
			string getTablesColumnsQuery = @"select r.RDB$RELATION_NAME as table_name, r.RDB$FIELD_NAME as column_name,
                                                    f.RDB$FIELD_TYPE as column_type, f.RDB$FIELD_SUB_TYPE as column_subtype, f.RDB$FIELD_SCALE as column_scale,
                                                    RDB$CHARACTER_LENGTH as chr_length, RDB$FIELD_POSITION as column_position
                                             from RDB$RELATION_FIELDS r
                                              join RDB$FIELDS f on r.RDB$FIELD_SOURCE = f.RDB$FIELD_NAME
                                             where r.RDB$SYSTEM_FLAG = 0";
			if(useTablesFilter)
				getTablesColumnsQuery = string.Format("{0} {1}", getTablesColumnsQuery, GetTablesFilter(tables, "and UPPER(r.RDB$RELATION_NAME)"));
			getTablesColumnsQuery = string.Format("{0} order by r.RDB$RELATION_NAME, RDB$FIELD_POSITION", getTablesColumnsQuery);
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesColumnsQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string tableName = reader.GetString(0).TrimEnd();
					if(table == null || !string.Equals(table.Name, tableName))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, tableName));
					if(table == null)
						continue;
					string columnName = reader.GetString(1).TrimEnd();
					short type = reader.IsDBNull(2) ? (short)0 : reader.GetInt16(2);
					short subtype = reader.IsDBNull(3) ? (short)0 : reader.GetInt16(3);
					short scale = reader.IsDBNull(4) ? (short)0 : reader.GetInt16(4);
					short size = reader.IsDBNull(5) ? (short)0 : reader.GetInt16(5);
					DBColumnType columnType = GetColumnType(type, subtype, scale, size);
					DBColumn column = ConnectionProviderHelper.CreateColumn(columnName, false, false, columnType == DBColumnType.String ? size : 0, columnType);
					table.AddColumn(column);
				}
			});
		}
		void GetStorageTablesForeignKeys(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.ForeignKeys.Clear();
			string getTablesForeignKeysQuery = @"select i.RDB$RELATION_NAME as table_name, p.RDB$FIELD_NAME as column_name, i.RDB$INDEX_NAME as index_name,
                                                        pt.RDB$RELATION_NAME as primary_name, f.RDB$FIELD_NAME as primary_column, f.RDB$FIELD_POSITION as column_position
                                                 from RDB$INDICES i
                                                  join RDB$INDICES pt on i.RDB$FOREIGN_KEY = pt.RDB$INDEX_NAME
                                                  join RDB$INDEX_SEGMENTS f on f.RDB$INDEX_NAME = i.RDB$INDEX_NAME
                                                  join RDB$INDEX_SEGMENTS p on p.RDB$INDEX_NAME = i.RDB$FOREIGN_KEY and p.RDB$FIELD_POSITION = f.RDB$FIELD_POSITION
                                                 where  i.RDB$FOREIGN_KEY is NOT NULL";
			if(useTablesFilter)
				getTablesForeignKeysQuery = string.Format("{0} {1}", getTablesForeignKeysQuery, GetTablesFilter(tables, "and UPPER(i.RDB$RELATION_NAME)"));
			getTablesForeignKeysQuery = string.Format("{0} order by i.RDB$INDEX_NAME, f.RDB$FIELD_POSITION", getTablesForeignKeysQuery);
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesForeignKeysQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				DBForeignKey fk = null;
				while(reader.Read()) {
					string foreignTableName = reader.GetString(0).TrimEnd();
					if(table == null || !string.Equals(table.Name, foreignTableName)) {
						table = tables.FirstOrDefault(t => string.Equals(t.Name, foreignTableName));
						fk = null;
					}
					if(table == null)
						continue;
					string primaryColumnName = reader.GetString(1).TrimEnd();
					string keyName = reader.GetString(2).TrimEnd();
					string primaryTableName = reader.GetString(3).TrimEnd();
					string foreignColumnName = reader.GetString(4).TrimEnd();
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
		#region ISupportStoredProc
		DBStoredProcedure[] ISupportStoredProc.GetStoredProcedures(params string[] procedureNames) {
			List<DBStoredProcedure> procedures = new List<DBStoredProcedure>();
			if(procedureNames.Length == 0) {
				const string getProcedureNamesQuery = "SELECT rdb$procedure_name FROM rdb$procedures ORDER BY rdb$procedure_name";
				DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getProcedureNamesQuery), (reader, cancellationToken)=> {
					while(reader.Read()) {
						string name = reader.GetString(0).TrimEnd();
						procedures.Add(ConnectionProviderHelper.CreateDBStoredProcedure(name));
					}
				});
			} else
				procedures.AddRange(procedureNames.Select(name => ConnectionProviderHelper.CreateDBStoredProcedure(name)));
			IDbCommand command = Connection.CreateCommand();
			foreach(DBStoredProcedure procedure in procedures) {
				try {
					PrepareCommandForGetStoredProcParameters(command, procedure.Name);
					procedure.Arguments.AddRange(ConnectionProviderHelper.GetStoredProcedureArgumentsFromCommand(command));
				} catch {
				}
			}
			return procedures.ToArray();
		}
		DBStoredProcedureResultSet ISupportStoredProc.GetStoredProcedureTableSchema(string procedureName) {
			using(IDbCommand command = Connection.CreateCommand()) {
				PrepareCommandForGetStoredProcParameters(command, procedureName);
				return ConnectionProviderHelper.GetResultSetFromCommand(command);
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
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand()) {
					PrepareCommandForStoredProc(command, sprocName, parameters);
					using(IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess)) {
						action(reader, cancellationToken);
					}
				}
			}
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
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand()) {
					PrepareCommandForStoredProc(command, sprocName, parameters);
					return DataStoreExHelper.GetData(command, cancellationToken);
				}
			}
		}
		void PrepareCommandForStoredProc(IDbCommand command, string sprocName, OperandValue[] parameters) {
			PrepareCommandForGetStoredProcParameters(command, sprocName);
			IDataParameter returnParameter;
			List<IDataParameter> outParameters;
			PrepareParametersForExecuteSproc(parameters, command, out outParameters, out returnParameter);
		}
		void PrepareCommandForGetStoredProcParameters(IDbCommand command, string procedureName) {
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = FormatTable(ComposeSafeSchemaName(procedureName), ComposeSafeTableName(procedureName));
			CommandBuilderDeriveParameters(command);
		}
		#endregion
	}
	public class DataAccessFirebirdProviderFactory : FirebirdProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessFirebirdConnectionProvider.DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessFirebirdConnectionProvider.DataAccessCreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
	}
}
