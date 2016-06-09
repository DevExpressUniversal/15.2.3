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
using System.Windows;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public sealed class DDAAlgorithm {
		readonly Rect bounds;
		readonly double dx;
		readonly double dy;
		GRealPoint2D currentPoint;
		public GRealPoint2D CurrentPoint { get { return currentPoint; } }
		public DDAAlgorithm(GRealPoint2D startPoint, GRealPoint2D finishPoint) {
			bounds = new Rect(new Point(startPoint.X, startPoint.Y), new Point(finishPoint.X, finishPoint.Y));
			double lengthX = Math.Abs(finishPoint.X - startPoint.X);
			double lengthY = Math.Abs(finishPoint.Y - startPoint.Y);
			if (lengthY <= lengthX) {
				dx = finishPoint.X - startPoint.X >= 0 ? 1 : -1;
				dy = finishPoint.Y - startPoint.Y >= 0 ? lengthY / lengthX : -lengthY / lengthX;
			}
			else {
				dx = finishPoint.X - startPoint.X >= 0 ? lengthX / lengthY : -lengthX / lengthY;
				dy = finishPoint.Y - startPoint.Y >= 0 ? 1 : -1;
			}
			currentPoint = startPoint;
		}
		public bool NextPoint() {
			currentPoint = new GRealPoint2D(currentPoint.X + dx, currentPoint.Y + dy);
			return bounds.Contains(new Point(currentPoint.X, currentPoint.Y));
		}
	}
	public static class MathUtils {
		public static int StrongRound(double value) {
			return Math.Sign(value) * (int)(Math.Abs(value) + 0.5);
		}
		public static Point StrongRound(Point point) {
			return new Point(StrongRound(point.X), StrongRound(point.Y));
		}
		public static Rect StrongRound(Rect rect) {
			double height = StrongRound(rect.Height);
			double width = StrongRound(rect.Width);
			return new Rect(StrongRound((rect.Left + rect.Right - width) / 2), StrongRound((rect.Top + rect.Bottom - height) / 2), width, height);
		}
		public static Thickness StrongRound(Thickness thickness) {
			return new Thickness(StrongRound(thickness.Left), StrongRound(thickness.Top), StrongRound(thickness.Right), StrongRound(thickness.Bottom));
		}
		public static double NormalizeDegree(double angleDegree) {
			return Radian2Degree(GeometricUtils.NormalizeRadian(Degree2Radian(angleDegree)));
		}		
		public static double Degree2Radian(double angleDegree) {
			return angleDegree * Math.PI / 180.0;
		}
		public static double Radian2Degree(double angleRadian) {
			return angleRadian * 180 / Math.PI;
		}
		public static double CancelAngle(double angle) {
			double result = angle - 360 * ((int)angle / 360);
			if (result < 0)
				result += 360;
			return result;
		}
		public static double CalcDistance(Point p1, Point p2) {
			double dx = p1.X - p2.X;
			double dy = p1.Y - p2.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}
		public static GRealPoint2D CalcCenter(GRect2D rect) {
			return new GRealPoint2D(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
		}
		public static Point CalcCenter(Point p1, Point p2) {
			return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
		}
		public static Point CalcCenter(Rect rect) {
			return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
		}
		public static void Normalize(ref Vector3D n) {
			if (n.Length > 0)
				n.Normalize();
		}
		public static Point3D Offset(Point3D point, double dx, double dy, double dz) {
			return new Point3D(point.X + dx, point.Y + dy, point.Z + dz);
		}
		public static double CalcDistance(Point3D p1, Point3D p2) {
			double dx = p1.X - p2.X;
			double dy = p1.Y - p2.Y;
			double dz = p1.Z - p2.Z;
			return Math.Sqrt(dx * dx + dy * dy + dz * dz);
		}
		public static Point3D CalcCenter(Rect3D rect) {
			return new Point3D(rect.Location.X + rect.SizeX / 2, rect.Location.Y + rect.SizeY / 2, rect.Location.Z + rect.SizeZ / 2);
		}		
		public static Vector3D CalcNormal(Point3D p1, Point3D p2, Point3D p3) {
			Vector3D v1 = p2 - p1;
			Vector3D v2 = p3 - p1;
			Vector3D normal = Vector3D.CrossProduct(p2 - p1, p3 - p1);
			Normalize(ref normal);
			return normal;
		}
		public static Matrix3D Transpose(Matrix3D matrix) {
			Matrix3D newMatrix = new Matrix3D();
			newMatrix.M11 = matrix.M11;
			newMatrix.M12 = matrix.M21;
			newMatrix.M13 = matrix.M31;
			newMatrix.M14 = matrix.OffsetX;
			newMatrix.M21 = matrix.M12;
			newMatrix.M22 = matrix.M22;
			newMatrix.M23 = matrix.M32;
			newMatrix.M24 = matrix.OffsetY;
			newMatrix.M31 = matrix.M13;
			newMatrix.M32 = matrix.M23;
			newMatrix.M33 = matrix.M33;
			newMatrix.M34 = matrix.OffsetZ;
			newMatrix.OffsetX = matrix.M14;
			newMatrix.OffsetY = matrix.M24;
			newMatrix.OffsetZ = matrix.M34;
			newMatrix.M44 = matrix.M44;
			return newMatrix;
		}
		public static Transform3D CalcRotationTransform(Matrix3D matrix) {
			Transform3DGroup group = new Transform3DGroup();
			Vector3D v1 = new Vector3D(1, 0, 0);
			Vector3D v2 = new Vector3D(0, 1, 0);
			Vector3D v1t = matrix.Transform(v1);
			Vector3D v2t = matrix.Transform(v2);
			double angle = Vector3D.AngleBetween(v1, v1t);
			if (angle > 0) {
				Vector3D n = angle < 180 ? Vector3D.CrossProduct(v1, v1t) : v2;
				RotateTransform3D t = new RotateTransform3D(new AxisAngleRotation3D(n, angle));
				v2 = t.Transform(v2);
				group.Children.Add(t);
			}
			angle = Vector3D.AngleBetween(v2, v2t);
			if (angle > 0) {
				Vector3D n = angle < 180 ? Vector3D.CrossProduct(v2, v2t) : v1t;
				RotateTransform3D t = new RotateTransform3D(new AxisAngleRotation3D(n, angle));
				group.Children.Add(t);
			}
			return group;
		}
		public static double CalcArea(IList<Point> points) {
			if (points.Count < 3)
				return 0;
			double area = 0;
			for (int i = 0; i < points.Count; i++) {
				int k = (i + 1) % points.Count;
				area += points[i].X * points[k].Y - points[k].X * points[i].Y;
			}
			return area / 2;
		}
		public static void CorrectSmoothLine(double thickness, ref GRealPoint2D start, ref GRealPoint2D end) {
			double x1 = start.X;
			double y1 = start.Y;
			double x2 = end.X;
			double y2 = end.Y;
			CorrectSmoothLine(thickness, ref x1, ref y1, ref x2, ref y2);
			start.X = x1;
			start.Y = y1;
			end.X = x2;
			end.Y = y2;
		}
		public static void CorrectSmoothLine(double thickness, ref Point start, ref Point end) {
			double x1 = start.X;
			double y1 = start.Y;
			double x2 = end.X; 
			double y2 = end.Y;
			CorrectSmoothLine(thickness, ref x1, ref y1, ref x2, ref y2);
			start.X = x1;
			start.Y = y1;
			end.X = x2;
			end.Y = y2;
		}
		public static void CorrectSmoothLine(double thickness, ref double x1, ref double y1, ref double x2, ref double y2) {
			bool isHorizontal = Math.Abs(y1 - y2) < 1;
			bool isVertical = Math.Abs(x1 - x2) < 1;
			if (thickness % 2 != 0) {
				if (isVertical)
					x2 = x1 = (int)Math.Floor(x1) + 0.5;
				else {
					x1 = MathUtils.StrongRound(x1);
					x2 = MathUtils.StrongRound(x2);
				}
				if (isHorizontal)
					y1 = y2 = Math.Ceiling(y1) - 0.5;
				else {
					y1 = MathUtils.StrongRound(y1);
					y2 = MathUtils.StrongRound(y2);
				}
			}
			else {
				x1 = MathUtils.StrongRound(x1);
				x2 = (int)MathUtils.StrongRound(x2);
				if (isVertical)
					x2 = x1;
				y1 = MathUtils.StrongRound(y1);
				y2 = (int)MathUtils.StrongRound(y2);
				if (isHorizontal)
					y2 = y1;
			}
		}
		public static double ConvertInfinityToDefault(double value, double defaultValue) {
			return double.IsInfinity(value) ? defaultValue : value;
		}
		public static Size ConvertInfinityToDefault(Size size, double defaultValue) {
			return new Size(MathUtils.ConvertInfinityToDefault(size.Width, defaultValue), MathUtils.ConvertInfinityToDefault(size.Height, defaultValue));
		}
		public static bool isEvenNumber(int number) {
			if (number % 2 == 0)
				return true;
			else
				return false;
		}
		public static IMinMaxValues CorrectMinMaxByRange(IAxisRangeData range, IMinMaxValues values) {
			if (values.Max * values.Min < 0)
				return values;
			double actualMin = range.Min;
			double actualMax = range.Max;
			if (values.Max > 0) {
				if (actualMin > values.Max || actualMax < values.Min)
					return new MinMaxValues(Double.NaN);
				if (values.Min > actualMin)
					actualMin = values.Min;
				if (values.Max < actualMax)
					actualMax = values.Max;
			} else {
				if (actualMax < values.Max || actualMin > values.Min)
					return new MinMaxValues(Double.NaN);
				if (values.Max > actualMin)
					actualMin = values.Max;
				if (values.Min < actualMax)
					actualMax = values.Min;
			}
			return new MinMaxValues(actualMin, actualMax);
		}
		public static double CalculateCenterValueByRange(IAxisRangeData range, IMinMaxValues values) {
			values = CorrectMinMaxByRange(range, values);
			return (Double.IsNaN(values.Min) || Double.IsNaN(values.Max)) ? Double.NaN : ((values.Min + values.Max) / 2);
		}
		public static bool IsNumericDouble(double d) {
			return !double.IsNaN(d) && !double.IsInfinity(d);
		}
	}
}
