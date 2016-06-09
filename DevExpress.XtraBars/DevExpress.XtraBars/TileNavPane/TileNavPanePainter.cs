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

using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Navigation {
	public class TileNavPanePainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			TileNavPaneViewInfo vi = e as TileNavPaneViewInfo;
			if(vi == null) return;
			Draw(vi);
		}
		public virtual void Draw(TileNavPaneViewInfo e) {
			DrawBackground(e);
			TileNavButtonPainter painter = new TileNavButtonPainter();
			foreach(TileNavButtonViewInfo button in e.Buttons) {
				if(button.IsVisible)
					ObjectPainter.DrawObject(e.Cache, painter, button);
			}
			painter = null;
		}
		protected virtual void DrawBackground(TileNavPaneViewInfo e) {
			if(e.TileNavPane.BackgroundImage != null)
				DrawBackgroundImage(e);
			else e.PaintAppearance.DrawBackground(e.Cache, e.ClientBounds);
		}
		protected virtual void DrawBackgroundImage(TileNavPaneViewInfo e) {
			if(e.TileNavPane.BackgroundImageLayout == ImageLayout.Tile)
				DrawBackgroundImageCore(e);
		}
		protected virtual void DrawBackgroundImageCore(TileNavPaneViewInfo e) {
			Point loc = e.Bounds.Location;
			for(; loc.Y < e.Bounds.Bottom; loc.Y += e.TileNavPane.BackgroundImage.Height) {
				loc.X = e.Bounds.X;
				for(; loc.X < e.Bounds.Right; loc.X += e.TileNavPane.BackgroundImage.Width) {
					Rectangle origin = new Rectangle(loc, e.TileNavPane.BackgroundImage.Size);
					Rectangle dest = origin;
					dest.Intersect(e.Bounds);
					e.Cache.Graphics.DrawImage(e.TileNavPane.BackgroundImage, dest, dest.X - origin.X, 0, dest.Width, dest.Height, GraphicsUnit.Pixel);
				}
			}
		}
	}
	public class TileNavButtonPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			TileNavButtonViewInfo vi = e as TileNavButtonViewInfo;
			if(vi == null) return;
			DrawButton(vi);
		}
		protected virtual void DrawButton(TileNavButtonViewInfo e) {
			if(e.ShouldDrawBackground)
				e.PaintAppearance.DrawBackground(e.Cache, e.Bounds);
			DrawButtonText(e);
			DrawButtonImage(e);
			DrawButtonDropDownGlyph(e);
			if(e.ControlInfo.TileNavPane.IsDesignMode)
				DrawItemDesignRectangle(e);
			DrawItemDropTargetSplitter(e);
			DrawFocusedRect(e);
		}
		private void DrawFocusedRect(TileNavButtonViewInfo e) {
			if(!e.ShouldDrawFocusRect) return;
			Rectangle r = e.Bounds;
			r.Inflate(-1, -1);
			e.Paint.DrawFocusRectangle(e.Graphics, r,
					e.PaintAppearance.ForeColor, Color.Empty);
		}
		private void DrawItemDropTargetSplitter(TileNavButtonViewInfo e) {
			if(!e.DrawDropSplitter) return;
			int x = e.Bounds.Left;
			x += 1;
			if(!e.DropSplitterIsLeft) {
				x = e.Bounds.Right;
				x -= 2;
			}
			Color c = e.ControlInfo.PaintAppearance.ForeColor;
			e.Cache.Graphics.DrawLine(new Pen(c, 3f), new Point(x, e.Bounds.Top), new Point(x, e.Bounds.Bottom));
		}
		protected virtual void DrawItemDesignRectangle(TileNavButtonViewInfo e) {
			if(e.ControlInfo.DesignTimeManager.IsComponentSelected(e.Element))
				e.ControlInfo.DesignTimeManager.DrawSelection(e.Cache, e.Bounds, Color.Magenta);
		}
		protected virtual void DrawButtonText(TileNavButtonViewInfo e) {
			StringInfo info = e.StringInfo;
			if(info == null || e.TextBounds.Width < 1 || e.TextBounds.Height < 1)
				return;
			XPaint prev = e.Cache.Paint;
			e.Cache.Paint = TileNavButtonViewInfo.GdiPaint;
			try {
				StringPainter.Default.DrawString(e.Cache, info, e.TextOptions);
			}
			finally {
				e.Cache.Paint = prev;
			}
		}
		protected virtual void DrawButtonImage(TileNavButtonViewInfo e) {
			if(!e.ShouldDrawGlyph)
				return;
			if(e.GlyphOpacity != 1.0f) {
				if(e.AllowGlyphSkinning)
					DrawGlypSkinnedOpacityImage(e);
				else
					DrawOpacityImage(e, e.Glyph, e.GlyphBounds, e.GlyphOpacity);
				return;
			}
			if(e.AllowGlyphSkinning) {
				DrawButtonImageColorized(e);
				return;
			}
			e.Cache.Graphics.DrawImage(e.Glyph, e.GlyphBounds);
		}
		private void DrawOpacityImage(TileNavButtonViewInfo e, Image img, Rectangle bounds, float opacity) {
			ImageAttributes attr = new ImageAttributes();
			ColorMatrix mat = new ColorMatrix();
			mat.Matrix33 = opacity;
			attr.SetColorMatrix(mat);
			e.Cache.Graphics.DrawImage(img, bounds, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attr);
		}
		private void DrawGlypSkinnedOpacityImage(TileNavButtonViewInfo e) {
			Image img = ImageColorizer.GetColoredImage(e.Glyph, e.PaintAppearance.ForeColor);
			DrawOpacityImage(e, img, e.GlyphBounds, e.GlyphOpacity);
		}
		protected virtual void DrawButtonDropDownGlyph(TileNavButtonViewInfo e) {
			if(!e.DropDownVisible)
				return;
			DrawButtonImageColorizedCore(e, e.DropDownGlyphBounds, e.DropDownGlyph);
		}
		protected virtual void DrawButtonImageColorized(TileNavButtonViewInfo e) {
			Image img = e.Glyph;
			DrawButtonImageColorizedCore(e, e.GlyphBounds, img);
		}
		protected virtual void DrawButtonImageColorizedCore(TileNavButtonViewInfo e, Rectangle rect, Image img) {
			ImageAttributes attr = ImageColorizer.GetColoredAttributes(e.PaintAppearance.ForeColor);
			e.Cache.Graphics.DrawImage(img, rect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attr);
		}
	}
}
