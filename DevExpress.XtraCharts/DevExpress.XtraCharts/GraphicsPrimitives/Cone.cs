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
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Native {
	public class Cone {
		const double startAngle = Math.PI / 2;
		List<PlanePolygon> laterals = new List<PlanePolygon>();
		PlanePolygon topBase;
		PlanePolygon bottomBase;
		DiagramPoint location;
		double width;
		double height;
		double depth;
		protected virtual bool IsSmoothing { get { return true; } }
		public double Width { get { return width; } }
		public double Height { get { return height; } }
		public double Depth { get { return depth; } }
		public DiagramPoint Location { get { return location; } }
		public List<PlanePolygon> Laterals { get { return laterals; } }
		public PlanePolygon TopBase { get { return topBase; } }
		public PlanePolygon BottomBase { get { return bottomBase; } }
		public Cone(YPlaneRectangle bottomBase, YPlaneRectangle topBase, int segmentsCount) {
			if (ComparingUtils.CompareDoubles(bottomBase.Center.X, topBase.Center.X, Diagram3D.Epsilon) != 0 ||
				ComparingUtils.CompareDoubles(bottomBase.Center.Z, topBase.Center.Z, Diagram3D.Epsilon) != 0) {
				double dx = topBase.Center.X - bottomBase.Center.X;
				double dz = topBase.Center.Z - bottomBase.Center.Z;
				topBase.Offset(dx, 0, dz);
			}
			CreateTruncatedCone(bottomBase, topBase, segmentsCount);
		}
		public Cone(YPlaneRectangle coneBase, double coneHeight, double beginHeight, double endHeight, int segmentsCount) {
			double newWidth = coneBase.Height * (coneHeight - beginHeight) / coneHeight;
			double newDepth = coneBase.Width * (coneHeight - beginHeight) / coneHeight;
			DiagramPoint newLocation = DiagramPoint.Offset(coneBase.Location, (coneBase.Height - newWidth) / 2, beginHeight, (coneBase.Width - newDepth) / 2);
			YPlaneRectangle bottomBase = new YPlaneRectangle(newLocation, newDepth, newWidth);
			bool isTruncatedPyramid = coneHeight >= 0 ? endHeight < coneHeight : endHeight > coneHeight;
			if (isTruncatedPyramid) {
				newWidth = coneBase.Height * (coneHeight - endHeight) / coneHeight;
				newDepth = coneBase.Width * (coneHeight - endHeight) / coneHeight;
				newLocation = DiagramPoint.Offset(coneBase.Location, (coneBase.Height - newWidth) / 2, endHeight, (coneBase.Width - newDepth) / 2);
				YPlaneRectangle topBase = new YPlaneRectangle(newLocation, newDepth, newWidth);
				CreateTruncatedCone(bottomBase, topBase, segmentsCount);
			}
			else {
				CreateSimpleCone(bottomBase, endHeight - beginHeight, segmentsCount);
			}
		}
		protected virtual PlanePolygon CalcBase(YPlaneRectangle rectBase, int segmentsCount) {
			PlanePolygon result = new PlanePolygon(MathUtils.CalcInscribedEllipsePoints(rectBase, startAngle, segmentsCount));
			result.SameNormals = true;
			result.Normal = MathUtils.CalcNormal(result);
			return result;
		}
		DiagramVector[] CalcNormalsForTruncatedCone(DiagramPoint centerBase) {
			if (topBase == null || bottomBase == null || bottomBase.Vertices.Length < 1)
				return null;
			DiagramVector[] normals = new DiagramVector[bottomBase.Vertices.Length];
			double coneHeight = topBase.Vertices[0].Y - bottomBase.Vertices[0].Y;
			for (int i = 0; i < bottomBase.Vertices.Length; i++) {
				double dx = bottomBase.Vertices[i].X - topBase.Vertices[i].X;
				double dz = bottomBase.Vertices[i].Z - topBase.Vertices[i].Z;
				double radius = Math.Sqrt(dx * dx + dz * dz);
				double coneAngle = coneHeight != 0 ? Math.Atan2(radius, coneHeight) : Math.PI / 2;
				double y = Math.Sin(coneAngle);
				double xz = Math.Cos(coneAngle);
				double x = bottomBase.Vertices[i].X - centerBase.X;
				double z = bottomBase.Vertices[i].Z - centerBase.Z;
				normals[i] = new DiagramVector(2 * x * xz / width, y, 2 * z * xz / depth);
				normals[i].Normalize();
			}
			return normals;
		}
		DiagramVector[] CalcNormalsForSimpleCone(DiagramPoint coneVertex) {
			if (bottomBase == null || bottomBase.Vertices.Length < 1)
				return null;
			DiagramVector[] normals = new DiagramVector[bottomBase.Vertices.Length];
			double coneHeight = coneVertex.Y - bottomBase.Vertices[0].Y;
			for (int i = 0; i < bottomBase.Vertices.Length; i++) {
				double x = bottomBase.Vertices[i].X - coneVertex.X;
				double z = bottomBase.Vertices[i].Z - coneVertex.Z;
				double radius = Math.Sqrt(x * x + z * z);
				double coneAngle = coneHeight != 0 ? Math.Atan2(radius, coneHeight) : Math.PI / 2;
				double y = Math.Sin(coneAngle);
				double xz = Math.Cos(coneAngle);
				normals[i] = new DiagramVector(2 * x * xz / width, y, 2 * z * xz / depth);
				normals[i].Normalize();
			}
			return normals;
		}
		void CreateTruncatedCone(YPlaneRectangle bottomRectBase, YPlaneRectangle topRectBase, int segmentsCount) {
			this.height = topRectBase.Center.Y - bottomRectBase.Center.Y;
			this.width = bottomRectBase.Height;
			this.depth = bottomRectBase.Width;
			this.location = bottomRectBase.Location;
			this.bottomBase = CalcBase(bottomRectBase, segmentsCount);
			this.topBase = CalcBase(topRectBase, segmentsCount);
			DiagramVector[] normals = CalcNormalsForTruncatedCone(bottomRectBase.Center);
			for (int i = 0; i < bottomBase.Vertices.Length; i++) {
				int nextIndex = i > bottomBase.Vertices.Length - 2 ? 0 : i + 1;
				Vertex v1 = new Vertex(bottomBase.Vertices[nextIndex].Point, normals[nextIndex]);
				Vertex v2 = new Vertex(bottomBase.Vertices[i].Point, normals[i]);
				Vertex v3 = new Vertex(topBase.Vertices[i].Point, normals[i]);
				Vertex v4 = new Vertex(topBase.Vertices[nextIndex].Point, normals[nextIndex]);
				PlanePolygon plane = new PlaneQuadrangle(v1, v2, v3, v4);
				plane.Normal = MathUtils.CalcNormal(plane);
				plane.SameNormals = !IsSmoothing;
				laterals.Add(plane);
			}
		}
		void CreateSimpleCone(YPlaneRectangle bottomRectBase, double height, int segmentsCount) {
			this.height = height;
			this.width = bottomRectBase.Height;
			this.depth = bottomRectBase.Width;
			this.location = bottomRectBase.Location;
			this.bottomBase = CalcBase(bottomRectBase, segmentsCount);
			this.topBase = null;
			DiagramPoint coneVertex = DiagramPoint.Offset(bottomRectBase.Center, 0, height, 0);
			DiagramVector[] normals = CalcNormalsForSimpleCone(coneVertex);
			for (int i = 0; i < bottomBase.Vertices.Length; i++) {
				int nextIndex = i > bottomBase.Vertices.Length - 2 ? 0 : i + 1;
				Vertex v1 = new Vertex(bottomBase.Vertices[nextIndex], normals[nextIndex]);
				Vertex v2 = new Vertex(bottomBase.Vertices[i], normals[i]);
				DiagramVector n = normals[nextIndex] + normals[i];
				n.Normalize();
				Vertex v3 = new Vertex(coneVertex, n);
				PlanePolygon plane = new PlaneTriangle(v1, v2, v3);
				plane.Normal = MathUtils.CalcNormal(plane);
				plane.SameNormals = !IsSmoothing;
				laterals.Add(plane);
			}
		}		
	}
}
