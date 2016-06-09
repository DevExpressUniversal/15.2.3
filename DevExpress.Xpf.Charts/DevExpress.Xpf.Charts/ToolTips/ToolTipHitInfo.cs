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

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts.Native {
	public partial class ToolTipHitInfo {
		const double pointRelativePositionOffsetX = 0.5;
		const double pointRelativePositionOffsetY = 0.0;
		const double seriesRelativePositionOffsetX = 0.5;
		const double seriesRelativePositionOffsetY = 0.5;
		const double touchTargetSize = 10.0;
		const double pointRelativePositionOffsetZ = 0.5;
		readonly bool calculateForSeries;
		readonly bool touchUses;
		readonly ChartControl chart;
		readonly Diagram diagram;
		readonly Point point;
		SeriesPoint seriesPoint;
		RefinedPoint refinedPoint;
		Series series;
		IHitTestableElement hitTestableElement;
		HitTestResult result;
		bool inElement;
		bool inLegend;
		bool inLabel;
		public SeriesPoint SeriesPoint { get { return seriesPoint; } }
		public RefinedPoint RefinedPoint { get { return refinedPoint; } }
		public Series Series { get { return series; } }
		public Point ToolTipPoint { get { return GetToolTipPoint(); } }
		internal ToolTipHitInfo(ChartControl chart, Point point, bool calculateForSeries, bool touchUses) {
			this.chart = chart;
			this.point = point;
			this.touchUses = touchUses;
			this.calculateForSeries = calculateForSeries;
			diagram = chart.Diagram;
			if (chart != null && diagram != null && chart.Visibility == Visibility.Visible)
				PerformHitTesting();
		}
		void PerformHitTesting() {
			Legend legend = chart.Legend;
			if (legend != null && legend.Visibility == Visibility.Visible && PerformElementHitTesting(legend))
				return;
			PerformDiagramHitTesting();
		}
		void PerformDiagram2DHitTesting() {
			if (chart.Diagram is SimpleDiagram2D || chart.Diagram is CircularDiagram2D)
				PerformElementHitTesting(chart.Diagram);
			else {
				XYDiagram2D diagram = chart.Diagram as XYDiagram2D;
				if (diagram != null)
					foreach (Pane pane in diagram.ActualPanes)
						if (PerformElementHitTesting(pane))
							break;
			}
		}
		Rect GetTouchRect(Point elementPoint) {
			double offset = touchTargetSize / 2;
			return new Rect(elementPoint.X - offset, elementPoint.Y - offset, touchTargetSize, touchTargetSize);
		}
		Point GetToolTipPoint() {
			Point location = new Point();
			if (inLegend) {
				LegendItemContainer container = hitTestableElement as LegendItemContainer;
				if (container != null) {
					FrameworkElement marker = LayoutHelper.FindElementByName(container, "PART_MarkerPath");
					if (marker != null) {
						Rect markerRect = LayoutHelper.GetRelativeElementRect(marker, chart);
						location = new Point(markerRect.Left + markerRect.Width * seriesRelativePositionOffsetX,
							markerRect.Top + markerRect.Height * seriesRelativePositionOffsetY);
					}
					else {
						Rect containerRect = LayoutHelper.GetRelativeElementRect(container, chart);
						location = new Point(containerRect.Left + containerRect.Width * seriesRelativePositionOffsetX,
							containerRect.Top + containerRect.Height * seriesRelativePositionOffsetY);
					}
				}
			}
			else if (seriesPoint != null)
				location = GetSeriesPointToolTipLocation();
			else 
				if (series != null)
					location = point;
			return location;
		}
		Point GetPoint2DLocation() {
			Point location = new Point();
			SeriesPointItem pointItem = GetPointItem();
			if (pointItem != null) {
				if (series is PieSeries2D)
					location = ((PieSeries2D)series).CalculateToolTipPoint(pointItem);
				else if (series is XYSeries)
					location = ((XYSeries)series).CalculateToolTipPoint(pointItem, inLabel);
				else if (series is FunnelSeries2D)
					location = ((FunnelSeries2D)series).CalculateToolTipPoint(pointItem);
			}
			return location;
		}
		SeriesPointItem GetPointItem() {
			SeriesPointItemPresentation pointItemPresentation = hitTestableElement as SeriesPointItemPresentation;
			if (pointItemPresentation != null)
				return pointItemPresentation.PointItem;
			else {
				SeriesLabelPresentation labelPresentation = hitTestableElement as SeriesLabelPresentation;
				if (labelPresentation != null)
					return labelPresentation.LabelItem.PointItem;
				else {
					SeriesLabelConnectorPresentation connectorPresentation = hitTestableElement as SeriesLabelConnectorPresentation;
					if (connectorPresentation != null)
						return connectorPresentation.LabelItem.PointItem;
				}
			}
			return null;
		}
		HitTestResultBehavior HitTestResult(HitTestResult result) {
			this.result = result;
			return HitTestResultBehavior.Stop;
		}
		void PerformHitTesting3D() {
			result = null;
			VisualTreeHelper.HitTest(chart, new HitTestFilterCallback(HitTestingHelper.HitFilter), new HitTestResultCallback(HitTestResult), new PointHitTestParameters(point));
			if (result is RayMeshGeometry3DHitTestResult) {
				RayMeshGeometry3DHitTestResult ray3DResult = (RayMeshGeometry3DHitTestResult)result;
				refinedPoint = ray3DResult.ModelHit.GetValue(ChartControl.SeriesPointHitTestInfoProperty) as RefinedPoint;
				seriesPoint = refinedPoint != null ? SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint) : null;
				series = ray3DResult.ModelHit.GetValue(ChartControl.SeriesHitTestInfoProperty) as Series;
				if (seriesPoint != null && series == null)
					series = seriesPoint.Series;
			}
		}
		HitTestResultBehavior OnElementHitTestResult(HitTestResult result) {
			inElement = true;
			hitTestableElement = HitTestingHelper.GetParentHitTestableElement(result.VisualHit);
			HitTestResultBehavior behaviour = HitTestResultBehavior.Continue;
			if (hitTestableElement != null) {
				series = hitTestableElement.Element as Series;
				refinedPoint = hitTestableElement.AdditionalElement is RefinedPoint ? (RefinedPoint)hitTestableElement.AdditionalElement : hitTestableElement.Element as RefinedPoint;
				seriesPoint = refinedPoint != null ? SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint) : null;
				if (seriesPoint != null || calculateForSeries)
					behaviour = HitTestResultBehavior.Stop;
				inLegend = hitTestableElement is LegendItemContainer;
				inLabel = hitTestableElement is SeriesLabelPresentation || hitTestableElement is SeriesLabelConnectorPresentation;
				if (series == null && seriesPoint != null)
					series = seriesPoint.Series;
			}
			return behaviour;
		}
		bool PerformElementHitTesting(UIElement element) {
			inElement = false;
			if (chart.FindCommonVisualAncestor(element) != null) {
				Rect elementRect = LayoutHelper.GetRelativeElementRect(element, chart);
				Point elementPoint = new Point(point.X - elementRect.Left, point.Y - elementRect.Top);
				HitTestParameters parameters;
				if (touchUses) {
					Geometry geometry = new RectangleGeometry(GetTouchRect(elementPoint));
					parameters = new GeometryHitTestParameters(geometry);
				}
				else
					parameters = new PointHitTestParameters(elementPoint);
				VisualTreeHelper.HitTest(element, null, OnElementHitTestResult, parameters);
			}
			return inElement;
		}
		void PerformDiagramHitTesting() {
			if (diagram is Diagram3D)
				PerformHitTesting3D();
			else
				PerformDiagram2DHitTesting();
		}
		Point GetSeriesPointToolTipLocation() {
			if (diagram is Diagram3D)
				return GetPoint3DLocation();
			else
				return GetPoint2DLocation();
		}
		Point GetPoint3DLocation() {
			Point location = new Point();
			series = seriesPoint.Series;
			if (series != null) {
				if (series is PieSeries3D)
					location = ((PieSeries3D)series).CalculateToolTipPoint(seriesPoint);
				else
					location = ((XYSeries3D)series).CalculateToolTipPoint(seriesPoint);
			}
			return location;
		}
	}
}
