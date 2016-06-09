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
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using DevExpress.XtraMap.Native;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Map.Native;
using DevExpress.Utils;
namespace DevExpress.XtraMap.Drawing {
	public class RenderWorker : MapDisposableObject, IRenderContextProvider {
		readonly Thread renderThread;
		readonly ICollection<LayerBase> layers;
		readonly LegendCollection legends;
		readonly IRenderContextProvider contentProvider;
		readonly IRenderOverlayProvider overlayProvider;
		readonly IRenderStyleProvider styleProvider;
		readonly IRenderMiniMapProvider miniMapProvider;
		readonly IBoundingRectUpdater boundingRectUpdater;
		readonly List<object> objectsToDispose = new List<object>();
		ManualResetEvent readyEvent;
		AutoResetEvent drawingEvent;
		IRenderer renderer;
		object context;
		bool isSuspend = false;
		Rectangle bounds = Rectangle.Empty;
		bool termination;
		object renderLocker = new object();
		IRenderContext renderContext;
		protected LegendCollection Legends { get { return legends; } }
		protected ICollection<LayerBase> Layers { get { return layers; } }
		IRenderContext IRenderContextProvider.RenderContext { get { return renderContext; } }
		public bool IsSuspend { get { return isSuspend; } }
#if DEBUG
		Stopwatch sw = new Stopwatch();
		long lastTime;
		double fps;
		long fpsCounts;
		public bool ShowDebugInfo { get; set; }
		public double AverageFps { get; private set; }
#endif
		public object Context {
			get { return context; }
			set {
				if(Object.Equals(context, value))
					return;
				context = value;
			}
		}
		public IRenderer Renderer {
			get { return renderer; }
			set {
				if(Object.Equals(renderer, value))
					return;
				RegisterObjectToDispose(renderer);
				renderer = value;
			}
		}
		public Rectangle SelectedRegion { get; set; }
		public Rectangle Bounds {
			get {
				return bounds;
			}
			set {
				if(Object.Equals(bounds, value))
					return;
				bounds = value;
			}
		}
		public RenderWorker(RenderController controller) {
			this.contentProvider = (IRenderContextProvider)controller;
			this.overlayProvider = (IRenderOverlayProvider)controller;
			this.styleProvider = (IRenderStyleProvider)controller;
			this.miniMapProvider = (IRenderMiniMapProvider)controller;
			this.boundingRectUpdater = (IBoundingRectUpdater)controller;
			this.layers = controller.RenderLayers;
			this.legends = controller.Legends;
			this.renderContext = DevExpress.XtraMap.Drawing.RenderContext.Empty;
			this.renderThread = MapUtils.CreateThread(Render, "Render thread");
			Start();
		}
		protected virtual void Start() {
			drawingEvent = new AutoResetEvent(true);
			readyEvent = new ManualResetEvent(true);
			renderThread.Start();
		}
		protected virtual void Stop() {
			renderThread.Join();
			drawingEvent.Close();
			readyEvent.Close();
		}
		protected override void DisposeOverride() {
			Terminate();
			Stop();
			MapUtils.DisposeObject(Context);
			context = null;
			bounds = Rectangle.Empty;
			renderer = null;
			base.DisposeOverride();
		}
		void Render() {
			while(true) {
				drawingEvent.WaitOne();
				if(termination)
					break;
				if(!CanRender()) {
					readyEvent.Set();
					continue;
				}
				lock(renderLocker) {
					readyEvent.Reset();
					lock(Layers) {
						lock(Context) {
							this.renderContext = this.contentProvider.RenderContext;
#if DEBUG
								StartGetDebugInformation();
#endif
							Renderer.StartRender(this.renderContext);
							try {
								RenderContent();
								PrepareVectorItemsLayers(Layers);
								RenderSelection();
								RenderMiniMap();
								Renderer.BeforeRenderOverlay();
								Renderer.RenderOverlays(this.overlayProvider);
								Renderer.RenderBorder(Context as Graphics, miniMapProvider.StyleProvider);
								Renderer.RenderBorder(Context as Graphics, styleProvider);
							} finally {
								Renderer.EndRender();
								Renderer.RenderBorderFinally(Context as Graphics, miniMapProvider.StyleProvider);
								Renderer.RenderBorderFinally(Context as Graphics, styleProvider);
#if DEBUG
								CalculateFps();
								if(ShowDebugInfo) RenderDebugInformation();
								OnEndRender();
#endif
							}
							if(termination)
								break;
						}
					}
				}
				readyEvent.Set();
			}
			OnTerminateThread();
			readyEvent.Set();
		}
		protected void RegisterObjectToDispose(object obj) {
			IDisposable dispObject = obj as IDisposable;
			if(dispObject == null) return;
			DoInterlocked(() => {
				if(!objectsToDispose.Contains(obj))
					objectsToDispose.Add(obj);
			});
		}
		void OnEndRender() {
			foreach(object item in objectsToDispose)
				MapUtils.DisposeObject(item);
			objectsToDispose.Clear();
		}
		void OnTerminateThread() {
			if(Renderer != null) Renderer.DisposeResources();
		}
		internal bool CanRender() {
			return renderer != null && Context != null && Bounds != Rectangle.Empty && !this.isSuspend;
		}
		void RenderSelection() {
			if(!SelectedRegion.IsEmpty) {
				BorderedElementStyle regionStyle = this.styleProvider.BorderedElementStyle;
				Renderer.DrawRectangle(SelectedRegion, regionStyle.Fill, regionStyle.Stroke);
			}
		}
		void RenderMiniMap() {
			IRenderMiniMapContentProvider contentProvider = miniMapProvider.ContentProvider;
			if(contentProvider == null)
				return;
			IRenderContext context = contentProvider.RenderContext;
			Renderer.DrawRectangle(context.Bounds, context.BackColor, Color.Empty);
			foreach (LayerBase layer in contentProvider.Layers)
				RenderItemProvider(layer, context);
			PrepareVectorItemsLayers(contentProvider.Layers);
			Rectangle viewport = contentProvider.ViewportInPixels;
			Rectangle bounds = context.Bounds;
			viewport.Offset(bounds.Location);
			IRenderStyleProvider styleProvider = miniMapProvider.StyleProvider;
			if(styleProvider != null) {
				BorderedElementStyle regionStyle = styleProvider.BorderedElementStyle;
				Renderer.DrawRectangle(viewport, regionStyle.Fill, regionStyle.Stroke);
			}
		}
		void PrepareVectorItemsLayers(ICollection<LayerBase> layers) {
			foreach(LayerBase layer in layers) {
				MapItemsLayerBase vectorLayer = layer as MapItemsLayerBase;
				if(vectorLayer != null && vectorLayer.NeedEnsureBoundingRect)
					vectorLayer.EnsureBoundingRect();
			}
			if(boundingRectUpdater.NeedUpdateBoundingRect)
				boundingRectUpdater.EnsureBoundingRect(layers);
		}
		void RenderContent() {
			foreach(LayerBase layer in Layers) 
				RenderItemProvider(layer, renderContext);
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
#if DEBUG
		void StartGetDebugInformation() {
			sw.Reset();
			sw.Start();
			lastTime = sw.ElapsedMilliseconds;
		}
  void CalculateFps() {
			double delta = (double)(sw.ElapsedMilliseconds - lastTime);
			fps = 1.0 / (delta / 1000.0);
			lastTime = sw.ElapsedMilliseconds;
			if (!double.IsInfinity(fps) && !double.IsNaN(fps)) {
				AverageFps = ((double)fpsCounts / (fpsCounts + 1)) * AverageFps + fps / (double)(fpsCounts + 1);
				fpsCounts++;
			} 
		}
		void RenderDebugInformation() {
			Graphics gr = Context as Graphics;
			if(gr == null || renderContext == null)
				return;
			using(Brush textBrush = new SolidBrush(LegendAppearance.DefaultElementColor)) {
				using(Font textFont = new Font("Arial", 10)) {
					Point pt = new Point(16, 16);
					int textMargin = 4;
					RenderMode mode = typeof(D3DRenderer) == renderer.GetType() ? RenderMode.DirectX : RenderMode.GdiPlus;
					string centerString;
					if (renderContext.CenterPoint is GeoPoint)
						centerString = string.Format("Lat: {0}\r\nLon: {1}", MathUtils.GetLatitudeString(renderContext.CenterPoint.GetY()), MathUtils.GetLongituteString(renderContext.CenterPoint.GetX()));
					else
						centerString = string.Format("X: {0}\r\nY: {1}", renderContext.CenterPoint.GetX(), renderContext.CenterPoint.GetY());
					string s = String.Format("{0}\r\nFPS: {1:F3}\r\nZoom: {2:F3}\r\nCenter:\r\n{3} \r\nAverageFps:{4}",
										  mode, fps, renderContext.ZoomLevel, centerString, AverageFps.ToString());
					Size size = gr.MeasureString(s, textFont).ToSize();
					Rectangle rect = new Rectangle(pt.X, pt.Y, size.Width + textMargin * 2, size.Height + textMargin * 2);
					using(Brush br = new SolidBrush(BackgroundStyle.DefaultOverlayBackColor)) {
						gr.FillRectangle(br, rect);
					}
					gr.DrawString(s, textFont, textBrush, new Point(pt.X + textMargin, pt.Y + textMargin));
				}
			}
		}
#endif
		protected virtual void Terminate() {
			termination = true;
			drawingEvent.Set();
		}
		protected internal virtual void DoInterlocked(Action action) {
			readyEvent.WaitOne();
			lock(renderLocker) {
				drawingEvent.Reset();
				action.Invoke();
			}
		}
		protected internal virtual void Invalidate() {
			readyEvent.Reset();
			drawingEvent.Set();
		}
		protected internal virtual void Pause(bool suspend, bool shouldInvalidate) {
			if(this.isSuspend == suspend)
				return;
			DoInterlocked(() => {
				this.isSuspend = suspend;
			});
			if(!suspend && shouldInvalidate)
				Invalidate();
		}
	}
}
