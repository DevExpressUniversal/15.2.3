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
using System.IO;
using System.Collections;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Data.Helpers;
#if SL 
using DevExpress.Xpf.Drawing;
using DevExpress.Xpf.Printing.Stubs;
using DevExpress.Xpf.Drawing.Drawing2D;
using System.Windows.Media;
using DevExpress.Xpf.Windows.Forms;
using DevExpress.Xpf.Collections;
using DevExpress.Xpf.Drawing.Imaging;
using DevExpress.Xpf.Security;
using Matrix = DevExpress.Xpf.Printing.Stubs.Matrix;
using Brush = DevExpress.Xpf.Drawing.Brush;
#else
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Collections.Generic;
using DevExpress.Pdf.Common;
#endif
namespace DevExpress.XtraPrinting.Export.Pdf {
	public class PdfGraphicsImpl {
		enum PdfGraphicsState {
			No, Pending, Opened
		}
		static bool IsPathClosed(GraphicsPath path) {
			PointF[] pathPoints = path.PathPoints;
			if(pathPoints.Length == 0) return false;
			return pathPoints[0] == pathPoints[pathPoints.Length - 1];
		}
		static RectangleF PointArrayToFramingRectangle(PointF[] pointArray) {
			if(pointArray == null)
				return new RectangleF(float.MinValue / 2, float.MinValue / 2, float.MaxValue, float.MaxValue);
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			float maxX = float.MinValue;
			float maxY = float.MinValue;
			foreach(PointF point in pointArray) {
				minX = Math.Min(minX, point.X);
				minY = Math.Min(minY, point.Y);
				maxX = Math.Max(maxX, point.X);
				maxY = Math.Max(maxY, point.Y);
			}
			return new RectangleF(minX, minY, maxX - minX, maxY - minY);
		}
		static PointF[] RectangleToPointArray(RectangleF rect) {
			PointF[] pointArray = new PointF[4];
			pointArray[0] = new PointF(rect.Left, rect.Top);
			pointArray[1] = new PointF(rect.Right, rect.Top);
			pointArray[2] = new PointF(rect.Right, rect.Bottom);
			pointArray[3] = new PointF(rect.Left, rect.Bottom);
			return pointArray;
		}
		#region PointArray2Rectangle
		static bool FindDelta(SizeF d, SizeF[] deltas) {
			foreach(SizeF delta in deltas)
				if(delta == d)
					return true;
			return false;
		}
		static bool IsRectangle(SizeF[] deltas, float width, float height) {
			if(width == 0 || height == 0)
				return false;
			if(!FindDelta(new SizeF(width, 0), deltas))
				return false;
			if(!FindDelta(new SizeF(0, height), deltas))
				return false;
			if(!FindDelta(new SizeF(width, height), deltas))
				return false;
			return true;
		}
		static RectangleF CreateRectangle(PointF basePoint, float width, float height) {
			PointF leftTop = basePoint;
			if(width < 0)
				leftTop.X += width;
			if(height < 0)
				leftTop.Y += height;
			return new RectangleF(leftTop, new SizeF(Math.Abs(width), Math.Abs(height)));
		}
		static SizeF[] CreateDeltas(PointF[] pointArray, out float width, out float height) {
			width = 0;
			height = 0;
			SizeF[] deltas = new SizeF[3];
			for(int i = 1; i < 4; i++) {
				float w = pointArray[i].X - pointArray[0].X;
				float h = pointArray[i].Y - pointArray[0].Y;
				if(w != 0 && width == 0)
					width = w;
				if(h != 0 && height == 0)
					height = h;
				deltas[i - 1] = new SizeF(w, h);
			}
			return deltas;
		}
		internal static RectangleF PointArray2Rectangle(PointF[] pointArray) {
			if(pointArray == null || pointArray.Length != 4)
				return RectangleF.Empty;
			float width, height;
			SizeF[] deltas = CreateDeltas(pointArray, out width, out height);
			return
				IsRectangle(deltas, width, height) ?
				CreateRectangle(pointArray[0], width, height) :
				RectangleF.Empty;
		}
		#endregion
		PdfDocument document;
		Matrix transform;
		Stack<Matrix> transformStack = new Stack<Matrix>();
		PointF[] clipArray;
		PdfGraphicsState pdfGraphicsState;
		PdfDrawContext context;
		SizeF size;
		IXObjectsOwner xObjectsOwner;
		IPdfDocumentOwner documentInfo;
		GraphicsUnit pageUnit;
		public GraphicsUnit PageUnit { get { return pageUnit; } set { ApplyPageUnitToClipArray(value); pageUnit = value; } }
		PdfImageCache ImageCache { get { return documentInfo.ImageCache; } }
		PdfNeverEmbeddedFonts NeverEmbeddedFonts { get { return documentInfo.NeverEmbeddedFonts; } }
		public bool ScaleStrings { get { return documentInfo.ScaleStrings; } }
		PdfDrawContext Context { get { return context; } }
		SizeF PageSizeInPoints { get { return size; } }
		public Matrix Transform { get { return transform; } }
#if DEBUGTEST
		public static bool Test_ProcessSimpleStings;
#endif
		public PdfGraphicsImpl(IPdfDocumentOwner documentInfo, PdfDrawContext context, SizeF size, PdfDocument document, IXObjectsOwner xObjectsOwner) {
			this.documentInfo = documentInfo;
			this.document = document;
			this.context = context;
			this.size = size;
			this.xObjectsOwner = xObjectsOwner;
			this.transform = new Matrix();
			pdfGraphicsState = PdfGraphicsState.No;
		}
		#region Values correction
		float TransformValue(float value) {
			return PdfCoordinate.TransformValue(PageUnit, value);
		}
		SizeF TransformValue(SizeF value) {
			return PdfCoordinate.TransformValue(PageUnit, value);
		}
		PointF TransformValue(PointF value) {
			return PdfCoordinate.TransformValue(PageUnit, value);
		}
		float BackTransformValue(float value) {
			return PdfCoordinate.BackTransformValue(PageUnit, value);
		}
		SizeF BackTransformValue(SizeF value) {
			return PdfCoordinate.BackTransformValue(PageUnit, value);
		}
		PointF BackTransformValue(PointF value) {
			return PdfCoordinate.BackTransformValue(PageUnit, value);
		}
		PointF CorrectPoint(PointF pt) {
			return PdfCoordinate.CorrectPoint(PageUnit, pt, PageSizeInPoints);
		}
		RectangleF CorrectRectangle(RectangleF rect) {
			return PdfCoordinate.CorrectRectangle(PageUnit, rect, PageSizeInPoints);
		}
		PointF BackTransformPoint(PointF pt) {
			PointF[] pp = new PointF[] { pt };
			using(Matrix transform = this.transform.Clone()) {
				transform.Invert();
				transform.TransformPoints(pp);
				return pp[0];
			}
		}
		#endregion
		#region Clipping
		void ApplyPageUnitToClipArray(GraphicsUnit newPageUnit) {
			if(this.clipArray == null)
				return;
			float c = GraphicsDpi.UnitToDpiI(newPageUnit) / GraphicsDpi.UnitToDpiI(pageUnit);
			using(Matrix transform = this.transform.Clone())
			using(Matrix m = new Matrix(c, 0, 0, c, 0, 0)) {
				m.Multiply(transform, MatrixOrder.Prepend);
				transform.Invert();
				m.Multiply(transform, MatrixOrder.Append);
				m.TransformPoints(this.clipArray);
			}
		}
		void ApplyTransformCacheToClipArray() {
			if(this.clipArray == null)
				return;
			using(Matrix transform = this.transform.Clone()) {
				transform.Invert();
				transform.TransformPoints(this.clipArray);
			}
		}
		void ResetClip() {
			if(this.clipArray == null)
				return;
			using(Matrix transform = this.transform.Clone()) {
				transform.TransformPoints(this.clipArray);
			}
		}
		void DoPolygonClip() {
			PointF[] array = new PointF[this.clipArray.Length];
			this.clipArray.CopyTo(array, 0);
			for(int i = 0; i < array.Length; i++)
				array[i] = CorrectPoint(array[i]);
			if(array.Length > 0) {
				Context.MoveTo(array[0].X, array[0].Y);
				for(int i = 1; i < array.Length; i++)
					Context.LineTo(array[i].X, array[i].Y);
				Context.LineTo(array[0].X, array[0].Y);
			}
		}
		void DoRectangleClip(RectangleF clipRect) {
			PdfGraphicsRectangle pdfClipRect = CreatePdfGraphicsRectangle(clipRect);
			Context.Rectangle(pdfClipRect.Left, pdfClipRect.Bottom, pdfClipRect.Width, pdfClipRect.Height);
		}
		void DoClip() {
			if(this.clipArray != null) {
				RectangleF clipRect = PointArray2Rectangle(this.clipArray);
				if(clipRect.IsEmpty)
					DoPolygonClip();
				else
					DoRectangleClip(clipRect);
				Context.Clip();
				Context.NewPath();
			}
		}
		PointF[] ApplyClip(RectangleF bounds, StringFormat format) {
			PointF[] clipArrayOld = this.clipArray;
			bool needClip = !((format.FormatFlags & StringFormatFlags.NoClip) != 0);
			if(needClip) {
				RectangleF clipRect = PointArray2Rectangle(this.clipArray);
				if(!clipRect.IsEmpty)
					ClipBounds = RectangleF.Intersect(clipRect, bounds);
			}
			return clipArrayOld;
		}
		#endregion
		Pen TestPen(Pen pen) {
			return (pen != null) ? pen : new Pen(DXColor.Black);
		}
		PdfGraphicsRectangle CreatePdfGraphicsRectangle(RectangleF rect) {
			RectangleF rectInPoints = new RectangleF(TransformValue(rect.Location), TransformValue(rect.Size));
			return new PdfGraphicsRectangle(rectInPoints, PageSizeInPoints);
		}
		static PdfLineCapStyle GetLineCapStyle(LineCap lineCap) {
			switch(lineCap) {
				case LineCap.Square:
					return PdfLineCapStyle.ProtectingSquare;
				case LineCap.Round:
					return PdfLineCapStyle.Round;
				default:
					return PdfLineCapStyle.Butt;
			}
		}
		static PdfLineCapStyle GetLineCapStyle(DashCap dashCap) {
			switch(dashCap) {
				case DashCap.Round:
					return PdfLineCapStyle.Round;
				default:
					return PdfLineCapStyle.Butt;
			}
		}
		static PdfLineJoinStyle GetLineJoinStyle(LineJoin lineJoin) {
			switch(lineJoin) {
				case LineJoin.Bevel:
					return PdfLineJoinStyle.Bevel;
				case LineJoin.Round:
					return PdfLineJoinStyle.Round;
				default:
					return PdfLineJoinStyle.Miter;
			}
		}
		float[] GetDashArray(Pen pen, float width) {
			float[] result = new float[pen.DashPattern.Length];
			width = (width == 0 ? 1 : width);
			for(int i = 0; i < result.Length; i++)
				result[i] = pen.DashPattern[i] * width;
			return result;
		}
		void ConstructEllipse(float x, float y, float w, float h) {
			Context.MoveTo(x, y + h / 2);
			Context.CurveTo(x,
				y + h / 2 - h / 2 * 11 / 20,
				x + w / 2 - w / 2 * 11 / 20,
				y,
				x + w / 2,
				y);
			Context.CurveTo(x + w / 2 + w / 2 * 11 / 20,
				y,
				x + w,
				y + h / 2 - h / 2 * 11 / 20,
				x + w,
				y + h / 2);
			Context.CurveTo(x + w,
				y + h / 2 + h / 2 * 11 / 20,
				x + w / 2 + w / 2 * 11 / 20,
				y + h,
				x + w / 2,
				y + h);
			Context.CurveTo(x + w / 2 - w / 2 * 11 / 20,
				y + h,
				x,
				y + h / 2 + h / 2 * 11 / 20,
				x,
				y + h / 2);
		}
		void ConstructLineParams(Pen pen) {
			Color color = ApplyTransparency(pen.Color);
			Context.SetRGBStrokeColor(color);
			float width = TransformValue(pen.Width);
			Context.SetLineWidth(width);
			if(pen.DashStyle != DashStyle.Solid)
				Context.SetDash(GetDashArray(pen, width), 0);
			PdfLineCapStyle pdfCapStyle = pen.DashStyle != DashStyle.Solid ? GetLineCapStyle(pen.DashCap) : GetLineCapStyle(pen.StartCap);
			if(pdfCapStyle != PdfLineCapStyle.Butt)
				Context.SetLineCap(pdfCapStyle);
			PdfLineJoinStyle pfgJoinStyle = GetLineJoinStyle(pen.LineJoin);
			if(pfgJoinStyle != PdfLineJoinStyle.Miter)
				Context.SetLineJoin(pfgJoinStyle);
		}
		string RemoveChar(string str, char ch) {
			for(int i = str.Length - 1; i >= 0; i--)
				if(str[i] == ch)
					str = str.Remove(i, 1);
			return str;
		}
		float GetTabStopInterval(StringFormat stringFormat) {
			float firstTabOffset;
			float[] tabStops = stringFormat.GetTabStops(out firstTabOffset);
			if(tabStops.Length > 0)
				return TransformValue(tabStops[0]);
			return 0;
		}
		RectangleF CutBoundsByPage(RectangleF bounds) {
			PointF size = BackTransformPoint(BackTransformValue(PageSizeInPoints).ToPointF());
			if(bounds.Right > size.X)
				bounds.Width = Math.Abs(size.X - bounds.X);
			if(bounds.Bottom > size.Y)
				bounds.Height = Math.Abs(size.Y - bounds.Y);
			return new RectangleF(bounds.Location, bounds.Size);
		}
		RectangleF PointToBounds(PointF point) {
			return CutBoundsByPage(new RectangleF(point, new SizeF(float.MaxValue, float.MaxValue)));
		}
		static StringAlignment ReverseAlignment(StringAlignment alignment) {
			switch(alignment) {
				case StringAlignment.Near:
					return StringAlignment.Far;
				case StringAlignment.Far:
					return StringAlignment.Near;
				default:
					return alignment;
			}
		}
		static StringFormat PrepareStringFormat(StringFormat format) {
			if(format == null)
				return (StringFormat)StringFormat.GenericTypographic.Clone();
			StringFormat actualFormat = (StringFormat)format.Clone();
			StringAlignment alignment = actualFormat.Alignment;
			StringAlignment lineAlignment = actualFormat.LineAlignment;
			bool directionVertical = (actualFormat.FormatFlags & StringFormatFlags.DirectionVertical) != 0;
			bool directionRightToLeft = (actualFormat.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0;
			if(directionVertical) {
				actualFormat.Alignment = lineAlignment;
				actualFormat.LineAlignment = alignment;
			}
			if(directionRightToLeft)
				actualFormat.Alignment = ReverseAlignment(actualFormat.Alignment);
			return actualFormat;
		}
		void DrawPathInternal(GraphicsPath path) {
			PathData pathData = path.PathData;
			for(int i = 0; i < pathData.Points.Length; i++) {
				PointF p1 = CorrectPoint(pathData.Points[i]);
				switch(pathData.Types[i] & (byte)PathPointType.PathTypeMask) {
					case 0:
						Context.MoveTo(p1.X, p1.Y);
						break;
					case 1:
						Context.LineTo(p1.X, p1.Y);
						break;
					case 3:
						PointF p2 = CorrectPoint(pathData.Points[++i]);
						PointF p3 = CorrectPoint(pathData.Points[++i]);
						Context.CurveTo(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);
						break;
				}
				if((pathData.Types[i] & (byte)PathPointType.CloseSubpath) != 0)
					Context.ClosePath();
			}
		}
		void DrawStringCore(string[] lines, Font actualFont, PdfFont pdfFont, SolidBrush solidBrush, PdfGraphicsRectangle pdfRect, StringFormat actualFormat) {
			PdfTextLines pdfLines = new PdfTextLines(lines, actualFormat);
			float lineSpacing = FontHelper.GetLineSpacing(actualFont);
			float ascent = FontHelper.GetCellAscent(actualFont);
			float descent = FontHelper.GetCellDescent(actualFont);
			float textHeight = (lines.Length - 1) * lineSpacing + ascent + descent;
			bool oblique = actualFont.Italic && pdfFont.TTFFile != null && pdfFont.TTFFile.Post != null && pdfFont.TTFFile.Post.ItalicAngle == 0;
			bool emulateBold = pdfFont.EmulateBold(document.Fonts);
			FontLines fontLines = new FontLines(actualFont, ascent, descent);
			TextLocation textLocation = TextLocation.CreateInstance(actualFormat);
			int startLine, endLine;
			float yPos = GetBaseYAndLimits(textLocation.CalculateTextY(pdfRect, textHeight), lineSpacing, pdfLines.Count, out startLine, out endLine) - ascent;
			if(endLine - startLine < 0)
				return;
			ApplyPendingTransform();
			Context.GSave();
			DoClip();
			Color foreColor = ApplyTransparency(solidBrush.Color);
			Context.BeginText();
			Context.SetRGBFillColor(foreColor);
			Context.SetRGBStrokeColor(foreColor);
			Context.SetFontAndSize(pdfFont, actualFont);
			if(!oblique)
				Context.MoveTextPoint(0, yPos);
			if(emulateBold)
				SetBoldEmulation(FontSizeHelper.GetSizeInPoints(actualFont));
			float tabStopInterval = GetTabStopInterval(actualFormat);
			using(TextProcessor textProcessor = new TextProcessor(Context, tabStopInterval, actualFont, IsRtl(actualFormat))) {
				for(int i = startLine; i <= endLine; i++) {
					string line = pdfLines[i];
					float textWidth = textProcessor.GetTextWidth(line);
					float xPos = textLocation.CalculateTextX(pdfRect, textWidth);
					fontLines.AddLines(xPos, yPos - (lineSpacing * (i - startLine)), textWidth);
					float xScale = ScaleStrings && textWidth - pdfRect.Width > 0.01f ? pdfRect.Width / textWidth : 1f;
					if(oblique) {
						Context.SetTextMatrix(xScale, 0, 0.3333f, 1, xPos, yPos);
						yPos -= lineSpacing;
					} else {
						if(xScale < 1f) {
							Context.SetTextMatrix(xScale, 0, 0, 1, xPos, yPos);
						} else
							Context.MoveTextPoint(xPos, 0);
					}
					textProcessor.ShowText(line);
					if(!oblique && xScale >= 1f) {
						Context.MoveTextPoint(-xPos, -lineSpacing);
					}
				}
			}
			Context.EndText();
			fontLines.DrawContents(Context, solidBrush.Color);
			Context.GRestore();
		}
		float GetBaseYAndLimits(float yBase, float lineSpacing, int lineCount, out int startLine, out int endLine) {
			return GetBaseYAndLimits(yBase, lineSpacing, lineCount, out startLine, out endLine, CreatePdfGraphicsRectangle(this.ClipBounds));
		}
		static float GetBaseYAndLimits(float yBase, float lineSpacing, int lineCount, out int startLine, out int endLine, PdfGraphicsRectangle clipBounds) {
			startLine = CalculateStartLine(yBase, lineSpacing, clipBounds);
			endLine = Math.Min(CalculateEndLine(yBase, lineSpacing, clipBounds), lineCount - 1);
			return yBase - startLine * lineSpacing;
		}
		static int CalculateStartLine(float yBase, float lineSpacing, PdfGraphicsRectangle clipBounds) {
			double startLineF = (yBase - clipBounds.Top) / lineSpacing + 0.1;
			double result = Math.Floor(Math.Max(0, startLineF));
			return (int)result;
		}
		static int CalculateEndLine(float yBase, float lineSpacing, PdfGraphicsRectangle clipBounds) {
			double endLineF = (yBase - clipBounds.Bottom) / lineSpacing - 0.1;
			double result = Math.Floor(endLineF);
			return (int)result;
		}
#if DEBUGTEST
		public static float Test_GetBaseYAndLimits(float yBase, float lineSpacing, int lineCount, out int startLine, out int endLine, RectangleF clipBounds, float pageHeight) {
			return GetBaseYAndLimits(yBase, lineSpacing, lineCount, out startLine, out endLine, new PdfGraphicsRectangle(clipBounds, new SizeF(pageHeight, pageHeight)));
		}
#endif
		static bool IsRtl(StringFormat actualFormat) {
			return (actualFormat.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0;
		}
		void ApplyShading(Color startColor, Color endColor) {
			PdfShading shading = document.Shading.CreateAddUnique(startColor, endColor);
			xObjectsOwner.AddShading(shading);
			Context.Shading(shading);
		}
		void ApplyPattern(Color foreColor, Color backColor) {
			PdfPattern pattern = document.Patterns.CreateAddUnique(foreColor, backColor);
			xObjectsOwner.AddPattern(pattern);
			Context.Pattern(pattern);
		}
		Color ApplyTransparency(Color color) {
			if(color.A == 255)
				return color;
			SetTransparency(color.A);
			return Color.FromArgb(255, color);
		}
		void SetTransparency(int transparency) {
			PdfTransparencyGS gs = document.TransparencyGS.CreateAddUnique(transparency);
			xObjectsOwner.AddTransparencyGS(gs);
			Context.ExecuteGraphicsState(gs);
		}
		void SetBoldEmulation(float fontSize) {
			Context.SetRenderingMode(PdfTextRenderingMode.FillThenStroke);
			Context.SetLineWidth(fontSize / 30);
		}
		void DrawPdfImage(PdfImageBase pdfImage, RectangleF bounds) {
			RectangleF clipBounds = RectangleF.Intersect(ClipBounds, bounds);
			RectangleF correctedBounds = CorrectRectangle(bounds);
			RectangleF correctedClipBounds = CorrectRectangle(clipBounds);
			ApplyPendingTransform();
			Context.GSave();
			Context.Rectangle(correctedClipBounds.X, correctedClipBounds.Y, correctedClipBounds.Width, correctedClipBounds.Height);
			Context.Clip();
			Context.NewPath();
			Context.Concat(pdfImage.Transform(correctedBounds));
			Context.ExecuteXObject(pdfImage);
			Context.GRestore();
		}
		bool CanApplyImageBackgroundColor(Image image) {
			return
				!(image.RawFormat.Equals(ImageFormat.Wmf) && !document.ConvertImagesToJpeg) &&
				!(image.RawFormat.Equals(ImageFormat.Emf) && !document.ConvertImagesToJpeg) &&
				!image.RawFormat.Equals(ImageFormat.Jpeg) &&
				!(image.RawFormat.Equals(ImageFormat.Gif) && !document.ConvertImagesToJpeg) &&
				image.PixelFormat != PixelFormat.Format24bppRgb &&
				image.PixelFormat != PixelFormat.Format1bppIndexed &&
				image.PixelFormat != PixelFormat.Format32bppArgb;
		}
		Color GetActualBackColor(Image image, Color underlyingColor) {
			return CanApplyImageBackgroundColor(image) && !DXColor.IsEmpty(underlyingColor) ?
				Color.FromArgb(255, underlyingColor.R, underlyingColor.G, underlyingColor.B) :
				DXColor.Empty;
		}
		PdfImageBase GetPdfImage(Image image, Color underlyingColor) {
			Color actualBackColor = GetActualBackColor(image, underlyingColor);
			PdfImageCache.Params imageParams = new PdfImageCache.Params(image, actualBackColor);
			PdfImageBase pdfImage = this.ImageCache[imageParams];
			if(pdfImage == null) {
				pdfImage = document.CreatePdfImage(documentInfo, xObjectsOwner, image, actualBackColor);
				this.ImageCache.AddPdfImage(pdfImage, imageParams);
			} else
				xObjectsOwner.AddExistingXObject(pdfImage);
			return pdfImage;
		}
		void AddPendingTransform() {
			CloseTransform();
			if(!this.transform.IsIdentity)
				this.pdfGraphicsState = PdfGraphicsState.Pending;
		}
		void ApplyPendingTransform() {
			if(this.pdfGraphicsState == PdfGraphicsState.Pending) {
				Context.GSave();
				ApplyTransform();
				this.pdfGraphicsState = PdfGraphicsState.Opened;
			}
		}
		void ApplyTransform() {
			if(this.transform.IsIdentity)
				return;
			using(Matrix result = this.transform.Clone()) {
				TransformActivator activator = new TransformActivator(result, this.PageUnit, this.size);
				activator.ApplyToDrawContext(this.context);
			}
		}
		void CloseTransform() {
			if(pdfGraphicsState == PdfGraphicsState.Opened) {
				Context.GRestore();
				this.pdfGraphicsState = PdfGraphicsState.No;
			}
		}
		public float Dpi {
			get { return GraphicsDpi.Point; }
		}
		public RectangleF ClipBounds {
			get { return PointArrayToFramingRectangle(clipArray); }
			set { clipArray = RectangleToPointArray(value); }
		}
		private void SetClip(PointF[] points, CombineMode combineMode) {
		}
		public void SetClip(GraphicsPath path, CombineMode combineMode) {
			SetClip(new Region(path), combineMode);
		}
		public void SetClip(Region region, CombineMode combineMode) {
			Region oldClipBounds = clipArray != null ? new Region(GetPathForPoints(clipArray)) : new Region();
			switch(combineMode) {
				case CombineMode.Complement:
					oldClipBounds.Complement(region);
					break;
				case CombineMode.Exclude:
					oldClipBounds.Exclude(region);
					break;
				case CombineMode.Intersect:
					oldClipBounds.Intersect(region);
					break;
				case CombineMode.Replace:
					oldClipBounds = region;
					break;
				case CombineMode.Union:
					oldClipBounds.Union(region);
					break;
				case CombineMode.Xor:
					oldClipBounds.Xor(region);
					break;
			}
			RectangleF clipRect = oldClipBounds.GetBounds(Graphics.FromHwnd(IntPtr.Zero));
			clipArray = RectangleToPointArray(clipRect);
			if(clipRect.IsEmpty) {
				clipArray = null;
			}
		}
		static GraphicsPath GetPathForPoints(PointF[] points) {
			GraphicsPath gp = new GraphicsPath();
			gp.AddLines(points);
			return gp;
		}
		public void SetClip(RectangleF rect, CombineMode combineMode) {
			SetClip(new Region(rect), combineMode);
		}
		public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format) {
			RectangleF bounds;
			if(format != null && (format.Alignment != StringAlignment.Near || format.LineAlignment != StringAlignment.Near)) {
				SizeF size = this.documentInfo.Measurer.MeasureString(s, font, float.MaxValue, format, PageUnit);
				bounds = new RectangleF(point.X - GetOffset(format.Alignment, size.Width), point.Y - GetOffset(format.LineAlignment, size.Height), size.Width, size.Height);
			} else {
				bounds = PointToBounds(point);
			}
			DrawString(s, font, brush, bounds, format);
		}
		static float GetOffset(StringAlignment alignment, float size) {
			switch(alignment) {
				case StringAlignment.Near:
					return 0;
				case StringAlignment.Center:
					return size / 2;
				default:
					return size;
			}
		}
		public void DrawString(string s, Font font, Brush brush, RectangleF bounds, StringFormat format) {
			if(s == null)
				throw new ArgumentNullException("s");
			if(font == null)
				throw new ArgumentNullException("font");
			if(brush == null)
				throw new ArgumentNullException("brush");
			SolidBrush solidBrush = brush as SolidBrush;
			if(solidBrush == null)
				throw new PdfGraphicsException("The brush must be solid");
			if(solidBrush.Color.A == 0)
				return;
			if(bounds.IsEmpty)
				return;
			StringFormat actualFormat = PrepareStringFormat(format);
			s = HotkeyPrefixHelper.PreprocessHotkeyPrefixesInString(s, actualFormat.HotkeyPrefix);
			PointF[] clipArrayOld = ApplyClip(bounds, actualFormat);
			try {
				if(ClipBounds.IsEmpty)
					return;
				string actualString = RemoveChar(s, '\r');
				Font newFont = null;
				PdfFont pdfFont = document.RegisterFontSmart(font, ref actualString, ref newFont);
				Font actualFont = newFont == null ? font : newFont;
				pdfFont.NeedToEmbedFont = !this.NeverEmbeddedFonts.FindFont(actualFont);
				PdfTextRotation textRotation = PdfTextRotation.CreateInstance(this, actualFormat);
				try {
					PdfTextRotation.BeginRotation(textRotation, bounds);
					try {
						string[] lines = null;
						GraphicsBase.EnsureStringFormat(font, bounds, PageUnit, actualFormat, validFormat => {
							lines = new TextFormatter(PageUnit, documentInfo.Measurer).FormatMultilineText(actualString, actualFont, bounds.Width, bounds.Height, validFormat);
						});
						if(lines.Length == 0) return;
						DrawStringCore(lines, actualFont, pdfFont, solidBrush, CreatePdfGraphicsRectangle(bounds), actualFormat);
					} finally {
						actualFormat.Dispose();
						if(newFont != null) newFont.Dispose();
					}
				} finally {
					PdfTextRotation.EndRotation(textRotation);
				}
			} finally {
				this.clipArray = clipArrayOld;
			}
		}
		public void DrawLine(Pen pen, PointF pt1, PointF pt2) {
			DrawLines(pen, new PointF[] { pt1, pt2 });
		}
		public void DrawLines(Pen pen, PointF[] points) {
			if(pen.Color.A == 0)
				return;
			if(points.Length < 2)
				return;
			ApplyPendingTransform();
			Context.GSave();
			DoClip();
			DrawStartCap(ref points[0], points[1], pen);
			DrawEndCap(ref points[points.Length - 1], points[points.Length - 2], pen);
			ConstructLineParams(TestPen(pen));
			PointF pt = CorrectPoint(points[0]);
			Context.MoveTo(pt.X, pt.Y);
			int count = points.Length;
			for(int i = 1; i < count; i++) {
				pt = CorrectPoint(points[i]);
				Context.LineTo(pt.X, pt.Y);
			}
			Context.Stroke();
			Context.GRestore();
		}
		public void DrawBeziers(Pen pen, PointF[] points) {
			if(pen.Color.A == 0)
				return;
			ApplyPendingTransform();
			Context.GSave();
			DoClip();
			ConstructLineParams(TestPen(pen));
			PointF pt = CorrectPoint(points[0]);
			Context.MoveTo(pt.X, pt.Y);
			int count = points.Length;
			for(int i = 1; i < count; i += 3) {
				pt = CorrectPoint(points[i]);
				PointF pt2 = CorrectPoint(points[i + 1]);
				PointF pt3 = CorrectPoint(points[i + 2]);
				Context.CurveTo(pt.X, pt.Y, pt2.X, pt2.Y, pt3.X, pt3.Y);
			}
			Context.Stroke();
			Context.GRestore();
		}
		public void DrawImage(Image image, RectangleF bounds, Color underlyingColor) {
			if(bounds.Width <= 0 || bounds.Height <= 0)
				return;
			if(image == null)
				return;
			PdfImageBase pdfImage = GetPdfImage(image, underlyingColor);
			DrawPdfImage(pdfImage, bounds);
		}
		public void DrawImage(Image image, RectangleF bounds, RectangleF sourceRect, Color underlyingColor) {
			if(bounds.Width <= 0 || bounds.Height <= 0)
				return;
			if(image == null)
				return;
			DrawImage(image, bounds, underlyingColor);
		}
		public void DrawRectangle(Pen pen, RectangleF bounds) {
			if(pen.Color.A == 0)
				return;
			bounds = CorrectRectangle(bounds);
			ApplyPendingTransform();
			Context.GSave();
			DoClip();
			ConstructLineParams(TestPen(pen));
			Context.Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
			Context.ClosePathAndStroke();
			Context.GRestore();
		}
		public void FillRectangle(Brush brush, RectangleF bounds) {
			Action makeFigure = () => {
				bounds = CorrectRectangle(bounds);
				Context.Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
			};
			FillFigure(brush, makeFigure);
		}
		public void DrawPath(Pen pen, GraphicsPath path) {
			if(pen.Color.A == 0)
				return;
			ApplyPendingTransform();
			Context.GSave();
			DoClip();
			ConstructLineParams(TestPen(pen));
			DrawPathInternal(path);
			if(IsPathClosed(path))
				Context.ClosePathAndStroke();
			else
				Context.Stroke();
			Context.GRestore();
		}
		public void FillPath(Brush brush, GraphicsPath path) {
			FillFigure(brush, () => DrawPathInternal(path));
		}
		public void DrawEllipse(Pen pen, RectangleF rect) {
			if(pen.Color.A == 0)
				return;
			rect = CorrectRectangle(rect);
			ApplyPendingTransform();
			Context.GSave();
			DoClip();
			ConstructLineParams(TestPen(pen));
			ConstructEllipse(rect.X, rect.Y, rect.Width, rect.Height);
			Context.ClosePathAndStroke();
			Context.GRestore();
		}
		public void FillEllipse(Brush brush, RectangleF rect) {
			Action makeFigure = () => {
				rect = CorrectRectangle(rect);
				ConstructEllipse(rect.X, rect.Y, rect.Width, rect.Height);
			};
			FillFigure(brush, makeFigure);
		}
		void FillFigure(Brush brush, Action makeFigure) {
			Color color = Color.Black;
			FillAction action = GetFillAction(brush, ref color);
			if(action == FillAction.None)
				return;
			ApplyPendingTransform();
			Context.GSave();
			DoClip();
			switch(action) {
				case FillAction.Solid:
					FillSolid(color, makeFigure);
					break;
				case FillAction.Hatch:
					FillHatch(brush , makeFigure);
					break;
				case FillAction.Gradient:
					FillGradient(brush, makeFigure);
					break;
			}
			Context.GRestore();
		}
		enum FillAction { 
			None,
			Solid,
			Hatch,
			Gradient,
		}
		FillAction GetFillAction(Brush brush, ref Color color) {
			if(brush == null) {
				color = Color.Black;
				return FillAction.Solid;
			}
			if(brush is SolidBrush) {
				color = ((SolidBrush)brush).Color;
				return color.A != 0 ? FillAction.Solid : FillAction.None;
			} else if(brush is HatchBrush) {
				HatchBrush hatchBrush = ((HatchBrush)brush);
				if(hatchBrush.HatchStyle == HatchStyle.WideUpwardDiagonal)
					return FillAction.Hatch;
				color = hatchBrush.BackgroundColor;
				return color.A != 0 ? FillAction.Solid : FillAction.None;
			} else if(brush is LinearGradientBrush) {
				return FillAction.Gradient;
			}
			return FillAction.None;
		}
		void FillHatch(Brush brush, Action makeFigure) {
			HatchBrush hatchBrush = brush as HatchBrush;
			ApplyPattern(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
			makeFigure();
			Context.Fill();
		}
		void FillGradient(Brush brush, Action makeFigure) {
			LinearGradientBrush gradientBrush = brush as LinearGradientBrush;
			makeFigure();
			Context.Clip();
			Context.NewPath();
			RectangleF gradientRect = gradientBrush.Rectangle;
			PointF[] gradientPoints = new PointF[] { gradientRect.Location, new PointF(gradientRect.Right, gradientRect.Top), new PointF(gradientRect.Left, gradientRect.Bottom) };
			gradientBrush.Transform.TransformPoints(gradientPoints);
			for(int i = 0; i < gradientPoints.Length; i++)
				gradientPoints[i] = CorrectPoint(gradientPoints[i]);
			using(Matrix shadingMatrix = new Matrix(new RectangleF(0, 0, 1, gradientRect.Height / gradientRect.Width), gradientPoints)) {
				Context.Concat(shadingMatrix);
			}
			ApplyShading(gradientBrush.LinearColors[0], gradientBrush.LinearColors[gradientBrush.LinearColors.Length - 1]);
		}
		void FillSolid(Color fillColor, Action makeFigure) {
			fillColor = ApplyTransparency(fillColor);
			Context.SetRGBFillColor(fillColor);
			makeFigure();
			Context.Fill();
		}
		public void ResetTransform() {
			ResetClip();
			this.transform.Reset();
			AddPendingTransform();
		}
		void AddTransform(Matrix matrix, MatrixOrder order) {
			ResetClip();
			this.transform.Multiply(matrix, order);
			AddPendingTransform();
			ApplyTransformCacheToClipArray();
		}
		public void SetTransform(Matrix matrix) {
			ResetClip();
			this.transform.Reset();
			this.transform.Multiply(matrix);
			AddPendingTransform();
			ApplyTransformCacheToClipArray();
		}
		public void RotateTransform(float angle, MatrixOrder order) {
			Matrix matrix = new Matrix();
			matrix.Rotate(angle);
			AddTransform(matrix, order);
		}
		public void ScaleTransform(float sx, float sy, MatrixOrder order) {
			Matrix matrix = new Matrix();
			matrix.Scale(sx, sy);
			AddTransform(matrix, order);
		}
		public void TranslateTransform(float dx, float dy, MatrixOrder order) {
			Matrix matrix = new Matrix();
			matrix.Translate(dx, dy);
			AddTransform(matrix, order);
		}
		public void MultiplyTransform(Matrix matrix, MatrixOrder order) {
			AddTransform(matrix, order);
		}
		public void SaveTransformState() {
			this.transformStack.Push(this.transform.Clone());
		}
		public void ApplyTransformState(MatrixOrder order, bool removeState) {
			Matrix transform = removeState ? this.transformStack.Pop() : this.transformStack.Peek();
			AddTransform(transform, order);
			if(removeState)
				transform.Dispose();
		}
		public void Finish() {
			CloseTransform();
		}
		void DrawStartCap(ref PointF start, PointF end, Pen pen) {
			if(pen == null)
				return;
			DrawCap(ref start, end, pen.Width, pen.StartCap, pen.StartCap == LineCap.Custom ? pen.CustomStartCap : null, pen.Color);
		}
		void DrawEndCap(ref PointF start, PointF end, Pen pen) {
			if(pen == null)
				return;
			DrawCap(ref start, end, pen.Width, pen.EndCap, pen.EndCap == LineCap.Custom ? pen.CustomEndCap : null, pen.Color);
		}
		void DrawCap(ref PointF start, PointF end, float width, LineCap cap, CustomLineCap customCap, Color color) {
			LineCapHelper.LineCapDrawInfo capInfo = LineCapHelper.MakeCapInfo(start, end, width, cap, customCap);
			if(capInfo == null || capInfo.path == null)
				return;
			start = capInfo.start;
			color = ApplyTransparency(color);
			if(capInfo.fill) {
				Context.SetRGBFillColor(color);
				DrawPathInternal(capInfo.path);
				Context.Fill();
			} else {
				Context.SetRGBStrokeColor(color);
				Context.SetLineWidth(TransformValue(width));
				DrawPathInternal(capInfo.path);
				Context.Stroke();
			}
			capInfo.path.Dispose();
		}
	}
	public sealed class TextUtils {
		const char charTab = '\t';
		public static string[] GetTabbedPieces(string source) {
			return source.Split(charTab);
		}
		TextUtils() {
		}
	}
	public class TextProcessor : PdfTextMeasuringProcessor {
		PdfDrawContext context;
		public TextProcessor(PdfDrawContext context, float tabStopInterval, Font font, bool rtl)
			: base(context, tabStopInterval, font, rtl) {
			this.context = context;
		}
		public void ShowText(string source) {
			string[] tabbedPieces = TextUtils.GetTabbedPieces(source);
			if(Rtl)
				Array.Reverse(tabbedPieces);
			if(TabStopInterval > 0 && tabbedPieces.Length > 1)
				ShowTextWithTabs(tabbedPieces);
			else
				ShowTextWithoutTabs(tabbedPieces);
		}
		void ShowTextWithTabs(string[] tabbedPieces) {
			int totalTabStopCount = 0;
			foreach(string tabbedPiece in tabbedPieces) {
				ShowTextInternal(tabbedPiece);
				float pieceWidth = GetSimpleTextWidth(tabbedPiece);
				int tabStopCount = (int)(pieceWidth / TabStopInterval) + 1;
				this.context.MoveTextPoint(TabStopInterval * tabStopCount, 0);
				totalTabStopCount += tabStopCount;
			}
			context.MoveTextPoint(-TabStopInterval * totalTabStopCount, 0);
		}
		void ShowTextWithoutTabs(string[] tabbedPieces) {
			foreach(string tabbedPiece in tabbedPieces) {
				ShowTextInternal(tabbedPiece);
			}
		}
		void ShowTextInternal(string text) {
			this.context.ShowText(GetTextRun(text));
		}
	}
	public class PdfTextRotation {
		const float angle = 90;
		public static PdfTextRotation CreateInstance(PdfGraphicsImpl gr, StringFormat format) {
			bool directionVertical = (format.FormatFlags & StringFormatFlags.DirectionVertical) != 0;
			if(directionVertical)
				return new PdfTextRotation(gr, format.LineAlignment, format.Alignment);
			else
				return null;
		}
		public static void BeginRotation(PdfTextRotation rotation, RectangleF bounds) {
			if(rotation != null)
				rotation.BeginRotation(bounds);
		}
		public static void EndRotation(PdfTextRotation rotation) {
			if(rotation != null)
				rotation.EndRotation();
		}
		PdfGraphicsImpl gr;
		StringAlignment lineAlignment;
		StringAlignment alignment;
		PdfTextRotation(PdfGraphicsImpl gr, StringAlignment lineAlignment, StringAlignment alignment) {
			this.gr = gr;
			this.lineAlignment = lineAlignment;
			this.alignment = alignment;
		}
		void ApplyTransform(float x, float y) {
			this.gr.TranslateTransform(-x, -y, MatrixOrder.Append);
			this.gr.RotateTransform(angle, MatrixOrder.Append);
			this.gr.TranslateTransform(x, y, MatrixOrder.Append);
		}
		float CalculateCenterX(RectangleF bounds) {
			switch(this.alignment) {
				case StringAlignment.Near:
					return bounds.Left;
				case StringAlignment.Center:
					return bounds.Left + bounds.Width / 2;
				case StringAlignment.Far:
					return bounds.Right;
				default:
					throw new ArgumentException();
			}
		}
		float CalculateCenterY(RectangleF bounds) {
			switch(this.lineAlignment) {
				case StringAlignment.Near:
					return bounds.Top;
				case StringAlignment.Center:
					return bounds.Top + bounds.Height / 2;
				case StringAlignment.Far:
					return bounds.Bottom;
				default:
					throw new ArgumentException();
			}
		}
		void BeginRotation(RectangleF bounds) {
			this.gr.SaveTransformState();
			float x = CalculateCenterX(bounds);
			float y = CalculateCenterY(bounds);
			ApplyTransform(x, y);
		}
		void EndRotation() {
			this.gr.ResetTransform();
			this.gr.ApplyTransformState(MatrixOrder.Append, true);
		}
	}
	public class PdfTextLines : IEnumerable {
		#region inner classes
		public abstract class Enumerator : IEnumerator {
			PdfTextLines lines;
			int index;
			protected PdfTextLines Lines { get { return lines; } }
			protected Enumerator(PdfTextLines lines) {
				this.lines = lines;
				((IEnumerator)this).Reset();
			}
			object IEnumerator.Current { get { return this.lines[this.index]; } }
			void IEnumerator.Reset() {
				ResetIndex(ref this.index);
			}
			bool IEnumerator.MoveNext() {
				return NextIndex(ref index);
			}
			protected abstract void ResetIndex(ref int index);
			protected abstract bool NextIndex(ref int index);
		}
		public class ForwardEnumerator : Enumerator {
			public ForwardEnumerator(PdfTextLines lines)
				: base(lines) {
			}
			protected override void ResetIndex(ref int index) {
				index = -1;
			}
			protected override bool NextIndex(ref int index) {
				return ++index < Lines.Count;
			}
		}
		public class BackwardEnumerator : Enumerator {
			public BackwardEnumerator(PdfTextLines lines)
				: base(lines) {
			}
			protected override void ResetIndex(ref int index) {
				index = Lines.Count;
			}
			protected override bool NextIndex(ref int index) {
				return --index >= 0;
			}
		}
		#endregion
		string[] lines;
		bool directionRightToLeft;
		bool directionVertical;
		public int Count { get { return lines.Length; } }
		public string this[int index] { get { return lines[index]; } }
		public PdfTextLines(string[] lines, StringFormat format) {
			this.lines = lines;
			this.directionRightToLeft = (format.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0;
			this.directionVertical = (format.FormatFlags & StringFormatFlags.DirectionVertical) != 0;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			if(this.directionVertical) {
				if(this.directionRightToLeft)
					return new ForwardEnumerator(this);
				else
					return new BackwardEnumerator(this);
			} else
				return new ForwardEnumerator(this);
		}
	}
	#region TextLocation classes
	public abstract class TextLocation {
		public static TextLocation CreateInstance(StringFormat format) {
			StringAlignment lineAlignment = format.LineAlignment;
			StringAlignment alignment = format.Alignment;
			bool directionVertical = (format.FormatFlags & StringFormatFlags.DirectionVertical) != 0;
			switch(lineAlignment) {
				case StringAlignment.Near:
					return NearTextLocation.CreateInstance(alignment, directionVertical);
				case StringAlignment.Center:
					return CenterTextLocation.CreateInstance(alignment, directionVertical);
				case StringAlignment.Far:
					return FarTextLocation.CreateInstance(alignment, directionVertical);
				default:
					throw new ArgumentException();
			}
		}
		bool directionVertical;
		protected TextLocation(bool directionVertical) {
			this.directionVertical = directionVertical;
		}
		protected abstract float CalculateTextX_Horizontal(PdfGraphicsRectangle pdfRect, float textWidth);
		protected abstract float CalculateTextY_Horizontal(PdfGraphicsRectangle pdfRect, float textHeight);
		protected abstract float CalculateTextX_Vertical(PdfGraphicsRectangle pdfRect, float textWidth);
		protected abstract float CalculateTextY_Vertical(PdfGraphicsRectangle pdfRect, float textHeight);
		public float CalculateTextX(PdfGraphicsRectangle pdfRect, float textWidth) {
			return
				this.directionVertical ?
				CalculateTextX_Vertical(pdfRect, textWidth) :
				CalculateTextX_Horizontal(pdfRect, textWidth);
		}
		public float CalculateTextY(PdfGraphicsRectangle pdfRect, float textHeight) {
			return
				this.directionVertical ?
				CalculateTextY_Vertical(pdfRect, textHeight) :
				CalculateTextY_Horizontal(pdfRect, textHeight);
		}
	}
	public abstract class NearTextLocation : TextLocation {
		public static NearTextLocation CreateInstance(StringAlignment alignment, bool directionVertical) {
			switch(alignment) {
				case StringAlignment.Near:
					return new NearNearTextLocation(directionVertical);
				case StringAlignment.Center:
					return new NearCenterTextLocation(directionVertical);
				case StringAlignment.Far:
					return new NearFarTextLocation(directionVertical);
				default:
					throw new ArgumentException();
			}
		}
		protected NearTextLocation(bool directionVertical)
			: base(directionVertical) {
		}
		protected override float CalculateTextY_Horizontal(PdfGraphicsRectangle pdfRect, float textHeight) {
			return pdfRect.Top;
		}
	}
	public class NearNearTextLocation : NearTextLocation {
		public NearNearTextLocation(bool directionVertical)
			: base(directionVertical) {
		}
		protected override float CalculateTextX_Horizontal(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Left;
		}
		protected override float CalculateTextX_Vertical(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Left;
		}
		protected override float CalculateTextY_Vertical(PdfGraphicsRectangle pdfRect, float textHeight) {
			return pdfRect.Top + textHeight;
		}
	}
	public class NearCenterTextLocation : NearTextLocation {
		public NearCenterTextLocation(bool directionVertical)
			: base(directionVertical) {
		}
		protected override float CalculateTextX_Horizontal(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Left + (pdfRect.Width - textWidth) / 2;
		}
		protected override float CalculateTextX_Vertical(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Left + pdfRect.Width / 2;
		}
		protected override float CalculateTextY_Vertical(PdfGraphicsRectangle pdfRect, float textHeight) {
			return pdfRect.Top + textHeight / 2;
		}
	}
	public class NearFarTextLocation : NearTextLocation {
		public NearFarTextLocation(bool directionVertical)
			: base(directionVertical) {
		}
		protected override float CalculateTextX_Horizontal(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Right - textWidth;
		}
		protected override float CalculateTextX_Vertical(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Right;
		}
		protected override float CalculateTextY_Vertical(PdfGraphicsRectangle pdfRect, float textHeight) {
			return pdfRect.Top;
		}
	}
	public abstract class CenterTextLocation : TextLocation {
		public static CenterTextLocation CreateInstance(StringAlignment alignment, bool directionVertical) {
			switch(alignment) {
				case StringAlignment.Near:
					return new CenterNearTextLocation(directionVertical);
				case StringAlignment.Center:
					return new CenterCenterTextLocation(directionVertical);
				case StringAlignment.Far:
					return new CenterFarTextLocation(directionVertical);
				default:
					throw new ArgumentException();
			}
		}
		protected CenterTextLocation(bool directionVertical)
			: base(directionVertical) {
		}
		protected override float CalculateTextY_Horizontal(PdfGraphicsRectangle pdfRect, float textHeight) {
			return pdfRect.Bottom + (pdfRect.Height + textHeight) / 2;
		}
	}
	public class CenterNearTextLocation : CenterTextLocation {
		public CenterNearTextLocation(bool directionVertical)
			: base(directionVertical) {
		}
		protected override float CalculateTextX_Horizontal(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Left;
		}
		protected override float CalculateTextX_Vertical(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Left - textWidth / 2;
		}
		protected override float CalculateTextY_Vertical(PdfGraphicsRectangle pdfRect, float textHeight) {
			return pdfRect.Bottom + pdfRect.Height / 2 + textHeight;
		}
	}
	public class CenterCenterTextLocation : CenterTextLocation {
		public CenterCenterTextLocation(bool directionVertical)
			: base(directionVertical) {
		}
		protected override float CalculateTextX_Horizontal(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Left + (pdfRect.Width - textWidth) / 2;
		}
		protected override float CalculateTextX_Vertical(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Left + (pdfRect.Width - textWidth) / 2;
		}
		protected override float CalculateTextY_Vertical(PdfGraphicsRectangle pdfRect, float textHeight) {
			return pdfRect.Bottom + (pdfRect.Height + textHeight) / 2;
		}
	}
	public class CenterFarTextLocation : CenterTextLocation {
		public CenterFarTextLocation(bool directionVertical)
			: base(directionVertical) {
		}
		protected override float CalculateTextX_Horizontal(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Right - textWidth;
		}
		protected override float CalculateTextX_Vertical(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Right - textWidth / 2;
		}
		protected override float CalculateTextY_Vertical(PdfGraphicsRectangle pdfRect, float textHeight) {
			return pdfRect.Bottom + pdfRect.Height / 2;
		}
	}
	public abstract class FarTextLocation : TextLocation {
		public static FarTextLocation CreateInstance(StringAlignment alignment, bool directionVertical) {
			switch(alignment) {
				case StringAlignment.Near:
					return new FarNearTextLocation(directionVertical);
				case StringAlignment.Center:
					return new FarCenterTextLocation(directionVertical);
				case StringAlignment.Far:
					return new FarFarTextLocation(directionVertical);
				default:
					throw new ArgumentException();
			}
		}
		protected FarTextLocation(bool directionVertical)
			: base(directionVertical) {
		}
		protected override float CalculateTextY_Horizontal(PdfGraphicsRectangle pdfRect, float textHeight) {
			return pdfRect.Bottom + textHeight;
		}
	}
	public class FarNearTextLocation : FarTextLocation {
		public FarNearTextLocation(bool directionVertical)
			: base(directionVertical) {
		}
		protected override float CalculateTextX_Horizontal(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Left;
		}
		protected override float CalculateTextX_Vertical(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Left - textWidth;
		}
		protected override float CalculateTextY_Vertical(PdfGraphicsRectangle pdfRect, float textHeight) {
			return pdfRect.Bottom + textHeight;
		}
	}
	public class FarCenterTextLocation : FarTextLocation {
		public FarCenterTextLocation(bool directionVertical)
			: base(directionVertical) {
		}
		protected override float CalculateTextX_Horizontal(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Left + (pdfRect.Width - textWidth) / 2;
		}
		protected override float CalculateTextX_Vertical(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Left + pdfRect.Width / 2 - textWidth;
		}
		protected override float CalculateTextY_Vertical(PdfGraphicsRectangle pdfRect, float textHeight) {
			return pdfRect.Bottom + textHeight / 2;
		}
	}
	public class FarFarTextLocation : FarTextLocation {
		public FarFarTextLocation(bool directionVertical)
			: base(directionVertical) {
		}
		protected override float CalculateTextX_Horizontal(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Right - textWidth;
		}
		protected override float CalculateTextX_Vertical(PdfGraphicsRectangle pdfRect, float textWidth) {
			return pdfRect.Right - textWidth;
		}
		protected override float CalculateTextY_Vertical(PdfGraphicsRectangle pdfRect, float textHeight) {
			return pdfRect.Bottom;
		}
	}
	#endregion
	public class PdfGraphicsRectangle {
		RectangleF rect;
		public float Width { get { return rect.Width; } }
		public float Height { get { return rect.Height; } }
		public float Top { get { return rect.Bottom; } }
		public float Bottom { get { return rect.Top; } }
		public float Left { get { return rect.Left; } }
		public float Right { get { return rect.Right; } }
		public PdfGraphicsRectangle(RectangleF rect, SizeF pageSize) {
			this.rect = rect;
			this.rect.Y = pageSize.Height - rect.Bottom;
		}
	}
	#region FontLine classes
	public class FontLines {
		float thickness;
		ArrayList fontLineList = new ArrayList();
		public FontLines(Font font, float ascent, float descent) {
			this.thickness = font.Size / 14;
			if(font.Underline)
				this.fontLineList.Add(new Underline(ascent, descent));
			if(font.Strikeout)
				this.fontLineList.Add(new Strikeout(ascent, descent));
		}
		public void AddLines(float textX, float textY, float textWidth) {
			foreach(FontLine fontLine in this.fontLineList)
				fontLine.AddLine(textX, textY, textWidth);
		}
		public void DrawContents(PdfDrawContext context, Color color) {
			foreach(FontLine fontLine in this.fontLineList)
				fontLine.DrawContents(context, color, this.thickness);
		}
	}
	public abstract class FontLine {
		#region inner classes
		class Info {
			float x;
			float y;
			float length;
			public Info(float x, float y, float length) {
				this.x = x;
				this.y = y;
				this.length = length;
			}
			public void DrawContents(PdfDrawContext context) {
				context.MoveTo(this.x, this.y);
				context.LineTo(this.x + this.length, this.y);
			}
		}
		#endregion
		float yOffset;
		ArrayList infos = new ArrayList();
		protected FontLine(float ascent, float descent) {
			this.yOffset = CalculateYOffset(ascent, descent);
		}
		protected abstract float CalculateYOffset(float ascent, float descent);
		public void AddLine(float textX, float textY, float textWidth) {
			this.infos.Add(new Info(textX, textY + this.yOffset, textWidth));
		}
		public void DrawContents(PdfDrawContext context, Color color, float thickness) {
			context.SetRGBStrokeColor(color);
			context.SetLineWidth(thickness);
			foreach(Info info in this.infos)
				info.DrawContents(context);
			context.Stroke();
		}
	}
	public class Underline : FontLine {
		public Underline(float ascent, float descent)
			: base(ascent, descent) {
		}
		protected override float CalculateYOffset(float ascent, float descent) {
			return -descent * 0.5f;
		}
	}
	public class Strikeout : FontLine {
		public Strikeout(float ascent, float descent)
			: base(ascent, descent) {
		}
		protected override float CalculateYOffset(float ascent, float descent) {
			return ascent * 0.5f - descent;
		}
	}
	#endregion
	#region Transform classes
	public class TransformActivator {
		Matrix transform;
		GraphicsUnit pageUnit;
		SizeF pageSize;
		public Matrix Transform { get { return this.transform; } }
		public TransformActivator(Matrix transform, GraphicsUnit pageUnit, SizeF pageSize) {
			this.transform = transform;
			this.pageUnit = pageUnit;
			this.pageSize = pageSize;
		}
		float TransformValue(float value) {
			return GraphicsUnitConverter.Convert(value, GraphicsDpi.UnitToDpiI(this.pageUnit), GraphicsDpi.Point);
		}
		public void ApplyToDrawContext(PdfDrawContext context) {
			if(this.transform.IsIdentity)
				return;
			using(Matrix result = this.transform.Clone())
			using(Matrix toPdfTransform = new Matrix()) {
				float ratio = TransformValue(1f);
				toPdfTransform.Translate(0, this.pageSize.Height);
				toPdfTransform.Scale(ratio, -ratio);
				result.Multiply(toPdfTransform, MatrixOrder.Append);
				toPdfTransform.Invert();
				result.Multiply(toPdfTransform, MatrixOrder.Prepend);
				context.Concat(result);
			}
		}
	}
	#endregion
	static class PdfCoordinate {
		public static float TransformValue(GraphicsUnit pageUnit, float value) {
			return GraphicsUnitConverter.Convert(value, GraphicsDpi.UnitToDpiI(pageUnit), GraphicsDpi.Point);
		}
		public static SizeF TransformValue(GraphicsUnit pageUnit, SizeF value) {
			return GraphicsUnitConverter.Convert(value, GraphicsDpi.UnitToDpiI(pageUnit), GraphicsDpi.Point);
		}
		public static PointF TransformValue(GraphicsUnit pageUnit, PointF value) {
			return GraphicsUnitConverter.Convert(value, GraphicsDpi.UnitToDpiI(pageUnit), GraphicsDpi.Point);
		}
		public static float BackTransformValue(GraphicsUnit pageUnit, float value) {
			return GraphicsUnitConverter.Convert(value, GraphicsDpi.Point, GraphicsDpi.UnitToDpiI(pageUnit));
		}
		public static SizeF BackTransformValue(GraphicsUnit pageUnit, SizeF value) {
			return GraphicsUnitConverter.Convert(value, GraphicsDpi.Point, GraphicsDpi.UnitToDpiI(pageUnit));
		}
		public static PointF BackTransformValue(GraphicsUnit pageUnit, PointF value) {
			return GraphicsUnitConverter.Convert(value, GraphicsDpi.Point, GraphicsDpi.UnitToDpiI(pageUnit));
		}
		public static PointF CorrectPoint(GraphicsUnit pageUnit, PointF pt, SizeF pageSize) {
			pt = TransformValue(pageUnit, pt);
			return new PointF(pt.X, pageSize.Height - pt.Y);
		}
		public static RectangleF CorrectRectangle(GraphicsUnit pageUnit, RectangleF rect) {
			rect.Location = TransformValue(pageUnit, rect.Location);
			rect.Size = TransformValue(pageUnit, rect.Size);
			return rect;
		}
		public static RectangleF CorrectRectangle(GraphicsUnit pageUnit, RectangleF rect, SizeF pageSize) {
			RectangleF result = CorrectRectangle(pageUnit, rect);
			result.Y = pageSize.Height - result.Bottom;
			return result;
		}
	}
	static class LineCapHelper {
		public class LineCapDrawInfo {
			public bool fill;
			public PointF start;
			public GraphicsPath path;
			public float width;
			public LineCapDrawInfo(bool fill, PointF start, GraphicsPath path, float width) {
				this.fill = fill;
				this.start = start;
				this.path = path;
				this.width = width;
			}
		}
		public static LineCapDrawInfo MakeCapInfo(PointF start, PointF end, float width, LineCap cap, CustomLineCap customCap) {
			float delta = 0;
			GraphicsPath path = null;
			switch(cap) {
				case LineCap.Triangle:
					path = ToPolyPath(GetTriangle(width));
					break;
				case LineCap.SquareAnchor:
					path = ToPolyPath(GetSquareAnchor(width));
					break;
				case LineCap.RoundAnchor:
					path = GetRoundAnchor(width);
					break;
				case LineCap.DiamondAnchor:
					path = ToPolyPath(GetDiamondAnchor(width));
					break;
				case LineCap.ArrowAnchor:
					path = ToPolyPath(GetArrowAnchor(width, ref delta));
					break;
				case LineCap.Custom:
					return DrawCustomCap(start, end, width, customCap);
				default:
					break;
			}
			if(path != null)
				return FillCapPath(start, end, delta, path);
			return null;
		}
		static PointF[] GetTriangle(float width) {
			float w = width / 2;
			float s = 0.1f * width;
			return new PointF[] { new PointF(s, w), new PointF(s, -w), new PointF(0, -w), new PointF(-w, 0), new PointF(0, w) };
		}
		static PointF[] GetSquareAnchor(float width) {
			float c = -width * 0.7f;
			float s = -c;
			return new PointF[] { new PointF(c, c), new PointF(c, s), new PointF(s, s), new PointF(s, c) };
		}
		static GraphicsPath GetRoundAnchor(float width) {
			GraphicsPath path = new GraphicsPath();
			path.AddEllipse(-width, -width, 2f * width, 2f * width);
			return path;
		}
		static PointF[] GetDiamondAnchor(float width) {
			return new PointF[] { new PointF(-width, 0), new PointF(0, width), new PointF(width, 0), new PointF(0, -width) };
		}
		static PointF[] GetArrowAnchor(float width, ref float delta) {
			float c = width * 1.7f;
			delta = 1.6f * width;
			return new PointF[] { new PointF(0, 0), new PointF(c, width), new PointF(c, -width) };
		}
		static LineCapDrawInfo DrawCustomCap(PointF start, PointF end, float width, CustomLineCap customCap) {
			if(!(customCap is AdjustableArrowCap))
				return null;
			AdjustableArrowCap arrowCap = (AdjustableArrowCap)customCap;
			float scaledWidth = width * arrowCap.WidthScale;
			float hSize = scaledWidth * arrowCap.Height;
			float wSize = scaledWidth * arrowCap.Width / 2;
			float delta = arrowCap.Height / arrowCap.Width * scaledWidth;
			PointF[] points = new PointF[] { new PointF(hSize, wSize), new PointF(0, 0), new PointF(hSize, -wSize) };
			GraphicsPath path = new GraphicsPath();
			if(arrowCap.Filled) {
				path.AddPolygon(points);
				return FillCapPath(start, end, delta, path);
			} else {
				path.AddLines(points);
				return DrawCapPath(start, end, delta, path, scaledWidth);
			}
		}
		static void TransformLineCap(ref PointF start, PointF end, ref GraphicsPath path, float delta) {
			bool hasPoints = path != null;
			bool hasDelta = Math.Abs(delta) > 0.001;
			if(!hasPoints && !hasDelta)
				return;
			float dx = end.X - start.X;
			float dy = end.Y - start.Y;
			float d = (float)Math.Sqrt(dx * dx + dy * dy);
			using(Matrix rotate = new Matrix(dx / d, dy / d, -dy / d, dx / d, 0, 0)) {
				if(hasPoints) {
					using(Matrix m = new Matrix()) {
						m.Translate(start.X, start.Y);
						m.Multiply(rotate);
						PointF[] points = path.PathPoints;
						m.TransformPoints(points);
						path = new GraphicsPath(points, path.PathTypes, path.FillMode);
					}
				}
				if(hasDelta) {
					using(Matrix m = new Matrix()) {
						m.Multiply(rotate);
						m.Translate(delta, 0);
						rotate.Invert();
						m.Multiply(rotate);
						PointF[] pStart = new PointF[] { start };
						m.TransformPoints(pStart);
						start = pStart[0];
					}
				}
			}
		}
		static GraphicsPath ToPolyPath(PointF[] points) {
			GraphicsPath path = new GraphicsPath();
			path.AddPolygon(points);
			return path;
		}
		static LineCapDrawInfo FillCapPath(PointF start, PointF end, float delta, GraphicsPath path) {
			TransformLineCap(ref start, end, ref path, delta);
			return new LineCapDrawInfo(true, start, path, 0);
		}
		static LineCapDrawInfo DrawCapPath(PointF start, PointF end, float delta, GraphicsPath path, float scaledWidth) {
			TransformLineCap(ref start, end, ref path, delta);
			return new LineCapDrawInfo(false, start, path, scaledWidth);
		}
	}
}
