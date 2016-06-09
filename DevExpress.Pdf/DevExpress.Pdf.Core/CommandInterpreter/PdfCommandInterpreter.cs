#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
namespace DevExpress.Pdf.Native {
	public abstract class PdfCommandInterpreter : PdfDisposableObject {
		readonly Stack<PdfGraphicsState> graphicsStateStack = new Stack<PdfGraphicsState>();
		readonly Stack<PdfTransformationMatrix> tilingPatternTransformationMatrixStack = new Stack<PdfTransformationMatrix>();
		readonly PdfPage page;
		readonly PdfRectangle boundingBox;
		readonly PdfPolygonClipper boundingBoxClipper;
		PdfGraphicsState graphicsState = new PdfGraphicsState();
		PdfTransformationMatrix tilingPatternTransformationMatrix;
		List<PdfGraphicsPath> paths = new List<PdfGraphicsPath>();
		PdfTransformationMatrix TextTransformationMatrix {
			get {
				PdfTransformationMatrix matrix = PdfTransformationMatrix.Multiply(graphicsState.TextState.TextMatrix, graphicsState.TransformationMatrix);
				return new PdfTransformationMatrix(matrix.A, matrix.B, matrix.C, matrix.D, 0, 0);
			}
		}
		PdfGraphicsPath CurrentPath {
			get {
				int count = paths.Count;
				return count == 0 ? null : paths[count - 1];
			}
		}
		protected PdfRectangle BoundingBox { get { return boundingBox; } }
		protected IList<PdfGraphicsPath> Paths { get { return paths; } }
		protected double FontSizeFactor { get { return graphicsState.TextState.FontSize / 1000; } }
		protected double TextWidthFactor { get { return CalcTextScalingFactor(new PdfPoint(1, 0)); } }
		protected double TextHeightFactor { get { return CalcTextScalingFactor(new PdfPoint(0, 1)); } }
		protected double TextAngle { get { return CalcAngle(new PdfPoint(1, 0)); } }
		public PdfGraphicsState GraphicsState { get { return graphicsState; } }
		public PdfTransformationMatrix TilingPatternTransformationMatrix { get { return tilingPatternTransformationMatrix; } }
		protected PdfCommandInterpreter() {
		}
		protected PdfCommandInterpreter(PdfPage page, int rotateAngle, PdfRectangle boundingBox) {
			this.page = page;
			double userUnit = page.UserUnit;
			this.boundingBox = boundingBox;
			PdfTransformationMatrix matrix;
			double xOffset;
			double yOffset;
			switch (rotateAngle) {
				case 90:
					matrix = new PdfTransformationMatrix(0, -1, 1, 0, 0, 0);
					xOffset = -boundingBox.Bottom;
					yOffset = boundingBox.Left;
					break;
				case 180:
					matrix = new PdfTransformationMatrix(-1, 0, 0, -1, 0, 0);
					xOffset = boundingBox.Right;
					yOffset = boundingBox.Top;
					break;
				case 270:
					matrix = new PdfTransformationMatrix(0, 1, -1, 0, 0, 0);
					xOffset = boundingBox.Top;
					yOffset = -boundingBox.Right;
					break;
				default:
					matrix = new PdfTransformationMatrix();
					xOffset = -boundingBox.Left;
					yOffset = -boundingBox.Bottom;
					break;
			}
			boundingBoxClipper = new PdfPolygonClipper(new PdfRectangle(matrix.Transform(boundingBox.BottomLeft), matrix.Transform(boundingBox.TopRight)));
			graphicsState.TransformationMatrix = PdfTransformationMatrix.Scale(PdfTransformationMatrix.Translate(matrix, xOffset, yOffset), userUnit, userUnit);
			UpdateTilingPatternTransformationMatrix();
		}
		protected PdfCommandInterpreter(PdfPage page, int rotateAngle) : this(page, PdfPageTreeNode.NormalizeRotate(page.Rotate + rotateAngle), page.CropBox) {
		}
		protected void Execute(IEnumerable<PdfCommand> commands) {
			foreach (PdfCommand command in commands)
				try {
					command.Execute(this);
				}
				catch {
				}
		}
		protected double CalcAngle(PdfPoint point) {
			PdfPoint p = TextTransformationMatrix.Transform(point);
			return Math.Atan2(p.Y, p.X);
		}
		double CalcTextScalingFactor(PdfPoint p) {
			p = TextTransformationMatrix.Transform(p);
			double x = p.X;
			double y = p.Y;
			return Math.Sqrt(x * x + y * y);
		}
		void UpdateTilingPatternTransformationMatrix() {
			tilingPatternTransformationMatrix = graphicsState.TransformationMatrix;
		}
		public void Execute() {
			Execute(page.Commands);
		}
		public void UpdateTransformationMatrix(PdfTransformationMatrix matrix) {
			graphicsState.TransformationMatrix = PdfTransformationMatrix.Multiply(matrix, graphicsState.TransformationMatrix);
		}
		public void SetLineWidth(double lineWidth) {
			graphicsState.LineWidth = lineWidth;
		}
		public void SetLineStyle(PdfLineStyle lineStyle) {
			graphicsState.LineStyle = lineStyle;
		}
		public void SetLineCapStyle(PdfLineCapStyle lineCapStyle) {
			graphicsState.LineCap = lineCapStyle;
		}
		public void SetLineJoinStyle(PdfLineJoinStyle lineJoinStyle) {
			graphicsState.LineJoin = lineJoinStyle;
		}
		public void SetMiterLimit(double miterLimit) {
			graphicsState.MiterLimit = miterLimit;
		}
		public void SetFlatnessTolerance(double flatnessTolerance) {
			graphicsState.FlatnessTolerance = flatnessTolerance;
		}
		public void SetColorSpaceForStrokingOperations(PdfColorSpace colorSpace) {
			graphicsState.StrokingColorSpace = colorSpace;
		}
		public void SetColorSpaceForNonStrokingOperations(PdfColorSpace colorSpace) {
			graphicsState.NonStrokingColorSpace = colorSpace;
		}
		public void SetRenderingIntent(PdfRenderingIntent renderingIntent) {
			graphicsState.RenderingIntent = renderingIntent;
		}
		public void SetTextRenderingMode(PdfTextRenderingMode renderingMode) {
			graphicsState.TextState.RenderingMode = renderingMode;
		}
		public void ApplyGraphicsStateParameters(PdfGraphicsStateParameters parameters) {
			UpdateGraphicsState(graphicsState.ApplyParameters(parameters));
		}
		public virtual void BeginPath(PdfPoint startPoint) {
			paths.Add(new PdfGraphicsPath(startPoint));
		}
		public virtual void AppendPathLineSegment(PdfPoint endPoint) {
			PdfGraphicsPath currentPath = CurrentPath;
			if (currentPath != null)
				currentPath.AppendLineSegment(endPoint);
		}
		public virtual void AppendPathBezierSegment(PdfPoint controlPoint1, PdfPoint controlPoint2, PdfPoint endPoint) {
			PdfGraphicsPath currentPath = CurrentPath;
			if (currentPath != null)
				currentPath.AppendBezierSegment(controlPoint1, controlPoint2, endPoint);
		}
		public virtual void AppendPathBezierSegment(PdfPoint controlPoint2, PdfPoint endPoint) {
			PdfGraphicsPath currentPath = CurrentPath;
			if (currentPath != null)
				currentPath.AppendBezierSegment(currentPath.EndPoint, controlPoint2, endPoint);
		}
		public virtual void ClosePath() {
			PdfGraphicsPath currentPath = CurrentPath;
			if (currentPath != null) {
				PdfPoint startPoint = currentPath.StartPoint;
				PdfPoint endPoint = currentPath.EndPoint;
				if (startPoint.X != endPoint.X || startPoint.Y != endPoint.Y)
					currentPath.AppendLineSegment(currentPath.StartPoint);
				currentPath.Closed = true;
			}
		}
		public virtual void AppendRectangle(double x, double y, double width, double height) {
			paths.Add(new PdfRectangularGraphicsPath(x, y, width, height));
		}
		public virtual void TransformPaths() {
			PdfTransformationMatrix matrix = graphicsState.TransformationMatrix;
			foreach (PdfGraphicsPath path in paths)
				path.Transform(matrix);
		}
		public virtual void CreateNewPaths() {
			paths = new List<PdfGraphicsPath>();
		}
		public virtual void TransformAndStrokePaths() {
			TransformPaths();
			StrokePaths();
			CreateNewPaths();
		}
		public void Clip(bool useNonzeroWindingRule) {
			TransformPaths();
			foreach (PdfGraphicsPath path in paths)
				if (!(path is PdfRectangularGraphicsPath))
					boundingBoxClipper.Clip(path);
			ClipPaths(useNonzeroWindingRule);
			CreateNewPaths();
		}
		public void SetCharacterSpacing(double characterSpacing) {
			graphicsState.TextState.CharacterSpacing = characterSpacing;
		}
		public void SetWordSpacing(double wordSpacing) {
			graphicsState.TextState.WordSpacing = wordSpacing;
		}
		public void SetTextLeading(double leading) {
			graphicsState.TextState.Leading = leading;
		}
		public void SetTextHorizontalScaling(double scaling) {
			graphicsState.TextState.HorizontalScaling = Math.Abs(scaling);
		}
		public void SetTextRise(double rise) {
			graphicsState.TextState.Rise = rise;
		}
		public void SetTextMatrix(double offsetTextByX, double offsetTextByY) {
			SetTextMatrix(PdfTransformationMatrix.Multiply(new PdfTransformationMatrix(1, 0, 0, 1, offsetTextByX, offsetTextByY), graphicsState.TextState.TextLineMatrix));
		}
		public void MoveToNextLine() {
			SetTextMatrix(0, -graphicsState.TextState.Leading);
		}
		public void DrawString(byte[] data, double[] offsets) {
			PdfTextState textState = graphicsState.TextState;
			PdfFont font = textState.Font;
			PdfStringData stringData = font.ActualEncoding.GetStringData(data, offsets);
			double fontSizeFactor = FontSizeFactor;
			double firstGyphOffset = stringData.Offsets[0] * fontSizeFactor;
			double textWidthFactor = TextWidthFactor;
			double[] glyphOffsets = font.GetGlyphPositions(stringData, fontSizeFactor, textState.CharacterSpacing, textState.WordSpacing, textWidthFactor, textState.HorizontalScaling / 100);
			PdfTransformationMatrix textRenderingMatrix = PdfTransformationMatrix.Multiply(textState.TextRenderingMatrix, graphicsState.TransformationMatrix);
			double textAngle = TextAngle;
			double offset = firstGyphOffset * textWidthFactor;
			DrawString(stringData, new PdfPoint(textRenderingMatrix.E - Math.Cos(textAngle) * offset, textRenderingMatrix.F - Math.Sin(textAngle) * offset), glyphOffsets);
			int positionsLength = glyphOffsets.Length;
			if (positionsLength > 0) {
				double moving = glyphOffsets[positionsLength - 1] / textWidthFactor - firstGyphOffset;
				PdfTransformationMatrix textMatrix = textState.TextMatrix;
				PdfPoint p = new PdfTransformationMatrix(textMatrix.A, textMatrix.B, textMatrix.C, textMatrix.D, 0, 0).Transform(new PdfPoint(moving, 0));
				textState.TextMatrix = PdfTransformationMatrix.Translate(textMatrix, p.X, p.Y);
			}
		}
		public void DrawForm(PdfForm form) {
			SaveGraphicsState();
			UpdateTransformationMatrix(form.Matrix);
			tilingPatternTransformationMatrixStack.Push(tilingPatternTransformationMatrix);
			try {
				UpdateTilingPatternTransformationMatrix();
				PdfRectangle bBox = form.BBox;
				AppendRectangle(bBox.Left, bBox.Bottom, bBox.Width, bBox.Height);
				Clip(true);
				Execute(form.Commands);
			}
			finally {
				if (tilingPatternTransformationMatrixStack.Count > 0)
					tilingPatternTransformationMatrix = tilingPatternTransformationMatrixStack.Pop(); 
				RestoreGraphicsState();
			}
		}
		public void DrawAnnotation(PdfAnnotation annotation, PdfAnnotationAppearanceState state, PdfRGBColor? highlightColor) {
			if (ShouldDrawAnnotation(annotation)) {
				PdfForm annotationForm = highlightColor.HasValue ? annotation.GetHighlightedAppearanceForm(state, highlightColor.Value) : annotation.GetAppearanceForm(state);
				if (annotationForm != null) {
					SaveGraphicsState();
					try {
						PdfTransformationMatrix matrix = annotationForm.Matrix;
						PdfRectangle boundingBox = annotationForm.BBox;
						double left = boundingBox.Left;
						double right = boundingBox.Right;
						double top = boundingBox.Top;
						double bottom = boundingBox.Bottom;
						PdfPoint bottomLeft = matrix.Transform(new PdfPoint(left, bottom));
						double xMin = bottomLeft.X;
						double xMax = bottomLeft.X;
						double yMin = bottomLeft.Y;
						double yMax = bottomLeft.Y;
						PdfPoint[] points = new PdfPoint[] { matrix.Transform(new PdfPoint(left, top)), matrix.Transform(new PdfPoint(right, top)), matrix.Transform(new PdfPoint(right, bottom)) };
						foreach (PdfPoint point in points) {
							double x = point.X;
							xMin = PdfMathUtils.Min(xMin, x);
							xMax = PdfMathUtils.Max(xMax, x);
							double y = point.Y;
							yMin = PdfMathUtils.Min(yMin, y);
							yMax = PdfMathUtils.Max(yMax, y);
						}
						PdfRectangle annotationRect = annotation.Rect;
						double scaleX = annotationRect.Width / (xMax - xMin);
						double scaleY = annotationRect.Height / (yMax - yMin);
						UpdateTransformationMatrix(new PdfTransformationMatrix(scaleX, 0, 0, scaleY, annotationRect.Left - xMin * scaleX, annotationRect.Bottom - yMin * scaleY));
						DrawForm(annotationForm);
					}
					finally {
						RestoreGraphicsState();
					}
				}
			}
		}
		public virtual void SaveGraphicsState() {
			graphicsStateStack.Push(graphicsState.Clone());
		}
		public virtual void RestoreGraphicsState() {
			if (graphicsStateStack.Count > 0)
				graphicsState = graphicsStateStack.Pop();
		}
		public virtual void SetColorForStrokingOperations(PdfColor color) {
			PdfColorSpace colorSpace = graphicsState.StrokingColorSpace;
			graphicsState.StrokingColor = colorSpace == null ? color : colorSpace.Transform(color);
		}
		public virtual void SetColorForNonStrokingOperations(PdfColor color) {
			PdfColorSpace colorSpace = graphicsState.NonStrokingColorSpace;
			graphicsState.NonStrokingColor = colorSpace == null ? color : colorSpace.Transform(color);
		}
		public virtual void BeginText() {
			SetTextMatrix(new PdfTransformationMatrix());
		}
		public virtual void EndText() {
		}
		public virtual void SetFont(PdfFont font, double fontSize) {
			PdfTextState pdfTextState = graphicsState.TextState;
			pdfTextState.Font = font;
			pdfTextState.FontSize = fontSize;
		}
		public virtual void SetTextMatrix(PdfTransformationMatrix matrix) {
			PdfTextState textState = graphicsState.TextState;
			textState.TextLineMatrix = matrix;
			textState.TextMatrix = matrix.Clone();
		}
		protected override void Dispose(bool disposing) {
		}
		protected virtual bool ShouldDrawAnnotation(PdfAnnotation annotation) {
			if (annotation.Flags.HasFlag(PdfAnnotationFlags.Hidden))
				return false;
			PdfTextMarkupAnnotation textMarkupAnnotation = annotation as PdfTextMarkupAnnotation;
			return textMarkupAnnotation == null || textMarkupAnnotation.Type != PdfTextMarkupAnnotationType.Highlight;
		}
		protected abstract void UpdateGraphicsState(PdfGraphicsStateChange change);
		protected abstract void DrawString(PdfStringData stringData, PdfPoint location, double[] glyphOffsets);
		public abstract void DrawImage(PdfImage image);
		public abstract void DrawShading(PdfShading shading);
		public abstract void StrokePaths();
		public abstract void FillPaths(bool useNonzeroWindingRule);
		public abstract void ClipPaths(bool useNonzeroWindingRule);
	}
}
