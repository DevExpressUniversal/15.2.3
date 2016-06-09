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
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.CodedUISupport;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.Customization;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Customization;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
namespace DevExpress.XtraGrid.CodedUISupport {
	class GridCodedUIHelper : IGridCodedUIHelper {
		RemoteObject remoteObject;
		public GridCodedUIHelper(RemoteObject remoteObject) {
			this.remoteObject = remoteObject;
		}
		public GridControlElements GetGridElementFromPoint(IntPtr windowHandle, int pointX, int pointY, out int rowHandle, out string columnName, out string viewName) {
			columnName = viewName = null;
			rowHandle = 0;
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			if(gridControl == null)
				return GridControlElements.Unknown;
			BaseHitInfo hitInfo = null;
			BaseView viewFromPoint = GetViewFromPoint(gridControl, new Point(pointX, pointY), ref hitInfo);
			viewName = GetViewName(viewFromPoint);
			if(viewFromPoint is AdvBandedGridView)
				return GetAdvBandedGridViewElementFromPoint(viewFromPoint as AdvBandedGridView, hitInfo as BandedGridHitInfo, ref rowHandle, ref columnName);
			if(viewFromPoint is BandedGridView)
				return GetBandedGridViewElementFromPoint(viewFromPoint as BandedGridView, hitInfo as BandedGridHitInfo, ref rowHandle, ref columnName);
			else if(viewFromPoint is GridView)
				return GetGridViewElementFromPoint(viewFromPoint as GridView, hitInfo as GridHitInfo, ref rowHandle, ref columnName);
			else if(viewFromPoint is CardView)
				return GetCardViewElementFromPoint(viewFromPoint as CardView, hitInfo as CardHitInfo, ref rowHandle, ref columnName);
			return GridControlElements.Unknown;
		}
		protected BaseView GetViewFromPoint(GridControl gridControl, Point clientPoint, ref BaseHitInfo hitInfo) {
			foreach(BaseView view in gridControl.Views) {
				hitInfo = view.CalcHitInfo(clientPoint);
				if(hitInfo.HitTestInt != (int)GridHitTest.None)
					if(hitInfo.HitTestInt != (int)GridHitTest.RowDetail && hitInfo.HitTestInt != (int)GridHitTest.RowDetailEdge && hitInfo.HitTestInt != (int)GridHitTest.RowDetailIndicator)
						return view;
			}
			return null;
		}
		protected GridControlElements GetAdvBandedGridViewElementFromPoint(AdvBandedGridView advBandedGridView, BandedGridHitInfo hitInfo, ref int rowHandle, ref string columnName) {
			if(hitInfo.HitTest == BandedGridHitTest.Column) {
				columnName = GetColumnName(hitInfo.Column);
				return GridControlElements.BandedColumnHeader;
			}
			else if(hitInfo.HitTest == BandedGridHitTest.GroupPanelColumn) {
				columnName = GetColumnName(hitInfo.Column);
				return GridControlElements.GroupPanelBandedColumn;
			}
			else return GetBandedGridViewElementFromPoint(advBandedGridView, hitInfo, ref rowHandle, ref columnName);
		}
		protected GridControlElements GetBandedGridViewElementFromPoint(BandedGridView bandedGridView, BandedGridHitInfo hitInfo, ref int rowHandle, ref string columnName) {
			switch(hitInfo.HitTest) {
				case BandedGridHitTest.Band:
					columnName = GetBandName(hitInfo.Band);
					return GridControlElements.Band;
				case BandedGridHitTest.BandEdge:
					columnName = GetBandName(hitInfo.Band);
					return GridControlElements.BandEdge;
			}
			return GetGridViewElementFromPoint(bandedGridView, hitInfo, ref rowHandle, ref columnName);
		}
		protected GridControlElements GetGridViewElementFromPoint(GridView gridView, GridHitInfo hitInfo, ref int rowHandle, ref string columnName) {
			rowHandle = hitInfo.RowHandle;
			if(hitInfo.Column != null)
				columnName = GetColumnName(hitInfo.Column);
			switch(hitInfo.HitTest) {
				case GridHitTest.Column:
					return GridControlElements.ColumnHeader;
				case GridHitTest.ColumnEdge:
					return GridControlElements.ColumnEdge;
				case GridHitTest.GroupPanelColumnFilterButton:
					return GridControlElements.GroupPanelColumnFilterButton;
				case GridHitTest.ColumnFilterButton:
					return GridControlElements.ColumnFilterButton;
				case GridHitTest.GroupPanel:
					return GridControlElements.GroupPanel;
				case GridHitTest.GroupPanelColumn:
					return GridControlElements.GroupPanelColumn;
				case GridHitTest.Row:
					GridGroupRowInfo groupRowInfo = gridView.ViewInfo.GetGridRowInfo(rowHandle) as GridGroupRowInfo;
					if(groupRowInfo != null) {
						columnName = GetColumnName(groupRowInfo.Column);
						return GridControlElements.GroupRow;
					}
					else {
						if(gridView.FocusedColumn != null) {
							columnName = GetColumnName(gridView.FocusedColumn);
							return GridControlElements.Cell;
						}
						else return GridControlElements.Row;
					}
				case GridHitTest.RowPreview:
					rowHandle = hitInfo.RowHandle;
					return GridControlElements.RowPreview;
				case GridHitTest.RowEdge:
					if(gridView.FocusedColumn != null) {
						columnName = GetColumnName(gridView.FocusedColumn);
						return GridControlElements.Cell;
					}
					else return GridControlElements.Row;
				case GridHitTest.RowIndicator:
					return GridControlElements.RowIndicator;
				case GridHitTest.RowGroupButton:
					return GridControlElements.RowGroupButton;
				case GridHitTest.RowCell:
					return GridControlElements.Cell;
				case GridHitTest.CellButton:
					return GridControlElements.CellButton;
				case GridHitTest.RowFooter:
					if(hitInfo.FooterCell != null) {
						if(GetRowFooterCellRowColumn(gridView, hitInfo, ref rowHandle, ref columnName))
							return GridControlElements.RowFooterCell;
					}
					else if(GetRowFooterRowColumn(gridView, ref rowHandle, ref columnName))
						return GridControlElements.RowFooter;
					break;
				case GridHitTest.FilterPanel:
					return GridControlElements.FilterPanel;
				case GridHitTest.FilterPanelText:
					return GridControlElements.FilterPanelText;
				case GridHitTest.FilterPanelActiveButton:
					return GridControlElements.FilterPanelActiveButton;
				case GridHitTest.FilterPanelCloseButton:
					return GridControlElements.FilterPanelCloseButton;
				case GridHitTest.FilterPanelCustomizeButton:
					return GridControlElements.FilterPanelCustomizeButton;
				case GridHitTest.FilterPanelMRUButton:
					return GridControlElements.FilterPanelMRUButton;
				case GridHitTest.FixedLeftDiv:
					return GridControlElements.FixedLeftDiv;
				case GridHitTest.FixedRightDiv:
					return GridControlElements.FixedRightDiv;
				case GridHitTest.Footer:
					if(hitInfo.FooterCell != null) {
						rowHandle = GetFooterCellRow(gridView, hitInfo);
						return GridControlElements.FooterCell;
					}
					return GridControlElements.Footer;
				case GridHitTest.MasterTabPageHeader:
					string masterTabPageHeaderName = GetMasterTabPageHeaderName(gridView.TabControl, hitInfo.MasterTabRelationIndex);
					if(masterTabPageHeaderName != null) {
						columnName = masterTabPageHeaderName;
						return GridControlElements.MasterTabPageHeader;
					}
					break;
			}
			return GridControlElements.Unknown;
		}
		protected GridControlElements GetCardViewElementFromPoint(CardView cardView, CardHitInfo hitInfo, ref int rowHandle, ref string columnName) {
			switch(hitInfo.HitTest) {
				case CardHitTest.MasterTabPageHeader:
					string masterTabPageHeaderName = GetMasterTabPageHeaderName(cardView.TabControl, hitInfo.MasterTabRelationIndex);
					if(masterTabPageHeaderName != null) {
						columnName = masterTabPageHeaderName;
						return GridControlElements.MasterTabPageHeader;
					}
					break;
			}
			return GridControlElements.Unknown;
		}
		protected string GetMasterTabPageHeaderName(Tab.ViewTab tabControl, int masterTabRelationIndex) {
			if(tabControl != null && tabControl.Pages != null)
				for(int i = 0; i < tabControl.Pages.Count; i++)
					if(tabControl.Pages[i].DetailInfo.RelationIndex == masterTabRelationIndex)
						return i.ToString();
			return null;
		}
		protected Rectangle GetMasterTabPageHeaderRectangle(Tab.ViewTab tabControl, string tabName) {
			int index;
			if(Int32.TryParse(tabName, out index)) {
				if(tabControl != null && tabControl.ViewInfo != null && tabControl.ViewInfo.HeaderInfo != null && tabControl.ViewInfo.HeaderInfo.VisiblePages != null)
					if(index >= 0 && index < tabControl.ViewInfo.HeaderInfo.VisiblePages.Count)
						return tabControl.ViewInfo.HeaderInfo.VisiblePages[index].Bounds;
			}
			return Rectangle.Empty;
		}
		protected bool GetRowFooterCellRowColumn(GridView gridView, GridHitInfo hitInfo, ref int rowHandle, ref string columnName) {
			if(rowHandle >= 0) {
				rowHandle = gridView.GetParentRowHandle(rowHandle);
			}
			GridRowInfo rowInfo = gridView.ViewInfo.GetGridRowInfo(gridView.GetChildRowHandle(rowHandle, gridView.GetChildRowCount(rowHandle) - 1));
			if(rowInfo == null)
				rowInfo = gridView.ViewInfo.GetGridRowInfo(rowHandle);
			if(rowInfo != null) {
				for(int i = 0; i < rowInfo.RowFooters.Count; i++)
					foreach(FooterCellInfoArgs footerCell in rowInfo.RowFooters[i].Cells)
						if(footerCell == hitInfo.FooterCell) {
							if(i != 0)
								columnName += "," + i.ToString();
							return true;
						}
			}
			return false;
		}
		protected bool GetRowFooterRowColumn(GridView gridView, ref int rowHandle, ref string columnName) {
			if(rowHandle >= 0) {
				rowHandle = gridView.GetParentRowHandle(rowHandle);
			}
			GridGroupRowInfo grouprowInfo = gridView.ViewInfo.GetGridRowInfo(rowHandle) as GridGroupRowInfo;
			if(grouprowInfo != null) {
				columnName = GetColumnName(grouprowInfo.Column);
				return true;
			}
			return false;
		}
		protected int GetFooterCellRow(GridView gridView, GridHitInfo hitInfo) {
			int row = 0;
			foreach(GridFooterCellInfoArgs cell in gridView.ViewInfo.FooterInfo.Cells) {
				if(cell == hitInfo.FooterCell)
					break;
				if(cell.Column == hitInfo.FooterCell.Column)
					row++;
			}
			return row;
		}
		public string HandleViewViaReflection(IntPtr gridHandle, string viewName, string membersString, string newValue, string newValueType, bool isSet) {
			GridControl grid = Control.FromHandle(gridHandle) as GridControl;
			if(grid != null) {
				BaseView view = GetViewFromName(grid, viewName);
				if(view != null) {
					ClientSideHelper csh = new ClientSideHelper(this.remoteObject);
					for(int i = 0; i < grid.Views.Count; i++)
						if(grid.Views[i] == view)
							return csh.HandleViaReflection(gridHandle, "Views[" + i + "]." + membersString, newValue, newValueType, null, isSet);
				}
			}
			return null;
		}
		public string GetGridElementRectangleOrMakeElementVisible(IntPtr windowHandle, int rowHandle, string columnName, string viewName, GridControlElements elementType) {
			Rectangle elementRectangle = Rectangle.Empty;
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			BaseView baseView = GetViewFromName(gridControl, viewName);
			if(baseView is BandedGridView)
				elementRectangle = GetBandedGridViewElementRectangle(gridControl, baseView as BandedGridView, rowHandle, columnName, elementType);
			else if(baseView is GridView) {
				elementRectangle = GetGridViewElementRectangle(gridControl, baseView as GridView, rowHandle, columnName, elementType);
			}
			else if(baseView is CardView) {
				elementRectangle = GetCardViewElementRectangle(gridControl, baseView as CardView, rowHandle, columnName, elementType);
			}
			if(elementRectangle != Rectangle.Empty)
				return CodedUIUtils.ConvertToString(elementRectangle);
			else return null;
		}
		protected Rectangle GetGridViewElementRectangle(GridControl gridControl, GridView gridView, int rowHandle, string columnName, GridControlElements elementType) {
			Rectangle elementRectangle;
			GridRowInfo rowInfo = null;
			GridGroupRowInfo groupRowInfo = null;
			GridColumn gridColumn = null;
			GridColumnInfoArgs columnInfo = null;
			GridFooterCellInfoArgs footerCell = null;
			GridViewInfo viewInfo = (GridViewInfo)gridView.GetViewInfo();
			try {
				switch(elementType) {
					case GridControlElements.GroupPanel:
						return viewInfo.ViewRects.GroupPanel;
					case GridControlElements.FilterPanel:
						if(viewInfo.FilterPanel != null)
							return viewInfo.FilterPanel.Bounds;
						else return Rectangle.Empty;
					case GridControlElements.FilterPanelActiveButton:
						if(viewInfo.FilterPanel.ActiveButtonInfo != null)
							return viewInfo.FilterPanel.ActiveButtonInfo.Bounds;
						else return Rectangle.Empty;
					case GridControlElements.FilterPanelCloseButton:
						if(viewInfo.FilterPanel.CloseButtonInfo != null)
							return viewInfo.FilterPanel.CloseButtonInfo.Bounds;
						else return Rectangle.Empty;
					case GridControlElements.FilterPanelCustomizeButton:
						if(viewInfo.FilterPanel.CustomizeButtonInfo != null)
							return viewInfo.FilterPanel.CustomizeButtonInfo.Bounds;
						else return Rectangle.Empty;
					case GridControlElements.FilterPanelMRUButton:
						if(viewInfo.FilterPanel.MRUButtonInfo != null)
							return viewInfo.FilterPanel.MRUButtonInfo.Bounds;
						else return Rectangle.Empty;
					case GridControlElements.FilterPanelText:
						return viewInfo.FilterPanel.TextBounds;
					case GridControlElements.FixedLeftDiv:
						if(viewInfo.HasFixedLeft) {
							elementRectangle = viewInfo.ViewRects.FixedLeft;
							elementRectangle.Width = gridView.FixedLineWidth;
							elementRectangle.X = viewInfo.ViewRects.FixedLeft.Right - elementRectangle.Width;
							if(viewInfo.GroupPanel != null) {
								elementRectangle.Y = elementRectangle.Y + viewInfo.ViewRects.GroupPanel.Height;
								elementRectangle.Height = elementRectangle.Height - viewInfo.ViewRects.GroupPanel.Height;
							}
							return elementRectangle;
						}
						return Rectangle.Empty;
					case GridControlElements.FixedRightDiv:
						if(viewInfo.HasFixedRight) {
							elementRectangle = viewInfo.ViewRects.FixedRight;
							elementRectangle.Width = gridView.FixedLineWidth;
							elementRectangle.X = elementRectangle.X - elementRectangle.Width;
							if(viewInfo.GroupPanel != null) {
								elementRectangle.Y = elementRectangle.Y + viewInfo.ViewRects.GroupPanel.Height;
								elementRectangle.Height = elementRectangle.Height - viewInfo.ViewRects.GroupPanel.Height;
							}
							return elementRectangle;
						}
						return Rectangle.Empty;
					case GridControlElements.Footer:
						if(viewInfo.FooterInfo != null)
							return viewInfo.FooterInfo.Bounds;
						else return Rectangle.Empty;
					case GridControlElements.FooterCell:
						footerCell = GetFooterCell(gridView, GridControlElements.FooterCell, rowHandle, columnName);
						if(footerCell != null)
							return footerCell.Bounds;
						else return Rectangle.Empty;
					case GridControlElements.GroupRow:
					case GridControlElements.Row:
					case GridControlElements.RowPreview:
						rowInfo = viewInfo.GetGridRowInfo(rowHandle);
						if(rowInfo == null) {
							gridControl.BeginInvoke(new MethodInvoker(
									delegate() {
										gridView.MakeRowVisible(rowHandle);
									})
								);
							return Rectangle.Empty;
						}
						if(elementType == GridControlElements.RowPreview && rowInfo is GridDataRowInfo)
							return (rowInfo as GridDataRowInfo).PreviewBounds;
						else return rowInfo.Bounds;
					case GridControlElements.RowGroupButton:
						groupRowInfo = viewInfo.GetGridRowInfo(rowHandle) as GridGroupRowInfo;
						if(groupRowInfo == null) {
							gridControl.BeginInvoke(new MethodInvoker(
									delegate() {
										gridView.MakeRowVisible(rowHandle);
									})
								);
							return Rectangle.Empty;
						}
						else return groupRowInfo.ButtonBounds;
					case GridControlElements.RowIndicator:
						rowInfo = viewInfo.GetGridRowInfo(rowHandle) as GridRowInfo;
						if(rowInfo == null) {
							gridControl.BeginInvoke(new MethodInvoker(
									delegate() {
										gridView.MakeRowVisible(rowHandle);
									})
								);
							return Rectangle.Empty;
						}
						else return rowInfo.IndicatorRect;
					case GridControlElements.RowFooter:
					case GridControlElements.RowFooterCell:
						int childRowHandle = gridView.GetChildRowHandle(rowHandle, gridView.GetChildRowCount(rowHandle) - 1);
						rowInfo = viewInfo.GetGridRowInfo(childRowHandle);
						if(rowInfo == null)
							rowInfo = viewInfo.GetGridRowInfo(rowHandle);
						if(rowInfo == null) {
							gridControl.BeginInvoke(new MethodInvoker(
									delegate() {
										gridView.MakeRowVisible(childRowHandle);
									})
								);
							return Rectangle.Empty;
						}
						else {
							if(elementType == GridControlElements.RowFooter)
								return rowInfo.RowFooters.Bounds;
							else {
								footerCell = GetFooterCell(gridView, GridControlElements.RowFooterCell, rowHandle, columnName);
								if(footerCell != null)
									return footerCell.Bounds;
							}
							return Rectangle.Empty;
						}
					case GridControlElements.MasterTabPageHeader:
						return GetMasterTabPageHeaderRectangle(gridView.TabControl, columnName);
				}
				gridColumn = GetColumnFromName(gridView, columnName);
				if(gridColumn == null) return Rectangle.Empty;
				switch(elementType) {
					case GridControlElements.ColumnHeader:
					case GridControlElements.BandedColumnHeader:
						columnInfo = viewInfo.ColumnsInfo[gridColumn];
						if(columnInfo != null)
							return columnInfo.Bounds;
						else return Rectangle.Empty;
					case GridControlElements.ColumnEdge:
						columnInfo = viewInfo.ColumnsInfo[gridColumn];
						if(columnInfo != null) {
							Rectangle columnRect = columnInfo.Bounds;
							if(gridColumn == viewInfo.FixedLeftColumn)
								return new Rectangle(columnRect.Right - ControlUtils.ColumnResizeEdgeSize, columnRect.Top, ControlUtils.ColumnResizeEdgeSize, columnRect.Height);
							else return new Rectangle(columnRect.Right - ControlUtils.ColumnResizeEdgeSize, columnRect.Top, ControlUtils.ColumnResizeEdgeSize * 2, columnRect.Height);
						}
						else return Rectangle.Empty;
					case GridControlElements.GroupPanelColumn:
					case GridControlElements.GroupPanelBandedColumn:
						columnInfo = viewInfo.GroupPanel.Rows.GetColumnInfo(gridColumn);
						if(columnInfo != null)
							return columnInfo.Bounds;
						else return Rectangle.Empty;
					case GridControlElements.ColumnFilterButton:
						columnInfo = viewInfo.ColumnsInfo[gridColumn];
						if(columnInfo != null)
							foreach(DrawElementInfo elementInfo in columnInfo.InnerElements)
								if(elementInfo.ElementInfo is GridFilterButtonInfoArgs) {
									if(!elementInfo.Visible) {
										return columnInfo.Bounds;
									}
									else {
										return elementInfo.ElementInfo.Bounds;
									}
								}
						return Rectangle.Empty;
					case GridControlElements.GroupPanelColumnFilterButton:
						columnInfo = viewInfo.GroupPanel.Rows.GetColumnInfo(gridColumn);
						if(columnInfo != null)
							foreach(DrawElementInfo elementInfo in columnInfo.InnerElements)
								if(elementInfo.ElementInfo is GridFilterButtonInfoArgs) {
									if(!elementInfo.Visible) {
										return columnInfo.Bounds;
									}
									else {
										return elementInfo.ElementInfo.Bounds;
									}
								}
						return Rectangle.Empty;
					case GridControlElements.Cell:
					case GridControlElements.CellButton:
						GridCellInfo cell = viewInfo.GetGridCellInfo(rowHandle, gridColumn);
						if(cell == null) {
							gridControl.BeginInvoke(new MethodInvoker(
									delegate() {
										gridView.MakeRowVisible(rowHandle);
										gridView.MakeColumnVisible(gridColumn);
										cell = viewInfo.GetGridCellInfo(rowHandle, gridColumn);
									})
								);
							return Rectangle.Empty;
						}
						else {
							return elementType == GridControlElements.CellButton ? cell.CellButtonRect : cell.Bounds;
						}
				}
			}
			catch { }
			return Rectangle.Empty;
		}
		protected Rectangle GetBandedGridViewElementRectangle(GridControl gridControl, BandedGridView bandedView, int rowHandle, string columnName, GridControlElements elementType) {
			GridBand gridBand = null;
			BandedGridViewInfo viewInfo = bandedView.ViewInfo as BandedGridViewInfo;
			switch(elementType) {
				case GridControlElements.Band:
					gridBand = GetBandFromName(bandedView, columnName);
					if(gridBand != null) {
						GridBandInfoArgs bandInfo = GetBandInfo(viewInfo, gridBand);
						if(bandInfo != null)
							return bandInfo.Bounds;
					}
					return Rectangle.Empty;
				case GridControlElements.BandEdge:
					gridBand = GetBandFromName(bandedView, columnName);
					if(gridBand != null) {
						if(gridBand != null) {
							GridBandInfoArgs bandInfo = GetBandInfo(viewInfo, gridBand);
							if(bandInfo != null) {
								Rectangle bandRect = bandInfo.Bounds;
								if(gridBand == viewInfo.FixedLeftBand)
									return new Rectangle(bandRect.Right - ControlUtils.ColumnResizeEdgeSize, bandRect.Top, ControlUtils.ColumnResizeEdgeSize, bandRect.Height);
								else return new Rectangle(bandRect.Right - ControlUtils.ColumnResizeEdgeSize, bandRect.Top, ControlUtils.ColumnResizeEdgeSize * 2, bandRect.Height);
							}
						}
					}
					return Rectangle.Empty;
			}
			return GetGridViewElementRectangle(gridControl, bandedView, rowHandle, columnName, elementType);
		}
		protected Rectangle GetCardViewElementRectangle(GridControl gridControl, CardView cardView, int rowHandle, string columnName, GridControlElements elementType) {
			CardViewInfo viewInfo = cardView.ViewInfo;
			switch(elementType) {
				case GridControlElements.MasterTabPageHeader:
					return GetMasterTabPageHeaderRectangle(cardView.TabControl, columnName);
			}
			return Rectangle.Empty;
		}
		protected GridFooterCellInfoArgs GetFooterCell(GridView gridView, GridControlElements elementType, int rowHandle, string columnName) {
			GridColumn gridColumn = null;
			if(elementType == GridControlElements.FooterCell) {
				if(gridView.ViewInfo.FooterInfo != null) {
					gridColumn = GetColumnFromName(gridView, columnName);
					if(gridColumn != null) {
						int count = 0;
						foreach(GridFooterCellInfoArgs footerCell in gridView.ViewInfo.FooterInfo.Cells)
							if(footerCell.Column == gridColumn)
								if(count == rowHandle)
									return footerCell;
								else count++;
					}
				}
			}
			else if(elementType == GridControlElements.RowFooterCell) {
				int childRowHandle = gridView.GetChildRowHandle(rowHandle, gridView.GetChildRowCount(rowHandle) - 1);
				GridRowInfo rowInfo = gridView.ViewInfo.GetGridRowInfo(childRowHandle);
				if(rowInfo == null)
					rowInfo = gridView.ViewInfo.GetGridRowInfo(rowHandle);
				if(rowInfo != null) {
					int rowFooterIndex = 0;
					if(columnName.Contains(",")) {
						if(Int32.TryParse(columnName.Substring(columnName.LastIndexOf(",") + 1), out rowFooterIndex)) {
							gridColumn = GetColumnFromName(gridView, columnName.Substring(0, columnName.LastIndexOf(",")));
						}
					}
					if(rowInfo.RowFooters.Count > rowFooterIndex) {
						if(gridColumn == null)
							gridColumn = GetColumnFromName(gridView, columnName);
						if(gridColumn != null) {
							foreach(GridFooterCellInfoArgs footerCell in rowInfo.RowFooters[rowFooterIndex].Cells)
								if(footerCell.Column == gridColumn)
									return footerCell;
						}
					}
				}
			}
			return null;
		}
		public IntPtr GetGridActiveEditorHandleOrSetActiveEditor(IntPtr windowHandle, int rowHandle, string columnName, string viewName) {
			GridControl grid = Control.FromHandle(windowHandle) as GridControl;
			if(grid == null)
				return IntPtr.Zero;
			GridView gridView = GetViewFromName(grid, viewName) as GridView;
			if(gridView == null)
				return IntPtr.Zero;
			GridColumn gridColumn = GetColumnFromName(gridView, columnName);
			if(gridColumn == null)
				return IntPtr.Zero;
			if(gridView.FocusedRowHandle != rowHandle || gridView.FocusedColumn != gridColumn || gridView.ActiveEditor == null)
				grid.BeginInvoke(new MethodInvoker(delegate() {
					ShowGridActiveEditor(gridView, rowHandle, gridColumn);
				}));
			else
				return gridView.ActiveEditor.Handle;
			return IntPtr.Zero;
		}
		public IntPtr GetGridActiveEditorHandle(IntPtr windowHandle, string viewName) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			if(gridControl != null) {
				GridView gridView = GetViewFromName(gridControl, viewName) as GridView;
				if(gridView != null) {
					if(gridView.ActiveEditor != null)
						return gridView.ActiveEditor.Handle;
				}
			}
			return IntPtr.Zero;
		}
		public void SetGridActiveEditorValue(IntPtr gridHandle, int rowHandle, string columnName, string viewName, ValueStruct value) {
			GridControl grid = Control.FromHandle(gridHandle) as GridControl;
			if(grid == null)
				return;
			GridView gridView = GetViewFromName(grid, viewName) as GridView;
			if(gridView == null)
				return;
			GridColumn gridColumn = GetColumnFromName(gridView, columnName);
			if(gridColumn == null)
				return;
			grid.BeginInvoke(new MethodInvoker(delegate() {
				if(gridView.FocusedRowHandle != rowHandle || gridView.FocusedColumn != gridColumn || gridView.ActiveEditor == null)
					ShowGridActiveEditor(gridView, rowHandle, gridColumn);
				if(gridView.ActiveEditor != null) {
					gridView.ActiveEditor.Focus();
					gridView.ActiveEditor.EditValue = CodedUIUtils.GetValue(value);
					gridView.PostEditor();
				}
			}));
		}
		public void ShowGridActiveEditor(GridView gridView, int rowHandle, GridColumn gridColumn) {
			if(gridView.FocusedRowHandle != rowHandle || gridView.FocusedColumn != gridColumn) {
				gridView.CloseEditor();
				gridView.MakeRowVisible(rowHandle);
				gridView.MakeColumnVisible(gridColumn);
				gridView.FocusedColumn = gridColumn;
				gridView.FocusedRowHandle = rowHandle;
			}
			gridView.ShowEditor();
		}
		public bool GetGridViewColumnPosition(IntPtr windowHandle, string columnName, string viewName, bool getValueFromFields, out int visibleIndex, out int groupIndex) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			GridView gridView = GetViewFromName(gridControl, viewName) as GridView;
			if(gridView != null) {
				GridColumn gridColumn = GetColumnFromName(gridView, columnName);
				if(gridColumn != null) {
					if(getValueFromFields) visibleIndex = gridColumn.VisibleIndex; 
					else visibleIndex = gridColumn.VisibleIndex;
					groupIndex = gridColumn.GroupIndex;
					return true;
				}
			}
			visibleIndex = Int32.MinValue;
			groupIndex = Int32.MinValue;
			return false;
		}
		public void SetGridViewColumnPosition(IntPtr windowHandle, string columnName, string viewName, int visibleIndex, int groupIndex) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			GridView gridView = GetViewFromName(gridControl, viewName) as GridView;
			if(gridView != null) {
				GridColumn gridColumn = GetColumnFromName(gridView, columnName);
				if(gridColumn != null)
					gridControl.BeginInvoke(new MethodInvoker(delegate() {
						SetColumnVisibleIndex(gridColumn, -1, visibleIndex);
						SetColumnGroupIndex(gridColumn, groupIndex);
					})
						);
			}
		}
		public bool GetAdvBandedGridViewColumnPosition(IntPtr windowHandle, string columnName, string viewName, out string bandName, out int rowIndex, out int visibleIndex, out int groupIndex, bool getRealVisibleIndex) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			AdvBandedGridView advBandedGridView = GetViewFromName(gridControl, viewName) as AdvBandedGridView;
			if(advBandedGridView != null) {
				BandedGridColumn gridColumn = GetColumnFromName(advBandedGridView, columnName) as BandedGridColumn;
				if(gridColumn != null) {
					if(gridColumn.Visible) {
						if(!getRealVisibleIndex && IsColumnAloneInRow(gridColumn))
							visibleIndex = -1;
						else visibleIndex = (gridColumn as BandedGridColumn).ColVIndex;
						rowIndex = (gridColumn as BandedGridColumn).RowIndex;
					}
					else {
						visibleIndex = -1;
						rowIndex = -1;
					}
					groupIndex = (gridColumn as BandedGridColumn).GroupIndex;
					bandName = GetBandName(gridColumn.OwnerBand);
					return true;
				}
			}
			visibleIndex = Int32.MinValue;
			groupIndex = Int32.MinValue;
			rowIndex = Int32.MinValue;
			bandName = null;
			return false;
		}
		public void SetAdvBandedGridViewColumnPosition(IntPtr windowHandle, string viewName, string columnName, string bandName, int rowIndex, int visibleIndex, int groupIndex) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			AdvBandedGridView advBandedGridView = GetViewFromName(gridControl, viewName) as AdvBandedGridView;
			if(advBandedGridView != null) {
				BandedGridColumn gridColumn = GetColumnFromName(advBandedGridView, columnName) as BandedGridColumn;
				if(gridColumn != null)
					gridControl.BeginInvoke(new MethodInvoker(delegate() {
						SetColumnBand(gridColumn, bandName);
						SetColumnVisibleIndex(gridColumn, rowIndex, visibleIndex);
						SetColumnGroupIndex(gridColumn, groupIndex);
					}));
			}
		}
		protected void SetColumnBand(BandedGridColumn gridColumn, string bandName) {
			string currentBandName = GetBandName(gridColumn.OwnerBand);
			if(currentBandName != bandName) {
				GridBand targetBand = GetBandFromName(gridColumn.View, bandName);
				gridColumn.OwnerBand = targetBand;
			}
		}
		protected void SetColumnVisibleIndex(GridColumn gridColumn, int rowIndex, int visibleIndex) {
			if(visibleIndex != Int32.MinValue) {
				if(rowIndex >= 0) {
					if(gridColumn.View is AdvBandedGridView) {
						(gridColumn.View as AdvBandedGridView).SetColumnPosition(gridColumn as BandedGridColumn, rowIndex, visibleIndex);
						if((gridColumn as BandedGridColumn).ColVIndex != visibleIndex && visibleIndex > 0)
							(gridColumn.View as AdvBandedGridView).SetColumnPosition(gridColumn as BandedGridColumn, rowIndex, visibleIndex + 1);
					}
				}
				else if(visibleIndex == -1)
					gridColumn.Visible = false;
				else {
					gridColumn.VisibleIndex = visibleIndex;
					if(gridColumn.VisibleIndex != visibleIndex && visibleIndex > 0)
						gridColumn.VisibleIndex = visibleIndex + 1;
				}
			}
		}
		protected void SetColumnGroupIndex(GridColumn gridColumn, int groupIndex) {
			if(groupIndex != Int32.MinValue) {
				if(groupIndex >= 0) {
					Dictionary<int, int> savedColumnsGroupIndices = new Dictionary<int, int>();
					for(int i = groupIndex; i < gridColumn.View.GroupedColumns.Count; i++) {
						if(gridColumn.View.GroupedColumns[i] != gridColumn)
							savedColumnsGroupIndices.Add(gridColumn.View.GroupedColumns[i].AbsoluteIndex, gridColumn.View.GroupedColumns[i].GroupIndex);
					}
					gridColumn.GroupIndex = groupIndex;
					foreach(KeyValuePair<int, int> savedColumn in savedColumnsGroupIndices)
						gridColumn.View.Columns[savedColumn.Key].GroupIndex = savedColumn.Value + 1;
				}
				else gridColumn.GroupIndex = groupIndex;
			}
		}
		protected bool IsColumnAloneInRow(BandedGridColumn gridColumn) {
			foreach(BandedGridColumn column in (gridColumn.View as AdvBandedGridView).Columns) {
				if(gridColumn != column && column.Visible && column.RowIndex == gridColumn.RowIndex && column.OwnerBand == gridColumn.OwnerBand)
					return false;
			}
			return true;
		}
		public int GetColumnWidth(IntPtr windowHandle, string columnName, string viewName) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			ColumnView columnView = GetViewFromName(gridControl, viewName) as ColumnView;
			if(columnView != null) {
				GridColumn gridColumn = GetColumnFromName(columnView, columnName);
				if(gridColumn != null)
					return gridColumn.Width;
			}
			return -1;
		}
		public void SetColumnWidth(IntPtr windowHandle, string columnName, string viewName, int columnWidth) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			ColumnView columnView = GetViewFromName(gridControl, viewName) as ColumnView;
			if(columnView != null) {
				GridColumn gridColumn = GetColumnFromName(columnView, columnName);
				if(gridColumn != null)
					gridControl.BeginInvoke(new MethodInvoker(delegate() { gridColumn.Width = columnWidth; }));
			}
		}
		public string GetColumnFilterString(IntPtr windowHandle, string columnName, string viewName) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			ColumnView columnView = GetViewFromName(gridControl, viewName) as ColumnView;
			if(columnView != null) {
				GridColumn gridColumn = GetColumnFromName(columnView, columnName);
				if(gridColumn != null)
					return gridColumn.FilterInfo.FilterString;
			}
			return null;
		}
		public void OpenCustomFilterWindow(IntPtr windowHandle, string columnName, string viewName) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			ColumnView columnView = GetViewFromName(gridControl, viewName) as ColumnView;
			if(columnView != null) {
				GridColumn gridColumn = GetColumnFromName(columnView, columnName);
				if(gridColumn != null) {
					MethodInfo applyColumnFilter = columnView.GetType().GetMethod("ApplyColumnFilter", BindingFlags.NonPublic | BindingFlags.Instance);
					if(applyColumnFilter != null) {
						object[] parameters = new object[2];
						parameters[0] = gridColumn;
						parameters[1] = new FilterItem("custom", new FilterItem("custom", 1));
						gridControl.BeginInvoke(new MethodInvoker(delegate() { applyColumnFilter.Invoke(columnView, parameters); }));
					}
				}
			}
		}
		public string GetActiveFilterString(IntPtr windowHandle, string viewName) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			ColumnView columnView = GetViewFromName(gridControl, viewName) as ColumnView;
			if(columnView != null) {
				return columnView.ActiveFilterString;
			}
			return null;
		}
		public void SetActiveFilterString(IntPtr windowHandle, string viewName, string filterString) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			ColumnView columnView = GetViewFromName(gridControl, viewName) as ColumnView;
			if(columnView != null)
				gridControl.BeginInvoke(new MethodInvoker(delegate() { columnView.ActiveFilterString = filterString; }));
		}
		public bool GetGridFocus(IntPtr windowHandle, string viewName, out int rowHandle, out string columnName) {
			rowHandle = Int32.MinValue;
			columnName = null;
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			GridView gridView = GetViewFromName(gridControl, viewName) as GridView;
			if(gridView != null) {
				columnName = GetColumnName(gridView.FocusedColumn);
				rowHandle = gridView.FocusedRowHandle;
				return true;
			}
			return false;
		}
		public void SetGridFocus(IntPtr windowHandle, int rowHandle, string columnName, string viewName) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			BaseView baseView = GetViewFromName(gridControl, viewName);
			if(baseView is GridView) {
				GridView gridView = baseView as GridView;
				GridColumn gridColumn = GetColumnFromName(gridView, columnName);
				gridControl.BeginInvoke(new MethodInvoker(
		delegate() {
			gridView.FocusedRowHandle = rowHandle;
			gridView.FocusedColumn = gridColumn;
			if(gridView.IsRowVisible(rowHandle) != RowVisibleState.Visible)
				gridView.MakeRowVisible(rowHandle);
			if(gridColumn != null && gridColumn.Visible == false)
				gridView.MakeColumnVisible(gridColumn);
		})
	);
			}
		}
		public void MakeGridElementVisible(IntPtr windowHandle, GridControlElements elementType, int rowHandle, string columnName, string viewName) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			BaseView baseView = GetViewFromName(gridControl, viewName);
			if(baseView is GridView) {
				GridView gridView = baseView as GridView;
				GridColumn gridColumn = GetColumnFromName(gridView, columnName);
				switch(elementType) {
					case GridControlElements.ColumnHeader:
						this.MakeColumnVisible(gridControl, gridView, gridColumn);
						break;
					case GridControlElements.Cell:
					case GridControlElements.CellButton:
						this.MakeRowVisible(gridControl, gridView, rowHandle);
						this.MakeColumnVisible(gridControl, gridView, gridColumn);
						break;
					case GridControlElements.FooterCell:
						this.MakeColumnVisible(gridControl, gridView, gridColumn);
						break;
					case GridControlElements.GroupRow:
					case GridControlElements.Row:
					case GridControlElements.RowGroupButton:
					case GridControlElements.RowIndicator:
						this.MakeRowVisible(gridControl, gridView, rowHandle);
						break;
					case GridControlElements.RowFooter:
					case GridControlElements.RowFooterCell:
						int childRowHandle = gridView.GetChildRowHandle(rowHandle, gridView.GetChildRowCount(rowHandle) - 1);
						this.MakeRowVisible(gridControl, gridView, childRowHandle);
						break;
				}
			}
		}
		protected void MakeRowVisible(GridControl gridControl, GridView gridView, int rowHandle) {
			gridControl.BeginInvoke(new MethodInvoker(
delegate() {
	if(gridView.IsRowVisible(rowHandle) != RowVisibleState.Visible)
		gridView.MakeRowVisible(rowHandle);
})
);
		}
		protected void MakeColumnVisible(GridControl gridControl, GridView gridView, GridColumn gridColumn) {
			gridControl.BeginInvoke(new MethodInvoker(
delegate() {
	if(gridColumn != null && gridColumn.Visible == false)
		gridView.MakeColumnVisible(gridColumn);
})
);
		}
		public ValueStruct GetGridElementValue(IntPtr windowHandle, int rowHandle, string columnName, string viewName, GridControlElements elementType) {
			ValueStruct result = new ValueStruct();
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			if(gridControl == null)
				return result;
			BaseView baseView = GetViewFromName(gridControl, viewName);
			if(elementType == GridControlElements.MasterTabPageHeader) {
				int index;
				if(Int32.TryParse(columnName, out index))
					if(index >= 0 && baseView.TabControl != null && baseView.TabControl.Pages != null && index < baseView.TabControl.Pages.Count) {
						result = new ValueStruct(baseView.TabControl.SelectedPage == baseView.TabControl.Pages[index]);
						result.DisplayText = baseView.TabControl.Pages[index].Text;
						result.WasValueRead = true;
					}
				return result;
			}
			if(baseView is GridView) {
				GridView gridView = baseView as GridView;
				GridColumn gridColumn = null;
				switch(elementType) {
					case GridControlElements.Cell:
					case GridControlElements.CellButton:
						gridColumn = GetColumnFromName(gridView, columnName);
						if(gridColumn != null) {
							result = new ValueStruct(gridView.GetRowCellValue(rowHandle, gridColumn));
							result.DisplayText = gridView.GetRowCellDisplayText(rowHandle, gridColumn);
							GridCellInfo cellInfo = gridView.ViewInfo.GetGridCellInfo(rowHandle, gridColumn);
							if(cellInfo != null && cellInfo.ViewInfo != null && cellInfo.ViewInfo.ShowErrorIcon)
								result.ErrorText = cellInfo.ViewInfo.ErrorIconText;
						}
						return result;
					case GridControlElements.GroupRow:
						GridGroupRowInfo groupRowInfo = gridView.ViewInfo.GetGridRowInfo(rowHandle) as GridGroupRowInfo;
						if(groupRowInfo != null) {
							result = new ValueStruct(groupRowInfo.EditValue);
							result.DisplayText = groupRowInfo.GroupText;
						}
						return result;
					case GridControlElements.RowPreview:
						GridDataRowInfo dataRowInfo = gridView.ViewInfo.RowsInfo.GetInfoByHandle(rowHandle) as GridDataRowInfo;
						if(dataRowInfo != null) {
							result.DisplayText = dataRowInfo.PreviewText;
							result.WasValueRead = true;
						}
						return result;
					case GridControlElements.RowFooterCell:
					case GridControlElements.FooterCell:
						GridFooterCellInfoArgs footerCell = GetFooterCell(gridView, elementType, rowHandle, columnName);
						if(footerCell != null) {
							result = new ValueStruct(footerCell.Value);
							result.DisplayText = footerCell.DisplayText;
						}
						return result;
					case GridControlElements.ColumnHeader:
						gridColumn = GetColumnFromName(gridView, columnName);
						if(gridColumn != null) {
							result.DisplayText = gridColumn.Caption;
							result.WasValueRead = true;
						}
						return result;
				}
			}
			return result;
		}
		public AppearanceObjectSerializable GetGridElementAppearance(IntPtr windowHandle, GridElementInfo elementInfo) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			BaseView baseView = GetViewFromName(gridControl, elementInfo.ViewName);
			if(baseView is GridView) {
				GridView gridView = baseView as GridView;
				GridColumn gridColumn = null;
				switch(elementInfo.ElementType) {
					case GridControlElements.Cell:
						gridColumn = GetColumnFromName(gridView, elementInfo.ColumnName);
						if(gridColumn != null) {
							GridCellInfo cell = gridView.ViewInfo.GetGridCellInfo(elementInfo.RowHandle, gridColumn);
							if(cell != null)
								return new AppearanceObjectSerializable(cell.Appearance);
						}
						break;
				}
			}
			return null;
		}
		public void SetCellValue(IntPtr windowHandle, int rowHandle, string columnName, string viewName, string cellValue, string cellValueType) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			BaseView baseView = GetViewFromName(gridControl, viewName);
			if(baseView is GridView) {
				GridView gridView = baseView as GridView;
				GridColumn gridColumn = GetColumnFromName(gridView, columnName);
				if(gridColumn != null) {
					object newValue = cellValue;
					if(cellValueType != null) {
						newValue = CodedUIUtils.ConvertFromString(cellValue, cellValueType);
					}
					gridControl.BeginInvoke(new MethodInvoker(delegate() {
						gridView.SetRowCellValue(rowHandle, gridColumn, newValue);
					}));
				}
			}
		}
		public int GetBandWidth(IntPtr windowHandle, string viewName, string bandName) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			BandedGridView bandedView = GetViewFromName(gridControl, viewName) as BandedGridView;
			if(bandedView != null) {
				GridBand gridBand = GetBandFromName(bandedView, bandName);
				if(gridBand != null)
					return gridBand.Width;
			}
			return -1;
		}
		public void SetBandWidth(IntPtr windowHandle, string viewName, string bandName, int bandWidth) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			BandedGridView bandedView = GetViewFromName(gridControl, viewName) as BandedGridView;
			if(bandedView != null) {
				GridBand gridBand = GetBandFromName(bandedView, bandName);
				if(gridBand != null)
					gridControl.BeginInvoke(new MethodInvoker(delegate() { gridBand.Width = bandWidth; }));
			}
		}
		public bool GetBandPosition(IntPtr windowHandle, string viewName, string bandName, out string parentBand, out int visibleIndex) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			BandedGridView bandedView = GetViewFromName(gridControl, viewName) as BandedGridView;
			if(bandedView != null) {
				GridBand gridBand = GetBandFromName(bandedView, bandName);
				if(gridBand != null) {
					parentBand = GetBandName(gridBand.ParentBand);
					visibleIndex = gridBand.VisibleIndex;
					return true;
				}
			}
			parentBand = null;
			visibleIndex = Int32.MinValue;
			return false;
		}
		public void SetBandPosition(IntPtr windowHandle, string viewName, string bandName, string parentBandName, int visibleIndex) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			BandedGridView bandedView = GetViewFromName(gridControl, viewName) as BandedGridView;
			if(bandedView != null) {
				GridBand gridBand = GetBandFromName(bandedView, bandName);
				if(gridBand != null)
					gridControl.BeginInvoke(new MethodInvoker(delegate() {
						SetBandParent(bandedView, gridBand, parentBandName);
						SetBandVisibleIndex(bandedView, gridBand, visibleIndex);
					}));
			}
		}
		protected void SetBandVisibleIndex(BandedGridView view, GridBand gridBand, int visibleIndex) {
			if(visibleIndex != Int32.MinValue) {
				if(visibleIndex < 0)
					gridBand.Visible = false;
				else {
					bool wasSet = false;
					foreach(GridBand band in gridBand.Collection) {
						if(band.VisibleIndex == visibleIndex) {
							if(gridBand.VisibleIndex < band.VisibleIndex && gridBand.VisibleIndex >= 0)
								gridBand.Collection.MoveTo(band.Index + 1, gridBand);
							else gridBand.Collection.MoveTo(band.Index, gridBand);
							wasSet = true;
							break;
						}
					}
					if(!wasSet)
						gridBand.Collection.MoveTo(gridBand.Collection.Count, gridBand);
					gridBand.Visible = true;
				}
			}
		}
		protected void SetBandParent(BandedGridView view, GridBand gridBand, string parentBandName) {
			GridBand parentBand = GetBandFromName(view, parentBandName);
			if(parentBand != gridBand.ParentBand)
				gridBand.SetParentBandCore(parentBand);
		}
		public int GetCustomizationListBoxItemIndex(IntPtr windowHandle, string innerElementName) {
			CustomCustomizationListBox listBox = Control.FromHandle(windowHandle) as CustomCustomizationListBox;
			object innerElement = null;
			if(listBox is ColumnCustomizationListBox) {
				GridView gridView = listBox.View;
				if(gridView != null) {
					innerElement = GetColumnFromName(gridView, innerElementName);
				}
			}
			else if(listBox is BandCustomizationListBox) {
				BandedGridView bandedGridView = (listBox as BandCustomizationListBox).View;
				if(bandedGridView != null) {
					innerElement = GetBandFromName(bandedGridView, innerElementName);
				}
			}
			if(innerElement != null) {
				for(int i = 0; i < listBox.Items.Count; i++)
					if(listBox.Items[i] == innerElement)
						return i;
			}
			return -1;
		}
		public bool GetInnerElementInformationForCustomizationListBoxItem(IntPtr listBoxHandle, int itemIndex, out IntPtr gridHandle, out string viewName, out GridControlElements innerElementType, out string innerElementName) {
			gridHandle = IntPtr.Zero;
			viewName = innerElementName = null;
			innerElementType = GridControlElements.Unknown;
			CustomCustomizationListBox customListBox = Control.FromHandle(listBoxHandle) as CustomCustomizationListBox;
			if(itemIndex < customListBox.Items.Count) {
				if(customListBox is ColumnCustomizationListBox) {
					GridColumn gridColumn = customListBox.Items[itemIndex] as GridColumn;
					if(gridColumn != null) {
						if(gridColumn.View is AdvBandedGridView)
							innerElementType = GridControlElements.BandedColumnHeader;
						else innerElementType = GridControlElements.ColumnHeader;
						innerElementName = GetColumnName(gridColumn);
						viewName = GetViewName(gridColumn.View);
						gridHandle = gridColumn.View.GridControl.Handle;
						return true;
					}
				}
				else if(customListBox is BandCustomizationListBox) {
					GridBand gridBand = customListBox.Items[itemIndex] as GridBand;
					if(gridBand != null) {
						innerElementType = GridControlElements.Band;
						innerElementName = GetBandName(gridBand);
						viewName = GetViewName(gridBand.View);
						gridHandle = gridBand.View.GridControl.Handle;
						return true;
					}
				}
			}
			return false;
		}
		public string GetCustomizationFormViewName(IntPtr windowHandle) {
			CustomizationForm custForm = Control.FromHandle(windowHandle) as CustomizationForm;
			if(custForm == null) return null;
			GridControl gridControl = custForm.GridControl;
			if(gridControl != null)
				return GetViewName(custForm.View);
			else return null;
		}
		public IntPtr GetCustomizationFormHandleOrShowIt(IntPtr windowHandle, string viewName) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			GridView gridView = GetViewFromName(gridControl, viewName) as GridView;
			if(gridView != null) {
				if(gridView.CustomizationForm == null) {
					gridControl.BeginInvoke(new MethodInvoker(delegate() { gridView.ShowCustomization(); }));
					return IntPtr.Zero;
				}
				else return gridView.CustomizationForm.Handle;
			}
			else return IntPtr.Zero;
		}
		protected Type GetTypeFromString(string stringType) {
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach(Assembly assembly in assemblies) {
				Type typeFromAssembly = assembly.GetType(stringType);
				if(typeFromAssembly != null) {
					return typeFromAssembly;
				}
			}
			return typeof(object);
		}
		protected static class StringConstants {
			public const string ColumnHandle = "ColumnHandle";
			public const string BandIndex = "BandIndex";
		}
		public string GetColumnName(IntPtr windowHandle, int absoluteIndex, string viewName) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			ColumnView columnView = GetViewFromName(gridControl, viewName) as ColumnView;
			if(columnView != null)
				foreach(GridColumn column in columnView.Columns)
					if(column.AbsoluteIndex == absoluteIndex)
						return GetColumnName(column);
			return null;
		}
		public string GetColumnNameFromGroupRow(IntPtr windowHandle, int rowHandle, string viewName) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			GridView gridView = GetViewFromName(gridControl, viewName) as GridView;
			if(gridView != null) {
				GridGroupRowInfo groupRowInfo = gridView.ViewInfo.GetGridRowInfo(rowHandle) as GridGroupRowInfo;
				if(groupRowInfo != null)
					if(groupRowInfo.Column != null)
						return GetColumnName(groupRowInfo.Column);
			}
			return null;
		}
		public string GetFocusedViewName(IntPtr windowHandle) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			if(gridControl == null) return null;
			return GetViewName(gridControl.FocusedView);
		}
		protected string GetColumnName(GridColumn column) {
			if(column == null) return null;
			if(column.Name != String.Empty)
				return column.Name;
			int count = 0;
			foreach(GridColumn gridColumn in column.View.Columns) {
				if(gridColumn.ColumnHandle == column.ColumnHandle)
					count++;
				if(gridColumn == column) {
					string name = StringConstants.ColumnHandle + column.ColumnHandle;
					if(count != 1) name += "-" + count;
					return name;
				}
			}
			return null;
		}
		public int GetColumnAbsoluteIndexFromName(IntPtr windowHandle, string viewName, string columnName) {
			GridControl grid = Control.FromHandle(windowHandle) as GridControl;
			if(grid != null) {
				ColumnView view = GetViewFromName(grid, viewName) as ColumnView;
				if(view != null) {
					GridColumn column = GetColumnFromName(view, columnName);
					if(column != null)
						return column.AbsoluteIndex;
				}
			}
			return -1;
		}
		protected GridColumn GetColumnFromName(ColumnView columnView, string columnName) {
			if(columnName == null)
				return null;
			if(columnName == GridView.CheckBoxSelectorColumnName && columnView is GridView)
				return (columnView as GridView).CheckboxSelectorColumn;
			foreach(GridColumn column in columnView.Columns)
				if(column.Name == columnName)
					return column;
			if(columnName.StartsWith(StringConstants.ColumnHandle)) {
				int columnHandle;
				int number = 1;
				try {
					if(columnName.Contains("-")) {
						columnHandle = Int32.Parse(columnName.Substring(StringConstants.ColumnHandle.Length, columnName.LastIndexOf("-") - StringConstants.ColumnHandle.Length));
						number = Int32.Parse(columnName.Substring(columnName.LastIndexOf("-") + 1));
					}
					else columnHandle = Int32.Parse(columnName.Substring(StringConstants.ColumnHandle.Length));
					int count = 0;
					foreach(GridColumn column in columnView.Columns)
						if(column.ColumnHandle == columnHandle) {
							count++;
							if(count == number)
								return column;
						}
				}
				catch { }
			}
			return null;
		}
		protected string GetBandName(GridBand band) {
			if(band == null) return null;
			return band.Name;
		}
		protected GridBand GetBandFromName(BandedGridView bandedView, string bandName) {
			if(bandName == null || bandName == String.Empty) return null;
			GridBand band = GetBandFromName(bandedView.Bands, bandName);
			return band;
		}
		protected GridBand GetBandFromName(GridBandCollection bandCollection, string bandName) {
			foreach(GridBand band in bandCollection) {
				if(band.Name == bandName)
					return band;
				else if(band.Children.Count > 0) {
					GridBand gridBand = GetBandFromName(band.Children, bandName);
					if(gridBand != null)
						return gridBand;
				}
			}
			return null;
		}
		protected GridBandInfoArgs GetBandInfo(BandedGridViewInfo viewInfo, GridBand gridBand) {
			List<GridBand> bands = new List<GridBand>();
			GridBand band = gridBand;
			while(true)
				if(band.ParentBand != null) {
					bands.Add(band.ParentBand);
					band = band.ParentBand;
				}
				else break;
			GridBandInfoCollection infoCollection = viewInfo.BandsInfo;
			for(int i = bands.Count - 1; i >= 0; i--)
				infoCollection = infoCollection[bands[i]].Children;
			return infoCollection[gridBand];
		}
		public string[] GetViewsNames(IntPtr windowHandle) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			if(gridControl == null) return null;
			string[] viewsNames = new string[gridControl.Views.Count];
			for(int i = 0; i < viewsNames.Length; i++)
				viewsNames[i] = GetViewName(gridControl.Views[i]);
			return viewsNames;
		}
		protected string GetViewName(BaseView view) {
			if(view != null) {
				int count = 0;
				foreach(BaseView baseView in view.GridControl.Views)
					if(baseView.Name == view.Name) {
						count++;
						if(baseView == view)
							if(count == 1)
								return view.Name;
							else return view.Name + "-" + count;
					}
			}
			return null;
		}
		protected BaseView GetViewFromName(GridControl gridControl, string viewName) {
			try {
				int number = 1;
				if(viewName.Contains("-")) {
					number = Int32.Parse(viewName.Substring(viewName.LastIndexOf("-") + 1));
					viewName = viewName.Substring(0, viewName.LastIndexOf("-"));
				}
				int count = 0;
				foreach(BaseView view in gridControl.Views)
					if(view.Name == viewName) {
						count++;
						if(count == number)
							return view;
					}
			}
			catch { }
			return null;
		}
		public GridControlViews GetViewTypeFromName(IntPtr windowHandle, string viewName) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			if(gridControl != null) {
				BaseView view = GetViewFromName(gridControl, viewName);
				if(view is AdvBandedGridView)
					return GridControlViews.AdvBandedGridView;
				else if(view is BandedGridView)
					return GridControlViews.BandedGridView;
				else if(view is GridView)
					return GridControlViews.GridView;
				else if(view is CardView)
					return GridControlViews.CardView;
				else if(IsLayoutView(view))
					return GridControlViews.LayoutView;
			}
			return GridControlViews.Undefined;
		}
		protected bool IsLayoutView(BaseView view) {
			if(view is LayoutView)
				return true;
			return false;
		}
		public void SetLookUpEditBaseValue(IntPtr windowHandle, ValueStruct value, bool byDisplayMember) {
			LookUpEditBase lookUpBase = Control.FromHandle(windowHandle) as LookUpEditBase;
			DevExpress.XtraEditors.CodedUISupport.XtraEditorsCodedUIHelper xtraEditorsHelper = new DevExpress.XtraEditors.CodedUISupport.XtraEditorsCodedUIHelper(this.remoteObject);
			if(lookUpBase is GridLookUpEditBase)
				xtraEditorsHelper.SetLookUpEditBaseValue(lookUpBase, value,
					delegate(string fieldName, int index) {
						return (lookUpBase as GridLookUpEditBase).Properties.Controller.GetValueEx(index, fieldName);
					},
				byDisplayMember);
			else if(lookUpBase is LookUpEdit)
				xtraEditorsHelper.SetLookUpEditValue(lookUpBase as LookUpEdit, value, byDisplayMember);
			else
				xtraEditorsHelper.SetLookUpEditBaseValue(lookUpBase, value, null, byDisplayMember);
		}
		public int GetSetLookUpEditSelectedIndex(IntPtr windowHandle, int newValue, bool isSet) {
			LookUpEditBase lookUpBase = Control.FromHandle(windowHandle) as LookUpEditBase;
			if(lookUpBase != null) {
				if(isSet)
					lookUpBase.BeginInvoke(new MethodInvoker(delegate() {
						if(lookUpBase is LookUpEdit)
							(lookUpBase as LookUpEdit).ItemIndex = newValue;
						else if(lookUpBase is GridLookUpEditBase)
							lookUpBase.EditValue = (lookUpBase as GridLookUpEditBase).Properties.Controller.GetValueEx(newValue, lookUpBase.Properties.ValueMember);
					}));
				else
					if(lookUpBase is LookUpEdit)
						return (lookUpBase as LookUpEdit).ItemIndex;
					else if(lookUpBase is GridLookUpEdit)
						return (lookUpBase as GridLookUpEdit).Properties.GetIndexByKeyValue(lookUpBase.EditValue);
					else if(lookUpBase is SearchLookUpEdit)
						return (lookUpBase as SearchLookUpEdit).Properties.GetIndexByKeyValue(lookUpBase.EditValue);
			}
			return -1;
		}
		public int[] GetCellsRowHandlesByValue(IntPtr windowHandle, string columnName, string viewName, ValueStruct searchParams, bool findByText, bool findFirst) {
			GridControl gridControl = Control.FromHandle(windowHandle) as GridControl;
			List<int> result = new List<int>();
			if(gridControl != null) {
				BaseView baseView = GetViewFromName(gridControl, viewName);
				if(baseView is GridView) {
					GridView gridView = baseView as GridView;
					GridColumn gridColumn = GetColumnFromName(gridView, columnName);
					if(gridColumn != null) {
						if(findByText) {
							for(int i = 0; i < gridView.RowCount; i++) {
								if(gridView.GetRowCellDisplayText(i, gridColumn) == searchParams.DisplayText) {
									result.Add(i);
									if(findFirst)
										return result.ToArray();
								}
							}
						}
						else {
							for(int i = 0; i < gridView.RowCount; i++) {
								object value = gridView.GetRowCellValue(i, gridColumn);
								if(value == null) {
									if(searchParams.ValueAsString == null)
										result.Add(i);
									else continue;
								}
								else {
									if(CodedUIUtils.ConvertToString(value) == searchParams.ValueAsString && value.GetType().FullName == searchParams.ValueTypeName)
										result.Add(i);
									else continue;
								}
								if(findFirst)
									if(result.Count > 0)
										return result.ToArray();
							}
						}
					}
				}
			}
			return result.ToArray();
		}
		public string GetCheckboxSelectorColumnName(IntPtr windowHandle) {
			return GridView.CheckBoxSelectorColumnName;
		}
	}
}
