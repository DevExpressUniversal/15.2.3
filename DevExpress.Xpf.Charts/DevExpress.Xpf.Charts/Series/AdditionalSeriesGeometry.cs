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
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class AdditionalLineSeriesGeometry : ChartElementBase, IHitTestableElement, IFinishInvalidation {
		const int maxBrushesCacheSize = 10;
		readonly XYSeries series;
		readonly Dictionary<Color, SolidColorBrush> brushesCache = new Dictionary<Color, SolidColorBrush>();
		readonly Dictionary<AdditionalGeometryCacheKey, Geometry> geometryCache = new Dictionary<AdditionalGeometryCacheKey, Geometry>();
		bool selectedState = false;
		bool shouldUpdateGeometry = true;
		object IHitTestableElement.Element { get { return Series; } }
		object IHitTestableElement.AdditionalElement { get { return null; } }
		bool ShouldRoundPoints { get { return CanRoundPoints &&  (LineThickness % 2 == 1); } }
		protected XYSeries Series { get { return series; } }
		protected virtual int LineThickness { get { return ((ILineSeries)Series).LineThickness; } }
		protected virtual LineStyle LineStyle { get { return ((ILineSeries)Series).LineStyle; } }
		protected virtual bool CanRoundPoints { get { return false; } }
		internal bool SelectedState {
			get { return selectedState; }
			set {
				if (selectedState != value) {
					selectedState = value;
					OnSelectedStateChanged();
				}
			}
		}
#if DEBUGTEST
		internal Geometry LineGeometry { get { return geometryCache[AdditionalGeometryCacheKey.Line]; } }
		internal Geometry AreaGeometry { get { return geometryCache[AdditionalGeometryCacheKey.Area]; } }
#endif
		public AdditionalLineSeriesGeometry(XYSeries series) {
			this.series = series;
		}
		void OnSelectedStateChanged() {
			Update(false);
		}
		void RenderContent(DrawingContext drawingContext) {
			if (Series == null || Series.Item == null || Series.Item.RefinedSeries == null)
				return;
			IList<IGeometryStrip> strips = CalculateStrips();
			if (strips.Count == 0)
				return;
			IMapping mapping = Series.CreateDiagramMapping();
			Transform transform = Series.CreateSeriesAnimationTransform(mapping);
			if (transform != null)
				transform.Freeze();
			Pen pen = CreatePen(GetLineColor(), LineThickness, LineStyle);
			RenderCore(drawingContext, strips, pen, mapping, transform);
			shouldUpdateGeometry = true;
		}
		IList<IGeometryStrip> CalculateStrips() {
			GeometryCalculator geometryCalculator = new GeometryCalculator();
			return geometryCalculator.CreateStrips(Series.Item.RefinedSeries);
		}
		protected ChartGeometryFigure CreatePolyLineFigure(List<Point> points) {
			ChartGeometryFigure figure = new ChartGeometryFigure();
			if (points != null && points.Count > 0) {
				ChartPolyLineSegment segment = new ChartPolyLineSegment(points, ShouldRoundPoints);
				figure.Segments.Add(segment);
				if (segment.Points.Count > 0)
					figure.StartPoint = segment.Points[0];
			}
			return figure;
		}
		protected ChartGeometryFigure CreatePolyBezierFigure(List<Point> points) {
			ChartGeometryFigure figure = new ChartGeometryFigure();
			if (points != null && points.Count > 0) {
				ChartPolyBezierSegment segment = new ChartPolyBezierSegment(points, ShouldRoundPoints);
				figure.Segments.Add(segment);
				if (segment.Points.Count > 0) {
					figure.StartPoint = segment.Points[0];
					segment.Points.RemoveAt(0);
				}
			}
			return figure;
		}
		protected SolidColorBrush GetBrush(Color color) {
			SolidColorBrush brush;
			if (!brushesCache.TryGetValue(color, out brush)) {
				if (brushesCache.Count == maxBrushesCacheSize)
					brushesCache.Clear();
				brush = new SolidColorBrush(color);
				brush.Freeze();
				brushesCache.Add(color, brush);
			}
			return brush;
		}
		protected List<Point> GetRoundedPoints(IMapping mapping, LineStrip lineStrip) {
			List<Point> points = new List<Point>();
			foreach (GRealPoint2D point in lineStrip)
				points.Add(mapping.GetRoundedDiagramPoint(point.X, point.Y));
			return points;
		}
		protected Pen CreatePen(Color color, int thickness, LineStyle lineStyle) {
			Brush brush = GetBrush(color);
			Pen pen = new Pen(brush, thickness);
			if (lineStyle != null) {
				pen.DashCap = lineStyle.DashCap;
				pen.DashStyle = lineStyle.DashStyle;
				pen.LineJoin = lineStyle.LineJoin;
				pen.MiterLimit = lineStyle.MiterLimit;
			}
			pen.Freeze();
			return pen;
		}
		protected Geometry GetCachedGeometry(AdditionalGeometryCacheKey cacheKey) {
			Geometry geometry;
			if (!shouldUpdateGeometry && geometryCache.TryGetValue(cacheKey, out geometry))
				return geometry;
			return null;
		}
		protected void SaveGeometryToCache(Geometry geometry, AdditionalGeometryCacheKey cacheKey) {
			if (geometryCache.ContainsKey(cacheKey))
				geometryCache.Remove(cacheKey);
			geometryCache.Add(cacheKey, geometry);
		}
		protected Geometry CalculateLineGeometry(IMapping mapping, IList<IGeometryStrip> strips, Transform transform, bool isTopStrip) {
			StreamGeometry lineGeometry = new StreamGeometry() { Transform = transform };
			using (StreamGeometryContext streamContext = lineGeometry.Open()) {
				lineGeometry.Freeze();
				foreach (IGeometryStrip strip in strips) {
					foreach (ChartGeometryFigure figure in CalculateLineFigures(mapping, strip, isTopStrip)) {
						figure.Render(streamContext);
					}
				}
			}
			return lineGeometry;
		}
		protected virtual List<ChartGeometryFigure> CalculateLineFigures(IMapping mapping, IGeometryStrip strip, bool isTopStrip) {
			List<ChartGeometryFigure> figures = new List<ChartGeometryFigure>();
			LineStrip lineStrip;
			if (strip is RangeStrip)
				lineStrip = isTopStrip ? ((RangeStrip)strip).TopStrip : ((RangeStrip)strip).BottomStrip;
			else
				lineStrip = strip as LineStrip;
			if (lineStrip == null)
				return figures;
			List<Point> points = GetLinePoints(mapping, lineStrip);
			ChartGeometryFigure figure = CreatePolyLineFigure(points);
			figures.Add(figure);
			return figures;
		}
		protected virtual void RenderCore(DrawingContext drawingContext, IList<IGeometryStrip> strips, Pen pen, IMapping diagramMapping, Transform transform) {
			Geometry geometry = GetCachedGeometry(AdditionalGeometryCacheKey.Line);
			if (geometry == null) {
				geometry = CalculateLineGeometry(diagramMapping, strips, transform, true);
				SaveGeometryToCache(geometry, AdditionalGeometryCacheKey.Line);
			}
			drawingContext.DrawGeometry(null, pen, geometry);
		}
		protected virtual List<Point> GetLinePoints(IMapping mapping, IGeometryStrip geometryStrip) {
			List<Point> points = new List<Point>();
			LineStrip lineStrip = (LineStrip)geometryStrip;
			foreach (GRealPoint2D point in lineStrip)
				points.Add(mapping.GetDiagramPoint(point.X, point.Y));
			return points;
		}
		protected virtual Color GetLineColor() {
			return Series.Item.DrawOptions.Color;
		}
		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			RenderContent(drawingContext);
		}
		internal void Update(bool shouldUpdateGeometry) {
			this.shouldUpdateGeometry = shouldUpdateGeometry;
			InvalidateVisual();
		}
	}
	public class AdditionalStepLineSeriesGeometry : AdditionalLineSeriesGeometry {
		protected override bool CanRoundPoints { get { return true; } }
		public AdditionalStepLineSeriesGeometry(XYSeries series)
			: base(series) {
		}
		protected override List<Point> GetLinePoints(IMapping mapping, IGeometryStrip geometryStrip) {
			return GetRoundedPoints(mapping, geometryStrip as LineStrip);
		}
	}
	public class AdditionalAreaSeriesGeometry : AdditionalLineSeriesGeometry {
		protected override LineStyle LineStyle { get { return ((ISupportSeriesBorder)Series).ActualBorder.LineStyle; } }
		protected override int LineThickness { get { return LineStyle != null ? LineStyle.Thickness : 1; } }
		public AdditionalAreaSeriesGeometry(XYSeries series)
			: base(series) {
		}
		bool IsZeroWidthStrip(RangeStrip strip) {
			double? x = new double?();
			List<GRealPoint2D> points = new List<GRealPoint2D>();
			points.AddRange(strip.TopStrip);
			points.AddRange(strip.BottomStrip);
			foreach (GRealPoint2D point in points) {
				if (x.HasValue && x != point.X)
					return false;
				else if (!x.HasValue)
					x = point.X;
			}
			return x.HasValue;
		}
		Geometry CalculateAreaAdditionalGeometry(IMapping mapping, IList<IGeometryStrip> strips, Transform transform) {
			StreamGeometry areaGeometry = new StreamGeometry() { Transform = transform };
			using (StreamGeometryContext streamContext = areaGeometry.Open()) {
				areaGeometry.Freeze();
				foreach (IGeometryStrip strip in strips) {
					RangeStrip rangeStrip = strip as RangeStrip;
					if (rangeStrip != null) {
						ChartGeometryFigure figure = CalculateAreaFigure(mapping, rangeStrip);
						figure.Render(streamContext);
					}
				}
			}
			return areaGeometry;
		}
		Color GetAreaColor() {
			return Series.Item.DrawOptions.Color;
		}
		void RenderAreaGeometry(DrawingContext drawingContext, IList<IGeometryStrip> strips, IMapping diagramMapping, Transform transform) {
			Geometry geometry = GetCachedGeometry(AdditionalGeometryCacheKey.Area);
			if (geometry == null) {
				geometry = CalculateAreaAdditionalGeometry(diagramMapping, strips, transform);
				SaveGeometryToCache(geometry, AdditionalGeometryCacheKey.Area);
			}
			Brush areaBrush = GetBrush(GetAreaColor());
			drawingContext.PushOpacity(Series.GetOpacity());
			if (SelectedState)
				drawingContext.PushOpacityMask(Series.AdditionalGeometrySelectionOpacityMask);
			drawingContext.DrawGeometry(areaBrush, null, geometry);
			if (SelectedState)
				drawingContext.Pop();
			drawingContext.Pop();
		}
		List<Point> GetZeroWidthStripPoints(IMapping mapping, RangeStrip strip) {
			List<Point> points = new List<Point>();
			GRealPoint2D? topPoint = new GRealPoint2D?();
			LineStrip topPoints;
			LineStrip bottomPoints;
			GetPointsForDrawing(strip, out topPoints, out bottomPoints);
			foreach (GRealPoint2D point in topPoints)
				if (!topPoint.HasValue || point.Y > topPoint.Value.Y)
					topPoint = point;
			GRealPoint2D? bottomPoint = new GRealPoint2D?();
			foreach (GRealPoint2D point in bottomPoints)
				if (!bottomPoint.HasValue || point.Y < bottomPoint.Value.Y)
					bottomPoint = point;
			if (topPoint.HasValue && bottomPoint.HasValue) {
				Point top = mapping.GetRoundedDiagramPoint(topPoint.Value.X, topPoint.Value.Y);
				Point bottom = mapping.GetRoundedDiagramPoint(bottomPoint.Value.X, bottomPoint.Value.Y);
				points.Add(new Point(top.X, top.Y));
				points.Add(new Point(top.X + 1, top.Y));
				points.Add(new Point(bottom.X + 1, bottom.Y));
				points.Add(new Point(bottom.X, bottom.Y));
			}
			return points;
		}
		ChartGeometryFigure CalculateAreaFigure(IMapping mapping, RangeStrip strip) {
			List<Point> points;
			if (IsZeroWidthStrip(strip) && Series.HasAdditionalGeometryBottomStrip)
				points = GetZeroWidthStripPoints(mapping, strip);
			else
				points = GetAreaPoints(mapping, strip);
			ChartGeometryFigure figure = CreateAreaFigure(points);
			figure.IsClosed = true;
			figure.IsFilled = true;
			return figure;
		}
		protected virtual ChartGeometryFigure CreateAreaFigure(List<Point> points) {
			return CreatePolyLineFigure(points);
		}
		protected virtual void GetPointsForDrawing(RangeStrip strip, out LineStrip topPoints, out LineStrip bottomPoints) {
			topPoints = strip.TopStrip;
			bottomPoints = strip.BottomStrip;
		}
		protected virtual List<Point> GetAreaPoints(IMapping mapping, RangeStrip strip) {
			List<Point> points = new List<Point>();
			foreach (GRealPoint2D point in strip.TopStrip)
				points.Add(mapping.GetDiagramPoint(point.X, point.Y));
			int position = points.Count;
			if (Series.HasAdditionalGeometryBottomStrip)
				foreach (GRealPoint2D point in strip.BottomStrip)
					points.Insert(position, mapping.GetDiagramPoint(point.X, point.Y));
			return points;
		}
		protected override Color GetLineColor() {
			SolidColorBrush seriesBrush = ((ISupportSeriesBorder)Series).ActualBorder.Brush;
			return seriesBrush != null ? seriesBrush.Color : Series.BaseColor;
		}
		protected override void RenderCore(DrawingContext drawingContext, IList<IGeometryStrip> strips, Pen pen, IMapping diagramMapping, Transform transform) {
			RenderAreaGeometry(drawingContext, strips, diagramMapping, transform);
			base.RenderCore(drawingContext, strips, pen, diagramMapping, transform);
		}
	}
	public class AdditionalStepAreaSeriesGeometry : AdditionalAreaSeriesGeometry {
		protected override bool CanRoundPoints { get { return true; } }
		public AdditionalStepAreaSeriesGeometry(XYSeries series)
			: base(series) {
		}
		protected override List<Point> GetLinePoints(IMapping mapping, IGeometryStrip geometryStrip) {
			return GetRoundedPoints(mapping, geometryStrip as LineStrip);
		}
		protected override List<Point> GetAreaPoints(IMapping mapping, RangeStrip strip) {
			List<Point> points = new List<Point>();
			foreach (GRealPoint2D point in strip.TopStrip)
				points.Add(mapping.GetRoundedDiagramPoint(point.X, point.Y));
			int position = points.Count;
			foreach (GRealPoint2D point in strip.BottomStrip)
				points.Insert(position, mapping.GetRoundedDiagramPoint(point.X, point.Y));
			return points;
		}
	}
	public class AdditionalStackedAreaSeriesGeometry : AdditionalAreaSeriesGeometry {
		public AdditionalStackedAreaSeriesGeometry(XYSeries series)
			: base(series) {
		}
		protected override List<ChartGeometryFigure> CalculateLineFigures(IMapping mapping, IGeometryStrip strip, bool isTopStrip) {
			List<ChartGeometryFigure> figures = new List<ChartGeometryFigure>();
			LineStrip lineStrip = strip is RangeStrip ? ((RangeStrip)strip).TopStrip : strip as LineStrip;
			if (lineStrip == null)
				return figures;
			LineStrip uniqueLineStrip = lineStrip.CreateUniqueStrip();
			List<Point> points = new List<Point>();
			if (uniqueLineStrip.Count >= 1)
				points.Add(mapping.GetRoundedDiagramPoint(uniqueLineStrip[0].X, uniqueLineStrip[0].Y));
			for (int i = 1; i < uniqueLineStrip.Count; i++) {
				if (uniqueLineStrip[i - 1].X != uniqueLineStrip[i].X)
					points.Add(mapping.GetRoundedDiagramPoint(uniqueLineStrip[i].X, uniqueLineStrip[i].Y));
				else {
					figures.Add(CreatePolyLineFigure(points));
					points = new List<Point>();
					points.Add(mapping.GetRoundedDiagramPoint(uniqueLineStrip[i].X, uniqueLineStrip[i].Y));
				}
			}
			figures.Add(CreatePolyLineFigure(points));
			return figures;
		}
	}
	public class AdditionalRangeAreaSeriesGeometry : AdditionalAreaSeriesGeometry {
		public AdditionalRangeAreaSeriesGeometry(XYSeries series)
			: base(series) {
		}
		void RenderSecondLine(DrawingContext drawingContext, IList<IGeometryStrip> strips, IMapping diagramMapping, Transform transform) {
			RangeAreaSeries2D rangeAreaSeries = (RangeAreaSeries2D)Series;
			LineStyle lineStyle = rangeAreaSeries.ActualBorder2.LineStyle;
			int lineThickness = lineStyle != null ? lineStyle.Thickness : 1;
			Pen pen = CreatePen(rangeAreaSeries.Border2Color, lineThickness, lineStyle);
			Geometry geometry = GetCachedGeometry(AdditionalGeometryCacheKey.Line2);
			if (geometry == null) {
				geometry = CalculateLineGeometry(diagramMapping, strips, transform, false);
				SaveGeometryToCache(geometry, AdditionalGeometryCacheKey.Line2);
			}
			drawingContext.DrawGeometry(null, pen, geometry);
		}
		protected override void RenderCore(DrawingContext drawingContext, IList<IGeometryStrip> strips, Pen pen, IMapping diagramMapping, Transform transform) {
			base.RenderCore(drawingContext, strips, pen, diagramMapping, transform);
			RenderSecondLine(drawingContext, strips, diagramMapping, transform);
		}
	}
	public class AdditionalSplineSeriesGeometry : AdditionalLineSeriesGeometry {
		protected override bool CanRoundPoints { get { return true; } }
		public AdditionalSplineSeriesGeometry(XYSeries series)
			: base(series) {
		}
		protected override List<Point> GetLinePoints(IMapping mapping, IGeometryStrip geometryStrip) {
			BezierStrip bezierStrip = geometryStrip as BezierStrip;
			if (bezierStrip == null)
				return null;
			bezierStrip.ClipLargeValues = false;
			List<GRealPoint2D> pointsForDrawing = bezierStrip.GetPointsForDrawing(true, false);
			List<Point> points = new List<Point>();
			foreach (GRealPoint2D point in pointsForDrawing)
				points.Add(mapping.GetRoundedDiagramPoint(point.X, point.Y));
			return points;
		}
		protected override List<ChartGeometryFigure> CalculateLineFigures(IMapping mapping, IGeometryStrip strip, bool isTopStrip) {
			List<Point> points = GetLinePoints(mapping, strip);
			ChartGeometryFigure figure = CreatePolyBezierFigure(points);
			List<ChartGeometryFigure> figures = new List<ChartGeometryFigure>() { figure };
			return figures;
		}
	}
	public class AdditionalSplineAreaSeriesGeometry : AdditionalAreaSeriesGeometry {
		protected override bool CanRoundPoints { get { return true; } }
		public AdditionalSplineAreaSeriesGeometry(XYSeries series)
			: base(series) {
		}
		protected override List<Point> GetLinePoints(IMapping mapping, IGeometryStrip geometryStrip) {
			BezierRangeStrip bezierStrip = geometryStrip as BezierRangeStrip;
			if (bezierStrip == null)
				return base.GetLinePoints(mapping, geometryStrip);
			LineStrip topPointsForDrawing = new LineStrip();
			LineStrip bottomPointsForDrawing = new LineStrip();
			((BezierStrip)bezierStrip.TopStrip).ClipLargeValues = false;
			((BezierStrip)bezierStrip.BottomStrip).ClipLargeValues = false;
			bezierStrip.GetPointsForDrawing(out topPointsForDrawing, out bottomPointsForDrawing);
			List<Point> points = new List<Point>();
			foreach (GRealPoint2D point in topPointsForDrawing)
				points.Add(mapping.GetRoundedDiagramPoint(point.X, point.Y));
			return points;
		}
		protected override List<ChartGeometryFigure> CalculateLineFigures(IMapping mapping, IGeometryStrip strip, bool isTopStrip) {
			List<Point> points = GetLinePoints(mapping, strip);
			ChartGeometryFigure figure = CreatePolyBezierFigure(points);
			List<ChartGeometryFigure> figures = new List<ChartGeometryFigure>() { figure };
			return figures;
		}
		protected override List<Point> GetAreaPoints(IMapping mapping, RangeStrip strip) {
			List<Point> points = new List<Point>();
			BezierRangeStrip bezierRangeStrip = strip as BezierRangeStrip;
			if (bezierRangeStrip != null) {
				((BezierStrip)bezierRangeStrip.TopStrip).ClipLargeValues = false;
				((BezierStrip)bezierRangeStrip.BottomStrip).ClipLargeValues = false;
				LineStrip topPoints;
				LineStrip bottomPoints;
				bezierRangeStrip.GetPointsForDrawing(out topPoints, out bottomPoints, 0);
				foreach (GRealPoint2D point in topPoints)
					points.Add(mapping.GetDiagramPoint(point.X, point.Y));
				points.Add(points[points.Count - 1]);
				points.Add(points[points.Count - 1]);
				if (Series.HasAdditionalGeometryBottomStrip)
					foreach (GRealPoint2D point in bottomPoints)
						points.Add(mapping.GetDiagramPoint(point.X, point.Y));
			}
			return points;
		}
		protected override ChartGeometryFigure CreateAreaFigure(List<Point> points) {
			return CreatePolyBezierFigure(points);
		}
		protected override void GetPointsForDrawing(RangeStrip strip, out LineStrip topPoints, out LineStrip bottomPoints) {
			topPoints = strip.TopStrip;
			bottomPoints = strip.BottomStrip;
			BezierRangeStrip bezierRangeStrip = strip as BezierRangeStrip;
			if (bezierRangeStrip == null)
				return;
			((BezierStrip)bezierRangeStrip.TopStrip).ClipLargeValues = false;
			((BezierStrip)bezierRangeStrip.BottomStrip).ClipLargeValues = false;
			bezierRangeStrip.GetPointsForDrawing(out topPoints, out bottomPoints);
		}
	}
	public class AdditionalSplineStackedAreaSeriesGeometry : AdditionalSplineAreaSeriesGeometry {
		public AdditionalSplineStackedAreaSeriesGeometry(XYSeries series)
			: base(series) {
		}
		protected override List<ChartGeometryFigure> CalculateLineFigures(IMapping mapping, IGeometryStrip strip, bool isTopStrip) {
			List<ChartGeometryFigure> result = new List<ChartGeometryFigure>();
			BezierRangeStrip lineStrip = strip as BezierRangeStrip;
			if (lineStrip != null) {
				LineStrip topPoints = new LineStrip();
				LineStrip bottomPoints = new LineStrip();
				lineStrip.GetPointsForDrawing(out topPoints, out bottomPoints, 0);
				List<Point> points = new List<Point>();
				if (topPoints.Count >= 1)
					points.Add(mapping.GetRoundedDiagramPoint(topPoints[0].X, topPoints[0].Y));
				for (int i = 1; i < topPoints.Count; i++) {
					if (topPoints[i - 1].X != topPoints[i].X ||
						(topPoints[i - 1].X == topPoints[i].X && topPoints[i - 1].Y == topPoints[i].Y))
						points.Add(mapping.GetRoundedDiagramPoint(topPoints[i].X, topPoints[i].Y));
					else {
						result.Add(CreatePolyBezierFigure(points));
						points = new List<Point>();
						points.Add(mapping.GetRoundedDiagramPoint(topPoints[i].X, topPoints[i].Y));
					}
				}
				result.Add(CreatePolyBezierFigure(points));
			}
			return result;
		}
	}
}
