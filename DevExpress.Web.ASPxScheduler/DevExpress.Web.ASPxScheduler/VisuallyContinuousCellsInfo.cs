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
using System.Collections.Generic;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.Web.ASPxScheduler.Drawing {
	public interface IWebTimeIntervalCollection : ITimeIntervalCollection {
		new IWebTimeCell this[int index] { get; }
		string GetIdByIndex(int index);
	}
	public interface IContinuousCellsInfosCalculator {
		ResourceVisuallyContinuousCellsInfosCollection Calculate(ISchedulerWebViewInfoBase viewBase, ResourceBaseCollection resources);
	}
	#region WebVisuallyContinuousCellsInfo
	public class WebVisuallyContinuousCellsInfo : IVisuallyContinuousCellsInfoCore {
		#region Fields
		IWebTimeIntervalCollection cells;
		#endregion
		public WebVisuallyContinuousCellsInfo(IWebTimeIntervalCollection cells) {
			if (cells == null)
				Exceptions.ThrowArgumentException("cells", cells);
			this.cells = cells;
		}
		#region Properties
		public IWebTimeIntervalCollection Cells { get { return cells; } }
		public virtual Resource Resource {
			get {
				if (Count == 0)
					return ResourceBase.Empty;
				else
					return cells[0].Resource;
			}
		}
		public virtual int Count { get { return cells.Count; } }
		public virtual int VisibleCellsCount { get { return cells.Count; } }
		public virtual TimeInterval VisibleInterval { get { return Interval; } }
		public virtual ITimeIntervalCollection VisibleCells { get { return cells; } }
		public virtual TimeInterval Interval {
			get {
				if (Count == 0)
					return TimeInterval.Empty;
				else
					return new TimeInterval(cells[0].Interval.Start, cells[cells.Count - 1].Interval.End);
			}
		}
		IAppointmentViewInfoContainer<IAppointmentViewInfoCollection> IVisuallyContinuousCellsInfoCore.ScrollContainer {
			get { return null; }
		}
		#endregion
		public virtual TimeInterval GetIntervalByIndex(int index) {
			return cells[index].Interval;
		}
		public virtual int GetIndexByStartDate(DateTime date) {
			return cells.BinarySearchStartDate(date);
		}
		public virtual int GetIndexByEndDate(DateTime date) {
			return cells.BinarySearchEndDate(date);
		}
		public virtual int GetNextCellIndexByStartDate(DateTime date) {
			return cells.BinarySearchNextStartDate(date);
		}
		public virtual int GetPreviousCellIndexByEndDate(DateTime date) {
			return cells.BinarySearchPreviousEndDate(date);
		}
		public int GetMaxCellHeight() {
			return 0;
		}
	}
	#endregion
	#region WebVisuallyContinuousCellsInfoCollection
	public class WebVisuallyContinuousCellsInfoCollection : DXCollection<WebVisuallyContinuousCellsInfo> {
	}
	#endregion
	#region WebContinuousCellsInfosCalculator
	public abstract class ContinuousCellsInfosCalculator<TWebView> : IContinuousCellsInfosCalculator {
		public virtual ResourceVisuallyContinuousCellsInfosCollection Calculate(ISchedulerWebViewInfoBase viewBase, ResourceBaseCollection resources) {
			ResourceVisuallyContinuousCellsInfosCollection result = PrepareResult(resources);
			int count = result.Count;
			TWebView view = (TWebView)viewBase;
			for (int i = 0; i < count; i++) {
				VisuallyContinuousCellsInfoCollection cells = CreateGroup(view, i);
				result[i].CellsInfoCollection.AddRange(cells);
			}
			return result;
		}
		protected internal ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			ResourceVisuallyContinuousCellsInfosCollection result = new ResourceVisuallyContinuousCellsInfosCollection();
			int count = resources.Count;
			for (int i = 0; i < count; i++) {
				ResourceVisuallyContinuousCellsInfos infos = new ResourceVisuallyContinuousCellsInfos(resources[i]);
				result.Add(infos);
			}
			return result;
		}
		abstract protected internal VisuallyContinuousCellsInfoCollection CreateGroup(TWebView dayView, int resourceIndx);
	}
	#endregion
	#region DayViewGroupByNoneAllDayAreaVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByNoneAllDayAreaVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WebDayViewGroupByNone> {
		public override ResourceVisuallyContinuousCellsInfosCollection Calculate(ISchedulerWebViewInfoBase view, ResourceBaseCollection resources) {
			return base.Calculate(view, WebWeekViewInfoBase.EmptyResourceCollection);
		}
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WebDayViewGroupByNone dayView, int resourceIndex) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			WebVisuallyContinuousCellsInfo cell = new WebVisuallyContinuousCellsInfo(CreateVisuallyContinuousCellsInfos(dayView.ResourcesCellContainers[0]));
			result.Add(cell);
			return result;
		}
		private IWebTimeIntervalCollection CreateVisuallyContinuousCellsInfos(List<WebDayViewColumn> columns) {
			WebAllDayAreaCellCollection result = new WebAllDayAreaCellCollection();
			int count = columns.Count;
			for (int i = 0; i < count; i++)
				result.Add(columns[i].AllDayAreaCell);
			return result;
		}
	}
	#endregion
	#region DayViewGroupByDateAllDayAreaVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByDateAllDayAreaVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WebDayViewGroupByDate> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WebDayViewGroupByDate dayView, int resourceIndex) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			List<WebDayViewColumn> columns = dayView.ResourcesCellContainers[resourceIndex];
			int count = columns.Count;
			for (int i = 0; i < count; i++) {
				WebVisuallyContinuousCellsInfo cell = new WebVisuallyContinuousCellsInfo(CreateVisuallyContinuousCellsInfos(columns[i]));
				result.Add(cell);
			}
			return result;
		}
		protected internal virtual IWebTimeIntervalCollection CreateVisuallyContinuousCellsInfos(WebDayViewColumn column) {
			WebAllDayAreaCellCollection result = new WebAllDayAreaCellCollection();
			result.Add(column.AllDayAreaCell);
			return result;
		}
	}
	#endregion
	#region DayViewGroupByResourceAllDayAreaVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByResourceAllDayAreaVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WebDayViewGroupByResource> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WebDayViewGroupByResource dayView, int resourceIndex) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			List<WebDayViewColumn> columns = dayView.ResourcesCellContainers[resourceIndex];
			WebVisuallyContinuousCellsInfo cell = new WebVisuallyContinuousCellsInfo(CreateVisuallyContinuousCellsInfos(columns));
			result.Add(cell);
			return result;
		}
		protected internal virtual IWebTimeIntervalCollection CreateVisuallyContinuousCellsInfos(List<WebDayViewColumn> columns) {
			WebAllDayAreaCellCollection result = new WebAllDayAreaCellCollection();
			int count = columns.Count;
			for (int i = 0; i < count; i++)
				result.Add(columns[i].AllDayAreaCell);
			return result;
		}
	}
	#endregion
	#region DayViewGroupByNoneVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByNoneVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WebDayViewGroupByNone> {
		public override ResourceVisuallyContinuousCellsInfosCollection Calculate(ISchedulerWebViewInfoBase view, ResourceBaseCollection resources) {
			return base.Calculate(view, WebWeekViewInfoBase.EmptyResourceCollection);
		}
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WebDayViewGroupByNone dayView, int resourceIndex) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			List<WebDayViewColumn> columns = dayView.ResourcesCellContainers[0];
			int count = columns.Count;
			for (int i = 0; i < count; i++) {
				WebTimeCellCollection cells = columns[i].Cells.Cells;
				WebVisuallyContinuousCellsInfo cell = new WebVisuallyContinuousCellsInfo(cells);
				result.Add(cell);
			}
			return result;
		}
	}
	#endregion
	#region DayViewGroupByDateVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByDateVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WebDayViewGroupByDate> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WebDayViewGroupByDate dayView, int resourceIndex) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			List<WebDayViewColumn> resourceColumns = dayView.ResourcesCellContainers[resourceIndex];
			int count = resourceColumns.Count;
			for (int i = 0; i < count; i++) {
				WebVisuallyContinuousCellsInfo cell = new WebVisuallyContinuousCellsInfo(resourceColumns[i].Cells.Cells);
				result.Add(cell);
			}
			return result;
		}
	}
	#endregion
	#region DayViewGroupByResourceVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByResourceVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WebDayViewGroupByResource> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WebDayViewGroupByResource dayView, int resourceIndex) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			List<WebDayViewColumn> columns = dayView.ResourcesCellContainers[resourceIndex];
			int count = columns.Count;
			for (int i = 0; i < count; i++) {
				WebTimeCellCollection timeCells = columns[i].Cells.Cells;
				WebVisuallyContinuousCellsInfo cell = new WebVisuallyContinuousCellsInfo(timeCells);
				result.Add(cell);
			}
			return result;
		}
	}
	#endregion
	#region WorkWeekViewGroupByNoneVisuallyContinuousCellsInfosCalculator
	public class WorkWeekViewGroupByNoneVisuallyContinuousCellsInfosCalculator : DayViewGroupByNoneVisuallyContinuousCellsInfosCalculator {
	}
	#endregion
	#region WorkWeekViewGroupByResourceVisuallyContinuousCellsInfosCalculator
	public class WorkWeekViewGroupByResourceVisuallyContinuousCellsInfosCalculator : DayViewGroupByResourceVisuallyContinuousCellsInfosCalculator {
	}
	#endregion
	#region WorkWeekViewGroupByDateVisuallyContinuousCellsInfosCalculator
	public class WorkWeekViewGroupByDateVisuallyContinuousCellsInfosCalculator : DayViewGroupByDateVisuallyContinuousCellsInfosCalculator {
	}
	#endregion
	#region WorkWeekViewGroupByNoneAllDayAreaVisuallyContinuousCellsInfosCalculator
	public class WorkWeekViewGroupByNoneAllDayAreaVisuallyContinuousCellsInfosCalculator : DayViewGroupByNoneAllDayAreaVisuallyContinuousCellsInfosCalculator {
	}
	#endregion
	#region WorkWeekViewGroupByResourceAllDayAreaVisuallyContinuousCellsInfosCalculator
	public class WorkWeekViewGroupByResourceAllDayAreaVisuallyContinuousCellsInfosCalculator : DayViewGroupByResourceAllDayAreaVisuallyContinuousCellsInfosCalculator {
	}
	#endregion
	#region WorkWeekViewGroupByDateAllDayAreaVisuallyContinuousCellsInfosCalculator
	public class WorkWeekViewGroupByDateAllDayAreaVisuallyContinuousCellsInfosCalculator : DayViewGroupByDateAllDayAreaVisuallyContinuousCellsInfosCalculator {
	}
	#endregion
	#region WeekViewContinuousCellsInfosCalculatorHelper
	public static class WeekViewContinuousCellsInfosCalculatorHelper {
		static internal VisuallyContinuousCellsInfoCollection CreateContinuousCellsFromHorizontalWeeks(List<WebWeekBase> horizontalWeeks, bool compressWeekend) {
			XtraSchedulerDebug.Assert(horizontalWeeks.Count > 0);
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			int horizontalWeekCount = horizontalWeeks.Count;
			for (int weekIndex = 0; weekIndex < horizontalWeekCount; weekIndex++) {
				WebWeekBase horizontalWeek = horizontalWeeks[weekIndex];
				WebDateCellCollection dates = horizontalWeek.Cells;
				VisuallyContinuousCellsInfoCollection cells = CreateContinuousCellsForHorizontalWeek(dates, compressWeekend);
				result.AddRange(cells);
			}
			return result;
		}
		public static VisuallyContinuousCellsInfoCollection CreateContinuousCellsForVerticalWeek(WebDateCellCollection dates) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			int count = dates.Count;
			for (int i = 0; i < count; i++) {
				WebDateCell date = dates[i];
				WebVisuallyContinuousCellsInfo cell = CreateVisuallyContinuousCellsInfo(date);
				result.Add(cell);
			}
			return result;
		}
		static WebVisuallyContinuousCellsInfo CreateVisuallyContinuousCellsInfo(WebDateCell date) {
			WebDateCellCollection dateCellCollection = new WebDateCellCollection();
			dateCellCollection.Add(date);
			WebVisuallyContinuousCellsInfo cell = new WebVisuallyContinuousCellsInfo(dateCellCollection);
			return cell;
		}
		public static VisuallyContinuousCellsInfoCollection CreateContinuousCellsForHorizontalWeek(WebDateCellCollection dates, bool compressWeekend) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			int count = dates.Count;
			if (!compressWeekend) {
				WebVisuallyContinuousCellsInfo cell = new WebVisuallyContinuousCellsInfo(dates);
				result.Add(cell);
			}
			else {
				int currentDateIndex = 0;
				while (currentDateIndex < count) {
					WebVisuallyContinuousCellsInfo continuousCells = GetFilteredCells(dates, currentDateIndex);
					result.Add(continuousCells);
					currentDateIndex += continuousCells.Count;
				}
			}
			return result;
		}
		static WebVisuallyContinuousCellsInfo GetFilteredCells(WebDateCellCollection dates, int currentDateIndex) {
			WebDateCellCollection filteredDates = new WebDateCellCollection();
			WebDateCell currentDate = dates[currentDateIndex];
			if (currentDate.Interval.Start.DayOfWeek != DayOfWeek.Sunday) {
				int count = dates.Count;
				for (int i = currentDateIndex; i < count; i++) {
					WebDateCell date = dates[i];
					filteredDates.Add(date);
					if (date.Interval.Start.DayOfWeek == DayOfWeek.Saturday)
						break;
				}
			}
			else
				filteredDates.Add(currentDate);
			return new WebVisuallyContinuousCellsInfo(filteredDates);
		}
	}
	#endregion
	#region WeekViewGroupByNoneVisuallyContinuousCellsInfosCalculator
	public class WeekViewGroupByNoneVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WebWeekViewGroupByNone> {
		public override ResourceVisuallyContinuousCellsInfosCollection Calculate(ISchedulerWebViewInfoBase view, ResourceBaseCollection resources) {
			return base.Calculate(view, WebWeekViewInfoBase.EmptyResourceCollection);
		}
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WebWeekViewGroupByNone weekView, int resourceIndex) {
			return WeekViewContinuousCellsInfosCalculatorHelper.CreateContinuousCellsForVerticalWeek(weekView.ResourcesCellContainers[resourceIndex][0].Cells);
		}
	}
	#endregion
	#region WeekViewGroupByDateVisuallyContinuousCellsInfosCalculator
	public class WeekViewGroupByDateVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WebWeekViewGroupByDate> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WebWeekViewGroupByDate view, int resourceIndex) {
			List<WebWeekBase> resourcesWeeks = view.ResourcesCellContainers[resourceIndex];
			return WeekViewContinuousCellsInfosCalculatorHelper.CreateContinuousCellsFromHorizontalWeeks(resourcesWeeks, true);
		}
	}
	#endregion
	#region WeekViewGroupByResourceVisuallyContinuousCellsInfosCalculator
	public class WeekViewGroupByResourceVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WebWeekViewGroupByResource> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WebWeekViewGroupByResource view, int resourceIndex) {
			List<WebWeekBase> resourcesWeeks = view.ResourcesCellContainers[resourceIndex];
			return WeekViewContinuousCellsInfosCalculatorHelper.CreateContinuousCellsForVerticalWeek(resourcesWeeks[0].Cells);
		}
	}
	#endregion
	#region MonthViewGroupByNoneVisuallyContinuousCellsInfosCalculator
	public class MonthViewGroupByNoneVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WebMonthViewGroupByNone> {
		public override ResourceVisuallyContinuousCellsInfosCollection Calculate(ISchedulerWebViewInfoBase view, ResourceBaseCollection resources) {
			return base.Calculate(view, WebWeekViewInfoBase.EmptyResourceCollection);
		}
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WebMonthViewGroupByNone monthView, int resourceIndex) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			List<WebWeekBase> weeks = monthView.ResourcesCellContainers[resourceIndex];
			int count = weeks.Count;
			bool compressWeekend = ((MonthView)monthView.View).CompressWeekend;
			for (int i = 0; i < count; i++) {
				WebWeekBase week = weeks[i];
				VisuallyContinuousCellsInfoCollection weekCells = WeekViewContinuousCellsInfosCalculatorHelper.CreateContinuousCellsForHorizontalWeek(week.Cells, compressWeekend);
				result.AddRange(weekCells);
			}
			return result;
		}
	}
	#endregion
	#region MonthViewGroupByResourceVisuallyContinuousCellsInfosCalculator
	public class MonthViewGroupByResourceVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WebMonthViewGroupByResource> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WebMonthViewGroupByResource monthView, int resourceIndex) {
			List<WebWeekBase> resourceWeeks = monthView.ResourcesCellContainers[resourceIndex];
			return WeekViewContinuousCellsInfosCalculatorHelper.CreateContinuousCellsFromHorizontalWeeks(resourceWeeks, monthView.View.InnerView.CompressWeekendInternal);
		}
	}
	#endregion
	#region MonthViewGroupByDateVisuallyContinuousCellsInfosCalculator
	public class MonthViewGroupByDateVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WebMonthViewGroupByDate> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WebMonthViewGroupByDate view, int resourceIndex) {
			List<WebWeekBase> resourcesWeeks = view.ResourcesCellContainers[resourceIndex];
			return WeekViewContinuousCellsInfosCalculatorHelper.CreateContinuousCellsFromHorizontalWeeks(resourcesWeeks, view.View.InnerView.CompressWeekendInternal);
		}
	}
	#endregion
	#region TimelineViewGroupByNoneVisuallyContinuousCellsInfosCalculator
	public class TimelineViewGroupByNoneVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WebTimelineViewGroupByNone> {
		public override ResourceVisuallyContinuousCellsInfosCollection Calculate(ISchedulerWebViewInfoBase view, ResourceBaseCollection resources) {
			return base.Calculate(view, WebWeekViewInfoBase.EmptyResourceCollection);
		}
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WebTimelineViewGroupByNone timelineView, int resourceIndex) {
			WebTimelineCellCollection cells = timelineView.ResourcesCellContainers[0][0].Cells;
			return VisuallyContinuousCellsInfoHelper.CreateCollectionWithOneItem(cells);
		}
	}
	#endregion
	#region TimelineViewGroupByDateVisuallyContinuousCellsInfosCalculator
	public class TimelineViewGroupByDateVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WebTimelineViewGroupByDate> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WebTimelineViewGroupByDate timelineView, int resourceIndex) {
			WebTimelineCells element = timelineView.ResourcesCellContainers[resourceIndex][0];
			WebTimelineCellCollection cells = element.Cells;
			return VisuallyContinuousCellsInfoHelper.CreateCollectionWithOneItem(cells);
		}
	}
	#endregion
	#region VisuallyContinuousCellsInfoHelper
	public static class VisuallyContinuousCellsInfoHelper {
		public static VisuallyContinuousCellsInfoCollection CreateCollectionWithOneItem(WebTimeCellCollection cells) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			IVisuallyContinuousCellsInfoCore continuousCells = new WebVisuallyContinuousCellsInfo(cells);
			result.Add(continuousCells);
			return result;
		}
		public static VisuallyContinuousCellsInfoCollection CreateCollectionWithOneItem(WebTimelineCellCollection cells) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			IVisuallyContinuousCellsInfoCore continuousCells = new WebVisuallyContinuousCellsInfo(cells);
			result.Add(continuousCells);
			return result;
		}
	}
	#endregion
}
