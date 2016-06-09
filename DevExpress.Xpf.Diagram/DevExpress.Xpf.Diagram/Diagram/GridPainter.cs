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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Diagram.Native;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Diagram {
	public class GridPainter : FrameworkElement {
		public static readonly DependencyProperty GridSizeProperty;
		public static readonly DependencyProperty LineBrushProperty;
		public static readonly DependencyProperty MeasureUnitProperty;
		public static readonly DependencyProperty OffsetProperty;
		public static readonly DependencyProperty LocationProperty;
		public static readonly DependencyProperty ViewportProperty;
		public static readonly DependencyProperty ZoomFactorProperty;
		static GridPainter() {
			DependencyPropertyRegistrator<GridPainter>.New()
				.Register(d => d.GridSize, out GridSizeProperty, default(Size?), x => x.InvalidateDrawing())
				.Register(d => d.LineBrush, out LineBrushProperty, default(Brush), x => x.InvalidateDrawing())
				.Register(d => d.MeasureUnit, out MeasureUnitProperty, MeasureUnits.Pixels, x => x.InvalidateVisual())
				.Register(d => d.Offset, out OffsetProperty, default(Point), x => x.InvalidateDrawing())
				.Register(d => d.Location, out LocationProperty, default(Point), x => x.InvalidateDrawing())
				.Register(d => d.Viewport, out ViewportProperty, default(Size), x => x.InvalidateDrawing())
				.Register(d => d.ZoomFactor, out ZoomFactorProperty, 1, x => x.InvalidateDrawing())
			;
		}
		const int LineThickness = 1;
		const double MediumLineOpactity = 0.5;
		public Size? GridSize {
			get { return (Size?)GetValue(GridSizeProperty); }
			set { SetValue(GridSizeProperty, value); }
		}
		public Brush LineBrush {
			get { return (Brush)GetValue(LineBrushProperty); }
			set { SetValue(LineBrushProperty, value); }
		}
		public MeasureUnit MeasureUnit {
			get { return (MeasureUnit)GetValue(MeasureUnitProperty); }
			set { SetValue(MeasureUnitProperty, value); }
		}
		public Point Offset {
			get { return (Point)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		public Point Location{
			get { return (Point)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		public Size Viewport {
			get { return (Size)GetValue(ViewportProperty); }
			set { SetValue(ViewportProperty, value); }
		}
		public double ZoomFactor {
			get { return (double)GetValue(ZoomFactorProperty); }
			set { SetValue(ZoomFactorProperty, value); }
		}
		protected Pen Pen { get { return pen.Value; } }
		public GridPainter() {
			InvalidatePen();
		}
		protected override void OnRender(DrawingContext drawingContext) {
			var drawingArea = Rect.Intersect(new Rect(Offset.OffsetPoint(Location.InvertPoint()), Viewport), new Rect(RenderSize));
			RulerRenderHelper.DrawGrid(MeasureUnit, GridSize, drawingArea, ZoomFactor, true, (line) => {
				DrawLine(drawingContext, line);
			});
			drawingContext.PushOpacity(MediumLineOpactity);
			RulerRenderHelper.DrawGrid(MeasureUnit, GridSize, drawingArea, ZoomFactor, false, (line) => {
				DrawLine(drawingContext, line);
			});
			drawingContext.Pop();
		}
		void DrawLine(DrawingContext drawingContext, AxisLine line) {
			var x = line.CorrectForRender();
			drawingContext.DrawLine(Pen, x.From, x.To);
		}
		protected virtual void InvalidatePen() {
			pen = new Lazy<Pen>(() => new Pen(LineBrush, LineThickness).Do(x => x.Freeze()));
		}
		void InvalidateDrawing() {
			InvalidatePen();
			InvalidateVisual();
		}
		Lazy<Pen> pen;
	}
	public class PageBorderControl : FrameworkElement {
		public static readonly DependencyProperty PageSizeProperty;
		public static readonly DependencyProperty PenProperty;
		public static readonly DependencyProperty ZoomFactorProperty;
		static PageBorderControl() {
			DependencyPropertyRegistrator<PageBorderControl>.New()
			  .Register(d => d.PageSize, out PageSizeProperty, default(Size), FrameworkPropertyMetadataOptions.AffectsRender)
			  .Register(d => d.Pen, out PenProperty, default(Pen), FrameworkPropertyMetadataOptions.AffectsRender)
			  .Register(d => d.ZoomFactor, out ZoomFactorProperty, 0d, FrameworkPropertyMetadataOptions.AffectsRender);
		}
		public Size PageSize {
			get { return (Size)GetValue(PageSizeProperty); }
			set { SetValue(PageSizeProperty, value); }
		}
		public double ZoomFactor {
			get { return (double)GetValue(ZoomFactorProperty); }
			set { SetValue(ZoomFactorProperty, value); }
		}
		public Pen Pen {
			get { return (Pen)GetValue(PenProperty); }
			set { SetValue(PenProperty, value); }
		}
		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			var delta = PageSize.ScaleSize(ZoomFactor);
			var dif = MathHelper.GetDifference(delta, new Size(Math.Ceiling(delta.Width), Math.Ceiling(delta.Height)));
			double maxWidth = RenderSize.Width - dif.X;
			double maxHeight = RenderSize.Height - dif.Y;
			for(double i = delta.Width; Math.Ceiling(i) < maxWidth; i = i + delta.Width) {
				var line = new AxisLine(new Point(i, 0), RenderSize.Height, Orientation.Vertical).CorrectForRender();
				drawingContext.DrawLine(Pen, line.From, line.To);
			}
			for(double i = delta.Height; Math.Ceiling(i) < maxHeight; i = i + delta.Height) {
				var line = new AxisLine(new Point(0, i), RenderSize.Width, Orientation.Horizontal).CorrectForRender();
				drawingContext.DrawLine(Pen, line.From, line.To);
			}
		}
	}
}
