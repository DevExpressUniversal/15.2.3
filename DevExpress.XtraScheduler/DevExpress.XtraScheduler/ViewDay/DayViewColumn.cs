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
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class DayViewColumn : SchedulerViewCellContainer, IWorkTimeInfo {
		#region Fields
		bool isWorkDay;
		TimeOfDayIntervalCollection workTimes;
		AllDayAreaCell allDayAreaCell;
		Rectangle statusLineBounds;
		Rectangle cellsBounds;
		SchedulerViewCellBaseCollection extendedCells;
		AppearanceObject allDayAreaAppearance;
		AppearanceObject allDayAreaSelectionAppearance;
		AppearanceObject allDayAreaSeparatorAppearance;
		AppearanceObject statusLineAppearance;
		int scrollOffset;
		#endregion
		public DayViewColumn(Resource resource, TimeInterval interval, SchedulerColorSchema colorSchema)
			: base(colorSchema) {
			if (resource == null)
				Exceptions.ThrowArgumentException("resource", resource);
			if (interval == null)
				Exceptions.ThrowArgumentException("interval", interval);
			this.Resource = resource;
			this.Interval = interval;
			allDayAreaCell = null;
			extendedCells = new SchedulerViewCellBaseCollection();
			this.workTimes = new TimeOfDayIntervalCollection();
			this.workTimes.Add(WorkTimeInterval.WorkTime);
		}
		#region Properties
		public Rectangle StatusLineBounds { get { return statusLineBounds; } set { statusLineBounds = value; } }
		public Rectangle CellsBounds { get { return cellsBounds; } set { cellsBounds = value; } }
		public bool IsWorkDay { get { return isWorkDay; } set { isWorkDay = value; } }
		public AllDayAreaCell AllDayAreaCell { get { return allDayAreaCell; } set { allDayAreaCell = value; } }
		public override SchedulerHitTest HitTestType { get { return SchedulerHitTest.DayViewColumn; } }
		public virtual TimeInterval VisibleInterval {
			get {
				if (Cells.Count == 0)
					return TimeInterval.Empty;
				return new TimeInterval(Cells[0].Interval.Start, Cells[Cells.Count - 1].Interval.End);
			}
		}
		internal SchedulerViewCellBaseCollection ExtendedCells { get { return extendedCells; } }
		public TimeOfDayIntervalCollection WorkTimes { get { return workTimes; } }
		public TimeOfDayInterval WorkTime { get { return new TimeOfDayInterval(WorkTimes.Start, WorkTimes.End); } }
		public AppearanceObject AllDayAreaAppearance {
			get {
				if (allDayAreaAppearance == null)
					allDayAreaAppearance = new AppearanceObject();
				return allDayAreaAppearance;
			}
		}
		public AppearanceObject StatusLineAppearance {
			get {
				if (statusLineAppearance == null)
					statusLineAppearance = new AppearanceObject();
				return statusLineAppearance;
			}
		}
		public AppearanceObject AllDayAreaSelectionAppearance {
			get {
				if (allDayAreaSelectionAppearance == null)
					allDayAreaSelectionAppearance = new AppearanceObject();
				return allDayAreaSelectionAppearance;
			}
		}
		public AppearanceObject AllDayAreaSeparatorAppearance {
			get {
				if (allDayAreaSeparatorAppearance == null)
					allDayAreaSeparatorAppearance = new AppearanceObject();
				return allDayAreaSeparatorAppearance;
			}
		}
		public int ScrollOffset {
			get { return this.scrollOffset; }
			set { this.scrollOffset = value; }
		}
		#endregion
		protected internal override SchedulerViewCellBase CreateCellInstance() {
			return new TimeCell();
		}
		protected internal virtual SchedulerViewCellBase CreateExtendedCellInstance() {
			return new TimeCell();
		}
		protected internal override void UpdateSelection(SchedulerViewSelection selection) {
			SelectAllDayArea(selection);
			if (AllDayAreaCell != null && DateTimeHelper.IsIntervalWholeDays(selection.Interval))
				MakeCellsUnselected();
			else
				base.UpdateSelection(selection);
		}
		void SelectAllDayArea(SchedulerViewSelection selection) {
			if (allDayAreaCell == null)
				return;
			bool allDayAreaSelected = selection.Interval.Contains(allDayAreaCell.Interval) && Object.Equals(selection.Resource.Id, allDayAreaCell.Resource.Id);
			allDayAreaCell.Selected = allDayAreaSelected;
		}
		void MakeCellsUnselected() {
			int count = Cells.Count;
			for (int i = 0; i < count; i++)
				Cells[i].Selected = false;
		}
		public override SchedulerHitInfo CalculateHitInfo(Point pt, SchedulerHitInfo nextHitInfo) {
			SchedulerHitInfo result = base.CalculateHitInfo(pt, nextHitInfo);
			if (AllDayAreaCell != null)
				result = AllDayAreaCell.CalculateHitInfo(pt, result);
			if (!statusLineBounds.Contains(pt))
				return result;
			int count = Cells.Count;
			for (int i = 0; i < count; i++)
				if (pt.Y >= Cells[i].Bounds.Top && pt.Y <= Cells[i].Bounds.Bottom)
					return new SchedulerHitInfo(Cells[i], SchedulerHitTest.Cell, result);
			return result;
		}
		protected internal override void CalculateFinalAppearance(BaseViewAppearance appearance, SchedulerColorSchema colorSchema) {
			base.CalculateFinalAppearance(appearance, colorSchema);
			DayViewAppearance dayViewAppearance = (DayViewAppearance)appearance;
			AllDayAreaAppearance.Combine(dayViewAppearance.AllDayArea);
			AllDayAreaSelectionAppearance.Combine(dayViewAppearance.SelectedAllDayArea);
			AllDayAreaSeparatorAppearance.Combine(dayViewAppearance.AllDayAreaSeparator);
			StatusLineAppearance.Combine(dayViewAppearance.Appointment);
			if (AllDayAreaCell != null) {
				AllDayAreaCell.Appearance.Combine(AllDayAreaAppearance);
				AllDayAreaCell.SelectionAppearance.Combine(AllDayAreaSelectionAppearance);
			}
		}
		protected internal override void CalculateCellsFinalAppearance(BaseViewAppearance appearance, SchedulerColorSchema colorSchema) {
			base.CalculateCellsFinalAppearance(appearance, colorSchema);
			int count = ExtendedCells.Count;
			for (int i = 0; i < count; i++) {
				ExtendedCells[i].CalculateFinalAppearance(appearance, colorSchema);
			}
		}
		protected internal override void InitializeCell(SchedulerViewCellBase cell, TimeInterval interval) {
			base.InitializeCell(cell, interval);
			TimeCell timeCell = cell as TimeCell;
			if (timeCell != null) {
				timeCell.EndOfHour = DateTimeHelper.IsBeginOfHour(cell.Interval.End);
				timeCell.IsWorkTime = DayViewTimeCellHelper.IsWorkTime(this, interval, this.Interval);
			}
		}
		protected internal virtual void CalculateCellBorders(DayViewColumnPainter painter) {
			InitializeCellBorders();
			CalculateCellBordersCore(painter);
		}
		protected internal virtual void InitializeCellBorders() {
			if (AllDayAreaCell != null)
				InitializeAllDayCellBorders();
			InitializeTimeCellsBorders();
			InitializeExtendedCellsBorders();
		}
		protected internal virtual void InitializeAllDayCellBorders() {
			AllDayAreaCell.DrawLeftSeparator = HasLeftBorder;
		}
		protected internal virtual void CalculateCellBordersCore(DayViewColumnPainter painter) {
			TimeCellPainter cellPainter = painter.CellPainter;
			if (AllDayAreaCell != null)
				CalculateAllDayAreaCellBorders(painter, cellPainter);
			int count = Cells.Count;
			for (int i = 0; i < count; i++)
				Cells[i].CalcBorderBounds(cellPainter);
			count = ExtendedCells.Count;
			for (int i = 0; i < count; i++)
				ExtendedCells[i].CalcBorderBounds(cellPainter);
		}
		protected internal virtual void CalculateAllDayAreaCellBorders(DayViewColumnPainter painter, TimeCellPainter cellPainter) {
			AllDayAreaCell.CalcBorderBounds(cellPainter);
			Rectangle leftSeparatorBounds = RectUtils.GetLeftSideRect(AllDayAreaCell.Bounds, 1);
			leftSeparatorBounds.Inflate(0, -painter.AllDayAreaSeparatorVerticalMargin);
			allDayAreaCell.LeftSeparatorBounds = leftSeparatorBounds;
		}
		protected internal virtual void InitializeTimeCellsBorders() {
			int count = Cells.Count;
			for (int i = 0; i < count; i++)
				InitializeTimeCellBorders(Cells[i]);
		}
		protected internal virtual void InitializeExtendedCellsBorders() {
			int count = ExtendedCells.Count;
			for (int i = 0; i < count; i++)
				InitializeExtendedCellBorders(ExtendedCells[i]);
		}
		protected internal virtual void InitializeTimeCellBorders(SchedulerViewCellBase cell) {
			cell.HasLeftBorder = false;
			cell.HasRightBorder = HasRightBorder;
			cell.HasTopBorder = false;
			cell.HasBottomBorder = true;
		}
		protected internal virtual void InitializeExtendedCellBorders(SchedulerViewCellBase cell) {
			cell.HasLeftBorder = true;
			cell.HasRightBorder = HasRightBorder;
			cell.HasTopBorder = false;
			cell.HasBottomBorder = true;
		}
		protected internal override int CalculateScrollOffset() {
			return ScrollOffset;
		}
	}
}
