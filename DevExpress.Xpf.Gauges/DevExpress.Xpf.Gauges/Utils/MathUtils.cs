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
namespace DevExpress.Xpf.Gauges.Native {
	public static class MathUtils {
		public static bool IsValueInRange(double value, double edge1, double edge2) {
			return value >= Math.Min(edge1, edge2) && value <= Math.Max(edge1, edge2);
		}
		public static double StrongRound(double value) {
			return Math.Sign(value) * (double)(int)(Math.Abs(value) + 0.5);
		}
		public static Point StrongRound(Point value) {
			return new Point(StrongRound(value.X), StrongRound(value.Y));
		}
		public static double Degree2Radian(double angleDegree) {
			return angleDegree * Math.PI / 180.0;
		}
		public static double Radian2Degree(double angleRadian) {
			return angleRadian * 180 / Math.PI;
		}
		public static double NormalizeRadian(double angleRadian) {
			int count = (int)(0.5 * angleRadian / Math.PI);
			if (angleRadian >= 0)
				return angleRadian - count * Math.PI * 2;
			else
				return Math.PI * 2 + angleRadian - count * Math.PI * 2;
		}
		public static double NormalizeDegree(double angleDegree) {
			double angleRadian = NormalizeRadian(Degree2Radian(angleDegree));
			return Radian2Degree(angleRadian);
		}
		public static Point CalculateCenter(Rect rect) {
			return new Point(0.5 * (rect.Left + rect.Right), 0.5 * (rect.Top + rect.Bottom));
		}
		public static double CalculateBetweenPointsAngle(Point p1, Point p2) {
			double angle = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
			return NormalizeRadian(angle);
		}
		public static Point CorrectLocationByTransformOrigin(Point location, Point transformOrigin, Size size) {
			return new Point(location.X - transformOrigin.X * size.Width, location.Y - transformOrigin.Y * size.Height);
		}
		public static double CalculateDistance(Point startPoint, Point endPoint) {
			return Math.Sqrt(Math.Pow((endPoint.X - startPoint.X),2) + Math.Pow((endPoint.Y - startPoint.Y), 2));
		}
		public static Point CalculateEllipsePoint(Point ellipseCenter, double ellipseWidth, double ellipseHeight, double alpha) {
			return ellipseWidth > 0 && ellipseHeight > 0 ? new Point(ellipseCenter.X + 0.5 * ellipseWidth * Math.Cos(alpha), ellipseCenter.Y + 0.5 * ellipseHeight * Math.Sin(alpha)) : ellipseCenter;
		}
	}	
}
