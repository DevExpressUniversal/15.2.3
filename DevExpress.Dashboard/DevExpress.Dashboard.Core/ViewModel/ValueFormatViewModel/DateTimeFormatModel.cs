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

namespace DevExpress.DashboardCommon.ViewModel {
	public class DateTimeFormatViewModel {
		DateTimeGroupInterval groupInterval;
		ExactDateFormat exactDateFormat;
		YearFormat yearFormat;
		QuarterFormat quarterFormat;
		MonthFormat monthFormat;
		DayOfWeekFormat dayOfWeekFormat;
		HourFormat hourFormat;
		DateFormat dateFormat;
		DateTimeFormat dateHourFormat;
		DateTimeFormat dateHourMinuteFormat;
		DateTimeFormat dateTimeFormat;
		public DateTimeGroupInterval GroupInterval {
			get { return groupInterval; }
			set { groupInterval = value; }
		}
		public ExactDateFormat ExactDateFormat {
			get { return exactDateFormat; }
			set { exactDateFormat = value; }
		}
		public YearFormat YearFormat { 
			get { return yearFormat; }
			set { yearFormat = value; }
		}
		public QuarterFormat QuarterFormat {
			get { return quarterFormat; }
			set { quarterFormat = value; }
		}
		public MonthFormat MonthFormat { 
			get { return monthFormat; }
			set { monthFormat = value; }
		}
		public DayOfWeekFormat DayOfWeekFormat { 
			get { return dayOfWeekFormat; }
			set { dayOfWeekFormat = value; }
		}
		public DateFormat DateFormat { 
			get { return dateFormat; }
			set { dateFormat = value; }
		}
		public DateTimeFormat DateHourFormat { 
			get { return dateHourFormat; }
			set { dateHourFormat = value; }
		}
		public DateTimeFormat DateHourMinuteFormat { 
			get { return dateHourMinuteFormat; } 
			set { dateHourMinuteFormat = value; }
		}
		public DateTimeFormat DateTimeFormat { 
			get { return dateTimeFormat; }
			set { dateTimeFormat = value; }
		}
		public HourFormat HourFormat {
			get { return hourFormat; }
			set { hourFormat = value; }
		}
		public DateTimeFormatViewModel(DataItemDateTimeFormat format) {
			groupInterval = format.GroupInterval;
			yearFormat = format.YearFormat;
			if (yearFormat == YearFormat.Default)
				yearFormat = YearFormat.Full;
			quarterFormat = format.QuarterFormat;
			if (quarterFormat == QuarterFormat.Default)
				quarterFormat = QuarterFormat.Full;
			monthFormat = format.MonthFormat;
			if (monthFormat == MonthFormat.Default)
				monthFormat = MonthFormat.Full;
			dayOfWeekFormat = format.DayOfWeekFormat;
			if (dayOfWeekFormat == DayOfWeekFormat.Default)
				dayOfWeekFormat = DayOfWeekFormat.Full;
			dateFormat = format.DateFormat;
			if(dateFormat == DateFormat.Default)
				dateFormat = DateFormat.Short;
			dateHourFormat = format.DateHourFormat;
			if(dateHourFormat == DateTimeFormat.Default)
				dateHourFormat = DateTimeFormat.Short;
			dateHourMinuteFormat = format.DateHourMinuteFormat;
			if(dateHourMinuteFormat == DateTimeFormat.Default)
				dateHourMinuteFormat = DateTimeFormat.Short;
			dateTimeFormat = format.DateTimeFormat;
			if (dateTimeFormat == DateTimeFormat.Default)
				dateTimeFormat = DateTimeFormat.Short;
			hourFormat = format.HourFormat;
			if (hourFormat == HourFormat.Default)
				hourFormat = DashboardCommon.HourFormat.Long;
			exactDateFormat = format.ExactDateFormat;
		}
		public bool EqualsViewModel(DateTimeFormatViewModel format) {
			return format != null &&
				format.groupInterval == groupInterval &&
				format.exactDateFormat == exactDateFormat &&
				format.yearFormat == yearFormat &&
				format.quarterFormat == quarterFormat &&
				format.monthFormat == monthFormat &&
				format.dayOfWeekFormat == dayOfWeekFormat &&
				format.hourFormat == hourFormat &&
				format.dateFormat == dateFormat &&
				format.dateHourFormat == dateHourFormat &&
				format.dateHourMinuteFormat == dateHourMinuteFormat &&
				format.dateTimeFormat == dateTimeFormat;
		}
	}
}
