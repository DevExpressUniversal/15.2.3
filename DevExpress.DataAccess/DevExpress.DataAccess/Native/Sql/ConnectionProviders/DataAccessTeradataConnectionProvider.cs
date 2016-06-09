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
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.DataAccess.Native.Sql.ConnectionProviders {
	public class DataAccessTeradataConnectionProvider : ConnectionProviderSql, IAliasFormatter, IDataStoreSchemaExplorerEx, ISupportStoredProc, IDataStoreEx {
		#region static
		static readonly char[] achtungChars = {'_', '%'};
		static DataAccessTeradataConnectionProvider() {
			RegisterDataStoreProvider(XpoProviderTypeString, CreateProviderFromString);
			RegisterDataStoreProvider("TdConnection", CreateProviderFromConnection);
			RegisterFactory(new DataAccessTeradataProviderFactory());
		}
		public static string GetConnectionString(string server, string database, string userid, string password) {
			return string.Format("{0}={1};Data Source={2};Database={3};User Id={4};Password={5};Session Mode=ANSI",
				XpoProviderTypeParameterName, XpoProviderTypeString, server, database, userid, password);
		}
		public static string GetConnectionString(string server, string database, string userid, string password, int port) {
			return string.Format("{0};Port Number={1}", GetConnectionString(server, database, userid, password), port);
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection};
			return CreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DataAccessTeradataConnectionProvider(connection, autoCreateOption);
		}
		public static IDbConnection CreateConnection(string connectionString) {
			return ReflectConnectionHelper.GetConnection("Teradata.Client.Provider", "Teradata.Client.Provider.TdConnection", connectionString);
		}
		public static void ProviderRegister() {
		}
		static DBColumnType GetColumnTypeByCode(string typeName, int length) {
			switch(typeName.Trim()) {
				case "BF":
				case "BV":
				case "BO":
					return DBColumnType.ByteArray;
				case "CF":
				case "CV":
				case "CO":
					return length <= 1 ? DBColumnType.Char : DBColumnType.String;
				case "D":
					return DBColumnType.Decimal;
				case "DA":
					return DBColumnType.DateTime;
				case "F":
				case "N":
					return DBColumnType.Double;
				case "I1":
					return DBColumnType.Byte;
				case "I2":
					return DBColumnType.Int16;
				case "I":
					return DBColumnType.Int32;
				case "I8":
					return DBColumnType.Int64;
				case "AT":
				case "TS":
				case "TZ":
				case "SZ":
					return DBColumnType.DateTime;
				case "YR":
				case "YM":
				case "MO":
				case "DY":
				case "DH":
				case "DM":
				case "DS":
				case "HR":
				case "HM":
				case "HS":
				case "MI":
				case "MS":
				case "SC":
					return DBColumnType.TimeSpan;
				case "PD":
				case "PM":
				case "PS":
				case "PT":
				case "PZ":
					return DBColumnType.String;
			}
			return DBColumnType.Unknown;
		}
		static DBColumnType GetColumnTypeByString(string typeName, int length) {
			switch(typeName) {
				case "BYTE":
				case "VARBYTE":
				case "BLOB":
					return DBColumnType.ByteArray;
				case "CHAR":
				case "VARCHAR":
				case "CLOB":
					return length <= 1 ? DBColumnType.Char : DBColumnType.String;
				case "DECIMAL":
					return DBColumnType.Decimal;
				case "DATE":
				case "TIME":
				case "TIMESTAMP":
					return DBColumnType.DateTime;
				case "FLOAT":
				case "NUMBER":
					return DBColumnType.Double;
				case "BYTEINT":
					return DBColumnType.Byte;
				case "SMALLINT":
					return DBColumnType.Int16;
				case "INTEGER":
					return DBColumnType.Int32;
				case "BIGINT":
					return DBColumnType.Int64;
			}
			if(typeName.StartsWith("INTERVAL"))
				return DBColumnType.TimeSpan;
			if(typeName.StartsWith("PERIOD"))
				return DBColumnType.String;
			return DBColumnType.Unknown;
		}
		#endregion
		public const string XpoProviderTypeString = "Teradata";
		const string aliasLead = "\"";
		const string aliasEnd = "\"";
		const bool singleQuotedString = true;
		const int minRandomValue = -2147483648;
		const int maxRandomValue = 2147483647;
		ReflectConnectionHelper helper;
		ExecMethodDelegate commandBuilderDeriveParametersHandler;
		int serverMajorVersion = -1;
		public override bool SupportNamedParameters { get { return false; } }
		public override bool NativeSkipTakeSupported { get { return true; } }
		ReflectConnectionHelper ConnectionHelper { get { return this.helper ?? (this.helper = new ReflectConnectionHelper(Connection, "Teradata.Client.Provider.TdException")); } }
		int ServerMajorVersion {
			get {
				if(this.serverMajorVersion >= 0)
					return this.serverMajorVersion;
				try {
					DataStoreEx.ProcessQuery(CancellationToken.None, new Query("select InfoData from dbc.dbcinfo where InfoKey = 'VERSION';"), (reader, cancellationToken) => {
						reader.Read();
						string versionString = reader.GetString(0);
						this.serverMajorVersion = int.Parse(versionString.Split('.')[0]);
					});
				} catch(SqlExecutionErrorException) {
					this.serverMajorVersion = 13;
				}
				return this.serverMajorVersion;
			}
		}
		IDataStoreEx DataStoreEx { get { return this; } }
		public DataAccessTeradataConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		[SuppressMessage("ReSharper", "RedundantAssignment")]
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			OperandParameter param = parameter as OperandParameter;
			if(!ReferenceEquals(param, null)) {
				createParameter = true;
				return "?";
			}
			object value = parameter.Value;
			createParameter = false;
			if(value == null)
				return null;
			switch(Type.GetTypeCode(value.GetType())) {
				case TypeCode.Int32:
					return ((int)value).ToString(CultureInfo.InvariantCulture);
				case TypeCode.Double:
					return ((double)value).ToString(CultureInfo.InvariantCulture);
				case TypeCode.Boolean:
					return (bool)value ? "true" : "false";
				case TypeCode.Char:
					return string.Format("'{0}'", value);
				case TypeCode.String:
					return string.Format("'{0}'", ((string)value).Replace("'", "''").Replace(@"\", @"\\"));
				case TypeCode.Decimal:
					return ((decimal)value).ToString(CultureInfo.InvariantCulture);
				case TypeCode.DateTime:
					return string.Format("TIMESTAMP '{0}'", ((DateTime)value).ToString("yyyy-MM-dd HH':'mm':'ss'.'ffffff", CultureInfo.InvariantCulture));
			}
			createParameter = true;
			return "?";
		}
		public override string FormatTable(string schema, string tableName) {
			return string.IsNullOrEmpty(schema)
				? string.Format(CultureInfo.InvariantCulture, "\"{0}\"", tableName)
				: string.Format(CultureInfo.InvariantCulture, "\"{0}\".\"{1}\"", schema, tableName);
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			if(tableName == SubSelectStatement.CustomSqlString)
				return tableAlias;
			if(string.IsNullOrEmpty(tableAlias))
				return FormatTable(schema, tableName);
			return string.IsNullOrEmpty(schema)
				? string.Format(CultureInfo.InvariantCulture, "\"{0}\" \"{1}\"", tableName, tableAlias)
				: string.Format(CultureInfo.InvariantCulture, "\"{0}\".\"{1}\" \"{2}\"", schema, tableName, tableAlias);
		}
		public override string FormatColumn(string columnName) {
			return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", columnName);
		}
		public override string FormatColumn(string columnName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "\"{1}\".\"{0}\"", columnName, tableAlias);
		}
		public override string ComposeSafeColumnName(string columnName) {
			return columnName;
		}
		public override string FormatConstraint(string constraintName) {
			return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", ComposeSafeConstraintName(constraintName));
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
				case FunctionOperatorType.Len:
					return string.Concat("CHARACTER_LENGTH(", operands[0], ')');
				case FunctionOperatorType.Substring:
					return FormatSubstring(operands);
				case FunctionOperatorType.Char:
					return string.Format(CultureInfo.InvariantCulture, "CHAR({0})", operands[0]);
				case FunctionOperatorType.CharIndex:
					return FormatCharIndex(operatorType, operands);
				case FunctionOperatorType.Insert:
					return string.Format(CultureInfo.InvariantCulture, "(SUBSTR({0}, 0, ({1}) + 1) || {2} || SUBSTR({0}, ({1}) + 1))", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.PadLeft:
					return FormatPadLeft(operatorType, operands);
				case FunctionOperatorType.PadRight:
					return FormatPadRight(operatorType, operands);
				case FunctionOperatorType.IsNull:
					return FormatIsNull(operatorType, operands);
				default:
					return base.FormatFunction(operatorType, operands);
			}
		}
		string FormatArithmeticFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.Log:
					return FormatLog(operatorType, operands);
				case FunctionOperatorType.Log10:
					return string.Format(CultureInfo.InvariantCulture, "LOG({0})", operands[0]);
				case FunctionOperatorType.Rnd:
					return FormatRnd(operatorType, operands);
				case FunctionOperatorType.Round:
					return FormatRound(operatorType, operands);
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
				case FunctionOperatorType.ToInt:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS int)", operands[0]);
				case FunctionOperatorType.ToLong:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS bigint)", operands[0]);
				case FunctionOperatorType.ToFloat:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS real)", operands[0]);
				case FunctionOperatorType.ToDouble:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS double precision)", operands[0]);
				case FunctionOperatorType.ToDecimal:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS decimal)", operands[0]);
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, "(CAST({0} as varchar(255)))", operands[0]);
				case FunctionOperatorType.BigMul:
					return string.Format(CultureInfo.InvariantCulture, "(CAST({0} AS bigint) * CAST({1} AS bigint))", operands[0], operands[1]);
				default:
					return null;
			}
		}
		string FormatDateTimeFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.Today:
					return "CURRENT_DATE";
				case FunctionOperatorType.Now:
					return "CURRENT_TIME";
				case FunctionOperatorType.UtcNow:
					if(ServerMajorVersion < 14)
						return "(CURRENT_TIME AT TIME ZONE INTERVAL '00:00' HOUR TO MINUTE)";
					return "(CURRENT_TIME AT TIME ZONE 'GMT')";
				case FunctionOperatorType.GetMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "CAST((EXTRACT(SECOND FROM {0}) * 1000 MOD 1000) AS INT)", operands[0]);
				case FunctionOperatorType.GetSecond:
					return string.Format(CultureInfo.InvariantCulture, "CAST((EXTRACT(SECOND FROM {0})) AS INT)", operands[0]);
				case FunctionOperatorType.GetMinute:
					return string.Format(CultureInfo.InvariantCulture, "EXTRACT(MINUTE FROM {0})", operands[0]);
				case FunctionOperatorType.GetHour:
					return string.Format(CultureInfo.InvariantCulture, "EXTRACT(HOUR FROM {0})", operands[0]);
				case FunctionOperatorType.GetDay:
					return string.Format(CultureInfo.InvariantCulture, "EXTRACT(DAY FROM {0})", operands[0]);
				case FunctionOperatorType.GetMonth:
					return string.Format(CultureInfo.InvariantCulture, "EXTRACT(MONTH FROM {0})", operands[0]);
				case FunctionOperatorType.GetYear:
					return string.Format(CultureInfo.InvariantCulture, "EXTRACT(YEAR FROM {0})", operands[0]);
				case FunctionOperatorType.GetDate:
					return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS DATE)", operands[0]);
				case FunctionOperatorType.AddTicks:
					return string.Format(CultureInfo.InvariantCulture, "({0} + INTERVAL '0 00:00:01' DAY TO SECOND * {1} / 10000000)", operands[0], operands[1]);
				case FunctionOperatorType.AddMilliSeconds:
					return string.Format(CultureInfo.InvariantCulture, "({0} + INTERVAL '0 00:00:01' DAY TO SECOND * {1} / 1000)", operands[0], operands[1]);
				case FunctionOperatorType.AddTimeSpan:
					return string.Format(CultureInfo.InvariantCulture, "({0} + INTERVAL '0 00:00:01' DAY TO SECOND * CAST(({1} * 1000 + 0.5) AS INT) / 1000)", operands[0], operands[1]);
				case FunctionOperatorType.AddSeconds:
					return string.Format(CultureInfo.InvariantCulture, "({0} + INTERVAL '1' SECOND * {1})", operands[0], operands[1]);
				case FunctionOperatorType.AddMinutes:
					return string.Format(CultureInfo.InvariantCulture, "({0} + INTERVAL '1' MINUTE * {1})", operands[0], operands[1]);
				case FunctionOperatorType.AddHours:
					return string.Format(CultureInfo.InvariantCulture, "({0} + INTERVAL '1' HOUR * {1})", operands[0], operands[1]);
				case FunctionOperatorType.AddDays:
					return string.Format(CultureInfo.InvariantCulture, "({0} + INTERVAL '1' DAY * {1})", operands[0], operands[1]);
				case FunctionOperatorType.AddMonths:
					return string.Format(CultureInfo.InvariantCulture, "({0} + INTERVAL '1' MONTH * {1})", operands[0], operands[1]);
				case FunctionOperatorType.AddYears:
					return string.Format(CultureInfo.InvariantCulture, "({0} + INTERVAL '1' YEAR * {1})", operands[0], operands[1]);
				default:
					return null;
			}
		}
		string FormatIsNull(FunctionOperatorType operatorType, string[] operands) {
			switch(operands.Length) {
				case 1:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) IS NULL)", operands[0]);
				case 2:
					return string.Format(CultureInfo.InvariantCulture, "COALESCE({0}, {1})", operands[0], operands[1]);
			}
			return base.FormatFunction(operatorType, operands);
		}
		string FormatPadRight(FunctionOperatorType operatorType, string[] operands) {
			if(ServerMajorVersion < 14)
				throw new NotSupportedException();
			switch(operands.Length) {
				case 2:
					return string.Format(CultureInfo.InvariantCulture, "RPAD({0}, {1})", operands[0], operands[1]);
				case 3:
					return string.Format(CultureInfo.InvariantCulture, "RPAD({0}, {1}, {2})", operands[0], operands[1], operands[2]);
			}
			return base.FormatFunction(operatorType, operands);
		}
		string FormatPadLeft(FunctionOperatorType operatorType, string[] operands) {
			if(ServerMajorVersion < 14)
				throw new NotSupportedException();
			switch(operands.Length) {
				case 2:
					return string.Format(CultureInfo.InvariantCulture, "LPAD({0}, {1})", operands[0], operands[1]);
				case 3:
					return string.Format(CultureInfo.InvariantCulture, "LPAD({0}, {1}, {2})", operands[0], operands[1], operands[2]);
			}
			return base.FormatFunction(operatorType, operands);
		}
		string FormatRound(FunctionOperatorType operatorType, string[] operands) {
			if(ServerMajorVersion < 14)
				throw new NotSupportedException();
			switch(operands.Length) {
				case 1:
					return string.Format(CultureInfo.InvariantCulture, "ROUND({0})", operands[0]);
				case 2:
					return string.Format(CultureInfo.InvariantCulture, "ROUND({0}, {1})", operands[0], operands[1]);
			}
			return base.FormatFunction(operatorType, operands);
		}
		string FormatRnd(FunctionOperatorType operatorType, string[] operands) {
			switch(operands.Length) {
				case 0:
					return string.Format(CultureInfo.InvariantCulture, "RANDOM({0}, {1})", minRandomValue, maxRandomValue);
				case 1:
					return string.Format(CultureInfo.InvariantCulture, "RANDOM({0}, {1})", minRandomValue, operands[0]);
				case 2:
					return string.Format(CultureInfo.InvariantCulture, "RANDOM({0}, {1})", operands[0], operands[1]);
			}
			return base.FormatFunction(operatorType, operands);
		}
		string FormatLog(FunctionOperatorType operatorType, string[] operands) {
			switch(operands.Length) {
				case 1:
					return string.Format(CultureInfo.InvariantCulture, "LN({0})", operands[0]);
				case 2:
					return string.Format(CultureInfo.InvariantCulture, "(LN({0}) / LN({1}))", operands[0], operands[1]);
			}
			return base.FormatFunction(operatorType, operands);
		}
		string FormatCharIndex(FunctionOperatorType operatorType, string[] operands) {
			switch(operands.Length) {
				case 2:
					return string.Format(CultureInfo.InvariantCulture, "(INDEX({0}, {1})-1)", operands[0], operands[1]);
				case 3:
					return ServerMajorVersion < 14
						? string.Format(CultureInfo.InvariantCulture, "(INDEX(SUBSTR({0}, {2}), {1})-1)", operands[0], operands[1], operands[2])
						: string.Format(CultureInfo.InvariantCulture, "(INSTR({0}, {1}, {2})-1)", operands[0], operands[1], operands[2]);
				case 4:
					if(ServerMajorVersion < 14)
						throw new NotSupportedException();
					return string.Format(CultureInfo.InvariantCulture, "(INSTR({0}, {1}, {2}, {3})-1)", operands[0], operands[1], operands[2], operands[3]);
			}
			return base.FormatFunction(operatorType, operands);
		}
		static string FormatSubstring(string[] operands) {
			return operands.Length < 3
				? string.Format("SUBSTR({0}, {1})", operands[0], operands[1])
				: string.Format("SUBSTR({0}, {1}, {2})", operands[0], operands[1], operands[2]);
		}
		public override string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			string formatDateTime = FormatDateTimeFunction(processParameter, operatorType, operands);
			if(formatDateTime != null)
				return formatDateTime;
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
								return string.Format(CultureInfo.InvariantCulture, "(({0} like {2}) And (Substr({0}, 0, CHARACTER_LENGTH({1}) + 1) = ({1})))", processParameter(operands[0]), processParameter(operandValue), processParameter(new ConstantValue(operandString.Substring(0, likeIndex) + "%")));
						}
					}
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(SUBSTR({0}, 0, CHARACTER_LENGTH({1}) + 1) = ({1}))", operands[0], operands[1]);
				case FunctionOperatorType.EndsWith:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(SUBSTR({0}, CHARACTER_LENGTH({0}) - CHARACTER_LENGTH({1}) + 1) = ({1}))", operands[0], operands[1]);
				case FunctionOperatorType.Contains:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(INDEX({0}, {1}) > 0)", operands[0], operands[1]);
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(({0}) IS NULL OR CHARACTER_LENGTH({0}) = 0)", operands[0]);
				case FunctionOperatorType.Max:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(CASE WHEN {0} > {1} THEN {0} ELSE {1} END)", operands[0], operands[1]);
				case FunctionOperatorType.Min:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(CASE WHEN {0} < {1} THEN {0} ELSE {1} END)", operands[0], operands[1]);
				case FunctionOperatorType.Remove:
					switch(operands.Length) {
						case 3:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "(SUBSTR({0}, 0, {1} + 1) || SUBSTR({0}, {1} + {2} + 1))", operands[0], operands[1], operands[2]);
						case 2:
							return string.Format(new ProcessParameterInvariantCulture(processParameter), "(SUBSTR({0}, 0, {1} + 1))", operands[0], operands[1]);
					}
					throw new NotSupportedException();
			}
			return base.FormatFunction(processParameter, operatorType, operands);
		}
		string FormatDateTimeFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.GetTimeOfDay:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "CAST(EXTRACT(HOUR FROM {0}) * 36000000000 + EXTRACT(MINUTE FROM {0}) * 600000000 + EXTRACT(SECOND FROM {0}) * 10000000 AS BIGINT)", operands[0]);
				case FunctionOperatorType.GetDayOfWeek:
					if(ServerMajorVersion < 14)
						return string.Format(new ProcessParameterInvariantCulture(processParameter), "(((CAST({0} AS DATE) - CAST('1900-01-01' AS DATE)) mod 7) + 1)", operands[0]);
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(CAST({0} AS DATE) - TRUNC(OrderDate, 'D'))", operands[0]);
				case FunctionOperatorType.GetDayOfYear:
					if(ServerMajorVersion < 14)
						return string.Format(new ProcessParameterInvariantCulture(processParameter), "(CAST({0} AS DATE) - (CAST({0} AS DATE) - INTERVAL '1' MONTH * (EXTRACT(MONTH FROM {0}) - 1) - EXTRACT(DAY FROM {0}) + 1))", operands[0]);
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(CAST({0} AS DATE) - TRUNC(OrderDate, 'Y'))", operands[0]);
				case FunctionOperatorType.DateDiffYear:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(CAST({1} AS DATE) - CAST({0} AS DATE) YEAR(4))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMonth:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(CAST({1} AS DATE) - CAST({0} AS DATE) MONTH(4))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffDay:
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "EXTRACT(DAY from ({1} - {0} DAY(4)))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffHour:
					Func<string, string> intervalH = str => string.Format(new ProcessParameterInvariantCulture(processParameter), "EXTRACT({2} from (({1} - {0}) DAY(4) TO HOUR))", operands[0], operands[1], str);
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "({0} * 24 + {1})", intervalH("DAY"), intervalH("HOUR"));
				case FunctionOperatorType.DateDiffMinute:
					Func<string, string> intervalM = str => string.Format(new ProcessParameterInvariantCulture(processParameter), "EXTRACT({2} from (({1} - {0}) DAY(4) TO MINUTE))", operands[0], operands[1], str);
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "(({0} * 24 + {1}) * 60 + {2})", intervalM("DAY"), intervalM("HOUR"), intervalM("MINUTE"));
				case FunctionOperatorType.DateDiffSecond:
					Func<string, string> intervalS = str => string.Format(new ProcessParameterInvariantCulture(processParameter), "EXTRACT({2} from (({1} - {0}) DAY(4) TO SECOND))", operands[0], operands[1], str);
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "CAST((({0} * 24 + {1}) * 60 + {2}) * 60 + {3} AS BIGINT)", intervalS("DAY"), intervalS("HOUR"), intervalS("MINUTE"), intervalS("SECOND"));
				case FunctionOperatorType.DateDiffMilliSecond:
					Func<string, string> intervalMS = str => string.Format(new ProcessParameterInvariantCulture(processParameter), "EXTRACT({2} from (({1} - {0}) DAY(4) TO SECOND))", operands[0], operands[1], str);
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "CAST((((({0} * 24 + {1}) * 60 + {2}) * 60 + {3}) * 1000) AS BIGINT)", intervalMS("DAY"), intervalMS("HOUR"), intervalMS("MINUTE"), intervalMS("SECOND"));
				case FunctionOperatorType.DateDiffTick:
					Func<string, string> intervalMkS = str => string.Format(new ProcessParameterInvariantCulture(processParameter), "EXTRACT({2} from (({1} - {0}) DAY(4) TO SECOND))", operands[0], operands[1], str);
					return string.Format(new ProcessParameterInvariantCulture(processParameter), "CAST((((({0} * 24 + {1}) * 60 + {2}) * 60 + {3}) * 10000000) AS BIGINT)", intervalMkS("DAY"), intervalMkS("HOUR"), intervalMkS("MINUTE"), intervalMkS("SECOND"));
			}
			return null;
		}
		public override string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int skipSelectedRecords, int topSelectedRecords) {
			if(skipSelectedRecords != 0)
				base.FormatSelect(selectedPropertiesSql, fromSql, whereSql, orderBySql, groupBySql, havingSql, skipSelectedRecords, topSelectedRecords);
			string expandedWhereSql = whereSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}where {1}", Environment.NewLine, whereSql) : string.Empty;
			string expandedOrderBySql = orderBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}order by {1}", Environment.NewLine, orderBySql) : string.Empty;
			string expandedHavingSql = havingSql != null ? string.Format(CultureInfo.InvariantCulture, "{0}having {1}", Environment.NewLine, havingSql) : string.Empty;
			string expandedGroupBySql = groupBySql != null ? string.Format(CultureInfo.InvariantCulture, "{0}group by {1}", Environment.NewLine, groupBySql) : string.Empty;
			if(skipSelectedRecords == 0) {
				string modificatorsSql = topSelectedRecords != 0 ? string.Format(CultureInfo.InvariantCulture, "top {0} ", topSelectedRecords) : string.Empty;
				return string.Format(CultureInfo.InvariantCulture, "select {0}{1} from {2}{3}{4}{5}{6}", modificatorsSql, selectedPropertiesSql, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedOrderBySql);
			}
			string qualifySql = topSelectedRecords == 0
				? string.Format(CultureInfo.InvariantCulture, "qualify row_number() over({0}) > {1}", expandedOrderBySql, skipSelectedRecords)
				: string.Format(CultureInfo.InvariantCulture, "qualify row_number() over({0}) between {1} and {2}", expandedOrderBySql, skipSelectedRecords + 1, skipSelectedRecords + topSelectedRecords);
			return string.Format(CultureInfo.InvariantCulture, "select {0} from {1}{2}{3}{4}{5}{6}", selectedPropertiesSql, fromSql, qualifySql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedOrderBySql);
		}
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
		protected override IDbConnection CreateConnection() {
			return ConnectionHelper.GetConnection(ConnectionString);
		}
		protected override int GetSafeNameTableMaxLength() {
			return 30;
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
			if(this.commandBuilderDeriveParametersHandler == null) {
				this.commandBuilderDeriveParametersHandler = ReflectConnectionHelper.GetCommandBuilderDeriveParametersDelegate("Teradata.Client.Provider", "Teradata.Client.Provider.TdCommandBuilder");
			}
			this.commandBuilderDeriveParametersHandler(command);
		}
		protected override SelectedData ExecuteSproc(string sprocName, params OperandValue[] parameters) {
			IDbCommand command = CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = string.Format("{0}.{1}", Connection.Database, sprocName);
			CommandBuilderDeriveParameters(command);
			IDataParameter returnParameter;
			List<IDataParameter> outParameters;
			PrepareParametersForExecuteSproc(parameters, command, out outParameters, out returnParameter);
			SelectedData executeSprocInternal = ExecuteSprocInternal(command, returnParameter, outParameters);
			SelectStatementResult schema = new SelectStatementResult();
			DBStoredProcedureResultSet curResultSet = new DBStoredProcedureResultSet();
			using(IDataReader reader = command.ExecuteReader()) {
				List<DBNameTypePair> dbColumns = new List<DBNameTypePair>();
				for(int i = 0; i < reader.FieldCount; i++) {
					DBColumnType columnType = DBColumn.GetColumnType(reader.GetFieldType(i));
					dbColumns.Add(new DBNameTypePair(reader.GetName(i), columnType));
				}
				curResultSet.Columns.AddRange(dbColumns);
			}
			schema.Rows = curResultSet.Columns.Select(c => new SelectStatementResultRow(new object[] {c.Name, string.Empty, c.Type.ToString()})).ToArray();
			return new SelectedData(executeSprocInternal.ResultSet[0], schema);
		}
		protected override void CreateDataBase() {
		}
		protected override void ProcessClearDatabase() {
			throw new NotSupportedException();
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
			string typeCondition = ConnectionProviderHelper.GetDataObjectTypeCondition(types, "tablekind = 'T'", "tablekind = 'V'");
			Query getTablesListQuery = new Query(string.Format(@"select trim(tablename) from dbc.tables where ({1}) and DatabaseName = '{0}'", Connection.Database, typeCondition));
			List<string> result = new List<string>();
			DataStoreEx.ProcessQuery(CancellationToken.None, getTablesListQuery, (reader, cancellationToken)=> {
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
			foreach(DBTable table in tables)
				table.Columns.Clear();
			string getTablesColumnsQuery = string.Format(@"select trim(TableName), trim(ColumnName), trim(ColumnType), ColumnLength
                                                           from dbc.columns
                                                           where DatabaseName = '{0}' {1}
                                                           order by TableName, ColumnId",
				Connection.Database, GetTablesFilter(tables, "and TableName", useTablesFilter));
			List<ColumnInfo> columnsToClarify = new List<ColumnInfo>();
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesColumnsQuery), (reader, cancellationToken)=> {
				while(reader.Read()) {
					string tableName = reader.GetString(0);
					DBTable table = tables.FirstOrDefault(t => string.Equals(t.Name, tableName));
					if(table == null)
						continue;
					string columnName = reader.GetString(1);
					if(!reader.IsDBNull(3)) {
						string typeName = reader.IsDBNull(2) ? null : reader.GetString(2);
						int length = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
						DBColumnType type = GetColumnTypeByCode(typeName, length);
						DBColumn column = ConnectionProviderHelper.CreateColumn(columnName, false, false, type == DBColumnType.String ? length : 0, type);
						table.AddColumn(column);
					} else {
						DBColumn column = ConnectionProviderHelper.CreateColumn(columnName, false, false, 0, DBColumnType.Unknown);
						columnsToClarify.Add(new ColumnInfo {TableName = table.Name, Column = column});
						table.AddColumn(column);
					}
				}
			});
			foreach(ColumnInfo columnInfo in columnsToClarify) {
				try {
					DBColumn column = columnInfo.Column;
					DataStoreEx.ProcessQuery(CancellationToken.None, new Query(string.Format("select TYPE({0}.{1}.{2});", Connection.Database, columnInfo.TableName, column.Name)), (reader, cancellationToken)=> {
						reader.Read();
						string typeString = reader.GetString(0);
						string[] typeParts = typeString.Trim().Split('(', ')');
						int length;
						if(typeParts.Length > 1)
							int.TryParse(typeParts[1], out length);
						else
							length = 0;
						DBColumnType type = GetColumnTypeByString(typeParts[0], length);
						column.ColumnType = type;
						column.Size = type == DBColumnType.String ? length : 0;
					});
				} catch {
				}
			}
		}
		struct ColumnInfo {
			public string TableName { get; set; }
			public DBColumn Column { get; set; }
		}
		void GetStorageTablesForeignKeys(ICollection<DBTable> tables, bool useTablesFilter) {
			foreach(DBTable table in tables)
				table.ForeignKeys.Clear();
			string getTablesForeignKeysQuery = string.Format(@"select trim(ChildTable), trim(ChildKeyColumn), trim(IndexName), trim(ParentTable), trim(ParentKeyColumn)
                                                               from dbc.All_RI_Children
                                                               where ParentDB = '{0}' and ChildDB = '{0}' {1}",
				Connection.Database, GetTablesFilter(tables, "and ChildTable", useTablesFilter));
			DataStoreEx.ProcessQuery(CancellationToken.None, new Query(getTablesForeignKeysQuery), (reader, cancellationToken)=> {
				while(reader.Read()) {
					string foreignTableName = reader.GetString(0);
					DBTable table = tables.FirstOrDefault(t => t.Name == foreignTableName);
					if(table == null)
						continue;
					string foreignColumnName = reader.GetString(1);
					string keyName = reader.GetString(2);
					string primaryTableName = reader.GetString(3);
					string primaryColumnName = reader.GetString(4);
					DBForeignKey fk = table.ForeignKeys.FirstOrDefault(k => k.Name == keyName);
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
		#region ISupportStoredProc Members
		DBStoredProcedure[] ISupportStoredProc.GetStoredProcedures(params string[] procedureNames) {
			List<DBStoredProcedure> result = new List<DBStoredProcedure>();
			if(procedureNames.Length > 0)
				result.AddRange(procedureNames.Select(procedureName => new DBStoredProcedure {Name = procedureName}));
			else {
				Query query = new Query(@"SELECT ExternalProcedureName FROM dbc.ExternalSPs WHERE DatabaseName = ?", new QueryParameterCollection(new OperandValue(Connection.Database)), new[] {"dbname"});
				SelectStatementResult data = SelectData(query);
				result.AddRange(data.Rows.Select(row => new DBStoredProcedure {Name = Convert.ToString(row.Values[0]).Trim()}));
				query = new Query(@"select TableName from dbc.tables where databasename = ? and TableKind = 'P';", new QueryParameterCollection(new OperandValue(Connection.Database)), new[] {"dbname"});
				data = SelectData(query);
				result.AddRange(data.Rows.Select(row => new DBStoredProcedure {Name = Convert.ToString(row.Values[0]).Trim()}));
			}
			using(IDbCommand command = Connection.CreateCommand()) {
				command.CommandType = CommandType.StoredProcedure;
				foreach(DBStoredProcedure procedure in result) {
					try {
						command.CommandText = string.Format("{0}.{1}", Connection.Database, procedure.Name);
						CommandBuilderDeriveParameters(command);
						procedure.Arguments.AddRange(ConnectionProviderHelper.GetStoredProcedureArgumentsFromCommand(command));
					} catch {
					}
				}
			}
			return result.ToArray();
		}
		DBStoredProcedureResultSet ISupportStoredProc.GetStoredProcedureTableSchema(string procedureName) {
			using(IDbCommand command = Connection.CreateCommand()) {
				command.CommandType = CommandType.StoredProcedure;
				command.CommandText = string.Format("{0}.{1}", Connection.Database, procedureName);
				CommandBuilderDeriveParameters(command);
				foreach(IDataParameter parameter in command.Parameters)
					parameter.Value = DBNull.Value;
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
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = string.Format("{0}.{1}", Connection.Database, sprocName);
			CommandBuilderDeriveParameters(command);
			IDataParameter returnParameter;
			List<IDataParameter> outParameters;
			PrepareParametersForExecuteSproc(parameters, command, out outParameters, out returnParameter);
		}
		#endregion
	}
	public class DataAccessTeradataProviderFactory : ProviderFactory {
		public const string PortParamID = "Port";
		public override bool HasUserName { get { return true; } }
		public override bool HasPassword { get { return true; } }
		public override bool HasIntegratedSecurity { get { return false; } }
		public override bool HasMultipleDatabases { get { return true; } }
		public override bool IsServerbased { get { return true; } }
		public override bool IsFilebased { get { return false; } }
		public override string ProviderKey { get { return DataAccessTeradataConnectionProvider.XpoProviderTypeString; } }
		public override string FileFilter { get { return null; } }
		public override bool MeanSchemaGeneration { get { return true; } }
		public override bool SupportStoredProcedures { get { return true; } }
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessTeradataConnectionProvider.CreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessTeradataConnectionProvider.CreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(ServerParamID) || !parameters.ContainsKey(DatabaseParamID))
				return null;
			if(!parameters.ContainsKey(UserIDParamID) || !parameters.ContainsKey(PasswordParamID))
				return null;
			string server = parameters[ServerParamID];
			string database = parameters[DatabaseParamID];
			string userid = parameters[UserIDParamID];
			string password = parameters[PasswordParamID];
			return parameters.ContainsKey(PortParamID)
				? DataAccessTeradataConnectionProvider.GetConnectionString(server, database, userid, password, int.Parse(parameters[PortParamID]))
				: DataAccessTeradataConnectionProvider.GetConnectionString(server, database, userid, password);
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
		public override string[] GetDatabases(string server, string userid, string password) {
			return GetDatabases(server, string.Empty, userid, password);
		}
		public string[] GetDatabases(string server, string port, string userid, string password) {
			if(string.IsNullOrEmpty(server))
				return new string[0];
			int portNumber;
			string connectionString = int.TryParse(port, out portNumber)
				? DataAccessTeradataConnectionProvider.GetConnectionString(server, string.Empty, userid, password, portNumber)
				: DataAccessTeradataConnectionProvider.GetConnectionString(server, string.Empty, userid, password);
			Xpo.DB.Helpers.ConnectionStringParser helper = new Xpo.DB.Helpers.ConnectionStringParser(connectionString);
			helper.RemovePartByName(DataStoreBase.XpoProviderTypeParameterName);
			helper.RemovePartByName("Database");
			IDbConnection connection = null;
			IDataReader reader;
			try {
				connection = DataAccessTeradataConnectionProvider.CreateConnection(helper.GetConnectionString());
				connection.Open();
				IDbCommand command = connection.CreateCommand();
				command.CommandText = "select distinct trim(DatabaseName) from dbc.tables";
				reader = command.ExecuteReader();
			} catch {
				if(connection != null)
					connection.Close();
				return new string[0];
			}
			List<string> result = new List<string>();
			while(reader.Read()) {
				try {
					string databaseName = reader.GetString(0);
					if(databaseName != "DBC")
						result.Add(databaseName);
				} catch {
				}
			}
			connection.Close();
			return result.ToArray();
		}
	}
}
