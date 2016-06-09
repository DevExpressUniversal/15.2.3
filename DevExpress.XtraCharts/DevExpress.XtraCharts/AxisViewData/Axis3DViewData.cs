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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class Axis3DViewData {
		static List<PlanePolygon> CreateAndFillInterlacedPolygons(Axis3D axis, XYDiagram3DViewData diagramViewData, Box filterBox, Box foreOrBackBox, ZPlaneRectangle area, double depth, double planeDepth) {
			Box left = diagramViewData.Left;
			Box right = diagramViewData.Right;
			Box bottom = diagramViewData.Bottom;
			RectangleFillStyle3D fillStyle = axis.ActualInterlacedFillStyle;
			Color color = axis.ActualInterlacedColor;
			int weight = axis.IsVertical ? XYDiagram3DViewData.InterlacedYWeight : XYDiagram3DViewData.InterlacedXWeight;
			ZPlaneRectangle gradientRect = new ZPlaneRectangle(foreOrBackBox.Location, foreOrBackBox.Fore.Width, foreOrBackBox.Fore.Height);
			List<PlanePolygon> polygons = new List<PlanePolygon>();
			List<PlaneQuadrangle> planes = filterBox.FilterPlanes(foreOrBackBox);
			polygons.AddRange(XYDiagram3DFillStyleHelper.FillPlanePolygons(planes.ToArray(), gradientRect, fillStyle, color, weight));
			if (axis.IsVertical)
				if (right == null) {
					Box box = new Box(area.Location, -planeDepth, area.Height, depth);
					planes = left.FilterPlanes(box);
					polygons.AddRange(XYDiagram3DFillStyleHelper.FillPlanePolygons(planes.ToArray(), gradientRect, fillStyle, color, weight));
				}
				else {
					Box box = new Box(DiagramPoint.Offset(area.Location, area.Width, 0, 0), planeDepth, area.Height, depth);
					planes = right.FilterPlanes(box);
					polygons.AddRange(XYDiagram3DFillStyleHelper.FillPlanePolygons(planes.ToArray(), gradientRect, fillStyle, color, weight));
				}
			else {
				Box box = new Box(area.Location, area.Width, -planeDepth, depth);
				planes = bottom.FilterPlanes(box);
				polygons.AddRange(XYDiagram3DFillStyleHelper.FillPlanePolygons(planes.ToArray(), gradientRect, fillStyle, color, weight));
			}
			return polygons;
		}
		static IList<PlanePolygon> CreateInterlacedPolygons(Axis3D axis, XYDiagram3DViewData diagramViewData, IList<InterlacedData> interlacedData) {
			List<PlanePolygon> polygons = new List<PlanePolygon>();
			if (!axis.Interlaced)
				return polygons;
			XYDiagram3DCoordsCalculator coordsCalculator = diagramViewData.CoordsCalculator;
			double minValue = axis.VisualRangeData.Min;
			double maxValue = axis.VisualRangeData.Max;
			Axis3DMapping mapping = new Axis3DMapping(coordsCalculator, axis);
			List<ZPlaneRectangle> interlacedAreas = new List<ZPlaneRectangle>(interlacedData.Count);
			foreach (InterlacedData data in interlacedData) {
				ZPlaneRectangle rect = new ZPlaneRectangle(mapping.GetNearDiagramPoint(data.Near < minValue ? Double.NegativeInfinity : data.Near, 0, 0),
														   mapping.GetFarDiagramPoint(data.Far > maxValue ? Double.PositiveInfinity : data.Far, 0, 0));
				interlacedAreas.Add(rect);
			}
			int interlacedAreasCount = interlacedAreas.Count;
			double depth = coordsCalculator.Depth;
			double planeDepth = coordsCalculator.PlaneDepth;
			Box back = diagramViewData.Back;
			Box fore = diagramViewData.Fore;		   
			if (back == null)
				foreach (ZPlaneRectangle area in interlacedAreas) {
					Box box = new Box((ZPlaneRectangle)PlaneRectangle.Offset(area, 0, 0, depth), planeDepth);
					polygons.AddRange(CreateAndFillInterlacedPolygons(axis, diagramViewData, fore, box, area, depth, planeDepth));
				}
			else
				foreach (ZPlaneRectangle area in interlacedAreas) {
					Box box = new Box(area, -planeDepth);
					polygons.AddRange(CreateAndFillInterlacedPolygons(axis, diagramViewData, back, box, area, depth, planeDepth));
				}			
			return polygons;
		}
		static IList<Line> CreateHorizontalGridLines(Axis3D axis, XYDiagram3DViewData diagramViewData, List<double> values) {
			XYDiagram3DCoordsCalculator coordsCalculator = diagramViewData.CoordsCalculator;
			double depth = coordsCalculator.Depth;
			double planeDepth = coordsCalculator.PlaneDepth;
			double fullDepth = depth + planeDepth;
			List<Line> lines = new List<Line>();
			foreach (double value in values) {
				DiagramPoint p1 = axis.GetDiagramPoint(coordsCalculator, value, Double.NegativeInfinity);
				DiagramPoint p2 = axis.GetDiagramPoint(coordsCalculator, value, Double.PositiveInfinity);
				if (diagramViewData.Back == null)
					lines.AddRange(new XPlaneRectangle(DiagramPoint.Offset(p1, 0, 0, depth), DiagramPoint.Offset(p2, 0, 0, fullDepth)).GetPerimeter());
				else 
					lines.AddRange(new XPlaneRectangle(p1, DiagramPoint.Offset(p2, 0, 0, -planeDepth)).GetPerimeter());
				lines.AddRange(new XPlaneRectangle(p1, DiagramPoint.Offset(p1, 0, -planeDepth, depth)).GetPerimeter());
			}
			return lines;
		}
		static IList<Line> CreateVerticalGridLines(Axis3D axis, XYDiagram3DViewData diagramViewData, List<double> values) {
			XYDiagram3DCoordsCalculator coordsCalculator = diagramViewData.CoordsCalculator;
			double depth = coordsCalculator.Depth;
			double planeDepth = coordsCalculator.PlaneDepth;
			double fullDepth = depth + planeDepth;
			List<Line> lines = new List<Line>();
			foreach (double value in values) {
				DiagramPoint p1 = axis.GetDiagramPoint(coordsCalculator, value, Double.NegativeInfinity);
				DiagramPoint p2 = axis.GetDiagramPoint(coordsCalculator, value, Double.PositiveInfinity);
				if (diagramViewData.Back == null)
					lines.AddRange(new YPlaneRectangle(DiagramPoint.Offset(p1, 0, 0, depth), DiagramPoint.Offset(p2, 0, 0, fullDepth)).GetPerimeter());
				else
					lines.AddRange(new YPlaneRectangle(p1, DiagramPoint.Offset(p2, 0, 0, -planeDepth)).GetPerimeter());
				if (diagramViewData.Right == null)
					lines.AddRange(new YPlaneRectangle(p1, DiagramPoint.Offset(p1, -planeDepth, 0, depth)).GetPerimeter());
				else
					lines.AddRange(new YPlaneRectangle(DiagramPoint.Offset(p2, planeDepth, 0, depth), p2).GetPerimeter());
			}
			return lines;
		}
		static IList<Line> CreateGridLines(Axis3D axis, XYDiagram3DViewData diagramViewData, List<double> values, Color color, LineStyle lineStyle, int weight) {
			IList<Line> lines = axis.IsVertical ? CreateVerticalGridLines(axis, diagramViewData, values) : 
												CreateHorizontalGridLines(axis, diagramViewData, values); 
			foreach (Line line in lines) {
				lineStyle.SetLineStyle(line);
				line.Color = color;
				line.Weight = weight;
			}
			return lines;
		}
		static IList<Line> CreateGridLines(Axis3D axis, XYDiagram3DViewData diagramViewData, AxisGridDataEx gridData) {
			GridLines gridLines = axis.GridLines;
			List<Line> lines = new List<Line>();
			XYDiagram3DAppearance appearance = CommonUtils.GetActualAppearance(axis).XYDiagram3DAppearance;
			if (gridLines.Visible) {
				IList<Line> majorLines = CreateGridLines(axis, diagramViewData, gridData.Items.VisibleValues, gridLines.GetActualColor(appearance), 
					gridLines.LineStyle, axis.IsVertical ? XYDiagram3DViewData.GridLinesYWeight : XYDiagram3DViewData.GridLinesXWeight);
				lines.AddRange(majorLines);
			}
			if (gridLines.MinorVisible) {
				IList<Line> minorLines = CreateGridLines(axis, diagramViewData, gridData.MinorValues, gridLines.GetActualMinorColor(appearance),
					gridLines.MinorLineStyle, axis.IsVertical ? XYDiagram3DViewData.MinorGridLinesYWeight : XYDiagram3DViewData.MinorGridLinesXWeight);
				lines.AddRange(minorLines);
			}
			return lines;
		}
		readonly IList<PlanePolygon> interlacedPolygons;
		readonly IList<Line> gridLines;
		readonly AxisLabel3DViewData labelViewData;
		public IList<PlanePolygon> InterlacedPolygons { get { return interlacedPolygons; } }
		public IList<Line> GridLines { get { return gridLines; } }
		public Axis3DViewData(Axis3D axis, XYDiagram3DViewData diagramViewData, TextMeasurer textMeasurer) {
			XYDiagram3DCoordsCalculator coordsCalculator = diagramViewData.CoordsCalculator;
			IMinMaxValues visualRange = (IMinMaxValues)axis.VisualRangeData;
			GridAndTextDataEx gridAndTextData = new GridAndTextDataEx(axis.GetSeries(), axis, false, visualRange, visualRange, 
				axis.IsVertical ? coordsCalculator.Bounds.Height : coordsCalculator.Bounds.Width, (axis.Label == null) ? false : axis.Label.Staggered);
			AxisGridDataEx gridData = gridAndTextData.GridData;
			interlacedPolygons = CreateInterlacedPolygons(axis, diagramViewData, gridData.InterlacedData);
			gridLines = CreateGridLines(axis, diagramViewData, gridData);
			if (axis.Label.ActualVisibility)
				labelViewData = new AxisLabel3DViewData(axis, textMeasurer, coordsCalculator, gridAndTextData.TextData);
		}
		public void RenderLabels(IRenderer renderer) {
			if (labelViewData != null)
				labelViewData.Render(renderer);
		}
	}
}
