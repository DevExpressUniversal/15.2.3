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
namespace DevExpress.XtraCharts.Native {
	public sealed class ZExtendedCalculator {
		static void CalcBackAndFore(IList<DiagramPoint> contour, double depth, out List<DiagramPoint> backContour, out List<DiagramPoint> foreContour) {
			backContour = new List<DiagramPoint>(contour.Count);
			foreContour = new List<DiagramPoint>(contour.Count);
			double half = depth / 2;
			foreach (DiagramPoint point in contour) {
				backContour.Add(DiagramPoint.Offset(point, 0, 0, -half));
				foreContour.Add(DiagramPoint.Offset(point, 0, 0, half));
			}
		}
		static void CalcVerticesNormals(IList<PlanePolygon> envelope) {
			PlanePolygon previousPolygon = envelope[envelope.Count - 1];
			foreach (PlanePolygon polygon in envelope) {
				polygon.SameNormals = false;
				DiagramVector n = previousPolygon.Normal + polygon.Normal;
				n.Normalize();
				previousPolygon.Vertices[2].Normal = n;
				previousPolygon.Vertices[3].Normal = n;
				polygon.Vertices[0].Normal = n;
				polygon.Vertices[1].Normal = n;
				previousPolygon = polygon;
			}
		}
		static List<PlanePolygon> CalculateEnvelope(IList<DiagramPoint> contour, double depth, bool curving, Color color, int polygonWeight) {
			if (contour == null && contour.Count < 2)
				return new List<PlanePolygon>();
			List<DiagramPoint> backContour, foreContour;
			CalcBackAndFore(contour, depth, out backContour, out foreContour);
			int count = contour.Count;
			List<PlanePolygon> polygons = new List<PlanePolygon>(count);
			for (int index = 0, nextIndex = 1; index < count; index++, nextIndex++) {
				if (nextIndex == count)
					nextIndex = 0;
				PlaneQuadrangle quad = new PlaneQuadrangle(backContour[index], foreContour[index], foreContour[nextIndex], backContour[nextIndex]);
				quad.Normal = MathUtils.CalcNormal(quad);
				quad.Color = color;
				quad.SameColors = true;
				quad.Weight = polygonWeight;
				quad.SameNormals = true;
				polygons.Add(quad);
			}
			if (curving)
				CalcVerticesNormals(polygons);
			return polygons;
		}
		static List<PlanePolygon> CalculateLaterals(IList<DiagramPoint> contour, double depth, Color color, int polygonWeight) {
			if (contour == null || contour.Count < 3)
				return new List<PlanePolygon>();
			using (Tessellator tess = new Tessellator()) {
				IList<PlaneTriangle> triangles = tess.Triangulate(contour, new DiagramVector(0, 0, 1), color);
				if (triangles == null || triangles.Count == 0) 
					return new List<PlanePolygon>();
				List<PlanePolygon> container = new List<PlanePolygon>(triangles.Count * 2);
				double half = depth / 2;
				for (int i = 0; i < triangles.Count; i++) {
					triangles[i].Weight = polygonWeight;
					PlaneTriangle triangle = (PlaneTriangle)PlanePolygon.Offset(triangles[i], 0, 0, -half);
					triangle.InvertVerticesDirection();
					container.Add(triangle);
					container.Add((PlanePolygon)PlanePolygon.Offset(triangles[i], 0, 0, half));
				}
				return container;
			}
		}
		public static List<PlanePolygon> CalculateFigure(List<DiagramPoint> contour, double depth, bool curving, Color color, int polygonWeight) {
			List<PlanePolygon> container = new List<PlanePolygon>();
			container.AddRange(CalculateEnvelope(contour, depth, curving, color, polygonWeight));
			container.AddRange(CalculateLaterals(contour, depth, color, polygonWeight));
			return container;
		}
	}
}
