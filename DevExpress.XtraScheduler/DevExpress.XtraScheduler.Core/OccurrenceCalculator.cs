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
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler {
	public abstract class OccurrenceCalculator {
		#region static
		public static OccurrenceCalculator CreateInstance(IRecurrenceInfo info) {
			return CreateInstance(info, new TimeZoneEngine());
		}
		public static OccurrenceCalculator CreateInstance(IRecurrenceInfo info, TimeZoneInfo operationTimeZone) {
			TimeZoneEngine timeZoneEngine = new TimeZoneEngine();
			timeZoneEngine.OperationTimeZone = operationTimeZone;
			return CreateInstance(info, timeZoneEngine);
		}
		internal static OccurrenceCalculator CreateInstance(IRecurrenceInfo info, TimeZoneEngine timeZoneEngine) {
			switch (info.Type) {
				case RecurrenceType.Daily:
					return CreateDailyCalculator(info, timeZoneEngine);
				case RecurrenceType.Weekly:
					return new WeeklyCalculator(info, timeZoneEngine);
				case RecurrenceType.Monthly:
					return CreateMonthlyCalculator(info, timeZoneEngine);
				case RecurrenceType.Yearly:
					return CreateYearlyCalculator(info, timeZoneEngine);
				case RecurrenceType.Hourly:
					return new EveryNHoursCalculator(info, timeZoneEngine);
				case RecurrenceType.Minutely:
					return new EveryNMinutesCalculator(info, timeZoneEngine);
				default:
					Exceptions.ThrowInternalException();
					return null;
			}
		}
		static OccurrenceCalculator CreateDailyCalculator(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine) {
			if (recurrenceInfo.WeekDays == WeekDays.EveryDay)
				return new EveryNDaysCalculator(recurrenceInfo, timeZoneEngine);
			else
				return new EveryCertainWeekDaysCalculator(recurrenceInfo, timeZoneEngine);
		}
		static OccurrenceCalculator CreateMonthlyCalculator(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine) {
			if (recurrenceInfo.WeekOfMonth == WeekOfMonth.None)
				return new EveryDayNumberMonthCalculator(recurrenceInfo, timeZoneEngine);
			else
				return new EveryWeekOfMonthCalculator(recurrenceInfo, timeZoneEngine);
		}
		static OccurrenceCalculator CreateYearlyCalculator(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine) {
			if (recurrenceInfo.WeekOfMonth == WeekOfMonth.None)
				return new EveryDayNumberYearlyCalculator(recurrenceInfo, timeZoneEngine);
			else
				return new EveryWeekOfMonthYearlyCalculator(recurrenceInfo, timeZoneEngine);
		}
		#endregion
		readonly IRecurrenceInfo recurrenceInfo;
		DateTime startTimeBase;
		int indexOffset;
		protected OccurrenceCalculator(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine) {
			if (recurrenceInfo == null)
				Exceptions.ThrowArgumentException("recurrenceInfo", recurrenceInfo);
			this.recurrenceInfo = recurrenceInfo;
			TimeZoneEngine = timeZoneEngine;
		}
		protected DateTime StartTimeBase { get { return startTimeBase; } set { startTimeBase = value; } }
		protected IRecurrenceInfo RecurrenceInfo { get { return recurrenceInfo; } }
		protected int IndexOffset { get { return indexOffset; } set { indexOffset = value; } }
		protected DateTime RecurrenceInfoStartInBaseTimeZone {
			get {
				if (TimeZoneEngine == null)
					return RecurrenceInfo.Start;
				return TimeZoneEngine.FromOperationTime(recurrenceInfo.Start, recurrenceInfo.TimeZoneId);
			} 
		}
		protected TimeZoneEngine TimeZoneEngine { get; private set; }
		public abstract DateTime CalcOccurrenceStartTime(int index);
		protected internal virtual DateTime GetActualRecurrenceEnd() {
			return recurrenceInfo.End.Date + DateTimeHelper.DaySpan;
		}
		protected internal virtual bool CanContainOccurrence(TimeInterval interval, TimeSpan occurrenceDuration) {
			if (interval.Start >= DateTime.MaxValue)
				return false;
			switch (recurrenceInfo.Range) {
				case RecurrenceRange.EndByDate:
					return interval.IntersectsWith(new TimeInterval(recurrenceInfo.Start, GetActualRecurrenceEnd()));
				case RecurrenceRange.NoEndDate:
					return interval.End >= recurrenceInfo.Start;
				case RecurrenceRange.OccurrenceCount:
					if (interval.End < recurrenceInfo.Start)
						return false;
					DateTime lastEndDate;
					try {
						lastEndDate = CalcOccurrenceStartTime(recurrenceInfo.OccurrenceCount - 1) + occurrenceDuration;
					}
					catch {
						lastEndDate = DateTime.MaxValue;
					}
					return interval.IntersectsWith(new TimeInterval(recurrenceInfo.Start, lastEndDate));
			}
			Exceptions.ThrowInternalException();
			return false;
		}
		internal DateTime FindNextOccurrenceTimeAfter(DateTime after, Appointment pattern, out int index) {
			after += pattern.Duration;
			after = after.AddTicks(1);
			index = FindNearestOccurrenceIndex(pattern, after);
			if (index < 0)
				return DateTime.MaxValue;
			DateTime result = CalcOccurrenceStartTime(index);
			TimeInterval interval = new TimeInterval(after, DateTime.MaxValue);
			if (IsOccurrenceIncorrect(interval, result, pattern.Duration, index)) {
				index = -1;
				return DateTime.MaxValue;
			}
			else
				return result;
		}
		internal int FindNearestOccurrenceIndex(Appointment pattern, DateTime time) {
			TimeInterval interval = new TimeInterval(time, DateTime.MaxValue);
			int index = FindFirstOccurrenceIndex(interval, pattern);
			if (index < 0)
				return index;
			for (; ; ) {
				if (pattern.FindException(index) == null)
					break;
				index++;
			}
			return index;
		}
		public DateTime FindNextOccurrenceTimeAfter(DateTime after, Appointment pattern) {
			int index;
			return FindNextOccurrenceTimeAfter(after, pattern, out index);
		}
		internal OccurrenceInfo FindMinTimeOfOccurrenceInsideInterval(TimeInterval interval, Appointment pattern, IPredicate<OccurrenceInfo> predicate) {
			int index = FindFirstOccurrenceIndex(interval, pattern);
			if (index < 0)
				return new OccurrenceInfo(pattern, DateTime.MaxValue, index);
			DateTime result = DateTime.MaxValue;
			int validIndex = -1;
			for (; ; ) {
				if (pattern.FindException(index) != null)
					index++;
				else {
					DateTime occurrenceTime = CalcOccurrenceStartTime(index);
					if (IsOccurrenceIncorrect(interval, occurrenceTime, pattern.Duration, index))
						break;
					if (occurrenceTime < interval.Start)
						index++;
					else if (occurrenceTime < interval.End) {
						if (predicate.Calculate(new OccurrenceInfo(pattern, occurrenceTime, index))) {
							result = occurrenceTime;
							validIndex = index;
							break;
						}
						else
							index++;
					}
					else
						break;
				}
			}
			return new OccurrenceInfo(pattern, result, validIndex);
		}
		internal OccurrenceInfo FindMaxTimeOfOccurrenceInsideInterval(TimeInterval interval, Appointment pattern, IPredicate<OccurrenceInfo> predicate) {
			if (interval.Duration.Ticks <= 0)
				return new OccurrenceInfo(pattern, DateTime.MinValue, -1);
			interval = new TimeInterval(interval.Start.AddTicks(1), interval.End);
			int index = FindFirstOccurrenceIndex(interval, pattern);
			if (index < 0)
				return new OccurrenceInfo(pattern, DateTime.MinValue, index);
			DateTime result = DateTime.MinValue;
			int validIndex = -1;
			for (; ; ) {
				if (pattern.FindException(index) != null)
					index++;
				else {
					DateTime occurrenceTime = CalcOccurrenceStartTime(index);
					if (IsOccurrenceIncorrect(interval, occurrenceTime, pattern.Duration, index))
						break;
					if (occurrenceTime < interval.End && occurrenceTime + pattern.Duration <= interval.End) {
						if (predicate.Calculate(new OccurrenceInfo(pattern, occurrenceTime, index))) {
							result = occurrenceTime;
							validIndex = index;
						}
						index++;
					}
					else
						break;
				}
			}
			return new OccurrenceInfo(pattern, result, validIndex);
		}
		internal Appointment CalcOccurrenceByIndex(int index, Appointment pattern) {
			DateTime occurrenceStart = CalcOccurrenceStartTime(index);
			return ((IInternalAppointment)pattern).RecurrenceChain.CreateOccurrence(occurrenceStart, index);
		}
		protected internal int CalcOccurrenceCount(TimeInterval interval, Appointment pattern) {
			return CalcOccurrenceCountCore(interval, pattern, Int32.MaxValue);
		}
		protected internal int CalcOccurrenceCountCore(TimeInterval interval, Appointment pattern, int maxValue) {
			int index = FindFirstOccurrenceIndex(interval, pattern);
			if (index < 0)
				return 0;
			for (; ; ) {
				DateTime occurrenceStart = CalcOccurrenceStartTime(index);
				if (IsOccurrenceIncorrect(interval, occurrenceStart, pattern.Duration, index))
					break;
				index++;
				if (index >= maxValue)
					return index;
			}
			return index;
		}
		public AppointmentBaseCollection CalcOccurrences(TimeInterval interval, Appointment pattern) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			result.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
			int index = FindFirstOccurrenceIndex(interval, pattern);
			if (index < 0)
				return result;
			for (; ; ) {
				DateTime occurrenceStart = CalcOccurrenceStartTime(index);
				if (IsOccurrenceIncorrect(interval, occurrenceStart, pattern.Duration, index))
					break;
				Appointment occurrence = ((IInternalAppointment)pattern).RecurrenceChain.CreateOccurrence(occurrenceStart, index);
#if (DEBUG)
				TimeInterval occurrenceInterval = ((IInternalAppointment)occurrence).GetInterval();
				if (occurrence.Duration.Ticks > 0) {
					if (interval.Duration.Ticks > 0)
						XtraSchedulerDebug.Assert(occurrenceInterval.IntersectsWithExcludingBounds(interval));
					else
						XtraSchedulerDebug.Assert(occurrence.Start == interval.Start);
				}
				else
					XtraSchedulerDebug.Assert(occurrenceInterval.IntersectsWith(interval));
#endif
				result.Add(occurrence);
				index++;
			}
			return result;
		}
		public int FindOccurrenceIndex(DateTime date, Appointment pattern) {
			return FindFirstOccurrenceIndex(new TimeInterval(date, DateTimeHelper.ZeroSpan), pattern);
		}
		public int FindFirstOccurrenceIndex(TimeInterval interval, Appointment pattern) {
			TimeSpan occurrenceDuration = pattern.Duration;
			if (!CanContainOccurrence(interval, occurrenceDuration))
				return -1;
			DateTime occurrenceStart;
			int index;
			if (occurrenceDuration > TimeSpan.Zero)
				index = FindFirstOccurrenceIndexCore(interval.Start, occurrenceDuration, out occurrenceStart);
			else
				index = FindFirstOccurrenceIndexCoreZeroOccurrenceDuration(interval.Start, out occurrenceStart);
			if (IsOccurrenceIncorrect(interval, occurrenceStart, pattern.Duration, index))
				return -1;
			return index;
		}
		public int FindLastOccurrenceIndex(TimeInterval interval, Appointment pattern) {
			TimeInterval actualInterval = interval.Clone();
			TimeSpan occurrenceDuration = pattern.Duration;
			if (!CanContainOccurrence(actualInterval, occurrenceDuration))
				return -1;
			DateTime occurrenceStart;
			if (recurrenceInfo.Range == RecurrenceRange.EndByDate) {
				DateTime actualRecurrenceEnd = GetActualRecurrenceEnd();
				if (actualInterval.End > actualRecurrenceEnd)
					actualInterval.End = actualRecurrenceEnd;
				if (actualInterval.Duration == TimeSpan.Zero && occurrenceDuration != TimeSpan.Zero)
					return -1;
			}
			else if (recurrenceInfo.Range == RecurrenceRange.OccurrenceCount) {
				DateTime actualRecurrenceEnd = CalcOccurrenceStartTime(recurrenceInfo.OccurrenceCount - 1) + occurrenceDuration;
				if (actualInterval.End > actualRecurrenceEnd) {
					actualInterval.End = actualRecurrenceEnd;
					if (actualInterval.Duration == TimeSpan.Zero && occurrenceDuration != TimeSpan.Zero)
						return -1;
					if (occurrenceDuration == TimeSpan.Zero)
						actualInterval.End += TimeSpan.FromTicks(1);
				}
			}
			int index = FindLastOccurrenceIndexCore(actualInterval.End, occurrenceDuration, out occurrenceStart);
			if (IsOccurrenceIncorrect(actualInterval, occurrenceStart, pattern.Duration, index))
				return -1;
			return index;
		}
		internal int FindLastOccurrenceIndexCore(DateTime intervalEnd, TimeSpan occurrenceDuration, out DateTime occurrenceStart) {
			int step = 16;
			int index = step - 1;
			for (; ; ) {
				occurrenceStart = CalcOccurrenceStartTime(index);
				if (occurrenceStart < intervalEnd) {
					index += step;
				}
				else {
					if (step == 1)
						break;
					step /= 2;
					index -= step;
				}
			}
			int resultIndex = index - 1;
			if (resultIndex > -1)
				occurrenceStart = CalcOccurrenceStartTime(resultIndex);
			return resultIndex;
		}
		internal int FindFirstOccurrenceIndexCore(DateTime intervalStart, TimeSpan occurrenceDuration, out DateTime occurrenceStart) {
			int step = 16;
			int index = step - 1;
			int forwardCount = 0;
			for (; ; ) {
				occurrenceStart = CalcOccurrenceStartTime(index);
				if (occurrenceStart + occurrenceDuration > intervalStart) {
					if (step == 1)
						break;
					step /= 2;
					index -= step;
					forwardCount = 0;
				}
				else {
					forwardCount++;
					if (forwardCount > step) { 
						step *= 2;
						forwardCount = 0;
					}
					index += step;					
				}			   
			}
			return index;
		}
		internal int FindFirstOccurrenceIndexCoreZeroOccurrenceDuration(DateTime intervalStart, out DateTime occurrenceStart) {
			int step = 16;
			int index = step - 1;
			for (; ; ) {
				occurrenceStart = CalcOccurrenceStartTime(index);
				if (occurrenceStart >= intervalStart) {
					if (step == 1)
						break;
					step /= 2;
					index -= step;
				}
				else
					index += step;
			}
			return index;
		}
		internal bool IsOccurrenceIncorrect(TimeInterval interval, DateTime occurrenceStart, TimeSpan occurrenceDuration, int occurrenceIndex) {
			if (recurrenceInfo.Range == RecurrenceRange.EndByDate && occurrenceStart >= GetActualRecurrenceEnd())
				return true;
			if (recurrenceInfo.Range == RecurrenceRange.OccurrenceCount && occurrenceIndex >= recurrenceInfo.OccurrenceCount)
				return true;
			if (occurrenceDuration == TimeSpan.Zero)
				occurrenceDuration = TimeSpan.FromTicks(1);
			return ((interval.Duration > TimeSpan.Zero) ? ((occurrenceStart >= interval.End) || (occurrenceStart + occurrenceDuration <= interval.Start)) : occurrenceStart > interval.End);
		}
		public virtual int CalcLastOccurrenceIndex(Appointment pattern) {
			IRecurrenceInfo recurrenceInfo = pattern.RecurrenceInfo;
			if (recurrenceInfo.Range == RecurrenceRange.OccurrenceCount) {
				DateTime startRange = recurrenceInfo.Start;
				DateTime endRange = CalcOccurrenceStartTime(recurrenceInfo.OccurrenceCount - 1);
				TimeInterval interval = new TimeInterval(startRange, endRange + DateTimeHelper.DaySpan);
				AppointmentBaseCollection apts = CalcOccurrences(interval, pattern);
				return apts.Count - 1;
			} else if (recurrenceInfo.Range == RecurrenceRange.EndByDate) {
				TimeInterval interval = new TimeInterval(recurrenceInfo.Start, recurrenceInfo.End.Date + DateTimeHelper.DaySpan);
				AppointmentBaseCollection apts = CalcOccurrences(interval, pattern);
				return apts.Count - 1;
			} else
				return -1;
		}
	}
}
namespace DevExpress.XtraScheduler.Native {
	#region OccurrenceInfo
	public class OccurrenceInfo {
		Appointment pattern;
		DateTime start;
		int index;
		public OccurrenceInfo(Appointment pattern, DateTime start, int index) {
			this.pattern = pattern;
			this.start = start;
			this.index = index;
		}
		public Appointment Pattern { get { return pattern; } }
		public DateTime Start { get { return start; }  }
		public int Index { get { return index; }  }
	}
	#endregion
	#region OccurrenceInfoPredicate
	public abstract class OccurrenceInfoPredicate : SchedulerPredicate<OccurrenceInfo> {
	}
	#endregion
	#region EmptyOccurrenceInfoPredicate
	public class EmptyOccurrenceInfoPredicate : SchedulerEmptyPredicate<OccurrenceInfo> {
	}
	#endregion
	#region FilterOccurrenceInfoPredicate
	public class FilterOccurrenceInfoPredicate : OccurrenceInfoPredicate {
		IPredicate<Appointment> innerPredicate;
		public FilterOccurrenceInfoPredicate(IPredicate<Appointment> innerPredicate) {
			if (innerPredicate == null)
				Exceptions.ThrowArgumentNullException("innerPredicate");
			this.innerPredicate = innerPredicate;
		}
		protected internal IPredicate<Appointment> InnerPredicate { get { return innerPredicate; } }
		public override bool Calculate(OccurrenceInfo obj) {
			using (Appointment occurrence = ((IInternalAppointment)obj.Pattern).RecurrenceChain.CreateOccurrence(obj.Start, obj.Index)) {
				return innerPredicate.Calculate(occurrence);
			}
		}
	}
	#endregion
	#region EveryNMinutesCalculator
	public class EveryNMinutesCalculator : OccurrenceCalculator {
		public EveryNMinutesCalculator(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine)
			: base(recurrenceInfo, timeZoneEngine) {
			if (recurrenceInfo.Type != RecurrenceType.Minutely)
				Exceptions.ThrowArgumentException("recurrenceInfo.Type", recurrenceInfo.Type);
		}
		public override DateTime CalcOccurrenceStartTime(int index) {
			DateTime recurrenceInfoStart = RecurrenceInfo.Start;
			if (TimeZoneEngine != null) {
				recurrenceInfoStart = TimeZoneEngine.ValidateDateTime(recurrenceInfoStart, TimeZoneEngine.OperationTimeZone);
				recurrenceInfoStart = TimeZoneEngine.FromOperationTimeToUtc(recurrenceInfoStart);
			}
			DateTime occurrenceStart = recurrenceInfoStart.AddMinutes(RecurrenceInfo.Periodicity * index);
			if (TimeZoneEngine == null)
				return occurrenceStart;
			return TimeZoneEngine.ToOperationTimeFromUtc(occurrenceStart);
		}
		protected internal override DateTime GetActualRecurrenceEnd() {
			return DateTimeHelper.Floor(RecurrenceInfo.End, TimeSpan.FromMinutes(1)) + TimeSpan.FromMinutes(1);
		}
	}
	#endregion
	#region EveryNHoursCalculator
	public class EveryNHoursCalculator : OccurrenceCalculator {
		public EveryNHoursCalculator(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine)
			: base(recurrenceInfo, timeZoneEngine) {
			if (recurrenceInfo.Type != RecurrenceType.Hourly)
				Exceptions.ThrowArgumentException("recurrenceInfo.Type", recurrenceInfo.Type);
		}
		public override DateTime CalcOccurrenceStartTime(int index) {
			DateTime recurrenceInfoStart = RecurrenceInfo.Start;
			if (TimeZoneEngine != null) {
				recurrenceInfoStart = TimeZoneEngine.ValidateDateTime(recurrenceInfoStart, TimeZoneEngine.OperationTimeZone);
				recurrenceInfoStart = TimeZoneEngine.FromOperationTimeToUtc(recurrenceInfoStart);
			}
			DateTime occurrenceStart = recurrenceInfoStart.AddHours(RecurrenceInfo.Periodicity * index);
			if (TimeZoneEngine == null)
			return occurrenceStart;
			return TimeZoneEngine.ToOperationTimeFromUtc(occurrenceStart);
		}
		protected internal override DateTime GetActualRecurrenceEnd() {
			return DateTimeHelper.Floor(RecurrenceInfo.End, DateTimeHelper.HourSpan) + DateTimeHelper.HourSpan;
		}
	}
	#endregion
	#region EveryNDaysCalculator
	public class EveryNDaysCalculator : OccurrenceCalculator {
		public EveryNDaysCalculator(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine)
			: base(recurrenceInfo, timeZoneEngine) {
			if (recurrenceInfo.Type != RecurrenceType.Daily)
				Exceptions.ThrowArgumentException("recurrenceInfo.Type", recurrenceInfo.Type);
			if (recurrenceInfo.WeekDays != WeekDays.EveryDay)
				Exceptions.ThrowArgumentException("recurrenceInfo.WeekDays", recurrenceInfo.WeekDays);
		}
		public override DateTime CalcOccurrenceStartTime(int index) {
			DateTime occurrenceStart = RecurrenceInfoStartInBaseTimeZone.AddDays(RecurrenceInfo.Periodicity * index);
			if (TimeZoneEngine != null) {
				occurrenceStart = TimeZoneEngine.ValidateDateTime(occurrenceStart, RecurrenceInfo.TimeZoneId);
				return TimeZoneEngine.ToOperationTime(occurrenceStart, RecurrenceInfo.TimeZoneId);
			}
			return occurrenceStart;
		}
	}
	#endregion
	#region WeeklyCalculator
	public class WeeklyCalculator : OccurrenceCalculator {
		TimeSpan[] offsets;
		int offsetCount;
		int weeksInterval;
		protected WeeklyCalculator(IRecurrenceInfo recurrenceInfo, int weeksInterval, TimeZoneEngine timeZoneEngine)
			: base(recurrenceInfo, timeZoneEngine) {
			this.weeksInterval = weeksInterval;
			this.StartTimeBase = CalcStartTimeBase();
			this.offsets = CalcOffsets();
			this.offsetCount = offsets.Length;
			if (this.offsetCount <= 0)
				Exceptions.ThrowArgumentException("recurrenceInfo.WeekDays", recurrenceInfo.WeekDays);
		}
		public WeeklyCalculator(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine)
			: this(recurrenceInfo, recurrenceInfo != null ? recurrenceInfo.Periodicity : 1, timeZoneEngine) {
			if (recurrenceInfo.Type != RecurrenceType.Weekly)
				Exceptions.ThrowArgumentException("recurrenceInfo.Type", recurrenceInfo.Type);
		}
		protected virtual DateTime GetStartOfWeek(DateTime date) {
			return DateTimeHelper.GetStartOfWeekUI(RecurrenceInfoStartInBaseTimeZone, RecurrenceInfo.FirstDayOfWeek);
		}
		DateTime CalcStartTimeBase() {
			return GetStartOfWeek(RecurrenceInfoStartInBaseTimeZone) + RecurrenceInfoStartInBaseTimeZone.TimeOfDay;
		}
		TimeSpan[] CalcOffsets() {
			List<TimeSpan> offsetList = new List<TimeSpan>();
			TimeSpan offset = TimeSpan.Zero;
			for (int i = 0; i < 7; i++) {
				DateTime date = StartTimeBase + offset;
				DayOfWeek dayOfWeek = date.DayOfWeek;
				if ((RecurrenceInfo.WeekDays & DateTimeHelper.ToWeekDays(dayOfWeek)) != 0) {
					offsetList.Add(offset);
					if (date < RecurrenceInfoStartInBaseTimeZone)
						IndexOffset = offsetList.Count;
				}
				offset += DateTimeHelper.DaySpan;
			}
			return offsetList.ToArray();
		}
		public override DateTime CalcOccurrenceStartTime(int index) {
			index += IndexOffset;
			int weeksOffset = index / offsetCount;
			int offset = index % offsetCount;
			DateTime occurrenceStart = StartTimeBase + TimeSpan.FromDays(7 * weeksInterval * weeksOffset) +
				offsets[offset];
			if (TimeZoneEngine != null) {
				occurrenceStart = TimeZoneEngine.ValidateDateTime(occurrenceStart, RecurrenceInfo.TimeZoneId);
				return TimeZoneEngine.ToOperationTime(occurrenceStart, RecurrenceInfo.TimeZoneId);
			}
			return occurrenceStart;
		}
	}
	#endregion
	#region EveryCertainWeekDaysCalculator
	public class EveryCertainWeekDaysCalculator : WeeklyCalculator {
		public EveryCertainWeekDaysCalculator(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine)
			: base(recurrenceInfo, 1, timeZoneEngine) {
			if (recurrenceInfo.Type != RecurrenceType.Daily)
				Exceptions.ThrowArgumentException("recurrenceInfo.Type", recurrenceInfo.Type);
			if (recurrenceInfo.WeekDays == WeekDays.EveryDay)
				Exceptions.ThrowArgumentException("recurrenceInfo.WeekDays", recurrenceInfo.WeekDays);
		}
	}
	#endregion
	#region EveryDayNumberCalculatorBase
	public abstract class EveryDayNumberCalculatorBase : OccurrenceCalculator {
		protected EveryDayNumberCalculatorBase(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine)
			: base(recurrenceInfo, timeZoneEngine) {
			if (recurrenceInfo.WeekOfMonth != WeekOfMonth.None)
				Exceptions.ThrowArgumentException("recurrenceInfo.WeekOfMonth", recurrenceInfo.WeekOfMonth);
		}
		protected abstract DateTime OffsetDate(DateTime date);
		public override DateTime CalcOccurrenceStartTime(int index) {
			index += IndexOffset;
			DateTime date = StartTimeBase;
			for (int i = 0; i < index; i++)
				date = OffsetDate(date);
			int daysCount = DateTime.DaysInMonth(date.Year, date.Month);
			int actualDayNumber = daysCount < RecurrenceInfo.DayNumber ? daysCount : RecurrenceInfo.DayNumber;
			date = date.AddDays(actualDayNumber - 1);
			if (TimeZoneEngine != null)
				return TimeZoneEngine.ToOperationTime(date, RecurrenceInfo.TimeZoneId);
			return date;
		}
	}
	#endregion
	#region EveryDayNumberMonthCalculator
	public class EveryDayNumberMonthCalculator : EveryDayNumberCalculatorBase {
		public EveryDayNumberMonthCalculator(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine)
			: base(recurrenceInfo, timeZoneEngine) {
			if (recurrenceInfo.Type != RecurrenceType.Monthly)
				Exceptions.ThrowArgumentException("recurrenceInfo.Type", recurrenceInfo.Type);
			this.IndexOffset = (recurrenceInfo.DayNumber < recurrenceInfo.Start.Day) ? 1 : 0;
			DateTime start = RecurrenceInfoStartInBaseTimeZone;
			this.StartTimeBase = new DateTime(start.Year, start.Month, 1) + start.TimeOfDay;
		}
		protected override DateTime OffsetDate(DateTime date) {
			return date.AddMonths(RecurrenceInfo.Periodicity);
		}
	}
	#endregion
	#region EveryDayNumberYearlyCalculator
	public class EveryDayNumberYearlyCalculator : EveryDayNumberCalculatorBase {
		public EveryDayNumberYearlyCalculator(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine)
			: base(recurrenceInfo, timeZoneEngine) {
			if (recurrenceInfo.Type != RecurrenceType.Yearly)
				Exceptions.ThrowArgumentException("recurrenceInfo.Type", recurrenceInfo.Type);
			DateTime start = RecurrenceInfoStartInBaseTimeZone;
			if (recurrenceInfo.Month < start.Month ||
				(recurrenceInfo.Month == start.Month && recurrenceInfo.DayNumber < start.Day))
				this.IndexOffset = 1;
			else
				this.IndexOffset = 0;
			this.StartTimeBase = new DateTime(start.Year, recurrenceInfo.Month, 1, start.Hour, start.Minute, start.Second, start.Millisecond);
		}
		protected override DateTime OffsetDate(DateTime date) {
			return date.AddYears(RecurrenceInfo.Periodicity);
		}
	}
	#endregion
	#region EveryWeekOfMonthCalculatorBase
	public abstract class EveryWeekOfMonthCalculatorBase : OccurrenceCalculator {
		List<DateTime> matches = new List<DateTime>(31);
		protected EveryWeekOfMonthCalculatorBase(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine)
			: base(recurrenceInfo, timeZoneEngine) {
			if (recurrenceInfo.WeekOfMonth == WeekOfMonth.None)
				Exceptions.ThrowArgumentException("recurrenceInfo.WeekOfMonth", recurrenceInfo.WeekOfMonth);
		}
		protected abstract DateTime OffsetDate(DateTime date, int index);
		public override DateTime CalcOccurrenceStartTime(int index) {
			index += IndexOffset;
			matches.Clear();
			DateTime date = StartTimeBase;
			date = OffsetDate(date, index);
			int dayCount = DateTime.DaysInMonth(date.Year, date.Month);
			for (int i = 0; i < dayCount; i++) {
				if ((DateTimeHelper.ToWeekDays(date.DayOfWeek) & RecurrenceInfo.WeekDays) != 0)
					matches.Add(date);
				date = date.AddDays(1);
			}
			DateTime occurrenceStart = ChooseMatch();
			if (TimeZoneEngine != null)
				return TimeZoneEngine.ToOperationTime(occurrenceStart, RecurrenceInfo.TimeZoneId);
			return occurrenceStart;
		}
		protected int CalcIndexOffset() {
			int index = 0;
			while (CalcOccurrenceStartTime(index) < RecurrenceInfo.Start)
				index++;
			return index;
		}
		DateTime ChooseMatch() {
			if (matches.Count < 4)
				Exceptions.ThrowInternalException();
			switch (RecurrenceInfo.WeekOfMonth) {
				case WeekOfMonth.First:
					return matches[0];
				case WeekOfMonth.Second:
					return matches[1];
				case WeekOfMonth.Third:
					return matches[2];
				case WeekOfMonth.Fourth:
					return matches[3];
				case WeekOfMonth.Last:
					return matches[matches.Count - 1];
				default:
					Exceptions.ThrowInternalException();
					return DateTime.MinValue;
			}
		}
	}
	#endregion
	#region EveryWeekOfMonthCalculator
	public class EveryWeekOfMonthCalculator : EveryWeekOfMonthCalculatorBase {
		public EveryWeekOfMonthCalculator(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine)
			: base(recurrenceInfo, timeZoneEngine) {
			if (recurrenceInfo.Type != RecurrenceType.Monthly)
				Exceptions.ThrowArgumentException("recurrenceInfo.Type", recurrenceInfo.Type);
			DateTime start = RecurrenceInfoStartInBaseTimeZone;
			this.StartTimeBase = new DateTime(start.Year, start.Month, 1, start.Hour, start.Minute, start.Second, start.Millisecond);
			this.IndexOffset = CalcIndexOffset();
		}
		protected override DateTime OffsetDate(DateTime date, int index) {
			return date.AddMonths(index * RecurrenceInfo.Periodicity);
		}
	}
	#endregion
	#region EveryWeekOfMonthYearlyCalculator
	public class EveryWeekOfMonthYearlyCalculator : EveryWeekOfMonthCalculatorBase {
		public EveryWeekOfMonthYearlyCalculator(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine)
			: base(recurrenceInfo, timeZoneEngine) {
			if (recurrenceInfo.Type != RecurrenceType.Yearly)
				Exceptions.ThrowArgumentException("recurrenceInfo.Type", recurrenceInfo.Type);
			DateTime start = RecurrenceInfoStartInBaseTimeZone;
			this.StartTimeBase = new DateTime(start.Year, recurrenceInfo.Month, 1, start.Hour, start.Minute, start.Second, start.Millisecond);
			this.IndexOffset = CalcIndexOffset();
		}
		protected override DateTime OffsetDate(DateTime date, int index) {
			return date.AddYears(index * RecurrenceInfo.Periodicity);
		}
	}
	#endregion
}
