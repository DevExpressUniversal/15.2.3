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

using System.Collections.Generic;
using DevExpress.Utils;
namespace DevExpress.Charts.Native {
	public struct RangeStripTriangulationResult {
		readonly IList<GPolygon2D> polygons;
		readonly LineStrip borderStrip;
		public IList<GPolygon2D> Polygons { get { return polygons; } }
		public LineStrip BorderStrip { get { return borderStrip; } }
		public RangeStripTriangulationResult(IList<GPolygon2D> polygons, LineStrip borderStrip) {
			this.polygons = polygons;
			this.borderStrip = borderStrip;
		}
	}
	public class RangeStripTriangulation {
		readonly double epsilon;
		readonly double epsilonSquare;
		readonly LineStrip topStrip;
		readonly LineStrip bottomStrip;
		readonly int topStripLength;
		readonly int bottomStripLength;
		readonly List<GPolygon2D> polygons = new List<GPolygon2D>();
		readonly LineStrip topBorderStrip = new LineStrip();
		readonly LineStrip bottomBorderStrip = new LineStrip();
		public static RangeStripTriangulationResult Triangulate(RangeStrip strip, double epsilon) {
			RangeStripTriangulation triangulation = new RangeStripTriangulation(strip, epsilon);
			triangulation.Process();
			triangulation.bottomBorderStrip.Reverse();
			triangulation.topBorderStrip.AddRange(triangulation.bottomBorderStrip);
			return new RangeStripTriangulationResult(triangulation.polygons, triangulation.topBorderStrip);
		}
		RangeStripTriangulation(RangeStrip strip, double epsilon) {
			this.epsilon = epsilon;
			epsilonSquare = epsilon * epsilon;
			topStrip = strip.TopStrip.CreateUniqueStrip();
			bottomStrip = strip.BottomStrip.CreateUniqueStrip();
			topStripLength = topStrip.Count;
			bottomStripLength = bottomStrip.Count;
		}
		bool ArePointsEquals(GRealPoint2D p1, GRealPoint2D p2) {
			double xDiff = p2.X - p1.X;
			double yDiff = p2.Y - p1.Y;
			return ComparingUtils.CompareDoubles(xDiff * xDiff + yDiff * yDiff, 0, epsilonSquare) == 0;
		}
		void ProcessEmptyStrip() {
			if (topStripLength > 0 && bottomStripLength == 1) {
				GRealPoint2D bottomPoint = bottomStrip[0];
				bool isTopAndBottomPointsDifferent = false;
				foreach (GRealPoint2D topPoint in topStrip)
					if (!ArePointsEquals(topPoint, bottomPoint)) {
						topBorderStrip.Add(topPoint);
						isTopAndBottomPointsDifferent = true;
					}
				if (isTopAndBottomPointsDifferent)
					topBorderStrip.Add(bottomPoint);
			}
		}
		void AddPointsToBorderStrips(GRealPoint2D topPoint, GRealPoint2D bottomPoint) {
			if (bottomPoint.Y > topPoint.Y) {
				topBorderStrip.Add(bottomPoint);
				bottomBorderStrip.Add(topPoint);
			}
			else {
				topBorderStrip.Add(topPoint);
				bottomBorderStrip.Add(bottomPoint);
			}
		}
		GRealPoint2D MakeStep(LineStrip strip, ref GRealPoint2D point, ref int stripIndex) {
			int stripLength = strip.Count;
			while (stripIndex < stripLength) {
				GRealPoint2D nextPoint = strip[stripIndex++];
				if (ComparingUtils.CompareDoubles(point.X, nextPoint.X, epsilon) != 0)
					return nextPoint;
				GRealPoint2D lastTopPoint = topBorderStrip[topBorderStrip.Count - 1];
				GRealPoint2D lastBottomPoint = bottomBorderStrip.Count == 0 ? topBorderStrip[0] : bottomBorderStrip[bottomBorderStrip.Count - 1];
				if (point == lastBottomPoint)
					if (nextPoint.Y > lastTopPoint.Y) {
						if (lastTopPoint != lastBottomPoint)
							bottomBorderStrip.Add(lastTopPoint);
						topBorderStrip.Add(nextPoint);
					}
					else
						bottomBorderStrip.Add(nextPoint);
				else
					if (nextPoint.Y < lastBottomPoint.Y) {
						if (lastBottomPoint != lastTopPoint)
							topBorderStrip.Add(lastBottomPoint);
						bottomBorderStrip.Add(nextPoint);
					}
					else
						topBorderStrip.Add(nextPoint);
				point = nextPoint;
			} 
			return point;
		}
		void ProcessRegularStrip() {
			GRealPoint2D topPoint = topStrip[0];
			GRealPoint2D bottomPoint = bottomStrip[0];
			if (ArePointsEquals(topPoint, bottomPoint))
				topBorderStrip.Add(bottomPoint);
			else
				AddPointsToBorderStrips(topPoint, bottomPoint);
			int topStripIndex = 1;
			int bottomStripIndex = 1;
			do {
				GRealPoint2D nextTopPoint = MakeStep(topStrip, ref topPoint, ref topStripIndex);
				if (bottomStripIndex >= bottomStripLength)
					break;
				GRealPoint2D nextBottomPoint = MakeStep(bottomStrip, ref bottomPoint, ref bottomStripIndex);
				bool areNextPointsMatched = ArePointsEquals(nextTopPoint, nextBottomPoint);
				if (ArePointsEquals(topPoint, bottomPoint)) {
					if (!areNextPointsMatched)
						if (nextBottomPoint.Y > nextTopPoint.Y)
							polygons.Add(new GPolygon2D(new GRealPoint2D[] { nextBottomPoint, topPoint, nextTopPoint }));
						else
							polygons.Add(new GPolygon2D(new GRealPoint2D[] { nextTopPoint, topPoint, nextBottomPoint }));
				}
				else {
					bool reverted = bottomPoint.Y > topPoint.Y;
					if (areNextPointsMatched)
						if (reverted)
							polygons.Add(new GPolygon2D(new GRealPoint2D[] { nextTopPoint, bottomPoint, topPoint }));
						else
							polygons.Add(new GPolygon2D(new GRealPoint2D[] { nextTopPoint, topPoint, bottomPoint }));
					else {
						GRealPoint2D? intersectionPoint = GeometricUtils.CalcLinesIntersection(topPoint, nextTopPoint, bottomPoint, nextBottomPoint, true);
						if (intersectionPoint == null)
							if (reverted)
								polygons.Add(new GPolygon2D(new GRealPoint2D[] { nextBottomPoint, bottomPoint, topPoint, nextTopPoint }));
							else
								polygons.Add(new GPolygon2D(new GRealPoint2D[] { nextTopPoint, topPoint, bottomPoint, nextBottomPoint }));
						else {
							topBorderStrip.Add(intersectionPoint.Value);
							bottomBorderStrip.Add(intersectionPoint.Value);
							if (reverted) {
								polygons.Add(new GPolygon2D(new GRealPoint2D[] { intersectionPoint.Value, bottomPoint, topPoint }));
								polygons.Add(new GPolygon2D(new GRealPoint2D[] { nextTopPoint, intersectionPoint.Value, nextBottomPoint }));
							}
							else {
								polygons.Add(new GPolygon2D(new GRealPoint2D[] { intersectionPoint.Value, topPoint, bottomPoint }));
								polygons.Add(new GPolygon2D(new GRealPoint2D[] { nextBottomPoint, intersectionPoint.Value, nextTopPoint }));
							}
						}
					}
				}
				topPoint = nextTopPoint;
				bottomPoint = nextBottomPoint;
				if (areNextPointsMatched) {
					topBorderStrip.Add(topPoint);
					if (topStripIndex < topStripLength)
						bottomBorderStrip.Add(bottomPoint);
				}
				else
					AddPointsToBorderStrips(topPoint, bottomPoint);
			} while (topStripIndex < topStripLength);
		}
		void Process() {
			if (topStripLength < 2 || bottomStripLength < 2)
				ProcessEmptyStrip();
			else
				ProcessRegularStrip();
		}
	}
}
