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
using System.Collections.Specialized;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler {
	public interface ITimeScaleDateTimeFormatter {
		string Format(string format, DateTime start, DateTime end);
		StringCollection GetDateTimeAutoFormats();
		bool SupportsAutoFormats { get; }
	}
	public interface ITimeScaleDateTimeFormatterFactory {
		ITimeScaleDateTimeFormatter CreateFormatter(TimeScale scale);
	}
}
namespace DevExpress.XtraScheduler.Native {
	public class TimeScaleDateTimeFormatterFactory : ITimeScaleDateTimeFormatterFactory {
		#region static
		readonly static TimeScaleDateTimeFormatterFactory defaultFactory;
		public static TimeScaleDateTimeFormatterFactory Default { get { return defaultFactory; } }
		static TimeScaleDateTimeFormatterFactory() {
			defaultFactory = new TimeScaleDateTimeFormatterFactory();
		}
		#endregion
		public virtual ITimeScaleDateTimeFormatter CreateFormatter(TimeScale scale) {
			if (scale == null)
				return null;
			Type scaleType = scale.GetType();
			if (typeof(TimeScaleYear).IsAssignableFrom(scaleType))
				return new TimeScaleYearFormatter();
			if (typeof(TimeScaleQuarter).IsAssignableFrom(scaleType))
				return new TimeScaleQuarterFormatter();
			if (typeof(TimeScaleMonth).IsAssignableFrom(scaleType))
				return new TimeScaleMonthFormatter();
			if (typeof(TimeScaleWeek).IsAssignableFrom(scaleType))
				return new TimeScaleWeekFormatter();
			if (typeof(TimeScaleDay).IsAssignableFrom(scaleType))
				return new TimeScaleDayFormatter();
			if (typeof(TimeScaleHour).IsAssignableFrom(scaleType))
				return new TimeScaleHourFormatter();
			if (typeof(TimeScaleFixedInterval).IsAssignableFrom(scaleType))
				return new TimeScaleFixedIntervalFormatter();
			return new TimeScaleFormatter();
		}
	}
	public class TimeScaleWeekNumberFormatterFactory : TimeScaleDateTimeFormatterFactory {
		public override ITimeScaleDateTimeFormatter CreateFormatter(TimeScale scale) {
			if (typeof(TimeScaleWeek).IsAssignableFrom(scale.GetType()))
				return new TimeScaleWeekNumberFormatter();
			return base.CreateFormatter(scale);
		}
	}
	public abstract class TimeScaleDateTimeFormatterBase : ITimeScaleDateTimeFormatter {
		public abstract string Format(string format, DateTime start, DateTime end);
		public virtual StringCollection GetDateTimeAutoFormats() {
			return new StringCollection();
		}
		public virtual bool SupportsAutoFormats { get { return false; } }
	}
	public class TimeScaleFormatter : TimeScaleDateTimeFormatterBase {
		public override string Format(string format, DateTime start, DateTime end) {
			return SysDate.ToString(format, start);
		}
		public override StringCollection GetDateTimeAutoFormats() {
			return new StringCollection();
		}
	}
	public class TimeScaleFixedIntervalFormatter : TimeScaleDateTimeFormatterBase {
		public override string Format(string format, DateTime start, DateTime end) {
			return start.ToString(format);
		}
		public override StringCollection GetDateTimeAutoFormats() {
			return base.GetDateTimeAutoFormats();
		}
	}
	public class TimeScaleHourFormatter : TimeScaleFormatter {
		public override StringCollection GetDateTimeAutoFormats() {
			return DateTimeFormatHelper.GenerateFormatsTimeOnly();
		}
	}
	public class TimeScaleDayFormatter : TimeScaleFormatter {
		public override StringCollection GetDateTimeAutoFormats() {
			return DateTimeFormatHelper.GenerateFormatsWithoutYear();
		}
		public override bool SupportsAutoFormats { get { return true; } }
	}
	public class TimeScaleWeekFormatter : TimeScaleFixedIntervalFormatter {
		public override string Format(string format, DateTime start, DateTime end) {
			if (end < DateTime.MinValue.AddDays(1))
				return String.Format("{0} - {1}", base.Format(format, start, end), base.Format(format, end, end));
			DateTime correctedEnd = end.AddDays(-1);
			return String.Format("{0} - {1}", base.Format(format, start, correctedEnd), base.Format(format, correctedEnd, correctedEnd));
		}
		public override bool SupportsAutoFormats { get { return true; } }
		public override StringCollection GetDateTimeAutoFormats() {
			return DateTimeFormatHelper.GenerateWeekAutoFormats();
		}
	}
	public class TimeScaleWeekNumberFormatter : TimeScaleWeekFormatter {
		public override string Format(string format, DateTime start, DateTime end) {
			string dateString = "w{0}-w{1}";
			return String.Format(dateString, DateTimeHelper.GetWeekOfYear(start), DateTimeHelper.GetWeekOfYear(end));
		}
	}
	public class TimeScaleMonthFormatter : TimeScaleFormatter {
		public override StringCollection GetDateTimeAutoFormats() {
			return DateTimeFormatHelper.GenerateFormatsDateWithYear();
		}
	}
	public class TimeScaleQuarterFormatter : TimeScaleFormatter {
		public override string Format(string format, DateTime start, DateTime end) {
			string dateString = base.Format(format, start, end);
			return String.Format(dateString, DateTimeHelper.CalcQuarterNumber(start.Month));
		}
	}
	public class TimeScaleYearFormatter : TimeScaleFormatter {
		public override StringCollection GetDateTimeAutoFormats() {
			return DateTimeFormatHelper.GenerateFormatsDateWithYear();
		}
	}
}
