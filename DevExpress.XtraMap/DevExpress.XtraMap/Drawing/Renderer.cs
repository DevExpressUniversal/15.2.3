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

using DevExpress.XtraMap.Native;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.XtraMap.Drawing {
	public enum RenderTransform {
		None,
		Locatable,
		Scalable,
		ImageTile
	}
	public abstract class RendererBase : MapDisposableObject, IRenderer {
		IRenderContext renderContext;
		Rectangle clipBounds;
		MapPoint renderOffset;
		double renderScaleFactorX;
		double renderScaleFactorY;
		double renderScale;
		RenderTransform currentTransform;
		bool canRenderer;
		protected bool CanRenderer { get { return canRenderer; } set { canRenderer = value; } }
		protected internal IRenderContext RenderContext { get { return renderContext; } }
		protected Rectangle ClipBounds { get { return clipBounds; } }
		protected virtual MapPoint RenderOffset { get { return renderOffset; } }
		protected double RenderScaleFactorX { get { return renderScaleFactorX; } }
		protected double RenderScaleFactorY { get { return renderScaleFactorY; } }
		protected double RenderScale { get { return renderScale; } }
		protected virtual bool ShouldDisposeContext { get { return true; } }
		protected RendererBase() {
			this.currentTransform = RenderTransform.None;
		}
		protected override void DisposeOverride() {
			DisposeCore();
			base.DisposeOverride();
		}
		#region IRenderer Members
		bool IRenderer.Initialize(object context, Size size) {
			SetViewSize(size);
			return Initialize(context);
		}
		void IRenderer.StartUpdateItems() {
			StartUpdateItems();
		}
		void IRenderer.UpdateItems(IRenderItemProvider provider) {
			UpdateItems(provider);
		}
		void IRenderer.EndUpdateItems() {
			EndUpdateItems();
		}
		void IRenderer.StartRender(IRenderContext renderContext) {
			CanRenderer = true;
			StartRender(renderContext);
		}
		void IRenderer.Render(IRenderItemProvider provider) {
			if(CanRenderer)
				Render(provider);
		}
		void IRenderer.RenderBorder(Graphics gr, IRenderStyleProvider provider) {
			if(CanRenderer)
				RenderBorder(gr, provider);
		}
		void IRenderer.RenderBorderFinally(Graphics gr, IRenderStyleProvider provider) {
			if(CanRenderer)
				RenderBorderFinally(gr, provider);
		}
		void IRenderer.RenderOverlays(IRenderOverlayProvider provider) {
			if(CanRenderer)
				RenderOverlays(provider);
		}
		void IRenderer.BeforeRenderOverlay() {
			if(CanRenderer)
				BeforeRenderOverlay();
		}
		void IRenderer.DrawRectangle(Rectangle rect, Color fill, Color stroke) {
			if(CanRenderer)
				DrawRectangle(rect, fill, stroke);
		}
		void IRenderer.EndRender() {
			if(CanRenderer)
				EndRender();
		}
		void IRenderer.DisposeResources() {
			DisposeCore();
		}
		#endregion
		#region IResourceHolderCreator Members
		IRenderItemResourceHolder IRenderer.CreateResourceHolder(IRenderItemProvider provider, IRenderItem owner) {
			return CreateItemResourceHolder(provider, owner);
		}
		#endregion
		protected virtual void SetClip(Rectangle clipRect) {
		}
		protected virtual void ResetClip() {
		}
		protected virtual void SetViewSize(Size size) {
		}
		protected virtual void StartUpdateItems() {
		}
		protected virtual void UpdateItems(IRenderItemProvider provider) {
			foreach(IRenderItem item in provider.RenderItems) {
				item.PrepareGeometry();
				IRenderItemContainer container = item as IRenderItemContainer;
				if(container != null) {
					if(container.RenderItself)
						item.SetResourceHolder(this, provider);
					foreach(IRenderItem part in container.Items)
						part.SetResourceHolder(this, provider);
				} else
					item.SetResourceHolder(this, provider);
			}
		}
		protected virtual void EndUpdateItems() {
		}
		protected virtual void StartRender(IRenderContext renderContext) {
			SetRenderContext(renderContext);
		}
		protected virtual void Render(IRenderItemProvider provider) {
			SetRenderParam(provider);
			if (!provider.RenderClipBounds.IsEmpty)
				SetClip(provider.RenderClipBounds);
			RenderItems(provider);
			ResetClip();
			ResetRenderTransform();
		}
		protected virtual void SetRenderParam() {
		}
		protected virtual void DrawRectangle(Rectangle rect, Color fill, Color stroke) {
		}
		protected virtual void EndRender() {
		}
		protected virtual void ResetRenderTransform() {
		}
		protected virtual void SetScaledTransform(bool antiAliasing) {
		}
		protected virtual void SetLocatableTransform(ILocatableRenderItem locatableItem, bool antiAliasing) {
		}
		protected virtual void RenderBorder(Graphics gr, IRenderStyleProvider provider) {
		}
		protected virtual void RenderBorderFinally(Graphics gr, IRenderStyleProvider provider) {
		}
		protected virtual void RenderOverlays(IRenderOverlayProvider provider) {
			foreach(IRenderOverlay overlay in provider.GetOverlays())
				if(overlay.OverlayImage != null)
					RenderOverlay(overlay);
		}
		protected virtual void BeforeRenderOverlay() {
		}
		protected virtual void SetImageTileTransform(ILocatableRenderItem locatableItem, bool antiAliasing) {
		}
		protected virtual void RenderOverlay(IRenderOverlay overlay) {
		}
		protected virtual void RenderItems(IRenderItemProvider provider) {
			IEnumerable<IRenderItem> renderItems = provider.RenderItems;
			List<IRenderItem> topItems = new List<IRenderItem>();
			List<IRenderShapeTitle> titles = new List<IRenderShapeTitle>();
			foreach(IRenderItem item in renderItems) {
				item.PrepareGeometry();
				IRenderItemContainer container = item as IRenderItemContainer;
				if(container != null)
					foreach(IRenderItem containerItem in container.Items)
						containerItem.PrepareGeometry();
				if(!item.Visible) continue;
				UpdateResourceHolder(item, provider);
				ColorizeItem(provider, item);
				if(item.Title != null && item.Title.Visible)
					titles.Add(item.Title);
				IInteractiveElement interactiveItem = item as IInteractiveElement;
				if(interactiveItem != null) {
					if(interactiveItem.IsHighlighted || interactiveItem.IsSelected) {
						topItems.Add(item);
						continue;
					}
				}
				RenderItem(item);
			}
			foreach(IRenderItem topItem in topItems) 
				RenderItem(topItem);
			foreach(IRenderShapeTitle title in titles)
				RenderItemTitle(title);
		}
		protected virtual void ItemRendering(IRenderItem item) {
			IMapItemGeometry geometry = item.Geometry;
			SetTransform(item);
			if(geometry is IUnitGeometry && item.ResourceHolder != RenderItemResourceHolder.Empty)
				RenderGeometry(item.ResourceHolder, item.Style);
			else
				RenderItemImageGeometry(item.Geometry as IImageGeometry);
		}
		protected abstract IRenderItemResourceHolder CreateItemResourceHolder(IRenderItemProvider provider, IRenderItem owner);
		protected abstract void RenderImage(IImageGeometry geometry);
		protected abstract bool Initialize(object context);
		protected abstract void RenderGeometry(IRenderItemResourceHolder holder, IRenderItemStyle style);
		protected abstract void DisposeCore();
		void SetRenderParam(IRenderItemProvider provider) {
			this.clipBounds = provider.RenderClipBounds;
			this.renderOffset = provider.RenderOffset;
			this.renderScaleFactorX = provider.RenderScaleFactorX;
			this.renderScaleFactorY = provider.RenderScaleFactorY;
			this.renderScale = provider.RenderScale;
			this.currentTransform = RenderTransform.None;
			SetRenderParam();
		}
		void UpdateResourceHolder(IRenderItem item, IRenderItemProvider provider) {
			IRenderItemContainer container = item as IRenderItemContainer;
			if(container != null) {
				foreach(IRenderItem part in container.Items)
					if(part.ForceUpdateResourceHolder)
						UpdateResourceHolderCore(part, provider);
			}
			if(item.ForceUpdateResourceHolder)
				UpdateResourceHolderCore(item, provider);
		}
		void UpdateResourceHolderCore(IRenderItem item, IRenderItemProvider provider) {
			if(item.ResourceHolder == RenderItemResourceHolder.Empty)
				item.SetResourceHolder(this, provider);
			item.ResourceHolder.Update();
			item.ForceUpdateResourceHolder = false;
		}
		void RenderItemTitle(IRenderShapeTitle title) {
				IImageGeometry imageGeometry = title.Geometry as IImageGeometry;
				SetShapeTitleTransform(title);
				RenderItemImageGeometry(imageGeometry);
		}
		void RenderItemImageGeometry(IImageGeometry geometry) {
			if(geometry != null && geometry.Image != null)
				RenderImage(geometry);
		}
		void SetShapeTitleTransform(IRenderShapeTitle title) {
			this.currentTransform = RenderTransform.Locatable;
			SetLocatableTransform((ILocatableRenderItem)title, title.UseAntiAliasing);
		}
		void SetTransform(IRenderItem item) {
			ILocatableRenderItem locatableItem = item as ILocatableRenderItem;
			bool antiAliasing = item.UseAntiAliasing;
			if(locatableItem == null) {
				if(this.currentTransform != RenderTransform.Scalable) {
					SetScaledTransform(antiAliasing);
					this.currentTransform = RenderTransform.Scalable;
				}
			} else {
				this.currentTransform = RenderTransform.Locatable;
				if(item is BitmapTile)
					SetImageTileTransform(locatableItem, antiAliasing);
				else
					SetLocatableTransform(locatableItem, antiAliasing);
			}
		}
		protected internal void RenderItem(IRenderItem item) {
			item.OnRender();
			if(RenderContext.IsExport) {
				if(!item.CanExport())
					return;
			}
			lock(item.UpdateLocker) {
				IRenderItemContainer container = item as IRenderItemContainer;
				if(container != null) {
					foreach(IRenderItem part in container.Items)
						ItemRendering(part);
					if(container.RenderItself)
						ItemRendering(item);
				}
				else
					ItemRendering(item);
			}
		}
		protected internal static void ColorizeItem(IRenderItemProvider provider, IRenderItem item) {
			MapColorizer colorizer = provider.GetColorizer(item);
			if(colorizer != null) {
				IColorizerElement colorizerElement = item as IColorizerElement;
				if(colorizerElement != null && MapUtils.IsColorEmpty(colorizerElement.ColorizerColor))
					colorizer.ColorizeElement(colorizerElement);
			}
			IRenderItemContainer container = item as IRenderItemContainer;
			if(container != null) {
				foreach(IRenderItem part in container.Items)
					ColorizeItem(provider, part);
			}
		}
		internal void SetRenderContext(IRenderContext renderContext) {
			this.renderContext = renderContext;
		}
	}
}
