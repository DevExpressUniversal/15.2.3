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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public enum AdditionalGeometryCacheKey {
		Line,
		Area,
		Line2
	}
	public abstract class ChartGeometrySegmentBase {
		readonly List<Point> points;
		readonly bool shouldRoundPoints;
		protected abstract bool ShouldOptimizePoints { get; }
		public List<Point> Points { get { return points; } }
		protected ChartGeometrySegmentBase(List<Point> points, bool shouldRoundPoints) {
			this.points = points;
			this.shouldRoundPoints = shouldRoundPoints;
		}
		bool SamePixels(double d1, double d2) {
			return Math.Floor(d1) == Math.Floor(d2);
		}
		void AddPointToListConditional(List<Point> points, Point point, bool condition) {
			if (condition)
				points.Add(point);
		}
		List<Point> OptimizePoints(List<Point> sourcePoints) {
			List<Point> resultPoints = new List<Point>();
			int pointsCount = sourcePoints.Count;
			for (int sourcePointIndex = 0; sourcePointIndex < pointsCount; sourcePointIndex++) {
				Point sourcePoint = sourcePoints[sourcePointIndex];
				double minY = sourcePoint.Y;
				double maxY = sourcePoint.Y;
				Point lastPoint = new Point();
				int minPointIndex = sourcePointIndex;
				int maxPointIndex = sourcePointIndex;
				int lastPointIndex = sourcePointIndex;
				for (int nextPointIndex = sourcePointIndex + 1; nextPointIndex < pointsCount; nextPointIndex++) {
					Point nextPoint = sourcePoints[nextPointIndex];
					if (SamePixels(sourcePoint.X, nextPoint.X)) {
						if (nextPoint.Y < minY) {
							minY = nextPoint.Y;
							minPointIndex = nextPointIndex;
						}
						else if (nextPoint.Y > maxY) {
							maxY = nextPoint.Y;
							maxPointIndex = nextPointIndex;
						}
						lastPoint = nextPoint;
						lastPointIndex = nextPointIndex;
					}
					else
						break;
				}
				resultPoints.Add(sourcePoint);
				if (lastPointIndex != sourcePointIndex) {
					Point minPoint = new Point(sourcePoint.X, minY);
					Point maxPoint = new Point(sourcePoint.X, maxY);
					bool shouldAddMinPoint = minPointIndex != sourcePointIndex && minPointIndex != sourcePointIndex;
					bool shouldAddMaxPoint = maxPointIndex != lastPointIndex && maxPointIndex != lastPointIndex;
					if (minPointIndex < maxPointIndex) {
						AddPointToListConditional(resultPoints, minPoint, shouldAddMinPoint);
						AddPointToListConditional(resultPoints, maxPoint, shouldAddMaxPoint);
					}
					else {
						AddPointToListConditional(resultPoints, maxPoint, shouldAddMaxPoint);
						AddPointToListConditional(resultPoints, minPoint, shouldAddMinPoint);
					}
					resultPoints.Add(lastPoint);
					sourcePointIndex = lastPointIndex;
				}
			}
			return resultPoints;
		}
		void RoundPoints(List<Point> points) {
			for (int pointIndex = 0; pointIndex < points.Count; pointIndex++) {
				Point sourcePoint = points[pointIndex];
				points[pointIndex] = new Point(sourcePoint.X + 0.5, sourcePoint.Y + 0.5);
			}
		}
		protected abstract void RenderCore(StreamGeometryContext geometryContext, List<Point> renderPoints);
		public void Render(StreamGeometryContext geometryContext) {
			List<Point> renderPoints = ShouldOptimizePoints ? OptimizePoints(points) : points;
			if (shouldRoundPoints)
				RoundPoints(renderPoints);
			if (points.Count > 0)
				RenderCore(geometryContext, renderPoints);
		}
	}
	public class ChartPolyLineSegment : ChartGeometrySegmentBase {
		protected override bool ShouldOptimizePoints { get { return true; } }
		public ChartPolyLineSegment(List<Point> points, bool shouldRoundPoints)
			: base(points, shouldRoundPoints) {
		}
		protected override void RenderCore(StreamGeometryContext geometryContext, List<Point> renderPoints) {
			geometryContext.PolyLineTo(renderPoints, true, false);
		}
	}
	public class ChartPolyBezierSegment : ChartGeometrySegmentBase {
		protected override bool ShouldOptimizePoints { get { return false; } }
		public ChartPolyBezierSegment(List<Point> points, bool shouldRoundPoints)
			: base(points, shouldRoundPoints) {
		}
		protected override void RenderCore(StreamGeometryContext geometryContext, List<Point> renderPoints) {
			geometryContext.PolyBezierTo(renderPoints, true, false);
		}
	}
	public class ChartGeometryFigure {
		readonly List<ChartGeometrySegmentBase> segments = new List<ChartGeometrySegmentBase>();
		bool isClosed;
		bool isFilled;
		public List<ChartGeometrySegmentBase> Segments { get { return segments; } }
		public Point StartPoint { get; set; }
		public bool IsClosed { get { return isClosed; } set { isClosed = value; } }
		public bool IsFilled { get { return isFilled; } set { isFilled = value; } }
		public ChartGeometryFigure()
			: this(false, false) {
		}
		public ChartGeometryFigure(bool isFilled, bool isClosed) {
			this.isFilled = isFilled;
			this.isClosed = isClosed;
		}
		public void Render(StreamGeometryContext geometryContext) {
			if (segments.Count > 0)
				geometryContext.BeginFigure(StartPoint, IsFilled, IsClosed);
			foreach (ChartGeometrySegmentBase segment in segments) {
				segment.Render(geometryContext);
			}
		}
	}
}
