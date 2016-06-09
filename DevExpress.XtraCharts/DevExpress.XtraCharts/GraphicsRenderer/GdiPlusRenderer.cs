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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class GdiPlusRenderer : IRenderer {
		const int pointsCountForOptimizing = 500;
		const int gdiPlusGenericError = unchecked((int)0x80004005);
		static void RoundPoint(ref PointF point) {
			point.X = MathUtils.StrongRound(point.X);
			point.Y = MathUtils.StrongRound(point.Y);
		}
		static void RoundRectangle(ref RectangleF rect) {
			rect.X = MathUtils.StrongRound(rect.X);
			rect.Y = MathUtils.StrongRound(rect.Y);
			rect.Width = MathUtils.StrongRound(rect.Width);
			rect.Height = MathUtils.StrongRound(rect.Height);
		}
		static RectangleF GetClipRectangle(RectangleF rect, PointF correctPosition, SizeF correctSize) {
			return new RectangleF(rect.X + correctPosition.X, rect.Y + correctPosition.Y, rect.Width + correctSize.Width, rect.Height + correctSize.Height);
		}
		static RectangleF GetEllipseBounds(PointF center, float radius) {
			RectangleF bounds = new RectangleF(center, new Size(0, 0));
			bounds.Inflate(radius, radius);
			return bounds;
		}
		static RectangleF GetEllipseBounds(PointF center, float semiAxisX, float semiAxisY) {
			return RectangleF.Inflate(new RectangleF(center.X, center.Y, 0, 0), semiAxisX, semiAxisY);
		}
		static RectangleF GetBoundsForCircle(PointF[] polygon, PointF centerPoint) {
			PointF point1;
			PointF point2 = centerPoint;
			float radius = float.MinValue;
			foreach (PointF point in polygon) {
				point1 = new PointF(point.X, point.Y);
				float length = (float)Math.Ceiling(MathUtils.CalcLength2D(point1, point2));
				if (length > radius)
					radius = length;
			}
			int x = MathUtils.Ceiling(centerPoint.X - radius);
			int y = MathUtils.Ceiling(centerPoint.Y - radius);
			return new RectangleF(x, y, radius * 2, radius * 2);
		}
		static Rectangle RoundRectangle(RectangleF rect) {
			return new Rectangle(MathUtils.StrongRound(rect.X), MathUtils.StrongRound(rect.Y), MathUtils.StrongRound(rect.Width), MathUtils.StrongRound(rect.Height));
		}
		readonly Stack<GraphicsState> stateCache = new Stack<GraphicsState>();
		readonly Stack<Region> clippingCache = new Stack<Region>();
		readonly Stack<PixelOffsetMode> pixelOffsetCache = new Stack<PixelOffsetMode>();
		readonly Stack<SmoothingMode> antialiasingCache = new Stack<SmoothingMode>();
		readonly Stack<bool> optimizationCache = new Stack<bool>();
		Rectangle viewport;
		Graphics context;
		public Matrix Transform { get { return context.Transform; } }
		public RectangleF CurrentClippingBounds {
			get {
				return context.ClipBounds;
			}
		}
		public bool IsRightToLeft { get; set; }
		void DrawTiledImage(Image source, Rectangle bounds) {
			int srcWidth = source.Width;
			int srcHeight = source.Height;
			int xCount = bounds.Width / srcWidth + 1;
			int yCount = bounds.Height / srcHeight + 1;
			for (int x = 0; x < xCount; x++)
				for (int y = 0; y < yCount; y++) {
					Rectangle tileRect = new Rectangle(x * srcWidth + bounds.X, y * srcHeight + bounds.Y, srcWidth, srcHeight);
					int cut = tileRect.Right - bounds.Right;
					if (cut > 0)
						tileRect.Width -= cut;
					cut = tileRect.Bottom - bounds.Bottom;
					if (cut > 0)
						tileRect.Height -= cut;
					Rectangle sourceRect = tileRect;
					sourceRect.X = 0;
					sourceRect.Y = 0;
					DrawImage(source, tileRect, sourceRect);
				}
		}
		void DrawZoomedImage(Image source, Rectangle bounds) {
			double zoomRatio = Math.Min((double)bounds.Width / source.Width, (double)bounds.Height / source.Height);
			Size imageSize = new Size((int)(source.Width * zoomRatio), (int)(source.Height * zoomRatio));
			Point location = new Point(bounds.Location.X + (bounds.Width - imageSize.Width) / 2, bounds.Location.Y + (bounds.Height - imageSize.Height) / 2);
			DrawImage(source, new Rectangle(location, imageSize));
		}
		void FillRectangleGradient(RectangleF bounds, Color color, Color color2, RectangleGradientMode mode) {
			if (!bounds.AreWidthAndHeightPositive())
				return;
			SetPixelOffsetMode(PixelOffsetMode.Half);
			switch (mode) {
				case RectangleGradientMode.BottomLeftToTopRight:
					using (Brush brush = new LinearGradientBrush(bounds, color2, color, LinearGradientMode.BackwardDiagonal))
						FillRectangle(bounds, brush);
					break;
				case RectangleGradientMode.BottomRightToTopLeft:
					using (Brush brush = new LinearGradientBrush(bounds, color2, color, LinearGradientMode.ForwardDiagonal))
						FillRectangle(bounds, brush);
					break;
				case RectangleGradientMode.TopLeftToBottomRight:
					using (Brush brush = new LinearGradientBrush(bounds, color, color2, LinearGradientMode.ForwardDiagonal))
						FillRectangle(bounds, brush);
					break;
				case RectangleGradientMode.TopRightToBottomLeft:
					using (Brush brush = new LinearGradientBrush(bounds, color, color2, LinearGradientMode.BackwardDiagonal))
						FillRectangle(bounds, brush);
					break;
				case RectangleGradientMode.BottomToTop:
					using (Brush brush = new LinearGradientBrush(bounds, color2, color, LinearGradientMode.Vertical))
						FillRectangle(bounds, brush);
					break;
				case RectangleGradientMode.TopToBottom:
					using (Brush brush = new LinearGradientBrush(bounds, color, color2, LinearGradientMode.Vertical))
						FillRectangle(bounds, brush);
					break;
				case RectangleGradientMode.LeftToRight:
					using (Brush brush = new LinearGradientBrush(bounds, color, color2, LinearGradientMode.Horizontal))
						FillRectangle(bounds, brush);
					break;
				case RectangleGradientMode.RightToLeft:
					using (Brush brush = new LinearGradientBrush(bounds, color2, color, LinearGradientMode.Horizontal))
						FillRectangle(bounds, brush);
					break;
				case RectangleGradientMode.ToCenterHorizontal:
					RectangleF toCenterHorizontalPart = bounds;
					toCenterHorizontalPart.Width = toCenterHorizontalPart.Width / 2;
					using (Brush brush = new LinearGradientBrush(toCenterHorizontalPart, color, color2, LinearGradientMode.Horizontal))
						FillRectangle(toCenterHorizontalPart, brush);
					toCenterHorizontalPart.X += toCenterHorizontalPart.Width;
					toCenterHorizontalPart.Width = bounds.Width - toCenterHorizontalPart.Width;
					using (Brush brush = new LinearGradientBrush(toCenterHorizontalPart, color2, color, LinearGradientMode.Horizontal))
						FillRectangle(toCenterHorizontalPart, brush);
					break;
				case RectangleGradientMode.FromCenterHorizontal:
					RectangleF fromCenterHorizontalPart = bounds;
					fromCenterHorizontalPart.Width = fromCenterHorizontalPart.Width / 2;
					using (Brush brush = new LinearGradientBrush(fromCenterHorizontalPart, color2, color, LinearGradientMode.Horizontal))
						FillRectangle(fromCenterHorizontalPart, brush);
					fromCenterHorizontalPart.X += fromCenterHorizontalPart.Width;
					fromCenterHorizontalPart.Width = bounds.Width - fromCenterHorizontalPart.Width;
					using (Brush brush = new LinearGradientBrush(fromCenterHorizontalPart, color, color2, LinearGradientMode.Horizontal))
						FillRectangle(fromCenterHorizontalPart, brush);
					break;
				case RectangleGradientMode.ToCenterVertical:
					RectangleF toCenterVerticalPart = bounds;
					toCenterVerticalPart.Height = toCenterVerticalPart.Height / 2;
					using (Brush brush = new LinearGradientBrush(toCenterVerticalPart, color, color2, LinearGradientMode.Vertical))
						FillRectangle(toCenterVerticalPart, brush);
					toCenterVerticalPart.Y += toCenterVerticalPart.Height;
					toCenterVerticalPart.Height = bounds.Height - toCenterVerticalPart.Height;
					using (Brush brush = new LinearGradientBrush(toCenterVerticalPart, color2, color, LinearGradientMode.Vertical))
						FillRectangle(toCenterVerticalPart, brush);
					break;
				case RectangleGradientMode.FromCenterVertical:
					RectangleF fromCenterVerticalPart = bounds;
					fromCenterVerticalPart.Height = fromCenterVerticalPart.Height / 2;
					using (Brush brush = new LinearGradientBrush(fromCenterVerticalPart, color2, color, LinearGradientMode.Vertical))
						FillRectangle(fromCenterVerticalPart, brush);
					fromCenterVerticalPart.Y += fromCenterVerticalPart.Height;
					fromCenterVerticalPart.Height = bounds.Height - fromCenterVerticalPart.Height;
					using (Brush brush = new LinearGradientBrush(fromCenterVerticalPart, color, color2, LinearGradientMode.Vertical))
						FillRectangle(fromCenterVerticalPart, brush);
					break;
			}
			RestorePixelOffsetMode();
		}
		void FillCircle(PointF center, float radius, Brush brush) {
			context.FillEllipse(brush, GetEllipseBounds(center, radius));
		}
		void FillEllipse(PointF center, float semiAxisX, float semiAxisY, Brush brush) {
			FillEllipse(GetEllipseBounds(center, semiAxisX, semiAxisY), brush);
		}
		void FillBezier(BezierRangeStrip strip, Brush brush) {
			if (strip.Count == 1) {
				GRealRect2D boundingRectangle = strip.GetBoundingRectangle();
				RectangleF bounds = new RectangleF((float)boundingRectangle.Left, (float)boundingRectangle.Top, (float)boundingRectangle.Width, (float)boundingRectangle.Height);
				context.FillRectangle(brush, bounds);
			} else
				using (GraphicsPath path = new GraphicsPath()) {
					LineStrip topPoints, bottomPoints;
					strip.GetPointsForDrawing(out topPoints, out bottomPoints);
					path.AddBeziers(StripsUtils.Convert(topPoints));
					path.AddBeziers(StripsUtils.Convert(bottomPoints));
					context.FillPath(brush, path);
				}
		}
		Pen CreatePen(Color color, int thickness, LineStyle lineStyle) {
			Pen pen = new Pen(color, thickness);
			if (lineStyle != null) {
				pen.LineJoin = lineStyle.LineJoin;
				DashStyleHelper.ApplyDashStyle(pen, lineStyle.ActualDashStyle);
			}
			return pen;
		}
		protected bool CanDrawSegmentedLine(Pen pen) {
			return pen.Width <= 2 && pen.DashStyle == System.Drawing.Drawing2D.DashStyle.Solid;
		}
		public void Clear(Color color) {
			if (context != null)
				context.Clear(color);
		}
		public void Present() {
			if (stateCache.Count > 0)
				throw new Exception("Renderer state cache not empty on finishing drawing");
			if (clippingCache.Count > 0)
				throw new Exception("Renderer clipping cache not empty on finishing drawing");
			if (pixelOffsetCache.Count > 0)
				throw new Exception("Renderer pixel offset cache not empty on finishing drawing");
			if (antialiasingCache.Count > 0)
				throw new Exception("Antialiasing cache not empty on finishing drawing");
		}
		public void Reset(object graphics, Rectangle bounds) {
			context = graphics as Graphics;
			viewport = bounds;
		}
		public void SetPixelOffsetMode(PixelOffsetMode mode) {
			pixelOffsetCache.Push(context.PixelOffsetMode);
			context.PixelOffsetMode = mode;
		}
		public void RestorePixelOffsetMode() {
			if (pixelOffsetCache.Count > 0)
				context.PixelOffsetMode = pixelOffsetCache.Pop();
			else
				DevExpress.Charts.Native.ChartDebug.Fail("PixelOffsetMode stack corrupted!");
		}
		public void EnableAntialiasing(bool enable) {
			antialiasingCache.Push(context.SmoothingMode);
			context.SmoothingMode = enable ? SmoothingMode.AntiAlias : SmoothingMode.Default;
		}
		public void RestoreAntialiasing() {
			if (antialiasingCache.Count > 0)
				context.SmoothingMode = antialiasingCache.Pop();
			else
				DevExpress.Charts.Native.ChartDebug.Fail("Antialiasing stack corrupted!");
		}
		public void EnablePolygonAntialiasing(bool enable) {
			EnableAntialiasing(enable);
		}
		public void RestorePolygonAntialiasing() {
			RestoreAntialiasing();
		}
		public void EnablePolygonOptimization(bool enable) {
			optimizationCache.Push(enable);
		}
		public void RestorePolygonOptimization() {
			if (optimizationCache.Count > 0)
				optimizationCache.Pop();
			else
				DevExpress.Charts.Native.ChartDebug.Fail("Optimization stack corrupted!");
		}
		public void SaveState() {
			stateCache.Push(context.Save());
		}
		public void RestoreState() {
			if (stateCache.Count > 0)
				context.Restore(stateCache.Pop());
			else
				DevExpress.Charts.Native.ChartDebug.Fail("State stack corrupted!");
		}
		public void TranslateModel(PointF offset) {
			context.TranslateTransform(offset.X, offset.Y);
		}
		public void TranslateModel(float x, float y) {
			context.TranslateTransform(x, y);
		}
		public void RotateModel(float angle) {
			context.RotateTransform(angle);
		}
		public void ProcessHitTestRegion(HitTestController hitTestController, IHitTest hitTestObject, object additionalObject, IHitRegion region) {
			ProcessHitTestRegion(hitTestController, hitTestObject, additionalObject, region, false);
		}
		public void ProcessHitTestRegion(HitTestController hitTestController, IHitTest hitTestObject, object additionalObject, IHitRegion region, bool force) {
			ProcessHitTestRegion(hitTestController, hitTestObject, additionalObject, false, region, force);
		}
		public void ProcessHitTestRegion(HitTestController hitTestController, IHitTest hitTestObject, object additionalObject, bool useSpecificCursor, IHitRegion region, bool force) {
			if ((hitTestController != null) && (hitTestObject != null) && (region != null) && (hitTestController.Enabled || force))
				hitTestController.Register(new HitTestParams(hitTestObject, additionalObject, useSpecificCursor, region));
		}
		public void SetClipping(RectangleF rect) {
			SetClipping(rect, CombineMode.Intersect);
		}
		public void SetClipping(Region region) {
			SetClipping(region, CombineMode.Intersect);
		}
		public void SetClipping(GraphicsPath path) {
			SetClipping(path, CombineMode.Intersect);
		}
		public void SetClipping(RectangleF rect, CombineMode mode) {
			clippingCache.Push(context.Clip);
			RoundRectangle(ref rect);
			context.SetClip(rect, mode);
		}
		public void SetClipping(Region region, CombineMode mode) {
			clippingCache.Push(context.Clip);
			context.SetClip(region, mode);
		}
		public void SetClipping(GraphicsPath path, CombineMode mode) {
			clippingCache.Push(context.Clip);
			context.SetClip(path, mode);
		}
		public void RestoreClipping() {
			if (clippingCache.Count > 0)
				context.SetClip(clippingCache.Pop(), CombineMode.Replace);
			else
				DevExpress.Charts.Native.ChartDebug.Fail("Clipping stack corrupted!");
		}
		public void DrawImage(Image image, RectangleF bounds) {
			if (image == null)
				return;
			RoundRectangle(ref bounds);
			context.DrawImage(image, bounds);
		}
		public void DrawImage(Image image, RectangleF bounds, RectangleF source) {
			if (image == null)
				return;
			RoundRectangle(ref bounds);
			RoundRectangle(ref source);
			context.DrawImage(image, bounds, source, GraphicsUnit.Pixel);
		}
		public void DrawImage(Image image, PointF position) {
			if (image == null)
				return;
			RoundPoint(ref position);
			context.DrawImage(image, position);
		}
		public void DrawImage(Image image, RectangleF bounds, ChartImageSizeMode sizeMode) {
			if (image == null)
				return;
			if (!bounds.AreWidthAndHeightPositive())
				return;
			switch (sizeMode) {
				case ChartImageSizeMode.AutoSize:
				case ChartImageSizeMode.Stretch:
					DrawImage(image, bounds);
					break;
				case ChartImageSizeMode.Tile:
					DrawTiledImage(image, RoundRectangle(bounds));
					break;
				case ChartImageSizeMode.Zoom:
					DrawZoomedImage(image, RoundRectangle(bounds));
					break;
				default:
					ChartDebug.Fail("Unkown ImageSizeMode.");
					break;
			}
		}
		public void DrawRectangle(RectangleF bounds, Pen pen) {
			context.DrawRectangle(pen, RoundRectangle(bounds));
		}
		public void DrawRectangle(RectangleF bounds, Color color, float thickness) {
			using (Pen pen = new Pen(color, thickness))
				DrawRectangle(bounds, pen);
		}
		public void FillRectangle(RectangleF bounds, Brush brush) {
			context.FillRectangle(brush, RoundRectangle(bounds));
		}
		public void FillRectangle(RectangleF bounds, Color color) {
			using (Brush brush = new SolidBrush(color))
				FillRectangle(bounds, brush);
		}
		public void FillRectangle(RectangleF bounds, Color color, RectangleFillStyle fillStyle) {
			if (fillStyle.FillMode == FillMode.Gradient) {
				RectangleGradientFillOptions options = fillStyle.Options as RectangleGradientFillOptions;
				FillRectangleGradient(bounds, color, options.Color2, options.GradientMode);
			}
			else if (fillStyle.FillMode == FillMode.Solid) {
				using (Brush brush = new SolidBrush(color))
					FillRectangle(bounds, brush);
			}
			else if (fillStyle.FillMode == FillMode.Hatch) {
				HatchFillOptions options = fillStyle.Options as HatchFillOptions;
				FillRectangle(bounds, options.HatchStyle, options.Color2, color);
			}
		}
		public void FillRectangle(RectangleF bounds, RectangleF gradient, Color color, Color color2, LinearGradientMode mode) {
			if (gradient.Width == 0 || gradient.Height == 0)
				return;
			SetPixelOffsetMode(PixelOffsetMode.Half);
			using (Brush brush = new LinearGradientBrush(gradient, color, color2, mode))
				FillRectangle(bounds, brush);
			RestorePixelOffsetMode();
		}
		public void FillRectangle(RectangleF bounds, HatchStyle hatch, Color color) {
			FillRectangle(bounds, hatch, color, Color.Transparent);
		}
		public void FillRectangle(RectangleF bounds, HatchStyle hatch, Color color, Color backColor) {
			using (Brush brush = new HatchBrush(hatch, color, backColor))
				context.FillRectangle(brush, bounds);
		}
		public void DrawPath(FillOptionsBase fillOptions, GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			if (fillOptions is SolidFillOptions) {
				FillPath(path, color);
			}
			else if (fillOptions is RectangleGradientFillOptions) {
				FillPath(path, gradientRect, color, color2, ((RectangleGradientFillOptions)fillOptions).GradientMode);
			}
			else if (fillOptions is HatchFillOptions) {
				FillPath(path, ((HatchFillOptions)fillOptions).HatchStyle, color2, color);
			}
		}
		public void DrawPath(GraphicsPath path, Color color, int thickness) {
			using (Pen pen = new Pen(color, thickness))
				DrawPath(path, pen);
		}
		public void DrawPath(GraphicsPath path, Pen pen) {
			context.DrawPath(pen, path);
		}
		public void FillPath(GraphicsPath path, Brush brush) {
			context.FillPath(brush, path);
		}
		public void FillPath(GraphicsPath path, Color color) {
			using (SolidBrush brush = new SolidBrush(color))
				context.FillPath(brush, path);
		}
		public void FillPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2, LinearGradientMode mode) {
			if (gradientRect.IsEmpty)
				return;
			using (Brush brush = new LinearGradientBrush(gradientRect, color, color2, mode))
				FillPath(path, brush);
		}
		public void FillPath(GraphicsPath path, RectangleF bounds, Color color, Color color2, RectangleGradientMode mode) {
			if (!bounds.AreWidthAndHeightPositive())
				return;
			SetPixelOffsetMode(PixelOffsetMode.Half);
			switch (mode) {
				case RectangleGradientMode.BottomLeftToTopRight:
					using (Brush brush = new LinearGradientBrush(bounds, color2, color, LinearGradientMode.BackwardDiagonal))
						FillPath(path, brush);
					break;
				case RectangleGradientMode.BottomRightToTopLeft:
					using (Brush brush = new LinearGradientBrush(bounds, color2, color, LinearGradientMode.ForwardDiagonal))
						FillPath(path, brush);
					break;
				case RectangleGradientMode.TopLeftToBottomRight:
					using (Brush brush = new LinearGradientBrush(bounds, color, color2, LinearGradientMode.BackwardDiagonal))
						FillPath(path, brush);
					break;
				case RectangleGradientMode.TopRightToBottomLeft:
					using (Brush brush = new LinearGradientBrush(bounds, color, color2, LinearGradientMode.ForwardDiagonal))
						FillPath(path, brush);
					break;
				case RectangleGradientMode.BottomToTop:
					using (Brush brush = new LinearGradientBrush(bounds, color2, color, LinearGradientMode.Vertical))
						FillPath(path, brush);
					break;
				case RectangleGradientMode.TopToBottom:
					using (Brush brush = new LinearGradientBrush(bounds, color, color2, LinearGradientMode.Vertical))
						FillPath(path, brush);
					break;
				case RectangleGradientMode.LeftToRight:
					using (Brush brush = new LinearGradientBrush(bounds, color, color2, LinearGradientMode.Horizontal))
						FillPath(path, brush);
					break;
				case RectangleGradientMode.RightToLeft:
					using (Brush brush = new LinearGradientBrush(bounds, color2, color, LinearGradientMode.Horizontal))
						FillPath(path, brush);
					break;
				case RectangleGradientMode.ToCenterHorizontal:
					RectangleF[] toCenterHorizontalParts = GraphicUtils.SplitRectangle(bounds, SplitRectangleMode.Horizontal, true);
					SetClipping(GetClipRectangle(toCenterHorizontalParts[0], new Point(-1, -1), new Size(1, 2)), CombineMode.Intersect);
					using (Brush brush = new LinearGradientBrush(toCenterHorizontalParts[0], color, color2, LinearGradientMode.Horizontal))
						FillPath(path, brush);
					RestoreClipping();
					SetClipping(GetClipRectangle(toCenterHorizontalParts[1], new Point(0, -1), new Size(1, 2)), CombineMode.Intersect);
					using (Brush brush = new LinearGradientBrush(toCenterHorizontalParts[1], color2, color, LinearGradientMode.Horizontal))
						FillPath(path, brush);
					RestoreClipping();
					break;
				case RectangleGradientMode.FromCenterHorizontal:
					RectangleF[] fromCenterHorizontalParts = GraphicUtils.SplitRectangle(bounds, SplitRectangleMode.Horizontal, true);
					SetClipping(GetClipRectangle(fromCenterHorizontalParts[0], new Point(-1, -1), new Size(1, 2)), CombineMode.Intersect);
					using (Brush brush = new LinearGradientBrush(fromCenterHorizontalParts[0], color2, color, LinearGradientMode.Horizontal))
						FillPath(path, brush);
					RestoreClipping();
					SetClipping(GetClipRectangle(fromCenterHorizontalParts[1], new Point(0, -1), new Size(1, 2)), CombineMode.Intersect);
					using (Brush brush = new LinearGradientBrush(fromCenterHorizontalParts[1], color, color2, LinearGradientMode.Horizontal))
						FillPath(path, brush);
					RestoreClipping();
					break;
				case RectangleGradientMode.ToCenterVertical:
					RectangleF[] toCenterVerticalParts = GraphicUtils.SplitRectangle(bounds, SplitRectangleMode.Vertical, true);
					SetClipping(GetClipRectangle(toCenterVerticalParts[0], new Point(0, 0), new Size(0, 0)), CombineMode.Intersect);
					using (Brush brush = new LinearGradientBrush(toCenterVerticalParts[0], color, color2, LinearGradientMode.Vertical))
						FillPath(path, brush);
					RestoreClipping();
					SetClipping(GetClipRectangle(toCenterVerticalParts[1], new Point(0, 0), new Size(0, 0)), CombineMode.Intersect);
					using (Brush brush = new LinearGradientBrush(toCenterVerticalParts[1], color2, color, LinearGradientMode.Vertical))
						FillPath(path, brush);
					RestoreClipping();
					break;
				case RectangleGradientMode.FromCenterVertical:
					RectangleF[] fromCenterVerticalParts = GraphicUtils.SplitRectangle(bounds, SplitRectangleMode.Vertical, true);
					SetClipping(GetClipRectangle(fromCenterVerticalParts[0], new Point(0, 0), new Size(0, 0)), CombineMode.Intersect);
					using (Brush brush = new LinearGradientBrush(fromCenterVerticalParts[0], color2, color, LinearGradientMode.Vertical))
						FillPath(path, brush);
					RestoreClipping();
					SetClipping(GetClipRectangle(fromCenterVerticalParts[1], new Point(0, 0), new Size(0, 0)), CombineMode.Intersect);
					using (Brush brush = new LinearGradientBrush(fromCenterVerticalParts[1], color, color2, LinearGradientMode.Vertical))
						FillPath(path, brush);
					RestoreClipping();
					break;
			}
			RestorePixelOffsetMode();
		}
		public void FillPath(GraphicsPath path, HatchStyle hatch, Color color, Color color2) {
			using (Brush brush = new HatchBrush(hatch, color, color2))
				FillPath(path, brush);
		}
		public void DrawText(string text, NativeFont font, Color color, ISupportTextAntialiasing antialiasing, PointF position) {
			context.TextRenderingHint = AntialiasingSupport.GetRenderHint(antialiasing);
			StringFormat sf = new StringFormat();
			if (IsRightToLeft) {
				sf.FormatFlags = sf.FormatFlags | StringFormatFlags.DirectionRightToLeft;
				sf.Alignment = StringAlignment.Far;
			}
			try {
				using (Brush brush = new SolidBrush(color))
					context.DrawString(text, font.GdiPlusFont, brush, position, sf);
			}
			finally {
				context.TextRenderingHint = TextRenderingHint.SystemDefault;
			}
		}
		public void DrawBoundedText(string text, NativeFont font, Color color, ISupportTextAntialiasing antialiasing, RectangleF bounds, StringAlignment alignment, StringAlignment lineAlignment) {
			DrawBoundedText(text, font, color, antialiasing, bounds, alignment, lineAlignment, 0.0f, false);
		}
		public void DrawBoundedText(string text, NativeFont font, Color color, ISupportTextAntialiasing antialiasing, RectangleF bounds, StringAlignment alignment, StringAlignment lineAlignment, bool useTypographicStringFormat) {
			DrawBoundedText(text, font, color, antialiasing, bounds, alignment, lineAlignment, 0.0f, false);
		}
		public void DrawBoundedText(string text, NativeFont font, Color color, ISupportTextAntialiasing antialiasing, RectangleF bounds, StringAlignment alignment, StringAlignment lineAlignment, float textHeight) {
			DrawBoundedText(text, font, color, antialiasing, bounds, alignment, lineAlignment, textHeight, false);
		}
		public void DrawBoundedText(string text, NativeFont font, Color color, ISupportTextAntialiasing antialiasing, RectangleF bounds, StringAlignment alignment, StringAlignment lineAlignment, float textHeight, bool useTypographicStringFormat) {
			Rectangle roundedBounds = new Rectangle(MathUtils.StrongRound(bounds.X), MathUtils.StrongRound(bounds.Y), MathUtils.Ceiling(bounds.Width), MathUtils.Ceiling(bounds.Height));
			using (StringFormat sf = useTypographicStringFormat ? (StringFormat)StringFormat.GenericTypographic.Clone() : new StringFormat()) {
				if (useTypographicStringFormat)
					sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
				if (bounds.Height > textHeight)
					sf.FormatFlags |= StringFormatFlags.LineLimit;
				sf.Trimming = StringTrimming.EllipsisCharacter;
				StringAlignment actualAlignment = alignment;
				sf.Alignment = alignment;			  
				sf.LineAlignment = lineAlignment;
				float[] tabs = { TextMeasurer.TabStop };
				sf.SetTabStops(0, tabs);
				if (IsRightToLeft)
					sf.FormatFlags = sf.FormatFlags | StringFormatFlags.DirectionRightToLeft;
				context.TextRenderingHint = AntialiasingSupport.GetRenderHint(antialiasing);
				try {
					using (Brush brush = new SolidBrush(color))
						context.DrawString(text, font.GdiPlusFont, brush, roundedBounds, sf);
				}
				catch (System.Runtime.InteropServices.ExternalException exception) {
					if (exception.ErrorCode != gdiPlusGenericError)
						throw;
				}
				finally {
					context.TextRenderingHint = TextRenderingHint.SystemDefault;
				}
			}
		}
		public void DrawLine(PointF p1, PointF p2, Color color, LineStyle lineStyle) {
			DrawLine(p1, p2, color, lineStyle.Thickness, lineStyle, LineCap.Flat);
		}
		public void DrawLine(PointF p1, PointF p2, Color color, int thickness, LineStyle lineStyle, LineCap lineCap) {
			using (Pen pen = new Pen(color, thickness)) {
				pen.StartCap = lineCap;
				pen.EndCap = lineCap;
				if (lineStyle != null) {
					pen.LineJoin = lineStyle.LineJoin;
					DashStyleHelper.ApplyDashStyle(pen, lineStyle.ActualDashStyle);
				}
				context.DrawLine(pen, p1, p2);
			}
		}
		public void DrawLine(PointF p1, PointF p2, Color color, int thickness) {
			DrawLine(p1, p2, color, thickness, null, LineCap.Flat);
		}
		void OptimizeAndDrawLines(LineStrip strip, Pen pen) {
			double x0 = strip[0].X;
			double y0 = strip[0].Y;
			for (int i = 1; i < strip.Count; i++) {
				if (x0 == strip[i].X) {
					double y1;
					if (strip[i].Y > y0)
						y1 = strip[i].Y;
					else {
						y1 = y0;
						y0 = strip[i].Y;
					}
					for (int j = i; j < strip.Count; j++) {
						if (strip[j].X != x0) {
							context.FillRectangle(pen.Brush, (float)x0 - 0.5f * pen.Width, (float)y0, pen.Width, (float)(y1 - y0));
							if (j < strip.Count - 2 && strip[j + 1].X - strip[j].X <= 1.0 && strip[j + 1].X == strip[j + 2].X) {
								double height1 = Math.Abs(strip[j + 2].Y - strip[j + 1].Y) + y1 - y0;
								double height2 = Math.Max(y1, Math.Max(strip[j + 2].Y, strip[j + 1].Y)) - Math.Min(y0, Math.Min(strip[j + 2].Y, strip[j + 1].Y));
								if (height2 < height1) {
									i = j;
									x0 = strip[j].X;
									y0 = strip[j].Y;
									break;
								}
							}
							i = j - 1;
							x0 = strip[j - 1].X;
							y0 = strip[j - 1].Y;
							break;
						}
						y0 = Math.Min(y0, strip[j].Y);
						y1 = Math.Max(y1, strip[j].Y);
					}
				}
				else {
					context.DrawLine(pen, new PointF((float)x0, (float)y0), new PointF((float)strip[i].X, (float)strip[i].Y));
					x0 = strip[i].X;
					y0 = strip[i].Y;
				}
			}
		}
		public void DrawLines(Point[] points, Color color, int thickness, LineStyle lineStyle) {
			if (points.Length > 1)
				using (Pen pen = CreatePen(color, thickness, lineStyle)) {
					if (CanDrawSegmentedLine(pen)) {
						for (int i = 0; i < points.Length - 1; i++)
							context.DrawLine(pen, points[i], points[i + 1]);
					}
					else
						context.DrawLines(pen, points);
				}
		}
		public void DrawLines(PointF[] points, Color color, int thickness, LineStyle lineStyle) {
			if (points.Length > 1)
				using (Pen pen = CreatePen(color, thickness, lineStyle)) {
					if (CanDrawSegmentedLine(pen)) {
						for (int i = 0; i < points.Length - 1; i++)
							context.DrawLine(pen, points[i], points[i + 1]);
					}
					else
						context.DrawLines(pen, points);
				}
		}
		public void DrawLines(LineStrip strip, Color color, int thickness, LineStyle lineStyle, LineCap lineCap) {
			if (strip.Count > 1)
				using (Pen pen = CreatePen(color, thickness, lineStyle)) {
					bool canOptimize = optimizationCache.Count > 0 ? optimizationCache.Peek() : true;
					if (CanDrawSegmentedLine(pen) && canOptimize && strip.Count > pointsCountForOptimizing)
						OptimizeAndDrawLines(strip, pen);
					else {
						PointF[] vertices = StripsUtils.Convert(strip);
						if (CanDrawSegmentedLine(pen)) {
							for (int i = 0; i < vertices.Length - 1; i++)
								context.DrawLine(pen, vertices[i], vertices[i + 1]);
						}
						else {
							pen.StartCap = lineCap;
							pen.EndCap = lineCap;
							context.DrawLines(pen, vertices);
						}
					}
				}
			else {
				PointF[] vertices = StripsUtils.Convert(strip);
				if (vertices.Length > 0) {
					float halfThickness = 0.5f * thickness;
					using (Brush brush = new SolidBrush(color))
						context.FillRectangle(brush, vertices[0].X - halfThickness, vertices[0].Y - halfThickness, thickness, thickness);
				}
			}
		}
		public void DrawCircle(PointF center, float radius, Color color) {
			DrawCircle(center, radius, color, 1);
		}
		public void DrawCircle(PointF center, float radius, Color color, int thickness) {
			DrawCircle(center, radius, color, thickness, null);
		}
		public void DrawCircle(PointF center, float radius, Color color, int thickness, LineStyle lineStyle) {
			using (Pen pen = new Pen(color, thickness)) {
				if (lineStyle != null)
					DashStyleHelper.ApplyDashStyle(pen, lineStyle.ActualDashStyle);
				context.DrawEllipse(pen, GetEllipseBounds(center, radius));
			}
		}
		public void FillCircle(PointF center, float radius, Color color) {
			using (Brush brush = new SolidBrush(color))
				FillCircle(center, radius, brush);
		}
		public void FillCircle(PointF center, float radius, Color color, Color color2, LinearGradientMode mode) {
			RectangleF gradient = GetEllipseBounds(center, radius);
			if (!GraphicUtils.CheckIsSizePositive(gradient.Size))
				return;
			SetPixelOffsetMode(PixelOffsetMode.Half);
			using (Brush brush = new LinearGradientBrush(gradient, color, color2, mode))
				FillCircle(center, radius, brush);
			RestorePixelOffsetMode();
		}
		public void FillCircle(PointF center, float radius, Color color, Color color2) {
			FillEllipse(center, radius, radius, color, color2);
		}
		public void FillCircle(PointF center, float radius, HatchStyle hatch, Color color, Color color2) {
			FillEllipse(center, radius, radius, hatch, color, color2);
		}
		public void FillEllipse(RectangleF rect, Brush brush) {
			context.FillEllipse(brush, rect);
		}
		public void FillEllipse(PointF center, float semiAxisX, float semiAxisY, Color color) {
			using (Brush brush = new SolidBrush(color))
				FillEllipse(center, semiAxisX, semiAxisY, brush);
		}
		public void FillEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2, LinearGradientMode mode) {
			SetPixelOffsetMode(PixelOffsetMode.Half);
			RectangleF gradient = GetEllipseBounds(center, semiAxisX, semiAxisY);
			using (Brush brush = new LinearGradientBrush(gradient, color, color2, mode))
				FillEllipse(center, semiAxisX, semiAxisY, brush);
			RestorePixelOffsetMode();
		}
		public void FillEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			SetPixelOffsetMode(PixelOffsetMode.Half);
			using (GraphicsPath path = new GraphicsPath()) {
				RectangleF bounds = GetEllipseBounds(Point.Round(center), semiAxisX, semiAxisY);
				path.AddEllipse(bounds);
				using (PathGradientBrush brush = new PathGradientBrush(path)) {
					brush.CenterColor = color;
					brush.SurroundColors = new Color[] { color2 };
					brush.CenterPoint = new PointF((float)center.X, (float)center.Y);
					context.FillEllipse(brush, bounds);
				}
			}
			RestorePixelOffsetMode();
		}
		public void FillEllipse(PointF center, float semiAxisX, float semiAxisY, RectangleF gradientRect, Color color, Color color2, LinearGradientMode mode) {
			SetPixelOffsetMode(PixelOffsetMode.Half);
			using (Brush brush = new LinearGradientBrush(gradientRect, color, color2, mode))
				FillEllipse(Point.Round(center), (int)semiAxisX, (int)semiAxisY, brush);
			RestorePixelOffsetMode();
		}
		public void FillEllipse(PointF center, float semiAxisX, float semiAxisY, HatchStyle hatch, Color color, Color color2) {
			using (Brush brush = new HatchBrush(hatch, color, color2))
				FillEllipse(Point.Round(center), (int)semiAxisX, (int)semiAxisY, brush);
		}
		public void DrawPolygon(LineStrip strip, Color color, int thickness) {
			using (Pen pen = new Pen(color, thickness))
				context.DrawPolygon(pen, StripsUtils.Convert(strip, true));
		}
		void OptimizeAndFillPolygon(LineStrip strip, Brush brush) {
			List<PointF> points = new List<PointF>(strip.Count);
			for (int i = 0; i < strip.Count; i++)
				if (i == 0 || strip[i].X != strip[i - 1].X || i == strip.Count - 1)
					points.Add(new PointF((float)strip[i].X, (float)strip[i].Y));
			context.FillPolygon(brush, points.ToArray());
		}
		public void FillPolygon(LineStrip strip, Brush brush) {
			bool canOptimize = optimizationCache.Count > 0 ? optimizationCache.Peek() : true;
			if (canOptimize && strip.Count > 2 * pointsCountForOptimizing)
				OptimizeAndFillPolygon(strip, brush);
			else
				context.FillPolygon(brush, StripsUtils.Convert(strip, true));
		}
		public void FillPolygon(LineStrip strip, Color color) {
			using (Brush brush = new SolidBrush(color))
				FillPolygon(strip, brush);
		}
		public void FillPolygon(LineStrip strip, HatchStyle hatch, Color color) {
			FillPolygon(strip, hatch, color, Color.Transparent);
		}
		public void FillPolygon(LineStrip strip, HatchStyle hatch, Color color, Color backColor) {
			using (Brush brush = new HatchBrush(hatch, color, backColor))
				context.FillPolygon(brush, StripsUtils.Convert(strip, true));
		}
		public void FillPolygonGradient(LineStrip strip, RectangleF bounds, Color color, Color color2, PolygonGradientMode mode) {
			if (!bounds.AreWidthAndHeightPositive())
				return;
			SetPixelOffsetMode(PixelOffsetMode.Half);
			switch (mode) {
				case PolygonGradientMode.BottomLeftToTopRight:
					using (Brush brush = new LinearGradientBrush(bounds, color2, color, LinearGradientMode.BackwardDiagonal))
						FillPolygon(strip, brush);
					break;
				case PolygonGradientMode.BottomRightToTopLeft:
					using (Brush brush = new LinearGradientBrush(bounds, color2, color, LinearGradientMode.ForwardDiagonal))
						FillPolygon(strip, brush);
					break;
				case PolygonGradientMode.TopLeftToBottomRight:
					using (Brush brush = new LinearGradientBrush(bounds, color, color2, LinearGradientMode.BackwardDiagonal))
						FillPolygon(strip, brush);
					break;
				case PolygonGradientMode.TopRightToBottomLeft:
					using (Brush brush = new LinearGradientBrush(bounds, color, color2, LinearGradientMode.ForwardDiagonal))
						FillPolygon(strip, brush);
					break;
				case PolygonGradientMode.BottomToTop:
					using (Brush brush = new LinearGradientBrush(bounds, color2, color, LinearGradientMode.Vertical))
						FillPolygon(strip, brush);
					break;
				case PolygonGradientMode.TopToBottom:
					using (Brush brush = new LinearGradientBrush(bounds, color, color2, LinearGradientMode.Vertical))
						FillPolygon(strip, brush);
					break;
				case PolygonGradientMode.LeftToRight:
					using (Brush brush = new LinearGradientBrush(bounds, color, color2, LinearGradientMode.Horizontal))
						FillPolygon(strip, brush);
					break;
				case PolygonGradientMode.RightToLeft:
					using (Brush brush = new LinearGradientBrush(bounds, color2, color, LinearGradientMode.Horizontal))
						FillPolygon(strip, brush);
					break;
				case PolygonGradientMode.ToCenter:
					RectangleF[] toCenterHorizontalParts = GraphicUtils.SplitRectangle(bounds, SplitRectangleMode.Horizontal, true);
					SetClipping(GetClipRectangle(toCenterHorizontalParts[0], new Point(-1, -1), new Size(1, 2)), CombineMode.Intersect);
					using (Brush brush = new LinearGradientBrush(toCenterHorizontalParts[0], color, color2, LinearGradientMode.Horizontal))
						FillPolygon(strip, brush);
					RestoreClipping();
					SetClipping(GetClipRectangle(toCenterHorizontalParts[1], new Point(0, -1), new Size(1, 2)), CombineMode.Intersect);
					using (Brush brush = new LinearGradientBrush(toCenterHorizontalParts[1], color2, color, LinearGradientMode.Horizontal))
						FillPolygon(strip, brush);
					RestoreClipping();
					break;
				case PolygonGradientMode.FromCenter:
					RectangleF[] fromCenterHorizontalParts = GraphicUtils.SplitRectangle(bounds, SplitRectangleMode.Horizontal, true);
					SetClipping(GetClipRectangle(fromCenterHorizontalParts[0], new Point(-1, -1), new Size(1, 2)), CombineMode.Intersect);
					using (Brush brush = new LinearGradientBrush(fromCenterHorizontalParts[0], color2, color, LinearGradientMode.Horizontal))
						FillPolygon(strip, brush);
					RestoreClipping();
					SetClipping(GetClipRectangle(fromCenterHorizontalParts[1], new Point(0, -1), new Size(1, 2)), CombineMode.Intersect);
					using (Brush brush = new LinearGradientBrush(fromCenterHorizontalParts[1], color, color2, LinearGradientMode.Horizontal))
						FillPolygon(strip, brush);
					RestoreClipping();
					break;
			}
			RestorePixelOffsetMode();
		}
		public void FillPolygonGradient(LineStrip strip, RectangleF gradient, Color color, Color color2, LinearGradientMode mode) {
			SetPixelOffsetMode(PixelOffsetMode.Half);
			using (Brush brush = new LinearGradientBrush(gradient, color, color2, mode))
				FillPolygon(strip, brush);
			RestorePixelOffsetMode();
		}
		public void FillPolygonGradient(LineStrip strip, RectangleF bounds, Color color, Color color2) {
			SetPixelOffsetMode(PixelOffsetMode.Half);
			using (GraphicsPath path = new GraphicsPath()) {
				PointF centerPoint = GraphicUtils.CalcCenter(bounds, false);
				PointF[] polygon = StripsUtils.Convert(strip, true);
				path.AddEllipse(GetBoundsForCircle(polygon, centerPoint));
				using (PathGradientBrush brush = new PathGradientBrush(path)) {
					brush.CenterColor = color;
					brush.SurroundColors = new Color[] { color2 };
					brush.CenterPoint = new PointF((float)centerPoint.X, (float)centerPoint.Y);
					context.FillPolygon(brush, polygon);
				}
			}
			RestorePixelOffsetMode();
		}
		public void DrawBezier(BezierStrip strip, Color color, float thickness) {
			DrawBezier(strip, color, thickness, null);
		}
		public void DrawBezier(BezierStrip strip, Color color, float thickness, LineStyle lineStyle) {
			if (!strip.IsEmpty)
				using (Pen pen = new Pen(color, thickness)) {
					if (lineStyle != null)
						DashStyleHelper.ApplyDashStyle(pen, lineStyle.ActualDashStyle);
					context.DrawBeziers(pen, StripsUtils.Convert(strip.GetPointsForDrawing(true, true)));
				}
		}
		public void FillBezier(BezierRangeStrip strip, Color color) {
			if (strip != null && strip.Count > 0)
				using (Brush brush = new SolidBrush(color))
					FillBezier(strip, brush);
		}
		public void FillBezier(BezierRangeStrip strip, RectangleF gradient, Color color, Color color2, LinearGradientMode mode) {
			SetPixelOffsetMode(PixelOffsetMode.Half);
			using (Brush brush = new LinearGradientBrush(gradient, color, color2, mode))
				FillBezier(strip, brush);
			RestorePixelOffsetMode();
		}
		public void FillBezier(BezierRangeStrip strip, HatchStyle hatch, Color color) {
			FillBezier(strip, hatch, color, Color.Transparent);
		}
		public void FillBezier(BezierRangeStrip strip, HatchStyle hatch, Color color, Color color2) {
			using (Brush brush = new HatchBrush(hatch, color, color2))
				FillBezier(strip, brush);
		}
		public void DrawPie(Pie pie, PointF basePoint, Color color, int thickness) {
			DrawPie(pie, basePoint, pie.StartAngleInDegrees, pie.SweepAngleInDegrees, color, thickness);
		}
		public void DrawPie(Pie pie, PointF basePoint, float startAngle, float sweepAngle, Color color, int thickness) {
			GRealPoint2D pieCenter = pie.CalculateCenter(basePoint);
			Rectangle emptyRect = new Rectangle((int)Math.Round(pieCenter.X), (int)Math.Round(pieCenter.Y), 0, 0);
			int minorSemiAxisInt = Convert.ToInt32(pie.MinorSemiaxis);
			int majorSemiAxisInt = Convert.ToInt32(pie.MajorSemiaxis);
			Rectangle rect = Rectangle.Inflate(emptyRect, majorSemiAxisInt, minorSemiAxisInt);
			int maxThickness = (int)Math.Floor(Math.Min(rect.Width, rect.Height) / 2.0f);
			if (thickness <= maxThickness) {
				bool shouldAddLines = sweepAngle != -360.0;
				using (Pen pen = new Pen(color, Math.Min(thickness, maxThickness))) {
					int innerMajorSemiaxis = Convert.ToInt32(pie.MajorSemiaxis * pie.HoleFraction);
					int innerMinorSemiaxis = Convert.ToInt32(pie.MinorSemiaxis * pie.HoleFraction);
					if (innerMajorSemiaxis >= 1.0f && innerMinorSemiaxis >= 1.0f) {
						Rectangle innerRect = Rectangle.Inflate(emptyRect, innerMajorSemiaxis, innerMinorSemiaxis);
						context.DrawArc(pen, rect, startAngle, sweepAngle);
						context.DrawArc(pen, innerRect, startAngle, sweepAngle);
						if (shouldAddLines) {
							GRealPoint2D startPoint, finishPoint;
							GRealPoint2D innerStartPoint, innerFinishPoint;
							GraphicUtils.CalculateStartFinishPoints(pieCenter, majorSemiAxisInt,
								minorSemiAxisInt, startAngle, sweepAngle, out startPoint, out finishPoint);
							GraphicUtils.CalculateStartFinishPoints(pieCenter, innerMajorSemiaxis,
								innerMinorSemiaxis, startAngle, sweepAngle, out innerStartPoint, out innerFinishPoint);
							pen.StartCap = LineCap.AnchorMask;
							pen.EndCap = LineCap.AnchorMask;
							context.DrawLine(pen, new PointF((float)startPoint.X, (float)startPoint.Y), new PointF((float)innerStartPoint.X, (float)innerStartPoint.Y));
							context.DrawLine(pen, new PointF((float)finishPoint.X, (float)finishPoint.Y), new PointF((float)innerFinishPoint.X, (float)innerFinishPoint.Y));
						}
					}
					else if (shouldAddLines)
						context.DrawPie(pen, rect, startAngle, sweepAngle);
					else
						context.DrawEllipse(pen, rect);
				}
			}
			else {
				int halfThickness = thickness / 2;
				rect.Inflate(halfThickness, halfThickness);
				using (Brush brush = new SolidBrush(color))
					context.FillEllipse(brush, rect);
			}
		}
		public void FillPie(Rectangle rect, float startAngle, float sweepAngle, Brush brush) {
			context.FillPie(brush, rect, startAngle, sweepAngle);
		}
		public void FillPie(Pie pie, PointF basePoint, HatchStyle hatchStyle, Color color, Color backColor) {
			using (Brush brush = new HatchBrush(hatchStyle, color, backColor)) {
				using (GraphicsPath path = GraphicUtils.CreatePieGraphicsPath(pie.CalculateCenter(basePoint), pie.MajorSemiaxis, pie.MinorSemiaxis, pie.HoleFraction, pie.StartAngleInDegrees, pie.SweepAngleInDegrees))
					context.FillPath(brush, path);
			}
		}
		public void FillPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, Color color, Color color2) {
			SetPixelOffsetMode(PixelOffsetMode.Half);
			using (GraphicsPath path = new GraphicsPath()) {
				Rectangle rect = Rectangle.Inflate(new Rectangle((int)Math.Round(center.X), (int)Math.Round(center.Y), 0, 0),
					Convert.ToInt32(majorSemiAxis), Convert.ToInt32(minorSemiAxis));
				path.AddEllipse(rect);
				using (PathGradientBrush brush = new PathGradientBrush(path)) {
					brush.CenterPoint = new PointF(center.X, center.Y);
					brush.CenterColor = color;
					brush.SurroundColors = new Color[] { color2 };
					if (holePercent > 0.0f)
						brush.SetBlendTriangularShape(1.0f - holePercent);
					using (GraphicsPath piePath = GraphicUtils.CreatePieGraphicsPath(new GRealPoint2D(center.X, center.Y), majorSemiAxis, minorSemiAxis, holePercent, startAngle, sweepAngle))
						FillPath(piePath, brush);
				}
			}
			RestorePixelOffsetMode();
		}
		public void DrawArc(PointF center, float radius, float startAngleDegree, float sweepAngleDegree, Color color, int thickness, DashStyle dashStyle) {
			using (Pen pen = new Pen(color, thickness)) {
				DashStyleHelper.ApplyDashStyle(pen, dashStyle);
				context.DrawArc(pen, RectangleF.Inflate(new RectangleF((PointF)center, SizeF.Empty), radius, radius), startAngleDegree, sweepAngleDegree);
			}
		}
	}
}
