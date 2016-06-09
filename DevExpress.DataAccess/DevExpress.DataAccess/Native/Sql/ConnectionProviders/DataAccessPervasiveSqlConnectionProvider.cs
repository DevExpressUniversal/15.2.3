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
using System.Text;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.DataAccess.Native.Sql.ConnectionProviders {
	public class DataAccessPervasiveSqlConnectionProvider : PervasiveSqlConnectionProvider, IAliasFormatter, IDataStoreSchemaExplorerEx, ISupportOrderByExpressionAlias, IDataStoreEx {
		#region static
		static DBColumnType GetColumnType(byte dataType, UInt16 size, UInt16 flags) {
			switch(dataType) {
				case 0: 
					if((flags & 0x1000) != 0x1000) {
						switch(size) {
							case 1:
								return DBColumnType.Char;
							case 36:
								return DBColumnType.Guid;
							default:
								return DBColumnType.String;
						}
					}
					goto default;
				case 1: 
					switch(size) {
						case 1:
							return DBColumnType.Byte;
						case 2:
							return DBColumnType.Int16;
						case 4:
							return DBColumnType.Int32;
						case 8:
							return DBColumnType.Int64;
					}
					goto default;
				case 2:
					switch(size) {
						case 4:
							return DBColumnType.Single;
						case 8:
							return DBColumnType.Double;
					}
					goto default;
				case 3: 
					return DBColumnType.DateTime;
				case 5: 
				case 6: 
					return DBColumnType.Decimal;
				case 8: 
					switch(size) {
						case 3:
							return DBColumnType.SByte;
						case 5:
							return DBColumnType.UInt16;
						case 10:
							return DBColumnType.UInt32;
						case 20:
							return DBColumnType.UInt64;
					}
					goto default;
				case 11: 
					return DBColumnType.String;
				case 14: 
					switch(size) {
						case 1:
							return DBColumnType.Byte;
						case 2:
							return DBColumnType.UInt16;
						case 4:
							return DBColumnType.UInt32;
						case 8:
							return DBColumnType.UInt64;
					}
					goto default;
				case 15: 
					switch(size) {
						case 4:
							return DBColumnType.Int32;
						case 2:
							return DBColumnType.Int16;
					}
					goto default;
				case 16: 
					return DBColumnType.Boolean;
				case 20: 
					return DBColumnType.DateTime;
				case 21: 
					if((flags & 0x1000) == 0x1000) {
						return DBColumnType.ByteArray;
					}
					return DBColumnType.String;
				case 30: 
					return DBColumnType.DateTime;
				default:
					return DBColumnType.Unknown;
			}
		}
		static int GetColumnSize(byte dataType, UInt16 size, UInt16 flags) {
			switch(dataType) {
				case 0: 
					if((flags & 0x1000) != 0x1000) {
						switch(size) {
							case 1:
							case 36:
								return 0;
							default:
								return size;
						}
					}
					goto default;
				case 11: 
					return size - 1;
				default:
					return 0;
			}
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
			return new DataAccessPervasiveSqlConnectionProvider(connection, autoCreateOption);
		}
		static DataAccessPervasiveSqlConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterDataStoreProvider("Pervasive.Data.SqlClient.PsqlConnection", DataAccessCreateProviderFromConnection);
			RegisterFactory(new DataAccessPervasiveSqlProviderFactory());
		}
		public static void ProviderRegister() {
		}
		IDataStoreEx DataStoreEx { get { return this; } }
		public DataAccessPervasiveSqlConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			bool shouldCreateParameter = createParameter;
			OperandParameter param = parameter as OperandParameter;
			if(!ReferenceEquals(param, null)) {
				createParameter = true;
				return "?";
			}
			object value = parameter.Value;
			if(value != null) {
				createParameter = false;
				switch(Type.GetTypeCode(value.GetType())) {
					case TypeCode.Char:
						return string.Format("'{0}'", ((char)value).ToString().Replace("'", "''"));
					case TypeCode.Decimal:
						return ((decimal)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.DateTime:
						return string.Format("CONVERT('{0}', SQL_TIMESTAMP)", ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
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
					return string.Format(CultureInfo.InvariantCulture, "MOD({0}, {1})", leftOperand, rightOperand);
				default:
					return base.FormatBinary(operatorType, leftOperand, rightOperand);
			}
		}
		public override string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.AddDays:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "TIMESTAMPADD(SQL_TSI_DAY, {1}, {0})", operands[0], operands[1]);
				default:
					return base.FormatFunction(processParameter, operatorType, operands);
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
			StringBuilder queryText = new StringBuilder();
			if((types & DataObjectTypes.Tables) != 0)
				queryText.Append(@"select RTRIM(Xf$Name) from X$File where Xf$Flags & 16 = 0");
			if((types & DataObjectTypes.Tables) != 0 && (types & DataObjectTypes.Views) != 0)
				queryText.Append(" union ");
			if((types & DataObjectTypes.Views) != 0)
				queryText.Append(@"select RTRIM(Xv$Name) from X$View");
			Query getTablesDetails = new Query(queryText.ToString());
			DataStoreEx.ProcessQuery(CancellationToken.None, getTablesDetails, (reader, cancellationToken)=> {
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
			if(tables.Count == 0)
				return;
			foreach(DBTable table in tables) {
				table.Columns.Clear();
				if(table.IsView)
					ConnectionProviderHelper.GetColumnsByReader(Connection, table, table.Name);
			}
			string getTablesColumnsQuery = string.Format(@"select RTRIM(t.Xf$Name), c.Xe$Name, c.Xe$DataType, c.Xe$Size, c.Xe$Dec, c.Xe$Flags
                                                           from X$Field c
                                                            join X$File t on c.Xe$File = t.Xf$Id
                                                           where (Xe$DataType != 255 and Xe$DataType != 227 and t.Xf$Flags & 16 = 0) {0}
                                                           order by t.Xf$Name, c.Xe$Offset", GetTablesFilter(tables, "and t.Xf$Name", useTablesFilter));
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesColumnsQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string tableName = reader.GetString(0);
					string columnName = reader.GetString(1).TrimEnd();
					if(table == null || !string.Equals(table.Name, tableName))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, tableName));
					if(table == null)
						continue;
					byte dataType = reader.GetByte(2);
					ushort size = (ushort)reader.GetValue(3);
					ushort flags = (ushort)reader.GetValue(5);
					int columnSize = GetColumnSize(dataType, size, flags);
					DBColumnType columnType = GetColumnType(dataType, size, flags);
					table.AddColumn(ConnectionProviderHelper.CreateColumn(columnName, false, false, columnSize, columnType));
				}
			});
		}
		void GetStorageTablesForeignKeys(ICollection<DBTable> tables, bool useTablesFilter) {
			if(tables.Count == 0)
				return;
			foreach(DBTable table in tables)
				table.ForeignKeys.Clear();
			string getTablesForeignKeysQuery = string.Format(@"select RTRIM(t.Xf$Name), fc.Xe$Name, pc.Xe$Name, p.Xf$Name, r.Xr$Name
                                                               from X$Relate r
                                                                join X$File t on t.Xf$Id = r.Xr$FId
                                                                join X$File p on p.Xf$Id = r.Xr$PId
                                                                join X$Index fi on fi.Xi$Number = r.Xr$FIndex and fi.Xi$File = r.Xr$FId
                                                                join X$Index pi1 on pi1.Xi$Number = r.Xr$Index and pi1.Xi$File = r.Xr$PId and pi1.Xi$Part = fi.Xi$Part 
                                                                join X$Field fc on fc.Xe$Id = fi.Xi$Field
                                                                join X$Field pc on pc.Xe$Id = pi1.Xi$Field
                                                               {0}
                                                               order by r.Xr$Name, fi.Xi$Part", GetTablesFilter(tables, "where t.Xf$Name", useTablesFilter));
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesForeignKeysQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string tableName = reader.GetString(0);
					if(table == null || !string.Equals(table.Name, tableName, StringComparison.InvariantCultureIgnoreCase))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, tableName, StringComparison.InvariantCultureIgnoreCase));
					if(table == null)
						continue;
					string foreignColumnName = reader.GetString(1).TrimEnd();
					string primaryColumnName = reader.GetString(2).TrimEnd();
					string primaryTableName = reader.GetString(3).TrimEnd();
					string keyName = reader.GetString(4).TrimEnd();
					DBForeignKey fk = table.ForeignKeys.FirstOrDefault(k => string.Equals(k.Name, keyName, StringComparison.InvariantCultureIgnoreCase));
					if(fk == null) {
						fk = new DBForeignKey {
							Name = keyName, PrimaryKeyTable = primaryTableName
						};
						table.ForeignKeys.Add(fk);
					}
					fk.Columns.Add(foreignColumnName);
					fk.PrimaryKeyTableKeyColumns.Add(primaryColumnName);
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
	public class DataAccessPervasiveSqlProviderFactory : PervasiveProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessPervasiveSqlConnectionProvider.DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessPervasiveSqlConnectionProvider.DataAccessCreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
	}
}
