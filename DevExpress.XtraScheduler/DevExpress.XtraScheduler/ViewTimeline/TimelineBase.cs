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

using System.Drawing;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class TimelineBase : SchedulerViewCellContainer {
		WorkDaysCollection workDays;
		ISupportTimeline timelineSupport;
		protected TimelineBase(ISupportTimeline timeLineSupport, Resource resource, TimeInterval interval, SchedulerColorSchema colorSchema)
			: base(colorSchema) {
			if (timeLineSupport == null)
				Exceptions.ThrowArgumentException("timeLineSupport", timeLineSupport);
			if (resource == null)
				Exceptions.ThrowArgumentException("resource", resource);
			if (interval == null)
				Exceptions.ThrowArgumentException("interval", interval);
			this.timelineSupport = timeLineSupport;
			this.workDays = new WorkDaysCollection();
			this.workDays.AddRange(timeLineSupport.WorkDays.Clone());
			Resource = resource;
			Interval = interval;
		}
		public override SchedulerHitTest HitTestType { get { return SchedulerHitTest.Timeline; } }
		public ISupportTimeline TimelineSupport { get { return timelineSupport; } }
		public WorkDaysCollection WorkDays { get { return workDays; } }
		public virtual void CalcLayout(ISchedulerObjectAnchorCollection anchors, BorderObjectPainter cellsPainter) {
			CalcLayout(anchors, cellsPainter, TimelineSupport.TimelinePaintAppearance);
		}
		public virtual void CalcLayout(ISchedulerObjectAnchorCollection anchors, BorderObjectPainter cellsPainter, BaseViewAppearance appearance) {
			CalculateCellsBounds(anchors);
			CalculateFinalAppearance(appearance, ColorSchema);
			CalculateCellsBorders(cellsPainter);
		}
		protected internal virtual Rectangle CalculateCellBounds(Rectangle anchorBounds) {
			return Rectangle.FromLTRB(anchorBounds.Left, Bounds.Top, anchorBounds.Right, Bounds.Bottom);
		}
		protected internal virtual void CalculateCellsBorders(BorderObjectPainter cellsPainter) {
			InitializeCellsBorders();
			CalculateCellsBordersCore(cellsPainter);
		}
		protected internal virtual void CalculateCellsBordersCore(BorderObjectPainter cellsPainter) {
			int count = Cells.Count;
			for (int i = 0; i < count; i++)
				Cells[i].CalcBorderBounds(cellsPainter);
		}
		protected internal override SchedulerViewCellBase CreateCellInstance() {
			return new TimeCell();
		}
		protected internal virtual void CreateCellsByAnchors(ISchedulerObjectAnchorCollection anchors) {
			Cells.Clear();
			int count = anchors.Count;
			for (int i = 0; i < count; i++) {
				ISchedulerObjectAnchor anchor = anchors[i];
				TimeInterval interval = anchor.Interval.Clone();
				TimeCell cell = (TimeCell)CreateCell(interval);
				cell.Bounds = CalculateCellBounds(anchor.Bounds);
				Cells.Add(cell);
			}
		}
		protected internal abstract TimelineWorkTimeCalculatorBase CreateWorkTimeCalculator();
		protected internal override void InitializeCell(SchedulerViewCellBase cell, TimeInterval interval) {
			base.InitializeCell(cell, interval);
			TimeCell timeCell = (TimeCell)cell;
			timeCell.EndOfHour = DateTimeHelper.IsBeginOfHour(cell.Interval.Start);
			TimelineWorkTimeCalculatorBase workTimeCalculator = CreateWorkTimeCalculator();
			timeCell.IsWorkTime = workTimeCalculator.CalcIsWorkTime(cell);
		}
		protected internal virtual void InitializeCellBorders(TimeCell cell, int columnIndex) {
			cell.HasLeftBorder = columnIndex > 0;
			cell.HasRightBorder = false;
			cell.HasTopBorder = HasTopBorder;
			cell.HasBottomBorder = HasBottomBorder;
		}
		protected internal virtual void InitializeCellsBorders() {
			for (int i = 0; i < Cells.Count; i++)
				InitializeCellBorders((TimeCell)Cells[i], i);
		}
		internal void SetViewInfo(ISupportTimeline timelineSupport) {
			this.timelineSupport = timelineSupport;
		}
		void CalculateCellsBounds(ISchedulerObjectAnchorCollection anchors) {
			for (int i = 0; i < Cells.Count; i++) {
				ISchedulerObjectAnchor anchor = anchors[i];
				Cells[i].Bounds = CalculateCellBounds(anchor.Bounds);
			}
		}
	}
}
