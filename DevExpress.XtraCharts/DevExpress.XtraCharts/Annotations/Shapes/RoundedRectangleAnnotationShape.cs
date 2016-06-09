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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class RoundedRectangleAnnotationShape : RectangleAnnotationShape {
		public RoundedRectangleAnnotationShape(IAnnotationShapeOwner annotation) : base(annotation) {
		}
		int GetIndexBySegmentKind(SegmentKind segmentKind) {
			switch (segmentKind) {
				case SegmentKind.LeftTop:
				case SegmentKind.Left:
					return 0;
				case SegmentKind.LeftBottom:
				case SegmentKind.Bottom:
					return 6;
				case SegmentKind.RightBottom:
				case SegmentKind.Right:
					return 4;
				case SegmentKind.RightTop:
				case SegmentKind.Top:
					return 2;
				default:
					ChartDebug.Fail("Unknown segment kind.");
					return 0;
			}
		}
		bool IsLineSegment(SegmentKind segmentKind) {
			switch (segmentKind) {
				case SegmentKind.LeftTop:
				case SegmentKind.RightTop:
				case SegmentKind.RightBottom:
				case SegmentKind.LeftBottom:
					return false;
				default:
					return true;
			}
		}
		void CalcArcParams(ZPlaneRectangle bounds, int fillet, int segmentIndex, out ZPlaneRectangle arcRect, out int startAngle) {
			DiagramPoint[] points = GetArcsPoints(bounds, fillet);
			int arcIndex = segmentIndex / 2;
			startAngle = 180 + arcIndex * 90;
			arcRect = new ZPlaneRectangle(points[arcIndex], fillet * 2, fillet * 2);
		}
		DiagramPoint[] GetLinesPoints(ZPlaneRectangle bounds, int fillet) {
			return new DiagramPoint[] { 
				new DiagramPoint(bounds.Left, bounds.Top - fillet), new DiagramPoint(bounds.Left, bounds.Bottom + fillet), 
				new DiagramPoint(bounds.Left + fillet, bounds.Bottom), new DiagramPoint(bounds.Right - fillet, bounds.Bottom), new DiagramPoint(bounds.Right, bounds.Bottom + fillet),
				new DiagramPoint(bounds.Right, bounds.Top - fillet), new DiagramPoint(bounds.Right - fillet, bounds.Top), new DiagramPoint(bounds.Left + fillet, bounds.Top)
			};
		}
		DiagramPoint[] GetArcsPoints(ZPlaneRectangle bounds, int fillet) {
			return new DiagramPoint[] { new DiagramPoint(bounds.Left, bounds.Bottom), new DiagramPoint(bounds.Right - fillet * 2, bounds.Bottom), 
				new DiagramPoint(bounds.Right - fillet * 2, bounds.Top - fillet * 2), new DiagramPoint(bounds.Left, bounds.Top - fillet * 2)
			};
		}
		protected override void CalcTangentLines(ZPlaneRectangle bounds, DiagramPoint anchorPoint, out double minAngle, out double maxAngle) {
			int fillet = GetCorrectedFillet(new Size((int)bounds.Width, (int)bounds.Height));
			if (fillet < 1) {
				base.CalcTangentLines(bounds, anchorPoint, out minAngle, out maxAngle);
				return;
			}
			DiagramPoint[] points = GetLinesPoints(bounds, fillet);
			minAngle = GeometricUtils.CalcBetweenPointsAngle((GRealPoint2D)anchorPoint, (GRealPoint2D)points[0]);
			maxAngle = Double.MinValue;
			for (int i = 1; i < points.Length; i++)
				ProsessAngle(GeometricUtils.CalcBetweenPointsAngle((GRealPoint2D)anchorPoint, (GRealPoint2D)points[i]), ref minAngle, ref maxAngle);
			points = GetArcsPoints(bounds, fillet);
			foreach (DiagramPoint point in points) {
				double angle1, angle2;
				if (MathUtils.CalcEllipseTangentLines(new ZPlaneRectangle(point, fillet * 2, fillet * 2), anchorPoint, out angle1, out angle2)) {
					ProsessAngle(angle1, ref minAngle, ref maxAngle);
					ProsessAngle(angle2, ref minAngle, ref maxAngle);
				}
			}
		}
		protected override IntersectionInfo CalcLineSegmentWithShapeIntersection(DiagramPoint segmentPoint1, DiagramPoint segmentPoint2, ZPlaneRectangle bounds) {
			int fillet = GetCorrectedFillet(new Size((int)bounds.Width, (int)bounds.Height));
			if (fillet < 1) {
				return GeometricUtils.CalcLineSegmentWithRectIntersection((GRealPoint2D)segmentPoint1, (GRealPoint2D)segmentPoint2,
					(GRealPoint2D)bounds.LeftBottom.Point, (GRealPoint2D)bounds.RightTop.Point);
			}
			else
				return GeometricUtils.CalcLineSegmentWithRoundedRectIntersection((GRealPoint2D)segmentPoint1, (GRealPoint2D)segmentPoint2,
					(GRealPoint2D)bounds.LeftBottom.Point, (GRealPoint2D)bounds.RightTop.Point, fillet);
		}
		protected override GraphicsPath CreateGraphicsPath(ZPlaneRectangle bounds) {
			int fillet = GetCorrectedFillet(new Size((int)bounds.Width, (int)bounds.Height));
			if (fillet < 1)
				return base.CreateGraphicsPath(bounds);
			DiagramPoint[] points = GetLinesPoints(bounds, fillet);
			DiagramPoint[] arcsPoints = GetArcsPoints(bounds, fillet);
			GraphicsPath path = new GraphicsPath();
			for (int i = 0; i < points.Length - 1; i += 2) {
				int arcIndex = i / 2;
				ZPlaneRectangle arcRect = new ZPlaneRectangle(arcsPoints[arcIndex], fillet * 2, fillet * 2);
				path.AddLine((PointF)points[i], (PointF)points[i + 1]);
				path.AddArc((RectangleF)arcRect, 180 + arcIndex * 90, 90);
			}
			return path;
		}
		protected override void CreateShape(GraphicsPath path, IntersectionInfo positiveIntersection, IntersectionInfo negativeIntersection, ZPlaneRectangle bounds) {
			int fillet = GetCorrectedFillet(new Size((int)bounds.Width, (int)bounds.Height));
			if (fillet < 1) {
				base.CreateShape(path, positiveIntersection, negativeIntersection, bounds);
				return;
			}
			DiagramPoint[] points = GetLinesPoints(bounds, fillet);
			int startIndex = GetIndexBySegmentKind(negativeIntersection.SegmentKind);
			int endIndex = GetIndexBySegmentKind(positiveIntersection.SegmentKind);
			int startAngle;
			ZPlaneRectangle arcRect;
			CalcArcParams(bounds, fillet, startIndex, out arcRect, out startAngle);
			DiagramPoint positivePoint = (DiagramPoint)positiveIntersection.IntersectionPoint;
			DiagramPoint negativePoint = (DiagramPoint)negativeIntersection.IntersectionPoint;
			if (IsLineSegment(negativeIntersection.SegmentKind)) {
				path.AddLine((PointF)negativePoint, (PointF)points[startIndex + 1]);
				path.AddArc((RectangleF)arcRect, startAngle, 90);
			}
			else {
				double angle1 = GeometricUtils.CalcBetweenPointsAngle((GRealPoint2D)arcRect.Center, (GRealPoint2D)negativePoint);
				double angle2 = GeometricUtils.NormalizeRadian(MathUtils.Degree2Radian(startAngle + 90));
				double sweepAngle = angle1 > angle2 ? 2 * Math.PI - (angle1 - angle2) : -(angle1 - angle2);
				path.AddArc((RectangleF)arcRect, (float)MathUtils.Radian2Degree(angle1), (float)MathUtils.Radian2Degree(sweepAngle));
			}
			for (int i = (startIndex + 2) % points.Length; i != endIndex; i = (i + 2) % points.Length) {
				CalcArcParams(bounds, fillet, i, out arcRect, out startAngle);
				path.AddLine((PointF)points[i], (PointF)points[i + 1]);
				path.AddArc((RectangleF)arcRect, startAngle, 90);
			}
			if (IsLineSegment(positiveIntersection.SegmentKind))
				path.AddLine((PointF)points[endIndex], (PointF)positivePoint);
			else {
				CalcArcParams(bounds, fillet, endIndex, out arcRect, out startAngle);
				path.AddLine((PointF)points[endIndex], (PointF)points[endIndex + 1]);
				double angle1 = GeometricUtils.NormalizeRadian(MathUtils.Degree2Radian(startAngle));
				double angle2 = GeometricUtils.CalcBetweenPointsAngle((GRealPoint2D)arcRect.Center, (GRealPoint2D)positivePoint);
				double sweepAngle = angle1 > angle2 ? 2 * Math.PI - (angle1 - angle2) : -(angle1 - angle2);
				path.AddArc((RectangleF)arcRect, (float)MathUtils.Radian2Degree(angle1), (float)MathUtils.Radian2Degree(sweepAngle));
			}
		}
		protected internal override bool IsPointInsideShape(DiagramPoint point, ZPlaneRectangle bounds) {
			int fillet = GetCorrectedFillet(new Size((int)bounds.Width, (int)bounds.Height));
			if (fillet < 1)
				return base.IsPointInsideShape(point, bounds);
			ZPlaneRectangle rect = new ZPlaneRectangle(new DiagramPoint(bounds.Left + fillet, bounds.Bottom), bounds.Width - fillet * 2, bounds.Height);
			if (rect.Contains(point))
				return true;
			rect = new ZPlaneRectangle(new DiagramPoint(bounds.Left, bounds.Bottom + fillet), bounds.Width, bounds.Height - fillet * 2);
			if (rect.Contains(point))
				return true;
			DiagramPoint[] points = GetArcsPoints(bounds, fillet);
			foreach (DiagramPoint rectLocation in points) {
				if (MathUtils.IsPointInsideEllipse(point, new ZPlaneRectangle(rectLocation, fillet * 2, fillet * 2)))
					return true;
			}
			return false;
		}
		internal int GetCorrectedFillet(Size size) {
			int minHalfSize = (int)Math.Min(size.Width / 2, size.Height / 2);
			return minHalfSize < Annotation.ShapeFillet ? minHalfSize : Annotation.ShapeFillet;
		}
		public override Size CalcInnerSize(Size outerSize) {
			int fillet = GetCorrectedFillet(outerSize);
			if (outerSize.Width < outerSize.Height)
				return new Size(outerSize.Width, outerSize.Height - fillet * 2);
			else
				return new Size(outerSize.Width - fillet * 2, outerSize.Height);
		}
		public override Size CalcOuterSize(Size innerSize) {
			if (innerSize.Width < innerSize.Height)
				return new Size(innerSize.Width, innerSize.Height + Annotation.ShapeFillet * 2);
			else
				return new Size(innerSize.Width + Annotation.ShapeFillet * 2, innerSize.Height);
		}
	}
}
