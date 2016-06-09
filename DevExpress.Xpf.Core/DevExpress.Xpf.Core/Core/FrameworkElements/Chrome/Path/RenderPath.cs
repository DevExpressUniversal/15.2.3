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
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Core.Native {
	public class RenderPath : FrameworkRenderElement {
		Geometry data;
		Stretch stretch;
		Brush fill;
		Brush stroke;
		double strokeThickness;
		PenLineCap strokeStartLineCap;
		PenLineCap strokeEndLineCap;
		PenLineCap strokeDashCap;
		PenLineJoin strokeLineJoin;
		double strokeMiterLimit;
		double strokeDashOffset;
		DoubleCollection strokeDashArray;
		public Stretch Stretch {
			get { return stretch; }
			set { SetProperty(ref stretch, value); }
		}
		public Geometry Data {
			get { return data; }
			set { SetProperty(ref data, value.With(x => (Geometry)x.GetCurrentValueAsFrozen())); }
		}
		public Brush Fill {
			get { return fill; }
			set { SetProperty(ref fill, value.With(x => (Brush)x.GetCurrentValueAsFrozen())); }
		}
		public Brush Stroke {
			get { return stroke; }
			set { SetProperty(ref stroke, value.With(x => (Brush)x.GetCurrentValueAsFrozen())); }
		}
		public double StrokeThickness {
			get { return strokeThickness; }
			set { SetProperty(ref strokeThickness, value); }
		}
		public PenLineCap StrokeStartLineCap {
			get { return strokeStartLineCap; }
			set { SetProperty(ref strokeStartLineCap, value); }
		}
		public PenLineCap StrokeEndLineCap {
			get { return strokeEndLineCap; }
			set { SetProperty(ref strokeEndLineCap, value); }
		}
		public PenLineCap StrokeDashCap {
			get { return strokeDashCap; }
			set { SetProperty(ref strokeDashCap, value); }
		}
		public PenLineJoin StrokeLineJoin {
			get { return strokeLineJoin; }
			set { SetProperty(ref strokeLineJoin, value); }
		}
		public double StrokeMiterLimit {
			get { return strokeMiterLimit; }
			set { SetProperty(ref strokeMiterLimit, value); }
		}
		public double StrokeDashOffset {
			get { return strokeDashOffset; }
			set { SetProperty(ref strokeDashOffset, value); }
		}
		public DoubleCollection StrokeDashArray {
			get { return strokeDashArray; }
			set { SetProperty(ref strokeDashArray, value); }
		}
		bool IsPenNoOp(RenderPathContext context) {
			return context.ActualStroke == null || double.IsNaN(context.ActualStrokeThickness) || context.ActualStrokeThickness.IsZero();
		}
		double GetStrokeThickness(RenderPathContext context) {
			return IsPenNoOp(context) ? 0.0 : Math.Abs(context.ActualStrokeThickness);
		}
		public RenderPath() {
		}
		protected override Size MeasureOverride(Size availableSize, IFrameworkRenderElementContext context) {
			var pContext = (RenderPathContext)context;
			Geometry definingGeometry = pContext.ActualData;
			Size size = pContext.ActualStretch != Stretch.None ? GetStretchedRenderSize(pContext.ActualStretch, GetStrokeThickness(pContext), availableSize, definingGeometry.Bounds) : GetNaturalSize(pContext);
			if (SizeIsInvalidOrEmpty(size)) {
				size = new Size(0.0, 0.0);
				pContext.RenderedGeometry = Geometry.Empty;
			}
			return size;
		}
		internal bool SizeIsInvalidOrEmpty(Size size) {
			if (!size.Width.IsNotNumber() && !size.Height.IsNotNumber())
				return size.IsEmpty;
			return true;
		}
		Pen GetPen(RenderPathContext context) {
			if (IsPenNoOp(context))
				return null;
			if (context.Pen == null) {
				double num = Math.Abs(context.ActualStrokeThickness);
				context.Pen = new Pen();
				context.Pen.Thickness = num;
				context.Pen.Brush = context.ActualStroke;
				context.Pen.StartLineCap = context.ActualStrokeStartLineCap;
				context.Pen.EndLineCap = context.ActualStrokeEndLineCap;
				context.Pen.DashCap = context.ActualStrokeDashCap;
				context.Pen.LineJoin = context.ActualStrokeLineJoin;
				context.Pen.MiterLimit = context.ActualStrokeMiterLimit;
				DoubleCollection doubleCollection = null;
				if (context.ActualStrokeDashArray != null)
					doubleCollection = context.ActualStrokeDashArray;
				double strokeDashOffset = context.ActualStrokeDashOffset;
				if (doubleCollection != null || strokeDashOffset.AreClose(0d))
					context.Pen.DashStyle = new DashStyle(doubleCollection, strokeDashOffset);
				context.Pen.Freeze();
			}
			return context.Pen;
		}
		Size GetNaturalSize(RenderPathContext context) {
			Geometry definingGeometry = context.ActualData;
			Pen pen = GetPen(context);
			Rect renderBounds = definingGeometry.GetRenderBounds(pen);
			return new Size(Math.Max(renderBounds.Right, 0.0), Math.Max(renderBounds.Bottom, 0.0));
		}
		internal Size GetStretchedRenderSize(Stretch mode, double strokeThickness, Size availableSize, Rect geometryBounds) {
			double xScale;
			double yScale;
			double dX;
			double dY;
			Size stretchedSize;
			GetStretchMetrics(mode, strokeThickness, availableSize, geometryBounds, out xScale, out yScale, out dX, out dY, out stretchedSize);
			return stretchedSize;
		}
		void GetStretchMetrics(Stretch mode, double strokeThickness, Size availableSize, Rect geometryBounds, out double xScale, out double yScale, out double dX, out double dY, out Size stretchedSize) {
			if (!geometryBounds.IsEmpty) {
				double num = strokeThickness / 2.0;
				bool shouldSkip = false;
				xScale = Math.Max(availableSize.Width - strokeThickness, 0.0);
				yScale = Math.Max(availableSize.Height - strokeThickness, 0.0);
				dX = num - geometryBounds.Left;
				dY = num - geometryBounds.Top;
				if (geometryBounds.Width > xScale * 4.94065645841247E-324) {
					xScale /= geometryBounds.Width;
				}
				else {
					xScale = 1.0;
					if (geometryBounds.Width.AreClose(0d))
						shouldSkip = true;
				}
				if (geometryBounds.Height > yScale * 4.94065645841247E-324) {
					yScale /= geometryBounds.Height;
				}
				else {
					yScale = 1.0;
					if (geometryBounds.Height.AreClose(0d))
						shouldSkip = true;
				}
				if (mode != Stretch.Fill && !shouldSkip) {
					if (mode == Stretch.Uniform) {
						if (yScale > xScale)
							yScale = xScale;
						else
							xScale = yScale;
					}
					else if (xScale > yScale)
						yScale = xScale;
					else
						xScale = yScale;
				}
				stretchedSize = new Size(geometryBounds.Width * xScale + strokeThickness, geometryBounds.Height * yScale + strokeThickness);
			}
			else {
				xScale = yScale = 1.0;
				dX = dY = 0.0;
				stretchedSize = new Size(0.0, 0.0);
			}
		}
		protected override Size ArrangeOverride(Size finalSize, IFrameworkRenderElementContext context) {
			var pContext = (RenderPathContext)context;
			Geometry definingGeometry = pContext.ActualData;
			Size size;
			if (pContext.ActualStretch == Stretch.None) {
				pContext.RenderedGeometry = null;
				pContext.StretchMatrix = null;
				size = finalSize;
			}
			else
				size = GetStretchedRenderSizeAndSetStretchMatrix(pContext, pContext.ActualStretch, GetStrokeThickness(pContext), finalSize, definingGeometry.Bounds);
			if (SizeIsInvalidOrEmpty(size)) {
				size = new Size(0.0, 0.0);
				pContext.RenderedGeometry = Geometry.Empty;
			}
			return size;
		}
		internal Size GetStretchedRenderSizeAndSetStretchMatrix(RenderPathContext context, Stretch mode, double strokeThickness, Size availableSize, Rect geometryBounds) {
			double xScale;
			double yScale;
			double dX;
			double dY;
			Size stretchedSize;
			GetStretchMetrics(mode, strokeThickness, availableSize, geometryBounds, out xScale, out yScale, out dX, out dY, out stretchedSize);
			Matrix identity = Matrix.Identity;
			identity.ScaleAt(xScale, yScale, geometryBounds.Location.X, geometryBounds.Location.Y);
			identity.Translate(dX, dY);
			context.StretchMatrix = identity;
			context.RenderedGeometry = null;
			return stretchedSize;
		}
		protected override void RenderOverride(DrawingContext dc, IFrameworkRenderElementContext context) {
			RenderPathContext pContext = (RenderPathContext)context;
			EnsureRenderedGeometry(pContext);
			if (Equals(pContext.RenderedGeometry, Geometry.Empty))
				return;
			dc.DrawGeometry(pContext.ActualFill, GetPen(pContext), pContext.RenderedGeometry);
			base.RenderOverride(dc, context);
		}
		void EnsureRenderedGeometry(RenderPathContext context) {
			if (context.RenderedGeometry != null)
				return;
			Geometry definingGeometry = context.ActualData;
			context.RenderedGeometry = definingGeometry;
			if (context.ActualStretch == Stretch.None)
				return;
			Geometry geometry = context.RenderedGeometry.CloneCurrentValue();
			context.RenderedGeometry = !ReferenceEquals(context.RenderedGeometry, geometry) ? geometry : geometry.Clone();
			Transform transform = context.RenderedGeometry.Transform;
			Matrix? boxedMatrix = context.StretchMatrix;
			Matrix matrix = boxedMatrix == null ? Matrix.Identity : boxedMatrix.Value;
			if (transform == null || Equals(transform, Transform.Identity))
				context.RenderedGeometry.Transform = new MatrixTransform(matrix);
			else
				context.RenderedGeometry.Transform = new MatrixTransform(transform.Value * matrix);
		}
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderPathContext(this);
		}
	}
}
