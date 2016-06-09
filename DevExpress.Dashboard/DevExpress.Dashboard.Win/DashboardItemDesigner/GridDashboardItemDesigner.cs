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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.XtraBars;
namespace DevExpress.DashboardWin.Native {
	public class GridDashboardItemDesigner : DataDashboardItemDesigner {
		BarManager barManager = new BarManager();
		GridDashboardItem GridDashboardItem { get { return (GridDashboardItem)DashboardItem; } }
		GridDashboardItemViewer GridViewer { get { return (GridDashboardItemViewer)ItemViewer; } }
		GridDashboardControl GridControl { get { return GridViewer.GridControl; } }
		public IEnumerable<DashboardItemViewerPopupMenuCreator> GetColumnWidthPopupMenuCreators(int columnIndex) {
			GridDashboardItem grid = GridDashboardItem;
			GridColumnBase selectedColumns = grid.Columns[columnIndex];
			if(GridControl.DashboardView.ColumnWidthMode != GridColumnWidthMode.AutoFitToContents) {
				float charWidth = FontMeasurer.MeasureMaxDigitWidthF(GridControl.Font);
				Dictionary<GridColumnBase, int> columnActualWidthsTable = new Dictionary<GridColumnBase, int>();
				foreach(GridDashboardColumn column in GridControl.DashboardView.DashboardColumns)
					columnActualWidthsTable.Add(grid.Columns[((IGridColumn)column).ActualIndex], column.VisibleWidth);
				yield return new GridDesignerColumnWidthOptionsPopupMenuCreator(DashboardDesigner, grid, columnActualWidthsTable, selectedColumns, charWidth);
			}
			GridDimensionColumn dimensionColumn = selectedColumns as GridDimensionColumn;
			GridMeasureColumn measureColumn = selectedColumns as GridMeasureColumn;
			if(dimensionColumn != null || (measureColumn != null && measureColumn.DisplayMode != GridMeasureColumnDisplayMode.Bar)) {
				DataItem dataItem = dimensionColumn != null ? (DataItem)dimensionColumn.Dimension : measureColumn.Measure;
				if(grid.FormatRulesInternal != null && grid.IsConditionalFormattingCalculateByAllowed(dataItem)) {
					barManager.Items.Clear();
					yield return new GridDesignerConditionalFormattingPopupMenuCreator(barManager, DashboardDesigner, GridDashboardItem, dataItem);
				}
			}
		}
		public DashboardItemViewerPopupMenuCreator GetGridColumnTotalPopupMenuCreator(int columnIndex) {
			return new GridColumnTotalPopupMenuCreator(DashboardDesigner, GridDashboardItem, columnIndex);
		}
		public IEnumerable<DashboardItemViewerPopupMenuCreator> GetChangeGridColumnTotalPopupMenuCreators(int columnIndex, int summaryItemIndex) {
			yield return new ChangeGridColumnTotalPopupMenuCreator(DashboardDesigner, GridDashboardItem, columnIndex, summaryItemIndex);
			yield return new RemoveGridColumnTotalPopupMenuCreator(DashboardDesigner, GridDashboardItem, columnIndex, summaryItemIndex);
		}
		protected override void InitializeInternal() {
			base.InitializeInternal();
			GridControl.DashboardView.GridColumnWidthChanged += OnGridDashboardControlColumnWidthChanged;
		}
		void OnGridDashboardControlColumnWidthChanged(object sender, GridColumnWidthsChangedEventArgs e) {
			GridColumnBase leftColumn = GridDashboardItem.Columns[e.LeftColumnActualIndex];
			GridColumnBase rightColumn = GridDashboardItem.Columns[e.RightColumnActualIndex];
			DashboardDesigner.Dashboard.BeginUpdate();
			GridResizeColumnsHistoryItem historyItem = new GridResizeColumnsHistoryItem(GridDashboardItem, e.OptionsInfo, rightColumn, leftColumn);
			historyItem.Redo(DashboardDesigner);
			DashboardDesigner.History.Add(historyItem);
			DashboardDesigner.Dashboard.CancelUpdate();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing)
				barManager.Dispose();
		}
	}
}
