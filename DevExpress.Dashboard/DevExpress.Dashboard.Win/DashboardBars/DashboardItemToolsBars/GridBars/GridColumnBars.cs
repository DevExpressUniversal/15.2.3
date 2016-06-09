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
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.XtraBars;
namespace DevExpress.DashboardWin.Bars {
	public class GridColumnFitToContentBarItem : BarCheckItem, IDashboardViewerCommandBarItem {
		readonly GridColumnBase selectedColumn;
		readonly DashboardDesigner designer;
		readonly GridDashboardItem grid;
		readonly Dictionary<GridColumnBase, int> columnActualWidthsTable;
		public GridColumnFitToContentBarItem(DashboardDesigner designer, GridDashboardItem grid, Dictionary<GridColumnBase, int> columnActualWidthsTable, GridColumnBase selectedColumn)
			: base() {
			this.designer = designer;
			this.grid = grid;
			this.columnActualWidthsTable = columnActualWidthsTable;
			this.selectedColumn = selectedColumn;
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.GridColumnFitToContentMenuItemCaption);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.GridColumnFitToContentMenuItemDescription);
			Checked = selectedColumn.WidthType == GridColumnFixedWidthType.FitToContent;
			Glyph = ImageHelper.GetImage("Bars.GridColumnFitToContent_16x16");
			LargeGlyph = ImageHelper.GetImage("Bars.GridColumnFitToContent_32x32");
		}
		public void ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			GridColumnFitToContentHistoryItem historyItem = new GridColumnFitToContentHistoryItem(grid, columnActualWidthsTable, selectedColumn);
			historyItem.Redo(designer);
			if(designer != null)
				designer.History.Add(historyItem);
		}
	}
	public class GridColumnFixedWidthBarItem : BarCheckItem, IDashboardViewerCommandBarItem {
		readonly DashboardDesigner designer;
		readonly GridDashboardItem grid;
		readonly GridColumnBase selectedColumn;
		readonly Dictionary<GridColumnBase, int> columnActualWidthsTable;
		readonly float charWidth;
		public GridColumnFixedWidthBarItem(DashboardDesigner designer, GridDashboardItem grid, Dictionary<GridColumnBase, int> columnActualWidthsTable, GridColumnBase selectedColumn, float charWidth)
			: base() {
			this.designer = designer;
			this.grid = grid;
			this.columnActualWidthsTable = columnActualWidthsTable;
			this.selectedColumn = selectedColumn;
			this.charWidth = charWidth;
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.GridColumnFixedWidthMenuItemCaption);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.GridColumnFixedWidthMenuItemDescription);
			Checked = selectedColumn.WidthType == GridColumnFixedWidthType.FixedWidth;
			Glyph = ImageHelper.GetImage("Bars.GridColumnFixedWidth_16x16");
			LargeGlyph = ImageHelper.GetImage("Bars.GridColumnFixedWidth_32x32");
		}
		public void ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			GridColumnFixedWidthHistoryItem historyItem = new GridColumnFixedWidthHistoryItem(grid, columnActualWidthsTable, selectedColumn, selectedColumn.WidthType != GridColumnFixedWidthType.FixedWidth, columnActualWidthsTable[selectedColumn] / charWidth);
			historyItem.Redo(designer);
			if(designer != null)
				designer.History.Add(historyItem);
		}
	}
	public class GridColumnWidthBarItem : BarButtonItem, IDashboardViewerCommandBarItem {
		readonly DashboardDesigner designer;
		readonly GridDashboardItem grid;
		readonly GridColumnBase selectedColumn;
		readonly float charWidth;
		readonly Dictionary<GridColumnBase, int> columnActualWidthsTable;
		public GridColumnWidthBarItem(DashboardDesigner designer, GridDashboardItem grid, Dictionary<GridColumnBase, int> columnActualWidthsTable, GridColumnBase selectedColumn, float charWidth)
			: base() {
			this.designer = designer;
			this.grid = grid;
			this.selectedColumn = selectedColumn;
			this.charWidth = charWidth;
			this.columnActualWidthsTable = columnActualWidthsTable;
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.GridColumnWidthMenuItemCaption);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.GridColumnWidthMenuItemDescription);
			Glyph = ImageHelper.GetImage("Bars.GridColumnWidth_16x16");
			LargeGlyph = ImageHelper.GetImage("Bars.GridColumnWidth_32x32");
		}
		public void ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new GridColumnWidthCommand(designer, grid, selectedColumn, charWidth, columnActualWidthsTable).Execute();
		}
	}
	public class GridSortAscendingBarItem : BarCheckItem, IDashboardViewerCommandBarItem {
		readonly GridDashboardColumn column;
		public GridSortAscendingBarItem(GridDashboardColumn column)
			: base() {
			this.column = column;
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.GridSortAscendingMenuItemCaption);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.GridSortAscendingMenuItemDescription);
			Checked = column.SortOrder == Data.ColumnSortOrder.Ascending;
			Glyph = ImageHelper.GetImage("Bars.GridSortAscending_16x16");
			LargeGlyph = ImageHelper.GetImage("Bars.GridSortAscending_32x32");
		}
		public void ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new GridSortAscendingCommand(viewer, itemViewer, column).Execute();
		}
	}
	public class GridSortDescendingBarItem : BarCheckItem, IDashboardViewerCommandBarItem {
		readonly GridDashboardColumn column;
		public GridSortDescendingBarItem(GridDashboardColumn column)
			: base() {
			this.column = column;
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.GridSortDescendingMenuItemCaption);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.GridSortDescendingMenuItemDescription);
			Checked = column.SortOrder == Data.ColumnSortOrder.Descending;
			Glyph = ImageHelper.GetImage("Bars.GridSortDescending_16x16");
			LargeGlyph = ImageHelper.GetImage("Bars.GridSortDescending_32x32");
		}
		public void ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new GridSortDescendingCommand(viewer, itemViewer, column).Execute();
		}
	}
	public class GridClearSortingBarItem : BarButtonItem, IDashboardViewerCommandBarItem {
		public GridClearSortingBarItem()
			: base() {
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.GridClearSortingMenuItemCaption);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.GridClearSortingMenuItemDescription);
			Glyph = ImageHelper.GetImage("Bars.GridClearSorting_16x16");
			LargeGlyph = ImageHelper.GetImage("Bars.GridClearSorting_32x32");
		}
		public void ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new GridClearSortingCommand(viewer, itemViewer).Execute();
		}
	}
	public class ResetGridColumnWidthsBarItem : BarButtonItem, IDashboardViewerCommandBarItem {
		public ResetGridColumnWidthsBarItem(bool columnsResized)
			: base() {
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.ResetGridColumnsWidthMenuItemCaption);
			Description = DashboardWinLocalizer.GetString(DashboardWinStringId.ResetGridColumnsWidthMenuItemDescription);
			Enabled = columnsResized;
			Glyph = ImageHelper.GetImage("Bars.GridResetColumnWidths_16x16");
			LargeGlyph = ImageHelper.GetImage("Bars.GridResetColumnWidths_32x32");
		}
		public void ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			new ResetGridColumnsWidthCommand(viewer, itemViewer).Execute();
		}
	}
}
