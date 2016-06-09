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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
using DevExpress.Data.Utils;
using DevExpress.Office.Internal;
#if SL
using DevExpress.XtraPrinting.Stubs;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Drawing;
using PlatformIndependentPen = DevExpress.Xpf.Drawing.Pen;
using PlatformIndependentBrush = System.Windows.Media.Brush;
using PlatformBrush = System.Windows.Media.Brush;
using PlatformIndependentColor = System.Windows.Media.Color;
using PlatformColor = System.Windows.Media.Color;
using PlatformIndependentImage = System.Windows.Controls.Image;
using PlatformImage = System.Windows.Controls.Image;
using TransformMatrix = DevExpress.Xpf.Core.Native.Matrix;
#else
using PlatformIndependentPen = System.Drawing.Pen;
using PlatformIndependentBrush = System.Drawing.Brush;
using PlatformBrush = System.Windows.Media.Brush;
using PlatformIndependentColor = System.Drawing.Color;
using PlatformImage = System.Windows.Controls.Image;
using TransformMatrix = System.Drawing.Drawing2D.Matrix;
#endif
namespace DevExpress.XtraRichEdit.Drawing {
	#region XpfPainter (abstract class)
	public abstract class XpfPainter : Painter {
		#region Fields
		readonly DocumentLayoutUnitConverter unitConverter;
		readonly List<RectangleGeometry> excludeRectangles;
		readonly Stack<TransformMatrix> transformStack;
		RectangleF clipBounds;
		float zoomFactor;
		Dictionary<object, ImageSource> imageSourceCache;
		bool useClipBounds;
		readonly int dpi;
		#endregion
		protected XpfPainter(DocumentLayoutUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
			this.excludeRectangles = new List<RectangleGeometry>();
			this.clipBounds = new RectangleF(float.NegativeInfinity, float.NegativeInfinity, float.PositiveInfinity, float.PositiveInfinity);
			this.transformStack = new Stack<TransformMatrix>();
			this.dpi = (int)Math.Round(unitConverter.ScreenDpi);
		}
		#region Properties
		protected override RectangleF RectangularClipBounds { get { return clipBounds; } }
		protected override void SetClipBounds(RectangleF bounds) {
			clipBounds = bounds;
		}
		public override int DpiY { get { return dpi; } }
		public virtual int DpiX { get { return dpi; } }
		public DocumentLayoutUnitConverter UnitConverter { get { return unitConverter; } }
		protected internal List<RectangleGeometry> ExcludeRectangles { get { return excludeRectangles; } }
		protected internal bool IsInfinityClipBounds { get { return float.IsNegativeInfinity(ClipBounds.X) && float.IsNegativeInfinity(ClipBounds.Y) && float.IsPositiveInfinity(ClipBounds.Width) && float.IsPositiveInfinity(ClipBounds.Height); } }
		public float ZoomFactor { get { return zoomFactor; } set { zoomFactor = value; } }
		internal Dictionary<object, ImageSource> ImageSourceCache { get { return imageSourceCache; } set { imageSourceCache = value; } }
		protected internal bool UseClipBounds { get { return useClipBounds; } }
		#endregion
		public override RectangleF ApplyClipBounds(RectangleF clipBounds) {
			this.useClipBounds = true;
			return base.ApplyClipBounds(clipBounds);
		}
		public override void RestoreClipBounds(RectangleF clipBounds) {
			base.RestoreClipBounds(clipBounds);
			this.useClipBounds = false;
		}
		public override PlatformIndependentPen GetPen(PlatformIndependentColor color) {
			return new PlatformIndependentPen(color);
		}
		public override PlatformIndependentPen GetPen(PlatformIndependentColor color, float thickness) {
			return new PlatformIndependentPen(color, thickness);
		}
		public override void ReleasePen(PlatformIndependentPen pen) {
			pen.Dispose();
		}
		public override PlatformIndependentBrush GetBrush(PlatformIndependentColor color) {
#if SL
			return new SolidColorBrush(XpfTypeConverter.ToPlatformColor(color));
#else
			return new SolidBrush(color);
#endif
		}
		public override void ReleaseBrush(PlatformIndependentBrush brush) {
		}
		public void SetWidthHeight(FrameworkElement elem, int w, int h) {
			double newW = Math.Max(UnitConverter.LayoutUnitsToPixelsF(w, DpiX), 1);
			double newH = Math.Max(UnitConverter.LayoutUnitsToPixelsF(h, DpiY), 1);
			SetWidthHeightInPixels(elem, newW, newH);
		}
		public void SetWidthHeightInPixels(FrameworkElement elem, double w, double h) {
			if (elem.Width != w || elem.Height != h) {
				if (Double.IsInfinity(w) || Double.IsInfinity(h))
					return;
				elem.Width = w;
				elem.Height = h;
			}
		}
		public MatrixTransform SetPositionInPixels(FrameworkElement elem, double x, double y) {
			MatrixTransform transform = GetTransform(elem);
			SetMatrixTransform(transform, x, y);
			if (elem.HorizontalAlignment != HorizontalAlignment.Left)
				elem.HorizontalAlignment = HorizontalAlignment.Left;
			if (elem.VerticalAlignment != System.Windows.VerticalAlignment.Top)
				elem.VerticalAlignment = System.Windows.VerticalAlignment.Top;
			return transform;
		}
		public void SetPosition(FrameworkElement elem, System.Drawing.Rectangle rect) {
			double newX = UnitConverter.LayoutUnitsToPixelsF(rect.X, DpiX);
			double newY = UnitConverter.LayoutUnitsToPixelsF(rect.Y, DpiY);
			SetPositionInPixels(elem, newX, newY);
		}
		protected MatrixTransform GetTransform(FrameworkElement elem) {
			MatrixTransform transform = elem.RenderTransform as MatrixTransform;
			if (transform.Matrix.IsIdentity) {
				transform = new MatrixTransform();
				elem.RenderTransform = transform;
			}
#if !SL
			else if (transform.IsFrozen) {
				transform = transform.Clone();
				elem.RenderTransform = transform;
			}
#endif
			return transform;
		}
		protected internal void SetMatrixTransform(MatrixTransform transform, double scaleX) {
			System.Windows.Media.Matrix matrix = transform.Matrix;
			SetMatrixTransform(transform, scaleX, matrix.OffsetX, matrix.OffsetY);
		}
		protected internal void SetMatrixTransform(MatrixTransform transform, double offsetX, double offsetY) {
			System.Windows.Media.Matrix maxtix = new System.Windows.Media.Matrix();
			SetMatrixTransform(transform, maxtix.M11, offsetX, offsetY);
		}
		protected internal void SetMatrixTransform(MatrixTransform transform, double scaleX, double offsetX, double offsetY) {
			if (transformStack.Count > 0) {
				TransformMatrix matrix = transformStack.Peek().Clone();
				matrix.Translate((float)offsetX, (float)offsetY);
				matrix.Scale((float)scaleX, 1);
				transform.Matrix = ToPlatformMatrix(matrix);
			}
			else
				transform.Matrix = new System.Windows.Media.Matrix(scaleX, 0, 0, 1, offsetX, offsetY);
		}
		public static MatrixTransform CreateMatrixTransform(TransformMatrix matrix) {
			System.Windows.Media.Matrix platformMatrix = XpfPainter.ToPlatformMatrix(matrix);
			MatrixTransform result = new MatrixTransform();
			result.Matrix = platformMatrix;
			return result;
		}
		public static System.Windows.Media.Matrix ToPlatformMatrix(TransformMatrix matrix) {
#if SL
			return new System.Windows.Media.Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY);
#else
			float[] elements = matrix.Elements;
			return new System.Windows.Media.Matrix(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
#endif
		}
		public override void FillRectangle(PlatformIndependentColor color, System.Drawing.Rectangle bounds) {
			FillRectangleCore(new SolidColorBrush(XpfTypeConverter.ToPlatformColor(color)), bounds);
		}
		public override void FillRectangle(PlatformIndependentBrush brush, System.Drawing.Rectangle bounds) {
			FillRectangleCore(XpfTypeConverter.ToPlatformBrush(brush), bounds);
		}
		public override void FillEllipse(PlatformIndependentBrush brush, System.Drawing.Rectangle bounds) {
			Ellipse ellipse = CreateEllipse();
			ellipse.Fill = XpfTypeConverter.ToPlatformBrush(brush);
			SetPosition(ellipse, bounds);
			SetWidthHeight(ellipse, bounds.Width, bounds.Height);
		}
		public override void DrawEllipse(PlatformIndependentPen pen, System.Drawing.Rectangle bounds) {
			Ellipse ellipse = CreateEllipse();
			ellipse.Fill = null;
			int additionalSize = (int)pen.Width / 2;
			bounds.Inflate(additionalSize, additionalSize);
			SetPosition(ellipse, bounds);
			SetWidthHeight(ellipse, bounds.Width, bounds.Height);
			ApplyPen(ellipse, pen);
		}
		public override void ExcludeCellBounds(System.Drawing.Rectangle rect, System.Drawing.Rectangle rowBounds) {
			double x = Math.Max(UnitConverter.LayoutUnitsToPixelsF(rect.X - rowBounds.X, DpiX), 1);
			double y = Math.Max(UnitConverter.LayoutUnitsToPixelsF(rect.Y - rowBounds.Y, DpiY), 1);
			double width = Math.Max(UnitConverter.LayoutUnitsToPixelsF(rect.Width, DpiX), 1);
			double height = Math.Max(UnitConverter.LayoutUnitsToPixelsF(rect.Height, DpiY), 1);
			RectangleGeometry drawingBounds = new RectangleGeometry();
			drawingBounds.Rect = new System.Windows.Rect(x, y, width, height);
			ExcludeRectangles.Add(drawingBounds);
		}
		public override void ResetCellBoundsClip() {
			ExcludeRectangles.Clear();
		}
		protected internal virtual void FillRectangleCore(PlatformBrush brush, System.Drawing.Rectangle bounds) {
			System.Windows.Shapes.Rectangle rect = CreateRectangle();
			rect.Fill = brush;
			SetPosition(rect, bounds);
			SetWidthHeight(rect, bounds.Width, bounds.Height);
			Matrix matrix = new Matrix();
			matrix.Translate(UnitConverter.LayoutUnitsToPixelsF(bounds.X, DpiX), UnitConverter.LayoutUnitsToPixelsF(bounds.Y, DpiY));
			SetClipBounds(rect, new MatrixTransform(matrix));
			if (ExcludeRectangles.Count > 0) {
				GeometryGroup excludeGroup = new GeometryGroup();
				RectangleGeometry totalDrawingBounds = new RectangleGeometry();
				totalDrawingBounds.Rect = new System.Windows.Rect(0, 0, rect.Width, rect.Height);
				excludeGroup.Children.Add(totalDrawingBounds);
				for (int i = 0; i < ExcludeRectangles.Count; i++) {
					excludeGroup.Children.Add(ExcludeRectangles[i]);
				}
				rect.Clip = excludeGroup;
			}
		}
		public override void DrawRectangle(PlatformIndependentPen pen, System.Drawing.Rectangle bounds) {
			System.Windows.Shapes.Rectangle path = CreateRectangle();
			int additionalSize = (int)pen.Width / 2;
			bounds.Inflate(additionalSize, additionalSize);
			SetPosition(path, bounds);
			SetWidthHeight(path, bounds.Width, bounds.Height);
			path.Fill = null;
			ApplyPen(path, pen);
			Matrix matrix = new Matrix();
			matrix.Translate(UnitConverter.LayoutUnitsToPixelsF(bounds.X, DpiX), UnitConverter.LayoutUnitsToPixelsF(bounds.Y, DpiY));
			SetClipBounds(path, new MatrixTransform(matrix));
		}
		public override void DrawString(string text, FontInfo fontInfo, System.Drawing.Rectangle rectangle) {
			double newX = UnitConverter.LayoutUnitsToPixelsF(rectangle.X, DpiX);
			double newY = UnitConverter.LayoutUnitsToPixelsF(rectangle.Y, DpiY);
			DrawString(text, fontInfo, TextForeColor, newX, newY);
		}
		public override void DrawString(string text, FontInfo fontInfo, System.Drawing.Rectangle rectangle, StringFormat stringFormat) {
			double newX = UnitConverter.LayoutUnitsToPixelsF(rectangle.X, DpiX);
			double newY = UnitConverter.LayoutUnitsToPixelsF(rectangle.Y, DpiY);
			DrawString(text, fontInfo, TextForeColor, newX, newY);
		}
		public void DrawString(string text, FontInfo fontInfo, PlatformIndependentColor foreColor, double x, double y) {
			TextBlock textBlock = CreateTextBlock(fontInfo);
			DrawStringCore(textBlock, text, fontInfo, foreColor);
			SetPositionInPixels(textBlock, x, y);
			Matrix matrix = new Matrix();
			matrix.Translate(x, y);
			SetClipBounds(textBlock, new MatrixTransform(matrix));
		}
		void SetClipBounds(UIElement element, MatrixTransform transform) {
			if (!UseClipBounds)
				return;
			if (!IsInfinityClipBounds) {
				try {
					Rect clipBounds = GetElementClipBounds(transform);
					RectangleGeometry clipGeometry = element.Clip as RectangleGeometry;
					if (clipGeometry != null) {
						Rect oldClipBounds = clipGeometry.Rect;
						if (!AreEqualOrClose(clipBounds, oldClipBounds))
							clipGeometry.Rect = clipBounds;
					}
					else {
						clipGeometry = new RectangleGeometry();
						clipGeometry.Rect = clipBounds;
						element.Clip = clipGeometry;
					}
				}
				catch {
					element.Clip = null;
				}
			}
			else
				element.Clip = null;
		}
		bool AreEqualOrClose(Rect rect1, Rect rect2) {
			return AreEqualOrClose(rect1.X, rect2.X) && AreEqualOrClose(rect1.Y, rect2.Y) && AreEqualOrClose(rect1.Width, rect2.Width) && AreEqualOrClose(rect1.Height, rect2.Height);
		}
		bool AreEqualOrClose(double value1, double value2) {
			if (value1 == value2)
				return true;
			double tolerance = ((Math.Abs(value1) + Math.Abs(value2)) + 10.0) * 2.2204460492503131E-16;
			double diff = value1 - value2;
			return ((-tolerance < diff) && (tolerance > diff));
		}
		void SetClipBounds(FrameworkElement element) {
			SetClipBounds(element, GetTransform(element));
		}
		Rect GetElementClipBounds(MatrixTransform transform) {
			float x = UnitConverter.LayoutUnitsToPixelsF(ClipBounds.X, DpiX);
			float y = UnitConverter.LayoutUnitsToPixelsF(ClipBounds.Y, DpiY);
			float width = UnitConverter.LayoutUnitsToPixelsF(ClipBounds.Width, DpiX);
			float height = UnitConverter.LayoutUnitsToPixelsF(ClipBounds.Height, DpiY);
			return transform.Inverse.TransformBounds(new Rect(x, y, width, height));
		}
#if !SL
		public override void DrawString(string text, PlatformIndependentBrush brush, Font font, float x, float y) {
			throw new NotImplementedException(); 
		}
		public override SizeF MeasureString(string text, Font font) {
			throw new NotImplementedException(); 
		}
#endif
		public void DrawVString(string text, FontInfo fontInfo, PlatformIndependentColor foreColor, double x, double y) {
			TextBlock textBlock = CreateTextBlock(fontInfo);
			DrawStringCore(textBlock, text, fontInfo, foreColor);
			MatrixTransform transform = GetTransform(textBlock);
			SetMatrixTransform(transform, 1, x, y);
			if (textBlock.HorizontalAlignment != HorizontalAlignment.Left)
				textBlock.HorizontalAlignment = HorizontalAlignment.Left;
			if (textBlock.VerticalAlignment != System.Windows.VerticalAlignment.Top)
				textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Top;
			System.Windows.Media.Matrix matrix = transform.Matrix;
			transform.Matrix = new System.Windows.Media.Matrix(matrix.M12, -matrix.M11, matrix.M22, -matrix.M21, matrix.OffsetX, matrix.OffsetY);
		}
		public override void DrawSpacesString(string text, FontInfo fontInfo, System.Drawing.Rectangle rectangle) {
			DrawString(text, fontInfo, TextForeColor, rectangle);
		}
		public override void DrawImage(OfficeImage img, System.Drawing.Rectangle bounds) {
			DrawImage(img, bounds, XtraPrinting.ImageSizeMode.Normal);
		}
		void ApplyImageBrush(System.Windows.Shapes.Rectangle rectangle, OfficeImage img, System.Drawing.Rectangle bounds, XtraPrinting.ImageSizeMode sizing) {
#if !SL
			RenderOptions.SetBitmapScalingMode(rectangle, BitmapScalingMode.HighQuality);
#endif
			if (sizing == XtraPrinting.ImageSizeMode.Tile) {
				VisualBrush brush = new VisualBrush(CreatePlatformImage(img));
				brush.TileMode = TileMode.Tile;
				brush.ViewportUnits = BrushMappingMode.Absolute;
				double x = UnitConverter.LayoutUnitsToPixelsF(bounds.X, DpiX);
				double y = UnitConverter.LayoutUnitsToPixelsF(bounds.Y, DpiY);
				brush.Viewport = new Rect(x, y, img.SizeInPixels.Width, img.SizeInPixels.Height);
				rectangle.Fill = brush;
			}
			else if (!TryUseCachedImageBrush(rectangle, img))
				ApplyImageBrushCore(rectangle, img);
		}
		bool TryUseCachedImageBrush(System.Windows.Shapes.Rectangle rectangle, OfficeImage img) {
			if (ImageSourceCache == null)
				return false;
			ImageSource source;
			if (!ImageSourceCache.TryGetValue(img.NativeImage, out source))
				return false;
			if (source == null)
				return false;
			ImageBrush brush = new ImageBrush();
			brush.ImageSource = source;
			brush.Stretch = Stretch.Fill;
			rectangle.Fill = brush;
			return true;
		}
		void ApplyImageBrushCore(System.Windows.Shapes.Rectangle rectangle, OfficeImage img) {
			ImageBrush brush = new ImageBrush();
			PlatformImage platformImage = CreatePlatformImage(img);
			if (platformImage != null) {
				brush.ImageSource = platformImage.Source;
				if (ImageSourceCache != null)
					ImageSourceCache[img.NativeImage] = platformImage.Source;
			}
			brush.Stretch = Stretch.Fill;
			rectangle.Fill = brush;
		}
		public override void DrawImage(OfficeImage img, System.Drawing.Rectangle bounds, System.Drawing.Size imgActualSizeInLayoutUnits, XtraPrinting.ImageSizeMode sizing) {
			RectangleF oldClipBounds = ClipBounds;
			RectangleF newClipBounds = (IsInfinityClipBounds) ? bounds : RectangleF.Intersect(oldClipBounds, bounds);
			ClipBounds = newClipBounds;
			try {
				System.Drawing.Rectangle imgRect = System.Drawing.Rectangle.Round(ImageTool.CalculateImageRectCore(bounds, imgActualSizeInLayoutUnits, sizing));
				DrawImage(img, imgRect, sizing);
			}
			finally {
				ClipBounds = oldClipBounds;
			}
		}
		void DrawImage(OfficeImage img, System.Drawing.Rectangle bounds, XtraPrinting.ImageSizeMode sizing) {
			System.Windows.Shapes.Rectangle rectangle = CreateRectangle();
			ApplyImageBrush(rectangle, img, bounds, sizing);
			if (!IsInfinityClipBounds) {
				double newX = UnitConverter.LayoutUnitsToPixelsF(ClipBounds.X - bounds.X, DpiX);
				double newY = UnitConverter.LayoutUnitsToPixelsF(ClipBounds.Y - bounds.Y, DpiY);
				double newW = Math.Max(UnitConverter.LayoutUnitsToPixelsF(ClipBounds.Width, DpiX), 1);
				double newH = Math.Max(UnitConverter.LayoutUnitsToPixelsF(ClipBounds.Height, DpiY), 1);
				rectangle.Clip = new RectangleGeometry() { Rect = new Rect(newX, newY, newW, newH) };
			}
			SetPosition(rectangle, bounds);
			SetWidthHeight(rectangle, bounds.Width, bounds.Height);
		}
		internal static PlatformImage CreatePlatformImage(OfficeImage img) {
			try {
				try {
					return CreatePlatformImageCore(img);
				}
				catch (NotSupportedException) {
					return CreatePlatformImageCore(CreateBitmapImage(img));
				}
			}
			catch {
				return null;
			}
		}
		static PlatformImage CreatePlatformImageCore(OfficeImage img) {
#if SL
			return img.NativeImage;
#else
			BitmapImage imageSource = new BitmapImage();
			imageSource.BeginInit();
			byte[] bytes = img.GetImageBytesSafe(img.RawFormat);
			imageSource.StreamSource = new System.IO.MemoryStream(bytes);
			imageSource.EndInit();
			imageSource.Freeze();
			PlatformImage image = new PlatformImage();
			image.Source = imageSource;
			return image;
#endif
		}
		static OfficeImage CreateBitmapImage(OfficeImage img) {
#if !SL
			System.Drawing.Size size = img.SizeInPixels;
			size.Width = Math.Max(1, size.Width);
			size.Height = Math.Max(1, size.Height);
			Bitmap bitmap = new Bitmap(size.Width, size.Height);
			using (Graphics graphics = Graphics.FromImage(bitmap)) {
				graphics.Clear(DXColor.Transparent);
				graphics.DrawImage(img.NativeImage, 0, 0);
			}
			return OfficeImageWin.CreateImage(bitmap);
#else
			System.Drawing.Size size = img.SizeInPixels;
			return UriBasedOfficeImageBase.CreatePlaceHolder(null, Math.Max(1, size.Width), Math.Max(1, size.Height));
#endif
		}
		public void DrawHorizontalLine(PlatformIndependentPen pen, double x1, double y, double x2) {
			double newX1 = Math.Round(UnitConverter.LayoutUnitsToPixelsF((float)x1, (float)DpiX));
			double newX2 = Math.Round(UnitConverter.LayoutUnitsToPixelsF((float)x2, (float)DpiX));
			double newY = Math.Round(UnitConverter.LayoutUnitsToPixelsF((float)y, (float)DpiY));
			Line line = CreateLine();
#if !SL
			line.SnapsToDevicePixels = true;
#endif
			line.Width = Math.Abs(newX2 - newX1);
			line.X2 = Math.Abs(newX2 - newX1);
			if (line.Width < 2)
				line.X2 = line.Width = 2;
			SetPositionInPixels(line, newX1, newY);
			line.Stretch = Stretch.Fill;
			ApplyPen(line, pen);
			line.Height = line.StrokeThickness;
			SetClipBounds(line);
		}
		protected internal virtual void ApplyPen(System.Windows.Shapes.Shape line, PlatformIndependentPen pen) {
			double thickness = Math.Ceiling(UnitConverter.LayoutUnitsToPixelsF((float)pen.Width, (float)DpiY));
			if (thickness == 0)
				thickness = 1;
			line.Stroke = new SolidColorBrush(XpfTypeConverter.ToPlatformColor(pen.Color));
			line.StrokeThickness = thickness;
#if !SL
			if (pen.DashStyle == System.Drawing.Drawing2D.DashStyle.Custom)
#endif
				if (pen.DashPattern != null) {
					DoubleCollection dc = new DoubleCollection();
					foreach (var p in pen.DashPattern) {
						dc.Add((double)p);
					}
					line.StrokeDashArray = dc;
				}
		}
		public override void DrawLine(PlatformIndependentPen pen, float x1, float y1, float x2, float y2) {
			if (Math.Abs(y1 - y2) < 1) {
				int additionalOffset = (int)pen.Width / 2;
				x1 -= additionalOffset;
				x2 += additionalOffset;
				y1 -= additionalOffset;
				DrawHorizontalLine(pen, (double)x1, (double)y1, (double)x2);
				return;
			}
			double newX1 = Math.Round(UnitConverter.LayoutUnitsToPixelsF(x1, (float)DpiX));
			double newX2 = Math.Round(UnitConverter.LayoutUnitsToPixelsF(x2, (float)DpiX));
			double newY1 = Math.Round(UnitConverter.LayoutUnitsToPixelsF(y1, (float)DpiY));
			double newY2 = Math.Round(UnitConverter.LayoutUnitsToPixelsF(y2, (float)DpiY));
			Line line = CreateLine();
			if (line.X1 != newX1 || line.X2 != newX2 || line.Y1 != newY1 || line.Y2 != newY2) {
				line.X1 = newX1;
				line.X2 = newX2;
				line.Y1 = newY1;
				line.Y2 = newY2;
			}
			SetPosition(line, new System.Drawing.Rectangle());
			ApplyPen(line, pen);
			SetClipBounds(line);
		}
		protected internal virtual Path CreatePolyline(PointF[] pointF, bool isClosed, bool isFilled) {
			Path line = CreatePolyline();
			PathGeometry pg = new PathGeometry();
			line.Data = pg;
			PathFigure pf = new PathFigure();
			pg.Figures.Add(pf);
			pf.IsClosed = isClosed;
			pf.IsFilled = isFilled;
			SetPosition(line, new System.Drawing.Rectangle());
			foreach (var point in pointF) {
				double x = Math.Round(UnitConverter.LayoutUnitsToPixelsF(point.X, (float)DpiX));
				double y = Math.Round(UnitConverter.LayoutUnitsToPixelsF(point.Y, (float)DpiX));
				pf.Segments.Add(new LineSegment() { Point = new System.Windows.Point(x, y) });
			}
			pf.StartPoint = ((LineSegment)pf.Segments[0]).Point;
			return line;
		}
		public override void DrawLines(PlatformIndependentPen pen, PointF[] pointF) {
			Path line = CreatePolyline(pointF, false, false);
			ApplyPen(line, pen);
			SetClipBounds(line);
		}
		public override void FillPolygon(PlatformIndependentBrush brush, PointF[] points) {
		}
		protected virtual void ApplyTextBoxText(TextBlock textBlock, string text) {
			if (textBlock.Text != text)
				textBlock.Text = text;
		}
		protected virtual void ApplyTextBoxForeColor(TextBlock textBlock, PlatformIndependentColor foreColor) {
			PlatformBrush textBlockForeground = textBlock.Foreground;
			if (textBlockForeground == null || ((SolidColorBrush)textBlockForeground).Color != XpfTypeConverter.ToPlatformColor(foreColor)) {
				textBlock.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(XpfTypeConverter.ToPlatformColor(foreColor))); 
			}
		}
#if SL
		protected virtual void ApplyTextBoxFontInfo(TextBlock textBlock, FontInfo fontInfo) {
			PrecalculatedMetricsFontInfo precalculatedMetricsFontInfo = (PrecalculatedMetricsFontInfo)fontInfo;
			Font font = precalculatedMetricsFontInfo.Font;
			bool isPredefined = font.FontSource == null;
			FontWeight fontWeight = isPredefined && font.Bold ? FontWeights.Bold : FontWeights.Normal;
			System.Windows.FontStyle fontStyle = font.Italic ? FontStyles.Italic : FontStyles.Normal;
			double fontSize = Units.DocumentsToPixelsF(Units.PointsToDocumentsF(font.SizeInPoints), DocumentModel.DpiY);
			if (textBlock.FontFamily.Source != font.FontFamily.Source)
				textBlock.FontFamily = font.FontFamily;
			if (textBlock.FontSource != font.FontSource)
				textBlock.FontSource = font.FontSource;
			if (textBlock.FontStyle != fontStyle)
				textBlock.FontStyle = fontStyle;
			if (textBlock.FontWeight != fontWeight)
				textBlock.FontWeight = fontWeight;
			if (textBlock.FontSize != fontSize)
				textBlock.FontSize = fontSize;
			if (font.SimulateBold) {
				MatrixTransform transform = GetTransform(textBlock);
				double normalWidth = textBlock.ActualWidth;
				textBlock.FontWeight = FontWeights.Bold;
				double styledWidth = textBlock.ActualWidth;
				SetMatrixTransform(transform, normalWidth / styledWidth);
				textBlock.Tag = true; 
			}
			else {
				if (Object.Equals(textBlock.Tag, true)) { 
					MatrixTransform transform = GetTransform(textBlock);
					SetMatrixTransform(transform, 1);
					textBlock.ClearValue(FrameworkElement.TagProperty);
				}
			}
		}
		protected virtual void DrawStringCore(TextBlock textBlock, string text, FontInfo fontInfo, PlatformIndependentColor foreColor) {
			ApplyTextBoxText(textBlock, text);
			ApplyTextBoxForeColor(textBlock, foreColor);
			ApplyTextBoxFontInfo(textBlock, fontInfo);
		}
#else
		protected virtual void DrawStringCore(TextBlock textBlock, string text, FontInfo fontInfo, PlatformIndependentColor foreColor) {
			WpfFontInfo wpfFontInfo = (WpfFontInfo)fontInfo;
			double fontSize = Units.DocumentsToPixelsF(Units.PointsToDocumentsF(wpfFontInfo.SizeInPoints), DpiY);
			if (textBlock.Text != text)
				textBlock.Text = text;
			textBlock.FontFamily = wpfFontInfo.FontFamily;
			textBlock.FontStyle = wpfFontInfo.Typeface.Style;
			textBlock.FontWeight = wpfFontInfo.Typeface.Weight;
			textBlock.FontStretch = wpfFontInfo.Typeface.Stretch;
			if (textBlock.FontSize != fontSize)
				textBlock.FontSize = fontSize;
			if (textBlock.Foreground == null || ((SolidColorBrush)textBlock.Foreground).Color != XpfTypeConverter.ToPlatformColor(foreColor)
#if !SL
 || DependencyPropertyHelper.GetValueSource(textBlock, TextBlock.ForegroundProperty).BaseValueSource != BaseValueSource.Local
#endif
)
				textBlock.Foreground = new SolidColorBrush(XpfTypeConverter.ToPlatformColor(foreColor));
		}
#endif
		protected override PointF[] TransformToPixels(PointF[] points) {
			int count = points.Length;
			for (int i = 0; i < count; i++) {
				float x = UnitConverter.LayoutUnitsToPixelsF(points[i].X, DpiX) * ZoomFactor;
				float y = UnitConverter.LayoutUnitsToPixelsF(points[i].Y, DpiY) * ZoomFactor;
				points[i] = new PointF(x, y);
			}
			return points;
		}
		protected override PointF[] TransformToLayoutUnits(PointF[] points) {
			int count = points.Length;
			for (int i = 0; i < count; i++) {
				float x = UnitConverter.PixelsToLayoutUnitsF(points[i].X / ZoomFactor, DpiX);
				float y = UnitConverter.PixelsToLayoutUnitsF(points[i].Y / ZoomFactor, DpiY);
				points[i] = new PointF(x, y);
			}
			return points;
		}
		public override void PushRotationTransform(System.Drawing.Point center, float angleInDegrees) {
			TransformMatrix transform = new TransformMatrix();
			transform.RotateAt(angleInDegrees, new PointF(UnitConverter.LayoutUnitsToPixelsF(center.X, DpiX), UnitConverter.LayoutUnitsToPixelsF(center.Y, DpiY)));
			transformStack.Push(transform);
		}
		public override void PopTransform() {
			transformStack.Pop();
		}
		public override void PushSmoothingMode(bool highQuality) {
		}
		public override void PopSmoothingMode() {
		}
		public override void PushPixelOffsetMode(bool highQualtity) {
		}
		public override void PopPixelOffsetMode() {
		}
		protected internal abstract T CreateObject<T>() where T : FrameworkElement, new();
		public abstract void DrawControl<T>(RectangleF boundsF) where T : FrameworkElement, new();
		protected abstract Line CreateLine();
		protected abstract System.Windows.Shapes.Rectangle CreateRectangle();
		protected abstract Path CreatePolyline();
		protected abstract TextBlock CreateTextBlock(FontInfo fontInfo);
		protected abstract Ellipse CreateEllipse();
	}
	#endregion
	#region XpfSlowPainter
	public class XpfSlowPainter : XpfPainter {
		readonly UIElementCollection surfaceChildren;
		public XpfSlowPainter(DocumentLayoutUnitConverter unitConverter, UIElementCollection surfaceChildren)
			: base(unitConverter) {
			Guard.ArgumentNotNull(surfaceChildren, "surfaceChildren");
			this.surfaceChildren = surfaceChildren;
		}
		public UIElementCollection SurfaceChildren { get { return surfaceChildren; } }
		protected override Ellipse CreateEllipse() {
			Ellipse result = new Ellipse();
			SurfaceChildren.Add(result);
			return result;
		}
		protected override System.Windows.Shapes.Rectangle CreateRectangle() {
			System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
			SurfaceChildren.Add(rectangle);
			return rectangle;
		}
		protected override Line CreateLine() {
			Line line = new Line();
			SurfaceChildren.Add(line);
			return line;
		}
		protected override Path CreatePolyline() {
			Path line = new Path();
			SurfaceChildren.Add(line);
			return line;
		}
		protected override TextBlock CreateTextBlock(FontInfo fontInfo) {
			TextBlock textBlock = new TextBlock();
			SurfaceChildren.Add(textBlock);
			return textBlock;
		}
#if !SL
		public override void DrawBrick(XtraPrinting.PrintingSystemBase ps, XtraPrinting.VisualBrick brick, System.Drawing.Rectangle bounds) {
			throw new NotImplementedException();
		}
#endif
		protected internal override T CreateObject<T>() {
			return new T();
		}
		public override void DrawControl<T>(RectangleF boundsF) {
			FrameworkElement control = CreateObject<T>();
			SetPositionInPixels(control, boundsF.X, boundsF.Y);
			SetWidthHeightInPixels(control, boundsF.Width, boundsF.Height);
			SurfaceChildren.Add(control);
		}
	}
	#endregion
	#region XpfDrawingSurface
	public class XpfDrawingSurface : IDrawingSurface {
		readonly UIElementCollection collection;
		readonly DevExpress.XtraRichEdit.Layout.Export.CustomMarkVisualInfoCollection customMarkVisualInfoCollection;
		int customPaintingCount;
		List<int> customPaintedObjectsIndices;
		public XpfDrawingSurface(UIElementCollection collection, List<int> customPaintedObjectsIndices) {
			Guard.ArgumentNotNull(collection, "collection");
			this.collection = collection;
			this.customMarkVisualInfoCollection = new Layout.Export.CustomMarkVisualInfoCollection();
			this.customPaintedObjectsIndices = customPaintedObjectsIndices != null ? customPaintedObjectsIndices : new List<int>();
			Position = 0;
		}
		public XpfDrawingSurface(UIElementCollection collection)
			: this(collection, null) {
		}
		public UIElementCollection Collection { get { return collection; } }
		public DevExpress.XtraRichEdit.Layout.Export.CustomMarkVisualInfoCollection CustomMarkVisualInfoCollection { get { return customMarkVisualInfoCollection; } }
		protected internal virtual int CollectionSize { get { return collection.Count; } }
		internal List<int> CustomPaintedObjectsIndices { get { return customPaintedObjectsIndices; } }
		public int Position { get; set; }
		public void BeginCustomPaint() {
			this.customPaintingCount++;
		}
		public void EndCustomPaint() {
			this.customPaintingCount--;
		}
		public virtual void BeginPaint() {
			Position = 0;
			customMarkVisualInfoCollection.Clear();
			ClearCustomObjects();
		}
		void ClearCustomObjects() {
			if (CollectionSize <= 0)
				return;
			for (int i = CustomPaintedObjectsIndices.Count - 1; i >= 0; i--) {
				Collection.RemoveAt(CustomPaintedObjectsIndices[i]);
			}
			CustomPaintedObjectsIndices.Clear();
		}
		public void EndPaint() {
			for (int i = CollectionSize - 1; i >= Position; i--)
				RemoveAt(i);
		}
		protected internal virtual void InsertAt(int index, FrameworkElement element) {
			collection.Insert(index, element);
			if (this.customPaintingCount > 0)
				CustomPaintedObjectsIndices.Add(index);
		}
		protected internal virtual void RemoveAt(int index) {
			if (!this.customPaintedObjectsIndices.Contains(index))
				collection.RemoveAt(index);
		}
		public T CreateObject<T>() where T : FrameworkElement, new() {
			UIElement elem = (Position < CollectionSize && this.customPaintingCount <= 0) ? Collection[Position] : null;
			T instance = elem as T;
			if (instance != null) {
				Position++;
				return instance;
			}
			if (elem != null)
				RemoveAt(Position);
			T obj = new T();
			InsertAt(Position, obj);
			Position++;
			return obj;
		}
		public PlatformImage AppendExistingImage(PlatformImage image) {
			UIElement elem = (Position < CollectionSize) ? Collection[Position] : null;
			if (elem == image)
				Position++;
			else {
				InsertAt(Position, image);
				Position++;
			}
			return image;
		}
	}
	#endregion
	#region XpfCachedDrawingSurface
	public class XpfCachedDrawingSurface : XpfDrawingSurface {
		int collectionSize;
		readonly List<object> cache;
		public XpfCachedDrawingSurface(UIElementCollection collection)
			: base(collection) {
			this.cache = new List<object>();
		}
		public IList<object> Cache { get { return cache; } }
		protected internal override int CollectionSize { get { return collectionSize; } }
		public override void BeginPaint() {
			base.BeginPaint();
			this.collectionSize = cache.Count;
			System.Diagnostics.Debug.Assert(cache.Count == Collection.Count);
		}
		protected internal override void InsertAt(int index, FrameworkElement element) {
			base.InsertAt(index, element);
			cache.Insert(index, null);
			collectionSize++;
		}
		protected internal override void RemoveAt(int index) {
			base.RemoveAt(index);
			cache.RemoveAt(index);
			collectionSize--;
		}
		public object GetLastCreatedObjectCache() {
			return cache[Position - 1];
		}
		public void SetLastCreatedObjectCache(object cacheItem) {
			cache[Position - 1] = cacheItem;
		}
	}
	#endregion
	#region XpfPainterOverwrite
	public class XpfPainterOverwrite : XpfPainter, DevExpress.XtraRichEdit.API.Layout.ISupportCustomPainting {
		readonly XpfDrawingSurface surface;
		public XpfPainterOverwrite(DocumentLayoutUnitConverter unitConverter, XpfDrawingSurface surface)
			: base(unitConverter) {
			Guard.ArgumentNotNull(surface, "surface");
			this.surface = surface;
			Surface.BeginPaint();
		}
		public XpfDrawingSurface Surface { get { return surface; } }
		void DevExpress.XtraRichEdit.API.Layout.ISupportCustomPainting.BeginCustomPaint() {
			Surface.BeginCustomPaint();
		}
		void DevExpress.XtraRichEdit.API.Layout.ISupportCustomPainting.EndCustomPaint() {
			Surface.EndCustomPaint();
		}
		public override void FinishPaint() {
			Surface.EndPaint();
		}
		protected internal override T CreateObject<T>() {
			T result = Surface.CreateObject<T>();
			return result;
		}
		protected override Line CreateLine() {
			return CreateObject<Line>();
		}
		protected override Path CreatePolyline() {
			return CreateObject<Path>();
		}
		protected override Ellipse CreateEllipse() {
			return CreateObject<Ellipse>();
		}
		protected override System.Windows.Shapes.Rectangle CreateRectangle() {
			System.Windows.Shapes.Rectangle rect = CreateObject<System.Windows.Shapes.Rectangle>();
			if (rect.Clip != null)
				rect.Clip = null;
			return rect;
		}
		protected override TextBlock CreateTextBlock(FontInfo fontInfo) {
			TextBlock textBlock = CreateObject<TextBlock>();
			if (!UseClipBounds)
				textBlock.Clip = null;
			return textBlock;
		}
		public override void DrawControl<T>(RectangleF boundsF) {
			T control = CreateObject<T>();
			SetPositionInPixels(control, boundsF.X, boundsF.Y);
			SetWidthHeightInPixels(control, boundsF.Width, boundsF.Height);
		}
#if !SL
		public override void DrawBrick(XtraPrinting.PrintingSystemBase ps, XtraPrinting.VisualBrick brick, System.Drawing.Rectangle bounds) {
			throw new NotImplementedException();
		}
#endif
		MatrixTransform customMarkTransform;
		public void DrawCustomMark(CustomMark customMark, System.Drawing.Rectangle bounds) {
			if (customMarkTransform == null) {
				customMarkTransform = new MatrixTransform();
			}
			float x = UnitConverter.LayoutUnitsToPixelsF(bounds.X, (float)DpiX);
			float y = UnitConverter.LayoutUnitsToPixelsF(bounds.Y, (float)DpiY);
			float width = UnitConverter.LayoutUnitsToPixelsF(bounds.Width, (float)DpiX);
			float height = UnitConverter.LayoutUnitsToPixelsF(bounds.Height, (float)DpiY);
			SetMatrixTransform(customMarkTransform, x, y);
			Rect r = customMarkTransform.TransformBounds(new Rect(0, 0, width, height));
			surface.CustomMarkVisualInfoCollection.Add(new Layout.Export.CustomMarkVisualInfo(customMark, new System.Drawing.Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height)));
		}
	}
	#endregion
	#region TextBlockCacheItem
	public class TextBlockCacheItem {
		string text;
		FontInfo fontInfo;
		PlatformIndependentColor foreColor;
		public TextBlockCacheItem(string text, FontInfo fontInfo, PlatformIndependentColor foreColor) {
			this.text = text;
			this.fontInfo = fontInfo;
			this.foreColor = foreColor;
		}
		public string Text { get { return text; } set { text = value; } }
		public FontInfo FontInfo { get { return fontInfo; } set { fontInfo = value; } }
		public PlatformIndependentColor ForeColor { get { return foreColor; } set { foreColor = value; } }
	}
	#endregion
	#region XpfCachedPainterOverwrite
	public class XpfCachedPainterOverwrite : XpfPainterOverwrite {
		public XpfCachedPainterOverwrite(DocumentLayoutUnitConverter unitConverter, XpfCachedDrawingSurface surface)
			: base(unitConverter, surface) {
		}
#if SL
		protected override void DrawStringCore(TextBlock textBlock, string text, FontInfo fontInfo, PlatformIndependentColor foreColor) {
			XpfCachedDrawingSurface surface = (XpfCachedDrawingSurface)Surface;
			TextBlockCacheItem cacheItem = surface.GetLastCreatedObjectCache() as TextBlockCacheItem;
			if (cacheItem != null) {
				if (cacheItem.Text != text) {
					textBlock.Text = text;
					cacheItem.Text = text;
				}
				if (cacheItem.ForeColor != foreColor) {
					ApplyTextBoxForeColor(textBlock, foreColor);
					cacheItem.ForeColor = foreColor;
				}
				if (cacheItem.FontInfo != fontInfo) {
					ApplyTextBoxFontInfo(textBlock, fontInfo);
					cacheItem.FontInfo = fontInfo;
				}
			}
			else {
				cacheItem = new TextBlockCacheItem(text, fontInfo, foreColor);
				surface.SetLastCreatedObjectCache(cacheItem);
				ApplyTextBoxText(textBlock, text);
				ApplyTextBoxForeColor(textBlock, foreColor);
				ApplyTextBoxFontInfo(textBlock, fontInfo);
			}
		}
#endif
	}
	#endregion
}
