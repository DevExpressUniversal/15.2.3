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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Xpf.Bars.Internal;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.Editors.Flyout;
using DevExpress.Xpf.PdfViewer.Internal;
namespace DevExpress.Xpf.PdfViewer {
	public class PdfPageWrapper : PageWrapper {
		public PdfPageWrapper(PdfPageViewModel page) : base(page) { }
		public PdfPageWrapper(IEnumerable<PdfPageViewModel> pages) : base(pages) { }
		public PdfPoint CalcTopLeftAngle(double angle, int pageIndex) {
			return ((PdfPageViewModel)Pages.Single(x => x.PageIndex == pageIndex)).CalcTopLeftPoint(angle);
		}
		public RenderItem RenderItem { get; internal set; }
		internal double PageWrapperWidth { get; set; }
		internal double PageWrapperTwoPageCenter { get; set; }
		internal double PageWrapperMargin { get; set; }
		protected internal override double CalcFirstPageLeftOffset() {
			if (IsCoverPage)
				return PageWrapperTwoPageCenter * ZoomFactor + HorizontalPageSpacing;
			if (IsColumnMode)
				return (PageWrapperTwoPageCenter - (IsVertical ? Pages.First().PageSize.Width : Pages.First().PageSize.Height)) * ZoomFactor;
			return base.CalcFirstPageLeftOffset();
		}
		protected override double CalcPageTopOffset(IPage page) {
			return ((PageSize.Height - (IsVertical ? page.PageSize.Height : page.PageSize.Width)) / 2d) * ZoomFactor;
		}
		protected override Size CalcRenderSize() {
			Size renderSize = base.CalcRenderSize();
			if (renderSize.Width.LessThan(PageWrapperWidth * ZoomFactor + (IsCoverPage ? PageWrapperMargin / 2d : PageWrapperMargin)))
				return new Size(PageWrapperWidth * ZoomFactor + (IsCoverPage ? PageWrapperMargin / 2d : PageWrapperMargin), renderSize.Height);
			return renderSize;
		}
		protected override Size CalcPageSize() {
			var pageSize = base.CalcPageSize();
			return new Size(PageWrapperWidth.IsZero() ? pageSize.Width : PageWrapperWidth, pageSize.Height);
		}
		internal void InitializeInternal() {
			Initialize();
		}
	}
	public class PdfPageControl : PageControl, IVisualOwner, ILogicalOwner {
		public static readonly DependencyProperty PagesProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		static PdfPageControl() {
			Type ownerType = typeof(PdfPageControl);
			PagesProperty = DependencyProperty.Register("Pages", typeof(IEnumerable<PdfPageViewModel>), ownerType, new FrameworkPropertyMetadata(null));
			IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), ownerType, new PropertyMetadata(false));
		}
		public IEnumerable<PdfPageViewModel> Pages {
			get { return (IEnumerable<PdfPageViewModel>)GetValue(PagesProperty); }
			set { SetValue(PagesProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		VisualChildrenContainer VCContainer { get; set; }
		LogicalChildrenContainer LCContainer { get; set; }
		FrameworkElement editor;
		ContentControl flyoutContentControl;
		FlyoutControl flyoutControl;
		protected override int VisualChildrenCount { get { return base.VisualChildrenCount + VCContainer.VisualChildrenCount; } }
		protected override Visual GetVisualChild(int index) {
			return index < base.VisualChildrenCount ? base.GetVisualChild(index) : VCContainer.GetVisualChild(index - base.VisualChildrenCount);
		}
		protected override IEnumerator LogicalChildren { get { return new MergedEnumerator(VCContainer.GetEnumerator(), base.LogicalChildren, LCContainer.GetEnumerator()); } }
		public PdfPageControl() {
			DefaultStyleKey = typeof(PdfPageControl);
			VCContainer = new VisualChildrenContainer(this, this);
			LCContainer = new LogicalChildrenContainer(this);
			RequestBringIntoView += PdfPageControl_RequestBringIntoView;
			flyoutControl = new FlyoutControl { AllowMoveAnimation = false, AnimationDuration = TimeSpan.Zero, HorizontalAlignment = HorizontalAlignment.Center };
			flyoutContentControl = new ContentControl();
			flyoutContentControl.Content = flyoutControl;
			flyoutControl.PlacementTarget = flyoutContentControl;
			VCContainer.AddChild(flyoutContentControl);
			LCContainer.AddLogicalChild(flyoutContentControl);
		}
		void PdfPageControl_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e) {
			e.Handled = true;
		}
		protected override Size MeasureOverride(Size constraint) {
			if (editor != null) {
				Tuple<Func<Rect>, double> tuple = (Tuple<Func<Rect>, double>)editor.Tag;
				Rect rect = tuple.Item1();
				double angle = tuple.Item2;
				Size measureSize = (angle % 180) == 0 ? rect.Size : new Size(rect.Size.Height, rect.Size.Width);
				editor.Measure(measureSize);
			}
			var flyoutRectHandler = flyoutContentControl.Tag as Func<Rect>;
			if (flyoutRectHandler != null)
				flyoutContentControl.Measure(flyoutRectHandler().Size);
			return base.MeasureOverride(constraint);
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			if (editor != null) {
				Tuple<Func<Rect>, double> tuple = (Tuple<Func<Rect>, double>)editor.Tag;
				Rect rect = tuple.Item1();
				double angle = tuple.Item2;
				editor.RenderTransform = CalcRenderTransform(angle, rect);
			} 
			var flyoutRectHandler = flyoutContentControl.Tag as Func<Rect>;
			if (flyoutRectHandler != null)
				flyoutContentControl.Arrange(flyoutRectHandler());
			return base.ArrangeOverride(arrangeBounds);
		}
		TransformGroup CalcRenderTransform(double angle, Rect rect) {
			bool isHorizontal = (angle % 180) == 0;
			Size arrangeSize = isHorizontal ? rect.Size : new Size(rect.Size.Height, rect.Size.Width);
			editor.Arrange(new Rect(rect.TopLeft, arrangeSize));
			editor.RenderTransform = new RotateTransform(angle);
			TransformGroup group = new TransformGroup();
			RotateTransform rotate = new RotateTransform(angle);
			@group.Children.Add(rotate);
			Point translatePoint = CalcTranslatePoint(arrangeSize, angle);
			TranslateTransform tr = new TranslateTransform(translatePoint.X, translatePoint.Y);
			@group.Children.Add(tr);
			return @group;
		}
		Point CalcTranslatePoint(Size arrangeSize, double angle) {
			if (angle.AreClose(0d))
				return new Point(0, 0);
			if (angle.AreClose(90d))
				return new Point(arrangeSize.Height, 0);
			if (angle.AreClose(180d))
				return new Point(arrangeSize.Width, arrangeSize.Height);
			if (angle.AreClose(270d))
				return new Point(0, arrangeSize.Width);
			return new Point(0, 0);
		}
		protected override void OnRender(DrawingContext dc) {
			base.OnRender(dc);
			if (Pages != null)
				Pages.ForEach(page => page.RenderContent.Do(x => x.ForEach(element => element.Render(dc, RenderSize))));
		}
		public void AddEditor(FrameworkElement editor, Func<Rect> rectHandler, double angle) {
			this.editor = editor;
			VCContainer.AddChild(editor);
			LCContainer.AddLogicalChild(editor);
			editor.Tag = new Tuple<Func<Rect>, double>(rectHandler, angle);
			InvalidateMeasure();
		}
		public void RemoveEditor() {
			if (editor == null)
				return;
			LCContainer.RemoveLogicalChild(editor);
			VCContainer.RemoveChild(editor);
		}
		public void HidePopup() {
			flyoutControl.IsOpen = false;
		}
		public void ShowPopup(string title, string text, Func<Rect> controlRectHandler) {
			flyoutContentControl.Tag = controlRectHandler;
			InvalidateMeasure();
			var controlRect = controlRectHandler();
			var cursorPosition = Mouse.GetPosition(this);
			if (!controlRect.IsInside(cursorPosition))
				return;
			var superTip = CreateSuperTipControl(title, text);
			flyoutControl.Content = superTip;
			flyoutControl.IsOpen = true;
		}
		SuperTipControl CreateSuperTipControl(string title, string text) {
			var sp = new SuperTip();
			if (!string.IsNullOrEmpty(title)) {
				SuperTipHeaderItem h = new SuperTipHeaderItem() { Content = title };
				sp.Items.Add(h);
			}
			SuperTipItem item = new SuperTipItem() { Content = text };
			sp.Items.Add(item);
			return new SuperTipControl(sp);
		}
		#region IVisualOwner
		void IVisualOwner.AddChild(Visual child) {
			AddVisualChild(child);
		}
		void IVisualOwner.RemoveChild(Visual child) {
			RemoveVisualChild(child);
		}
		#endregion
		void ILogicalOwner.AddChild(object child) {
			AddLogicalChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			RemoveLogicalChild(child);
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new PdfPageControlItem();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is PdfPageControlItem;
		}
	}
	public class PdfPageControlItem : PageControlItem {
		public PdfPageControlItem() {
			DefaultStyleKey = typeof(PdfPageControlItem);
		}
	}
	public abstract class PdfElement {
		public abstract void Render(DrawingContext dc, Size renderSize);
	}
	public class PdfGeometry : PdfElement {
		public Brush Brush { get; private set; }
		public IEnumerable<PdfPoint> Vertices { get; private set; }
		public PdfPageViewModel Page { get; private set; }
		public double Angle { get; private set; }
		public double ZoomFactor { get; private set; }
		Geometry geometry;
		public PdfGeometry(Brush brush, PdfPageViewModel page, double zoomFactor, double angle, IEnumerable<PdfPoint> vertices) {
			Brush = brush;
			Vertices = vertices;
			Page = page;
			Angle = angle;
			ZoomFactor = zoomFactor;
			geometry = GenerateGeometry();
			geometry.Freeze();
		}
		Geometry GenerateGeometry() {
			var points = Vertices.ToList();
			if (points.Count < 2)
				return null;
			Point start = Page.GetPoint(points[0], ZoomFactor, Angle);
			List<LineSegment> segments = new List<LineSegment>();
			for (int i = 1; i < points.Count; i++) {
				var point = Page.GetPoint(points[i], ZoomFactor, Angle);
				segments.Add(new LineSegment(point, true));
			}
			PathFigure figure = new PathFigure(start, segments, true);
			PathGeometry geometry = new PathGeometry();
			geometry.Figures.Add(figure);
			return geometry;
		}
		public override void Render(DrawingContext dc, Size renderSize) {
			if (geometry != null)
				dc.DrawGeometry(Brush, new Pen(Brush, 1), geometry);
		}
	}
	public class PdfCombinedGeometry : PdfElement {
		public Brush Brush { get; private set; }
		public IEnumerable<PdfOrientedRectangle> Rectangles { get; private set; }
		public PdfPageViewModel Page { get; private set; }
		public double Angle { get; private set; }
		public double ZoomFactor { get; private set; }
		Geometry geometry;
		public PdfCombinedGeometry(Brush brush, PdfPageViewModel page, double zoomFactor, double angle, IEnumerable<PdfOrientedRectangle> rectangles) {
			Brush = brush;
			Rectangles = rectangles;
			Page = page;
			Angle = angle;
			ZoomFactor = zoomFactor;
			geometry = GenerateGeometry();
		}
		Geometry GenerateGeometry() {
			CombinedGeometry combinedGeometry = new CombinedGeometry() { GeometryCombineMode = GeometryCombineMode.Union };
			foreach (var rect in Rectangles) {
				Geometry geometry = GetRectangleGeometry(rect);
				combinedGeometry = new CombinedGeometry(GeometryCombineMode.Union, combinedGeometry, geometry);
			}
			return combinedGeometry;
		}
		Geometry GetRectangleGeometry(PdfOrientedRectangle rectangle) {
			var points = rectangle.Vertices;
			Point start = Page.GetPoint(points[0], ZoomFactor, Angle);
			List<LineSegment> segments = new List<LineSegment>();
			for (int i = 1; i < points.Count; i++) {
				var point = Page.GetPoint(points[i], ZoomFactor, Angle);
				segments.Add(new LineSegment(point, true));
			}
			PathFigure figure = new PathFigure(start, segments, true);
			PathGeometry geometry = new PathGeometry();
			geometry.Figures.Add(figure);
			return geometry;
		}
		public override void Render(DrawingContext dc, Size renderSize) {
			if (geometry != null)
				dc.DrawGeometry(Brush, new Pen(Brush, 1), geometry);
		}
	}
	public class PdfCaretElement : PdfElement {
		readonly TimeSpan CaretHideTimespan = TimeSpan.FromMilliseconds(300);
		public PdfCaret Caret { get; private set; }
		public PdfPageWrapper PageWrapper { get; private set; }
		public double ZoomFactor { get; private set; }
		public double RotateAngle { get; private set; }
		public Brush CaretBrush { get; private set; }
		Pen pen;
		public PdfCaretElement(Brush caretBrush, PdfPageWrapper pageWrapper, double zoomFactor, double rotateAngle, PdfCaret caret) {
			Caret = caret;
			PageWrapper = pageWrapper;
			ZoomFactor = zoomFactor;
			RotateAngle = rotateAngle;
			CaretBrush = caretBrush;
			InitPen();
		}
		void InitPen() {
			pen = new Pen(CaretBrush, 1d);
			DoubleAnimation doubleAnimation = new DoubleAnimation(1, 0, new Duration(CaretHideTimespan)) { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };
			AnimationClock animationClock = doubleAnimation.CreateClock();
			pen.ApplyAnimationClock(Pen.ThicknessProperty, animationClock);
		}
		public override void Render(DrawingContext dc, Size renderSize) {
			PdfPoint anchorPoint = Caret.ViewData.TopLeft;
			PdfPoint point = new PdfPoint(anchorPoint.X, anchorPoint.Y - Caret.ViewData.Height);
			if (!Caret.ViewData.Angle.AreClose(0d)) {
				double alpha = Caret.ViewData.Angle;
				double x = anchorPoint.X + Math.Cos(alpha) * (point.X - anchorPoint.X) - Math.Sin(alpha) * (point.Y - anchorPoint.Y);
				double y = anchorPoint.Y + Math.Sin(alpha) * (point.X - anchorPoint.X) + Math.Cos(alpha) * (point.Y - anchorPoint.Y);
				point = new PdfPoint(x, y);
			}
			PdfPageViewModel page = (PdfPageViewModel)PageWrapper.Pages.Single(x => x.PageIndex == Caret.Position.PageIndex);
			var pageRect = PageWrapper.GetPageRect(page);
			Point startPointInternal = page.GetPoint(anchorPoint, ZoomFactor, RotateAngle);
			Point endPointInternal = page.GetPoint(point, ZoomFactor, RotateAngle);
			Point startPoint = new Point(startPointInternal.X + pageRect.Left, startPointInternal.Y + pageRect.Top);
			Point endPoint = new Point(endPointInternal.X + pageRect.Left, endPointInternal.Y + pageRect.Top);
			dc.DrawLine(pen, startPoint, endPoint);
		}
	}
}
