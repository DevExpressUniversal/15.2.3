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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public static class SeriesView3DHelper {
		public static List<PlanePolygon> GetPolygons(XYDiagram3DCoordsCalculator coordsCalculator, Prizm prizm, int seriesWeight, Color color) {
			List<PlanePolygon> result = new List<PlanePolygon>();
			PlanePolygon[] clippedPolygons = coordsCalculator.ClipPolyhedron(prizm.Polygons);
			if (clippedPolygons != null) 
				foreach (PlanePolygon polygon in coordsCalculator.ClipPolyhedron(prizm.Polygons))
					if (polygon.Visible) {
						polygon.Weight = seriesWeight;
						polygon.Color = color;
						polygon.SameColors = true;
						result.Add(polygon);
					}
			return result;
		}
		public static List<PlanePolygon> GetPolygons(XYDiagram3DCoordsCalculator coordsCalculator, Line topLine, Line bottomLine, bool leftVisible, bool rightVisible, double seriesWidth, int seriesWeight, Color color) {
			Prizm prizm = new Prizm(new PlanePolygon(new DiagramPoint[] { topLine.V2, topLine.V1, bottomLine.V1, bottomLine.V2 }), seriesWidth);
			prizm.Laterals[1].Visible = leftVisible;
			prizm.Laterals[3].Visible = rightVisible;
			return GetPolygons(coordsCalculator, prizm, seriesWeight, color);
		}
		public static List<PlanePolygon> GetPolygons(XYDiagram3DCoordsCalculator coordsCalculator, DiagramPoint point, Line line, bool lineVisible, bool sidesVisible, double seriesWidth, int seriesWeight, Color color) {
			Prizm prizm = new Prizm(new PlanePolygon(new DiagramPoint[] { point, line.V1, line.V2 }), seriesWidth);
			prizm.Laterals[0].Visible = sidesVisible;
			prizm.Laterals[1].Visible = lineVisible;
			prizm.Laterals[2].Visible = sidesVisible;
			return GetPolygons(coordsCalculator, prizm, seriesWeight, color);
		}
		public static List<PlanePolygon> GetPolygons(XYDiagram3DCoordsCalculator coordsCalculator, Series series, IList<IGeometryStrip> strips, double minLimit, double maxLimit, Color color) {
			List<PlanePolygon> result = new List<PlanePolygon>();
			XYDiagram3DSeriesViewBase view = series.View as XYDiagram3DSeriesViewBase;
			if (view == null)
				return result;
			double z = coordsCalculator.CalcSeriesOffset(series);
			double zoomingFactor = coordsCalculator.Diagram.ZoomPercent / 100.0;
			double seriesWidth = coordsCalculator.CalcSeriesWidth(view);
			int seriesWeight = coordsCalculator.GetSeriesWeight(series);
			foreach (RangeStrip strip in StripsUtils.MapRangeStrips(coordsCalculator, strips)) {
				List<GRealPoint2D> topStrip = BezierUtils.CalcBezier(strip.TopStrip as BezierStrip, zoomingFactor);
				List<GRealPoint2D> bottomStrip = BezierUtils.CalcBezier(strip.BottomStrip as BezierStrip, zoomingFactor);
				List<DiagramPoint> contour = new List<DiagramPoint>();
				if (topStrip.Count > 0) {
					contour.Add(new DiagramPoint(topStrip[0].X, topStrip[0].Y, z));
					foreach (GRealPoint2D point in topStrip)
						contour.Add(new DiagramPoint(point.X, point.Y, z));
					contour.Add(new DiagramPoint(topStrip[topStrip.Count - 1].X, topStrip[topStrip.Count - 1].Y, z));
				}
				if (bottomStrip.Count > 0) {
					int lastIndex = bottomStrip.Count - 1;
					contour.Add(new DiagramPoint(bottomStrip[lastIndex].X, bottomStrip[lastIndex].Y, z));
					for (int i = lastIndex; i >= 0; i--)
						contour.Add(new DiagramPoint(bottomStrip[i].X, bottomStrip[i].Y, z));
					contour.Add(new DiagramPoint(bottomStrip[0].X, bottomStrip[0].Y, z));
				}
				List<IList<DiagramPoint>> contours = ContourUtils.CalcIntersection(contour,  
					new List<DiagramPoint>(coordsCalculator.GetSeriesBounds(series, minLimit, maxLimit)), true, Diagram3D.Epsilon);
				if (contours != null)
					foreach (List<DiagramPoint> item in contours)
						result.AddRange(ZExtendedCalculator.CalculateFigure(item, seriesWidth, true, color, seriesWeight));
			}
			return result;
		}
	}
}
