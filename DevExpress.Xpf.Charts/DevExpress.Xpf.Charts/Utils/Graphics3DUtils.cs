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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows;
namespace DevExpress.Xpf.Charts.Native {
	public sealed class Graphics3DUtils {
		static void UpdateRange(ref double min, ref double max, double value) {
			if (value < min)
				min = value;
			else if (value > max)
				max = value;
		}
		static void TransformPoints(Point3DCollection positions, Matrix3D matrix) {
			if (positions != null)
				for (int i = 0; i < positions.Count; i++)
					positions[i] = matrix.Transform(positions[i]);
		}
		static void TransformNormals(Vector3DCollection normals, Matrix3D matrix) {
			if (normals != null) {
				Matrix3D normalsMatrix;
				if (matrix.HasInverse) {
					matrix.Invert();
					normalsMatrix = MathUtils.Transpose(matrix);
				}
				else
					normalsMatrix = matrix;
				for (int i = 0; i < normals.Count; i++)
					normals[i] = normalsMatrix.Transform(normals[i]);
			}
		}
		static void Triangulate(MeshGeometry3D geometry, int startIndex) {
			if (geometry != null) {
				Point3DCollection positions = geometry.Positions;
				Int32Collection triangleIndices = geometry.TriangleIndices;
				if (positions != null && triangleIndices != null && positions.Count - startIndex >= 3)
					for (int i = startIndex + 1; i < positions.Count - 1; i++) {
						triangleIndices.Add(startIndex);
						triangleIndices.Add(i);
						triangleIndices.Add(i + 1);
					}
			}
		}
		static void SetBrush(Material material, Brush targetBrush, DependencyProperty brushProperty, MaterialBrushCache cache) {
			Brush newBrush = targetBrush;
			Brush oldBrush = material.GetValue(brushProperty) as Brush;
			if (cache != null && newBrush != oldBrush) {
				Brush cachedBrush;
				cache.TryGetValue(material, out cachedBrush);
				if (cachedBrush != null) {
					newBrush = cachedBrush;
					cache.Remove(material);
				}
				else
					cache.Add(material, oldBrush);
			}
			material.SetValue(brushProperty, newBrush);
		}
		static void SetSingleMaterialBrush(Material material, Brush brush, bool diffuseMaterialOnly, MaterialBrushCache cache) {
			if (material is DiffuseMaterial)
				SetBrush(material, brush, DiffuseMaterial.BrushProperty, cache);
			else if (!diffuseMaterialOnly) {
				if (material is EmissiveMaterial)
					SetBrush(material, brush, EmissiveMaterial.BrushProperty, cache);
				if (material is SpecularMaterial)
					SetBrush(material, brush, SpecularMaterial.BrushProperty, cache);
			}
		}
		public static void Transform(MeshGeometry3D geometry, Matrix3D matrix) {
			if (geometry != null && !matrix.IsIdentity) {
				TransformPoints(geometry.Positions, matrix);
				TransformNormals(geometry.Normals, matrix);
			}
		}
		public static void CacheBrush(Brush brush) {
			RenderOptions.SetCacheInvalidationThresholdMinimum(brush, 0.1);
			RenderOptions.SetCacheInvalidationThresholdMaximum(brush, 10);
			RenderOptions.SetCachingHint(brush, CachingHint.Cache);
		}
		public static Rect3D CalcBounds(IList<Point3D> points, bool alignedByX, bool alignedByY, bool alignedByZ) {
			if (points == null || points.Count == 0)
				return Rect3D.Empty;
			double xMin = points[0].X;
			double xMax = points[0].X;
			double yMin = points[0].Y;
			double yMax = points[0].Y;
			double zMin = points[0].Z;
			double zMax = points[0].Z;
			foreach (Point3D point in points) {
				UpdateRange(ref xMin, ref xMax, point.X);
				UpdateRange(ref yMin, ref yMax, point.Y);
				UpdateRange(ref zMin, ref zMax, point.Z);
			}
			if (alignedByX) {
				xMax = Math.Max(Math.Abs(xMin), Math.Abs(xMax));
				xMin = -xMax;
			}
			if (alignedByY) {
				yMax = Math.Max(Math.Abs(yMin), Math.Abs(yMax));
				yMin = -yMax;
			}
			if (alignedByZ) {
				zMax = Math.Max(Math.Abs(zMin), Math.Abs(zMax));
				zMin = -zMax;
			}
			return new Rect3D(xMin, yMin, zMin, xMax - xMin, yMax - yMin, zMax - zMin);
		}
		public static void InvertTriangles(Int32Collection triangleIndices) {
			if (triangleIndices != null)
				for (int i = 0; i <= triangleIndices.Count - 3; i += 3) {
					int index = triangleIndices[i + 1];
					triangleIndices[i + 1] = triangleIndices[i + 2];
					triangleIndices[i + 2] = index;
				}
		}
		public static void RevertPositions(Point3DCollection positions) {
			if (positions != null && positions.Count >= 2)
				for (int i = 0; i < positions.Count / 2; i++) {
					Point3D position = positions[i];
					positions[i] = positions[positions.Count - 1 - i];
					positions[positions.Count - 1 - i] = position;
				}
		}
		public static void AddPolygon(MeshGeometry3D geometry, IList<Point3D> points) {
			if (points != null && points.Count >= 3) {
				int stratIndex = geometry.Positions.Count;
				foreach (Point3D point in points)
					geometry.Positions.Add(point);
				Triangulate(geometry, stratIndex);
			}
		}
		public static void AddPolygon(MeshGeometry3D geometry, IList<Vertex> vertices) {
			if (vertices != null) {
				List<Point3D> points = new List<Point3D>(vertices.Count);
				foreach (Vertex vertex in vertices)
					points.Add(vertex.Position);
				AddPolygon(geometry, points);
			}
		}
		public static void AddPolygon(MeshGeometry3D geometry, MeshGeometry3D polygon) {
			int startIndex = geometry.Positions.Count;
			for (int i = 0; i < polygon.Positions.Count; i++)
				geometry.Positions.Add(polygon.Positions[i]);
			for (int i = 0; i < polygon.TriangleIndices.Count; i++)
				geometry.TriangleIndices.Add(polygon.TriangleIndices[i] + startIndex);
		}
		public static void SetMaterialBrush(Material material, Brush brush, bool diffuseMaterialOnly) {
			SetMaterialBrush(material, brush, null, diffuseMaterialOnly);
		}
		public static void SetMaterialBrush(Material material, Brush brush, MaterialBrushCache cache, bool diffuseMaterialOnly) {
			MaterialGroup group = material as MaterialGroup;
			if (group == null)
				SetSingleMaterialBrush(material, brush, diffuseMaterialOnly, cache);
			else
				foreach (Material child in group.Children)
					SetMaterialBrush(child, brush, cache, diffuseMaterialOnly);
		}
		public static Matrix3D CalcAlignByZeroMatrix(Rect3D bounds) {
			Point3D center = MathUtils.CalcCenter(bounds);
			Matrix3D matrix = new Matrix3D();
			matrix.Translate(new Vector3D(-center.X, -center.Y, -center.Z));
			return matrix;
		}
		public static Point3D[] GetRefPoints(Rect3D bounds) {
			return new Point3D[] { bounds.Location, 
				new Point3D(bounds.Location.X + bounds.SizeX, bounds.Location.Y + bounds.SizeY, bounds.Location.Z + bounds.SizeZ) };
		}
		public static Model3D CreateLineModel(Point3D p1, Point3D p2, double thickness, Material material) {
			Vector3D vector = p2 - p1;
			double length = vector.Length;
			if (length == 0 || thickness <= 0)
				return null;
			Transform3DGroup group = new Transform3DGroup();
			Vector3D vector1 = new Vector3D(0, length, 0);
			double angle1 = Vector3D.AngleBetween(vector1, vector);
			if (angle1 > 0) {
				Vector3D axisZ = new Vector3D(0, 0, 1);
				RotateTransform3D rotate1 = new RotateTransform3D(new AxisAngleRotation3D(axisZ, angle1));
				group.Children.Add(rotate1);
				if (angle1 < 180) {
					Vector3D vector2 = rotate1.Transform(vector1);
					Vector3D vectorXZ = new Vector3D(vector.X, 0, vector.Z);
					Vector3D vector2XZ = new Vector3D(vector2.X, 0, vector2.Z);
					double angle2 = Vector3D.AngleBetween(vectorXZ, vector2XZ);
					Vector3D normal = Vector3D.CrossProduct(vector2XZ, vectorXZ);
					angle2 = normal.Y > 0 ? angle2 : -angle2;
					Vector3D axisY = new Vector3D(0, 1, 0);
					RotateTransform3D rotate2 = new RotateTransform3D(new AxisAngleRotation3D(axisY, angle2));
					group.Children.Add(rotate2);
				}
			}
			TranslateTransform3D translate = new TranslateTransform3D((Vector3D)p1);
			group.Children.Add(translate);
			Point3D p = new Point3D();
			p.Offset(-thickness / 2, 0, -thickness / 2);
			Box box = new Box(p, thickness, length, thickness);
			MeshGeometry3D geometry = box.GetGeometry();
			Model3D model = new GeometryModel3D(geometry, material);
			model.Transform = group;
			return model;
		}
		public static void HandleModel(Model3D model, ModelHandler handler) {
			GeometryModel3D geometryModel = model as GeometryModel3D;
			if (geometryModel == null) {
				Model3DGroup group = model as Model3DGroup;
				if (group != null) 
				foreach (Model3D child in group.Children)
					HandleModel(child, handler);
			}
			else
				handler(geometryModel);
		}
		public delegate void ModelHandler(GeometryModel3D model);
	}
	public class MaterialBrushCache : Dictionary<Material, Brush> {
	}
}
