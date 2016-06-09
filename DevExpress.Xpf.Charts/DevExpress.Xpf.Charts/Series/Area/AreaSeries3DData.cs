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
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class AreaSeries3DData : Series3DData {
		const int pointPrecision = 6;
		static int CompareByY(GRealPoint2D point1, GRealPoint2D point2) {
			return (point1.Y.CompareTo(point2.Y));
		}
		static bool IsLeftTriangle(GPolygon2D polygon) {
			return polygon.Vertices[0].X == polygon.Vertices[1].X && polygon.Vertices[0].X < polygon.Vertices[2].X ||
				polygon.Vertices[0].X == polygon.Vertices[2].X && polygon.Vertices[0].X < polygon.Vertices[1].X ||
				polygon.Vertices[1].X == polygon.Vertices[2].X && polygon.Vertices[1].X < polygon.Vertices[0].X;
		}
		static bool IsRightTriangle(GPolygon2D polygon) {
			return polygon.Vertices[0].X == polygon.Vertices[1].X && polygon.Vertices[0].X > polygon.Vertices[2].X ||
				polygon.Vertices[0].X == polygon.Vertices[2].X && polygon.Vertices[0].X > polygon.Vertices[1].X ||
				polygon.Vertices[1].X == polygon.Vertices[2].X && polygon.Vertices[1].X > polygon.Vertices[0].X;
		}
		static void AddTriangle(MeshGeometry3D geometry, Point3D p1, Point3D p2, Point3D p3) {
			geometry.TriangleIndices.Add(GetPointIndex(geometry, p1));
			geometry.TriangleIndices.Add(GetPointIndex(geometry, p2));
			geometry.TriangleIndices.Add(GetPointIndex(geometry, p3));
		}
		static void OffsetByZ(MeshGeometry3D geometry, double offset) {
			for (int i = 0; i < geometry.Positions.Count; i++)
				geometry.Positions[i] = new Point3D(geometry.Positions[i].X, geometry.Positions[i].Y, geometry.Positions[i].Z + offset);
		}
		static void Invert(MeshGeometry3D geometry) {
			Int32Collection collection = new Int32Collection();
			foreach (int index in geometry.TriangleIndices)
				collection.Insert(0, index);
			geometry.TriangleIndices = collection;
		}
		static void FillPoints(GPolygon2D polygon, out List<GRealPoint2D> leftPoints, out List<GRealPoint2D> rightPoints) {
			leftPoints = new List<GRealPoint2D>();
			rightPoints = new List<GRealPoint2D>();
			double maxX = double.NegativeInfinity;
			for (int i = 0; i < polygon.Vertices.Length; i++)
				if (polygon.Vertices[i].X > maxX)
					maxX = polygon.Vertices[i].X;
			for (int i = 0; i < polygon.Vertices.Length; i++)
				if (polygon.Vertices[i].X == maxX)
					rightPoints.Add(polygon.Vertices[i]);
				else
					leftPoints.Add(polygon.Vertices[i]);
			leftPoints.Sort(CompareByY);
			rightPoints.Sort(CompareByY);
		}
		static MeshGeometry3D GetQuadrangle(Point3D point1, Point3D point2, double zOffset, double maxLengthInPixels) {
			Point3D p1 = new Point3D(point1.X, point1.Y, point1.Z - zOffset);
			Point3D p2 = new Point3D(point1.X, point1.Y, point1.Z + zOffset);
			Point3D p3 = new Point3D(point2.X, point2.Y, point2.Z + zOffset);
			Point3D p4 = new Point3D(point2.X, point2.Y, point2.Z - zOffset);
			MeshGeometry3D qundrangle = new MeshGeometry3D();
			SplitAndAddTriangle(qundrangle, p1, p2, p3, maxLengthInPixels);
			SplitAndAddTriangle(qundrangle, p3, p4, p1, maxLengthInPixels);
			return qundrangle;
		}
		static List<GRealPoint2D> GetSplitPoints(GPolygon2D polygon, List<GRealPoint2D> points) {
			List<GRealPoint2D> splitPoints = new List<GRealPoint2D>();
			for (int i = 0; i < polygon.Vertices.Length; i++)
				if (polygon.Vertices[i].X == points[0].X && polygon.Vertices[i].Y < points[points.Count - 1].Y && polygon.Vertices[i].Y > points[0].Y)
					splitPoints.Add(polygon.Vertices[i]);
			splitPoints.Sort(CompareByY);
			splitPoints.Reverse();
			return splitPoints;
		}
		static List<GPolygon2D> SplitLeftSide(GPolygon2D polygon, GPolygon2D previousPolygon) {
			List<GPolygon2D> polygons = new List<GPolygon2D>();
			List<GRealPoint2D> leftPoints, rightPoints;
			FillPoints(polygon, out leftPoints, out rightPoints);
			if (leftPoints.Count != 0 && rightPoints.Count != 0) {
				List<GRealPoint2D> splitPoints = GetSplitPoints(previousPolygon, leftPoints);
				if (splitPoints.Count > 0 && splitPoints.Count < 3) {
					polygons.Add(new GPolygon2D(new GRealPoint2D[] { splitPoints[0], rightPoints[rightPoints.Count - 1], leftPoints[leftPoints.Count - 1] }));
					if (splitPoints.Count == 2) {
						polygons.Add(new GPolygon2D(new GRealPoint2D[] { splitPoints[splitPoints.Count - 1], leftPoints[0], rightPoints[0] }));
						polygons.Add(new GPolygon2D(rightPoints[0] == rightPoints[rightPoints.Count - 1] ?
							new GRealPoint2D[] { splitPoints[splitPoints.Count - 1], rightPoints[0], splitPoints[0] } :
							new GRealPoint2D[] { splitPoints[splitPoints.Count - 1], rightPoints[0], rightPoints[rightPoints.Count - 1], splitPoints[0] }));
					}
					else
						polygons.Add(new GPolygon2D(rightPoints[0] == rightPoints[rightPoints.Count - 1] ?
							new GRealPoint2D[] { splitPoints[0], leftPoints[0], rightPoints[0] } :
							new GRealPoint2D[] { splitPoints[0], leftPoints[0], rightPoints[0], rightPoints[rightPoints.Count - 1] }));
				}
				else
					polygons.Add(polygon);
			}
			return polygons;
		}
		static List<GPolygon2D> SplitRightSide(GPolygon2D polygon, GPolygon2D nextPolygon) {
			List<GPolygon2D> polygons = new List<GPolygon2D>();
			List<GRealPoint2D> leftPoints, rightPoints;
			FillPoints(polygon, out leftPoints, out rightPoints);
			if (leftPoints.Count != 0 && rightPoints.Count != 0) {
				List<GRealPoint2D> splitPoints = GetSplitPoints(nextPolygon, rightPoints);
				if (splitPoints.Count > 0 && splitPoints.Count < 3) {
					polygons.Add(new GPolygon2D(new GRealPoint2D[] { splitPoints[0], rightPoints[rightPoints.Count - 1], leftPoints[leftPoints.Count - 1] }));
					if (splitPoints.Count == 2) {
						polygons.Add(new GPolygon2D(new GRealPoint2D[] { splitPoints[splitPoints.Count - 1], leftPoints[0], rightPoints[0] }));
						polygons.Add(new GPolygon2D(leftPoints[0] == leftPoints[leftPoints.Count - 1] ?
							new GRealPoint2D[] { splitPoints[splitPoints.Count - 1], splitPoints[0], leftPoints[0] } :
							new GRealPoint2D[] { splitPoints[splitPoints.Count - 1], splitPoints[0], leftPoints[leftPoints.Count - 1], leftPoints[0] }));
					}
					else 
						polygons.Add(new GPolygon2D(leftPoints[0] == leftPoints[leftPoints.Count - 1] ?
							new GRealPoint2D[] { splitPoints[0], leftPoints[0], rightPoints[0] } :
							new GRealPoint2D[] { splitPoints[0], leftPoints[leftPoints.Count - 1], leftPoints[0], rightPoints[0] }));
				}
				else
					polygons.Add(polygon);
			}
			return polygons;
		}
		internal static int GetPointIndex(MeshGeometry3D geometry, Point3D point) {
			point = new Point3D(Math.Round(point.X, pointPrecision), Math.Round(point.Y, pointPrecision), Math.Round(point.Z, pointPrecision));
			if (!geometry.Positions.Contains(point))
				geometry.Positions.Add(point);
			return geometry.Positions.IndexOf(point);
		}
		internal static void SplitAndAddTriangle(MeshGeometry3D geometry, Point3D p1, Point3D p2, Point3D p3, double maxLengthInPixels) {
			double length12 = MathUtils.CalcDistance(p1, p2);
			double length23 = MathUtils.CalcDistance(p2, p3);
			double length31 = MathUtils.CalcDistance(p3, p1);
			if (length12 >= length23 && length12 >= length31 && length12 > maxLengthInPixels) {
				Point3D point = new Point3D((p2.X - p1.X) / 2 + p1.X, (p2.Y - p1.Y) / 2 + p1.Y, (p2.Z - p1.Z) / 2 + p1.Z);
				SplitAndAddTriangle(geometry, p1, point, p3, maxLengthInPixels);
				SplitAndAddTriangle(geometry, p3, point, p2, maxLengthInPixels);
			}
			else if (length23 >= length12 && length23 >= length31 && length23 > maxLengthInPixels) {
				Point3D point = new Point3D((p3.X - p2.X) / 2 + p2.X, (p3.Y - p2.Y) / 2 + p2.Y, (p3.Z - p2.Z) / 2 + p2.Z);
				SplitAndAddTriangle(geometry, p1, point, p3, maxLengthInPixels);
				SplitAndAddTriangle(geometry, p1, p2, point, maxLengthInPixels);
			}
			else if (length31 >= length12 && length31 >= length23 && length31 > maxLengthInPixels) {
				Point3D point = new Point3D((p1.X - p3.X) / 2 + p3.X, (p1.Y - p3.Y) / 2 + p3.Y, (p1.Z - p3.Z) / 2 + p3.Z);
				SplitAndAddTriangle(geometry, p1, p2, point, maxLengthInPixels);
				SplitAndAddTriangle(geometry, point, p2, p3, maxLengthInPixels);
			}
			else
				AddTriangle(geometry, p1, p2, p3);
		}
		internal static List<GPolygon2D> SplitPolygon(GPolygon2D polygon, GPolygon2D previousPolygon, GPolygon2D nextPolygon) {
			List<GPolygon2D> polygons = new List<GPolygon2D>();
			if (previousPolygon != null || nextPolygon != null) {
				if (polygon.Vertices.Length == 3) {
					if (previousPolygon != null && IsLeftTriangle(polygon))
						return SplitLeftSide(polygon, previousPolygon);
					else if (nextPolygon != null && IsRightTriangle(polygon))
						return SplitRightSide(polygon, nextPolygon);
				}
				else if (polygon.Vertices.Length == 4) {
					List<GPolygon2D> leftSideSplittedPolygons = previousPolygon != null ? SplitLeftSide(polygon, previousPolygon) :
						new List<GPolygon2D> { polygon };
					for (int i = 0; i < leftSideSplittedPolygons.Count; i++)
						if (i < leftSideSplittedPolygons.Count - 1 || nextPolygon == null)
							polygons.Add(leftSideSplittedPolygons[i]);
						else
							polygons.AddRange(SplitRightSide(leftSideSplittedPolygons[i], nextPolygon));
					return polygons;
				}
			}
			polygons.Add(polygon);
			return polygons;
		}
		MeshGeometry3D faceGeometry;
		MeshGeometry3D sideGeometry;
		AreaSeries3D AreaSeries { get { return (AreaSeries3D)Series; } }
		public MeshGeometry3D FaceGeometry { get { return faceGeometry; } }
		public MeshGeometry3D SideGeometry { get { return sideGeometry; } }
		public AreaSeries3DData(AreaSeries3D series) : base(series) {
		}
		protected override Model3DInfo CreateSeriesPointModelInfo(RefinedPoint refinedPoint, Diagram3DDomain domain) {
			XYDiagram3DDomain xyDomain = (XYDiagram3DDomain)domain;
			double zOffset = xyDomain.GetValueByAxisX(AreaSeries.AreaWidth) / 2;
			Point3D point = GetLabelPoint(xyDomain, refinedPoint);
			Point3D frontPoint = new Point3D(point.X, point.Y, point.Z + zOffset);
			Point3D backPoint = new Point3D(point.X, point.Y, point.Z - zOffset);
			AreaSeries3DLabelLayout labelLayout = AreaSeries.LabelsVisibility ?
				new AreaSeries3DLabelLayout(Series.ActualLabel, GetLabelPoint(xyDomain, refinedPoint), new Vector3D(), zOffset) : null;
			return new XYSeriesModel3DInfo(SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint), null, labelLayout);
		}
		protected virtual Point3D GetLabelPoint(XYDiagram3DDomain domain, RefinedPoint refinedPoint) {
			return GetValuePoint(refinedPoint, domain);
		}
		protected virtual Point3D GetValuePoint(RefinedPoint refinedPoint, XYDiagram3DDomain domain) {
			return domain.GetDiagramPoint(AreaSeries, refinedPoint.Argument, ((IXYPoint)refinedPoint).Value);
		}
		protected virtual Point3D GetZeroPoint(RefinedPoint refinedPoint, XYDiagram3DDomain domain) {
			return domain.GetDiagramPoint(AreaSeries, refinedPoint.Argument, 0);
		}
		protected internal override Point3D CalculateToolTipPoint(SeriesPointCache pointCache, Diagram3DDomain domain) {
			RefinedPoint refinedPoint = pointCache.RefinedPoint;
			XYDiagram3DDomain xyDomain = domain as XYDiagram3DDomain;
			return GetLabelPoint(xyDomain, refinedPoint);
		}
		public override void CreateModel(Diagram3DDomain domain, Model3DGroup modelHolder, IRefinedSeries refinedSeries) {
			base.CreateModel(domain, modelHolder, refinedSeries);
			XYDiagram3DDomain xyDomain = domain as XYDiagram3DDomain;
			GeometryCalculator geometryCalculator = new GeometryCalculator();
			IList<IGeometryStrip> strips = geometryCalculator.CreateStrips(refinedSeries);
			IInteractiveElement interative = Series as IInteractiveElement;
			if (xyDomain != null && strips.Count > 0) {
				Material material = AreaSeries.ActualMaterial;
				Graphics3DUtils.SetMaterialBrush(material, Series.MixColor(Series.Cache.DrawOptions.ActualColor), true);
				double epsilon = Math.Min(((IAxisData)xyDomain.Diagram.ActualAxisX).VisualRange.Delta, ((IAxisData)xyDomain.Diagram.ActualAxisY).VisualRange.Delta) * 0.01;
				double zOffset = xyDomain.GetValueByAxisX(AreaSeries.AreaWidth) / 2;
				faceGeometry = new MeshGeometry3D();
				sideGeometry = new MeshGeometry3D();
				List<GRealPoint2D> points = new List<GRealPoint2D>();
				foreach (RangeStrip strip in strips) {
					points.AddRange(strip.TopStrip);
					points.AddRange(strip.BottomStrip);
				}
				double minX = points[0].X, maxX = points[0].X;
				double minY = points[0].Y, maxY = points[0].Y;
				for (int i = 1; i < points.Count; i++) {
					double x = points[i].X;
					double y = points[i].Y;
					if (x < minX)
						minX = x;
					else if (x > maxX)
						maxX = x;
					if (y < minY)
						minY = y;
					else if (y > maxY)
						maxY = y;
				}
				double axisXRangeFactor = ((IAxisData)xyDomain.Diagram.ActualAxisX).VisualRange.Delta / (maxX - minX);
				double axisYRangeFactor = ((IAxisData)xyDomain.Diagram.ActualAxisY).VisualRange.Delta / (maxY - minY);
				double maxLengthInPixels = XYDiagram3DBox.PredefinedWidth * 0.1 * Math.Max(1, 1 / (axisXRangeFactor * axisYRangeFactor));
				foreach (RangeStrip strip in strips) {
					RangeStripTriangulationResult triangulationResult = RangeStripTriangulation.Triangulate(strip, epsilon);
					GPolygon2D previousPolygon = null;
					for (int i = 0; i < triangulationResult.Polygons.Count; i++) {
						GPolygon2D nextPolygon = i < triangulationResult.Polygons.Count - 1 ? triangulationResult.Polygons[i + 1] : null;
						List<GPolygon2D> polygons = SplitPolygon(triangulationResult.Polygons[i], previousPolygon, nextPolygon);
						foreach (GPolygon2D polygon in polygons) {
							if (polygon.Vertices.Length > 2) {
								Point3D p1 = xyDomain.GetDiagramPoint(AreaSeries, polygon.Vertices[0].X, polygon.Vertices[0].Y);
								Point3D p2 = xyDomain.GetDiagramPoint(AreaSeries, polygon.Vertices[1].X, polygon.Vertices[1].Y);
								for (int j = 2; j < polygon.Vertices.Length; j++) {
									Point3D p3 = xyDomain.GetDiagramPoint(AreaSeries, polygon.Vertices[j].X, polygon.Vertices[j].Y);
									SplitAndAddTriangle(faceGeometry, p1, p2, p3, maxLengthInPixels);
									p2 = p3;
								}
							}
						}
						previousPolygon = triangulationResult.Polygons[i];
					}
					LineStrip border = triangulationResult.BorderStrip;
					if (border.Count > 1) {
						Point3D firstPoint = xyDomain.GetDiagramPoint(AreaSeries, border[0].X, border[0].Y);
						Point3D previousPoint = firstPoint;
						for (int i = 1; i < border.Count; i++) {
							Point3D point = xyDomain.GetDiagramPoint(AreaSeries, border[i].X, border[i].Y);
							Graphics3DUtils.AddPolygon(sideGeometry, GetQuadrangle(previousPoint, point, zOffset, maxLengthInPixels));
							previousPoint = point;
						}
						Graphics3DUtils.AddPolygon(sideGeometry, GetQuadrangle(previousPoint, firstPoint, zOffset, maxLengthInPixels));
					}
				}
				GeometryModel3D frontGeometry = new GeometryModel3D();
				frontGeometry.Geometry = faceGeometry;
				frontGeometry.Material = material;
				GeometryModel3D backGeometry = frontGeometry.Clone();
				OffsetByZ((MeshGeometry3D)frontGeometry.Geometry, zOffset);
				OffsetByZ((MeshGeometry3D)backGeometry.Geometry, -zOffset);
				Invert((MeshGeometry3D)backGeometry.Geometry);
				modelHolder.Children.Add(frontGeometry);
				modelHolder.Children.Add(backGeometry);
				GeometryModel3D boundGeometry = new GeometryModel3D();
				boundGeometry.Geometry = sideGeometry;
				boundGeometry.Material = material;
				modelHolder.Children.Add(boundGeometry);
				domain.OnModelAdd(refinedSeries.Series, frontGeometry, backGeometry, boundGeometry);
			}
		}
	}
}
