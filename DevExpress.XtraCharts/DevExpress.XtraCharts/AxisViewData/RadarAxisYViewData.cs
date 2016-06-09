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
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class RadarAxisYViewData {
		readonly RadarAxisY axis;
		readonly GridAndTextDataEx gridAndTextData;
		readonly RadarAxisYTickmarksViewData tickmarksViewData;
		readonly RadarAxisYLabelViewData labelViewData;
		readonly bool isVerticalOrHorizontal;
		int half;
		int drawingHalf;
		int drawingCrossHalf;		
		HitTestController HitTestController { get { return axis.Diagram.Chart.HitTestController; } }
		public RadarAxisY Axis { get { return axis; } }
		public RadarAxisYLabelViewData LabelData { get { return labelViewData; } }
		public int Half { get { return half; } }
		public int DrawingHalf { get { return drawingHalf; } }
		public int DrawingCrossHalf { get { return drawingCrossHalf; } }
		public double LabelOffset { get { return tickmarksViewData != null ? tickmarksViewData.Extent : Half; } }
		public bool IsVerticalOrHorizontal { get { return isVerticalOrHorizontal; } }
		public AxisLabelItemList LabelItems { get { return labelViewData != null ? labelViewData.Items :  null; } }
		public RadarAxisYViewData(TextMeasurer textMeasurer, RadarAxisY axis, RadarDiagramMapping diagramMapping, GridAndTextDataEx gridAndTextData, bool isVerticalOrHorizontal) {
			this.axis = axis;
			this.gridAndTextData = gridAndTextData;
			if (axis.Visible) {
				this.isVerticalOrHorizontal = isVerticalOrHorizontal;
				int crossHalf;
				ThicknessUtils.SplitToHalfs(axis.Thickness, out this.half, out crossHalf);
				ThicknessUtils.SplitToHalfs(axis.Thickness, ((IHitTest)axis).State, out this.drawingHalf, out this.drawingCrossHalf);
				RadarAxisYMapping mapping = new RadarAxisYMapping(diagramMapping);
				if (axis.Tickmarks.Visible || axis.Tickmarks.MinorVisible)
					tickmarksViewData = new RadarAxisYTickmarksViewData(mapping, this, gridAndTextData.GridData, isVerticalOrHorizontal);
				if (axis.Label.ActualVisibility)
					labelViewData = new RadarAxisYLabelViewData(textMeasurer, this, mapping, gridAndTextData);
			}
		}
		public void UpdateCorrection(RectangleCorrection correction) {
			if (tickmarksViewData != null)
				correction.Update(tickmarksViewData.Bounds);
			if(labelViewData != null)
				labelViewData.UpdateCorrection(correction);
		}
		void RenderTickmarks(IRenderer renderer) {
			if (tickmarksViewData == null)
				return;
			GraphicsPath tickmarksPath = tickmarksViewData.CreateGraphicsPath();
			if (tickmarksPath != null)
				renderer.ProcessHitTestRegion(HitTestController, axis, null, new HitRegion(tickmarksPath));
			foreach (TickmarkLineBase tickmarkLine in tickmarksViewData.TickmarkLines)
				tickmarkLine.Render(renderer, axis.DrawingColor);
			foreach (TickmarkLineBase minorTickmarkLine in tickmarksViewData.MinorTickmarkLines)
				minorTickmarkLine.Render(renderer, axis.DrawingColor);
		} 
		public void RenderInterlacedAreas(IRenderer renderer, RadarDiagramMapping mapping, RadarDiagramViewData diagramViewData) {
			if (!axis.Interlaced)
				return;
			PolygonFillStyle fillStyle = axis.ActualInterlacedFillStyle;
			Color color = axis.ActualInterlacedColor;
			RectangleF gradientBounds = mapping.MappingBounds;
			foreach (InterlacedData data in gridAndTextData.GridData.InterlacedData) {
				float radius1 = (float)mapping.GetDistance(data.Near);
				float radius2 = (float)mapping.GetDistance(data.Far);
				if (radius2 > mapping.Radius)
					radius2 = (float)mapping.Radius;
				if (radius2 <= radius1)
					break;
				IPolygon polygon1 = radius1 <= 0.0f ? null : diagramViewData.CreatePolygon(mapping.Center, radius1, mapping.VerticesCount, gradientBounds);
				IPolygon polygon2 = diagramViewData.CreatePolygon(mapping.Center, radius2, mapping.VerticesCount, gradientBounds);
				if (axis.Diagram.DrawingStyle == RadarDiagramDrawingStyle.Circle) {
					if (polygon1 == null)
						fillStyle.Options.RenderCircle(renderer, new PointF((float)mapping.Center.X, (float)mapping.Center.Y), radius2, color, Color.Empty);
					else {
						Region region;
						using (GraphicsPath path = new GraphicsPath()) {
							RectangleF rect = new RectangleF((float)mapping.Center.X, (float)mapping.Center.Y, 0, 0);
							rect.Inflate(radius1, radius1);
							path.AddEllipse(rect);
							region = new Region(path);
						}
						renderer.SetClipping(region, CombineMode.Exclude);
						fillStyle.Options.RenderCircle(renderer, new PointF((float)mapping.Center.X, (float)mapping.Center.Y), radius2, color, Color.Empty);
						renderer.RestoreClipping();
					}
				}
				else {
					LineStrip vertices;
					if (polygon1 == null)
						vertices = polygon2.Vertices;
					else {
						LineStrip vertices1 = polygon1.Vertices;
						LineStrip vertices2 = polygon2.Vertices;
						LineStrip verticesArray = new LineStrip((mapping.VerticesCount + 1) * 2);
						verticesArray.AddRange(vertices1);
						verticesArray.Add(vertices1[0]);
						verticesArray.Add(vertices2[0]);
						for (int j = vertices2.Count - 1; j >= 0; j--)
							verticesArray.Add(vertices2[j]);
						vertices = verticesArray;
					}
					fillStyle.Options.Render(renderer, vertices, (RectangleF)gradientBounds, color, Color.Empty);
				}
			}
		}		
		public void RenderMinorGridLines(IRenderer renderer, RadarDiagramMapping diagramMapping, RadarDiagramViewData diagramViewData, DiagramAppearance appearance) {
			if (axis.GridLines.MinorVisible)
				RenderGridLines(renderer, CalculateGridLinesViewData(diagramMapping, diagramViewData, gridAndTextData.GridData.MinorValues), axis.GridLines.GetActualMinorColor(appearance), axis.GridLines.MinorLineStyle);
		}
		public void RenderGridLines(IRenderer renderer, RadarDiagramMapping diagramMapping, RadarDiagramViewData diagramViewData, DiagramAppearance appearance) {
			if (axis.GridLines.Visible)
				RenderGridLines(renderer, CalculateGridLinesViewData(diagramMapping, diagramViewData, gridAndTextData.GridData.Items.VisibleValues), 
					axis.GridLines.GetActualColor(appearance), axis.GridLines.LineStyle);
		}
		List<RadarAxisYGridLineViewData> CalculateGridLinesViewData(RadarDiagramMapping diagramMapping, RadarDiagramViewData diagramViewData, List<double> values) {
			if (values.Count == 0) 
				return null;
			RadarDiagram diagram = Axis.Diagram as RadarDiagram;
			if (diagram == null)
				return null;
			List<RadarAxisYGridLineViewData> viewData = new List<RadarAxisYGridLineViewData>();
			int verticesCount = diagramMapping.VerticesCount;
			RectangleF bounds = (RectangleF)diagramMapping.MappingBounds;
			for (int i = 0; i < values.Count; i++) {
				float radius = (float)diagramMapping.GetDistance(values[i]);
				if (radius > 0)
					viewData.Add(new RadarAxisYGridLineViewData(diagramViewData.CreatePolygon(diagramMapping.Center, radius, verticesCount, bounds)));
			}
			return viewData;
		}
		static void RenderGridLines(IRenderer renderer, List<RadarAxisYGridLineViewData> viewData, Color color, LineStyle lineStyle) {
			if (viewData == null || viewData.Count == 0)
				return;
			foreach (RadarAxisYGridLineViewData data in viewData) {
				renderer.EnableAntialiasing(lineStyle.AntiAlias);
				IPolygon polygon = data.Polygon;
				CirclePolygon circle = polygon as CirclePolygon;
				if (circle != null)
					renderer.DrawCircle(new Point((int)circle.Center.X, (int)circle.Center.Y), (int)circle.Radius, color, lineStyle.Thickness, lineStyle);
				else {
					LineStrip vertices = polygon.Vertices;
					LineStrip wholeVertices = new LineStrip(vertices.Count + 1);
					wholeVertices.AddRange(vertices);
					wholeVertices.Add(wholeVertices[0]);
					renderer.DrawLines(wholeVertices, color, lineStyle.Thickness, lineStyle, LineCap.Flat);
				}
				renderer.RestoreAntialiasing();
			}
		}
		GraphicsPath CreateAxisGraphicsPath(Point zeroPoint, Point maxPoint) {
			if (zeroPoint.X == maxPoint.X && zeroPoint.Y == maxPoint.Y)
				return null;
			GraphicsPath path = new GraphicsPath();
			try {
				path.AddLine(zeroPoint, maxPoint);
				using (Pen pen = new Pen(Color.Empty, axis.Thickness))
					path.Widen(pen);
				return path;
			}
			catch {
				path.Dispose();
				return null;
			}
		}
		void RenderVerticalOrHorizontalAxis(IRenderer renderer, RadarAxisYMapping mappping) {
			DiagramPoint zeroPoint = mappping.GetDiagramPoint(axis.VisualRangeData.Min, 0, DrawingHalf);
			DiagramPoint maxPoint = mappping.GetDiagramPoint(axis.VisualRangeData.Max, 0, -DrawingCrossHalf);
			if (Math.Abs(maxPoint.X - zeroPoint.X) < 1.0 && Math.Abs(maxPoint.Y - zeroPoint.Y) < 1.0)
				return;
			RenderTickmarks(renderer);
			ZPlaneRectangle axisRect = ZPlaneRectangle.MakeRectangle(zeroPoint, maxPoint);
			renderer.FillRectangle((RectangleF)axisRect, axis.DrawingColor);
			renderer.ProcessHitTestRegion(HitTestController, axis, null, new HitRegion((Rectangle)axisRect));
		}
		void RenderObliqueAxis(IRenderer renderer, RadarAxisYMapping mappping) {
			DiagramPoint zeroPoint = mappping.GetDiagramPoint(axis.VisualRangeData.Min, 0, 0);
			DiagramPoint maxPoint = mappping.GetDiagramPoint(axis.VisualRangeData.Max, 0, 0);
			if (Math.Abs(maxPoint.X - zeroPoint.X) < 1.0 && Math.Abs(maxPoint.Y - zeroPoint.Y) < 1.0)
				return;
			renderer.EnableAntialiasing(true);
			RenderTickmarks(renderer);
			GraphicsPath axisPath = CreateAxisGraphicsPath((Point)zeroPoint, (Point)maxPoint);
			if (axisPath != null) {
				renderer.ProcessHitTestRegion(HitTestController, axis, null, new HitRegion(axisPath));
				int thickness = ThicknessUtils.CorrectThicknessByHitState(axis.Thickness, ((IHitTest)axis).State);
				renderer.DrawLine((Point)zeroPoint, (Point)maxPoint, axis.DrawingColor, thickness);
			}
			renderer.RestoreAntialiasing();
		}
		public void Render(IRenderer renderer, RadarDiagramViewData diagramViewData) {
			if (!axis.Visible)
				return;
			RadarAxisYMapping mappping = new RadarAxisYMapping(diagramViewData.DiagramMapping);
			if (IsVerticalOrHorizontal)
				RenderVerticalOrHorizontalAxis(renderer, mappping);
			else
				RenderObliqueAxis(renderer, mappping);
		}
	}
}
