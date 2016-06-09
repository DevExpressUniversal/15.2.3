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
using System.Security;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Interop;
namespace DevExpress.Pdf.Drawing {
	public enum PdfRenderMode { View, Print }
	public class PdfViewerCommandInterpreter : PdfRenderingCommandInterpreter {
		const double textAngleFactor = 1800 / Math.PI;
		const double halfPI = Math.PI / 2;
		const double quarterPI = Math.PI / 4;
		const double threeQuarterPI = Math.PI * 3 / 4;
		const double degreeToRadianFactor = Math.PI / 180;
		const int maxFontNameLength = 31;
		static readonly PointF[] imageDestinationPointsCorrected = new PointF[] { new PointF(-0.0008f, -0.0008f), new PointF(1.0008f, -0.0008f), new PointF(-0.0008f, 1.0008f) };
		static readonly object drawingContext = new object();
		public static void Draw(PdfDocumentState documentState, int pageIndex, float scale, PointF position, PdfRenderMode renderMode, Graphics graphics, Rectangle targetRectangle) {
			lock (drawingContext)
				using (Graphics g = Graphics.FromHwnd(IntPtr.Zero)) {
					BufferedGraphics bufferedGraphics;
					try {
						bufferedGraphics = BufferedGraphicsManager.Current.Allocate(g, targetRectangle);
					}
					catch {
						throw new PdfRenderingOutOfMemoryException();
					}
					using (bufferedGraphics) {
						Graphics gr = bufferedGraphics.Graphics;
						gr.ResetTransform();
						gr.Clear(Color.White);
						using (PdfViewerCommandInterpreter commandInterpreter = new PdfViewerCommandInterpreter(documentState, pageIndex, gr, scale, position, renderMode))
							commandInterpreter.DrawContent();
						try {
							bufferedGraphics.Render(graphics);
						}
						catch {
							throw new PdfRenderingOutOfMemoryException();
						}
					}
				}
		}
		public static Bitmap GetBitmap(PdfDocumentState documentState, int pageIndex, float scale, PdfRenderMode renderMode) {
			PdfPoint pageSize = documentState.GetPageSize(pageIndex);
			int pageWidth = Math.Max(1, Convert.ToInt32(pageSize.X * scale));
			int pageHeight = Math.Max(1, Convert.ToInt32(pageSize.Y * scale));
			Bitmap bitmap;
			try {
				bitmap = new Bitmap(pageWidth, pageHeight, PixelFormat.Format24bppRgb);
			}
			catch {
				throw new PdfRenderingOutOfMemoryException();
			}
			try {
				using (Graphics gr = Graphics.FromImage(bitmap))
					PdfViewerCommandInterpreter.Draw(documentState, pageIndex, scale, Point.Empty, renderMode, gr, new Rectangle(0, 0, pageWidth, pageHeight));
			}
			catch {
				bitmap.Dispose();
				throw;
			}
			return bitmap;
		}
		public static Bitmap GetBitmap(PdfDocumentState documentState, int pageIndex, int largestEdgeLength, PdfRenderMode renderMode) {
			PdfPoint pageSize = documentState.GetPageSize(pageIndex);
			return GetBitmap(documentState, pageIndex, largestEdgeLength / (float)Math.Max(pageSize.X, pageSize.Y), renderMode);
		}
		public static Bitmap GetTilingPatternKeyCellBitmap(PdfDocumentState documentState, int pageIndex, PdfTilingPattern pattern, Size bitmapSize, Size keyBitmapSize, PdfColor color) { 
			int keyBitmapWidth = keyBitmapSize.Width;
			int keyBitmapHeight = keyBitmapSize.Height;
			int bitmapWidth = bitmapSize.Width;
			int bitmapHeight = bitmapSize.Height;
			if (keyBitmapWidth == 0 || keyBitmapHeight == 0 || bitmapWidth == 0 || bitmapHeight == 0)
				return null;
			Bitmap bitmap = null;
			try {
				bitmap = new Bitmap(bitmapWidth, bitmapHeight);
				if (bitmapWidth >= keyBitmapWidth && bitmapHeight >= keyBitmapHeight) 
					DrawTillingPatternKeyCell(bitmap, documentState, pageIndex, pattern, keyBitmapSize, color);
				else 
					using (Bitmap keyCellBitmap = new Bitmap(keyBitmapWidth, keyBitmapHeight)) { 
						DrawTillingPatternKeyCell(keyCellBitmap, documentState, pageIndex, pattern, keyBitmapSize, color);
						using (Graphics graphics = Graphics.FromImage(bitmap)) { 
							graphics.Clear(Color.Transparent);
							double absoluteXStep = bitmapWidth;
							double absoluteYStep = bitmapHeight;
							int stepXRepeats = Convert.ToInt32(Math.Ceiling(keyBitmapWidth / absoluteXStep - 1));
							int stepYRepeats = Convert.ToInt32(Math.Ceiling(keyBitmapWidth / absoluteYStep - 1));
							float xStep = (float)(pattern.XStep < 0 ? -absoluteXStep : absoluteXStep);
							float yStep = (float)(pattern.YStep > 0 ? -absoluteYStep : absoluteYStep);
							int maxXStepCount = stepXRepeats * 2;
							int maxYStepCount = stepYRepeats * 2;
							float y = -yStep * stepYRepeats;
							for (int i = 0; i <= maxYStepCount; i++, y += yStep) { 
								float x = -xStep * stepXRepeats;
								for (int j = 0; j <= maxXStepCount; j++, x += xStep)  
									graphics.DrawImage(keyCellBitmap, new PointF(x, y));
							}
						}
					}
			}
			catch {
				bitmap.Dispose();
				throw;
			}
			return bitmap;
		}		
		public static double CalcAngle(PdfPoint startPoint, PdfPoint endPoint) {
			return Math.Atan2(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
		}
		static void DrawTillingPatternKeyCell(Bitmap bitmap, PdfDocumentState documentState, int pageIndex, PdfTilingPattern pattern, Size keyBitmapSize, PdfColor color) { 
			if (bitmap != null)
				using (Graphics graphics = Graphics.FromImage(bitmap)) { 
					graphics.Clear(Color.Transparent);
					using (PdfViewerCommandInterpreter commandInterpreter = new PdfViewerCommandInterpreter(documentState, pageIndex, graphics, keyBitmapSize, PdfRenderMode.View)) 
						commandInterpreter.DrawTilingPatternKeyCell(pattern, color);
				}
		}
		static double CalculateDistanceSquare(PointF point1, PointF point2) { 
			double dx = point2.X - point1.X;
			double dy = point2.Y - point1.Y;
			return dx * dx + dy * dy;
		}
		readonly PdfDocumentState documentState;
		readonly Graphics graphics;
		readonly PointF position;
		readonly PdfRenderMode renderMode;
		readonly PdfPageState pageState;
		readonly int width;
		readonly int height;
		readonly GraphicsState graphicsState;
		readonly Region initialClipRegion;
		readonly double offsetX;
		readonly double offsetY;
		readonly double rotateAngle;
		readonly RectangleF pageClippingRect;
		readonly Stack<Region> clipRegionStack = new Stack<Region>();
		readonly Stack<PdfGdiBrush> strokingBrushStack = new Stack<PdfGdiBrush>();
		readonly Stack<PdfGdiBrush> nonStrokingBrushStack = new Stack<PdfGdiBrush>();
		Region currentClipRegion;
		Region clipRegion;
		PdfGdiBrush currentStrokingBrush;
		PdfGdiBrush currentNonStrokingBrush;
		Graphics hdcGraphics;
		IntPtr hdc;
		IntPtr defaultFont;
		IntPtr clipRegionHandle;
		GraphicsMode defaultGraphicsMode;
		TextAlign defaultTextAlign;
		BackgroundMode defaultBackgroundMode;
		int defaultTextColor;
		bool useEmbeddedFontEncoding;
		public PdfDocumentState DocumentState { get { return documentState; } }
		public int PageIndex { get { return pageState.PageIndex; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public PointF Location { 
			get { 
				float scale = Scale;
				return new PointF(position.X * scale, position.Y * scale);
			} 
		}
		public PdfTransformationMatrix DeviceTransformationMatrix { 
			get { 
				float scale = Scale;
				return new PdfTransformationMatrix(scale, 0, 0, -scale, offsetX, offsetY);
			} 
		}
		public bool ShouldUseRectangularGraphicsPath {
			get {
				PdfTransformationMatrix matrix = GraphicsState.TransformationMatrix;
				return Paths.Count <= 1 && ((matrix.A == 0 && matrix.D == 0) || (matrix.B == 0 && matrix.C == 0));
			}
		}
		bool IsVerticalOrientation {
			get {
				double textAngle = Math.Abs(TextAngle);
				return textAngle > quarterPI && textAngle < threeQuarterPI;
			}
		}
		PdfViewerCommandInterpreter(PdfDocumentState documentState, int pageIndex, Graphics graphics, float scale, int rotateAngle, PdfRectangle boundingBox, PointF position, PdfRenderMode renderMode) 
				: base(documentState.GetPageState(pageIndex).Page, rotateAngle, documentState.ImageDataStorage, scale, boundingBox) { 
			this.documentState = documentState;
			this.graphics = graphics;
			this.position = position;
			this.renderMode = renderMode;
			this.rotateAngle = rotateAngle * degreeToRadianFactor;
			pageState = documentState.GetPageState(pageIndex);
			graphicsState = graphics.Save();
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			initialClipRegion = graphics.Clip;
			offsetX = position.X * scale;
			offsetY = position.Y * scale;
			PdfPoint clippingTopLeft;
			PdfPoint clippingBottomRight;
			switch (rotateAngle) {
				case 90:
				case 270:
					clippingTopLeft = new PdfPoint(0, 0);
					clippingBottomRight = new PdfPoint(boundingBox.Height, -boundingBox.Width);
					break;
				default:
					offsetY += boundingBox.Height * scale;
					clippingTopLeft = new PdfPoint(0, boundingBox.Height);
					clippingBottomRight = new PdfPoint(boundingBox.Width, 0);
					break;
			}
			PointF transformedClippingTopLeft = TransformPoint(clippingTopLeft);
			PointF transformedClippingBottomRight = TransformPoint(clippingBottomRight);
			pageClippingRect = RectangleF.FromLTRB(transformedClippingTopLeft.X, transformedClippingTopLeft.Y, transformedClippingBottomRight.X, transformedClippingBottomRight.Y);
			UpdateClip();
			UpdateBrushes();
		}
		PdfViewerCommandInterpreter(PdfDocumentState documentState, int pageIndex, Graphics graphics, Size bitmapSize, PdfRenderMode renderMode)
				: this(documentState, pageIndex, graphics, 1, 0, new PdfRectangle(0, 0, bitmapSize.Width, bitmapSize.Height), new PointF(0, 0), renderMode) {	  
			width = bitmapSize.Width;
			height = bitmapSize.Height;
		}
		public PdfViewerCommandInterpreter(PdfDocumentState documentState, int pageIndex, Graphics graphics, float scale, PointF position, PdfRenderMode renderMode)
				: this(documentState, pageIndex, graphics, scale, PdfPageTreeNode.NormalizeRotate(documentState.RotationAngle + documentState.GetPageState(pageIndex).Page.Rotate), documentState.GetPageState(pageIndex).Page.CropBox, position, renderMode) {
			PdfPoint pageSize = documentState.GetPageSize(pageIndex);
			width = Convert.ToInt32(pageSize.X * scale);
			height = Convert.ToInt32(pageSize.Y * scale);
		}
		public PointF TransformPoint(PdfPoint point) {
			float scale = Scale;
			return new PointF((float)(offsetX + scale * point.X), (float)(offsetY - scale * point.Y));
		}
		public PointF TransformShadingPoint(PdfPoint point) {
			PointF transformedPoint = TransformPoint(point);
			PointF location = Location;
			return new PointF(transformedPoint.X - location.X, transformedPoint.Y - location.Y);
		}
		public Size CalculateSize(PdfTransformationMatrix matrix, PdfRectangle boundingBox) { 
			if (boundingBox.Width == 0 || boundingBox.Height == 0)
				return Size.Empty;
			PointF topLeft = TransformPoint(matrix.Transform(boundingBox.TopLeft));
			PointF topRight = TransformPoint(matrix.Transform(boundingBox.TopRight));
			PointF bottomRight = TransformPoint(matrix.Transform(boundingBox.BottomRight)); 
			PointF bottomLeft = TransformPoint(matrix.Transform(boundingBox.BottomLeft));
			return new Size(Math.Max(1, Convert.ToInt32(Math.Sqrt(Math.Max(CalculateDistanceSquare(topRight, topLeft), CalculateDistanceSquare(bottomRight, bottomLeft))))), 
							Math.Max(1, Convert.ToInt32(Math.Sqrt(Math.Max(CalculateDistanceSquare(topLeft, bottomLeft), CalculateDistanceSquare(bottomRight, topRight))))));
		}
		public void DrawAnnotations() {
			if (documentState != null)
				documentState.EnsureWidgetAppearances();
			foreach (PdfAnnotationState annotationState in pageState.AnnotationStates)
				annotationState.Draw(this);
		}
		public void DrawContent() {
			SaveGraphicsState();
			Execute();
			RestoreGraphicsState();
			if (renderMode == PdfRenderMode.Print) 
				DrawAnnotations();
			if (HasFunctionalLimits)
				pageState.Page.HasFunctionalLimits = true;
		}
		public void DrawSelection(Color selectionColor) {
			PdfSelection selection = documentState.SelectionState.Selection;
			if (selection != null) {
				int pageIndex = pageState.PageIndex;
				List<PdfOrientedRectangle> rectangles = new List<PdfOrientedRectangle>();
				foreach (PdfContentHighlight highlight in selection.Highlights)
					if (highlight.PageIndex == pageIndex)
						rectangles.AddRange(highlight.Rectangles);
				if (rectangles.Count != 0) {
					Matrix transform = graphics.Transform;
					try {
						using (GraphicsPath path = new GraphicsPath()) {
							path.FillMode = FillMode.Winding;
							PdfPage page = pageState.Page;
							float scale = Scale;
							foreach (PdfOrientedRectangle rectangle in rectangles) {
								IList<PdfPoint> vertices = rectangle.Vertices;
								int verticesCount = vertices.Count;
								PointF[] polygonPoints = new PointF[verticesCount];
								for (int i = 0; i < verticesCount; i++) {
									PdfPoint viewPoint = page.ToUserSpace(vertices[i], scale, scale, documentState.RotationAngle);
									polygonPoints[i] = new PointF((float)(viewPoint.X + position.X * scale), (float)(viewPoint.Y + position.Y * scale));
								}
								path.AddPolygon(polygonPoints);
							}
							using (SolidBrush brush = new SolidBrush(selectionColor))
								graphics.FillPath(brush, path);
						}
					}
					finally {
						graphics.Transform = transform;
						transform.Dispose();
					}
				}
			}
		}
		void UpdateClip() {
			if (clipRegion != null)
				clipRegion.Dispose();
			clipRegion = initialClipRegion.Clone();
			clipRegion.Intersect(pageClippingRect);
			if (currentClipRegion != null)
				clipRegion.Intersect(currentClipRegion);
			graphics.Clip = clipRegion;
		}
		void UpdateSmoothingMode() {
			if (ShouldUseRectangularGraphicsPath && GraphicsState.SmoothnessTolerance == 0) {
				foreach (PdfGraphicsPath path in Paths)
					if (!path.Flat)
						return;
				graphics.SmoothingMode = SmoothingMode.None;
			}
		}
		void EnsureEndText() {
			if (hdc != IntPtr.Zero) {
				DoEndText(hdcGraphics);
				EnsureEndText();
			}
		}
		[SecuritySafeCritical]
		void DoBeginText(Graphics graphics) {
			EnsureEndText();
			hdcGraphics = graphics;
			clipRegionHandle = clipRegion.GetHrgn(graphics);
			hdc = graphics.GetHdc();
			Gdi32Interop.SelectClipRgn(hdc, clipRegionHandle);
			UpdateCurrentFont();
			defaultGraphicsMode = Gdi32Interop.SetGraphicsMode(hdc, GraphicsMode.GM_ADVANCED);
			defaultTextAlign = Gdi32Interop.SetTextAlign(hdc, TextAlign.TA_BASELINE);
			defaultBackgroundMode = Gdi32Interop.SetBkMode(hdc, BackgroundMode.TRANSPARENT);
			defaultTextColor = UpdateTextColor();
		}
		[SecuritySafeCritical]
		void DoEndText(Graphics graphics) {
			if (defaultFont != IntPtr.Zero) {
				Gdi32Interop.DeleteObject(Gdi32Interop.SelectObject(hdc, defaultFont));
				defaultFont = IntPtr.Zero;
			}
			Gdi32Interop.SelectClipRgn(hdc, IntPtr.Zero);
			Gdi32Interop.SetTextAlign(hdc, defaultTextAlign);
			Gdi32Interop.SetBkMode(hdc, defaultBackgroundMode);
			Gdi32Interop.SetTextColor(hdc, defaultTextColor);
			Gdi32Interop.SetGraphicsMode(hdc, defaultGraphicsMode);
			graphics.ReleaseHdc(hdc);
			hdc = IntPtr.Zero;
			clipRegion.ReleaseHrgn(clipRegionHandle);
		}
		[SecuritySafeCritical]
		bool UpdateCurrentFont(string fontName, PdfFontRegistrationData fontData, int height, int width, FontQuality quality) {
			int angle = Convert.ToInt32(TextAngle * textAngleFactor);
			IntPtr handle = Gdi32Interop.CreateFont(height, width, angle, angle, fontData.Weight, fontData.Italic, false, false,
				FontCharSet.DEFAULT_CHARSET, FontOutputPrecision.OUT_DEFAULT_PRECIS, FontClipPrecision.CLIP_DEFAULT_PRECIS, quality, fontData.PitchAndFamily, fontName);
			if (handle == IntPtr.Zero)
				return false;
			IntPtr previousFont = Gdi32Interop.SelectObject(hdc, handle);
			if (defaultFont == IntPtr.Zero)
				defaultFont = previousFont;
			else
				Gdi32Interop.DeleteObject(previousFont);
			return true;
		}
		[SecuritySafeCritical]
		void UpdateCurrentFont(FontQuality quality = FontQuality.DEFAULT_QUALITY) {
			if (hdc != IntPtr.Zero) {
				PdfTextState textState = GraphicsState.TextState;
				PdfFont font = textState.Font;
				if (font != null) {
					PdfFontRegistrationData fontData = documentState.FontStorage.Register(font);
					if (fontData.IsType3Font)
						HasFunctionalLimits = true;
					string fontName = fontData.Name;
					if (fontName.Length > maxFontNameLength)
						fontName = fontName.Substring(0, maxFontNameLength);
					useEmbeddedFontEncoding = fontData.UseEmbeddedEncoding;
					double fontHeight = textState.FontSize * Scale;
					double fontWidthFactor = fontData.WidthFactor;
					double textHeightFactor = TextHeightFactor;
					double textWidthFactor = TextWidthFactor;
					int height = Convert.ToInt32(-fontHeight * textHeightFactor);
					if (height != 0 && UpdateCurrentFont(fontName, fontData, height, Convert.ToInt32(fontHeight * fontWidthFactor * textWidthFactor), quality) && fontWidthFactor == 0) {
						double widthToHeightRatio = textWidthFactor / textHeightFactor;
						if (widthToHeightRatio != 1) {
							TEXTMETRIC textMetric;
							Gdi32Interop.GetTextMetrics(hdc, out textMetric);
							int averageCharWidth = textMetric.AveCharWidth;
							int updatedCharWidth = Convert.ToInt32(textMetric.AveCharWidth * widthToHeightRatio);
							if (updatedCharWidth != averageCharWidth)
								UpdateCurrentFont(fontName, fontData, height, updatedCharWidth, quality);
						}
					}
				}
			}
		}
		[SecuritySafeCritical]
		int UpdateTextColor() {
			int textColor;
			PdfColor color = GraphicsState.NonStrokingColor;
			if (color == null) {
				HasFunctionalLimits = true;
				textColor = 0;
			}
			else {
				PdfRGBColor rgbColor = PdfRGBColor.FromColor(color);
				textColor = (Convert.ToByte(rgbColor.B * 255) << 16) + (Convert.ToByte(rgbColor.G * 255) << 8) + Convert.ToByte(rgbColor.R * 255);
			}
			return Gdi32Interop.SetTextColor(hdc, textColor);
		}
		[SecuritySafeCritical]
		void DrawString(short[] str, Point p, int[] spacing) {
			int x = p.X;
			int y = p.Y;
			PdfTextState textState = GraphicsState.TextState;
			double horizontalScaling = textState.HorizontalScaling / 100;
			double skewAngleTangens = Math.Tan(CalcAngle(new PdfPoint(0, 1)) - TextAngle - halfPI);
			bool shouldTransform = horizontalScaling != 1 || skewAngleTangens != 0.0;
			if (shouldTransform) {
				XFORM xForm = IsVerticalOrientation ? new XFORM(1, -(float)skewAngleTangens, 0, (float)horizontalScaling, 0, (float)(x * skewAngleTangens)) :
													  new XFORM((float)horizontalScaling, 0, (float)skewAngleTangens, 1, -(float)(y * skewAngleTangens), 0);
				Gdi32Interop.SetWorldTransform(hdc, ref xForm);
			}
			int stringLength = str.Length;
			RECT rect = new RECT();
			PdfFont font = textState.Font;
			PdfSimpleFontEncoding encoding = font.ActualEncoding as PdfSimpleFontEncoding;
			if (encoding == null) {
				font.UpdateGlyphCodes(str);
				Gdi32Interop.ExtTextOut(hdc, x, y, font.UseGlyphIndices ? TextOutOptions.ETO_GLYPH_INDEX : TextOutOptions.ETO_IGNORELANGUAGE, ref rect, str, stringLength, spacing);
			}
			else {
				PdfTrueTypeFont trueTypeFont = font as PdfTrueTypeFont;
				PdfFontDescriptor fontDescriptor = font.FontDescriptor;
				bool isSymbolic = fontDescriptor != null && (fontDescriptor.Flags & PdfFontFlags.Symbolic) != 0;
				if ((trueTypeFont == null || !isSymbolic) && !useEmbeddedFontEncoding)
					PdfTrueTypeFont.UpdateGlyphCodes(str, encoding);
				else
					font.UpdateGlyphCodes(str);
				Gdi32Interop.ExtTextOut(hdc, x, y, TextOutOptions.ETO_IGNORELANGUAGE, ref rect, str, stringLength, spacing);
			}
			if (shouldTransform) {
				XFORM xForm = new XFORM();
				Gdi32Interop.ModifyWorldTransform(hdc, ref xForm, ModifyWorldTransformMode.MWT_IDENTITY);
			}
		}
		void UpdateStrokingBrush() {
			PdfGraphicsState graphicsState = GraphicsState;
			PdfGdiBrush newStrokingBrush = new PdfGdiBrush(this, graphicsState.StrokingColor, graphicsState.StrokingAlphaConstant);
			if (currentStrokingBrush != null)  
				currentStrokingBrush.Dispose();
			currentStrokingBrush = newStrokingBrush;
		}
		void UpdateNonStrokingBrush() {
			PdfGraphicsState graphicsState = GraphicsState;
			PdfGdiBrush newNonStrokingBrush = new PdfGdiBrush(this, graphicsState.NonStrokingColor, graphicsState.NonStrokingAlphaConstant);
			if (currentNonStrokingBrush != null)  
				currentNonStrokingBrush.Dispose();
			currentNonStrokingBrush = newNonStrokingBrush;
		}
		void UpdateBrushes() { 
			UpdateStrokingBrush();
			UpdateNonStrokingBrush();
		}
		double CalcFinalAngle(PdfPoint startPoint, PdfPoint endPoint) {
			return PdfMathUtils.NormalizeAngle(CalcAngle(startPoint, endPoint) + rotateAngle);
		}
		double CalcLength(PdfPoint direction) {
			PdfTransformationMatrix transformationMatrix = GraphicsState.TransformationMatrix;
			PdfTransformationMatrix matrix = new PdfTransformationMatrix(transformationMatrix.A, transformationMatrix.B, transformationMatrix.C, transformationMatrix.D, 0, 0);
			PdfPoint transformed = matrix.Transform(direction);
			double x = transformed.X;
			double y = transformed.Y;
			return Math.Sqrt(x * x + y * y);
		}
		double CalcLineExtendFactor(PdfPoint startPoint, PdfPoint endPoint) {
			double angle = CalcFinalAngle(startPoint, endPoint);
			double sin = Math.Sin(angle);
			double cos = Math.Cos(angle);
			return CalcLength(new PdfPoint(sin, cos)) / CalcLength(new PdfPoint(cos, sin));
		}
		double GetLineWidth(PdfPoint startPoint, PdfPoint endPoint) {
			double lineWidth = GraphicsState.LineWidth;
			if (lineWidth >= 0.1)
				return lineWidth;
			double angle = CalcFinalAngle(startPoint, endPoint);
			return 1 / (CalcLength(new PdfPoint(Math.Cos(angle), Math.Sin(angle))) * Scale);
		}
		float GetPenWidth(PdfPoint startPoint, PdfPoint endPoint) {
			double lineWidth = GraphicsState.LineWidth;
			double angle = CalcFinalAngle(startPoint, endPoint);
			return lineWidth < 0.1 ? 1 : (float)(lineWidth * CalcLength(new PdfPoint(Math.Cos(angle), Math.Sin(angle))) * Scale);
		}
		void UpdatePenProperties(Pen pen, PdfPoint startPoint, PdfPoint endPoint) {
			pen.Width = GetPenWidth(startPoint, endPoint);
			PdfGraphicsState graphicsState = GraphicsState;
			PdfLineStyle lineStyle = graphicsState.LineStyle;
			if (lineStyle.IsDashed) {
				double lineWidth = GetLineWidth(startPoint, endPoint);
				pen.DashStyle = DashStyle.Custom;
				pen.DashOffset = (float)lineStyle.DashPhase;
				double[] dashPattern = (double[])lineStyle.DashPattern.Clone();
				int patternLength = dashPattern.Length;
				if (patternLength == 2 && dashPattern[0] == 0)
					dashPattern[1]--;
				List<float> pattern = new List<float>();
				for (int i = 0; i < patternLength; i++) {
					float v = (float)(dashPattern[i] / lineWidth);
					if (v <= 0) {
						if (graphicsState.LineCap != PdfLineCapStyle.Butt)
							pattern.Add(1f);
					}
					else
						pattern.Add(v);
				}
				if (pattern.Count == 1)
					pattern.Add(pattern[0]);
				pen.DashPattern = pattern.ToArray();
			}
		}
		bool StrokeRectangle(Pen pen, PdfGraphicsPath path) {
			PdfRectangularGraphicsPath rectangularPath = path as PdfRectangularGraphicsPath;
			if (rectangularPath == null)
				return false;
			PdfRectangle rectangle = rectangularPath.Rectangle;
			double left = rectangle.Left;
			double right = rectangle.Right;
			double bottom = rectangle.Bottom;
			PdfPoint bottomLeft = new PdfPoint(left, bottom);
			PdfPoint bottomRight = new PdfPoint(right, bottom);
			double lineExtendFactor = CalcLineExtendFactor(bottomLeft, bottomRight);
			if (lineExtendFactor > 0.5 && lineExtendFactor < 2)
				return false;
			LineCap lineCap = pen.StartCap;
			try {
				pen.StartCap = LineCap.Flat;
				pen.EndCap = LineCap.Flat;
				float scaleFactor = Scale * 2;
				double top = rectangle.Top;
				PdfPoint topLeft = new PdfPoint(left, top);
				UpdatePenProperties(pen, bottomLeft, bottomRight);
				float horizontalExtend = GetPenWidth(bottomLeft, topLeft) / scaleFactor;
				double actualLeft = left - horizontalExtend;
				double actualRight = right + horizontalExtend;
				graphics.DrawLine(pen, TransformPoint(new PdfPoint(actualLeft, bottom)), TransformPoint(new PdfPoint(actualRight, bottom)));
				graphics.DrawLine(pen, TransformPoint(new PdfPoint(actualLeft, top)), TransformPoint(new PdfPoint(actualRight, top)));
				UpdatePenProperties(pen, bottomLeft, topLeft);
				float verticalExtend = GetPenWidth(bottomLeft, bottomRight) / scaleFactor;
				double actualBottom = bottom - verticalExtend;
				double actualTop = top + verticalExtend;
				graphics.DrawLine(pen, TransformPoint(new PdfPoint(left, actualBottom)), TransformPoint(new PdfPoint(left, actualTop)));
				graphics.DrawLine(pen, TransformPoint(new PdfPoint(right, actualBottom)), TransformPoint(new PdfPoint(right, actualTop)));
			}
			finally {
				pen.StartCap = lineCap;
				pen.EndCap = lineCap;
			}
			return true;
		}
		void DrawTilingPatternKeyCell(PdfTilingPattern pattern, PdfColor color) {
			SaveGraphicsState();
			try {
				PdfRectangle boundingBox = BoundingBox;
				UpdateTransformationMatrix(pattern.GetTransformationMatrix(Convert.ToInt32(boundingBox.Width), Convert.ToInt32(boundingBox.Height)));
				if (!pattern.Colored)
					SetColorForNonStrokingOperations(new PdfColor(color.Components));
				Execute(pattern.Commands);
			}
			finally {
				RestoreGraphicsState();
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				EnsureEndText();
				graphics.Restore(graphicsState);
				clipRegion.Dispose();
				foreach (Region region in clipRegionStack)
					region.Dispose();
				initialClipRegion.Dispose();
				documentState.FontStorage.Unregister();
				foreach (PdfGdiBrush strokingBrush in strokingBrushStack)
					strokingBrush.Dispose();
				foreach (PdfGdiBrush nonStrokingBrush in nonStrokingBrushStack)
					nonStrokingBrush.Dispose();
			}
		}
		protected override void UpdateGraphicsState(PdfGraphicsStateChange change) {
			if ((change & PdfGraphicsStateChange.Font) == PdfGraphicsStateChange.Font)
				UpdateCurrentFont();
			if ((change & PdfGraphicsStateChange.Alpha) == PdfGraphicsStateChange.Alpha) 
				UpdateBrushes();
		}
		[SecuritySafeCritical]
		protected override void DrawString(PdfStringData data, PdfPoint location, double[] glyphOffsets) {
			PdfGraphicsState graphicsState = GraphicsState;
			PdfTextState textState = graphicsState.TextState;
			float scale = Scale;
			if (textState.RenderingMode != PdfTextRenderingMode.Invisible && textState.FontSize * scale * TextHeightFactor >= 0.5) {
				PointF origin = TransformPoint(location);
				Point p = Point.Round(origin);
				float diff = p.X - origin.X;
				double actualScale = scale;
				double horizontalScaling = 100 / textState.HorizontalScaling;
				if (horizontalScaling != 1) {
					actualScale *= horizontalScaling;
					if (IsVerticalOrientation)
						p.Y = Convert.ToInt32(p.Y * horizontalScaling);
					else
						p.X = Convert.ToInt32(p.X * horizontalScaling);
				}
				short[] str = data.Str;
				int stringLength = str.Length;
				int[] spacing = new int[stringLength];
				int previousX = 0;
				for (int i = 0; i < stringLength; i++) {
					int next = (int)Math.Round(glyphOffsets[i] * actualScale + diff) - previousX;
					spacing[i] = next;
					previousX += next;
				}
				byte alphaConstant = Convert.ToByte(graphicsState.NonStrokingAlphaConstant * 255);
				if (alphaConstant == 255)
					DrawString(str, p, spacing);
				else {
					DoEndText(graphics);
					using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb)) {
						using (Graphics g = Graphics.FromImage(bitmap)) {
							g.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, width, height));
							DoBeginText(g);
							try {
								UpdateCurrentFont(FontQuality.NONANTIALIASED_QUALITY);
								DrawString(str, p, spacing);
							}
							finally {
								DoEndText(g);
							}
							BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
							try {
								int count = width * height * 4;
								byte[] imageData = new byte[count];
								IntPtr ptr = bitmapData.Scan0;
								Marshal.Copy(ptr, imageData, 0, count);
								for (int index = 3; index < count; index += 4)
									imageData[index] = imageData[index] == 0 ? (byte)0 : alphaConstant;
								Marshal.Copy(imageData, 0, ptr, count);
							}
							finally {
								bitmap.UnlockBits(bitmapData);
							}
							graphics.DrawImage(bitmap, Point.Empty);
						}
					}
					DoBeginText(graphics);
				}
			}
		}
		protected override bool ShouldDrawAnnotation(PdfAnnotation annotation) {
			switch (renderMode) {
				case PdfRenderMode.View:
					if (annotation.Flags.HasFlag(PdfAnnotationFlags.NoView))
						return false;
					break;
				case PdfRenderMode.Print:
					if (!annotation.Flags.HasFlag(PdfAnnotationFlags.Print))
						return false;
					break;
			}
			return base.ShouldDrawAnnotation(annotation);
		}
		public override void SaveGraphicsState() {
			base.SaveGraphicsState();
			if (currentClipRegion != null)
				clipRegionStack.Push((Region)currentClipRegion.Clone());
			strokingBrushStack.Push(currentStrokingBrush.Clone());
			nonStrokingBrushStack.Push(currentNonStrokingBrush.Clone());
		}
		public override void RestoreGraphicsState() {
			base.RestoreGraphicsState();
			if (currentClipRegion != null)
				currentClipRegion.Dispose();
			currentClipRegion = clipRegionStack.Count > 0 ? clipRegionStack.Pop() : null;
			UpdateClip();
			if (strokingBrushStack.Count > 0) { 
				currentStrokingBrush.Dispose();
				currentStrokingBrush = strokingBrushStack.Pop();
			}
			if (nonStrokingBrushStack.Count > 0) { 
				currentNonStrokingBrush.Dispose();
				currentNonStrokingBrush = nonStrokingBrushStack.Pop();
			}
		}		
		public override void SetColorForStrokingOperations(PdfColor color) {
			base.SetColorForStrokingOperations(color);  
			UpdateStrokingBrush();
		}
		public override void SetColorForNonStrokingOperations(PdfColor color) {
			base.SetColorForNonStrokingOperations(color);
			UpdateNonStrokingBrush();
			if (hdc != IntPtr.Zero)
				UpdateTextColor();
		}
		public override void SetFont(PdfFont font, double fontSize) {
			base.SetFont(font, fontSize);
			UpdateCurrentFont();
		}
		public override void SetTextMatrix(PdfTransformationMatrix matrix) {
			base.SetTextMatrix(matrix);
			UpdateCurrentFont();
		}
		public override void DrawImage(PdfImage image) {
			PdfImageData imageData = GetImageData(image);
			if (imageData == null)
				HasFunctionalLimits = true;
			else {
				bool shouldResetTextDrawing = hdc != IntPtr.Zero;
				if (shouldResetTextDrawing)
					DoEndText(graphics);
				InterpolationMode interpolationMode = graphics.InterpolationMode;
				Matrix currentTransform = graphics.Transform;
				try {
					PerformRendering(image, imageData, bitmap => {
						using (Matrix transform = GetImageMatrix(TransformPoint(GraphicsState.TransformationMatrix.Transform(new PdfPoint(0, 0))))) {
							transform.Translate(0, -1);
							graphics.Transform = transform;
						}
						if (image.BitsPerComponent > 1 && GraphicsState.NonStrokingAlphaConstant == 1 && bitmap.PixelFormat != PixelFormat.Format32bppArgb) {
							graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
							DrawImage(graphics, bitmap, imageDestinationPointsCorrected);
							graphics.InterpolationMode = InterpolationMode.Default;
							DrawImage(graphics, bitmap, ImageDestinationPoints);
						}
						else {
							graphics.InterpolationMode = GetInterpolationMode(imageData.Width, imageData.Height, image.BitsPerComponent);
							DrawImage(graphics, bitmap, ImageDestinationPoints);
						}
					});
				}
				finally {
					graphics.InterpolationMode = interpolationMode;
					graphics.Transform = currentTransform;
					currentTransform.Dispose();
					if (shouldResetTextDrawing)
						DoBeginText(graphics);
				}
			}
		}
		public override void DrawShading(PdfShading shading) { 
			PdfRectangle boundingBox = shading.BoundingBox;
			if (boundingBox == null) 
				PdfShadingPainter.Draw(this, graphics, shading);
			else { 
				Bitmap bitmap =  PdfShadingPainter.CreateBitmap(this, shading);
				if (bitmap != null)
					using (bitmap) {
						Matrix currentTransform = graphics.Transform;
						try {
							PdfTransformationMatrix currentMatrix = GraphicsState.TransformationMatrix;
							using (Matrix matrix = GetImageMatrix(TransformPoint(currentMatrix.Transform(boundingBox.TopLeft)))) 
								graphics.Transform = matrix;
							DrawImage(graphics, bitmap, new PointF[] { new PointF(0, 0), new PointF((float)boundingBox.Width, 0), new PointF(0, (float)boundingBox.Height) });
						}
						finally {
							graphics.Transform = currentTransform;
							currentTransform.Dispose();
						} 
					} 
			}
		}
		public override void StrokePaths() {
			SmoothingMode smoothingMode = graphics.SmoothingMode;
			try {
				UpdateSmoothingMode();
				using (Pen pen = new Pen(currentStrokingBrush.Brush)) {
					PdfGraphicsState graphicsState = GraphicsState;
					LineCap lineCap;
					bool shouldExtendLine;
					switch (graphicsState.LineCap) {
						case PdfLineCapStyle.ProjectingSquare:
							pen.DashCap = DashCap.Flat;
							lineCap = LineCap.Square;
							shouldExtendLine = true;
							break;
						case PdfLineCapStyle.Round:
							pen.DashCap = DashCap.Round;
							lineCap = LineCap.Round;
							shouldExtendLine = true;
							break;
						default:
							pen.DashCap = DashCap.Flat;
							lineCap = LineCap.Flat;
							shouldExtendLine = false;
							break;
					}
					pen.StartCap = lineCap;
					pen.EndCap = lineCap;
					switch (graphicsState.LineJoin) {
						case PdfLineJoinStyle.Bevel:
							pen.LineJoin = LineJoin.Bevel;
							break;
						case PdfLineJoinStyle.Round:
							pen.LineJoin = LineJoin.Round;
							break;
						default:
							pen.LineJoin = LineJoin.MiterClipped;
							break;
					}
					pen.MiterLimit = (float)graphicsState.MiterLimit;
					PdfGraphicsPathBuilder builder = new PdfGraphicsPathBuilder(this);
					foreach (PdfGraphicsPath path in Paths) {
						IList<PdfGraphicsPathSegment> segments = path.Segments;
						if (segments.Count > 0) {
							PdfPoint startPoint = path.StartPoint;
							PdfPoint endPoint = segments[0].EndPoint;
							double extendSize;
							if (shouldExtendLine && !path.Closed && !path.EndPoint.Equals(startPoint)) {
								double factor = CalcLineExtendFactor(startPoint, endPoint);
								extendSize = factor > 2 ? (GetLineWidth(startPoint, endPoint) * factor * 0.5) : 0;
							}
							else {
								if (StrokeRectangle(pen, path))
									continue;
								extendSize = 0;
							}
							UpdatePenProperties(pen, startPoint, endPoint);
							using (GraphicsPath graphicsPath = builder.CreatePath(path, extendSize)) {
								RectangleF bounds = graphicsPath.GetBounds();
								if (bounds.Width >= 0.08f || bounds.Height >= 0.08f) 
									graphics.DrawPath(pen, graphicsPath);
							}
						}
					}
				}
			}
			finally {
				graphics.SmoothingMode = smoothingMode;
			}
		}
		public override void FillPaths(bool useNonzeroWindingRule) {
			using (GraphicsPath graphicsPath = new PdfGraphicsPathBuilder(this).CreatePath(Paths)) {
				graphicsPath.FillMode = useNonzeroWindingRule ? FillMode.Winding : FillMode.Alternate;
				SmoothingMode smoothingMode = graphics.SmoothingMode;
				try {
					UpdateSmoothingMode();
					graphics.FillPath(currentNonStrokingBrush.Brush, graphicsPath);
				}
				finally {
					graphics.SmoothingMode = smoothingMode;
				}
			}
		}
		public override void ClipPaths(bool useNonzeroWindingRule) {
			IList<PdfGraphicsPath> paths = Paths;
			if (paths.Count == 0)
				return;
			Region newClipRegion;
			PdfGraphicsPathBuilder builder = new PdfGraphicsPathBuilder(this);
			RectangleF rectangle = builder.CreateRectangle(paths[0]);
			if (rectangle.IsEmpty) {
				using (GraphicsPath graphicsPath = builder.CreatePath(paths))
					if (graphicsPath.PointCount == 0)
						return;
				newClipRegion = new Region(builder.CreatePath(paths));
			}
			else 
				newClipRegion = new Region(rectangle);
			if (currentClipRegion == null)
				currentClipRegion = newClipRegion;
			else {
				currentClipRegion.Intersect(newClipRegion);
				newClipRegion.Dispose();
			}
			UpdateClip();
		}
		public override void BeginText() {
			base.BeginText();
			DoBeginText(graphics);
		}
		public override void EndText() {
			base.EndText();
			DoEndText(graphics);
		}
	}
}
