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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Dragging;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.Drawing;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
namespace DevExpress.XtraGrid.Dragging {
	public class AdvBandedColumnPositionInfo : BandedColumnPositionInfo {
		protected int fRowIndex, fColIndex;
		public AdvBandedColumnPositionInfo() {
			this.fRowIndex = this.fColIndex = 0;
		}
		public override bool IsEquals(PositionInfo pi) {
			AdvBandedColumnPositionInfo cpi = pi as AdvBandedColumnPositionInfo;
			bool res = base.IsEquals(pi);
			if(!res || cpi == null) return false;
			return this.RowIndex == cpi.RowIndex && this.ColIndex == cpi.ColIndex;
		}
		public int RowIndex { get { return fRowIndex; } }
		public int ColIndex { get { return fColIndex; } }
		protected internal void SetRowColIndex(int row, int col) { 
			this.fRowIndex = row;
			this.fColIndex = col;
		}
	}
	public class AdvBandedGridDragManager : BandedGridDragManager {
		public AdvBandedGridDragManager(AdvBandedGridView view) : base(view) {
		}
		public new AdvBandedGridView View { get { return base.View as AdvBandedGridView; } }
		public new AdvBandedGridViewInfo ViewInfo { get { return base.ViewInfo as AdvBandedGridViewInfo; } }
		protected override ColumnPositionInfo CreateColumnPositionInfo() {
			return new AdvBandedColumnPositionInfo();
		}
		protected virtual GridColumnsInfo GetRowColumns(GridBand band, int rowIndex) {
			GridColumnsInfo info = new GridColumnsInfo();
			GridBandInfoArgs bi = ViewInfo.BandsInfo.FindBand(band);
			if(bi == null) return info;
			for(int n = 0; n < bi.Columns.Count; n++) {
				GridColumnInfoArgs args = bi.Columns[n];
				if(args.Column == null) continue;
				BandedGridColumn col = args.Column as BandedGridColumn;
				if(col.RowIndex == rowIndex) {
					info.Add(args);
				}
			}
			return info;
		}
		protected virtual Rectangle CalcColumnRowRectangle(GridBand band, int rowIndex, int colIndex) {
			GridBandInfoArgs bi = ViewInfo.BandsInfo.FindBand(band);
			if(bi == null) return Rectangle.Empty;
			Rectangle r = bi.Bounds;
			r.Y = ViewInfo.ViewRects.ColumnPanel.Top;
			int rTop = -10000, rBottom = -10000, rLeft = -10000, rRight = -10000;
			int curCol = 0;
			for(int n = 0; n < bi.Columns.Count; n++) {
				GridColumnInfoArgs args = bi.Columns[n];
				if(args.Column == null) continue;
				BandedGridColumn col = args.Column as BandedGridColumn;
				if(col.RowIndex == rowIndex) {
					rTop = args.Bounds.Top;
					rBottom = Math.Max(rBottom, args.Bounds.Bottom);
					if(col.ColVIndex == colIndex) { 
						rLeft = args.Bounds.Left;
						rRight = args.Bounds.Right;
					}
					curCol ++;
				}
			}
			if(rTop != -10000) {
				r.Y = rTop;
				r.Height = rBottom - rTop;
				if(colIndex != -1) {
					if(rLeft == -10000) return Rectangle.Empty;
					r.X = rLeft;
					r.Width = rRight - rLeft;
				}
				return r;
			}
			return Rectangle.Empty;
		}
		protected virtual void UpdateColumnRect(AdvBandedColumnPositionInfo res) {
			if(res.Band == null) return;
			if(res.Index == -1) return;
			Rectangle rowRect = CalcColumnRowRectangle(res.Band, res.RowIndex, -1);
			if(res.ColIndex == -1) { 
				if(rowRect == Rectangle.Empty) {
					rowRect = CalcColumnRowRectangle(res.Band, res.RowIndex - 1, -1);
					if(rowRect.IsEmpty) {
						res.Valid = false;
						return;
					}
					res.IsHorizontal = true;
					rowRect.Y = rowRect.Bottom - ViewInfo.ColumnRowHeight / 2;
					rowRect.Height = ViewInfo.ColumnRowHeight;
					res.Bounds = rowRect;
				} else {
					res.IsHorizontal = true;
					rowRect.Height = ViewInfo.ColumnRowHeight;
					rowRect.Y -= rowRect.Height / 2;
					res.Bounds = rowRect;
				}
				return;
			}
			Rectangle r = rowRect;
			if(r.IsEmpty) {
				res.Valid = false;
				return;
			}
			r = CalcColumnRowRectangle(res.Band, res.RowIndex, res.ColIndex);
			if(r.IsEmpty) {
				r = CalcColumnRowRectangle(res.Band, res.RowIndex, res.ColIndex - 1);
				if(r.IsEmpty || res.ColIndex == 0) {
					res.Valid = false;
					return;
				}
				if(UseArrows) {
					r.X = r.Right - 1;
					r.Width = 1;
				}
				else {
					r.X = r.Right - r.Width / 2;
					r.Width = r.Width / 2;
				}
				res.Bounds = r;
			} else {
				r.Width = r.Width / 2;
				res.Bounds = r;
			}
		}
		protected override bool CanColumnHasSameIndex { get { return true; } }
		protected override PositionInfo CalcColumnDrag(GridHitInfo hit, GridColumn column) {
			if(hit.InGroupPanel) {
				return CalcInGroupPanel(hit, column);
			}
			AdvBandedColumnPositionInfo res = base.CalcColumnDrag(hit, column) as AdvBandedColumnPositionInfo,
				emptyRes = CreateColumnPositionInfo() as AdvBandedColumnPositionInfo;
			if(!View.GridControl.Visible) return emptyRes;
			if(hit.HitTest == GridHitTest.CustomizationForm || hit.HitPoint.Y > ViewInfo.ViewRects.ColumnPanel.Bottom + 50 || IsPointOutsideControl(hit.HitPoint)) {
				res = emptyRes;
				res.SetIndex(hit.HitTest == GridHitTest.CustomizationForm ? CustomizationFormPosition : HideElementPosition);
				res.Valid = column.OptionsColumn.AllowShowHide;
				if(res.Index == HideElementPosition) res.Valid = column.OptionsColumn.AllowShowHide && View.OptionsCustomization.AllowQuickHideColumns;
				return res;
			}
			if(res.Band == null || res.Band.Columns.Count == 0 || res.Band.Columns.VisibleColumnCount == 0) return res;
			BandedGridColumn col = column as BandedGridColumn;
			if(col == null || col.View != View) return res;
			Rectangle row;
			res.SetRowColIndex(res.RowIndex, 0);
			for(int n = 0;; n++) {
				row = CalcColumnRowRectangle(res.Band, n, -1);
				if(row.IsEmpty) {
					res.SetRowColIndex(n, -1);
					break;
				} else {
					if(hit.HitPoint.Y < row.Y) {
						res.SetRowColIndex(n, -1);
						break;
					}
					if(hit.HitPoint.Y < row.Bottom) {
						res.SetRowColIndex(n, res.ColIndex);
						if(hit.HitPoint.Y > (row.Bottom - row.Height / 3)) {
							res.SetRowColIndex(res.RowIndex + 1, -1);
							break;
						}
						GridColumnsInfo cols = GetRowColumns(res.Band, n);
						ColumnDropInfo drInfo = CalcColumnInfoCore(cols, hit, row);
						if(!drInfo.PositionInfo.Valid) return emptyRes;
						AdvBandedColumnPositionInfo tres = drInfo.PositionInfo as AdvBandedColumnPositionInfo;
						if(tres.Index != 9999) {
							if(drInfo.Column == null) return emptyRes;
							int index = (drInfo.Column as BandedGridColumn).ColIndex;
							res.SetRowColIndex(res.RowIndex, drInfo.Pos == DropPosition.Left ? index : index + 1);
						}
						break;
					}
				}
			}
			if(res.RowIndex == col.RowIndex && res.Band == col.OwnerBand && res.ColIndex > -1) {
				int colIndex = col.ColIndex;
				if(res.ColIndex == colIndex || res.ColIndex == colIndex + 1) {
					if(col.GroupIndex != -1 || !col.Visible) {
						GridColumnInfoArgs ci = ViewInfo.ColumnsInfo[column];
						if(ci != null) res.Bounds = ci.Bounds;
						return res;
					}
					return emptyRes;
				}
			}
			UpdateColumnRect(res);
			return res;
		}
		protected override void EndDragColumnCore(GridColumn col) {
			BandedGridColumn bCol = col as BandedGridColumn;
			AdvBandedColumnPositionInfo pInfo = LastPosition as AdvBandedColumnPositionInfo;
			if(pInfo == null || col == null || !pInfo.Valid) return;
			if(pInfo.InGroupPanel) {
				base.EndDragColumnCore(col);
				return;
			}
			if(pInfo.Index == -1) {
				col.UnGroup();
				col.Visible = true;
				return;
			}
			if(pInfo.Index <= -100) {
				col.UnGroup();
				col.Visible = false;
				return;
			}
			if(bCol == null) return;
			if(col.GroupIndex > -1 && DragStartHitInfo.InGroupColumn) col.GroupIndex = -1;
			if(bCol.OwnerBand == pInfo.Band) {
				AdvBandedGridView bv = bCol.View as AdvBandedGridView;
				if(bv == null) bv = View;
				bv.SetColumnPosition(bCol, pInfo.RowIndex, pInfo.ColIndex);
			} else {
				col.View.BeginUpdate();
				try {
					bool widthLocked = bCol.WidthLocked;
					try {
						if(pInfo.Band.Columns.VisibleColumnCount > 0 && pInfo.ColIndex == -1)
							bCol.WidthLocked = true;
						pInfo.Band.Columns.Insert(pInfo.Index, bCol);
						View.SetColumnPosition(bCol, pInfo.RowIndex, pInfo.ColIndex);
					}
					finally {
						bCol.WidthLocked = widthLocked;
					}
				}
				finally {
					col.View.EndUpdate();
				}
			}
		}
	}
}
