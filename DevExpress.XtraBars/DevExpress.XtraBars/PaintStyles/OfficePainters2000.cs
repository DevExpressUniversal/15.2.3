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
using System.Drawing;
using DevExpress.XtraBars.Controls;
using System.Windows.Forms;
using System.Collections;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.Objects;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars.ViewInfo;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraBars.Painters {
	public class PrimitivesPainterOffice2000 : PrimitivesPainter { 
		public PrimitivesPainterOffice2000(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override ObjectPainter CreateDefaultLinkBorderPainter() {
			return new TextFlatBorderPainter();
		}
		protected override MenuHeaderPainter CreateDefaultMenuHeaderPainter() {
			return new TextFlatMenuHeaderPainter();
		}
		public virtual int DrawLinkButtonAdorments(PrimitivesPainterOffice2000 PPainter, BarLinkPaintArgs e, BarLinkState drawState) {
			BarButtonItemLink link = e.LinkInfo.Link as BarButtonItemLink;
			Rectangle ar = e.LinkInfo.Rects[BarLinkParts.OpenArrow], r;
			if(ar.IsEmpty) return -1;
			if(drawState == BarLinkState.Normal) {
				return 0; 
			}
			r = e.LinkInfo.SelectRect;
			r.Width = ar.Left - r.Left;
			Color clr = PaintHelper.GetBrushColor(e.LinkInfo.GetBackBrush(e.Cache));
			PPainter.DrawFlatFrame(e, r, clr, drawState == BarLinkState.Pressed);
			r = ar;
			PPainter.DrawFlatFrame(e, ar, clr, link.Opened);
			r.Width = DrawParameters.Constants.BarButtonRealArrowWidth;
			r.Y += (ar.Height - DrawParameters.Constants.BarButtonRealArrowWidth) / 2;
			r.X += (ar.Width - DrawParameters.Constants.BarButtonRealArrowWidth) / 2;
			PPainter.DrawArrow(e, PPainter.CalcLinkForeBrush(e, drawState), r.Location, r.Width, MarkArrowDirection.Down);
			return 1;
		}
		public override void DrawButtonLinkArrowAdormentsInMenu(BarLinkPaintArgs e, BarLinkState drawState, Brush brush) {
			Rectangle ar = e.LinkInfo.Rects[BarLinkParts.OpenArrow], r;
			r = ar;
			r.Width = DrawParameters.Constants.SubMenuRealArrowWidth;
			r.Y += (ar.Height - DrawParameters.Constants.SubMenuRealArrowWidth) / 2;
			r.X += (ar.Width - DrawParameters.Constants.SubMenuRealArrowWidth) / 2;
			DrawArrow(e, CalcLinkForeBrush(e, drawState), r.Location, r.Width, MarkArrowDirection.Right);
			if(drawState == BarLinkState.Normal) {
				ar.Width = 1;
				PaintHelper.FillRectangle(e.Graphics, DrawParameters.Colors.Brushes(BarColor.SubMenuSeparatorColor), ar);
			}
		}
		protected override void DrawLinkBackgroundInMenuCore(BarLinkPaintArgs e) {
			Brush backBrush = GetLinkInMenuBackBrush(e);
			Rectangle r = e.LinkInfo.SelectRect;
			if(!e.LinkInfo.IsRecentLink) {
				PaintHelper.FillRectangle(e.Graphics, backBrush, r);
			}
			Rectangle glRect = CalcInMenuGlyphRect(e);
			Color clr = PaintHelper.GetBrushColor(e.LinkInfo.GetBackBrush(e.Cache));
			if(!glRect.IsEmpty && (e.LinkInfo.LinkState & BarLinkState.Checked) != 0) {
				DrawFlatFrame(e, glRect, clr, true);
				glRect.Inflate(-1, -1);
				if(e.LinkInfo.LinkState == BarLinkState.Checked && !e.LinkInfo.DrawDisabled)
					PaintHelper.FillRectangle(e.Graphics, CheckedBrush, glRect);
			}
		}
		protected virtual Brush GetLinkInMenuBackBrush(BarLinkPaintArgs e) {
			Brush backBrush;
			if(!e.LinkInfo.IsRecentLink)
				backBrush = e.LinkInfo.AppearanceMenu.SideStripNonRecent.GetBackBrush(e.Cache, e.LinkInfo.Bounds);
			else {
				backBrush = e.LinkInfo.GetBackBrush(e.Cache, e.LinkInfo.Bounds);
			}
			return backBrush;
		}
		public override Rectangle CalcInMenuGlyphRect(BarLinkPaintArgs e) {
			Rectangle r = e.LinkInfo.SelectRect;
			r.Inflate(-1, 0);
			if(e.LinkInfo.IsDrawAnyGlyph) {
				Rectangle glRect = r;
				glRect.Width = e.LinkInfo.GlyphRect.Right - glRect.X;
				return glRect;
			}
			return Rectangle.Empty;
		}
		public override void DrawLinkHighlightedBackgroundInMenu(BarLinkPaintArgs e, BarLinkEmptyBorder border, Brush backBrush, BarLinkState drawState) {
			Rectangle r = e.LinkInfo.SelectRect;
			PaintHelper.FillRectangle(e.Graphics, GetLinkInMenuBackBrush(e), r);
			r.Inflate(-1, 0);
			if(e.LinkInfo.IsDrawAnyGlyph) {
				Rectangle glRect = CalcInMenuGlyphRect(e);
				Color clr = PaintHelper.GetBrushColor(e.LinkInfo.GetBackBrush(e.Cache));
				DrawFlatFrame(e, glRect, clr, (e.LinkInfo.LinkState & BarLinkState.Checked) != 0);
				r.X = glRect.Right + 1;
				r.Width = (e.LinkInfo.SelectRect.Right - 1) - r.X;
				glRect.Inflate(-1, -1);
			}
			PaintHelper.FillRectangle(e.Graphics, SystemBrushes.Highlight, r);
		}
		public override Brush CalcLinkForeBrush(BarLinkPaintArgs e, BarLinkState state) {
			Brush itemBrush = e.LinkInfo.GetForeBrush(e.Cache, state);
			if(e.LinkInfo.DrawDisabled)
				itemBrush = e.LinkInfo.DrawParameters.Colors.Brushes(BarColor.LinkDisabledForeColor);
			if(state == BarLinkState.Highlighted && !e.LinkInfo.DrawDisabled && e.LinkInfo.IsLinkInMenu) 
				itemBrush = SystemBrushes.HighlightText;
			if(e.LinkInfo.Link is BarQMenuAddRemoveButtonsItemLink) itemBrush = e.LinkInfo.DrawParameters.Colors.Brushes(BarColor.LinkForeColor);
			return itemBrush;
		}
		protected override void DrawLinkDescriptionCore(BarLinkPaintArgs e, BarLinkState state, Rectangle bounds, string text, Brush foreBrush) {
			if(state == BarLinkState.Highlighted)
				foreBrush = CalcLinkForeBrush(e, state);
			base.DrawLinkDescriptionCore(e, state, bounds, text, foreBrush);
		}
		public override void DrawLinkCaption(BarLinkPaintArgs e, Brush foreBrush, BarLinkState state) {
			if(e.LinkInfo.DrawDisabled && state == BarLinkState.Normal) state = BarLinkState.Disabled;
			Rectangle r = e.LinkInfo.CaptionRect;
			switch(state) {
				case BarLinkState.Disabled:
					Brush brush = Brushes.White;
					r.X ++;	r.Y ++;
					PaintHelper.DrawString(e.Cache, e.LinkInfo.DisplayCaption, e.LinkInfo.Font, 
						brush, r, e.LinkInfo.LinkCaptionStringFormat);
					r.X --; r.Y --;
					break;
			}
			PaintHelper.DrawString(e.Cache, e.LinkInfo.DisplayCaption, e.LinkInfo.Font, 
				foreBrush, r, e.LinkInfo.LinkCaptionStringFormat);
		}
		public override void DrawLinkImage(BarLinkPaintArgs e, Rectangle r, ImageAttributes attr, Image image, BarLinkState state) {
			BarLinkState rstate = e.LinkInfo.CalcRealPaintState();
			if(image == null) image = e.LinkInfo.GetLinkImage(state);
			if(image == null) return;
			r.Offset(-1, -1);
			switch(state) {
				case BarLinkState.Disabled:
					if(attr == null) break;
					r.Offset(1, 1);
					Size linkImageSize = CalcLinkImageSize(e.LinkInfo, image);
					ImageLayoutMode mode = e.LinkInfo.DrawParameters.ScaleImages ? ImageLayoutMode.ZoomInside : ImageLayoutMode.Squeeze;
					Rectangle rect = ImageLayoutHelper.GetImageBounds(r, linkImageSize, mode);
					PaintHelper.DrawImageCore(e.Graphics, image, rect.X, rect.Y, rect.Width, rect.Height, new Rectangle(Point.Empty, image.Size), PaintHelper.WhiteAttributes);
					rect.Offset(-1, -1);
					PaintHelper.DrawImageCore(e.Graphics, image, rect.X, rect.Y, rect.Width, rect.Height, new Rectangle(Point.Empty, image.Size), PaintHelper.DisabledAttributes);
					return;
			}
			if(!e.LinkInfo.IsLinkInMenu) {
				if((e.LinkInfo.LinkState & BarLinkState.Checked) != 0) r.Offset(1, 1);
				if((e.LinkInfo.LinkState & BarLinkState.Pressed) != 0) r.Offset(1, 1);
			}
			base.DrawLinkImage(e, r, attr, image, state);
		}
		Brush checkedBrush = null;
		protected virtual Brush CheckedBrush { 
			get { 
				if(checkedBrush == null) {
					checkedBrush = new HatchBrush(HatchStyle.Percent50, SystemColors.Control, SystemColors.ControlLightLight);
				}
				return checkedBrush;
			}
		}
		protected virtual Brush GetLinkHighlightedBackBrush(BarLinkPaintArgs e) {
			if(e.LinkInfo.LinkState == BarLinkState.Checked && !e.LinkInfo.DrawDisabled) {
				return CheckedBrush;
			}
			return e.LinkInfo.GetBackBrush(e.Cache, e.LinkInfo.Bounds);
		}
		public override void DrawLinkHighLightedBackground(BarLinkPaintArgs e, Rectangle r, BarLinkEmptyBorder border, BarLinkState realState, Brush backBrush) {
			Color clr = PaintHelper.GetBrushColor(e.LinkInfo.GetBackBrush(e.Cache));
			bool down = (e.LinkInfo.LinkState & (BarLinkState.Pressed | BarLinkState.Checked)) != 0;
			if(e.LinkInfo.Link is BarCustomContainerItemLink) {
				BarCustomContainerItemLink ci = e.LinkInfo.Link as BarCustomContainerItemLink;
				if(ci.Opened) down = true;
			}
			DrawFlatFrame(e, r, clr, down);
			r.Inflate(-1, -1);
			PaintHelper.FillRectangle(e.Graphics, GetLinkHighlightedBackBrush(e), r);
		}
		public override void DrawFlatFrame(GraphicsInfoArgs e, Rectangle rect, Color backColor, bool downed) {
			Rectangle r = rect;
			r.Height = 1; r.Width --;
			Brush lightBrush = e.Cache.GetSolidBrush(PaintHelper.LightLight(backColor)), darkBrush = e.Cache.GetSolidBrush(PaintHelper.Dark(backColor));
			Brush brush = (downed ? darkBrush : lightBrush);
			PaintHelper.FillRectangle(e.Graphics, brush, r);
			r.Width = 1; r.Height = rect.Height - 1;
			PaintHelper.FillRectangle(e.Graphics, brush, r);
			brush = (!downed ? darkBrush : lightBrush);
			r.X = rect.Right - 1;
			PaintHelper.FillRectangle(e.Graphics, brush, r);
			r.Height = 1; r.X = rect.X;r.Width = rect.Width;r.Y = rect.Bottom - 1;
			PaintHelper.FillRectangle(e.Graphics, brush, r);
		}
		public override void DrawSubmenuSeparator(GraphicsInfoArgs e, Rectangle rect, BarLinkViewInfo li, object sourceInfo) {
			CustomSubMenuBarControlViewInfo menuInfo = sourceInfo as CustomSubMenuBarControlViewInfo;
			if(menuInfo == null) return;
			Rectangle r = rect;
			Brush backBrush = li.GetBackBrush(e.Cache);
			Brush lightBrush = e.Cache.GetSolidBrush(PaintHelper.LightLight(PaintHelper.GetBrushColor(backBrush))),
				darkBrush = e.Cache.GetSolidBrush(PaintHelper.Dark(PaintHelper.GetBrushColor(backBrush)));
			if(!li.IsRecentLink) {
				if(menuInfo != null) {
					BarLinkViewInfo prevLink = menuInfo.GetLinkViewInfo(li.Link, LinkViewInfoRange.Prev);
					if(prevLink != null && !prevLink.IsRecentLink) {
						darkBrush = backBrush;
						backBrush = menuInfo.AppearanceMenu.SideStripNonRecent.GetBackBrush(e.Cache, li.Bounds);
					}
				}
			}
			if(!li.IsRecentLink && PaintHelper.GetBrushColor(backBrush).A == 255) PaintHelper.FillRectangle(e.Graphics, backBrush, r);
			r.Y += DrawParameters.Constants.SubMenuSeparatorHeight / 2 - DrawParameters.Constants.BarSeparatorLineThickness / 2;
			r.Inflate(-8, 0);
			r.Height = 1;
			PaintHelper.FillRectangle(e.Graphics, darkBrush, r);
			r.Y ++;
			PaintHelper.FillRectangle(e.Graphics, lightBrush, r);
		}
		public override void DrawSeparator(GraphicsInfoArgs e, RectInfo info, BarLinkViewInfo li, object sourceInfo) {
			Rectangle r = info.Rect;
			int th = li.DrawParameters.Constants.BarSeparatorLineThickness;
			bool vert = info.Type == RectInfoType.VertSeparator;
			int width = (vert ? info.Rect.Width : info.Rect.Height);
			int indt = (width - th) / 2;
			Brush backBrush = li.GetBackBrush(e.Cache);
			Brush lightBrush = e.Cache.GetSolidBrush(PaintHelper.LightLight(PaintHelper.GetBrushColor(backBrush))),
				darkBrush = e.Cache.GetSolidBrush(PaintHelper.Dark(PaintHelper.GetBrushColor(backBrush)));
			if(vert) {
				r.Width = 1;
				r.X += indt;
				r.Inflate(0, -2);
			} else {
				r.Height = 1;
				r.Y += indt;
				r.Inflate(-2, 0);
			}
			PaintHelper.FillRectangle(e.Graphics, darkBrush, r);
			if(vert) r.X ++;
			else r.Y++;
			PaintHelper.FillRectangle(e.Graphics, lightBrush, r);
		}
	}
	public class BarOffice2000Painter : BarPainter {
		public new PrimitivesPainterOffice2000 PPainter { get { return base.PPainter as PrimitivesPainterOffice2000; } }
		public BarOffice2000Painter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		const int DragBorderRealWidth = 3;
		protected override void DrawDragBorder(GraphicsInfoArgs e, BarControlViewInfo info) {
			if(!info.DragBorder.IsEmpty) {
				Color clr = PaintHelper.GetBrushColor(info.Appearance.Normal.GetBackBrush(e.Cache));
				Rectangle r = info.DragBorder;
				if(info.IsVertical) {
					r.Y ++;
					r.Inflate(-2, -2);
					r.Height = DragBorderRealWidth;
				} else {
					r.X ++;
					r.Inflate(-2, -2);
					r.Width = DragBorderRealWidth;
				}
				PPainter.DrawFlatFrame(e, r, clr, false);
			}
		}
		public override void DrawBackgroundSurfaceCore(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			Rectangle r = info.Bounds;
			if(info.Bar == null || !info.Bar.IsStatusBar) {
				Color clr = info.Appearance.Normal.BackColor;
				PPainter.DrawFlatFrame(e, r, clr, false);
				r.Inflate(-1, -1);
			}
			info.Appearance.Normal.DrawBackground(e.Graphics, e.Cache, r);
		}
		public override void DrawBackground(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			DrawBackgroundSurface(e, info, sourceInfo);
			if(!info.DragBorder.IsEmpty) {
				DrawDragBorder(e, info);
			}
			if(!info.SizeGrip.IsEmpty) {
				DrawSizeGrip(e, info);
			}
		}
	}
	public class ControlFormOffice2000Painter : ControlFormPainter {
		public new PrimitivesPainterOffice2000 PPainter { get { return base.PPainter as PrimitivesPainterOffice2000; } }
		public ControlFormOffice2000Painter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public override void DrawBorder(GraphicsInfoArgs e, ControlFormViewInfo vi) {
			Rectangle r = vi.WindowRect;
			Color clr = DrawParameters.Colors.MenuAppearance.Menu.BackColor;
			PPainter.DrawFlatFrame(e, r, PaintHelper.Dark(SystemColors.Control), false);
			r.Inflate(-1, -1);
			PPainter.DrawFlatFrame(e, r, SystemColors.Control, false);
			r.Inflate(-1, -1);
			PaintHelper.DrawRectangle(e.Graphics, SystemPens.Control, r);
		}
	}
	public class BarBaseButtonOffice2000LinkPainter : BarCheckButtonLinkPainter { 
		protected override bool AllowFillCheckBackground { get { return false; } }
		public BarBaseButtonOffice2000LinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
	}
	public class BarButtonOffice2000LinkPainter : BarButtonLinkPainter {
		public new PrimitivesPainterOffice2000 PPainter { get { return base.PPainter as PrimitivesPainterOffice2000; } }
		public BarButtonOffice2000LinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override bool AllowFillCheckBackground { get { return false; } }
		protected override void DrawButtonLinkHighlightedBackground(BarLinkPaintArgs e, BarLinkState drawState) {
			DrawLinkButtonAdorments(e, drawState);
		}
		protected internal override void DrawLinkButtonAdorments(BarLinkPaintArgs e, BarLinkState drawState) {
			if(PPainter.DrawLinkButtonAdorments(PPainter, e, drawState) == 0)
				base.DrawLinkButtonAdorments(e, drawState);
		}
	}
	public class BarLargeButtonOffice2000LinkPainter : BarLargeButtonLinkPainter {
		public new PrimitivesPainterOffice2000 PPainter { get { return base.PPainter as PrimitivesPainterOffice2000; } }
		public BarLargeButtonOffice2000LinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override void DrawLinkGlyphInMenu(BarLinkPaintArgs e, BarLinkState state) {
			if((e.LinkInfo.LinkState & BarLinkState.Checked) == 0) {
				base.DrawLinkGlyphInMenu(e, state);
				return;
			}
			Rectangle r = e.LinkInfo.GlyphRect;
			if(!e.LinkInfo.IsDrawPart(BarLinkParts.Glyph) || !PPainter.IsExistsLinkImage(e.LinkInfo)) 
				PPainter.DrawLinkCheckMark(e, e.LinkInfo.GlyphRect, state);
			else
				DrawLinkNormalGlyph(e, false);
		}
		protected override void DrawButtonLinkHighlightedBackground(BarLinkPaintArgs e, BarLinkState drawState) {
			DrawLinkButtonAdorments(e, drawState);
		}
		protected internal override void DrawLinkButtonAdorments(BarLinkPaintArgs e, BarLinkState drawState) {
			if(PPainter.DrawLinkButtonAdorments(PPainter, e, drawState) == 0)
				base.DrawLinkButtonAdorments(e, drawState);
		}
	}
	public class BarQMenuCustomizationOffice2000LinkPainter : BarQMenuCustomizationLinkPainter {
		public new PrimitivesPainterOffice2000 PPainter { get { return base.PPainter as PrimitivesPainterOffice2000; } }
		public BarQMenuCustomizationOffice2000LinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public override void DrawLink(BarLinkPaintArgs e) {
			BarQMenuCustomizationLinkViewInfo li = e.LinkInfo as BarQMenuCustomizationLinkViewInfo;
			Rectangle r = li.GlyphRect;
			int indent = DrawParameters.Constants.SubMenuQGlyphGlyphIndent;
			li.Rects.SetWidth(BarLinkParts.Glyph, li.GlyphWidth - indent);
			BarLinkViewInfo prevViewInfo = li.InnerViewInfo;
			try {
				if(li.InnerViewInfo != null) {
					Rectangle gr, sr, selRect;
					gr = sr = li.InnerViewInfo.Rects[BarLinkParts.Glyph];
					selRect = li.InnerViewInfo.Rects[BarLinkParts.Select];
					gr.X = li.Rects[BarLinkParts.Glyph].Right + indent;
					gr.Width = sr.Right - gr.X;
					li.InnerViewInfo.Rects[BarLinkParts.Glyph] = gr;
					li.InnerViewInfo.Rects[BarLinkParts.Select] = new Rectangle(gr.X - indent, selRect.Y, (selRect.Right - gr.X) + indent, selRect.Height);
					li.InnerViewInfo.SetBackBrush(li.GetBackBrush(e.Cache, li.InnerViewInfo.Bounds));
					li.InnerViewInfo.LinkState = li.LinkState & (~BarLinkState.Checked);
					li.InnerViewInfo.AllowDrawBackground = true;
					li.InnerViewInfo.AllowDrawDisabled = false;
					li.InnerViewInfo.Painter.DrawLink(new BarLinkPaintArgs(e.Cache, li.InnerViewInfo, e.SourceInfo));
					li.InnerViewInfo.Rects[BarLinkParts.Glyph] = sr;
					li.InnerViewInfo.Rects[BarLinkParts.Select] = selRect;
					Region reg = new Region(li.SelectRect);
					reg.Exclude(PPainter.CalcInMenuGlyphRect(e));
					e.Graphics.ExcludeClip(reg);
					reg.Dispose();
				}
				li.InnerViewInfo = null;
				base.DrawLink(e);
			}
			finally {
				li.InnerViewInfo = prevViewInfo;
				li.GlyphRect = r;
			}
		}
		protected override void DrawQLinkFrame(BarLinkPaintArgs e) {
		}
		protected virtual void ButtonDrawLinkGlyphInMenu(BarLinkPaintArgs e, BarLinkState state) {
			if((e.LinkInfo.LinkState & BarLinkState.Checked) == 0) {
				base.DrawLinkGlyphInMenu(e, state);
				return;
			}
			Rectangle r = e.LinkInfo.GlyphRect;
			if(!e.LinkInfo.IsDrawPart(BarLinkParts.Glyph) || !PPainter.IsExistsLinkImage(e.LinkInfo)) 
				PPainter.DrawLinkCheckMark(e, e.LinkInfo.GlyphRect, state);
			else
				DrawLinkNormalGlyph(e, false);
		}
		protected override void DrawLinkGlyphInMenu(BarLinkPaintArgs e, BarLinkState state) {
			BarQMenuCustomizationLinkViewInfo li = e.LinkInfo as BarQMenuCustomizationLinkViewInfo;
			try {
				if(li.InnerViewInfo != null) 
					li.Rects.AddWidth(BarLinkParts.Glyph, -(li.InnerViewInfo.GlyphSize.Width + 2));
				ButtonDrawLinkGlyphInMenu(e, state);
			}
			finally {
				if(li.InnerViewInfo != null) 
					li.Rects.AddWidth(BarLinkParts.Glyph, +(li.InnerViewInfo.GlyphSize.Width + 2));
			}
		}
	}
	public class BarEditOffice2000LinkPainter : BarEditLinkPainter {
		public BarEditOffice2000LinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override void DrawEdit(BarLinkPaintArgs e) {
			BarEditLinkViewInfo vInfo = e.LinkInfo as BarEditLinkViewInfo;
			if(vInfo.EditViewInfo == null) return;
			vInfo.UpdateEditState();
			GetManager(e.LinkInfo).EditorHelper.DrawCellEdit(e, vInfo.EditViewInfo.Item, vInfo.EditViewInfo);
			Rectangle r = e.LinkInfo.SelectRect;
			DrawLinkSelection(e, ref r);
		}
	}
	public class BarRecentExpanderOffice2000LinkPainter : BarRecentExpanderLinkPainter {
		public new PrimitivesPainterOffice2000 PPainter { get { return base.PPainter as PrimitivesPainterOffice2000; } }
		public BarRecentExpanderOffice2000LinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected internal override void DrawLinkHighlightedBackgroundInMenu(BarLinkPaintArgs e, BarLinkState drawState) {
			if(!e.LinkInfo.AllowDrawBackground) return;
			Rectangle r = e.LinkInfo.SelectRect;
			PPainter.DrawFlatFrame(e, r, PaintHelper.GetBrushColor(e.LinkInfo.GetBackBrush(e.Cache)), false);
			r.Inflate(-1, -1);
			PaintHelper.FillRectangle(e.Graphics, e.LinkInfo.AppearanceMenu.SideStripNonRecent.GetBackBrush(e.Cache, r), r);
		}
	}
	public class BarQMenuAddRemoveButtonsOffice2000LinkPainter : BarQMenuAddRemoveButtonsLinkPainter {
		public BarQMenuAddRemoveButtonsOffice2000LinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override Brush CalcLinkHighlightedBrush(BarLinkPaintArgs e, BarLinkState state, BarLinkParts part) {
			return DrawParameters.GetBackBrush(BarAppearance.Bar, e.Cache);
		}
	}
	public class FloatingBarControlFormOffice2000Painter : FloatingBarControlFormPainter {
		public new PrimitivesPainterOffice2000 PPainter { get { return base.PPainter as PrimitivesPainterOffice2000; } }
		public FloatingBarControlFormOffice2000Painter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public override void DrawBorder(GraphicsInfoArgs e, ControlFormViewInfo vi) {
			Rectangle r = vi.WindowRect;
			Color clr = DrawParameters.Colors.MenuAppearance.Menu.BackColor;
			PPainter.DrawFlatFrame(e, r, PaintHelper.Dark(SystemColors.Control), false);
			r.Inflate(-1, -1);
			PPainter.DrawFlatFrame(e, r, SystemColors.Control, false);
			r.Inflate(-1, -1);
			PaintHelper.DrawRectangle(e.Graphics, SystemPens.Control, r);
		}
	}
	class TextFlatMenuHeaderPainter : MenuHeaderPainter {
		public override void DrawObject(BarLinkPaintArgs e) {
			Graphics g = e.Graphics;
			g.FillRectangle(Brushes.Aquamarine, e.LinkInfo.Bounds);
		}
		public override int CalcElementHeight(Graphics g, BarHeaderLinkViewInfo linkInfo, Size captionSize, int res) {
			return res;
		}
	}
}
