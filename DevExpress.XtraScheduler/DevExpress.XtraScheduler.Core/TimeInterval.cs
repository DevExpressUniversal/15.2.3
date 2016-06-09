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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
#if SL
using DateTimeConverter = DevExpress.Xpf.ComponentModel.DateTimeConverter;
#else
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using DevExpress.XtraScheduler.Design;
#endif
namespace DevExpress.XtraScheduler {
	#region TimeInterval
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	public class TimeInterval : ICloneable, IXtraSupportShouldSerialize {
		#region Fields
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("TimeIntervalEmpty")]
#endif
		public static TimeInterval Empty { get { return new TimeInterval(DateTime.MinValue, TimeSpan.Zero); } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("TimeIntervalDay")]
#endif
		public static TimeInterval Day { get { return new TimeInterval(DateTime.MinValue, DateTimeHelper.DaySpan); } }
		internal static readonly TimeSpan DefaultDuration = DateTimeHelper.HalfHourSpan;
		internal static readonly DateTime DefaultStart = DateTime.MinValue;
		DateTime start = DefaultStart;
		TimeSpan duration = DefaultDuration;
		bool allDay;
		#endregion
		public TimeInterval() {
		}
		public TimeInterval(DateTime start, TimeSpan duration) {
			this.Start = start;
			this.Duration = duration;
		}
		public TimeInterval(DateTime start, DateTime end) {
			this.Start = start;
			this.End = end;
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeIntervalAllDay"),
#endif
DefaultValue(false), NotifyParentProperty(true), XtraSerializableProperty(100)]
		public virtual bool AllDay {
			get { return allDay; }
			set {
				if (allDay == value)
					return;
				allDay = value;
				RaiseChanged();
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeIntervalStart"),
#endif
NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
#if SL
		[TypeConverter(typeof(DateTimeConverter))]
#endif
		public virtual DateTime Start {
			get { return AllDay ? start.Date : start; }
			set {
				if (ShouldSetStart(value)) {
					SetStartCore(value);
					RaiseChanged();
				}
			}
		}
		internal DateTime InnerStart { get { return start; } }
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeIntervalEnd"),
#endif
NotifyParentProperty(true)]
#if SL
		[TypeConverter(typeof(DateTimeConverter))]
#endif
		public virtual DateTime End {
			get {
				return Start + Duration;
			}
			set {
				TimeSpan newDuration = value - Start;
				if (newDuration.Ticks < 0)
					Exceptions.ThrowArgumentException("End", value);
				if (ShouldSetDuration(newDuration)) {
					SetDurationCore(newDuration);
					RaiseChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeIntervalDuration"),
#endif
NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public virtual TimeSpan Duration {
			get { return (AllDay) ? CalculateAllDayDuration() : duration; }
			set {
				if (value.Ticks < 0)
					Exceptions.ThrowArgumentException("Duration", value);
				if (ShouldSetDuration(value)) {
					SetDurationCore(value);
					RaiseChanged();
				}
			}
		}
		internal TimeSpan InnerDuration { get { return duration; } }
		[Browsable(false)]
		public bool SameDay {
			get {
				return Start.Date == End.Date ||
					(Duration < DateTimeHelper.DaySpan && End == Start.Date + DateTimeHelper.DaySpan);
			}
		}
		[Browsable(false)]
		public bool LongerThanADay { get { return Duration >= DateTimeHelper.DaySpan; } }
		#endregion
		public bool Contains(DateTime date) { return date >= Start && date <= End; }
		public bool Contains(TimeInterval interval) {
			if (interval == null)
				return false;
			return interval.Start >= Start && interval.End <= End;
		}
		protected internal virtual bool ShouldSerializeStart() { return start != DefaultStart; }
		protected internal virtual bool XtraShouldSerializeStart() { return ShouldSerializeStart(); }
		protected internal virtual bool ShouldSerializeEnd() { return false; }
		protected internal virtual bool ShouldSerializeDuration() { return duration != DefaultDuration; }
		protected internal virtual bool XtraShouldSerializeDuration() { return ShouldSerializeDuration(); }
		#region ICloneable implementation
		object ICloneable.Clone() {
			return CloneCore();
		}
		TimeInterval CloneCore() {
			return new TimeInterval(start, duration);
		}
		public TimeInterval Clone() {
			return CloneCore();
		}
		#endregion
		protected internal virtual void RaiseChanged() {
		}
		protected internal virtual bool ShouldSetStart(DateTime value) {
			return start != value && Start != value;
		}
		protected internal virtual void SetStartCore(DateTime value) {
			if (AllDay) {
				value = value.Date;
				TimeSpan validDuration = DateTimeHelper.Min(duration, DateTime.MaxValue - start);
				DateTime nonAllDayEnd = start + validDuration;
				duration = nonAllDayEnd - start.Date;
			}
			start = value;
			long totalTicks = Start.Ticks + Duration.Ticks;
			if (totalTicks > DateTime.MaxValue.Ticks) {
				SetDurationCore(DateTime.MaxValue - Start);
			}
		}
		protected internal virtual bool ShouldSetDuration(TimeSpan value) {
			return duration != value && Duration != value;
		}
		protected internal virtual void SetDurationCore(TimeSpan value) {
			if (AllDay)
				this.start = Start.Date;
			TimeSpan validDuration = DateTimeHelper.Min(DateTime.MaxValue - Start, value);
			this.duration = validDuration;
		}
		public override bool Equals(object obj) {
			TimeInterval val = obj as TimeInterval;
			if (val == null)
				return false;
			return Start == val.Start && Duration == val.Duration;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public static TimeInterval Union(TimeInterval interval1, TimeInterval interval2) {
			if (interval1 == null) {
				if (interval2 == null)
					return TimeInterval.Empty;
				else
					return interval2.Clone();
			}
			if (interval2 == null)
				return interval1;
			long start = Math.Min(interval1.Start.Ticks, interval2.Start.Ticks);
			long end = Math.Max(interval1.End.Ticks, interval2.End.Ticks);
			return new TimeInterval(new DateTime(start), new DateTime(end));
		}
		public static TimeInterval Intersect(TimeInterval interval1, TimeInterval interval2) {
			if (interval1 == null) {
				if (interval2 == null)
					return TimeInterval.Empty;
				else
					return interval2;
			}
			if (interval2 == null)
				return interval1;
			long start = Math.Max(interval1.Start.Ticks, interval2.Start.Ticks);
			long end = Math.Min(interval1.End.Ticks, interval2.End.Ticks);
			return end >= start ? new TimeInterval(new DateTime(start), new DateTime(end)) : TimeInterval.Empty;
		}
		public TimeIntervalCollection Subtract(TimeInterval interval) {
			TimeIntervalCollectionEx result = new TimeIntervalCollectionEx();
			if (interval == null || !IntersectsWithExcludingBounds(interval)) {
				result.Add(this);
				return result;
			}
			if (interval.Contains(this))
				return result;
			if (this.Contains(interval)) {
				result.Add(new TimeInterval(Start, interval.Start));
				result.Add(new TimeInterval(interval.End, End));
				return result;
			}
			if (Start >= interval.Start)
				result.Add(new TimeInterval(interval.End, End));
			else
				result.Add(new TimeInterval(Start, interval.Start));
			return result;
		}
		public bool IntersectsWith(TimeInterval interval) {
			if (interval == null)
				return false;
			return interval.End >= Start && interval.Start <= End;
		}
		public bool IntersectsWithExcludingBounds(TimeInterval interval) {
			if (interval == null)
				return false;
			if (Duration == TimeSpan.Zero && interval.Duration == TimeSpan.Zero && interval.Start == Start)
				return true;
			return interval.End > Start && interval.Start < End;
		}
		protected internal virtual TimeSpan CalculateAllDayDuration() {
			TimeSpan validDuration = DateTimeHelper.Min(duration, DateTime.MaxValue - start);
			TimeSpan span = DateTimeHelper.Ceil(start + validDuration, DateTimeHelper.DaySpan) - start.Date;
			return span.Days > 1 ? span : DateTimeHelper.DaySpan;
		}
		internal TimeOfDayInterval ToTimeOfDayInterval() {
			TimeSpan start = Start.TimeOfDay;
			return new TimeOfDayInterval(start, start + Duration);
		}
		public override string ToString() {
			CultureInfo culture = CultureInfo.CurrentCulture;
			return ToString(culture);
		}
		public string ToString(IFormatProvider provider) {
			return String.Format(provider, "({0})-({1}) [{2}]", Start.ToString(provider), End.ToString(provider), Duration.ToString());
		}
		public TimeInterval Intersect(TimeInterval interval) {
			DateTime start = DateTimeHelper.Max(interval.Start, this.Start);
			DateTime end = DateTimeHelper.Min(interval.End, this.End);
			if (end < start)
				return TimeInterval.Empty;
			else
				return new TimeInterval(start, end);
		}
		public TimeInterval Merge(TimeInterval interval) {
			DateTime start = DateTimeHelper.Min(interval.Start, this.Start);
			DateTime end = DateTimeHelper.Max(interval.End, this.End);
			if (end < start)
				return TimeInterval.Empty;
			else
				return new TimeInterval(start, end);
		}
		#region IXtraSupportShouldSerialize Members
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			if (propertyName == "Start")
				return XtraShouldSerializeStart();
			else if (propertyName == "Duration")
				return XtraShouldSerializeDuration();
			else
				return true;
		}
		#endregion
	}
	#endregion
	#region TimeOfDayInterval
#if !SL
	[TypeConverter(typeof(TimeSpanIntervalConverter<TimeOfDayInterval>))]
#endif
	public class TimeOfDayInterval : ICloneable, ISupportObjectChanged {
		#region Fields
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("TimeOfDayIntervalDay")]
#endif
		public static TimeOfDayInterval Day { get { return new TimeOfDayInterval(TimeSpan.Zero, DateTimeHelper.DaySpan); } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("TimeOfDayIntervalEmpty")]
#endif
		public static TimeOfDayInterval Empty { get { return new TimeOfDayInterval(TimeSpan.Zero, TimeSpan.Zero); } }
		internal static TimeOfDayInterval CreateEmpty() { 
			return new TimeOfDayInterval(TimeSpan.Zero, TimeSpan.Zero); 
		} 
		TimeSpan start;
		TimeSpan end;
		#endregion
		#region ISupportObjectChanged implementation
		EventHandler onChanged;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("TimeOfDayIntervalChanged")]
#endif
		public event EventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		public TimeOfDayInterval() {
			this.start = TimeSpan.Zero;
			this.end = TimeSpan.FromDays(1);
		}
		public TimeOfDayInterval(TimeSpan start, TimeSpan end) {
			if (start > end)
				Exceptions.ThrowArgumentException("end", end);
			this.start = start;
			this.end = end;
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeOfDayIntervalStart"),
#endif
		NotifyParentProperty(true), XtraSerializableProperty(), AutoFormatEnable()]
		public TimeSpan Start {
			get { return start; }
			set {
				if (start.Equals(value))
					return;
				if (value > end)
					Exceptions.ThrowArgumentException("Start", value);
				if (!RaiseChanging("Start", start, value))
					return;
				start = value;
				RaiseChanged();
			}
		}
		protected internal virtual bool ShouldSerializeStart() { return Start != TimeSpan.Zero; }
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeOfDayIntervalEnd"),
#endif
		NotifyParentProperty(true), XtraSerializableProperty(), AutoFormatEnable()]
		public TimeSpan End {
			get { return end; }
			set {
				if (end.Equals(value))
					return;
				if (start > value)
					Exceptions.ThrowArgumentException("End", value);
				if (!RaiseChanging("End", end, value))
					return;
				end = value;
				RaiseChanged();
			}
		}
		protected internal virtual bool ShouldSerializeEnd() { return End != DateTimeHelper.DaySpan; }
		[Browsable(false), AutoFormatDisable(), NotifyParentProperty(true)]
		public TimeSpan Duration {
			get { return end - start; }
			set {
				if (Duration == value)
					return;
				if (value < TimeSpan.Zero)
					Exceptions.ThrowArgumentException("Duration", value);
				if (!RaiseChanging("Duration", Duration, value))
					return;
				end = start + value;
				RaiseChanged();
			}
		}
		protected internal virtual bool ShouldSerializeDuration() { return false; }
		#endregion
		#region Events
		#region Changing
		public event CancellablePropertyChangingEventHandler Changing;
		protected internal bool RaiseChanging(string propertyName, object oldValue, object newValue) {
			if (Changing == null)
				return true;
			CancellablePropertyChangingEventArgs ea = new CancellablePropertyChangingEventArgs(propertyName, oldValue, newValue);
			Changing(this, ea);
			return !ea.Cancel;
		}
		#endregion
		#endregion
		#region ICloneable implementation
		object ICloneable.Clone() {
			return CloneCore();
		}
		protected virtual TimeOfDayInterval CloneCore() {
			return new TimeOfDayInterval(start, end);
		}
		public TimeOfDayInterval Clone() {
			return CloneCore();
		}
		#endregion
		public bool Contains(TimeSpan time) {
			return time >= Start && time <= End;
		}
		public static TimeOfDayInterval Union(TimeOfDayInterval interval1, TimeOfDayInterval interval2) {
			if (interval1 == null) {
				if (interval2 == null)
					return TimeOfDayInterval.Empty;
				else
					return interval2.Clone();
			}
			if (interval2 == null)
				return interval1;
			TimeSpan start = DateTimeHelper.Min(interval1.Start, interval2.Start);
			TimeSpan end = DateTimeHelper.Max(interval1.End, interval2.End);
			return new TimeOfDayInterval(start, end);
		}
		public static TimeOfDayInterval Intersect(TimeOfDayInterval interval1, TimeOfDayInterval interval2) {
			if (interval1 == null) {
				if (interval2 == null)
					return TimeOfDayInterval.Empty;
				else
					return interval2;
			}
			if (interval2 == null)
				return interval1;
			TimeSpan start = DateTimeHelper.Max(interval1.Start, interval2.Start);
			TimeSpan end = DateTimeHelper.Min(interval1.End, interval2.End);
			if (end < start)
				return TimeOfDayInterval.Empty;
			return new TimeOfDayInterval(start, end);
		}
		public bool IntersectsWithExcludingBounds(TimeOfDayInterval interval) {
			return interval.End > Start && interval.Start < End;
		}
		public TimeInterval ToTimeInterval() {
			return ToTimeInterval(DateTime.MinValue);
		}
		public TimeInterval ToTimeInterval(DateTime date) {
			return new TimeInterval(date + Start, Duration);
		}
		public override bool Equals(object obj) {
			TimeOfDayInterval val = obj as TimeOfDayInterval;
			if (val == null)
				return false;
			return IsEqual(val);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		internal bool IsEqual(TimeOfDayInterval val) {
			if (val == null)
				return false;
			return Start == val.Start && End == val.End;
		}
		public override string ToString() {
			return String.Format(CultureInfo.CurrentCulture, "({0})-({1}) [{2}]", Start.ToString(), End.ToString(), Duration.ToString());
		}
		public virtual void Assign(TimeOfDayInterval source) {
			if (source == null)
				return;
			Duration = source.Duration;
			End = source.End;
			Start = source.Start;
		}
	}
	#endregion
	#region TimeOfDayIntervalCollection
	public class TimeOfDayIntervalCollection : DXCollection<TimeOfDayInterval> {
		public TimeOfDayIntervalCollection() {
			this.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("TimeOfDayIntervalCollectionStart")]
#endif
		public TimeSpan Start {
			get {
				if (Count <= 0)
					return TimeSpan.Zero;
				else
					return this[0].Start;
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("TimeOfDayIntervalCollectionEnd")]
#endif
		public TimeSpan End {
			get {
				if (Count <= 0)
					return TimeSpan.Zero;
				else
					return this[Count - 1].End;
			}
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("TimeOfDayIntervalCollectionDuration")]
#endif
		public TimeSpan Duration { get { return End - Start; } }
		internal bool IntersectsWithExcludingBounds(TimeOfDayInterval interval) {
			int count = Count;
			for (int i = 0; i < count; i++) {
				if (this[i].IntersectsWithExcludingBounds(interval))
					return true;
			}
			return false;
		}
	}
	#endregion
	#region WorkTimeInterval
#if !SL
	[TypeConverter(typeof(TimeSpanIntervalConverter<WorkTimeInterval>))]
#endif
	public class WorkTimeInterval : TimeOfDayInterval {
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("WorkTimeIntervalWorkTime")]
#endif
		public static WorkTimeInterval WorkTime { get { return new WorkTimeInterval(TimeSpan.FromHours(9), TimeSpan.FromHours(18)); } }
		public WorkTimeInterval() {
		}
		public WorkTimeInterval(TimeSpan start, TimeSpan end)
			: base(start, end) {
		}
		protected internal override bool ShouldSerializeStart() { return Start != WorkTime.Start || End != WorkTime.End; }
		protected internal override bool ShouldSerializeEnd() { return Start != WorkTime.Start || End != WorkTime.End; }
		protected override TimeOfDayInterval CloneCore() {
			return new WorkTimeInterval(Start, End);
		}
	}
	#endregion
	#region TimeIntervalStartComparer
	public class TimeIntervalStartComparer : IComparer<TimeInterval>, IComparer {
		public int Compare(TimeInterval x, TimeInterval y) {
			return CompareCore(x, y);
		}
		int IComparer.Compare(object x, object y) {
			TimeInterval intervalX = (TimeInterval)x;
			TimeInterval intervalY = (TimeInterval)y;
			return CompareCore(intervalX, intervalY);
		}
		static int CompareCore(TimeInterval intervalX, TimeInterval intervalY) {
			return intervalX.Start.CompareTo(intervalY.Start);
		}
	}
	#endregion
	#region TimeIntervalComparer (compatibility class)
	[Obsolete("You should use the TimeIntervalStartComparer class instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
	public class TimeIntervalComparer : TimeIntervalStartComparer {
	}
	#endregion
	#region TimeIntervalCollection
	public class TimeIntervalCollection : NotificationCollection<TimeInterval>, ICloneable {
		#region Fields
		bool keepSorted = true;
		public TimeIntervalCollection() {
		}
		internal TimeIntervalCollection(DXCollectionUniquenessProviderType uniquenessProviderType)
			: base(uniquenessProviderType) {
		}
		#endregion
		#region Properties
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("TimeIntervalCollectionStart")]
#endif
		public DateTime Start { get { return Count > 0 ? this[0].Start : DateTime.MinValue; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("TimeIntervalCollectionEnd")]
#endif
		public DateTime End { get { return CalculateEnd(); } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("TimeIntervalCollectionDuration")]
#endif
		public TimeSpan Duration { get { return End - Start; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("TimeIntervalCollectionInterval")]
#endif
		public TimeInterval Interval { get { return new TimeInterval(Start, End); } }
		protected virtual bool KeepSorted { get { return keepSorted; } set { keepSorted = value; } }
		#endregion
		public void SetContent(TimeIntervalCollection coll) {
			BeginUpdate();
			try {
				Clear();
				AddRange(coll);
			} finally {
				EndUpdate();
			}
		}
		public void SetContent(TimeInterval interval) {
			BeginUpdate();
			try {
				Clear();
				Add(interval);
			} finally {
				EndUpdate();
			}
		}
		public override void AddRange(ICollection intervals) {
			if (intervals == null)
				return;
			if (intervals.Count <= 0)
				return;
			OnInsert(-1, null);
			AddRangeCore(intervals);
			OnInsertComplete(-1, null);
		}
		public override int Add(TimeInterval interval) {
			OnInsert(Count - 1, interval);
			AddCore(interval);
			OnInsertComplete(Count - 1, interval);
			return Count - 1;
		}
		protected override void AddRangeCore(ICollection collection) {
			if (collection == null)
				return;
			foreach (TimeInterval item in collection)
				AddCore(item);
		}
		protected override int AddCore(TimeInterval interval) {
			InnerList.Add(interval);
			return Count - 1;
		}
		protected override void OnInsertComplete(int index, TimeInterval value) {
			if (KeepSorted) {
				TimeIntervalStartComparer comparer = new TimeIntervalStartComparer();
				Sort(comparer);
			}
			base.OnInsertComplete(index, value);
		}
		internal void SuspendSort() {
			KeepSorted = false;
		}
		internal void ResumeSort() {
			KeepSorted = true;
			TimeIntervalStartComparer comparer = new TimeIntervalStartComparer();
			Sort(comparer);
		}
		protected internal virtual void ShiftCore(TimeSpan offset) {
			int count = Count;
			for (int i = 0; i < count; i++) {
				TimeInterval clone = this[i].Clone();
				clone.Start += offset;
				InnerList[i] = clone;
			}
		}
		public virtual void Shift(TimeSpan offset) {
			if (offset.Ticks == 0 || this.Count <= 0)
				return;
			OnInsert(-1, null);
			ShiftCore(offset);
			OnInsertComplete(-1, null);
		}
		#region ICloneable implementation
		object ICloneable.Clone() {
			return CloneCore();
		}
		public TimeIntervalCollection Clone() {
			return CloneCore();
		}
		internal TimeIntervalCollection CloneCore() {
			TimeIntervalCollection clone = CreateEmptyClone();
			CloneContent(clone);
			return clone;
		}
		protected internal virtual TimeIntervalCollection CreateEmptyClone() {
			return new TimeIntervalCollection();
		}
		protected internal virtual void CloneContent(TimeIntervalCollection target) {
			int count = Count;
			for (int i = 0; i < count; i++)
				target.InnerList.Add(this[i].Clone());
		}
		#endregion
		protected internal virtual DateTime CalculateEnd() {
			int count = Count;
			if (count <= 0)
				return DateTime.MinValue;
			long maxEndTicks = this[0].End.Ticks;
			for (int i = 1; i < count; i++)
				maxEndTicks = Math.Max(this[i].End.Ticks, maxEndTicks);
			return new DateTime(maxEndTicks);
		}
		protected internal virtual bool IsContinuous() {
			int count = this.Count;
			if (count <= 1)
				return true;
			DateTime prevEnd = this[0].End;
			for (int i = 1; i < count; i++) {
				TimeInterval interval = this[i];
				if (interval.Start != prevEnd)
					return false;
				prevEnd = interval.End;
			}
			return true;
		}
		protected internal virtual void AssignProperties(TimeIntervalCollection timeIntervals) {
		}
	}
	#endregion
	#region TimeIntervalCollectionEx
	public class TimeIntervalCollectionEx : TimeIntervalCollection {
		protected override int AddCore(TimeInterval interval) {
			if (Contains(interval))
				return 0;
			List<TimeInterval> toRemove = new List<TimeInterval>();
			int count = Count;
			for (int i = 0; i < count; i++) {
				if (interval.IntersectsWith(this[i])) {
					interval = TimeInterval.Union(this[i], interval);
					toRemove.Add((TimeInterval)InnerList[i]);
				}
			}
			RemoveCore(toRemove);
			InnerList.Add(interval);
			return Count - 1;
		}
		void RemoveCore(List<TimeInterval> toRemove) {
			int count = toRemove.Count;
			for (int i = 0; i < count; i++)
				InnerList.Remove(toRemove[i]);
		}
		void AddCore(List<TimeInterval> toAdd) {
			int count = toAdd.Count;
			for (int i = 0; i < count; i++)
				InnerList.Add(toAdd[i]);
		}
		public override bool Remove(TimeInterval interval) {
			if (interval == null)
				return false;
			int index = InnerList.IndexOf(interval);
			if (index >= 0) {
				OnRemove(index, interval);
				InnerList.RemoveAt(index);
				OnRemoveComplete(index, interval);
				return true;
			}
			OnInsert(-1, null);
			List<TimeInterval> toRemove = new List<TimeInterval>();
			List<TimeInterval> toAdd = new List<TimeInterval>();
			int count = Count;
			for (int i = 0; i < count; i++) {
				if (interval.IntersectsWithExcludingBounds(this[i])) {
					toRemove.Add(this[i]);
					TimeIntervalCollection subtractResult = this[i].Subtract(interval);
					int subtractResultCount = subtractResult.Count;
					for (int subtractResultIndex = 0; subtractResultIndex < subtractResultCount; subtractResultIndex++)
						toAdd.Add(subtractResult[subtractResultIndex]);
				}
			}
			RemoveCore(toRemove);
			AddCore(toAdd);
			OnInsertComplete(-1, null);
			return true;
		}
		public override bool Contains(TimeInterval interval) {
			if (InnerList.Contains(interval))
				return true;
			int count = Count;
			for (int i = 0; i < count; i++)
				if (this[i].Contains(interval))
					return true;
			return false;
		}
		protected internal override TimeIntervalCollection CreateEmptyClone() {
			return new TimeIntervalCollectionEx();
		}
		protected internal override DateTime CalculateEnd() {
			return Count > 0 ? this[Count - 1].End : DateTime.MinValue;
		}
	}
	#endregion
	#region DiscreteIntervalCollection
	public abstract class DiscreteIntervalCollection : TimeIntervalCollection {
		protected internal abstract DateTime RoundToStart(TimeInterval interval);
		protected internal abstract DateTime RoundToEnd(TimeInterval interval);
		protected internal abstract List<TimeInterval> SplitByDiscreteIntervals(TimeInterval interval);
		public override bool Contains(TimeInterval interval) {
			if (interval == null)
				return false;
			List<TimeInterval> intervals = SplitByDiscreteIntervals(interval);
			int count = intervals.Count;
			for (int i = 0; i < count; i++) {
				if (!base.Contains(intervals[i]))
					return false;
			}
			return count > 0;
		}
		protected override int AddCore(TimeInterval interval) {
			if (interval == null)
				return -1;
			List<TimeInterval> intervals = SplitByDiscreteIntervals(interval);
			int count = intervals.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval tm = intervals[i];
				if (!InnerList.Contains(tm))
					InnerList.Add(tm);
			}
			return Count - 1;
		}
		public override bool Remove(TimeInterval interval) {
			if (interval == null)
				return false;
			bool wasChanged = false;
			try {
				int count = Count;
				for (int i = count - 1; i >= 0; i--) {
					if (interval.Contains(this[i])) {
						if (!wasChanged) {
							OnCollectionChanging(new CollectionChangingEventArgs<TimeInterval>(CollectionChangedAction.Remove, null));
							wasChanged = true;
						}
						InnerList.RemoveAt(i);
					}
				}
			} finally {
				if (wasChanged)
					OnCollectionChanged(new CollectionChangedEventArgs<TimeInterval>(CollectionChangedAction.Remove, null));
			}
			return wasChanged;
		}
		protected internal override void ShiftCore(TimeSpan offset) {
			if (offset.Ticks == 0 || Count == 0)
				return;
			TimeInterval interval;
			if (offset.Ticks < 0)
				interval = new TimeInterval(CalculateDescreteShiftStart(Start + offset), Start);
			else
				interval = new TimeInterval(End, CalculateDescreteShiftEnd(End + offset));
			List<TimeInterval> intervals = SplitByDiscreteIntervals(interval);
			int count = Math.Min(intervals.Count, Count);
			for (int i = 0; i < count; i++) {
				if (offset.Ticks < 0)
					InsertReplaceInterval(intervals[i]);
				else
					AddReplaceInterval(intervals[intervals.Count - i - 1]);
			}
		}
		protected virtual DateTime CalculateDescreteShiftStart(DateTime start) {
			return start;
		}
		protected virtual DateTime CalculateDescreteShiftEnd(DateTime end) {
			return end;
		}
		void InsertReplaceInterval(TimeInterval interval) {
			InnerList.RemoveAt(Count - 1);
			InnerList.Insert(0, interval);
		}
		void AddReplaceInterval(TimeInterval interval) {
			InnerList.RemoveAt(0);
			InnerList.Add(interval);
		}
		protected internal override DateTime CalculateEnd() {
			return Count > 0 ? this[Count - 1].End : DateTime.MinValue;
		}
	}
	#endregion
	#region FixedDurationIntervalCollection (abstract class)
	public abstract class FixedDurationIntervalCollection : DiscreteIntervalCollection {
		TimeSpan duration;
		protected FixedDurationIntervalCollection(TimeSpan duration) {
			this.duration = duration;
		}
		protected internal override List<TimeInterval> SplitByDiscreteIntervals(TimeInterval interval) {
			List<TimeInterval> result = new List<TimeInterval>();
			DateTime date = RoundToStart(interval);
			DateTime end = RoundToEnd(interval);
			do {
				result.Add(new TimeInterval(date, duration));
				date += duration;
			}
			while (date < end);
			return result;
		}
	}
	#endregion
	#region DayIntervalCollection
	public class DayIntervalCollection : FixedDurationIntervalCollection {
		public DayIntervalCollection()
			: base(DateTimeHelper.DaySpan) {
		}
		protected internal override DateTime RoundToStart(TimeInterval interval) {
			return interval.Start.Date;
		}
		protected internal override DateTime RoundToEnd(TimeInterval interval) {
			return DateTimeHelper.Ceil(interval.End, DateTimeHelper.DaySpan);
		}
		protected internal override TimeIntervalCollection CreateEmptyClone() {
			return new DayIntervalCollection();
		}
	}
	#endregion
	#region WeekIntervalCollection
	public class WeekIntervalCollection : FixedDurationIntervalCollection {
		DayOfWeek firstDayOfWeek = DayOfWeek.Sunday;
		bool compressWeekEnd;
		public WeekIntervalCollection()
			: base(DateTimeHelper.WeekSpan) {
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("WeekIntervalCollectionFirstDayOfWeek")]
#endif
		public DayOfWeek FirstDayOfWeek { get { return firstDayOfWeek; } set { firstDayOfWeek = value; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("WeekIntervalCollectionCompressWeekend")]
#endif
		public bool CompressWeekend { get { return compressWeekEnd; } set { compressWeekEnd = value; } }
		protected internal override DateTime RoundToStart(TimeInterval interval) {
			return DateTimeHelper.GetStartOfWeekUI(interval.Start.Date, FirstDayOfWeek, CompressWeekend);
		}
		protected internal override DateTime RoundToEnd(TimeInterval interval) {
			DateTime end = DateTimeHelper.GetStartOfWeekUI(interval.End.Date, FirstDayOfWeek, CompressWeekend);
			if (end != interval.End.Date && (DateTime.MaxValue - end) > DateTimeHelper.WeekSpan)
				end += DateTimeHelper.WeekSpan;
			return end;
		}
		protected internal override TimeIntervalCollection CreateEmptyClone() {
			WeekIntervalCollection clone = new WeekIntervalCollection();
			clone.CompressWeekend = this.CompressWeekend;
			clone.FirstDayOfWeek = this.FirstDayOfWeek;
			return clone;
		}
		protected internal override void AssignProperties(TimeIntervalCollection timeIntervals) {
			WeekIntervalCollection weekIntervals = timeIntervals as WeekIntervalCollection;
			if (weekIntervals == null)
				return;
			CompressWeekend = weekIntervals.CompressWeekend;
			FirstDayOfWeek = weekIntervals.FirstDayOfWeek;
		}
	}
	#endregion
	#region MonthIntervalCollection
	public class MonthIntervalCollection : DiscreteIntervalCollection {
		protected internal override DateTime RoundToStart(TimeInterval interval) {
			DateTime date = interval.Start.Date;
			return new DateTime(date.Year, date.Month, 1);
		}
		protected internal override DateTime RoundToEnd(TimeInterval interval) {
			DateTime date = interval.End.Date;
			date = new DateTime(date.Year, date.Month, 1);
			if (interval.End.Date != date ||
				(interval.End.Date == date && interval.End.Date == interval.Start.Date))
				date = date.AddMonths(1);
			return date;
		}
		protected internal override List<TimeInterval> SplitByDiscreteIntervals(TimeInterval interval) {
			List<TimeInterval> result = new List<TimeInterval>();
			DateTime date = RoundToStart(interval);
			DateTime end = RoundToEnd(interval);
			do {
				result.Add(new TimeInterval(date, date.AddMonths(1)));
				date = date.AddMonths(1);
			}
			while (date < end);
			return result;
		}
		protected internal override TimeIntervalCollection CreateEmptyClone() {
			return new MonthIntervalCollection();
		}
	}
	#endregion
	#region TimeScaleIntervalCollection
	public class TimeScaleIntervalCollection : DiscreteIntervalCollection {
		TimeScale scale;
		public TimeScaleIntervalCollection(TimeScale scale) {
			this.scale = scale;
		}
		protected internal TimeScale Scale { get { return scale; } internal set { scale = value; } }
		protected internal override List<TimeInterval> SplitByDiscreteIntervals(TimeInterval interval) {
			List<TimeInterval> result = new List<TimeInterval>();
			DateTime date = RoundToStart(interval);
			DateTime end = RoundToEnd(interval);
			do {
				DateTime nextDate = Scale.GetNextDate(date);
				result.Add(new TimeInterval(date, nextDate));
				date = nextDate;
			}
			while (date < end);
			return result;
		}
		protected override DateTime CalculateDescreteShiftStart(DateTime start) {
			return Scale.Ceil(start);
		}
		protected override DateTime CalculateDescreteShiftEnd(DateTime end) {
			return Scale.Floor(end);
		}
		protected internal override DateTime RoundToStart(TimeInterval interval) {
			return Scale.Floor(interval.Start);
		}
		protected internal override DateTime RoundToEnd(TimeInterval interval) {
			DateTime dateTime = Scale.Floor(interval.End);
			return (dateTime < interval.End) ? Scale.GetNextDate(dateTime) : dateTime;
		}
		protected internal override TimeIntervalCollection CreateEmptyClone() {
			return new TimeScaleIntervalCollection(scale);
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region NotificationTimeInterval
	public class NotificationTimeInterval : TimeInterval, ISupportObjectChanged {
		public static readonly DateTime MinDateTime = new DateTime(1753, 1, 1);
		public static readonly DateTime MaxDateTime = new DateTime(9998, 12, 31);
		public static new NotificationTimeInterval Empty { get { return new NotificationTimeInterval(DateTime.MinValue, TimeSpan.Zero); } }
		public static NotificationTimeInterval FullRange { get { return new NotificationTimeInterval(MinDateTime, MaxDateTime); } }
		public NotificationTimeInterval(DateTime start, TimeSpan duration)
			: base(start, duration) {
		}
		public NotificationTimeInterval(DateTime start, DateTime end)
			: base(start, end) {
		}
		#region ISupportObjectChanged implementation
		EventHandler onChanged;
		public event EventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal override void RaiseChanged() {
			if (onChanged != null) onChanged(this, EventArgs.Empty);
		}
		#endregion
		[DefaultValue(false), Browsable(false), XtraSerializableProperty(XtraSerializationFlags.None)]
		public override bool AllDay { get { return base.AllDay; } set { } }
		[Browsable(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public override TimeSpan Duration { get { return base.Duration; } set { base.Duration = value; } }
		protected internal override bool ShouldSerializeStart() { return Start != MinDateTime; }
		protected internal override bool ShouldSerializeDuration() { return Duration != MaxDateTime - MinDateTime; }
		public override string ToString() {
			if (FullRange.Equals(this))
				return Design.DesignSR.NoneString;
			return String.Format(CultureInfo.CurrentCulture, "({0})-({1})", Start.ToString(), End.ToString());
		}
	}
	#endregion
	public static class TimeOfDayIntervalValidators {
		internal const string IntervalDurationMustBeLessThanDay = "The time interval must be less than or equal to one day.";
		public static void AttachIntervalMaxDurationValidator(TimeOfDayInterval interval, TimeSpan maxDuration) {
			new TimeIntervalDurationValidator(interval, maxDuration);
		}
		public static void ValidateMaxDuration(string propertyName, TimeOfDayInterval interval, TimeSpan maxDuration) {
			if (!TimeOfDayIntervalValidators.IsLessThanDuration(interval, maxDuration))
				Exceptions.ThrowArgumentOutOfRangeException(propertyName, TimeOfDayIntervalValidators.IntervalDurationMustBeLessThanDay);
		}
		public static bool IsLessThanDuration(TimeOfDayInterval interval, TimeSpan maxDuration) {
			return IsLessThanDuration(interval.Start, interval.End, maxDuration);
		}
		public static bool IsLessThanDuration(TimeSpan start, TimeSpan end, TimeSpan maxDuration) {
			return end - start <= maxDuration;
		}
	}
	public class TimeIntervalDurationValidator {
		public TimeIntervalDurationValidator(TimeOfDayInterval interval, TimeSpan maxDuration) {
			Interval = interval;
			MaxDuration = maxDuration;
			Interval.Changing += OnIntervalChanging;
		}
		TimeOfDayInterval Interval { get; set; }
		TimeSpan MaxDuration { get; set; }
		void OnIntervalChanging(object sender, CancellablePropertyChangingEventArgs e) {
			TimeSpan start = Interval.Start;
			TimeSpan end = Interval.End;
			string propertyName = e.PropertyName;
			TimeSpan newValue = (TimeSpan)e.NewValue;
			if (propertyName == "Start")
				start = newValue;
			else if (propertyName == "End")
				end = newValue;
			else if (propertyName == "Duration")
				end = start + newValue;
			if (!TimeOfDayIntervalValidators.IsLessThanDuration(start, end, MaxDuration))
				Exceptions.ThrowArgumentOutOfRangeException(propertyName, TimeOfDayIntervalValidators.IntervalDurationMustBeLessThanDay);
		}
	}
}
namespace DevExpress.XtraScheduler.Design {
#if !SL
	public class TimeSpanIntervalConverter<T> : ExpandableObjectConverter where T : TimeOfDayInterval {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		[System.Security.SecuritySafeCritical]
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				T interval = value as T;
				if (interval != null) {
					ConstructorInfo ctor = typeof(T).GetConstructor(
							  new Type[] { typeof(TimeSpan), typeof(TimeSpan) });
					if (ctor != null)
						return new InstanceDescriptor(ctor, new object[] { interval.Start, interval.End });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}	
#endif
}
