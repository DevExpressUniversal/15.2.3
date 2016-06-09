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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Data.Utils;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
namespace DevExpress.Office.Drawing {
	#region IGraphicsPainter
	public class IGraphicsPainter : GdiPlusPainterBase {
		readonly IGraphicsBase graphics;
		int transformLevel;
		RectangleF actualClipBounds;
		public IGraphicsPainter(IGraphicsBase graphics)
			: base() {
			Guard.ArgumentNotNull(graphics, "graphics");
			this.graphics = graphics;
			this.actualClipBounds = IGraphics.ClipBounds;
		}
		public IGraphicsBase IGraphics { get { return graphics; } }
		protected override RectangleF RectangularClipBounds { get { return actualClipBounds; } }
		protected override void SetClipBounds(RectangleF bounds) {
			IGraphics.ClipBounds = bounds;
			SetActualClipBounds(IGraphics.ClipBounds);
		}
		protected internal void SetActualClipBounds(RectangleF bounds) {
			actualClipBounds = bounds;
		}
		public bool HasTransform { get { return transformLevel != 0; } }
		public override int DpiY { get { return 300; } }
		public override void DrawImage(OfficeImage img, Rectangle bounds) {
			IGraphics.DrawImage(img.NativeImage, bounds);
		}
		public override void DrawImage(OfficeImage img, Rectangle bounds, Size imgActualSizeInLayoutUnits, ImageSizeMode sizing) {
			RectangleF oldClip = IGraphics.ClipBounds;
			Rectangle imgRect = Rectangle.Round(ImageTool.CalculateImageRectCore(bounds, imgActualSizeInLayoutUnits, sizing));
			GdiGraphics gdiGraphics = IGraphics as GdiGraphics;
			if (gdiGraphics != null) {
				RectangleF boundsF = bounds;
				PointF[] leftTop = new PointF[] { bounds.Location };
				gdiGraphics.Graphics.TransformPoints(CoordinateSpace.Device, CoordinateSpace.World, leftTop);
				leftTop[0] = new PointF((float)Math.Ceiling(leftTop[0].X), (float)Math.Ceiling(leftTop[0].Y));
				gdiGraphics.Graphics.TransformPoints(CoordinateSpace.World, CoordinateSpace.Device, leftTop);
				boundsF.Location = leftTop[0];
				IGraphics.ClipBounds = RectangleF.Intersect(boundsF, oldClip);
				imgRect = Rectangle.Round(ImageTool.CalculateImageRectCore(boundsF, imgActualSizeInLayoutUnits, sizing));
			}
			else
				IGraphics.ClipBounds = Rectangle.Intersect(bounds, Rectangle.Round(oldClip));
			DrawImage(img, imgRect);
			IGraphics.ClipBounds = oldClip;
		}
		public override void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
			IGraphics.DrawLine(pen, x1, y1, x2, y2);
		}
		public override void DrawLines(Pen pen, PointF[] points) {
			IGraphics.DrawLines(pen, points);
		}
		public override void DrawString(string text, FontInfo fontInfo, Rectangle rectangle) {
			DrawString(text, fontInfo, rectangle, StringFormat);
		}
		public override void DrawString(string text, FontInfo fontInfo, Rectangle rectangle, StringFormat stringFormat) {
			Brush brush = GetBrush(TextForeColor);
			IGraphics.DrawString(text, fontInfo.Font, brush, CorrectTextDrawingBounds(fontInfo, rectangle), stringFormat);
			ReleaseBrush(brush);
		}
#if !SL
		public override void DrawString(string text, Brush brush, Font font, float x, float y) {
			IGraphics.DrawString(text, font, brush, new PointF(x, y), StringFormat);
		}
		public override SizeF MeasureString(string text, Font font) {
			return IGraphics.MeasureString(text, font, float.MaxValue, StringFormat, GraphicsUnit.Pixel);
		}
#endif
		public override void FillRectangle(Color color, Rectangle bounds) {
			Brush brush = GetBrush(color);
			IGraphics.FillRectangle(brush, bounds);
			ReleaseBrush(brush);
		}
		public override void FillRectangle(Brush brush, Rectangle bounds) {
			IGraphics.FillRectangle(brush, bounds);
		}
		public override void FillRectangle(Color color, RectangleF bounds) {
			Brush brush = GetBrush(color);
			IGraphics.FillRectangle(brush, bounds);
			ReleaseBrush(brush);
		}
		public override void FillRectangle(Brush brush, RectangleF bounds) {
			IGraphics.FillRectangle(brush, bounds);
		}
		public override void FillEllipse(Brush brush, Rectangle bounds) {
			IGraphics.FillEllipse(brush, bounds);
		}
		public override void FillPolygon(Brush brush, PointF[] points) {
			GraphicsPath path = new GraphicsPath();
			path.AddPolygon(points);
			IGraphics.FillPath(brush, path);
		}
		public override void DrawRectangle(Pen pen, Rectangle bounds) {
			IGraphics.DrawRectangle(pen, bounds);
		}
#if !SL
		public override void DrawBrick(PrintingSystemBase ps, XtraPrinting.VisualBrick brick, Rectangle bounds) {						
		}
#endif
		public override void ExcludeCellBounds(Rectangle rect, Rectangle rowBounds) {
		}
		public override void ResetCellBoundsClip() {
		}
		public override Pen GetPen(Color color) {
			return new Pen(color);
		}
		public override Pen GetPen(Color color, float thickness) {
			return new Pen(color, thickness);
		}
		public override void ReleasePen(Pen pen) {
			pen.Dispose();
		}
		public override Brush GetBrush(Color color) {
			return new SolidBrush(color);
		}
		public override void ReleaseBrush(Brush brush) {
			brush.Dispose();
		}
		public override void SnapWidths(float[] widths) {
		}
		public override void SnapHeights(float[] heights) {
		}
		public override PointF GetSnappedPoint(PointF point) {
			return point;
		}
		protected override PointF[] TransformToPixels(PointF[] points) {
			return points;
		}
		protected override PointF[] TransformToLayoutUnits(PointF[] points) {
			return points;
		}
		public override void PushRotationTransform(Point center, float angleInDegrees) {
			transformLevel++;
			IGraphics.SaveTransformState();
			IGraphics.ResetTransform();
			IGraphics.ApplyTransformState(MatrixOrder.Append, false);
			IGraphics.TranslateTransform(center.X, center.Y);
			IGraphics.RotateTransform(angleInDegrees);
			IGraphics.TranslateTransform(-center.X, -center.Y);
		}
		public override void PopTransform() {
			transformLevel--;
			IGraphics.ResetTransform();
			IGraphics.ApplyTransformState(MatrixOrder.Prepend, true);
		}
		public override void PushSmoothingMode(bool highQuality) {
		}
		public override void PopSmoothingMode() {
		}
		public override void PushPixelOffsetMode(bool highQualtity) {
		}
		public override void PopPixelOffsetMode() {
		}
	}
	#endregion
#if !SL
	#region GdiGraphicsPainter
	public class GdiGraphicsPainter : IGraphicsPainter {
		readonly Stack<Region> clipRegions;
		public GdiGraphicsPainter(GdiGraphics graphics)
			: base(graphics) {
			this.clipRegions = new Stack<Region>();
		}
		public new GdiGraphics IGraphics { get { return (GdiGraphics)base.IGraphics; } }
		protected override void SetClipBounds(RectangleF bounds) {
			if (clipRegions.Count > 0) {
				using (Region region = clipRegions.Peek().Clone()) {
					region.Intersect(bounds);
					IGraphics.Clip = region;
				}
				SetActualClipBounds(bounds);
			}
			else
				base.SetClipBounds(bounds);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				while (clipRegions.Count > 0) {
					clipRegions.Pop().Dispose();
				}
			}
			base.Dispose(disposing);
		}
		public override void PushRotationTransform(Point center, float angleInDegrees) {
			base.PushRotationTransform(center, angleInDegrees);
			clipRegions.Push(IGraphics.Clip);
		}
		public override void PopTransform() {
			clipRegions.Pop().Dispose();
			base.PopTransform();
		}
	}
	#endregion
#endif
}
