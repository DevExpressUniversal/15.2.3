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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class TimeRulerLayoutCalculator : TimeRulerLayoutCalculatorBase {
		TimeMarkerVisibility? timeMarkerVisibility;
		public TimeRulerLayoutCalculator(GraphicsCache cache, DayViewInfo viewInfo, TimeRulerPainter painter, TimeMarkerVisibility visibility)
			: base(cache, viewInfo, painter) {
			this.timeMarkerVisibility = visibility;
		}
		protected internal virtual DayViewInfo ViewInfo { get { return (DayViewInfo)base.TimeRulerSupport; } }
		public virtual Rectangle CalcPreliminaryLayout(Rectangle bounds) {
			Rectangle availableBounds = bounds;
			ITimeRulerFormatStringService formatStringProvider = TimeRulerSupport.GetFormatStringProvider();
			TimeRulerCollection timeRulers = GetVisibleTimeRulers();
			int count = timeRulers.Count;
			for (int i = 0; i < count; i++) {
				TimeRulerViewInfo viewInfo = CalcRulerPreliminaryLayout(timeRulers[i], availableBounds, formatStringProvider);
				ViewInfo.TimeRulers.Add(viewInfo);
				availableBounds = RectUtils.CutFromLeft(availableBounds, viewInfo.Bounds.Width);
			}
			return availableBounds;
		}
		public virtual Rectangle CalcBoundsWithoutTimeRulers(Rectangle bounds) {
			Rectangle availableBounds = bounds;
			TimeRulerViewInfoCollection timeRulers = ViewInfo.TimeRulers;
			int count = timeRulers.Count;
			for (int i = 0; i < count; i++)
				availableBounds = RectUtils.CutFromLeft(availableBounds, timeRulers[i].Bounds.Width);
			return availableBounds;
		}
		public virtual void CalcLayout() {
			TimeRulerViewInfoCollection timeRulers = ViewInfo.TimeRulers;
			int count = timeRulers.Count;
			for (int i = 0; i < count; i++)
				CalcRulerLayout(timeRulers[i], i == 0);
		}
		protected internal virtual void RecalcRulersCurrentTimeLayout() {
			TimeRulerViewInfoCollection timeRulers = ViewInfo.TimeRulers;
			int count = timeRulers.Count;
			for (int i = 0; i < count; i++)
				RecalcRulerCurrentTimeLayout(timeRulers[i]);
		}
		protected internal virtual TimeRulerCollection GetVisibleTimeRulers() {
			return ((InnerDayView)ViewInfo.View.InnerView).GetVisibleTimeRulers();
		}
		protected internal virtual void CalcRulerLayout(TimeRulerViewInfo ruler, bool isFirstRuler) {
			Rectangle[] rowsBounds = GetRowsBounds();
			DateTime[] actualTimes = CreateActualTimes(ruler);
			CalcRulerLayoutCore(ruler, rowsBounds, actualTimes, isFirstRuler);
			CreateTimeMarker(ruler);
		}
		protected internal virtual void RecalcRulerCurrentTimeLayout(TimeRulerViewInfo ruler) {
			ruler.DisposeCurrentTimeItems();
			ruler.CurrentTimeItems.Clear();
			CreateTimeMarker(ruler);
		}
		protected internal virtual void CreateTimeMarker(TimeRulerViewInfo ruler) {
			DateTime nowInClientTime = TimeZoneEngine.ConvertTime(Now, TimeZoneEngine.Local, TimeRulerSupport.TimeZoneHelper.ClientTimeZone);
			ruler.CurrentTimeItems.AddRange(CreateTimeMarkerCore(ruler, nowInClientTime));
		}
		protected internal virtual DateTime Now {
			get {
#if DEBUGTEST
				return DevExpress.XtraScheduler.Tests.TestEnvironment.GetNowTime();
#else
				return DateTime.Now;
#endif
			}
		}
		protected internal virtual ViewInfoItemCollection CreateTimeMarkerCore(TimeRulerViewInfo ruler, DateTime now) {
			ViewInfoItemCollection result = new ViewInfoItemCollection();
			TimeMarkerVisibility? visibility = this.timeMarkerVisibility;
			if (ruler.Ruler.TimeMarkerVisibility != null)
				visibility = ruler.Ruler.TimeMarkerVisibility;
			if (!CanShowTimeMarker(visibility, ViewInfo.View.GetVisibleIntervals().Interval, now))
				return result;
			Rectangle bounds = ruler.ClientBounds;
			TimeSpan time = CalcTimeOfDay(now);
			int currentTimeRowIndex = ViewInfo.VisibleRows.BinarySearchStartDate(DateTimeHelper.Floor(time, TimeRulerSupport.TimeScale));
			if (currentTimeRowIndex < 0)
				return result;
			Rectangle markerBounds = Rectangle.FromLTRB(bounds.X, 0, bounds.Right + Painter.ContentSpan, 0);
			DayViewRow row = ViewInfo.VisibleRows[currentTimeRowIndex];
			int y = RectUtils.CalcDateY(row.Interval, row.Bounds, time);
			result.Add(CreateCurrentTimeArea(ruler, markerBounds, y));
			result.Add(CreateCurrentTimeline(ruler, markerBounds, y));
			return result;
		}
		protected internal virtual TimeSpan CalcTimeOfDay(DateTime date) {
			TimeSpan result = date.TimeOfDay;
			if (result < ViewInfo.View.VisibleTime.Start)
				result += DateTimeHelper.DaySpan;
			return result;
		}
		protected internal virtual TimeRulerCurrentTimeAreaItem CreateCurrentTimeArea(TimeRulerViewInfo ruler, Rectangle bounds, int y) {
			TimeRulerCurrentTimeAreaItem item = new TimeRulerCurrentTimeAreaItem();
			AppearanceHelper.Combine(item.Appearance, new AppearanceObject[] { ruler.NowAreaAppearance });
			int height = Painter.CurrentTimeAreaHeight;
			item.Bounds = new Rectangle(bounds.X, y - height, bounds.Width, height);
			return item;
		}
		protected internal virtual TimeRulerCurrentTimelineItem CreateCurrentTimeline(TimeRulerViewInfo ruler, Rectangle bounds, int y) {
			TimeRulerCurrentTimelineItem item = new TimeRulerCurrentTimelineItem();
			AppearanceHelper.Combine(item.Appearance, new AppearanceObject[] { ruler.NowLineAppearance });
			int height = Painter.CurrentTimelineHeight;
			item.Bounds = new Rectangle(bounds.X, y - height, bounds.Width, height);
			return item;
		}
		protected internal virtual Rectangle[] GetRowsBounds() {
			int count = ViewInfo.VisibleRows.Count;
			Rectangle[] result = new Rectangle[count];
			for (int i = 0; i < count; i++)
				result[i] = ViewInfo.VisibleRows[i].Bounds;
			return result;
		}
		protected internal virtual DateTime[] CreateActualTimes(TimeRulerViewInfo ruler) {
			DateTime date = ViewInfo.View.VisibleStart;
			TimeSpan currentUtcOffset = TimeRulerSupport.TimeZoneHelper.ClientTimeZone.GetUtcOffset(date);
			TimeSpan targetUtcOffset = CalcTargetUtcOffset(ruler.Ruler, date);
			return CreateActualTimesCore(currentUtcOffset, targetUtcOffset);
		}
		protected internal virtual DateTime[] CreateActualTimesCore(TimeSpan currentUtcOffset, TimeSpan targetUtcOffset) {
			DayViewRowCollection rows = ViewInfo.VisibleRows;
			int count = rows.Count;
			DateTime[] result = new DateTime[count];
			for (int i = 0; i < count; i++)
				result[i] = CalcActualDate(rows[i].Interval.Start, currentUtcOffset, targetUtcOffset);
			return result;
		}
		protected bool CanShowTimeMarker(TimeMarkerVisibility? timeTimeMarkerVisibility, TimeInterval visibleInterval, DateTime now) {
			if (timeTimeMarkerVisibility == TimeMarkerVisibility.Never)
				return false;
			if (timeTimeMarkerVisibility == TimeMarkerVisibility.Always)
				return true;
			return visibleInterval.Contains(now);
		}
	}
}
