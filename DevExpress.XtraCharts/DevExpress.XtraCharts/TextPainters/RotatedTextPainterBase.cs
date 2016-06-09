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
using System.Drawing;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public abstract class RotatedTextPainterBase : TextPainterBase {
		static LineStrip CalculatePoints(TextRotation rotation, Rectangle rect, double angle) {
			GRealRect2D convertedBounds = new GRealRect2D(rect.Left, rect.Top, rect.Width, rect.Height);
			GRealPoint2D[] points = AxisLabelRotationHelper.CalculateRotatedItemPoints(convertedBounds, angle);
			GRealPoint2D offset = AxisLabelRotationHelper.CalculateOffset(rotation, convertedBounds, angle);
			LineStrip result = new LineStrip(4);
			foreach (GRealPoint2D point in points)
				result.Add(new GRealPoint2D(point.X + offset.X, point.Y + offset.Y));
			return result;
		}
		Point GetOffset(ref Rectangle rect, ref Point center) {
			switch (CalculateRotation()) {
				case TextRotation.LeftTop:
					return new Point(rect.Left, rect.Top);
				case TextRotation.CenterTop:
					return new Point(center.X, rect.Top);
				case TextRotation.RightTop:
					return new Point(rect.Right, rect.Top);
				case TextRotation.LeftCenter:
					return new Point(rect.Left, center.Y);
				case TextRotation.CenterCenter:
					return new Point(center.X, center.Y);
				case TextRotation.RightCenter:
					return new Point(rect.Right, center.Y);
				case TextRotation.LeftBottom:
					return new Point(rect.Left, rect.Bottom);
				case TextRotation.CenterBottom:
					return new Point(center.X, rect.Bottom);
				case TextRotation.RightBottom:
					return new Point(rect.Right, rect.Bottom);
				default:
					return new Point(0, 0);
			}
		}
		public static float CancelAngle(float angle) {
			float result = angle - 360 * ((int)angle / 360);
			if (result < 0)
				result += 360;
			return result;
		}
		public static bool IsTopHalfCircle(float angle) {
			return (angle >= 0 && angle <= 90) || (angle >= 270 && angle <= 360);
		}
		Point basePoint;
		double radianAngle;
		double degreeAngle;
		Rectangle InitialTextRect { get { return new Rectangle(CalculateLeftTopPoint(), new Size(Width, Height)); } }
		internal double TextAngleRadian { get { return radianAngle; } }
		internal double TextAngleDegree { get { return degreeAngle; } }
		public Point BasePoint { get { return basePoint; } }
		public RotatedTextPainterBase(Point basePoint, string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, bool mixColorByHitTestState)
			: this(basePoint, text, textSize, propertiesProvider, mixColorByHitTestState, false, null, 0, 0, false) {
		}
		public RotatedTextPainterBase(Point basePoint, string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, bool mixColorByHitTestState, bool parseText, TextMeasurer textMeasurer, int maxWidth, int maximumLinesCount, bool wordWrap)
			: base(text, textSize, propertiesProvider, mixColorByHitTestState, parseText, textMeasurer, maxWidth, maximumLinesCount, wordWrap) {
			this.basePoint = basePoint;
		}
		public RotatedTextPainterBase(Point basePoint, string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, bool mixColorByHitTestState, TextMeasurer textMeasurer, int maxWidth, int maxHeight)
			: base(text, textSize, propertiesProvider, mixColorByHitTestState, textMeasurer, maxWidth, maxHeight) {
			this.basePoint = basePoint;
		}
		protected override RectangleF GetBounds() {
			double minX = Double.MaxValue, minY = Double.MaxValue, maxX = Double.MinValue, maxY = Double.MinValue;
			foreach (GRealPoint2D point in CalculatePoints(CalculateRotation(), InitialTextRect, radianAngle)) {
				if (point.X < minX)
					minX = point.X;
				if (point.X > maxX)
					maxX = point.X;
				if (point.Y < minY)
					minY = point.Y;
				if (point.Y > maxY)
					maxY = point.Y;
			}
			return new RectangleF((float)minX, (float)minY, (float)(maxX - minX), (float)(maxY - minY));
		}
		protected override VariousPolygon GetPolygon() {
			return new VariousPolygon(CalculatePoints(CalculateRotation(), Rectangle.Inflate(InitialTextRect, 1, 1), radianAngle), GetBounds());
		}
		protected override VariousPolygon CalculateHitPolygon(RectangleF bounds) {
			GRealRect2D convertedBounds = new GRealRect2D(bounds.Left, bounds.Top, bounds.Width, bounds.Height);
			Point leftTop = CalculateLeftTopPoint();
			GRealPoint2D rotationPoint = new GRealPoint2D(leftTop.X + InitialTextRect.Width / 2.0, leftTop.Y + InitialTextRect.Height / 2.0);
			LineStrip strip = AxisLabelRotationHelper.RotateRectangleOverPoint(convertedBounds, rotationPoint, radianAngle);
			return new VariousPolygon(strip, bounds);
		}
		protected abstract Point CalculateLeftTopPoint();
		protected abstract TextRotation CalculateRotation();
		protected void SetTextAngle(double angle) {
			degreeAngle = angle;
			radianAngle = angle * Math.PI / 180.0;
		}
		public override void Render(IRenderer renderer, HitTestController hitTestController, IHitRegion hitRegion, object additionalHitObject, Color color) {
			Rectangle rect = InitialTextRect;
			Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
			Point offset = GetOffset(ref rect, ref center);
			rect.Offset(-offset.X, -offset.Y);
			ProcessHitTesting(renderer, hitTestController, hitRegion, additionalHitObject);
			renderer.SaveState();
			renderer.TranslateModel(offset);
			renderer.RotateModel((float)degreeAngle);
			Shadow shadow = Shadow;
			if (shadow != null)
				shadow.Render(renderer, rect);
			RenderInternal(renderer, hitTestController, rect, color, true);
			renderer.RestoreState();
		}
		public override void RenderWithClipping(IRenderer renderer, HitTestController hitTestController, object additionalHitObject, Color color, Rectangle clipBounds) {
			Rectangle rect = InitialTextRect;
			Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
			Point offset = GetOffset(ref rect, ref center);
			rect.Offset(-offset.X, -offset.Y);
			VariousPolygon polygon = GetPolygon();
			VariousPolygon clippedPolygon = VariousPolygon.Intersect(polygon, clipBounds);
			HitRegion hitRegion = new HitRegion(clippedPolygon);
			ProcessHitTesting(renderer, hitTestController, hitRegion, additionalHitObject);
			renderer.SaveState();
			renderer.SetClipping(clipBounds);
			renderer.TranslateModel(offset);
			renderer.RotateModel((float)degreeAngle);
			Shadow shadow = Shadow;
			if (shadow != null)
				shadow.Render(renderer, rect);
			RenderInternal(renderer, hitTestController, rect, color, true);
			renderer.RestoreClipping();
			renderer.RestoreState();
		}
		public override void Offset(double dx, double dy) {
			basePoint.X += (int)dx;
			basePoint.Y += (int)dy;
			CalculateBounds();
		}
		public override void Rotate(double angle) {
			SetTextAngle(CancelAngle((float)angle));
			CalculateBounds();
		}
	}
}
