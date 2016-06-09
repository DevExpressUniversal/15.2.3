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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class DayViewGroupCellsLayoutCalculator : SchedulerViewCellsLayoutCalculator {
		protected DayViewGroupCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter)
			: base(cache, viewInfo, painter) {
		}
		protected internal new DayViewInfo ViewInfo { get { return (DayViewInfo)base.ViewInfo; } }
		protected internal new DayView View { get { return (DayView)base.View; } }
		protected internal abstract SchedulerHeaderCollection AnchorHeaders { get; }
		protected internal abstract bool ShouldPaintAllDayAreaWithResourceColor { get; }
		protected internal override void CalculatePreliminaryLayout() {
			DayViewPreliminaryLayoutResult preliminaryResult = ViewInfo.PreliminaryLayoutResult;
			DayViewCellsPreliminaryLayoutResult cellsPreliminaryResult = preliminaryResult.CellsPreliminaryLayoutResult;
			TimeSpan timeScale = View.TimeScale;
			TimeOfDayInterval alignedVisibleTime = CreateAlignedVisibleTime(View.ActualVisibleTime, timeScale);
			cellsPreliminaryResult.AlignedVisibleTime = alignedVisibleTime;
			cellsPreliminaryResult.TimeIntervalsCount = CalculateTimeIntervalsCount(alignedVisibleTime, timeScale);
			SchedulerViewCellContainerCollection columns = CreatePreliminaryDayViewColumns();
			InitializeContainerBorders(columns);
			preliminaryResult.CellContainers.Clear();
			preliminaryResult.CellContainers.AddRange(columns);
			if (columns.Count > 0)
				preliminaryResult.ExtendedRowCount = ((DayViewColumn)columns[0]).ExtendedCells.Count;
		}
		protected internal virtual void InitializeContainerBorders(SchedulerViewCellContainerCollection columns) {
			int count = columns.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewCellContainer column = columns[i];
				ResetContainerBorders(column);
				column.HasLeftBorder = i > 0;
			}
		}
		public virtual void CalculateCellsBounds(Rectangle bounds, DayViewPreliminaryLayoutResult preliminaryResult) {
			DayViewCellsPreliminaryLayoutResult cellsPreliminaryResult = preliminaryResult.CellsPreliminaryLayoutResult;
			CalculateColumnsBounds(bounds, preliminaryResult);
			cellsPreliminaryResult.MinRowHeight = CalculateMinRowHeight();
			cellsPreliminaryResult.AllDayAreaHeight = CalculateAllDayAreaHeight(preliminaryResult);
			CalcualteRowsWithExtendedCellsBounds(preliminaryResult);
		}
		public virtual void CalculateExtendedCellsBounds(Rectangle bounds, DayViewPreliminaryLayoutResult preliminaryResult) {
			if (!View.ShowExtendedCells) {
				preliminaryResult.CellsPreliminaryLayoutResult.RowsBounds = preliminaryResult.CellsPreliminaryLayoutResult.RowsBoundsWithExtendedCells;
				return;
			} else {
				int extendedCellsHeight = CalculatExtendedCellsHeight(preliminaryResult);
				Rectangle rowsBounds = RectUtils.CutFromBottom(preliminaryResult.CellsPreliminaryLayoutResult.RowsBoundsWithExtendedCells, extendedCellsHeight);
				if (rowsBounds.Height < 0)
					rowsBounds.Height = 0;
				Rectangle extendedCellsBounds = RectUtils.CutFromTop(preliminaryResult.CellsPreliminaryLayoutResult.RowsBoundsWithExtendedCells, rowsBounds.Height);
				preliminaryResult.CellsPreliminaryLayoutResult.RowsBounds = rowsBounds;
				preliminaryResult.ExtendedCellsBounds = extendedCellsBounds;
			}
		}
		protected internal virtual int CalculatExtendedCellsHeight(DayViewPreliminaryLayoutResult layoutResult) {
			DayViewCellsPreliminaryLayoutResult cellsLayoutResult = layoutResult.CellsPreliminaryLayoutResult;
			if (cellsLayoutResult.DateTimeScrollBarVisible)
				return layoutResult.ExtendedRowCount * cellsLayoutResult.MinRowHeight;
			else
				return cellsLayoutResult.RowsBoundsWithExtendedCells.Height / (cellsLayoutResult.TimeIntervalsCount + layoutResult.ExtendedRowCount) * layoutResult.ExtendedRowCount;
		}
		protected internal virtual void CalculateColumnsBounds(Rectangle bounds, DayViewPreliminaryLayoutResult preliminaryResult) {
			Rectangle columnsBounds = bounds;
			int totalHeadersHeight = preliminaryResult.TotalHeadersHeight;
			columnsBounds.Height = Math.Max(0, bounds.Height - totalHeadersHeight);
			columnsBounds.Y = bounds.Top + totalHeadersHeight;
			preliminaryResult.CellsPreliminaryLayoutResult.ColumnsBounds = columnsBounds;
		}
		protected internal virtual void CalcualteRowsWithExtendedCellsBounds(DayViewPreliminaryLayoutResult preliminaryResult) {
			Rectangle columnsBounds = preliminaryResult.CellsPreliminaryLayoutResult.ColumnsBounds;
			int allDayAreaHeight = preliminaryResult.CellsPreliminaryLayoutResult.AllDayAreaHeight;
			Rectangle rowsBoundsWithExtendedCells = RectUtils.CutFromTop(columnsBounds, allDayAreaHeight);
			rowsBoundsWithExtendedCells.Height = Math.Max(0, rowsBoundsWithExtendedCells.Height);
			preliminaryResult.CellsPreliminaryLayoutResult.RowsBoundsWithExtendedCells = rowsBoundsWithExtendedCells;
		}
		protected internal virtual int CalculateAllDayAreaHeight(DayViewPreliminaryLayoutResult preliminaryResult) {
			DayViewPainter painter = (DayViewPainter)(ViewInfo.Painter);
			if (!View.ShowAllDayArea)
				return painter.HiddenAllDayAreaHeight;
			else {
				int minRowHeight = ViewInfo.CellsPreliminaryLayoutResult.MinRowHeight;
				AppointmentIntermediateViewInfoCollection allDayAppointments = ViewInfo.PreliminaryLayoutResult.CellsLayerInfos.GetAppointmentViewInfos();
				int height = DayViewCellsCalculatorHelper.CalculateAllDayAreaHeight(painter.AppointmentPainter, allDayAppointments, minRowHeight);
				height += (preliminaryResult.CellsPreliminaryLayoutResult.ColumnsBounds.Height - height) % minRowHeight;
				bool canScroll = true; 
				return canScroll ? Math.Min(height, preliminaryResult.CellsPreliminaryLayoutResult.ColumnsBounds.Height / 2) : height;
			}
		}
		protected internal virtual int CalculateTimeIntervalsCount(TimeOfDayInterval alignedVisibleTime, TimeSpan timeScale) {
			return DayViewCellsCalculatorHelper.CalculateTimeIntervalsCount(alignedVisibleTime, timeScale);
		}
		protected internal virtual TimeOfDayInterval CreateAlignedVisibleTime(TimeOfDayInterval visibleTime, TimeSpan timeScale) {
			return DayViewCellsCalculatorHelper.CreateAlignedVisibleTime(visibleTime, timeScale, View.VisibleTimeSnapMode);
		}
		public virtual int CalculateMinRowHeight() {
			object aptImages = GetAppointmentImages();
			AppointmentPainter aptPainter = ViewInfo.Painter.TimeCellsAppointmentPainter;
			int height = DayViewCellsCalculatorHelper.CalculateMinAppointmentHeight(ViewInfo.PaintAppearance.Appointment, aptImages, aptPainter, Cache);
			TimeRulerViewInfoCollection rulers = ViewInfo.TimeRulers;
			int count = rulers.Count;
			for (int i = 0; i < count; i++)
				height = Math.Max(height, rulers[i].BackgroundAppearance.CalcDefaultTextSize(Cache.Graphics).Height);
			return Math.Max(View.RowHeight, height);
		}
		protected internal virtual object GetAppointmentImages() {
			object appointmentImages = ViewInfo.View.Control.AppointmentImages;
			if (appointmentImages == null)
				return ViewInfo.Painter.AppointmentPainter.DefaultAppointmentImages;
			return appointmentImages;
		}
		protected internal virtual void CreateRowsInfo() {
			TimeOfDayInterval alignedVisibleTime = ViewInfo.CellsPreliminaryLayoutResult.AlignedVisibleTime;
			int timeIntervalsCount = ViewInfo.CellsPreliminaryLayoutResult.TimeIntervalsCount;
			XtraSchedulerDebug.Assert(ViewInfo.Rows.Count == 0);
			CreateRows(alignedVisibleTime.Start, timeIntervalsCount);
		}
		protected void CalculateRowsInfo() {
			TimeOfDayInterval alignedVisibleTime = ViewInfo.CellsPreliminaryLayoutResult.AlignedVisibleTime;
			int timeIntervalsCount = ViewInfo.CellsPreliminaryLayoutResult.TimeIntervalsCount;
			Rectangle bounds = ViewInfo.CellsPreliminaryLayoutResult.RowsBounds;
			XtraSchedulerDebug.Assert(ViewInfo.VisibleRows.Count == 0);
			if (ViewInfo.CellsPreliminaryLayoutResult.DateTimeScrollBarVisible)
				CreateRowsInfoForScrolling(bounds, alignedVisibleTime.Start, timeIntervalsCount);
			else
				CreateRowsInfoWithoutScrolling(bounds, alignedVisibleTime.Start, timeIntervalsCount);
			CreateExtendedRows();
		}
		protected internal virtual void CreateRowsInfoWithoutScrolling(Rectangle bounds, TimeSpan start, int timeIntervalsCount) {
			ViewInfo.TopRowTime = start;
			CalculateVisibleRowsInfo(bounds, start, timeIntervalsCount);
		}
		protected internal virtual void CreateRowsInfoForScrolling(Rectangle bounds, TimeSpan start, int timeIntervalsCount) {
			int minRowHeight = ViewInfo.CellsPreliminaryLayoutResult.MinRowHeight;
			int visibleRowsCount = 0;
			if (bounds.Height > 0)
				visibleRowsCount = Math.Max(1, bounds.Height / minRowHeight);
			TimeSpan timeScale = View.TimeScale;
			XtraSchedulerDebug.Assert(timeIntervalsCount > visibleRowsCount);
			TimeSpan topRowTime = CalculateTopRowTime(start, timeIntervalsCount, visibleRowsCount);
			ViewInfo.TopRowTime = topRowTime;
			int invisibleTopRowsCount = DateTimeHelper.Divide(topRowTime - start, View.TimeScale);
			CalculateInvisibleRows(bounds, start, invisibleTopRowsCount, -minRowHeight);
			CalculateVisibleRowsInfo(bounds, topRowTime, visibleRowsCount);
			TimeSpan visibleRowsEndTime = topRowTime + TimeSpan.FromTicks(visibleRowsCount * timeScale.Ticks);
			int invisibleBottomRowsCount = timeIntervalsCount - invisibleTopRowsCount - visibleRowsCount;
			CalculateInvisibleRows(bounds, visibleRowsEndTime, invisibleBottomRowsCount, minRowHeight);
		}
		protected internal virtual void CalculateVisibleRowsInfo(Rectangle visibleRowsBounds, TimeSpan topRowTime, int visibleRowsCount) {
			Rectangle[] rowBounds = RectUtils.SplitVertically(visibleRowsBounds, visibleRowsCount);
			TimeSpan timeScale = View.TimeScale;
			for (int i = 0; i < visibleRowsCount; i++) {
				TimeSpan endRowTime = GetValidEndRowTime(topRowTime + timeScale);
				TimeOfDayInterval timeInterval = new TimeOfDayInterval(topRowTime, endRowTime);
				DayViewRow row = ViewInfo.PreliminaryLayoutResult.Rows.Find(r => TimeOfDayInterval.Equals(r.Interval, timeInterval));
				row.Bounds = rowBounds[i];
				ViewInfo.VisibleRows.Add(row);
				topRowTime += View.TimeScale;
			}
		}
		protected virtual void CreateExtendedRows() {
			Rectangle extendedRowsBounds = ViewInfo.PreliminaryLayoutResult.ExtendedCellsBounds;
			int extendedRowsCount = ViewInfo.PreliminaryLayoutResult.ExtendedRowCount;
			Rectangle[] rowBounds = RectUtils.SplitVertically(extendedRowsBounds, extendedRowsCount);
			for (int i = 0; i < extendedRowsCount; i++) {
				DayViewRow row = new DayViewRow(TimeOfDayInterval.Empty);
				row.Bounds = rowBounds[i];
				ViewInfo.ExtendedRows.Add(row);
			}
		}
		protected internal virtual TimeSpan GetValidEndRowTime(TimeSpan endRowTime) {
			return DayViewCellsCalculatorHelper.GetValidEndRowTime(endRowTime, View.ActualVisibleTime);
		}
		protected internal virtual TimeSpan CalculateTopRowTime(TimeSpan start, int timeIntervalsCount, int visibleRowsCount) {
			TimeSpan scrollStartTime;
			if (!View.VisibleTimeSnapMode)
				scrollStartTime = DateTimeHelper.Ceil(View.ScrollStartTime, View.TimeScale);
			else
				scrollStartTime = DateTimeHelper.Ceil(View.ScrollStartTime, View.TimeScale, start);
			if (scrollStartTime < start)
				return start;
			TimeSpan maxTopRowTime = start + TimeSpan.FromTicks((timeIntervalsCount - visibleRowsCount) * View.TimeScale.Ticks);
			if (scrollStartTime > maxTopRowTime)
				return maxTopRowTime;
			return scrollStartTime;
		}
		protected internal virtual void CalculateInvisibleRows(Rectangle bounds, TimeSpan startTime, int rowCount, int rowHeight) {
			if (rowCount == 0)
				return;
			TimeSpan timeScale = View.TimeScale;
			if (rowHeight < 0) {
				rowHeight = -rowHeight;
				bounds.Y -= rowCount * rowHeight;
			} else
				bounds.Y = bounds.Bottom;
			Rectangle rect = new Rectangle(bounds.X, bounds.Y, bounds.Width, rowHeight);
			for (int i = 0; i < rowCount; i++) {
				TimeSpan endRowTime = GetValidEndRowTime(startTime + timeScale);
				TimeOfDayInterval timeInterval = new TimeOfDayInterval(startTime, endRowTime);
				DayViewRow row = ViewInfo.PreliminaryLayoutResult.Rows.Find(r => TimeOfDayInterval.Equals(r.Interval, timeInterval));
				row.Bounds = rect;
				startTime += timeScale;
				rect.Offset(0, rowHeight);
			}
		}
		protected internal virtual void CreateRows(TimeSpan startTime, int rowCount) {
			if (rowCount == 0)
				return;
			TimeSpan timeScale = View.TimeScale;
			for (int i = 0; i < rowCount; i++) {
				TimeSpan endRowTime = GetValidEndRowTime(startTime + timeScale);
				DayViewRow row = new DayViewRow(new TimeOfDayInterval(startTime, endRowTime));
				startTime += timeScale;
				ViewInfo.PreliminaryLayoutResult.Rows.Add(row);
			}
		}
		public override void CalcLayout(Rectangle bounds) {
			if (bounds.Height <= 0 || bounds.Width <= 0)
				return;
			CalculateScrollOffset(ViewInfo.PreliminaryLayoutResult.CellContainers);
			CalculateRowsInfo();
			CalculateColumns();
			ViewInfo.Columns.AddRange(ViewInfo.PreliminaryLayoutResult.CellContainers);
			ViewInfo.PreliminaryLayoutResult.CalculateAllDayAreaBounds();
			ViewInfo.Rows.AddRange(ViewInfo.PreliminaryLayoutResult.Rows);
		}
		protected internal virtual SchedulerViewCellContainerCollection CreatePreliminaryDayViewColumns() {
			SchedulerViewCellContainerCollection columns = new SchedulerViewCellContainerCollection();
			int count = AnchorHeaders.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHeader anchorHeader = AnchorHeaders[i];
				DayViewColumn column = (DayViewColumn)GetCellContainerByResourceAndInterval(anchorHeader.Resource, anchorHeader.Interval);
				if (column == null) {
					int resourceColorIndex = GetResourceColorIndexFromColumnIndex(i);
					column = CreatePreliminaryColumn(anchorHeader, resourceColorIndex);
					CreatePreliminaryAllDayAreaCell(column, i, anchorHeader.Bounds);
				}
				columns.Add(column);
			}
			CreatePreliminaryExtendedCells(columns);
			return columns;
		}
		protected internal virtual void CreatePreliminaryExtendedCells(SchedulerViewCellContainerCollection columns) {
			if (!View.ShowExtendedCells || columns == null || columns.Count == 0)
				return;
			bool reloaded;
			AppointmentBaseCollection appointments = View.Control.InnerControl.GetFilteredAppointments(columns[0].Interval, View.VisibleResources, out reloaded);
			DayViewCellsCalculatorHelper.CreatePreliminaryExtendedCells(columns, appointments, View.MinimumExtendedCellsInColumn, View.ActualVisibleTime);
		}
		void CalculateColumns() {
			DayViewPreliminaryLayoutResult preliminaryResult = ViewInfo.PreliminaryLayoutResult;
			SchedulerViewCellContainerCollection columns = preliminaryResult.CellContainers;
			int count = columns.Count;
			DayViewColumnPainter columnPainter = (DayViewColumnPainter)ViewInfo.Painter.SelectCellsLayoutPainter();
			for (int i = 0; i < count; i++) {
				SchedulerHeader anchorHeader = AnchorHeaders[i];
				DayViewColumn column = (DayViewColumn)columns[i];
				CalculateColumn(anchorHeader, column);
				CalculateAllDayAreaCell(column, i, anchorHeader.AnchorBounds);
				CalculateExtendedCells(column, columnPainter);
				CreateCells(column);
				column.CalculateCellBorders(columnPainter);
				column.CalculateFinalAppearance(ViewInfo.PaintAppearance, column.ColorSchema);
			}
		}
		protected internal virtual DayViewColumn CreatePreliminaryColumn(SchedulerHeader anchorHeader, int resourceColorIndex) {
			SchedulerColorSchema colorSchema = GetColorSchema(anchorHeader.Resource, resourceColorIndex);
			DayViewColumn column = ViewInfo.CreateDayViewColumn(anchorHeader.Resource, anchorHeader.Interval, colorSchema);
			return column;
		}
		protected internal virtual void CalculateColumn(SchedulerHeader anchorHeader, DayViewColumn column) {
			Rectangle anchorBounds = anchorHeader.AnchorBounds;
			Rectangle headerBounds = anchorHeader.Bounds;
			Rectangle columnsBounds = ViewInfo.CellsPreliminaryLayoutResult.ColumnsBounds;
			Rectangle rowsBounds = ViewInfo.CellsPreliminaryLayoutResult.RowsBounds;
			int statusLineWidth = ViewInfo.CellsPreliminaryLayoutResult.StatusLineWidth;
			Rectangle cellsBounds = Rectangle.FromLTRB(headerBounds.Left + statusLineWidth, rowsBounds.Top, anchorBounds.Right, rowsBounds.Bottom);
			Rectangle columnBounds = new Rectangle(headerBounds.Left, 0, headerBounds.Width, 0);
			column.Bounds = new Rectangle(columnBounds.Left, columnsBounds.Top, columnBounds.Width, columnsBounds.Height);
			column.CellsBounds = cellsBounds;
			column.StatusLineBounds = new Rectangle(headerBounds.Left, cellsBounds.Top, statusLineWidth, cellsBounds.Height);
			DayViewWorkTimeInfoCalculator workTimeInfoCalculator = new DayViewWorkTimeInfoCalculator((InnerDayView)View.InnerView);
			WorkTimeInfo workTimeInfo = workTimeInfoCalculator.CalcWorkTimeInfo(column.Interval, column.Resource);
			column.IsWorkDay = workTimeInfo.IsWorkDay;
			column.WorkTimes.Clear();
			column.WorkTimes.AddRange(workTimeInfo.WorkTimes);
		}
		protected internal virtual void CreatePreliminaryAllDayAreaCell(DayViewColumn column, int columnIndex, Rectangle anchorBounds) {
			if (!View.ShowAllDayArea)
				return;
			AllDayAreaCell allDayAreaCell = new AllDayAreaCell();
			column.InitializeCell(allDayAreaCell, column.Interval);
			allDayAreaCell.DrawLeftSeparator = columnIndex > 0;
			column.AllDayAreaCell = allDayAreaCell;
		}
		protected internal virtual void CalculateAllDayAreaCell(DayViewColumn column, int columnIndex, Rectangle anchorBounds) {
			if (!View.ShowAllDayArea)
				return;
			AllDayAreaCell allDayAreaCell = column.AllDayAreaCell;
			Rectangle bounds = new Rectangle(anchorBounds.Left, 0, anchorBounds.Width, Int32.MaxValue);
			allDayAreaCell.Bounds = new Rectangle(bounds.Left, column.Bounds.Top, bounds.Width, ViewInfo.CellsPreliminaryLayoutResult.AllDayAreaHeight);
			DayViewColumnPainter painter = (DayViewColumnPainter)Painter;
			if (ShouldPaintAllDayAreaWithResourceColor)
				painter.PrepareCachedSkinElementInfo(allDayAreaCell, column.ColorSchema.Cell);
		}
		protected internal virtual void CalculateExtendedCells(DayViewColumn column, DayViewColumnPainter painter) {
			if (!View.ShowExtendedCells)
				return;
			int count = column.ExtendedCells.Count;
			Rectangle bounds = column.Bounds;
			int left = bounds.X;
			int width = bounds.Width;
			for (int i = 0; i < count; i++) {
				DayViewRow row = ViewInfo.ExtendedRows[i];
				Rectangle cellRect = row.Bounds;
				cellRect.X = left;
				cellRect.Width = width;
				TimeCell cell = (TimeCell)column.ExtendedCells[i];
				cell.Bounds = cellRect;
			}
		}
		protected internal virtual void CreateCells(DayViewColumn column) {
			int count = ViewInfo.VisibleRows.Count;
			int left = column.CellsBounds.X;
			int width = column.CellsBounds.Width;
			column.Cells.Clear();
			for (int i = 0; i < count; i++) {
				DayViewRow row = ViewInfo.VisibleRows[i];
				TimeInterval interval = CreateCellTimeInterval(column, row);
				Rectangle cellRect = row.Bounds;
				cellRect.X = left;
				cellRect.Width = width;
				TimeCell cell = (TimeCell)column.CreateCell(interval);
				cell.Bounds = cellRect;
				column.Cells.Add(cell);
			}
		}
		protected internal virtual TimeInterval CreateCellTimeInterval(DayViewColumn column, DayViewRow row) {
			return new TimeInterval(column.Interval.Start.Date + row.Interval.Start, row.Interval.Duration);
		}
		protected internal abstract int GetResourceColorIndexFromColumnIndex(int columnIndex);
		void CalculateScrollOffset(SchedulerViewCellContainerCollection columns) {
			int minRowHeight = ViewInfo.CellsPreliminaryLayoutResult.MinRowHeight;
			TimeSpan timeScale = View.TimeScale;
			int visibleRowsCount;
			if (ViewInfo.CellsPreliminaryLayoutResult.DateTimeScrollBarVisible) {
				visibleRowsCount = ViewInfo.CellsPreliminaryLayoutResult.RowsBounds.Height / minRowHeight;
				XtraSchedulerDebug.Assert(ViewInfo.CellsPreliminaryLayoutResult.TimeIntervalsCount > visibleRowsCount);
			} else
				visibleRowsCount = ViewInfo.CellsPreliminaryLayoutResult.TimeIntervalsCount;
			TimeSpan topRowTime = CalculateTopRowTime(ViewInfo.CellsPreliminaryLayoutResult.AlignedVisibleTime.Start, ViewInfo.CellsPreliminaryLayoutResult.TimeIntervalsCount, visibleRowsCount);
			ViewInfo.TopRowTime = topRowTime;
			int invisibleTopRowsCount = DateTimeHelper.Divide(topRowTime - ViewInfo.CellsPreliminaryLayoutResult.AlignedVisibleTime.Start, timeScale);
			foreach (DayViewColumn column in columns)
				column.ScrollOffset = invisibleTopRowsCount * minRowHeight;
		}
	}
}
