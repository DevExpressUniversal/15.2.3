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
using System.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Drawing {
	#region IVisuallyContinuousCellsInfo
	public interface IVisuallyContinuousCellsInfo : IVisuallyContinuousCellsInfoCore {
		SchedulerViewCellBaseCollection VisibleCells { get; }
		Rectangle GetContentBounds(int index);
		Rectangle GetTotalBounds();
		new SchedulerViewCellContainer ScrollContainer { get; }
		MoreButton[] MoreButtons { get; }
	}
	#endregion
	#region VisuallyContinuousCellsInfo
	public class VisuallyContinuousCellsInfo : IVisuallyContinuousCellsInfo {
		#region Fields
		SchedulerViewCellBaseCollection cells;
		SchedulerViewCellContainer scrollContainer;
		MoreButton[] moreButtons;
		#endregion
		public VisuallyContinuousCellsInfo(SchedulerViewCellBaseCollection cells, SchedulerViewCellContainer scrollContainer) {
			if (cells == null)
				Exceptions.ThrowArgumentException("cells", cells);
			if (scrollContainer == null)
				Exceptions.ThrowArgumentNullException("scrollContainer");
			this.cells = cells;
			this.scrollContainer = scrollContainer;
			CreateMoreButtons(cells.Count);
		}
		private void CreateMoreButtons(int count) {
			this.moreButtons = new MoreButton[count];
			for (int i = 0; i < count; i++) {
				this.moreButtons[i] = MoreButton.Empty;
			}
		}
		#region IVisuallyContinuousCellsInfoCore implementation
		public Resource Resource {
			get {
				if (cells.Count == 0)
					return ResourceBase.Empty;
				else
					return cells[0].Resource;
			}
		}
		public int VisibleCellsCount { get { return VisibleCells.Count; } }
		public int Count { get { return cells.Count; } }
		public TimeInterval Interval {
			get {
				if (cells.Count == 0)
					return TimeInterval.Empty;
				else
					return new TimeInterval(cells[0].Interval.Start, cells[cells.Count - 1].Interval.End);
			}
		}
		public TimeInterval VisibleInterval { get { return Interval; } }
		public TimeInterval GetIntervalByIndex(int index) {
			return cells[index].Interval;
		}
		public int GetIndexByStartDate(DateTime date) {
			return cells.BinarySearchStartDate(date);
		}
		public int GetIndexByEndDate(DateTime date) {
			return cells.BinarySearchEndDate(date);
		}
		public int GetNextCellIndexByStartDate(DateTime date) {
			return cells.BinarySearchNextStartDate(date);
		}
		public int GetPreviousCellIndexByEndDate(DateTime date) {
			return cells.BinarySearchPreviousEndDate(date);
		}
		#endregion
		#region IVisuallyContinuousCellsInfo Implementation
		public MoreButton[] MoreButtons { get { return moreButtons; } }
		public SchedulerViewCellBaseCollection VisibleCells { get { return cells; } }
		public Rectangle GetContentBounds(int index) {
			return GetContentBoundsInternal(index);
		}
		protected internal virtual Rectangle GetContentBoundsInternal(int index) {
			return cells[index].ContentBounds;
		}
		public virtual Rectangle GetTotalBounds() {
			if (Count == 0)
				return Rectangle.Empty;
			Rectangle firstCell = GetContentBounds(0);
			Rectangle lastCell = GetContentBounds(Count - 1);
			XtraSchedulerDebug.Assert(firstCell.Top == lastCell.Top);
			int bottom = Math.Max(firstCell.Bottom, lastCell.Bottom);
			return Rectangle.FromLTRB(firstCell.Left, firstCell.Top, lastCell.Right, bottom);
		}
		public SchedulerViewCellContainer ScrollContainer {
			get { return scrollContainer; }
		}
		IAppointmentViewInfoContainer<IAppointmentViewInfoCollection> IVisuallyContinuousCellsInfoCore.ScrollContainer {
			get { return scrollContainer; }
		}
		public int GetMaxCellHeight() {
			int count = this.cells.Count;
			int maxCellHeight = 0;
			for (int i = 0; i < count; i++) {
				maxCellHeight = Math.Max(maxCellHeight, this.cells[i].ContentBounds.Height);
			}
			return maxCellHeight;
		}
		#endregion
	}
	#endregion
	public class DayViewVisuallyContinuousCellsInfo : IVisuallyContinuousCellsInfo {
		#region Fields
		DayViewColumn column;
		DayViewRowCollection rows;
		MoreButton[] moreButtons;
		#endregion
		public DayViewVisuallyContinuousCellsInfo(DayViewRowCollection rows, DayViewColumn column) {
			if (rows == null)
				Exceptions.ThrowArgumentException("rows", rows);
			if (column == null)
				Exceptions.ThrowArgumentException("column", column);
			this.rows = rows;
			this.column = column;
			CreateMoreButtons(2);
		}
		private void CreateMoreButtons(int count) {
			this.moreButtons = new MoreButton[count];
			for (int i = 0; i < count; i++) {
				this.moreButtons[i] = MoreButton.Empty;
			}
		}
		#region Properties
		public virtual DayViewRowCollection Rows { get { return rows; } set { rows = value; } }
		public virtual DayViewColumn Column { get { return column; } }
		public SchedulerViewCellContainer ScrollContainer { get { return Column; } }
		public MoreButton[] MoreButtons { get { return moreButtons; } }
		#endregion
		#region IVisuallyContinuousCellsInfo implementation
		public SchedulerViewCellBaseCollection VisibleCells { get { return Column.Cells; } }
		public Rectangle GetContentBounds(int index) {
			Rectangle rowsBounds = Rows[index].Bounds;
			return Rectangle.FromLTRB(column.ContentBounds.Left, rowsBounds.Top, column.ContentBounds.Right, rowsBounds.Bottom);
		}
		public virtual Rectangle GetTotalBounds() {
			if (Count == 0)
				return Rectangle.Empty;
			Rectangle firstCell = GetContentBounds(0);
			Rectangle lastCell = GetContentBounds(Count - 1);
			XtraSchedulerDebug.Assert(firstCell.Left == lastCell.Left);
			XtraSchedulerDebug.Assert(firstCell.Right == lastCell.Right);
			return Rectangle.FromLTRB(firstCell.Left, firstCell.Top, lastCell.Right, lastCell.Bottom);
		}
		#endregion
		#region IVisuallyContinuousCellsInfoCore implementation
		public Resource Resource { get { return column.Resource; } }
		public int Count { get { return rows.Count; } }
		public int VisibleCellsCount { get { return VisibleCells.Count; } }
		public TimeInterval VisibleInterval { get { return column.VisibleInterval; } }
		public TimeInterval Interval {
			get {
				if (rows.Count == 0)
					return TimeInterval.Empty;
				else
					return new TimeInterval(column.Interval.Start + rows[0].Interval.Start, column.Interval.Start + rows[rows.Count - 1].Interval.End);
			}
		}
		IAppointmentViewInfoContainer<IAppointmentViewInfoCollection> IVisuallyContinuousCellsInfoCore.ScrollContainer {
			get { return Column; }
		}
		public TimeInterval GetIntervalByIndex(int index) {
			TimeOfDayInterval rowInterval = rows[index].Interval;
			return new TimeInterval(column.Interval.Start + rowInterval.Start, rowInterval.Duration);
		}
		public int GetIndexByStartDate(DateTime date) {
			return rows.BinarySearchStartDate(date - column.Interval.Start);
		}
		public int GetIndexByEndDate(DateTime date) {
			return rows.BinarySearchEndDate(date - column.Interval.Start);
		}
		public int GetNextCellIndexByStartDate(DateTime date) {
			return Math.Min(rows.Count - 1, rows.BinarySearchStartDate(date - column.Interval.Start) + 1);
		}
		public int GetPreviousCellIndexByEndDate(DateTime date) {
			return Math.Max(0, rows.BinarySearchEndDate(date - column.Interval.Start) - 1);
		}
		public int GetMaxCellHeight() {
			return 0;
		}
		#endregion
	}
	#region VisuallyContinuousCellsInfosCalculator (abstract class)
	public abstract class VisuallyContinuousCellsInfosCalculator {
		public virtual ResourceVisuallyContinuousCellsInfosCollection Calculate(SchedulerViewInfoBase viewInfo, ResourceBaseCollection resources) {
			return Calculate(viewInfo, viewInfo.CellContainers, resources);
		}
		public virtual ResourceVisuallyContinuousCellsInfosCollection Calculate(SchedulerViewInfoBase viewInfo, SchedulerViewCellContainerCollection cellContainers, ResourceBaseCollection resources) {
			ResourceVisuallyContinuousCellsInfosCollection result = PrepareResult(resources);
			int count = result.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewCellContainerCollection containers = FilterCellContainers(cellContainers, count, i);
				result[i].CellsInfoCollection.AddRange(CreateVisuallyContinuousCellsInfos(viewInfo, containers));
			}
			return result;
		}
		protected internal virtual ResourceVisuallyContinuousCellsInfosCollection PrepareResultGroupByNone(ResourceBaseCollection resources) {
			ResourceVisuallyContinuousCellsInfosCollection result = new ResourceVisuallyContinuousCellsInfosCollection();
			ResourceVisuallyContinuousCellsInfos infos = new ResourceVisuallyContinuousCellsInfos(ResourceBase.Empty);
			result.Add(infos);
			return result;
		}
		protected internal virtual ResourceVisuallyContinuousCellsInfosCollection PrepareResultGroupByDate(ResourceBaseCollection resources) {
			ResourceVisuallyContinuousCellsInfosCollection result = new ResourceVisuallyContinuousCellsInfosCollection();
			int count = resources.Count;
			for (int i = 0; i < count; i++) {
				ResourceVisuallyContinuousCellsInfos infos = new ResourceVisuallyContinuousCellsInfos(resources[i]);
				result.Add(infos);
			}
			return result;
		}
		protected internal virtual ResourceVisuallyContinuousCellsInfosCollection PrepareResultGroupByResource(ResourceBaseCollection resources) {
			return PrepareResultGroupByDate(resources);
		}
		protected internal virtual VisuallyContinuousCellsInfoCollection CreateVisuallyContinuousCellsInfos(SchedulerViewInfoBase viewInfo, SchedulerViewCellContainerCollection containers) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			int count = containers.Count;
			for (int i = 0; i < count; i++)
				result.AddRange(CreateContainerVisuallyContinuousCellsInfos(viewInfo, containers[i]));
			return result;
		}
		protected internal virtual SchedulerViewCellContainerCollection FilterCellContainersGroupByNone(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return sourceCellContainers;
		}
		protected internal virtual SchedulerViewCellContainerCollection FilterCellContainersGroupByResource(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			int containerCountByResource = sourceCellContainers.Count / resourceCount;
			int startIndex = containerCountByResource * resourceIndex;
			int endIndex = startIndex + containerCountByResource - 1;
			SchedulerViewCellContainerCollection result = new SchedulerViewCellContainerCollection();
			for (int i = startIndex; i <= endIndex; i++)
				result.Add(sourceCellContainers[i]);
			return result;
		}
		protected internal virtual SchedulerViewCellContainerCollection FilterCellContainersGroupByDate(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByResource(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal abstract ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources);
		protected internal abstract SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex);
		protected internal abstract VisuallyContinuousCellsInfoCollection CreateContainerVisuallyContinuousCellsInfos(SchedulerViewInfoBase viewInfo, SchedulerViewCellContainer container);
	}
	#endregion
	#region DayViewVisuallyContinuousCellsInfosCalculator (abstract class)
	public abstract class DayViewVisuallyContinuousCellsInfosCalculator : VisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainersGroupByDate(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			int dateCount = sourceCellContainers.Count / resourceCount;
			int endIndex = resourceIndex + (dateCount - 1) * resourceCount;
			SchedulerViewCellContainerCollection result = new SchedulerViewCellContainerCollection();
			for (int i = resourceIndex; i <= endIndex; i += resourceCount)
				result.Add(sourceCellContainers[i]);
			return result;
		}
		protected internal virtual VisuallyContinuousCellsInfo CreateCellsInfo(SchedulerViewInfoBase viewInfo, SchedulerViewCellBaseCollection cells) {
			DayViewInfo dayViewInfo = (DayViewInfo)viewInfo;
			return new VisuallyContinuousCellsInfo(cells, dayViewInfo.AllDayAreaScrollContainer);
		}
	}
	#endregion
	#region DayViewTimeCellsVisuallyContinuousCellsInfosCalculator (abstract class)
	public abstract class DayViewTimeCellsVisuallyContinuousCellsInfosCalculator : DayViewVisuallyContinuousCellsInfosCalculator {
		protected internal override VisuallyContinuousCellsInfoCollection CreateContainerVisuallyContinuousCellsInfos(SchedulerViewInfoBase viewInfo, SchedulerViewCellContainer container) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			result.Add(new DayViewVisuallyContinuousCellsInfo(((DayViewInfo)viewInfo).PreliminaryLayoutResult.Rows, (DayViewColumn)container));
			return result;
		}
	}
	#endregion
	#region DayViewGroupByNoneAllDayAreaVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByNoneAllDayAreaVisuallyContinuousCellsInfosCalculator : DayViewVisuallyContinuousCellsInfosCalculator {
		protected internal override VisuallyContinuousCellsInfoCollection CreateVisuallyContinuousCellsInfos(SchedulerViewInfoBase viewInfo, SchedulerViewCellContainerCollection containers) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			SchedulerViewCellBaseCollection cells = new SchedulerViewCellBaseCollection();
			int count = containers.Count;
			for (int i = 0; i < count; i++) {
				DayViewColumn column = (DayViewColumn)containers[i];
				cells.Add(column.AllDayAreaCell);
			}
			result.Add(CreateCellsInfo(viewInfo, cells));
			return result;
		}
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByNone(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override VisuallyContinuousCellsInfoCollection CreateContainerVisuallyContinuousCellsInfos(SchedulerViewInfoBase viewInfo, SchedulerViewCellContainer container) {
			return null;
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByNone(resources);
		}
	}
	#endregion
	#region DayViewGroupByResourceAllDayAreaVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByResourceAllDayAreaVisuallyContinuousCellsInfosCalculator : DayViewGroupByNoneAllDayAreaVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByResource(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByResource(resources);
		}
	}
	#endregion
	#region DayViewGroupByDateAllDayAreaVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByDateAllDayAreaVisuallyContinuousCellsInfosCalculator : DayViewVisuallyContinuousCellsInfosCalculator {
		protected internal override VisuallyContinuousCellsInfoCollection CreateContainerVisuallyContinuousCellsInfos(SchedulerViewInfoBase viewInfo, SchedulerViewCellContainer container) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			DayViewColumn column = (DayViewColumn)container;
			SchedulerViewCellBaseCollection cells = new SchedulerViewCellBaseCollection();
			cells.Add(column.AllDayAreaCell);
			result.Add(CreateCellsInfo(viewInfo, cells));
			return result;
		}
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByDate(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByDate(resources);
		}
	}
	#endregion
	#region DayViewGroupByNoneVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByNoneVisuallyContinuousCellsInfosCalculator : DayViewTimeCellsVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByNone(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByNone(resources);
		}
	}
	#endregion
	#region DayViewGroupByResourceVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByResourceVisuallyContinuousCellsInfosCalculator : DayViewTimeCellsVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByResource(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByResource(resources);
		}
	}
	#endregion
	#region DayViewGroupByDateVisuallyContinuousCellsInfosCalculator
	public class DayViewGroupByDateVisuallyContinuousCellsInfosCalculator : DayViewTimeCellsVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByDate(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByDate(resources);
		}
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
	#region WeekViewVisuallyContinuousCellsInfosCalculator (abstract class)
	public abstract class WeekViewVisuallyContinuousCellsInfosCalculator : VisuallyContinuousCellsInfosCalculator {
		protected internal override VisuallyContinuousCellsInfoCollection CreateContainerVisuallyContinuousCellsInfos(SchedulerViewInfoBase viewInfo, SchedulerViewCellContainer container) {
			WeekViewInfo weekViewInfo = (WeekViewInfo)viewInfo;
			WeekVisuallyContinuousCellsCalculatorHelper helper = new WeekVisuallyContinuousCellsCalculatorHelper(weekViewInfo);
			return helper.CreateVerticalWeekContinuousCells(container);
		}
	}
	#endregion
	#region WeekViewHorizontalVisuallyContinuousCellsInfosCalculator (abstract class)
	public abstract class MonthViewVisuallyContinuousCellsInfosCalculator : VisuallyContinuousCellsInfosCalculator {
		protected internal override VisuallyContinuousCellsInfoCollection CreateContainerVisuallyContinuousCellsInfos(SchedulerViewInfoBase viewInfo, SchedulerViewCellContainer container) {
			WeekViewInfo weekViewInfo = (WeekViewInfo)viewInfo;
			WeekVisuallyContinuousCellsCalculatorHelper helper = new WeekVisuallyContinuousCellsCalculatorHelper(weekViewInfo);
			return helper.CreateHorizontalWeekContinuousCells(container);
		}
	}
	#endregion
	#region MonthViewCompressWeekendVisuallyContinuousCellsInfosCalculator (abstract class)
	public abstract class MonthViewCompressWeekendVisuallyContinuousCellsInfosCalculator : VisuallyContinuousCellsInfosCalculator {
		protected internal override VisuallyContinuousCellsInfoCollection CreateContainerVisuallyContinuousCellsInfos(SchedulerViewInfoBase viewInfo, SchedulerViewCellContainer container) {
			WeekViewInfo weekViewInfo = (WeekViewInfo)viewInfo;
			WeekVisuallyContinuousCellsCalculatorHelper helper = new WeekVisuallyContinuousCellsCalculatorHelper(weekViewInfo);
			return helper.CreateHorizontalWeekCompressWeekendContinuousCells(container);
		}
	}
	#endregion
	#region WeekViewGroupByNoneVisuallyContinuousCellsInfosCalculator
	public class WeekViewGroupByNoneVisuallyContinuousCellsInfosCalculator : WeekViewVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByNone(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByNone(resources);
		}
	}
	#endregion
	#region WeekViewGroupByResourceVisuallyContinuousCellsInfosCalculator
	public class WeekViewGroupByResourceVisuallyContinuousCellsInfosCalculator : WeekViewVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByResource(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByResource(resources);
		}
	}
	#endregion
	#region WeekViewGroupByDateVisuallyContinuousCellsInfosCalculator
	public class WeekViewGroupByDateVisuallyContinuousCellsInfosCalculator : MonthViewCompressWeekendVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByDate(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByDate(resources);
		}
	}
	#endregion
	#region MonthViewGroupByNoneVisuallyContinuousCellsInfosCalculator
	public class MonthViewGroupByNoneVisuallyContinuousCellsInfosCalculator : MonthViewVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByNone(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByNone(resources);
		}
	}
	#endregion
	#region MonthViewGroupByDateVisuallyContinuousCellsInfosCalculator
	public class MonthViewGroupByDateVisuallyContinuousCellsInfosCalculator : MonthViewVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByDate(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByDate(resources);
		}
	}
	#endregion
	#region MonthViewGroupByResourceVisuallyContinuousCellsInfosCalculator
	public class MonthViewGroupByResourceVisuallyContinuousCellsInfosCalculator : MonthViewVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByResource(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByResource(resources);
		}
	}
	#endregion
	#region MonthViewGroupByNoneCompressWeekendVisuallyContinuousCellsInfosCalculator
	public class MonthViewGroupByNoneCompressWeekendVisuallyContinuousCellsInfosCalculator : MonthViewCompressWeekendVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByNone(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByNone(resources);
		}
	}
	#endregion
	#region MonthViewGroupByDateCompressWeekendVisuallyContinuousCellsInfosCalculator
	public class MonthViewGroupByDateCompressWeekendVisuallyContinuousCellsInfosCalculator : MonthViewCompressWeekendVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByDate(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByDate(resources);
		}
	}
	#endregion
	#region MonthViewGroupByResourceCompressWeekendVisuallyContinuousCellsInfosCalculator
	public class MonthViewGroupByResourceCompressWeekendVisuallyContinuousCellsInfosCalculator : MonthViewCompressWeekendVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByResource(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByResource(resources);
		}
	}
	#endregion
	public abstract class TimelineViewVisuallyContinuousCellsInfosCalculator : VisuallyContinuousCellsInfosCalculator {
		protected internal override VisuallyContinuousCellsInfoCollection CreateContainerVisuallyContinuousCellsInfos(SchedulerViewInfoBase viewInfo, SchedulerViewCellContainer container) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			result.Add(new VisuallyContinuousCellsInfo(container.Cells, container));
			return result;
		}
	}
	#region TimelineViewGroupByNoneVisuallyContinuousCellsInfosCalculator
	public class TimelineViewGroupByNoneVisuallyContinuousCellsInfosCalculator : TimelineViewVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByNone(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByNone(resources);
		}
	}
	#endregion
	#region TimelineViewGroupByDateVisuallyContinuousCellsInfosCalculator
	public class TimelineViewGroupByDateVisuallyContinuousCellsInfosCalculator : TimelineViewVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByDate(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByDate(resources);
		}
	}
	#endregion
	#region TimelineViewGroupByResourceVisuallyContinuousCellsInfosCalculator
	public class TimelineViewGroupByResourceVisuallyContinuousCellsInfosCalculator : TimelineViewVisuallyContinuousCellsInfosCalculator {
		protected internal override SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection sourceCellContainers, int resourceCount, int resourceIndex) {
			return FilterCellContainersGroupByResource(sourceCellContainers, resourceCount, resourceIndex);
		}
		protected internal override ResourceVisuallyContinuousCellsInfosCollection PrepareResult(ResourceBaseCollection resources) {
			return PrepareResultGroupByResource(resources);
		}
	}
	#endregion
	#region WeekVisuallyContinuousCellsCalculatorHelper (abstract class)
	public class WeekVisuallyContinuousCellsCalculatorHelper {
		ISupportWeekCells viewInfo;
		public WeekVisuallyContinuousCellsCalculatorHelper(ISupportWeekCells viewInfo) {
			if (viewInfo == null)
				Exceptions.ThrowArgumentNullException("viewInfo");
			this.viewInfo = viewInfo;
		}
		public ISupportWeekCells ViewInfo { get { return viewInfo; } }
		protected internal virtual VisuallyContinuousCellsInfoCollection CreateVerticalWeekContinuousCells(SchedulerViewCellContainer container) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			int count = container.Cells.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewCellBase cell = container.Cells[i];
				if (ShouldHideCellContent(cell))
					continue;
				SchedulerViewCellBaseCollection cells = new SchedulerViewCellBaseCollection();
				cells.Add(cell);
				result.Add(new VisuallyContinuousCellsInfo(cells, container));
			}
			return result;
		}
		protected internal virtual VisuallyContinuousCellsInfoCollection CreateHorizontalWeekContinuousCells(SchedulerViewCellContainer container) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			int count = container.Cells.Count;
			SchedulerViewCellBaseCollection cells = new SchedulerViewCellBaseCollection();
			for (int i = 0; i < count; i++) {
				SchedulerViewCellBase cell = container.Cells[i];
				if (ShouldHideCellContent(cell)) {
					if (cells.Count > 0) {
						result.Add(new VisuallyContinuousCellsInfo(cells, container));
						cells = new SchedulerViewCellBaseCollection();
					}
					continue;
				} else
					cells.Add(cell);
			}
			if (cells.Count > 0)
				result.Add(new VisuallyContinuousCellsInfo(cells, container));
			return result;
		}
		protected internal virtual VisuallyContinuousCellsInfoCollection CreateHorizontalWeekCompressWeekendContinuousCells(SchedulerViewCellContainer container) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			int count = container.Cells.Count;
			SchedulerViewCellBaseCollection cells = new SchedulerViewCellBaseCollection();
			for (int i = 0; i < count; i++) {
				SchedulerViewCellBase cell = container.Cells[i];
				if (ShouldHideCellContent(cell)) {
					if (cells.Count > 0) {
						result.Add(new VisuallyContinuousCellsInfo(cells, container));
						cells = new SchedulerViewCellBaseCollection();
					}
					continue;
				}
				DayOfWeek dayOfWeek = cell.Interval.Start.DayOfWeek;
				if (dayOfWeek == DayOfWeek.Saturday) {
					cells.Add(cell);
					result.Add(new VisuallyContinuousCellsInfo(cells, container));
					cells = new SchedulerViewCellBaseCollection();
				} else if (dayOfWeek == DayOfWeek.Sunday) {
					if (cells.Count > 0) {
						result.Add(new VisuallyContinuousCellsInfo(cells, container));
						cells = new SchedulerViewCellBaseCollection();
					}
					cells.Add(cell);
					result.Add(new VisuallyContinuousCellsInfo(cells, container));
					cells = new SchedulerViewCellBaseCollection();
				} else
					cells.Add(cell);
			}
			if (cells.Count > 0)
				result.Add(new VisuallyContinuousCellsInfo(cells, container));
			return result;
		}
		protected internal virtual bool ShouldHideCellContent(SchedulerViewCellBase cell) {
			return ViewInfo.ShouldHideCellContent(cell);
		}
	}
	#endregion
}
