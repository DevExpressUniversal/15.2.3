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
using System.Collections.Generic;
namespace DevExpress.Pdf.Drawing {
	public class PdfGraphicsPathBuilder {
		readonly PdfViewerCommandInterpreter interpreter;
		readonly bool shouldUseRectangularGraphicsPath;
		public PdfGraphicsPathBuilder(PdfViewerCommandInterpreter interpreter) {
			this.interpreter = interpreter;
			shouldUseRectangularGraphicsPath = interpreter.ShouldUseRectangularGraphicsPath;
		}
		public GraphicsPath CreatePath(IEnumerable<PdfGraphicsPath> paths) {
			GraphicsPath graphicsPath = new GraphicsPath();
			foreach (PdfGraphicsPath path in paths)
				AppendPath(graphicsPath, path);
			return graphicsPath;
		}
		public GraphicsPath CreatePath(PdfGraphicsPath path, double extendSize) {
			GraphicsPath graphicsPath = new GraphicsPath();
			IList<PdfGraphicsPathSegment> segments = path.Segments;
			int lastSegmentIndex = segments.Count - 1;
			if (lastSegmentIndex >= 0) {
				bool shouldExtend = extendSize > 0;
				if (shouldExtend) {
					PdfLineGraphicsPathSegment segment = segments[0] as PdfLineGraphicsPathSegment;
					if (segment != null)
						AppendExtendLine(graphicsPath, path.StartPoint, segment.EndPoint, extendSize, true);
				}
				AppendPath(graphicsPath, path);
				if (shouldExtend) {
					PdfLineGraphicsPathSegment segment = segments[lastSegmentIndex] as PdfLineGraphicsPathSegment;
					if (segment != null)
						AppendExtendLine(graphicsPath, lastSegmentIndex == 0 ? path.StartPoint : segments[lastSegmentIndex - 1].EndPoint, segment.EndPoint, extendSize, false);
				}
			}
			return graphicsPath;
		}
		public RectangleF CreateRectangle(PdfGraphicsPath path) {
			if (!shouldUseRectangularGraphicsPath)
				return RectangleF.Empty;
			PdfRectangularGraphicsPath rectangularGraphicsPath = path as PdfRectangularGraphicsPath;
			if (rectangularGraphicsPath == null)
				return RectangleF.Empty;
			PdfRectangle rectangle = rectangularGraphicsPath.Rectangle;
			PointF point1 = interpreter.TransformPoint(new PdfPoint(rectangle.Left, rectangle.Bottom));
			PointF point2 = interpreter.TransformPoint(new PdfPoint(rectangle.Right, rectangle.Top));
			float x1 = point1.X;
			float x2 = point2.X;
			if (x1 > x2) {
				float temp = x1;
				x1 = x2;
				x2 = temp;
			}
			x2 = Math.Max(x2, x1 + 1);
			float y1 = point1.Y;
			float y2 = point2.Y;
			if (y1 > y2) {
				float temp = y1;
				y1 = y2;
				y2 = temp;
			}
			return new RectangleF(x1, y1, Math.Max(x2 - x1, 1), Math.Max(y2 - y1, 1));
		}
		void AppendBezier(GraphicsPath graphicsPath, PointF startPoint, PointF endPoint, PdfBezierGraphicsPathSegment segment) {
			graphicsPath.AddBezier(startPoint, interpreter.TransformPoint(segment.ControlPoint1), interpreter.TransformPoint(segment.ControlPoint2), endPoint);
		}
		void AppendPath(GraphicsPath graphicsPath, PdfGraphicsPath path) {
			RectangleF rectangle = CreateRectangle(path);
			if (rectangle.IsEmpty) {
				PointF startPoint = interpreter.TransformPoint(path.StartPoint);
				IList<PdfGraphicsPathSegment> segments = path.Segments;
				if (segments.Count == 1) {
					PdfGraphicsPathSegment segment = segments[0];
					PointF endPoint = interpreter.TransformPoint(segment.EndPoint);
					PdfBezierGraphicsPathSegment bezierSegment = segment as PdfBezierGraphicsPathSegment;
					if (bezierSegment == null) {
						if (startPoint == endPoint) {
							int rotationAngle = interpreter.DocumentState.RotationAngle;
							if (rotationAngle == 90 || rotationAngle == 270)
								graphicsPath.AddLine(startPoint, new PointF(endPoint.X, endPoint.Y + 1));
							else
								graphicsPath.AddLine(startPoint, new PointF(endPoint.X + 1, endPoint.Y));
						}
						else
							graphicsPath.AddLine(startPoint, endPoint);
					}
					else 
						AppendBezier(graphicsPath, startPoint, endPoint, bezierSegment);
				}
				else 
					foreach (PdfGraphicsPathSegment segment in segments) {
						PointF endPoint = interpreter.TransformPoint(segment.EndPoint);
						PdfBezierGraphicsPathSegment bezierSegment = segment as PdfBezierGraphicsPathSegment;
						if (bezierSegment == null)
							graphicsPath.AddLine(startPoint, endPoint);
						else
							AppendBezier(graphicsPath, startPoint, endPoint, bezierSegment);
						startPoint = endPoint;
					}
			}
			else
				graphicsPath.AddRectangle(rectangle);
			if (path.Closed)
				graphicsPath.CloseFigure();
			else
				graphicsPath.StartFigure();
		}
		void AppendExtendLine(GraphicsPath graphicsPath, PdfPoint startPoint, PdfPoint endPoint, double size, bool fromLeft) {
			double angle = PdfViewerCommandInterpreter.CalcAngle(startPoint, endPoint);
			double extendX = Math.Sin(angle) * size;
			double extendY = Math.Cos(angle) * size;
			if (fromLeft)
				graphicsPath.AddLine(interpreter.TransformPoint(new PdfPoint(startPoint.X - extendX, startPoint.Y - extendY)), interpreter.TransformPoint(startPoint));
			else
				graphicsPath.AddLine(interpreter.TransformPoint(endPoint), interpreter.TransformPoint(new PdfPoint(endPoint.X + extendX, endPoint.Y + extendY)));
		}
	}
}
