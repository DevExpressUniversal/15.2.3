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
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Styles;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Win;
using DevExpress.XtraBars.InternalItems;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars.Helpers.Docking;
using System.Drawing.Imaging;
namespace DevExpress.XtraBars.Painters {
	public class PrimitivesPainterWindowsXP : PrimitivesPainterOffice2000 { 
		public PrimitivesPainterWindowsXP(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override ObjectPainter CreateDefaultLinkBorderPainter() {
			return new LinkWindowsXPBorderPainter(this);
		}
		public override void DrawLinkImage(BarLinkPaintArgs e, Rectangle r, ImageAttributes attr, Image image, BarLinkState state) {
			BarLinkState rstate = e.LinkInfo.CalcRealPaintState();
			if(image == null) image = e.LinkInfo.GetLinkImage(state);
			if(image == null) return;
			r.Offset(1, 1);
			switch(state) {
				case BarLinkState.Disabled:
					if(attr == null) break;
					r.Offset(-1, -1);
					Size linkImageSize = CalcLinkImageSize(e.LinkInfo, image);
					ImageLayoutMode mode = e.LinkInfo.DrawParameters.ScaleImages ? ImageLayoutMode.ZoomInside : ImageLayoutMode.Squeeze;
					Rectangle rect = ImageLayoutHelper.GetImageBounds(r, linkImageSize, mode);
					PaintHelper.DrawImageCore(e.Graphics, image, rect.X, rect.Y, rect.Width, rect.Height, new Rectangle(Point.Empty, image.Size), PaintHelper.DisabledAttributes);
					return;
			}
			base.DrawLinkImage(e, r, attr, image, state);
		}
		public override void DrawLinkCaption(BarLinkPaintArgs e, Brush foreBrush, BarLinkState state) {
			if(e.LinkInfo.DrawDisabled && state == BarLinkState.Normal) state = BarLinkState.Disabled;
			Rectangle r = e.LinkInfo.CaptionRect;
			PaintHelper.DrawString(e.Cache, e.LinkInfo.DisplayCaption, e.LinkInfo.Font, 
				foreBrush, r, e.LinkInfo.LinkCaptionStringFormat);
		}
		public override int DrawLinkButtonAdorments(PrimitivesPainterOffice2000 PPainter, BarLinkPaintArgs e, BarLinkState drawState) {
			BarButtonItemLink link = e.LinkInfo.Link as BarButtonItemLink;
			Rectangle ar = e.LinkInfo.Rects[BarLinkParts.OpenArrow], r;
			if(ar.IsEmpty) return -1;
			r = CalcBarLinkButtonRectangle(e);
			r.X = ar.Left;
			r.Width = ar.Width;
			object state = CalcBarLinkButtonState(e, true);
			NativeControlAdvPaintArgs args = CreateArgs(e.Graphics, "toolbar", e.CalcRectangle(r), XP_TOOLBAR.SPLITBUTTONDROPDOWN, state);
			Painter.Draw(args);
			return 1;
		}
		protected virtual object CalcBarLinkButtonType(BarLinkPaintArgs e) {
			bool openArrow = !e.LinkInfo.Rects[BarLinkParts.OpenArrow].IsEmpty && (e.LinkInfo.DrawParts & BarLinkParts.OpenArrow) != 0;
			object btn = PrimitivesPainterWindowsXP.XP_TOOLBAR.BUTTON;
			if(openArrow) {
				btn = PrimitivesPainterWindowsXP.XP_TOOLBAR.DROPDOWNBUTTON;
				if(e.LinkInfo.Link is BarButtonItemLink)
					btn = PrimitivesPainterWindowsXP.XP_TOOLBAR.SPLITBUTTON;
			}
			return btn;
		}
		protected virtual object CalcBarLinkButtonState(BarLinkPaintArgs e, bool calcForArrow) {
			object ms = XPState_TOOLBAR.NORMAL;
			bool down = (e.LinkInfo.LinkState & (BarLinkState.Pressed | BarLinkState.Checked)) != 0;
			if(e.LinkInfo.LinkState != BarLinkState.Normal) 
				ms = PrimitivesPainterWindowsXP.XPState_TOOLBAR.HOT;
			BarCustomContainerItemLink csLink = e.LinkInfo.Link as BarCustomContainerItemLink;
			BarButtonItemLink btnLink = e.LinkInfo.Link as BarButtonItemLink;
			if(csLink != null) {
				if(csLink.Opened) down = true;
			}
			if(btnLink != null && (e.LinkInfo.DrawParts & BarLinkParts.OpenArrow) != 0) {
				if(calcForArrow) {
					if(btnLink.Opened) down = true; else down = false;
				} else {
				}
			}
			if(down) ms = PrimitivesPainterWindowsXP.XPState_TOOLBAR.PRESSED;
			if(e.LinkInfo.LinkState == BarLinkState.Checked) ms = PrimitivesPainterWindowsXP.XPState_TOOLBAR.CHECKED;
			if(e.LinkInfo.DrawDisabled) ms = PrimitivesPainterWindowsXP.XPState_TOOLBAR.DISABLED;
			return ms;
		}
		protected virtual Rectangle CalcBarLinkButtonRectangle(BarLinkPaintArgs e) {
			Rectangle r = e.LinkInfo.Bounds;
			return r;
		}
		protected virtual void DrawLinkCheckedFrameInMenu(BarLinkPaintArgs e) {
			Rectangle glRect = CalcInMenuGlyphRect(e);
			if(!glRect.IsEmpty && (e.LinkInfo.LinkState & BarLinkState.Checked) != 0) {
				if(e.LinkInfo.IsDrawPart(BarLinkParts.Glyph) && IsExistsLinkImage(e.LinkInfo)) 
					PaintHelper.DrawRectangle(e.Graphics, DrawParameters.Colors.Pens(BarColor.LinkForeColor), glRect);
			}
		}
		protected override void DrawLinkBackgroundInMenuCore(BarLinkPaintArgs e) {
			Brush backBrush = GetLinkInMenuBackBrush(e);
			Rectangle r = e.LinkInfo.SelectRect;
			if(!e.LinkInfo.IsRecentLink)
				PaintHelper.FillRectangle(e.Graphics, backBrush, r);
			DrawLinkCheckedFrameInMenu(e);
		}
		public override void DrawLinkHighlightedBackgroundInMenu(BarLinkPaintArgs e, BarLinkEmptyBorder border, Brush backBrush, BarLinkState drawState) {
			Rectangle r = e.LinkInfo.SelectRect;
			Rectangle r1 = r;
			Brush brush = e.LinkInfo.GetBackBrush(e.Cache);
			r1.Width = 1;PaintHelper.FillRectangle(e.Graphics, brush, r1);
			r1.X = r.Right - 1;PaintHelper.FillRectangle(e.Graphics, brush, r1);
			r.Inflate(-1, 0);
			PaintHelper.FillRectangle(e.Graphics, SystemBrushes.Highlight, r);
			DrawLinkCheckedFrameInMenu(e);
		}
		protected override Pen GetLinkCheckPen(BarLinkPaintArgs e, BarLinkState state) {
			return e.Cache.GetPen(PaintHelper.GetBrushColor(CalcLinkForeBrush(e, state)));
		}
		public override Brush CalcLinkForeBrush(BarLinkPaintArgs e, BarLinkState state) {
			if(IsMenuOnMainMenu(e)) {
				if((state & (BarLinkState.Pressed | BarLinkState.Highlighted)) != 0) return SystemBrushes.HighlightText;
			}
			return base.CalcLinkForeBrush(e, state);
		}
		protected virtual bool IsMenuOnMainMenu(BarLinkPaintArgs e) {
			if(e.LinkInfo.Link is BarCustomContainerItemLink) {
				BarCustomContainerItemLink ci = e.LinkInfo.Link as BarCustomContainerItemLink;
				if(e.LinkInfo.Link.Bar != null && e.LinkInfo.Link.Bar.IsMainMenu) {
					return !(e.LinkInfo.Link is BarQBarCustomizationItemLink);
				}
			}
			return false;
		}
		public override void DrawLinkHighLightedBackground(BarLinkPaintArgs e, Rectangle r, BarLinkEmptyBorder border, BarLinkState realState, Brush backBrush) {
			bool down = (e.LinkInfo.LinkState & (BarLinkState.Pressed | BarLinkState.Checked)) != 0;
			r = CalcBarLinkButtonRectangle(e);
			if(e.LinkInfo.Link is BarCustomContainerItemLink) {
				BarCustomContainerItemLink ci = e.LinkInfo.Link as BarCustomContainerItemLink;
				if(IsMenuOnMainMenu(e)) {
					PaintHelper.FillRectangle(e.Graphics, DrawParameters.Colors.Brushes(BarXPColor.MainMenuContainerLinkBackColor), r);
					return;
				}
				if(ci.Opened) down = true;
			}
			object ms = PrimitivesPainterWindowsXP.XPState_TOOLBAR.HOT;
			if(down) ms = PrimitivesPainterWindowsXP.XPState_TOOLBAR.PRESSED;
			if(e.LinkInfo.LinkState == BarLinkState.Checked) ms = PrimitivesPainterWindowsXP.XPState_TOOLBAR.CHECKED;
			DrawLinkBarButton(e, ms);
		}
		public override void DrawLinkNormalBackground(BarLinkPaintArgs e, Brush brush, Rectangle r) {
			object ms = PrimitivesPainterWindowsXP.XPState_TOOLBAR.NORMAL;
			if(e.LinkInfo.DrawDisabled) ms = PrimitivesPainterWindowsXP.XPState_TOOLBAR.DISABLED;
			DrawLinkBarButton(e, ms);
		}
		public virtual void DrawLinkBarButton(BarLinkPaintArgs e, object state) {
			Rectangle r = CalcBarLinkButtonRectangle(e);
			NativeControlAdvPaintArgs args = CreateArgs(e.Graphics, "toolbar", e.CalcRectangle(r), CalcBarLinkButtonType(e), state);
			Painter.Draw(args);
		}
		public static NativeControlAdvPaintArgs CreateArgsCore(Graphics g, string name, object part, object state) {
			NativeControlAdvPaintArgs args = new NativeControlAdvPaintArgs(g, Rectangle.Empty, ButtonPredefines.OK,
				ButtonStates.None, null, true, name, (int)part, (int)state, false);
			return args;
		}
		public virtual NativeControlAdvPaintArgs CreateArgs(Graphics g, string name, object part, object state) {
			return CreateArgsCore(g, name, part, state);
		}
		public virtual NativeControlAdvPaintArgs CreateArgs(Graphics g, string name, Rectangle bounds, object part, object state) {
			NativeControlAdvPaintArgs args = CreateArgs(g, name, part, state);
			args.Bounds = bounds;
			return args;
		}
		public override void DrawSubmenuSeparator(GraphicsInfoArgs e, Rectangle rect, BarLinkViewInfo li, object sourceInfo) {
			CustomSubMenuBarControlViewInfo menuInfo = sourceInfo as CustomSubMenuBarControlViewInfo;
			if(menuInfo == null) return;
			Rectangle r = rect;
			Brush backBrush = li.GetBackBrush(e.Cache, r);
			Brush sBrush = menuInfo.DrawParameters.Colors.Brushes(BarColor.SubMenuSeparatorColor);
			if(!li.IsRecentLink) {
				if(menuInfo != null) {
					BarLinkViewInfo prevLink = menuInfo.GetLinkViewInfo(li.Link, LinkViewInfoRange.Prev);
					if(prevLink != null && !prevLink.IsRecentLink) {
						backBrush = menuInfo.AppearanceMenu.SideStripNonRecent.GetBackBrush(e.Cache, r);
					}
				}
			}
			if(!li.IsRecentLink && PaintHelper.GetBrushColor(backBrush).A == 255) PaintHelper.FillRectangle(e.Graphics, backBrush, r);
			r.Y += DrawParameters.Constants.SubMenuSeparatorHeight / 2 - 1;
			r.Inflate(-8, 0);
			r.Height = 1;
			PaintHelper.FillRectangle(e.Graphics, sBrush, r);
		}
		public override void DrawSeparator(GraphicsInfoArgs e, RectInfo info, BarLinkViewInfo li, object sourceInfo) {
			Rectangle r = info.Rect;
			bool vert = info.Type == RectInfoType.VertSeparator;
			XP_TOOLBAR part = !vert ? XP_TOOLBAR.SEPARATORVERT : XP_TOOLBAR.SEPARATOR;
			if(vert) {
				r.Width = DrawParameters.Constants.BarSeparatorWidth;
			} else {
				r.Height = DrawParameters.Constants.BarSeparatorWidth;
			}
			NativeControlAdvPaintArgs args = CreateArgs(e.Graphics, "toolbar", e.CalcRectangle(r), part, 1);
			Painter.Draw(args);
		}
		public enum XP_WINDOW { BASE, CAPTION, SMALLCAPTION, MINCAPTION,SMALLMINCAPTION,MAXCAPTION, SMALLMAXCAPTION, FRAMELEFT, FRAMERIGHT, FRAMEBOTTOM, SMALLFRAMELEFT, 
			SMALLFRAMERIGHT, SMALLFRAMEBOTTOM, SYSBUTTON, MDISYSBUTTON, MINBUTTON, MDIMINBUTTON, MAXBUTTON, CLOSEBUTTON, 
			SMALLCLOSEBUTTON, MDICLOSEBUTTON, RESTOREBUTTON, MDIRESTOREBUTTON, HELPBUTTON, MDIHELPBUTTON, 
			HORZSCROLL, HORZTHUMB, VERTSCROLL, VERTTHUMB};
		public enum XP_REBAR { BASE, GRIPPER, GRIPPERVERT, BAND, CHEVRON}
		public enum XPState_CHEVRON { BASE, NORMAL, HOT, PRESSED}
		public enum XP_TOOLBAR {BASE, BUTTON, DROPDOWNBUTTON, SPLITBUTTON, SPLITBUTTONDROPDOWN, SEPARATOR, SEPARATORVERT}
		public enum XPState_TOOLBAR { BASE, NORMAL, HOT, PRESSED, DISABLED, CHECKED, HOTCHECKED}
		public enum XP_STATUS { BASE, PANE, GRIPPERPANE, GRIPPER }
		public enum XPState_Button { BASE, NORMAL, HOT, PUSHED, DISABLED };
	}
	public class BarWindowsXPPainter : BarPainter {
		public new PrimitivesPainterWindowsXP PPainter { get { return base.PPainter as PrimitivesPainterWindowsXP; } }
		public BarWindowsXPPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override void DrawDragBorder(GraphicsInfoArgs e, BarControlViewInfo info) {
			if(!info.DragBorder.IsEmpty) {
				BarWindowsXPConstants cs = DrawParameters.Constants as BarWindowsXPConstants;
				Rectangle r = info.DragBorder;
				int part = (int)PrimitivesPainterWindowsXP.XP_REBAR.GRIPPER;
				if(info.IsVertical) {
					part = (int)PrimitivesPainterWindowsXP.XP_REBAR.GRIPPERVERT;
					r.Y ++;
					r.Inflate(-1, -2);
					r.Height = cs.DragBorderRealWidth;
				} else {
					r.X ++;
					r.Inflate(-2, -1);
					r.Width = cs.DragBorderRealWidth;
				}
				NativeControlAdvPaintArgs pArgs = PPainter.CreateArgs(e.Graphics, "rebar", e.CalcRectangle(r), part, 1);
				Painter.Draw(pArgs);
			}
		}
		protected virtual void DrawStatusBarBackground(GraphicsInfoArgs e, BarControlViewInfo info) {
			Rectangle r = info.Bounds;
			NativeControlAdvPaintArgs pArgs = PPainter.CreateArgs(e.Graphics, "status", e.CalcRectangle(r), PrimitivesPainterWindowsXP.XP_STATUS.BASE, 1);
			Painter.Draw(pArgs);
		}
		public override void DrawBackgroundSurfaceCore(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			if(info.Bar != null && info.Bar.IsStatusBar)
				DrawStatusBarBackground(e, info);
		}
		public override void DrawBackground(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			DrawBackgroundSurface(e, info, sourceInfo);
			Rectangle r = info.Bounds;
			if(!info.DragBorder.IsEmpty) {
				DrawDragBorder(e, info);
			}
			if(!info.SizeGrip.IsEmpty)
				DrawSizeGrip(e, info);
			if(info.Bar != null && info.Bar.DockControl != null) DrawDockedBorder(e, info);
		}
		protected virtual void DrawDockedBorder(GraphicsInfoArgs e, BarControlViewInfo info) {
			if(info.Bar.OptionsBar.UseWholeRow) return;
			Rectangle r = info.Bounds;
			r.Width = DrawParameters.Constants.BarSeparatorWidth;
			r.Inflate(0, -1);
			DockRow row = info.Bar.DockControl.Rows.RowByDockable(info.Bar);
			if(row != null) {
			}
		}
	}
	public class QuickCustomizationBarWindowsXPPainter : BarWindowsXPPainter {
		public QuickCustomizationBarWindowsXPPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public override void DrawBackgroundSurfaceCore(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			Rectangle r = e.CalcRectangle(info.Bounds);
			NativeControlAdvPaintArgs pArgs = PPainter.CreateArgs(e.Graphics, "rebar", r, 0, 0);
			Painter.Draw(pArgs);
		}
		public override void DrawBackground(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			DrawBackgroundSurface(e, info, sourceInfo);
		}
	}
	public class FloatingWindowsXPBarPainter : FloatingBarPainter { 
		public new PrimitivesPainterWindowsXP PPainter { get { return base.PPainter as PrimitivesPainterWindowsXP; } }
		public FloatingWindowsXPBarPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public override void DrawBackgroundSurfaceCore(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			Rectangle r = e.CalcRectangle(info.Bounds);
			NativeControlAdvPaintArgs pArgs = PPainter.CreateArgs(e.Graphics, "rebar", r, 0, 0);
			Painter.Draw(pArgs);
		}
		public override void DrawBackground(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			DrawBackgroundSurface(e, info, sourceInfo);
		}
	}
	public class ControlFormWindowsXPPainter : ControlFormOffice2000Painter {
		public new PrimitivesPainterWindowsXP PPainter { get { return base.PPainter as PrimitivesPainterWindowsXP; } }
		public ControlFormWindowsXPPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public override void DrawBorder(GraphicsInfoArgs e, ControlFormViewInfo vi) {
			Rectangle r = vi.WindowRect;
			PaintHelper.DrawRectangle(e.Graphics, SystemPens.ControlDark, r);
			r.Inflate(-1, -1);
			Color color = DrawParameters.Colors.MenuAppearance.Menu.BackColor;
			color = Color.FromArgb(color.R, color.G, color.B);
			PaintHelper.DrawRectangle(e.Graphics, e.Cache.GetPen(color), r);
		}
	}
	public class DockControlWindowsXPPainter : CustomPainter {
		public new PrimitivesPainterWindowsXP PPainter { get { return base.PPainter as PrimitivesPainterWindowsXP; } }
		public DockControlWindowsXPPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public override void Draw(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
			DockControlViewInfo vi = (DockControlViewInfo)info;
			Rectangle r = e.CalcRectangle(vi.Bounds);
			NativeControlAdvPaintArgs pArgs = PPainter.CreateArgs(e.Graphics, "rebar", r, 0, 0);
			Painter.Draw(pArgs);
			bool reverse = vi.BarControl.DockStyle == BarDockStyle.Right || vi.BarControl.DockStyle == BarDockStyle.Bottom;
			int count = vi.BarControl.Rows.Count;
			for(int n = (reverse ? count - 1 : 0); (reverse ? n > 0 : n < count - 1); n += (reverse ? -1: 1)) {
				if(vi.BarControl.IsVertical) 
					DrawVerticalRowSeparator(e, vi.BarControl.Rows[n]);
				else 
					DrawRowSeparator(e, vi.BarControl.Rows[n]);
			}
		}
		public virtual void DrawRowSeparator(GraphicsInfoArgs e, DockRow row) {
			Rectangle r = row.RowRect;
			int delta = 1;
			r.Y = r.Bottom; r.Height = 1;
			e.Graphics.FillRectangle(DrawParameters.Colors.Brushes(BarXPColor.ToolbarEdgeShadow), r);
			r.Y += delta;
			e.Graphics.FillRectangle(DrawParameters.Colors.Brushes(BarXPColor.ToolbarEdgeHighlight), r);
		}
		public virtual void DrawVerticalRowSeparator(GraphicsInfoArgs e, DockRow row) {
			Rectangle r = row.RowRect;
			r.X = r.Right; r.Width = 1;
			e.Graphics.FillRectangle(DrawParameters.Colors.Brushes(BarXPColor.ToolbarEdgeShadow), r); r.X ++;
			e.Graphics.FillRectangle(DrawParameters.Colors.Brushes(BarXPColor.ToolbarEdgeHighlight), r);
		}
	}
	public class BarBaseButtonWindowsXPLinkPainter : BarCheckButtonLinkPainter {
		public BarBaseButtonWindowsXPLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override bool AllowFillCheckBackground { get { return false; } }
	}
	public class BarButtonWindowsXPLinkPainter : BarButtonLinkPainter {
		public new PrimitivesPainterWindowsXP PPainter { get { return base.PPainter as PrimitivesPainterWindowsXP; } }
		public BarButtonWindowsXPLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override bool AllowFillCheckBackground { get { return false; } }
		protected override void DrawButtonLinkHighlightedBackground(BarLinkPaintArgs e, BarLinkState drawState) {
			base.DrawButtonLinkHighlightedBackground(e, drawState);
			DrawLinkButtonAdorments(e, drawState);
		}
		protected internal override void DrawLinkButtonAdorments(BarLinkPaintArgs e, BarLinkState drawState) {
			if(PPainter.DrawLinkButtonAdorments(PPainter, e, drawState) == 0)
				base.DrawLinkButtonAdorments(e, drawState);
		}
	}
	public class BarLargeButtonWindowsXPLinkPainter : BarLargeButtonLinkPainter {
		public new PrimitivesPainterWindowsXP PPainter { get { return base.PPainter as PrimitivesPainterWindowsXP; } }
		public BarLargeButtonWindowsXPLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
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
			base.DrawButtonLinkHighlightedBackground(e, drawState);
			DrawLinkButtonAdorments(e, drawState);
		}
		protected internal override void DrawLinkButtonAdorments(BarLinkPaintArgs e, BarLinkState drawState) {
			if(PPainter.DrawLinkButtonAdorments(PPainter, e, drawState) == 0)
				base.DrawLinkButtonAdorments(e, drawState);
		}
	}
	public class LinkWindowsXPBorderPainter : BorderPainter {
		PrimitivesPainter ppainter;
		public LinkWindowsXPBorderPainter(PrimitivesPainter ppainter) {
			this.ppainter = ppainter;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { 
			Rectangle r = e.Bounds;
			r.Inflate(-1, -1);
			r.Width --;
			return r;
		}
#if XtraV3
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle bounds) {
			Rectangle r = bounds;
			r.Inflate(1, 1);
			r.Width ++;
			return r;
		}
#else
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle bounds) {
			Rectangle r = e.Bounds;
			r.Inflate(1, 1);
			r.Width ++;
			return r;
		}
#endif
		protected PrimitivesPainter PPainter { get { return ppainter; } }
		public override void DrawObject(ObjectInfoArgs e) {
			Brush b1 = PPainter.DrawParameters.Colors.Brushes(BarXPColor.ToolbarEdgeShadow),
				  b2 = PPainter.DrawParameters.Colors.Brushes(BarXPColor.ToolbarEdgeHighlight);
			Rectangle r = Rectangle.Inflate(e.Bounds, 0, -2);
			r.X = r.Right - 1;r.Height ++;
			r.Width = 1;
			e.Graphics.FillRectangle(b1, r); r.X ++;
			e.Graphics.FillRectangle(b2, r);
		}
	}
}
