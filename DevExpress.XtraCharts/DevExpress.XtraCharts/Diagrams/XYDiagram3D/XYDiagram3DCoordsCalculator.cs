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
using DevExpress.Utils;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class XYDiagram3DCoordsCalculator : Diagram3DDomain {
		public static XYDiagram3DCoordsCalculator Create(XYDiagram3D diagram, Rectangle bounds, IList<RefinedSeriesData> seriesData) {
			XYDiagram3DCoordsCalculator domain = new XYDiagram3DCoordsCalculator(diagram, bounds, seriesData);
			domain.CalculateParameters();
			return domain;
		}
		public static PlanePolygon[] ClipPolyhedron(PlaneEquation clippingPlane, PlaneEquation separatingPlane, PlanePolygon[] polyhedron) {
			List<PlanePolygon> positivePolyhedron;
			List<PlanePolygon> negativePolyhedron;
			PlanePolygon positiveIntersection, negativeIntersection;
			IntersectionUtils.CalcPolyhedronIntersection(clippingPlane, polyhedron, Diagram3D.Epsilon, out positivePolyhedron, out negativePolyhedron, out positiveIntersection, out negativeIntersection);
			if (positiveIntersection != null) {
				positiveIntersection.Visible = true;
				PlanePolygon frontPolygon, backPolygon;
				Vertex[] vertices;
				if (separatingPlane != null && Intersection.Yes == IntersectionUtils.CalcPolygonIntersection3D(separatingPlane, positiveIntersection, Diagram3D.Epsilon, out vertices, out frontPolygon, out backPolygon)) {
					if (frontPolygon != null)
						positivePolyhedron.Add(frontPolygon);
					if (backPolygon != null)
						positivePolyhedron.Add(backPolygon);
				}
				else
					positivePolyhedron.Add(positiveIntersection);
			}
			return positivePolyhedron != null ? positivePolyhedron.ToArray() : null;
		}
		const int diagramCorrection = 1;
		const int seriesWeightStep = 10000;
		double seriesBoundsDelta = 2;
		IList<RefinedSeriesData> seriesData;
		List<List<Series>> seriesGroups = new List<List<Series>>();
		IAxisRangeData VisualRangeX { get { return Diagram.AxisX.VisualRangeData; } }
		IAxisRangeData VisualRangeY { get { return Diagram.AxisY.VisualRangeData; } }
		public new XYDiagram3D Diagram { get { return (XYDiagram3D)base.Diagram; } }
		public new double Depth { get { return base.Depth - PlaneDepth * 2; } }
		public double PlaneDepth { get { return Diagram.PlaneDepthFixed; } }
		public Rectangle MappingBounds { get { return Bounds; } }
		public override Rectangle ElementBounds { get { return Bounds; } }
		XYDiagram3DCoordsCalculator(XYDiagram3D diagram, Rectangle bounds, IList<RefinedSeriesData> seriesData)
			: base(diagram, bounds) {
			this.seriesData = seriesData;
		}
		int GetSeriesGroupIndex(Series series) {
			for (int i = 0; i < seriesGroups.Count; i++) {
				if (seriesGroups[i].Contains(series))
					return i;
			}
			throw new ArgumentException("series");
		}
		int GetSeriesIndex(Series series) {
			for (int i = 0; i < seriesData.Count; i++)
				if (Object.ReferenceEquals(seriesData[i].Series, series))
					return i;
			return -1;
		}
		void AddSeriesGroup(List<Series> group) {
			int index = GetSeriesIndex(group[0]);
			int pos;
			for (pos = 0; pos < seriesGroups.Count; pos++)
				if (GetSeriesIndex(seriesGroups[pos][0]) > index)
					break;
			seriesGroups.Insert(pos, group);
		}
		void CalcSeriesGroups() {
			seriesGroups.Clear();
			List<Series> seriesGroup = new List<Series>();
			IRefinedSeries groupSeries = null;
			foreach (RefinedSeriesData data in seriesData) {
				if (GetSeriesIndex(data.Series) < 0)
					continue;
				if (seriesGroup.Count == 0) {
					seriesGroup.Add(data.Series);
					groupSeries = data.RefinedSeries;
				}
				else {
					if (groupSeries.IsSameContainers(data.RefinedSeries))
						seriesGroup.Add(data.Series);
					else {
						AddSeriesGroup(seriesGroup);
						seriesGroup = new List<Series>();
						seriesGroup.Add(data.Series);
						groupSeries = data.RefinedSeries;
					}
				}
			}
			if (seriesGroup.Count > 0)
				AddSeriesGroup(seriesGroup);
		}
		double CalcSeriesDepth() {
			double maxSeriesDepth = double.PositiveInfinity;
			int maxSeriesDepthFixed = 0;
			foreach (RefinedSeriesData data in seriesData) {
				XYDiagram3DSeriesViewBase view = data.Series.View as XYDiagram3DSeriesViewBase;
				if (view != null) {
					double depth = view.GetSeriesDepth();
					if (!Double.IsInfinity(depth) && (Double.IsInfinity(maxSeriesDepth) || depth > maxSeriesDepth))
						maxSeriesDepth = depth;
					int depthFixed = view.GetSeriesDepthFixed();
					if (depthFixed > maxSeriesDepthFixed)
						maxSeriesDepthFixed = depthFixed;
				}
			}
			if (Double.IsInfinity(maxSeriesDepth)) {
				if (maxSeriesDepthFixed > 0)
					return maxSeriesDepthFixed;
				int divider = seriesData.Count;
				if (divider <= 0)
					divider = 1;
				return Bounds.Width * 0.3 / divider;
			}
			else {
				double z1 = AxisCoordCalculator.GetCoord(VisualRangeX, 0.0, Bounds.Width);
				double z2 = AxisCoordCalculator.GetCoord(VisualRangeX, maxSeriesDepth, Bounds.Width);
				return Math.Abs(z2 - z1) + maxSeriesDepthFixed;
			}
		}		
		double CalcSeriesDistance() {
			double z1 = AxisCoordCalculator.GetCoord(VisualRangeX, 0.0, Bounds.Width);
			double z2 = AxisCoordCalculator.GetCoord(VisualRangeX, Diagram.SeriesDistance, Bounds.Width);
			return Math.Abs(z2 - z1) + Diagram.SeriesDistanceFixed;
		}
		double CalcDiagramDepth() {
			double seriesDepth = CalcSeriesDepth();
			if (seriesGroups.Count == 0)
				return seriesDepth;
			double seriesDistance = CalcSeriesDistance();
			return seriesDepth * seriesGroups.Count +
				seriesDistance * (seriesGroups.Count - 1) + Diagram.SeriesIndentFixed * 2;
		}
		double CalcSeriesBoundsDelta() {
			return this.seriesBoundsDelta;
		}
		PlaneEquation GetLeftPlaneEquation() {
			return new PlaneEquation(1, 0, 0, 0);
		}
		PlaneEquation GetRightPlaneEquation() {
			double X = AxisCoordCalculator.GetClampedCoord(Diagram.AxisX.VisualRangeData, Double.PositiveInfinity, Bounds.Width);
			int x = (int)Math.Round(X);
			return new PlaneEquation(-1, 0, 0, x);
		}
		PlaneEquation GetBottomPlaneEquation() {
			return new PlaneEquation(0, 1, 0, 0);
		}
		PlaneEquation GetTopPlaneEquation() {
			double Y = AxisCoordCalculator.GetClampedCoord(Diagram.AxisY.VisualRangeData, Double.PositiveInfinity, Bounds.Height);
			int y = (int)Math.Round(Y);
			return new PlaneEquation(0, -1, 0, y);
		}
		PlanePolygon[] ClipPolyhedronByLeftPlane(PlanePolygon[] polyhedron, Box boundingBox, PlaneEquation separatingPlane) {
			if (polyhedron == null || polyhedron.Length == 0)
				return null;
			PlaneEquation leftPlane = GetLeftPlaneEquation();
			if (boundingBox != null) {
				if (boundingBox.Location.X + boundingBox.Width < leftPlane.D)
					return null;
				if (boundingBox.Location.X >= leftPlane.D)
					return polyhedron;
			}
			return ClipPolyhedron(leftPlane, separatingPlane, polyhedron);
		}
		PlanePolygon[] ClipPolyhedronByRightPlane(PlanePolygon[] polyhedron, Box boundingBox, PlaneEquation separatingPlane) {
			if (polyhedron == null || polyhedron.Length == 0)
				return null;
			PlaneEquation rightPlane = GetRightPlaneEquation();
			if (boundingBox != null) {
				if (boundingBox.Location.X + boundingBox.Width <= rightPlane.D)
					return polyhedron;
				if (boundingBox.Location.X > rightPlane.D)
					return null;
			}
			return ClipPolyhedron(rightPlane, separatingPlane, polyhedron);
		}
		PlanePolygon[] ClipPolyhedronByBottomPlane(PlanePolygon[] polyhedron, Box boundingBox, PlaneEquation separatingPlane) {
			if (polyhedron == null || polyhedron.Length == 0)
				return null;
			PlaneEquation bottomPlane = GetBottomPlaneEquation();
			if (boundingBox != null) {
				if (boundingBox.Location.Y + boundingBox.Height < bottomPlane.D)
					return null;
				if (boundingBox.Location.Y >= bottomPlane.D)
					return polyhedron;
			}
			return ClipPolyhedron(bottomPlane, separatingPlane, polyhedron);
		}
		PlanePolygon[] ClipPolyhedronByTopPlane(PlanePolygon[] polyhedron, Box boundingBox, PlaneEquation separatingPlane) {
			if (polyhedron == null || polyhedron.Length == 0)
				return null;
			PlaneEquation topPlane = GetTopPlaneEquation();
			if (boundingBox != null) {
				if (boundingBox.Location.Y + boundingBox.Height <= topPlane.D)
					return polyhedron;
				if (boundingBox.Location.Y > topPlane.D)
					return null;
			}
			return ClipPolyhedron(topPlane, separatingPlane, polyhedron);
		}
		IList<PlanePolygon> ClipPolygons(PlaneEquation plane, IList<PlanePolygon> polygons) {
			List<PlanePolygon> list = new List<PlanePolygon>();
			foreach (PlanePolygon polygon in polygons) {
				Vertex[] intersectionVertices;
				PlanePolygon positivePolygon, negativePolygon;
				Intersection intersection = IntersectionUtils.CalcPolygonIntersection3D(plane,
					polygon, Diagram3D.Epsilon, out intersectionVertices, out positivePolygon, out negativePolygon);
				if (intersection == Intersection.Match)
					list.Add(polygon);
				else if (positivePolygon != null)
					list.Add(positivePolygon);
			}
			return list;
		}
		Line[] ClipLines(PlaneEquation plane, Line[] lines) {
			List<Line> list = new List<Line>();
			foreach (Line line in lines) {
				Line positiveLine, negativeLine;
				Vertex intersectionVertex;
				Intersection intersection = IntersectionUtils.CalcLineIntersection3D(plane, line, Diagram3D.Epsilon, out intersectionVertex, out positiveLine, out negativeLine);
				if (intersection == Intersection.Match)
					list.Add(line);
				else if (positiveLine != null)
					list.Add(positiveLine);
			}
			return list.ToArray();
		}
		internal GraphicsCommand CreateDiagramLightingCommand(out GraphicsCommand innerLightingCommand) {
			GraphicsCommand command = new LightingGraphicsCommand(Color.FromArgb(255, 160, 160, 160), Color.White, Color.Black, 2.0f);
			DiagramPoint location = new DiagramPoint(-ViewRadius, ViewRadius, ViewRadius * 1.25);
			innerLightingCommand = new LightGraphicsCommand(0, Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 140, 140, 140),
				Color.FromArgb(255, 0, 0, 0), location, DiagramVector.CreateNormalized(ViewRadius - location.X, -location.Y, -location.Z));
			command.AddChildCommand(innerLightingCommand);
			return command;
		}
		protected override void SetDimensions() {
			CalcSeriesGroups();
			double correction = (PlaneDepth + diagramCorrection) * 2;
			SetWidth(Bounds.Width + correction);
			SetHeight(Bounds.Height + correction);
			SetDepth(CalcDiagramDepth() + correction);
		}
		protected override GraphicsCommand CreateAdditionalModelViewCommand() {
			return new TranslateGraphicsCommand(PlaneDepth + diagramCorrection - Width * 0.5, PlaneDepth + diagramCorrection - Height * 0.5, -Depth * 0.5);
		}
		public GraphicsCommand CreateLightingCommand(out GraphicsCommand innerLightingCommand) {
			GraphicsCommand command = new LightingGraphicsCommand(Color.FromArgb(255, 105, 105, 105), Color.White, Color.Black, 2.0f);
			DiagramPoint location = new DiagramPoint(-ViewRadius * 1.1, ViewRadius, ViewRadius * 1.8);
			GraphicsCommand light0Command = new LightGraphicsCommand(0, Color.FromArgb(255, 0, 0, 0),
				Color.FromArgb(255, 225, 225, 225), Color.FromArgb(255, 20, 20, 20), location,
				DiagramVector.CreateNormalized(ViewRadius - location.X, -location.Y, -location.Z), 8.0f, 180.0f, 1.0f, 0.0f, 0.0f);
			command.AddChildCommand(light0Command);
			location = new DiagramPoint(ViewRadius, ViewRadius * 1.6, ViewRadius);
			GraphicsCommand light1Command = new LightGraphicsCommand(1, Color.FromArgb(255, 0, 0, 0),
				Color.FromArgb(255, 125, 125, 125), Color.FromArgb(255, 0, 0, 0), location,
				DiagramVector.CreateNormalized(-location.X, -location.Y, -location.Z), 0.0f, 180.0f, 1.0f, 0.0f, 0.0f);
			light0Command.AddChildCommand(light1Command);
			innerLightingCommand = light1Command;
			return command;
		}
		public double CalcSeriesWidth(XYDiagram3DSeriesViewBase view) {
			return Math.Abs(GetDiagramPoint(0, 0, false).X - GetDiagramPoint(view.GetSeriesDepth(), 0, false).X);
		}
		public double CalcSeriesOffset(Series series) {
			int index = GetSeriesGroupIndex(series);
			if (index < 0)
				throw new ArgumentException("series");
			double seriesDepth = CalcSeriesDepth();
			double seriesDistance = CalcSeriesDistance();
			return index * (seriesDistance + seriesDepth) + Diagram.SeriesIndentFixed + seriesDepth / 2;
		}
		public BoxPlane CalcVisiblePlanes() {
			DiagramPoint zeroPoint = Project(DiagramPoint.Zero);
			DiagramPoint xPoint = Project(new DiagramPoint(Width, 0, 0));
			DiagramPoint zPoint = Project(new DiagramPoint(0, 0, Depth));
			BoxPlane planes = BoxPlane.Bottom;
			planes |= zeroPoint.Z >= zPoint.Z ? BoxPlane.Back : BoxPlane.Fore;
			planes |= zeroPoint.Z >= xPoint.Z ? BoxPlane.Left : BoxPlane.Right;
			return planes;
		}
		public DiagramPoint GetDiagramPoint(double argument, double value, bool clipping) {
			return GetDiagramPoint(argument, value, clipping, true);
		}
		public DiagramPoint GetDiagramPoint(double argument, double value, bool clipping, bool round) {
			double x, y;
			x = AxisCoordCalculator.GetCoord(VisualRangeX, argument, Bounds.Width, clipping);
			y = AxisCoordCalculator.GetCoord(VisualRangeY, value, Bounds.Height, clipping);
			if (round) {
				x = Math.Round(x);
				y = Math.Round(y);
			}
			return new DiagramPoint(x, y);
		}
		public DiagramPoint GetDiagramPoint(double argument, double value) {
			return GetDiagramPoint(argument, value, true);
		}
		public DiagramPoint GetDiagramPointWithoutRound(double argument, double value) {
			return GetDiagramPoint(argument, value, true, false);
		}
		public DiagramPoint GetDiagramPoint(Series series, double argument, double value, bool clipping) {
			return DiagramPoint.Offset(GetDiagramPoint(argument, value, clipping, true), 0, 0, CalcSeriesOffset(series));
		}
		public DiagramPoint GetDiagramPointForDiagram(double argument, double value) {
			double X = AxisCoordCalculator.GetClampedCoord(VisualRangeX, argument, Bounds.Width + diagramCorrection * 2);
			double Y = AxisCoordCalculator.GetClampedCoord(VisualRangeY, value, Bounds.Height + diagramCorrection * 2);
			int x = (int)Math.Round(X) - diagramCorrection;
			int y = (int)Math.Round(Y) - diagramCorrection;
			return new DiagramPoint(x, y, -diagramCorrection);
		}
		public List<DiagramPoint> GetSeriesBounds(Series series, double minValue, double maxValue) {
			DiagramPoint p1 = GetDiagramPoint(series, Double.NegativeInfinity, minValue, true);
			DiagramPoint p2 = GetDiagramPoint(series, Double.PositiveInfinity, minValue, true);
			DiagramPoint p3 = GetDiagramPoint(series, Double.PositiveInfinity, maxValue, true);
			DiagramPoint p4 = GetDiagramPoint(series, Double.NegativeInfinity, maxValue, true);
			return new List<DiagramPoint>(new DiagramPoint[] { p1, p1, p2, p2, p3, p3, p4, p4 });
		}
		public PlanePolygon[] ClipPolyhedron(PlanePolygon[] polyhedron) {
			return ClipPolyhedron(polyhedron, null, null);
		}
		public PlanePolygon[] ClipPolyhedron(PlanePolygon[] polyhedron, Box boundingBox, PlaneEquation separatingPlane) {
			polyhedron = ClipPolyhedronByLeftPlane(polyhedron, boundingBox, separatingPlane);
			if (polyhedron == null)
				return null;
			polyhedron = ClipPolyhedronByRightPlane(polyhedron, boundingBox, separatingPlane);
			if (polyhedron == null)
				return null;
			if (polyhedron == null)
				return null;
			polyhedron = ClipPolyhedronByBottomPlane(polyhedron, boundingBox, separatingPlane);
			if (polyhedron == null)
				return null;
			return ClipPolyhedronByTopPlane(polyhedron, boundingBox, separatingPlane);
		}
		public IList<PlanePolygon> ClipPolygons(IList<PlanePolygon> polygons) {
			if (polygons == null || polygons.Count == 0)
				return null;
			polygons = ClipPolygons(GetLeftPlaneEquation(), polygons);
			if (polygons == null || polygons.Count == 0)
				return null;
			polygons = ClipPolygons(GetRightPlaneEquation(), polygons);
			if (polygons == null || polygons.Count == 0)
				return null;
			polygons = ClipPolygons(GetBottomPlaneEquation(), polygons);
			if (polygons == null || polygons.Count == 0)
				return null;
			return ClipPolygons(GetTopPlaneEquation(), polygons);
		}
		public Line[] ClipLines(Line[] lines) {
			if (lines == null || lines.Length == 0)
				return null;
			lines = ClipLines(GetLeftPlaneEquation(), lines);
			if (lines == null || lines.Length == 0)
				return null;
			lines = ClipLines(GetRightPlaneEquation(), lines);
			if (lines == null || lines.Length == 0)
				return null;
			lines = ClipLines(GetBottomPlaneEquation(), lines);
			if (lines == null || lines.Length == 0)
				return null;
			return ClipLines(GetTopPlaneEquation(), lines);
		}
		public int GetSeriesWeight(Series series) {
			int seriesIndex = GetSeriesIndex(series);
			return seriesIndex == -1 ? 0 : (seriesIndex * seriesWeightStep);
		}
		public double CalcConnectorAngle(DiagramVector direction) {
			DiagramVector v = MathUtils.Mult(direction, Diagram.RotationMatrix);
			double angle = MathUtils.CalcAngle(v, new DiagramVector(0, 0, 1));
			return (ComparingUtils.CompareDoubles(angle, 0, SeriesLabelViewData.InertAngle) == 0 ||
					ComparingUtils.CompareDoubles(angle, Math.PI, SeriesLabelViewData.InertAngle) == 0) ?
				Double.NaN : Math.Atan2(v.DY, v.DX);
		}
	}
}
