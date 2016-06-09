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
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.Drawing {
	public class SelectionBarLayoutCalculator : SchedulerViewLayoutCalculatorBase {
		SchedulerHeaderCollection anchorHeaders;
		SelectionBar selectionBar;
		public SelectionBarLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter, SchedulerHeaderCollection anchorHeaders)
			: base(cache, viewInfo, painter) {
			if (ViewInfo.SelectionBar == null)
				Exceptions.ThrowArgumentException("selectionBar", selectionBar);
			this.selectionBar = ViewInfo.SelectionBar;
			this.anchorHeaders = anchorHeaders;
		}
		public SchedulerHeaderCollection AnchorHeaders { get { return anchorHeaders; } }
		protected internal new SelectionBarPainter Painter { get { return (SelectionBarPainter)base.Painter; } }
		protected internal SelectionBar SelectionBar { get { return selectionBar; } }
		protected internal new TimelineView View { get { return (TimelineView)base.View; } }
		protected internal new TimelineViewInfo ViewInfo { get { return (TimelineViewInfo)base.ViewInfo; } }
		public override void CalcLayout(Rectangle bounds) {
			if (bounds == Rectangle.Empty)
				return;
			int count = AnchorHeaders.Count;
			if (count == 0)
				return;
			SchedulerViewCellBaseCollection cells = PerformSelectionBarCellsLayout();
			SelectionBar.Cells.AddRange(cells);
			SelectionBar.Bounds = CalculateSelectionBarBounds();
			UpdateSelectionBarInterval();
			UpdateSelectionBarAppearance(); 
		}
		protected internal virtual Rectangle CalculateSelectionBarBounds() {
			int count = AnchorHeaders.Count;
			Rectangle head = AnchorHeaders[0].Bounds;
			Rectangle tail = AnchorHeaders[count - 1].Bounds;
			return Rectangle.FromLTRB(head.Left, head.Bottom, tail.Right, head.Bottom + ViewInfo.CalculateSelectionBarHeight());
		}
		protected internal virtual SchedulerViewCellBase CreateCellByHeader(SchedulerHeader header, int index) {
			Rectangle r = header.AnchorBounds;
			Rectangle bounds = new Rectangle(r.Left, r.Bottom, r.Width, ViewInfo.CalculateSelectionBarHeight());
			SelectionBarCell cell = (SelectionBarCell)SelectionBar.CreateCell(header.Interval);
			cell.Bounds = bounds;
			cell.ToolTipText = header.ToolTipText;
			cell.ShouldShowToolTip = r.Height <= 0;
			cell.EndOfHour = true;
			cell.HasLeftBorder = index > 0;
			cell.HasBottomBorder = true;
			cell.CalcBorderBounds((BorderObjectPainter)Painter);
			cell.BottomBorderBounds = RectUtils.GetBottomSideRect(bounds, 1);
			return cell;
		}
		protected internal SchedulerViewCellBaseCollection PerformSelectionBarCellsLayout() {
			SchedulerViewCellBaseCollection cells = new SchedulerViewCellBaseCollection();
			int count = AnchorHeaders.Count;
			for (int i = 0; i < count; i++) {
				cells.Add(CreateCellByHeader(AnchorHeaders[i], i));
			}
			return cells;
		}
		protected internal virtual void UpdateSelectionBarAppearance() {
			SelectionBar.CalculateFinalAppearance(ViewInfo.TimelinePaintAppearance, SelectionBar.ColorSchema);
		}
		protected internal virtual void UpdateSelectionBarInterval() {
			int count = SelectionBar.Cells.Count;
			if (count > 0) {
				SelectionBar.Interval = new TimeInterval(SelectionBar.Cells[0].Interval.Start, SelectionBar.Cells[count - 1].Interval.End);
			}
		}
	}
}
