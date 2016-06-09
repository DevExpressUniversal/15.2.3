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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Controls.Internal {
	public class BookTransformBuilder {
		#region Property
		protected internal virtual double RotationAngle { get { return 0.0; } }
		protected internal virtual double RotationX { get { return 0.0; } }
		protected internal virtual double RotationY { get { return 0.0; } }
		protected internal virtual double TranslationX { get { return 0.0; } }
		protected internal virtual double TranslationY { get { return 0.0; } }
		protected internal double BaseForeShadowWidth { get { return Params.BaseForeShadowWidth; } }
		protected internal double OverlayForeShadowWidth { get { return Params.OverlayForeShadowWidth; } }
		protected internal double BackShadowWidth { get { return Params.BackShadowWidth; } }
		protected internal double DraggingLineAngle { get { return Tracker.DraggingLine.AbscissAngleDeg; } }
		protected internal bool IsNextOdd { get { return PageLayout == BookPageLayout.NextOdd; } }
		protected internal bool IsNextEven { get { return PageLayout == BookPageLayout.NextEven; } }
		protected internal double ShadowRotationPointY { get { return IntersectY - BaseRect.Top - Params.ShadowTop; } }
		protected internal double ActualBackShadowWidth {
			get {
				double distance = Tracker.DragPoint.Distance(Tracker.BaseCornerPoint);
				return Math.Min(BackShadowWidth, distance) - 2;
			}
		}
		protected internal double IntersectX { get { return IntersectPoint.X; } }
		protected internal double IntersectY { get { return IntersectPoint.Y; } }
		protected internal Point IntersectPoint {
			get {
				Point? rawPoint = Tracker.CuttingLine.Intersect(Tracker.BaseLine);
				return rawPoint.HasValue ? (Point)rawPoint : new Point(10000, 10000);
			}
		}
		protected internal Rect BaseRect { get { return Tracker.BasePageRect; } }
		protected internal Book Book { get; set; }
		protected internal BookPageLayout PageLayout { get; set; }
		protected internal BookTemplateElementType PageElement { get; set; }
		protected internal BookGeometryParams Params { get { return Book.GeometryParams; } }
		protected internal BookDragTracker Tracker { get { return Book.DragTracker; } }
		#endregion
		public BookTransformBuilder(Book book, BookPageLayout layout, BookTemplateElementType element) {
			Book = book;
			PageLayout = layout;
			PageElement = element;
		}
	}
	public class BookGridTransformBuilder : BookTransformBuilder {
		public BookGridTransformBuilder(Book book, BookPageLayout layout, BookTemplateElementType element) : base(book, layout, element) { }
		protected internal override double RotationAngle { get { return DraggingLineAngle * 2; } }
		protected internal override double RotationX { get { return BaseRect.Right - IntersectX; } }
		protected internal override double RotationY { get { return IntersectY - BaseRect.Top; } }
		protected internal override double TranslationX { get { return (IntersectX - Tracker.BaseNearCornerPoint.X) * 2; } }
	}
	public class BookBaseForeShadowTransformBuilder : BookTransformBuilder {
		public BookBaseForeShadowTransformBuilder(Book book, BookPageLayout layout, BookTemplateElementType element) : base(book, layout, element) { }
		protected internal override double RotationAngle { get { return DraggingLineAngle; } }
		protected internal override double RotationX { get { return IsNextEven ? 1 : BaseForeShadowWidth; } }
		protected internal override double RotationY { get { return ShadowRotationPointY; } }
		protected internal override double TranslationX { get { return IntersectX - BaseRect.Left - RotationX; } }
	}
	public class BookOverlayForeShadowTransformBuilder : BookTransformBuilder {
		public BookOverlayForeShadowTransformBuilder(Book book, BookPageLayout layout, BookTemplateElementType element) : base(book, layout, element) { }
		protected internal override double RotationAngle { get { return -DraggingLineAngle; } }
		protected internal override double RotationX { get { return IsNextOdd ? OverlayForeShadowWidth - 1 : 0; } }
		protected internal override double RotationY { get { return ShadowRotationPointY; } }
		protected internal override double TranslationX { get { return BaseRect.Right - IntersectX - RotationX; } }
	}
	public class BookBackShadowTransformBuilder : BookTransformBuilder {
		public BookBackShadowTransformBuilder(Book book, BookPageLayout layout, BookTemplateElementType element) : base(book, layout, element) { }
		protected internal override double RotationAngle { get { return DraggingLineAngle; } }
		protected internal override double RotationX { get { return IsNextOdd ? ActualBackShadowWidth : BackShadowWidth - ActualBackShadowWidth; } }
		protected internal override double RotationY { get { return ShadowRotationPointY; } }
		protected internal override double TranslationX { get { return IntersectX - BaseRect.Left - RotationX; } }
	}
	public class BookClipBuilder {
		#region Property
		protected internal Book Book { get; set; }
		protected internal BookPageLayout PageLayout { get; set; }
		protected internal BookDragTracker Tracker { get { return Book.DragTracker; } }
		protected internal BookPageManager PageManager { get { return Book.PageManager; } }
		protected internal bool IsPrevView { get { return PageManager.IsPrevView; } }
		protected internal bool IsNextView { get { return PageManager.IsNextView; } }
		#endregion
		public BookClipBuilder(Book book, BookPageLayout layout) {
			Book = book;
			PageLayout = layout;
		}
		public static Geometry CreateClip(Book book, BookPageLayout layout) { return new BookClipBuilder(book, layout).CalcClip(); }
		protected internal Geometry CalcClip() {
			PointCollection points = CalcPoints();
			if(points == null || points.Count == 0)
				return null;
			PolyLineSegment segment = new PolyLineSegment() { Points = points };
			PathFigure figure = new PathFigure() { IsClosed = true, StartPoint = segment.Points[0] };
			figure.Segments.Add(segment);
			PathGeometry geometry = new PathGeometry();
			geometry.Figures.Add(figure);
			return geometry;
		}
		protected internal PointCollection CalcPoints() {
			if(IsClip())
				return CalcClipPoints();
			if(IsRectangle())
				return CalcRectanglePoints();
			return null;
		}
		protected internal PointCollection CalcClipPoints() {
			RectCorner baseCorner = Tracker.BaseCorner;
			RectCorner primaryCorner = baseCorner.DiagonalMirror();
			RectCorner secondaryCorner = baseCorner.HorizontalMirror();
			BookPageTruncator truncator = new BookPageTruncator(Tracker.BasePageRect, Tracker.CuttingLine, primaryCorner, secondaryCorner);
			bool isBasePage = PageLayout == BookPageLayout.Even || PageLayout == BookPageLayout.Odd;
			return isBasePage ? truncator.BasePageResult : truncator.OverlayPageResult;
		}
		protected internal PointCollection CalcRectanglePoints() {
			BookPageTruncator truncator = new BookPageTruncator(Tracker.BasePageRect);
			return truncator.BasePageResult;
		}
		protected internal bool IsClip() {
			bool prev = IsPrevView && (PageLayout == BookPageLayout.Odd || PageLayout == BookPageLayout.PrevEven);
			bool next = IsNextView && (PageLayout == BookPageLayout.Even || PageLayout == BookPageLayout.NextOdd);
			return prev || next;
		}
		protected internal bool IsRectangle() {
			bool prev = IsPrevView && PageLayout == BookPageLayout.PrevOdd;
			bool next = IsNextView && PageLayout == BookPageLayout.NextEven;
			return prev || next;
		}
	}
	public class BookPageTruncator {
		#region Property
		public PointCollection BasePageResult { get; private set; }
		public PointCollection OverlayPageResult { get; private set; }
		protected internal Rect Rectangle { get; set; }
		protected internal MathLine Line { get; set; }
		#endregion
		public BookPageTruncator(Rect rect) {
			Rectangle = rect;
			BasePageResult = CreateRectanglePoints(true);
		}
		public BookPageTruncator(Rect rect, MathLine line, RectCorner primaryCorner, RectCorner secondaryCorner) {
			Rectangle = rect;
			Line = line;
			if(!Proceed(primaryCorner))
				Proceed(secondaryCorner);
		}
		protected internal bool Proceed(RectCorner corner) {
			RectTruncator truncator = new RectTruncator(Rectangle, Line, corner);
			switch(truncator.ResultType) {
				case RectTruncatorResultType.MindlessCorner:
					return false;
				case RectTruncatorResultType.Path:
					ProceedPaths(truncator);
					return true;
				case RectTruncatorResultType.TangentLine:
					ProceedEdgeResult(Line.IsMoreHorizontal);
					return true;
				default:
					ProceedEdgeResult(true);
					return true;
			}
		}
		protected internal void ProceedEdgeResult(bool isBasePageVisible) {
			BasePageResult = CreateRectanglePoints(isBasePageVisible);
			OverlayPageResult = CreateRectanglePoints(!isBasePageVisible);
		}
		protected internal void ProceedPaths(RectTruncator truncator) {
			BasePageResult = truncator.ResultPoints;
			OverlayPageResult = truncator.InvertedResultPoints;
			MovePointsToNull(BasePageResult);
			MovePointsToNull(OverlayPageResult);
			HorizontalFlipPoints(OverlayPageResult);
		}
		protected internal void MovePointsToNull(PointCollection points) {
			for(int i = 0; i < points.Count; i++)
				points[i] = MovePointToNull(points[i]);
		}
		protected internal Point MovePointToNull(Point point) { return new Point(point.X - Rectangle.Left, point.Y - Rectangle.Top); }
		protected internal void HorizontalFlipPoints(PointCollection points) {
			for(int i = 0; i < points.Count; i++)
				points[i] = HorizontalFlipPoint(points[i]);
		}
		protected internal Point HorizontalFlipPoint(Point point) { return new Point(Rectangle.Width - point.X, point.Y); }
		protected internal PointCollection CreateRectanglePoints(bool isVisible) {
			PointCollection points = new PointCollection();
			points.Add(Rectangle.TopLeft());
			if(isVisible) {
				points.Add(Rectangle.TopRight());
				points.Add(Rectangle.BottomRight());
				points.Add(Rectangle.BottomLeft());
			}
			MovePointsToNull(points);
			return points;
		}
	}
}
