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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Map;
using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public enum MiniMapAlignment {
		TopLeft = 1,
		TopRight = 4,
		BottomLeft = 8,
		BottomRight = 16
	}
	public class MiniMap : MapDisposableObject, IOwnedElement, IMapView, IRenderMiniMapContentProvider, IRenderStyleProvider, ISupportUnitConverter, IUIThreadRunner, IServiceProvider, IMapStyleOwner {
		const MiniMapAlignment DefaultAlignment = MiniMapAlignment.BottomLeft;
		const int DefaultWidth = 150;
		const int DefaultHeight = 150;
		const bool DefaultVisible = true;
		static readonly Size DefaultSize = new Size(DefaultWidth, DefaultHeight);
		bool enableZooming = InnerMap.DefaultEnableZooming;
		bool enableScrolling = InnerMap.DefaultEnableScrolling;
		bool visible = DefaultVisible;
		object owner;
		MiniMapController controller;
		MiniMapAlignment alignment = DefaultAlignment;
		MiniMapBehavior behavior;
		Rectangle bounds;
		MapViewportInternal viewport;
		RenderContext renderContext;
		MapUnitConverter unitConverter;
		MiniMapLayerCollection layers;
		SortedLayerCollection actualLayers;
		bool setMapCenterOnClick = true;
		double actualZoomLevel = InnerMap.DefaultZoomLevel;
		CoordPoint actualCenterPoint = InnerMap.DefaultCenterPoint;
		Rectangle viewportInPixels = Rectangle.Empty;
		BorderedElementStyle viewportStyle;
		BorderedElementStyle actualViewportStyle = MapItemStyleProvider.Default.SelectedRegionStyle;
		MapRect RenderBounds { get; set; }
		protected Color ActualBackColor { get { return Map != null ? Map.BackColor : InnerMap.DefaultBackColor; } }
		protected internal MiniMapController Controller { get { return controller; } }
		protected internal InnerMap Map { get { return Owner as InnerMap; } }
		protected internal MapCoordinateSystem CoordinateSystem { get { return Map != null ? Map.CoordinateSystem : InnerMap.DefaultCoordinateSystem; } }
		protected internal MapViewportInternal Viewport { get { return viewport; } }
		protected internal MapUnitConverter UnitConverter {
			get {
				if(unitConverter == null)
					unitConverter = CreateUnitConverter();
				return unitConverter;
			}
		}
		protected RenderContext RenderContext { get { return renderContext; } }
		protected object Owner { get { return owner; } }
		protected IUIThreadRunner ThreadRunner { get { return Map as IUIThreadRunner; } }
		protected internal SortedLayerCollection ActualLayers { get { return actualLayers; } }
		internal double MapZoomLevel { get { return Map != null ? Map.ActualZoomLevel : InnerMap.DefaultZoomLevel; } }
		internal CoordPoint MapCenterPoint { get { return Map != null ? Map.ActualCenterPoint : InnerMap.DefaultCenterPoint; } }
		internal object MapImageList { get { return Map != null ? Map.ImageList : null; } }
		internal double ActualZoomLevel {
			get { return actualZoomLevel; }
			set {
				if(actualZoomLevel == value)
					return;
				actualZoomLevel = value;
				OnActualZoomLevelChanged();
			}
		}
		internal CoordPoint ActualCenterPoint {
			get { return actualCenterPoint; }
			set {
				if(actualCenterPoint == value)
					return;
				actualCenterPoint = value;
				OnActualCenterPointChanged();
			}
		}
		internal Rectangle Bounds {
			get { return bounds; }
			set {
				bounds = value;
				OnBoundsChanged();
			}
		}
		internal Size Size { get { return Bounds.Size; } }
		[
		Category(SRCategoryNames.Map), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraMap.Design.MiniMapLayerCollectionEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor))]
		public MiniMapLayerCollection Layers { get { return layers; } }
		[DefaultValue(null),
		Category(SRCategoryNames.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Editor("DevExpress.XtraMap.Design.MiniMapBehaviorPickerEditor," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign, typeof(UITypeEditor)),
#if !SL
	DevExpressXtraMapLocalizedDescription("MiniMapBehavior"),
#endif
		RefreshProperties(RefreshProperties.All), NotifyParentProperty(true)]
		public MiniMapBehavior Behavior {
			get { return behavior; }
			set {
				if(Object.Equals(behavior, value))
					return;
				UnsubscribeMiniMapBehaviorEvents(behavior);
				behavior = value ?? new DynamicMiniMapBehavior();
				SubscribeMiniMapBehaviorEvents(behavior);
				OnMiniMapBehaviorChanged();
			}
		}
		[Category(SRCategoryNames.Layout),
		DefaultValue(DefaultAlignment),
#if !SL
	DevExpressXtraMapLocalizedDescription("MiniMapAlignment")
#else
	Description("")
#endif
]
		public MiniMapAlignment Alignment {
			get { return alignment; }
			set {
				if(alignment == value)
					return;
				alignment = value;
				OnAlignmentChanged();
			}
		}
		[Category(SRCategoryNames.Layout),
		DefaultValue(DefaultWidth),
#if !SL
	DevExpressXtraMapLocalizedDescription("MiniMapWidth")
#else
	Description("")
#endif
]
		public int Width {
			get { return bounds.Width; }
			set {
				if(bounds.Width != value)
					Bounds = new Rectangle(Bounds.Left, Bounds.Top, Math.Max(1, value), Bounds.Height);
			}
		}
		[Category(SRCategoryNames.Layout),
		DefaultValue(DefaultHeight),
#if !SL
	DevExpressXtraMapLocalizedDescription("MiniMapHeight")
#else
	Description("")
#endif
]
		public int Height {
			get { return bounds.Height; }
			set {
				if(bounds.Height != value)
					Bounds = new Rectangle(Bounds.Left, Bounds.Top, Bounds.Width, Math.Max(1, value));
			}
		}
		[Category(SRCategoryNames.Behavior),
		DefaultValue(true),
#if !SL
	DevExpressXtraMapLocalizedDescription("MiniMapSetMapCenterOnClick")
#else
	Description("")
#endif
]
		public bool SetMapCenterOnClick {
			get { return setMapCenterOnClick; }
			set {
				if(setMapCenterOnClick == value)
					return;
				setMapCenterOnClick = value;
				OnSetMapCenterOnClickChanged();
			}
		}
		[Category(SRCategoryNames.Behavior),
#if !SL
	DevExpressXtraMapLocalizedDescription("MiniMapEnableZooming"),
#endif
		DefaultValue(InnerMap.DefaultEnableZooming)]
		public bool EnableZooming {
			get { return enableZooming; }
			set {
				if(enableZooming != value) {
					enableZooming = value;
					OnEnableZoomingChanged();
				}
			}
		}
		[Category(SRCategoryNames.Behavior),
#if !SL
	DevExpressXtraMapLocalizedDescription("MiniMapEnableScrolling"),
#endif
		DefaultValue(InnerMap.DefaultEnableScrolling)]
		public bool EnableScrolling {
			get { return enableScrolling; }
			set {
				if(enableScrolling != value) {
					enableScrolling = value;
					OnEnableScrollingChanged();
				}
			}
		}
		[Category(SRCategoryNames.Behavior),
#if !SL
	DevExpressXtraMapLocalizedDescription("MiniMapVisible"),
#endif
		DefaultValue(DefaultVisible)]
		public bool Visible {
			get { return visible; }
			set {
				if(visible == value)
					return;
				visible = value;
				OnVisibleChanged();
			}
		}
		[Category(SRCategoryNames.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(ExpandableObjectConverter))]
		public BorderedElementStyle ViewportStyle { get { return viewportStyle; } }
		public MiniMap() {
			this.layers = new MiniMapLayerCollection(this);
			this.actualLayers = new SortedLayerCollection(this);
			viewport = new MapViewportInternal();
			this.behavior = new DynamicMiniMapBehavior();
			SubscribeMiniMapBehaviorEvents(behavior);
			this.controller = CreateController();
			this.renderContext = CreateRenderContext();
			this.viewportStyle = new BorderedElementStyle();
			MapUtils.SetOwner(viewportStyle, this);
			viewport.AnimatedViewportRect = MapViewportInternal.DefaultViewPort;
			Bounds = new Rectangle(Point.Empty, DefaultSize);
			RenderBounds = MapRect.Empty;
		}
		#region IUIThreadRunner
		bool IUIThreadRunner.AllowInvoke {
			get { return ThreadRunner.AllowInvoke; }
		}
		void IUIThreadRunner.BeginInvoke(Action action) {
			ThreadRunner.BeginInvoke(action);
		}
		void IUIThreadRunner.Invoke(Action action) {
			ThreadRunner.Invoke(action);
		}
		#endregion
		#region IRenderMiniMapProvider
		IRenderContext IRenderMiniMapContentProvider.RenderContext { get { return renderContext; } }
		Rectangle IRenderMiniMapContentProvider.ViewportInPixels { get { return viewportInPixels; } }
		IList<LayerBase> IRenderMiniMapContentProvider.Layers { get { return ActualLayers; } }
		#endregion
		#region IRenderStyleProvider
		Rectangle IRenderStyleProvider.Bounds { get { return Bounds; } }
		BorderedElementStyle IRenderStyleProvider.BorderedElementStyle { get { return actualViewportStyle; } }
		bool IRenderStyleProvider.IsSkinActive {
			get { return Map != null ? Map.IsSkinActive : false; }
		}
		Skins.ISkinProvider IRenderStyleProvider.SkinProvider {
			get { return Map; }
		}
		XtraEditors.Controls.BorderStyles IRenderStyleProvider.BorderStyle {
			get { return Map != null ? Map.BorderStyle : XtraEditors.Controls.BorderStyles.Default; }
		}
		Skins.SkinElement IRenderStyleProvider.BorderElement {
			get { return Map != null ? ((IRenderStyleProvider)Map.RenderController).BorderElement : null; }
		}
		#endregion
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			IServiceProvider provider = Owner as IServiceProvider;
			return provider != null ? provider.GetService(serviceType) : null;
		}
		#endregion
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if(owner == value)
					return;
				object oldOwner = owner;
				owner = value;
				OwnerChanged(oldOwner, owner);
			}
		}
		protected virtual void OwnerChanged(object oldOwner, object owner) {
			OnMapUnassigned(oldOwner as InnerMap);
			InnerMap innerMap = owner as InnerMap;
			Controller.SetMap(innerMap);
			UpdateLookAndFeel();
			unitConverter = null;
			OnMapAssigned(innerMap);
			UpdateLayersImages();
			UpdateMap();
		}
		#endregion
		#region IMapView implementation
		double IMapView.ZoomLevel { get { return ActualZoomLevel; } }
		double IMapView.MaxZoomLevel { get { return Map != null ? Map.MaxZoomLevel : InnerMap.DefaultMaxZoomLevel; } }
		CoordPoint IMapView.CenterPoint { get { return ActualCenterPoint; } }
		MapViewportInternal IMapView.Viewport { get { return Viewport; } }
		bool IMapView.ReadyForRender { get { return Map != null && !Map.IsDesignMode; } }
		Size IMapView.InitialMapSize { get { return Map != null ? Map.InitialMapSize : InnerMap.DefaultInitialSize; } }
		bool IMapView.AnimationInProgress { get { return Map != null && Map.AnimationController.InProgress; } }
		MapRect IMapView.RenderBounds { get { return RenderBounds; } }
		IMapItemStyleProvider IMapView.StyleProvider { get { return Map != null ? Map.RenderController.DefaultItemStyleProvider : null; } }
		IMapEventHandler IMapView.EventHandler { get { return Map != null ? Map.EventHandler : null; } }
		MapCoordinateSystem IMapView.CoordinateSystem { get { return Map != null ? Map.CoordinateSystem : InnerMap.DefaultCoordinateSystem; } }
		CoordPoint IMapView.AnchorPoint { get { return Map != null ? Map.AnchorPoint : InnerMap.DefaultCenterPoint; } }
		#endregion
		#region ISupportUnitConverter implementation
		MapUnitConverter ISupportUnitConverter.UnitConverter { get { return UnitConverter; } }
		#endregion
		#region IMapStyleOwner
		void IMapStyleOwner.OnStyleChanged() {
			UpdateStyles();
			RenderMap();
		}
		#endregion
		internal void UpdateRenderContext() {
			this.renderContext = CreateRenderContext();
		}
		internal void UpdateLookAndFeel() {
			UpdateRenderContext();
			UpdateStyles();
		}
		RenderContext CreateRenderContext() {
			return new RenderContext() {
				ZoomLevel = ActualZoomLevel,
				CenterPoint = ActualCenterPoint,
				Bounds = Bounds,
				ContentBounds = Bounds,
				ClipBounds = Bounds,
				BackColor = ActualBackColor,
				IsExport = false
			};
		}
		void UpdateRenderContextBounds() {
			renderContext.Bounds = Bounds;
			renderContext.ContentBounds = Bounds;
			renderContext.ClipBounds = Bounds;
		}
		void UpdateMap() {
			Controller.UpdateActualZoomLevel(MapZoomLevel);
			Controller.UpdateActualCenterPoint(MapCenterPoint);
			UpdateViewportRect();
			RenderMap();
		}
		void UpdateMapViewport() {
			if(Map != null) {
				MapRect viewport = CoordinateSystem.CalculateViewport(Map.ZoomLevel, Map.CenterPoint, Map.CurrentViewport.ViewportInPixels, Map.InitialMapSize);
				MapPoint leftTop = UnitConverter.MapUnitToScreenPoint(new MapUnit(viewport.Left, viewport.Top), false);
				MapPoint rightBottom = UnitConverter.MapUnitToScreenPoint(new MapUnit(viewport.Right, viewport.Bottom), false);
				viewportInPixels = new Rectangle((int)leftTop.X, (int)leftTop.Y, (int)(rightBottom.X - leftTop.X), (int)(rightBottom.Y - leftTop.Y));
				viewportInPixels.Intersect(new Rectangle(0, 0, Viewport.ViewportInPixels.Width, Viewport.ViewportInPixels.Height));
			}
		}
		MapRect CalculateRenderBounds() {
			Rectangle miniMapBounds = Bounds;
			MapUnit p1 = Map.ScreenPointToMapUnit(new MapPoint(miniMapBounds.Left, miniMapBounds.Top));
			MapUnit p2 = Map.ScreenPointToMapUnit(new MapPoint(miniMapBounds.Right, miniMapBounds.Bottom));
			return MapRect.FromLTRB(p1.X, p1.Y, p2.X, p2.Y);
		}
		void OnMiniMapBehaviorChanged() {
			UpdateMap();
		}
		protected void UpdateStyles() {
			BorderedElementStyle style = (Map != null) ? ((IRenderStyleProvider)Map.RenderController).BorderedElementStyle : MapItemStyleProvider.Default.SelectedRegionStyle;
			BorderedElementStyle.MergeStyles(viewportStyle, style, actualViewportStyle);
		}
		protected void InvalidateMap() {
			if(Map != null) Map.ReArrangeMiniMap();
		}
		protected void RenderMap() {
			if(Map != null) Map.Render();
		}
		protected void SetController(MiniMapController controller) {
			this.controller = controller;
		}
		protected override void DisposeOverride() {
			int count = Layers.Count;
			for(int i = count - 1; i >= 0; i--) {
				Layers[i].Dispose();
			}
			Layers.Clear();
			if(controller != null) {
				controller = null;
			}
			if(actualLayers != null) {
				actualLayers.Clear();
				actualLayers = null;
			}
			base.DisposeOverride();
		}
		protected virtual MiniMapController CreateController() {
			return new MiniMapController(this);
		}
		protected virtual MapUnitConverter CreateUnitConverter() {
			return new MapViewUnitConverter(this, Map != null ? Map.CoordinateSystem : InnerMap.DefaultCoordinateSystem);
		}
		protected virtual void UnsubscribeEvents(InnerMap map) {
		}
		protected virtual void SubscribeEvents(InnerMap map) {
		}
		protected virtual void UnsubscribeMiniMapBehaviorEvents(MiniMapBehavior miniMapBehavior) {
			ISupportObjectChanged obj = miniMapBehavior as ISupportObjectChanged;
			if(obj != null) obj.Changed -= OnMiniMapBehaviorPropertiesChanged;
		}
		protected virtual void SubscribeMiniMapBehaviorEvents(MiniMapBehavior miniMapBehavior) {
			ISupportObjectChanged obj = miniMapBehavior as ISupportObjectChanged;
			if(obj != null) obj.Changed += OnMiniMapBehaviorPropertiesChanged;
		}
		protected virtual void OnMiniMapBehaviorPropertiesChanged(object sender, EventArgs e) {
			UpdateMap();
		}
		protected virtual void OnSetMapCenterOnClickChanged() {
		}
		protected virtual void OnEnableScrollingChanged() {
		}
		protected virtual void OnEnableZoomingChanged() {
		}
		protected virtual void OnBoundsChanged() {
			UpdateRenderContextBounds();
			UpdateLayersSize(Size);
			Viewport.ViewportInPixels = Size;
			UpdateViewportRect();
			InvalidateMap();
		}
		void UpdateLayersSize(Size size) {
			Layers.ForEach(d => d.SetClientSize(size));
		}
		void UpdateLayersImages() { 
			Layers.ForEach(d => d.UpdateLayersImages(MapImageList));
		}
		void UpdateLayersViewport() {
			Layers.ForEach(d => d.ViewportUpdated());
		}
		protected void OnVisibleChanged() {
			RenderMap();
		}
		protected virtual void OnActualCenterPointChanged() {
			renderContext.CenterPoint = ActualCenterPoint;
			UpdateMapViewport();
		}
		protected virtual void OnActualZoomLevelChanged() {
			renderContext.ZoomLevel = ActualZoomLevel;
			UpdateMapViewport();
		}
		protected virtual void OnAlignmentChanged() {
			InvalidateMap();
		}
		protected virtual void OnMapAssigned(InnerMap map) {
			if(map != null) {
				SubscribeEvents(map);
				map.AddNotificationObserver(Controller);
			}
		}
		protected virtual void OnMapUnassigned(InnerMap oldMap) {
			if(oldMap != null) {
				UnsubscribeEvents(oldMap);
				oldMap.RemoveNotificationObserver(Controller);
			}
		}
		protected internal void UpdateViewportRect() {
			if(Map != null) {
				UpdateMapViewport();
				Viewport.AnimatedViewportRect = CoordinateSystem.CalculateViewport(ActualZoomLevel, ActualCenterPoint, Size, Map.InitialMapSize);
				UpdateLayersViewport();
				RenderBounds = CalculateRenderBounds();
			}
		}
		internal void Shift(Point offset) {
			Controller.Shift(offset);
		}
		internal void UpdateCenterPoint(Point point) {
			Controller.UpdateMapCenterPoint(point);
		}
		public override string ToString() {
			return "(MiniMap)";
		}
		internal void OnLayerInsert(MiniMapLayerBase value) {
			if(Map == null)  {
				ActualLayers.Add(value.InnerLayer);
				return;
			}
			Map.ExecuteSafeRenderAction(() => {
				lock(actualLayers) {
					actualLayers.Add(value.InnerLayer);
				}
				if (value is MiniMapImageTilesLayer)
					ResetErrorPanel();
			});
		}
		internal void OnLayerRemove(MiniMapLayerBase value) {
			if(Map == null) {
				ActualLayers.Remove(value.InnerLayer);
				return;
			}
			Map.ExecuteSafeRenderAction(() => {
				lock(actualLayers) {
					actualLayers.Remove(value.InnerLayer);
				}
			});
		}
		internal void OnLayersClear() {
			if(Map == null) {
				ActualLayers.Clear();
				return;
			}
			Map.ExecuteSafeRenderAction(() => {
				lock(actualLayers) {
					actualLayers.Clear();
				}
			});
		}
		internal void RecreateActualLayers(MiniMapLayerCollection miniMapLayerCollection) {
		}
		internal void ResetErrorPanel() {
			if (Map != null)
				Map.ResetMiniMapErrorPanel();
		}
		internal void OnLayerInsertComplete() {
			UpdateLayersSize(Size);
		}
	}
}
