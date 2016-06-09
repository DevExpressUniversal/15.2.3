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
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.BandedGrid.Drawing;
namespace DevExpress.XtraGrid.Views.BandedGrid.ViewInfo {
	internal class NullBandedGridViewInfo : BandedGridViewInfo {
		public NullBandedGridViewInfo(BandedGridView gridView) : base(gridView) { }
		public override bool IsNull { get { return true; } }
	}
	public class BandedGridViewInfo : GridViewInfo {
		int fBandRowHeight;
		int fBandRowCount;
		public GridBandInfoCollection BandsInfo;
		GridBand fFixedRightBand, fFixedLeftBand;
		GridColumnInfoArgs fFixedLeft, fFixedRight;
		public BandedGridViewInfo(BandedGridView gridView) : base(gridView) {
			this.fFixedLeft = this.fFixedRight = null;
			this.fBandRowHeight = 20; 
			this.fBandRowCount = 1;
			this.fFixedRightBand = this.fFixedLeftBand = null;
		}
		public virtual GridBand FixedRightBand { get { return fFixedRightBand; } }
		public virtual GridBand FixedLeftBand { get { return fFixedLeftBand; } }
		public new BandedGridPainter Painter { get { return base.Painter as BandedGridPainter; } }
		public new BandedGridView View { get { return base.View as BandedGridView; } }
		public new BandedGridViewRects ViewRects { get { return base.ViewRects as BandedGridViewRects; } }
		public override GridViewRects CreateViewRects() {
			return new BandedGridViewRects(this);
		}
		public new BandedViewAppearances PaintAppearance { get { return base.PaintAppearance as BandedViewAppearances; } }
		protected override BaseAppearanceCollection CreatePaintAppearances() { return new BandedViewAppearances(View); }
		protected override BaseSelectionInfo CreateSelectionInfo() {
			return new BandedGridSelectionInfo(View);
		}
		public override GridHitInfo CreateHitInfo() { return new BandedGridHitInfo(); } 
		protected override AutoWidthCalculator CreateWidthCalculator() {
			return new BandAutoWidthCalculator(View);
		}
		public new BandedGridSelectionInfo SelectionInfo { get { return base.SelectionInfo as BandedGridSelectionInfo; } }
		protected virtual int GetAutoWidthBandPanelWidth() {
			int res = ViewRects.BandPanelWidth;
			if(HasFixedLeft) res -= View.FixedLineWidth;
			if(HasFixedRight) res -= View.FixedLineWidth;
			return res;
		}
		protected internal override Rectangle GetTargetDragRect(int baseHeight) {
			Rectangle rect = base.GetTargetDragRect(baseHeight);
			rect.Height += ViewRects.BandPanel.Height;
			return rect;
		}
		public override void RecalcColumnWidthes() {
			RecalcBandWidthes(-1, View.OptionsView.ColumnAutoWidth, GetAutoWidthBandPanelWidth());
			ViewRects.ColumnTotalWidth = CalcTotalColumnWidth();
		}
		public override void RecalcColumnWidthes(GridColumn column) {
		}
		public override void RecalcColumnWidthes(GridAutoWidthCalculatorArgs args) {
			if(args.MaxVisibleWidth == 0) return;
			RecalcBandWidthes(-1, args.IsAutoWidth, args.MaxVisibleWidth);
		}
		public virtual void RecalcOnColumnWidthChanged(BandedGridColumn column) {
			if(column == null || column.OwnerBand == null || !column.OwnerBand.ReallyVisible) return;
			if(ViewRects.BandPanelWidth == 0) return;
			BandedGridAutoWidthCalculatorArgs e = new BandedGridAutoWidthCalculatorArgs(null, View.OptionsView.ColumnAutoWidth, GetAutoWidthBandPanelWidth(), null, 0);
			WidthCalculator.ChangeObjectWidth(e, column);
			WidthCalculator.UpdateRealObjects(e);
		}
		public virtual void RecalcOnBandWidthChanged(GridBand band) {
			if(band == null) return;
			if(!band.ReallyVisible) return;
			band.SetVisibleWidthCore(band.Width);
			BandedGridAutoWidthCalculatorArgs e = new BandedGridAutoWidthCalculatorArgs(null, View.OptionsView.ColumnAutoWidth, GetAutoWidthBandPanelWidth(), null, 0);
			WidthCalculator.ChangeObjectWidth(e, band);
			WidthCalculator.UpdateRealObjects(e);
		}
		public void RecalcBandWidthes(int startBand, bool isAutoWidth, int maxVWidth) {
			RecalcBandWidthes(new BandedGridAutoWidthCalculatorArgs(null, isAutoWidth, maxVWidth, View.Bands, startBand));
		}
		public virtual void RecalcBandWidthes(BandedGridAutoWidthCalculatorArgs args) {
			WidthCalculator.Calc(args);
			WidthCalculator.UpdateRealObjects(args);
		}
		public override void Clear() {
			BandsInfo = new GridBandInfoCollection();
			base.Clear();
		}
		public virtual int BandRowCount { get { return fBandRowCount; } }
		public virtual int BandRowHeight { get { return fBandRowHeight; } }
		protected override bool GetHeaderIsTopMost(GridColumnInfoArgs info) {
			if(View.OptionsView.ShowBands) return false;
			return true;
		}
		public override bool IsShowHeaders { get { return base.IsShowHeaders || View.OptionsView.ShowBands; } }
		protected override int CalcRectsColumnPanel(int startY) {
			Rectangle r = ViewRects.Client;
			r.Y = startY;
			r.Height = BandRowHeight * BandRowCount;
			ViewRects.BandPanel = r;
			if(View.OptionsView.ShowBands) {
				startY = ViewRects.BandPanel.Bottom;
			}
			return base.CalcRectsColumnPanel(startY);
		}
		protected override void CalcRectsConstants() {
			base.CalcRectsConstants();
			this.fBandRowCount = CalcBandRowCount();
			CalcMinBandRowHeight(ScaleVertical(View.BandPanelRowHeight));
		}
		protected internal virtual int CalcBandRowCount() {
			int res = Math.Max(1, View.MinBandPanelRowCount);
			for(int n = 0; n < View.Bands.Count; n++) {
				GridBand band = View.Bands[n];
				if(!band.Visible) continue;
				res = Math.Max(res, CalcBandRowCount(band));
			}
			return res;
		}
		protected virtual int CalcBandRowCount(GridBand band) {
			if(!band.Visible) return 0;
			int res = band.RowCount;
			if(!band.HasChildren) return res;
			int chMax = 0;
			for(int n = 0; n < band.Children.Count; n++) {
				GridBand chBand = band.Children[n];
				chMax = Math.Max(CalcBandRowCount(chBand), chMax);
			}
			return res + chMax;
		}
		protected override void UpdatePaintAppearanceDefaults() {
			if(View.IsColumnHeaderAutoHeight) PaintAppearance.BandPanel.TextOptions.WordWrap = WordWrap.Wrap;
			base.UpdatePaintAppearanceDefaults();
		}
		protected virtual void CalcMinBandRowHeight(int bandHeight) {
			bandHeight = Math.Max(bandHeight, 12);
			int maxY = 0;
			Graphics g = GInfo.AddGraphics(null);
			try {
				GridBandInfoArgs e = new GridBandInfoArgs(null, new GraphicsCache(g));
				e.AutoHeight = View.IsColumnHeaderAutoHeight;
				e.InnerElements.Add(new DrawElementInfo(new GlyphElementPainter(), new GlyphElementInfoArgs(View.Images, 0, View.GetMaxHeightBandImage()), StringAlignment.Near));
				e.SetAppearance(PaintAppearance.BandPanel);
				maxY = Painter.ElementsPainter.Band.CalcObjectMinBounds(e).Height;
				if(View.OptionsView.AllowHtmlDrawHeaders) {
					e.UseHtmlTextDraw = true;
					for(int n = 0; n < Math.Min(ColumnViewInfo.AutoHeightCalculateMaxColumnCount, View.Bands.Count); n++) {
						var band = View.Bands[n];
						e.Caption = band.Caption;
						if(View.IsColumnHeaderAutoHeight) {
							e.CaptionRect = Rectangle.Empty;
							e.Bounds = new Rectangle(0, 0, band.VisibleWidth, 0);
							e.Band = band;
							e.CreateInnerCollection();
							e.UpdateCaption();
						}
						maxY = Math.Max(maxY, Painter.ElementsPainter.Band.CalcObjectMinBounds(e).Height);
					}
				}
				e.Cache.Dispose();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			this.fBandRowHeight = Math.Max(maxY, bandHeight);
		}
		protected override Rectangle CalcGroupRowDataBounds(GridRowInfo ri, Rectangle db) {
			return db;
		}
		protected override void CalcColumnsDrawInfo() {
			CalcBandsDrawInfo();
			UpdateFixedCI();
		}
		protected override void UpdateFixedRects() {
			ViewRects.FixedLeft = ViewRects.FixedRight = Rectangle.Empty;
			Rectangle r = Rectangle.Empty;
			GridBandInfoArgs bi;
			if(FixedLeftBand != null) {
				bi = BandsInfo[FixedLeftBand];
				if(bi != null) {
					r = ViewRects.Client;
					if(IsRightToLeft) {
						r.X = bi.Bounds.X;
						r.Width = Math.Max(0, ViewRects.Client.Right - View.IndicatorWidth - r.X);
					}
					else {
						r.Width = Math.Max((bi.Bounds.Right + FixedLineWidth) - r.Left, 0);
					}
					ViewRects.FixedLeft = r;
				}
				else {
					ViewRects.FixedLeft = ViewRects.Client;
				}
			}
			if(FixedRightBand != null) {
				bi = BandsInfo[FixedRightBand];
				if(bi != null) {
					r = ViewRects.Client;
					if(IsRightToLeft) {
						r.X += View.IndicatorWidth;
						int maxX = Math.Min(ViewRects.FixedLeft.IsEmpty ? ViewRects.Client.Right : ViewRects.FixedLeft.X, r.X + bi.Bounds.Width + FixedLineWidth);
						r.Width = maxX - r.X;
					}
					else {
						r.X = Math.Max(bi.Bounds.Left, ViewRects.FixedLeft.Right);
						r.Width = Math.Max(ViewRects.Client.Right - r.X, 0);
					}
					ViewRects.FixedRight = r;
				}
			}
		}
		protected virtual void UpdateFixedCI() {
			this.fFixedLeft = this.fFixedRight = null;
			for(int n = 0; n < ColumnsInfo.Count; n++) {
				GridColumnInfoArgs ci = ColumnsInfo[n];
				GridBandInfoArgs bi = ci.Tag as GridBandInfoArgs;
				if(bi == null || bi.Band == null) continue;
				GridBand band = bi.Band.RootBand;
				if(band == null) continue;
				if(band.Fixed == FixedStyle.Left) this.fFixedLeft = ci;
				if(this.fFixedRight == null && band.Fixed == FixedStyle.Right) {
					this.fFixedRight = ci;
					break;
				}
			}
		}
		protected internal virtual void CalcBandInfoState(GridBandInfoArgs bi) {
			if(bi.Band == null) return;
			bi.State = ObjectState.Normal;
			if(bi.AllowEffects) {
				if(bi.Band == SelectionInfo.PressedBand) bi.State = ObjectState.Pressed;
				if(bi.State == ObjectState.Normal && SelectionInfo.HotTrackedBand == bi.Band && bi.Band.OptionsBand.AllowHotTrack) bi.State = ObjectState.Hot;
				bi.DesignTimeSelected = false;
				if(bi.Band != null && View.GridControl.IsDesignMode) {
					bi.DesignTimeSelected = View.GridControl.GetComponentSelected(bi.Band);
				}
			}
			bi.RightToLeft = IsRightToLeft;
		}
		public override int GetColumnLeftCoord(GridColumn lookColumn) {
			BandedGridColumn column = lookColumn as BandedGridColumn;
			if(column == null || column.VisibleIndex < 0 || column.OwnerBand == null || !column.OwnerBand.Visible) return 0;
			GridBand rootBand = column.OwnerBand.RootBand;
			if(!rootBand.Visible) return 0;
			GridBandRowCollection rows = View.GetBandRows(column.OwnerBand, false);
			if(rows == null) return 0;
			GridBandRow row = rows.FindRow(column);
			if(row == null) return 0;
			int res = 0;
			foreach(GridBand band in View.Bands) {
				if(!band.Visible) continue;
				if(band == rootBand) {
					if(column.OwnerBand == rootBand) {
						break;
					}
					GridBand bnd = column.OwnerBand;
					while(bnd.ParentBand != null)  {
						int index = bnd.Index;
						for(int n = 0; n < index; n++) {
							GridBand child = bnd.ParentBand.Children[n];
							if(child.VisibleIndex < 0) continue;
							res += child.VisibleWidth;
						}
						bnd = bnd.ParentBand;
					}
					break;
				}
				res += band.VisibleWidth;
			}
			foreach(GridColumn col in row.Columns) {
				if(col == lookColumn) break;
				if(col.VisibleIndex < 0) continue;
				res += col.VisibleWidth;
			}
			return res;
		}
		public override bool HasFixedLeft { get { return FixedLeftBand != null; } }
		public override bool HasFixedRight { get { return FixedRightBand != null; } }
		public override bool HasFixedColumns {
			get { return FixedRightBand != null || FixedLeftBand != null; }
		}
		public override bool IsFixedRight(GridColumnInfoArgs ci) {
			return ci != null && ci == fFixedRight;
		}
		public override bool IsFixedLeft(GridColumnInfoArgs ci) {
			return ci != null && ci == fFixedLeft;
		}
		public override bool IsFixedLeftPaint(GridColumnInfoArgs ci) {
			if(ci == null) return false;
			GridBandInfoArgs bi = ci.Tag as GridBandInfoArgs;
			if(bi == null) return false;
			return bi.Fixed == FixedStyle.Left;
		}
		public override bool IsFixedRightPaint(GridColumnInfoArgs ci) {
			if(ci == null) return false;
			GridBandInfoArgs bi = ci.Tag as GridBandInfoArgs;
			if(bi == null) return false;
			return bi.Fixed == FixedStyle.Right;
		}
		public virtual Rectangle UpdateFixedRange(Rectangle rect, GridBandInfoArgs bi) {
			if(bi != null && bi.Fixed == FixedStyle.Left) return rect;
			return UpdateFixedRange(rect, (bi == null || bi.Fixed != FixedStyle.Right));
		}
		protected override int CalcTotalColumnWidth() {
			int totalWidth = 0;
			for(int i = 0; i < View.Bands.Count; i++) {
				GridBand band = View.Bands[i];
				if(!band.Visible) continue;
				totalWidth += band.VisibleWidth;
			}
			return totalWidth;
		}
		protected override bool AllowBehindColumn { get { return false; } }
		protected virtual void CalcChildrenBandsDrawInfo(GridBandInfoArgs parent, Point leftTop, int rows, BandPositionInfo rootInfo) {
			int left = leftTop.X, top = leftTop.Y;
			for(int i = 0; i < parent.Band.Children.Count;i++) {
				GridBand band = parent.Band.Children[i];
				if(!band.Visible) continue;
				GridBandInfoArgs bi = CreateBandInfo(band);
				bi.Bounds = new Rectangle(left - (IsRightToLeft ? band.VisibleWidth : 0), top, band.VisibleWidth, BandRowHeight * band.RowCount);
				CalcBandInfo(bi, ref left, rows, new BandPositionInfo(i, parent.Band.Children.Count, rootInfo));
				if(bi.Bounds.Width > 0) parent.AddChild(bi);
			}
		}
		protected virtual GridBandInfoArgs CreateBandInfo(GridBand band) {
			GridBandInfoArgs res = new GridBandInfoArgs(band);
			res.RightToLeft = IsRightToLeft;
			res.AutoHeight = View.IsColumnHeaderAutoHeight;
			return res;
		}
		protected virtual void CalcBandsDrawInfo() {
			int left = ViewRects.BandPanel.Left,
				top = ViewRects.BandPanel.Top, firstIndex;
			Size size = Size.Empty;
			int direction = 1;
			if(IsRightToLeft) {
				left = ViewRects.BandPanel.Right;
				direction = -1;
			}
			firstIndex = ViewRects.IndicatorWidth > 0 ? -1 : 0;
			bool leftCoordSubstracted = false;
			if(FixedLeftBand == null) {
				left -= View.LeftCoord * direction;
				leftCoordSubstracted = true;
			}
			for(int i = firstIndex; i < View.Bands.Count + 1;i++) {
				GridBand band = null;
				band = (i >= 0 && i < View.Bands.Count ? View.Bands[i] : null);
				GridBandInfoArgs bi = CreateBandInfo(band);
				if(i == -1)	bi.Type = GridColumnInfoType.Indicator;
				if(i == View.Bands.Count) 
					bi.Type = GridColumnInfoType.BehindColumn;
				if(band != null && !band.Visible) continue;
				if(band != null && band.Fixed != FixedStyle.Left) {
					if(!leftCoordSubstracted) {
						left -= View.LeftCoord * direction;
						leftCoordSubstracted = true;
					}
				}
				if(band != null && band == FixedRightBand) {
					left += View.FixedLineWidth * direction;
					int w = CalcRestBandsWidth(band);
					if(IsRightToLeft) {
						if(left - w < ViewRects.BandPanel.Right) {
							left = ViewRects.BandPanel.Left + w;
						}
					}
					else {
						if(left + w > ViewRects.BandPanel.Right) {
							left = ViewRects.BandPanel.Right - w;
						}
					}
				}
				int rowCount = band == null ? 1 : band.RowCount;
				if(IsRightToLeft)
					bi.Bounds = new Rectangle(left - (band == null ? 0 : band.VisibleWidth), top, band == null ? 0 : band.VisibleWidth, BandRowHeight * rowCount);
				else
					bi.Bounds = new Rectangle(left, top, band == null ? 0 : band.VisibleWidth, BandRowHeight * rowCount);
				CalcBandInfo(bi, ref left, 0, new BandPositionInfo(i, View.Bands.Count, null));
				if(band != null && band == FixedLeftBand) {
					left += View.FixedLineWidth * direction;
				}
				if(bi.Bounds.Width > 0) BandsInfo.Add(bi);
			}
		}
		public class BandPositionInfo {
			int curIndex;
			int count;
			BandPositionInfo rootInfo;
			public BandPositionInfo(int curIndex, int count, BandPositionInfo rootInfo) {
				this.curIndex = curIndex;
				this.count = count;
				this.rootInfo = rootInfo;
			}
			public int CurIndex { get { return curIndex; } }
			public int Count { get { return count; } }
			public BandPositionInfo RootInfo { get { return rootInfo; } }
			public bool IsTopMost { get { return RootInfo == null; } }
			public bool IsLeft { get { return CurIndex == 0 && IsTopMost; } }
			public bool IsRight { get { return CurIndex == Count && Count > 1 && IsTopMost; } }
		}
		protected virtual void CalcBandInfo(GridBandInfoArgs bi, ref int lastLeft, int rows, BandPositionInfo posInfo) {
			int direction = IsRightToLeft ? -1 : 1;
			Size size = bi.Bounds.Size;
			if(size.Width == 0)
				size.Width = (bi.Band == null ? 100 : bi.Band.VisibleWidth);
			if(size.Height == 0) size.Height = BandRowHeight;
			bi.SetAppearance(PaintAppearance.BandPanel);
			if(bi.Band != null) {
				bi.Fixed = bi.Band.RootBand.Fixed;
				AppearanceObject app = new AppearanceObject();
				AppearanceHelper.Combine(app, new AppearanceObject[] { bi.Band.AppearanceHeader, PaintAppearance.BandPanel});
				bi.SetAppearance(app);
				if(posInfo.IsLeft && !View.OptionsView.ShowIndicator) bi.HeaderPosition = HeaderPositionKind.Left;
				if(posInfo.IsRight && AllowUseRightHeaderKind) bi.HeaderPosition = HeaderPositionKind.Right;
			}
			bi.Bounds = new Rectangle(bi.Bounds.Location, size);
			bi.IsTopMost = posInfo.IsTopMost;
			Rectangle bounds = bi.Bounds;
			switch(bi.Type) {
				case GridColumnInfoType.Indicator :
					if(IsRightToLeft) {
						bounds.X = ViewRects.ColumnPanel.Right - ViewRects.IndicatorWidth;
					}
					else {
						bounds.X = ViewRects.ColumnPanel.Left;
					}
					bounds.Width  = ViewRects.IndicatorWidth;
					bounds.Height = BandRowHeight * BandRowCount;
					lastLeft += bounds.Width * direction;
					break;
				case GridColumnInfoType.BehindColumn :
					if(IsRightToLeft) {
						bounds.Width = (lastLeft - ViewRects.ColumnPanelActual.Left) + 1; 
						bounds.X = ViewRects.ColumnPanelActual.Left - 1;
					}
					else
						bounds.Width = (ViewRects.Scroll.Right - lastLeft) + 1; 
					if(bounds.Width == 1) bounds.Width = 0;
					bounds.Height = BandRowHeight * BandRowCount;
					if(posInfo.IsRight && AllowUseRightHeaderKind) bi.HeaderPosition = HeaderPositionKind.Right;
					break;
			}
			bi.Bounds = bounds;
			if(bi.Type != GridColumnInfoType.Column) {
				GridColumnInfoArgs ci = CreateColumnInfo(null);
				ci.Type = bi.Type;
				ci.Bounds = new Rectangle(bounds.X, ViewRects.ColumnPanel.Top, bounds.Width, ColumnRowHeight * ColumnPanelRowCount);
				int cleft = ci.Bounds.Left;
				if(IsRightToLeft) cleft = ci.Bounds.Right + 1;
				CalcColumnInfo(ci, ref cleft);
				if(ci.Bounds.Width > 0) ColumnsInfo.Add(ci);
				return;
			}
			if(bounds.Width > 0) {
				lastLeft += bi.Bounds.Width * direction;
				if(bi.ValidateCoord) {
					if(bi.Bounds.Right  < ViewRects.BandPanelActual.Left ||
						bi.Bounds.Left > ViewRects.BandPanelActual.Right) {
						bounds.Width = 0;
						bi.Bounds = bounds;
						return;
					}
				}
			}
			bool hasChildren = bi.Band.HasChildren && bi.Band.Children.VisibleBandCount > 0;
			if(!hasChildren && bi.Band.AutoFillDown) {
				int rc = bi.Band.RowCount + rows;
				if(rc < BandRowCount) {
					rc = BandRowCount - rc;
					bounds.Height = BandRowHeight * (bi.Band.RowCount + rc);
					bi.Bounds = bounds;
				}
			}
			Painter.ElementsPainter.Band.CalcObjectBounds(bi);
			if(hasChildren) {
				var topLeft = new Point(bi.Bounds.Left, bi.Bounds.Bottom);
				if(IsRightToLeft) topLeft.X = bi.Bounds.Right;
				CalcChildrenBandsDrawInfo(bi, topLeft, rows + bi.Band.RowCount, posInfo.RootInfo == null ? posInfo : posInfo.RootInfo);
			} else {
				CalcBandColumnsDrawInfo(bi, posInfo.RootInfo == null ? posInfo : posInfo.RootInfo, posInfo);
			}
		}
		protected override void CalcDataRight() {
			int res = ViewRects.BandPanel.Right;
			GridBandInfoArgs lastBandInfo = BandsInfo.LastBandInfo;
			if(lastBandInfo != null && lastBandInfo.Bounds.Right < res)
				res = lastBandInfo.Bounds.Right;
			ViewRects.DataRectRight = res;
			ViewRects.DataRectRight = res;
		}
		protected virtual bool IsFirstBand(GridBand band) {
			while(band !=  null) {
				if(band.VisibleIndex != 0) return false;
				band = band.ParentBand;
			}
			return true;
		}
		protected virtual void CalcBandColumnsDrawInfo(GridBandInfoArgs bi, BandPositionInfo rootInfo, BandPositionInfo posInfo) {
			GridColumnInfoArgs ci;
			int left = bi.Bounds.Left, top = ViewRects.ColumnPanel.Top;
			int direction = IsRightToLeft ? -1 : 1;
			if(IsRightToLeft) left = bi.Bounds.Right;
			int childCount = 0;
			if(!bi.HasChildren && bi.Band != null && bi.Band.Columns.Count > 0) {
				int cIndex = 0;
				foreach(GridColumn column in bi.Band.Columns) {
					if(column.VisibleIndex < 0) continue;
					ci = CreateColumnInfo(column);
					ci.HtmlContext = View;
					if(IsFirstBand(bi.Band)) 
						ci.Info.CellIndex = cIndex ++;
					if(rootInfo.IsLeft && ci.Info.CellIndex == 0 && !View.OptionsView.ShowIndicator) ci.HeaderPosition = HeaderPositionKind.Left;
					ci.Tag = bi;
					if(IsRightToLeft)
						ci.Bounds = new Rectangle(left - (column == null ? 0 : column.VisibleWidth), top, column == null ? 0 : column.VisibleWidth, ColumnRowHeight);
					else
						ci.Bounds = new Rectangle(left, top, column == null ? 0 : column.VisibleWidth, ColumnRowHeight);
					childCount ++;
					CalcColumnInfo(ci, ref left);
					if(ci.Bounds.Width > 0) {
						bi.AddColumn(ci);
						ColumnsInfo.Add(ci);
					}
				}
				if(posInfo.IsRight && AllowUseRightHeaderKind && ColumnsInfo.Count > 0) {
					ColumnsInfo[ColumnsInfo.Count - 1].HeaderPosition = HeaderPositionKind.Right;
				}
			}
			if(childCount > 0) return;
			ci = CreateColumnInfo(null);
			ci.UseHtmlTextDraw = View.OptionsView.AllowHtmlDrawHeaders;
			if(bi.Band != null && IsFirstBand(bi.Band))
				ci.Info.CellIndex = 0;
			ci.Tag = bi;
			ci.Type = GridColumnInfoType.EmptyColumn;
			ci.Bounds = new Rectangle(left - (IsRightToLeft ? bi.Bounds.Width : 0), top, bi.Bounds.Width, ColumnRowHeight * ColumnPanelRowCount);
			childCount ++;
			CalcColumnInfo(ci, ref left);
			if(ci.Bounds.Width > 0) {
				bi.AddColumn(ci);
				ColumnsInfo.Add(ci);
			}
		}
		protected virtual int CalcRestBandsWidth(GridBand band) {
			if(band == null) return 0;
			int res = 0;
			GridBandCollection coll = View.Bands;
			if(band.ParentBand != null) {
				coll = band.ParentBand.Children;
				if(coll == null) return 0;
			}
			for(int n = 0; n < coll.Count; n++) {
				GridBand ban = coll[n];
				if(ban == band || band == null) {
					if(ban.Visible)
						res += ban.VisibleWidth;
					band = null;
				}
			}
			return res;
		}
		public override void UpdateFixedColumnInfo() {
			this.fFixedLeftColumn = this.fFixedRightColumn = null;
			this.fFixedLeftBand = this.fFixedRightBand = null;
			foreach(GridBand band in View.Bands) {
				if(!band.Visible) continue;
				if(band.Fixed == FixedStyle.Left) {
					this.fFixedLeftBand = band;
				}
				if(this.fFixedRightBand == null && band.Fixed == FixedStyle.Right) {
					this.fFixedRightBand = band;
				}
			}
		}
		public override GridHitInfo CalcHitInfo(Point pt) {
			BandedGridHitInfo hi = base.CalcHitInfo(pt) as BandedGridHitInfo;
			if(hi.HitTest != BandedGridHitTest.None) return hi;
			if(!IsReady || IsDataDirty) return hi;
			if(View.OptionsView.ShowBands && GridDrawing.PtInRect(ViewRects.BandPanel, pt)) {
				hi.HitTest = BandedGridHitTest.BandPanel;
				GridBandInfoArgs bi = CalcBandHitInfo(pt);
				if(bi == null || !bi.Bounds.Contains(pt)) return hi;
				switch(bi.Type) {
					case GridColumnInfoType.Indicator:
						hi.HitTest = BandedGridHitTest.BandButton;
						return hi;
					case GridColumnInfoType.EmptyColumn:
						return hi;
					case GridColumnInfoType.BehindColumn:
						return hi;
					default: 
						hi.Band = bi.Band;
						hi.HitTest = BandedGridHitTest.Band;
						bool leftEdge, rightEdge;
						leftEdge = IntInRange(pt.X, bi.Bounds.Left, bi.Bounds.Left + ControlUtils.ColumnResizeEdgeSize);
						rightEdge = IntInRange(pt.X, bi.Bounds.Right - ControlUtils.ColumnResizeEdgeSize, bi.Bounds.Right);
						if(leftEdge || rightEdge) {
							if(IsRightToLeft) {
								if(rightEdge && bi.Band != null && bi.Band.Collection.FirstVisibleBand == bi.Band) break;
								if(leftEdge && bi.Band != null && bi.Band.Fixed == FixedStyle.Right) break;
								hi.HitTest = BandedGridHitTest.BandEdge;
								if(rightEdge && bi.Band != null) {
									GridBand band = bi.Band.Collection.GetVisibleBand(bi.Band.VisibleIndex - 1);
									if(band != null) {
										hi.Band = band;
									}
								}
							}
							else {
								if(leftEdge) {
									if(bi.Band != null && bi.Band.Collection.FirstVisibleBand == bi.Band) break;
									if(bi.Band != null && bi.Band.Fixed == FixedStyle.Right) break;
								}
								hi.HitTest = BandedGridHitTest.BandEdge;
								if(leftEdge && bi.Band != null) {
									GridBand band = bi.Band.Collection.GetVisibleBand(bi.Band.VisibleIndex - 1);
									if(band != null) {
										hi.Band = band;
									}
								}
							}
						}
						break;
				}
				return hi;
			}
			return hi;
		}
		protected override void CalcRowHitInfo(Point pt, GridRowInfo ri, GridHitInfo hi) {
			base.CalcRowHitInfo(pt, ri, hi);
			BandedGridHitInfo hit = hi as BandedGridHitInfo;
			if(hit.InRowCell) {
				if(hit.Column != null) hit.Band = hit.Column.OwnerBand;
			}
		}
		protected virtual GridBandInfoArgs CalcBandHitInfo(GridBandInfoCollection bands, Point pt, bool onlyFixed) {
			int bandDelta = View.OptionsView.ShowBands ? 0 : -ViewRects.BandPanel.Height;
			foreach(GridBandInfoArgs bi in bands) {
				if(onlyFixed && bi.Fixed == FixedStyle.None) continue;
				if(!bi.Bounds.IsEmpty && IntInRange(pt.X, bi.Bounds.Left, bi.Bounds.Right)) {
					Rectangle bandBounds = bi.Bounds;
					bandBounds.Offset(0, bandDelta);
					if(bandBounds.Contains(pt)) return bi;
					if(bi.Band != null && bi.HasChildren && pt.Y > (bi.Bounds.Bottom + bandDelta)) {
						return CalcBandHitInfo(bi.Children, pt, onlyFixed);
					}
					return bi;
				}
			}
			return null;
		}
		public virtual GridBandInfoArgs CalcBandHitInfo(Point pt) {
			if(HasFixedLeft || HasFixedRight) {
				GridBandInfoArgs bi = CalcBandHitInfo(BandsInfo, pt, true);
				if(bi != null) return bi;
			}
			return CalcBandHitInfo(BandsInfo, pt, false);
		}
	}
	public class BandedGridViewRects : GridViewRects {
		Rectangle bandPanel;
		public int BandTotalWidth;
		public BandedGridViewRects(BandedGridViewInfo viewInfo) : base(viewInfo) {
			Clear();
		}
		public override void Clear() {
			this.bandPanel = Rectangle.Empty;
			this.BandTotalWidth = 0;
			base.Clear();
		}
		public new BandedGridView View { get { return base.View as BandedGridView; } }
		public Rectangle BandPanel { get { return bandPanel; } set { bandPanel = value; } }
		public Rectangle BandPanelActual {
			get {
				Rectangle res = BandPanel;
				if(ViewInfo.IsRightToLeft) {
					res.X = Scroll.Left;
					res.Width = BandPanel.Right - res.X;
				}
				else {
					res.Width = Scroll.Right - res.X;
				}
				return res;
			}
		}
		public int BandPanelWidth { 
			get { 
				if(BandPanel.Width > 0) return BandPanel.Width - IndicatorWidth; 
				int res = 0;
				foreach(GridBand band in View.Bands) {
					if(band.Visible) res += band.VisibleWidth;
				}
				return res;
			}
		}
		public override void AssignTo(GridViewRects vr) {
			BandedGridViewRects bvr = vr as BandedGridViewRects;
			if(bvr != null) {
				bvr.BandPanel = this.BandPanel;
				bvr.BandTotalWidth = this.BandTotalWidth;
			}
			base.AssignTo(vr);
		}
	}
	public class GridBandInfoCollection : CollectionBase {
		public GridBandInfoCollection() { }
		public virtual void Add(GridBandInfoArgs ci) { 
			List.Add(ci); 
		}
		public GridBandInfoArgs this[GridBand band] {
			get {
				if(band == null) return null;
				for(int n = 0; n < Count; n++) {
					GridBandInfoArgs bi = this[n];
					if(bi.Band == band) return bi;
				}
				return null;
			}
		}
		public GridBandInfoArgs LastBandInfo { 
			get {
				for(int n = Count - 1; n > -1; n--) {
					GridBandInfoArgs bi = this[n];
					if(bi.Type == GridColumnInfoType.Column)
						return bi;
				}
				return null;
			}
		}
		public GridBandInfoArgs this[int index] { get { return List[index] as GridBandInfoArgs; } }
		public int BandCount {
			get {
				int count = 0;
				foreach(GridBandInfoArgs bi in this) {
					if(bi.Type == GridColumnInfoType.Column)
						count++;
				}
				return count;
			}
		}
		public virtual int IndexOf(GridBand band) { return List.IndexOf(band); }
		public GridBandInfoArgs FindBand(GridBand band) {
			if(band == null) return null;
			GridBandInfoArgs res = this[band];
			if(res != null) return res;
			foreach(GridBandInfoArgs info in this) {
				if(info.HasChildren) {
					res = info.Children.FindBand(band);
					if(res != null) return res;
				}
			}
			return null;
		}
	}
}
