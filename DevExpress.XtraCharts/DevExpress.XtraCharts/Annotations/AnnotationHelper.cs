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
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Native {
	public static class AnnotationHelper {
		class PaneDistanceDescription {
			readonly AxisXBase axisX;
			readonly AxisYBase axisY;
			readonly AxisAlignment horizontalAlignment;
			readonly AxisAlignment verticalAlignment;
			readonly int distance;
			public AxisXBase AxisX {
				get { return axisX; }
			}
			public AxisYBase AxisY {
				get { return axisY; }
			}
			public AxisAlignment HorizontalAlignment {
				get { return horizontalAlignment; } 
			}
			public AxisAlignment VerticalAlignment {
				get { return verticalAlignment; }
			}
			public int Distance {
				get { return distance; }
			}
			public PaneDistanceDescription(AxisXBase axisX, AxisYBase axisY, AxisAlignment horizontalAlignment, AxisAlignment verticalAlignment, int distance) {
				this.axisX = axisX;
				this.axisY = axisY;
				this.horizontalAlignment = horizontalAlignment;
				this.verticalAlignment = verticalAlignment;
				this.distance = distance;
			}
		}
		static PaneDistanceDescription CalculatePaneDistanceDescription(XYDiagram2D diagram, XYDiagramPaneBase pane, Point point) {
			Rectangle? bounds = pane.LastMappingBounds;
			if (bounds.HasValue) {
				PaneAxesContainer paneAxesData = diagram.GetPaneAxesData(pane);
				if (paneAxesData != null) {
					AxisXBase axisX = paneAxesData.PrimaryAxisX as AxisXBase;
					AxisYBase axisY = paneAxesData.PrimaryAxisY as AxisYBase;
					if (axisX != null && axisY != null) {
						Rectangle rect = bounds.Value;
						AxisAlignment horizontalAlignment;
						int horizontalDistance;
						int x = point.X;
						int leftDistance = Math.Abs(x - rect.Left);
						int rightDistance = Math.Abs(x - rect.Right);
						if (leftDistance <= rightDistance) {
							horizontalAlignment = AxisAlignment.Near;
							horizontalDistance = leftDistance;
						}
						else {
							horizontalAlignment = AxisAlignment.Far;
							horizontalDistance = rightDistance;
						}
						AxisAlignment verticalAlignment;
						int verticalDistance;
						int y = point.Y;
						int topDistance = Math.Abs(y - rect.Top);
						int bottomDistance = Math.Abs(y - rect.Bottom);
						if (bottomDistance <= topDistance) {
							verticalAlignment = AxisAlignment.Near;
							verticalDistance = bottomDistance;
						}
						else {
							verticalAlignment = AxisAlignment.Far;
							verticalDistance = topDistance;
						}
						return new PaneDistanceDescription(axisX, axisY, horizontalAlignment, verticalAlignment, Math.Min(horizontalDistance, verticalDistance));
					}
				}
			}
			return new PaneDistanceDescription(null, null, AxisAlignment.Near, AxisAlignment.Near, Int32.MaxValue);
		}
		public static Size CalculateAnnotationSize(RefinedSeriesData seriesData) {
			Size result = new Size(0, 0);
			foreach (RefinedPointData pointData in seriesData) {
				SeriesPoint seriesPoint = pointData.SeriesPoint as SeriesPoint;
				if (seriesPoint != null && seriesPoint.Annotations.Count > 0) {
					foreach (Annotation annotation in seriesPoint.Annotations) {
						RelativePosition shapePosition = annotation.ShapePosition as RelativePosition;
						if (annotation.ActualLabelMode && shapePosition != null) {
							DiagramPoint shapeLocation = shapePosition.GetShapeLocation(new DiagramPoint(0, 0));
							ZPlaneRectangle bounds = MathUtils.CalcBounds(new ZPlaneRectangle(shapeLocation, annotation.Width, annotation.Height), annotation.Angle);
							double left = Math.Min(0, bounds.Left);
							double right = Math.Max(0, bounds.Right);
							double top = Math.Max(0, bounds.Top);
							double bottom = Math.Min(0, bounds.Bottom);
							result.Width = Math.Max(result.Width, MathUtils.StrongRound(right - left));
							result.Height = Math.Max(result.Height, MathUtils.StrongRound(top - bottom));
						}
					}
				}
			}
			return result;
		}
		public static void CalculateAnnotationsCorrection(List<AnnotationViewData> annotationsViewData, RectangleCorrection correction) {
			foreach (AnnotationViewData viewData in annotationsViewData)
				viewData.CalculateDiagramBoundsCorrection(correction);
		}
		public static int CompareByAnnotationZOrder(AnnotationViewData x, AnnotationViewData y) {
			if (x == null) {
				if (y == null)
					return 0;
				else
					return -1;
			}
			else {
				if (y == null)
					return 1;
				else {
					if (x.Annotation.ZOrder > y.Annotation.ZOrder)
						return 1;
					else {
						if (x.Annotation.ZOrder < y.Annotation.ZOrder)
							return -1;
						else
							return x.IndexInRepository.CompareTo(y.IndexInRepository);
					}
				}
			}
		}
		public static ChartAnchorPoint CreateChartAnchorPoint(Annotation annotation, Chart chart) {
			ChartAnchorPoint anchorPoint = new ChartAnchorPoint();
			if (annotation != null && chart != null)
				anchorPoint.SetPosition((int)annotation.LastAnchorPosition.X - chart.Border.ActualThickness,
					(int)annotation.LastAnchorPosition.Y - chart.Border.ActualThickness);
			return anchorPoint;
		}
		public static PaneAnchorPoint CreatePaneAnchorPoint(Annotation annotation, Chart chart) {
			PaneAnchorPoint anchorPoint = new PaneAnchorPoint();
			if (annotation != null && chart != null) {
				XYDiagram2D diagram = chart.Diagram as XYDiagram2D;
				if (diagram != null) {
					Point point = (Point)annotation.LastAnchorPosition;
					DiagramCoordinates coordinates = diagram.PointToDiagram(point);
					if (coordinates.IsEmpty) {
						XYDiagramPaneBase nearestPane = diagram.DefaultPane;
						PaneDistanceDescription nearestPaneDescription = CalculatePaneDistanceDescription(diagram, nearestPane, point);
						foreach (XYDiagramPaneBase pane in diagram.ActualPanes) {
							PaneDistanceDescription paneDescription = CalculatePaneDistanceDescription(diagram, pane, point);
							if (paneDescription.Distance < nearestPaneDescription.Distance) {
								nearestPane = pane;
								nearestPaneDescription = paneDescription;
							}
						}
						anchorPoint.Pane = nearestPane;
						AxisAlignment axisXAlignment;
						AxisAlignment axisYAlignment;
						if (diagram.ActualRotated) {
							axisXAlignment = nearestPaneDescription.VerticalAlignment;
							axisYAlignment = nearestPaneDescription.HorizontalAlignment;
						}
						else {
							axisXAlignment = nearestPaneDescription.HorizontalAlignment;
							axisYAlignment = nearestPaneDescription.VerticalAlignment;
						}
						AxisXBase axisX = nearestPaneDescription.AxisX ?? (AxisXBase)diagram.ActualAxisX;
						anchorPoint.AxisXCoordinate.Axis = axisX;
						IVisualAxisRangeData axisXRange = axisX.VisualRangeData;
						object axisXValue;
						if (axisXAlignment == AxisAlignment.Far ^ axisX.Reverse) {
							axisXValue = axisXRange.MaxValue;
							if (axisXValue == null)
								axisXValue = ((IAxisData)axisX).AxisScaleTypeMap.InternalToNative(axisXRange.Max);
						}
						else {
							axisXValue = axisXRange.MinValue;
							if (axisXValue == null)
								axisXValue = ((IAxisData)axisX).AxisScaleTypeMap.InternalToNative(axisXRange.Min);
						}
						anchorPoint.AxisXCoordinate.AxisValue = axisXValue;
						AxisYBase axisY = nearestPaneDescription.AxisY ?? (AxisYBase)diagram.ActualAxisY;
						anchorPoint.AxisYCoordinate.Axis = axisY;
						IVisualAxisRangeData axisYRange = axisY.VisualRangeData;
						object axisYValue;
						if (axisYAlignment == AxisAlignment.Far ^ axisY.Reverse) {
							axisYValue = axisYRange.MaxValue;
							if (axisYValue == null)
								axisYValue = ((IAxisData)axisY).AxisScaleTypeMap.InternalToNative(axisYRange.Max);
						}
						else {
							axisYValue = axisYRange.MinValue;
							if (axisYValue == null)
								axisYValue = ((IAxisData)axisY).AxisScaleTypeMap.InternalToNative(axisYRange.Min);
						}
						anchorPoint.AxisYCoordinate.AxisValue = axisYValue;
					}
					else {
						anchorPoint.Pane = coordinates.Pane;
						anchorPoint.AxisXCoordinate.Axis = (AxisXBase)coordinates.AxisX;
						switch (coordinates.ArgumentScaleType) {
							case ScaleType.Numerical:
								anchorPoint.AxisXCoordinate.AxisValue = coordinates.NumericalArgument;
								break;
							case ScaleType.Qualitative:
								anchorPoint.AxisXCoordinate.AxisValue = coordinates.QualitativeArgument;
								break;
							case ScaleType.DateTime:
								anchorPoint.AxisXCoordinate.AxisValue = coordinates.DateTimeArgument;
								break;
						}
						anchorPoint.AxisYCoordinate.Axis = (AxisYBase)coordinates.AxisY;
						switch (coordinates.ValueScaleType) {
							case ScaleType.Numerical:
								anchorPoint.AxisYCoordinate.AxisValue = coordinates.NumericalValue;
								break;
							case ScaleType.DateTime:
								anchorPoint.AxisYCoordinate.AxisValue = coordinates.DateTimeValue;
								break;
						}
					}
				}
			}
			return anchorPoint;
		}
		public static FreePosition CreateFreePosition(Annotation annotation, Chart chart) {
			FreePosition position = new FreePosition();
			if (annotation != null && chart != null) {
				position.DockCorner = DockCorner.LeftTop;
				position.DockTarget = null;
				if (annotation.LastLocation.X > chart.Border.ActualThickness) {
					position.OuterIndents.Left = 0;
					position.InnerIndents.Left = (int)annotation.LastLocation.X - chart.Border.ActualThickness;
				}
				else {
					position.OuterIndents.Left = annotation.LastLocation.X > 0 ? (int)annotation.LastLocation.X + chart.Border.ActualThickness
						: chart.Border.ActualThickness - (int)annotation.LastLocation.X;
					position.InnerIndents.Left = 0;
				}
				if (annotation.LastLocation.Y > chart.Border.ActualThickness) {
					position.OuterIndents.Top = 0;
					position.InnerIndents.Top = (int)annotation.LastLocation.Y - chart.Border.ActualThickness;
				}
				else {
					position.OuterIndents.Top = annotation.LastLocation.Y > 0 ? (int)annotation.LastLocation.Y + chart.Border.ActualThickness :
						chart.Border.ActualThickness - (int)annotation.LastLocation.Y;
					position.InnerIndents.Top = 0;
				}
			}
			return position;
		}
		public static RelativePosition CreateRelativePosition(Annotation annotation) {
			RelativePosition position = new RelativePosition();
			if (annotation != null) {
				DiagramPoint center = new DiagramPoint(annotation.LastLocation.X + 0.5 * annotation.Width, annotation.LastLocation.Y + 0.5 * annotation.Height);
				double length = MathUtils.CalcLength2D(annotation.LastAnchorPosition, center);
				double angle = GeometricUtils.CalcBetweenPointsAngle((GRealPoint2D)annotation.LastAnchorPosition, (GRealPoint2D)center);
				position.SetConnectorLength(length);
				position.SetAngle(MathUtils.Radian2Degree(-angle));
			}
			return position;
		}
		public static List<AnnotationViewData> CreateInnerAnnotationsViewData(List<AnnotationLayout> annotationsAnchorPointsLayout, TextMeasurer textMeasurer) {
			List<AnnotationViewData> result = new List<AnnotationViewData>();
			foreach (AnnotationLayout layout in annotationsAnchorPointsLayout.ToArray()) {
				RelativePosition shapePosition = layout.Annotation.ShapePosition as RelativePosition;
				if (shapePosition != null) {
					annotationsAnchorPointsLayout.Remove(layout);
					AnnotationLayout shapeLayout = new AnnotationLayout(layout.Annotation, shapePosition.GetShapeLocation(layout.Position));
					AnnotationViewData viewData = layout.Annotation.CalculateViewData(textMeasurer, shapeLayout, layout);
					if (viewData != null)
						result.Add(viewData);
				}
			}
			result.Sort(AnnotationHelper.CompareByAnnotationZOrder);
			return result;
		}
		public static List<AnnotationViewData> CreateInnerAnnotationsViewData(List<AnnotationLayout> annotationsAnchorPointsLayout, TextMeasurer textMeasurer, Rectangle allowedBoundsForAnnotationPlacing) {
			List<AnnotationViewData> result = new List<AnnotationViewData>();
			foreach (AnnotationLayout layout in annotationsAnchorPointsLayout.ToArray()) {
				RelativePosition shapePosition = layout.Annotation.ShapePosition as RelativePosition;
				if (shapePosition != null) {
					annotationsAnchorPointsLayout.Remove(layout);
					AnnotationLayout shapeLayout = new AnnotationLayout(layout.Annotation, shapePosition.GetShapeLocation(layout.Position));
					AnnotationViewData viewData = layout.Annotation.CalculateViewData(textMeasurer, shapeLayout, layout, allowedBoundsForAnnotationPlacing);
					if (viewData != null)
						result.Add(viewData);
				}
			}
			result.Sort(AnnotationHelper.CompareByAnnotationZOrder);
			return result;
		}
		public static List<AnnotationLayout> CreateFreAnnotationsShapesLayout(AnnotationRepository annotations, ChartElement dockTarget, ZPlaneRectangle viewport) {
			List<AnnotationLayout> result = new List<AnnotationLayout>();
			foreach (Annotation annotation in annotations) {
				FreePosition shapePosition = annotation.ShapePosition as FreePosition;
				if (shapePosition != null && shapePosition.DockTarget == dockTarget)
					result.Add(new AnnotationLayout(annotation, shapePosition.GetShapeLocation(viewport)));
			}
			return result;
		}
		public static IDockTarget[] GetDockTargets(XYDiagram2D diagram) {
			if (diagram == null)
				return new IDockTarget[] { null };
			IDockTarget[] totalPanes = new XYDiagramPaneBase[diagram.Panes.Count + 2];
			totalPanes[0] = null;
			totalPanes[1] = diagram.DefaultPane;
			for (int i = 2; i < totalPanes.Length; i++)
				totalPanes[i] = diagram.Panes[i - 2];
			return totalPanes;
		}
		public static string GetDockTargetName(IDockTarget dockTarget) {
			return dockTarget != null ? dockTarget.Name : ChartLocalizer.GetString(ChartStringId.ChartControlDockTarget);
		}
		public static ChartElement GetDockTarget(string name, XYDiagram2D diagram) {
			if (name != null && diagram != null)
				return diagram.FindPaneByName(name);
			return null;
		}
		public static bool IsPaneAnchorPointSupported(Annotation annotation) {
			Chart chart = CommonUtils.FindOwnerChart(annotation);
			return chart != null && chart.Diagram is XYDiagram2D;
		}
		public static bool IsSeriesPointAnchorPointSupported(Annotation annotation) {
			Chart chart = CommonUtils.FindOwnerChart(annotation);
			if (chart == null)
				return true;
			foreach (Series series in chart.Series) {
				if (!series.Autocreated && series.Points.Count > 0)
					return true;
			}
			return false;
		}
		public static DiagramPoint? CalculateAchorPointForCenterAreaPointWithoutScrolling(XYDiagramMappingContainer mappingContainer, double argument, MinMaxValues values) {
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
		public static void CalculateAchorPointLayoutForCenterAreaPoint(Annotation annotation, AreaSeriesView areaView, XYDiagramAnchorPointLayoutList anchorPointLayoutList, RefinedPointData pointData) {
			XYDiagramMappingContainer mappingContainer = anchorPointLayoutList.MappingContainer;
			MinMaxValues values = areaView.GetSeriesPointValues(pointData.RefinedPoint);
			double argument = pointData.RefinedPoint.Argument;
			if (areaView.IsScrollingEnabled && annotation.ScrollingSupported) {
				double centerValue = MinMaxValues.Intersection(mappingContainer.MappingForScrolling.AxisY.WholeRangeData, values).CalculateCenter();
				if (!Double.IsNaN(centerValue))
					anchorPointLayoutList.Add(new AnnotationLayout(annotation, mappingContainer.MappingForScrolling.GetScreenPoint(argument, centerValue), pointData.RefinedPoint));
			}
			else {
				DiagramPoint? anchorPoint = CalculateAchorPointForCenterAreaPointWithoutScrolling(mappingContainer, argument, values);
				if (!anchorPoint.HasValue)
					return;
				anchorPointLayoutList.Add(new AnnotationLayout(annotation, anchorPoint.Value, pointData.RefinedPoint));
			}
		}
		public static void CalculateAchorPointLayoutForCenterBarPoint(Annotation annotation, BarSeriesView barView, XYDiagramAnchorPointLayoutList anchorPointLayoutList, RefinedPointData pointData) {
			BarData barData = pointData.GetBarData(barView);
			if (barView.IsScrollingEnabled && annotation.ScrollingSupported) {
				XYDiagramMappingBase mapping = anchorPointLayoutList.MappingContainer.MappingForScrolling;
				double centerValue = MinMaxValues.Intersection(mapping.AxisY.WholeRangeData, new MinMaxValues(barData.ZeroValue, barData.ActualValue)).CalculateCenter(); 
				if (!Double.IsNaN(centerValue))
					anchorPointLayoutList.Add(new AnnotationLayout(annotation, barData.GetScreenPoint(barData.Argument, centerValue, mapping), pointData.RefinedPoint));
			}
			else {
				DiagramPoint? anchorPoint = barData.CalculateAnchorPointForCenterLabelPosition(anchorPointLayoutList.MappingContainer);
				if (anchorPoint.HasValue)
					anchorPointLayoutList.Add(new AnnotationLayout(annotation, anchorPoint.Value, pointData.RefinedPoint));
			}
		}
	}
}
