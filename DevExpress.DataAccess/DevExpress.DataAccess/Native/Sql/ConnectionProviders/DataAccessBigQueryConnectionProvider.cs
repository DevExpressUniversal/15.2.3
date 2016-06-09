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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.DataAccess.Native.Sql.ConnectionProviders {
	public class DataAccessBigQueryConnectionProvider : ConnectionProviderSql, IAliasFormatter, IDataStoreSchemaExplorerEx, IDataStoreEx, ISupportOrderByExpressionAlias, ISupportGroupByExpressionAlias {
		#region static
		static readonly char[] achtungChars = {'_', '%'};
		static DataAccessBigQueryConnectionProvider() {
			RegisterDataStoreProvider(XpoProviderTypeString, CreateProviderFromString);
			RegisterDataStoreProvider("BigQueryConnection", CreateProviderFromConnection);
			RegisterFactory(new DataAccessBigQueryProviderFactory());
		}
		public static string GetConnectionString(string projectId, string datasetId, string oAuthCilentID, string oAuthClientSecret, string oAuthRefreshToken) {
			return string.Format("{0}={1};ProjectId={5};DatasetId={6};OAuthClientID={2};OAuthClientSecret={3};OAuthRefreshToken={4}",
				XpoProviderTypeParameterName, XpoProviderTypeString, oAuthCilentID, oAuthClientSecret, oAuthRefreshToken, projectId, datasetId);
		}
		public static string GetConnectionString(string projectId, string datasetId, string privateKeyFileName, string serviceAccountEmail) {
			return string.Format("{0}={1};ProjectId={4};DatasetId={5};ServiceAccountEmail={2};PrivateKeyFileName={3}",
				XpoProviderTypeParameterName, XpoProviderTypeString, serviceAccountEmail, privateKeyFileName, projectId, datasetId);
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection};
			return CreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DataAccessBigQueryConnectionProvider(connection, autoCreateOption);
		}
		public static IDbConnection CreateConnection(string connectionString) {
			return ReflectConnectionHelper.GetConnection("DevExpress.DataAccess.BigQuery", "DevExpress.DataAccess.BigQuery.BigQueryConnection", connectionString);
		}
		public static void ProviderRegister() {
		}
		static DBTable CreateDBTable(System.Data.DataTable table) {
			DBTable dbTable = new DBTable {Name = table.TableName};
			foreach(DataRow row in table.Rows) {
				Type o = (Type)row["DataType"];
				dbTable.AddColumn(new DBColumn((string)row["ColumnName"], false, o.ToString(), 0, DBColumn.GetColumnType(o, true)));
			}
			return dbTable;
		}
		#endregion
		public const string XpoProviderTypeString = "BigQuery";
		const string aliasLead = "";
		const string aliasEnd = "";
		const bool singleQuotedString = true;
		ReflectConnectionHelper helper;
		public override bool SupportNamedParameters { get { return false; } }
		public override bool NativeSkipTakeSupported { get { return false; } }
		ReflectConnectionHelper ConnectionHelper { get { return helper ?? (helper = new ReflectConnectionHelper(Connection, "System.Exception")); } }
		public DataAccessBigQueryConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		[SuppressMessage("ReSharper", "RedundantAssignment")]
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			OperandParameter param = parameter as OperandParameter;
			if(!ReferenceEquals(param, null)) {
				createParameter = true;
				return string.Format("@{0}", param.ParameterName);
			}
			object value = parameter.Value;
			if(value != null) {
				createParameter = false;
				Type type = value.GetType();
				if(type == typeof(TimeSpan))
					return ((TimeSpan)value).TotalSeconds.ToString(CultureInfo.InvariantCulture);
				switch(Type.GetTypeCode(type)) {
					case TypeCode.Int32:
						return ((int)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.Single:
						return ((float)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.Double:
						return ((double)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.Boolean:
						return (bool)value ? "true" : "false";
					case TypeCode.Char:
						return string.Format("'{0}'", (char)value);
					case TypeCode.String:
						return string.Concat("'", ((string)value).Replace("'", "''").Replace(@"\", @"\\"), "'");
					case TypeCode.Decimal:
						return ((decimal)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.DateTime:
						return string.Format("TIMESTAMP(\"{0}\")", ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
				}
			}
			createParameter = true;
			return "@p" + index.ToString(CultureInfo.InvariantCulture);
		}
		public override string FormatTable(string schema, string tableName) {
			return string.IsNullOrEmpty(schema)
				? string.Format(CultureInfo.InvariantCulture, "[{0}]", tableName)
				: string.Format(CultureInfo.InvariantCulture, "[{0}.{1}]", schema, tableName);
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			if(tableName == SubSelectStatement.CustomSqlString)
				return tableAlias;
			if(string.IsNullOrEmpty(tableAlias))
				return string.IsNullOrEmpty(schema)
					? string.Format(CultureInfo.InvariantCulture, "[{0}.{1}] [{1}]", Connection.Database, tableName)
					: string.Format(CultureInfo.InvariantCulture, "[{0}.{1}] [{1}]", schema, tableName);
			return string.IsNullOrEmpty(schema)
				? string.Format(CultureInfo.InvariantCulture, "[{0}.{1}] [{2}]", Connection.Database, tableName, tableAlias)
				: string.Format(CultureInfo.InvariantCulture, "[{0}.{1}] [{2}]", schema, tableName, tableAlias);
		}
		public override string FormatColumn(string columnName) {
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", columnName);
		}
		public override string FormatColumn(string columnName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "[{0}.{1}]", tableAlias, columnName);
		}
		public override string FormatConstraint(string constraintName) {
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", ComposeSafeConstraintName(constraintName));
		}
		public override string FormatOrder(string sortProperty, SortingDirection direction) {
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", sortProperty, direction == SortingDirection.Ascending ? "asc" : "desc");
		}
		public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			string formatArithmeticFunction = FormatArithmeticFunction(operatorType, operands);
			if(formatArithmeticFunction != null)
				return formatArithmeticFunction;
			string formatDateTimeFunction = FormatDateTimeFunction(operatorType, operands);
			if(formatDateTimeFunction != null)
				return formatDateTimeFunction;
			switch(operatorType) {
				case FunctionOperatorType.Substring:
					return operands.Length < 3
						? string.Format("SUBSTR({0}, {1})", operands[0], operands[1])
						: string.Format("SUBSTR({0}, {1}, {2})", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Char:
					throw new NotSupportedException();
				case FunctionOperatorType.CharIndex:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(INSTR({0}, {1})-1)", operands[0], operands[1]);
						case 3:
						case 4:
							throw new NotSupportedException();
					}
					goto default;
				case FunctionOperatorType.Remove:
					switch(operands.Length) {
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "CONCAT(SUBSTR({0}, 0, {1} + 1), SUBSTR({0}, {1} + {2} + 1))", operands[0], operands[1], operands[2]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(SUBSTR({0}, 0, {1} + 1))", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.ToInt:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS INTEGER)", operands[0]);
				case FunctionOperatorType.ToFloat:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS float)", operands[0]);
				case FunctionOperatorType.ToLong:
				case FunctionOperatorType.ToDouble:
				case FunctionOperatorType.ToDecimal:
					throw new NotSupportedException();
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} as string)", operands[0]);
				case FunctionOperatorType.Insert:
					return string.Format(CultureInfo.InvariantCulture, "CONCAT(SUBSTR({0}, 0, ({1}) + 1), {2}, SUBSTR({0}, ({1}) + 1))", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.PadLeft:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "LPAD({0}, {1}, ' ')", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "LPAD({0}, {1}, {2})", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.PadRight:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "RPAD({0}, {1}, ' ')", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "RPAD({0}, {1}, {2})", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.IsNull:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "(({0}) IS NULL)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "IFNULL({0}, {1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) IS NULL OR LENGTH({0}) = 0)", operands[0]);
				case FunctionOperatorType.EndsWith:
					return string.Format(CultureInfo.InvariantCulture, "(SUBSTR({0}, LENGTH({0}) - LENGTH({1}) + 1) = ({1}))", operands[0], operands[1]);
				case FunctionOperatorType.Contains:
					return string.Format(CultureInfo.InvariantCulture, "({0} CONTAINS {1})", operands[0], operands[1]);
				default:
					return base.FormatFunction(operatorType, operands);
			}
		}
		static string FormatArithmeticFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.Log:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "LN({0})", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(LN({0}) / LN({1}))", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Log10:
					return string.Format(CultureInfo.InvariantCulture, "log10({0})", operands[0]);
				case FunctionOperatorType.Rnd:
					return "rand()";
				case FunctionOperatorType.Max:
					return string.Format(CultureInfo.InvariantCulture, "GREATEST({0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Min:
					return string.Format(CultureInfo.InvariantCulture, "LEAST({0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Round:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "ROUND({0})", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "ROUND({0}, {1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Sqr:
					return string.Format(CultureInfo.InvariantCulture, "SQRT({0})", operands[0]);
				case FunctionOperatorType.Acos:
					return string.Format(CultureInfo.InvariantCulture, "ACOS({0})", operands[0]);
				case FunctionOperatorType.Asin:
					return string.Format(CultureInfo.InvariantCulture, "ASIN({0})", operands[0]);
				case FunctionOperatorType.Atn:
					return string.Format(CultureInfo.InvariantCulture, "ATAN({0})", operands[0]);
				case FunctionOperatorType.Atn2:
					return string.Format(CultureInfo.InvariantCulture, "ATAN2({0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Cosh:
					return string.Format(CultureInfo.InvariantCulture, "COSH({0})", operands[0]);
				case FunctionOperatorType.Sinh:
					return string.Format(CultureInfo.InvariantCulture, "SINH({0})", operands[0]);
				case FunctionOperatorType.Tanh:
					return string.Format(CultureInfo.InvariantCulture, "TANH({0})", operands[0]);
				case FunctionOperatorType.BigMul:
					return string.Format(CultureInfo.InvariantCulture, "({0} * {1})", operands[0], operands[1]);
				default:
					return null;
			}
		}
		static string FormatDateTimeFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.Today:
					return "CURRENT_DATE()";
				case FunctionOperatorType.Now:
					return "CURRENT_TIMESTAMP()";
				case FunctionOperatorType.UtcNow:
					return "CURRENT_TIMESTAMP()";
				case FunctionOperatorType.GetMilliSecond:
					throw new NotSupportedException();
				case FunctionOperatorType.GetSecond:
					return string.Format(CultureInfo.InvariantCulture, "SECOND({0})", operands[0]);
				case FunctionOperatorType.GetMinute:
					return string.Format(CultureInfo.InvariantCulture, "MINUTE({0})", operands[0]);
				case FunctionOperatorType.GetHour:
					return string.Format(CultureInfo.InvariantCulture, "HOUR({0})", operands[0]);
				case FunctionOperatorType.GetDay:
					return string.Format(CultureInfo.InvariantCulture, "DAY({0})", operands[0]);
				case FunctionOperatorType.GetMonth:
					return string.Format(CultureInfo.InvariantCulture, "MONTH({0})", operands[0]);
				case FunctionOperatorType.GetYear:
					return string.Format(CultureInfo.InvariantCulture, "YEAR({0})", operands[0]);
				case FunctionOperatorType.GetTimeOfDay:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMP_TO_MSEC(TIMESTAMP(TIME({0})))", operands[0]);
				case FunctionOperatorType.GetDayOfWeek:
					return string.Format(CultureInfo.InvariantCulture, "(DAYOFWEEK({0}) - 1)", operands[0]);
				case FunctionOperatorType.GetDayOfYear:
					return string.Format(CultureInfo.InvariantCulture, "DAYOFYEAR({0})", operands[0]);
				case FunctionOperatorType.GetDate:
					return string.Format(CultureInfo.InvariantCulture, "DATE({0})", operands[0]);
				case FunctionOperatorType.AddTicks:
				case FunctionOperatorType.AddMilliSeconds:
					throw new NotSupportedException();
				case FunctionOperatorType.AddTimeSpan:
					return string.Format(CultureInfo.InvariantCulture, "DATE_ADD({0}, CAST({1} AS INTEGER), \"SECOND\")", operands[0], operands[1]);
				case FunctionOperatorType.AddSeconds:
					return string.Format(CultureInfo.InvariantCulture, "DATE_ADD({0}, {1}, \"SECOND\")", operands[0], operands[1]);
				case FunctionOperatorType.AddMinutes:
					return string.Format(CultureInfo.InvariantCulture, "DATE_ADD({0}, {1}, \"MINUTE\")", operands[0], operands[1]);
				case FunctionOperatorType.AddHours:
					return string.Format(CultureInfo.InvariantCulture, "DATE_ADD({0}, {1}, \"HOUR\")", operands[0], operands[1]);
				case FunctionOperatorType.AddDays:
					return string.Format(CultureInfo.InvariantCulture, "DATE_ADD({0}, {1}, \"DAY\")", operands[0], operands[1]);
				case FunctionOperatorType.AddMonths:
					return string.Format(CultureInfo.InvariantCulture, "DATE_ADD({0}, {1}, \"MONTH\")", operands[0], operands[1]);
				case FunctionOperatorType.AddYears:
					return string.Format(CultureInfo.InvariantCulture, "DATE_ADD({0}, {1}, \"YEAR\")", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffYear:
					return string.Format(CultureInfo.InvariantCulture, "(YEAR({1}) - YEAR({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMonth:
					return string.Format(CultureInfo.InvariantCulture, "((YEAR({1}) - YEAR({0})) * 12 + MONTH({1}) - MONTH({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffDay:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF({1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffHour:
					return string.Format(CultureInfo.InvariantCulture, "CAST(((TIMESTAMP_TO_SEC({1}) - TIMESTAMP_TO_SEC({0})) / 3600) AS INTEGER)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMinute:
					return string.Format(CultureInfo.InvariantCulture, "CAST(((TIMESTAMP_TO_SEC({1}) - TIMESTAMP_TO_SEC({0})) / 60) AS INTEGER)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffSecond:
					return string.Format(CultureInfo.InvariantCulture, "(TIMESTAMP_TO_SEC({1}) - TIMESTAMP_TO_SEC({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "(TIMESTAMP_TO_MSEC({1}) - TIMESTAMP_TO_MSEC({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffTick:
					return string.Format(CultureInfo.InvariantCulture, "CAST(((TIMESTAMP_TO_MSEC({1}) - TIMESTAMP_TO_MSEC({0})) * 10000) AS INTEGER)", operands[0], operands[1]);
				default:
					return null;
			}
		}
		public override string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.StartsWith:
					OperandParameter operandParameter = operands[1] as OperandParameter;
					if(ReferenceEquals(operandParameter, null)) {
						OperandValue operandValue = operands[1] as OperandValue;
						if(!ReferenceEquals(operandValue, null)) {
							string operandString = operandValue.Value.ToString();
							int likeIndex = operandString.IndexOfAny(achtungChars);
							if(likeIndex < 0)
								return string.Format(CultureInfo.InvariantCulture, "({0} LIKE {1})", processParameter(operands[0]), processParameter(new ConstantValue(operandString + "%")));
							if(likeIndex > 0)
								return string.Format(CultureInfo.InvariantCulture, "(SUBSTR({0}, 0, LENGTH({1})) = {1})", processParameter(operands[0]), processParameter(operandValue));
						}
					}
					return string.Format(CultureInfo.InvariantCulture, "(SUBSTR({0}, 0, LENGTH({1})) = ({1}))", processParameter(operands[0]), processParameter(operands[1]));
				default:
					return base.FormatFunction(processParameter, operatorType, operands);
			}
		}
		public override string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int topSelectedRecords) {
			string expandedWhereSql = whereSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}where {1}", Environment.NewLine, whereSql) : string.Empty;
			string expandedOrderBySql = orderBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}order by {1}", Environment.NewLine, orderBySql) : string.Empty;
			string expandedHavingSql = havingSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}having {1}", Environment.NewLine, havingSql) : string.Empty;
			string expandedGroupBySql = groupBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}group by {1}", Environment.NewLine, groupBySql) : string.Empty;
			string modificatorsSql = topSelectedRecords != 0 ? string.Format(CultureInfo.InvariantCulture, "{0}limit {1}", Environment.NewLine, topSelectedRecords) : string.Empty;
			return string.Format(CultureInfo.InvariantCulture, "select {0} from {1}{2}{3}{4}{5}{6}", selectedPropertiesSql, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedOrderBySql, modificatorsSql);
		}
		protected override IDbConnection CreateConnection() {
			return ConnectionHelper.GetConnection(ConnectionString);
		}
		protected override int GetSafeNameTableMaxLength() {
			return 1024;
		}
		protected override int GetSafeNameColumnMaxLength() {
			return 128;
		}
		#region Overrides of ConnectionProviderSql
		public override void GetTableSchema(DBTable table, bool checkIndexes, bool checkForeignKeys) {
			throw new NotSupportedException();
		}
		public override string[] GetStorageTablesList(bool includeViews) {
			throw new NotSupportedException();
		}
		public override void CreateTable(DBTable table) {
			throw new NotSupportedException();
		}
		public override ICollection CollectTablesToCreate(ICollection tables) {
			throw new NotSupportedException();
		}
		public override string GetSqlCreateColumnFullAttributes(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		public override string FormatInsertDefaultValues(string tableName) {
			throw new NotSupportedException();
		}
		public override string FormatInsert(string tableName, string fields, string values) {
			throw new NotSupportedException();
		}
		public override string FormatUpdate(string tableName, string sets, string whereClause) {
			throw new NotSupportedException();
		}
		public override string FormatDelete(string tableName, string whereClause) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForBoolean(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForByte(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForSByte(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForChar(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForDecimal(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForDouble(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForSingle(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForInt32(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForUInt32(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForInt16(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForUInt16(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForInt64(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForUInt64(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForString(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForDateTime(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForGuid(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override string GetSqlCreateColumnTypeForByteArray(DBTable table, DBColumn column) {
			throw new NotSupportedException();
		}
		protected override long GetIdentity(InsertStatement root, TaggedParametersHolder identitiesByTag) {
			throw new NotSupportedException();
		}
		protected override void CommandBuilderDeriveParameters(IDbCommand command) {
			throw new NotSupportedException();
		}
		protected override void CreateDataBase() {
		}
		#endregion
		#region Overrides of DataStoreBase
		protected override void ProcessClearDatabase() {
			throw new NotSupportedException();
		}
		#endregion
		#region IAliasFormatter
		string IAliasFormatter.AliasLead { get { return aliasLead; } }
		string IAliasFormatter.AliasEnd { get { return aliasEnd; } }
		bool IAliasFormatter.SingleQuotedString { get { return singleQuotedString; } }
		int IAliasFormatter.MaxTableAliasLength { get { return GetSafeNameTableMaxLength(); } }
		int IAliasFormatter.MaxColumnAliasLength { get { return GetSafeNameColumnMaxLength(); } }
		#endregion
		#region IDataStoreSchemaExplorerEx Members
		string[] IDataStoreSchemaExplorerEx.GetStorageDataObjectsNames(DataObjectTypes types) {
			Type connectionType = Connection.GetType();
			List<string> result = new List<string>();
			GetObjectNames(connectionType, result, (types & DataObjectTypes.Tables) != 0, "GetTableNames");
			GetObjectNames(connectionType, result, (types & DataObjectTypes.Views) != 0, "GetViewNames");
			return result.ToArray();
		}
		void GetObjectNames(Type connectionType, List<string> result, bool condition, string methodName) {
			if(!condition)
				return;
			MethodInfo getTables = connectionType.GetMethod(methodName);
			string[] tableStrings = getTables.Invoke(Connection, new object[0]) as string[];
			if(tableStrings != null)
				result.AddRange(tableStrings);
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
			IDbCommand command = Connection.CreateCommand();
			using(IDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly)) {
				while(reader.NextResult()) {
					System.Data.DataTable dataTable = reader.GetSchemaTable();
					if(dataTable == null)
						continue;
					DBTable target = tables.FirstOrDefault(t => t.Name == dataTable.TableName);
					if(target == null)
						continue;
					DBTable result = CreateDBTable(dataTable);
					target.Columns.Clear();
					target.Columns.AddRange(result.Columns);
				}
			}
		}
		DBTable[] GetStorageDataObjects(bool includeColumns, bool isView, params string[] tablesList) {
			string[] tableNames = ((IDataStoreSchemaExplorerEx)this).GetStorageDataObjectsNames(isView ? DataObjectTypes.Views : DataObjectTypes.Tables);
			IEnumerable<string> filteredTables = tablesList.Length == 0 ? tableNames : tableNames.Where(n => tablesList.Any(f => f == n));
			DBTable[] tables = filteredTables.Select(p => ConnectionProviderHelper.CreateTable(p, isView)).ToArray();
			if(includeColumns)
				((IDataStoreSchemaExplorerEx)this).GetColumns(tables);
			return tables;
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
	public class DataAccessBigQueryProviderFactory : ProviderFactory {
		public override bool HasUserName { get { return false; } }
		public override bool HasPassword { get { return false; } }
		public override bool HasIntegratedSecurity { get { return true; } }
		public override bool HasMultipleDatabases { get { return true; } }
		public override bool IsServerbased { get { return true; } }
		public override bool IsFilebased { get { return false; } }
		public override string FileFilter { get { return null; } }
		public override bool MeanSchemaGeneration { get { return true; } }
		public override bool SupportStoredProcedures { get { return false; } }
		public override string ProviderKey { get { return DataAccessBigQueryConnectionProvider.XpoProviderTypeString; } }
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessBigQueryConnectionProvider.CreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessBigQueryConnectionProvider.CreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(ServerParamID) || !parameters.ContainsKey(DatabaseParamID))
				return null;
			string projectId = parameters[ServerParamID];
			string datasetId = parameters[DatabaseParamID];
			if(parameters.ContainsKey(DataAccessConnectionParameter.PrivateKeyFileNameParamID)) {
				string privateKeyFileName = parameters[DataAccessConnectionParameter.PrivateKeyFileNameParamID];
				string serviceAccountEmail = ConnectionProviderHelper.GetStringValue(parameters, DataAccessConnectionParameter.ServiceAccountEmailParamID);
				return DataAccessBigQueryConnectionProvider.GetConnectionString(projectId, datasetId, privateKeyFileName, serviceAccountEmail);
			}
			if(!parameters.ContainsKey(DataAccessConnectionParameter.OAuthClientIDParamID) || !parameters.ContainsKey(DataAccessConnectionParameter.OAuthClientSecretParamID))
				return null;
			string oAuthCilentID = parameters[DataAccessConnectionParameter.OAuthClientIDParamID];
			string oAuthClientSecret = parameters[DataAccessConnectionParameter.OAuthClientSecretParamID];
			string oAuthRefreshToken = ConnectionProviderHelper.GetStringValue(parameters, DataAccessConnectionParameter.OAuthRefreshTokenParamID);
			return DataAccessBigQueryConnectionProvider.GetConnectionString(projectId, datasetId, oAuthCilentID, oAuthClientSecret, oAuthRefreshToken);
		}
		public override IDataStore CreateProvider(Dictionary<string, string> parameters, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			string connectionString = GetConnectionString(parameters);
			if(connectionString == null) {
				objectsToDisposeOnDisconnect = new IDisposable[0];
				return null;
			}
			Xpo.DB.Helpers.ConnectionStringParser helper = new Xpo.DB.Helpers.ConnectionStringParser(connectionString);
			helper.RemovePartByName(DataStoreBase.XpoProviderTypeParameterName);
			return CreateProviderFromString(helper.GetConnectionString(), autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public string[] GetDatabases(string project, string client, string secret, string token) {
			if(string.IsNullOrEmpty(project))
				return new string[0];
			string connectionString = DataAccessBigQueryConnectionProvider.GetConnectionString(project, string.Empty, client, secret, token);
			return GetDatabases(connectionString);
		}
		public override string[] GetDatabases(string project, string file, string email) {
			if(string.IsNullOrEmpty(project))
				return new string[0];
			string connectionString = DataAccessBigQueryConnectionProvider.GetConnectionString(project, string.Empty, file, email);
			return GetDatabases(connectionString);
		}
		static string[] GetDatabases(string connectionString) {
			Xpo.DB.Helpers.ConnectionStringParser helper = new Xpo.DB.Helpers.ConnectionStringParser(connectionString);
			helper.RemovePartByName(DataStoreBase.XpoProviderTypeParameterName);
			string[] result;
			IDbConnection connection = null;
			try {
				connection = DataAccessBigQueryConnectionProvider.CreateConnection(helper.GetConnectionString());
				connection.Open();
				Type connectionType = connection.GetType();
				MethodInfo getTables = connectionType.GetMethod("GetDataSetNames");
				result = getTables.Invoke(connection, new object[0]) as string[];
			} catch {
				if(connection != null)
					connection.Close();
				return new string[0];
			}
			connection.Close();
			return result;
		}
	}
}
