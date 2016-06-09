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

using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using System.Drawing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Reporting {
	public abstract class VisuallyContinuousCellsCalculatorBase {
		protected VisuallyContinuousCellsCalculatorBase() {
		}
		public virtual ResourceVisuallyContinuousCellsInfosCollection Calculate(SchedulerViewCellContainerCollection cellContainers) {
			ResourceVisuallyContinuousCellsInfosCollection result = new ResourceVisuallyContinuousCellsInfosCollection();
			ResourceBaseCollection resources = GetContainerResources(cellContainers);
			int count = resources.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = resources[i];
				ResourceVisuallyContinuousCellsInfos resourceCellInfos = CreateResourceCellInfos(cellContainers, resource);
				result.Add(resourceCellInfos);
			}
			return result;
		}
		protected internal virtual ResourceVisuallyContinuousCellsInfos CreateResourceCellInfos(SchedulerViewCellContainerCollection cellContainers, Resource resource) {
			ResourceVisuallyContinuousCellsInfos resourceCellInfos = new ResourceVisuallyContinuousCellsInfos(resource);
			SchedulerViewCellContainerCollection containers = FilterCellContainers(cellContainers, resource);
			VisuallyContinuousCellsInfoCollection cellInfos = CreateCellsInfosForResource(containers);
			resourceCellInfos.CellsInfoCollection.AddRange(cellInfos);
			return resourceCellInfos;
		}
		protected internal virtual ResourceBaseCollection GetContainerResources(SchedulerViewCellContainerCollection cellContainers) {
			ResourceBaseCollection resources = new ResourceBaseCollection();
			int count = cellContainers.Count;
			for (int i = 0; i < count; i++)
				resources.Add(cellContainers[i].Resource);
			return resources;
		}
		protected internal virtual SchedulerViewCellContainerCollection FilterCellContainers(SchedulerViewCellContainerCollection cellContainers, Resource resource) {
			int count = cellContainers.Count;
			SchedulerViewCellContainerCollection result = new SchedulerViewCellContainerCollection();
			for (int i = 0; i < count; i++) {
				SchedulerViewCellContainer container = cellContainers[i];
				if (IsSameResource(container, resource))
					result.Add(container);
			}
			return result;
		}
		protected internal virtual bool IsSameResource(SchedulerViewCellContainer container, Resource resource) {
			return ResourceBase.InternalMatchIds(container.Resource.Id, resource.Id);
		}
		protected internal virtual VisuallyContinuousCellsInfoCollection CreateCellsInfosForResource(SchedulerViewCellContainerCollection containers) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			int count = containers.Count;
			for (int i = 0; i < count; i++)
				result.AddRange(CreateCellsInfosForResourceCore(containers[i]));
			return result;
		}
		protected internal abstract VisuallyContinuousCellsInfoCollection CreateCellsInfosForResourceCore(SchedulerViewCellContainer container);
	}
	public abstract class NonBreakingVisuallyContinuousCellsCalculator : VisuallyContinuousCellsCalculatorBase {
		protected NonBreakingVisuallyContinuousCellsCalculator()
			: base() {
		}
		protected internal override VisuallyContinuousCellsInfoCollection CreateCellsInfosForResourceCore(SchedulerViewCellContainer container) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			VisuallyContinuousCellsInfo cellsInfo = CreateCellsInfo(container);
			result.Add(cellsInfo);
			return result;
		}
		protected internal virtual VisuallyContinuousCellsInfo CreateCellsInfo(SchedulerViewCellContainer container) {
			return new VisuallyContinuousCellsInfo(container.Cells, container);
		}
	}
	public class DayViewTimeCellVisuallyContinuousCellsCalculator : NonBreakingVisuallyContinuousCellsCalculator {
		public DayViewTimeCellVisuallyContinuousCellsCalculator()
			: base() {
		}
		protected internal override VisuallyContinuousCellsInfo CreateCellsInfo(SchedulerViewCellContainer container) {
			DayViewColumn column = (DayViewColumn)container;
			return new ColumnVisuallyContinuousCellsInfo(column, column);
		}
	}
	public class DayViewAllDayVisuallyContinuousCellsCalculator : VisuallyContinuousCellsCalculatorBase {
		public DayViewAllDayVisuallyContinuousCellsCalculator()
			: base() {
		}
		protected internal override ResourceVisuallyContinuousCellsInfos CreateResourceCellInfos(SchedulerViewCellContainerCollection cellContainers, Resource resource) {
			ResourceVisuallyContinuousCellsInfos result = new ResourceVisuallyContinuousCellsInfos(resource);
			int count = cellContainers.Count;
			SchedulerViewCellBaseCollection cells = new SchedulerViewCellBaseCollection();
			for (int i = 0; i < count; i++) {
				DayViewColumn column = (DayViewColumn)cellContainers[i];
				if (IsSameResource(column, resource))
					cells.Add(column.AllDayAreaCell);
				else
					if (cells.Count != 0) {
						result.CellsInfoCollection.Add(new VisuallyContinuousCellsInfo(cells, AllDayAreaScrollContainer.Empty));
						cells = new SchedulerViewCellBaseCollection();
					}
			}
			if (cells.Count != 0)
				result.CellsInfoCollection.Add(new VisuallyContinuousCellsInfo(cells, AllDayAreaScrollContainer.Empty));
			return result;
		}
		protected internal override VisuallyContinuousCellsInfoCollection CreateCellsInfosForResourceCore(SchedulerViewCellContainer container) {
			return new VisuallyContinuousCellsInfoCollection();
		}
	}
	public class TimelineVisuallyContinuousCellsCalculator : NonBreakingVisuallyContinuousCellsCalculator {
		public TimelineVisuallyContinuousCellsCalculator()
			: base() {
		}
	}
	public abstract class WeekVisuallyContinuousCellsCalculator : VisuallyContinuousCellsCalculatorBase {
		ISupportWeekCells cellsControl;
		readonly WeekVisuallyContinuousCellsCalculatorHelper helper;
		protected WeekVisuallyContinuousCellsCalculator(ISupportWeekCells cellsControl)
			: base() {
			if (cellsControl == null)
				Exceptions.ThrowArgumentNullException("cellsControl");
			this.cellsControl = cellsControl;
			helper = new WeekVisuallyContinuousCellsCalculatorHelper(CellsControl);
		}
		internal ISupportWeekCells CellsControl { get { return cellsControl; } set { cellsControl = value; } }
		internal WeekVisuallyContinuousCellsCalculatorHelper Helper { get { return helper; } }
	}
	public class FullWeekVisuallyContinuousCellsCalculator : WeekVisuallyContinuousCellsCalculator {
		public FullWeekVisuallyContinuousCellsCalculator(ISupportWeekCells viewControl)
			: base(viewControl) {
		}
		protected internal override VisuallyContinuousCellsInfoCollection CreateCellsInfosForResourceCore(SchedulerViewCellContainer container) {
			return Helper.CreateVerticalWeekContinuousCells(container);
		}
	}
	public class HorizontalWeekVisuallyContinuousCellsCalculator : WeekVisuallyContinuousCellsCalculator {
		public HorizontalWeekVisuallyContinuousCellsCalculator(HorizontalWeek viewControl)
			: base(viewControl) {
		}
		protected internal new HorizontalWeek CellsControl { get { return (HorizontalWeek)base.CellsControl; } }
		protected internal override VisuallyContinuousCellsInfoCollection CreateCellsInfosForResourceCore(SchedulerViewCellContainer container) {
			if (CellsControl.ActualCompressWeekend)
				return Helper.CreateHorizontalWeekCompressWeekendContinuousCells(container);
			else
				return Helper.CreateHorizontalWeekContinuousCells(container);
		}
	}
	public class ColumnVisuallyContinuousCellsInfo : VisuallyContinuousCellsInfo {
		DayViewColumn column;
		public ColumnVisuallyContinuousCellsInfo(DayViewColumn column, SchedulerViewCellContainer scrollContainer)
			: base(column.Cells, scrollContainer) {
			if (column == null)
				Exceptions.ThrowArgumentNullException("column");
			this.column = column;
		}
		internal DayViewColumn Column { get { return column; } }
		public override Rectangle GetTotalBounds() {
			if (Count == 0)
				return Rectangle.Empty;
			Rectangle firstCell = GetContentBounds(0);
			Rectangle lastCell = GetContentBounds(Count - 1);
			XtraSchedulerDebug.Assert(firstCell.Left == lastCell.Left);
			XtraSchedulerDebug.Assert(firstCell.Right == lastCell.Right);
			return Rectangle.FromLTRB(firstCell.Left, firstCell.Top, lastCell.Right, lastCell.Bottom);
		}
		protected internal override Rectangle GetContentBoundsInternal(int index) {
			Rectangle cellBounds = VisibleCells[index].Bounds;
			return Rectangle.FromLTRB(Column.ContentBounds.Left, cellBounds.Top, Column.ContentBounds.Right, cellBounds.Bottom);
		}
	}
}
