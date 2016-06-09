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
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Utils;
namespace DevExpress.Xpf.Charts.Native {
	public class SliceInfo {
		Model3D model;
		Point3D maxBottom, maxTop;
		public Model3D Model { get { return model; } }
		public Point3D MaxBottom { get { return maxBottom; } }
		public Point3D MaxTop { get { return maxTop; } }
		public SliceInfo(Model3D model, Point3D maxBottom, Point3D maxTop) {
			this.model = model;
			this.maxBottom = maxBottom;
			this.maxTop = maxTop;
		}
	}
	public sealed class Pie3DCalculator {
		const int minPieSegmentation = 10;
		const double maxHoleRadiusRatio = 0.993;
		static int NormPieSegmentation(int segmentation) {
			return segmentation < minPieSegmentation ? minPieSegmentation : segmentation;
		}
		static List<double> CalcAngles(double minAngle, double maxAngle, double pieSegmentation) {
			double step = 360 / pieSegmentation;
			List<double> angles = new List<double>();
			for (double currentAngle = minAngle; currentAngle <= maxAngle; currentAngle += step)
				angles.Add(currentAngle);
			if (angles.Count > 0 && angles[angles.Count - 1] != maxAngle)
				angles.Add(maxAngle);
			return angles;
		}
		static void CalcNormals(MeshGeometry3D section) {
			for (int i = 0; i < section.Positions.Count; i++) {
				Point3D p1 = section.Positions[i == 0 ? section.Positions.Count - 1 : i - 1];
				Point3D p2 = section.Positions[i];
				Point3D p3 = section.Positions[i == section.Positions.Count - 1 ? 0 : i + 1];
				Point3D p0 = MathUtils.Offset(p2, 0, 0, 1);
				Vector3D n1 = MathUtils.CalcNormal(p1, p2, p0);
				Vector3D n2 = MathUtils.CalcNormal(p2, p3, p0);
				Vector3D n = n1 + n2;
				MathUtils.Normalize(ref n);
				section.Normals.Add(n);
			}
		}
		static MeshGeometry3D CalcSection(MeshGeometry3D section, double angle) {
			MeshGeometry3D newSection = section.CloneCurrentValue();
			Matrix3D matrix = CalcMatrix(angle);
			Graphics3DUtils.Transform(newSection, matrix);
			return newSection;
		}
		static List<MeshGeometry3D> CalcSections(IList<double> samplingAngles, MeshGeometry3D section) {
			List<MeshGeometry3D> sections = new List<MeshGeometry3D>();
			foreach (double angle in samplingAngles)
				sections.Add(CalcSection(section, angle));
			return sections;
		}
		static MeshGeometry3D CalcSurface(IList<MeshGeometry3D> sections) {
			MeshGeometry3D surface = new MeshGeometry3D();
			int pointsCount = sections[0].Positions.Count;
			for (int k = 0; k < sections.Count; k++) {
				for (int i = 0; i <= pointsCount; i++) {
					if (i < pointsCount) {
						surface.Positions.Add(sections[k].Positions[i]);
						surface.Normals.Add(sections[k].Normals[i]);
					}
					if (k > 0 && i > 0) {
						int i1 = i - 1 + (k - 1) * pointsCount;
						int i2 = (i == pointsCount ? 0 : i) + (k - 1) * pointsCount;
						int i3 = (i == pointsCount ? 0 : i) + k * pointsCount;
						int i4 = i - 1 + k * pointsCount;
						surface.TriangleIndices.Add(i1);
						surface.TriangleIndices.Add(i3);
						surface.TriangleIndices.Add(i2);
						surface.TriangleIndices.Add(i1);
						surface.TriangleIndices.Add(i4);
						surface.TriangleIndices.Add(i3);
					}
				}
			}
			return surface;
		}
		static MeshGeometry3D CalcSidewalls(IList<MeshGeometry3D> sections, bool firstSidwall, bool lastSidewall) {
			if (!firstSidwall && !lastSidewall)
				return null;
			MeshGeometry3D sidewalls = new MeshGeometry3D();
			if (lastSidewall) {
				Graphics3DUtils.AddPolygon(sidewalls, sections[sections.Count - 1].Positions);
				Graphics3DUtils.InvertTriangles(sidewalls.TriangleIndices);
			}
			if (firstSidwall)
				Graphics3DUtils.AddPolygon(sidewalls, sections[0].Positions);
			return sidewalls;
		}
		static Model3DGroup CalcModel(IList<MeshGeometry3D> sections, bool firstSidwall, bool lastSidewall, Material material) {
			Model3DGroup group = new Model3DGroup();
			MeshGeometry3D surfaceGeometry = CalcSurface(sections);
			if (surfaceGeometry != null) {
				GeometryModel3D surface = new GeometryModel3D();
				surface.Geometry = surfaceGeometry;
				surface.Material = material;
				group.Children.Add(surface);
			}
			MeshGeometry3D sidewallsGeometry = CalcSidewalls(sections, firstSidwall, lastSidewall);
			if (sidewallsGeometry != null) {
				GeometryModel3D sidewalls = new GeometryModel3D();
				sidewalls.Geometry = sidewallsGeometry;
				sidewalls.Material = material;
				group.Children.Add(sidewalls);
			}
			return group;
		}
		static Pie3DLabelLayout CalcLabelLayout(SimpleDiagram3DDomain domain, SeriesLabel label, MeshGeometry3D section, double startAngle, double finishAngle) {
			Rect3D bounds = section.Bounds;
			Point3D midBottom = new Point3D(bounds.Location.X + bounds.SizeX / 2, bounds.Location.Y, 0);
			Point3D midTop = new Point3D(bounds.Location.X + bounds.SizeX / 2, bounds.Location.Y + bounds.SizeY, 0);
			Matrix3D matrix = CalcMatrix(startAngle + (finishAngle - startAngle) / 2);
			return new Pie3DLabelLayout(label, GetMaxTop(section, startAngle, finishAngle), GetMaxBottom(section, startAngle, finishAngle), 
				matrix.Transform(midTop), matrix.Transform(midBottom), matrix.Transform(new Vector3D(1, 0, 0)));
		}
		internal static Point3D GetMaxBottom(MeshGeometry3D section, double startAngle, double finishAngle) {
			Rect3D bounds = section.Bounds;
			Point3D maxBottom = new Point3D(bounds.Location.X + bounds.SizeX, bounds.Location.Y + bounds.SizeY / 2, 0);
			foreach (Point3D point in section.Positions) {
				if (ComparingUtils.CompareDoubles(point.X, maxBottom.X, 1e-004) == 0)
					if (point.Y < maxBottom.Y)
						maxBottom = point;
			}
			Matrix3D matrix = CalcMatrix(startAngle + (finishAngle - startAngle) / 2);
			return matrix.Transform(maxBottom);
		}
		internal static Point3D GetMaxTop(MeshGeometry3D section, double startAngle, double finishAngle) {
			Rect3D bounds = section.Bounds;
			Point3D maxBottom = new Point3D(bounds.Location.X + bounds.SizeX, bounds.Location.Y + bounds.SizeY / 2, 0);
			Point3D maxTop = maxBottom;
			foreach (Point3D point in section.Positions) {
				if (ComparingUtils.CompareDoubles(point.X, maxBottom.X, 1e-004) == 0)
					if (point.Y >= maxBottom.Y && point.Y > maxTop.Y)
						maxTop = point;
			}
			Matrix3D matrix = CalcMatrix(startAngle + (finishAngle - startAngle) / 2);
			return matrix.Transform(maxTop);
		}
		internal static GeometryModel3D CreateSectionModel(Pie3DModel model) {
			if (model == null)
				return null;
			Polyline poly = model.GetPolyline();
			if (poly == null)
				return null;
			if (poly.Points == null || poly.Points.Count < 3)
				return null;
			MeshGeometry3D mesh = new MeshGeometry3D();
			foreach (Point point in poly.Points) {
				Point3D point3D = new Point3D(point.X, point.Y, 0);
				mesh.Positions.Add(point3D);
			}
			double area = MathUtils.CalcArea(poly.Points);
			if (area < 0)
				Graphics3DUtils.RevertPositions(mesh.Positions);
			GeometryModel3D sectionModel = new GeometryModel3D();
			MaterialGroup group = new MaterialGroup();
			group.Children.Add(new DiffuseMaterial());
			group.Children.Add(new SpecularMaterial(new SolidColorBrush(Color.FromArgb(255, 229, 229, 229)), 50));
			sectionModel.Material = group;
			sectionModel.Geometry = mesh;
			return sectionModel;
		}
		internal static void Prepare(MeshGeometry3D section, double heightRatio, double radius, double holeRadiusRatio) {
			double modelWidth = radius * (1 - NormalizeHoleRadiusRatio(holeRadiusRatio));
			double scaleRatio = modelWidth / section.Bounds.SizeX;
			Matrix3D matrix = Graphics3DUtils.CalcAlignByZeroMatrix(section.Bounds);
			matrix.Scale(new Vector3D(scaleRatio, heightRatio * scaleRatio, 1));
			Matrix3D offsetMartrix = new Matrix3D();
			offsetMartrix.OffsetX = radius - modelWidth / 2;
			matrix.Append(offsetMartrix);
			Graphics3DUtils.Transform(section, matrix);
		}
		internal static Matrix3D CalcMatrix(double angle) {
			Matrix3D matrix = new Matrix3D();
			matrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), angle));
			return matrix;
		}
		public static double NormalizeHoleRadiusRatio(double holeRadiusRatio) {
			return holeRadiusRatio > maxHoleRadiusRatio ? maxHoleRadiusRatio : holeRadiusRatio;
		}
		public static Model3D CalculateSlice(SimpleDiagram3DDomain domain, SeriesLabel label, Pie3DModel model, double radius, double holeRadiusRatio, double heightRatio, double startAngle, double finishAngle, bool startSidewall, bool finishSidewall, int pieSegmentation, SolidColorBrush brush, out Pie3DLabelLayout labelLayout) {
			labelLayout = null;
			pieSegmentation = NormPieSegmentation(pieSegmentation);
			double minAngle = Math.Min(startAngle, finishAngle);
			double maxAngle = Math.Max(startAngle, finishAngle);
			List<double> angles = CalcAngles(minAngle, maxAngle, pieSegmentation);
			GeometryModel3D sectionModel = CreateSectionModel(model);
			if (sectionModel == null)
				return null;
			MeshGeometry3D section = (MeshGeometry3D)sectionModel.Geometry;
			Prepare(section, heightRatio, radius, holeRadiusRatio);
			CalcNormals(section);
			List<MeshGeometry3D> sections = CalcSections(angles, section);
			if (startAngle > finishAngle) {
				bool k = startSidewall;
				startSidewall = finishSidewall;
				finishSidewall = k;
			}
			Graphics3DUtils.SetMaterialBrush(sectionModel.Material, brush, true);
			labelLayout = CalcLabelLayout(domain, label, section, startAngle, finishAngle);
			return CalcModel(sections, startSidewall, finishSidewall, sectionModel.Material);
		}
	}
}
