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
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraVerticalGrid.Utils;
using DevExpress.XtraVerticalGrid.Painters;
using DevExpress.XtraPrinting;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraVerticalGrid.Data;
using DevExpress.XtraVerticalGrid.Internal;
using DevExpress.XtraEditors;
namespace DevExpress.XtraVerticalGrid.ViewInfo {
	public abstract class BaseViewInfo : IDisposable {
		VGridControlBase grid;
		VGridResourceCache rc;
		ViewRects viewRects;
		bool isReady, paintAppearanceDirty;
		RowViewInfoReadOnlyCollection rowsViewInfo;
		RepositoryItemDictionary repositoryItemCache = new RepositoryItemDictionary();
		public ArrayList FootersInfo;
		internal Lines LinesInfo;
		internal Lines FocusLinesInfo;
		internal Lines NotFixedFocusedLinesInfo;
		bool isPrinting;
		protected PaintStyleCalcHelper fCalcHelper;
		VGridAppearanceCollection paintAppearance;
		protected internal const int invalid_position = -100000;
		protected BaseViewInfo(VGridControlBase grid, bool isPrinting) {
			this.grid = grid;
			this.rc = CreateResourceCache();
			this.viewRects = CreateViewRects();
			this.paintAppearance = Grid.CreateAppearance();
			this.isReady = false;
			this.paintAppearanceDirty = true;
			IsPrinting = isPrinting;
			UpdateCalcHelper();
			Clear();
		}
		public virtual int CompleteVisibleValuesCount { get { return VisibleValuesCount; } }
		public RowViewInfoReadOnlyCollection RowsViewInfo { get { return rowsViewInfo; } }
		public int FullRowHeaderWidth {
			get {
				if(!Grid.OptionsView.ShowRows)
					return 0;
				return HeaderViewWidth + RC.separatorWidth;
			}
		}
		public virtual int FullRecordValueWidth { get { return (Grid.RecordCount != 0 ? RC.VertLineWidth + ValueViewWidth : 0); } }
		public virtual int ValuesWidth { get { return ViewRects.Client.Width - FullRowHeaderWidth - RC.VertLineWidth; } }
		protected internal bool IsPrinting { get { return isPrinting; } private set { isPrinting = value; } }
		protected internal RepositoryItemDictionary RepositoryItemCache { get { return repositoryItemCache; } }
		protected virtual int OneBandWidth { get { return FullRowHeaderWidth + FullRecordValueWidth + RC.VertLineWidth; } }
		protected virtual ViewRects CreateViewRects() { return new ViewRects(this); }
		protected virtual VGridResourceCache CreateResourceCache() { return new VGridResourceCache(); }
		protected bool IsCategoryRowViewInfo(BaseRowViewInfo rowViewInfo) {
			if(rowViewInfo == null)
				return false;
			return Grid.IsCategoryRow(rowViewInfo.Row);
		}
		public abstract BasePrintInfo CreatePrintInfo();
		public virtual void Clear() {
			this.rowsViewInfo = new RowViewInfoReadOnlyCollection();
			this.FootersInfo = new ArrayList();
			this.LinesInfo = new Lines();
			this.FocusLinesInfo = new Lines();
			this.NotFixedFocusedLinesInfo = new Lines();
			this.ViewRects.Clear();
			ClearRepositoryItemCache();
		}
		void ClearRepositoryItemCache() {
			foreach(RepositoryItem item in this.repositoryItemCache.Values) {
				if(Grid.IsDefault(item)) {
					item.Dispose();
				}
			}
			this.repositoryItemCache = new RepositoryItemDictionary();
			Grid.OnClearViewInfo();
		}
		public virtual void Dispose() {
			RC.Dispose();
			Clear();
		}
		public BaseRowViewInfo this[BaseRow row] {
			get {
				if(row == null) return null;
				foreach(BaseRowViewInfo ri in RowsViewInfo) {
					if(ri.Row == row) return ri;
				}
				return null;
			}
		}
		public BaseRowViewInfo this[Point pt] {
			get {
				foreach(BaseRowViewInfo ri in RowsViewInfo) {
					if(ri.RowRect.Contains(pt)) return ri;
				}
				return null;
			}
		}
		internal BaseRowViewInfo FocusedRowViewInfo { get { return this[Grid.FocusedRow]; } }
		public void Calc() {
			UpdateCache();
			Clear();
			viewRects.Calc();
			CalcRowsViewInfo();
			CalcFootersInfo();
			IsReady = true;
		}
		public void UpdateCache() {
			if(Graphics == null) Graphics = Grid.CreateGraphics();
			RC.CheckUpdate(Grid);
		}
		public void UpdateCalcHelper() {
			fCalcHelper = null;
		}
		internal VGridHitTest CalcHitTest(Point pt) {
			VGridHitTest ht = new VGridHitTest();
			ht.PtMouse = pt;
			if(Grid.CustomizationForm != null) {
				if(Grid.CustomizationForm.Bounds.Contains(Grid.PointToScreen(pt))) {
					if(Grid.CustomizationForm.ListBoxContains(pt)) {
						ht.HitInfoType = HitInfoTypeEnum.CustomizationForm;
						ht.CustomizationHeaderInfo = Grid.CustomizationForm.GetRowInfoByGridPoint(pt);
						return ht;
					}
					return ht;
				}
			}
			if(!ViewRects.Window.Contains(pt)) {
				if(pt.X > ViewRects.Client.Left && pt.X < ViewRects.Client.Right) {
					if(pt.Y < ViewRects.Client.Top) ht.HitInfoType = HitInfoTypeEnum.OutTopSide;
					else ht.HitInfoType = HitInfoTypeEnum.OutBottomSide;
				}
				if(pt.Y > ViewRects.Client.Top && pt.Y < ViewRects.Client.Bottom) {
					if(pt.X < ViewRects.Client.Left) ht.HitInfoType = HitInfoTypeEnum.OutLeftSide;
					else ht.HitInfoType = HitInfoTypeEnum.OutRightSide;
				}
				return ht;
			}
			HitInfoTypeEnum scrollType = Scroller.GetHitTest(pt);
			if (scrollType != HitInfoTypeEnum.None) {
				ht.HitInfoType = scrollType;
				return ht;
			}
			if(!ViewRects.Client.Contains(pt)) {
				if(ht.HitInfoType == HitInfoTypeEnum.None)
					ht.HitInfoType = HitInfoTypeEnum.Border;
				return ht;
			}
			return CalcClientHitTestCore(pt);
		}
		protected internal virtual bool ShouldDrawRows() {
			return RowsViewInfo.Count > 0;
		}
		protected virtual VGridHitTest CalcClientHitTestCore(Point pt) {
			VGridHitTest ht = new VGridHitTest();
			ht.PtMouse = pt;
			if(CanResizeHeaderPanel) {
				if(ViewRects.BandRects.Count > 0) {
					Rectangle r = (Rectangle)ViewRects.BandRects[0];
					int x = GetHeader_ValuesSeparatorPosition(r);
					if(x != invalid_position) {
						Rectangle sepRect = new Rectangle(x - 2, r.Top, 4, r.Height);
						if(sepRect.Contains(pt)) {
							ht.RowInfo = this[pt];
							if(!IsCategoryRowViewInfo(ht.RowInfo)) {
								ht.HitInfoType = HitInfoTypeEnum.HeaderSeparator;
								return ht;
							} else { }
						}
					}
				}
			}
			foreach(Rectangle r in ViewRects.EmptyRects) {
				if(r.Contains(pt)) {
					ht.HitInfoType = HitInfoTypeEnum.Empty;
					return ht;
				}
			}
			foreach(BaseRowViewInfo ri in RowsViewInfo) {
				VGridHitTest row_ht = ri.CalcHitTest(pt, this);
				if(row_ht.RowInfo == ri) {
					ht = row_ht;
				}
			}
			return ht;
		}
		SizeF DefaultClientPageSize = new SizeF(624, 864);
		protected virtual Rectangle PrintAreaRect {
			get {
				BrickGraphics brickGraphics = Grid.curentPGraph as BrickGraphics;
				SizeF size = brickGraphics == null ? DefaultClientPageSize : brickGraphics.ClientPageSize;
				return new Rectangle(0, 0, (int)size.Width, int.MaxValue);
			}
		}
		internal Rectangle GetClientWithoutScrollBars() {
			return IsPrinting ? PrintAreaRect : GetScrollRect(Grid.ClientRectangle);
		}
		internal Rectangle GetActualArea() {
			return IsPrinting ? PrintAreaRect : Grid.ClientRectangle;
		}
		internal Rectangle GetActualClient() {
			return IsPrinting ? GetActualArea() : GetClientRect(GetActualArea());
		}
		int GetRowIndentWidth() {
			return RC.treeButSize.Width + 2 * BaseRowViewInfo.HorizontalOffset;
		}
		public virtual int RowIndentWidth {
			get {
				return Grid.OptionsView.LevelIndent == VGridOptionsView.DefaultLevelIndent ? GetRowIndentWidth() : Grid.OptionsView.LevelIndent;
			}
		}
		internal protected virtual void CalcBandRects() {
			CalcBandWidth();
			CreateBand(int.MaxValue, ViewRects.BandWidth);
		}
		GridRowReadOnlyCollection GetNotFixedVisibleRows() {
			GridRowReadOnlyCollection list = new GridRowReadOnlyCollection();
			for(int i = Scroller.TopVisibleRowIndex; i < Grid.NotFixedRows.Count; i++) {
				list.Add(Grid.NotFixedRows[i]);
			}
			return list;
		}
		internal protected abstract void AddEmptyRects();
		protected abstract Rectangle GetValueRectCore(int valueIndex, BaseRowViewInfo ri);
		public virtual int GetRecordWidth(int rowWidth, int headerWidth) { return 0; }
		protected virtual void CalcBandWidth() {
			ViewRects.BandWidth = OneBandWidth;
		}
		protected virtual void CreateBand(int bandHeight, int bandWidth) {
			int left = (ViewRects.BandRects.Count == 0 ? ViewRects.Client.Left : 
				((Rectangle)ViewRects.BandRects[ViewRects.BandRects.Count - 1]).Right);
			Rectangle br = new Rectangle(left, ViewRects.Client.Top, bandWidth, Math.Min(bandHeight, ViewRects.Client.Height));
			ViewRects.BandRects.Add(IsRightToLeft ? ConvertBoundsToRTL(br, ViewRects.Client) : br);
		}
		int FixedLineWidth { get { return Grid.OptionsView.FixedLineWidth; } }
		int FixedTopLineWidth {
			get {
				if(Grid.FixedTopRows.Count > 0 && Grid.VisibleRows.Count != Grid.FixedTopRows.Count)
					return FixedLineWidth;
				return 0;
			}
		}
		int TruncatedFixedBottomLineWidth {
			get {
				if(ViewRects.FixedBottom.Top - FixedBottomLineWidth < ViewRects.FixedTop.Bottom) {
					return ViewRects.FixedBottom.Top - ViewRects.FixedTop.Bottom;
				}
				return FixedBottomLineWidth;
			}
		}
		int FixedBottomLineWidth {
			get {
				if(Grid.FixedBottomRows.Count > 0 && Grid.VisibleRows.Count != Grid.FixedBottomRows.Count)
					return FixedLineWidth;
				return 0;
			}
		}
		public void CalcRowsViewInfo() {
			int i = TopVisibleRowIndex;
			for (int bandIndex = 0; bandIndex < Scroller.BandsInfo.Count; bandIndex++) {
				Rectangle bandRectangle = ViewRects.BandRects[bandIndex];
				if(bandRectangle.Left > ViewRects.Client.Right)
					return;
				BandInfo bi = (BandInfo)Scroller.BandsInfo[bandIndex];
				Rectangle availableRectangle = new Rectangle(bandRectangle.X, bandRectangle.Y, bandRectangle.Width, ViewRects.Client.Height - RC.HorzLineWidth);
				ViewRects.FixedTop = CalcFixedTopRowsViewInfo(Grid.FixedTopRows, availableRectangle, bandIndex, bi.rowsCount);
				Rectangle afterTopRectangle = availableRectangle;
				if(ViewRects.FixedTop.Height > 0) {
					afterTopRectangle = new Rectangle(availableRectangle.X, ViewRects.FixedTop.Bottom + FixedTopLineWidth, availableRectangle.Width, availableRectangle.Height - ViewRects.FixedTop.Height - FixedTopLineWidth);
				}
				Rectangle bottomAvailableRectangle = FillFromBottom(availableRectangle) ? availableRectangle : afterTopRectangle;
				bottomAvailableRectangle.Intersect(afterTopRectangle);
				bottomAvailableRectangle.Height -= RC.HorzLineWidth;
				ViewRects.FixedBottom = CalcFixedBottomRowsViewInfo(Grid.FixedBottomRows, bottomAvailableRectangle, bandIndex, bi.rowsCount);
				Rectangle scrollableRectangle = afterTopRectangle;
				if(ViewRects.FixedBottom.Height > 0) {
					scrollableRectangle = new Rectangle(afterTopRectangle.X, GetFixedTopWithBounds().Bottom, afterTopRectangle.Width, afterTopRectangle.Height - GetFixedBottomWithBounds().Height);
				}
				ViewRects.ScrollableRectangle = new Rectangle(ViewRects.Client.X, scrollableRectangle.Y, ViewRects.Client.Width, scrollableRectangle.Height);
				CalcBandRowsViewInfo(ref i, scrollableRectangle, bandIndex, bi.rowsCount);
				AddBandTopLine(ViewRects.BandRects[bandIndex]);
				AddBandLeftLine(ViewRects.BandRects[bandIndex]);
				AddBandBottomLine(ViewRects.BandRects[bandIndex]);
			}
			AddFixedBottomLines();
			AddFixedTopLines();
			RowsViewInfo.Sort();
		}
		public Rectangle GetFixedTopWithBounds() {
			return new Rectangle(ViewRects.FixedTop.X, ViewRects.FixedTop.Y - RC.HorzLineWidth, ViewRects.FixedTop.Width, RC.HorzLineWidth + ViewRects.FixedTop.Height + FixedTopLineWidth);
		}
		public Rectangle GetFixedBottomWithBounds() {
			return Rectangle.Intersect(new Rectangle(ViewRects.FixedBottom.X, ViewRects.FixedBottom.Y - TruncatedFixedBottomLineWidth, ViewRects.FixedBottom.Width, TruncatedFixedBottomLineWidth + ViewRects.FixedBottom.Height + RC.HorzLineWidth), ViewRects.Client);
		}
		bool FillFromBottom(Rectangle rectangle) {
			return true;
		}
		void AddFixedTopLines() {
			if(ViewRects.FixedTop.Height <= 0)
				return;
			int x = IsRightToLeft ? ViewRects.FixedTop.Right - RC.VertLineWidth : ViewRects.FixedTop.X;
			LinesInfo.AddLine(new LineInfo(x, ViewRects.FixedTop.Top, RC.VertLineWidth, ViewRects.FixedTop.Height, RC.BandBorderBrush));
			LinesInfo.AddLine(new LineInfo(ViewRects.FixedTop.X, ViewRects.FixedTop.Bottom, ViewRects.FixedTop.Width, FixedTopLineWidth, RC.FixedLineBrush));
		}
		void AddFixedBottomLines() {
			if(ViewRects.FixedBottom.Height <= 0)
				return;
			if(TruncatedFixedBottomLineWidth != 0) {
				LinesInfo.AddLine(new LineInfo(ViewRects.FixedBottom.X, GetFixedBottomWithBounds().Top, ViewRects.FixedBottom.Width, TruncatedFixedBottomLineWidth, RC.FixedLineBrush));
			}
			int x = IsRightToLeft ? ViewRects.FixedBottom.Right - RC.VertLineWidth :  ViewRects.FixedBottom.X;
			LinesInfo.AddLine(new LineInfo(x, ViewRects.FixedBottom.Top, RC.VertLineWidth, ViewRects.FixedBottom.Height, RC.BandBorderBrush));
		}
		protected internal virtual BaseRow GetTopScrollableRow() {
			return Grid.NotFixedRows[Scroller.TopVisibleRowIndex];
		}
		protected internal virtual BaseRow GetBottomScrollableRow() {
			return Grid.NotFixedRows[Grid.NotFixedRows.Count - 1];
		}
		Rectangle CalcFixedBottomRowsViewInfo(GridRowReadOnlyCollection fixedBottom, Rectangle bandRect, int bandIndex, int bandRowCount) {
			if(fixedBottom.Count == 0)
				return new Rectangle(bandRect.X, bandRect.Y, bandRect.Width, 0);
			int bottom = bandRect.Bottom;
			for(int i = fixedBottom.Count - 1; 0 <= i; i--) {
				BaseRow row = fixedBottom[i];
				BaseRow nextRow = fixedBottom[i + 1];
				if(row == null)
					break;
				int rowHeight = GetVisibleRowHeight(row);
				Rectangle rowRect = new Rectangle(bandRect.Left + RC.VertLineWidth, bottom - rowHeight, bandRect.Width - 2 * RC.VertLineWidth, rowHeight);
				if(!CanAddRowToBandBottom(rowRect, bandRect, -1))
					break;
				BaseRowViewInfo ri = row.CreateViewInfo();
				ri.BandIndex = bandIndex;
				ri.BandRowIndex = -1;
				ri.Calc(rowRect, this, nextRow);
				RowsViewInfo.Add(ri);
				bottom -= rowHeight + RC.HorzLineWidth;
			}
			if(fixedBottom.Count > 0) {
				bottom += RC.HorzLineWidth;
			}
			return new Rectangle(bandRect.X, bottom, bandRect.Width, bandRect.Bottom - bottom);
		}
		Rectangle CalcFixedTopRowsViewInfo(GridRowReadOnlyCollection fixedTop, Rectangle bandRect, int bandIndex, int bandRowCount) {
			if(fixedTop.Count == 0)
				return new Rectangle(bandRect.X, bandRect.Y, bandRect.Width, 0);
			int top = bandRect.Top;
			for(int i = 0; i < fixedTop.Count; i++) {
				BaseRow row = fixedTop[i];
				BaseRow nextRow = fixedTop[i + 1];
				if(nextRow == null) {
					nextRow = GetTopScrollableRow();
				}
				if(row == null)
					break;
				Rectangle rowRect = new Rectangle(bandRect.Left + RC.VertLineWidth, top, bandRect.Width - 2 * RC.VertLineWidth, GetVisibleRowHeight(row));
				if(!CanAddRowToBand(rowRect, bandRect, -1))
					break;
				BaseRowViewInfo ri = row.CreateViewInfo();
				ri.BandIndex = bandIndex;
				ri.BandRowIndex = -1;
				ri.Calc(rowRect, this, nextRow);
				RowsViewInfo.Add(ri);
				top += GetVisibleRowHeight(row) + RC.HorzLineWidth;
			}
			if(fixedTop.Count > 0) {
				top -= RC.HorzLineWidth;
			}
			return new Rectangle(bandRect.X, bandRect.Y, bandRect.Width, top - bandRect.Y);
		}
		void CalcBandRowsViewInfo(ref int firstRowIndex, Rectangle br, int bandIndex, int bandRowCount) {
			if(br.Height <= 0)
				return;
			int bandRowIndex = 0;
			int top = br.Top - (bandIndex == 0 ? Scroller.GetTopVisibleRowPixelOffset() : 0);
			BaseRow row = Grid.NotFixedRows[firstRowIndex];
			BaseRow nextRow = (bandRowIndex == bandRowCount - 1 ? null : Grid.NotFixedRows[firstRowIndex + 1]);
			do {
				if(row == null)
					break;
				top += RC.HorzLineWidth;
				Rectangle rowRect = new Rectangle(br.Left + RC.VertLineWidth, top, br.Width - 2 * RC.VertLineWidth, GetVisibleRowHeight(row));
				if(!CanAddRowToBand(rowRect, br, bandRowIndex))
					break;
				BaseRowViewInfo ri = row.CreateViewInfo();
				ri.BandIndex = bandIndex;
				ri.BandRowIndex = bandRowIndex;
				ri.Calc(rowRect, this, nextRow);
				RowsViewInfo.Add(ri);
				top += GetVisibleRowHeight(row);
				row = nextRow;
				firstRowIndex++;
				bandRowIndex++;
				nextRow = (bandRowIndex == bandRowCount - 1 ? null : Grid.NotFixedRows[firstRowIndex + 1]);
			} while(bandRowIndex < bandRowCount);
			if (0 < bandRowIndex) {
				top += RC.HorzLineWidth;
			}
			br.Height += top - br.Bottom;
			ViewRects.BandRects[bandIndex] = br;
			if (br.Height < ViewRects.Client.Height)
				ViewRects.EmptyRects.Add(new Rectangle(br.Left, br.Bottom, ViewRects.Client.Width, ViewRects.Client.Height - br.Height));
		}
		protected virtual bool CanAddRowToBand(Rectangle rowRect, Rectangle bandRect, int bandRowIndex) {
			return (rowRect.Top < bandRect.Bottom) || IsPrinting;
		}
		protected virtual bool CanAddRowToBandBottom(Rectangle rowRectangle, Rectangle rectangle, int bandRowIndex) {
			return (rectangle.Top <= rowRectangle.Bottom) || IsPrinting;
		}
		protected virtual void AddBandTopLine(Rectangle br) {
			if(RC.HorzLineWidth == 0)
				return;
			LinesInfo.AddLine(new LineInfo(br.Left, br.Top, br.Width, RC.HorzLineWidth, RC.BandBorderBrush));
		}
		protected virtual void AddBandBottomLine(Rectangle br) {
			if(RC.HorzLineWidth == 0) return;
			if(ViewRects.FixedBottom.Height > 0 && br.Bottom - RC.HorzLineWidth + RC.HorzLineWidth > ViewRects.FixedBottom.Y) return;
			LinesInfo.AddLine(new LineInfo(br.Left, br.Bottom - RC.HorzLineWidth, br.Width, RC.HorzLineWidth, RC.BandBorderBrush));
		}
		protected virtual void AddBandLeftLine(Rectangle br) {
			if(br.Height <= 0) return;
			LinesInfo.AddLine(new LineInfo(IsRightToLeft ? br.Right - RC.VertLineWidth : br.Left, br.Top, RC.VertLineWidth, br.Height, RC.BandBorderBrush));
		}
		public void CalcFootersInfo() {
			FootersInfo = new ArrayList();
		}
		internal void AddBottomValueSide(Rectangle r, Brush horzLineBrush, BaseRow row) {
			if(row.Fixed == FixedStyle.None && ViewRects.FixedBottom.Height > 0 && GetFixedBottomWithBounds().Top < r.Bottom) {
				this.NotFixedFocusedLinesInfo.AddLine(new LineInfo(r.Left, r.Bottom, r.Width, RC.HorzLineWidth, horzLineBrush));
				return;
			}
			if(RC.HorzLineWidth != 0)
				LinesInfo.AddLine(new LineInfo(r.Left, r.Bottom, r.Width, RC.HorzLineWidth, horzLineBrush));
		}
		protected internal virtual void AddVertValueSide(Rectangle r, bool first, bool last, Brush vertLineBrush, BaseRow row) {
			if(RC.VertLineWidth == 0)
				return;
			int height = r.Height + RC.HorzLineWidth;
			int x = IsRightToLeft ? r.Left - RC.VertLineWidth : r.Right;
			r = RectUtils.IncreaseFromLeft(r, RC.VertLineWidth);
			if(NeedTruncateHeightByFixedBottom(r, height, row)) {
				this.NotFixedFocusedLinesInfo.AddLine(new LineInfo(x, r.Top, RC.VertLineWidth, height, last ? RC.BandBorderBrush : vertLineBrush));
			}
			if(!ShouldAddVertValueSide(row)) return;
			LinesInfo.AddLine(new LineInfo(x, r.Top, RC.VertLineWidth, TruncateHeightByFixedBottom(r, height, row), last ? RC.BandBorderBrush : vertLineBrush));
		}
		protected virtual bool ShouldAddVertValueSide(BaseRow row) { return true; }
		protected bool NeedTruncateHeightByFixedBottom(Rectangle rowRectangle, int height, BaseRow row) {
			return row.Fixed == FixedStyle.None && ViewRects.FixedBottom.Height > 0 && GetFixedBottomWithBounds().Top < rowRectangle.Top + height;
		}
		protected int TruncateHeightByFixedBottom(Rectangle rowRectangle, int height, BaseRow row) {
			if(NeedTruncateHeightByFixedBottom(rowRectangle, height, row))
				return GetFixedBottomWithBounds().Top - rowRectangle.Top;
			return height;
		}
		bool RowIsEditing(BaseRow row) {
			return Grid.ActiveEditor != null && Grid.FocusedRow == row;
		}
		protected Rectangle[] ScaleRowRects(Rectangle rowRect) {
			int sepWidth = (Grid.OptionsView.ShowVertLines && ValueWidth != 0) ? RC.separatorWidth : 0;
			RectScaler rs = new RectScaler();
			rs.Items.Add(RowHeaderWidth, Grid.RowHeaderMinWidth, sepWidth);
			rs.Items.Add(ValueWidth, Grid.BandMinWidth);
			return rs.ScaleRect(rowRect);
		}
		public Rectangle GetClientRect(Rectangle window) {
			Rectangle clRect = GetScrollRect(window);
			if(!Scroller.ScrollInfo.IsOverlapScrollbar) {
				if(Scroller.IsNeededHScrollBar)
					clRect.Height = Math.Max(0, clRect.Height - Scroller.ScrollInfo.HScrollHeight);
				if(Scroller.IsNeededVScrollBar) {
					int width = Math.Max(0, clRect.Width - Scroller.ScrollInfo.VScrollWidth);
					if(IsRightToLeft)
						clRect.X = Math.Max(0, clRect.Right - width);
					clRect.Width = width;
				}
			}
			ViewRects.FindPanel = CalcFindPanelRect(clRect);
			if (!ViewRects.FindPanel.IsEmpty) {
				clRect.Y = ViewRects.FindPanel.Bottom;
				clRect.Height -= ViewRects.FindPanel.Height;
			}
			return clRect;
		}
		protected virtual Rectangle CalcFindPanelRect(Rectangle client) {
			Rectangle rect = Rectangle.Empty;
			if (Grid.FindPanelVisible && Grid.FindPanel != null) {
				rect = client;
				rect.Height = Grid.FindPanel.Height;
				Grid.FindPanel.Bounds = rect;
				Grid.FindPanel.Visible = true;
			}
			return rect;
		}
		internal Rectangle GetScrollRect() {
			return GetScrollRect(ViewRects.Client);
		}
		Rectangle GetScrollRect(Rectangle windowRect) {
			return PaintStyle.BorderPainter.GetObjectClientRectangle(GetBorderObjectArgs(windowRect, null));
		}
		protected internal virtual BorderObjectInfoArgs GetBorderObjectArgs(Rectangle bounds, GraphicsCache cache) {
			return new BorderObjectInfoArgs(cache, bounds, PaintAppearance.HorzLine, Grid.BorderStyle == BorderStyles.UltraFlat ? ObjectState.Disabled : ObjectState.Normal);
		}
		protected virtual void CalcRowRectsCore(BaseRowViewInfo ri) {
			if(CheckNotToShowRows(ri))
				return;
			Rectangle[] rects = ScaleRowRects(ri.RowRect);
			ri.HeaderInfo.HeaderRect = rects[0];
			ri.ValuesRect = rects[1];
		}
		protected bool CheckNotToShowRows(BaseRowViewInfo ri) {
			if(Grid.OptionsView.ShowRows)
				return false;
			ri.ValuesRect = ri.RowRect;
			ri.HeaderInfo.HeaderRect = new Rectangle(ri.RowRect.X, ri.RowRect.Y, 0, ri.RowRect.Height);
			return true;
		}
		protected virtual int GetHeader_ValuesSeparatorPosition(Rectangle bandRect) {
			if(!Grid.OptionsView.ShowRows) return invalid_position;
			Rectangle bandRectArea = Rectangle.Inflate(bandRect, -RC.VertLineWidth, -RC.HorzLineWidth);
			Rectangle[] rects = ScaleRowRects(bandRectArea);
			return rects[0].Right + RC.VertLineWidth;
		}
		internal DevExpress.XtraEditors.Drawing.BaseEditPainter GetPainter(DevExpress.XtraEditors.Repository.RepositoryItem item) {
			return Grid.ContainerHelper.GetPainter(item);
		}
		static internal Rectangle GetInrect(Rectangle bounds, int size) {
			return GetInrect(bounds, new Size(size, size));
		}
		static internal Rectangle GetInrect(Rectangle bounds, Size size) {
			if(bounds.Width < size.Width || bounds.Height < size.Height) return Rectangle.Empty;
			Point location = new Point(bounds.Left + (bounds.Width - size.Width) / 2, bounds.Top + (bounds.Height - size.Height) / 2);
			return new Rectangle(location, size);
		}
		internal Rectangle GetVisibleRowRect(Rectangle rowRect) { 
			return GetVisibleRowRectCore(rowRect); 
		}
		public virtual Rectangle RestoreRowRect(Rectangle rowRect) { return rowRect; }
		public virtual int GetVisibleRowHeight(BaseRow row) {
			if(row.Height != -1)
				return row.Height;
			int? height = Grid.AutoHeights[row];
			if(height == null) {
				height = CalcRowAutoHeight(row);
				Grid.AutoHeights[row] = height;
			}
			return (int)height;
		}
		public void SetAppearanceDirty() { this.paintAppearanceDirty = true; }
		protected virtual int CalcRowAutoHeight(BaseRow row) {
			return Math.Max(GetMinCaptionAutoHeight(row), GetMinValueAutoHeight(row));
		}
		protected virtual int GetMinCaptionAutoHeight(BaseRow row) {
			int height = Grid.OptionsView.MinRowAutoHeight;
			IRowViewScaler rowScaler = ((IServiceProvider)row).GetService(typeof(IRowViewScaler)) as IRowViewScaler;
			if(rowScaler == null) return height;
			int indent = GetCommonCaptionIndentWidth(row);
			for(int i = 0; i < row.RowPropertiesCount; i++) {
				RowProperties p = row.GetRowProperties(i);
				if(p.Bindable && !Grid.OptionsView.ShowRows) return 0;
				if(!string.IsNullOrEmpty(p.Caption)) {
					AppearanceObject rowHeaderStyle = row.GetRowHeaderStyle(this.PaintAppearance);
					int headerWidth = Math.Min(ViewRects.Client.Width, row.GetHeaderViewWidth(HeaderViewWidth, ViewRects.BandWidth)) - indent - RC.HorzLineWidth;
					int cellViewRectWidth = rowScaler.GetCellViewRectWidth(p, headerWidth, false);
					Size textSize = CalcTextSize(rowHeaderStyle, Graphics, p.Caption, cellViewRectWidth);
					height = Math.Max(height, textSize.Height + GetRowVerticalOffset(row));
				}
			}
			if(row.MaxCaptionLineCount > 0) {
				int lineHeight = RC.CalcMinHeight(row.GetRowHeaderStyle(this.PaintAppearance));
				int reducedHeight = lineHeight * row.MaxCaptionLineCount + GetRowVerticalOffset(row);
				if (reducedHeight < height)
					height = reducedHeight;
			}
			return Math.Max(height, RC.GetExpandCollapseButtonHeight(row));
		}
		int GetRowVerticalOffset(BaseRow row) {
			return Grid.IsCategoryRow(row) ? 4 * BaseRowViewInfo.VerticalOffset : 2 * BaseRowViewInfo.VerticalOffset;
		}
		protected virtual int GetCommonCaptionIndentWidth(BaseRow row) {
			int indentWidth = (row.Level + 1) * RowIndentWidth;
			for(int i = 0; i < row.RowPropertiesCount; i++) {
				RowProperties p = row.GetRowProperties(i);
				indentWidth += BaseRowHeaderInfo.LeftCaptionOffset + row.HeaderInfo.RightCaptionTextIndent;
				if(p.ImageIndex != -1 && Grid.ImageList != null)
					indentWidth += ImageCollection.GetImageListSize(Grid.ImageList).Width;
			}
			indentWidth += BaseRowHeaderInfo.RightCaptionOffset;
			return indentWidth;
		}
		protected virtual int GetMinValueAutoHeight(BaseRow row) {
			int recordIndex = -1;
			if(this.Grid != null)
				if(this.Grid.LayoutStyle == LayoutViewStyle.BandsView || this.Grid.LayoutStyle == LayoutViewStyle.SingleRecordView)
					recordIndex = this.Grid.FocusedRecord;
			return GetMinValueAutoHeightCore(row, recordIndex);
		}
		const int MaxAutoHeightCount = 70;
		protected virtual int GetMinValueAutoHeightCore(BaseRow row, int recordIndex) {
			int height = Grid.OptionsView.MinRowAutoHeight;
			GraphicsCache cache = new GraphicsCache(Graphics);
			BaseRowViewInfo ri = row.CreateViewInfo();
			int top  = Grid.LeftVisibleRecord;
			if(Grid.RecordCount < MaxAutoHeightCount) top = 0;
			int count = Grid.RecordCount - top;
			count = Math.Min(count, MaxAutoHeightCount);
			for(int i = 0; i < row.RowPropertiesCount; i++) {
				RowProperties p = row.GetRowProperties(i);
				if(!p.Bindable) continue;
				int calcIndex = recordIndex;
				if(!IsRowAutoHeight(ri, p, recordIndex)) calcIndex = top;
				if(calcIndex == -1) {
					for(int j = top; j < count; j++) {
						height = Math.Max(height, GetRecordCellHeight(ri, p, cache, j));
					}
				}
				else
					height = Math.Max(height, GetRecordCellHeight(ri, p, cache, calcIndex));
			}
			cache.Dispose();
			return height;
		}
		protected bool IsRowAutoHeight(BaseRowViewInfo ri, RowProperties p, int recordIndex) {
			if(Grid != null && Grid.IsCustomRecordCellEditExists) return true;
			return Grid.CreateEditorViewInfo(p, recordIndex) is IHeightAdaptable;
		}
		int MaxRowAutoHeight {
			get {
				return Grid.OptionsView.MaxRowAutoHeight < Grid.OptionsView.MinRowAutoHeight ? int.MaxValue : Grid.OptionsView.MaxRowAutoHeight;
			}
		}
		protected internal int GetRecordCellHeight(BaseRowViewInfo rowInfo, RowProperties p,GraphicsCache cache, int recordIndex) {
			BaseEditViewInfo editorViewInfo = rowInfo.CalcRowValueInfo(p, new Rectangle(0, 0, ValueViewWidth, 0), this, recordIndex).EditorViewInfo;
			IHeightAdaptable ah = editorViewInfo as IHeightAdaptable;
			if (ah != null) {
				int someMagicNumberForMemoEdit = 5;
				return MathUtils.FitBounds(ah.CalcHeight(cache, Math.Max(0, editorViewInfo.Bounds.Width - someMagicNumberForMemoEdit)) + 2 * BaseRowViewInfo.VerticalOffset,
					Grid.OptionsView.MinRowAutoHeight,
					MaxRowAutoHeight);
			}
			else {
				return Math.Max(Grid.OptionsView.MinRowAutoHeight, editorViewInfo.CalcMinHeight(cache.Graphics) + 2 * BaseRowViewInfo.VerticalOffset);
			}
		}
		internal Rectangle ChangeFocusRow(BaseRow newFocus, BaseRow oldFocus) {
			if(Grid.GridDisposing) return Rectangle.Empty;
			LinesInfo.lockAddLines = true;
			Rectangle rInvalidate = Rectangle.Empty;
			try { rInvalidate = fCalcHelper.ChangeFocusRow(newFocus, oldFocus); }
			finally { LinesInfo.lockAddLines = false; }
			return rInvalidate;
		}
		protected internal virtual Rectangle UpdateRecord(int recordIndex) {
			if(Grid.IsUpdateLocked)
				return Rectangle.Empty;
			if(Grid.GridDisposing) return Rectangle.Empty;
			if(RowsViewInfo.Count == 0) return Rectangle.Empty;
			if(recordIndex < 0) return Rectangle.Empty;
			Rectangle rcInvalidate = new Rectangle(0, ViewRects.Client.Top, 0, 0);
			foreach(BaseRowViewInfo ri in RowsViewInfo) {
				for(int cellIndex = 0; cellIndex < ri.HeaderInfo.CaptionsInfo.Count; cellIndex++) {
					RowValueInfo rv = ri[recordIndex, cellIndex];
					if(rv != null) {
						if(cellIndex == 0) rcInvalidate.X = rv.Bounds.X;
						rcInvalidate.Width += rv.Bounds.Width;
						ri.UpdateCellData(rv);
					}
				}
				rcInvalidate.Height += ri.RowRect.Bottom - rcInvalidate.Top;
			}
			return rcInvalidate;
		}
		internal void UpdateCellData(BaseRow row, int recordIndex, int cellIndex) {
			BaseRowViewInfo rowInfo = this[row];
			if(rowInfo == null)
				return;
			RowValueInfo valueInfo = rowInfo[recordIndex, cellIndex];
			if(valueInfo == null)
				return;
			rowInfo.UpdateCellData(valueInfo);
		}
		internal Rectangle UpdateRow(BaseRow row) {
			if(Grid.GridDisposing) return Rectangle.Empty;
			BaseRowViewInfo ri = this[row];
			if(ri == null) return Rectangle.Empty;
			LinesInfo.lockAddLines = true;
			FocusLinesInfo.lockAddLines = true;
			try { 
				ri.Calc(RestoreRowRect(ri.RowRect), this, Grid.VisibleRows[ri.Row.VisibleIndex + 1]); 
			}
			finally {
				LinesInfo.lockAddLines = false;
				FocusLinesInfo.lockAddLines = false;
			}
			return ri.RowRect;
		}
		protected virtual void UpdatePaintAppearance() {
			if(!this.paintAppearanceDirty) return;
			this.paintAppearanceDirty = false;
			PaintAppearance.Reset();
			PaintAppearance.Combine(Grid.Appearance, PaintStyle.GetAppearanceDefaults(), true);
			UpdateAlphaBlending();
		}
		protected virtual void UpdateAlphaBlending() {
			if(!Grid.CanUpdatePaintAppearanceBlending) return;
			foreach(DictionaryEntry entry in Grid.Blending.AlphaStyles) {
				AppearanceObject app = PaintAppearance.GetAppearance(entry.Key.ToString());
				if(app == null || app.BackColor.IsEmpty) continue;
				int level = (int)entry.Value;
				if(level >= 255) continue;
				app.BackColor = Color.FromArgb(level, app.BackColor);
				if(app.BackColor2 != Color.Empty) app.BackColor2 = Color.FromArgb(level, app.BackColor2);
			}
		}
		public VGridAppearanceCollection PaintAppearance {
			get {
				UpdatePaintAppearance();
				return paintAppearance;
			}
		}
		protected virtual Rectangle GetVisibleRowRectCore(Rectangle rowRect) { return rowRect; }
		internal Rectangle GetValueRect(int valueIndex, BaseRowViewInfo ri) {
			return GetValueRectCore(valueIndex, ri);
		}
		internal void CalcRowRects(BaseRowViewInfo ri) {
			CalcRowRectsCore(ri);
		}
		public static Size CalcTextSize(AppearanceObject style, Graphics g, string text, int width) {
			SizeF sizeF = style.CalcTextSize(g, text, width);
			Size size = sizeF.ToSize();
			if(size.Width < sizeF.Width) size.Width++;
			if(size.Height < sizeF.Height) size.Height++;
			return size;
		}
		internal AppearanceObject FocusedRowStyle {
			get {
				if (!Grid.OptionsSelectionAndFocus.EnableAppearanceFocusedRow)
					return PaintAppearance.RowHeaderPanel;
				return Grid.HasFocus ? PaintAppearance.FocusedRow : PaintAppearance.HideSelectionRow;
			}
		}
		protected bool CanResizeRowHeaders { get { return Grid.OptionsBehavior.ResizeRowHeaders; } }
		protected bool CanResizeHeaderPanel { get { return Grid.OptionsBehavior.ResizeHeaderPanel; } }
		protected AutoHeightsStore AutoHeights { get { return Grid.AutoHeights; } }
		public virtual bool CanAddCategoryValuesSeparatorLines { get { return false; } }
		public virtual bool CanUseFocusedRecordStyle { get { return false; } }
		public virtual int VisibleValuesCount { get { return 1; } }
		public virtual float HorzScaleCoeff { 
			get {
				return (float)Grid.RowHeaderWidth / (float)HeaderViewWidth; 
			} 
		}
		public int VisibleRowCount { get { return RowsViewInfo.Count; } }
		public int RowHeaderWidth {
			get {
				if(!Grid.OptionsView.ShowRows)
					return 0;
				return Grid.RowHeaderWidth;
			}
		}
		public int ValueWidth {
			get {
				if(VisibleValuesCount == 0)
					return 0;
				return Grid.RecordWidth;
			}
		}
		public virtual int HeaderViewWidth { get { return Grid.RowHeaderWidth; } }
		public virtual int ValueViewWidth { get { return Grid.RecordWidth; } }
		public Graphics Graphics {
			get { return RC.Graphics; }
			set {
				if(Graphics != value) {
					if(Graphics != null) Graphics.Dispose();
					RC.Graphics = value;
				}
			}
		}
		public PaintStyleCalcHelper CalcHelper {
			get {
				if(fCalcHelper == null)
					fCalcHelper = PaintStyle.CreateCalcHelper(this);
				return fCalcHelper; 
			}
			set {
				if(value != null)
					fCalcHelper = value;
			}
		}
		protected VGridPaintStyle PaintStyle { get { return Grid.Painter.PaintStyle; } }
		internal protected int TopVisibleRowIndex { get { return IsPrinting ? 0 : Scroller.TopVisibleRowIndex; } }
		internal protected virtual int FirstVisibleRecordIndex { get { return Scroller.LeftVisibleRecord; } }
		public bool IsReady {
			get { return isReady; }
			set { isReady = value; }
		}
		public ViewRects ViewRects { get { return viewRects; } }
		public VGridResourceCache RC { get { return rc; } }
		public VGridControlBase Grid { get { return grid; } }
		public VGridScroller Scroller { get { return Grid.Scroller; } }
		public virtual bool IsActivateKey(Keys keys, BaseRow row, int recordIndex, int cellIndex) {
			RowValueInfo vi = GetRowValueInfo(row, recordIndex, cellIndex);
			if(vi == null || vi.Item == null)
				return false;
			return vi.Item.IsActivateKey(keys);
		}
		public RowValueInfo GetRowValueInfo(BaseRow row, int recordIndex, int cellIndex) {
			BaseRowViewInfo ri = this[row];
			if(ri == null)
				return null;
			return ri[recordIndex, cellIndex];
		}
		protected internal virtual int GetDrawnRecordIndex(int index) {
			return FirstVisibleRecordIndex + index;
		}
		protected internal virtual bool IsResizeable(BaseRow row) {
			BaseRowViewInfo rowViewInfo = this[row];
			if(rowViewInfo == null)
				return false;
			return rowViewInfo.RowRect.Bottom <= ViewRects.Client.Bottom;
		}
		protected internal bool GetAllowGlyphSkinning(BaseRow row) {
			EditorRow eRow = row as EditorRow;
			return Grid.OptionsView.AllowGlyphSkinning && (eRow == null || eRow.Enabled);
		}
		protected internal virtual bool GetAllowHtmlRowValueText(BaseRow row, bool enabled) {
			if(row == null || enabled) return false;
			return GetAllowHtmlRowHeaderText(row);
		}
		protected internal virtual bool GetAllowHtmlRowHeaderText(BaseRow row) {
			switch(row.OptionsRow.AllowHtmlText) {
				case DefaultBoolean.Default: return Grid.OptionsView.AllowHtmlText;
				case DefaultBoolean.True: return true;
				case DefaultBoolean.False: return false;
			}
			return false;
		}
		public virtual Rectangle GetVisibleValuesRect() {
			int x = ViewRects.Client.X + FullRowHeaderWidth + 2 * RC.VertLineWidth;
			Rectangle rect = new Rectangle(x, ViewRects.Client.Y, ViewRects.Client.Right - x - RC.VertLineWidth, ViewRects.Client.Height);
			if(!IsRightToLeft) return rect;
			return ConvertBoundsToRTL(rect, ViewRects.Client);
		}
		internal virtual bool IsRightToLeft { get { return WindowsFormsSettings.GetIsRightToLeft(Grid); } }
		public Rectangle ConvertBoundsToRTL(Rectangle bounds, Rectangle ownerBounds) {
			int x = ownerBounds.Right + ownerBounds.X - bounds.Right;
			return new Rectangle(x, bounds.Y, bounds.Width, bounds.Height);
		}
	}
	public class SingleRecordViewInfo : BaseViewInfo {
		public SingleRecordViewInfo(VGridControlBase grid, bool isPrinting)
			: base(grid, isPrinting) {
		}
		internal protected override void AddEmptyRects() {}
		protected override void CalcBandWidth() {
			ViewRects.BandWidth = ViewRects.Client.Width;
		}
		protected override Rectangle GetValueRectCore(int valueIndex, BaseRowViewInfo ri) {
			Rectangle r = new Rectangle(ri.ValuesRect.Left, ri.RowRect.Top, ri.RowRect.Right - ri.ValuesRect.Left, ri.RowRect.Height);
			if(IsRightToLeft)
				r = ConvertBoundsToRTL(r, ViewRects.Client);
			return r;
		}
		protected override int GetHeader_ValuesSeparatorPosition(Rectangle bandRect) {
			if(!Grid.OptionsView.ShowRows) return invalid_position;
			Rectangle bandRectArea = Rectangle.Inflate(bandRect, -RC.VertLineWidth, -RC.HorzLineWidth);
			Rectangle[] rects = ScaleRowRects(bandRectArea);
			int x = rects[0].Right + RC.VertLineWidth - bandRect.X;
			return IsRightToLeft ? ViewRects.Client.Right - x : x;
		}
		public override int GetRecordWidth(int rowWidth, int headerWidth) { return rowWidth - headerWidth; }
		protected internal override Rectangle UpdateRecord(int recordIndex) {
			if(recordIndex != Grid.FocusedRecord) return Rectangle.Empty;
			return base.UpdateRecord(recordIndex);
		}
		public override BasePrintInfo CreatePrintInfo() {
			return new SingleRecordPrintInfo(this);
		}
		public override int HeaderViewWidth { get { return ScaledRowRects[0].Width; } }
		public override int ValueViewWidth { get { return ScaledRowRects[1].Width; } }
		public override void Clear() {
			base.Clear();
			scaledRowRects = null;
		}
		Rectangle[] scaledRowRects;
		Rectangle[] ScaledRowRects {
			get {
				if (scaledRowRects == null)
					scaledRowRects = GetScaledRowRects();
				return scaledRowRects;
			}
		}
		Rectangle[] GetScaledRowRects() {
			return ScaleRowRects(new Rectangle(Point.Empty, new Size(ViewRects.Client.Width, BaseRow.MinHeight)));
		}
	}
	public class MultiRecordViewInfo : BaseViewInfo {
		int visibleValuesCount;
		public MultiRecordViewInfo(VGridControlBase grid, bool isPrinting)
			: base(grid, isPrinting) {
		}
		public override int CompleteVisibleValuesCount {
			get {
				if(Grid.RecordCount == 0)
					return 0;
				int withSpaceCount = ValuesWidth / FullRecordValueWidth;
				int residualWidth = ValuesWidth % FullRecordValueWidth;
				int correctedCount = withSpaceCount + ((ValueViewWidth + RC.VertLineWidth <= residualWidth) ? 1 : 0);
				return correctedCount;
			}
		}
		internal protected override int FirstVisibleRecordIndex { get { return IsPrinting ? 0 : Scroller.LeftVisibleRecord; } }
		public override int VisibleValuesCount {
			get {
				if(IsPrinting)
					return Grid.RecordCount;
				if(visibleValuesCount == -1) {
					visibleValuesCount = CalcVisibleValuesCount();
				}
				return visibleValuesCount;
			}
		}
		public int RecordsInterval { get { return Grid.RecordsInterval; } }
		public override bool CanAddCategoryValuesSeparatorLines { get { return RecordsInterval > 0; } }
		public override bool CanUseFocusedRecordStyle { get { return true; } }
		public override float HorzScaleCoeff { get { return 1.0f; } }
		int AvailableValuesCount {
			get {
				if(ValuesWidth < 0)
					return 0;
				return (int)Math.Ceiling(((double)ValuesWidth + Scroller.LeftVisibleRecordPixelOffset) / (double)FullRecordValueWidth);
			}
		}
		int RecordValueSpace { get { return (RecordsInterval == 0) ? RC.VertLineWidth : RecordsInterval + 2 * RC.VertLineWidth; } }
		public override int FullRecordValueWidth { get { return (Grid.RecordCount != 0 ? RecordValueSpace + ValueViewWidth : 0); } }
		protected override int OneBandWidth { get { return FullRowHeaderWidth + (VisibleValuesCount != 0 ? RC.VertLineWidth + ValueViewWidth : 0) + RC.VertLineWidth; } }
		public override void Clear() {
			base.Clear();
			this.visibleValuesCount = -1;
		}
		protected override void CalcBandWidth() {
			ViewRects.CommonWidth = OneBandWidth;
			if(Grid.RecordCount > 1)
				ViewRects.CommonWidth += (VisibleValuesCount - 1) * FullRecordValueWidth;
			ViewRects.BandWidth = Math.Min(ViewRects.CommonWidth, ViewRects.Client.Width);
		}
		protected override Rectangle PrintAreaRect { get { return new Rectangle(0, 0, int.MaxValue, int.MaxValue); } }
		internal protected override void AddEmptyRects() {
			if(ViewRects.BandRects.Count == 0)
				return;
			Rectangle bandRect = ViewRects.BandRects[0];
			if (RecordsInterval > 0) {
				int left = bandRect.Left + OneBandWidth - Scroller.LeftVisibleRecordPixelOffset;
				for (int i = 0; i < Grid.RecordCount - 1; i++) {
					int rectLeft = Math.Max(left, bandRect.Left + FullRowHeaderWidth + RC.VertLineWidth);
					Rectangle r = new Rectangle(rectLeft, ViewRects.Client.Top, RecordsInterval - (rectLeft - left), ViewRects.Client.Height);
					if (r.Right > ViewRects.Client.Right) {
						if (r.Left > ViewRects.Client.Right)
							break;
						r.Width -= r.Right - ViewRects.Client.Right;
					}
					ViewRects.EmptyRects.Add(r);
					left += FullRecordValueWidth;
				}
			}
			if(bandRect.Right < ViewRects.Client.Right)
				ViewRects.EmptyRects.Add(new Rectangle(bandRect.Right, bandRect.Top, ViewRects.Client.Right - bandRect.Right, ViewRects.Client.Height));
		}
		protected override VGridHitTest CalcClientHitTestCore(Point pt) {
			if(RowsViewInfo.Count > 0 && Grid.OptionsBehavior.ResizeRowValues) {
				VGridHitTest ht = new VGridHitTest();
				ht.PtMouse = pt;
				ht.RowInfo = this[pt];
				if (!IsCategoryRowViewInfo(ht.RowInfo)) {
					foreach (BaseRowViewInfo ri in RowsViewInfo) {
						if (ri.ValuesInfo.Count == 0)
							continue;
						RowValueInfo valueInfo = ri.ValuesInfo[0];
						int firstValue_rightSide = IsRightToLeft ? valueInfo.Bounds.Left : valueInfo.Bounds.Right;
						if (Math.Abs(pt.X - firstValue_rightSide) <= 2 && (ViewRects.BandRects.Count == 0 || pt.Y < ((Rectangle)ViewRects.BandRects[0]).Bottom)) {
							if (!IsCategoryRowViewInfo(ht.RowInfo)) {
								ht.HitInfoType = HitInfoTypeEnum.RecordValueEdge;
								return ht;
							}
						}
					}
				}
			}
			return base.CalcClientHitTestCore(pt);			
		}
		protected override void CalcRowRectsCore(BaseRowViewInfo ri) {
			ri.HeaderInfo.HeaderRect = new Rectangle(ri.RowRect.Left, ri.RowRect.Top, RowHeaderWidth, ri.RowRect.Height);
			int left = ri.RowRect.Left + FullRowHeaderWidth;
			ri.ValuesRect = new Rectangle(left, ri.RowRect.Top, ri.RowRect.Right - left, ri.RowRect.Height);
		}
		protected override bool ShouldAddVertValueSide(BaseRow row) {
			if(!(row is CategoryRow)) return true;
			return !Grid.Scroller.CanHorizontalScroll;
		}
		public override BasePrintInfo CreatePrintInfo() {
			return new MultiRecordPrintInfo(this);
		}
		protected override Rectangle GetValueRectCore(int valueIndex, BaseRowViewInfo ri) {
			int left = ri.RowRect.Left + FullRowHeaderWidth - Scroller.LeftVisibleRecordPixelOffset + valueIndex * FullRecordValueWidth;
			Rectangle r = new Rectangle(left, ri.RowRect.Top, ValueViewWidth, ri.RowRect.Height);
			if(IsRightToLeft)
				r = ConvertBoundsToRTL(r, ri.RowRect);
			return r;
		}
		public override int GetRecordWidth(int rowWidth, int headerWidth) { return Grid.RecordWidth; }
		protected override int GetHeader_ValuesSeparatorPosition(Rectangle bandRect) {
			if(!Grid.OptionsView.ShowRows) return invalid_position;
			int x = IsRightToLeft ?  bandRect.Right : bandRect.Left;
			int indent = RowHeaderWidth + RC.VertLineWidth + RC.separatorWidth / 2;
			return IsRightToLeft ? x - indent : x + indent;
		}
		protected internal override void AddVertValueSide(Rectangle r, bool first, bool last, Brush vertLineBrush, BaseRow row) {
			if (ViewRects.BandRects.Count == 0)
				return;
			Rectangle bandRect = (Rectangle)ViewRects.BandRects[0];
			if(r.Right > bandRect.Left + FullRowHeaderWidth) base.AddVertValueSide(r, first, last, vertLineBrush, row);
			if(IsRightToLeft && r.Left > bandRect.Left) base.AddVertValueSide(r, first, last, vertLineBrush, row);
			if(RC.VertLineWidth != 0 && CanAddCategoryValuesSeparatorLines && !first) {
				r = RectUtils.IncreaseFromLeft(r, RC.VertLineWidth);
				if (r.Left > bandRect.Left + FullRowHeaderWidth)
					LinesInfo.AddLine(new LineInfo(r.Left, r.Top, RC.VertLineWidth, TruncateHeightByFixedBottom(r, r.Height + RC.HorzLineWidth, row), vertLineBrush));
			}
		}
		int CalcVisibleValuesCount() {
			return Math.Max(0, CalcVisibleValuesCountCore());
		}
		int CalcVisibleValuesCountCore() {
			if(Grid.RecordCount < AvailableValuesCount)
				return Grid.RecordCount;
			return Math.Min(AvailableValuesCount, Grid.RecordCount - Scroller.LeftVisibleRecord);
		}
	}
	public class BandsViewInfo : BaseViewInfo {
		public BandsViewInfo(VGridControlBase grid, bool isPrinting)
			: base(grid, isPrinting) {
		}
		public override int VisibleValuesCount { get { return Grid.RecordCount == 0 ? 0 : 1; } }
		internal protected override void CalcBandRects() {
			for(int i = 0; i < Scroller.BandsInfo.Count; i++) {
				BandInfo bi = (BandInfo)Scroller.BandsInfo[i];
				CreateBand(bi.bandHeight, 40);
			}
			UpdateBandsWidth();
		}
		protected override int GetHeader_ValuesSeparatorPosition(Rectangle bandRect) {
			if(!Grid.OptionsView.ShowRows) return invalid_position;
			Rectangle bandRectArea = Rectangle.Inflate(bandRect, -RC.VertLineWidth, -RC.HorzLineWidth);
			Rectangle[] rects = ScaleRowRects(IsRightToLeft ? ConvertBoundsToRTL(bandRectArea, ViewRects.Client) : bandRectArea);
			int x = rects[0].Right + RC.VertLineWidth;
			return IsRightToLeft ? ViewRects.Client.Right - x : x;
		}
		public override int ValuesWidth { get { return ViewRects.BandRects.Count * FullRecordValueWidth; } }
		protected override void CalcBandWidth() {
			if(!AutoScaleBands) {
				base.CalcBandWidth();
				return;
			}
			if (ViewRects.BandRects.Count == 0)
				return;
			int bandIndentWidth = ViewRects.BandRects.Count > 1 ? (ViewRects.BandRects.Count - 1) * BandsInterval : 0;
			ViewRects.BandWidth = (ViewRects.Client.Width - bandIndentWidth) / ViewRects.BandRects.Count;
			if(ViewRects.BandWidth < Grid.BandMinWidth)
				ViewRects.BandWidth = Grid.BandMinWidth;
		}
		protected override void CreateBand(int bandHeight, int bandWidth) {
			int bandIndex = ViewRects.BandRects.Count;
			int left = (bandIndex == 0 ? ViewRects.Client.Left : 
				((Rectangle)ViewRects.BandRects[bandIndex - 1]).Right);
			Rectangle br = new Rectangle(left, ViewRects.Client.Top, bandWidth, Math.Min(bandHeight, ViewRects.Client.Height));
			if(IsRightToLeft)
				br = ConvertBoundsToRTL(br, ViewRects.Client);
			ViewRects.BandRects.Add(br);
		}
		void UpdateBandsWidth() {
			CalcBandWidth();
			int curLeft = ViewRects.Client.Left;
			for(int i = 0; i < ViewRects.BandRects.Count; i++) {
				Rectangle r = (Rectangle)ViewRects.BandRects[i];
				r.X = curLeft;
				r.Width = ViewRects.BandWidth;
				curLeft += ViewRects.BandWidth + BandsInterval;
				if(AutoScaleBands) {
					if(i == ViewRects.BandRects.Count - 1) {
						curLeft -= BandsInterval;
						r.Width += ViewRects.Client.Right - curLeft;
					}
				}
				if(IsRightToLeft)
					r = ConvertBoundsToRTL(r, ViewRects.Client);
				ViewRects.BandRects[i] = r;
			}
			for(int i = 0; i < ViewRects.EmptyRects.Count; i++) {
				Rectangle er = (Rectangle)ViewRects.EmptyRects[i];
				Rectangle br = (Rectangle)ViewRects.BandRects[er.Width];
				er.X = br.Left;
				er.Width = br.Width;
				if(IsRightToLeft)
					er = ConvertBoundsToRTL(er, ViewRects.Client);
				ViewRects.EmptyRects[i] = er;
			}
		}
		protected override VGridHitTest CalcClientHitTestCore(Point pt) {
			if(Grid.OptionsBehavior.ResizeRowValues && !Grid.OptionsView.AutoScaleBands && ViewRects.BandRects.Count > 0 && VisibleValuesCount > 0) {
				Rectangle r = (Rectangle)ViewRects.BandRects[0];
				int x = IsRightToLeft ? r.Left + 2 : r.Right - 2;
				r = new Rectangle(x, r.Top, 4, r.Height);
				if(r.Contains(pt)) {
					VGridHitTest ht = new VGridHitTest();
					ht.PtMouse = pt;
					ht.HitInfoType = HitInfoTypeEnum.BandEdge;
					ht.RowInfo = this[pt];
					return ht;
				}
			}
			return base.CalcClientHitTestCore(pt);			
		}
		internal protected override void AddEmptyRects() {
			if(ViewRects.BandRects.Count == 0)
				return;
			if(BandsInterval > 0) {
				for(int i = 0; i < ViewRects.BandRects.Count - 1; i++) {
					Rectangle band = (Rectangle)ViewRects.BandRects[i];
					ViewRects.EmptyRects.Add(new Rectangle(band.Right, band.Top, BandsInterval, ViewRects.Client.Height));
				}
			}
			Rectangle lastBand = (Rectangle)ViewRects.BandRects[ViewRects.BandRects.Count - 1];
			if(lastBand.Right < ViewRects.Client.Right) 
				ViewRects.EmptyRects.Add(new Rectangle(lastBand.Right, lastBand.Top,  ViewRects.Client.Right - lastBand.Right, ViewRects.Client.Height));
		}
		protected override bool CanAddRowToBand(Rectangle rowRect, Rectangle bandRect, int bandRowIndex) {
			return (rowRect.Bottom <= bandRect.Bottom || bandRowIndex == 0);
		}
		protected override Rectangle GetValueRectCore(int valueIndex, BaseRowViewInfo ri) {
			Rectangle r = new Rectangle(ri.ValuesRect.Left, ri.RowRect.Top, ri.RowRect.Right - ri.ValuesRect.Left, ri.RowRect.Height);
			if(IsRightToLeft)
				r = ConvertBoundsToRTL(r, ri.RowRect);
			return r;
		}
		public override int GetRecordWidth(int rowWidth, int headerWidth) { return Grid.RecordWidth; }
		protected internal override Rectangle UpdateRecord(int recordIndex) {
			if(recordIndex != Grid.FocusedRecord) return Rectangle.Empty;
			base.UpdateRecord(recordIndex);
			if(ViewRects.BandRects.Count > 0) {
				Rectangle first = (Rectangle)ViewRects.BandRects[0];
				Rectangle last = (Rectangle )ViewRects.BandRects[ViewRects.BandRects.Count - 1];
				return Rectangle.Union(first, last);
			}
			return Rectangle.Empty;
		}
		public override BasePrintInfo CreatePrintInfo() {
			return new BandsPrintInfo(this);
		}
		public int BandsInterval { get { return Grid.BandsInterval; } }
		public override int HeaderViewWidth { 
			get {
				if(!AutoScaleBands) return base.HeaderViewWidth;
				Rectangle[] rects = ScaleRowRects(new Rectangle(Point.Empty, new Size(ViewRects.BandWidth, BaseRow.MinHeight)));
				return rects[0].Width; 
			} 
		}
		public override int ValueViewWidth { 
			get {
				if(!AutoScaleBands) return base.ValueViewWidth;
				Rectangle[] rects = ScaleRowRects(new Rectangle(Point.Empty, new Size(ViewRects.BandWidth, BaseRow.MinHeight)));
				return rects[1].Width; 
			} 
		}
		protected bool AutoScaleBands { get { return Grid.IsAutoScaleBands; } }
	}
	public class ViewRects {
		BaseViewInfo viewInfo;
		ArrayList emptyRects;
		int bandWidth;
		Rectangle scrollableRectangle;
		Nullable<Rectangle> client;
		Rectangle fixedTop;
		Rectangle fixedBottom;
		bool isReady;
		List<Rectangle> bandRects;
		int commonWidth;
		public Rectangle ScrollSquare;
		public ViewRects(BaseViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			Clear();
		}
		public int BandWidth {
			get {
				if(bandWidth == 0)
					return this.ScrollableRectangle.Width;
				return bandWidth;
			}
			set {
				if(value == bandWidth)
					return;
				bandWidth = value;
			}
		}
		public List<Rectangle> BandRects { get { return bandRects; } }
		public ArrayList EmptyRects { get { return emptyRects; } }
		public Rectangle FixedTop { get { return fixedTop; } set { fixedTop = value; } }
		public Rectangle FixedBottom { get { return fixedBottom; } set { fixedBottom = value; } }
		public Rectangle Client {
			get {
				if (client == null) {
					if (!IsReady) {
						client = this.viewInfo.GetClientWithoutScrollBars();
						client = this.viewInfo.GetActualClient();
					}
				}
				return client.Value;
			}
		}
		public int CommonWidth { get { return commonWidth; } set { commonWidth = value; } }
		public Rectangle Window { get { return this.viewInfo.GetActualArea(); } }
		public void Calc() {
			Clear();
			if(this.viewInfo.Grid.VisibleRows.Count == 0) {
				BandRects.Add(new Rectangle(Client.Location, new Size(Client.Width, 0)));
				EmptyRects.Add(Client);
			} else {
				this.viewInfo.CalcBandRects();
				this.viewInfo.AddEmptyRects();
			}
			IsReady = true;
		}
		public void Clear() {
			IsReady = false;
			this.bandRects = new List<Rectangle>();
			this.emptyRects = new ArrayList();
			this.client = null;
			ScrollSquare = Rectangle.Empty;
			scrollableRectangle = Rectangle.Empty;
			BandWidth = 0;
			CommonWidth = 0;
		}
		bool IsReady { get { return isReady; } set { isReady = value; } }
		public Rectangle FindPanel { get; set; }
		public Rectangle ScrollableRectangle {
			get {
				if (scrollableRectangle.IsEmpty) {
					scrollableRectangle = this.viewInfo.GetClientWithoutScrollBars();
				}
				return scrollableRectangle;
			}
			set { scrollableRectangle = value; }
		}
		public void ReCalcActualClient() {
			client = this.viewInfo.GetActualClient();
		}
	}
	public class BaseRowHeaderInfo {
		protected internal const int VerticalOffset = -1,
			LeftCaptionOffset = 3,
			RightCaptionOffset = 1;
		BaseRow row;
		TreeButtonType treeButtonType;
		AppearanceObject style, indentStyle, expandButtonStyle;
		Indents rowIndents, categoryIndents;
		Lines linesInfo;
		Rectangle headerRect;
		public ArrayList CaptionsInfo;
		bool isDraftObject;
		public Rectangle IndentBounds { get; protected set; }
		public Rectangle ExpandButtonRect { get; protected set; }
		public Rectangle HeaderCellsRect { get; protected set;}
		public Rectangle HeaderRect { get { return headerRect; } set { headerRect = value; } }
		protected bool DraftCalculation { get { return isDraftObject; } }
		public BaseRowHeaderInfo(BaseRow row) {
			this.row = row;
			this.CaptionsInfo = new ArrayList();
			this.linesInfo = new Lines();
			this.rowIndents = new Indents();
			this.categoryIndents = new Indents();
			this.HeaderRect = this.IndentBounds = this.ExpandButtonRect = this.HeaderCellsRect = Rectangle.Empty;
			this.style = new AppearanceObject();
			this.indentStyle = this.expandButtonStyle = null;
			this.treeButtonType = TreeButtonType.TreeViewButton;
		}
		internal protected virtual void MakeDraft() {
			this.isDraftObject = true;
		}
		public void Calc(Rectangle headerRect, BaseViewInfo vi, BaseRow nextRow, bool calcBounds, BaseRowViewInfo rowViewInfo) {
			Clear();
			if(vi.IsRightToLeft && rowViewInfo != null)
				HeaderRect = vi.ConvertBoundsToRTL(HeaderRect, rowViewInfo.RowRect);
			else
				HeaderRect = headerRect;
			CalcIndentBounds(vi, nextRow, calcBounds);
			if(CanShowGroupButtons && calcBounds) {
				CalcExpandButtonRect(vi);
				if(vi.IsRightToLeft)
					ExpandButtonRect = vi.ConvertBoundsToRTL(ExpandButtonRect, HeaderRect);
			}
			ExpandButtonStyle = GetExpandButtonStyle(vi);
			CalcStyles(vi, calcBounds, rowViewInfo);
			int indent = HeaderRect.Right - IndentBounds.Right;
			HeaderCellsRect = new Rectangle(vi.IsRightToLeft ? IndentBounds.Left : IndentBounds.Right, IndentBounds.Top, indent, HeaderRect.Height);
			AddBoundHeaderLines(vi, nextRow);
			CalcRowCaptionsInfo(vi);
		}
		protected virtual void Clear() {
			CaptionsInfo.Clear();
			LinesInfo.Clear();
			RowIndents.Clear();
			CategoryIndents.Clear();
		}
		protected virtual void CalcIndentBounds(BaseViewInfo vi, BaseRow nextRow, bool calc) {
			if(!calc || HeaderRect.Width == 0) {
				IndentBounds = new Rectangle(HeaderRect.Left, HeaderRect.Top, 0, HeaderRect.Height);
				return;
			}
			int levelCount = Row.Level;
			int nextLevel = (nextRow == null ? -1 : nextRow.Level);
			bool isNextCategory = vi.CalcHelper.IsCategory(nextRow);
			ArrayList indents = CalcIndentsInfo(vi, levelCount, nextRow);
			Rectangle cr = new Rectangle(HeaderRect.Left, HeaderRect.Top, 0, HeaderRect.Height);
			IndentRectInfo ir = null;
			int boundsWidth = 0, j = 0;
			levelCount = indents.Count - 1;
			for(int i = 0; i < levelCount + 1; i++) {
				ir = (IndentRectInfo)indents[levelCount - i];
				if(ir.isCategory) {
					if(cr.Width != 0) {
						cr.Width +=  vi.RC.VertLineWidth;
						int sepPos = cr.Left;
						if(isNextCategory && nextLevel >= i - cr.Width / vi.RowIndentWidth && nextLevel < i + 1 + cr.Width / vi.RowIndentWidth)
							sepPos += vi.RowIndentWidth * (nextLevel - (i - cr.Width / vi.RowIndentWidth));
						AddNextIndentInfo(new IndentInfo(cr, sepPos, ((IndentRectInfo)indents[levelCount - j]).style), HasCategoryLineColor(isNextCategory, nextLevel), nextLevel - i <= 0, true, vi, false);
						boundsWidth += vi.RC.VertLineWidth;
					}
					AddNextIndentInfo(new IndentInfo(new Rectangle(HeaderRect.Left + boundsWidth, HeaderRect.Top, ir.size.Width + vi.RC.VertLineWidth, ir.size.Height), HeaderRect.Left + boundsWidth, ir.style), true, ir.underline, i < levelCount, vi, false);
					boundsWidth += (i < levelCount ? vi.RC.VertLineWidth : (IncreaseBoundsByLastVertLine ? vi.RC.VertLineWidth : 0));
					cr = new Rectangle(HeaderRect.Left + boundsWidth + ir.size.Width, HeaderRect.Top, 0, HeaderRect.Height);
				}
				else { 
					cr.Width += ir.size.Width;
					j = i;
				}
				boundsWidth += ir.size.Width;
			}
			if(cr.Width != 0) {
				AddNextIndentInfo(new IndentInfo(cr, vi.IsRightToLeft ? cr.Right : cr.Left, ((IndentRectInfo)indents[levelCount - j]).style),
					HasCategoryLineColor(isNextCategory, nextLevel), true, false, vi, true);
			}
			IndentBounds = new Rectangle(HeaderRect.Left, HeaderRect.Top, boundsWidth, HeaderRect.Height);
		}
		bool HasCategoryLineColor(bool isCategory, int nextLevel) { return (isCategory && nextLevel <= Row.Level); }
		protected virtual void CalcExpandButtonRect(BaseViewInfo vi) {
			int width = GetButtonPlaceBackgroundWidth(vi);
			Rectangle buttonPlace = new Rectangle(IndentBounds.Right - width,
				IndentBounds.Top, width, IndentBounds.Height);
			ExpandButtonRect = BaseViewInfo.GetInrect(buttonPlace, vi.RC.treeButSize.Height);
			treeButtonType = TreeButtonType.TreeViewButton;
		}
		private ArrayList CalcIndentsInfo(BaseViewInfo vi, int levelCount, BaseRow nextRow) {
			ArrayList indents = new ArrayList();
			BaseRow curRow = Row;
			bool underline = false;
			for(int i = levelCount; i > -1 ; i--) {
				bool isCategory = vi.CalcHelper.IsCategory(curRow);
				if(i == 0 && !Grid.OptionsView.ShowRootCategories && isCategory)
					continue;
				if(isCategory) {
					if(nextRow == null) underline = true;
					else underline = (curRow.Level >= nextRow.Level || !Grid.OptionsView.ShowRows);
				}
				else underline = i == levelCount;
				indents.Add(new IndentRectInfo(new Size(vi.RowIndentWidth, HeaderRect.Height), isCategory, underline, vi.CalcHelper.GetIndentStyle(this, curRow)));
				curRow = curRow.ParentRow;
			}
			return indents;
		}
		private void AddNextIndentInfo(IndentInfo val, bool toCategories, bool addHorzLine, bool addVertLine, BaseViewInfo vi, bool nativeCaptionIndent) {
			bool categoryIndent = (nativeCaptionIndent ? vi.CalcHelper.IsCategory(Row) : toCategories);
			if(categoryIndent) CategoryIndents.Add(val);
			else RowIndents.Add(val);
			vi.CalcHelper.AddHeaderIndentLines(this, val, toCategories, addHorzLine, addVertLine);
		}
		protected internal virtual Brush GetHorzLineBrush(VGridResourceCache rc) { return rc.RowHorzLineBrush; }
		protected internal virtual Brush GetVertLineBrush(VGridResourceCache rc) { return rc.RowVertLineBrush; }
		protected virtual void CalcRowCaptionsInfo(BaseViewInfo vi) {
			RowCaptionInfo ci = CalcCaptionInfo(Row.Properties, HeaderCellsRect, vi);
			CaptionsInfo.Add(ci);
			AddRightVertLine(ci.CaptionRect, vi);
		}
		protected virtual void AddBoundHeaderLines(BaseViewInfo vi, BaseRow nextRow) {
			vi.CalcHelper.AddBoundHeaderLines(this, nextRow);
		}
		public virtual void AddRightVertLine(Rectangle r, BaseViewInfo vi) {
			if(vi.RC.VertLineWidth == 0) return;
			LineInfo line = new LineInfo(vi.IsRightToLeft ? r.Left - vi.RC.VertLineWidth : r.Right, r.Top, vi.RC.VertLineWidth, r.Height + vi.RC.HorzLineWidth, GetVertLineBrush(vi.RC));
			LinesInfo.AddLine(line);
		}
		public virtual void AddBottomHorzLine(Rectangle r, BaseViewInfo vi, Brush horzLineBrush) {
			if(vi.RC.HorzLineWidth != 0)
				LinesInfo.AddLine(new LineInfo(r.Left, r.Bottom, r.Width, vi.RC.HorzLineWidth, horzLineBrush));
		}
		protected internal virtual RowCaptionInfo CalcCaptionInfo(RowProperties properties, Rectangle r, BaseViewInfo vi) {
			RowCaptionInfo ci = new RowCaptionInfo();
			ci.SetRow(Row);
			ci.CaptionRect = r;
			ci.AllowGlyphSkinning = vi.GetAllowGlyphSkinning(Row);
			ci.AllowHtmlText = vi.GetAllowHtmlRowHeaderText(Row);
			ci.ImageIndex = GetImageIndex(properties);
			int w = GetImageWidth(properties);
			ci.ImageRect = new Rectangle(ci.CaptionRect.Left, RectUtils.GetTopCentralPoint(vi.RC.imageSize.Height, ci.CaptionRect), w, vi.RC.imageSize.Height);
			ci.CaptionTextRect = new Rectangle(ci.ImageRect.Right + LeftCaptionOffset, ci.CaptionRect.Top + VerticalOffset, Math.Max(0, ci.CaptionRect.Right - ci.ImageRect.Right - LeftCaptionOffset - RightCaptionOffset - RightCaptionTextIndent), ci.CaptionRect.Height - VerticalOffset);
			ci.Caption = GetActualCaption(properties);
			ci.SetToolTip(GetToolTip(properties, ci.Caption));
			ci.IsCaptionFit = GetTextWidth(vi.RC.Graphics, ci.Caption) < ci.CaptionTextRect.Width;
			ci.Focused = Row == Grid.FocusedRow;
			CalcCaptionAppearance(ci, vi, properties);
			if(vi.IsRightToLeft) {
				ci.CaptionTextRect = vi.ConvertBoundsToRTL(ci.CaptionTextRect, ci.CaptionRect);
				ci.ImageRect = vi.ConvertBoundsToRTL(ci.ImageRect, ci.CaptionRect);
				ci.FocusRect = vi.ConvertBoundsToRTL(ci.FocusRect, ci.CaptionRect);
			}
			return ci;
		}
		protected virtual string GetToolTip(RowProperties properties, string caption) {
			if(properties == null || properties.ToolTip == string.Empty)
				return caption;
			return properties.ToolTip;
		}
		protected virtual string GetActualCaption(RowProperties properties) {
			if(properties.Row == null)
				return properties.Caption;
			if(!properties.Row.Visible && properties.Row.OptionsRow.ShowInCustomizationForm && !string.IsNullOrEmpty(properties.CustomizationCaption))
				return properties.CustomizationCaption;
			return properties.Caption;
		}
		protected int GetImageWidth(RowProperties p) {
			int w = 0;
			if(Grid.ImageList != null) {
				w = Grid.ViewInfo.RC.imageSize.Width;
				int imageIndex = GetImageIndex(p);
				if(imageIndex == -1 && !Grid.OptionsView.ShowEmptyRowImage)
					w = 0;
			}
			return w;
		}
		protected int GetImageIndex(RowProperties p) {
			return ImageCollection.IsImageListImageExists(Grid.ImageList, p.ImageIndex) ? p.ImageIndex : -1;
		}
		protected virtual void CalcCaptionAppearance(RowCaptionInfo captionInfo, BaseViewInfo vi, RowProperties p) {
			AppearanceHelper.Combine(captionInfo.Style, Style, null);
		}
		protected virtual AppearanceObject GetExpandButtonStyle(BaseViewInfo vi) {
			return vi.PaintAppearance.ExpandButton;
		}
		protected virtual void CalcStyles(BaseViewInfo vi, bool allowFocus, BaseRowViewInfo rowViewInfo) {
			AppearanceHelper.Combine(Style, GetRowHeaderAppearanceList(vi, allowFocus, Row.Properties, rowViewInfo));
			IndentStyle = vi.CalcHelper.GetIndentStyle(this, Row);
		}
		protected virtual AppearanceObject[] GetRowHeaderAppearanceList(BaseViewInfo vi, bool allowFocus, RowProperties properties, BaseRowViewInfo rowViewInfo) {
			AppearanceObject[] result = new AppearanceObject[2];
			if(allowFocus && Row == Grid.FocusedRow) result[0] = vi.FocusedRowStyle;
			result[1] = Row.GetRowHeaderStyle(vi.PaintAppearance);
			return result;
		}
		protected virtual int GetButtonPlaceBackgroundWidth(BaseViewInfo vi) {
			return vi.RowIndentWidth;
		}
		protected internal virtual int GetLeftViewPoint(PaintStyleCalcHelper calcHelper) { return calcHelper.GetRowHeaderViewPoint(this); }
		protected internal virtual int GetRightViewPoint(PaintStyleCalcHelper calcHelper) { return calcHelper.GetRowHeaderViewEndPoint(this); }
		protected virtual bool IncreaseBoundsByLastVertLine { get { return false; } }
		protected virtual bool CanShowGroupButtons { get { return (Row.HasChildren && Grid.OptionsView.ShowButtons && Row.ChildRows.HasVisibleItems); } }
		protected internal VGridControlBase Grid { get { return Row.Grid; } }
		public BaseRow Row { get { return row; } }
		public Lines LinesInfo { get { return linesInfo; } }
		public Indents RowIndents { get { return rowIndents; } }
		public Indents CategoryIndents { get { return categoryIndents; } }
		public TreeButtonType TreeButtonType { get { return treeButtonType; } set { treeButtonType = value; } }
		public AppearanceObject Style { get { return style; } }
		public AppearanceObject IndentStyle { get { return indentStyle; } set { indentStyle = value; } }																		   
		public AppearanceObject ExpandButtonStyle { get { return expandButtonStyle; } set { expandButtonStyle = value; } }
		public virtual int RightCaptionTextIndent { get { return 0; } }
		float GetRowHeaderWidth() {
			if(Row.VisibleIndex == -1)
				return 0f;
			return GetIndent() + GetCaptionsWidth();
		}
		protected virtual float GetCaptionsWidth() {
			return GetCaptionWidth(Grid.ViewInfo.RC.Graphics, Row.Properties);
		}
		protected int GetCaptionWidth(Graphics graphics, RowProperties p) {
			return GetNonTextSpace(p) + GetTextWidth(graphics, p.Caption);
		}
		protected virtual int GetTextWidth(Graphics graphics, string caption) {
			return (int)(Style.CalcTextSize(graphics, caption, int.MaxValue).Width);
		}
		protected int GetNonTextSpace(RowProperties p) {
			return LeftCaptionOffset + GetImageWidth(p) + RightCaptionOffset;
		}
		float GetIndent() {
			if(RowIndents.Count != 0)
				return GetRowIndent();
			if(CategoryIndents.Count != 0)
				return GetCategoryIndent();
			return 0f;
		}
		float GetRowIndent() {
			if(!Grid.ViewInfo.IsRightToLeft) return RowIndents[RowIndents.Count - 1].Bounds.Right;
			return Grid.ViewInfo.ViewRects.Client.Right - RowIndents[RowIndents.Count - 1].Bounds.Left;
		}
		float GetCategoryIndent() {
			if(!Grid.ViewInfo.IsRightToLeft) return CategoryIndents[CategoryIndents.Count - 1].Bounds.Right;
			return Grid.ViewInfo.ViewRects.Client.Right - CategoryIndents[CategoryIndents.Count - 1].Bounds.Left;
		}
		protected virtual internal float CalculateBestFit() {
			return GetRowHeaderWidth();
		}
		const int EndIndent = 10;
		protected virtual internal bool InEnd(Point point) {
			return new Rectangle(HeaderRect.Right - EndIndent, HeaderRect.Top, EndIndent, HeaderRect.Height).Contains(point);
		}
		protected bool DrawReadOnly(RowProperties properties, BaseRowViewInfo rowViewInfo) {
			if(rowViewInfo == null)
				return properties.GetReadOnly();
			return rowViewInfo.DrawHeaderReadOnly(Grid.FocusedRecord, properties);
		}
		protected internal virtual void CalcSeparatorWidth(Graphics g, int lineSeparatorWidth) { }
	}
	public abstract class BaseRowViewInfo {
		public const int HorizontalOffset = 1,
			VerticalOffset = 1;
		BaseRow row;
		BaseRowHeaderInfo headerInfo; 
		RowValueCollection valuesInfo;
		int bandIndex, bandRowIndex;
		Rectangle rowRect;
		public Rectangle ValuesRect;
		protected BaseRowViewInfo(BaseRow row) {
			this.row = row;
			this.bandIndex = this.bandRowIndex = -1;
			this.valuesInfo = new RowValueCollection(this);
			this.rowRect = this.ValuesRect = Rectangle.Empty;
			this.headerInfo = Row.CreateHeaderInfo();
		}
		public RowValueInfo this[int recordIndex, int cellIndex] {
			get {
				foreach(RowValueInfo rv in ValuesInfo) {
					if(rv.RecordIndex == recordIndex && rv.RowCellIndex == cellIndex)
						return rv;
				}
				return null;
			}
		}
		public Rectangle RowRect { get { return rowRect; } }
		protected internal void Calc(Rectangle rowRect, BaseViewInfo vi, BaseRow nextRow) {
			this.rowRect = vi.GetVisibleRowRect(rowRect);
			vi.CalcRowRects(this);
			HeaderInfo.CalcSeparatorWidth(Grid.ViewInfo.RC.Graphics, Grid.ViewInfo.RC.separatorWidth);
			CalcValuesInfo(vi, nextRow);
			CalcRowHeaderInfo(vi, nextRow);
			CalcPaintStyleLines(vi, nextRow);
			if(vi.IsRightToLeft) {
				ValuesRect = vi.ConvertBoundsToRTL(ValuesRect, rowRect);
			}
		}
		protected virtual void CalcRowHeaderInfo(BaseViewInfo vi, BaseRow nextRow) {
			HeaderInfo.Calc(HeaderInfo.HeaderRect, vi, nextRow, true, this);
		}
		protected internal virtual RowValueInfo CalcRowValueInfo(RowProperties p, Rectangle r, BaseViewInfo viewInfo, int recordIndex) {
			RowValueInfo valueInfo = CreateValueInfo();
			valueInfo.SetRow(Row);
			valueInfo.Bounds = r;
			valueInfo.SetRecordIndex(recordIndex);
			valueInfo.SetRowCellIndex(p.CellIndex);
			valueInfo.SetEditorViewInfo(Grid.CreateEditorViewInfo(p, recordIndex));
			valueInfo.CalcAppearance(viewInfo);
			valueInfo.EditorViewInfo.Appearance = valueInfo.Style;
			valueInfo.EditorViewInfo.PaintAppearance = valueInfo.Style;
			valueInfo.EditorViewInfo.AllowTextToolTip = true;
			valueInfo.EditorViewInfo.DetailLevel = GetDetailLevel(valueInfo);
			valueInfo.EditorViewInfo.FillBackground = false;
			valueInfo.Focused = IsRowCellFocused(valueInfo);
			if(valueInfo.Focused)
				valueInfo.DrawFocusFrame = Grid.OptionsView.ShowFocusedFrame;
			valueInfo.EditorViewInfo.Bounds = Rectangle.Inflate(valueInfo.Bounds, -HorizontalOffset, -VerticalOffset);
			valueInfo.CalcViewInfo();
			return valueInfo;
		}
		protected virtual RowValueInfo CreateValueInfo() {
			return new RowValueInfo(this);
		}
		protected virtual bool IsRowCellDefaultValue(RowValueInfo rv) {
			return Grid.IsCellDefaultValue(Row, rv.RecordIndex);
		}
		protected virtual void CalcPaintStyleLines(BaseViewInfo vi, BaseRow nextRow) {
			vi.CalcHelper.CalcPaintStyleLines(this, nextRow);
		}
		protected virtual bool IsRowCellFocused(RowValueInfo rv) {
			if(Grid.HasFocus && Row == Grid.FocusedRow && rv.RecordIndex == Grid.DataModeHelper.Position && rv.RowCellIndex == Grid.FocusedRecordCellIndex)
				return true;
			return false;
		}
		private DetailLevel GetDetailLevel(RowValueInfo rv) {
			if(Grid.ShowButtonMode == ShowButtonModeEnum.ShowAlways) return DetailLevel.Full;
			if(rv.RecordIndex == Grid.DataModeHelper.Position) {
				if(Grid.ShowButtonMode == ShowButtonModeEnum.ShowForFocusedRecord) return DetailLevel.Full;
				if(Grid.ShowButtonMode == ShowButtonModeEnum.ShowForFocusedCell &&
					IsRowCellFocused(rv))
					return DetailLevel.Full;
			}
			if(rv.Row == Grid.FocusedRow && Grid.ShowButtonMode == ShowButtonModeEnum.ShowForFocusedRow) return DetailLevel.Full;
			return DetailLevel.Minimum;
		}
		internal protected virtual AppearanceObject[] GetRowValueAppearanceList(BaseViewInfo vi, RowValueInfo rv) {
			return GetRowValueAppearanceListCore(vi, rv);
		}
		internal protected virtual RowProperties GetRowProperties(int index) {
			return Row.GetRowProperties(index);
		}
		protected internal AppearanceObject[] GetRowValueAppearanceListCore(BaseViewInfo vi, RowValueInfo rv) {
			AppearanceObject[] result = new AppearanceObject[5];
			result[0] = (rv.Row.Appearance.Options.HighPriority ? rv.Row.Appearance : null);
			if(IsRowCellFocused(rv)) result[1] = vi.PaintAppearance.FocusedCell;
			if(rv.RecordIndex == Grid.FocusedRecord && vi.CanUseFocusedRecordStyle) result[2] = vi.PaintAppearance.FocusedRecord;
			result[3] = (rv.Row.Appearance.Options.HighPriority ? null : rv.Row.Appearance);
			result[4] = vi.PaintAppearance.RecordValue;
			return result;
		}
		protected internal VGridHitTest CalcHitTest(Point pt, BaseViewInfo vi) {
			VGridHitTest ht = new VGridHitTest();
			ht.PtMouse = pt;
			if(IsOnRowEdge(pt, vi)) {
				ht.HitInfoType = HitInfoTypeEnum.RowEdge;
				ht.RowInfo = this;
				return ht;
			}
			int hlw = vi.RC.HorzLineWidth, vlw = vi.RC.VertLineWidth;
			Rectangle rowBorderRect = new Rectangle(RowRect.Left - vlw, RowRect.Top - (BandRowIndex == 0 ? hlw : 0),
				RowRect.Width + 2 * vlw, RowRect.Height + hlw + (BandRowIndex == 0 ? hlw : 0));
			if(!rowBorderRect.Contains(pt)) return ht;
			ht.RowInfo = this;
			ht.HitInfoType = HitInfoTypeEnum.Row;
			if(HeaderInfo.ExpandButtonRect.Contains(pt)) {
				ht.HitInfoType = HitInfoTypeEnum.ExpandButton;
				return ht;
			}
			if(HeaderInfo.HeaderCellsRect.Contains(pt)) {
				return CalcHeaderHitTest(pt, ht);
			}
			if(ValuesRect.Contains(pt)) {
				return CalcValuesHitTest(pt, ht);
			}
			return ht;
		}
		protected internal virtual VGridHitTest CalcHeaderHitTest(Point pt, VGridHitTest ht) {
			foreach(RowCaptionInfo rc in HeaderInfo.CaptionsInfo) {
				if(rc.CaptionRect.Contains(pt)) {
					ht.CaptionInfo = rc;
					ht.HitInfoType = HitInfoTypeEnum.HeaderCell;
					if(rc.ImageRect.Contains(pt)) {
						ht.HitInfoType = HitInfoTypeEnum.HeaderCellImage;
						return ht;
					}
					if(rc.SortShapeRect.Contains(pt)) {
						ht.HitInfoType = HitInfoTypeEnum.HeaderCellSortShape;
						return ht;
					}
					if(rc.FilterButtonRect.Contains(pt)) {
						ht.HitInfoType = HitInfoTypeEnum.HeaderCellFilterButton;
						return ht;
					}
					return ht;
				}
			}
			return ht;
		}
		protected internal virtual VGridHitTest CalcValuesHitTest(Point pt, VGridHitTest ht) {
			foreach(RowValueInfo rv in ValuesInfo) {
				if(rv.Bounds.Contains(pt)) {
					ht.HitInfoType = HitInfoTypeEnum.ValueCell;
					ht.ValueInfo = rv;
					return ht;
				}
			}
			return ht;
		}
		public virtual Rectangle UpdateCells(int recordIndex) {
			Rectangle bounds = Rectangle.Empty;
			for(int cellIndex = 0; cellIndex < Row.RowPropertiesCount; cellIndex++) {
				RowValueInfo rv = this[recordIndex, cellIndex];
				if(rv != null) {
					UpdateCellData(rv);
					if(bounds.IsEmpty)
						bounds = rv.Bounds;
					else
						bounds = Rectangle.Union(bounds, rv.Bounds);
				}
			}
			return bounds;
		}
		internal void UpdateCellData(RowValueInfo rv) {
			if(rv == null)
				return;
			rv.CalcViewInfo();
		}
		const int RowEdgeHeight = 5;
		protected bool IsOnRowEdge(Point pt, BaseViewInfo vi) {
			if (!CanResizeHeaders)
				return false;
			int left = HeaderInfo.GetLeftViewPoint(vi.CalcHelper);
			int width = HeaderInfo.GetRightViewPoint(vi.CalcHelper) - left;
			int top;
			Rectangle rEdge;
			if(Row.Fixed == FixedStyle.Bottom) {
				top = HeaderInfo.IndentBounds.Top - Grid.OptionsView.FixedLineWidth - RowEdgeHeight / 2;
				rEdge = new Rectangle(left, top, width, Grid.OptionsView.FixedLineWidth / 2 + RowEdgeHeight);
			} else {
				top = HeaderInfo.IndentBounds.Bottom - RowEdgeHeight / 2 - vi.RC.HorzLineWidth;
				rEdge = new Rectangle(left, top, width, (Row.Fixed == FixedStyle.Top) ? Grid.OptionsView.FixedLineWidth + RowEdgeHeight : RowEdgeHeight);
			}
			return rEdge.Contains(pt);
		}
		protected void AddRectValueLines(Rectangle r, BaseViewInfo vi, bool first, bool last, BaseRow nextRow) {
			vi.AddBottomValueSide(vi.CalcHelper.UpdateGridLinesBounds(r, nextRow), vi.CalcHelper.GetUnderLineHorzBrush(HeaderInfo, nextRow), Row);
			vi.AddVertValueSide(r, first, last, HeaderInfo.GetVertLineBrush(vi.RC), Row);
		}
		protected VGridControlBase Grid { get { return Row.Grid; } }
		protected abstract void CalcValuesInfo(BaseViewInfo vi, BaseRow nextRow);
		protected bool ShowButtons { get { return Grid.OptionsView.ShowButtons; } }
		protected bool CanResizeHeaders { 
			get { 
				if(Grid == null || Row == null) return false;
				return (Grid.OptionsBehavior.ResizeRowHeaders && Row.OptionsRow.AllowSize); 
			} 
		}
		protected bool CanResizedValues {
			get {
				if (Grid == null || Row == null) return false;
				return (Grid.OptionsBehavior.ResizeRowValues && Row.OptionsRow.AllowSize);
			}
		}
		protected internal bool DrawReadOnly(RowValueInfo valueInfo) {
			if(Grid == null)
				return true;
			return Grid.ViewInfoHelper.DrawReadOnly(valueInfo);
		}
		protected internal bool DrawHeaderReadOnly(int recordIndex, RowProperties properties) {
			if(Grid == null)
				return true;
			return Grid.ViewInfoHelper.DrawHeaderReadOnly(properties, recordIndex, Grid);
		}
		public BaseRow Row { get { return row; } }
		public BaseRowHeaderInfo HeaderInfo { get { return headerInfo; } } 
		public RowValueCollection ValuesInfo { get { return valuesInfo; } }
		public int BandIndex { get { return bandIndex; } set { bandIndex = value; } }
		public int BandRowIndex { get { return bandRowIndex; } set { bandRowIndex = value; } }
	}
	public class CategoryRowHeaderInfo : BaseRowHeaderInfo {
		int valuesSeparatorPos;
		int rightCaptionTextIndent;
		public CategoryRowHeaderInfo(BaseRow row) : base(row) {
			this.valuesSeparatorPos = BaseViewInfo.invalid_position;
		}
		protected override float GetCaptionsWidth() {
			return 0f;
		}
		public override void AddRightVertLine(Rectangle r, BaseViewInfo vi) {
		}
		protected override AppearanceObject GetExpandButtonStyle(BaseViewInfo vi) {
			return vi.PaintAppearance.CategoryExpandButton;
		}
		protected override AppearanceObject[] GetRowHeaderAppearanceList(BaseViewInfo vi, bool allowFocus, RowProperties properties, BaseRowViewInfo rowViewInfo) {
			return new AppearanceObject[] { Row.GetRowHeaderStyle(vi.PaintAppearance)};
		}
		protected override int GetButtonPlaceBackgroundWidth(BaseViewInfo vi) {
			return vi.RowIndentWidth;
		}
		protected override void CalcRowCaptionsInfo(BaseViewInfo vi) {
			base.CalcRowCaptionsInfo(vi);
			if(CaptionsInfo.Count == 0) return;
			RowCaptionInfo ci = (RowCaptionInfo)CaptionsInfo[0];
			ci.FocusRect = GetFocusRect(vi);
		}
		protected virtual Rectangle GetFocusRect(BaseViewInfo vi) {
			if(Grid.HasFocus)
				return vi.CalcHelper.GetCategoryFocusRect(this); 
			return Rectangle.Empty;
		}
		protected override void CalcIndentBounds(BaseViewInfo vi, BaseRow nextRow, bool calc) {
			base.CalcIndentBounds(vi, nextRow, calc && ShowRows);
		}
		protected override void CalcExpandButtonRect(BaseViewInfo vi) {
			if (Row.TreeButtonType == TreeButtonType.ExplorerBarButton && ShowRows) {
				TreeButtonType = TreeButtonType.ExplorerBarButton;
				Size buttonSize = Row.Expanded ? vi.RC.ExplorerButtonExpandedSize : vi.RC.ExplorerButtonCollapsedSize;
				this.rightCaptionTextIndent = GetRightCaptionTextIndent(vi.RC);
				int left = HeaderRect.Right - RightCaptionTextIndent;
				if (left < GetLeftViewPoint(vi.CalcHelper))
					return;
				Rectangle buttonPlace = new Rectangle(left, HeaderRect.Top, buttonSize.Width, HeaderRect.Height);
				ExpandButtonRect = BaseViewInfo.GetInrect(buttonPlace, buttonSize);
			}
			else {
				base.CalcExpandButtonRect(vi);
			}
		}
		protected internal override Brush GetHorzLineBrush(VGridResourceCache rc) { return rc.CategoryHorzLineBrush; }
		protected internal override Brush GetVertLineBrush(VGridResourceCache rc) { return rc.CategoryHorzLineBrush; }
		protected internal override int GetLeftViewPoint(PaintStyleCalcHelper calcHelper) { return calcHelper.GetCategoryHeaderViewPoint(this); }
		public override void AddBottomHorzLine(Rectangle r, BaseViewInfo vi, Brush horzLineBrush) {
			if(vi.RC.HorzLineWidth == 0) return;
			r = vi.IsRightToLeft ? RectUtils.IncreaseFromRight(r, vi.RC.VertLineWidth) : RectUtils.IncreaseFromLeft(r, vi.RC.VertLineWidth);
			LinesInfo.AddLine(new LineInfo(r.Left, r.Bottom, r.Width + vi.RC.VertLineWidth, vi.RC.HorzLineWidth, GetHorzLineBrush(vi.RC)));
		}
		public void EnlargeHeaderBounds(Rectangle valuesBounds) {
			valuesSeparatorPos = valuesBounds.Left;
			HeaderRect = new Rectangle(HeaderRect.X, HeaderRect.Y, valuesBounds.Right - HeaderRect.Left, HeaderRect.Height);
		}
		protected int GetRightCaptionTextIndent(VGridResourceCache rc) {
			if (TreeButtonType == TreeButtonType.TreeViewButton)
				return base.RightCaptionTextIndent;
			Size buttonSize = Row.Expanded ? rc.ExplorerButtonExpandedSize : rc.ExplorerButtonCollapsedSize;
			return 3 * buttonSize.Width / 2;
		}
		protected override bool IncreaseBoundsByLastVertLine { get { return true; } }
		protected new CategoryRow Row { get { return (CategoryRow)base.Row; } }
		public override int RightCaptionTextIndent { get { return rightCaptionTextIndent; } }
		protected bool ShowRows { get { return Grid.OptionsView.ShowRows; } }
		public int ValuesSeparatorPos { get { return valuesSeparatorPos; } }
	}
	public class CategoryRowViewInfo : BaseRowViewInfo {
		public CategoryRowViewInfo(BaseRow row)
			: base(row) {
		}
		protected override void CalcRowHeaderInfo(BaseViewInfo vi, BaseRow nextRow) {
			CategoryRowHeaderInfo categoryHeaderInfo = HeaderInfo as CategoryRowHeaderInfo;
			categoryHeaderInfo.EnlargeHeaderBounds(ValuesRect);
			ValuesRect.X = HeaderInfo.HeaderRect.Right;
			ValuesRect.Width = 0;
			base.CalcRowHeaderInfo(vi, nextRow);
		}
		protected override void CalcValuesInfo(BaseViewInfo vi, BaseRow nextRow) {
			if(vi.CanAddCategoryValuesSeparatorLines) {
				for(int i = 0; i < vi.VisibleValuesCount; i++) {
					Rectangle rect = RectUtils.IncreaseFromTop(vi.GetValueRect(i, this), vi.RC.HorzLineWidth);
					vi.AddVertValueSide(rect, i == 0, i == vi.VisibleValuesCount - 1, HeaderInfo.GetVertLineBrush(vi.RC), Row);
				}
			} else {
				Rectangle rect = RectUtils.IncreaseFromTop(ValuesRect, vi.RC.HorzLineWidth);
				if(vi.IsRightToLeft)
					rect = vi.ConvertBoundsToRTL(rect, RowRect);
				vi.AddVertValueSide(rect, true, true, HeaderInfo.GetVertLineBrush(vi.RC), Row);
			}
		}
		protected internal override VGridHitTest CalcHeaderHitTest(Point pt, VGridHitTest ht) {
			if(HeaderInfo.HeaderRect.Contains(pt)) {
				RowCaptionInfo rc = (RowCaptionInfo)HeaderInfo.CaptionsInfo[0];
				ht.CaptionInfo = rc;
				ht.HitInfoType = HitInfoTypeEnum.HeaderCell;
				if(rc.ImageRect.Contains(pt)) {
					ht.HitInfoType = HitInfoTypeEnum.HeaderCellImage;
					return ht;
				}
			}
			return ht;
		}
		protected new CategoryRow Row { get { return (CategoryRow)base.Row; } }
	}
	public class EditorRowHeaderInfo : BaseRowHeaderInfo {
		public EditorRowHeaderInfo(BaseRow row) : base(row) {}
		protected override void CalcStyles(BaseViewInfo vi, bool allowFocus, BaseRowViewInfo rowViewInfo) {
			base.CalcStyles(vi, allowFocus, rowViewInfo);
			if(!Row.Enabled) {
				IndentStyle = Style;
			}
		}
		protected override AppearanceObject[] GetRowHeaderAppearanceList(BaseViewInfo vi, bool allowFocus, RowProperties properties, BaseRowViewInfo rowViewInfo) {
			AppearanceObject[] result = base.GetRowHeaderAppearanceList(vi, allowFocus, properties, rowViewInfo);
			if(!Row.Enabled) {
				result = MemoryUtils.IncreaseFromHead(result, 1);
				result[0] = result[1];
				result[1] = vi.PaintAppearance.DisabledRow;
			}
			if(properties != null && DrawReadOnly(properties, rowViewInfo)) {
				result = MemoryUtils.IncreaseFromHead(result, 1);
				result[0] = result[1];
				result[1] = vi.PaintAppearance.ReadOnlyRow;
			}
			if (!Grid.IsCellDefaultValue(Row, -1) && !DrawReadOnly(properties, rowViewInfo)) {
				result = MemoryUtils.IncreaseFromHead(result, 1);
				result[0] = result[1];
				result[1] = vi.PaintAppearance.ModifiedRow;
			}
			return result;
		}
		protected new EditorRow Row { get { return (EditorRow)base.Row; } }
	}
	public class EditorRowViewInfo : BaseRowViewInfo {
		public EditorRowViewInfo(BaseRow row) : base(row) {
		}
		internal protected override AppearanceObject[] GetRowValueAppearanceList(BaseViewInfo vi, RowValueInfo rv) {
			AppearanceObject[] result = base.GetRowValueAppearanceList(vi, rv);
			if(!Row.Enabled) {
				result = MemoryUtils.IncreaseFromHead(result, 1);
				result[0] = vi.PaintAppearance.DisabledRecordValue;
			}
			if(Row.Properties != null && DrawReadOnly(rv)) {
				result = MemoryUtils.IncreaseFromHead(result, 1);
				result[0] = vi.PaintAppearance.ReadOnlyRecordValue;
			}
			if(!IsRowCellDefaultValue(rv)) {
				result = MemoryUtils.IncreaseFromHead(result, 1);
				result[0] = vi.PaintAppearance.ModifiedRecordValue;
			}
			return result;
		}
		protected override void CalcValuesInfo(BaseViewInfo vi, BaseRow nextRow) {
			ValuesInfo.Clear();
			for(int i = 0; i < vi.VisibleValuesCount; i++) {
				Rectangle rValue = vi.GetValueRect(i, this);
				RowValueInfo rv = CalcRowValueInfo(Row.Properties, rValue, vi, vi.GetDrawnRecordIndex(i));
				ValuesInfo.Add(rv);
				AddRectValueLines(rValue, vi, i == 0, i == vi.VisibleValuesCount - 1, nextRow);
			}
		}
		protected new EditorRow Row { get { return (EditorRow)base.Row; } }
	}
	public class MultiEditorRowHeaderInfo : EditorRowHeaderInfo {
		internal ArrayList SeparatorRects;
		SeparatorInfo separatorInfo;
		bool needArrangeCaptions;
		Lazy<IList<MultiEditorRowProperties>> visibleRowProperties;
		public IList<MultiEditorRowProperties> VisibleRowProperties { get { return visibleRowProperties.Value; } }
		protected static bool GetNeedArrangeCaptions(BaseRow row) {
			return ((MultiEditorRowHeaderInfo)row.HeaderInfo).NeedArrangeCaptions;
		}
		protected static void SetNeedArrangeCaptions(MultiEditorRow row, bool value) {
			((MultiEditorRowHeaderInfo)row.HeaderInfo).NeedArrangeCaptions = value;
		}
		public MultiEditorRowHeaderInfo(BaseRow row) : base(row) {
			SeparatorRects = new ArrayList();
			separatorInfo = new SeparatorInfo(Row);
			this.visibleRowProperties = new Lazy<IList<MultiEditorRowProperties>>(() => Row.GetVisibleRowProperties());
		}
		protected internal SeparatorInfo SeparatorInfo {
			get { return separatorInfo; }
		}
		protected virtual bool NeedArrangeCaptions {
			get { return needArrangeCaptions; }
			set { needArrangeCaptions = value; }
		}
		protected override float GetCaptionsWidth() {
			separatorInfo = new SeparatorInfo(Row);
			Graphics graphics = Grid.ViewInfo.RC.Graphics;
			CalcSeparatorWidth(graphics, Grid.ViewInfo.RC.separatorWidth);
			float captionsWidth = 0f;
			for (int i = 0; i < VisibleRowProperties.Count; i++)
				captionsWidth += (float)GetCaptionWidth(graphics, VisibleRowProperties[i]);
			return captionsWidth + ((VisibleRowProperties.Count - 1) * separatorInfo.HeaderSeparatorWidth);
		}
		protected override void CalcRowCaptionsInfo(BaseViewInfo vi) {
			CalcSeparatorWidth(vi.Graphics, vi.RC.separatorWidth);
			Rectangle[] rects;
			if(DraftCalculation)
				return;
			if(GetNeedArrangeCaptions(Row)) {
				rects = FitRects(HeaderCellsRect);
				SaveWidth(rects);
				SetNeedArrangeCaptions(Row, false);
			} else {
				RectScaleScrollerArgs args = new RectScaleScrollerArgs(HeaderCellsRect, Row, 0, 0, separatorInfo.HeaderSeparatorWidth, false);
				rects = CalcCellRects(args);
			}
			if(rects != null) {
				for (int i = 0; i < rects.Length; i++) {
					Rectangle r = rects[i];
					if (!r.IsEmpty) {
						RowCaptionInfo rc = CalcCaptionInfo(VisibleRowProperties[i], r, vi);
						rc.SetRowCellIndex(VisibleRowProperties[i].CellIndex);
						CaptionsInfo.Add(rc);
					}
				}
				if(rects.Length > 0)
					AddRightVertLine(HeaderCellsRect, vi);
				CalcSeparatorRects(ref rects);
			}
		}
		void SaveWidth(Rectangle[] rects) {
			for(int i = 0; i < VisibleRowProperties.Count; i++)
				((MultiEditorRowProperties)VisibleRowProperties[i]).SetWidth(rects[i].Width);
		}
		Rectangle[] FitRects(Rectangle headerCellsRect) {
			if (VisibleRowProperties.Count == 0)
				return new Rectangle[] { headerCellsRect };
			Rectangle[] rects = new Rectangle[VisibleRowProperties.Count];
			Point leftTop = headerCellsRect.Location;
			for(int i = 0; i < rects.Length; i++) {
				rects[i] = new Rectangle(leftTop, new Size(GetCaptionWidth(Grid.ViewInfo.RC.Graphics, VisibleRowProperties[i]), HeaderCellsRect.Height));
				leftTop.X = rects[i].Right + separatorInfo.HeaderSeparatorWidth;
			}
			int appendix = (headerCellsRect.Width - (int)GetCaptionsWidth()) / rects.Length;
			ResizeRects(appendix, ref rects);
			int rightLug = headerCellsRect.Right - rects[rects.Length - 1].Right;
			if(rightLug > 0)
				rects[rects.Length - 1].Width += rightLug;
			return rects;
		}
		void ResizeRects(int diff, ref Rectangle[] rects) {
			if(diff == 0)
				return;
			int factor = 0;
			for(int i = 0; i < rects.Length; i++){
				rects[i].X += diff * factor;
				rects[i].Width += diff;
				factor++;
			}
		}
		protected override int GetTextWidth(Graphics graphics, string caption) {
			return Math.Max(base.GetTextWidth(graphics, caption), MultiEditorRowProperties.MinWidth);
		}
		protected override void CalcCaptionAppearance(RowCaptionInfo captionInfo, BaseViewInfo vi, RowProperties properties) {
			AppearanceObject[] styles = GetRowHeaderAppearanceList(vi, true, properties, null);
			AppearanceHelper.Combine(captionInfo.Style, styles);
		}
		Rectangle[] CalcCellRects(RectScaleScrollerArgs args) {
			if(args.Bounds.Width == 0 || !Grid.OptionsView.ShowRows || VisibleRowProperties[0] == MultiEditorRowViewInfo.NullProperties) {
				Rectangle[] result = new Rectangle[Math.Max(1, VisibleRowProperties.Count)];
				for(int i = 0; i < result.Length; i++)
					result[i] = args.Bounds;
				return result;
			}
			RectScaleScroller.Instance.CalcCellRects(args);
			return args.Rects;
		}
		protected virtual void CalcSeparatorRects(ref Rectangle[] headerCells) {
			SeparatorRects.Clear();
			MultiEditorRowViewInfo.AddSeparatorRects(SeparatorRects, separatorInfo.SeparatorKind, ref headerCells, Grid.ViewInfo.IsRightToLeft);
		}
		protected internal override void CalcSeparatorWidth(Graphics g, int lineSeparatorWidth) {
			switch(separatorInfo.SeparatorKind) {
				case SeparatorKind.VertLine:
					separatorInfo.ValueSeparatorWidth = lineSeparatorWidth;
					separatorInfo.HeaderSeparatorWidth = lineSeparatorWidth;
					break;
				case SeparatorKind.String:
					separatorInfo.ValueSeparatorWidth = BaseViewInfo.CalcTextSize(Style, g, separatorInfo.SeparatorString, 0).Width + 4;
					separatorInfo.HeaderSeparatorWidth = Math.Min(HeaderSeparatorMaxWidth, separatorInfo.ValueSeparatorWidth);
					break;
			}
		}
		int HeaderSeparatorMaxWidth {
			get {
				if(Row.PropertiesCollection.Count < 2) return 0;
				int sumMinCellsWidth = Row.PropertiesCollection.Count * MultiEditorRowProperties.MinWidth;
				return (HeaderCellsRect.Width - sumMinCellsWidth) / (Row.PropertiesCollection.Count - 1);
			}
		}
		protected new MultiEditorRow Row { get { return (MultiEditorRow)base.Row; } }
		protected internal override float CalculateBestFit() {
			SetNeedArrangeCaptions(Row, true);
			return base.CalculateBestFit();
		}
	}
	public class MultiEditorRowViewInfo : EditorRowViewInfo {
		internal ArrayList SeparatorRects;
		static MultiEditorRowProperties nullProperties = new MultiEditorRowProperties();
		public int FirstRowCellIndex, Offset;
		public MultiEditorRowViewInfo(BaseRow row) : base(row) {
			SeparatorRects = new ArrayList();
			FirstRowCellIndex = Offset = 0;
		}
		protected override void CalcValuesInfo(BaseViewInfo vi, BaseRow nextRow) {
			ValuesInfo.Clear();
			SeparatorRects.Clear();
			int sepWidth = HeaderInfo.SeparatorInfo.ValueSeparatorWidth;
			for(int i = 0; i < vi.VisibleValuesCount; i++) {
				RectScaleScrollerArgs args = new RectScaleScrollerArgs(vi.GetValueRect(i, this), Row, FirstRowCellIndex, Offset, sepWidth, true);
				Rectangle[] rects = CalcCellRects(args);
				FirstRowCellIndex = args.FirstCellIndex;
				Offset = args.Offset;
				if(rects != null) {
					for(int j = 0; j < rects.Length; j++) {
						Rectangle r = rects[j];
						if(!r.IsEmpty) {
							RowValueInfo rv = CalcRowValueInfo(GetRowProperties(FirstRowCellIndex + j), r, vi, vi.FirstVisibleRecordIndex + i);
							ValuesInfo.Add(rv);
						}
					}
					AddSeparatorRects(SeparatorRects, HeaderInfo.SeparatorInfo.SeparatorKind, ref rects, vi.IsRightToLeft);
				}
				AddRectValueLines(args.Bounds, vi, i == vi.FirstVisibleRecordIndex, i == vi.VisibleValuesCount - 1, nextRow);
			}
		}
		internal protected override RowProperties GetRowProperties(int index) {
			if(Row.PropertiesCollection.Count == 0)
				return NullProperties;
			return base.GetRowProperties(index);
		}
		Rectangle[] CalcCellRects(RectScaleScrollerArgs args) {
			if(Row.RowPropertiesCount == 0)
				return new Rectangle[] {args.Bounds };
			RectScaleScroller.Instance.CalcCellRects(args);
			return args.Rects;
		}
		internal protected override AppearanceObject[] GetRowValueAppearanceList(BaseViewInfo vi, RowValueInfo rv) {
			AppearanceObject[] result = GetRowValueAppearanceListCore(vi, rv);
			if(DrawReadOnly(rv)) {
				result = MemoryUtils.IncreaseFromHead(result, 1);
				result[0] = vi.PaintAppearance.ReadOnlyRecordValue;
			}
			if(!IsRowCellDefaultValue(rv)) {
				result = MemoryUtils.IncreaseFromHead(result, 1);
				result[0] = vi.PaintAppearance.ModifiedRecordValue;
			}
			return result;
		}
		protected override bool IsRowCellFocused(RowValueInfo rv) {
			if(Row.RowPropertiesCount == 0) return false;
			return base.IsRowCellFocused(rv);
		}
		protected internal virtual AppearanceObject GetMultiEditorValuesSeparatorStyle(RowValueInfo rv, AppearanceObject style, VGridAppearanceCollection paintAppearance) {
			if(rv.Focused) {
				if(Row.Enabled) return paintAppearance.FocusedRecord;
				style = new AppearanceObject(paintAppearance.DisabledRecordValue, paintAppearance.FocusedRecord);
			}
			return style;
		}
		protected internal override VGridHitTest CalcHeaderHitTest(Point pt, VGridHitTest ht) {
			if(CanResizeHeaders) {
				for(int i = 0; i < HeaderInfo.SeparatorRects.Count; i++) {
					Rectangle r = CheckHitTestSeparatorWidth((Rectangle)HeaderInfo.SeparatorRects[i]);
					if(r.Contains(pt)) {
						ht.HitInfoType = HitInfoTypeEnum.MultiEditorCellSeparator;
						ht.CaptionInfo = (RowCaptionInfo)HeaderInfo.CaptionsInfo[i];
						return ht;
					}
				}
			}
			return base.CalcHeaderHitTest(pt, ht);
		}
		protected internal override VGridHitTest CalcValuesHitTest(Point pt, VGridHitTest ht) {
			if(CanResizedValues) {
				for(int i = 0; i < SeparatorRects.Count; i++) {
					Rectangle r = CheckHitTestSeparatorWidth((Rectangle)SeparatorRects[i]);
					if(r.Contains(pt)) {
						ht.HitInfoType = HitInfoTypeEnum.MultiEditorCellSeparator;
						int groupCellsInd = i / (Row.RowPropertiesCount - 1);
						ht.ValueInfo = ValuesInfo[i + groupCellsInd];
						return ht;
					}
				}
			}
			return base.CalcValuesHitTest(pt, ht);
		}
		protected virtual Rectangle CheckHitTestSeparatorWidth(Rectangle r) {
			if(r.Width < 4) {
				r.X -= (4 - r.Width) / 2;
				r.Width = 4;
			}
			return r;
		}
		static internal void AddSeparatorRects(ArrayList sepRects, SeparatorKind sepKind, ref Rectangle[] valueCells, bool isRightToLeft) {
			if(isRightToLeft) {
				for(int i = valueCells.Length - 2; i >= 0; i--)
					sepRects.Add(new Rectangle(valueCells[i].Left - 1, valueCells[i].Top,
						valueCells[i].Left - valueCells[i + 1].Right, valueCells[i].Height));
			}
			else {
				for(int i = 0; i < valueCells.Length - 1; i++)
					sepRects.Add(new Rectangle(valueCells[i].Right, valueCells[i].Top,
						valueCells[i + 1].Left - valueCells[i].Right, valueCells[i].Height));
			}
		}
		protected new MultiEditorRow Row { get { return (MultiEditorRow)base.Row; } }
		internal new MultiEditorRowHeaderInfo HeaderInfo { get { return (MultiEditorRowHeaderInfo)base.HeaderInfo; } }
		protected internal static MultiEditorRowProperties NullProperties { get { return nullProperties; } }
	}
	public class RowCaptionInfo : RowCellInfo {
		bool isCaptionFit;
		public Rectangle CaptionRect, CaptionTextRect, FilterButtonRect, SortShapeRect, ImageRect, FocusRect;
		public bool Focused, Pressed, HotTrack;
		public int ImageIndex;
		public string Caption;
		internal bool IsCaptionFit { get { return isCaptionFit; } set { isCaptionFit = value; } }
		public RowCaptionInfo() {
			this.CaptionRect = this.CaptionTextRect = this.FilterButtonRect = 
				this.SortShapeRect = this.ImageRect = this.FocusRect = Rectangle.Empty;
			this.Focused = this.Pressed = this.HotTrack = false;
			this.ImageIndex = -1;
			this.Caption = string.Empty;
			this.isCaptionFit = false;
		}
		public bool AllowGlyphSkinning { get; set; }
		public bool AllowHtmlText { get; set; }
	}
	public class RowValueInfo : RowCellInfo {
		BaseEditViewInfo editorViewInfo;
		public bool Focused, DrawFocusFrame;
		int recordIndex;
		public Rectangle Bounds;
		public BaseRowViewInfo RowViewInfo;
		public RowValueInfo(BaseRowViewInfo rowViewInfo) {
			this.editorViewInfo = null;
			this.Focused = this.DrawFocusFrame = false;
			this.recordIndex = -1;
			this.Bounds = Rectangle.Empty;
			this.RowViewInfo = rowViewInfo;
		}
		public BaseEditViewInfo EditorViewInfo { get { return editorViewInfo; } }
		public RepositoryItem Item { get { return EditorViewInfo.Item; } }
		public RowProperties Properties { get { return RowViewInfo != null ? RowViewInfo.GetRowProperties(RowCellIndex) : null; } }
		public int RecordIndex { get { return recordIndex; } }
		internal bool IsValidErrorIconText { get { return IsValidErrorIconTextInternal(EditorViewInfo.ErrorIconText); } }
		static internal bool IsValidErrorIconTextInternal(string errorText) {
			return !string.IsNullOrEmpty(errorText);
		}
		internal void RecalcViewInfo(Point point) {
			CalcViewInfo(true, point);
		}
		internal void CalcViewInfo() {
			CalcViewInfo(false, Point.Empty);
		}
		void CalcViewInfo(bool recalc, Point point) {
			if(Row == null || Row.Grid == null)
				return;
			Row.Grid.UpdateEditViewInfo(Properties, EditorViewInfo, RecordIndex);
			Row.Grid.UpdateEditViewInfoData(EditorViewInfo, Properties, RecordIndex);
			DevExpress.XtraEditors.DXErrorProvider.ErrorInfo errorInfo = Row.Grid.GetRowErrorInfo(Properties, RecordIndex);
			EditorViewInfo.ErrorIconText = errorInfo.ErrorText;
			EditorViewInfo.ShowErrorIcon = IsValidErrorIconTextInternal(errorInfo.ErrorText);
			if(EditorViewInfo.ShowErrorIcon)
				EditorViewInfo.ErrorIcon = DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider.GetErrorIconInternal(errorInfo.ErrorType);
			if(recalc) {
				EditorViewInfo.CalcViewInfo(null, MouseButtons.None, point, EditorViewInfo.Bounds);
			} else {
				EditorViewInfo.CalcViewInfo(null);
			}
		}
		internal void SetEditorViewInfo(BaseEditViewInfo editorViewInfo) {
			this.editorViewInfo = editorViewInfo;
		}
		public void SetRecordIndex(int recordIndex) {
			this.recordIndex = recordIndex;
		}
		internal void CalcAppearance(BaseViewInfo viewInfo) {
			AppearanceHelper.Combine(Style, RowViewInfo.GetRowValueAppearanceList(viewInfo, this));
			SetStyle(viewInfo.Grid.InternalRecordCellStyle(this));
			if(Style.HAlignment == HorzAlignment.Default)
				Style.TextOptions.HAlignment = viewInfo.Grid.ContainerHelper.GetDefaultValueAlignment(EditorViewInfo.Item, Properties == null ? typeof(string) : Properties.RowType);
		}
	}
	public class RowCellInfo {
		BaseRow row;
		AppearanceObject style;
		int cellIndex;
		string toolTip;
		public RowCellInfo() : this(null, 0) {}
		public RowCellInfo(BaseRow row, int rowCellIndex) {
			SetRow(row);
			SetRowCellIndex(rowCellIndex);
			this.style = new AppearanceObject();
			this.toolTip = string.Empty;
		}
		public BaseRow Row { get { return this.row; } }
		public int RowCellIndex { get { return this.cellIndex; } }
		public AppearanceObject Style { get { return style; } }
		public string ToolTip { get { return toolTip; } }
		public void SetRow(BaseRow row) {
			this.row = row;
		}
		public void SetRowCellIndex(int cellIndex) {
			this.cellIndex = cellIndex;
		}
		public void SetStyle(AppearanceObject style) {
			this.style = style;
		}
		public void SetToolTip(string toolTip) {
			this.toolTip = toolTip;
		}
	}
	public class VGridResourceCache : IDisposable {
		public bool NeedsUpdate;
		public Brush RowHorzLineBrush,
			RowVertLineBrush,
			CategoryHorzLineBrush,
			CategoryVertLineBrush,
			BandBorderBrush,
			FixedLineBrush;
		public Graphics Graphics;
		public int HorzLineWidth, VertLineWidth, separatorWidth;
		public Size imageSize, treeButSize;
		private VGridScrollStylesController scrollStyles;
		public Size ExplorerButtonCollapsedSize { get; private set; }
		public Size ExplorerButtonExpandedSize { get; private set; }
		public VGridResourceCache () {
			this.NeedsUpdate = true;
			SetBrushesToNull();
			this.Graphics = Graphics.FromHwnd(IntPtr.Zero);
			this.HorzLineWidth = this.VertLineWidth = 0;
			this.separatorWidth = 0;
			this.imageSize = treeButSize = Size.Empty;
		}
		public virtual void Dispose() {
			if(Graphics != null) {
				Graphics.Dispose();
				Graphics = null;
			}
			DestroyBrushes();
		}
		void DestroyBrushes() {
			if(RowHorzLineBrush == null) return;
			RowHorzLineBrush.Dispose();
			RowVertLineBrush.Dispose();
			CategoryHorzLineBrush.Dispose();
			CategoryVertLineBrush.Dispose();
			BandBorderBrush.Dispose();
			FixedLineBrush.Dispose();
			SetBrushesToNull();
		}
		void SetBrushesToNull() {
			RowHorzLineBrush = RowVertLineBrush = CategoryHorzLineBrush = CategoryVertLineBrush = BandBorderBrush = FixedLineBrush = null;
		}
		public Color ScrollBarColor { get { return scrollStyles.BackColor; } }
		public void CheckUpdate(VGridControlBase grid) {
			if(!NeedsUpdate) return;
			CreateBrushes(grid);
			CalcLayoutSizes(grid);
			CalcParamValues(grid);
			scrollStyles = grid.ScrollsStyle;
			NeedsUpdate = false;
		}
		private void CreateBrushes(VGridControlBase grid) {
			DestroyBrushes();
			RowHorzLineBrush = CreateLineBrush(grid.ViewInfo.PaintAppearance.HorzLine);
			RowVertLineBrush = CreateLineBrush(grid.ViewInfo.PaintAppearance.VertLine);
			CategoryHorzLineBrush = CreateLineBrush(grid.ViewInfo.PaintAppearance.Category.BorderColor);
			CategoryVertLineBrush = CreateLineBrush(grid.ViewInfo.PaintAppearance.Category.BorderColor);
			BandBorderBrush = CreateLineBrush(grid.ViewInfo.PaintAppearance.BandBorder);
			FixedLineBrush = CreateLineBrush(grid.ViewInfo.PaintAppearance.FixedLine);
		}
		Brush CreateLineBrush(AppearanceObject lineStyle) {
			if(lineStyle.BackColor.IsEmpty) return CreateLineBrush(lineStyle.BackColor2);
			if(lineStyle.BackColor2.IsEmpty) return CreateLineBrush(lineStyle.BackColor);
			return new HatchBrush(HatchStyle.Percent50, lineStyle.BackColor, lineStyle.BackColor2);
		}
		Brush CreateLineBrush(Color color) { return new SolidBrush(color); }
		void CalcLayoutSizes(VGridControlBase grid) {
			HorzLineWidth = (grid.OptionsView.ShowHorzLines ? 1 : 0);
			VertLineWidth = (grid.OptionsView.ShowVertLines ? 1 : 0);
			imageSize = ImageCollection.GetImageListSize(grid.ImageList);
			ExplorerButtonExpandedSize = grid.Painter.PaintStyle.ExplorerButtonExpandSize;
			ExplorerButtonCollapsedSize = grid.Painter.PaintStyle.ExplorerButtonCollapseSize;
		}
		void CalcParamValues(VGridControlBase grid) {
			PaintStyleParams pars = grid.PaintStyleParams;
			treeButSize = grid.Painter.PaintStyle.OpenCloseButtonPainter.CalcObjectMinBounds(new OpenCloseButtonInfoArgs(null, new Rectangle(Point.Empty, pars.TreeButClientSize), true,
				grid.ViewInfo.PaintAppearance.ExpandButton, ObjectState.Normal)).Size;
			separatorWidth = pars.SeparatorWidth;
		}
		internal int CalcMinHeight(AppearanceObject style) {
			if(style == null) return 0;
			int height = BaseViewInfo.CalcTextSize(style, Graphics, "Wg", 0).Height;
			return height;
		}
		public virtual int GetExpandCollapseButtonHeight(BaseRow row) {
			if(row.TreeButtonType == TreeButtonType.ExplorerBarButton)
				return Math.Max(ExplorerButtonCollapsedSize.Height, ExplorerButtonExpandedSize.Height);
			return treeButSize.Height;
		}
	}
	public class VGridCellToolTipInfo {
		int record;
		BaseRow row;
		object cellObject;
		public VGridCellToolTipInfo(int record, BaseRow row, object cellObject) {
			this.row = row;
			this.record = record;
			this.cellObject = cellObject;
		}
		public BaseRow Row { get { return row; } }
		public int Record { get { return record; } }
		public object CellObject { get { return cellObject; } }
		public override bool Equals(object obj) {
			VGridCellToolTipInfo info = obj as VGridCellToolTipInfo;
			if(info == null) return false;
			return info.Row == this.Row && info.Record == this.Record &&
				Object.Equals(info.CellObject, this.CellObject);
		}
		public override int GetHashCode() {
			return string.Format("{0},{1},{2}", Record, Row == null ? 0 : Row.GetHashCode(), this.CellObject == null ? 0 : this.CellObject.GetHashCode()).GetHashCode();
		}
	}
	public class RowViewInfoReadOnlyCollection : ReadOnlyCollectionBase {
		static IComparer comparer = new RowViewInfoComparerAdapter(new FixedRowsPaintOrderComparer());
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual BaseRowViewInfo this[int i] { get { return (BaseRowViewInfo)InnerList[i]; } }
		protected internal virtual IComparer Comparer { get { return comparer; } set { comparer = value; } }
		protected internal virtual void Add(BaseRowViewInfo rowViewInfo) {
			InnerList.Add(rowViewInfo);
		}
		protected internal virtual void Sort() {
			InnerList.Sort(Comparer);
		}
	}
	public class VGridViewInfoHelper {
		public virtual bool DrawReadOnly(RowValueInfo cell) {
			if (cell == null || cell.Properties == null)
				return true;
			return cell.Properties.GetReadOnly();
		}
		public virtual bool IsReadOnly(RowValueInfo cell, VGridControlBase grid) {
			if (grid == null || cell == null || cell.Properties == null)
				return true;
			return cell.Properties.GetReadOnly();
		}
		public virtual bool DrawHeaderReadOnly(RowProperties properties, int recordIndex, VGridControlBase grid) {
			if(grid == null || properties == null)
				return true;
			return properties.GetReadOnly();
		}
	}
	public class PGridViewInfoHelper : VGridViewInfoHelper {
		PropertyGridControl grid;
		public PGridViewInfoHelper(PropertyGridControl grid) {
			this.grid = grid;
		}
		public PropertyGridControl Grid { get { return grid; } }
		public override bool DrawReadOnly(RowValueInfo valueInfo) {
			if(valueInfo.Properties == null)
				return true;
			if (valueInfo.Properties.ReadOnly.HasValue)
				return valueInfo.Properties.ReadOnly.Value;
			if (!Grid.IsDefault(valueInfo.Item)) {
				return valueInfo.Item.ReadOnly;
			}
			if(valueInfo.Properties != null)
				return valueInfo.Properties.RenderReadOnly;
			return true;
		}
		public override bool IsReadOnly(RowValueInfo cell, VGridControlBase grid) {
			if (grid == null || cell == null || cell.Properties == null)
				return true;
			if (cell.Properties.ReadOnly.HasValue)
				return cell.Properties.ReadOnly.Value;
			return cell.Item.ReadOnly;
		}
		public override bool DrawHeaderReadOnly(RowProperties properties, int recordIndex, VGridControlBase grid) {
			if(grid == null || properties == null)
				return true;
			if (properties.ReadOnly.HasValue)
				return properties.ReadOnly.Value;
			RepositoryItem item = grid.GetRowEdit(properties, recordIndex);
			if (!Grid.IsDefault(item)) {
				return item.ReadOnly;
			}
			return properties.RenderReadOnly;
		}
	}
	public class RepositoryItemDictionary {
		Dictionary<GridCell, RepositoryItem> dictionary = new Dictionary<GridCell,RepositoryItem>();
		public Dictionary<GridCell, RepositoryItem>.ValueCollection Values { get { return dictionary.Values; } }
		public bool TryGetValue(GridCell cell, out RepositoryItem item) {
			return dictionary.TryGetValue(cell, out item);
		}
		public void Add(GridCell cell, RepositoryItem item) {
			dictionary.Add(cell, item);
		}
		public void Remove(BaseRow row) {
			List<GridCell> listToRemove = new List<GridCell>();
			foreach(GridCell cell in dictionary.Keys) {
				if(cell.Row == row) {
					listToRemove.Add(cell);
				}
			}
			foreach(GridCell cell in listToRemove) {
				dictionary.Remove(cell);
			}
		}
	}
}
