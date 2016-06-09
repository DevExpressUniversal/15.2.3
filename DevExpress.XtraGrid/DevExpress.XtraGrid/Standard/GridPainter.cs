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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DevExpress.XtraGrid;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraGrid.Registrator;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing.Helpers;
using System.Threading;
using DevExpress.Skins;
using DevExpress.XtraEditors.Helpers;
using System.Drawing.Imaging;
namespace DevExpress.XtraGrid.Views.Grid.Drawing {
	public class GridViewDrawArgs : ViewDrawArgs {
		public GridViewDrawArgs(GraphicsCache graphicsCache, GridViewInfo viewInfo, Rectangle bounds) : base(graphicsCache, viewInfo, bounds) { }
		public virtual new GridViewInfo ViewInfo { get { return base.ViewInfo as GridViewInfo; } }
	}
	public class GridPainter : ColumnViewPainter {
		object reSizingObject = null;
		public const int IndicatorFocused = 0, IndicatorChanged = 1, IndicatorNewItemRow = 2,
			IndicatorZoom = 3, IndicatorNoZoom = 4, IndicatorEditing = 5, IndicatorError = 6, IndicatorFocusedError = 7,
			IndicatorFilterRow = 8;
		[ThreadStatic]
		static ImageList indicator = null;
		public static ImageList Indicator { 
			get { 
				if(indicator == null) {
					indicator = DevExpress.Utils.ResourceImageHelper.CreateImageListFromResources("DevExpress.Utils.Indicator.bmp", typeof(DevExpress.Utils.Controls.ImageHelper).Assembly, new Size(7, 9));
				}
				return indicator; 
			} 
		}
		GridElementsPainter elementsPainter;
		public GridPainter(GridView view) : base(view) {
			this.elementsPainter = (view.PaintStyle as GridPaintStyle).CreateElementsPainter(view);
		}
		public virtual GridElementsPainter ElementsPainter { get { return elementsPainter; } }
		public new GridView View { get { return base.View as GridView; } }
		protected ScrollPositionInfoBase PaintPositionInfo { get { return View.Scroller.ScrollInfo; } }
		public override void Draw(ViewDrawArgs ee) {
			GridViewDrawArgs e = ee as GridViewDrawArgs;
			GridHandler handler = View.Handler as GridHandler;
			bool prevSizerVisible = IsSizerLineVisible;
			HideSizerLine();
			GraphicsClipState clipState = e.Cache.ClipInfo.SaveAndSetClip(e.ViewInfo.ViewRects.Bounds, true, true);
			DrawBorder(e, e.ViewInfo.ViewRects.Bounds);
			e.Cache.ClipInfo.RestoreClipRelease(clipState);
			clipState = e.Cache.ClipInfo.SaveAndSetClip(e.ViewInfo.ViewRects.Scroll, true, true);
			try {
				DrawContents(e);
				DrawScrollbars(e);
				DrawSizeGrip(e);
			}
			finally {
				e.Cache.ClipInfo.RestoreClipRelease(clipState);
			}
			base.Draw(e);
			DrawDragFrame(e);
			if(prevSizerVisible) {
				View.BeginInvoke(new MethodInvoker(()=> { ShowSizerLine(); }));
			}
		}
		void DrawDragFrame(ViewDrawArgs e) {
			if(e.ViewInfo.DragFrameRect.IsEmpty) return;
			if(NativeVista.IsVista && !NativeVista.IsCompositionEnabled()) {
				using(Pen pen = new Pen(GetDragFrameColor(), 2)) {
					e.Cache.DrawRectangle(pen, e.ViewInfo.DragFrameRect);
				}
			}
		}
		protected virtual Color GetDragFrameColor() { return Color.Black; }
		protected virtual bool GetAllowSmartScroller(GridViewInfo viewInfo) {
			if(View.GridControl == null|| View.IsLoadingPanelVisible) return false;
			if(View.IsInListChangedEvent || View.IsAsyncInProgress) return false;
			if(View.AllowFixedGroups && !View.IsPixelScrolling) return false;
			return View.OptionsBehavior.AllowPartialRedrawOnScrolling && viewInfo.PaintAppearance.Empty.BackColor2.IsEmpty && 
				View.GridControl.allowFastScroll && !View.OptionsView.AllowCellMerge && View.TopRowIndex != 0;
		}
		protected internal virtual bool DrawScrolledContents(GridViewInfo viewInfo) {
			if(PaintPositionInfo.IsEmpty || !GetAllowSmartScroller(viewInfo)) return false;
			Point res = PaintPositionInfo.GetDistance(viewInfo);
			PaintPositionInfo.Clear();
			if(res.X == 0 || viewInfo.ViewRects.Rows.Height - Math.Abs(res.X) < 50) return false;
			for(int n = viewInfo.RowsInfo.Count - 1; n >= 0; n--) {
				GridDataRowInfo dataRow = viewInfo.RowsInfo[n] as GridDataRowInfo;
				if(dataRow != null && !dataRow.DetailBounds.IsEmpty)
					return false;
			}
			Rectangle scrollableBounds = PaintPositionInfo.GetCurrentScrollableBounds(viewInfo);
			bool valid = WindowScroller.ScrollVertical(View.GridControl, scrollableBounds, res.Y, res.X);
			if(!valid) return false;
			PaintPositionInfo.InvalidateNonScrollableArea(viewInfo);
			return true;
		}
		protected virtual void DrawContents(GridViewDrawArgs e) {
			Rectangle fill = e.ViewInfo.ViewRects.Client;
			if(!e.PaintArgs.ClipRectangle.IsEmpty && fill.Contains(e.PaintArgs.ClipRectangle) && e.ViewInfo.PaintAppearance.Empty.BackColor2.IsEmpty && e.ViewInfo.PaintAppearance.Empty.Image == null) {
				fill = e.PaintArgs.ClipRectangle;
			}
			StyleFillRectangle(e.Cache, e.ViewInfo.PaintAppearance.Empty, fill);
			if(IsOnlyLoadingPanel(e)) {
				DrawLoadingPanel(e);
				return;
			}
			DrawViewCaption(e);
			DrawGroupPanel(e);
			DrawColumnPanel(e);
			DrawRows(e);
			DrawFilterPanel(e);
			DrawTotalFooter(e);
			DrawLoadingPanel(e);
			PaintPositionInfo.Save(e.ViewInfo);
		}
		private void DrawScrollbars(GridViewDrawArgs e) {
			ScrollInfo si = View.ScrollInfo;
			if(!si.IsNeedToDrawScrollbar) return;
			if(!si.HBounds.IsEmpty) {
				var brush = e.Cache.GetSolidBrush(LookAndFeelHelper.GetSystemColor(View.ElementsLookAndFeel, SystemColors.Control));
				e.Cache.FillRectangle(brush, si.HBounds);
			} 
			if(si.VScrollVisible && !si.IsOverlapScrollBar) StyleFillRectangle(e.Cache, e.ViewInfo.PaintAppearance.Empty, si.VScrollRect);
		}
		bool IsOnlyLoadingPanel(GridViewDrawArgs e) {
			if(!View.IsAllowBusyPanel) return false;
			return View.LoadingPanel.IsOnlyLoadingPanelDraw(e.Cache);
		}
		protected virtual void DrawLoadingPanel(GridViewDrawArgs e) {
			if(!View.IsAllowBusyPanel) {
				if(View.LoadingPanel != null) View.LoadingPanel.Stop();
				return;
			}
			View.LoadingPanel.Bounds = e.ViewInfo.ViewRects.Rows;
			View.LoadingPanel.Draw(e.Cache);
		}
		protected virtual void DrawLoadingPanel2(GridViewDrawArgs e) {
			if(!View.IsAsyncInProgress) return;
			UserLookAndFeel lookAndFeel = View.ElementsLookAndFeel;
			ObjectPainter op = SkinElementPainter.Default;
			SkinElement element = BarSkins.GetSkin(lookAndFeel)[BarSkins.SkinAlertWindow];
			SkinElementInfo einfo = new SkinElementInfo(element);
			einfo.Cache = e.Cache;
			Rectangle rows = e.ViewInfo.ViewRects.Rows;
			Size min = new Size(300, 100);
			rows.Y += (rows.Height - min.Height) / 2;
			rows.X += (rows.Width - min.Width) / 2;
			if(min.Height > rows.Height || min.Width > rows.Width) return;
			rows.Size = min;
			einfo.Bounds = rows;
			op.CalcObjectBounds(einfo);
			op.DrawObject(einfo);
		}
		protected override ObjectState CalcFilterTextState(ViewDrawArgs e) { 
			if(View.MRUFilterPopup != null) return ObjectState.Pressed;
			return CalcObjectState(e as GridViewDrawArgs, GridHitTest.FilterPanelText, GridState.FilterPanelTextPressed);
		}
		protected override ObjectState CalcFilterCloseButtonState(ViewDrawArgs e) { 
			return CalcObjectState(e as GridViewDrawArgs, GridHitTest.FilterPanelCloseButton, GridState.FilterPanelCloseButtonPressed);
		}
		protected override ObjectState CalcFilterCustomizeButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as GridViewDrawArgs, GridHitTest.FilterPanelCustomizeButton, GridState.FilterPanelCustomizeButtonPressed);
		}
		protected override ObjectState CalcFilterMRUButtonState(ViewDrawArgs e) { 
			if(View.MRUFilterPopup != null) return ObjectState.Pressed;
			return CalcObjectState(e as GridViewDrawArgs, GridHitTest.FilterPanelMRUButton, GridState.FilterPanelMRUButtonPressed);
		}
		protected override ObjectState CalcFilterActiveButtonState(ViewDrawArgs e) { 
			return CalcObjectState(e as GridViewDrawArgs, GridHitTest.FilterPanelActiveButton, GridState.FilterPanelActiveButtonPressed);
		}
		ObjectState CalcObjectState(GridViewDrawArgs e, GridHitTest hitTest, GridState gridState) {
			ObjectState state = ObjectState.Normal;
			if(e.ViewInfo.SelectionInfo.HotTrackedInfo.HitTest == hitTest) state |= ObjectState.Hot;
			if(View.State == gridState) state |= ObjectState.Pressed;
			return state;
		}
		protected override void DrawIndentCore(ViewDrawArgs e, IndentInfo indent, ObjectInfoArgs sourceObject) {
			if(indent.Appearance != null && indent.IsGroupStyle) {
				ElementsPainter.GroupRow.DrawGroupRowIndent(sourceObject as GridRowInfo, e.Cache, indent.Appearance, indent.Bounds);
				return;
			}
			base.DrawIndentCore(e, indent, sourceObject);
		}
		protected virtual void DrawColumnCore(GridViewDrawArgs e, GridColumnInfoArgs ci) {
			Rectangle r = ci.Bounds;
			ci.Cache = e.Cache;
			try {
				if(!ci.TrueBounds.IsEmpty) ci.Bounds = ci.TrueBounds;
				ColumnHeaderCustomDrawEventArgs cs = new ColumnHeaderCustomDrawEventArgs(e.Cache, ElementsPainter.Column, ci);
				cs.SetDefaultDraw(() => {
					ElementsPainter.Column.DrawObject(ci);
				});
				View.RaiseCustomDrawColumnHeader(cs);
				cs.DefaultDraw();
			}
			finally {
				ci.Bounds = r;
				ci.Cache = null;
			}
		}
		protected virtual void DrawColumnIndicator(GridViewDrawArgs e) {
			if(e.ViewInfo.ColumnsInfo.Count > 0) {
				GridColumnInfoArgs ci = e.ViewInfo.ColumnsInfo[0];
				if(ci.Type == GridColumnInfoType.Indicator) {
					int imageIndex = -1;
					if(View.IsDetailView && View.ParentView.IsAllowZoomDetail) {
						imageIndex = View.IsZoomedView ? IndicatorNoZoom : IndicatorZoom;
					}
					AppearanceObject appearance = null;
					DrawIndicator(e, ci.Bounds, GridControl.InvalidRowHandle, imageIndex, appearance, IndicatorKind.Header, ci);
					DrawLoadingIndicator(e, ci);
				}
			}
		}
		protected virtual void DrawLoadingIndicator(GridViewDrawArgs e, GridColumnInfoArgs ci) {
			if(!View.IsAllowBusyIndicator) {
				View.LoadingAnimator.StopAnimation();
				return;
			}
			Rectangle b = Rectangle.Inflate(ci.Bounds, -2, -2);
			View.LoadingAnimator.Bounds = ci.Bounds;
			View.LoadingAnimator.DrawAnimatedItem(e.Cache, b);
		}
		protected virtual void DrawColumn(GridViewDrawArgs e, GridColumnInfoArgs ci) {
			if(ci.Type == GridColumnInfoType.EmptyColumn) return;
			e.ViewInfo.CalcColumnInfoState(ci);
			DrawColumnCore(e, ci);
		}
		public class GridClipInfo {
			public bool ShouldPaint, Clipped;
			public GraphicsClipState ClipState;
			public GraphicsCache Cache;
			public GridClipInfo(GraphicsCache cache) {
				this.Cache = cache;
				this.ClipState = null;
				this.ShouldPaint = true;
				this.Clipped = false;
			}
			public virtual void Prepare(GridViewDrawArgs e, GridColumnInfoArgs ci, Rectangle bounds) {
				if(e.ViewInfo.HasFixedColumns && !e.ViewInfo.IsFixedLeftPaint(ci)) {
					if(ci.Type == GridColumnInfoType.Indicator || ci.Type == GridColumnInfoType.BehindColumn) return;
					Rectangle clipR = CheckRectangle(e.ViewInfo.UpdateFixedRange(bounds, ci));
					if(clipR.Width == 0) {
						this.ShouldPaint = false;
					}
					else {
						if(clipR != bounds) {
							ClipState = Cache.ClipInfo.SaveAndSetClip(clipR, true);
							Cache.ClipInfo.MaximumBounds = Cache.ClipInfo.CheckBounds(clipR);
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
			public virtual void Restore(GridViewDrawArgs e) {
				if(Clipped) {
					this.Clipped = false;
					Cache.ClipInfo.RestoreClipRelease(ClipState);
					ClipState = null;
				}
			}
		}
		protected virtual GridClipInfo PrepareClipInfo(GridViewDrawArgs e, GridColumnInfoArgs ci, Rectangle bounds) {
			GridClipInfo cli = new GridClipInfo(e.Cache);
			cli.Prepare(e, ci, bounds);
			return cli;
		}
		protected virtual void DrawColumnPanelBackground(GridViewDrawArgs e) {
		}
		protected virtual void DrawColumnPanel(GridViewDrawArgs e) {
			Graphics g = e.Graphics;
			if(!View.OptionsView.ShowColumnHeaders || !IsNeedDrawRect(e, e.ViewInfo.ViewRects.ColumnPanelActual)) return;
			DrawColumnPanelBackground(e);
			DrawColumnIndicator(e);
			GraphicsClipState clipState = null;
			if(e.ViewInfo.ViewRects.IndicatorWidth > 0) {
				Rectangle clipr = e.ViewInfo.ViewRects.ColumnPanelActual;
				if(e.ViewInfo.IsRightToLeft) {
					clipr.Width -= e.ViewInfo.ViewRects.IndicatorWidth;
				}
				else {
				clipr.X += e.ViewInfo.ViewRects.IndicatorWidth;
				clipr.Width -= e.ViewInfo.ViewRects.IndicatorWidth;
				}
				clipState = e.Cache.ClipInfo.SaveAndSetClip(clipr, true, true);
			}
			foreach(GridColumnInfoArgs ci in e.ViewInfo.ColumnsInfo) {
				Rectangle columnClipBounds = ci.Bounds;
				bool fixedLeft = e.ViewInfo.IsFixedLeft(ci);
				bool fixedRight = e.ViewInfo.IsFixedRight(ci);
				if(e.ViewInfo.IsRightToLeft) {
					var f = fixedLeft;
					fixedLeft = fixedRight;
					fixedRight = f;
				}
				if(fixedLeft || fixedRight) { 
					columnClipBounds.X -= View.FixedLineWidth;
					columnClipBounds.Width = View.FixedLineWidth;
				}
				GridClipInfo cli = PrepareClipInfo(e, ci, columnClipBounds);
				if(cli.ShouldPaint)
					DrawColumn(e, ci);
				Rectangle r = Rectangle.Empty;
				if(fixedLeft) {
					r = ci.Bounds;
					r.X = ci.Bounds.Right;
					r.Width = View.FixedLineWidth;
				}
				if(fixedRight) {
					r = ci.Bounds;
					r.X -= View.FixedLineWidth;
					r.Width = View.FixedLineWidth;
					Rectangle fleft = e.ViewInfo.ViewRects.FixedLeft;
					if(e.ViewInfo.IsRightToLeft) fleft = e.ViewInfo.ViewRects.FixedRight;
					if(!fleft.IsEmpty && fleft.Right >= r.X) {
						int delta = fleft.Right - r.X;
						r.X += delta;
						r.Width -= delta;
						if(r.Width < 1) r = Rectangle.Empty;
					}
				}
				if(!r.IsEmpty) {
					AppearanceObject appearance = e.ViewInfo.PaintAppearance.FixedLine;
					StyleFillRectangle(e.Cache, appearance, appearance.GetBackColor(), r);
				}
				cli.Restore(e);
			}
			e.Cache.ClipInfo.RestoreClipRelease(clipState);
		}
		protected virtual void DrawFooterCells(GridViewDrawArgs e, GridRowFooterInfo fi, bool isRowFooter) {
			Graphics g = e.Graphics;
			foreach(GridFooterCellInfoArgs fci in fi.Cells) {
				if(fci.Visible)	{
					Rectangle r = fci.Bounds;
					GridClipInfo cli = PrepareClipInfo(e, fci.ColumnInfo, r);
					if(!cli.ShouldPaint) {
						cli.Restore(e);
						continue;
					}
					ObjectPainter painter = isRowFooter ? ElementsPainter.GroupFooterCell : ElementsPainter.FooterCell;
					fci.Cache = e.Cache;
					FooterCellCustomDrawEventArgs cs = new FooterCellCustomDrawEventArgs(e.Cache, (isRowFooter ? fi.RowHandle : GridControl.InvalidRowHandle), fci.Column, painter, fci);
					cs.SetDefaultDraw(() => {
						painter.DrawObject(fci);
					});
					if(isRowFooter)
						View.RaiseCustomDrawRowFooterCell(cs);
					else
						View.RaiseCustomDrawFooterCell(cs);
					if(!cs.Handled) cs.DefaultDraw();
					fci.Cache = null;
					cli.Restore(e);
				}
			}
		}
		protected virtual void DrawTotalFooter(GridViewDrawArgs e) {
			e.ViewInfo.CheckFooterData();
			DrawFooter(e, e.ViewInfo.FooterInfo);
		}
		public virtual void DrawFooter(GridViewDrawArgs e, GridRowFooterInfo fi) {
			if(fi.Bounds.IsEmpty) return;
			if(!IsNeedDrawRect(e, fi.Bounds)) return;
			StyleObjectInfoArgs panel = new StyleObjectInfoArgs(e.Cache, fi.Bounds, e.ViewInfo.PaintAppearance.FooterPanel.Clone() as AppearanceObject, ObjectState.Normal);
			RowObjectCustomDrawEventArgs cs = new RowObjectCustomDrawEventArgs(e.Cache, GridControl.InvalidRowHandle, ElementsPainter.FooterPanel, panel, panel.Appearance);
			Rectangle r = panel.Bounds;
			cs.SetDefaultDraw(() => {
				ElementsPainter.FooterPanel.DrawObject(panel);
				r = ElementsPainter.FooterPanel.GetObjectClientRectangle(panel);
			});
			View.RaiseCustomDrawFooter(cs);
			cs.DefaultDraw();
			if(e.ViewInfo.IsRightToLeft) {
				int right = e.ViewInfo.ViewRects.Rows.Right - e.ViewInfo.ViewRects.IndicatorWidth;
				if(r.Right > right) r.Width = right - r.X;
			}
			else {
				int dataRowStartX = e.ViewInfo.ViewRects.Rows.X + e.ViewInfo.ViewRects.IndicatorWidth;
				if(dataRowStartX > r.X) {
					r.Width = r.Right - dataRowStartX;
					r.X = dataRowStartX;
				}
			}
			GraphicsClipState clipState = e.Cache.ClipInfo.SaveAndSetClip(r);
			DrawFooterCells(e, fi, false);
			e.Cache.ClipInfo.RestoreClipRelease(clipState);
		}
		protected virtual void DrawViewCaption(GridViewDrawArgs e) {
			if(!IsNeedDrawRect(e, e.ViewInfo.ViewRects.ViewCaption)) return;
			StyleObjectInfoArgs panel = new StyleObjectInfoArgs(e.Cache, e.ViewInfo.ViewRects.ViewCaption, e.ViewInfo.PaintAppearance.ViewCaption.Clone() as AppearanceObject, ObjectState.Normal);
			ElementsPainter.ViewCaption.DrawObject(panel);
		}
		protected virtual void DrawGroupPanel(GridViewDrawArgs e) {
			Graphics g = e.Graphics;
			AppearanceObject groupStyle = e.ViewInfo.PaintAppearance.GroupPanel;
			if(!IsNeedDrawRect(e, e.ViewInfo.ViewRects.GroupPanel)) return;
			Rectangle r = e.ViewInfo.ViewRects.GroupPanel;
			CustomDrawEventArgs cs = new CustomDrawEventArgs(e.Cache, r, groupStyle);
			StyleObjectInfoArgs groupArgs = new StyleObjectInfoArgs(e.Cache, r, cs.Appearance, ObjectState.Normal);
			groupArgs.RightToLeft = e.ViewInfo.IsRightToLeft;
			cs.SetDefaultDraw(() => {
				groupArgs = new StyleObjectInfoArgs(e.Cache, r, cs.Appearance, ObjectState.Normal);
				groupArgs.RightToLeft = e.ViewInfo.IsRightToLeft;
				ElementsPainter.GroupPanel.DrawObject(groupArgs);
			});
			View.RaiseCustomDrawGroupPanel(cs);
			if(!cs.Handled) cs.DefaultDraw();
			if(e.ViewInfo.GroupPanel.Rows.Count == 0) {
				ElementsPainter.GroupPanel.DrawEmptyText(groupArgs, e.ViewInfo.GetGroupPanelText());
			}
			else {
				foreach(GroupPanelRow row in e.ViewInfo.GroupPanel.Rows) {
					row.Painter.DrawGroupPanelRow(e, row, groupStyle);
				}
			}
		}
		protected virtual void DrawGroupPanelRow(GridViewDrawArgs e, GroupPanelRow row, AppearanceObject appearance) {
			Graphics g = e.Graphics;
			GridColumnInfoArgs prevci = null;
			int count = 0;
			for(int i = row.CaptionInfo != null ? -1 : 0; i < row.ColumnsInfo.Count; i++) {
				GridColumnInfoArgs ci = i == -1 ? row.CaptionInfo : row.ColumnsInfo[i] as GridColumnInfoArgs;
				if(ci == null) continue;
				DrawColumn(e, ci);
				if(count ++ > 0) { 
					if(row.LineStyle) {
						if(prevci != null) {
							int x = prevci.Bounds.Right;
							int y = prevci.Bounds.Height / 2 + prevci.Bounds.Y;
							int dx = ci.Bounds.X;
							if(e.ViewInfo.IsRightToLeft) {
								dx = x + 1;
								x = ci.Bounds.Right;
							}
							StyleFillRectangle(e.Cache, appearance, appearance.GetForeColor(), new Rectangle(x, y, dx - x, 1));
						}
					}
					else {
						int sx = prevci.Bounds.Left + prevci.Bounds.Width / 2;
						int sy = prevci.Bounds.Bottom;
						StyleFillRectangle(e.Cache, appearance, appearance.GetForeColor(), new Rectangle(sx, sy, 1, 3));
						sy += 3;
						int dx = ci.Bounds.Left;
						if(e.ViewInfo.IsRightToLeft) {
							dx = sx + 1;
							sx = ci.Bounds.Right;
						}
						StyleFillRectangle(e.Cache, appearance, appearance.GetForeColor(), new Rectangle(sx, sy, dx - sx, 1));
					}
				}
				prevci = ci;
			}
		}
		protected virtual void DrawGroupRow(GridViewDrawArgs e, GridGroupRowInfo ri) {
			ri.Cache = e.Cache;
			RowObjectCustomDrawEventArgs cs = new RowObjectCustomDrawEventArgs(e.Cache, ri.RowHandle, ElementsPainter.GroupRow, ri, ri.Appearance);
			cs.SetDefaultDraw(() => {
				ElementsPainter.GroupRow.DrawGroupRowBackground(ri);
				ObjectPainter.DrawObject(e.Cache, ElementsPainter.GroupRow, ri);
			});
			View.RaiseCustomDrawGroupRow(cs);
			cs.DefaultDraw();
		}
		protected virtual void DrawIndent(GridViewDrawArgs e, GridRowInfo ri) {
			Graphics g = e.Graphics;
			DrawIndents(e, ri.Indents, ri);
			GridDataRowInfo dataRow = ri as GridDataRowInfo;
			if(dataRow != null && !dataRow.DetailIndentBounds.IsEmpty) { 
				AppearanceObject app = new AppearanceObject();
				e.ViewInfo.fDetailIndentDrawingInternal = true;
				app = e.ViewInfo.GetRowAppearance(ri, app, ri.RowState & ~(GridRowCellState.Focused | GridRowCellState.Selected));
				e.ViewInfo.fDetailIndentDrawingInternal = false;
				StyleFillRectangle(e.Cache, app, app.GetBackColor(), dataRow.DetailIndentBounds);
			}
		}
		protected virtual void DrawIndicatorCore(GridViewDrawArgs e, IndicatorObjectInfoArgs info, int rowHandle, IndicatorKind kind) {
			info.Cache = e.Cache;
			try {
				RowIndicatorCustomDrawEventArgs cs = new RowIndicatorCustomDrawEventArgs(e.Cache, rowHandle, ElementsPainter.Indicator, info);
				cs.SetDefaultDraw(() => {
					GetIndicatorPainter(kind).DrawObject(info);
				});
				View.RaiseCustomDrawRowIndicator(cs);
				cs.DefaultDraw();
			}
			finally {
				info.Cache = null;
			}
		}
		protected virtual void DrawIndicator(GridViewDrawArgs e, Rectangle r, int rowHandle, int imageIndex, AppearanceObject appearance, IndicatorKind kind, HeaderObjectInfoArgs columnInfo) {
			appearance = (appearance == null ? e.ViewInfo.PaintAppearance.GetIndicatorAppearance() : appearance);
			IndicatorObjectInfoArgs args = new IndicatorObjectInfoArgs(r, appearance, ElementsPainter.GetIndicatorImages(Indicator), imageIndex, kind);
			args.RightToLeft = e.ViewInfo.IsRightToLeft;
			if(columnInfo != null) args.IsTopMost = columnInfo.IsTopMost;
			DrawIndicatorCore(e, args, rowHandle, kind);
		}
		protected virtual ObjectPainter GetIndicatorPainter(IndicatorKind kind) {
			if(kind == IndicatorKind.Header) return ElementsPainter.ColumnIndicator;
			return ElementsPainter.Indicator;
		}
		protected virtual void DrawIndicatorCell(GridViewDrawArgs e, GridRowInfo ri, bool focusedRow) { 
			Graphics g = e.Graphics;
			if(e.ViewInfo.ViewRects.IndicatorWidth == 0) return;
			int imageIndex = -1;
			GridDataRowInfo dataRow = ri as GridDataRowInfo;
			bool hasError = (dataRow != null && dataRow.ErrorText != null && dataRow.ErrorText.Length > 0);
			if(View.IsNewItemRow(ri.RowHandle)) 
				imageIndex = IndicatorNewItemRow;
			if(hasError) imageIndex = IndicatorError;
			if(focusedRow) {
				imageIndex = IndicatorFocused;
				if(View.State == GridState.Editing) {
					imageIndex = IndicatorFocused;
				}
				if(View.IsEditing)
					imageIndex = IndicatorEditing;
				if(View.FocusedRowModified)
					imageIndex = IndicatorChanged;
				if(hasError)
					imageIndex = IndicatorFocusedError;
			}
			if(View.IsFilterRow(ri.RowHandle)) imageIndex = IndicatorFilterRow;
			IndicatorObjectInfoArgs args = new IndicatorObjectInfoArgs(ri.IndicatorRect, e.ViewInfo.PaintAppearance.GetIndicatorAppearance(), ElementsPainter.GetIndicatorImages(Indicator), imageIndex, IndicatorKind.Row);
			args.RightToLeft = e.ViewInfo.IsRightToLeft;
			args.IsTopMost = ri.VisibleIndex == 0 && !e.ViewInfo.IsShowHeaders;
			DrawIndicatorCore(e, args, ri.RowHandle, IndicatorKind.Row);
		}
		protected virtual void DrawMasterRowIndicator(GridViewDrawArgs e, GridDataRowInfo ri) {
			if(ri == null || ri.DetailIndicatorBounds.IsEmpty) return;
			DrawIndicator(e, ri.DetailIndicatorBounds, ri.RowHandle, -1, null, IndicatorKind.Detail, null);
		}
		protected virtual void DrawRowFooterIndicator(GridViewDrawArgs e, GridRowInfo ri) {
			foreach(GridRowFooterInfo fi in ri.RowFooters) {
				if(fi.Bounds.IsEmpty || fi.IndicatorRect.IsEmpty) return;
				DrawIndicator(e, fi.IndicatorRect, ri.RowHandle, -1, null, IndicatorKind.RowFooter, null);
			}
		}
		protected virtual bool CanDrawRowFocus(GridRowInfo ri) {
			return ((ri.RowState & GridRowCellState.FocusedAndGridFocused) == GridRowCellState.FocusedAndGridFocused && (View.FocusRectStyle == DrawFocusRectStyle.RowFocus || View.FocusRectStyle == DrawFocusRectStyle.RowFullFocus || (ri.IsGroupRow &&  View.FocusRectStyle != DrawFocusRectStyle.None))); 
		}
		protected virtual void DrawRegularRow(GridViewDrawArgs e, GridDataRowInfo ri) {
			int cellCount = ri.Cells.Count;
			StyleFillRectangle(e.Cache, ri.Appearance, ri.DataBounds);
			for(int n = 0; n < cellCount; n++) {
				GridCellInfo ci = (GridCellInfo)ri.Cells[n];
				if(ci.ColumnInfo.Type != GridColumnInfoType.Column && ci.ColumnInfo.Type != GridColumnInfoType.EmptyColumn) continue;
				DrawRegularRowCell(e, ci);
			}
		}
		protected virtual void DrawRegularRowCell(GridViewDrawArgs e, GridCellInfo ci) {
			ci.CustomDrawHandled = true;
			if(ci.MergedCell != null || ci == GridCellInfo.EmptyCellInfo) return;
			GridClipInfo cli = PrepareClipInfo(e, ci.ColumnInfo, ci.Bounds);
			try {
				if(cli.ShouldPaint) {
					e.ViewInfo.RequestCellEditViewInfo(ci);
					if(ci.ViewInfo != null) {
						if(View.IsFilterRow(ci.RowHandle) && ci.CellValue == null) {
							ci.ViewInfo.SetDisplayText(string.Empty);
						}
						ci.ViewInfo.SetDisplayText(View.RaiseCustomColumnDisplayText(ci.RowHandle, ci.Column, ci.CellValue, ci.ViewInfo.DisplayText, false), ci.CellValue);
						ci.ViewInfo.MatchedString = string.Empty;
						if(ci.RowHandle == View.FocusedRowHandle && !View.IsFilterRow(ci.RowHandle)) {
							if(View.State == GridState.IncrementalSearch && ci.Column == View.FocusedColumn && View.IncrementalText != "") {
								ci.ViewInfo.MatchedString = View.IncrementalText;
								ci.ViewInfo.MatchedStringAllowPartial = View.IsServerMode;
							}
							if(View.WorkAsLookup && ci.Column.IsLookUpDisplayColumn) {
								ci.ViewInfo.MatchedString = View.ExtraFilterText;
								ci.State = GridRowCellState.Dirty;
							}
						}
						if(View.WorkAsLookup && View.LookUpOwner.GetFilterMode() == PopupFilterMode.Contains && ci.Column.IsLookUpDisplayColumn) {
							if(!View.IsFilterRow(ci.RowHandle)) {
								ci.ViewInfo.UseHighlightSearchAppearance = true;
								ci.ViewInfo.MatchedStringUseContains = true;
								ci.ViewInfo.MatchedString = View.ExtraFilterText;
								ci.State = GridRowCellState.Dirty;
							}
						}
						if(!View.IsFilterRow(ci.RowHandle) && View.IsAllowHighlightFind(ci.Column)) {
							ci.ViewInfo.UseHighlightSearchAppearance = true;
							ci.ViewInfo.MatchedStringUseContains = true;
							ci.ViewInfo.MatchedString = View.GetFindMatchedText(ci.Column, ci.ViewInfo.DisplayText);
							ci.State = GridRowCellState.Dirty;
						}
					}
					e.ViewInfo.UpdateCellAppearance(ci);
					DrawRowCell(e, ci);
				}
			}
			finally {
				cli.Restore(e);
			}
		}
		bool IsBackEquals(AppearanceObject app1, AppearanceObject app2) {
			return app1 == app2 || (app1.GetBackColor() == app2.GetBackColor() && app1.GetBackColor2() == app2.GetBackColor2() &&
				app1.GetGradientMode() == app2.GetGradientMode());
		}
		protected virtual bool DrawNewItemRow(GridViewDrawArgs e, GridRowInfo ri) {
			if(e.ViewInfo.NewItemRow != NewItemRowPosition.Top) return false;
			if((ri.RowState & GridRowCellState.Focused) != 0) return false;
			Rectangle r = ri.DataBounds;
			if(r.X < e.ViewInfo.ViewRects.ColumnPanelLeft) r.X = e.ViewInfo.ViewRects.ColumnPanelLeft;
			if(r.Right > e.ViewInfo.ViewRects.Rows.Right) r.Width = e.ViewInfo.ViewRects.Rows.Right - r.X;
			StyleFillRectangle(e.Cache, ri.Appearance, r);
			ri.Appearance.DrawString(e.Cache, e.ViewInfo.GetNewItemRowText(), r);
			DrawRowIndent(e, ri);
			return true;
		}
		protected virtual void DrawSpecialTopRowIndent(GridViewDrawArgs e, GridRowInfo ri) {
			ObjectPainter.DrawObject(e.Cache, ElementsPainter.SpecialTopRowIndent, new StyleObjectInfoArgs(e.Cache, ri.RowSeparatorBounds, e.ViewInfo.PaintAppearance.HeaderPanel));
		}
		Rectangle GetRowClipBounds(GridViewDrawArgs e, GridRowInfo ri, Rectangle bounds, bool excludeIndicator) {
			Rectangle rx = bounds, clipRect;
			if(excludeIndicator) {
				if(e.ViewInfo.IsRightToLeft) {
					if(rx.Right > e.ViewInfo.ViewRects.Rows.Right - e.ViewInfo.ViewRects.IndicatorWidth) {
						int delta = rx.Right - (e.ViewInfo.ViewRects.Rows.Right - e.ViewInfo.ViewRects.IndicatorWidth);
						rx.Width -= delta;
					}
				}
				else {
					if(rx.X < e.ViewInfo.ViewRects.Rows.X + e.ViewInfo.ViewRects.IndicatorWidth) {
				int delta = (e.ViewInfo.ViewRects.Rows.X + e.ViewInfo.ViewRects.IndicatorWidth) - rx.X;
				rx.X += delta;
				rx.Width -= delta;
			}
				}
			}
			if(rx.Right > e.ViewInfo.ViewRects.Rows.Right)
				rx.Width -= (rx.Right - e.ViewInfo.ViewRects.Rows.Right);
			if(!e.ViewInfo.IsRightToLeft) { 
				if(ri != null && rx.Right > ri.DataBounds.Right)
					rx.Width -= (rx.Right - ri.DataBounds.Right);
			}
			clipRect = rx;
			Rectangle rowsRect = e.ViewInfo.ViewRects.Rows;
			int lastFixedRowIndex = e.ViewInfo.RowsInfo.GetFirstScrollableRowIndex(View) - 1;
			if(lastFixedRowIndex > -1) {
				int index = e.ViewInfo.RowsInfo.IndexOf(ri);
				if(index > lastFixedRowIndex) {
					GridRowInfo lastFixedRow = e.ViewInfo.RowsInfo[lastFixedRowIndex];
					rowsRect.Y = lastFixedRow.TotalBounds.Bottom;
					rowsRect.Height = e.ViewInfo.ViewRects.Rows.Bottom - rowsRect.Y;
				}
			}
			if(clipRect.Top < rowsRect.Top) {
				int bottom = clipRect.Bottom;
				clipRect.Y = rowsRect.Top;
				clipRect.Height = bottom - clipRect.Y;
			}
			if(clipRect.Bottom > rowsRect.Bottom) {
				clipRect.Height -= (clipRect.Bottom - rowsRect.Bottom);
			}
			return clipRect;
		}
		protected virtual void DrawRow(GridViewDrawArgs e, GridRowInfo ri) {
			if(!IsNeedDrawRect(e, ri.TotalBounds)) return;
			e.ViewInfo.UpdateBeforePaint(ri);
			Rectangle rowClipBounds = GetRowClipBounds(e, ri, ri.TotalBounds, true);
			if(rowClipBounds.Height < 1) return; 
			GraphicsClipState clipState = e.Cache.ClipInfo.SaveAndSetClip(GetRowClipBounds(e, ri, ri.TotalBounds, false), true, true);
			DrawIndicatorCell(e, ri, (ri.RowState & GridRowCellState.Focused) != 0);
			DrawMasterRowIndicator(e, ri as GridDataRowInfo);
			DrawRowFooterIndicator(e, ri);
			e.Cache.ClipInfo.RestoreClipRelease(clipState);
			GridDataRowInfo dataRow = ri as GridDataRowInfo;
			if(dataRow != null && dataRow.IsNewItemRow) {
				if(DrawNewItemRow(e, dataRow)) return;
			}
			clipState = e.Cache.ClipInfo.SaveAndSetClip(rowClipBounds, true, true);
			try {
				DrawIndent(e, ri);
				if(ri.IsGroupRow) {
					DrawGroupRow(e, ri as GridGroupRowInfo);
				} else {
					DrawRegularRow(e, dataRow);
					DrawRowPreview(e, dataRow);
				}
				DrawRowFooters(e, ri);
				DrawRowLines(e, ri);
				if(dataRow != null) DrawTransparentCells(e, dataRow);
				DrawRowFocus(e, ri);
				if((ri.RowState & GridRowCellState.Selected) != 0 && View.OptionsSelection.EnableAppearanceFocusedRow) {
					if(e.ViewInfo.IsTransparentFocusedStyle || e.ViewInfo.IsTransparentSelectedStyle) {
						AppearanceObject app = e.ViewInfo.GetRowTransparentAppearance(ri);
						if(app != null) app.FillRectangle(e.Cache, ri.DataBounds, true);
					}
				}
			}
			finally {
				e.Cache.ClipInfo.RestoreClipRelease(clipState);
			}
			DrawRowIndent(e, ri);
		}
		protected virtual void DrawRowFocus(GridViewDrawArgs e, GridRowInfo ri) {
			if(!ri.IsGroupRow && CanDrawRowFocus(ri)) {
				Rectangle r = ri.DataBounds;
				GridDataRowInfo dataRow = ri as GridDataRowInfo;
				if(View.GetShowHorizontalLines()) r.Height--;
				if(View.DrawFullRowFocus && !dataRow.PreviewBounds.IsEmpty) {
					r.Height = dataRow.PreviewBounds.Bottom - r.Y;
				}
				if(View.GetShowVerticalLines()) r.Width --;
				if(ri.IndicatorRect.Width > 0) {
					r.Width = r.Right - ri.IndicatorRect.Right;
					r.X = ri.IndicatorRect.Right;
				}
				if(AllowDrawFocusRectangle) e.Cache.Paint.DrawFocusRectangle(e.Graphics, r, ri.Appearance.GetForeColor(), ri.Appearance.GetBackColor());
			}
		}
		protected bool AllowDrawFocusRectangle {
			get {
				return true;
			}
		}
		protected virtual void DrawTransparentCells(GridViewDrawArgs e, GridDataRowInfo ri) {
			for(int n = 0; n < ri.Cells.Count; n++) {
				GridCellInfo ci = (GridCellInfo)ri.Cells[n];
				if(ci.ColumnInfo.Type != GridColumnInfoType.Column || ci.CustomDrawHandled) continue;
				AppearanceObject transparent = e.ViewInfo.GetCellTransparentAppearance(ci);
				if(transparent != null) {
					Rectangle cellBounds = ci.Bounds;
					if(View.GetShowHorizontalLines()) cellBounds.Height += 1;
					if(View.GetShowVerticalLines()) cellBounds.Width += 1;
					GridClipInfo cli = PrepareClipInfo(e, ci.ColumnInfo, cellBounds);
					try {
						if(cli.ShouldPaint) {
					StyleFillRectangle(e.Cache, transparent, cellBounds);
				}
			}
					finally {
						cli.Restore(e);
					}
				}
			}
		}
		protected virtual bool AllowDrawCellBackground { get { return false; } }
		protected virtual void DrawRowIndent(GridViewDrawArgs e, GridRowInfo ri) {
			if(ri.RowSeparatorBounds.IsEmpty) return;
			GridDataRowInfo dataRow = ri as GridDataRowInfo;
			if(dataRow != null && (View.IsFilterRow(dataRow.RowHandle) || (dataRow.IsNewItemRow && View.OptionsView.NewItemRowPosition == NewItemRowPosition.Top))) {
				DrawSpecialTopRowIndent(e, dataRow);
				return;
			}
			Rectangle r = ri.RowSeparatorBounds;
			AppearanceObject appearance;
			if(View.GetShowHorizontalLines()) { 
				r.X = e.ViewInfo.ViewRects.Rows.X ;
				r.Width = ri.DataBounds.X - r.X;
				r.Y = r.Bottom - 1;
				r.Height = 1;
				if(r.Width > 0) {
					Rectangle line = r;
					GridRowInfo nextRow = e.ViewInfo.RowsInfo.NextRow(ri);
					if(nextRow == null) nextRow = ri;
					line.X = nextRow.DataBounds.X;
					line.Width = nextRow.DataBounds.Width;
					appearance = e.ViewInfo.PaintAppearance.HorzLine;
					StyleFillRectangle(e.Cache, appearance, appearance.GetBackColor(), line);
				}
				r = ri.RowSeparatorBounds;
				r.Height --;
			}
			StyleFillRectangle(e.Cache, e.ViewInfo.PaintAppearance.RowSeparator, r);
		}
		protected virtual void DrawRowFooters(GridViewDrawArgs e, GridRowInfo ri) {
			foreach(GridRowFooterInfo fi in ri.RowFooters) {
				DrawRowFooter(e, fi);
			}
		}
		protected virtual void DrawRowFooter(GridViewDrawArgs e, GridRowFooterInfo fi) {
			if(fi.Bounds.IsEmpty) return;
			if(!IsNeedDrawRect(e, fi.Bounds)) return;
			Graphics g = e.Graphics;
			StyleObjectInfoArgs panelArgs = new StyleObjectInfoArgs(e.Cache, fi.Bounds, e.ViewInfo.PaintAppearance.GroupFooter.Clone() as AppearanceObject, ObjectState.Normal);
			RowObjectCustomDrawEventArgs cs = new RowObjectCustomDrawEventArgs(e.Cache, fi.RowHandle, ElementsPainter.GroupFooterPanel, panelArgs, panelArgs.Appearance);
			cs.SetDefaultDraw(() => {
				ElementsPainter.GroupFooterPanel.DrawObject(panelArgs);
			});
			View.RaiseCustomDrawRowFooter(cs);
			cs.DefaultDraw();
			DrawFooterCells(e, fi, true);
		}
		protected virtual void DrawRowCell(GridViewDrawArgs e, GridCellInfo cell) {
			if(cell.Bounds.IsEmpty) return;
			bool isInvertSelection = !View.IsMultiSelect && View.OptionsSelection.InvertSelection;
			Graphics g = e.Graphics;
			bool isFocusCell = (cell.State & GridRowCellState.FocusedCell) != 0 && (cell.State & GridRowCellState.GridFocused) != 0;
			bool isSelectedRow = (cell.State & GridRowCellState.Selected) != 0;
			bool isFocusedRow = (cell.State & GridRowCellState.Focused) != 0;
			AppearanceObject appearance = cell.Appearance;
			BaseEditViewInfo editInfo = e.ViewInfo.RequestCellEditViewInfo(cell);
			if(editInfo != null) {
				cell.CustomDrawHandled = false;
				RowCellCustomDrawEventArgs cs = new RowCellCustomDrawEventArgs(e.Cache, cell);
				cs.SetDefaultDraw(() => {
					if(cs.changed) editInfo.SetDisplayText(cs.DisplayText, cs.CellValue);
					appearance = cs.Appearance;
					DrawRowCellInternal(e, cell, isInvertSelection, g, isFocusCell, isSelectedRow, isFocusedRow, appearance, editInfo);
				});
				View.RaiseCustomDrawCell(cs);
				cell.CustomDrawHandled = cs.Handled;
				cs.DefaultDraw();
				return;
			}
			DrawRowCellInternal(e, cell, isInvertSelection, g, isFocusCell, isSelectedRow, isFocusedRow, appearance, editInfo);
		}
		private void DrawRowCellInternal(GridViewDrawArgs e, GridCellInfo cell, bool isInvertSelection, Graphics g, bool isFocusCell, bool isSelectedRow, bool isFocusedRow, AppearanceObject appearanceValue, BaseEditViewInfo editInfo) {
			if(AllowDrawCellBackground || !IsBackEquals(cell.RowInfo.Appearance, appearanceValue) || (cell is GridMergedCellInfo))
				StyleFillRectangle(e.Cache, appearanceValue, cell.Bounds);
			DrawAppearanceMethod drawMethod = (cache, appearance) => {
				if(isFocusCell && View.FocusRectStyle == DrawFocusRectStyle.CellFocus && AllowDrawFocusRectangle)
					e.Cache.Paint.DrawFocusRectangle(g, cell.Bounds, appearance.GetForeColor(), appearance.GetBackColor());
				if(cell.ColumnInfo.Type == GridColumnInfoType.Column) {
					if(!cell.CellButtonRect.IsEmpty) {
						ObjectPainter.DrawObject(e.Cache, ElementsPainter.DetailButton,
							new DetailButtonObjectInfoArgs(cell.CellButtonRect, View.GetMasterRowExpanded(cell.RowHandle), cell.RowInfo.IsMasterRowEmpty));
					}
					DrawCellEdit(e, editInfo, cell, appearance, (isSelectedRow && !isFocusCell && (isFocusedRow && !isInvertSelection && View.OptionsSelection.EnableAppearanceFocusedRow)) || (isFocusCell && isInvertSelection));
				}
			};
			if(cell.ColumnInfo.Type == GridColumnInfoType.Column) {
				if(cell.RowInfo.FormatInfo.ApplyDrawFormat(e.Cache, cell.Column, cell.Bounds, cell.RowHandle, cell.ViewInfo, drawMethod, appearanceValue)) return;
			}
			drawMethod(e.Cache, appearanceValue);
			if(cell.ColumnInfo.Type == GridColumnInfoType.EmptyColumn) {
				if(!AllowDrawCellBackground) return;
				StyleFillRectangle(e.Cache, appearanceValue, appearanceValue.GetBackColor(), cell.Bounds);
			}
		}
		protected virtual void DrawCellEdit(GridViewDrawArgs e, BaseEditViewInfo editInfo, GridCellInfo cell, AppearanceObject appearance, bool isSelectedCell) {
			if(editInfo == null) return;
			editInfo.IsCellSelected = isSelectedCell;
			GraphicsClipState clipInfo = null;
			Rectangle maxClipBounds = e.Cache.ClipInfo.MaximumBounds;
			try {
				if(editInfo.RequireClipping) {
					Rectangle maxBounds = GetRowClipBounds(e, null, e.ViewInfo.UpdateFixedRange(cell.CellValueRect, cell.ColumnInfo), true);
					maxBounds.Intersect(maxClipBounds); 
					e.Cache.ClipInfo.MaximumBounds = maxBounds;
					clipInfo = e.Cache.ClipInfo.SaveAndSetClip(maxClipBounds);
				}
				ImageLoadInfo info = View.GetImageLoadInfo(cell);
				if(View.OptionsImageLoad.AsyncLoad && info != null && info.IsLoadingImage) {
					DrawLoadingImage(e, cell, info);
				}
				else View.GridControl.EditorHelper.DrawCellEdit(e, cell.Editor, editInfo, appearance, cell.CellValueRect.Location);
			}
			finally {
				if(clipInfo != null) {
					e.Cache.ClipInfo.MaximumBounds = maxClipBounds;
					e.Cache.ClipInfo.RestoreClipRelease(clipInfo);
				}
			}
		}
		protected virtual void DrawLoadingImage(GridViewDrawArgs e, GridCellInfo cell, ImageLoadInfo info) {
			if(info.LoadingImage == null) return;
			SelectActiveFrame(View.ViewInfo, info);
			Rectangle bounds = ImageAnimationHelper.CalcLoadingImageRectangle(cell.CellValueRect, info.LoadingImage.Size);
			e.Graphics.DrawImage(info.LoadingImage, bounds);
		}
		protected void SelectActiveFrame(GridViewInfo viewInfo, ImageLoadInfo itemInfo) {
			if(viewInfo == null || viewInfo.View == null || !viewInfo.View.OptionsImageLoad.AsyncLoad) return;
			BaseAnimationInfo info = XtraAnimator.Current.Get(viewInfo.View.ImageLoader as ISupportXtraAnimation, itemInfo.AnimationObject);
			if(info != null) itemInfo.LoadingImage.SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
		}
		protected void DrawRowLines(GridViewDrawArgs e, GridRowInfo ri) {
			DrawLines(e, ri.Lines);
		}
		protected override bool IsAllowDrawIndent(IndentInfo indent) { 
			GridCellInfo cell = indent.IndentOwner as GridCellInfo;
			if(cell != null && cell.MergedCell != null) {
				if(cell.MergedCell.Bounds.Bottom <= indent.Bounds.Bottom) return true;
				return false;
			}
			return true; 
		}
		protected virtual void DrawRowPreview(GridViewDrawArgs e, GridDataRowInfo ri) {
			if(ri.PreviewBounds.IsEmpty) return;
			e.ViewInfo.UpdateRowAppearancePreview(ri);
			RowPreviewCustomDrawEventArgs cs = new RowPreviewCustomDrawEventArgs(e.Cache, ElementsPainter.RowPreview, ri);
			cs.SetDefaultDraw(() => {
				ObjectPainter.DrawObject(e.Cache, ElementsPainter.RowPreview, ri);
			});
			View.RaiseCustomDrawRowPreview(cs);
			cs.DefaultDraw();
		}
		protected virtual void DrawMergedCells(GridViewDrawArgs e) {
			for(int n = 0; n < e.ViewInfo.CellMerger.MergedCells.Count; n++) {
				DrawMergedCell(e, e.ViewInfo.CellMerger.MergedCells[n]);
			}
		}
		protected virtual void DrawMergedCell(GridViewDrawArgs e, GridMergedCellInfo mergedCell) {
			if(!IsNeedDrawRect(e, mergedCell.Bounds)) return;
			if(mergedCell.Bounds.IsEmpty) return;
			Rectangle clipBounds = GetRowClipBounds(e, null, mergedCell.Bounds, true);
			if(clipBounds.Width < 1 || clipBounds.Height < 1) return;
			GraphicsClipState clipInfo = e.Cache.ClipInfo.SaveAndSetClip(clipBounds, true, true);
			try {
				DrawRegularRowCell(e, mergedCell);
			} finally {
				e.Cache.ClipInfo.RestoreClipRelease(clipInfo);
			}
		}
		protected virtual void DrawRows(GridViewDrawArgs e) {
			if(!IsNeedDrawRect(e, e.ViewInfo.ViewRects.Rows)) return;
			int rCount = e.ViewInfo.RowsInfo.Count;
			for(int n = 0; n < rCount; n++) {
				GridRowInfo ri = (GridRowInfo)e.ViewInfo.RowsInfo[n];
				DrawRow(e, ri);
			}
			DrawMergedCells(e);
			DrawEmptyArea(e);
		}
		protected virtual void DrawEmptyArea(GridViewDrawArgs e) {
			AppearanceObject appearance = e.ViewInfo.PaintAppearance.Empty;
			if(View.DataController.VisibleCount == 0) {
				Rectangle bounds = e.ViewInfo.ViewRects.Rows;
				if(e.ViewInfo.RowsInfo.Count > 0) {
					GridRowInfo row = e.ViewInfo.RowsInfo.Last();
					bounds.Height = bounds.Bottom - row.TotalBounds.Bottom;
					bounds.Y = row.TotalBounds.Bottom;
				}
				View.RaiseCustomDrawEmptyForeground(new CustomDrawEventArgs(e.Cache, bounds, appearance));
			}
		}
		protected virtual void DrawSizeGrip(GridViewDrawArgs e) {
			ScrollInfo si = View.ScrollInfo;
			if(!si.SizeGripBounds.IsEmpty) {
				Brush brush = e.Cache.GetSolidBrush(LookAndFeelHelper.GetSystemColor(View.LookAndFeel, SystemColors.Control));
				if(si.IsTouchMode) brush = e.ViewInfo.PaintAppearance.Empty.GetBackBrush(e.Cache);
				e.Cache.Paint.FillRectangle(e.Graphics, brush, si.SizeGripBounds);
			}
		}
		public virtual void DrawColumnDrag(GraphicsCache cache, GridViewInfo viewInfo, GridColumn column, Rectangle bounds, bool pressed, bool customization) {
			GridColumnInfoArgs ci = viewInfo.CreateColumnInfo(column);
			ci.HtmlContext = viewInfo.View;
			ci.CustomizationForm = customization;
			if(customization)
				ci.HeaderPosition = HeaderPositionKind.Center;
			else
				ci.HeaderPosition = HeaderPositionKind.Special;
			ci.Column = column;
			ci.Bounds = bounds;
			ci.Info.AllowEffects = false;
			viewInfo.CalcColumnInfoState(ci);
			ci.SetAppearance(viewInfo.PaintAppearance.HeaderPanel.Clone() as AppearanceObject);
			ElementsPainter.Column.CalcObjectBounds(ci);
			ci.State = pressed ? ObjectState.Pressed : ObjectState.Normal;
			ColumnHeaderCustomDrawEventArgs cs = new ColumnHeaderCustomDrawEventArgs(null, ElementsPainter.Column, ci);
			DrawObject(cache, ElementsPainter.Column, ci, new CustomEventCall(View.RaiseCustomDrawColumnHeader), cs);
		}
		public virtual Bitmap GetColumnDragBitmap(GridViewInfo viewInfo, GridColumn column, Size columnSize, bool pressed, bool customization) {
			if(columnSize.Width == 0) {
				columnSize.Width = column.VisibleWidth;
				if(columnSize.Width > 200) columnSize.Width = 200;
				if(columnSize.Width < 100) columnSize.Width = 100;
			}
			Rectangle bounds = new Rectangle(0, 0, columnSize.Width, viewInfo.ColumnRowHeight);
			Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
			Graphics g = Graphics.FromImage(bmp);
			GraphicsCache cache = new GraphicsCache(new DXPaintEventArgs(g, Rectangle.Empty), new DevExpress.Utils.Paint.XPaint());
			DrawColumnDrag(cache, viewInfo, column, bounds, pressed, customization);
			g.Dispose();
			return bmp;
		}
		protected virtual void CalcSizingPoints(ref Point start, ref Point end) {
			GridViewInfo viewInfo = View.ViewInfo as GridViewInfo;
			int startX, startY, endX, endY;
			startY = endX = endY = startX = -10000;
			if(View.State == GridState.ColumnSizing) {
				startX = fCurrentSizerPos;
				startY = viewInfo.ViewRects.ColumnPanel.Y;
				endX = fCurrentSizerPos;
				endY = viewInfo.ViewRects.Rows.Bottom;
				if(startX > viewInfo.ViewRects.ColumnPanel.Right) return;
				if(startX < viewInfo.ViewRects.ColumnPanel.Left + viewInfo.ViewRects.IndicatorWidth)
					return;
			}
			if(View.State == GridState.RowDetailSizing || View.State == GridState.RowSizing) {
				startY = fCurrentSizerPos;
				endY = fCurrentSizerPos;
				startX = viewInfo.ViewRects.Rows.Left;
				endX = viewInfo.ViewRects.Rows.Right;
				if(startY < viewInfo.ViewRects.Rows.Top) return;
				if(startY > viewInfo.ViewRects.Rows.Bottom) return;
			}
			if(startX == -10000) return;
			start = new Point(startX, startY);
			end = new Point(endX, endY);
		}
		public override void DrawSizerLine() {
			GridViewInfo viewInfo = View.ViewInfo as GridViewInfo;
			Point startPoint = BaseViewInfo.EmptyPoint, endPoint = BaseViewInfo.EmptyPoint;
			CalcSizingPoints(ref startPoint, ref endPoint);
			if(startPoint.X == BaseViewInfo.EmptyPoint.X) return;
			if(startPoint.X < viewInfo.ViewRects.Client.X || startPoint.X > viewInfo.ViewRects.Client.Right) return;
			SplitterLineHelper.Default.DrawReversibleLine(View.GridControl.Handle, startPoint, endPoint);
		}
		public override void StopSizing() {
			base.StopSizing();
			ReSizingObject = null;
		}
		public object ReSizingObject { get { return reSizingObject; } set { reSizingObject = value; } }
	}
	public class GridElementsPainter {
		ObjectPainter detailButton;
		ObjectPainter specialTopRowIndent;
		IndicatorObjectPainter columnIndicator;
		IndicatorObjectPainter indicator;
		GridFilterPanelPainter filterPanel; 
		GridGroupPanelPainter groupPanel; 
		HeaderObjectPainter column;
		ObjectPainter filterButton, smartFilterButton;
		ObjectPainter openCloseButton, viewCaption;
		GridGroupRowPainter groupRow;
		GridRowPreviewPainter rowPreview;
		ObjectPainter shapePainter;
		FooterCellPainter footerCell;
		FooterPanelPainter footerPanel;
		FooterCellPainter groupFooterCell;
		FooterPanelPainter groupFooterPanel;
		BaseView view;
		public GridElementsPainter(BaseView view) {
			this.view = view;
			this.detailButton = CreateDetailButtonPainter();
			this.indicator = CreateIndicatorPainter();
			this.columnIndicator = CreateColumnIndicatorPainter();
			this.filterPanel = CreateFilterPanelPainter();
			this.groupPanel = CreateGroupPanelPainter();
			this.rowPreview = CreateRowPreviewPainter();
			this.groupRow = CreateGroupRowPainter();
			this.openCloseButton = CreateOpenCloseButtonPainter();
			this.shapePainter = CreateColumnSortedShapePainter();
			this.column = CreateColumnPainter();
			this.column.UseInnerElementsForBestHeight = false;
			this.viewCaption = CreateViewCaptionPainter();
			this.footerCell = CreateFooterCellPainter();
			this.footerPanel = CreateFooterPanelPainter();
			this.groupFooterCell = CreateGroupFooterCellPainter();
			this.groupFooterPanel = CreateGroupFooterPanelPainter();
			this.specialTopRowIndent = CreateSpecialTopRowPainter();
			this.filterButton = CreateHeaderFilterButtonPainter();
			this.smartFilterButton = CreateHeaderSmartFilterButtonPainter();
		}
		public virtual bool ShouldInflateRowOnInvalidate() { return false; }
		public virtual object GetIndicatorImages(object imageCollection) { return imageCollection; }
		public virtual GridView View { get { return (GridView)view; } }
		public ObjectPainter DetailButton { get { return detailButton; } }
		public ObjectPainter ViewCaption { get { return viewCaption; } }
		public ObjectPainter SpecialTopRowIndent { get { return specialTopRowIndent; } }
		public IndicatorObjectPainter ColumnIndicator { get { return columnIndicator; } }
		public IndicatorObjectPainter Indicator { get { return indicator; } }
		public GridFilterPanelPainter FilterPanel { get { return filterPanel; } }
		public GridGroupPanelPainter GroupPanel { get { return groupPanel; } }
		public GridRowPreviewPainter RowPreview { get { return rowPreview; } }
		public GridGroupRowPainter GroupRow { get { return groupRow; } }
		public FooterCellPainter FooterCell { get { return footerCell; } }
		public FooterPanelPainter FooterPanel { get { return footerPanel; } }
		public FooterCellPainter GroupFooterCell { get { return groupFooterCell; } }
		public FooterPanelPainter GroupFooterPanel { get { return groupFooterPanel; } }
		public HeaderObjectPainter Column { get { return column; } }
		public ObjectPainter OpenCloseButton { get { return openCloseButton; } }
		public ObjectPainter FilterButton { get { return filterButton; } }
		public ObjectPainter SmartFilterButton { get { return smartFilterButton; } }
		public ObjectPainter SortedShape { get { return shapePainter; } }
		protected virtual ActiveLookAndFeelStyle ElementsStyle { get { return ActiveLookAndFeelStyle.Flat; } }
		public UserLookAndFeel ElementsLookAndFeel  { get { return View.ElementsLookAndFeel; } }
		protected virtual ObjectPainter CreateViewCaptionPainter() { return new GridViewCaptionPainter(View); }
		protected virtual ObjectPainter CreateDetailButtonPainter() { return new DetailButtonObjectPainter(); }
		protected virtual IndicatorObjectPainter CreateColumnIndicatorPainter() { return CreateIndicatorPainter(); }
		protected virtual IndicatorObjectPainter CreateIndicatorPainter() { return new IndicatorObjectPainter(new FlatButtonObjectPainter()); }
		protected virtual GridFilterPanelPainter CreateFilterPanelPainter() { return new GridFilterPanelPainter(EditorButtonHelper.GetPainter(BorderStyles.Default, ElementsLookAndFeel), CheckPainterHelper.GetPainter(ElementsLookAndFeel)); }
		protected virtual GridGroupPanelPainter CreateGroupPanelPainter() { return new GridGroupPanelPainter(); }
		protected virtual GridRowPreviewPainter CreateRowPreviewPainter() { return new GridRowPreviewPainter(this); }
		protected virtual GridGroupRowPainter CreateGroupRowPainter() { return new GridGroupRowPainter(this); }
		protected virtual FooterCellPainter CreateFooterCellPainter() { return LookAndFeelPainterHelper.GetPainter(ElementsStyle).FooterCell; }
		protected virtual FooterPanelPainter CreateFooterPanelPainter() { return LookAndFeelPainterHelper.GetPainter(ElementsStyle).FooterPanel; } 
		protected virtual FooterCellPainter CreateGroupFooterCellPainter() { return new GridGroupFooterCellPainter(new TextFlatBorderPainter()); }
		protected virtual FooterPanelPainter CreateGroupFooterPanelPainter() { return new GridGroupFooterPanelPainter(new EmptyButtonObjectPainter()); }
		protected virtual HeaderObjectPainter CreateColumnPainter() { return LookAndFeelPainterHelper.GetPainter(ElementsStyle).Header; }
		protected virtual ObjectPainter CreateColumnSortedShapePainter() { return LookAndFeelPainterHelper.GetPainter(ElementsStyle).SortedShape; }
		protected virtual ObjectPainter CreateOpenCloseButtonPainter() { return LookAndFeelPainterHelper.GetPainter(ElementsStyle).OpenCloseButton; }
		protected virtual ObjectPainter CreateHeaderSmartFilterButtonPainter() { return new GridSmartFlatFilterButtonPainter();	}
		protected virtual ObjectPainter CreateHeaderFilterButtonPainter() { return new GridFilterButtonPainter(EditorButtonHelper.GetPainter(BorderStyles.Flat)); } 
		protected virtual ObjectPainter CreateSpecialTopRowPainter() { return new GridSpecialTopRowIndentPainter(); }
	}
	public class GridWebElementsPainter : GridElementsPainter {
		public GridWebElementsPainter(BaseView view) : base(view) {
		}
		protected override HeaderObjectPainter CreateColumnPainter() { return new HeaderObjectPainter(new HeaderWebButtonObjectPainter()); }
		protected override ActiveLookAndFeelStyle ElementsStyle { get { return ActiveLookAndFeelStyle.Flat; } }
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { return new GridFilterButtonPainter(new EditorButtonPainter(new WebFlatButtonObjectPainter())); } 
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new IndicatorObjectPainter(new WebIndicatorButtonObjectPainter()); }
		protected override ObjectPainter CreateColumnSortedShapePainter() { return new FlatSortedShapeObjectPainter();; }
		protected override GridFilterPanelPainter CreateFilterPanelPainter() { return new GridFilterPanelPainter(new EditorButtonPainter(new WebFlatButtonObjectPainter()), CheckPainterHelper.GetPainter(ElementsLookAndFeel)); }
	}
	public class GridStyle3DElementsPainter : GridElementsPainter {
		public GridStyle3DElementsPainter(BaseView view) : base(view) {
		}
		protected override ActiveLookAndFeelStyle ElementsStyle { get { return ActiveLookAndFeelStyle.Style3D; } }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new Style3DIndicatorObjectPainter(); }
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { return new GridFilterButtonPainter(EditorButtonHelper.GetPainter(BorderStyles.Style3D)); } 
		protected override FooterCellPainter CreateGroupFooterCellPainter() { return new GridGroupFooterCellPainter(new Border3DSunkenPainter()); }
		protected override FooterPanelPainter CreateGroupFooterPanelPainter() { return new GridGroupFooterPanelPainter(new Style3DButtonObjectPainter()); }
	}
	public class GridUltraFlatElementsPainter : GridElementsPainter {
		public GridUltraFlatElementsPainter(BaseView view) : base(view) {
		}
		protected override ActiveLookAndFeelStyle ElementsStyle { get { return ActiveLookAndFeelStyle.UltraFlat; } }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new UltraFlatIndicatorObjectPainter(); }
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { return new GridFilterButtonPainter(EditorButtonHelper.GetPainter(BorderStyles.UltraFlat)); } 
		protected override FooterCellPainter CreateGroupFooterCellPainter() { return new GridGroupFooterCellPainter(new SimpleBorderPainter()); }
		protected override FooterPanelPainter CreateGroupFooterPanelPainter() { return new GridGroupFooterPanelPainter(new GridUltraFlatButtonPainter()); }
		protected override ObjectPainter CreateSpecialTopRowPainter() { return new GridSpecialTopRowIndentUltraFlatPainter(); }
	}
	public class GridWindowsXPElementsPainter : GridElementsPainter {
		public GridWindowsXPElementsPainter(BaseView view) : base(view) {
		}
		protected override ActiveLookAndFeelStyle ElementsStyle { get { return ActiveLookAndFeelStyle.WindowsXP; } }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new XPIndicatorObjectPainter(); }
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { return new GridFilterButtonPainter(new WindowsXPEditorButtonPainter()); } 
		protected override FooterCellPainter CreateGroupFooterCellPainter() { return new GridGroupFooterCellPainter(new TextFlatBorderPainter()); }
		protected override FooterPanelPainter CreateGroupFooterPanelPainter() { return new GridGroupFooterPanelPainter(new GridWindowsXPButtonPainter()); }
	}
	public class GridMixedXPElementsPainter : GridElementsPainter {
		public GridMixedXPElementsPainter(BaseView view) : base(view) { }
		protected override GridFilterPanelPainter CreateFilterPanelPainter() { return new GridFilterPanelPainter(new WindowsXPEditorButtonPainter(), new WindowsXPCheckObjectPainter()); }
	}
	public class GridOffice2003ElementsPainter : GridElementsPainter {
		public GridOffice2003ElementsPainter(BaseView view) : base(view) {
		}
		protected override ActiveLookAndFeelStyle ElementsStyle { get { return ActiveLookAndFeelStyle.Office2003; } }
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { return new Office2003GridFilterButtonPainter(); } 
		protected override ObjectPainter CreateHeaderSmartFilterButtonPainter() { return new GridSmartOffice2003FilterButtonPainter();	}
		protected override FooterCellPainter CreateGroupFooterCellPainter() { return new Office2003FooterCellPainter(); }
		protected override FooterPanelPainter CreateGroupFooterPanelPainter() { return new GridGroupFooterPanelPainter(new Office2003FooterPanelObjectPainter()); }
		protected override GridFilterPanelPainter CreateFilterPanelPainter() { return new Office2003GridFilterPanelPainter(); }
		protected override GridGroupPanelPainter CreateGroupPanelPainter() { return new Office2003GridGroupPanelPainter(); }
		protected override GridGroupRowPainter CreateGroupRowPainter() { return new Office2003GridGroupRowPainter(this); }
		protected override IndicatorObjectPainter CreateColumnIndicatorPainter() { return new Office2003IndicatorObjectPainter(); }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new Office2003IndicatorObjectPainter(); }
		protected override ObjectPainter CreateSpecialTopRowPainter() { return new GridTopNewItemRowIndentOffice2003Painter(); }
	}
}
