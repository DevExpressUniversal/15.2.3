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
using DevExpress.XtraScheduler.Native;
using System.Globalization;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Reporting.Native {
	public abstract class TimeIntervalFormatterBase {
		public static TimeIntervalFormatterBase CreateInstance(TimeIntervalFormatType type) {
			switch (type) {
				case TimeIntervalFormatType.Daily:
					return new DailyTimeIntervalFormatter();
				case TimeIntervalFormatType.Weekly:
					return new WeeklyTimeIntervalFormatter();
				case TimeIntervalFormatType.Monthly:
					return new MonthlyTimeIntervalFormatter();
				case TimeIntervalFormatType.Timeline:
					return new TimelineTimeIntervalFormatter();
				default:
					return new DailyTimeIntervalFormatter();
			}
		}
		public abstract TimeIntervalFormatType Type { get; }
		public virtual float SecondLineTextSizeMultiplier { get { return 1.0f; } }
		public abstract string FormatFirstLineText(TimeInterval currentInterval);
		public abstract string FormatSecondLineText(TimeInterval currentInterval);
		protected internal virtual void Initialize(ReportViewBase View) {
		}
	}
	public class DailyTimeIntervalFormatter : TimeIntervalFormatterBase {
		public override float SecondLineTextSizeMultiplier { get { return 0.618f; } }
		public override TimeIntervalFormatType Type { get { return TimeIntervalFormatType.Daily; } }
		public override string FormatFirstLineText(TimeInterval currentInterval) {
			DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
			string pattern = DateTimeFormatHelper.StripDayOfWeek(dtfi.LongDatePattern);
			return SysDate.ToString(pattern, currentInterval.Start);
		}
		public override string FormatSecondLineText(TimeInterval currentInterval) {
			return SysDate.ToString("dddd", currentInterval.Start);
		}
	}
	public class WeeklyTimeIntervalFormatter : TimeIntervalFormatterBase {
		public override TimeIntervalFormatType Type { get { return TimeIntervalFormatType.Weekly; } }
		public override string FormatFirstLineText(TimeInterval currentInterval) {
			DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
			string pattern = DateTimeFormatHelper.StripDayOfWeek(dtfi.LongDatePattern);
			pattern = DateTimeFormatHelper.StripYear(pattern);
			return String.Format("{0} -", SysDate.ToString(pattern, currentInterval.Start));
		}
		public override string FormatSecondLineText(TimeInterval currentInterval) {
			DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
			string pattern = DateTimeFormatHelper.StripDayOfWeek(dtfi.LongDatePattern);
			return SysDate.ToString(pattern, currentInterval.End.AddDays(-1));
		}
	}
	public class MonthlyTimeIntervalFormatter : TimeIntervalFormatterBase {
		public override TimeIntervalFormatType Type { get { return TimeIntervalFormatType.Monthly; } }
		public override string FormatFirstLineText(TimeInterval currentInterval) {
			DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
			string pattern = DateTimeFormatHelper.StripDayOfWeek(dtfi.LongDatePattern);
			pattern = DateTimeFormatHelper.StripDay(pattern);
			return SysDate.ToString(pattern, currentInterval.Start);
		}
		public override string FormatSecondLineText(TimeInterval currentInterval) {
			return String.Empty;
		}
	}
	public class TimelineTimeIntervalFormatter : TimeIntervalFormatterBase {
		TimeScale scale;
		public override TimeIntervalFormatType Type { get { return TimeIntervalFormatType.Timeline; } }
		public TimelineTimeIntervalFormatter() { 
		}
		public TimeScale Scale { get { return scale; } }
		protected internal override void Initialize(ReportViewBase view) {
			ReportTimelineView timeLine = view as ReportTimelineView;
			if (timeLine != null)
				scale = timeLine.GetBaseTimeScale();
			else
				scale = new TimeScaleDay();
		}
		public override string FormatFirstLineText(TimeInterval currentInterval) {
			XtraSchedulerDebug.Assert(Scale != null);
			DateTime start = currentInterval.Start;
			DateTime end = Scale.GetPrevDate(currentInterval.End);
			if (Scale.SortingWeight < TimeSpan.FromDays(7))
				return String.Format("{0} - {1}", Scale.FormatCaption(start, end), Scale.FormatCaption(end, end));
			return Scale.FormatCaption(start, end);
		}
		public override string FormatSecondLineText(TimeInterval currentInterval) {
			return string.Empty;
		}
	}
	#region SimpleVisibleIntervalFormatter
	public class SimpleVisibleIntervalFormatter : TimeIntervalFormatterBase {
		string firstLineFormat;
		public SimpleVisibleIntervalFormatter() { 
		}
		public override TimeIntervalFormatType Type { get { return TimeIntervalFormatType.Default; } }
		public string FirstLineFormat { get { return firstLineFormat; } set { firstLineFormat = value; } }
		public override string FormatFirstLineText(TimeInterval currentInterval) {
			try {
				return String.Format(FirstLineFormat, currentInterval.Start, currentInterval.End.AddTicks(-1));
			} catch {
				return string.Empty;
			}
		}
		public override string FormatSecondLineText(TimeInterval currentInterval) {
			return string.Empty;
		}
	}
	#endregion
	#region AutomaticVisibleIntervalFormatter
	public class AutomaticVisibleIntervalFormatter : TimeIntervalFormatterBase {
		internal const string DefaultIntervalFormat = "{0:D} - {1:D}";
		string firstLineFormat = DefaultIntervalFormat;
		VisibleIntervalFormatter innerFormatter;
		public AutomaticVisibleIntervalFormatter() {
			this.innerFormatter = new VisibleIntervalFormatter();
		}
		protected VisibleIntervalFormatter InnerFormatter { get { return innerFormatter; } }
		public override TimeIntervalFormatType Type { get { return TimeIntervalFormatType.Default; } }
		public string FirstLineFormat { get { return firstLineFormat; } set { firstLineFormat = value; } }
		public bool FormatForMonth { get { return InnerFormatter.FormatForMonth; } set { innerFormatter.FormatForMonth = value; } }
		public override string FormatFirstLineText(TimeInterval currentInterval) {
			return InnerFormatter.Format(currentInterval, FirstLineFormat);
		}
		public override string FormatSecondLineText(TimeInterval currentInterval) {
			return string.Empty;
		}
	}
	#endregion
}
