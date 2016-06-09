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
using System.Drawing.Drawing2D;
using System.Drawing;
using DevExpress.Data.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.Office.Drawing {
	#region GdiPlusPainter
	public class GdiPlusPainter : GdiPlusPainterBase {
		class RichEditGdiGraphics : GdiGraphics, DevExpress.XtraPrinting.Native.IPixelAdjuster {
			public RichEditGdiGraphics(Graphics gr, PrintingSystemBase ps) : base(gr, ps) {
			}
			protected override void SetPageUnit() {
			}
			public RectangleF AdjustRect(RectangleF bounds) {
				PointF[] points = new PointF[] { new PointF(0, 0), new PointF(bounds.Left, bounds.Top), new PointF(bounds.Width, bounds.Height) };
				Graphics.TransformPoints(CoordinateSpace.Device, CoordinateSpace.World, points);
				points[0] = new PointF((int)(points[0].X + 0.5), (int)(points[0].Y + 0.5));
				points[1] = new PointF((int)(points[1].X + 0.5), (int)(points[1].Y + 0.5));
				points[2] = new PointF((int)(points[2].X + 0.5), (int)(points[2].Y + 0.5));
				Graphics.TransformPoints(CoordinateSpace.World, CoordinateSpace.Device, points);
				return new RectangleF(points[1].X, points[1].Y, points[2].X - points[0].X, points[2].Y - points[0].Y);
			}
			public SizeF AdjustSize(SizeF size) {
				PointF[] points = new PointF[] { new PointF(0, 0), new PointF(size.Width, size.Height) };
				Graphics.TransformPoints(CoordinateSpace.Device, CoordinateSpace.World, points);
				points[1] = new PointF((int)(points[1].X - points[0].X + 0.5), (int)(points[1].Y - points[0].Y + 0.5));
				points[0] = new PointF(0, 0);
				if (size.Width != 0 && points[1].X == 0)
					points[1].X = Math.Sign(size.Width);
				if (size.Height != 0 && points[1].Y == 0)
					points[1].Y = Math.Sign(size.Height);
				Graphics.TransformPoints(CoordinateSpace.World, CoordinateSpace.Device, points);
				return new SizeF(points[1].X - points[0].X, points[1].Y - points[0].Y);
			}
			public SizeF GetDevicePointSize() {
				PointF[] points = new PointF[] { new PointF(0, 0), new PointF(1, 1) };
				Graphics.TransformPoints(CoordinateSpace.World, CoordinateSpace.Device, points);
				return new SizeF(points[1].X - points[0].X, points[1].Y - points[0].Y);
			}
		}
		#region Fields
		readonly IGraphicsCache cache;
		Dictionary<OfficeImage, WeakReference> bitmapCache;
		readonly Stack<Matrix> transformsStack;
		readonly Stack<SmoothingMode> smoothingmodeStack;
		readonly Stack<PixelOffsetMode> pixelmodeStack;
		readonly Stack<Region> clipRegions;
		readonly Stack<RectangleF> rectangularBounds;
		RectangleF rectangularClipBounds;
		int dpiY;
		const int maxAccessibleLength = 2000;
		const int maxCachedBitmapSize = 2000 * 1200;
		#endregion
		public GdiPlusPainter(IGraphicsCache cache) {
			Guard.ArgumentNotNull(cache, "cache");
			this.cache = cache;
			this.transformsStack = new Stack<Matrix>();
			this.smoothingmodeStack = new Stack<SmoothingMode>();
			this.pixelmodeStack = new Stack<PixelOffsetMode>();
			this.clipRegions = new Stack<Region>();
			rectangularBounds = new Stack<RectangleF>();
			this.dpiY = (int)cache.Graphics.DpiY;
			rectangularClipBounds = Graphics.ClipBounds;
		}
		#region Properties
		public IGraphicsCache Cache { get { return cache; } }
		public Graphics Graphics { get { return cache.Graphics; } }
		public override int DpiY { get { return dpiY; } }
		protected override RectangleF RectangularClipBounds { get { return rectangularClipBounds; }  }		
		public bool HasTransform { get { return transformsStack.Count > 0; } }
		public Dictionary<OfficeImage, WeakReference> BitmapCache { get { return bitmapCache; } set { bitmapCache = value; } }
		#endregion
		public override void SnapHeights(float[] heights) {
			if(transformsStack.Count == 0)
				base.SnapHeights(heights);
		}
		public override void SnapWidths(float[] widths) {
			if (transformsStack.Count == 0)
				base.SnapWidths(widths);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (clipRegions != null) {
					while(clipRegions.Count > 0)
						clipRegions.Pop().Dispose();
				}
			}
			base.Dispose(disposing);
		}
		protected override void SetClipBounds(RectangleF bounds) {
			if (clipRegions.Count > 0) {
				using (Region region = clipRegions.Peek().Clone()) {
					region.Intersect(bounds);
					Graphics.SetClip(region, CombineMode.Replace);				   
				}
				rectangularClipBounds = bounds;
			}
			else {
				Graphics.SetClip(bounds);
				rectangularClipBounds = Graphics.ClipBounds;
			}
		}
		public override void FillRectangle(Brush brush, Rectangle actualBounds) {
			cache.FillRectangle(brush, actualBounds);
		}
		public override void FillRectangle(Color color, Rectangle actualBounds) {
			cache.FillRectangle(color, actualBounds);
		}
		public override void FillEllipse(Brush brush, Rectangle bounds) {
			Graphics.FillEllipse(brush, bounds);
		}
		public override void DrawRectangle(Pen pen, Rectangle bounds) {
			cache.DrawRectangle(pen, bounds);
		}
		public override void DrawString(string text, FontInfo fontInfo, Rectangle bounds) {
			DrawString(text, fontInfo, bounds, StringFormat);
		}
		public override void DrawString(string text, FontInfo fontInfo, Rectangle bounds, StringFormat stringFormat) {
			GdiPlusFontInfo gdiPlusFontInfo = (GdiPlusFontInfo)fontInfo;
			Brush foreBrush = cache.GetSolidBrush(TextForeColor);
			Graphics.DrawString(text, gdiPlusFontInfo.Font, foreBrush, CorrectTextDrawingBounds(fontInfo, bounds), StringFormat);
		}
#if !SL
		public override void DrawString(string text, Brush brush, Font font, float x, float y) {
			Graphics.DrawString(text, font, brush, new Rectangle((int)x, (int)y, int.MaxValue, int.MaxValue), StringFormat);
		}
		public override SizeF MeasureString(string text, Font font) {
			return Graphics.MeasureString(text, font, int.MaxValue, StringFormat);
		}
#endif
		public override void DrawImage(OfficeImage img, Rectangle bounds) {
			DrawImage(img, bounds, ImageSizeMode.Normal);
		}
		void DrawImage(OfficeImage img, Rectangle bounds, ImageSizeMode sizing) {
			RectangleF correctedBounds = SnapToDevicePixelsHelper.GetCorrectedBounds(Graphics, img.NativeImage.Size, bounds);
			Image cachedBmp = GetImageFromCache(img, new Size((int)correctedBounds.Size.Width, (int)correctedBounds.Size.Height));
			DrawImageCore(cachedBmp, correctedBounds, sizing);
		}
		protected virtual void DrawImageCore(Image img, RectangleF bounds, ImageSizeMode sizing) {
			Graphics.DrawImage(img, bounds);
		}
		public override void DrawImage(OfficeImage img, Rectangle bounds, Size imgActualSizeInLayoutUnits,DevExpress.XtraPrinting.ImageSizeMode sizing) {
			RectangleF oldClip = Graphics.ClipBounds;
			try {
				Rectangle imgRect = Rectangle.Round(ImageTool.CalculateImageRectCore(bounds, imgActualSizeInLayoutUnits, sizing));
				Rectangle newClip = Rectangle.Intersect(bounds, Rectangle.Round(oldClip));
				Graphics.SetClip(newClip);
				DrawImage(img, imgRect, sizing);
			}
			finally {
				Graphics.SetClip(oldClip);
			}
		}
		bool ShouldCacheImage(Size nativeImageSize, Size actualSize) {
			if (actualSize.Width * actualSize.Height > maxCachedBitmapSize)
				return false;
			if (nativeImageSize.Width >= maxAccessibleLength || nativeImageSize.Height >= maxAccessibleLength)
				return true;
			return actualSize.Width < nativeImageSize.Width && actualSize.Height < nativeImageSize.Width;
		}
		Image GetImageFromCache(OfficeImage img, Size actualSize) {
			if(this.bitmapCache == null || !ShouldCacheImage(img.NativeImage.Size, actualSize))
				return img.NativeImage;
			Image cachedBmp = null;
			WeakReference bmpRef = null;
			if(!this.bitmapCache.TryGetValue(img, out bmpRef))
				cachedBmp = AddBitmapToCache(img, actualSize);
			else {
				object obj = bmpRef.Target;
				if(obj == null)
					cachedBmp = AddBitmapToCache(img, actualSize);
				else {
					cachedBmp = (Image)obj;
					if(cachedBmp.Size.Width != actualSize.Width || cachedBmp.Size.Height != actualSize.Height) {
						cachedBmp.Dispose();
						cachedBmp = AddBitmapToCache(img, actualSize);
					}
				}
			}
			return cachedBmp;
		}
		Image AddBitmapToCache(OfficeImage img, Size sz) {
			Image cachedBmp = new Bitmap(img.NativeImage, sz.Width, sz.Height);
			this.bitmapCache[img] = new WeakReference(cachedBmp);
			return cachedBmp;
		}
		public override void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
			Graphics.DrawLine(pen, x1, y1, x2, y2);
		}
		public override void DrawLines(Pen pen, PointF[] points) {
			Graphics.DrawLines(pen, points);
		}
		public override void FillPolygon(Brush brush, PointF[] points) {
			SmoothingMode oldSmoothingMode = Graphics.SmoothingMode;
			Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			Graphics.FillPolygon(brush, points);
			Graphics.SmoothingMode = oldSmoothingMode;
		}
		public override void ExcludeCellBounds(Rectangle rect, Rectangle rowBounds) {
			Graphics.ExcludeClip(rect);
		}
		public override void ResetCellBoundsClip() {
		}
		public override void DrawBrick(PrintingSystemBase ps, XtraPrinting.VisualBrick brick, Rectangle bounds) {
			VisualBrickExporter exporter = (VisualBrickExporter)ExportersFactory.CreateExporter(brick);
			using (GdiGraphics gdiGraphics = new RichEditGdiGraphics(Graphics, ps)) {
				exporter.Draw(gdiGraphics, bounds, bounds);
			}
		}
		public override void FillRectangle(Color color, RectangleF bounds) {
			Brush brush = cache.GetSolidBrush(color);
			FillRectangle(brush, bounds);
		}
		public override void FillRectangle(Brush brush, RectangleF bounds) {
			cache.Graphics.FillRectangle(brush, bounds);
		}
		public override Pen GetPen(Color color) {
			return cache.GetPen(color);
		}
		public override Pen GetPen(Color color, float thickness) {
			return cache.GetPen(color, (int)thickness);
		}
		public override void ReleasePen(Pen pen) {
		}
		public override Brush GetBrush(Color color) {
			return cache.GetSolidBrush(color);
		}
		public override void ReleaseBrush(Brush brush) {
		}
		protected override PointF[] TransformToPixels(PointF[] points) {
			Graphics.TransformPoints(CoordinateSpace.Device, CoordinateSpace.World, points);
			return points;
		}
		protected override PointF[] TransformToLayoutUnits(PointF[] points) {
			Graphics.TransformPoints(CoordinateSpace.World, CoordinateSpace.Device, points);
			return points;
		}
		public override void PushRotationTransform(Point center, float angleInDegrees) {
			transformsStack.Push(Graphics.Transform.Clone());
			rectangularBounds.Push(rectangularClipBounds);
			Matrix transform = Graphics.Transform;
			transform.RotateAt(angleInDegrees, center);
			Graphics.Transform = transform;
			clipRegions.Push(Graphics.Clip);
			rectangularClipBounds = Graphics.ClipBounds;
		}
		public override void PopTransform() {
			using (Region region = clipRegions.Pop()) {
				Graphics.SetClip(region, CombineMode.Replace);
			}			
			Graphics.Transform = transformsStack.Pop();			
			SetClipBounds(rectangularBounds.Pop());			
		}
		public override void PushSmoothingMode(bool highQuality) {
			smoothingmodeStack.Push(Graphics.SmoothingMode);
			Graphics.SmoothingMode = highQuality ? SmoothingMode.HighQuality : SmoothingMode.Default;
		}
		public override void PopSmoothingMode() {
			Graphics.SmoothingMode = smoothingmodeStack.Pop();
		}
		public override void PushPixelOffsetMode(bool highQualtity) {
			pixelmodeStack.Push(Graphics.PixelOffsetMode);
			Graphics.PixelOffsetMode = highQualtity ? PixelOffsetMode.HighQuality : PixelOffsetMode.Default;
		}
		public override void PopPixelOffsetMode() {
			Graphics.PixelOffsetMode = pixelmodeStack.Pop();
		}
	}
	#endregion
}
