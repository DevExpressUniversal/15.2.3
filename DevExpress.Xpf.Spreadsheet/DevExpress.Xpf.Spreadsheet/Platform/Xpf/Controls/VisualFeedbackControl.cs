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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	#region VisualFeedbackState (abstract class)
	public abstract class VisualFeedbackState {
		#region Properties
		public Brush PenBrush { get; set; }
		public Pen LinePen { get { return GetLinePen(); } }
		#endregion
		protected virtual Pen GetLinePen() {
			return new Pen(PenBrush, 1);
		}
		public abstract void DrawFeedback(DrawingContext dc, VisualFeedbackControl container);
	}
	#endregion
	#region DashStyleVisualFeedbackState (abstract class)
	public abstract class DashStyleVisualFeedbackState : VisualFeedbackState {
		#region Fields
		DashStyle dashStyle;
		#endregion
		#region Properties
		public DashStyle DashStyle {
			get {
				if (dashStyle == null)
					dashStyle = CreateDashStyle();
				return dashStyle;
			}
		}
		#endregion
		protected override Pen GetLinePen() {
			Pen pen = base.GetLinePen();
			pen.DashStyle = DashStyle;
			return pen;
		}
		DashStyle CreateDashStyle() {
			return new DashStyle(new double[] { 2, 4 }, 0d);
		}
	}
	#endregion
	#region ResizeFeedbackState
	public class ResizeFeedbackState : DashStyleVisualFeedbackState {
		int Coordinate { get; set; }
		double ScaleFactor { get; set; }
		bool IsVertical { get; set; }
		public ResizeFeedbackState(bool isVertical, int coordinate, double scaleFactor) {
			ScaleFactor = scaleFactor;
			IsVertical = isVertical;
			Coordinate = coordinate;
		}
		public override void DrawFeedback(DrawingContext dc, VisualFeedbackControl container) {
			Size size = new Size(container.ActualWidth, container.ActualHeight);
			double offset = 0.5;
			Point start = IsVertical ? new Point(Coordinate - offset + 1, 0) : new Point(0, Coordinate - offset + 1);
			Point end = IsVertical ? new Point(Coordinate - offset + 1, size.Height * (1 / ScaleFactor)) :
									 new Point(size.Width * (1 / ScaleFactor), Coordinate - offset + 1);
			dc.DrawLine(LinePen, start, end);
		}
	}
	#endregion
	#region DragRangeVisualFeedbackState
	public class DragRangeVisualFeedbackState : VisualFeedbackState {
		#region Fields
		const int borderWidthInPixels = 3;
		#endregion
		public DragRangeVisualFeedbackState(Rect bounds, Brush brush) {
			Bounds = bounds;
			Brush = brush;
		}
		#region Properties
		Rect Bounds { get; set; }
		Brush Brush { get; set; }
		#endregion
		public override void DrawFeedback(DrawingContext dc, VisualFeedbackControl container) {
			Bounds = PatchBounds(Bounds);
			dc.DrawRectangle(null, LinePen, Bounds);
		}
		protected override Pen GetLinePen() {
			Pen result = new Pen(Brush, borderWidthInPixels);
			result.Freeze();
			return result;
		}
		Rect PatchBounds(Rect bounds) {
			return RectangleExtensions.GetSaveRect(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
		}
	}
	#endregion
	#region CommentFeedbackState
	public class CommentVisualFeedbackState : VisualFeedbackState {
		#region Fields
		Rect bounds;
		#endregion
		public CommentVisualFeedbackState(Rect bounds) {
			this.bounds = bounds;
		}
		public override void DrawFeedback(DrawingContext dc, VisualFeedbackControl container) {
			dc.DrawRectangle(null, LinePen, PatchBounds(bounds));
		}
		Rect PatchBounds(Rect bounds) {
			return new Rect(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
		}
	}
	#endregion
	#region DragFloatingObjectVisualFeedbackState
	public class DragFloatingObjectVisualFeedbackState : VisualFeedbackState {
		public DragFloatingObjectVisualFeedbackState(UIElement content, DragFloatingObjectFeedbackParams p) {
			Container = new Border() { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black };
			Container.Focusable = false;
			content.Focusable = false;
			Container.Child = content;
			SetParams(p);
		}
		private Point CalcCenterPoint(Rect bounds) {
			double x = bounds.Width / 2;
			double y = bounds.Height / 2;
			return new Point(x, y);
		}
		Border Container { get; set; }
		Rect Bounds { get; set; }
		public override void DrawFeedback(DrawingContext dc, VisualFeedbackControl container) {
			container.DrawContent(Container, Bounds);
		}
		public void SetParams(DragFloatingObjectFeedbackParams p) {
			Bounds = p.Bounds;
			Point center = CalcCenterPoint(Bounds);
			Container.RenderTransform = new RotateTransform(p.Angle) { CenterX = center.X, CenterY = center.Y };
			Container.Opacity = 0.5;
		}
	}
	#endregion
	public class VisualFeedbackControl : ContentPresenter {
		public static readonly DependencyProperty LineBrushProperty;
		static VisualFeedbackControl() {
			LineBrushProperty = DependencyProperty.Register("LineBrush", typeof(Brush), typeof(VisualFeedbackControl), new FrameworkPropertyMetadata(Brushes.Black));
		}
		public VisualFeedbackControl() {
			DefaultStyleKey = typeof(VisualFeedbackControl);
			RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
		}
		public Brush LineBrush {
			get { return (Brush)GetValue(LineBrushProperty); }
			set { SetValue(LineBrushProperty, value); }
		}
		VisualFeedbackState State { get; set; }
		public void BeginDrawFeedback(VisualFeedbackState state) {
			State = state;
			InvalidateVisual();
		}
		public void EndDrawFeedback() {
			State = null;
			ContentRect = new Rect();
			Content = null;
			InvalidateMeasure();
			InvalidateVisual();
		}
		protected override void OnRender(DrawingContext dc) {
			base.OnRender(dc);
			if (State != null) {
				State.PenBrush = LineBrush;
				State.DrawFeedback(dc, this);
			}
		}
		Rect ContentRect { get; set; }
		internal void DrawContent(object content, Rect rect) {
			ContentRect = rect;
			Content = content;
			InvalidateMeasure();
		}
		protected override Size MeasureOverride(Size constraint) {
			if (IsContentRectEmpty()) return base.MeasureOverride(constraint);
			UIElement element = Content as UIElement;
			if (element != null)
				element.Measure(ContentRect.Size);
			return constraint;
		}
		private bool IsContentRectEmpty() {
			return ContentRect.Width == 0 || ContentRect.Height == 0;
		}
		private ContentPresenter FindPresenter() {
			return LayoutHelper.FindElementByType(this, typeof(ContentPresenter)) as ContentPresenter;
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			if (IsContentRectEmpty()) return base.ArrangeOverride(arrangeBounds);
			UIElement element = Content as UIElement;
			if (element != null)
				element.Arrange(ContentRect);
			return arrangeBounds;
		}
	}
}
