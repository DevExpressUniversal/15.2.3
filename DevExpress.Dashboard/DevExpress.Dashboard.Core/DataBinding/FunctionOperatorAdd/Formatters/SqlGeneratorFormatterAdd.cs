#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Globalization;
using DevExpress.PivotGrid.ServerMode;
namespace DevExpress.DashboardCommon.Native {
	interface ISqlGeneratorFormatterAdd {
		string FormatFunctionAdd(FunctionOperatorTypeAdd operatorType, params string[] operands);
	}
	abstract class SqlGeneratorFormatterAddBase : ISqlGeneratorFormatterAdd {
		public virtual string FormatFunctionAdd(FunctionOperatorTypeAdd operatorType, params string[] operands) {
			return null;
		}
	}
	class AccessFormatterAdd : SqlGeneratorFormatterAddBase {
		public override string FormatFunctionAdd(FunctionOperatorTypeAdd operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorTypeAdd.GetQuarter:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART('q', {0})", operands[0]);
				case FunctionOperatorTypeAdd.GetDateMonthYear:
					return string.Format(CultureInfo.InvariantCulture, "IIF(ISNULL({0}), NULL, DATEADD('m', DATEDIFF('m', #01/01/1970#, {0}), #01/01/1970#))", operands[0]);
				case FunctionOperatorTypeAdd.GetDateQuarterYear:
					return string.Format(CultureInfo.InvariantCulture, "IIF(ISNULL({0}), NULL, DATEADD('q', DATEDIFF('q', #01/01/1970#, {0}), #01/01/1970#))", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHour:
					return string.Format(CultureInfo.InvariantCulture, "IIF(ISNULL({0}), NULL, DATEADD('h', DATEDIFF('h', #01/01/1970#, {0}), #01/01/1970#))", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinute:
					return string.Format(CultureInfo.InvariantCulture, "IIF(ISNULL({0}), NULL, DATEADD('n', DATEDIFF('n', #01/01/1970#, {0}), #01/01/1970#))", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinuteSecond:
					return string.Format(CultureInfo.InvariantCulture, "IIF(ISNULL({0}), NULL, CDATE(FORMAT({0}, 'yyyy-m-d h:n:s')))", operands[0]);
				case FunctionOperatorTypeAdd.GetWeekOfYear:
					int firstWeekOfYear = 1;
					switch (CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule) {
						case CalendarWeekRule.FirstDay :
							firstWeekOfYear = 1;
							break;
						case CalendarWeekRule.FirstFourDayWeek :
							firstWeekOfYear = 2;
							break;
						case CalendarWeekRule.FirstFullWeek :
							firstWeekOfYear = 3;
							break;
					}
					int firstDayOfWeek = 1;
					switch (CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek) {
						case DayOfWeek.Monday :
							firstDayOfWeek = 2;
							break;
						case DayOfWeek.Tuesday :
							firstDayOfWeek = 3;
							break;
						case DayOfWeek.Wednesday :
							firstDayOfWeek = 4;
							break;
						case DayOfWeek.Thursday:
							firstDayOfWeek = 5;
							break;
						case DayOfWeek.Friday:
							firstDayOfWeek = 6;
							break;
						case DayOfWeek.Saturday:
							firstDayOfWeek = 7;
							break;
						case DayOfWeek.Sunday:
							firstDayOfWeek = 1;
							break;
					}
					string initialWeekNumber = string.Format(CultureInfo.InvariantCulture, "DATEPART('ww', {0}, {1}, {2})", operands[0], firstDayOfWeek, firstWeekOfYear);
					if(CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule == CalendarWeekRule.FirstFullWeek)
						return initialWeekNumber;
					string previousWeekNumber = string.Format(CultureInfo.InvariantCulture, "DATEPART('ww', dateadd('ww', -1, {0}), {1}, {2})", operands[0], firstDayOfWeek, firstWeekOfYear);
					return string.Format(CultureInfo.InvariantCulture, "iif(datepart('m', {0}) = 12 and datepart('d', {0}) >= 26 and {1} = 1, {2} + 1, {1})", operands[0], initialWeekNumber, previousWeekNumber);
				}
			return base.FormatFunctionAdd(operatorType, operands);
		}
	}
	class AdvantageFormatterAdd : SqlGeneratorFormatterAddBase {
		public override string FormatFunctionAdd(FunctionOperatorTypeAdd operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorTypeAdd.GetQuarter:
					return string.Format(CultureInfo.InvariantCulture, "QUARTER({0})", operands[0]);
				case FunctionOperatorTypeAdd.GetDateMonthYear:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_MONTH, TIMESTAMPDIFF(SQL_TSI_MONTH, '0001-01-01', {0}), '0001-01-01')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateQuarterYear:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_QUARTER, TIMESTAMPDIFF(SQL_TSI_QUARTER, '0001-01-01', {0}), '0001-01-01')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHour:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_HOUR, TIMESTAMPDIFF(SQL_TSI_HOUR, '1970-01-01', {0}), '1970-01-01')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinute:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_MINUTE, TIMESTAMPDIFF(SQL_TSI_MINUTE, '1970-01-01', {0}), '1970-01-01')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinuteSecond:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_SECOND, SECOND(CAST({0} AS SQL_TIMESTAMP)), TIMESTAMPADD(SQL_TSI_MINUTE, TIMESTAMPDIFF(SQL_TSI_MINUTE, '1970-01-01', {0}), '1970-01-01'))", operands[0]);
			}
			return base.FormatFunctionAdd(operatorType, operands);
		}
	}
	class AsaFormatterAdd : SqlGeneratorFormatterAddBase {
	}
	class AseFormatterAdd : SqlGeneratorFormatterAddBase {
		public override string FormatFunctionAdd(FunctionOperatorTypeAdd operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorTypeAdd.GetQuarter:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(quarter, {0})", operands[0]);
				case FunctionOperatorTypeAdd.GetDateMonthYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(mm, DATEDIFF(mm, '01-01-01', {0}), '01-01-01')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateQuarterYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(qq, DATEDIFF(qq, '01-01-01', {0}), '01-01-01')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHour:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(hh, DATEDIFF(hh, '01-01-01', {0}), '01-01-01')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinute:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(mi, DATEDIFF(mi, '01-01-01', {0}), '01-01-01')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinuteSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(ss, DATEPART(ss, CAST({0} AS DATETIME)), DATEADD(mi, DATEDIFF(mi, '01-01-01', {0}), '01-01-01'))", operands[0]);
			}
			return base.FormatFunctionAdd(operatorType, operands);
		}
	}
	class DB2FormatterAdd : SqlGeneratorFormatterAddBase {
	}
	class FirebirdFormatterAdd : SqlGeneratorFormatterAddBase {
		static string DecodeStrParam(string operand) {
			if(operand.StartsWith("@s")) {
				int strLen = int.Parse(operand.Substring(operand.IndexOf('_') + 1));
				return string.Format(CultureInfo.InvariantCulture, "cast({0} as varchar({1}))", operand, strLen);
			}
			return operand;
		}
		public override string FormatFunctionAdd(FunctionOperatorTypeAdd operatorType, params string[] operands) {
			for(int i = 0; i < operands.Length; i++) {
				operands[i] = DecodeStrParam(operands[i]);
			}
			switch(operatorType) {
				case FunctionOperatorTypeAdd.GetQuarter:
					return string.Format(CultureInfo.InvariantCulture, "((extract(month from {0}) - 1) / 3 + 1)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateMonthYear:
					return string.Format(CultureInfo.InvariantCulture, "cast({0} as date) - extract(day from {0}) + 1", operands[0]);
				case FunctionOperatorTypeAdd.GetDateQuarterYear:
					return string.Format(CultureInfo.InvariantCulture, "cast(extract(year from {0}) || '-' || ((extract(month from {0}) - 1) / 3 * 3 + 1) || '-1' as date)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHour:
					return string.Format(CultureInfo.InvariantCulture, "cast(cast({0} as date) || ' ' || extract(hour from cast({0} as timestamp)) || ':00:00' as timestamp)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinute:
					return string.Format(CultureInfo.InvariantCulture, "cast(cast({0} as date) || ' ' || extract(hour from cast({0} as timestamp)) || ':' || extract(minute from cast({0} as timestamp)) || ':00' as timestamp)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinuteSecond:
					return string.Format(CultureInfo.InvariantCulture, "cast(cast({0} as date) || ' ' || extract(hour from cast({0} as timestamp)) || ':' || extract(minute from cast({0} as timestamp)) || ':' || (cast(extract(second from cast({0} as timestamp)) * 10000 as integer) / 10000) as timestamp)", operands[0]);
			}
			return base.FormatFunctionAdd(operatorType, operands);
		}
	}
	class MSSQLFormatterAdd : SqlGeneratorFormatterAddBase {
		public override string FormatFunctionAdd(FunctionOperatorTypeAdd operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorTypeAdd.GetQuarter:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(quarter, {0})", operands[0]);
				case FunctionOperatorTypeAdd.GetDateMonthYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(mm, DATEDIFF(mm, 0, {0}), 0)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateQuarterYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(qq, DATEDIFF(qq, 0, {0}), 0)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHour:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(hh, DATEDIFF(hh, 0, {0}), 0)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinute:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(mi, DATEDIFF(mi, 0, {0}), 0)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinuteSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(ss, DATEPART(ss, CAST({0} AS DATETIME)), DATEADD(mi, DATEDIFF(mi, 0, {0}), 0))", operands[0]);
				case FunctionOperatorTypeAdd.GetWeekOfYear:
					switch(CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule) {
						case CalendarWeekRule.FirstDay:
							return string.Format(CultureInfo.InvariantCulture, "DATEPART(ww, {0})", operands[0]);
						case CalendarWeekRule.FirstFourDayWeek:
							return string.Format(CultureInfo.InvariantCulture, "case when datepart(m, {0}) = 1 and datepart(d, {0}) in (1, 2, 3) and datepart(ww, DATEADD(dd, 3, DATEADD(yy, DATEDIFF(yy, 0, {0}), 0))) > 1 then case when datepart(ww, DATEADD(dd, 3, DATEADD(yy, DATEDIFF(yy, 0, dateadd(d, 1-datepart(dw, {0}), {0})), 0))) = 1 then datepart(ww, dateadd(dd, 1-datepart(dw, {0}), {0})) else datepart(ww, dateadd(d, 1-datepart(dw, {0}), {0})) - 1 end when datepart(ww, DATEADD(dd, 3, DATEADD(yy, DATEDIFF(yy, 0, {0}), 0))) = 1 then datepart(ww, {0}) else datepart(ww, {0}) - 1 end", operands[0]);
						case CalendarWeekRule.FirstFullWeek:
							return string.Format(CultureInfo.InvariantCulture, "case when datepart(dw, DATEADD(yy, DATEDIFF(yy, 0, {0}), 0)) = 1 then DATEPART(ww, {0}) else case when DATEPART(ww, {0}) = 1 then case when DATEPART(dw, DATEADD(yy, DATEDIFF(yy, 0, {0})-1, 0)) = 1 then DATEPART(ww, DATEADD(yy, DATEDIFF(yy, 0, {0}), -1)) else DATEPART(ww, DATEADD(yy, DATEDIFF(yy, 0, {0}), -1)) - 1 end else DATEPART(ww, {0}) - 1 end end", operands[0]);
					}
					break;
			}
			return base.FormatFunctionAdd(operatorType, operands);
		}
	}
	class MSSQLCEFormatterAdd : SqlGeneratorFormatterAddBase {
		public override string FormatFunctionAdd(FunctionOperatorTypeAdd operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorTypeAdd.GetQuarter:
					return string.Format(CultureInfo.InvariantCulture, "DATEPART(quarter, {0})", operands[0]);
				case FunctionOperatorTypeAdd.GetDateMonthYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(mm, DATEDIFF(mm, 0, {0}), 0)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateQuarterYear:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(qq, DATEDIFF(qq, 0, {0}), 0)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHour:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(hh, DATEDIFF(hh, 0, {0}), 0)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinute:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(mi, DATEDIFF(mi, 0, {0}), 0)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinuteSecond:
					return string.Format(CultureInfo.InvariantCulture, "DATEADD(ss, DATEPART(ss, {0}), DATEADD(mi, DATEDIFF(mi, 0, {0}), 0))", operands[0]);
			}
			return base.FormatFunctionAdd(operatorType, operands);
		}
	}
	class MySqlFormatterAdd : SqlGeneratorFormatterAddBase {
		public override string FormatFunctionAdd(FunctionOperatorTypeAdd operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorTypeAdd.GetQuarter:
					return string.Format(CultureInfo.InvariantCulture, "quarter({0})", operands[0]);
				case FunctionOperatorTypeAdd.GetDateMonthYear:
					return string.Format(CultureInfo.InvariantCulture, "date({0}) - interval (day({0}) - 1) day", operands[0]);
				case FunctionOperatorTypeAdd.GetDateQuarterYear:
					return string.Format(CultureInfo.InvariantCulture, "date({0}) - interval (day({0}) - 1) day - interval ((month({0}) - 1) % 3) month", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHour:
					return string.Format(CultureInfo.InvariantCulture, "date({0}) + interval hour({0}) hour", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinute:
					return string.Format(CultureInfo.InvariantCulture, "date({0}) + interval hour({0}) hour + interval minute({0}) minute", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinuteSecond:
					return string.Format(CultureInfo.InvariantCulture, "date({0}) + interval hour({0}) hour + interval minute({0}) minute + interval second({0}) second", operands[0]);
				case FunctionOperatorTypeAdd.GetWeekOfYear:
					DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
					if(firstDayOfWeek != DayOfWeek.Sunday && firstDayOfWeek != DayOfWeek.Monday)
						return null;
					CalendarWeekRule calendarWeekRule = CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
					if(calendarWeekRule == CalendarWeekRule.FirstDay)
						return null;
					switch(calendarWeekRule) {
						case CalendarWeekRule.FirstFourDayWeek:
							return string.Format(CultureInfo.InvariantCulture, "week({0}, {1})", operands[0], firstDayOfWeek == DayOfWeek.Sunday ? 6 : 3);
						case CalendarWeekRule.FirstFullWeek:
							return string.Format(CultureInfo.InvariantCulture, "week({0}, {1})", operands[0], firstDayOfWeek == DayOfWeek.Sunday ? 2 : 7);
					}
					return null;
			}
			return base.FormatFunctionAdd(operatorType, operands);
		}
	}
	class OracleFormatterAdd : SqlGeneratorFormatterAddBase {
		public override string FormatFunctionAdd(FunctionOperatorTypeAdd operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorTypeAdd.GetQuarter:
					return string.Format(CultureInfo.InvariantCulture, "TO_NUMBER(TO_CHAR({0}, 'Q'))", operands[0]);
				case FunctionOperatorTypeAdd.GetDateMonthYear:
					return string.Format(CultureInfo.InvariantCulture, "TRUNC({0}, 'MM')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateQuarterYear:
					return string.Format(CultureInfo.InvariantCulture, "TRUNC({0}, 'Q')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHour:
					return string.Format(CultureInfo.InvariantCulture, "TRUNC({0}, 'HH')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinute:
					return string.Format(CultureInfo.InvariantCulture, "TRUNC({0}, 'MI')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinuteSecond:
					return string.Format(CultureInfo.InvariantCulture, "TRUNC({0}, 'MI') + NUMTODSINTERVAL(TO_NUMBER(TO_CHAR({0}, 'SS')), 'SECOND') ", operands[0]);
			}
			return base.FormatFunctionAdd(operatorType, operands);
		}
	}
	class PervasiveFormatterAdd : SqlGeneratorFormatterAddBase {
		public override string FormatFunctionAdd(FunctionOperatorTypeAdd operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorTypeAdd.GetQuarter:
					return string.Format(CultureInfo.InvariantCulture, "QUARTER({0})", operands[0]);
				case FunctionOperatorTypeAdd.GetDateMonthYear:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_MONTH, TIMESTAMPDIFF(SQL_TSI_MONTH, '0001-01-01', {0}), '0001-01-01')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateQuarterYear:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_QUARTER, TIMESTAMPDIFF(SQL_TSI_QUARTER, '0001-01-01', {0}), '0001-01-01')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHour:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_HOUR, TIMESTAMPDIFF(SQL_TSI_HOUR, '1970-01-01', {0}), '1970-01-01')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinute:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_MINUTE, TIMESTAMPDIFF(SQL_TSI_MINUTE, '1970-01-01', {0}), '1970-01-01')", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinuteSecond:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMPADD(SQL_TSI_SECOND, SECOND(CAST({0} AS TIMESTAMP)), TIMESTAMPADD(SQL_TSI_MINUTE, TIMESTAMPDIFF(SQL_TSI_MINUTE, '1970-01-01', {0}), '1970-01-01'))", operands[0]);
			}
			return base.FormatFunctionAdd(operatorType, operands);
		}
	}
	class PostgresFormatterAdd : SqlGeneratorFormatterAddBase {
		public override string FormatFunctionAdd(FunctionOperatorTypeAdd operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorTypeAdd.GetQuarter:
					return string.Format(CultureInfo.InvariantCulture, "extract(quarter from {0})", operands[0]);
				case FunctionOperatorTypeAdd.GetDateMonthYear:
					return string.Format(CultureInfo.InvariantCulture, "date_trunc('month', {0})", operands[0]);
				case FunctionOperatorTypeAdd.GetDateQuarterYear:
					return string.Format(CultureInfo.InvariantCulture, "cast(to_char({0}, 'YYYY-') || cast(3 * (extract(quarter from {0}) - 1) + 1 as integer) || '-01'	as timestamp)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHour:
					return string.Format(CultureInfo.InvariantCulture, "date_trunc('hour', {0})", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinute:
					return string.Format(CultureInfo.InvariantCulture, "date_trunc('minute', {0})", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinuteSecond:
					return string.Format(CultureInfo.InvariantCulture, "date_trunc('second', {0})", operands[0]);
			}
			return base.FormatFunctionAdd(operatorType, operands);
		}
	}
	class TeradataFormatterAdd : SqlGeneratorFormatterAddBase {
		public override string FormatFunctionAdd(FunctionOperatorTypeAdd operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorTypeAdd.GetQuarter:
					return string.Format(CultureInfo.InvariantCulture, "((EXTRACT(MONTH FROM {0}) - 1)/3 + 1)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateMonthYear:
					return string.Format(CultureInfo.InvariantCulture, "(CAST({0} as DATE) - EXTRACT(DAY FROM {0}) + 1)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateQuarterYear:
					return string.Format(CultureInfo.InvariantCulture, "(CAST((ADD_MONTHS({0}, - (EXTRACT(MONTH FROM {0}) - 1) MOD 3)) as DATE) - EXTRACT(DAY FROM {0}) + 1)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHour:
					return string.Format(CultureInfo.InvariantCulture, "({0} - INTERVAL '1' MINUTE * (EXTRACT(MINUTE FROM {0})) - INTERVAL '1' SECOND * (EXTRACT(SECOND FROM {0})))", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinute:
					return string.Format(CultureInfo.InvariantCulture, "({0} - INTERVAL '1' SECOND * (EXTRACT(SECOND FROM {0})))", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinuteSecond:
					return string.Format(CultureInfo.InvariantCulture, "(CAST({0} as TIMESTAMP))", operands[0]);
			}
			return base.FormatFunctionAdd(operatorType, operands);
		}
	}
	class BigQueryFormatterAdd : SqlGeneratorFormatterAddBase {
		public override string FormatFunctionAdd(FunctionOperatorTypeAdd operatorType, params string[] operands) {
			switch(operatorType) {
				case FunctionOperatorTypeAdd.GetQuarter:
					return string.Format(CultureInfo.InvariantCulture, "QUARTER({0})", operands[0]);
				case FunctionOperatorTypeAdd.GetDateMonthYear:
					return string.Format(CultureInfo.InvariantCulture, "CAST(DATE(DATE_ADD({0}, 1-DAY({0}), \"DAY\")) as TIMESTAMP) ", operands[0]);
				case FunctionOperatorTypeAdd.GetDateQuarterYear:
					return string.Format(CultureInfo.InvariantCulture, "CAST(DATE(DATE_ADD(DATE_ADD({0}, 1-DAY({0}), \"DAY\"), 1-MONTH({0}) % 3, \"MONTH\")) as TIMESTAMP)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHour:
					return string.Format(CultureInfo.InvariantCulture, "CAST(DATE_ADD(DATE_ADD({0}, -MINUTE({0}), \"MINUTE\"), -SECOND({0}), \"SECOND\") as TIMESTAMP)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinute:
					return string.Format(CultureInfo.InvariantCulture, "CAST(DATE_ADD({0}, -SECOND({0}), \"SECOND\") as TIMESTAMP)", operands[0]);
				case FunctionOperatorTypeAdd.GetDateHourMinuteSecond:
					return string.Format(CultureInfo.InvariantCulture, "TIMESTAMP({0})", operands[0]);
			}
			return base.FormatFunctionAdd(operatorType, operands);
		}
	}
	class SQLiteFormatterAdd : SqlGeneratorFormatterAddBase {
	}
	class VistaDBFormatterAdd : SqlGeneratorFormatterAddBase {
	}
}
