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
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.BandedGrid.Drawing;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
namespace DevExpress.XtraGrid.Dragging {
	public class BandPositionInfo : PositionInfo {
		protected GridBandCollection fDestinationCollection;
		public BandPositionInfo() {
			this.fDestinationCollection = null;
		}
		public override bool IsEquals(PositionInfo pi) {
			BandPositionInfo cpi = pi as BandPositionInfo;
			bool res = base.IsEquals(pi);
			if(!res || cpi == null) return false;
			return this.DestinationCollection == cpi.DestinationCollection;
		}
		public GridBandCollection DestinationCollection { get { return fDestinationCollection; } }
		internal void SetDestinationCollectionCore(GridBandCollection newValue) { this.fDestinationCollection = newValue; }
	}
	public class BandedColumnPositionInfo : ColumnPositionInfo {
		protected GridBand fBand;
		public BandedColumnPositionInfo() {
			this.fBand = null;
		}
		public override bool IsEquals(PositionInfo pi) {
			BandedColumnPositionInfo cpi = pi as BandedColumnPositionInfo;
			bool res = base.IsEquals(pi);
			if(!res || cpi == null) return false;
			return this.Band == cpi.Band;
		}
		public GridBand Band { get { return fBand; } }
		internal void SetBandCore(GridBand newValue) {  this.fBand = newValue; }
	}
	public class BandedGridDragManager : GridDragManager {
		public BandedGridDragManager(BandedGridView view) : base(view) {
		}
		public new BandedGridView View { get { return base.View as BandedGridView; } }
		public new BandedGridViewInfo ViewInfo { get { return base.ViewInfo as BandedGridViewInfo; } }
		protected override ColumnPositionInfo CreateColumnPositionInfo() {
			return new BandedColumnPositionInfo();
		}
		protected virtual BandPositionInfo CreateBandPositionInfo() { return new BandPositionInfo(); }
		protected override PositionInfo CalcColumnDrag(GridHitInfo hit, GridColumn column) {
			if(hit.InGroupPanel) {
				return CalcInGroupPanel(hit, column);
			}
			BandedColumnPositionInfo res = CreateColumnPositionInfo() as BandedColumnPositionInfo,
				emptyRes = CreateColumnPositionInfo() as BandedColumnPositionInfo;
			if(!View.GridControl.Visible) return emptyRes;
			BandedGridColumn col = column as BandedGridColumn;
			if(col == null || col.View != View) return res;
			if(hit.HitTest == GridHitTest.CustomizationForm) {
				res.SetBandCore(col.OwnerBand);
				res.SetIndex(CustomizationFormPosition);
				res.Valid = col.OptionsColumn.AllowShowHide;
				return res;
			}
			if(hit.HitPoint.Y > ViewInfo.ViewRects.ColumnPanel.Bottom + 50 || IsPointOutsideControl(hit.HitPoint)) {
				res.SetIndex(HideElementPosition);
				res.Valid = col.OptionsColumn.AllowShowHide && View.OptionsCustomization.AllowQuickHideColumns;
				return res;
			}
			if(column.GroupIndex >= 0) {
				emptyRes.SetIndex(-1);
				emptyRes.Valid = true;
			}
			GridBand hitBand = null;
			BandedGridHitInfo hi = hit as BandedGridHitInfo;
			GridBandInfoArgs bi = null;
			if(hit.HitPoint.Y < ViewInfo.ViewRects.BandPanel.Top && View.OptionsView.ShowBands) return res;
			bi = ViewInfo.CalcBandHitInfo(hit.HitPoint);
			if(bi != null) hitBand = bi.Band;
			if(hitBand == null || hitBand.HasChildren) return res;
			Rectangle r = new Rectangle(bi.Bounds.Left, ViewInfo.ViewRects.ColumnPanel.Top, bi.Bounds.Width, ViewInfo.ViewRects.ColumnPanel.Height);
			if(bi.HasColumns && bi.Columns.Count > 0) {
				ColumnDropInfo drInfo = CalcColumnInfoCore(bi.Columns, hi, r);
				if(!drInfo.PositionInfo.Valid) return res;
				res = UpdateBandedColumnPositionInfo(col, drInfo, hitBand);
				if(!CheckColumnOptions(column, res)) return emptyRes;
				return res;
			} else {
				res.SetBandCore(hitBand);
				res.SetIndex(0);
				res.Bounds = r;
				res.Valid = true;
				if(!CheckColumnOptions(column, res)) return emptyRes;
			}
			return res;
		}
		protected virtual BandedColumnPositionInfo UpdateBandedColumnPositionInfo(BandedGridColumn col, ColumnDropInfo drInfo, GridBand hitBand) {
			BandedColumnPositionInfo emptyRes = CreateColumnPositionInfo() as BandedColumnPositionInfo;
			BandedColumnPositionInfo res;
			res = drInfo.PositionInfo as BandedColumnPositionInfo;
			res.SetBandCore(hitBand);
			if(res.Index != 9999) {
				int index = hitBand.Columns.IndexOf(drInfo.Column as BandedGridColumn);
				if(index == -1) return emptyRes;
				res.SetIndex(drInfo.Pos == DropPosition.Left ? index : index + 1);
			} else {
				res.SetIndex(0);
			}
			if(CanColumnHasSameIndex) return res;
			if(col.OwnerBand == res.Band && col.Visible) {
				int curIndex = hitBand.Columns.IndexOf(col);
				if(curIndex == res.Index || curIndex + 1 == res.Index) {
					if(!drInfo.PositionInfo.InGroupPanel && col.GroupIndex >= 0) {
						GridColumnInfoArgs ci = ViewInfo.ColumnsInfo[col];
						if(ci != null) res.Bounds = ci.Bounds;
						return res;
					}
					return emptyRes;
				}
			}
			return res;
		}
		protected virtual bool CanColumnHasSameIndex { get { return false; } }
		public override PositionInfo Calc(GridHitInfo hit) {
			if(DragObject is GridBand) return CalcBandDrag(hit, DragObject as GridBand);
			return base.Calc(hit);
		}
		public override void EndDrag() {
			if(DragObject is GridBand) {
				EndDragBand(DragObject as GridBand);
			}
			base.EndDrag();
		}
		protected virtual void EndDragBand(GridBand band) {
			EndDragBandCore(band);
			View.FireChangedBands();
		}
		protected virtual void EndDragBandCore(GridBand band) {
			BandPositionInfo pInfo = LastPosition as BandPositionInfo;
			if(pInfo == null || band == null || !pInfo.Valid) return;
			if(pInfo.Index < 0) {
				if(pInfo.DestinationCollection == null) 
					band.Visible = false;
				else {
					View.BeginUpdate();
					try {
						band.Visible = true;
						pInfo.DestinationCollection.InsertRoot(band);
					}
					finally {
						View.EndUpdate();
					}
				}
				return;
			}
			View.BeginUpdate();
			try {
				if(pInfo.DestinationCollection == band.Collection)
					band.Collection.MoveTo(LastPosition.Index, band);
				else 
					pInfo.DestinationCollection.Insert(pInfo.Index, band);
				band.Visible = true;
			}
			finally {
				View.EndUpdate();
			}
		}
		protected override void EndDragColumnCore(GridColumn col) {
			BandedGridColumn bCol = col as BandedGridColumn;
			BandedColumnPositionInfo pInfo = LastPosition as BandedColumnPositionInfo;
			if(pInfo == null || col == null || !pInfo.Valid) return;
			if(pInfo.InGroupPanel) {
				col.View.BeginUpdate();
				try {
					col.Visible = true;
					if(col.GroupIndex > -1)
						col.GroupIndex = pInfo.Index;
					else {
						col.View.SortInfo.InsertGroup(pInfo.Index, col);
					}
				} finally {
					col.View.EndUpdate();
				}
				View.RaiseColumnPositionChanged(col);
				return;
			}
			if(pInfo.Index <= HideElementPosition) {
				col.UnGroup();
				col.Visible = false;
				return;
			}
			if(pInfo.Index == -1) {
				col.UnGroup();
				col.Visible = true;
				return;
			}
			if(bCol == null) return;
			col.View.BeginUpdate();
			col.View.LockRaiseColumnPositionChanged();
			try {
				bCol.Visible = true;
				if(col.GroupIndex > -1 && DragStartHitInfo.InGroupColumn) col.GroupIndex = -1;
				if(bCol.OwnerBand == pInfo.Band) {
					pInfo.Band.Columns.MoveTo(pInfo.Index, bCol);
				} else {
					pInfo.Band.Columns.Insert(pInfo.Index, bCol);
				}
			}
			finally {
				col.View.EndUpdate();
				col.View.UnlockRaiseColumnPositionChanged();
				View.RaiseColumnPositionChanged(col);
			}
		}
		protected virtual PositionInfo CalcBandDrag(GridHitInfo hit, GridBand drBand) {
			GridBand hitBand = null;
			BandPositionInfo res = CreateBandPositionInfo();
			if(!View.GridControl.Visible) return res;
			BandedGridHitInfo hi = hit as BandedGridHitInfo;
			if(hi == null || hi.HitTest == BandedGridHitTest.BandEdge) return res;
			int curIndex = drBand.Index;
			DropPosition pos = DropPosition.None;
			GridBandInfoArgs bi = null;
			hitBand = hi.Band;
			if(hit.HitTest == GridHitTest.CustomizationForm) {
				res.SetIndex(-1);
				res.SetDestinationCollectionCore(null);
				res.Valid = true;
				return res;
			}
			if(hit.HitPoint.Y > ViewInfo.ViewRects.ColumnPanel.Bottom + 50 || IsPointOutsideControl(hit.HitPoint)) {
				res.SetIndex(HideElementPosition);
				res.Valid = View.OptionsCustomization.AllowQuickHideColumns;
				return res;
			}
			if(!View.ViewRect.Contains(hit.HitPoint)) return res;
			if(hitBand == null) {
				if(hit.HitPoint.Y < ViewInfo.ViewRects.BandPanel.Top) return res;
				bi = ViewInfo.CalcBandHitInfo(hit.HitPoint);
				if(bi != null) hitBand = bi.Band;
			}
			BandPositionInfo pInfo = null;
			if(hitBand != null) {
				if(bi == null) bi = ViewInfo.BandsInfo.FindBand(hitBand);
				pos = CalcByBounds(bi.Bounds, hit.HitPoint);
				pInfo = CalcByDropPosition(bi, pos, hit.HitPoint);
			} else {
				if(View.Bands.VisibleBandCount == 0 && (!drBand.Visible || drBand.View == null)) {
					pInfo = new BandPositionInfo();
					pInfo.SetDestinationCollectionCore(View.Bands);
					pInfo.SetIndex(0);
					pInfo.Valid = true;
				}
			}
			if(pInfo != null && pInfo.Valid) {
				if(!pInfo.DestinationCollection.CanAdd(drBand) && drBand.ParentBand != pInfo.DestinationCollection.OwnerBand) return res;
				if(pInfo.DestinationCollection == drBand.Collection && drBand.Visible) {
					if(curIndex == pInfo.Index) return res;
					if(curIndex > -1 && curIndex + 1 == pInfo.Index) return res;
				}
				if(!CheckBandOptions(drBand, pInfo)) return res;
				res = pInfo;
				res.Bounds = CalcBandRect(res);
				res.Valid = !res.Bounds.IsEmpty;
				return res;
			}
			return res;
		}
		protected override bool CheckColumnOptions(GridColumn drColumn, ColumnPositionInfo pInfo) {
			BandedGridColumn bCol = drColumn as BandedGridColumn;
			BandedColumnPositionInfo bInfo = pInfo as BandedColumnPositionInfo;
			if(bInfo == null || bCol == null) return true;
			if(pInfo.InGroupPanel || pInfo.Index < 0) return true;
			if(View.IsDesignMode || View.ForcedDesignMode) return true;
			if(!View.OptionsCustomization.AllowChangeColumnParent) {
				if(bCol.OwnerBand == null || bInfo.Band == null) return true;
				if(bCol.OwnerBand != bInfo.Band) return false;
			}
			return true;
		}
		protected virtual bool CheckBandOptions(GridBand drBand, BandPositionInfo pInfo) {
			if(!View.OptionsCustomization.AllowChangeBandParent) {
				if(drBand.Collection == null) return true;
				if(drBand.Collection == pInfo.DestinationCollection && pInfo.Index >= 0) {
					if(drBand.Collection != View.Bands) return true;
				} else {
					return false;
				}
			}
			if(pInfo.Index < 0) return true;
			GridBand rootBand = drBand.RootBand;
			if(rootBand == null) return true;
			GridBand rootDestBand = pInfo.DestinationCollection.OwnerBand == null ? null : pInfo.DestinationCollection.OwnerBand.RootBand;
			int fLeftIndex = ViewInfo.FixedLeftBand == null ? -1 : ViewInfo.FixedLeftBand.Index,
				fRightIndex = ViewInfo.FixedRightBand == null ? -1 : ViewInfo.FixedRightBand.Index;
			if(fLeftIndex == -1 && fRightIndex == -1) return true;
			if(rootDestBand != null) {
				return rootBand.Fixed == rootDestBand.Fixed;
			}
			if(fLeftIndex != -1 && pInfo.Index <= fLeftIndex && drBand.Fixed != FixedStyle.Left) return false;
			if(fLeftIndex != -1 && pInfo.Index > fLeftIndex + 1 && drBand.Fixed == FixedStyle.Left) return false;
			if(fRightIndex != -1 && pInfo.Index > fRightIndex && drBand.Fixed != FixedStyle.Right) return false;
			if(fRightIndex != -1 && pInfo.Index < fRightIndex && drBand.Fixed == FixedStyle.Right) return false;
			return true;
		}
		protected virtual Rectangle CalcBandRect(BandPositionInfo res) {
			Rectangle r = Rectangle.Empty;
			GridBandInfoArgs bi = null;
			if(res.DestinationCollection.VisibleBandCount == 0 || res.Index == -1) {
				if(res.DestinationCollection.OwnerBand == null) {
					r = ViewInfo.ViewRects.BandPanel;
					if(ViewInfo.ViewRects.IndicatorWidth > 0) {
						r.X += ViewInfo.ViewRects.IndicatorWidth;
						r.Width -= ViewInfo.ViewRects.IndicatorWidth;
					}
					return r;
				}
				bi = ViewInfo.BandsInfo.FindBand(res.DestinationCollection.OwnerBand);
				if(bi == null) return r;
				r.Height = ViewInfo.BandRowHeight;
				r = new Rectangle(bi.Bounds.Left, bi.Bounds.Bottom - r.Height / 2, bi.Bounds.Width, r.Height);
				res.IsHorizontal = true;
				return r;
			}
			GridBand band = res.DestinationCollection[(res.Index >= res.DestinationCollection.Count ? res.DestinationCollection.Count - 1 : res.Index)];
			if(band == null || (!band.Visible && band.Index == 0)) return r;
			if(!band.Visible) {
				GridBand prevBand = band;
				band = res.DestinationCollection.GetVisibleBand(band);
				if(band == null) band = res.DestinationCollection[prevBand.Index - 1];
			}
			bi = ViewInfo.BandsInfo.FindBand(band);
			if(bi == null) return r;
			r = bi.Bounds;
			r.Width = bi.Bounds.Width / 2;
			r.Height = ViewInfo.ViewRects.BandPanel.Bottom - r.Top;
			if(band.Index < res.Index) {
				if(UseArrows) {
					r.X = bi.Bounds.Right - 1;
					r.Width = 1;
				}
				else {
					r.X = bi.Bounds.Right - r.Width;
				}
			}
			return r;
		}
		protected virtual BandPositionInfo CalcByDropPosition(GridBandInfoArgs bi, DropPosition pos, Point hitPoint) {
			GridBand dr = DragObject as GridBand;
			GridBand dest = bi == null ? null : bi.Band;
			BandPositionInfo res = CreateBandPositionInfo();
			if(dr == null || dest == null || dest == dr || pos == DropPosition.None) return res;
			switch(pos) {
				case DropPosition.Current:
				case DropPosition.Left:
					res.SetDestinationCollectionCore(dest.Collection);
					res.SetIndex(dest.Index);
					break;
				case DropPosition.Right:
					res.SetDestinationCollectionCore(dest.Collection);
					res.SetIndex(dest.Index + 1);
					break;
				case DropPosition.Up:
					return res;
				case DropPosition.Down:
					res.SetDestinationCollectionCore(dest.Children);
					res.SetIndex(bi.Bounds.Bottom > hitPoint.Y || dest.Children.VisibleBandCount > 0 ? -1 : 0); 
					break;
				default:
					return res;
			}
			res.Valid = true;
			return res;
		}
	}
}
