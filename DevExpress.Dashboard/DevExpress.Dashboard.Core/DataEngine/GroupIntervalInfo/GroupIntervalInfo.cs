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
using DevExpress.XtraPivotGrid;
namespace DevExpress.DashboardCommon.Native {
	public abstract class GroupIntervalInfo {
		enum GroupIntervalType { Text, Numeric, DateTime, Unknown }
		class GroupIntervalInfoKey<TType> {
			public GroupIntervalType GroupIntervalType { get; set; }
			public TType Val { get; set; }
			public override bool Equals(object obj) {
				GroupIntervalInfoKey<TType> ti = obj as GroupIntervalInfoKey<TType>;
				return ti != null && ti.GroupIntervalType == GroupIntervalType && Comparer<TType>.Default.Compare(ti.Val, Val) == 0;
			}
			public override int GetHashCode() {
				return GroupIntervalType.GetHashCode() ^ Val.GetHashCode();
			}
		}
		static Dictionary<object, GroupIntervalInfo> cache = new Dictionary<object, GroupIntervalInfo>();
		static GroupIntervalInfo GetTextGroupIntervalInfo(TextGroupInterval groupInterval) {
			if(groupInterval == TextGroupInterval.Alphabetical)
				return new AlphabeticalGroupIntervalInfo();
			return null;
		}
		static GroupIntervalInfo GetNumericGroupIntervalInfo(bool isDiscreteNumber) {
			return isDiscreteNumber ? null : new ContinuousGroupIntervalInfo();
		}
		static GroupIntervalInfo CreateDateTimeGroupIntervalInfo(DateTimeGroupInterval groupInterval) {
			switch(groupInterval) {
				case DateTimeGroupInterval.Year:
					return new YearGroupIntervalInfo();
				case DateTimeGroupInterval.Quarter:
					return new QuarterGroupIntervalInfo();
				case DateTimeGroupInterval.Month:
					return new MonthGroupIntervalInfo();
				case DateTimeGroupInterval.Day:
					return new DayGroupIntervalInfo();
				case DateTimeGroupInterval.Hour:
					return new HourGroupIntervalInfo();
				case DateTimeGroupInterval.Minute:
					return new MinuteGroupIntervalInfo();
				case DateTimeGroupInterval.Second:
					return new SecondGroupIntervalInfo();
				case DateTimeGroupInterval.DayOfYear:
					return new DayOfYearGroupIntervalInfo();
				case DateTimeGroupInterval.DayOfWeek:
					return new DayOfWeekGroupIntervalInfo();
				case DateTimeGroupInterval.WeekOfYear:
					return new WeekOfYearGroupIntervalInfo();
				case DateTimeGroupInterval.WeekOfMonth:
					return new WeekOfMonthGroupIntervalInfo();
				case DateTimeGroupInterval.MonthYear:
					return new MonthYearGroupIntervalInfo();
				case DateTimeGroupInterval.QuarterYear:
					return new QuarterYearGroupIntervalInfo();
				case DateTimeGroupInterval.DayMonthYear:
					return new DayMonthYearGroupIntervalInfo();
				case DateTimeGroupInterval.DateHour:
					return new DateHourGroupIntervalInfo();
				case DateTimeGroupInterval.DateHourMinute:
					return new DateHourMinuteGroupIntervalInfo();
				case DateTimeGroupInterval.DateHourMinuteSecond:
					return new DateHourMinuteSecondGroupIntervalInfo();
				case DateTimeGroupInterval.None:
					return new NoneGroupIntervalInfo();
				default:
					throw new Exception(Helper.GetUnknownEnumValueMessage(groupInterval));
			}
		}
		public static GroupIntervalInfo GetInstance(Dimension dimension, DataFieldType fieldType) {
			object key = null;
			switch(fieldType) {
				case DataFieldType.Text:
					key = new GroupIntervalInfoKey<TextGroupInterval>() { GroupIntervalType = GroupIntervalInfo.GroupIntervalType.Text, Val = dimension.TextGroupInterval };
					break;
				case DataFieldType.Integer:
				case DataFieldType.Float:
				case DataFieldType.Double:
				case DataFieldType.Decimal:
					key = new GroupIntervalInfoKey<bool>() { GroupIntervalType = GroupIntervalType.Numeric, Val = dimension.IsDiscreteNumericScale };
					break;
				case DataFieldType.DateTime:
					key = new GroupIntervalInfoKey<DateTimeGroupInterval>() { GroupIntervalType = GroupIntervalType.DateTime, Val = dimension.DateTimeGroupInterval };
					break;
			}
			if(key == null)
				return null;
			GroupIntervalInfo val;
			if(!cache.TryGetValue(key, out val)) {
				switch(fieldType) {
					case DataFieldType.Text:
						val = GetTextGroupIntervalInfo(dimension.TextGroupInterval);
						break;
					case DataFieldType.Integer:
					case DataFieldType.Float:
					case DataFieldType.Double:
					case DataFieldType.Decimal:
						val = GetNumericGroupIntervalInfo(dimension.IsDiscreteNumericScale);
						break;
					case DataFieldType.DateTime:
						val = CreateDateTimeGroupIntervalInfo(dimension.DateTimeGroupInterval);
						break;
				}
				cache[key] = val;
			}
			return val;
		}
		public abstract PivotGroupInterval PivotGroupInterval { get; }
		public virtual Type DataType { get { return null; } }
		public virtual Type ChartDataType { get { return typeof(string); } }
		public virtual bool IsDiscreteDate { get { return false; } }
	}
	public class AlphabeticalGroupIntervalInfo : GroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.Alphabetical; } }
	}
	public class ContinuousGroupIntervalInfo : GroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.Default; } }
		public override Type ChartDataType { get { return null; } }
	}
	public abstract class DiscreteDateGroupIntervalInfo : GroupIntervalInfo {
		public override bool IsDiscreteDate { get { return true; } }
	}
	public class YearGroupIntervalInfo : DiscreteDateGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.DateYear; } }
	}
	public class QuarterGroupIntervalInfo : DiscreteDateGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.DateQuarter; } }
	}
	public class MonthGroupIntervalInfo : DiscreteDateGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.DateMonth; } }
	}
	public class DayGroupIntervalInfo : DiscreteDateGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.DateDay; } }
	}
	public class HourGroupIntervalInfo : DiscreteDateGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.Hour; } }
	}
	public class MinuteGroupIntervalInfo : DiscreteDateGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.Minute; } }
	}
	public class SecondGroupIntervalInfo : DiscreteDateGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.Second; } }
	}
	public class DayOfYearGroupIntervalInfo : DiscreteDateGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.DateDayOfYear; } }
	}
	public class DayOfWeekGroupIntervalInfo : DiscreteDateGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.DateDayOfWeek; } }
	}
	public class WeekOfYearGroupIntervalInfo : DiscreteDateGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.DateWeekOfYear; } }
	}
	public class WeekOfMonthGroupIntervalInfo : DiscreteDateGroupIntervalInfo {
		public override PivotGroupInterval PivotGroupInterval { get { return PivotGroupInterval.DateWeekOfMonth; } }
	}
}
