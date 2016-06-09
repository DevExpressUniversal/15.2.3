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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public static class ContourUtils {
		#region inner classes
		class VertexDescriptor {
			DiagramPoint point;
			VertexDescriptor prev, next;
			EdgeDescriptor inEdge, outEdge;
			ContourMark contourMark;
			EdgeMark edgeMark;
			int contourIndex;
			bool passed;
			bool doubled;
			public DiagramPoint Point { get { return point; } }
			public VertexDescriptor Next { get { return next; } set { next = value; } }
			public VertexDescriptor Prev { get { return prev; } set { prev = value; } }
			public EdgeDescriptor InEdge { get { return inEdge; } set { inEdge = value; } }
			public EdgeDescriptor OutEdge { get { return outEdge; } set { outEdge = value; } }
			public ContourMark ContourMark { get { return contourMark; } set { contourMark = value; } }
			public EdgeMark EdgeMark { get { return edgeMark; } set { edgeMark = value; } }
			public int ContourIndex { get { return contourIndex; } }
			public bool Passed { get { return passed; } set { passed = value; } }
			public bool Doubled { get { return doubled; } set { doubled = value; } }
			public VertexDescriptor(DiagramPoint point, int contourIndex) {
				this.point = point;
				this.contourIndex = contourIndex;
			}
			public List<VertexDescriptor> GetCrossVertices() {
				List<VertexDescriptor> vertices = new List<VertexDescriptor>();
				if (inEdge != null) {
					EdgeDescriptor current = inEdge;
					do {
						if (current.IsIn) {
							if (current.VertexDescriptor.ContourIndex != contourIndex)
								vertices.Add(current.VertexDescriptor);
						}
						current = current.Next;
					}
					while (current != inEdge);
				}
				return vertices;
			}
			public IList<DiagramPoint> GetContour(bool forward) {
				List<DiagramPoint> points = new List<DiagramPoint>();
				VertexDescriptor current = this;
				do {
					points.Add(current.point);
					if (current.Doubled)
						points.Add(current.point);
					current = forward ? current.next : current.prev;
				}
				while (current != this);
				return points;
			}
			public VertexDescriptor GetEdgeVertex(bool forward) {
				return forward ? this : prev;
			}
		}
		class EdgeDescriptor : IComparable {
			double angle;
			bool isIn;
			VertexDescriptor vertexDescriptor;
			EdgeDescriptor prev, next;
			public double Angle { get { return angle; } }
			public bool IsIn { get { return isIn; } }
			public VertexDescriptor VertexDescriptor { get { return vertexDescriptor; } }
			public EdgeDescriptor Prev { get { return prev; } set { prev = value; } }
			public EdgeDescriptor Next { get { return next; } set { next = value; } }
			public EdgeDescriptor(VertexDescriptor vertexDescriptor, double angle, bool isIn) {
				this.vertexDescriptor = vertexDescriptor;
				this.angle = angle;
				this.isIn = isIn;
			}
			#region IComparable Members
			int IComparable.CompareTo(object obj) {
				EdgeDescriptor edge = (EdgeDescriptor)obj;
				return angle.CompareTo(edge.angle);
			}
			#endregion
		}
		enum ContourMark {
			Inside,
			Outside,
			Sected
		}
		enum EdgeMark {
			Shared1,
			Shared2,
			Inside,
			Outside
		}
		enum Operation {
			Intersection,
			Union
		}
		#endregion
		static VertexDescriptor AddVertex(VertexDescriptor vertex1, VertexDescriptor vertex2, DiagramPoint point, double epsilon) {
			VertexDescriptor current = vertex1;
			while (current != vertex2) {
				PointIntervalPositon position;
				if (IntersectionUtils.IsPointOnInterval2D(current.Point, current.Next.Point, point, epsilon, out position)) {
					switch (position) {
						case PointIntervalPositon.FirstPoint:
							return current;
						case PointIntervalPositon.LastPoint:
							return current.Next;
						case PointIntervalPositon.Inside:
							VertexDescriptor newVertex = new VertexDescriptor(point, vertex1.ContourIndex);
							newVertex.Next = current.Next;
							newVertex.Prev = current;
							current.Next = newVertex;
							newVertex.Next.Prev = newVertex;
							return newVertex;
						case PointIntervalPositon.Outside:
							break;
					}
				}
				current = current.Next;
			}
			return null;
		}
		static List<EdgeDescriptor> GetEdges(VertexDescriptor vertex, double epsilon) {
			if (vertex.InEdge == null) {
				double angle = MathUtils.CalcAxisXAngle2D(vertex.Point, vertex.Prev.Point, epsilon);
				vertex.InEdge = new EdgeDescriptor(vertex, angle, true);
				angle = MathUtils.CalcAxisXAngle2D(vertex.Point, vertex.Next.Point, epsilon);
				vertex.OutEdge = new EdgeDescriptor(vertex, angle, false);
				vertex.InEdge.Next = vertex.OutEdge;
				vertex.InEdge.Prev = vertex.OutEdge;
				vertex.OutEdge.Next = vertex.InEdge;
				vertex.OutEdge.Prev = vertex.InEdge;
			}
			List<EdgeDescriptor> edges = new List<EdgeDescriptor>();
			EdgeDescriptor current = vertex.InEdge;
			do {
				edges.Add(current);
				current = current.Next;
			}
			while (current != vertex.InEdge);
			return edges;
		}
		static void Combine(VertexDescriptor v1, VertexDescriptor v2, double epsilon) {
			if (v1 == null || v2 == null)
				return;
			List<EdgeDescriptor> list1 = GetEdges(v1, epsilon);
			List<EdgeDescriptor> list2 = GetEdges(v2, epsilon);
			foreach (EdgeDescriptor edge in list1)
				if (edge.VertexDescriptor == v2)
					return;
			list1.AddRange(list2);
			list1.Sort();
			for (int i = 0; i < list1.Count - 1; i++) {
				list1[i].Next = list1[i + 1];
				list1[i + 1].Prev = list1[i];
			}
			list1[list1.Count - 1].Next = list1[0];
			list1[0].Prev = list1[list1.Count - 1];
		}
		static bool CalcIntersections(VertexDescriptor vertexA, VertexDescriptor vertexB, bool doubleIntersections, double epsilon) {
			bool sected = false;
			VertexDescriptor currentA1 = vertexA;
			VertexDescriptor currentA2 = vertexA.Next;
			do {
				VertexDescriptor currentB1 = vertexB;
				VertexDescriptor currentB2 = vertexB.Next;
				do {
					List<DiagramPoint> points;
					IntersectionUtils.CalcLinesIntersection2D(currentA1.Point, currentA2.Point, currentB1.Point, currentB2.Point, epsilon, out points);
					if (points != null && points.Count > 0) {
						sected = true;
						foreach (DiagramPoint point in points) {
							VertexDescriptor vA = AddVertex(currentA1, currentA2, point, epsilon);
							VertexDescriptor vB = AddVertex(currentB1, currentB2, point, epsilon);
							if (vA != null && vB != null) {
								if (doubleIntersections)
									vA.Doubled = vB.Doubled = true;
								vA.Doubled = vB.Doubled = vB.Doubled || vA.Doubled;
								Combine(vA, vB, epsilon);
							}
						}
					}
					currentB1 = currentB2;
					currentB2 = currentB2.Next;
				}
				while (currentB1 != vertexB);
				currentA1 = currentA2;
				currentA2 = currentA2.Next;
			}
			while (currentA1 != vertexA);
			return sected;
		}
		static VertexDescriptor BuildContour(IList<DiagramPoint> contour, int contourIndex, double epsilon) {
			VertexDescriptor first = new VertexDescriptor(contour[0], contourIndex);
			VertexDescriptor current = first;
			for (int i = 1; i < contour.Count; i++) {
				if (MathUtils.ArePointsEquals2D(current.Point, contour[i], epsilon)) {
					current.Doubled = true;
					continue;
				}
				current.Next = new VertexDescriptor(contour[i], contourIndex);
				current.Next.Prev = current;
				current = current.Next;
			}
			if (MathUtils.ArePointsEquals2D(current.Point, first.Point, epsilon)) {
				first.Doubled = true;
				current = current.Prev;
			}
			current.Next = first;
			current.Next.Prev = current;
			return first;
		}
		static bool IsContourValid(IList<DiagramPoint> contour) {
			return contour != null && contour.Count >= 3;
		}
		static void MarkContours(VertexDescriptor vertexA, VertexDescriptor vertexB, IList<DiagramPoint> contourA, IList<DiagramPoint> contourB, bool sected, double epsilon) {
			if (sected) {
				vertexA.ContourMark = ContourMark.Sected;
				vertexB.ContourMark = ContourMark.Sected;
			}
			else {
				vertexA.ContourMark = IntersectionUtils.IsPointInPolygon(vertexA.Point, contourB, epsilon) ? ContourMark.Inside : ContourMark.Outside;
				if (vertexA.ContourMark == ContourMark.Inside)
					vertexB.ContourMark = ContourMark.Outside;
				else
					vertexB.ContourMark = IntersectionUtils.IsPointInPolygon(vertexB.Point, contourA, epsilon) ? ContourMark.Inside : ContourMark.Outside;
			}
		}
		static bool MarkUncrossed(VertexDescriptor vertex, IList<VertexDescriptor> vertices1, IList<VertexDescriptor> vertices2, IList<DiagramPoint> contour, bool first, double epsilon) {
			if (vertices1.Count > 0 || vertices2.Count > 0)
				return false;
			if (first)
				vertex.EdgeMark = IntersectionUtils.IsPointInPolygon(vertex.Point, contour, epsilon) ? EdgeMark.Inside : EdgeMark.Outside;
			else
				vertex.EdgeMark = vertex.Prev.EdgeMark;
			return true;
		}
		static bool MarkShared(VertexDescriptor vertex, IList<VertexDescriptor> vertices1, IList<VertexDescriptor> vertices2) {
			if (vertices1.Count == 0 || vertices2.Count == 0)
				return false;
			foreach (VertexDescriptor v in vertices1) {
				if (vertices2.Contains(v.Next)) {
					vertex.EdgeMark = EdgeMark.Shared1;
					return true;
				}
				if (vertices2.Contains(v.Prev)) {
					vertex.EdgeMark = EdgeMark.Shared2;
					return true;
				}
			}
			return false;
		}
		static bool MarkInsideByAngles(VertexDescriptor vertex, List<VertexDescriptor> vertices, bool firstVertex) {
			foreach (VertexDescriptor v in vertices) {
				double angle = firstVertex ? vertex.OutEdge.Angle : vertex.Next.InEdge.Angle;
				double angle1 = v.InEdge.Angle;
				double angle2 = v.OutEdge.Angle;
				if (angle == angle1 || angle == angle2 ||
					(angle2 > angle1 && (angle > angle2 || angle < angle1)) ||
					(angle1 > angle2 && angle < angle1 && angle > angle2)) {
					vertex.EdgeMark = EdgeMark.Inside;
					return true;
				}
			}
			return false;
		}
		static void MarkEdges(VertexDescriptor vertex, IList<DiagramPoint> contour, double epsilon) {
			VertexDescriptor current = vertex;
			List<VertexDescriptor> vertices1 = vertex.GetCrossVertices();
			do {
				List<VertexDescriptor> vertices2 = current.Next.GetCrossVertices();
				if (!MarkUncrossed(current, vertices1, vertices2, contour, current == vertex, epsilon))
					if (!MarkShared(current, vertices1, vertices2))
						if (!MarkInsideByAngles(current, vertices1, true))
							if (!MarkInsideByAngles(current, vertices2, false))
								current.EdgeMark = EdgeMark.Outside;
				current = current.Next;
				vertices1 = vertices2;
			}
			while (current != vertex);
		}
		static bool AreIntersects(VertexDescriptor vertexA, VertexDescriptor vertexB, double epsilon) {
			Combine(vertexA, vertexB, epsilon);
			bool result =
				vertexA.InEdge.Angle != vertexB.InEdge.Angle &&
				vertexA.InEdge.Angle != vertexB.OutEdge.Angle &&
				vertexA.OutEdge.Angle != vertexB.InEdge.Angle &&
				vertexA.OutEdge.Angle != vertexB.OutEdge.Angle &&
				vertexA.InEdge.Next != vertexA.OutEdge &&
				vertexA.InEdge.Prev != vertexA.OutEdge;
			vertexA.InEdge = null;
			vertexA.OutEdge = null;
			vertexB.InEdge = null;
			vertexB.OutEdge = null;
			return result;
		}
		static bool CalcSelfContacts(VertexDescriptor vertex, double epsilon) {
			VertexDescriptor current = vertex;
			do {
				VertexDescriptor next = current.Next;
				DiagramPoint p1 = current.Point;
				DiagramPoint p2 = next.Point;
				VertexDescriptor current1 = next;
				do {
					VertexDescriptor next1 = current1.Next;
					DiagramPoint p3 = current1.Point;
					DiagramPoint p4 = next1.Point;
					List<DiagramPoint> points;
					Intersection intersection = IntersectionUtils.CalcLinesIntersection2D(p1, p2, p3, p4, epsilon, out points);
					switch (intersection) {
						case Intersection.Yes:
							if (MathUtils.ArePointsEquals2D(p2, points[0], epsilon))
								break;
							VertexDescriptor v = AddVertex(current, next, points[0], epsilon);
							VertexDescriptor v1 = AddVertex(current1, next1, points[0], epsilon);
							if (AreIntersects(v, v1, epsilon))
								return false;
							break;
						case Intersection.Match:
							if (points.Count > 1)
								return false;
							if (points.Count == 1) {
								AddVertex(current, next, points[0], epsilon);
								AddVertex(current1, next1, points[0], epsilon);
							}
							break;
						case Intersection.No:
							break;
					}
					current1 = current1.Next;
				}
				while (current1 != current);
				current = current.Next;
			}
			while (current != vertex);
			return true;
		}
		static bool CalcContours(IList<DiagramPoint> contourA, IList<DiagramPoint> contourB, bool doubleIntersection, double epsilon, out VertexDescriptor vertexA, out VertexDescriptor vertexB) {
			vertexA = null;
			vertexB = null;
			if (!IsContourValid(contourA) || !IsContourValid(contourB))
				return false;
			vertexA = BuildContour(contourA, 1, epsilon);
			if (!CalcSelfContacts(vertexA, epsilon))
				return false;
			vertexB = BuildContour(contourB, 2, epsilon);
			if (!CalcSelfContacts(vertexB, epsilon))
				return false;
			bool sected = CalcIntersections(vertexA, vertexB, doubleIntersection, epsilon);
			MarkContours(vertexA, vertexB, contourA, contourB, sected, epsilon);
			MarkEdges(vertexA, contourB, epsilon);
			MarkEdges(vertexB, contourA, epsilon);
			return true;
		}
		static bool IsEdgeValid(Operation operation, VertexDescriptor vertex, out bool forward) {
			switch (operation) {
				case Operation.Intersection:
					forward = true;
					return vertex.EdgeMark == EdgeMark.Inside || vertex.EdgeMark == EdgeMark.Shared1;
				case Operation.Union:
					forward = true;
					return vertex.EdgeMark == EdgeMark.Outside || vertex.EdgeMark == EdgeMark.Shared1;
				default:
					throw new DefaultSwitchException();
			}
		}
		static bool Jump(Operation operation, ref VertexDescriptor vertex, ref bool forward) {
			EdgeDescriptor start = forward ? vertex.InEdge.Prev : vertex.OutEdge.Prev;
			EdgeDescriptor current = start;
			do {
				VertexDescriptor v = current.VertexDescriptor;
				bool newForward;
				if (!v.Passed && IsEdgeValid(operation, v, out newForward)) {
					if ((current.IsIn && !newForward) ||
						(!current.IsIn && newForward)) {
						forward = newForward;
						vertex = v;
						return true;
					}
				}
				current = current.Prev;
			}
			while (current != start);
			return false;
		}
		static IList<DiagramPoint> ConstructContour(Operation operation, VertexDescriptor vertex, bool forward) {
			List<DiagramPoint> points = new List<DiagramPoint>();
			VertexDescriptor currentEdge = vertex.GetEdgeVertex(forward);
			VertexDescriptor currentVertex = vertex;
			do {
				points.Add(currentVertex.Point);
				if (currentVertex.Doubled)
					points.Add(currentVertex.Point);
				currentEdge.Passed = true;
				if (currentEdge.EdgeMark == EdgeMark.Shared1 || currentEdge.EdgeMark == EdgeMark.Shared2) {
					List<VertexDescriptor> vertices1 = currentEdge.GetCrossVertices();
					List<VertexDescriptor> vertices2 = currentEdge.Next.GetCrossVertices();
					foreach (VertexDescriptor v in vertices1) {
						if (currentEdge.EdgeMark == EdgeMark.Shared1 && vertices2.Contains(v.Next))
							v.Passed = true;
						if (currentEdge.EdgeMark == EdgeMark.Shared2 && vertices2.Contains(v.Prev))
							v.Prev.Passed = true;
					}
				}
				currentVertex = forward ? currentVertex.Next : currentVertex.Prev;
				if (currentVertex.InEdge != null && !Jump(operation, ref currentVertex, ref forward))
					break;
				currentEdge = currentVertex.GetEdgeVertex(forward);
			}
			while (!currentEdge.Passed);
			return points;
		}
		static void ConstructSectedContours(Operation operation, VertexDescriptor vertex, IList<IList<DiagramPoint>> contours) {
			VertexDescriptor current = vertex;
			do {
				bool forward;
				if (!current.Passed && IsEdgeValid(operation, current, out forward)) {
					IList<DiagramPoint> contour = ConstructContour(operation, forward ? current : current.Next, forward);
					if (contour != null && contour.Count >= 3)
						contours.Add(contour);
				}
				current = current.Next;
			}
			while (current != vertex);
		}
		static void ConstructNonSectedContours(Operation operation, VertexDescriptor vertexA, VertexDescriptor vertexB, List<IList<DiagramPoint>> contours) {
			switch (operation) {
				case Operation.Intersection:
					if (vertexA.ContourMark == ContourMark.Inside)
						contours.Add(vertexA.GetContour(true));
					else if (vertexB.ContourMark == ContourMark.Inside)
						contours.Add(vertexB.GetContour(true));
					break;
				case Operation.Union:
					if (vertexA.ContourMark == ContourMark.Outside) {
						contours.Add(vertexA.GetContour(true));
						contours.Add(vertexB.GetContour(true));
					}
					break;
				default:
					throw new DefaultSwitchException();
			}
		}
		static IList<IList<DiagramPoint>> CalcOperation(Operation operation, IList<DiagramPoint> contourA, IList<DiagramPoint> contourB, bool doubleIntersections, double epsilon) {
			List<IList<DiagramPoint>> contours = new List<IList<DiagramPoint>>();
			VertexDescriptor vertexA, vertexB;
			if (!CalcContours(contourA, contourB, doubleIntersections, epsilon, out vertexA, out vertexB))
				return null;
			if (vertexA.ContourMark == ContourMark.Sected) {
				ConstructSectedContours(operation, vertexA, contours);
				ConstructSectedContours(operation, vertexB, contours);
			}
			else
				ConstructNonSectedContours(operation, vertexA, vertexB, contours);
			return contours;
		}
		public static List<IList<DiagramPoint>> CalcIntersection(IList<DiagramPoint> contourA, IList<DiagramPoint> contourB, DiagramVector normal, bool doubleIntersections, bool adjacentEdges, double epsilon) {
			using (Tessellator tess = new Tessellator()) {
				IList<IList<DiagramPoint>> contoursA = tess.Prepare(contourA, normal, adjacentEdges, epsilon);
				if (contoursA == null)
					return null;
				IList<IList<DiagramPoint>> contoursB = tess.Prepare(contourB, normal, adjacentEdges, epsilon);
				if (contoursB == null)
					return null;
				List<IList<DiagramPoint>> result = new List<IList<DiagramPoint>>();
				foreach (IList<DiagramPoint> cA in contoursA) {
					foreach (IList<DiagramPoint> cB in contoursB) {
						IList<IList<DiagramPoint>> intersection = CalcOperation(Operation.Intersection, cA, cB, doubleIntersections, epsilon);
						if (intersection != null)
							result.AddRange(intersection);
					}
				}
				return result;
			}
		}
		public static List<IList<DiagramPoint>> CalcIntersection(IList<DiagramPoint> contourA, IList<DiagramPoint> contourB, double epsilon) {
			return CalcIntersection(contourA, contourB, new DiagramVector(0, 0, 1), false, true, epsilon);
		}
		public static List<IList<DiagramPoint>> CalcIntersection(IList<DiagramPoint> contourA, IList<DiagramPoint> contourB, bool doubleIntersections, double epsilon) {
			return CalcIntersection(contourA, contourB, new DiagramVector(0, 0, 1), doubleIntersections, true, epsilon);
		}
		public static IList<IList<DiagramPoint>> Union(IList<DiagramPoint> contourA, IList<DiagramPoint> contourB, double epsilon) {
			return CalcOperation(Operation.Union, contourA, contourB, false, epsilon);
		}
		public static IList<IList<DiagramPoint>> Union(IList<IList<DiagramPoint>> contours, double epsilon) {
			List<IList<DiagramPoint>> actual = new List<IList<DiagramPoint>>(contours);
			for (int i = 0; i < actual.Count; i++) {
				if (actual[i] == null)
					continue;
				for (int k = 0; k < actual.Count; k++) {
					if (actual[k] == null || actual[k] == actual[i])
						continue;
					IList<IList<DiagramPoint>> c = Union(actual[i], actual[k], epsilon);
					if (c == null || c.Count == 0)
						return null;
					if (c.Count == 1) {
						actual[i] = c[0];
						actual[k] = null;
					}
				}
			}
			List<IList<DiagramPoint>> result = new List<IList<DiagramPoint>>();
			foreach (IList<DiagramPoint> contour in actual)
				if (contour != null)
					result.Add(contour);
			return result;
		}
	}
}
