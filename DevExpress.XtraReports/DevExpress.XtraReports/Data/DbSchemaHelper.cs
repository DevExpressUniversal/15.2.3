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
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
namespace DevExpress.XtraReports.Data {
	public class DbSchemaHelper : IDbSchemaHelper {
		public static string GetSchemaColumnName(DbConnection connection) {
			return connection is OdbcConnection ? ((OdbcConnection)connection).Driver.Contains("PSQLODBC") ? "TABLE_OWNER" : "TABLE_SCHEM" : "TABLE_SCHEMA";  
		}
		static class OleDbHelper {
			public const string NameColumn = "COLUMN_NAME";
			public const string DataTypeColumn = "DATA_TYPE";
			static Dictionary<OleDbType, Type> types = new Dictionary<OleDbType, Type>();
			static OleDbHelper() {
				types.Add(OleDbType.Binary, typeof(byte[]));
				types.Add(OleDbType.Boolean, typeof(bool));
				types.Add(OleDbType.BSTR, typeof(string));
				types.Add(OleDbType.Char, typeof(string));
				types.Add(OleDbType.Currency, typeof(decimal));
				types.Add(OleDbType.Date, typeof(DateTime));
				types.Add(OleDbType.DBDate, typeof(DateTime));
				types.Add(OleDbType.DBTime, typeof(TimeSpan));
				types.Add(OleDbType.DBTimeStamp, typeof(DateTime));
				types.Add(OleDbType.Decimal, typeof(decimal));
				types.Add(OleDbType.Error, typeof(int));
				types.Add(OleDbType.Filetime, typeof(DateTime));
				types.Add(OleDbType.Guid, typeof(Guid));
				types.Add(OleDbType.TinyInt, typeof(short));
				types.Add(OleDbType.SmallInt, typeof(short));
				types.Add(OleDbType.Integer, typeof(int));
				types.Add(OleDbType.BigInt, typeof(long));
				types.Add(OleDbType.LongVarBinary, typeof(byte[]));
				types.Add(OleDbType.LongVarChar, typeof(string));
				types.Add(OleDbType.Numeric, typeof(decimal));
				types.Add(OleDbType.Single, typeof(float));
				types.Add(OleDbType.Double, typeof(double));
				types.Add(OleDbType.UnsignedTinyInt, typeof(byte));
				types.Add(OleDbType.UnsignedSmallInt, typeof(int));
				types.Add(OleDbType.UnsignedInt, typeof(long));
				types.Add(OleDbType.UnsignedBigInt, typeof(decimal));
				types.Add(OleDbType.VarBinary, typeof(byte[]));
				types.Add(OleDbType.VarChar, typeof(string));
				types.Add(OleDbType.VarNumeric, typeof(decimal));
				types.Add(OleDbType.WChar, typeof(string));
				types.Add(OleDbType.VarWChar, typeof(string));
				types.Add(OleDbType.LongVarWChar, typeof(string));
				types.Add(OleDbType.Empty, typeof(IDataReader));
			}
			public static Type GetClrType(int oleDataType) {
				Type result;
				if(!types.TryGetValue((OleDbType)oleDataType, out result))
					result = typeof(object);
				return result;
			}
		}
		public DataRow[] GetSchemaTableRows(DbConnection connection, string collectionName, string restrictionValue, string paramName) {
			DataTable schemaTable = connection.GetSchema(collectionName);
			List<DataRow> result = new List<DataRow>();
			foreach(DataRow row in schemaTable.Rows) {
				if(restrictionValue == null || (string)row[paramName] == restrictionValue)
					result.Add(row);
			}
			return result.ToArray();
		}
		public string[] GetTableColumnNames(DbConnection connection, string tableName, string tableSchemaName) {
			List<string> tableColumnNames = new List<string>();
			DataTable schemaTable = connection.GetSchema("Columns", GetSchemaRestrictionValues(connection, tableName));
			foreach(DataRow row in schemaTable.Rows) {
				string tableSchema = GetSchemaColumnName(connection);
				if(!row.Table.Columns.Contains(tableSchema) || row.IsNull(tableSchema) || string.Equals(row[tableSchema], tableSchemaName)) {
					string columnName = (string)row["COLUMN_NAME"];
					tableColumnNames.Add(columnName);
				}
			}
			return tableColumnNames.ToArray();
		}
		public string GetSelectColumnsQuery(DbConnection connection, string tableName, string tableSchemaName) {
			return GetSelectColumnsQuery(connection, tableName, tableSchemaName, GetTableColumnNames(connection, tableName, tableSchemaName));
		}
		public string GetSelectColumnsQuery(DbConnection connection, string tableName, string tableSchemaName, string[] columnNames) {
			string prefix = GetLiteralValue(connection, OleDbLiteral.Escape_Underscore_Prefix);
			string suffix = GetLiteralValue(connection, OleDbLiteral.Escape_Underscore_Suffix);
			if(string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(suffix)) {
				prefix = GetLiteralValue(connection, OleDbLiteral.Quote_Prefix);
				suffix = GetLiteralValue(connection, OleDbLiteral.Quote_Suffix);
			}
			if(string.IsNullOrEmpty(suffix))
				suffix = prefix;
			return SelectQueryBuilder.BuildSelectCommand(tableName, tableSchemaName, columnNames, prefix, suffix);
		}
		public void FillSchema(DbConnection connection, DbDataAdapter dataAdapter, DataSet dataSet, string tableName) {
			try {
				dataAdapter.FillSchema(dataSet, SchemaType.Mapped);
			}
			catch { 
				if(connection is OleDbConnection)
					dataSet.Tables.Add(GenerateOleTableScheme((OleDbConnection)connection, tableName));
				else
					throw;
			}
		}
		static DataTable GenerateOleTableScheme(OleDbConnection connection, string tableName) {
			string catalogName = null;
			object[] restrictions = new object[] { connection.Database, catalogName, tableName };
			DataTable schema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, restrictions);
			DataTable result = new DataTable();
			foreach(DataRow columnInfo in schema.Rows)
				result.Columns.Add(columnInfo[OleDbHelper.NameColumn].ToString(), OleDbHelper.GetClrType((int)columnInfo[OleDbHelper.DataTypeColumn]));
			return result;
		}
		static string GetLiteralValue(DbConnection connection, OleDbLiteral name) {
			if(connection is OleDbConnection)
				return GetLiteralValueForOleDbConnection((OleDbConnection)connection, name);
			else if(connection is OdbcConnection)
				return GetLiteralValueForOdbcConnection((OdbcConnection)connection);
			return "\"";
		}
		static string GetLiteralValueForOleDbConnection(OleDbConnection connection, OleDbLiteral name) {
			if(connection.State == ConnectionState.Closed)
				connection.Open();
			DataTable schemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.DbInfoLiterals, null);
			schemaTable.PrimaryKey = new DataColumn[] { schemaTable.Columns["Literal"] };
			DataRow row = null;
			row = schemaTable.Rows.Find((int)name);
			return row != null ? row["LiteralValue"] as string : string.Empty;
		}
		static string GetLiteralValueForOdbcConnection(OdbcConnection connection) {
			using(DataTable schema = connection.GetSchema(DbMetaDataCollectionNames.DataSourceInformation)) {
				if(schema.Rows.Count == 0)
					return string.Empty;
				string quotedIdentifierPattern = schema.Rows[0][DbMetaDataColumnNames.QuotedIdentifierPattern].ToString();
				if(quotedIdentifierPattern.Length == 0)
					return string.Empty;
				return quotedIdentifierPattern[0].ToString();
			}
		}
		protected virtual string[] GetSchemaRestrictionValues(DbConnection connection, string tableName) {
			return new string[] { null, null, tableName };
		}
	}
}
