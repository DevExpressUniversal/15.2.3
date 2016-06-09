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
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	public static class DateTimeFormatCommandGenerator {
		public static IEnumerable<DataItemDateTimeFormatMenuItemCommand> CreateNonExactDateFormatCommands(DashboardDesigner designer, DataDashboardItem dashboardItem, Dimension dimension) {
			switch(dimension.DateTimeGroupInterval) {
				case DateTimeGroupInterval.Year:
					foreach(YearFormat yearFormat in Enum.GetValues(typeof(YearFormat))) {
						yield return new DateTimeYearFormatMenuItemCommand(designer, dashboardItem, dimension, yearFormat);
					}
					break;
				case DateTimeGroupInterval.Quarter:
					foreach(QuarterFormat quarterFormat in Enum.GetValues(typeof(QuarterFormat))) {
						yield return new DateTimeQuarterFormatMenuItemCommand(designer, dashboardItem, dimension, quarterFormat);
					}
					break;
				case DateTimeGroupInterval.Month:
					foreach(MonthFormat monthFormat in Enum.GetValues(typeof(MonthFormat))) {
						yield return new DateTimeMonthFormatMenuItemCommand(designer, dashboardItem, dimension, monthFormat);
					}
					break;
				case DateTimeGroupInterval.DayOfWeek:
					foreach(DayOfWeekFormat dayOfWeekFormat in Enum.GetValues(typeof(DayOfWeekFormat))) {
						yield return new DateTimeDayOfWeekFormatMenuItemCommand(designer, dashboardItem, dimension, dayOfWeekFormat);
					}
					break;
				case DateTimeGroupInterval.DayMonthYear:
					foreach(DateFormat dateFormat in Enum.GetValues(typeof(DateFormat))) {
						yield return new DateTimeDateFormatMenuItemCommand(designer, dashboardItem, dimension, dateFormat);
					}
					break;
				case DateTimeGroupInterval.DateHour:
					foreach(DateTimeFormat dateHourFormat in Enum.GetValues(typeof(DateTimeFormat))) {
						yield return new DateTimeDateHourFormatMenuItemCommand(designer, dashboardItem, dimension, dateHourFormat);
					}
					break;
				case DateTimeGroupInterval.DateHourMinute:
					foreach(DateTimeFormat dateHourMinuteFormat in Enum.GetValues(typeof(DateTimeFormat))) {
						yield return new DateTimeDateHourMinuteFormatMenuItemCommand(designer, dashboardItem, dimension, dateHourMinuteFormat);
					}
					break;
				case DateTimeGroupInterval.DateHourMinuteSecond:
					foreach(DateTimeFormat dateTimeFormat in Enum.GetValues(typeof(DateTimeFormat))) {
						yield return new DateTimeDateTimeFormatMenuItemCommand(designer, dashboardItem, dimension, dateTimeFormat);
					}
					break;
				case DateTimeGroupInterval.Hour:
					foreach(HourFormat hourFormat in Enum.GetValues(typeof(HourFormat))) {
						yield return new DateTimeHourFormatMenuItemCommand(designer, dashboardItem, dimension, hourFormat);
					}
					break;
			}
		}
		public static ExactDateFormatMenuItemCommand CreateExactDateFormatCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, Dimension dimension, ExactDateFormat exactDateFormat) {
			switch(exactDateFormat) {
				case ExactDateFormat.Quarter:
					return new ExactDateFormatQuarterMenuItemCommand(designer, dashboardItem, dimension);
				case ExactDateFormat.Month:
					return new ExactDateFormatMonthMenuItemCommand(designer, dashboardItem, dimension);
			}
			return null;
		}
		public static IEnumerable<DataItemDateTimeFormatMenuItemCommand> CreateExactDateFormatSubcommands(DashboardDesigner designer, DataDashboardItem dashboardItem, Dimension dimension, ExactDateFormat exactDateFormat) {
			switch(exactDateFormat) {
				case ExactDateFormat.Year:
					foreach(YearFormat yearFormat in Enum.GetValues(typeof(YearFormat))) {
						yield return new ExactDateFormatYearMenuItemSubcommand(designer, dashboardItem, dimension, yearFormat);
					}
					break;
				case ExactDateFormat.Day:
					foreach(DateFormat dateFormat in Enum.GetValues(typeof(DateFormat))) {
						yield return new ExactDateFormatDayMenuItemSubcommand(designer, dashboardItem, dimension, dateFormat);
					}
					break;
				case ExactDateFormat.Hour:
					foreach(DateTimeFormat dateHourFormat in Enum.GetValues(typeof(DateTimeFormat))) {
						yield return new ExactDateFormatHourMenuItemSubcommand(designer, dashboardItem, dimension, dateHourFormat);
					}
					break;
				case ExactDateFormat.Minute:
					foreach(DateTimeFormat dateHourMinuteFormat in Enum.GetValues(typeof(DateTimeFormat))) {
						yield return new ExactDateFormatMinuteMenuItemSubcommand(designer, dashboardItem, dimension, dateHourMinuteFormat);
					}
					break;
				case ExactDateFormat.Second:
					foreach(DateTimeFormat dateTimeFormat in Enum.GetValues(typeof(DateTimeFormat))) {
						yield return new ExactDateFormatSecondMenuItemSubcommand(designer, dashboardItem, dimension, dateTimeFormat);
					}
					break;
			}
		}
		public static IEnumerable<DataItemDateTimeFormatMenuItemCommand> CreateMeasureCommands(DashboardDesigner designer, DataDashboardItem dashboardItem, Measure measure) {
			foreach (DateFormat dateTimeFormat in Enum.GetValues(typeof(DateFormat))) {
				yield return new DateTimeDateFormatMenuItemCommand(designer, dashboardItem, measure, dateTimeFormat);
			}
		}
	}
}
