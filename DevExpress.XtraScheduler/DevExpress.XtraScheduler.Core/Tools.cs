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
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Tools {
	#region IntervalLoadRatioCalculator
	public class IntervalLoadRatioCalculator {
		#region Fields
		TimeInterval interval;
		AppointmentBaseCollection appointments;
		TimeSpan totalUsedTime;
		#endregion
		public IntervalLoadRatioCalculator(TimeInterval interval, AppointmentBaseCollection appointments) {
			if (interval == null)
				Exceptions.ThrowArgumentNullException("internal");
			if (appointments == null)
				Exceptions.ThrowArgumentNullException("appointments");
			this.interval = interval;
			this.appointments = appointments;
		}
		#region Properties
		protected internal TimeInterval Interval { get { return interval; } }
		protected internal AppointmentBaseCollection Appointments { get { return appointments; } }
		public TimeSpan TotalUsedTime { get { return totalUsedTime; } }
		#endregion
		public virtual float Calculate() {
			if (interval.Duration.Ticks <= 0)
				return 1.0f;
			TimeIntervalCollectionEx usedIntervals = CalculateUsedIntervals();
			this.totalUsedTime = CalculateTotalUsedTime(usedIntervals);
			return (float)(totalUsedTime.Ticks / (double)interval.Duration.Ticks);
		}
		protected internal virtual TimeIntervalCollectionEx CalculateUsedIntervals() {
			return FreeTimeCalculator.CalculateBusyTime(appointments, interval);
		}
		protected internal virtual TimeSpan CalculateTotalUsedTime(TimeIntervalCollectionEx usedIntervals) {
			TimeSpan result = TimeSpan.Zero;
			int count = usedIntervals.Count;
			for (int i = 0; i < count; i++)
				result += usedIntervals[i].Duration;
			return result;
		}
	}
	#endregion
	#region FreeTimeCalculator
	#region IntervalFoundEventHandler
	public delegate void IntervalFoundEventHandler(object sender, IntervalFoundEventArgs e);
	#endregion
	#region IntervalFoundEventArgs
	public class IntervalFoundEventArgs : EventArgs {
		TimeIntervalCollectionEx freeIntervals;
		Resource resource;
		public IntervalFoundEventArgs(TimeIntervalCollectionEx freeIntervals, Resource resource)
			: base() {
			this.resource = resource;
			this.freeIntervals = freeIntervals;
		}
		public TimeIntervalCollectionEx FreeIntervals { get { return freeIntervals; } set { freeIntervals = value; } }
		public Resource Resource { get { return resource; } set { resource = value; } }
	}
	#endregion
	public class FreeTimeCalculator {
		ISchedulerStorageBase storage;
		bool applyAppointmentFilters;
		public FreeTimeCalculator(ISchedulerStorageBase storage) {
			if (storage == null)
				Exceptions.ThrowArgumentNullException("storage");
			this.storage = storage;
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("FreeTimeCalculatorStorage")]
#endif
		public ISchedulerStorageBase Storage { get { return storage; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("FreeTimeCalculatorApplyAppointmentFilters")]
#endif
		public bool ApplyAppointmentFilters { get { return applyAppointmentFilters; } set { applyAppointmentFilters = value; } }
		#region IntervalFound
		IntervalFoundEventHandler onInterfalFound;
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("FreeTimeCalculatorIntervalFound")]
#endif
		public event IntervalFoundEventHandler IntervalFound { add { onInterfalFound += value; } remove { onInterfalFound -= value; } }
		protected internal void RaiseIntervalFound(TimeIntervalCollectionEx freeIntervals, Resource resource) {
			if (onInterfalFound != null) {
				IntervalFoundEventArgs args = new IntervalFoundEventArgs(freeIntervals, resource);
				onInterfalFound(this, args);
			}
		}
		#endregion
		public TimeIntervalCollection CalculateFreeTime(TimeInterval interval) {
			return CalculateFreeTime(interval, ResourceBase.Empty);
		}
		public TimeIntervalCollection CalculateFreeTime(TimeInterval interval, Resource resource) {
			AppointmentBaseCollection appointments = GetAppointments(interval, resource);
			TimeIntervalCollectionEx busyIntervals = CalculateBusyTime(appointments, interval);
			return CalculateFreeTimeCore(busyIntervals, interval, resource);
		}
		public TimeInterval FindFreeTimeInterval(TimeInterval interval, TimeSpan duration, bool forward) {
			return FindFreeTimeInterval(interval, ResourceBase.Empty, duration, forward);
		}
		public TimeInterval FindFreeTimeInterval(TimeInterval interval, Resource resource, TimeSpan duration, bool forward) {
			TimeIntervalCollection freeIntervals = CalculateFreeTime(interval, resource);
			if (forward)
				return FindFreeTimeIntervalForward(freeIntervals, duration);
			else
				return FindFreeTimeIntervalBackward(freeIntervals, duration);
		}
		internal static TimeIntervalCollectionEx CalculateBusyTime(AppointmentBaseCollection appointments, TimeInterval interval) {
			TimeIntervalCollectionEx busyIntervals = new TimeIntervalCollectionEx();
			int count = appointments.Count;
			for (int i = 0; i < count; i++)
				busyIntervals.Add(TimeInterval.Intersect(interval, ((IInternalAppointment)appointments[i]).GetInterval()));
			return busyIntervals;
		}
		protected internal TimeInterval FindFreeTimeIntervalForward(TimeIntervalCollection freeIntervals, TimeSpan duration) {
			int count = freeIntervals.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = freeIntervals[i];
				if (interval.Duration >= duration)
					return new TimeInterval(interval.Start, duration);
			}
			return TimeInterval.Empty;
		}
		protected internal TimeInterval FindFreeTimeIntervalBackward(TimeIntervalCollection freeIntervals, TimeSpan duration) {
			int count = freeIntervals.Count;
			for (int i = count - 1; i >= 0; i--) {
				TimeInterval interval = freeIntervals[i];
				if (interval.Duration >= duration)
					return new TimeInterval(interval.End - duration, duration);
			}
			return TimeInterval.Empty;
		}
		protected internal virtual AppointmentBaseCollection GetAppointments(TimeInterval interval, Resource resource) {
			AppointmentBaseCollection appointments;
			if (ApplyAppointmentFilters)
				appointments = Storage.GetAppointments(interval);
			else
				appointments = ((IInternalSchedulerStorageBase)Storage).GetNonFilteredAppointments(interval, this);
			return FilterAppointmentsByResource(appointments, resource);
		}
		protected internal AppointmentBaseCollection FilterAppointmentsByResource(AppointmentBaseCollection appointments, Resource resource) {
			ResourcesAppointmentsFilter filter = new ResourcesAppointmentsFilter(resource);
			filter.Process(appointments);
			return (AppointmentBaseCollection)filter.DestinationCollection;
		}
		protected internal TimeIntervalCollection CalculateFreeTimeCore(TimeIntervalCollectionEx busyIntervals, TimeInterval interval, Resource resource) {
			TimeIntervalCollectionEx result = new TimeIntervalCollectionEx();
			DateTime start = interval.Start;
			int count = busyIntervals.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval busyInterval = busyIntervals[i];
				AddFreeIntervals(result, start, busyInterval.Start, resource);
				start = busyInterval.End;
			}
			AddFreeIntervals(result, start, interval.End, resource);
			return result;
		}
		protected internal void AddFreeIntervals(TimeIntervalCollectionEx target, DateTime start, DateTime end, Resource resource) {
			TimeIntervalCollectionEx freeIntervals = new TimeIntervalCollectionEx();
			if (start != end) {
				freeIntervals.Add(new TimeInterval(start, end));
				RaiseIntervalFound(freeIntervals, resource);
			}
			target.AddRange(freeIntervals);
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.UI {
	public sealed class ControlInputHelper {
		ControlInputHelper() {
		}
		public static int GetIntegerValue(object val) {
			try {
				return Convert.ToInt32(val);
			}
			catch {
				return 0;
			}
		}
		public static bool CheckPositiveValue(object val) {
			try {
				int day = Convert.ToInt32(val);
				if (day <= 0)
					return false;
			}
			catch {
				return false;
			}
			return true;
		}
		public static bool CheckLimitedPositiveValue(object val, int maxValue) {
			try {
				int day = Convert.ToInt32(val);
				if (day <= 0 || day > maxValue)
					return false;
			}
			catch {
				return false;
			}
			return true;
		}
		public static string GetPositiveValueErrorMessage(object val, SchedulerStringId invalidFormat, SchedulerStringId invalidValue) {
			try {
				int day = Convert.ToInt32(val ); 
				if (day <= 0)
					return SchedulerLocalizer.GetString(invalidValue);
			}
			catch {
				return SchedulerLocalizer.GetString(invalidFormat);
			}
			return String.Empty;
		}
		public static string GetLimitedPositiveValueErrorMessage(object val, int maxValue, SchedulerStringId invalidFormat, SchedulerStringId invalidValue) {
			try {
				int day = Convert.ToInt32(val);
				if (day <= 0 || day > maxValue)
					return String.Format(SchedulerLocalizer.GetString(invalidValue), maxValue);
			}
			catch {
				return String.Format(SchedulerLocalizer.GetString(invalidFormat), maxValue);
			}
			return String.Empty;
		}
	}
}
