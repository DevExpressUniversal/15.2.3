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
using System.Collections.Generic;
namespace DevExpress.XtraCharts.Native {
	public abstract class PlanePrimitive {
		public delegate GraphicsCommand PaintingMethod(PlanePrimitive primitive);
		public static PlanePrimitive Offset(PlanePrimitive primitive, double dx, double dy, double dz) {
			PlanePrimitive result = primitive.CreateInstance();
			result.Assign(primitive);
			result.Offset(dx, dy, dz);
			return result;
		}
		Vertex[] vertices;
		DiagramVector normal;
		Color color;
		bool sameNormals, sameColors;
		int weight;
		bool visible = true;
		PaintingMethod paintingMethod;
		public Vertex[] Vertices { get { return vertices; } set { SetVertices(value); } }
		public DiagramVector Normal { get { return normal; } set { normal = value; } }
		public Color Color { get { return color; } set { color = value; } }
		public bool SameNormals { get { return sameNormals; } set { sameNormals = value; } }
		public bool SameColors { get { return sameColors; } set { sameColors = value; } }
		public int Weight { get { return weight; } set { weight = value; } }
		public bool Visible { get { return visible; } set { visible = value; } }
		protected PlanePrimitive() {
		}
		public PlanePrimitive(IList<DiagramPoint> points) {
			SetVertices(points);
		}
		public PlanePrimitive(DiagramPoint[] points, bool sameNormals, bool sameColors, DiagramVector normal, Color color) {
			this.sameNormals = sameNormals;
			this.sameColors = sameColors;
			this.normal = normal;
			this.color = color;
			SetVertices(points);
		}
		public PlanePrimitive(Vertex[] vertices) {
			SetVertices(vertices);
		}
		public PlanePrimitive(Vertex[] vertices, bool sameNormals, bool sameColors, DiagramVector normal, Color color) {
			this.sameNormals = sameNormals;
			this.sameColors = sameColors;
			this.normal = normal;
			this.color = color;
			SetVertices(vertices);
		}
		public virtual void AssignProperties(PlanePrimitive primitive) {
			if (primitive == null)
				throw new ArgumentNullException("primitive");
			sameNormals = primitive.sameNormals;
			sameColors = primitive.sameColors;
			normal = primitive.normal;
			color = primitive.color;
			weight = primitive.weight;
			visible = primitive.visible;
			paintingMethod = primitive.paintingMethod;
		}
		protected void Assign(PlanePrimitive primitive) {
			if (primitive == null)
				throw new ArgumentNullException("primitive");
			Vertex[] vertices = new Vertex[primitive.vertices.Length];
			primitive.vertices.CopyTo(vertices, 0);
			SetVertices(vertices);
			AssignProperties(primitive);
		}
		DiagramPoint[] MakePointsArray(Vertex[] vertices) {
			if (vertices == null)
				return null;
			DiagramPoint[] points = new DiagramPoint[vertices.Length];
			for (int i = 0; i < vertices.Length; i++)
				points[i] = vertices[i];
			return points;
		}
		protected void SetVertices(Vertex[] vertices) {
#if DEBUGTEST
			if (vertices == null)
				throw new ArgumentNullException();
			if (!IsVerticesCountValid(vertices.Length))
				throw new ArgumentException();
			DiagramPoint[] points = MakePointsArray(vertices);
			if (!MathUtils.IsOnePlanePoints(points, Diagram3D.Epsilon))
				throw new ArgumentException();
#endif
			this.vertices = vertices;
		}
		protected void SetVertices(IList<DiagramPoint> points) {
#if DEBUGTEST
			if (points == null)
				throw new ArgumentNullException();
			if (!IsVerticesCountValid(points.Count))
				throw new ArgumentException();
			if (!MathUtils.IsOnePlanePoints(points, Diagram3D.Epsilon))
				throw new ArgumentException();
#endif
			vertices = new Vertex[points.Count];
			for (int i = 0; i < vertices.Length; i++)
				vertices[i].Point = points[i];
		}
		protected abstract bool IsVerticesCountValid(int count);
		protected abstract PlanePrimitive CreateInstance();
		protected internal abstract GraphicsCommand CreateGraphicsCommand();
		public override bool Equals(object obj) {
			if (obj == null)
				return false;
			if (!GetType().Equals(obj.GetType()))
				return false;
			PlanePrimitive primitive = (PlanePrimitive)obj;
			if (sameNormals != primitive.sameNormals ||
				sameColors != primitive.sameColors ||
				!normal.Equals(primitive.normal) ||
				!color.Equals(primitive.color) ||
				weight != primitive.weight ||
				visible != primitive.visible ||
				paintingMethod != primitive.paintingMethod ||
				vertices.Length != primitive.vertices.Length)
				return false;
			for (int i = 0; i < vertices.Length; i++)
				if (!vertices[i].Equals(primitive.vertices[i]))
					return false;
			return true;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public void Offset(double dx, double dy, double dz) {
			for (int i = 0; i < vertices.Length; i++)
				vertices[i].Offset(dx, dy, dz);
		}
		public void SetPaintingMethod(PaintingMethod paintingMethod) {
			this.paintingMethod = paintingMethod;
		}
		public void Offset(DiagramPoint offset) {
			Offset(offset.X, offset.Y, offset.Z);
		}
		public void InvertVerticesDirection() {
			Vertex[] newVertices = new Vertex[vertices.Length];
			for (int i = 0, k = vertices.Length - 1; i < vertices.Length; i++, k--)
				newVertices[k] = vertices[i];
			vertices = newVertices;
			normal.Revert();
		}
		public IList<DiagramPoint> GetContour() {
			List<DiagramPoint> contour = new List<DiagramPoint>();
			foreach (DiagramPoint vertex in vertices)
				contour.Add(vertex);
			return contour;
		}
		public GraphicsCommand CreateGraphicsCommandWithPainter() {
			return paintingMethod == null ? CreateGraphicsCommand() : paintingMethod(this);
		}
	}
}
