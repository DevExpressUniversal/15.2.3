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
using System.Linq;
using System.Text;
using DevExpress.Diagram.Core;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Utils;
namespace DevExpress.XtraDiagram.Paint {
	public class DiagramPaintCache : IDisposable {
		Image backgroundImg;
		Dictionary<PageImageCacheKey, Image> pageImageCache;
		Dictionary<ShapeShadowImageCacheKey, Image> shapeShadowCache;
		public DiagramPaintCache() {
			this.backgroundImg = null;
			this.pageImageCache = new Dictionary<PageImageCacheKey, Image>();
			this.shapeShadowCache = new Dictionary<ShapeShadowImageCacheKey, Image>();
		}
		public Image GetPageImage(DiagramPageObjectInfoArgs drawArgs, Rectangle bounds, PageDrawDelegate pageDrawer) {
			Image result = null;
			PageImageCacheKey key = new PageImageCacheKey(bounds.Size);
			if(!this.pageImageCache.TryGetValue(key, out result)) {
				Rectangle rect = bounds;
				rect.X = rect.Y = 0;
				result = CreatePageImage(drawArgs, rect, pageDrawer);
				this.pageImageCache.Add(key, result);
			}
			return result;
		}
		protected Image CreatePageImage(DiagramPageObjectInfoArgs drawArgs, Rectangle bounds, PageDrawDelegate pageDrawer) {
			Bitmap pageImage = new Bitmap(Math.Max(8, bounds.Width), Math.Max(8, bounds.Height));
			pageImage.DrawUsingGraphics(cache => {
				GraphicsCache controlGraphicsCache = drawArgs.Cache;
				drawArgs.Cache = cache;
				try {
					pageDrawer(drawArgs, bounds);
				}
				finally {
					drawArgs.Cache = controlGraphicsCache;
				}
			});
			return pageImage;
		}
		public Image GetBackgroundImage(Rectangle bounds, SkinElementInfo elementInfo) {
			if(this.backgroundImg == null || this.backgroundImg.Size != bounds.Size) {
				bounds.X = bounds.Y = 0;
				DestroyBackgroundImage();
				this.backgroundImg = new Bitmap(Math.Max(8, bounds.Width), Math.Max(8, bounds.Height));
				this.backgroundImg.DrawUsingGraphics(cache => {
					elementInfo.ImageIndex = 0;
					ObjectPainter.DrawObject(cache, SkinElementPainter.Default, elementInfo);
				});
			}
			return this.backgroundImg;
		}
		public Image GetShapeShadowImage(DiagramItemDrawArgs drawArgs, ShapeShadowDrawDelegate shapeShadowDrawer) {
			Image result = null;
			ShapeShadowImageCacheKey key = new ShapeShadowImageCacheKey(drawArgs.ItemSize, drawArgs.Shape, drawArgs.ShadowColor, drawArgs.Angle, drawArgs.ShapeParameters);
			if(!this.shapeShadowCache.TryGetValue(key, out result)) {
				result = CreateShapeShadowImage(drawArgs, shapeShadowDrawer);
				this.shapeShadowCache.Add(key, result);
			}
			return result;
		}
		protected Image CreateShapeShadowImage(DiagramItemDrawArgs drawArgs, ShapeShadowDrawDelegate shapeShadowDrawer) {
			Bitmap image = new Bitmap(drawArgs.ItemSize.Width, drawArgs.ItemSize.Height + drawArgs.ShadowSize);
			image.DrawUsingGraphics(cache => {
				cache.Graphics.SmoothingMode = SmoothingMode.None;
				shapeShadowDrawer(drawArgs, cache);
			});
			return image;
		}
		public int PageImageCacheSize { get { return this.pageImageCache.Count; } }
		public int ShapeShadowCacheSize { get { return this.shapeShadowCache.Count; } }
		public void ClearCache() {
			ClearPageCache();
			ClearShadowCache();
		}
		public void ClearPageCache() { DestroyPageImageCache(); }
		public void ClearShadowCache() { DestroyShapeShadowCache(); }
		protected void DestroyBackgroundImage() {
			this.backgroundImg.DoIfNotNull(image => image.Dispose());
		}
		protected void DestroyPageImageCache() {
			if(this.pageImageCache != null) {
				this.pageImageCache.ForEachValue(image => image.Dispose());
				this.pageImageCache.Clear();
			}
		}
		protected void DestroyShapeShadowCache() {
			if(this.shapeShadowCache != null) {
				this.shapeShadowCache.ForEachValue(image => image.Dispose());
				this.shapeShadowCache.Clear();
			}
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				DestroyBackgroundImage();
				DestroyPageImageCache();
				DestroyShapeShadowCache();
			}
			this.backgroundImg = null;
			this.pageImageCache = null;
			this.shapeShadowCache = null;
		}
	}
	public struct PageImageCacheKey {
		readonly Size pageSize;
		public PageImageCacheKey(Size pageSize) {
			this.pageSize = pageSize;
		}
		public Size PageSize { get { return pageSize; } }
		public override bool Equals(object obj) {
			PageImageCacheKey other = (PageImageCacheKey)obj;
			return pageSize == other.PageSize;
		}
		public override int GetHashCode() {
			return (PageSize.Width + PageSize.Height);
		}
		public override string ToString() { return PageSize.ToString(); }
	}
	public delegate void PageDrawDelegate(DiagramPageObjectInfoArgs drawArgs, Rectangle bounds);
	public delegate void ShapeShadowDrawDelegate(DiagramItemDrawArgs drawArgs, GraphicsCache cache);
	public struct ShapeShadowImageCacheKey {
		readonly Size size;
		readonly ShapeDescription shape;
		readonly Color backColor;
		readonly DoubleCollection parameters;
		readonly double angle;
		public ShapeShadowImageCacheKey(Size size, ShapeDescription shape, Color backColor, double angle, DoubleCollection parameters) {
			this.size = size;
			this.shape = shape;
			this.backColor = backColor;
			this.angle = angle;
			this.parameters = parameters;
		}
		public Size Size { get { return size; } }
		public Color BackColor { get { return backColor; } }
		public ShapeDescription Shape { get { return shape; } }
		public double Angle { get { return angle; } }
		public DoubleCollection Parameters { get { return parameters; } }
		public override bool Equals(object obj) {
			ShapeShadowImageCacheKey other = (ShapeShadowImageCacheKey)obj;
			return Size == other.Size && ReferenceEquals(Shape, other.Shape) && BackColor.Equals(other.BackColor) && MathUtils.IsEquals(Angle, other.Angle) && ReferenceEquals(Parameters, other.Parameters);
		}
		public override int GetHashCode() {
			int hashCode = (Size.Width + Size.Height) + shape.GetHashCode() + BackColor.GetHashCode() + Angle.GetHashCode();
			if(Parameters != null) {
				hashCode += Parameters.GetHashCode();
			}
			return hashCode;
		}
		public override string ToString() { return Size.ToString(); }
	}
}
