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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public static class DoubleUtil {
		public static bool IsZero(double value) {
			return (Math.Abs(value) < 2.2204460492503131E-15);
		}
		public static bool AreClose(double value1, double value2) {
			if (value1 == value2) {
				return true;
			}
			double num2 = ((Math.Abs(value1) + Math.Abs(value2)) + 10.0) * 2.2204460492503131E-16;
			double num = value1 - value2;
			return ((-num2 < num) && (num2 > num));
		}
		public static bool AreCloseApproximately(double value1, double value2, double tolerance) {
			if (value1 == value2) {
				return true;
			}
			double diff = value1 - value2;
			return ((-tolerance < diff) && (tolerance > diff));
		}
		public static bool GreaterThan(double value1, double value2) {
			return ((value1 > value2) && !AreClose(value1, value2));
		}
		public static bool GreaterThanOrClose(double value1, double value2) {
			if (value1 <= value2) {
				return AreClose(value1, value2);
			}
			return true;
		}
		public static bool LessThan(double value1, double value2) {
			return GreaterThan(value2, value1);
		}
		public static bool LessThanOrClose(double value1, double value2) {
			return GreaterThanOrClose(value2, value1);
		}
	}
	public class RoundedBorderControl : XPFContentControl {
		public RoundedBorderControl() {
			DefaultStyleKey = typeof(RoundedBorderControl);
		}
		#region CornerRadius
		public CornerRadius CornerRadius {
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}
		public static readonly DependencyProperty CornerRadiusProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RoundedBorderControl, CornerRadius>("CornerRadius", new CornerRadius());
		#endregion
		#region RoundBorderThickness
		public static readonly DependencyProperty RoundBorderThicknessProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RoundedBorderControl, Thickness>("RoundBorderThickness", new Thickness(0));
		public Thickness RoundBorderThickness { get { return (Thickness)GetValue(RoundBorderThicknessProperty); } set { SetValue(RoundBorderThicknessProperty, value); } }
		#endregion
	}
#if SL
	public class RoundedBorder : SchedulerBorder {
		#region Radii struct
		struct Radii {
			internal double LeftTop;
			internal double TopLeft;
			internal double TopRight;
			internal double RightTop;
			internal double RightBottom;
			internal double BottomRight;
			internal double BottomLeft;
			internal double LeftBottom;
			internal Radii(CornerRadius radii, Thickness borders, bool outer) {
				double num = 0.5 * borders.Left;
				double num2 = 0.5 * borders.Top;
				double num3 = 0.5 * borders.Right;
				double num4 = 0.5 * borders.Bottom;
				if (outer) {
					if (DoubleUtil.IsZero(radii.TopLeft)) {
						this.LeftTop = this.TopLeft = 0.0;
					}
					else {
						this.LeftTop = radii.TopLeft + num;
						this.TopLeft = radii.TopLeft + num2;
					}
					if (DoubleUtil.IsZero(radii.TopRight)) {
						this.TopRight = this.RightTop = 0.0;
					}
					else {
						this.TopRight = radii.TopRight + num2;
						this.RightTop = radii.TopRight + num3;
					}
					if (DoubleUtil.IsZero(radii.BottomRight)) {
						this.RightBottom = this.BottomRight = 0.0;
					}
					else {
						this.RightBottom = radii.BottomRight + num3;
						this.BottomRight = radii.BottomRight + num4;
					}
					if (DoubleUtil.IsZero(radii.BottomLeft)) {
						this.BottomLeft = this.LeftBottom = 0.0;
					}
					else {
						this.BottomLeft = radii.BottomLeft + num4;
						this.LeftBottom = radii.BottomLeft + num;
					}
				}
				else {
					this.LeftTop = Math.Max((double)0.0, (double)(radii.TopLeft - num));
					this.TopLeft = Math.Max((double)0.0, (double)(radii.TopLeft - num2));
					this.TopRight = Math.Max((double)0.0, (double)(radii.TopRight - num2));
					this.RightTop = Math.Max((double)0.0, (double)(radii.TopRight - num3));
					this.RightBottom = Math.Max((double)0.0, (double)(radii.BottomRight - num3));
					this.BottomRight = Math.Max((double)0.0, (double)(radii.BottomRight - num4));
					this.BottomLeft = Math.Max((double)0.0, (double)(radii.BottomLeft - num4));
					this.LeftBottom = Math.Max((double)0.0, (double)(radii.BottomLeft - num));
				}
			}
		}
		#endregion
		#region BackgroundGeometryCache
		static readonly DependencyProperty BackgroundGeometryCacheProperty = DependencyProperty.Register("BackgroundGeometryCache", typeof(PathGeometry), typeof(RoundedBorder), new PropertyMetadata(null));
		public PathGeometry BackgroundGeometryCache { get { return (PathGeometry)GetValue(BackgroundGeometryCacheProperty); } protected set { SetValue(BackgroundGeometryCacheProperty, value); } }
		#endregion
		private static Size HelperCollapseThickness(Thickness th) {
			return new Size(th.Left + th.Right, th.Top + th.Bottom);
		}
		private static Rect HelperDeflateRect(Rect rt, Thickness thick) {
			return new Rect(rt.Left + thick.Left, rt.Top + thick.Top, Math.Max((double)0.0, (double)((rt.Width - thick.Left) - thick.Right)), Math.Max((double)0.0, (double)((rt.Height - thick.Top) - thick.Bottom)));
		}
		internal bool IsUniform(Thickness thickness) {
			return ((DoubleUtil.AreClose(thickness.Left, thickness.Top) && DoubleUtil.AreClose(thickness.Left, thickness.Right)) && DoubleUtil.AreClose(thickness.Left, thickness.Bottom));
		}
		internal bool IsZero(Thickness thickness) {
			return (((DoubleUtil.IsZero(thickness.Left) && DoubleUtil.IsZero(thickness.Top)) && DoubleUtil.IsZero(thickness.Right)) && DoubleUtil.IsZero(thickness.Bottom));
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			base.ArrangeOverride(arrangeSize);
			Rect bounds = HelperDeflateRect(new Rect(new Point(0, 0), arrangeSize), BorderThickness);
			if (!DoubleUtil.IsZero(bounds.Width) && !DoubleUtil.IsZero(bounds.Height)) {
				Radii radii = new Radii(CornerRadius, BorderThickness, false);
				PathGeometry geometry = new PathGeometry();
				GenerateGeometry(geometry, bounds, radii);
				BackgroundGeometryCache = geometry;
			}
			else
				BackgroundGeometryCache = null;
			if (Child != null)
				Child.Clip = BackgroundGeometryCache;
			return arrangeSize;
		}
		private static void GenerateGeometry(PathGeometry geometry, Rect rect, Radii radii) {
			PathSegmentCollection segments = new PathSegmentCollection();
			GenerateSegments(segments, rect, radii);
			PathFigure path = new PathFigure();
			path.IsClosed = true;
			path.IsFilled = true;
			path.Segments = segments;
			geometry.Figures.Add(path);
		}
		private static void GenerateSegments(PathSegmentCollection segments, Rect rect, Radii radii) {
			Point startPoint = new Point(radii.LeftTop, 0.0);
			Point point = new Point(rect.Width - radii.RightTop, 0.0);
			Point point3 = new Point(rect.Width, radii.TopRight);
			Point point4 = new Point(rect.Width, rect.Height - radii.BottomRight);
			Point point5 = new Point(rect.Width - radii.RightBottom, rect.Height);
			Point point6 = new Point(radii.LeftBottom, rect.Height);
			Point point7 = new Point(0.0, rect.Height - radii.BottomLeft);
			Point point8 = new Point(0.0, radii.TopLeft);
			if (startPoint.X > point.X) {
				double num = (radii.LeftTop / (radii.LeftTop + radii.RightTop)) * rect.Width;
				startPoint.X = num;
				point.X = num;
			}
			if (point3.Y > point4.Y) {
				double num2 = (radii.TopRight / (radii.TopRight + radii.BottomRight)) * rect.Height;
				point3.Y = num2;
				point4.Y = num2;
			}
			if (point5.X < point6.X) {
				double num3 = (radii.LeftBottom / (radii.LeftBottom + radii.RightBottom)) * rect.Width;
				point5.X = num3;
				point6.X = num3;
			}
			if (point7.Y < point8.Y) {
				double num4 = (radii.TopLeft / (radii.TopLeft + radii.BottomLeft)) * rect.Height;
				point7.Y = num4;
				point8.Y = num4;
			}
			Vector vector = new Vector(rect.Left, rect.Top);
			startPoint += vector;
			point += vector;
			point3 += vector;
			point4 += vector;
			point5 += vector;
			point6 += vector;
			point7 += vector;
			point8 += vector;
			segments.Add(new LineSegment() { Point = startPoint });
			segments.Add(new LineSegment() { Point = point });
			double width = rect.Right - point.X;
			double height = point3.Y - rect.Top;
			if (!DoubleUtil.IsZero(width) || !DoubleUtil.IsZero(height)) {
				segments.Add(new ArcSegment() { Size = new Size(width, height), Point = point3, SweepDirection = SweepDirection.Clockwise, IsLargeArc = false });
			}
			segments.Add(new LineSegment() { Point = point4 });
			width = rect.Right - point5.X;
			height = rect.Bottom - point4.Y;
			if (!DoubleUtil.IsZero(width) || !DoubleUtil.IsZero(height)) {
				segments.Add(new ArcSegment() { Size = new Size(width, height), Point = point5, SweepDirection = SweepDirection.Clockwise, IsLargeArc = false });
			}
			segments.Add(new LineSegment() { Point = point6 });
			width = point6.X - rect.Left;
			height = rect.Bottom - point7.Y;
			if (!DoubleUtil.IsZero(width) || !DoubleUtil.IsZero(height)) {
				segments.Add(new ArcSegment() { Size = new Size(width, height), Point = point7, SweepDirection = SweepDirection.Clockwise, IsLargeArc = false });
			}
			segments.Add(new LineSegment() { Point = point8 });
			width = startPoint.X - rect.Left;
			height = point8.Y - rect.Top;
			if (!DoubleUtil.IsZero(width) || !DoubleUtil.IsZero(height)) {
				segments.Add(new ArcSegment() { Size = new Size(width, height), Point = startPoint, SweepDirection = SweepDirection.Clockwise, IsLargeArc = false });
			}
		}
	}
#else
	public class RoundedBorder : Decorator {
		private struct Radii {
			internal double LeftTop;
			internal double TopLeft;
			internal double TopRight;
			internal double RightTop;
			internal double RightBottom;
			internal double BottomRight;
			internal double BottomLeft;
			internal double LeftBottom;
			internal Radii(CornerRadius radii, Thickness borders, bool outer) {
				double num = 0.5 * borders.Left;
				double num2 = 0.5 * borders.Top;
				double num3 = 0.5 * borders.Right;
				double num4 = 0.5 * borders.Bottom;
				if (outer) {
					if (DoubleUtil.IsZero(radii.TopLeft)) {
						this.LeftTop = this.TopLeft = 0.0;
					}
					else {
						this.LeftTop = radii.TopLeft + num;
						this.TopLeft = radii.TopLeft + num2;
					}
					if (DoubleUtil.IsZero(radii.TopRight)) {
						this.TopRight = this.RightTop = 0.0;
					}
					else {
						this.TopRight = radii.TopRight + num2;
						this.RightTop = radii.TopRight + num3;
					}
					if (DoubleUtil.IsZero(radii.BottomRight)) {
						this.RightBottom = this.BottomRight = 0.0;
					}
					else {
						this.RightBottom = radii.BottomRight + num3;
						this.BottomRight = radii.BottomRight + num4;
					}
					if (DoubleUtil.IsZero(radii.BottomLeft)) {
						this.BottomLeft = this.LeftBottom = 0.0;
					}
					else {
						this.BottomLeft = radii.BottomLeft + num4;
						this.LeftBottom = radii.BottomLeft + num;
					}
				}
				else {
					this.LeftTop = Math.Max((double)0.0, (double)(radii.TopLeft - num));
					this.TopLeft = Math.Max((double)0.0, (double)(radii.TopLeft - num2));
					this.TopRight = Math.Max((double)0.0, (double)(radii.TopRight - num2));
					this.RightTop = Math.Max((double)0.0, (double)(radii.TopRight - num3));
					this.RightBottom = Math.Max((double)0.0, (double)(radii.BottomRight - num3));
					this.BottomRight = Math.Max((double)0.0, (double)(radii.BottomRight - num4));
					this.BottomLeft = Math.Max((double)0.0, (double)(radii.BottomLeft - num4));
					this.LeftBottom = Math.Max((double)0.0, (double)(radii.BottomLeft - num));
				}
			}
		}
		bool _useComplexRenderCodePath;
		#region BorderThickness
		public Thickness BorderThickness {
			get { return (Thickness)GetValue(BorderThicknessProperty); }
			set { SetValue(BorderThicknessProperty, value); }
		}
		public static readonly DependencyProperty BorderThicknessProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RoundedBorder, Thickness>("BorderThickness", new Thickness(0), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure);
		#endregion
		#region CornerRadius
		public CornerRadius CornerRadius {
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}
		public static readonly DependencyProperty CornerRadiusProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RoundedBorder, CornerRadius>("CornerRadius", new CornerRadius(), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure);
		#endregion
		#region BorderBrush
		public Brush BorderBrush { 
			get { return (Brush)GetValue(BorderBrushProperty); } 
			set { SetValue(BorderBrushProperty, value); } }
		public static readonly DependencyProperty BorderBrushProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RoundedBorder, Brush>("BorderBrush", null, FrameworkPropertyMetadataOptions.AffectsRender);
		#endregion
		#region Background
		public static readonly DependencyProperty BackgroundProperty = Border.BackgroundProperty.AddOwner(typeof(RoundedBorder));
		public Brush Background { get { return (Brush)GetValue(BackgroundProperty); } set { SetValue(BackgroundProperty, value); } }
		#endregion
		#region BackgroundGeometryCache
		static readonly DependencyPropertyKey BackgroundGeometryCachePropertyKey = DependencyProperty.RegisterReadOnly("BackgroundGeometryCache", typeof(StreamGeometry), typeof(RoundedBorder), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty BackgroundGeometryCacheProperty = BackgroundGeometryCachePropertyKey.DependencyProperty;
		public StreamGeometry BackgroundGeometryCache { get { return (StreamGeometry)GetValue(BackgroundGeometryCacheProperty); } protected set { SetValue(BackgroundGeometryCachePropertyKey, value); } }
		#endregion
		StreamGeometry BorderGeometryCache { get; set; }
		Pen LeftPenCache { get; set; }
		Pen RightPenCache { get; set; }
		Pen TopPenCache { get; set; }
		Pen BottomPenCache { get; set; }
		protected override Size MeasureOverride(Size constraint) {
			UIElement child = this.Child;
			Size size = new Size();
			Size size2 = HelperCollapseThickness(this.BorderThickness);
			if (child != null) {
				Size size4 = new Size(size2.Width, size2.Height);
				Size childConstraint = new Size(Math.Max((double)0.0, (double)(constraint.Width - size4.Width)), Math.Max((double)0.0, (double)(constraint.Height - size4.Height)));
				child.Measure(childConstraint);
				Size desiredSize = child.DesiredSize;
				size.Width = desiredSize.Width + size4.Width;
				size.Height = desiredSize.Height + size4.Height;
				return size;
			}
			return new Size(size2.Width, size2.Height);
		}
		private static Size HelperCollapseThickness(Thickness th) {
			return new Size(th.Left + th.Right, th.Top + th.Bottom);
		}
		private static Rect HelperDeflateRect(Rect rt, Thickness thick) {
			return new Rect(rt.Left + thick.Left, rt.Top + thick.Top, Math.Max((double)0.0, (double)((rt.Width - thick.Left) - thick.Right)), Math.Max((double)0.0, (double)((rt.Height - thick.Top) - thick.Bottom)));
		}
		internal bool IsUniform(Thickness thickness) {
			return ((DoubleUtil.AreClose(thickness.Left, thickness.Top) && DoubleUtil.AreClose(thickness.Left, thickness.Right)) && DoubleUtil.AreClose(thickness.Left, thickness.Bottom));
		}
		internal bool IsZero(Thickness thickness) {
			return (((DoubleUtil.IsZero(thickness.Left) && DoubleUtil.IsZero(thickness.Top)) && DoubleUtil.IsZero(thickness.Right)) && DoubleUtil.IsZero(thickness.Bottom));
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			Thickness borderThickness = this.BorderThickness;
			Rect rt = new Rect(arrangeSize);
			Rect rect2 = HelperDeflateRect(rt, borderThickness);
			UIElement child = this.Child;
			if (child != null) {
				child.Arrange(rect2);
			}
			CornerRadius cornerRadius = this.CornerRadius;
			this._useComplexRenderCodePath = true;
			if (this._useComplexRenderCodePath) {
				Radii radii = new Radii(cornerRadius, borderThickness, false);
				StreamGeometry geometry = null;
				if (!DoubleUtil.IsZero(rect2.Width) && !DoubleUtil.IsZero(rect2.Height)) {
					geometry = new StreamGeometry();
					using (StreamGeometryContext context = geometry.Open()) {
						GenerateGeometry(context, rect2, radii);
					}
					geometry.Freeze();
					this.BackgroundGeometryCache = geometry;
				}
				else {
					this.BackgroundGeometryCache = null;
				}
				if (!DoubleUtil.IsZero(rt.Width) && !DoubleUtil.IsZero(rt.Height)) {
					Radii radii2 = new Radii(cornerRadius, borderThickness, true);
					StreamGeometry geometry2 = new StreamGeometry();
					using (StreamGeometryContext context2 = geometry2.Open()) {
						GenerateGeometry(context2, rt, radii2);
						if (geometry != null) {
							GenerateGeometry(context2, rect2, radii);
						}
					}
					geometry2.Freeze();
					this.BorderGeometryCache = geometry2;
					return arrangeSize;
				}
				this.BorderGeometryCache = null;
				return arrangeSize;
			}
			this.BackgroundGeometryCache = null;
			this.BorderGeometryCache = null;
			return arrangeSize;
		}
		private static bool AreUniformCorners(CornerRadius borderRadii) {
			double topLeft = borderRadii.TopLeft;
			return ((DoubleUtil.AreClose(topLeft, borderRadii.TopRight) && DoubleUtil.AreClose(topLeft, borderRadii.BottomLeft)) && DoubleUtil.AreClose(topLeft, borderRadii.BottomRight));
		}
		private static void GenerateGeometry(StreamGeometryContext ctx, Rect rect, Radii radii) {
			Point startPoint = new Point(radii.LeftTop, 0.0);
			Point point = new Point(rect.Width - radii.RightTop, 0.0);
			Point point3 = new Point(rect.Width, radii.TopRight);
			Point point4 = new Point(rect.Width, rect.Height - radii.BottomRight);
			Point point5 = new Point(rect.Width - radii.RightBottom, rect.Height);
			Point point6 = new Point(radii.LeftBottom, rect.Height);
			Point point7 = new Point(0.0, rect.Height - radii.BottomLeft);
			Point point8 = new Point(0.0, radii.TopLeft);
			if (startPoint.X > point.X) {
				double num = (radii.LeftTop / (radii.LeftTop + radii.RightTop)) * rect.Width;
				startPoint.X = num;
				point.X = num;
			}
			if (point3.Y > point4.Y) {
				double num2 = (radii.TopRight / (radii.TopRight + radii.BottomRight)) * rect.Height;
				point3.Y = num2;
				point4.Y = num2;
			}
			if (point5.X < point6.X) {
				double num3 = (radii.LeftBottom / (radii.LeftBottom + radii.RightBottom)) * rect.Width;
				point5.X = num3;
				point6.X = num3;
			}
			if (point7.Y < point8.Y) {
				double num4 = (radii.TopLeft / (radii.TopLeft + radii.BottomLeft)) * rect.Height;
				point7.Y = num4;
				point8.Y = num4;
			}
			Vector vector = new Vector(rect.TopLeft.X, rect.TopLeft.Y);
			startPoint += vector;
			point += vector;
			point3 += vector;
			point4 += vector;
			point5 += vector;
			point6 += vector;
			point7 += vector;
			point8 += vector;
			ctx.BeginFigure(startPoint, true, true);
			ctx.LineTo(point, true, false);
			double num5 = rect.TopRight.X - point.X;
			double num6 = point3.Y - rect.TopRight.Y;
			if (!DoubleUtil.IsZero(num5) || !DoubleUtil.IsZero(num6)) {
				ctx.ArcTo(point3, new Size(num5, num6), 0.0, false, SweepDirection.Clockwise, true, false);
			}
			ctx.LineTo(point4, true, false);
			num5 = rect.BottomRight.X - point5.X;
			num6 = rect.BottomRight.Y - point4.Y;
			if (!DoubleUtil.IsZero(num5) || !DoubleUtil.IsZero(num6)) {
				ctx.ArcTo(point5, new Size(num5, num6), 0.0, false, SweepDirection.Clockwise, true, false);
			}
			ctx.LineTo(point6, true, false);
			num5 = point6.X - rect.BottomLeft.X;
			num6 = rect.BottomLeft.Y - point7.Y;
			if (!DoubleUtil.IsZero(num5) || !DoubleUtil.IsZero(num6)) {
				ctx.ArcTo(point7, new Size(num5, num6), 0.0, false, SweepDirection.Clockwise, true, false);
			}
			ctx.LineTo(point8, true, false);
			num5 = startPoint.X - rect.TopLeft.X;
			num6 = point8.Y - rect.TopLeft.Y;
			if (!DoubleUtil.IsZero(num5) || !DoubleUtil.IsZero(num6)) {
				ctx.ArcTo(startPoint, new Size(num5, num6), 0.0, false, SweepDirection.Clockwise, true, false);
			}
		}
		protected override void OnRender(DrawingContext dc) {
			if (this._useComplexRenderCodePath) {
				Brush brush;
				StreamGeometry borderGeometryCache = this.BorderGeometryCache;
				if ((borderGeometryCache != null) && ((brush = this.BorderBrush) != null)) {
					dc.DrawGeometry(brush, null, borderGeometryCache);
				}
				StreamGeometry backgroundGeometryCache = this.BackgroundGeometryCache;
				if ((backgroundGeometryCache != null) && ((brush = this.Background) != null)) {
					dc.DrawGeometry(brush, null, backgroundGeometryCache);
				}
			}
			else {
				Brush brush2;
				Thickness borderThickness = this.BorderThickness;
				CornerRadius cornerRadius = this.CornerRadius;
				double topLeft = cornerRadius.TopLeft;
				bool flag2 = !DoubleUtil.IsZero(topLeft);
				if (!IsZero(borderThickness) && ((brush2 = this.BorderBrush) != null)) {
					double num2;
					Pen leftPenCache = this.LeftPenCache;
					if (leftPenCache == null) {
						leftPenCache = new Pen();
						leftPenCache.Brush = brush2;
						leftPenCache.Thickness = borderThickness.Left;
						if (brush2.IsFrozen) {
							leftPenCache.Freeze();
						}
						this.LeftPenCache = leftPenCache;
					}
					if (IsUniform(borderThickness)) {
						num2 = leftPenCache.Thickness * 0.5;
						Rect rectangle = new Rect(new Point(num2, num2), new Point(base.RenderSize.Width - num2, base.RenderSize.Height - num2));
						if (flag2) {
							dc.DrawRoundedRectangle(null, leftPenCache, rectangle, topLeft, topLeft);
						}
						else {
							dc.DrawRectangle(null, leftPenCache, rectangle);
						}
					}
					else {
						if (DoubleUtil.GreaterThan(borderThickness.Left, 0.0)) {
							num2 = leftPenCache.Thickness * 0.5;
							dc.DrawLine(leftPenCache, new Point(num2, 0.0), new Point(num2, base.RenderSize.Height));
						}
						if (DoubleUtil.GreaterThan(borderThickness.Right, 0.0)) {
							leftPenCache = this.RightPenCache;
							if (leftPenCache == null) {
								leftPenCache = new Pen();
								leftPenCache.Brush = brush2;
								leftPenCache.Thickness = borderThickness.Right;
								if (brush2.IsFrozen) {
									leftPenCache.Freeze();
								}
								this.RightPenCache = leftPenCache;
							}
							num2 = leftPenCache.Thickness * 0.5;
							dc.DrawLine(leftPenCache, new Point(base.RenderSize.Width - num2, 0.0), new Point(base.RenderSize.Width - num2, base.RenderSize.Height));
						}
						if (DoubleUtil.GreaterThan(borderThickness.Top, 0.0)) {
							leftPenCache = this.TopPenCache;
							if (leftPenCache == null) {
								leftPenCache = new Pen();
								leftPenCache.Brush = brush2;
								leftPenCache.Thickness = borderThickness.Top;
								if (brush2.IsFrozen) {
									leftPenCache.Freeze();
								}
								this.TopPenCache = leftPenCache;
							}
							num2 = leftPenCache.Thickness * 0.5;
							dc.DrawLine(leftPenCache, new Point(0.0, num2), new Point(base.RenderSize.Width, num2));
						}
						if (DoubleUtil.GreaterThan(borderThickness.Bottom, 0.0)) {
							leftPenCache = this.BottomPenCache;
							if (leftPenCache == null) {
								leftPenCache = new Pen();
								leftPenCache.Brush = brush2;
								leftPenCache.Thickness = borderThickness.Bottom;
								if (brush2.IsFrozen) {
									leftPenCache.Freeze();
								}
								this.BottomPenCache = leftPenCache;
							}
							num2 = leftPenCache.Thickness * 0.5;
							dc.DrawLine(leftPenCache, new Point(0.0, base.RenderSize.Height - num2), new Point(base.RenderSize.Width, base.RenderSize.Height - num2));
						}
					}
				}
				Brush background = this.Background;
				if (background != null) {
					Point point;
					Point point2;
					point = new Point(borderThickness.Left, borderThickness.Top);
					point2 = new Point(base.RenderSize.Width - borderThickness.Right, base.RenderSize.Height - borderThickness.Bottom);
					if ((point2.X > point.X) && (point2.Y > point.Y)) {
						if (flag2) {
							Radii radii = new Radii(cornerRadius, borderThickness, false);
							double radiusX = radii.TopLeft;
							dc.DrawRoundedRectangle(background, null, new Rect(point, point2), radiusX, radiusX);
						}
						else {
							dc.DrawRectangle(background, null, new Rect(point, point2));
						}
					}
				}
			}
		}
	}
#endif
	public class AppointmentRoundedBorder : RoundedBorderControl {
		#region ViewInfo
		public VisualAppointmentViewInfo ViewInfo {
			get { return (VisualAppointmentViewInfo)GetValue(ViewInfoProperty); }
			set { SetValue(ViewInfoProperty, value); }
		}
		public static readonly DependencyProperty ViewInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentRoundedBorder, VisualAppointmentViewInfo>("ViewInfo", null, (d, e) => d.OnViewInfoChanged(e.OldValue, e.NewValue), null);
		void OnViewInfoChanged(VisualAppointmentViewInfo oldValue, VisualAppointmentViewInfo newValue) {
			if(newValue != null)
				newValue.PropertiesChanged += OnViewInfoPropertiesChanged;
			if(oldValue != null)
				oldValue.PropertiesChanged -= OnViewInfoPropertiesChanged;
			Update();
		}
		#endregion
		#region DefaultCornerRadius
		public CornerRadius DefaultCornerRadius {
			get { return (CornerRadius)GetValue(DefaultCornerRadiusProperty); }
			set { SetValue(DefaultCornerRadiusProperty, value); }
		}
		public static readonly DependencyProperty DefaultCornerRadiusProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentRoundedBorder, CornerRadius>("DefaultCornerRadius", new CornerRadius(0), (d, e) => d.OnDefaultCornerRadiusChanged(e.OldValue, e.NewValue), null);
		void OnDefaultCornerRadiusChanged(CornerRadius oldValue, CornerRadius newValue) {
			UpdateCornerRaius();
		}
		#endregion
		#region DefaultMargin
		public Thickness DefaultMargin {
			get { return (Thickness)GetValue(DefaultMarginProperty); }
			set { SetValue(DefaultMarginProperty, value); }
		}
		public static readonly DependencyProperty DefaultMarginProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentRoundedBorder, Thickness>("DefaultMargin", new Thickness(0), (d, e) => d.OnDefaultMarginChanged(e.OldValue, e.NewValue), null);
		void OnDefaultMarginChanged(Thickness oldValue, Thickness newValue) {
			UpdateMargin();
		}
		#endregion
		#region NoBorderMargin
		public Thickness NoBorderMargin {
			get { return (Thickness)GetValue(NoBorderMarginProperty); }
			set { SetValue(NoBorderMarginProperty, value); }
		}
		public static readonly DependencyProperty NoBorderMarginProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentRoundedBorder, Thickness>("NoBorderMargin", new Thickness(0), (d, e) => d.OnNoBorderMarginChanged(e.OldValue, e.NewValue), null);
		void OnNoBorderMarginChanged(Thickness oldValue, Thickness newValue) {
			UpdateMargin();
		}
		#endregion
		#region DefaultRoundBorderThickness
		public Thickness DefaultRoundBorderThickness {
			get { return (Thickness)GetValue(DefaultRoundBorderThicknessProperty); }
			set { SetValue(DefaultRoundBorderThicknessProperty, value); }
		}
		public static readonly DependencyProperty DefaultRoundBorderThicknessProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentRoundedBorder, Thickness>("DefaultRoundBorderThickness", new Thickness(0), (d, e) => d.OnDefaultRoundBorderThicknessChanged(e.OldValue, e.NewValue), null);
		void OnDefaultRoundBorderThicknessChanged(Thickness oldValue, Thickness newValue) {
			UpdateRoundBorderThickness();
		}	   
		#endregion
		protected virtual void OnViewInfoPropertiesChanged(object sender, EventArgs e) {
			Update();
		}
		protected virtual void Update() {
			UpdateCornerRaius();
			UpdateRoundBorderThickness();
		}
		protected virtual void UpdateCornerRaius() {
			if(ViewInfo == null)
				return;
			CornerRadius cornerRadius = DefaultCornerRadius;
			double topLeft = (ViewInfo.HasLeftBorder && ViewInfo.HasTopBorder) ? cornerRadius.TopLeft : 0;
			double topRight = (ViewInfo.HasRightBorder && ViewInfo.HasTopBorder) ? cornerRadius.TopRight : 0;
			double bottomLeft = (ViewInfo.HasLeftBorder && ViewInfo.HasBottomBorder) ? cornerRadius.BottomLeft : 0;
			double bottomRight = (ViewInfo.HasRightBorder && ViewInfo.HasBottomBorder) ? cornerRadius.BottomRight : 0;
			CornerRadius = new CornerRadius(topLeft, topRight, bottomRight, bottomLeft);
		}
		protected virtual void UpdateMargin() {
			if(ViewInfo == null)
				return;
			Thickness margin = DefaultMargin;
			Thickness noBorderMargin = NoBorderMargin;
			double left = ViewInfo.HasLeftBorder ? margin.Left : noBorderMargin.Left;
			double right = ViewInfo.HasRightBorder ? margin.Right : noBorderMargin.Right;
			double top = ViewInfo.HasTopBorder ? margin.Top : noBorderMargin.Top;
			double bottom = ViewInfo.HasBottomBorder ? margin.Bottom : noBorderMargin.Bottom;
			Margin = new Thickness(left, top, right, bottom);
		}
		protected void UpdateRoundBorderThickness() {
			if(ViewInfo == null)
				return;
			Thickness borderThickness = DefaultRoundBorderThickness;
			double left = ViewInfo.HasLeftBorder ? borderThickness.Left : 0;
			double right = ViewInfo.HasRightBorder ? borderThickness.Right : 0;
			double top = ViewInfo.HasTopBorder ? borderThickness.Top : 0;
			double bottom = ViewInfo.HasBottomBorder ? borderThickness.Bottom : 0;
			RoundBorderThickness = new Thickness(left, top, right, bottom);
		}
	}
}
