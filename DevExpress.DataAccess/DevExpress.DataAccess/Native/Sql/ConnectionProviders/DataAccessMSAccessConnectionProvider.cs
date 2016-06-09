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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.DataAccess.Native.Sql.ConnectionProviders {
	public class DataAccessMSAccessConnectionProvider : AccessConnectionProvider, IAliasFormatter, IDataStoreSchemaExplorerEx, ICommandParameterNameProvider, IDataStoreEx {
		#region static
		static DBColumnType GetColumnType(OleDbType type) {
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
				case OleDbType.UnsignedTinyInt:
					return DBColumnType.Byte;
				case OleDbType.SmallInt:
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
		#endregion
		const string aliasLead = "[";
		const string aliasEnd = "]";
		const bool singleQuotedString = true;
		[SuppressMessage("Microsoft.Design", "CA1021")]
		public static IDataStore DataAccessCreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = new OleDbConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection};
			return DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore DataAccessCreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			OleDbConnection oleDbConnection = connection as OleDbConnection;
			if(oleDbConnection == null)
				return null;
			if(oleDbConnection.Provider.StartsWith("Microsoft.Jet.OLEDB", StringComparison.InvariantCultureIgnoreCase)
			   || oleDbConnection.Provider.StartsWith("Microsoft.ACE.OLEDB", StringComparison.InvariantCultureIgnoreCase))
				return new DataAccessMSAccessConnectionProvider(connection, autoCreateOption);
			return null;
		}
		static DataAccessMSAccessConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterDataStoreProvider("System.Data.OleDb.OleDbConnection", DataAccessCreateProviderFromConnection);
			RegisterFactory(new DataAccessMSAccess97ProviderFactory());
			RegisterFactory(new DataAccessMSAccess2007ProviderFactory());
		}
		public static void ProviderRegister() {
		}
		public DataAccessMSAccessConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
			SortColumnsAlphabetically = false;
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
					case TypeCode.Int32:
						return ((int)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.Boolean:
						return (bool)value ? "true" : "false";
					case TypeCode.String:
						return "'" + ((string)value).Replace("'", "''") + "'";
					case TypeCode.Decimal:
						return ((decimal)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.DateTime:
						return string.Format("#{0}#", ((DateTime)value).ToString(CultureInfo.InvariantCulture));
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
					return string.Format(CultureInfo.InvariantCulture, "IIF(ISNULL({1}), NULL, DATEADD('d', {1}, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, "IIF(ISNULL({0}), NULL, CStr({0}))", operands[0]);
				case FunctionOperatorType.ToInt:
					return string.Format(CultureInfo.InvariantCulture, "IIF(ISNULL({0}), NULL, Clng({0}))", operands[0]);
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
			DataTable tables = OleDBConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] {});
			if(tables == null)
				return result.ToArray();
			bool needTables = (types & DataObjectTypes.Tables) != 0;
			bool needViews = (types & DataObjectTypes.Views) != 0;
			foreach(DataRow row in tables.Rows) {
				if((needTables && (string)row["TABLE_TYPE"] == "TABLE") || (needViews && (string)row["TABLE_TYPE"] == "VIEW"))
					result.Add((string)row["TABLE_NAME"]);
			}
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
				DataTable dataTable = OleDBConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] {null, null, ComposeSafeTableName(table.Name), null});
				if(dataTable == null)
					continue;
				SortedList cols = new SortedList();
				foreach(DataRow r in dataTable.Rows) {
					DBColumnType type = GetColumnType((OleDbType)r["DATA_TYPE"]);
					cols.Add(r["ORDINAL_POSITION"], new DBColumn((string)r["COLUMN_NAME"], false, String.Empty, type == DBColumnType.String ? (int)(long)r["character_maximum_length"] : 0, type));
				}
				foreach(DictionaryEntry de in cols) {
					table.AddColumn(de.Value as DBColumn);
				}
			}
		}
		void GetStorageTablesForeignKeys(IEnumerable<DBTable> tables) {
			foreach(DBTable table in tables) {
				table.ForeignKeys.Clear();
				DataTable dataTable = OleDBConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, new object[] {null, null, null, null, null, ComposeSafeTableName(table.Name)});
				if(dataTable == null)
					continue;
				foreach(DataRow row in dataTable.Rows) {
					string foreignKeyName = (string)row["FK_NAME"];
					DBForeignKey fk = table.ForeignKeys.FirstOrDefault(f => f.Name == foreignKeyName);
					if(fk == null) {
						fk = new DBForeignKey {
							Name = foreignKeyName, PrimaryKeyTable = (string)row["PK_TABLE_NAME"]
						};
						table.ForeignKeys.Add(fk);
					}
					fk.Columns.Add((string)row["FK_COLUMN_NAME"]);
					fk.PrimaryKeyTableKeyColumns.Add((string)row["PK_COLUMN_NAME"]);
				}
			}
		}
		#endregion
		#region ICommandParameterNameProvider
		IEnumerable<string> ICommandParameterNameProvider.GetCommandParameterNames(string parameterName) {
			string[] parameterFormats = new string[] {"[{0}]", "[@{0}]"};
			foreach(string format in parameterFormats)
				yield return string.Format(format, parameterName);
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
	public class DataAccessMSAccess97ProviderFactory : Access97ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessMSAccessConnectionProvider.DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessMSAccessConnectionProvider.DataAccessCreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
	}
	public class DataAccessMSAccess2007ProviderFactory : Access2007ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessMSAccessConnectionProvider.DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessMSAccessConnectionProvider.DataAccessCreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
	}
}
