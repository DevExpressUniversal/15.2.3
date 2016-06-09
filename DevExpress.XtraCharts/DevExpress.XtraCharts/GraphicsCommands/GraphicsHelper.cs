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
	public class PrimitivesContainer {
		List<Line> lines = new List<Line>();
		List<PlanePolygon> polygons = new List<PlanePolygon>();
		public List<Line> Lines { get { return lines; } }
		public List<PlanePolygon> Polygons { get { return polygons; } }
		public PrimitivesContainer() {
		}
		public PrimitivesContainer(IList<Line> lines, IList<PlanePolygon> polygons) {
			Add(lines, polygons);
		}
		public void Add(IList<Line> lines, IList<PlanePolygon> polygons) {
			if (lines != null)
				this.lines.AddRange(lines);
			if (polygons != null)
				this.polygons.AddRange(polygons);
		}
		public void Add(PrimitivesContainer container) {
			if (container == null)
				return;
			Add(container.lines, container.polygons);
		}
	}
	public abstract class PlaneGroup {
		#region inner classes
		protected class PlaneTriangleComparer : IComparer<PlaneTriangle> {
			int IComparer<PlaneTriangle>.Compare(PlaneTriangle triangle1, PlaneTriangle triangle2) {
				return triangle1.Weight.CompareTo(triangle2.Weight);
			}
		}
		protected class LineComparer : IComparer<Line> {
			int IComparer<Line>.Compare(Line line1, Line line2) {
				return line1.Weight.CompareTo(line2.Weight);
			}
		}
		#endregion
		protected static readonly LineComparer lineComparerByWeight = new LineComparer();
		protected static readonly PlaneTriangleComparer triangleComparerByWeight = new PlaneTriangleComparer();
		PlaneEquation plane;
		List<Line> lines = new List<Line>();
		List<PlaneTriangle> triangles = new List<PlaneTriangle>();
		int groupId;
		public List<Line> Lines { get { return lines; } }
		public List<PlaneTriangle> Triangles { get { return triangles; } }
		public PlaneGroup(PlaneTriangle triangle, int groupId) {
			this.groupId = groupId;
			this.plane = MathUtils.CalcPlaneEquation(triangle);
			triangles.Add(triangle);
		} 
		public PlaneGroup(PlaneTriangle triangle) : this(triangle, 0) {			
		}
		GraphicsCommand CreateLinesGrpahicsCommand() {
			GraphicsCommand container = new ContainerGraphicsCommand();
			foreach (Line line in lines)
				container.AddChildCommand(line.CreateGraphicsCommandWithPainter());
			return container;
		}
		GraphicsCommand CreateTrianglesGrpahicsCommand() {
			GraphicsCommand container = new ContainerGraphicsCommand();
			foreach (PlaneTriangle triangle in triangles)
				container.AddChildCommand(triangle.CreateGraphicsCommandWithPainter());
			return container;
		}
		protected GraphicsCommand CreateGraphicsCommandInternal(bool stencilBuffer) {
			GraphicsCommand command = stencilBuffer ? (GraphicsCommand)new StencilFuncGraphicsCommand(groupId) 
				: (GraphicsCommand)new ContainerGraphicsCommand();
			if (stencilBuffer) {
				command.AddChildCommand(CreateLinesGrpahicsCommand());
				command.AddChildCommand(CreateTrianglesGrpahicsCommand());
			}
			else {
				command.AddChildCommand(CreateTrianglesGrpahicsCommand());
				command.AddChildCommand(CreateLinesGrpahicsCommand());
			}
			return command;
		}
		public bool Add(PlaneTriangle triangle) {
			if (MathUtils.IsTriangleOnPlane(plane, triangle, Diagram3D.Epsilon)) {
				triangles.Add(triangle);
				return true;
			}
			return false;
		}
		public bool Add(Line line) {
			if (MathUtils.IsLineOnPlane(plane, line, Diagram3D.Epsilon)) {
				Lines.Add(line);
				return true;
			}
			return false;
		}
		public virtual void Sort() {
			lines.Sort(lineComparerByWeight);
			triangles.Sort(triangleComparerByWeight);
		}
	}
	public class Group : PlaneGroup {
		public Group(PlaneTriangle triangle, int groupId) : base(triangle, groupId) {
		}
		public GraphicsCommand CreateGraphicsCommand() {
			return CreateGraphicsCommandInternal(true);
		}
	}
	public class Node : PlaneGroup {
		DiagramVector normal;
		PlaneEquation plane;
		Node front, back;
		List<Line> backLines = new List<Line>();
		List<Line> frontLines = new List<Line>();
		public PlaneEquation Plane { get { return plane; } }
		public Node Back { get { return back; } set { back = value; } }
		public Node Front { get { return front; } set { front = value; } }
		public Node(PlaneTriangle triangle) : base(triangle) {
			normal = MathUtils.CalcNormal(triangle);
			plane = MathUtils.CalcPlaneEquation(triangle);
		}
		GraphicsCommand CreateBackGraphicsCommand(DiagramPoint eye) {
			GraphicsCommand container = new ContainerGraphicsCommand();
			foreach (Line line in backLines)
				container.AddChildCommand(line.CreateGraphicsCommandWithPainter());
			if (back != null)
				container.AddChildCommand(back.CreateGraphicsCommand(eye));
			return container;
		}
		GraphicsCommand CreateFrontGraphicsCommand(DiagramPoint eye) {
			GraphicsCommand container = new ContainerGraphicsCommand();
			foreach (Line line in frontLines)
				container.AddChildCommand(line.CreateGraphicsCommandWithPainter());
			if (front != null)
				container.AddChildCommand(front.CreateGraphicsCommand(eye));
			return container;
		}
		GraphicsCommand CreateRootGraphicsCommand() {
			return CreateGraphicsCommandInternal(false);
		}
		public void AddRootLines(List<Line> lines) {
			if (lines == null)
				return;
			foreach (Line line in lines)
				Add(line);
		}
		public void AddBackLines(List<Line> lines) {
			if (lines == null)
				return;
			backLines.AddRange(lines);
		}
		public void AddFrontLines(List<Line> lines) {
			if (lines == null)
				return;
			frontLines.AddRange(lines);
		}
		public GraphicsCommand CreateGraphicsCommand(DiagramPoint eye) {
			GraphicsCommand container = new ContainerGraphicsCommand();
			int k = MathUtils.IsPointOnPlane(plane, eye, Diagram3D.Epsilon);
			if (k == 0) {
				container.AddChildCommand(CreateBackGraphicsCommand(eye));
				container.AddChildCommand(CreateFrontGraphicsCommand(eye));
			}
			else if (k > 0) {
				container.AddChildCommand(CreateBackGraphicsCommand(eye));
				container.AddChildCommand(CreateRootGraphicsCommand());
				container.AddChildCommand(CreateFrontGraphicsCommand(eye));
			}
			else if (k < 0) {
				container.AddChildCommand(CreateFrontGraphicsCommand(eye));
				container.AddChildCommand(CreateRootGraphicsCommand());
				container.AddChildCommand(CreateBackGraphicsCommand(eye));
			}
			return container;
		}
		public override void Sort() {
			base.Sort();
			backLines.Sort(lineComparerByWeight);
			frontLines.Sort(lineComparerByWeight);
			if (back != null)
				back.Sort();
			if (front != null)
				front.Sort();
		}
	}
	public class PolygonGroups {
		List<Group> groups;
		List<Line> ungroupedLines;
		public PolygonGroups(List<Group> groups, List<Line> ungroupedLines) {
			this.groups = groups;
			this.ungroupedLines = ungroupedLines;
		}
		public GraphicsCommand CreateGraphicsCommand() {
			GraphicsCommand commandContainer = new ContainerGraphicsCommand();
			if (groups != null) {
				StencilBufferGraphicsCommand stencilBufferGraphicsCommand = new StencilBufferGraphicsCommand();
				commandContainer.AddChildCommand(stencilBufferGraphicsCommand);
				foreach (Group group in groups)
					stencilBufferGraphicsCommand.AddChildCommand(group.CreateGraphicsCommand());
			}
			if (ungroupedLines != null) {
				foreach (Line line in ungroupedLines)
					commandContainer.AddChildCommand(line.CreateGraphicsCommandWithPainter());
			}
			return commandContainer;
		}
		public void Sort() {
			if (groups != null) {
				foreach (Group group in groups)
					group.Sort();
			}			
		}
	}
	public sealed class GraphicsHelper {
		static int currentGroupId = 1;
		static List<PlaneTriangle> Triangulate(PlanePolygon polygon) {
			List<PlaneTriangle> triangles = new List<PlaneTriangle>();
			if (polygon == null)
				return triangles;
			if (polygon is PlaneTriangle) {
				triangles.Add((PlaneTriangle)polygon);
				return triangles;
			}
			Vertex v1 = polygon.Vertices[0];
			for (int i = 1, k = 0; i < polygon.Vertices.Length - 1; i++, k++) {
				Vertex v2 = polygon.Vertices[i];
				Vertex v3 = polygon.Vertices[i + 1];
				PlaneTriangle triangle = polygon.CreateTriangle(v1, v2, v3);
				if (IsTriangleValid(triangle))
					triangles.Add(triangle);
			}
			return triangles;
		}
		static List<PlaneTriangle> Triangulate(List<PlanePolygon> polygons) {
			if (polygons == null || polygons.Count == 0)
				return null;
			List<PlaneTriangle> triangles = new List<PlaneTriangle>();
			foreach (PlanePolygon polygon in polygons)
				triangles.AddRange(Triangulate(polygon));
			return triangles;
		}
		static bool IsTriangleValid(PlaneTriangle triangle) {
			return !MathUtils.CalcNormal(triangle).IsZero;
		}
		static bool IsLineValid(Line line) {
			return !MathUtils.ArePointsEquals(line.V1, line.V2, Diagram3D.Epsilon);
		}
		static List<Group> CreateGroups(List<PlaneTriangle> triangles) {
			if (triangles == null || triangles.Count == 0)
				return null;
			List<Group> groups = new List<Group>();
			foreach (PlaneTriangle triangle in triangles) {
				if (!IsTriangleValid(triangle))
					continue;
				bool added = false;
				foreach (Group group in groups) {
					if (group.Add(triangle)) {
						added = true;
						break;
					}
				}
				if (!added) {
					Group group = new Group(triangle, currentGroupId);
					groups.Add(group);
					currentGroupId++;					
				}
			}
			return groups;
		}
		static Node BuildTree(List<PlaneTriangle> baseTriangles) {
			if (baseTriangles == null || baseTriangles.Count == 0)
				return null;
			Node root = new Node(baseTriangles[0]);
			List<PlaneTriangle> backTriangles = new List<PlaneTriangle>();
			List<PlaneTriangle> frontTriangles = new List<PlaneTriangle>();
			for (int i = 1; i < baseTriangles.Count; i++) {
				if (root.Add(baseTriangles[i]))
					continue;
				Vertex[] vertices;
				PlanePolygon frontPolygon;
				PlanePolygon backPolygon;
				Intersection intersection = IntersectionUtils.CalcPolygonIntersection3D(root.Plane, 
					baseTriangles[i], Diagram3D.Epsilon, out vertices, out frontPolygon, out backPolygon);
				switch (intersection) {
					case Intersection.Yes:
						List<PlaneTriangle> triangles = Triangulate(frontPolygon);
						if (triangles != null)
							frontTriangles.AddRange(triangles);
						triangles = Triangulate(backPolygon);
						if (triangles != null)
							backTriangles.AddRange(triangles);
						break;
					case Intersection.No:
						goto case Intersection.Yes;
					case Intersection.Match:
						root.Add(baseTriangles[i]);
						break;
					default:
						throw new DefaultSwitchException();
				}
			}
			root.Back = BuildTree(backTriangles);
			root.Front = BuildTree(frontTriangles);
			return root;
		}
		static void CalcBackAndFrontTriangles(Node root, List<PlaneTriangle> triangles, out List<PlaneTriangle> backTriangles, out List<PlaneTriangle> frontTriangles) {
			backTriangles = new List<PlaneTriangle>();
			frontTriangles = new List<PlaneTriangle>();
			if (triangles == null)
				return;
			for (int i = 1; i < triangles.Count; i++) {
				if (root.Add(triangles[i]))
					continue;
				Vertex[] vertices;
				PlanePolygon frontPolygon, backPolygon;
				Intersection intersection = IntersectionUtils.CalcPolygonIntersection3D(root.Plane, 
					triangles[i], Diagram3D.Epsilon, out vertices, out frontPolygon, out backPolygon);
				switch (intersection) {
					case Intersection.Yes:
						List<PlaneTriangle> list = Triangulate(frontPolygon);
						if (list != null)
							frontTriangles.AddRange(list);
						list = Triangulate(backPolygon);
						if (list != null)
							backTriangles.AddRange(list);
						break;
					case Intersection.No:
						goto case Intersection.Yes;
					case Intersection.Match:
						root.Add(triangles[i]);
						break;
					default:
						throw new DefaultSwitchException();
				}
			}
		}
		static void CalcBackAndFrontLines(Node root, List<Line> lines, out List<Line> backLines, out List<Line> frontLines) {
			backLines = new List<Line>();
			frontLines = new List<Line>();
			if (lines == null)
				return;
			for (int i = 0; i < lines.Count; i++) {
				if (root.Add(lines[i]))
					continue;
				Vertex vertex;
				Line frontLine, backLine;
				Intersection intersection = IntersectionUtils.CalcLineIntersection3D(root.Plane,
					lines[i], Diagram3D.Epsilon, out vertex, out frontLine, out backLine);
				switch (intersection) {
					case Intersection.Yes:
						if (frontLine != null)
							frontLines.Add(frontLine);
						if (backLine != null)
							backLines.Add(backLine);
						break;
					case Intersection.No:
						goto case Intersection.Yes;
					case Intersection.Match:
						root.Add(lines[i]);
						break;
					default:
						throw new DefaultSwitchException();
				}
			}
		}
		static Node BuildTree(List<PlaneTriangle> triangles, List<Line> lines) {
			if (triangles == null || triangles.Count == 0)
				return null;
			Node root = new Node(triangles[0]);
			List<PlaneTriangle> backTriangles, frontTriangles;
			CalcBackAndFrontTriangles(root, triangles, out backTriangles, out frontTriangles);
			List<Line> backLines, frontLines;
			CalcBackAndFrontLines(root, lines, out backLines, out frontLines);
			List<Line> newLines = new List<Line>();
			newLines.AddRange(root.Lines);
			newLines.AddRange(backLines);
			root.Back = BuildTree(backTriangles, newLines);
			if (root.Back == null)
				root.AddBackLines(backLines);
			newLines = new List<Line>();
			newLines.AddRange(root.Lines);
			newLines.AddRange(frontLines);
			root.Front = BuildTree(frontTriangles, newLines);
			if (root.Front == null)
				root.AddFrontLines(frontLines);
			return root;
		}
		static List<PlaneTriangle> GetValidTriangles(List<PlaneTriangle> triangles) {
			if (triangles == null || triangles.Count == 0)
				return null;
			List<PlaneTriangle> validTriangles = new List<PlaneTriangle>();
			foreach (PlaneTriangle triangle in triangles)
				if (IsTriangleValid(triangle))
					validTriangles.Add(triangle);
			return validTriangles;
		}
		static List<Line> GetValidLines(List<Line> lines) {
			if (lines == null || lines.Count == 0)
				return null;
			List<Line> validLines = new List<Line>();
			foreach (Line line in lines)
				if (IsLineValid(line))
					validLines.Add(line);
			return validLines;
		}
		static List<Line> AddLines(List<Group> groups, List<Line> lines) {
			if (groups == null)
				return lines == null ? null : new List<Line>(lines);
			if (lines == null)
				return null;
			List<Line> ungroupedLines = new List<Line>();
			foreach (Line line in lines) {
				bool added = false;
				foreach (Group group in groups) {
					if (group.Add(line))
						added = true;
				}
				if (!added)
					ungroupedLines.Add(line);
			}
			return ungroupedLines;
		}
		public static Node BuildGraphicsTree(PrimitivesContainer container) {
			try {
				if (container == null)
					return null;
				List<Line> validLines = GetValidLines(container.Lines);
				List<PlaneTriangle> validTriangles = GetValidTriangles(Triangulate(container.Polygons));
				Node root = BuildTree(validTriangles, validLines);
				if (root != null)
					root.Sort();
				return root;
			}
			catch (ArithmeticException) {
				return null;
			}
		}
		public static PolygonGroups BuildGroups(PrimitivesContainer container) {
			try {
				List<Group> groups = CreateGroups(Triangulate(container.Polygons));
				return new PolygonGroups(groups, AddLines(groups, container.Lines));
			}
			catch (ArithmeticException) {
				return null;
			}
		}
	}
}
