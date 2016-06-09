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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraMap.Drawing {
	public class DirectRenderHelper : MapDisposableObject {
		readonly RenderController controller;
		IRenderer renderer;
		protected IRenderOverlayProvider OverlayProvider { get { return (IRenderOverlayProvider)controller; } }
		protected IRenderContextProvider ContextProvider { get { return (IRenderContextProvider)controller; } }
		protected IRenderStyleProvider StyleProvider { get { return (IRenderStyleProvider)controller; } }
		protected IRenderMiniMapProvider MiniMapProvider { get { return (IRenderMiniMapProvider)controller; } }
		protected bool ShouldRenderContent { get; set; }
		protected internal IRenderer Renderer { get { return renderer; } }
		protected bool ShouldRenderMiniMap { get { return !ShouldRenderContent; } }
		protected Printing.PrintOptions PrintOptions { get; set; }
		public DirectRenderHelper(RenderController renderController, Printing.PrintOptions printOptions) {
			bool printing = printOptions != null;
			ShouldRenderContent = printing;
			this.controller = renderController;
			PrintOptions = printOptions;
			this.renderer = CreateRenderer(printing);
		}
		IRenderer CreateRenderer(bool print) {
			if(print)
				return new PrintRenderer();
			return new DesignModeRenderer();
		}
		protected override void DisposeOverride() {
			base.DisposeOverride();
			MapUtils.DisposeObject(renderer);
		}
		public void Render(Graphics graphics, Rectangle rectangle) {
			renderer.Initialize(graphics, rectangle.Size);
			renderer.StartRender(ContextProvider.RenderContext);
			try {
				if (ShouldRenderContent) {
					RenderContent();
					if (PrintOptions != null && PrintOptions.PrintMiniMap)
						RenderMiniMap();
				}
				else
					RenderMiniMapBorder();
				renderer.BeforeRenderOverlay();
				renderer.RenderOverlays(OverlayProvider);
			} finally {
				renderer.RenderBorder(graphics, StyleProvider);
				renderer.EndRender();
			}
		}
		void RenderMiniMap() {
			IRenderMiniMapContentProvider contentProvider = MiniMapProvider.ContentProvider;
			if (contentProvider == null)
				return;
			IRenderContext context = contentProvider.RenderContext;
			Renderer.DrawRectangle(context.Bounds, context.BackColor, Color.Empty);
			foreach (LayerBase layer in contentProvider.Layers)
				RenderItemProvider(layer, context);
			Rectangle viewport = contentProvider.ViewportInPixels;
			Rectangle bounds = context.Bounds;
			viewport.Offset(bounds.Location);
			IRenderStyleProvider styleProvider = MiniMapProvider.StyleProvider;
			if (styleProvider != null) {
				BorderedElementStyle regionStyle = styleProvider.BorderedElementStyle;
				Renderer.DrawRectangle(viewport, regionStyle.Fill, regionStyle.Stroke);
			}
			RenderMiniMapBorder();
		}
		void RenderMiniMapBorder() {
			IRenderStyleProvider styleProvider = MiniMapProvider.StyleProvider;
			if(styleProvider != null) Renderer.RenderBorder(null, styleProvider);
		}
		void RenderItemProvider(LayerBase layer, IRenderContext context) {
			if (layer == null || !layer.CheckVisibility())
				return;
			IRenderItemProvider provider = layer as IRenderItemProvider;
			if (provider != null) {
				lock (layer.UpdateLocker) {
					provider.UpdateRenderParameters(context);
					provider.PrepareData();
					Renderer.Render(provider);
					provider.OnRenderComplete();
				}
			}
		}
		void RenderContent() {
			IRenderContext renderContext = ContextProvider.RenderContext;
			foreach(LayerBase layer in controller.RenderLayers)
				RenderItemProvider(layer, renderContext);
			}
	}
	public class PrintRenderer : GdiRenderer {
		protected override bool CanRenderOverlay(IRenderOverlay overlay) {
			return overlay.Printable;
		}
	}
	public class DesignModeRenderer : GdiRenderer {
		protected override bool ShouldDisposeContext { get { return false; } }
		protected override bool CanRenderOverlay(IRenderOverlay overlay) {
			return overlay.ShowInDesign;
		}
	}
}
