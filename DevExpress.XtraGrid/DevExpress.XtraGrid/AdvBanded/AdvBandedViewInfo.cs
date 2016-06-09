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
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.BandedGrid;
using System.Collections.Generic;
namespace DevExpress.XtraGrid.Views.BandedGrid.ViewInfo {
	internal class NullAdvBandedGridViewInfo : AdvBandedGridViewInfo {
		public NullAdvBandedGridViewInfo(AdvBandedGridView gridView) : base(gridView) { }
		public override bool IsNull { get { return true; } }
	}
	public class AdvBandedGridViewInfo : BandedGridViewInfo {
		int _columnPanelRowCount, _rowLineCount;
		Dictionary<GridColumn, ColumnRowInfo> columnRowsInfo;
		public AdvBandedGridViewInfo(AdvBandedGridView gridView) : base(gridView) {
			this._rowLineCount = this._columnPanelRowCount = 1;
			this.columnRowsInfo = new Dictionary<GridColumn, ColumnRowInfo>();
		}
		public new AdvBandedGridView View { get { return base.View as AdvBandedGridView; } }
		protected override bool AllowFooterCellUseAllBounds { get { return false; } }
		protected override GridColumnInfoArgs CalcColumnHitInfo(Point pt, bool onlyFixedColumns, GridColumnsInfo cols) {
			foreach(GridColumnInfoArgs ci in cols) {
				if(!IsFixedLeftPaint(ci) && !IsFixedRightPaint(ci) && onlyFixedColumns) continue;
				if(!ci.Bounds.IsEmpty && IntInRange(pt.X, ci.Bounds.Left, ci.Bounds.Right)) {
					if(ci.Type == GridColumnInfoType.EmptyColumn) continue;
					if(pt.Y > ci.Bounds.Bottom && ci.Column != null) {
						BandedGridColumn bc = ci.Column as BandedGridColumn;
						GridBand band = bc.OwnerBand;
						if(band == null) return null;
						GridBandRowCollection rows = View.GetBandRows(band);
						GridBandRow row = rows.FindRow(bc);
						if(row == null || rows.IndexOf(row) == rows.Count - 1)
							return ci;
						continue;
					}
					return ci;
				}
			}
			return null;
		}
		public override bool IsFixedRight(GridColumnInfoArgs ci) {
			if(ci != null) {
				if(IsFixedRightPaint(ci) && (ci.Info.Location == GridColumnLocation.Left || ci.Info.Location == GridColumnLocation.Single)) {
					GridBandInfoArgs bi = ci.Tag as GridBandInfoArgs;
					if(bi == null) return false;
					if(bi.Band.RootBand == FixedRightBand) return true;
					return false;
				}
			}
			return false;
		}
		public override bool IsFixedLeft(GridColumnInfoArgs ci) {
			if(ci != null) {
				GridBandInfoArgs bi = ci.Tag as GridBandInfoArgs;
				if(bi != null && bi.Band.RootBand == FixedLeftBand) {
				if(IsFixedLeftPaint(ci) && (ci.Info.Location == GridColumnLocation.Right || ci.Info.Location == GridColumnLocation.Single)) return true;
			}
			}
			return false;
		}
		protected override bool GetHeaderIsTopMost(GridColumnInfoArgs info) {
			if(View.OptionsView.ShowBands) return base.GetHeaderIsTopMost(info);
			return info.Info.StartRow == 0;
		}
		protected override AutoWidthCalculator CreateWidthCalculator() {
			return new AdvBandAutoWidthCalculator(View);
		}
		protected override void CheckLeftEdgeHitTest(GridHitInfo hi, GridColumnInfoArgs ci) {
			if(hi.HitTest == GridHitTest.ColumnEdge) {
				foreach(GridColumnInfoArgs col in ColumnsInfo) {
					Rectangle r = col.Bounds;
					if(IsRightToLeft) {
						r.X -= ControlUtils.ColumnResizeEdgeSize + 2;
					}
					r.Width += ControlUtils.ColumnResizeEdgeSize + 2;
					if(r.Contains(hi.HitPoint)) {
						hi.Column = col.Column;
						hi.ColumnInfo = col;
						return;
					}
				}
				hi.HitTest = GridHitTest.Column;
			}
		}
		public override int RowLineCount { get { return _rowLineCount; } }
		public override int ColumnPanelRowCount { get { return _columnPanelRowCount; } }
		protected internal virtual int CalcColumnPanelRowCount() {
			Hashtable hash = new Hashtable();
			for(int n = 0; n < View.Columns.Count; n++) {
				BandedGridColumn col = View.Columns[n];
				if(col.VisibleIndex < 0 || col.OwnerBand == null || !col.OwnerBand.ReallyVisible) continue;
				Hashtable bands = hash[col.OwnerBand] as Hashtable;
				if(bands == null) {
					bands = new Hashtable();
					hash[col.OwnerBand] = bands;
				} 
				int rc = bands.ContainsKey(col.RowIndex) ? (int)bands[col.RowIndex] : 0;
				bands[col.RowIndex] = Math.Max(rc, col.RowCount);
			}
			int count = 0;
			foreach(Hashtable rows in hash.Values) {
				int lc = 0;
				foreach(DictionaryEntry entry in rows) {
					lc += (int)entry.Value;
				}
				count = Math.Max(count, lc);
			}
			return count;
		}
		protected override bool IsRowAutoHeight { get { return false; } }
		protected virtual void CalcRectContantsEx() {
			this._columnPanelRowCount = 1;
			this._rowLineCount = this._columnPanelRowCount = Math.Max(CalcColumnPanelRowCount(), this._columnPanelRowCount);
			this.columnRowsInfo = GetColumnsRowCount();
		}
		protected override void ValidateTopRowIndexByPixel() {
			CalcRectContantsEx();
			base.ValidateTopRowIndexByPixel();
		}
		protected override void CalcRectsConstants() {
			if(_rowLineCount == 1) CalcRectContantsEx();
			base.CalcRectsConstants();
		}
		protected internal virtual int CalcColumnRowCount(BandedGridColumn column, bool isLastRow, int startRow, int maxColumnRowCount) {
			int colRowCount = column.RowCount;
			if(column.AutoFillDown) {
				if(isLastRow) 
					colRowCount = ColumnPanelRowCount - startRow; 
				else {
					colRowCount = maxColumnRowCount;
				}
				return colRowCount;
			}
			return colRowCount;
		}
		protected Dictionary<GridColumn, ColumnRowInfo> ColumnRowsInfo { get { return columnRowsInfo; } }
		protected Dictionary<GridColumn, ColumnRowInfo> GetColumnsRowCount() {
			Hashtable ownerBands = new Hashtable();
			Dictionary<GridColumn, ColumnRowInfo> res = new Dictionary<GridColumn, ColumnRowInfo>();
			foreach(BandedGridColumn column in View.Columns) {
				if(column.VisibleIndex < 0 || column.OwnerBand == null) continue;
				if(ownerBands.ContainsKey(column.OwnerBand)) continue;
				ownerBands[column.OwnerBand] = View.GetBandRows(column.OwnerBand);
			}
			foreach(GridBandRowCollection rows in ownerBands.Values) {
			int startRow = 0;
				for(int r = 0; r < rows.Count; r++) {
					GridBandRow row = rows[r];
					if(row.Columns.Count == 0) continue;
					int mxCount = 0;
					int maxColumnRowCount = row.MaxColumnRowCount;
					bool isLastRow = (r == rows.Count - 1);
					for(int c = 0; c < row.Columns.Count; c++) {
						BandedGridColumn column = row.Columns[c];
						if(column.VisibleIndex < 0) continue;
						int colRowCount = 0;
						if(res.ContainsKey(column)) {
							colRowCount = res[column].RowCount;
						}
						else {
						colRowCount = CalcColumnRowCount(column, isLastRow, startRow, maxColumnRowCount);
						}
						mxCount = Math.Max(mxCount, colRowCount);
						res[column] = new ColumnRowInfo(startRow, colRowCount);
					}
					startRow += mxCount;
						}
						}
			return res;
		}
		public override ArrayList CalcUniqueEditors() {
			Hashtable hash = new Hashtable();
			GridControl.EditorHelper.DefaultRepository.GetRepositoryItem(typeof(string));
			GridControl.EditorHelper.DefaultRepository.GetRepositoryItem(typeof(bool));
			GridControl.EditorHelper.DefaultRepository.GetRepositoryItem(typeof(DateTime));
			FillRepositoryHashtable(GridControl.EditorHelper.DefaultRepository.Items, hash);
			ArrayList multiRowEditors = new ArrayList();
			foreach(KeyValuePair<GridColumn, ColumnRowInfo> pair in ColumnRowsInfo) {
				RepositoryItem item = pair.Key.RealColumnEdit;
				if(item.IsDisposed) continue;
				if(pair.Value.RowCount == 1) {
					if(hash.ContainsKey(item)) continue;
					hash[item] = new EditorInfo(FillRepositoryCreateViewInfo(item));
					continue;
				}
				multiRowEditors.Add(new EditorInfo(FillRepositoryCreateViewInfo(item), pair.Value.RowCount));
								}
			ArrayList res = new ArrayList(hash.Values);
			res.AddRange(multiRowEditors);
			return res;
							}
		protected override void CalcBandColumnsDrawInfo(GridBandInfoArgs bi, BandPositionInfo rootInfo, BandPositionInfo posInfo) {
			GridColumnInfoArgs ci;
			int left = bi.Bounds.Left, top = ViewRects.ColumnPanel.Top;
			if(IsRightToLeft) left = bi.Bounds.Right;
			int startRow = 0;
			ArrayList list = new ArrayList();
			if(!bi.HasChildren && bi.Band != null && bi.Band.Columns.Count > 0) {
				GridBandRowCollection rows = View.GetBandRows(bi.Band);
				for(int r = 0; r < rows.Count; r++) {
					GridBandRow row = rows[r];
					if(row.Columns.Count == 0) continue;
					int mxCount = 0;
					int savedLeft = left;
					bool isLastRow = (r == rows.Count - 1);
					bool isSameColumnRowCount = row.IsColumnRowCountEquals;
					int maxColumnRowCount = row.MaxColumnRowCount;
					int cellIndex = 0;
					int columnsInfoCount = ColumnsInfo.Count;
					for(int c = 0; c < row.Columns.Count; c++) {
						CalcBandColumnCore(bi, rootInfo, row, list, ref left, top, ref mxCount, isLastRow, isSameColumnRowCount, maxColumnRowCount, ref cellIndex, c);
					}
					if(posInfo.IsRight && AllowUseRightHeaderKind && row.Columns.Count > 0) {
						if(columnsInfoCount != ColumnsInfo.Count) ColumnsInfo[ColumnsInfo.Count - 1].HeaderPosition = HeaderPositionKind.Right;
					}
					top += mxCount * ColumnRowHeight;
					startRow += mxCount;
					left = savedLeft;
				}
			}
			if(list.Count > 0) {
				if(startRow == ColumnPanelRowCount) return;
			}
			foreach(GridColumnInfoArgs info in list) info.Info.BottomColumn = false;
			ci = CreateColumnInfo(null);
			ci.HtmlContext = View;
			ci.UseHtmlTextDraw = View.OptionsView.AllowHtmlDrawHeaders;
			ci.Tag = bi;
			if(bi.Band != null && IsFirstBand(bi.Band)) 
				ci.Info.CellIndex = 0;
			ci.Type = GridColumnInfoType.EmptyColumn;
			ci.Bounds = new Rectangle(left, top, bi.Bounds.Width, ColumnRowHeight * (ColumnPanelRowCount - startRow));
			ci.Info.StartRow = startRow;
			ci.Info.RowCount = ColumnPanelRowCount - startRow;
			CalcColumnInfo(ci, ref left);
			if(ci.Bounds.Width > 0) {
				bi.AddColumn(ci);
				ColumnsInfo.Add(ci);
			}
		}
		void CalcBandColumnCore(GridBandInfoArgs bi, BandPositionInfo rootInfo, GridBandRow row, ArrayList list, ref int left, int top, ref int mxCount, bool isLastRow, bool isSameColumnRowCount, int maxColumnRowCount, ref int cellIndex, int columnIndex) {
			BandedGridColumn column = row.Columns[columnIndex];
			if(column.VisibleIndex < 0) return;
			GridColumnInfoArgs ci;
			ColumnRowInfo info = ColumnRowsInfo[column];
			bool isLastColumn = (columnIndex == row.Columns.Count - 1);
			ci = CreateColumnInfo(column);
			ci.HtmlContext = View;
			ci.Info.BottomColumn = isLastRow;
			ci.Info.Location = (columnIndex == 0 ? GridColumnLocation.Left : ci.Info.Location);
			if(rootInfo.IsLeft && ci.Info.Location == GridColumnLocation.Left && !View.OptionsView.ShowIndicator)
				ci.HeaderPosition = HeaderPositionKind.Left;
			ci.Info.Location = (columnIndex == row.Columns.Count - 1 ? GridColumnLocation.Right : ci.Info.Location);
			if(row.Columns.Count == 1) ci.Info.Location = GridColumnLocation.Single;
			if(IsFirstBand(bi.Band)) {
				ci.Info.CellIndex = cellIndex++;
			}
			list.Add(ci);
			ci.Info.StartRow = info.StartRow;
			ci.Info.RowCount = info.RowCount;
			ci.Tag = bi;
			ci.Bounds = new Rectangle(left - (IsRightToLeft ? column.VisibleWidth : 0), top, column.VisibleWidth, ColumnRowHeight * info.RowCount);
			CalcColumnInfo(ci, ref left);
			if(ci.Bounds.Width > 0) {
				bi.AddColumn(ci);
				ColumnsInfo.Add(ci);
			}
			if(!isSameColumnRowCount && maxColumnRowCount != info.RowCount) {
				CreateEmptyColumnInfoCore(bi, row, isLastRow, maxColumnRowCount, columnIndex, column, ci, info, isLastColumn);
			}
			mxCount = Math.Max(mxCount, info.RowCount);
		}
		void CreateEmptyColumnInfoCore(GridBandInfoArgs bi, GridBandRow row, bool isLastRow, int maxColumnRowCount, int columnIndex, BandedGridColumn column, GridColumnInfoArgs ci, ColumnRowInfo info, bool isLastColumn) {
			ci.Info.BottomColumn = false;
			Point p = new Point(ci.Bounds.Left, ci.Bounds.Bottom);
			int emptyRowCount = maxColumnRowCount - info.RowCount, emptyStartRow = info.NextRow;
			bool allowRightBorder = true, allowBottomBorder = true;
			if(!isLastColumn) {
				allowRightBorder = false;
				int nextColRowCount = ColumnRowsInfo[row.Columns[columnIndex + 1]].RowCount;
				if(nextColRowCount > info.RowCount) {
					if(nextColRowCount < maxColumnRowCount) {
						CreateEmptyColumnInfo(bi, p.X, p.Y, column.VisibleWidth, info.NextRow, nextColRowCount - info.RowCount, true, false).Info.CellIndex = ci.Info.CellIndex;
						p.Y += (nextColRowCount - info.RowCount) * ColumnRowHeight;
						emptyStartRow = info.NextRow + (nextColRowCount - info.RowCount);
						emptyRowCount = maxColumnRowCount - nextColRowCount;
					}
					else {
						allowRightBorder = true;
					}
				}
			}
			if(isLastRow)
				allowBottomBorder = (emptyStartRow + emptyRowCount == ColumnPanelRowCount);
			CreateEmptyColumnInfo(bi, p.X, p.Y, column.VisibleWidth, emptyStartRow, emptyRowCount, allowRightBorder, allowBottomBorder).Info.CellIndex = ci.Info.CellIndex;
		}
		protected virtual GridColumnInfoArgs CreateEmptyColumnInfo(GridBandInfoArgs bi, int left, int top, int width, int startRow, int rowCount, bool allowRightBorder, bool allowBottomBorder) {
			GridColumnInfoArgs ci = CreateColumnInfo(null);
			ci.Info.BottomColumn = false;
			ci.Tag = bi;
			ci.Type = GridColumnInfoType.EmptyColumn;
			ci.Bounds = new Rectangle(left, top, width, ColumnRowHeight * rowCount);
			ci.Info.StartRow = startRow;
			ci.Info.RowCount = rowCount;
			ci.Info.AllowRightBorder = allowRightBorder;
			ci.Info.AllowBottomBorder = allowBottomBorder;
			CalcColumnInfo(ci, ref left);
			if(ci.Bounds.Width > 0) {
				bi.AddColumn(ci);
				ColumnsInfo.Add(ci);
			}
			return ci;
		}
	}
	public class ColumnRowInfo {
		public int StartRow, RowCount;
		public ColumnRowInfo(int startRow, int rowCount) {
			this.StartRow = startRow;
			this.RowCount = rowCount;
		}
		public int NextRow { get { return StartRow + RowCount; } }
	}
}
