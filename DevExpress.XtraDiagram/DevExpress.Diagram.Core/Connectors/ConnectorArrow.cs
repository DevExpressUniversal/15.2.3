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
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DevExpress.Internal;
namespace DevExpress.Diagram.Core.Native {
	public static class ArrowDescriptionExtensions {
		public static Point GetConnectionPoint(this ArrowDescription description, Size size, Point position, double angle) {
			Point connectionPoint = description.GetConnectionPoint(size.Width, size.Height);
			var offset = GetArrowOffset(size);
			return connectionPoint.OffsetPoint(offset).Rotate(angle).OffsetPoint(position);
		}
		public static ShapeGeometry GetShapePoints(this ArrowDescription description, Size size, Point position, double angle) {
			var shape = description.GetShapePoints(size.Width, size.Height);
			var offset = GetArrowOffset(size);
			return shape.Offset(offset).Rotate(angle).Offset(position);
		}
		static Point GetArrowOffset(Size size) {
			return size.ToPoint().ScalePoint(-1).OffsetY(size.Height / 2);
		}
#if DEBUGTEST
		public static Point GetArrowOffsetForTests(Size size) {
			return GetArrowOffset(size);
		}
#endif
	}
	public static class ShapeExtensions {
		public static Rect GetBounds(this ShapeGeometry shape) {
			var points = shape.Segments.Select(p => p.Point);
			Point p1 = new Point(points.Min(p => p.X), points.Min(p => p.Y));
			Point p2 = new Point(points.Max(p => p.X), points.Max(p => p.Y));
			return new Rect(p1, p2);
		}
	}
	public static class DiagramConnectorExtensions {
		public static ShapeGeometry GetConnectorShapeWithBridges(this IEnumerable<ShapeSegment> connectorShape, IEnumerable<Point> intersectPoints) {
			return connectorShape.GetConnectorShapeWithBridges(intersectPoints, DiagramConnectorController.BridgeSize);
		}
		public static ShapeGeometry GetConnectorShapeWithBridges(this IEnumerable<ShapeSegment> connectorShape, IEnumerable<Point> intersectPoints, Size originBridgeSize) {
			var connectorLine = Enumerable.Range(0, connectorShape.Count() - 1).SelectMany((i) => {
				var startSegment = connectorShape.ElementAt(i);
				var endSegment = connectorShape.ElementAt(i + 1);
				var segmentPoints = intersectPoints.Where(ip => IsSegmentPoint(ip, startSegment.Point, endSegment.Point, originBridgeSize))
												   .OrderBy(p => p.Distance(startSegment.Point));
				double angle = endSegment.Point.GetAngle(startSegment.Point);
				return startSegment.Yield().Concat(GetSegmentBridges(segmentPoints, originBridgeSize, angle));
			}).Concat(connectorShape.Last().Yield());
			return new ShapeGeometry(connectorLine.ToArray());
		}
		static IEnumerable<ShapeSegment> GetSegmentBridges(IOrderedEnumerable<Point> intersectPoints, Size originBridgeSize, double angle) {
			var bridges = GroupByDistance(intersectPoints, originBridgeSize.Width)
					.SelectMany(points => {
						double newWidth = originBridgeSize.Width + points.GetLineLength();
						var size = new Size(newWidth, originBridgeSize.Height * newWidth / originBridgeSize.Width);
						return ConnectionBridgeFactory.CreateBowBridge(size, points.GetPointPosition(0.5), angle).Segments;
					});
			return bridges;
		}
		static IEnumerable<IEnumerable<Point>> GroupByDistance(IOrderedEnumerable<Point> orderedPoints, double maxDistance) {
			int count = orderedPoints.Count();
			if(count == 0)
				return Enumerable.Empty<IEnumerable<Point>>();
			List<IEnumerable<Point>> result = new List<IEnumerable<Point>>();
			List<Point> current = new List<Point>();
			result.Add(current);
			for(int i = 0; i < count; i++) {
				Point point = orderedPoints.ElementAt(i);
				current.Add(point);
				if(i + 1 >= count)
					break;
				Point nextPoint = orderedPoints.ElementAt(i + 1);
				if(point.Distance(nextPoint) > maxDistance) {
					current = new List<Point>();
					result.Add(current);
				}
			}
			return result;
		}
		static bool IsSegmentPoint(Point point, Point start, Point end, Size bridgeSize) {
			double startDistance = point.Distance(start);
			double endDistance = point.Distance(end);
			double minDistance = bridgeSize.Width / 2;
			return point.IsLinePoint(start, end) && startDistance > minDistance && endDistance > minDistance;
		}
		public static IEnumerable<ShapeSegment> GetLineSegments(ConnectorType type, Point beginPoint, IEnumerable<Point> points, Point endPoint) {
			if(type == ConnectorType.Curved)
				return GetCurvedLineSegments(beginPoint, points, endPoint);
			return GetStraigtLineSegments(beginPoint, points, endPoint);
		}
		static IEnumerable<ShapeSegment> GetStraigtLineSegments(Point beginPoint, IEnumerable<Point> points, Point endPoint) {
			var start = StartSegment.Create(beginPoint, GeometryKind.None, true);
			var segmentPoints = points.Select(p => new LineSegment(p));
			var linePoints = start.Yield().Concat(segmentPoints.Cast<ShapeSegment>());
			linePoints = linePoints.Concat(new LineSegment(endPoint).Yield());
			return linePoints.ToArray();
		}
		static IEnumerable<ShapeSegment> GetCurvedLineSegments(Point beginPoint, IEnumerable<Point> points, Point endPoint) {
			int count = points.Count();
			if(count == 0)
				return GetStraigtLineSegments(beginPoint, points, endPoint);
			var start = StartSegment.Create(beginPoint, GeometryKind.None, true);
			if(count == 1)
				return new[] { (ShapeSegment)start, new QuadraticBezierSegment(points.Single(), endPoint) };
			IEnumerable<Point> linePoints;
			const double reduceCoef = 0.2;
			if(count == 2)
				linePoints = new[] { points.First(), points.Last(), endPoint };
			else
				linePoints = MathHelper.GetSplinePoints(points, reduceCoef).Concat(endPoint.Yield());
			var segments = new List<ShapeSegment>();
			segments.Add(start);
			for(int i = 0; i < linePoints.Count() - 2; i += 3)
				segments.Add(new BezierSegment(linePoints.ElementAt(i), linePoints.ElementAt(i + 1), linePoints.ElementAt(i + 2)));
			return segments;
		}
	}
}
