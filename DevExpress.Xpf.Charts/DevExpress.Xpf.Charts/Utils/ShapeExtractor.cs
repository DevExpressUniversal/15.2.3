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
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
namespace DevExpress.Xpf.Charts.Native {
	public static class ShapeExtractor {
		static void Extract(LineSegment segment, List<Point> container) {
			container.Add(segment.Point);
		}
		static void Extract(PolyLineSegment segment, List<Point> container) {
			foreach (Point point in segment.Points)
				container.Add(point);
		}
		static void Extract(BezierSegment segment, List<Point> container) {
			container.Add(segment.Point1);
			container.Add(segment.Point2);
			container.Add(segment.Point3);
		}
		static void Extract(PathSegment segment, List<Point> container) {
			if (segment is LineSegment)
				Extract((LineSegment)segment, container);
			else if (segment is PolyLineSegment)
				Extract((PolyLineSegment)segment, container);
			else if (segment is BezierSegment)
				Extract((BezierSegment)segment, container);
		}
		static void Extract(PathFigure figure, List<Point> container) {
			if (figure.Segments.Count > 0) {
				if (!figure.IsClosed)
					container.Add(figure.StartPoint);
				foreach (PathSegment segment in figure.Segments)
					Extract(segment, container);
			}
		}
		static void Extract(PathGeometry geometry, List<Point> container) {
			foreach (PathFigure figure in geometry.Figures)
				Extract(figure, container);
		}
		static void Extract(DependencyObject obj, List<Shape> container) {
			if (obj == null)
				return;
			if (obj is Shape) {
				Shape shape = (Shape)obj;
				container.Add(shape);
			}
			int count = VisualTreeHelper.GetChildrenCount(obj);
			for (int i = 0; i < count; i++) {
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				Extract(child, container);
			}
		}
		static Transform GetTransformToAncestor(DependencyObject obj) {
			Transform transfrom = new MatrixTransform() { Matrix = Matrix.Identity };
			Visual visual = obj as Visual;
			if (visual != null) {
				Visual parent = VisualTreeHelper.GetParent(visual) as Visual;
				if (parent != null) {
					GeneralTransform general = visual.TransformToAncestor(parent);
					if (general != null) {
						Point point = general.Transform(new Point());
						transfrom = new TranslateTransform() { X = point.X, Y = point.Y };
					}
				}
			}
			return transfrom;
		}
		static void AddTransform(Geometry geometry, Transform transform) {
			if (geometry.Transform == null)
				geometry.Transform = transform;
			else if (transform != null) {
				TransformGroup group = new TransformGroup();
				group.Children.Add(geometry.Transform);
				group.Children.Add(transform);
				geometry.Transform = group;
			}
		}
		static void Arrange(DependencyObject obj) {
			UIElement element = obj as UIElement;
			if (element == null)
				return;
			if (element.DesiredSize.IsEmpty || element.DesiredSize == new Size()) {
				element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				element.Arrange(new Rect() { Width = element.DesiredSize.Width, Height = element.DesiredSize.Height });
			}
		}
		static Geometry ExtractGeometry(Shape shape) {			
			Geometry geometry = shape.RenderedGeometry;
			if (geometry == null || geometry.IsEmpty())
				return null;
			Transform transformToAncestor = GetTransformToAncestor(shape);
			AddTransform(geometry, transformToAncestor);
			return geometry;
		}
		static void Extract(Geometry geometry, List<Point> container) {
			PathGeometry pathGeometry = geometry.GetFlattenedPathGeometry();
			if(pathGeometry == null || pathGeometry.IsEmpty())
				return;
			Extract(pathGeometry, container);
		}
		static void Extract(Shape shape, List<Point> container) {
			Geometry geometry = ExtractGeometry(shape);
			if(geometry == null || geometry.IsEmpty())
				return;
			Extract(geometry, container);
		}
		public static List<Shape> ExtractShapes(DependencyObject obj) {
			Arrange(obj);
			List<Shape> container = new List<Shape>();
			Extract(obj, container);
			return container;
		}
		public static Polyline ExtractPolyline(DependencyObject obj) {
			List<Shape> shapes = ExtractShapes(obj);
			List<Point> container = new List<Point>();
			foreach (Shape shape in shapes)
				Extract(shape, container);
			Polyline polyline = new Polyline();
			polyline.Points = new PointCollection();
			foreach (Point point in container)
				polyline.Points.Add(point);
			Arrange(polyline);
			return polyline;
		}
	}
}
