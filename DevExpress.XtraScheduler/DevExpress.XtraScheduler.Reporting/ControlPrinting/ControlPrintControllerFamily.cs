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
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Reporting.Native {
	#region HorizontalResourceHeadersPrintController
	public class HorizontalResourceHeadersPrintController : ResourcePrintControllerBase {
		public HorizontalResourceHeadersPrintController(ISchedulerResourceProvider resourceProvider)
			: base(resourceProvider) {
		}
		protected internal override ResourcePrintObjectsCollection GetHorizontalPrintObjects() {
			PrintObjectCollection printObjects = CreatePrintObjects(PrintResources);
			return CreateEmptyResourcePrintObjects(printObjects);			
		}		
	}
	#endregion
	#region VerticalResourceHeadersPrintController
	public class VerticalResourceHeadersPrintController : ResourcePrintControllerBase {
		public VerticalResourceHeadersPrintController(ISchedulerResourceProvider resourceProvider)
			: base(resourceProvider) {
		}
		protected internal override ResourcePrintObjectsCollection GetVerticalPrintObjects(AnchorCollection hAnchors) {
			PrintObjectCollection printObjects = CreatePrintObjects(PrintResources);
			return CreateEmptyResourcePrintObjects(printObjects);
		}
		protected internal override AnchorCollection CalculateVerticalMasterAnchors() {
			AnchorCollection anchors = base.CalculateVerticalMasterAnchors();
			ClearInnerAnchors(anchors);
			return anchors;
		}
		protected internal virtual void ClearInnerAnchors(AnchorCollection anchors) {
			foreach (AnchorBase anchor in anchors)
				ClearInnerAnchors(anchor);
		}
		protected internal virtual void ClearInnerAnchors(AnchorBase anchor) {
			anchor.InnerAnchors.Clear();
			AnchorBase innerAnchor = anchor.Clone();
			anchor.InnerAnchors.Add(innerAnchor);
		}		
	}
	#endregion
	#region HorizontalDateHeadersPrintController
	public class HorizontalDateHeadersPrintController : TimeIntervalPrintControllerBase {
		public HorizontalDateHeadersPrintController(ISchedulerTimeIntervalProvider provider)
			: base(provider) {
		}
		public WeekDays VisibleWeekDays { get { return DataCache.VisibleWeekDays; } set { DataCache.VisibleWeekDays = value; } }
		protected internal new DaysIntervalDataCache DataCache { get { return (DaysIntervalDataCache)base.DataCache; } }
		protected internal override ResourcePrintObjectsCollection GetHorizontalPrintObjects() {			
			PrintObjectCollection printObjects = CreatePrintObjects(PrintTimeIntervals);
			return CreateEmptyResourcePrintObjects(printObjects);
		}
		protected override PrintDataCache CreateDataCache() {
			return new DaysIntervalDataCache(TimeIntervalProvider);
		}
	}
	#endregion
	#region TimelineScaleHeadersPrintController
	public class TimelineScaleHeadersPrintController : TimeIntervalPrintControllerBase {
		public TimelineScaleHeadersPrintController(ISchedulerTimeIntervalProvider provider)
			: base(provider) {
		}
		protected internal new TimelineIntervalDataCache DataCache { get { return (TimelineIntervalDataCache)base.DataCache; } }
		public VisibleIntervalsSplitting IntervalsSplitting { get { return DataCache.IntervalsSplitting; } set { DataCache.IntervalsSplitting = value; } }
		public DayOfWeek FirstDayOfWeek { get { return DataCache.FirstDayOfWeek; } set { DataCache.FirstDayOfWeek = value; } }
		protected internal override ResourcePrintObjectsCollection GetHorizontalPrintObjects() {
			PrintObjectCollection printObjects = CreatePrintObjects(PrintTimeIntervals);
			return CreateEmptyResourcePrintObjects(printObjects);
		}
		protected override PrintDataCache CreateDataCache() {
			return new TimelineIntervalDataCache(TimeIntervalProvider);
		}
	}
	#endregion
	#region FullWeekPrintController
	public class FullWeekPrintController : WeekControlBasePrintController {
		public FullWeekPrintController(ISchedulerTimeIntervalProvider provider, ISchedulerResourceProvider resourceProvider)
			: base(provider, resourceProvider) {
		}
		protected internal new WeekControlBaseDataCache DataCache { get { return (WeekControlBaseDataCache)base.DataCache; } }
		protected internal override PrintObjectCollection GetVerticalPrintObjectsCore(AnchorCollection hAnchors) {
			if (PrintWeeks.Count == 0)
				return new PrintObjectCollection();
			TimeIntervalCollection intervals = GetWeekDaysIntervals(PrintWeeks[0]);
			return CreatePrintObjects(intervals);
		}
		protected internal override PrintObjectCollection GetHorizontalPrintObjectsCore() {
			return CreatePrintObjects(PrintWeeks.GetWeekIntervals());
		}		
	}
	#endregion
	#region HorizontalWeekPrintController
	public class HorizontalWeekPrintController : WeekControlBasePrintController {
		public HorizontalWeekPrintController(ISchedulerTimeIntervalProvider provider, ISchedulerResourceProvider resourceProvider)
			: base(provider, resourceProvider) {
		}
		public new HorizontalWeek Control { get { return (HorizontalWeek)base.Control; } }
		protected internal new WeekControlBaseDataCache DataCache { get { return (WeekControlBaseDataCache)base.DataCache; } }
		protected internal override PrintObjectCollection GetHorizontalPrintObjectsCore() {
			if (PrintWeeks.Count == 0)
				return new PrintObjectCollection();
			XtraSchedulerDebug.Assert(PrintWeeks.Count != 0);
			TimeIntervalCollection intervals = GetWeekDaysIntervals(PrintWeeks[0]);
			return CreatePrintObjects(intervals);
		}
		protected internal override PrintObjectCollection GetVerticalPrintObjectsCore(AnchorCollection hAnchors) {
			return CreatePrintObjects(PrintWeeks.GetWeekIntervals());
		}
	}
	#endregion
	#region DayCellsPrintController
	public class DayCellsPrintController : CellsControlPrintControllerBase {		
		TimeSpan timeScale;
		public DayCellsPrintController(ISchedulerTimeIntervalProvider provider, ISchedulerResourceProvider resourceProvider)
			: base(provider, resourceProvider) {
		}		
		protected internal TimeSpan TimeScale { get { return timeScale; } set { timeScale = value; } }
		public new ReportDayView View { get { return (ReportDayView)base.View; } }
		public new DayViewTimeCells Control { get { return (DayViewTimeCells)base.Control; } }
		public WeekDays VisibleWeekDays { get { return DataCache.VisibleWeekDays; } set { DataCache.VisibleWeekDays = value; } }
		protected internal new DaysIntervalDataCache DataCache { get { return (DaysIntervalDataCache)base.DataCache; } }
		protected internal override PrintObjectCollection GetHorizontalPrintObjectsCore() {
			return CreatePrintObjects(PrintTimeIntervals);
		}
		protected internal override PrintObjectCollection GetVerticalPrintObjectsCore(AnchorCollection hAnchors) {
			ReportDayCellsCalculatorHelper helper = new ReportDayCellsCalculatorHelper();
			TimeOfDayInterval visibleTime = CalculateTotalVisibleTime(hAnchors);
			TimeIntervalCollection intervals = helper.CalculateRowIntervals(visibleTime, TimeScale, Control.VisibleTimeSnapMode);
			return CreatePrintObjects(intervals);
		}
		protected internal virtual TimeOfDayInterval CalculateTotalVisibleTime(AnchorCollection hAnchors) {
			TimeOfDayInterval result = TimeOfDayInterval.Empty;
			AnchorCollection columnsAnchors = GetColumnsAnchors(hAnchors);
			int count = columnsAnchors.Count;			
			for (int i = 0; i < count; i++) {
				AnchorBase anchor = columnsAnchors[i];
				TimeOfDayInterval visibleTime = Control.GetColumnVisibleTime(anchor.Interval, anchor.Resource);
				result = MergeIntervals(result, visibleTime);
			}
			return new TimeOfDayInterval(result.Start, result.End);
		}		
		protected internal virtual AnchorCollection GetColumnsAnchors(AnchorCollection hAnchors) {
			AnchorCollection result = new AnchorCollection();
			int count = hAnchors.Count;
			for (int i = 0; i < count; i++)
				result.AddRange(hAnchors[i].InnerAnchors);
			return result;
		}
		protected internal virtual TimeOfDayInterval MergeIntervals(TimeOfDayInterval interval1, TimeOfDayInterval interval2) {
			if (interval1.Equals(TimeOfDayInterval.Empty))
				return interval2;
			TimeSpan start = TimeSpan.Compare(interval1.Start, interval2.Start) < 0 ? interval1.Start : interval2.Start;
			TimeSpan end = TimeSpan.Compare(interval1.End, interval2.End) > 0 ? interval1.End : interval2.End;
			return new TimeOfDayInterval(start, end);
	   }
		protected override PrintDataCache CreateDataCache() {
			return new DaysIntervalDataCache(TimeIntervalProvider);
		}
	}
	#endregion
	#region TimeRulerPrintController
	public class TimeRulerPrintController : TimeIntervalPrintControllerBase {
		public TimeRulerPrintController(ISchedulerTimeIntervalProvider provider)
			: base(provider) {
		}
		public new DayViewTimeRuler Control { get { return (DayViewTimeRuler)base.Control; } }
		protected internal DayViewTimeCells TimeCells { get { return (DayViewTimeCells)Control.LayoutOptionsVertical.MasterControl; } }
		protected internal override ResourcePrintObjectsCollection GetVerticalPrintObjects(AnchorCollection hAnchors) {
			PrintObjectCollection objects = CreateDefaultVerticalPrintObjects();
			return CreateEmptyResourcePrintObjects(objects);
		}
		private PrintObjectCollection CreateDefaultVerticalPrintObjects() {
			ReportDayCellsCalculatorHelper helper = new ReportDayCellsCalculatorHelper();
			TimeIntervalCollection intervals = helper.CalculateRowIntervals(DayViewTimeCells.GetDefaultVisibleTime(), DayViewTimeCells.DefaultTimeScale, DayViewTimeCells.DefaultVisibleTimeSnapMode);
			return CreatePrintObjects(intervals);
		}
		protected internal virtual DateTime GetCurrentVisibleStart() {
			DateTime result = DateTime.Now;
			if (TimeCells != null) {
				AnchorCollection cellAnchors = TimeCells.ActualHorizontalAnchors;
				if (cellAnchors.Count > 0)
					result = cellAnchors[0].Interval.Start;
			}
			return result;
		}
	}
	#endregion
	public class TimelineCellsPrintController : CellsControlPrintControllerBase {		
		public TimelineCellsPrintController(ISchedulerTimeIntervalProvider provider, ISchedulerResourceProvider resourceProvider)
			: base(provider, resourceProvider) {
		}
		public VisibleIntervalsSplitting IntervalsSplitting { get { return DataCache.IntervalsSplitting; } set { DataCache.IntervalsSplitting = value; } }
		public DayOfWeek FirstDayOfWeek { get { return DataCache.FirstDayOfWeek; } set { DataCache.FirstDayOfWeek = value; } }
		public new ReportTimelineView View { get { return (ReportTimelineView)base.View; } }
		public new TimelineCells Control { get { return (TimelineCells)base.Control; } }
		protected internal new TimelineIntervalDataCache DataCache { get { return (TimelineIntervalDataCache)base.DataCache; } }
		protected internal override PrintObjectCollection GetHorizontalPrintObjectsCore() {
			return CreatePrintObjects(PrintTimeIntervals);
		}
		protected internal override PrintObjectCollection GetVerticalPrintObjectsCore(AnchorCollection hAnchors) {
			PrintObjectCollection printObjects = new PrintObjectCollection();
			printObjects.Add(new PrintObject(PrintTimeIntervals.Interval));
			return printObjects;
		}
		protected override PrintDataCache CreateDataCache() {
			return new TimelineIntervalDataCache(TimeIntervalProvider);
		}
	}
	public class DayOfWeekHeadersPrintController : TimeIntervalPrintControllerBase {
		public DayOfWeekHeadersPrintController(ISchedulerTimeIntervalProvider provider)
			: base(provider) {
		}
		public new DayOfWeekHeaders Control { get { return (DayOfWeekHeaders)base.Control; } }
		protected internal override ResourcePrintObjectsCollection GetHorizontalPrintObjects() {
			PrintObjectCollection printObjects = CreateDefaultHorizontalPrintObjects();
			return CreateEmptyResourcePrintObjects(printObjects);
		}
		private static PrintObjectCollection CreateDefaultHorizontalPrintObjects() {
			DateTime[] weekDates = DateTimeHelper.GetWeekDates(DateTime.Now, DayOfWeek.Sunday, false);
			PrintObjectCollection result = new PrintObjectCollection();
			int count = weekDates.Length;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = new TimeInterval(weekDates[i], TimeSpan.FromDays(1));
				result.Add(new PrintObject(interval));
			}
			return result;
		}
	}
}
