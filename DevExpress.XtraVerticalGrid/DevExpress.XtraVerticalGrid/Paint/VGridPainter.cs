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

namespace DevExpress.XtraVerticalGrid.Painters {
	using System;
	using System.Collections;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;
	using DevExpress.LookAndFeel;
	using DevExpress.XtraEditors.Drawing;
	using DevExpress.XtraEditors.ViewInfo;
	using DevExpress.XtraEditors.Controls;
	using DevExpress.XtraVerticalGrid.Events;
	using DevExpress.XtraVerticalGrid.Rows;
	using DevExpress.XtraVerticalGrid.ViewInfo;
	using DevExpress.Utils;
	using DevExpress.Utils.Win;
	using DevExpress.Utils.Paint;
	using DevExpress.Utils.Drawing;
	using DevExpress.XtraVerticalGrid.Data;
	using DevExpress.Utils.Text;
	public class VGridPainter {
		protected Rectangle paintClipRect;
		protected Clipping clip;
		protected VGridResourceCache RC;
		VGridDrawInfo drawInfo;
		PaintEventHelper eventHelper;
		VGridPaintStyleCollection paintStyles;
		public VGridPainter(PaintEventHelper eventHelper) {
			this.paintClipRect = Rectangle.Empty;
			this.clip = new DevExpress.Utils.Paint.Clipping();
			this.RC = null;
			this.eventHelper = eventHelper;
			this.drawInfo = null;
			this.paintStyles = CreatePaintStyles();
		}
		protected virtual VGridPaintStyleCollection CreatePaintStyles() {
			return new VGridPaintStyleCollection(this);
		}
		int drawLocker = 0;
		public void DoDraw(DXPaintEventArgs e, BaseViewInfo vi) {
			if(drawLocker != 0) return;
			drawLocker++;
			try {
				DoBeforeDraw(e, vi);
				DoDrawCore(vi);
			}
			finally {
				DoAfterDraw(vi);
				drawLocker--;
			}
		}
		protected virtual void DoBeforeDraw(DXPaintEventArgs e, BaseViewInfo vi) {
			paintClipRect = e.ClipRectangle;
			RC = vi.RC;
			this.drawInfo = new VGridDrawInfo(new GraphicsCache(e), vi, vi.ViewRects.Client);
			XPaint.Graphics.BeginPaint(Graphics);
		}
		protected virtual void DoAfterDraw(BaseViewInfo vi) {
			if(!vi.Grid.Enabled && vi.Grid.UseDisabledStatePainter)
				BackgroundPaintHelper.PaintDisabledControl(PaintStyles.LookAndFeel, DrawInfo.Cache, vi.ViewRects.Window);
			XPaint.Graphics.EndPaint(Graphics);
			paintClipRect = Rectangle.Empty;
			DrawInfo.Dispose();
			this.drawInfo = null;
		}
		public virtual bool NeedsRedraw(Rectangle r) {
			if(r.Width == 0 || r.Height == 0) return false;
			Rectangle rUpdate = Rectangle.Intersect(r, paintClipRect);
			return !(rUpdate.Width == 0 || rUpdate.Height == 0);
		}
		protected virtual void FillEmptyRects(ViewRects viewRects, AppearanceObject emptyStyle, Rectangle cl) {
			if(viewRects.EmptyRects.Count == 0) return;
			Region rgn = Graphics.Clip;
			for(int i = 0; i < viewRects.EmptyRects.Count; i++)
				Graphics.SetClip((Rectangle)viewRects.EmptyRects[i], CombineMode.Union);
			FillRectangle(emptyStyle, cl);
			Graphics.Clip = rgn;
			for(int i = 0; i < viewRects.EmptyRects.Count; i++)
				Graphics.SetClip((Rectangle)viewRects.EmptyRects[i], CombineMode.Exclude);
		}
		protected virtual void DrawTreeButtonCore(CustomDrawTreeButtonEventArgs e) {
			if(!NeedsRedraw(e.Bounds)) return;
			if(e.TreeButtonType == TreeButtonType.TreeViewButton) DrawTreeViewButton(e);
			else if(e.TreeButtonType == TreeButtonType.ExplorerBarButton) DrawExplorerButton(e);
		}
		internal void DrawRowHeader(PaintEventArgs e, BaseRowHeaderInfo rh, VGridResourceCache rc) {
			GridSavePaintArgs pa = CreateSavePaintArgs();
			SavePaintArgs(e, pa);
			VGridResourceCache originalRC = RC;
			RC = rc;
			XPaint.Graphics.BeginPaint(Graphics);
			try {
				DrawRowHeaderCore(rh);
			}
			finally {
				RC = originalRC;
				XPaint.Graphics.EndPaint(Graphics);
				RestorePaintArgs(pa);
			}
		}
		internal VGridResourceCache ResourceCache {
			get { return RC; }
			set { RC = value; }			
		}
		protected virtual GridSavePaintArgs CreateSavePaintArgs() {
			return new GridSavePaintArgs();
		}
		protected virtual void SavePaintArgs(PaintEventArgs e, GridSavePaintArgs pa) {
			pa.ClipRect = paintClipRect;
			paintClipRect = e.ClipRectangle;
			GridSavePaintArgs ga = (GridSavePaintArgs)pa;
			ga.DrawInfo = DrawInfo;
			this.drawInfo = new VGridDrawInfo(new GraphicsCache(CreateDXPaintEventArgs(e)), null, e.ClipRectangle);
		}
		protected virtual void RestorePaintArgs(GridSavePaintArgs pa) {
			paintClipRect = pa.ClipRect;
			DrawInfo.Dispose();
			this.drawInfo = pa.DrawInfo;
		}
		protected virtual void DoDrawCore(BaseViewInfo vi) {
			FillEmptyRects(vi.ViewRects, vi.PaintAppearance.Empty, vi.ViewRects.Client);
			if(vi.ShouldDrawRows()) {
				DrawRows(vi);
				DrawLines(vi.LinesInfo, vi.ViewRects.Client);
			}
			DrawStyleFeatures(vi);
			if(NeedsRedraw(vi.ViewRects.ScrollSquare)) {
				DrawInfo.Graphics.FillRectangle(DrawInfo.Cache.GetSolidBrush(PaintStyle.GetSizeGripColor(RC.ScrollBarColor)), vi.ViewRects.ScrollSquare);
			}
			if(IsEditingRowOverlapedByFixedBottom(vi)) {
				DrawRow(vi.FocusedRowViewInfo);
				DrawLines(vi.NotFixedFocusedLinesInfo, vi.ViewRects.Client);
			}
			DrawBorder(vi);
		}
		protected virtual internal DXPaintEventArgs CreateDXPaintEventArgs(PaintEventArgs e) {
			return new DXPaintEventArgs(e);
		}
		protected virtual bool IsEditingRowOverlapedByFixedBottom(BaseViewInfo gridViewInfo) {
			return gridViewInfo.Grid.ActiveEditor != null && gridViewInfo.Grid.FocusedRow != null && gridViewInfo.Grid.FocusedRow.Fixed == FixedStyle.None &&
				gridViewInfo.ViewRects.FixedBottom.Height > 0 && gridViewInfo.GetFixedBottomWithBounds().Top < gridViewInfo.FocusedRowViewInfo.RowRect.Bottom;
		}
		protected virtual void FillRectangle(AppearanceObject appearance, Rectangle r) {
			if(NeedsRedraw(r))
				appearance.FillRectangle(DrawInfo.Cache, r);
		}
		protected internal virtual void DrawLines(Lines LinesInfo, Rectangle client) {
			Graphics.SetClip(client);
			for(int i = 0; i < LinesInfo.Count; i++)
				LinesInfo[i].Draw(DrawInfo.Cache);
			Graphics.ResetClip();
		}
		private void DrawRows(BaseViewInfo vi) {
			foreach(BaseRowViewInfo ri in vi.RowsViewInfo)
				DrawRow(ri);
		}
		private void DrawRow(BaseRowViewInfo ri) {
			if(!NeedsRedraw(Rectangle.Inflate(ri.RowRect, RC.VertLineWidth, RC.HorzLineWidth))) return;
			DrawRowHeaderCore(ri.HeaderInfo);
			int sepIndex = 0;
			MultiEditorRowViewInfo meri = ri as MultiEditorRowViewInfo;
			GraphicsClipState clipState = null;
			clipState = drawInfo.Cache.ClipInfo.SaveAndSetClip(ri.ValuesRect, true, true);
			for(int i = 0; i < ri.ValuesInfo.Count; i++)
				DrawRowValueCell(ri.ValuesInfo[i], DrawInfo.ViewInfo, meri, ref sepIndex);
			drawInfo.Cache.ClipInfo.RestoreClip(clipState);
			PaintStyle.DrawHeader_ValuesSeparator(ri, RC.VertLineWidth);
		}
		protected virtual void DrawRowHeaderCore(BaseRowHeaderInfo rh) {
			if(!NeedsRedraw(Rectangle.Inflate(rh.HeaderRect, RC.VertLineWidth, RC.HorzLineWidth))) return;
			DrawRowIndent(rh);
			MultiEditorRowHeaderInfo merh = rh as MultiEditorRowHeaderInfo;
			for(int i = 0; i < rh.CaptionsInfo.Count; i++)
				DrawRowHeaderCell((RowCaptionInfo)rh.CaptionsInfo[i], i, merh);
			DrawTreeButton(rh);
			DrawLines(rh.LinesInfo, paintClipRect);
		}
		private void DrawRowIndent(BaseRowHeaderInfo rh) {
			CustomDrawRowHeaderIndentEventArgs e = new CustomDrawRowHeaderIndentEventArgs(DrawInfo.Cache, rh.IndentBounds, rh.Style);
			e.Init(rh);
			EventHelper.DrawRowHeaderIndent(e);
			if(!e.Handled)
				DrawRowHeaderIndentCore(e);
		}
		private void DrawTreeButton(BaseRowHeaderInfo rh) {
			if(!NeedsRedraw(rh.ExpandButtonRect)) return;
			CustomDrawTreeButtonEventArgs e = new CustomDrawTreeButtonEventArgs(DrawInfo.Cache, rh.ExpandButtonRect, rh.ExpandButtonStyle, rh.Row, rh.TreeButtonType);
			EventHelper.DrawTreeButton(e);
			if(!e.Handled)
				DrawTreeButtonCore(e);
		}
		private void DrawFocusRect(CustomDrawRowHeaderCellEventArgs e) {
			if(!NeedsRedraw(e.FocusRect)) return;
			XPaint.Graphics.DrawFocusRectangle(Graphics, e.FocusRect, e.Appearance.ForeColor, e.Appearance.BackColor);
		}
		private void DrawSeparator(CustomDrawSeparatorEventArgs e) {
			EventHelper.DrawSeparator(e);
			if(!e.Handled)
				DrawSeparatorCore(e);
		}
		internal void DrawRowHeaderCell(RowCaptionInfo rc, int cellIndex, MultiEditorRowHeaderInfo merh) {
			CustomDrawRowHeaderCellEventArgs e = new CustomDrawRowHeaderCellEventArgs(DrawInfo.Cache, rc.CaptionRect, rc.Style);
			e.Init(rc);
			EventHelper.DrawRowHeaderCell(e);
			if(!e.Handled)
				DrawRowHeaderCellCore(e);
			if(merh != null && cellIndex < merh.SeparatorRects.Count) {
				CustomDrawSeparatorEventArgs e2 = new CustomDrawSeparatorEventArgs(DrawInfo.Cache, (Rectangle)merh.SeparatorRects[cellIndex], e.Appearance.Clone() as AppearanceObject, (MultiEditorRow)merh.Row);
				e2.Init(merh.SeparatorInfo, cellIndex, true);
				DrawSeparator(e2);
			}
		}
		internal void DrawRowValueCell(RowValueInfo rv, BaseViewInfo vi, MultiEditorRowViewInfo meri, ref int sepIndex) {
			CustomDrawRowValueCellEventArgs e = new CustomDrawRowValueCellEventArgs(DrawInfo.Cache, rv.Bounds, rv.Style, rv.Properties, rv.RecordIndex, 
				rv.RowCellIndex, rv.EditorViewInfo.EditValue, rv.EditorViewInfo.DisplayText);
			EventHelper.DrawRowValueCell(e);
			if(!e.Handled) {
				EventHelper.DrawnCell = rv;
				DrawRowValueCellCore(e, vi.GetPainter(rv.Item), rv.EditorViewInfo, vi);
				EventHelper.DrawnCell = null;
			}
			if(meri != null && rv.RowCellIndex != -1 && rv.RowCellIndex - meri.FirstRowCellIndex != meri.HeaderInfo.CaptionsInfo.Count - 1 && meri.SeparatorRects.Count > sepIndex) {
				AppearanceObject sepStyle = meri.GetMultiEditorValuesSeparatorStyle(rv, e.Appearance, vi.PaintAppearance);
				CustomDrawSeparatorEventArgs e2 = new CustomDrawSeparatorEventArgs(DrawInfo.Cache, (Rectangle)meri.SeparatorRects[sepIndex], sepStyle.Clone() as AppearanceObject, (MultiEditorRow)meri.Row);
				e2.Init(meri.HeaderInfo.SeparatorInfo, rv.RowCellIndex, false);
				DrawSeparator(e2);
				sepIndex++;
			}
		}
		protected virtual void DrawBorder(BaseViewInfo vi) {
			Rectangle bounds = vi.ViewRects.Window;
			if(!NeedsRedraw(bounds)) return;
			Graphics.Clip = DrawInfo.NativeClip;
			PaintStyle.BorderPainter.DrawObject(vi.GetBorderObjectArgs(bounds, DrawInfo.Cache));
		}
		protected virtual void DrawStyleFeatures(BaseViewInfo vi) { PaintStyle.DrawStyleFeatures(vi); }
		protected virtual void DrawSeparatorCore(CustomDrawSeparatorEventArgs e) {
			if(!NeedsRedraw(e.Bounds)) return;
			if(e.SeparatorKind == SeparatorKind.String) {
				e.Appearance.FillRectangle(e.Cache, e.Bounds);
				e.Appearance.DrawString(e.Cache, e.SeparatorString, e.Bounds);
			}
			else
				PaintStyle.DrawVertLineSeparator(e);
		}
		protected virtual void DrawRowHeaderCellCore(CustomDrawRowHeaderCellEventArgs e) {
			if(!NeedsRedraw(e.Bounds))
				return;
			PaintStyle.DrawRowHeaderCellBackground(e);
			if(!CheckDrawMatchedText(e)) {
				DrawText(e);
			}
			if(!e.AllowGlyphSkinning)
				DrawImage(e.Cache, e.ImageIndex, e.ImageRect);
			else
				DrawImageSkinned(e.Cache, e.ImageIndex, e.ImageRect, e.Appearance.GetForeColor());
			DrawFocusRect(e);
		}
		protected virtual void DrawText(CustomDrawRowHeaderCellEventArgs e) {
			if(e.AllowHtmlText)
				DrawHtmlText(e);
			else
				DrawSimpleText(e);
		}
		protected virtual void DrawSimpleText(CustomDrawRowHeaderCellEventArgs e) {
			e.Appearance.DrawString(DrawInfo.Cache, e.Caption, e.CaptionRect);
		}
		protected virtual void DrawHtmlText(CustomDrawRowHeaderCellEventArgs e) {
			StringInfo si = StringPainter.Default.Calculate(e.Graphics, e.Appearance, e.Caption, e.CaptionRect);
			StringPainter.Default.DrawString(e.Cache, si);
		}
		protected virtual bool CheckDrawMatchedText(CustomDrawRowHeaderCellEventArgs e) {
			VGridControlBase grid = e.Row.Grid;
			if (!grid.HighlightHeaders || !grid.FindPanelOwner.IsFindFilterActive || !grid.OptionsFind.HighlightFindResults)
				return false;
			int containsIndex, length;
			string text = e.Caption;
			string matchedText = e.Row.Grid.FindPanelOwner.GetFindMatchedText(PGridDataModeHelper.CaptionColumnName, text);
			if (!TextEditPainter.IsTextMatch(text, matchedText, true, false, out containsIndex, out length))
				return false;
			AppearanceObject appearance = (AppearanceObject)e.Appearance.Clone();
			AppearanceDefault highlight = LookAndFeelHelper.GetHighlightSearchAppearance(grid.LookAndFeel, false);
			e.Cache.Paint.DrawMultiColorString(e.Cache,
				e.CaptionRect,
				text,
				matchedText,
				appearance,
				appearance.GetStringFormat(TextOptions.DefaultOptionsNoWrap),
				highlight.ForeColor,
				highlight.BackColor,
				false,
				containsIndex);
			return true;
		}
		protected virtual void DrawRowHeaderIndentCore(CustomDrawRowHeaderIndentEventArgs e) {
			foreach(IndentInfo ii in e.CategoryIndents)
				FillRectangle(ii.Style, ii.Bounds);
			for(int i = 0; i < e.RowIndents.Count; i++)
				FillRectangle(e.RowIndents[i].Style, e.RowIndents[i].Bounds);
		}
		protected virtual void DrawTreeViewButton(CustomDrawTreeButtonEventArgs e) {
			OpenCloseButtonInfoArgs args = new OpenCloseButtonInfoArgs(DrawInfo.Cache, e.Bounds, e.Expanded, e.Appearance, ObjectState.Normal);
			args.RightToLeft = e.Row.Grid.ViewInfo.IsRightToLeft;
			PaintStyle.OpenCloseButtonPainter.DrawObject(args);
		}
		protected virtual void DrawExplorerButton(CustomDrawTreeButtonEventArgs e) {
			PaintStyle.DrawExplorerButton(e);
		}
		protected virtual void DrawRowValueCellCore(CustomDrawRowValueCellEventArgs e, BaseEditPainter pb, BaseEditViewInfo bvi, BaseViewInfo vi) {
			bvi.PaintAppearance = e.Appearance;
			bvi.SetDisplayText(e.CellText);
			if (vi.Grid.FindPanelOwner.IsAllowHighlightFind(e.Properties)) {
				bvi.UseHighlightSearchAppearance = true;
				bvi.MatchedStringUseContains = true;
				bvi.MatchedString = vi.Grid.FindPanelOwner.GetFindMatchedText(e.Properties.FieldName, e.CellText);
			}
			if(!bvi.FillBackground)
				bvi.PaintAppearance.FillRectangle(DrawInfo.Cache, EventHelper.DrawnCell.Bounds);
			if(vi.GetAllowHtmlRowValueText(e.Row, e.Enabled)) {
				StringInfo si = StringPainter.Default.Calculate(e.Cache.Graphics, e.Appearance, e.CellText, bvi.Bounds);
				StringPainter.Default.DrawString(e.Cache, si);
			}
			else {
				pb.Draw(new ControlGraphicsInfoArgs(bvi, DrawInfo.Cache, EventHelper.DrawnCell.EditorViewInfo.Bounds));
			}
			if(EventHelper.DrawnCell.DrawFocusFrame)
				XPaint.Graphics.DrawFocusRectangle(Graphics, EventHelper.DrawnCell.Bounds, vi.PaintAppearance.FocusedRecord.GetForeColor(), vi.PaintAppearance.FocusedRecord.GetBackColor());
		}
		protected virtual void DrawImage(GraphicsCache cache, int imageIndex, Rectangle imageRect) {
			if(imageIndex != -1 && NeedsRedraw(imageRect)) {
				IntPtr rgn = clip.MySaveClip(Graphics);
				ImageCollection.DrawImageListImage(cache, EventHelper.ImageList, imageIndex, imageRect);
				clip.MyRestoreClip(Graphics, rgn);
			}
		}
		protected virtual void DrawImageSkinned(GraphicsCache cache, int imageIndex, Rectangle imageRect, Color foreColor) {
			if(imageIndex != -1 && NeedsRedraw(imageRect)) {
				IntPtr rgn = clip.MySaveClip(Graphics);
				var attributes = ImageColorizer.GetColoredAttributes(foreColor);
				ImageCollection.DrawImageListImage(cache, EventHelper.ImageList, imageIndex, imageRect, attributes);
				clip.MyRestoreClip(Graphics, rgn);
			}
		}
		protected VGridDrawInfo DrawInfo { get { return drawInfo; } }
		protected internal Graphics Graphics { get { return DrawInfo.Cache.Graphics; } }
		protected VGridPaintStyleCollection PaintStyles { get { return paintStyles; } }
		protected internal PaintEventHelper EventHelper { get { return eventHelper; } }
		public VGridPaintStyle PaintStyle { get { return PaintStyles[EventHelper.LookAndFeel.ActiveStyle]; } }
	}	
}
