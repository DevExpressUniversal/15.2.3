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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Internal;
using ResizeMode = DevExpress.Diagram.Core.ResizeMode;
namespace DevExpress.Diagram.Core {
	public static class MathHelper {
		public const double VeryLargeValue = 100000;
		public static readonly Rect InfiniteRect = new Rect(-VeryLargeValue, -VeryLargeValue, 2 * VeryLargeValue, 2 * VeryLargeValue);
		#region DEBUGTEST
#if DEBUGTEST
		public static double EpsilonForTests { get { return Epsilon; } }
#endif
		#endregion
		const double Epsilon = 0.0000000001;
		public static bool IsGreaterThan(double x, double y) {
			return x > y && Math.Abs(x - y) > Epsilon;
		}
		public static bool IsLessThan(double x, double y) {
			return x < y && Math.Abs(x - y) > Epsilon;
		}
		public static bool AreEqual(double x, double y) {
			return Math.Abs(x - y) < Epsilon;
		}
		public static double RemoveBias(double value, double originalValue, double epsilon = Epsilon) {
			if(double.IsNaN(value))
				return value;
			return (Math.Abs(value - originalValue) > epsilon) ? value : originalValue;
		}
		public static Size RemoveBias(this Size value, Size originalValue) {
			return new Size(RemoveBias(value.Width, originalValue.Width), RemoveBias(value.Height, originalValue.Height));
		}
		public static Point RemoveBias(this Point value, Point originalValue) {
			return new Point(RemoveBias(value.X, originalValue.X), RemoveBias(value.Y, originalValue.Y));
		}
		public static Rect RemoveBias(this Rect value, Rect originalValue) {
			return new Rect(value.Location.RemoveBias(originalValue.Location), value.Size.RemoveBias(originalValue.Size));
		}
		public static Rect RectFromPoints(Point p1, Point p2) {
			return new Rect(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
		}
		public static bool IsDragGesture(Point p1, Point p2, double step) {
			return Math.Abs(p1.X - p2.X) > step || Math.Abs(p1.Y - p2.Y) > step;
		}
		public static Size ScaleSize(this Size size, double scale) {
			return new Size(size.Width * scale, size.Height * scale);
		}
		public static Size ScaleSize(this Size size, Point scale) {
			return new Size(size.Width * scale.X, size.Height * scale.Y);
		}
		public static Rect ScaleRect(this Rect rect, double scale) {
			return new Rect(rect.TopLeft.ScalePoint(scale), rect.BottomRight.ScalePoint(scale));
		}
		public static Point ScalePoint(this Point point, double scale) {
			return new Point(point.X * scale, point.Y * scale);
		}
		public static Rect SetSize(this Rect rect, Size size) {
			rect.Size = size;
			return rect;
		}
		public static Rect SetLocation(this Rect rect, Point location) {
			rect.Location = location;
			return rect;
		}
		public static Rect SetLeft(this Rect rect, double value) {
			return new Rect(new Point(value, rect.Top), rect.BottomRight);
		}
		public static Rect SetRight(this Rect rect, double value) {
			return new Rect(rect.TopLeft, new Point(value, rect.Bottom));
		}
		public static Rect SetTop(this Rect rect, double value) {
			return new Rect(new Point(rect.Left, value), rect.BottomRight);
		}
		public static Rect SetBottom(this Rect rect, double value) {
			return new Rect(rect.TopLeft, new Point(rect.Right, value));
		}
		public static Point SetX(this Point point, double value) {
			return new Point(value, point.Y);
		}
		public static Point OffsetX(this Point point, double value) {
			return new Point(point.X + value, point.Y);
		}
		public static Point SetY(this Point point, double value) {
			return new Point(point.X, value);
		}
		public static Point OffsetY(this Point point, double value) {
			return new Point(point.X, point.Y + value);
		}
		public static Point InvertPoint(this Point point) {
			return point.ScalePoint(-1);
		}
		public static Point ScalePoint(this Point point, Size scale) {
			return new Point(point.X * scale.Width, point.Y * scale.Height);
		}
		public static Point RoundPoint(this Point point) {
			return new Point(Math.Round(point.X), Math.Round(point.Y));
		}
		public static Point FloorPoint(this Point point) {
			return new Point(Math.Floor(point.X), Math.Floor(point.Y));
		}
		public static Size RoundSize(this Size size) {
			return new Size(Math.Round(size.Width), Math.Round(size.Height));
		}
		public static Point OffsetPoint(this Point point, Point offset) {
			return new Point(point.X + offset.X, point.Y + offset.Y);
		}
		public static Rect OffsetRect(this Rect rect, Point offset) {
			return new Rect(rect.TopLeft.OffsetPoint(offset), rect.Size);
		}
		public static Rect ChangeRectSize(this Rect rect, Point delta) {
			return new Rect(rect.TopLeft, new Size(rect.Width + delta.X, rect.Height + delta.Y));
		}
		public static Point GetOffset(Point from, Point to) {
			return new Point(to.X - from.X, to.Y - from.Y);
		}
		public static Point GetDifference(Size from, Size to) {
			return new Point(to.Width - from.Width, to.Height - from.Height);
		}
		public static Size GetScale(Size from, Size to) {
			return new Size(GetScale(from.Width, to.Width), GetScale(from.Height, to.Height));
		}
		static double GetScale(double from, double to) {
			return AreEqual(from, to) ? 1 : to / from;
		}
		public static Size CoerceNaNSize(this Size size, Size nanSize) {
			return new Size(
				double.IsNaN(size.Width) ? nanSize.Width : size.Width,
				double.IsNaN(size.Height) ? nanSize.Height : size.Height
			);
		}
		public static Size CoerceMinSize(this Size size, Size minSize) {
			return new Size(Math.Max(size.Width, minSize.Width), Math.Max(size.Height, minSize.Height));
		}
		public static Size CoerceMaxSize(this Size size, Size maxSize) {
			return new Size(Math.Min(size.Width, maxSize.Width), Math.Min(size.Height, maxSize.Height));
		}
		public static Point CoerceMinPoint(this Point point, Point minPoint) {
			return new Point(Math.Max(point.X, minPoint.X), Math.Max(point.Y, minPoint.Y));
		}
		public static Point CoerceMaxPoint(this Point point, Point maxPoint) {
			return new Point(Math.Min(point.X, maxPoint.X), Math.Min(point.Y, maxPoint.Y));
		}
		public static Point GetCenter(this Rect rect) {
			if(MathHelper.IsNotaSize(rect.Size))
				return rect.Location;
			return new Point((rect.Left + rect.Right) / 2, (rect.Top + rect.Bottom) / 2);
		}
		public static Rect Intersection(Rect rect1, Rect rect2) {
			rect1.Intersect(rect2);
			return rect1;
		}
		public static Rect GetContainingRect(this IEnumerable<Rect> rects) {
			return rects.SelectMany(r => r.GetPoints()).GetContainingRect();
		}
		public static Rect GetContainingRect(this IEnumerable<Rect_Angle> rects) {
			return rects.SelectMany(r => r.GetPoints()).GetContainingRect();
		}
		public static Rect GetContainingRect(this IEnumerable<Point> points) {
			return new Rect(
				new Point(points.Min(x => x.X), points.Min(x => x.Y)),
				new Point(points.Max(x => x.X), points.Max(x => x.Y))
			);
		}
		public static Point[] GetPoints(this Rect rect) {
			return new Point[] { rect.TopLeft, rect.TopRight, rect.BottomRight, rect.BottomLeft };
		}
		public static Point[] GetPoints(this Rect_Angle rect) {
			Point center = rect.Rect.GetCenter();
			return rect.Rect.GetPoints().Select(p => p.Rotate(rect.Angle, center)).ToArray();
		}
		public static Rect GetCenteredRect(this Point center, Size size) {
			if(MathHelper.IsNotaSize(size))
				return new Rect(center, size);
			var halfSize = size.ScaleSize(0.5).ToPoint();
			return new Rect(center.OffsetPoint(halfSize.InvertPoint()), center.OffsetPoint(halfSize));
		}
		public static Point GetCenterPosition(Size inner, Size outer) {
			return outer.ScaleSize(0.5).ToPoint().GetCenteredRect(inner).Location;
		}
		public static Point GetCenterRectOffset(this Rect rect, Size size) {
			return GetOffset(rect.Location, GetCenterPosition(rect.Size, size));
		}
		public static Rect InflateRect(this Rect rect, double margin) {
			return rect.InflateRect(new Thickness(margin));
		}
		public static Rect InflateRect(this Rect rect, Thickness margin) {
			var topLeftOffset = new Point(margin.Left, margin.Top).InvertPoint();
			var bottomRightOffset = new Point(margin.Right, margin.Bottom);
			return rect.InflateRect(topLeftOffset, bottomRightOffset);
		}
		public static Rect InflateRect(this Rect rect, Point topLeftOffset, Point bottomRightOffset) {
			return new Rect(
				rect.Left + topLeftOffset.X, 
				rect.Top + topLeftOffset.Y, 
				rect.Width - topLeftOffset.X + bottomRightOffset.X, 
				rect.Height - topLeftOffset.Y + bottomRightOffset.Y);
		}
		public static Thickness Invert(this Thickness margin) {
			return new Thickness(-margin.Left, -margin.Top, -margin.Right, -margin.Bottom);
		}
		public static Size InflateSize(this Size size, Thickness thickness) {
			return new Size(
				size.Width + thickness.Left + thickness.Right,
				size.Height + thickness.Top + thickness.Bottom
			);
		}
		public static Point GetOffset(this Thickness thickness) {
			return new Point(thickness.Left, thickness.Top);
		}
		public static Point ValidatePoint(this Point point, Point min, Point max) {
			return new Point(ValidateValue(point.X, min.X, max.X), ValidateValue(point.Y, min.Y, max.Y));
		}
		public static Size ValidateSize(this Size size, Size min, Size max) {
			return new Size(ValidateValue(size.Width, min.Width, max.Width), ValidateValue(size.Height, min.Height, max.Height));
		}
		public static double ValidateValue(double val, double min, double max) {
			if(min > max)
				return val;
			return Math.Max(Math.Min(val, max), min);
		}
		public static Point TransformPoint(this Point p, Matrix transfrom) {
			return transfrom.Transform(p);
		}
		public static Rect TransformRect(this Rect r, Matrix transfrom) {
			return Rect.Transform(r, transfrom);
		}
		public static Point ToPoint(this Size size) {
			return new Point(size.Width, size.Height);
		}
		public static bool IsNotaRect(this Rect rect) {
			return rect.Location.IsNotaPoint() || rect.Size.IsNotaSize();
		}
		public static bool IsNotaSize(this Size size) {
			return double.IsNaN(size.Width) || double.IsNaN(size.Height);
		}
		public static bool IsNotaPoint(this Point point) {
			return double.IsNaN(point.X) || double.IsNaN(point.Y);
		}
		public static Point Rotate(this Point p, double angle) {
			Matrix m = new Matrix();
			m.Rotate(angle);
			return m.Transform(p);
		}
		public static Point Rotate(this Point p, double angle, Point center) {
			return p.OffsetPoint(center.InvertPoint()).Rotate(-angle).OffsetPoint(center);
		}
		public static double Distance(this Point p, Point point) {
			return (p - point).Length;
		}
		public static Point GetNearestPoint(this Point point, IEnumerable<Point> points) {
			return points.MinBy(p => point.Distance(p));
		}
		public static double GetAngleRad(this Point point, Point from) {
			double deltaY = point.Y - from.Y;
			double deltaX = point.X - from.X;
			return Math.Atan2(deltaY, deltaX);
		}
		public static double GetAngle(this Point point, Point from) {
			return point.GetAngleRad(from) * (180 / Math.PI);
		}
		internal static double GetNormalizedAngle(this Point point, Point from) {
			double angle = point.GetAngle(from);
			if(angle < 0)
				angle += 360;
			return angle;
		}
		public static Point GetIntersectionPoint(this Rect rect, Point from) {
			var point = rect.GetCenter();
			var angle = point.GetAngle(from) / 180 * Math.PI;
			var atangent = Math.Atan(rect.Height / rect.Width);
			if(angle >= -atangent && angle < atangent) {
				return new Point(point.X - rect.Width / 2, point.Y - (rect.Width / 2) * Math.Tan(angle));
			} else if(angle >= atangent && angle < Math.PI - atangent) {
			   return new Point(point.X - rect.Height / (2 * Math.Tan(angle)), point.Y - rect.Height / 2);
			} else if(angle >= Math.PI - atangent && angle < Math.PI + atangent) {
			   return new Point(point.X + rect.Width / 2, rect.Width / 2 * Math.Tan(angle) + point.Y);
			} else
				return new Point(point.X + rect.Height / (2 * Math.Tan(angle)), point.Y + rect.Height / 2);
		}
		public static double GetLineLength(this IEnumerable<Point> points) {
			if (points.Count() < 2)
				return 0;
			return Enumerable.Range(0, points.Count() - 1).Sum(pointIndex => (points.ElementAt(pointIndex + 1).Distance(points.ElementAt(pointIndex))));
		}
		public static double GetLineLength(this ShapeGeometry points) {
			return points.Segments.Select(p => p.Point).GetLineLength();
		}
		public static Point GetPointPosition(this IEnumerable<Point> points, double distanceCoef) {
			if(points.Count() == 1)
				return points.First();
			var length = points.GetLineLength() * distanceCoef;
			List<Point> segments = new List<Point>();
			Point segmentStart = points.First();
			Point segmentEnd = default(Point);
			for (int i = 0; i < points.Count(); i++) {
				segmentEnd = points.ElementAt(i);
				if (segments.Concat(new[] { segmentEnd }).GetLineLength() >= length)
					break;
				segments.Add(segmentEnd);
				segmentStart = segmentEnd;
			}
			if (segmentStart.Equals(segmentEnd))
				return segmentEnd;
			double segmentLength = segmentStart.Distance(segmentEnd);
			double segmentCoef = (length - segments.GetLineLength()) / segmentLength;
			return segmentStart.GetPointPosition(segmentCoef, segmentEnd);
		}
		public static Point GetPointPosition(this Point point, double distanceCoef, Point endLinePoint) {
			var angleRad = endLinePoint.GetAngle(point) / 180 * Math.PI;
			double distance = distanceCoef * point.Distance(endLinePoint);
			double x = point.X + Math.Cos(angleRad) * distance;
			double y = point.Y + Math.Sin(angleRad) * distance;
			return new Point(x, y);
		}
		public static Point MoveCornerPoint(this Point p, Point corner, Point offset) {
			var result = p;
			if(AreEqual(p.X, corner.X))
				result.X += offset.X;
			if(AreEqual(p.Y, corner.Y))
				result.Y += offset.Y;
			return result;
		}
		public static Point? GetIntersectionPoint(Point start1, Point end1, Point start2, Point end2) {
			var point = GetSegmentAndLineIntersectionPoint(start1, end1, start2, end2);
			return point.HasValue && point.Value.IsLinePoint(start2, end2) ? point : null;
		}
		public static Point? GetSegmentAndLineIntersectionPoint(Point segmentStart, Point segmentEnd, Point linePoint1, Point linePoint2) {
			var lineVector = linePoint2 - linePoint1;
			double a = -lineVector.Y;
			double b = lineVector.X;
			double c = -(a * linePoint1.X + b * linePoint1.Y);
			double start = a * segmentStart.X + b * segmentStart.Y + c;
			double end = a * segmentEnd.X + b * segmentEnd.Y + c;
			if(start * end > 0)
				return null;
			double u = start / (start - end);
			if(double.IsNaN(u))
				return segmentStart;
			return segmentStart + u * (segmentEnd - segmentStart);
		}
		public static bool IsLinePoint(this Point point, Point start, Point end) {
			double startDistance = point.Distance(start);
			double endDistance = point.Distance(end);
			double lineLength = start.Distance(end);
			return Math.Abs(startDistance + endDistance - lineLength) < 0.0000000001d;
		}
		public static Point CalcQuadraticBezierPoint(double t, Point point1, Point point2, Point point3) {
			double offset = 1 - t;
			double t2 = t * t;
			double x = offset * offset * point1.X + 2 * t * offset * point2.X + t2 * point3.X;
			double y = offset * offset * point1.Y + 2 * t * offset * point2.Y + t2 * point3.Y;
			return new Point(x, y);
		}
		public static Point CalcBezierPoint(double t, Point point1, Point point2, Point point3, Point point4) {
			double offset = 1 - t;
			double t2 = t * t;
			double x = Math.Pow(offset, 3) * point1.X + 3 * offset * offset * t * point2.X + 3 * offset * t2 * point3.X + t2 * t * point4.X;
			double y = Math.Pow(offset, 3) * point1.Y + 3 * offset * offset * t * point2.Y + 3 * offset * t2 * point3.Y + t2 * t * point4.Y;
			return new Point(x, y);
		}
		public static double GetQuadraticBezierLength(this QuadraticBezierSegment segment, Point from) {
			double a = 4 * (Math.Pow(from.X - 2 * segment.Point1.X + segment.Point.X, 2)
						  + Math.Pow(from.Y - 2 * segment.Point1.Y + segment.Point.Y, 2));
			double b = 8 * ((segment.Point1.X - from.X) * (from.X - 2 * segment.Point1.X + segment.Point.X) 
						  + (segment.Point1.Y - from.Y) * (from.Y - 2 * segment.Point1.Y + segment.Point.Y));
			double c = 4 * (Math.Pow(segment.Point1.X - from.X, 2)
						  + Math.Pow(segment.Point1.Y - from.Y, 2));
			double x = .25 * a + c + .5 * b;
			double y = b + a;
			double a2 = a * a;
			var length = Math.Sqrt(x) + .4340277778e-3 * Math.Sqrt(x) * (-.3076171875 * Math.Pow(y, 4) * a2 / Math.Pow(x, 6)
				- .3906250000e-1 * Math.Pow(a, 4) / Math.Pow(x, 4) + .2734375000 * Math.Pow(y, 2) * a2 * a / Math.Pow(x, 5)
				+ .1127929687 * Math.Pow(y, 6) * a / Math.Pow(x, 7) - .1309204102e-1 * Math.Pow(y, 8) / Math.Pow(x, 8))
				+ .2232142857e-2 * Math.Sqrt(x) * (.6250e-1 * a2 * a / Math.Pow(x, 3) - .234375 * Math.Pow(y, 2) * a2 / Math.Pow(x, 4)
				+ .1367187500 * Math.Pow(y, 4) * a / Math.Pow(x, 5) - .2050781250e-1 * Math.Pow(y, 6) / Math.Pow(x, 6))
				+ .1250e-1 * Math.Sqrt(x) * (-.125 * a2 / Math.Pow(x, 2) + .1875 * Math.Pow(y, 2) * a / Math.Pow(x, 3) - .3906250000e-1 * Math.Pow(y, 4) / Math.Pow(x, 4))
				+ .8333333333e-1 * Math.Sqrt(x) * (.5 * a / x - .125 * Math.Pow(y, 2) / Math.Pow(x, 2));
			Debug.Assert(length > 0d);
			return length;
		}
		public static double GetBezierLength(this BezierSegment segment, Point from) {
			double a = Math.Pow(3 * (3 * (segment.Point1.X - segment.Point2.X) + segment.Point.X - from.X), 2) 
					 + Math.Pow(3 * (3 * (segment.Point1.Y - segment.Point2.Y) + segment.Point.Y - from.Y), 2);
			double b = 36 * (from.X - 2 * segment.Point1.X + segment.Point2.X) * (3 * (segment.Point1.X - segment.Point2.X) - from.X + segment.Point.X)
					 + 36 * (from.Y - 2 * segment.Point1.Y + segment.Point2.Y) * (3 * (segment.Point1.Y - segment.Point2.Y) - from.Y + segment.Point.Y);
			double c = 18 * (segment.Point1.X - from.X) * (3 * (segment.Point1.X - segment.Point2.X) - from.X + segment.Point.X) + Math.Pow(6 * (from.X - 2 * segment.Point1.X + segment.Point2.X), 2)
					 + 18 * (segment.Point1.Y - from.Y) * (3 * (segment.Point1.Y - segment.Point2.Y) - from.Y + segment.Point.Y) + Math.Pow(6 * (from.Y - 2 * segment.Point1.Y + segment.Point2.Y), 2);
			double d = 36 * (segment.Point1.X - from.X) * (from.X - 2 * segment.Point1.X + segment.Point2.X)
					 + 36 * (segment.Point1.Y - from.Y) * (from.Y - 2 * segment.Point1.Y + segment.Point2.Y);
			double e = Math.Pow(3 * (segment.Point1.X - from.X), 2) + Math.Pow(3 * (segment.Point1.Y - from.Y), 2);
			double a2 = a * a, a3 = a2 * a;
			double b2 = b * b, b3 = b2 * b;
			double c2 = c * c, c3 = c2 * c;
			double d2 = d * d, d3 = d2 * d;
			double e2 = e * e, e3 = e2 * e;
			double x = 160 * a3 * a + b * (2169 * b3 + 4423680 * e3) + d3 * (467200 * d + 3850240 * e)
				+ 11714560 * d2 * e2 + 7864320 * e3 * (2 * d + e)
				+ (1240 * (b + 2 * c) + 5216 * d + 11840 * e) * a3
				+ (34896 * d + 76992 * e) * b3 + (245760 * d + 524288 * e) * c3
				+ (3580 * b2 + 13840 * c2 + 54528 * d2 + 224256 * d * e + 212992 * e2) * a2
				+ (198880 * d2 + 829440 * e2) * b2 + (729088 * d2 + 3055616 * d * e + 3100672 * e2) * c2
				+ (4572 * b3 + 884224 * b * d * e + 34560 * c3 + 281600 * d3 + 2506752 * e3) * a
				+ (956416 * d3 + 5957632 * d2 * e + 12124160 * d * e2 + 8192000 * e3) * c;
			double y = x + ((14192 * c + 29472 * d + 65920 * e) * b + (56064 * d + 121600 * e) * c) * a2
				+ ((55840 * d + 27064 * c + 123840 * e) * b2 + (52928 * c2 + (466432 * e + 214976 * d) * c + 212160 * d2 + 866304 * e2) * b
				+ (209920 * d + 451072 * e) * c2 + (1773568 * e2 + 419712 * d2 + 1761280 * e * d) * c + 1766400 * e * d2 + 3629056 * e2 * d) * a
				+ 17008 * b3 * c + (49600 * c2 + (200960 * d + 435840 * e) * c + 834048 * e * d) * b2
				+ (64000 * c3 + (830464 * e + 386560 * d) * c2 + (3224576 * e * d + 767744 * d2 + 3256320 * e2) * c
				+ 6488064 * e2 * d + 508160 * d3 + 3177472 * e * d2) * b + 30720 * c3 * c;
			double length = y / (480 * Math.Pow(a + 8 * (2 * e + d) + 2 * (b + 2 * c), 3.5));
			Debug.Assert(length > 0d);
			return length;
		}
		static double GetBezierCurveLength(Point from, double t, Func<double, Point> getPoint) {
			if(t < 0d || t > 1d)
				throw new ArgumentOutOfRangeException();
			const int calcLengthStepsCount = 10000;
			Point A, B = from;
			double length = 0d;
			var j = 0d;
			var delta = t / calcLengthStepsCount;
			for(var i = 0; i < calcLengthStepsCount; i++) {
				j += delta;
				A = getPoint(j);
				length += A.Distance(B);
				B = A;
			}
			return length;
		}
		public static IEnumerable<Point> GetSplinePoints(IEnumerable<Point> points, double reduceCoef = 0.5) {
			if(points.Count() < 3)
				return points;
			const double center = .5;
			var segments = points.Take(points.Count() - 1).Select((p, i) => {
				   var next = points.ElementAt(i + 1);
				   return new { Start = p, End = next, Center = p.GetPointPosition(center, next), Length = p.Distance(next) };
			   });
			var segmentPairs = segments.Take(segments.Count() - 1).Select((segment, i) => {
				var segment2 = segments.ElementAt(i + 1);
				var point = segment.End.Equals(segment2.Start) ? segment.End : segment2.End;
				return new { Segment1 = segment, Segment2 = segment2, CommonPoint = point };
			});
			var totalPoints = segmentPairs.SelectMany(pair => {
				double controlPointCoef = pair.Segment1.Length / (pair.Segment1.Length + pair.Segment2.Length);
				var controlPoint = pair.Segment1.Center.GetPointPosition(controlPointCoef, pair.Segment2.Center);
				var point1 = pair.Segment1.Center.GetPointPosition(reduceCoef, controlPoint);
				var point2 = pair.Segment2.Center.GetPointPosition(reduceCoef, controlPoint);
				var offsetVector = pair.CommonPoint - controlPoint;
				return new[] { point1, controlPoint, point2 }.Select(p => p + offsetVector);
			});
			var result = new List<Point>();
			result.Add(points.First());
			result.AddRange(totalPoints);
			result.Add(points.Last());
			return result;
		}
		public static double GetLineLength(this IEnumerable<ShapeSegment> segments) {
			var first = segments.FirstOrDefault();
			double length = 0d;
			if(first == null)
				return 0d;
			else if(!(first is StartSegment))
				throw new InvalidOperationException();
			Point currentPoint = default(Point);
			List<double> lengths = new List<double>();
			ShapeSegmentProcessContext context = new ShapeSegmentProcessContext(
				s => { lengths.Add(length); length = 0d; },
				s => length += s.Point.Distance(currentPoint),
				null,
				s => length += new[] { currentPoint, s.Point1, s.Point }.GetLineLength(),
				s => length += new[] { currentPoint, s.Point1, s.Point2, s.Point }.GetLineLength());
			segments.ForEach(s => { s.ApplyToContext(context); currentPoint = s.Point; });
			lengths.Add(length);
			return lengths.Sum();
		}
		public static Point GetPointPosition(this IEnumerable<ShapeSegment> segments, double distanceCoef) {
			double length = segments.GetLineLength() * distanceCoef;
			double currentLength = 0d;
			int i = 1;
			for(; i < segments.Count(); i++) {
				currentLength = segments.Take(i + 1).GetLineLength();
				if(currentLength >= length)
					break;
			}
			ShapeSegment segment = segments.ElementAt(i);
			ShapeSegment previous = segments.ElementAt(i - 1);
			double previousLength = segments.Take(i).GetLineLength();
			double segmentLength = currentLength - previousLength;
			double segmentCoef = (length - previousLength) / segmentLength;
			return segment.GetPointPosition(previous.Point, segmentCoef);
		}
		public static Point GetPointPosition(this ShapeSegment segment, Point from, double segmentCoef) {
			Point result = from;
			var context = new ShapeSegmentProcessContext(
			   s => result = s.Point,
			   s => result = from.GetPointPosition(segmentCoef, s.Point),
			   null,
			   s => result = CalcQuadraticBezierPoint(segmentCoef, from, s.Point1, s.Point),
			   s => result = CalcBezierPoint(segmentCoef, from, s.Point1, s.Point2, s.Point));
			segment.ApplyToContext(context);
			return result;
		}
		public static Color ChangeColorBrightness(Color baseColor, double brightness) {
			brightness = Math.Min(1, Math.Max(-1, brightness));
			Func<byte, byte> transform;
			if(brightness > 0)
				transform = x => (byte)(x + ((255 - x) * brightness));
			else
				transform = x => (byte)(x * (1 + brightness));
			return Color.FromArgb(baseColor.A, transform(baseColor.R), transform(baseColor.G), transform(baseColor.B));
		}
		public static bool IsCorner(Point point1, Point point2, Point point3) {
			return (point1.X != point2.X || point1.X != point3.X) && (point1.Y != point2.Y || point1.Y != point3.Y);
		}
		public static IEnumerable<Point> GetIntersectionPoints(this Rect rect, Point start, Point end) {
			var intersectionPoint = GetIntersectionPoint(start, end, rect.TopLeft, rect.TopRight);
			if (intersectionPoint.HasValue)
				yield return intersectionPoint.Value;
			intersectionPoint = GetIntersectionPoint(start, end, rect.TopRight, rect.BottomRight);
			if(intersectionPoint.HasValue)
				yield return intersectionPoint.Value;
			intersectionPoint = GetIntersectionPoint(start, end, rect.BottomRight, rect.BottomLeft);
			if(intersectionPoint.HasValue)
				yield return intersectionPoint.Value;
			intersectionPoint = GetIntersectionPoint(start, end, rect.TopLeft, rect.BottomLeft);
			if(intersectionPoint.HasValue)
				yield return intersectionPoint.Value;
		}
		public static Size GetArcSize(this ArcSegment arc, Point startPoint) {
			return arc.Size ?? new Size(Math.Abs(startPoint.X - arc.Point.X), Math.Abs(startPoint.Y - arc.Point.Y));
		}
		public static Point[] GetEllipseAndLineIntersectionPoints(double a, double b, Point p1, Point p2) {
			var vector = p2 - p1;
			double a1 = vector.Y;
			double a2 = -vector.X;
			double a3 = p2.Y * vector.X - p2.X * vector.Y;
			if(AreEqual(a2, 0)) {
				double x = -a3 / a1;
				double b1 = a * a;
				double b3 = b * b * (x * x - a * a);
				var yArray = SolveQuadraticEquation(b1, 0, b3);
				return yArray.Select(y => new Point(x, y)).ToArray();
			}
			else {
				double b1 = -a1 / a2;
				double b2 = -a3 / a2;
				double c1 = b * b + a * a * b1 * b1;
				double c2 = 2 * a * a * b1 * b2;
				double c3 = a * a * b2 * b2 - a * a * b * b;
				var xArray = SolveQuadraticEquation(c1, c2, c3);
				return xArray.Select(x => new Point(x, b1 * x + b2)).ToArray();
			}
		}
		public static double[] SolveQuadraticEquation(double a, double b, double c) {
			if(Math.Abs(a) > 1) {
				b /= a;
				c /= a;
				a = 1;
			}
			double d = b * b - 4 * a * c;
			d = RemoveBias(d, 0, Epsilon * 1e6);
			if(IsLessThan(d, 0))
				return new double[0];
			if(AreEqual(d, 0))
				return new double[] { -b / (2 * a) };
			double x1 = (-b + Math.Sqrt(d)) / (2 * a);
			double x2 = (-b - Math.Sqrt(d)) / (2 * a);
			return new double[] { x1, x2 };
		}
		public static Point? GetEllipseCenter(double a, double b, Point p1, Point p2, SweepDirection direction) {
			var centers = GetEllipseCenters(a, b, p1, p2);
			if(centers.Length < 1)
				return null;
			if(centers.Length == 1)
				return centers[0];
			if(direction == SweepDirection.Clockwise ^ (p1.GetNormalizedAngle(centers[0]) < p2.GetNormalizedAngle(centers[0])))
				return centers[0];
			return centers[1];
		}
		internal static Point[] GetEllipseCenters(double a, double b, Point p1, Point p2) {
			double epsilon = 100 * Epsilon;
			double x1 = p1.X, y1 = p1.Y, x2 = RemoveBias(p2.X, p1.X, epsilon), y2 = RemoveBias(p2.Y, p1.Y, epsilon);
			double a1 = 2 * b * b * (x2 - x1);
			double a2 = 2 * a * a * (y2 - y1);
			double a3 = b * b * (x1 * x1 - x2 * x2) + a * a * (y1 * y1 - y2 * y2);
			double c = b * b * x1 * x1 + a * a * y1 * y1 - a * a * b * b;
			if(MathHelper.AreEqual(a2, 0)) {
				double x = -a3 / a1;
				double b1 = a * a;
				double b2 = -2 * y1 * a * a;
				double b3 = c + b * b * x * x - 2 * x * x1 * b * b;
				var yArray = SolveQuadraticEquation(b1, b2, b3);
				return yArray.Select(y => new Point(x, y)).ToArray();
			}
			else {
				double b1 = -a1 / a2;
				double b2 = -a3 / a2;
				double c1 = a * a * b1 * b1;
				double c2 = 2 * a * a * b1 * (b2 - y1);
				double c3 = a * a * b2 * b2 - 2 * y1 * a * a * b2;
				double d1 = b * b;
				double d2 = -2 * x1 * b * b;
				double d3 = c;
				double e1 = c1 + d1;
				double e2 = c2 + d2;
				double e3 = c3 + d3;
				var xArray = SolveQuadraticEquation(e1, e2, e3);
				return xArray.Select(x => new Point(x, b1 * x + b2)).ToArray();
			}
		}
		public static Point[] GetArcAndLineIntersectionPoints(double a, double b, Point arcPoint1, Point arcPoint2, SweepDirection direction, Point linePoint1, Point linePoint2) {
			var center = MathHelper.GetEllipseCenter(a, b, arcPoint1, arcPoint2, direction);
			if(!center.HasValue)
				return new Point[0];
			var centeredEllipseIntersectionPoints = MathHelper.GetEllipseAndLineIntersectionPoints(a, b, linePoint1.OffsetPoint(center.Value.InvertPoint()), linePoint2.OffsetPoint(center.Value.InvertPoint()));
			double minAngle = direction == SweepDirection.Clockwise ? arcPoint2.GetNormalizedAngle(center.Value) : arcPoint1.GetNormalizedAngle(center.Value);
			double maxAngle = direction == SweepDirection.Clockwise ? arcPoint1.GetNormalizedAngle(center.Value) : arcPoint2.GetNormalizedAngle(center.Value);
			Func<double, bool> filterCriteria = angle => angle >= minAngle && angle <= maxAngle;
			if(maxAngle < minAngle)
				filterCriteria = angle => angle >= minAngle || angle <= maxAngle;
			var ellipseIntersectionPoints = centeredEllipseIntersectionPoints.Select(point => point.OffsetPoint(center.Value));
			var arcIntersectionPoints = ellipseIntersectionPoints.Where(point => filterCriteria(point.GetNormalizedAngle(center.Value))).ToArray();
			return arcIntersectionPoints;
		}
		#region Rotation
		public static double NormalizeRotationAngle(double angle) {
			return angle % 360;
		}
		public static double GetRotationAngle(Point center, Point point) {
			double deltaX = center.Y - point.Y;
			double deltaY = center.X - point.X;
			return Math.Atan2(deltaY, deltaX) * (180 / Math.PI);
		}
		public static double SnapAngle(double angle, double interval) {
			double intervalIndex = Math.Round(angle / interval);
			return interval * intervalIndex;
		}
		public static ResizeMode Rotate(this ResizeMode mode, double angle) {
			var resizeModes = new List<ResizeMode> 
			{ ResizeMode.Left, ResizeMode.TopLeft, ResizeMode.Top, ResizeMode.TopRight, ResizeMode.Right, ResizeMode.BottomRight, ResizeMode.Bottom, ResizeMode.BottomLeft };
			const double interval = 45;
			double offset = Math.Floor(angle / interval + 0.5);
			int newIndex = (resizeModes.IndexOf(mode) - Convert.ToInt32(offset)) % resizeModes.Count;
			if(newIndex < 0)
				newIndex += resizeModes.Count;
			return resizeModes[newIndex];
		}
		public static Rect Rotate(this Rect rect, double angle, Point? rotationCenter = null) {
			Point center = rotationCenter ?? rect.GetCenter();
			Matrix matrix = new Matrix();
			matrix.RotateAt(-angle, center.X, center.Y);
			return rect.TransformRect(matrix);
		}
		public static Point GetRotationCenter(IEnumerable<Rect_Angle> rects, double rotationAngle) {
			var points = rects.SelectMany(rect => rect.GetPoints()).Select(point => point.Rotate(rotationAngle));
			return MathHelper.GetContainingRect(points).GetCenter().Rotate(-rotationAngle);
		}
		public static Rect_Angle Rotate(this Rect_Angle rect, double angle, Point? rotationCenter = null) {
			Point actualRotationCenter = rotationCenter ?? rect.Rect.GetCenter();
			Point oldCenter = rect.Rect.GetCenter();
			Point newCenter = oldCenter.Rotate(angle, actualRotationCenter);
			Rect newRect = rect.Rect.OffsetRect(MathHelper.GetOffset(oldCenter, newCenter));
			return new Rect_Angle(newRect, rect.Angle + angle);
		}
		#endregion
		public static Rect ProportionalRect(this Rect origin, Rect current) {
			if(origin.Contains(current))
				return origin;
			int leftCoef = 0, rightCoef = 0, topCoef = 0, bottomCoef = 0;
			if(origin.Left > current.Left)
				leftCoef = (int)((origin.Left - current.Left) / origin.Width) + 1;
			if(current.Right > origin.Right)
				rightCoef = (int)((current.Right - origin.Right) / origin.Width) + 1;
			if(current.Top < origin.Top)
				topCoef = (int)((origin.Top - current.Top) / origin.Height) + 1;
			if(current.Bottom > origin.Bottom)
				bottomCoef = (int)((current.Bottom - origin.Bottom) / origin.Height) + 1;
			return origin.InflateRect(new Thickness(leftCoef * origin.Width, topCoef * origin.Height, rightCoef * origin.Width, bottomCoef * origin.Height));
		}
	}
	public class RectInfo {
		public RectInfo(Rect_Angle rect, Size minSize) {
			Rect = rect;
			MinSize = minSize;
		}
		public Rect_Angle Rect { get; private set; }
		public Size MinSize { get; private set; }
	}
	public class SizeInfo<T> : RectInfo {
		public readonly T Item;
		public SizeInfo(T item, Rect_Angle bounds, Size minSize)
			: base(bounds, minSize) {
			this.Item = item;
		}
	}
	public class ParentChildBounds {
		public readonly Rect ParentBounds;
		public readonly Rect[] ChildBounds;
		public ParentChildBounds(Rect parentBounds, Rect[] childBounds) {
			this.ParentBounds = parentBounds;
			this.ChildBounds = childBounds;
		}
	}
	public static class ResizeHelper {
		public static List<Rect> ResizeRects(IEnumerable<RectInfo> rectInfo, ResizeMode resizeMode, Point startPosition, Point endPosition) {
			return ResizeRects(rectInfo, resizeMode, MathHelper.GetOffset(startPosition, endPosition), 0);
		}
		public static List<Rect> ResizeRects(IEnumerable<RectInfo> rectInfo, ResizeMode resizeMode, Point offset, double rotationAngle) {
			Point rotatedOffset = offset.Rotate(rotationAngle);
			Point rotationCenter = MathHelper.GetRotationCenter(rectInfo.Select(item => item.Rect), rotationAngle);
			var rotatedItems = rectInfo.Select(x => x.Rect.Rotate(-rotationAngle, rotationCenter));
			Rect containingRect = rotatedItems.Select(x => x.RotatedRect).GetContainingRect();
			var scale = GetCorrectedScale(rotatedOffset, containingRect, resizeMode);
			var finalScale = rectInfo.Select(x => MathHelper.GetScale(x.Rect.Rect.Size, x.MinSize)).Aggregate(scale, (acc, x) => acc.CoerceMinSize(x));
			return ResizeRects(rotatedItems, finalScale, GetImmutableCorner(containingRect, resizeMode), rotationAngle, rotationCenter);
		}
		static List<Rect> ResizeRects(IEnumerable<Rect_Angle> rotatedRects, Size scale, Point relativeTo, double rotationAngle, Point rotationCenter) {
			return rotatedRects.Select(rect => {
				Size sizeScale = RotateScale(scale, rect.Angle);
				Size newSize = rect.Rect.Size.ScaleSize(new Point(sizeScale.Width, sizeScale.Height));
				Point newRotatedCenter = GetStretchedPoint(scale, relativeTo, rect.Rect.GetCenter());
				Point oldRotatedCenter = rect.Rect.GetCenter();
				Point rotatedOffset = MathHelper.GetOffset(oldRotatedCenter, newRotatedCenter);
				Point centerOffset = rotatedOffset.Rotate(-rotationAngle);
				Point newCenter = oldRotatedCenter.Rotate(rotationAngle, rotationCenter).OffsetPoint(centerOffset);
				return MathHelper.GetCenteredRect(newCenter, newSize);
			}).ToList();
		}
		static Size RotateScale(Size scale, double angle) {
			if(Math.Abs(angle) > 45 && Math.Abs(angle) < 135)
				return new Size(scale.Height, scale.Width);
			return scale;
		}
		static Size CorrectScale(Size scale, ResizeMode resizeMode) {
			var res = new Size(1, 1);
			if(resizeMode.IsVertical()) {
				res.Height = scale.Height;
			}
			if(resizeMode.IsHorizontal()) {
				res.Width = scale.Width;
			}
			return res;
		}
		static Point GetImmutableCorner(Rect containingRect, ResizeMode resizeMode) {
			double x = 0, y = 0;
			if(resizeMode.IsTop()) {
				y = containingRect.Bottom;
			}
			if(resizeMode.IsBottom()) {
				y = containingRect.Top;
			}
			if(resizeMode.IsLeft()) {
				x = containingRect.Right;
			}
			if(resizeMode.IsRight()) {
				x = containingRect.Left;
			}
			return new Point(x, y);
		}
		public static Point SnapPointToRect(Point point, Rect rect, ResizeMode resizeMode) {
			if(resizeMode.IsTop()) {
				point.Y = rect.Top;
			}
			if(resizeMode.IsBottom()) {
				point.Y = rect.Bottom;
			}
			if(resizeMode.IsLeft()) {
				point.X = rect.Left;
			}
			if(resizeMode.IsRight()) {
				point.X = rect.Right;
			}
			return point;
		}
		static Point GetStretchedPoint(Size scale, Point relativeTo, Point point) {
			return point.OffsetPoint(relativeTo.InvertPoint()).ScalePoint(scale).OffsetPoint(relativeTo);
		}
		static Size GetCorrectedScale(Point offset, Rect containingRect, ResizeMode resizeMode) {
			var newSize = ChangeSize(containingRect.Size, offset, resizeMode);
			var scale = MathHelper.GetScale(containingRect.Size, newSize);
			return CorrectScale(scale, resizeMode);
		}
		static Size ChangeSize(Size size, Point delta, ResizeMode resizeMode) {
			double newWidth = 0, newHeight = 0;
			if(resizeMode.IsTop()) {
				newHeight = size.Height - delta.Y;
			}
			if(resizeMode.IsBottom()) {
				newHeight = size.Height + delta.Y;
			}
			if(resizeMode.IsLeft()) {
				newWidth = size.Width - delta.X;
			}
			if(resizeMode.IsRight()) {
				newWidth = size.Width + delta.X;
			}
			return new Size(Math.Max(0, newWidth), Math.Max(0, newHeight));
		}
	}
	public static class AnchorsHelper {
		public static Size GetMinResizingSize<T>(
			T item, 
			Func<T, IEnumerable<T>> getChildren, 
			Func<T, Rect> getRect, 
			Func<T, Size> getMinSize, 
			Func<T, Sides> getAnchors, 
			Func<T, Direction, bool> shouldProtectDirection, 
			Func<T, IEnumerable<Direction>, Size> getChildMinResizingSize,
			IEnumerable<Direction> directions) {
			var itemRect = getRect(item);
			Func<T, Sides, Orientation, bool, double> getMinSizeCore = (child, anchors, orientation, handleNearSide) => {
				if(anchors.IsBoth(orientation))
					return orientation.GetSize(itemRect) - orientation.GetSize(getRect(child)) 
					+ orientation.GetSize(getChildMinResizingSize(child, directions));
				if((anchors.IsNear(orientation) && handleNearSide) || (!anchors.IsFar(orientation) && !handleNearSide))
					return orientation.GetSide(getRect(child), Side.Far);
				return orientation.GetSize(itemRect) - orientation.GetSide(getRect(child), Side.Near);
			};
			var result = getMinSize(item);
			foreach(var direction in directions) {
				var orientation = direction.GetOrientation();
				if(shouldProtectDirection(item, direction)) {
					var coercedSize = getChildren(item).Aggregate(orientation.GetSize(result), (minSize, child) => {
						var anchors = getAnchors(child);
						return Math.Max(minSize, getMinSizeCore(child, anchors, orientation, direction.IsNear()));
					});
					result = orientation.SetSize(result, coercedSize);
				}
			}
			return result;
		}
		public static Rect AnchorBounds(this Sides anchors, Rect bounds, Point topLeftOffset, Point bottomRightOffset) {
			var actualTopLeftOffset = topLeftOffset.InvertPoint();
			var actualBottomRightOffset = default(Point);
			foreach(Orientation orientation in Enum.GetValues(typeof(Orientation))) {
				var farSideActualOffset = orientation.GetPoint(bottomRightOffset) + orientation.GetPoint(topLeftOffset.InvertPoint());
				if(anchors.IsNear(orientation)) {
					actualTopLeftOffset = orientation.SetPoint(actualTopLeftOffset, 0);
				} else if(anchors.IsFar(orientation)) {
					actualTopLeftOffset = orientation.SetPoint(actualTopLeftOffset, farSideActualOffset);
				} else if(anchors.IsBoth(orientation)) {
					actualTopLeftOffset = orientation.SetPoint(actualTopLeftOffset, 0);
					actualBottomRightOffset = orientation.SetPoint(actualBottomRightOffset, farSideActualOffset);
				}
			}
			bounds = bounds.OffsetRect(actualTopLeftOffset);
			bounds.Width = Math.Max(0, bounds.Width + actualBottomRightOffset.X);
			bounds.Height = Math.Max(0, bounds.Height + actualBottomRightOffset.Y);
			return bounds;
		}
	}
}
