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
using System.Globalization;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Compatibility.System.Collections.Specialized;
using DevExpress.Compatibility.System.Data;
#if !DXPORTABLE
using DevExpress.DataAccess.Sql;
#endif
#if DXRESTRICTED
using IDbTransaction = System.Data.Common.DbTransaction;
using IDataReader = System.Data.Common.DbDataReader;
using IDbConnection = System.Data.Common.DbConnection;
using IDbCommand = System.Data.Common.DbCommand;
using IDataParameter = System.Data.Common.DbParameter;
using IDbDataParameter = System.Data.Common.DbParameter;
#endif
namespace DevExpress.DataAccess.Native.Sql.ConnectionProviders {
#if DXPORTABLE
	public static class DbDataReaderExtensions {
		public static void Close(this System.Data.Common.DbDataReader instance) {
			instance.Dispose();
		}
		public static DataTable GetSchemaTable(this System.Data.Common.DbDataReader instance) {
			DataTable result = new DataTable();
			result.Columns.Add(new DataColumn("DataType", typeof(Type)));
			result.Columns.Add(new DataColumn("ColumnName", typeof(string)));
			result.Columns.Add(new DataColumn("ColumnSize", typeof(int)));
			return result;
		}
	}
#endif
	public static class ConnectionProviderHelper {
		const string vistaDBExclusiveReadWriteString = "ExclusiveReadWrite";
		const string vistaDBNonexclusivereadwrite = "NonexclusiveReadWrite";
		public static string EscapeAlias(string alias, string lead, string end) {
			if(String.IsNullOrEmpty(alias) || String.IsNullOrEmpty(lead) || String.IsNullOrEmpty(end))
				return alias;
			string result = alias.Replace(lead, Twice(lead));
			if(lead != end)
				result = result.Replace(end, Twice(end));
			return result;
		}
		public static string EncloseAlias(string alias, string lead, string end) {
			if(String.IsNullOrEmpty(alias) || String.IsNullOrEmpty(lead) || String.IsNullOrEmpty(end))
				return alias;
			return String.Format("{0}{1}{2}", lead, EscapeAlias(alias, lead, end), end);
		}
		static string Twice(string value) {
			return String.Format(CultureInfo.InvariantCulture, "{0}{0}", value);
		}
		public static string GetDataObjectTypeCondition(DataObjectTypes types, string includeTablesCondition, string includeViewsCondition) {
			string getTablesListQuery = String.Empty;
			if((types & DataObjectTypes.Tables) != 0)
				getTablesListQuery += includeTablesCondition;
			if((types & DataObjectTypes.Tables) != 0 && (types & DataObjectTypes.Views) != 0)
				getTablesListQuery += " or ";
			if((types & DataObjectTypes.Views) != 0)
				getTablesListQuery += includeViewsCondition;
			return getTablesListQuery;
		}
		public static string GetStringValue(IDictionary<string, string> dictionary, string key) {
			return dictionary.ContainsKey(key) ? dictionary[key] : String.Empty;
		}
		public static DBTable CreateTable(string name, bool isView) {
			DBTable table = CreateTable(name);
			table.IsView = isView;
			return table;
		}
		public static DBTable CreateTable(string name) {
			return new DBTable(name);
		}
		public static List<DBTable> CreateDbTables(IDataStoreSchemaExplorerEx provider, string[] tableNames) {
			List<DBTable> result = new List<DBTable>(tableNames.Length);
			string[] tablesList = provider.GetStorageDataObjectsNames(DataObjectTypes.Tables);
			string[] viewsList = provider.GetStorageDataObjectsNames(DataObjectTypes.Views);
			foreach(string tableName in tableNames) {
				if(tablesList.Contains(tableName))
					result.Add(CreateTable(tableName));
				if(viewsList.Contains(tableName))
					result.Add(CreateTable(tableName, true));
			}
			return result;
		}
		public static DBColumn CreateColumn(string name, bool isKey, bool isIdentity, int size, DBColumnType type) {
			return new DBColumn(name, isKey, String.Empty, size, type) {IsIdentity = isIdentity};
		}
		public static DBForeignKey CreateForeignKey(string name, StringCollection columns, string refTable) {
			return new DBForeignKey(columns, refTable, columns) { Name = name };
		}
		public static DBForeignKey CreateForeignKey(string name, StringCollection columns, string refTable, StringCollection refColumns) {
			return new DBForeignKey(columns, refTable, refColumns) { Name = name };
		}
		public static DBStoredProcedure CreateDBStoredProcedure(string name) {
			return new DBStoredProcedure {Name = name};
		}
		public static DBStoredProcedureArgumentDirection GetDBStoredProcedureArgumentDirection(IDataParameter parameter) {
			DBStoredProcedureArgumentDirection direction;
			switch(parameter.Direction) {
				case ParameterDirection.InputOutput:
					direction = DBStoredProcedureArgumentDirection.InOut;
					break;
				case ParameterDirection.Output:
				case ParameterDirection.ReturnValue:
					direction = DBStoredProcedureArgumentDirection.Out;
					break;
				default:
					direction = DBStoredProcedureArgumentDirection.In;
					break;
			}
			return direction;
		}
		public static DBStoredProcedureResultSet GetResultSetFromCommand(IDbCommand command) {
			DBStoredProcedureResultSet result = new DBStoredProcedureResultSet();
			using(IDataReader reader = command.ExecuteReader()) {
				DataTable table = reader.GetSchemaTable();
				if(table == null)
					for(int i = 0; i < reader.FieldCount; i++) {
						DBColumnType columnType = DBColumn.GetColumnType(reader.GetFieldType(i));
						result.Columns.Add(new DBNameTypePair(reader.GetName(i), columnType));
					}
				else {
					int count = table.Rows.Count;
					for(int i = 0; i < count; i++) {
						var row = table.Rows[i];
						DBColumnType type = DBColumn.GetColumnType(row["DataType"] as Type);
						result.Columns.Add(new DBNameTypePair(row["ColumnName"].ToString(), type));
					}
				}
				reader.Close();
			}
			return result;
		}
		public static void GetColumnsByReader(IDbConnection connection, DBTable table, string tableName) {
			table.Columns.Clear();
			try {
				IDbCommand command = connection.CreateCommand();
				command.CommandText = String.Format("select * from {0}", tableName);
				using(IDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly)) {
					DataTable schemaTable = reader.GetSchemaTable();
					reader.Close();
					if(schemaTable == null)
						return;
					int count = schemaTable.Rows.Count;
					for(int i = 0; i < count; i++) {
						var row = schemaTable.Rows[i];
						Type columnType = (Type)row["DataType"];
						string columnName = (string)row["ColumnName"];
						int columnSize = (int)row["ColumnSize"];
						DBColumnType dbColumnType = DBColumn.GetColumnType(columnType, true);
						table.AddColumn(new DBColumn(columnName, false, columnType.ToString(), dbColumnType == DBColumnType.String ? columnSize : 0, dbColumnType));
					}
				}
			} catch {
				table.Columns.Clear();
			}
		}
		public static IEnumerable<DBStoredProcedureArgument> GetStoredProcedureArgumentsFromCommand(IDbCommand command) {
			List<DBStoredProcedureArgument> dbArguments = new List<DBStoredProcedureArgument>();
			foreach(IDataParameter parameter in command.Parameters) {
				if(parameter.Direction == ParameterDirection.ReturnValue)
					continue;
				DBStoredProcedureArgumentDirection direction = GetDBStoredProcedureArgumentDirection(parameter);
				DBColumnType columnType = ConnectionProviderSql.GetColumnType(parameter.DbType, true);
				dbArguments.Add(new DBStoredProcedureArgument(parameter.ParameterName.TrimEnd(), columnType, direction));
			}
			return dbArguments;
		}
#if !DXPORTABLE
		internal static XPVistaDBDatabase GetVistaDBDataBase(IDbConnection connection) {
			XPVistaDBDA vistaDbda = XPVistaDBDA.GetVistaDBDA(connection.GetType());
			if(connection.State == ConnectionState.Open && vistaDbda.GetConnectionOpenmode(connection) == vistaDBExclusiveReadWriteString) {
				connection.Close();
			}
			string encryptionKeyString = String.IsNullOrEmpty(vistaDbda.GetConnectionPassword(connection)) ? null : vistaDbda.GetConnectionPassword(connection);
			return vistaDbda.OpenDatabase(vistaDbda.GetConnectionSource(connection), vistaDBNonexclusivereadwrite, encryptionKeyString);
		}
#endif
	}
	public class DBColumnEqualityComparer : IEqualityComparer<DBColumn> {
		public bool Equals(DBColumn x, DBColumn y) {
			if(x == null && y == null)
				return true;
			if(x == null || y == null)
				return false;
			if(x.Name != y.Name)
				return false;
			if(x.ColumnType != y.ColumnType)
				return false;
			if(x.Size != y.Size)
				return false;
			if(x.IsKey != y.IsKey)
				return false;
			if(x.IsIdentity != y.IsIdentity)
				return false;
			if(x.DBTypeName != y.DBTypeName)
				return false;
			return true;
		}
		public int GetHashCode(DBColumn column) {
			return column.Name.GetHashCode();
		}
	}
	public class DBIndexEqualityComparer : IEqualityComparer<DBIndex> {
		public bool Equals(DBIndex x, DBIndex y) {
			if(x == null && y == null)
				return true;
			if(x == null || y == null)
				return false;
			if(x.Name != y.Name)
				return false;
			if(!new StringCollectionEqualityComparer().Equals(x.Columns, y.Columns))
				return false;
			if(x.IsUnique != y.IsUnique)
				return false;
			return true;
		}
		public int GetHashCode(DBIndex index) {
			return index.Name.GetHashCode();
		}
	}
	public class DBForeignKeyEqualityComparer : IEqualityComparer<DBForeignKey> {
		public bool Equals(DBForeignKey x, DBForeignKey y) {
			if(x == null && y == null)
				return true;
			if(x == null || y == null)
				return false;
			if(x.Name != y.Name)
				return false;
			if(x.PrimaryKeyTable != y.PrimaryKeyTable)
				return false;
			if(!new StringCollectionEqualityComparer().Equals(x.Columns, y.Columns))
				return false;
			if(!new StringCollectionEqualityComparer().Equals(x.PrimaryKeyTableKeyColumns, y.PrimaryKeyTableKeyColumns))
				return false;
			return true;
		}
		public int GetHashCode(DBForeignKey key) {
			return key.Name.GetHashCode();
		}
	}
	public class DBTableEqualityComparer : IEqualityComparer<DBTable> {
		public bool Equals(DBTable x, DBTable y) {
			if(x == null && y == null)
				return true;
			if(x == null || y == null)
				return false;
			if(x.Name != y.Name)
				return false;
			if(x.IsView != y.IsView)
				return false;
			if(!new DBIndexEqualityComparer().Equals(x.PrimaryKey, y.PrimaryKey))
				return false;
			if(x.Columns.Count != y.Columns.Count)
				return false;
			for(int i = 0; i < x.Columns.Count; i++)
				if(!new DBColumnEqualityComparer().Equals(x.Columns[i], y.Columns[i]))
					return false;
			if(x.Indexes.Count != y.Indexes.Count)
				return false;
			for(int i = 0; i < x.Indexes.Count; i++)
				if(!new DBIndexEqualityComparer().Equals(x.Indexes[i], y.Indexes[i]))
					return false;
			if(x.ForeignKeys.Count != y.ForeignKeys.Count)
				return false;
			for(int i = 0; i < x.ForeignKeys.Count; i++)
				if(!new DBForeignKeyEqualityComparer().Equals(x.ForeignKeys[i], y.ForeignKeys[i]))
					return false;
			return true;
		}
		public int GetHashCode(DBTable table) {
			return table.Name.GetHashCode();
		}
	}
#if !DXPORTABLE
	public class DBSchemaEqualityComparer : IEqualityComparer<DBSchema> {
		readonly DBTableEqualityComparer tableComparer = new DBTableEqualityComparer();
		public bool Equals(DBSchema x, DBSchema y) {
			if(x == null && y == null)
				return true;
			if(x == null || y == null)
				return false;
			if(x.Tables.Length != y.Tables.Length)
				return false;
			for(int i = 0; i < x.Tables.Length; i++)
				if(!this.tableComparer.Equals(x.Tables[i], y.Tables[i]))
					return false;
			if(x.Views.Length != y.Views.Length)
				return false;
			for(int i = 0; i < x.Views.Length; i++)
				if(!this.tableComparer.Equals(x.Views[i], y.Views[i]))
					return false;
			return true;
		}
		public int GetHashCode(DBSchema schema) {
			return (schema.Tables.Any()) ? schema.Tables[0].Name.GetHashCode() : base.GetHashCode();
		}
	}
#endif
	class StringCollectionEqualityComparer : IEqualityComparer<StringCollection> {
		public bool Equals(StringCollection x, StringCollection y) {
			if(x == null && y == null)
				return false;
			if(x == null || y == null)
				return false;
			if(x.Count != y.Count)
				return false;
			for(int i = 0; i < x.Count; i++)
				if(x[i] != y[i])
					return false;
			return true;
		}
		public int GetHashCode(StringCollection collection) {
			return collection.OfType<string>().Aggregate((result, next) => result + next).GetHashCode();
		}
	}
	class SubSelectStatement : SelectStatement {
		internal const string CustomSqlString = "DashboardCustomSQLSt";
		public SubSelectStatement(string customSql)
			: base(new DBTable(CustomSqlString), customSql) {
		}
	}
}
