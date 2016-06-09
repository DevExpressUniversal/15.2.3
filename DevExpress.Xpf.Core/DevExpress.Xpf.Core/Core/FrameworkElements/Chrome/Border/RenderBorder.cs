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
using System.Windows;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Core.Native {
	public class RenderBorder : RenderDecorator {
		struct Radii {
			internal readonly double LeftTop;
			internal readonly double TopLeft;
			internal readonly double TopRight;
			internal readonly double RightTop;
			internal readonly double RightBottom;
			internal readonly double BottomRight;
			internal readonly double BottomLeft;
			internal readonly double LeftBottom;
			internal Radii(CornerRadius radii, Thickness borders, bool outer) {
				double num1 = 0.5 * borders.Left;
				double num2 = 0.5 * borders.Top;
				double num3 = 0.5 * borders.Right;
				double num4 = 0.5 * borders.Bottom;
				if (outer) {
					if (radii.TopLeft.IsZero()) {
						LeftTop = TopLeft = 0.0;
					}
					else {
						LeftTop = radii.TopLeft + num1;
						TopLeft = radii.TopLeft + num2;
					}
					if (radii.TopRight.IsZero()) {
						TopRight = RightTop = 0.0;
					}
					else {
						TopRight = radii.TopRight + num2;
						RightTop = radii.TopRight + num3;
					}
					if (radii.BottomRight.IsZero()) {
						RightBottom = BottomRight = 0.0;
					}
					else {
						RightBottom = radii.BottomRight + num3;
						BottomRight = radii.BottomRight + num4;
					}
					if (radii.BottomLeft.IsZero()) {
						BottomLeft = LeftBottom = 0.0;
					}
					else {
						BottomLeft = radii.BottomLeft + num4;
						LeftBottom = radii.BottomLeft + num1;
					}
				}
				else {
					LeftTop = Math.Max(0.0, radii.TopLeft - num1);
					TopLeft = Math.Max(0.0, radii.TopLeft - num2);
					TopRight = Math.Max(0.0, radii.TopRight - num2);
					RightTop = Math.Max(0.0, radii.TopRight - num3);
					RightBottom = Math.Max(0.0, radii.BottomRight - num3);
					BottomRight = Math.Max(0.0, radii.BottomRight - num4);
					BottomLeft = Math.Max(0.0, radii.BottomLeft - num4);
					LeftBottom = Math.Max(0.0, radii.BottomLeft - num1);
				}
			}
		}
		Brush background;
		Brush borderBrush;
		Thickness borderThickness;
		Thickness padding;
		CornerRadius cornerRadius;
		public Brush Background {
			get { return background; }
			set { SetProperty(ref background, value.With(x => x.GetCurrentValueAsFrozen()) as Brush); }
		}
		public Brush BorderBrush {
			get { return borderBrush; }
			set { SetProperty(ref borderBrush, value.With(x => x.GetCurrentValueAsFrozen()) as Brush); }
		}
		public Thickness BorderThickness {
			get { return borderThickness; }
			set { SetProperty(ref borderThickness, value); }
		}
		public Thickness Padding {
			get { return padding; }
			set { SetProperty(ref padding, value); }
		}
		public CornerRadius CornerRadius {
			get { return cornerRadius; }
			set { SetProperty(ref cornerRadius, value); }
		}
		public RenderBorder() {
		}
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderBorderContext(this);
		}
		protected override Size MeasureOverride(Size availableSize, IFrameworkRenderElementContext context) {
			var borderContext = (RenderBorderContext)context;
			var child = borderContext.Child;
			Thickness borderThickness = CalcDpiAwareThickness(borderContext, borderContext.BorderThickness ?? BorderThickness);
			Thickness padding = CalcDpiAwareThickness(borderContext, borderContext.Padding ?? Padding);
			double leftRightBorderThickness = borderThickness.Left + borderThickness.Right;
			double topBottomBorderThickness = borderThickness.Top + borderThickness.Bottom;
			double leftRightPadding = padding.Left + padding.Right;
			double topBottomPadding = padding.Top + padding.Bottom;
			Size size;
			if (child != null) {
				Size constraintSize = new Size(leftRightBorderThickness + leftRightPadding, topBottomBorderThickness + topBottomPadding);
				Size childAvailableSize = new Size(Math.Max(0.0, availableSize.Width - constraintSize.Width), Math.Max(0.0, availableSize.Height - constraintSize.Height));
				child.Measure(childAvailableSize);
				Size desiredSize = child.DesiredSize;
				size = new Size(desiredSize.Width + constraintSize.Width, desiredSize.Height + constraintSize.Height);
			}
			else
				size = new Size(leftRightBorderThickness + leftRightPadding, topBottomBorderThickness + topBottomPadding);
			return size;
		}
		protected override Size ArrangeOverride(Size finalSize, IFrameworkRenderElementContext context) {
			Rect boundRect = new Rect(finalSize);
			var borderContext = (RenderBorderContext)context;
			Thickness borderThickness = CalcDpiAwareThickness(borderContext, borderContext.BorderThickness ?? BorderThickness);
			Thickness padding = CalcDpiAwareThickness(borderContext, borderContext.Padding ?? Padding);
			CornerRadius cornerRadius = borderContext.CornerRadius ?? CornerRadius;
			Brush background = borderContext.Background ?? Background;
			Brush borderBrush = borderContext.BorderBrush ?? BorderBrush;
			bool useLayoutRounding = ((FrameworkRenderElementContext)context).UseLayoutRounding;
			Rect innerRect = DeflateRect(boundRect, borderThickness);
			var child = borderContext.Child;
			if (child != null) {
				Rect finalRect = DeflateRect(innerRect, padding);
				child.Arrange(finalRect);
			}
			borderContext.UseComplexRenderCodePath = !AreUniformCorners(cornerRadius);
			if (!borderContext.UseComplexRenderCodePath && borderBrush != null) {
				SolidColorBrush solidColorBrush = borderBrush as SolidColorBrush;
				bool isUniform = IsUniformThickness(borderThickness);
				borderContext.UseComplexRenderCodePath = solidColorBrush == null ||
					!isUniform && (solidColorBrush.Color.A < byte.MaxValue || CornerRadius.TopLeft.AreClose(0));
			}
			if (borderContext.UseComplexRenderCodePath) {
				Radii radii1 = new Radii(cornerRadius, borderThickness, false);
				StreamGeometry backgroundGeometry = null;
				if (!innerRect.Width.IsZero() && !innerRect.Height.IsZero()) {
					backgroundGeometry = new StreamGeometry();
					using (StreamGeometryContext ctx = backgroundGeometry.Open())
						GenerateGeometry(ctx, innerRect, radii1);
					backgroundGeometry.Freeze();
					borderContext.BackgroundGeometryCache = backgroundGeometry;
				}
				else
					borderContext.BackgroundGeometryCache = null;
				if (!boundRect.Width.IsZero() && !boundRect.Height.IsZero()) {
					Radii radii2 = new Radii(cornerRadius, borderThickness, true);
					StreamGeometry borderGeometry = new StreamGeometry();
					using (StreamGeometryContext ctx = borderGeometry.Open()) {
						GenerateGeometry(ctx, boundRect, radii2);
						if (backgroundGeometry != null)
							GenerateGeometry(ctx, innerRect, radii1);
					}
					borderGeometry.Freeze();
					borderContext.BorderGeometryCache = borderGeometry;
				}
				else
					borderContext.BorderGeometryCache = null;
			}
			else {
				borderContext.BackgroundGeometryCache = null;
				borderContext.BorderGeometryCache = null;
			}
			return finalSize;
		}
		protected void DrawGeometry(RenderBorderContext context, DrawingContext dc, Brush brush, Pen pen, Geometry geometry) {
			dc.DrawGeometry(brush, pen, geometry);
		}
		protected void DrawRectangle(RenderBorderContext context, DrawingContext dc, Brush brush, Pen pen, Rect rectangle) {
			dc.DrawRectangle(brush, pen, rectangle);
		}
		protected void DrawRoundedRectangle(RenderBorderContext context, DrawingContext dc, Brush brush, Pen pen, Rect rectangle, double radiusX, double radiusY) {
			dc.DrawRoundedRectangle(brush, pen, rectangle, radiusX, radiusY);
		}
		protected void DrawLine(RenderBorderContext context, DrawingContext dc, Pen pen, Point point0, Point point1) {
			dc.DrawLine(pen, point0, point1);
		}
		protected override void RenderOverride(DrawingContext dc, IFrameworkRenderElementContext context) {
			base.RenderOverride(dc, context);
			bool useLayoutRounding = ((FrameworkRenderElementContext)context).UseLayoutRounding;
			var borderContext = (RenderBorderContext)context;
			Brush background = borderContext.Background ?? Background;
			Brush borderBrush = borderContext.BorderBrush ?? BorderBrush;
			if (borderContext.UseComplexRenderCodePath) {
				StreamGeometry borderGeometryCache = borderContext.BorderGeometryCache;
				if (borderGeometryCache != null && borderBrush != null)
					DrawGeometry(borderContext, dc, borderBrush, null, borderGeometryCache);
				StreamGeometry backgroundGeometryCache = borderContext.BackgroundGeometryCache;
				if (backgroundGeometryCache == null || background == null)
					return;
				DrawGeometry(borderContext, dc, background, null, backgroundGeometryCache);
			}
			else {
				Thickness borderThickness = CalcDpiAwareThickness(borderContext, borderContext.BorderThickness ?? BorderThickness);
				CornerRadius cornerRadius = borderContext.CornerRadius ?? CornerRadius;
				double outerCornerRadius = cornerRadius.TopLeft;
				bool roundedCorners = !outerCornerRadius.IsZero();
				if (!IsZeroThickness(borderThickness) && borderBrush != null) {
					Pen pen = borderContext.LeftPenCache;
					if (pen == null) {
						pen = new Pen();
						pen.Brush = borderBrush;
						pen.Thickness = !useLayoutRounding ? borderThickness.Right : RoundLayoutValue(borderThickness.Right, DpiScaleX);
						if (borderBrush.IsFrozen)
							pen.Freeze();
						borderContext.LeftPenCache = pen;
					}
					double halfThickness;
					if (IsUniformThickness(borderThickness)) {
						halfThickness = pen.Thickness * 0.5;
						Rect rect = new Rect(new Point(halfThickness, halfThickness), new Point(context.RenderSize.Width - halfThickness, context.RenderSize.Height - halfThickness));
						if (roundedCorners)
							dc.DrawRoundedRectangle(null, pen, rect, outerCornerRadius, outerCornerRadius);
						else
							dc.DrawRectangle(null, pen, rect);
					}
					else {
						if (borderThickness.Left.GreaterThan(0d)) {
							halfThickness = pen.Thickness * 0.5;
							dc.DrawLine(pen, new Point(halfThickness, 0), new Point(halfThickness, borderContext.RenderSize.Height));
						}
						if (borderThickness.Right.GreaterThan(0d)) {
							pen = borderContext.RightPenCache;
							if (pen == null) {
								pen = new Pen();
								pen.Brush = borderBrush;
								pen.Thickness = useLayoutRounding ? RoundLayoutValue(borderThickness.Right, ScreenHelper.ScaleX) : borderThickness.Right;
								if (borderBrush.IsFrozen)
									pen.Freeze();
								borderContext.RightPenCache = pen;
							}
							halfThickness = pen.Thickness * 0.5;
							dc.DrawLine(pen, new Point(borderContext.RenderSize.Width - halfThickness, 0), new Point(borderContext.RenderSize.Width - halfThickness, borderContext.RenderSize.Height));
						}
						if (borderThickness.Top.GreaterThan(0d)) {
							pen = borderContext.TopPenCache;
							if (pen == null) {
								pen = new Pen();
								pen.Brush = borderBrush;
								pen.Thickness = useLayoutRounding ? RoundLayoutValue(borderThickness.Top, ScreenHelper.ScaleX) : borderThickness.Top;
								if (borderBrush.IsFrozen)
									pen.Freeze();
								borderContext.TopPenCache = pen;
							}
							halfThickness = pen.Thickness * 0.5;
							dc.DrawLine(pen, new Point(0, halfThickness), new Point(borderContext.RenderSize.Width, halfThickness));
						}
						if (borderThickness.Bottom.GreaterThan(0d)) {
							pen = borderContext.BottomPenCache;
							if (pen == null) {
								pen = new Pen();
								pen.Brush = borderBrush;
								pen.Thickness = useLayoutRounding ? RoundLayoutValue(borderThickness.Bottom, ScreenHelper.ScaleX) : borderThickness.Bottom;
								if (borderBrush.IsFrozen)
									pen.Freeze();
								borderContext.BottomPenCache = pen;
							}
							halfThickness = pen.Thickness * 0.5;
							dc.DrawLine(pen, new Point(0, borderContext.RenderSize.Height - halfThickness), new Point(borderContext.RenderSize.Width, borderContext.RenderSize.Height - halfThickness));
						}
					}
				}
				if (background != null) {
					Point ptTL, ptBR;
					if (useLayoutRounding) {
						ptTL = new Point(RoundLayoutValue(borderThickness.Left, ScreenHelper.ScaleX), RoundLayoutValue(borderThickness.Top, ScreenHelper.ScaleX));
						ptBR = new Point(RoundLayoutValue(borderContext.RenderSize.Width - borderThickness.Right, ScreenHelper.ScaleX), 
							RoundLayoutValue(borderContext.RenderSize.Height - borderThickness.Bottom, ScreenHelper.ScaleX));
					}
					else {
						ptTL = new Point(borderThickness.Left, borderThickness.Top);
						ptBR = new Point(borderContext.RenderSize.Width - borderThickness.Right, borderContext.RenderSize.Height - borderThickness.Bottom);
					}
					if (ptBR.X > ptTL.X && ptBR.Y > ptTL.Y) {
						if (roundedCorners) {
							Radii innerRadii = new Radii(cornerRadius, borderThickness, false); 
							double innerCornerRadius = innerRadii.TopLeft;  
							dc.DrawRoundedRectangle(background, null, new Rect(ptTL, ptBR), innerCornerRadius, innerCornerRadius);
						}
						else {
							dc.DrawRectangle(background, null, new Rect(ptTL, ptBR));
						}
					}
				}
			}
		}
		static bool IsZeroThickness(Thickness th) {
			if (th.Left.IsZero() && th.Top.IsZero() && th.Right.IsZero())
				return th.Bottom.IsZero();
			return false;
		}
		static bool IsUniformThickness(Thickness th) {
			if (th.Left.AreClose(th.Top) && th.Left.AreClose(th.Right))
				return th.Left.AreClose(th.Bottom);
			return false;
		}
		static bool AreUniformCorners(CornerRadius borderRadii) {
			double topLeft = borderRadii.TopLeft;
			if (topLeft.AreClose(borderRadii.TopRight) && topLeft.AreClose(borderRadii.BottomLeft))
				return topLeft.AreClose(borderRadii.BottomRight);
			return false;
		}
		static Rect DeflateRect(Rect rt, Thickness thick) {
			return new Rect(rt.Left + thick.Left, rt.Top + thick.Top, Math.Max(0.0, rt.Width - thick.Left - thick.Right), Math.Max(0.0, rt.Height - thick.Top - thick.Bottom));
		}
		static void GenerateGeometry(StreamGeometryContext ctx, Rect rect, Radii radii) {
			Point point1 = new Point(radii.LeftTop, 0.0);
			Point point2 = new Point(rect.Width - radii.RightTop, 0.0);
			Point point3 = new Point(rect.Width, radii.TopRight);
			Point point4 = new Point(rect.Width, rect.Height - radii.BottomRight);
			Point point5 = new Point(rect.Width - radii.RightBottom, rect.Height);
			Point point6 = new Point(radii.LeftBottom, rect.Height);
			Point point7 = new Point(0.0, rect.Height - radii.BottomLeft);
			Point point8 = new Point(0.0, radii.TopLeft);
			if (point1.X > point2.X) {
				double num = radii.LeftTop / (radii.LeftTop + radii.RightTop) * rect.Width;
				point1.X = num;
				point2.X = num;
			}
			if (point3.Y > point4.Y) {
				double num = radii.TopRight / (radii.TopRight + radii.BottomRight) * rect.Height;
				point3.Y = num;
				point4.Y = num;
			}
			if (point5.X < point6.X) {
				double num = radii.LeftBottom / (radii.LeftBottom + radii.RightBottom) * rect.Width;
				point5.X = num;
				point6.X = num;
			}
			if (point7.Y < point8.Y) {
				double num = radii.TopLeft / (radii.TopLeft + radii.BottomLeft) * rect.Height;
				point7.Y = num;
				point8.Y = num;
			}
			Vector vector = new Vector(rect.TopLeft.X, rect.TopLeft.Y);
			point1 += vector;
			point2 += vector;
			point3 += vector;
			point4 += vector;
			point5 += vector;
			point6 += vector;
			point7 += vector;
			point8 += vector;
			ctx.BeginFigure(point1, true, true);
			ctx.LineTo(point2, true, false);
			double width1 = rect.TopRight.X - point2.X;
			double height1 = point3.Y - rect.TopRight.Y;
			if (!width1.IsZero() || !height1.IsZero())
				ctx.ArcTo(point3, new Size(width1, height1), 0.0, false, SweepDirection.Clockwise, true, false);
			ctx.LineTo(point4, true, false);
			double width2 = rect.BottomRight.X - point5.X;
			double height2 = rect.BottomRight.Y - point4.Y;
			if (!width2.IsZero() || !height2.IsZero())
				ctx.ArcTo(point5, new Size(width2, height2), 0.0, false, SweepDirection.Clockwise, true, false);
			ctx.LineTo(point6, true, false);
			double width3 = point6.X - rect.BottomLeft.X;
			double height3 = rect.BottomLeft.Y - point7.Y;
			if (!width3.IsZero() || !height3.IsZero())
				ctx.ArcTo(point7, new Size(width3, height3), 0.0, false, SweepDirection.Clockwise, true, false);
			ctx.LineTo(point8, true, false);
			double width4 = point1.X - rect.TopLeft.X;
			double height4 = point8.Y - rect.TopLeft.Y;
			if (width4.IsZero() && height4.IsZero())
				return;
			ctx.ArcTo(point1, new Size(width4, height4), 0.0, false, SweepDirection.Clockwise, true, false);
		}
	}
}
