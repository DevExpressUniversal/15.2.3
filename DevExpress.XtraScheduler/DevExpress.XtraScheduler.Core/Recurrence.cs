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
using System.ComponentModel;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Xml;
using System.Collections.Generic;
using System.Collections;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using System.Xml.Serialization;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler {
	#region FirstDayOfWeek
	public enum FirstDayOfWeek {
		System = 7,
		Sunday = DayOfWeek.Sunday,
		Monday = DayOfWeek.Monday,
		Tuesday = DayOfWeek.Tuesday,
		Wednesday = DayOfWeek.Wednesday,
		Thursday = DayOfWeek.Thursday,
		Friday = DayOfWeek.Friday,
		Saturday = DayOfWeek.Saturday
	}
	#endregion
	#region WeekDays
	[Flags,
	Serializable,
	]
	public enum WeekDays {
		Sunday = 1,
		Monday = 2,
		Tuesday = 4,
		Wednesday = 8,
		Thursday = 16,
		Friday = 32,
		Saturday = 64,
		WeekendDays = Sunday | Saturday,
		WorkDays = Monday | Tuesday | Wednesday | Thursday | Friday,
		EveryDay = WeekendDays | WorkDays
	}
	#endregion
	#region WeekOfMonth
	public enum WeekOfMonth {
		None = 0,
		First = 1,
		Second = 2,
		Third = 3,
		Fourth = 4,
		Last = 5
	}
	#endregion
	#region RecurrenceType
	public enum RecurrenceType {
		Daily = 0,
		Weekly = 1,
		Monthly = 2,
		Yearly = 3,
		Minutely = 4,
		Hourly = 5,
	}
	#endregion
	#region RecurrenceRange
	public enum RecurrenceRange {
		NoEndDate = 0,
		OccurrenceCount = 1,
		EndByDate = 2
	}
	#endregion
	public interface IRecurrenceInfo : IBatchUpdateable, INotifyPropertyChanged {
		object Id { get; }
		string TimeZoneId { get; }
		DateTime Start { get; set; }
		DateTime End { get; set; }
		TimeSpan Duration { get; set; }
		bool AllDay { get; set; }
		RecurrenceType Type { get; set; }
		RecurrenceRange Range { get; set; }
		int OccurrenceCount { get; set; }
		int DayNumber { get; set; }
		DayOfWeek FirstDayOfWeek { get; set; }
		int Month { get; set; }
		int Periodicity { get; set; }
		WeekDays WeekDays { get; set; }
		WeekOfMonth WeekOfMonth { get; set; }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new bool IsUpdateLocked { get; }
		void Assign(IRecurrenceInfo src, bool assignId);
		void Assign(IRecurrenceInfo src);
		void Reset(RecurrenceType type);
		void FromXml(string val);
		string ToXml();
	}
	public interface IIdProvider {
		void SetId(object id);
	}
	#region RecurrenceInfo
	public class RecurrenceInfo : IRecurrenceInfo, IIdProvider, IInternalRecurrenceInfo, IBatchUpdateable, IBatchUpdateHandler, INotifyPropertyChanged {
		#region static
		public static string GetDescription(Appointment apt, DayOfWeek firstDayOfWeek) {
			return GetDescription(apt, null, firstDayOfWeek);
		}
		internal static string GetDescription(Appointment apt, TimeZoneHelper timeZoneEngine, DayOfWeek firstDayOfWeek) {
			RecurrenceDescriptionBuilder builder = RecurrenceDescriptionBuilder.CreateInstance(apt, timeZoneEngine, firstDayOfWeek);
			return builder.BuildString();
		}
		#endregion
		#region Fields
		protected internal const int DefaultPeriodicity = 1;
		protected internal const WeekOfMonth DefaultWeekOfMonth = WeekOfMonth.First;
		protected internal const WeekDays DefaultWeekDays = WeekDays.EveryDay;
		protected internal const int DefaultOccurrenceCount = SchedulerControlCompatibility.DefaultOccurrenceCountValue;
		protected internal const RecurrenceType DefaultRecurrenceType = RecurrenceType.Daily;
		protected internal const RecurrenceRange DefaultRecurrenceRange = RecurrenceRange.NoEndDate;
		protected internal const DayOfWeek DefaultFirstDayOfWeek = DayOfWeek.Monday;
		BatchUpdateHelper batchUpdateHelper;
		DayOfWeek firstDayOfWeek = DayOfWeek.Monday;
		RecurrenceRange range = DefaultRecurrenceRange;
		int occurrenceCount = 1;
		RecurrenceType type = DefaultRecurrenceType;
		int periodicity = DefaultPeriodicity;
		int dayNumber;
		WeekDays weekDays = DefaultWeekDays;
		WeekOfMonth weekOfMonth = DefaultWeekOfMonth;
		int month;
		TimeInterval interval;
		object id = Guid.NewGuid();
		#endregion
		#region Constructors
		public RecurrenceInfo() {
			this.interval = new TimeInterval();
			this.occurrenceCount = 1;
			Initialize();
		}
		public RecurrenceInfo(DateTime start) {
			this.interval = new TimeInterval(start, start);
			this.occurrenceCount = 1;
			Initialize();
		}
		public RecurrenceInfo(DateTime start, int occurrenceCount) {
			this.interval = new TimeInterval(start, start);
			this.range = RecurrenceRange.OccurrenceCount;
			this.SetOccurrenceCount(occurrenceCount);
			Initialize();
		}
		public RecurrenceInfo(DateTime start, TimeSpan duration) {
			this.interval = new TimeInterval(start, duration);
			this.range = RecurrenceRange.EndByDate;
			this.occurrenceCount = 1;
			Initialize();
		}
		public RecurrenceInfo(DateTime start, DateTime end) {
			this.interval = new TimeInterval(start, end);
			this.range = RecurrenceRange.EndByDate;
			this.occurrenceCount = 1;
			Initialize();
		}
		#endregion
		#region Properties
		#region TimeZoneId
		string timeZoneId;
		[XmlIgnoreAttribute]
		public string TimeZoneId {
			get { return timeZoneId; }			
		}
		#endregion
		#region FirstDayOfWeek
		public DayOfWeek FirstDayOfWeek {
			get { return firstDayOfWeek; }
			set {
				DayOfWeek oldValue = firstDayOfWeek;
				if ( oldValue == value )
					return;
				firstDayOfWeek = value;
				if ( OnChanging() )
					OnChanged("FirstDayOfWeek", oldValue, value);
				else
					firstDayOfWeek = oldValue;
			}
		}
		#endregion
		#region Start
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("RecurrenceInfoStart")]
#endif
		public DateTime Start {
			get { return interval.Start; }
			set {
				if ( interval.Start == value )
					return;
				DateTime oldValue = interval.Start;
				DateTime oldEnd = interval.End;
				interval.Start = value;
				if (value < oldEnd)
					interval.End = oldEnd;
				if ( OnChanging() )
					OnChanged("Start", oldValue, value);
				else
					interval.Start = oldValue;
			}
		}
		#endregion
		#region End
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("RecurrenceInfoEnd")]
#endif
		public DateTime End {
			get { return interval.End; }
			set {
				if ( interval.End == value )
					return;
				DateTime oldValue = interval.End;
				interval.End = value;
				if ( OnChanging() )
					OnChanged("End", oldValue, value);
				else
					interval.End = oldValue;
			}
		}
		#endregion
		#region Duration
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("RecurrenceInfoDuration")]
#endif
		public TimeSpan Duration {
			get { return interval.Duration; }
			set {
				if ( interval.Duration == value )
					return;
				TimeSpan oldValue = interval.Duration;
				interval.Duration = value;
				if ( OnChanging() )
					OnChanged("Duration", oldValue, value);
				else
					interval.Duration = oldValue;
			}
		}
		#endregion
		#region AllDay
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("RecurrenceInfoAllDay")]
#endif
		public bool AllDay {
			get { return interval.AllDay; }
			set {
				if ( interval.AllDay == value )
					return;
				interval.AllDay = value;
				if ( OnChanging() )
					OnChanged("AllDay", !value, value);
				else
					interval.AllDay = !value;
			}
		}
		#endregion
		#region Type
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("RecurrenceInfoType")]
#endif
		public RecurrenceType Type {
			get { return type; }
			set {
				if ( type == value )
					return;
				RecurrenceType oldValue = type;
				WeekDays oldWeekDays = weekDays;
				type = value;
				this.weekDays = (type == RecurrenceType.Daily) ? WeekDays.EveryDay : DateTimeHelper.ToWeekDays(Start.DayOfWeek);
				if ( OnChanging() )
					OnChanged("Type", oldValue, value);
				else {
					type = oldValue;
					weekDays = oldWeekDays;
				}
			}
		}
		#endregion
		#region Range
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("RecurrenceInfoRange")]
#endif
		public RecurrenceRange Range {
			get { return range; }
			set {
				if ( range == value )
					return;
				RecurrenceRange oldValue = range;
				range = value;
				if ( OnChanging() )
					OnChanged("Range", oldValue, value);
				else
					range = oldValue;
			}
		}
		#endregion
		#region Periodicity
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("RecurrenceInfoPeriodicity")]
#endif
		public int Periodicity {
			get { return periodicity; }
			set {
				value = Math.Max(value, 1);
				if ( periodicity == value )
					return;
				int oldValue = periodicity;
				periodicity = value;
				if ( OnChanging() ) {
					OnChanged("Periodicity", oldValue, value);
					NotifyPropertyChanged("Periodicity");
				} else
					periodicity = oldValue;
			}
		}
		#endregion
		#region WeekDays
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("RecurrenceInfoWeekDays")]
#endif
		public WeekDays WeekDays {
			get { return weekDays; }
			set {
				if ( weekDays == value )
					return;
				if ( !IsWeekDaysValueValid(Type, value) )
					throw new ArgumentException(SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidDayOfWeekForDailyRecurrence));
				WeekDays oldValue = weekDays;
				weekDays = value;
				if ( OnChanging() ) {
					OnChanged("WeekDays", oldValue, value);
					NotifyPropertyChanged("WeekDays");
				} else
					weekDays = oldValue;
			}
		}
		#endregion
		#region DayNumber
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("RecurrenceInfoDayNumber")]
#endif
		public int DayNumber {
			get { return dayNumber; }
			set {
				value = Math.Max(Math.Min(31, value), 1);
				if ( dayNumber == value )
					return;
				int oldValue = dayNumber;
				dayNumber = value;
				if ( OnChanging() ) {
					OnChanged("DayNumber", oldValue, value);
					NotifyPropertyChanged("DayNumber");
				} else
					dayNumber = oldValue;
			}
		}
		#endregion
		#region WeekOfMonth
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("RecurrenceInfoWeekOfMonth")]
#endif
		public WeekOfMonth WeekOfMonth {
			get { return weekOfMonth; }
			set {
				if ( weekOfMonth == value )
					return;
				WeekOfMonth oldValue = weekOfMonth;
				weekOfMonth = value;
				if ( OnChanging() ) {
					OnChanged("WeekOfMonth", oldValue, value);
					NotifyPropertyChanged("WeekOfMonth");
				} else
					weekOfMonth = oldValue;
			}
		}
		#endregion
		#region Month
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("RecurrenceInfoMonth")]
#endif
		public int Month {
			get { return month; }
			set {
				value = Math.Min(Math.Max(value, 1), 12);
				if ( month == value )
					return;
				int oldValue = month;
				month = value;
				if ( OnChanging() )
					OnChanged("Month", oldValue, value);
				else
					month = oldValue;
			}
		}
		#endregion
		#region OccurrenceCount
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("RecurrenceInfoOccurrenceCount")]
#endif
		public int OccurrenceCount {
			get { return occurrenceCount; }
			set {
				if ( occurrenceCount == value )
					return;
				int oldValue = occurrenceCount;
				SetOccurrenceCount(value);
				if ( OnChanging() )
					OnChanged("OccurrenceCount", oldValue, value);
				else
					SetOccurrenceCount(oldValue);
			}
		}
		#endregion
		#region Id
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("RecurrenceInfoId")]
#endif
		public object Id { get { return id; } }
		#endregion
		#endregion
		#region Events
		#region Changing
		CancelEventHandler onChanging;
		event CancelEventHandler IInternalRecurrenceInfo.Changing {
			add { onChanging += value; }
			remove { onChanging -= value; }
		}
		protected internal virtual bool RaiseChanging() {
			if ( onChanging != null ) {
				CancelEventArgs args = new CancelEventArgs();
				onChanging(this, args);
				return !args.Cancel;
			} else
				return true;
		}
		#endregion
		#region Changed
		EventHandler onChanged;
		event EventHandler IInternalRecurrenceInfo.Changed {
			add { onChanged += value; }
			remove { onChanged -= value; }
		}
		protected internal virtual void RaiseChanged() {
			if ( onChanged != null )
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			onContentChangingWasRaised = false;
			changeCancelled = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if ( onContentChangingWasRaised && !changeCancelled )
				OnChanged(String.Empty, String.Empty, String.Empty);
			if ( changedProperties.Count > 0 ) {
				int count = changedProperties.Count;
				for ( int i = 0; i < count; i++ )
					NotifyPropertyChanged(changedProperties[i]);
				changedProperties.Clear();
			}
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
		bool onContentChangingWasRaised;
		bool changeCancelled;
		protected internal virtual bool OnChanging() {
			if ( IsUpdateLocked ) {
				if ( !onContentChangingWasRaised ) {
					if ( !RaiseChanging() )
						changeCancelled = true;
					onContentChangingWasRaised = true;
				}
				return !changeCancelled;
			}
			return RaiseChanging();
		}
		List<String> changedProperties = new List<string>();
		protected internal virtual void OnChanged<T>(string propertyName, T oldValue, T newValue) {
			if ( !IsUpdateLocked )
				RaiseChanged();
			if ( !IsUpdateLocked )
				NotifyPropertyChanged(propertyName);
			else {
				if ( !changedProperties.Contains(propertyName) )
					changedProperties.Add(propertyName);
			}
		}
		internal static bool IsWeekDaysValueValid(RecurrenceType recurrenceType, WeekDays val) {
			if ( recurrenceType == RecurrenceType.Daily )
				return val == WeekDays.EveryDay || val == WeekDays.WeekendDays || val == WeekDays.WorkDays;
			else
				return true;
		}
		void Initialize() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.month = Start.Month;
			this.dayNumber = Start.Day;
		}
		void IIdProvider.SetId(object id) {
			this.id = id;
		}
		void IInternalRecurrenceInfo.SetWeekDaysWithoutValidation(WeekDays weekDays) {
			SetWeekDays(weekDays);
		}
		protected internal void SetWeekDays(WeekDays weekDays) {
			this.weekDays = weekDays;
		}
		void IInternalRecurrenceInfo.SetTimeZoneId(string value) {
			if (this.timeZoneId == value)
				return;
			string oldValue = this.timeZoneId;
			this.timeZoneId = value;
			if (OnChanging())
				OnChanged("TimeZoneId", oldValue, value);
			else
				timeZoneId = oldValue;
		}
		internal void SetTimeZoneId(string value) {
			((IInternalRecurrenceInfo)this).SetTimeZoneId(value);
		}
		void IInternalRecurrenceInfo.SetTypeWithoutValidation(RecurrenceType type) {
			SetType(type);
		}
		protected internal void SetType(RecurrenceType type) {
			this.type = type;
		}
		void SetOccurrenceCount(int val) {
			if ( val <= 0 )
				Exceptions.ThrowArgumentException("OccurrenceCount", val);
			occurrenceCount = val;
		}
		public void Assign(IRecurrenceInfo src) {
			Assign(src, false);
		}
		public void Assign(IRecurrenceInfo src, bool assignId) {
			BeginUpdate();
			try {
				this.FirstDayOfWeek = src.FirstDayOfWeek;
				this.DayNumber = src.DayNumber;
				this.WeekOfMonth = src.WeekOfMonth;
				this.Periodicity = src.Periodicity;
				this.Month = src.Month;
				this.OccurrenceCount = src.OccurrenceCount;
				this.Range = src.Range;
				this.Type = src.Type;
				this.WeekDays = src.WeekDays;
				this.AllDay = src.AllDay;
				this.Start = src.Start;
				this.Duration = src.Duration;
				if ( assignId )
					((IIdProvider)this).SetId(src.Id);
			} finally {
				EndUpdate();
			}
		}
		public void Reset(RecurrenceType type) {
			BeginUpdate();
			try {
				Type = type;
				switch ( type ) {
					case RecurrenceType.Monthly:
						WeekOfMonth = WeekOfMonth.None;
						DayNumber = Start.Day;
						break;
					case RecurrenceType.Yearly:
						WeekOfMonth = WeekOfMonth.None;
						DayNumber = Start.Day;
						Month = Start.Month;
						WeekDays = DateTimeHelper.ToWeekDays(Start.DayOfWeek);
						break;
				}
				Periodicity = 1;
			} finally {
				EndUpdate();
			}
		}
		public void FromXml(string val) {
			if (String.IsNullOrEmpty(val))
				return;
			IRecurrenceInfo rinfo = RecurrenceInfoXmlPersistenceHelper.ObjectFromXml(val);
			this.Assign((RecurrenceInfo)rinfo, true);
		}
		public string ToXml() {
			RecurrenceInfoXmlPersistenceHelper helper = new RecurrenceInfoXmlPersistenceHelper(this);
			return helper.ToXml();
		}
		void IInternalRecurrenceInfo.UpdateRange(DateTime start, DateTime end, RecurrenceRange rangeType, int occurrencesCount, Appointment pattern) {
			UpdateRangeCore(start, end, rangeType, occurrencesCount, pattern);
		}
		protected internal virtual void UpdateRangeCore(DateTime start, DateTime end, RecurrenceRange rangeType, int occurrencesCount, Appointment pattern) {
			this.BeginUpdate();
			try {
				this.Start = start;
				this.Range = rangeType;
				if ( rangeType == RecurrenceRange.EndByDate ) {
					this.End = end;
					OccurrenceCount = CalcOccurrenceCountInRangeCore(pattern, 10000);
				} else {
					this.OccurrenceCount = occurrencesCount;
					OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(this.CreateValidRecurrenceInfoCopyCore());
					try {
						this.End = calc.CalcOccurrenceStartTime(occurrencesCount - 1);
					} catch {
						this.Duration = DateTime.MaxValue - this.Start;
					}
				}
			} finally {
				this.EndUpdate();
			}
		}
		int IInternalRecurrenceInfo.CalcOccurrenceCountInRange(Appointment pattern) {
			return CalcOccurrenceCountInRangeCore(pattern, Int32.MaxValue);
		}
		protected internal int CalcOccurrenceCountInRangeCore(Appointment pattern, int maxCount) {
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(this.CreateValidRecurrenceInfoCopyCore());
			TimeInterval interval = new TimeInterval(Start, calc.GetActualRecurrenceEnd());
			return Math.Max(1, calc.CalcOccurrenceCountCore(interval, pattern, maxCount));
		}
		IRecurrenceInfo IInternalRecurrenceInfo.CreateValidRecurrenceInfoCopy() {
			return CreateValidRecurrenceInfoCopyCore();
		}
		protected internal virtual RecurrenceInfo CreateValidRecurrenceInfoCopyCore() {
			RecurrenceInfo result = new RecurrenceInfo();
			result.Assign(this);
			if ( result.WeekDays == (WeekDays)0 )
				result.WeekDays = WeekDays.Sunday;
			return result;
		}
		public bool Equals(RecurrenceInfo recurrenceInfo) {
			if ( Type != recurrenceInfo.Type )
				return false;
			if ( Start != recurrenceInfo.Start )
				return false;
			if ( Range != recurrenceInfo.Range )
				return false;
			if ( Range == RecurrenceRange.OccurrenceCount && OccurrenceCount != recurrenceInfo.OccurrenceCount )
				return false;
			if ( Range == RecurrenceRange.EndByDate && End != recurrenceInfo.End )
				return false;
			if ( Type == RecurrenceType.Daily || Type == RecurrenceType.Weekly )
				if ( WeekDays != recurrenceInfo.WeekDays || Periodicity != recurrenceInfo.Periodicity )
					return false;
			if ( Type == RecurrenceType.Monthly || Type == RecurrenceType.Yearly ) {
				if ( WeekOfMonth == WeekOfMonth.None ) {
					if ( DayNumber != recurrenceInfo.DayNumber )
						return false;
				} else
					if ( WeekDays != recurrenceInfo.WeekDays )
						return false;
			}
			if ( Type == RecurrenceType.Monthly ) {
				if ( WeekOfMonth != recurrenceInfo.WeekOfMonth || Periodicity != recurrenceInfo.Periodicity )
					return false;
			}
			if ( Type == RecurrenceType.Yearly )
				if ( WeekOfMonth != recurrenceInfo.WeekOfMonth || Month != recurrenceInfo.Month )
					return false;
			return true;
		}
		public override bool Equals(object obj) {
			RecurrenceInfo other = obj as RecurrenceInfo;
			if (other != null)
				return Equals(other);
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#region INotifyPropertyChanged Members
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("RecurrenceInfoPropertyChanged")]
#endif
		public event PropertyChangedEventHandler PropertyChanged;
		void NotifyPropertyChanged(String info) {
			if ( PropertyChanged != null )
				PropertyChanged(this, new PropertyChangedEventArgs(info));
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.UI {
	#region ValidationArgs
	public class ValidationArgs {
		#region Fields
		bool valid = true;
		string errorMessage = String.Empty;
		object control;
		#endregion
		#region Properties
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("ValidationArgsValid")]
#endif
		public bool Valid { get { return valid; } set { valid = value; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("ValidationArgsErrorMessage")]
#endif
		public string ErrorMessage { get { return errorMessage; } set { errorMessage = value; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("ValidationArgsControl")]
#endif
		public object Control { get { return control; } set { control = value; } }
		#endregion
	}
	#endregion
	public class SchedulerRecurrenceValidator {
		protected int DaysInMonth(int month) {
			return DateTime.DaysInMonth(2000, month);
		}
		public virtual bool CheckPositiveValue(object val) {
			return ControlInputHelper.CheckPositiveValue(val);
		}
		public virtual int GetIntegerValue(object val) {
			return ControlInputHelper.GetIntegerValue(val);
		}
		public virtual bool CheckDayNumber(object val, int maxValue) {
			return ControlInputHelper.CheckLimitedPositiveValue(val, maxValue);
		}
		public virtual bool CheckMonthDayNumber(object val, int month) {
			return ControlInputHelper.CheckLimitedPositiveValue(val, DaysInMonth(month));
		}
		public virtual void ValidateDayCount(ValidationArgs args, object control, object val) {
			if (!CheckPositiveValue(val)) {
				args.Valid = false;
				args.ErrorMessage = GetInvalidDayCountErrorMessage(val);
				args.Control = control;
			}
		}
		public virtual void ValidateWeekCount(ValidationArgs args, object control, object val) {
			if (!CheckPositiveValue(val)) {
				args.Valid = false;
				args.ErrorMessage = ControlInputHelper.GetPositiveValueErrorMessage(val, SchedulerStringId.Msg_InvalidWeekCount, SchedulerStringId.Msg_InvalidWeekCountValue);
				args.Control = control;
			}
		}
		public virtual void ValidateMonthCount(ValidationArgs args, object control, object val) {
			if (!CheckPositiveValue(val)) {
				args.Valid = false;
				args.ErrorMessage = GetInvalidMonthCountErrorMessage(val);
				args.Control = control;
			}
		}
		public virtual void ValidateDayOfWeek(ValidationArgs args, object control, object val) {
			if (GetIntegerValue(val) == 0) {
				args.Valid = false;
				args.ErrorMessage = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidDayOfWeek);
				args.Control = control;
			}
		}
		public virtual void ValidateDayNumber(ValidationArgs args, object control, object val, int maxValue) {
			ValidateDayNumberCore(args, control, val, maxValue);
		}
		public virtual void ValidateMonthDayNumber(ValidationArgs args, object control, object val, int month) {
			ValidateDayNumberCore(args, control, val, DaysInMonth(month));
		}
		protected virtual void ValidateDayNumberCore(ValidationArgs args, object control, object val, int maxValue) {
			if (!CheckDayNumber(val, maxValue)) {
				args.Valid = false;
				args.ErrorMessage = GetInvalidDayNumberErrorMessage(val, maxValue);
				args.Control = control;
			}
		}
		public virtual void ValidateOccurrencesCount(ValidationArgs args, object control, object val) {
			if (!CheckPositiveValue(val)) {
				args.Valid = false;
				args.ErrorMessage = GetInvalidOccurrencesCountErrorMessage(val);
				args.Control = control;
			}
		}
		public virtual string GetInvalidOccurrencesCountErrorMessage(object val) {
			return ControlInputHelper.GetPositiveValueErrorMessage(val, SchedulerStringId.Msg_InvalidOccurrencesCount, SchedulerStringId.Msg_InvalidOccurrencesCountValue);
		}
		public virtual string GetInvalidDayNumberErrorMessage(object val, int maxValue) {
			return ControlInputHelper.GetLimitedPositiveValueErrorMessage(val, maxValue, SchedulerStringId.Msg_InvalidDayNumber, SchedulerStringId.Msg_InvalidDayNumberValue);
		}
		public virtual string GetInvalidMonthDayNumberErrorMessage(object val, int month) {
			return GetInvalidDayNumberErrorMessage(val, DaysInMonth(month));
		}
		public virtual string GetInvalidDayCountErrorMessage(object val) {
			return ControlInputHelper.GetPositiveValueErrorMessage(val, SchedulerStringId.Msg_InvalidDayCount, SchedulerStringId.Msg_InvalidDayCountValue);
		}
		public virtual string GetInvalidMonthCountErrorMessage(object val) {
			return ControlInputHelper.GetPositiveValueErrorMessage(val, SchedulerStringId.Msg_InvalidMonthCount, SchedulerStringId.Msg_InvalidMonthCountValue);
		}
		public bool NeedToCheckLargeDayNumberWarning(WeekOfMonth weekOfMonth) {
			return weekOfMonth == WeekOfMonth.None;
		}
		public virtual void CheckLargeDayNumberWarning(ValidationArgs args, object control, object val) {
			int dayNumber = GetIntegerValue(val);
			if (dayNumber > 28) {
				args.Valid = false;
				args.ErrorMessage = GetInvalidLargeDayNumberWarningMessage(dayNumber);
				args.Control = control;
			}
		}
		public virtual string GetInvalidLargeDayNumberWarningMessage(int dayNumber) {
			string format = SchedulerLocalizer.GetString(SchedulerStringId.Msg_WarningDayNumber);
			return String.Format(format, dayNumber);
		}
	}
}
namespace DevExpress.XtraScheduler.Xml {
	using System.Xml;
	using DevExpress.Utils.Serializing;
	public class RecurrenceInfoXmlPersistenceHelper : XmlPersistenceHelper {
		IRecurrenceInfo recurrenceInfo;
		TimeZoneEngine timeZoneEngine;
		string patternTimeZoneId;
		public RecurrenceInfoXmlPersistenceHelper(IRecurrenceInfo recurrenceInfo)
			: this(recurrenceInfo, new TimeZoneEngine(), recurrenceInfo.TimeZoneId) {
			this.recurrenceInfo = recurrenceInfo;
		}
		public RecurrenceInfoXmlPersistenceHelper(Appointment appointment)
			: this(appointment.RecurrenceInfo, InternalAppointmentPropertyAccessor.GetTimeZoneEngine(appointment), appointment.TimeZoneId) {
		}
		internal RecurrenceInfoXmlPersistenceHelper(IRecurrenceInfo recurrenceInfo, TimeZoneEngine timeZoneEngine, string patternTimeZoneId) {
			this.recurrenceInfo = recurrenceInfo;
			this.timeZoneEngine = timeZoneEngine;
			this.patternTimeZoneId = patternTimeZoneId;			
		}
		protected internal IRecurrenceInfo RecurrenceInfo { get { return recurrenceInfo; } }
		protected internal TimeZoneEngine TimeZoneEngine { get { return timeZoneEngine; } }
		protected override IXmlContext GetXmlContext() {
			XmlContext context = new XmlContext(RecurrenceInfoSR.XmlElementName);
			DateTime start = RecurrenceInfo.Start;
			DateTime end = RecurrenceInfo.End;
			string timeZoneId = RecurrenceInfo.TimeZoneId;
			if (String.IsNullOrEmpty(timeZoneId)) {
				start = TimeZoneEngine.FromOperationTimeToDefaultAppointmentTimeZone(start);
				end = TimeZoneEngine.FromOperationTimeToDefaultAppointmentTimeZone(end);
			} else if (TimeZoneEngine != null) {
				start = TimeZoneEngine.FromOperationTime(start, timeZoneId);
				end = TimeZoneEngine.FromOperationTime(end, timeZoneId);
			}
			DateTime defaultEnd = DateTime.MinValue + TimeInterval.DefaultDuration;
			context.Attributes.Add(new DateTimeContextAttribute(RecurrenceInfoSR.Start, start, DateTime.MinValue));
			if (RecurrenceInfo.Range != RecurrenceRange.NoEndDate)			
				context.Attributes.Add(new DateTimeContextAttribute(RecurrenceInfoSR.End, end, defaultEnd));
			context.Attributes.Add(new BooleanContextAttribute(RecurrenceInfoSR.AllDay, RecurrenceInfo.AllDay, false));
			context.Attributes.Add(new IntegerContextAttribute(RecurrenceInfoSR.DayNumber, RecurrenceInfo.DayNumber, 1));
			context.Attributes.Add(new IntegerContextAttribute(RecurrenceInfoSR.WeekOfMonth, (int)RecurrenceInfo.WeekOfMonth, (int)DevExpress.XtraScheduler.RecurrenceInfo.DefaultWeekOfMonth));
			context.Attributes.Add(new IntegerContextAttribute(RecurrenceInfoSR.WeekDays, (int)RecurrenceInfo.WeekDays, (int)DevExpress.XtraScheduler.RecurrenceInfo.DefaultWeekDays));
			context.Attributes.Add(new ObjectContextAttribute(RecurrenceInfoSR.Id, RecurrenceInfo.Id, Guid.Empty));
			context.Attributes.Add(new IntegerContextAttribute(RecurrenceInfoSR.Month, RecurrenceInfo.Month, 1));
			if (RecurrenceInfo.Range != RecurrenceRange.NoEndDate)
				context.Attributes.Add(new IntegerContextAttribute(RecurrenceInfoSR.OccurrenceCount, RecurrenceInfo.OccurrenceCount, SchedulerControlCompatibility.DefaultOccurrenceCount));
			context.Attributes.Add(new IntegerContextAttribute(RecurrenceInfoSR.Periodicity, RecurrenceInfo.Periodicity, DevExpress.XtraScheduler.RecurrenceInfo.DefaultPeriodicity));
			context.Attributes.Add(new IntegerContextAttribute(RecurrenceInfoSR.Range, (int)RecurrenceInfo.Range, (int)DevExpress.XtraScheduler.RecurrenceInfo.DefaultRecurrenceRange));
			context.Attributes.Add(new IntegerContextAttribute(RecurrenceInfoSR.Type, (int)RecurrenceInfo.Type, (int)DevExpress.XtraScheduler.RecurrenceInfo.DefaultRecurrenceType));
			context.Attributes.Add(new IntegerContextAttribute(RecurrenceInfoSR.FirstDayOfWeek, (int)RecurrenceInfo.FirstDayOfWeek, (int)DevExpress.XtraScheduler.RecurrenceInfo.DefaultFirstDayOfWeek));
			context.Attributes.Add(new IntegerContextAttribute("Version", 1, 0));
			return context;
		}
		public override ObjectXmlLoader CreateObjectXmlLoader(XmlNode root) {
			return new RecurrenceInfoXmlLoader(root, TimeZoneEngine, this.patternTimeZoneId);
		}
		public static IRecurrenceInfo ObjectFromXml(string xml) {
			return ObjectFromXml(GetRootElement(xml));
		}
		internal static IRecurrenceInfo ObjectFromXml(string xml, TimeZoneEngine timeZoneEngine, string patternTimeZoneId) {
			return ObjectFromXml(GetRootElement(xml), timeZoneEngine, patternTimeZoneId);
		}
		public static IRecurrenceInfo ObjectFromXml(XmlNode root) {
			return ObjectFromXml(root, new TimeZoneEngine(), String.Empty);
		}
		internal static IRecurrenceInfo ObjectFromXml(XmlNode root, TimeZoneEngine timeZoneEngine, string patternTimeZoneId) {
			RecurrenceInfoXmlPersistenceHelper helper = new RecurrenceInfoXmlPersistenceHelper(null, timeZoneEngine, patternTimeZoneId);
			return (IRecurrenceInfo)helper.FromXmlNode(root);
		}
	}
	public class RecurrenceInfoXmlLoader : ObjectXmlLoader {
		TimeZoneEngine timeZoneEngine;
		string patternTimeZoneId;
		public RecurrenceInfoXmlLoader(XmlNode root)
			: base(root) {
			this.timeZoneEngine = new TimeZoneEngine();
		}
		internal RecurrenceInfoXmlLoader(XmlNode root, TimeZoneEngine timeZoneEngine, string patternTimeZoneId)
			: base(root) {
			this.timeZoneEngine = timeZoneEngine;
			this.patternTimeZoneId = patternTimeZoneId;
		}
		protected internal TimeZoneEngine TimeZoneEngine { get { return timeZoneEngine; } }
		public override object ObjectFromXml() {
			RecurrenceInfo ri = new RecurrenceInfo();
			ri.BeginUpdate();
			try {
				DateTime start = ReadAttributeAsDateTime(RecurrenceInfoSR.Start, DateTime.MinValue);
				DateTime end = ReadAttributeAsDateTime(RecurrenceInfoSR.End, DateTime.MinValue);
				if (String.IsNullOrEmpty(this.patternTimeZoneId)) {
					if (TimeZoneEngine != null) {
						if (start != DateTime.MinValue)
							start = TimeZoneEngine.ToOperationTimeFromDefaultAppointmentTimeZone(start);
						if (end != DateTime.MinValue)
							end = TimeZoneEngine.ToOperationTimeFromDefaultAppointmentTimeZone(end);
					}
				} else {
					ri.SetTimeZoneId(this.patternTimeZoneId);
					if (start != DateTime.MinValue)
						start = TimeZoneEngine.ToOperationTime(start, this.patternTimeZoneId);
					if (end != DateTime.MinValue)
						end = TimeZoneEngine.ToOperationTime(end, this.patternTimeZoneId);
				}
				ri.Start = start;
				if (end != DateTime.MinValue)
					ri.End = end;
				ri.AllDay = ReadAttributeAsBoolean(RecurrenceInfoSR.AllDay, false);
				ri.DayNumber = ReadAttributeAsInt(RecurrenceInfoSR.DayNumber, 1);
				ri.WeekOfMonth = (WeekOfMonth)ReadAttributeAsInt(RecurrenceInfoSR.WeekOfMonth, (int)RecurrenceInfo.DefaultWeekOfMonth);
				((IInternalRecurrenceInfo)ri).SetWeekDaysWithoutValidation((WeekDays)ReadAttributeAsInt(RecurrenceInfoSR.WeekDays, (int)RecurrenceInfo.DefaultWeekDays));
				((IIdProvider)ri).SetId(ReadAttributeAsGuid(RecurrenceInfoSR.Id, Guid.Empty));
				ri.Month = ReadAttributeAsInt(RecurrenceInfoSR.Month, 1);
				ri.OccurrenceCount = ReadAttributeAsInt(RecurrenceInfoSR.OccurrenceCount, SchedulerControlCompatibility.DefaultOccurrenceCount);
				ri.Periodicity = ReadAttributeAsInt(RecurrenceInfoSR.Periodicity, RecurrenceInfo.DefaultPeriodicity);
				ri.Range = (RecurrenceRange)ReadAttributeAsInt(RecurrenceInfoSR.Range, (int)RecurrenceInfo.DefaultRecurrenceRange);
				((IInternalRecurrenceInfo)ri).SetTypeWithoutValidation((RecurrenceType)ReadAttributeAsInt(RecurrenceInfoSR.Type, (int)RecurrenceInfo.DefaultRecurrenceType));
				ri.FirstDayOfWeek = (DayOfWeek)ReadAttributeAsInt(RecurrenceInfoSR.FirstDayOfWeek, (int)RecurrenceInfo.DefaultFirstDayOfWeek);
			} finally {
				ri.EndUpdate();
			}
			return ri;
		}
	}
	#region RecurrenceInfoContextElement
	public class RecurrenceInfoContextElement : XmlContextItem {
		TimeZoneEngine timeZoneEngine;
		public RecurrenceInfoContextElement(IRecurrenceInfo recurrenceInfo)
			: base(RecurrenceInfoSR.XmlElementName, recurrenceInfo, null) {
		}
		public RecurrenceInfoContextElement(Appointment pattern)
			: this(pattern.RecurrenceInfo) {
			this.timeZoneEngine = InternalAppointmentPropertyAccessor.GetTimeZoneEngine(pattern);
		}
		protected IRecurrenceInfo RecurrenceInfo { get { return (IRecurrenceInfo)Value; } }
		public override string ValueToString() {
			return new RecurrenceInfoXmlPersistenceHelper(RecurrenceInfo, this.timeZoneEngine ?? new TimeZoneEngine(), RecurrenceInfo.TimeZoneId).ToXml();
		}
	}
	#endregion
	#region PatternReference
	public class PatternReference {
		object id;
		int recurrenceIndex;
		public PatternReference(object id, int recurrenceIndex) {
			this.id = id;
			this.recurrenceIndex = recurrenceIndex;
		}
		public object Id { get { return id; } }
		public int RecurrenceIndex { get { return recurrenceIndex; } }
	}
	#endregion
	#region PatternReferenceContextElement
	public class PatternReferenceContextElement : XmlContextItem {
		public PatternReferenceContextElement(PatternReference patternReference)
			: base(RecurrenceInfoSR.XmlElementName, patternReference, null) {
		}
		protected PatternReference PatternReference { get { return (PatternReference)Value; } }
		public override string ValueToString() {
			return new PatternReferenceXmlPersistenceHelper(PatternReference).ToXml();
		}
	}
	#endregion
	#region PatternReferenceXmlPersistenceHelper
	public class PatternReferenceXmlPersistenceHelper : XmlPersistenceHelper {
		PatternReference patternRef;
		public PatternReferenceXmlPersistenceHelper(PatternReference patternRef) {
			this.patternRef = patternRef;
		}
		protected override IXmlContext GetXmlContext() {
			XmlContext context = new XmlContext(RecurrenceInfoSR.XmlElementName);
			context.Attributes.Add(new ObjectContextAttribute(RecurrenceInfoSR.Id, patternRef.Id, Guid.Empty));
			context.Attributes.Add(new IntegerContextAttribute(RecurrenceInfoSR.Index, patternRef.RecurrenceIndex, 0));
			return context;
		}
		public override ObjectXmlLoader CreateObjectXmlLoader(XmlNode root) {
			return new PatternReferenceXmlLoader(root);
		}
		public static PatternReference ObjectFromXml(string xml) {
			return ObjectFromXml(GetRootElement(xml));
		}
		public static PatternReference ObjectFromXml(XmlNode root) {
			PatternReferenceXmlPersistenceHelper helper = new PatternReferenceXmlPersistenceHelper(null);
			return (PatternReference)helper.FromXmlNode(root);
		}
	}
	#endregion
	#region PatternReferenceXmlLoader
	public class PatternReferenceXmlLoader : ObjectXmlLoader {
		public PatternReferenceXmlLoader(XmlNode root)
			: base(root) {
		}
		public override object ObjectFromXml() {
			return new PatternReference(ReadAttributeAsGuid(RecurrenceInfoSR.Id, Guid.Empty),
				ReadAttributeAsInt(RecurrenceInfoSR.Index, 0));
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	public static class RecurrenceInfoSR {
		public const string XmlElementName = "RecurrenceInfo";
		public const string AllDay = "AllDay";
		public const string DayNumber = "DayNumber";
		public const string WeekOfMonth = "WeekOfMonth";
		public const string WeekDays = "WeekDays";
		public const string End = "End";
		public const string Id = "Id";
		public const string Index = "Index"; 
		public const string Month = "Month";
		public const string OccurrenceCount = "OccurrenceCount";
		public const string Periodicity = "Periodicity";
		public const string Range = "Range";
		public const string Start = "Start";
		public const string Type = "Type";
		public const string FirstDayOfWeek = "FirstDayOfWeek";
		public const string TimeZoneId = "TimeZoneId";
	}
	#region RecurrenceDescriptionBuilder (abstract class)
	public abstract class RecurrenceDescriptionBuilder {
		static Hashtable displayNameHT = new Hashtable();
		static RecurrenceDescriptionBuilder() {
			displayNameHT[WeekOfMonth.First] = SchedulerStringId.Caption_WeekOfMonthFirst;
			displayNameHT[WeekOfMonth.Second] = SchedulerStringId.Caption_WeekOfMonthSecond;
			displayNameHT[WeekOfMonth.Third] = SchedulerStringId.Caption_WeekOfMonthThird;
			displayNameHT[WeekOfMonth.Fourth] = SchedulerStringId.Caption_WeekOfMonthFourth;
			displayNameHT[WeekOfMonth.Last] = SchedulerStringId.Caption_WeekOfMonthLast;
		}
		protected static string GetWeekOfMonthString(WeekOfMonth weekOfMonth) {
			if (!displayNameHT.ContainsKey(weekOfMonth))
				return String.Empty;
			return SchedulerLocalizer.GetString((SchedulerStringId)displayNameHT[weekOfMonth]);
		}
		public static RecurrenceDescriptionBuilder CreateInstance(Appointment apt, TimeZoneHelper timeZoneEngine, DayOfWeek firstDayOfWeek) {
			Appointment pattern = ObtainRecurrencePattern(apt);
			if (pattern == null)
				return new EmptyRecurrenceDescriptionBuilder();
			switch (pattern.RecurrenceInfo.Type) {
				case RecurrenceType.Daily:
					return new DailyRecurrenceDescriptionBuilder(pattern, timeZoneEngine);
				case RecurrenceType.Weekly:
					return new WeeklyRecurrenceDescriptionBuilder(pattern, timeZoneEngine, firstDayOfWeek);
				case RecurrenceType.Monthly:
					return new MonthlyRecurrenceDescriptionBuilder(pattern, timeZoneEngine);
				case RecurrenceType.Yearly:
					return new YearlyRecurrenceDescriptionBuilder(pattern, timeZoneEngine);
				case RecurrenceType.Hourly:
				case RecurrenceType.Minutely:
				default:
					return new EmptyRecurrenceDescriptionBuilder();
			}
		}
		internal static Appointment ObtainRecurrencePattern(Appointment apt) {
			if (apt == null)
				return null;
			if (apt.Type == AppointmentType.Pattern)
				return apt;
			else
				return apt.RecurrencePattern;
		}
		#region Fields
		Appointment pattern;
		IRecurrenceInfo recurrenceInfo;
		TimeZoneHelper timeZoneEngine;
		#endregion
		protected RecurrenceDescriptionBuilder() {
		}
		protected RecurrenceDescriptionBuilder(Appointment pattern, TimeZoneHelper timeZoneEngine) {
			if (pattern == null)
				Exceptions.ThrowArgumentException("pattern", pattern);
			if (pattern.RecurrenceInfo == null)
				Exceptions.ThrowArgumentException("pattern.RecurrenceInfo", pattern.RecurrenceInfo);
			this.pattern = pattern;
			this.recurrenceInfo = pattern.RecurrenceInfo;
			this.timeZoneEngine = timeZoneEngine;
		}
		#region Properties
		protected internal Appointment Pattern { get { return pattern; } }
		protected internal IRecurrenceInfo RecurrenceInfo { get { return recurrenceInfo; } }
		protected internal TimeZoneHelper TimeZoneHelper { get { return timeZoneEngine; } }
		#endregion
		public virtual string BuildString() {
			TimeInterval clientInterval = CreateClientInterval();
			string intervalString = IntervalToString(clientInterval);
			return BuildStringCore(intervalString);
		}
		protected internal abstract string BuildStringCore(string intervalString);
		protected internal virtual TimeInterval CreateClientInterval() {
			TimeInterval patternInterval = CalculatePatternInterval(Pattern);
			if (timeZoneEngine != null)
				return timeZoneEngine.ToClientTime(patternInterval, Pattern.TimeZoneId);
			else
				return patternInterval;
		}
		protected internal virtual TimeInterval CalculatePatternInterval(Appointment pattern) {
			return new TimeInterval(pattern.RecurrenceInfo.Start, pattern.Duration);
		}
		protected internal string Format(SchedulerStringId stringId, params object[] args) {
			return String.Format(SchedulerLocalizer.GetString(stringId), args);
		}
		internal string ValueToString(int num, SchedulerStringId singular, SchedulerStringId plural) {
			SchedulerStringId unitsStringId = num == 1 ? singular : plural;
			string unitsString = SchedulerLocalizer.GetString(unitsStringId);
			return Format(SchedulerStringId.TextDuration_ForPattern, num, unitsString);
		}
		protected internal virtual string IntervalToString(TimeInterval interval) {
			DateTime start = interval.Start;
			DateTime end = interval.End;
			TimeSpan duration = interval.Duration;
			string startTimeString = DateTimeFormatHelper.DateToShortTimeString(start);
			if (duration.Days >= 1) {
				string daysString = ValueToString(duration.Days, SchedulerStringId.Abbr_Day, SchedulerStringId.Abbr_Days);
				string hoursString = ValueToString(duration.Hours, SchedulerStringId.Abbr_Hour, SchedulerStringId.Abbr_Hours);
				string minutesString = ValueToString(duration.Minutes, SchedulerStringId.Abbr_Minute, SchedulerStringId.Abbr_Minutes);
				SchedulerStringId formatStringId = ChooseDurationFormatString(duration);
				return Format(formatStringId, startTimeString, daysString, hoursString, minutesString);
			} else {
				string endTimeString = DateTimeFormatHelper.DateToShortTimeString(end);
				return Format(SchedulerStringId.TextDuration_FromTo, startTimeString, endTimeString);
			}
		}
		internal SchedulerStringId ChooseDurationFormatString(TimeSpan duration) {
			if (duration.Hours != 0 && duration.Minutes == 0)
				return SchedulerStringId.TextDuration_FromForDaysHours;
			else if (duration.Hours == 0 && duration.Minutes != 0)
				return SchedulerStringId.TextDuration_FromForDaysMinutes;
			else if (duration.Hours != 0 && duration.Minutes != 0)
				return SchedulerStringId.TextDuration_FromForDaysHoursMinutes;
			else
				return SchedulerStringId.TextDuration_FromForDays;
		}
		protected internal virtual string GetDayName(DateTimeFormatInfo dtfi, WeekDays weekDays) {
			if ((weekDays & WeekDays.EveryDay) == WeekDays.EveryDay)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekDaysEveryDay);
			if ((weekDays & WeekDays.WorkDays) == WeekDays.WorkDays)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekDaysWorkDays);
			if ((weekDays & WeekDays.WeekendDays) == WeekDays.WeekendDays)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekDaysWeekendDays);
			return dtfi.GetDayName(DateTimeHelper.ToDayOfWeek(weekDays));
		}
	}
	#endregion
	#region EmptyRecurrenceDescriptionBuilder
	public class EmptyRecurrenceDescriptionBuilder : RecurrenceDescriptionBuilder {
		public override string BuildString() {
			return String.Empty;
		}
		protected internal override string BuildStringCore(string intervalString) {
			return String.Empty;
		}
	}
	#endregion
	#region DailyRecurrenceDescriptionBuilder
	public class DailyRecurrenceDescriptionBuilder : RecurrenceDescriptionBuilder {
		public DailyRecurrenceDescriptionBuilder(Appointment pattern, TimeZoneHelper timeZoneEngine)
			: base(pattern, timeZoneEngine) {
		}
		protected internal override string BuildStringCore(string intervalString) {
			SchedulerStringId formatPatternStringId = GetFormatPatternStringId();
			SchedulerStringId abbrDaysStringId = GetAbbrDaysStringId(formatPatternStringId);
			string abbrDaysString = SchedulerLocalizer.GetString(abbrDaysStringId);
			return Format(formatPatternStringId, intervalString, abbrDaysString, RecurrenceInfo.Periodicity);
		}
		protected internal virtual SchedulerStringId GetFormatPatternStringId() {
			if (RecurrenceInfo.WeekDays == WeekDays.WorkDays)
				return SchedulerStringId.TextDailyPatternString_EveryWeekDay;
			else if (RecurrenceInfo.WeekDays == WeekDays.WeekendDays)
				return SchedulerStringId.TextDailyPatternString_EveryWeekend;
			else {
				if (RecurrenceInfo.Periodicity == 1)
					return SchedulerStringId.TextDailyPatternString_EveryDay;
				else
					return SchedulerStringId.TextDailyPatternString_EveryDays;
			}
		}
		protected internal virtual SchedulerStringId GetAbbrDaysStringId(SchedulerStringId formatPatternStringId) {
			if (formatPatternStringId == SchedulerStringId.TextDailyPatternString_EveryDays)
				return SchedulerStringId.Abbr_Days;
			else
				return SchedulerStringId.Abbr_Day;
		}
	}
	#endregion
	#region WeeklyRecurrenceDescriptionBuilder
	public class WeeklyRecurrenceDescriptionBuilder : RecurrenceDescriptionBuilder {
		DayOfWeek firstDayOfWeek;
		internal static SchedulerStringId[] textId = new SchedulerStringId[] {
			SchedulerStringId.TextWeekly_0Day,
			SchedulerStringId.TextWeekly_1Day,
			SchedulerStringId.TextWeekly_2Day,
			SchedulerStringId.TextWeekly_3Day,
			SchedulerStringId.TextWeekly_4Day,
			SchedulerStringId.TextWeekly_5Day,
			SchedulerStringId.TextWeekly_6Day,
			SchedulerStringId.TextWeekly_7Day
		};
		public WeeklyRecurrenceDescriptionBuilder(Appointment pattern, TimeZoneHelper timeZoneEngine, DayOfWeek firstDayOfWeek)
			: base(pattern, timeZoneEngine) {
			this.firstDayOfWeek = firstDayOfWeek;
		}
		internal DayOfWeek FirstDayOfWeek { get { return firstDayOfWeek; } set { firstDayOfWeek = value; } }
		protected internal override string BuildStringCore(string intervalString) {
			string[] dayList = GetDayNames();
			XtraSchedulerDebug.Assert(dayList.Length == 7);
			int actualDayCount = CalcActualDayNamesCount(dayList);
			string daysNames = Format(textId[actualDayCount], dayList[0], dayList[1], dayList[2], dayList[3], dayList[4], dayList[5], dayList[6]);
			SchedulerStringId formatPatternStringId = GetFormatPatternStringId();
			SchedulerStringId abbrWeeksStringId = GetAbbrWeeksStringId(formatPatternStringId);
			string abbrWeeksString = SchedulerLocalizer.GetString(abbrWeeksStringId);
			return Format(formatPatternStringId, intervalString, RecurrenceInfo.Periodicity, abbrWeeksString, daysNames);
		}
		protected internal virtual SchedulerStringId GetFormatPatternStringId() {
			if (RecurrenceInfo.Periodicity == 1)
				return SchedulerStringId.TextWeeklyPatternString_EveryWeek;
			else
				return SchedulerStringId.TextWeeklyPatternString_EveryWeeks;
		}
		protected internal virtual SchedulerStringId GetAbbrWeeksStringId(SchedulerStringId formatPatternStringId) {
			if (formatPatternStringId == SchedulerStringId.TextWeeklyPatternString_EveryWeeks)
				return SchedulerStringId.Abbr_Weeks;
			else
				return SchedulerStringId.Abbr_Week;
		}
		protected internal virtual string[] GetDayNames() {
			DateTimeFormatInfo dtfi = DateTimeFormatInfo.CurrentInfo;
			string[] result = new string[7];
			DayOfWeek[] weekDays = DateTimeHelper.GetWeekDays(firstDayOfWeek);
			int currentDay = 0;
			for (int i = 0; i < 7; i++) {
				if ((RecurrenceInfo.WeekDays & DateTimeHelper.ToWeekDays(weekDays[i])) != 0) {
					result[currentDay] = dtfi.GetDayName(weekDays[i]);
					currentDay++;
				}
			}
			return result;
		}
		protected internal virtual int CalcActualDayNamesCount(string[] dayNames) {
			int count = dayNames.Length;
			for (int i = 0; i < count; i++)
				if (String.IsNullOrEmpty(dayNames[i]))
					return i;
			return count;
		}
	}
	#endregion
	#region MonthlyRecurrenceDescriptionBuilder
	public class MonthlyRecurrenceDescriptionBuilder : RecurrenceDescriptionBuilder {
		public MonthlyRecurrenceDescriptionBuilder(Appointment pattern, TimeZoneHelper timeZoneEngine)
			: base(pattern, timeZoneEngine) {
		}
		protected internal override string BuildStringCore(string intervalString) {
			SchedulerStringId abbrMonthsStringId = GetAbbrMonthsStringId();
			string abbrMonthsString = SchedulerLocalizer.GetString(abbrMonthsStringId);
			string subString = Format(SchedulerStringId.TextMonthlyPatternString_SubPattern, RecurrenceInfo.Periodicity, abbrMonthsString, intervalString);
			DateTimeFormatInfo dtfi = DateTimeFormatHelper.CurrentUIDateTimeFormat;
			string dayName = GetDayName(dtfi, RecurrenceInfo.WeekDays);
			SchedulerStringId formatPatternStringId = GetFormatPatternStringId();
			return Format(formatPatternStringId, subString, GetWeekOfMonthString(RecurrenceInfo.WeekOfMonth), dayName, RecurrenceInfo.DayNumber);
		}
		protected internal virtual SchedulerStringId GetAbbrMonthsStringId() {
			if (RecurrenceInfo.Periodicity == 1)
				return SchedulerStringId.Abbr_Month;
			else
				return SchedulerStringId.Abbr_Months;
		}
		protected internal virtual SchedulerStringId GetFormatPatternStringId() {
			if (RecurrenceInfo.WeekOfMonth == WeekOfMonth.None)
				return SchedulerStringId.TextMonthlyPatternString1;
			else
				return SchedulerStringId.TextMonthlyPatternString2;
		}
	}
	#endregion
	#region YearlyRecurrenceDescriptionBuilder
	public class YearlyRecurrenceDescriptionBuilder : RecurrenceDescriptionBuilder {
		public YearlyRecurrenceDescriptionBuilder(Appointment pattern, TimeZoneHelper timeZoneEngine)
			: base(pattern, timeZoneEngine) {
		}
		protected internal override string BuildStringCore(string intervalString) {
			DateTimeFormatInfo dtfi = DateTimeFormatInfo.CurrentInfo;
			string monthName = dtfi.GetMonthName(RecurrenceInfo.Month);
			SchedulerStringId abbrYearsStringId = GetAbbrYearsStringId();
			string abbrYearsString = SchedulerLocalizer.GetString(abbrYearsStringId);
			string dayName = GetDayName(dtfi, RecurrenceInfo.WeekDays);
			SchedulerStringId formatPatternStringId = GetFormatPatternStringId();
			return Format(formatPatternStringId,
					intervalString,
					RecurrenceInfo.Periodicity,
					abbrYearsString,
					monthName,
					RecurrenceInfo.DayNumber,
					GetWeekOfMonthString(RecurrenceInfo.WeekOfMonth),
					dayName);
		}		
		protected internal virtual SchedulerStringId GetAbbrYearsStringId() {
			if (RecurrenceInfo.Periodicity == 1)
				return SchedulerStringId.Abbr_Year;
			else
				return SchedulerStringId.Abbr_Years;
		}
		protected internal virtual SchedulerStringId GetFormatPatternStringId() {
			if (RecurrenceInfo.WeekOfMonth == WeekOfMonth.None) {
				if (RecurrenceInfo.Periodicity == 1)
					return SchedulerStringId.TextYearlyPattern_YearString1;
				else
					return SchedulerStringId.TextYearlyPattern_YearsString1;
			} else {
				if (RecurrenceInfo.Periodicity == 1)
					return SchedulerStringId.TextYearlyPattern_YearString2;
				else
					return SchedulerStringId.TextYearlyPattern_YearsString2;
			}
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Internal {
	public interface IInternalRecurrenceInfo {
		void UpdateRange(DateTime start, DateTime end, RecurrenceRange rangeType, int occurrencesCount, Appointment pattern);
		int CalcOccurrenceCountInRange(Appointment pattern);
		IRecurrenceInfo CreateValidRecurrenceInfoCopy();
		void SetWeekDaysWithoutValidation(WeekDays weekDays);
		void SetTypeWithoutValidation(RecurrenceType type);
		void SetTimeZoneId(string timeZoneId);
		event CancelEventHandler Changing;
		event EventHandler Changed;
	}
}
