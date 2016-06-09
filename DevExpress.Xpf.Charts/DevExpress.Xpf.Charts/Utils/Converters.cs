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
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Charts.Native;
using System.Windows.Controls;
namespace DevExpress.Xpf.Charts {
	public class IndicatorGeometryToGeometryConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Geometry) && value is IndicatorGeometry)
				return ((IndicatorGeometry)value).CreateGeometry();
			else
				return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class BrushOverlayConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Brush) && value is Color) {
				Color targetColor = (Color)value;
				Brush maskBrush = null;
				if (parameter is string)
					maskBrush = new SolidColorBrush(ColorHelper.CreateColorFromString(parameter as string));
				if (parameter is Color)
					maskBrush = new SolidColorBrush((Color)parameter);
				if (parameter is Brush)
					maskBrush = parameter as Brush;
				return maskBrush != null ? ColorHelper.MixColors(ColorHelper.NormalizeGradients(maskBrush), targetColor) : new SolidColorBrush(targetColor);
			}
			else
				return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class RectsListToGeometryConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Geometry)) {
				if (value is Rect)
					return new RectangleGeometry() { Rect = (Rect)value };
				if (value is List<Rect>) {
					GeometryGroup geometryGroup = new GeometryGroup();
					foreach (Rect rect in (List<Rect>)value)
						geometryGroup.Children.Add(new RectangleGeometry() { Rect = rect });
					return geometryGroup;
				}				
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class LinesListToGeometryConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Geometry)) {
				if (value is GRealLine2D) {
					GRealLine2D line = (GRealLine2D)value;
					return new LineGeometry() { StartPoint = new Point(line.Start.X, line.Start.Y), EndPoint = new Point(line.End.X, line.End.Y) };
				}
				if (value is List<GRealLine2D>) {
					GeometryGroup geometryGroup = new GeometryGroup();
					foreach (GRealLine2D line in (List<GRealLine2D>)value) {
						geometryGroup.Children.Add(new LineGeometry() { StartPoint = new Point(line.Start.X, line.Start.Y), EndPoint = new Point(line.End.X, line.End.Y) });
					}
					return geometryGroup;
				}
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class GridLineGeometryToGeometryConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Geometry) && (value is GridLineGeometry)) {
				GridLineGeometry geometry = (GridLineGeometry)value;
				switch (geometry.Type) { 
					case GridLineType.Polyline:
						if (geometry.Points.Count > 2) {
							PathFigure pathFigure = new PathFigure();
							pathFigure.StartPoint = geometry.Points[0];
							pathFigure.IsClosed = true;
							for (int i = 1; i < geometry.Points.Count; i++) {
								LineSegment lineSegment = new LineSegment();
								lineSegment.Point = geometry.Points[i];
								pathFigure.Segments.Add(lineSegment);
							}
							PathGeometry pathGeometry = new PathGeometry();
							pathGeometry.Figures.Add(pathFigure);
							return pathGeometry;
						}
						else if (geometry.Points.Count > 1)
							return new LineGeometry() { StartPoint = geometry.Points[0], EndPoint = geometry.Points[1] };
					return null;
					case GridLineType.Ellipse: {
						if (geometry.Points.Count > 1 ) {
							Point center = new Point(0.5 * (geometry.Points[0].X + geometry.Points[1].X), 0.5 * (geometry.Points[0].Y + geometry.Points[1].Y));
							double rx = 0.5 * Math.Abs(geometry.Points[0].X - geometry.Points[1].X);
							double ry = 0.5 * Math.Abs(geometry.Points[0].Y - geometry.Points[1].Y);
							return new EllipseGeometry() { Center = center, RadiusX = rx, RadiusY = ry };
						}
						return null;						
					}
					default:
					return null;
				}
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class InterlaceGeometryToGeometryConverter : IValueConverter {
		ArcSegment CreateArc(Point p1, Point p2, Rect bounds, SweepDirection direction) {
			GRealPoint2D center = new GRealPoint2D (0.5 * (bounds.Left + bounds.Right), 0.5 * (bounds.Top + bounds.Right));
			double angle1 = GeometricUtils.CalcBetweenPointsAngle(center, new GRealPoint2D(p1.X, p1.Y));
			double angle2 = GeometricUtils.NormalizeRadian(GeometricUtils.CalcBetweenPointsAngle(center, new GRealPoint2D(p2.X, p2.Y)) - angle1);
			bool largeAngle = direction == SweepDirection.Clockwise ? angle2 > Math.PI : angle2 < Math.PI;
			ArcSegment arc = new ArcSegment() {Point = p2, IsLargeArc = largeAngle, Size = new Size(0.5 * bounds.Height , 0.5 * bounds.Width), SweepDirection = direction};
			return arc;
		}
		Geometry CreateRectGeometry(List<Point> points) {
			return points.Count > 1 ? new RectangleGeometry() { Rect = new Rect(points[0], points[1]) } : null;
		}
		Geometry CreatePolygonGeometry(List<Point> points, List<Point> holePoints) {
			if (points.Count > 2) {
				PolyLineSegment segment = new PolyLineSegment() { Points = new PointCollection() };
				foreach (Point point in points)
					segment.Points.Add(point);
				PathFigure figure = new PathFigure() { StartPoint = points[0], IsClosed = true, Segments = new PathSegmentCollection() { segment } };
				PathGeometry result = new PathGeometry() { FillRule = FillRule.EvenOdd, Figures = new PathFigureCollection() { figure } };
				if (holePoints.Count > 2) {
					segment = new PolyLineSegment() { Points = new PointCollection() };
					foreach (Point point in holePoints)
						segment.Points.Add(point);
					result.Figures.Add(new PathFigure() { StartPoint = holePoints[0], IsClosed = true, Segments = new PathSegmentCollection() { segment } });
				}
				return result;
			}
			return null;
		}
		Geometry CreatePieGeometry(List<Point> points, List<Point> holePoints, Rect bounds, SweepDirection direction) {
			if (points.Count > 2) {
				PolyLineSegment line = new PolyLineSegment() { Points = new PointCollection() { points[1], points[2] } };
				PathFigure figure = new PathFigure() { StartPoint = points[0], IsClosed = true, Segments = new PathSegmentCollection() { CreateArc(points[0], points[1], bounds, direction), line } };
				return  new PathGeometry() { FillRule = FillRule.EvenOdd, Figures = new PathFigureCollection() { figure } };
			}
			return null;
		}
		Geometry CreateEllipse(List<Point> points, List<Point> holePoints) {
			if (points.Count > 1) {
				GeometryGroup result = new GeometryGroup();
				double rx = Math.Abs(0.5 * ( points[0].X - points[1].X));
				double ry = Math.Abs(0.5 * (points[0].Y - points[1].Y));
				EllipseGeometry geometry = new EllipseGeometry() { Center = new Point(0.5 * (points[0].X + points[1].X), 0.5 * (points[0].X + points[1].X)), RadiusX = rx, RadiusY = ry };
				result.Children.Add(geometry);
				if (holePoints.Count > 1) {
					rx = Math.Abs(0.5 * (holePoints[0].X - holePoints[1].X));
					ry = Math.Abs(0.5 * (holePoints[0].Y - holePoints[1].Y));
					geometry = new EllipseGeometry() { Center = new Point(0.5 * (holePoints[0].X + holePoints[1].X), 0.5 * (holePoints[0].X + holePoints[1].X)), RadiusX = rx, RadiusY = ry };
					result.Children.Add(geometry);
				}
				return result;
			}
			return null;
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Geometry) && (value is InterlaceGeometry)) {
				InterlaceGeometry geometry = (InterlaceGeometry)value;
				switch (geometry.Type) {
					case InterlaceType.Rectangle:
						return CreateRectGeometry(geometry.Points);
					case InterlaceType.Polygon: 
						return CreatePolygonGeometry(geometry.Points, geometry.HolePoints);
					case InterlaceType.ClockwisePie:
						return CreatePieGeometry(geometry.Points, geometry.HolePoints, geometry.Bounds, SweepDirection.Clockwise);
					case InterlaceType.CounterclockwisePie:
						return CreatePieGeometry(geometry.Points, geometry.HolePoints, geometry.Bounds, SweepDirection.Counterclockwise);
					case InterlaceType.Ellipse: 
						return CreateEllipse(geometry.Points, geometry.HolePoints);
					default:
					return null;
				}
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class XYSeries2DToAdditionalGeometryConverter : IValueConverter {
		static Control CreateAdditionalGeometry(Series series) {
			if (series is XYSeries2D)
				return ((XYSeries2D)series).CreateAdditionalGeometry();
			if (series is CircularSeries2D)
				return ((CircularSeries2D)series).CreateAdditionalGeometry();
			return null;
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			XYSeries series = value as XYSeries;
			if (series != null){
				Control additionalGeometry = CreateAdditionalGeometry(series);
				additionalGeometry.DataContext = series;
				return additionalGeometry;
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class AnnotationLocationToTransformConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is AnnotationLocation && targetType == typeof(Transform)) {
				AnnotationLocation position = (AnnotationLocation)value;
				TransformGroup transform = new TransformGroup();
				switch (position) {
					case AnnotationLocation.BottomLeft:
						transform.Children.Add(new ScaleTransform() { ScaleX = -1, ScaleY = -1});
						break;
					case AnnotationLocation.BottomRight:
						transform.Children.Add(new ScaleTransform() { ScaleY = -1});
						break;
					case AnnotationLocation.TopLeft:
						transform.Children.Add(new ScaleTransform() { ScaleX = -1});
						break;
					default:
						break;
				}
				return transform;
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PointModelToModelControlConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(ModelControl)) {
				PointModel model = value as PointModel;
				if(model == null) 
					return null;
				ModelControl control = model.CreateModelControl();
				control.Name = "PART_PointPresenter";
				return control;
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PointCollectionToGeometryConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			List<Point> points = value as List<Point>;
			if (targetType == typeof(Geometry) && points != null && points.Count > 1) {
				PointCollection newPoints = new PointCollection();
				foreach (Point point in points)
					newPoints.Add(point);
				PolyLineSegment segment = new PolyLineSegment() { Points = newPoints };
				PathFigure figure = new PathFigure() { IsFilled = false, IsClosed = false, StartPoint = points[0], Segments = new PathSegmentCollection() };
				figure.Segments.Add(segment); 
				PathGeometry geometry = new PathGeometry() { Figures = new PathFigureCollection() };
				geometry.Figures.Add(figure);
				return geometry;
			}
			else
				return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class SelectionListRectToGeometryConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Geometry) && value is List<Rect>) {
				List<Rect> selectionRects = (List<Rect>)value;
				if (selectionRects.Count == 1) {
					return new RectangleGeometry() {
						Rect = selectionRects[0],
						RadiusX = VisualSelectionHelper.SelectionRectCornerRadius,
						RadiusY = VisualSelectionHelper.SelectionRectCornerRadius
					};
				}
				else if (selectionRects.Count > 1) {
					EllipseGeometry outerEllipse = CreateEllipseGeometry(selectionRects[0]);
					EllipseGeometry innerEllipse = CreateEllipseGeometry(selectionRects[1]);
					return Geometry.Combine(outerEllipse, innerEllipse, GeometryCombineMode.Exclude, null);
				}
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		EllipseGeometry CreateEllipseGeometry(Rect rect) {
			return new EllipseGeometry() {
				RadiusX = rect.Width / 2,
				RadiusY = rect.Height / 2,
				Center = rect.Center()
			};
		}
	}
	public class IsEnabledToOpacityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(double) && value is bool)
				return (bool)value ? 1.0 : 0.35;
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class BoolToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if ((value.GetType() != typeof(bool)) && (value.GetType() != typeof(bool?)))
				return value;
			return (bool)value ? Visibility.Visible : Visibility.Hidden;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
