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
using System.Text;
using DevExpress.Data.Filtering;
using System.Globalization;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.DB;
using DevExpress.Data.Db;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using DevExpress.Compatibility.System.Collections.Specialized;
namespace DevExpress.Data.Db {
	public static class BaseFormatterHelper {
		public const string STR_TheIifFunctionOperatorRequiresThreeOrMoreArgumen = "The 'Iif' function operator requires three or more arguments. The number of arguments must be odd.";
		public static string DefaultFormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			string args;
			switch(operatorType) {
				case FunctionOperatorType.Iif: {
						if (operands.Length < 3 || (operands.Length % 2) == 0) throw new ArgumentException(STR_TheIifFunctionOperatorRequiresThreeOrMoreArgumen);
						if(operands.Length == 3)
							return string.Format(CultureInfo.InvariantCulture, "case when {0} then {1} else {2} end", operands[0], operands[1], operands[2]);
						StringBuilder sb = new StringBuilder();
						int index = -2;
						sb.Append("case ");
						do {
							index += 2;
							sb.AppendFormat("when {0} then {1} ", operands[index], operands[index + 1]);
						} while((index + 3) < operands.Length);
						sb.AppendFormat("else {0} end", operands[index + 2]);
						return sb.ToString();
					}
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) is null or (Len({0}) = 0))", operands[0]);
				case FunctionOperatorType.Trim:
					return "ltrim(rtrim(" + operands[0] + "))";
				case FunctionOperatorType.Concat: {
						args = string.Empty;
						foreach(string arg in operands) {
							if(args.Length > 0)
								args += " || ";
							args += arg;
						}
						return args;
					}
				case FunctionOperatorType.Substring:
					string len = operands.Length < 3 ? "Len(" + operands[0] + ")" + " - " + operands[1] : operands[2];
					return "Substring(" + operands[0] + ", (" + operands[1] + ") + 1, " + len + ")";
				case FunctionOperatorType.IsNull:
				case FunctionOperatorType.Len:
				case FunctionOperatorType.Lower:
				case FunctionOperatorType.Upper:
					args = string.Empty;
					foreach(string arg in operands) {
						if(args.Length > 0)
							args += ", ";
						args += arg;
					}
					return operatorType.ToString() + '(' + args + ')';
				case FunctionOperatorType.Abs:
					return string.Format(CultureInfo.InvariantCulture, "Abs({0})", operands[0]);
				case FunctionOperatorType.Sqr:
					return string.Format(CultureInfo.InvariantCulture, "Sqr({0})", operands[0]);
				case FunctionOperatorType.Log:
					return string.Format(CultureInfo.InvariantCulture, "Log({0})", operands[0]);
				case FunctionOperatorType.Rnd:
					return "Rnd()";
				case FunctionOperatorType.Sin:
					return string.Format(CultureInfo.InvariantCulture, "Sin({0})", operands[0]);
				case FunctionOperatorType.Tan:
					return string.Format(CultureInfo.InvariantCulture, "Tan({0})", operands[0]);
				case FunctionOperatorType.Atn:
					return string.Format(CultureInfo.InvariantCulture, "Atn({0})", operands[0]);
				case FunctionOperatorType.Cos:
					return string.Format(CultureInfo.InvariantCulture, "Cos({0})", operands[0]);
				case FunctionOperatorType.Exp:
					return string.Format(CultureInfo.InvariantCulture, "Exp({0})", operands[0]);
				case FunctionOperatorType.Power:
					return string.Format(CultureInfo.InvariantCulture, "Power({0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.GetMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Millisecond, {0})", operands[0]);
				case FunctionOperatorType.GetSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Second, {0})", operands[0]);
				case FunctionOperatorType.GetMinute:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Minute, {0})", operands[0]);
				case FunctionOperatorType.GetHour:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Hour, {0})", operands[0]);
				case FunctionOperatorType.GetDay:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Day, {0})", operands[0]);
				case FunctionOperatorType.GetMonth:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Month, {0})", operands[0]);
				case FunctionOperatorType.GetYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Year, {0})", operands[0]);
				case FunctionOperatorType.GetTimeOfDay:
					return string.Format(CultureInfo.InvariantCulture, "(CONVERT(BigInt,((CONVERT(BigInt,DATEPART(HOUR, {0}))) * 36000000000) + ((CONVERT(BigInt,DATEPART(MINUTE, {0}))) * 600000000) + ((CONVERT(BigInt,DATEPART(SECOND, {0}))) * 10000000) + ((CONVERT(BigInt,DATEPART(MILLISECOND, {0}))) * 10000)))", operands[0]);
				case FunctionOperatorType.GetDayOfWeek:
					return string.Format(CultureInfo.InvariantCulture, "CONVERT(Int,(DATEPART(dw, {0}) + (@@DATEFIRST) + 6) % 7)", operands[0]);
				case FunctionOperatorType.GetDayOfYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(DayOfYear, {0})", operands[0]);
				case FunctionOperatorType.GetDate:
					return string.Format(CultureInfo.InvariantCulture, "CONVERT(DATE, {0})", operands[0]);
				case FunctionOperatorType.Ascii:
					return string.Format(CultureInfo.InvariantCulture, "Ascii({0})", operands[0]);
				case FunctionOperatorType.Char:
					return string.Format(CultureInfo.InvariantCulture, "Char({0})", operands[0]);
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, "Str({0})", operands[0]);
				case FunctionOperatorType.Replace:
					return string.Format(CultureInfo.InvariantCulture, "Replace({0}, {1}, {2})", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Reverse:
					return string.Format(CultureInfo.InvariantCulture, "Reverse({0})", operands[0]);
				case FunctionOperatorType.Remove:
					return string.Format(CultureInfo.InvariantCulture, "Stuff({0}, {1} + 1, {2}, '')", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Insert:
					return string.Format(CultureInfo.InvariantCulture, "Stuff({0}, {1} + 1, 0, {2})", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.CharIndex:
					return string.Format(CultureInfo.InvariantCulture, "(Charindex({0}, {1}) - 1)", operands[0], operands[1]);
				case FunctionOperatorType.Sign:
					return string.Format(CultureInfo.InvariantCulture, "Sign({0})", operands[0]);
				case FunctionOperatorType.Round:
					return string.Format(CultureInfo.InvariantCulture, "Round({0})", operands[0]);
				case FunctionOperatorType.Floor:
					return string.Format(CultureInfo.InvariantCulture, "Floor({0})", operands[0]);
				case FunctionOperatorType.Ceiling:
					return string.Format(CultureInfo.InvariantCulture, "Ceiling({0})", operands[0]);
				case FunctionOperatorType.AddDays:
					return string.Format(CultureInfo.InvariantCulture, "AddDays({0})", operands[0], operands[1]);
				case FunctionOperatorType.UtcNow:
					return "UTCNOW()";
				case FunctionOperatorType.Now:
					return "NOW()";
				case FunctionOperatorType.Today:
					return "DATE()";
			}
			throw new NotImplementedException("DefaultFormatFunction for " + operatorType.ToString());
		}
		public static string DefaultFormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			string sqlOperator;
			switch(operatorType) {
				case BinaryOperatorType.Equal:
					sqlOperator = "=";
					break;
				case BinaryOperatorType.Less:
					sqlOperator = "<";
					break;
				case BinaryOperatorType.Greater:
					sqlOperator = ">";
					break;
				case BinaryOperatorType.LessOrEqual:
					sqlOperator = "<=";
					break;
				case BinaryOperatorType.GreaterOrEqual:
					sqlOperator = ">=";
					break;
				case BinaryOperatorType.NotEqual:
					sqlOperator = "<>";
					break;
#pragma warning disable 618
				case BinaryOperatorType.Like:
#pragma warning restore 618
					sqlOperator = "like";
					break;
				case BinaryOperatorType.Plus:
					sqlOperator = "+";
					break;
				case BinaryOperatorType.Minus:
					sqlOperator = "-";
					break;
				case BinaryOperatorType.Multiply:
					sqlOperator = "*";
					break;
				case BinaryOperatorType.Divide:
					sqlOperator = "/";
					break;
				case BinaryOperatorType.BitwiseAnd:
					sqlOperator = "&";
					break;
				case BinaryOperatorType.BitwiseOr:
					sqlOperator = "|";
					break;
				case BinaryOperatorType.BitwiseXor:
					sqlOperator = "^";
					break;
				case BinaryOperatorType.Modulo:
					sqlOperator = "%";
					break;
				default:
					throw new NotImplementedException("DefaultFormatBinary('" + operatorType.ToString() + "', '" + leftOperand + "', '" + rightOperand + "')");  
			}
			return "(" + leftOperand + " " + sqlOperator + " " + rightOperand + ")";
		}
		public static string DefaultFormatUnary(UnaryOperatorType operatorType, string operand) {
			string format;
			switch(operatorType) {
				case UnaryOperatorType.IsNull:
					format = "{0} is null";
					break;
				case UnaryOperatorType.Not:
					format = "not ({0})";
					break;
				case UnaryOperatorType.Minus:
					format = "-{0}";
					break;
				case UnaryOperatorType.Plus:
					format = "+{0}";
					break;
				case UnaryOperatorType.BitwiseNot:
					format = "~({0})";
					break;
				default:
					throw new NotImplementedException("DefaultFormatUnary('" + operatorType.ToString() + "', '" + operand + "')");  
			}
			return String.Format(CultureInfo.InvariantCulture, format, operand);
		}
	}
	public static class MsSqlFormatterHelper {
		public class MSSqlServerVersion {
			public bool Is2000;
			public bool Is2005;
			public bool Is2008;
			public bool? IsAzure;
			public MSSqlServerVersion(bool is2000, bool is2005, bool is2008, bool? isAzure) {
				Is2000 = is2000;
				Is2005 = is2005;
				Is2008 = is2008;
				IsAzure = isAzure;
			}
			public MSSqlServerVersion(bool is2000, bool is2005, bool? isAzure)
				: this(is2000, is2005, false, isAzure) {
			}
		}
		public static string FormatColumn(string columnName) {
			return "\"" + columnName + "\"";
		}
		public static string FormatColumn(string columnName, string tableAlias) {
			return tableAlias + ".\"" + columnName + "\"";
		}
		public static string FormatInsertDefaultValues(string tableName) {
			return "insert into " + tableName + " default values";
		}
		public static string FormatInsert(string tableName, string fields, string values) {
			return "insert into " + tableName + "(" + fields + ")values(" + values + ")";
		}
		public static string FormatUpdate(string tableName, string sets, string whereClause) {
			return "update " + tableName + " set " + sets + " where " + whereClause;
		}
		public static string FormatDelete(string tableName, string whereClause) {
			return "delete from " + tableName + " where " + whereClause;
		}
		public static string FormatFunction(FunctionOperatorType operatorType, MSSqlServerVersion sqlServerVersion, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.Abs:
					return string.Format(CultureInfo.InvariantCulture, "ABS({0})", operands[0]);
				case FunctionOperatorType.Sqr:
					return string.Format(CultureInfo.InvariantCulture, "sqrt({0})", operands[0]);
				case FunctionOperatorType.Rnd:
					return "rand()";
				case FunctionOperatorType.Log:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "Log({0})", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(Log({0}) / Log({1}))", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.BigMul:
					return string.Format(CultureInfo.InvariantCulture, "(Convert(bigint, {0}) * CONVERT(bigint,  {1}))", operands[0], operands[1]);
				case FunctionOperatorType.Log10:
					return string.Format(CultureInfo.InvariantCulture, "Log10({0})", operands[0]);
				case FunctionOperatorType.Sin:
					return string.Format(CultureInfo.InvariantCulture, "Sin({0})", operands[0]);
				case FunctionOperatorType.Tan:
					return string.Format(CultureInfo.InvariantCulture, "Tan({0})", operands[0]);
				case FunctionOperatorType.Atn:
					return string.Format(CultureInfo.InvariantCulture, "Atan({0})", operands[0]);
				case FunctionOperatorType.Atn2:
					return string.Format(CultureInfo.InvariantCulture, "(case when ({1}) = 0 then Sign({0}) * Atan(1) * 2 else Atn2({0},  {1}) end)", operands[0], operands[1]);
				case FunctionOperatorType.Cos:
					return string.Format(CultureInfo.InvariantCulture, "Cos({0})", operands[0]);
				case FunctionOperatorType.Acos:
					return string.Format(CultureInfo.InvariantCulture, "Acos({0})", operands[0]);
				case FunctionOperatorType.Asin:
					return string.Format(CultureInfo.InvariantCulture, "Asin({0})", operands[0]);
				case FunctionOperatorType.Cosh:
					return string.Format(CultureInfo.InvariantCulture, "((Exp({0}) + Exp(-({0}))) / 2)", operands[0]);
				case FunctionOperatorType.Sinh:
					return string.Format(CultureInfo.InvariantCulture, "((Exp({0}) - Exp(-({0}))) / 2)", operands[0]);
				case FunctionOperatorType.Tanh:
					return string.Format(CultureInfo.InvariantCulture, "((Exp({0}) - Exp(-({0}))) / (Exp({0}) + Exp(-({0}))))", operands[0]);
				case FunctionOperatorType.Exp:
					return string.Format(CultureInfo.InvariantCulture, "Exp({0})", operands[0]);
				case FunctionOperatorType.Power:
					return string.Format(CultureInfo.InvariantCulture, "Power({0},{1})", operands[0], operands[1]);
				case FunctionOperatorType.Round:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "Round({0},0)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "Round({0},{1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Sign:
					return string.Format(CultureInfo.InvariantCulture, "Sign({0})", operands[0]);
				case FunctionOperatorType.Floor:
					return string.Format(CultureInfo.InvariantCulture, "Floor({0})", operands[0]);
				case FunctionOperatorType.Ceiling:
					return string.Format(CultureInfo.InvariantCulture, "Ceiling({0})", operands[0]);
				case FunctionOperatorType.Max:
					return string.Format(CultureInfo.InvariantCulture, "(case when {0} > {1} then {0} else {1} end)", operands[0], operands[1]);
				case FunctionOperatorType.Min:
					return string.Format(CultureInfo.InvariantCulture, "(case when {0} < {1} then {0} else {1} end)", operands[0], operands[1]);
				case FunctionOperatorType.Ascii:
					return string.Format(CultureInfo.InvariantCulture, "Ascii({0})", operands[0]);
				case FunctionOperatorType.Char:
					return string.Format(CultureInfo.InvariantCulture, "Char({0})", operands[0]);
				case FunctionOperatorType.ToInt:
					return string.Format(CultureInfo.InvariantCulture, "Cast(({0}) as int)", operands[0]);
				case FunctionOperatorType.ToLong:
					return string.Format(CultureInfo.InvariantCulture, "Cast(({0}) as bigint)", operands[0]);
				case FunctionOperatorType.ToFloat:
					return string.Format(CultureInfo.InvariantCulture, "Cast(({0}) as real)", operands[0]);
				case FunctionOperatorType.ToDouble:
					return string.Format(CultureInfo.InvariantCulture, "Cast(({0}) as double precision)", operands[0]);
				case FunctionOperatorType.ToDecimal:
					return string.Format(CultureInfo.InvariantCulture, "Cast(({0}) as money)", operands[0]);
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, sqlServerVersion.Is2005 ? "Cast(({0}) as nvarchar(max))" : "Cast(({0}) as nvarchar(4000))", operands[0]);
				case FunctionOperatorType.GetMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Millisecond, {0})", operands[0]);
				case FunctionOperatorType.GetSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Second, {0})", operands[0]);
				case FunctionOperatorType.GetMinute:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Minute, {0})", operands[0]);
				case FunctionOperatorType.GetHour:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Hour, {0})", operands[0]);
				case FunctionOperatorType.GetDay:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Day, {0})", operands[0]);
				case FunctionOperatorType.GetMonth:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Month, {0})", operands[0]);
				case FunctionOperatorType.GetYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(Year, {0})", operands[0]);
				case FunctionOperatorType.GetTimeOfDay:
					return string.Format(CultureInfo.InvariantCulture, "(CONVERT(BigInt,((CONVERT(BigInt,DATEPART(HOUR, {0}))) * 36000000000) + ((CONVERT(BigInt,DATEPART(MINUTE, {0}))) * 600000000) + ((CONVERT(BigInt,DATEPART(SECOND, {0}))) * 10000000) + ((CONVERT(BigInt,DATEPART(MILLISECOND, {0}))) * 10000)))", operands[0]);
				case FunctionOperatorType.GetDayOfWeek:
					return string.Format(CultureInfo.InvariantCulture, "CONVERT(Int,(DATEPART(dw, {0}) + (@@DATEFIRST) + 6) % 7)", operands[0]);
				case FunctionOperatorType.GetDayOfYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(DayOfYear, {0})", operands[0]);
				case FunctionOperatorType.GetDate:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(HOUR, -DATEPART(HOUR, {0}), DATEADD(MINUTE, -DATEPART(MINUTE, {0}), DATEADD(SECOND, -DATEPART(SECOND, {0}), DATEADD(MILLISECOND, -DATEPART(MILLISECOND, {0}), {0}))))", operands[0]);
				case FunctionOperatorType.AddTicks:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(ms, CONVERT(BigInt, ({1}) / 10000) % 86400000, DATEADD(day, ({1}) / 864000000000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddMilliSeconds:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(ms, {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.AddTimeSpan:
				case FunctionOperatorType.AddSeconds:
					return string.Format(CultureInfo.InvariantCulture, (sqlServerVersion.Is2005 
						? "DATEADD(ms, (CONVERT(decimal(38, 19),({1})) * 1000)"
						: "DATEADD(ms, CONVERT(bigint, (CONVERT(decimal(38, 19),({1})) * 1000))") 
						+ " % 86400000, DATEADD(day, (CONVERT(decimal(38, 19),({1})) * 1000) / 86400000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddMinutes:
					return string.Format(CultureInfo.InvariantCulture, (sqlServerVersion.Is2005 
						? "DATEADD(ms, (CONVERT(decimal(38, 19),({1})) * 60000)"
						: "DATEADD(ms, CONVERT(bigint, (CONVERT(decimal(38, 19),({1})) * 60000))") 
						+ " % 86400000, DATEADD(day, (CONVERT(decimal(38, 19),({1})) * 60000) / 86400000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddHours:
					return string.Format(CultureInfo.InvariantCulture,  (sqlServerVersion.Is2005 
						? "DATEADD(ms, (CONVERT(decimal(38, 19),({1})) * 3600000)"
						: "DATEADD(ms, CONVERT(bigint, (CONVERT(decimal(38, 19),({1})) * 3600000))") 
						+ " % 86400000, DATEADD(day, (CONVERT(decimal(38, 19),({1})) * 3600000) / 86400000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddDays:
					return string.Format(CultureInfo.InvariantCulture, (sqlServerVersion.Is2005
						? "DATEADD(ms, (CONVERT(decimal(38, 19),({1})) * 86400000)"
						: "DATEADD(ms, CONVERT(bigint, (CONVERT(decimal(38, 19),({1})) * 86400000))")
						+ " % 86400000, DATEADD(day, (CONVERT(decimal(38, 19),({1})) * 86400000) / 86400000, {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddMonths:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(MONTH, {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.AddYears:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(YEAR, {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffDay:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(day, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffHour:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(hour, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(millisecond, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMinute:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(minute, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMonth:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(month, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(second, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffTick:
					return string.Format(CultureInfo.InvariantCulture, sqlServerVersion.Is2008 ? "((DATEDIFF(microsecond, {0}, {1})) * 10)" : "((DATEDIFF(millisecond, {0}, {1})) * 10000)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF(year, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Now:
					return "getdate()";
				case FunctionOperatorType.UtcNow:
					return "getutcdate()";
				case FunctionOperatorType.Today:
					return "DATEADD(day, DATEDIFF(day, '00:00:00', getdate()), '00:00:00')";
				case FunctionOperatorType.Concat:
					return FnConcat(operands);
				case FunctionOperatorType.Replace:
					return string.Format(CultureInfo.InvariantCulture, "Replace({0}, {1}, {2})", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Reverse:
					return string.Format(CultureInfo.InvariantCulture, "Reverse({0})", operands[0]);
				case FunctionOperatorType.Insert:
					return string.Format(CultureInfo.InvariantCulture, "Stuff({0}, ({1})+1, 0, {2})", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Remove:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "LEFT({0}, {1})", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "Stuff({0}, ({1})+1, {2}, '')", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.CharIndex:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(Charindex({0}, {1}) - 1)", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "(Charindex({0}, {1}, ({2}) + 1) - 1)", operands[0], operands[1], operands[2]);
						case 4:
							return string.Format(CultureInfo.InvariantCulture, "(Charindex({0}, SUBSTRING({1}, 1, ({2}) + ({3})), ({2}) + 1) - 1)", operands[0], operands[1], operands[2], operands[3]);
					}
					goto default;
				case FunctionOperatorType.PadLeft:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "isnull(REPLICATE(' ', (({1}) - LEN({0}))) + ({0}), {0})", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "isnull(REPLICATE({2}, (({1}) - LEN({0}))) + ({0}), {0})", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.PadRight:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "isnull({0} + REPLICATE(' ', (({1}) - LEN({0}))), {0})", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "isnull({0} + REPLICATE({2}, (({1}) - LEN({0}))), {0})", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.IsNull:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "(({0}) is null)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "isnull({0}, {1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) is null or len({0}) = 0)", operands[0]);
				case FunctionOperatorType.EndsWith:
					return string.Format(CultureInfo.InvariantCulture, "(Right({0}, Len({1})) = ({1}))", operands[0], operands[1]);
				case FunctionOperatorType.Contains:
					return string.Format(CultureInfo.InvariantCulture, "(isnull(CharIndex({1}, {0}), 0) > 0)", operands[0], operands[1]);				
				default:
					return null;
			}
		}
		readonly static char[] achtungChars = new char[] { '_', '%', '[', ']' };
		public static string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, MSSqlServerVersion sqlServerVersion, params object[] operands) {
			switch (operatorType) {
				case FunctionOperatorType.StartsWith: {
						object secondOperand = operands[1];
						if (secondOperand is OperandValue && ((OperandValue)secondOperand).Value is string) {
							string operandString = (string)((OperandValue)secondOperand).Value;
							int likeIndex = operandString.IndexOfAny(achtungChars);
							if (likeIndex < 0) {
								return string.Format(CultureInfo.InvariantCulture, "({0} like {1})", processParameter(operands[0]), processParameter(new ConstantValue(operandString + "%")));
							} else if (likeIndex > 0) {
								return string.Format(CultureInfo.InvariantCulture, "(({0} like {2}) And (Left({0}, Len({1})) = ({1})))", processParameter(operands[0]), processParameter(secondOperand), processParameter(new ConstantValue(operandString.Substring(0, likeIndex) + "%")));
							}
						}
						return string.Format(CultureInfo.InvariantCulture, "(Left({0}, Len({1})) = ({1}))", processParameter(operands[0]), processParameter(secondOperand));
					}				
				default:
					return null;
			}			
		}
		static string FnConcat(string[] operands) {
			string args = String.Empty;
			foreach(string arg in operands) {
				if(args.Length > 0)
					args += " + ";
				args += arg;
			}
			return args;
		}
		public static string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			switch(operatorType) {
				case BinaryOperatorType.Modulo:
					return leftOperand + " % " + rightOperand;
				default:
					return BaseFormatterHelper.DefaultFormatBinary(operatorType, leftOperand, rightOperand);
			}
		}
		public static string FormatConstraint(string constraintName) {
			return "\"" + constraintName + "\"";
		}
	}
	public static class AccessFormatterHelper {
		public static string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			switch(operatorType) {
				case BinaryOperatorType.Modulo:
					return string.Format(CultureInfo.InvariantCulture, "{0} mod {1}", leftOperand, rightOperand);
				case BinaryOperatorType.BitwiseXor:
				case BinaryOperatorType.BitwiseOr:
				case BinaryOperatorType.BitwiseAnd:
					throw new NotSupportedException("Jet drivers (Microsoft ODBC Driver for Access and Microsoft OLE DB Provider for Jet) do not support bitwise operators.");
				default:
					return BaseFormatterHelper.DefaultFormatBinary(operatorType, leftOperand, rightOperand);
			}
		}
		public static string FormatColumn(string columnName) {
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", columnName);
		}
		public static string FormatColumn(string columnName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "{1}.[{0}]", columnName, tableAlias);
		}
		public static string FormatConstraint(string constraintName) {
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", constraintName);
		}
		public static string FormatDelete(string tableName, string whereClause) {
			return string.Format(CultureInfo.InvariantCulture, "delete from {0} where {1}", tableName, whereClause);
		}
		public static string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.Abs:
					return string.Format(CultureInfo.InvariantCulture, "Abs({0})", operands[0]);
				case FunctionOperatorType.Sqr:
					return string.Format(CultureInfo.InvariantCulture, "Sqr({0})", operands[0]);
				case FunctionOperatorType.Rnd:
					return "Rnd()";
				case FunctionOperatorType.Log:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "Log({0})", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(Log({0}) / Log({1}))", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Log10:
					return string.Format(CultureInfo.InvariantCulture, "(Log({0}) / Log(10))", operands[0]);
				case FunctionOperatorType.Sin:
					return string.Format(CultureInfo.InvariantCulture, "Sin({0})", operands[0]);
				case FunctionOperatorType.Tan:
					return string.Format(CultureInfo.InvariantCulture, "Tan({0})", operands[0]);
				case FunctionOperatorType.Atn:
					return string.Format(CultureInfo.InvariantCulture, "Atn({0})", operands[0]);
				case FunctionOperatorType.Atn2:
					return string.Format(CultureInfo.InvariantCulture, "IIF(({0}) = 0, IIF(({1}) >= 0, 0, Atn(1) * 4), 2 * Atn({0} / (Sqr(({1}) * ({1}) + ({0}) * ({0})) + ({1}))))", operands[0], operands[1]);
				case FunctionOperatorType.Cos:
					return string.Format(CultureInfo.InvariantCulture, "Cos({0})", operands[0]);
				case FunctionOperatorType.Acos:
					return string.Format(CultureInfo.InvariantCulture, "IIF(({0}) = 1, 0, Atn(-({0}) / Sqr(-({0}) * ({0}) + 1)) + Atn(1) * 2)", operands[0]);
				case FunctionOperatorType.Asin:
					return string.Format(CultureInfo.InvariantCulture, "IIF(({0}) = 1, Atn(1) * 2, Atn(({0}) / Sqr(-({0}) * ({0}) + 1)))", operands[0]);
				case FunctionOperatorType.Exp:
					return string.Format(CultureInfo.InvariantCulture, "Exp({0})", operands[0]);
				case FunctionOperatorType.Cosh:
					return string.Format(CultureInfo.InvariantCulture, "((Exp({0}) + Exp(-({0}))) / 2)", operands[0]);
				case FunctionOperatorType.Sinh:
					return string.Format(CultureInfo.InvariantCulture, "((Exp({0}) - Exp(-({0}))) / 2)", operands[0]);
				case FunctionOperatorType.Tanh:
					return string.Format(CultureInfo.InvariantCulture, "((Exp({0}) - Exp(-({0}))) / (Exp({0}) + Exp(-({0}))))", operands[0]);
				case FunctionOperatorType.Power:
					return string.Format(CultureInfo.InvariantCulture, "(({0})^({1}))", operands[0], operands[1]);
				case FunctionOperatorType.Max:
					return string.Format(CultureInfo.InvariantCulture, "iif({0} > {1}, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Min:
					return string.Format(CultureInfo.InvariantCulture, "iif({0} < {1}, {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.IsNull:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "isnull({0})", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "iif(isnull({0}), {1}, {0})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(CultureInfo.InvariantCulture, "(isnull({0}) or len({0}) = 0)", operands[0]);
				case FunctionOperatorType.StartsWith:
					return string.Format(CultureInfo.InvariantCulture, "(Left({0}, Len({1})) = ({1}))", operands[0], operands[1]);
				case FunctionOperatorType.EndsWith:
					return string.Format(CultureInfo.InvariantCulture, "(Right({0}, Len({1})) = ({1}))", operands[0], operands[1]);
				case FunctionOperatorType.Contains:
					return string.Format(CultureInfo.InvariantCulture, "(Instr({0}, {1}) > 0)", operands[0], operands[1]);
				case FunctionOperatorType.Iif: {
						if (operands.Length < 3 || (operands.Length % 2) == 0) throw new ArgumentException(BaseFormatterHelper.STR_TheIifFunctionOperatorRequiresThreeOrMoreArgumen);
						if(operands.Length == 3)
							return string.Format(CultureInfo.InvariantCulture, "IIF({0}, {1}, {2})", operands[0], operands[1], operands[2]);
						StringBuilder sb = new StringBuilder();
						int index = -2;
						int counter = 0;
						do {
							index += 2;
							sb.AppendFormat("IIF({0}, {1}, ", operands[index], operands[index + 1]);
							counter++;
						} while((index + 3) < operands.Length);
						sb.AppendFormat("{0}", operands[index + 2]);
						sb.Append(new string(')', counter));
						return sb.ToString();
					}
				case FunctionOperatorType.Round:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "Round({0},0)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "Round({0},{1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Sign:
					return string.Format(CultureInfo.InvariantCulture, "Sgn({0})", operands[0]);
				case FunctionOperatorType.Floor:
					return string.Format(CultureInfo.InvariantCulture, "int({0})", operands[0]);
				case FunctionOperatorType.Ceiling:
					return string.Format(CultureInfo.InvariantCulture, "IIF((({0}) > 0), iif(({0}) - fix({0}) = 0, {0}, fix({0}) + 1), fix({0}))", operands[0]);
				case FunctionOperatorType.Trim:
					return string.Format(CultureInfo.InvariantCulture, "Trim({0})", operands[0]);
				case FunctionOperatorType.Ascii:
					return string.Format(CultureInfo.InvariantCulture, "Asc({0})", operands[0]);
				case FunctionOperatorType.Char:
					return string.Format(CultureInfo.InvariantCulture, "Chr({0})", operands[0]);
				case FunctionOperatorType.ToInt:
					return string.Format(CultureInfo.InvariantCulture, "Clng({0})", operands[0]);
				case FunctionOperatorType.ToLong:
					throw new NotSupportedException();
				case FunctionOperatorType.ToFloat:
					return string.Format(CultureInfo.InvariantCulture, "CSng({0})", operands[0]);
				case FunctionOperatorType.ToDouble:
					return string.Format(CultureInfo.InvariantCulture, "CDbl({0})", operands[0]);
				case FunctionOperatorType.ToDecimal:
					return string.Format(CultureInfo.InvariantCulture, "CCur({0})", operands[0]);
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, "Str({0})", operands[0]);
				case FunctionOperatorType.Reverse:
					throw new NotSupportedException();
				case FunctionOperatorType.Concat:
					return FnConcat(operands);
				case FunctionOperatorType.Upper:
					return string.Format(CultureInfo.InvariantCulture, "UCase({0})", operands[0]);
				case FunctionOperatorType.Lower:
					return string.Format(CultureInfo.InvariantCulture, "LCase({0})", operands[0]);
				case FunctionOperatorType.Insert:
					return string.Format(CultureInfo.InvariantCulture, "(LEFT({0}, {1}) + ({2}) + RIGHT({0}, len({0}) - ({1})))", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Remove:
					return FnRemove(operands);
				case FunctionOperatorType.Substring:
					return FnSubstring(operands);
				case FunctionOperatorType.CharIndex:
					return FnCharIndex(operands);
				case FunctionOperatorType.PadLeft:
					return FnPadLeft(operands);
				case FunctionOperatorType.PadRight:
					return FnPadRight(operands);
				case FunctionOperatorType.GetMilliSecond:
					throw new NotSupportedException();
				case FunctionOperatorType.GetSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART('s', {0})", operands[0]);
				case FunctionOperatorType.GetMinute:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART('n', {0})", operands[0]);
				case FunctionOperatorType.GetHour:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART('h', {0})", operands[0]);
				case FunctionOperatorType.GetDay:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART('d', {0})", operands[0]);
				case FunctionOperatorType.GetMonth:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART('m', {0})", operands[0]);
				case FunctionOperatorType.GetYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART('yyyy', {0})", operands[0]);
				case FunctionOperatorType.GetTimeOfDay:
					return string.Format(CultureInfo.InvariantCulture, "((DATEPART('h', {0})) * 36000000000) + ((DATEPART('n', {0})) * 600000000) + (DATEPART('s', {0}) * 10000000)", operands[0]);
				case FunctionOperatorType.GetDayOfWeek:
					return string.Format(CultureInfo.InvariantCulture, "(DatePart ('w', {0}, 1, 0) - 1)", operands[0]);
				case FunctionOperatorType.GetDayOfYear:
					return string.Format(CultureInfo.InvariantCulture, "DatePart ('y', {0})", operands[0]);
				case FunctionOperatorType.GetDate:
					return string.Format(CultureInfo.InvariantCulture, "iif(isnull({0}), {0}, DateSerial(Datepart('yyyy',{0}), DATEPART('m', {0}) , DATEPART('d', {0})))", operands[0]);
				case FunctionOperatorType.AddTicks:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD('s', Fix(({1}) / 10000000) mod 86400, {0})", operands[0], operands[1]);
				case FunctionOperatorType.AddMilliSeconds:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD('s', Fix(({1}) / 1000) mod 86400, {0})", operands[0], operands[1]);
				case FunctionOperatorType.AddTimeSpan:
				case FunctionOperatorType.AddSeconds:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD('s', {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.AddMinutes:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD('s', Fix(({1}) * 60) mod 86400, DATEADD('d', Fix((({1}) * 60) / 86400), {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddHours:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD('s', Fix(({1}) * 3600) mod 86400, DATEADD('d', Fix((({1}) * 3600) / 86400), {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddDays:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD('s', (Fix(({1}) * 86400)) mod 86400, DATEADD('d', Fix({1}), {0}))", operands[0], operands[1]);
				case FunctionOperatorType.AddMonths:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD('m', {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.AddYears:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD('yyyy', {1}, {0})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffDay:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF('d', {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffHour:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF('h', {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "(DATEDIFF('s', {0}, {1}) * 1000)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMinute:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF('n', {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMonth:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF('m', {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF('s', {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffTick:
					return string.Format(CultureInfo.InvariantCulture, "(DATEDIFF('s', {0}, {1}) * 10000000)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEDIFF('yyyy', {0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Now:
					return "NOW()";
				case FunctionOperatorType.Today:
					return "DATE()";					
				case FunctionOperatorType.BigMul:
				case FunctionOperatorType.Replace:
					throw new NotSupportedException();
				default:
					return null;
			}
		}
		static string FnPadRight(string[] operands) {
			if(operands.Length == 2) {
				return string.Format(CultureInfo.InvariantCulture, "IIF((({1}) - Len({0})) > 0, ({0}) + Space({1} - Len({0})), {0})", operands[0], operands[1]);
			}
			throw new NotSupportedException();
		}
		static string FnPadLeft(string[] operands) {
			if(operands.Length == 2) {
				return string.Format(CultureInfo.InvariantCulture, "IIF((({1}) - Len({0})) > 0, Space(({1}) - Len({0})) +  ({0}), {0})", operands[0], operands[1]);
			}
			throw new NotSupportedException();
		}
		static string FnCharIndex(string[] operands) {
			switch(operands.Length) {
				case 2:
					return string.Format(CultureInfo.InvariantCulture, "(Instr({1}, {0}) - 1)", operands[0], operands[1]);
				case 3:
					return string.Format(CultureInfo.InvariantCulture, "IIF(Instr(Right({1}, Len({1}) - ({2})), {0}) = 0 , -1, Instr(Right({1}, Len({1}) - ({2})), {0}) + ({2}) - 1)", operands[0], operands[1], operands[2]);
				case 4:
					return string.Format(CultureInfo.InvariantCulture, "IIF(Instr(Left(Right({1}, Len({1}) - ({2})), {3}), {0}) = 0 , -1, Instr(Left(Right({1}, Len({1}) - ({2})), {3}), {0}) + ({2}) - 1)", operands[0], operands[1], operands[2], operands[3]);
			}
			throw new NotSupportedException();
		}
		static string FnSubstring(string[] operands) {
			switch(operands.Length) {
				case 2:
					return string.Format(CultureInfo.InvariantCulture, "Mid({0}, ({1}) + 1)", operands[0], operands[1]);
				case 3:
					return string.Format(CultureInfo.InvariantCulture, "Mid({0}, ({1}) + 1, {2})", operands[0], operands[1], operands[2]);
			}
			throw new NotSupportedException();
		}
		static string FnRemove(string[] operands) {
			switch(operands.Length) {
				case 2:
					return string.Format(CultureInfo.InvariantCulture, "LEFT({0}, {1})", operands[0], operands[1]);
				case 3:
					return string.Format(CultureInfo.InvariantCulture, "(LEFT({0}, {1}) + RIGHT({0}, len({0}) - ({1}) - ({2})))", operands[0], operands[1], operands[2]);
			}
			throw new NotSupportedException();
		}
		static string FnConcat(string[] operands) {
			string args = String.Empty;
			foreach(string arg in operands) {
				if(args.Length > 0)
					args += " & ";
				args += arg;
			}
			return args;
		}
		public static string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.UtcNow:
					return FnUtcNow(processParameter);								 
				default:
					return null;
			}
		}
		static string FnUtcNow(ProcessParameter processParameter) {
			DateTime now = DateTime.Now;
			DateTime utcNow = now.ToUniversalTime();
			int diffHour = (int)((TimeSpan)(utcNow - now)).TotalHours;
			return string.Format(new ProcessParameterInvariantCulture(processParameter), "DATEADD('h', {0}, NOW())", new OperandValue(diffHour));
		}
		public static string FormatInsert(string tableName, string fields, string values) {
			return string.Format(CultureInfo.InvariantCulture, "insert into {0}({1})values({2})", tableName, fields, values);
		}
		public static string FormatInsertDefaultValues(string tableName) {
			return string.Format(CultureInfo.InvariantCulture, "insert into {0} default values", tableName);
		}
		public static string FormatTable(string schema, string tableName) {
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", tableName);
		}
		public static string FormatTable(string schema, string tableName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "[{0}] {1}", tableName, tableAlias);
		}
		public static string FormatUpdate(string tableName, string sets, string whereClause) {
			return string.Format(CultureInfo.InvariantCulture, "update {0} set {1} where {2}", tableName, sets, whereClause);
		}
	}
	public static class OracleFormatterHelper {
		public static string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			switch(operatorType) {
				case BinaryOperatorType.BitwiseAnd:
					return string.Format(CultureInfo.InvariantCulture, "bitand({0}, {1})", leftOperand, rightOperand);
				case BinaryOperatorType.BitwiseOr:
					return string.Format(CultureInfo.InvariantCulture, "({0} - bitand({0}, {1}) + {1})", leftOperand, rightOperand);
				case BinaryOperatorType.BitwiseXor:
					return string.Format(CultureInfo.InvariantCulture, "({0} - bitand({0}, {1}) + {1} - bitand({0}, {1}))", leftOperand, rightOperand);
				case BinaryOperatorType.Modulo:
					return string.Format(CultureInfo.InvariantCulture, "mod({0}, {1})", leftOperand, rightOperand);
				default:
					return BaseFormatterHelper.DefaultFormatBinary(operatorType, leftOperand, rightOperand);
			}
		}
		public static string FormatColumn(string columnName) {
			return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", columnName);
		}
		public static string FormatColumn(string columnName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "{1}.\"{0}\"", columnName, tableAlias);
		}
		public static string FormatConstraint(string constraintName) {
			return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", constraintName);
		}
		public static string FormatDelete(string tableName, string whereClause) {
			return string.Format(CultureInfo.InvariantCulture, "delete from {0} where {1}", tableName, whereClause);
		}
		public static string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.GetMilliSecond:
					throw new NotSupportedException();
				case FunctionOperatorType.GetSecond:
					return string.Format(CultureInfo.InvariantCulture, "TO_NUMBER(TO_CHAR({0}, 'SS'))", operands[0]);
				case FunctionOperatorType.GetMinute:
					return string.Format(CultureInfo.InvariantCulture, "TO_NUMBER(TO_CHAR({0}, 'MI'))", operands[0]);
				case FunctionOperatorType.GetHour:
					return string.Format(CultureInfo.InvariantCulture, "TO_NUMBER(TO_CHAR({0}, 'HH24'))", operands[0]);
				case FunctionOperatorType.GetDay:
					return string.Format(CultureInfo.InvariantCulture, "EXTRACT(DAY FROM ({0}))", operands[0]);
				case FunctionOperatorType.GetMonth:
					return string.Format(CultureInfo.InvariantCulture, "EXTRACT(MONTH FROM ({0}))", operands[0]);
				case FunctionOperatorType.GetYear:
					return string.Format(CultureInfo.InvariantCulture, "EXTRACT(YEAR FROM ({0}))", operands[0]);
				case FunctionOperatorType.GetDayOfWeek:
					return string.Format(CultureInfo.InvariantCulture, "MOD((TO_NUMBER(TO_CHAR({0}, 'D'))) - (TO_NUMBER(TO_CHAR(TO_DATE('01-01-1900','DD-MM-YYYY'), 'D'))) + 8, 7)", operands[0]);
				case FunctionOperatorType.GetDayOfYear:
					return string.Format(CultureInfo.InvariantCulture, "TO_NUMBER(TO_CHAR({0}, 'DDD'))", operands[0]);
				case FunctionOperatorType.GetTimeOfDay:
					return string.Format(CultureInfo.InvariantCulture, "(TO_NUMBER(TO_CHAR({0}, 'SSSSS')) * 10000000)", operands[0]);
				case FunctionOperatorType.GetDate:
					return string.Format(CultureInfo.InvariantCulture, "TRUNC({0})", operands[0]);
				case FunctionOperatorType.AddTicks:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + NUMTODSINTERVAL(TRUNC(({1}) / 864000000000), 'DAY') + NUMTODSINTERVAL(MOD(TRUNC(({1}) / 10000000),86400), 'SECOND'))", operands[0], operands[1]);
				case FunctionOperatorType.AddMilliSeconds:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + NUMTODSINTERVAL(TRUNC(({1}) / 1000), 'SECOND'))", operands[0], operands[1]);
				case FunctionOperatorType.AddTimeSpan:
				case FunctionOperatorType.AddSeconds:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + NUMTODSINTERVAL({1}, 'SECOND'))", operands[0], operands[1]);
				case FunctionOperatorType.AddMinutes:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + NUMTODSINTERVAL(TRUNC((({1}) * 60) / 86400), 'DAY') + NUMTODSINTERVAL(MOD(TRUNC(({1}) * 60),86400), 'SECOND'))", operands[0], operands[1]);
				case FunctionOperatorType.AddHours:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + NUMTODSINTERVAL(TRUNC((({1}) * 3600) / 86400), 'DAY') + NUMTODSINTERVAL(MOD(TRUNC(({1}) * 3600),86400), 'SECOND'))", operands[0], operands[1]);
				case FunctionOperatorType.AddDays:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) + NUMTODSINTERVAL(TRUNC({1}), 'DAY') + NUMTODSINTERVAL(MOD(TRUNC(({1}) * 86400),86400), 'SECOND'))", operands[0], operands[1]);
				case FunctionOperatorType.AddMonths:
					return string.Format(CultureInfo.InvariantCulture, "ADD_MONTHS({0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.AddYears:
					return string.Format(CultureInfo.InvariantCulture, "ADD_MONTHS({0}, ({1}) * 12)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffYear:
					return string.Format(CultureInfo.InvariantCulture, "(EXTRACT(YEAR FROM ({1})) - EXTRACT(YEAR FROM ({0})))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMonth:
					return string.Format(CultureInfo.InvariantCulture, "(((EXTRACT(YEAR FROM ({1})) - EXTRACT(YEAR FROM ({0}))) * 12) + EXTRACT(MONTH FROM ({1})) - EXTRACT(MONTH FROM ({0})))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffDay:
					return string.Format(CultureInfo.InvariantCulture, "(TRUNC({1}) - TRUNC({0}))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffHour:
					return string.Format(CultureInfo.InvariantCulture, "((TRUNC(({1}), 'HH24') - TRUNC(({0}), 'HH24')) * 24)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMinute:
					return string.Format(CultureInfo.InvariantCulture, "((TRUNC(({1}), 'MI') - TRUNC(({0}), 'MI')) * 1440)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffSecond:
					return string.Format(CultureInfo.InvariantCulture, "(ROUND((TRUNC(({1}), 'MI') - TRUNC(({0}), 'MI')) * 86400) + TO_NUMBER(TO_CHAR(({1}), 'SS')) - TO_NUMBER(TO_CHAR(({0}), 'SS')))", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "((ROUND((TRUNC(({1}), 'MI') - TRUNC(({0}), 'MI')) * 86400) + TO_NUMBER(TO_CHAR(({1}), 'SS')) - TO_NUMBER(TO_CHAR(({0}), 'SS'))) * 1000)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffTick:
					return string.Format(CultureInfo.InvariantCulture, "((ROUND((TRUNC(({1}), 'MI') - TRUNC(({0}), 'MI')) * 86400) + TO_NUMBER(TO_CHAR(({1}), 'SS')) - TO_NUMBER(TO_CHAR(({0}), 'SS'))) * 10000000)", operands[0], operands[1]);					
				case FunctionOperatorType.Now:
					return "SYSDATE";
				case FunctionOperatorType.Today:
					return "TRUNC(SYSDATE)";
				case FunctionOperatorType.Abs:
					return string.Format(CultureInfo.InvariantCulture, "ABS({0})", operands[0]);
				case FunctionOperatorType.BigMul:
					return string.Format(CultureInfo.InvariantCulture, "CAST((({0}) * ({1})) as INTEGER)", operands[0], operands[1]);
				case FunctionOperatorType.Sqr:
					return string.Format(CultureInfo.InvariantCulture, "SQRT({0})", operands[0]);
				case FunctionOperatorType.Rnd:
					throw new NotSupportedException();
				case FunctionOperatorType.Log:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "LN({0})", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "Log({1}, {0})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Log10:
					return string.Format(CultureInfo.InvariantCulture, "Log(10, {0})", operands[0]);
				case FunctionOperatorType.Sin:
					return string.Format(CultureInfo.InvariantCulture, "SIN({0})", operands[0]);
				case FunctionOperatorType.Tan:
					return string.Format(CultureInfo.InvariantCulture, "TAN({0})", operands[0]);
				case FunctionOperatorType.Atn:
					return string.Format(CultureInfo.InvariantCulture, "ATAN({0})", operands[0]);
				case FunctionOperatorType.Atn2:
					return string.Format(CultureInfo.InvariantCulture, "(case when ({1}) = 0 then Sign({0}) * Atan(1) * 2 else ATAN2({0}, {1}) end)", operands[0], operands[1]);
				case FunctionOperatorType.Cos:
					return string.Format(CultureInfo.InvariantCulture, "COS({0})", operands[0]);
				case FunctionOperatorType.Acos:
					return string.Format(CultureInfo.InvariantCulture, "ACOS({0})", operands[0]);
				case FunctionOperatorType.Asin:
					return string.Format(CultureInfo.InvariantCulture, "ASIN({0})", operands[0]);
				case FunctionOperatorType.Cosh:
					return string.Format(CultureInfo.InvariantCulture, "Cosh({0})", operands[0]);
				case FunctionOperatorType.Sinh:
					return string.Format(CultureInfo.InvariantCulture, "Sinh({0})", operands[0]);
				case FunctionOperatorType.Tanh:
					return string.Format(CultureInfo.InvariantCulture, "Tanh({0})", operands[0]);
				case FunctionOperatorType.Exp:
					return string.Format(CultureInfo.InvariantCulture, "EXP({0})", operands[0]);
				case FunctionOperatorType.Power:
					return string.Format(CultureInfo.InvariantCulture, "POWER({0},{1})", operands[0], operands[1]);
				case FunctionOperatorType.Round:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "ROUND({0},0)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "ROUND({0},{1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Sign:
					return string.Format(CultureInfo.InvariantCulture, "SIGN({0})", operands[0]);
				case FunctionOperatorType.Max:
					return string.Format(CultureInfo.InvariantCulture, "(case when {0} > {1} then {0} else {1} end)", operands[0], operands[1]);
				case FunctionOperatorType.Min:
					return string.Format(CultureInfo.InvariantCulture, "(case when {0} < {1} then {0} else {1} end)", operands[0], operands[1]);
				case FunctionOperatorType.Floor:
					return string.Format(CultureInfo.InvariantCulture, "FLOOR({0})", operands[0]);
				case FunctionOperatorType.Ceiling:
					return string.Format(CultureInfo.InvariantCulture, "CEIL({0})", operands[0]);
				case FunctionOperatorType.ToInt:
					return string.Format(CultureInfo.InvariantCulture, "TRUNC(CAST(({0}) AS INT))", operands[0]);
				case FunctionOperatorType.ToLong:
					return string.Format(CultureInfo.InvariantCulture, "TRUNC(CAST(({0}) AS NUMBER(20,0)))", operands[0]);
				case FunctionOperatorType.ToFloat:
					return string.Format(CultureInfo.InvariantCulture, "CAST(({0}) AS FLOAT)", operands[0]);
				case FunctionOperatorType.ToDouble:
					return string.Format(CultureInfo.InvariantCulture, "CAST(({0}) AS DOUBLE PRECISION)", operands[0]);
				case FunctionOperatorType.ToDecimal:
					return string.Format(CultureInfo.InvariantCulture, "CAST(({0}) AS NUMBER(19,5))", operands[0]);
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, "TO_CHAR({0})", operands[0]);
				case FunctionOperatorType.Ascii:
					return string.Format(CultureInfo.InvariantCulture, "ASCII({0})", operands[0]);
				case FunctionOperatorType.Char:
					return string.Format(CultureInfo.InvariantCulture, "CHR({0})", operands[0]);
				case FunctionOperatorType.Replace:
					return string.Format(CultureInfo.InvariantCulture, "REPLACE({0}, {1}, {2})", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Reverse:
					return string.Format(CultureInfo.InvariantCulture, "REVERSE({0})", operands[0]);
				case FunctionOperatorType.Insert:
					return string.Format(CultureInfo.InvariantCulture, "(SUBSTR({0}, 1, {1}) || ({2}) || SUBSTR({0}, ({1}) +  1))", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Remove:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "SUBSTR({0}, 1, {1})", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "(SUBSTR({0}, 1, {1}) || SUBSTR({0}, ({1}) + ({2}) + 1))", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.IsNull:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "(({0}) is null)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "NVL({0}, {1})", operands[0], operands[1]);
					}
					goto default;
				case FunctionOperatorType.Substring:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "SUBSTR({0}, ({1}) + 1)", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "SUBSTR({0}, ({1}) + 1, {2})", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.Len:
					return string.Format(CultureInfo.InvariantCulture, "LENGTH({0})", operands[0]);
				case FunctionOperatorType.CharIndex:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(INSTR({1}, {0}) - 1)", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "(INSTR({1}, {0}, ({2}) + 1) - 1)", operands[0], operands[1], operands[2]);
						case 4:
							return string.Format(CultureInfo.InvariantCulture, "(INSTR(SUBSTR({1}, 1, ({2}) + ({3})), {0}, ({2}) + 1) - 1)", operands[0], operands[1], operands[2], operands[3]);
					}
					goto default;
				case FunctionOperatorType.PadLeft:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(case when length({0}) > ({1}) then {0} else lpad({0}, {1}) end)", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "(case when length({0}) > ({1}) then {0} else lpad({0}, {1}, {2}) end)", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.PadRight:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(case when length({0}) > {1} then {0} else rpad({0}, {1}) end)", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "(case when length({0}) > {1} then {0} else rpad({0}, {1}, {2}) end)", operands[0], operands[1], operands[2]);
					}
					goto default;
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) is null or length({0}) = 0)", operands[0]);
				case FunctionOperatorType.EndsWith:
					return string.Format(CultureInfo.InvariantCulture, "(SUBSTR(({0}), GREATEST(-LENGTH({0}), -LENGTH({1}))) = ({1}))", operands[0], operands[1]);
				case FunctionOperatorType.Contains:
					return string.Format(CultureInfo.InvariantCulture, "(Instr({0}, {1}) > 0)", operands[0], operands[1]);
				default:
					return null;
			}
		}
		readonly static char[] achtungChars = new char[] { '_', '%' };
		public static string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.StartsWith: {
						object secondOperand = operands[1];
						if(secondOperand is OperandValue && ((OperandValue)secondOperand).Value is string) {
							string operandString = (string)((OperandValue)secondOperand).Value;
							int likeIndex = operandString.IndexOfAny(achtungChars);
							if(likeIndex < 0) {
								return string.Format(CultureInfo.InvariantCulture, "({0} like {1})", processParameter(operands[0]), processParameter(new ConstantValue(operandString + "%")));
							} else if(likeIndex > 0) {
								return string.Format(CultureInfo.InvariantCulture, "(({0} like {2}) And (Instr({0}, {1}) = 1))", processParameter(operands[0]), processParameter(secondOperand), processParameter(new ConstantValue(operandString.Substring(0, likeIndex) + "%")));
							}
						}
						return string.Format(CultureInfo.InvariantCulture, "(Instr({0}, {1}) = 1)", processParameter(operands[0]), processParameter(secondOperand));
					}				
				default:
					return null;
			}
		}
		public static string FormatInsert(string tableName, string fields, string values) {
			return string.Format(CultureInfo.InvariantCulture, "insert into {0}({1})values({2})", tableName, fields, values);
		}
		public static string FormatInsertDefaultValues(string tableName) {
			return string.Format(CultureInfo.InvariantCulture, "insert into {0} values(default)", tableName);
		}
		public static string FormatOrder(string sortProperty, SortingDirection direction) {
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", sortProperty, direction == SortingDirection.Ascending ? "asc nulls first" : "desc nulls last");
		}
	}
	public static class DynamicLinqFormatterHelper {
		public static string FormatUnary(UnaryOperatorType operatorType, string operand) {
			switch(operatorType) {
				case UnaryOperatorType.IsNull:
					return String.Format(CultureInfo.InvariantCulture, "({0} == null)", operand);
				default:
					return BaseFormatterHelper.DefaultFormatUnary(operatorType, operand);
			}
		}
		public static string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			switch(operatorType) {
				case BinaryOperatorType.BitwiseXor:
				case BinaryOperatorType.BitwiseOr:
				case BinaryOperatorType.BitwiseAnd:
#pragma warning disable 618
				case BinaryOperatorType.Like:
					throw new NotSupportedException();
#pragma warning restore 618
				default:
					return BaseFormatterHelper.DefaultFormatBinary(operatorType, leftOperand, rightOperand);
			}
		}
		public static string FormatColumn(string columnName) {
			return columnName;
		}
		public static string FormatColumn(string columnName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "{1}.{0}", columnName, tableAlias);
		}
		public static string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.Abs:
					return string.Format(CultureInfo.InvariantCulture, "Math.Abs({0})", operands[0]);
				case FunctionOperatorType.Sqr:
					return string.Format(CultureInfo.InvariantCulture, "Math.Sqrt({0})", operands[0]);
				case FunctionOperatorType.Log:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "Math.Log({0})", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "(Math.Log({0}, {1}))", operands[0], operands[1]);
					}
					throw new NotSupportedException();
				case FunctionOperatorType.Log10:
					return string.Format(CultureInfo.InvariantCulture, "(Math.Log10({0}))", operands[0]);
				case FunctionOperatorType.Sin:
					return string.Format(CultureInfo.InvariantCulture, "Math.Sin({0})", operands[0]);
				case FunctionOperatorType.Tan:
					return string.Format(CultureInfo.InvariantCulture, "Math.Tan({0})", operands[0]);
				case FunctionOperatorType.Atn:
					return string.Format(CultureInfo.InvariantCulture, "Math.Atan({0})", operands[0]);
				case FunctionOperatorType.Atn2:
					return string.Format(CultureInfo.InvariantCulture, "Math.Atan2({0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Cos:
					return string.Format(CultureInfo.InvariantCulture, "Math.Cos({0})", operands[0]);
				case FunctionOperatorType.Acos:
					return string.Format(CultureInfo.InvariantCulture, "Math.Acos({0})", operands[0]);
				case FunctionOperatorType.Asin:
					return string.Format(CultureInfo.InvariantCulture, "Math.Asin({0})", operands[0]);
				case FunctionOperatorType.Exp:
					return string.Format(CultureInfo.InvariantCulture, "Math.Exp({0})", operands[0]);
				case FunctionOperatorType.Cosh:
					return string.Format(CultureInfo.InvariantCulture, "Math.Cosh({0})", operands[0]);
				case FunctionOperatorType.Sinh:
					return string.Format(CultureInfo.InvariantCulture, "Math.Sinh({0})", operands[0]);
				case FunctionOperatorType.Tanh:
					return string.Format(CultureInfo.InvariantCulture, "Math.Tanh({0})", operands[0]);
				case FunctionOperatorType.Power:
					return string.Format(CultureInfo.InvariantCulture, "Math.Pow({0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Max:
					return string.Format(CultureInfo.InvariantCulture, "Math.Max({0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.Min:
					return string.Format(CultureInfo.InvariantCulture, "Math.Min({0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.IsNull:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "(({0}) == null)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "iif(({0}) == null, {1}, {0})", operands[0], operands[1]);
					}
					throw new NotSupportedException();
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) == null or ({0}).Length = 0)", operands[0]);
				case FunctionOperatorType.Iif: {
						if (operands.Length < 3 || (operands.Length % 2) == 0) throw new ArgumentException(BaseFormatterHelper.STR_TheIifFunctionOperatorRequiresThreeOrMoreArgumen);
						if(operands.Length == 3)
							return string.Format(CultureInfo.InvariantCulture, "iif({0}, {1}, {2})", operands[0], operands[1], operands[2]);
						StringBuilder sb = new StringBuilder();
						int index = -2;
						int counter = 0;
						do {
							index += 2;
							sb.AppendFormat("iif({0}, {1}, ", operands[index], operands[index + 1]);
							counter++;
						} while((index + 3) < operands.Length);
						sb.AppendFormat("{0}", operands[index + 2]);
						sb.Append(new string(')', counter));
						return sb.ToString();
					}
				case FunctionOperatorType.Round:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "Math.Round({0}, 0)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "Math.Round({0}, {1})", operands[0], operands[1]);
					}
					throw new NotSupportedException();
				case FunctionOperatorType.Sign:
					return string.Format(CultureInfo.InvariantCulture, "Math.Sign({0})", operands[0]);
				case FunctionOperatorType.Floor:
					return string.Format(CultureInfo.InvariantCulture, "Math.Floor({0})", operands[0]);
				case FunctionOperatorType.Ceiling:
					return string.Format(CultureInfo.InvariantCulture, "Math.Ceiling({0})", operands[0]);
				case FunctionOperatorType.Trim:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Trim()", operands[0]);
				case FunctionOperatorType.Replace:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Replace({0}, {1})", operands[0], operands[1]);
				case FunctionOperatorType.ToStr:
					return string.Format(CultureInfo.InvariantCulture, "({0}).ToString()", operands[0]);
				case FunctionOperatorType.Concat:
					if(operands.Length < 2) { throw new InvalidOperationException(); }
					string result = string.Empty;
					for(int i = 0; i < operands.Length; i++) {
						if(i != 0) { result += " + "; }
						result += operands[i];
					}
					return result;
				case FunctionOperatorType.Contains:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Contains({1})", operands[0], operands[1]);
				case FunctionOperatorType.StartsWith:
					return string.Format(CultureInfo.InvariantCulture, "({0}).StartsWith({1})", operands[0], operands[1]);
				case FunctionOperatorType.EndsWith:
					return string.Format(CultureInfo.InvariantCulture, "({0}).EndsWith({1})", operands[0], operands[1]);
				case FunctionOperatorType.Upper:
					return string.Format(CultureInfo.InvariantCulture, "({0}).ToUpper()", operands[0]);
				case FunctionOperatorType.Lower:
					return string.Format(CultureInfo.InvariantCulture, "({0}).ToLower()", operands[0]);
				case FunctionOperatorType.Insert:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Insert({1}, {2})", operands[0], operands[1], operands[2]);
				case FunctionOperatorType.Remove:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "({0}).Remove({1})", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "({0}).Remove({1}, {2})", operands[0], operands[1], operands[2]);
					}
					throw new NotSupportedException();
				case FunctionOperatorType.Substring:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "({0}).Substring({1})", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "({0}).Substring({1}, {2})", operands[0], operands[1], operands[2]);
					}
					throw new NotSupportedException();
				case FunctionOperatorType.CharIndex:
					switch(operands.Length) {
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "({0}).IndexOf({1})", operands[0], operands[1]);
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "({0}).IndexOf({1}, {2})", operands[0], operands[1], operands[2]);
						case 4:
							return string.Format(CultureInfo.InvariantCulture, "({0}).IndexOf({1}, {2}, {3})", operands[0], operands[1], operands[2], operands[3]);
					}
					throw new NotSupportedException();
				case FunctionOperatorType.PadLeft:
					if(operands.Length != 2) { throw new NotSupportedException(); }
					return string.Format(CultureInfo.InvariantCulture, "({0}).PadLeft({1})", operands[0], operands[1]);
				case FunctionOperatorType.PadRight:
					if(operands.Length != 2) { throw new NotSupportedException(); }
					return string.Format(CultureInfo.InvariantCulture, "({0}).PadRight({1})", operands[0], operands[1]);
				case FunctionOperatorType.GetMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.Millisecond", operands[0]);
				case FunctionOperatorType.GetSecond:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.Second", operands[0]);
				case FunctionOperatorType.GetMinute:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.Minute", operands[0]);
				case FunctionOperatorType.GetHour:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.Hour", operands[0]);
				case FunctionOperatorType.GetDay:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.Day", operands[0]);
				case FunctionOperatorType.GetMonth:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.Month", operands[0]);
				case FunctionOperatorType.GetYear:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.Year", operands[0]);
				case FunctionOperatorType.GetTimeOfDay:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.TimeOfDay", operands[0]);
				case FunctionOperatorType.GetDayOfWeek:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.DayOfWeek", operands[0]);
				case FunctionOperatorType.GetDayOfYear:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.DayOfYear", operands[0]);
				case FunctionOperatorType.GetDate:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.Date", operands[0]);
				case FunctionOperatorType.AddTicks:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.AddTicks({1})", operands[0], operands[1]);
				case FunctionOperatorType.AddMilliSeconds:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.AddMilliSeconds({1})", operands[0], operands[1]);
				case FunctionOperatorType.AddTimeSpan:
				case FunctionOperatorType.AddSeconds:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.AddSeconds({1})", operands[0], operands[1]);
				case FunctionOperatorType.AddMinutes:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.AddMinutes({1})", operands[0], operands[1]);
				case FunctionOperatorType.AddHours:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.AddHours({1})", operands[0], operands[1]);
				case FunctionOperatorType.AddDays:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.AddDays({1})", operands[0], operands[1]);
				case FunctionOperatorType.AddMonths:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.AddMonths({1})", operands[0], operands[1]);
				case FunctionOperatorType.AddYears:
					return string.Format(CultureInfo.InvariantCulture, "({0}).Value.AddYears({1})", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffDay:
					return string.Format(CultureInfo.InvariantCulture, "Math.Ceiling((({0}).Value - ({1}).Value).TotalDays)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffHour:
					return string.Format(CultureInfo.InvariantCulture, "Math.Ceiling((({0}).Value - ({1}).Value).TotalHours)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMilliSecond:
					return string.Format(CultureInfo.InvariantCulture, "Math.Ceiling((({0}).Value - ({1}).Value).TotalMilliseconds)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMinute:
					return string.Format(CultureInfo.InvariantCulture, "Math.Ceiling((({0}).Value - ({1}).Value).TotalMinutes)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffMonth:
					return string.Format(CultureInfo.InvariantCulture, "Math.Ceiling((({0}).Value - ({1}).Value).TotalMonths)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffSecond:
					return string.Format(CultureInfo.InvariantCulture, "Math.Ceiling((({0}).Value - ({1}).Value).TotalSeconds)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffTick:
					return string.Format(CultureInfo.InvariantCulture, "(Math.Ceiling((({0}).Value - ({1}).Value).TotalSeconds) * 10000000)", operands[0], operands[1]);
				case FunctionOperatorType.DateDiffYear:
					return string.Format(CultureInfo.InvariantCulture, "Math.Ceiling((({0}).Value - ({1}).Value).TotalYears)", operands[0], operands[1]);
				case FunctionOperatorType.Now:
				case FunctionOperatorType.Today:
				case FunctionOperatorType.Rnd:
				case FunctionOperatorType.Ascii:
				case FunctionOperatorType.Char:
				case FunctionOperatorType.Reverse:
					throw new NotSupportedException();
			}
			return null;
		}
		public static string FormatTable(string schema, string tableName) {
			return string.Format(CultureInfo.InvariantCulture, "{0}", tableName);
		}
		public static string FormatTable(string schema, string tableName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", tableName, tableAlias);
		}
	}
	public static class DataSetFormatterHelper {
		public static string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			switch(operatorType) {
				case BinaryOperatorType.BitwiseXor:
				case BinaryOperatorType.BitwiseOr:
				case BinaryOperatorType.BitwiseAnd:
					throw new NotSupportedException();
				default:
					return BaseFormatterHelper.DefaultFormatBinary(operatorType, leftOperand, rightOperand);
			}
		}
		public static string FormatColumn(string columnName) {
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", columnName);
		}
		public static string FormatColumn(string columnName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "{1}.[{0}]", columnName, tableAlias);
		}
		public static string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.IsNull:
					switch(operands.Length) {
						case 1:
							return string.Format(CultureInfo.InvariantCulture, "(({0}) is null)", operands[0]);
						case 2:
							return string.Format(CultureInfo.InvariantCulture, "iif(({0}) is null, {1}, {0})", operands[0], operands[1]);
					}
					throw new NotSupportedException();
				case FunctionOperatorType.IsNullOrEmpty:
					return string.Format(CultureInfo.InvariantCulture, "(({0}) is null or (len({0}) = 0))", operands[0]);
				case FunctionOperatorType.Iif: {
						if (operands.Length < 3 || (operands.Length % 2) == 0) throw new ArgumentException(BaseFormatterHelper.STR_TheIifFunctionOperatorRequiresThreeOrMoreArgumen);
						if(operands.Length == 3)
							return string.Format(CultureInfo.InvariantCulture, "iif({0}, {1}, {2})", operands[0], operands[1], operands[2]);
						StringBuilder sb = new StringBuilder();
						int index = -2;
						int counter = 0;
						do {
							index += 2;
							sb.AppendFormat("iif({0}, {1}, ", operands[index], operands[index + 1]);
							counter++;
						} while((index + 3) < operands.Length);
						sb.AppendFormat("{0}", operands[index + 2]);
						sb.Append(new string(')', counter));
						return sb.ToString();
					}
				case FunctionOperatorType.Trim:
					return string.Format(CultureInfo.InvariantCulture, "trim({0})", operands[0]);
				case FunctionOperatorType.Concat: {
						if(operands.Length < 2) { throw new InvalidOperationException(); }
						string result = string.Empty;
						for(int i = 0; i < operands.Length; i++) {
							if(i != 0) { result += " + "; }
							result += operands[i];
						}
						return result;
					}
				case FunctionOperatorType.Substring:
					switch(operands.Length) {
						case 3:
							return string.Format(CultureInfo.InvariantCulture, "({0}).Substring({1}, {2})", operands[0], operands[1], operands[2]);
					}
					throw new NotSupportedException();
				case FunctionOperatorType.StartsWith:
					return string.Format(CultureInfo.InvariantCulture, "({0} like '{1}%')", operands[0], operands[1].Trim('\''));
				case FunctionOperatorType.EndsWith:
					return string.Format(CultureInfo.InvariantCulture, "({0} like '%{1}')", operands[0], operands[1].Trim('\''));
				case FunctionOperatorType.Contains:
					return string.Format(CultureInfo.InvariantCulture, "({0} like '%{1}%')", operands[0], operands[1].Trim('\''));
			}
			return null;
		}
		public static string FormatTable(string schema, string tableName) {
			return string.Format(CultureInfo.InvariantCulture, "{0}", tableName);
		}
		public static string FormatTable(string schema, string tableName, string tableAlias) {
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", tableName, tableAlias);
		}
	}
}
namespace DevExpress.Data.Filtering {
	using DevExpress.Data.Filtering.Helpers;
	public static class CriteriaToWhereClauseHelper {
		public static string GetAccessWhere(CriteriaOperator op) {
			return new AccessWhereGenerator().Process(op);
		}
		public static string GetMsSqlWhere(CriteriaOperator op) {
			return GetMsSqlWhere(op, new MsSqlFormatterHelper.MSSqlServerVersion(true, false, null));
		}
		public static string GetMsSqlWhere(CriteriaOperator op, MsSqlFormatterHelper.MSSqlServerVersion sqlServerVersion) {
			return new MsSqlWhereGenerator(sqlServerVersion).Process(op);
		}
		public static string GetOracleWhere(CriteriaOperator op) {
			return GetOracleWhere(op, false);
		}
		public static string GetOracleWhere(CriteriaOperator op, bool useQuotesOnOperandProperties) {
			return new OracleWhereGenerator(useQuotesOnOperandProperties).Process(op);
		}
		public static string GetDynamicLinqWhere(CriteriaOperator op) {
			return new DynamicLinqWhereGenerator().Process(op);
		}
		public static string GetDataSetWhere(CriteriaOperator op) {
			return new DataSetWhereGenerator().Process(op);
		}
	}
}
namespace DevExpress.Data.Filtering.Helpers {
	public abstract class BaseWhereGenerator : IClientCriteriaVisitor<string> {
		public string Process(CriteriaOperator operand) {
			if(ReferenceEquals(operand, null))
				return string.Empty;
			return operand.Accept(this);
		}
		string IClientCriteriaVisitor<string>.Visit(OperandProperty theOperand) {
			return FormatOperandProperty(theOperand);
		}
		protected abstract string FormatOperandProperty(OperandProperty operand);
		string ICriteriaVisitor<string>.Visit(OperandValue theOperand) {
			return theOperand.ToString();
		}
		string ICriteriaVisitor<string>.Visit(BetweenOperator theOperator) {
			return Process(GroupOperator.And(
				new BinaryOperator(theOperator.TestExpression, theOperator.BeginExpression, BinaryOperatorType.GreaterOrEqual),
				new BinaryOperator(theOperator.TestExpression, theOperator.EndExpression, BinaryOperatorType.LessOrEqual))
			);
		}
		string ICriteriaVisitor<string>.Visit(InOperator theOperator) {
			string left = Process(theOperator.LeftOperand);
			StringBuilder inString = new StringBuilder();
			foreach(CriteriaOperator value in theOperator.Operands) {
				if(inString.Length > 0)
					inString.Append(',');
				inString.Append(Process(value));
			}
			return String.Format(CultureInfo.InvariantCulture, "{0} in ({1})",
				left, inString.ToString());
		}
		string ICriteriaVisitor<string>.Visit(GroupOperator theOperator) {
			StringCollection list = new StringCollection();
			foreach(CriteriaOperator cr in theOperator.Operands) {
				string crs = Process(cr);
				if(crs != null)
					list.Add(crs);
			}
			return "(" + StringListHelper.DelimitedText(list, " " + theOperator.OperatorType.ToString() + " ") + ")";
		}
		string ICriteriaVisitor<string>.Visit(UnaryOperator theOperator) {
			return BaseFormatterHelper.DefaultFormatUnary(theOperator.OperatorType, Process(theOperator.Operand));
		}
		string ICriteriaVisitor<string>.Visit(BinaryOperator theOperator) {
			throw new NotImplementedException();
		}
		string ICriteriaVisitor<string>.Visit(FunctionOperator theOperator) {
			switch(theOperator.OperatorType){
				case FunctionOperatorType.LocalDateTimeThisYear:
				case FunctionOperatorType.LocalDateTimeThisMonth:
				case FunctionOperatorType.LocalDateTimeLastWeek:
				case FunctionOperatorType.LocalDateTimeThisWeek:
				case FunctionOperatorType.LocalDateTimeYesterday:
				case FunctionOperatorType.LocalDateTimeToday:
				case FunctionOperatorType.LocalDateTimeNow:
				case FunctionOperatorType.LocalDateTimeTomorrow:
				case FunctionOperatorType.LocalDateTimeDayAfterTomorrow:
				case FunctionOperatorType.LocalDateTimeNextWeek:
				case FunctionOperatorType.LocalDateTimeTwoWeeksAway:
				case FunctionOperatorType.LocalDateTimeNextMonth:
				case FunctionOperatorType.LocalDateTimeNextYear:
					if(theOperator.Operands.Count != 0)
						throw new ArgumentException("theOperator.Operands.Count != 0");
					return Process(new ConstantValue(DevExpress.Data.Filtering.Helpers.EvalHelpers.EvaluateLocalDateTime(theOperator.OperatorType)));
				case FunctionOperatorType.IsThisMonth:
				case FunctionOperatorType.IsThisWeek:
				case FunctionOperatorType.IsThisYear:
					return Process(DevExpress.Data.Filtering.Helpers.EvalHelpers.ExpandIsOutlookInterval(theOperator));
			}
			return VisitInternal(theOperator);
		}
		protected virtual string VisitInternal(FunctionOperator theOperator) {
			if(theOperator.OperatorType == FunctionOperatorType.Custom || theOperator.OperatorType == FunctionOperatorType.CustomNonDeterministic) {
				return FormatCustomFunction(theOperator);
			}
			CriteriaOperator result;
			if(IsLocalDateTimeOrOutlookInterval(theOperator, out result)) {
				return Process(result);
			}
			string[] operands = new string[theOperator.Operands.Count];
			for(int i = 0; i < theOperator.Operands.Count; ++i) {
				operands[i] = Process((CriteriaOperator)theOperator.Operands[i]);
			}
			string resultStr = BaseFormatterHelper.DefaultFormatFunction(theOperator.OperatorType, operands);
			if(resultStr != null) return resultStr;
			throw new NotImplementedException();
		}
		string FormatCustomFunction(FunctionOperator customOperator) {
			if(customOperator.Operands.Count < 1)
				throw new ArgumentException("customOperator.Operands.Count < 1");
			var ov = customOperator.Operands[0] as OperandValue;
			if(ReferenceEquals(null, ov))
				throw new ArgumentNullException("customOperator.Operands[0] as OperandValue");
			string functionName = ov.Value as string;
			if(functionName == null)
				throw new ArgumentException("functionName (customOperator.Operands[0].Value is not string)");
			ICustomFunctionOperator customFunction = CriteriaOperator.GetCustomFunction(functionName);
			if(customFunction == null)
				throw new InvalidOperationException(string.Format("Undefined custom function '{0}'", functionName));
			ICustomFunctionOperatorFormattable formattable = customFunction as ICustomFunctionOperatorFormattable;
			if(formattable == null)
				throw new InvalidOperationException(string.Format("Custom function '{0}' does not implement ICustomFunctionOperatorFormattable", functionName));
			return formattable.Format(this.GetType(), customOperator.Operands.Skip(1).Select(x => Process(x)).ToArray());
		}
		string IClientCriteriaVisitor<string>.Visit(AggregateOperand theOperand) {
			throw new NotSupportedException("AggregateOperand");
		}
		string IClientCriteriaVisitor<string>.Visit(JoinOperand theOperand) {
			throw new NotSupportedException("JoinOperand");
		}
		static bool IsLocalDateTimeOrOutlookInterval(FunctionOperator op, out CriteriaOperator res) {
			res = null;
			switch(op.OperatorType) {
				case FunctionOperatorType.LocalDateTimeThisYear:
				case FunctionOperatorType.LocalDateTimeThisMonth:
				case FunctionOperatorType.LocalDateTimeLastWeek:
				case FunctionOperatorType.LocalDateTimeThisWeek:
				case FunctionOperatorType.LocalDateTimeYesterday:
				case FunctionOperatorType.LocalDateTimeToday:
				case FunctionOperatorType.LocalDateTimeNow:
				case FunctionOperatorType.LocalDateTimeTomorrow:
				case FunctionOperatorType.LocalDateTimeDayAfterTomorrow:
				case FunctionOperatorType.LocalDateTimeNextWeek:
				case FunctionOperatorType.LocalDateTimeTwoWeeksAway:
				case FunctionOperatorType.LocalDateTimeNextMonth:
				case FunctionOperatorType.LocalDateTimeNextYear:
					if(op.Operands.Count != 0)
						throw new ArgumentException("theOperator.Operands.Count != 0");
					res = new OperandValue(EvalHelpers.EvaluateLocalDateTime(op.OperatorType));
					return true;
				case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
				case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
				case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
				case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
				case FunctionOperatorType.IsOutlookIntervalNextWeek:
				case FunctionOperatorType.IsOutlookIntervalTomorrow:
				case FunctionOperatorType.IsOutlookIntervalToday:
				case FunctionOperatorType.IsOutlookIntervalYesterday:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
				case FunctionOperatorType.IsOutlookIntervalLastWeek:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
				case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
					res = EvalHelpers.ExpandIsOutlookInterval(op);
					return true;
			}
			return false;
		}
	}
	public class AccessWhereGenerator : BaseWhereGenerator, ICriteriaVisitor<string> {
		string ICriteriaVisitor<string>.Visit(BinaryOperator theOperator) {
			string left = Process(theOperator.LeftOperand);
			string right = Process(theOperator.RightOperand);
			return AccessFormatterHelper.FormatBinary(theOperator.OperatorType, left, right);
		}
		protected override string VisitInternal(FunctionOperator theOperator) {
			string result = AccessFormatterHelper.FormatFunction(new ProcessParameter(delegate(object obj) {
				return Process((CriteriaOperator)obj);
			}), theOperator.OperatorType, theOperator.Operands.ToArray());
			if(result != null) return result;
			string[] operands = new string[theOperator.Operands.Count];
			for(int i = 0; i < theOperator.Operands.Count; ++i) {
				operands[i] = Process((CriteriaOperator)theOperator.Operands[i]);
			}
			result = AccessFormatterHelper.FormatFunction(theOperator.OperatorType, operands);
			if(result != null) return result;
			return base.VisitInternal(theOperator);
		}
		const string nullString = "null";
		string ICriteriaVisitor<string>.Visit(OperandValue theOperand) {
			object value = theOperand.Value;
			if(value == null)
				return nullString;
			TypeCode tc = DXTypeExtensions.GetTypeCode(value.GetType());
			switch(tc) {
				case DXTypeExtensions.TypeCodeDBNull:
				case TypeCode.Empty:
					return nullString;
				case TypeCode.Boolean:
					return ((bool)value) ? "True" : "False";
				case TypeCode.Char:
					return "'" + (char)value + "'";
				case TypeCode.DateTime:
					DateTime datetimeValue = (DateTime)value;
					string dateTimeFormatPattern;
					if(datetimeValue.TimeOfDay == TimeSpan.Zero) {
						dateTimeFormatPattern = "MM/dd/yyyy";
					} else {
						dateTimeFormatPattern = "MM/dd/yyyy HH:mm:ss";
					}
					return "#" + datetimeValue.ToString(dateTimeFormatPattern, CultureInfo.InvariantCulture) + "#";
				case TypeCode.String:
					return AsString(value);
				case TypeCode.Decimal:
					return FixNonFixedText(((Decimal)value).ToString(CultureInfo.InvariantCulture));
				case TypeCode.Double:
					return FixNonFixedText(((Double)value).ToString("r", CultureInfo.InvariantCulture));
				case TypeCode.Single:
					return FixNonFixedText(((Single)value).ToString("r", CultureInfo.InvariantCulture));
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
					if(value is Enum)
						return Convert.ToInt64(value).ToString();
					return value.ToString();
				case TypeCode.UInt64:
					if(value is Enum)
						return Convert.ToUInt64(value).ToString();
					return value.ToString();
				case TypeCode.Object:
				default:
					if(value is Guid) {
#if DXRESTRICTED
						return "{" + ((Guid)value).ToString() + "}";
#else
						return ((Guid)value).ToString("B", CultureInfo.InvariantCulture);
#endif
					} else if(value is TimeSpan) {
						return FixNonFixedText(((TimeSpan)value).TotalSeconds.ToString("r", CultureInfo.InvariantCulture));
					} else {
						throw new ArgumentException(value.ToString());
					}
			}
		}
		static string AsString(object value) {
			return "'" + value.ToString().Replace("'", "''") + "'";
		}
		static string FixNonFixedText(string toFix) {
			if(toFix.IndexOfAny(new char[] { '.', 'e', 'E' }) < 0)
				toFix += ".0";
			return toFix;
		}
		protected override string FormatOperandProperty(OperandProperty operand) {
			return string.Format("[{0}]", operand.PropertyName);
		}
	}
	public class MsSqlWhereGenerator : BaseWhereGenerator, ICriteriaVisitor<string> {
		MsSqlFormatterHelper.MSSqlServerVersion sqlServerVersion;
		public MsSqlWhereGenerator(MsSqlFormatterHelper.MSSqlServerVersion sqlServerVersion) {
			this.sqlServerVersion = sqlServerVersion;
		}
		const string nullString = "null";
		string ICriteriaVisitor<string>.Visit(OperandValue theOperand) {
			object value = theOperand.Value;
			if(value == null)
				return nullString;
			TypeCode tc = DXTypeExtensions.GetTypeCode(value.GetType());
			switch(tc) {
				case DXTypeExtensions.TypeCodeDBNull:
				case TypeCode.Empty:
					return nullString;
				case TypeCode.Boolean:
					return ((bool)value) ? "1" : "0";
				case TypeCode.Char:
					return "'" + (char)value + "'";
				case TypeCode.DateTime:
					DateTime datetimeValue = (DateTime)value;
					string dateTimeFormatPattern;
					if(datetimeValue.TimeOfDay == TimeSpan.Zero) {
						dateTimeFormatPattern = "yyyyMMdd";
					} else if(datetimeValue.Millisecond == 0) {
						dateTimeFormatPattern = "yyyyMMdd HH:mm:ss";
					} else {
						dateTimeFormatPattern = "yyyyMMdd HH:mm:ss.fff";
					}
					return "Cast('" + datetimeValue.ToString(dateTimeFormatPattern, CultureInfo.InvariantCulture) + "' as datetime)";
				case TypeCode.String:
					return AsString(value);
				case TypeCode.Decimal:
					return FixNonFixedText(((Decimal)value).ToString(CultureInfo.InvariantCulture));
				case TypeCode.Double:
					return FixNonFixedText(((Double)value).ToString("r", CultureInfo.InvariantCulture));
				case TypeCode.Single:
					return FixNonFixedText(((Single)value).ToString("r", CultureInfo.InvariantCulture));
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
					if(value is Enum)
						return Convert.ToInt64(value).ToString();
					return value.ToString();
				case TypeCode.UInt64:
					if(value is Enum)
						return Convert.ToUInt64(value).ToString();
					return value.ToString();
				case TypeCode.Object:
				default:
					if(value is Guid) {
						return "Cast('" + ((Guid)value).ToString() + "' as uniqueidentifier)";
					} else if(value is TimeSpan) {
						return FixNonFixedText(((TimeSpan)value).TotalSeconds.ToString("r", CultureInfo.InvariantCulture));
					} else {
						throw new ArgumentException(value.ToString());
					}
			}
		}
		static string AsString(object value) {
			return "N'" + value.ToString().Replace("'", "''") + "'";
		}
		static string FixNonFixedText(string toFix) {
			if(toFix.IndexOfAny(new char[] { '.', 'e', 'E' }) < 0)
				toFix += ".0";
			return toFix;
		}
		string ICriteriaVisitor<string>.Visit(BinaryOperator theOperator) {
			string left = Process(theOperator.LeftOperand);
			string right = Process(theOperator.RightOperand);
			return MsSqlFormatterHelper.FormatBinary(theOperator.OperatorType, left, right);
		}
		protected override string VisitInternal(FunctionOperator theOperator) {
			string result = MsSqlFormatterHelper.FormatFunction(new ProcessParameter(delegate(object obj) {
				return Process((CriteriaOperator)obj);
			}), theOperator.OperatorType, sqlServerVersion, theOperator.Operands.ToArray());
			if(result != null) return result;
			string[] operands = new string[theOperator.Operands.Count];
			for(int i = 0; i < theOperator.Operands.Count; ++i) {
				operands[i] = Process((CriteriaOperator)theOperator.Operands[i]);
			}
			result = MsSqlFormatterHelper.FormatFunction(theOperator.OperatorType, sqlServerVersion, operands);
			if(result != null) return result;
			return base.VisitInternal(theOperator);
		}
		protected override string FormatOperandProperty(OperandProperty operand) {
			return string.Format("\"{0}\"", operand.PropertyName);
		}
	}
	public class OracleWhereGenerator : BaseWhereGenerator, ICriteriaVisitor<string> {
		bool useQuotesOnOperandProperties;
		public OracleWhereGenerator(bool useQuotesOnOperandProperties) {
			this.useQuotesOnOperandProperties = useQuotesOnOperandProperties;
		}
		const string nullString = "NULL";
		string ICriteriaVisitor<string>.Visit(OperandValue theOperand) {
			object value = theOperand.Value;
			if(value == null)
				return nullString;
			TypeCode tc = DXTypeExtensions.GetTypeCode(value.GetType());
			switch(tc) {
				case DXTypeExtensions.TypeCodeDBNull:
				case TypeCode.Empty:
					return nullString;
				case TypeCode.Boolean:
					return ((bool)value) ? "1" : "0";
				case TypeCode.Char:
					return "'" + (char)value + "'";
				case TypeCode.DateTime:
					DateTime datetimeValue = (DateTime)value;
					string dateTimeFormatPattern;
					string dateTimeFormatPatternQuery;
					if(datetimeValue.TimeOfDay == TimeSpan.Zero) {
						dateTimeFormatPattern = "yyyy/MM/dd";
						dateTimeFormatPatternQuery = "yyyy/mm/dd";
					} else {
						dateTimeFormatPattern = "yyyy/MM/dd:HH:mm:ss";
						dateTimeFormatPatternQuery = "yyyy/mm/dd:hh24:mi:ss";
					}
					return "TO_DATE('" + datetimeValue.ToString(dateTimeFormatPattern, CultureInfo.InvariantCulture) + "', '"
						+ dateTimeFormatPatternQuery + "')";
				case TypeCode.String:
					return AsString(value);
				case TypeCode.Decimal:
					return FixNonFixedText(((Decimal)value).ToString(CultureInfo.InvariantCulture));
				case TypeCode.Double:
					return FixNonFixedText(((Double)value).ToString("r", CultureInfo.InvariantCulture));
				case TypeCode.Single:
					return FixNonFixedText(((Single)value).ToString("r", CultureInfo.InvariantCulture));
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
					if(value is Enum)
						return Convert.ToInt64(value).ToString();
					return value.ToString();
				case TypeCode.UInt64:
					if(value is Enum)
						return Convert.ToUInt64(value).ToString();
					return value.ToString();
				case TypeCode.Object:
				default:
					if(value is Guid) {
						return "'" + ((Guid)value).ToString() + "'";
					} else if(value is TimeSpan) {
						return FixNonFixedText(((TimeSpan)value).TotalSeconds.ToString("r", CultureInfo.InvariantCulture));
					} else {
						throw new ArgumentException(value.ToString());
					}
			}
		}
		static string AsString(object value) {
			return "'" + value.ToString().Replace("'", "''") + "'";
		}
		static string FixNonFixedText(string toFix) {
			if(toFix.IndexOfAny(new char[] { '.', 'e', 'E' }) < 0)
				toFix += ".0";
			return toFix;
		}
		string ICriteriaVisitor<string>.Visit(BinaryOperator theOperator) {
			string left = Process(theOperator.LeftOperand);
			string right = Process(theOperator.RightOperand);
			return OracleFormatterHelper.FormatBinary(theOperator.OperatorType, left, right);
		}
		protected override string VisitInternal(FunctionOperator theOperator) {
			string result = OracleFormatterHelper.FormatFunction(new ProcessParameter(delegate(object obj) {
				return Process((CriteriaOperator)obj);
			}), theOperator.OperatorType, theOperator.Operands.ToArray());
			if(result != null) return result;
			string[] operands = new string[theOperator.Operands.Count];
			for(int i = 0; i < theOperator.Operands.Count; ++i) {
				operands[i] = Process((CriteriaOperator)theOperator.Operands[i]);
			}
			result = OracleFormatterHelper.FormatFunction(theOperator.OperatorType, operands);
			if(result != null) return result;
			return base.VisitInternal(theOperator);
		}
		protected override string FormatOperandProperty(OperandProperty operand) {
			string quote = useQuotesOnOperandProperties ? "\"" : string.Empty;
			return string.Format("{1}{0}{1}", operand.PropertyName, quote);
		}
	}
	public class DynamicLinqWhereGenerator : BaseWhereGenerator, ICriteriaVisitor<string> {
		string ICriteriaVisitor<string>.Visit(OperandValue theOperand) {
			object value = theOperand.Value;
			if(value != null){
				Type type = value.GetType();
				switch(DXTypeExtensions.GetTypeCode(type)){
					case TypeCode.Boolean:
						return (bool)value ? "true" : "false";
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.SByte:
					case TypeCode.UInt16:
					case TypeCode.UInt32:
					case TypeCode.UInt64:
						return value.ToString();
					case TypeCode.String:
						return string.Concat("\"", (string)value, "\"");
					case TypeCode.Single:
						return ((float)value).ToString("R", CultureInfo.InvariantCulture);
					case TypeCode.Double:
						return ((double)value).ToString("R", CultureInfo.InvariantCulture);
					case TypeCode.Decimal:
						return ((decimal)value).ToString("G", CultureInfo.InvariantCulture);
					case TypeCode.DateTime: {
							DateTime dt = (DateTime)value;
							if(dt.TimeOfDay.Ticks == 0)
								return string.Format("DateTime({0}, {1}, {2})", dt.Year, dt.Month, dt.Day);
							else
								return string.Format("DateTime({0}, {1}, {2}, {3}, {4}, {5})", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
						}
				}
			}
			return string.Format("\"{0}\"", theOperand.Value);
		}
		string ICriteriaVisitor<string>.Visit(BinaryOperator theOperator) {
			string left = Process(theOperator.LeftOperand);
			string right = Process(theOperator.RightOperand);
			return DynamicLinqFormatterHelper.FormatBinary(theOperator.OperatorType, left, right);
		}
		protected override string VisitInternal(FunctionOperator theOperator) {
			string[] operands = new string[theOperator.Operands.Count];
			for(int i = 0; i < theOperator.Operands.Count; ++i) {
				operands[i] = Process((CriteriaOperator)theOperator.Operands[i]);
			}
			string result = DynamicLinqFormatterHelper.FormatFunction(theOperator.OperatorType, operands);
			if(result != null) return result;
			return base.VisitInternal(theOperator);
		}
		string ICriteriaVisitor<string>.Visit(UnaryOperator theOperator) {
			return DynamicLinqFormatterHelper.FormatUnary(theOperator.OperatorType, Process(theOperator.Operand));
		}
		string ICriteriaVisitor<string>.Visit(InOperator theOperator) {
			if(theOperator.Operands == null || theOperator.Operands.Count == 0)
				return string.Empty;
			if(theOperator.Operands.Count == 1)
				return Process(new BinaryOperator(theOperator.LeftOperand, theOperator.Operands[0], BinaryOperatorType.Equal));
			GroupOperator result = new GroupOperator(GroupOperatorType.Or);
			foreach(CriteriaOperator operand in theOperator.Operands) {
				result.Operands.Add(new BinaryOperator(theOperator.LeftOperand, operand, BinaryOperatorType.Equal));
			}
			return Process(result);
		}
		protected override string FormatOperandProperty(OperandProperty operand) {
			return operand.PropertyName;
		}
	}
	public class DataSetWhereGenerator : BaseWhereGenerator, ICriteriaVisitor<string> {
		string ICriteriaVisitor<string>.Visit(BinaryOperator theOperator) {
			string left = Process(theOperator.LeftOperand);
			string right = Process(theOperator.RightOperand);
			return DataSetFormatterHelper.FormatBinary(theOperator.OperatorType, left, right);
		}
		protected override string VisitInternal(FunctionOperator theOperator) {
			string[] operands = new string[theOperator.Operands.Count];
			for(int i = 0; i < theOperator.Operands.Count; ++i) {
				operands[i] = Process((CriteriaOperator)theOperator.Operands[i]);
			}
			string result = DataSetFormatterHelper.FormatFunction(theOperator.OperatorType, operands);
			if(result != null) return result;
			return base.VisitInternal(theOperator);
		}
		const string nullString = "null";
		string ICriteriaVisitor<string>.Visit(OperandValue theOperand) {
			object value = theOperand.Value;
			if(value == null)
				return nullString;
			TypeCode tc = DXTypeExtensions.GetTypeCode(value.GetType());
			switch(tc) {
				case DXTypeExtensions.TypeCodeDBNull:
				case TypeCode.Empty:
					return nullString;
				case TypeCode.Boolean:
					return ((bool)value) ? "true" : "false";
				case TypeCode.Char:
					return "'" + (char)value + "'";
				case TypeCode.DateTime:
					DateTime datetimeValue = (DateTime)value;
					string dateTimeFormatPattern;
					if(datetimeValue.TimeOfDay == TimeSpan.Zero) {
						dateTimeFormatPattern = "MM/dd/yyyy";
					} else {
						dateTimeFormatPattern = "MM/dd/yyyy HH:mm:ss";
					}
					return "#" + datetimeValue.ToString(dateTimeFormatPattern, CultureInfo.InvariantCulture) + "#";
				case TypeCode.String:
					return AsString(value);
				case TypeCode.Decimal:
					return FixNonFixedText(((Decimal)value).ToString(CultureInfo.InvariantCulture));
				case TypeCode.Double:
					return FixNonFixedText(((Double)value).ToString("r", CultureInfo.InvariantCulture));
				case TypeCode.Single:
					return FixNonFixedText(((Single)value).ToString("r", CultureInfo.InvariantCulture));
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
					if(value is Enum)
						return Convert.ToInt64(value).ToString();
					return value.ToString();
				case TypeCode.UInt64:
					if(value is Enum)
						return Convert.ToUInt64(value).ToString();
					return value.ToString();
				case TypeCode.Object:
				default:
					if(value is Guid) {
						return "CONVERT('{" + ((Guid)value).ToString() + "}','System.Guid')";
					} else if(value is TimeSpan) {
						return FixNonFixedText(((TimeSpan)value).TotalSeconds.ToString("r", CultureInfo.InvariantCulture));
					} else {
						throw new ArgumentException(value.ToString());
					}
			}
		}
		static string AsString(object value) {
			return "'" + value.ToString().Replace("'", "''") + "'";
		}
		static string FixNonFixedText(string toFix) {
			if(toFix.IndexOfAny(new char[] { '.', 'e', 'E' }) < 0)
				toFix += ".0";
			return toFix;
		}
		protected override string FormatOperandProperty(OperandProperty operand) {
			return string.Format("[{0}]", operand.PropertyName);
		}
	}
}
