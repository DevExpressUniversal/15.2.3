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
	public class XYDiagram3DViewData : PaneViewData {
		public const int BackImageWeight = 0;
		public const int GridLinesYWeight = 1;
		public const int GridLinesXWeight = 2;
		public const int MinorGridLinesYWeight = 3;
		public const int MinorGridLinesXWeight = 4;
		public const int InterlacedYWeight = 5;
		public const int InterlacedXWeight = 6;
		public const int DiagramWeight = 7;
		static GraphicsCommand CreateImageBackgroundGraphicsCommand(PlanePrimitive primitive) {
			PlanePolygon polygon = primitive as PlanePolygon;
			if (polygon == null)
				throw new ArgumentException("primitive");
			MaskColorBufferGraphicsCommand maskColorCommand = new MaskColorBufferGraphicsCommand();
			MaskDepthBufferGraphicsCommand maskDepthCommand = new MaskDepthBufferGraphicsCommand();
			maskColorCommand.AddChildCommand(maskDepthCommand);
			maskDepthCommand.AddChildCommand(new PolygonGraphicsCommand(polygon));
			return maskColorCommand;
		}
		readonly List<AnnotationLayout> annotationsAnchorPointsLayout;
		readonly List<AnnotationViewData> annotationsViewData;
		readonly XYDiagram3DCoordsCalculator coordsCalculator;
		readonly Axis3DViewData axisXViewData;
		readonly Axis3DViewData axisYViewData;
		readonly Graphics3DCache graphicsCache;
		readonly Box inner;
		Box back, fore, left, right, bottom;
		new XYDiagram3D Diagram { get { return (XYDiagram3D)base.Diagram; } }
		public override List<AnnotationLayout> AnnotationsAnchorPointsLayout { get { return annotationsAnchorPointsLayout; } }
		public XYDiagram3DCoordsCalculator CoordsCalculator { get { return coordsCalculator; } }
		public Axis3DViewData AxisXViewData { get { return axisXViewData; } }
		public Axis3DViewData AxisYViewData { get { return axisYViewData; } }
		public Box Inner { get { return inner; } }
		public Box Back { get { return back; } }
		public Box Fore { get { return fore; } }
		public Box Left { get { return left; } }
		public Box Right { get { return right; } }
		public Box Bottom { get { return bottom; } }
		public XYDiagram3DViewData(XYDiagram3DCoordsCalculator coordsCalculator, Graphics3DCache graphicsCache, TextMeasurer textMeasurer, List<SeriesLayout> seriesLayoutList, List<SeriesLabelLayoutList> labelLayoutLists,
			List<AnnotationLayout> annotationsAnchorPointsLayout, List<AnnotationViewData> annotationsViewData) : base(coordsCalculator.Diagram, coordsCalculator.Bounds, seriesLayoutList, labelLayoutLists) {
			this.coordsCalculator = coordsCalculator;
			this.graphicsCache = graphicsCache;
			inner = CalcInnerBox();
			CalcPlaneBoxes(coordsCalculator.CalcVisiblePlanes());
			XYDiagram3D diagram = coordsCalculator.Diagram;
			axisXViewData = new Axis3DViewData(diagram.AxisX, this, textMeasurer);
			axisYViewData = new Axis3DViewData(diagram.AxisY, this, textMeasurer);
			this.annotationsAnchorPointsLayout = annotationsAnchorPointsLayout;
			this.annotationsViewData = annotationsViewData;
		}
		Box CalcInnerBox() {
			DiagramPoint p1 = coordsCalculator.GetDiagramPointForDiagram(Double.NegativeInfinity, Double.NegativeInfinity);
			DiagramPoint p2 = coordsCalculator.GetDiagramPointForDiagram(Double.PositiveInfinity, Double.PositiveInfinity);
			ZPlaneRectangle rect = new ZPlaneRectangle(p1, p2);
			return new Box(rect.Location, rect.Width, rect.Height, coordsCalculator.Depth);
		}
		void CalcPlaneBoxes(BoxPlane plane) {
			double depth = coordsCalculator.Diagram.PlaneDepthFixed;
			if ((plane & BoxPlane.Back) > 0)
				back = new Box(inner.Back, -depth);
			if ((plane & BoxPlane.Fore) > 0)
				fore = new Box(inner.Fore, depth);
			if ((plane & BoxPlane.Left) > 0)
				left = new Box(inner.Left, -depth);
			if ((plane & BoxPlane.Right) > 0)
				right = new Box(inner.Right, depth);
			bottom = new Box(inner.Bottom, -depth);
		}
		void CalculateLayout(ChartDrawingHelper drawingHelper) {
			if (graphicsCache != null && graphicsCache.Container == null) {
				foreach (XYDiagram3DSeriesLayout seriesLayout in SeriesLayoutList)
					seriesLayout.Calculate();
				PrimitivesContainer container = GetPrimitivesContainer();
				graphicsCache.Container = container;
				graphicsCache.GraphicsTree = drawingHelper == null ?
					GraphicsHelper.BuildGraphicsTree(container) : drawingHelper.GetGraphicsTree(container);
				PolygonGroups groups = GraphicsHelper.BuildGroups(container);
				groups.Sort();
				graphicsCache.PolygonGroups = groups;
			}
		}
		List<Line> GetDiagramGridLines() {
			List<Line> gridLines = new List<Line>();
			gridLines.AddRange(axisXViewData.GridLines);
			gridLines.AddRange(axisYViewData.GridLines);
			return gridLines;
		}
		PrimitivesContainer GetPrimitivesContainer() {
			PrimitivesContainer container = new PrimitivesContainer();
			foreach(XYDiagram3DSeriesLayout seriesLayout in SeriesLayoutList)
				seriesLayout.FillPrimitivesContainer(container);
			foreach(XYDiagram3DSeriesLabelLayoutList labelLayoutList in LabelLayoutLists)
				labelLayoutList.FillPrimitivesContainer(container);
			return container;
		}
		GraphicsCommand CreateCachedGraphicsCommand() {
			return graphicsCache == null ? null : graphicsCache.CreateGraphicsCommand(coordsCalculator);
		}
		GraphicsCommand CreateDiagramGraphicsCommand() {
			GraphicsCommand container = new SaveStateGraphicsCommand();
			GraphicsCommand innerLightingCommand;
			container.AddChildCommand(CoordsCalculator.CreateDiagramLightingCommand(out innerLightingCommand));
			innerLightingCommand.AddChildCommand(CoordsCalculator.CreateModelViewGraphicsCommand());
			container.AddChildCommand(Diagram.BackImage.CreateGraphicsCommand(back == null ? inner.Fore : inner.Back, true));
			PrimitivesContainer primitives = new PrimitivesContainer(GetDiagramGridLines(), GetDiagramPolygons());
			PolygonGroups groups = GraphicsHelper.BuildGroups(primitives);
			if (groups != null) {
				groups.Sort();
				innerLightingCommand.AddChildCommand(groups.CreateGraphicsCommand());
			}
			return container;
		}
		GraphicsCommand CreateSeriesGraphicsCommand() {
			GraphicsCommand container = new SaveStateGraphicsCommand();
			GraphicsCommand innerLightingCommand;
			container.AddChildCommand(CoordsCalculator.CreateLightingCommand(out innerLightingCommand));
			innerLightingCommand.AddChildCommand(CoordsCalculator.CreateModelViewGraphicsCommand());
			innerLightingCommand.AddChildCommand(CreateCachedGraphicsCommand());
			return container;
		}
		GraphicsCommand CreateContentGraphicsCommand() {
			GraphicsCommand depthCommand = new DepthTestGraphicsCommand();
			depthCommand.AddChildCommand(CreateDiagramGraphicsCommand());
			depthCommand.AddChildCommand(CreateSeriesGraphicsCommand());
			return depthCommand;
		}
		void RenderAxisLabels(IRenderer renderer) {
			axisXViewData.RenderLabels(renderer);
			axisYViewData.RenderLabels(renderer);			
		}
		void RenderAnnotations(IRenderer renderer) {
			if (annotationsViewData != null) {
				foreach (AnnotationViewData annotationViewData in annotationsViewData)
					annotationViewData.Render(renderer);
			}
		}
		public override void CalculateSeriesAndLabelLayout(TextMeasurer textMeasurer, ChartDrawingHelper drawingHelper) {
			CalculateLayout(drawingHelper);
			if (IsLabelsResolveOverlapping)
				XYDiagramResolveOverlappingHelper.Process(GetLabelsForResolveOverlapping(), null, Diagram.LabelsResolveOverlappingMinIndent);
		}
		public override GraphicsCommand CreateGraphicsCommand() {
			GraphicsCommand command = new ContainerGraphicsCommand();
			GraphicsCommand viewPortCommand = new ViewPortGraphicsCommand(coordsCalculator.Bounds);
			command.AddChildCommand(viewPortCommand);
			GraphicsCommand depthCommand = new DepthTestGraphicsCommand();
			viewPortCommand.AddChildCommand(depthCommand);
			GraphicsCommand saveStateCommand = new SaveStateGraphicsCommand();
			depthCommand.AddChildCommand(saveStateCommand);
			viewPortCommand.AddChildCommand(saveStateCommand);
			saveStateCommand.AddChildCommand(new IdentityTransformGraphicsCommand(TransformMatrix.ModelView));
			saveStateCommand.AddChildCommand(new IdentityTransformGraphicsCommand(TransformMatrix.Projection));
			GraphicsCommand projectionCommand = coordsCalculator.CreateProjectionGraphicsCommand();
			saveStateCommand.AddChildCommand(projectionCommand);
			GraphicsCommand antialiasingCommand = new PolygonAntialiasingGraphicsCommand();
			projectionCommand.AddChildCommand(antialiasingCommand);
			antialiasingCommand.AddChildCommand(CreateContentGraphicsCommand());
			return command;
		}
		public override void Render(IRenderer renderer) {
		}
		public override void RenderMiddle(IRenderer renderer) {
		}
		public override void RenderAbove(IRenderer renderer) {
			renderer.SetClipping(Bounds);
			RenderAxisLabels(renderer);
			RenderSeriesLabels(renderer);
			RenderAnnotations(renderer);
			renderer.RestoreClipping();
		}
		public IList<PlanePolygon> GetDiagramPolygons() {
			RectangleFillStyle3D fillStyle = coordsCalculator.Diagram.ActualFillStyle;
			Color color = coordsCalculator.Diagram.ActualBackColor;
			List<PlanePolygon> polygons = new List<PlanePolygon>();
			ZPlaneRectangle gradientRect = new ZPlaneRectangle(inner.Fore.Location, inner.Fore.Width, inner.Fore.Height);
			if (left == null)
				polygons.AddRange(XYDiagram3DFillStyleHelper.FillPlanePolygons(right.GetRepresentation(), gradientRect, fillStyle, color, DiagramWeight));
			else
				polygons.AddRange(XYDiagram3DFillStyleHelper.FillPlanePolygons(left.GetRepresentation(), gradientRect, fillStyle, color, DiagramWeight));
			if (back == null)
				polygons.AddRange(XYDiagram3DFillStyleHelper.FillPlanePolygons(fore.GetRepresentation(), gradientRect, fillStyle, color, DiagramWeight));
			else
				polygons.AddRange(XYDiagram3DFillStyleHelper.FillPlanePolygons(back.GetRepresentation(), gradientRect, fillStyle, color, DiagramWeight));
			polygons.AddRange(XYDiagram3DFillStyleHelper.FillPlanePolygons(bottom.GetRepresentation(), gradientRect, fillStyle, color, DiagramWeight));
			if (coordsCalculator.Diagram.BackImage.Image != null) {
				PlaneRectangle rect = back == null ? inner.Fore : inner.Back;
				rect.SameColors = true;
				rect.Color = Color.Black;
				rect.Weight = BackImageWeight;
				rect.SetPaintingMethod(CreateImageBackgroundGraphicsCommand);
				polygons.Add(rect);
			}
			polygons.AddRange(axisXViewData.InterlacedPolygons);
			polygons.AddRange(axisYViewData.InterlacedPolygons);
			return polygons;
		}
	}
	public sealed class XYDiagram3DFillStyleHelper {
		public static IList<PlanePolygon> FillPlanePolygons(PlanePolygon[] rects, PlaneRectangle gradientRect, RectangleFillStyle3D fillStyle, Color color, int weight) {
			List<PlanePolygon> result = new List<PlanePolygon>();
			Color color2 = Color.Empty;
			if (fillStyle.FillMode == FillMode3D.Gradient) {
				RectangleGradientFillOptions fillOptions = (RectangleGradientFillOptions)fillStyle.Options;
				color2 = fillOptions.CalculateActualColor2(color, color2);
			}
			foreach (PlanePolygon rect in rects) {
				IList<PlanePolygon> polygons = fillStyle.FillPlanePolygon(rect, gradientRect, color, color2);
				foreach (PlanePolygon polygon in polygons) {
					polygon.Weight = weight;
					result.Add(polygon);
				}
			}
			return result;
		}
	}
}
