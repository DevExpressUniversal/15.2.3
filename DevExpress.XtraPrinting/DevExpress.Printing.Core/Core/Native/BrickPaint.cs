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
using DevExpress.Utils;
using DevExpress.Data.Utils;
#if SL
using DevExpress.Xpf.Windows.Forms;
using System.Windows.Media;
using DevExpress.Xpf.Drawing.Drawing2D;
using DevExpress.Xpf.Drawing;
using Brush = DevExpress.Xpf.Drawing.Brush;
#else
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using DevExpress.Printing;
#endif
namespace DevExpress.XtraPrinting.Native {
#if !SL
	[Obsolete("This class is now obsolete. You should use the CheckBoxBrick for printing checkboxes.")]
	public static class CheckImages {
		static Image checkImage = ResourceImageHelper.CreateImageFromResources(NativeSR.CheckFileName, typeof(DevExpress.Printing.ResFinder));
		static Image uncheckImage = ResourceImageHelper.CreateImageFromResources(NativeSR.UncheckFileName, typeof(DevExpress.Printing.ResFinder));
		static Image checkGrayImage = ResourceImageHelper.CreateImageFromResources(NativeSR.CheckGreyFileName, typeof(DevExpress.Printing.ResFinder));
		public static Image GetImage(CheckState checkState) {
			return (checkState == CheckState.Checked) ? checkImage :
				(checkState == CheckState.Unchecked) ? uncheckImage :
				(checkState == CheckState.Indeterminate) ? checkGrayImage :
				null;
		}
	}
#endif
	public class BrickPaintBase {
		#region static
		static SizeF AlignSizeInDocToPixels(SizeF size) {
			size = GraphicsUnitConverter.Convert(size, GraphicsDpi.Document, GraphicsDpi.Pixel);
			size = Size.Ceiling(size);
			return GraphicsUnitConverter.Convert(size, GraphicsDpi.Pixel, GraphicsDpi.Document);
		}
		static SizeF AlignSizeInLayoutUnitsToPixels(SizeF size, float layoutUnitDpi) {
			size = GraphicsUnitConverter.Convert(size, layoutUnitDpi, GraphicsDpi.Pixel);
			size = Size.Ceiling(size);
			return GraphicsUnitConverter.Convert(size, GraphicsDpi.Pixel, layoutUnitDpi);
		}
		#endregion
		protected BrickStyle style;
		readonly bool patchTransparentBackground;
		public BrickStyle BrickStyle {
			get { return style; }
			set { style = value; }
		}
		protected Color BackColor {
			get {
				if(!patchTransparentBackground)
					return style.BackColor;
				return DXColor.IsTransparentOrEmpty(style.BackColor) ? DXColor.White : style.BackColor;
			}
		}
		public BrickPaintBase(bool patchTransparentBackground) {
			this.patchTransparentBackground = patchTransparentBackground;
			BrickStyle = BrickStyle.CreateDefault();
		}
		void SetClipRegion(IGraphics gr, RectangleF rect) {
			rect.Intersect(gr.ClipBounds);
			gr.ClipBounds = rect;
		}
		public void FillRect(IGraphics gr, RectangleF rect, GdiHashtable gdi) {
			gr.FillRectangle(gdi.GetBrush(BackColor), rect);
		}
		public void DrawRect(IGraphics gr, RectangleF rect, GdiHashtable gdi) {
			DrawRect(gr, rect, gdi, GraphicsDpi.Document);
		}
		public void DrawRect(IGraphics gr, RectangleF rect, GdiHashtable gdi, float toDpi) {
			FillRect(gr, rect, gdi);
			DrawBorder(gr, rect, gdi, toDpi);
		}
		public void DrawTrackBar(IGraphics gr, RectangleF rect, GdiHashtable gdi, int position, int min, int max) {
			int offset = 8;
			Size thumbSize = new Size(20, (int)rect.Height - 2 * offset);
			RectangleF thumb = new RectangleF(rect.X + (rect.Width - thumbSize.Width) * (position-min) / (max - min), rect.Y - thumbSize.Height / 2 + rect.Height / 2, thumbSize.Width, thumbSize.Height);
			gr.DrawLine(gdi.GetPen(style.ForeColor, 2f), new PointF(rect.X, rect.Y + rect.Height / 2f), new PointF(rect.Right, rect.Y + rect.Height / 2f));
			gr.FillRectangle(gdi.GetBrush(style.ForeColor), thumb);
			if(BorderSide.None == style.Sides) return;
			DrawBorder(gr, rect, gdi.GetBrush(style.BorderColor), style.Sides);
		}
		public void DrawImage(IGraphics gr, Image image, RectangleF boundsRect, SizeF imageSize, PointF point, GdiHashtable gdi, ImageSizeMode sizeMode) {
			DrawRect(gr, boundsRect, gdi);
			if(image == null) return;
			AdjustClientRect(ref boundsRect);
			DrawImage(gr, image, new RectangleF(point, GraphicsUnitConverter.Convert(imageSize, GraphicsDpi.Pixel, GraphicsDpi.Document)), boundsRect);
		}
		public void DrawImage(IGraphics gr, Image image, RectangleF boundsRect, SizeF imageSize, GdiHashtable gdi, ImageSizeMode sizeMode) {
			DrawRect(gr, boundsRect, gdi);
			if (image == null) return;
			AdjustClientRect(ref boundsRect);
			DrawImage(gr, image, ImageTool.CalculateImageRect(boundsRect, imageSize, sizeMode), boundsRect);
		}
		void DrawImage(IGraphics gr, Image image, RectangleF imageRect, RectangleF clipRect) {
			RectangleF oldClip = gr.ClipBounds;
			SetClipRegion(gr, clipRect);
			try {
				gr.DrawImage(image, imageRect, style.BackColor);
			}
			finally {
				gr.ClipBounds = oldClip;
			}
		}
		[Obsolete("This method is now obsolete. You should use the DrawCheck(CheckState, IGraphics, RectangleF, SizeF, GdiHashtable) method instead.")]
		public void DrawCheck(CheckState state, IGraphics gr, RectangleF rect, GdiHashtable gdi) {
			DrawCheck(state, gr, rect, new SizeF(12, 12), gdi);
		}
		public virtual void DrawCheck(CheckState state, IGraphics gr, RectangleF rect, SizeF sizeInPixels, GdiHashtable gdi) {
			DrawCheck(state, gr, rect, sizeInPixels, gdi, false);
		}
		public virtual void DrawCheck(CheckState state, IGraphics gr, RectangleF rect, SizeF sizeInPixels, GdiHashtable gdi, bool shouldAlignToBottom) {
			DrawCheck(state, gr, rect, sizeInPixels, gdi, shouldAlignToBottom, GraphicsDpi.Document);
		}
		public virtual void DrawCheck(CheckState state, IGraphics gr, RectangleF rect, SizeF sizeInPixels, GdiHashtable gdi, bool shouldAlignToBottom, float toDpi) {
			DrawCheckCore(state, gr, rect, sizeInPixels, gdi, DXColor.Gray, DXColor.Black, shouldAlignToBottom, toDpi);
		}
		public void DrawToggleSwitch(bool isOn, IGraphics gr, object[] images, RectangleF rect, GdiHashtable gdi) {
			float x, y;
			Image im = (Image)images[0];
			SizeF s = GraphicsUnitConverter.Convert(im.Size, GraphicsDpi.Pixel, GraphicsDpi.Document);
			x = rect.X + (rect.Width - s.Width) / 2;
			y = rect.Y + (rect.Height - s.Height) / 2;			
			PointF p = new PointF(x, y);
			DrawImage(gr, im, rect, im.Size, p, gdi, ImageSizeMode.AutoSize);
			im = (Image)images[1];
			SizeF s1 = GraphicsUnitConverter.Convert(im.Size, GraphicsDpi.Pixel, GraphicsDpi.Document);
			x = rect.X + (rect.Width - s1.Width) / 2;
			y = rect.Y + (rect.Height - s1.Height) / 2;
			PointF p1 = new PointF(x, y);
			gr.DrawImage(im, new RectangleF(p1, GraphicsUnitConverter.Convert(im.Size, GraphicsDpi.Pixel, GraphicsDpi.Document)), style.BackColor);		   
			im = (Image)images[2];
			SizeF s2 = GraphicsUnitConverter.Convert(im.Size, GraphicsDpi.Pixel, GraphicsDpi.Document);
			x = p.X + s.Width - s2.Width;			
			PointF p2 = new PointF(x, p.Y);
			gr.DrawImage(im, new RectangleF(isOn ? p2 : p, GraphicsUnitConverter.Convert(im.Size, GraphicsDpi.Pixel, GraphicsDpi.Document)), style.BackColor);
		}
		protected void DrawCheckCore(CheckState state, IGraphics gr, RectangleF rect, SizeF sizeInPixels, GdiHashtable gdi, Color borderColor, Color markColor, bool shouldAlignToBottom, float toDpi) {
			DrawRect(gr, rect, gdi, toDpi);
			RectangleF oldClip = gr.ClipBounds;
			SetClipRegion(gr, rect);
			SizeF size = GraphicsUnitConverter.Convert(sizeInPixels, GraphicsDpi.Pixel, toDpi);
			RectangleF checkBoxRect = new RectangleF(new PointF(0, 0), size);
			checkBoxRect = !shouldAlignToBottom ? RectF.Center(checkBoxRect, rect) : RectF.Align(checkBoxRect, rect, BrickAlignment.Center, BrickAlignment.Far);
			checkBoxRect = DrawCheckBorder(gr, checkBoxRect, gdi, borderColor, toDpi);
			switch (state) {
				case CheckState.Checked:
					DrawCheckMark(gr, checkBoxRect, size, gdi, markColor);
					break;
				case CheckState.Unchecked:
					break;
				default:
					DrawCheckUndefinedMark(gr, checkBoxRect, size, gdi, markColor);
					break;
			}
			gr.ClipBounds = oldClip;
		}
		protected RectangleF DrawCheckBorder(IGraphics gr, RectangleF checkBoxRect, GdiHashtable gdi, Color color, float toDpi) {
			IPixelAdjuster pixelAdjuster = gr as IPixelAdjuster;
			RectangleF r = checkBoxRect;
			SizeF borderSize = MathMethods.Scale(checkBoxRect.Size, -0.1f);
			if (pixelAdjuster != null) {
				SizeF minSize = pixelAdjuster.GetDevicePointSize();
				checkBoxRect = pixelAdjuster.AdjustRect(checkBoxRect);
				borderSize = MathMethods.Scale(checkBoxRect.Size, -0.1f);
				borderSize = pixelAdjuster.AdjustSize(borderSize);
				r.Inflate(borderSize);
				r = pixelAdjuster.AdjustRect(r);
				if (r.Bottom == checkBoxRect.Bottom) {
					r.Height -= minSize.Height;
				}					
				if (r.Top == checkBoxRect.Top) {
					r.Y += minSize.Height;
					r.Height -= minSize.Height;
				}
				if (r.Right == checkBoxRect.Right) {
					r.Width -= minSize.Width;
				}
				if (r.Left == checkBoxRect.Left) {
					r.X += minSize.Width;
					r.Width -= minSize.Width;
				}
			}
			else {
				r.Inflate(AlignSizeInLayoutUnitsToPixels(borderSize, toDpi));
			}
			gr.FillRectangle(gdi.GetBrush(color), checkBoxRect);
			gr.FillRectangle(gdi.GetBrush(DXColor.White), r);
			return checkBoxRect;
		}
		void DrawCheckUndefinedMark(IGraphics gr, RectangleF checkBoxRect, SizeF size, GdiHashtable gdi, Color color) {
			RectangleF r = checkBoxRect;
			r.Inflate(MathMethods.Scale(size, -0.35f));
			gr.FillRectangle(gdi.GetBrush(color), r);
		}
		void DrawCheckMark(IGraphics gr, RectangleF checkBoxRect, SizeF size, GdiHashtable gdi, Color color) {
			PointF[] points = { 
				new PointF(2f/9, 3f/9), new PointF(4f/9, 5f/9), new PointF(7f/9, 2f/9), new PointF(7f/9, 4f/9),
				new PointF(4f/9, 7f/9), new PointF(2f/9, 5f/9), new PointF(2f/9, 3f/9)
			};
			byte[] pointTypes = { 
				(byte)PathPointType.Start, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line,
				(byte)PathPointType.Line, (byte)PathPointType.Line, (byte)(PathPointType.Line | PathPointType.CloseSubpath) 
			};
			using(GraphicsPath graphicsPath = new GraphicsPath(points, pointTypes))
			using(Matrix translateMatrix = new Matrix()) {
				translateMatrix.Translate(checkBoxRect.Left, checkBoxRect.Top);
				translateMatrix.Scale(size.Width, size.Height);
				graphicsPath.Transform(translateMatrix);
				gr.FillPath(gdi.GetBrush(color), graphicsPath);
			}
		}
		public void DrawString(string s, IGraphics gr, RectangleF rect, GdiHashtable gdi) {
			DrawRect(gr, rect, gdi);
			AdjustClientRect(ref rect);
			gr.DrawString(s, style.Font, gdi.GetBrush(style.ForeColor), rect, style.StringFormat.Value);
		}
		public virtual RectangleF GetClientRect(RectangleF rect) {
			return rect;
		}
		protected void AdjustClientRect(ref RectangleF rect) {
			rect = style.Padding.Deflate(GetClientRect(rect), GraphicsDpi.Document);
		}
		public void DrawBorder(IGraphics gr, RectangleF rect, GdiHashtable gdi) {
			DrawBorder(gr, rect, gdi.GetBrush(style.BorderColor), style.Sides, GraphicsDpi.Document);
		}
		public void DrawBorder(IGraphics gr, RectangleF rect, GdiHashtable gdi, float toDpi) {
			DrawBorder(gr, rect, gdi.GetBrush(style.BorderColor), style.Sides, toDpi);
		}
		protected virtual void DrawBorder(IGraphics gr, RectangleF rect, Brush brush, BorderSide sides) {
			DrawBorder(gr, rect, brush, sides, GraphicsDpi.Document);
		}
		protected virtual void DrawBorder(IGraphics gr, RectangleF rect, Brush brush, BorderSide sides, float toDpi) {
		}
	}
	public class BrickPaint : BrickPaintBase {
		public BrickPaint() : base(false) { }
		public override RectangleF GetClientRect(RectangleF rect) {
			return this.BrickStyle.DeflateBorderWidth(rect, GraphicsDpi.Document, true);
		}
		protected override void DrawBorder(IGraphics gr, RectangleF rect, Brush brush, BorderSide sides, float toDpi) {
			float borderWidth = GraphicsUnitConverter.Convert(style.BorderWidth, GraphicsDpi.Pixel, toDpi);
			if (borderWidth == 0 || sides == BorderSide.None)
				return;
			AdjustBorderRect(ref rect, borderWidth);
			borderWidth = Math.Min(borderWidth, Math.Min(rect.Width, rect.Height));
			DrawDashStyleBorders(gr, style.BorderDashStyle, rect, brush, sides, borderWidth);
		}
		internal static void DrawDashStyleBorders(IGraphics gr, BorderDashStyle dashStyle, RectangleF rect, Brush brush, BorderSide sides, float borderWidth) {
			if(dashStyle == BorderDashStyle.Solid)
				DrawSolidBorder(gr, rect, brush, sides, borderWidth);
			else if(dashStyle == BorderDashStyle.Double)
				DrawDoubleBorder(gr, rect, brush, sides, borderWidth);
			else
				DrawBorderDashDot(gr, rect, ((SolidBrush)brush).Color, sides, borderWidth, dashStyle);
		}
		static void DrawDoubleBorder(IGraphics gr, RectangleF rect, Brush brush, BorderSide sides, float borderWidth) {
			float thinBorderWidth = borderWidth / 3;
			DrawSolidBorder(gr, rect, brush, sides, thinBorderWidth);
			float inflateValue = -thinBorderWidth * 2;
			RectangleF innerRect = RectHelper.InflateRect(rect, -thinBorderWidth * 2, sides);
			DrawSolidBorder(gr, innerRect, brush, sides, thinBorderWidth);
		}
		static void DrawBorderDashDot(IGraphics gr, RectangleF rect, Color color, BorderSide sides, float borderWidth, BorderDashStyle dashStyle) {
			using(Pen pen = new Pen(color, borderWidth)) {
				pen.EndCap = pen.StartCap = LineCap.Square;
				pen.DashStyle = DashStyle.Custom;
				pen.DashPattern = VisualBrick.GetDashPattern(dashStyle);
				float strokeOffset = - borderWidth / 2;
				RectangleF bounds = RectangleF.Inflate(rect, strokeOffset, strokeOffset);
				if((sides & BorderSide.Left) > 0)
					gr.DrawLine(pen, RectHelper.TopLeft(bounds), RectHelper.BottomLeft(bounds));
				if((sides & BorderSide.Right) > 0)
					gr.DrawLine(pen, RectHelper.BottomRight(bounds), RectHelper.TopRight(bounds));
				if((sides & BorderSide.Top) > 0)
					gr.DrawLine(pen, RectHelper.TopRight(bounds), RectHelper.TopLeft(bounds));
				if((sides & BorderSide.Bottom) > 0)
					gr.DrawLine(pen, RectHelper.BottomLeft(bounds), RectHelper.BottomRight(bounds));
			}
		}
		static void DrawSolidBorder(IGraphics gr, RectangleF rect, Brush brush, BorderSide sides, float borderWidth) {
			RectangleF r = new RectangleF(rect.Location, new SizeF(borderWidth, rect.Height));
			if((sides & BorderSide.Left) > 0)
				gr.FillRectangle(brush, r);
			if((sides & BorderSide.Right) > 0) {
				r.Offset(rect.Width - borderWidth, 0);
				gr.FillRectangle(brush, r);
			}
			r = new RectangleF(rect.Location, new SizeF(rect.Width, borderWidth));
			if((sides & BorderSide.Top) > 0)
				gr.FillRectangle(brush, r);
			if((sides & BorderSide.Bottom) > 0) {
				r.Offset(0, rect.Height - borderWidth);
				gr.FillRectangle(brush, r);
			}
		}
		void AdjustBorderRect(ref RectangleF rect, float borderWidth) {
			if (style.BorderStyle == BrickBorderStyle.Center)
				rect.Inflate(borderWidth / 2, borderWidth / 2);
			else if (style.BorderStyle == BrickBorderStyle.Outset)
				rect.Inflate(borderWidth, borderWidth);
		}
	}
	public interface IPixelAdjuster {
		RectangleF AdjustRect(RectangleF bounds);
		SizeF AdjustSize(SizeF size);
		SizeF GetDevicePointSize();
	}
}
