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
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public abstract class PlaneRectangle : PlaneQuadrangle {
		public static PlaneRectangle Inflate(PlaneRectangle rect, double dx, double dy) {
			PlaneRectangle result = (PlaneRectangle)rect.CreateInstance();
			result.Assign(rect);
			result.Inflate(dx, dy);
			return result;
		}
		readonly Box box;
		readonly BoxPlane boxPlane = 0;
		DiagramPoint RelativeLeftBottom { get { return AbsoluteToRelative(LeftBottom); } }
		DiagramPoint RelativeRightTop { get { return AbsoluteToRelative(RightTop); } }
		public Vertex LeftBottom { get { return Vertices[0]; } }
		public Vertex LeftTop { get { return Vertices[3]; } }
		public Vertex RightBottom { get { return Vertices[1]; } }
		public Vertex RightTop { get { return Vertices[2]; } }
		public Vertex Location { get { return Vertices[0]; } }
		public DiagramPoint Center { get { return RelativeToAbsolute(DiagramPoint.Offset(RelativeLeftBottom, Width / 2.0, Height / 2.0, 0)); } }
		public DiagramPoint LeftCenter { get { return RelativeToAbsolute(DiagramPoint.Offset(RelativeLeftBottom, 0, Height / 2.0, 0)); } }
		public DiagramPoint RightCenter { get { return RelativeToAbsolute(DiagramPoint.Offset(RelativeRightTop, 0, -Height / 2.0, 0)); } }
		public DiagramPoint BottomCenter { get { return RelativeToAbsolute(DiagramPoint.Offset(RelativeLeftBottom, Width / 2.0, 0, 0)); } }
		public DiagramPoint TopCenter { get { return RelativeToAbsolute(DiagramPoint.Offset(RelativeRightTop, -Width / 2.0, 0, 0)); } }
		public double Left { get { return LeftCenter.X; } }
		public double Right { get { return RightCenter.X; } }
		public double Bottom { get { return BottomCenter.Y; } }
		public double Top { get { return TopCenter.Y; } }
		public SizeF Size { get { return new SizeF(Convert.ToSingle(Width), Convert.ToSingle(Height)); } }
		public double Width {
			get { return AbsoluteToRelative(RightTop).X - AbsoluteToRelative(LeftBottom).X; }
			set {
				DiagramPoint leftBottom = AbsoluteToRelative(LeftBottom);
				DiagramPoint rightTop = AbsoluteToRelative(RightTop);
				rightTop = new DiagramPoint(leftBottom.X + value, rightTop.Y, rightTop.Z);
				Init(leftBottom, rightTop);
			}
		}
		public double Height {
			get { return AbsoluteToRelative(RightTop).Y - AbsoluteToRelative(LeftBottom).Y; }
			set {
				DiagramPoint leftBottom = AbsoluteToRelative(LeftBottom);
				DiagramPoint rightTop = AbsoluteToRelative(RightTop);
				rightTop = new DiagramPoint(rightTop.X, leftBottom.Y + value, rightTop.Z);
				Init(leftBottom, rightTop);
			}
		}
		public Box Box { get { return box; } }
		public override BoxPlane BoxPlane {
			get {
				if (boxPlane != 0)
					return boxPlane;
				return box == null ? 0 : box.GetBoxPlane(this);
			}
		}
		public bool IsZero { get { return Width == 0 || Height == 0; } }
		protected PlaneRectangle() {
		}
		public PlaneRectangle(DiagramPoint p1, DiagramPoint p2)
			: base() {
#if DEBUGTEST
			if (!ArePointsCorrect(p1, p2))
				throw new ArgumentException();
#endif
			DiagramPoint relP1 = AbsoluteToRelative(p1);
			DiagramPoint relP2 = AbsoluteToRelative(p2);
			Init(relP1, relP2);
		}
		public PlaneRectangle(DiagramPoint point, double width, double height, Box box)
			: this(point, width, height) {
			this.box = box;
		}
		public PlaneRectangle(DiagramPoint point, double width, double height, BoxPlane boxPlane)
			: this(point, width, height) {
			this.boxPlane = boxPlane;
		}
		public PlaneRectangle(DiagramPoint point, double width, double height) {
			DiagramPoint relP1 = AbsoluteToRelative(point);
			DiagramPoint relP2 = DiagramPoint.Offset(relP1, width, height, 0);
			Init(relP1, relP2);
		}
		void Init(DiagramPoint p1, DiagramPoint p2) {
			DiagramPoint leftBottom = new DiagramPoint(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Min(p1.Z, p2.Z));
			DiagramPoint rightTop = new DiagramPoint(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y), leftBottom.Z);
			DiagramPoint rightBottom = new DiagramPoint(rightTop.X, leftBottom.Y, rightTop.Z);
			DiagramPoint leftTop = new DiagramPoint(leftBottom.X, rightTop.Y, rightTop.Z);
			SetVertices(new DiagramPoint[] {
				RelativeToAbsolute(leftBottom),
				RelativeToAbsolute(rightBottom),
				RelativeToAbsolute(rightTop),
				RelativeToAbsolute(leftTop) });
			Normal = RelativeToAbsolute(new DiagramVector(0, 0, 1));
			SameNormals = true;
		}
		Line CreateLine(DiagramPoint vertex1, DiagramPoint vertex2) {
			return new Line(vertex1, vertex2, true, false, MathUtils.CalcNormal(vertex2 - vertex1, Normal), Color.Empty);
		}
		protected abstract bool ArePointsCorrect(DiagramPoint p1, DiagramPoint p2);
		protected abstract DiagramPoint AbsoluteToRelative(DiagramPoint p);
		protected abstract DiagramPoint RelativeToAbsolute(DiagramPoint p);
		protected abstract DiagramVector RelativeToAbsolute(DiagramVector v);
		public void Inflate(double dx, double dy) {
			DiagramPoint leftBottom = DiagramPoint.Offset(RelativeLeftBottom, -dx, -dy, 0);
			DiagramPoint rightTop = DiagramPoint.Offset(RelativeRightTop, dx, dy, 0);
			if (rightTop.X < leftBottom.X)
				rightTop.X = leftBottom.X;
			if (rightTop.Y < leftBottom.Y)
				rightTop.Y = leftBottom.Y;
			Init(leftBottom, rightTop);
		}
		public void Round() {
			Init(DiagramPoint.Round(RelativeLeftBottom), DiagramPoint.Round(RelativeRightTop));
		}
		protected void IncSize() {
			DiagramPoint rightTop = RelativeRightTop;
			rightTop.X++;
			rightTop.Y++;
			Init(RelativeLeftBottom, rightTop);
		}
		public Line[] GetPerimeter() {
			return new Line[] { 
				CreateLine(Vertices[0], Vertices[1]),
				CreateLine(Vertices[1], Vertices[2]),
				CreateLine(Vertices[2], Vertices[3]),
				CreateLine(Vertices[3], Vertices[0])
			};
		}
		public bool Contains(DiagramPoint point) {
			DiagramPoint leftBottom = RelativeLeftBottom;
			DiagramPoint rightTop = RelativeRightTop;
			DiagramPoint relativePoint = AbsoluteToRelative(point);
			return relativePoint.X >= leftBottom.X && relativePoint.X <= rightTop.X && relativePoint.Y >= leftBottom.Y && relativePoint.Y <= rightTop.Y;
		}
	}
	public class XPlaneRectangle : PlaneRectangle {
		public static XPlaneRectangle Empty { get { return new XPlaneRectangle(DiagramPoint.Zero, DiagramPoint.Zero); } }
		public static XPlaneRectangle MakeRectangle(DiagramPoint corner1, DiagramPoint corner2) {
			XPlaneRectangle rect = new XPlaneRectangle(corner1, corner2);
			rect.IncSize();
			return rect;
		}
		protected XPlaneRectangle()
			: base() {
		}
		public XPlaneRectangle(DiagramPoint p1, DiagramPoint p2)
			: base(p1, p2) {
		}
		public XPlaneRectangle(DiagramPoint point, double width, double height)
			: base(point, width, height) {
		}
		public XPlaneRectangle(DiagramPoint point, double width, double height, Box box)
			: base(point, width, height, box) {
		}
		public XPlaneRectangle(DiagramPoint point, double width, double height, BoxPlane boxPlane)
			: base(point, width, height, boxPlane) {
		}
		protected override bool ArePointsCorrect(DiagramPoint p1, DiagramPoint p2) {
			return p1.X == p2.X;
		}
		protected override DiagramPoint AbsoluteToRelative(DiagramPoint p) {
			return new DiagramPoint(p.Y, p.Z, p.X);
		}
		protected override DiagramPoint RelativeToAbsolute(DiagramPoint p) {
			return new DiagramPoint(p.Z, p.X, p.Y);
		}
		protected override DiagramVector RelativeToAbsolute(DiagramVector v) {
			return new DiagramVector(v.DZ, v.DX, v.DY);
		}
		protected override PlanePrimitive CreateInstance() {
			return new XPlaneRectangle();
		}
	}
	public class YPlaneRectangle : PlaneRectangle {
		public static YPlaneRectangle Empty { get { return new YPlaneRectangle(DiagramPoint.Zero, DiagramPoint.Zero); } }
		public static YPlaneRectangle MakeRectangle(DiagramPoint corner1, DiagramPoint corner2) {
			YPlaneRectangle rect = new YPlaneRectangle(corner1, corner2);
			rect.IncSize();
			return rect;
		}
		protected YPlaneRectangle()
			: base() {
		}
		public YPlaneRectangle(DiagramPoint p1, DiagramPoint p2)
			: base(p1, p2) {
		}
		public YPlaneRectangle(DiagramPoint point, double width, double height)
			: base(point, width, height) {
		}
		public YPlaneRectangle(DiagramPoint point, double width, double height, Box box)
			: base(point, width, height, box) {
		}
		public YPlaneRectangle(DiagramPoint point, double width, double height, BoxPlane boxPlane)
			: base(point, width, height, boxPlane) {
		}
		protected override bool ArePointsCorrect(DiagramPoint p1, DiagramPoint p2) {
			return p1.Y == p2.Y;
		}
		protected override DiagramPoint AbsoluteToRelative(DiagramPoint p) {
			return new DiagramPoint(p.Z, p.X, p.Y);
		}
		protected override DiagramPoint RelativeToAbsolute(DiagramPoint p) {
			return new DiagramPoint(p.Y, p.Z, p.X);
		}
		protected override DiagramVector RelativeToAbsolute(DiagramVector v) {
			return new DiagramVector(v.DY, v.DZ, v.DX);
		}
		protected override PlanePrimitive CreateInstance() {
			return new YPlaneRectangle();
		}
	}
	public class ZPlaneRectangle : PlaneRectangle {
		public static ZPlaneRectangle Empty { get { return new ZPlaneRectangle(DiagramPoint.Zero, DiagramPoint.Zero); } }
		public static ZPlaneRectangle Inflate(ZPlaneRectangle rect, double dx, double dy) { return (ZPlaneRectangle)PlaneRectangle.Inflate(rect, dx, dy); }
		public static ZPlaneRectangle MakeRectangle(DiagramPoint corner1, DiagramPoint corner2) {
			ZPlaneRectangle rect = new ZPlaneRectangle(corner1, corner2);
			rect.IncSize();
			return rect;
		}
		public static ZPlaneRectangle MakeRectangle(IList<GRealPoint2D> points) {
			if (points.Count == 0)
				return ZPlaneRectangle.Empty;
			double left = points[0].X;
			double right = points[0].X;
			double bottom = points[0].Y;
			double top = points[0].Y;
			for (int i = 1; i < points.Count; i++) {
				GRealPoint2D point = points[i];
				if (point.X < left)
					left = point.X;
				else if (point.X > right)
					right = point.X;
				if (point.Y < bottom)
					bottom = point.Y;
				else if (point.Y > top)
					top = point.Y;
			}
			return new ZPlaneRectangle(new DiagramPoint(left, bottom), new DiagramPoint(right, top));
		}
		public static ZPlaneRectangle MakeRectangle(IList<DiagramPoint> points) {
			if (points.Count == 0)
				return ZPlaneRectangle.Empty;
			double left = points[0].X;
			double right = points[0].X;
			double bottom = points[0].Y;
			double top = points[0].Y;
			for (int i = 1; i < points.Count; i++) {
				DiagramPoint point = points[i];
				if (point.X < left)
					left = point.X;
				else if (point.X > right)
					right = point.X;
				if (point.Y < bottom)
					bottom = point.Y;
				else if (point.Y > top)
					top = point.Y;
			}
			return new ZPlaneRectangle(new DiagramPoint(left, bottom), new DiagramPoint(right, top));
		}
		public static ZPlaneRectangle Intersect(ZPlaneRectangle rect1, ZPlaneRectangle rect2) {
			DiagramPoint p1 = rect1.LeftBottom;
			DiagramPoint p2 = rect2.LeftBottom;
			DiagramPoint leftBottom = new DiagramPoint(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));
			p1 = rect1.RightTop;
			p2 = rect2.RightTop;
			DiagramPoint rightTop = new DiagramPoint(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
			return (rightTop.X <= leftBottom.X || rightTop.Y <= leftBottom.Y) ? null : new ZPlaneRectangle(leftBottom, rightTop);
		}
		public static ZPlaneRectangle Union(ZPlaneRectangle rect1, ZPlaneRectangle rect2) {
			DiagramPoint p1 = rect1.LeftBottom;
			DiagramPoint p2 = rect2.LeftBottom;
			DiagramPoint leftBottom = new DiagramPoint(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
			p1 = rect1.RightTop;
			p2 = rect2.RightTop;
			DiagramPoint rightTop = new DiagramPoint(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));
			return new ZPlaneRectangle(leftBottom, rightTop);
		}
		public static explicit operator ZPlaneRectangle(Rectangle rect) {
			return new ZPlaneRectangle((DiagramPoint)rect.Location, rect.Width, rect.Height);
		}
		public static explicit operator ZPlaneRectangle(RectangleF rect) {
			return new ZPlaneRectangle((DiagramPoint)rect.Location, rect.Width, rect.Height);
		}
		public static explicit operator ZPlaneRectangle(GRect2D rect) {
			return new ZPlaneRectangle(new DiagramPoint(rect.Left, rect.Top), rect.Width, rect.Height);
		}
		public static explicit operator Rectangle(ZPlaneRectangle rect) {
			Point leftBottom = (Point)rect.LeftBottom.Point;
			Point rightTop = (Point)rect.RightTop.Point;
			return new Rectangle(leftBottom, new Size(rightTop.X - leftBottom.X, rightTop.Y - leftBottom.Y));
		}
		public static explicit operator RectangleF(ZPlaneRectangle rect) {
			PointF leftBottom = (PointF)rect.LeftBottom.Point;
			PointF rightTop = (PointF)rect.RightTop.Point;
			return new RectangleF(leftBottom, new SizeF(rightTop.X - leftBottom.X, rightTop.Y - leftBottom.Y));
		}
		public static explicit operator GRect2D(ZPlaneRectangle rect) {
			return GraphicUtils.ConvertRect((Rectangle)rect);
		}
		protected ZPlaneRectangle() : base() {
		}
		public ZPlaneRectangle(ZPlaneRectangle rect)
			: base(rect.LeftBottom, rect.RightTop) {
		}
		public ZPlaneRectangle(DiagramPoint p1, DiagramPoint p2)
			: base(p1, p2) {
		}
		public ZPlaneRectangle(DiagramPoint point, double width, double height)
			: base(point, width, height) {
		}
		public ZPlaneRectangle(DiagramPoint point, double width, double height, Box box)
			: base(point, width, height, box) {
		}
		public ZPlaneRectangle(DiagramPoint point, double width, double height, BoxPlane boxPlane)
			: base(point, width, height, boxPlane) {
		}
		protected override bool ArePointsCorrect(DiagramPoint p1, DiagramPoint p2) {
			return p1.Z == p2.Z;
		}
		protected override DiagramPoint AbsoluteToRelative(DiagramPoint p) {
			return new DiagramPoint(p.X, p.Y, p.Z);
		}
		protected override DiagramPoint RelativeToAbsolute(DiagramPoint p) {
			return new DiagramPoint(p.X, p.Y, p.Z);
		}
		protected override DiagramVector RelativeToAbsolute(DiagramVector v) {
			return new DiagramVector(v.DX, v.DY, v.DZ);
		}
		protected override PlanePrimitive CreateInstance() {
			return new ZPlaneRectangle();
		}
		public bool AreWidthAndHeightPositive(){
		   return (Width > 0 && Height > 0);
		}
		public Rectangle RoundToRectangle() {
			int x = MathUtils.StrongRound(Location.X);
			int y = MathUtils.StrongRound(Location.Y);
			int width = MathUtils.StrongRound(Width);
			int height = MathUtils.StrongRound(Height);
			return new Rectangle(x, y, width, height);
		}
	}
}
