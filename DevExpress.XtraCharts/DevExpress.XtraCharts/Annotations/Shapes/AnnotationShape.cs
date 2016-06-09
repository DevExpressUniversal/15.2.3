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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public interface IAnnotationShapeOwner : IHitTest {
		bool BorderVisible { get; }
		int BorderThickness { get; }
		Color BorderColor { get; }
		RectangleFillStyle FillStyle { get; }
		Color BackColor { get; }
		int ShapeFillet { get; }
		Shadow Shadow { get; }
		AnnotationConnectorStyle ConnectorStyle { get; }
		int Angle { get; }
	}
	public class AdjectiveConnectorInfo {
		double angle;
		double sweepAngle;
		DiagramPoint centerPoint;
		IntersectionInfo positiveIntersection;
		IntersectionInfo negativeIntersection;
		public double Angle { get { return angle; } }
		public double SweepAngle { get { return sweepAngle; } }
		public DiagramPoint CenterPoint { get { return centerPoint; } }
		public IntersectionInfo PositiveIntersection { get { return positiveIntersection; } }
		public IntersectionInfo NegativeIntersection { get { return negativeIntersection; } }
		public AdjectiveConnectorInfo(double angle, double sweepAngle, DiagramPoint centerPoint, IntersectionInfo positiveIntersection, IntersectionInfo negativeIntersection) {
			this.angle = angle;
			this.sweepAngle = sweepAngle;
			this.centerPoint = centerPoint;
			this.positiveIntersection = positiveIntersection;
			this.negativeIntersection = negativeIntersection;
		}
	}
	public abstract class AnnotationShape {
		class ArrowInfo {
			DiagramPoint arrowPoint;
			DiagramPoint arrowConnectionPoint;
			public DiagramPoint ArrowPoint { get { return arrowPoint; } }
			public DiagramPoint ArrowConnectionPoint { get { return arrowConnectionPoint; } }
			public ArrowInfo(DiagramPoint arrowPoint, DiagramPoint arrowConnectionPoint) {
				this.arrowPoint = arrowPoint;
				this.arrowConnectionPoint = arrowConnectionPoint;
			}
		}
		static DiagramPoint Rotate(DiagramPoint point, float angle) {
			Matrix rotationMatrix = new Matrix();
			rotationMatrix.Rotate(angle);
			PointF[] points = { (PointF)point };
			rotationMatrix.TransformPoints(points);
			return (DiagramPoint)points[0];
		}
		public static AnnotationShape CreateInstance(Annotation annotation) {
			switch (annotation.ShapeKind) {
				case ShapeKind.Rectangle:
					return new RectangleAnnotationShape(annotation);
				case ShapeKind.RoundedRectangle:
					return new RoundedRectangleAnnotationShape(annotation);
				case ShapeKind.Ellipse:
					return new EllipseAnnotationShape(annotation);
				default:
					ChartDebug.Fail("Unknown annotation shape kind");
					return new RectangleAnnotationShape(annotation);
			}
		}
		const int DefaultArrowLength = 13;
		const double AdditionalArrowAngle = 0.262;
		readonly IAnnotationShapeOwner annotation;
		int ActualBorderThickness {
			get { return !((IHitTest)annotation).State.Normal ? annotation.BorderThickness + 1 : annotation.BorderThickness; }
		}
		bool BorderVisible { get { return annotation.BorderVisible || !((IHitTest)annotation).State.Normal; } }
		FillOptionsBase FillOptions { get { return annotation.FillStyle.Options; } }
		protected IAnnotationShapeOwner Annotation { get { return annotation; } }
		public AnnotationShape(IAnnotationShapeOwner annotation) {
			this.annotation = annotation;
		}
		void RenderLineConnector(IRenderer renderer, ZPlaneRectangle bounds, DiagramPoint anchorPoint, int thickness, Color color) {
			DiagramPoint? finishPoint = GetLineConnectorFinishPoint(bounds, anchorPoint);
			if (finishPoint != null && finishPoint.HasValue) {
				Point startPoint = new Point((int)Math.Floor(anchorPoint.X), (int)Math.Floor(anchorPoint.Y));
				if (startPoint.X == finishPoint.Value.X || startPoint.Y == finishPoint.Value.Y)
					renderer.DrawLine(startPoint, (Point)finishPoint.Value, color, thickness);
				else {
					renderer.EnableAntialiasing(true);
					renderer.DrawLine(startPoint, (Point)finishPoint.Value, color, thickness);
					renderer.RestoreAntialiasing();					
				}				
			}
		}
		static void ReduceSweepAngle(double delta, ref double angle1, ref double angle2) { 
			double difference = CalcBetweenAnglesMinDifference(angle2, angle1);
			if (difference > 0) {
				angle2 -= delta;
				angle1 += delta;
			}
			else {
				angle2 += delta;
				angle1 -= delta;
			}
		}
		double CorrectAngle(ZPlaneRectangle rect, DiagramPoint anchorPoint, double angle) {
			double limitAngle1, limitAngle2;
			CalcTangentLines(rect, anchorPoint, out limitAngle1, out limitAngle2);
			ReduceSweepAngle(0.001, ref limitAngle1, ref limitAngle2);
			double sweepAngle = CalcBetweenAnglesMinDifference(limitAngle2, limitAngle1);
			double difference1 = CalcBetweenAnglesMinDifference(angle, limitAngle1);
			double difference2 = CalcBetweenAnglesMinDifference(angle, limitAngle2);
			bool shouldCorrect = false;
			if (sweepAngle > 0) {
				if (difference1 < 0)
					shouldCorrect = true;
				else if (difference1 > sweepAngle)
					shouldCorrect = true;
			}
			else {
				if (difference1 > 0)
					shouldCorrect = true;
				else if (difference1 < sweepAngle)
					shouldCorrect = true;
			}
			if (shouldCorrect) {
				if (Math.Abs(difference1) < Math.Abs(difference2))
					return limitAngle1;
				return limitAngle2;
			}
			return angle;
		}
		ArrowInfo CalcArrowInfo(DiagramPoint anchorPoint, DiagramPoint connectionPoint, AdjectiveConnectorInfo connectorInfo, bool isNegative) {
			double sweepAngle = GeometricUtils.NormalizeRadian(connectorInfo.SweepAngle / 2 + AdditionalArrowAngle);
			double additionalArrowAngleCos = Math.Cos(AdditionalArrowAngle);
			double minSideLength = Math.Min(MathUtils.CalcLength2D((DiagramPoint)connectorInfo.PositiveIntersection.IntersectionPoint, anchorPoint),
				MathUtils.CalcLength2D((DiagramPoint)connectorInfo.NegativeIntersection.IntersectionPoint, anchorPoint)) * additionalArrowAngleCos;
			double arrowLength = Math.Min(DefaultArrowLength + Annotation.BorderThickness, Math.Floor(minSideLength));
			double sweepAngleCos = Math.Cos(sweepAngle);
			double centerLength = MathUtils.CalcLength2D(anchorPoint, connectorInfo.CenterPoint);
			if (arrowLength - centerLength == arrowLength)
				return null;
			double factor = arrowLength / centerLength;
			if (arrowLength - sweepAngleCos == arrowLength)
				return null;
			double length = arrowLength / sweepAngleCos;
			double angle = isNegative ? -sweepAngle : sweepAngle;
			angle = GeometricUtils.NormalizeRadian(connectorInfo.Angle + angle);
			if (Annotation.ConnectorStyle == AnnotationConnectorStyle.NotchedArrow)
				factor /= 2;
			DiagramPoint arrowConnectionPoint = new DiagramPoint(anchorPoint.X + (connectorInfo.CenterPoint.X - anchorPoint.X) * factor,
				anchorPoint.Y + (connectorInfo.CenterPoint.Y - anchorPoint.Y) * factor);
			DiagramPoint arrowPoint = new DiagramPoint(anchorPoint.X + length * Math.Cos(angle), anchorPoint.Y + length * Math.Sin(angle));
			GRealPoint2D? point = GeometricUtils.CalcLinesIntersection((GRealPoint2D)arrowPoint, (GRealPoint2D)arrowConnectionPoint,
				(GRealPoint2D)anchorPoint, (GRealPoint2D)connectionPoint, false);
			return point.HasValue ? new ArrowInfo(arrowPoint, (DiagramPoint)point.Value) : null;
		}
		DiagramPoint? GetLineConnectorFinishPoint(ZPlaneRectangle bounds, DiagramPoint anchorPoint) {
			anchorPoint = Rotate(anchorPoint, -annotation.Angle);
			if (Annotation.ConnectorStyle == AnnotationConnectorStyle.Line && !IsPointInsideShape(anchorPoint, bounds)) {
				GraphicsPath linePath = new GraphicsPath();
				IntersectionInfo intersection = CalcLineSegmentWithShapeIntersection(anchorPoint, bounds.Center, bounds);
				if (intersection.HasIntersection) {
					DiagramPoint intersectionPoint = Rotate((DiagramPoint)intersection.IntersectionPoint, annotation.Angle);
					return new DiagramPoint?(new DiagramPoint(Math.Floor(intersectionPoint.X), Math.Floor(intersectionPoint.Y)));
				}
			}
			return null;
		}
		GraphicsPath CreateGraphicsPathWithConnector(ZPlaneRectangle bounds, DiagramPoint anchorPoint) {
			GraphicsPath path = new GraphicsPath();
			AdjectiveConnectorInfo connectorInfo = CalcAdjectiveConnectorPoints(bounds, anchorPoint);
			if (connectorInfo == null)
				return path;
			DiagramPoint positivePoint = (DiagramPoint)connectorInfo.PositiveIntersection.IntersectionPoint;
			DiagramPoint negativePoint = (DiagramPoint)connectorInfo.NegativeIntersection.IntersectionPoint;
			if (Annotation.ConnectorStyle == AnnotationConnectorStyle.Tail)
				path.AddLine((PointF)anchorPoint, (PointF)negativePoint);
			else {
				ArrowInfo arrowInfo = CalcArrowInfo(anchorPoint, negativePoint, connectorInfo, true);
				if (arrowInfo != null) {
					path.AddLine((PointF)anchorPoint, (PointF)arrowInfo.ArrowPoint);
					path.AddLine((PointF)arrowInfo.ArrowPoint, (PointF)arrowInfo.ArrowConnectionPoint);
					path.AddLine((PointF)arrowInfo.ArrowConnectionPoint, (PointF)negativePoint);
				}
			}
			CreateShape(path, connectorInfo.PositiveIntersection, connectorInfo.NegativeIntersection, bounds);
			if (Annotation.ConnectorStyle == AnnotationConnectorStyle.Tail)
				path.AddLine((PointF)positivePoint, (PointF)anchorPoint);
			else {
				ArrowInfo arrowInfo = CalcArrowInfo(anchorPoint, positivePoint, connectorInfo, false);
				if (arrowInfo != null) {
					path.AddLine((PointF)positivePoint, (PointF)arrowInfo.ArrowConnectionPoint);
					path.AddLine((PointF)arrowInfo.ArrowConnectionPoint, (PointF)arrowInfo.ArrowPoint);
					path.AddLine((PointF)arrowInfo.ArrowPoint, (PointF)anchorPoint);
				}
			}
			return path;
		}
		void RenderBorder(IRenderer renderer, ZPlaneRectangle bounds, DiagramPoint anchorPoint) {
			if (!BorderVisible)
				return;
			ZPlaneRectangle rect = CorrectRectangle(bounds);
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(GetPixelOffsetMode(bounds));
			if (!rect.AreWidthAndHeightPositive()) {
				using (GraphicsPath path = CreateGraphicsPath(bounds, anchorPoint))
					renderer.FillPath(path, Annotation.BorderColor);
				renderer.DrawLine((Point)anchorPoint, (Point)bounds.Center, Annotation.BorderColor, 1);
			}
			else
				using (GraphicsPath path = CreateGraphicsPath(rect, anchorPoint))
					renderer.DrawPath(path, Annotation.BorderColor, ActualBorderThickness);
			RenderLineConnector(renderer, bounds, anchorPoint, ActualBorderThickness, Annotation.BorderColor);
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();
		}
		void RenderFill(IRenderer renderer, ZPlaneRectangle bounds, DiagramPoint anchorPoint) {
			ZPlaneRectangle clippingRect = CorrectRectangle(bounds);
			bool borderVisible = BorderVisible;
			if (borderVisible) {
				if (!clippingRect.AreWidthAndHeightPositive())
					return;
				renderer.SetClipping(CreateGraphicsPath(clippingRect, anchorPoint), CombineMode.Intersect);
			}
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.HighQuality);
			Color color = Annotation.BackColor;
			GraphicsPath path = CreateGraphicsPath(bounds, anchorPoint);
			if (FillOptions == null)
				renderer.FillPath(path, color);
			else {
				Color color2 = FillOptions is FillOptionsColor2Base ? ((FillOptionsColor2Base)(FillOptions)).Color2 : color;
				renderer.DrawPath(FillOptions, path, new Rectangle((int)path.GetBounds().X, (int)path.GetBounds().Y, (int)path.GetBounds().Width, (int)path.GetBounds().Height), color, color2);
			}
			renderer.RestorePixelOffsetMode();
			if (!BorderVisible)
				RenderLineConnector(renderer, bounds, anchorPoint, 1, color);
			renderer.RestoreAntialiasing();
			if (borderVisible)
				renderer.RestoreClipping();
		}
		PixelOffsetMode GetPixelOffsetMode(ZPlaneRectangle rect) {
			int rotationAngleInDegree = (int)MathUtils.NormalizeDegree(Annotation.Angle);
			if (((rotationAngleInDegree >= 45 && rotationAngleInDegree < 135) ||
				(rotationAngleInDegree > 225 && rotationAngleInDegree <= 315)) &&
				(int)(rect.Height + rect.Width) % 2 != 0)
				return ActualBorderThickness % 2 != 0 ? PixelOffsetMode.HighQuality : PixelOffsetMode.Default;
			return ActualBorderThickness % 2 == 0 ? PixelOffsetMode.HighQuality : PixelOffsetMode.Default;
		}
		ZPlaneRectangle CorrectRectangle(ZPlaneRectangle rect) {
			int rotationAngleInDegree = (int)MathUtils.NormalizeDegree(Annotation.Angle);
			int xOffset, yOffset;
			if (rotationAngleInDegree >= 0 && rotationAngleInDegree < 45 ||
				rotationAngleInDegree > 315 && rotationAngleInDegree <= 360) {
				xOffset = yOffset = ActualBorderThickness / 2;
			}
			else {
				if (rotationAngleInDegree >= 45 && rotationAngleInDegree < 135) {
					if ((int)(rect.Height + rect.Width) % 2 == 0) {
						xOffset = ActualBorderThickness / 2;
						yOffset = MathUtils.Ceiling(ActualBorderThickness / 2.0);
					}
					else {
						xOffset = MathUtils.Ceiling(ActualBorderThickness / 2.0);
						yOffset = ActualBorderThickness / 2;
					}
				}
				else {
					if (rotationAngleInDegree > 225 && rotationAngleInDegree <= 315) {
						if ((int)(rect.Height + rect.Width) % 2 == 0) {
							xOffset = MathUtils.Ceiling(ActualBorderThickness / 2.0);
							yOffset = ActualBorderThickness / 2;
						}
						else {
							xOffset = ActualBorderThickness / 2;
							yOffset = MathUtils.Ceiling(ActualBorderThickness / 2.0);
						}
					}
					else
						xOffset = yOffset = MathUtils.Ceiling(ActualBorderThickness / 2.0);
				}
			}
			ZPlaneRectangle result = (ZPlaneRectangle)ZPlaneRectangle.Offset(rect, xOffset, yOffset, 0);
			result.Width = result.Width - ActualBorderThickness < 0 ? 0 : result.Width - ActualBorderThickness;
			result.Height = result.Height - ActualBorderThickness < 0 ? 0 : result.Height - ActualBorderThickness;
			return result;
		}
		protected static void ProsessAngle(double angle, ref double minAngle, ref double maxAngle) {
			double difference = CalcBetweenAnglesMinDifference(angle, minAngle);
			if (difference < 0)
				minAngle = angle;
			difference = CalcBetweenAnglesMinDifference(angle, maxAngle);
			if (difference > 0)
				maxAngle = angle;
		}
		protected static double CalcBetweenAnglesMinDifference(double minuend, double deducts) {
			minuend = GeometricUtils.NormalizeRadian(minuend);
			deducts = GeometricUtils.NormalizeRadian(deducts);
			double difference = minuend - deducts;
			if (difference > Math.PI)
				return difference - 2 * Math.PI;
			if (difference < -Math.PI)
				return difference + 2 * Math.PI;
			return difference;
		}
		protected abstract void CalcTangentLines(ZPlaneRectangle bounds, DiagramPoint anchorPoint, out double angle1, out double angle2);
		protected abstract void CreateShape(GraphicsPath path, IntersectionInfo positiveIntersection, IntersectionInfo negativeIntersection, ZPlaneRectangle bounds);
		protected abstract IntersectionInfo CalcLineSegmentWithShapeIntersection(DiagramPoint segmentPoint1, DiagramPoint segmentPoint2, ZPlaneRectangle bounds);
		protected abstract GraphicsPath CreateGraphicsPath(ZPlaneRectangle bounds);
		protected internal abstract bool IsPointInsideShape(DiagramPoint point, ZPlaneRectangle bounds);
		internal AdjectiveConnectorInfo CalcAdjectiveConnectorPoints(ZPlaneRectangle rect, DiagramPoint anchorPoint) {
			IntersectionInfo intersection = CalcLineSegmentWithShapeIntersection(anchorPoint, rect.Center, rect);
			DiagramPoint point = intersection.HasIntersection ? (DiagramPoint)intersection.IntersectionPoint : rect.Center;				
			double length = MathUtils.CalcLength2D(anchorPoint, point);
			double anchorSize = 0.15 * Math.Min(rect.Width, rect.Height);
			double sweepAngle = Math.Atan2(anchorSize, length);
			double angle = GeometricUtils.CalcBetweenPointsAngle((GRealPoint2D)anchorPoint, (GRealPoint2D)rect.Center);
			double positiveAngle = GeometricUtils.NormalizeRadian(angle + sweepAngle / 2);
			double negativeAngle = GeometricUtils.NormalizeRadian(angle - sweepAngle / 2);
			positiveAngle = CorrectAngle(rect, anchorPoint, positiveAngle);
			negativeAngle = CorrectAngle(rect, anchorPoint, negativeAngle);
			length += 2 * Math.Max(rect.Width, rect.Height);
			DiagramPoint positivePoint = new DiagramPoint(anchorPoint.X + length * Math.Cos(positiveAngle), anchorPoint.Y + length * Math.Sin(positiveAngle));
			DiagramPoint negativePoint = new DiagramPoint(anchorPoint.X + length * Math.Cos(negativeAngle), anchorPoint.Y + length * Math.Sin(negativeAngle));
			IntersectionInfo positiveIntersection = CalcLineSegmentWithShapeIntersection(anchorPoint, positivePoint, rect);
			IntersectionInfo negativeIntersection = CalcLineSegmentWithShapeIntersection(anchorPoint, negativePoint, rect);
			return !positiveIntersection.HasIntersection || !negativeIntersection.HasIntersection ? null :
				new AdjectiveConnectorInfo(angle, sweepAngle, point, positiveIntersection, negativeIntersection);
		}
		internal GraphicsPath CreateGraphicsPath(ZPlaneRectangle bounds, DiagramPoint anchorPoint) {
			anchorPoint = Rotate(anchorPoint, -annotation.Angle);
			GraphicsPath path;
			if (Annotation.ConnectorStyle == AnnotationConnectorStyle.None || Annotation.ConnectorStyle == AnnotationConnectorStyle.Line
				|| IsPointInsideShape(anchorPoint, bounds))
				path = CreateGraphicsPath(bounds);
			else
				path = CreateGraphicsPathWithConnector(bounds, anchorPoint);
			Matrix rotationMatrix = new Matrix();
			rotationMatrix.Rotate(annotation.Angle);
			path.Transform(rotationMatrix);
			return path; 
		}
		public abstract Size CalcInnerSize(Size outerSize);
		public abstract Size CalcOuterSize(Size innerSize);
		public void RenderShadow(IRenderer renderer, ZPlaneRectangle bounds, DiagramPoint anchorPoint) {
			if (!Annotation.Shadow.Visible || !bounds.AreWidthAndHeightPositive())
				return;
			Annotation.Shadow.BeforeShadowRender(renderer);
			Point offset = new Point((int)-bounds.Center.X, (int)-bounds.Center.Y);
			renderer.SaveState();
			renderer.TranslateModel(new Point(-offset.X, -offset.Y));
			using (GraphicsPath path = CreateGraphicsPath((ZPlaneRectangle)ZPlaneRectangle.Offset(bounds, offset.X, offset.Y, 0), DiagramPoint.Offset(anchorPoint, offset.X, offset.Y, 0)))
				renderer.FillPath(path, Annotation.Shadow.Color);
			renderer.RestoreState();
			Annotation.Shadow.AfterShadowRender(renderer);
		}
		public void Render(IRenderer renderer, ZPlaneRectangle bounds, DiagramPoint anchorPoint) {
			if (!bounds.AreWidthAndHeightPositive())
				return;
			renderer.SaveState();
			DiagramPoint offset = new DiagramPoint(-bounds.Center.X, -bounds.Center.Y);
			renderer.TranslateModel((float)-offset.X, (float)-offset.Y);
			anchorPoint = DiagramPoint.Offset(anchorPoint, offset.X, offset.Y, 0);
			bounds = (ZPlaneRectangle)ZPlaneRectangle.Offset(bounds, offset.X, offset.Y, 0);
			RenderFill(renderer, bounds, anchorPoint);
			RenderBorder(renderer, bounds, anchorPoint);
			renderer.RestoreState();
		}
		public HitRegion CreateHitRegion(ZPlaneRectangle bounds, DiagramPoint anchorPoint) {
			DiagramPoint offset = new DiagramPoint(-bounds.Center.X, -bounds.Center.Y);
			anchorPoint = DiagramPoint.Offset(anchorPoint, offset.X, offset.Y, 0);
			ZPlaneRectangle actualBounds = (ZPlaneRectangle)ZPlaneRectangle.Offset(bounds, offset.X, offset.Y, 0);
			GraphicsPath path = CreateGraphicsPath(actualBounds, anchorPoint);
			DiagramPoint? finishPoint = GetLineConnectorFinishPoint(actualBounds, anchorPoint);
			if (finishPoint != null && finishPoint.HasValue && finishPoint.Value != new DiagramPoint(Math.Floor(anchorPoint.X), Math.Floor(anchorPoint.Y))) {
				GraphicsPath linePath = new GraphicsPath();
				linePath.AddLine(new Point((int)Math.Floor(anchorPoint.X), (int)Math.Floor(anchorPoint.Y)), (Point)finishPoint.Value);
				using (Pen pen = new Pen(Annotation.BorderColor, ActualBorderThickness))
					linePath.Widen(pen);
				path.CloseFigure();
				path.AddPath(linePath, false);
			}
			Matrix translateTransform = new Matrix();
			translateTransform.Translate((float)(-offset.X), (float)(-offset.Y));
			path.Transform(translateTransform);
			return new HitRegion(path);
		}
	}
}
