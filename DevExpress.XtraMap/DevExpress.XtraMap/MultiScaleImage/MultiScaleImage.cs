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

using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.Utils;
using System.ComponentModel;
namespace DevExpress.XtraMap {
	public interface ICacheOptionsProvider {
		CacheOptions CacheOptions { get; }
	}
}
namespace DevExpress.XtraMap.Native {
	public interface IMultiScaleImage {
		void RedrawVisualContent();
		MultiScaleTileBase CreateTile(MultiScaleViewport viewport, int x, int y);
		byte Transparency { get; }
	}
	public class MultiScaleImage : MapDisposableObject, IMultiScaleImage {
		const double DefaultViewportWidth = 300.0;
		const double DefaultViewportHeight = 300.0;
		const int DefaultInflateCount = 1;
		static readonly MapPoint DefaultViewportOrigin = MapPoint.Empty;
		MapPoint viewportOrigin = DefaultViewportOrigin;
		MapSize viewportSize = new MapSize(1.0, 1.0);
		MultiScaleViewport viewport;
		ImageTilesLayer layer;
		Size clientSize;
		internal MultiScaleViewport Viewport {
			get { return viewport; }
		}
		internal ImageTilesLayer Layer {
			get { return layer; }
		}
		internal Size ClientSize {
			get { return clientSize; }
			set {
				if (clientSize == value)
					return;
				clientSize = value;
				Invalidate();
			}
		}
		[
		Description("")
		]
		public MapPoint ViewportOrigin {
			get { return viewportOrigin; }
			set {
				if (viewportOrigin == value)
					return;
				viewportOrigin = value;
				OnViewportOriginChanged();
			}
		}
		void ResetViewportOrigin() { ViewportOrigin = DefaultViewportOrigin; }
		bool ShouldSerializeViewportOrigin() { return ViewportOrigin != DefaultViewportOrigin; }
		public MapSize ViewportSize {
			get { return viewportSize; }
			set {
				if(viewportSize == value)
					return;
				viewportSize = value;
				OnViewportSizeChanged();
			}
		}
		[
		DefaultValue(MultiScaleViewport.DefaultTileSource)]
		public MultiScaleTileSource Source {
			get { return viewport.TileSource; }
			set {
				viewport.TileSource = value;
				Invalidate();
			}
		}
		public MultiScaleImage(ImageTilesLayer layer) {
			Guard.ArgumentNotNull(layer, "layer");
			this.layer = layer;
			this.viewport = new MultiScaleViewport(this, DefaultInflateCount);
		}
		#region IMultiScaleImage
		MultiScaleTileBase IMultiScaleImage.CreateTile(MultiScaleViewport viewport, int x, int y) {
			return new MultiScaleTile(viewport, x, y);
		}
		void IMultiScaleImage.RedrawVisualContent() {
			if (Layer.Map != null)
				Layer.Map.Render();
		}
		byte IMultiScaleImage.Transparency { get { return Layer.Transparency; } }
		#endregion
		void OnViewportOriginChanged() {
			Invalidate();
		}
		void OnViewportSizeChanged() {
			Invalidate();
		}
		void Invalidate() {
			if (MapUtils.IsValidSize(ClientSize)) {
				Viewport.BeginUpdate();
				try {
					Viewport.ViewSize = new Size(ClientSize.Width, ClientSize.Height);
					Viewport.ContentOrigin = ViewportOrigin;
					Viewport.ContentSize = ViewportSize;
				}
				finally {
					Viewport.EndUpdate(true);
				}
			}
		}
		IEnumerable<IRenderItem> CreateRenderItems() {
			return Viewport.CreateRenderItems();
		}
		protected override void DisposeOverride() {
			if (viewport != null) {
				viewport.Dispose();
				viewport = null;
			}
		}
		internal void CheckTilesLoadCompleted() {
			if (Viewport.ShouldRaiseRequestDataLoadingEvent) {
				Layer.RaiseRequestDataLoading();
				Viewport.ShouldRaiseRequestDataLoadingEvent = false;
			}
			Viewport.UpdateTilesLoadCompleted();
			if (Viewport.ShouldRaiseDataLoadedEvent) {
				Layer.RaiseDataLoaded();
				Viewport.ShouldRaiseDataLoadedEvent = false;
			}
		}
		internal IEnumerable<IRenderItem> GetRenderItems() {
			return CreateRenderItems();
		}
		protected internal void StartLoadTiles() {
			if (Source != null && Source.Cache != null)
				Source.Cache.StartLoadTiles();
		}
	}
}
