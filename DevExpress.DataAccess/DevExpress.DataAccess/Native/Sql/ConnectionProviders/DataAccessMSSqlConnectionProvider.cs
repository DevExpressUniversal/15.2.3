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
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Utils;
#if DXRESTRICTED
using IDbTransaction = System.Data.Common.DbTransaction;
using IDataReader = System.Data.Common.DbDataReader;
using IDbConnection = System.Data.Common.DbConnection;
using IDbCommand = System.Data.Common.DbCommand;
using IDataParameter = System.Data.Common.DbParameter;
using IDbDataParameter = System.Data.Common.DbParameter;
#endif
namespace DevExpress.DataAccess.Native.Sql.ConnectionProviders {
	public class DataAccessMSSqlConnectionProvider : MSSqlConnectionProvider, IAliasFormatter, IDataStoreSchemaExplorerEx, ISupportStoredProc, IDataStoreEx {
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
			IDbConnection connection = new SqlConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection};
			return DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore DataAccessCreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DataAccessMSSqlConnectionProvider(connection, autoCreateOption);
		}
		public static void ProviderRegister() {
		}
		IDataStoreEx DataStoreEx { get { return this; } }
		static DataAccessMSSqlConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterDataStoreProvider("System.Data.SqlClient.SqlConnection", DataAccessCreateProviderFromConnection);
			RegisterFactory(new DataAccessMSSqlProviderFactory());
		}
		public DataAccessMSSqlConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
			CheckIs2005(Connection);
			if(Connection.State == ConnectionState.Open)
				Connection.Close();
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
					case TypeCode.Char:
						return string.Format("'{0}'", value);
					case TypeCode.Decimal:
						return ((decimal)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.DateTime:
						return string.Format("convert(datetime, '{0}', 120)", ((DateTime)value).ToString("yyyy-MM-dd HH':'mm':'ss", CultureInfo.InvariantCulture));
					case TypeCode.Int64:
						return ((long)value).ToString(CultureInfo.InvariantCulture);
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
		public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.AddDays:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(dd, {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.GetDate:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(dd, DATEDIFF(dd, 0, {0}), 0)", operands[0]);
				default:
					return base.FormatFunction(operatorType, operands);
			}
		}
		public override string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.StartsWith:
					if(operands[1] is OperandParameter)
						return string.Format(CultureInfo.InvariantCulture, "(Left({0}, Len({1})) = ({1}))",
							processParameter(operands[0]), processParameter(operands[1]));
					break;
			}
			return base.FormatFunction(processParameter, operatorType, operands);
		}
		bool is2005;
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		void CheckIs2005(IDbConnection conn) {
			SqlConnection sqlConnection = conn as SqlConnection;
			int majorVersion;
			if(sqlConnection != null)
				majorVersion = Convert.ToInt32(sqlConnection.ServerVersion.Split('.')[0]);
			else {
				Query query = new Query("select @@MICROSOFTVERSION / 0x1000000");
				using(IDbCommand getVersion = CreateCommand(query)) {
					majorVersion = Convert.ToInt32(getVersion.ExecuteScalar());
				}
			}
			this.is2005 = majorVersion > 8;
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
			string dataObjectTypeCondition = ConnectionProviderHelper.GetDataObjectTypeCondition(types, "objects.type = 'U'", "objects.type = 'V'");
			string getTablesListQuery = this.is2005
				? string.Format(@"select case when schemas.name <> '{0}' then schemas.name + '.' ELSE '' END + objects.name as full_table_name, objects.type
                                  from sys.objects objects
                                    join sys.schemas schemas on schemas.schema_id = objects.schema_id
                                  where {1}", this.ObjectsOwner, dataObjectTypeCondition)
				: string.Format(@"select case when schemas.name <> '{0}' then schemas.name + '.' ELSE '' END + objects.name as full_table_name, objects.type
                                  from sysobjects objects
                                    join sysusers schemas on schemas.uid = objects.uid
                                  where ({1}) and objectProperty(id, 'IsMSShipped') = 0", this.ObjectsOwner, dataObjectTypeCondition);
			List<string> result = new List<string>();
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
			List<DBTable> tables = filteredTables.Select(p => ConnectionProviderHelper.CreateTable(p, isView)).ToList();
			if(includeColumns)
				GetStorageTablesColumns(tables, tablesList.Length != 0);
			GetStorageTablesForeignKeys(tables, tablesList.Length != 0);
			return tables.ToArray();
		}
		static string GetTablesFilter(ICollection<DBTable> tables, string prefix) {
			return tables.Count != 0 ? string.Format("{0} in ({1})", prefix, string.Join(", ", tables.Select(n => string.Format("'{0}'", n.Name)))) : string.Empty;
		}
		void GetStorageTablesColumns(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.Columns.Clear();
			string getTablesColumnsQuery = this.is2005
				? string.Format(@"select case when schemas.name <> '{0}' then schemas.name + '.' ELSE '' END + objects.name as full_table_name,
                                         columns.COLUMN_NAME, columns.DATA_TYPE, columns.CHARACTER_MAXIMUM_LENGTH, columns.ORDINAL_POSITION
                                  from sys.objects objects
                                    join sys.schemas schemas on schemas.schema_id = objects.schema_id
                                    join INFORMATION_SCHEMA.COLUMNS columns on columns.TABLE_NAME = objects.name and columns.TABLE_SCHEMA = schemas.name
                                  where objects.type = 'U' or objects.type = 'V'", this.ObjectsOwner)
				: string.Format(@"select case when schemas.name <> '{0}' then schemas.name + '.' ELSE '' END + objects.name as full_table_name,
                                         columns.COLUMN_NAME, columns.DATA_TYPE, columns.CHARACTER_MAXIMUM_LENGTH, columns.ORDINAL_POSITION
                                  from sysobjects objects
                                    join sysusers schemas on schemas.uid = objects.uid
                                    join INFORMATION_SCHEMA.COLUMNS columns on columns.TABLE_NAME = objects.name
                                  where (objects.type = 'U' or objects.type = 'V') and objectProperty(objects.id, 'IsMSShipped') = 0", this.ObjectsOwner);
			getTablesColumnsQuery = useTablesFilter
				? string.Format("select * from ({0}) table_columns {1} order by ORDINAL_POSITION", getTablesColumnsQuery, GetTablesFilter(tables, "where full_table_name"))
				: string.Format("{0} order by ORDINAL_POSITION", getTablesColumnsQuery);
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesColumnsQuery), (reader, cancellationToken)=> {
				while(reader.Read()) {
					string tableName = reader.GetString(0);
					string columnName = reader.GetString(1);
					DBTable table = tables.FirstOrDefault(t => string.Equals(t.Name, tableName));
					if(table == null)
						continue;
					string typeName = reader.GetString(2);
					object sizeObject = reader.GetValue(3);
					int size = (sizeObject != null && sizeObject != DBNull.Value) ? ((IConvertible)sizeObject).ToInt32(CultureInfo.InvariantCulture) : 0;
					DBColumnType type = GetColumnType(typeName, size);
					table.AddColumn(ConnectionProviderHelper.CreateColumn(columnName, false, false, type == DBColumnType.String ? size : 0, type));
				}
			});
		}
		void GetStorageTablesForeignKeys(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.ForeignKeys.Clear();
			string getTablesForeignKeysQuery = this.is2005
				? string.Format(@"select case when fks.name <> '{0}' then fks.name + '.' ELSE '' END + fkt.name as full_table_name,
                                         fk.name as key_column, c.name as key_name,
                                         case when pks.name <> '{0}' then pks.name + '.' ELSE '' END + pkt.name as primary_table_name,
                                         pk.name as primary_column_name, r.constraint_column_id as column_id
                                  from sys.foreign_key_columns r
                                    inner join sys.foreign_keys c on r.constraint_object_id = c.object_id
                                    inner join sys.columns fk on r.parent_object_id = fk.object_id and r.parent_column_id = fk.column_id
                                    inner join sys.objects fkt on r.parent_object_id = fkt.object_id
                                    inner join sys.schemas fks on fks.schema_id = fkt.schema_id
                                    inner join sys.columns pk on r.referenced_object_id = pk.object_id and r.referenced_column_id = pk.column_id
                                    inner join sys.objects pkt on r.referenced_object_id = pkt.object_id
                                    inner join sys.schemas pks on pks.schema_id = pkt.schema_id", this.ObjectsOwner)
				: string.Format(@"select case when u.name <> '{0}' then u.name + '.' ELSE '' END + tbl.name as full_table_name,
                                         c.name as key_column, fk.name as key_name,
                                         case when ru.name <> '{0}' then ru.name + '.' ELSE '' END + rtbl.name as primary_table_name,
                                         rc.name primary_column_name, fkdata.keyno as column_id
                                  from sysforeignkeys fkdata
                                    join sysobjects fk on fkdata.constid = fk.id
                                    join sysobjects rtbl on rtbl.id = fkdata.rkeyid
                                    join sysobjects tbl on tbl.id = fkdata.fkeyid
                                    join syscolumns c on c.id = fkdata.fkeyid and c.colid = fkdata.fkey
                                    join syscolumns rc on rc.id = fkdata.rkeyid and rc.colid = fkdata.rkey
                                    join sysusers u on u.uid = tbl.uid
                                    join sysusers ru on ru.uid = rtbl.uid", this.ObjectsOwner);
			if(useTablesFilter)
				getTablesForeignKeysQuery = string.Format("select * from ({0}) table_foreign_keys {1} order by key_name, column_id", getTablesForeignKeysQuery, GetTablesFilter(tables, "where full_table_name"));
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesForeignKeysQuery), (reader, cancellationToken)=> {
				while(reader.Read()) {
					string tableName = reader.GetString(0);
					string columnName = reader.GetString(1);
					DBTable table = tables.FirstOrDefault(t => t.Name == tableName);
					if(table == null)
						continue;
					string keyName = reader.GetString(2);
					DBForeignKey fk = table.ForeignKeys.FirstOrDefault(k => k.Name == keyName);
					if(fk == null) {
						fk = new DBForeignKey {
							Name = keyName, PrimaryKeyTable = reader.GetString(3)
						};
						table.ForeignKeys.Add(fk);
					}
					fk.Columns.Add(columnName);
					fk.PrimaryKeyTableKeyColumns.Add(reader.GetString(4));
				}
			});
		}
		#endregion
		#region ISupportStoredProc Members
		DBStoredProcedure[] ISupportStoredProc.GetStoredProcedures(params string[] procedureNames) {
			List<DBStoredProcedure> result = new List<DBStoredProcedure>();
			string getProcedureNamesQuery;
			if(this.is2005)
				getProcedureNamesQuery = string.Format(@"SELECT case when schemas.name <> '{0}' then schemas.name + '.' ELSE '' END + objects.name as full_procedure_name,
                                                  param.name AS [Name], ISNULL(baset.name, N'') AS [SystemType],
                                                  CAST(CASE WHEN baset.name IN (N'char', N'varchar', N'binary', N'varbinary', N'nchar', N'nvarchar') THEN param.prec ELSE param.length END AS int) as [Length],
                                                  CAST(CASE param.isoutparam WHEN 1 THEN param.isoutparam WHEN 0 THEN CASE param.name WHEN '' THEN 1 ELSE 0 END END AS bit) as [IsOutputParameter], colorder
                                           FROM   sys.all_objects objects
                                             JOIN sys.schemas schemas on schemas.schema_id = objects.schema_id
                                             LEFT OUTER JOIN syscolumns AS param on param.id = objects.object_id and param.number = 1
                                             LEFT OUTER JOIN systypes AS baset ON baset.xusertype = param.xtype and baset.xusertype = baset.xtype
                                           WHERE  objects.type = 'P' and objects.is_ms_shipped = 0", this.ObjectsOwner);
			else
				getProcedureNamesQuery = string.Format(@"SELECT case when schemas.name <> '{0}' then schemas.name + '.' ELSE '' END + objects.name as full_procedure_name,
                                                  param.name AS [Name], ISNULL(baset.name, N'') AS [SystemType],
                                                  CAST(CASE WHEN baset.name IN (N'char', N'varchar', N'binary', N'varbinary', N'nchar', N'nvarchar') THEN param.prec ELSE param.length END AS int) as [Length],
                                                  CAST(CASE param.isoutparam WHEN 1 THEN param.isoutparam WHEN 0 THEN CASE param.name WHEN '' THEN 1 ELSE 0 END END AS bit) as [IsOutputParameter], colorder
                                           FROM   sysobjects objects
                                             JOIN sysusers schemas on schemas.uid = objects.uid
                                             LEFT OUTER JOIN syscolumns AS param on param.id = objects.id and param.number = 1
                                             LEFT OUTER JOIN systypes AS baset ON baset.xusertype = param.xtype and baset.xusertype = baset.xtype
                                           WHERE  objects.xtype = N'P' and OBJECTPROPERTY(objects.id, N'IsMSShipped') = 0", this.ObjectsOwner);
			if(procedureNames.Length > 0)
				getProcedureNamesQuery = string.Format("select * from ({0}) as procedures where full_procedure_name in ({1})", getProcedureNamesQuery, string.Join(", ", procedureNames.Select(n => string.Format("'{0}'", n))));
			getProcedureNamesQuery = string.Format("{0} ORDER BY full_procedure_name, colorder", getProcedureNamesQuery);
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getProcedureNamesQuery), (reader, cancellationToken)=> {
				while(reader.Read()) {
					string procedureName = reader.GetString(0);
					DBStoredProcedure procedure = result.FirstOrDefault(p => p.Name == procedureName);
					if(procedure == null) {
						procedure = new DBStoredProcedure {Name = procedureName};
						result.Add(procedure);
					}
					string parameterName = reader.GetValue(1) as string;
					if(parameterName == null)
						continue;
					DBStoredProcedureArgument arg = new DBStoredProcedureArgument {
						Name = parameterName,
						Type = GetColumnType(reader.GetString(2), Convert.ToInt32(reader.GetValue(3))/2),
						Direction = Convert.ToInt32(reader.GetValue(4)) == 1 ? DBStoredProcedureArgumentDirection.Out : DBStoredProcedureArgumentDirection.In
					};
					procedure.Arguments.Add(arg);
				}
			});
			return result.ToArray();
		}
		DBStoredProcedureResultSet ISupportStoredProc.GetStoredProcedureTableSchema(string procedureName) {
			try {
				string fullProcedureName = FormatTable(ComposeSafeSchemaName(procedureName), ComposeSafeTableName(procedureName));
				IEnumerable<string> fakeParamNames;
				using(IDbCommand fakeCommand = CreateCommand()) {
					PrepareCommandForGetStoredProcParameters(fakeCommand, procedureName);
					fakeParamNames = fakeCommand.Parameters.OfType<IDbDataParameter>().Where(p => p.Direction != ParameterDirection.ReturnValue).Select(p => "null");
				}
				Query query = new Query(string.Format("set fmtonly on; exec {0} {1}; set fmtonly off", fullProcedureName, string.Join(", ", fakeParamNames)));
				using(IDbCommand command = CreateCommand(query))
					return ConnectionProviderHelper.GetResultSetFromCommand(command);
			} finally {
				if(Connection.State == ConnectionState.Open)
					Connection.Close();
			}
		}
		#endregion
		#region IDataStoreEx
		void IDataStoreEx.ProcessQuery(CancellationToken cancellationToken, Query query, Action<IDataReader, CancellationToken> action) {
			lock(SyncRoot) {
				try {
					using(IDbCommand command = CreateCommand(query)) {
						using(IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess)) {
							action(reader, cancellationToken);
						}
					}
				} finally {
					if(Connection.State == ConnectionState.Open)
						Connection.Close();
				}
			}
		}
		void IDataStoreEx.ProcessStoredProc(CancellationToken cancellationToken, string sprocName, Action<IDataReader, CancellationToken> action, params OperandValue[] parameters) {
			lock(SyncRoot) {
				try {
					using(IDbCommand command = CreateCommand()) {
						PrepareCommandForStoredProc(command, sprocName, parameters);
						using(IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess)) {
							action(reader, cancellationToken);
						}
					}
				} finally {
					if(Connection.State == ConnectionState.Open)
						Connection.Close();
				}
			}
		}
		SelectedDataEx IDataStoreEx.SelectData(CancellationToken cancellationToken, Query query, string[] columns) {
			lock(SyncRoot) {
				try {
					using(IDbCommand command = CreateCommand(query)) {
						return DataStoreExHelper.GetData(command, cancellationToken, columns, NativeSkipTakeSupported, query.SkipSelectedRecords);
					}
				} finally {
					if(Connection.State == ConnectionState.Open)
						Connection.Close();
				}
			}
		}
		ColumnInfoEx[] IDataStoreEx.SelectSchema(Query query) {
			lock(SyncRoot) {
				try {
					using(IDbCommand command = CreateCommand(query)) {
						return DataStoreExHelper.GetSchema(command);
					}
				} finally {
					if(Connection.State == ConnectionState.Open)
						Connection.Close();
				}
			}
		}
		SelectedDataEx IDataStoreEx.ExecuteStoredProcedure(CancellationToken cancellationToken, string sprocName, params OperandValue[] parameters) {
			lock(SyncRoot) {
				try {
					using(IDbCommand command = CreateCommand()) {
						PrepareCommandForStoredProc(command, sprocName, parameters);
						return DataStoreExHelper.GetData(command, cancellationToken);
					}
				} finally {
					if(Connection.State == ConnectionState.Open)
						Connection.Close();
				}
			}
		}
		void PrepareCommandForStoredProc(IDbCommand command, string sprocName, params OperandValue[] parameters) {
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
	public class DataAccessMSSqlProviderFactory : MSSqlProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessMSSqlConnectionProvider.DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessMSSqlConnectionProvider.DataAccessCreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
	}
}
