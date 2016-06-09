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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public static class SeriesLabelHelper {
		static DiagramPoint? CalculateAnchorPointWithScrolling(XYDiagramMappingBase mapping, double argument, MinMaxValues values) {
			if (mapping == null)
				return null;
			double centerValue = MinMaxValues.Intersection(mapping.AxisY.WholeRangeData, values).CalculateCenter();
			return Double.IsNaN(centerValue) ? null : (DiagramPoint?)mapping.GetScreenPoint(argument, centerValue);
		}
		static DiagramPoint? CalculateAnchorPoint(XYDiagramMappingContainer mappingContainer, double argument, MinMaxValues values) {
			AxisIntervalLayout layoutX = mappingContainer.GetIntervalLayoutX(argument);
			if (layoutX == null)
				return null;
			AxisIntervalLayoutMapping mapping = new AxisIntervalLayoutMapping(layoutX, mappingContainer.AxisX.ScaleTypeMap.Transformation);
			double interimX = mapping.GetInterimCoord(argument, false, true);
			IntMinMaxValues interimValues = mappingContainer.CalculateMinMaxInterimY(null, true, values.Min, values.Max);
			if (interimValues == null)
				return null;
			double interimY = (interimValues.MinValue + interimValues.MaxValue) / 2.0;
			DiagramPoint interimAnchorPoint = new DiagramPoint(interimX, interimY);
			return XYDiagramMappingHelper.InterimPointToScreenPoint(interimAnchorPoint, mappingContainer);
		}
		public static XYDiagramSeriesLabelLayout CalculateLayoutForCenterAreaPosition(XYDiagramSeriesLabelLayoutList xyLabelLayoutList, TextMeasurer textMeasurer, RefinedPointData pointData, SeriesLabelBase label, MinMaxValues values) {
			RefinedPoint refinedPoint = pointData.RefinedPoint;
			DiagramPoint? anchorPoint = xyLabelLayoutList.View.IsScrollingEnabled ?
				CalculateAnchorPointWithScrolling(xyLabelLayoutList.MappingContainer.MappingForScrolling, refinedPoint.Argument, values) :
				CalculateAnchorPoint(xyLabelLayoutList.MappingContainer, refinedPoint.Argument, values);
			if (anchorPoint == null)
				return null;
			SeriesLabelViewData labelViewData = pointData.LabelViewData[0];
			TextPainter textPainter = labelViewData.CreateTextPainterForCenterDrawing(label, textMeasurer, (DiagramPoint)anchorPoint);
			return XYDiagramSeriesLabelLayout.Create(pointData, pointData.DrawOptions.Color,
				textPainter, null, label.ResolveOverlappingMode, (DiagramPoint)anchorPoint);
		}
		public static XYDiagramSeriesLabelLayout CalculateLayoutForCenterBarPosition(XYDiagramSeriesLabelLayoutList labelLayoutList, TextMeasurer textMeasurer, BarData barData, RefinedPointData pointData, SeriesLabelViewData labelViewData, Color color) {
			return CalculateLayoutForInsideBarPosition(labelLayoutList, textMeasurer, barData, pointData, labelViewData, BarSeriesLabelPosition.Center, 0, color);
		}
		public static XYDiagramSeriesLabelLayout CalculateLayoutForInsideBarPosition(XYDiagramSeriesLabelLayoutList labelLayoutList, TextMeasurer textMeasurer, BarData barData, RefinedPointData pointData, SeriesLabelViewData labelViewData, BarSeriesLabelPosition position, int indent, Color color) {
			DiagramPoint? anchorPoint = labelLayoutList.View.IsScrollingEnabled ?
				barData.CalculateAnchorPointForInsideBarPositionWithScrolling(labelLayoutList.MappingContainer, labelLayoutList.Label, position, indent, labelViewData.TextSize) :
				barData.CalculateAnchorPointForInsideLabelPosition(labelLayoutList.MappingContainer, position, indent, labelViewData.TextSize, labelLayoutList.Label.Border.Thickness);
			if (anchorPoint == null)
				return null;
			RectangleF validRectangle = RectangleF.Empty;
			if (!labelLayoutList.View.IsScrollingEnabled)
				validRectangle = barData.GetTotalRect(labelLayoutList.MappingContainer);
			return XYDiagramSeriesLabelLayout.CreateWithValidRectangle(pointData, color,
				labelViewData.CreateTextPainterForCenterDrawing(labelLayoutList.Label, textMeasurer, (DiagramPoint)anchorPoint),
				null, labelLayoutList.Label.ResolveOverlappingMode, (DiagramPoint)anchorPoint, validRectangle);
		}
		public static RectangleF CalcAnchorHoleForPoint(DiagramPoint anchor, double lineLength) {
			return new RectangleF((PointF)DiagramPoint.Offset(anchor, -lineLength, -lineLength, 0), new SizeF((float)lineLength * 2, (float)lineLength * 2));
		}
		public static XYDiagramSeriesLabelLayout CalculateLayoutForPoint(XYDiagramSeriesLabelLayoutList xyLabelLayoutList, TextMeasurer textMeasurer, RefinedPointData pointData, SeriesLabelViewData labelViewData, SeriesLabelBase label, double pointValue, int angleDegree) {
			RefinedPoint refinedPoint = pointData.RefinedPoint;
			XYDiagramMappingBase mapping = xyLabelLayoutList.GetMapping(refinedPoint.Argument, pointValue);
			if (mapping == null)
				return null;
			double angle = XYDiagramMappingHelper.CorrectAngle(mapping, MathUtils.Degree2Radian(MathUtils.NormalizeDegree(angleDegree)));
			DiagramPoint startPoint = mapping.GetScreenPoint(refinedPoint.Argument, pointValue);
			DiagramPoint finishPoint = label.CalculateFinishPoint(angle, startPoint);
			TextPainter painter = labelViewData.CreateTextPainterForFlankDrawing(label, textMeasurer, finishPoint, angle);
			return XYDiagramSeriesLabelLayout.CreateWithExcludedRectangle(pointData,
				pointData.DrawOptions.Color, painter,
				label.ActualLineVisible ? new LineConnectorPainter(startPoint, finishPoint, angle, (ZPlaneRectangle)painter.BoundsWithBorder, true) : null,
				label.ResolveOverlappingMode, startPoint, CalcAnchorHoleForPoint(startPoint, label.LineLength));
		}
		public static DiagramPoint CalculateFinishPoint(double angle, DiagramPoint startPoint, double connectorLength) {
			return new DiagramPoint(startPoint.X + Math.Cos(angle) * connectorLength, startPoint.Y - Math.Sin(angle) * connectorLength, startPoint.Z);
		}
		public static RectangleF CalculateValidRectangleForCenterPosition(DiagramPoint centerPoint, double markerSize) {
			return new RectangleF(new PointF((float)(centerPoint.X - markerSize / 2), (float)(centerPoint.Y - markerSize / 2)), new SizeF((float)markerSize, (float)markerSize));
		}
		public static void CalculateLayoutForLine3DLabel(XYDiagram3DSeriesLabelLayoutList xyLabelLayoutList, TextMeasurer textMeasurer, RefinedPointData pointData, double value, double connectorAngle, DiagramVector connectorDirection) {
			SeriesLabelBase label = xyLabelLayoutList.Label;
			XYDiagram3DCoordsCalculator coordsCalculator = xyLabelLayoutList.CoordsCalculator;
			DiagramPoint startPoint = coordsCalculator.GetDiagramPoint(xyLabelLayoutList.View.Series, ((IXYPoint)pointData.RefinedPoint).Argument, value, false);
			DiagramPoint finishPoint = CalculateFinishPoint(-connectorAngle, startPoint, label.LineLength);
			DiagramPoint anchorPoint = coordsCalculator.Project(finishPoint);
			double angle = coordsCalculator.CalcConnectorAngle(connectorDirection);
			SeriesLabelViewData labelViewData = pointData.LabelViewData[0];
			TextPainter painter = labelViewData.CreateTextPainterForFlankDrawing(label, textMeasurer, anchorPoint, angle);
			ConnectorPainterBase connectorPainter;
			if (label.ActualLineVisible) {
				bool resolveOverlapping = label.ResolveOverlappingMode != ResolveOverlappingMode.None;
				if (resolveOverlapping) {
					startPoint = coordsCalculator.Project(startPoint);
					finishPoint = anchorPoint;
				}
				connectorPainter = new LineConnectorPainter(startPoint, finishPoint, angle, (ZPlaneRectangle)painter.BoundsWithBorder, resolveOverlapping);
			}
			else
				connectorPainter = null;
			XYDiagramSeriesLabelLayout layout = XYDiagramSeriesLabelLayout.Create(pointData, pointData.DrawOptions.Color,
				painter, connectorPainter, label.ResolveOverlappingMode, startPoint);
			xyLabelLayoutList.AddLabelLayout(layout);
		}
		public static void CalculateLayoutForStackedArea3DLabel(XYDiagram3DSeriesLabelLayoutList xyLabelLayoutList, TextMeasurer textMeasurer, RefinedPointData pointData, SeriesLabelViewData labelViewData, MinMaxValues values, double labelPosition) {
			if (Double.IsNaN(labelPosition))
				return;
			IXYPoint xyPoint = pointData.RefinedPoint;
			SeriesLabelBase label = xyLabelLayoutList.Label;
			XYDiagram3DCoordsCalculator coordsCalculator = xyLabelLayoutList.CoordsCalculator;
			DiagramPoint point = coordsCalculator.GetDiagramPoint(xyLabelLayoutList.View.Series, xyPoint.Argument, labelPosition, false);
			point.Z += ((coordsCalculator.CalcVisiblePlanes() & BoxPlane.Back) > 0 ? 1 : -1) * coordsCalculator.CalcSeriesWidth(xyLabelLayoutList.View) / 2.0;
			TextPainter textPainter = labelViewData.CreateTextPainterForCenterDrawing(label, textMeasurer, coordsCalculator.Project(point));
			XYDiagramSeriesLabelLayout layout = XYDiagramSeriesLabelLayout.Create(pointData, pointData.DrawOptions.Color,
				textPainter, null, label.ResolveOverlappingMode, point);
			xyLabelLayoutList.AddLabelLayout(layout);
		}
	}
}
