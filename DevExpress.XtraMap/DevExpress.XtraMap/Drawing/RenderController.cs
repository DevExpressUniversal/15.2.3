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

using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
namespace DevExpress.XtraMap.Drawing {
	[Flags]
	public enum UpdateActionType { None = 0, Render = 1, InvalidateViewInfo = 2, UpdateStyles = 4, UpdateImages = 8, ClearSkinElementCache = 16, ArrangeMiniMap = 32 };
	public class RenderController : MapDisposableObject, IRenderContextProvider, IRenderOverlayProvider, IRenderStyleProvider, 
		IBatchUpdateable, IBatchUpdateHandler, IViewInfoStyleProvider, IRenderMiniMapProvider, IBoundingRectUpdater {
		internal const ViewInfoUpdateType DefaultViewInfoUpdateType = ViewInfoUpdateType.All;
		readonly InnerMap map;
		MapDeferredUpdates deferredUpdates;
		bool isInitialized;
		RenderStrategyBase renderStrategy;
		Dictionary<int, RenderOverlay> renderOverlaysCache;
		ZoomTrackBarController zoomTrackBarController;
		ScrollButtonsController scrollButtonsController;
		ViewInfoBuilderBase viewInfoBuilder;
		MapUIElementsPainter painter;
		IMapItemStyleProvider itemStyleProvider;
		ViewInfoStyleProviderBase viewInfoStyleProvider;
		IRenderContext renderContext;
		ViewInfoUpdateType viewInfoUpdateType = ViewInfoUpdateType.None;
		Graphics graphics;
		int renderSuspendCount = 0;
		MapViewInfo viewInfo;
		CoordPoint activePoint;
		double kilometersScale;
		BatchUpdateHelper batchUpdateHelper;
		UpdateActionType updateActions = UpdateActionType.None;
		bool ShouldUpdateViewInfo { get { return this.viewInfoUpdateType != ViewInfoUpdateType.None; } }
		bool ShouldRearrangeViewinfo { get { return this.viewInfoUpdateType == ViewInfoUpdateType.All; } }
		protected virtual bool IsSkinActive { get { return Map.IsSkinActive; } }
		protected internal InnerMap Map { get { return map; } }
		protected MapUnitConverter UnitConverter { get { return Map != null ? Map.UnitConverter : EmptyUnitConverter.Instance; } }
		protected bool IsDesignMode { get { return Map.IsDesignMode; } }
		protected bool IsDesigntimeProcess { get { return Map.IsDesigntimeProcess; } }
		public bool IsInitialized { get { return isInitialized; } }
		internal UpdateActionType UpdateActions { get { return updateActions; } }
		internal MapDeferredUpdates DeferredUpdates { get { return deferredUpdates; } }
		internal MapViewInfo ViewInfo { get { return viewInfo; } }
		protected internal RenderStrategyBase RenderStrategy { get { return renderStrategy; } }
		Rectangle Bounds { get { return RenderStrategy != null ? RenderStrategy.Bounds : Map.ClientRectangle; } }
		internal Graphics Graphics {
			get { return graphics; }
			set {
				if(Object.Equals(graphics, value))
					return;
				MapUtils.DisposeObject(graphics);
				graphics = value;
			}
		}
		internal ViewInfoBuilderBase ViewInfoBuilder {
			get {
				if(viewInfoBuilder == null)
					viewInfoBuilder = new DefaultViewInfoBuilder(this);
				return viewInfoBuilder;
			}
		}
		internal LegendCollection Legends { get { return map.Legends; } }
		internal ICollection<LayerBase> RenderLayers { get { return GetRenderLayers(); } }
		internal ZoomTrackBarController ZoomTrackBarController { get { return zoomTrackBarController; } }
		internal ScrollButtonsController ScrollButtonsController { get { return scrollButtonsController; } }
		internal bool IsRenderSuspend { get { return this.renderSuspendCount > 0; } }
		internal ViewInfoUpdateType ViewInfoUpdateType { get { return viewInfoUpdateType; } }
		internal Dictionary<int, RenderOverlay> RenderOverlaysCache { get { return renderOverlaysCache; } }
		public IMapItemStyleProvider DefaultItemStyleProvider { get { return itemStyleProvider; } }
		public ViewInfoStyleProviderBase DefaultViewInfoStyleProvider { get { return viewInfoStyleProvider; } }
		public MapUIElementsPainter Painter { get { return painter; } }
		public CoordPoint ActivePoint {
			get { return activePoint; }
			set {
				if(activePoint == value)
					return;
				activePoint = value;
				OnActivePointChanged();
			}
		}
		public double KilometersScale {
			get { return kilometersScale; }
			set {
				if(kilometersScale == value)
					return;
				kilometersScale = value;
				OnActivePointChanged();
			}
		}
		IRenderContext IRenderContextProvider.RenderContext { get { return renderContext; } }
		IRenderMiniMapContentProvider IRenderMiniMapProvider.ContentProvider { get { return Map.OperationHelper.CanShowMiniMap() ? Map.MiniMap : null; } }
		IRenderStyleProvider IRenderMiniMapProvider.StyleProvider { get { return Map.OperationHelper.CanShowMiniMap() ? Map.MiniMap : null; } }
		public RenderController(InnerMap map) {
			this.map = map;
			this.deferredUpdates = new MapDeferredUpdates();
			this.painter = CreateUIElementsPainter();
			RecreateViewInfoStyles();
			this.zoomTrackBarController = new ZoomTrackBarController();
			this.scrollButtonsController = new ScrollButtonsController();
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.renderOverlaysCache = new Dictionary<int, RenderOverlay>();
			this.activePoint = map.UnitConverter.PointFactory.CreatePoint(0, 0);
		}
		public void Initialize(IntPtr handle) {
			this.renderStrategy = CreateRenderStrategy();
			RenderStrategy.SetRenderer(CreateRenderer(map.ActualRenderMode, handle));
			ApplyDeferredUpdates();
			DeferredUpdates.Reset();
			this.isInitialized = true;
		}
		protected ICollection<LayerBase> GetRenderLayers() {
			return map.ActualLayers;
		}
		void UiInvoke(Action action) {
			IUIThreadRunner uiRunner = Map as IUIThreadRunner;
			if(uiRunner != null && uiRunner.AllowInvoke)
				uiRunner.BeginInvoke(action);
			else
				action();
		}
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			UiInvoke(() => { batchUpdateHelper.BeginUpdate(); });
		}
		public void EndUpdate() {
			UiInvoke(() => { batchUpdateHelper.EndUpdate(); });
		}
		public void CancelUpdate() {
			UiInvoke(() => { batchUpdateHelper.CancelUpdate(); });
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			ApplyUpdates();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			CancelUpdates();
		}
		#endregion
		#region IBoundingRectUpdater implementation
		bool IBoundingRectUpdater.NeedUpdateBoundingRect {
			get { return Map.CoordinateSystem.NeedUpdateBoundingBox; }
		}
		void IBoundingRectUpdater.EnsureBoundingRect(ICollection<LayerBase> layers) {
			Map.EnsureBoundingRect(layers);
		}
		#endregion
		IRenderer CreateRendererInstance(RenderMode renderMode, IntPtr handle) {
			switch(renderMode) {
				case RenderMode.GdiPlus:
					return CreateGDIRenderer();
				case RenderMode.DirectX:
					return CreateD3DRenderer(handle);
			}
			throw new ArgumentException();
		}
		IRenderer CreateGDIRenderer() {
			return new GdiRenderer();
		}
		Rectangle CalculateNavigationPanelBounds(Rectangle controlBounds) {
			int height = map.NavigationPanelOptions.Height;
			return RectUtils.CutFromTop(controlBounds, controlBounds.Height - height);
		}
		internal Point TranslateToNavPanelPosition(Point mousePosition) {
			Rectangle panelRect = CalculateNavigationPanelBounds(Bounds);
			return new Point(mousePosition.X, mousePosition.Y - panelRect.Y);
		}
		void DisposeViewInfo() {
			if(viewInfo != null) {
				viewInfo.Dispose();
				viewInfo = null;
			}
		}
		void DisposeViewInfoStyleProvider(){
			if(this.viewInfoStyleProvider != null) {
				this.viewInfoStyleProvider.Dispose();
				this.viewInfoStyleProvider = null;
			}
		}
		void UpdateRenderContext(double zoomLevel, CoordPoint centerPoint, Rectangle bounds, Rectangle contentBounds, Color backColor, bool isExport) {
			renderContext = new RenderContext() {
				ZoomLevel = zoomLevel,
				CenterPoint = centerPoint,
				Bounds = bounds,
				ContentBounds = contentBounds,
				BackColor = backColor,
				IsExport = isExport
			};
		}
		internal Graphics CreateGraphics() {
			Graphics gr;
			IMapControl control = (IMapControl)Map.OwnedControl;
			gr = control.CreateGraphics();
			if(gr != null) {
				gr.SetClip(map.ClientRectangle);
				return gr;
			}
			using(Image bitmap = new Bitmap(Map.ContentRectangle.Width, Map.ContentRectangle.Height)) {
				gr = Graphics.FromImage(bitmap);
			}
			return gr;
		}
		void OnClientSizeChanged(Rectangle clientRectangle) {
			RenderStrategy.OnClientSizeChanged(clientRectangle);
		}
		void OnSuspendChange() {
			Pause(IsRenderSuspend, true);
		}
		internal void CreateMapControlViewInfo() {
			DisposeViewInfo();
			this.viewInfo = new MapViewInfo(this.map);
			this.viewInfo.Initialize(Painter);
			this.viewInfoUpdateType = DefaultViewInfoUpdateType;
		}
		void OnActivePointChanged() {
			this.viewInfoUpdateType |= ViewInfoUpdateType.NavigationPanel;
		}
		void DoUpdateStyles() {
			RecreateStyles();
			ExecuteForEachItemsLayer(ResetLayerItemsStyles);
			Map.UpdateMiniMapLookAndFeel();
		}
		void ResetLayerItemsStyles(MapItemsLayerBase itemsLayer) {
			itemsLayer.ResetItemsStyle(MapItemUpdateType.Style);
		}
		void UpdateLayerImageHolders(MapItemsLayerBase itemsLayer) {
			itemsLayer.UpdateImageHolders(Map.ImageList);
		}
		void ClearColoredSkinElementCache(MapItemsLayerBase itemsLayer) {
			itemsLayer.ResetColoredSkinElementCache();
		}
		void ExecuteForEachItemsLayer(Action<MapItemsLayerBase> action) {
			lock(Map.ActualLayers) {
				foreach(LayerBase layer in Map.ActualLayers) {
					MapItemsLayerBase itemsLayer = layer as MapItemsLayerBase;
					if(itemsLayer != null)
						action(itemsLayer);
				}
			}
		}
		void RecreateStyles() {
			RecreateViewInfoStyles();
			RecreateItemStyleProvider();
		}
		internal void RecreateItemStyleProvider() {
			this.itemStyleProvider = CreateStyleProvider();
		}
		internal void RecreateViewInfoStyles() {
			DisposeViewInfoStyleProvider();
			this.viewInfoStyleProvider = CreateViewInfoStyleProvider();
		}
		protected virtual IMapItemStyleProvider CreateStyleProvider() {
			if(IsSkinActive)
				return new SkinMapItemStyleProvider(Map);
			return new MapItemStyleProvider();
		}
		ViewInfoStyleProviderBase CreateViewInfoStyleProvider() {
			if(IsSkinActive)
				return new SkinViewInfoStyleProvider(Map);
			return new ViewInfoStyleProvider();
		}
		void Reset() {
			this.viewInfoUpdateType = DefaultViewInfoUpdateType;
			ResetOverlayCache();
			if(ViewInfo != null)
				ViewInfo.Initialize(Painter);
		}
		void ForceRender() {
			if(IsDisposed)
				return;
			RenderStrategy.ForceRender();
		}
		void ResetUpdateActions() {
			this.updateActions = UpdateActionType.None;
		}
		void CancelUpdates() {
			ResetUpdateActions();
		}
		void ApplyUpdates() {
			if(IsUpdateLocked)
				return;
			if(updateActions == UpdateActionType.None)
				return;
			if(IsUpdateRequired(UpdateActionType.UpdateImages))
				UpdateImages();
			if(IsUpdateRequired(UpdateActionType.ClearSkinElementCache))
				UpdateColoredElementCache();
			if(IsUpdateRequired(UpdateActionType.UpdateStyles))
				UpdateStyles();
			if(IsUpdateRequired(UpdateActionType.ArrangeMiniMap))
				ArrangeMiniMap();
			if(IsUpdateRequired(UpdateActionType.InvalidateViewInfo))
				InvalidateViewInfo();
			if(IsUpdateRequired(UpdateActionType.Render)) {
				ForceRender();
			}
			ResetUpdateActions();
		}
		void UpdateImages() {
			ExecuteForEachItemsLayer(UpdateLayerImageHolders);
		}
		void UpdateColoredElementCache() {
			DoInterlockedAction(() => { ExecuteForEachItemsLayer(ClearColoredSkinElementCache); });
		}
		void UpdateStyles() {
			DoInterlockedAction(() => { DoUpdateStyles(); });
		}
		void InvalidateViewInfo() {
			DoInterlockedAction(() => { Reset(); });
		}
		bool IsUpdateRequired(UpdateActionType action) {
			return (UpdateActions & action) != 0;
		}
		MapPoint CalculateScrollOffset(Point mousePosition, Rectangle bounds) {
			Point pos = TranslateToNavPanelPosition(mousePosition);
			Point center = RectUtils.GetCenter(bounds);
			double maxOffset = ScrollButtonsViewInfo.MaxScrollOffset;
			double offsetX = MathUtils.MinMax((pos.X - center.X) * ScrollButtonsViewInfo.OffsetFactor, -maxOffset, maxOffset);
			double offsetY = MathUtils.MinMax((pos.Y - center.Y) * ScrollButtonsViewInfo.OffsetFactor, -maxOffset, maxOffset);
			return new MapPoint(offsetX, offsetY);
		}
		void Pause(bool suspend, bool shouldInvalidate) {
			if(IsInitialized)
				RenderStrategy.Pause(suspend, shouldInvalidate);
		}
		void DoInterlockedAction(Action action) {
			if (RenderStrategy != null)
				RenderStrategy.DoAction(action);
		}
		protected virtual RenderStrategyBase CreateRenderStrategy() {
			if(IsDesignMode || IsDesigntimeProcess)
				return new DesigntimeRenderStrategy(this);
			return new RuntimeRenderStrategy(this);
		}
		protected virtual MapUIElementsPainter CreateUIElementsPainter() {
			return new MapUIElementsPainter(this);
		}
		protected internal IRenderer CreateRenderer(RenderMode renderMode, IntPtr handle) {
			return CreateRendererInstance(renderMode, handle);
		}
		protected override void DisposeOverride() {
			base.DisposeOverride();
			if(renderStrategy != null) {
				renderStrategy.Dispose();
				renderStrategy = null;
			}
			if(deferredUpdates != null)
				deferredUpdates = null;
			if(graphics != null) {
				graphics.Dispose();
				graphics = null;
			}
			DisposeViewInfoStyleProvider();
			DisposeViewInfo();
			viewInfoBuilder = null;
		}
		protected virtual void ExecuteSafeActionCore(Action action) {
			action();
		}
		#region ViewInfos
		IEnumerable<IRenderOverlay> IRenderOverlayProvider.GetOverlays() {
			if(ShouldUpdateViewInfo) {
				UpdateViewInfos();
				this.viewInfoUpdateType = ViewInfoUpdateType.None;
			}
			return RenderOverlaysCache.Values.ToArray();
		}
		internal void UpdateOverlayCache(OverlayViewInfoBase viewInfo) {
			if(viewInfo == null || IsViewInfoSizeZero(viewInfo))
				return;
			int key = viewInfo.CreateKey();
			RenderOverlay oldValue;
			RenderOverlay overlay = CreateOverlay(viewInfo);
			if(overlay.OverlayImage == null)
				return;
			if(RenderOverlaysCache.TryGetValue(key, out oldValue)) {
				oldValue.Dispose();
			}
			RenderOverlaysCache[key] = overlay;
		}
		bool IsViewInfoSizeZero(OverlayViewInfoBase viewInfo) {
			return (viewInfo.Bounds.Width == 0) || (viewInfo.Bounds.Height == 0);
		}
		void ResetOverlayCache() {
			foreach(RenderOverlay item in RenderOverlaysCache.Values) {
				item.Dispose();
			}
			RenderOverlaysCache.Clear();
		}
		RenderOverlay CreateOverlay(OverlayViewInfoBase viewInfo) {
			RenderOverlay overlay = new RenderOverlay();
			Image overlayImage = CreateOverlayImage(viewInfo);
			Size size = ImageSafeAccess.GetSize(overlayImage);
			overlay.OverlayImage = overlayImage;
			overlay.OverlayImageSize = size;
			overlay.ScreenPosition = viewInfo.Bounds.Location;
			overlay.StoringInPool = viewInfo.CanStore;
			if (viewInfo is NavigationPanelViewInfo)
				overlay.Printable = viewInfo.Printable && Map.PrintOptions.PrintNavigationPanel;
			else
				overlay.Printable = viewInfo.Printable;
			overlay.ShowInDesign = viewInfo.ShowInDesign;
			return overlay;
		}
		protected internal Image CreateOverlayImage(OverlayViewInfoBase viewInfo) {
			Size size = viewInfo.Bounds.Size;
			Bitmap bitmap = MapUtils.TryCreateBitmap(size.Width, size.Height);
			if(bitmap == null)
				return null;
			using(Graphics gr = Graphics.FromImage(bitmap)) {
				using(GraphicsCache cache = new GraphicsCache(gr)) {
					cache.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
					Painter.Draw(cache, viewInfo);
				}
			}
			return bitmap;
		}
		internal void UpdateViewInfos() {
			if(ViewInfo == null)
				return;
			ViewInfoUpdateParams parameters = CreateViewInfoUpdateParams();
			ViewInfoBuilder.Update(ViewInfo, Graphics, parameters);
		}
		ViewInfoUpdateParams CreateViewInfoUpdateParams() {
			ViewInfoUpdateParams p = new ViewInfoUpdateParams();
			p.Bounds = Bounds;
			p.CoordPoint = ActivePoint;
			p.MousePosition = map.MouseHandler.MousePosition;
			p.KilometersScale = KilometersScale;
			p.UpdateType = ViewInfoUpdateType;
			p.ShouldRearrange = ShouldRearrangeViewinfo;
			return p;
		}
		internal void OnViewInfoUpdated() {
			UpdateZoomTrackBarController(map.ZoomLevel);
		}
		#endregion
		internal static double CalculateRenderScale(double zoomLevel) {
			if(zoomLevel < 1.0)
				return zoomLevel;
			return Math.Pow(2.0, Math.Max(0.0, zoomLevel - 1.0));
		}
		internal void UpdateRenderer(RenderMode renderMode, IntPtr handle) {
			if(IsInitialized)
				RenderStrategy.UpdateRenderer(renderMode, handle);
		}
		internal void UpdateRenderContext(bool isExport) {
			if(CanUpdateViewport())
				Map.UpdateViewportRect(false);
			UpdateRenderContext(map.ActualZoomLevel, map.ActualCenterPoint, map.ClientRectangle, map.ContentRectangle, map.BackColor, isExport);
		}
		bool CanUpdateViewport() {
			return !map.ActualZoomLevel.Equals(map.ZoomLevel) || !map.ActualCenterPoint.Equals(map.CenterPoint);
		}
		internal IRenderer CreateD3DRenderer(IntPtr handle) {
			return new D3DRenderer(handle);
		}
		internal IMapUiHitInfo CalcUiHitInfo(Point point) {
			if(ViewInfo != null && ViewInfo.IsReady)
				return ViewInfo.CalcHitInfo(point);
			Rectangle panelRect = CalculateNavigationPanelBounds(Bounds);
			if(panelRect.Contains(point))
				return new MapUiHitInfo(point, MapHitUiElementType.NavigationPanel);
			panelRect = Map.InteractionController.CalculateSearchPanelBounds(Bounds);
			if(panelRect.Contains(point))
				return new MapUiHitInfo(point, MapHitUiElementType.SearchPanel);
			return new MapUiHitInfo(point, MapHitUiElementType.None);
		}
		internal void UpdateZoomLevel(Point mousePos) {
			if(!map.EnableZooming || ViewInfo == null)
				return;
			Rectangle trackBarRect = ZoomTrackBarController.ViewInfoClientBounds;
			int zoomMaxPosition = trackBarRect.Width;
			int pos = (int)MathUtils.MinMax(mousePos.X - trackBarRect.X, 0, zoomMaxPosition);
			zoomTrackBarController.UpdateZoomLevel(pos, zoomMaxPosition, Map.MinZoomLevel, Map.MaxZoomLevel);
			if(map.ZoomLevel != zoomTrackBarController.ZoomLevel) {
				map.Zoom(zoomTrackBarController.ZoomLevel);
				this.viewInfoUpdateType |= ViewInfoUpdateType.NavigationPanel;
			}
		}
		internal Point GetScrollingOffset(Point mousePosition) {
			if(ViewInfo == null)
				return Point.Empty;
			this.viewInfoUpdateType |= ViewInfoUpdateType.NavigationPanel;
			Rectangle scrollButtonsRect = ScrollButtonsController.ViewInfoClientBounds;
			MapPoint offset = CalculateScrollOffset(mousePosition, scrollButtonsRect);
			return offset.ToPoint();
		}
		internal void UpdateVisualState(MapHitUiElementType hitTestType) {
			ViewInfo.UpdateVisualState(hitTestType);
		}
		internal void ResetVisualState() {
			if(!IsInitialized) return;
			ViewInfo.UpdateVisualState(MapHitUiElementType.None);
		}
		internal void UpdateZoomTrackBarController(double zoomLevel) {
			this.zoomTrackBarController.InitZoomLevel(zoomLevel);
		}
		internal void UpdateTrackBarZoomLimits(double minZoomLevel, double maxZoomLevel) {
			this.zoomTrackBarController.UpdateMinMaxZoomLevel(minZoomLevel, maxZoomLevel);
		}
		internal void UpdateSearchPanel() {
			PerformUpdate(UpdateActionType.InvalidateViewInfo | UpdateActionType.Render);
		}
		internal void RegisterOverlaysUpdate() {
			this.viewInfoUpdateType |= ViewInfoUpdateType.CustomOverlay;
		}
		internal void RenderToImage(Image image, Size imageSize, Printing.PrintOptions printOptions) {
			if(!IsInitialized || image == null || imageSize.Width == 0 || imageSize.Height == 0)
				return;
			Pause(true, false);
			try {
				Graphics graphics = Graphics.FromImage(image);
				lock(graphics) {
					CreateMapControlViewInfo();
					UpdateRenderContext(true);
					using (DirectRenderHelper renderHelper = new DirectRenderHelper(this, printOptions)) {
						RenderStrategy.UpdateRenderItems(renderHelper.Renderer, null);
						renderHelper.Render(graphics, new Rectangle(Point.Empty, imageSize));
					}
				}
				if(map.ActualRenderMode != RenderMode.GdiPlus) {
					UpdateRenderItems(null);
				}
			} finally {
				Pause(false, false);
			}
		}
		public void Render(Graphics gr) {
			if(IsDisposed)
				return;
			RenderStrategy.Render(gr);
		}
#if DEBUG
		protected internal double AverageFps {
			get {
				RuntimeRenderStrategy str = RenderStrategy as RuntimeRenderStrategy;
				return str != null ? str.RenderWorker.AverageFps : 0.0;
			}
		}
		internal void UpdateShowDebugInfo(bool value) {
			RenderStrategy.ShowDebugInfo(value);
		}
#endif
		public void UpdateRenderItems(LayerBase updatedLayer) {
			if(IsInitialized)
				RenderStrategy.UpdateRenderItems(RenderStrategy.Renderer, updatedLayer);
			else
				DeferredUpdates.RegisterUpdateRenderItems();
		}
		public void SetClientSize(Rectangle clientRectangle) {
			Size size = clientRectangle.Size;
			Pause(true, false);
			OnClientSizeChanged(clientRectangle);
			map.SetClientSize(size);
			ArrangeMiniMap();
			Pause(false, false);
		}
		void ArrangeMiniMap() {
			MiniMap miniMap = Map.MiniMap;
			if(miniMap == null)
				return;
			miniMap.Bounds = CalculateMiniMapBounds(miniMap);
		}
		internal Rectangle CalculateMiniMapBounds(MiniMap miniMap) {
			ContentAlignment alignment = MapUtils.ConvertToContentAlignment(miniMap.Alignment);
			Rectangle viewBounds = Map.GetViewAreaBounds();
			viewBounds.Inflate(-10, -10);
			return RectUtils.AlignRectangle(new Rectangle(Point.Empty, miniMap.Size), viewBounds, alignment);
		}
		public void PerformUpdate(UpdateActionType update) {
			if(!IsInitialized) {
				DeferredUpdates.RegisterUpdate(update);
				return;
			}
			BeginUpdate();
			try {
				RegisterUpdate(update);
			} finally {
				EndUpdate();
			}
		}
		public void RegisterUpdate(UpdateActionType update) {
			if(!IsInitialized) {
				DeferredUpdates.RegisterUpdate(update);
				return;
			}
			this.updateActions |= update;
		}
		public void SuspendRender() {
			this.renderSuspendCount++;
			OnSuspendChange();
		}
		public void ResumeRender() {
			this.renderSuspendCount--;
			OnSuspendChange();
		}
		public void ExecuteSafeAction(Action<VectorItemsLayer> action, VectorItemsLayer layer) {
			SuspendRender();
			try {
				action(layer);
			} finally {
				ResumeRender();
			}
		}
		public void ExecuteSafeAction(Action action) {
			SuspendRender();
			try {
				ExecuteSafeActionCore(action);
			} finally {
				ResumeRender();
			}
		}
		MapUnit CalcCenterAfterZoomToRegion(Rectangle regionInPixels) {
			MapUnit leftTop = Map.ScreenPointToMapUnit(new MapPoint(regionInPixels.Left, regionInPixels.Top));
			MapUnit rightBottom = Map.ScreenPointToMapUnit(new MapPoint(regionInPixels.Right, regionInPixels.Bottom));
			return new MapUnit((leftTop.X + rightBottom.X) / 2, (leftTop.Y + rightBottom.Y) / 2);
		}
		double CalcZoomAfterZoomToRegion(Rectangle regionInPixels) {
			MapUnit minPoint = Map.ScreenPointToMapUnit(MapPoint.Empty);
			Rectangle contentBounds = renderContext.ContentBounds;
			MapUnit maxPoint = Map.ScreenPointToMapUnit(new MapPoint(contentBounds.Width, contentBounds.Height));
			CoordVector viewPortSize = new CoordVector(maxPoint.X - minPoint.X, maxPoint.Y - minPoint.Y);
			MapUnit leftTop = Map.ScreenPointToMapUnit(new MapPoint(regionInPixels.Left, regionInPixels.Top));
			MapUnit rightBottom = Map.ScreenPointToMapUnit(new MapPoint(regionInPixels.Right, regionInPixels.Bottom));
			double regionWidth = rightBottom.X - leftTop.X;
			double regionHeight = rightBottom.Y - leftTop.Y;
			CoordVector selectedRegionSize = new CoordVector(regionWidth, regionHeight);
			double newZoomLevel = NavigationCalculations.CalculateZoomLevelAfterZoomToRegion(viewPortSize, selectedRegionSize, map.ZoomLevel);
			return newZoomLevel;
		}
		internal void ZoomToRegion(Rectangle regionInPixels) {
			if(regionInPixels.Size == Size.Empty || !Map.OperationHelper.CanScroll() || !Map.OperationHelper.CanZoom())
				return;
			double newZoom = CalcZoomAfterZoomToRegion(regionInPixels);
			MapUnit center = CalcCenterAfterZoomToRegion(regionInPixels);
			Map.Zoom(newZoom, UnitConverter.MapUnitToScreenPoint(center, true));
			Map.CenterPoint = Map.CoordinateSystem.MapUnitToCoordPoint(center);
		}
		internal void UpdateSelectedRegion(Rectangle region) {
			if(IsInitialized)
				RenderStrategy.UpdateSelectedRegion(region);
		}
		internal void RaiseOverlayLayoutCalculated(OverlaysArrangedEventArgs args) {
			Map.RaiseOverlaysArranged(args);
		}
		public void UpdateViewInfoStyle() {
			DefaultViewInfoStyleProvider.Update();
		}
		NavigationPanelAppearance IViewInfoStyleProvider.DefaultNavigationPanelAppearance {
			get { return DefaultViewInfoStyleProvider.DefaultNavigationPanelAppearance; }
		}
		LegendAppearance IViewInfoStyleProvider.DefaultLegendAppearance {
			get { return DefaultViewInfoStyleProvider.DefaultLegendAppearance; }
		}
		ErrorPanelAppearance IViewInfoStyleProvider.DefaultErrorPanelAppearance {
			get { return DefaultViewInfoStyleProvider.DefaultErrorPanelAppearance; }
		}
		SearchPanelAppearance IViewInfoStyleProvider.DefaultSearchPanelAppearance {
			get { return DefaultViewInfoStyleProvider.DefaultSearchPanelAppearance; }
		}
		CustomOverlayAppearance IViewInfoStyleProvider.DefaultCustomOverlayAppearance {
			get { return DefaultViewInfoStyleProvider.DefaultCustomOverlayAppearance; }
		}
		OverlayTextAppearance IViewInfoStyleProvider.DefaultOverlayTextAppearance {
			get { return DefaultViewInfoStyleProvider.DefaultOverlayTextAppearance; }
		}
		bool IRenderStyleProvider.IsSkinActive { get { return IsSkinActive; } }
		SkinElement IRenderStyleProvider.BorderElement {
			get {
				return IsSkinActive && DefaultItemStyleProvider != null ? DefaultItemStyleProvider.MapBorderStyle.BorderElement : null;
			}
		}
		ISkinProvider IRenderStyleProvider.SkinProvider { get { return map; } }
		BorderStyles IRenderStyleProvider.BorderStyle { get { return map.BorderStyle; } }
		Rectangle IRenderStyleProvider.Bounds { get { return map.ClientRectangle; } }
		BorderedElementStyle IRenderStyleProvider.BorderedElementStyle {
			get {
				return DefaultItemStyleProvider != null ? DefaultItemStyleProvider.SelectedRegionStyle : MapItemStyleProvider.Default.SelectedRegionStyle;
			}
		}
		void ApplyDeferredUpdates() {
			if(DeferredUpdates.IsEmpty)
				return;
			BeginUpdate();
			try {
				this.updateActions |= DeferredUpdates.Update;
				if(DeferredUpdates.RenderItemUpdates)
					RenderStrategy.UpdateRenderItems(RenderStrategy.Renderer, null);
			} finally {
				EndUpdate();
			}
		}
	}
}
