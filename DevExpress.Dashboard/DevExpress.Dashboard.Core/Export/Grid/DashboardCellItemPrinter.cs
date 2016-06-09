#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Text;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.PivotGrid.Printing;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.DashboardExport {
	public class DashboardCellItemPrinter : PivotGridWebPrinter {
		List<int> columnWidths;
		int emptySpaceHeight = 0;
		int visibleRowCount = 0;
		protected internal bool ShowHScroll { get; protected set; }
		protected internal bool ShowVScroll { get; protected set; }
		protected List<int> ColumnWidths { get { return columnWidths; } set { columnWidths = value; } }
		protected int VisibleRowCount { get { return visibleRowCount; } set { visibleRowCount = value; } }
		protected int VisibleDataAreaHeight { get; set; }
		protected int VisibleDataAreaWidth { get; set; }
		int AllRowHeightSum {
			get { return CellSizeProvider.GetHeightDifference(false, 0, VisualItems.GetLastLevelItemCount(false)); }
		}
		public DashboardCellItemPrinter(IPivotGridPrinterOwner owner, PivotGridData data, PrintAppearance appearance)
			: base(owner, data, appearance) {
		}
		protected void CalcScrollVisibilityAndColumnsWidths(DashboardExportMode mode, ItemViewerClientState clientState, int pathRowIndex, int columnOffset) {
			int initialScrollableAreaWidth = clientState.ViewerArea.Width;
			CalculateVScrollPositionAndSize(clientState, pathRowIndex);
			bool approxShowVScroll = clientState.VScrollingState.VirtualSize > clientState.VScrollingState.ScrollableAreaSize;
			clientState.ViewerArea.Width -= approxShowVScroll ? ExportScrollBar.PrintSize : 0;
			VisibleDataAreaWidth -= approxShowVScroll ? ExportScrollBar.PrintSize : 0;
			this.columnWidths = CalcColumnWidths();
			CalculateHScrollPositionAndSize(clientState, columnOffset);
			ExportScrollController showScrollBarCalculator = new ExportScrollController(clientState);
			showScrollBarCalculator.CalculateShowScrollbars(false, initialScrollableAreaWidth, true);
			if(showScrollBarCalculator.ShowHScroll) {
				clientState.ViewerArea.Height -= ExportScrollBar.PrintSize;
				VisibleDataAreaHeight -= ExportScrollBar.PrintSize;
				clientState.VScrollingState.ScrollableAreaSize -= ExportScrollBar.PrintSize;
				this.visibleRowCount = CalcVisibleRowCount(mode, pathRowIndex);
				clientState.VScrollingState.VirtualSize = AllRowHeightSum + emptySpaceHeight;
				showScrollBarCalculator.CalculateShowScrollbars(false, initialScrollableAreaWidth, false);
			}
			clientState.ViewerArea.Width -= !approxShowVScroll && showScrollBarCalculator.ShowVScroll ? ExportScrollBar.PrintSize : 0;
			VisibleDataAreaWidth -= !approxShowVScroll && showScrollBarCalculator.ShowVScroll ? ExportScrollBar.PrintSize : 0;
			if(!approxShowVScroll && showScrollBarCalculator.ShowVScroll) {
				this.columnWidths = CalcColumnWidths();
				CalculateHScrollPositionAndSize(clientState, columnOffset);
			}
			showScrollBarCalculator.CalculateShowScrollbars(false, initialScrollableAreaWidth, false);
			ShowVScroll = showScrollBarCalculator.ShowVScroll;
			ShowHScroll = showScrollBarCalculator.ShowHScroll;
		}
		protected virtual List<int> CalcColumnWidths() {
			List<int> columnWidths = new List<int>();
			int columnCount = VisualItems.GetLastLevelItemCount(true);
			for(int i = 0; i < columnCount; i++)
				columnWidths.Add(CellSizeProvider.GetWidthDifference(true, i, i + 1));
			return columnWidths;
		}
		protected int CalcVisibleRowCount(DashboardExportMode mode, int pathRowIndex) {
			int allRowCount = Data.VisualItems.RowCount;
			if(mode == DashboardExportMode.EntireDashboard) {
				int count = 0;
				int toPrint = VisibleDataAreaHeight;
				int bottomRowCount = pathRowIndex > 0 ? allRowCount - pathRowIndex : allRowCount;
				while(toPrint > 0 && count < bottomRowCount)
					toPrint -= CellSizeProvider.GetHeightDifference(false, count, ++count);
				emptySpaceHeight = toPrint >= 0 ? toPrint : 0;
				return count;
			}
			return allRowCount;
		}
		void CalculateVScrollPositionAndSize(ItemViewerClientState clientState, int pathRowIndex) {
			int topInvisibleRowHeightSum = CellSizeProvider.GetHeightDifference(false, 0, Math.Max(0, pathRowIndex));
			if(clientState.VScrollingState == null)
				clientState.VScrollingState = new ScrollingState();
			clientState.VScrollingState.VirtualSize = AllRowHeightSum + emptySpaceHeight;
			clientState.VScrollingState.ScrollableAreaSize = VisibleDataAreaHeight;
			clientState.VScrollingState.PositionRatio = (double)topInvisibleRowHeightSum / (double)clientState.VScrollingState.VirtualSize;
		}
		void CalculateHScrollPositionAndSize(ItemViewerClientState clientState, int columnOffset) {
			if(clientState.HScrollingState == null)
				clientState.HScrollingState = new ScrollingState();
			int allColumnWidthSum = 0;
			int leftInvisibleColumnWidthSum = 0;
			int rightColumnWidthSum = 0;
			for(int i = 0; i < columnWidths.Count; i++) {
				allColumnWidthSum += columnWidths[i];
				if(i < columnOffset)
					leftInvisibleColumnWidthSum += columnWidths[i];
				else
					rightColumnWidthSum += columnWidths[i];
			}
			int emptySpaceWidth = clientState.ViewerArea.Width - rightColumnWidthSum > 0 ? clientState.ViewerArea.Width - rightColumnWidthSum : 0;
			clientState.HScrollingState.VirtualSize = allColumnWidthSum + emptySpaceWidth;
			clientState.HScrollingState.ScrollableAreaSize = VisibleDataAreaWidth;
			clientState.HScrollingState.PositionRatio = (double)(leftInvisibleColumnWidthSum) / (double)clientState.HScrollingState.VirtualSize;
		}
	}
}
