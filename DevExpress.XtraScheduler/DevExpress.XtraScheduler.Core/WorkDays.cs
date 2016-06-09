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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
namespace DevExpress.XtraScheduler {
	using DevExpress.Schedule;
	#region WorkDaysCollection
	public class WorkDaysCollection : NotificationCollection<WorkDay>, ICloneable {
		WorkDayFastCalculator calculator;
		public WorkDaysCollection() {
			this.calculator = CreateWorkDayFastCalculator();
		}
		protected internal WorkDayFastCalculator Calculator { get { return calculator; } }
		protected internal virtual WorkDayFastCalculator CreateWorkDayFastCalculator() {
			return new WorkDayFastCalculator();
		}
		public int Add(WeekDays weekDays) {
			return Add(new WeekDaysWorkDay(weekDays));
		}
		public int AddHoliday(DateTime date, string displayName) {
			return Add(new Holiday(date, displayName));
		}
		protected override void OnInsertComplete(int index, WorkDay value) {
			calculator.Add(value);
			base.OnInsertComplete(index, value);
		}
		protected override void OnRemoveComplete(int index, WorkDay value) {
			base.OnRemoveComplete(index, value);
			calculator.Remove(value);
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			calculator.Clear();
		}
		internal bool IsWeekDaysWorkDay(DateTime date) {
			return calculator.CombinedWeekDaysWorkDay.IsWorkDay(date);
		}
		public bool IsWorkDay(DateTime date) {
			return calculator.IsWorkDay(date);
		}
		public bool IsHoliday(DateTime date) {
			return calculator.IsHoliday(date);
		}
		public WeekDays GetWeekDays() {
			return calculator.CombinedWeekDaysWorkDay.WeekDays;
		}
		public virtual void Assign(WorkDaysCollection source) {
			if (source == null)
				return;
			BeginUpdate();
			try {
				Clear();
				source.CloneContent(this);
			} finally {
				EndUpdate();
			}
		}
		#region ICloneable implementation
		object ICloneable.Clone() {
			return CloneCore();
		}
		public WorkDaysCollection Clone() {
			return CloneCore();
		}
		internal WorkDaysCollection CloneCore() {
			WorkDaysCollection clone = CreateEmptyClone();
			CloneContent(clone);
			return clone;
		}
		protected internal virtual WorkDaysCollection CreateEmptyClone() {
			return new WorkDaysCollection();
		}
		protected internal virtual void CloneContent(WorkDaysCollection target) {
			int count = Count;
			for (int i = 0; i < count; i++) {
				target.InnerList.Add((WorkDay)((ICloneable)this[i]).Clone());
				target.Calculator.Add(this[i]);
			}
		}
		#endregion
	}
	#endregion
	#region WeekDaysWorkDay
	public class WeekDaysWorkDay : WorkDay {
		WeekDays weekDays;
		public WeekDaysWorkDay(WeekDays weekDays) {
			this.weekDays = weekDays;
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("WeekDaysWorkDayType")]
#endif
		public override WorkDayType Type { get { return WorkDayType.WeekDay; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("WeekDaysWorkDayWeekDays")]
#endif
		public WeekDays WeekDays {
			get { return weekDays; }
		}
		public override bool IsWorkDay(DateTime date) { return CheckDate(date) == DateCheckResult.WorkDay; }
		public override DateCheckResult CheckDate(DateTime date) {
			if ((WeekDays & DateTimeHelper.ToWeekDays(date.DayOfWeek)) != 0)
				return DateCheckResult.WorkDay;
			return DateCheckResult.Holiday;
		}
		#region ICloneable implementation
		protected override object CloneCore() {
			return new WeekDaysWorkDay(this.weekDays);
		}
		public WeekDaysWorkDay Clone() {
			return (WeekDaysWorkDay)CloneCore();
		}
		#endregion
		public override bool Equals(object obj) {
			WeekDaysWorkDay val = obj as WeekDaysWorkDay;
			if (val == null)
				return false;
			return WeekDays == val.WeekDays;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	using DevExpress.Schedule;
	public class KnownDateDayCollection : List<KnownDateDay> {
	}
	#region WorkDayFastCalculator
	public class WorkDayFastCalculator {
		#region Fields
		WeekDaysWorkDay combinedWeekDaysWorkDay;
		Dictionary<DateTime, KnownDateDayCollection> holidays;
		#endregion
		public WorkDayFastCalculator() {
			this.combinedWeekDaysWorkDay = new WeekDaysWorkDay((WeekDays)0);
			this.holidays = new Dictionary<DateTime, KnownDateDayCollection>();
		}
		#region Properties
		internal WeekDaysWorkDay CombinedWeekDaysWorkDay { get { return combinedWeekDaysWorkDay; } }
		internal Dictionary<DateTime, KnownDateDayCollection> Holidays { get { return holidays; } }
		#endregion
		public virtual void Add(WorkDay workDay) {
			WeekDaysWorkDay weekDaysWorkDay = workDay as WeekDaysWorkDay;
			if (weekDaysWorkDay != null)
				combinedWeekDaysWorkDay = new WeekDaysWorkDay(combinedWeekDaysWorkDay.WeekDays | weekDaysWorkDay.WeekDays);
			else {
				KnownDateDay day = workDay as KnownDateDay;
				if (day != null) {
					KnownDateDayCollection days;
					if (holidays.TryGetValue(day.Date.Date, out days)) {
						int count = days.Count;
						for (int i = 0; i < count; i++) {
							if ((int)days[i].Type <= (int)day.Type) {
								days.Insert(i, day);
								return;
							}
						}
						days.Add(day);
					} else {
						days = new KnownDateDayCollection();
						holidays.Add(day.Date.Date, days);
						days.Add(day);
					}
				}
			}
		}
		public virtual void Remove(WorkDay workDay) {
			WeekDaysWorkDay weekDaysWorkDay = workDay as WeekDaysWorkDay;
			if (weekDaysWorkDay != null)
				combinedWeekDaysWorkDay = new WeekDaysWorkDay(combinedWeekDaysWorkDay.WeekDays & ~weekDaysWorkDay.WeekDays);
			else {
				KnownDateDay day = workDay as KnownDateDay;
				if (day != null) {
					KnownDateDayCollection days;
					if (holidays.TryGetValue(day.Date.Date, out days)) {
						days.Remove(day);
						if (days.Count <= 0)
							holidays.Remove(day.Date.Date);
					}
				}
			}
		}
		public virtual void Clear() {
			combinedWeekDaysWorkDay = new WeekDaysWorkDay((WeekDays)0);
			holidays.Clear();
		}
		public virtual bool IsWorkDay(DateTime date) {
			KnownDateDayCollection days;
			if (holidays.TryGetValue(date.Date, out days))
				return days[0].IsWorkDay(date);
			else
				return combinedWeekDaysWorkDay.IsWorkDay(date);
		}
		public virtual bool IsHoliday(DateTime date) {
			if (holidays.ContainsKey(date))
				return true;
			return false;
		}
	}
	#endregion
	public static class HolidaysHelper {
		public static AppointmentBaseCollection GenerateHolidayAppointments(ISchedulerStorageBase storage, WorkDaysCollection workDays) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			if (storage == null)
				return result;
			if (workDays == null)
				return result;
			HolidayBaseCollection holidays = SelectHolidays(workDays);
			int count = holidays.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = CreateHolidayAppointment(holidays[i], storage);
				if (apt != null)
					result.Add(apt);
			}
			return result;
		}
		internal static HolidayBaseCollection SelectHolidays(WorkDaysCollection workDays) {
			HolidayBaseCollection holidays = new HolidayBaseCollection();
			if (workDays == null)
				return holidays;
			int count = workDays.Count;
			for (int i = 0; i < count; i++) {
				if (workDays[i] is Holiday) {
					Holiday h = (Holiday)workDays[i];
					holidays.Add(h.Clone());
				}
			}
			return holidays;
		}
		public static Appointment CreateHolidayAppointment(Holiday holiday, ISchedulerStorageBase storage) {
			if (holiday == null)
				return null;
			Appointment apt = SchedulerUtils.CreateAppointmentInstance(storage, AppointmentType.Normal);
			apt.Start = holiday.Date;
			apt.AllDay = true;
			apt.Subject = holiday.DisplayName;
			apt.Location = holiday.Location;
			return apt;
		}
	}
}
