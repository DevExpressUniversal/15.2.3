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
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.DataAccess.Native.Sql.ConnectionProviders {
	public class DataAccessVistaDB5ConnectionProvider : VistaDB5ConnectionProvider, IAliasFormatter, ISupportOrderByExpressionAlias, IDataStoreEx, IDataStoreSchemaExplorerEx {
		const string aliasLead = "[";
		const string aliasEnd = "]";
		const bool singleQuotedString = true;
		const string vistaDBExclusiveReadWriteString = "ExclusiveReadWrite";
		[SuppressMessage("Microsoft.Design", "CA1021")]
		public static IDataStore DataAccessCreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			IDataStore result = DataAccessCreateProviderFromConnection(connection, autoCreateOption);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection, (IDisposable)result};
			return result;
		}
		public static IDataStore DataAccessCreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DataAccessVistaDB5ConnectionProvider(connection, autoCreateOption);
		}
		static DataAccessVistaDB5ConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterFactory(new DataAccessVistaDB5ProviderFactory());
		}
		public static void ProviderRegister() {
		}
		public DataAccessVistaDB5ConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
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
			string typeCondition = ConnectionProviderHelper.GetDataObjectTypeCondition(types, "typeid = 1", "typeid = 10");
			Query getTablesDetails = new Query(string.Format(@"SELECT [name] FROM [database schema] where {0}", typeCondition));
			SelectStatementResult schemaTablesDetails = SelectData(getTablesDetails);
			return schemaTablesDetails.Rows.Select(p => ((string)p.Values[0]).TrimEnd()).ToArray();
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
				if(table.IsView)
					ConnectionProviderHelper.GetColumnsByReader(Connection, table, FormatTable(string.Empty, ComposeSafeTableName(table.Name)));
				else {
					table.Columns.Clear();
					XPVistaDBTableSchema vtable = DataBase.TableSchema(ComposeSafeTableName(table.Name));
					foreach(XPVistaDBColumnAttributes col in vtable.GetColumns()) {
						DBColumnType dbColumnType = DBColumn.GetColumnType(col.SystemType);
						table.AddColumn(new DBColumn(col.Name, false, string.Empty, dbColumnType == DBColumnType.String ? col.MaxLength : 0, dbColumnType));
					}
				}
			}
		}
		void GetStorageTablesForeignKeys(IEnumerable<DBTable> tables) {
			foreach(DBTable table in tables) {
				if(table.IsView)
					continue;
				table.ForeignKeys.Clear();
				try {
					XPVistaDBTableSchema vtable = DataBase.TableSchema(ComposeSafeTableName(table.Name));
					List<XPVistaDBRelationshipInformation> vtableGetForeignKeys = vtable.GetForeignKeys();
					if(vtableGetForeignKeys == null)
						continue;
					foreach(XPVistaDBRelationshipInformation fk in vtableGetForeignKeys) {
						StringCollection cols = SplitColumnsDefinition(fk.ForeignKey);
						XPVistaDBTableSchema primaryTableSchema = DataBase.TableSchema(fk.PrimaryTable);
						List<XPVistaDBIndexInformation> vtableGetIndices = primaryTableSchema.GetIndices();
						if(vtableGetIndices == null)
							continue;
						XPVistaDBIndexInformation primaryKey = vtableGetIndices.FirstOrDefault(i => i.Primary);
						if(primaryKey == null)
							continue;
						StringCollection rcols = SplitColumnsDefinition(primaryKey.KeyExpression);
						table.ForeignKeys.Add(new DBForeignKey(cols, fk.PrimaryTable, rcols) {
							Name = fk.Name
						});
					}
				} catch {
				}
			}
		}
		static StringCollection SplitColumnsDefinition(string foreignKey) {
			StringCollection cols = new StringCollection();
			foreach(string col in foreignKey.Split(';'))
				cols.Add(col);
			return cols;
		}
		XPVistaDBDatabase dataBase;
		XPVistaDBDatabase DataBase { get { return this.dataBase ?? (this.dataBase = ConnectionProviderHelper.GetVistaDBDataBase(Connection)); } }
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
	public class DataAccessVistaDB5ProviderFactory : VistaDB5ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessVistaDB5ConnectionProvider.DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessVistaDB5ConnectionProvider.DataAccessCreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
	}
}
