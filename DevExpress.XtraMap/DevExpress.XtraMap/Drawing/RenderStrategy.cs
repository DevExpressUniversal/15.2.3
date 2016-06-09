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
using System.Linq;
using System.Drawing;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap.Drawing {
	public abstract class RenderStrategyBase : MapDisposableObject {
		readonly RenderController controller;
		protected RenderController Controller { get { return controller; } }
		protected InnerMap Map { get { return Controller.Map; } }
		public virtual Rectangle Bounds { get { return Map.ContentRectangle; } }
		protected RenderStrategyBase(RenderController controller) {
			this.controller = controller;
		}
		public abstract void DoAction(Action action);
		public abstract IRenderer Renderer { get; set; }
		public abstract void OnClientSizeChanged(Rectangle clientRectangle);
		public abstract void SetRenderer(IRenderer renderer);
		public abstract void UpdateRenderer(RenderMode renderMode, IntPtr handle);
		public abstract void UpdateSelectedRegion(Rectangle region);
		public abstract void UpdateRenderItems(IRenderer renderer, LayerBase updatedLayer);
		public abstract void Render(Graphics gr);
		public abstract void ForceRender();
		public abstract void Pause(bool suspend, bool shouldInvalidate);
#if DEBUG
		public abstract void ShowDebugInfo(bool value);
#endif
	}
	public class RuntimeRenderStrategy : RenderStrategyBase {
		RenderWorker renderWorker;
		IRenderer renderer;
		public override Rectangle Bounds { get { return renderWorker.Bounds; } }
		public RenderWorker RenderWorker { get { return renderWorker; } }
		public override IRenderer Renderer {
			get { return renderer; }
			set {
				if(Object.Equals(renderer, value))
					return;
				SetRenderer(value);
			}
		}
		public RuntimeRenderStrategy(RenderController controller)
			: base(controller) {
			this.renderWorker = CreateRenderWorker();
		}
		protected virtual RenderWorker CreateRenderWorker() {
			return new RenderWorker(Controller);
		}
		public override void DoAction(Action action) {
			renderWorker.DoInterlocked(action);
		}
		protected override void DisposeOverride() {
			base.DisposeOverride();
			if(renderWorker != null) {
				renderWorker.Dispose();
				renderWorker = null;
			}
			if(renderer != null) {
				MapUtils.DisposeObject(renderer);
				renderer = null;
			}
		}
		void SetRendererInternal(IRenderer renderer) {
			this.renderer = renderer;
			renderWorker.Renderer = renderer;
			Controller.UpdateRenderItems(null);
		}
		public override void SetRenderer(IRenderer renderer) {
			DoAction(() => {
				SetRendererInternal(renderer);
			});
		}
		public override void UpdateRenderer(RenderMode renderMode, IntPtr handle) {
			DoAction(() => {
				Controller.Graphics = Controller.CreateGraphics();
				SetRendererInternal(Controller.CreateRenderer(renderMode, handle));
				Renderer.Initialize(Controller.Graphics, Bounds.Size);
				Controller.UpdateRenderItems(null);
				renderWorker.Context = Controller.Graphics;
				renderWorker.Renderer = Renderer;
			});
		}
		public override void OnClientSizeChanged(Rectangle clientRectangle) {
			if(RectUtils.IsBoundsEmpty(clientRectangle)) {
				DoAction(() => { renderWorker.Bounds = Rectangle.Empty; });
				return;
			}
			DoAction(() => {
				renderWorker.Bounds = clientRectangle;
				Graphics gr = Controller.CreateGraphics();
				Controller.Graphics = gr;
				renderWorker.Context = gr;
				Renderer.Initialize(gr, Map.ClientRectangle.Size);
				Controller.CreateMapControlViewInfo();
			});
		}
		public override void UpdateSelectedRegion(Rectangle region) {
			this.renderWorker.SelectedRegion = region;
		}
		public override void UpdateRenderItems(IRenderer renderer, LayerBase updatedLayer) {
			renderer.StartUpdateItems();
			try {
				if (updatedLayer != null)
					UpdateItemsInLayer(renderer, updatedLayer);
				else {
					UpdateItemsInAllLayers(renderer);
				}
			} finally {
				renderer.EndUpdateItems();
			}
		}
		void UpdateItemsInAllLayers(IRenderer renderer) {
			foreach (LayerBase layer in Map.ActualLayers)
				UpdateItemsInLayer(renderer, layer);
			if (Map.MiniMap != null) {
				foreach (MiniMapLayerBase layer in Map.MiniMap.Layers)
					UpdateItemsInLayer(renderer, layer.InnerLayer);
			}
		}
		void UpdateItemsInLayer(IRenderer renderer, LayerBase layer) {
			if(layer != null && renderer != null)
				lock(layer.UpdateLocker) {
					renderer.UpdateItems(layer);
				}
		}
		public override void Pause(bool suspend, bool shouldInvalidate) {
			renderWorker.Pause(suspend, shouldInvalidate);
		}
		public override void ForceRender() {
			Render(null);
		}
		public override void Render(Graphics gr) {
			Controller.UpdateRenderContext(false);
			renderWorker.Invalidate();
		}
#if DEBUG
		public override void ShowDebugInfo(bool value) {
			DoAction(() => {
				renderWorker.ShowDebugInfo = value;
			});
		}
#endif
	}
	public class DesigntimeRenderStrategy : RenderStrategyBase {
		public DesigntimeRenderStrategy(RenderController controller)
			: base(controller) {
		}
		public override IRenderer Renderer { get { return null; } set { ; } }
		public override void DoAction(Action action) {
			action();
		}
		public override void SetRenderer(IRenderer renderer) {
		}
		public override void UpdateRenderer(RenderMode renderMode, IntPtr handle) {
		}
		public override void OnClientSizeChanged(Rectangle clientRectangle) {
			if(RectUtils.IsBoundsEmpty(clientRectangle))
				return;
			Controller.Graphics = Controller.CreateGraphics(); 
			Controller.CreateMapControlViewInfo();
		}
		public override void UpdateSelectedRegion(Rectangle region) {
		}
		public override void UpdateRenderItems(IRenderer renderer, LayerBase updatedLayer) {
		}
		public override void Pause(bool suspend, bool shouldInvalidate) {
		}
		public override void ForceRender() {
			Map.OwnedControl.Refresh();
		}
		public override void Render(Graphics gr) {
			if(gr == null) return;
			lock(gr) {
				Controller.Graphics = gr;
				Controller.CreateMapControlViewInfo();
				Controller.UpdateRenderContext(false);
				using(DirectRenderHelper renderHelper = new DirectRenderHelper(Controller, null)) {
					renderHelper.Render(gr, Bounds);
				}
			}
		}
#if DEBUG
		public override void ShowDebugInfo(bool value) {
		}
#endif
	}
}
