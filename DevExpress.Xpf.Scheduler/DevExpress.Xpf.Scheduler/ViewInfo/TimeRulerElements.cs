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
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class TimeRulerViewInfo : ISelectableIntervalViewInfo {
		#region Fields
		DayView view;
		TimeZoneHelper timeZoneEngine;
		TimeRuler timeRuler;
		TimeSpan timeScale;
		TimeOfDayInterval interval;
		TimeRulerElementCollection elements;
		TimeRulerMinuteItemCollection allMinuteItems;
		IViewInfo header;
		bool isFirstTimeRuler;
		TimeFormatInfo formatInfo;
		bool isTimeMarkerVisibility;
		#endregion
		public TimeRulerViewInfo(DayView view, DateTime visibleStart, TimeRuler timeRuler, TimeOfDayInterval interval, TimeZoneInfo clientTimeZone, TimeSpan timeScale, bool isFirstTimeRuler, int timeRulerHeaderSpan, TimeFormatInfo formatInfo) {
			if (view == null)
				Exceptions.ThrowArgumentNullException("view");
			if (timeRuler == null)
				Exceptions.ThrowArgumentNullException("timeRuler");
			if (interval == null)
				Exceptions.ThrowArgumentNullException("interval");
			if (clientTimeZone == null)
				Exceptions.ThrowArgumentNullException("clientTimeZone");
			if (timeScale.Ticks <= 0)
				Exceptions.ThrowArgumentException("timeScale", timeScale);
			if (formatInfo == null)
				Exceptions.ThrowArgumentNullException("formatInfo");
			this.view = view;
			this.timeRuler = timeRuler;
			this.interval = interval;
			this.timeScale = timeScale;
			this.isFirstTimeRuler = isFirstTimeRuler;
			this.formatInfo = formatInfo;
			DateTime[] times = CreateActualTimes(visibleStart, TimeRuler, Interval, TimeScale, clientTimeZone);
			if (timeRulerHeaderSpan > 0) {
				this.header = CreateHeader(TimeRuler);
			}
			this.elements = CreateTimeRulerItems(times);
			this.allMinuteItems = GetAllMinuteItems();
			int count = elements.Count;
			if (count > 0)
				elements[count - 1].HideBottomBorder();
			TimeInterval visibleInterval = GetVisibleInterval(times);
			DateTime localNowTime = CalculateLocalNowTime(timeRuler.TimeZoneId);
			TimeMarkerVisibility? visibility = timeRuler.TimeMarkerVisibility;
			if (visibility == null)
				visibility = view.TimeMarkerVisibility;
			this.isTimeMarkerVisibility = CanShowTimeMarker(visibility, visibleInterval, localNowTime);
		}
		#region Properties
		public DayView View { get { return view; } }
		public TimeZoneHelper TimeZoneHelper { get { return timeZoneEngine; } set { timeZoneEngine = value; } }
		public TimeRuler TimeRuler { get { return timeRuler; } }
		public TimeSpan TimeScale { get { return timeScale; } }
		public TimeOfDayInterval Interval { get { return interval; } }
		public TimeRulerElementCollection Elements { get { return elements; } }
		public TimeRulerMinuteItemCollection AllMinuteItems { get { return allMinuteItems; } }
		public IViewInfo Header { get { return header; } }
		public bool IsFirstTimeRuler { get { return isFirstTimeRuler; } }
		public TimeFormatInfo FormatInfo { get { return formatInfo; } }
		public bool IsTimeMarkerVisibility { get { return isTimeMarkerVisibility; } }
		#endregion
		protected internal TimeRulerMinuteItemCollection GetAllMinuteItems() {
			TimeRulerMinuteItemCollection result = new TimeRulerMinuteItemCollection();
			int count = elements.Count;
			for (int i = 0; i < count; i++)
				result.AddRange(elements[i].MinuteItems);
			return result;
		}
		protected internal virtual IViewInfo CreateHeader(TimeRuler ruler) {
			TimeRulerHeader header = new TimeRulerHeader(ruler);
			return header;
		}
		protected internal virtual DateTime[] CreateActualTimes(DateTime visibleStart, TimeRuler timeRuler, TimeOfDayInterval interval, TimeSpan timeScale, TimeZoneInfo clientTimeZone) {
			TimeOfDayIntervalCollection intervals = TimeRulerHelper.SplitInterval(interval, timeScale);
			DateTime[] result = TimeRulerHelper.CreateActualTimes(visibleStart, timeRuler, intervals, clientTimeZone);
			XtraSchedulerDebug.Assert(result.Length == intervals.Count);
			return result;
		}
		protected internal virtual TimeRulerElementCollection CreateTimeRulerItems(DateTime[] times) {
			TimeRulerElementCollection result = new TimeRulerElementCollection();
			int timeIndex = 0;
			int count = times.Length;
			while (timeIndex < count) {
				TimeRulerElement element = CreateTimeRulerElement(timeIndex, times);
				result.Add(element);
				timeIndex += element.TimeItemCount;
			}
			return result;
		}
		protected internal virtual TimeRulerElement CreateTimeRulerElement(int startIndex, DateTime[] times) {
			DateTime time = times[startIndex];
			if (TimeScale.Ticks % DateTimeHelper.HourSpan.Ticks == 0)
				return new TimeRulerElement(new TimeRulerSingleHourItem(time, ScaleFormatHelper.ChooseHourFormat(time, timeRuler.AlwaysShowTimeDesignator, formatInfo)));
			TimeRulerHoursItem hourItem = new TimeRulerHoursItem(time, formatInfo.HourOnlyFormat);
			TimeRulerMinuteItemCollection minuteItems = CreateMinuteItems(startIndex, times);
			return new TimeRulerElement(hourItem, minuteItems);
		}
		protected internal virtual TimeRulerMinuteItemCollection CreateMinuteItems(int startIndex, DateTime[] times) {
			TimeRulerMinuteItemCollection minuteItems = new TimeRulerMinuteItemCollection();
			int length = times.Length;
			int currentIndex = startIndex;
			bool isFirstCellInHour = true;
			do {
				DateTime time = times[currentIndex];
				string format;
				if (timeRuler.ShowMinutes || time.Minute == 0 || isFirstCellInHour)
					format = ScaleFormatHelper.ChooseMinutesFormat(time, timeRuler.AlwaysShowTimeDesignator, formatInfo);
				 else
					format = String.Empty;
				minuteItems.Add(new TimeRulerMinuteItem(time, format));
				currentIndex++;
				isFirstCellInHour = false;
			}
			while (currentIndex < length && !IsBeginOfHourCell(times[currentIndex]));
			return minuteItems;
		}
		bool IsBeginOfHourCell(DateTime time) {
			return time.Minute - (int)TimeScale.TotalMinutes < 0L;
		}
		#region ISelectableIntervalViewInfo Members
		SchedulerHitTest ISelectableIntervalViewInfo.HitTestType { get { return SchedulerHitTest.Ruler; } }
		TimeInterval ISelectableIntervalViewInfo.Interval { get { return Interval.ToTimeInterval(); } }
		Resource ISelectableIntervalViewInfo.Resource { get { return ResourceBase.Empty; } }
		bool ISelectableIntervalViewInfo.Selected { get { return false; } }
		#endregion
		bool CanShowTimeMarker(TimeMarkerVisibility? visibility, TimeInterval visibleInterval, DateTime now) {
			if (visibility == TimeMarkerVisibility.Never)
				return false;
			if (visibility == TimeMarkerVisibility.Always)
				return true;
			return visibleInterval.Contains(now);
		}
		DateTime CalculateLocalNowTime(string rulerTimeZoneId) {
			if (String.IsNullOrEmpty(rulerTimeZoneId))
				rulerTimeZoneId = TimeZoneInfo.Local.Id;
			return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(rulerTimeZoneId));
		}
		TimeInterval GetVisibleInterval(DateTime[] times) {
			if (times == null || times.Length == 0)
				return TimeInterval.Empty;
			DateTime startInterval = times[0];
			DateTime endInterval = times[times.Length - 1];
			return new TimeInterval(startInterval, endInterval);
		}
	}
	public class TimeRulerElement {
		#region Fields
		TimeRulerHoursItem hourItem;
		TimeRulerMinuteItemCollection minuteItems;
		#endregion
		public TimeRulerElement(TimeRulerHoursItem hourItem)
			: this(hourItem, new TimeRulerMinuteItemCollection()) {
		}
		public TimeRulerElement(TimeRulerHoursItem hourItem, TimeRulerMinuteItemCollection minuteItems) {
			Guard.ArgumentNotNull(hourItem, "hourItem");
			Guard.ArgumentNotNull(minuteItems, "minuteItems");
			this.hourItem = hourItem;
			this.minuteItems = minuteItems;
		}
		#region Peoperties
		public TimeRulerHoursItem HourItem { get { return hourItem; } }
		public TimeRulerMinuteItemCollection MinuteItems { get { return minuteItems; } }
		public int TimeItemCount { get { return Math.Max(1, minuteItems.Count); } }
		#endregion
		public virtual void HideBottomBorder() {
		}
	}
	public abstract class TimeRulerItem : IViewInfo {
		#region Fields
		DateTime time;
		string format;
		string caption;
		#endregion
		protected TimeRulerItem(DateTime time, string format) {
			this.time = time;
			this.format = format;
			if (!String.IsNullOrEmpty(format))
				caption = Time.ToString(format);
		}
		#region Properties
		public DateTime Time { get { return time; } }
		protected internal string Format { get { return format; } }
		public string Caption { get { return caption; } }
		#endregion
		#region SetDefaultContent
		#endregion
	}
	public class TimeRulerHoursItem : TimeRulerItem {
		public TimeRulerHoursItem(DateTime time, string format)
			: base(time, format) {
		}
	}
	public class TimeRulerSingleHourItem : TimeRulerHoursItem {
		public TimeRulerSingleHourItem(DateTime time, string format)
			: base(time, format) {
		}
	}
	public class TimeRulerMinuteItem : TimeRulerItem {
		public TimeRulerMinuteItem(DateTime time, string format)
			: base(time, format) {
		}
	}
	public class TimeRulerHelper {
		internal static TimeOfDayIntervalCollection SplitInterval(TimeOfDayInterval interval, TimeSpan timeScale) {
			TimeOfDayIntervalCollection intervals = new TimeOfDayIntervalCollection();
			TimeSpan start = interval.Start;
			while (start < interval.End) {
				intervals.Add(new TimeOfDayInterval(start, start + timeScale));
				start += timeScale;
			}
			return intervals;
		}
		public static DateTime[] CreateActualTimes(DateTime visibleStart, TimeRuler timeRuler, TimeOfDayIntervalCollection intervals, TimeZoneInfo clientTimeZone) {
			int count = intervals.Count;
			DateTime[] result = new DateTime[count];
			if (count <= 0)
				return result;
			for (int i = 0; i < count; i++)
				result[i] = CalcActualDate(visibleStart.Date, intervals[i].Start, clientTimeZone, timeRuler.TimeZoneId);
			return result;
		}
		static DateTime CalcActualDate(DateTime baseDate, TimeSpan time, TimeZoneInfo currentTimeZone, string timeZoneId) {
			TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			return TimeZoneEngine.ConvertTime(baseDate + time, currentTimeZone, targetTimeZone);
		}
	}
}
