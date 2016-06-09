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
	public class DataAccessAseConnectionProvider : AseConnectionProvider, IAliasFormatter, IDataStoreSchemaExplorerEx, IDataStoreEx, ISupportStoredProc {
		#region static
		static DBColumnType GetColumnType(byte type, byte prec, int length) {
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
				case 48: 
					return DBColumnType.Byte;
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
				case 35:
				case 39:
					return DBColumnType.String;
				case 174:
				case 155:
					return DBColumnType.String;
				case 34:
				case 45:
					return DBColumnType.ByteArray;
				case 49: 
				case 123: 
				case 189: 
				case 187: 
				case 58: 
				case 61:  
				case 111: 
				case 37: 
					return DBColumnType.DateTime;
				case 109:
					return DBColumnType.Double;
				case 110:
				case 60:
				case 55:
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
		static int GetColumnLength(byte type, int length, short userType, byte charsize) {
			switch(type) {
				case 35:
				case 39:
					if(userType == 2)
						return length;
					return length/charsize;
				case 174:
				case 155:
					return length/2;
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
			return new DataAccessAseConnectionProvider(connection, autoCreateOption);
		}
		static DataAccessAseConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterDataStoreProvider("AseConnection", DataAccessCreateProviderFromConnection);
			RegisterFactory(new DataAccessAseProviderFactory());
		}
		public static void ProviderRegister() {
		}
		IDataStoreEx DataStoreEx { get { return this; } }
		public DataAccessAseConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
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
		public override string FormatTable(string schema, string tableName) {
			return !string.IsNullOrWhiteSpace(schema) 
				? string.Format(CultureInfo.InvariantCulture, "[{0}.{1}]", schema, tableName) 
				: string.Format(CultureInfo.InvariantCulture, "[{0}]", tableName);
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			if(tableName == SubSelectStatement.CustomSqlString)
				return tableAlias;
			string encloseAlias = ConnectionProviderHelper.EncloseAlias(tableAlias, aliasLead, aliasEnd);
			string tableNameWithSchema = FormatTable(schema, tableName);
			return string.IsNullOrEmpty(encloseAlias) ? tableNameWithSchema : string.Format("{0} {1}", tableNameWithSchema, encloseAlias);
		}
		public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.AddDays:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(dd, {1}, {0})", operands[0], operands[1]);
				default:
					return base.FormatFunction(operatorType, operands);
			}
		}
		#region IAliasFormatter
		string IAliasFormatter.AliasLead { get { return aliasLead; } }
		string IAliasFormatter.AliasEnd { get { return aliasEnd; } }
		bool IAliasFormatter.SingleQuotedString { get { return singleQuotedString; } }
		int IAliasFormatter.MaxTableAliasLength { get { return GetSafeNameTableMaxLength(); } }
		int IAliasFormatter.MaxColumnAliasLength { get { return GetSafeNameColumnMaxLength(); } }
		#endregion
		#region IDataStoreSchemaExplorerEx
		string[] IDataStoreSchemaExplorerEx.GetStorageDataObjectsNames(DataObjectTypes types) {
			List<string> result = new List<string>();
			string typeCondition = ConnectionProviderHelper.GetDataObjectTypeCondition(types, "type = 'U'", "(type = 'V' and name != 'sysquerymetrics')");
			string getTablesListQuery = string.Format(@"select case when loginame <> null then loginame || '.' || name else name end as table_name
                                                                    from sysobjects
                                                                    where {0}", typeCondition);
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
		static string GetTablesFilter(ICollection<DBTable> tables, string prefix, bool useTablesFilter) {
			return (useTablesFilter && tables.Count != 0) ? string.Format("{0} in ({1})", prefix, string.Join(", ", tables.Select(n => string.Format("'{0}'", n.Name)))) : string.Empty;
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
		void GetStorageTablesColumns(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.Columns.Clear();
			string getTablesColumnsQuery = string.Format(@"select full_table_name, table_name, column_type, column_prec, column_length, column_usertype, char_size, column_status
                                                            from (select case when t.loginame <> null then t.loginame || '.' || t.name else t.name end as full_table_name,
                                                                         c.name as table_name, c.type as column_type, c.prec as column_prec, c.length as column_length,
                                                                         c.usertype as column_usertype, @@ncharsize as char_size, c.status as column_status
                                                                  from syscolumns c left join sysobjects t on c.id = t.id) table_columns {0}",
				GetTablesFilter(tables, "where full_table_name", useTablesFilter));
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesColumnsQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string tableName = reader.GetString(0);
					if(table == null || !string.Equals(table.Name, tableName))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, tableName));
					if(table == null)
						continue;
					string columnName = reader.GetString(1);
					byte typeCode = reader.GetByte(2);
					byte prec = reader.IsDBNull(3) ? (byte)0 : reader.GetByte(3);
					int length = reader.GetInt32(4);
					int len = GetColumnLength(typeCode, length, reader.GetInt16(5), reader.GetByte(6));
					DBColumnType type = GetColumnType(typeCode, prec, length);
					bool isIdentity = (Convert.ToInt32(reader.GetValue(7)) & 128) == 128;
					DBColumn column = ConnectionProviderHelper.CreateColumn(columnName, false, isIdentity, len, type);
					table.AddColumn(column);
				}
			});
		}
		void GetStorageTablesForeignKeys(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.ForeignKeys.Clear();
			string getTablesForeignKeysQuery = string.Format(@"select full_table_name, full_primary_table_name, column_name, primary_column_name,
                                                                      keycnt, constrid, key_name
                                                               from (select case when o.loginame <> null then o.loginame || '.' || o.name else o.name end as full_table_name,
                                                                            case when r.loginame <> null then r.loginame || '.' || r.name else r.name end as full_primary_table_name,
                                                                            o.name as table_name, r.name as primary_table_name, fc.name as column_name, pc.name as primary_column_name,
                                                                            f.keycnt, f.constrid, rc.name as key_name
                                                                     from sysreferences f
                                                                      join sysobjects o on o.id = f.tableid
                                                                      join sysobjects r on r.id = f.reftabid
                                                                      join sysobjects rc on rc.id = f.constrid
                                                                      join syscolumns fc on f.fokey1 = fc.colid and fc.id = o.id
                                                                      join syscolumns pc on f.refkey1 = pc.colid and pc.id = r.id) table_foreign_keys {0}
                                                               order by key_name, column_name",
				GetTablesFilter(tables, "where full_table_name", useTablesFilter));
			Dictionary<DBForeignKey, ComplexKeyInfo> complexKeysMapping = new Dictionary<DBForeignKey, ComplexKeyInfo>();
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesForeignKeysQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string tableName = reader.GetString(0);
					if(table == null || !string.Equals(table.Name, tableName))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, tableName));
					if(table == null)
						continue;
					string primaryKeyTable = reader.GetString(1);
					string primaryColumn = reader.GetString(2);
					string foreignColumn = reader.GetString(3);
					short refcount = reader.GetInt16(4);
					int constrid = reader.GetInt32(5);
					DBForeignKey foreignKey = new DBForeignKey {
						PrimaryKeyTable = primaryKeyTable,
						Name = reader.GetString(6)
					};
					foreignKey.Columns.Add(foreignColumn);
					foreignKey.PrimaryKeyTableKeyColumns.Add(primaryColumn);
					if(refcount > 1)
						complexKeysMapping.Add(foreignKey, new ComplexKeyInfo(constrid, refcount));
					table.ForeignKeys.Add(foreignKey);
				}
			});
			GetStorageTablesComplexForeignKeys(complexKeysMapping);
		}
		void GetStorageTablesComplexForeignKeys(Dictionary<DBForeignKey, ComplexKeyInfo> complexKeysMapping) {
			foreach(KeyValuePair<DBForeignKey, ComplexKeyInfo> info in complexKeysMapping) {
				for(short refnum = 2; refnum <= info.Value.RefCount; refnum++) {
					Query getTablesIndexColumn = new Query(string.Format(@"select fc.name, pc.name
                                                                 from sysreferences f
                                                                  join syscolumns fc on f.fokey{0} = fc.colid and fc.id = f.tableid
                                                                  join syscolumns pc on f.refkey{0} = pc.colid and pc.id = f.reftabid
                                                                 where f.constrid = {1}", refnum, info.Value.ConstraintID));
					DBForeignKey foreignKey = info.Key;
					DataStoreEx.ProcessQuery(CancellationToken.None, getTablesIndexColumn, (reader, cancellationToken)=> {
						while(reader.Read()) {
							foreignKey.PrimaryKeyTableKeyColumns.Add(reader.GetString(0));
							foreignKey.Columns.Add(reader.GetString(1));
						}
					});
				}
			}
		}
		struct ComplexKeyInfo {
			public int ConstraintID { get; set; }
			public int RefCount { get; set; }
			public ComplexKeyInfo(int constraintID, int refCount) : this() {
				ConstraintID = constraintID;
				RefCount = refCount;
			}
		}
		#endregion
		#region ISupportStoredProc
		DBStoredProcedure[] ISupportStoredProc.GetStoredProcedures(params string[] procedureNames) {
			List<DBStoredProcedure> procedures = new List<DBStoredProcedure>();
			if(procedureNames.Length == 0) {
				const string getProcedureNamesQuery = "select * from sysobjects where type = 'P'";
				DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getProcedureNamesQuery), (reader, cancellationToken)=> {
					while(reader.Read()) {
						string name = reader.GetString(0);
						procedures.Add(ConnectionProviderHelper.CreateDBStoredProcedure(name));
					}
				});
			} else
				procedures.AddRange(procedureNames.Select(name => ConnectionProviderHelper.CreateDBStoredProcedure(name)));;
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
				string fullProcedureName = FormatTable(ComposeSafeSchemaName(procedureName), ComposeSafeTableName(procedureName));
				PrepareCommandForGetStoredProcParameters(command, procedureName);
				List<string> fakeParams = new List<string>();
				for(int index = 0; index < command.Parameters.Count; index++)
					fakeParams.Add("null");
				command.CommandType = CommandType.Text;
				command.CommandText = string.Format("set showplan on " +
													"set fmtonly on " +
													"exec {0} {1} " +
													"set fmtonly off " +
													"set showplan off", fullProcedureName, string.Join(", ", fakeParams.ToArray()));
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
					PrepareCommandForExecuteStoredProc(command, sprocName, parameters);
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
					PrepareCommandForExecuteStoredProc(command, sprocName, parameters);
					return DataStoreExHelper.GetData(command, cancellationToken);
				}
			}
		}
		void PrepareCommandForExecuteStoredProc(IDbCommand command, string sprocName, OperandValue[] parameters) {
			PrepareCommandForGetStoredProcParameters(command, sprocName);
			IDataParameter returnParameter;
			List<IDataParameter> outParameters;
			PrepareParametersForExecuteSproc(parameters, command, out outParameters, out returnParameter);
		}
		void PrepareCommandForGetStoredProcParameters(IDbCommand command, string sprocName) {
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = FormatTable(ComposeSafeSchemaName(sprocName), ComposeSafeTableName(sprocName));
			CommandBuilderDeriveParameters(command);
		}
		#endregion
	}
	public class DataAccessAseProviderFactory : AseProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessAseConnectionProvider.DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessAseConnectionProvider.DataAccessCreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
	}
}
