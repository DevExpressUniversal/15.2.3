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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public partial class CrosshairPaneViewData {
		readonly RectangleIndents labelIndents = new RectangleIndents(null) { Left = 4, Top = 1, Right = 4, Bottom = 1 };
		readonly CrosshairPaneInfoEx crosshairPaneInfo;
		readonly CrosshairSeriesLabel crosshairSeriesLabel;
		readonly TextMeasurer textMeasurer;
		readonly CrosshairDrawInfo drawInfo;
		readonly List<CrosshairHighlightedPointInfo> highlightedPointsInfo;
		List<CrosshairAxisLabelElement> crosshairAxisLabelElements;
		public CrosshairLineElement CrosshairLineElement {
			get { return drawInfo.CrosshairLineElement; }
			set { drawInfo.CrosshairLineElement = value; }
		}
		public List<CrosshairAxisLabelElement> CrosshairAxisLabelElements {
			get {
				if (crosshairAxisLabelElements == null)
					crosshairAxisLabelElements = GetCrosshairAxisLabelElements(drawInfo.CursorAxisLableDrawInfos);
				return crosshairAxisLabelElements;
			}
		}
		public List<CrosshairElementGroup> CrosshairElementGroups { get { return drawInfo.CrosshairElementGroups; } }
		public CrosshairPaneViewData(CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSeriesLabel crosshairSeriesLabel, TextMeasurer textMeasurer, CrosshairOptions crosshairOptions,
			List<CrosshairHighlightedPointInfo> highlightedPointsInfo) {
			this.crosshairPaneInfo = crosshairPaneInfo;
			this.crosshairSeriesLabel = crosshairSeriesLabel;
			this.textMeasurer = textMeasurer;
			this.highlightedPointsInfo = highlightedPointsInfo;
			drawInfo = new CrosshairDrawInfo(crosshairPaneInfo, crosshairOptions, crosshairSeriesLabel.TextAppearence);
		}
		void RenderCrosshairAxisLable(IRenderer renderer, CrosshairAxisInfo axisLableInfo, CrosshairAxisLabelElement axisLabelElement) {
			if (!axisLabelElement.Visible)
				return;
			RectangleF rect = GetRectangle(axisLableInfo, axisLabelElement);
			renderer.FillRectangle(rect, axisLabelElement.BackColor);
			using (NativeFont nativeFont = new NativeFont(axisLabelElement.Font))
				renderer.DrawBoundedText(axisLabelElement.Text, nativeFont, axisLabelElement.TextColor, axisLabelElement, rect, StringAlignment.Center, StringAlignment.Center);
		}
		RectangleF GetRectangle(CrosshairAxisInfo axisLableInfo, CrosshairAxisLabelElement axisLabelElement) {
			SizeF size = textMeasurer.MeasureString(axisLabelElement.Text, axisLabelElement.Font);
			axisLableInfo.Size = new GRealSize2D(size.Width, size.Height);
			GRealRect2D bounds = labelIndents.IncreaseRectangle(axisLableInfo.Bounds);
			RectangleF rect = new RectangleF((float)bounds.Left, (float)bounds.Top, (float)bounds.Width, (float)bounds.Height);
			return rect;
		}
		void RenderCrosshairLine(IRenderer renderer, CrosshairLine crosshairLine, CrosshairLineElement crosshairLineElement) {
			if (!crosshairLineElement.Visible)
				return;
			Color crosshairLineColor = crosshairLineElement.Color;
			LineStyle crosshairLineStyle = crosshairLineElement.LineStyle;
			renderer.DrawLine(CreatePointF(crosshairLine.Line.Start), CreatePointF(crosshairLine.Line.End), crosshairLineColor, crosshairLineStyle);
		}
		PointF CreatePointF(GRealPoint2D point) {
			return new PointF((float)point.X, (float)point.Y);
		}
		List<CrosshairAxisLabelElement> GetCrosshairAxisLabelElements(List<CrosshairDrawInfo.CrosshairAxisLableDrawInfo> cursorAxisLableDrawInfos) {
			List<CrosshairAxisLabelElement> list = new List<CrosshairAxisLabelElement>();
			foreach (CrosshairDrawInfo.CrosshairAxisLableDrawInfo axisLableDrawInfo in cursorAxisLableDrawInfos)
				list.Add(axisLableDrawInfo.CrosshairAxisLabelElement);
			return list;
		}
		public void CalculatePointsHighlightingVisibility() {
			foreach (CrosshairElementGroup group in CrosshairElementGroups)
				foreach (CrosshairElement element in group.CrosshairElements)
					if (!element.Visible)
						foreach (CrosshairHighlightedPointInfo highlightedInfo in highlightedPointsInfo)
							if (highlightedInfo.Series == element.Series) {
								highlightedInfo.Hide();
								break;
							}
		}
		public void RenderAxisLables(IRenderer renderer) {
			foreach (CrosshairDrawInfo.CrosshairPointDrawInfo pointDrawInfo in drawInfo.PointDrawInfos)
				if (pointDrawInfo.CrosshairElement.Visible)
					RenderCrosshairAxisLable(renderer, pointDrawInfo.CrosshairSeriesPoint.CrosshairAxisInfo, pointDrawInfo.CrosshairElement.AxisLabelElement);
		}
		public void RenderCursorAxisLables(IRenderer renderer) {
			foreach (CrosshairDrawInfo.CrosshairAxisLableDrawInfo axisLableDrawInfo in drawInfo.CursorAxisLableDrawInfos)
				RenderCrosshairAxisLable(renderer, axisLableDrawInfo.CrosshairAxisInfo, axisLableDrawInfo.CrosshairAxisLabelElement);
		}
		public void RenderLines(IRenderer renderer) {
			foreach (CrosshairDrawInfo.CrosshairPointDrawInfo pointDrawInfo in drawInfo.PointDrawInfos)
				if (pointDrawInfo.CrosshairElement.Visible)
					RenderCrosshairLine(renderer, pointDrawInfo.CrosshairSeriesPoint.CrosshairLine, pointDrawInfo.CrosshairElement.LineElement);
		}
		public void RenderCursorLines(IRenderer renderer) {
			if (drawInfo.CursorLineDrawInfo.CrosshairLine != null && drawInfo.CursorLineDrawInfo.CrosshairLineElement != null)
				RenderCrosshairLine(renderer, drawInfo.CursorLineDrawInfo.CrosshairLine, drawInfo.CursorLineDrawInfo.CrosshairLineElement);
		}
		public void RenderLables(IRenderer renderer) {
			crosshairSeriesLabel.Render(renderer, crosshairPaneInfo, textMeasurer, drawInfo.CrosshairElementGroups);
		}
		public void RenderHighlighting(IRenderer renderer) {
			foreach (CrosshairHighlightedPointInfo pointInfo in highlightedPointsInfo) {
				Series series = pointInfo.Series;
				if (pointInfo.Visible && series != null && series.ActualCrosshairHighlightPoints) {
					XYDiagram2DSeriesViewBase view = series.View as XYDiagram2DSeriesViewBase;
					Region seriesClipRegion = pointInfo.ClipRegion;
					if (view != null && seriesClipRegion != null) {
						renderer.SetClipping(seriesClipRegion);
						view.RenderHighlightedPoint(renderer, pointInfo.Layout);
						renderer.RestoreClipping();
					}
				}
			}
		}
		public void Render(IRenderer renderer) {
			RenderLines(renderer);
			RenderCursorLines(renderer);
		}
		public void RenderAbove(IRenderer renderer) {
			RenderAxisLables(renderer);
			RenderCursorAxisLables(renderer);
			RenderLables(renderer);
		}
	}
}
