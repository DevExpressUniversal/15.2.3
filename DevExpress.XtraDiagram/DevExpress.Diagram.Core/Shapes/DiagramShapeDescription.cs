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

using DevExpress.Diagram.Core.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using DevExpress.Internal;
using DevExpress.Diagram.Core.Shapes;
using System.Globalization;
using DevExpress.Diagram.Core.TypeConverters;
using System.IO;
using DevExpress.Diagram.Core.Native;
namespace DevExpress.Diagram.Core {
	public delegate ShapeGeometry ShapeGetter(double width, double height, IEnumerable<double> parameters);
	public delegate IEnumerable<Point> ShapeConnectionPointsGetter(double width, double height, IEnumerable<double> parameters);
	public delegate Rect EditorBoundsGetter(double width, double height, IEnumerable<double> parameters);
	[TypeConverter(typeof(ShapeDescriptionTypeConverter))]
	public class ShapeDescription {
		#region Static
		readonly static DiagramItemStyleId defaultShapeStyleId = DefaultDiagramStyleId.Variant1;
		internal static ShapeDescription Create(string id, DiagramControlStringId stringId, Func<Size> getDefaultSize, ShapeGetter getShape,
			ShapeConnectionPointsGetter getConnectionPoints, Func<ParameterCollection> getParameterCollection, EditorBoundsGetter getEditorBounds = null, DiagramItemStyleId styleId = null, bool isQuick = false, bool useBackgroundAsForeground = false) {
			return new ShapeDescription(id, () => DiagramControlLocalizer.GetString(stringId), getDefaultSize, getShape, getConnectionPoints, getParameterCollection, getEditorBounds, () => styleId ?? defaultShapeStyleId, () => isQuick, () => useBackgroundAsForeground);
		}
		internal static ShapeDescription Create(string id, Func<string> getName, Func<Size> getDefaultSize, Func<double, double, ShapeGeometry> getShape, DiagramItemStyleId styleId, bool isQuick) {
			return new ShapeDescription(id, getName, getDefaultSize, (w, h, p) => getShape(w, h), null, null, null, () => styleId, () => isQuick, () => false);
		}
		internal static ShapeDescription Create(string id, DiagramControlStringId stringId, Func<Size> getDefaultSize, Func<double, double, ShapeGeometry> getShape,
			Func<double, double, IEnumerable<Point>> getConnectionPoints, DiagramItemStyleId styleId = null, bool isQuick = false, bool useBackgroundAsForeground = false) {
			return new ShapeDescription(id, () => DiagramControlLocalizer.GetString(stringId), getDefaultSize, (w, h, p) => getShape(w, h), (w, h, p) => getConnectionPoints(w, h), null, null, () => styleId ?? defaultShapeStyleId, () => isQuick, () => useBackgroundAsForeground);
		}
		public static ShapeDescription CreateTemplateShape(string shapeId, Func<string> getName, Func<ShapeTemplate> getTemplateCore) {
			Func<IEnumerable<double>, double[]> getParams = p => (p ?? Enumerable.Empty<double>()).ToArray();
			ShapeTemplate template = null;
			Func<ShapeTemplate> getTemplate = () => template ?? (template = getTemplateCore());
			return new ShapeDescription(shapeId, getName,
				() => getTemplate().DefaultSize,
				(w, h, p) => getTemplate().GetShape(new Size(w, h), getParams(p)),
				(w, h, p) => getTemplate().GetConnectionPoints(new Size(w, h), getParams(p)),
				() => getTemplate().GetParameterCollection(),
				(w, h, p) => getTemplate().GetEditorBounds(new Size(w, h), getParams(p)),
				() => getTemplate().Style ?? defaultShapeStyleId,
				() => getTemplate().IsQuick,
				() => getTemplate().UseBackgroundAsForeground);
		}
		internal static ShapeDescription CreateSvgShape(string shapeId, Func<string> getName, Stream svgStream, bool isQuick) {
			var parseResult = DiagramSvgParser.Parse(svgStream);
			return Create(shapeId, () => shapeId, () => parseResult.Item2, (w, h) => parseResult.Item1, null, isQuick);
		}
		#endregion
		#region Fields
		readonly string idCore;
		readonly Func<string> getName;
		readonly ShapeGetter getShape;
		readonly ShapeConnectionPointsGetter getShapeConnections;
		readonly Func<ParameterCollection> getParameterCollection;
		readonly Func<Size> getDefaultSize;
		readonly EditorBoundsGetter getEditorBounds;
		readonly Func<DiagramItemStyleId> getStyleId;
		readonly Func<bool> getIsQuick;
		readonly Func<bool> getUseBackgroundAsForeground;
		DiagramItemStyleId styleIdCore;
		bool? isQuickCore;
		bool? useBackgroundAsForeground;
		ParameterCollection parameterCollectionCore;
		Size? sizeCore;
		#endregion
		#region Properties
		public ParameterCollection ParameterCollection {
			get {
				if (parameterCollectionCore == null) {
					if (getParameterCollection != null)
						parameterCollectionCore = getParameterCollection();
					parameterCollectionCore = parameterCollectionCore ?? new ParameterCollection();
				}
				return parameterCollectionCore;
			}
		}
		public string Id { get { return idCore; } }
		public string Name { get { return getName(); } }
		public Size DefaultSize { get { return (sizeCore ?? (sizeCore = getDefaultSize())).Value; } }
		public IEnumerable<double> DefaultParameters { get { return ParameterCollection.Parameters.Select(param => param.DefaultValue); } }
		public DiagramItemStyleId StyleId { get { return styleIdCore ?? (styleIdCore = getStyleId()); } }
		public bool IsQuick { get { return (isQuickCore ?? (isQuickCore = getIsQuick())).Value; } }
		public bool UseBackgroundAsForeground { get { return (useBackgroundAsForeground ?? (useBackgroundAsForeground = getUseBackgroundAsForeground())).Value; } }
		#endregion
		public ShapeDescription(string id, Func<string> getName, Func<Size> getDefaultSize, ShapeGetter getShape,
			ShapeConnectionPointsGetter getConnectionPoints, Func<ParameterCollection> getParameterCollection, EditorBoundsGetter getEditorBounds, Func<DiagramItemStyleId> getStyleId, Func<bool> getIsQuick, Func<bool> getUseBackgroundAsForeground) {
			this.idCore = id;
			this.getName = getName;
			this.getDefaultSize = getDefaultSize;
			this.getShape = getShape;
			this.getShapeConnections = getConnectionPoints;
			this.getParameterCollection = getParameterCollection;
			this.getEditorBounds = getEditorBounds;
			this.getStyleId = getStyleId;
			this.getIsQuick = getIsQuick;
			this.getUseBackgroundAsForeground = getUseBackgroundAsForeground;
		}
		#region Methods
		public override string ToString() {
			return Name;
		}
		public ShapeGeometry GetDefaultShape() {
			return getShape(DefaultSize.Width, DefaultSize.Height, DefaultParameters);
		}
		public ShapeGeometry GetShape(Size size, IEnumerable<double> parameters) {
			return getShape(size.Width, size.Height, parameters);
		}
		public IEnumerable<Point> GetConnectionPoints(Size size, IEnumerable<double> parameters) {
			if(getShapeConnections != null)
				return getShapeConnections(size.Width, size.Height, parameters);
			return Enumerable.Empty<Point>();
		}
		public Rect GetEditorBounds(Size size, IEnumerable<double> parameters) {
			if (getEditorBounds != null)
				return getEditorBounds(size.Width, size.Height, parameters);
			return new Rect(size);
		}
		#endregion
	}
	public delegate Point ParameterPointGetter(double width, double height, double[] parameters, double value);
	public delegate double ParameterValueGetter(double width, double height, double[] parameters, Point localPoint);
	public class ParameterCollection {
		readonly ParameterDescription[] parameters;
		public ParameterDescription[] Parameters { get { return parameters; } }
		public ParameterCollection(params ParameterDescription[] parameters) {
			this.parameters = parameters;
		}
	}
	public class ParameterDescription {
		readonly ParameterValueGetter getValue;
		readonly ParameterPointGetter getPoint;
		readonly double defaultValue;
		public double DefaultValue { get { return defaultValue; } }
		public ParameterDescription(ParameterValueGetter getValue, ParameterPointGetter getPoint, double defaultValue) {
			this.getValue = getValue;
			this.getPoint = getPoint;
			this.defaultValue = defaultValue;
		}
		public Point GetPoint(Size size, double[] parameters, double value) {
			return getPoint(size.Width, size.Height, parameters, value);
		}
		public double GetValue(Size size, double[] parameters, Point localPoint) {
			return getValue(size.Width, size.Height, parameters, localPoint);
		}
	}
	public class ShapeGeometry {
		readonly IEnumerable<ShapeSegment> segments;
		public ShapeGeometry(params ShapeSegment[] segments) {
			this.segments = segments;
		}
		#region Methods
		public IEnumerable<ShapeSegment> Segments { get { return segments; } }
		public override string ToString() {
			return ConvertToString("G");
		}
		internal string ConvertToString(string format) {
			StringBuilder builder = new StringBuilder();
			foreach (ShapeSegment segment in segments) {
				if (builder.Length > 0)
					builder.Append(' ');
				builder.Append(segment.ConvertToString(format));
			}
			return builder.ToString();
		}
		#endregion
	}
	public static class ShapeExtensions {
		public static ShapeSegment Offset(this ShapeSegment shape, Point p) {
			return shape.Offset(p);
		}
		public static Color GetFillColor(this StartSegmentStyle style, Color baseColor) {
			return MathHelper.ChangeColorBrightness(style.FillColor ?? baseColor, style.FillBrightness);
		}
		public static ShapeGeometry Concat(this ShapeGeometry shape1, ShapeGeometry shape2) {
			return new ShapeGeometry(shape1.Segments.Concat(shape2.Segments).ToArray());
		}
		public static ShapeGeometry Rotate(this ShapeGeometry shape, double angle) {
			var newPoints = shape.Segments.Select(point => point.Rotate(angle)).ToArray();
			return new ShapeGeometry(newPoints);
		}
		public static ShapeGeometry Rotate(this ShapeGeometry shape, double angle, Point center) {
			var newPoints = shape.Segments.Select(point => point.Offset(center.InvertPoint()).Rotate(angle).Offset(center)).ToArray();
			return new ShapeGeometry(newPoints);
		}
		public static ShapeGeometry Offset(this ShapeGeometry shape, Point p) {
			var newPoints = shape.Segments.Select(s => s.Offset(p)).ToArray();
			return new ShapeGeometry(newPoints);
		}
		public static ShapeGeometry Scale(this ShapeGeometry shape, Size scale) {
			var newPoints = shape.Segments.Select(point => point.Scale(scale)).ToArray();
			return new ShapeGeometry(newPoints);
		}
		public static bool GetIsFilled(this ShapeGeometry shape) {
			bool hasFilledSegments = false;
			ShapeSegmentProcessContext context = new ShapeSegmentProcessContext(s => hasFilledSegments |= s.Kind.IsFilled(), null, null, null, null);
			shape.Segments.ForEach(s => s.ApplyToContext(context));
			return hasFilledSegments;
		}
	}
	public class ShapeParser {
		readonly ShapeParseStrategyBase strategy;
		Point? CurrentPoint { get; set; }
		public ShapeParser(ShapeParseStrategyBase strategy) {
			this.strategy = strategy;
		}
		public void Parse(ShapeGeometry shape) {
			if (shape == null)
				return;
			ShapeSegmentProcessContext context = new ShapeSegmentProcessContext(ProcessStart, ProcessLine, ProcessArc, ProcessQuadraticBezier, ProcessBezier);
			shape.Segments.ForEach(s => {
				s.ApplyToContext(context);
				CurrentPoint = s.Point;
			});
			if (CurrentPoint.HasValue)
				strategy.EndFigure();
		}
		void ProcessBezier(BezierSegment obj) {
			strategy.BezierTo(obj.Point1, obj.Point2, obj.Point, true);
		}
		void ProcessQuadraticBezier(QuadraticBezierSegment obj) {
			strategy.QuadraticBezierTo(obj.Point1, obj.Point, true);
		}
		void ProcessArc(ArcSegment obj) {
			strategy.ArcTo(obj.Point, obj.GetArcSize(CurrentPoint.Value), 0, false, obj.Direction, true);
		}
		void ProcessLine(LineSegment obj) {
			strategy.LineTo(obj.Point, true);
		}
		void ProcessStart(StartSegment start) {
			strategy.IsSmoothJoin = start.IsSmoothJoin;
			strategy.BeginFigure(start.Point, start.Kind.IsFilled(), start.Kind.IsClosed(), start.IsNewShape, start.Style);
		}
	}
	public sealed class ShapeSegmentProcessContext {
		readonly Action<StartSegment> ProcessStart;
		readonly Action<LineSegment> ProcessLine;
		readonly Action<ArcSegment> ProcessArc;
		readonly Action<QuadraticBezierSegment> ProcessQuadraticBezier;
		readonly Action<BezierSegment> ProcessBezier;
		public ShapeSegmentProcessContext(Action<StartSegment> start, Action<LineSegment> line, Action<ArcSegment> arc, Action<QuadraticBezierSegment> quadraticBezier, Action<BezierSegment> bezier) {
			ProcessStart = start;
			ProcessLine = line;
			ProcessArc = arc;
			ProcessQuadraticBezier = quadraticBezier;
			ProcessBezier = bezier;
		}
		public void ProcessLineSegment(LineSegment segment) {
			ProcessLine.Do(x => x(segment));
		}
		public void ProcessStartSegment(StartSegment segment) {
			ProcessStart.Do(x => x(segment));
		}
		public void ProcessArcSegment(ArcSegment segment) {
			ProcessArc.Do(x => x(segment));
		}
		public void ProcessQuadraticBezierSegment(QuadraticBezierSegment segment) {
			ProcessQuadraticBezier.Do(x => x(segment));
		}
		public void ProcessBezierSegment(BezierSegment segment) {
			ProcessBezier.Do(x => x(segment));
		}
	}
	public abstract class ShapeParseStrategyBase {
		bool isInitialized = false;
		public bool IsSmoothJoin { get; set; }
		public void BeginFigure(Point startPoint, bool isFilled, bool isClosed, bool isNewShape, StartSegmentStyle style) {
			if (isNewShape && isInitialized)
				EndFigure();
			isInitialized = true;
			BeginFigureCore(startPoint, isFilled, isClosed, isNewShape, style);
		}
		protected abstract void BeginFigureCore(Point startPoint, bool isFilled, bool isClosed, bool isNewShape, StartSegmentStyle style);
		public abstract void EndFigure();
		public abstract void LineTo(Point point, bool isStroked);
		public abstract void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked);
		public abstract void BezierTo(Point point1, Point point2, Point point3, bool isStroked);
		public abstract void QuadraticBezierTo(Point point1, Point point2, bool isStroked);
	}
	public static class ShapeIntersectionHelper {
		static readonly ShapeSegmentProcessContext context;
		static Point? CurrentPoint { get; set; }
		static Tuple<Point, Point> Line { get; set; }
		static List<Point> IntersectionPoints { get; set; }
		static Point Position { get; set; }
		static StartSegment Start { get; set; }
		static ShapeIntersectionHelper() {
			context = new ShapeSegmentProcessContext(
				ProcessStart, ProcessLine, ProcessArc,
				ProcessQuadraticBeizer, ProcessBezier);
		}
		public static IEnumerable<Point> GetIntersectionPoints(this ShapeGeometry shape, Point position, Point lineBegin, Point lineEnd) {
			Line = Tuple.Create(lineBegin, lineEnd);
			IntersectionPoints = new List<Point>();
			Position = position;
			var trimmedSegments = TrimSegments(shape.Segments);
			foreach(var segments in trimmedSegments) {
				foreach(var segment in segments) {
					segment.ApplyToContext(context);
					CurrentPoint = GetSegmentPoint(segment);
				}
				var startSegment = (StartSegment)segments.First();
				if(CurrentPoint.HasValue && startSegment.Kind != GeometryKind.None) {
					ProcessLine(CurrentPoint.Value, GetSegmentPoint(startSegment), Line.Item1, Line.Item2);
				}
			}
			Clear();
			return IntersectionPoints;
		}
		static Point GetSegmentPoint(ShapeSegment segment) {
			return segment.Point.OffsetPoint(Position);
		}
		static IEnumerable<IEnumerable<ShapeSegment>> TrimSegments(IEnumerable<ShapeSegment> segments) {
			var result = new List<List<ShapeSegment>>();
			List<ShapeSegment> list = null;
			foreach(var segment in segments) {
				if(segment is StartSegment) {
					list = new List<ShapeSegment>();
					result.Add(list);
				} 
				list.Add(segment);
			}
			return result;
		}
		static void Clear() {
			Line = null;
			CurrentPoint = null;
		}
		static void ProcessBezier(BezierSegment segment) {
#if DEBUGTEST
			throw new NotImplementedException();
#endif
		}
		static void ProcessQuadraticBeizer(QuadraticBezierSegment segment) {
#if DEBUGTEST
			throw new NotImplementedException();
#endif
		}
		static void ProcessArc(ArcSegment arc) {
			Size size = arc.GetArcSize(CurrentPoint.Value.OffsetPoint(Position.InvertPoint()));
			SweepDirection direction = arc.Direction == SweepDirection.Clockwise ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
			var points = MathHelper.GetArcAndLineIntersectionPoints(size.Width, size.Height, CurrentPoint.Value, arc.Point.OffsetPoint(Position), direction, Line.Item1, Line.Item2);
			IntersectionPoints.AddRange(points);
		}
		static void ProcessLine(LineSegment segment) {
			ProcessLine(CurrentPoint.Value, segment.Point.OffsetPoint(Position), Line.Item1, Line.Item2);
		}
		static void ProcessLine(Point segmentStart, Point segmentEnd, Point linePoint1, Point linePoint2) {
			var point = MathHelper.GetSegmentAndLineIntersectionPoint(segmentStart, segmentEnd, linePoint1, linePoint2);
			if(point.HasValue)
				IntersectionPoints.Add(point.Value);
		}
		static void ProcessStart(StartSegment segment) {
			CurrentPoint = GetSegmentPoint(segment);
		}
	}
}
