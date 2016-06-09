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
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public abstract class DataItemDateTimeFormatMenuItemCommand : DataItemMenuItemCommand {
		static void AssignDateTimeFormat(DataItemDateTimeFormat target, DataItemDateTimeFormat source) {
			target.DateFormat = source.DateFormat;
			target.DateHourFormat = source.DateHourFormat;
			target.DateHourMinuteFormat = source.DateHourMinuteFormat;
			target.DateTimeFormat = source.DateTimeFormat;
			target.DayOfWeekFormat = source.DayOfWeekFormat;
			target.MonthFormat = source.MonthFormat;
			target.QuarterFormat = source.QuarterFormat;
			target.YearFormat = source.YearFormat;
			target.HourFormat = source.HourFormat;
			target.ExactDateFormat = source.ExactDateFormat;
		}
		readonly DataItemDateTimeFormat format;
		protected DataItemDateTimeFormat Format { get { return format; } }
		protected DataItemDateTimeFormatMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem)
			: base(designer, dashboardItem, dataItem) {
			format = new DataItemDateTimeFormat(null);
			AssignDateTimeFormat(format, dataItem.DateTimeFormat);
		}
		public override void Execute() {
			if(format != DataItem.DateTimeFormat) {
				DateTimeFormatHistoryItem historyItem = new DateTimeFormatHistoryItem(DashboardItem, DataItem, format);
				Designer.History.RedoAndAdd(historyItem);
			}
		}
	}
	public abstract class ExactDateFormatMenuItemCommand : DataItemDateTimeFormatMenuItemCommand {
		protected abstract ExactDateFormat ExactDateFormat { get; }
		public override bool Checked { get { return DataItem.DateTimeFormat.ExactDateFormat == ExactDateFormat; } }
		public override string Caption { get { return DateTimeFormatCaptionProvider.GetCaption(ExactDateFormat); } }
		protected ExactDateFormatMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem)
			: base(designer, dashboardItem, dataItem) {
			Format.ExactDateFormat = ExactDateFormat;
		}
	}
	public class ExactDateFormatQuarterMenuItemCommand : ExactDateFormatMenuItemCommand {
		protected override ExactDateFormat ExactDateFormat { get { return ExactDateFormat.Quarter; } }
		public ExactDateFormatQuarterMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem)
			: base(designer, dashboardItem, dataItem) {
		}
	}
	public class ExactDateFormatMonthMenuItemCommand : ExactDateFormatMenuItemCommand {
		protected override ExactDateFormat ExactDateFormat { get { return ExactDateFormat.Month; } }
		public ExactDateFormatMonthMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem)
			: base(designer, dashboardItem, dataItem) {
		}
	}
	public class DateTimeYearFormatMenuItemCommand : DataItemDateTimeFormatMenuItemCommand {
		readonly YearFormat yearFormat;
		public override bool Checked { get { return DataItem.DateTimeFormat.YearFormat == yearFormat; } }
		public override string Caption { get { return DateTimeFormatCaptionProvider.GetCaption(yearFormat); } }
		public DateTimeYearFormatMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem, YearFormat yearFormat)
			: base(designer, dashboardItem, dataItem) {
			this.yearFormat = yearFormat;
			Format.YearFormat = yearFormat;
		}
	}
	public class ExactDateFormatYearMenuItemSubcommand : DateTimeYearFormatMenuItemCommand {
		const ExactDateFormat EDFormat = ExactDateFormat.Year;
		public override bool Checked { get { return base.Checked && DataItem.DateTimeFormat.ExactDateFormat == EDFormat; } }
		public ExactDateFormatYearMenuItemSubcommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem, YearFormat yearFormat)
			: base(designer, dashboardItem, dataItem, yearFormat) {
			Format.ExactDateFormat = EDFormat;
		}
	}
	public class DateTimeQuarterFormatMenuItemCommand : DataItemDateTimeFormatMenuItemCommand {
		readonly QuarterFormat quarterFormat;
		public override bool Checked { get { return DataItem.DateTimeFormat.QuarterFormat == quarterFormat; } }
		public override string Caption { get { return DateTimeFormatCaptionProvider.GetCaption(quarterFormat); } }
		public DateTimeQuarterFormatMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem, QuarterFormat quarterFormat)
			: base(designer, dashboardItem, dataItem) {
			this.quarterFormat = quarterFormat;
			Format.QuarterFormat = quarterFormat;
		}
	}
	public class DateTimeMonthFormatMenuItemCommand : DataItemDateTimeFormatMenuItemCommand {
		readonly MonthFormat monthFormat;
		public override bool Checked { get { return DataItem.DateTimeFormat.MonthFormat == monthFormat; } }
		public override string Caption { get { return DateTimeFormatCaptionProvider.GetCaption(monthFormat); } }
		public DateTimeMonthFormatMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem, MonthFormat monthFormat)
			: base(designer, dashboardItem, dataItem) {
			this.monthFormat = monthFormat;
			Format.MonthFormat = monthFormat;
		}
	}
	public class DateTimeDayOfWeekFormatMenuItemCommand : DataItemDateTimeFormatMenuItemCommand {
		readonly DayOfWeekFormat dayOfWeekFormat;
		public override bool Checked { get { return DataItem.DateTimeFormat.DayOfWeekFormat == dayOfWeekFormat; } }
		public override string Caption { get { return DateTimeFormatCaptionProvider.GetCaption(dayOfWeekFormat); } }
		public DateTimeDayOfWeekFormatMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem, DayOfWeekFormat dayOfWeekFormat)
			: base(designer, dashboardItem, dataItem) {
			this.dayOfWeekFormat = dayOfWeekFormat;
			Format.DayOfWeekFormat = dayOfWeekFormat;
		}
	}
	public class DateTimeDateFormatMenuItemCommand : DataItemDateTimeFormatMenuItemCommand {
		readonly DateFormat dateFormat;
		public override bool Checked { get { return DataItem.DateTimeFormat.DateFormat == dateFormat; } }
		public override string Caption { get { return DateTimeFormatCaptionProvider.GetCaption(dateFormat); } }
		public DateTimeDateFormatMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem, DateFormat dateFormat)
			: base(designer, dashboardItem, dataItem) {
			this.dateFormat = dateFormat;
			Format.DateFormat = dateFormat;
		}
	}
	public class ExactDateFormatDayMenuItemSubcommand : DateTimeDateFormatMenuItemCommand {
		const ExactDateFormat EDFormat = ExactDateFormat.Day;
		public override bool Checked { get { return base.Checked && DataItem.DateTimeFormat.ExactDateFormat == EDFormat; } }
		public ExactDateFormatDayMenuItemSubcommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem, DateFormat dateFormat)
			: base(designer, dashboardItem, dataItem, dateFormat) {
			Format.ExactDateFormat = EDFormat;
		}
	}
	public class DateTimeDateHourFormatMenuItemCommand : DataItemDateTimeFormatMenuItemCommand {
		readonly DateTimeFormat dateHourFormat;
		public override bool Checked { get { return DataItem.DateTimeFormat.DateHourFormat == dateHourFormat; } }
		public override string Caption { get { return DateTimeFormatCaptionProvider.GetCaption(dateHourFormat); } }
		public DateTimeDateHourFormatMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem, DateTimeFormat dateHourFormat)
			: base(designer, dashboardItem, dataItem) {
			this.dateHourFormat = dateHourFormat;
			Format.DateHourFormat = dateHourFormat;
		}
	}
	public class ExactDateFormatHourMenuItemSubcommand : DateTimeDateHourFormatMenuItemCommand {
		const ExactDateFormat EDFormat = ExactDateFormat.Hour;
		public override bool Checked { get { return base.Checked && DataItem.DateTimeFormat.ExactDateFormat == EDFormat; } }
		public ExactDateFormatHourMenuItemSubcommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem, DateTimeFormat dateHourFormat)
			: base(designer, dashboardItem, dataItem, dateHourFormat) {
			Format.ExactDateFormat = EDFormat;
		}
	}
	public class DateTimeDateHourMinuteFormatMenuItemCommand : DataItemDateTimeFormatMenuItemCommand {
		readonly DateTimeFormat dateHourMinuteFormat;
		public override bool Checked { get { return DataItem.DateTimeFormat.DateHourMinuteFormat == dateHourMinuteFormat; } }
		public override string Caption { get { return DateTimeFormatCaptionProvider.GetCaption(dateHourMinuteFormat); } }
		public DateTimeDateHourMinuteFormatMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem, DateTimeFormat dateHourMinuteFormat)
			: base(designer, dashboardItem, dataItem) {
			this.dateHourMinuteFormat = dateHourMinuteFormat;
			Format.DateHourMinuteFormat = dateHourMinuteFormat;
		}
	}
	public class ExactDateFormatMinuteMenuItemSubcommand : DateTimeDateHourMinuteFormatMenuItemCommand {
		const ExactDateFormat EDFormat = ExactDateFormat.Minute;
		public override bool Checked { get { return base.Checked && DataItem.DateTimeFormat.ExactDateFormat == EDFormat; } }
		public ExactDateFormatMinuteMenuItemSubcommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem, DateTimeFormat dateHourMinuteFormat)
			: base(designer, dashboardItem, dataItem, dateHourMinuteFormat) {
			Format.ExactDateFormat = EDFormat;
		}
	}
	public class DateTimeDateTimeFormatMenuItemCommand : DataItemDateTimeFormatMenuItemCommand {
		readonly DateTimeFormat dateTimeFormat;
		public override bool Checked { get { return DataItem.DateTimeFormat.DateTimeFormat == dateTimeFormat; } }
		public override string Caption { get { return DateTimeFormatCaptionProvider.GetCaption(dateTimeFormat); } }
		public DateTimeDateTimeFormatMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem, DateTimeFormat dateTimeFormat)
			: base(designer, dashboardItem, dataItem) {
			this.dateTimeFormat = dateTimeFormat;
			Format.DateTimeFormat = dateTimeFormat;
		}
	}
	public class ExactDateFormatSecondMenuItemSubcommand : DateTimeDateTimeFormatMenuItemCommand {
		const ExactDateFormat EDFormat = ExactDateFormat.Second;
		public override bool Checked { get { return base.Checked && DataItem.DateTimeFormat.ExactDateFormat == EDFormat; } }
		public ExactDateFormatSecondMenuItemSubcommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem, DateTimeFormat dateTimeFormat)
			: base(designer, dashboardItem, dataItem, dateTimeFormat) {
			Format.ExactDateFormat = EDFormat;
		}
	}
	public class DateTimeHourFormatMenuItemCommand : DataItemDateTimeFormatMenuItemCommand {
		readonly HourFormat hourFormat;
		public override bool Checked { get { return DataItem.DateTimeFormat.HourFormat == hourFormat; } }
		public override string Caption { get { return DateTimeFormatCaptionProvider.GetCaption(hourFormat); } }
		public DateTimeHourFormatMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem, HourFormat hourFormat)
			: base(designer, dashboardItem, dataItem) {
			this.hourFormat = hourFormat;
			Format.HourFormat = hourFormat;
		}
	}
}
