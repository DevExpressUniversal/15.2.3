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
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Implementations;
#if WPF || SL
using DevExpress.XtraScheduler.Internal.Diagnostics;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public interface IContinuousCellsInfosCalculator {
		ResourceVisuallyContinuousCellsInfosCollection Calculate(ISchedulerViewInfoBase viewBase, ResourceBaseCollection resources);
	}
	#region WebVisuallyContinuousCellsInfo
	public class VisuallyContinuousCellsInfo : IVisuallyContinuousCellsInfoCore {
		#region Fields
		ITimeIntervalCollection cells;
		int originalFirstCellIndex;
		#endregion
		public VisuallyContinuousCellsInfo(ITimeIntervalCollection cells, int originalFirstCellIndex) {
			Guard.ArgumentNotNull(cells, "cells");
			Guard.ArgumentNonNegative(originalFirstCellIndex, "originalFirstCellIndex");
			this.cells = cells;
			this.originalFirstCellIndex = originalFirstCellIndex;
		}
		#region Properties
		public ITimeIntervalCollection Cells { get { return cells; } }
		public int OriginalFirstCellIndex { get { return originalFirstCellIndex; } }
		public virtual Resource Resource {
			get {
				if(Count == 0)
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
				if(Count == 0)
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
	public class WebVisuallyContinuousCellsInfoCollection : DXCollection<VisuallyContinuousCellsInfo> {
	}
	#endregion
	#region WebContinuousCellsInfosCalculator
	public abstract class ContinuousCellsInfosCalculator<TWebView> : IContinuousCellsInfosCalculator {
		public virtual ResourceVisuallyContinuousCellsInfosCollection Calculate(ISchedulerViewInfoBase viewBase, ResourceBaseCollection resources) {
			ResourceVisuallyContinuousCellsInfosCollection result = PrepareResult(resources);
			int count = result.Count;
			TWebView view = (TWebView)viewBase;
			for(int i = 0; i < count; i++) {
				VisuallyContinuousCellsInfoCollection cells = CreateGroup(view, i);
				result[i].CellsInfoCollection.AddRange(cells);
			}
			return result;
		}
		protected internal ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			ResourceVisuallyContinuousCellsInfosCollection result = new ResourceVisuallyContinuousCellsInfosCollection();
			int count = resources.Count;
			for(int i = 0; i < count; i++) {
				ResourceVisuallyContinuousCellsInfos infos = new ResourceVisuallyContinuousCellsInfos(resources[i]);
				result.Add(infos);
			}
			return result;
		}
		abstract protected internal VisuallyContinuousCellsInfoCollection CreateGroup(TWebView dayView, int resourceIndx);
	}
	#endregion
	#region DayViewGroupByNoneAllDayAreaVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByNoneAllDayAreaVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<DayViewGroupByNone> {
		public override ResourceVisuallyContinuousCellsInfosCollection Calculate(ISchedulerViewInfoBase view, ResourceBaseCollection resources) {
			return base.Calculate(view, WeekViewInfoBase.EmptyResourceCollection);
		}
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(DayViewGroupByNone dayView, int resourceIndex) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			VisuallyContinuousCellsInfo cell = new VisuallyContinuousCellsInfo(CreateVisuallyContinuousCellsInfos(dayView.ResourcesCellContainers[0]), 0);
			result.Add(cell);
			return result;
		}
		private ITimeIntervalCollection CreateVisuallyContinuousCellsInfos(AssignableCollection<DayViewColumn> columns) {
			AllDayAreaCellCollection result = new AllDayAreaCellCollection();
			int count = columns.Count;
			for (int i = 0; i < count; i++)
				result.Add(columns[i].AllDayAreaCell);
			return result;
		}
	}
	#endregion
	#region DayViewGroupByDateAllDayAreaVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByDateAllDayAreaVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<DayViewGroupByDate> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(DayViewGroupByDate dayView, int resourceIndex) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			foreach(SingleDayViewInfo dayViewInfo in dayView.DaysContainers) {
				SingleResourceViewInfo resourceViewInfo = dayViewInfo.SingleResourceViewInfoCollection[resourceIndex];
				foreach(ICellContainer cellContainer in resourceViewInfo.CellContainers) {
					AllDayAreaCellsContainter allDayAreaContainer = cellContainer as AllDayAreaCellsContainter;
					if(allDayAreaContainer == null)
						continue;
					VisuallyContinuousCellsInfo cellsInfo = new VisuallyContinuousCellsInfo(allDayAreaContainer.Cells, 0);
					result.Add(cellsInfo);
				}
			}
			return result;
		}
		protected internal virtual ITimeIntervalCollection CreateVisuallyContinuousCellsInfos(DayViewColumn column) {
			AllDayAreaCellCollection result = new AllDayAreaCellCollection();
			result.Add(column.AllDayAreaCell);
			return result;
		}
	}
	#endregion
	#region DayViewGroupByResourceAllDayAreaVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByResourceAllDayAreaVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<DayViewGroupByResource> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(DayViewGroupByResource dayView, int resourceIndex) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			AssignableCollection<DayViewColumn> columns = dayView.ResourcesCellContainers[resourceIndex];
			VisuallyContinuousCellsInfo cell = new VisuallyContinuousCellsInfo(CreateVisuallyContinuousCellsInfos(columns), 0);
			result.Add(cell);
			return result;
		}
		protected internal virtual ITimeIntervalCollection CreateVisuallyContinuousCellsInfos(AssignableCollection<DayViewColumn> columns) {
			AllDayAreaCellCollection result = new AllDayAreaCellCollection();
			int count = columns.Count;
			for(int i = 0; i < count; i++)
				result.Add(columns[i].AllDayAreaCell);
			return result;
		}
	}
	#endregion
	#region DayViewGroupByNoneVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByNoneVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<DayViewInfoBase> {
		public override ResourceVisuallyContinuousCellsInfosCollection Calculate(ISchedulerViewInfoBase view, ResourceBaseCollection resources) {
			return base.Calculate(view, WeekViewInfoBase.EmptyResourceCollection);
		}
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(DayViewInfoBase dayView, int resourceIndex) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			AssignableCollection<DayViewColumn> columns = dayView.ResourcesCellContainers[resourceIndex];
			int count = columns.Count;
			for (int i = 0; i < count; i++) {
				TimeCellCollection cells = columns[i].Cells;
				VisuallyContinuousCellsInfo cell = new VisuallyContinuousCellsInfo(cells, 0);
				result.Add(cell);
			}
			return result;
		}
	}
	#endregion
	#region DayViewGroupByDateVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByDateVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<DayViewGroupByDate> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(DayViewGroupByDate dayView, int resourceIndex) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			foreach(SingleDayViewInfo dayViewInfo in dayView.DaysContainers) {
				SingleResourceViewInfo resourceViewInfo = dayViewInfo.SingleResourceViewInfoCollection[resourceIndex];
				foreach(ICellContainer cellContainer in resourceViewInfo.CellContainers) {
					DayViewColumn dayColumn = cellContainer as DayViewColumn;
					if(dayColumn == null)
						continue;
					VisuallyContinuousCellsInfo cellsInfo = new VisuallyContinuousCellsInfo(dayColumn.Cells, 0);
					result.Add(cellsInfo);
				}
			}
			return result;
		}
	}
	#endregion
	#region DayViewGroupByResourceVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByResourceVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<DayViewGroupByResource> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(DayViewGroupByResource dayView, int resourceIndex) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			AssignableCollection<DayViewColumn> columns = dayView.ResourcesCellContainers[resourceIndex];
			int count = columns.Count;
			for(int i = 0; i < count; i++) {
				TimeCellCollection timeCells = columns[i].Cells;
				VisuallyContinuousCellsInfo cell = new VisuallyContinuousCellsInfo(timeCells, 0);
				result.Add(cell);
			}
			return result;
		}
	}
	#endregion
	#region WeekViewContinuousCellsInfosCalculatorHelper
	public class WeekViewContinuousCellsInfosCalculatorHelper {
		static internal VisuallyContinuousCellsInfoCollection CreateContinuousCellsFromHorizontalWeeks(AssignableCollection<WeekBase> horizontalWeeks, bool compressWeekend) {
			XtraSchedulerDebug.Assert(horizontalWeeks.Count > 0);
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			int horizontalWeekCount = horizontalWeeks.Count;
			for(int weekIndex = 0; weekIndex < horizontalWeekCount; weekIndex++) {
				WeekBase horizontalWeek = horizontalWeeks[weekIndex];
				DateCellCollection dates = horizontalWeek.Cells;
				VisuallyContinuousCellsInfoCollection cells = CreateContinuousCellsForHorizontalWeek(dates, compressWeekend);
				result.AddRange(cells);
			}
			return result;
		}
		public static VisuallyContinuousCellsInfoCollection CreateContinuousCellsForVerticalWeek(DateCellCollection dates) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			int count = dates.Count;
			for(int i = 0; i < count; i++) {
				DateCell date = dates[i];
				VisuallyContinuousCellsInfo cell = CreateVisuallyContinuousCellsInfo(date, i);
				result.Add(cell);
			}
			return result;
		}
		static VisuallyContinuousCellsInfo CreateVisuallyContinuousCellsInfo(DateCell date, int originalFirstCellIndex) {
			DateCellCollection dateCellCollection = new DateCellCollection();
			dateCellCollection.Add(date);
			VisuallyContinuousCellsInfo cell = new VisuallyContinuousCellsInfo(dateCellCollection, originalFirstCellIndex);
			return cell;
		}
		public static VisuallyContinuousCellsInfoCollection CreateContinuousCellsForHorizontalWeek(DateCellCollection dates, bool compressWeekend) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			int count = dates.Count;
			if(!compressWeekend) {
				VisuallyContinuousCellsInfo cell = new VisuallyContinuousCellsInfo(dates, 0);
				result.Add(cell);
			} else {
				int currentDateIndex = 0;
				while(currentDateIndex < count) {
					VisuallyContinuousCellsInfo continuousCells = GetFilteredCells(dates, currentDateIndex);
					result.Add(continuousCells);
					currentDateIndex += continuousCells.Count;
				}
			}
			return result;
		}
		static VisuallyContinuousCellsInfo GetFilteredCells(DateCellCollection dates, int currentDateIndex) {
			DateCellCollection filteredDates = new DateCellCollection();
			DateCell currentDate = dates[currentDateIndex];
			if(currentDate.Interval.Start.DayOfWeek != DayOfWeek.Sunday) {
				int count = dates.Count;
				for(int i = currentDateIndex; i < count; i++) {
					DateCell date = dates[i];
					filteredDates.Add(date);
					if(date.Interval.Start.DayOfWeek == DayOfWeek.Saturday)
						break;
				}
			} else
				filteredDates.Add(currentDate);
			return new VisuallyContinuousCellsInfo(filteredDates, currentDateIndex);
		}
	}
	#endregion
	#region WeekViewGroupByNoneVisuallyContinuousCellsInfosCalculator
	public class WeekViewGroupByNoneVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WeekViewGroupByNone> {
		public override ResourceVisuallyContinuousCellsInfosCollection Calculate(ISchedulerViewInfoBase view, ResourceBaseCollection resources) {
			return base.Calculate(view, WeekViewInfoBase.EmptyResourceCollection);
		}
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WeekViewGroupByNone weekView, int resourceIndex) {
			return WeekViewContinuousCellsInfosCalculatorHelper.CreateContinuousCellsForVerticalWeek(weekView.ResourcesCellContainers[resourceIndex][0].Cells);
		}
	}
	#endregion
	#region WeekViewGroupByDateVisuallyContinuousCellsInfosCalculator
	public class WeekViewGroupByDateVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WeekViewGroupByDate> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WeekViewGroupByDate view, int resourceIndex) {
			AssignableCollection<WeekBase> resourcesWeeks = view.ResourcesCellContainers[resourceIndex];
			return WeekViewContinuousCellsInfosCalculatorHelper.CreateContinuousCellsFromHorizontalWeeks(resourcesWeeks, true);
		}
	}
	#endregion
	#region WeekViewGroupByResourceVisuallyContinuousCellsInfosCalculator
	public class WeekViewGroupByResourceVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<WeekViewGroupByResource> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(WeekViewGroupByResource view, int resourceIndex) {
			AssignableCollection<WeekBase> resourcesWeeks = view.ResourcesCellContainers[resourceIndex];
			return WeekViewContinuousCellsInfosCalculatorHelper.CreateContinuousCellsForVerticalWeek(resourcesWeeks[0].Cells);
		}
	}
	#endregion
	#region MonthViewGroupByNoneVisuallyContinuousCellsInfosCalculator
	public class MonthViewGroupByNoneVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<MonthViewGroupByNone> {
		public override ResourceVisuallyContinuousCellsInfosCollection Calculate(ISchedulerViewInfoBase view, ResourceBaseCollection resources) {
			return base.Calculate(view, WeekViewInfoBase.EmptyResourceCollection);
		}
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(MonthViewGroupByNone monthView, int resourceIndex) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			AssignableCollection<WeekBase> weeks = monthView.ResourcesCellContainers[resourceIndex];
			int count = weeks.Count;
			bool compressWeekend = ((MonthView)monthView.View).CompressWeekend;
			for(int i = 0; i < count; i++) {
				WeekBase week = weeks[i];
				VisuallyContinuousCellsInfoCollection weekCells = WeekViewContinuousCellsInfosCalculatorHelper.CreateContinuousCellsForHorizontalWeek(week.Cells, compressWeekend);
				result.AddRange(weekCells);
			}
			return result;
		}
	}
	#endregion
	#region MonthViewGroupByResourceVisuallyContinuousCellsInfosCalculator
	public class MonthViewGroupByResourceVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<MonthViewGroupByResource> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(MonthViewGroupByResource monthView, int resourceIndex) {
			AssignableCollection<WeekBase> resourceWeeks = monthView.ResourcesCellContainers[resourceIndex];
			return WeekViewContinuousCellsInfosCalculatorHelper.CreateContinuousCellsFromHorizontalWeeks(resourceWeeks, monthView.View.InnerView.CompressWeekendInternal);
		}
	}
	#endregion
	#region MonthViewGroupByDateVisuallyContinuousCellsInfosCalculator
	public class MonthViewGroupByDateVisuallyContinuousCellsInfosCalculator : ContinuousCellsInfosCalculator<MonthViewGroupByDate> {
		protected internal override VisuallyContinuousCellsInfoCollection CreateGroup(MonthViewGroupByDate view, int resourceIndex) {
			AssignableCollection<WeekBase> resourcesWeeks = view.ResourcesCellContainers[resourceIndex];
			return WeekViewContinuousCellsInfosCalculatorHelper.CreateContinuousCellsFromHorizontalWeeks(resourcesWeeks, view.View.InnerView.CompressWeekendInternal);
		}
	}
	#endregion
	#region VisuallyContinuousCellsInfoHelper
	public class VisuallyContinuousCellsInfoHelper {
		public static VisuallyContinuousCellsInfoCollection CreateCollectionWithOneItem(TimeCellCollection cells) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			IVisuallyContinuousCellsInfoCore continuousCells = new VisuallyContinuousCellsInfo(cells, 0);
			result.Add(continuousCells);
			return result;
		}
	}
	#endregion
}
