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

using DevExpress.Diagram.Core.Shapes;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Diagram.Core {
	public partial class BasicShapes {
		#region Constants
		internal const double DefaultSnip = 0.2;
		internal const double DefaultRounding = 0.2;
		internal const double DefaultCanRadius = 0.2;
		internal const double DefaultFrameThickness = 10;
		internal const double DefaultLShapeThickness = 0.2;
		internal const double DefaultChevronThickness = 0.5;
		internal const double DefaultCubeDepth = 0.2;
		internal const double DefaultDonutThickness = 0.5;
		internal const double DefaultNoSymbolThickness = 0.3;
		internal const double DefaultStarAngle = Math.PI / 5;
		internal const double DefaultEditorBoundsHeight = 20;
		#endregion
		static DiagramItemStyleId GetStyleId(string id) {
			return DefaultDiagramStyleId.Variant1;
		}
		internal static double GetParameter(IEnumerable<double> parameters, int index) {
			if(parameters != null && parameters.Count() > index)
				return parameters.ElementAt(index);
			throw new IndexOutOfRangeException("Invalid parameter index");
		}
		static double GetNormalizedParameter(IEnumerable<double> p, int index) {
			return Normalize(GetParameter(p, index));
		}
		internal static double Normalize(double value) {
			return Normalize(value, 0, 1);
		}
		internal static double Normalize(double value, double min, double max) {
			return Math.Min(max, Math.Max(min, value));
		}
		internal static ShapeGeometry GetRectanglePoints(double width, double height) {
			return GetRectanglePoints(0, 0, width, height);
		}
		internal static IEnumerable<Point> GetRectangleConnectionPoints(double width, double height) {
			yield return new Point(width / 2, 0);
			yield return new Point(width, height / 2);
			yield return new Point(width / 2, height);
			yield return new Point(0, height / 2);
		}
		internal static IEnumerable<Point> GetCirclePoints(double width, double height, int pointCount, double startPhase) {
			double diameter = Math.Min(width, height);
			double radius = diameter / 2;
			Size scale = new Size(1, 1);
			if(MathHelper.IsGreaterThan(diameter, 0))
				scale = new Size(width / diameter, height / diameter);
			double phase = startPhase;
			for(int i = 0; i < pointCount; i++) {
				yield return GetCartesianPointByPolarPoint(radius, phase).OffsetPoint(new Point(radius, radius)).ScalePoint(scale);
				phase += 2 * Math.PI / pointCount;
			}
		}
		internal static Point GetCartesianPointByPolarPoint(double magnitude, double phase) {
			double x = magnitude * Math.Cos(phase);
			double y = magnitude * Math.Sin(phase);
			return new Point(x, y);
		}
		internal static IEnumerable<Point> GetCircleConnectionPoints(double width, double height) {
			return GetCirclePoints(width, height, 8, -Math.PI / 2);
		}
		internal static ShapeGeometry GetEllipsePoints(double width, double height) {
			return GetEllipsePoints(width, height, GeometryKind.ClosedFilled);
		}
		internal static ShapeGeometry GetEllipsePoints(double width, double height, GeometryKind kind, bool isNewShape = true) {
			SweepDirection direction = SweepDirection.Clockwise;
			Size size = new Size(width / 2, height / 2);
			return new ShapeGeometry(
				StartSegment.Create(new Point(0, height / 2), kind, isNewShape: isNewShape),
				ArcSegment.Create(width, height / 2, size, direction),
				ArcSegment.Create(0, height / 2, size, direction)
			);
		}
		internal static IEnumerable<Point> GetEllipseConnectionPoints(double width, double height) {
			return GetRectangleConnectionPoints(width, height);
		}
		internal static ShapeGeometry GetRightTrianglePoints(double width, double height, IEnumerable<double> p) {
			return GetTrianglePoints(width, height, p);
		}
		internal static Func<ParameterCollection> GetRightTriangleParameters() {
			return GetTriangleParameters(0);
		}
		internal static IEnumerable<Point> GetRightTriangleConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return GetTriangleConnectionPoints(width, height, ps);
		}
		internal static ShapeGeometry GetTrianglePoints(double width, double height, IEnumerable<double> p) {
			double offset = GetParameter(p, 0);
			return new ShapeGeometry(StartSegment.Create(width * offset, 0), LineSegment.Create(width, height), LineSegment.Create(0, height));
		}
		internal static Func<ParameterCollection> GetTriangleParameters() {
			return GetTriangleParameters(0.5);
		}
		internal static Func<ParameterCollection> GetTriangleParameters(double defaultValue) {
			return () => new ParameterCollection(new ParameterDescription((w, h, ps, p) => Normalize(p.X / w), (w, h, ps, v) => new Point(w * Normalize(v), 0), defaultValue));
		}
		internal static IEnumerable<Point> GetTriangleConnectionPoints(double width, double height, IEnumerable<double> ps) {
			double offset = GetParameter(ps, 0);
			yield return new Point(width * offset, 0);
			yield return new Point(width, height);
			yield return new Point(0, height);
		}
		internal static IEnumerable<ShapeSegment> GetCircleShapePoints(double radius, int pointCount, double startPhase, bool isNewShape = true) {
			return GetCircleShapePoints(2 * radius, 2 * radius, pointCount, startPhase, isNewShape);
		}
		internal static IEnumerable<ShapeSegment> GetCircleShapePoints(double width, double height, int pointCount, double startPhase, bool isNewShape = true) {
			foreach(Point point in GetCirclePoints(width, height, pointCount, startPhase)) {
				if(isNewShape)
					yield return StartSegment.Create(point);
				else
					yield return LineSegment.Create(point);
				isNewShape = false;
			}
		}
		internal static ShapeGeometry GetCircleShape(double width, double height, int pointCount, double startPhase, bool isNewShape = true) {
			return new ShapeGeometry(GetCircleShapePoints(width, height, pointCount, startPhase, isNewShape).ToArray());
		}
		#region Regular polygons
		internal static ShapeGeometry GetPentagonPoints(double width, double height) {
			return GetCircleShape(width, height, 5, -Math.PI / 2);
		}
		internal static ShapeGeometry GetHexagonPoints(double width, double height) {
			return GetCircleShape(width, height, 6, 0);
		}
		internal static ShapeGeometry GetHeptagonPoints(double width, double height) {
			return GetCircleShape(width, height, 7, -Math.PI / 2);
		}
		internal static ShapeGeometry GetOctagonPoints(double width, double height) {
			return GetCircleShape(width, height, 8, Math.PI / 8);
		}
		internal static ShapeGeometry GetDecagonPoints(double width, double height) {
			return GetCircleShape(width, height, 10, 0);
		}
		internal static IEnumerable<Point> GetPentagonConnectionPoints(double width, double height) {
			return GetCirclePoints(width, height, 5, -Math.PI / 2);
		}
		internal static IEnumerable<Point> GetHexagonConnectionPoints(double width, double height) {
			return GetCirclePoints(width, height, 6, 0);
		}
		internal static IEnumerable<Point> GetHeptagonConnectionPoints(double width, double height) {
			return GetCirclePoints(width, height, 7, -Math.PI / 2);
		}
		internal static IEnumerable<Point> GetOctagonConnectionPoints(double width, double height) {
			return GetCirclePoints(width, height, 8, Math.PI / 8);
		}
		internal static IEnumerable<Point> GetDecagonConnectionPoints(double width, double height) {
			return GetCirclePoints(width, height, 10, 0);
		}
		#endregion
		static double GetCanRadius(double height, params double[] p) {
			return GetParameter(p, 0) * height / 2;
		}
		internal static ShapeGeometry GetCanPoints(double width, double height, IEnumerable<double> p) {
			double radius = GetCanRadius(height, p.ToArray());
			return new ShapeGeometry(
				StartSegment.Create(new Point(0, radius), GeometryKind.ClosedFilled), LineSegment.Create(0, height - radius), 
				ArcSegment.Create(width / 2, height, SweepDirection.Counterclockwise), ArcSegment.Create(width, height - radius, SweepDirection.Counterclockwise), 
				LineSegment.Create(width, radius), 
				ArcSegment.Create(width / 2, 0, SweepDirection.Counterclockwise), 
				ArcSegment.Create(0, radius, SweepDirection.Counterclockwise),
				StartSegment.Create(new Point(0, radius), GeometryKind.None), 
				ArcSegment.Create(width / 2, 2 * radius, SweepDirection.Counterclockwise), 
				ArcSegment.Create(width, radius, SweepDirection.Counterclockwise)
			);
		}
		internal static IEnumerable<Point> GetCanConnectionPoints(double width, double height, IEnumerable<double> p) {
			double radius = GetCanRadius(height, p.ToArray());
			yield return new Point(width / 2, 0);
			yield return new Point(width / 2, 2 * radius);
			yield return new Point(width, height / 2);
			yield return new Point(width / 2, height);
			yield return new Point(0, height / 2);
		}
		internal static Func<ParameterCollection> GetCanParameters() {
			return () => new ParameterCollection(new ParameterDescription(
				(w, h, ps, p) => Normalize(p.Y / h), 
				(w, h, ps, v) => new Point(w / 2, 2 * GetCanRadius(h, v)), 
				DefaultCanRadius));
		}
		static double GetParallelogramOffset(double width, IEnumerable<double> p) {
			double sideOffsetFactor = GetNormalizedParameter(p, 0);
			return width * sideOffsetFactor;
		}
		internal static ShapeGeometry GetParallelogramPoints(double width, double height, IEnumerable<double> p) {
			double sideOffset = GetParallelogramOffset(width, p);
			return new ShapeGeometry(StartSegment.Create(sideOffset, 0), LineSegment.Create(width, 0), LineSegment.Create(width - sideOffset, height), LineSegment.Create(0, height));
		}
		internal static IEnumerable<Point> GetParallelogramConnectionPoints(double width, double height, IEnumerable<double> p) {
			double sideOffset = GetParallelogramOffset(width, p);
			return new List<Point> { new Point((width + sideOffset) / 2, 0), new Point(width - sideOffset / 2, height / 2), new Point((width - sideOffset) / 2, height), new Point(sideOffset / 2, height / 2) };
		}
		internal static Func<ParameterCollection> GetParallelogramParameters() {
			return GetParallelogramParameters(DefaultSnip);
		}
		internal static Func<ParameterCollection> GetParallelogramParameters(double defaultValue) {
			return () => new ParameterCollection(new ParameterDescription(
				(w, h, ps, p) => Normalize(p.X / w),
				(w, h, ps, v) => new Point(v * w, 0),
				defaultValue));
		}
		static double GetTrapezoidOffset(double width, IEnumerable<double> p) {
			double sideOffsetFactor = GetNormalizedParameter(p, 0);
			return (width / 2) * sideOffsetFactor;
		}
		internal static ShapeGeometry GetTrapezoidPoints(double width, double height, IEnumerable<double> p) {
			double sideOffset = GetTrapezoidOffset(width, p);
			return new ShapeGeometry(StartSegment.Create(sideOffset, 0), LineSegment.Create(width - sideOffset, 0), LineSegment.Create(width, height), LineSegment.Create(0, height));
		}
		internal static IEnumerable<Point> GetTrapezoidConnectionPoints(double width, double height, IEnumerable<double> p) {
			double sideOffset = GetTrapezoidOffset(width, p);
			return new List<Point> { new Point(width / 2, 0), new Point(width - sideOffset / 2, height / 2), new Point(width / 2, height), new Point(sideOffset / 2, height / 2) };
		}
		internal static Func<ParameterCollection> GetTrapezoidParameters() {
			return () => new ParameterCollection(new ParameterDescription(
				(w, h, ps, p) => Normalize(p.X / (w / 2)),
				(w, h, ps, v) => new Point(v * w / 2, 0),
				DefaultSnip * 2));
		}
		internal static ShapeGeometry GetDiamondPoints(double width, double height) {
			return GetDiamondPoints(width, height, GeometryKind.ClosedFilled);
		}
		internal static ShapeGeometry GetDiamondPoints(double width, double height, GeometryKind kind) {
			return new ShapeGeometry(StartSegment.Create(width / 2, 0, kind), LineSegment.Create(width, height / 2), LineSegment.Create(width / 2, height), LineSegment.Create(0, height / 2));
		}
		internal static IEnumerable<Point> GetDiamondConnectionPoints(double width, double height) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
		static double GetCubeDepth(double width, double height, IEnumerable<double> p) {
			double depthFactor = GetParameter(p, 0);
			return Math.Min(width, height * depthFactor);
		}
		internal static ShapeGeometry GetCubePoints(double width, double height, IEnumerable<double> p) {
			double depth = GetCubeDepth(width, height, p);
			return new ShapeGeometry(
				StartSegment.Create(0, depth, GeometryKind.ClosedFilled, true), LineSegment.Create(width - depth, depth),
				LineSegment.Create(width - depth, height), LineSegment.Create(0, height),
				LineSegment.Create(0, depth),
				StartSegment.Create(0, depth, GeometryKind.ClosedFilled, true, 0.4F), LineSegment.Create(depth, 0),
				LineSegment.Create(width, 0), LineSegment.Create(width - depth, depth),
				LineSegment.Create(0, depth),
				StartSegment.Create(width - depth, depth, GeometryKind.ClosedFilled, true,  -0.4F), LineSegment.Create(width, 0),
				LineSegment.Create(width, height - depth), LineSegment.Create(width - depth, height),
				LineSegment.Create(width - depth, depth)
		   );
		}
		internal static IEnumerable<Point> GetCubeConnectionPoints(double width, double height, IEnumerable<double> p) {
			double depth = GetCubeDepth(width, height, p);
			yield return new Point((width - depth) / 2, depth);
			yield return new Point(width - depth, (depth + height) / 2);
			yield return new Point((width - depth) / 2, height);
			yield return new Point(0, (depth + height) / 2);
			yield return new Point((depth + width) / 2, 0);
			yield return new Point(width, (height - depth) / 2);
		}
		internal static Func<ParameterCollection> GetCubeParameters() {
			return () => new ParameterCollection(new ParameterDescription(
				(w, h, ps, p) => Normalize(p.Y / h),
				(w, h, ps, v) => new Point(0, v * h),
				DefaultCubeDepth));
		}
		internal static Rect GetCubeEditorBounds(double width, double height, IEnumerable<double> p) {
			double depth = GetCubeDepth(width, height, p);
			return new Rect(0, depth, width - depth, height - depth);
		}
		internal static ShapeGeometry GetChevronPoints(double width, double height, IEnumerable<double> p) {
			double sideOffsetFactor = GetParameter(p, 0);
			double sideOffset = width * sideOffsetFactor;
			return new ShapeGeometry(StartSegment.Create(0, 0), LineSegment.Create(sideOffset, 0), LineSegment.Create(width, height / 2), LineSegment.Create(sideOffset, height), LineSegment.Create(0, height), LineSegment.Create(width - sideOffset, height / 2));
		}
		internal static IEnumerable<Point> GetChevronConnectionPoints(double width, double height, IEnumerable<double> p) {
			double sideOffsetFactor = GetParameter(p, 0);
			double sideOffset = width * sideOffsetFactor;
			yield return new Point(sideOffset / 2, 0);
			yield return new Point(width, height / 2);
			yield return new Point(sideOffset / 2, height);
			yield return new Point(width - sideOffset, height / 2);
		}
		internal static Func<ParameterCollection> GetChevronParameters() {
			return GetParallelogramParameters(DefaultChevronThickness);
		}
		#region Stars
		internal static ShapeGeometry GetStar4Points(double width, double height, IEnumerable<double> p) {
			return GetStarPoints(4)(width, height, p);
		}
		internal static ShapeGeometry GetStar5Points(double width, double height, IEnumerable<double> p) {
			return GetStarPoints(5)(width, height, p);
		}
		internal static ShapeGeometry GetStar6Points(double width, double height, IEnumerable<double> p) {
			return GetStarPoints(6)(width, height, p);
		}
		internal static ShapeGeometry GetStar7Points(double width, double height, IEnumerable<double> p) {
			return GetStarPoints(7)(width, height, p);
		}
		internal static ShapeGeometry GetStar16Points(double width, double height, IEnumerable<double> p) {
			return GetStarPoints(16)(width, height, p);
		}
		internal static ShapeGeometry GetStar24Points(double width, double height, IEnumerable<double> p) {
			return GetStarPoints(24)(width, height, p);
		}
		internal static ShapeGeometry GetStar32Points(double width, double height, IEnumerable<double> p) {
			return GetStarPoints(32)(width, height, p);
		}
		static ShapeGetter GetStarPoints(int pointCount) {
			return new ShapeGetter((width, height, parameters) => {
				return new ShapeGeometry(GetStarPointsCore(pointCount, width, height, parameters).ToArray());
			});
		}
		static IEnumerable<ShapeSegment> GetStarPointsCore(int pointCount, double width, double height, IEnumerable<double> p) {
			double phase = GetParameter(p, 0);
			double outerRadius = Math.Min(width, height) / 2;
			Size scale = new Size(width / Math.Min(width, height), height / Math.Min(width, height));
			return GetStarPoints(outerRadius, pointCount, phase).Select(x => x.Scale(scale));
		}
		static IEnumerable<ShapeSegment> GetStarPoints(double outerRadius, int pointCount, double angle) {
			double defaultPhase = Math.PI / pointCount;
			double innerRadius = outerRadius * Math.Sin(angle / 2) / Math.Sin(Math.PI - (defaultPhase + angle / 2));
			var outerEnumerator = GetCircleShapePoints(outerRadius, pointCount, -Math.PI / 2).GetEnumerator();
			double offset = outerRadius - innerRadius;
			var innerEnumerator = GetCircleShapePoints(innerRadius, pointCount, -Math.PI / 2 + defaultPhase, false)
				.Select(point => point.Offset(new Point(offset, offset))).GetEnumerator();
			while(outerEnumerator.MoveNext() && innerEnumerator.MoveNext()) {
				yield return outerEnumerator.Current;
				yield return innerEnumerator.Current;
			}
		}
		static Func<ParameterCollection> GetStarParameters(int pointCount, double angle) {
			return () => new ParameterCollection(new ParameterDescription(GetStarParameterValue(pointCount), GetStarParameterPoint(pointCount), angle));
		}
		static ParameterPointGetter GetStarParameterPoint(int pointCount) {
			return (width, height, ps, value) => GetStarPointsCore(pointCount, width, height, new double[] { value }).ElementAt(1).Point;
		}
		static ParameterValueGetter GetStarParameterValue(int pointCount) {
			return (width, height, ps, point) => GetStarAngleByPoint(pointCount, width, height, point);
		}
		static double GetStarAngleByPoint(int pointCount, double width, double height, Point p) {
			double radius = Math.Min(width, height) / 2;
			double y = radius - Normalize(p.Y * Math.Min(width, height) / height, 0, radius);
			double angle = Math.PI / pointCount;
			double x = Math.Tan(angle) * y;
			return 2 * Math.Atan(x / (radius - y));
		}
		internal static Func<ParameterCollection> GetStar4Parameters() {
			return GetStarParameters(4, DefaultStarAngle);
		}
		internal static Func<ParameterCollection> GetStar5Parameters() {
			return GetStarParameters(5, DefaultStarAngle);
		}
		internal static Func<ParameterCollection> GetStar6Parameters() {
			return GetStarParameters(6, DefaultStarAngle);
		}
		internal static Func<ParameterCollection> GetStar7Parameters() {
			return GetStarParameters(7, DefaultStarAngle);
		}
		internal static Func<ParameterCollection> GetStar16Parameters() {
			return GetStarParameters(16, DefaultStarAngle);
		}
		internal static Func<ParameterCollection> GetStar24Parameters() {
			return GetStarParameters(24, DefaultStarAngle);
		}
		internal static Func<ParameterCollection> GetStar32Parameters() {
			return GetStarParameters(32, DefaultStarAngle);
		}
		static ShapeConnectionPointsGetter GetStarConnectionPoints(int pointCount) {
			return new ShapeConnectionPointsGetter((w, h, ps) => GetStarPointsCore(pointCount, w, h, ps).Select (x => x.Point));
		}
		internal static IEnumerable<Point> GetStar4ConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return GetStarConnectionPoints(4)(width, height, ps);
		}
		internal static IEnumerable<Point> GetStar5ConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return GetStarConnectionPoints(5)(width, height, ps);
		}
		internal static IEnumerable<Point> GetStar6ConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return GetStarConnectionPoints(6)(width, height, ps);
		}
		internal static IEnumerable<Point> GetStar7ConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return GetStarConnectionPoints(7)(width, height, ps);
		}
		internal static IEnumerable<Point> GetStar16ConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return GetStarConnectionPoints(16)(width, height, ps);
		}
		internal static IEnumerable<Point> GetStar24ConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return GetStarConnectionPoints(24)(width, height, ps);
		}
		internal static IEnumerable<Point> GetStar32ConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return GetStarConnectionPoints(32)(width, height, ps);
		}
		#endregion
		#region Rectangles
		internal class RectangleParameter<T> {
			public T LeftTop { get; set; }
			public T RightTop { get; set; }
			public T RightBottom { get; set; }
			public T LeftBottom { get; set; }
			public RectangleParameter(T snip)
				: this(snip, snip, snip, snip) { }
			public RectangleParameter(T leftTop, T rightTop, T rightBottom, T leftBottom) {
				this.LeftTop = leftTop;
				this.RightTop = rightTop;
				this.RightBottom = rightBottom;
				this.LeftBottom = leftBottom;
			}
		}
		internal static ShapeGeometry GetRectanglePoints(double x, double y, double width, double height, bool isNewShape = true) {
			return new ShapeGeometry(StartSegment.Create(new Point(x, y), GeometryKind.ClosedFilled, isNewShape: isNewShape), LineSegment.Create(x + width, y), LineSegment.Create(x + width, y + height), LineSegment.Create(x, y + height), LineSegment.Create(x, y));
		}
		internal static ShapeGeometry GetRectanglePoints(double width, double height, RectangleParameter<double> roundings, RectangleParameter<bool> arcs) {
			SweepDirection? direction = SweepDirection.Clockwise;
			RectangleParameter<SweepDirection?> directions = new RectangleParameter<SweepDirection?>(arcs.LeftTop ? direction : null, arcs.RightTop ? direction : null,
				arcs.RightBottom ? direction : null, arcs.LeftBottom ? direction : null);
			return GetRectanglePoints(width, height, roundings, directions);
		}
		static double GetMaxSnip(double w, double h) {
			return Math.Min(w, h) / 2;
		}
		static ShapeGeometry GetRectanglePoints(double width, double height, RectangleParameter<double> roundings, RectangleParameter<SweepDirection?> arcs) {
			double maxSnip = GetMaxSnip(width, height);
			double leftTopSnip = roundings.LeftTop * maxSnip;
			double rightTopSnip = roundings.RightTop * maxSnip;
			double rightBottomSnip = roundings.RightBottom * maxSnip;
			double leftBottomSnip = roundings.LeftBottom * maxSnip;
			return new ShapeGeometry(StartSegment.Create(0, leftTopSnip), GetRectangleSegment(leftTopSnip, 0, arcs.LeftTop), LineSegment.Create(width - rightTopSnip, 0), GetRectangleSegment(width, rightTopSnip, arcs.RightTop), 
				LineSegment.Create(width, height - rightBottomSnip), GetRectangleSegment(width - rightBottomSnip, height, arcs.RightBottom), LineSegment.Create(leftBottomSnip, height), GetRectangleSegment(0, height - leftBottomSnip, arcs.LeftBottom));
		}
		static ShapeSegment GetRectangleSegment(double x, double y, SweepDirection? sweepDirection) {
			if(sweepDirection.HasValue)
				return ArcSegment.Create(x, y, sweepDirection.Value);
			return LineSegment.Create(x, y);
		}
		static Func<ParameterCollection> GetRectangleParameterCollection(double param) {
			var parameter = new ParameterDescription(
				(w, h, ps, p) => Normalize((w - p.X) / GetMaxSnip(w, h)),
				(w, h, ps, v) => new Point(w - v * GetMaxSnip(w, h), 0),
				param);
			return () => new ParameterCollection(parameter);
		}
		static ParameterDescription GetRectangleParamenterByIndex(int index, double defaultValue) {
			bool left = index == 0 || index == 3;
			bool top = index == 0 || index == 1;
			return new ParameterDescription(
				(w, h, ps, p) => Normalize((left ? (p.X) : (w - p.X)) / GetMaxSnip(w, h)),
				(w, h, ps, v) => new Point((left ? (v * GetMaxSnip(w, h)) : (w - v * GetMaxSnip(w, h))), top ? 0 : h),
				defaultValue);
		}
		static Func<ParameterCollection> GetRectangleParameterCollection(params double[] ps) {
			var parameters = Enumerable.Range(0, ps.Length).Select(index => GetRectangleParamenterByIndex(index, ps[index])).ToArray();
			return () => new ParameterCollection(parameters);
		}
		internal static ShapeGeometry GetRoundedRectanglePoints(double width, double height, IEnumerable<double> p) {
			double rounding = GetParameter(p, 0);
			return GetRectanglePoints(width, height, new RectangleParameter<double>(rounding), new RectangleParameter<bool>(true));
		}
		internal static Func<ParameterCollection> GetRoundedRectangleParameters() {
			return () => new ParameterCollection(GetRectangleParamenterByIndex(1, DefaultRounding));
		}
		internal static ShapeGeometry GetSingleSnipCornerRectanglePoints(double width, double height, IEnumerable<double> p) {
			double snip = GetParameter(p, 0);
			return GetRectanglePoints(width, height, new RectangleParameter<double>(0, snip, 0, 0), new RectangleParameter<bool>(false));
		}
		internal static Func<ParameterCollection> GetSingleSnipCornerRectangleParameters() {
			return () => new ParameterCollection(GetRectangleParamenterByIndex(1, DefaultSnip));
		}
		internal static ShapeGeometry GetSnipSameSideCornerRectanglePoints(double width, double height, IEnumerable<double> p) {
			double snip = GetParameter(p, 0);
			return GetRectanglePoints(width, height, new RectangleParameter<double>(snip, snip, 0, 0), new RectangleParameter<bool>(false));
		}
		internal static Func<ParameterCollection> GetSnipSameSideCornerRectangleParameters() {
			return () => new ParameterCollection(GetRectangleParamenterByIndex(1, DefaultSnip));
		}
		internal static ShapeGeometry GetSnipDiagonalCornerRectanglePoints(double width, double height, IEnumerable<double> p) {
			double snip = GetParameter(p, 0);
			return GetRectanglePoints(width, height, new RectangleParameter<double>(0, snip, 0, snip), new RectangleParameter<bool>(false));
		}
		internal static Func<ParameterCollection> GetSnipDiagonalCornerRectangleParameters() {
			return () => new ParameterCollection(GetRectangleParamenterByIndex(1, DefaultSnip));
		}
		static ShapeGeometry GetSingleRoundCornerRectanglePoints(double width, double height, IEnumerable<double> p) {
			double snip = GetParameter(p, 0);
			return GetRectanglePoints(width, height, new RectangleParameter<double>(0, snip, 0, 0), new RectangleParameter<bool>(true));
		}
		internal static Func<ParameterCollection> GetSingleRoundCornerRectangleParameters() {
			return () => new ParameterCollection(GetRectangleParamenterByIndex(1, DefaultRounding));
		}
		static ShapeGeometry GetRoundSameSideCornerRectanglePoints(double width, double height, IEnumerable<double> p) {
			double snip = GetParameter(p, 0);
			return GetRectanglePoints(width, height, new RectangleParameter<double>(snip, snip, 0, 0), new RectangleParameter<bool>(true));
		}
		internal static Func<ParameterCollection> GetRoundSameSideCornerRectangleParameters() {
			return () => new ParameterCollection(GetRectangleParamenterByIndex(1, DefaultRounding));
		}
		static ShapeGeometry GetRoundDiagonalCornerRectanglePoints(double width, double height, IEnumerable<double> p) {
			double snip = GetParameter(p, 0);
			return GetRectanglePoints(width, height, new RectangleParameter<double>(snip, 0, snip, 0), new RectangleParameter<bool>(true));
		}
		internal static Func<ParameterCollection> GetRoundDiagonalCornerRectangleParameters() {
			return () => new ParameterCollection(GetRectangleParamenterByIndex(2, DefaultRounding));
		}
		static ShapeGeometry GetSnipAndRoundSingleCornerRectanglePoints(double width, double height, IEnumerable<double> p) {
			double rounding = GetParameter(p, 0);
			double snip = GetParameter(p, 1);
			return GetRectanglePoints(width, height, new RectangleParameter<double>(rounding, snip, 0, 0), new RectangleParameter<bool>(true, false, false, false));
		}
		internal static Func<ParameterCollection> GetSnipAndRoundSingleCornerRectangleParameters() {
			return GetRectangleParameterCollection(DefaultRounding, DefaultSnip);
		}
		static ShapeGeometry GetSnipCornerRectanglePoints(double width, double height, IEnumerable<double> p) {
			double leftTop = GetParameter(p, 0);
			double rightTop = GetParameter(p, 1);
			double rightBottom = GetParameter(p, 2);
			double leftBottom = GetParameter(p, 3);
			return GetRectanglePoints(width, height, new RectangleParameter<double>(leftTop, rightTop, rightBottom, leftBottom), new RectangleParameter<bool>(false));
		}
		internal static Func<ParameterCollection> GetSnipCornerRectangleParameters() {
			return GetRectangleParameterCollection(DefaultSnip, DefaultSnip, DefaultSnip, DefaultSnip);
		}
		static ShapeGeometry GetRoundCornerRectanglePoints(double width, double height, IEnumerable<double> p) {
			double leftTop = GetParameter(p, 0);
			double rightTop = GetParameter(p, 1);
			double rightBottom = GetParameter(p, 2);
			double leftBottom = GetParameter(p, 3);
			return GetRectanglePoints(width, height,
				new RectangleParameter<double>(leftTop, rightTop, rightBottom, leftBottom),
				new RectangleParameter<bool>(true));
		}
		internal static Func<ParameterCollection> GetRoundCornerRectangleParameters() {
			return GetRectangleParameterCollection(DefaultRounding, DefaultRounding, DefaultRounding, DefaultRounding);
		}
		static ShapeGeometry GetSnipAndRoundCornerRectanglePoints(double width, double height, IEnumerable<double> p) {
			return GetRectanglePoints(width, height, 
				new RectangleParameter<double>(GetParameter(p, 0), GetParameter(p, 1), GetParameter(p, 2), GetParameter(p, 3)), 
				new RectangleParameter<bool>(true, false, true, false));
		}
		internal static Func<ParameterCollection> GetSnipAndRoundCornerRectangleParameters() {
			return GetRectangleParameterCollection(DefaultRounding, DefaultSnip, DefaultRounding, DefaultSnip);
		}
		internal static IEnumerable<Point> GetRoundedRectangleConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
		internal static IEnumerable<Point> GetSingleSnipCornerRectangleConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
		internal static IEnumerable<Point> GetSnipSameSideCornerRectangleConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
		internal static IEnumerable<Point> GetSnipDiagonalCornerRectangleConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
		internal static IEnumerable<Point> GetSingleRoundCornerRectangleConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
		internal static IEnumerable<Point> GetRoundSameSideCornerRectangleConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
		internal static IEnumerable<Point> GetRoundDiagonalCornerRectangleConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
		internal static IEnumerable<Point> GetSnipAndRoundSingleCornerRectangleConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
		internal static IEnumerable<Point> GetSnipCornerRectangleConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
		internal static IEnumerable<Point> GetRoundCornerRectangleConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
		internal static IEnumerable<Point> GetSnipAndRoundCornerRectangleConnectionPoints(double width, double height, IEnumerable<double> ps) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
		#endregion
		static ShapeGeometry GetPlaquePoints(double width, double height, IEnumerable<double> p) {
			double roundingPercent = GetParameter(p, 0);
			return GetRectanglePoints(width, height, new RectangleParameter<double>(roundingPercent), new RectangleParameter<SweepDirection?>(SweepDirection.Counterclockwise));
		}
		static IEnumerable<Point> GetPlaqueConnectionPoints(double width, double height, IEnumerable<double> p) {
			return GetRectangleConnectionPoints(width, height);
		}
		internal static Func<ParameterCollection> GetPlaqueParameters() {
			return GetRectangleParameterCollection(DefaultRounding);
		}
		static ShapeGeometry GetFramePoints(double width, double height, IEnumerable<double> p) {
			double thickness = GetParameter(p, 0);
			var outerPoints = GetRectanglePoints(0, 0, width, height);
			var innerPoints = GetRectanglePoints(thickness, thickness, width - 2 * thickness, height - 2 * thickness, false);
			return outerPoints.Concat(innerPoints);
		}
		static IEnumerable<Point> GetFrameConnectionPoints(double width, double height, IEnumerable<double> p) {
			return GetRectangleConnectionPoints(width, height);
		}
		internal static Func<ParameterCollection> GetFrameParameters() {
			return () => new ParameterCollection(new ParameterDescription(
				(w, h, ps, p) => Normalize(p.X, 0, Math.Min(w, h) / 2),
				(w, h, ps, v) => new Point(v, 0),
				DefaultFrameThickness));
		}
		static ShapeGeometry GetFrameCornerPoints(double width, double height, IEnumerable<double> p) {
			double thickness = GetParameter(p, 0);
			return new ShapeGeometry(StartSegment.Create(0, 0), LineSegment.Create(width, 0), LineSegment.Create(width - thickness, thickness), 
				LineSegment.Create(thickness, thickness), LineSegment.Create(thickness, height - thickness), LineSegment.Create(0, height));
		}
		static IEnumerable<Point> GetFrameCornerConnectionPoints(double width, double height, IEnumerable<double> p) {
			double thickness = GetParameter(p, 0);
			yield return new Point(width / 2, 0);
			yield return new Point(width - thickness / 2, thickness / 2);
			yield return new Point(width / 2, thickness);
			yield return new Point(thickness, height / 2);
			yield return new Point(thickness / 2, height - thickness / 2);
			yield return new Point(0, height / 2);
		}
		internal static Func<ParameterCollection> GetFrameCornerParameters() {
			return () => new ParameterCollection(new ParameterDescription(
				(w, h, ps, p) => Normalize(p.X, 0, Math.Min(w, h) / 2),
				(w, h, ps, p) => new Point(p, 0),
				DefaultFrameThickness));
		}
		static Rect GetFrameCornerEditorBounds(double width, double height, IEnumerable<double> parameters) {
			return new Rect(0, height, width, DefaultEditorBoundsHeight);
		}
		static double GetLShapeHorizontalOffset(double width, IEnumerable<double> p) {
			double horizontalOffset = GetParameter(p, 0);
			return horizontalOffset * width;
		}
		static double GetLShapeVerticalOffset(double height, IEnumerable<double> p) {
			double verticalOffset = GetParameter(p, 1);
			return height - verticalOffset * height;
		}
		static ShapeGeometry GetLShapePoints(double width, double height, IEnumerable<double> p) {
			double width1 = GetLShapeHorizontalOffset(width, p);
			double height1 = GetLShapeVerticalOffset(height, p);
			return new ShapeGeometry(StartSegment.Create(0, 0), LineSegment.Create(width1, 0), LineSegment.Create(width1, height1), 
				LineSegment.Create(width, height1), LineSegment.Create(width, height), LineSegment.Create(0, height));
		}
		static IEnumerable<Point> GetLShapeConnectionPoints(double width, double height, IEnumerable<double> p) {
			double width1 = GetLShapeHorizontalOffset(width, p);
			double height1 = GetLShapeVerticalOffset(height, p);
			return new List<Point> { new Point(width1 / 2, 0), new Point(width1, height1 / 2), new Point((width + width1) / 2, height1), 
				new Point(width, (height + height1) / 2), new Point(width / 2, height), new Point(0, height / 2)};
		}
		internal static Func<ParameterCollection> GetLShapeParameters() {
			return () => new ParameterCollection(
				new ParameterDescription((w, h, ps, p) => Normalize(p.X / w), (w, h, ps, v) => new Point(w * v, 0), DefaultLShapeThickness),
				new ParameterDescription((w, h, ps, p) => 1 - Normalize(p.Y / h), (w, h, ps, v) => new Point(0, h * (1 - v)), DefaultLShapeThickness)
				);
		}
		static Rect GetLShapeEditorBounds(double width, double height, IEnumerable<double> parameters) {
			return new Rect(0, height, width, DefaultEditorBoundsHeight);
		}
		static double GetDiagonalStripeOffset(double x, IEnumerable<double> p) {
			double offsetPercent = GetParameter(p, 0);
			return x * offsetPercent;
		}
		static ShapeGeometry GetDiagonalStripePoints(double width, double height, IEnumerable<double> p) {
			double offsetWidth = GetDiagonalStripeOffset(width, p);
			double offsetHeight = GetDiagonalStripeOffset(height, p);
			return new ShapeGeometry(StartSegment.Create(0, height - offsetHeight), LineSegment.Create(width - offsetWidth, 0), LineSegment.Create(width, 0), 
				LineSegment.Create(0, height), LineSegment.Create(0, height - offsetHeight));
		}
		static IEnumerable<Point> GetDiagonalStripeConnectionPoints(double width, double height, IEnumerable<double> p) {
			double offsetWidth = GetDiagonalStripeOffset(width, p);
			double offsetHeight = GetDiagonalStripeOffset(height, p);
			yield return new Point(width - offsetWidth / 2, 0);
			yield return new Point(width / 2, height / 2);
			yield return new Point(0, height - offsetHeight / 2);
			yield return new Point((width - offsetWidth) / 2, (height - offsetHeight) / 2);
		}
		internal static Func<ParameterCollection> GetDiagonalStripeParameters() {
			return () => new ParameterCollection(new ParameterDescription(
				(w, h, ps, p) => 1 - Normalize(p.Y / h),
				(w, h, ps, v) => new Point(0, h - h * v),
				DefaultLShapeThickness));
		}
		static Rect GetDiagonalStripeEditorBounds(double width, double height, IEnumerable<double> parameters) {
			return new Rect(0, height, width, DefaultEditorBoundsHeight);
		}
		static ShapeGeometry GetDonutPoints(double width, double height, IEnumerable<double> p) {
			double outerDiameter = Math.Min(width, height);
			double innerDiameter = (1 - GetParameter(p, 0)) * outerDiameter;
			var outerShape = GetEllipsePoints(outerDiameter, outerDiameter);
			double offset = (outerDiameter - innerDiameter) / 2;
			var innerShape = GetEllipsePoints(innerDiameter, innerDiameter, GeometryKind.ClosedFilled, false).Offset(new Point(offset, offset));
			return outerShape.Concat(innerShape).Scale(new Size(width / outerDiameter, height / outerDiameter));
		}
		static IEnumerable<Point> GetDonutConnectionPoints(double width, double height, IEnumerable<double> p) {
			return GetCircleConnectionPoints(width, height);
		}
		internal static Func<ParameterCollection> GetDonutParameters() {
			return GetDonutParameters(DefaultDonutThickness);
		}
		internal static Func<ParameterCollection> GetDonutParameters(double defaultValue) {
			return () => new ParameterCollection(new ParameterDescription(
				(w, h, ps, p) => Normalize(p.X / (w / 2)),
				(w, h, ps, v) => new Point(w * v / 2, h / 2),
				defaultValue));
		}
		static Rect GetDonutEditorBounds(double width, double height, IEnumerable<double> parameters) {
			return new Rect(0, height, width, DefaultEditorBoundsHeight);
		}
		static ShapeGeometry GetNoSymbolPoints(double width, double height, IEnumerable<double> p) {
			double outerDiameter = Math.Min(width, height);
			double thickness = GetParameter(p, 0) * outerDiameter;
			double innerDiameter = outerDiameter - thickness;
			double innerRadius = innerDiameter / 2;
			var outerEllipse = GetEllipsePoints(outerDiameter, outerDiameter);
			double angle = Math.Acos(thickness / innerDiameter / 2);
			double angle1 = Math.PI / 4 - angle;
			double angle2 = Math.PI / 4 + angle;
			double angle3 = angle1 + Math.PI;
			double angle4 = angle2 + Math.PI;
			Size size = new Size(innerRadius, innerRadius);
			double offset = outerDiameter / 2;
			var innerEllipse = new ShapeGeometry (
				StartSegment.Create(GetCartesianPointByPolarPoint(innerRadius, angle1), GeometryKind.ClosedFilled, isNewShape: false),
				ArcSegment.Create(GetCartesianPointByPolarPoint(innerRadius, angle2), size,  SweepDirection.Clockwise),
				LineSegment.Create(GetCartesianPointByPolarPoint(innerRadius, angle1)),
				StartSegment.Create(GetCartesianPointByPolarPoint(innerRadius, angle3), GeometryKind.ClosedFilled, isNewShape: false),
				ArcSegment.Create(GetCartesianPointByPolarPoint(innerRadius, angle4), size,  SweepDirection.Clockwise),
				LineSegment.Create(GetCartesianPointByPolarPoint(innerRadius, angle3))
			).Offset(new Point(offset, offset));
			return outerEllipse.Concat(innerEllipse).Scale(new Size(width / outerDiameter, height / outerDiameter));
		}
		static IEnumerable<Point> GetNoSymbolConnectionPoints(double width, double height, IEnumerable<double> p) {
			return GetCircleConnectionPoints(width, height);
		}
		internal static Func<ParameterCollection> GetNoSymbolParameters() {
			return GetDonutParameters(DefaultNoSymbolThickness);
		}
		static Rect GetNoSymbolEditorBounds(double width, double height, IEnumerable<double> parameters) {
			return new Rect(0, height, width, DefaultEditorBoundsHeight);
		}
	}
}
