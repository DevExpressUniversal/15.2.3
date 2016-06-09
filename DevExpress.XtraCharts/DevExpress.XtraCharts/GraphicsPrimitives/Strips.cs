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
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public static class StripsUtils {
		const double stripLimitValue = 150000000.0;
		static GRealPoint2D CalcStepPoint(GRealPoint2D p1, GRealPoint2D p2, bool invertedStep) {
			return invertedStep ? new GRealPoint2D(p1.X, p2.Y) : new GRealPoint2D(p2.X, p1.Y);
		}
		public static LineStrip CalcLegendStepLinePoints(Rectangle bounds, bool invertedStep) {
			int lineLevel = bounds.Top + (bounds.Height - 1) / 2;
			GRealPoint2D p1 = new GRealPoint2D(bounds.Left, bounds.Top);
			GRealPoint2D p2 = new GRealPoint2D(bounds.Left + bounds.Width / 2, lineLevel);
			GRealPoint2D p3 = new GRealPoint2D(bounds.Right - 1, bounds.Bottom - 1);
			GRealPoint2D step1 = CalcStepPoint(p1, p2, invertedStep);
			GRealPoint2D step2 = CalcStepPoint(p2, p3, invertedStep);
			return new LineStrip(new GRealPoint2D[] { p1, step1, p2, step2, p3 });
		}
		public static RangeStrip CreateAreaMarkerStrip(IGeometryStripCreator creator, Rectangle bounds, bool pointMarkerVisible) {
			RectangleF rect = new RectangleF(bounds.Location, bounds.Size);
			double middle = rect.Left + rect.Width / 2.0;
			double top = rect.Top;
			if (pointMarkerVisible)
				top += rect.Height / 4.0;
			RangeStrip rangeStrip = creator.CreateStrip() as RangeStrip;
			if (rangeStrip != null) {
				rangeStrip.Add(new StripRange(new GRealPoint2D(rect.Left, rect.Bottom), new GRealPoint2D(rect.Left, rect.Bottom)));
				if (rangeStrip is BezierRangeStrip) {
					double middleOffset = rect.Width / 7;
					top += rect.Height / 16;
					rangeStrip.Add(new StripRange(new GRealPoint2D(middle - middleOffset, top), new GRealPoint2D(middle - middleOffset, rect.Bottom)));
					rangeStrip.Add(new StripRange(new GRealPoint2D(middle + middleOffset, top), new GRealPoint2D(middle + middleOffset, rect.Bottom)));
				}
				else
					rangeStrip.Add(new StripRange(new GRealPoint2D(middle, top), new GRealPoint2D(middle, rect.Bottom)));
				rangeStrip.Add(new StripRange(new GRealPoint2D(rect.Right, rect.Bottom), new GRealPoint2D(rect.Right, rect.Bottom)));
			}
			return rangeStrip;
		}
		public static List<IGeometryStrip> MapLineStrips(IXYDiagramMapping diagramMapping, IList<IGeometryStrip> strips) {
			List<IGeometryStrip> screenStrips = new List<IGeometryStrip>();
			foreach (LineStrip strip in strips) 
				screenStrips.Add(MapStrip(diagramMapping, strip));
			return screenStrips;
		}
		public static List<IGeometryStrip> MapLineStrips(XYDiagram3DCoordsCalculator coordsCalculator, IList<IGeometryStrip> strips) {
			List<IGeometryStrip> screenStrips = new List<IGeometryStrip>();
			foreach (LineStrip strip in strips) 
				screenStrips.Add(MapStrip(coordsCalculator, strip));
			return screenStrips;
		}
		public static List<IGeometryStrip> MapRangeStrips(IXYDiagramMapping diagramMapping, IList<IGeometryStrip> strips) {
			List<IGeometryStrip> screenStrips = new List<IGeometryStrip>();
			foreach (RangeStrip strip in strips) 
				screenStrips.Add(MapStrip(diagramMapping, strip));
			return screenStrips;
		}
		public static List<IGeometryStrip> MapRangeStrips(XYDiagram3DCoordsCalculator coordsCalculator, IList<IGeometryStrip> strips) {
			List<IGeometryStrip> screenStrips = new List<IGeometryStrip>();
			foreach (RangeStrip strip in strips) 
				screenStrips.Add(MapStrip(coordsCalculator, strip));
			return screenStrips;
		}
		public static LineStrip MapStrip(IXYDiagramMapping mapping, LineStrip lineStrip) {
			LineStrip screenStrip = lineStrip.CreateInstance();
			foreach (GRealPoint2D point in lineStrip) {
				DiagramPoint mappedPoint = mapping.GetScreenPoint(point.X, point.Y);
				double pointX = mappedPoint.X;
				double pointY = mappedPoint.Y;
				if (mappedPoint.Y > stripLimitValue)
					pointY = stripLimitValue;
				if (mappedPoint.Y < -stripLimitValue)
					pointY = -stripLimitValue;
				if (mappedPoint.X > stripLimitValue)
					pointX = stripLimitValue;
				if (mappedPoint.X < -stripLimitValue)
					pointX = -stripLimitValue;
				screenStrip.Add(new GRealPoint2D(pointX, pointY));
			}
			BezierStrip bezierStrip = lineStrip as BezierStrip;
			BezierStrip screenBezierStrip = screenStrip as BezierStrip;
			if (bezierStrip != null && screenBezierStrip != null) {
				IList<GRealPoint2D> drawingPoints = bezierStrip.GetPointsForDrawing(false, false);
				if (drawingPoints != null) {
					List<GRealPoint2D> screenDrawingPoints = new List<GRealPoint2D>(drawingPoints.Count);
					foreach (GRealPoint2D point in drawingPoints) {
						DiagramPoint mappedPoint = mapping.GetScreenPoint(point.X, point.Y);
						screenDrawingPoints.Add(new GRealPoint2D(mappedPoint.X, mappedPoint.Y));  
					}
					screenBezierStrip.SetPointsForDrawing(screenDrawingPoints);
				}
			}
			return screenStrip;
		}
		public static LineStrip MapStrip(XYDiagram3DCoordsCalculator coordsCalculator, LineStrip lineStrip) {
			LineStrip screenStrip = lineStrip.CreateInstance();
			foreach (GRealPoint2D point in lineStrip) {
				DiagramPoint mappedPoint = coordsCalculator.GetDiagramPoint(point.X, point.Y, false);
				screenStrip.Add(new GRealPoint2D(mappedPoint.X, mappedPoint.Y));
			}
			BezierStrip bezierStrip = lineStrip as BezierStrip;
			BezierStrip screenBezierStrip = screenStrip as BezierStrip;
			if (bezierStrip != null && screenBezierStrip != null) {
				IList<GRealPoint2D> drawingPoints = bezierStrip.GetPointsForDrawing(false, false);
				if (drawingPoints != null) {
					List<GRealPoint2D> screenDrawingPoints = new List<GRealPoint2D>(drawingPoints.Count);
					foreach (GRealPoint2D point in drawingPoints) {
						DiagramPoint mappedPoint = coordsCalculator.GetDiagramPoint(point.X, point.Y, false);
						screenDrawingPoints.Add(new GRealPoint2D(mappedPoint.X, mappedPoint.Y));  
					}
					screenBezierStrip.SetPointsForDrawing(screenDrawingPoints);
				}
			}
			return screenStrip;
		}
		public static RangeStrip MapStrip(IXYDiagramMapping mapping, RangeStrip rangeStrip) {
			RangeStrip screenStrip = rangeStrip.CreateInstance();
			screenStrip.TopStrip = StripsUtils.MapStrip(mapping, rangeStrip.TopStrip);
			screenStrip.BottomStrip = StripsUtils.MapStrip(mapping, rangeStrip.BottomStrip);
			return screenStrip;
		}
		public static RangeStrip MapStrip(XYDiagram3DCoordsCalculator coordsCalculator, RangeStrip rangeStrip) {
			RangeStrip screenStrip = rangeStrip.CreateInstance();
			screenStrip.TopStrip = StripsUtils.MapStrip(coordsCalculator, rangeStrip.TopStrip);
			screenStrip.BottomStrip = StripsUtils.MapStrip(coordsCalculator, rangeStrip.BottomStrip);
			return screenStrip;
		}
		public static GraphicsPath GetPath(LineStrip lineStrip, int thickness, LineStyle lineStyle) {
			GraphicsPath path = new GraphicsPath();
			BezierStrip bezierStrip = lineStrip as BezierStrip;
			if (bezierStrip == null) {
				LineStrip uniqueStrip = lineStrip.CreateUniqueStrip();
				ZPlaneRectangle bounds = ZPlaneRectangle.MakeRectangle(uniqueStrip);
				if (bounds.Width < 0.5 && bounds.Height < 0.5)
					return path;
				path.AddLines(Convert(uniqueStrip));
			}
			else {			
				if (bezierStrip.IsEmpty)
					return path;
				List<GRealPoint2D> points = bezierStrip.GetPointsForDrawing(true, true);
				ZPlaneRectangle bounds = ZPlaneRectangle.MakeRectangle(points);
				if (bounds.Width < 0.5 && bounds.Height < 0.5)
					return path;
				path.AddBeziers(Convert(points));
			}
			using (Pen pen = new Pen(Color.Empty, thickness)) {
				if (lineStyle != null)
					pen.LineJoin = lineStyle.LineJoin;
				path.Widen(pen);
			}
			return path;
		}
		public static GraphicsPath GetPath(LineStrip lineStrip, int thickness) {
			return GetPath(lineStrip, thickness, null);
		}
		public static GraphicsPath GetPath(LineStrip lineStrip) {
			return GetPath(lineStrip, 1);
		}
		public static GraphicsPath GetPath(RangeStrip rangeStrip) {
			GraphicsPath path = new GraphicsPath();
			BezierRangeStrip bezierStrip = rangeStrip as BezierRangeStrip;
			if (bezierStrip == null) {
				ZPlaneRectangle rect;
				LineStrip vertices = FillVertices(rangeStrip, out rect);
				if (vertices.Count > 1) 
					path.AddLines(StripsUtils.Convert(vertices));
			}
			else if (bezierStrip.IsEmpty) {
				if (bezierStrip.Count == 1) {
					GRealRect2D boundingRectangle = bezierStrip.GetBoundingRectangle();
					RectangleF bounds = new RectangleF((float)boundingRectangle.Left, (float)boundingRectangle.Top, (float)boundingRectangle.Width, (float)boundingRectangle.Height);
					path.AddRectangle(bounds);
				}
			}
			else {
				LineStrip topPoints, bottomPoints;
				bezierStrip.GetPointsForDrawing(out topPoints, out bottomPoints);
				path.AddBeziers(StripsUtils.Convert(topPoints));
				path.AddBeziers(StripsUtils.Convert(bottomPoints));
			}
			return path;
		}
		public static void Render(IRenderer renderer, LineStrip lineStrip, Color color, int thickness) {
			BezierStrip bezierStrip = lineStrip as BezierStrip;
			if (bezierStrip == null)
				renderer.DrawLines(lineStrip, color, thickness, null, LineCap.Flat);
			else
				renderer.DrawBezier(bezierStrip, color, thickness);
		}
		public static void Render(IRenderer renderer, RangeStrip rangeStrip, FillOptionsBase fillOptions, Color color, Color color2, SeriesHitTestState hitTestState, ZPlaneRectangle mappingBounds, SelectionState selectionState) {
			BezierRangeStrip bezierStrip = rangeStrip as BezierRangeStrip;
			if (bezierStrip == null) {
				if (rangeStrip != null) {
					ZPlaneRectangle boundingRectangle;
					LineStrip vertices = FillVertices(rangeStrip, out boundingRectangle);
					PolygonGradientFillOptions gradientFillOptions = fillOptions as PolygonGradientFillOptions;
					if (mappingBounds != null && gradientFillOptions != null &&
						(gradientFillOptions.GradientMode == PolygonGradientMode.ToCenter ||
						 gradientFillOptions.GradientMode == PolygonGradientMode.FromCenter))
						boundingRectangle = mappingBounds;
					fillOptions.Render(renderer, vertices, (RectangleF)boundingRectangle, color, color2);
					if (selectionState != SelectionState.Normal)
						renderer.FillPolygon(vertices, hitTestState.HatchStyle, hitTestState.GetPointHatchColor(selectionState));
				}
			}
			else {
				fillOptions.Render(renderer, bezierStrip, color, color2);
				if (selectionState != SelectionState.Normal)
					renderer.FillBezier(bezierStrip, hitTestState.HatchStyle, hitTestState.GetPointHatchColor(selectionState));
			}
		}
		public static void Render(IRenderer renderer, RangeStrip rangeStrip, Color color) {
			BezierRangeStrip bezierStrip = rangeStrip as BezierRangeStrip;
			if (bezierStrip == null) {
				ZPlaneRectangle boundingRectangle;
				LineStrip strip = FillVertices(rangeStrip, out boundingRectangle);
				renderer.FillPolygon(strip, color);
				return;
			}
			renderer.FillBezier(bezierStrip, color);
		}
		public static PointF[] Convert(IList<GRealPoint2D> points, bool skipEqualPoints) {
			int count = points.Count;
			List<PointF> result = new List<PointF>(count);
			if (count > 0) {
				PointF previousPoint = new PointF((float)points[0].X, (float)points[0].Y);
				result.Add(previousPoint);
				for (int i = 1; i < count; i++) {
					PointF point = new PointF((float)points[i].X, (float)points[i].Y);
					if (skipEqualPoints && point == previousPoint)
						continue;
					result.Add(point);
					previousPoint = point;
				}
			}
			return result.ToArray();
		}
		public static PointF[] Convert(IList<GRealPoint2D> points) {
			return Convert(points, false);
		}
		public static LineStrip FillVertices(RangeStrip rangeStrip, out ZPlaneRectangle boundingRectangle) {
			boundingRectangle = ZPlaneRectangle.Empty;
			if (rangeStrip.Count == 0)
				return new LineStrip();
			LineStrip vertices = new LineStrip();
			vertices.AddRange(rangeStrip.TopStrip);
			for (int i = rangeStrip.BottomStrip.Count - 1; i >= 0; i--)
				vertices.Add(rangeStrip.BottomStrip[i]);
			LargeScaleHelper.Validate(vertices);
			boundingRectangle = ZPlaneRectangle.MakeRectangle(vertices);
			boundingRectangle = LargeScaleHelper.Validate(boundingRectangle);
			if (boundingRectangle.Width < 0.5 || boundingRectangle.Height < 0.5) {
				if (boundingRectangle.Width < 0.5)
					boundingRectangle.Inflate(0.5f, 0.0f);
				if (boundingRectangle.Height < 0.5)
					boundingRectangle.Inflate(0.0f, 0.5f);
				vertices = new LineStrip(4);
				vertices.Add(new GRealPoint2D(boundingRectangle.Location.X, boundingRectangle.Location.Y));
				vertices.Add(new GRealPoint2D(boundingRectangle.Location.X + boundingRectangle.Width, boundingRectangle.Location.Y));
				vertices.Add(new GRealPoint2D(boundingRectangle.Location.X + boundingRectangle.Width, boundingRectangle.Location.Y + boundingRectangle.Height));
				vertices.Add(new GRealPoint2D(boundingRectangle.Location.X, boundingRectangle.Location.Y + boundingRectangle.Height));
			}
			return vertices;
		}
	}
}
