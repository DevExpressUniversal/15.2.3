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

using DevExpress.Utils.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
namespace DevExpress.XtraMap.Drawing {
	public class GdiRenderer : RendererBase {
		Size viewSize;
		GraphicsCache cache;
		Bitmap bitmap;
		Graphics activeGraphics;
		Graphics targetGraphics;
		float screenDpiX;
		float screenDpiY;
		MapPoint transformScale = new MapPoint(1,1);
		MapPoint transformOffset = new MapPoint();
		bool isShapeDrawing = false;
		bool smoothing = false;
		bool Smoothing {
			get { return smoothing; }
			set {
				if(smoothing == value)
					return;
				smoothing = value;
				OnSmoothingChanged();
			}
		}
		public GdiRenderer() {
		}
		protected override bool Initialize(object context) {
			ReleaseResources();
			this.targetGraphics = context as Graphics;
			this.screenDpiX = this.targetGraphics.DpiX;
			this.screenDpiY = this.targetGraphics.DpiY;
			if(bitmap != null) {
				bitmap.Dispose();
				bitmap = null;
			}
			if(MapUtils.IsValidSize(viewSize)) {
				this.bitmap = new Bitmap(viewSize.Width, viewSize.Height);
				this.activeGraphics = Graphics.FromImage(bitmap);
				this.activeGraphics.SmoothingMode = SmoothingMode.AntiAlias;
				this.smoothing = true;
				this.cache = new GraphicsCache(activeGraphics);
				return true;
			}
			return false;
		}
		protected override void DisposeCore() {
			ReleaseResources();
		}
		void ReleaseResources() {
			if(ShouldDisposeContext && this.targetGraphics != null) {
				this.targetGraphics.Dispose();
				this.targetGraphics = null;
			}
			if(cache != null) {
				cache.Dispose();
				cache = null;
			}
			if(bitmap != null) {
				bitmap.Dispose();
				bitmap = null;
			}
			if(activeGraphics != null) {
				activeGraphics.Dispose();
				activeGraphics = null;
			}
		}
		void OnSmoothingChanged() {
			if(activeGraphics != null) {
				SmoothingMode mode = Smoothing ? SmoothingMode.AntiAlias : SmoothingMode.None;
				activeGraphics.SmoothingMode = mode;
			}
		}
		void DrawPolygon(IList<MapPoint[]> contours, IRenderItemStyle style) {
			if(contours.Count == 0)
				return;
			using(GraphicsPath path = new GraphicsPath()) {
				SmoothingMode mode = activeGraphics.SmoothingMode;
				if((!MapUtils.CanDrawColor(style.Stroke) || style.StrokeWidth == 0) && isShapeDrawing)
					activeGraphics.SmoothingMode = SmoothingMode.None;
				foreach(MapPoint[] points in contours)
					if(points.Length > 2)
						path.AddPolygon(TransformPoligonPoints(points));
				if(MapUtils.CanDrawColor(style.Fill))
					activeGraphics.FillPath(cache.GetSolidBrush(style.Fill), path);
				activeGraphics.SmoothingMode = mode;
				if(MapUtils.CanDrawColor(style.Stroke) && style.StrokeWidth > 0) {
					using(Pen pen = new Pen(style.Stroke, style.StrokeWidth)) {
						pen.LineJoin = LineJoin.Round;
						activeGraphics.DrawPath(pen, path);
					}
				}
			}
		}
		PointF[] TransformPoligonPoints(MapPoint[] points) {
			PointF[] newPoints = new PointF[points.Length];
			for(int i = 0; i < points.Length; i++)
				newPoints[i] = new PointF((float)Math.Round(points[i].X * transformScale.X + transformOffset.X, 1), (float)Math.Round(points[i].Y * transformScale.Y + transformOffset.Y, 1));
			return newPoints;
		}
		PointF[] TransformStrokePoints(MapPoint[] points) {
			PointF[] newPoints = new PointF[points.Length];
			for(int i = 0; i < points.Length; i++)
				newPoints[i] = new PointF((float)(points[i].X * transformScale.X + transformOffset.X), (float)(points[i].Y * transformScale.Y + transformOffset.Y));
			return newPoints;
		}
		void DrawPolyLine(IList<MapPoint[]> contours, IRenderItemStyle style) {
			if(contours.Count == 0)
				return;
			using(GraphicsPath path = new GraphicsPath()) {
				foreach(MapPoint[] points in contours) {
					path.StartFigure();
					path.AddLines(TransformStrokePoints(points));
				}
				if(style.Stroke != Color.Transparent && style.StrokeWidth > 0) {
					using(Pen pen = new Pen(style.Stroke, style.StrokeWidth)) {
						pen.LineJoin = LineJoin.Round;
						activeGraphics.DrawPath(pen, path);
					}
				}
			}
		}
		protected override void SetViewSize(Size size) {
			this.viewSize = size;
		}
		protected override void StartRender(IRenderContext renderContext) {
			base.StartRender(renderContext);
			using(Region region = new Region(renderContext.ContentBounds))
				activeGraphics.Clip = region;
			activeGraphics.Clear(renderContext.BackColor);
		}
		protected override void RenderBorder(Graphics gr, IRenderStyleProvider provider) {
			if(provider == null) return;
			DevExpress.Utils.Drawing.BorderPainter painter = MapUtils.GetBorderPainter(provider);
			painter.DrawObject(new BorderObjectInfoArgs(cache, null, provider.Bounds));
		}
		protected override void SetScaledTransform(bool antiAliasing) {
			transformOffset = new MapPoint( RenderOffset.X, RenderOffset.Y);
			transformScale = new MapPoint(RenderScaleFactorX, RenderScaleFactorY);
			Smoothing = antiAliasing;
			isShapeDrawing = true;
		}
		protected override void SetImageTileTransform(ILocatableRenderItem locatableItem, bool antiAliasing) {
			MapPoint stretchFactor = locatableItem.StretchFactor;
			MapUnit itemLocation = locatableItem.Location;
			double x = (RenderOffset.X + itemLocation.X) * stretchFactor.X;
			double y = (RenderOffset.Y + itemLocation.Y) * stretchFactor.Y;
			this.transformScale = stretchFactor;
			this.transformOffset = new MapPoint(x, y);
			Smoothing = antiAliasing;
		}
		protected override void SetLocatableTransform(ILocatableRenderItem locatableItem, bool antiAliasing) {
			MapPoint stretchFactor = locatableItem.StretchFactor;
			MapUnit itemLocation = locatableItem.Location;
			double imageOriginX = -locatableItem.SizeInPixels.Width * stretchFactor.X * locatableItem.Origin.X;
			double imageOriginY = -locatableItem.SizeInPixels.Height * stretchFactor.Y * locatableItem.Origin.Y;
			double x = itemLocation.X * RenderScaleFactorX * stretchFactor.X;
			double y = itemLocation.Y * RenderScaleFactorY * stretchFactor.Y;
			MapPoint position = new MapPoint(x + imageOriginX + RenderOffset.X,
											 y + imageOriginY + RenderOffset.Y);
			ITemplateGeometryItem template = locatableItem as ITemplateGeometryItem;
			transformOffset = new MapPoint(position.X, position.Y);
			transformScale = new MapPoint(stretchFactor.X, stretchFactor.Y);
			if(template != null)
				transformScale = transformScale * template.StretchFactor;
			Smoothing = antiAliasing;
		}
		protected override void ResetRenderTransform() {
			ResetTransform();
		}
		protected override void DrawRectangle(Rectangle rect, Color fill, Color stroke) {
			activeGraphics.FillRectangle(cache.GetSolidBrush(fill), rect);
			activeGraphics.DrawRectangle(cache.GetPen(stroke), rect);
		}
		protected override void EndRender() {
			targetGraphics.DrawImageUnscaled(bitmap, Point.Empty);
			activeGraphics.ResetTransform();
		}
		protected override void RenderOverlay(IRenderOverlay overlay) {
			if(CanRenderOverlay(overlay))
				RenderOverlay(overlay.OverlayImage, overlay.ScreenPosition);
		}
		protected virtual bool CanRenderOverlay(IRenderOverlay overlay) { return true; }
		protected override void SetClip(Rectangle clipRect) {
			activeGraphics.SetClip(clipRect);
		}
		protected override void ResetClip() {
			activeGraphics.ResetClip();
		}
		void RenderOverlay(Image image, PointF pos) {
			activeGraphics.DrawImageUnscaled(image, (int)pos.X, (int)pos.Y);
		}
		public void ResetTransform() {
			transformOffset = new MapPoint();
			transformScale = new MapPoint(1, 1);
			activeGraphics.ResetTransform();
			isShapeDrawing = false;
		}
		protected override IRenderItemResourceHolder CreateItemResourceHolder(IRenderItemProvider provider, IRenderItem owner) {
			ScreenGeometryResourceHolder holder = new ScreenGeometryResourceHolder(provider, owner);
			holder.Initialize();
			return holder;
		}
		protected override void RenderGeometry(IRenderItemResourceHolder holder, IRenderItemStyle style) {
			ScreenGeometryResourceHolder screenGeometryResourceHolder = (ScreenGeometryResourceHolder)holder;
			IGeometry geometry = screenGeometryResourceHolder.ScreenGeometry;
			if(geometry is PolygonGeometry) {
				DrawPolygon(screenGeometryResourceHolder.Points, style);
				return;
			}
			if(geometry is PolylineGeometry) {
				DrawPolyLine(screenGeometryResourceHolder.Points, style);
			}
		}
		protected override void RenderImage(IImageGeometry geometry) {
			Image image = geometry.Image;
			RectangleF imageRect = TransformRect(geometry.ImageRect);
			RectangleF clipRect = geometry.ClipRect;
			byte transparency = geometry.Transparency;
			lock(image) {
				imageRect = RoundImageRect(imageRect, !geometry.AlignImage);
				if(clipRect == RectangleF.Empty)
					DrawImage(image, imageRect, transparency);
				else
					DrawClipImage(image, imageRect, clipRect, (byte)(transparency / 2));
			}
		}
		RectangleF TransformRect(RectangleF rect) {
			if(rect.IsEmpty) return rect;
			float x = (float)Math.Floor(rect.X * transformScale.X + transformOffset.X );
			float y = (float)Math.Floor(rect.Y * transformScale.Y + transformOffset.Y);
			float width = (float)Math.Ceiling(rect.Width * transformScale.X);
			float height = (float)Math.Ceiling(rect.Height * transformScale.Y);
			return new RectangleF(x, y, width, height);
		}
		RectangleF RoundImageRect(RectangleF rect, bool increaseRect) {
			float roundKoeff = increaseRect? 0.5f :0.0f;
			return new RectangleF(rect.X, rect.Y, rect.Width + roundKoeff, rect.Height + roundKoeff);
		}
		void DrawClipImage(Image image, RectangleF imageRect, RectangleF clipRect, byte transparency) {
			ImageAttributes attributes = GetImageAttributes(transparency);
			ImageSafeAccess.Draw(activeGraphics, image, imageRect, CorrectClipRect(clipRect), GraphicsUnit.Pixel, attributes);
		}
		ImageAttributes GetImageAttributes(byte transparency) {
			ColorMatrix matrix = new ColorMatrix() { Matrix33 = (255 - transparency) / 255.0f };
			ImageAttributes attributes = new ImageAttributes();
			attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			return attributes;
		}
		RectangleF CorrectClipRect(RectangleF clipRect) {
			float blurKoeff = 1.1f;
			return new RectangleF(clipRect.X, clipRect.Y, clipRect.Width - blurKoeff, clipRect.Height - blurKoeff);
		}
		void DrawImage(Image image, RectangleF imageRect, byte transparency) {
			if(Size.Equals(image.Size, imageRect.Size.ToSize()))
				if(screenDpiX == image.HorizontalResolution && screenDpiY == image.VerticalResolution) {
					ImageSafeAccess.DrawUnscaled(activeGraphics, image, (int)imageRect.X, (int)imageRect.Y);
					return;
				}
			if(transparency == 0) {
				ImageSafeAccess.Draw(activeGraphics, image, imageRect);
				return;
			}
			ImageAttributes attributes = GetImageAttributes(transparency);
			ImageSafeAccess.Draw(activeGraphics, image, imageRect, attributes);
		}
		public void RenderBorder(Color color, Rectangle rect, int width) {
			cache.DrawRectangle(cache.GetPen(color, width), rect);
		}
	}
}
