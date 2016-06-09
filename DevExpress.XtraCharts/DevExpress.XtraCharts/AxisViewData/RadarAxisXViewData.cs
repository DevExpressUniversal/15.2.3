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
	public class RadarAxisXViewData {
		static float CalcAngle(RadarDiagramMapping mapping, double argument) {
			return (float)(mapping.CalcAngle(argument) / Math.PI * 180.0);
		}
		readonly RadarAxisX axis;
		readonly GridAndTextDataEx gridAndTextData;
		readonly AxisLabelItemList labelItems;
		public GridAndTextDataEx GridAndTextData { get { return gridAndTextData; } }
		public RadarAxisX Axis { get { return axis; } }
		public AxisLabelItemList LabelItems { get { return labelItems; } }
		public RadarAxisXViewData(TextMeasurer textMeasurer, RadarAxisX axis, RadarDiagramMapping diagramMapping, GridAndTextDataEx gridAndTextData, int indent) {
			this.axis = axis;
			this.gridAndTextData = gridAndTextData;
			if (axis.Label.ActualVisibility) {
				int fullIndent = indent + 3;
				int labelsCount = gridAndTextData.TextData.PrimaryItems.Count - 1;
				labelItems = new AxisLabelItemList(labelsCount);
				for (int i = 0; i < labelsCount; i++) {
					AxisTextItem item = gridAndTextData.TextData.PrimaryItems[i];
					double value = item.Value;
					Point basePoint = (Point)diagramMapping.GetScreenPoint(value, Double.MaxValue, false, fullIndent);
					float angle = RotatedTextPainterBase.CancelAngle((float)(diagramMapping.CalcAngle(value) * 180.0 / Math.PI) + 90.0f);
					labelItems.Add(new RadarAxisXLabelItem(axis.Label, basePoint, item, angle));
				}
				labelItems.AdjustPropertiesAndCreatePainters(axis, textMeasurer);
			}
		}
		public void RenderLabels(IRenderer renderer) {
			if (labelItems != null)
				labelItems.Render(renderer, axis.Diagram.Chart.HitTestController);
		}
		public void UpdateCorrection(RectangleCorrection correction) {
			if (labelItems != null)
				labelItems.UpdateCorrection(correction);
		}
		static GridLineViewData[] CalculateViewData(RadarAxisXMapping axisMapping, List<double> values) {
			if (values.Count == 0) 
				return null;
			GridLineViewData[] viewData = new GridLineViewData[values.Count];
			for (int i = 0; i < values.Count; i++) {
				DiagramPoint point1 = axisMapping.GetNearScreenPoint(values[i]);
				DiagramPoint point2 = axisMapping.GetFarScreenPoint(values[i]);
				viewData[i] = new GridLineViewData(point1, point2);
			}
			return viewData;
		}
		static void RenderGridLines(IRenderer renderer, GridLineViewData[] viewData, Color color, LineStyle lineStyle) {
			if (viewData != null)
				GridLineViewData.RenderGridLines(renderer, viewData, color, lineStyle);
		}
		public void RenderGridLines(IRenderer renderer, RadarAxisXMapping axisMapping, DiagramAppearance appearance) {
			if (axis.GridLines.Visible)
				RenderGridLines(renderer, CalculateViewData(axisMapping, gridAndTextData.GridData.Items.VisibleValues), axis.GridLines.GetActualColor(appearance), axis.GridLines.LineStyle);
		}
		public void RenderMinorGridLines(IRenderer renderer, RadarAxisXMapping axisMapping, DiagramAppearance appearance) {
			if (axis.GridLines.MinorVisible)
				RenderGridLines(renderer, CalculateViewData(axisMapping, gridAndTextData.GridData.MinorValues), 
					axis.GridLines.GetActualMinorColor(appearance), axis.GridLines.MinorLineStyle);
		}
		void RenderCircleInterlacedAreas(IRenderer renderer, RadarDiagramMapping mapping) {
			PolygonFillStyle fillStyle = axis.ActualInterlacedFillStyle;
			Color color = axis.ActualInterlacedColor;
			AxisGridDataEx gridData = gridAndTextData.GridData;
			float radius = (float)mapping.Radius;
			foreach (InterlacedData data in gridData.InterlacedData) {
				float startAngle = CalcAngle(mapping, data.Near);
				float endAngle = CalcAngle(mapping, data.Far);
				float sweepAngle = endAngle - startAngle;
				RectangleF gradientBounds = GraphicUtils.BoundsFromPointsArray(GraphicUtils.FillAnchorPoints(mapping,
					MathUtils.Degree2Radian(startAngle), MathUtils.Degree2Radian(sweepAngle), true));
				fillStyle.Options.RenderPie(renderer, new PointF((float)mapping.Center.X, (float)mapping.Center.Y), radius, radius,
					startAngle, sweepAngle, 0, 0, gradientBounds, color, Color.Empty);
			}
		}
		void RenderPolygonInterlacedAreas(IRenderer renderer, RadarDiagramMapping mapping) {
			PolygonFillStyle fillStyle = axis.ActualInterlacedFillStyle;
			Color color = axis.ActualInterlacedColor;
			AxisGridDataEx gridData = gridAndTextData.GridData;
			foreach (InterlacedData data in gridData.InterlacedData) {
				LineStrip polygon = new LineStrip(3);
				polygon.Add(new GRealPoint2D(mapping.Center.X, mapping.Center.Y));
				DiagramPoint point = mapping.GetScreenPoint(data.Near, Double.MaxValue);
				polygon.Add(new GRealPoint2D(point.X, point.Y));
				point = mapping.GetScreenPoint(data.Far, Double.MaxValue);
				polygon.Add(new GRealPoint2D(point.X, point.Y));
				ZPlaneRectangle gradientBounds = (ZPlaneRectangle)GraphicUtils.BoundsFromPointsArray(polygon);
				if (fillStyle.FillMode == FillMode.Gradient) {
					PolygonGradientFillOptions fillOptions = (PolygonGradientFillOptions)fillStyle.Options;
					if (fillOptions.GradientMode == PolygonGradientMode.ToCenter ||
						fillOptions.GradientMode == PolygonGradientMode.FromCenter)
						gradientBounds = (ZPlaneRectangle)mapping.MappingBounds;
				}
				fillStyle.Options.Render(renderer, polygon, (RectangleF)gradientBounds, color, Color.Empty);
			}
		}
		public void RenderInterlacedAreas(IRenderer renderer, RadarDiagramMapping mapping) {
			if (!axis.Interlaced)
				return;
			if (axis.Diagram.DrawingStyle == RadarDiagramDrawingStyle.Circle)
				RenderCircleInterlacedAreas(renderer, mapping);
			else
				RenderPolygonInterlacedAreas(renderer, mapping);
		}
	}
}
