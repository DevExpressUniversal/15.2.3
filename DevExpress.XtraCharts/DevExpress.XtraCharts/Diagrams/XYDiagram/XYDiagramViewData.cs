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

using System.Drawing;
using System.Collections.Generic;
using System.Collections.Specialized;
using DevExpress.Charts.Native;
using System;
namespace DevExpress.XtraCharts.Native {
	public class XYDiagramViewData : DiagramViewData {
		readonly XYDiagram2D diagram;
		readonly List<XYDiagramPaneViewData> paneViewDataList;
		readonly List<XYDiagramEmptyPaneViewData> emptyPaneViewDataList;
		XYDiagramPaneViewData DefaultPaneViewData { get { return paneViewDataList.Count > 0 ? paneViewDataList[0] : null; } }
		public List<XYDiagramPaneViewData> PaneViewDataList { get { return paneViewDataList; } }
		public List<XYDiagramEmptyPaneViewData> EmptyPaneViewDataList { get { return emptyPaneViewDataList; } }
		public Rectangle DefaultPaneMappingBounds {
			get {
				XYDiagramPaneViewData defaultPaneViewData = DefaultPaneViewData;
				return defaultPaneViewData != null ? defaultPaneViewData.Bounds : Rectangle.Empty;
			}
		}
		public override Rectangle LegendMappingBounds {
			get {
				XYDiagramPaneViewData defaultPaneViewData = DefaultPaneViewData;
				return defaultPaneViewData == null ? Rectangle.Empty : GraphicUtils.InflateRect(defaultPaneViewData.Bounds, -1, -1);
			}
		}
		public override IList<SeriesLabelLayoutList> LabelLayoutLists {
			get {
				List<SeriesLabelLayoutList> result = new List<SeriesLabelLayoutList>();
				foreach (XYDiagramPaneViewData viewData in paneViewDataList)
					result.AddRange(viewData.LabelLayoutLists);
				return result;
			}
		}
		public override List<AnnotationLayout> AnnotationsShapesLayout {
			get {
				List<AnnotationLayout> result = new List<AnnotationLayout>();
				foreach (XYDiagramPaneViewData paneViewData in PaneViewDataList)
					result.AddRange(paneViewData.AnnotationsShapesLayout);
				foreach (XYDiagramEmptyPaneViewData paneViewData in emptyPaneViewDataList)
					result.AddRange(paneViewData.AnnotationsShapesLayout);
				return result;
			}
		}
		public override List<AnnotationLayout> AnnotationsAnchorPointsLayout {
			get {
				List<AnnotationLayout> result = new List<AnnotationLayout>();
				foreach (XYDiagramPaneViewData paneViewData in PaneViewDataList)
					result.AddRange(paneViewData.AnnotationsAnchorPointsLayout);
				return result;
			}
		}
		public XYDiagramViewData(XYDiagram2D diagram, Rectangle bounds, List<XYDiagramPaneViewData> paneViewDataList, List<XYDiagramEmptyPaneViewData> emptyPaneViewDataList)
			: base(diagram, bounds) {
			this.diagram = diagram;
			this.paneViewDataList = paneViewDataList;
			this.emptyPaneViewDataList = emptyPaneViewDataList;
		}
		void RenderEmptyPane(IRenderer renderer, XYDiagramEmptyPaneViewData paneViewData) {
			Rectangle bounds = paneViewData.Bounds;
			if (!bounds.AreWidthAndHeightPositive())
				return;
			XYDiagramPaneBase pane = paneViewData.Pane;
			pane.RenderBeforeContent(renderer, bounds, paneViewData.MaxBounds, null);
			XYDiagramEmptyPaneTextPainter painer = new XYDiagramEmptyPaneTextPainter(pane, diagram.Chart.Container.DesignMode,
				diagram.Appearance.TextColor, diagram.PaneLayoutDirection);
			painer.Render(renderer, paneViewData);
			pane.RenderBorder(renderer, bounds);
		}
		public override void CalculateSeriesAndLabelLayout(TextMeasurer textMeasurer, ChartDrawingHelper drawingHelper) {
			foreach (XYDiagramPaneViewData paneViewData in PaneViewDataList)
				paneViewData.CalculateSeriesAndLabelLayout(textMeasurer, drawingHelper);
		}
		public override GraphicsCommand CreateGraphicsCommand() {
			return null;
		}
		public override void Render(IRenderer renderer) {
			foreach (XYDiagramEmptyPaneViewData emptyPaneViewData in EmptyPaneViewDataList)
				RenderEmptyPane(renderer, emptyPaneViewData);
			foreach (XYDiagramPaneViewData paneViewData in PaneViewDataList)
				paneViewData.Render(renderer);
		}
		public override void RenderMiddle(IRenderer renderer) {
			foreach (XYDiagramPaneViewData paneViewData in PaneViewDataList)
				paneViewData.RenderMiddle(renderer);
		}
		public override void RenderAbove(IRenderer renderer) {
			foreach (XYDiagramPaneViewData paneViewData in PaneViewDataList)
				paneViewData.RenderAbove(renderer);
		}
	}
	public class PaneSeriesRepository : Dictionary<XYDiagramPaneBase, List<RefinedSeriesData>> {
		public void Register(RefinedSeriesData seriesData) {
			XYDiagram2DSeriesViewBase view = seriesData.Series.View as XYDiagram2DSeriesViewBase;
			if (view != null) {
				XYDiagramPaneBase pane = view.ActualPane;
				List<RefinedSeriesData> seriesDataList;
				if (!TryGetValue(pane, out seriesDataList)) {
					seriesDataList = new List<RefinedSeriesData>();
					Add(pane, seriesDataList);
				}
				seriesDataList.Add(seriesData);
			}
		}
	}
	public class XYDiagramViewDataCalculator {
		static int GetShadowActualSize(XYDiagramPaneBase pane) {
			return pane.Shadow.GetActualSize(-1);
		}
		static PaneSeriesRepository CreatePaneSeriesRepository(IList<RefinedSeriesData> seriesDataList) {
			PaneSeriesRepository paneSeriesRepository = new PaneSeriesRepository();
			foreach (RefinedSeriesData seriesData in seriesDataList)
				paneSeriesRepository.Register(seriesData);
			return paneSeriesRepository;
		}
		static void PerformPaneViewDataCalculation(List<XYDiagramPaneViewDataCalculator> paneViewDataCalculatorList, TextMeasurer textMeasurer) {
			for (; ; ) {
				bool calculationComplete = true;
				foreach (XYDiagramPaneViewDataCalculator calculator in paneViewDataCalculatorList)
					if (calculator != null && !calculator.IsNull)
						calculationComplete &= calculator.Calculate(textMeasurer);
				if (calculationComplete)
					return;
			}
		}
		static void CorrectRanges(List<XYDiagramPaneViewDataCalculator> paneViewDataCalculatorList) {
			Dictionary<IAxisData, MinMaxValues> correctedRanges = new Dictionary<IAxisData, MinMaxValues>();
			foreach (XYDiagramPaneViewDataCalculator calculator in paneViewDataCalculatorList)
				if (calculator != null && !calculator.IsNull)
					Merge(correctedRanges, calculator.CalculateCorrectedAxisRange());
			foreach (IAxisData axis in correctedRanges.Keys) {
				List<AxisIntervalLayout> axisIntervalLayouts = GetAxisIntervalLayouts(paneViewDataCalculatorList, axis);
				ApplyRangesStates(axis, correctedRanges[axis], axisIntervalLayouts);
			}
		}
		static List<AxisIntervalLayout> GetAxisIntervalLayouts(List<XYDiagramPaneViewDataCalculator> paneViewDataCalculatorList, IAxisData axis) {
			List<AxisIntervalLayout> axisIntervalLayouts = new List<AxisIntervalLayout>();
			foreach (XYDiagramPaneViewDataCalculator calculator in paneViewDataCalculatorList)
				if (calculator != null && !calculator.IsNull)
					axisIntervalLayouts.AddRange(calculator.AxisIntervalsLayoutRepository.GetIntervalsLayout(axis));
			return axisIntervalLayouts;
		}
		static void ApplyRangesStates(IAxisData axis, MinMaxValues correctedRange, List<AxisIntervalLayout> axisIntervalLayouts) {
			if (!correctedRange.HasValues)
				return;
			IVisualAxisRangeData visualRange = axis.VisualRange;
			IWholeAxisRangeData wholeRange = axis.WholeRange;
			if (wholeRange.CorrectionMode == RangeCorrectionMode.Auto) {
				IIntervalContainer intervalContainer = axis as IIntervalContainer;
				foreach (AxisInterval interval in intervalContainer.Intervals) {
					interval.WholeRange.Min = correctedRange.Min;
					interval.WholeRange.Max = correctedRange.Max;
					if (visualRange.CorrectionMode == RangeCorrectionMode.Auto) {
						interval.Range.Min = correctedRange.Min;
						interval.Range.Max = correctedRange.Max;
					}
				}
			}
		}
		static void Merge(Dictionary<IAxisData, MinMaxValues> main, Dictionary<IAxisData, MinMaxValues> correctedRanges) {
			if (correctedRanges == null)
				return;
			foreach (IAxisData key in correctedRanges.Keys) {
				MinMaxValues range;
				if (main.TryGetValue(key, out range)) {
					range = range.Union(correctedRanges[key]);
					main.Remove(key);
					main.Add(key, range);
				}
				else {
					main.Add(key, correctedRanges[key]);
				}
			}
		}
		readonly XYDiagram2D diagram;
		readonly IList<IPane> actualPanes;
		public XYDiagramViewDataCalculator(XYDiagram2D diagram) {
			this.diagram = diagram;
			this.actualPanes = diagram.ActualPanes;
		}
		public XYDiagramViewData Calculate(TextMeasurer textMeasurer, Rectangle diagramBounds, IList<RefinedSeriesData> seriesDataList, bool performRangeCorrection) {
			if (actualPanes.Count == 0)
				return new XYDiagramViewData(diagram, diagramBounds, new List<XYDiagramPaneViewData>(), new List<XYDiagramEmptyPaneViewData>());
			if (!diagramBounds.AreWidthAndHeightPositive())
				return null;
			Rectangle maxBounds = CorrectBoundsByShadowAndMargins(diagramBounds);
			if (!maxBounds.AreWidthAndHeightPositive())
				return null;
			XYDiagramPaneLayoutCalculator paneLayoutCalculator = new XYDiagramPaneLayoutCalculator(actualPanes, diagram.PaneDistance, diagram.PaneLayoutDirection, diagramBounds, maxBounds);
			List<XYDiagramPaneLayout> paneLayoutList = paneLayoutCalculator.Calculate();
			if (paneLayoutList == null)
				return null;
			ChartDebug.Assert(paneLayoutList.Count == actualPanes.Count);
			if (performRangeCorrection || IsZoomIn(diagram.ActualAxisX))
				diagram.ActualAxisX.UpdateIntervals();
			if (performRangeCorrection || IsZoomIn(diagram.ActualAxisY))
				diagram.ActualAxisY.UpdateIntervals();
			if (performRangeCorrection) {
				diagram.ActualAxisX.UpdateIntervals();
				diagram.ActualAxisY.UpdateIntervals();
				foreach (Axis2D secondaryAxisY in diagram.ActualSecondaryAxesY)
					secondaryAxisY.UpdateIntervals();
				foreach (Axis2D secondaryAxisX in diagram.ActualSecondaryAxesX)
					secondaryAxisX.UpdateIntervals();
			}
			List<XYDiagramPaneViewDataCalculator> paneViewDataCalculatorList = PreparePaneViewDataCalculatorList(textMeasurer,
				paneLayoutList, seriesDataList, performRangeCorrection);
			ChartDebug.Assert(paneViewDataCalculatorList.Count == actualPanes.Count);
			PerformPaneViewDataCalculation(paneViewDataCalculatorList, textMeasurer);
			CorrectRanges(paneViewDataCalculatorList);
			Rectangle? mappingBounds = null;
			List<XYDiagramPaneViewData> paneViewDataList = CalculatePaneViewDataList(paneViewDataCalculatorList, ref mappingBounds);
			List<XYDiagramEmptyPaneViewData> emptyPaneViewDataList = CalculateEmptyPaneViewDataList(paneViewDataCalculatorList,
				paneLayoutList, mappingBounds);
			return mappingBounds == null && emptyPaneViewDataList.Count == 0 ? null :
				new XYDiagramViewData(diagram, diagramBounds, paneViewDataList, emptyPaneViewDataList);
		}
		bool IsZoomIn(IAxisData axis) {
			return axis.VisualRange.Delta != axis.WholeRange.Delta;
		}
		XYDiagramPaneBase GetMaxShadowPane() {
			ChartDebug.Assert(actualPanes.Count > 0);
			XYDiagramPaneBase maxShadowPane = (XYDiagramPaneBase)actualPanes[0];
			int maxShadowSize = GetShadowActualSize(maxShadowPane);
			for (int i = 1; i < actualPanes.Count; i++) {
				XYDiagramPaneBase pane = (XYDiagramPaneBase)actualPanes[i];
				int size = GetShadowActualSize(pane);
				if (size > maxShadowSize) {
					maxShadowPane = pane;
					maxShadowSize = size;
				}
			}
			return maxShadowPane;
		}
		Rectangle CorrectBoundsByShadowAndMargins(Rectangle bounds) {
			XYDiagramPaneBase maxShadowPane = GetMaxShadowPane();
			if (maxShadowPane.Shadow.Visible)
				return new Rectangle(diagram.Margins.DecreaseRectangle(bounds).Location, maxShadowPane.Shadow.DecreaseSize(diagram.Margins.DecreaseRectangle(bounds).Size));
			return diagram.Margins.DecreaseRectangle(bounds);
		}
		List<XYDiagramPaneViewData> CalculatePaneViewDataList(List<XYDiagramPaneViewDataCalculator> paneViewDataCalculatorList, ref Rectangle? mappingBounds) {
			List<XYDiagramPaneViewData> paneViewDataList = new List<XYDiagramPaneViewData>();
			for (int i = 0; i < paneViewDataCalculatorList.Count; i++) {
				XYDiagramPaneViewDataCalculator calculator = paneViewDataCalculatorList[i];
				if (calculator != null) {
					XYDiagramPaneViewData paneViewData = calculator.CreateViewData();
					if (paneViewData != null) {
						if (mappingBounds == null)
							mappingBounds = paneViewData.Bounds;
						paneViewDataList.Add(paneViewData);
						XYDiagramPaneBase pane = (XYDiagramPaneBase)actualPanes[i];
						pane.LastMappingBounds = paneViewData.Bounds;
						pane.MappingList = paneViewData.DiagramMappingList;
						pane.SeriesClipRegions = paneViewData.PaneAreas.SeriesClipRegions;
					}
					else
						((XYDiagramPaneBase)actualPanes[i]).MappingList = null;
				}
			}
			return paneViewDataList;
		}
		List<XYDiagramEmptyPaneViewData> CalculateEmptyPaneViewDataList(List<XYDiagramPaneViewDataCalculator> paneViewDataCalculatorList, List<XYDiagramPaneLayout> paneLayoutList, Rectangle? mappingBounds) {
			AnnotationRepository annotations = diagram.Chart.AnnotationRepository;
			List<XYDiagramEmptyPaneViewData> emptyPaneViewDataList = new List<XYDiagramEmptyPaneViewData>();
			for (int i = 0; i < paneViewDataCalculatorList.Count; i++) {
				XYDiagramPaneViewDataCalculator calculator = paneViewDataCalculatorList[i];
				if (calculator == null) {
					Rectangle paneBounds = mappingBounds == null ? paneLayoutList[i].InitialMappingBounds :
						CreateEmptyPaneBounds((Rectangle)mappingBounds, paneLayoutList[i].InitialMappingBounds);
					XYDiagramPaneBase pane = (XYDiagramPaneBase)actualPanes[i];
					List<AnnotationLayout> annotationsShapeLayout = AnnotationHelper.CreateFreAnnotationsShapesLayout(annotations,
						pane, (ZPlaneRectangle)paneBounds);
					emptyPaneViewDataList.Add(new XYDiagramEmptyPaneViewData(pane, annotationsShapeLayout, paneBounds, paneLayoutList[i].MaxBounds));
					pane.LastMappingBounds = null;
					pane.MappingList = null;
				}
			}
			return emptyPaneViewDataList;
		}
		Rectangle CreateEmptyPaneBounds(Rectangle mappingBounds, Rectangle emptyMappingBounds) {
			switch (diagram.PaneLayoutDirection) {
				case PaneLayoutDirection.Vertical:
					return new Rectangle(mappingBounds.X, emptyMappingBounds.Y, mappingBounds.Width + 1, emptyMappingBounds.Height);
				case PaneLayoutDirection.Horizontal:
					return new Rectangle(emptyMappingBounds.X, mappingBounds.Y, emptyMappingBounds.Width, mappingBounds.Height + 1);
				default:
					throw new DefaultSwitchException();
			}
		}
		List<XYDiagramPaneViewDataCalculator> PreparePaneViewDataCalculatorList(TextMeasurer textMeasurer, List<XYDiagramPaneLayout> paneLayoutList, IList<RefinedSeriesData> seriesDataList, bool performRangeCorrection) {
			List<XYDiagramPaneViewDataCalculator> paneViewDataCalculatorList = new List<XYDiagramPaneViewDataCalculator>();
			PaneSeriesRepository paneSeriesRepository = CreatePaneSeriesRepository(seriesDataList);
			LimitsCorrection mappingLimitsCorrectionGlobal = new LimitsCorrection();
			for (int i = 0; i < actualPanes.Count; i++) {
				XYDiagramPaneBase pane = (XYDiagramPaneBase)actualPanes[i];
				PaneAxesContainer paneAxesData = diagram.GetPaneAxesData(pane);
				if (paneAxesData == null)
					paneViewDataCalculatorList.Add(null);
				else {
					List<RefinedSeriesData> paneSeriesDataList;
					if (!paneSeriesRepository.TryGetValue(pane, out paneSeriesDataList))
						paneSeriesDataList = new List<RefinedSeriesData>();
					XYDiagramPaneViewDataCalculator calculator = XYDiagramPaneViewDataCalculator.CreateInstance(textMeasurer, diagram,
						pane, paneAxesData, paneSeriesDataList, paneLayoutList[i].MaxBounds, paneLayoutList[i].InitialMappingBounds,
						mappingLimitsCorrectionGlobal, performRangeCorrection);
					paneViewDataCalculatorList.Add(calculator);
				}
			}
			return paneViewDataCalculatorList;
		}
	}
}
