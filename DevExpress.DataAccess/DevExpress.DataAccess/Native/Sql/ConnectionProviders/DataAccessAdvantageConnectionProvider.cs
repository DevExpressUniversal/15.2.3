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
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using DevExpress.CodeParser;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.DataAccess.Native.Sql.ConnectionProviders {
	public class DataAccessAdvantageConnectionProvider : AdvantageConnectionProvider, IAliasFormatter, ISupportStoredProc, ISupportOrderByExpressionAlias, IDataStoreEx, IDataStoreSchemaExplorerEx {
		#region Static
		static DBColumnType GetTypeFromCode(int code) {
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
				case 23: 
				case 26: 
				case 27: 
					return DBColumnType.String;
				case 28: 
					return DBColumnType.String;
				case 21:
					return DBColumnType.Int32;
				case 22:
					return DBColumnType.DateTime;
			}
			return DBColumnType.Unknown;
		}
		#endregion
		const string aliasLead = "\"";
		const string aliasEnd = "\"";
		const bool singleQuotedString = true;
		public static string GetConnectionString(string database, string username, string password, string serverType) {
			StringBuilder builder = new StringBuilder();
			builder.Append(string.Format("{0}={1};", DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString));
			builder.Append(string.Format("Data source={0};", database));
			if(username != null) {
				builder.Append(string.Format("User id={0};", username));
				if(password != null)
					builder.Append(string.Format("Password={0};", password));
			} else
				builder.Append("User id=ADSSYS;");
			if(serverType != null)
				builder.Append(string.Format("servertype={0};", serverType));
			else
				builder.Append("servertype=local;");
			builder.Append("TrimTrailingSpaces=true");
			return builder.ToString();
		}
		[SuppressMessage("Microsoft.Design", "CA1021")]
		public static IDataStore DataAccessCreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection};
			return DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore DataAccessCreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DataAccessAdvantageConnectionProvider(connection, autoCreateOption);
		}
		static DataAccessAdvantageConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterDataStoreProvider("Advantage.Data.Provider.AdsConnection", DataAccessCreateProviderFromConnection);
			RegisterFactory(new DataAccessAdvantageProviderFactory());
		}
		public static void ProviderRegister() {
		}
		IDataStoreEx DataStoreEx { get { return this; } }
		public DataAccessAdvantageConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			bool shouldCreateParameter = createParameter;
			OperandParameter param = parameter as OperandParameter;
			if(!ReferenceEquals(param, null)) {
				createParameter = true;
				return string.Format(":{0}", param.ParameterName);
			}
			object value = parameter.Value;
			if(value != null) {
				createParameter = false;
				switch(Type.GetTypeCode(value.GetType())) {
					case TypeCode.Decimal:
						return ((decimal)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.DateTime:
						return string.Format("TIMESTAMP'{0}'", ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
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
		#region IDataStoreSchemaExplorerEx
		string[] IDataStoreSchemaExplorerEx.GetStorageDataObjectsNames(DataObjectTypes types) {
			List<string> result = new List<string>();
			string getTablesListQuery = string.Empty;
			if((types & DataObjectTypes.Tables) != 0)
				getTablesListQuery += "select name from system.tables";
			if((types & DataObjectTypes.Tables) != 0 && (types & DataObjectTypes.Views) != 0)
				getTablesListQuery += " union ";
			if((types & DataObjectTypes.Views) != 0)
				getTablesListQuery += "select name from system.views";
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesListQuery), (reader, cancellationToken)=> {
				while(reader.Read()) {
					result.Add(reader.GetString(0).TrimEnd());
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
			foreach(DBTable table in tables) {
				table.Columns.Clear();
				if(table.IsView)
					ConnectionProviderHelper.GetColumnsByReader(Connection, table, FormatTable(string.Empty, ComposeSafeTableName(table.Name)));
			}
			string getTablesColumnsQuery = string.Format(@"select Parent, Name, Field_Type, Field_Length, Field_Num from system.columns {0}"
				, GetTablesFilter(tables, "where Parent", useTablesFilter));
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesColumnsQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string tableName = reader.GetString(0);
					string columnName = reader.GetString(1);
					if(table == null || !string.Equals(table.Name, tableName))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, tableName));
					if(table == null)
						continue;
					DBColumnType columnType = GetTypeFromCode(Convert.ToInt32(reader.GetValue(2)));
					int size = (columnType == DBColumnType.String) ? Convert.ToInt16(reader.GetValue(3)) - 2 : 0;
					DBColumn column = new DBColumn(columnName == null ? string.Empty : columnName.TrimEnd(), false, string.Empty, size, columnType);
					table.AddColumn(column);
				}
			});
		}
		void GetStorageTablesForeignKeys(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.ForeignKeys.Clear();
			string getTablesForeignKeysQuery = string.Format(@"select r.RI_Primary_Table, r.RI_Foreign_Table, i.Index_Expression, ip.Index_Expression, r.Name
                                                               from system.relations r
				                                                 inner join system.indexes i on r.RI_Foreign_Index = i.Name and r.RI_Foreign_Table = i.Parent
				                                                 inner join system.indexes ip on r.RI_Primary_Index = ip.Name and r.RI_Primary_Table = ip.Parent {0}"
				, GetTablesFilter(tables, "where r.RI_Foreign_Table", useTablesFilter));
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesForeignKeysQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string primaryTableName = reader.GetString(0);
					string foreignTableName = reader.GetString(1);
					if(table == null || !string.Equals(table.Name, foreignTableName))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, foreignTableName));
					if(table == null)
						continue;
					string foreignColumns = reader.GetString(2);
					string primaryColumns = reader.GetString(3);
					string keyName = reader.GetString(4).TrimEnd();
					if(primaryColumns == null || foreignColumns == null)
						continue;
					StringCollection fkc = new StringCollection();
					fkc.AddRange(foreignColumns.TrimEnd().Split(';'));
					StringCollection pkc = new StringCollection();
					pkc.AddRange(primaryColumns.TrimEnd().Split(';'));
					DBForeignKey fk = new DBForeignKey(fkc, primaryTableName, pkc) {
						Name = keyName.TrimEnd()
					};
					table.ForeignKeys.Add(fk);
				}
			});
		}
		#endregion
		#region ISupportStoredProc Members
		DBStoredProcedure[] ISupportStoredProc.GetStoredProcedures(params string[] procedureNames) {
			List<DBStoredProcedure> procedures = new List<DBStoredProcedure>();
			if(procedureNames.Length == 0) {
				DataStoreEx.ProcessQuery(CancellationToken.None, new Query("select name from system.storedprocedures"), (reader, cancellationToken)=> {
					while(reader.Read()) {
						string name = reader.GetString(0);
						procedures.Add(ConnectionProviderHelper.CreateDBStoredProcedure(name));
					}
				});
			} else
				procedures.AddRange(procedureNames.Select(name => ConnectionProviderHelper.CreateDBStoredProcedure(name)));
			using(IDbCommand command = Connection.CreateCommand()) {
				foreach(DBStoredProcedure procedure in procedures) {
					PrepareCommandForGetStoredProcParameters(command, procedure.Name);
					procedure.Arguments.AddRange(ConnectionProviderHelper.GetStoredProcedureArgumentsFromCommand(command));
				}
			}
			return procedures.ToArray();
		}
		DBStoredProcedureResultSet ISupportStoredProc.GetStoredProcedureTableSchema(string procedureName) {
			using(IDbCommand command = CreateCommand()) {
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
			command.CommandText = ComposeSafeTableName(sprocName);
			CommandBuilderDeriveParameters(command);
		}
		#endregion
	}
	public class DataAccessAdvantageProviderFactory : AdvantageProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessAdvantageConnectionProvider.DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessAdvantageConnectionProvider.DataAccessCreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(DatabaseParamID))
				return null;
			string username = parameters.ContainsKey(UserIDParamID) ? parameters[UserIDParamID] ?? string.Empty : null;
			string password = parameters.ContainsKey(PasswordParamID) ? parameters[PasswordParamID] ?? string.Empty : null;
			string servertype = parameters.ContainsKey(DataAccessConnectionParameter.AdvantageServerTypeParamID) ? parameters[DataAccessConnectionParameter.AdvantageServerTypeParamID] : null;
			return DataAccessAdvantageConnectionProvider.GetConnectionString(parameters[DatabaseParamID], username, password, servertype); 
		}
	}
}
