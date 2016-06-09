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
using System.Text;
namespace DevExpress.Charts.Native {
	public enum TextRotation {
			LeftTop,
			CenterTop,
			RightTop,
			LeftCenter,
			CenterCenter,
			RightCenter,
			LeftBottom,
			CenterBottom,
			RightBottom
	};
	public static class AxisLabelRotationHelper {
		static double GetValueHalf(double value, bool useRounding) {
			return (useRounding ? (int)(value / 2) : value / 2);
		}
		static double GetStrongRoundedValue(double value, bool useRounding) {
			return (useRounding ? GeometricUtils.StrongRound(value) : value);
		}
		public static GRealPoint2D CalculateLeftTopPointForBottomPosition(GRealRect2D bounds, double angleDegree, double angleRadian, bool useBoundsRounding) {
			GRealPoint2D basePoint = new GRealPoint2D(bounds.Left, bounds.Top);
			if (angleDegree == 0 || angleDegree == 180)
				return new GRealPoint2D(basePoint.X - GetValueHalf(bounds.Width, useBoundsRounding), basePoint.Y);
			double halfHeight = bounds.Height / 2.0;
			return angleDegree < 180 ?
				new GRealPoint2D(basePoint.X, basePoint.Y - GetStrongRoundedValue(halfHeight * (1 - Math.Abs(Math.Cos(angleRadian))), useBoundsRounding)) :
				new GRealPoint2D(basePoint.X - bounds.Width, basePoint.Y - GetStrongRoundedValue(halfHeight * (1 - Math.Abs(Math.Cos(angleRadian - Math.PI))), useBoundsRounding));
		}
		public static GRealPoint2D CalculateLeftTopPointForLeftPosition(GRealRect2D bounds, double angleDegree, double angleRadian, bool useBoundsRounding) {
			GRealPoint2D basePoint = new GRealPoint2D(bounds.Left, bounds.Top);
			double halfHeight = GetValueHalf(bounds.Height, useBoundsRounding);
			if (angleDegree == 90 || angleDegree == 270)
				return new GRealPoint2D(basePoint.X - GetValueHalf(bounds.Width, useBoundsRounding) - halfHeight, basePoint.Y - halfHeight);
			return (angleDegree < 90 || angleDegree > 270) ?
				new GRealPoint2D(basePoint.X - GetStrongRoundedValue(bounds.Width + Math.Abs(bounds.Height / 2.0 * Math.Sin(angleRadian)), useBoundsRounding), basePoint.Y - halfHeight) :
				new GRealPoint2D(basePoint.X - GetStrongRoundedValue(Math.Abs(bounds.Height / 2.0 * Math.Cos(angleRadian - Math.PI / 2)), useBoundsRounding), basePoint.Y - halfHeight);
		}
		public static GRealPoint2D CalculateLeftTopPointForRightPosition(GRealRect2D bounds, double angleDegree, double angleRadian, bool useBoundsRounding) {
			GRealPoint2D basePoint = new GRealPoint2D(bounds.Left, bounds.Top);
			double halfHeight = GetValueHalf(bounds.Height, useBoundsRounding);
			if (angleDegree == 90 || angleDegree == 270)
				return new GRealPoint2D(basePoint.X - GetValueHalf(bounds.Width, useBoundsRounding) + halfHeight, basePoint.Y - halfHeight);
			return (angleDegree < 90 || angleDegree > 270) ?
				new GRealPoint2D(basePoint.X + GetStrongRoundedValue(Math.Abs(bounds.Height / 2.0 * Math.Sin(angleRadian)), useBoundsRounding), basePoint.Y - halfHeight) :
				new GRealPoint2D(basePoint.X - bounds.Width + GetStrongRoundedValue(Math.Abs(bounds.Height / 2.0 * Math.Cos(angleRadian - Math.PI / 2.0)), useBoundsRounding), basePoint.Y - halfHeight);
		}
		public static GRealPoint2D CalculateLeftTopPointForTopPosition(GRealRect2D bounds, double angleDegree, double angleRadian, bool useBoundsRounding) {
			GRealPoint2D basePoint = new GRealPoint2D(bounds.Left, bounds.Top);
			if (angleDegree == 0 || angleDegree == 180)
				return new GRealPoint2D(basePoint.X - GetValueHalf(bounds.Width, useBoundsRounding), basePoint.Y - bounds.Height);
			double halfHeight = bounds.Height / 2.0;
			return angleDegree < 180 ?
				new GRealPoint2D(basePoint.X - bounds.Width, basePoint.Y - GetStrongRoundedValue(halfHeight + Math.Abs(halfHeight * Math.Cos(angleRadian)), useBoundsRounding)) :
				new GRealPoint2D(basePoint.X, basePoint.Y - GetStrongRoundedValue(halfHeight + Math.Abs(halfHeight * Math.Cos(angleRadian - Math.PI)), useBoundsRounding));
		}
		public static TextRotation CalculateRotationForBottomNearPosition(double degreeAngle) {
			if (degreeAngle == 0 || degreeAngle == 180)
				return TextRotation.CenterCenter;
			return degreeAngle < 180 ? TextRotation.LeftCenter : TextRotation.RightCenter;
		}
		public static TextRotation CalculateRotationForLeftNearPosition(double degreeAngle) {
			if (degreeAngle == 90 || degreeAngle == 270)
				return TextRotation.CenterCenter;
			return (degreeAngle < 90 || degreeAngle > 270) ? TextRotation.RightCenter : TextRotation.LeftCenter;
		}
		public static TextRotation CalculateRotationForRightNearPosition(double degreeAngle) {
			if (degreeAngle == 90 || degreeAngle == 270)
				return TextRotation.CenterCenter;
			return (degreeAngle < 90 || degreeAngle > 270) ? TextRotation.LeftCenter : TextRotation.RightCenter;
		}
		public static TextRotation CalculateRotationForTopNearPosition(double degreeAngle) {
			if (degreeAngle == 0 || degreeAngle == 180)
				return TextRotation.CenterCenter;
			return degreeAngle < 180 ? TextRotation.RightCenter : TextRotation.LeftCenter;
		}
		public static GRealPoint2D[] CalculateRotatedItemPoints(GRealRect2D bounds, double radianAngle) {
			double width = bounds.Width / 2.0;
			double height = bounds.Height / 2.0;
			double sin = Math.Sin(radianAngle);
			double cos = Math.Cos(radianAngle);
			GRealPoint2D[] points = new GRealPoint2D[4];
			points[0] = new GRealPoint2D(bounds.Left + height * sin + width * (1 - cos), bounds.Top + height * (1 - cos) - width * sin);
			points[1] = new GRealPoint2D(bounds.Right + height * sin - width * (1 - cos), bounds.Top + height * (1 - cos) + width * sin);
			points[2] = new GRealPoint2D(bounds.Right - height * sin - width * (1 - cos), bounds.Bottom - height * (1 - cos) + width * sin);
			points[3] = new GRealPoint2D(bounds.Left - height * sin + width * (1 - cos), bounds.Bottom - height * (1 - cos) - width * sin);
			return points;
		}
		public static GRealPoint2D CalculateOffset(TextRotation rotation, GRealRect2D rect, double radianAngle) {
			double width = rect.Width / 2.0;
			double height = rect.Height / 2.0;
			double sin = Math.Sin(radianAngle);
			double cos = Math.Cos(radianAngle);
			switch (rotation) {
				case TextRotation.LeftTop:
					return new GRealPoint2D(-width * (1 - cos) - height * sin, width * sin - height * (1 - cos));
				case TextRotation.CenterTop:
					return new GRealPoint2D(-height * sin, -height * (1 - cos));
				case TextRotation.RightTop:
					return new GRealPoint2D(width * (1 - cos) - height * sin, -width * sin - height * (1 - cos));
				case TextRotation.LeftCenter:
					return new GRealPoint2D(-width * (1 - cos), width * sin);
				case TextRotation.RightCenter:
					return new GRealPoint2D(width * (1 - cos), -width * sin);
				case TextRotation.LeftBottom:
					return new GRealPoint2D(-width * (1 - cos) + height * sin, width * sin + height * (1 - cos));
				case TextRotation.CenterBottom:
					return new GRealPoint2D(height * sin, height * (1 - cos));
				case TextRotation.RightBottom:
					return new GRealPoint2D(width * (1 - cos) + height * sin, -width * sin + height * (1 - cos));
				default:
					return new GRealPoint2D();
			}
		}
		public static LineStrip RotateRectangleOverPoint(GRealRect2D rectangle, GRealPoint2D basePoint, double radianAngle) {
			rectangle.Offset(-basePoint.X, -basePoint.Y);
			double sin = Math.Sin(radianAngle);
			double cos = Math.Cos(radianAngle);
			GRealPoint2D[] points = new GRealPoint2D[4];
			points[0] = new GRealPoint2D(rectangle.Left * cos - rectangle.Top * sin, rectangle.Left * sin + rectangle.Top * cos);
			points[1] = new GRealPoint2D(rectangle.Right * cos - rectangle.Top * sin, rectangle.Right * sin + rectangle.Top * cos);
			points[2] = new GRealPoint2D(rectangle.Right * cos - rectangle.Bottom * sin, rectangle.Right * sin + rectangle.Bottom * cos);
			points[3] = new GRealPoint2D(rectangle.Left * cos - rectangle.Bottom * sin, rectangle.Left * sin + rectangle.Bottom * cos);
			LineStrip strip = new LineStrip(4);
			foreach (GRealPoint2D point in points)
				strip.Add(new GRealPoint2D(point.X + basePoint.X, point.Y + basePoint.Y));
			return strip;
		}
	}
}
