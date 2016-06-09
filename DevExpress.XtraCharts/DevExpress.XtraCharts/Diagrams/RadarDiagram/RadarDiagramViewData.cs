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
	public class RadarDiagramViewData : PaneViewData {
		readonly List<AnnotationLayout> annotationsAnchorPointsLayout;
		readonly List<AnnotationViewData> annotationsViewData;
		readonly RadarDiagram diagram;
		readonly RadarDiagramMapping diagramMapping;
		readonly RectangleF actualBounds;
		readonly RadarAxisXViewData axisXViewData;
		readonly RadarAxisYViewData axisYViewData;
		readonly int borderThickness;
		readonly Color borderColor;
		readonly Rectangle maxBounds;
		HitTestController HitTestController { get { return diagram.Chart.HitTestController; } }
		public RectangleF ActualBounds { get { return actualBounds; } }
		public override List<AnnotationLayout> AnnotationsAnchorPointsLayout { get { return annotationsAnchorPointsLayout; } }
		public override bool IsEmpty { get { return false; } }
		public RadarAxisXViewData AxisXViewData { get { return axisXViewData; } }
		public RadarAxisYViewData AxisYViewData { get { return axisYViewData; } }
		public GRealPoint2D Center { get { return diagramMapping.Center; } }
		public float Radius { get { return (float)diagramMapping.Radius; } }
		public float BorderRadius { get { return Radius + borderThickness / 2.0f; } }
		public int BorderThickness { get { return borderThickness; } }
		public Color BorderColor { get { return borderColor; } }
		public RadarDiagramMapping DiagramMapping { get { return diagramMapping; } }
		public Rectangle MaxBounds { get { return maxBounds; } }
		public RadarDiagramViewData(RadarDiagram diagram, RadarDiagramMapping diagramMapping, Rectangle bounds, RectangleF actualBounds, RadarAxisXViewData axisXViewData, 
				RadarAxisYViewData axisYViewData, int borderThickness, Color borderColor, List<SeriesLayout> seriesLayoutList, List<SeriesLabelLayoutList> labelLayoutLists,
				List<AnnotationLayout> annotationsAnchorPointsLayout, List<AnnotationViewData> annotationsViewData, Rectangle maxBounds) : base(diagram, bounds, seriesLayoutList, labelLayoutLists) {
			this.diagram = diagram;
			this.diagramMapping = diagramMapping;
			this.actualBounds = actualBounds;
			this.axisXViewData = axisXViewData;
			this.axisYViewData = axisYViewData;
			this.borderThickness = borderThickness;
			this.borderColor = borderColor;
			this.maxBounds = maxBounds;
			this.annotationsAnchorPointsLayout = annotationsAnchorPointsLayout;
			this.annotationsViewData = annotationsViewData;
		}
		double CalcPolygonStartAngle() {
			double startAngle = (diagram.RotationDirection == RadarDiagramRotationDirection.Counterclockwise) ?
				-diagram.StartAngleInDegrees * Math.PI / 180.0 : diagram.StartAngleInDegrees * Math.PI / 180.0;
			return startAngle - Math.PI / 2.0;
		}
		void ProcessOverlappingAxisLabels() {
			RadarAxisXLabel axisXLabel = axisXViewData.Axis.Label;
			RadarAxisYLabel axisYLabel = axisYViewData.Axis.Label;
			if ((axisXLabel.ActualVisibility && axisXLabel.ResolveOverlappingOptions.AllowHide) ||
				(axisYLabel.ActualVisibility && axisYLabel.ResolveOverlappingOptions.AllowHide)) {
				List<IAxisLabelLayout> labels = new List<IAxisLabelLayout>();
				if (axisXViewData.LabelItems != null) {
					labels.Clear();
					foreach (AxisLabelItemBase item in axisXViewData.LabelItems) {
						IAxisLabelLayout itemLayout = item as IAxisLabelLayout;
						if (itemLayout != null)
							labels.Add(itemLayout);
					}
					AxisLabelsResolveOverlappingHelper.Process(labels, axisXViewData.Axis, AxisAlignment.Near);
				}
				if (axisYViewData.LabelItems != null) {
					labels.Clear();
					foreach (AxisLabelItemBase item in axisYViewData.LabelItems) {
						IAxisLabelLayout itemLayout = item as IAxisLabelLayout;
						if (itemLayout != null)
							labels.Add(itemLayout);
					}
					AxisLabelsResolveOverlappingHelper.Process(labels, axisYViewData.Axis, AxisAlignment.Near);
				}
			}
		}
		void RenderContent(IRenderer renderer, DiagramAppearance appearance) {
			RadarAxisXMapping axisXMapping = new RadarAxisXMapping(diagramMapping);
			if (axisXViewData != null) {
				axisXViewData.RenderInterlacedAreas(renderer, diagramMapping);
				axisYViewData.RenderInterlacedAreas(renderer, diagramMapping, this);
				axisXViewData.RenderMinorGridLines(renderer, axisXMapping, appearance);
				axisYViewData.RenderMinorGridLines(renderer, diagramMapping, this, appearance);
				axisXViewData.RenderGridLines(renderer, axisXMapping, appearance);
				axisYViewData.RenderGridLines(renderer, diagramMapping, this, appearance);
			}
		}
		void RenderAxes(IRenderer renderer) {
			axisYViewData.Render(renderer, this);
			axisXViewData.RenderLabels(renderer);
			if (axisYViewData.LabelData != null)
				axisYViewData.LabelData.Render(renderer);
		}
		public IPolygon CreatePolygon(GRealPoint2D center, float radius, int verticesCount, RectangleF bounds) {
			if (diagram.DrawingStyle == RadarDiagramDrawingStyle.Circle)
				return new CirclePolygon(center, radius);
			LineStrip points = GraphicUtils.CalcPolygon(CalcPolygonStartAngle(), verticesCount, center, radius);
			return new VariousPolygon(points, bounds);
		}
		public override void CalculateSeriesAndLabelLayout(TextMeasurer textMeasurer, ChartDrawingHelper drawingHelper) {
			foreach (RadarDiagramSeriesLayout seriesLayout in SeriesLayoutList)
				seriesLayout.Calculate();
			if (IsLabelsResolveOverlapping)
				XYDiagramResolveOverlappingHelper.Process(GetLabelsForResolveOverlapping(), (ZPlaneRectangle)MaxBounds, Diagram.LabelsResolveOverlappingMinIndent);
			ProcessOverlappingAxisLabels();
		}
		public override GraphicsCommand CreateGraphicsCommand() {
			return null;
		}
		public override void Render(IRenderer renderer) {
			GRealPoint2D center = Center;
			IPolygon diagramPolygon = CreatePolygon(new GRealPoint2D(center.X, center.Y), Radius, DiagramMapping.VerticesCount, ActualBounds);
			VariousPolygon polygon = diagramPolygon as VariousPolygon;
			if (diagram.DrawingStyle == RadarDiagramDrawingStyle.Circle) {
				diagram.Shadow.Render(renderer, center, Radius);
				renderer.EnableAntialiasing(true);
				if (diagram.BackImage.Image == null)
					diagram.ActualFillStyle.Options.RenderCircle(renderer, new PointF((float)center.X, (float)center.Y), Radius, diagram.ActualBackColor, Color.Empty);
				else
					diagram.BackImage.Render(renderer, diagramPolygon);
				renderer.RestoreAntialiasing();
			}
			else {
				if (polygon != null) {
					diagram.Shadow.Render(renderer, polygon);
					renderer.EnableAntialiasing(true);
					if (diagram.BackImage.Image == null)
						diagram.ActualFillStyle.Options.Render(renderer, polygon.Vertices, (RectangleF)ActualBounds, diagram.ActualBackColor, Color.Empty);
					else
						diagram.BackImage.Render(renderer, diagramPolygon);
					renderer.RestoreAntialiasing();
				}
			}
			renderer.EnableAntialiasing(true);
			RenderContent(renderer, CommonUtils.GetActualAppearance(diagram).RadarDiagramAppearance);
			if (diagram.DrawingStyle == RadarDiagramDrawingStyle.Circle) {
				if (BorderThickness > 0)
					renderer.DrawCircle(new Point((int)center.X, (int)center.Y), (int)BorderRadius, BorderColor, BorderThickness);
			}
			else {
				if (polygon != null) {
					if (BorderThickness > 0) {
						LineStrip points = GraphicUtils.CalcPolygon(CalcPolygonStartAngle(), DiagramMapping.VerticesCount, new GRealPoint2D(center.X, center.Y), BorderRadius);
						renderer.DrawPolygon(points, BorderColor, BorderThickness);
					}
				}
			}
			renderer.RestoreAntialiasing();
			if (!diagram.AxisY.TopLevel)
				RenderAxes(renderer);
			using (GraphicsPath path = diagramPolygon.GetPath()) {
				renderer.ProcessHitTestRegion(HitTestController, diagram, null, new HitRegion((GraphicsPath)path.Clone()));
				if (SeriesLayoutList.Count > 0) {
					using (Region clip = new Region(path)) {
						renderer.SetClipping(clip);
						foreach (RadarDiagramSeriesLayout seriesLayout in SeriesLayoutList)
							seriesLayout.RenderShadow(renderer);
						foreach (RadarDiagramSeriesLayout seriesLayout in SeriesLayoutList)
							seriesLayout.Render(renderer);
						renderer.RestoreClipping();
					}
				}
			}
			RenderSeriesLabels(renderer);
			if (diagram.AxisY.TopLevel)
				RenderAxes(renderer);
			foreach (AnnotationViewData viewData in annotationsViewData)
				viewData.Render(renderer);
		}
		public override void RenderMiddle(IRenderer renderer) {
		}
		public override void RenderAbove(IRenderer renderer) {  
		}
	}
}
