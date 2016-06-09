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
using System.Linq;
using System.Text;
using System.Windows;
namespace DevExpress.XtraDiagram.Utils {
	public static class MathUtils {
		static readonly double Epsilon = 0.00001;
		public static bool IsEquals(double x, double y) {
			return Math.Abs(x - y) < Epsilon;
		}
		public static bool IsNotEquals(double x, double y) {
			return !Equals(x, y);
		}
		public static bool IsGreaterThan(double x, double y) {
			if(IsEquals(x, y)) return false;
			return x > y;
		}
		public static bool IsLessThan(double x, double y) {
			if(IsEquals(x, y)) return false;
			return x < y;
		}
		public static double Coerce(double value, double minValue, double maxValue) {
			if(value < minValue) value = minValue;
			if(value > maxValue) value = maxValue;
			return value;
		}
		public static double GetArcAngle(double height, double distance) {
			return 4 * Math.Atan(2 * height / distance);
		}
		public static double GetArcRadius(double height, double angleRad) {
			return height / (1 - Math.Cos(angleRad / 2));
		}
		public static Rect GetBoundingRect(Point leftPt, double distance, double height, bool clockwise) {
			double angle = GetArcAngle(height, distance);
			double radius = GetArcRadius(height, angle);
			Rect rect = new Rect(0, 0, 2 * radius, 2 * radius);
			if(clockwise) {
				rect.Y = leftPt.Y + height - 2 * radius;
				rect.X = leftPt.X - (radius - distance / 2);
			}
			else {
				rect.Y = leftPt.Y - height;
				rect.X = leftPt.X - (radius - distance / 2);
			}
			return rect;
		}
		public static float GetArcStartAngle(double height, double distance, bool clockwise) {
			float angle = (float)ToAngle(GetArcAngle(height, distance));
			return clockwise ? (90f - angle / 2) : (angle / 2 - 90f);
		}
		public static float GetArcSweepAngle(double height, double distance, bool clockwise) {
			float angle = (float)MathUtils.ToAngle(GetArcAngle(height, distance));
			return clockwise ? angle : -angle;
		}
		public static double ToAngle(double radian) {
			return radian * 180 / Math.PI;
		}
	}
}
