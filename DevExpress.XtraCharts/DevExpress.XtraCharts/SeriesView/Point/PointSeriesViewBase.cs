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
using System.ComponentModel;
using System.Diagnostics;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	public abstract class PointSeriesViewBase : SeriesViewColorEachSupportBase, IPointSeriesView {
		protected const int LegendMarkerShadowSize = 1;
		protected const int CrosshairMarkerAdditionalSize = 2;
		readonly MarkerBase marker;
		readonly PointSeriesViewPainter painter;
		protected internal MarkerBase Marker { get { return marker; } }
		protected internal virtual bool ActualMarkerVisible { get { return true; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Type DiagramType { get { return typeof(XYDiagram); } }
		public PointSeriesViewBase() : base() {
			marker = CreateMarker();
			painter = CreatePainter();
		}
		#region IPointSeriesView implementation
		MarkerBase IPointSeriesView.Marker { get { return marker; } }
		#endregion
		protected Color GetMarkerSelfColor(PointDrawOptionsBase drawOptions) {
			return painter.GetMarkerSelfColor(drawOptions);
		}
		protected Color GetBorderDrawColor(PointDrawOptionsBase drawOptions) {
			return painter.GetBorderDrawColor(drawOptions, marker);
		}
		protected virtual int CalculateCrosshairPolygonSize(XYDiagramMappingBase diagramMapping, RefinedPoint refinedPoint) {
			SimpleMarker marker = Marker as SimpleMarker;
			if (marker != null)
				return marker.Size + CrosshairMarkerAdditionalSize;
			else
				return 0;
		}
		protected virtual void CalculateAnnotationAnchorPointLayout(Annotation annotation, XYDiagramAnchorPointLayoutList anchorPointLayoutList, RefinedPointData pointData) {
			XYDiagramMappingBase mapping = anchorPointLayoutList.GetMapping(pointData.RefinedPoint.Argument, GetHighlightedPointValue(pointData.RefinedPoint), annotation.ScrollingSupported);
			if (mapping != null)
				anchorPointLayoutList.Add(new AnnotationLayout(annotation, mapping.GetScreenPointNoRound(pointData.RefinedPoint.Argument, GetHighlightedPointValue(pointData.RefinedPoint)), pointData.RefinedPoint));
		}
		protected virtual MarkerBase CreateMarker() {
			return new MarkerBase(this);
		}
		protected override Rectangle CorrectLegendMarkerBounds(Rectangle bounds) {
			int shadowSize = Shadow.GetActualSize(PointSeriesViewBase.LegendMarkerShadowSize);
			return new Rectangle(bounds.Left, bounds.Top, bounds.Width - shadowSize, bounds.Height - shadowSize);
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new PointDrawOptionsBase(this);
		}
		protected virtual PointSeriesViewPainter CreatePainter() {
			return new PointSeriesViewPainter(this);
		}
		protected virtual double GetHighlightedPointValue(RefinedPoint point) {
			return ((IXYWPoint)point).Value;
		}
		protected override void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			painter.RenderLegendMarker(renderer, bounds, seriesPointDrawOptions, seriesDrawOptions, selectionState);
		}
		protected override void RenderCrosshairMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions) {
			painter.RenderLegendMarker(renderer, bounds, seriesPointDrawOptions, seriesDrawOptions, SelectionState.Normal);
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value;
		}
		protected internal override void Render(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			painter.Render(renderer, mappingBounds, pointLayout, drawOptions);
		}
		protected internal override void RenderShadow(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			painter.RenderShadow(renderer, mappingBounds, pointLayout, drawOptions);
		}
		protected internal override void RenderWholeSeries(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) {
			painter.RenderWholeSeries(renderer, mappingBounds, layout);
		}
		protected internal override void RenderWholeSeriesShadow(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) {
			painter.RenderWholeSeriesShadow(renderer, mappingBounds, layout);
		}
		protected internal override DiagramPoint? CalculateRelativeToolTipPosition(XYDiagramMappingBase mapping, RefinedPointData pointData) {
			return mapping.GetScreenPointNoRound(pointData.RefinedPoint.Argument, GetHighlightedPointValue(pointData.RefinedPoint));
		}
		protected internal override void CalculateAnnotationsAnchorPointsLayout(XYDiagramAnchorPointLayoutList anchorPointLayoutList) {
			foreach (RefinedPointData pointData in anchorPointLayoutList.SeriesData) {
				SeriesPoint seriesPoint = pointData.SeriesPoint as SeriesPoint;
				if (seriesPoint != null && seriesPoint.Annotations.Count > 0) {
					foreach (Annotation annotation in seriesPoint.Annotations)
						CalculateAnnotationAnchorPointLayout(annotation, anchorPointLayoutList, pointData);
				}
				foreach (RefinedPoint refinedPoint in pointData.RefinedPoint.Children) {
					seriesPoint = refinedPoint.SeriesPoint as SeriesPoint;
					if (seriesPoint != null && seriesPoint.Annotations.Count > 0) {
						foreach (Annotation annotation in seriesPoint.Annotations)
							CalculateAnnotationAnchorPointLayout(annotation, anchorPointLayoutList, pointData);
					}
				}
			}
		}
		protected internal override HighlightedPointLayout CalculateHighlightedPointLayout(XYDiagramMappingBase diagramMapping, RefinedPoint refinedPoint, ISeriesView seriesView, DrawOptions drawOptions) {
			DiagramPoint point = diagramMapping.GetScreenPointNoRound(refinedPoint.Argument, GetHighlightedPointValue(refinedPoint));
			if (diagramMapping.Bounds.Contains((int)point.X, (int)point.Y)) {
				int polygonSize = CalculateCrosshairPolygonSize(diagramMapping, refinedPoint);
				IPolygon polygon = Marker.CalculatePolygon(new GRealPoint2D(Math.Round(point.X), Math.Round(point.Y)), true, polygonSize);
				PointDrawOptionsBase pointDrawOptions = drawOptions as PointDrawOptionsBase;
				SeriesPoint seriesPoint = refinedPoint.SeriesPoint as SeriesPoint;
				if (pointDrawOptions != null) {
					System.Drawing.Color color;
					System.Drawing.Color borderColor;
					if (seriesPoint != null && !seriesPoint.Color.IsEmpty) {
						color = seriesPoint.Color;
						borderColor = seriesPoint.Color;
					}
					else {
						color = GetMarkerSelfColor(pointDrawOptions);
						borderColor = GetBorderDrawColor(pointDrawOptions);
					}
					return new HighlightedPolygonPointLayout(polygon, color, borderColor);
				}
			}
			return null;
		}
		protected internal override void RenderHighlightedPoint(IRenderer renderer, HighlightedPointLayout pointLayout) {
			HighlightedPolygonPointLayout polygonPointLayout = pointLayout as HighlightedPolygonPointLayout;
			if (polygonPointLayout.Polygon != null) {
				renderer.EnablePolygonAntialiasing(true);
				polygonPointLayout.Polygon.Render(renderer, new SolidFillOptions(), polygonPointLayout.Color, polygonPointLayout.Color, polygonPointLayout.BorderColor, 1);
				renderer.RestorePolygonAntialiasing();
			}
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			IPointSeriesView view = obj as IPointSeriesView;
			if (view == null)
				return;
			marker.Assign(view.Marker);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			PointSeriesViewBase view = (PointSeriesViewBase)obj;
			return marker.Equals(view.marker);
		}
	}
}
