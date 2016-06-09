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
namespace DevExpress.Xpf.Gauges.Native {
	public class ArcScaleLayout : ScaleLayout {
		readonly double ellipseHeight;
		readonly double ellipseWidth;
		readonly Point ellipseCenter;
		public double EllipseHeight { get { return ellipseHeight; } }
		public double EllipseWidth { get { return ellipseWidth; } }
		public Point EllipseCenter { get { return ellipseCenter; } }
		public override bool IsEmpty { get { return !(ellipseHeight > 0 && ellipseWidth > 0); } }
		public override Geometry Clip {
			get { return IsEmpty ? null : new EllipseGeometry() { Center = EllipseCenter, RadiusX = 0.5 * EllipseWidth, RadiusY = 0.5 * EllipseHeight }; }
		}
		public ArcScaleLayout(Rect initialBounds, Point ellipseCenter, double ellipseWidth, double ellipseHeight)
			: base(initialBounds) {
			this.ellipseCenter = ellipseCenter;
			this.ellipseWidth = ellipseWidth;
			this.ellipseHeight = ellipseHeight;
		}
	}
	public class ArcScaleMapping : ScaleMapping {
		public double AnglesRange { get { return Scale.EndAngle - Scale.StartAngle; } }		
		new public ArcScale Scale { get { return base.Scale as ArcScale; } }
		new public ArcScaleLayout Layout { get { return base.Layout as ArcScaleLayout; } }		
		public ArcScaleMapping(ArcScale scale, Rect bounds) : base(scale, new ArcScaleLayoutCalculator(scale, bounds).Calculate()) {			
		}
		double GetSecondaryAlpha(double primaryAlpha, double offset) {
			double primaryA = Layout.EllipseWidth / 2;
			double primaryB = Layout.EllipseHeight / 2;
			return Math.Atan2((primaryB * (primaryA + offset) * Math.Sin(primaryAlpha)), (primaryA * (primaryB + offset) * Math.Cos(primaryAlpha)));
		}
		protected override double GetAngleByPoint(Point point) {
			return MathUtils.Radian2Degree(MathUtils.CalculateBetweenPointsAngle(Layout.EllipseCenter, point));
		}
		public double GetAlphaByPercent(double percent) {
			return MathUtils.Degree2Radian(Scale.StartAngle + percent * AnglesRange);
		}
		public Point GetPointByAlpha(double alpha) {
			return MathUtils.CalculateEllipsePoint(Layout.EllipseCenter, Layout.EllipseWidth, Layout.EllipseHeight, alpha);
		}
		public Point GetPointByAlpha(double alpha, double offset) {
			double convertedAlpha = GetSecondaryAlpha(alpha, offset);
			return MathUtils.CalculateEllipsePoint(Layout.EllipseCenter, Layout.EllipseWidth + 2 * offset, Layout.EllipseHeight + 2 * offset, convertedAlpha);
		}
		public override Point GetPointByPercent(double percent) {
			return GetPointByAlpha(GetAlphaByPercent(percent));
		}
		public override Point GetPointByPercent(double percent, double offset) {
			return GetPointByAlpha(GetAlphaByPercent(percent), offset);
		}		
		public override double? GetValueByPoint(Point point) {
			if (Layout.IsEmpty || point == Layout.EllipseCenter)
				return null;
			double x = point.X - Layout.EllipseCenter.X;
			double y = point.Y - Layout.EllipseCenter.Y;
			SweepDirection direction = Scale.StartAngle < Scale.EndAngle ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
			double alpha = MathUtils.Radian2Degree(MathUtils.NormalizeRadian(Math.Atan2(0.5 * Layout.EllipseWidth * y, 0.5 * Layout.EllipseHeight * x)));
			double startAlpha = MathUtils.NormalizeDegree(Scale.StartAngle);
			double factor;
			if (direction == SweepDirection.Clockwise)
				factor = alpha > startAlpha ? (alpha - startAlpha) / (Scale.EndAngle - Scale.StartAngle) :
												(360 - startAlpha + alpha) / (Scale.EndAngle - Scale.StartAngle);
			else
				factor = alpha > startAlpha ? (360 - alpha + startAlpha) / (Scale.StartAngle - Scale.EndAngle) :
											  (startAlpha - alpha) / (Scale.StartAngle - Scale.EndAngle);
			double result = Scale.StartValue + factor * (Scale.EndValue - Scale.StartValue);
			if (MathUtils.IsValueInRange(result, Scale.StartValue, Scale.EndValue)) 
				return result;
			return null;
		}
	}
	public static class ArcSegmentCalculator {
		static Geometry CalculateClipGeometry(ElementInfoBase elmentInfo, ArcScaleMapping mapping, double startValue, double endValue, double offset, int thickness) {
			double startAngle = MathUtils.Radian2Degree(mapping.GetAlphaByPercent(mapping.Scale.GetValueInPercent(startValue)));
			double sweepAngle = MathUtils.Radian2Degree(mapping.GetAlphaByPercent(mapping.Scale.GetValueInPercent(endValue))) - startAngle;
			Geometry clipGeometry = CalculateGeometry(mapping, startAngle, sweepAngle, offset, thickness);
			clipGeometry.Transform = new TranslateTransform() { X = elmentInfo.Layout.Width.Value / 2 - mapping.Layout.EllipseCenter.X, Y = elmentInfo.Layout.Height.Value / 2 - mapping.Layout.EllipseCenter.Y };
			return clipGeometry;
		}
		public static ElementLayout CreateRangeLayout(ArcScaleMapping mapping, double offset, int thickness) {
			double width = Math.Max(mapping.Layout.EllipseWidth + offset * 2 + thickness, 0);
			double height = Math.Max(mapping.Layout.EllipseHeight + offset * 2 + thickness, 0);
			return new ElementLayout(width, height);
		}
		public static void CompleteRangeLayout(ElementInfoBase elementInfo, ArcScale scale, double startValue, double endValue, double offset, int thickness) {
			Point arrangePoint = scale.Mapping.Layout.EllipseCenter;
			Point layoutOffset = scale.GetLayoutOffset();
			arrangePoint.X += layoutOffset.X;
			arrangePoint.Y += layoutOffset.Y;
			elementInfo.Layout.CompleteLayout(arrangePoint, null, CalculateClipGeometry(elementInfo, scale.Mapping, startValue, endValue, offset, thickness));
		}
		public static Geometry CalculateGeometry(ArcScaleMapping mapping, double startAlpha, double sweepAngle, double offset, int thickness) {
			Geometry geometry = null;
			double outerArcWidth = Math.Max(0.5 * mapping.Layout.EllipseWidth + offset + Math.Floor(thickness / 2.0), 0);
			double outerArcHeight = Math.Max(0.5 * mapping.Layout.EllipseHeight + offset + Math.Floor(thickness / 2.0), 0);
			double innerArcWidth = Math.Max(0.5 * mapping.Layout.EllipseWidth + offset - Math.Ceiling(thickness / 2.0), 0);
			double innerArcHeight = Math.Max(0.5 * mapping.Layout.EllipseHeight + offset - Math.Ceiling(thickness / 2.0), 0);
			if (Math.Abs(sweepAngle) < 360) {
				Point outerStartPoint = mapping.GetPointByAlpha(MathUtils.Degree2Radian(startAlpha), offset + Math.Floor(thickness / 2.0));
				Point outerEndPoint = mapping.GetPointByAlpha(MathUtils.Degree2Radian(startAlpha + sweepAngle), offset + Math.Floor(thickness / 2.0));
				Point innerStartPoint = mapping.GetPointByAlpha(MathUtils.Degree2Radian(startAlpha), offset - Math.Ceiling(thickness / 2.0));
				Point innerEndPoint = mapping.GetPointByAlpha(MathUtils.Degree2Radian(startAlpha + sweepAngle), offset - Math.Ceiling(thickness / 2.0));
				PathFigure figure = new PathFigure() { StartPoint = innerStartPoint };
				figure.Segments.Add(new LineSegment() { Point = outerStartPoint });
				ArcSegment outerArcSegment = new ArcSegment();
				outerArcSegment.Point = outerEndPoint;
				outerArcSegment.Size = new Size(outerArcWidth, outerArcHeight);
				outerArcSegment.IsLargeArc = sweepAngle > 180 || sweepAngle < -180;
				outerArcSegment.SweepDirection = sweepAngle > 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
				figure.Segments.Add(outerArcSegment);
				figure.Segments.Add(new LineSegment() { Point = innerEndPoint });
				ArcSegment innerArcSegment = new ArcSegment();
				innerArcSegment.Point = innerStartPoint;
				innerArcSegment.Size = new Size(innerArcWidth, innerArcHeight);
				innerArcSegment.IsLargeArc = sweepAngle > 180 || sweepAngle < -180;
				innerArcSegment.SweepDirection = sweepAngle < 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
				figure.Segments.Add(innerArcSegment);
				geometry = new PathGeometry();
				((PathGeometry)geometry).Figures.Add(figure);
			}
			else {
				geometry = new GeometryGroup() { FillRule = FillRule.EvenOdd };
				((GeometryGroup)geometry).Children.Add(new EllipseGeometry() { Center = mapping.Layout.EllipseCenter, RadiusX = innerArcWidth, RadiusY = innerArcHeight });
				((GeometryGroup)geometry).Children.Add(new EllipseGeometry() { Center = mapping.Layout.EllipseCenter, RadiusX = outerArcWidth, RadiusY = outerArcHeight });
			};
			return geometry;
		}
	}
}
