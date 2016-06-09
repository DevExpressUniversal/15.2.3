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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors.Helpers;
namespace DevExpress.XtraTreeList.Painter {
	public class TreeListDrawInfo : GraphicsInfoArgs {
		TreeListViewInfo viewInfo;
		bool isRightToLeftCore;
		public TreeListDrawInfo(GraphicsCache graphicsCache, TreeListViewInfo viewInfo, Rectangle bounds, bool isRightToLeft)
			: base(graphicsCache, bounds) {
			this.viewInfo = viewInfo;
			this.isRightToLeftCore = isRightToLeft;
		}
		public virtual TreeListViewInfo ViewInfo { get { return viewInfo; } }
		public bool IsRightToLeft { get { return isRightToLeftCore; } }
	}
	public class TreeListPainter : IDisposable {
		Rectangle paintClipRect;
		DevExpress.Utils.Paint.Clipping clip;
		ImageCollection indicatorImageList;
		ImageCollection nodeDragImageList;
		ITreeListPaintHelper defaultPaintHelper;
		PaintLink paintLink;
		TreeListDrawInfo drawInfo;
		ElementPainters elementPainters;
		internal CellInfo drawnCell;
		protected internal TreeListPainter() {
			this.clip = new DevExpress.Utils.Paint.Clipping();
			this.paintClipRect = Rectangle.Empty;
		}
		public virtual void Dispose() {
			if(indicatorImageList != null)
				indicatorImageList.Dispose();
			if(nodeDragImageList != null)
				nodeDragImageList.Dispose();
		}
		protected virtual TreeListDrawInfo CreateDrawInfo(GraphicsCache cache, TreeListViewInfo viewInfo, Rectangle bounds, bool rightToLeft) {
			return new TreeListDrawInfo(cache, viewInfo, bounds, rightToLeft);
		}
		protected internal virtual void DoDraw(TreeListViewInfo viewInfo, DXPaintEventArgs e) {
			paintClipRect = e.ClipRectangle;
			ResourceCache.DefaultCache.CheckCache();
			this.drawInfo = CreateDrawInfo(new GraphicsCache(e), viewInfo, viewInfo.ViewRects.Window, viewInfo.TreeList.IsRightToLeft);
			XPaint.Graphics.BeginPaint(DrawInfo.Graphics);
			GraphicsClipState clipState = null;
			try {
				if(!viewInfo.PaintAnimatedItems) {
					clipState = drawInfo.Cache.ClipInfo.SaveAndSetClip(viewInfo.ViewRects.Window, true, true);
					DrawBorder(viewInfo.PaintAppearance.HorzLine, viewInfo.ViewRects.Window);
					DrawCaption();
					DrawBandPanel();
					DrawColumns();
					DrawEmptyArea();
				}
				DrawRows();
				DrawFilterPanel();
				if(!viewInfo.PaintAnimatedItems) {
					DrawSummaryFooter(viewInfo.SummaryFooterInfo);
				}
			}
			finally {
				if(clipState != null)
					drawInfo.Cache.ClipInfo.RestoreClipRelease(clipState);
				XPaint.Graphics.EndPaint(DrawInfo.Graphics);
				Cache.Dispose();
				this.drawInfo = null;
				paintClipRect = Rectangle.Empty;
			}
		}
		protected virtual void DrawCaption() {
			if(!NeedsRedraw(DrawInfo.ViewInfo.ViewRects.Caption)) return;
			StyleObjectInfoArgs e = new StyleObjectInfoArgs(DrawInfo.Cache, DrawInfo.ViewInfo.ViewRects.Caption, DrawInfo.ViewInfo.PaintAppearance.Caption.Clone() as AppearanceObject, ObjectState.Normal);
			ElementPainters.CaptionPainter.DrawObject(e);
		}
		protected virtual void DrawBandPanel() {
			Rectangle bandPanelRect = DrawInfo.ViewInfo.ViewRects.ActualBandPanel;
			if(!NeedsRedraw(bandPanelRect)) return;
			DrawInfo.ViewInfo.PaintAppearance.HeaderPanelBackground.DrawBackground(DrawInfo.Cache, bandPanelRect);
			TreeListViewInfo viewInfo = drawInfo.ViewInfo;
			if(viewInfo.BandsInfo.Count > 0) {
				BandInfo bi = viewInfo.BandsInfo[0];
				if(bi.Type == ColumnInfo.ColumnInfoType.ColumnButton) 
					DrawBand(bi);
			}
			GraphicsClipState clipState = null;
			if(viewInfo.ViewRects.IndicatorWidth > 0) {
				Rectangle clipR = viewInfo.ViewRects.ActualBandPanel;
				if(!DrawInfo.IsRightToLeft)
					clipR.X += viewInfo.ViewRects.IndicatorWidth;
				clipR.Width -= viewInfo.ViewRects.IndicatorWidth;
				clipState = DrawInfo.Cache.ClipInfo.SaveAndSetClip(clipR, true, true);
			}
			foreach(BandInfo bi in viewInfo.BandsInfo) {			   
				if(bi.Type == ColumnInfo.ColumnInfoType.ColumnButton) continue;
				Rectangle rect = bi.Bounds;
				rect.Height = viewInfo.ViewRects.BandPanel.Height;
				TreeListClipInfo cli = PrepareBandClipInfo(DrawInfo, bi, rect);
				if(cli.ShouldPaint)
					DrawBand(bi);
				Rectangle fixedRect = Rectangle.Empty;
				if(bi.Band != null) {
					if(bi.Band == viewInfo.FixedLeftBand) {
						fixedRect = DrawInfo.IsRightToLeft ?
							CalcFixedRightLine(viewInfo.ViewRects.FixedLeft) :
							CalcFixedLeftLine(bi);
						fixedRect.Height = viewInfo.ViewRects.BandPanel.Height;
					}
					if(bi.Band == viewInfo.FixedRightBand) {
						fixedRect = DrawInfo.IsRightToLeft ?
							CalcFixedLeftLine(bi) :
							CalcFixedRightLine(viewInfo.ViewRects.FixedRight);
						fixedRect.Height = viewInfo.ViewRects.BandPanel.Height;
					}
				}
				if(!fixedRect.IsEmpty) {
					AppearanceObject appearance = DrawInfo.ViewInfo.PaintAppearance.FixedLine;
					Cache.Paint.FillRectangle(Cache.Graphics, appearance.GetBackBrush(Cache), fixedRect);
				}
				cli.Restore(DrawInfo);
			}
			DrawInfo.Cache.ClipInfo.RestoreClipRelease(clipState);
		}
		protected TreeListClipInfo PrepareBandClipInfo(TreeListDrawInfo e, BandInfo bi, Rectangle bounds) {
			TreeListClipInfo cli = new TreeListClipInfo(Cache);
			cli.PrepareBand(e, bi, bounds);
			return cli;
		}
		protected virtual void DrawBand(BandInfo bi) {
			DrawBandCore(DrawInfo.Cache, bi);
			if(bi.HasChildren) {
				foreach(BandInfo child in bi.Bands)
					DrawBand(child);
			}
		}
		protected virtual void DrawBandCore(GraphicsCache cache, BandInfo bi) {
			bi.Cache = cache;
			Rectangle r = bi.Bounds;
			try {
				if(!bi.ActualBounds.IsEmpty)
					bi.Bounds = bi.ActualBounds;
				bi.DesignTimeSelected = PaintLink.IsBandSelected(bi);
				CustomDrawBandHeaderEventArgs e = new CustomDrawBandHeaderEventArgs(cache, ElementPainters.BandPainter, bi, bi.RightToLeft);
				e.SetDefaultDraw(() => { DefaultPaintHelper.DrawBand(e); });
				PaintLink.DrawBand(e);
				e.DefaultDraw();
			}
			finally {
				bi.Bounds = r;
				bi.Cache = null;
			}
		}
		protected TreeListClipInfo PrepareClipInfo(TreeListDrawInfo e, ColumnInfo ci, Rectangle bounds) {
			TreeListClipInfo cli = new TreeListClipInfo(Cache);
			cli.Prepare(e, ci, bounds);
			return cli;
		}
		Rectangle CalcFixedLeftLine(IHeaderObjectInfo hi) {
			Rectangle res = hi.Bounds;
			res.X = hi.Bounds.Right;
			res.Width = DrawInfo.ViewInfo.TreeList.FixedLineWidth;
			return res;
		}
		Rectangle CalcFixedRightLine(Rectangle columnBounds) {
			Rectangle res = columnBounds;
			res.Width = DrawInfo.ViewInfo.TreeList.FixedLineWidth;
			return res;
		}
		protected virtual void DrawColumns() {
			if(!NeedsRedraw(DrawInfo.ViewInfo.ViewRects.ActualColumnPanel)) return;
			int i = 0;
			if(DrawInfo.ViewInfo.TreeList.ActualShowBands || DrawInfo.ViewInfo.TreeList.HasBands) 
				DrawInfo.ViewInfo.PaintAppearance.HeaderPanelBackground.DrawBackground(DrawInfo.Cache, DrawInfo.ViewInfo.ViewRects.ActualColumnPanel);
			if(DrawInfo.ViewInfo.ViewRects.IndicatorWidth != 0) {
				ColumnInfo indicatorColumn = DrawInfo.ViewInfo.ColumnsInfo.Columns[i++] as ColumnInfo;
				DrawColumn(Cache, indicatorColumn);
			}
			GraphicsClipState clipState = null;
			if(DrawInfo.ViewInfo.ViewRects.IndicatorWidth > 0) {
				Rectangle clipr = DrawInfo.ViewInfo.ViewRects.ActualColumnPanel;
				if(!DrawInfo.IsRightToLeft)
					clipr.X += DrawInfo.ViewInfo.ViewRects.IndicatorWidth;
				clipr.Width -= DrawInfo.ViewInfo.ViewRects.IndicatorWidth;
				clipState = Cache.ClipInfo.SaveAndSetClip(clipr, true, true);
			}
			try {
				for(; i < DrawInfo.ViewInfo.ColumnsInfo.Columns.Count; i++) {
					ColumnInfo ci = DrawInfo.ViewInfo.ColumnsInfo.Columns[i] as ColumnInfo;
					TreeListClipInfo cli = PrepareClipInfo(DrawInfo, ci, ci.Bounds);
					if(cli.ShouldPaint)
						DrawColumn(Cache, ci);
					Rectangle r = Rectangle.Empty;
					if(DrawInfo.ViewInfo.IsFixedLeft(ci)) {
						r = DrawInfo.IsRightToLeft ?
							CalcFixedRightLine(DrawInfo.ViewInfo.ViewRects.FixedLeft) :
							CalcFixedLeftLine(ci);
					}
					if(DrawInfo.ViewInfo.IsFixedRight(ci)) {
						r = DrawInfo.IsRightToLeft ?
							CalcFixedLeftLine(ci) :
							CalcFixedRightLine(DrawInfo.ViewInfo.ViewRects.FixedRight);
						Rectangle fleft = DrawInfo.ViewInfo.ViewRects.FixedLeft;
						bool checkIntersection = DrawInfo.IsRightToLeft ? fleft.X <= r.Right : fleft.Right >= r.X;
						if(!fleft.IsEmpty && checkIntersection) {
							if(DrawInfo.IsRightToLeft) r.Width -= r.Right - fleft.X;
							else {
								int delta = fleft.Right - r.X;
								r.X += delta;
								r.Width -= delta;
							}
							if(r.Width < 1) r = Rectangle.Empty;
						}
					}
					if(!r.IsEmpty) {
						AppearanceObject appearance = DrawInfo.ViewInfo.PaintAppearance.FixedLine;
						Cache.Paint.FillRectangle(Cache.Graphics, appearance.GetBackBrush(Cache), r);
					}
					cli.Restore(DrawInfo);
				}
				if(!DrawInfo.ViewInfo.ColumnDragFrameRect.IsEmpty) {
					using(Pen pen = new Pen(GetColumnDragFrameColor(), 2)) {
						Cache.Paint.DrawRectangle(Cache.Graphics, pen, DrawInfo.ViewInfo.ColumnDragFrameRect);
					}
				}
			}
			finally {
				Cache.ClipInfo.RestoreClipRelease(clipState);
			}
		}
		protected internal virtual Color GetColumnDragFrameColor() { return Color.Black; }
		protected internal virtual void DrawColumn(GraphicsCache cache, ColumnInfo ci) {
			if(ci.Type == ColumnInfo.ColumnInfoType.EmptyColumn) return;
			Rectangle r = ci.Bounds;
			try {
				if(!ci.ActualBounds.IsEmpty)
					ci.Bounds = ci.ActualBounds;
				CustomDrawColumnHeaderEventArgs e = new CustomDrawColumnHeaderEventArgs(cache, ci.Bounds, ci.Appearance, ci.RightToLeft);
				e.Init(ci, ElementPainters.HeaderPainter, PaintLink.IsColumnSelected(ci));
				e.SetDefaultDraw(() => DefaultPaintHelper.DrawColumn(e));
				PaintLink.DrawColumnHeader(e);
				e.DefaultDraw();
			}
			finally {
				ci.Bounds = r;
				ci.Cache = null;
			}
		}
		Rectangle GetIndicatorClipBounds(TreeListViewInfo viewInfo) {
			int x = DrawInfo.IsRightToLeft ? viewInfo.ViewRects.Rows.Right - viewInfo.ViewRects.IndicatorWidth : viewInfo.ViewRects.Rows.Left;
			return new Rectangle(x, viewInfo.ViewRects.TotalRows.Top, viewInfo.ViewRects.IndicatorWidth, viewInfo.ViewRects.TotalRows.Height);
		}
		internal void DrawRows() {
			TreeListViewInfo viewInfo = DrawInfo.ViewInfo;
			RowInfo autoFilterRowInfo = viewInfo.AutoFilterRowInfo;
			if(NeedsRedraw(viewInfo.ViewRects.TotalRows)) {
				if(viewInfo.ViewRects.IndicatorWidth != 0) {
					Rectangle indicators = GetIndicatorClipBounds(viewInfo);
					if(!viewInfo.PaintAnimatedItems) {
						if(NeedsRedraw(indicators)) {
							clip.SetClip(DrawInfo, indicators);
							if(autoFilterRowInfo != null) {
								viewInfo.UpdateBeforePaint(autoFilterRowInfo);
								DrawIndicator(DrawInfo.Graphics, autoFilterRowInfo.IndicatorInfo.Bounds, autoFilterRowInfo.IndicatorInfo.Appearance, autoFilterRowInfo.IndicatorInfo.ImageIndex, autoFilterRowInfo.Node, autoFilterRowInfo.TopMostIndicator, true);
							}
							foreach(RowInfo ri in viewInfo.RowsInfo.Rows) {
								viewInfo.UpdateBeforePaint(ri);
								Rectangle clipRect = GetRowClipBounds(ri, ri.Bounds, false);
								if(clipRect.Height < 1) continue;
								GraphicsClipState clipState = Cache.ClipInfo.SaveAndSetClip(clipRect);
								DrawIndicator(DrawInfo.Graphics, ri.IndicatorInfo.Bounds, ri.IndicatorInfo.Appearance, ri.IndicatorInfo.ImageIndex, ri.Node, ri.TopMostIndicator, true);
								Cache.ClipInfo.RestoreClipRelease(clipState);
							}
							foreach(RowFooterInfo fi in viewInfo.RowsInfo.RowFooters) {
								DrawIndicator(DrawInfo.Graphics, fi.IndicatorInfo.Bounds, fi.IndicatorInfo.Appearance, -1, fi.Node, false, false);
							}
							clip.RestoreClip(DrawInfo.Graphics);
						}
					}
				}
				if(autoFilterRowInfo != null)
					DrawRow(drawInfo, autoFilterRowInfo);
				foreach(RowInfo ri in viewInfo.RowsInfo.Rows) {
					DrawRow(drawInfo, ri);
				}
				if(!viewInfo.PaintAnimatedItems) {
					foreach(RowFooterInfo fi in viewInfo.RowsInfo.RowFooters) {
						Rectangle clipRect = GetRowFooterClipBounds(fi, fi.Bounds);
						if(clipRect.Height < 1) continue;
						GraphicsClipState clipState = DrawInfo.Cache.ClipInfo.SaveAndSetClip(clipRect, true, true); 
						DrawRowFooter(fi, viewInfo.RC);
						DrawInfo.Cache.ClipInfo.RestoreClipRelease(clipState);
					}
					DrawDropNodeArrow(viewInfo.RowsInfo, viewInfo.dragInfo, viewInfo.ViewRects.IndicatorWidth);
				}   
			}
			if(autoFilterRowInfo != null) 
				DrawTopRowSeparator(drawInfo, viewInfo.AutoFilterRowInfo);
			DrawSizeGrip();
		}
		Rectangle GetRowFooterClipBounds(RowFooterInfo ri, Rectangle bounds) {			
			Rectangle rx = bounds, clipRect;
			if(DrawInfo.IsRightToLeft) {
				if(rx.Right > DrawInfo.ViewInfo.ViewRects.Rows.Right + DrawInfo.ViewInfo.ViewRects.IndicatorWidth)
					rx.Width -= DrawInfo.ViewInfo.ViewRects.IndicatorWidth;
			}
			else {
				if(rx.X < DrawInfo.ViewInfo.ViewRects.Rows.X + DrawInfo.ViewInfo.ViewRects.IndicatorWidth) {
					int delta = (DrawInfo.ViewInfo.ViewRects.Rows.X + DrawInfo.ViewInfo.ViewRects.IndicatorWidth) - rx.X;
					rx.X += delta;
					rx.Width -= delta;
				}
				if(rx.Right > DrawInfo.ViewInfo.ViewRects.Rows.Right)
					rx.Width -= (rx.Right - DrawInfo.ViewInfo.ViewRects.Rows.Right);
				if(ri != null && rx.Right > ri.Bounds.Right)
					rx.Width -= (rx.Right - ri.Bounds.Right);
			}
			clipRect = rx;
			if(clipRect.Top < DrawInfo.ViewInfo.ViewRects.Rows.Top) {
				int bottom = clipRect.Bottom;
				clipRect.Y = DrawInfo.ViewInfo.ViewRects.Rows.Top;
				clipRect.Height = bottom - clipRect.Y;
			}
			if(clipRect.Bottom > DrawInfo.ViewInfo.ViewRects.Rows.Bottom) {
				clipRect.Height -= (clipRect.Bottom - DrawInfo.ViewInfo.ViewRects.Rows.Bottom);
			}
			return clipRect;
		}
		protected virtual void DrawSizeGrip() {
			TreeListViewInfo viewInfo = DrawInfo.ViewInfo;
			if(!viewInfo.PaintAnimatedItems && !viewInfo.ViewRects.BeyondScrollSquare.IsEmpty) {
				Color color = viewInfo.TreeList.ScrollInfo.IsOverlapScrollbar ? viewInfo.PaintAppearance.Empty.BackColor : ElementPainters.SizeGripColor;
				XPaint.Graphics.FillRectangle(DrawInfo.Graphics, Cache.GetSolidBrush(color), viewInfo.ViewRects.BeyondScrollSquare);
			}
		}
		protected virtual void DrawTopRowSeparator(TreeListDrawInfo e, RowInfo autoFilterRow) {
			ObjectPainter.DrawObject(e.Cache, this.ElementPainters.SpecialRowSeparatorPainter, new StyleObjectInfoArgs(e.Cache, e.ViewInfo.ViewRects.TopRowSeparator, e.ViewInfo.PaintAppearance.HeaderPanel));
		}
		protected virtual void DrawFilterPanel() {
			TreeListViewInfo viewInfo = DrawInfo.ViewInfo;
			if(viewInfo.FilterPanel.Bounds.IsEmpty) return;
			if(!NeedsRedraw(viewInfo.FilterPanel.Bounds)) return;
			TreeListFilterPanelInfoArgs filterPanel = viewInfo.FilterPanel;
			CustomDrawObjectEventArgs cs = new CustomDrawObjectEventArgs(DrawInfo.Cache, viewInfo.FilterPanelPainter, filterPanel, filterPanel.Appearance, DrawInfo.IsRightToLeft);
			cs.SetDefaultDraw(() => ObjectPainter.DrawObject(DrawInfo.Cache, viewInfo.FilterPanelPainter, filterPanel));
			viewInfo.TreeList.RaiseCustomDrawFilterPanel(cs);
			cs.DefaultDraw();
		}
		void DrawEmptyArea() {
			if(NeedsRedraw(DrawInfo.ViewInfo.EmptyArea)) {
				CustomDrawEmptyAreaEventArgs e = new CustomDrawEmptyAreaEventArgs(Cache, DrawInfo.ViewInfo.EmptyArea, DrawInfo.ViewInfo.PaintAppearance.Empty, DrawInfo.ViewInfo.ViewRects.EmptyRows, DrawInfo.ViewInfo.ViewRects.EmptyBehindColumn, DrawInfo.ViewInfo.EmptyAreaRegion, DrawInfo.IsRightToLeft);
				e.SetDefaultDraw(() => DefaultPaintHelper.DrawEmptyArea(e));
				PaintLink.DrawEmptyArea(e);
				e.DefaultDraw();
			}
		}
		protected virtual void DrawRow(TreeListDrawInfo e, RowInfo ri) {
			if(NeedsRedraw(ri.Bounds)) {
				if(ri.Cells.Count == 0 || ri.Node.TreeList == null) return; 
				Rectangle clipBounds = GetRowClipBounds(ri, ri.Bounds, true);
				if(clipBounds.Height < 1) return;
				GraphicsClipState clipState = e.Cache.ClipInfo.SaveAndSetClip(clipBounds, true, true); 
				e.ViewInfo.UpdateBeforePaint(ri);
				if(!DrawInfo.ViewInfo.PaintAnimatedItems) {
					FillRowBackground(ri);
					DrawPreview(ri);
					DrawCheckBox(ri);
					DrawImages(ri);
					DrawIndents(ri.IndentInfo, 0);
					DrawNodeButton(ri);
				}
				DrawCells(ri);
				if(!DrawInfo.ViewInfo.PaintAnimatedItems)
					DrawLines(ri);
				DrawRowFocus(e, ri);
				DrawTransparentForeground(e, ri);
				e.Cache.ClipInfo.RestoreClipRelease(clipState);
			}
		}
		protected virtual void DrawRowFocus(TreeListDrawInfo e, RowInfo ri) {
			TreeList treeList = e.ViewInfo.TreeList;
			DrawFocusRectStyle drawStyle = treeList.OptionsView.FocusRectStyle;
			if((drawStyle == DrawFocusRectStyle.RowFocus || drawStyle == DrawFocusRectStyle.RowFullFocus) && ((ri.RowState & TreeNodeCellState.FocusedAndTreeFocused) == TreeNodeCellState.FocusedAndTreeFocused)) {
				Rectangle r = ri.DataBounds;
				if(treeList.OptionsView.ShowVertLines) {
					r.Offset(e.ViewInfo.RC.vlw, 0);
					r.Width -= 2 * e.ViewInfo.RC.vlw;
				}
				if(drawStyle == DrawFocusRectStyle.RowFullFocus && !ri.PreviewBounds.IsEmpty)
					r.Height = ri.PreviewBounds.Bottom - r.Y;
				XPaint.Graphics.DrawFocusRectangle(e.Graphics, r, ri.Appearance.GetForeColor(), ri.Appearance.GetBackColor());
			}
		}
		void DrawTransparentForeground(TreeListDrawInfo drawInfo, RowInfo ri) {
			if((ri.RowState & TreeNodeCellState.Selected) != 0 && drawInfo.ViewInfo.TreeList.OptionsSelection.EnableAppearanceFocusedRow) {
				TreeListViewInfo viewInfo = drawInfo.ViewInfo;
				if(viewInfo.IsTransparentFocusedStyle || viewInfo.IsTransparentSelectedStyle) {
					AppearanceObject app = drawInfo.ViewInfo.GetRowTransparentAppearance(ri);
					if(app != null) app.FillRectangle(drawInfo.Cache, ri.DataBounds, true);
				}
			}
		}
		void FillCellBackground(CellInfo ci) {
			ci.PaintAppearance.FillRectangle(DrawInfo.Cache, ci.Bounds);
		}
		protected virtual void FillRowBackground(RowInfo ri) {
			if (ri.IndentAsRowStyle) 
				ri.PaintAppearance.FillRectangle(DrawInfo.Cache, ri.GetFillBackgroundBounds(true));
			else {
				bool canFiilWithIndent = DrawNodeIndent(ri.IndentInfo, ri.Node, ri.IndentInfo.Bounds, true, ri.PaintAppearance);
				ri.PaintAppearance.FillRectangle(DrawInfo.Cache, ri.GetFillBackgroundBounds(canFiilWithIndent));
			}
		}
		private void DrawPreview(RowInfo ri) {
			if(ri.PreviewBounds.IsEmpty) return;
			CustomDrawNodePreviewEventArgs e = new CustomDrawNodePreviewEventArgs(Cache, ri.PreviewBounds, 
				ri.AppearancePreview, ri.Node, ri.PreviewText, DrawInfo.IsRightToLeft);
			e.SetDefaultDraw(() => DefaultPaintHelper.DrawNodePreview(e));
			PaintLink.DrawNodePreview(e);
			e.DefaultDraw();
		}
		bool IsButtonCentered { get { return DrawInfo.ViewInfo.TreeList.OptionsView.ExpandButtonCentered; } }
		protected virtual void DrawIndents(IndentInfo indentInfo, int hlw) {
			if(NeedsRedraw(indentInfo.Bounds) && indentInfo.TreeLineBrush != null) {
				IntPtr save = clip.MySaveClip(DrawInfo.Graphics);
				try {
					Rectangle indentItem = new Rectangle(indentInfo.Bounds.Location,
						new Size(indentInfo.LevelWidth, indentInfo.Bounds.Height + hlw));
					int actualRowHeight = IsButtonCentered ? indentItem.Height : indentInfo.ActualRowHeight;
					foreach(RowIndentItem rii in indentInfo.IndentItems) {
						switch(rii) {
							case RowIndentItem.FirstRoot:
								DrawFirstRootIndentItem(indentItem, indentInfo.TreeLineBrush, actualRowHeight);
								break;
							case RowIndentItem.Root:
								DrawRootIndentItem(indentItem, indentInfo.TreeLineBrush);
								break;
							case RowIndentItem.LastRoot:
								DrawLastRootIndentItem(indentItem, indentInfo.TreeLineBrush, actualRowHeight);
								break;
							case RowIndentItem.Parent:
								DrawParentIndentItem(indentItem, indentInfo.TreeLineBrush, actualRowHeight);
								break;
							case RowIndentItem.Single:
								DrawSingleIndentItem(indentItem, indentInfo.TreeLineBrush, actualRowHeight);
								break;
							case RowIndentItem.NextChild:
								DrawNextChildIndentItem(indentItem, indentInfo.TreeLineBrush, actualRowHeight);
								break;
							case RowIndentItem.LastChild:
								DrawLastChildIndentItem(indentItem, indentInfo.TreeLineBrush, actualRowHeight);
								break;
							case RowIndentItem.None:
								break;
						}
						indentItem.X += indentInfo.LevelWidth;
					}
				}
				finally {
					clip.MyRestoreClip(DrawInfo.Graphics, save);
				}
			}
		}
		private void DrawNodeButton(RowInfo ri) {
			if(ri.ButtonBounds.IsEmpty) return;
			CustomDrawNodeButtonEventArgs e = new CustomDrawNodeButtonEventArgs(Cache,
				ri.ButtonBounds, ri.AppearanceButton, ri.Node, ElementPainters.OpenCloseButtonPainter, DrawInfo.IsRightToLeft);
			e.SetDefaultDraw(() => DefaultPaintHelper.DrawOpenCloseButton(e));
			PaintLink.DrawNodeButton(e);
			e.DefaultDraw();
		}
		private void DrawIndicator(Graphics g, Rectangle indicatorRect, AppearanceObject appearance, int imageIndex, TreeListNode node, bool topMost, bool isNode) {
			CustomDrawNodeIndicatorEventArgs e = new CustomDrawNodeIndicatorEventArgs(Cache, indicatorRect, appearance, isNode, imageIndex, topMost, node, ElementPainters.IndicatorPainter, DrawInfo.IsRightToLeft);
			e.SetDefaultDraw(() => DefaultPaintHelper.DrawIndicator(e));
			PaintLink.DrawIndicator(e);
			e.DefaultDraw();
		}
		private void DrawCheckBox(RowInfo ri) {
			if(ri.CheckBounds.IsEmpty) return;
			CustomDrawNodeCheckBoxEventArgs e = new CustomDrawNodeCheckBoxEventArgs(Cache, ri.CheckBounds, ri.AppearanceButton , ri.Node, ElementPainters.CheckPainter, DrawInfo.IsRightToLeft);
			CheckObjectInfoArgs args = e.ObjectArgs as CheckObjectInfoArgs;
			args.CheckState = ri.Node.CheckState;
			args.LookAndFeel = PaintLink.LookAndFeel;
			e.SetDefaultDraw(() => DefaultPaintHelper.DrawNodeCheckBox(e));
			PaintLink.DrawNodeCheckBox(e);
			e.DefaultDraw();
		}
		protected virtual void DrawImages(RowInfo ri) {
			Rectangle bounds = Rectangle.Union(ri.SelectImageBounds, ri.StateImageBounds);
			if(!NeedsRedraw(bounds)) return;
			CustomDrawNodeImagesEventArgs e = new CustomDrawNodeImagesEventArgs(Cache, bounds,
				ri.IndentInfo.Appearance, ri.Node, ri.SelectImageIndex, ri.StateImageIndex, 
				ri.SelectImageBounds, ri.StateImageBounds, ri.SelectImageLocation, ri.StateImageLocation, DrawInfo.IsRightToLeft);
			e.SetDefaultDraw(() => DrawImagesCore(ri, e));
			IntPtr save = clip.MySaveClip(DrawInfo.Graphics);
			PaintLink.DrawNodeImages(e);
			clip.MyRestoreClip(DrawInfo.Graphics, save);
			e.DefaultDraw();
		}
		void DrawImagesCore(RowInfo ri, CustomDrawNodeImagesEventArgs e) {
			e.FillBackground = !TreeListViewInfo.IsSamePaintBackground(e.Appearance, ri.IndentInfo.Appearance) || ri.FillImagesBackground;
			if(NeedsRedraw(e.SelectRect)) {
				IntPtr save1 = clip.MySaveClip(DrawInfo.Graphics);
				DefaultPaintHelper.DrawNodeSelectImage(e);
				clip.MyRestoreClip(DrawInfo.Graphics, save1);
			}
			if(NeedsRedraw(ri.StateImageBounds)) {
				IntPtr save1 = clip.MySaveClip(DrawInfo.Graphics);
				DefaultPaintHelper.DrawNodeStateImage(e);
				clip.MyRestoreClip(DrawInfo.Graphics, save1);
			}
		}
		protected void DrawFirstRootIndentItem(Rectangle indentItem, Brush foreBrush, int actualRowHeight) {
			TreeListIndentItemPainter.DrawFirstRootIndentItem(DrawInfo.Graphics, indentItem, foreBrush, actualRowHeight, !IsButtonCentered); 
		}
		protected void DrawRootIndentItem(Rectangle indentItem, Brush foreBrush) {
			TreeListIndentItemPainter.DrawRootIndentItem(DrawInfo.Graphics, indentItem, foreBrush);
		}
		protected void DrawLastRootIndentItem(Rectangle indentItem, Brush foreBrush, int actualRowHeight) {
			TreeListIndentItemPainter.DrawLastRootIndentItem(DrawInfo.Graphics, indentItem, foreBrush, actualRowHeight);
		}
		protected void DrawParentIndentItem(Rectangle indentItem, Brush foreBrush, int actualRowHeight) {
			TreeListIndentItemPainter.DrawParentIndentItem(DrawInfo.Graphics, indentItem, foreBrush, actualRowHeight, !IsButtonCentered); 
		}
		protected void DrawSingleIndentItem(Rectangle indentItem, Brush foreBrush, int actualRowHeight) {
			TreeListIndentItemPainter.DrawSingleIndentItem(DrawInfo.Graphics, indentItem, foreBrush, actualRowHeight);
		}
		protected void DrawNextChildIndentItem(Rectangle indentItem, Brush foreBrush, int actualRowHeight) {
			TreeListIndentItemPainter.DrawNextChildIndentItem(DrawInfo.Graphics, indentItem, foreBrush, actualRowHeight);
		}
		protected void DrawLastChildIndentItem(Rectangle indentItem, Brush foreBrush, int actualRowHeight) {
			TreeListIndentItemPainter.DrawLastChildIndentItem(DrawInfo.Graphics, indentItem, foreBrush, actualRowHeight); 
		}
		private void DrawDropNodeArrow(RowsInfo rowsInfo, DragScrollInfo dragInfo, int indicatorWidth) {
			if(dragInfo == null) return;
			if(dragInfo.LastEffect == DragDropEffects.None) return;
			RowInfo ri = dragInfo.RowInfo;
			if(ri == null || ri.Bounds.Bottom > DrawInfo.ViewInfo.ViewRects.Client.Bottom) return;
			Size size = NodeDragImages.ImageSize;
			int y = ri.Bounds.Top + (ri.Bounds.Height - size.Height) / 2;
			int x = DrawInfo.IsRightToLeft ? ri.Bounds.Right - indicatorWidth - size.Width : ri.Bounds.Left + indicatorWidth;
			Rectangle r = new Rectangle(new Point(x, y), size);
			if(dragInfo.imageIndex > -1 && dragInfo.imageIndex < NodeDragImages.Images.Count)
				DrawInfo.Graphics.DrawImageUnscaled(GetNodeDragImage(dragInfo.imageIndex), r);
		}
		Image GetNodeDragImage(int index){
			Image im = NodeDragImages.Images[index];
			if(DrawInfo.IsRightToLeft)
				return GetRTLImage(im);
			return im;
		}
		Image GetRTLImage(Image image) {
			if(image == null) return null;
			Image i = image.Clone() as Image;
			i.RotateFlip(RotateFlipType.RotateNoneFlipX);
			return i;
		}
		protected virtual void DrawCells(RowInfo ri) {
			drawnCell = null;
			try {
				for(int i = 0; i < ri.Cells.Count; i++) {
					drawnCell = (CellInfo)ri.Cells[i];
					IAnimatedItem item = drawnCell.EditorViewInfo as IAnimatedItem;
					if(item != null)
						CheckAnimation(drawnCell, ri.Node, item);
					if(drawInfo.ViewInfo != null && drawInfo.ViewInfo.PaintAnimatedItems) {
						if(item == null || (item.FramesCount < 2 && item.AnimationInterval == 0)) continue;
						FillCellBackground(drawnCell);
					}
					TreeListClipInfo cli = null;
					if(drawInfo.ViewInfo != null) {
						cli = PrepareClipInfo(DrawInfo, drawnCell.ColumnInfo, drawnCell.Bounds);
						if(!cli.ShouldPaint) {
							cli.Restore(DrawInfo);
							continue;
						}
					}
					DrawCell(drawnCell);
					if(drawInfo.ViewInfo != null) cli.Restore(DrawInfo);
				}
			}
			finally {
				drawnCell = null;
			}
		}
		protected internal virtual void CheckAnimation(CellInfo cellInfo, TreeListNode node, IAnimatedItem item) {
			if(drawInfo.ViewInfo == null) return;
			TreeListCellId id = new TreeListCellId(node, drawnCell.Column);
			EditorAnimationInfo info = XtraAnimator.Current.Get(drawInfo.ViewInfo.TreeList, id) as EditorAnimationInfo;
			if(info != null) {
				if(drawInfo.ViewInfo.ShouldStopAnimation(info))
					item.OnStop();
				else
					item.OnStart();
			}
		}
		private void DrawCell(CellInfo cell) {
			CustomDrawNodeCellEventArgs e = new CustomDrawNodeCellEventArgs(Cache, cell.Bounds,
				cell.PaintAppearance, PaintLink.GetPainter(drawnCell.Item), cell.ColumnInfo.Column, cell.RowInfo.Node,
				cell.EditorViewInfo, cell.Focused && PaintLink.CanShowFocusRect, DrawInfo.IsRightToLeft);
			e.SetDefaultDraw(() => DrawCellCore(cell, e));
			PaintLink.DrawNodeCell(e);
			e.DefaultDraw();
		}
		void DrawCellCore(CellInfo cell, CustomDrawNodeCellEventArgs e) {
			e.FillBackground = !cell.RowInfo.IsSamePaintBackground(e.Appearance);
			GraphicsClipState clipInfo = null;
			Rectangle maxClipBounds = e.Cache.ClipInfo.MaximumBounds;
			try {
				if(cell.ColumnInfo.Type == ColumnInfo.ColumnInfoType.Column) {
					if((cell.EditorViewInfo.RequireClipping || cell.RowInfo.FormatRuleInfo.HasDrawFormatRules) && DrawInfo.ViewInfo != null) {
						Rectangle rect = GetRowClipBounds(cell.RowInfo, DrawInfo.ViewInfo.UpdateFixedRange(cell.Bounds, cell.ColumnInfo), true);
						if(rect.Width < 0) return;
						e.Cache.ClipInfo.MaximumBounds = rect;
						clipInfo = e.Cache.ClipInfo.SaveAndSetClip(maxClipBounds);
					}
					UpdateEditViewInfoIsCellSelected(cell);
					if(e.FillBackground) {
						Rectangle backBounds = GetBackgroundCellBounds(e.EditViewInfo, e.Focused);
						e.Appearance.DrawBackground(e.Cache, backBounds);
					}
					DrawAppearanceMethod drawMethod = (cache, appearance) =>
					{
						DefaultPaintHelper.DrawNodeCell(e);	
					};
					if(cell.ColumnInfo.Type == ColumnInfo.ColumnInfoType.Column) 
						if(cell.RowInfo.FormatRuleInfo.ApplyDrawFormat(e.Cache, cell.Column, cell.RowInfo.Node, cell.Bounds, cell.EditorViewInfo, drawMethod, e.Appearance)) return;
					drawMethod(e.Cache, e.Appearance);
				}
				if(cell.ColumnInfo.Type == ColumnInfo.ColumnInfoType.EmptyColumn)
					FillCellBackground(cell);
			}
			finally {
				if(clipInfo != null) {
					e.Cache.ClipInfo.MaximumBounds = maxClipBounds;
					e.Cache.ClipInfo.RestoreClipRelease(clipInfo);
				}
			}
		}
		protected virtual Rectangle GetBackgroundCellBounds(DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo vi, bool focused) {
			const int inflateFocusFrameSize = 1;
			if(vi.ErrorIconBounds.IsEmpty) return Rectangle.Inflate(vi.Bounds, inflateFocusFrameSize, inflateFocusFrameSize);
			Rectangle result = Rectangle.Union(vi.Bounds, vi.ErrorIconBounds);
			result.Inflate(inflateFocusFrameSize, inflateFocusFrameSize);
			return result;
		}
		void UpdateEditViewInfoIsCellSelected(CellInfo cell) {
			if(DrawInfo.ViewInfo == null) return;
			TreeListOptionsSelection optSelection = DrawInfo.ViewInfo.TreeList.OptionsSelection;
			bool isInvertSelection = !optSelection.MultiSelect && optSelection.InvertSelection;
			bool isFocusCell = (cell.State & TreeNodeCellState.FocusedCell) != 0 && (cell.State & TreeNodeCellState.TreeFocused) != 0;
			bool isSelectedRow = (cell.State & TreeNodeCellState.Selected) != 0;
			bool isFocusedRow = (cell.State & TreeNodeCellState.Focused) != 0;
			cell.EditorViewInfo.IsCellSelected = (isSelectedRow && !isFocusCell && (isFocusedRow && !isInvertSelection)) || (isFocusCell && isInvertSelection);
		}
		Rectangle GetClipBoundsRows(bool excludeIndicator, bool isAutoFiterRow) {
			Rectangle clipRectRows = isAutoFiterRow ? DrawInfo.ViewInfo.ViewRects.AutoFilterRow : DrawInfo.ViewInfo.ViewRects.Rows;
			if(excludeIndicator) {
				if(!DrawInfo.IsRightToLeft)
					clipRectRows.X += DrawInfo.ViewInfo.ViewRects.IndicatorWidth;
				clipRectRows.Width -= DrawInfo.ViewInfo.ViewRects.IndicatorWidth;
			}
			return clipRectRows;
		}
		Rectangle GetRowClipBounds(RowInfo ri, Rectangle bounds, bool excludeIndicator) {
			bool isAutoFilterRow = DrawInfo.ViewInfo.AutoFilterRowInfo == ri;
			Rectangle clipRectRows = GetClipBoundsRows(excludeIndicator, isAutoFilterRow);
			Rectangle clipRect = Rectangle.Intersect(clipRectRows, bounds);
			if(DrawInfo.IsRightToLeft) {			 
				if(ri != null && clipRect.X < ri.DataBounds.X) {
					clipRect.Width -= ri.DataBounds.X - clipRect.X;
					clipRect.X = ri.DataBounds.X;
				}
			}
			else {			   
				if(ri != null && clipRect.Right > ri.DataBounds.Right)
					clipRect.Width -= (clipRect.Right - ri.DataBounds.Right);
			}			
			if(clipRect.Top < DrawInfo.ViewInfo.ViewRects.Rows.Top && !isAutoFilterRow) {
				int bottom = clipRect.Bottom;
				clipRect.Y = DrawInfo.ViewInfo.ViewRects.Rows.Top;
				clipRect.Height = bottom - clipRect.Y;
			}		  
			return clipRect;
		}
		protected virtual void DrawLines(RowInfo ri) {
			DrawLines(ri.Lines);
			DrawLines(ri.FloatLines);
		}
		void DrawLines(ArrayList lines) {
			foreach(LineInfo li in lines)
				li.Appearance.FillRectangle(Cache, li.Rect);
		}
		protected virtual void DrawBorder(AppearanceObject appearance, Rectangle r) {
			if(!NeedsRedraw(r)) return;
			GraphicsClipState clip = Cache.ClipInfo.SaveAndSetClip(r);
			try {
				ElementPainters.BorderPainter.DrawObject(ElementPainters.GetBorderPainterInfoArgs(Cache, r, appearance));
			}
			finally {
				Cache.ClipInfo.RestoreClipRelease(clip);
			}
		}
		protected virtual void DrawFooterItems(SummaryFooterInfo fi) {
			bool isRowFooter = (fi is RowFooterInfo);
			CustomDrawFooterCellEventArgs e;
			foreach(FooterItem f in fi.FooterItems) {
				Rectangle r = f.ItemBounds;
				TreeListClipInfo cli = PrepareClipInfo(DrawInfo, f.Column.TreeList.ViewInfo.ColumnsInfo[f.Column], r);
				if(!cli.ShouldPaint) {
					cli.Restore(DrawInfo);
					continue;
				}
				if(isRowFooter) {
					e = new CustomDrawRowFooterCellEventArgs(Cache, f.ItemBounds,
						fi.Appearance, f.Column, ((RowFooterInfo)fi).Node, f.ItemType, f.ItemText, ElementPainters.FooterCellPainter, DrawInfo.IsRightToLeft);
					e.SetDefaultDraw(() => DefaultPaintHelper.DrawRowFooterCell((CustomDrawRowFooterCellEventArgs)e));
					PaintLink.DrawRowFooterCell((CustomDrawRowFooterCellEventArgs)e);
					e.DefaultDraw();
				}
				else {
					e = new CustomDrawFooterCellEventArgs(Cache, f.ItemBounds, fi.Appearance, f.Column, f.ItemType, f.ItemText, ElementPainters.FooterCellPainter, DrawInfo.IsRightToLeft);
					e.SetDefaultDraw(() => DefaultPaintHelper.DrawFooterCell(e));
					PaintLink.DrawFooterCell(e);
					e.DefaultDraw();
				}
				cli.Restore(DrawInfo);
			}
		}
		private void DrawRowFooter(RowFooterInfo fi, ResourceInfo rc) {
			if(!NeedsRedraw(fi.Bounds) || fi.Bounds.Width == fi.IndicatorInfo.Bounds.Width) return;
			if(fi.CanFillIndent) {
				DrawNodeIndent(fi.IndentInfo, fi.Node, fi.IndentBounds, false, fi.Appearance);
			}
			DrawLines(fi.FloatLines);
			DrawIndents(fi.IndentInfo, rc.hlw);
			CustomDrawRowFooterEventArgs e = new CustomDrawRowFooterEventArgs(Cache, fi.FooterBounds, fi.Appearance, ElementPainters.FooterPanelPainter, fi.Node, DrawInfo.IsRightToLeft);
			e.SetDefaultDraw(() => DefaultPaintHelper.DrawFooterBackGround(e));
			PaintLink.DrawRowFooter(e);
			e.DefaultDraw();
			DrawFooterItems(fi);
		}
		bool DrawNodeIndent(IndentInfo ii, TreeListNode node, Rectangle bounds, bool isNodeIndent, AppearanceObject etalonAppearance) {
			if(!NeedsRedraw(bounds)) return false;
			CustomDrawNodeIndentEventArgs e = new CustomDrawNodeIndentEventArgs(Cache, bounds, ii.Appearance, new StyleObjectPainter(),
				node, isNodeIndent, DrawInfo.IsRightToLeft);
			e.SetDefaultDraw(() => DefaultPaintHelper.DrawNodeIndent(e));
			PaintLink.DrawNodeIndent(e);
			e.DefaultDraw();
			return (e.Handled || TreeListViewInfo.IsSamePaintBackground(e.Appearance, etalonAppearance));
		}
		private void DrawSummaryFooter(SummaryFooterInfo fi) {
			if(NeedsRedraw(fi.Bounds)) {
				CustomDrawEventArgs e = new CustomDrawFooterEventArgs(Cache, fi.Bounds, fi.Appearance, ElementPainters.FooterPanelPainter, DrawInfo.IsRightToLeft);
				e.SetDefaultDraw(() => DefaultPaintHelper.DrawFooterBackGround(e));
				PaintLink.DrawFooter(e);
				e.DefaultDraw();
				Rectangle r = fi.Bounds;
				int x = DrawInfo.ViewInfo.ViewRects.Rows.X + (DrawInfo.IsRightToLeft ? 0 : DrawInfo.ViewInfo.ViewRects.IndicatorWidth);
				if(x > r.X) {
					r.Width = r.Right - x;
					r.X = x;
				}
				GraphicsClipState clipState = e.Cache.ClipInfo.SaveAndSetClip(r);
				DrawFooterItems(fi);
				e.Cache.ClipInfo.RestoreClipRelease(clipState);
			}
		}
		protected virtual internal void DrawDragPreview(TreeListViewInfo viewInfo, DrawDragPreviewEventArgs e) {
			if(e.RowsInfo == null || e.RowsInfo.Length == 0) return;
			Rectangle currentClipRect = paintClipRect;
			paintClipRect = e.Bounds;
			TreeListDrawInfo oldInfo = DrawInfo;
			this.drawInfo = CreateDrawInfo(e.Cache, null, e.Bounds, viewInfo.IsRightToLeft);
			XPaint.Graphics.BeginPaint(e.Cache.Graphics);
			try {
				foreach(RowInfo info in e.RowsInfo)
					DrawDragNode(info);
			}
			finally {
				XPaint.Graphics.EndPaint(e.Cache.Graphics);
				paintClipRect = currentClipRect;
				this.drawInfo = oldInfo;
			}
		}
		protected virtual void DrawDragNode(RowInfo rowInfo) {
			using(SolidBrush brush = new SolidBrush(rowInfo.PaintAppearance.BackColor))
				DrawInfo.Cache.FillRectangle(brush, rowInfo.Bounds);
			DrawImages(rowInfo);
			DrawCells(rowInfo);
			DrawLines(rowInfo);
		}
		internal void DrawDragColumn(DrawDragColumnEventArgs e) {
			DrawDragObject<ColumnInfo>(e.Graphics, e.ColumnInfo, (cache, obj) => { DrawColumn(cache, obj); });
		}
		internal void DrawDragBand(Graphics g, BandInfo bi) {
			DrawDragObject<BandInfo>(g, bi, (cache, obj) => { DrawBandCore(cache, obj); });
		}
		void DrawDragObject<T>(Graphics g, T obj, Action<GraphicsCache, T> paintDelegate) where T : HeaderObjectInfoArgs  {
			Rectangle currentClipRect = paintClipRect;
			GraphicsCache cache = new GraphicsCache(new DXPaintEventArgs(g, Rectangle.Empty), new DevExpress.Utils.Paint.XPaint());
			paintClipRect = obj.Bounds;
			XPaint.Graphics.BeginPaint(cache.Graphics);
			try {
				paintDelegate(cache, obj);
			}
			finally {
				XPaint.Graphics.EndPaint(cache.Graphics);
				cache.Dispose();
				paintClipRect = currentClipRect;
			}
		}
		internal PaintLink PaintLink {
			get { return paintLink; }
			set {
				if(PaintLink != value) {
					paintLink = value;
					DefaultPaintHelper = CreatePaintHelper();
					UpdateElementPainters();
				}
			}
		}
		public virtual ITreeListPaintHelper DefaultPaintHelper { 
			get { return defaultPaintHelper; } 
			set {
				if(value == null) return;
				if(DefaultPaintHelper != value) {
					defaultPaintHelper = value;
					PaintLink.DefaultPaintHelperChanged();
				}
			}
		}
		public virtual ITreeListPaintHelper CreatePaintHelper() {
			return new TreeListPaintHelper();
		}
		public virtual bool NeedsRedraw(Rectangle r) {
			if(r.Width == 0 || r.Height == 0) return false;
			Rectangle rUpdate = Rectangle.Intersect(r, paintClipRect);
			return !(rUpdate.Width == 0 || rUpdate.Height == 0);
		}
		protected internal void UpdateElementPainters() {
			switch(PaintLink.LookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Flat: elementPainters = new FlatElementPainters(PaintLink); break;
				case ActiveLookAndFeelStyle.UltraFlat: elementPainters = new UltraFlatElementPainters(PaintLink); break;
				case ActiveLookAndFeelStyle.Style3D: elementPainters = new Style3DElementPainters(PaintLink); break;
				case ActiveLookAndFeelStyle.WindowsXP: elementPainters = new XPElementPainters(PaintLink); break;
				case ActiveLookAndFeelStyle.Office2003: elementPainters = new Office2003ElementPainters(PaintLink); break;
				case ActiveLookAndFeelStyle.Skin: elementPainters = new SkinElementPainters(PaintLink); break;
				default: elementPainters = new FlatElementPainters(PaintLink); break;
			}
		}
		protected TreeListDrawInfo DrawInfo { get { return drawInfo; } }
		protected GraphicsCache Cache { get { return DrawInfo.Cache; } }
		protected internal ElementPainters ElementPainters { get { return elementPainters; } }
		public Rectangle CurrentPaintClipRect { get { return paintClipRect; } }
		public ImageCollection IndicatorImages {
			get {
				if(indicatorImageList == null)
					this.indicatorImageList = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.Utils.Indicator.bmp", typeof(DevExpress.Utils.Controls.ImageHelper).Assembly, new Size(7, 9), Color.Magenta);
				return indicatorImageList;
			}
		}		
		public ImageCollection NodeDragImages {
			get {
				if(nodeDragImageList == null) {
					this.nodeDragImageList = new ImageCollection();
					this.nodeDragImageList.ImageSize = new Size(15, 12);
					this.nodeDragImageList.Images.Add(Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraTreeList.Images.arrow.png")));
					this.nodeDragImageList.Images.Add(Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraTreeList.Images.arrow_ins_before.png")));
					this.nodeDragImageList.Images.Add(Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraTreeList.Images.arrow_ins_after.png")));
				}
				return nodeDragImageList;
			}
		}
		public const int EditorIsInactiveIndicatorImageIndex = 0,
			EditorIsModifiedIndicatorImageIndex = 1,
			EditorIsActiveIndicatorImageIndex = 5,
			ErrorInFocusedNodeIndicatorImageIndex = 7,
			ErrorInNodeIndicatorImageIndex = 6,
			AutoFilterRowIndicatorImageIndex = 8;
		#region TreeListIndentItemPainter
		class TreeListIndentItemPainter {
			public static void DrawFirstRootIndentItem(Graphics g, Rectangle indentItem, Brush foreBrush, int actualHeight, bool isPreview) {
				XPaint.Graphics.FillRectangle(g, foreBrush,
					new Rectangle(indentItem.Left + indentItem.Width / 2,
					indentItem.Top + actualHeight / 2,
					indentItem.Width / 2,
					1));
				XPaint.Graphics.FillRectangle(g, foreBrush,
					new Rectangle(indentItem.Left + indentItem.Width / 2,
					indentItem.Top + actualHeight / 2,
					1,
					isPreview ? indentItem.Height + 1 : indentItem.Height / 2 + 1));
			}
			public static void DrawRootIndentItem(Graphics g, Rectangle indentItem, Brush foreBrush) {
				XPaint.Graphics.FillRectangle(g, foreBrush,
					new Rectangle(indentItem.Left + indentItem.Width / 2,
					indentItem.Top,
					1,
					indentItem.Height));
			}
			public static void DrawLastRootIndentItem(Graphics g, Rectangle indentItem, Brush foreBrush, int actualHeight) {
				XPaint.Graphics.FillRectangle(g, foreBrush,
					new Rectangle(indentItem.Left + indentItem.Width / 2,
					indentItem.Top + actualHeight / 2,
					indentItem.Width / 2,
					1));
			}
			public static void DrawParentIndentItem(Graphics g, Rectangle indentItem, Brush foreBrush, int actualHeight, bool isPreview) {
				DrawSingleIndentItem(g, indentItem, foreBrush, actualHeight);
				XPaint.Graphics.FillRectangle(g, foreBrush,
					new Rectangle(indentItem.Left + indentItem.Width / 2,
					indentItem.Top + actualHeight / 2,
					1,
					isPreview ? indentItem.Height + 1 : indentItem.Height / 2 + 1));
			}
			public static void DrawSingleIndentItem(Graphics g, Rectangle indentItem, Brush foreBrush, int actualHeight) {
				XPaint.Graphics.FillRectangle(g, foreBrush,
					new Rectangle(indentItem.Left,
					indentItem.Top + actualHeight / 2,
					indentItem.Width,
					1));
			}
			public static void DrawNextChildIndentItem(Graphics g, Rectangle indentItem, Brush foreBrush, int actualHeight) {
				DrawRootIndentItem(g, indentItem, foreBrush);
				XPaint.Graphics.FillRectangle(g, foreBrush,
					new Rectangle(indentItem.Left + indentItem.Width / 2,
					indentItem.Top + actualHeight / 2,
					indentItem.Width / 2,
					1));
			}
			public static void DrawLastChildIndentItem(Graphics g, Rectangle indentItem, Brush foreBrush, int actualHeight) {
				XPaint.Graphics.FillRectangle(g, foreBrush,
					new Rectangle(indentItem.Left + indentItem.Width / 2,
					indentItem.Top,
					1,
					actualHeight / 2));
				XPaint.Graphics.FillRectangle(g, foreBrush,
					new Rectangle(indentItem.Left + indentItem.Width / 2,
					indentItem.Top + actualHeight / 2,
					indentItem.Width / 2,
					1));
			}
		}
		#endregion
	}
	public class TreeListClipInfo {
		public bool ShouldPaint, Clipped;
		public GraphicsClipState ClipState;
		public GraphicsCache Cache;
		public TreeListClipInfo(GraphicsCache cache) {
			this.Cache = cache;
			this.ClipState = null;
			this.ShouldPaint = true;
			this.Clipped = false;
		}
		public virtual void Prepare(TreeListDrawInfo e, ColumnInfo ci, Rectangle bounds) {
			if(e.ViewInfo.HasFixedColumns && !e.ViewInfo.IsFixedLeftPaint(ci)) {
				if(ci.Type == DevExpress.XtraTreeList.ViewInfo.ColumnInfo.ColumnInfoType.ColumnButton || ci.Type == ColumnInfo.ColumnInfoType.BehindColumn) return;
				Rectangle clipR = CheckRectangle(e.ViewInfo.UpdateFixedRange(bounds, ci));
				if(clipR.Width == 0) {
					this.ShouldPaint = false;
				}
				else {
					if(clipR != bounds) {
						ClipState = Cache.ClipInfo.SaveAndSetClip(clipR);
						this.Clipped = true;
					}
				}
			}
		}
		public virtual void PrepareBand(TreeListDrawInfo e, BandInfo bi, Rectangle bounds) {
			if(e.ViewInfo.HasFixedBands && bi.Fixed != FixedStyle.Left) {
				if(bi.Type == ColumnInfo.ColumnInfoType.ColumnButton || bi.Type == ColumnInfo.ColumnInfoType.BehindColumn) return;
				Rectangle clipR = CheckRectangle(e.ViewInfo.UpdateFixedRange(bounds, bi));
				if(clipR.Width == 0) {
					this.ShouldPaint = false;
				}
				else {
					if(clipR != bounds) {
						ClipState = Cache.ClipInfo.SaveAndSetClip(clipR);
						this.Clipped = true;
					}
				}
			}
		}
		public Rectangle CheckRectangle(Rectangle rect) {
			if(rect.Left < Cache.ClipInfo.MaximumBounds.Left) {
				rect.Width -= (Cache.ClipInfo.MaximumBounds.Left - rect.Left);
				rect.X = Cache.ClipInfo.MaximumBounds.Left;
			}
			if(rect.Width < 1) return Rectangle.Empty;
			if(rect.Y < Cache.ClipInfo.MaximumBounds.Top) {
				rect.Height -= (Cache.ClipInfo.MaximumBounds.Y - rect.Y);
				rect.Y = Cache.ClipInfo.MaximumBounds.Y;
			}
			if(rect.Height < 1) return Rectangle.Empty;
			return rect;
		}
		public virtual void Restore(TreeListDrawInfo e) {
			if(Clipped) {
				this.Clipped = false;
				Cache.ClipInfo.RestoreClipRelease(ClipState);
				ClipState = null;
			}
		}
	}
}
