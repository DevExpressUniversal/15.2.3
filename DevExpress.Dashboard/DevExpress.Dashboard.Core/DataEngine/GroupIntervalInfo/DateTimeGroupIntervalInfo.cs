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
using DevExpress.XtraPivotGrid;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardCommon.Localization;
namespace DevExpress.DashboardCommon.Native {
	public abstract class DateTimeGroupIntervalInfo : GroupIntervalInfo {
		public override Type DataType { get { return typeof(DateTime); } }
		public override Type ChartDataType { get { return typeof(DateTime); } }
	}
	public abstract class DiscontinuousDateTimeGroupIntervalInfo : DateTimeGroupIntervalInfo {
		public abstract DateTimePresentationUnit Unit { get; }
	}
	public class MonthYearGroupIntervalInfo : DiscontinuousDateTimeGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.DateMonthYear; } }
		public override DateTimePresentationUnit Unit { get { return DateTimePresentationUnit.Month; } }
	}
	public class QuarterYearGroupIntervalInfo : DiscontinuousDateTimeGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.DateQuarterYear; } }
		public override DateTimePresentationUnit Unit { get { return DateTimePresentationUnit.Quarter; } }
	}
	public class DayMonthYearGroupIntervalInfo : DiscontinuousDateTimeGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.Date; } }
		public override DateTimePresentationUnit Unit { get { return DateTimePresentationUnit.Day; } }
	}
	public class DateHourGroupIntervalInfo : DiscontinuousDateTimeGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.DateHour; } }
		public override DateTimePresentationUnit Unit { get { return DateTimePresentationUnit.Hour; } }
	}
	public class DateHourMinuteGroupIntervalInfo : DiscontinuousDateTimeGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.DateHourMinute; } }
		public override DateTimePresentationUnit Unit { get { return DateTimePresentationUnit.Minute; } }
	}
	public class DateHourMinuteSecondGroupIntervalInfo : DiscontinuousDateTimeGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.DateHourMinuteSecond; } }
		public override DateTimePresentationUnit Unit { get { return DateTimePresentationUnit.Second; } }
	}
	public class NoneGroupIntervalInfo : DateTimeGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.Default; } }
	}
}
