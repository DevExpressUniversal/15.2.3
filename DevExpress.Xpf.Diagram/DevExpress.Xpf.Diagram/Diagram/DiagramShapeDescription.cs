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

using DevExpress.Diagram.Core;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Diagram.Core.Themes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Diagram.Native {
	class GeometrySegment {
		readonly Brush brush;
		readonly Geometry geometry;
		readonly Pen pen;
		public Brush Brush { get { return brush; } }
		public Geometry Geometry { get { return geometry; } }
		public Pen Pen { get { return pen; } }
		public GeometrySegment(Geometry geometry, Brush brush, Pen pen) {
			this.pen = pen;
			this.brush = brush;
			this.geometry = geometry;
		}
	}
	public static class DiagramShapeFactory {
		public static StreamGeometry Create(ShapeGeometry shape) {
			StreamGeometry geometry = new StreamGeometry();
			using(var context = geometry.Open()) {
				StreamGeometryParseStrategy paintStrategy = new StreamGeometryParseStrategy(context);
				ShapeParser painter = new ShapeParser(paintStrategy);
				painter.Parse(shape);
			}
			return geometry;
		}
		public static void Draw(ShapeGeometry shape, DrawingContext context, Brush brush, Pen pen) {
			var segments = Parse(shape, brush, pen);
			foreach(var segment in segments) {
				context.DrawGeometry(segment.Brush, segment.Pen, segment.Geometry);
			}
		}
		internal static IEnumerable<GeometrySegment> Parse(ShapeGeometry shape, Brush background, Pen pen) {
			GeometrySegmentParseStrategy paintStrategy = new GeometrySegmentParseStrategy(background, pen);
			ShapeParser painter = new ShapeParser(paintStrategy);
			painter.Parse(shape);
			return paintStrategy.Segments;
		}
	}
	public abstract class StreamGeometryParseStrategyBase : ShapeParseStrategyBase {
		protected abstract StreamGeometryContext Context { get; }
		protected override void BeginFigureCore(Point startPoint, bool isFilled, bool isClosed, bool isNewShape, StartSegmentStyle style) {
			Context.BeginFigure(startPoint, isFilled, isClosed);
		}
		public override void LineTo(Point point, bool isStroked) {
			Context.LineTo(point, isStroked, IsSmoothJoin);
		}
		public override void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked) {
			Context.ArcTo(point, size, 0, isLargeArc, sweepDirection, isStroked, IsSmoothJoin);
		}
		public override void BezierTo(Point point1, Point point2, Point point3, bool isStroked) {
			Context.BezierTo(point1, point2, point3, isStroked, IsSmoothJoin);
		}
		public override void QuadraticBezierTo(Point point1, Point point2, bool isStroked) {
			Context.QuadraticBezierTo(point1, point2, isStroked, IsSmoothJoin);
		}
		public override void EndFigure() { }
	}
	public class StreamGeometryParseStrategy : StreamGeometryParseStrategyBase {
		readonly StreamGeometryContext context;
		protected override StreamGeometryContext Context {
			get { return context; }
		}
		public StreamGeometryParseStrategy(StreamGeometryContext context) {
			this.context = context;
		}
	}
	class GeometrySegmentParseStrategy : StreamGeometryParseStrategyBase {
		readonly Brush defaultBrush;
		readonly Pen defaultPen;
		readonly List<GeometrySegment> segments;
		StreamGeometryContext geometryContext;
		public readonly ReadOnlyCollection<GeometrySegment> Segments;
		protected override StreamGeometryContext Context {
			get { return geometryContext; }
		}
		public GeometrySegmentParseStrategy(Brush background, Pen pen) {
			this.defaultBrush = background;
			this.defaultPen = pen;
			this.segments = new List<GeometrySegment>();
			Segments = new ReadOnlyCollection<GeometrySegment>(segments);
		}
		protected override void BeginFigureCore(Point startPoint, bool isFilled, bool isClosed, bool isNewShape, StartSegmentStyle style) {
			if(isNewShape) {
				var geometry = new StreamGeometry();
				var brush = GetBackgroundBrush(defaultBrush, style);
				var pen = GetPen(defaultPen, style);
				segments.Add(new GeometrySegment(geometry, brush, pen));
				geometryContext = geometry.Open();
			}
			base.BeginFigureCore(startPoint, isFilled, isClosed, isNewShape, style);
		}
		static Brush GetBackgroundBrush(Brush defaultBrush, StartSegmentStyle style) {
			var defaultColor = DiagramItemStyleHelper.BrushToColor(defaultBrush);
			if(defaultColor.HasValue)
				return DiagramItemStyleHelper.ColorToBrush(style.GetFillColor(defaultColor.Value));
			return defaultBrush;
		}
		static Pen GetPen(Pen defaultPen, StartSegmentStyle style) {
			if(style.StrokeColor.HasValue || style.StrokeThickness.HasValue) {
				var pen = defaultPen.Clone();
				if(style.StrokeColor.HasValue)
					pen.Brush = DiagramItemStyleHelper.ColorToBrush(style.StrokeColor.Value);
				if(style.StrokeThickness.HasValue)
					pen.Thickness = style.StrokeThickness.Value;
				pen.Freeze();
				return pen;
			}
			return defaultPen;
		}
		public override void EndFigure() {
			geometryContext.Close();
			base.EndFigure();
		}
	}
}
