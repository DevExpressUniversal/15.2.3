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
using System.Windows.Media;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core.Native {
#endif
	public enum RectCorner { TopLeft, TopRight, BottomRight, BottomLeft }
	public enum RectKeyPoint { TopLeft, TopRight, BottomRight, BottomLeft, LeftMiddle, RightMiddle, TopMiddle, BottomMiddle }
	public enum RectTruncatorResultType { InvalidLine, MindlessCorner, Path, Rectangle, TangentLine }
	public class MathLine {
		public const double Precision = 1e-5;
		public double A { get; protected internal set; }
		public double B { get; protected internal set; }
		public double C { get; protected internal set; }
		public bool IsValid { get { return Math.Abs(A) > Precision || Math.Abs(B) > Precision; } }
		public MathLine() { A = B = C = 0; }
		public MathLine(double a, double b, double c) { A = a; B = b; C = c; }
		public MathLine(Point p1, Point p2) {
			A = p2.Y - p1.Y;
			B = p1.X - p2.X;
			C = p2.X * p1.Y - p1.X * p2.Y;
		}
		public Point? Intersect(MathLine line) {
			double det = A * line.B - B * line.A;
			double x = (-C * line.B + B * line.C) / det;
			double y = (-A * line.C + C * line.A) / det;
			if(x.IsNotNumber() || y.IsNotNumber()) return null;
			return new Point(x, y);
		}
		public MathLine Perpendicular(Point p) { return new MathLine(B, -A, A * p.Y - B * p.X); }
		public bool IsVertical { get { return Math.Abs(B) < Precision; } }
		public bool IsHorizontal { get { return Math.Abs(A) < Precision; } }
		public double AngleCoefficient { get { return -A / B; } }
		public bool IsMoreHorizontal { get { return Math.Abs(AngleCoefficient) < 1.0; } }
		public bool IsMoreVertical { get { return Math.Abs(AngleCoefficient) > 1.0; } }
		public double AbscissAngleRad {
			get {
				double x = B;
				double y = -A;
				if(x < 0) {
					x = -x;
					y = -y;
				}
				if(x == 0 && y < 0)
					y = -y;
				return Math.Atan2(y, x);
			}
		}
		public double AbscissAngleDeg { get { return AbscissAngleRad * 180 / Math.PI; } }
		public double CalcX(double y) { return -(B * y + C) / A; }
		public double CalcY(double x) { return -(A * x + C) / B; }
		public double CalcLineFunction(Point point) { return A * point.X + B * point.Y + C; }
		public double Distance(Point point) { return Math.Abs(CalcLineFunction(point)) / Math.Sqrt(A * A + B * B); }
		public bool IsSameSide(Point point, Point otherPoint) { return Math.Sign(CalcLineFunction(point)) == Math.Sign(CalcLineFunction(otherPoint)); }
		public bool IsDifferentSide(Point point, Point otherPoint) { return !IsSameSide(point, otherPoint); }
	}
	public static class DoubleExtension {
		public static bool IsNotNumber(this double number) { return double.IsNaN(number) || double.IsInfinity(number); }
		public static bool IsNumber(this double number) { return !number.IsNotNumber(); }
	}
	public static class PointExtension {
		public static Point MiddlePoint(this Point point, Point otherPoint) {
			return new Point((point.X + otherPoint.X) / 2, (point.Y + otherPoint.Y) / 2);
		}
		public static Point RadialEdgePoint(this Point startPoint, Point endPoint, double radius) {
			MathLine vectorLine = new MathLine(startPoint, endPoint);
			double t = radius / Math.Sqrt(vectorLine.A * vectorLine.A + vectorLine.B * vectorLine.B);
			if(t.IsNotNumber()) return startPoint;
			double dx = Math.Abs(vectorLine.B) * t * Math.Sign(endPoint.X - startPoint.X);
			double dy = Math.Abs(vectorLine.A) * t * Math.Sign(endPoint.Y - startPoint.Y);
			return new Point(startPoint.X + dx, startPoint.Y + dy);
		}
		public static Point MirrorPoint(this Point point, MathLine line) {
			MathLine perpendicular = line.Perpendicular(point);
			Point? middle = line.Intersect(perpendicular);
			if(!middle.HasValue) return point;
			double dx = ((Point)middle).X - point.X;
			double dy = ((Point)middle).Y - point.Y;
			return new Point(point.X + 2 * dx, point.Y + 2 * dy);
		}
		public static Rect GetNearestRect(this Point point, IEnumerable<Rect> rects) {
			return point.GetNearestRect(rects, (p, r) => ((Vector)p - (Vector)r.Center()).Length);
		}
		public static Rect GetNearestRect(this Point point, IEnumerable<Rect> rects, Func<Point, Rect, double> measure) {
			Rect nearestRect = new Rect();
			double distance = double.MaxValue;
			foreach (Rect rect in rects) {
				double currentDistance = measure(point, rect);
				if (currentDistance < distance) {
					nearestRect = rect;
					distance = currentDistance;
				}
			}
			return nearestRect;
		}
	}
	public static class RectExtension {
		public static MathLine TopLine(this Rect rect) { return new MathLine(0, 1, -rect.Top); }
		public static MathLine BottomLine(this Rect rect) { return new MathLine(0, 1, -rect.Bottom); }
		public static MathLine LeftLine(this Rect rect) { return new MathLine(1, 0, -rect.Left); }
		public static MathLine RightLine(this Rect rect) { return new MathLine(1, 0, -rect.Right); }
		public static RectCorner NearestCorner(this Rect rect, Point point) {
			double[] distances = {
									 point.Distance(rect.TopLeft()),
									 point.Distance(rect.TopRight()),
									 point.Distance(rect.BottomRight()),
									 point.Distance(rect.BottomLeft())
								 };
			double minDistance = distances[0];
			RectCorner nearestCorner = RectCorner.TopLeft;
			for(int i = 0; i < 4; i++) {
				if(distances[i] < minDistance) {
					minDistance = distances[i];
					nearestCorner = (RectCorner)i;
				}
			}
			return nearestCorner;
		}
		public static Point Corner(this Rect rect, RectCorner corner) {
			switch(corner) {
				case RectCorner.TopLeft: return rect.TopLeft();
				case RectCorner.TopRight: return rect.TopRight();
				case RectCorner.BottomRight: return rect.BottomRight();
				default: return rect.BottomLeft();
			}
		}
		public static Point KeyPoint(this Rect rect, RectKeyPoint keyPoint) {
			switch(keyPoint) {
				case RectKeyPoint.TopLeft: return rect.Corner(RectCorner.TopLeft);
				case RectKeyPoint.TopMiddle: return rect.TopMiddle();
				case RectKeyPoint.TopRight: return rect.Corner(RectCorner.TopRight);
				case RectKeyPoint.RightMiddle: return rect.RightMiddle();
				case RectKeyPoint.BottomRight: return rect.Corner(RectCorner.BottomRight);
				case RectKeyPoint.BottomMiddle: return rect.BottomMiddle();
				case RectKeyPoint.BottomLeft: return rect.Corner(RectCorner.BottomLeft);
				default: return rect.LeftMiddle();
			}
		}
		public static double CenterX(this Rect rect) { return rect.Left + rect.Width / 2; }
		public static double CenterY(this Rect rect) { return rect.Top + rect.Height / 2; }
		public static Point Center(this Rect rect) { return new Point(rect.CenterX(), rect.CenterY()); }
		public static Point TopMiddle(this Rect rect) { return new Point(rect.CenterX(), rect.Top); }
		public static Point BottomMiddle(this Rect rect) { return new Point(rect.CenterX(), rect.Bottom); }
		public static Point LeftMiddle(this Rect rect) { return new Point(rect.Left, rect.CenterY()); }
		public static Point RightMiddle(this Rect rect) { return new Point(rect.Right, rect.CenterY()); }
		public static bool IsInRange(double x, double min, double max) { return x >= min && x <= max; }
		public static bool IsXInRange(this Rect rect, Point point) { return IsInRange(point.X, rect.Left, rect.Right); }
		public static bool IsYInRange(this Rect rect, Point point) { return IsInRange(point.Y, rect.Top, rect.Bottom); }
		public static bool IsInside(this Rect rect, Point point) { return IsXInRange(rect, point) && IsYInRange(rect, point); }
	}
	public static class RectCornerExtension {
		public static bool IsLeft(this RectCorner corner) { return corner == RectCorner.TopLeft || corner == RectCorner.BottomLeft; }
		public static bool IsRight(this RectCorner corner) { return corner == RectCorner.TopRight || corner == RectCorner.BottomRight; }
		public static bool IsTop(this RectCorner corner) { return corner == RectCorner.TopLeft || corner == RectCorner.TopRight; }
		public static bool IsBottom(this RectCorner corner) { return corner == RectCorner.BottomLeft || corner == RectCorner.BottomRight; }
		public static bool IsSameHorizontalSide(this RectCorner corner, RectCorner otherCorner) { return corner.IsLeft() == otherCorner.IsLeft(); }
		public static bool IsSameVerticalSide(this RectCorner corner, RectCorner otherCorner) { return corner.IsTop() == otherCorner.IsTop(); }
		public static RectCorner HorizontalMirror(this RectCorner corner) {
			RectCorner[] horizontalMirrorCorners = new RectCorner[] { RectCorner.TopRight, RectCorner.TopLeft, RectCorner.BottomLeft, RectCorner.BottomRight };
			return horizontalMirrorCorners[(int)corner];
		}
		public static RectCorner VerticalMirror(this RectCorner corner) {
			RectCorner[] verticalMirrorCorners = new RectCorner[] { RectCorner.BottomLeft, RectCorner.BottomRight, RectCorner.TopRight, RectCorner.TopLeft };
			return verticalMirrorCorners[(int)corner];
		}
		public static RectCorner DiagonalMirror(this RectCorner corner) {
			RectCorner[] diagonalMirrorCorners = new RectCorner[] { RectCorner.BottomRight, RectCorner.BottomLeft, RectCorner.TopLeft, RectCorner.TopRight };
			return diagonalMirrorCorners[(int)corner];
		}
		public static RectKeyPoint HorizontalMiddle(this RectCorner corner) { return corner.IsTop() ? RectKeyPoint.TopMiddle : RectKeyPoint.BottomMiddle; }
		public static RectKeyPoint VerticalMiddle(this RectCorner corner) { return corner.IsLeft() ? RectKeyPoint.LeftMiddle : RectKeyPoint.RightMiddle; }
	}
	public class RectTruncator {
		protected internal enum PointType { Null, Simple, Intersect, Outside }
		const double Precision = 0.5;
		#region Property
		public RectTruncatorResultType ResultType;
		public PointCollection ResultPoints { get; set; }
		public PointCollection InvertedResultPoints { get; set; }
		protected internal Rect Rectangle { get; set; }
		protected internal MathLine Line { get; set; }
		protected internal RectCorner Corner { get; set; }
		protected internal Point?[] Points { get; set; }
		protected internal PointType[] PointTypes { get; set; }
		protected internal int[] NearestOutsidePointsCounters { get; set; }
		protected internal Point? TopIntersectPoint { get { return Rectangle.TopLine().Intersect(Line); } }
		protected internal Point? BottomIntersectPoint { get { return Rectangle.BottomLine().Intersect(Line); } }
		protected internal Point? RightIntersectPoint { get { return Rectangle.RightLine().Intersect(Line); } }
		protected internal Point? LeftIntersectPoint { get { return Rectangle.LeftLine().Intersect(Line); } }
		protected internal bool IsCornerDeterminant { get { return PointTypes[FirstPointIndex] == PointType.Simple; } }
		protected internal int FirstPointIndex { get { return ((int)Corner) * 2; } }
		#endregion
		public RectTruncator(Rect rect, MathLine line, RectCorner corner) {
			Rectangle = rect;
			Line = line;
			Corner = corner;
			Proceed();
		}
		protected internal void Proceed() {
			if(Line.IsValid)
				ProceedValidLine();
			else
				ResultType = RectTruncatorResultType.InvalidLine;
		}
		protected internal void ProceedValidLine() {
			CreatePointsArray();
			CreatePointTypesArray();
			CreateNearestOutsidePointsCounterArray();
			CorrectPointTypesArrayByNearestOutsidePointsCounterArray();
			CorrectPointTypesArrayByDistances();
			if(IsInvalidNumberOfIntersectPoints()) {
				ResultType = RectTruncatorResultType.Rectangle;
				return;
			}
			if(IsIntersectPointsMindless()) {
				ResultType = RectTruncatorResultType.TangentLine;
				return;
			}
			if(!IsCornerDeterminant) {
				ResultType = RectTruncatorResultType.MindlessCorner;
				return;
			}
			ProceedPath();
		}
		protected internal void ProceedPath() {
			ResultPoints = new PointCollection();
			InvertedResultPoints = new PointCollection();
			CreatePaths();
			ResultType = RectTruncatorResultType.Path;
		}
		protected internal void CreatePointsArray() {
			Points = new Point?[8];
			Points[0] = Rectangle.TopLeft();
			Points[1] = TopIntersectPoint;
			Points[2] = Rectangle.TopRight();
			Points[3] = RightIntersectPoint;
			Points[4] = Rectangle.BottomRight();
			Points[5] = BottomIntersectPoint;
			Points[6] = Rectangle.BottomLeft();
			Points[7] = LeftIntersectPoint;
		}
		protected internal void CreatePointTypesArray() {
			PointTypes = new PointType[8];
			for(int i = 0; i < 8; i += 2)
				PointTypes[i] = PointType.Simple;
			for(int i = 1; i < 8; i += 2) {
				Point? point = Points[i];
				if(!point.HasValue)
					PointTypes[i] = PointType.Null;
				else
					PointTypes[i] = IsIntersectPoint(point.Value, i) ? PointType.Intersect : PointType.Outside;
			}
		}
		protected internal bool IsIntersectPoint(Point point, int index) {
			if(index == 1 || index == 5)
				return Rectangle.IsXInRange(point);
			return Rectangle.IsYInRange(point);
		}
		protected internal void CreateNearestOutsidePointsCounterArray() {
			NearestOutsidePointsCounters = new int[4] { 0, 0, 0, 0 };
			for(int i = 1; i < 8; i += 2) {
				if(PointTypes[i] == PointType.Outside) {
					RectCorner nearestCorner = Rectangle.NearestCorner(Points[i].Value);
					NearestOutsidePointsCounters[(int)nearestCorner]++;
				}
			}
		}
		protected internal void CorrectPointTypesArrayByNearestOutsidePointsCounterArray() {
			for(int i = 0; i < 8; i += 2)
				if(NearestOutsidePointsCounters[i / 2] == 2)
					PointTypes[i] = PointType.Intersect;
		}
		protected internal void CorrectPointTypesArrayByDistances() {
			for(int currIndex = 0; currIndex < 8; currIndex += 2) {
				int prevIndex = SafeArrayIndex(currIndex - 1);
				int nextIndex = SafeArrayIndex(currIndex + 1);
				Point cornerPoint = Points[currIndex].Value;
				if(PointTypes[prevIndex] == PointType.Intersect && cornerPoint.Distance(Points[prevIndex].Value) < Precision) {
					PointTypes[currIndex] = PointType.Intersect;
					PointTypes[prevIndex] = PointType.Outside;
				}
				if(PointTypes[nextIndex] == PointType.Intersect && cornerPoint.Distance(Points[nextIndex].Value) < Precision) {
					PointTypes[currIndex] = PointType.Intersect;
					PointTypes[nextIndex] = PointType.Outside;
				}
			}
		}
		protected internal bool IsInvalidNumberOfIntersectPoints() { return CalcNumberOfIntersectPoints() != 2; }
		protected internal int CalcNumberOfIntersectPoints() {
			int result = 0;
			for(int i = 0; i < 8; i++)
				if(PointTypes[i] == PointType.Intersect)
					result++;
			return result;
		}
		protected internal bool IsIntersectPointsMindless() {
			int firstIndex = FindIntersectPointIndex(0);
			int secondIndex = FindIntersectPointIndex(firstIndex + 1);
			int difference = secondIndex - firstIndex;
			bool conditionAll = difference == 0 || difference == 1 || difference == 7;
			bool conditionEven = firstIndex % 2 == 0 && (difference == 2 || difference == 6);
			return conditionAll || conditionEven;
		}
		protected internal int FindIntersectPointIndex(int startIndex) {
			for(int i = 0; i < 8; i++) {
				int index = SafeArrayIndex(i + startIndex);
				if(PointTypes[index] == PointType.Intersect)
					return index;
			}
			return 0;
		}
		protected internal void CreatePaths() {
			bool skipFlag = false;
			for(int i = 0; i < 8; i++) {
				int index = Index(i);
				bool isIntersect = PointTypes[index] == PointType.Intersect;
				bool isSimple = PointTypes[index] == PointType.Simple;
				if(isIntersect)
					skipFlag = !skipFlag;
				if(isIntersect || (isSimple && !skipFlag))
					ResultPoints.Add(Points[index].Value);
				if(isIntersect || (isSimple && skipFlag))
					InvertedResultPoints.Add(Points[index].Value);
			}
		}
		protected internal int Index(int i) { return SafeArrayIndex(FirstPointIndex + i); }
		protected internal static int SafeArrayIndex(int i) { return (i % 8 + 8) % 8; }
	}
	public class PathBuilder {
		#region Property
		public int NumberOfPoints { get; private set; }
		public Point StartPoint { get; private set; }
		public Point EndPoint { get; private set; }
		protected internal double SpeedRate { get; set; }
		protected internal double Radius { get; set; }
		protected internal Point CenterPoint { get; set; }
		protected internal bool IsArc { get; set; }
		protected internal double DeltaX { get; set; }
		protected internal double DeltaY { get; set; }
		protected internal int CurrentPointNumber { get; set; }
		protected internal double StartAngle { get; set; }
		protected internal double EndAngle { get; set; }
		protected internal double DeltaAngle { get; set; }
		protected internal bool IsPath { get { return StartPoint != EndPoint; } }
		#endregion
		public PathBuilder(Point startPoint, Point endPoint, double speedRate) {
			StartPoint = startPoint;
			EndPoint = endPoint;
			SpeedRate = speedRate;
			CurrentPointNumber = 0;
		}
		public void CreateLinePath() {
			IsArc = false;
			if(!IsPath)
				return;
			NumberOfPoints = (int)(StartPoint.Distance(EndPoint) * SpeedRate);
			DeltaX = (EndPoint.X - StartPoint.X) / NumberOfPoints;
			DeltaY = (EndPoint.Y - StartPoint.Y) / NumberOfPoints;
		}
		public void CreateArcPath(double radius, Point referencePoint, bool isReferenceInside) {
			IsArc = true;
			if(!IsPath)
				return;
			Radius = radius;
			CalcCenterPoint(referencePoint, isReferenceInside);
			CalcArcAngles();
			NumberOfPoints = (int)(Math.Abs(EndAngle - StartAngle) * Radius * SpeedRate);
			DeltaAngle = Math.Sign(EndAngle - StartAngle) / (Radius * SpeedRate);
		}
		public Point GetPoint() {
			if(!IsPath || CurrentPointNumber == NumberOfPoints)
				return EndPoint;
			return IsArc ? GetArcPoint() : GetLinePoint();
		}
		public void Reset() { CurrentPointNumber = 0; }
		protected internal void CalcCenterPoint(Point referencePoint, bool isReferenceInside) {
			MathLine line = new MathLine(StartPoint, EndPoint);
			Point middlePoint = StartPoint.MiddlePoint(EndPoint);
			MathLine perpendicular = line.Perpendicular(middlePoint);
			double d = StartPoint.Distance(EndPoint);
			double l = Math.Sqrt(Radius * Radius - d * d / 4);
			double divisor = Math.Sqrt(perpendicular.A * perpendicular.A + perpendicular.B * perpendicular.B);
			double addX = l * perpendicular.B / divisor;
			double addY = l * perpendicular.A / divisor;
			double fun1 = perpendicular.CalcLineFunction(new Point(middlePoint.X + addX, middlePoint.Y + addY));
			double fun2 = perpendicular.CalcLineFunction(new Point(middlePoint.X + addX, middlePoint.Y - addY));
			if(Math.Abs(fun2) < Math.Abs(fun1))
				addY = -addY;
			CenterPoint = new Point(middlePoint.X + addX, middlePoint.Y + addY);
			if(line.IsSameSide(CenterPoint, referencePoint) != isReferenceInside)
				CenterPoint = new Point(middlePoint.X - addX, middlePoint.Y - addY);
		}
		protected internal void CalcArcAngles() {
			StartAngle = Math.Acos((StartPoint.X - CenterPoint.X) / Radius) * Math.Sign(StartPoint.Y - CenterPoint.Y);
			EndAngle = Math.Acos((EndPoint.X - CenterPoint.X) / Radius) * Math.Sign(EndPoint.Y - CenterPoint.Y);
		}
		protected internal Point GetLinePoint() {
			double x = StartPoint.X + DeltaX * CurrentPointNumber;
			double y = StartPoint.Y + DeltaY * CurrentPointNumber;
			return NextPoint(x, y);
		}
		protected internal Point GetArcPoint() {
			double angle = StartAngle + DeltaAngle * CurrentPointNumber;
			double x = CenterPoint.X + Radius * Math.Cos(angle);
			double y = CenterPoint.Y + Radius * Math.Sin(angle);
			return NextPoint(x, y);
		}
		protected internal Point NextPoint(double x, double y) {
			CurrentPointNumber++;
			return new Point(x, y);
		}
	}
}
