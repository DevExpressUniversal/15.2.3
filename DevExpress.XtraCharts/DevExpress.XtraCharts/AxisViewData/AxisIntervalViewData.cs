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
using DevExpress.Charts.Native;
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Native {	
	public class AxisIntervalViewData {
		const int axisLabelIndent = 3;
		readonly bool visible = true;
		readonly Axis2D axis;
		readonly AxisIntervalMapping intervalMapping;
		readonly GridAndTextDataEx gridAndTextData;
		readonly Rectangle paneIntervalBounds;
		readonly Rectangle axisIntervalBounds;
		readonly int nextElementOffset;
		readonly AxisLineViewData lineViewData;
		readonly AxisTickmarksViewData tickmarksViewData;
		readonly AxisTickmarksViewData crossTickmarksViewData;
		readonly AxisLabelViewData labelViewData;
		readonly List<GridLineViewData> majorGridLinesViewData;
		readonly List<GridLineViewData> minorGridLinesViewData;
		readonly List<ZPlaneRectangle> interlacedAreas;
		readonly List<ConstantLineViewData> backConstantLinesViewData = new List<ConstantLineViewData>();
		readonly List<ConstantLineViewData> frontConstantLinesViewData = new List<ConstantLineViewData>();
		readonly List<StripViewData> stripsViewData = new List<StripViewData>();
		public Rectangle PaneIntervalBounds { get { return paneIntervalBounds; } }
		public Rectangle AxisIntervalBounds { get { return axisIntervalBounds; } }
		public int NextElementOffset { get { return nextElementOffset + ((labelViewData != null) ? labelViewData.Size : 0); } }
		public AxisIntervalMapping IntervalMapping { get { return intervalMapping; } }
		public GridAndTextDataEx GridAndTextData { get { return gridAndTextData; } }
		public List<ConstantLineViewData> BackConstantLinesViewData { get { return backConstantLinesViewData; } }
		public List<ConstantLineViewData> FrontConstantLinesViewData { get { return frontConstantLinesViewData; } }
		public List<StripViewData> StripsViewData { get { return stripsViewData; } }
		public AxisLabelViewData LabelViewData { get { return labelViewData; } }
		public AxisIntervalViewData(TextMeasurer textMeasurer, Axis2D axis, AxisIntervalMapping intervalMapping, GridAndTextDataEx gridAndTextData, bool visible, int axisOffset, int elementsOffset) {
			this.axis = axis;
			this.intervalMapping = intervalMapping;
			this.gridAndTextData = gridAndTextData;
			this.visible = visible;
			paneIntervalBounds = CalculateBounds();
			AxisGridDataEx gridData = gridAndTextData.GridData;
			if (visible || axis.Visibility == DefaultBoolean.Default) {
				nextElementOffset = elementsOffset;
				lineViewData = new AxisLineViewData(axis, intervalMapping, axisOffset, nextElementOffset);
				nextElementOffset += axis.Thickness;
				Tickmarks tickmarks = axis.Tickmarks;
				if (tickmarks.Visible || tickmarks.MinorVisible) {
					tickmarksViewData = AxisTickmarksViewData.Create(axis, intervalMapping, axisOffset, nextElementOffset, gridData);
					nextElementOffset += tickmarks.MaxLength;
					if (tickmarks.CrossAxis)
						crossTickmarksViewData = AxisTickmarksViewData.CreateCrossAxis(axis, intervalMapping, axisOffset, gridData);
				}
				if (axis.Label.ActualVisibility) {
					nextElementOffset += axisLabelIndent;
					labelViewData = new AxisLabelViewData(textMeasurer, axis, intervalMapping, axisOffset, nextElementOffset, gridAndTextData.TextData);
				}
				if (axis.Visibility == DefaultBoolean.Default) {
					axisIntervalBounds = lineViewData.Bounds;
					if (tickmarksViewData != null) {
						Rectangle tickmarksBounds = tickmarksViewData.GetBounds();
						if (!tickmarksBounds.IsEmpty)
							axisIntervalBounds = Rectangle.Union(axisIntervalBounds, tickmarksBounds);
					}
					if (labelViewData != null) {
						Rectangle labelBounds = labelViewData.CalculateBounds();
						if (!labelBounds.IsEmpty)
							axisIntervalBounds = Rectangle.Union(axisIntervalBounds, labelBounds);
					}
				}
			}
			GridLines gridLines = axis.GridLines;
			if (gridLines.Visible)
				majorGridLinesViewData = CalculateGridLinesViewData(gridData.Items.VisibleValues);
			if (gridLines.MinorVisible)
				minorGridLinesViewData = CalculateGridLinesViewData(gridData.MinorValues);
			if (axis.Interlaced)
				interlacedAreas = CalculateInterlacedAreas();
		}
		List<GridLineViewData> CalculateGridLinesViewData(IList<double> values) {
			List<GridLineViewData> gridLinesViewData = new List<GridLineViewData>(values.Count);
			foreach (double value in values) {
				GridLineViewData viewData = new GridLineViewData(intervalMapping.GetNearScreenPoint(value, 0, 0),
																 intervalMapping.GetFarScreenPoint(value, 0, 0));
				gridLinesViewData.Add(viewData);
			}
			return gridLinesViewData;
		}
		List<ZPlaneRectangle> CalculateInterlacedAreas() {
			List<ZPlaneRectangle> interlacedAreas = new List<ZPlaneRectangle>();
			IMinMaxValues limits = intervalMapping.Limits;
			foreach (InterlacedData data in gridAndTextData.GridData.InterlacedData) {
				double value1 = data.Near;
				double value2 = data.Far;
				if (value1 < value2) {
					if (value1 < limits.Min)
						value1 = Double.NegativeInfinity;
					if (value2 > limits.Max)
						value2 = Double.PositiveInfinity;
					ZPlaneRectangle area = new ZPlaneRectangle(intervalMapping.GetNearScreenPoint(value1, 0, 0),
															   intervalMapping.GetFarScreenPoint(value2, 0, 0));
					interlacedAreas.Add(area);
				}
			}
			return interlacedAreas;
		}
		Rectangle CalculateBounds() {
			Point point1 = (Point)intervalMapping.GetNearScreenPoint(double.NegativeInfinity, 0, 0);
			Point point2 = (Point)intervalMapping.GetFarScreenPoint(double.PositiveInfinity, 0, 0);
			return GraphicUtils.MakeRectangle(point1, point2);
		}
		public void CalculateDiagramBoundsCorrection(RectangleCorrection correction) {
			if (visible && lineViewData != null) {
				lineViewData.CalculateDiagramBoundsCorrection(correction);
				if (tickmarksViewData != null)
					tickmarksViewData.CalculateDiagramBoundsCorrection(correction);
				if (crossTickmarksViewData != null)
					crossTickmarksViewData.CalculateDiagramBoundsCorrection(correction);
				if (labelViewData != null)
					labelViewData.CalculateDiagramBoundsCorrection(correction);
			}
		}
		public void Render(IRenderer renderer, AxisIntervalViewData previousIntervalViewData) {
			if (!visible || lineViewData == null || labelViewData == null)
				return;
			Rectangle previousPrimaryLabelItemBounds = previousIntervalViewData == null ? Rectangle.Empty :
				previousIntervalViewData.labelViewData.LastPrimaryLabelItemBounds;
			Rectangle previousStaggeredLabelItemBounds = previousIntervalViewData == null ? Rectangle.Empty :
				previousIntervalViewData.labelViewData.LastStaggeredLabelItemBounds;
			labelViewData.Render(renderer, previousPrimaryLabelItemBounds, previousStaggeredLabelItemBounds);
		}
		public void RenderMiddle(IRenderer renderer, AxisIntervalViewData previousIntervalViewData) {
			if (!visible || lineViewData == null)
				return;
			lineViewData.Render(renderer);
			if (tickmarksViewData != null)
				tickmarksViewData.Render(renderer);
			if (crossTickmarksViewData != null)
				crossTickmarksViewData.Render(renderer);
		}
		public void RenderInterlacedGraphics(IRenderer renderer) {
			if (interlacedAreas == null)
				return;
			foreach (ZPlaneRectangle interlacedArea in interlacedAreas)
				renderer.FillRectangle((RectangleF)interlacedArea, axis.ActualInterlacedColor, axis.ActualInterlacedFillStyle);
		}
		public void RenderStrips(IRenderer renderer) {
			foreach (StripViewData stripViewData in stripsViewData)
				stripViewData.Render(renderer);
		}
		public void RenderBackConstantLines(IRenderer renderer, HitRegionContainer paneHitContainer) {
			foreach (ConstantLineViewData constantLineViewData in backConstantLinesViewData)
				constantLineViewData.Render(renderer, paneHitContainer);
		}
		public void RenderBackConstantLinesTitles(IRenderer renderer) {
			foreach (ConstantLineViewData constantLineViewData in backConstantLinesViewData)
				constantLineViewData.RenderTitle(renderer);
		}
		public void RenderFrontConstantLines(IRenderer renderer, HitRegionContainer paneHitContainer) {
			foreach (ConstantLineViewData constantLineViewData in frontConstantLinesViewData)
				constantLineViewData.Render(renderer, paneHitContainer);
		}
		public void RenderFrontConstantLinesTitles(IRenderer renderer) {
			foreach (ConstantLineViewData constantLineViewData in frontConstantLinesViewData)
				constantLineViewData.RenderTitle(renderer);
		}
		public void RenderGridLines(IRenderer renderer, DiagramAppearance appearance) {
			GridLines gridLines = axis.GridLines;
			if (majorGridLinesViewData != null)
				GridLineViewData.RenderGridLines(renderer, majorGridLinesViewData, gridLines.GetActualColor(appearance), gridLines.LineStyle);
			if (minorGridLinesViewData != null)
				GridLineViewData.RenderGridLines(renderer, minorGridLinesViewData, gridLines.GetActualMinorColor(appearance), gridLines.MinorLineStyle);
		}
	}
}
