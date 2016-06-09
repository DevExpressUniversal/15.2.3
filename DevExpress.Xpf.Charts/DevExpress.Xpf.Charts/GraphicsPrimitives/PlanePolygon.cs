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
using System.Windows.Media.Media3D;
namespace DevExpress.Xpf.Charts.Native {
	public class PlanePolygon : ICloneable {
		public static PlanePolygon Offset(PlanePolygon polygon, double dx, double dy, double dz) {
			PlanePolygon clone = (PlanePolygon)polygon.Clone();
			clone.Offset(dx, dy, dz);
			return clone;
		}
		List<Vertex> vertices = new List<Vertex>();
		Material material;
		bool textureCoordsEnabled;
		public List<Vertex> Vertices { get { return vertices; } }
		public Material Material { get { return material; } set { material = value; } }
		public bool TextureCoordsEnabled { get { return textureCoordsEnabled; } set { textureCoordsEnabled = value; } }
		protected PlanePolygon() {
		}
		public PlanePolygon(IList<Point3D> points) {
			SetVertices(points);
		}
		void Assign(PlanePolygon polygon) {
			if (polygon == null)
				throw new ArgumentNullException("primitive");
			material = polygon.material;
			textureCoordsEnabled = polygon.textureCoordsEnabled;
			Vertex[] vertices = new Vertex[polygon.vertices.Count];
			polygon.vertices.CopyTo(vertices, 0);
			SetVertices(vertices);
		}
		public virtual object Clone() {
			PlanePolygon polygon = CreateInstance();
			polygon.Assign(this);
			return polygon;
		}
		protected void SetVertices(IList<Point3D> points) {
			Vertex[] vertices = new Vertex[points.Count];
			for (int i = 0; i < points.Count; i++)
				vertices[i] = new Vertex(points[i]);
			SetVertices(vertices);
		}
		protected void SetVertices(IList<Vertex> vertices) {
			this.vertices.Clear();
			this.vertices.AddRange(vertices);
		}
		protected virtual PlanePolygon CreateInstance() {
			return new PlanePolygon();
		}
		public void InvertVerticesDirection() {
			Vertex[] newVertices = new Vertex[vertices.Count];
			for (int i = 0; i < vertices.Count; i++) {
				int k = i == 0 ? 0 : vertices.Count - i;
				newVertices[i] = vertices[k];
			}
			vertices.Clear();
			vertices.AddRange(newVertices);
		}
		public virtual MeshGeometry3D GetGeometry() {
			MeshGeometry3D geometry = new MeshGeometry3D();
			foreach (Vertex vertex in vertices) {
				geometry.Positions.Add(vertex.Position);
				if (textureCoordsEnabled)
					geometry.TextureCoordinates.Add(vertex.TextureCoord);
			}
			for (int i = 0; i < vertices.Count - 2; i++) {
				geometry.TriangleIndices.Add(i);
				geometry.TriangleIndices.Add(i + 1);
				geometry.TriangleIndices.Add(vertices.Count - 1);
			}
			return geometry;
		}
		public Model3D GetModel() {
			MeshGeometry3D geometry = GetGeometry();
			return new GeometryModel3D(geometry, material);
		}
		public void Offset(double dx, double dy, double dz) {
			for (int i = 0; i < vertices.Count; i++)
				vertices[i] = Vertex.Offset(vertices[i], dx, dy, dz);
		}
	}
}
