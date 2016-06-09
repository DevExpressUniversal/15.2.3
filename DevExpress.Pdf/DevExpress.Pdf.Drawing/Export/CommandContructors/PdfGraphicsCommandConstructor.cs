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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	abstract class PdfGraphicsCommandConstructor {
		const float pdfDpi = 72f;
		readonly PdfRectangle bBox;
		readonly PdfResources resources;
		readonly PdfDocumentCatalog documentCatalog;
		float factorX;
		float factorY;
		readonly IList<PdfCommand> commands = new List<PdfCommand>();
		readonly PdfGraphicsStateStack graphicsStateStack = new PdfGraphicsStateStack();
		readonly PdfCommandConstructor commandConstructor;
		readonly PdfStringPainter textPainter;
		readonly PdfEditableFontDataCache fontCache;
		readonly PdfImageCache imageCache;
		readonly PdfGraphicsStateParamersCache<double> strokingAlphaCache = new PdfGraphicsStateParamersCache<double>(p => new PdfGraphicsStateParameters() { StrokingAlphaConstant = p });
		readonly PdfGraphicsStateParamersCache<double> nonStrokingAlphaCache = new PdfGraphicsStateParamersCache<double>(p => new PdfGraphicsStateParameters() { NonStrokingAlphaConstant = p });
		double actualPageHeight;
		public PdfEditableFontDataCache FontCache { get { return fontCache; } }
		public PdfImageCache ImageCache { get { return imageCache; } }
		public float DpiX {
			get { return factorX * pdfDpi; }
			set { factorX = value / pdfDpi; }
		}
		public float DpiY {
			get { return factorY * pdfDpi; }
			set { factorY = value / pdfDpi; }
		}
		public float FactorX { get { return factorX; } }
		public float FactorY { get { return factorY; } }
		public PdfResources Resources { get { return resources; } }
		public PdfRectangle BBox { get { return bBox; } }
		protected IList<PdfCommand> Commands { get { return commands; } }
		protected PdfGraphicsCommandConstructor(PdfRectangle bBox, PdfResources resources, float factorX, float factorY, PdfEditableFontDataCache fontCache, PdfImageCache imageCache, PdfDocumentCatalog catalog)
			: this(bBox, new List<PdfCommand>(), resources, factorX, factorY, fontCache, imageCache, catalog) {
		}
		protected PdfGraphicsCommandConstructor(PdfRectangle bBox, IList<PdfCommand> commands, PdfResources resources, float factorX, float factorY, PdfEditableFontDataCache fontCache, PdfImageCache imageCache, PdfDocumentCatalog catalog) {
			this.resources = resources;
			this.bBox = bBox;
			this.factorX = factorX;
			this.factorY = factorY;
			this.fontCache = fontCache;
			this.imageCache = imageCache;
			this.commands = commands;
			actualPageHeight = bBox.Height;
			commandConstructor = new PdfCommandConstructor(commands, resources);
			this.documentCatalog = catalog;
			this.textPainter = new PdfStringPainter(commandConstructor);
		}
		public void SaveGraphicsState() {
			graphicsStateStack.Push();
			commandConstructor.SaveGraphicsState();
		}
		public void RestoreGraphicsState() {
			graphicsStateStack.Pop();
			commandConstructor.RestoreGraphicsState();
		}
		public void SetMiterLimit(double miterLimit) {
			if (graphicsStateStack.Current.MiterLimit != miterLimit) {
				commandConstructor.SetMiterLimit(miterLimit);
				graphicsStateStack.Current.MiterLimit = miterLimit;
			}
		}
		public void DrawPath(PointF[] pathPoints, byte[] pathTypes) {
			foreach (PdfGraphicsPath path in TransformPath(pathPoints, pathTypes))
				commandConstructor.DrawPath(path);
		}
		public void FillPath(PointF[] pathPoints, byte[] pathTypes, bool nonZero) {
			foreach (PdfGraphicsPath path in TransformPath(pathPoints, pathTypes))
				commandConstructor.FillPath(path, nonZero);
		}
		public void DrawLine(float x1, float y1, float x2, float y2) {
			commandConstructor.DrawLine(TransformPoint(new PointF(x1, y1)), TransformPoint(new PointF(x2, y2)));
		}
		public void DrawPolygon(PointF[] points) {
			commandConstructor.DrawPolygon(TransformPoints(points));
		}
		public void FillPolygon(PointF[] points, bool nonZero) {
			commandConstructor.FillPolygon(TransformPoints(points), nonZero);
		}
		public void DrawRectangle(RectangleF rect) {
			commandConstructor.DrawRectangle(TransformRectangle(rect));
		}
		public void FillRectangle(RectangleF rect) {
			commandConstructor.FillRectangle(TransformRectangle(rect));
		}
		public void DrawEllipse(RectangleF rect) {
			commandConstructor.DrawEllipse(TransformRectangle(rect));
		}
		public void FillEllipse(RectangleF rect) {
			commandConstructor.FillEllipse(TransformRectangle(rect));
		}
		public void DrawBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4) {
			PdfPoint pdfPt1 = TransformPoint(pt1);
			PdfPoint pdfPt2 = TransformPoint(pt2);
			PdfPoint pdfPt3 = TransformPoint(pt3);
			PdfPoint pdfPt4 = TransformPoint(pt4);
			commandConstructor.DrawBezier(pdfPt1, pdfPt2, pdfPt3, pdfPt4);
		}
		public void DrawBeziers(PointF[] points) {
			commandConstructor.DrawBeziers(TransformPoints(points));
		}
		public void DrawImage(int imageNumber, RectangleF bounds) {
			commandConstructor.DrawXObject(resources.AddXObject(imageNumber), TransformRectangle(bounds));
		}
		public void DrawMetafile(int metafileFormNumber, RectangleF bounds) {
			PdfRectangle actualBounds = TransformRectangle(bounds);
			string formName = resources.AddXObject(metafileFormNumber);
			PdfForm form = (PdfForm)resources.GetXObject(formName);
			commandConstructor.DrawXObject(formName, new PdfRectangle(actualBounds.Left, actualBounds.Bottom, actualBounds.Left + (actualBounds.Width / form.BBox.Width),
				actualBounds.Bottom + (actualBounds.Height / form.BBox.Height)));
		}
		public void DrawMetafile(int metafileFormNumber, PointF location) {
			string formName = resources.AddXObject(metafileFormNumber);
			PdfForm form = (PdfForm)resources.GetXObject(formName);
			PdfPoint actualLocation = TransformPoint(location);
			commandConstructor.DrawXObject(formName, new PdfPoint(actualLocation.X, actualLocation.Y - form.BBox.Height));
		}
		public void RotateTransform(float degree) {
			commandConstructor.TranslateTransform(0, actualPageHeight);
			commandConstructor.RotateTransform(degree);
			commandConstructor.TranslateTransform(0, -actualPageHeight);
			actualPageHeight = GetActualPageHeight();
		}
		public void TranslateTransform(double x, double y) {
			commandConstructor.TranslateTransform(x / factorX, -y / factorY);
		}
		public void SetPen(PdfPen pen) {
			PdfGraphicsExportState current = graphicsStateStack.Current;
			double lineWidth = pen.Width / factorX;
			if (lineWidth != current.LineWidth) {
				commandConstructor.SetLineWidth(lineWidth);
				current.LineWidth = lineWidth;
			}
			SetColorForStrokingOperations(pen.Brush.GetBrush(this).GetColor(bBox, documentCatalog));
			if (pen.DashStyle != DashStyle.Solid) {
				float[] dashPattern = pen.DashPattern;
				double dashOffset = pen.DashOffset;
				double[] graphicsStateDashPattern = null;
				if (dashPattern != null) {
					int dashCount = dashPattern.Length;
					graphicsStateDashPattern = new double[dashCount];
					for (int i = 0; i < dashCount; i++)
						graphicsStateDashPattern[i] = dashPattern[i] * lineWidth;
					commandConstructor.SetLineStyle(PdfLineStyle.CreateDashed(graphicsStateDashPattern, dashOffset));
				}
			}
			else if (current.DashStyle != DashStyle.Solid)
				commandConstructor.SetLineStyle(PdfLineStyle.CreateSolid());
			current.DashStyle = pen.DashStyle;
			if (pen.DashCap == DashCap.Flat) {
				if (current.DashCap != pen.DashCap || current.LineCap != pen.StartCap) {
					commandConstructor.SetLineCapStyle(pen.StartCap == LineCap.Square ? PdfLineCapStyle.ProjectingSquare : PdfLineCapStyle.Butt);
					current.LineCap = pen.StartCap;
				}
			}
			else if (current.DashCap == DashCap.Flat)
				commandConstructor.SetLineCapStyle(PdfLineCapStyle.Round);
			current.DashCap = pen.DashCap;
			switch (pen.LineJoin) {
				case LineJoin.Bevel:
					if (current.LineJoin != LineJoin.Bevel)
						commandConstructor.SetLineJoinStyle(PdfLineJoinStyle.Bevel);
					break;
				case LineJoin.Round:
					if (current.LineJoin != LineJoin.Round)
						commandConstructor.SetLineJoinStyle(PdfLineJoinStyle.Round);
					break;
				default:
					if (current.LineJoin != LineJoin.Miter && current.LineJoin != LineJoin.MiterClipped)
						commandConstructor.SetLineJoinStyle(PdfLineJoinStyle.Miter);
					SetMiterLimit(pen.MiterLimit);
					break;
			}
			current.LineJoin = pen.LineJoin;
		}
		public void SetBrush(PdfBrush brush) {
			PdfTransparentColor color = brush.GetColor(bBox, documentCatalog);
			SetColorForNonStrokingOperation(color);
			if (color.Alpha != graphicsStateStack.Current.NonStrokingAlpha) {
				SetGraphicsStateParameters(nonStrokingAlphaCache.GetParameters(Math.Round(color.Alpha, 4)));
				graphicsStateStack.Current.NonStrokingAlpha = color.Alpha;
			}
		}
		public void SetBrush(PdfBrushContainer converter) {
			SetBrush(converter.GetBrush(this));
		}
		public void DrawLines(PointF[] points) {
			commandConstructor.DrawLines(TransformPoints(points));
		}
		public void IntersectClip(RectangleF rect) {
			commandConstructor.IntersectClip(TransformRectangle(rect));
		}
		public void IntersectClipWithoutWorldTransform(RectangleF rect) {
			PdfTransformationMatrix invertedMatrix = PdfTransformationMatrix.Invert(commandConstructor.CurrentTransform);
			PdfPoint[] points = new PdfPoint[] {new PdfPoint(rect.X, rect.Y), new PdfPoint(rect.Left, rect.Bottom), 
				new PdfPoint(rect.Right, rect.Bottom), new PdfPoint(rect.Right, rect.Top) };
			PdfTransformationMatrix gdi2PdfTransform = new PdfTransformationMatrix(1 / factorX, 0, 0, - 1 /factorY, 0, bBox.Height);
			invertedMatrix = PdfTransformationMatrix.Multiply(gdi2PdfTransform, invertedMatrix);
			PdfGraphicsPath path = new PdfGraphicsPath(invertedMatrix.Transform(points[0]));
			for (int i = 1; i < 4; i++)
				path.AppendLineSegment(invertedMatrix.Transform(points[i]));
			path.Closed = true;
			commandConstructor.IntersectClip(path);
		}
		public void UpdateTransformationMatrix(PdfTransformationMatrix matrix) {
			commandConstructor.ModifyTransformationMatrix(new PdfTransformationMatrix(matrix.A, matrix.B, matrix.C, matrix.D, matrix.E / factorX, -matrix.F / factorY));
			actualPageHeight = GetActualPageHeight();
		}
		public PdfRectangle TransformRectangle(RectangleF rect) {
			PdfPoint bottomLeft = TransformPoint(new PointF(Math.Min(rect.Left, rect.Right), Math.Max(rect.Top, rect.Bottom)));
			PdfPoint topRigth = TransformPoint(new PointF(Math.Max(rect.Left, rect.Right), Math.Min(rect.Top, rect.Bottom)));
			return new PdfRectangle(bottomLeft.X, bottomLeft.Y, topRigth.X, topRigth.Y);
		}
		public void Clear() {
			commandConstructor.FillRectangle(bBox);
		}
		public void DrawUnsupportedMetafile(Metafile metafile) {
			using (Bitmap bitmap = BitmapCreator.CreateBitmapWithResolutionLimit(metafile, Color.Transparent))
				DrawImage(imageCache.GetPdfImageObjectNumber(bitmap, false, null, 0), new RectangleF(0, 0, metafile.Width, metafile.Height));
		}
		public void DrawString(string text, RectangleF layout, PdfStringFormat format, PdfEditableFontData fontData, double fontSize, Color lineColor, bool useKerning) {
			PdfStringFormatter formatter = fontData.CreateStringFormatter(fontSize);
			IList<PdfStringGlyphRun> formattedLines;
			if (layout.IsEmpty) {
				PdfPoint point = TransformPoint(layout.Location);
				formattedLines = formatter.FormatString(text, point, format, useKerning);
				if (formattedLines.Count > 0)
					SetColors(fontData, lineColor);
				textPainter.DrawLines(formattedLines, fontData, fontSize, point, format, useKerning);
			}
			else {
				PdfRectangle layoutRect = TransformRectangle(layout);
				formattedLines = formatter.FormatString(text, layoutRect, format, useKerning);
				if (formattedLines.Count > 0)
					SetColors(fontData, lineColor);
				textPainter.DrawLines(formattedLines, fontData, fontSize, layoutRect, format, useKerning);
			}
		}
		void SetColors(PdfEditableFontData fontData, Color lineColor) {
			if (fontData.Underline || fontData.Strikeout)
				SetColorForStrokingOperations(new PdfTransparentColor(lineColor.A, lineColor.R / 255.0, lineColor.G / 255.0, lineColor.B / 255.0));
		}
		public void DrawString(string text, PointF location, PdfGraphicsTextOrigin textOrigin, PdfEditableFontData fontData,
			double fontSize, Color lineColor, bool useKerning) {
			DrawString(text, location, PdfStringFormat.GenericDefault, textOrigin, fontData, fontSize, lineColor, useKerning);
		}
		public void DrawString(string text, PointF location, PdfStringFormat format, PdfGraphicsTextOrigin textOrigin, PdfEditableFontData fontData,
			double fontSize, Color lineColor, bool useKerning) {
			PdfPoint point = TransformPoint(location);
			if (textOrigin == PdfGraphicsTextOrigin.Baseline) {
				format = new PdfStringFormat(format);
				format.LeadingMarginFactor = 0;
				format.TrailingMarginFactor = 0;
				point = new PdfPoint(point.X, point.Y + fontData.Metrics.GetAscent(fontSize));
			}
			PdfStringFormatter formatter = fontData.CreateStringFormatter(fontSize);
			IList<PdfStringGlyphRun> formattedLines = formatter.FormatString(text, point, format, useKerning);
			if (formattedLines.Count > 0)
				SetColors(fontData, lineColor);
			textPainter.DrawLines(formattedLines, fontData, fontSize, point, format, useKerning);
		}
		internal void DrawPrecalculatedString(PdfStringGlyphRun text, PointF location, PdfGraphicsTextOrigin textOrigin, PdfEditableFontData fontData, double fontSize, Color lineColor) {
			double lineWidth = text.Width * fontSize / 1000.0;
			PdfPoint pos = TransformPoint(location);
			PdfFont pdfFont = fontData.PdfFont;
			PdfFontMetrics metrics = fontData.Metrics;
			double ascent = metrics.GetAscent(fontSize);
			double descent = metrics.GetDescent(fontSize);
			double x = pos.X;
			double y = textOrigin == PdfGraphicsTextOrigin.Baseline ? pos.Y : pos.Y - ascent;
			if (fontData.ShouldEmulateItalic)
				commandConstructor.DrawObliqueString(text.TextData, new PdfPoint(x, y), pdfFont, fontSize, text.GlyphOffsets);
			else
				commandConstructor.DrawString(text.TextData, new PdfPoint(x, y), pdfFont, fontSize, text.GlyphOffsets);
			if (fontData.Underline || fontData.Strikeout) {
				SetColorForStrokingOperations(new PdfTransparentColor(lineColor.A, lineColor.R / 255.0, lineColor.G / 255.0, lineColor.B / 255.0));
				commandConstructor.SetLineWidth(fontSize / 14);
				if (fontData.Underline) {
					double underlineY = y - 0.5 * descent;
					commandConstructor.DrawLine(new PdfPoint(x, underlineY), new PdfPoint(x + lineWidth, underlineY));
				}
				if (fontData.Strikeout) {
					double strikeoutY = y + ascent * 0.5 - descent;
					commandConstructor.DrawLine(new PdfPoint(x, strikeoutY), new PdfPoint(x + lineWidth, strikeoutY));
				}
			}
		}
		internal double TransformX(float value) {
			return value / factorX;
		}
		internal double TransformY(float value) {
			return actualPageHeight - value / factorY;
		}
		protected abstract void AddNewCommands(IList<PdfCommand> sourceCommands, IList<PdfCommand> newCommands);
		PdfPoint TransformPoint(PointF point) {
			return new PdfPoint(point.X / factorX, actualPageHeight - point.Y / factorY);
		}
		void SetGraphicsStateParameters(PdfGraphicsStateParameters parameters) {
			commandConstructor.SetGraphicsStateParameters(parameters);
		}
		double GetActualPageHeight() {
			PdfTransformationMatrix currentTransform = commandConstructor.CurrentTransform;
			if (currentTransform.IsDefault || (currentTransform.A == 1 && currentTransform.B == 0 && currentTransform.C == 0 && currentTransform.D == 1))
				return bBox.Height;
			else {
				PdfRectangle cropBox = bBox;
				PdfTransformationMatrix transform = new PdfTransformationMatrix(currentTransform.A, currentTransform.B, currentTransform.C, currentTransform.D, 0, 0);
				transform = PdfTransformationMatrix.Invert(transform);
				PdfPoint[] points = new[] { cropBox.TopLeft, cropBox.BottomLeft };
				transform.TransformPoints(points);
				return PdfMathUtils.CalcDistance(points[0], points[1]);
			}
		}
		IList<PdfGraphicsPath> TransformPath(PointF[] pathPoints, byte[] pathTypes) {
			List<PdfGraphicsPath> paths = new List<PdfGraphicsPath>();
			PdfGraphicsPath current = null;
			for (int i = 0; i < pathPoints.Length; i++) {
				PdfPoint p1 = TransformPoint(pathPoints[i]);
				switch (pathTypes[i] & (byte)PathPointType.PathTypeMask) {
					case 0:
						current = new PdfGraphicsPath(p1);
						paths.Add(current);
						break;
					case 1:
						if (current == null)
							return paths;
						current.AppendLineSegment(p1);
						break;
					case 3:
						if (current == null)
							return paths;
						PdfPoint p2 = TransformPoint(pathPoints[++i]);
						PdfPoint p3 = TransformPoint(pathPoints[++i]);
						current.AppendBezierSegment(p1, p2, p3);
						break;
				}
				if ((pathTypes[i] & (byte)PathPointType.CloseSubpath) != 0 && current != null)
					current.Closed = true;
			}
			return paths;
		}
		PdfPoint[] TransformPoints(PointF[] points) {
			int count = points.Length;
			PdfPoint[] pdfPoints = new PdfPoint[count];
			for (int i = 0; i < count; i++)
				pdfPoints[i] = TransformPoint(points[i]);
			return pdfPoints;
		}
		bool CompareComponents(double[] first, double[] second) {
			if (first.Length != second.Length)
				return false;
			for (int i = 0; i < first.Length; i++) {
				if (first[i] != second[i])
					return false;
			}
			return true;
		}
		void SetColorForStrokingOperations(PdfTransparentColor color) {
			PdfGraphicsExportState current = graphicsStateStack.Current;
			if (color.Alpha != current.StrokingAlpha) {
				SetGraphicsStateParameters(strokingAlphaCache.GetParameters(Math.Round(color.Alpha, 4)));
				current.StrokingAlpha = color.Alpha;
			}
			if (current.StrokingColorComponents != null && CompareComponents(current.StrokingColorComponents, color.Components))
				return;
			commandConstructor.SetColorForStrokingOperations(color);
			current.StrokingColorComponents = color.Components;
		}
		void SetColorForNonStrokingOperation(PdfColor color) {
			PdfGraphicsExportState current = graphicsStateStack.Current;
			if (color.Pattern == null) {
				double[] colorComponents = color.Components;
				if (current.NonStrokingColorComponents != null && CompareComponents(current.NonStrokingColorComponents, color.Components))
					return;
				current.NonStrokingColorComponents = colorComponents;
			}
			else
				current.NonStrokingColorComponents = null;
			commandConstructor.SetColorForNonStrokingOperations(color);
		}
	}
}
