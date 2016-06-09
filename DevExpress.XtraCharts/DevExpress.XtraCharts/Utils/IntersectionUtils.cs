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
using System.Collections;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public enum Intersection {
		Yes,
		No,
		Match
	}
	public enum PointIntervalPositon {
		Inside,
		Outside,
		FirstPoint,
		LastPoint
	}
	public class LineEquation {
		double k;
		double b;
		public double K { get { return k; } }
		public double B { get { return b; } }
		public LineEquation(double k, double b) {
			this.k = k;
			this.b = b;
		}
		public double GetValue(double argument) {
			return argument * k + b;
		}
		public double GetArgument(double value) {
			return (value - b) / k;
		}
	}
	public class PlaneEquation {
		double[] eq = new double[4];
		public double A { get { return eq[0]; } }
		public double B { get { return eq[1]; } }
		public double C { get { return eq[2]; } }
		public double D { get { return eq[3]; } }
		public double[] Equation { get { return eq; } }
		public PlaneEquation(double a, double b, double c, double d) {
			eq[0] = a;
			eq[1] = b;
			eq[2] = c;
			eq[3] = d;
		}
	}
	public static class IntersectionUtils {
		#region inner classes
		class VertexItem {
			Vertex vertex;
			double angle, length;
			public Vertex Vertex { get { return vertex; } }
			public double Angle { get { return angle; } }
			public double Length { get { return length; } }
			public VertexItem(Vertex vertex, double angle, double length) {
				this.vertex = vertex;
				this.angle = angle;
				this.length = length;
			}
		}
		class VertexItemComparer : IComparer {
			public VertexItemComparer() {
			}
			#region IComparer implementation
			int IComparer.Compare(object obj1, object obj2) {
				VertexItem item1 = (VertexItem)obj1;
				VertexItem item2 = (VertexItem)obj2;
				if (item1.Angle < item2.Angle)
					return -1;
				if (item1.Angle > item2.Angle)
					return 1;
				double diff = item1.Angle <= 0 ? (item1.Length - item2.Length) : (item2.Length - item1.Length);
				if (diff < 0)
					return -1;
				if (diff > 0)
					return 1;
				return 0;
			}
			#endregion
		}
		#endregion
		static bool ContainsVertex(ArrayList vertices, Vertex vertex, double epsilon) {
			foreach (Vertex item in vertices)
				if (MathUtils.ArePointsEquals(item, vertex, epsilon))
					return true;
			return false;
		}
		static void LocateVertexByPlane(PlaneEquation plane, Vertex vertex, List<Vertex> frontVertices, List<Vertex> backVertices, double epsilon) {
			int k = MathUtils.IsPointOnPlane(plane, vertex, epsilon);
			if (k == 0) {
				frontVertices.Add(vertex);
				backVertices.Add(vertex);
			}
			else if (k > 0)
				frontVertices.Add(vertex);
			else
				backVertices.Add(vertex);
		}
		static void AddIntersectionVertex(List<Vertex> intersectionVertices, Vertex vertex, double epsilon) {
			if (intersectionVertices.Count == 0 ||
				(!MathUtils.ArePointsEquals(intersectionVertices[0], vertex, epsilon) &&
				!MathUtils.ArePointsEquals(intersectionVertices[intersectionVertices.Count - 1], vertex, epsilon)))
				intersectionVertices.Add(vertex);
		}
		static PlanePolygon ConstructConvexPolygon(Vertex[] vertices, double epsilon) {
			ArrayList list = new ArrayList();
			foreach (Vertex vertex in vertices) {
				if (!ContainsVertex(list, vertex, epsilon))
					list.Add(vertex);
			}
			if (list.Count < 3)
				return null;
			ArrayList items = new ArrayList();
			Vertex v0 = (Vertex)list[0];
			Vertex v1 = (Vertex)list[1];
			DiagramVector vector0 = v1.Point - v0.Point;
			items.Add(new VertexItem(v1, 0, vector0.Length));
			DiagramVector n0 = DiagramVector.Zero;
			for (int i = 2; i < list.Count; i++) {
				Vertex v = (Vertex)list[i];
				DiagramVector vector = v.Point - v0.Point;
				double angle = MathUtils.CalcAngle(vector0, vector);
				if (ComparingUtils.CompareDoubles(angle, 0, epsilon) == 0 || ComparingUtils.CompareDoubles(angle, Math.PI, epsilon) == 0)
					items.Add(new VertexItem(v, angle, vector.Length));
				else {
					DiagramVector n = MathUtils.CalcNormal(vector0, vector);
					if (n0.IsZero) {
						n0 = n;
						items.Add(new VertexItem(v, angle, vector.Length));
					}
					else {
						double a = MathUtils.CalcAngle(n0, n);
						int sign = ComparingUtils.CompareDoubles(a, 0, epsilon) == 0 ? 1 : -1;
						items.Add(new VertexItem(v, sign * angle, vector.Length));
					}
				}
			}
			items.Sort(new VertexItemComparer());
			Vertex[] result = new Vertex[items.Count + 1];
			result[0] = v0;
			for (int i = 0, k = 1; i < items.Count; i++, k++)
				result[k] = ((VertexItem)items[i]).Vertex;
			return new PlanePolygon(result);
		}
		static PlanePolygon ConstructIntersectionPolygon(PlaneEquation plane, Vertex[] intersectionVertices, bool front, double epsilon) {
			PlanePolygon polygon = ConstructConvexPolygon(intersectionVertices, epsilon);
			if (polygon != null) {
				DiagramVector normal = front ? DiagramVector.CreateNormalized(-plane.A, -plane.B, -plane.C) : 
					DiagramVector.CreateNormalized(plane.A, plane.B, plane.C);
				polygon.SameNormals = true;
				DiagramVector actualNormal = MathUtils.CalcNormal(polygon);
				double angle = MathUtils.CalcAngle(actualNormal, normal);
				if (ComparingUtils.CompareDoubles(angle, 0, epsilon) != 0)
					polygon.InvertVerticesDirection();
				polygon.Normal = normal;
			}
			return polygon;
		}
		static Intersection CalcLineIntersection3D(PlaneEquation plane, DiagramPoint p1, DiagramPoint p2, double epsilon, out DiagramPoint intersecionPoint) {
			intersecionPoint = DiagramPoint.Zero;
			int r1 = ComparingUtils.CompareDoubles(plane.A * p1.X + plane.B * p1.Y + plane.C * p1.Z + plane.D, 0, epsilon);
			int r2 = ComparingUtils.CompareDoubles(plane.A * p2.X + plane.B * p2.Y + plane.C * p2.Z + plane.D, 0, epsilon);
			if (r1 == 0 && r2 == 0)
				return Intersection.Match;
			DiagramVector v = p2 - p1;;
			double denominator = plane.A * v.DX + plane.B * v.DY + plane.C * v.DZ;
			if (Math.Abs(denominator) >= epsilon) {
				double k = -(plane.D + plane.A * p1.X + plane.B * p1.Y + plane.C * p1.Z) / denominator;
				if (k >= 0 && k <= 1) {
					intersecionPoint = new DiagramPoint(p1.X + k * v.DX, p1.Y + k * v.DY, p1.Z + k * v.DZ);
					return Intersection.Yes;
				}
			}
			return Intersection.No;
		}
		public static bool IsPointOnInterval2D(DiagramPoint p1, DiagramPoint p2, DiagramPoint p, double epsilon, out PointIntervalPositon position) {
			if (MathUtils.ArePointsEquals2D(p, p1, epsilon))
				position = PointIntervalPositon.FirstPoint;
			else if (MathUtils.ArePointsEquals2D(p, p2, epsilon))
				position = PointIntervalPositon.LastPoint;
			else
				position = MathUtils.CalcCos(p1 - p, p2 - p) > 0 ? PointIntervalPositon.Outside : PointIntervalPositon.Inside;
			return position != PointIntervalPositon.Outside;
		}
		public static Intersection CalcLinesIntersection2D(DiagramPoint p1, DiagramPoint p2, DiagramPoint p3, DiagramPoint p4, double epsilon, out List<DiagramPoint> points) {
			points = new List<DiagramPoint>();
			DiagramPoint point;
			Intersection intersection = CalcLinesIntersection2D(p1, p2, p3, p4, true, epsilon, out point);
			switch (intersection) {
				case Intersection.No:
					break;
				case Intersection.Yes:
					points.Add(point);
					break;
				case Intersection.Match:
					PointIntervalPositon position;
					if (IsPointOnInterval2D(p1, p2, p3, epsilon, out position))
						points.Add(p3);
					if (IsPointOnInterval2D(p1, p2, p4, epsilon, out position))
						points.Add(p4);
					if (points.Count < 2) {
						if (IsPointOnInterval2D(p3, p4, p1, epsilon, out position))
							points.Add(p1);
						if (points.Count < 2) {
							if (IsPointOnInterval2D(p3, p4, p2, epsilon, out position))
								points.Add(p2);
						}
					}
					if (points.Count == 2) {
						if (MathUtils.ArePointsEquals2D(points[0], points[1], epsilon))
							points.Remove(points[1]);
					}
					break;
			}
			return intersection;
		}
		public static Intersection CalcLinesIntersection2D(DiagramPoint p1, DiagramPoint p2, DiagramPoint p3, DiagramPoint p4, bool intervalMode, double epsilon, out DiagramPoint intersectionPoint) {
#if DEBUGTEST
			if (MathUtils.ArePointsEquals2D(p1, p2, epsilon) || MathUtils.ArePointsEquals2D(p3, p4, epsilon))
				throw new ArgumentException();
#endif
			intersectionPoint = DiagramPoint.Zero;
			double denominator = (p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y);
			double numerator1 = (p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X);
			double numerator2 = (p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X);
			bool denominatorZeroEquality = ComparingUtils.CompareDoubles(denominator, 0, epsilon) == 0;
			bool numerator1ZeroEquality = ComparingUtils.CompareDoubles(numerator1, 0, epsilon) == 0;
			bool numerator2ZeroEquality = ComparingUtils.CompareDoubles(numerator2, 0, epsilon) == 0;
			if (denominatorZeroEquality && (numerator1ZeroEquality || numerator2ZeroEquality))
				return Intersection.Match;
			if (denominatorZeroEquality)
				return Intersection.No;
			double factor1 = numerator1 / denominator;
			double x = p1.X + factor1 * (p2.X - p1.X);
			double y = p1.Y + factor1 * (p2.Y - p1.Y);
			intersectionPoint = new DiagramPoint(x, y, p1.Z);
			if (intervalMode) {
				double factor2 = numerator2 / denominator;
				return factor1 >= 0 && factor1 <= 1 && factor2 >= 0 && factor2 <= 1 ? Intersection.Yes : Intersection.No;
			}
			return Intersection.Yes;
		}	   
		public static Intersection CalcLineIntersection3D(PlaneEquation plane, Line line, double epsilon, out Vertex intersectionVertex, out Line frontLine, out Line backLine) {
			intersectionVertex = new Vertex();
			frontLine = null;
			backLine = null;
			DiagramPoint intersectionPoint;
			Intersection intersection = CalcLineIntersection3D(plane, line.V1, line.V2, epsilon, out intersectionPoint);
			switch (intersection) {
				case Intersection.Yes:
					DiagramVector normal = MathUtils.InterpolateNormal(line.V1.Normal, line.V2.Normal, MathUtils.CalcRelativePosition(line.V1, line.V2, intersectionPoint));
					Color color = MathUtils.InterpolateColor(line.V1.Color, line.V2.Color, MathUtils.CalcRelativePosition(line.V1, line.V2, intersectionPoint));
					intersectionVertex = new Vertex(intersectionPoint, normal, color);
					int k = MathUtils.IsPointOnPlane(plane, line.V1, epsilon);
					if(k > 0)
						frontLine = Line.CreateFromLine(new Vertex[] {line.V1, intersectionVertex}, line);
					else if(k<0)
						backLine = Line.CreateFromLine(new Vertex[] { line.V1, intersectionVertex }, line);
					k = MathUtils.IsPointOnPlane(plane, line.V2, epsilon);
					if (k > 0)
						frontLine = Line.CreateFromLine(new Vertex[] { intersectionVertex, line.V2 }, line);
					else if (k < 0)
						backLine = Line.CreateFromLine(new Vertex[] { intersectionVertex, line.V2 }, line);
					return Intersection.Yes;
				case Intersection.No:
					if (MathUtils.IsPointOnPlane(plane, line.V1, epsilon) > 0)
						frontLine = line;
					else 
						backLine = line;
					return Intersection.No;
				case Intersection.Match:
					return Intersection.Match;
				default:
					throw new DefaultSwitchException();
			}
		}
		public static Intersection CalcPolygonIntersection3D(PlaneEquation plane, PlanePolygon polygon, double epsilon, out Vertex[] intersectionVertices, out PlanePolygon frontPolygon, out PlanePolygon backPolygon) {
			List<Vertex> frontVertices = new List<Vertex>();
			List<Vertex> backVertices = new List<Vertex>();
			List<Vertex> vertices = new List<Vertex>();
			for (int i = 0; i < polygon.Vertices.Length; i++) {
				Vertex v1 = polygon.Vertices[i];
				Vertex v2 = polygon.Vertices[i == polygon.Vertices.Length - 1 ? 0 : i + 1];
				LocateVertexByPlane(plane, v1, frontVertices, backVertices, epsilon);
				Line line = new Line(v1, v2, polygon.SameNormals, polygon.SameColors, polygon.Normal, polygon.Color);
				Vertex lineIntersectionVertex;
				Line frontLine, backLine;
				Intersection intersection = CalcLineIntersection3D(plane, line, epsilon, out lineIntersectionVertex, out frontLine, out backLine);
				switch (intersection) {
					case Intersection.Yes:
						if (frontLine != null && backLine != null)
							LocateVertexByPlane(plane, lineIntersectionVertex, frontVertices, backVertices, epsilon);
						AddIntersectionVertex(vertices, lineIntersectionVertex, epsilon);
						break;
					case Intersection.No:
						break;
					case Intersection.Match:
						AddIntersectionVertex(vertices, v1, epsilon);
						break;
					default:
						throw new DefaultSwitchException();
				}
			}
			frontPolygon = polygon.CreateInstance(frontVertices.ToArray());
			backPolygon = polygon.CreateInstance(backVertices.ToArray());
			intersectionVertices = vertices.Count > 0 ? vertices.ToArray() : null;
			if (frontPolygon != null && backPolygon != null && frontPolygon.Equals(backPolygon)) {
				frontPolygon = null;
				backPolygon = null;
				return Intersection.Match;
			}
			else if (intersectionVertices == null || intersectionVertices.Length == 0)
				return Intersection.No;
			else
				return Intersection.Yes;
		}
		public static void CalcPolyhedronIntersection(PlaneEquation plane, PlanePolygon[] polyhedron, double epsilon, out List<PlanePolygon> frontPolyhedron, out List<PlanePolygon> backPolyhedron, out PlanePolygon frontIntersection, out PlanePolygon backIntersection) {
			frontIntersection = null;
			backIntersection = null;
			frontPolyhedron = new List<PlanePolygon>();
			backPolyhedron= new List<PlanePolygon>();
			List<Vertex> frontVertices = new List<Vertex>();
			List<Vertex> backVertices = new List<Vertex>();
			PlanePolygon matchPolygon = null;
			foreach (PlanePolygon polygon in polyhedron) {
				PlanePolygon frontPolygon, backPolygon;
				Vertex[] intersectionVertices;
				Intersection intersection = CalcPolygonIntersection3D(plane, polygon, epsilon, out intersectionVertices, out frontPolygon, out backPolygon);
				switch (intersection) {
					case Intersection.Yes:
						if (frontPolygon != null) {
							frontPolyhedron.Add(frontPolygon);
							frontVertices.AddRange(intersectionVertices);
						}
						if (backPolygon != null) {
							backPolyhedron.Add(backPolygon);
							backVertices.AddRange(intersectionVertices);
						}
						break;
					case Intersection.No:
						if (frontPolygon != null)
							frontPolyhedron.Add(frontPolygon);
						if (backPolygon != null)
							backPolyhedron.Add(backPolygon);
						break;
					case Intersection.Match:
						matchPolygon = polygon;
						break;
					default:
						throw new DefaultSwitchException();
				}
			}
			if (matchPolygon == null) {
				frontIntersection = ConstructIntersectionPolygon(plane, frontVertices.ToArray(), true, epsilon);
				backIntersection = ConstructIntersectionPolygon(plane, backVertices.ToArray(), false, epsilon);
			}
			else {
				if (frontPolyhedron.Count > 1)
					frontIntersection = matchPolygon;
				else
					backIntersection = matchPolygon;
			}
			if (frontPolyhedron.Count < 2)
				frontPolyhedron = null;
			if (backPolyhedron.Count < 2)
				backPolyhedron = null;
		}
		public static bool IsPointInPolygon(DiagramPoint point, IList<DiagramPoint> polygon, double epsilon) {
			int count = 0;
			double firstPosition = 0;
			double prevPosition = 0;
			DiagramPoint point2 = new DiagramPoint(point.X + 10, point.Y);
			for (int i = 0; i < polygon.Count; i++) {
				DiagramPoint p1 = polygon[i];
				DiagramPoint p2 = polygon[i == polygon.Count - 1 ? 0 : i + 1];
				if (MathUtils.ArePointsEquals2D(p1, point, epsilon))
					return true;
				if (MathUtils.ArePointsEquals2D(p1, p2, epsilon)) {
					if ((i == polygon.Count - 1) &&
						((prevPosition < 0 && firstPosition > 0) || (prevPosition > 0 && firstPosition < 0)))
						count++;
					continue;
				}
				DiagramPoint intersectionPoint;
				Intersection intersection = CalcLinesIntersection2D(p1, p2, point, point2, false, epsilon, out intersectionPoint);
				if (intersection == Intersection.Match) {
					if ((point.X > p1.X && point.X < p2.X) || (point.X > p2.X && point.X < p1.X))
						return true;
					if ((i == polygon.Count - 1) &&
						((prevPosition < 0 && firstPosition > 0) || (prevPosition > 0 && firstPosition < 0)))
						count++;
				}
				else if (intersection == Intersection.Yes) {
					PointIntervalPositon intervalPosition;
					if (!IntersectionUtils.IsPointOnInterval2D(p1, p2, intersectionPoint, epsilon, out intervalPosition))
						continue;
					if (MathUtils.ArePointsEquals2D(point, intersectionPoint, epsilon))
						return true;
					if (intersectionPoint.X < point.X)
						continue;
					if (MathUtils.ArePointsEquals2D(intersectionPoint, p1, epsilon)) {
						double position = p2.Y - point.Y;
						if (firstPosition == 0)
							firstPosition = position;
						if ((prevPosition < 0 && position > 0) || (prevPosition > 0 && position < 0)) {
							count++;
							prevPosition = 0;
						}
					}
					else if (MathUtils.ArePointsEquals2D(intersectionPoint, p2, epsilon)) {
						double position = p1.Y - point.Y;
						prevPosition = position;
						if (i == polygon.Count - 1) {
							if ((firstPosition < 0 && position > 0) || (firstPosition > 0 && position < 0))
								count++;
						}
					}
					else {
						if ((p1.Y < point.Y && p2.Y > point.Y) || (p1.Y > point.Y && p2.Y < point.Y)) {
							count++;
							prevPosition = 0;
						}
					}
				}
			}
			return count % 2 != 0;
		}
	}
}
