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
namespace DevExpress.DashboardCommon.Native {
	public class DimensionGroupIntervalInfo {
		public static DimensionGroupIntervalInfo Default {
			get {
				return new DimensionGroupIntervalInfo {
					TextGroupIntervals = GetTextGroupIntervals(),
					DateTimeDiscreteGroupIntervals = GetDateTimeDiscreteGroupIntervals(),
					DateTimeContinuousGroupIntervalsButExactDate = GetDateTimeContinuousGroupIntervalsButExactDate(false),
					IsSupportExactDateGroupInterval = true
				};
			}
		}
		public static DimensionGroupIntervalInfo Continuous {
			get {
				return new DimensionGroupIntervalInfo {
					DateTimeContinuousGroupIntervalsButExactDate = GetDateTimeContinuousGroupIntervalsButExactDate(true),
					IsSupportExactDateGroupInterval = true
				};
			}
		}
		public static DimensionGroupIntervalInfo Empty {
			get {
				return new DimensionGroupIntervalInfo();
			}
		}
		static IList<TextGroupInterval> GetTextGroupIntervals() {
			return new List<TextGroupInterval>((IEnumerable<TextGroupInterval>)Enum.GetValues(typeof(TextGroupInterval)));
		}
		static IList<DateTimeGroupInterval> GetDateTimeDiscreteGroupIntervals() {
			List<DateTimeGroupInterval> groupIntervals = new List<DateTimeGroupInterval>();
			groupIntervals.AddRange(((IEnumerable<DateTimeGroupInterval>)Enum.GetValues(typeof(DateTimeGroupInterval))).Where(grInt => !IsContinuousDateTimeGroupInterval(grInt)));
			return groupIntervals;
		}
		static IList<DateTimeGroupInterval> GetDateTimeContinuousGroupIntervalsButExactDate(bool includeYear) {
			List<DateTimeGroupInterval> groupIntervals = new List<DateTimeGroupInterval>();
			if(includeYear)
				groupIntervals.Add(DateTimeGroupInterval.Year);
			groupIntervals.AddRange(((IEnumerable<DateTimeGroupInterval>)Enum.GetValues(typeof(DateTimeGroupInterval))).Where(grInt => IsContinuousDateTimeGroupInterval(grInt) && grInt != DateTimeGroupInterval.None));
			return groupIntervals;
		}
		static bool IsContinuousDateTimeGroupInterval(DateTimeGroupInterval groupInterval) {
			return
				groupInterval == DateTimeGroupInterval.None ||
				groupInterval == DateTimeGroupInterval.MonthYear ||
				groupInterval == DateTimeGroupInterval.QuarterYear ||
				groupInterval == DateTimeGroupInterval.DayMonthYear ||
				groupInterval == DateTimeGroupInterval.DateHour ||
				groupInterval == DateTimeGroupInterval.DateHourMinute ||
				groupInterval == DateTimeGroupInterval.DateHourMinuteSecond;
		}
		public IList<TextGroupInterval> TextGroupIntervals { get; set; }
		public IList<DateTimeGroupInterval> DateTimeDiscreteGroupIntervals { get; set; }
		public IList<DateTimeGroupInterval> DateTimeContinuousGroupIntervalsButExactDate { get; set; }
		public bool IsSupportExactDateGroupInterval { get; set; }
		public bool IsSupportNumericGroupIntervals { get; set; }
	}
}
