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
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Compatibility;
namespace DevExpress.XtraScheduler.Native {
	public interface IStatusBrushAdapter {
		Image CreateImage(Rectangle bounds, IAppointmentStatus status);
		void ClearCache();
	}
	public static class AppointmentIntersectionHelper {
		public static bool EqualsToInterval(Appointment apt, TimeInterval interval) {
			XtraSchedulerDebug.CheckNotNull("apt", apt);
			return ((IInternalAppointment)apt).GetInterval().Equals(interval);
		}
		public static bool IntersectsWithInterval(Appointment apt, TimeInterval interval) {
			XtraSchedulerDebug.CheckNotNull("apt", apt);
			if (interval.Duration == TimeSpan.Zero)
				return IntersectsWithZeroDurationInterval(apt, interval);
			else
				return IntersectsWithNonZeroDurationInterval(apt, interval);
		}
		internal static bool IntersectsWithZeroDurationInterval(Appointment apt, TimeInterval interval) {
			XtraSchedulerDebug.Assert(interval.Duration == TimeSpan.Zero);
			TimeInterval aptInterval = ((IInternalAppointment)apt).GetInterval();
			return interval.IntersectsWith(aptInterval);
		}
		internal static bool IntersectsWithNonZeroDurationInterval(Appointment apt, TimeInterval interval) {
			XtraSchedulerDebug.Assert(interval.Duration != TimeSpan.Zero);
			TimeInterval aptInterval = ((IInternalAppointment)apt).GetInterval();
			return interval.IntersectsWithExcludingBounds(aptInterval) || (aptInterval.Duration == TimeSpan.Zero && interval.IntersectsWith(aptInterval));
		}
	}
	#region AppointmentStartComparer
	public class AppointmentStartComparer : IComparable<Appointment> {
		readonly DateTime date;
		readonly TimeInterval interval;
		public AppointmentStartComparer(DateTime date) {
			this.date = date;
			this.interval = new TimeInterval(date, TimeSpan.Zero);
		}
		protected DateTime Date { get { return date; } }
		#region IComparable<Appointment> Members
		public int CompareTo(Appointment other) {
			if (IsAppointmentBeforeDate(other))
				return -1;
			if (IsAppointmentAfterDate(other))
				return 1;
			return 0;
		}
		protected bool IsAppointmentBeforeDate(Appointment apt) {
			return apt.Start < Date && !AppointmentIntersectionHelper.IntersectsWithInterval(apt, interval);
		}
		protected bool IsAppointmentAfterDate(Appointment apt) {
			return apt.Start > Date && apt.End > Date;
		}
		#endregion
	}
	#endregion
	#region SortedAppointmentBaseCollection
	public class SortedAppointmentBaseCollection : AppointmentBaseCollection {
		#region static
		static IComparer<Appointment> defaultComparer;
		static SortedAppointmentBaseCollection() {
			defaultComparer = new AppointmentTimeIntervalComparer();
		}
		public static IComparer<Appointment> DefaultComparer { get { return defaultComparer; } }
		#endregion
		bool sortLocked;
		IComparer<Appointment> comparer;
		public SortedAppointmentBaseCollection() {
		}
		public SortedAppointmentBaseCollection(IComparer<Appointment> comparer) {
			this.comparer = comparer;
		}
		protected bool SortLocked { get { return sortLocked; } }
		public IComparer<Appointment> Comparer {
			get {
				return (comparer != null) ? comparer : DefaultComparer;
			}
			set {
				if (comparer == value)
					return;
				this.comparer = value;
				OnComparerChanged();
			}
		}
		public virtual void AddSortedRange(ICollection collection) {
			this.sortLocked = true;
			try {
				base.AddRange(collection);
			} finally {
				this.sortLocked = false;
			}
		}
		protected virtual void OnComparerChanged() {
			SortItems();
		}
		public override void Sort(IComparer<Appointment> comparer) {
			this.comparer = comparer;
			SortItems();
		}
		protected internal virtual void SortItems() {
			if (SortLocked || IsUpdateLocked)
				return;
			PerformSort();
		}
		protected internal virtual void PerformSort() {
			base.Sort(Comparer);
		}
		protected override void OnLastEndUpdate() {
			base.OnLastEndUpdate();
			SortItems();
		}
		protected override void OnInsertComplete(int index, Appointment value) {
			base.OnInsertComplete(index, value);
			SortItems();
		}
		protected override void OnRemoveComplete(int index, Appointment value) {
			base.OnRemoveComplete(index, value);
			SortItems();
		}
		public override AppointmentBaseCollection GetAppointments(TimeInterval interval) {
			SortedAppointmentBaseCollection result = new SortedAppointmentBaseCollection();
			result.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
			if (Count == 0)
				return result;
			int startIndex = GetAppointmentRangeStartIndex(interval.Start);
			if (startIndex < 0)
				return result;
			int endIndex = GetAppointmentRangeEndIndex(interval.End, startIndex);
			if (startIndex >= 0 && endIndex >= 0) {
				SelectAppointmentRange(result, interval, startIndex, endIndex);
			}
			return result;
		}
		protected internal virtual int GetAppointmentRangeStartIndex(DateTime date) {
			XtraSchedulerDebug.Assert(Count > 0);
			IComparable<Appointment> predicate = new AppointmentStartComparer(date);
			int index = -1;
			int lastFounded = -1;
			int endIndex = Count - 1;
			do {
				index = FindAppointmentRangeStartIndex(predicate, 0, endIndex);
				if (index == lastFounded)
					break;
				lastFounded = index;
				endIndex = Math.Max(0, index - 1);
			} while (index > 0);
			return lastFounded;
		}
		protected int FindAppointmentRangeStartIndex(IComparable<Appointment> predicate, int start, int end) {
			int index = DevExpress.Utils.Algorithms.BinarySearch<Appointment>(InnerList, predicate, start, end);
			if (index < 0) {
				return (index == ~Count) ? -1 : ~index;
			}
			return index;
		}
		protected internal virtual int GetAppointmentRangeEndIndex(DateTime date, int startIndex) {
			XtraSchedulerDebug.Assert(Count > 0);
			IComparable<Appointment> predicate = new AppointmentStartComparer(date);
			int index = -1;
			int lastFounded = -1;
			int lastIndex = Count - 1;
			do {
				index = FindAppointmentRangeEndIndex(predicate, startIndex, lastIndex);
				if (index == ~0) { 
					lastFounded = index;
					break;
				}
				if (index < 0) {
					index = Math.Max(0, ~index - 1);
				}
				if (index == lastFounded)
					break;
				lastFounded = index;
				startIndex = Math.Max(0, index + 1);
			} while (index < lastIndex);
			return lastFounded;
		}
		protected int FindAppointmentRangeEndIndex(IComparable<Appointment> predicate, int start, int end) {
			int index = DevExpress.Utils.Algorithms.BinarySearch<Appointment>(InnerList, predicate, start, end);
			if (index < 0) {
				return (index == ~Count) ? Count - 1 : index;
			}
			return index;
		}
		protected internal virtual void SelectAppointmentRange(SortedAppointmentBaseCollection targetCollection, TimeInterval interval, int startIndex, int endIndex) {
			List<Appointment> selected = new List<Appointment>();
			for (int i = startIndex; i <= endIndex; i++) {
				Appointment apt = this[i];
				if (AppointmentIntersectionHelper.IntersectsWithInterval(apt, interval))
					selected.Add(apt);
			}
			if (selected.Count > 0)
				targetCollection.AddSortedRange(selected);
		}
	}
	#endregion
	#region Exceptions
	public sealed class Exceptions {
		Exceptions() {
		}
		public static void ThrowArgumentException(string propName, object val) {
			string valueStr = (val != null) ? val.ToString() : "null";
			string s = String.Format(SchedulerLocalizer.GetString(SchedulerStringId.Msg_IsNotValid), valueStr, propName);
			throw new ArgumentException(s);
		}
		public static void ThrowArgumentOutOfRangeException(string propName, string message) {
			throw new ArgumentOutOfRangeException(propName, message);
		}
		public static void ThrowArgumentNullException(string propName) {
			throw new ArgumentNullException(propName);
		}
		public static void ThrowInternalException() {
			throw new Exception(SchedulerLocalizer.GetString(SchedulerStringId.Msg_InternalError));
		}
		public static void ThrowObjectDisposedException(string objectName, string propName) {
			string s = String.Format("Cannot access the '{0}' property of a disposed object", propName);
			throw new ObjectDisposedException(objectName, s);
		}
	}
	#endregion
	#region DateTimeCollection
	public class DateCollection : DXCollection<DateTime> {
		public DateCollection() {
		}
		protected internal DateCollection(DXCollectionUniquenessProviderType uniquenessProviderType)
			: base(uniquenessProviderType) {
		}
	}
	#endregion
	#region DateTimeHelper
	public sealed class DateTimeHelper {
		public static readonly TimeSpan FiveMinutesSpan = new TimeSpan(0, 5, 0);
		public static readonly TimeSpan SixMinutesSpan = new TimeSpan(0, 6, 0);
		public static readonly TimeSpan TenMinutesSpan = new TimeSpan(0, 10, 0);
		public static readonly TimeSpan FifteenMinutesSpan = new TimeSpan(0, 15, 0);
		public static readonly TimeSpan TwentyMinutesSpan = new TimeSpan(0, 20, 0);
		public static readonly TimeSpan HalfHourSpan = new TimeSpan(0, 30, 0);
		public static readonly TimeSpan HourSpan = new TimeSpan(1, 0, 0);
		public static readonly TimeSpan DaySpan = new TimeSpan(1, 0, 0, 0);
		public static readonly TimeSpan WeekSpan = new TimeSpan(7, 0, 0, 0);
		public static readonly TimeSpan ZeroSpan = TimeSpan.Zero;
		public const int MinutesPerHour = 60;
		static Dictionary<DayOfWeek, WeekDays> dayOfWeekHT = CreateDayOfWeekTable();
		public static DayOfWeek FirstDayOfWeek {
			get {
				DateTimeFormatInfo dtfi = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;
				return dtfi.FirstDayOfWeek;
			}
		}
		DateTimeHelper() {
		}
		static Dictionary<DayOfWeek, WeekDays> CreateDayOfWeekTable() {
			Dictionary<DayOfWeek, WeekDays> dayOfWeekHT = new Dictionary<DayOfWeek, WeekDays>();
			dayOfWeekHT.Add(DayOfWeek.Saturday, WeekDays.Saturday);
			dayOfWeekHT.Add(DayOfWeek.Friday, WeekDays.Friday);
			dayOfWeekHT.Add(DayOfWeek.Thursday, WeekDays.Thursday);
			dayOfWeekHT.Add(DayOfWeek.Wednesday, WeekDays.Wednesday);
			dayOfWeekHT.Add(DayOfWeek.Tuesday, WeekDays.Tuesday);
			dayOfWeekHT.Add(DayOfWeek.Monday, WeekDays.Monday);
			dayOfWeekHT.Add(DayOfWeek.Sunday, WeekDays.Sunday);
			return dayOfWeekHT;
		}
		public static bool IsWholeDaysNumber(TimeSpan duration) {
			return duration.Ticks % TimeSpan.TicksPerDay == 0;
		}
		public static List<DateTime> CreateRoundedDateList(DateTime start, DateTime end) {
			DateTime roundedStart = start.Date;
			int count = CalculateWholeDayDuration(start, end).Days;
			List<DateTime> result = new List<DateTime>();
			for (int i = 0; i < count; i++) {
				result.Add(roundedStart.AddDays(i));
			}
			return result;
		}
		static TimeSpan CalculateWholeDayDuration(DateTime start, DateTime end) {
			TimeSpan span = end.Date - start.Date;
			return span.Days > 1 ? span : DateTimeHelper.DaySpan;
		}
		public static WeekDays ToWeekDays(DayOfWeek val) {
			WeekDays result;
			if (dayOfWeekHT.TryGetValue(val, out result))
				return result;
			else
				return WeekDays.EveryDay;
		}
		public static DayOfWeek ToDayOfWeek(WeekDays val) {
			foreach (DayOfWeek key in dayOfWeekHT.Keys) {
				WeekDays day = dayOfWeekHT[key];
				if ((day & val) != 0)
					return key;
			}
			Exceptions.ThrowInternalException();
			return DayOfWeek.Sunday;
		}
		public static DayOfWeek[] ToDayOfWeeks(WeekDays val) {
			List<DayOfWeek> result = new List<DayOfWeek>();
			foreach (DayOfWeek key in dayOfWeekHT.Keys) {
				WeekDays day = dayOfWeekHT[key];
				if ((day & val) != 0)
					result.Add(key);
			}
			return result.ToArray();
		}
		public static DateTime ToDateTime(TimeSpan span) {
			return new DateTime(span.Ticks);
		}
		public static TimeSpan ToTimeSpan(DateTime date) {
			return date - DateTime.MinValue;
		}
		internal static bool IsPositiveSpanShorterThanADay(TimeSpan span) {
			return span.Ticks > 0 && span.Ticks < DateTimeHelper.DaySpan.Ticks;
		}
		public static DateTime FloorTime(DateTime date, TimeSpan span) {
			if (!IsPositiveSpanShorterThanADay(span))
				Exceptions.ThrowArgumentException("span", span);
			TimeSpan timeOfDay = date.TimeOfDay;
			date = date.Date;
			long remainder = timeOfDay.Ticks % span.Ticks;
			if (remainder != 0)
				timeOfDay += TimeSpan.FromTicks(-remainder);
			date = date.Add(timeOfDay);
			return date;
		}
		public static DateTime CeilTime(DateTime date, TimeSpan span) {
			if (!IsPositiveSpanShorterThanADay(span))
				Exceptions.ThrowArgumentException("span", span);
			TimeSpan timeOfDay = date.TimeOfDay;
			date = date.Date;
			long remainder = timeOfDay.Ticks % span.Ticks;
			if (remainder != 0)
				timeOfDay += TimeSpan.FromTicks(span.Ticks - remainder);
			date = date.Add(timeOfDay);
			return date;
		}	   
		public static DateTime CeilTimeKeepDay(DateTime date, TimeSpan span) {
			DateTime result = CeilTime(date, span);
			if ((result - date.Date).Ticks > DateTimeHelper.DaySpan.Ticks)
				return date.Date + DateTimeHelper.DaySpan;
			return result;
		}
		internal static long Ceil(long date, long span) {
			if (span <= 0)
				Exceptions.ThrowArgumentException("span", span);
			long remainder = date % span;
			if (remainder != 0) {
				long delta = span - remainder;
				if (DateTime.MaxValue.Ticks - date > delta)
					date += delta;
				else
					date -= remainder;
			}
			XtraSchedulerDebug.Assert(date % span == 0);
			return date;
		}
		internal static long Ceil(long date, long span, long baseDate) {
			if (span <= 0)
				Exceptions.ThrowArgumentException("span", span);
			if (baseDate > date)
				baseDate = 0;
			long actualDate = date - baseDate;
			long remainder = actualDate % span;
			if (remainder != 0) {
				long delta = span - remainder;
				if (DateTime.MaxValue.Ticks - date > delta)
					actualDate += delta;
				else
					actualDate -= remainder;
			}
			XtraSchedulerDebug.Assert(actualDate % span == 0);
			return actualDate + baseDate;
		}
		public static TimeSpan Ceil(TimeSpan date, TimeSpan span) {
			return TimeSpan.FromTicks(Ceil(date.Ticks, span.Ticks));
		}
		public static TimeSpan Ceil(TimeSpan date, TimeSpan span, TimeSpan baseDate) {
			return TimeSpan.FromTicks(Ceil(date.Ticks - baseDate.Ticks, span.Ticks) + baseDate.Ticks);
		}
		public static DateTime Ceil(DateTime date, TimeSpan span, TimeSpan baseTimeOfDay) {
			return new DateTime(Ceil(date.Ticks, span.Ticks, (date.Date + baseTimeOfDay).Ticks));
		}
		public static DateTime Ceil(DateTime date, TimeSpan span) {
			return new DateTime(Ceil(date.Ticks, span.Ticks));
		}
		public static DateTime TrimSeconds(DateTime date) {
			return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
		}
		internal static long Floor(long date, long span) {
			return Floor(date, span, 0);
		}
		internal static long Floor(long date, long span, long baseDate) {
			if (span <= 0)
				Exceptions.ThrowArgumentException("span", span);
			if (baseDate > date)
				baseDate = 0;
			long actualDate = date - baseDate;
			long remainder = actualDate % span;
			if (remainder != 0)
				actualDate -= remainder;
			XtraSchedulerDebug.Assert(actualDate % span == 0);
			return baseDate + actualDate;
		}
		public static TimeSpan Floor(TimeSpan date, TimeSpan span) {
			return TimeSpan.FromTicks(Floor(date.Ticks, span.Ticks));
		}
		public static DateTime Floor(DateTime date, TimeSpan span) {
			return new DateTime(Floor(date.Ticks, span.Ticks));
		}
		public static DateTime Floor(DateTime date, TimeSpan span, DateTime baseDate) {
			return new DateTime(Floor(date.Ticks, span.Ticks, baseDate.Ticks));
		}
		public static DateTime GetStartOfWeek(DateTime date) {
			return GetStartOfWeekUI(date, FirstDayOfWeek, false);
		}
		public static DateTime GetStartOfWeekUI(DateTime date, DayOfWeek firstDayOfWeek) {
			return GetStartOfWeekUI(date, firstDayOfWeek, false);
		}
		public static DateTime GetStartOfWeekUI(DateTime start, DayOfWeek firstDayOfWeek, bool compressed) {
			int offset = CalcStartOfWeekUIDayOffset(start.DayOfWeek, firstDayOfWeek, compressed);
			TimeSpan timeOffset = TimeSpan.FromDays(offset);
			if (start.Ticks < timeOffset.Ticks)
				timeOffset = TimeSpan.FromDays(offset - 7 - 1);
			return start.Date - timeOffset;
		}
		internal static int CalcStartOfWeekUIDayOffset(DayOfWeek dayOfWeek, DayOfWeek firstDayOfWeek, bool compressed) {
			if (compressed && firstDayOfWeek == DayOfWeek.Sunday)
				firstDayOfWeek = DayOfWeek.Monday;
			return (dayOfWeek + 7 - firstDayOfWeek) % 7;
		}
		internal static DateTime AddTimeSpanWithoutOverfull(DateTime dateTime, TimeSpan span) {
			if (DateTime.MaxValue - dateTime < span)
				return DateTime.MaxValue;
			return dateTime + span;
		}
		internal static DateTime SubTimeSpanWithoutOverfull(DateTime dateTime, TimeSpan span) {
			if (span.Ticks > dateTime.Ticks)
				dateTime = DateTime.MinValue;
			else
				dateTime = dateTime - span;
			return dateTime;
		}
		public static int FindDayOfWeekNumber(DateTime[] dates, DayOfWeek dayOfWeek) {
			if (dates == null)
				return -1;
			for (int i = 1; i <= dates.Length; i++)
				if (dates[i - 1].DayOfWeek == dayOfWeek)
					return i;
			return -1;
		}
		public static int FindDayOfWeekIndex(DayOfWeek[] days, DayOfWeek dayOfWeek) {
			if (days == null)
				return -1;
			for (int i = 0; i < days.Length; i++)
				if (days[i] == dayOfWeek)
					return i;
			return -1;
		}
		public static int FindDayOfWeekIndex(DateTime[] weekDates, DayOfWeek day) {
			int count = weekDates.Length;
			for (int i = 0; i < count; i++)
				if (weekDates[i].DayOfWeek == day)
					return i;
			return -1;
		}
		public static DateTime[] GetWeekDates(DateTime start, DayOfWeek firstDayOfWeek, bool compressed, bool showWeekEnd) {
			DateTime[] dates = GetWeekDates(start, firstDayOfWeek, compressed);
			if (!showWeekEnd)
				dates = FilterWeekends(dates);
			return dates;
		}
		public static DateTime[] GetWeekDates(DateTime start, DayOfWeek firstDayOfWeek, bool compressed) {
			DateTime[] result = new DateTime[7];
			DateTime date = DateTimeHelper.GetStartOfWeekUI(start, firstDayOfWeek, compressed);
			for (int i = 0; i < 7; i++) {
				result[i] = date;
				date = date.AddDays(1);
			}
			return result;
		}
		public static DayOfWeek[] GetWeekDays(DayOfWeek firstDayOfWeek) {
			DayOfWeek[] result = new DayOfWeek[7];
			DateTime date = GetStartOfWeekUI(DateTime.MinValue.AddDays(7), firstDayOfWeek, false);
			for (int i = 0; i < 7; i++) {
				result[i] = date.DayOfWeek;
				date = date.AddDays(1);
			}
			return result;
		}
		internal static DayOfWeek[] GetWeekDays(DayOfWeek firstDayOfWeek, WeekDays validDaysMask) {
			List<DayOfWeek> result = new List<DayOfWeek>();
			DateTime date = GetStartOfWeekUI(DateTime.MinValue.AddDays(7), firstDayOfWeek, false);
			for (int i = 0; i < 7; i++) {
				DayOfWeek current = date.DayOfWeek;
				if ((validDaysMask & ToWeekDays(current)) != 0)
					result.Add(current);
				date = date.AddDays(1);
			}
			return result.ToArray();
		}
		public static DateTime[] FilterWeekends(DateTime[] dates) {
			int count = dates.Length;
			List<DateTime> result = new List<DateTime>(count);
			for (int i = 0; i < count; i++) {
				DateTime date = dates[i];
				if (date.DayOfWeek != DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday)
					result.Add(date);
			}
			return result.ToArray();
		}
		internal static DateTime[] GetDates(TimeInterval interval) {
			List<DateTime> result = new List<DateTime>();
			DateTime date = interval.Start.Date;
			while (date < interval.End) {
				result.Add(date);
				date = date.AddDays(1);
			}
			return result.ToArray();
		}
		public static DayOfWeek ConvertFirstDayOfWeek(FirstDayOfWeek dayOfWeek) {
			return dayOfWeek == DevExpress.XtraScheduler.FirstDayOfWeek.System ? DateTimeHelper.FirstDayOfWeek : (DayOfWeek)dayOfWeek;
		}
		public static int CalcWeekCount(TimeSpan interval) {
			return (int)(interval.Ticks / TimeSpan.FromDays(7).Ticks);
		}
		public static int Divide(TimeSpan interval, TimeSpan time) {
			return (int)(interval.Ticks / time.Ticks);
		}
		public static TimeSpan Min(TimeSpan span1, TimeSpan span2) {
			return span1 < span2 ? span1 : span2;
		}
		public static TimeSpan Max(TimeSpan span1, TimeSpan span2) {
			return span1 > span2 ? span1 : span2;
		}
		public static DateTime Min(DateTime span1, DateTime span2) {
			return span1 < span2 ? span1 : span2;
		}
		public static DateTime Max(DateTime span1, DateTime span2) {
			return span1 > span2 ? span1 : span2;
		}
		internal static DateTime GetNextBeginOfMonth(DateTime date) {
			DateTime result = new DateTime(date.Year, date.Month, 1);
			if (result == date)
				result = result.AddMonths(-1);
			return result;
		}
		internal static DateTime GetNextEndOfMonth(DateTime date) {
			DateTime result = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
			if (result == date) {
				if (date.Month == 12)
					result = new DateTime(date.Year + 1, 1, DateTime.DaysInMonth(date.Year + 1, 1));
				else
					result = new DateTime(date.Year, date.Month + 1, DateTime.DaysInMonth(date.Year, date.Month + 1));
			}
			return result;
		}
		public static bool IsBeginOfHour(DateTime time) {
			return (time.Ticks % DateTimeHelper.HourSpan.Ticks) == 0;
		}
		public static TimeInterval ExpandTimeIntervalToFullDays(TimeInterval interval) {
			DateTime start = interval.Start.Date;
			DateTime end = interval.End.Date;
			if (interval.End.TimeOfDay != TimeSpan.Zero)
				end += DateTimeHelper.DaySpan;
			return new TimeInterval(start, end);
		}
		public static bool IsIntervalWholeDays(TimeInterval interval) {
			return interval.Start.TimeOfDay == TimeSpan.Zero &&
				interval.End.TimeOfDay == TimeSpan.Zero &&
				interval.Duration.Ticks > 0;
		}
		public static TimeInterval ToTimeInterval(DateTime date) {
			return new TimeInterval(date.Date, DateTimeHelper.DaySpan);
		}
		public static bool IsExactlyOneDay(TimeInterval interval) {
			return (interval.Start.Ticks % DateTimeHelper.DaySpan.Ticks == 0) && (interval.Duration == DateTimeHelper.DaySpan);
		}
		public static DayOfWeek[] GetDaysOfWeek(DateTime start, DayOfWeek firstDayOfWeek, bool showWeekend, bool compressWeekend) {
			DateTime[] dates = GetWeekDates(start, firstDayOfWeek, compressWeekend);
			return GetDaysOfWeek(dates, showWeekend, compressWeekend);
		}
		public static DayOfWeek[] GetDaysOfWeek(DateTime[] dates, bool showWeekend, bool compressWeekend) {
			if (showWeekend) {
				if (compressWeekend)
					return GetDaysOfWeekCompressedWeekend(dates);
				else
					return GetDaysOfWeek(dates);
			} else
				return GetDaysOfWeekWithoutWeekend(dates);
		}
		public static DayOfWeek[] GetDaysOfWeek(DateTime[] dates) {
			int count = dates.Length;
			DayOfWeek[] result = new DayOfWeek[count];
			for (int i = 0; i < count; i++)
				result[i] = dates[i].DayOfWeek;
			return result;
		}
		public static DayOfWeek[] GetDaysOfWeekCompressedWeekend(DateTime[] dates) {
			int count = dates.Length;
			List<DayOfWeek> result = new List<DayOfWeek>();
			for (int i = 0; i < count; i++) {
				DayOfWeek dayOfWeek = dates[i].DayOfWeek;
				if (dayOfWeek != DayOfWeek.Sunday)
					result.Add(dayOfWeek);
			}
			return result.ToArray();
		}
		public static DayOfWeek[] GetDaysOfWeekWithoutWeekend(DateTime[] dates) {
			int count = dates.Length;
			List<DayOfWeek> result = new List<DayOfWeek>();
			for (int i = 0; i < count; i++) {
				DayOfWeek dayOfWeek = dates[i].DayOfWeek;
				if (dayOfWeek != DayOfWeek.Sunday && dayOfWeek != DayOfWeek.Saturday)
					result.Add(dayOfWeek);
			}
			return result.ToArray();
		}
		public static int CalcQuarterNumber(int month) {
			return (month - 1) / 3 + 1;
		}
		public static int CalcFirstMonthOfQuarter(int quarter) {
			return (quarter - 1) * 3 + 1;
		}
		public static int GetWeekOfYear(DateTime dateTime) {
			return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFourDayWeek, FirstDayOfWeek);
		}
	}
	#endregion
	public struct StringOptimalMeasurement {
		string text;
		bool abbreviated;
		public string Text { get { return text; } set { text = value; } }
		public bool Abbreviated { get { return abbreviated; } set { abbreviated = value; } }
	}
	#region DateTimeFormatHelper
	public static class DateTimeFormatHelper {
		static string formatTerminators = "ydfghHmMstz";
		static string separators = ".,:/-";
		public static DateTimeFormatInfo CurrentUIDateTimeFormat {
			get {
				CultureInfo culture = CultureInfo.CurrentUICulture;
				if (culture.IsNeutralCulture)
					culture = CultureInfo.CurrentCulture;
				return culture.DateTimeFormat;
			}
		}
		public static DateTimeFormatInfo CurrentDateTimeFormat { get { return CultureInfo.CurrentCulture.DateTimeFormat; } }
		public static StringCollection GenerateFormatsWithoutYear() {
			return GenerateFormatsWithoutYear(CurrentDateTimeFormat);
		}
		public static StringCollection GenerateFormatsWithoutYearAndWeekDay() {
			return GenerateFormatsWithoutYearAndWeekDay(CurrentDateTimeFormat);
		}
		public static StringCollection GenerateFormatsWithoutYear(DateTimeFormatInfo dtfi) {
			StringCollection formats = GetFormatsFromLocalizationString(SchedulerStringId.DateTimeAutoFormat_WithoutYear);
			if (formats.Count != 0)
				return formats;
			string longPattern = StripYear(dtfi.LongDatePattern);
			if (!longPattern.Contains("ddd"))
				longPattern = String.Format("dddd, {0}", longPattern);
			AddFormatPattern(formats, longPattern);
			string pattern = longPattern.Replace("MMMM", "MMM");
			AddFormatPattern(formats, pattern);
			pattern = longPattern.Replace("dddd", "ddd");
			AddFormatPattern(formats, pattern);
			pattern = pattern.Replace("MMMM", "MMM");
			AddFormatPattern(formats, pattern);
			pattern = StripYear(dtfi.ShortDatePattern);
			AddFormatPattern(formats, pattern);
			pattern = StripMonth(pattern);
			AddFormatPattern(formats, pattern);
			return formats;
		}
		public static StringCollection GetFormatsFromLocalizationString(SchedulerStringId schedulerStringId) {
			string localizationValue = SchedulerLocalizer.GetString(schedulerStringId);
			StringCollection result = new StringCollection();
			if (String.IsNullOrEmpty(localizationValue))
				return result;
			string[] formats = localizationValue.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			int count = formats.Length;
			for (int i = 0; i < count; i++) {
				string format = formats[i].Trim();
				if (String.IsNullOrEmpty(format))
					continue;
				result.Add(format);
			}
			return result;
		}
		public static StringCollection GenerateWeekAutoFormats() {
			return GenerateWeekAutoFormats(CurrentDateTimeFormat);
		}
		public static StringCollection GenerateWeekAutoFormats(DateTimeFormatInfo dtfi) {
			StringCollection formats = GetFormatsFromLocalizationString(SchedulerStringId.DateTimeAutoFormat_Week);
			if (formats.Count != 0)
				return formats;
			string longPattern = dtfi.LongDatePattern;
			if (!longPattern.Contains("ddd"))
				longPattern = String.Format("dddd, {0}", longPattern);
			AddFormatPattern(formats, longPattern);
			longPattern = StripYear(longPattern);
			AddFormatPattern(formats, longPattern);
			string pattern = longPattern.Replace("MMMM", "MMM");
			AddFormatPattern(formats, pattern);
			pattern = longPattern.Replace("dddd", "ddd");
			AddFormatPattern(formats, pattern);
			pattern = pattern.Replace("MMMM", "MMM");
			AddFormatPattern(formats, pattern);
			pattern = pattern.Replace("MMM", "MM");
			AddFormatPattern(formats, pattern);
			pattern = StripYear(dtfi.ShortDatePattern);
			AddFormatPattern(formats, pattern);
			pattern = StripMonth(pattern);
			AddFormatPattern(formats, pattern);
			return formats;
		}
		public static StringCollection GenerateFormatsWithoutYearAndWeekDay(DateTimeFormatInfo dtfi) {
			StringCollection formats = new StringCollection();
			string pattern = StripYear(dtfi.LongDatePattern);
			pattern = StripDayOfWeek(pattern);
			AddFormatPattern(formats, pattern);
			pattern = pattern.Replace("MMMM", "MMM");
			AddFormatPattern(formats, pattern);
			pattern = StripYear(dtfi.ShortDatePattern);
			pattern = StripDayOfWeek(pattern);
			AddFormatPattern(formats, pattern);
			return formats;
		}
		public static StringCollection GenerateFormatsTimeOnly() {
			return GenerateFormatsTimeOnly(CurrentDateTimeFormat);
		}
		public static StringCollection GenerateFormatsTimeOnly(DateTimeFormatInfo dtfi) {
			StringCollection formats = new StringCollection();
			string pattern = dtfi.LongTimePattern;
			pattern = StripSeconds(pattern);
			AddFormatPattern(formats, pattern);
			return formats;
		}
		public static StringCollection GenerateFormatsTimeWithMonthDay() {
			return GenerateFormatsTimeWithMonthDay(CurrentDateTimeFormat);
		}
		public static StringCollection GenerateFormatsTimeWithMonthDay(DateTimeFormatInfo dtfi) {
			StringCollection formats = new StringCollection();
			string pattern = StripYear(dtfi.FullDateTimePattern);
			pattern = StripDayOfWeek(pattern);
			pattern = StripSeconds(pattern);
			AddFormatPattern(formats, pattern);
			return formats;
		}
		public static StringCollection GenerateFormatsDateTimeWithYear() {
			return GenerateFormatsDateTimeWithYear(CurrentDateTimeFormat);
		}
		public static StringCollection GenerateFormatsDateTimeWithYear(DateTimeFormatInfo dtfi) {
			StringCollection formats = new StringCollection();
			string pattern = StripDayOfWeek(dtfi.FullDateTimePattern);
			pattern = StripSeconds(pattern);
			AddFormatPattern(formats, pattern);
			return formats;
		}
		public static StringCollection GenerateFormatsDateWithYear() {
			return GenerateFormatsDateWithYear(CurrentDateTimeFormat);
		}
		public static StringCollection GenerateFormatsDateWithYear(DateTimeFormatInfo dtfi) {
			StringCollection formats = new StringCollection();
			string pattern = StripDayOfWeek(dtfi.LongDatePattern);
			AddFormatPattern(formats, pattern);
			return formats;
		}
		static StringCollection GenerateFormatsWithYearCore(string pattern) {
			StringCollection formats = new StringCollection();
			pattern = StripDayOfWeek(pattern);
			AddFormatPattern(formats, pattern);
			pattern = pattern.Replace("MMMM", "MMM");
			AddFormatPattern(formats, pattern);
			return formats;
		}
		public static StringCollection GenerateFormatsWithYearForJanuary() {
			return GenerateFormatsWithYearForJanuary(CurrentDateTimeFormat);
		}
		public static StringCollection GenerateFormatsWithYearForJanuary(DateTimeFormatInfo dtfi) {
			string pattern = dtfi.LongDatePattern;
			StringCollection formats = GenerateFormatsWithYearCore(pattern.Replace("yyyy", "yy"));
			AddFormatPattern(formats, StripDayOfWeek(dtfi.LongDatePattern));
			return formats;
		}
		static void AddFormatPattern(StringCollection formats, string pattern) {
			if (!formats.Contains(pattern))
				formats.Add(pattern);
		}
#if !SL
		public static string DateToStringOptimal(Graphics gr, Font font, DateTime date, int boundsWidth, StringCollection formats) {
			StringOptimalMeasurement measurement = DateToStringOptimalMeasurement(gr, font, date, boundsWidth, formats);
			return measurement.Text;
		}
		public static string FindOptimalDateTimeFormat(ITimeScaleDateTimeFormatter formatter, DateTime[] dates, int boundsWidth, Graphics gr, Font font) {
			if (formatter == null || !formatter.SupportsAutoFormats)
				return string.Empty;
			StringCollection formats = formatter.GetDateTimeAutoFormats();
			if (formats == null || formats.Count == 0)
				return string.Empty;
			int count = formats.Count;
			string s = String.Empty;
			int adjustedWidth = int.MinValue;
			int minWidth = int.MaxValue;
			string minFormat = String.Empty;
			string adjustedFormat = String.Empty;
			for (int n = 0; n < dates.Length; n++) {
				DateTime date = dates[n];
				for (int i = 0; i < count; i++) {
					string currentFormat = formats[i];
					s = formatter.Format(currentFormat, date, date);
					int width = (int)gr.MeasureString(s + "Wg", font).Width;
					if (width < minWidth) {
						minWidth = width;
						minFormat = currentFormat;
					}
					if (width <= boundsWidth) {
						if (width > adjustedWidth) {
							adjustedWidth = width;
							adjustedFormat = currentFormat;
						}
					}
				}
			}
			if (adjustedFormat.Length == 0)
				adjustedFormat = minFormat;
			return adjustedFormat;
		}
		public static StringOptimalMeasurement DateToStringOptimalMeasurement(Graphics gr, Font font, DateTime date, int boundsWidth, StringCollection formats) {
			int count = formats.Count;
			string s = String.Empty;
			int adjustedWidth = int.MinValue;
			int minWidth = int.MaxValue;
			string minFormat = String.Empty;
			string adjustedFormat = String.Empty;
			bool wasOptimized = false;
			for (int i = 0; i < count; i++) {
				s = SysDate.ToString(formats[i], date);
				int width = (int)gr.MeasureString(s, font).Width;
				if (width < minWidth) {
					minWidth = width;
					minFormat = s;
				}
				if (width <= boundsWidth) {
					if (width > adjustedWidth) {
						adjustedWidth = width;
						adjustedFormat = s;
					}
				} else
					wasOptimized = true;
			}
			if (adjustedFormat.Length == 0)
				adjustedFormat = minFormat;
			StringOptimalMeasurement result = new StringOptimalMeasurement();
			result.Text = adjustedFormat;
			result.Abbreviated = wasOptimized;
			return result;
		}
		public static string DateToStringWithoutYearOptimal(Graphics gr, Font font, DateTime date, int boundsWidth) {
			StringCollection formats = GenerateFormatsWithoutYear();
			return DateToStringOptimal(gr, font, date, boundsWidth, formats);
		}
		public static string DateToStringWithoutYearAndWeekDayOptimal(Graphics gr, Font font, DateTime date, int boundsWidth) {
			StringCollection formats = GenerateFormatsWithoutYearAndWeekDay();
			return DateToStringOptimal(gr, font, date, boundsWidth, formats);
		}
		public static string DateToStringForNewYearOptimal(Graphics gr, Font font, DateTime date, int boundsWidth) {
			StringCollection formats = GenerateFormatsWithYearForJanuary();
			return DateToStringOptimal(gr, font, date, boundsWidth, formats);
		}
#endif
		public static string DateToStringWithoutYearAndWeekDay(DateTime date) {
			StringCollection formats = GenerateFormatsWithoutYearAndWeekDay();
			if (formats.Count > 0)
				return SysDate.ToString(formats[0], date);
			else
				return date.ToString();
		}
		public static string DateToStringForNewYear(DateTime date) {
			StringCollection formats = GenerateFormatsWithYearForJanuary();
			if (formats.Count > 0)
				return SysDate.ToString(formats[0], date);
			else
				return date.ToString();
		}
		public static string DateToLongDateString(DateTime date) {
			return SysDate.ToString(DateTimeFormatHelper.CurrentDateTimeFormat.LongDatePattern, date);
		}
		static string prevLongTimePattern;
		static string correctShortTimePattern;
		internal static string CorrectShortTimePattern {
			get {
				string pattern = DateTimeFormatHelper.CurrentDateTimeFormat.LongTimePattern;
				if (pattern != prevLongTimePattern) {
					prevLongTimePattern = pattern;
					correctShortTimePattern = StripSeconds(pattern);
				}
				return correctShortTimePattern;
			}
		}
		public static string DateToShortTimeString(DateTime date) {
			return date.ToString(CorrectShortTimePattern);
		}
		static int FindBeginOfSequence(string str, int from, string sequenceChars, bool forward) {
			bool inStringLiteral = false;
			int count = str.Length;
			int step = forward ? 1 : -1;
			for (int i = from; i >= 0 && i < count; i += step) {
				char ch = str[i];
				if (ch == '\'')
					inStringLiteral = !inStringLiteral;
				else if (!inStringLiteral && sequenceChars.IndexOf(ch) >= 0)
					return i;
			}
			return -1;
		}
		static int FindEndOfSequence(string str, int from, char sequenceChar) {
			int count = str.Length;
			int i = from;
			while (i < count && str[i] == sequenceChar)
				i++;
			return i;
		}
		internal static string RemoveCustomFormatting(string pattern) {
			if (pattern == null || pattern.Length == 0)
				return pattern;
			bool inStringLiteral = false;
			bool quoteClosed = false;
			int count = pattern.Length;
			StringBuilder sb = new StringBuilder(count);
			for (int i = 0; i < count; i++) {
				char ch = pattern[i];
				if (ch == '\'') {
					inStringLiteral = !inStringLiteral;
					if (!inStringLiteral)
						quoteClosed = true;
				} else {
					if (quoteClosed && formatTerminators.IndexOf(ch) >= 0 &&
						formatTerminators.IndexOf(sb[sb.Length - 1]) >= 0)
						sb.Append(' ');
					if (!inStringLiteral)
						sb.Append(ch);
					quoteClosed = false;
				}
			}
			return sb.ToString();
		}
		internal static string StripCore(string pattern, char formatChar, int length) {
			if (pattern == null || pattern.Length == 0)
				return pattern;
			string composedTerminators = formatTerminators + separators;
			pattern = pattern.Trim();
			int sequenceStart, sequenceEnd = -1;
			for (;;) {
				sequenceStart = FindBeginOfSequence(pattern, sequenceEnd + 1, new String(formatChar, 1), true);
				if (sequenceStart < 0)
					return pattern;
				sequenceEnd = FindEndOfSequence(pattern, sequenceStart, formatChar);
				if (length == -1 || length == sequenceEnd - sequenceStart)
					break;
			}
			if (sequenceStart == 0) { 
				int yearSuffixEnd = FindBeginOfSequence(pattern, sequenceEnd, formatTerminators, true);
				if (yearSuffixEnd < 0)
					pattern = pattern.Substring(0, sequenceStart);
				else
					pattern = pattern.Substring(0, sequenceStart) + pattern.Substring(yearSuffixEnd);
			} else if (sequenceEnd >= pattern.Length) { 
				int yearPrefixStart = FindBeginOfSequence(pattern, sequenceStart - 1, formatTerminators, false);
				if (yearPrefixStart < 0)
					pattern = pattern.Substring(sequenceStart);
				else
					pattern = pattern.Substring(0, yearPrefixStart + 1) + pattern.Substring(sequenceEnd);
			} else { 
				int yearSuffixEnd = FindBeginOfSequence(pattern, sequenceEnd, composedTerminators, true);
				int yearPrefixStart = FindBeginOfSequence(pattern, sequenceStart - 1, formatTerminators, false);
				string whiteSpace = String.Empty;
				if (yearPrefixStart >= 0 && yearSuffixEnd >= 0) {
					char prevChar = yearPrefixStart > 0 ? pattern[yearPrefixStart - 1] : formatTerminators[0];
					char nextChar = pattern[yearSuffixEnd];
					if (formatTerminators.IndexOf(prevChar) >= 0 && formatTerminators.IndexOf(nextChar) >= 0)
						whiteSpace = " ";
				}
				if (yearSuffixEnd < 0)
					pattern = pattern.Substring(0, sequenceStart);
				else
					pattern = pattern.Substring(0, sequenceStart) + pattern.Substring(yearSuffixEnd);
				if (yearPrefixStart < 0)
					pattern = pattern.Substring(sequenceStart);
				else {
					int patternLength = pattern.Length;
					if (yearPrefixStart < patternLength && sequenceStart < patternLength) {
						char prevChar = pattern[yearPrefixStart];
						char nextChar = pattern[sequenceStart];
						if (formatTerminators.IndexOf(prevChar) >= 0 && formatTerminators.IndexOf(nextChar) >= 0)
							whiteSpace = " ";
					}
					pattern = pattern.Substring(0, yearPrefixStart + 1) + whiteSpace + pattern.Substring(sequenceStart);
				}
			}
			return pattern.Trim();
		}
		public static string Strip(string pattern, char formatChar) {
			return FixCustomFormat(StripCore(pattern, formatChar, -1));
		}
		public static string StripYear(string pattern) {
			try {
				return Strip(pattern, 'y');
			} catch {
				return pattern;
			}
		}
		public static string StripMonth(string pattern) {
			try {
				return Strip(pattern, 'M');
			} catch {
				return pattern;
			}
		}
		public static string StripMinutes(string pattern) {
			try {
				return Strip(pattern, 'm');
			} catch {
				return pattern;
			}
		}
		public static string StripTimeDesignator(string pattern) {
			try {
				return Strip(pattern, 't');
			} catch {
				return pattern;
			}
		}
		public static string StripSeconds(string pattern) {
			try {
				return Strip(pattern, 's');
			} catch {
				return pattern;
			}
		}
		public static string StripDay(string pattern) {
			try {
				pattern = StripCore(pattern, 'd', 2);
				pattern = StripCore(pattern, 'd', 1);
				return FixCustomFormat(pattern);
			} catch {
				return FixCustomFormat(pattern);
			}
		}
		public static string StripDayOfWeek(string pattern) {
			try {
				pattern = StripCore(pattern, 'd', 4);
				pattern = StripCore(pattern, 'd', 3);
				return FixCustomFormat(pattern);
			} catch {
				return FixCustomFormat(pattern);
			}
		}
		public static StringCollection GetWholeHourTimeFormats() {
			return GetWholeHourTimeFormats(CurrentDateTimeFormat);
		}
		public static StringCollection GetWholeHourTimeFormats(DateTimeFormatInfo dtfi) {
			return GetWholeHourTimeFormats(dtfi.LongTimePattern);
		}
		public static bool IsTimeDesignator(string pattern) {
			if (pattern == null || pattern.Length <= 0)
				return false;
			return pattern[0] == 't' || pattern.StartsWith("%t");
		}
		public static StringCollection GetWholeHourTimeFormats(string pattern) {
			pattern = RemoveCustomFormatting(pattern);
			StringCollection result = new StringCollection();
			string hourString = GetHourString(pattern);
			result.Add(hourString);
			string minutesString = GetMinutesString(pattern);
			if (hourString[0] == 'h' || hourString.StartsWith("%h"))
				result.Add(GetTimeDesignatorString(pattern));
			else
				result.Add(minutesString);
			result.Add(minutesString);
			return result;
		}
		public static string FixCustomFormat(string pattern) {
			if (pattern == "h")
				pattern = "%h";
			if (pattern == "H")
				pattern = "%H";
			if (pattern == "t")
				pattern = "%t";
			if (pattern == "m")
				pattern = "%m";
			if (pattern == "M")
				pattern = "%M";
			if (pattern == "d")
				pattern = "%d";
			if (pattern == "y")
				pattern = "%y";
			if (pattern == "s")
				pattern = "%s";
			return pattern;
		}
		internal static string GetHourString(string pattern) {
			int hoursBegin = FindBeginOfSequence(pattern, 0, "h", true);
			int hoursBegin2 = FindBeginOfSequence(pattern, 0, "H", true);
			if (hoursBegin < 0)
				hoursBegin = int.MaxValue;
			if (hoursBegin2 < 0)
				hoursBegin2 = int.MaxValue;
			char sequenceChar;
			if (hoursBegin < hoursBegin2)
				sequenceChar = 'h';
			else {
				sequenceChar = 'H';
				hoursBegin = hoursBegin2;
			}
			string hourString;
			if (hoursBegin == int.MaxValue) {
				hourString = "HH";
			} else {
				int hoursEnd = FindEndOfSequence(pattern, hoursBegin, sequenceChar);
				if (hoursEnd >= pattern.Length)
					hourString = pattern.Substring(hoursBegin);
				else
					hourString = pattern.Substring(hoursBegin, hoursEnd - hoursBegin);
			}
			return FixCustomFormat(hourString);
		}
		internal static string GetMinutesString(string where) {
			string result = FixCustomFormat(GetContinuousString(where, 'm'));
			return result.Length == 0 ? "mm" : result;
		}
		internal static string GetTimeDesignatorString(string where) {
			string result = FixCustomFormat(GetContinuousString(where, 't'));
			return result.Length == 0 ? "tt" : result;
		}
		static string GetContinuousString(string where, char ch) {
			int begin = FindBeginOfSequence(where, 0, new String(ch, 1), true);
			if (begin < 0)
				return String.Empty;
			int end = FindEndOfSequence(where, begin, ch);
			if (end >= where.Length)
				return where.Substring(begin);
			else
				return where.Substring(begin, end - begin);
		}
	}
	#endregion
	#region SysDate
	public static class SysDate {
		public static string ToString(string format, DateTime val) {
			return ToString(format, val, CultureInfo.CurrentCulture);
		}
		public static string ToString(string format, DateTime val, CultureInfo culture) {
			return val.ToString(format, culture);
		}
	}
	#endregion
	#region SYSTEMTIME
	[StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct SYSTEMTIME {
		short wYear;
		short wMonth;
		internal short wDayOfWeek;
		short wDay;
		short wHour;
		short wMinute;
		short wSecond;
		short wMilliseconds;
		internal static SYSTEMTIME CreateEmpty() {
			return new SYSTEMTIME(1);
		}
		SYSTEMTIME(short fake) {
			this.wYear = fake;
			this.wMonth = 1;
			this.wDayOfWeek = 0;
			this.wDay = 1;
			this.wHour = 0;
			this.wMinute = 0;
			this.wSecond = 0;
			this.wMilliseconds = 0;
		}
		public SYSTEMTIME(DateTime date) {
			this.wYear = (short)date.Year;
			this.wMonth = (short)date.Month;
			this.wDayOfWeek = (short)date.DayOfWeek;
			this.wDay = (short)date.Day;
			this.wHour = (short)date.Hour;
			this.wMinute = (short)date.Minute;
			this.wSecond = (short)date.Second;
			this.wMilliseconds = (short)date.Millisecond;
		}
		public DateTime ToDateTime() {
			if (wYear <= 0)
				wYear = 1;
			if (wMonth <= 0)
				wMonth = 1;
			if (wDay <= 0)
				wDay = 1;
			return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);
		}
		public void LoadFromStream(System.IO.BinaryReader sr) {
			wYear = sr.ReadInt16();
			wMonth = sr.ReadInt16();
			wDayOfWeek = sr.ReadInt16();
			wDay = sr.ReadInt16();
			wHour = sr.ReadInt16();
			wMinute = sr.ReadInt16();
			wSecond = sr.ReadInt16();
			wMilliseconds = sr.ReadInt16();
		}
	}
	#endregion
#if SL
	public enum ContentAlignment {
		BottomCenter = 0x200,
		BottomLeft = 0x100,
		BottomRight = 0x400,
		MiddleCenter = 0x20,
		MiddleLeft = 0x10,
		MiddleRight = 0x40,
		TopCenter = 2,
		TopLeft = 1,
		TopRight = 4
	}
#endif
	#region RectUtils
	public sealed class RectUtils {
		RectUtils() {
		}
		public static Rectangle GetTopSideRect(Rectangle r, int size) {
			return new Rectangle(r.X, r.Y, r.Width, size);
		}
		public static Rectangle GetBottomSideRect(Rectangle r, int size) {
			return new Rectangle(r.X, r.Bottom - size, r.Width, size);
		}
		public static Rectangle GetLeftSideRect(Rectangle r, int size) {
			return new Rectangle(r.X, r.Y, size, r.Height);
		}
		public static Rectangle GetRightSideRect(Rectangle r, int size) {
			return new Rectangle(r.Right - size, r.Y, size, r.Height);
		}
		public static Rectangle SetLeft(Rectangle r, int value) {
			if (value > r.Right)
				return Rectangle.Empty;
			return Rectangle.FromLTRB(value, r.Top, r.Right, r.Bottom);
		}
		public static Rectangle SetRight(Rectangle r, int value) {
			if (value < r.Left)
				return Rectangle.Empty;
			return Rectangle.FromLTRB(r.Left, r.Top, value, r.Bottom);
		}
		public static Rectangle GetLeftSideRect(Rectangle r) {
			return GetLeftSideRect(r, 1);
		}
		public static Rectangle GetRightSideRect(Rectangle r) {
			return GetRightSideRect(r, 1);
		}
		public static Rectangle GetTopSideRect(Rectangle r) {
			return GetTopSideRect(r, 1);
		}
		public static Rectangle GetBottomSideRect(Rectangle r) {
			return GetBottomSideRect(r, 1);
		}
		public static Rectangle CutFromTop(Rectangle r, int height) {
			r.Y += height;
			r.Height -= height;
			return r;
		}
		public static Rectangle CutFromBottom(Rectangle r, int height) {
			r.Height -= height;
			return r;
		}
		public static Rectangle CutFromLeft(Rectangle r, int width) {
			r.X += width;
			r.Width -= width;
			return r;
		}
		public static Rectangle CutFromRight(Rectangle r, int width) {
			r.Width -= width;
			return r;
		}
		public static bool ContainsX(Rectangle r, Point pt) {
			return ContainsX(r, pt.X);
		}
		public static bool ContainsX(Rectangle r, int x) {
			return (r.Left <= x) && (x <= r.Right);
		}
		public static bool ContainsY(Rectangle r, Point pt) {
			return ContainsY(r, pt.Y);
		}
		public static bool ContainsY(Rectangle r, int y) {
			return (r.Top <= y) && (y <= r.Bottom);
		}
		public static Rectangle ExpandToLeft(Rectangle source, int value) {
			source.X -= value;
			source.Width += value;
			return source;
		}
		public static Rectangle ExpandToRight(Rectangle source, int value) {
			source.Width += value;
			return source;
		}
		public static Rectangle ExpandToTop(Rectangle source, int value) {
			source.Y -= value;
			source.Height += value;
			return source;
		}
		public static Rectangle ExpandToBottom(Rectangle source, int value) {
			source.Height += value;
			return source;
		}
		public static Rectangle AlignRectangle(Rectangle rect, Rectangle baseRect, ContentAlignment aligment) {
			rect.Location = baseRect.Location;
			switch (aligment) {
				case ContentAlignment.TopCenter:
					rect.X += (baseRect.Width - rect.Width) / 2;
					break;
				case ContentAlignment.TopLeft:
					break;
				case ContentAlignment.TopRight:
					rect.X += baseRect.Width - rect.Width;
					break;
				case ContentAlignment.MiddleLeft:
					rect.Y += (baseRect.Height - rect.Height) / 2;
					break;
				case ContentAlignment.MiddleCenter:
					rect.Offset((baseRect.Width - rect.Width) / 2, (baseRect.Height - rect.Height) / 2);
					break;
				case ContentAlignment.MiddleRight:
					rect.Offset(baseRect.Width - rect.Width, (baseRect.Height - rect.Height) / 2);
					break;
				case ContentAlignment.BottomLeft:
					rect.Y += baseRect.Height - rect.Height;
					break;
				case ContentAlignment.BottomCenter:
					rect.Offset((baseRect.Width - rect.Width) / 2, baseRect.Height - rect.Height);
					break;
				case ContentAlignment.BottomRight:
					rect.Offset(baseRect.Width - rect.Width, baseRect.Height - rect.Height);
					break;
			}
			return rect;
		}
		public static Rectangle[] SplitHorizontally(Rectangle bounds, int cellCount) {
			if (cellCount <= 0)
				return new Rectangle[] { bounds };
			Rectangle[] cells = new Rectangle[cellCount];
			int offset = bounds.X;
			int height = bounds.Height;
			int columnsAreaWidth = bounds.Width;
			int columnWidth = columnsAreaWidth / cellCount;
			int remainder = columnsAreaWidth - columnWidth * cellCount;
			for (int i = 0; i < cellCount; i++, remainder--) {
				int width = columnWidth + ((remainder > 0) ? 1 : 0);
				cells[i] = new Rectangle(offset, bounds.Y, width, height);
				offset += width;
			}
			return cells;
		}
		public static Rectangle[] SplitVertically(Rectangle bounds, int cellCount) {
			if (cellCount <= 0)
				return new Rectangle[] { bounds };
			Rectangle[] cells = new Rectangle[cellCount];
			int offset = bounds.Y;
			int width = bounds.Width;
			int columnsAreaHeight = bounds.Height;
			int columnHeight = columnsAreaHeight / cellCount;
			int remainder = columnsAreaHeight - columnHeight * cellCount;
			for (int i = 0; i < cellCount; i++, remainder--) {
				int height = columnHeight + ((remainder > 0) ? 1 : 0);
				cells[i] = new Rectangle(bounds.X, offset, width, height);
				offset += height;
			}
			return cells;
		}
		public static int CalcDateY(TimeOfDayInterval what, Rectangle whatRect, TimeSpan time) {
			if (!what.Contains(time))
				return -1;
			float offsetRatio = (time - what.Start).Ticks / (float)what.Duration.Ticks;
			int offset = (int)(whatRect.Height * offsetRatio);
			return whatRect.Y + offset;
		}
		public static Rectangle[] TileVertically(Rectangle bounds, int count) {
			Rectangle[] result = new Rectangle[count];
			int height = bounds.Height;
			for (int i = 0; i < count; i++) {
				result[i] = bounds;
				bounds.Y += height;
			}
			return result;
		}
		public static Rectangle UnionNonEmpty(Rectangle first, Rectangle second) {
			if (first == Rectangle.Empty)
				return second;
			if (second == Rectangle.Empty)
				return first;
			return Rectangle.Union(first, second);
		}
		public static bool IntersectsExcludeBounds(Rectangle first, Rectangle second) {
			Rectangle intersection = Rectangle.Intersect(first, second);
			return (intersection.Height * intersection.Width) != 0;
		}
	}
	#endregion
	#region TypeSerializationHelper
	public static class TypeSerializationHelper {
		public static string GetSerializationTypeName(Type type) {
			if (type.Assembly.Equals(Assembly.GetExecutingAssembly()))
				return type.FullName;
			else {
				string typeName = type.AssemblyQualifiedName;
				return typeName;
			}
		}
		public static Type CreateTypeFromSerializationTypeName(string typeName) {
			Type type = Type.GetType(typeName, false);
			return type;
		}
	}
	#endregion
	#region AppointmentResourceIdReadOnlyCollection
	public class AppointmentResourceIdReadOnlyCollection : AppointmentResourceIdCollection {
		static readonly object[] emptyObjectArray = new object[] { };
		readonly AppointmentResourceIdCollection sourceCollection;
		public AppointmentResourceIdReadOnlyCollection(AppointmentResourceIdCollection sourceCollection) {
			Guard.ArgumentNotNull(sourceCollection, "sourceCollection");
			this.sourceCollection = sourceCollection;
		}
		protected override IList<object> InnerList { get { return this.sourceCollection != null ? this.sourceCollection.ObtainInnerList() : null; } }
		internal AppointmentResourceIdCollection SourceCollection { get { return sourceCollection; } }
		public override object[] ToArray() {
			if (sourceCollection != null)
				return sourceCollection.ToArray();
			else
				return emptyObjectArray;
		}
		protected internal override void AddDefaultContent() {
		}
		protected override bool OnClear() {
			ThrowReadOnlyException();
			return false;
		}
		protected override bool OnInsert(int index, object value) {
			ThrowReadOnlyException();
			return false;
		}
		protected override bool OnRemove(int index, object value) {
			ThrowReadOnlyException();
			return false;
		}
		protected override bool OnSet(int index, object oldValue, object newValue) {
			ThrowReadOnlyException();
			return false;
		}
		protected override void RaiseCollectionChanging(CollectionChangingEventArgs<object> e) {
			e.Cancel = true;
		}
		protected override void RaiseCollectionChanged(CollectionChangedEventArgs<object> e) {
		}
		protected internal virtual void ThrowReadOnlyException() {
			throw new Exception("You cannot modify ResourceIds collection for occurrence or exception appointment. You should modify recurrence pattern instead.");
		}
	}
	#endregion
	#region MathUtils
	public static class MathUtils {
		public static int GCD(int a, int b) {
			if (a < 0)
				Exceptions.ThrowArgumentException("a", a);
			if (b < 0)
				Exceptions.ThrowArgumentException("b", b);
			if (b == 0)
				return a;
			if (b == 1)
				return 1;
			else
				return GCD(b, a % b);
		}
		public static int LCM(int a, int b) {
			if (a <= 0)
				Exceptions.ThrowArgumentException("a", a);
			if (b <= 0)
				Exceptions.ThrowArgumentException("b", b);
			return a * b / GCD(a, b);
		}
		public static Size Max(Size val1, Size val2) {
			return new Size(Math.Max(val1.Width, val2.Width), Math.Max(val1.Height, val2.Height));
		}
		public static Size Plus(Size val1, Size val2) {
			return new Size(val1.Width + val2.Width, val1.Height + val2.Height);
		}
	}
	#endregion
	#region SRCategoryNames
	public static class SRCategoryNames {
		public const string View = "View"; 
		public const string Appearance = "Appearance";
		public const string Data = "Data";
		public const string Behavior = "Behavior";
		public const string Options = "Options";
		public const string Scheduler = "Scheduler";
		public const string CustomDraw = "Custom Draw";
		public const string OptionsCustomization = "Options Customization";
		public const string Design = "Design";
		public const string Printing = "Printing";
		public const string Forms = "Forms";
		public const string Layout = "Layout";
	}
	#endregion
	#region SchedulerUtils
	public static class SchedulerUtils {
		static bool IsInteger(object value) {
			return value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint;
		}
		public static bool IsBaseType(AppointmentType type) {
			return type == AppointmentType.Pattern || type == AppointmentType.Normal;
		}
		public static bool IsExceptionType(AppointmentType type) {
			return type == AppointmentType.ChangedOccurrence || type == AppointmentType.DeletedOccurrence;
		}
		public static bool IsOccurrenceType(AppointmentType type) {
			return type == AppointmentType.Occurrence || IsExceptionType(type);
		}
		public static bool IsAppointmentsEquals(Appointment appointment1, Appointment appointment2) {
			if (appointment1.Type == AppointmentType.Occurrence && appointment2.Type == AppointmentType.Occurrence)
				return appointment1.RecurrencePattern == appointment2.RecurrencePattern &&
					appointment1.RecurrenceIndex == appointment2.RecurrenceIndex;
			return appointment1 == appointment2;
		}
		internal static Resource CreateResourceInstance(IResourceStorageBase storage) {
			if (storage == null)
				return new ResourceBase();
			return storage.CreateResource(null);
		}
		internal static AppointmentDependency CreateAppointmentDependencyInstance(IAppointmentDependencyStorage storage) {
			if (storage == null)
				return new AppointmentDependencyInstance();
			return storage.CreateAppointmentDependency(AppointmentDependencyInstance.Empty, AppointmentDependencyInstance.Empty);
		}
		public static Appointment CreateAppointmentInstance(IAppointmentStorageBase storage, AppointmentType type) {
			if (storage == null)
				return new DevExpress.XtraScheduler.Internal.Implementations.AppointmentInstance(type);
			return CreateAppointmentInstance(storage.Storage, type);
		}
		public static Appointment CreateAppointmentInstance(ISchedulerStorageBase storage, AppointmentType type) {
			Appointment result = storage != null ? storage.CreateAppointment(type) : null;
			return result != null ? result : StaticAppointmentFactory.CreateAppointment(type);
		}
		public static string FormatAppointmentFormCaption(bool allDay, string subject, bool readOnly) {
			string format = SchedulerLocalizer.GetString(allDay ? SchedulerStringId.Caption_Event : SchedulerStringId.Caption_Appointment);
			string text = subject;
			if (text == null || text.Length == 0)
				text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_UntitledAppointment);
			text = String.Format(CultureInfo.InvariantCulture, format, text);
			if (readOnly)
				text += SchedulerLocalizer.GetString(SchedulerStringId.Caption_ReadOnly);
			return text;
		}
		public static void UpdateAppointment(Appointment apt) {
			((IInternalAppointment)apt).OnContentChanged();
		}
		public static bool SpecialEquals(object obj1, object obj2) {
			if (obj1 == null || obj2 == null)
				return Object.Equals(obj1, obj2);
			bool checkAsLong = obj1.GetType() == typeof(long) || obj2.GetType() == typeof(long);
			if (checkAsLong)
				return Object.Equals(Convert.ToInt64(obj1), Convert.ToInt64(obj2));
			bool checkAsInt1 = obj1.GetType() == typeof(int) || (obj1.GetType() != typeof(int) && IsInteger(obj1));
			bool checkAsInt2 = obj2.GetType() == typeof(int) || (obj2.GetType() != typeof(int) && IsInteger(obj2));
			if (checkAsInt1 && checkAsInt2)
				return Object.Equals(Convert.ToInt32(obj1), Convert.ToInt32(obj2));
			return Object.Equals(obj1, obj2);
		}
	}
	#endregion
#if !SL
	public static class DataHelper {
		static BindingContext bindingContext = new BindingContext();
		static BindingManagerBase GetBindingManager(object dataSource, string dataMember) {
			BindingManagerBase mgr = null;
			if (dataSource != null) {
				try {
					if (dataMember.Length > 0)
						mgr = bindingContext[dataSource, dataMember];
					else
						mgr = bindingContext[dataSource];
				} catch { }
			}
			return mgr;
		}
		public static BindingManagerBase ValidateBindingContext(object dataSource, string dataMember) {
			if (!CanUseContext(dataSource, dataMember))
				return null;
			BindingManagerBase mgr = null;
			if (dataMember.Length > 0)
				mgr = bindingContext[dataSource, dataMember];
			else
				mgr = bindingContext[dataSource];
			return mgr;
		}
		public static PropertyDescriptorCollection GetItemProperties(object dataSource, string dataMember) {
			BindingManagerBase mgr = GetBindingManager(dataSource, dataMember);
			return (mgr != null) ? mgr.GetItemProperties() : null;
		}
		public static IList GetList(object dataSource, string dataMember) {
			if (dataMember == null)
				dataMember = String.Empty;
			bool useContext = CanUseContext(dataSource, dataMember);
			return DevExpress.Data.Helpers.MasterDetailHelper.GetDataSource((useContext ? bindingContext : null), dataSource, dataMember);
		}
		static bool CanUseContext(object dataSource, string dataMember) {
			if (!DevExpress.Data.Helpers.SecurityHelper.IsPartialTrust)
				return !((dataSource is DataSet) && dataMember.Length == 0);
			return false;
		}
	}
#else
	public static class DataHelper {
		public static IList GetList(object dataSource, string dataMember) {
			return dataSource as IList;
		}
	}
#endif
	#region HumanReadableTimeSpanHelper
	public sealed class HumanReadableTimeSpanHelper {
		static Dictionary<string, int> ht;
		static Decimal[] Halfs = { (Decimal)0.0, (Decimal)0.5 };
		static Decimal[] TenthsAndQuarters = { (Decimal)0.0, (Decimal)0.1, (Decimal)0.2, (Decimal)0.25, (Decimal)0.3, (Decimal)0.4, (Decimal)0.5, (Decimal)0.6, (Decimal)0.75, (Decimal)0.8, (Decimal)0.9 };
		#region Events
		static TimeSpanStringConvertEventHandler onParse;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("HumanReadableTimeSpanHelperParseString")]
#endif
		public static event TimeSpanStringConvertEventHandler ParseString { add { onParse += value; } remove { onParse -= value; } }
		static void RaiseParse(TimeSpanStringConvertEventArgs e) {
			if (onParse != null)
				onParse(null, e);
		}
		static TimeSpanStringConvertEventHandler onToString;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("HumanReadableTimeSpanHelperConvertToString")]
#endif
		public static event TimeSpanStringConvertEventHandler ConvertToString { add { onToString += value; } remove { onToString -= value; } }
		static void RaiseToString(TimeSpanStringConvertEventArgs e) {
			if (onToString != null)
				onToString(null, e);
		}
		#endregion
		HumanReadableTimeSpanHelper() {
		}
		static HumanReadableTimeSpanHelper() {
			Reset();
		}
		public static void Reset() {
			ht = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
			ht[SchedulerLocalizer.GetString(SchedulerStringId.Abbr_MinutesShort1)] = 1;
			ht[SchedulerLocalizer.GetString(SchedulerStringId.Abbr_MinutesShort2)] = 1;
			ht[SchedulerLocalizer.GetString(SchedulerStringId.Abbr_Minute)] = 1;
			ht[SchedulerLocalizer.GetString(SchedulerStringId.Abbr_Minutes)] = 1;
			ht[SchedulerLocalizer.GetString(SchedulerStringId.Abbr_HoursShort)] = 60;
			ht[SchedulerLocalizer.GetString(SchedulerStringId.Abbr_Hour)] = 60;
			ht[SchedulerLocalizer.GetString(SchedulerStringId.Abbr_Hours)] = 60;
			ht[SchedulerLocalizer.GetString(SchedulerStringId.Abbr_DaysShort)] = 1440; 
			ht[SchedulerLocalizer.GetString(SchedulerStringId.Abbr_Day)] = 1440; 
			ht[SchedulerLocalizer.GetString(SchedulerStringId.Abbr_Days)] = 1440; 
			ht[SchedulerLocalizer.GetString(SchedulerStringId.Abbr_WeeksShort)] = 10080; 
			ht[SchedulerLocalizer.GetString(SchedulerStringId.Abbr_Week)] = 10080; 
			ht[SchedulerLocalizer.GetString(SchedulerStringId.Abbr_Weeks)] = 10080; 
		}
		public static TimeSpan Parse(string str) {
			TimeSpanStringConvertEventArgs args = new TimeSpanStringConvertEventArgs();
			args.StringValue = str;
			RaiseParse(args);
			if (args.Handled)
				return args.TimeSpanValue;
			if (str == null)
				return TimeSpan.MinValue;
			TimeSpan result = ParseCore(str);
			if (result == TimeSpan.MinValue)
				return result;
			return ParseCore(ToString(result));
		}
		internal static TimeSpan ParseCore(string str) {
			string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
			separator = separator.Replace(".", @"\.").Trim();
			string positiveFloatWithUnits = @"^\s*(?<val>\d+(" + separator + @"\d*)?)(?<word1Optional>\p{L}+)?(\s+(?<anotherWords>\p{L}+))*\s*$";
			Regex r = new Regex(positiveFloatWithUnits, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
			Match m = r.Match(str.ToLower());
			if (!m.Success)
				return TimeSpan.MinValue;
			Decimal val = Convert.ToDecimal(m.Groups["val"].Value);
			string units = String.Empty;
			Group unitsGroup = m.Groups["word1Optional"];
			if (unitsGroup != null)
				units = unitsGroup.Value;
			unitsGroup = m.Groups["anotherWords"];
			if (unitsGroup != null)
				units += unitsGroup.Value;
			return CalcTimeSpan(val, units);
		}
		static TimeSpan CalcTimeSpan(Decimal val, string units) {
			int minutes;
			if (!ht.TryGetValue(units, out minutes))
				minutes = 1;
			if (minutes == 1)
				val = Decimal.Floor(val + (Decimal)0.5);
			try {
				return TimeSpan.FromMinutes(Convert.ToDouble(val * (Decimal)minutes));
			} catch {
				return TimeSpan.MinValue;
			}
		}
		public static string ToString(TimeSpan val) {
			TimeSpanStringConvertEventArgs args = new TimeSpanStringConvertEventArgs();
			args.TimeSpanValue = val;
			RaiseToString(args);
			if (args.Handled)
				return args.StringValue;
			TimeSpan absTimeSpan;
			if (val == TimeSpan.MinValue)
				absTimeSpan = TimeSpan.MaxValue;
			else
				absTimeSpan = TimeSpan.FromTicks(Math.Abs(val.Ticks));
			string absString = ToHumanReadableString(absTimeSpan);
			return absString;
		}
		static string ToHumanReadableString(TimeSpan val) {
			string result = String.Empty;
			if (val >= DateTimeHelper.WeekSpan)
				result = ToWeeksString(val);
			if (result.Length == 0 && val >= TimeSpan.FromHours(23))
				result = ToDaysString(val);
			if (result.Length == 0 && val >= DateTimeHelper.HourSpan)
				result = ToHoursString(val);
			return result.Length == 0 ? ToMinutesString(val) : result;
		}
		static string ToUnitsString(TimeSpan val, TimeSpan divider, Decimal[] rounding, SchedulerStringId unitStringId, SchedulerStringId severalUnitsStringId) {
			Decimal decimalResult = val.Ticks / (Decimal)divider.Ticks;
			Decimal intResult = Decimal.Floor(decimalResult);
			int count = rounding.Length;
			for (int i = 0; i < count; i++) {
				if ((Decimal)Math.Abs((Decimal)(decimalResult - (Decimal)(intResult + rounding[i]))) < (Decimal)0.0001f)
					return String.Format("{0:0.##} {1}", decimalResult, SchedulerLocalizer.GetString(decimalResult == (Decimal)1.0 && rounding[i] == 0 ? unitStringId : severalUnitsStringId));
			}
			return String.Empty;
		}
		static string ToWeeksString(TimeSpan val) {
			return ToUnitsString(val, DateTimeHelper.WeekSpan, Halfs, SchedulerStringId.Abbr_Week, SchedulerStringId.Abbr_Weeks);
		}
		static string ToDaysString(TimeSpan val) {
			return ToUnitsString(val, DateTimeHelper.DaySpan, TenthsAndQuarters, SchedulerStringId.Abbr_Day, SchedulerStringId.Abbr_Days);
		}
		static string ToHoursString(TimeSpan val) {
			return ToUnitsString(val, DateTimeHelper.HourSpan, TenthsAndQuarters, SchedulerStringId.Abbr_Hour, SchedulerStringId.Abbr_Hours);
		}
		static string ToMinutesString(TimeSpan val) {
			Decimal decimalResult = val.Ticks / (Decimal)TimeSpan.FromMinutes(1).Ticks;
			Decimal intResult = Decimal.Floor(decimalResult + (Decimal)0.5);
			if (intResult < 0)
				intResult = 0;
			return String.Format("{0} {1}", intResult, SchedulerLocalizer.GetString(intResult == 1 ? SchedulerStringId.Abbr_Minute : SchedulerStringId.Abbr_Minutes));
		}
	}
	#endregion
	#region ReminderTimeSpans
	public static class ReminderTimeSpans {
		static ReminderTimeSpans() {
			List<TimeSpan> reminderTimeSpanValues = new List<TimeSpan>();
			reminderTimeSpanValues.Add(TimeSpan.FromMinutes(0));
			reminderTimeSpanValues.AddRange(ReminderTimeSpanWithoutZeroValues);
			ReminderTimeSpanValues = reminderTimeSpanValues.ToArray();
		}
		public static TimeSpan[] ReminderTimeSpanWithoutZeroValues = new TimeSpan[] {
			TimeSpan.FromMinutes(5),
			TimeSpan.FromMinutes(10),
			TimeSpan.FromMinutes(15),
			TimeSpan.FromMinutes(30),
			TimeSpan.FromHours(1),
			TimeSpan.FromHours(2),
			TimeSpan.FromHours(3),
			TimeSpan.FromHours(4),
			TimeSpan.FromHours(5),
			TimeSpan.FromHours(6),
			TimeSpan.FromHours(7),
			TimeSpan.FromHours(8),
			TimeSpan.FromHours(9),
			TimeSpan.FromHours(10),
			TimeSpan.FromHours(11),
			TimeSpan.FromHours(12),
			TimeSpan.FromHours(18),
			TimeSpan.FromDays(1),
			TimeSpan.FromDays(2),
			TimeSpan.FromDays(3),
			TimeSpan.FromDays(4),
			TimeSpan.FromDays(7),
			TimeSpan.FromDays(14)
		};
		public static TimeSpan[] ReminderTimeSpanValues;
		public static TimeSpan[] BeforeStartTimeSpanValues = new TimeSpan[] {
			TimeSpan.FromMinutes(-15),
			TimeSpan.FromMinutes(-10),
			TimeSpan.FromMinutes(-5),
			TimeSpan.FromMinutes(0),
		};
	}
	#endregion
	#region VisibleIntervalFormatter
	public class VisibleIntervalFormatter {
		bool formatForMonth;
		string whiteSpace = String.Empty;
		public VisibleIntervalFormatter() {
			this.ShowWeekend = true;
		}
		public string WhiteSpace { get { return whiteSpace; } set { whiteSpace = value; } }
		public bool FormatForMonth { get { return formatForMonth; } set { formatForMonth = value; } }
		public bool ShowWeekend { get; set; }
		public virtual string Format(TimeInterval interval, string datePairFormat) {
			TimeInterval actualTimeInterval = CreateActualFormatInterval(interval);
			if (FormatForMonth)
				return FormatForMonthViewCore(actualTimeInterval, datePairFormat);
			else
				return FormatCore(actualTimeInterval, datePairFormat);
		}
		protected internal virtual string ReplaceWhiteSpace(string str) {
			if (String.IsNullOrEmpty(WhiteSpace))
				return str;
			else
				return str.Replace(" ", WhiteSpace);
		}
		protected internal virtual string GetDefaultDateFormat() {
			DateTimeFormatInfo dtfi = DateTimeFormatHelper.CurrentDateTimeFormat;
			return dtfi.LongDatePattern;
		}
		protected internal virtual TimeInterval CreateActualFormatInterval(TimeInterval interval) {
			DateTime start = interval.Start.Date;
			DateTime end = DateTimeHelper.Ceil(interval.End, DateTimeHelper.DaySpan).AddTicks(-1).Date;
			if (end < start)
				end = start;
			if (!ShowWeekend) {
				if (start.DayOfWeek == DayOfWeek.Saturday)
					start = start.AddDays(1);
				if (start.DayOfWeek == DayOfWeek.Sunday)
					start = start.AddDays(1);
				if (end.DayOfWeek == DayOfWeek.Sunday)
					end = end.AddDays(-1);
				if (end.DayOfWeek == DayOfWeek.Saturday)
					end = end.AddDays(-1);
				if (end < start)
					end = start;
			}
			return new TimeInterval(start, end);
		}
		protected internal virtual string FormatCore(TimeInterval interval, string datePairFormat) {
			string endDateFormat = GetDefaultDateFormat();
			endDateFormat = DateTimeFormatHelper.StripDayOfWeek(endDateFormat);
			if (interval.Duration == TimeSpan.Zero)
				return ReplaceWhiteSpace(SysDate.ToString(endDateFormat, interval.Start));
			string startDateFormat = CalculateStartDateFormat(interval.Start, interval.End, endDateFormat);
			string start = ReplaceWhiteSpace(SysDate.ToString(startDateFormat, interval.Start));
			string end = ReplaceWhiteSpace(SysDate.ToString(endDateFormat, interval.End));
			return String.Format(datePairFormat, start, end);
		}
		protected internal virtual string FormatForMonthViewCore(TimeInterval interval, string datePairFormat) {
			MonthIntervalCollection months = new MonthIntervalCollection();
			months.Add(interval);
			string endDateFormat = GetDefaultDateFormat();
			endDateFormat = DateTimeFormatHelper.StripDayOfWeek(endDateFormat);
			endDateFormat = DateTimeFormatHelper.StripDay(endDateFormat);
			switch (months.Count) {
				case 1:
					return ReplaceWhiteSpace(SysDate.ToString(endDateFormat, interval.Start));
				default:
				case 2:
					return FormatForMonthIntervalCore(interval, datePairFormat, endDateFormat);
				case 3:
					if (TimeInterval.Intersect(interval, months[0]).Duration <= TimeSpan.FromDays(6) &&
						TimeInterval.Intersect(interval, months[2]).Duration <= TimeSpan.FromDays(6))
						return ReplaceWhiteSpace(SysDate.ToString(endDateFormat, months[1].Start));
					else
						return FormatForMonthIntervalCore(interval, datePairFormat, endDateFormat);
			}
		}
		protected internal virtual string FormatForMonthIntervalCore(TimeInterval interval, string datePairFormat, string endDateFormat) {
			string startDateFormat = CalculateMonthViewStartDateFormat(interval.Start, interval.End, endDateFormat);
			string start = ReplaceWhiteSpace(SysDate.ToString(startDateFormat, interval.Start));
			string end = ReplaceWhiteSpace(SysDate.ToString(endDateFormat, interval.End));
			return String.Format(datePairFormat, start, end);
		}
		protected internal virtual string CalculateStartDateFormat(DateTime start, DateTime end, string endDateFormat) {
			string result = endDateFormat;
			if (end.Year == start.Year) {
				result = DateTimeFormatHelper.StripYear(result);
				if (end.Month == start.Month)
					result = DateTimeFormatHelper.StripMonth(result);
			}
			return result;
		}
		protected internal virtual string CalculateMonthViewStartDateFormat(DateTime start, DateTime end, string endDateFormat) {
			string result = endDateFormat;
			if (end.Year == start.Year)
				result = DateTimeFormatHelper.StripYear(result);
			return result;
		}
	}
	#endregion
	#region SchedulerVisibleIntervalFormatter
	public class SchedulerVisibleIntervalFormatter : VisibleIntervalFormatter {
		InnerSchedulerViewBase view;
		public SchedulerVisibleIntervalFormatter(InnerSchedulerViewBase view) {
			if (view == null)
				Exceptions.ThrowArgumentNullException("view");
			this.view = view;
			FormatForMonth = view.Type == SchedulerViewType.Month;
			if (FormatForMonth) {
				InnerMonthView monthView = view as InnerMonthView;
				if (monthView != null)
					ShowWeekend = monthView.ShowWeekend;
			}
		}
		protected internal InnerSchedulerViewBase View { get { return view; } }
	}
	#endregion
	#region XtraShouldSerializePropertyDelegate
	public delegate bool XtraShouldSerializePropertyDelegate();
	#endregion
	public class XtraSupportShouldSerializeHelper : IXtraSupportShouldSerialize {
		Dictionary<string, XtraShouldSerializePropertyDelegate> table = new Dictionary<string, XtraShouldSerializePropertyDelegate>();
		public void RegisterXtraShouldSerializeMethod(string propertyName, XtraShouldSerializePropertyDelegate method) {
			table.Add(propertyName, method);
		}
		internal Dictionary<string, XtraShouldSerializePropertyDelegate> Table { get { return table; } }
		#region IXtraSupportShouldSerialize Members
		public bool ShouldSerialize(string propertyName) {
			XtraShouldSerializePropertyDelegate method;
			if (table.TryGetValue(propertyName, out method))
				return method();
			else
				return true;
		}
		#endregion
	}
	public static class SchedulerCommandImagesNames {
		public const string ResourcePath = "DevExpress.XtraScheduler.Images";
		public const string Appointment = "Appointment";
		public const string RecurringAppointment = "RecurringAppointment";
		public const string GoToDate = "GoToDate";
		public const string Reminder = "Reminder";
		public const string Delete = "Delete";
		public const string GroupByNone = "GroupByNone";
		public const string GroupByDate = "GroupByDate";
		public const string GroupByResource = "GroupByResource";
		public const string Print = "Print";
		public const string PrintPreview = "Preview";
		public const string PrintPageSetup = "PageSetup";
		public const string SwitchTimeScalesTo = "SwitchTimeScalesTo";
		public const string ShowWorkTimeOnly = "ShowWorkTimeOnly";
		public const string CompressWeekend = "CompressWeekend";
		public const string CellsAutoHeight = "CellsAutoHeight";
		public const string Open = "Open";
		public const string SplitAppointment = "SplitAppointment";
		public const string ToggleRecurrence = "ToggleRecurrence";
		public const string OpenSchedule = "OpenSchedule";
		public const string SaveSchedule = "SaveSchedule";
		public const string SnapToCells = "SnapToCells";
		public const string ChangeStatus = "ChangeStatus";
		public const string ChangeLabel = "ChangeLabel";
	}
	public class SchedulerBlocker : IDisposable {
		int lockCount;
		public bool IsLocked { get { return lockCount > 0; } }
		public SchedulerBlocker Lock() {
			lockCount++;
			return this;
		}
		public void Unlock() {
			if (IsLocked)
				lockCount--;
		}
		public static implicit operator bool (SchedulerBlocker locker) {
			return locker.IsLocked;
		}
		void IDisposable.Dispose() {
			Unlock();
		}
	}
	public static class SchedulerViewAutomaticAdjustHelper {
		public static SchedulerViewType SelectAdjustedView(InnerSchedulerControl innerControl, DayIntervalCollection days) {
			SchedulerViewType newViewType = CalculateNewViewTypeByDays(innerControl, days);
			return ChooseEnabledViewType(innerControl, newViewType);
		}
		internal static SchedulerViewType CalculateNewViewTypeByDays(InnerSchedulerControl innerControl, DayIntervalCollection days) {
			if (IsExactMonths(days)) {
				return SchedulerViewType.Month;
			}
			int dayCount = days.Count;
			if (dayCount == 7) {
				SchedulerViewType weekViewType = (innerControl.FullWeekView.Enabled) ? SchedulerViewType.FullWeek : SchedulerViewType.Week;
				DayOfWeek weekViewFirstDayOfWeek = innerControl.Views.GetInnerView(weekViewType).GetVisibleIntervals().Start.DayOfWeek;
				if (IsWholeWeekDays(weekViewFirstDayOfWeek, days))
					return weekViewType;
			} else {
				DayOfWeek weekViewFirstDayOfWeek = innerControl.MonthView.GetVisibleIntervals().Start.DayOfWeek;
				if (IsWholeWeekDays(weekViewFirstDayOfWeek, days))
					return SchedulerViewType.Month;
			}
			if ((innerControl.ActiveViewType == SchedulerViewType.FullWeek || innerControl.ActiveViewType == SchedulerViewType.Week) && dayCount == 1)
				return innerControl.ActiveViewType;
			if (innerControl.ActiveViewType == SchedulerViewType.WorkWeek && dayCount == 1)
				return SchedulerViewType.WorkWeek;
			else
				return SchedulerViewType.Day;
		}
		internal static bool IsExactMonths(DayIntervalCollection days) {
			int count = days.Count;
			if (count > 0) {
				DateTime start = days[0].Start;
				DateTime end = days[count - 1].End;
				int daysCountInInterval = (days[count - 1].End - days[0].Start).Days;
				if (start.Day == 1 && end.Day == 1 && daysCountInInterval == count)
					return true;
			}
			return false;
		}
		internal static SchedulerViewType ChooseEnabledViewType(InnerSchedulerControl innerControl, SchedulerViewType viewType) {
			List<SchedulerViewType> viewTypesByPriority = new List<SchedulerViewType> { SchedulerViewType.Timeline, SchedulerViewType.Month, SchedulerViewType.Week, SchedulerViewType.FullWeek, SchedulerViewType.Day, SchedulerViewType.WorkWeek };
			if (innerControl.Views.GetInnerView(viewType).Enabled)
				return viewType;
			int viewTypeIndx = viewTypesByPriority.IndexOf(viewType);
			if (viewTypeIndx < 0)
				return viewType;
			int count = viewTypesByPriority.Count;
			for (int i = 0; i < count; i++) {
				int indx = (i + viewTypeIndx) % count;
				SchedulerViewType tryViewType = viewTypesByPriority[indx];
				if (innerControl.Views.GetInnerView(tryViewType).Enabled)
					return tryViewType;
			}
			return innerControl.ActiveViewType;
		}
		internal static bool IsWholeWeekDays(DayOfWeek firstDayOfWeek, DayIntervalCollection days) {
			if (days.Count == 0)
				return false;
			if ((days.Count % 7) != 0)
				return false;
			int weekCount = days.Count / 7;
			TimeSpan weekSpan = TimeSpan.FromDays(6);
			for (int i = 0, dayIndex = 0; i < weekCount; i++, dayIndex += 7) {
				DateTime date = days[dayIndex].Start;
				if (date.DayOfWeek != firstDayOfWeek)
					return false;
				if (days[dayIndex + 6].Start - date != weekSpan)
					return false;
			}
			return true;
		}
	}
	#region SchedulerLogger
	[GeneratedCode("Suppress FxCop check", "")]
	public class LoggerTraceLevel {
		public const int None = 0;
		public const int DateNavigator = 0x1;
		public const int Warning = 0x2;
		public const int Mouse = 0x4;
	}
	public static class SchedulerLogger {
		public static int UsageMessageType = LoggerTraceLevel.None;
		static int indent = 0;
		[Conditional("DEBUGTEST")]
		public static void TraceOpenGroup(int level, string format, params object[] args) {
			if (!CanWriteMessage(level))
				return;
			WriteMessage(format, args);
			indent += 4;
		}
		[Conditional("DEBUGTEST")]
		public static void TraceCloseGroup(int level, string format, params object[] args) {
			if (!CanWriteMessage(level))
				return;
			TraceCloseGroup(level);
			WriteMessage(format, args);
		}
		[Conditional("DEBUGTEST")]
		public static void Trace(int level, string format, params object[] args) {
			if (!CanWriteMessage(level))
				return;
			WriteMessage(format, args);
		}
		[Conditional("DEBUGTEST")]
		public static void TraceCloseGroup(int level) {
			if (!CanWriteMessage(level))
				return;
			indent -= 4;
			if (indent < 0)
				indent = 0;
		}
		[Conditional("DEBUGTEST")]
		public static void WriteWithoutIndent(string message) {
			System.Diagnostics.Debug.WriteLine(message);
		}
		[Conditional("DEBUGTEST")]
		public static void WriteWithoutIndent(int messageType, string message) {
			if (CanWriteMessage(messageType))
				WriteWithoutIndent(message);
		}
		[Conditional("DEBUGTEST")]
		static void WriteMessage(string format, object[] args) {
			System.Diagnostics.Debug.WriteLine(new String(' ', indent) + String.Format(format, args));
		}
		static bool CanWriteMessage(int messageType) {
			return (messageType & UsageMessageType) != 0;
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler {
#if !SL
	public enum MouseWheelScrollAction {
		Time = 0,
		Auto = 1
	}
#endif
#if SL
	#region Algorithms (Max, Min)
	public static class Algorithms {
		public static T Min<T>(T index1, T index2) where T : IComparable<T> {
			if (index1.CompareTo(index2) < 0)
				return index1;
			else
				return index2;
		}
		public static T Max<T>(T index1, T index2) where T : IComparable<T> {
			if (index1.CompareTo(index2) < 0)
				return index2;
			else
				return index1;
		}
	}
	#endregion
#endif
	#region QueryDeleteAppointmentResult (obsoleted)
	[Obsolete("You should use the 'RecurrentAppointmentAction' instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
	public enum QueryDeleteAppointmentResult {
		Cancel = 0,
		Series = 1,
		Occurrence = 2
	}
	#endregion
	#region RecurrentAppointmentAction
	public enum RecurrentAppointmentAction {
		Cancel = 0,
		Series = 1,
		Occurrence = 2,
		Ask = 3
	}
	#endregion
	public class MappingException : Exception {
		public MappingException(String message)
			: base(message) {
		}
	}
	#region AppointmentBaseCollection
	public class AppointmentBaseCollection : SchedulerCollectionBase<Appointment> {
		public AppointmentBaseCollection() {
		}
		protected internal AppointmentBaseCollection(DXCollectionUniquenessProviderType uniquenessProviderType)
			: base(uniquenessProviderType) {
		}
		protected override bool IsIndexCacheEnabled {
			get { return true; }
		}
		internal Appointment FindAppointmentExact(TimeInterval interval) {
			int count = Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = this[i];
				if (AppointmentIntersectionHelper.EqualsToInterval(apt, interval))
					return apt;
			}
			return null;
		}
		internal Appointment FindAppointmentExact(TimeInterval interval, object resourceId) {
			for (int i = 0; i < Count; i++) {
				Appointment apt = this[i];
				if (AppointmentIntersectionHelper.EqualsToInterval(apt, interval) && Object.Equals(apt.ResourceId, resourceId))
					return apt;
			}
			return null;
		}
		public virtual AppointmentBaseCollection GetAppointments(TimeInterval interval) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			int count = Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = this[i];
				if (AppointmentIntersectionHelper.IntersectsWithInterval(apt, interval))
					result.Add(apt);
			}
			return result;
		}
		[Obsolete("You should use the 'GetAppointments' instead.", false)]
		protected AppointmentBaseCollection GetAppointmentsCoreZeroDurationInterval(TimeInterval interval) {
			return GetAppointments(interval);
		}
		[Obsolete("You should use the 'GetAppointments' instead.", false)]
		protected AppointmentBaseCollection GetAppointmentsCoreNonZeroDurationInterval(TimeInterval interval) {
			return GetAppointments(interval);
		}
		public AppointmentBaseCollection FindAll(Predicate<Appointment> match) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			FindAllCore(result, match);
			return result;
		}
		public static AppointmentBaseCollection GetAppointmentsFromSortedCollection(AppointmentBaseCollection coll, TimeInterval interval) {
			DateTime intervalEnd = interval.End;
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			result.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
			int count = coll.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = coll[i];
				if (apt.Start > intervalEnd)
					break;
				if (AppointmentIntersectionHelper.IntersectsWithInterval(apt, interval))
					result.Add(apt);
			}
			return result;
		}
	}
	#endregion
	#region ResourceBaseCollection
	public class ResourceBaseCollection : SchedulerCollectionBase<Resource> {
		Dictionary<object, Resource> idHash = new Dictionary<object, Resource>();
		public ResourceBaseCollection() {
		}
		public ResourceBaseCollection(DXCollectionUniquenessProviderType uniquenessProviderType)
			: base(uniquenessProviderType) {
		}
		internal Dictionary<object, Resource> IdHash { get { return idHash; } }
		protected override bool IsIndexCacheEnabled {
			get { return true; }
		}
		internal int IndexOfById(object resourceId) {
			return IndexOf(GetResourceById(resourceId));
		}
		public Resource GetResourceById(object resourceId) {
			return ResourceExists(resourceId) ? idHash[resourceId] : ResourceBase.Empty;
		}
		protected override void OnInsertComplete(int index, Resource value) {
			base.OnInsertComplete(index, value);
			if (value.Id != null) {
				if (!IdHash.ContainsKey(value.Id))
					IdHash.Add(value.Id, value);
				else
					IdHash[value.Id] = value;
			}
		}
		protected override void OnRemoveComplete(int index, Resource value) {
			base.OnRemoveComplete(index, value);
			object id = value.Id;
			if (id != null && IdHash.ContainsKey(id))
				IdHash.Remove(id);
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			IdHash.Clear();
		}
		protected internal bool ResourceExists(object resourceId) {
			return resourceId != null ? idHash.ContainsKey(resourceId) : false;
		}
		internal void AddRange(ResourceBaseCollection resources, int startIndex, int count) {
			int endIndex = Math.Min(startIndex + count, resources.Count) - 1;
			for (int i = startIndex; i <= endIndex; i++)
				Add(resources[i]);
		}
		internal ResourceBaseCollection Clone() {
			ResourceBaseCollection result = new ResourceBaseCollection();
			result.AddRange(this);
			return result;
		}
		public ResourceBaseCollection FindAll(Predicate<Resource> match) {
			ResourceBaseCollection result = new ResourceBaseCollection();
			FindAllCore(result, match);
			return result;
		}
	}
	#endregion
	#region ResourceIdCollection
	public class ResourceIdCollection : NotificationCollection<object> {
	}
	#endregion
	#region AppointmentResourceIdCollection
	public class AppointmentResourceIdCollection : ResourceIdCollection {
		public AppointmentResourceIdCollection() : this(true) {
		}
		public AppointmentResourceIdCollection(bool canAddDefaultContent) {
			if (canAddDefaultContent)
				AddDefaultContent();
		}
		internal IList<object> ObtainInnerList() {
			return this.InnerList;
		}
		protected internal virtual void AddDefaultContent() {
			InnerList.Add(ResourceBase.Empty.Id);
		}
		protected override bool OnInsert(int index, object value) {
			return !Object.ReferenceEquals(value, ResourceBase.Empty.Id) && base.OnInsert(index, value);
		}
		protected override void OnInsertComplete(int index, object value) {
			if (Count > 1) {
				int defaultResourceIndex = InnerList.IndexOf(ResourceBase.Empty.Id);
				if (defaultResourceIndex >= 0)
					InnerList.RemoveAt(defaultResourceIndex);
			}
			base.OnInsertComplete(index, value);
		}
		protected override void OnRemoveComplete(int index, object value) {
			if (Count <= 0)
				AddDefaultContent();
			base.OnRemoveComplete(index, value);
		}
		protected override void OnClearComplete() {
			if (Count <= 0)
				AddDefaultContent();
			base.OnClearComplete();
		}
	}
	#endregion
	#region SchedulerImageHelper
	public static class SchedulerImageHelper {
#if !SL
		public static Image CreateImageFromStream(Stream stream) {
			return Image.FromStream(stream);
		}
		public static byte[] GetImageBytes(Image image) {
			return DevExpress.XtraEditors.Controls.ByteImageConverter.ToByteArray(image, System.Drawing.Imaging.ImageFormat.Png);
		}
		public static Image CreateImageFromBytes(byte[] bytes) {
			return DevExpress.XtraEditors.Controls.ByteImageConverter.FromByteArray(bytes);
		}
#else
		public static Image CreateImageFromStream(Stream stream) {
			System.Windows.Media.Imaging.BitmapImage imageSource = new System.Windows.Media.Imaging.BitmapImage();
			imageSource.SetSource(stream);
			return new Image() { Source = imageSource };
		}
		public static byte[] GetImageBytes(Image image) {
			return new byte[] { };
		}
		public static Image CreateImageFromBytes(byte[] bytes) {
			using (MemoryStream stream = new MemoryStream(bytes)) {
				return CreateImageFromStream(stream);
			}
		}
#endif
	}
	#endregion
	#region DelegateHelper
	public static class DelegateHelper {
#if !SL
		public static Delegate RemoveAll(Delegate source, Delegate value) {
			return Delegate.RemoveAll(source, value);
		}
#else
		public static Delegate RemoveAll(Delegate source, Delegate value) {
			Delegate current;
			do {
				current = source;
				source = Delegate.Remove(source, value);
			}
			while (current != source);
			return current;
		}
#endif
	}
	#endregion
#if SL
	#region UnboundColumnType
	public enum UnboundColumnType {
		Bound,
		Integer,
		Decimal,
		DateTime,
		String,
		Boolean,
		Object
	}
	#endregion
	public class UnboundColumnInfo {
	#region Fields
		static Type[] dataTypes = new Type[] { typeof(object), typeof(int), typeof(decimal), typeof(DateTime), typeof(string), typeof(bool), typeof(object) };
		UnboundColumnType columnType;
		Type dataType;
		string name;
		bool readOnly;
	#endregion
		public UnboundColumnInfo()
			: this(string.Empty, UnboundColumnType.Object, true) {
		}
		public UnboundColumnInfo(string name, UnboundColumnType columnType, bool readOnly) {
			this.name = name;
			this.columnType = columnType;
			this.readOnly = readOnly;
			this.dataType = GetDataType();
		}
	#region Properties
		public UnboundColumnType ColumnType {
			get { return columnType; }
			set {
				columnType = value;
				dataType = GetDataType();
			}
		}
		public Type DataType { get { return dataType; } set { dataType = value; } }
		public string Name { get { return name; } set { name = value; } }
		public bool ReadOnly { get { return readOnly; } set { readOnly = value; } }
	#endregion
		protected Type GetDataType() {
			return dataTypes[(int)ColumnType];
		}
	}
#endif
}
namespace DevExpress.XtraScheduler.Design {
	#region DesignSR
	public static class DesignSR {
		public const string ProductName = "DevExpress.XtraScheduler";
		public const string NoneString = "(none)";
	}
	#endregion
#if !SL
	#region PropertyDescriptorWrapper
	public class PropertyDescriptorWrapper : PropertyDescriptor {
		PropertyDescriptor pd;
		public PropertyDescriptorWrapper(PropertyDescriptor pd)
			: base(pd, new Attribute[] { }) {
			this.pd = pd;
		}
		internal PropertyDescriptor WrappedPropertyDescriptor { get { return pd; } }
		#region MemberDescriptor wrapping
		public override AttributeCollection Attributes { get { return pd.Attributes; } }
		public override string Category { get { return pd.Category; } }
		public override string Description { get { return pd.Description; } }
		public override bool DesignTimeOnly { get { return pd.DesignTimeOnly; } }
		public override string DisplayName { get { return pd.DisplayName; } }
		public override bool IsBrowsable { get { return pd.IsBrowsable; } }
		public override string Name { get { return pd.Name; } }
		#endregion
		#region PropertyDescriptor wrapping
		public override Type ComponentType { get { return pd.ComponentType; } }
		public override TypeConverter Converter { get { return pd.Converter; } }
		public override bool IsLocalizable { get { return pd.IsLocalizable; } }
		public override bool IsReadOnly { get { return pd.IsReadOnly; } }
		public override Type PropertyType { get { return pd.PropertyType; } }
		public override bool SupportsChangeEvents { get { return pd.SupportsChangeEvents; } }
		public override void AddValueChanged(object component, EventHandler handler) {
			pd.AddValueChanged(component, handler);
		}
		public override bool CanResetValue(object component) {
			return pd.CanResetValue(component);
		}
		public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter) {
			return pd.GetChildProperties(instance, filter);
		}
		public override object GetEditor(Type editorBaseType) {
			return pd.GetEditor(editorBaseType);
		}
		public override object GetValue(object component) {
			return pd.GetValue(component);
		}
		public override void RemoveValueChanged(object component, EventHandler handler) {
			pd.RemoveValueChanged(component, handler);
		}
		public override void ResetValue(object component) {
			pd.ResetValue(component);
		}
		public override void SetValue(object component, object value) {
			pd.SetValue(component, value);
		}
		public override bool ShouldSerializeValue(object component) {
			return pd.ShouldSerializeValue(component);
		}
		#endregion
	}
	#endregion
	#region AddAttributesPropertyDescriptor
	public class AddAttributesPropertyDescriptor : PropertyDescriptorWrapper {
		Attribute[] attributesToAppend;
		public AddAttributesPropertyDescriptor(PropertyDescriptor pd, Attribute[] attributesToAppend)
			: base(pd) {
			if (attributesToAppend == null)
				Exceptions.ThrowArgumentNullException("attributesToAppend");
			this.attributesToAppend = attributesToAppend;
		}
		internal Attribute[] AttributesToAppend { get { return attributesToAppend; } }
		public override AttributeCollection Attributes { get { return AttributeCollection.FromExisting(base.Attributes, attributesToAppend); } }
	}
	#endregion
	#region ForceSerializePropertyDescriptorWrapper
	public class ForceSerializePropertyDescriptorWrapper : PropertyDescriptorWrapper {
		public ForceSerializePropertyDescriptorWrapper(PropertyDescriptor pd)
			: base(pd) {
		}
		public override bool ShouldSerializeValue(object component) {
			return true;
		}
	}
	#endregion
	#region IPropertyDescriptorModifier
	public interface IPropertyDescriptorModifier {
		PropertyDescriptor Modify(PropertyDescriptor pd);
	}
	#endregion
	#region UndoRedoSerializationPropertyDescriptorModifier
	public class UndoRedoSerializationPropertyDescriptorModifier : IPropertyDescriptorModifier {
		public virtual PropertyDescriptor Modify(PropertyDescriptor pd) {
			if (pd.IsReadOnly)
				return pd;
			AttributeCollection attrs = pd.Attributes;
			int count = attrs.Count;
			for (int i = 0; i < count; i++) {
				DesignerSerializationVisibilityAttribute designerSerializationAttribute = attrs[i] as DesignerSerializationVisibilityAttribute;
				if (designerSerializationAttribute != null && designerSerializationAttribute.Visibility == DesignerSerializationVisibility.Hidden)
					return pd;
			}
			return new ForceSerializePropertyDescriptorWrapper(pd);
		}
	}
	#endregion
	public abstract class ModifiedTypeDescriptor : CustomTypeDescriptor {
		#region Fields
		IPropertyDescriptorModifier modifier;
		ICustomTypeDescriptor parentTypeDescriptor;
		Type objectType;
		object instance;
		int recurrenceDeep;
		#endregion
		protected ModifiedTypeDescriptor(IPropertyDescriptorModifier modifier, ICustomTypeDescriptor parentTypeDescriptor, Type objectType, object instance)
			: base(parentTypeDescriptor) {
			Guard.ArgumentNotNull(modifier, "modifier");
			Guard.ArgumentNotNull(parentTypeDescriptor, "parentTypeDescriptor");
			Guard.ArgumentNotNull(objectType, "objectType");
			this.modifier = modifier;
			this.parentTypeDescriptor = parentTypeDescriptor;
			this.objectType = objectType;
			this.instance = instance;
		}
		#region Properties
		protected internal ICustomTypeDescriptor ParentTypeDescriptor { get { return parentTypeDescriptor; } }
		protected internal IPropertyDescriptorModifier Modifier { get { return modifier; } }
		protected internal Type ObjectType { get { return objectType; } }
		protected internal object Instance { get { return instance; } }
		internal int RecurrenceDeep { get { return recurrenceDeep; } }
		#endregion
		public override PropertyDescriptorCollection GetProperties() {
			return this.GetProperties(null);
		}
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
			lock (this) {
				if (recurrenceDeep > 0) {
					RefreshInstanceTypeDescriptor();
					return parentTypeDescriptor.GetProperties(attributes);
				}
				PropertyDescriptorCollection properties = parentTypeDescriptor.GetProperties(attributes);
				if (ShouldModifyProperties()) {
					properties = ModifyProperties(properties);
					recurrenceDeep++;
					try {
						RefreshInstanceTypeDescriptor();
					} finally {
						recurrenceDeep--;
					}
				}
				return properties;
			}
		}
		protected internal virtual PropertyDescriptorCollection ModifyProperties(PropertyDescriptorCollection properties) {
			List<PropertyDescriptor> result = new List<PropertyDescriptor>(properties.Count);
			foreach (PropertyDescriptor pd in properties) {
				if (ShouldModifyPropertyDescriptor(pd)) {
					PropertyDescriptor descriptor = Modifier.Modify(pd);
					result.Add(descriptor);
				} else
					result.Add(pd);
			}
			return new PropertyDescriptorCollection(result.ToArray());
		}
		protected internal virtual void RefreshInstanceTypeDescriptor() {
			TypeDescriptor.Refresh(instance);
		}
		protected internal abstract bool ShouldModifyProperties();
		protected internal abstract bool ShouldModifyPropertyDescriptor(PropertyDescriptor pd);
	}
	public class EnsureInnerObjectUndoRedoSupportTypeDescriptor : ModifiedTypeDescriptor {
		public EnsureInnerObjectUndoRedoSupportTypeDescriptor(ICustomTypeDescriptor parentTypeDescriptor, Type objectType, object instance)
			: base(new UndoRedoSerializationPropertyDescriptorModifier(), parentTypeDescriptor, objectType, instance) {
		}
		protected internal override bool ShouldModifyProperties() {
			return Environment.StackTrace.Contains("UndoEngine");
		}
		protected internal override bool ShouldModifyPropertyDescriptor(PropertyDescriptor pd) {
			return Instance == null || !pd.ShouldSerializeValue(Instance);
		}
	}
	public class EnsureInnerObjectUndoRedoSupportTypeDescriptionProvider : TypeDescriptionProvider {
		TypeDescriptionProvider parent;
		public EnsureInnerObjectUndoRedoSupportTypeDescriptionProvider(TypeDescriptionProvider parent)
			: base(parent) {
			this.parent = parent;
		}
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
			ICustomTypeDescriptor parentTypeDescriptor = parent.GetTypeDescriptor(objectType, instance);
			return new EnsureInnerObjectUndoRedoSupportTypeDescriptor(parentTypeDescriptor, objectType, instance);
		}
	}
	public class EnsureInnerObjectUndoRedoSupport : IDisposable {
		EnsureInnerObjectUndoRedoSupportTypeDescriptionProvider newProvider;
		Type type;
		[SecuritySafeCritical]
		public EnsureInnerObjectUndoRedoSupport(Type type) {
			this.type = type;
			this.newProvider = new EnsureInnerObjectUndoRedoSupportTypeDescriptionProvider(TypeDescriptor.GetProvider(type));
			TypeDescriptor.AddProvider(newProvider, type);
		}
		#region IDisposable implementation
		[SecuritySafeCritical]
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				TypeDescriptor.RemoveProvider(newProvider, type);
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~EnsureInnerObjectUndoRedoSupport() {
			Dispose(false);
		}
		#endregion
	}
#endif
}
namespace DevExpress.XtraScheduler.VCalendar {
	public static class VCalendarConsts {
		public const string VCalendar10Version = "1.0";
		public const string VCalendar20Version = "2.0";
		public const string DefaultCalendarVersion = VCalendar10Version; 
		public const string DefaultCalendarProductID = "-//Developer Express Inc.//XtraScheduler//EN";
		public const char TimeDesignator = 'T';
		public const char UTCDesignator = 'Z';
		public const string Iso8601DateTimeFormat = "yyyyMMddTHHmmss";
		public const string Iso8601UtcDateTimeFormat = "yyyyMMddTHHmmssZ"; 
	}
	public static class VCalendarConvert {
		public static string FromDateTime(DateTime dateTime, bool utc) {
			return dateTime.ToString(GetDateTimeFormat(utc), CultureInfo.InvariantCulture);
		}
		public static string GetDateTimeFormat(bool utc) {
			return utc ? VCalendarConsts.Iso8601UtcDateTimeFormat : VCalendarConsts.Iso8601DateTimeFormat;
		}
		public static bool IsUtcDateTime(string val) {
			if (string.IsNullOrEmpty(val))
				return false;
			return val.IndexOf(VCalendarConsts.UTCDesignator) == val.Length - 1;
		}
	}
}
namespace DevExpress.XtraScheduler.Internal.Diagnostics {
	[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
	public class XtraSchedulerDebug {
		public static bool SkipInsertionCheck = false;
		public static Function<bool, Type> AllowTraceCallback;
		[System.Diagnostics.Conditional("DEBUG")]
		public static void WriteLine(string formatString, params object[] args) {
			if (AllowTraceCallback == null)
				return;
			MethodBase callForMethod = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
			Type currentType = callForMethod.DeclaringType;
			if (AllowTraceCallback(currentType)) {
				string callString = String.Format("->{0}.{1}: ", currentType.Name, callForMethod.Name);
				System.Diagnostics.Debug.WriteLine(String.Format(callString + formatString, args));
			}
		}
		[System.Diagnostics.Conditional("DEBUG")]
		public static void Assert(bool condition) {
#if !SL
			if (!condition) {
				string trace = Environment.StackTrace.ToLower();
				if (trace.IndexOf("nunittestrunner") < 0 &&
					trace.IndexOf("nunit.core") < 0 &&
					trace.IndexOf("nunittask") < 0)
					System.Diagnostics.Debug.Assert(condition);
#if (DEBUGTEST)
				else
					NUnit.Framework.Assert.Fail("DEBUG ASSERTION FAILED. Please check stack trace.\n" + trace);
#endif
			}
#else
			System.Diagnostics.Debug.Assert(condition);
#endif
		}
		public static void CheckNotNull(string parameterName, object val) {
			if (val == null)
				Exceptions.ThrowArgumentNullException(parameterName);
		}
	}
}
