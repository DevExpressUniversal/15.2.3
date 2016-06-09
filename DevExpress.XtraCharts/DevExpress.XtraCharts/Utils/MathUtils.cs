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
using DevExpress.Utils;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public static class MathUtils {
		public static bool IsValidNumber(double value) {
			return !double.IsNaN(value) && !double.IsInfinity(value);
		}
		public static double CalcRelativePosition(DiagramPoint start, DiagramPoint end, DiagramPoint p) {
			double length = (end - start).Length;
			return length == 0 ? 0.5 : (p - start).Length / length;
		}
		public static DiagramVector CalcNormal(DiagramVector v1, DiagramVector v2) {
			DiagramVector normal = v1 * v2;
			normal.Normalize();
			return normal;
		}
		public static DiagramVector CalcNormal(DiagramPoint p1, DiagramPoint p2, DiagramPoint p3) {
			return CalcNormal(p2 - p1, p3 - p1);
		}
		public static bool IsPointOnLine(DiagramPoint p, DiagramPoint p1, DiagramPoint p2, double epsilon) {
			if (ComparingUtils.CompareDoubles(Math.Abs(p1.X - p2.X), 0, epsilon) == 0) {
				if (ComparingUtils.CompareDoubles(Math.Abs(p.X - p1.X), 0, epsilon) != 0)
					return false;
				if (ComparingUtils.CompareDoubles(Math.Abs(p1.Y - p2.Y), 0, epsilon) == 0)
					return ComparingUtils.CompareDoubles(Math.Abs(p.Y - p1.Y), 0, epsilon) == 0;
				return ComparingUtils.CompareDoubles(Math.Abs((p.Z - p1.Z) * (p2.Y - p1.Y) - (p2.Z - p1.Z) * (p.Y - p1.Y)), 0, epsilon) == 0;
			}
			double t = (p.X - p1.X) / (p2.X - p1.X);
			return ComparingUtils.CompareDoubles(Math.Abs(p.Y - (p1.Y + t * (p2.Y - p1.Y))), 0, epsilon) == 0 &&
				   ComparingUtils.CompareDoubles(Math.Abs(p.Z - (p1.Z + t * (p2.Z - p1.Z))), 0, epsilon) == 0;
		}
		public static bool IsPointInsideEllipse(DiagramPoint p, ZPlaneRectangle ellipseRect) {
			p.X -= ellipseRect.Center.X;
			p.Y -= ellipseRect.Center.Y;
			double a2 = ellipseRect.Width * ellipseRect.Width / 4;
			double b2 = ellipseRect.Height * ellipseRect.Height / 4;
			return p.X * p.X / a2 + p.Y * p.Y / b2 <= 1;
		}
		static bool IsPointOnPlane(DiagramPoint p, DiagramPoint p1, DiagramPoint p2, DiagramPoint p3, double epsilon) {
			double a = (p2.Y - p1.Y) * (p3.Z - p1.Z) - (p3.Y - p1.Y) * (p2.Z - p1.Z);
			double b = (p3.X - p1.X) * (p2.Z - p1.Z) - (p2.X - p1.X) * (p3.Z - p1.Z);
			double c = (p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p2.Y - p1.Y);
			double d = a * p1.X + b * p1.Y + c * p1.Z;
			return ComparingUtils.CompareDoubles(Math.Abs(a * p.X + b * p.Y + c * p.Z - d), 0, epsilon) == 0;
		}
		public static bool IsOnePlanePoints(IList<DiagramPoint> points, double epsilon) {
			if (points.Count < 4)
				return true;
			DiagramPoint p1 = DiagramPoint.Zero, p2 = DiagramPoint.Zero, p3 = DiagramPoint.Zero;
			int count = 0;
			foreach (DiagramPoint point in points)
				switch (count) {
					case 0:
						p1 = point;
						count++;
						break;
					case 1:
						if (!ArePointsEquals(point, p1, epsilon)) {
							p2 = point;
							count++;
						}
						break;
					case 2:
						if (!ArePointsEquals(point, p1, epsilon) && !ArePointsEquals(point, p2, epsilon) && !IsPointOnLine(point, p1, p2, epsilon)) {
							p3 = point;
							count++;
						}
						break;
					default:
						if (!IsPointOnPlane(point, p1, p2, p3, epsilon))
							return false;
						break;
				}
			return true;
		}
		static int InterpolateColorComponent(int comp1, int comp2, double ratio) {
			if (ratio == 0)
				return comp1;
			return (int)(comp1 + (comp2 - comp1) * ratio);
		}
		public static Color InterpolateColor(Color color1, Color color2, double ratio) {
			int r = InterpolateColorComponent(color1.R, color2.R, ratio);
			int g = InterpolateColorComponent(color1.G, color2.G, ratio);
			int b = InterpolateColorComponent(color1.B, color2.B, ratio);
			int a = InterpolateColorComponent(color1.A, color2.A, ratio);
			return Color.FromArgb(a, r, g, b);
		}
		public static double ScalarProduct(DiagramVector v1, DiagramVector v2) {
			return v1.DX * v2.DX + v1.DY * v2.DY + v1.DZ * v2.DZ;
		}
		public static double CalcAxisXAngle2D(DiagramPoint p1, DiagramPoint p2, double epsilon) {
			return Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
		}
		public static int IsPointOnPlane(PlaneEquation plane, DiagramPoint point, double epsilon) {
			double k = plane.A * point.X + plane.B * point.Y + plane.C * point.Z + plane.D;
			return ComparingUtils.CompareDoubles(k, 0, epsilon);
		}
		public static bool IsTriangleOnPlane(PlaneEquation plane, PlaneTriangle triangle, double epsilon) {
			return IsPointOnPlane(plane, triangle.Vertices[0], epsilon) == 0 && IsPointOnPlane(plane, triangle.Vertices[1], epsilon) == 0 &&
				IsPointOnPlane(plane, triangle.Vertices[2], epsilon) == 0;  
		}
		public static bool IsLineOnPlane(PlaneEquation plane, Line line, double epsilon) {
			return IsPointOnPlane(plane, line.V1, epsilon) == 0 && IsPointOnPlane(plane, line.V2, epsilon) == 0;				
		}
		public static double CalcAngle(DiagramVector v1, DiagramVector v2) {
			return Math.Acos(CalcCos(v1, v2));
		}
		public static double CalcCos(DiagramVector v1, DiagramVector v2) {
			double cos = ScalarProduct(v1, v2) / (v1.Length * v2.Length);
			if (cos > 1)
				return 1;
			else if (cos < -1)
				return -1;
			return cos;
		}
		public static double CalcSquaredCos(DiagramVector v1, DiagramVector v2) {
			double product = ScalarProduct(v1, v2);
			double cos = product * product / (v1.SquaredLength * v2.SquaredLength);
			return cos > 1 ? 1 : cos;
		}
		public static DiagramVector InterpolateNormal(DiagramVector n1, DiagramVector n2, double ratio) {
			return DiagramVector.CreateNormalized((n2.DX - n1.DX) * ratio + n1.DX, (n2.DY - n1.DY) * ratio + n1.DY, (n2.DZ - n1.DZ) * ratio + n1.DZ);
		}
		public static double CalcLength2D(GRealPoint2D p1, GRealPoint2D p2) {
			return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
		}
		public static float CalcLength2D(PointF p1, PointF p2) {
			return (float)Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
		}
		public static double CalcLength2D(DiagramPoint p1, DiagramPoint p2) {
			return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
		}
		public static double CalcLength(DiagramPoint p1, DiagramPoint p2) {
			return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2) + Math.Pow((p2.Z - p1.Z), 2));
		}
		public static bool ArePointsEquals(DiagramPoint p1, DiagramPoint p2, double epsilon) {
			return ComparingUtils.CompareDoubles(p1.X, p2.X, epsilon) == 0 &&
				   ComparingUtils.CompareDoubles(p1.Y, p2.Y, epsilon) == 0 &&
				   ComparingUtils.CompareDoubles(p1.Z, p2.Z, epsilon) == 0;
		}
		public static bool ArePointsEquals(DiagramPoint[] points, double epsilon) {
			for (int i = 1; i < points.Length; i++)
				if (!ArePointsEquals(points[0], points[i], epsilon))
					return false;
			return true;
		}
		public static bool ArePointsEquals2D(DiagramPoint p1, DiagramPoint p2, double epsilon) {
			double length = Math.Abs(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
			return ComparingUtils.CompareDoubles(length, 0, epsilon * epsilon) == 0;
		}
		public static bool AreVectorsEquals(DiagramVector v1, DiagramVector v2, double epsilon) {
			return ComparingUtils.CompareDoubles(v1.DX, v2.DX, epsilon) == 0 &&
				   ComparingUtils.CompareDoubles(v1.DY, v2.DY, epsilon) == 0 &&
				   ComparingUtils.CompareDoubles(v1.DZ, v2.DZ, epsilon) == 0;
		}
		public static double NormalizeDegree(double angleDegree) {
			return Radian2Degree(GeometricUtils.NormalizeRadian(Degree2Radian(angleDegree)));
		}
		public static double Degree2Radian(double angleDegree) {
			return angleDegree * Math.PI / 180.0;
		}
		public static double Radian2Degree(double angleRadian) {
			return angleRadian * 180 / Math.PI;
		}
		public static Matrix CalcTranslateMatrix(DiagramPoint p1, DiagramPoint p2, double epsilon) {
			Matrix matrix = new Matrix();
			matrix.Reset();
			double angle = CalcAxisXAngle2D(p1, p2, epsilon);
			matrix.Rotate((float)Radian2Degree(angle), MatrixOrder.Append);
			matrix.Translate((float)p1.X, (float)p1.Y, MatrixOrder.Append);
			return matrix;
		}
		public static bool IsOnePlanePrimitives(PlanePrimitive primitive1, PlanePrimitive primitive2) {
			Vertex[] vertices1 = primitive1.Vertices;
			Vertex[] vertices2 = primitive2.Vertices;
			if (vertices1.Length > 2 && vertices2.Length == 2) {
				DiagramVector n = CalcNormal(vertices1[0], vertices1[1], vertices1[2]);
				return ComparingUtils.CompareDoubles(ScalarProduct(vertices2[0].Point - vertices1[0].Point, n), 0, Diagram3D.Epsilon) == 0 &&
					   ComparingUtils.CompareDoubles(ScalarProduct(vertices2[1].Point - vertices1[1].Point, n), 0, Diagram3D.Epsilon) == 0;
			}
			else if (vertices1.Length > 2 && vertices2.Length > 2) {
				DiagramVector n = CalcNormal(vertices1[0], vertices1[1], vertices1[2]);
				return ComparingUtils.CompareDoubles(ScalarProduct(vertices2[0].Point - vertices1[0].Point, n), 0, Diagram3D.Epsilon) == 0 &&
					   ComparingUtils.CompareDoubles(ScalarProduct(vertices2[1].Point - vertices1[1].Point, n), 0, Diagram3D.Epsilon) == 0 &&
					   ComparingUtils.CompareDoubles(ScalarProduct(vertices2[2].Point - vertices1[2].Point, n), 0, Diagram3D.Epsilon) == 0;
			}
			else
				throw new ArgumentException();
		}
		public static double CalcAngleBetweenLines2D(DiagramPoint p1, DiagramPoint p2, DiagramPoint p3, DiagramPoint p4, double epsilon) {
			double angle1 = CalcAxisXAngle2D(p1, p2, epsilon);
			double angle2 = CalcAxisXAngle2D(p3, p4, epsilon);
			return angle2 - angle1;
		}
		public static DiagramPoint CalcIntervalCenter2D(DiagramPoint p1, DiagramPoint p2, double epsilon) {
			if (ArePointsEquals2D(p1, p2, epsilon))
				return p1;
			double x1 = Math.Min(p1.X, p2.X);
			double x2 = Math.Max(p1.X, p2.X);
			double y1 = Math.Min(p1.Y, p2.Y);
			double y2 = Math.Max(p1.Y, p2.Y);
			double x = x1 + (x2 - x1) / 2.0;
			double y = y1 + (y2 - y1) / 2.0;
			return new DiagramPoint(x, y, p1.Z);
		}
		public static DiagramPoint CalcIntervalCenter3D(DiagramPoint p1, DiagramPoint p2, double epsilon) {
			if (ArePointsEquals2D(p1, p2, epsilon))
				return p1;
			double x1 = Math.Min(p1.X, p2.X);
			double x2 = Math.Max(p1.X, p2.X);
			double y1 = Math.Min(p1.Y, p2.Y);
			double y2 = Math.Max(p1.Y, p2.Y);
			double z1 = Math.Min(p1.Z, p2.Z);
			double z2 = Math.Max(p1.Z, p2.Z);
			double x = x1 + (x2 - x1) / 2.0;
			double y = y1 + (y2 - y1) / 2.0;
			double z = z1 + (z2 - z1) / 2.0;
			return new DiagramPoint(x, y, z);
		}
		public static Vertex CalcIntervalCenter3D(Vertex v1, Vertex v2, double epsilon) {
			DiagramPoint point = CalcIntervalCenter3D((DiagramPoint)v1, (DiagramPoint)v2, epsilon);
			DiagramVector normal = InterpolateNormal(v1.Normal, v2.Normal, 0.5);
			Color color = InterpolateColor(v1.Color, v2.Color, 0.5);
			return new Vertex(point, normal, color);
		}
		public static PlaneEquation CalcPlaneEquation(PlanePolygon polygon) {
			DiagramVector n = CalcNormal(polygon.Vertices[0], polygon.Vertices[1], polygon.Vertices[2]);
			double d = -(n.DX * polygon.Vertices[0].X + n.DY * polygon.Vertices[0].Y + n.DZ * polygon.Vertices[0].Z);
			return new PlaneEquation(n.DX, n.DY, n.DZ, d);
		}
		public static double CalcSquaredLength(DiagramPoint p1, DiagramPoint p2) {
			return (p2 - p1).SquaredLength;
		}
		public static DiagramVector CalcNormal(PlanePolygon polygon) {
#if DEBUGTEST
			if (polygon == null)
				throw new ArgumentNullException("polygon");
#endif
			Vertex[] vertices = polygon.Vertices;
			if (vertices == null)
				return DiagramVector.Zero;
			for (int i = 2; i < vertices.Length; i++)
				if (!MathUtils.IsPointOnLine(vertices[i], vertices[0], vertices[1], Diagram3D.Epsilon))
					return CalcNormal(vertices[i], vertices[0], vertices[1]);
			return DiagramVector.Zero;
		}
		public static DiagramVector CalcNormal(IList<DiagramPoint> points) {
			if (points == null || points.Count == 0)
				return DiagramVector.Zero;
			List<DiagramPoint> result = new List<DiagramPoint>();
			foreach (DiagramPoint point in points) {
				bool good = true;
				foreach (DiagramPoint resultPoint in result) {
					if (ArePointsEquals(resultPoint, point, Diagram3D.Epsilon)) {
						good = false;
						break;
					}
				}
				if (result.Count == 2)
					good = !IsPointOnLine(point, result[0], result[1], Diagram3D.Epsilon);
				if (good)
					result.Add(point);
				if (result.Count == 3)
					break;
			}
			if (result.Count < 3)
				return DiagramVector.Zero;
			return CalcNormal(result[0], result[1], result[2]);
		}
		public static GRealPoint2D GetPointInDirection2D(GRealPoint2D sourcePoint, DiagramVector direction, double length) {
			direction.DZ = 0.0;
			direction.Normalize();
			return new GRealPoint2D(sourcePoint.X + length * direction.DX, sourcePoint.Y + length * direction.DY);
		}
		public static DiagramPoint StrongRoundXY(DiagramPoint point) {
			return new DiagramPoint(StrongRound(point.X), StrongRound(point.Y));
		}
		public static int StrongRound(double value) {
			return Math.Sign(value) * (int)(Math.Abs(value) + 0.5);
		}
		public static Rectangle StrongRound(RectangleF rect) {
			return new Rectangle(StrongRound(rect.X), StrongRound(rect.Y), StrongRound(rect.Width), StrongRound(rect.Height));
		}
		public static Size StrongRound(SizeF size) {
			return new Size(StrongRound(size.Width), StrongRound(size.Height));
		}
		public static GRect2D MakeGRect2D(RectangleF rect) {
			return new GRect2D(StrongRound(rect.X), StrongRound(rect.Y), StrongRound(rect.Width), StrongRound(rect.Height));
		}
		public static List<DiagramPoint> MakeRectangle(DiagramPoint p1, DiagramPoint p2, bool doublePoints, double width) {
			GRealPoint2D v1 = GetPointInDirection2D(new GRealPoint2D(0, 0), MathUtils.CalcNormal(p2 - p1, new DiagramVector(0, 0, 1)), width / 2.0);
			Matrix matrix1 = new Matrix();
			matrix1.Translate((float)v1.X, (float)v1.Y);
			Matrix matrix2 = new Matrix();
			matrix2.Translate((float)-v1.X, (float)-v1.Y);
			PointF[] points1 = new PointF[] { (PointF)p1, (PointF)p2 };
			PointF[] points2 = new PointF[] { (PointF)p1, (PointF)p2 };
			matrix1.TransformPoints(points1);
			matrix2.TransformPoints(points2);
			List<DiagramPoint> contour = new List<DiagramPoint>();
			contour.Add(new DiagramPoint(points1[0].X, points1[0].Y, p1.Z));
			if (doublePoints)
				contour.Add(contour[contour.Count - 1]);
			contour.Add(new DiagramPoint(points1[1].X, points1[1].Y, p1.Z));
			if (doublePoints)
				contour.Add(contour[contour.Count - 1]);
			contour.Add(new DiagramPoint(points2[1].X, points2[1].Y, p1.Z));
			if (doublePoints)
				contour.Add(contour[contour.Count - 1]);
			contour.Add(new DiagramPoint(points2[0].X, points2[0].Y, p1.Z));
			if (doublePoints)
				contour.Add(contour[contour.Count - 1]);
			return contour;
		}
		public static ZPlaneRectangle MakeRectangle(DiagramPoint center, SizeF size) {
			DiagramPoint location = DiagramPoint.Offset(center, -size.Width / 2.0, -size.Height / 2.0, 0);
			return new ZPlaneRectangle(location, size.Width, size.Height);
		}
		public static RectangleF MakeRectangle(PointF corner1, PointF corner2) {
			return MakeRectangle(corner1, corner2, 0);   
		}
		public static RectangleF MakeRectangle(PointF corner1, PointF corner2, float extend) {
			float x = corner1.X < corner2.X ? corner1.X : corner2.X;
			float y = corner1.Y < corner2.Y ? corner1.Y : corner2.Y;
			float width = Math.Abs(corner1.X - corner2.X) + extend;
			float height = Math.Abs(corner1.Y - corner2.Y) + extend;
			return new RectangleF(x, y, width, height);
		}
		public static bool IsAngleNormal(double angle) {
			double x = Math.Abs(angle) / (Math.PI / 2);
			return ComparingUtils.CompareDoubles(x, Math.Round(x), 1e-5) == 0;
		}
		public static DiagramVector Mult(DiagramVector v, double[] matrix) {
			return new DiagramVector(v.DX * matrix[0] + v.DY * matrix[4] + v.DZ * matrix[8], 
									 v.DX * matrix[1] + v.DY * matrix[5] + v.DZ * matrix[9], 
									 v.DX * matrix[2] + v.DY * matrix[6] + v.DZ * matrix[10]);
		}
		public static DiagramPoint CalcMinDistancePoint2D(DiagramPoint point, DiagramPoint p1, DiagramPoint p2, double epsilon, out double squaredDistance) {
			squaredDistance = 0;
			if (ArePointsEquals2D(p1, p2, epsilon)) {
				squaredDistance = CalcSquaredLength(point, p1);
				return p1;
			}
			DiagramPoint result;
			DiagramVector v = p2 - p1;
			DiagramPoint point2 = new DiagramPoint(point.X + v.DY, point.Y - v.DX);
			DiagramPoint intersectionPoint;
			IntersectionUtils.CalcLinesIntersection2D(point, point2, p1, p2, false, epsilon, out intersectionPoint);
			PointIntervalPositon position;
			if (IntersectionUtils.IsPointOnInterval2D(p1, p2, intersectionPoint, epsilon, out position)) {
				result = intersectionPoint;
				squaredDistance = CalcSquaredLength(point, intersectionPoint);
			}
			else {
				double d1 = CalcSquaredLength(point, p1);
				double d2 = CalcSquaredLength(point, p2);
				if (d1 <= d2) {
					result = p1;
					squaredDistance = d1;
				}
				else {
					result = p2;
					squaredDistance = d2;
				}
			}
			return result;
		}
		public static DiagramPoint[] CalcInscribedEllipsePoints(YPlaneRectangle plane, double startAngle, int pointsCount) {
			double a = plane.Height / 2;
			double b = plane.Width / 2;
			DiagramPoint[] points = new DiagramPoint[pointsCount];
			double angleStep = 2 * Math.PI / pointsCount;
			for (int i = 0; i < pointsCount; i++) {
				double angle = i * angleStep + startAngle;
				double x = a * Math.Cos(angle);
				double z = b * Math.Sin(angle);
				points[i] = new DiagramPoint(x + plane.Center.X, plane.Center.Y, z + plane.Center.Z);
			}
			return points;
		}
		public static bool CalcEllipseTangentLines(ZPlaneRectangle ellipseRect, DiagramPoint point, out double angle1, out double angle2) {
			angle1 = angle2 = 0;
			if (IsPointInsideEllipse(point, ellipseRect))
				return false;
			double x = point.X - ellipseRect.Center.X;
			double y = point.Y - ellipseRect.Center.Y;
			double xSqr = x * x;
			double ySqr = y * y;
			double aSqr = ellipseRect.Width * ellipseRect.Width / 4;
			double bSqr = ellipseRect.Height * ellipseRect.Height / 4;
			double a, b, c;
			if (xSqr < ySqr) {
				a = ySqr * aSqr + bSqr * xSqr;
				b = -2 * aSqr * bSqr * x;
				c = bSqr * aSqr * aSqr - ySqr * aSqr * aSqr;
			}
			else {
				a = xSqr * bSqr + ySqr * aSqr;
				b = -2 * aSqr * bSqr * y;
				c = bSqr * bSqr * aSqr - xSqr * bSqr * bSqr;
			}
			List<double> roots = GeometricUtils.CalcQuadraticEquation(a, b, c);
			if (roots.Count < 2)
				return false;
			double _x1, _y1, _x2, _y2;
			if (xSqr < ySqr) {
				_x1 = roots[0];
				_y1 = bSqr * (aSqr - roots[0] * x) / (y * aSqr);
				_x2 = roots[1];
				_y2 = bSqr * (aSqr - roots[1] * x) / (y * aSqr);
			}
			else {
				_y1 = roots[0];
				_x1 = aSqr * (bSqr - roots[0] * y) / (x * bSqr);
				_y2 = roots[1];
				_x2 = aSqr * (bSqr - roots[1] * y) / (x * bSqr);
			}
			angle1 = GeometricUtils.NormalizeRadian(Math.Atan2(_y1 - y, _x1 - x));
			angle2 = GeometricUtils.NormalizeRadian(Math.Atan2(_y2 - y, _x2 - x));
			return true;
		}
		public static ZPlaneRectangle CalcBounds(ZPlaneRectangle rect, int rotationAngleInDegrees) {
			Matrix m = new Matrix();
			m.RotateAt(rotationAngleInDegrees, (PointF)rect.Center);
			double left, bottom, right, top;
			left = bottom = Double.MaxValue;
			right = top = Double.MinValue;
			foreach (Vertex vertex in rect.Vertices) {
				DiagramPoint point = TransformUtils.ApplyTransform(m, vertex.Point);
				if (point.X < left)
					left = point.X;
				if (point.X > right)
					right = point.X;
				if (point.Y < bottom)
					bottom = point.Y;
				if (point.Y > top)
					top = point.Y;
			}
			return new ZPlaneRectangle(new DiagramPoint(left, bottom), new DiagramPoint(right, top));
		}
		public static int Ceiling(double doubleNumber) {
			return (int)Math.Ceiling(doubleNumber);
		}
	}
	public static class TransformUtils {
		public static DiagramPoint ApplyTransform(Matrix matrix, DiagramPoint diagramPoint) {
			PointF[] points = new PointF[] { (PointF)diagramPoint };
			matrix.TransformPoints(points);
			return (DiagramPoint)points[0];
		}
	}
}
