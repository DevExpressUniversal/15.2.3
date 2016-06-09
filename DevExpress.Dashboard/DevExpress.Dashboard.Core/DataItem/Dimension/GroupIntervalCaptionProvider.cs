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

using DevExpress.DashboardCommon.Localization;
namespace DevExpress.DashboardCommon.Native {
	public static class GroupIntervalCaptionProvider {
		public static string GetTextGroupIntervalCaption(TextGroupInterval groupInterval) {
			return groupInterval == TextGroupInterval.Alphabetical ? DashboardLocalizer.GetString(DashboardStringId.TextGroupIntervalAlphabetical) :
																	 DashboardLocalizer.GetString(DashboardStringId.GroupIntervalNone);
		}
		public static string GetNumericGroupIntervalCaption(bool isDiscreteScale) {
			return isDiscreteScale ? DashboardLocalizer.GetString(DashboardStringId.NumericGroupIntervalDiscrete) : 
				DashboardLocalizer.GetString(DashboardStringId.NumericGroupIntervalContinuous);
		}
		public static string GetDateTimeGroupIntervalCaption(DateTimeGroupInterval groupInterval) {
			switch (groupInterval) {
				case DateTimeGroupInterval.Quarter:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalQuarter);
				case DateTimeGroupInterval.Month:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalMonth);
				case DateTimeGroupInterval.Day:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalDay);
				case DateTimeGroupInterval.Hour:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalHour);
				case DateTimeGroupInterval.Minute:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalMinute);
				case DateTimeGroupInterval.Second:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalSecond);
				case DateTimeGroupInterval.DayOfYear:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalDayOfYear);
				case DateTimeGroupInterval.DayOfWeek:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalDayOfWeek);
				case DateTimeGroupInterval.WeekOfYear:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalWeekOfYear);
				case DateTimeGroupInterval.WeekOfMonth:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalWeekOfMonth);
				case DateTimeGroupInterval.MonthYear:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalMonthYear);
				case DateTimeGroupInterval.QuarterYear:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalQuarterYear);
				case DateTimeGroupInterval.DayMonthYear:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalDayMonthYear);
				case DateTimeGroupInterval.DateHour:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalDateHour);
				case DateTimeGroupInterval.DateHourMinute:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalDateHourMinute);
				case DateTimeGroupInterval.DateHourMinuteSecond:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalDateHourMinuteSecond);
				case DateTimeGroupInterval.None:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalExactDate);
				default:
					return DashboardLocalizer.GetString(DashboardStringId.DateTimeGroupIntervalYear);
			}
		}
	}
}
