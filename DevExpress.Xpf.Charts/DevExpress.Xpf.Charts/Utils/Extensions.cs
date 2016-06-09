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

using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using System;
namespace DevExpress.Xpf.Charts.Native {
	public static class RectExtensions {
		public static readonly Rect Zero = new Rect(0, 0, 0, 0);
		public static Point CalcCenter(this Rect rect) {
			return new Point(0.5 * (rect.Left + rect.Right), 0.5 * (rect.Top + rect.Bottom));
		}
		public static Point CalcRelativeToLeftTopCenter(this Rect rect) {
			return new Point(rect.Width/2.0, rect.Height/2.0);
		}
		public static Rect Intersect(Rect rect1, Rect rect2) {
			return Rect.Intersect(rect1, rect2);
		}
		public static GRealRect2D ToGRealRect2D(this Rect rect) {
			return new GRealRect2D(rect.Left, rect.Top, rect.Width, rect.Height);
		}
	}
	public static class TransformExtensions {
		public static bool IsIdentity(this Transform transform) {
			MatrixTransform matrixTransform = transform as MatrixTransform;
			return matrixTransform != null && matrixTransform.Matrix == Matrix.Identity;
		}
	}
	public static class PointExtensions {
		public static GRealPoint2D ToGRealPoint2D(this Point point) {
			return new GRealPoint2D(point.X, point.Y);
		}
		public static double CalculateDistanceTo(this Point thisPoint, Point anotherPoint) {
			return Math.Sqrt((thisPoint.X - anotherPoint.X) * (thisPoint.X - anotherPoint.X) + (thisPoint.Y - anotherPoint.Y) * (thisPoint.Y - anotherPoint.Y));
		}
		public static Point MoveByVector(this Point thisPoint, double deltaX, double deltaY){
			double newX = thisPoint.X + deltaX;
			double newY = thisPoint.Y + deltaY;
			return new Point(newX, newY);
		}
		public static Point MoveByAngle(this Point startPoint, double length, double angleDeg) {
			double angleRadian = MathUtils.Degree2Radian(angleDeg);
			return new Point(startPoint.X + Math.Cos(angleRadian) * length, startPoint.Y + Math.Sin(angleRadian) * length);
		}
	}
	public static class GRealPoint2DExtensions {
		public static Point ToPoint(this GRealPoint2D point) {
			return new Point(point.X, point.Y);
		}  
	}
	public static class GRect2DExtensions {
		public static Rect ToRect(this GRect2D gRect){
			return new Rect(gRect.Left, gRect.Top, gRect.Width, gRect.Height);
		}
		public static GRect2D Inflate(this GRect2D thisGRect, Thickness thickness){
			int left = thisGRect.Left - MathUtils.StrongRound(thickness.Left);
			int top = thisGRect.Top - MathUtils.StrongRound(thickness.Top);
			int width = thisGRect.Width + MathUtils.StrongRound(thickness.Left + thickness.Right);
			int height = thisGRect.Height + MathUtils.StrongRound(thickness.Top + thickness.Bottom);
			width = width < 0 ? 0 : width;
			height = height < 0 ? 0 : height;
			return new GRect2D(left, top, width, height);
		}
	}
}
