#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Kpi {
	public enum DateBase { ExactDate, Now, DayStart, DayEnd, WeekStart, WeekEnd, MonthStart, MonthEnd, YearStart, YearEnd, QuarterStart, QuarterEnd }
	[XafDefaultProperty("DateTime")]
	public interface IDateTimePoint {
		DateTime DateTime { get; }
	}
	[DomainComponent]
	[XafDefaultProperty("Caption")]
	public interface IDateRange {
		string Name { get; set; }
		string Caption { get; }
		DateTime StartPoint { get; }
		DateTime EndPoint { get; }
	}
	[DefaultProperty("DateTime")]
	public class RangePoint : IDateTimePoint {
		private static DateTime GetToday() {
#if DebugTest
			if(CustomGetToday != null) {
				CustomGetTodayEventArgs args = new CustomGetTodayEventArgs();
				CustomGetToday(null, args);
				return args.Today;
			}
#endif
			return DateTime.Today;
		}
		private DateTime exactDateTime;
		private DateBase dateBase = DateBase.Now;
		private int shiftValue = 0;
		private TimeIntervalType shiftType = TimeIntervalType.Day;
		private DateTime GetBaseDate(DateBase dateBase, DateTime exactDate) {
			switch(dateBase) {
				case DateBase.ExactDate:
					return exactDate;
				case DateBase.Now:
					return DateTime.Now;
				case DateBase.DayStart:
					return DateTime.Today;
				case DateBase.DayEnd:
					return DateTime.Today.AddDays(1).AddSeconds(-1);
				case DateBase.WeekStart:
					return GetToday().AddDays(-DateTimeHelper.GetDayOfWeek(GetToday().DayOfWeek));
				case DateBase.WeekEnd:
					return GetToday().AddDays(7 - DateTimeHelper.GetDayOfWeek(GetToday().DayOfWeek)).AddSeconds(-1);
				case DateBase.MonthStart:
					return DateTime.Today.AddDays(-DateTime.Today.Day + 1);
				case DateBase.MonthEnd:
					return DateTime.Today.AddDays(DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month) - DateTime.Today.Day + 1).AddSeconds(-1);
				case DateBase.QuarterStart:
					return GetQuarterStart();
				case DateBase.QuarterEnd:
					return GetQuarterEnd();
				case DateBase.YearStart:
					return DateTime.Today.AddDays(-DateTime.Today.DayOfYear + 1);
				case DateBase.YearEnd:
					return DateTime.Today.AddDays(-DateTime.Today.DayOfYear + 1).AddYears(1).AddSeconds(-1);
			}
			return DateTime.Now;
		}
		private DateTime GetQuarterStart() {
			DateTime today = GetToday();
			if(today.Month < 4) {
				return new DateTime(today.Year, 1, 1);
			}
			if(today.Month < 7) {
				return new DateTime(today.Year, 4, 1);
			}
			if(today.Month < 10) {
				return new DateTime(today.Year, 7, 1);
			}
			return new DateTime(today.Year, 10, 1);
		}
		private DateTime GetQuarterEnd() {
			DateTime today = GetToday();
			if(today.Month >= 10) {
				return new DateTime(today.Year + 1, 1, 1).AddSeconds(-1);
			}
			if(today.Month >= 7) {
				return new DateTime(today.Year, 10, 1).AddSeconds(-1);
			}
			if(today.Month >= 4) {
				return new DateTime(today.Year, 7, 1).AddSeconds(-1);
			}
			return new DateTime(today.Year, 4, 1).AddSeconds(-1);
		}
		private DateTime GetShiftedDate(DateTime baseDate, int shiftValue, TimeIntervalType shiftDurationType) {
			if(shiftValue != 0) {
				switch(shiftDurationType) {
					case TimeIntervalType.Hour:
						return baseDate.AddHours(shiftValue);
					case TimeIntervalType.Day:
						return baseDate.AddDays(shiftValue);
					case TimeIntervalType.Week:
						return baseDate.AddDays(7 * shiftValue);
					case TimeIntervalType.Month:
						return baseDate.AddMonths(shiftValue);
					case TimeIntervalType.Quarter:
						if(baseDate.Day == DateTime.DaysInMonth(baseDate.Year, baseDate.Month)) {
							return baseDate.AddDays(1).AddMonths(3 * shiftValue).AddDays(-1);
						}
						return baseDate.AddMonths(3 * shiftValue);
					case TimeIntervalType.Year:
						return baseDate.AddYears(shiftValue);
				}
			}
			return baseDate;
		}
		private void InitializeTimeShift(int shiftValue, TimeIntervalType shiftType) {
			this.shiftValue = shiftValue;
			this.shiftType = shiftType;
		}
		public RangePoint() { }
		public RangePoint(DateBase dateBase)
			: this(dateBase, 0, TimeIntervalType.Day) {
		}
		public RangePoint(DateTime exactDateTime, int shiftValue, TimeIntervalType shiftType) {
			this.exactDateTime = exactDateTime;
			this.dateBase = DateBase.ExactDate;
			InitializeTimeShift(shiftValue, shiftType);
		}
		public RangePoint(DateBase dateBase, int shiftValue, TimeIntervalType shiftType) {
			if(dateBase == DateBase.ExactDate) {
				throw new ArgumentException("Cannot create ExactDate RangePoint without certain DateTime value.");
			}
			this.dateBase = dateBase;
			InitializeTimeShift(shiftValue, shiftType);
		}
		public DateTime ExactDateTime { get; set; }
		public DateBase DateBase { get; set; }
		public int ShiftValue { get; set; }
		public TimeIntervalType ShiftType { get; set; }
		public DateTime DateTime {
			get { return GetShiftedDate(GetBaseDate(dateBase, exactDateTime), shiftValue, shiftType); }
		}
#if DebugTest
		internal static event EventHandler<CustomGetTodayEventArgs> CustomGetToday;
#endif
	}
#if DebugTest
	internal class CustomGetTodayEventArgs : EventArgs {
		public DateTime Today { get; set; }
	}
#endif
	[DefaultProperty("Caption")]
	public class DateRange : IDateRange {
		private IDateTimePoint startPoint;
		private IDateTimePoint endPoint;
		protected DateRange(string name) {
			this.Name = name;
		}
		public DateRange(string name, IDateTimePoint startPoint, IDateTimePoint endPoint)
			: this(name) {
			this.startPoint = startPoint;
			this.endPoint = endPoint;
		}
		public string Name { get; set; }
		public string Caption {
			get {
				string caption = CaptionHelper.GetLocalizedText("DateRanges", Name);
				if(string.IsNullOrEmpty(caption)) {
					caption = CaptionHelper.ConvertCompoundName(Name);
				}
				return caption;
			}
		}
		public virtual DateTime StartPoint {
			get { return startPoint.DateTime; }
		}
		public virtual DateTime EndPoint {
			get { return endPoint.DateTime; }
		}
	}
}
