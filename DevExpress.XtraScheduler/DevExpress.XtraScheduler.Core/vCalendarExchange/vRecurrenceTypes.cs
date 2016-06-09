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
namespace DevExpress.XtraScheduler.VCalendar {
	public enum VRecurrenceFrequency { Daily, Weekly, Monthly, Yearly }
	public enum VRecurrenceRange { OccurrenceCount, EndByDate, Forever }
	[Flags]
	public enum VRecurrenceWeekDays {
		Sunday = 1,
		Monday = 2,
		Tuesday = 4,
		Wednesday = 8,
		Thursday = 16,
		Friday = 32,
		Saturday = 64,
	}
	public abstract class VRecurrenceRuleNumberItem {
		int val;
		protected VRecurrenceRuleNumberItem(int val) {
			if (val < MinValue || val > MaxValue)
				throw new ArgumentException("Incorrect value: " + val.ToString());
			this.val = val;
		}
		public abstract int MinValue { get; }
		public abstract int MaxValue { get; }
		public int Value { get { return val; } }
	}
	public abstract class SignedNumberItem : VRecurrenceRuleNumberItem {
		bool negative;
		protected SignedNumberItem(int val)
			: base(val) {
		}
		public bool Negative { get { return negative; } set { negative = value; } }
		protected internal int GetSignedValue() {
			return Negative ? -1 * Value : Value;
		}
		protected internal string GetSignedValueString() {
			string s = Value.ToString();
			if (Negative) s += "-";
			return s;
		}
	}
	public class VRecurrenceRuleDay : VRecurrenceRuleNumberItem {
		public VRecurrenceRuleDay(int val)
			: base(val) {
		}
		public override int MinValue { get { return 1; } }
		public override int MaxValue { get { return 366; } }
		public int Day { get { return base.Value; } }
	}
	public class VRecurrenceRuleMonth : VRecurrenceRuleNumberItem {
		public VRecurrenceRuleMonth(int val)
			: base(val) {
		}
		public override int MinValue { get { return 1; } }
		public override int MaxValue { get { return 12; } }
		public int Month { get { return base.Value; } }
	}
	public class VRecurrenceRuleDayNumber : SignedNumberItem {
		readonly bool lastDay;
		public VRecurrenceRuleDayNumber(int val, bool lastDay)
			: base(val) {
			this.lastDay = lastDay;
		}
		public override int MinValue { get { return 1; } }
		public override int MaxValue { get { return 31; } }
		public bool LastDay { get { return lastDay; } }
		public int DayNumber { get { return base.Value; } }
	}
	public class WeekDaysOccurrence : SignedNumberItem {
		VRecurrenceWeekDays weekDays;
		public WeekDaysOccurrence(VRecurrenceWeekDays weekDays, int occurrenceNumber)
			: base(occurrenceNumber) {
			this.weekDays = weekDays;
		}
		public WeekDaysOccurrence(int occurrenceNumber)
			: base(occurrenceNumber) {
		}
		public override int MinValue { get { return 1; } }
		public override int MaxValue { get { return 5; } }
		public int OccurrenceNumber { get { return base.Value; } }
		public VRecurrenceWeekDays WeekDays { get { return weekDays; } set { weekDays = value; } }
	}
	public class VRecurrenceRuleMonthList : List<VRecurrenceRuleMonth> {
	}
	public class VRecurrenceRuleDayList : List<VRecurrenceRuleDay> {
	}
	public class VRecurrenceRuleDayNumberList : List<VRecurrenceRuleDayNumber> {
	}
	public class WeekDaysOccurrenceList : List<WeekDaysOccurrence> {
		public void Add(VRecurrenceWeekDays weekDays, int occurrenceNumber) {
			Add(new WeekDaysOccurrence(weekDays, occurrenceNumber));
		}
	}
	public class VRecurrenceRule {
		VRecurrenceFrequency frequency;
		VRecurrenceRange range;
		DateTime startDate;
		int interval = 0;
		int duration = 0;
		DateTime endDate;
		readonly WeekDaysOccurrenceList weekDaysList;
		readonly VRecurrenceRuleDayNumberList dayNumberList;
		readonly VRecurrenceRuleMonthList monthList;
		readonly VRecurrenceRuleDayList dayList;
		public VRecurrenceRule() {
			dayNumberList = new VRecurrenceRuleDayNumberList();
			weekDaysList = new WeekDaysOccurrenceList();
			monthList = new VRecurrenceRuleMonthList();
			dayList = new VRecurrenceRuleDayList();
		}
		public VRecurrenceFrequency Frequency { get { return frequency; } set { frequency = value; } }
		public int Interval { get { return interval; } set { interval = value; } }
		public VRecurrenceRange Range { get { return range; } set { range = value; } }
		public int Duration { get { return duration; } set { duration = value; } }
		public DateTime StartDate { get { return startDate; } set { startDate = value; } }
		public DateTime EndDate { get { return endDate; } set { endDate = value; } }
		public WeekDaysOccurrenceList WeekDaysList { get { return weekDaysList; } }
		public VRecurrenceRuleDayNumberList DayNumberList { get { return dayNumberList; } }
		public VRecurrenceRuleMonthList MonthList { get { return monthList; } }
		public VRecurrenceRuleDayList DayList { get { return dayList; } }
	}
	public class VRecurrenceRuleCollection : List<VRecurrenceRule> {
	}
	public class DateTimeCollection : List<DateTime> {
	}
}
