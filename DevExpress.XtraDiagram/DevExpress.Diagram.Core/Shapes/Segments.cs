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
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Diagram.Core {
	public abstract class ShapeSegment {
		public Point Point { get { return pointCore; } }
		Point pointCore;
		public ShapeSegment(Point point) {
			this.pointCore = point;
		}
		public abstract ShapeSegment Offset(Point p);
		public abstract ShapeSegment Rotate(double angle);
		public abstract ShapeSegment Scale(Size scale);
#if DEBUGTEST
		public override string ToString() {
			return pointCore.ToString();
		}
#endif
		public override bool Equals(object obj) {
			if(obj == null || !GetType().Equals(obj.GetType()))
				return false;
			if(obj is ShapeSegment)
				return pointCore.Equals(((ShapeSegment)obj).pointCore);
			return false;
		}
		public override int GetHashCode() {
			return pointCore.GetHashCode();
		}
		internal abstract void ApplyToContext(ShapeSegmentProcessContext context);
		internal abstract string ConvertToString(string format);
	}
	public class LineSegment : ShapeSegment {
		public LineSegment(Point point)
			: base(point) {
		}
		public static LineSegment Create(Point point) {
			return new LineSegment(point);
		}
		public static LineSegment Create(double x, double y) {
			return new LineSegment(new Point(x, y));
		}
		public override ShapeSegment Offset(Point p) {
			return new LineSegment(Point.OffsetPoint(p));
		}
		public override ShapeSegment Rotate(double angle) {
			return new LineSegment(Point.Rotate(angle));
		}
		public override ShapeSegment Scale(Size scale) {
			return new LineSegment(Point.ScalePoint(scale));
		}
		internal override void ApplyToContext(ShapeSegmentProcessContext context) {
			context.ProcessLineSegment(this);
		}
		internal override string ConvertToString(string format) {
			return string.Format(CultureInfo.InvariantCulture, "L{0:" + format + "}", Point);
		}
	}
	[Flags]
	public enum GeometryKind {
		None = 0,
		Closed = 1 << 0,
		Filled = 1 << 1,
		ClosedFilled = Closed | Filled,
	}
	public class StartSegment : ShapeSegment {
		const float DefaultBrightness = 0F;
		readonly GeometryKind kindCore;
		readonly bool isSmoothJoinCore;
		readonly bool isNewShape;
		readonly StartSegmentStyle style;
		public bool IsSmoothJoin { get { return isSmoothJoinCore; } }
		public GeometryKind Kind { get { return kindCore; } }
		public bool IsNewShape { get { return isNewShape; } }
		public StartSegmentStyle Style { get { return style; } }
		public StartSegment(Point point, GeometryKind kind, bool isSmoothJoin, bool isNewShape, StartSegmentStyle style)
			: base(point) {
			kindCore = kind;
			this.isSmoothJoinCore = isSmoothJoin;
			this.isNewShape = isNewShape;
			this.style = style;
		}
		public static StartSegment Create(double x, double y, GeometryKind kind = GeometryKind.ClosedFilled, bool isSmoothJoin = false, float fillBrightness = DefaultBrightness, Color? fillColor = null, bool isNewShape = true) {
			return new StartSegment(new Point(x, y), kind, isSmoothJoin, isNewShape, new StartSegmentStyle(fillBrightness, fillColor, null, null));
		}
		public static StartSegment Create(Point point, GeometryKind kind = GeometryKind.ClosedFilled, bool isSmoothJoin = false, float fillBrightness = DefaultBrightness, Color? fillColor = null, bool isNewShape = true) {
			return new StartSegment(point, kind, isSmoothJoin, isNewShape, new StartSegmentStyle(fillBrightness, fillColor, null, null));
		}
		public override ShapeSegment Offset(Point p) {
			return CreateCopy(Point.OffsetPoint(p));
		}
		public override ShapeSegment Rotate(double angle) {
			return CreateCopy(Point.Rotate(angle));
		}
		public override ShapeSegment Scale(Size scale) {
			return CreateCopy(Point.ScalePoint(scale));
		}
		StartSegment CreateCopy(Point newPoint) {
			return new StartSegment(newPoint, Kind, IsSmoothJoin, IsNewShape, Style);
		}
		internal override void ApplyToContext(ShapeSegmentProcessContext context) {
			context.ProcessStartSegment(this);
		}
		internal override string ConvertToString(string format) {
			return string.Format(CultureInfo.InvariantCulture, "M{0:" + format + "}", Point);
		}
	}
	public class StartSegmentStyle {
		readonly float fillBrightness;
		readonly Color? fillColor;
		readonly Color? strokeColor;
		readonly double? strokeThickness;
		public float FillBrightness { get { return fillBrightness; } }
		public Color? FillColor { get { return fillColor; } }
		public Color? StrokeColor { get { return strokeColor; } }
		public double? StrokeThickness { get { return strokeThickness; } }
		public StartSegmentStyle(float fillBrightness, Color? fillColor, Color? strokeColor, double? strokeThickness) {
			this.fillBrightness = fillBrightness;
			this.fillColor = fillColor;
			this.strokeColor = strokeColor;
			this.strokeThickness = strokeThickness;
		}
	}
	public class ArcSegment : ShapeSegment {
		readonly SweepDirection directionCore;
		public SweepDirection Direction { get { return directionCore; } }
		readonly Size? sizeCore;
		public Size? Size { get { return sizeCore; } }
		public ArcSegment(Point point, SweepDirection direction, Size? size)
			: base(point) {
			this.directionCore = direction;
			this.sizeCore = size;
		}
		public static ArcSegment Create(Point p, Size? size, SweepDirection direction) {
			return new ArcSegment(p, direction, size);
		}
		public static ArcSegment Create(double x, double y, SweepDirection direction) {
			return new ArcSegment(new Point(x, y), direction, null);
		}
		public static ArcSegment Create(double x, double y, Size? size, SweepDirection direction) {
			return new ArcSegment(new Point(x, y), direction, size);
		}
		public override ShapeSegment Offset(Point p) {
			return new ArcSegment(Point.OffsetPoint(p), Direction, Size);
		}
		public override ShapeSegment Rotate(double angle) {
			return new ArcSegment(Point.Rotate(angle), Direction, Size);
		}
		public override ShapeSegment Scale(Size scale) {
			Size? newSize = null;
			if(Size != null)
				newSize = new Size(Size.Value.Width * scale.Width, Size.Value.Height * scale.Height);
			return new ArcSegment(Point.ScalePoint(scale), Direction, newSize);
		}
		internal override void ApplyToContext(ShapeSegmentProcessContext context) {
			context.ProcessArcSegment(this);
		}
		internal override string ConvertToString(string format) {
			return string.Format(CultureInfo.InvariantCulture, "A{0:" + format + "} {1} {2:" + format + "}", Size, (int)Direction, Point);
		}
	}
	public class QuadraticBezierSegment : ShapeSegment {
		public Point Point1 { get; private set; }
		public QuadraticBezierSegment(Point point1, Point endPoint) : base(endPoint) {
			Point1 = point1;
		}
		public override ShapeSegment Offset(Point p) {
			return new QuadraticBezierSegment(Point1.OffsetPoint(p), Point.OffsetPoint(p));
		}
		public override ShapeSegment Rotate(double angle) {
			return new QuadraticBezierSegment(Point1.Rotate(angle), Point.Rotate(angle));
		}
		public override ShapeSegment Scale(Size scale) {
			return new QuadraticBezierSegment(Point1.ScalePoint(scale), Point.ScalePoint(scale));
		}
		internal override void ApplyToContext(ShapeSegmentProcessContext context) {
			context.ProcessQuadraticBezierSegment(this);
		}
		internal override string ConvertToString(string format) {
			return string.Format(CultureInfo.InvariantCulture, "Q{0:" + format + "} {1:" + format + "}", Point1, Point);
		}
	}
	public class BezierSegment : QuadraticBezierSegment {
		public Point Point2 { get; private set; }
		public BezierSegment(Point point1, Point point2, Point endPoint)
			: base(point1, endPoint) {
			Point2 = point2;
		}
		public override ShapeSegment Offset(Point p) {
			return new BezierSegment(Point1.OffsetPoint(p), Point2.OffsetPoint(p), Point.OffsetPoint(p));
		}
		public override ShapeSegment Rotate(double angle) {
			return new BezierSegment(Point1.Rotate(angle), Point2.Rotate(angle), Point.Rotate(angle));
		}
		public override ShapeSegment Scale(Size scale) {
			return new BezierSegment(Point1.ScalePoint(scale), Point2.ScalePoint(scale), Point.ScalePoint(scale));
		}
		internal override void ApplyToContext(ShapeSegmentProcessContext context) {
			context.ProcessBezierSegment(this);
		}
		internal override string ConvertToString(string format) {
			return string.Format(CultureInfo.InvariantCulture, "C{0:" + format + "} {1:" + format + "} {2:" + format + "}", Point1, Point2, Point);
		}
	}
}
