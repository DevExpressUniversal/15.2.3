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
using System.Linq;
using System.Collections.Generic;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraBars;
using DevExpress.Utils.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.Utils;
namespace DevExpress.DashboardWin.Native {
	public abstract class DashboardViewerPopupMenuCreator {
		protected abstract void FillBarItems(List<BarItem> items);
		public List<BarItem> GetBarItems() {
			List<BarItem> items = new List<BarItem>();
			FillBarItems(items);
			return items;
		}
	}
	public class ExportBarItemsPopupMenuCreator : DashboardViewerPopupMenuCreator {
		protected override void FillBarItems(List<BarItem> items) {
			items.Add(new PrintPreviewDashboardBarItem());
			items.Add(new ExportDashboardToPdfBarItem());
			items.Add(new ExportDashboardToImageBarItem());
		}
	}
	public class ParametersBarItemsPopupMenuCreator : DashboardViewerPopupMenuCreator {
		protected override void FillBarItems(List<BarItem> items) {
			items.Add(new ParametersDashboardBarItem());
		}
	}
	public abstract class DashboardItemCaptionButtonInfoCreator : DashboardItemViewerPopupMenuCreator {
		protected abstract DashboardItemCaptionButtonInfo CreateCaptionButtonInfo();
		public DashboardItemCaptionButtonInfo GetCaptionButtonInfo() {
			DashboardItemCaptionButtonInfo info = CreateCaptionButtonInfo();
			if(info != null)
				info.AddBarItems(GetBarItems());
			return info;
		}
	}
	public abstract class DashboardItemViewerPopupMenuCreator : DashboardViewerPopupMenuCreator {
		public virtual bool AllowViewerCommands { get { return true; } }
	}
	public class SelectDashboardBarItemPopupMenuCreator : DashboardItemViewerPopupMenuCreator {
		protected override void FillBarItems(List<BarItem> items) {
			items.Add(new SelectDashboardBarItem());
		}
	}
	public class SelectDashboardItemGroupBarItemPopupMenuCreator : DashboardItemViewerPopupMenuCreator {
		protected override void FillBarItems(List<BarItem> items) {
			items.Add(new SelectDashboardItemGroupBarItem());
		}
	}
	public class ExportItemBarItemsPopupMenuCreator : DashboardItemCaptionButtonInfoCreator {
		protected override void FillBarItems(List<BarItem> items) {
			items.Add(new PrintPreviewItemBarItem());
			items.Add(new ExportItemToPdfBarItem());
			items.Add(new ExportItemToImageBarItem());
		}
		protected override DashboardItemCaptionButtonInfo CreateCaptionButtonInfo() {
			return new DashboardItemCaptionButtonInfo(DashboardButtonType.Export, ImageHelper.ExportImage, ObjectState.Normal, DashboardWinLocalizer.GetString(DashboardWinStringId.CommandExportTo));
		}
	}
	public class ExportItemToExcelBarItemsPopupMenuCreator : ExportItemBarItemsPopupMenuCreator {
		protected override void FillBarItems(List<BarItem> items) {
			base.FillBarItems(items);
			items.Add(new ExportItemToExcelBarItem());
		}
	}
	public class GridDesignerColumnWidthOptionsPopupMenuCreator : DashboardItemViewerPopupMenuCreator {
		readonly DashboardDesigner designer;
		readonly GridColumnBase selectedColumn;
		readonly GridDashboardItem grid;
		readonly float charWidth;
		readonly Dictionary<GridColumnBase, int> columnActualWidthsTable;
		public GridDesignerColumnWidthOptionsPopupMenuCreator(DashboardDesigner designer, GridDashboardItem grid, Dictionary<GridColumnBase, int> columnActualWidthsTable, GridColumnBase selectedColumn, float charWidth) {
			Guard.ArgumentNotNull(designer, "designer");
			Guard.ArgumentNotNull(grid, "grid");
			Guard.ArgumentNotNull(columnActualWidthsTable, "columnActualWidthsTable");
			Guard.ArgumentNotNull(selectedColumn, "selectedColumn");
			this.designer = designer;
			this.grid = grid;
			this.columnActualWidthsTable = columnActualWidthsTable;
			this.selectedColumn = selectedColumn;
			this.charWidth = charWidth;
		}
		protected override void FillBarItems(List<BarItem> items) {
			items.Add(new GridColumnFitToContentBarItem(designer, grid, columnActualWidthsTable, selectedColumn));
			items.Add(new GridColumnFixedWidthBarItem(designer, grid, columnActualWidthsTable, selectedColumn, charWidth));
			items.Add(new GridColumnWidthBarItem(designer, grid, columnActualWidthsTable, selectedColumn, charWidth));
		}
	}
	public abstract class GridColumnTotalPopupMenuCreatorBase : DashboardItemViewerPopupMenuCreator {
		readonly DashboardDesigner designer;
		readonly GridDashboardItem grid;
		readonly int columnIndex;
		readonly int totalIndex;
		protected DashboardDesigner Designer { get { return designer; } }
		protected GridDashboardItem Grid { get { return grid; } }
		protected int ColumnIndex { get { return columnIndex; } }
		protected int TotalIndex { get { return totalIndex; } }
		protected GridColumnTotalPopupMenuCreatorBase(DashboardDesigner designer, GridDashboardItem grid, int columnIndex, int totalIndex) {
			this.designer = designer;
			this.grid = grid;
			this.columnIndex = columnIndex;
			this.totalIndex = totalIndex;
		}
	}
	public class GridColumnTotalPopupMenuCreator : GridColumnTotalPopupMenuCreatorBase {
		public GridColumnTotalPopupMenuCreator(DashboardDesigner designer, GridDashboardItem grid, int columnIndex)
			: base(designer, grid, columnIndex, 0) {
		}
		protected override void FillBarItems(List<BarItem> items) {
			GridColumnBase column = Grid.Columns[ColumnIndex];
			GridAddColumnTotalsBarItem addItem = new GridAddColumnTotalsBarItem();
			foreach(GridColumnTotalType type in column.GetAvailableTotalTypes())
				addItem.AddItem(new CreateNewGridTotalBarItem(Designer, Grid, ColumnIndex, type));
			items.Add(addItem);
			items.Add(new ClearGridColumnTotalsBarItem(Designer, Grid, ColumnIndex) { Enabled = column.Totals.Count > 0 });
		}
	}
	public class ChangeGridColumnTotalPopupMenuCreator : GridColumnTotalPopupMenuCreatorBase {
		public ChangeGridColumnTotalPopupMenuCreator(DashboardDesigner designer, GridDashboardItem grid, int columnIndex, int totalIndex)
			: base(designer, grid, columnIndex, totalIndex) {
		}
		protected override void FillBarItems(List<BarItem> items) {
			GridColumnBase column = Grid.Columns[ColumnIndex];
			GridColumnTotal total = column.Totals[TotalIndex];
			foreach(GridColumnTotalType type in column.GetAvailableTotalTypes())
				if(type != total.TotalType)
					items.Add(new ChangeGridColumnTotalTypeBarItem(Designer, Grid, ColumnIndex, TotalIndex, type));
		}
	}
	public class RemoveGridColumnTotalPopupMenuCreator : GridColumnTotalPopupMenuCreatorBase {
		public RemoveGridColumnTotalPopupMenuCreator(DashboardDesigner designer, GridDashboardItem grid, int columnIndex, int totalIndex)
			: base(designer, grid, columnIndex, totalIndex) {
		}
		protected override void FillBarItems(List<BarItem> items) {
			items.Add(new RemoveGridColumnTotalBarItem(Designer, Grid, ColumnIndex, TotalIndex));
		}
	}
	public class GridDesignerConditionalFormattingPopupMenuCreator : DashboardItemViewerPopupMenuCreator {
		readonly DashboardDesigner designer;
		readonly BarManager barManager;
		readonly GridDashboardItem grid;
		readonly DataItem dataItem;
		public override bool AllowViewerCommands { get { return false; } }
		public GridDesignerConditionalFormattingPopupMenuCreator(BarManager barManager, DashboardDesigner designer, GridDashboardItem grid, DataItem dataItem) {
			Guard.ArgumentNotNull(designer, "designer");
			Guard.ArgumentNotNull(barManager, "barManager");
			Guard.ArgumentNotNull(grid, "grid");
			Guard.ArgumentNotNull(dataItem, "dataItem");
			this.designer = designer;
			this.barManager = barManager;
			this.grid = grid;
			this.dataItem = dataItem;
		}
		protected override void FillBarItems(List<BarItem> items) {
			foreach(BarItem item in FormatRuleControlBarItemsCreator.CreateFormatRuleControlItems(barManager, designer, grid, dataItem))
				items.Add(item);
		}
	}
	public class GridViewerPopupMenuCreator : DashboardItemViewerPopupMenuCreator {
		bool columnsResized;
		public GridViewerPopupMenuCreator(bool columnsResized) {
			this.columnsResized = columnsResized;
		}
		protected override void FillBarItems(List<BarItem> items) {
			items.Add(new ResetGridColumnWidthsBarItem(columnsResized));
		}
	}
	public class GridViewerColumnPopupMenuCreator : DashboardItemViewerPopupMenuCreator {
		readonly GridDashboardColumn column;
		public GridViewerColumnPopupMenuCreator(GridDashboardColumn column) {
			this.column = column;
		}
		protected override void FillBarItems(List<BarItem> items) {
			items.Add(new GridSortAscendingBarItem(column));
			items.Add(new GridSortDescendingBarItem(column));
			items.Add(new GridClearSortingBarItem());
		}
	}
	public class SelectedItemElementsBarItemsPopupMenuCreator : DashboardItemCaptionButtonInfoCreator {
		readonly ContentDescriptionViewModel contentDescription;
		public SelectedItemElementsBarItemsPopupMenuCreator(ContentDescriptionViewModel contentDescription) {
			this.contentDescription = contentDescription;
		}
		protected override void FillBarItems(List<BarItem> items) {
			int selectedElementIndex = contentDescription.SelectedElementIndex;
			IList<string> elementNames = contentDescription.ElementNames;
			for(int i = 0; i < elementNames.Count; i++) {
				items.Add(new SelectedItemElementBarItem(i) {
					Caption = elementNames[i],
					Checked = selectedElementIndex == i
				});
			}
		}
		protected override DashboardItemCaptionButtonInfo CreateCaptionButtonInfo() {
			return new DashboardItemCaptionButtonInfo(DashboardButtonType.Values, ImageHelper.SelectOtherElementImage, ObjectState.Normal, DashboardWinLocalizer.GetString(DashboardWinStringId.CommandSelectOtherValues));
		}
	}
	public class DrillUpPopupMenuCreator : DashboardItemCaptionButtonInfoCreator {
		public bool Enabled { get; set; }
		protected override void FillBarItems(List<BarItem> items) {
			items.Add(new DrillUpBarItem() { Enabled = Enabled });
		}
		protected override DashboardItemCaptionButtonInfo CreateCaptionButtonInfo() {
			return new DashboardItemCaptionButtonInfo(DashboardButtonType.DrillUp, ImageHelper.DrillUpImage,
				Enabled ? ObjectState.Normal : ObjectState.Disabled, DashboardWinLocalizer.GetString(DashboardWinStringId.CommandDrillUp));
		}
	}
	public class ClearMasterFilterPopupMenuCreator : DashboardItemCaptionButtonInfoCreator {
		public bool Enabled { get; set; }
		protected override void FillBarItems(List<BarItem> items) {
			items.Add(new ClearMasterFilterBarItem() { Enabled = Enabled });
		}
		protected override DashboardItemCaptionButtonInfo CreateCaptionButtonInfo() {
			return new DashboardItemCaptionButtonInfo(DashboardButtonType.ClearMasterFilter, ImageHelper.ClearMasterFilterImage,
				Enabled ? ObjectState.Normal : ObjectState.Disabled, DashboardWinLocalizer.GetString(DashboardWinStringId.CommandClearMasterFilter));
		}
	}
	public class ClearSelectionPopupMenuCreator : DashboardItemCaptionButtonInfoCreator {
		public bool Enabled { get; set; }
		protected override void FillBarItems(List<BarItem> items) {
			items.Add(new ClearSelectionBarItem() { Enabled = Enabled });
		}
		protected override DashboardItemCaptionButtonInfo CreateCaptionButtonInfo() {
			return new DashboardItemCaptionButtonInfo(DashboardButtonType.ClearSelection, ImageHelper.ClearSelectionImage,
				Enabled ? ObjectState.Normal : ObjectState.Disabled, DashboardWinLocalizer.GetString(DashboardWinStringId.CommandClearSelection));
		}
	}
	public class MapInitialExtentPopupMenuCreator : DashboardItemCaptionButtonInfoCreator {
		protected override void FillBarItems(List<BarItem> items) {
			items.Add(new MapInitialExtentBarItem());
		}
		protected override DashboardItemCaptionButtonInfo CreateCaptionButtonInfo() {
			return new DashboardItemCaptionButtonInfo(DashboardButtonType.MapInitialExtent, ImageHelper.MapInitialExtent, ObjectState.Normal, DashboardWinLocalizer.GetString(DashboardWinStringId.CommandMapInitialExtentCaption));
		}
	}
}
