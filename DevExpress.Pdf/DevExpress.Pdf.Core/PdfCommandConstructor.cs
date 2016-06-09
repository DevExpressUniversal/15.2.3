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
	class PdfCommandConstructor {
		static readonly double ellipticFactor = 0.5 - (1 / Math.Sqrt(2) - 0.5) / 0.75;
		public PdfTransformationMatrix CurrentTransform { get { return currentTransform; } }
		readonly IList<PdfCommand> commands;
		readonly PdfResources resources;
		readonly Stack<PdfTransformationMatrix> matrixStack = new Stack<PdfTransformationMatrix>();
		PdfTransformationMatrix currentTransform = new PdfTransformationMatrix();
		public PdfCommandConstructor(IList<PdfCommand> commands, PdfResources resources) {
			this.commands = commands;
			this.resources = resources;
		}
		public void SaveGraphicsState() {
			matrixStack.Push(currentTransform);
			commands.Add(new PdfSaveGraphicsStateCommand());
		}
		public void RestoreGraphicsState() {
			if (matrixStack.Count != 0)
				currentTransform = matrixStack.Pop();
			commands.Add(new PdfRestoreGraphicsStateCommand());
		}
		public void SetGrayColorForNonStrokingOperations(double gray) {
			commands.Add(new PdfSetGrayColorSpaceForNonStrokingOperationsCommand(gray));
		}
		public void SetColorForNonStrokingOperations(PdfColor color) {
			if (color == null)
				return;
			if (color.Pattern != null) {
				commands.Add(new PdfSetColorSpaceForNonStrokingOperationsCommand(new PdfPatternColorSpace()));
				commands.Add(new PdfSetColorAdvancedForNonStrokingOperationsCommand(resources, GetPatternName(color.Pattern)));
			}
			else {
				double[] components = color.Components;
				switch (components.Length) {
					case 1:
						commands.Add(new PdfSetGrayColorSpaceForNonStrokingOperationsCommand(components[0]));
						break;
					case 4:
						commands.Add(new PdfSetCMYKColorSpaceForNonStrokingOperationsCommand(components[0], components[1], components[2], components[3]));
						break;
					default:
						commands.Add(new PdfSetRGBColorSpaceForNonStrokingOperationsCommand(components[0], components[1], components[2]));
						break;
				}
			}
		}
		public void SetColorForStrokingOperations(PdfColor color) {
			if (color == null)
				return;
			if (color.Pattern != null) {
				commands.Add(new PdfSetColorSpaceForStrokingOperationsCommand(new PdfPatternColorSpace()));
				commands.Add(new PdfSetColorAdvancedForStrokingOperationsCommand(resources, GetPatternName(color.Pattern)));
			}
			else {
				double[] components = color.Components;
				switch (components.Length) {
					case 1:
						commands.Add(new PdfSetGrayColorSpaceForStrokingOperationsCommand(components[0]));
						break;
					case 4:
						commands.Add(new PdfSetCMYKColorSpaceForStrokingOperationsCommand(components[0], components[1], components[2], components[3]));
						break;
					default:
						commands.Add(new PdfSetRGBColorSpaceForStrokingOperationsCommand(components[0], components[1], components[2]));
						break;
				}
			}
		}
		public void SetColorForStrokingOperations(PdfRGBColor color) {
			commands.Add(new PdfSetRGBColorSpaceForStrokingOperationsCommand(color.R, color.G, color.B));
		}
		public void SetLineStyle(PdfLineStyle lineStyle) {
			commands.Add(new PdfSetLineStyleCommand(lineStyle));
		}
		public void SetLineJoinStyle(PdfLineJoinStyle lineJoin) {
			commands.Add(new PdfSetLineJoinStyleCommand(lineJoin));
		}
		public void SetLineWidth(double lineWidth) {
			commands.Add(new PdfSetLineWidthCommand(lineWidth));
		}
		public void SetLineCapStyle(PdfLineCapStyle capStyle) {
			commands.Add(new PdfSetLineCapStyleCommand(capStyle));
		}
		public void SetMiterLimit(double miterLimit) {
			commands.Add(new PdfSetMiterLimitCommand(miterLimit));
		}
		public void SetGraphicsStateParameters(PdfGraphicsStateParameters parameters) {
			resources.AddGraphicsStateParameters(parameters);
			commands.Add(new PdfSetGraphicsStateParametersCommand(parameters));
		}
		public void DrawBezier(PdfPoint pt1, PdfPoint pt2, PdfPoint pt3, PdfPoint pt4) {
			commands.Add(new PdfBeginPathCommand(pt1));
			commands.Add(new PdfAppendBezierCurveCommand(pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y));
			StrokePath();
		}
		public void DrawBeziers(PdfPoint[] points) {
			int pointCount = points.Length;
			if (pointCount < 4)
				return;
			commands.Add(new PdfBeginPathCommand(points[0]));
			for (int i = 1; i < pointCount; i += 3) {
				if (pointCount - i < 3)
					return;
				PdfPoint pt1 = points[i];
				PdfPoint pt2 = points[i + 1];
				PdfPoint pt3 = points[i + 2];
				commands.Add(new PdfAppendBezierCurveCommand(pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y));
			}
			StrokePath();
		}
		public void DrawEllipse(PdfRectangle rect) {
			AppendEllipse(rect);
			CloseAndStrokePath();
		}
		public void FillEllipse(PdfRectangle rect) {
			AppendEllipse(rect);
			ClosePath();
			FillPath(true);
		}
		public void DrawXObject(string resourceName, PdfRectangle bounds) {
			DrawXObject(resourceName, new PdfTransformationMatrix(bounds.Width, 0, 0, bounds.Height, bounds.Left, bounds.Bottom));
		}
		public void DrawXObject(string resourceName, PdfRectangle bounds, PdfTransformationMatrix transform) {
			PdfTransformationMatrix imageTransform = new PdfTransformationMatrix(bounds.Width, 0, 0, bounds.Height, bounds.Left, bounds.Bottom);
			DrawXObject(resourceName, PdfTransformationMatrix.Multiply(imageTransform, transform));
		}
		public void DrawXObject(string resourceName, PdfPoint location) {
			DrawXObject(resourceName, new PdfTransformationMatrix(1, 0, 0, 1, location.X, location.Y));
		}
		public void DrawLine(PdfPoint pt1, PdfPoint pt2) {
			commands.Add(new PdfBeginPathCommand(pt1));
			commands.Add(new PdfAppendLineSegmentCommand(pt2));
			StrokePath();
		}
		public void DrawLines(PdfPoint[] points) {
			AppendPolygon(points);
			StrokePath();
		}
		public void DrawPolygon(PdfPoint[] points) {
			AppendPolygon(points);
			CloseAndStrokePath();
		}
		public void DrawPath(PdfGraphicsPath path) {
			path.GeneratePathCommands(commands);
			StrokePath();
		}
		public void FillPath(PdfGraphicsPath path, bool nonZero) {
			path.GeneratePathCommands(commands);
			FillPath(nonZero);
		}
		public void FillPolygon(PdfPoint[] points, bool nonZero) {
			AppendPolygon(points);
			FillPath(nonZero);
		}
		public void DrawRectangle(PdfRectangle rect) {
			AppendRectangle(rect);
			StrokePath();
		}
		public void FillRectangle(PdfRectangle rect) {
			AppendRectangle(rect);
			FillPath(true);
		}
		public void DrawString(byte[] text, PdfPoint point, PdfFont font, double fontSize, double[] glyphOffsets) {
			DrawString(text, () => commands.Add(new PdfStartTextLineWithOffsetsCommand(point.X, point.Y)), font, fontSize, glyphOffsets);
		}
		public void DrawObliqueString(byte[] text, PdfPoint point, PdfFont font, double fontSize, double[] glyphOffsets) {
			DrawString(text, () => commands.Add(new PdfSetTextMatrixCommand(new PdfTransformationMatrix(1, 0, 0.333, 1, point.X, point.Y))), font, fontSize, glyphOffsets);
		}
		public void IntersectClip(PdfRectangle rect) {
			AppendRectangle(rect);
			commands.Add(new PdfModifyClippingPathUsingNonzeroWindingNumberRuleCommand());
			commands.Add(new PdfEndPathWithoutFillingAndStrokingCommand());
		}
		public void IntersectClip(PdfGraphicsPath path) {
			path.GeneratePathCommands(commands);
			commands.Add(new PdfModifyClippingPathUsingNonzeroWindingNumberRuleCommand());
			commands.Add(new PdfEndPathWithoutFillingAndStrokingCommand());
		}
		public void ModifyTransformationMatrix(PdfTransformationMatrix matrix) {
			currentTransform = PdfTransformationMatrix.Multiply(matrix, currentTransform);
			commands.Add(new PdfModifyTransformationMatrixCommand(matrix));
		}
		public void TranslateTransform(double x, double y) {
			ModifyTransformationMatrix(PdfTransformationMatrix.Translate(new PdfTransformationMatrix(), x, y));
		}
		public void RotateTransform(float degree) {
			double rad = Math.PI * degree / 180;
			double sin = Math.Sin(rad);
			double cos = Math.Cos(rad);
			ModifyTransformationMatrix(new PdfTransformationMatrix(cos, -sin, sin, cos, 0, 0));
		}
		void AppendPolygon(PdfPoint[] points) {
			int pointCount = points != null ? points.Length : 0;
			if (pointCount >= 2) {
				commands.Add(new PdfBeginPathCommand(points[0]));
				for (int i = 1; i < pointCount; i++)
					commands.Add(new PdfAppendLineSegmentCommand(points[i]));
			}
		}
		void AppendRectangle(PdfRectangle rect) {
			commands.Add(new PdfAppendRectangleCommand(rect.Left, rect.Bottom, rect.Width, rect.Height));
		}
		PdfOperands GetPatternName(PdfPattern pattern) {
			PdfOperands operands = new PdfOperands();
			resources.AddPattern(pattern);
			operands.Add(resources.FindPatternName(pattern));
			return operands;
		}
		void AppendEllipse(PdfRectangle rect) {
			double left = rect.Left;
			double right = rect.Right;
			double centerX = (left + right) / 2;
			double bottom = rect.Bottom;
			double top = rect.Top;
			double centerY = (bottom + top) / 2;
			double horizontalOffset = (right - left) * ellipticFactor;
			double leftControlPoint = left + horizontalOffset;
			double rightControlPoint = right - horizontalOffset;
			double verticalOffset = (top - bottom) * ellipticFactor;
			double bottomControlPoint = bottom + verticalOffset;
			double topControlPoint = top - verticalOffset;
			commands.Add(new PdfBeginPathCommand(new PdfPoint(right, centerY)));
			commands.Add(new PdfAppendBezierCurveCommand(right, topControlPoint, rightControlPoint, top, centerX, top));
			commands.Add(new PdfAppendBezierCurveCommand(leftControlPoint, top, left, topControlPoint, left, centerY));
			commands.Add(new PdfAppendBezierCurveCommand(left, bottomControlPoint, leftControlPoint, bottom, centerX, bottom));
			commands.Add(new PdfAppendBezierCurveCommand(rightControlPoint, bottom, right, bottomControlPoint, right, centerY));
		}
		void ClosePath() {
			commands.Add(new PdfClosePathCommand());
		}
		void StrokePath() {
			commands.Add(new PdfStrokePathCommand());
		}
		void CloseAndStrokePath() {
			commands.Add(new PdfCloseAndStrokePathCommand());
		}
		void FillPath(bool nonZero) {
			commands.Add(nonZero ? (PdfCommand)new PdfFillPathUsingNonzeroWindingNumberRuleCommand() : new PdfFillPathUsingEvenOddRuleCommand());
		}
		void DrawString(byte[] text, Action setLocation, PdfFont font, double fontSize, double[] glyphOffsets) {
			if (text != null && text.Length != 0) {
				resources.AddFont(font);
				commands.Add(new PdfBeginTextCommand());
				commands.Add(new PdfSetTextFontCommand(font, fontSize));
				setLocation();
				commands.Add(glyphOffsets == null ? new PdfShowTextCommand(text) : new PdfShowTextWithGlyphPositioningCommand(text, glyphOffsets));
				commands.Add(new PdfEndTextCommand());
			}
		}
		void DrawXObject(string resourceName, PdfTransformationMatrix matrix) {
			SaveGraphicsState();
			ModifyTransformationMatrix(matrix);
			commands.Add(new PdfPaintXObjectCommand(resourceName, resources));
			RestoreGraphicsState();
		}
	}
}
