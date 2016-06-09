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
	public class BarQMenuCustomizationOffice2003LinkPainter : BarQMenuCustomizationLinkPainter  {
		public BarQMenuCustomizationOffice2003LinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		protected override Brush CalcLinkHighlightedBrush(BarLinkPaintArgs e, BarLinkState state, BarLinkParts part) {
			BarButtonItemLink link = e.LinkInfo.Link as BarButtonItemLink;
			if(part == BarLinkParts.Glyph && e.LinkInfo.IsLinkInMenu && (e.LinkInfo.LinkState & BarLinkState.Checked) != 0) {
				if((e.LinkInfo.LinkState & (BarLinkState.Pressed | BarLinkState.Highlighted)) != 0)
					return DrawParameters.Colors.Brushes(BarColor2003.LinkCheckedBackColor2);
				return DrawParameters.Colors.Brushes(BarColor.LinkCheckedBackColor);
			}
			return base.CalcLinkHighlightedBrush(e, state, part);
		}
	}
	public class BarBaseButtonOffice2003LinkPainter : BarCheckButtonLinkPainter {
		public BarBaseButtonOffice2003LinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override Brush CalcLinkHighlightedBrush(BarLinkPaintArgs e, BarLinkState state, BarLinkParts part) {
			BarButtonItemLink link = e.LinkInfo.Link as BarButtonItemLink;
			if(part == BarLinkParts.Glyph && e.LinkInfo.IsLinkInMenu && (e.LinkInfo.LinkState & BarLinkState.Checked) != 0) {
				if((e.LinkInfo.LinkState & (BarLinkState.Pressed | BarLinkState.Highlighted)) != 0)
					return DrawParameters.Colors.Brushes(BarColor2003.LinkCheckedBackColor2);
				return DrawParameters.Colors.Brushes(BarColor.LinkCheckedBackColor);
			}
			return base.CalcLinkHighlightedBrush(e, state, part);
		}
	}
	public class BarButtonOffice2003LinkPainter : BarButtonLinkPainter {
		public BarButtonOffice2003LinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override Brush CalcLinkHighlightedBrush(BarLinkPaintArgs e, BarLinkState state, BarLinkParts part) {
			BarButtonItemLink link = e.LinkInfo.Link as BarButtonItemLink;
			if(part == BarLinkParts.Glyph && e.LinkInfo.IsLinkInMenu && (e.LinkInfo.LinkState & BarLinkState.Checked) != 0) {
					if((e.LinkInfo.LinkState & (BarLinkState.Pressed | BarLinkState.Highlighted)) != 0)
						return DrawParameters.Colors.Brushes(BarColor2003.LinkCheckedBackColor2);
				return DrawParameters.Colors.Brushes(BarColor.LinkCheckedBackColor);
			}
			return base.CalcLinkHighlightedBrush(e, state, part);
		}
	}
	public class PrimitivesPainterOffice2003 : PrimitivesPainter { 
		public PrimitivesPainterOffice2003(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override ObjectPainter CreateDefaultLinkBorderPainter() {
			return new LinkOffice2003BorderPainter();
		}
		protected override MenuHeaderPainter CreateDefaultMenuHeaderPainter() {
			return new MenuHeaderOffice2003Painter();
		}
		public override void DrawLinkNormalBackground(BarLinkPaintArgs e, Brush brush, Rectangle r) {
		}
		public override void DrawLinkHighLightedBackground(BarLinkPaintArgs e, Rectangle r, BarLinkEmptyBorder border, BarLinkState realState, Brush backBrush) {
			DrawLinkHighlightedFrame(e, ref r, border);
			Brush brush = null;
			LinearGradientMode gmode = (e.LinkInfo.DrawMode != BarLinkDrawMode.Vertical ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal);
			if(realState == BarLinkState.Pressed || realState == BarLinkState.Checked) {
				if(realState == BarLinkState.Checked) {
					brush = e.Cache.GetGradientBrush(r, DrawParameters.Colors[BarColor.LinkCheckedBackColor], DrawParameters.Colors[BarColor2003.LinkCheckedBackColor2], gmode);
				} else {
					brush = e.Cache.GetGradientBrush(r, DrawParameters.Colors[BarColor.LinkPressedBackColor], DrawParameters.Colors[BarColor2003.LinkPressedBackColor2], gmode);
				}
			} else {
				bool isOpened = IsLinkOpened(e.LinkInfo.Link);
				if(isOpened && !e.LinkInfo.IsLinkInMenu) {
					brush = e.Cache.GetGradientBrush(r, e.LinkInfo.OwnerAppearance.BackColor, e.LinkInfo.OwnerAppearance.BackColor2, gmode);
				}
				else
					brush = e.Cache.GetGradientBrush(r, DrawParameters.Colors[BarColor.LinkHighlightBackColor], DrawParameters.Colors[BarColor2003.LinkHighlightBackColor2], gmode);
			}
			e.Paint.FillRectangle(e.Graphics, brush, r);
		}
		bool IsLinkOpened(BarItemLink link) {
			BarButtonItemLink btnLink = link as BarButtonItemLink;
			if(btnLink != null) return btnLink.IsPopupVisible;
			BarCustomContainerItemLink ctnLink = link as BarCustomContainerItemLink;
			if(ctnLink != null) return ctnLink.Opened;
			return false;
		}
		public override void DrawSeparator(GraphicsInfoArgs e, RectInfo info, BarLinkViewInfo li, object sourceInfo) {
			BarControlViewInfo bcInfo = li.BarControlInfo as BarControlViewInfo;
			if(bcInfo != null && bcInfo.Bar != null && bcInfo.Bar.IsStatusBar) {
				base.DrawSeparator(e, info, li, sourceInfo);
				return;
			}
			Rectangle r = info.Rect;
			int th = 2;
			bool vert = info.Type == RectInfoType.VertSeparator;
			int width = (vert ? info.Rect.Width : info.Rect.Height);
			int indt = (width - th) / 2;
			if(vert) {
				r.Y += 3;
				r.Width = 1;
				r.X += indt;
				r.Height -= 6;
			} else {
				r.Height = 1;
				r.Y += indt;
				r.Width -= 6;
				r.X += 4;
			}
			e.Paint.FillRectangle(e.Graphics, DrawParameters.Colors.Brushes(BarColor2003.BarSeparatorColor1), r);
			r.Offset(1, 1);
			e.Paint.FillRectangle(e.Graphics, DrawParameters.Colors.Brushes(BarColor2003.BarSeparatorColor2), r);
		}
	}
	public class DockControlOffice2003Painter : BarDockControlPainter {
		public DockControlOffice2003Painter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected virtual Rectangle GetGradientRectangle(CustomViewInfo info) {
			DockControlViewInfo vi = (DockControlViewInfo)info;
			if(vi.BarControl.DockStyle == BarDockStyle.Right) return Rectangle.Empty;
			if(vi.BarControl.DockStyle == BarDockStyle.Left) {
				Control ctrl = vi.BarControl.Parent;
				if(ctrl == null) return Rectangle.Empty;
				return new Rectangle(ctrl.ClientRectangle.X, 0, ctrl.ClientRectangle.Width, 1);
			}
			return new Rectangle(vi.Bounds.X, 0, vi.Bounds.Width, 100);
		}
		public override void Draw(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
			base.Draw(e, info, sourceInfo);
			DockControlViewInfo vi = (DockControlViewInfo)info;
			Rectangle r = GetGradientRectangle(info);
			if(r.IsEmpty) {
				e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(vi.Appearance.Normal.BackColor2), vi.Bounds);
			} else {
				Brush brush = e.Cache.GetGradientBrush(r, vi.Appearance.Normal.BackColor, vi.Appearance.Normal.BackColor2, LinearGradientMode.Horizontal);
				e.Graphics.FillRectangle(brush, e.Bounds);
			}
			DrawDesignTimeSelection(e, vi);
		}
	}
	public class BarOffice2003Painter : BarPainter {
		public BarOffice2003Painter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected virtual void DrawStatusBarBackground(GraphicsInfoArgs e, BarControlViewInfo info) {
		}
		public override void DrawBackgroundSurfaceCore(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			if(info.Bar != null) {
				if(info.Bar.IsStatusBar)
					DrawStatusBarBackground(e, info);
				else {
					if(info.Bar.IsMainMenu)
						DrawMainMenuBackground(e, info);
					else
						DrawBarBackground(e, info);
				}
			}
			else {
				e.Graphics.FillRectangle(info.Appearance.Normal.GetBackBrush(e.Cache, info.Bounds), info.Bounds);
			}
		}
		public override void DrawBackground(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			DrawBackgroundSurface(e, info, sourceInfo);
			Rectangle r = info.Bounds;
			if(!info.DragBorder.IsEmpty) {
				DrawDragBorder(e, info);
			}
			if(!info.SizeGrip.IsEmpty)
				DrawSizeGrip(e, info);
		}
		protected virtual void DrawMainMenuBackground(GraphicsInfoArgs e, BarControlViewInfo info) {
			e.Graphics.FillRectangle(Brushes.Transparent, info.Bounds);
		}
		protected virtual void DrawBarCorners(GraphicsInfoArgs e, BarControlViewInfo info, bool isVertical) {
			Rectangle r = info.Bounds;
			if(isVertical) {
			} else {
				r.X ++;
				r.Width = 1;
				r.Height = 1;
				Color clr = Color.FromArgb(150, info.Appearance.Normal.BackColor);
				Brush brush = e.Cache.GetSolidBrush(clr);
				e.Paint.FillRectangle(e.Graphics, brush, r);
				e.Graphics.ExcludeClip(r);
				r.Y ++; r.X = info.Bounds.X;
				e.Paint.FillRectangle(e.Graphics, brush, r);
				e.Graphics.ExcludeClip(r);
			}
		}
		protected virtual void DrawBarBackground(GraphicsInfoArgs e, BarControlViewInfo info) {
			LinearGradientBrush brush;
			bool isVerticalBar = info.BarControl != null && info.BarControl.IsVertical;
			DrawBarCorners(e, info, isVerticalBar); 
			Color c1, c2;
			Rectangle brushRect;
			LinearGradientMode gmode;
			c1 = info.Appearance.Normal.BackColor;
			c2 = info.Appearance.Normal.BackColor2;
			if(isVerticalBar) {
				brushRect = new Rectangle(0, 0, info.Bounds.Width, 1);
				gmode = LinearGradientMode.Horizontal;
			} else {
				brushRect = new Rectangle(0, 0, 1, info.Bounds.Height);
				gmode = LinearGradientMode.Vertical;
			}
			Brush fillBrush;
			fillBrush = brush = e.Cache.GetGradientBrush(brushRect, c1, c2, gmode, 4) as LinearGradientBrush;
			if(brush != null) {
				Blend bl = new Blend(4);
				bl.Positions = new float[] { 0f, 0.5f, 0.8f, 1f };
				bl.Factors = new float[] { 0f, 0.3f, 0.9f, 1f };
				brush.Blend = bl;
				e.Graphics.FillRectangle(fillBrush, info.Bounds);
			}
			DrawBarBorder(e, info);
		}
		protected virtual void DrawBarBorder(GraphicsInfoArgs e, BarControlViewInfo info) {
			Rectangle r = info.Bounds;
			if(info.IsVertical) {
				r.Width = 1;
				r.X = info.Bounds.Right - 1;
			} else {
				r.Height = 1;
				r.Y = info.Bounds.Bottom - 1;
			}
			Brush border = info.Appearance.Normal.BorderColor.IsEmpty ? DrawParameters.Colors.Brushes(BarColor2003.DockBarBorderColor) : e.Cache.GetSolidBrush(info.Appearance.Normal.BorderColor);
			e.Graphics.FillRectangle(border, r);
		}
		protected virtual void DrawDockedBorder(GraphicsInfoArgs e, BarControlViewInfo info) {
		}
		protected override void DrawDragBorder(GraphicsInfoArgs e, BarControlViewInfo info) {
			if(!info.DragBorder.IsEmpty) {
				int end = !info.IsVertical ? info.DragBorder.Height : info.DragBorder.Width;
				Brush b1 = DrawParameters.Colors.Brushes(BarColor2003.DragBorderColor1), b2 = DrawParameters.Colors.Brushes(BarColor2003.DragBorderColor2);
				Rectangle r;
				int step = 4;
				if(DrawParameters.BarManagerImageScaleFactor > 1) step ++; 
				for(int n = 4; n < end - ( info.IsVertical ? 4 : 7); n += step) {
					r = info.DragBorder;
					r.Width = 2;r.Height = 2;
					if(info.IsVertical) {
						r.X += n;
						r.Y += 3;
					} else {
						r.Y += n;
						r.X += 3;
					}
					r.Offset(1, 1);
					e.Paint.FillRectangle(e.Graphics, b2, r);
					r.Offset(-1, -1);
					e.Paint.FillRectangle(e.Graphics, b1, r);
				}
			}
		}
	}
	public class BarQBarCustomizationOffice2003LinkPainter : BarQBarCustomizationLinkPainter { 
		public BarQBarCustomizationOffice2003LinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override void DrawLinkVertical(BarLinkPaintArgs e) {
			Color c1, c2;
			Rectangle r = e.LinkInfo.SelectRect;
			r.Y += 2; r.Height -= 2;
			CalcColors(e, out c1, out c2);
			Brush brush = e.Cache.GetGradientBrush(r, c1, c2, LinearGradientMode.Horizontal);
			e.Paint.FillRectangle(e.Graphics, brush, r);
			brush = e.Cache.GetSolidBrush(ControlPaint.Light(c1, 1f));
			Rectangle cr = r;
			cr.Width = 1; cr.Height = 1; cr.Y -= 2;
			e.Paint.FillRectangle(e.Graphics, brush, cr);
			cr.Y ++; e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(c1), cr);
			cr.X ++; e.Paint.FillRectangle(e.Graphics, brush, cr);
			cr.X = r.X; cr.Y = r.Bottom - 2; e.Paint.FillRectangle(e.Graphics, brush, cr);
			cr.Y ++; cr.X ++;e.Paint.FillRectangle(e.Graphics, brush, cr);
			brush = e.Cache.GetSolidBrush(ControlPaint.Light(c2, 0.5f));
			cr.X = r.Right - 1; cr.Y = r.Y - 2;
			e.Paint.FillRectangle(e.Graphics, brush, cr);
			cr.Y ++; e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(c2), cr);
			cr.X --;e.Paint.FillRectangle(e.Graphics, brush, cr);
			cr.Y = r.Bottom - 2; cr.X = r.Right - 1; e.Paint.FillRectangle(e.Graphics, brush, cr);
			cr.X --; cr.Y ++;e.Paint.FillRectangle(e.Graphics, brush, cr);
			Size arrSize = new Size(3, 5);
			Point loc = new Point(r.Right - (arrSize.Height + 3), r.Y + (r.Height - arrSize.Height) / 2);
			DrawLinkCustArrow(e, loc, arrSize);
			DrawLinkExpandMarks(e, new Point(r.X + 4, r.Y + (r.Height - 3 * 2 + 2) / 2));
		}
		protected override void DrawLinkHorizontal(BarLinkPaintArgs e) {
			Color c1, c2;
			Rectangle r = e.LinkInfo.SelectRect;
			r.X += 2; r.Width -= 2;
			CalcColors(e, out c1, out c2);
			Brush brush = e.Cache.GetGradientBrush(r, c1, c2, LinearGradientMode.Vertical);
			e.Paint.FillRectangle(e.Graphics, brush, r);
			brush = e.Cache.GetSolidBrush(ControlPaint.Light(c1, 1f));
			Rectangle cr = r;
			cr.Width = 1; cr.Height = 1; cr.X -= 2;
			e.Paint.FillRectangle(e.Graphics, brush, cr);
			cr.X ++; e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(c1), cr);
			cr.Y ++; e.Paint.FillRectangle(e.Graphics, brush, cr);
			cr.X = r.Right - 2; cr.Y = r.Y; e.Paint.FillRectangle(e.Graphics, brush, cr);
			cr.Y ++; cr.X ++;e.Paint.FillRectangle(e.Graphics, brush, cr);
			cr.X -= 1; e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(ControlPaint.Light(c1, .5f)), cr);
			brush = e.Cache.GetSolidBrush(ControlPaint.Light(c2, 0.5f));
			cr.X = r.X - 2; cr.Y = r.Bottom - 1;
			e.Paint.FillRectangle(e.Graphics, brush, cr);
			cr.X ++; e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(c2), cr);
			cr.Y --;e.Paint.FillRectangle(e.Graphics, brush, cr);
			cr.X = r.Right - 2; cr.Y = r.Bottom - 1; e.Paint.FillRectangle(e.Graphics, brush, cr);
			cr.Y --; cr.X ++;e.Paint.FillRectangle(e.Graphics, brush, cr);
			Size arrSize = new Size(5, 3);
			Point loc = new Point(r.X + (r.Width - arrSize.Width) / 2, r.Bottom - (arrSize.Height + 3));
			DrawLinkCustArrow(e, loc, arrSize);
			DrawLinkExpandMarks(e, new Point(r.X + (r.Width - 3 * 2 + 2) / 2, r.Y + 4));
		}
		protected virtual void DrawLinkExpandMarks(BarLinkPaintArgs e, Point loc) {
			bool drawExpand = (e.LinkInfo as BarQBarCustomizationLinkViewInfo).DrawExpandMark;
			if(!drawExpand) return;
			MarkArrowDirection dir = e.LinkInfo.DrawMode == BarLinkDrawMode.Horizontal ? MarkArrowDirection.Right : MarkArrowDirection.Down;
			Brush brush = DrawParameters.Colors.Brushes(BarColor2003.QuickBarCustLinkArrow2);
			for(int c = 0; c < 2; c++) {
				Point p = loc;
				for(int n = 0; n < 2; n++) {
					DrawExpandMark(e, p, brush, dir);
					if(dir == MarkArrowDirection.Right) p.X += 4;
					else p.Y += 4;
				}
				brush = DrawParameters.Colors.Brushes(BarColor2003.QuickBarCustLinkArrow1);
				loc.X --;loc.Y --;
			}
		}
		void DrawExpandMark(BarLinkPaintArgs e, Point loc, Brush brush, MarkArrowDirection dir) {
			Rectangle r = new Rectangle(loc.X, loc.Y, 1, 1);
			if(dir == MarkArrowDirection.Right) {
				r.Height = 3;
				e.Graphics.FillRectangle(brush, r);
				r.Height = 1; r.X ++; r.Y ++;
				e.Graphics.FillRectangle(brush, r);
			} else {
				r.Width = 3;
				e.Graphics.FillRectangle(brush, r);
				r.Width = 1; r.X ++; r.Y ++;
				e.Graphics.FillRectangle(brush, r);
			}
		}
		protected virtual void DrawLinkCustArrow(BarLinkPaintArgs e, Point loc, Size size) {
			Brush brush = DrawParameters.Colors.Brushes(BarColor2003.QuickBarCustLinkArrow2);
			loc.Offset(1, 1);
			MarkArrowDirection dir = e.LinkInfo.DrawMode == BarLinkDrawMode.Horizontal ? MarkArrowDirection.Down : MarkArrowDirection.Right;
			for(int n = 0; n < 2; n++) {
				if(dir == MarkArrowDirection.Down) 
					e.Graphics.FillRectangle(brush, new Rectangle(loc.X, loc.Y - 3, size.Width, 1));
				else
					e.Graphics.FillRectangle(brush, new Rectangle(loc.X - 3, loc.Y, 1, size.Height));
				DrawLinkArrow(e, loc, size, brush, dir);
				brush = DrawParameters.Colors.Brushes(BarColor2003.QuickBarCustLinkArrow1);
				loc.Offset(-1, -1);
			}
		}
		protected virtual void DrawLinkArrow(BarLinkPaintArgs e, Point loc, Size size, Brush brush, MarkArrowDirection arrow) {
			Point[] p = new Point[3];	
			int m = size.Height < 6 ?  1 : 0;
			switch(arrow) {
				case MarkArrowDirection.Right:
					p[0] = new Point(loc.X, loc.Y - m);
					p[1] = new Point(loc.X, loc.Y + size.Height);
					p[2] = new Point(loc.X  + size.Width, loc.Y + size.Height / 2);
					break;
				case MarkArrowDirection.Down:
					p[0] = new Point(loc.X, loc.Y);
					p[1] = new Point(loc.X + size.Width, loc.Y );
					p[2] = new Point(loc.X + size.Width / 2, loc.Y + size.Height);
					break;
				case MarkArrowDirection.Left:
					p[0] = new Point(loc.X + size.Width, loc.Y);
					p[1] = new Point(loc.X + size.Width, loc.Y + size.Height);
					p[2] = new Point(loc.X, loc.Y + size.Height / 2 + m);
					break;
			}
			e.Graphics.FillPolygon(brush, p); 
		}
		protected virtual void CalcColors(BarLinkPaintArgs e, out Color c1, out Color c2) {
			BarLinkState st = e.LinkInfo.CalcRealPaintState();
			switch(st) {
				case BarLinkState.Highlighted : 
					c1 = DrawParameters.Colors[BarColor.LinkHighlightBackColor];
					c2 = DrawParameters.Colors[BarColor2003.LinkHighlightBackColor2];
					break;
				case BarLinkState.Pressed: DrawLinkPressed(e); 
					c1 = DrawParameters.Colors[BarColor.LinkPressedBackColor];
					c2 = DrawParameters.Colors[BarColor2003.LinkPressedBackColor2];
					break;
				default:
					c1 = DrawParameters.Colors[BarColor2003.QuickBarCustLinkBackColor1];
					c2 = DrawParameters.Colors[BarColor2003.QuickBarCustLinkBackColor2];
					DrawLinkNormal(e);
					break;
			}
		}
	}
	public class FloatingOffice2003BarPainter : FloatingBarPainter { 
		public FloatingOffice2003BarPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public override void DrawBackgroundSurfaceCore(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			Rectangle client = info.Bounds, r;
			if(info.Bar != null && info.Bar.IsMainMenu) {
				Brush brush = e.Cache.GetGradientBrush(client, info.Appearance.Normal.BackColor, info.Appearance.Normal.BackColor2, LinearGradientMode.Horizontal);
				e.Graphics.FillRectangle(brush, client);
				return;
			}
			for(int n = 0; n < info.Rows.Count; n++) {
				BarControlRowViewInfo rowInfo = info.Rows[n] as BarControlRowViewInfo;
				r = new Rectangle(client.X, rowInfo.Bounds.Y, client.Width, rowInfo.Bounds.Height);
				Brush brush = e.Cache.GetGradientBrush(r, info.Appearance.Normal.BackColor, info.Appearance.Normal.BackColor2, LinearGradientMode.Vertical);
				e.Graphics.FillRectangle(brush, r);
			}
		}
		public override void DrawBackground(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			DrawBackgroundSurface(e, info, sourceInfo);
		}
	}
	public class BarRecentExpanderOffice2003LinkPainter : BarRecentExpanderLinkPainter {
		public BarRecentExpanderOffice2003LinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override void DrawLinkAdornmentsInMenu(BarLinkPaintArgs e, BarLinkState drawState) {
			Rectangle r = e.LinkInfo.Bounds;
			Bitmap bmp = GetManager(e.LinkInfo).GetController().GetBitmap("Office2003ExpandButton");
			r = new Rectangle(r.X + (r.Width - bmp.Width) / 2, r.Y + (r.Height - bmp.Height) / 2, bmp.Width, bmp.Height);
			PaintHelper.DrawImageCore(e.Graphics, bmp, r.X, r.Y, r.Width, r.Height, new Rectangle(Point.Empty, bmp.Size), (GetManager(e.LinkInfo).DrawParameters.Colors as BarOffice2003ColorConstants).ExpanderAttributes);
		}
	}
	public class MenuHeaderOffice2003Painter : MenuHeaderPainter {
		public override void DrawObject(BarLinkPaintArgs e) {
			e.Cache.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(Office2003Colors.Default[Office2003Color.Header]), e.LinkInfo.Bounds);
		}
		public override int CalcElementHeight(Graphics g, BarHeaderLinkViewInfo linkInfo, Size captionSize, int res) {
			return res;
		}
	}
}
