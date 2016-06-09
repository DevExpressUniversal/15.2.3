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
using System.Drawing.Drawing2D;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Native {
	[Flags]
	public enum ScrollBarPositions {
		None = 0,
		Left = 1,
		Right = 2,
		Bottom = 4,
		Top = 8
	}
	public class XYDiagramEmptyPaneViewData {
		readonly XYDiagramPaneBase pane;
		readonly List<AnnotationLayout> annotationsShapesLayout;
		readonly Rectangle bounds;
		readonly Rectangle maxBounds;
		public XYDiagramPaneBase Pane { get { return pane; } }
		public List<AnnotationLayout> AnnotationsShapesLayout { get { return annotationsShapesLayout; } }
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle MaxBounds { get { return maxBounds; } }
		public XYDiagramEmptyPaneViewData(XYDiagramPaneBase pane, List<AnnotationLayout> annotationsShapesLayout, Rectangle bounds, Rectangle maxBounds) {
			this.pane = pane;
			this.annotationsShapesLayout = annotationsShapesLayout;
			this.bounds = bounds;
			this.maxBounds = maxBounds;
		}
	}
	public class XYDiagramPaneViewData : PaneViewData {
		readonly XYDiagramPaneBase pane;
		readonly XYDiagramSeriesLayoutList seriesLayoutList;
		readonly IndicatorLayoutList indicatorsLayout;
		readonly XYDiagramMappingList diagramMappingList;
		readonly ScrollBarViewData xAxisScrollBarViewData;
		readonly ScrollBarViewData yAxisScrollBarViewData;
		readonly List<AxisViewData> axisViewDataList;
		readonly Rectangle maxBounds;
		readonly XYDiagramPaneAreas paneAreas;
		readonly List<AnnotationLayout> annotationsShapesLayout;
		readonly List<AnnotationLayout> annotationsAnchorPointsLayout;
		readonly List<AnnotationViewData> annotationsViewData;
		bool EnableScrolling { get { return pane.ActualEnableAxisXScrolling || pane.ActualEnableAxisYScrolling; } }
		bool EnableZooming { get { return pane.ActualEnableAxisXZooming || pane.ActualEnableAxisYZooming; } }
		public override List<AnnotationLayout> AnnotationsShapesLayout { get { return annotationsShapesLayout; } }
		public override List<AnnotationLayout> AnnotationsAnchorPointsLayout { get { return annotationsAnchorPointsLayout; } }
		public List<AnnotationViewData> AnnotationsViewData { get { return annotationsViewData; } }
		public new XYDiagram2D Diagram { get { return (XYDiagram2D)base.Diagram; } }
		public XYDiagramPaneBase Pane { get { return pane; } }
		public new XYDiagramSeriesLayoutList SeriesLayoutList { get { return seriesLayoutList; } }
		public XYDiagramMappingList DiagramMappingList { get { return diagramMappingList; } }
		public ScrollBarViewData XAxisScrollBarViewData { get { return xAxisScrollBarViewData; } }
		public ScrollBarViewData YAxisScrollBarViewData { get { return yAxisScrollBarViewData; } }
		public List<AxisViewData> AxisViewDataList { get { return axisViewDataList; } }
		public XYDiagramPaneAreas PaneAreas { get { return paneAreas; } }
		public IndicatorLayoutList IndicatorsLayout { get { return indicatorsLayout; } }
		public XYDiagramPaneViewData(XYDiagramPaneBase pane, XYDiagramSeriesLayoutList seriesLayoutList, List<SeriesLabelLayoutList> labelLayoutLists, XYDiagramMappingList diagramMappingList,
			ScrollBarViewData xAxisScrollBarViewData, ScrollBarViewData yAxisScrollBarViewData, List<AxisViewData> axisViewDataList, List<AnnotationLayout> annotationsShapesLayout,
			List<AnnotationLayout> annotationsAnchorPointsLayout, List<AnnotationViewData> annotationsViewData, Rectangle maxBounds)
			: base(pane.Diagram, diagramMappingList.MappingBounds, null, labelLayoutLists) {
			this.pane = pane;
			this.seriesLayoutList = seriesLayoutList;
			this.diagramMappingList = diagramMappingList;
			this.xAxisScrollBarViewData = xAxisScrollBarViewData;
			this.yAxisScrollBarViewData = yAxisScrollBarViewData;
			this.axisViewDataList = axisViewDataList;
			this.annotationsAnchorPointsLayout = annotationsAnchorPointsLayout;
			this.annotationsShapesLayout = annotationsShapesLayout;
			this.annotationsViewData = annotationsViewData;
			this.maxBounds = maxBounds;
			indicatorsLayout = new IndicatorLayoutList(diagramMappingList);
			paneAreas = new XYDiagramPaneAreas(Diagram.RaggedGeometry, Diagram.WavedGeometry, axisViewDataList,
				seriesLayoutList, Diagram.Chart.HitTestingEnabled);
		}
		void RenderAnnotations(IRenderer renderer) {
			if (annotationsViewData != null)
				foreach (AnnotationViewData annotationViewData in annotationsViewData)
					annotationViewData.Render(renderer);
		}
		void RenderBackConstantLines(IRenderer renderer) {
			foreach (AxisViewData axisViewData in axisViewDataList)
				axisViewData.RenderBackConstantLines(renderer, paneAreas.HitRegion);
		}
		void RenderBackConstantLinesTitles(IRenderer renderer) {
			foreach (AxisViewData axisViewData in axisViewDataList)
				axisViewData.RenderBackConstantLinesTitles(renderer);
		}
		void RenderFrontConstantLines(IRenderer renderer) {
			foreach (AxisViewData axisViewData in axisViewDataList)
				axisViewData.RenderFrontConstantLines(renderer, paneAreas.HitRegion);
		}
		void RenderFrontConstantLinesTitles(IRenderer renderer) {
			foreach (AxisViewData axisViewData in axisViewDataList)
				axisViewData.RenderFrontConstantLinesTitles(renderer);
		}
		void RenderStrips(IRenderer renderer) {
			foreach (AxisViewData axisViewData in axisViewDataList)
				axisViewData.RenderStrips(renderer);
		}
		void RenderInterlacedGraphics(IRenderer renderer) {
			foreach (AxisViewData axisViewData in axisViewDataList)
				axisViewData.RenderInterlacedGraphics(renderer);
		}
		void RenderGridLines(IRenderer renderer, DiagramAppearance appearance) {
			foreach (AxisViewData axisViewData in axisViewDataList)
				axisViewData.RenderGridLines(renderer, appearance);
		}
		void RenderAxes(IRenderer renderer) {
			foreach (AxisViewData axisViewData in axisViewDataList)
				axisViewData.Render(renderer);
		}
		void RenderAxesMiddle(IRenderer renderer) {
			foreach (AxisViewData axisViewData in axisViewDataList)
				axisViewData.RenderMiddle(renderer);
		}
		void RenderContent(IRenderer renderer) {
			renderer.SetClipping(paneAreas.ClipRegion);
			RenderInterlacedGraphics(renderer);
			RenderStrips(renderer);
			RenderGridLines(renderer, CommonUtils.GetActualAppearance(Diagram).XYDiagramAppearance);
			RenderBackConstantLines(renderer);
			renderer.RestoreClipping();
			foreach (XYDiagramSeriesLayout layout in seriesLayoutList) {
				Region seriesClipRegion = paneAreas.SeriesClipRegions[layout.DiagramMapping];
				if (seriesClipRegion != null) {
					renderer.SetClipping(seriesClipRegion);
					layout.RenderShadow(renderer);
					renderer.RestoreClipping();
				}
			}
			foreach (XYDiagramSeriesLayout layout in seriesLayoutList) {
				Region seriesClipRegion = paneAreas.SeriesClipRegions[layout.DiagramMapping];
				if (seriesClipRegion != null) {
					renderer.SetClipping(seriesClipRegion);
					layout.Render(renderer, paneAreas.GetSeriesHitRegion(layout.DiagramMapping));
					renderer.RestoreClipping();
				}
			}
		}
		Rectangle CalculateMappingBounds() {
			Rectangle mappingBounds = Bounds;
			mappingBounds.Width++;
			mappingBounds.Height++;
			return mappingBounds;
		}
		void RenderConstantLinesTitles(IRenderer renderer, Rectangle mappingBounds) {
			renderer.SetClipping(mappingBounds);
			RenderBackConstantLinesTitles(renderer);
			RenderFrontConstantLinesTitles(renderer);
			renderer.RestoreClipping();
		}
		void CalculateIndicatorsLayout(TextMeasurer textMeasurer) {
			List<Indicator> indicators = Diagram.GetIndicatorsByPane(Pane);
			if (indicators != null) {
				foreach (Indicator indicator in indicators) {
					if (indicator.ShouldBeDrawnOnDiagram)
						indicatorsLayout.Initialize(indicator, textMeasurer);
				}
			}
		}
		public void RenderIndicators(IRenderer renderer, HitTestController hitTestController, HitRegionContainer seriesHitRegion) {
			foreach (IndicatorLayout layout in indicatorsLayout) {
				if (!(layout.Indicator.OwningSeries.View is SwiftPlotSeriesViewBase)) {
					GraphicsPath hitGraphicsPath = layout.CalculateHitTestGraphicsPath();
					IHitRegion hitRegion;
					if (hitGraphicsPath == null)
						hitRegion = new HitRegion();
					else {
						hitRegion = new HitRegion(hitGraphicsPath);
						if (seriesHitRegion != null) {
							HitRegionContainer container = new HitRegionContainer(hitRegion);
							container.Intersect(seriesHitRegion);
							hitRegion = container.Underlying;
						}
					}
					renderer.ProcessHitTestRegion(hitTestController, layout.Indicator, null, hitRegion);
				}
			}
			foreach (IndicatorLayout layout in indicatorsLayout)
				layout.Render(renderer);
		}
		public override void CalculateSeriesAndLabelLayout(TextMeasurer textMeasurer, ChartDrawingHelper drawingHelper) {
			foreach (XYDiagramSeriesLayout seriesLayout in seriesLayoutList)
				seriesLayout.Calculate(textMeasurer);
			CalculateIndicatorsLayout(textMeasurer);
			paneAreas.UpdateClipRegions(IndicatorsLayout);
			IList<SeriesLabelLayoutList> labelsForResolveOverlapping = GetLabelsForResolveOverlapping();
			if (IsLabelsResolveOverlapping)
				if (pane.ResolveOverlappingCache != null)
					pane.ResolveOverlappingCache.Apply(labelsForResolveOverlapping);
				else {
					var bounds = (ZPlaneRectangle)((EnableScrolling || EnableZooming) ? diagramMappingList.MaximumMappingBounds : maxBounds);
					XYDiagramResolveOverlappingHelper.Process(labelsForResolveOverlapping, bounds, Diagram.LabelsResolveOverlappingMinIndent);
					pane.ResolveOverlappingCache = new ResolveOverlappingCache(labelsForResolveOverlapping);
				}
			foreach (XYDiagramSeriesLabelLayoutList labelLayoutList in labelsForResolveOverlapping)
				labelLayoutList.CalculateLabelsVisibility();
		}
		public override GraphicsCommand CreateGraphicsCommand() {
			return null;
		}
		public override void Render(IRenderer renderer) {
			Rectangle mappingBounds = CalculateMappingBounds();
			Pane.RenderBeforeContent(renderer, mappingBounds, maxBounds, paneAreas.ClipRegion);
			RenderContent(renderer);
			RenderAxes(renderer);
		}
		public override void RenderMiddle(IRenderer renderer) {
			foreach (IndicatorLayout layout in indicatorsLayout) {
				Region seriesClipRegion = paneAreas.SeriesClipRegions[layout.DiagramMapping];
				if (seriesClipRegion != null) {
					renderer.SetClipping(seriesClipRegion);
					RenderIndicators(renderer, Diagram.Chart.HitTestController, paneAreas.GetSeriesHitRegion(layout.DiagramMapping));
					renderer.RestoreClipping();
				}
			}
			renderer.SetClipping(paneAreas.ClipRegion);
			RenderFrontConstantLines(renderer);
			renderer.RestoreClipping();
			Rectangle mappingBounds = CalculateMappingBounds();
			if (EnableScrolling) {
				renderer.SetClipping(mappingBounds);
				RenderSeriesLabels(renderer);
				RenderAnnotations(renderer);
				renderer.RestoreClipping();
			}
			RenderConstantLinesTitles(renderer, mappingBounds);
			foreach (XYDiagramPaneArea paneArea in paneAreas)
				Pane.RenderBorder(renderer, paneArea);
			RenderAxesMiddle(renderer);
		}
		public override void RenderAbove(IRenderer renderer) {
			Rectangle mappingBounds = CalculateMappingBounds();
			if (XAxisScrollBarViewData != null)
				XAxisScrollBarViewData.Render(renderer);
			if (YAxisScrollBarViewData != null)
				YAxisScrollBarViewData.Render(renderer);
			if (!EnableScrolling) {
				RenderSeriesLabels(renderer);
				RenderAnnotations(renderer);
			}
		}
	}
	public abstract class XYDiagramPaneViewDataCalculator {
		static Rectangle FromLimits(Limits limitsH, Limits limitsV) {
			return new Rectangle(limitsH.Start, limitsV.Start, limitsH.Length, limitsV.Length);
		}
		static bool CheckLimits(Limits limits) {
			return limits.Length > 0;
		}
		public static XYDiagramPaneViewDataCalculator CreateInstance(TextMeasurer textMeasurer, XYDiagram2D diagram, XYDiagramPaneBase pane, PaneAxesContainer paneAxesData, List<RefinedSeriesData> seriesDataList, Rectangle maxBounds, Rectangle initialMappingBounds, LimitsCorrection mappingLimitsCorrectionGlobal, bool performRangeCorrection) {
			switch (diagram.PaneLayoutDirection) {
				case PaneLayoutDirection.Vertical:
					return new XYDiagramPaneViewDataVerticalCalculator(textMeasurer, diagram, pane,
						paneAxesData, seriesDataList, maxBounds, initialMappingBounds, mappingLimitsCorrectionGlobal, performRangeCorrection);
				case PaneLayoutDirection.Horizontal:
					return new XYDiagramPaneViewDataHorizontalCalculator(textMeasurer, diagram, pane,
						paneAxesData, seriesDataList, maxBounds, initialMappingBounds, mappingLimitsCorrectionGlobal, performRangeCorrection);
				default:
					throw new DefaultSwitchException();
			}
		}
		readonly TextMeasurer textMeasurer;
		readonly XYDiagram2D diagram;
		readonly XYDiagramPaneBase pane;
		readonly PaneAxesContainer paneAxesData;
		readonly List<RefinedSeriesData> seriesDataList;
		readonly List<AxisGridAndTextData> axisGridAndTextDataList = new List<AxisGridAndTextData>();
		readonly Rectangle maxBounds;
		readonly Rectangle initialMappingBounds;
		readonly LimitsCorrection mappingLimitsCorrectionGlobal;
		readonly LimitsCorrection mappingLimitsCorrectionLocal;
		readonly bool performRangeCorrection;
		List<SeriesLabelLayoutList> labelLayoutLists;
		AxisIntervalsLayoutRepository intervalsLayoutRepository;
		List<AxisViewData> axisViewDataList;
		List<AnnotationLayout> annotationsShapesLayout;
		List<AnnotationLayout> annotationsAnchorPointsLayout;
		List<AnnotationViewData> annotationsViewData;
		ScrollBarViewData xAxisScrollBarViewData;
		ScrollBarViewData yAxisScrollBarViewData;
		ScrollBarPositions scrollBarPositions;
		XYDiagramMappingList diagramMappingList;
		Limits mappingLimitsH;
		Limits mappingLimitsV;
		bool isNull = false;
		bool EnableScrolling { get { return pane.ActualEnableAxisXScrolling || pane.ActualEnableAxisYScrolling; } }
		Axis2D PrimaryAxisX { get { return (Axis2D)paneAxesData.PrimaryAxisX; } }
		Axis2D PrimaryAxisY { get { return (Axis2D)paneAxesData.PrimaryAxisY; } }
		Rectangle MappingBounds {
			get {
				Rectangle mappingBounds = FromLimits(mappingLimitsH, mappingLimitsV);
				mappingBounds.Width = Math.Max(mappingBounds.Width - 1, 0);
				mappingBounds.Height = Math.Max(mappingBounds.Height - 1, 0);
				return mappingBounds;
			}
		}
		Rectangle MaximumMappingBounds {
			get {
				if (double.IsNaN(PrimaryAxisX.WholeRangeData.Min) ||
					double.IsNaN(PrimaryAxisX.WholeRangeData.Max) ||
					double.IsNaN(PrimaryAxisY.WholeRangeData.Min) ||
					double.IsNaN(PrimaryAxisY.WholeRangeData.Max))
					return Rectangle.Empty;
				XYDiagramWholeMapping wholeMapping = new XYDiagramWholeMapping(MappingBounds, diagram.ActualRotated, PrimaryAxisX, PrimaryAxisY);
				DiagramPoint p1 = wholeMapping.GetDiagramPoint(PrimaryAxisX.WholeRangeData.Min, PrimaryAxisY.WholeRangeData.Min, false, true);
				DiagramPoint p2 = wholeMapping.GetDiagramPoint(PrimaryAxisX.WholeRangeData.Max, PrimaryAxisY.WholeRangeData.Max, false, true);
				Rectangle mappingBounds = wholeMapping.MappingBounds;
				p1 = MatrixTransform.Project(p1, mappingBounds);
				p2 = MatrixTransform.Project(p2, mappingBounds);
				return GraphicUtils.MakeRectangle((Point)p1, (Point)p2);
			}
		}
		protected LimitsCorrection MappingLimitsCorrectionGlobal { get { return mappingLimitsCorrectionGlobal; } }
		protected LimitsCorrection MappingLimitsCorrectionLocal { get { return mappingLimitsCorrectionLocal; } }
		protected abstract LimitsCorrection MappingLimitsCorrectionH { get; }
		protected abstract LimitsCorrection MappingLimitsCorrectionV { get; }
		public bool IsNull { get { return isNull; } }
		public AxisIntervalsLayoutRepository AxisIntervalsLayoutRepository { get { return intervalsLayoutRepository; } }
		public XYDiagramPaneViewDataCalculator(TextMeasurer textMeasurer, XYDiagram2D diagram, XYDiagramPaneBase pane, PaneAxesContainer paneAxesData,
			List<RefinedSeriesData> seriesDataList, Rectangle maxBounds, Rectangle initialMappingBounds, LimitsCorrection mappingLimitsCorrectionGlobal,
			bool performRangeCorrection) {
			this.textMeasurer = textMeasurer;
			this.diagram = diagram;
			this.pane = pane;
			this.paneAxesData = paneAxesData;
			this.seriesDataList = seriesDataList;
			this.maxBounds = maxBounds;
			this.initialMappingBounds = initialMappingBounds;
			FillAxisGridAndTextDataList(paneAxesData.AxesX);
			FillAxisGridAndTextDataList(paneAxesData.AxesY);
			this.mappingLimitsCorrectionLocal = new LimitsCorrection();
			this.mappingLimitsCorrectionGlobal = mappingLimitsCorrectionGlobal;
			this.performRangeCorrection = performRangeCorrection;
		}
		void FillAxisGridAndTextDataList(IList<IAxisData> axes) {
			foreach (Axis2D axis in axes)
				axisGridAndTextDataList.Add(new AxisGridAndTextData(axis, maxBounds));
		}
		void CalculateAnnotationsLayoutAndViewData() {
			annotationsAnchorPointsLayout = new List<AnnotationLayout>();
			if (pane.Diagram.Chart.AnnotationRepository.Count > 0) {
				foreach (Annotation annotation in pane.Annotations) {
					PaneAnchorPoint anchorPoint = annotation.AnchorPoint as PaneAnchorPoint;
					if (anchorPoint != null) {
						AxisXCoordinate xCoordinate = anchorPoint.AxisXCoordinate;
						AxisYCoordinate yCoordinate = anchorPoint.AxisYCoordinate;
						if (xCoordinate.Visible && yCoordinate.Visible) {
							XYDiagramMappingContainer mappingContainer = diagramMappingList.GetMappingContainer(xCoordinate.Axis, yCoordinate.Axis);
							if (mappingContainer != null) {
								double argument = xCoordinate.Value;
								double value = yCoordinate.Value;
								XYDiagramMappingBase mapping = (EnableScrolling && annotation.ScrollingSupported) ?
									mappingContainer.MappingForScrolling : mappingContainer.GetMapping(argument, value);
								if (mapping != null)
									annotationsAnchorPointsLayout.Add(new AnnotationLayout(annotation, mapping.GetScreenPoint(argument, value)));
							}
						}
					}
				}
				foreach (RefinedSeriesData seriesData in seriesDataList) {
					XYDiagramMappingContainer mappingContainer = diagramMappingList.GetMappingContainer(seriesData);
					if (mappingContainer != null)
						annotationsAnchorPointsLayout.AddRange(new XYDiagramAnchorPointLayoutList(seriesData, mappingContainer));
				}
			}
			annotationsViewData = AnnotationHelper.CreateInnerAnnotationsViewData(annotationsAnchorPointsLayout, textMeasurer);
			annotationsShapesLayout = AnnotationHelper.CreateFreAnnotationsShapesLayout(pane.ChartContainer.Chart.AnnotationRepository,
				pane, (ZPlaneRectangle)FromLimits(mappingLimitsH, mappingLimitsV));
		}
		void CalculateScrollBarViewData() {
			xAxisScrollBarViewData = null;
			yAxisScrollBarViewData = null;
			if (pane.ActualEnableAxisXScrolling && pane.ScrollBarOptions.XAxisScrollBarVisible && PaneAxesContainer.CanZoomOutAxis((IAxisData)PrimaryAxisX) && pane.Diagram.ScrollingOptions.HasWays)
				xAxisScrollBarViewData = new ScrollBarViewData(PrimaryAxisX, pane, MappingBounds, pane.ScrollBarOptions);
			if (pane.ActualEnableAxisYScrolling && pane.ScrollBarOptions.YAxisScrollBarVisible && PaneAxesContainer.CanZoomOutAxis((IAxisData)PrimaryAxisY) && pane.Diagram.ScrollingOptions.HasWays)
				yAxisScrollBarViewData = new ScrollBarViewData(PrimaryAxisY, pane, MappingBounds, pane.ScrollBarOptions);
		}
		void CalculateScrollBarPosition() {
			scrollBarPositions = ScrollBarPositions.None;
			if (xAxisScrollBarViewData != null)
				if (diagram.ActualRotated)
					scrollBarPositions |= pane.ScrollBarOptions.XAxisScrollBarAlignment == ScrollBarAlignment.Near ?
						ScrollBarPositions.Left : ScrollBarPositions.Right;
				else
					scrollBarPositions |= pane.ScrollBarOptions.XAxisScrollBarAlignment == ScrollBarAlignment.Near ?
						ScrollBarPositions.Bottom : ScrollBarPositions.Top;
			if (yAxisScrollBarViewData != null)
				if (diagram.ActualRotated)
					scrollBarPositions |= pane.ScrollBarOptions.YAxisScrollBarAlignment == ScrollBarAlignment.Near ?
						ScrollBarPositions.Bottom : ScrollBarPositions.Top;
				else
					scrollBarPositions |= pane.ScrollBarOptions.YAxisScrollBarAlignment == ScrollBarAlignment.Near ?
						ScrollBarPositions.Left : ScrollBarPositions.Right;
		}
		void CalculateScrollBarCorrection(RectangleCorrection correction) {
			if (xAxisScrollBarViewData != null)
				xAxisScrollBarViewData.CalculateDiagramBoundsCorrection(correction);
			if (yAxisScrollBarViewData != null)
				yAxisScrollBarViewData.CalculateDiagramBoundsCorrection(correction);
		}
		void CalculateAxesCorrection(RectangleCorrection correction) {
			foreach (AxisViewData axisViewData in axisViewDataList)
				axisViewData.CalculateDiagramBoundsCorrection(correction);
		}
		void CalculateLabelsLayout(TextMeasurer textMeasurer) {
			labelLayoutLists = new List<SeriesLabelLayoutList>();
			diagramMappingList = new XYDiagramMappingList(diagram, intervalsLayoutRepository, MaximumMappingBounds);
			foreach (RefinedSeriesData seriesData in seriesDataList) {
				XYDiagramMappingContainer mappingContainer = diagramMappingList.GetMappingContainer(seriesData);
				if (mappingContainer != null)
					labelLayoutLists.Add(new XYDiagramSeriesLabelLayoutList(seriesData, mappingContainer, textMeasurer));
			}
		}
		void CalculateSeriesLabelsCorrection(RectangleCorrection correction) {
			if (!pane.ActualEnableAxisXScrolling && !pane.ActualEnableAxisYScrolling)
				foreach (XYDiagramSeriesLabelLayoutList labelLayoutList in labelLayoutLists)
					labelLayoutList.CalculateDiagramBoundsCorrection(correction);
		}
		bool PrepareAndCheckMappingLimits() {
			if (MappingLimitsCorrectionH.ShouldCorrect)
				mappingLimitsH = MappingLimitsCorrectionH.Correct(new Limits(initialMappingBounds.Left, initialMappingBounds.Right));
			else
				mappingLimitsH = new Limits(initialMappingBounds.Left, initialMappingBounds.Right);
			if (MappingLimitsCorrectionV.ShouldCorrect)
				mappingLimitsV = MappingLimitsCorrectionV.Correct(new Limits(initialMappingBounds.Top, initialMappingBounds.Bottom));
			else
				mappingLimitsV = new Limits(initialMappingBounds.Top, initialMappingBounds.Bottom);
			return CheckLimits(mappingLimitsH) && CheckLimits(mappingLimitsV);
		}
		void UpdateCache() {
			if (axisViewDataList != null)
				foreach (AxisViewData axisViewData in axisViewDataList) {
					axisViewData.UpdateCache();
				}
		}
		public bool Calculate(TextMeasurer textMeasurer) {
			if (!PrepareAndCheckMappingLimits()) {
				isNull = true;
				return true;
			}
			CalculateScrollBarViewData();
			CalculateScrollBarPosition();
			intervalsLayoutRepository = new AxisIntervalsLayoutRepository(pane, MappingBounds);
			bool autoLayout = this.diagram.Chart != null ? this.diagram.Chart.AutoLayout : false;
			axisViewDataList = AxisViewDataCalculator.Calculate(textMeasurer, pane, intervalsLayoutRepository, paneAxesData, scrollBarPositions, axisGridAndTextDataList, true, autoLayout);
			RectangleCorrection correction = new RectangleCorrection(maxBounds);
			CalculateScrollBarCorrection(correction);
			CalculateAxesCorrection(correction);
			if (correction.ShouldCorrect) {
				MappingLimitsCorrectionH.ApplyHorizontalCorrection(correction);
				MappingLimitsCorrectionV.ApplyVerticalCorrection(correction);
				return false;
			}
			if (pane.ActualEnableAxisXScrolling || pane.ActualEnableAxisYScrolling) {
				CalculateLabelsLayout(textMeasurer);
				CalculateAnnotationsLayoutAndViewData();
				UpdateCache();
				return true;
			}
			if (!PrepareAndCheckMappingLimits()) {
				isNull = true;
				UpdateCache();
				return true;
			}
			CalculateLabelsLayout(textMeasurer);
			CalculateAnnotationsLayoutAndViewData();
			correction = new RectangleCorrection(maxBounds);
			CalculateSeriesLabelsCorrection(correction);
			AnnotationHelper.CalculateAnnotationsCorrection(annotationsViewData, correction);
			if (!correction.ShouldCorrect) {
				UpdateCache();
				return true;
			}
			MappingLimitsCorrectionH.ApplyHorizontalCorrection(correction);
			MappingLimitsCorrectionV.ApplyVerticalCorrection(correction);
			return false;
		}
		public Dictionary<IAxisData, MinMaxValues> CalculateCorrectedAxisRange() {
			if (performRangeCorrection && (pane.ActualEnableAxisXScrolling || pane.ActualEnableAxisYScrolling)) {
				bool rotated = diagram.ActualRotated;
				Rectangle mappingBounds = MappingBounds;
				List<OutOfBoundsCheckerEx> checkers = new List<OutOfBoundsCheckerEx>();
				checkers.Add(new OutOfBoundsCheckerEx(paneAxesData.PrimaryAxisX, !rotated, GraphicUtils.ConvertRect(mappingBounds),
					intervalsLayoutRepository.GetIntervalsLayout(paneAxesData.PrimaryAxisX)[0].Interval.WholeRange));
				foreach (IAxisData axis in paneAxesData.SecondaryAxesX)
					checkers.Add(new OutOfBoundsCheckerEx(axis, !rotated, GraphicUtils.ConvertRect(mappingBounds),
					intervalsLayoutRepository.GetIntervalsLayout(axis)[0].Interval.WholeRange));
				checkers.Add(new OutOfBoundsCheckerEx(paneAxesData.PrimaryAxisY, rotated, GraphicUtils.ConvertRect(mappingBounds),
					intervalsLayoutRepository.GetIntervalsLayout(paneAxesData.PrimaryAxisY)[0].Interval.WholeRange));
				foreach (IAxisData axis in paneAxesData.SecondaryAxesY)
					checkers.Add(new OutOfBoundsCheckerEx(axis, rotated, GraphicUtils.ConvertRect(mappingBounds),
					intervalsLayoutRepository.GetIntervalsLayout(axis)[0].Interval.WholeRange));
				foreach (XYDiagramSeriesLabelLayoutList labelLayoutList in labelLayoutLists)
					foreach (IBoundsProvider labelLayout in labelLayoutList)
						if (labelLayout.Enable) {
							foreach (OutOfBoundsCheckerEx checker in checkers)
								checker.CheckOutOfBounds(labelLayout.GetBounds());
						}
				foreach (IBoundsProvider viewData in annotationsViewData)
					if (viewData.Enable) {
						foreach (OutOfBoundsCheckerEx checker in checkers)
							checker.CheckOutOfBounds(viewData.GetBounds());
					}
				Dictionary<IAxisData, MinMaxValues> dictionary = new Dictionary<IAxisData, MinMaxValues>();
				foreach (OutOfBoundsCheckerEx checker in checkers)
					dictionary.Add(checker.Axis, checker.GetCorrectedWholeRange());
				return dictionary;
			}
			return null;
		}
		IDictionary<IAxisData, IMinMaxValues> SelectAxisIntervals(IAxisData axisX, IAxisData axisY, AxisIntervalsLayoutRepository intervalsLayoutRepository) {
			Dictionary<IAxisData, IMinMaxValues> dictionary = new Dictionary<IAxisData, IMinMaxValues>();
			dictionary.Add(axisX, intervalsLayoutRepository.GetIntervalsLayout(axisX)[0].Interval.WholeRange);
			dictionary.Add(axisY, intervalsLayoutRepository.GetIntervalsLayout(axisY)[0].Interval.WholeRange);
			return dictionary;
		}
		public XYDiagramPaneViewData CreateViewData() {
			if (isNull)
				return null;
			XYDiagramSeriesLayoutList seriesLayoutList = new XYDiagramSeriesLayoutList(diagramMappingList);
			foreach (RefinedSeriesData seriesData in seriesDataList)
				seriesLayoutList.Initialize(seriesData);
			return new XYDiagramPaneViewData(pane, seriesLayoutList, labelLayoutLists, diagramMappingList, xAxisScrollBarViewData,
				yAxisScrollBarViewData, axisViewDataList, annotationsShapesLayout, annotationsAnchorPointsLayout, annotationsViewData, maxBounds);
		}
	}
	public class XYDiagramPaneViewDataVerticalCalculator : XYDiagramPaneViewDataCalculator {
		protected override LimitsCorrection MappingLimitsCorrectionH { get { return MappingLimitsCorrectionGlobal; } }
		protected override LimitsCorrection MappingLimitsCorrectionV { get { return MappingLimitsCorrectionLocal; } }
		public XYDiagramPaneViewDataVerticalCalculator(TextMeasurer textMeasurer, XYDiagram2D diagram, XYDiagramPaneBase pane, PaneAxesContainer paneAxesData,
			List<RefinedSeriesData> seriesDataList, Rectangle maxBounds, Rectangle initialMappingBounds, LimitsCorrection mappingLimitsCorrectionGlobal,
			bool performRangeCorrection)
			: base(textMeasurer, diagram, pane, paneAxesData, seriesDataList, maxBounds, initialMappingBounds, mappingLimitsCorrectionGlobal, performRangeCorrection) {
		}
	}
	public class XYDiagramPaneViewDataHorizontalCalculator : XYDiagramPaneViewDataCalculator {
		protected override LimitsCorrection MappingLimitsCorrectionH { get { return MappingLimitsCorrectionLocal; } }
		protected override LimitsCorrection MappingLimitsCorrectionV { get { return MappingLimitsCorrectionGlobal; } }
		public XYDiagramPaneViewDataHorizontalCalculator(TextMeasurer textMeasurer, XYDiagram2D diagram, XYDiagramPaneBase pane, PaneAxesContainer paneAxesData,
			List<RefinedSeriesData> seriesDataList, Rectangle maxBounds, Rectangle initialMappingBounds, LimitsCorrection mappingLimitsCorrectionGlobal,
			bool performRangeCorrection)
			: base(textMeasurer, diagram, pane, paneAxesData, seriesDataList, maxBounds, initialMappingBounds, mappingLimitsCorrectionGlobal, performRangeCorrection) {
		}
	}
	public class XYDiagramEmptyPaneTextPainter {
		const int splitRectDistance = 10;
		static bool SplitRectByHeight(Rectangle rect, out Rectangle topHalfRect, out Rectangle bottomHalfRect) {
			topHalfRect = Rectangle.Empty;
			bottomHalfRect = Rectangle.Empty;
			int height = rect.Height;
			if (height <= 0)
				return false;
			height -= splitRectDistance;
			if (height <= 0)
				return false;
			int topHalfHeight = (int)Math.Floor((double)height / 2);
			int bottomHalfHeight = height - topHalfHeight;
			if (topHalfHeight <= 0 || bottomHalfHeight <= 0)
				return false;
			topHalfRect = new Rectangle(rect.X, rect.Top, rect.Width, topHalfHeight);
			bottomHalfRect = new Rectangle(rect.X, rect.Bottom - bottomHalfHeight, rect.Width, bottomHalfHeight);
			return true;
		}
		readonly bool designMode;
		readonly Color textColor;
		readonly PaneLayoutDirection layoutDirection;
		readonly ISupportTextAntialiasing antialiasing;
		public XYDiagramEmptyPaneTextPainter(ISupportTextAntialiasing antialiasing, bool designMode, Color textColor, PaneLayoutDirection layoutDirection) {
			this.designMode = designMode;
			this.textColor = textColor;
			this.layoutDirection = layoutDirection;
			this.antialiasing = antialiasing;
		}
		string GetDesignTimeText() {
			switch (layoutDirection) {
				case PaneLayoutDirection.Vertical:
					return ChartLocalizer.GetString(ChartStringId.MsgEmptyPaneTextForVerticalLayout);
				case PaneLayoutDirection.Horizontal:
					return ChartLocalizer.GetString(ChartStringId.MsgEmptyPaneTextForHorizontalLayout);
				default:
					throw new DefaultSwitchException();
			}
		}
		void RenderPaneName(IRenderer renderer, string paneName, Rectangle bounds, StringAlignment lineAlignment) {
			renderer.DrawBoundedText(
				paneName,
				new NativeFont(DefaultFonts.Tahoma12),
				textColor,
				antialiasing,
				bounds,
				StringAlignment.Center,
				lineAlignment);
		}
		void RenderDesignTime(IRenderer renderer, XYDiagramEmptyPaneViewData paneViewData) {
			Rectangle topBounds, bottomBounds;
			bool success = SplitRectByHeight(paneViewData.Bounds, out topBounds, out bottomBounds);
			if (!success)
				return;
			string designTimeText = GetDesignTimeText();
			RenderPaneName(renderer, paneViewData.Pane.Name, topBounds, StringAlignment.Far);
			renderer.DrawBoundedText(
				designTimeText,
				new NativeFont(DefaultFonts.Tahoma8),
				textColor,
				new AntialiasingSupport(false, antialiasing),
				bottomBounds,
				StringAlignment.Center,
				StringAlignment.Near);
		}
		void RenderRuntime(IRenderer renderer, XYDiagramEmptyPaneViewData paneViewData) {
			RenderPaneName(renderer, paneViewData.Pane.Name, paneViewData.Bounds, StringAlignment.Center);
		}
		public void Render(IRenderer renderer, XYDiagramEmptyPaneViewData paneViewData) {
			if (designMode)
				RenderDesignTime(renderer, paneViewData);
			else
				RenderRuntime(renderer, paneViewData);
		}
	}
}
