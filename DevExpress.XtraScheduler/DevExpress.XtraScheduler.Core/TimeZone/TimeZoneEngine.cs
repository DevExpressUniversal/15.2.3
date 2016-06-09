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

using DevExpress.XtraScheduler.Internal;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler {
	public interface ISupportTimeZoneEngine {
		TimeZoneInfo DefaultAppointmentTimeZone { get; set; }
	}
}
namespace DevExpress.XtraScheduler.Native {
	public class TimeZoneEngineBase {
		public static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo targetTimeZone) {
			if (sourceTimeZone == null || targetTimeZone == null)
				return dateTime;
			if (sourceTimeZone.Equals(targetTimeZone))
				return dateTime;
			return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), sourceTimeZone, targetTimeZone);
		}
		public TimeZoneEngineBase() {
			OperationTimeZone = TimeZoneEngine.Local;
		}
		public virtual TimeZoneInfo OperationTimeZone { get; internal set; }
		public virtual DateTime ToOperationTime(DateTime dateTime, string tzId) {
			if (String.IsNullOrEmpty(tzId))
				return dateTime;
			TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(tzId);
			dateTime = NormalizeDate(dateTime);
			if (timeZone == OperationTimeZone)
				return dateTime;
			return TimeZoneInfo.ConvertTime(dateTime, timeZone, OperationTimeZone);
		}
		public virtual DateTime FromOperationTime(DateTime dateTime, string tzId) {
			if (String.IsNullOrEmpty(tzId))
				return dateTime;
			TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(tzId);
			if (timeZone == OperationTimeZone)
				return dateTime;
			dateTime = NormalizeDate(dateTime);
			return TimeZoneInfo.ConvertTime(dateTime, OperationTimeZone, timeZone);
		}
		public virtual DateTime FromOperationTimeToUtc(DateTime dateTime) {
			dateTime = NormalizeDate(dateTime);
			return TimeZoneInfo.ConvertTime(dateTime, OperationTimeZone, TimeZoneInfo.Utc);
		}
		public virtual DateTime ToOperationTimeFromUtc(DateTime dateTime) {
			dateTime = NormalizeDate(dateTime);
			return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc, OperationTimeZone);
		}
		public virtual DateTime ToUtcTime(DateTime dateTime, string sourceTimeZoneId) {
			TimeZoneInfo sourceTimeZone = (String.IsNullOrEmpty(sourceTimeZoneId)) ? OperationTimeZone : TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId);
			dateTime = NormalizeDate(dateTime);
			return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), sourceTimeZone, TimeZoneInfo.Utc);
		}
		public virtual DateTime FromUtc(DateTime dateTime, string targetTimeZoneId) {
			TimeZoneInfo targetTimeZone = (String.IsNullOrEmpty(targetTimeZoneId)) ? OperationTimeZone : TimeZoneInfo.FindSystemTimeZoneById(targetTimeZoneId);
			dateTime = NormalizeDate(dateTime);
			return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc, targetTimeZone);
		}
		public DateTime ValidateDateTime(DateTime start, string timeZoneId) {
			TimeZoneInfo tzi = (String.IsNullOrEmpty(timeZoneId)) ? OperationTimeZone : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			return ValidateDateTime(start, tzi);
		}
		public DateTime ValidateDateTime(DateTime start, TimeZoneInfo tzi) {
			start = NormalizeDate(start);
			if (!tzi.IsInvalidTime(start))
				return start;
			TimeSpan daylightDelta = ObtainDaylightDelta(start, tzi);
			start = start.Add(daylightDelta);
			return start;
		}
		protected TimeInterval ValidateInterval(TimeInterval interval, TimeZoneInfo tzi) {
			DateTime start = NormalizeDate(interval.Start);
			start = ValidateDateTime(start, tzi);
			return new TimeInterval(start, interval.Duration);
		}		
		protected DateTime NormalizeDate(DateTime dateTime) {
			if (dateTime.Kind != DateTimeKind.Unspecified)
				return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
			return dateTime;
		}
		TimeSpan ObtainDaylightDelta(DateTime dateTime, TimeZoneInfo timeZone) {
			TimeZoneInfo.AdjustmentRule[] rules = timeZone.GetAdjustmentRules();
			foreach (System.TimeZoneInfo.AdjustmentRule rule in rules) {
				if (rule.DaylightDelta == TimeSpan.Zero)
					continue;
				int year = dateTime.Year;
				DateTime startDaylightTime = TimeZoneInfoUtils.CalculateDateTimeFromTransitionTime(year, rule.DaylightTransitionStart);
				DateTime endDaylightTime = TimeZoneInfoUtils.CalculateDateTimeFromTransitionTime(year, rule.DaylightTransitionEnd);
				if (startDaylightTime <= dateTime && dateTime <= endDaylightTime) {
					return rule.DaylightDelta;
				}
			}
			return TimeSpan.Zero;
		}
	}
	public class TimeZoneEngine : TimeZoneEngineBase {
#if DEBUGTEST
		public static TimeZoneEngine TestDefaultInstance = new TimeZoneEngine();
		static TimeZoneInfo localTimeZone;
		internal static TimeZoneInfo Local {
			get {
				if (localTimeZone == null)
					return TimeZoneInfo.Local;
				return localTimeZone;
			}
			set {
				localTimeZone = value;
			}
		}		
#else
		internal static TimeZoneInfo Local { get { return TimeZoneInfo.Local; } }
#endif
		#region DefaultAppointmentTimeZone
		TimeZoneInfo defaultAppointmentTimeZone = null;
		public TimeZoneInfo DefaultAppointmentTimeZone {
			get {
				if (defaultAppointmentTimeZone == null)
					return OperationTimeZone;
				return defaultAppointmentTimeZone;
			}
			set {
				defaultAppointmentTimeZone = value;
			}
		}
		#endregion
		internal bool UseDefaultAppointmentTimeZone {
			get { return this.defaultAppointmentTimeZone != null; }
		}
		internal DateTime ToOperationTimeFromDefaultAppointmentTimeZone(DateTime val) {
			if (UseDefaultAppointmentTimeZone)
				return ConvertTime(val, DefaultAppointmentTimeZone, OperationTimeZone);
			return val;
		}
		internal DateTime FromOperationTimeToDefaultAppointmentTimeZone(DateTime val) {
			if (UseDefaultAppointmentTimeZone)
				return ConvertTime(val, OperationTimeZone, DefaultAppointmentTimeZone);
			return val;
		}
	}
	public class TimeZoneEngineSyncronizer {
		InnerSchedulerControl innerControl;
		SchedulerTimeZoneHelperEventWrapper timeZoneEventWrapper;
		public TimeZoneEngineSyncronizer(InnerSchedulerControl control, TimeZoneHelper timeZoneHelper) {
			Control = control;
			TimeZoneHelper = timeZoneHelper;
			SubscribeOptionsBehavior();
			this.timeZoneEventWrapper = new SchedulerTimeZoneHelperEventWrapper(this);
		}
		#region Properties
		protected InnerSchedulerControl Control {
			get {
				return innerControl;
			}
			private set {
				if (innerControl == value)
					return;
				UnsubscribeOptionsBehavior();
				UnsubscribeInnerControlEvents();
				innerControl = value;
				SubscribeOptionsBehavior();
				SubscribeInnerControlEvents();
			}
		}
		protected TimeZoneHelper TimeZoneHelper { get; private set; }
		#endregion
		internal void OnSysTimeChanged(object sender, EventArgs e) {
			TimeZoneInfo.ClearCachedData();
			UpdateTimeZoneEngine();
		}
		void SubscribeInnerControlEvents() {
			if (Control == null)
				return;
			Control.StorageChanged += StorageChanged;
		}
		void UnsubscribeInnerControlEvents() {
			if (Control == null)
				return;
			Control.StorageChanged -= StorageChanged;
		}
		void StorageChanged(object sender, EventArgs e) {
			SetStorage(Control.Storage);
		}
		protected virtual void SubscribeOptionsBehavior() {
			if (Control == null || Control.OptionsBehavior == null)
				return;
			Control.OptionsBehavior.Changed += OptionsBehavior_Changed;
		}
		protected virtual void UnsubscribeOptionsBehavior() {
			if (Control == null || Control.OptionsBehavior == null)
				return;
			Control.OptionsBehavior.Changed -= OptionsBehavior_Changed;
		}
		void OptionsBehavior_Changed(object sender, Utils.Controls.BaseOptionChangedEventArgs e) {
			UpdateTimeZoneEngine();
		}
		void UpdateTimeZoneEngine() {
			if (Control == null || Control.OptionsBehavior == null)
				return;
			TimeZoneHelper.ClientTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Control.OptionsBehavior.ClientTimeZoneId);
		}
		public void Dispose() {
			UnsubscribeOptionsBehavior();
			UnsubscribeInnerControlEvents();
			this.innerControl = null;
			if (this.timeZoneEventWrapper != null) {
				this.timeZoneEventWrapper.CleanUp();
				this.timeZoneEventWrapper = null;
			}
		}
		public void SetStorage(ISchedulerStorageBase storage) {
			IInternalSchedulerStorageBase internalStorage = storage as IInternalSchedulerStorageBase;
			if (TimeZoneHelper == null || internalStorage == null)
				return;
			TimeZoneHelper.StorageTimeZoneEngine = (storage == null) ? null : internalStorage.TimeZoneEngine;
		}
	}
}
