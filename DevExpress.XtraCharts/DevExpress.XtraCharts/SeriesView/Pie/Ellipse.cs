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
using DevExpress.Charts.Native;
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Native {
	public class Ellipse : IEllipse {
		static GRealPoint2D Polar2Cartesian(double angle, double radius) {
			return new GRealPoint2D(radius * Math.Cos(angle), -radius * Math.Sin(angle));
		}
		static double AtanMulTan(double multiplier, double tanAngle) {
			double angle = GeometricUtils.NormalizeRadian(tanAngle);
			double result = tanAngle;
			if (ComparingUtils.CompareDoubles(Math.Abs(angle), Math.PI / 2, 1e-5) != 0 && ComparingUtils.CompareDoubles(Math.Abs(angle), Math.PI / 2 * 3, 1e-5) != 0) {
				result = Math.Atan(multiplier * Math.Tan(angle)) + tanAngle - angle;
				if (Math.Abs(angle) > Math.PI / 2 && Math.Abs(angle) < Math.PI / 2 * 3)
					result += Math.Sign(tanAngle) * Math.PI;
				else if (Math.Abs(angle) > Math.PI / 2 * 3)
					result += Math.Sign(tanAngle) * 2 * Math.PI;
			}
			return result;
		}
		GRealPoint2D center;
		double majorSemiaxis;
		double minorSemiaxis;
		double majorMinorRatio;
		double minorMajorRatio;
		double majorMinorProduct;
		double area;
		public GRealPoint2D Center { get { return center; } }
		public double MajorSemiaxis { get { return majorSemiaxis; } }
		public double MinorSemiaxis { get { return minorSemiaxis; } }
		public double Area { get { return area; } }
		public Ellipse(GRealPoint2D center, double majorSemiaxis, double minorSemiaxis) {
			this.center = center;
			this.majorSemiaxis = majorSemiaxis;
			this.minorSemiaxis = minorSemiaxis;
			this.majorMinorRatio = majorSemiaxis / minorSemiaxis;
			this.minorMajorRatio = minorSemiaxis / majorSemiaxis;
			this.majorMinorProduct = majorSemiaxis * minorSemiaxis;
			this.area = Math.PI * this.majorMinorProduct;
		}
		public Ellipse Inflate(double dx, double dy) {
			return new Ellipse(center, majorSemiaxis + dx, minorSemiaxis + dy);
		}
		public LineStrip GetContour() {
			LineStrip contour = new LineStrip();
			int count = 150;
			double angle = 2 * Math.PI;
			double step = angle / count;
			for (int i = 0; i < count; i++) {
				contour.Add(CalcEllipsePoint(angle));
				angle -= step;
			}
			return contour;
		}
		public double CalcEllipseRadius(double angle) {
			return majorMinorProduct / Math.Sqrt(Math.Pow(majorSemiaxis * Math.Sin(angle), 2.0) + Math.Pow(minorSemiaxis * Math.Cos(angle), 2.0));
		}
		public GRealPoint2D CalcEllipsePoint(double angle) {
			GRealPoint2D point = Polar2Cartesian(angle, CalcEllipseRadius(angle));
			return new GRealPoint2D(point.X + center.X, point.Y + center.Y);
		}
		public double CalcEllipseAngleFromCircleAngle(double angle) {
			return AtanMulTan(minorMajorRatio, angle);
		}
		public double CalcEllipseSectorFinishAngle(double areaSector, double startAngle) {
			return AtanMulTan(minorMajorRatio, areaSector * 2.0 / majorMinorProduct + AtanMulTan(majorMinorRatio, startAngle));
		}
		public double CalcEllipseSectorArea(double startAngle, double finishAngle) {
			return majorMinorProduct * (AtanMulTan(majorMinorRatio, finishAngle) - AtanMulTan(majorMinorRatio, startAngle)) / 2.0;
		}
	}
}
