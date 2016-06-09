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
using DevExpress.XtraMap.Drawing;
namespace DevExpress.XtraMap.Native {
	public enum GeometryType { Unit, Screen };
	public class PolylineToPolygonConverter {
		void InitPointsFromFirstToCurrent(int cuurentPointIndex, PointF[] values, PointF[] result) {
			int lastIndex = result.Length - 1;
			for (int i = 0; i <= cuurentPointIndex; i++) {
				result[i] = values[0];
				result[lastIndex - i] = values[1];
			}
		}
		PointF[] CreateSinglePointPath(PointF point, float lineThickness) {
			float left = point.X - lineThickness;
			float right = point.X + lineThickness;
			float top = point.Y - lineThickness;
			float bottom = point.Y + lineThickness;
			return new PointF[] { new PointF(left, bottom), new PointF(left, top), new PointF(right, top), new PointF(right, bottom) };
		}
		PointF[] GetPreviousPoints(int index, PointF[] points, int pointsCount) {
			return new PointF[] { points[index - 1], points[pointsCount - index] };
		}
		PointF[] CalcCrossPoints(float coefA1, float coefA2, float coefB1, float coefB2, PointF crossPoint, float line1Distance, float line2Distance, float denominator) {
			float[] line1ParallelCoeffsC = CalcParallelCoeffsC(crossPoint, coefA1, coefB1, line1Distance);
			float[] line2ParallelCoeffsC = CalcParallelCoeffsC(crossPoint, coefA2, coefB2, line2Distance);
			float coeffUpperC1 = line1ParallelCoeffsC[0];
			float coeffUpperC2 = line2ParallelCoeffsC[0];
			float coeffLowerC1 = line1ParallelCoeffsC[1];
			float coeffLowerC2 = line2ParallelCoeffsC[1];
			float upperLinesCrossY = (coefA2 * coeffUpperC1 - coefA1 * coeffUpperC2) / denominator;
			float lowerLinesCrossY = (coefA2 * coeffLowerC1 - coefA1 * coeffLowerC2) / denominator;
			float upperLinesCrossX;
			float lowerLinesCrossX;
			if (coefA1 != 0.0f) {
				upperLinesCrossX = (-coefB1 * upperLinesCrossY - coeffUpperC1) / coefA1;
				lowerLinesCrossX = (-coefB1 * lowerLinesCrossY - coeffLowerC1) / coefA1;
			}
			else if (coefA2 != 0.0f) {
				upperLinesCrossX = (-coefB2 * upperLinesCrossY - coeffUpperC2) / coefA2;
				lowerLinesCrossX = (-coefB2 * lowerLinesCrossY - coeffLowerC2) / coefA2;
			}
			else {
				upperLinesCrossX = crossPoint.X;
				lowerLinesCrossX = crossPoint.X;
			}
			return new PointF[] { new PointF(upperLinesCrossX, upperLinesCrossY), new PointF(lowerLinesCrossX, lowerLinesCrossY) };
		}
		float GetDenominator(float coefA1, float coefA2, float coefB1, float coefB2) {
			return coefA1 * coefB2 - coefA2 * coefB1;
		}
		PointF[] CalcParallelLinesCross(PointF startPoint, PointF crossPoint, PointF endPoint, float distance) {
			float coefA1 = crossPoint.Y - startPoint.Y;
			float coefB1 = startPoint.X - crossPoint.X;
			float coefA2 = endPoint.Y - crossPoint.Y;
			float coefB2 = crossPoint.X - endPoint.X;
			float denominator = GetDenominator(coefA1, coefA2, coefB1, coefB2);
			if (denominator == 0.0f)
				return CalcCrossAtExtremePoints(startPoint, crossPoint, crossPoint, distance);
			else
				return CalcCrossPoints(coefA1, coefA2, coefB1, coefB2, crossPoint, distance, distance, denominator);
		}
		PointF[] CalcCrossAtExtremePoints(PointF startPoint, PointF crossPoint, PointF endPoint, float distance) {
			float coefA1 = endPoint.Y - startPoint.Y;
			float coefB1 = startPoint.X - endPoint.X;
			float coefA2 = coefB1;
			float coefB2 = -coefA1;
			float denominator = GetDenominator(coefA1, coefA2, coefB1, coefB2);
			return CalcCrossPoints(coefA1, coefA2, coefB1, coefB2, crossPoint, distance, 0, denominator);
		}
		float[] CalcParallelCoeffsC(PointF point, float coefA, float coefB, float thickness) {
			float offset = (float)Math.Sqrt(coefA * coefA + coefB * coefB) * thickness;
			float coordSum = coefA * point.X + coefB * point.Y;
			return new float[] { offset - coordSum, -coordSum - offset };
		}
		public PointF[] Convert(PointF[] points, float lineThickness) {
			int lastSourceIndex = points.Length - 1;
			int count = points.Length * 2;
			int lastIndex = count - 1;
			float thicknessHalf = lineThickness / 2.0f;
			bool isClosed = points[0] == points[lastSourceIndex];
			PointF[] result = new PointF[count];
			bool equalPoints = false;
			for (int i = 0; i <= lastSourceIndex; i++) {
				PointF[] crossPoints;
				PointF currentPoint = points[i];
				if (i == 0 || equalPoints) {
					if (i == lastSourceIndex) {
						result = CreateSinglePointPath(currentPoint, thicknessHalf);
						break;
					}
					PointF nextPoint = points[i + 1];
					equalPoints = currentPoint == nextPoint;
					if (!equalPoints) {
						PointF[] startPoints;
						if (isClosed)
							startPoints = CalcParallelLinesCross(points[lastSourceIndex - 1], currentPoint, nextPoint, thicknessHalf);
						else
							startPoints = CalcCrossAtExtremePoints(currentPoint, currentPoint, nextPoint, thicknessHalf);
						InitPointsFromFirstToCurrent(i, startPoints, result);
					}
					continue;
				}
				else {
					PointF previousPoint = points[i - 1];
					if (currentPoint == previousPoint)
						crossPoints = GetPreviousPoints(i, result, count);
					else if (i != lastSourceIndex)
						crossPoints = CalcParallelLinesCross(previousPoint, currentPoint, points[i + 1], thicknessHalf);
					else if (isClosed)
						crossPoints = CalcParallelLinesCross(previousPoint, currentPoint, points[1], thicknessHalf);
					else
						crossPoints = CalcCrossAtExtremePoints(previousPoint, currentPoint, currentPoint, thicknessHalf);
				}
				result[i] = crossPoints[0];
				result[lastIndex - i] = crossPoints[1];
			}
			return result;
		}
	}
	public abstract class PathHitTestGeometryBase {
		readonly PointF[] path;
		readonly IList<PointF[]> innerBoundaries = new List<PointF[]>();
		protected PointF[] Path { get { return path; } }
		protected IList<PointF[]> InnerBoundaries { get { return innerBoundaries; } }
		protected PathHitTestGeometryBase(PointF[] points, IList<PointF[]> innerBoundaries, float borderThickness) {
			this.path = points.Length > 1 ? CreatePath(points, borderThickness) : new PointF[] { };
			if(innerBoundaries != null) {
				foreach(PointF[] innerContour in innerBoundaries) {
					if(innerContour.Length > 1)
						InnerBoundaries.Add(CreatePath(innerContour, borderThickness));
				}
			}
		}
		protected bool CheckPoint(float testPointX, float testPointY) {
			foreach(PointF[] innerBoundary in InnerBoundaries)
				if(CheckPath(innerBoundary, testPointX, testPointY)) 
					return false;
			return CheckPath(path, testPointX, testPointY);
		}
		protected bool CheckPath(PointF[] path, float testPointX, float testPointY) {
			int intersectCount = 0;
			PointF previousPoint = path[path.Length - 1];
			bool isPreviousPointUnderTest = previousPoint.Y < testPointY;
			foreach(PointF currentPoint in path) {
				bool isCurrentPointUnderTest = currentPoint.Y < testPointY;
				PointF vectorToPrev = new PointF(previousPoint.X - testPointX, previousPoint.Y - testPointY);
				PointF vectorToCur = new PointF(currentPoint.X - testPointX, currentPoint.Y - testPointY);
				PointF subVector = new PointF(vectorToCur.Y - vectorToPrev.Y, vectorToCur.X - vectorToPrev.X);
				float dotProduct = vectorToPrev.X * subVector.X - vectorToPrev.Y * subVector.Y;
				bool intersectsAtTheRightUpToDown = isCurrentPointUnderTest && !isPreviousPointUnderTest && dotProduct > 0;
				bool intersectsAtTheRightDownToUp = !isCurrentPointUnderTest && isPreviousPointUnderTest && dotProduct < 0;
				if(intersectsAtTheRightUpToDown || intersectsAtTheRightDownToUp)
					intersectCount += 1;
				previousPoint = currentPoint;
				isPreviousPointUnderTest = isCurrentPointUnderTest;
			}
			return (intersectCount & 1) != 0;
		}			
		protected virtual RectangleF CalcBounds(float thickness) {
			return CalcBoundsCore(Path, thickness);
		}
		protected RectangleF CalcBoundsCore(PointF[] points, float thickness) {
			if(points.Length > 0) {
				float minX = points[0].X;
				float minY = points[0].Y;
				float maxX = points[0].X;
				float maxY = points[0].Y;
				for(int i = 1; i < points.Length; i++) {
					minX = Math.Min(minX, points[i].X);
					minY = Math.Min(minY, points[i].Y);
					maxX = Math.Max(maxX, points[i].X);
					maxY = Math.Max(maxY, points[i].Y);
				}
				return new RectangleF(minX - thickness / 2.0f, minY - thickness / 2.0f, maxX - minX + thickness, maxY - minY + thickness);
			}
			return RectangleF.Empty;
		}
		protected abstract PointF[] CreatePath(PointF[] points, float borderThickness);
	}
	public class RectangleScreenHitTestGeometry : IScreenHitTestGeometry {
		readonly MapRect bounds;
		public RectangleScreenHitTestGeometry(MapPoint location, Size size, MapPoint renderOrigin) {
			this.bounds = new MapRect(location.X - size.Width * renderOrigin.X, location.Y - size.Height * renderOrigin.Y, size.Width, size.Height);
		}
		public bool HitTest(Point screenPoint) {
			return bounds.Contains(new MapPoint(screenPoint.X, screenPoint.Y));
		}
	}
	public class PathScreenHitTestGeometry : PathHitTestGeometryBase, IScreenHitTestGeometry {
		readonly RectangleF bounds;
		public PathScreenHitTestGeometry(PointF[] points, IList<PointF[]> innerBoundaries, float thickness)
			: base(points, innerBoundaries, thickness) {
			this.bounds = CalcBounds(thickness);
		}
		protected override PointF[] CreatePath(PointF[] points, float borderThickness) {
			return points;
		}
		public bool HitTest(Point screenPoint) {
			if(!this.bounds.Contains(screenPoint))
				return false;
			return HitTestOverride(screenPoint);
		}
		protected virtual bool HitTestOverride(Point screenPoint) {
			return Path != null && Path.Length > 1 ? CheckPoint(screenPoint.X, screenPoint.Y) : false;
		}
	}
	public class StrokeHitTestGeometry : PathScreenHitTestGeometry {
		public StrokeHitTestGeometry(PointF[] points, float thickness) :
			base(points, new List<PointF[]>(), thickness) {
		}
		protected override PointF[] CreatePath(PointF[] points, float borderThickness) {
			PolylineToPolygonConverter converter = new PolylineToPolygonConverter();
			return converter.Convert(points, borderThickness);
		}
	}
	public class MultiLineHitTestGeometry : PathScreenHitTestGeometry {
		public MultiLineHitTestGeometry(IList<PointF[]> points, float thickness)
			: base(new PointF[0], points, thickness) {
		}
		protected override RectangleF CalcBounds(float thickness) {
			RectangleF result = RectangleF.Empty;
			for(int i = 0; i < InnerBoundaries.Count; i++) {
				RectangleF segmentBounds = CalcBoundsCore(InnerBoundaries[i], thickness);
				result = result.IsEmpty ? segmentBounds : RectangleF.Union(result, segmentBounds);
			}
			return result;
		}
		protected override PointF[] CreatePath(PointF[] points, float borderThickness) {
			PolylineToPolygonConverter converter = new PolylineToPolygonConverter();
			return converter.Convert(points, borderThickness);
		}
		protected override bool HitTestOverride(Point screenPoint) {
			for(int i = 0; i < InnerBoundaries.Count; i++)
				if(CheckPath(InnerBoundaries[i], screenPoint.X, screenPoint.Y))
					return true;
			return false;
		}
	}
	public class PathHitTestGeometry : PathHitTestGeometryBase, IUnitHitTestGeometry {
		public static PathHitTestGeometry CreateFromUnitGeometry(UnitGeometry geometry) {
			MapUnit[] geometryPoints = geometry.Points;
			int pointCount = geometryPoints.Length;
			IList<PointF[]> innerBoundaries = new List<PointF[]>();
			PathUnitGeometry pathGeometry = geometry as PathUnitGeometry;
			if(pathGeometry != null) {
				foreach(MapUnit[] contour in pathGeometry.InnerContours) {
					if(contour.Length < 1) continue;
					PointF[] contourF = new PointF[contour.Length];
					for(int i = 0; i < contour.Length; i++) 
						contourF[i] = new PointF((float)contour[i].X, (float)contour[i].Y);
					innerBoundaries.Add(contourF);
				}
			}
			if(pointCount > 0) {
				PointF[] points = new PointF[pointCount];
				for(int i = 0; i < pointCount; i++) {
					MapUnit mapUnit = geometryPoints[i];
					points[i] = new PointF((float)mapUnit.X, (float)mapUnit.Y);
				}
				return new PathHitTestGeometry(points, innerBoundaries);
			}
			return null;
		}
		public PathHitTestGeometry(PointF[] geometryPoints, IList<PointF[]> innerBoundaries)
			: base(geometryPoints, innerBoundaries, 0) {
		}
		protected override PointF[] CreatePath(PointF[] points, float borderThickness) {
			return points;
		}
		public bool HitTest(MapUnit mapUnit) {
			return Path != null && Path.Length > 1 ? CheckPoint((float)mapUnit.X, (float)mapUnit.Y) : false;
		}
	}
}
