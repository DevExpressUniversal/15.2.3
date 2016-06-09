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
namespace DevExpress.Charts.Native {
	public enum SegmentKind { 
		None,
		Left, 
		LeftBottom,
		Bottom,
		RightBottom,
		Right,
		RightTop,
		Top,
		LeftTop
	}
	internal static class SegmentKindHelper {
		public static SegmentKind GetKindForLineSegment(int index) {
			switch (index) {
				case 0:
					return SegmentKind.Left;
				case 1:
					return SegmentKind.Bottom;
				case 2:
					return SegmentKind.Right;
				case 3:
					return SegmentKind.Top;
				default:
					return SegmentKind.None;
			}	
		}
		public static SegmentKind GetKindForArcSegment(int index) {
			switch (index) {
				case 0:
					return SegmentKind.RightBottom ;
				case 1:
					return SegmentKind.LeftBottom;
				case 2:
					return SegmentKind.LeftTop;
				case 3:
					return SegmentKind.RightTop;
				default:
					return SegmentKind.None;
			}
		}
	}
	public struct IntersectionInfo {
		GRealPoint2D? intersectionPoint;
		SegmentKind segmentKind;
		public bool HasIntersection { get { return intersectionPoint.HasValue; } }
		public GRealPoint2D IntersectionPoint { get { return intersectionPoint.Value; } set { intersectionPoint = value; } }
		public SegmentKind SegmentKind { get { return segmentKind; } set { segmentKind = value; } }
		public IntersectionInfo(GRealPoint2D intersectionPoint, SegmentKind segmentKind) {
			this.intersectionPoint = intersectionPoint;
			this.segmentKind = segmentKind;
		}
	}
	public static class GeometricUtils {
		public static int StrongRound(double value) {
			return Math.Sign(value) * (int)(Math.Abs(value) + 0.5);
		}
		public static double CalcDistance(GRealPoint2D point1, GRealPoint2D point2) {
			double dx = point1.X - point2.X;
			double dy = point1.Y - point2.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}
		public static double CalcDistance(GRealPoint2D point1, GRealPoint2D point2, bool isHorizontalDistance) {
			return Math.Abs(isHorizontalDistance ? point1.X - point2.X : point1.Y - point2.Y);
		}
		public static GRealPoint2D CalcPointInLine(GRealPoint2D start, GRealPoint2D end, double ration) {
			double dx = end.X - start.X; double dy = end.Y - start.Y;
			return new GRealPoint2D(start.X + dx * ration, start.Y + dy * ration);
		}
		static List<GRealPoint2D> CalcLineWithEllipseIntersection(GRealPoint2D segmentPoint1, GRealPoint2D segmentPoint2, GRealPoint2D leftTopEllipseRect, GRealPoint2D rightBottomEllipseRect) {
			double dx = segmentPoint2.X - segmentPoint1.X;
			double dy = segmentPoint2.Y - segmentPoint1.Y;
			double centerX = (leftTopEllipseRect.X + rightBottomEllipseRect.X) / 2;
			double centerY = (leftTopEllipseRect.Y + rightBottomEllipseRect.Y) / 2;
			segmentPoint1 = new GRealPoint2D(segmentPoint1.X - centerX, segmentPoint1.Y - centerY);
			double B2 = (rightBottomEllipseRect.Y - leftTopEllipseRect.Y) * (rightBottomEllipseRect.Y - leftTopEllipseRect.Y) / 4;
			double A2 = (rightBottomEllipseRect.X - leftTopEllipseRect.X) * (rightBottomEllipseRect.X - leftTopEllipseRect.X) / 4;
			double a = B2 * dx * dx + A2 * dy * dy;
			double b = (segmentPoint1.X * dx * B2 + segmentPoint1.Y * dy * A2) * 2;
			double c = segmentPoint1.X * segmentPoint1.X * B2 + segmentPoint1.Y * segmentPoint1.Y * A2 - A2 * B2;
			List<GRealPoint2D> intersectionPoints = new List<GRealPoint2D>();
			List<double> roots = CalcQuadraticEquation(a, b, c);
			double minDistance = Double.MaxValue;
			foreach (double root in roots) {
				if (root < 0 || root > 1)
					continue;
				GRealPoint2D point = new GRealPoint2D(segmentPoint1.X + root * dx, segmentPoint1.Y + root * dy);
				double distance = CalcDistance(segmentPoint1, point);
				if (intersectionPoints.Count == 0 || distance < minDistance) {
					intersectionPoints.Insert(0, new GRealPoint2D(point.X + centerX, point.Y + centerY));
					minDistance = distance;
				}
				else
					intersectionPoints.Add(new GRealPoint2D(point.X + centerX, point.Y + centerY));
			}
			return intersectionPoints;
		}
		static List<GRealPoint2D> CalcLineSegmentWithArcIntersection(GRealPoint2D segmentPoint1, GRealPoint2D segmentPoint2,
			GRealPoint2D leftTopEllipseRect, GRealPoint2D rightBottomEllipseRect, double startAngle, double sweepAngle) {
			List<GRealPoint2D> intersectionPoints = CalcLineWithEllipseIntersection(segmentPoint1, segmentPoint2, leftTopEllipseRect, rightBottomEllipseRect);
			if (intersectionPoints.Count == 0)
				return intersectionPoints;
			startAngle = NormalizeRadian(startAngle);
			double endAngle = NormalizeRadian(startAngle + sweepAngle);
			GRealPoint2D center = new GRealPoint2D((leftTopEllipseRect.X + rightBottomEllipseRect.X) / 2, (leftTopEllipseRect.Y + rightBottomEllipseRect.Y) / 2);
			foreach (GRealPoint2D point in intersectionPoints.GetRange(0, intersectionPoints.Count)) {
				double currentAngle = CalcBetweenPointsAngle(center, point);
				if ((currentAngle < startAngle || currentAngle > startAngle + sweepAngle) &&
					(currentAngle < endAngle - sweepAngle || currentAngle > endAngle))
					intersectionPoints.Remove(point);
			}
			return intersectionPoints;
		}
		public static double CalcBetweenPointsAngle(GRealPoint2D p1, GRealPoint2D p2) {
			double angle = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
			return NormalizeRadian(angle);
		}
		public static List<double> CalcQuadraticEquation(double a, double b, double c) {
			List<double> roots = new List<double>();
			if (b - a == b) {
				if (c - b !=  c)
					roots.Add(-c / b);
				return roots;
			}
			double d = b * b - 4 * a * c;
			if (d < 0)
				return roots;
			if (d == 0) {
				roots.Add(-b / (2 * a));
				return roots;
			}
			roots.Add((-b - Math.Sqrt(d)) / (2 * a));
			roots.Add((-b + Math.Sqrt(d)) / (2 * a));
			return roots;
		}
		public static GRealRect2D CalcBounds(IList<GRealPoint2D> points) {
			if (points.Count == 0)
				return GRealRect2D.Empty;
			double left = points[0].X;
			double right = points[0].X;
			double bottom = points[0].Y;
			double top = points[0].Y;
			for (int i = 1; i < points.Count; i++) {
				GRealPoint2D point = points[i];
				if (point.X < left)
					left = point.X;
				else if (point.X > right)
					right = point.X;
				if (point.Y < bottom)
					bottom = point.Y;
				else if (point.Y > top)
					top = point.Y;
			}
			return new GRealRect2D(new GRealPoint2D(left, bottom), new GRealPoint2D(right, top));
		}
		public static GRealPoint2D? CalcLinesIntersection(GRealPoint2D line1P1, GRealPoint2D line1P2, GRealPoint2D line2P1, GRealPoint2D line2P2, bool intervalMode) {
			if (line1P1 == line2P1 || line1P1 == line2P2)
				return line1P1;
			if (line1P2 == line2P2 || line1P2 == line2P1)
				return line1P2;
			double denominator = (line2P2.Y - line2P1.Y) * (line1P2.X - line1P1.X) - (line2P2.X - line2P1.X) * (line1P2.Y - line1P1.Y);
			double numerator1 = (line2P2.X - line2P1.X) * (line1P1.Y - line2P1.Y) - (line2P2.Y - line2P1.Y) * (line1P1.X - line2P1.X);
			if (numerator1 + denominator == numerator1)
				return null;
			double factor1 = numerator1 / denominator;
			if (intervalMode && (factor1 < 0 || factor1 > 1))
				return null;
			double numerator2 = (line1P2.X - line1P1.X) * (line1P1.Y - line2P1.Y) - (line1P2.Y - line1P1.Y) * (line1P1.X - line2P1.X);
			if (numerator2 + denominator == numerator2)
				return null;
			double factor2 = numerator2 / denominator;
			if (intervalMode && (factor2 < 0 || factor2 > 1))
				return null;
			return new GRealPoint2D(line1P1.X + factor1 * (line1P2.X - line1P1.X), line1P1.Y + factor1 * (line1P2.Y - line1P1.Y));
		}
		public static IntersectionInfo CalcLineSegmentWithRectIntersection(GRealPoint2D segmentPoint1, GRealPoint2D segmentPoint2, GRealPoint2D leftTopRectPoint, GRealPoint2D rightBottomRectPoint) {
			IntersectionInfo result = new IntersectionInfo();
			double minDistance = Double.MaxValue;
			GRealPoint2D[] points = { leftTopRectPoint, new GRealPoint2D(leftTopRectPoint.X, rightBottomRectPoint.Y), 
				rightBottomRectPoint, new GRealPoint2D(rightBottomRectPoint.X, leftTopRectPoint.Y), leftTopRectPoint};
			for (int i = 0; i < points.Length - 1; i++) {
				GRealPoint2D? intersectionPoint = CalcLinesIntersection(segmentPoint1, segmentPoint2, points[i], points[i + 1], true);
				if (intersectionPoint != null) {
					double distance = CalcDistance(segmentPoint1, intersectionPoint.Value);
					if (!result.HasIntersection || distance < minDistance) {
						result = new IntersectionInfo(intersectionPoint.Value, SegmentKindHelper.GetKindForLineSegment(i));
						minDistance = distance;
					}
				}
			}
			return result;
		}
		public static IntersectionInfo CalcLineSegmentWithRoundedRectIntersection(GRealPoint2D startPoint, GRealPoint2D endPoint, GRealPoint2D leftTopRectPoint, GRealPoint2D rightBottomRectPoint, double fillet) {
			return CalcLineSegmentWithRoundedRectIntersection(startPoint, endPoint, leftTopRectPoint, rightBottomRectPoint, fillet, fillet, fillet, fillet);
		}
		public static IntersectionInfo CalcLineSegmentWithRoundedRectIntersection(GRealPoint2D startPoint, GRealPoint2D endPoint, GRealPoint2D leftTopRectPoint,
			GRealPoint2D rightBottomRectPoint, double leftBottomFillet, double leftTopFillet, double rightTopFillet, double rightBottomFillet) {
			GRealPoint2D[] points = {
				new GRealPoint2D(leftTopRectPoint.X, leftTopRectPoint.Y + leftTopFillet), 
				new GRealPoint2D(leftTopRectPoint.X, rightBottomRectPoint.Y - leftBottomFillet),
				new GRealPoint2D(leftTopRectPoint.X + leftBottomFillet, rightBottomRectPoint.Y),
				new GRealPoint2D(rightBottomRectPoint.X - rightBottomFillet, rightBottomRectPoint.Y),
				new GRealPoint2D(rightBottomRectPoint.X, rightBottomRectPoint.Y - rightBottomFillet),
				new GRealPoint2D(rightBottomRectPoint.X, leftTopRectPoint.Y + rightTopFillet),
				new GRealPoint2D(rightBottomRectPoint.X - rightTopFillet, leftTopRectPoint.Y),
				new GRealPoint2D(leftTopRectPoint.X + leftTopFillet, leftTopRectPoint.Y)};
			GRealPoint2D? intersectionPoint;
			List<IntersectionInfo> tempPoints = new List<IntersectionInfo>();
			for (int i = 0; i < points.Length - 1; i += 2) {
				intersectionPoint = CalcLinesIntersection(startPoint, endPoint, points[i], points[i + 1], true);
				if (intersectionPoint != null)
					tempPoints.Add(new IntersectionInfo(intersectionPoint.Value, SegmentKindHelper.GetKindForLineSegment(i / 2)));
			}
			points = new GRealPoint2D[] {
				new GRealPoint2D(rightBottomRectPoint.X - rightBottomFillet * 2, rightBottomRectPoint.Y - rightBottomFillet * 2),
				new GRealPoint2D(rightBottomRectPoint.X, rightBottomRectPoint.Y),
				new GRealPoint2D(leftTopRectPoint.X, rightBottomRectPoint.Y - leftBottomFillet * 2),
				new GRealPoint2D(leftTopRectPoint.X + leftBottomFillet * 2, rightBottomRectPoint.Y),
				new GRealPoint2D(leftTopRectPoint.X, leftTopRectPoint.Y),
				new GRealPoint2D(leftTopRectPoint.X + leftTopFillet * 2, leftTopRectPoint.Y + leftTopFillet * 2),
				new GRealPoint2D(rightBottomRectPoint.X - rightTopFillet * 2, leftTopRectPoint.Y),
				new GRealPoint2D(rightBottomRectPoint.X, leftTopRectPoint.Y + rightTopFillet * 2)
			};
			List<GRealPoint2D> arcPoints;
			for (int i = 0; i < points.Length; i += 2) {
				arcPoints = GeometricUtils.CalcLineSegmentWithArcIntersection((GRealPoint2D)startPoint, (GRealPoint2D)endPoint,
					points[i], points[i+1], i * Math.PI / 4, Math.PI / 2);
				foreach (GRealPoint2D point in arcPoints)
					tempPoints.Add(new IntersectionInfo(point, SegmentKindHelper.GetKindForArcSegment(i / 2)));
			}
			double minDistance = Double.MaxValue;
			IntersectionInfo result = new IntersectionInfo();
			foreach (IntersectionInfo intersection in tempPoints) {
				double distance = CalcDistance(startPoint, intersection.IntersectionPoint);
				if (!result.HasIntersection || distance < minDistance) {
					minDistance = distance;
					result = intersection;
				}
			}
			return result;	
		}
		public static IntersectionInfo CalcLineSegmentWithEllipseIntersection(GRealPoint2D segmentPoint1, GRealPoint2D segmentPoint2, GRealPoint2D leftTopEllipseRect, GRealPoint2D rightBottomEllipseRect) {
			List<GRealPoint2D> points = CalcLineWithEllipseIntersection(segmentPoint1, segmentPoint2, leftTopEllipseRect, rightBottomEllipseRect);
			return points.Count > 0 ? new IntersectionInfo(points[0], SegmentKind.None) : new IntersectionInfo();			
		}
		public static List<GRealPoint2D> CalcRectWithEllipseIntersection(GRealRect2D rect, GRealEllipse ellipse) {
			List<GRealPoint2D> result = new List<GRealPoint2D>();
			GRealPoint2D p1 = new GRealPoint2D(ellipse.Center.X - ellipse.RadiusX, ellipse.Center.Y - ellipse.RadiusY);
			GRealPoint2D p2 = new GRealPoint2D(ellipse.Center.X + ellipse.RadiusX, ellipse.Center.Y + ellipse.RadiusY);
			result.AddRange(CalcLineWithEllipseIntersection(new GRealPoint2D(rect.Left, rect.Top), new GRealPoint2D(rect.Right, rect.Top), p1, p2));
			result.AddRange(CalcLineWithEllipseIntersection(new GRealPoint2D(rect.Right, rect.Top), new GRealPoint2D(rect.Right, rect.Bottom), p1, p2));
			result.AddRange(CalcLineWithEllipseIntersection(new GRealPoint2D(rect.Right, rect.Bottom), new GRealPoint2D(rect.Left, rect.Bottom), p1, p2));
			result.AddRange(CalcLineWithEllipseIntersection(new GRealPoint2D(rect.Left, rect.Bottom), new GRealPoint2D(rect.Left, rect.Top), p1, p2));
			return result;
		}
		public static double NormalizeRadian(double angleRadian) {
			int count = (int)(0.5 * angleRadian / Math.PI);
			if (angleRadian >= 0)
				return angleRadian - count * Math.PI * 2;
			else
				return Math.PI * 2 + angleRadian - count * Math.PI * 2;
		}
		public static double ScalarProduct(GRealVector2D v1, GRealVector2D v2) {
			return v1.X * v2.X + v1.Y * v2.Y;
		}
		public static GPoint2D StrongRound(GRealPoint2D point) {
			return new GPoint2D(StrongRound(point.X), StrongRound(point.Y));
		}	   
		public static GRect2D StrongRound(GRealRect2D rect) {
			GPoint2D p1 = StrongRound(new GRealPoint2D(rect.Left, rect.Top));
			GPoint2D p2 = StrongRound(new GRealPoint2D(rect.Right, rect.Bottom));
			return new GRect2D(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Abs(p2.X - p1.X), Math.Abs(p2.Y - p1.Y));
		}
		public static GRealPoint2D CalcMean(ICollection<GRealPoint2D> points) {
			double sumX = 0, sumY = 0;
			foreach (GRealPoint2D point in points) {
				sumX += point.X;
				sumY += point.Y;
			}
			return new GRealPoint2D(sumX / points.Count, sumY / points.Count);
		}
		public static bool IsValidDouble(double value) {
			return !double.IsNaN(value) && !double.IsInfinity(value);
		}
	}
}
