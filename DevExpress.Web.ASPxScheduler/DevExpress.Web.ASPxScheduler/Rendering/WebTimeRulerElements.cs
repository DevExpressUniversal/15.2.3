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
using System.Globalization;
using DevExpress.Web.ASPxScheduler;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Web.ASPxScheduler.Rendering {
	public class WebTimeRuler : VerticalMergingContainer {
		#region Fields
		TimeRuler timeRuler;
		TimeSpan timeScale;
		TimeOfDayInterval interval;
		WebTimeRulerElementCollection elements;
		IWebViewInfo header;
		bool isFirstTimeRuler;
		TimeFormatInfo formatInfo;
		#endregion
		public WebTimeRuler(DateTime visibleStart, TimeRuler timeRuler, TimeOfDayInterval interval, TimeZoneHelper timeZoneHelper, TimeSpan timeScale, bool isFirstTimeRuler, int timeRulerHeaderSpan, TimeFormatInfo formatInfo) {
			if (timeRuler == null)
				Exceptions.ThrowArgumentNullException("timeRuler");
			if (interval == null)
				Exceptions.ThrowArgumentNullException("interval");
			if (timeZoneHelper == null)
				Exceptions.ThrowArgumentNullException("timeZoneEngine");
			if (timeScale.Ticks <= 0)
				Exceptions.ThrowArgumentException("timeScale", timeScale);
			if (formatInfo == null)
				Exceptions.ThrowArgumentNullException("formatInfo");
			TimeZoneHelper = timeZoneHelper;
			this.timeRuler = timeRuler;
			this.interval = interval;
			this.timeScale = timeScale;
			this.isFirstTimeRuler = isFirstTimeRuler;
			this.formatInfo = formatInfo;
			DateTime[] times = CreateActualTimes(visibleStart, TimeRuler, Interval, TimeScale);
			if (timeRulerHeaderSpan > 0) {
				this.header = CreateHeader(TimeRuler, timeRulerHeaderSpan);
				WebObjects.Add(header);
			}
			this.elements = CreateTimeRulerItems(times);
			int count = elements.Count;
			for (int i = 0; i < count; i++)
				WebObjects.Add(elements[i]);
			if (count > 0)
				elements[count - 1].HideBottomBorder();
		}
		#region Properties
		public TimeRuler TimeRuler { get { return timeRuler; } }
		public TimeSpan TimeScale { get { return timeScale; } }
		public TimeOfDayInterval Interval { get { return interval; } }
		public WebTimeRulerElementCollection Elements { get { return elements; } }
		public IWebViewInfo Header { get { return header; } }
		public bool IsFirstTimeRuler { get { return isFirstTimeRuler; } }
		public TimeFormatInfo FormatInfo { get { return formatInfo; } }
		protected TimeZoneHelper TimeZoneHelper { get; set; }
		#endregion
		protected internal virtual IWebViewInfo CreateHeader(TimeRuler ruler, int timeRulerHeaderSpan) {
			WebTimeRulerHeader header = new WebTimeRulerHeader(ruler, timeRulerHeaderSpan);
			header.IgnoreBorderSide = IgnoreBorderSide.Top;
			return header;
		}
		protected internal virtual DateTime[] CreateActualTimes(DateTime visibleStart, TimeRuler timeRuler, TimeOfDayInterval interval, TimeSpan timeScale) {
			TimeOfDayInterval adjustedInterval = DayViewCellsCalculatorHelper.CreateAlignedVisibleTime(interval, timeScale, false);
			TimeOfDayIntervalCollection intervals = TableHelper.SplitInterval(adjustedInterval, timeScale);
			DateTime[] result = WebTimeRulerHelper.CreateActualTimes(visibleStart, timeRuler, intervals, TimeZoneHelper);
			XtraSchedulerDebug.Assert(result.Length == intervals.Count);
			return result;
		}
		protected internal virtual WebTimeRulerElementCollection CreateTimeRulerItems(DateTime[] times) {
			WebTimeRulerElementCollection result = new WebTimeRulerElementCollection();
			int timeIndex = 0;
			int count = times.Length;
			while (timeIndex < count) {
				WebTimeRulerElement element = CreateTimeRulerElement(timeIndex, times);
				result.Add(element);
				timeIndex += element.TimeItemCount;
			}
			return result;
		}
		protected internal virtual WebTimeRulerElement CreateTimeRulerElement(int startIndex, DateTime[] times) {
			DateTime time = times[startIndex];
			if (TimeScale.Ticks % DateTimeHelper.HourSpan.Ticks == 0)
				return new WebTimeRulerElement(new WebTimeRulerSingleHourItem(time, ScaleFormatHelper.ChooseHourFormat(time, timeRuler.AlwaysShowTimeDesignator, formatInfo)));
			WebTimeRulerHoursItem hourItem = new WebTimeRulerHoursItem(time, formatInfo.HourOnlyFormat);
			WebTimeRulerMinuteItemCollection minuteItems = CreateMinuteItems(startIndex, times);
			return new WebTimeRulerElement(hourItem, minuteItems);
		}
		protected internal virtual WebTimeRulerMinuteItemCollection CreateMinuteItems(int startIndex, DateTime[] times) {
			WebTimeRulerMinuteItemCollection minuteItems = new WebTimeRulerMinuteItemCollection();
			int length = times.Length;
			int currentIndex = startIndex;
			do {
				DateTime time = times[currentIndex];
				string format;
				if (timeRuler.ShowMinutes || time.Minute == 0)
					format = ScaleFormatHelper.ChooseMinutesFormat(time, timeRuler.AlwaysShowTimeDesignator, formatInfo);
				else
					format = String.Empty;
				minuteItems.Add(new WebTimeRulerMinuteItem(time, format));
				currentIndex++;
			}
			while (currentIndex < length && !DateTimeHelper.IsBeginOfHour(times[currentIndex]));
			return minuteItems;
		}
	}
	public class WebTimeRulerElement : HorizontalMergingContainer {
		#region Fields
		WebTimeRulerHoursItem hourItem;
		WebTimeRulerMinuteItemCollection minuteItems;
		#endregion
		public WebTimeRulerElement(WebTimeRulerHoursItem hourItem)
			: this(hourItem, new WebTimeRulerMinuteItemCollection()) {
		}
		public WebTimeRulerElement(WebTimeRulerHoursItem hourItem, WebTimeRulerMinuteItemCollection minuteItems) {
			if (hourItem == null)
				Exceptions.ThrowArgumentNullException("hourItem");
			if (minuteItems == null)
				Exceptions.ThrowArgumentNullException("minuteItems");
			this.hourItem = hourItem;
			this.minuteItems = minuteItems;
			WebObjects.Add(HourItem);
			if (minuteItems.Count > 0)
				WebObjects.Add(CreateMergedMinuteItems(MinuteItems));
		}
		#region Peoperties
		public WebTimeRulerHoursItem HourItem { get { return hourItem; } }
		public WebTimeRulerMinuteItemCollection MinuteItems { get { return minuteItems; } }
		public int TimeItemCount { get { return Math.Max(1, minuteItems.Count); } }
		#endregion
		public virtual void HideBottomBorder() {
			HourItem.IgnoreBorderSide = IgnoreBorderSide.Bottom;
			int count = MinuteItems.Count;
			if (count > 0)
				MinuteItems[count - 1].IgnoreBorderSide = IgnoreBorderSide.Bottom;
		}
		public WebTimeRulerItem GetLeftItem() {
			return HourItem;
		}
		VerticalMergingContainer CreateMergedMinuteItems(WebTimeRulerMinuteItemCollection minuteItems) {
			VerticalMergingContainer mergedMinuteItems = new VerticalMergingContainer();
			int count = minuteItems.Count;
			for (int i = 0; i < count; i++)
				mergedMinuteItems.WebObjects.Add(minuteItems[i]);
			return mergedMinuteItems;
		}
	}
	public abstract class WebTimeRulerItem : InternalSchedulerCell {
		#region Fields
		readonly DateTime time;
		readonly string format;
		#endregion
		protected WebTimeRulerItem(DateTime time, string format) {
			this.time = time;
			this.format = format;
		}
		#region Properties
		public InternalSchedulerCell Cell { get { return this; } }
		public DateTime Time { get { return time; } }
		protected internal string Format { get { return format; } }
		#endregion
		public override SchedulerTable CreateTable() {
			return TableHelper.CreateTableWithOneCell(Cell);
		}
		#region SetDefaultContent
		protected internal override void SetDefaultContent() {
			if (!String.IsNullOrEmpty(format))
				this.Text = Time.ToString(format);
			else
				this.Text = "&nbsp;";
		}
		#endregion
	}
	public class WebTimeRulerHoursItem : WebTimeRulerItem {
		public WebTimeRulerHoursItem(DateTime time, string format)
			: base(time, format) {
		}
		#region Properties
		public override WebElementType CellType { get { return WebElementType.TimeRulerHours; } }
		#endregion
	}
	public class WebTimeRulerSingleHourItem : WebTimeRulerHoursItem {
		public WebTimeRulerSingleHourItem(DateTime time, string format)
			: base(time, format) {
		}
	}
	public class WebTimeRulerMinuteItem : WebTimeRulerItem {
		public WebTimeRulerMinuteItem(DateTime time, string format)
			: base(time, format) {
		}
		#region Properties
		public override WebElementType CellType { get { return WebElementType.TimeRulerMinute; } }
		#endregion
	}
	public static class WebTimeRulerHelper {
		public static DateTime[] CreateActualTimes(DateTime visibleStart, TimeRuler timeRuler, TimeOfDayIntervalCollection intervals, TimeZoneHelper timeZoneEngine) {
			int count = intervals.Count;
			DateTime[] result = new DateTime[count];
			if (count <= 0)
				return result;
			TimeSpan currentUtcOffset = timeZoneEngine.ClientTimeZone.GetUtcOffset(visibleStart);
			TimeSpan targetUtcOffset = CalcTargetUtcOffset(timeRuler, visibleStart, timeZoneEngine.OperationTimeZone);
			for (int i = 0; i < count; i++)
				result[i] = CalcActualDate(intervals[i].Start, currentUtcOffset, targetUtcOffset);
			return result;
		}
		static TimeSpan CalcTargetUtcOffset(TimeRuler ruler, DateTime date, TimeZoneInfo operationTimeZone) {
			TimeZoneInfo timeZoneInfo = (String.IsNullOrEmpty(ruler.TimeZoneId)) ? operationTimeZone : ObtainTimeZone(ruler.TimeZoneId);
			return timeZoneInfo.GetUtcOffset(date);
		}
		static TimeZoneInfo ObtainTimeZone(string timeZoneId) {
			try {
				return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			} catch {
				return TimeZoneInfo.Utc;
			}
		}
		static DateTime CalcActualDate(TimeSpan time, TimeSpan currentUtcOffset, TimeSpan targetUtcOffset) {
			TimeSpan utcTime = time + DateTimeHelper.DaySpan - currentUtcOffset;
			return new DateTime((utcTime + targetUtcOffset).Ticks);
		}
	}
}
